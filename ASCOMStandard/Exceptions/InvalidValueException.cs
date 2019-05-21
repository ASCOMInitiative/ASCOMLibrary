using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ASCOM.Alpaca
{
    /// <summary>
    /// Exception to report an invalid value.
    /// </summary>
    /// <remarks>
    /// <para>The most useful way to use this exception is to inform the user which property/method/parameter received the invalid value and also the range of allowed values.</para>
    /// </remarks>
    [Serializable]
    public class InvalidValueException : AlpacaException
    {
        /// <summary>The name of the property or method that has an invalid value.</summary>
        public string PropertyOrMethod { get; private set; }

        /// <summary>The invalid value.</summary>
        public string InvalidValue { get; private set; }


        /// <summary>The lowest value in the valid range.</summary>
        public string LowestValidValue { get; private set; }

        /// <summary>The highest value in the valid range.</summary>
        public string HighestValidValue { get; private set; }

        /// <summary>
        /// Create a new exception using a default error message.
        /// </summary>
        public InvalidValueException() : base("The parameter value is invalid.", ErrorCodes.InvalidValue)
        {
        }

        /// <summary>
        ///  Create a new exception using the specified message
        /// </summary>
        /// <param name = "message">Exception description</param>
        public InvalidValueException(string message) : base(message, ErrorCodes.InvalidValue)
        {
        }

        /// <summary>
        ///  Create a new exception using the specified message and exception
        /// </summary>
        /// <param name = "message">Exception description</param>
        /// <param name = "inner">Underlying exception that caused this exception to be thrown.</param>
        public InvalidValueException(string message, Exception inner) : base(message, ErrorCodes.InvalidValue, inner)
        {
        }

        /// <summary>
        /// Create a new exception object and identify the specified driver property or method as the source.
        /// </summary>
        /// <param name = "propertyOrMethod">The name of the driver property/accessor or method that caused the exception</param>
        /// <param name = "invalidValue">The invalid value that was supplied</param>
        /// <param name="fromValue"></param>
        /// <param name="toValue"></param>
        public InvalidValueException(string propertyOrMethod, string invalidValue, string fromValue, string toValue) 
            : base(String.Format(CultureInfo.InvariantCulture, "{0} - '{1}' is an invalid value. The valid range is: {2} to {3}.", propertyOrMethod, invalidValue, fromValue, toValue), ErrorCodes.InvalidValue)
        {
            PropertyOrMethod = propertyOrMethod;
            InvalidValue = invalidValue;
            LowestValidValue = fromValue;
            HighestValidValue = toValue;
        }

        /// <summary>
        /// De-serialise and create a new instance of the exception.
        /// </summary>
        /// <param name = "info">The <see cref = "SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name = "context">The <see cref = "StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref = "ArgumentNullException">The <paramref name = "info" /> parameter is null.</exception>
        /// <exception cref = "SerializationException">The class name is null or <see cref = "Exception.HResult" /> is zero (0).</exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected InvalidValueException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            PropertyOrMethod = info.GetString("PropertyOrMethod");
            InvalidValue = info.GetString("InvalidValue");
            LowestValidValue = info.GetString("LowestValidValue");
            HighestValidValue = info.GetString("HighestValidValue");
        }

        /// <summary>
        /// Serialise the exception
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException("info"); // Make sure that we have an info object

            info.AddValue("PropertyOrMethod", PropertyOrMethod); // Add the PropertyOrMethod field, which is unique to this class
            info.AddValue("InvalidValue", InvalidValue); // Add the InvalidValue field, which is unique to this class
            info.AddValue("LowestValidValue", LowestValidValue); // Add the LowestValidValue field, which is unique to this class
            info.AddValue("HighestValidValue", HighestValidValue); // Add the HighestValidValue field, which is unique to this class
            base.GetObjectData(info, context); // Call the base class to serialise the rest of the exception
        }
    }
}