using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

#if NETCOREAPP
using System.Reflection.PortableExecutable;
#endif

using System.Runtime.InteropServices;
using ASCOM.Common;
using ASCOM.Common.Interfaces;
// These items are shared between the ASCOM.Com and ASCOM.Astrometry assemblies

using Microsoft.Win32;

namespace ASCOM.Com
{

    #region Registry Utility Code

    static class RegistryCommonCode
    {
        internal static bool GetBool(string p_Name, bool p_DefaultValue)
        {
            var l_Value = default(bool);
            RegistryKey m_HKCU, m_SettingsKey;

            m_HKCU = Registry.CurrentUser;
            m_HKCU.CreateSubKey(GlobalConstants.REGISTRY_UTILITIES_FOLDER);
            m_SettingsKey = m_HKCU.OpenSubKey(GlobalConstants.REGISTRY_UTILITIES_FOLDER, true);

            try
            {
                if (m_SettingsKey.GetValueKind(p_Name) == RegistryValueKind.String) // Value does exist
                {
                    l_Value = Convert.ToBoolean(m_SettingsKey.GetValue(p_Name));
                }
            }
            catch (IOException) // Value doesn't exist so create it
            {
                try
                {
                    SetName(p_Name, p_DefaultValue.ToString());
                    l_Value = p_DefaultValue;
                }
                catch (Exception) // Unable to create value so just return the default value
                {
                    l_Value = p_DefaultValue;
                }
            }
            catch (Exception)
            {
                l_Value = p_DefaultValue;
            }
            m_SettingsKey.Flush(); // Clean up registry keys
            m_SettingsKey.Close();
            m_SettingsKey = null;
            m_HKCU.Flush();
            m_HKCU.Close();
            m_HKCU = null;

            return l_Value;
        }

        internal static string GetString(string p_Name, string p_DefaultValue)
        {
            string l_Value = "";
            RegistryKey m_HKCU, m_SettingsKey;

            m_HKCU = Registry.CurrentUser;
            m_HKCU.CreateSubKey(GlobalConstants.REGISTRY_UTILITIES_FOLDER);
            m_SettingsKey = m_HKCU.OpenSubKey(GlobalConstants.REGISTRY_UTILITIES_FOLDER, true);

            try
            {
                if (m_SettingsKey.GetValueKind(p_Name) == RegistryValueKind.String) // Value does exist
                {
                    l_Value = m_SettingsKey.GetValue(p_Name).ToString();
                }
            }
            catch (IOException) // Value doesn't exist so create it
            {
                try
                {
                    SetName(p_Name, p_DefaultValue.ToString());
                    l_Value = p_DefaultValue;
                }
                catch (Exception) // Unable to create value so just return the default value
                {
                    l_Value = p_DefaultValue;
                }
            }
            catch (Exception)
            {
                l_Value = p_DefaultValue;
            }
            m_SettingsKey.Flush(); // Clean up registry keys
            m_SettingsKey.Close();
            m_SettingsKey = null;
            m_HKCU.Flush();
            m_HKCU.Close();
            m_HKCU = null;

            return l_Value;
        }

        internal static double GetDouble(RegistryKey p_Key, string p_Name, double p_DefaultValue)
        {
            var l_Value = default(double);
            RegistryKey m_HKCU, m_SettingsKey;

            m_HKCU = Registry.CurrentUser;
            m_HKCU.CreateSubKey(GlobalConstants.REGISTRY_UTILITIES_FOLDER);
            m_SettingsKey = m_HKCU.OpenSubKey(GlobalConstants.REGISTRY_UTILITIES_FOLDER, true);

            // LogMsg("GetDouble", GlobalVarsAndCode.MessageLevel.msgDebug, p_Name.ToString & " " & p_DefaultValue.ToString)
            try
            {
                if (p_Key.GetValueKind(p_Name) == RegistryValueKind.String) // Value does exist
                {
                    l_Value = Convert.ToDouble(p_Key.GetValue(p_Name));
                }
            }
            catch (IOException) // Value doesn't exist so create it
            {
                try
                {
                    SetName(p_Name, p_DefaultValue.ToString());
                    l_Value = p_DefaultValue;
                }
                catch (Exception) // Unable to create value so just return the default value
                {
                    l_Value = p_DefaultValue;
                }
            }
            catch (Exception)
            {
                l_Value = p_DefaultValue;
            }
            m_SettingsKey.Flush(); // Clean up registry keys
            m_SettingsKey.Close();
            m_SettingsKey = null;
            m_HKCU.Flush();
            m_HKCU.Close();
            m_HKCU = null;

            return l_Value;
        }

        internal static DateTime GetDate(string p_Name, DateTime p_DefaultValue)
        {
            var l_Value = default(DateTime);
            RegistryKey m_HKCU, m_SettingsKey;

            m_HKCU = Registry.CurrentUser;
            m_HKCU.CreateSubKey(GlobalConstants.REGISTRY_UTILITIES_FOLDER);
            m_SettingsKey = m_HKCU.OpenSubKey(GlobalConstants.REGISTRY_UTILITIES_FOLDER, true);

            try
            {
                if (m_SettingsKey.GetValueKind(p_Name) == RegistryValueKind.String) // Value does exist
                {
                    l_Value = Convert.ToDateTime(m_SettingsKey.GetValue(p_Name));
                }
            }
            catch (IOException) // Value doesn't exist so create it
            {
                try
                {
                    SetName(p_Name, p_DefaultValue.ToString());
                    l_Value = p_DefaultValue;
                }
                catch (Exception) // Unable to create value so just return the default value
                {
                    l_Value = p_DefaultValue;
                }
            }
            catch (Exception)
            {
                l_Value = p_DefaultValue;
            }
            m_SettingsKey.Flush(); // Clean up registry keys
            m_SettingsKey.Close();
            m_SettingsKey = null;
            m_HKCU.Flush();
            m_HKCU.Close();
            m_HKCU = null;

            return l_Value;
        }

        internal static void SetName(string p_Name, string p_Value)
        {
            RegistryKey m_HKCU, m_SettingsKey;

            m_HKCU = Registry.CurrentUser;
            m_HKCU.CreateSubKey(GlobalConstants.REGISTRY_UTILITIES_FOLDER);
            m_SettingsKey = m_HKCU.OpenSubKey(GlobalConstants.REGISTRY_UTILITIES_FOLDER, true);

            m_SettingsKey.SetValue(p_Name, p_Value.ToString(), RegistryValueKind.String);
            m_SettingsKey.Flush(); // Clean up registry keys
            m_SettingsKey.Close();
            m_SettingsKey = null;
            m_HKCU.Flush();
            m_HKCU.Close();
            m_HKCU = null;

        }

    }

    #endregion

    #region Windows event log code

    static class EventLogCode
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
            EventLog ELog;
            string MsgTxt;

            // During Platform 6 RC testing a report was received showing that a failure in this code had caused a bad Profile migration
            // There was no problem with the migration code, the issue was caused by the event log code throwing an unexpected exception back to MigrateProfile
            // It is wrong that an error in logging code should cause a client process to fail, so this code has been 
            // made more robust and ultimately will swallow exceptions silently rather than throwing an unexpected exception back to the caller

