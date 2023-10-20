using ASCOM.Common;
using ASCOM.Common.Interfaces;
using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace ASCOM.Tools
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
    public class TraceLogger : IDisposable, ITraceLogger
    {
        // Configuration constants
        private const int IDENTIFIER_WIDTH_DEFAULT = 25;
        private const int MUTEX_WAIT_TIME = 5000;
        private const int MAXIMUM_UNIQUE_SUFFIX_ATTEMPTS = 20;

        // Path and file name constants for auto generated paths and file names
        private const string AUTO_FILE_NAME_TEMPLATE_WINDOWS = "ASCOM.{0}.{1:HHmm.ssfff}{2}.txt"; // Auto generated file name template
        private const string AUTO_FILE_NAME_TEMPLATE_LINUX = "ascom.{0}.{1:HHmm.ssfff}{2}.txt"; // Auto generated file name template
        private const string AUTO_PATH_BASE_DIRECTORY_WINDOWS = "ASCOM"; // Primary logging directory off the users's Documents (Windows) or HOME directory (Linux)
        private const string AUTO_PATH_BASE_DIRECTORY_LINUX = "ascom"; // Primary logging directory off the users's Documents (Windows) or HOME directory (Linux)
        private const string AUTO_PATH_WINDOWS_SYSTEM_USER_BASE_DIRECTORY = @"ASCOM\SystemLogs"; // Primary logging directory for the System account
        private const string AUTO_PATH_DIRECTORY_TEMPLATE_WINDOWS = "Logs {0:yyyy-MM-dd}"; // Sub directory template on Windows 
        private const string AUTO_PATH_DIRECTORY_TEMPLATE_LINUX = "logs{0:yyyy-MM-dd}"; // Sub directory template on Linux - lower case with no space between logs and date
        private const string ASCOM_LOGPATH = "ASCOM_LOGPATH";

        // Per user registry default log folder configuration values - These need to be kept in sync with Platform equivalents if changed!
        internal const string REGISTRY_UTILITIES_CONFGIGURATION_KEY = @"Software\ASCOM\Utilities";
        internal const string REGISTRY_DEFAULT_FOLDER_VALUE_NAME = "TraceLogger Default Folder";

        // Default value constants
        private const bool USE_UTC_DEFAULT = false;
        private const bool AUTO_GENERATE_FILENAME_DEFAULT = true;
        private const bool AUTO_GENERATE_FILEPATH__DEFAULT = true;
        private const bool RESPECT_CRLF_DEFAULT = true;

        // Property backing variables
        private readonly string logFileType;
        private int identifierWidthValue;

        // Global fields
        private StreamWriter logFileStream;
        private Mutex loggerMutex;
        private readonly bool autoGenerateFileName;
        private readonly bool autoGenerateFilePath;
        private bool traceLoggerHasBeenDisposed;
        private string mutexName;

        #region Initialise and Dispose

        /// <summary>
        /// Create a new TraceLogger instance with the given filename and path
        /// </summary>
        /// <param name="logFileName">Name of the log file (without path) or null / empty string to use automatic file naming.</param>
        /// <param name="logFilePath">Fully qualified path to the log file directory or null / empty string to use an automatically generated path.</param>
        /// <param name="logFileType">A short name to identify the contents of the log (only used in automatic file names).</param>
        /// <param name="enabled">Initial state of the trace logger - Enabled or Disabled.</param>
        /// <remarks>Automatically generated directory names will be of the form: <c>"Documents\ASCOM\Logs {CurrentDate:yyyymmdd}"</c> on Windows and <c>"HOME/ASCOM/Logs{CurrentDate:yyyymmdd}"</c> on Linux
        /// Automatically generated file names will be of the form: <c>"ASCOM.{LogFileType}.{CurrentTime:HHmm.ssfff}{1 or 2 Digits, usually 0}.txt"</c>.</remarks>
        public TraceLogger(string logFileName, string logFilePath, string logFileType, bool enabled)
        {
            if (string.IsNullOrEmpty(logFileType)) throw new InvalidValueException("TraceLogger Initialisation - Supplied log file type is null or empty");

            CommonInitialisation();

            LogFileName = logFileName;
            LogFilePath = logFilePath;
            this.logFileType = logFileType;
            Enabled = enabled;

            autoGenerateFileName = string.IsNullOrEmpty(LogFileName); // Flag auto file name generation if no file name is supplied
            autoGenerateFilePath = string.IsNullOrEmpty(LogFilePath); // Flag auto file path use if no path is supplied
        }

        /// <summary>
        /// Create a new TraceLogger instance with automatic naming incorporating the supplied log file type
        /// </summary>
        /// <param name="logFileType">A short name to identify the contents of the log.</param>
        /// <param name="enabled">Initial state of the trace logger - Enabled or Disabled.</param>
        /// <param name="identifierWidth">Width of the identifier field in the log message (Optional parameter, default: 25)</param>
        /// <param name="logLevel">Log level of the trace logger (Debug, Information, Warning etc.) (Optional parameter, default: LogLevel.Information)</param>
        /// <remarks>Automatically generated directory names will be of the form: <c>"Documents\ASCOM\Logs {CurrentDate:yyyymmdd}"</c> on Windows and <c>"HOME/ASCOM/Logs{CurrentDate:yyyymmdd}"</c> on Linux
        /// Automatically generated file names will be of the form: <c>"ASCOM.{LogFileType}.{CurrentTime:HHmm.ssfff}{1 or 2 Digits, usually 0}.txt"</c>.</remarks>
        public TraceLogger(string logFileType, bool enabled, int identifierWidth = IDENTIFIER_WIDTH_DEFAULT, LogLevel logLevel = LogLevel.Information)
        {
            // Validate the log file type
            if (string.IsNullOrEmpty(logFileType)) throw new InvalidValueException("TraceLogger Initialisation - Supplied log file type is null or empty");

            CommonInitialisation();

            LogFileName = "";
            LogFilePath = "";
            this.logFileType = logFileType;
            Enabled = enabled;
            identifierWidthValue = identifierWidth;

            SetMinimumLoggingLevel(logLevel);

            autoGenerateFileName = AUTO_GENERATE_FILENAME_DEFAULT;
            autoGenerateFilePath = AUTO_GENERATE_FILEPATH__DEFAULT;
        }

        /// <summary>
        /// Common code shared by all initialiser overloads
        /// </summary>
        private void CommonInitialisation()
        {
            traceLoggerHasBeenDisposed = false;
            identifierWidthValue = IDENTIFIER_WIDTH_DEFAULT;

            mutexName = Guid.NewGuid().ToString().ToUpper();
            loggerMutex = new Mutex(false, mutexName);

            UseUtcTime = USE_UTC_DEFAULT;
            RespectCrLf = RESPECT_CRLF_DEFAULT;
        }

        /// IDisposable
        /// <summary>
        /// Disposes of the TraceLogger object
        /// </summary>
        /// <param name="disposing">True if being disposed by the application, False if disposed by the finaliser.</param>
        /// <remarks></remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (traceLoggerHasBeenDisposed) return;

            if (disposing)
            {
                if (logFileStream != null)
                {
                    try
                    {
                        logFileStream.Flush();
                    }
                    catch
                    {
                    }
                    try
                    {
                        logFileStream.Close();
                    }
                    catch
                    {
                    }
                    try
                    {
                        logFileStream.Dispose();
                    }
                    catch
                    {
                    }

                    logFileStream = null;
                }
                if (loggerMutex != null)
                {
                    try
                    {
                        loggerMutex.Close();
                    }
                    catch
                    {
                    }
                    loggerMutex = null;
                }

                traceLoggerHasBeenDisposed = true;
            }
        }

        /// <summary>
        /// Disposes of the TraceLogger object
        /// </summary>
        /// <remarks></remarks>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region ITraceLogger implementation

        /// <summary>
        /// Write a message to the trace log
        /// </summary>
        /// <param name="identifier">Member name or function name.</param>
        /// <param name="message">Message text</param>
        /// <remarks>
        /// </remarks>
        public void LogMessage(string identifier, string message)
        {
            bool gotMutex;

            // Return immediately if the logger is not enabled
            if (!Enabled) return;

            // Ignore attempts to write to the logger after it is disposed
            if (traceLoggerHasBeenDisposed) return;

            // Get the trace logger mutex
            try
            {
                gotMutex = loggerMutex.WaitOne(MUTEX_WAIT_TIME, false);
            }
            catch (AbandonedMutexException ex)
            {
                throw new DriverException($"TraceLogger - Abandoned Mutex Exception for file {LogFileName}, in method {identifier}, when writing message: '{message}'. See inner exception for detail", ex);
            }

            if (!gotMutex)
            {
                throw new DriverException($"TraceLogger - Timed out waiting for TraceLogger mutex after {MUTEX_WAIT_TIME}ms in method {identifier}, when writing message: '{message}'");
            }

            // We have the mutex and can now persist the message
            try
            {
                // Create the log file if it doesn't yet exist
                if (logFileStream == null) CreateLogFile();

                // Right pad the identifier string to the required column width
                identifier = identifier.PadRight(identifierWidthValue);

                // Get a DateTime object in either local or UTC time as determined by configuration
                DateTime messageDateTime = DateTimeNow();

                // Write the message to the log file
                logFileStream.WriteLine($"{messageDateTime:HH:mm:ss.fff} {MakePrintable(identifier)} {MakePrintable(message)}");
                logFileStream.Flush(); // Flush to make absolutely sure that the data is persisted to disk and can't be lost in an application crash

                // Update the day on which the last message was written
            }
            catch (Exception ex)
            {
                throw new DriverException($"TraceLogger - Exception formatting message '{identifier}' - '{message}': {ex.Message}. See inner exception for details", ex);
            }
            finally
            {
                loggerMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Insert a blank line into the log file
        /// </summary>
        /// <remarks></remarks>
        public void BlankLine()
        {
            if (traceLoggerHasBeenDisposed) return;
            LogMessage("", "");
        }

        /// <summary>
        /// Enable or disable logging to the file.
        /// </summary>
        /// <value>True to enable logging</value>
        /// <returns>Boolean, current logging status (enabled/disabled).</returns>
        /// <remarks>If this property is False, calls to LogMessage do nothing. If True, messages are written to the log file.</remarks>
        public bool Enabled { get; set; }

        /// <summary>
        /// File name of the log file being created
        /// </summary>
        /// <value>Filename of the log file without the path.</value>
        /// <returns>String filename</returns>
        /// <remarks>This call will return an empty string until the first line has been written to the log file because the file is not created until required.</remarks>
        public string LogFileName { get; private set; }

        /// <summary>
        /// Path to the directory in which the log file will be created
        /// </summary>
        /// <returns>String path</returns>
        /// <remarks>This call will return an empty string until the first line has been written to the log file because the file is not created until required.</remarks>
        public string LogFilePath { get; private set; }

        /// <summary>
        /// Set or return the width of the identifier field in the log message
        /// </summary>
        /// <value>Width of the identifier field</value>
        /// <returns>Integer identifier width</returns>
        /// <exception cref="InvalidValueException">If the width is less than 0</exception>
        /// <remarks>Introduced with Platform 6.4.<para>If set, this width will be used instead of the default identifier field width.</para></remarks>
        public int IdentifierWidth
        {
            get
            {
                return identifierWidthValue;
            }
            set
            {
                if (value < 0) throw new InvalidValueException("IdentifierWidth", value.ToString(), "0", int.MaxValue.ToString("N0"));
                identifierWidthValue = value;
            }
        }

        /// <summary>
        /// Set True to use UTC time, set false to use Local time (default true)
        /// </summary>
        public bool UseUtcTime { get; set; }

        /// <summary>
        /// Set True to retain carriage return and line feed as control characters. Set false to translate these to [XX] format (default true)
        /// </summary>
        public bool RespectCrLf { get; set; }

        #endregion

        #region ILogger implementation

        /// <summary>
        /// Return the current log level.
        /// </summary>
        public LogLevel LoggingLevel
        {
            get;
            private set;
        } = LogLevel.Information;

        /// <summary>
        /// Write a message to the log.
        /// </summary>
        /// <param name="level">Logging level of this message.</param>
        /// <param name="message">Message text.</param>
        public void Log(LogLevel level, string message)
        {
            if (this.IsLevelActive(level))
            {
                LogMessage($"[{level}]", message);
            }
        }

        /// <summary>
        /// Set the minimum log level to display.
        /// </summary>
        /// <param name="level">Required logging level.</param>
        public void SetMinimumLoggingLevel(LogLevel level)
        {
            LoggingLevel = level;
        }

        #endregion

        #region Support code

        /// <summary>
        /// Create the stream writer that will write to the log file
        /// </summary>
        private void CreateLogFile()
        {
            int logFileSuffixInteger = 0; // Initialise suffix to 0

            // Establish the path to the log file, auto generating this if required
            try
            {
                if (autoGenerateFilePath) // We need to auto generate the file path
                {
                    // Set the default log file path
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) // We are running on a Windows OS
                    {
                        if (!string.IsNullOrEmpty(Environment.GetFolderPath(Environment.SpecialFolder.Personal))) // This is a normaL "User" account
                        {
                            // Create a fallback folder name within the Documents folder: Documents\ASCOM
                            string fallbackFolderName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), AUTO_PATH_BASE_DIRECTORY_WINDOWS);

                            // Get the user configured TraceLogger default folder name. Fall back to the Documents\ASCOM folder if no default folder has been set by the user
                            string folderName;
                            try
                            {
                                folderName = Registry.CurrentUser.CreateSubKey(REGISTRY_UTILITIES_CONFGIGURATION_KEY).GetValue(REGISTRY_DEFAULT_FOLDER_VALUE_NAME, fallbackFolderName).ToString();
                            }
                            catch
                            {
                                // Something went wrong so use the fallback folder name
                                folderName = fallbackFolderName;
                            }

                            // Set the default folder name variable that is used with the TraceLogger application
                            LogFilePath = Path.Combine(folderName, string.Format(AUTO_PATH_DIRECTORY_TEMPLATE_WINDOWS, DateTimeNow()));
                        }
                        else // This is the "System" account, which does not have a personal documents directory so put log files in the 
                        {
                            LogFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), AUTO_PATH_WINDOWS_SYSTEM_USER_BASE_DIRECTORY, string.Format(AUTO_PATH_DIRECTORY_TEMPLATE_WINDOWS, DateTimeNow()));
                        }
                    }
                    else // We are running on a non-Windows OS
                    {
                        // Define the fallback log file directory name as the user's home directory
                        string fallbackDirectoryName = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                        // Get the user specified log file directory name from the ASCOM_LOGPATH path environment variable
                        string directoryName = Environment.GetEnvironmentVariable(Environment.ExpandEnvironmentVariables(ASCOM_LOGPATH));

                       // Use the fallback directory if the environment variable is not set
                        if (directoryName is null)
                            directoryName = fallbackDirectoryName;

                        // Make sure that the directory name is valid, if not use the fall-back value
                        if (directoryName.IndexOfAny(Path.GetInvalidPathChars()) > 0)
                            directoryName = fallbackDirectoryName;

                        // Create the final log file directory path
                        LogFilePath = Path.Combine(directoryName, string.Format(AUTO_PATH_DIRECTORY_TEMPLATE_LINUX, DateTimeNow()));
                    }
                }
                else // We need to use the supplied log file path, which is already in the LogFilePath property
                {
                    // No action required
                }
            }
            catch (Exception ex)
            {
                throw new DriverException($"TraceLogger - Unable to find determine the log file path, IsWindows: {RuntimeInformation.IsOSPlatform(OSPlatform.Windows)}. {ex.Message}. See inner exception for details", ex);
            }

            // Create the directory if required
            try
            {
                Directory.CreateDirectory(LogFilePath);
            }
            catch (Exception ex)
            {
                throw new DriverException($"TraceLogger - Unable to create log file directory '{LogFilePath}': {ex.Message}. See inner exception for details", ex);
            }

            // Create the log file stream writer auto a file name if required
            if (autoGenerateFileName) // We need to auto-generate a file name ourselves
            {
                try
                {
                    // Create a unique log file name based on date, time and required name by incrementing an arbitrary final suffixed count value
                    do
                    {
                        LogFileName = string.Format(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? AUTO_FILE_NAME_TEMPLATE_WINDOWS : AUTO_FILE_NAME_TEMPLATE_LINUX, logFileType, DateTimeNow(), logFileSuffixInteger);
                        checked { ++logFileSuffixInteger; } // Increment the counter to ensure that no log file can have the same name as any other
                    }
                    while (File.Exists(Path.Combine(LogFilePath, LogFileName)) & (logFileSuffixInteger <= MAXIMUM_UNIQUE_SUFFIX_ATTEMPTS)); // Loop until the generated file name does not exist or we hit the maximum number of attempts

                    // Close any current file stream before creating a new one
                    if (!(logFileStream is null))
                    {
                        logFileStream.Close();
                        logFileStream.Dispose();
                    }

                    // Create the stream writer used to write to disk
                    logFileStream = new StreamWriter(Path.Combine(LogFilePath, LogFileName), false)
                    {
                        AutoFlush = true
                    };
                }
                catch (Exception ex)
                {
                    throw new DriverException($"TraceLogger - Unable to create auto-generated log file '{LogFileName}' in directory '{LogFilePath}': {ex.Message}. See inner exception for details", ex);
                }
            }
            else // We need to use the supplied file name
            {
                try
                {
                    // Close any current file stream before creating a new one
                    if (!(logFileStream is null))
                    {
                        logFileStream.Close();
                        logFileStream.Dispose();
                    }

                    // Create the stream writer used to write to disk
                    logFileStream = new StreamWriter(Path.Combine(LogFilePath, LogFileName), false)
                    {
                        AutoFlush = true
                    };
                }
                catch (Exception ex)
                {
                    throw new DriverException($"TraceLogger - Unable to create log file '{LogFileName}' in directory '{LogFilePath}': {ex.Message}. See inner exception for details", ex);
                }
            }
        }

        /// <summary>
        /// Translate control characters into printable versions 
        /// </summary>
        /// <param name="message">Message to be cleansed</param>
        /// <returns>Cleaned message string</returns>
        /// <remarks>Non printable ASCII characters 0::31 and 127 are translated to [XX] format where XX is a two digit hex code. 
        /// Characters 13 and 10 (carriage return and linefeed) are either translated or left as control characters according to the setting of the RespectCrLf property.
        /// The default is for these to be left as control characters so that exception stack dumps print properly.</remarks>
        private string MakePrintable(string message)
        {
            string printableMessage = "";
            int charNo;

            // Present any unprintable characters in [0xHH] format
            foreach (char c in message)
            {
                charNo = (int)c;
                switch (charNo)
                {
                    case int i when i == 10 && RespectCrLf: // Handle carriage return and line feed as "printable" characters if "Respect CrLf" is enabled
                    case int j when j == 13 && RespectCrLf:
                        {
                            printableMessage += c.ToString();
                            break;
                        }

                    case int i when (i >= 0 && i <= 31) || i == 127: // Represent "non-printable" characters as hex codes
                        {
                            printableMessage += "[" + charNo.ToString("X2") + "]";
                            break;
                        }

                    default: // Handle everything else as "printable" characters 
                        {
                            printableMessage += c.ToString();
                            break;
                        }
                }
            }
            return printableMessage; // Return the formatted printable message
        }

        /// <summary>
        /// Return a dateTime object reflecting Local or UTC time depending on the setting of the UseUtcTime property.
        /// </summary>
        /// <returns>DateTime object in either local or UTC time.</returns>
        private DateTime DateTimeNow()
        {
            if (UseUtcTime)
            {
                return DateTime.UtcNow;
            }
            else
            {
                return DateTime.Now;
            }
        }

        #endregion

    }
}