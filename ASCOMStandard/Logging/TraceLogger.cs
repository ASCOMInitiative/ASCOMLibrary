using System;
using System.IO;
using System.Linq;

namespace ASCOM.Alpaca.Logging
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
    public class TraceLogger : ILogger, IDisposable
    {
        private const string LOG_TIME_FORMAT = "HH:mm:ss.fff";
        private const int IDENTIFIER_WIDTH_DEFAULT = 25;
        private const string TRACE_LOGGER_PATH_BASE = @"\ASCOM"; // Path to TraceLogger directory from My Documents
        private const string TRACE_LOGGER_DIRECTORY_NAME_BASE = @"\Logs "; // Fixed part of the TraceLogger directory name.  Note: The trailing space must be retained!;
        private const string TRACE_LOGGER_DIRECTORY_NAME_DATE_FORMAT = "yyyy-MM-dd"; // Date format for variable part of the TraceLogger directory name
        private const string TRACE_LOGGER_FILE_NAME_BASE = @"\ASCOM."; // Fixed part of the TraceLogger directory name.  Note: The trailing space must be retained!;
        private const string TRACE_LOGGER_FILE_NAME_TIME_FORMAT = "HHmm.ssfff"; // Date format for variable part of the TraceLogger directory name
        private const string TRACE_LOGGER_FILE_EXTENSION = ".txt"; // File extension to apply to log files

        private StreamWriter logFileStream;
        private readonly object writeLockObject = new object();
        private string logFileType;

        /// <summary>
        /// Write the log event
        /// </summary>
        /// <param name="logEvent"></param>
        public void Log(LogEvent logEvent)
        {
            if (string.IsNullOrWhiteSpace(logEvent.Message) && string.IsNullOrWhiteSpace(logEvent.Exception?.Message))
            {
                BlankLine();
            }
            else if (!string.IsNullOrWhiteSpace(logEvent.Message) && !string.IsNullOrWhiteSpace(logEvent.Exception?.Message))
            {
                LogMessage(logEvent.EventId, $"{logEvent.Message}, Exception : {logEvent.Exception.Message}", logEvent.PropertyValues.Select(o => o.ToString()).ToArray());
            }
            else if (!string.IsNullOrWhiteSpace(logEvent.Exception?.Message))
            {
                LogMessage(logEvent.EventId, logEvent.Exception.Message, logEvent.PropertyValues.Select(o => o.ToString()).ToArray());
            }
            else if (!string.IsNullOrWhiteSpace(logEvent.Message))
            {
                LogMessage(logEvent.EventId, logEvent.Message, logEvent.PropertyValues.Select(o => o.ToString()).ToArray());
            }
        }
        #region Initialiser and IDisposable Support

       
        
        /// <summary>
        /// Initialise the TraceLogger
        /// </summary>
        public TraceLogger(string LogFileType, bool enabled = false, string logFilePath = null)
        {
            IdentifierWidth = IDENTIFIER_WIDTH_DEFAULT;

            LogFileName = ""; // Set a default value
            logFileType = LogFileType; // Save the log file type
            Enabled = enabled;
            //TODO Think about Linux and MacOS
            LogFilePath = logFilePath ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + TRACE_LOGGER_PATH_BASE + TRACE_LOGGER_DIRECTORY_NAME_BASE + DateTime.Now.ToString(TRACE_LOGGER_DIRECTORY_NAME_DATE_FORMAT);
        }

        private bool traceLoggerHasBeenDisposed = false; // To detect redundant calls

        /// <summary>
        /// Disposes of the TraceLogger object
        /// </summary>
        /// <param name="disposing">True if being disposed by the application, False if being disposed by the finaliser.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!traceLoggerHasBeenDisposed)
            {
                Enabled = false; // Disable further writing to the log file, in case the trace logger is called after being disposed
                if (disposing)
                {
                    if (!(logFileStream == null)) // If we have a streamwriter, flush, close and dispose of it 
                    {
                        try { logFileStream.Flush(); } catch { }
                        try { logFileStream.Close(); } catch { }
                        try { logFileStream.Dispose(); } catch { }
                        logFileStream = null;
                    }
                }

                traceLoggerHasBeenDisposed = true; // Flag that Dispose() has been run
            }
        }

        /// <summary>
        /// Disposes of the TraceLogger object
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        
        #endregion

        /// <summary>
        /// Logs a message as supplied
        /// </summary>
        /// <param name="Message">Message to be logged</param>
        private void LogMessage(string Message)
        {
            if (Enabled & !traceLoggerHasBeenDisposed) // Only atempt to write the to the log file if the trace logger is enabled and has not been disposed
            {
                lock (writeLockObject) // Get a write lock so that this thread can write exclusively to the file
                {
                    if (logFileStream == null) CreateLogFile(); // Create the log file if it doesn't already exist
                    logFileStream.WriteLine($"{DateTime.Now.ToString(LOG_TIME_FORMAT)} {Message}"); // Write the supplied message to the log file without any formatting

                    logFileStream.Flush(); // Ensure that the log line is written to the file rather than just being in the buffer
                }
            }
        }

        /// <summary>
        /// Logs a message identifier and the message itself in two columns
        /// </summary>
        /// <param name="Identifier">Identifies the source of the message e.g. name of module or method logging the message.</param>
        /// <param name="Message">Message to log</param>
        /// <remarks>
        /// </remarks>
        private void LogMessage(string Identifier, string Message)
        {
            LogMessage($"{Identifier.PadRight(IdentifierWidth)} {Message}"); // Interpolated string containing formatted log message
        }

        /// <summary>
        /// Logs a message identifier  and the message with parameters substituted into placeholders within the message.
        /// </summary>
        /// <param name="Identifier">Identifies the source of the message e.g. name of module or method logging the message.</param>
        /// <param name="Message">Message to log, including {0}, {1}... type placeholders for parameter values</param>
        /// <param name="parameters">A comma separated list of parameter values to be merged into the message at places indicated by {0}, {1}... type placeholders.</param>
        private void LogMessage(string Identifier, string Message, params string[] parameters)
        {
            LogMessage($"{Identifier.PadRight(IdentifierWidth)} {string.Format(Message, parameters)}"); // Interpolated string containing formatted log message
        }

        /// <summary>
        /// Insert a blank line into the log file
        /// </summary>
        /// <remarks></remarks>
        private void BlankLine()
        {
            LogMessage("");
        }

        /// <summary>
        /// Enables or disables logging to the file.
        /// </summary>
        /// <value>True to enable logging</value>
        /// <returns>Boolean, current logging status (enabled/disabled).</returns>
        /// <remarks>If this property is false then calls to LogMsg, LogStart, LogContinue and LogFinish do nothing. If True, 
        /// supplied messages are written to the log file.</remarks>
        public bool Enabled { get; private set; }

        /// <summary>
        /// Return the full filename of the log file being created
        /// </summary>
        /// <value>Full filename of the log file</value>
        /// <returns>String filename</returns>
        /// <remarks>This call will return an empty string until the first line has been written to the log file
        /// as the file is not created until required.</remarks>
        public string LogFileName { get; private set; }

        /// <summary>
        /// Set or return the path to a directory in which the log file will be created
        /// </summary>
        /// <returns>String path</returns>
        /// <remarks>If set, this path will be used instead of the default path, which is the user's Documents directory. This must be Set before the first message Is logged, otherwise it will have no effect.</remarks>
        public string LogFilePath { get; private set; }

        /// <summary>
        /// Set or return the width of the identifier field in the log message
        /// </summary>
        /// <value>Width of the identifier field</value>
        /// <returns>Integer width</returns>
        /// <remarks>If set, this width will be used instead of the default identifier field width.</remarks>
        public int IdentifierWidth { get; set; }

        #region Private support code

        private void CreateLogFile()
        {
            int FileNameSuffix = 0;

            Directory.CreateDirectory(LogFilePath); // Create the directory in case it doesn't exist

            string FileNameBase = TRACE_LOGGER_FILE_NAME_BASE + logFileType + "." + DateTime.Now.ToString(TRACE_LOGGER_FILE_NAME_TIME_FORMAT); // Create a base for trace logger file names

            // Created a unique log file name that doesn't clash with any others
            do
            {
                LogFileName = LogFilePath + FileNameBase + FileNameSuffix.ToString() + TRACE_LOGGER_FILE_EXTENSION;
                FileNameSuffix += 1;
            }
            while (!(!File.Exists(LogFileName))); // Increment counter that ensures that no log file can have the same name as any other

            // Try to create the log file
            try
            {
                logFileStream = new StreamWriter(LogFileName, false);
                logFileStream.AutoFlush = true; // Creation was successful
            }
            catch (IOException ex) // Creation was not succcessful so keep incrementing the FileNameSuffix to find a unique file name. Give up and throw an exception if there is no success after trying 20 unique filenames
            {
                bool ok = false;
                do
                {
                    try
                    {
                        LogFileName = FileNameBase + FileNameSuffix.ToString() + TRACE_LOGGER_FILE_EXTENSION;
                        logFileStream = new StreamWriter(LogFileName, false);
                        logFileStream.AutoFlush = true;
                        ok = true;
                    }
                    catch { }

                    FileNameSuffix += 1;
                }
                while (!(ok | (FileNameSuffix == 20)));

                if (!ok) throw new Exception("TraceLogger:CreateLogFile - Unable to create log file", ex);
            }
        }

        #endregion
    }
}
