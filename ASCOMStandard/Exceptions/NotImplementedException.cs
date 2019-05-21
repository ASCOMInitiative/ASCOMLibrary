using System;

namespace ASCOM.Alpaca
{
    /// <summary>
    /// All properties and methods defined by the relevant ASCOM standard interface must exist in each driver. However, those properties and methods do not all have to be <i>implemented</i>.
    /// </summary>
    /// <remarks>
    /// </remarks>
    [Serializable]
    public class NotImplementedException : AlpacaException
    {
        /// <summary>
        /// Create a new exception using a default error message.
        /// </summary>
        public NotImplementedException() : base("This capability is not implemented.", ErrorCodes.NotImplemented)
        {
        }

        /// <summary>
        ///  Create a new exception using the specified message
        /// </summary>
        /// <param name = "message">Exception description</param>
        public NotImplementedException(string message) : base(message, ErrorCodes.NotImplemented)
        {
        }

        /// <summary>
        ///  Create a new exception using the specified message and exception
        /// </summary>
        /// <param name = "message">Exception description</param>
        /// <param name = "inner">Underlying exception that caused this exception to be thrown.</param>
        public NotImplementedException(string message, Exception inner) : base(message, ErrorCodes.NotImplemented, inner)
        {
        }
    }
}