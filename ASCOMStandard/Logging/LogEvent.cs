using System;

namespace ASCOM.Alpaca.Logging
{
    /// <summary>
    /// Class to hold details of an event to be logged
    /// </summary>
    public class LogEvent 
    {
        /// <summary>
        /// Event log level
        /// </summary>
        public LogLevel LogLevel { get; }
        /// <summary>
        /// Identifier of the logged event
        /// </summary>
        public string EventId { get; set; }
        /// <summary>
        /// Event exception
        /// </summary>
        public Exception Exception { get; }
        /// <summary>
        /// Event message
        /// </summary>
        public string Message { get; }
        /// <summary>
        /// Event property values
        /// </summary>
        public object[] PropertyValues { get; }

        /// <summary>
        /// Initialise an event to be logged
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="propertyValues"></param>
        public LogEvent(LogLevel logLevel, string eventId = null, Exception exception = null, string message = null, object[] propertyValues = null)
        {
            LogLevel = logLevel;
            EventId = eventId;
            Exception = exception;
            Message = message;
            PropertyValues = propertyValues;
        }
    }
}