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
        /// Set the log level and message
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="message"></param>
        public LogEvent(LogLevel logLevel, string message)
        {
            LogLevel = logLevel;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        /// <summary>
        /// Set the lkog level, message and property values
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="message"></param>
        /// <param name="propertyValues"></param>
        public LogEvent(LogLevel logLevel, string message, object[] propertyValues)
        {
            LogLevel = logLevel;
            Message = message ?? throw new ArgumentNullException(nameof(message));
            PropertyValues = propertyValues;
        }

        /// <summary>
        /// Set the log level, message and exception
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public LogEvent(LogLevel logLevel, string message, Exception exception)
        {
            LogLevel = logLevel;
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        /// <summary>
        /// set the log level, message, exception and property values
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="propertyValues"></param>
        public LogEvent(LogLevel logLevel, string message, Exception exception, object[] propertyValues)
        {
            LogLevel = logLevel;
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            PropertyValues = propertyValues;
        }
    }
}