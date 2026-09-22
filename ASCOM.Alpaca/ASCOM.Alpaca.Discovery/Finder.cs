using ASCOM.Common;
using ASCOM.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;

// CASE SENSITIVITY NOTE - May 2026 
// The implementation of case sensitivity checking in 2022 was faulty and resulted in the logic being inverted. i.e. case insensitive parsing was used when
// strictCasing was set to true. This meant that the default behaviour was case insensitive parsing, which was not the intention. Also the strictCasing parameter
// in the "public Finder(bool strictCasing, ILogger logger)" constructor was misleading because it actually delivered case insensitive behaviour.
// In order to preserve four years of development on the basis of the faulty logic, the logic has been left as it is. However, the variable name has been changed
// from "strictCasing" to "propertyNameCaseInsensitive" to reflect the actual behaviour of the code. The constructor parameter name has not been changed in order
// to preserve backwards compatibility, but the constructor has been deprecated and replaced with a new constructor that just accepts an ILogger and defaults to
// case insensitive parsing. A new SetStrictCasing method has been added to allow the casing behaviour to be changed at runtime if required.

namespace ASCOM.Alpaca.Discovery
{
    /// <summary>
    /// Initiates a discovery and raises events as devices respond
    /// </summary>
    public class Finder : IDisposable
    {
        private readonly ILogger logger; // Optional logger
        private int discoveryPort = Constants.DiscoveryPort; // Default to the standard discovery port
        private readonly Dictionary<IPAddress, UdpClient> IPv4Clients = new Dictionary<IPAddress, UdpClient>(); // Collection of IP v4 clients for the various link local and localhost networks
        private readonly Dictionary<IPAddress, UdpClient> IPv6Clients = new Dictionary<IPAddress, UdpClient>(); // Collection of IP v6 clients for the various link local and localhost networks
        private bool disposedValue; // Disposed variable
        private readonly object broadcastResponsesLockObject = new object();
        private readonly object cachedEndpointsLockObject = new object();
        private JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();

        private const int SIO_UDP_CONNRESET = -1744830452; //Control code to turn off UDP ICMP Connection Reset
        private const JsonNameCaseSensitivity JSON_NAME_CASING_DEFAULT = JsonNameCaseSensitivity.AnyCasing; // Default to case insensitive parsing for backwards compatibility with the faulty logic in the original implementation

        #region Initialisation and Dispose

        /// <summary>
        /// Creates an Alpaca Finder object that sends out a search request for Alpaca devices
        /// </summary>
        /// <remarks>
        /// The results will be sent to the callback and stored in the cache
        /// Calling search and concatenating the results reduces the chance that a UDP packet is lost
        /// This may require firewall access
        /// </remarks>
        public Finder()
        {
            SetJsonNameCaseSensitivity(JSON_NAME_CASING_DEFAULT); // Set the default JSON name case sensitivity)
        }

        /// <summary>
        /// Creates an Alpaca Finder object with an associated ILogger instance that sends out a search request for Alpaca devices
        /// </summary>
        /// <param name="jsonNameCaseSensitivity">The JSON name case sensitivity to use when parsing the discovery response.</param>
        public Finder(JsonNameCaseSensitivity jsonNameCaseSensitivity) : this()
        {
            SetJsonNameCaseSensitivity(jsonNameCaseSensitivity); // Set the JSON name case sensitivity
            LogInformation("Finder", "Initialised with case sensitivity.");
        }

        /// <summary>
        /// Creates an Alpaca Finder object with an associated ILogger instance that sends out a search request for Alpaca devices
        /// </summary>
        /// <param name="logger">The logger instance to use for diagnostic messages.</param>
        public Finder(ILogger logger) : this()
        {
            this.logger = logger; // Save the trace logger object
            LogInformation("Finder", "Initialised with logger.");
        }

