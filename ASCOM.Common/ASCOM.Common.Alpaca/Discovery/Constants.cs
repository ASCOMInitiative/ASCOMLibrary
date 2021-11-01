using System.Text;

namespace ASCOM.Alpaca.Discovery
{
    public static class Constants
    {
        public const string DiscoveryMessage = "alpacadiscovery1";
        public const int DiscoveryPort = 32227;
        public const string ResponseString = "AlpacaPort";
        public const string MulticastGroup = "ff12::00a1:9aca";
        public const string TRYING_TO_CONTACT_MANAGEMENT_API_MESSAGE = "Trying to contact Alpaca management API";
        public const string FAILED_TO_CONTACT_MANAGEMENT_API_MESSAGE = "The Alpaca management API did not respond within the discovery response time";
        public const double MINIMUM_TIME_REMAINING_TO_UNDERTAKE_DNS_RESOLUTION = 0.1d; // Minimum discovery time (seconds) that must remain if a DNS IP to host name resolution is to be attempted
        public const int NUMBER_OF_THREAD_MESSAGE_INDENT_SPACES = 2;

        public static byte[] DiscoveryMessageArray
        {
            get
            {
                return Encoding.ASCII.GetBytes(DiscoveryMessage);
            }
        }
    }
}