            try
            {
                if (!EventLog.SourceExists(GlobalConstants.EVENT_SOURCE)) // Create the event log if it doesn't exist
                {
                    EventLog.CreateEventSource(GlobalConstants.EVENT_SOURCE, GlobalConstants.EVENTLOG_NAME);
                    ELog = new EventLog(GlobalConstants.EVENTLOG_NAME, ".", GlobalConstants.EVENT_SOURCE); // Create a pointer to the event log
                    ELog.ModifyOverflowPolicy(OverflowAction.OverwriteAsNeeded, 0); // Force the policy to overwrite oldest
                    ELog.MaximumKilobytes = 1024L; // Set the maximum log size to 1024kb, the Win 7 minimum size
                    ELog.Close(); // Force the log file to be created by closing the log
                    ELog.Dispose();
                    ELog = null;

                    // MSDN documentation advises waiting before writing, first time to a newly created event log file but doesn't say how long...
                    // Waiting 3 seconds to allow the log to be created by the OS
                    System.Threading.Thread.Sleep(3000);

                    // Try and create the initial log message
                    ELog = new EventLog(GlobalConstants.EVENTLOG_NAME, ".", GlobalConstants.EVENT_SOURCE); // Create a pointer to the event log
                    ELog.WriteEntry("Successfully created event log - Policy: " + ELog.OverflowAction.ToString() + ", Size: " + ELog.MaximumKilobytes + "kb", EventLogEntryType.Information, (int)GlobalConstants.EventLogErrors.EventLogCreated);
                    ELog.Close();
                    ELog.Dispose();
                }

                // Write the event to the log
                ELog = new EventLog(GlobalConstants.EVENTLOG_NAME, ".", GlobalConstants.EVENT_SOURCE); // Create a pointer to the event log

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

                    // Write to backup eventlog message and error logs
                    File.AppendAllText(ErrorLog, TodaysDateTime + " ErrorCode: 0x" + ex.ErrorCode.ToString("X8") + " NativeErrorCode: 0x" + ex.NativeErrorCode.ToString("X8") + " " + ex.ToString() + "\r\n");
                    File.AppendAllText(MessageLog, TodaysDateTime + " " + Caller + " " + Msg + " " + Severity.ToString() + " " + Id.ToString() + " " + Except + "\r\n");
                }
                catch (Exception) // Ignore exceptions here, the PC seems to be in a catastrophic failure!
                {

                }
            }
            catch (Exception ex) // Catch all other exceptions
            {
                // Somthing bad happened when writing to the event log so try and log it in a log file on the file system
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

    #endregion

    #region Version Code

    static partial class VersionCode
    {
        internal enum Bitness
        {
            Bits32,
            Bits64,
            BitsMSIL,
            BitsUnknown
        }

        internal static Bitness ApplicationBits()
        {
            switch (IntPtr.Size)
            {
                case 4:
                    {
                        return Bitness.Bits32;
                    }
                case 8:
                    {
                        return Bitness.Bits64;
                    }

                default:
                    {
                        return Bitness.BitsUnknown;
                    }
            }
        }

        /// <summary>
        /// Determines whether the specified process is running under WOW64 i.e. is a 32bit application running on a 64bit OS.
        /// </summary>
        /// <param name="hProcess">A handle to the process. The handle must have the PROCESS_QUERY_INFORMATION or PROCESS_QUERY_LIMITED_INFORMATION access right. 
        /// For more information, see Process Security and Access Rights.Windows Server 2003 and Windows XP:  
        /// The handle must have the PROCESS_QUERY_INFORMATION access right.</param>
        /// <param name="wow64Process">A pointer to a value that is set to TRUE if the process is running under WOW64. If the process is running under 
        /// 32-bit Windows, the value is set to FALSE. If the process is a 64-bit application running under 64-bit Windows, the value is also set to FALSE.</param>
        /// <returns>If the function succeeds, the return value is a nonzero value. If the function fails, the return value is zero. To get extended 
        /// error information, call GetLastError.</returns>
        /// <remarks></remarks>
#if NET7_0_OR_GREATER
        [LibraryImport("Kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool IsWow64Process(IntPtr hProcess, [MarshalAs(UnmanagedType.Bool)] ref bool wow64Process);
#else
        [DllImport("Kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        private static extern bool IsWow64Process(IntPtr hProcess, ref bool wow64Process);
#endif
        /// <summary>
        /// Return a message when a driver is not compatible with the requested 32/64bit application type. Returns an empty string if the driver is compatible
        /// </summary>
        /// <param name="ProgID">ProgID of the driver to be assessed</param>
        /// <param name="RequiredBitness">Application bitness for which application compatibility should be tested</param>
        /// <param name="TL">Logging trace logger</param>
        /// <returns>String compatibility message or empty string if driver is fully compatible</returns>
        /// <remarks></remarks>
        internal static string DriverCompatibilityMessage(string ProgID, Bitness RequiredBitness, ILogger TL)
        {
            string DriverCompatibilityMessageRet = default;
            ReadPECharacteristics InProcServer = null;
            bool Registered64Bit;
            Bitness InprocServerBitness;
            RegistryKey RK, RKInprocServer32;
            string CLSID, InprocFilePath, CodeBase;
            RegistryKey RK32 = null;
            RegistryKey RK64 = null;
#if NETFRAMEWORK
            string AssemblyFullName;
            Assembly LoadedAssembly;
            PortableExecutableKinds peKind;
            ImageFileMachine machine;
            Module[] Modules;
#endif
            using (var ProfileStore = new RegistryAccess()) // Get access to the profile store
            {

                DriverCompatibilityMessageRet = ""; // Set default return value as OK
                TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     ProgID: " + ProgID + ", Bitness: " + RequiredBitness.ToString());
                // Parse the COM registry section to determine whether this ProgID is an in-process DLL server.
                // If it is then parse the executable to determine whether it is a 32bit only driver and gie a suitable message if it is
                // Picks up some COM registration issues as well as a by-product.
                if (RequiredBitness == Bitness.Bits64) // We have a 64bit application so check to see whether this is a 32bit only driver
                {
                    RK = Registry.ClassesRoot.OpenSubKey(ProgID + @"\CLSID", false); // Look in the 64bit section first
                    if (RK is not null) // ProgID is registered and has a CLSID!
                    {
                        CLSID = RK.GetValue("").ToString(); // Get the CLSID for this ProgID
                        RK.Close();

                        RK = Registry.ClassesRoot.OpenSubKey(@"CLSID\" + CLSID); // Check the 64bit registry section for this CLSID
                        if (RK is null) // We don't have an entry in the 64bit CLSID registry section so try the 32bit section
                        {
                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     No entry in the 64bit registry, checking the 32bit registry");
                            RK = Registry.ClassesRoot.OpenSubKey(@"Wow6432Node\CLSID\" + CLSID); // Check the 32bit registry section
                            Registered64Bit = false;
                        }
                        else
                        {
                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Found entry in the 64bit registry");
                            Registered64Bit = true;
                        }
                        if (RK is not null) // We have a CLSID entry so process it
                        {
                            RKInprocServer32 = RK.OpenSubKey("InprocServer32");
                            RK.Close();
                            if (RKInprocServer32 is not null) // This is an in process server so test for compatibility
                            {
                                InprocFilePath = RKInprocServer32.GetValue("", "").ToString(); // Get the file location from the default position
                                CodeBase = RKInprocServer32.GetValue("CodeBase", "").ToString(); // Get the codebase if present to override the default value
                                if (!string.IsNullOrEmpty(CodeBase))
                                    InprocFilePath = CodeBase;

                                if (InprocFilePath.Trim().ToUpperInvariant() == "MSCOREE.DLL") // We have an assembly, most likely in the GAC so get the actual file location of the assembly
                                {
#if NETFRAMEWORK
                                    // If this assembly is in the GAC, we should have an "Assembly" registry entry with the full assmbly name, 
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Found MSCOREE.DLL");

                                    AssemblyFullName = RKInprocServer32.GetValue("Assembly", "").ToString(); // Get the full name
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Found full name: " + AssemblyFullName);
                                    if (!string.IsNullOrEmpty(AssemblyFullName)) // We did get an assembly full name so now try and load it to the reflection only context
                                    {
                                        try
                                        {
                                            LoadedAssembly = Assembly.ReflectionOnlyLoad(AssemblyFullName);
                                            // OK that wen't well so we have an MSIL version!
                                            InprocFilePath = LoadedAssembly.CodeBase; // Get the codebase for testing below
                                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Found file path: " + InprocFilePath);
                                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Found full name: " + LoadedAssembly.FullName + " ");
                                            Modules = LoadedAssembly.GetLoadedModules();
                                            Modules[0].GetPEKind(out peKind, out machine);
                                            if ((peKind & PortableExecutableKinds.Required32Bit) != 0)
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Kind Required32bit");
                                            if ((peKind & PortableExecutableKinds.PE32Plus) != 0)
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Kind PE32Plus");
                                            if ((peKind & PortableExecutableKinds.ILOnly) != 0)
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Kind ILOnly");
                                            if ((peKind & PortableExecutableKinds.NotAPortableExecutableImage) != 0)
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Kind Not PE Executable");
                                        }

                                        catch (IOException ex)
                                        {
                                            // That failed so try to load an x86 version
                                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "Could not find file, trying x86 version - " + ex.Message);

                                            try
                                            {
                                                LoadedAssembly = Assembly.ReflectionOnlyLoad(AssemblyFullName + ", processorArchitecture=x86");
                                                // OK that wen't well so we have an x86 only version!
                                                InprocFilePath = LoadedAssembly.CodeBase; // Get the codebase for testing below
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX86", "     Found file path: " + InprocFilePath);
                                                Modules = LoadedAssembly.GetLoadedModules();
                                                Modules[0].GetPEKind(out peKind, out machine);
                                                if ((peKind & PortableExecutableKinds.Required32Bit) != 0)
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX86", "     Kind Required32bit");
                                                if ((peKind & PortableExecutableKinds.PE32Plus) != 0)
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX86", "     Kind PE32Plus");
                                                if ((peKind & PortableExecutableKinds.ILOnly) != 0)
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX86", "     Kind ILOnly");
                                                if ((peKind & PortableExecutableKinds.NotAPortableExecutableImage) != 0)
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX86", "     Kind Not PE Executable");
                                            }

                                            catch (IOException ex1)
                                            {
                                                // That failed so try to load an x64 version
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "Could not find file, trying x64 version - " + ex1.Message);

                                                try
                                                {
                                                    LoadedAssembly = Assembly.ReflectionOnlyLoad(AssemblyFullName + ", processorArchitecture=x64");
                                                    // OK that wen't well so we have an x64 only version!
                                                    InprocFilePath = LoadedAssembly.CodeBase; // Get the codebase for testing below
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "     Found file path: " + InprocFilePath);
                                                    Modules = LoadedAssembly.GetLoadedModules();
                                                    Modules[0].GetPEKind(out peKind, out machine);
                                                    if ((peKind & PortableExecutableKinds.Required32Bit) != 0)
                                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "     Kind Required32bit");
                                                    if ((peKind & PortableExecutableKinds.PE32Plus) != 0)
                                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "     Kind PE32Plus");
                                                    if ((peKind & PortableExecutableKinds.ILOnly) != 0)
                                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "     Kind ILOnly");
                                                    if ((peKind & PortableExecutableKinds.NotAPortableExecutableImage) != 0)
                                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "     Kind Not PE Executable");
                                                }

                                                catch (Exception ex2)
                                                {
                                                    // Ignore exceptions here and leave MSCOREE.DLL as the InprocFilePath, this will fail below and generate an "incompatible driver" message
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", ex2.ToString());
                                                }
                                            }

                                            catch (Exception ex1)
                                            {
                                                // Ignore exceptions here and leave MSCOREE.DLL as the InprocFilePath, this will fail below and generate an "incompatible driver" message
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX32", ex1.ToString());
                                            }
                                        }

                                        catch (Exception ex)
                                        {
                                            // Ignore exceptions here and leave MSCOREE.DLL as the InprocFilePath, this will fail below and generate an "incompatible driver" message
                                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", ex.ToString());
                                        }
                                    }
                                    else
                                    {
                                        // No Assembly entry so we can't load the assembly, we'll just have to take a chance!
                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "'AssemblyFullName is null so we can't load the assembly, we'll just have to take a chance!");
                                        InprocFilePath = ""; // Set to null to bypass tests
                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Set InprocFilePath to null string");
                                    }
#else
                                    // This is .NET Core so we can't load the assembly, we'll just have to take a chance!
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "'Running .NET Core so we can't reflection load the assembly, we'll just have to take a chance!");
                                    InprocFilePath = ""; // Set to null to bypass tests
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Set InprocFilePath to null string");
#endif
                                }

                                if (InprocFilePath.Trim().Right(4).ToUpperInvariant() == ".DLL") // We have a path to the server and it is a dll
                                {
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Found DLL driver");

                                    // We have an assembly or other technology DLL, outside the GAC, in the file system
                                    try
                                    {
                                        InProcServer = new ReadPECharacteristics(InprocFilePath, TL); // Get hold of the executable so we can determine its characteristics
                                        InprocServerBitness = InProcServer.BitNess;
                                        if (InprocServerBitness == Bitness.Bits32) // 32bit driver executable
                                        {
                                            if (Registered64Bit) // 32bit driver executable registered in 64bit COM
                                            {
                                                DriverCompatibilityMessageRet = "This 32bit only driver won't work in a 64bit application even though it is registered as a 64bit COM driver." + "\r\n" + GlobalConstants.DRIVER_AUTHOR_MESSAGE_DRIVER;
                                            }
                                            else // 32bit driver executable registered in 32bit COM
                                            {
                                                DriverCompatibilityMessageRet = "This 32bit only driver won't work in a 64bit application even though it is correctly registered as a 32bit COM driver." + "\r\n" + GlobalConstants.DRIVER_AUTHOR_MESSAGE_DRIVER;
                                            }
                                        }
                                        else if (Registered64Bit) // 64bit driver
                                                                  // 64bit driver executable registered in 64bit COM section
                                        {
                                        }
                                        // This is the only OK combination, no message for this!
                                        else // 64bit driver executable registered in 32bit COM
                                        {
                                            DriverCompatibilityMessageRet = "This 64bit capable driver is only registered as a 32bit COM driver." + "\r\n" + GlobalConstants.DRIVER_AUTHOR_MESSAGE_INSTALLER;
                                        }
                                    }
                                    catch (FileNotFoundException) // Cannot open the file
                                    {
                                        DriverCompatibilityMessageRet = "Cannot find the driver executable: " + "\r\n" + "\"" + InprocFilePath + "\"";
                                    }
                                    catch (Exception ex) // Some other exception so log it
                                    {
                                        EventLogCode.LogEvent("DriverCompatibilityMessage", "Exception parsing " + ProgID + ", \"" + InprocFilePath + "\"", EventLogEntryType.Error, GlobalConstants.EventLogErrors.DriverCompatibilityException, ex.ToString());
                                        DriverCompatibilityMessageRet = "PEReader Exception, please check ASCOM application Event Log for details";
                                    }

                                    if (InProcServer is not null) // Clean up the PEReader class
                                    {
                                        InProcServer.Dispose();
                                        InProcServer = null;
                                    }
                                }
                                else
                                {
                                    // No codebase so can't test this driver, don't give an error message, just have to take a chance!
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "No codebase so can't test this driver, don't give an error message, just have to take a chance!");
                                }
                                RKInprocServer32.Close(); // Clean up the InProcServer registry key
                            }
                            else
                            {
                                // Please leave this empty clause here so the logic is clear!
                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Driver is not DLL");

                            } // This is not an inprocess DLL so no need to test further and no error message to return
                        }
                        else // Cannot find a CLSID entry
                        {
                            DriverCompatibilityMessageRet = "Unable to find a CLSID entry for this driver, please re-install.";
                        }
                    }
                    else // No COM ProgID registry entry
                    {
                        DriverCompatibilityMessageRet = "This driver is not registered for COM (can't find ProgID), please re-install.";
                    }
                }
                else // We are running a 32bit application test so make sure the executable is not 64bit only
                {
                    RK = Registry.ClassesRoot.OpenSubKey(ProgID + @"\CLSID", false); // Look in the 32bit registry

                    if (RK is not null) // ProgID is registered and has a CLSID!
                    {
                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Found 32bit ProgID registration");
                        CLSID = RK.GetValue("").ToString(); // Get the CLSID for this ProgID
                        RK.Close();
                        RK = null;

                        if (Environment.Is64BitOperatingSystem) // We want to test as if we are a 32bit app on a 64bit OS
                        {
                            try
                            {
                                RK32 = RegistryAccess.OpenSubKey3264(RegistryHive.ClassesRoot, @"CLSID\" + CLSID, false, RegistryView.Registry32);
                            }
                            catch (Exception) // Ignore any exceptions, they just mean the operation wasn't successful
                            {
                            }

                            try
                            {
                                RK64 = RegistryAccess.OpenSubKey3264(RegistryHive.ClassesRoot, @"CLSID\" + CLSID, false, RegistryView.Registry64);
                            }
                            catch (Exception) // Ignore any exceptions, they just mean the operation wasn't successful
                            {
                            }
                        }

                        else // We are running on a 32bit OS
                        {
                            RK = Registry.ClassesRoot.OpenSubKey(@"CLSID\" + CLSID); // Check the 32bit registry section for this CLSID
                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Running on a 32bit OS, 32Bit Registered: " + (RK is not null));
                        }

                        if (Environment.Is64BitOperatingSystem)
                        {
                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Running on a 64bit OS, 32bit Registered: " + (RK32 is not null) + ", 64Bit Registered: " + (RK64 is not null));
                            if (RK32 is not null) // We are testing as a 32bit app so if there is a 32bit key return this
                            {
                                RK = RK32;
                            }
                            else // Otherwise return the 64bit key
                            {
                                RK = RK64;
                            }
                        }

                        if (RK is not null) // We have a CLSID entry so process it
                        {
                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Found CLSID entry");
                            RKInprocServer32 = RK.OpenSubKey("InprocServer32");
                            RK.Close();
                            if (RKInprocServer32 is not null) // This is an in process server so test for compatibility
                            {
                                InprocFilePath = RKInprocServer32.GetValue("", "").ToString(); // Get the file location from the default position
                                CodeBase = RKInprocServer32.GetValue("CodeBase", "").ToString(); // Get the codebase if present to override the default value
                                if (!string.IsNullOrEmpty(CodeBase))
                                    InprocFilePath = CodeBase;

                                if (InprocFilePath.Trim().ToUpperInvariant() == "MSCOREE.DLL") // We have an assembly, most likely in the GAC so get the actual file location of the assembly
                                {
#if NETFRAMEWORK
                                    // If this assembly is in the GAC, we should have an "Assembly" registry entry with the full assmbly name, 
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Found MSCOREE.DLL");

                                    AssemblyFullName = RKInprocServer32.GetValue("Assembly", "").ToString(); // Get the full name
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Found full name: " + AssemblyFullName);
                                    if (!string.IsNullOrEmpty(AssemblyFullName)) // We did get an assembly full name so now try and load it to the reflection only context
                                    {
                                        try
                                        {
                                            LoadedAssembly = Assembly.ReflectionOnlyLoad(AssemblyFullName);
                                            // OK that wen't well so we have an MSIL version!
                                            InprocFilePath = LoadedAssembly.CodeBase; // Get the codebase for testing below
                                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Found file path: " + InprocFilePath);
                                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Found full name: " + LoadedAssembly.FullName + " ");
                                            Modules = LoadedAssembly.GetLoadedModules();
                                            Modules[0].GetPEKind(out peKind, out machine);
                                            if ((peKind & PortableExecutableKinds.Required32Bit) != 0)
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Kind Required32bit");
                                            if ((peKind & PortableExecutableKinds.PE32Plus) != 0)
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Kind PE32Plus");
                                            if ((peKind & PortableExecutableKinds.ILOnly) != 0)
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Kind ILOnly");
                                            if ((peKind & PortableExecutableKinds.NotAPortableExecutableImage) != 0)
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Kind Not PE Executable");
                                        }

                                        catch (IOException ex)
                                        {
                                            // That failed so try to load an x86 version
                                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "Could not find file, trying x86 version - " + ex.Message);

                                            try
                                            {
                                                LoadedAssembly = Assembly.ReflectionOnlyLoad(AssemblyFullName + ", processorArchitecture=x86");
                                                // OK that wen't well so we have an x86 only version!
                                                InprocFilePath = LoadedAssembly.CodeBase; // Get the codebase for testing below
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX86", "     Found file path: " + InprocFilePath);
                                                Modules = LoadedAssembly.GetLoadedModules();
                                                Modules[0].GetPEKind(out peKind, out machine);
                                                if ((peKind & PortableExecutableKinds.Required32Bit) != 0)
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX86", "     Kind Required32bit");
                                                if ((peKind & PortableExecutableKinds.PE32Plus) != 0)
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX86", "     Kind PE32Plus");
                                                if ((peKind & PortableExecutableKinds.ILOnly) != 0)
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX86", "     Kind ILOnly");
                                                if ((peKind & PortableExecutableKinds.NotAPortableExecutableImage) != 0)
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX86", "     Kind Not PE Executable");
                                            }

                                            catch (IOException ex1)
                                            {
                                                // That failed so try to load an x64 version
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "Could not find file, trying x64 version - " + ex.Message);

                                                try
                                                {
                                                    LoadedAssembly = Assembly.ReflectionOnlyLoad(AssemblyFullName + ", processorArchitecture=x64");
                                                    // OK that wen't well so we have an x64 only version!
                                                    InprocFilePath = LoadedAssembly.CodeBase; // Get the codebase for testing below
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "     Found file path: " + InprocFilePath);
                                                    Modules = LoadedAssembly.GetLoadedModules();
                                                    Modules[0].GetPEKind(out peKind, out machine);
                                                    if ((peKind & PortableExecutableKinds.Required32Bit) != 0)
                                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "     Kind Required32bit");
                                                    if ((peKind & PortableExecutableKinds.PE32Plus) != 0)
                                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "     Kind PE32Plus");
                                                    if ((peKind & PortableExecutableKinds.ILOnly) != 0)
                                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "     Kind ILOnly");
                                                    if ((peKind & PortableExecutableKinds.NotAPortableExecutableImage) != 0)
                                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "     Kind Not PE Executable");
                                                }

                                                catch (Exception)
                                                {
                                                    // Ignore exceptions here and leave MSCOREE.DLL as the InprocFilePath, this will fail below and generate an "incompatible driver" message
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", ex1.ToString());
                                                }
                                            }

                                            catch (Exception ex1)
                                            {
                                                // Ignore exceptions here and leave MSCOREE.DLL as the InprocFilePath, this will fail below and generate an "incompatible driver" message
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX32", ex1.ToString());
                                            }
                                        }

                                        catch (Exception ex)
                                        {
                                            // Ignore exceptions here and leave MSCOREE.DLL as the InprocFilePath, this will fail below and generate an "incompatible driver" message
                                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", ex.ToString());
                                        }
                                    }
                                    else
                                    {
                                        // No Assembly entry so we can't load the assembly, we'll just have to take a chance!
                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "'AssemblyFullName is null so we can't load the assembly, we'll just have to take a chance!");
                                        InprocFilePath = ""; // Set to null to bypass tests
                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Set InprocFilePath to null string");
                                    }
