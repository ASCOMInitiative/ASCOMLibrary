using ASCOM.Common.Com;
using System;

namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// Defines the properties that are common to all Alpaca responses.
    /// </summary>
    /// <remarks>
    /// If a command does not return a value, use <see cref="CommandCompleteResponse"/> instead of this class.
    /// </remarks>
    public class Response : IResponse
    {
        private Exception exception;

        /// <summary>
        /// Client's transaction ID (0 to 4294967295), as supplied by the client in the command request.
        /// </summary>
        public uint ClientTransactionID { get; set; }

        /// <summary>
        /// Server's transaction ID (0 to 4294967295), should be unique for each client transaction so that log messages on the client can be associated with logs on the device.
        /// </summary>
        public uint ServerTransactionID { get; set; }

        /// <summary>
        /// Zero for a successful transaction, or a non-zero integer(-2147483648 to 2147483647) if the device encountered an issue.Devices must use ASCOM reserved error
        /// numbers whenever appropriate so that clients can take informed actions. E.g.returning 0x401 (1025) to indicate that an invalid value was received.
        /// </summary>
        /// <seealso cref="AlpacaErrors"/>
        public AlpacaErrors ErrorNumber { get; set; }

        /// <summary>
        /// Empty string for a successful transaction, or a message describing the issue that was encountered. If an error message is returned,
        /// a non zero <see cref="ErrorNumber"/> must also be returned.
        /// </summary>
        public string ErrorMessage { get; set; } = "";

        /// <summary>
        /// Optional field for Windows drivers to return an exception to the client application.
        /// </summary>
        /// <remarks>Populating this automatically sets the ErrorMessage and ErrorNumber fields; COM errors in the range 0x80040400 to 0x8040FFF are translated to equivalent Alpaca error numbers in the range 0x400 to 0xFFF.</remarks>
        public Exception DriverException
        {
            get
            {
                return exception;
            }
            set
            {
                exception = value;
                if (exception != null)
                {
                    // Set the error number and message fields from the exception
                    ErrorMessage = exception.Message;

                    // Convert ASCOM exception error numbers (0x80040400 - 0x80040FFF) to equivalent Alpaca error numbers (0x400 to 0xFFF) so that they will be interpreted correctly by native Alpaca clients
                    if ((exception.HResult >= (int)ComErrorCodes.ComErrorNumberBase) && (exception.HResult <= (int)ComErrorCodes.ComErrorNumberMax))
                        ErrorNumber = (AlpacaErrors)(exception.HResult - (int)ComErrorCodes.ComErrorNumberOffset);
                    else // Return unspecified error
                        ErrorNumber = AlpacaErrors.UnspecifiedError;

                }
            }
        }


    }
}