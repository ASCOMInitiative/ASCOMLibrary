namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// Response that returns a string array value.
    /// </summary>
    public class StringArrayResponse : Response
    {
        private string[] stringArray;

        /// <summary>
        /// Create a new StringArrayResponse with default values
        /// </summary>
        public StringArrayResponse() { }

        /// <summary>
        /// Create a new StringArrayResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public StringArrayResponse(uint clientTransactionID, uint serverTransactionID, string[] value)
        {
            base.ServerTransactionID = serverTransactionID;
            stringArray = value;
            base.ClientTransactionID = clientTransactionID;
        }

        /// <summary>
        /// String array returned by the device
        /// </summary>
        public string[] Value
        {
            get { return stringArray; }
            set { stringArray = value; }
        }
    }
}
