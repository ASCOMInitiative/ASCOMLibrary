using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace ASCOM.Standard.Discovery
{
    public class Finder : IDisposable
    {
        public event EventHandler<IPEndPoint> ResponseReceivedEvent;

        private UdpClient IPv4Client = new UdpClient()
        {
            EnableBroadcast = true,
            MulticastLoopback = false
        };

        private Dictionary<IPAddress, UdpClient> IPv6Clients = new Dictionary<IPAddress, UdpClient>();

        private bool disposedValue;

        /// <summary>
        /// A cache of all endpoints found by the server
        /// </summary>
        public List<IPEndPoint> CachedEndpoints
        {
            get;
        } = new List<IPEndPoint>();

        /// <summary>
        /// Creates a Alpaca Finder object that sends out a search request for Alpaca devices
        /// The results will be sent to the callback and stored in the cache
        /// Calling search and concatenating the results reduces the chance that a UDP packet is lost
        /// This may require firewall access
        /// </summary>
        public Finder()
        {
            //0 tells OS to give us a free ethereal port
            IPv4Client.Client.Bind(new IPEndPoint(IPAddress.Any, 0));

            IPv4Client.BeginReceive(ReceiveCallback, IPv4Client);
        }

        /// <summary>
        /// Send out discovery message on each IPv4 broadcast address
        /// This dual targets NetStandard 2.0 and NetFX 3.5 so no Async Await
        /// Broadcasts on each adapters address as per Windows / Linux documentation
        /// </summary>
        private void SearchIPv4()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
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

                                    // Local host addresses (127.*.*.*) may have a null mask in Net Framework. We do want to search these. The correct mask is 255.0.0.0.
                                    IPv4Client.Send(Constants.DiscoveryMessageArray, Constants.DiscoveryMessageArray.Length, new IPEndPoint(GetBroadcastAddress(uni.Address, uni.IPv4Mask ?? IPAddress.Parse("255.0.0.0")), Constants.DiscoveryPort));
                                }
                                catch (Exception ex)
                                {
                                    ASCOM.Standard.Utilities.Logger.LogError(ex.Message);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ASCOM.Standard.Utilities.Logger.LogError(ex.Message);
                }
            }
        }

        /// <summary>
        /// Send out discovery message on the IPv6 multicast group
        /// This dual targets NetStandard 2.0 and NetFX 3.5 so no Async Await
        /// </summary>
        private void SearchIPv6()
        {
            // Windows needs to bind a socket to each adapter explicitly
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces())
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

                                        try
                                        {
                                            if (!IPv6Clients.ContainsKey(uni.Address))
                                            {
                                                IPv6Clients.Add(uni.Address, NewClient(uni.Address, 0));
                                            }

                                            IPv6Clients[uni.Address].Send(Constants.DiscoveryMessageArray, Constants.DiscoveryMessageArray.Length, new IPEndPoint(IPAddress.Parse(Constants.MulticastGroup), Constants.DiscoveryPort));
                                        }
                                        catch (SocketException)
                                        {
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ASCOM.Standard.Utilities.Logger.LogError(ex.Message);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ASCOM.Standard.Utilities.Logger.LogError(ex.Message);
                    }
                }
            }
            else
            {
                if (!IPv6Clients.ContainsKey(IPAddress.IPv6Any))
                {
                    IPv6Clients.Add(IPAddress.IPv6Any, NewClient(IPAddress.IPv6Any, 0));
                }

                IPv6Clients[IPAddress.IPv6Any].Send(Constants.DiscoveryMessageArray, Constants.DiscoveryMessageArray.Length, new IPEndPoint(IPAddress.Parse(Constants.MulticastGroup), Constants.DiscoveryPort));
            }
        }

        private UdpClient NewClient(IPAddress host, int port)
        {
            var client = new UdpClient(AddressFamily.InterNetworkV6);

            //0 tells OS to give us a free ethereal port
            client.Client.Bind(new IPEndPoint(host, port));

            client.BeginReceive(ReceiveCallback, client);

            client.Send(Constants.DiscoveryMessageArray, Constants.DiscoveryMessageArray.Length, new IPEndPoint(IPAddress.Parse(Constants.MulticastGroup), Constants.DiscoveryPort));

            return client;
        }

        /// <summary>
        /// Send out the IPv4 and IPv6 messages
        /// </summary>
        private void SendDiscoveryMessage(bool searchIPv4, bool searchIPv6)
        {
            if (searchIPv4) { SearchIPv4(); }

            if (searchIPv6) { SearchIPv6(); }
        }

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

        // This dual targets NetStandard 2.0 and NetFX 3.5 so no Async Await
        // This callback is shared between IPv4 and IPv6
        private void ReceiveCallback(IAsyncResult ar)
        {
            IPEndPoint endpoint = null;
            try
            {
                UdpClient udpClient = (UdpClient)ar.AsyncState;

                endpoint = new IPEndPoint(IPAddress.Any, Constants.DiscoveryPort);

                // Obtain the UDP message body and convert it to a string, with remote IP address attached as well
                string ReceiveString = Encoding.ASCII.GetString(udpClient.EndReceive(ar, ref endpoint));

                // Configure the UdpClient class to accept more messages, if they arrive
                udpClient.BeginReceive(ReceiveCallback, udpClient);

                //Do not report your own transmissions
                if (ReceiveString.Contains(Constants.ResponseString))
                {
                    var port = JsonSerializer.Deserialize<Response>(ReceiveString).AlpacaPort;

                    if (port == 0) //Failed to parse
                    {
                        throw new Exception($"Failed to parse {ReceiveString} into an Alpaca Port");
                    }

                    var alpacaEndpoint = new IPEndPoint(endpoint.Address, port);
                    if (!CachedEndpoints.Contains(alpacaEndpoint))
                    {
                        CachedEndpoints.Add(alpacaEndpoint);

                        ResponseReceivedEvent?.Invoke(this, alpacaEndpoint);
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.Logger.LogError($"Failed to parse response from {endpoint} with exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Resends the search request for IPv4 and IPv6
        /// </summary>
        public void Search(bool IPv4 = true, bool IPv6 = true)
        {
            if (!IPv4 && !IPv6)
            {
                throw new ArgumentException("You must search on one or more protocol types.");
            }
            SendDiscoveryMessage(IPv4, IPv6);
        }

        /// <summary>
        /// Clears the cached IP Endpoints in CachedEndpoints
        /// </summary>
        public void ClearCache()
        {
            CachedEndpoints.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //Dispose IPv4
                    try
                    {
                        IPv4Client.Dispose();
                    }
                    catch
                    {
                    }

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

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}