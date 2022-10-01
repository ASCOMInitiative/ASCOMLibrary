namespace ASCOM.Common.Interfaces
{
    /// <summary>
    /// Trace logger interface, extends ILogger for component-specific logging
    /// </summary>
    public interface ITraceLogger : ILogger
    {
        /// <summary>
        /// Write a message to the trace log
        /// </summary>
        /// <param name="identifier">Member name or function name.</param>
        /// <param name="message">Message text</param>
        void LogMessage(string identifier, string message);
    }
}