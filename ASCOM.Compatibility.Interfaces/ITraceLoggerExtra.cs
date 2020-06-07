using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Compatibility.Interfaces
{
    public interface ITraceLoggerExtra
    {
        void LogContinue(string Message);

        void LogFinish(string Message);

        void LogMessage(string Identifier, string Message);
    }
}
