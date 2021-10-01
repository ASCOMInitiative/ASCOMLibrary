using ASCOM.Common.DeviceInterfaces;

namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// Response that returns a <see cref="PointingState"/> value.
    /// </summary>
    public class PointingStateResponse : Response, IValueResponse<PointingState>
    {
        /// <summary>
        /// Create a new PointingStateResponse with default values
        /// </summary>
        public PointingStateResponse()
        {
        }

        /// <summary>
        /// Create a new PointingStateResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public PointingStateResponse(uint clientTransactionID, uint serverTransactionID, PointingState value)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            Value = value;
        }

        /// <summary>
        /// Create a new PointingStateResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="errorMessage">Value to return</param>
        /// <param name="errorCode">Server transaction ID</param>
        public PointingStateResponse(uint clientTransactionID, uint serverTransactionID, string errorMessage, AlpacaErrors errorCode)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            base.ErrorMessage = errorMessage;
            base.ErrorNumber = errorCode;
        }

        /// <summary>
        /// PointingState value returned by the device
        /// </summary>
        public PointingState Value { get; set; }

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