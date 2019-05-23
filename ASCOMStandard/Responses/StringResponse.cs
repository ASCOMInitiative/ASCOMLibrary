namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// Response that returns a string value
    /// </summary>
    public class StringResponse : Response, IValueResponse<string>
    {
        /// <summary>
        /// Create a new StringResponse with default values
        /// </summary>
        public StringResponse()
        {
        }

        /// <summary>
        /// Create a new StringResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public StringResponse(uint clientTransactionID, uint serverTransactionID, string value)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            Value = value;
        }

        /// <summary>
        /// Create a new StringResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="errorMessage">Value to return</param>
        /// <param name="errorCode">Server transaction ID</param>
        public StringResponse(uint clientTransactionID, uint serverTransactionID, string errorMessage, ErrorCodes errorCode)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            base.ErrorMessage = errorMessage;
            base.ErrorNumber = errorCode;
        }

        /// <summary>
        /// String value returned by the device
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Return the value as a string
        /// </summary>
        /// <returns>String representation of the response value</returns>
        public override string ToString()
        {
            // Return the sting value or an empty string if the value is null
            if (Value == null) return string.Empty;
            return Value.ToString();
        }
    }
}