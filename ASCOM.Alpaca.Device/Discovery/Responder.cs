// (c) 2019 Daniel Van Noord
// This code is licensed under MIT license (see License.txt for details)

using ASCOM.Common;
using ASCOM.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace ASCOM.Alpaca.Discovery
{
    /// <summary>
    /// Enable Alpaca devices to respond to Alpaca discovery broadcasts by returning the Alpaca port number. This component is delivered in NuGet package: <b>ASCOM.Alpaca.Device</b> to minimise its footprint.
    /// </summary>
    public class Responder : IDisposable
    {
        private readonly int port;

        private readonly int DiscoveryPort = Constants.DiscoveryPort;

        private readonly List<UdpClient> Clients = new List<UdpClient>();

        private readonly Dictionary<UdpClient, IPAddress> MulticastGroups = new Dictionary<UdpClient, IPAddress>();

        private ILogger Logger
        {
            get;
            set;
        }

        /// <summary>
        /// Enable responses to non-local discovery broadcasts
        /// </summary>
        public bool AllowRemoteAccess
        {
            get;
            set;
        } = true;

        /// <summary>
        /// Confine responses to localhost broadcasts only
        /// </summary>
        public bool LocalRespondOnlyToLocalHost
        {
            get;
            set;
        } = true;

        /// <summary>
        /// Flag whether the object has been disposed
        /// </summary>
        public bool Disposed
        {
            get;
            private set;
        } = false;

        /// <summary>
        /// Create an Alpaca Responder reporting the AlpacaPort. This will use default Discovery Port (32227) and will respond on IPv4 and IPv6.
        /// </summary>
        /// <param name="AlpacaPort">The port the Alpaca REST API is available on</param>
        /// <param name="Logger">ILogger object to which operational log messages will be sent.</param>
        public Responder(int AlpacaPort, ILogger Logger = null) : this(AlpacaPort, Constants.DiscoveryPort, true, true, Logger)
        {

        }

        /// <summary>
        /// Create an Alpaca Responder reporting the AlpacaPort. This will use default Discovery Port (32227).
        /// </summary>
        /// <param name="AlpacaPort">The port the Alpaca REST API is available on</param>
        /// <param name="IPv4">Respond on IPv4</param>
        /// <param name="IPv6">Respond on IPv6</param>
        /// <param name="Logger">ILogger object to which operational log messages will be sent.</param>
        public Responder(int AlpacaPort, bool IPv4, bool IPv6, ILogger Logger = null) : this(AlpacaPort, Constants.DiscoveryPort, IPv4, IPv6, Logger)
        {

        }

        /// <summary>
        /// Create an Alpaca Responder reporting the AlpacaPort using a custom discovery port.
        /// </summary>
        /// <param name="AlpacaPort">The port the Alpaca REST API is available on</param>
        /// <param name="DiscoveryPort">The Discovery Port</param>
        /// <param name="IPv4">Respond on IPv4</param>
        /// <param name="IPv6">Respond on IPv6</param>
        /// <param name="Logger">ILogger object to which operational log messages will be sent.</param>
        public Responder(int AlpacaPort, int DiscoveryPort, bool IPv4, bool IPv6, ILogger Logger = null)
        {
            this.Logger = Logger;

            LogDebug($"Responder.ctor - Creating discovery responder with AlpacaPort: {AlpacaPort}, DiscoveryPort: {DiscoveryPort}, IPv4: {IPv4}, IPv6: {IPv6}");
            port = AlpacaPort;
            this.DiscoveryPort = DiscoveryPort;

            if (!IPv4 && !IPv6)
            {
                throw new ArgumentException("You must search on one or more protocol types.");
            }

            if (IPv4)
            {
                InitIPv4();
            }

            if (IPv6)
            {
                InitIPv6();
            }
        }

        /// <summary>
        /// Create a UDP client that listens on the discovery port of all IPv4 addresses for discovery broadcasts.
        /// </summary>
        private void InitIPv4()
        {
            LogDebug($"Responder.InitIPv4 - Binding to IPv4 Discovery Port: {DiscoveryPort}");
            UdpClient udpClient = new UdpClient();

            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            udpClient.EnableBroadcast = true;
            udpClient.MulticastLoopback = false;
            udpClient.ExclusiveAddressUse = false;

            udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, DiscoveryPort));

            // This uses begin receive rather then async so it works on net 3.5
            udpClient.BeginReceive(ReceiveCallback, udpClient);

            Clients.Add(udpClient);
            LogInformation($"Added IPv4 discovery responder on port: {DiscoveryPort}");
        }

        /// <summary>
        /// Bind a UDP client to each network interface and set the index and address for multicast
        /// </summary>
        private void InitIPv6()
        {
            LogDebug($"Responder.InitIPv6 - Binding to IPv6 Discovery Port: {DiscoveryPort}");

            // Check whether the OS is Windows or not because the network interface enumeration is different on Linux and MacOS.
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) // Running on Windows
            {
                // Windows needs to have the IP Address and index set for an IPv6 multicast socket

                // Get all the network interfaces on the system
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                LogDebug($"Responder.InitIPv6 - Running on Windows, found {networkInterfaces.Length} network interfaces");

                // Enumerate the network interfaces
                foreach (NetworkInterface networkInterface in networkInterfaces)
                {
                    try
                    {
                        // Check if the interface is up and running and ignore it if it is not.
                        if (!(networkInterface.OperationalStatus == OperationalStatus.Up)) // The interface is not up and running
                        {
                            LogDebug($"Responder.InitIPv6 - Ignoring network interface: {networkInterface.Name} because it is not up.");
                            continue;
                        }

                        // Check if this interface supports IPv6 and multicast and ignore it if it does not.
                        if (!(networkInterface.Supports(NetworkInterfaceComponent.IPv6) && networkInterface.SupportsMulticast)) // Interface does not support IPv6 and multicast
                        {
                            LogDebug($"Responder.InitIPv6 - Ignoring interface: {networkInterface.Name} because it does not support IPv6 and Multicast.");
                            continue;
                        }

                        // Get the IP properties of the interface and ignore the interface if it does not have any descriptive properties
                        IPInterfaceProperties networkInterfaceProperties = networkInterface.GetIPProperties();
                        if (networkInterfaceProperties is null) // The interface has no IP properties so move to the next interface
                        {
                            LogDebug($"Responder.InitIPv6 - Ignoring interface: {networkInterface.Name} because it has no IP properties.");
                            continue;
                        }

                        // Confirm that the interface has unicast addresses and ignore it if it does not. 
                        UnicastIPAddressInformationCollection uniCastAddresses = networkInterfaceProperties.UnicastAddresses;
                        if (uniCastAddresses.Count == 0) // Interface has no unicast addresses so move to the next interface
                        {
                            LogDebug($"Responder.InitIPv6 - Ignoring interface: {networkInterface.Name} because it has no unicast addresses.");
                            continue;
                        }

                        LogDebug($"Responder.InitIPv6 - Found network interface: {networkInterface.Name}, Type: {networkInterface.NetworkInterfaceType}, Status: {networkInterface.OperationalStatus}, Supports IPv6: {networkInterface.Supports(NetworkInterfaceComponent.IPv6)}, Supports Multicast: {networkInterface.SupportsMulticast}");

                        // Enumerate the unicast addresses of the interface 
                        foreach (UnicastIPAddressInformation unicastAddress in uniCastAddresses)
                        {
                            try
                            {
                                // Confirm that the unicast address is an IPv6 address and ignore it if it is not.
                                if (!(unicastAddress.Address.AddressFamily == AddressFamily.InterNetworkV6)) // The unicast address is not an IPv6 address
                                {
                                    LogDebug($"Responder.InitIPv6 -   Ignoring {networkInterface.Name} address: {unicastAddress.Address} because it is not an IPv6 address.");
                                    continue;
                                }

                                // Confirm that the unicast address is either a loopback or link-local address and ignore it if it is not.
                                // NOTE - Must test for localhost / loopback first because this address returns IsIPv6LinkLocal = false. The "Conditional logical OR (short circuit)" operator || ensures the logic works as intended.
                                if (!(IPAddress.IsLoopback(unicastAddress.Address) || unicastAddress.Address.IsIPv6LinkLocal)) // The unicast address is not a loopback or link-local address
                                {
                                    LogDebug($"Responder.InitIPv6 -   Ignoring {networkInterface.Name} address: {unicastAddress.Address} because it is not a loopback or link-local address.");
                                    continue;
                                }

                                // Add a new UDP client for this IPv6 address and interface index
                                Clients.Add(NewIpV6Client(unicastAddress.Address, networkInterfaceProperties.GetIPv6Properties().Index, Constants.LinkLocalMulticastGroup));
                                LogInformation($"Added IPv6 discovery responder for address: {unicastAddress.Address}, on interface index: {networkInterfaceProperties.GetIPv6Properties().Index} and port {DiscoveryPort}");
                            }
                            catch (Exception ex)
                            {
                                //May not have permission to access a specific socket or address
                                LogDebug($"Responder.InitIPv6 - Exception occurred while enumerating addresses: {ex.Message}\r\n{ex}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogDebug($"Responder.InitIPv6 - Exception occurred while enumerating network interfaces: {ex.Message}\r\n{ex}");
                        //May not have permission to access a specific socket or address
                    }
                }
            } // Running on Windows
            else // Not running on Windows, so assume Linux or MacOS and use unicast for loopback and multicast for link-local addresses
            {
                // Record interface numbers that need clients. A hash-set is used to ensure that each interface index is added only once, even if it has multiple link-local addresses.
                HashSet<int> clientInterfaceIndexes = new HashSet<int>();

                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                LogDebug($"Responder.InitIPv6 - Running on Linux or MacOS, found {networkInterfaces.Length} network interfaces");

                foreach (NetworkInterface networkInterface in networkInterfaces)
                {
                    try
                    {
                        // Check whether this is a loopback interface
                        if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback) // This is a Linux/MacOS loopback interface
                        {
                            // Check if the networkInterface is up and supports IPv6. If it is not, ignore it.
                            if (!((networkInterface.OperationalStatus == OperationalStatus.Up) && networkInterface.Supports(NetworkInterfaceComponent.IPv6))) // The loopback interface is not up or does not support IPv6
                            {
                                LogDebug($"Responder.InitIPv6 - Ignoring loopback interface {networkInterface.Name} because it is not up or does not support IPv6");
                                continue;
                            }

                            // Get the IP properties of the networkInterface and ignore it if it does not have any descriptive properties
                            IPInterfaceProperties networkInterfaceProperties = networkInterface.GetIPProperties();
                            if (networkInterfaceProperties is null) // The networkInterface has no IP properties
                            {
                                LogDebug($"Responder.InitIPv6 - Ignoring loopback interface {networkInterface.Name} because it has no IP properties");
                                continue;
                            }

                            // Check whether the networkInterface has any IPv6 unicast addresses and ignore it if it does not.
                            IPv6InterfaceProperties ipv6Properties = networkInterfaceProperties.GetIPv6Properties();
                            if (ipv6Properties is null) // The networkInterface has no IPv6 properties
                            {
                                LogDebug($"Responder.InitIPv6 - Ignoring loopback interface {networkInterface.Name} because it has no IPv6 properties");
                                continue;
                            }

                            LogDebug($"Responder.InitIPv6 - Found loopback interface: {networkInterface.Name}, Type: {networkInterface.NetworkInterfaceType}, Status: {networkInterface.OperationalStatus}, Supports IPv6: {networkInterface.Supports(NetworkInterfaceComponent.IPv6)}, Supports Multicast: {networkInterface.SupportsMulticast}");

                            if (!networkInterface.SupportsMulticast)
                            {
                                LogDebug($"Responder.InitIPv6 - Ignoring loopback interface {networkInterface.Name} because it does not support multicast.");
                                continue;
                            }

                            try
                            {
                                // Use the loopback interface's index rather than its platform-specific name.
                                Clients.Add(NewIpV6Client(IPAddress.IPv6Any, ipv6Properties.Index, Constants.HostLocalMulticastGroup));
                                LogInformation($"Responder.InitIPv6 - Added HOST LOCAL multicast IPv6 discovery responder for loopback interface {networkInterface.Name} on port {DiscoveryPort}");
                            }
                            catch (Exception ex)
                            {
                                LogDebug($"Responder.InitIPv6 -   Error adding HOST LOCAL multicast IPv6 discovery responder for loopback interface {networkInterface.Name} on port {DiscoveryPort}: {ex.Message}\r\n{ex}");
                            }
                        } // Interface is non-WIndows loopback
                        else // Not a loopback interface
                        {
                            // Check if the networkInterface is up and supports IPv6
                            if (networkInterface.OperationalStatus != OperationalStatus.Up) // Interface is not up
                            {
                                LogDebug($"Responder.InitIPv6 - Non-loopback interface {networkInterface.Name} is not up");
                                continue;
                            }

                            // Check if the networkInterface supports IPv6
                            if (!networkInterface.Supports(NetworkInterfaceComponent.IPv6)) // Interface does not support IPv6
                            {
                                LogDebug($"Responder.InitIPv6 -   Non-loopback interface {networkInterface.Name} does not support IPv6");
                                continue;
                            }

                            LogDebug($"Responder.InitIPv6 - Found non-loopback interface: {networkInterface.Name}, Type: {networkInterface.NetworkInterfaceType}, Status: {networkInterface.OperationalStatus}, Supports IPv6: {networkInterface.Supports(NetworkInterfaceComponent.IPv6)}, Supports Multicast: {networkInterface.SupportsMulticast}");

                            // Get the IP properties of the networkInterface and ignore it if it does not have any descriptive properties
                            IPInterfaceProperties networkInterfaceProperties = networkInterface.GetIPProperties();
                            if (networkInterfaceProperties is null) // The networkInterface does not have IP properties so move to next interface
                            {
                                LogDebug($"Responder.InitIPv6 -   Non-loopback interface {networkInterface.Name} has no IP properties");
                                continue;
                            }

                            // Get the IPv6 properties of the networkInterface and ignore it if it does not have any descriptive properties
                            IPv6InterfaceProperties ipv6Properties = networkInterfaceProperties.GetIPv6Properties();
                            if (ipv6Properties is null) // The networkInterface does not have IPv6 properties so move to next interface
                            {
                                LogDebug($"Responder.InitIPv6 -   Non-loopback interface {networkInterface.Name} has no IPv6 properties");
                                continue;
                            }

                            // Iterate over the addresses
                            foreach (UnicastIPAddressInformation uni in networkInterfaceProperties.UnicastAddresses)
                            {
                                LogDebug($"Responder.InitIPv6 -  Interface {networkInterface.Name} has unicast address: {uni.Address}. Address family: {uni.Address.AddressFamily}, Is IPv6 Link Local: {uni.Address.IsIPv6LinkLocal}, Supports Multicast: {networkInterface.SupportsMulticast}.");

                                // Check if the address is an IPv6 link-local address and the interface supports multicast and ignore it if it does not. 
                                if (!(uni.Address.AddressFamily == AddressFamily.InterNetworkV6 && uni.Address.IsIPv6LinkLocal && networkInterface.SupportsMulticast))
                                {
                                    LogDebug($"Responder.InitIPv6 -   Ignoring non-loopback interface {networkInterface.Name} with unicast address: {uni.Address} because it is not an IPv6 link-local address or the interface does not support multicast.");
                                    continue;
                                }

                                LogDebug($"Responder.InitIPv6 -   Including non-loopback interface {networkInterface.Name} with unicast address: {uni.Address} and address family: {uni.Address.AddressFamily}.");

                                // Save the interface index so a client can be added later. A hash-set is used to ensure that each interface index is only added once, even if the interface has multiple link-local addresses.
                                clientInterfaceIndexes.Add(ipv6Properties.Index);
                            }
                        } // Non-Windows normal interface i.e. a non-loopback interface
                    }
                    catch (Exception ex)
                    {
                        LogDebug($"Responder.InitIPv6 - Exception occurred while enumerating Linux interfaces: {ex.Message}\r\n{ex}");
                    }
                }

                // Create a multicast client for each unique interface index that has been collected
                foreach (int interfaceIndex in clientInterfaceIndexes)
                {
                    try
                    {
                        // IPv6Any is used to ensure all linklocal addresses on the interface are included. The interface index is what actually limits the scope of the multicast group membership to addresses on the specific interface.
                        Clients.Add(NewIpV6Client(IPAddress.IPv6Any, interfaceIndex, Constants.LinkLocalMulticastGroup));
                        LogInformation($"Added link local multicast IPv6 discovery responder for address: {IPAddress.IPv6Any} on interface index : {interfaceIndex} and port {DiscoveryPort}");
                    }
                    catch (Exception ex)
                    {
                        LogError($"Responder.InitIPv6 - Error adding link local multicast IPv6 discovery responder on interface {interfaceIndex} and port {DiscoveryPort}: {ex.Message}\r\n{ex}");
                    }
                }
            } // Running on a non-Windows OS

            // Log the details of each UDP client that has been created, including its local endpoint and whether it is part of a multicast group.
            LogDebug("");
            LogDebug($"Responder.InitIPv6 - Summary of UDP clients created for IPv6 discovery:");
            foreach (UdpClient udp in Clients)
            {
                IPEndPoint localEndPoint = udp.Client.LocalEndPoint as IPEndPoint;
                bool isMulticast = MulticastGroups.TryGetValue(udp, out IPAddress multicastAddress);
                LogDebug($"Responder.InitIPv6 - Created UDP client on IP address: {localEndPoint?.Address.ToString() ?? "none"}, port: {localEndPoint?.Port.ToString() ?? "none"}, multicast address: {multicastAddress?.ToString() ?? "none - unicast only"}.");
            }
            LogDebug("");
            LogDebug($"Responder.InitIPv6 - Completed binding to IPv6 Discovery Port: {DiscoveryPort}");
        }

        private UdpClient NewIpV6Client(IPAddress host, int index, string multicastGroup)
        {
            UdpClient udpClientV6 = new UdpClient(AddressFamily.InterNetworkV6);

            udpClientV6.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpClientV6.ExclusiveAddressUse = false;
            udpClientV6.Client.Bind(new IPEndPoint(host, DiscoveryPort));

            // Join the multicast group for the specified interface index if it is greater than 0. An index of 0 indicates that multicast is not being used, such as for the IPv6 loopback address.
            if (index > 0 && !string.IsNullOrEmpty(multicastGroup)) // Index is greater than 0 and a multicast group is specified, so join the multicast group for the specified interface index
            {
                IPAddress multicastAddress = IPAddress.Parse(multicastGroup);
                udpClientV6.JoinMulticastGroup(index, multicastAddress);
                // Keep track of the multicast group for this UDP client so it can be reported in the logs
                MulticastGroups.Add(udpClientV6, multicastAddress);
            }

            // Start listening for discovery messages. This uses begin receive rather than async so it works on net 3.5
            udpClientV6.BeginReceive(ReceiveCallback, udpClientV6);

            return udpClientV6;
        }

        /// <summary>
        /// Callback method that is invoked when a UDP message is received. This method processes the received message and sends a response if it is a discovery message.
        /// </summary>
        /// <param name="asyncResult">The result of the asynchronous operation.</param>
        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            UdpClient udpClient = null;
            IPEndPoint endpoint = null;
            bool callbackReenabled = false;

            try
            {
                // Retrieve the UdpClient instance from the asyncResult's AsyncState property
                udpClient = (UdpClient)asyncResult.AsyncState;

                // Create a new IPEndPoint to hold the remote endpoint information. This will be populated with the sender's address and port when EndReceive is called.
                endpoint = new IPEndPoint(IPAddress.Any, DiscoveryPort);

                // Consume the completed receive before starting another one because UdpClient reuses its receive buffer.
                byte[] receivedBytes = udpClient.EndReceive(asyncResult, ref endpoint);

                // Configure the UdpClient class to accept more messages while this message is being processed.
                udpClient.BeginReceive(ReceiveCallback, udpClient);
                callbackReenabled = true;

                // Convert the UDP message body to a string
                string ReceiveString = Encoding.ASCII.GetString(receivedBytes);

                // Default discovery to disallowed, then check the address and configuration to see if it is allowed
                bool discoveryAllowed = false;
                // Check for loopback address
                if (IPAddress.IsLoopback(endpoint.Address)) // Loopback addresses are always allowed
                {
                    discoveryAllowed = true;
                }

                // Check for local address
                else if (IsLocalIpAddress(endpoint.Address.ToString())) // This is a local address
                {
                    // Check if we are configured to only respond to local host addresses
                    if (!LocalRespondOnlyToLocalHost) // We are responding on all addresses
                    {
                        discoveryAllowed = true;
                    }
                }

                // Check if we are configured to allow remote access
                else if (AllowRemoteAccess) // We are configured to allow remote access, so allow discovery from this address
                {
                    discoveryAllowed = true;
                }

                // Check if discovery is allowed
                if (discoveryAllowed) // Discovery is allowed
                {
                    // Check whether the message is a discovery message
                    if (ReceiveString.Contains(Constants.DiscoveryMessage)) // This is a discovery message - NOTE: Uses Contains rather then Equals because of invisible padding garbage
                    {
                        IPEndPoint localEndPoint = udpClient.Client.LocalEndPoint as IPEndPoint;
                        bool isMulticast = MulticastGroups.TryGetValue(udpClient, out IPAddress multicastAddress);
                        LogInformation($"Responding to a discovery packet from {endpoint.Address} {endpoint.Port} to Ip address: {localEndPoint?.Address.ToString() ?? "none"}, port: {localEndPoint?.Port.ToString() ?? "none"}, multicast address: {multicastAddress?.ToString() ?? "none - unicast only"}.  at {DateTime.Now}");

                        // Create a response message containing the Alpaca port number
                        byte[] response = Encoding.ASCII.GetBytes($"{{\"AlpacaPort\": {port}}}");

                        // Send the response message back to the originator of the discovery message
                        udpClient.Send(response, response.Length, endpoint);
                    }
                }
                else // Discovery is not allowed so log to debug so we know what is happening
                {
                    LogDebug($"Ignoring discovery request from {endpoint.Address} because it is not a loopback address and configuration is: Only respond on localhost for local addresses: {LocalRespondOnlyToLocalHost}, AllowRemoteAccess: {AllowRemoteAccess}.");
                }
            }
            catch (ObjectDisposedException)
            {
                // This exception is expected when the UdpClient is disposed while waiting for a message. It can be ignored.
                callbackReenabled = true; // Set to true to prevent re-enabling the callback in the finally block
            }
            catch (Exception ex)
            {
                LogError($"Responder.ReceiveCallback - Error processing discovery request from {endpoint.Address}: {ex.Message}\r\n{ex}");
            }
            finally
            {
                if (!callbackReenabled)
                {
                    try
                    {
                        // If processing failed before the normal restart, resume listening where possible.
                        udpClient?.BeginReceive(ReceiveCallback, udpClient);
                    }
                    catch (Exception ex)
                    {
                        LogError($"Responder.ReceiveCallback - Unable to resume listening for discovery broadcasts: {ex.Message}");
                    }
                }
            }
        }

        //Use string so localhost works
        internal bool IsLocalIpAddress(string host)
        {
            try
            {
                IPAddress[] hostIPs = Dns.GetHostAddresses(host);

                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

                foreach (IPAddress hostIP in hostIPs)
                {
                    if (IPAddress.IsLoopback(hostIP)) return true;

                    foreach (IPAddress localIP in localIPs)
                    {
                        if (hostIP.Equals(localIP)) return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Creates a scoped multicast address for IPv6 multicast. This is used to bind a UDP client to a specific network interface for IPv6 multicast.
        /// </summary>
        /// <param name="address">The IPv6 multicast address.</param>
        /// <param name="interfaceIndex">The index of the network interface.</param>
        /// <returns>A scoped IPv6 multicast address.</returns>
        /// <exception cref="ArgumentException"></exception>
        private static IPAddress CreateScopedMulticastAddress(string address, int interfaceIndex)
        {
            IPAddress multicastAddress = IPAddress.Parse(address);

            if (multicastAddress.AddressFamily != AddressFamily.InterNetworkV6 || !multicastAddress.IsIPv6Multicast)
            {
                throw new ArgumentException($"'{address}' is not an IPv6 multicast address.", nameof(address));
            }

            return new IPAddress(multicastAddress.GetAddressBytes(), interfaceIndex);
        }

        /// <summary>
        /// Finds the index of a network interface by name. This is used to bind a UDP client to a specific network interface for IPv6 multicast.
        /// </summary>
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

        /// <summary>
        /// Logs an informational message using the provided ILogger instance, if available.
        /// </summary>
        /// <param name="message">The message to log.</param>
        private void LogInformation(string message)
        {
            Logger?.LogInformation(message);
        }

        /// <summary>
        /// Logs an error message using the provided ILogger instance, if available.
        /// </summary>
        /// <param name="message">The message to log.</param>
        private void LogError(string message)
        {
            Logger?.LogError(message);
        }
        
        /// <summary>
        /// Logs a debug message using the provided ILogger instance, if available.
        /// </summary>
        /// <param name="message">The message to log.</param>
        private void LogDebug(string message)
        {
            Logger?.LogDebug(message);
        }

        /// <summary>
        /// Used by the CLR when disposing of objects. Do not use this method, use <see cref="Dispose()"/> instead.
        /// </summary>
        /// <param name="disposing">Indicates whether the method is being called from the Dispose method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    foreach (UdpClient udp in Clients)
                    {
                        try
                        {
                            udp.Close();
                            udp.Dispose();
                        }
                        catch
                        {
                        }
                    }

                    Clients.Clear();
                    MulticastGroups.Clear();
                }
                Disposed = true;
            }
        }

        /// <summary>
        /// Dispose of the Responder object.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put clean-up code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}