namespace ASCOM.Alpaca.Logging
{
    /// <summary>
    /// Null logger for test purposes
    /// </summary>
    public class NullLoggerAdapter : ILogger
    {
        /// <summary>
        /// Override the Log event
        /// </summary>
        /// <param name="logEvent"></param>
        public void Log(LogEvent logEvent)
        {
            //Do Nothing
        }
    }
}