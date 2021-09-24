﻿using System;
using System.Collections.Generic;
using ASCOM.Standard.Interfaces;

namespace ASCOM.Alpaca.Responses
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