        /// <summary>
        /// Creates an Alpaca Finder object with an associated ILogger instance that sends out a search request for Alpaca devices
        /// </summary>
        /// <param name="jsonNameCaseSensitivity">The JSON name case sensitivity to use when parsing the discovery response.</param>
        /// <param name="logger">The logger instance to use for diagnostic messages.</param>
        public Finder(JsonNameCaseSensitivity jsonNameCaseSensitivity, ILogger logger) : this()
        {
            this.logger = logger; // Save the trace logger object
            SetJsonNameCaseSensitivity(jsonNameCaseSensitivity); // Set the JSON name case sensitivity
            LogInformation("Finder", "Initialised with logger and case sensitivity.");
        }

        /// <summary>
        /// Creates an Alpaca Finder object with an associated ILogger instance and given JSON name case sensitivity that sends out a search request for Alpaca devices
        /// </summary>
        /// <param name="strictCasing">Enforce correct casing of the port variable name in the Alpaca JSON discovery response.</param>
        /// <param name="logger">An ILogger instance (or null) into which the Finder will log.</param>
        /// <remarks>
        /// The results will be sent to the callback and stored in the cache
        /// Calling search and concatenating the results reduces the chance that a UDP packet is lost
        /// This may require firewall access
        /// </remarks>
        [Obsolete("The strictCasing logic is inverted but for backward compatibility will not be changed. Use the ILogger‑only constructor to keep current behaviour." +
            "Use SetJsonNameCaseSensitivity to control case sensitivity explicitly if required.")]
        public Finder(bool strictCasing, ILogger logger) : this()
        {
            // NOTE - The logic in the next line is incorrect but has been left as it is for backwards compatibility.
            jsonSerializerOptions.PropertyNameCaseInsensitive = strictCasing; // Save the strict JSON de-serialisation casing state
            this.logger = logger; // Save the trace logger object
            LogInformation("Finder", "This Constructor is OBSOLETE - Initialised with logger and casing control.");
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
            lock (cachedEndpointsLockObject)
            {
                CachedEndpoints.Clear();
            }
        }

        /// <summary>
        /// Set casing sensitivity of the JSON de-serialisation of the discovery response.
        /// </summary>
        /// <param name="jsonNameCaseSensitivity">The JSON name case sensitivity to use.</param>
        /// <remarks>
        /// Note that this only affects the parsing of the discovery response "AlpacaPort" element.
        /// If StrictCasing is set then only "AlpacaPort" will be accepted. If AnyCasing is set then all other casings are accepted: e.g. "AlpacaPort", "alpacaport", "ALPACAPORT".
        /// </remarks>
        private void SetJsonNameCaseSensitivity(JsonNameCaseSensitivity jsonNameCaseSensitivity)
        {
            switch (jsonNameCaseSensitivity)
            {
                case JsonNameCaseSensitivity.AnyCasing:
                    LogInformation("SetJsonNameCaseSensitivity", $"Set JSON name case sensitivity to AnyCasing. The discovery response parser will accept any casing of the port variable name in the Alpaca JSON discovery response.");
                    jsonSerializerOptions.PropertyNameCaseInsensitive = true; // Set the JSON name case sensitivity to case insensitive
                    break;

                case JsonNameCaseSensitivity.CorrectCasingOnly:
                    LogInformation("SetJsonNameCaseSensitivity", $"Set JSON name case sensitivity to StrictCasing. The discovery response parser will only accept the correct casing of the port variable name in the Alpaca JSON discovery response.");
                    jsonSerializerOptions.PropertyNameCaseInsensitive = false; // Set the JSON name case sensitivity to case sensitive
                    break;

                default:
                    throw new InvalidValueException($"Invalid JsonNameCaseSensitivity value: {jsonNameCaseSensitivity}");
            }
        }

        #endregion

        #region Discovery Management

