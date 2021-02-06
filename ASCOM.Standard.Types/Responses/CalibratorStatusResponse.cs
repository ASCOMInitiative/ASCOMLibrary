using ASCOM.Standard.Interfaces;

namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// Response that returns an <see cref="CalibratorStatus"/> value.
    /// </summary>
    public class CalibratorStatusResponse : Response, IValueResponse<CalibratorStatus>
    {
        /// <summary>
        /// Create a new CalibratorStatusResponse with default values
        /// </summary>
        public CalibratorStatusResponse()
        {
        }

        /// <summary>
        /// Create a new CalibratorStatusResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public CalibratorStatusResponse(uint clientTransactionID, uint serverTransactionID, CalibratorStatus value)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            Value = value;
        }

        /// <summary>
        /// Create a new CalibratorStatusResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="errorMessage">Value to return</param>
        /// <param name="errorCode">Server transaction ID</param>
        public CalibratorStatusResponse(uint clientTransactionID, uint serverTransactionID, string errorMessage, ErrorCodes errorCode)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            base.ErrorMessage = errorMessage;
            base.ErrorNumber = errorCode;
        }

        /// <summary>
        /// CalibratorStatus returned by the device
        /// </summary>
        public CalibratorStatus Value { get; set; }

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