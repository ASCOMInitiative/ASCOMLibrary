using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ASCOM.Alpaca.Discovery;

namespace ASCOM.Alpaca.Tests
{
    /// <summary>
    /// In-process UDP responder used by Finder casing tests.
    /// Listens on the specified discovery port and replies to every Alpaca discovery broadcast
    /// with a JSON payload whose property name and port value are supplied at construction time.
    /// This lets individual tests drive arbitrary JSON key casings without needing an external process.
    /// </summary>
    internal sealed class UdpTestResponder : IDisposable
    {
        private readonly UdpClient _client;
        private readonly byte[] _responseBytes;
        private volatile bool _disposed;

        /// <summary>
        /// Create a responder that listens on <paramref name="discoveryPort"/> and replies with
        /// <c>{"&lt;jsonPropertyName&gt;": &lt;alpacaPort&gt;}</c>.
        /// </summary>
        public UdpTestResponder(int discoveryPort, string jsonPropertyName, int alpacaPort)
        {
            string json = $"{{\"{jsonPropertyName}\": {alpacaPort}}}";
            _responseBytes = Encoding.ASCII.GetBytes(json);

            _client = new UdpClient();
            _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _client.ExclusiveAddressUse = false;
            _client.EnableBroadcast = true;
            _client.Client.Bind(new IPEndPoint(IPAddress.Any, discoveryPort));
            _client.BeginReceive(ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            if (_disposed)
                return;

            try
            {
                var remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] received = _client.EndReceive(ar, ref remoteEndpoint);
                string message = Encoding.ASCII.GetString(received);

                if (message.Contains(Constants.DiscoveryMessage))
                    _client.Send(_responseBytes, _responseBytes.Length, remoteEndpoint);
            }
            catch (ObjectDisposedException) { }
            catch { }
            finally
            {
                if (!_disposed)
                {
                    try { _client.BeginReceive(ReceiveCallback, null); }
                    catch (ObjectDisposedException) { }
                }
            }
        }

        public void Dispose()
        {
            _disposed = true;
            _client.Dispose();
        }
    }
}
