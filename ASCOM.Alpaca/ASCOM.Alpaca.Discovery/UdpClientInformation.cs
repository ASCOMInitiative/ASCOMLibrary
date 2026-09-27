using System;
using System.Net;
using System.Net.Sockets;

namespace ASCOM.Alpaca.Discovery
{
    /// <summary>
    /// Represents the state of a UDP client.
    /// </summary>
    public class UdpClientInformation : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UdpClientInformation"/> class.
        /// </summary>
        public UdpClientInformation()
        {
            UdpClient = null;
            InterfaceNumber = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpClientInformation"/> class with the specified UDP client and interface number.
        /// </summary>
        /// <param name="udpClient">The UDP client.</param>
        /// <param name="interfaceNumber">The interface number.</param>
        public UdpClientInformation(UdpClient udpClient, int interfaceNumber)
        {
            UdpClient = udpClient;
            InterfaceNumber = interfaceNumber;
        }

        /// <summary>
        /// The UDP client.
        /// </summary>
        public UdpClient UdpClient { get; set; } = null;

        /// <summary>
        /// The interface number associated with the UDP client.
        /// </summary>
        public int InterfaceNumber { get; set; } = 0;

        /// <summary>
        /// Disposes the UDP client.
        /// </summary>
        public void Dispose()
        {
            UdpClient?.Close();
            UdpClient?.Dispose();
        }
    }
}
