using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// This exception is used when a JSON date-time value returned by the device is not a UTC date-time.
    /// </summary>
    public class InvalidJsonDateTimeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidJsonDateTimeException"/> class.
        /// </summary>
        public InvalidJsonDateTimeException() : base("The JSON date time value is invalid.")
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidJsonDateTimeException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidJsonDateTimeException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidJsonDateTimeException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="jsonString">The JSON date-time string from the device.</param>
        /// <param name="invalidDateTime">The DateTime value de-serialised from the returned JSON string.</param>
        public InvalidJsonDateTimeException(string message,string jsonString,DateTime invalidDateTime) : base(message)
        {
            InvalidDateTime = invalidDateTime;
            JsonDateTimeString = jsonString;
        }

        /// <summary>
        /// The invalid date time value returned by the Alpaca device
        /// </summary>
        public DateTime InvalidDateTime { get; private set; }

        /// <summary>
        /// Gets or sets the JSON-formatted date-time string returned by the device.
        /// </summary>
        public string JsonDateTimeString { get; private set; }
    }
}
