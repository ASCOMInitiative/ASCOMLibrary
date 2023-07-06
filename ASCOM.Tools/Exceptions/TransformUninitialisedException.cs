using ASCOM.Tools;
using System;

namespace ASCOM
{
    /// <summary>
    /// Exception thrown when an attempt is made to read from the transform component before it has had co-ordinates
    /// set once by SetJ2000 or SetJNow.
    /// </summary>
    /// <remarks></remarks>
    public class TransformUninitialisedException : HelperException
    {

        /// <summary>
        /// Create a new exception with message 
        /// </summary>
        /// <param name="message">Message to be reported by the exception</param>
        /// <remarks></remarks>
        public TransformUninitialisedException(string message) : base(message)
        {
        }

        /// <summary>
        /// Create a new exception with message 
        /// </summary>
        /// <param name="message">Message to be reported by the exception</param>
        /// <param name="inner">Exception to be reported as the inner exception</param>
        /// <remarks></remarks>
        public TransformUninitialisedException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Serialise the exception
        /// </summary>
        /// <param name="info">Serialisation information</param>
        /// <param name="context">Serialisation context</param>
        /// <remarks></remarks>
        public TransformUninitialisedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)

        {
        }
    }

}