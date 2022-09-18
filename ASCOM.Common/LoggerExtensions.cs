using ASCOM.Common.Interfaces;

namespace ASCOM.Common
{

    /// <summary>
    /// This is a standard set of extensions that add functionality to an ILogger. Because these can be implemented in a standard way they are not part of the interface
    /// </summary>
    public static class LoggerExtensions
    {

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
