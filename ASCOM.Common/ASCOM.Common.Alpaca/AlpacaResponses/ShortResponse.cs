namespace ASCOM.Common.Alpaca
{
    public class ShortResponse : Response
    {
        public ShortResponse() { }

        public ShortResponse(uint clientTransactionID, uint transactionID, short value)
        {
            base.ServerTransactionID = transactionID;
            base.ClientTransactionID = clientTransactionID;
            Value = value;
        }

        public short Value { get; set; }
    }
}
