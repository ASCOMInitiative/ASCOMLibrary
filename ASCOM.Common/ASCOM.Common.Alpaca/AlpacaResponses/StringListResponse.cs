using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// Response that returns a collection of strings
    /// </summary>
    public class StringListResponse : Response, IValueResponse<IList<string>>
    {
        /// <summary>
        /// Create a new StringListResponse with default values
        /// </summary>
        public StringListResponse()
        {
            Value = new List<string>(); // Make sure that Value contains at least an empty collection 
        }

        /// <summary>
        /// Create a new StringListResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public StringListResponse(uint clientTransactionID, uint serverTransactionID, IList<string> value)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            Value = value;
        }

        /// <summary>
        /// Create a new StringListResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="errorMessage">Value to return</param>
        /// <param name="errorCode">Server transaction ID</param>
        public StringListResponse(uint clientTransactionID, uint serverTransactionID, string errorMessage, AlpacaErrors errorCode)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            base.ErrorMessage = errorMessage;
            base.ErrorNumber = errorCode;
        }

        /// <summary>
        /// String collection returned by the device
        /// </summary>
        [JsonPropertyOrder(1000)]
        public IList<string> Value { get; set; }

        /// <summary>
        /// Return the value as a string
        /// </summary>
        /// <returns>String representation of the response value</returns>
        public override string ToString()
        {
            if (Value == null) return string.Empty;
            return string.Join(", ", Value);
        }
    }
}