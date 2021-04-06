using ASCOM.Alpaca.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Standard.Helpers
{
    /// <summary>
    /// This contains helper methods and extensions to make handling Exceptions from Alpaca devices easier
    /// </summary>
    public static class ExceptionHelpers
    {
        /// <summary>
        /// This extension gets an ASCOM driver exception from a response
        /// </summary>
        /// <param name="response">The Alpaca response</param>
        /// <returns>Null if there is no exception, otherwise the ASCOM Exception for the error code</returns>
        public static DriverException Exception(this IResponse response)
        {
            return ExceptionFromResponse(response);
        }

        /// <summary>
        /// This method gets a given driver exception from a response
        /// </summary>
        /// <param name="response">The Alpaca response</param>
        /// <returns>Null if there is no exception, otherwise the ASCOM Exception for the error code</returns>
        public static DriverException ExceptionFromResponse(IResponse response)
        {
            return ExceptionFromErrorCode(response.ErrorNumber, response.ErrorMessage); 
        }

        /// <summary>
        /// Gets a driver exception from an error code and adds the message if available
        /// </summary>
        /// <param name="code">The Alpaca ErrorCode for the Exception</param>
        /// <param name="message">The optional message of the exception</param>
        /// <returns>Null if there is no exception, otherwise the ASCOM Exception for the error code</returns>
        public static DriverException ExceptionFromErrorCode(Alpaca.ErrorCodes code, string message = "")
        {
            switch (code)
            {
                case Alpaca.ErrorCodes.InvalidValue:
                    return new InvalidValueException(message);
                case Alpaca.ErrorCodes.ValueNotSet:
                    return new ValueNotSetException(message);
                case Alpaca.ErrorCodes.NotConnected:
                    return new NotConnectedException(message);
                case Alpaca.ErrorCodes.InvalidWhileParked:
                    return new ParkedException(message);
                case Alpaca.ErrorCodes.InvalidWhileSlaved:
                    return new SlavedException(message);
                case Alpaca.ErrorCodes.InvalidOperationException:
                    return new InvalidOperationException(message);
                case Alpaca.ErrorCodes.UnspecifiedError:
                    return new DriverException(message);
                case Alpaca.ErrorCodes.NotImplemented:
                    return new NotImplementedException(message);
                case Alpaca.ErrorCodes.ActionNotImplementedException:
                    return new ActionNotImplementedException(message);
                case Alpaca.ErrorCodes.AlpacaNoError:
                    //No Error
                default:
                    return null;
            }
        }

        /// <summary>
        /// Extension method to get the Alpaca Error Code for an Exception. Returns UnspecifiedError if it cannot find a better code
        /// </summary>
        /// <param name="ex">The exception to get the error code for</param>
        /// <returns>The best matching ErrorCode for the exception, UnspecifiedError if a better one cannot be found</returns>
        public static Alpaca.ErrorCodes ErrorCode(this Exception ex)
        {
            return ErrorCodeFromException(ex);
        }

        /// <summary>
        /// A method to get the Alpaca Error Code for an Exception. Returns UnspecifiedError if it cannot find a better code
        /// </summary>
        /// <param name="ex">The exception to get the error code for</param>
        /// <returns>The best matching ErrorCode for the exception, UnspecifiedError if a better one cannot be found</returns>
        public static Alpaca.ErrorCodes ErrorCodeFromException(Exception ex)
        {
            //Try with the HResult first, then type names
            int HResult = ex.HResult;
            if (HResult == ASCOM.ErrorCodes.ActionNotImplementedException)
            {
                return Alpaca.ErrorCodes.ActionNotImplementedException;
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
