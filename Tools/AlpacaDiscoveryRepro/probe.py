"""Send repeated Alpaca discovery probes and summarize the responses."""

import argparse
import json
import socket
import time
from collections import Counter


DISCOVERY_MESSAGE = b"alpacadiscovery1"
DESTINATIONS = ("127.0.0.1", "255.255.255.255")


def collect_responses(client, timeout, counts):
    deadline = time.monotonic() + timeout

    while True:
        remaining = deadline - time.monotonic()
        if remaining <= 0:
            return

        client.settimeout(remaining)
        try:
            message, endpoint = client.recvfrom(4096)
        except (socket.timeout, ConnectionResetError, ConnectionRefusedError):
            return

        try:
            payload = json.loads(message.decode("ascii"))
            alpaca_port = payload["AlpacaPort"]
            if not isinstance(alpaca_port, int):
                continue
        except (UnicodeDecodeError, json.JSONDecodeError, KeyError, TypeError):
            print(
                f"Ignoring non-Alpaca response from {endpoint[0]}:{endpoint[1]}: "
                f"{message!r}",
                flush=True,
            )
            continue

        counts[alpaca_port] += 1


def main():
    parser = argparse.ArgumentParser(
        description="Probe Alpaca discovery over loopback and IPv4 broadcast."
    )
    parser.add_argument(
        "--discovery-port",
        type=int,
        default=32227,
        help="Alpaca discovery UDP port (default: 32227)",
    )
    parser.add_argument(
        "--simulator-port",
        type=int,
        default=32323,
        help="Simulator Alpaca port used in the result summary (default: 32323)",
    )
    parser.add_argument(
        "--server-a-port",
        type=int,
        default=32324,
        help="Port returned by server A (default: 32324)",
    )
    parser.add_argument(
        "--server-b-port",
        type=int,
        default=32325,
        help="Port returned by server B (default: 32325)",
    )
    parser.add_argument(
        "--attempts",
        type=int,
        default=10,
        help="Probes sent to each destination (default: 10)",
    )
    parser.add_argument(
        "--timeout",
        type=float,
        default=0.25,
        help="Seconds to collect replies after each probe (default: 0.25)",
    )
    parser.add_argument(
        "--interval",
        type=float,
        default=0.05,
        help="Seconds between probes (default: 0.05)",
    )
    args = parser.parse_args()

    if args.attempts < 1:
        parser.error("--attempts must be at least 1")
    if args.timeout <= 0:
        parser.error("--timeout must be greater than 0")
    if args.interval < 0:
        parser.error("--interval must not be negative")

    results = {}
    client = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    client.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)
    client.bind(("0.0.0.0", 0))

    try:
        print(
            f"Using UDP source port {client.getsockname()[1]} for "
            f"{args.attempts} probes per destination.",
            flush=True,
        )

        for destination in DESTINATIONS:
            counts = Counter()
            results[destination] = counts

            for _ in range(args.attempts):
                client.sendto(
                    DISCOVERY_MESSAGE, (destination, args.discovery_port)
                )
                collect_responses(client, args.timeout, counts)
                if args.interval:
                    time.sleep(args.interval)

            print(f"{destination}:{args.discovery_port}", flush=True)
            for port in (
                args.simulator_port,
                args.server_a_port,
                args.server_b_port,
            ):
                print(f"  AlpacaPort {port}: {counts[port]} response(s)", flush=True)
    finally:
        client.close()

    simulator_responses = sum(
        counts[args.simulator_port] for counts in results.values()
    )
    sibling_responses = sum(
        counts[args.server_a_port] + counts[args.server_b_port]
        for counts in results.values()
    )

    python_ports_seen = all(
        sum(counts[port] for counts in results.values()) > 0
        for port in (args.server_a_port, args.server_b_port)
    )

    if simulator_responses == 0 and results and python_ports_seen:
        print(
            "REPRODUCED: both SO_REUSEPORT servers replied, but the simulator "
            "did not reply.",
            flush=True,
        )
        return 0

    print(
        "Not reproduced: expected both Python servers to reply and the "
        f"simulator to remain silent (Python responses: {sibling_responses}, "
        f"simulator responses: {simulator_responses}).",
        flush=True,
    )
    return 1


if __name__ == "__main__":
    raise SystemExit(main())
