namespace ASCOM.Common.Interfaces
{

    /// <summary>
    /// Trace logger interface definition
    /// </summary>
    /// <remarks>Inherits the ILogger interface and exposed the </remarks>
    public interface ITraceLogger : ILogger
    {
        /// <summary>
        /// Log a message to the logger device
        /// </summary>
        /// <param name="method">Name of the initiating method</param>
        /// <param name="message">Message to log</param>
        void LogMessage(string method, string message);

        /// <summary>
        /// Create a blank line in the log
        /// </summary>
        void BlankLine();

        /// <summary>
        /// Enable or disable the logger
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Set the width in characters of the identifier / method name column in the log file
        /// </summary>
        int IdentifierWidth { get; set; }

        /// <summary>
        /// False to log local times, true to log UTC times, 
        /// </summary>
        bool UseUtcTime { get; set; }

        /// <summary>
        /// True to honour CrLf sequences (a log message could occupy one or more lines in the log), False to print CrLf as [Cr]{Lf} ensuring that each message only occupies one line in the log.
        /// </summary>
        bool RespectCrLf { get; set; }
    }
}
