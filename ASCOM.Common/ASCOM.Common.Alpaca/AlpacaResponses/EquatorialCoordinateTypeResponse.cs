using ASCOM.Common.DeviceInterfaces;

namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// Response that returns a <see cref="EquatorialCoordinateType"/> value.
    /// </summary>
    public class EquatorialCoordinateTypeResponse : Response, IValueResponse<EquatorialCoordinateType>
    {
        /// <summary>
        /// Create a new EquatorialCoordinateTypeResponse with default values
        /// </summary>
        public EquatorialCoordinateTypeResponse()
        {
        }

        /// <summary>
        /// Create a new EquatorialCoordinateTypeResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public EquatorialCoordinateTypeResponse(uint clientTransactionID, uint serverTransactionID, EquatorialCoordinateType value)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            Value = value;
        }

        /// <summary>
        /// Create a new EquatorialCoordinateTypeResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="errorMessage">Value to return</param>
        /// <param name="errorCode">Server transaction ID</param>
        public EquatorialCoordinateTypeResponse(uint clientTransactionID, uint serverTransactionID, string errorMessage, AlpacaErrors errorCode)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            base.ErrorMessage = errorMessage;
            base.ErrorNumber = errorCode;
        }

        /// <summary>
        /// Equatorial coordinate type returned by the device
        /// </summary>
        public EquatorialCoordinateType Value { get; set; }

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