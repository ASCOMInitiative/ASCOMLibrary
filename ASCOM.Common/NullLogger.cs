using ASCOM.Common.Interfaces;

namespace ASCOM.Common
{
    public class NullLogger : ITraceLogger
    {
        public LogLevel LoggingLevel { get; private set; }

        public void SetMinimumLoggingLevel(LogLevel level)
        {
            LoggingLevel = level;
        }

        public void Log(LogLevel level, string message)
        {
            // NullLogger does not log anything
        }

        public void LogMessage(string identifier, string message)
        {
            // NullLogger does not log anything
        }
    }
}