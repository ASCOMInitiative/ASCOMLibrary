using ASCOM.Common.DeviceInterfaces;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// Response that returns a collection of IStateValue objects
    /// </summary>
    public class DeviceStateResponse : Response, IValueResponse<List<StateValue>>
    {
        /// <summary>
        /// Create a new DeviceStateResponse with default values
        /// </summary>
        public DeviceStateResponse()
        {
            Value = new List<StateValue>(); // Make sure that Value contains at least an empty collection 
        }

        /// <summary>
        /// Create a new DeviceStateResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public DeviceStateResponse(uint clientTransactionID, uint serverTransactionID, List<StateValue> value)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            Value = value;
        }

        /// <summary>
        /// Create a new DeviceStateResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="errorMessage">Value to return</param>
        /// <param name="errorCode">Server transaction ID</param>
        public DeviceStateResponse(uint clientTransactionID, uint serverTransactionID, string errorMessage, AlpacaErrors errorCode)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            base.ErrorMessage = errorMessage;
            base.ErrorNumber = errorCode;
        }

        /// <summary>
        /// IStateValue collection returned by the device
        /// </summary>
        [JsonPropertyOrder(1000)]
        public List<StateValue> Value { get; set; }

        /// <summary>
        /// Return the value as a string
        /// </summary>
        /// <returns>String representation of the response value</returns>
        public override string ToString()
        {
            if (Value == null) return string.Empty;

            string response = "";
            foreach (StateValue item in Value)
            {
                response += $"{item.Name} = {item.Value}, ";
            }

            return response.Trim(',', ' ');
        }
    }
}
