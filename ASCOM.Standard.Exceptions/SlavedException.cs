using System;

namespace ASCOM.Alpaca
{
    /// <summary>
    /// This exception should be used to indicate that movement (or other invalid operation) was attempted while the device was in slaved mode. This applies primarily to domes drivers.
    /// </summary>
    /// <remarks>
    /// </remarks>
    [Serializable]
    public class SlavedException : AlpacaException
    {
        /// <summary>
        /// Create a new exception using a default error message.
        /// </summary>
        public SlavedException() : base("Operation not valid while the device is in slave mode.", ErrorCodes.InvalidWhileSlaved)
        {
        }

        /// <summary>
        /// Create a new exception with the specified message
        /// </summary>
        /// <param name = "message">Exception description</param>
        public SlavedException(string message) : base(message, ErrorCodes.InvalidWhileSlaved)
        {
        }

        /// <summary>
        /// Create a new exception with the specified message and exception
        /// </summary>
        /// <param name = "message">Exception description</param>
        /// <param name = "inner">Underlying exception that caused this exception to be thrown.</param>
        public SlavedException(string message, Exception inner) : base(message, ErrorCodes.InvalidWhileSlaved, inner)
        {
        }
    }
}