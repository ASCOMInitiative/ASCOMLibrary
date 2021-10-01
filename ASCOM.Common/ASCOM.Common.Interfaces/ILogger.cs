namespace ASCOM.Common.Interfaces
{
    /// <summary>
    /// Very simple logger interface, LoggerExtensions build on this to add standard functionality
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Get the current logging level. The log should write out at this level and higher
        /// </summary>
        LogLevel LoggingLevel
        {
            get;
        }

        /// <summary>
        /// Set the current minimum logging level. The logger should write out at this level and higher and not write out lower level events. This method should never throw.
        /// </summary>
        /// <param name="level"></param>
        void SetMinimumLoggingLevel(LogLevel level);

        /// <summary>
        /// Log out a message at a given logging level. Only active levels should be added to the log. This method should never throw.
        /// </summary>
        /// <param name="level">The level for this log entry</param>
        /// <param name="message">The message to record</param>
        void Log(LogLevel level, string message);
    }
}
