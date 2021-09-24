﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCOM.Alpaca.Responses
{
    public class StringArrayResponse : Response
    {
        private string[] stringArray;

        public StringArrayResponse() { }

        public StringArrayResponse(uint clientTransactionID, uint transactionID, string[] value)
        {
            base.ServerTransactionID = transactionID;
            stringArray = value;
            base.ClientTransactionID = clientTransactionID;
        }

        public string[] Value
        {
            get { return stringArray; }
            set { stringArray = value; }
        }
    }
}
