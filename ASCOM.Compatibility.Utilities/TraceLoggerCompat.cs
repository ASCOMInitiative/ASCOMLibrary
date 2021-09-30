using ASCOM.Common;
using ASCOM.Common.Interfaces;

namespace ASCOM.Compatibility.Utilities
{
    public class TraceLoggerCompat : ASCOM.Utilities.TraceLogger, ILogger
    {
        public TraceLoggerCompat() : base()
        {

        }

        public TraceLoggerCompat(string LogFileType) : base(LogFileType)
        {

        }

        public TraceLoggerCompat(string LogFileName, string LogFileType) : base(LogFileName, LogFileType)
        {

        }

        public LogLevel LoggingLevel
        {
            get;
            private set;
        } = LogLevel.Information;

        public void Log(LogLevel level, string message)
        {
            if (this.IsLevelActive(level))
            {
                this.LogMessage($"[{level}]", message);
            }
        }

        public void SetMinimumLoggingLevel(LogLevel level)
        {
            LoggingLevel = level;
        }
    }
}
