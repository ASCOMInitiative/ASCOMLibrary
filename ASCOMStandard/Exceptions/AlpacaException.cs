using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ASCOM.Alpaca
{
    /// <summary>
    /// This is the generic ASCOM Alpaca exception for use when none of the more specific exceptions applies.
    /// </summary>
    /// <remarks>
    /// <para>This exception should only be thrown if there is no other more appropriate ASCOM exception already defined, e.g. NotImplementedException,
    /// InvalidOperationException, InvalidValueException, NotConnectedException etc. Conform will not accept AlpacaException if a more appropriate exception is applicable.</para>
    /// </remarks>
    [Serializable]
    public class AlpacaException : Exception
    {
        /// <summary>The Alpaca error number for this exception (0x400 - 0x4FF)</summary>
        public int Number { get; set; }

        /// <summary>
        /// Create a new <see cref = "AlpacaException" /> containing the <see cref="ErrorCodes.UnspecifiedError"/> number and a default message.
        /// </summary>
        public AlpacaException()
            : base("Exception cause unknown - the application or driver did not set an exception message.")
        {
            HResult = ErrorCodes.UnspecifiedError;
            Number = ErrorCodes.UnspecifiedError;
        }

        /// <summary>
        /// Create a new <see cref = "AlpacaException" /> containing the <see cref="ErrorCodes.UnspecifiedError"/> number and a provided message.
        /// </summary>
        /// <param name = "message">The human-readable description of the problem.</param>
        public AlpacaException(string message)
            : base(message)
        {
            HResult = ErrorCodes.UnspecifiedError;
            Number = ErrorCodes.UnspecifiedError;
        }

        /// <summary>
        /// Create a new <see cref = "AlpacaException" /> containing the <see cref="ErrorCodes.UnspecifiedError"/> number, a provided message and an originating exception.
        /// </summary>
        /// <param name = "message">The human-readable description of the problem.</param>
        /// <param name = "innerException">The caught (inner) exception.</param>
        public AlpacaException(string message, Exception innerException)
            : base(message, innerException)
        {
            HResult = ErrorCodes.UnspecifiedError;
            Number = ErrorCodes.UnspecifiedError;
        }

        /// <summary>
        /// Create a new <see cref = "AlpacaException" /> containing the provided error number and message.
        /// </summary>
        /// <param name = "message">Descriptive text describing the cause of the exception</param>
        /// <param name = "number">Error code for the exception (0x400 - 0xFFF).</param>
        public AlpacaException(string message, int number)
            : base(message)
        {
            HResult = number;
            Number = number;
        }

        /// <summary>
        /// Create a new <see cref = "AlpacaException" /> containing the provided error number, message and originating exception.
        /// </summary>
        /// <param name = "message">Descriptive text describing the cause of the exception</param>
        /// <param name = "number">Alpaca error number for the exception (0x400 - 0xFFF).</param>
        /// <param name = "innerException">The inner exception that led to throwing this exception</param>
        public AlpacaException(string message, int number, Exception innerException)
            : base(message, innerException)
        {
            HResult = number;
            Number = number;
        }

        /// <summary>
        /// De-serialise and create a new instance of the <see cref = "AlpacaException" /> class.
        /// </summary>
        /// <param name = "info">The <see cref = "T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name = "context">The <see cref = "T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref = "ArgumentNullException">The <paramref name = "info" /> parameter is null.</exception>
        /// <exception cref = "SerializationException">The class name is null or <see cref = "Exception.HResult" /> is zero (0).</exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected AlpacaException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Number = info.GetInt32("Number");
        }

        /// <summary>
        /// Serialise the exception
        /// </summary>
        /// <param name="info">Serialised information</param>
        /// <param name="context">Formatting context</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException("info"); // Make sure that we have an info object

            info.AddValue("Number", Number); // Add the Number field, which is unique to this class
            base.GetObjectData(info, context); // Call the base class to serialise the exception
        }
    }
}