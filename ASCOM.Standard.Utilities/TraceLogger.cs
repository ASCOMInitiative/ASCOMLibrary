using ASCOM.Compatibility.Interfaces;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace ASCOM.Standard.Utilities
{
    /// <summary>
    /// Creates a log file for a driver or application. Uses a similar file name and internal format to the serial logger. Multiple logs can be created simultaneously if needed.
    /// </summary>
    /// <remarks>
    ///<para>In automatic mode the file will be stored in an ASCOM folder within XP's My Documents folder or equivalent places
    /// in other operating systems. Within the ASCOM folder will be a folder named Logs yyyy-mm-dd where yyyy, mm and dd are
    /// today's year, month and day numbers.The trace file will appear within the day folder with the name
    /// ASCOM.Identifier.hhmm.ssffff where hh, mm, ss and ffff are the current hour, minute, second and fraction of second
    /// numbers at the time of file creation.
    /// </para>
    /// <para>Within the file the format of each line is hh:mm:ss.fff Identifier Message where hh, mm, ss and fff are the hour, minute, second
    /// and fractional second at the time that the message was logged, Identifier is the supplied identifier (usually the subroutine,
    /// function, property or method from which the message is sent) and Message is the message to be logged.</para>
    ///</remarks>
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

        /// <summary>
        /// Creates a new TraceLogger instance
        /// </summary>
        /// <remarks>The LogFileType is used in the file name to allow you to quickly identify which of
        /// several logs contains the information of interest.
        /// <para>This call enables automatic logging and sets the file type to "Default".</para></remarks>
        public TraceLogger()
        {
            this.traceLoggerHasBeenDisposed = false;
            this.g_IdentifierWidth = 25;
            this.g_LogFileName = "";
            this.autoLogFilePath = true;
            this.g_LogFileType = "Default";
            this.g_DefaultLogFilePath = !string.IsNullOrEmpty(Environment.GetFolderPath(Environment.SpecialFolder.Personal)) ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ASCOM", "Logs " + DateTime.Now.ToString("yyyy-MM-dd")) : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "ASCOM", "SystemLogs", "Logs " + DateTime.Now.ToString("yyyy-MM-dd"));
            this.g_LogFilePath = this.g_DefaultLogFilePath;
            this.mut = new Mutex(false, "TraceLoggerMutex");
        }

        /// <summary>
        /// Creates a new TraceLogger instance and initialises filename and type
        /// </summary>
        /// <param name="LogFileName">Fully qualified trace file name or null string to use automatic file naming (recommended)</param>
        /// <param name="LogFileType">String identifying the type of log e,g, Focuser, LX200, GEMINI, MoonLite, G11</param>
        /// <remarks>The LogFileType is used in the file name to allow you to quickly identify which of several logs contains the information of interest.</remarks>
        public TraceLogger(string LogFileName, string LogFileType)
          : this()
        {
            this.g_LogFileName = LogFileName;
            this.g_LogFileType = LogFileType;
        }

        /// <summary>
        /// Create and enable a new TraceLogger instance with automatic naming based on the supplied log file type
        /// </summary>
        /// <param name="LogFileType">String identifying the type of log e,g, Focuser, LX200, GEMINI, MoonLite, G11</param>
        /// <remarks>The LogFileType is used in the file name to allow you to quickly identify which of several logs contains the information of interest.</remarks>
        public TraceLogger(string LogFileType)
          : this()
        {
            this.g_LogFileType = LogFileType;
            this.g_Enabled = true;
        }

        /// IDisposable
        /// <summary>
        /// Disposes of the TraceLogger object
        /// </summary>
        /// <param name="disposing">True if being disposed by the application, False if disposed by the finaliser.</param>
        /// <remarks></remarks>
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

        /// <summary>
        /// Disposes of the TraceLogger object
        /// </summary>
        /// <remarks></remarks>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        /// <summary>
        /// Finalizes the TraceLogger object
        /// </summary>
        /// <remarks></remarks>
        ~TraceLogger()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Logs an issue, closing any open line and opening a continuation line if necessary after the
        /// issue message.
        /// </summary>
        /// <param name="Identifier">Identifies the meaning of the message e.g. name of module or method logging the message.</param>
        /// <param name="Message">Message to log</param>
        /// <remarks>Use this for reporting issues that you don't want to appear on a line already opened
        /// with StartLine</remarks>
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

        /// <summary>
        /// Insert a blank line into the log file
        /// </summary>
        /// <remarks></remarks>
        public void BlankLine()
        {
            if (this.traceLoggerHasBeenDisposed)
                return;
            this.LogMessage("", "", false);
        }

        /// <summary>
        /// Logs a complete message in one call, including a hex translation of the message
        /// </summary>
        /// <param name="Identifier">Identifies the meaning of the message e.g. name of module or method logging the message.</param>
        /// <param name="Message">Message to log</param>
        /// <param name="HexDump">True to append a hex translation of the message at the end of the message</param>
        /// <remarks>
        /// <para>Use this for straightforward logging requirements. Writes all information in one command.</para>
        /// <para>Will create a LOGISSUE message in the log if called before a line started by LogStart has been closed with LogFinish.
        /// Possible reasons for this are exceptions causing the normal flow of code to be bypassed or logic errors.</para>
        /// </remarks>
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

        /// <summary>
        /// Displays a message respecting carriage return and linefeed characters
        /// </summary>
        /// <param name="Identifier">Identifies the meaning of the message e.g. name of module or method logging the message.</param>
        /// <param name="Message">The final message to terminate the line</param>
        /// <remarks>
        /// <para>Will create a LOGISSUE message in the log if called before a line has been started with LogStart.
        /// Possible reasons for this are exceptions causing the normal flow of code to be bypassed or logic errors.</para>
        /// </remarks>
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

        /// <summary>
        /// Writes the time and identifier to the log, leaving the line ready for further content through LogContinue and LogFinish
        /// </summary>
        /// <param name="Identifier">Identifies the meaning of the message e.g. name of module or method logging the message.</param>
        /// <param name="Message">Message to log</param>
        /// <remarks><para>Use this to start a log line where you want to write further information on the line at a later time.</para>
        /// <para>E.g. You might want to use this to record that an action has started and then append the word OK if all went well.
        ///  You would then end up with just one line to record the whole transaction even though you didn't know that it would be
        /// successful when you started. If you just used LogMsg you would have ended up with two log lines, one showing
        /// the start of the transaction and the next the outcome.</para>
        /// <para>Will create a LOGISSUE message in the log if called before a line started by LogStart has been closed with LogFinish.
        /// Possible reasons for this are exceptions causing the normal flow of code to be bypassed or logic errors.</para>
        /// </remarks>
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

        /// <summary>
        /// Appends further message to a line started by LogStart, appends a hex translation of the message to the line, does not terminate the line.
        /// </summary>
        /// <param name="Message">The additional message to appear in the line</param>
        /// <param name="HexDump">True to append a hex translation of the message at the end of the message</param>
        /// <remarks>
        /// <para>This can be called multiple times to build up a complex log line if required.</para>
        /// <para>Will create a LOGISSUE message in the log if called before a line has been started with LogStart.
        /// Possible reasons for this are exceptions causing the normal flow of code to be bypassed or logic errors.</para>
        /// </remarks>
        public void LogContinue(string Message, bool HexDump)
        {
            if (this.traceLoggerHasBeenDisposed)
                return;
            string Message1 = Message;
            if (HexDump)
                Message1 = Message + "  (HEX" + this.MakeHex(Message) + ")";
            this.LogContinue(Message1);
        }

        /// <summary>
        /// Closes a line started by LogStart with the supplied message and a hex translation of the message
        /// </summary>
        /// <param name="Message">The final message to terminate the line</param>
        /// <param name="HexDump">True to append a hex translation of the message at the end of the message</param>
        /// <remarks>
        /// <para>Will create a LOGISSUE message in the log if called before a line has been started with LogStart.
        /// Possible reasons for this are exceptions causing the normal flow of code to be bypassed or logic errors.</para>
        /// </remarks>
        public void LogFinish(string Message, bool HexDump)
        {
            if (this.traceLoggerHasBeenDisposed)
                return;
            string Message1 = Message;
            if (HexDump)
                Message1 = Message + "  (HEX" + this.MakeHex(Message) + ")";
            this.LogFinish(Message1);
        }

        /// <summary>
        /// Enables or disables logging to the file.
        /// </summary>
        /// <value>True to enable logging</value>
        /// <returns>Boolean, current logging status (enabled/disabled).</returns>
        /// <remarks>If this property is false then calls to LogMsg, LogStart, LogContinue and LogFinish do nothing. If True,
        /// supplied messages are written to the log file.</remarks>
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

        /// <summary>
        /// Sets the log filename and type if the constructor is called without parameters
        /// </summary>
        /// <param name="LogFileName">Fully qualified trace file name or null string to use automatic file naming (recommended)</param>
        /// <param name="LogFileType">String identifying the type of log e,g, Focuser, LX200, GEMINI, MoonLite, G11</param>
        /// <remarks>The LogFileType is used in the file name to allow you to quickly identify which of several logs contains the
        /// information of interest.
        /// <para><b>Note </b>This command is only required if the trace logger constructor is called with no
        /// parameters. It is provided for use in COM clients that can not call constructors with parameters.
        /// If you are writing a COM client then create the trace logger as:</para>
        /// <code>
        /// TL = New TraceLogger()
        /// TL.SetLogFile("","TraceName")
        /// </code>
        /// <para>If you are writing a .NET client then you can achieve the same end in one call:</para>
        /// <code>
        /// TL = New TraceLogger("",TraceName")
        /// </code>
        /// </remarks>
        public void SetLogFile(string LogFileName, string LogFileType)
        {
            if (this.traceLoggerHasBeenDisposed)
                return;
            this.g_LogFileName = LogFileName;
            this.g_LogFileType = LogFileType;
        }

        /// <summary>
        /// Return the full filename of the log file being created
        /// </summary>
        /// <value>Full filename of the log file</value>
        /// <returns>String filename</returns>
        /// <remarks>This call will return an empty string until the first line has been written to the log file
        /// as the file is not created until required.</remarks>
        public string LogFileName
        {
            get
            {
                return this.g_LogFileActualName;
            }
        }

        /// <summary>
        /// Set or return the path to a directory in which the log file will be created
        /// </summary>
        /// <returns>String path</returns>
        /// <remarks>Introduced with Platform 6.4.<para>If set, this path will be used instead of the user's Documents directory default path.This must be Set before the first message Is logged.</para></remarks>
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

        /// <summary>
        /// Set or return the width of the identifier field in the log message
        /// </summary>
        /// <value>Width of the identifier field</value>
        /// <returns>Integer width</returns>
        /// <remarks>Introduced with Platform 6.4.<para>If set, this width will be used instead of the default identifier field width.</para></remarks>
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

        /// <summary>
        /// Logs a complete message in one call
        /// </summary>
        /// <param name="Identifier">Identifies the meaning of the message e.g. name of module or method logging the message.</param>
        /// <param name="Message">Message to log</param>
        /// <remarks>
        /// <para>Use this for straightforward logging requirements. Writes all information in one command.</para>
        /// <para>Will create a LOGISSUE message in the log if called before a line started by LogStart has been closed with LogFinish.
        /// Possible reasons for this are exceptions causing the normal flow of code to be bypassed or logic errors.</para>
        /// <para>This overload is not available through COM, please use
        /// "LogMessage(ByVal Identifier As String, ByVal Message As String, ByVal HexDump As Boolean)"
        /// with HexDump set False to achieve this effect.</para>
        /// </remarks>
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

        /// <summary>
        /// Appends further message to a line started by LogStart, does not terminate the line.
        /// </summary>
        /// <param name="Message">The additional message to appear in the line</param>
        /// <remarks>
        /// <para>This can be called multiple times to build up a complex log line if required.</para>
        /// <para>Will create a LOGISSUE message in the log if called before a line has been started with LogStart.
        /// Possible reasons for this are exceptions causing the normal flow of code to be bypassed or logic errors.</para>
        /// <para>This overload is not available through COM, please use
        /// "LogContinue(ByVal Message As String, ByVal HexDump As Boolean)"
        /// with HexDump set False to achieve this effect.</para>
        /// </remarks>
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

        /// <summary>
        /// Closes a line started by LogStart with the supplied message
        /// </summary>
        /// <param name="Message">The final message to terminate the line</param>
        /// <remarks>
        /// <para>Can only be called once for each line started by LogStart.</para>
        /// <para>Will create a LOGISSUE message in the log if called before a line has been started with LogStart.
        /// Possible reasons for this are exceptions causing the normal flow of code to be bypassed or logic errors.</para>
        /// <para>This overload is not available through COM, please use
        /// "LogFinish(ByVal Message As String, ByVal HexDump As Boolean)"
        /// with HexDump set False to achieve this effect.</para>
        /// </remarks>
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
                str = Path.Combine(this.g_DefaultLogFilePath, "ASCOM." + this.g_LogFileType + "." + DateTime.Now.ToString("HHmm.ssfff"));
            }
            else
            {
                string directory = Path.Combine(this.g_LogFilePath, "Logs " + DateTime.Now.ToString("yyyy-MM-dd"));
                Directory.CreateDirectory(directory);
                str = Path.Combine(directory, "ASCOM." + this.g_LogFileType + "." + DateTime.Now.ToString("HHmm.ssfff"));
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