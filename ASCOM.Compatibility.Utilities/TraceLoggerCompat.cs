using ASCOM.Standard.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCOM.Compatibility.Utilities
{
    public class TraceLoggerCompat : ASCOM.Utilities.TraceLogger, ASCOM.Standard.Interfaces.ITraceLogger, ASCOM.Standard.Interfaces.ILogger
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
