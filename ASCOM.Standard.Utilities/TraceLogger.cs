using ASCOM.Compatibility.Interfaces;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace ASCOM.Standard.Utilities
{
    public class TraceLogger : ITraceLogger, ITraceLoggerExtra, ITraceLoggerFull, IDisposable
    {
        private const int IDENTIFIER_WIDTH_DEFAULT = 25;
        private string g_LogFileName;
        private string g_LogFileType;
        private StreamWriter g_LogFile;
        private bool g_LineStarted;
        private bool g_Enabled;
        private string g_DefaultLogFilePath;
        private string g_LogFileActualName;
        private string g_LogFilePath;
        private int g_IdentifierWidth;
        private bool autoLogFilePath;
        private Mutex mut;
        private bool GotMutex;
        private bool traceLoggerHasBeenDisposed;

        public TraceLogger()
        {
            this.traceLoggerHasBeenDisposed = false;
            this.g_IdentifierWidth = 25;
            this.g_LogFileName = "";
            this.autoLogFilePath = true;
            this.g_LogFileType = "Default";
            this.g_DefaultLogFilePath = !string.IsNullOrEmpty(Environment.GetFolderPath(Environment.SpecialFolder.Personal)) ? Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\ASCOM\\Logs " + DateTime.Now.ToString("yyyy-MM-dd") : Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\ASCOM\\SystemLogs\\Logs " + DateTime.Now.ToString("yyyy-MM-dd");
            this.g_LogFilePath = this.g_DefaultLogFilePath;
            this.mut = new Mutex(false, "TraceLoggerMutex");
        }

        public TraceLogger(string LogFileName, string LogFileType)
          : this()
        {
            this.g_LogFileName = LogFileName;
            this.g_LogFileType = LogFileType;
        }

        public TraceLogger(string LogFileType)
          : this()
        {
            this.g_LogFileType = LogFileType;
            this.g_Enabled = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.traceLoggerHasBeenDisposed)
                return;
            this.traceLoggerHasBeenDisposed = true;
            if (disposing)
            {
                if (this.g_LogFile != null)
                {
                    try
                    {
                        this.g_LogFile.Flush();
                    }
                    catch (Exception ex)
                    {
                        ProjectData.SetProjectError(ex);
                        ProjectData.ClearProjectError();
                    }
                    try
                    {
                        this.g_LogFile.Close();
                    }
                    catch (Exception ex)
                    {
                        ProjectData.SetProjectError(ex);
                        ProjectData.ClearProjectError();
                    }
                    try
                    {
                        this.g_LogFile.Dispose();
                    }
                    catch (Exception ex)
                    {
                        ProjectData.SetProjectError(ex);
                        ProjectData.ClearProjectError();
                    }
                    this.g_LogFile = (StreamWriter)null;
                }
                if (this.mut != null)
                {
                    try
                    {
                        this.mut.Close();
                    }
                    catch (Exception ex)
                    {
                        ProjectData.SetProjectError(ex);
                        ProjectData.ClearProjectError();
                    }
                    this.mut = (Mutex)null;
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        ~TraceLogger()
        {
            this.Dispose(false);
        }

        public void LogIssue(string Identifier, string Message)
        {
            if (this.traceLoggerHasBeenDisposed)
                return;
            try
            {
                this.GetTraceLoggerMutex(nameof(LogIssue), "\"" + Identifier + "\", \"" + Message + "\"");
                if (this.g_Enabled)
                {
                    if (this.g_LogFile == null)
                        this.CreateLogFile();
                    if (this.g_LineStarted)
                        this.g_LogFile.WriteLine();
                    this.LogMsgFormatter(Identifier, Message, true, false);
                    if (this.g_LineStarted)
                        this.LogMsgFormatter("Continuation", "", false, false);
                }
            }
            finally
            {
                this.mut.ReleaseMutex();
            }
        }

        public void BlankLine()
        {
            if (this.traceLoggerHasBeenDisposed)
                return;
            this.LogMessage("", "", false);
        }

        public void LogMessage(string Identifier, string Message, bool HexDump)
        {
            string p_Msg = Message;
            if (this.traceLoggerHasBeenDisposed)
                return;
            try
            {
                this.GetTraceLoggerMutex(nameof(LogMessage), "\"" + Identifier + "\", \"" + Message + "\", " + HexDump.ToString() + "\"");
                if (this.g_LineStarted)
                    this.LogFinish(" ");
                if (this.g_Enabled)
                {
                    if (this.g_LogFile == null)
                        this.CreateLogFile();
                    if (HexDump)
                        p_Msg = Message + "  (HEX" + this.MakeHex(Message) + ")";
                    this.LogMsgFormatter(Identifier, p_Msg, true, false);
                }
            }
            finally
            {
                this.mut.ReleaseMutex();
            }
        }

        public void LogMessageCrLf(string Identifier, string Message)
        {
            if (this.traceLoggerHasBeenDisposed)
                return;
            try
            {
                this.GetTraceLoggerMutex("LogMessage", "\"" + Identifier + "\", \"" + Message + "\"");
                if (this.g_LineStarted)
                    this.LogFinish(" ");
                if (this.g_Enabled)
                {
                    if (this.g_LogFile == null)
                        this.CreateLogFile();
                    this.LogMsgFormatter(Identifier, Message, true, true);
                }
            }
            finally
            {
                this.mut.ReleaseMutex();
            }
        }

        public void LogStart(string Identifier, string Message)
        {
            if (this.traceLoggerHasBeenDisposed)
                return;
            try
            {
                this.GetTraceLoggerMutex(nameof(LogStart), "\"" + Identifier + "\", \"" + Message + "\"");
                if (this.g_LineStarted)
                {
                    this.LogFinish("LOGISSUE: LogStart has been called before LogFinish. Parameters: " + Identifier + " " + Message);
                }
                else
                {
                    this.g_LineStarted = true;
                    if (this.g_Enabled)
                    {
                        if (this.g_LogFile == null)
                            this.CreateLogFile();
                        this.LogMsgFormatter(Identifier, Message, false, false);
                    }
                }
            }
            finally
            {
                this.mut.ReleaseMutex();
            }
        }

        public void LogContinue(string Message, bool HexDump)
        {
            if (this.traceLoggerHasBeenDisposed)
                return;
            string Message1 = Message;
            if (HexDump)
                Message1 = Message + "  (HEX" + this.MakeHex(Message) + ")";
            this.LogContinue(Message1);
        }

        public void LogFinish(string Message, bool HexDump)
        {
            if (this.traceLoggerHasBeenDisposed)
                return;
            string Message1 = Message;
            if (HexDump)
                Message1 = Message + "  (HEX" + this.MakeHex(Message) + ")";
            this.LogFinish(Message1);
        }

        public bool Enabled
        {
            get
            {
                return this.g_Enabled;
            }
            set
            {
                this.g_Enabled = value;
            }
        }

        public void SetLogFile(string LogFileName, string LogFileType)
        {
            if (this.traceLoggerHasBeenDisposed)
                return;
            this.g_LogFileName = LogFileName;
            this.g_LogFileType = LogFileType;
        }

        public string LogFileName
        {
            get
            {
                return this.g_LogFileActualName;
            }
        }

        public string LogFilePath
        {
            get
            {
                return this.g_LogFilePath;
            }
            set
            {
                this.autoLogFilePath = Operators.CompareString(value, "", false) == 0;
                this.g_LogFilePath = value.TrimEnd("\\".ToCharArray());
            }
        }

        public int IdentifierWidth
        {
            get
            {
                return this.g_IdentifierWidth;
            }
            set
            {
                this.g_IdentifierWidth = value;
            }
        }

        [ComVisible(false)]
        public void LogMessage(string Identifier, string Message)
        {
            if (this.traceLoggerHasBeenDisposed)
                return;
            try
            {
                this.GetTraceLoggerMutex(nameof(LogMessage), "\"" + Identifier + "\", \"" + Message + "\"");
                if (this.g_LineStarted)
                    this.LogFinish(" ");
                if (this.g_Enabled)
                {
                    if (this.g_LogFile == null)
                        this.CreateLogFile();
                    this.LogMsgFormatter(Identifier, Message, true, false);
                }
            }
            finally
            {
                this.mut.ReleaseMutex();
            }
        }

        [ComVisible(false)]
        public void LogContinue(string Message)
        {
            if (this.traceLoggerHasBeenDisposed)
                return;
            try
            {
                this.GetTraceLoggerMutex(nameof(LogContinue), "\"" + Message + "\"");
                if (!this.g_LineStarted)
                    this.LogMessage("LOGISSUE", "LogContinue has been called before LogStart. Parameter: " + Message);
                else if (this.g_Enabled)
                {
                    if (this.g_LogFile == null)
                        this.CreateLogFile();
                    this.g_LogFile.Write(this.MakePrintable(Message, false));
                }
            }
            finally
            {
                this.mut.ReleaseMutex();
            }
        }

        [ComVisible(false)]
        public void LogFinish(string Message)
        {
            if (this.traceLoggerHasBeenDisposed)
                return;
            try
            {
                this.GetTraceLoggerMutex(nameof(LogFinish), "\"" + Message + "\"");
                if (!this.g_LineStarted)
                {
                    this.LogMessage("LOGISSUE", "LogFinish has been called before LogStart. Parameter: " + Message);
                }
                else
                {
                    this.g_LineStarted = false;
                    if (this.g_Enabled)
                    {
                        if (this.g_LogFile == null)
                            this.CreateLogFile();
                        this.g_LogFile.WriteLine(this.MakePrintable(Message, false));
                    }
                }
            }
            finally
            {
                this.mut.ReleaseMutex();
            }
        }

        private void CreateLogFile()
        {
            int num1 = 0;
            string gLogFileName = this.g_LogFileName;
            if (Operators.CompareString(gLogFileName, "", false) != 0)
            {
                if (Operators.CompareString(gLogFileName, "C:\\SerialTraceAuto.txt", false) != 0)
                {
                    try
                    {
                        this.g_LogFile = new StreamWriter(this.g_LogFileName + ".txt", false);
                        this.g_LogFile.AutoFlush = true;
                        this.g_LogFileActualName = this.g_LogFileName + ".txt";
                        return;
                    }
                    catch (Exception ex)
                    {
                        ProjectData.SetProjectError(ex);
                        throw;
                    }
                }
            }
            if (Operators.CompareString(this.g_LogFileType, "", false) == 0)
                throw new ArgumentNullException("TRACELOGGER.CREATELOGFILE - Call made but no log file type has been set");
            string str;
            if (this.autoLogFilePath)
            {
                Directory.CreateDirectory(this.g_DefaultLogFilePath);
                str = this.g_DefaultLogFilePath + "\\ASCOM." + this.g_LogFileType + "." + DateTime.Now.ToString("HHmm.ssfff");
            }
            else
            {
                string directory = this.g_LogFilePath + "\\Logs " + DateTime.Now.ToString("yyyy-MM-dd");
                Directory.CreateDirectory(directory);
                str = directory + "\\ASCOM." + this.g_LogFileType + "." + DateTime.Now.ToString("HHmm.ssfff");
            }
            do
            {
                this.g_LogFileActualName = str + num1.ToString() + ".txt";
                checked { ++num1; }
            }
            while (File.Exists(this.g_LogFileActualName));
            try
            {
                this.g_LogFile = new StreamWriter(this.g_LogFileActualName, false);
                this.g_LogFile.AutoFlush = true;
            }
            catch (IOException ex1)
            {
                ProjectData.SetProjectError((Exception)ex1);
                IOException ioException = ex1;
                bool flag = false;
                do
                {
                    try
                    {
                        this.g_LogFileActualName = str + num1.ToString() + ".txt";
                        this.g_LogFile = new StreamWriter(this.g_LogFileActualName, false);
                        this.g_LogFile.AutoFlush = true;
                        flag = true;
                    }
                    catch (IOException ex2)
                    {
                        ProjectData.SetProjectError((Exception)ex2);
                        ProjectData.ClearProjectError();
                    }
                    checked { ++num1; }
                }
                while (!(flag | num1 == 20));
                if (!flag)
                    throw new Exception("TraceLogger:CreateLogFile - Unable to create log file", (Exception)ioException);
                ProjectData.ClearProjectError();
            }
        }

        private string MakePrintable(string p_Msg, bool p_RespectCrLf)
        {
            string str = "";
            int num1 = p_Msg.Length;
            int Start = 1;
            while (Start <= num1)
            {
                int Number = Strings.AscW(Mid(p_Msg, Start, 1));
                int num2 = Number;
                str = num2 != 10 && num2 != 13 ? (num2 != -9 && num2 != 11 && (num2 != 12 && num2 != -17) && num2 <= 126 ? str + Mid(p_Msg, Start, 1) : str + "[" + Right("00" + Number.ToString("X"), 2) + "]") : (!p_RespectCrLf ? str + "[" + Right("00" + Number.ToString("X"), 2) + "]" : str + Mid(p_Msg, Start, 1));
                if (!(Number < 32 | Number > 126))
                    ;
                checked { ++Start; }
            }
            return str;
        }

        private string MakeHex(string p_Msg)
        {
            string str = "";
            int num = p_Msg.Length;
            int Start = 1;
            while (Start <= num)
            {
                int Number = Strings.AscW(Mid(p_Msg, Start, 1));
                str = str + "[" + Right("00" + Number.ToString("X"), 2) + "]";
                checked { ++Start; }
            }
            return str;
        }

        private void LogMsgFormatter(string p_Test, string p_Msg, bool p_NewLine, bool p_RespectCrLf)
        {
            try
            {
                p_Test = Left(p_Test + new string(' ', this.g_IdentifierWidth), this.g_IdentifierWidth);
                string str = DateTime.Now.ToString("HH:mm:ss.fff") + " " + this.MakePrintable(p_Test, p_RespectCrLf) + " " + this.MakePrintable(p_Msg, p_RespectCrLf);
                if (this.g_LogFile == null)
                    return;
                if (p_NewLine)
                    this.g_LogFile.WriteLine(str);
                else
                    this.g_LogFile.Write(str);
                this.g_LogFile.Flush();
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                //EventLogCode.LogEvent(nameof(LogMsgFormatter), "Exception", EventLogEntryType.Error, GlobalConstants.EventLogErrors.TraceLoggerException, ex.ToString());
                ProjectData.ClearProjectError();
            }
        }

        private void GetTraceLoggerMutex(string Method, string Parameters)
        {
            try
            {
                this.GotMutex = this.mut.WaitOne(5000, false);
            }
            catch (AbandonedMutexException ex)
            {
                ProjectData.SetProjectError((Exception)ex);
                AbandonedMutexException abandonedMutexException = ex;
                /*EventLogCode.LogEvent(nameof(TraceLogger), "AbandonedMutexException in " + Method + ", parameters: " + Parameters, EventLogEntryType.Error, GlobalConstants.EventLogErrors.TraceLoggerMutexAbandoned, abandonedMutexException.ToString());
                if (RegistryCommonCode.GetBool("Trace Abandoned Mutexes", false))
                {
                    EventLogCode.LogEvent(nameof(TraceLogger), "AbandonedMutexException in " + Method + ": Throwing exception to application", EventLogEntryType.Warning, GlobalConstants.EventLogErrors.TraceLoggerMutexAbandoned, (string)null);
                    throw;
                }
                else
                {
                    EventLogCode.LogEvent(nameof(TraceLogger), "AbandonedMutexException in " + Method + ": Absorbing exception, continuing normal execution", EventLogEntryType.Warning, GlobalConstants.EventLogErrors.TraceLoggerMutexAbandoned, (string)null);
                    this.GotMutex = true;
                    ProjectData.ClearProjectError();
                }*/
            }
            if (!this.GotMutex)
            {
                /*EventLogCode.LogEvent(Method, "Timed out waiting for TraceLogger mutex in " + Method + ", parameters: " + Parameters, EventLogEntryType.Error, GlobalConstants.EventLogErrors.TraceLoggerMutexTimeOut, (string)null);
                throw new Exception("Timed out waiting for TraceLogger mutex in " + Method + ", parameters: " + Parameters);*/
            }
        }

        public static string Mid(string str, int start, int length)
        {
            return str.Substring(start - 1, length);
        }

        public static string Right(string str, int length)
        {
            return str.Substring(str.Length - length, length);
        }

        public static string Left(string str, int length)
        {
            return str.Substring(0, length);
        }
    }
}
