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
            logger.Log(new LogEvent(LogLevel.Trace, message:message, propertyValues:args));
        }

        /// <summary>
        /// Create a Trace level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="eventId"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogTrace(this ILogger logger, string eventId, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Trace, eventId, message:message, propertyValues:args));
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
            logger.Log(new LogEvent(LogLevel.Trace, exception:exception, message:message, propertyValues:args));
        }

        /// <summary>
        /// Create a Trace level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="eventId"></param>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogTrace(this ILogger logger, string eventId, Exception exception, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Trace, eventId, exception, message, args));
        }

        /// <summary>
        /// Create a Debug level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogDebug(this ILogger logger, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Debug, message:message, propertyValues:args));
        }

        /// <summary>
        /// Create a Debug level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="eventId"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogDebug(this ILogger logger, string eventId, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Debug, eventId, message:message, propertyValues:args));
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
            logger.Log(new LogEvent(LogLevel.Debug, exception:exception, message:message, propertyValues:args));
        }

        /// <summary>
        /// Create a Debug level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="eventId"></param>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogDebug(this ILogger logger, string eventId, Exception exception, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Debug, eventId, exception, message, args));
        }

        /// <summary>
        /// Create a Information level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogInformation(this ILogger logger, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Information, message:message, propertyValues:args));
        }

        /// <summary>
        /// Create a Information level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="eventId"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogInformation(this ILogger logger, string eventId, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Information, eventId, message:message, propertyValues:args));
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
            logger.Log(new LogEvent(LogLevel.Information, exception:exception, message:message, propertyValues:args));
        }

        /// <summary>
        /// Create a Information level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="eventId"></param>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogInformation(this ILogger logger, string eventId, Exception exception, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Information, eventId, exception, message, args));
        }

        /// <summary>
        /// Create a Warning level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogWarning(this ILogger logger, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Warning, message:message, propertyValues:args));
        }

        /// <summary>
        /// Create a Warning level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="eventId"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogWarning(this ILogger logger, string eventId, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Warning, eventId, message:message, propertyValues:args));
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
            logger.Log(new LogEvent(LogLevel.Warning, exception:exception, message:message, propertyValues:args));
        }

        /// <summary>
        /// Create a Warning level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="eventId"></param>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogWarning(this ILogger logger, string eventId, Exception exception, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Warning, eventId, exception, message, args));
        }

        /// <summary>
        /// Create a Error level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogError(this ILogger logger, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Error, message:message, propertyValues:args));
        }

        /// <summary>
        /// Create a Error level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="eventId"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogError(this ILogger logger, string eventId, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Error, eventId, message:message, propertyValues:args));
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
            logger.Log(new LogEvent(LogLevel.Error, exception:exception, message:message, propertyValues:args));
        }

        /// <summary>
        /// Create a Error level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="eventId"></param>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogError(this ILogger logger, string eventId, Exception exception, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Error, eventId, exception, message, args));
        }

        /// <summary>
        /// Create a Fatal level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogFatal(this ILogger logger, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Fatal, message:message, propertyValues:args));
        }

        /// <summary>
        /// Create a Fatal level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="eventId"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogFatal(this ILogger logger, string eventId, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Fatal, eventId, message:message, propertyValues:args));
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
            logger.Log(new LogEvent(LogLevel.Fatal, exception:exception, message:message, propertyValues:args));
        }

        /// <summary>
        /// Create a Fatal level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="eventId"></param>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogFatal(this ILogger logger, string eventId, Exception exception, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Fatal, eventId, exception, message, args));
        }

        /// <summary>
        /// Create a Information level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <seealso cref="LogInformation(ASCOM.Alpaca.Logging.ILogger,string,object[])"/>
        [Obsolete("Alias of LogInformation")]
        public static void LogMessage(this ILogger logger, string message)
        {
            logger.Log(new LogEvent(LogLevel.Information, message:message));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="identifier"></param>
        /// <param name="message"></param>
        /// <seealso cref="LogInformation(ASCOM.Alpaca.Logging.ILogger,string,string,object[])"/>
        [Obsolete("Alias of LogInformation")]
        public static void LogMessage(this ILogger logger, string identifier, string message)
        {
            logger.Log(new LogEvent(LogLevel.Information, identifier, message:message));
        }

        /// <summary>
        /// Create a Information level log entry
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="identifier"></param>
        /// <param name="message"></param>
        /// <param name="parameters"></param>
        /// <seealso cref="LogInformation(ASCOM.Alpaca.Logging.ILogger,string,string,object[])"/>
        [Obsolete("Alias of LogInformation")]
        public static void LogMessage(this ILogger logger, string identifier, string message, params string[] parameters)
        {
            logger.Log(new LogEvent(LogLevel.Information, identifier, message:message, propertyValues:parameters));
        }

        /// <summary>
        /// Create a Information level log entry with empty message
        /// </summary>
        /// <param name="logger"></param>
        [Obsolete("Alias of LogInformation with message parameter = \"\"")]
        public static void BlankLine(this ILogger logger)
        {
            logger.Log(new LogEvent(LogLevel.Information, message:""));
        }
    }
}