using System.Collections.Generic;
using ASCOM.Standard.Interfaces;

namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// Response that returns a collection of <see cref="AxisRate"/> values.
    /// </summary>
    public class AxisRatesResponse : Response, IValueResponse<IList<AxisRate>>
    {
        /// <summary>
        /// Create a new AxisRatesResponse with default values
        /// </summary>
        public AxisRatesResponse()
        {
            Value = new List<AxisRate>(); // Make sure that Value contains at least an empty collection 
        }

        /// <summary>
        /// Create a new AxisRatesResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public AxisRatesResponse(uint clientTransactionID, uint serverTransactionID, IList<AxisRate> value)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            Value = value;
        }

        /// <summary>
        /// Create a new AxisRatesResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="errorMessage">Value to return</param>
        /// <param name="errorCode">Server transaction ID</param>
        public AxisRatesResponse(uint clientTransactionID, uint serverTransactionID, string errorMessage, ErrorCodes errorCode)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            base.ErrorMessage = errorMessage;
            base.ErrorNumber = errorCode;
        }

        /// <summary>
        /// Axis rate collection returned by the device
        /// </summary>
        public IList<AxisRate> Value { get; set; }

        /// <summary>
        /// Return the value as a string
        /// </summary>
        /// <returns>String representation of the response value</returns>
        public override string ToString()
        {
            if (Value == null) return "AxisRates value is null";
            return string.Join(", ", Value);
        }
    }
}