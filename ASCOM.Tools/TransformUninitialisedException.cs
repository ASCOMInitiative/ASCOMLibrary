using System;


/// <summary>
/// Exception thrown by the Transform component when an uninitialised property is read.
/// </summary>
public class TransformUninitialisedException : ASCOM.DriverException
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