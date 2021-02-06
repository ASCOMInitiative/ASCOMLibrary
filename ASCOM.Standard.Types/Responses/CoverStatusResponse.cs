using ASCOM.Standard.Interfaces;

namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// Response that returns an <see cref="CoverStatus"/> value.
    /// </summary>
    public class CoverStatusResponse : Response, IValueResponse<CoverStatus>
    {
        /// <summary>
        /// Create a new CoverStatusResponse with default values
        /// </summary>
        public CoverStatusResponse()
        {
        }

        /// <summary>
        /// Create a new CoverStatusResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public CoverStatusResponse(uint clientTransactionID, uint serverTransactionID, CoverStatus value)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            Value = value;
        }

        /// <summary>
        /// Create a new CoverStatusResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="errorMessage">Value to return</param>
        /// <param name="errorCode">Server transaction ID</param>
        public CoverStatusResponse(uint clientTransactionID, uint serverTransactionID, string errorMessage, ErrorCodes errorCode)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            base.ErrorMessage = errorMessage;
            base.ErrorNumber = errorCode;
        }

        /// <summary>
        /// CoverStatus returned by the device
        /// </summary>
        public CoverStatus Value { get; set; }

        /// <summary>
        /// Return the value as a string
        /// </summary>
        /// <returns>String representation of the response value</returns>
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}