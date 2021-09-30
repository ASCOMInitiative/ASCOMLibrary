using ASCOM.Common.DeviceInterfaces;

namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// Response that return the value as a <see cref="ShutterState"/>
    /// </summary>
    public class ShutterStateResponse : Response, IValueResponse<ShutterState>
    {
        /// <summary>
        /// Create a new ShutterStateResponse with default values
        /// </summary>
        public ShutterStateResponse()
        {
        }

        /// <summary>
        /// Create a new ShutterStateResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public ShutterStateResponse(uint clientTransactionID, uint serverTransactionID, ShutterState value)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            Value = value;
        }

        /// <summary>
        /// Create a new ShutterStateResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="errorMessage">Value to return</param>
        /// <param name="errorCode">Server transaction ID</param>
        public ShutterStateResponse(uint clientTransactionID, uint serverTransactionID, string errorMessage, AlpacaErrors errorCode)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            base.ErrorMessage = errorMessage;
            base.ErrorNumber = errorCode;
        }

        /// <summary>
        /// Shutter state returned by the device
        /// </summary>
        public ShutterState Value { get; set; }

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

