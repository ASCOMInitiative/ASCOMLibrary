using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Compatibility.Interfaces
{
    public interface ITraceLogger
    {
        void LogStart(string Identifier, string Message);

        void LogContinue(string Message, bool HexDump);

        void LogFinish(string Message, bool HexDump);

        void LogMessage(string Identifier, string Message, bool HexDump);

        bool Enabled { get; set; }

        void LogIssue(string Identifier, string Message);

        void SetLogFile(string LogFileName, string LogFileType);

        void BlankLine();

        string LogFileName { get; }

        void LogMessageCrLf(string Identifier, string Message);

        string LogFilePath { get; set; }

        int IdentifierWidth { get; set; }

        #region ITraceLoggerExtra
        void LogContinue(string Message);

        void LogFinish(string Message);

        void LogMessage(string Identifier, string Message);
        #endregion
    }
}
