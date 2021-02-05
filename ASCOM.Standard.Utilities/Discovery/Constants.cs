using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCOM.Standard.Discovery
{
    public static class Constants
    {
        public const string DiscoveryMessage = "alpacadiscovery1";
        public const int DiscoveryPort = 32227;
        public const string ResponseString = "alpacaport";
        public const string MulticastGroup = "ff12::00a1:9aca";

        public static byte[] DiscoveryMessageArray 
        {
            get
            {
                return Encoding.ASCII.GetBytes(DiscoveryMessage);
            }
        }
    }
}