#else
                                    // This is .NET Core so we can't load the assembly, we'll just have to take a chance!
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "'Running .NET Core so we can't reflection load the assembly, we'll just have to take a chance!");
                                    InprocFilePath = ""; // Set to null to bypass tests
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Set InprocFilePath to null string");
#endif
                                }

                                if (InprocFilePath.Trim().Right(4).ToUpperInvariant() == ".DLL") // We do have a path to the server and it is a dll
                                {
                                    // We have an assembly or other technology DLL, outside the GAC, in the file system
                                    try
                                    {
                                        InProcServer = new ReadPECharacteristics(InprocFilePath, TL); // Get hold of the executable so we can determine its characteristics
                                        if (InProcServer.BitNess == Bitness.Bits64) // 64bit only driver executable
                                        {
                                            DriverCompatibilityMessageRet = "This is a 64bit only driver and is not compatible with this 32bit application." + "\r\n" + GlobalConstants.DRIVER_AUTHOR_MESSAGE_DRIVER;
                                        }
                                    }
                                    catch (FileNotFoundException) // Cannot open the file
                                    {
                                        DriverCompatibilityMessageRet = "Cannot find the driver executable: " + "\r\n" + "\"" + InprocFilePath + "\"";
                                    }
                                    catch (Exception ex) // Some other exception so log it
                                    {
                                        EventLogCode.LogEvent("DriverCompatibilityMessage", "Exception parsing " + ProgID + ", \"" + InprocFilePath + "\"", EventLogEntryType.Error, GlobalConstants.EventLogErrors.DriverCompatibilityException, ex.ToString());
                                        DriverCompatibilityMessageRet = "PEReader Exception, please check ASCOM application Event Log for details";
                                    }

                                    if (InProcServer is not null) // Clean up the PEReader class
                                    {
                                        InProcServer.Dispose();
                                        InProcServer = null;
                                    }
                                }
                                else
                                {
                                    // No codebase or not a DLL so can't test this driver, don't give an error message, just have to take a chance!
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "No codebase or not a DLL so can't test this driver, don't give an error message, just have to take a chance!");
                                }
                                RKInprocServer32.Close(); // Clean up the InProcServer registry key
                            }
                            else // This is not an inprocess DLL so no need to test further and no error message to return
                            {
                                // Please leave this empty clause here so the logic is clear!
                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "This is not an inprocess DLL so no need to test further and no error message to return");
                            }
                        }
                        else // Cannot find a CLSID entry
                        {
                            DriverCompatibilityMessageRet = "Unable to find a CLSID entry for this driver, please re-install.";
                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Could not find CLSID entry!");
                        }
                    }
                    else // No COM ProgID registry entry
                    {
                        DriverCompatibilityMessageRet = "This driver is not registered for COM (can't find ProgID), please re-install.";
                    }

                }

            }
            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Returning: \"" + DriverCompatibilityMessageRet + "\"");
            return DriverCompatibilityMessageRet;
        }

    }

    #endregion

    #region PeReader


    internal class ReadPECharacteristics : IDisposable
    {
#if NETCOREAPP // .NET Core build
        private bool disposedValue;
        private readonly ILogger TL;
        private readonly VersionCode.Bitness ExecutableBitness;
        private readonly bool OS32BitCompatible = false;

        public ReadPECharacteristics() { }

        internal ReadPECharacteristics(string FileName, ILogger TLogger)
        {
            TL = TLogger; // Save the TraceLogger instance we have been passed

            TL?.LogMessage(LogLevel.Debug, "PEReader", "Running within CLR version: " + RuntimeEnvironment.GetSystemVersion());

            if (FileName.Substring(0, 5).ToUpperInvariant() == "FILE:")
            {
                // Convert Uri to local path if required, Uri paths are not supported by FileStream - this method allows file names with # characters to be passed through
                var u = new Uri(FileName);
                FileName = u.LocalPath + Uri.UnescapeDataString(u.Fragment).Replace("/", @"\\");
            }
            TL?.LogMessage(LogLevel.Debug, "PEReader", "Filename to check: " + FileName);
            if (!File.Exists(FileName))
                throw new FileNotFoundException("PEReader - File not found: " + FileName);

            // Determine whether this is an assembly by testing whether we can load the file as an assembly, if so then it IS an assembly!
            TL?.LogMessage(LogLevel.Debug, "PEReader", "Determining whether this is an assembly");

            try
            {
                TL?.LogMessage(LogLevel.Debug, "PEReader", "Determining PE Machine type");
                FileStream stream = new(FileName, FileMode.Open, FileAccess.Read);
                BinaryReader reader = new(stream);

                PEReader pEReader = new(stream, PEStreamOptions.PrefetchMetadata);

                PEHeaders pEHeaders = pEReader.PEHeaders;


                // Determine whether this executable is flagged as a 32bit or 64bit and set OS32BitCompatible accordingly
                switch (pEHeaders.CoffHeader.Machine)
                {
                    case Machine.I386:
                        {
                            OS32BitCompatible = true;
                            TL?.LogMessage(LogLevel.Debug, "PEReader.MachineType", "Machine - found \"Intel 32bit\" executable. OS32BitCompatible: " + OS32BitCompatible);
                            break;
                        }
                    case Machine.IA64:
                        {
                            OS32BitCompatible = false;
                            TL?.LogMessage(LogLevel.Debug, "PEReader.MachineType", "Machine - found \"Itanium 64bit\" executable. OS32BitCompatible: " + OS32BitCompatible);
                            break;
                        }
                    case Machine.Amd64:
                        {
                            OS32BitCompatible = false;
                            TL?.LogMessage(LogLevel.Debug, "PEReader.MachineType", "Machine - found \"Intel 64bit\" executable. OS32BitCompatible: " + OS32BitCompatible);
                            break;
                        }

                    default:
                        {
                            TL?.LogMessage(LogLevel.Debug, "PEReader.MachineType", "Found Unknown machine type: " + pEHeaders.CoffHeader.Machine.ToString("X8") + ". OS32BitCompatible: " + OS32BitCompatible);
                            break;
                        }
                }



                Characteristics characteristics = pEHeaders.CoffHeader.Characteristics;

                characteristics.HasFlag(Characteristics.Bit32Machine);


                bool hasMetaData = pEReader.HasMetadata;



                CorHeader corHeader = pEHeaders.CorHeader;
                if (corHeader is not null)
                {
                    pEHeaders.CorHeader.Flags.HasFlag(CorFlags.Requires32Bit);

                    if (OS32BitCompatible) // Could be an x86 or MSIL assembly so determine which
                    {
                        if (corHeader.Flags.HasFlag(CorFlags.Requires32Bit))
                        {
                            TL?.LogMessage(LogLevel.Debug, "PEReader.Bitness", "Found \"32bit Required\" assembly");
                            ExecutableBitness = VersionCode.Bitness.Bits32;
                        }
                        else
                        {
                            TL?.LogMessage(LogLevel.Debug, "PEReader.Bitness", "Found \"MSIL\" assembly");
                            ExecutableBitness = VersionCode.Bitness.BitsMSIL;
                        }
                    }
                    else // Must be an x64 assembly
                    {
                        TL?.LogMessage(LogLevel.Debug, "PEReader.Bitness", "Found \"64bit Required\" assembly");
                        ExecutableBitness = VersionCode.Bitness.Bits64;
                    }
                    TL?.LogMessage(LogLevel.Debug, "PEReader", "Assembly required Runtime version: " + corHeader.MajorRuntimeVersion + "." + corHeader.MinorRuntimeVersion);


                }
                else // Not an assembly so just use the FileHeader.Machine value to determine bitness
                {
                    TL?.LogMessage(LogLevel.Debug, "PEReader", "This is not an assembly, determining Bitness through the executable bitness flag");
                    if (OS32BitCompatible)
                    {
                        TL?.LogMessage(LogLevel.Debug, "PEReader.Bitness", "Found 32bit executable");
                        ExecutableBitness = VersionCode.Bitness.Bits32;
                    }
                    else
                    {
                        TL?.LogMessage(LogLevel.Debug, "PEReader.Bitness", "Found 64bit executable");
                        ExecutableBitness = VersionCode.Bitness.Bits64;
                    }

                }



            }
            catch (Exception)
            {
            }

        }



        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }


        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


        internal VersionCode.Bitness BitNess
        {
            get
            {
                TL?.LogMessage(LogLevel.Debug, "PE.BitNess", "Returning: " + ((int)ExecutableBitness).ToString());
                return ExecutableBitness;
            }
        }

