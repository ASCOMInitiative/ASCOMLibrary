using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ASCOM.Alpaca.Clients
{
    internal struct AvailableInterface
    {
        /// <summary>
        /// The name of the host computer that is running the device server.
        /// </summary>
        internal string HostName;

        /// <summary>
        /// The port number on which the device server is listening.
        /// </summary>
        internal int Port;

        /// <summary>
        /// The IP address of the host computer that is running the device server.
        /// </summary>
        internal string IpAddress;

        /// <summary>
        /// The distance between the IP address of the host computer and the local computer, represented as a BigInteger. This can be used to determine the proximity of the device server to the local computer.
        /// </summary>
        internal BigInteger AddressDistance;
    }
}
