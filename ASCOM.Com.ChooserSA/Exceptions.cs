﻿using System;
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
    }

    /// <summary>
    /// Exception thrown if there is any problem in reading or writing the profile from or to the file system
    /// </summary>
    /// <remarks>This is an infrastructural exception indicating that there is a problem at the file access layer
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
    }
}