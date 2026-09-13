// (c) 2019 Daniel Van Noord
// This code is licensed under MIT license (see License.txt for details)

using ASCOM.Common;
using ASCOM.Common.Interfaces;
using System;
using System.Collections.Generic;
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

            Logger?.LogDebug($"Responder.ctor - Creating discovery responder with AlpacaPort: {AlpacaPort}, DiscoveryPort: {DiscoveryPort}, IPv4: {IPv4}, IPv6: {IPv6}");
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
            Logger?.LogDebug($"Responder.InitIPv4 - Binding to IPv4 Discovery Port: {DiscoveryPort}");
            UdpClient UDPClient = new UdpClient();

            UDPClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            UDPClient.EnableBroadcast = true;
            UDPClient.MulticastLoopback = false;
            UDPClient.ExclusiveAddressUse = false;

            UDPClient.Client.Bind(new IPEndPoint(IPAddress.Any, DiscoveryPort));

            // This uses begin receive rather then async so it works on net 3.5
            UDPClient.BeginReceive(ReceiveCallback, UDPClient);

            Clients.Add(UDPClient);
            Logger?.LogInformation($"Responder.InitIPv4 - Added discovery responder client on port: {DiscoveryPort}");
        }

        /// <summary>
        /// Bind a UDP client to each network interface and set the index and address for multicast
        /// </summary>
        private void InitIPv6()
        {
            Logger?.LogDebug($"Responder.InitIPv6 - Binding to IPv6 Discovery Port: {DiscoveryPort}");

            // Windows needs to have the IP Address and index set for an IPv6 multicast socket
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Get all the network interfaces on the system
                NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
                Logger?.LogDebug($"Responder.InitIPv6 - Running on Windows, found {adapters.Length} network adapters");

                // Enumerate the network interfaces
                foreach (NetworkInterface adapter in adapters)
                {
                    try
                    {
                        // Check if the interface is up and running
                        if (adapter.OperationalStatus != OperationalStatus.Up) // Interface is not up and running
                            continue;

                        Logger?.LogDebug($"Responder.InitIPv6 - Found network interface: {adapter.Name}, Type: {adapter.NetworkInterfaceType}, Status: {adapter.OperationalStatus}, Supports IPv6: {adapter.Supports(NetworkInterfaceComponent.IPv6)}, Supports Multicast: {adapter.SupportsMulticast}");

                        // Check if this interface supports IPv6 and multicast
                        if (adapter.Supports(NetworkInterfaceComponent.IPv6) && adapter.SupportsMulticast) // Interface supports IPv6 and multicast
                        {
                            // Get the IP properties of the interface and ignore the interface if it does not have any descriptive properties
                            IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                            if (adapterProperties == null)
                                continue;

                            // Get the unicast addresses of the interface
                            UnicastIPAddressInformationCollection uniCast = adapterProperties.UnicastAddresses;
                            Logger?.LogDebug($"Responder.InitIPv6 -   Interface: {adapter.Name} supports IPv6 and Multicast and has {uniCast.Count} unicast addresses");

                            // Check whether the interface has any unicast addresses and ignore the interface if it does not
                            if (uniCast.Count > 0) // Interface has unicast addresses
                            {
                                // Enumerate the unicast addresses of the interface 
                                foreach (UnicastIPAddressInformation uni in uniCast)
                                {
                                    try
                                    {
                                        Logger?.LogDebug($"Responder.InitIPv6 -   Interface: {adapter.Name} has address: {uni.Address} with address family: {uni.Address.AddressFamily}.");

                                        // Check if the unicast address is an IPv6 address
                                        if (uni.Address.AddressFamily != AddressFamily.InterNetworkV6) // Unicast address is not an IPv6 address so ignore it
                                            continue;

                                        Logger?.LogDebug($"Responder.InitIPv6 -   Found IPv6 address: {uni.Address}, LinkLocal: {uni.Address.IsIPv6LinkLocal}, Is localhost: {IPAddress.IsLoopback(uni.Address)}");

                                        // Check whether the unicast address is a LinkLocal or LocalHost address
                                        // NOTE - Must test for localhost / loopback first because this address returns IsIPv6LinkLocal = false. The "Conditional logical OR (short circuit)" operator || ensures the logic works as intended.
                                        if (IPAddress.IsLoopback(uni.Address) || uni.Address.IsIPv6LinkLocal) // The address is a LinkLocal or LocalHost address so process it
                                        {
                                            Clients.Add(NewClient(uni.Address, adapterProperties.GetIPv6Properties().Index));
                                            Logger?.LogInformation($"Responder.InitIPv6 -   Added discovery responder client for IPv6 address: {uni.Address}, Index: {adapterProperties.GetIPv6Properties().Index} on port {DiscoveryPort}");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger?.LogDebug($"Responder.InitIPv6 - Exception occurred while enumerating addresses: {ex.Message}\r\n{ex}");
                                        //May not have permission to access a specific socket or address
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger?.LogDebug($"Responder.InitIPv6 - Exception occurred while enumerating adapters: {ex.Message}\r\n{ex}");
                        //May not have permission to access a specific socket or address
                    }
                }
            }
            else
            {
                //Linux does not, it handles the binding
                Clients.Add(NewClient(IPAddress.IPv6Any, 0));
            }
            Logger?.LogDebug($"Responder.InitIPv6 - Completed binding to IPv6 Discovery Port: {DiscoveryPort}");
        }

        private UdpClient NewClient(IPAddress host, int index)
        {
            UdpClient UDPClientV6 = new UdpClient(AddressFamily.InterNetworkV6);

            UDPClientV6.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            UDPClientV6.ExclusiveAddressUse = false;

            UDPClientV6.Client.Bind(new IPEndPoint(host, DiscoveryPort));

            UDPClientV6.JoinMulticastGroup(index, IPAddress.Parse(Constants.MulticastGroup));
            // This uses begin receive rather then async so it works on net 3.5
            UDPClientV6.BeginReceive(ReceiveCallback, UDPClientV6);

            return UDPClientV6;
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient udpClient = null;
            IPEndPoint endpoint = null;
            bool callbackReenabled = false;

            try
            {
                udpClient = (UdpClient)ar.AsyncState;

                endpoint = new IPEndPoint(IPAddress.Any, DiscoveryPort);

                // Consume the completed receive before starting another one because UdpClient reuses its receive buffer.
                byte[] receivedBytes = udpClient.EndReceive(ar, ref endpoint);

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
                        //For testing only
                        Logger?.LogInformation($"Responding to a discovery packet from {endpoint.Address} at {DateTime.Now}");

                        // Create a response message containing the Alpaca port number
                        byte[] response = Encoding.ASCII.GetBytes($"{{\"AlpacaPort\": {port}}}");

                        // Send the response message back to the originator of the discovery message
                        udpClient.Send(response, response.Length, endpoint);
                    }
                }
                else // Discovery is not allowed so log to debug so we know what is happening
                {
                    Logger?.LogDebug($"Ignoring discovery request from {endpoint.Address} because it is not a loopback address and configuration is: Only respond on localhost for local addresses: {LocalRespondOnlyToLocalHost}, AllowRemoteAccess: {AllowRemoteAccess}.");
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError($"Responder.ReceiveCallback - Error processing discovery request from {endpoint.Address}: {ex.Message}");
            }
            finally
            {
                if (!callbackReenabled)
                {
                    try
                    {
                        // If processing failed before the normal restart, continue listening where possible.
                        udpClient?.BeginReceive(ReceiveCallback, udpClient);
                    }
                    catch (Exception ex)
                    {
                        Logger?.LogError($"Responder.ReceiveCallback - Unable to continue listening for discovery broadcasts: {ex.Message}");
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
                Logger?.LogError(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Used by the CLR when disposing of objects. Do not use this method, use <see cref="Dispose()"/> instead.
        /// </summary>
        /// <param name="disposing"></param>
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