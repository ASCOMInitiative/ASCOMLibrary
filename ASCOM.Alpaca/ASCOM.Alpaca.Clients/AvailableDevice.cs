using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// Represents an available Alpaca device.
    /// </summary>
    public struct AvailableDevice
    {
        /// <summary>
        /// The host name of the Alpaca device.
        /// </summary>
        public string HostName;

        /// <summary>
        /// The port number of the Alpaca device .
        /// </summary>
        public int Port;

        /// <summary>
        /// The IP address of the Alpaca device.
        /// </summary>
        public string IpAddress;

        /// <summary>
        /// The IP address of the Alpaca device represented as a BigInteger.
        /// </summary>
        public BigInteger IpAddressAsBigInteger;

        /// <summary>
        /// The distance between the IP address of the Alpaca device and the device's original IP address represented as a BigInteger.
        /// </summary>
        public BigInteger IpAddressDistance;
    }
}
