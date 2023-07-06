using System;
using System.Runtime.InteropServices;

namespace ASCOM
{

    /// <summary>
    /// General exception for use within Tools components.
    /// </summary>
    public class HelperException : Exception
    {
        //Exception for Utilities component exceptions

        /// <summary>
        /// Create a new exception with message
        /// </summary>
        /// <param name="message">Message to be reported by the exception</param>
        /// <remarks></remarks>
        public HelperException(string message) : base(message) { }

        /// <summary>
        /// Create a new exception with message and inner exception
        /// </summary>
        /// <param name="message">Message to be reported by the exception</param>
        /// <param name="inner">Exception to be reported as the inner exception</param>
        /// <remarks></remarks>
        public HelperException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Serialise the exception
        /// </summary>
        /// <param name="info">Serialisation information</param>
        /// <param name="context">Serialisation context</param>
        /// <remarks></remarks>
        public HelperException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }

    }

}