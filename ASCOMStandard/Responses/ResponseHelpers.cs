using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// This class contains helper functions to make responding from the server easier.
    /// </summary>
    public class ResponseHelpers
    {
        /// <summary>
        /// This generates a Response of a given Type with the Error Code and Message filled in from the passed Exception
        /// </summary>
        /// <typeparam name="T">The Type of Response to create and fill</typeparam>
        /// <param name="ex">The Exception to use</param>
        /// <param name="clientTransactionID">The Client Transaction ID</param>
        /// <param name="serverTransactionID">The Server Transaction ID</param>
        /// <returns></returns>
        public static T ExceptionResponseBuilder<T>(Exception ex, uint clientTransactionID = 0, uint serverTransactionID = 0) where T : Response, new()
        {
            if (ex == null)
            {
                throw new ArgumentException("The exception must not be null");
            }

            return ExceptionResponseBuilder<T>(ex, ex?.Message ?? string.Empty, clientTransactionID, serverTransactionID);
        }

        /// <summary>
        /// This generates a Response of a given Type with the Error Code filled in from the passed Exception and a custom message
        /// </summary>
        /// <typeparam name="T">The Type of Response to create and fill</typeparam>
        /// <param name="ex">The Exception to use</param>
        /// <param name="message">The message to use</param>
        /// <param name="clientTransactionID">The Client Transaction ID</param>
        /// <param name="serverTransactionID">The Server Transaction ID</param>
        /// <returns></returns>
        public static T ExceptionResponseBuilder<T>(Exception ex, string message, uint clientTransactionID = 0, uint serverTransactionID = 0) where T : Response, new()
        {
            if (ex == null)
            {
                throw new ArgumentException("The exception must not be null");
            }

            ErrorCodes errorCode = (ex as AlpacaException)?.Number ?? ErrorCodes.UnspecifiedError;

            return ExceptionResponseBuilder<T>(errorCode, message ?? string.Empty, clientTransactionID, serverTransactionID);
        }

        /// <summary>
        /// This generates a Response of a given Type with a custom message and Error Code
        /// </summary>
        /// <typeparam name="T">The Type of Response to create and fill</typeparam>
        /// <param name="code">The Alpaca Error Code</param>
        /// <param name="message">The message to use</param>
        /// <param name="clientTransactionID">The Client Transaction ID</param>
        /// <param name="serverTransactionID">The Server Transaction ID</param>
        /// <returns></returns>
        public static T ExceptionResponseBuilder<T>(ErrorCodes code, string message, uint clientTransactionID = 0, uint serverTransactionID = 0) where T : Response, new()
        {
            return new T()
            {
                ClientTransactionID = clientTransactionID,
                ServerTransactionID = serverTransactionID,
                ErrorNumber = code,
                ErrorMessage = message
            };
        }
    }
}
