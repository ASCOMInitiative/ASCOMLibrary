using System.Net;
using System.Threading;

namespace ASCOM.Alpaca.Discovery
{

    /// <summary>
    /// Define the state object for the callback. 
    /// Use hostName to correlate calls with the proper result.
    /// </summary>
    public class DnsResponse
    {
        private IPHostEntry f_IpHostEntry;

        /// <summary>
        /// Initialise the class with a new ManualResetEvent
        /// </summary>
        public DnsResponse()
        {
            CallComplete = new ManualResetEvent(false);
        }

        /// <summary>
        /// The IPHostEntry for the discovered Alpaca device
        /// </summary>
        public IPHostEntry IpHostEntry
        {
            get
            {
                return f_IpHostEntry;
            }

            set
            {
                // Save the value and populate the other DnsResponse fields
                f_IpHostEntry = value;
                HostName = value.HostName;
                Aliases = value.Aliases;
                AddressList = value.AddressList;
            }
        }

        /// <summary>
        /// The discovery's ManualResetEvent
        /// </summary>
        public ManualResetEvent CallComplete { get; set; }

        /// <summary>
        /// The Alpaca device's host name
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// The device's aliases
        /// </summary>
        public string[] Aliases { get; set; }

        /// <summary>
        /// The list of IP addresses presented by the discovered device
        /// </summary>
        public IPAddress[] AddressList { get; set; }
    }
}