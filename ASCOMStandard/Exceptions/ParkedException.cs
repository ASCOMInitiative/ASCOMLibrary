using System;

namespace ASCOM.Alpaca
{
    /// <summary>
    /// This exception should be used to indicate that movement (or other invalid operation) was attempted while the device was in a parked state.
    /// </summary>
    /// <remarks>
    /// </remarks>
    [Serializable]
    public class ParkedException : AlpacaException
    {
        /// <summary>
        /// Create a new exception using a default error message.
        /// </summary>
        public ParkedException() : base("Operation not valid while the device is parked", ErrorCodes.InvalidWhileParked)
        {
        }

        /// <summary>
        ///  Create a new exception using the specified message
        /// </summary>
        /// <param name = "message">Exception description</param>
        public ParkedException(string message) : base(message, ErrorCodes.InvalidWhileParked)
        {
        }

        /// <summary>
        ///  Create a new exception using the specified message and exception
        /// </summary>
        /// <param name = "message">Exception description</param>
        /// <param name = "inner">Underlying exception that caused this exception to be thrown.</param>
        public ParkedException(string message, Exception inner) : base(message, ErrorCodes.InvalidWhileParked, inner)
        {
        }
    }
}