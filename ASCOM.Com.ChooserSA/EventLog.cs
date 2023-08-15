using System;
using System.Diagnostics;
using System.IO;

namespace ASCOM.Com
{
    static class EventLog
    {
        /// <summary>
        /// Add an event record to the ASCOM Windows event log
        /// </summary>
        /// <param name="Caller">Name of routine creating the event</param>
        /// <param name="Msg">Event message</param>
        /// <param name="Severity">Event severity</param>
        /// <param name="Id">Id number</param>
        /// <param name="Except">Initiating exception or Nothing</param>
        /// <remarks></remarks>
        internal static void LogEvent(string Caller, string Msg, EventLogEntryType Severity, GlobalConstants.EventLogErrors Id, string Except)
        {
            System.Diagnostics.EventLog ELog;
            string MsgTxt;

            // During Platform 6 RC testing a report was received showing that a failure in this code had caused a bad Profile migration
            // There was no problem with the migration code, the issue was caused by the event log code throwing an unexpected exception back to MigrateProfile
            // It is wrong that an error in logging code should cause a client process to fail, so this code has been 
            // made more robust and ultimately will swallow exceptions silently rather than throwing an unexpected exception back to the caller

            try
            {
                if (!System.Diagnostics.EventLog.SourceExists(GlobalConstants.EVENT_SOURCE)) // Create the event log if it doesn't exist
                {
                    System.Diagnostics.EventLog.CreateEventSource(GlobalConstants.EVENT_SOURCE, GlobalConstants.EVENTLOG_NAME);
                    ELog = new System.Diagnostics.EventLog(GlobalConstants.EVENTLOG_NAME, ".", GlobalConstants.EVENT_SOURCE); // Create a pointer to the event log
                    ELog.ModifyOverflowPolicy(OverflowAction.OverwriteAsNeeded, 0); // Force the policy to overwrite oldest
                    ELog.MaximumKilobytes = 1024L; // Set the maximum log size to 1024kb, the Win 7 minimum size
                    ELog.Close(); // Force the log file to be created by closing the log
                    ELog.Dispose();
                    ELog = null;

                    // MSDN documentation advises waiting before writing, first time to a newly created event log file but doesn't say how long...
                    // Waiting 3 seconds to allow the log to be created by the OS
                    System.Threading.Thread.Sleep(3000);

                    // Try and create the initial log message
                    ELog = new System.Diagnostics.EventLog(GlobalConstants.EVENTLOG_NAME, ".", GlobalConstants.EVENT_SOURCE); // Create a pointer to the event log
                    ELog.WriteEntry("Successfully created event log - Policy: " + ELog.OverflowAction.ToString() + ", Size: " + ELog.MaximumKilobytes + "kb", EventLogEntryType.Information, (int)GlobalConstants.EventLogErrors.EventLogCreated);
                    ELog.Close();
                    ELog.Dispose();
                }

                // Write the event to the log
                ELog = new System.Diagnostics.EventLog(GlobalConstants.EVENTLOG_NAME, ".", GlobalConstants.EVENT_SOURCE); // Create a pointer to the event log

                MsgTxt = Caller + " - " + Msg; // Format the message to be logged
                if (Except is not null)
                    MsgTxt += "\r\n" + Except;
                ELog.WriteEntry(MsgTxt, Severity, (int)Id); // Write the message to the error log

                ELog.Close();
                ELog.Dispose();
            }
            catch (System.ComponentModel.Win32Exception ex) // Special handling because these exceptions contain error codes we may want to know
            {
                try
                {
                    string TodaysDateTime = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss.fff");
                    string ErrorLog = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + GlobalConstants.EVENTLOG_ERRORS;
                    string MessageLog = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + GlobalConstants.EVENTLOG_MESSAGES;

                    // Write to backup event log message and error logs
                    File.AppendAllText(ErrorLog, TodaysDateTime + " ErrorCode: 0x" + ex.ErrorCode.ToString("X8") + " NativeErrorCode: 0x" + ex.NativeErrorCode.ToString("X8") + " " + ex.ToString() + "\r\n");
                    File.AppendAllText(MessageLog, TodaysDateTime + " " + Caller + " " + Msg + " " + Severity.ToString() + " " + Id.ToString() + " " + Except + "\r\n");
                }
                catch (Exception) // Ignore exceptions here, the PC seems to be in a catastrophic failure!
                {

                }
            }
            catch (Exception ex) // Catch all other exceptions
            {
                // Something bad happened when writing to the event log so try and log it in a log file on the file system
                try
                {
                    string TodaysDateTime = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss.fff");
                    string ErrorLog = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + GlobalConstants.EVENTLOG_ERRORS;
                    string MessageLog = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + GlobalConstants.EVENTLOG_MESSAGES;

                    // Write to backup eventlog message and error logs
                    File.AppendAllText(ErrorLog, TodaysDateTime + " " + ex.ToString() + "\r\n");
                    File.AppendAllText(MessageLog, TodaysDateTime + " " + Caller + " " + Msg + " " + Severity.ToString() + " " + Id.ToString() + " " + Except + "\r\n");
                }
                catch (Exception) // Ignore exceptions here, the PC seems to be in a catastrophic failure!
                {

                }
            }
        }
    }
}
