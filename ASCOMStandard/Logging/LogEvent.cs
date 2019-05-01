using System;

namespace ASCOM.Alpaca.Logging
{
    public class LogEvent 
    {
        public LogLevel LogLevel { get; }
        public Exception Exception { get; }
        public string Message { get; }
        public object[] PropertyValues { get; }

        public LogEvent(LogLevel logLevel, string message)
        {
            LogLevel = logLevel;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public LogEvent(LogLevel logLevel, string message, object[] propertyValues)
        {
            LogLevel = logLevel;
            Message = message ?? throw new ArgumentNullException(nameof(message));
            PropertyValues = propertyValues;
        }

        public LogEvent(LogLevel logLevel, string message, Exception exception)
        {
            LogLevel = logLevel;
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }
        
        public LogEvent(LogLevel logLevel, string message, Exception exception, object[] propertyValues)
        {
            LogLevel = logLevel;
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            PropertyValues = propertyValues;
        }
    }
}