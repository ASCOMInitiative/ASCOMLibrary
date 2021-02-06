using ASCOM.Standard.Interfaces;

namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// Response that returns an <see cref="AlignmentMode"/> value.
    /// </summary>
    public class AlignmentModeResponse : Response, IValueResponse<AlignmentMode>
    {
        /// <summary>
        /// Create a new AlignmentModeResponse with default values
        /// </summary>
        public AlignmentModeResponse()
        {
        }

        /// <summary>
        /// Create a new AlignmentModeResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public AlignmentModeResponse(uint clientTransactionID, uint serverTransactionID, AlignmentMode value)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            Value = value;
        }

        /// <summary>
        /// Create a new AlignmentModeResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="errorMessage">Value to return</param>
        /// <param name="errorCode">Server transaction ID</param>
        public AlignmentModeResponse(uint clientTransactionID, uint serverTransactionID, string errorMessage, ErrorCodes errorCode)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            base.ErrorMessage = errorMessage;
            base.ErrorNumber = errorCode;
        }

        /// <summary>
        /// Alignment mode returned by the device
        /// </summary>
        public AlignmentMode Value { get; set; }

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