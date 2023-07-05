using System;
using System.Runtime.InteropServices;

namespace ASCOM.Tools
{
    // ASCOM.Astrometry exceptions

    /// <summary>
    /// 
    /// </summary>
    public class HelperException : Exception
    {
        //Exception for Utilitiesities component exceptions

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

    /// <summary>
    /// Exception thrown when an attempt is made to read from the transform component before it has had co-ordinates
    /// set once by SetJ2000 or SetJNow.
    /// </summary>
    /// <remarks></remarks>
    // Exception for Helper.NET component exceptions
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

    /// <summary>
    /// Exception thrown when an incompatible component is encountered that prevents an Astrometric compoent 
    /// from functioning correctly.
    /// correctly.
    /// </summary>
    /// <remarks></remarks>
    public class CompatibilityException : HelperException
    {

        /// <summary>
        /// Create a new exception with message 
        /// </summary>
        /// <param name="message">Message to be reported by the exception</param>
        /// <remarks></remarks>
        public CompatibilityException(string message) : base(message)
        {
        }

        /// <summary>
        /// Create a new exception with message 
        /// </summary>
        /// <param name="message">Message to be reported by the exception</param>
        /// <param name="inner">Exception to be reported as the inner exception</param>
        /// <remarks></remarks>
        public CompatibilityException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Serialise the exception
        /// </summary>
        /// <param name="info">Serialisation information</param>
        /// <param name="context">Serialisation context</param>
        /// <remarks></remarks>
        public CompatibilityException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)

        {
        }
    }

    /// <summary>
    /// Exception thrown when an attempt is made to read a value that has not yet been set.
    /// </summary>
    /// <remarks></remarks>
    public class ValueNotSetException : HelperException
    {

        /// <summary>
        /// Create a new exception with message 
        /// </summary>
        /// <param name="message">Message to be reported by the exception</param>
        /// <remarks></remarks>
        public ValueNotSetException(string message) : base(message)
        {
        }

        /// <summary>
        /// Create a new exception with message 
        /// </summary>
        /// <param name="message">Message to be reported by the exception</param>
        /// <param name="inner">Exception to be reported as the inner exception</param>
        /// <remarks></remarks>
        public ValueNotSetException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Serialise the exception
        /// </summary>
        /// <param name="info">Serialisation information</param>
        /// <param name="context">Serialisation context</param>
        /// <remarks></remarks>
        public ValueNotSetException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)

        {
        }
    }

    /// <summary>
    /// Exception thrown when an attempt is made to read a value that has not yet been calculated.
    /// </summary>
    /// <remarks>This probably occurs because another variable has not been set or a required method has not been called.</remarks>
    // Exception for Helper.NET component exceptions
    public class ValueNotAvailableException : HelperException
    {

        /// <summary>
        /// Create a new exception with message 
        /// </summary>
        /// <param name="message">Message to be reported by the exception</param>
        /// <remarks></remarks>
        public ValueNotAvailableException(string message) : base(message)
        {
        }

        /// <summary>
        /// Create a new exception with message 
        /// </summary>
        /// <param name="message">Message to be reported by the exception</param>
        /// <param name="inner">Exception to be reported as the inner exception</param>
        /// <remarks></remarks>
        public ValueNotAvailableException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Serialise the exception
        /// </summary>
        /// <param name="info">Serialisation information</param>
        /// <param name="context">Serialisation context</param>
        /// <remarks></remarks>
        public ValueNotAvailableException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)

        {
        }
    }

    /// <summary>
    /// Exception thrown when a NOVAS function returns a non-zero, error completion code.
    /// </summary>
    /// <remarks>This probably occurs because another variable has not been set or a required method has not been called.</remarks>
    // Exception for Helper.NET component exceptions
    public class NOVASFunctionException : HelperException
    {

        /// <summary>
        /// Create a new exception with message, function name and error code
        /// </summary>
        /// <param name="message">Message to be reported by the exception</param>
        /// <param name="FuncName">Name of the NOVAS function giving rise to the exception</param>
        /// <param name="ErrCode">Error code returned by the NOVAS function</param>
        /// <remarks></remarks>
        public NOVASFunctionException(string message, string FuncName, short ErrCode) : base(message + " Error returned from function " + FuncName + " - error code: " + ErrCode.ToString())
        {
        }

        /// <summary>
        /// Create a new exception with message 
        /// </summary>
        /// <param name="message">Message to be reported by the exception</param>
        /// <param name="inner">Exception to be reported as the inner exception</param>
        /// <remarks></remarks>
        public NOVASFunctionException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Serialise the exception
        /// </summary>
        /// <param name="info">Serialisation information</param>
        /// <param name="context">Serialisation context</param>
        /// <remarks></remarks>
        public NOVASFunctionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)

        {
        }
    }

    /// <summary>
    /// Exception thrown when an iterative Transform function fails to converge.
    /// </summary>
    /// <remarks></remarks>
    // Exception for Transform iteration convergence failure exceptions
    public class ConvergenceFailureException : HelperException
    {

        /// <summary>
        /// Create a new exception with the message
        /// </summary>
        /// <param name="message">Message to be reported by the exception</param>
        /// <remarks></remarks>
        public ConvergenceFailureException(string message) : base(message)
        {
        }

        /// <summary>
        /// Create a new exception with message 
        /// </summary>
        /// <param name="message">Message to be reported by the exception</param>
        /// <param name="inner">Exception to be reported as the inner exception</param>
        /// <remarks></remarks>
        public ConvergenceFailureException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Serialise the exception
        /// </summary>
        /// <param name="info">Serialisation information</param>
        /// <param name="context">Serialisation context</param>
        /// <remarks></remarks>
        public ConvergenceFailureException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)

        {
        }
    }


}