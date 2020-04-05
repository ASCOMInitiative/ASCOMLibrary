using ASCOM.Alpaca.Interfaces;

namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// Response that returns a <see cref="SensorType"/> value.
    /// </summary>
    public class SensorTypeResponse : Response, IValueResponse<SensorType>
    {
        /// <summary>
        /// Create a new SensorTypeResponse with default values
        /// </summary>
        public SensorTypeResponse()
        {
        }

        /// <summary>
        /// Create a new SensorTypeResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public SensorTypeResponse(uint clientTransactionID, uint serverTransactionID, SensorType value)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            Value = value;
        }

        /// <summary>
        /// Sensor type returned by the device
        /// </summary>
        /// <summary>
        /// Create a new SensorTypeResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="errorMessage">Value to return</param>
        /// <param name="errorCode">Server transaction ID</param>
        public SensorTypeResponse(uint clientTransactionID, uint serverTransactionID, string errorMessage, ErrorCodes errorCode)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            base.ErrorMessage = errorMessage;
            base.ErrorNumber = errorCode;
        }

        /// <summary>
        /// SensorType value returned by the device
        /// </summary>
        public SensorType Value { get; set; }

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