using ASCOM.Common;
using ASCOM.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace ASCOM.Alpaca.Discovery
{
    /// <summary>
    /// Initiates a discovery and raises events as devices respond
    /// </summary>
    public class Finder : IDisposable
    {
        private readonly bool strictCasing = true; // Default to strict casing of the AlpacaPort JSON variable name
        private readonly ILogger logger; // Optional logger
        private int discoveryPort = Constants.DiscoveryPort; // Default to the standard discovery port
        private readonly Dictionary<IPAddress, UdpClient> IPv4Clients = new Dictionary<IPAddress, UdpClient>(); // Collection of IP v4 clients for the various link local and localhost networks
        private readonly Dictionary<IPAddress, UdpClient> IPv6Clients = new Dictionary<IPAddress, UdpClient>(); // Collection of IP v6 clients for the various link local and localhost networks
        private bool disposedValue; // Disposed variable
        private readonly object broadcastResponsesLockObject = new object();

        private const int SIO_UDP_CONNRESET = -1744830452; //Control code to turn off UDP ICMP Connection Reset

        #region Initialisation and Dispose

        /// <summary>
        /// Creates a Alpaca Finder object that sends out a search request for Alpaca devices
        /// The results will be sent to the callback and stored in the cache
        /// Calling search and concatenating the results reduces the chance that a UDP packet is lost
        /// This may require firewall access
        /// </summary>
        public Finder()
        {

        }

        /// <summary>
        /// Creates a Alpaca Finder object that sends out a search request for Alpaca devices
        /// The results will be sent to the callback and stored in the cache
        /// Calling search and concatenating the results reduces the chance that a UDP packet is lost
        /// This may require firewall access
        /// </summary>
        /// <param name="strictCasing">Enforce correct casing of the port variable name in the Alpaca JSON discovery response.</param>
        /// <param name="logger">An ILogger instance (or null) into which the Finder will log.</param>
        public Finder(bool strictCasing, ILogger logger) : this()
        {
            this.strictCasing = strictCasing; // Save the strict JSON de-serialisation casing state
            this.logger = logger; // Save the trace logger object
            LogMessage("Finder", "Initialised");
        }

        /// <summary>
        /// Internal Dispose call used by the CLR, do not call directly, use the <see cref="Dispose()"/> method instead.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //Dispose IPv4
                    foreach (var dev in IPv4Clients)
                    {
                        try
                        {
                            dev.Value.Dispose();
                        }
                        catch
                        {
                        }
                    }

                    IPv4Clients.Clear();

                    //Dispose IPv6 clients
                    foreach (var dev in IPv6Clients)
                    {
                        try
                        {
                            dev.Value.Dispose();
                        }
                        catch
                        {
                        }
                    }

                    IPv6Clients.Clear();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// DIspose of the Finder
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put clean-up code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Public Events and Methods

        /// <summary>
        /// Event raised each time a discovery response is received from a device
        /// </summary>
        public event EventHandler<IPEndPoint> ResponseReceivedEvent;

        /// <summary>
        /// Resends the search request on the default discovery port
        /// </summary>
        /// <param name="IPv4">Set true to discover IPv4 Alpaca devices</param>
        /// <param name="IPv6">Set true to discover IPv6 Alpaca devices</param>
        /// <exception cref="ArgumentException"></exception>
        public void Search(bool IPv4 = true, bool IPv6 = true)
        {
            if (!IPv4 && !IPv6)
            {
                throw new ArgumentException("You must search on one or more protocol types.");
            }
            Search(Constants.DiscoveryPort, IPv4, IPv6);
        }

        /// <summary>
        /// Resends the search request on the specified discovery port
        /// </summary>
        /// <param name="discoveryPort">IP port number on which to send the IPv4 discovery broadcast / IPv6 multicast</param>
        /// <param name="IPv4">Set true to discover IPv4 Alpaca devices</param>
        /// <param name="IPv6">Set true to discover IPv6 Alpaca devices</param>
        /// <exception cref="ArgumentException"></exception>
        public void Search(int discoveryPort, bool IPv4 = true, bool IPv6 = true)
        {
            if (!IPv4 && !IPv6)
            {
                throw new ArgumentException("You must search on one or more protocol types.");
            }
            SendDiscoveryMessage(discoveryPort, IPv4, IPv6);
        }

        /// <summary>
        /// List of IP Endpoints that returned valid Alpaca discovery responses
        /// </summary>
        public List<IPEndPoint> CachedEndpoints
        {
            get;
        } = new List<IPEndPoint>();

        /// <summary>
        /// List of all responses to the broadcasts
        /// </summary>
        public List<BroadcastResponse> BroadcastResponses
        {
            get;
        } = new List<BroadcastResponse>();

        /// <summary>
        /// Clears the cached IP Endpoints in CachedEndpoints
        /// </summary>
        public void ClearCache()
        {
            CachedEndpoints.Clear();
        }

        #endregion

        #region Discovery Management

        // This dual targets NetStandard 2.0 and NetFX 3.5 so no Async Await
        // This callback is shared between IPv4 and IPv6
        private void ReceiveCallback(IAsyncResult ar)
        {
            IPEndPoint endpoint = null;
            UdpClient udpClient = null;
            try
            {
                udpClient = (UdpClient)ar.AsyncState;

                endpoint = new IPEndPoint(IPAddress.Any, discoveryPort);

                // Obtain the UDP message body as a byte[]
                byte[] returnedBytes = udpClient.EndReceive(ar, ref endpoint);

                // Save the broadcast response in a thread safe manner
                lock (broadcastResponsesLockObject)
                {
                    BroadcastResponses.Add(new BroadcastResponse(endpoint, returnedBytes));
                }

                // Convert the message bytes to a string, with remote IP address attached as well
                string ReceiveString = Encoding.ASCII.GetString(returnedBytes);
                LogMessage($"ReceiveCallback", $"Received {ReceiveString} from Alpaca device at {endpoint.Address}");

                // Accept responses containing the discovery response string and don't respond to your own transmissions
                if (ReceiveString.ToLowerInvariant().Contains(Constants.ResponseString.ToLowerInvariant())) // Accept responses in any casing so that bad casing can be reported
                {
                    int port = JsonSerializer.Deserialize<AlpacaDiscoveryResponse>(ReceiveString, new JsonSerializerOptions { PropertyNameCaseInsensitive = strictCasing }).AlpacaPort;

                    if (port == 0) //Failed to parse
                    {
                        throw new Exception($"Failed to parse {ReceiveString} into an Alpaca Port");
                    }

                    var alpacaEndpoint = new IPEndPoint(endpoint.Address, port);
                    if (!CachedEndpoints.Contains(alpacaEndpoint))
                    {
                        LogMessage("ReceiveCallback", $"Received new Alpaca API endpoint: {alpacaEndpoint} from broadcast endpoint: {endpoint}");

                        CachedEndpoints.Add(alpacaEndpoint);

                        ResponseReceivedEvent?.Invoke(this, alpacaEndpoint);
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // Ignore these because they occur naturally when the current BeginReceiveAsync is terminated because the UdpClient is closed or disposed.
            }

            catch (Exception ex)
            {
                logger?.LogError($"Failed to parse response from {endpoint} with exception: {ex.Message}");
                LogMessage("ReceiveCallback", $"Failed to parse response from {endpoint}: {ex}");

            }
            finally
            {
                try
                {
                    // Configure the UdpClient class to accept more messages, if they arrive
                    udpClient?.BeginReceive(new AsyncCallback(ReceiveCallback), udpClient);
                }
                catch (ObjectDisposedException)
                {
                    // Also ignore these here because they occur naturally when the current BeginReceiveAsync is terminated because the UdpClient is closed or disposed.
                }
                catch (Exception ex)
                {
                    logger?.LogError($"Error restarting search: {ex.Message}");
                    LogMessage("ReceiveCallback", $"Error restarting search: {ex}");
                }
            }
        }

        /// <summary>
        /// Send out discovery message on each IPv4 broadcast address
        /// This dual targets NetStandard 2.0 and NetFX 3.5 so no Async Await
        /// Broadcasts on each adapters address as per Windows / Linux documentation
        /// </summary>
        private void SearchIPv4()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            LogMessage("SearchIPv4", $"Sending IPv4 discovery broadcasts");

            foreach (NetworkInterface adapter in adapters)
            {
                try
                {
                    //Do not try and use non-operational adapters
                    if (adapter.OperationalStatus != OperationalStatus.Up)
                        continue;

                    if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                    {
                        IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                        if (adapterProperties == null)
                            continue;
                        UnicastIPAddressInformationCollection uniCast = adapterProperties.UnicastAddresses;
                        if (uniCast.Count > 0)
                        {
                            foreach (UnicastIPAddressInformation uni in uniCast)
                            {
                                try
                                {
                                    if (uni.Address.AddressFamily != AddressFamily.InterNetwork)
                                        continue;

                                    if (uni.IPv4Mask == IPAddress.Parse("255.255.255.255"))
                                    {
                                        //No broadcast on single device endpoint
                                        continue;
                                    }

                                    if (!IPv4Clients.ContainsKey(uni.Address))
                                    {
                                        IPv4Clients.Add(uni.Address, NewIPv4Client());
                                    }

                                    if (!IPv4Clients[uni.Address].Client.IsBound)
                                    {
                                        IPv4Clients.Remove(uni.Address);
                                        continue;
                                    }

                                    // Local host addresses (127.*.*.*) may have a null mask in Net Framework. We do want to search these. The correct mask is 255.0.0.0.
                                    IPv4Clients[uni.Address].Send(Constants.DiscoveryMessageArray, Constants.DiscoveryMessageArray.Length, new IPEndPoint(GetBroadcastAddress(uni.Address, uni.IPv4Mask ?? IPAddress.Parse("255.0.0.0")), discoveryPort));
                                    LogMessage("SearchIPv4", $"Sent broadcast to: {uni.Address}");

                                }
                                catch (Exception ex)
                                {
                                    logger?.LogError(ex.Message);
                                    LogMessage("SearchIPv4", $"Exception: {ex}");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex.Message);
                    LogMessage("SearchIPv4", $"Exception: {ex}");
                }
            }
        }

        private UdpClient NewIPv4Client()
        {
            var client = new UdpClient
            {
                EnableBroadcast = true,
                MulticastLoopback = false
            };

            //Fix for ICMP Reset
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                client.Client.IOControl((IOControlCode)SIO_UDP_CONNRESET, new byte[] { 0, 0, 0, 0 }, null);
            }

            //0 tells OS to give us a free ephemeral port
            client.Client.Bind(new IPEndPoint(IPAddress.Any, 0));

            client.BeginReceive(new AsyncCallback(ReceiveCallback), client);

            return client;
        }

        /// <summary>
        /// Send out discovery message on the IPv6 multicast group
        /// This dual targets NetStandard 2.0 and NetFX 3.5 so no Async Await
        /// </summary>
        private void SearchIPv6()
        {
            LogMessage("SearchIPv6", $"Sending IPv6 discovery broadcasts");

            // Bind a socket to each adapter explicitly

            foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                try
                {
                    LogMessage("SearchIPv6", $"Found network adapter {adapter.Description}, Interface type: {adapter.NetworkInterfaceType} - supports multicast: {adapter.SupportsMulticast}, Operational status: {adapter.OperationalStatus}");
                    if (adapter.OperationalStatus != OperationalStatus.Up)
                        continue;
                    LogMessage("SearchIPv6", $"Adapter {adapter.Description} is up");

                    if (adapter.Supports(NetworkInterfaceComponent.IPv6) && adapter.SupportsMulticast)
                    {
                        LogMessage("SearchIPv6", $"Adapter {adapter.Description} supports IPv6");

                        IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                        if (adapterProperties != null)
                        {
                            UnicastIPAddressInformationCollection uniCast = adapterProperties.UnicastAddresses;
                            LogMessage("SearchIPv6", $"Adapter {adapter.Description} does have properties. Number of unicast addresses: {uniCast.Count}");

                            if (uniCast.Count > 0)
                            {
                                foreach (UnicastIPAddressInformation uni in uniCast)
                                {
                                    try
                                    {
                                        if (uni.Address.AddressFamily == AddressFamily.InterNetworkV6)
                                        {
                                            LogMessage("SearchIPv6", $"Interface {uni.Address} supports IPv6 - IsLinkLocal: {uni.Address.IsIPv6LinkLocal}, Is loop back: {IPAddress.IsLoopback(uni.Address)}");

                                            if (uni.Address.IsIPv6LinkLocal)
                                            {
                                                if (!IPAddress.IsLoopback(uni.Address))
                                                {
                                                    try
                                                    {
                                                        LogMessage("SearchIPv6", $"Sending multicast IPv6 discovery packet to {uni.Address}.");

                                                        if (!IPv6Clients.ContainsKey(uni.Address))
                                                        {
                                                            IPv6Clients.Add(uni.Address, NewIPv6Client(uni.Address, 0));
                                                        }

                                                        IPv6Clients[uni.Address].Send(Constants.DiscoveryMessageArray, Constants.DiscoveryMessageArray.Length, new IPEndPoint(IPAddress.Parse(Constants.MulticastGroup), discoveryPort));
                                                        LogMessage("SearchIPv6", $"Sent multicast IPv6 discovery packet to {uni.Address}.");
                                                    }
                                                    catch (SocketException ex)
                                                    {
                                                        logger?.LogError(ex.Message);
                                                        LogMessage("SearchIPv6", $"Socket exception (error code: {ex.ErrorCode}) sending IPv6 discovery packet to {uni.Address}: {ex}");
                                                    }
                                                }
                                                else
                                                {
                                                    LogMessage("SearchIPv6", $"Ignoring {uni.Address} because it is a loop back address.");
                                                }
                                            }
                                            else
                                            {
                                                LogMessage("SearchIPv6", $"Ignoring {uni.Address} because it is not link local.");
                                            }
                                        }
                                        else
                                        {
                                            LogMessage("SearchIPv6", $"Ignoring {uni.Address} because it doe not support IPv6. Its address family is {uni.Address.AddressFamily}");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        logger?.LogError(ex.Message);
                                        LogMessage("SearchIPv6", $"Exception sending IPv6 discovery packet to {uni.Address}: {ex}");
                                    }
                                }
                            }
                            else
                            {
                                LogMessage("SearchIPv6", $"Ignoring adapter {adapter.Description} because it does have properties but its unicast address count is 0.");
                            }
                        }
                        else
                        {
                            LogMessage("SearchIPv6", $"Ignoring adapter {adapter.Description} because it does not have properties and consequently does not have any unicast addresses.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex.Message);
                    LogMessage("SearchIPv6", $"Exception: {ex}");
                }
            }
        }

        private UdpClient NewIPv6Client(IPAddress host, int port)
        {
            var client = new UdpClient(AddressFamily.InterNetworkV6);

            //0 tells OS to give us a free ephemeral port
            client.Client.Bind(new IPEndPoint(host, port));

            client.BeginReceive(new AsyncCallback(ReceiveCallback), client);

            return client;
        }

        /// <summary>
        /// Send out the IPv4 and IPv6 messages on the specified discovery port
        /// </summary>
        private void SendDiscoveryMessage(int discoveryPort, bool searchIPv4, bool searchIPv6)
        {
            this.discoveryPort = discoveryPort; // Save the supplied discovery port

            if (searchIPv4) { SearchIPv4(); }

            if (searchIPv6) { SearchIPv6(); }
        }

        #endregion

        #region Support code

        // This turns the unicast address and the subnet into the broadcast address for that range
        // http://blogs.msdn.com/b/knom/archive/2008/12/31/ip-address-calculations-with-c-subnetmasks-networks.aspx
        private static IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }

        /// <summary>
        /// Format log messages according to thread number in order to make then easier to follow in the log
        /// </summary>
        /// <param name="method">Method name</param>
        /// <param name="message">Message text</param>
        private void LogMessage(string method, string message)
        {
            logger.LogMessage(LogLevel.Information, $"Finder - {method}", $"{Thread.CurrentThread.ManagedThreadId,2} {message}");
        }

        #endregion

    }
}