using System;

namespace ASCOM.Alpaca.Logging
{
    /// <summary>
    /// Logger extensions
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Create a Trace level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogTrace(this ILogger logger, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Trace, message, args));
        }

        /// <summary>
        /// Create a Trace level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogTrace(this ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Trace, message, exception, args));
        }

        /// <summary>
        /// Create a Debug level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogDebug(this ILogger logger, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Debug, message, args));
        }

        /// <summary>
        /// Create a Debug level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogDebug(this ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Debug, message, exception, args));
        }

        /// <summary>
        /// Create a Information level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogInformation(this ILogger logger, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Information, message, args));
        }

        /// <summary>
        /// Create a Information level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogInformation(this ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Information, message, exception, args));
        }

        /// <summary>
        /// Create a Warning level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogWarning(this ILogger logger, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Warning, message, args));
        }

        /// <summary>
        /// Create a Warning level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogWarning(this ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Warning, message, exception, args));
        }

        /// <summary>
        /// Create a Error level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogError(this ILogger logger, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Error, message, args));
        }

        /// <summary>
        /// Create a Error level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogError(this ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Error, message, exception, args));
        }

        /// <summary>
        /// Create a Fatal level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogFatal(this ILogger logger, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Fatal, message, args));
        }

        /// <summary>
        /// Create a Fatal level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogFatal(this ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Fatal, message, exception, args));
        }
    }
}