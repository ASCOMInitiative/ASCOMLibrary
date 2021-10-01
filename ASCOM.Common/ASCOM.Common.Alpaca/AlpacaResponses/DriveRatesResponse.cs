using ASCOM.Common.DeviceInterfaces;
using System.Collections.Generic;

namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// Response that returns a collection of <see cref="DriveRate"/> values
    /// </summary>
    public class DriveRatesResponse : Response, IValueResponse<IList<DriveRate>>
    {
        /// <summary>
        /// Create a new DriveRateResponse with default values
        /// </summary>
        public DriveRatesResponse()
        {
            Value = new List<DriveRate>(); // Make sure that Value contains at least an empty collection 
        }

        /// <summary>
        /// Create a new DriveRatesResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public DriveRatesResponse(uint clientTransactionID, uint serverTransactionID, IList<DriveRate> value)
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
        public DriveRatesResponse(uint clientTransactionID, uint serverTransactionID, string errorMessage, AlpacaErrors errorCode)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            base.ErrorMessage = errorMessage;
            base.ErrorNumber = errorCode;
        }

        /// <summary>
        /// Drive rate collection returned by the device
        /// </summary>
        public IList<DriveRate> Value { get; set; }

        /// <summary>
        /// Return the value as a string
        /// </summary>
        /// <returns>String representation of the response value</returns>
        public override string ToString()
        {
            if (Value == null) return "DriveRates value is null";
            return string.Join(", ", Value);
        }
    }
}