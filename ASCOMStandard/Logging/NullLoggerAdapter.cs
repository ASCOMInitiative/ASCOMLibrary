namespace ASCOM.Alpaca.Logging
{
    public class NullLoggerAdapter : ILogger
    {
        public void Log(LogEvent logEvent)
        {
            //Do Nothing
        }
    }
}