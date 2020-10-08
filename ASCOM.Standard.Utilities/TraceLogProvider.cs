using ASCOM.Standard.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Standard.Utilities
{
    public class TraceLogProvider : ILogger
    {
        private readonly TraceLogger logger;

        public TraceLogProvider()
        {
            logger = new TraceLogger();
        }

        public TraceLogProvider(string LogFileType)
        {
            logger = new TraceLogger(null, LogFileType);
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
                logger.LogMessage($"[{level}]", message);
            }
        }

        public void SetMinimumLoggingLevel(LogLevel level)
        {
            LoggingLevel = level;
        }
    }
}
