"""Shared implementation for the issue #43 discovery test servers."""

import argparse
import json
import socket


DISCOVERY_MESSAGE = b"alpacadiscovery1"


def run(server_name, default_alpaca_port):
    parser = argparse.ArgumentParser(
        description="Run a minimal Alpaca discovery responder using SO_REUSEPORT."
    )
    parser.add_argument(
        "--bind-address",
        default="0.0.0.0",
        help="IPv4 address on which to listen (default: 0.0.0.0)",
    )
    parser.add_argument(
        "--discovery-port",
        type=int,
        default=32227,
        help="Alpaca discovery UDP port (default: 32227)",
    )
    parser.add_argument(
        "--alpaca-port",
        type=int,
        default=default_alpaca_port,
        help="Port returned in the discovery response (default: %(default)s)",
    )
    args = parser.parse_args()

    response = json.dumps({"AlpacaPort": args.alpaca_port}).encode("ascii")
    client = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

    try:
        client.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)

        if not hasattr(socket, "SO_REUSEPORT"):
            raise RuntimeError("This test requires a platform with SO_REUSEPORT.")

        client.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEPORT, 1)
        if client.getsockopt(socket.SOL_SOCKET, socket.SO_REUSEPORT) != 1:
            raise RuntimeError("The socket did not enable SO_REUSEPORT.")

        client.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)
        client.bind((args.bind_address, args.discovery_port))
    except (OSError, RuntimeError):
        client.close()
        raise

    print(
        f"{server_name} listening on UDP "
        f"{args.bind_address}:{args.discovery_port} "
        f"(SO_REUSEADDR + SO_REUSEPORT), returning {response.decode('ascii')}",
        flush=True,
    )

    try:
        while True:
            message, endpoint = client.recvfrom(4096)
            if DISCOVERY_MESSAGE not in message:
                continue

            client.sendto(response, endpoint)
            print(
                f"{server_name} replied to {endpoint[0]}:{endpoint[1]} "
                f"with {response.decode('ascii')}",
                flush=True,
            )
    except KeyboardInterrupt:
        print(f"{server_name} stopped.", flush=True)
    finally:
        client.close()
