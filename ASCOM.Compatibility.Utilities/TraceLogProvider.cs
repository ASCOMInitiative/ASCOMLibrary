using ASCOM.Standard.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Compatibility.Utilities
{
    public class TraceLogProvider : ILogger
    {
        private readonly ASCOM.Utilities.TraceLogger logger;

        public TraceLogProvider()
        {
            logger = new ASCOM.Utilities.TraceLogger()
            {
                Enabled = true
            };
        }

        public TraceLogProvider(string LogFileType)
        {
            logger = new ASCOM.Utilities.TraceLogger(null, LogFileType)
            {
                Enabled = true
            };
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
