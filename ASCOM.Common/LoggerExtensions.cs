using ASCOM.Common.Interfaces;

namespace ASCOM.Common
{

    /// <summary>
    /// This is a standard set of extensions that add functionality to an ILogger. Because these can be implemented in a standard way they are not part of the interface
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Log a message in TraceLogger format if the logging device is a TraceLogger, otherwise use the ILogger.Log format
        /// </summary>
        /// <param name="logger">ILogger device instance</param>
        /// <param name="logLevel">ILogger LogLevel to use if the logger is not a TraceLogger.</param>
        /// <param name="method">Calling method name</param>
        /// <param name="message">Log message</param>
        public static void LogMessage(this ILogger logger, LogLevel logLevel, string method, string message)
        {
            // Use a log format depending on whether the supplied logger is an ITraceLogger instance or just a plain ILogger instance
            if (logger is ITraceLogger traceLogger) // The logger is an ITraceLogger instance so use the ITraceLogger.LogMessage method to log the message
            {
                if (logLevel >= logger.LoggingLevel) // Only log the message if it is at or above the current logging level
                {
                    traceLogger.LogMessage($"[Lib] {method}", message); // Use the ITraceLogger.LogMessage method
                }
            }
            else // The logger is null or an ILogger instance so use the ILogger.Log method if the logger is not null, otherwise ignore
            {
                logger?.Log(logLevel, $"{method} - {message}"); // Use the ILogger.Log method
            }
        }

        /// <summary>
        /// Create a blank line in a log
        /// </summary>
        /// <param name="logger">ILogger device instance</param>
        /// <param name="logLevel">ILogger LogLevel to use if the logger is not an ITraceLogger instance.</param>
        public static void BlankLine(this ILogger logger, LogLevel logLevel)
        {
            // Use a log format depending on whether the supplied logger is a ITraceLogger instance or an ILogger instance
            if (logger is ITraceLogger traceLogger) // The logger is an ITraceLogger instance so use the ITraceLogger.BlankLine method to create the blank line
            {
                traceLogger.BlankLine(); // Use the ITraceLogger.LogMessage method
            }
            else // The logger is null or an ILogger instance so use the ILogger.Log method 
            {
                logger?.Log(logLevel, $""); // Use the ILogger.Log method to create a blank line or ignore if the logger instance is null
            }
        }

        /// <summary>
        /// Log a message at verbose level
        /// </summary>
        /// <param name="logger">ILogger device instance</param>
        /// <param name="message">Message to log</param>
        public static void LogVerbose(this ILogger logger, string message)
        {
            logger?.Log(LogLevel.Verbose, message);
        }

        /// <summary>
        /// Log a message at debug level
        /// </summary>
        /// <param name="logger">ILogger device instance</param>
        /// <param name="message">Message to log</param>
        public static void LogDebug(this ILogger logger, string message)
        {
            logger?.Log(LogLevel.Debug, message);
        }

        /// <summary>
        /// Log a message at information level
        /// </summary>
        /// <param name="logger">ILogger device instance</param>
        /// <param name="message">Message to log</param>
        public static void LogInformation(this ILogger logger, string message)
        {
            logger?.Log(LogLevel.Information, message);
        }

        /// <summary>
        /// Log a message at warning level
        /// </summary>
        /// <param name="logger">ILogger device instance</param>
        /// <param name="message">Message to log</param>
        public static void LogWarning(this ILogger logger, string message)
        {
            logger?.Log(LogLevel.Warning, message);
        }

        /// <summary>
        /// Log a message at error level
        /// </summary>
        /// <param name="logger">ILogger device instance</param>
        /// <param name="message">Message to log</param>
        public static void LogError(this ILogger logger, string message)
        {
            logger?.Log(LogLevel.Error, message);
        }

        /// <summary>
        /// Log a message at fatal level
        /// </summary>
        /// <param name="logger">ILogger device instance</param>
        /// <param name="message">Message to log</param>
        public static void LogFatal(this ILogger logger, string message)
        {
            logger?.Log(LogLevel.Fatal, message);
        }

        /// <summary>
        /// Determine whether the supplied ILogger instance is set at the specified log level or at a more verbose level
        /// </summary>
        /// <param name="logger">ILogger device instance</param>
        /// <param name="level">Test log level</param>
        /// <returns>True if the ILogger is set at the specified log level or at a more verbose level</returns>
        public static bool IsLevelActive(this ILogger logger, LogLevel level)
        {
            return level >= (logger?.LoggingLevel ?? LogLevel.Fatal);
        }
    }
}
