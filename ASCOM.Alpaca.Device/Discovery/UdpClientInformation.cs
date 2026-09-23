using System;
using System.Net;
using System.Net.Sockets;

namespace ASCOM.Alpaca.Discovery
{
    /// <summary>
    /// Represents the state of a UDP client.
    /// </summary>
    public class UdpClientInformation : IDisposable, IEquatable<UdpClientInformation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UdpClientInformation"/> class.
        /// </summary>
        public UdpClientInformation()
        {
            UdpClient = null;
            MulticastAddress = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpClientInformation"/> class with the specified UDP client and multicast address.
        /// </summary>
        /// <param name="udpClient">The UDP client.</param>
        /// <param name="multicastAddress">The multicast address.</param>
        public UdpClientInformation(UdpClient udpClient, IPAddress multicastAddress)
        {
            UdpClient = udpClient;
            MulticastAddress = multicastAddress;
        }

        /// <summary>
        /// The UDP client.
        /// </summary>
        public UdpClient UdpClient { get; set; } = null;

        /// <summary>
        /// The multicast address associated with the UDP client.
        /// </summary>
        public IPAddress MulticastAddress { get; set; } = null;

        /// <summary>
        /// Determines whether this instance and another instance represent the same UDP client configuration.
        /// </summary>
        /// <param name="other">The other UDP client information instance.</param>
        /// <returns><see langword="true"/> when the local IP addresses and ports, and multicast addresses match.</returns>
        public bool Equals(UdpClientInformation other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            IPEndPoint localEndPoint = GetLocalEndPoint();
            IPEndPoint otherLocalEndPoint = other.GetLocalEndPoint();

            return Equals(localEndPoint?.Address, otherLocalEndPoint?.Address)
                && (localEndPoint?.Port ?? 0) == (otherLocalEndPoint?.Port ?? 0)
                && IPAddress.Equals(MulticastAddress, other.MulticastAddress);
        }

        /// <summary>
        /// Determines whether this instance and another object represent the same UDP client configuration.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true"/> when the object represents the same UDP client configuration.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as UdpClientInformation);
        }

        /// <summary>
        /// Returns a hash code for this UDP client configuration.
        /// </summary>
        /// <returns>A hash code based on the local IP address, port, and multicast address.</returns>
        public override int GetHashCode()
        {
            IPEndPoint localEndPoint = GetLocalEndPoint();
            unchecked
            {
                int hashCode = 17;
                hashCode = (hashCode * 31) + (localEndPoint?.Address?.GetHashCode() ?? 0);
                hashCode = (hashCode * 31) + (localEndPoint?.Port ?? 0);
                hashCode = (hashCode * 31) + (MulticastAddress?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        /// <summary>
        /// Disposes the UDP client.
        /// </summary>
        public void Dispose()
        {
            UdpClient?.Close();
            UdpClient?.Dispose();
        }

        private IPEndPoint GetLocalEndPoint()
        {
            return UdpClient?.Client?.LocalEndPoint as IPEndPoint;
        }
    }
}
