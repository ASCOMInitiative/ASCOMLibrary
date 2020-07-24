using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Standard.Responses
{
    /// <summary>
    /// Response for methods that do not return a value.
    /// </summary>
    public class CommandCompleteResponse : Response
    {
        /// <summary>
        /// Create a new CommandCompleteResponse with default values
        /// </summary>
        public CommandCompleteResponse()
        {
        }

        /// <summary>
        /// Create a new CommandCompleteResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        public CommandCompleteResponse(uint clientTransactionID, uint serverTransactionID)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
        }

        /// <summary>
        /// Create a new CommandCompleteResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="errorMessage">Value to return</param>
        /// <param name="errorCode">Server transaction ID</param>
        public CommandCompleteResponse(uint clientTransactionID, uint serverTransactionID, string errorMessage, ErrorCodes errorCode)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            base.ErrorMessage = errorMessage;
            base.ErrorNumber = errorCode;
        }

        /// <summary>
        /// Return a string containing the ClientTransactionID, ServerTransactionID, ErrorMessage and ErrorNumber
        /// </summary>
        /// <returns>String representation of the response value</returns>
        public override string ToString()
        {
            if (ErrorMessage == null) return $"ClientTransactionID: {ClientTransactionID}, ServerTransactionID: {ServerTransactionID}, ErrorMessage:, ErrorNumber: {ErrorNumber}";
            return $"ClientTransactionID: {ClientTransactionID}, ServerTransactionID: {ServerTransactionID}, ErrorMessage: {ErrorMessage}, ErrorNumber: {ErrorNumber}";
        }
    }
}