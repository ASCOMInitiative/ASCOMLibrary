using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ASCOM.Alpaca.Tests
{
    /// <summary>
    /// Minimal in-process HTTP server that serves the three Alpaca management API endpoints:
    ///   /management/apiversions
    ///   /management/v1/description
    ///   /management/v1/configureddevices
    ///
    /// The JSON "Value" property name in every response uses the casing supplied at construction
    /// time, which lets tests drive any casing through the <see cref="ASCOM.Alpaca.Discovery.AlpacaDiscovery"/>
    /// HTTP-response parser without needing a real Alpaca device on the network.
    ///
    /// The server uses a raw <see cref="TcpListener"/> (rather than <see cref="System.Net.HttpListener"/>)
    /// so that no URL ACL registration is required on Windows.  Each HTTP/1.1 response carries
    /// <c>Connection: close</c>, which causes <see cref="System.Net.Http.HttpClient"/> to open a
    /// fresh TCP connection for each of the three management API requests.
    /// </summary>
    internal sealed class HttpManagementApiServer : IDisposable
    {
        private readonly TcpListener _listener;
        private readonly string _valueCasing;
        private volatile bool _disposed;

        /// <summary>
        /// Start the server listening on all local interfaces on <paramref name="port"/>,
        /// using <paramref name="valueCasing"/> as the JSON property name for the "Value" key.
        /// </summary>
        public HttpManagementApiServer(int port, string valueCasing)
        {
            _valueCasing = valueCasing;
            _listener = new TcpListener(IPAddress.Any, port);
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

        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                using (client)
                {
                    NetworkStream stream = client.GetStream();
                    byte[] buf = new byte[8192];
                    var headerText = new StringBuilder();

                    // Read bytes until the HTTP header terminator \r\n\r\n is seen.
                    while (!headerText.ToString().Contains("\r\n\r\n"))
                    {
                        int n = await stream.ReadAsync(buf, 0, buf.Length);
                        if (n == 0) return;
                        headerText.Append(Encoding.ASCII.GetString(buf, 0, n));
                    }

                    // Extract the request path from the first header line.
                    string[] lines = headerText.ToString().Split('\n');
                    string[] parts = lines[0].Trim().Split(' ');
                    if (parts.Length < 2) return;
                    string path = parts[1];

                    string body = BuildJsonBody(path);
                    if (body == null)
                    {
                        byte[] notFound = Encoding.ASCII.GetBytes(
                            "HTTP/1.1 404 Not Found\r\nContent-Length: 0\r\nConnection: close\r\n\r\n");
                        await stream.WriteAsync(notFound, 0, notFound.Length);
                        return;
                    }

                    byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
                    byte[] responseHeader = Encoding.ASCII.GetBytes(
                        $"HTTP/1.1 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n" +
                        $"Content-Length: {bodyBytes.Length}\r\nConnection: close\r\n\r\n");
                    await stream.WriteAsync(responseHeader, 0, responseHeader.Length);
                    await stream.WriteAsync(bodyBytes, 0, bodyBytes.Length);
                }
            }
            catch { }
        }

        private string BuildJsonBody(string path)
        {
            if (path.StartsWith("/management/apiversions"))
                return $"{{\"{_valueCasing}\": [1]}}";

            if (path.StartsWith("/management/v1/description"))
                return $"{{\"{_valueCasing}\": {{\"ServerName\": \"TestServer\", \"Manufacturer\": \"TestMfr\", \"ManufacturerVersion\": \"1.0.0\", \"Location\": \"TestLoc\"}}}}";

            if (path.StartsWith("/management/v1/configureddevices"))
                return $"{{\"{_valueCasing}\": [{{\"DeviceName\": \"TestCamera\", \"DeviceType\": \"Camera\", \"DeviceNumber\": 0, \"UniqueID\": \"test-guid-0001\"}}]}}";

            return null;
        }

        public void Dispose()
        {
            _disposed = true;
            _listener.Stop();
        }
    }
}
