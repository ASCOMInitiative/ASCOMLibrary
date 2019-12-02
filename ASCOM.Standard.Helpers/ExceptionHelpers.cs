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
                default:
                    return null;
            }
        }
    }
}