#else // .NET Framework build

        #region Constants
        internal const int CLR_HEADER = 14; // Header number of the CLR information, if present
        private const int MAX_HEADERS_TO_CHECK = 1000; // Safety limit to ensure that we don't lock up the machine if we get a PE image that indicates it has a huge number of header directories

        // Possible error codes when an assembly is loaded for reflection
        private const int COR_E_BADIMAGEFORMAT = int.MinValue + 0x0007000B;
        private const int CLDB_E_FILE_OLDVER = int.MinValue + 0x00131107;
        private const int CLDB_E_INDEX_NOTFOUND = int.MinValue + 0x00131124;
        private const int CLDB_E_FILE_CORRUPT = int.MinValue + 0x0013110E;
        private const int COR_E_NEWER_RUNTIME = int.MinValue + 0x0013101B;
        private const int COR_E_ASSEMBLYEXPECTED = int.MinValue + 0x00131018;
        private const int ERROR_BAD_EXE_FORMAT = int.MinValue + 0x000700C1;
        private const int ERROR_EXE_MARKED_INVALID = int.MinValue + 0x000700C0;
        private const int CORSEC_E_INVALID_IMAGE_FORMAT = int.MinValue + 0x0013141D;
        private const int ERROR_NOACCESS = int.MinValue + 0x000703E6;
        private const int ERROR_INVALID_ORDINAL = int.MinValue + 0x000700B6;
        private const int ERROR_INVALID_DLL = int.MinValue + 0x00070482;
        private const int ERROR_FILE_CORRUPT = int.MinValue + 0x00070570;
        private const int COR_E_LOADING_REFERENCE_ASSEMBLY = int.MinValue + 0x00131058;
        private const int META_E_BAD_SIGNATURE = int.MinValue + 0x00131192;

        // Executable machine types
        private const ushort IMAGE_FILE_MACHINE_I386 = 0x14C; // x86
        private const ushort IMAGE_FILE_MACHINE_IA64 = 0x200; // Intel(Itanium)
        private const ushort IMAGE_FILE_MACHINE_AMD64 = 0x8664; // x64

        #endregion

        #region Enums
        internal enum CLR_FLAGSType
        {
            CLR_FLAGS_ILONLY = 0x1,
            CLR_FLAGS_32BITREQUIRED = 0x2,
            CLR_FLAGS_IL_LIBRARY = 0x4,
            CLR_FLAGS_STRONGNAMESIGNED = 0x8,
            CLR_FLAGS_NATIVE_ENTRYPOINT = 0x10,
            CLR_FLAGS_TRACKDEBUGDATA = 0x10000
        }

        internal enum SubSystemType
        {
            NATIVE = 1, // The binary doesn't need a subsystem. This is used for drivers.
            WINDOWS_GUI = 2, // The image is a Win32 graphical binary. (It can still open a console with AllocConsole() but won't get one automatically at startup.)
            WINDOWS_CUI = 3, // The binary is a Win32 console binary. (It will get a console per default at startup, or inherit the parent's console.)
            UNKNOWN_4 = 4, // Unknown allocation
            OS2_CUI = 5, // The binary is a OS/2 console binary. (OS/2 binaries will be in OS/2 format, so this value will seldom be used in a PE file.)
            UNKNOWN_6 = 6, // Unknown allocation
            POSIX_CUI = 7, // The binary uses the POSIX console subsystem.
            NATIVE_WINDOWS = 8,
            WINDOWS_CE_GUI = 9,
            EFI_APPLICATION = 10, // Extensible Firmware Interface (EFI) application.
            EFI_BOOT_SERVICE_DRIVER = 11, // EFI driver with boot services.
            EFI_RUNTIME_DRIVER = 12, // EFI driver with run-time services.
            EFI_ROM = 13, // EFI ROM image.
            XBOX = 14, // Xbox sy stem.
            UNKNOWN_15 = 15, // Unknown allocation
            WINDOWS_BOOT_APPLICATION = 16 // Boot application.
        }
        #endregion

        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        internal struct IMAGE_DOS_HEADER
        {
            internal ushort e_magic; // Magic number
            internal ushort e_cblp; // Bytes on last page of file
            internal ushort e_cp; // Pages in file
            internal ushort e_crlc; // Relocations
            internal ushort e_cparhdr; // Size of header in paragraphs
            internal ushort e_minalloc; // Minimum extra paragraphs needed
            internal ushort e_maxalloc; // Maximum extra paragraphs needed
            internal ushort e_ss; // Initial (relative) SS value
            internal ushort e_sp; // Initial SP value
            internal ushort e_csum; // Checksum
            internal ushort e_ip; // Initial IP value
            internal ushort e_cs; // Initial (relative) CS value
            internal ushort e_lfarlc; // File address of relocation table
            internal ushort e_ovno; // Overlay number
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            internal ushort[] e_res1; // Reserved words
            internal ushort e_oemid; // OEM identifier (for e_oeminfo)
            internal ushort e_oeminfo; // 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            internal ushort[] e_res2; // Reserved words
            internal uint e_lfanew; // File address of new exe header
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct IMAGE_NT_HEADERS
        {
            internal uint Signature;
            internal IMAGE_FILE_HEADER FileHeader;
            internal IMAGE_OPTIONAL_HEADER32 OptionalHeader32;
            internal IMAGE_OPTIONAL_HEADER64 OptionalHeader64;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct IMAGE_FILE_HEADER
        {
            internal ushort Machine;
            internal ushort NumberOfSections;
            internal uint TimeDateStamp;
            internal uint PointerToSymbolTable;
            internal uint NumberOfSymbols;
            internal ushort SizeOfOptionalHeader;
            internal ushort Characteristics;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct IMAGE_OPTIONAL_HEADER32
        {
            internal ushort Magic;
            internal byte MajorLinkerVersion;
            internal byte MinorLinkerVersion;
            internal uint SizeOfCode;
            internal uint SizeOfInitializedData;
            internal uint SizeOfUninitializedData;
            internal uint AddressOfEntryPoint;
            internal uint BaseOfCode;
            internal uint BaseOfData;
            internal uint ImageBase;
            internal uint SectionAlignment;
            internal uint FileAlignment;
            internal ushort MajorOperatingSystemVersion;
            internal ushort MinorOperatingSystemVersion;
            internal ushort MajorImageVersion;
            internal ushort MinorImageVersion;
            internal ushort MajorSubsystemVersion;
            internal ushort MinorSubsystemVersion;
            internal uint Win32VersionValue;
            internal uint SizeOfImage;
            internal uint SizeOfHeaders;
            internal uint CheckSum;
            internal ushort Subsystem;
            internal ushort DllCharacteristics;
            internal uint SizeOfStackReserve;
            internal uint SizeOfStackCommit;
            internal uint SizeOfHeapReserve;
            internal uint SizeOfHeapCommit;
            internal uint LoaderFlags;
            internal uint NumberOfRvaAndSizes;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            internal IMAGE_DATA_DIRECTORY[] DataDirectory;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct IMAGE_OPTIONAL_HEADER64
        {
            internal ushort Magic;
            internal byte MajorLinkerVersion;
            internal byte MinorLinkerVersion;
            internal uint SizeOfCode;
            internal uint SizeOfInitializedData;
            internal uint SizeOfUninitializedData;
            internal uint AddressOfEntryPoint;
            internal uint BaseOfCode;
            internal ulong ImageBase;
            internal uint SectionAlignment;
            internal uint FileAlignment;
            internal ushort MajorOperatingSystemVersion;
            internal ushort MinorOperatingSystemVersion;
            internal ushort MajorImageVersion;
            internal ushort MinorImageVersion;
            internal ushort MajorSubsystemVersion;
            internal ushort MinorSubsystemVersion;
            internal uint Win32VersionValue;
            internal uint SizeOfImage;
            internal uint SizeOfHeaders;
            internal uint CheckSum;
            internal ushort Subsystem;
            internal ushort DllCharacteristics;
            internal ulong SizeOfStackReserve;
            internal ulong SizeOfStackCommit;
            internal ulong SizeOfHeapReserve;
            internal ulong SizeOfHeapCommit;
            internal uint LoaderFlags;
            internal uint NumberOfRvaAndSizes;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            internal IMAGE_DATA_DIRECTORY[] DataDirectory;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct IMAGE_DATA_DIRECTORY
        {
            internal uint VirtualAddress;
            internal uint Size;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct IMAGE_SECTION_HEADER
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            internal string Name;
            internal Misc Misc;
            internal uint VirtualAddress;
            internal uint SizeOfRawData;
            internal uint PointerToRawData;
            internal uint PointerToRelocations;
            internal uint PointerToLinenumbers;
            internal ushort NumberOfRelocations;
            internal ushort NumberOfLinenumbers;
            internal uint Characteristics;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct Misc
        {
            [FieldOffset(0)]
            internal uint PhysicalAddress;
            [FieldOffset(0)]
            internal uint VirtualSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct IMAGE_COR20_HEADER
        {
            internal uint cb;
            internal ushort MajorRuntimeVersion;
            internal ushort MinorRuntimeVersion;
            internal IMAGE_DATA_DIRECTORY MetaData;       // // Symbol table and startup information
            internal uint Flags;
            internal uint EntryPointToken;
            internal IMAGE_DATA_DIRECTORY Resources;        // // Binding information
            internal IMAGE_DATA_DIRECTORY StrongNameSignature;
            internal IMAGE_DATA_DIRECTORY CodeManagerTable;        // // Regular fixup and binding information
            internal IMAGE_DATA_DIRECTORY VTableFixups;
            internal IMAGE_DATA_DIRECTORY ExportAddressTableJumps;
            internal IMAGE_DATA_DIRECTORY ManagedNativeHeader;        // // Precompiled image info (internal use only - set to zero)
        }
        #endregion

        #region Fields

        private readonly IMAGE_DOS_HEADER dosHeader;
        private IMAGE_NT_HEADERS ntHeaders;
        private readonly IMAGE_COR20_HEADER CLR;
        private readonly IList<IMAGE_SECTION_HEADER> sectionHeaders = new List<IMAGE_SECTION_HEADER>();
        private readonly uint TextBase;
        private readonly BinaryReader reader;
        private Stream stream;
        private readonly bool IsAssembly = false;
        private readonly Assembly SuppliedAssembly;
        private readonly bool OS32BitCompatible = false;
        private readonly VersionCode.Bitness ExecutableBitness;

        private readonly ILogger TL;
        #endregion

        internal ReadPECharacteristics(string FileName, ILogger TLogger)
        {
            TL = TLogger; // Save the TraceLogger instance we have been passed

            TL?.LogMessage(LogLevel.Debug, "PEReader", "Running within CLR version: " + RuntimeEnvironment.GetSystemVersion());

            if (FileName.Substring(0, 5).ToUpperInvariant() == "FILE:")
            {
                // Convert uri to local path if required, uri paths are not supported by FileStream - this method allows file names with # characters to be passed through
                var u = new Uri(FileName);
                FileName = u.LocalPath + Uri.UnescapeDataString(u.Fragment).Replace("/", @"\\");
            }
            TL?.LogMessage(LogLevel.Debug, "PEReader", "Filename to check: " + FileName);
            if (!File.Exists(FileName))
                throw new FileNotFoundException("PEReader - File not found: " + FileName);

            // Determine whether this is an assembly by testing whether we can load the file as an assembly, if so then it IS an assembly!
            TL?.LogMessage(LogLevel.Debug, "PEReader", "Determining whether this is an assembly");
            try
            {
                SuppliedAssembly = Assembly.ReflectionOnlyLoadFrom(FileName);
                IsAssembly = true; // We got here without an exception so it must be an assembly
                TL?.LogMessage(LogLevel.Debug, "PEReader.IsAssembly", $"Found an assembly because it loaded Ok to the reflection context: {SuppliedAssembly.FullName}" + IsAssembly);
            }
            catch (FileNotFoundException)
            {
                TL?.LogMessage(LogLevel.Debug, "PEReader.IsAssembly", "FileNotFoundException: File not found so this is NOT an assembly: " + IsAssembly);
            }
            catch (BadImageFormatException ex1)
            {

                // There are multiple reasons why this can occur so now determine what actually happened by examining the hResult
                int hResult = Marshal.GetHRForException(ex1);

                switch (hResult)
                {
                    case COR_E_BADIMAGEFORMAT:
                        {
                            TL?.LogMessage(LogLevel.Debug, "PEReader.IsAssembly", "BadImageFormatException. hResult: " + hResult.ToString("X8") + " - COR_E_BADIMAGEFORMAT. Setting IsAssembly to: " + IsAssembly);
                            break;
                        }
                    case CLDB_E_FILE_OLDVER:
                        {
                            TL?.LogMessage(LogLevel.Debug, "PEReader.IsAssembly", "BadImageFormatException. hResult: " + hResult.ToString("X8") + " - CLDB_E_FILE_OLDVER. Setting IsAssembly to: " + IsAssembly);
                            break;
                        }
                    case CLDB_E_INDEX_NOTFOUND:
                        {
                            TL?.LogMessage(LogLevel.Debug, "PEReader.IsAssembly", "BadImageFormatException. hResult: " + hResult.ToString("X8") + " - CLDB_E_INDEX_NOTFOUND. Setting IsAssembly to: " + IsAssembly);
                            break;
                        }
                    case CLDB_E_FILE_CORRUPT:
                        {
                            TL?.LogMessage(LogLevel.Debug, "PEReader.IsAssembly", "BadImageFormatException. hResult: " + hResult.ToString("X8") + " - CLDB_E_FILE_CORRUPT. Setting IsAssembly to: " + IsAssembly);
                            break;
                        }
                    case COR_E_NEWER_RUNTIME: // This is an assembly but it requires a newer runtime than is currently running, so flag it as an assembly even though we can't load it
                        {
                            IsAssembly = true;
                            TL?.LogMessage(LogLevel.Debug, "PEReader.IsAssembly", "BadImageFormatException. hResult: " + hResult.ToString("X8") + " - COR_E_NEWER_RUNTIME. Setting IsAssembly to: " + IsAssembly);
                            break;
                        }
                    case COR_E_ASSEMBLYEXPECTED:
                        {
                            TL?.LogMessage(LogLevel.Debug, "PEReader.IsAssembly", "BadImageFormatException. hResult: " + hResult.ToString("X8") + " - COR_E_ASSEMBLYEXPECTED. Setting IsAssembly to: " + IsAssembly);
                            break;
                        }
                    case ERROR_BAD_EXE_FORMAT:
                        {
                            TL?.LogMessage(LogLevel.Debug, "PEReader.IsAssembly", "BadImageFormatException. hResult: " + hResult.ToString("X8") + " - ERROR_BAD_EXE_FORMAT. Setting IsAssembly to: " + IsAssembly);
                            break;
                        }
                    case ERROR_EXE_MARKED_INVALID:
                        {
                            TL?.LogMessage(LogLevel.Debug, "PEReader.IsAssembly", "BadImageFormatException. hResult: " + hResult.ToString("X8") + " - ERROR_EXE_MARKED_INVALID. Setting IsAssembly to: " + IsAssembly);
                            break;
                        }
                    case CORSEC_E_INVALID_IMAGE_FORMAT:
                        {
                            TL?.LogMessage(LogLevel.Debug, "PEReader.IsAssembly", "BadImageFormatException. hResult: " + hResult.ToString("X8") + " - CORSEC_E_INVALID_IMAGE_FORMAT. Setting IsAssembly to: " + IsAssembly);
                            break;
                        }
                    case ERROR_NOACCESS:
                        {
                            TL?.LogMessage(LogLevel.Debug, "PEReader.IsAssembly", "BadImageFormatException. hResult: " + hResult.ToString("X8") + " - ERROR_NOACCESS. Setting IsAssembly to: " + IsAssembly);
                            break;
                        }
                    case ERROR_INVALID_ORDINAL:
                        {
                            TL?.LogMessage(LogLevel.Debug, "PEReader.IsAssembly", "BadImageFormatException. hResult: " + hResult.ToString("X8") + " - ERROR_INVALID_ORDINAL. Setting IsAssembly to: " + IsAssembly);
                            break;
                        }
                    case ERROR_INVALID_DLL:
                        {
                            TL?.LogMessage(LogLevel.Debug, "PEReader.IsAssembly", "BadImageFormatException. hResult: " + hResult.ToString("X8") + " - ERROR_INVALID_DLL. Setting IsAssembly to: " + IsAssembly);
                            break;
                        }
                    case ERROR_FILE_CORRUPT:
                        {
                            TL?.LogMessage(LogLevel.Debug, "PEReader.IsAssembly", "BadImageFormatException. hResult: " + hResult.ToString("X8") + " - ERROR_FILE_CORRUPT. Setting IsAssembly to: " + IsAssembly);
                            break;
                        }
                    case COR_E_LOADING_REFERENCE_ASSEMBLY:
                        {
                            TL?.LogMessage(LogLevel.Debug, "PEReader.IsAssembly", "BadImageFormatException. hResult: " + hResult.ToString("X8") + " - COR_E_LOADING_REFERENCE_ASSEMBLY. Setting IsAssembly to: " + IsAssembly);
                            break;
                        }
                    case META_E_BAD_SIGNATURE:
                        {
                            TL?.LogMessage(LogLevel.Debug, "PEReader.IsAssembly", "BadImageFormatException. hResult: " + hResult.ToString("X8") + " - META_E_BAD_SIGNATURE. Setting IsAssembly to: " + IsAssembly);
                            break;
                        }

                    default:
                        {
                            TL?.LogMessage(LogLevel.Debug, "PEReader.IsAssembly", "BadImageFormatException. hResult: " + hResult.ToString("X8") + " - Meaning of error code is unknown. Setting IsAssembly to: " + IsAssembly);
                            break;
                        }
                }
            }

            catch (FileLoadException) // This is an assembly but that has already been loaded so flag it as an assembly
            {
                IsAssembly = true;
                TL?.LogMessage(LogLevel.Debug, "PEReader.IsAssembly", "FileLoadException: Assembly already loaded so this is an assembly: " + IsAssembly);
            }

            TL?.LogMessage(LogLevel.Debug, "PEReader", "Determining PE Machine type");
            stream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            reader = new BinaryReader(stream);

            reader.BaseStream.Seek(0L, SeekOrigin.Begin); // Reset reader position, just in case
            dosHeader = MarshalBytesTo<IMAGE_DOS_HEADER>(reader); // Read MS-DOS header section
            if (dosHeader.e_magic != 0x5A4D) // MS-DOS magic number should read 'MZ'
            {
                throw new InvalidOperationException("File is not a portable executable.");
            }

            reader.BaseStream.Seek(dosHeader.e_lfanew, SeekOrigin.Begin); // Skip MS-DOS stub and seek reader to NT Headers
            ntHeaders.Signature = MarshalBytesTo<uint>(reader); // Read NT Headers
            if (ntHeaders.Signature != 0x4550L) // Make sure we have 'PE' in the pe signature 
            {
                throw new InvalidOperationException("Invalid portable executable signature in NT header.");
            }
            ntHeaders.FileHeader = MarshalBytesTo<IMAGE_FILE_HEADER>(reader); // Read the IMAGE_FILE_HEADER which starts 4 bytes on from the start of the signature (already here by reading the signature itself)

            // Determine whether this executable is flagged as a 32bit or 64bit and set OS32BitCompatible accordingly
            switch (ntHeaders.FileHeader.Machine)
            {
                case IMAGE_FILE_MACHINE_I386:
                    {
                        OS32BitCompatible = true;
                        TL?.LogMessage(LogLevel.Debug, "PEReader.MachineType", "Machine - found \"Intel 32bit\" executable. Characteristics: " + ntHeaders.FileHeader.Characteristics.ToString("X8") + ", OS32BitCompatible: " + OS32BitCompatible);
                        break;
                    }
                case IMAGE_FILE_MACHINE_IA64:
                    {
                        OS32BitCompatible = false;
                        TL?.LogMessage(LogLevel.Debug, "PEReader.MachineType", "Machine - found \"Itanium 64bit\" executable. Characteristics: " + ntHeaders.FileHeader.Characteristics.ToString("X8") + ", OS32BitCompatible: " + OS32BitCompatible);
                        break;
                    }
                case IMAGE_FILE_MACHINE_AMD64:
                    {
                        OS32BitCompatible = false;
                        TL?.LogMessage(LogLevel.Debug, "PEReader.MachineType", "Machine - found \"Intel 64bit\" executable. Characteristics: " + ntHeaders.FileHeader.Characteristics.ToString("X8") + ", OS32BitCompatible: " + OS32BitCompatible);
                        break;
                    }

                default:
                    {
                        TL?.LogMessage(LogLevel.Debug, "PEReader.MachineType", "Found Unknown machine type: " + ntHeaders.FileHeader.Machine.ToString("X8") + ". Characteristics: " + ntHeaders.FileHeader.Characteristics.ToString("X8") + ", OS32BitCompatible: " + OS32BitCompatible);
                        break;
                    }
            }

            if (OS32BitCompatible) // Read optional 32bit header
            {
                TL?.LogMessage(LogLevel.Debug, "PEReader.MachineType", "Reading optional 32bit header");
                ntHeaders.OptionalHeader32 = MarshalBytesTo<IMAGE_OPTIONAL_HEADER32>(reader);
            }
            else // Read optional 64bit header
            {
                TL?.LogMessage(LogLevel.Debug, "PEReader.MachineType", "Reading optional 64bit header");
                ntHeaders.OptionalHeader64 = MarshalBytesTo<IMAGE_OPTIONAL_HEADER64>(reader);
            }

            if (IsAssembly)
            {
                TL?.LogMessage(LogLevel.Debug, "PEReader", "This is an assembly, determining Bitness through the CLR header");
                // Find the CLR header
                int NumberOfHeadersToCheck = MAX_HEADERS_TO_CHECK;
                if (OS32BitCompatible) // We have a 32bit assembly
                {
                    TL?.LogMessage(LogLevel.Debug, "PEReader.Bitness", "This is a 32 bit assembly, reading the CLR Header");
                    if (ntHeaders.OptionalHeader32.NumberOfRvaAndSizes < (long)MAX_HEADERS_TO_CHECK)
                        NumberOfHeadersToCheck = (int)ntHeaders.OptionalHeader32.NumberOfRvaAndSizes;
                    TL?.LogMessage(LogLevel.Debug, "PEReader.Bitness", "Checking " + NumberOfHeadersToCheck + " headers");

                    for (int i = 0, loopTo = NumberOfHeadersToCheck - 1; i <= loopTo; i++)
                    {
                        if (ntHeaders.OptionalHeader32.DataDirectory[i].Size > 0L)
                        {
                            sectionHeaders.Add(MarshalBytesTo<IMAGE_SECTION_HEADER>(reader));
                        }
                    }

                    foreach (IMAGE_SECTION_HEADER SectionHeader in sectionHeaders)
                    {
                        if (SectionHeader.Name == ".text")
                            TextBase = SectionHeader.PointerToRawData;
                    }

                    if (NumberOfHeadersToCheck >= CLR_HEADER + 1) // Only test if the number of headers meets or exceeds the lcoation of the CLR header
                    {
                        if (ntHeaders.OptionalHeader32.DataDirectory[CLR_HEADER].VirtualAddress > 0L)
                        {
                            reader.BaseStream.Seek(ntHeaders.OptionalHeader32.DataDirectory[CLR_HEADER].VirtualAddress - ntHeaders.OptionalHeader32.BaseOfCode + TextBase, SeekOrigin.Begin);
                            CLR = MarshalBytesTo<IMAGE_COR20_HEADER>(reader);
                        }
                    }
                }
                else // We have a 64bit assembly
                {
                    TL?.LogMessage(LogLevel.Debug, "PEReader.Bitness", "This is a 64 bit assembly, reading the CLR Header");
                    if (ntHeaders.OptionalHeader64.NumberOfRvaAndSizes < (long)MAX_HEADERS_TO_CHECK)
                        NumberOfHeadersToCheck = (int)ntHeaders.OptionalHeader64.NumberOfRvaAndSizes;
                    TL?.LogMessage(LogLevel.Debug, "PEReader.Bitness", "Checking " + NumberOfHeadersToCheck + " headers");

                    for (int i = 0, loopTo1 = NumberOfHeadersToCheck - 1; i <= loopTo1; i++)
                    {
                        if (ntHeaders.OptionalHeader64.DataDirectory[i].Size > 0L)
                        {
                            sectionHeaders.Add(MarshalBytesTo<IMAGE_SECTION_HEADER>(reader));
                        }
                    }

                    foreach (IMAGE_SECTION_HEADER SectionHeader in sectionHeaders)
                    {
                        if (SectionHeader.Name == ".text")
                        {
                            TL?.LogMessage(LogLevel.Debug, "PEReader.Bitness", "Found TEXT section");
                            TextBase = SectionHeader.PointerToRawData;
                        }
                    }

                    if (NumberOfHeadersToCheck >= CLR_HEADER + 1) // Only test if the number of headers meets or exceeds the location of the CLR header
                    {
                        if (ntHeaders.OptionalHeader64.DataDirectory[CLR_HEADER].VirtualAddress > 0L)
                        {
                            reader.BaseStream.Seek(ntHeaders.OptionalHeader64.DataDirectory[CLR_HEADER].VirtualAddress - ntHeaders.OptionalHeader64.BaseOfCode + TextBase, SeekOrigin.Begin);
                            CLR = MarshalBytesTo<IMAGE_COR20_HEADER>(reader);
                            TL?.LogMessage(LogLevel.Debug, "PEReader.Bitness", "Read CLR header successfully");
                        }
                    }

                }

                // Determine the bitness from the CLR header
                if (OS32BitCompatible) // Could be an x86 or MSIL assembly so determine which
                {
                    if ((CLR.Flags & (long)CLR_FLAGSType.CLR_FLAGS_32BITREQUIRED) > 0L)
                    {
                        TL?.LogMessage(LogLevel.Debug, "PEReader.Bitness", "Found \"32bit Required\" assembly");
                        ExecutableBitness = VersionCode.Bitness.Bits32;
                    }
                    else
                    {
                        TL?.LogMessage(LogLevel.Debug, "PEReader.Bitness", "Found \"MSIL\" assembly");
                        ExecutableBitness = VersionCode.Bitness.BitsMSIL;
                    }
                }
                else // Must be an x64 assembly
                {
                    TL?.LogMessage(LogLevel.Debug, "PEReader.Bitness", "Found \"64bit Required\" assembly");
                    ExecutableBitness = VersionCode.Bitness.Bits64;
                }

                TL?.LogMessage(LogLevel.Debug, "PEReader", "Assembly required Runtime version: " + CLR.MajorRuntimeVersion + "." + CLR.MinorRuntimeVersion);
            }
            else // Not an assembly so just use the FileHeader.Machine value to determine bitness
            {
                TL?.LogMessage(LogLevel.Debug, "PEReader", "This is not an assembly, determining Bitness through the executable bitness flag");
                if (OS32BitCompatible)
                {
                    TL?.LogMessage(LogLevel.Debug, "PEReader.Bitness", "Found 32bit executable");
                    ExecutableBitness = VersionCode.Bitness.Bits32;
                }
                else
                {
                    TL?.LogMessage(LogLevel.Debug, "PEReader.Bitness", "Found 64bit executable");
                    ExecutableBitness = VersionCode.Bitness.Bits64;
                }

            }
        }

        internal VersionCode.Bitness BitNess
        {
            get
            {
                TL?.LogMessage(LogLevel.Debug, "PE.BitNess", "Returning: " + ((int)ExecutableBitness).ToString());
                return ExecutableBitness;
            }
        }

        internal bool IsDotNetAssembly()
        {
            TL?.LogMessage(LogLevel.Debug, "PE.IsDotNetAssembly", "Returning: " + IsAssembly);
            return IsAssembly;
        }

        internal SubSystemType SubSystem()
        {
            if (OS32BitCompatible)
            {
                TL?.LogMessage(LogLevel.Debug, "PE.SubSystem", "Returning 32bit value: " + ((SubSystemType)ntHeaders.OptionalHeader32.Subsystem).ToString());
                return (SubSystemType)ntHeaders.OptionalHeader32.Subsystem; // Return the 32bit header field
            }
            else
            {
                TL?.LogMessage(LogLevel.Debug, "PE.SubSystem", "Returning 64bit value: " + ((SubSystemType)ntHeaders.OptionalHeader64.Subsystem).ToString());
                return (SubSystemType)ntHeaders.OptionalHeader64.Subsystem;
            } // Return the 64bit field
        }

        private static T MarshalBytesTo<T>(BinaryReader reader)
        {
            // Unmanaged data
            byte[] bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));

            // Create a pointer to the unmanaged data pinned in memory to be accessed by unmanaged code
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            // Use our previously created pointer to unmanaged data and marshal to the specified type
            T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));

            // Deallocate pointer
            handle.Free();

            return theStructure;
        }

        #region IDisposable Support
        private bool disposedValue; // To detect redundant calls

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        reader.Close();
                        stream.Close();
                        stream.Dispose();
                        stream = null;
                    }
                    catch (Exception) // Swallow any exceptions here
                    {
                    }
                }

            }
            disposedValue = true;
        }

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

#endif
    }

#endregion
}