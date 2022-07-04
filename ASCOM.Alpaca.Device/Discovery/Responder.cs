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
    /// Enable Alpaca devices to respond to Alpaca discovery broadcasts by returning the Alpaca port number
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
        /// Create and listen on an IPv4 broadcast port
        /// </summary>
        private void InitIPv4()
        {
            UdpClient UDPClient = new UdpClient();

            UDPClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            UDPClient.EnableBroadcast = true;
            UDPClient.MulticastLoopback = false;
            UDPClient.ExclusiveAddressUse = false;

            UDPClient.Client.Bind(new IPEndPoint(IPAddress.Any, DiscoveryPort));

            // This uses begin receive rather then async so it works on net 3.5
            UDPClient.BeginReceive(ReceiveCallback, UDPClient);

            Clients.Add(UDPClient);
        }

        /// <summary>
        /// Bind a UDP client to each network adapter and set the index and address for multicast
        /// </summary>
        private void InitIPv6()
        {
            // Windows needs to have the IP Address and index set for an IPv6 multicast socket
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();

                foreach (var adapter in adapters)
                {
                    try
                    {
                        if (adapter.OperationalStatus != OperationalStatus.Up)
                            continue;

                        if (adapter.Supports(NetworkInterfaceComponent.IPv6) && adapter.SupportsMulticast)
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
                                        if (uni.Address.AddressFamily != AddressFamily.InterNetworkV6)
                                            continue;

                                        //Only use LinkLocal or LocalHost addresses
                                        if (!uni.Address.IsIPv6LinkLocal && uni.Address != IPAddress.Parse("::1"))
                                            continue;

                                        Clients.Add(NewClient(uni.Address, adapterProperties.GetIPv6Properties().Index));
                                    }
                                    catch
                                    {
                                        //May not have permission to access a specific socket or address
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        //May not have permission to access a specific socket or address
                    }
                }
            }
            else
            {
                //Linux does not, it handles the binding
                Clients.Add(NewClient(IPAddress.IPv6Any, 0));
            }
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
            try
            {
                udpClient = (UdpClient)ar.AsyncState;

                IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, DiscoveryPort);

                // Obtain the UDP message body and convert it to a string, with remote IP address attached as well
                string ReceiveString = Encoding.ASCII.GetString(udpClient.EndReceive(ar, ref endpoint));

                bool discoveryAllowed = false;

                if (IPAddress.IsLoopback(endpoint.Address))
                {
                    discoveryAllowed = true;
                }
                else if (IsLocalIpAddress(endpoint.Address.ToString()))
                {
                    if (!LocalRespondOnlyToLocalHost)
                    {
                        discoveryAllowed = true;
                    }
                }
                else if (AllowRemoteAccess)
                {
                    discoveryAllowed = true;
                }

                if (discoveryAllowed)
                {
                    if (ReceiveString.Contains(Constants.DiscoveryMessage))//Contains rather then equals because of invisible padding garbage
                    {
                        //For testing only
                        Logger?.LogInformation(string.Format("Received a discovery packet from {0} at {1}", endpoint.Address, DateTime.Now));

                        byte[] response = Encoding.ASCII.GetBytes(string.Format("{{\"AlpacaPort\": {0}}}", port));

                        udpClient.Send(response, response.Length, endpoint);
                    }
                }

                // Configure the UdpClient class to accept more messages, if they arrive
                udpClient.BeginReceive(ReceiveCallback, udpClient);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex.Message);
            }
            finally
            {
                try
                {
                    // Configure the UdpClient class to accept more messages, if they arrive
                    udpClient.BeginReceive(ReceiveCallback, udpClient);
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex.Message);
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
                    foreach (var udp in Clients)
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
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}