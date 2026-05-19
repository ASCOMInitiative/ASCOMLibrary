using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ASCOM.Alpaca.Tests
{
    /// <summary>
    /// Minimal in-process HTTP server that responds to Alpaca client GET requests for a
    /// Telescope/0 device hosted at 127.0.0.1 on a configurable port.
    ///
    /// The JSON "Value" property name in every response uses the casing supplied at construction
    /// time, which allows tests to verify case-sensitive JSON parsing in
    /// <see cref="ASCOM.Alpaca.Clients.AlpacaTelescope"/> without requiring a real device.
    ///
    /// The server uses a raw <see cref="TcpListener"/> bound to loopback to avoid the URL ACL
    /// registration that <see cref="System.Net.HttpListener"/> requires on Windows.
    /// Each response carries <c>Connection: close</c> so that
    /// <see cref="System.Net.Http.HttpClient"/> does not attempt to pipeline further requests.
    ///
    /// The Alpaca client constructs the altitude URL as:
    ///   http://127.0.0.1:{port}/api/v1/telescope/0/altitude
    /// (all lower-cased by <c>RemoteDevice.GetValue</c> before the request is sent).
    /// Query-string parameters added by the client (e.g. ClientID, ClientTransactionID) are
    /// accepted but ignored; the server always returns the same fixed body.
    /// </summary>
    internal sealed class AlpacaTelescopeServer : IDisposable
    {
        private readonly TcpListener _listener;
        private readonly string _valueCasing;
        private readonly double _altitudeValue;
        private volatile bool _disposed;

        /// <summary>
        /// Start the server on loopback port <paramref name="port"/> using
        /// <paramref name="valueCasing"/> as the JSON key name and
        /// <paramref name="altitudeValue"/> as the altitude to embed in the response.
        /// </summary>
        public AlpacaTelescopeServer(int port, string valueCasing, double altitudeValue)
        {
            _valueCasing = valueCasing;
            _altitudeValue = altitudeValue;
            _listener = new TcpListener(IPAddress.Loopback, port);
            _listener.Start();
            _ = AcceptLoopAsync();
        }

        private async Task AcceptLoopAsync()
        {
            while (!_disposed)
            {
                try
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    _ = HandleClientAsync(client);
                }
                catch (SocketException) when (_disposed) { }
                catch (ObjectDisposedException) when (_disposed) { }
            }
        }

        private async Task HandleClientAsync(TcpClient tcpClient)
        {
            try
            {
                using (tcpClient)
                {
                    NetworkStream stream = tcpClient.GetStream();
                    byte[] buf = new byte[8192];
                    var headerText = new StringBuilder();

                    // Read bytes until the HTTP header terminator \r\n\r\n is seen.
                    while (!headerText.ToString().Contains("\r\n\r\n"))
                    {
                        int n = await stream.ReadAsync(buf, 0, buf.Length);
                        if (n == 0) return;
                        headerText.Append(Encoding.ASCII.GetString(buf, 0, n));
                    }

                    string body = BuildJsonBody();
                    byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
                    byte[] header = Encoding.ASCII.GetBytes(
                        $"HTTP/1.1 200 OK\r\n" +
                        $"Content-Type: application/json; charset=utf-8\r\n" +
                        $"Content-Length: {bodyBytes.Length}\r\n" +
                        $"Connection: close\r\n\r\n");
                    await stream.WriteAsync(header, 0, header.Length);
                    await stream.WriteAsync(bodyBytes, 0, bodyBytes.Length);
                }
            }
            catch { }
        }

        private string BuildJsonBody()
        {
            // Embed the altitude value with InvariantCulture to produce a valid JSON number (dot decimal).
            string altitudeStr = _altitudeValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            return $"{{\"ClientTransactionID\": 1, \"ServerTransactionID\": 1, \"ErrorNumber\": 0, \"ErrorMessage\": \"\", \"{_valueCasing}\": {altitudeStr}}}";
        }

        public void Dispose()
        {
            _disposed = true;
            _listener.Stop();
        }
    }
}
