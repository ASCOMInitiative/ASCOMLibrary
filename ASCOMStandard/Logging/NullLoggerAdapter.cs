namespace ASCOM.Alpaca.Logging
{
    /// <summary>
    /// Null logger for tests purposes
    /// </summary>
    public class NullLoggerAdapter : ILogger
    {
        public void Log(LogEvent logEvent)
        {
            //Do Nothing
        }
    }
}