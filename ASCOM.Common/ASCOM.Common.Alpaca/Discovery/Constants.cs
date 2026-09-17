using System.Text;

namespace ASCOM.Alpaca.Discovery
{
    /// <summary>
    /// Constants related to Alpaca discovery
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Discovery broadcast message from an Alpaca client
        /// </summary>
        public const string DiscoveryMessage = "alpacadiscovery1";

        /// <summary>
        /// Default Alpaca discovery broadcast / listening port
        /// </summary>
        public const int DiscoveryPort = 32227;

        /// <summary>
        /// JSON Property name for the Alpaca device's listening port
        /// </summary>
        public const string ResponseString = "AlpacaPort";

        /// <summary>
        /// IPv6 multicast group address for discovery over an IPv6 network
        /// </summary>
        public const string MulticastGroup = "ff12::a1:9aca"; // Link-local scope

        /// <summary>
        /// IPv6 multicast group address for discovery over an IPv6 network
        /// </summary>
        public const string MulticastGroupIpV6Loopback = "ff11::a1:9aca"; // Host only scope

        /// <summary>
        /// The name of the loopback interface on Linux and MacOS
        /// </summary>
        public const string UnixLoopbackInterfaceName = "lo";

        /// <summary>
        /// Returns the ALpaca discovery message as a byte array
        /// </summary>
        public static byte[] DiscoveryMessageArray
        {
            get
            {
                return Encoding.ASCII.GetBytes(DiscoveryMessage);
            }
        }
    }
}
