using System;

namespace ASCOM.Alpaca
{
    /// <summary>
    /// This exception should be raised when an operation is attempted that requires communication with the device, but the device is disconnected.
    /// </summary>
    /// <remarks>
    /// </remarks>
    [Serializable]
    public class NotConnectedException : AlpacaException
    {
        /// <summary>
        /// Create a new exception using a default error message.
        /// </summary>
        public NotConnectedException() : base("Device is not connected.", ErrorCodes.NotConnected)
        {
        }

        /// <summary>
        /// Create a new exception with the specified message
        /// </summary>
        /// <param name = "message">Exception description</param>
        public NotConnectedException(string message) : base(message, ErrorCodes.NotConnected)
        {
        }

        /// <summary>
        /// Create a new exception with the specified message and exception
        /// </summary>
        /// <param name = "message">Exception description</param>
        /// <param name = "inner">Underlying exception that caused this exception to be thrown.</param>
        public NotConnectedException(string message, Exception inner) : base(message, ErrorCodes.NotConnected, inner)
        {
        }
    }
}