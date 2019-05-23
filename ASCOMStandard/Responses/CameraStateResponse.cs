using ASCOM.Alpaca.Interfaces;

namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// Response that returns a <see cref="CameraState"/> value.
    /// </summary>
    public class CameraStateResponse : Response, IValueResponse<CameraState>
    {
        /// <summary>
        /// Create a new CameraStateResponse with default values
        /// </summary>
        public CameraStateResponse()
        {
        }

        /// <summary>
        /// Create a new CameraStateResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public CameraStateResponse(uint clientTransactionID, uint serverTransactionID, CameraState value)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            Value = value;
        }

        /// <summary>
        /// Create a new CameraStateResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="errorMessage">Value to return</param>
        /// <param name="errorCode">Server transaction ID</param>
        public CameraStateResponse(uint clientTransactionID, uint serverTransactionID, string errorMessage, ErrorCodes errorCode)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            base.ErrorMessage = errorMessage;
            base.ErrorNumber = errorCode;
        }

        /// <summary>
        /// Camera state returned by the device
        /// </summary>
        public CameraState Value { get; set; }

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