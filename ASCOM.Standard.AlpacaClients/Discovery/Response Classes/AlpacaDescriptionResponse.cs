using ASCOM.Alpaca.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Alpaca.Discovery
{
    /// <summary>
    /// The response for the AlpacaDeviceDescription
    /// </summary>
    public class AlpacaDescriptionResponse : Response, IValueResponse<AlpacaDeviceDescription>
    {
        /// <summary>
        /// Create a new AlpacaDescriptionResponse with default values
        /// </summary>
        public AlpacaDescriptionResponse()
        {
        }

        /// <summary>
        /// Create a new AlpacaDescriptionResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public AlpacaDescriptionResponse(uint clientTransactionID, uint serverTransactionID, AlpacaDeviceDescription value)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            Value = value;
        }

        /// <summary>
        /// Create a new AlpacaDescriptionResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="errorMessage">Value to return</param>
        /// <param name="errorCode">Server transaction ID</param>
        public AlpacaDescriptionResponse(uint clientTransactionID, uint serverTransactionID, string errorMessage, Alpaca.ErrorCodes errorCode)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            base.ErrorMessage = errorMessage;
            base.ErrorNumber = errorCode;
        }

        /// <summary>
        /// AlpacaDeviceDescription value returned by the device
        /// </summary>
        public AlpacaDeviceDescription Value { get; set; }

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
