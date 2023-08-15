using System;
// Exceptions for the Utilities namesapce

namespace ASCOM.Com.Exceptions
{
    /// <summary>
    /// Base exception for the Utilities components
    /// </summary>
    /// <remarks></remarks>
    public class HelperException : Exception
    {

        /// <summary>
        /// Create a new exception with message
        /// </summary>
        /// <param name="message">Message to be reported by the exception</param>
        /// <remarks></remarks>
        public HelperException(string message) : base(message)
        {
        }

        /// <summary>
        /// Create a new exception with message and inner exception
        /// </summary>
        /// <param name="message">Message to be reported by the exception</param>
        /// <param name="inner">Exception to be reported as the inner exception</param>
        /// <remarks></remarks>
        public HelperException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Serialise the exception
        /// </summary>
        /// <param name="info">Serialisation information</param>
        /// <param name="context">Serialisation context</param>
        /// <remarks></remarks>
        public HelperException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }

    }

    /// <summary>
    /// Exception thrown when an invalid value is passed to a Utilities component
    /// </summary>
    /// <remarks></remarks>
    public class InvalidValueException : HelperException
    {

        /// <summary>
        /// Create a new exception with message 
        /// </summary>
        /// <param name="message">Message to be reported by the exception</param>
        /// <remarks></remarks>
        public InvalidValueException(string message) : base(message)
        {
        }

        /// <summary>
        /// Create a new exception with message 
        /// </summary>
        /// <param name="message">Message to be reported by the exception</param>
        /// <param name="inner">Exception to be reported as the inner exception</param>
        /// <remarks></remarks>
        public InvalidValueException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Serialise the exception
        /// </summary>
        /// <param name="info">Serialisation information</param>
        /// <param name="context">Serialisation context</param>
        /// <remarks></remarks>
        public InvalidValueException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// Exception thrown if there is any problem in reading or writing the profile from or to the file system
    /// </summary>
    /// <remarks>This is an ifrastructural exception indicatig that there is a problem at the file access layer
    /// in the profile code. Possible underlying reasons are security access permissions, file system full and hardware failure.
    /// </remarks>
    public class ProfilePersistenceException : HelperException
    {

        /// <summary>
        /// Create a new exception with message 
        /// </summary>
        /// <param name="message">Message to be reported by the exception</param>
        /// <remarks></remarks>
        public ProfilePersistenceException(string message) : base(message)
        {
        }

        /// <summary>
        /// Create a new exception with message 
        /// </summary>
        /// <param name="message">Message to be reported by the exception</param>
        /// <param name="inner">Exception to be reported as the inner exception</param>
        /// <remarks></remarks>
        public ProfilePersistenceException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Serialise the exception
        /// </summary>
        /// <param name="info">Serialisation information</param>
        /// <param name="context">Serialisation context</param>
        /// <remarks></remarks>
        public ProfilePersistenceException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// Exception thrown when a profile request is made for a driver that is not registered
    /// </summary>
    /// <remarks>Drivers must be registered before other profile commands are used to manipulate their 
    /// values.</remarks>
    public class DriverNotRegisteredException : HelperException
    {

        /// <summary>
        /// Create a new exception with message 
        /// </summary>
        /// <param name="message">Message to be reported by the exception</param>
        /// <remarks></remarks>
        public DriverNotRegisteredException(string message) : base(message)
        {
        }

        /// <summary>
        /// Create a new exception with message 
        /// </summary>
        /// <param name="message">Message to be reported by the exception</param>
        /// <param name="inner">Exception to be reported as the inner exception</param>
        /// <remarks></remarks>
        public DriverNotRegisteredException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Serialise the exception
        /// </summary>
        /// <param name="info">Serialisation information</param>
        /// <param name="context">Serialisation context</param>
        /// <remarks></remarks>
        public DriverNotRegisteredException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }

}