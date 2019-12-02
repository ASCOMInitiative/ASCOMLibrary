using ASCOM.Alpaca;
using ASCOM.Alpaca.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Standard.Helpers
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
            Alpaca.ErrorCodes errorCode = (ex as AlpacaException)?.Number ?? GetErrorCodesForUnknownException(ex);

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
        public static T ExceptionResponseBuilder<T>(Alpaca.ErrorCodes code, string message, uint clientTransactionID = 0, uint serverTransactionID = 0) where T : Response, new()
        {
            return new T()
            {
                ClientTransactionID = clientTransactionID,
                ServerTransactionID = serverTransactionID,
                ErrorNumber = code,
                ErrorMessage = message
            };
        }

        private static Alpaca.ErrorCodes GetErrorCodesForUnknownException(Exception ex)
        {
            //Try with the HResult first, then type names
            int HResult = ex.HResult;
            if (HResult == ASCOM.ErrorCodes.ActionNotImplementedException)
            {
                return Alpaca.ErrorCodes.NotImplemented;
            }
            else if (HResult == ASCOM.ErrorCodes.InvalidOperationException)
            {
                return Alpaca.ErrorCodes.InvalidOperationException;
            }
            else if (HResult == ASCOM.ErrorCodes.InvalidValue)
            {
                return Alpaca.ErrorCodes.InvalidValue;
            }
            else if (HResult == ASCOM.ErrorCodes.InvalidWhileParked)
            {
                return Alpaca.ErrorCodes.InvalidWhileParked;
            }
            else if (HResult == ASCOM.ErrorCodes.InvalidWhileSlaved)
            {
                return Alpaca.ErrorCodes.InvalidWhileSlaved;
            }
            else if (HResult == ASCOM.ErrorCodes.NotConnected)
            {
                return Alpaca.ErrorCodes.NotConnected;
            }
            else if (HResult == ASCOM.ErrorCodes.NotImplemented)
            {
                return Alpaca.ErrorCodes.NotImplemented;
            }
            else if (HResult == ASCOM.ErrorCodes.NotInCacheException)
            {
                return Alpaca.ErrorCodes.UnspecifiedError;
            }
            else if (HResult == ASCOM.ErrorCodes.SettingsProviderError)
            {
                return Alpaca.ErrorCodes.UnspecifiedError;
            }
            else if (HResult == ASCOM.ErrorCodes.UnspecifiedError)
            {
                return Alpaca.ErrorCodes.UnspecifiedError;
            }
            else if (HResult == ASCOM.ErrorCodes.ValueNotSet)
            {
                return Alpaca.ErrorCodes.ValueNotSet;
            }

            var type = ex.GetType();

            switch (type.Name.ToString())
            {
                case "InvalidValueException":
                    return Alpaca.ErrorCodes.InvalidValue;
                case "ValueNotSetException":
                    return Alpaca.ErrorCodes.ValueNotSet;
                case "NotImplementedException":
                    return Alpaca.ErrorCodes.NotImplemented;
                case "ParkedException":
                    return Alpaca.ErrorCodes.InvalidWhileParked;
                case "MethodNotImplementedException":
                    return Alpaca.ErrorCodes.NotImplemented;
                case "PropertyNotImplementedException":
                    return Alpaca.ErrorCodes.NotImplemented;
                case "NotConnectedException":
                    return Alpaca.ErrorCodes.NotConnected;
                case "InvalidOperationException":
                    return Alpaca.ErrorCodes.InvalidOperationException;
                case "ActionNotImplementedException":
                    return Alpaca.ErrorCodes.NotImplemented;
                case "SlavedException":
                    return Alpaca.ErrorCodes.InvalidWhileSlaved;
            }

            return Alpaca.ErrorCodes.UnspecifiedError;
        }
    }
}
