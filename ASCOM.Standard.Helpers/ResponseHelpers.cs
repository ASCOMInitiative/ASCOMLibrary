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

            //This will add the correct error codes if the driver is using the platform version of ASCOM.Exceptions
            ErrorCodes errorCode = (ex as AlpacaException)?.Number ?? TryGetErrorCodesForUnknownException(ex);

            return ExceptionResponseBuilder<T>(errorCode, message ?? string.Empty, clientTransactionID, serverTransactionID);
        }

        private static ErrorCodes TryGetErrorCodesForUnknownException(Exception ex)
        {
            //Try with the HResult first, then type names
            int HResult = ex.HResult;
            if(HResult == ASCOM.ErrorCodes.ActionNotImplementedException) { 
                    return ErrorCodes.NotImplemented;
            } 
            else if (HResult == ASCOM.ErrorCodes.InvalidOperationException)
            {
                return ErrorCodes.InvalidOperationException;
            }
            else if (HResult == ASCOM.ErrorCodes.InvalidValue)
            {
                return ErrorCodes.InvalidValue;
            }
            else if (HResult == ASCOM.ErrorCodes.InvalidWhileParked)
            {
                return ErrorCodes.InvalidWhileParked;
            }
            else if (HResult == ASCOM.ErrorCodes.InvalidWhileSlaved)
            {
                return ErrorCodes.InvalidWhileSlaved;
            }
            else if (HResult == ASCOM.ErrorCodes.NotConnected)
            {
                return ErrorCodes.NotConnected;
            }
            else if (HResult == ASCOM.ErrorCodes.NotImplemented)
            {
                return ErrorCodes.NotImplemented;
            }
            else if (HResult == ASCOM.ErrorCodes.NotInCacheException)
            {
                return ErrorCodes.UnspecifiedError;
            }
            else if (HResult == ASCOM.ErrorCodes.SettingsProviderError)
            {
                return ErrorCodes.UnspecifiedError;
            }
            else if (HResult == ASCOM.ErrorCodes.UnspecifiedError)
            {
                return ErrorCodes.UnspecifiedError;
            }
            else if (HResult == ASCOM.ErrorCodes.ValueNotSet)
            {
                return ErrorCodes.ValueNotSet;
            }

            var type = ex.GetType();

            switch (type.Name.ToString())
            {
                case "InvalidValueException":
                    return ErrorCodes.InvalidValue;
                case "ValueNotSetException":
                    return ErrorCodes.ValueNotSet;
                case "NotImplementedException":
                    return ErrorCodes.NotImplemented;
                case "ParkedException":
                    return ErrorCodes.InvalidWhileParked;
                case "MethodNotImplementedException":
                    return ErrorCodes.NotImplemented;
                case "PropertyNotImplementedException":
                    return ErrorCodes.NotImplemented;
                case "NotConnectedException":
                    return ErrorCodes.NotConnected;
                case "InvalidOperationException":
                    return ErrorCodes.InvalidOperationException;
                case "ActionNotImplementedException":
                    return ErrorCodes.NotImplemented;
                case "SlavedException":
                    return ErrorCodes.InvalidWhileSlaved;
            }

            return ErrorCodes.UnspecifiedError;
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