        // This dual targets NetStandard 2.0 and NetFX 3.5 so no Async Await
        // This callback is shared between IPv4 and IPv6
        private void ReceiveCallback(IAsyncResult ar)
        {
            IPEndPoint endpoint = null;
            UdpClient udpClient = null;
            bool receiveRearmed = false;
            try
            {
                udpClient = (UdpClient)ar.AsyncState;

                endpoint = new IPEndPoint(IPAddress.Any, discoveryPort);

                // Obtain the UDP message body as a byte[]
                byte[] returnedBytes = udpClient.EndReceive(ar, ref endpoint);
                RestartReceive(udpClient, endpoint);
                receiveRearmed = true;

                // Save the broadcast response in a thread safe manner
                lock (broadcastResponsesLockObject)
                {
                    BroadcastResponses.Add(new BroadcastResponse(endpoint, returnedBytes));
                }

                // Convert the message bytes to a string, with remote IP address attached as well
                string ReceiveString = Encoding.ASCII.GetString(returnedBytes);
                LogInformation($"ReceiveCallback", $"Received {ReceiveString} from Alpaca device at {endpoint.Address}:{endpoint.Port}");

                // Accept responses containing the discovery response string and don't respond to your own transmissions
                if (ReceiveString.ToLowerInvariant().Contains(Constants.ResponseString.ToLowerInvariant())) // Accept responses in any casing so that bad casing can be reported
                {
                    int port = JsonSerializer.Deserialize<AlpacaDiscoveryResponse>(ReceiveString, jsonSerializerOptions).AlpacaPort;

                    if (port == 0) //Failed to parse
                    {
                        throw new Exception($"Failed to parse {ReceiveString} into an Alpaca Port");
                    }

                    var alpacaEndpoint = new IPEndPoint(endpoint.Address, port);
                    bool endpointAdded = false;
                    lock (cachedEndpointsLockObject)
                    {
                        if (!CachedEndpoints.Contains(alpacaEndpoint))
                        {
                            LogInformation("ReceiveCallback", $"Added new Alpaca API endpoint: {alpacaEndpoint.Address}:{alpacaEndpoint.Port} from endpoint: {endpoint.Address}:{endpoint.Port}");

                            CachedEndpoints.Add(alpacaEndpoint);
                            endpointAdded = true;
                        }
                    }

                    if (endpointAdded)
                    {
                        ResponseReceivedEvent?.Invoke(this, alpacaEndpoint);
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // The UdpClient/Socket was disposed while a receive was pending. This is expected during normal shutdown (e.g. when the discovery timer closes clients) and is not an error.
                return;
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.OperationAborted
                                           || ex.SocketErrorCode == SocketError.Interrupted)
            {
                // WSA_OPERATION_ABORTED (995) / WSAEINTR: the pending receive was cancelled because the socket was closed/disposed while EndReceive was still outstanding. This is an expected
                // consequence of the discovery timeout or Dispose() shutting down the UdpClient, not a genuine parsing or network failure, so it is safe to ignore.
                // LogMessage("ReceiveCallback", $"Ignored aborted receive on {udpClient}: {ex.SocketErrorCode} (expected during shutdown).");
                return;
            }
            catch (Exception ex)
            {
                LogError("ReceiveCallback", $"Failed to parse response from {endpoint}: {ex}");
            }
            finally
            {
                if (!receiveRearmed && udpClient != null && !disposedValue)
                {
                    RestartReceive(udpClient, endpoint);
                }
            }
        }

        private void RestartReceive(UdpClient udpClient, IPEndPoint endpoint)
        {
            try
            {
                // Keep the cached client ready to receive responses to subsequent searches.
                udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), udpClient);
            }
            catch (ObjectDisposedException)
            {
                // The client was disposed while the receive was being restarted.
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.OperationAborted
                                           || ex.SocketErrorCode == SocketError.Interrupted)
            {
                // The pending receive was cancelled during shutdown.
            }
            catch (Exception ex)
            {
                LogError("ReceiveCallback", $"Failed to restart receive from {endpoint}: {ex.Message}\r\n{ex}");
            }
        }

