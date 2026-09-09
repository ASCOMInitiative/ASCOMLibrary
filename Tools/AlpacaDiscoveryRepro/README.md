# Alpaca discovery issue #43 reproduction

These scripts reproduce the Linux UDP discovery problem reported in
[ASCOMLibrary issue #43](https://github.com/ASCOMInitiative/ASCOMLibrary/issues/43).
They are intentionally small discovery-only responders; no third-party Python
packages or REST API implementation is required.

`server_a.py` and `server_b.py` are separate processes. Each binds IPv4 UDP
port `32227` after setting both `SO_REUSEADDR` and `SO_REUSEPORT`, and returns a
different `AlpacaPort` value so that each response can be identified.

## Run the reproduction

Run these commands on the Linux host that has the Linux
`ASCOM.Alpaca.Simulators` executable:

```text
cd Tools/AlpacaDiscoveryRepro
python3 server_a.py
```

In a second terminal:

```text
cd Tools/AlpacaDiscoveryRepro
python3 server_b.py
```

Start the simulator in a third terminal. The simulator should use its normal
Alpaca port `32323` and report that discovery is running on port `32227`.
Starting the two Python processes before the simulator makes the
`SO_REUSEPORT` sharing arrangement explicit; restart the simulator after both
Python processes are listening if it was already running.

In a fourth terminal, run the probe:

```text
cd Tools/AlpacaDiscoveryRepro
python3 probe.py
```

The probe sends ten `alpacadiscovery1` messages to both `127.0.0.1` and
`255.255.255.255`. It exits successfully only when both Python response ports
are seen at least once and the simulator response port is never seen. The
per-destination counts can vary because `SO_REUSEPORT` load-balances unicast
packets between the two Python sockets. The expected result is similar to:

```text
127.0.0.1:32227
  AlpacaPort 32323: 0 response(s)
  AlpacaPort 32324: 10 response(s)
  AlpacaPort 32325: 0 response(s)
255.255.255.255:32227
  AlpacaPort 32323: 0 response(s)
  AlpacaPort 32324: 10 response(s)
  AlpacaPort 32325: 10 response(s)
REPRODUCED: both SO_REUSEPORT servers replied, but the simulator did not reply.
```

If the simulator uses another Alpaca port, pass it to the probe:

```text
python3 probe.py --simulator-port 32330
```

This is a Linux-specific reproduction. The relevant behavior is the kernel's
UDP delivery choice when `SO_REUSEPORT` listeners share a port with a listener
that only sets `SO_REUSEADDR`.
