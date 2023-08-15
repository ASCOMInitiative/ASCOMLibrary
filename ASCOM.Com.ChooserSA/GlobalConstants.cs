namespace ASCOM.Com
{

    // Common constants for the ASCOM.Com namesapce

    static class GlobalConstants
    {
        internal const string SERIAL_FILE_NAME_VARNAME = "SerTraceFile"; // Constant naming the profile trace file variable name
        internal const string SERIAL_AUTO_FILENAME = @"C:\SerialTraceAuto.txt"; // Special value to indicate use of automatic trace filenames
        internal const string SERIAL_DEFAULT_FILENAME = @"C:\SerialTrace.txt"; // Default manual trace filename
        internal const string SERIAL_DEBUG_TRACE_VARNAME = "SerDebugTrace"; // Constant naming the profile trace file variable name
        internal const string SERIALPORT_COM_PORT_SETTINGS = "COMPortSettings";
        internal const string SERIAL_FORCED_COMPORTS_VARNAME = SERIALPORT_COM_PORT_SETTINGS + @"\ForceCOMPorts"; // Constant listing COM ports that will be forced to be present
        internal const string SERIAL_IGNORE_COMPORTS_VARNAME = SERIALPORT_COM_PORT_SETTINGS + @"\IgnoreCOMPorts"; // Constant listing COM ports that will be ignored if present

        // Utilities configuration constants
        internal const string TRACE_XMLACCESS = "Trace XMLAccess";
        internal const bool TRACE_XMLACCESS_DEFAULT = false;
        internal const string TRACE_PROFILE = "Trace Profile";
        internal const bool TRACE_PROFILE_DEFAULT = false;
        internal const string TRACE_UTIL = "Trace Util";
        internal const bool TRACE_UTIL_DEFAULT = false;
        internal const string TRACE_TIMER = "Trace Timer";
        internal const bool TRACE_TIMER_DEFAULT = false;
        internal const string SERIAL_TRACE_DEBUG = "Serial Trace Debug";
        internal const bool SERIAL_TRACE_DEBUG_DEFAULT = false;
        internal const string SIMULATOR_TRACE = "Trace Simulators";
        internal const bool SIMULATOR_TRACE_DEFAULT = false;
        internal const string DRIVERACCESS_TRACE = "Trace DriverAccess";
        internal const bool DRIVERACCESS_TRACE_DEFAULT = false;
        internal const string CHOOSER_USE_CREATEOBJECT = "Chooser Use CreateObject";
        internal const bool CHOOSER_USE_CREATEOBJECT_DEFAULT = false;
        internal const string ABANDONED_MUTEXT_TRACE = "Trace Abandoned Mutexes";
        internal const bool ABANDONED_MUTEX_TRACE_DEFAULT = false;
        internal const string ASTROUTILS_TRACE = "Trace Astro Utils";
        internal const bool ASTROUTILS_TRACE_DEFAULT = false;
        internal const string NOVAS_TRACE = "Trace NOVAS";
        internal const bool NOVAS_TRACE_DEFAULT = false;
        internal const string SUPPRESS_ALPACA_DRIVER_ADMIN_DIALOGUE = "Suppress Alpaca Driver Admin Dialogue";
        internal const bool SUPPRESS_ALPACA_DRIVER_ADMIN_DIALOGUE_DEFAULT = false;
        internal const string PROFILE_MUTEX_NAME = "ASCOMProfileMutex"; // Name and timout value for the Profile mutex than ensures only one profile action happens at a time
        internal const int PROFILE_MUTEX_TIMEOUT = 5000;

        // Trace settings values, these are used to persist trace values on a per user basis
        internal const string TRACE_TRANSFORM = "Trace Transform";
        internal const bool TRACE_TRANSFORM_DEFAULT = false;
        internal const string REGISTRY_UTILITIES_FOLDER = @"Software\ASCOM\Utilities";
        internal const string TRACE_CACHE = "Trace Cache";
        internal const bool TRACE_CACHE_DEFAULT = false;
        internal const string TRACE_EARTHROTATION_DATA_FORM = "Trace Earth Rotation Data Form";
        internal const bool TRACE_EARTHROTATION_DATA_FORM_DEFAULT = false;

        // Settings for the ASCOM Windows event log
        internal const string EVENT_SOURCE = "ASCOM Platform"; // Name of the the event source
        internal const string EVENTLOG_NAME = "ASCOM"; // Name of the event log as it appears in Windows event viewer
        internal const string EVENTLOG_MESSAGES = @"ASCOM\EventLogMessages.txt";
        internal const string EVENTLOG_ERRORS = @"ASCOM\EventLogErrors.txt";

        // RegistryAccess constants
        internal const string REGISTRY_ROOT_KEY_NAME = @"SOFTWARE\ASCOM"; // Location of ASCOM profile in HKLM registry hive
        internal const string REGISTRY_5_BACKUP_SUBKEY = "Platform5Original"; // Location that the original Plartform 5 Profile will be copied to before migrating the 5.5 Profile back to the registry
        internal const string REGISTRY_55_BACKUP_SUBKEY = "Platform55Original"; // Location that the original Plartform 5.5 Profile will be copied to before removing Platform 5 and 5.5
        internal const string PLATFORM_VERSION_NAME = "PlatformVersion";
        // XML constants used by XMLAccess and RegistryAccess classes
        internal const string COLLECTION_DEFAULT_VALUE_NAME = "***** DefaultValueName *****"; // Name identifier label
        internal const string COLLECTION_DEFAULT_UNSET_VALUE = "===== ***** UnsetValue ***** ====="; // Value identifier label
        internal const string VALUES_FILENAME = "Profile.xml"; // Name of file to contain profile xml information
        internal const string VALUES_FILENAME_ORIGINAL = "ProfileOriginal.xml"; // Name of file to contain original profile xml information
        internal const string VALUES_FILENAME_NEW = "ProfileNew.xml"; // Name of file to contain original profile xml information

        internal const string PROFILE_NAME = "Profile"; // Name of top level XML element
        internal const string SUBKEY_NAME = "SubKey"; // Profile subkey element name
        internal const string DEFAULT_ELEMENT_NAME = "DefaultElement"; // Default value label
        internal const string VALUE_ELEMENT_NAME = "Element"; // Profile value element name
        internal const string NAME_ATTRIBUTE_NAME = "Name"; // Profile value name attribute
        internal const string VALUE_ATTRIBUTE_NAME = "Value"; // Profile element value attribute

        // Location of the lists of 32bit and 64bit only drivers and PlatformVersion exception lists
        public const string DRIVERS_32BIT = "Drivers Not Compatible With 64bit Applications"; // 32bit only registry location
        public const string DRIVERS_64BIT = "Drivers Not Compatible With 32bit Applications"; // 64bit only registry location
        internal const string PLATFORM_VERSION_EXCEPTIONS = "ForcePlatformVersion";
        internal const string PLATFORM_VERSION_SEPARATOR_EXCEPTIONS = "ForcePlatformVersionSeparator";


        // Contact driver author message
        internal const string DRIVER_AUTHOR_MESSAGE_DRIVER = "Please contact the driver author and request an updated driver.";
        internal const string DRIVER_AUTHOR_MESSAGE_INSTALLER = "Please contact the driver author and request an updated installer.";


        internal enum EventLogErrors : int
        {
            EventLogCreated = 0,
            ChooserFormLoad = 1,
            MigrateProfileVersions = 2,
            MigrateProfileRegistryKey = 3,
            RegistryProfileMutexTimeout = 4,
            XMLProfileMutexTimeout = 5,
            XMLAccessReadError = 6,
            XMLAccessRecoveryPreviousVersion = 7,
            XMLAccessRecoveredOK = 8,
            ChooserSetupFailed = 9,
            ChooserDriverFailed = 10,
            ChooserException = 11,
            Chooser32BitOnlyException = 12,
            Chooser64BitOnlyException = 13,
            FocusSimulatorNew = 14,
            FocusSimulatorSetup = 15,
            TelescopeSimulatorNew = 16,
            TelescopeSimulatorSetup = 17,
            VB6HelperProfileException = 18,
            DiagnosticsLoadException = 19,
            DriverCompatibilityException = 20,
            TimerSetupException = 21,
            DiagnosticsHijackedCOMRegistration = 22,
            UninstallASCOMInfo = 23,
            UninstallASCOMError = 24,
            ProfileExplorerException = 25,
            InstallTemplatesInfo = 26,
            InstallTemplatesError = 27,
            TraceLoggerException = 28,
            TraceLoggerMutexTimeOut = 29,
            TraceLoggerMutexAbandoned = 30,
            RegistryProfileMutexAbandoned = 31,
            EarthRotationUpdate = 32,
            ManageScheduledTask = 33,
            Sofa = 34
        }
    }
}