using System.Collections.Generic;

namespace ASCOM.Standard.Responses
{
    /// <summary>
    /// Response that returns a collection of integer values.
    /// </summary>
    public class IntListResponse : Response, IValueResponse<IList<int>>
    {
        /// <summary>
        /// Create a new IntListResponse with default values
        /// </summary>
        public IntListResponse()
        {
            Value = new List<int>(); // Make sure that Value contains at least an empty collection 
        }

        /// <summary>
        /// Create a new IntListResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public IntListResponse(uint clientTransactionID, uint serverTransactionID, IList<int> value)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            Value = value;
        }

        /// <summary>
        /// Create a new IntListResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="errorMessage">Value to return</param>
        /// <param name="errorCode">Server transaction ID</param>
        public IntListResponse(uint clientTransactionID, uint serverTransactionID, string errorMessage, ErrorCodes errorCode)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            base.ErrorMessage = errorMessage;
            base.ErrorNumber = errorCode;
        }

        /// <summary>
        /// Integer collection returned by the device
        /// </summary>
        public IList<int> Value { get; set; }

        /// <summary>
        /// Return the value as a string
        /// </summary>
        /// <returns>String representation of the response value</returns>
        public override string ToString()
        {
            if (Value == null) return "IntList value is null";
            return string.Join(", ", Value);
        }
    }
}