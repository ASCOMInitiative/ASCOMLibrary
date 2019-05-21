using System;

namespace ASCOM.Alpaca
{
    /// <summary>
    /// Exception to report that no value has yet been set for this property.
    /// </summary>
    /// <remarks>
    /// </remarks>
    [Serializable]
    public class ValueNotSetException : AlpacaException
    {
        /// <summary>
        /// Create a new exception using a default error message.
        /// </summary>
        public ValueNotSetException() : base("No value has been set", ErrorCodes.ValueNotSet)
        {
        }

        /// <summary>
        ///  Create a new exception using the specified message
        /// </summary>
        /// <param name = "message">Exception description</param>
        public ValueNotSetException(string message) : base(message, ErrorCodes.ValueNotSet)
        {
        }

        /// <summary>
        ///  Create a new exception using the specified message and exception
        /// </summary>
        /// <param name = "message">Exception description</param>
        /// <param name = "inner">Underlying exception that caused this exception to be thrown.</param>
        public ValueNotSetException(string message, Exception inner) : base(message, ErrorCodes.ValueNotSet, inner)
        {
        }
    }
}