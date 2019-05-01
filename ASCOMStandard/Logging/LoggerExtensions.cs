using System;

namespace ASCOM.Alpaca.Logging
{
    public static class LoggerExtensions
    {
        public static void LogTrace(this ILogger logger, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Trace, message, args));
        }
        
        public static void LogTrace(this ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Trace, message, exception, args));
        }
        
        public static void LogDebug(this ILogger logger, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Debug, message, args));
        }
        
        public static void LogDebug(this ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Debug, message, exception, args));
        }
        
        public static void LogInformation(this ILogger logger, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Information, message, args));
        }
        
        public static void LogInformation(this ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Information, message, exception, args));
        }
        
        public static void LogWarning(this ILogger logger, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Warning, message, args));
        }
        
        public static void LogWarning(this ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Warning, message, exception, args));
        }
        
        public static void LogError(this ILogger logger, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Error, message, args));
        }
        
        public static void LogError(this ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Error, message, exception, args));
        }
        
        public static void LogFatal(this ILogger logger, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Fatal, message, args));
        }
        
        public static void LogFatal(this ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.Log(new LogEvent(LogLevel.Fatal, message, exception, args));
        }
    }
}