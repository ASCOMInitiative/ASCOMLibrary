using ASCOM.Common.Alpaca;
using System.Collections.Generic;

namespace ASCOM.Alpaca.Discovery
{
    public class AlpacaConfiguredDevicesResponse : Response, IValueResponse<List<AlpacaConfiguredDevice>>
    {
        /// <summary>
        /// Create a new AlpacaConfiguredDevicesResponse with default values
        /// </summary>
        public AlpacaConfiguredDevicesResponse()
        {
        }

        /// <summary>
        /// Create a new AlpacaConfiguredDevicesResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public AlpacaConfiguredDevicesResponse(uint clientTransactionID, uint serverTransactionID, List<AlpacaConfiguredDevice> value)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            Value = value;
        }

        /// <summary>
        /// Create a new AlpacaConfiguredDevicesResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="errorMessage">Value to return</param>
        /// <param name="errorCode">Server transaction ID</param>
        public AlpacaConfiguredDevicesResponse(uint clientTransactionID, uint serverTransactionID, string errorMessage, AlpacaErrors errorCode)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            base.ErrorMessage = errorMessage;
            base.ErrorNumber = errorCode;
        }

        /// <summary>
        /// AlpacaConfiguredDevicesResponse value returned by the device
        /// </summary>
        public List<AlpacaConfiguredDevice> Value { get; set; } = new List<AlpacaConfiguredDevice>();

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
