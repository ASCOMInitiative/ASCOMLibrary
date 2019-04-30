using System;

namespace ASCOM.Alpaca
{
    /// <summary>
    ///Creates a log file for a driver or application. Uses a similar file name and internal format to the serial logger. Multiple logs can be created simultaneously if needed.
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
    public class TraceLogger
    {

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
        }

        /// <summary>
        /// Insert a blank line into the log file
        /// </summary>
        /// <remarks></remarks>
        public void BlankLine()
        {
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
                return false;
            }
            set
            {
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
                return "";
            }
        }

        /// <summary>
        /// Set or return the path to a directory in which the log file will be created
        /// </summary>
        /// <returns>String path</returns>
        /// <remarks>Introduced with Platform 6.4.<para>If set, this path will be used instead of the user's Documents directory default path. This must be Set before the first message Is logged.</para></remarks>
        public string LogFilePath
        {
            get
            {
                return "";
            }
            set
            {
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
                return 0;
            }
            set
            {
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
        public void LogMessage(string Identifier, string Message)
        {
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
        public void LogContinue(string Message)
        {
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
        public void LogFinish(string Message)
        {
        }
    }

}
