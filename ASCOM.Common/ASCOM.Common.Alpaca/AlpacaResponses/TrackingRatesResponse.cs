using ASCOM.Common.DeviceInterfaces;
using System.Collections.Generic;

namespace ASCOM.Common.Alpaca
{
    public class TrackingRatesResponse : Response
    {
        private List<DriveRate> rates;

        public TrackingRatesResponse() { }

        public TrackingRatesResponse(uint clientTransactionID, uint transactionID)
        {
            base.ServerTransactionID = transactionID;
            base.ClientTransactionID = clientTransactionID;
        }

        public List<DriveRate> Value
        {
            get { return rates; }
            set { rates = value; }
        }
    }
}
