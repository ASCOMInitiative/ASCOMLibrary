using System;

namespace ASCOM.Alpaca.Responses
{
    internal enum MethodTypes
    {
        PropertyGet,
        PropertySet,
        Method,
        Function
    }

    public class MethodResponse : Response
    {
        public MethodResponse() { }
        public MethodResponse(uint clientTransactionID, uint transactionID)
        {
            base.ServerTransactionID = transactionID;
            base.ClientTransactionID = clientTransactionID; 
        }
        // No additional fields for this class
    }
}
