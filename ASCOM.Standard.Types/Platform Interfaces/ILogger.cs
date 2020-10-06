using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Standard.Interfaces
{
    public interface ILogger
    {
        LogLevel LoggingLevel
        {
            get;
        }

        void SetMinimumLoggingLevel(LogLevel level);

        void Log(LogLevel level, string message);
    }
}
