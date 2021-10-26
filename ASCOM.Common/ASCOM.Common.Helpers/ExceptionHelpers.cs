using ASCOM.Common.Alpaca;
using System;

namespace ASCOM.Common.Helpers
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
        public static DriverException ExceptionFromErrorCode(AlpacaErrors code, string message = "")
        {
            switch (code)
            {
                case AlpacaErrors.InvalidValue:
                    return new InvalidValueException(message);
                case AlpacaErrors.ValueNotSet:
                    return new ValueNotSetException(message);
                case AlpacaErrors.NotConnected:
                    return new NotConnectedException(message);
                case AlpacaErrors.InvalidWhileParked:
                    return new ParkedException(message);
                case AlpacaErrors.InvalidWhileSlaved:
                    return new SlavedException(message);
                case AlpacaErrors.InvalidOperationException:
                    return new InvalidOperationException(message);
                case AlpacaErrors.UnspecifiedError:
                    return new DriverException(message);
                case AlpacaErrors.NotImplemented:
                    return new NotImplementedException(message);
                case AlpacaErrors.ActionNotImplementedException:
                    return new ActionNotImplementedException(message);
                case AlpacaErrors.AlpacaNoError:
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
        public static AlpacaErrors ErrorCode(this Exception ex)
        {
            return ErrorCodeFromException(ex);
        }

        /// <summary>
        /// A method to get the Alpaca Error Code for an Exception. Returns UnspecifiedError if it cannot find a better code
        /// </summary>
        /// <param name="ex">The exception to get the error code for</param>
        /// <returns>The best matching ErrorCode for the exception, UnspecifiedError if a better one cannot be found</returns>
        public static AlpacaErrors ErrorCodeFromException(Exception ex)
        {
            //Try with the HResult first, then type names
            int HResult = ex.HResult;
            if (HResult == ErrorCodes.ActionNotImplementedException)
            {
                return AlpacaErrors.ActionNotImplementedException;
            }
            else if (HResult == ErrorCodes.InvalidOperationException)
            {
                return AlpacaErrors.InvalidOperationException;
            }
            else if (HResult == ErrorCodes.InvalidValue)
            {
                return AlpacaErrors.InvalidValue;
            }
            else if (HResult == ErrorCodes.InvalidWhileParked)
            {
                return AlpacaErrors.InvalidWhileParked;
            }
            else if (HResult == ErrorCodes.InvalidWhileSlaved)
            {
                return AlpacaErrors.InvalidWhileSlaved;
            }
            else if (HResult == ErrorCodes.NotConnected)
            {
                return AlpacaErrors.NotConnected;
            }
            else if (HResult == ErrorCodes.NotImplemented)
            {
                return AlpacaErrors.NotImplemented;
            }
            else if (HResult == ErrorCodes.NotInCacheException)
            {
                return AlpacaErrors.UnspecifiedError;
            }
            else if (HResult == ErrorCodes.SettingsProviderError)
            {
                return AlpacaErrors.UnspecifiedError;
            }
            else if (HResult == ErrorCodes.UnspecifiedError)
            {
                return AlpacaErrors.UnspecifiedError;
            }
            else if (HResult == ErrorCodes.ValueNotSet)
            {
                return AlpacaErrors.ValueNotSet;
            }

            var type = ex.GetType();

            switch (type.Name.ToString())
            {
                case "InvalidValueException":
                    return AlpacaErrors.InvalidValue;
                case "ValueNotSetException":
                    return AlpacaErrors.ValueNotSet;
                case "NotImplementedException":
                    return AlpacaErrors.NotImplemented;
                case "ParkedException":
                    return AlpacaErrors.InvalidWhileParked;
                case "MethodNotImplementedException":
                    return AlpacaErrors.NotImplemented;
                case "PropertyNotImplementedException":
                    return AlpacaErrors.NotImplemented;
                case "NotConnectedException":
                    return AlpacaErrors.NotConnected;
                case "InvalidOperationException":
                    return AlpacaErrors.InvalidOperationException;
                case "ActionNotImplementedException":
                    return AlpacaErrors.NotImplemented;
                case "SlavedException":
                    return AlpacaErrors.InvalidWhileSlaved;
            }

            return AlpacaErrors.UnspecifiedError;
        }
    }
}
