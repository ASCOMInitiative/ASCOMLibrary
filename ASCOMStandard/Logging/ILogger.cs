namespace ASCOM.Alpaca.Logging
{
    /// <summary>
    /// ILogger interface
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Log an event
        /// </summary>
        /// <param name="logEvent"></param>
        void Log(LogEvent logEvent);
    }
}