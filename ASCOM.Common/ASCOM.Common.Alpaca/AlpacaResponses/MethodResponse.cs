namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// JSON response for a method that doesn't return a value.
    /// </summary>
    public class MethodResponse : Response
    {
        /// <summary>
        /// Create a new uninitialised instance
        /// </summary>
        public MethodResponse() { }

        /// <summary>
        /// Create a new instance initialised with client transaction ID and server transaction ID
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        public MethodResponse(uint clientTransactionID, uint serverTransactionID)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
        }
        // No additional fields for this class
    }
}
