using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM
{
    /// <summary>
    /// Exception thrown what an invalid operation is attempted on the Transform component
    /// </summary>
    public class TransformInvalidOperationException : HelperException
    {
        /// <summary>
        /// Create a new exception with message 
        /// </summary>
        /// <param name="message">Message to be reported by the exception</param>
        /// <remarks></remarks>
        public TransformInvalidOperationException(string message) : base(message)
        {
        }
        /// <summary>
        /// Create a new exception with message 
        /// </summary>
        /// <param name="message">Message to be reported by the exception</param>
        /// <param name="inner">Exception to be reported as the inner exception</param>
        /// <remarks></remarks>
        public TransformInvalidOperationException(string message, Exception inner) : base(message, inner)
        {
        }
        /// <summary>
        /// Serialise the exception
        /// </summary>
        /// <param name="info">Serialisation information</param>
        /// <param name="context">Serialisation context</param>
        /// <remarks></remarks>
        public TransformInvalidOperationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}