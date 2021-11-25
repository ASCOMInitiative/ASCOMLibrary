using ASCOM.Common.Interfaces;

namespace ASCOM.Common
{

    /// <summary>
    /// This is a standard set of extensions that add functionality to an ILogger. Because these can be implemented in a standard way they are not part of the interface
    /// </summary>
    public static class LoggerExtensions
    {
        public static void LogVerbose(this ILogger logger, string message)
        {
            logger?.Log(LogLevel.Verbose, message);
        }

        public static void LogDebug(this ILogger logger, string message)
        {
            logger?.Log(LogLevel.Debug, message);
        }

        public static void LogInformation(this ILogger logger, string message)
        {
            logger?.Log(LogLevel.Information, message);
        }

        public static void LogWarning(this ILogger logger, string message)
        {
            logger?.Log(LogLevel.Warning, message);
        }

        public static void LogError(this ILogger logger, string message)
        {
            logger?.Log(LogLevel.Error, message);
        }

        public static void LogFatal(this ILogger logger, string message)
        {
            logger?.Log(LogLevel.Fatal, message);
        }

        public static bool IsLevelActive(this ILogger logger, LogLevel level)
        {
            return level >= (logger?.LoggingLevel ?? LogLevel.Fatal);
        }
    }
}