using ASCOM.Common.DeviceInterfaces;

namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// Response that returns a <see cref="DriveRate"/> value.
    /// </summary>
    public class DriveRateResponse : Response, IValueResponse<DriveRate>
    {
        /// <summary>
        /// Create a new DriveRateResponse with default values
        /// </summary>
        public DriveRateResponse()
        {
        }

        /// <summary>
        /// Create a new DriveRateResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public DriveRateResponse(uint clientTransactionID, uint serverTransactionID, DriveRate value)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            Value = value;
        }

        /// <summary>
        /// Create a new DriveRateResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="errorMessage">Value to return</param>
        /// <param name="errorCode">Server transaction ID</param>
        public DriveRateResponse(uint clientTransactionID, uint serverTransactionID, string errorMessage, AlpacaErrors errorCode)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            base.ErrorMessage = errorMessage;
            base.ErrorNumber = errorCode;
        }

        /// <summary>
        /// Drive rate returned by the device
        /// </summary>
        public DriveRate Value { get; set; }

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