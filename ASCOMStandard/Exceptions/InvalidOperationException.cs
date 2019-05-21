using System;

namespace ASCOM.Alpaca
{
    /// <summary>
    /// This exception should be raised by a driver to reject a command from the client.
    /// </summary>
    /// <remarks>
    /// <para>The exception is intended to be used for "logical" errors e.g. trying to use a command when the current configuration of the device does
    /// not allow it rather than for device errors such as a communications error.</para>
    /// <para>Its the error to use when the client attempts something, which at another time would be sensible,
    /// but which is not sensible right now. If you expect the condition causing the issue to be short
    /// lived, you may choose to stall the request until the condition is cleared rather than throwing this exception.
    /// Clearly, that is a judgement that you can only make given a specific scenario.</para>
    ///</remarks>
    [Serializable]
    public class InvalidOperationException : AlpacaException
    {
        /// <summary>
        /// Create a new exception using a default error message.
        /// </summary>
        public InvalidOperationException() : base("The requested operation is not permitted at this time.", ErrorCodes.InvalidOperationException)
        {
        }

        /// <summary>
        ///  Create a new exception using the specified message
        /// </summary>
        /// <param name = "message">Exception description</param>
        public InvalidOperationException(string message) : base(message, ErrorCodes.InvalidOperationException)
        {
        }

        /// <summary>
        ///  Create a new exception using the specified message and exception
        /// </summary>
        /// <param name = "message">Exception description</param>
        /// <param name = "inner">Underlying exception that caused this exception to be thrown.</param>
        public InvalidOperationException(string message, Exception inner) : base(message, ErrorCodes.InvalidOperationException, inner)
        {
        }
    }
}