namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// Response that returns a short integer value.
    /// </summary>
    public class ShortResponse : Response
    {
        /// <summary>
        /// Create a new ShortResponse with default values
        /// </summary>
        public ShortResponse() { }

        /// <summary>
        /// Create a new IntResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public ShortResponse(uint clientTransactionID, uint serverTransactionID, short value)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            Value = value;
        }

        /// <summary>
        /// Integer value returned by the device
        /// </summary>
        public short Value { get; set; }
    }
}