        /// <summary>
        /// Send out discovery message on each IPv4 broadcast address
        /// This dual targets NetStandard 2.0 and NetFX 3.5 so no Async Await
        /// Broadcasts on each network interface address as per Windows / Linux documentation
        /// </summary>
        private void SearchIPv4()
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            LogInformation("SearchIPv4", $"Sending IPv4 discovery broadcasts");

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                try
                {
                    //Do not try and use non-operational network interfaces
                    if (networkInterface.OperationalStatus == OperationalStatus.Up && networkInterface.Supports(NetworkInterfaceComponent.IPv4))
                    {
                        IPInterfaceProperties ipInterfaceProperties = networkInterface.GetIPProperties();
                        if (ipInterfaceProperties != null)
                        {
                            UnicastIPAddressInformationCollection uniCast = ipInterfaceProperties.UnicastAddresses;
                            if (uniCast.Count > 0)
                            {
                                foreach (UnicastIPAddressInformation uni in uniCast)
                                {
                                    try
                                    {
                                        if (uni.Address.AddressFamily == AddressFamily.InterNetwork && uni.IPv4Mask != IPAddress.Parse("255.255.255.255"))
                                        {
                                            if (!IPv4Clients.ContainsKey(uni.Address))
                                            {
                                                IPv4Clients.Add(uni.Address, NewIPv4Client());
                                            }

                                            if (IPv4Clients[uni.Address].Client.IsBound)
                                            {
                                                // Local host addresses (127.*.*.*) may have a null mask in Net Framework. We do want to search these. The correct mask is 255.0.0.0.
                                                IPv4Clients[uni.Address].Send(Constants.DiscoveryMessageArray, Constants.DiscoveryMessageArray.Length, new IPEndPoint(GetBroadcastAddress(uni.Address, uni.IPv4Mask ?? IPAddress.Parse("255.0.0.0")), discoveryPort));
                                                LogInformation("SearchIPv4", $"Sent broadcast to: {uni.Address}");
                                            }
                                            else
                                            {
                                                IPv4Clients.Remove(uni.Address);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        LogError("SearchIPv4", $"Send exception: {ex.Message}\r\n{ex}");
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogError("SearchIPv4", $"Exception: {ex.Message}\r\n{ex}");
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
            LogInformation("SearchIPv6", $"Sending IPv6 discovery broadcasts");

            // Bind a socket to each network interface explicitly
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                try
                {
                    LogDebug("SearchIPv6", $"Found network interface {networkInterface.Description}, Interface type: {networkInterface.NetworkInterfaceType} - supports multicast: {networkInterface.SupportsMulticast}, Operational status: {networkInterface.OperationalStatus}");

                    // Check whether the network interface is up and running
                    if (networkInterface.OperationalStatus != OperationalStatus.Up) // The network interface is not up and running
                    {
                        LogDebug("SearchIPv6", $"  Ignoring network interface {networkInterface.Description} because it is not up and running. Its operational status is {networkInterface.OperationalStatus}");
                        continue;
                    }

                    LogDebug("SearchIPv6", $"  Network interface {networkInterface.Description} is up");

                    // Check whether the network interface supports IPv6
                    if (!networkInterface.Supports(NetworkInterfaceComponent.IPv6)) // The network interface does not support IPv6
                    {
                        LogDebug("SearchIPv6", $"  Ignoring network interface {networkInterface.Description} because it does not support IPv6.");
                        continue;
                    }

                    LogDebug("SearchIPv6", $"  Network interface {networkInterface.Description} supports IPv6");

                    // Check whether the network interface has any properties
                    IPInterfaceProperties ipInterfaceProperties = networkInterface.GetIPProperties();
                    if ((ipInterfaceProperties == null)) // The network interface has no properties
                    {
                        LogDebug("SearchIPv6", $"  Ignoring network interface {networkInterface.Description} because it does not have properties and consequently does not have any unicast addresses.");
                        continue;
                    }

                    // Check whether there are any unicast addresses on the network interface and ignore the interface if there are none
                    UnicastIPAddressInformationCollection uniCast = ipInterfaceProperties.UnicastAddresses;
                    LogDebug("SearchIPv6", $"  Network interface {networkInterface.Description} does have properties. Number of unicast addresses: {uniCast.Count}");
                    if (uniCast.Count == 0) // The network interface has no unicast addresses
                    {
                        LogDebug("SearchIPv6", $"  Ignoring network interface {networkInterface.Description} because it does not have any unicast addresses.");
                        continue;
                    }

                    // Process each address in turn
                    foreach (UnicastIPAddressInformation uni in uniCast)
                    {
                        try
                        {
                            // Check whether the address supports IPv6
                            if (uni.Address.AddressFamily != AddressFamily.InterNetworkV6) // The address does not support IPv6 so ignore it
                            {
                                LogDebug("SearchIPv6", $"  Ignoring {uni.Address} because it does not support IPv6. Its address family is {uni.Address.AddressFamily}");
                                continue;
                            }

                            LogDebug("SearchIPv6", $"  Address {uni.Address} supports IPv6 - Is linklocal: {uni.Address.IsIPv6LinkLocal}, Is loopback: {IPAddress.IsLoopback(uni.Address)}");

                            // Check whether this is a loopback address and process it as a multicast address.
                            // A multicast packet is delivered to every responder listening on the discovery port.
                            if (IPAddress.IsLoopback(uni.Address)) // Address is IPv6 loopback
                            {
                                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) // Address is IPv6 loopback and OSPlatform is Windows
                                {
                                    // Check whether the network interface supports multicast and ignore if it does not.
                                    if (!networkInterface.SupportsMulticast) // Address is IPv6 loopback and OSPlatform is Windows and network interface does not support multicast
                                    {
                                        LogDebug("SearchIPv6", $"  Ignoring {uni.Address} because the network interface does not support multicast.");
                                        continue;
                                    }

                                    try
                                    {
                                        LogDebug("SearchIPv6", $"  Sending multicast IPv6 discovery packet to {uni.Address}.");

                                        // Create a new UdpClient for this loopback address if one does not already exist
                                        if (!IPv6Clients.ContainsKey(uni.Address)) // Client does not exist for this loopback address, so create one
                                        {
                                            IPv6Clients.Add(uni.Address, NewIPv6Client(uni.Address, 0, ipInterfaceProperties.GetIPv6Properties().Index));
                                        }

                                        // Send the discovery packet to the multicast group on the loopback interface
                                        IPv6Clients[uni.Address].Send(Constants.DiscoveryMessageArray, Constants.DiscoveryMessageArray.Length, GetMulticastEndPoint(discoveryPort, ipInterfaceProperties.GetIPv6Properties().Index));
                                        LogInformation("SearchIPv6", $"  Sent multicast IPv6 discovery packet to {uni.Address}:{discoveryPort}.");
                                    }
                                    catch (SocketException ex)
                                    {
                                        LogError("SearchIPv6", $"  Socket exception {ex.Message} (error code: {ex.ErrorCode}) sending multicast IPv6 discovery packet to {uni.Address}:{discoveryPort}: {ex}");
                                    }
                                } // Platform is Windows
                                else // Address is IPv6 loopback and OSPlatform is not Windows
                                {
                                    // Try adding a HOST LOCAL multicast address to the loopback interface for non-Windows platforms (Linux, macOS, etc.) because multicast on the loopback interface may not be supported or may behave differently.
                                    try
                                    {
                                        LogDebug("SearchIPv6", $"  OS is not Windows, adding HOST LOCAL multicast address to the loopback interface.");
                                        // Get the index of the loopback interface on Unix-like systems (Linux, macOS, etc.) using the interface name "lo".
                                        int interfaceIndex = NetworkInterfaceIndexFinder.GetIndex(Constants.UnixLoopbackInterfaceName);

                                        // Create a scoped multicast address for the loopback interface using the HOST LOCAL multicast address and the interface index.
                                        IPAddress multicastAddress = CreateScopedMulticastAddress(Constants.HostLocalMulticastGroup, interfaceIndex);
                                        IPEndPoint targetEndPoint = new IPEndPoint(multicastAddress, discoveryPort);

                                        // Create a new UdpClient for the loopback interface and set the necessary socket options for multicast.
                                        UdpClient callerClient = new UdpClient(AddressFamily.InterNetworkV6);

                                        callerClient.Client.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastLoopback, true);
                                        callerClient.Client.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastInterface, interfaceIndex);

                                        // Bind the UdpClient to the loopback address and an ephemeral port (0) to allow the OS to assign a free port.
                                        callerClient.Client.Bind(new IPEndPoint(IPAddress.IPv6Loopback, 0));

                                        // Listen for discovery responses on the same client that sent the multicast datagram.
                                        callerClient.BeginReceive(new AsyncCallback(ReceiveCallback), callerClient);

                                        // Send the discovery packet to the HOST LOCAL multicast address on the loopback interface
                                        int bytesSent = callerClient.Send(Constants.DiscoveryMessageArray, Constants.DiscoveryMessageArray.Length, targetEndPoint);
                                        LogInformation("SearchIPv6", $"  Sent {bytesSent} bytes of HOST LOCAL multicast IPv6 discovery data to {targetEndPoint}.");

                                        // Retain the configured client or dispose of it if it is not required
                                        if (!IPv6Clients.ContainsKey(uni.Address))
                                        {
                                            IPv6Clients.Add(uni.Address, callerClient);
                                        }
                                        else
                                        {
                                            callerClient.Dispose();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        LogError("SearchIPv6", $"Error sending HOST LOCAL multicast IPv6 discovery packet to {uni.Address}:{discoveryPort}: {ex.Message}\r\n{ex}");
                                    }

                                    // Add a fallback to unicast discovery on the loopback address for non-Windows platforms (Linux, macOS, etc.) because multicast on the loopback interface may not be supported or may behave differently.
                                    try
                                    {
                                        LogDebug("SearchIPv6", $"  Sending unicast IPv6 discovery packet to {uni.Address}.");

                                        if (!IPv6Clients.ContainsKey(uni.Address))
                                        {
                                            IPv6Clients.Add(uni.Address, NewIPv6Client(uni.Address, 0, 0));
                                        }

                                        IPv6Clients[uni.Address].Send(Constants.DiscoveryMessageArray, Constants.DiscoveryMessageArray.Length, new IPEndPoint(IPAddress.IPv6Loopback, discoveryPort));
                                        LogInformation("SearchIPv6", $"  Sent unicast IPv6 discovery packet to {uni.Address}:{discoveryPort}.");
                                    }
                                    catch (SocketException ex)
                                    {
                                        LogError("SearchIPv6", $"  Socket exception {ex.Message} (error code: {ex.ErrorCode}) sending unicast IPv6 discovery packet to {uni.Address}:{discoveryPort}: {ex}");
                                    }
                                } // Platform is not Windows
                            } // Address is loopback
                            else // Address is not loopback
                            {
                                // Check whether this is a link local network interface and ignore it if it is not.
                                if (!uni.Address.IsIPv6LinkLocal) // Address is not linklocal
                                {
                                    LogDebug("SearchIPv6", $"  Ignoring {uni.Address} because it is not linklocal.");
                                    continue;
                                }

                                // Test whether the network interface supports multicast and ignore it if it does not.
                                if (!networkInterface.SupportsMulticast) // Network interface does not support multicast, so ignore this address
                                {
                                    LogDebug("SearchIPv6", $"  Ignoring {uni.Address} because the network interface does not support multicast.");
                                    continue;
                                }

                                try
                                {
                                    LogDebug("SearchIPv6", $"  Sending multicast IPv6 discovery packet to {uni.Address}.");

                                    // Create a new UdpClient for this link local address if one does not already exist
                                    if (!IPv6Clients.ContainsKey(uni.Address)) // Client does not exist for this link local address, so create one
                                    {
                                        IPAddress bindAddress = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? IPAddress.IPv6Any : uni.Address;
                                        IPv6Clients.Add(uni.Address, NewIPv6Client(bindAddress, 0, networkInterface.GetIPProperties().GetIPv6Properties().Index));
                                    }

                                    // Send the discovery packet to the multicast group on this link local interface
                                    IPv6Clients[uni.Address].Send(Constants.DiscoveryMessageArray, Constants.DiscoveryMessageArray.Length, GetMulticastEndPoint(discoveryPort, networkInterface.GetIPProperties().GetIPv6Properties().Index));
                                    LogInformation("SearchIPv6", $"  Sent multicast IPv6 discovery packet to {uni.Address}:{discoveryPort}.");
                                }
                                catch (SocketException ex)
                                {
                                    LogError("SearchIPv6", $"  Socket exception {ex.Message} (error code: {ex.ErrorCode}) sending IPv6 discovery packet to {uni.Address}:{discoveryPort}: {ex}");
                                }
                            } // Address is not loopback
                        }
                        catch (Exception ex)
                        {
                            LogError("SearchIPv6", $"  Exception sending IPv6 discovery packet to {uni.Address}: {ex.Message}\r\n{ex}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogError("SearchIPv6", $"Exception: {ex.Message}\r\n{ex}");
                }
            }
        }

        private static IPAddress CreateScopedMulticastAddress(string address, int interfaceIndex)
        {
            IPAddress multicastAddress = IPAddress.Parse(address);

            if (multicastAddress.AddressFamily != AddressFamily.InterNetworkV6 || !multicastAddress.IsIPv6Multicast)
            {
                throw new ArgumentException($"'{address}' is not an IPv6 multicast address.", nameof(address));
            }

            return new IPAddress(multicastAddress.GetAddressBytes(), interfaceIndex);
        }

        static class NetworkInterfaceIndexFinder
        {
            public static int GetIndex(string name)
            {
                NetworkInterface target = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(i => i.Name == name)
                    ?? throw new InvalidOperationException($"Interface '{name}' not found.");

                if (!target.Supports(NetworkInterfaceComponent.IPv6))
                {
                    throw new InvalidOperationException($"Interface '{name}' does not support IPv6.");
                }

                if (!target.SupportsMulticast && target.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    throw new InvalidOperationException($"Interface '{name}' does not support multicast.");
                }

                return target.GetIPProperties().GetIPv6Properties().Index;
            }
        }



        private UdpClient NewIPv6Client(IPAddress host, int port, int interfaceIndex)
        {
            var client = new UdpClient(AddressFamily.InterNetworkV6);

            client.MulticastLoopback = true;
            if (interfaceIndex != 0)
            {
                client.Client.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastInterface, interfaceIndex);
            }

            //0 tells OS to give us a free ephemeral port
            client.Client.Bind(new IPEndPoint(host, port));

            client.BeginReceive(new AsyncCallback(ReceiveCallback), client);

            return client;
        }

        private static IPEndPoint GetMulticastEndPoint(int port, int interfaceIndex)
        {
            IPAddress multicastAddress = IPAddress.Parse(Constants.LinkLocalMulticastGroup);
            multicastAddress.ScopeId = interfaceIndex;

            return new IPEndPoint(multicastAddress, port);
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
        private void LogInformation(string method, string message)
        {
            logger?.LogMessage(LogLevel.Information, $"Finder - {method}", $"{Thread.CurrentThread.ManagedThreadId,2} {message}");
        }

        /// <summary>
        /// Format log messages according to thread number in order to make then easier to follow in the log
        /// </summary>
        /// <param name="method">Method name</param>
        /// <param name="message">Message text</param>
        private void LogDebug(string method, string message)
        {
            logger?.LogMessage(LogLevel.Debug, $"Finder - {method}", $"{Thread.CurrentThread.ManagedThreadId,2} {message}");
        }

        /// <summary>
        /// Format log messages according to thread number in order to make then easier to follow in the log
        /// </summary>
        /// <param name="method">Method name</param>
        /// <param name="message">Message text</param>
        private void LogError(string method, string message)
        {
            logger?.LogMessage(LogLevel.Error, $"Finder - {method}", $"{Thread.CurrentThread.ManagedThreadId,2} {message}");
        }

        #endregion

    }
}