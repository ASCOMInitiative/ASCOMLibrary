using ASCOM.Common.DeviceInterfaces;
using System.Collections.Generic;

namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// Response that returns a tracking rates collection.
    /// </summary>
    public class TrackingRatesResponse : Response
    {
        private List<DriveRate> rates;

        /// <summary>
        /// Create a new TrackingRatesResponse with default values
        /// </summary>
        public TrackingRatesResponse() { }

        /// <summary>
        /// Create a new TrackingRatesResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        public TrackingRatesResponse(uint clientTransactionID, uint serverTransactionID)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
        }

        /// <summary>
        /// Set or return a list of tracking rates
        /// </summary>
        public List<DriveRate> Value
        {
            get { return rates; }
            set { rates = value; }
        }
    }
}
