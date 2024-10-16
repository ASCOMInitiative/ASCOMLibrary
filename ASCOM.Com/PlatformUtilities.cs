﻿using ASCOM.Common.Interfaces;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.IO;
using static System.Environment;
using ASCOM.Common;
using System.Collections.Generic;
using System.Linq;

namespace ASCOM.Com
{
    /// <summary>
    /// Utilities relevant to Windows / COM Driver / ASCOM Platform development
    /// </summary>
    public static class PlatformUtilities
    {
        // Constants for Version functions
        const int MINIMUM_VALID_PLATFORM_VERSION = 1;
        const int MAXIMUM_VALID_PLATFORM_VERSION = 7;
        const int MINIMUM_VALID_MINOR_VERSION = 0;
        const int MAXIMUM_VALID_MINOR_VERSION = 6;
        const int MINIMUM_VALID_SERVICEPACK_VERSION = 0;
        const int MAXIMUM_VALID_SERVICEPACK_VERSION = 3;
        const int MINIMUM_BUILD_NUMBER = 0;
        const int MAXIMUM_BUILD_NUMBER = 65535;

        const string PROFILE_ROOT_KEY = @"SOFTWARE\ASCOM";

        // Constants for CreateDynamicDriver method
        private const string DRIVER_PROGID_BASE = "ASCOM.AlpacaDynamic";
        private const string ALPACA_DYNAMIC_CLIENT_MANAGER_RELATIVE_PATH = @"ASCOM\Platform\Tools\AlpacaDynamicClientManager";
        private const string ALPACA_DYNAMIC_CLIENT_MANAGER_EXE_NAME = "ASCOM.AlpacaDynamicClientManager.exe";

        // Alpaca driver Profile store value names
        private const string PROFILE_VALUE_NAME_UNIQUEID = "UniqueID"; // Prefix applied to all COM drivers created to front Alpaca devices
        private const string PROFILE_VALUE_NAME_IP_ADDRESS = "IP Address";
        private const string PROFILE_VALUE_NAME_PORT_NUMBER = "Port Number";
        private const string PROFILE_VALUE_NAME_REMOTE_DEVICER_NUMBER = "Remote Device Number";
        private const string PROFILE_VALUE_NAME_COM_GUID = "COM Guid"; // This value must match the same named constant in the Dynamic Client Local Server project LocalServer.cs file

        // Variables
        private static readonly Version platformVersion = null;
        private static ILogger logger;
        private static bool driverGenerationComplete;

        #region Initialise

        /// <summary>
        /// initialise the internal Platform version variable if possible
        /// </summary>
        static PlatformUtilities()
        {
            // Populate the Platform version variable when the class is initialised

            // Get the detailed (four part) Platform version from the "Platform Version" value from the Profile's "Platform" key.
            try
            {
                using (RegistryKey localmachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    RegistryKey driverKey = localmachine32.OpenSubKey($"{PROFILE_ROOT_KEY}\\Platform", false);
                    platformVersion = new Version((string)driverKey.GetValue("Platform Version"));
                }
            }
            catch (Exception)
            {
                // Something went wrong. Possibly this is an early version of the Platform that doesn't have the full version string, so try to get the two part MAJOR.MINOR version from the profile root instead
                try
                {
                    using (RegistryKey localmachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                    {
                        RegistryKey driverKey = localmachine32.OpenSubKey(PROFILE_ROOT_KEY, false);
                        platformVersion = new Version((string)driverKey.GetValue("PlatformVersion"));
                    }
                }
                catch
                {
                    // Ignore exceptions here, platformVersion will be null.
                }
            }
        }

        #endregion

        #region Logger configuration

        /// <summary>
        /// Set a logger instance that will receive runtime diagnostic information
        /// </summary>
        /// <param name="logger">Optional ILogger instance to which operational / debug messages will be sent.</param>
        public static void SetLogger(ILogger logger)
        {
            PlatformUtilities.logger = logger;
        }

        #endregion

        #region Version related properties and methods

        /// <summary>
        /// Current Platform version in Major.Minor form
        /// </summary>
        /// <returns>Current Platform version in Major.Minor form</returns>
        /// <remarks>Please note that this function returns the version number in the invariant culture, which means that the MAJOR.MINOR separator is always the point
        /// character regardless of which character is used as the decimal separator in the application's locale.
        /// <para>If you wish to convert the Platform version into a Double value, you should parse the string using the invariant culture as follows:</para>
        /// <code>
        /// double platformVersion = Double.Parse(PlatformUtilities.PlatformVersion, CultureInfo.InvariantCulture)
        /// </code>
        /// <para>If you just wish to test whether the platform is greater than a particular level,
        /// you can use the <see cref="IsMinimumRequiredVersion">IsMinimumRequiredVersion</see> method.</para>
        /// </remarks>
        public static string PlatformVersion
        {
            get
            {
                // Check that the Platform version was read successfully when the class was initialised
                CheckPlatformVersionIsOk();

                return $"{platformVersion.Major}.{platformVersion.Minor}";
            }
        }

        /// <summary>
        /// Return the ASCOM Platform's major version number
        /// </summary>
        public static int MajorVersion
        {
            get
            {
                // Check that the Platform version was read successfully when the class was initialised
                CheckPlatformVersionIsOk();

                return platformVersion.Major;
            }
        }

        /// <summary>
        /// Return the ASCOM Platform's minor version number
        /// </summary>
        public static int MinorVersion
        {
            get
            {
                // Check that the Platform version was read successfully when the class was initialised
                CheckPlatformVersionIsOk();

                return platformVersion.Minor;
            }
        }

        /// <summary>
        /// Return the ASCOM Platform's Service pack number
        /// </summary>
        public static int ServicePack
        {
            get
            {
                // Check that the Platform version was read successfully when the class was initialised
                CheckPlatformVersionIsOk();

                return platformVersion.Build;
            }
        }

        /// <summary>
        /// Return the ASCOM Platform's build number
        /// </summary>
        public static int BuildNumber
        {
            get
            {
                // Check that the Platform version was read successfully when the class was initialised
                CheckPlatformVersionIsOk();

                return platformVersion.Revision;
            }
        }

        /// <summary>
        /// Tests whether the current platform version is at least equal to the supplied major and minor version numbers, returns false if this is not the case
        /// </summary>
        /// <param name="requiredMajorVersion">The required major version number</param>
        /// <param name="requiredMinorVersion">The required minor version number. Use 0 if any minor version is acceptable.</param>
        /// <param name="requiredServicePack">The required service pack number. Use 0 if any service pack is acceptable.</param>
        /// <param name="requiredBuild">The required build number. Use 0 if any build number is acceptable</param>
        /// <exception cref="InvalidValueException">When any parameter value is outside the range implemented by the Platform.</exception>
        /// <returns>True if the current platform version equals or exceeds the version specified.</returns>
        /// <remarks>This function provides a simple way to test for a minimum platform level.
        /// If for example, your application requires at least platform version 6.6.1.0 then you can use 
        /// code such as this to make a test and display information as appropriate.
        /// <code >
        /// if(!PlatformUtilities.IsMinimumRequiredVersion(6, 6, 1, 0)
        /// {
        ///    // Abort, throw an exception, return an error etc. as appropriate.
        /// }
        /// </code>
        /// </remarks>
        public static bool IsMinimumRequiredVersion(int requiredMajorVersion, int requiredMinorVersion, int requiredServicePack, int requiredBuild)
        {
            Version requiredVersion; // Version objects to hold the requested and actual Platform version numbers

            // Check that the Platform version was read successfully when the class was initialised
            CheckPlatformVersionIsOk();

            // Validate supplied parameters
            if ((requiredMajorVersion < MINIMUM_VALID_PLATFORM_VERSION) | (requiredMajorVersion > MAXIMUM_VALID_PLATFORM_VERSION))
                throw new InvalidValueException("Platform major version parameter", requiredMajorVersion.ToString(), MINIMUM_VALID_PLATFORM_VERSION.ToString(), MAXIMUM_VALID_PLATFORM_VERSION.ToString());

            if ((requiredMinorVersion < MINIMUM_VALID_MINOR_VERSION) | (requiredMinorVersion > MAXIMUM_VALID_MINOR_VERSION))
                throw new InvalidValueException("Platform minor version parameter", requiredMinorVersion.ToString(), MINIMUM_VALID_MINOR_VERSION.ToString(), MAXIMUM_VALID_MINOR_VERSION.ToString());

            if ((requiredServicePack < MINIMUM_VALID_SERVICEPACK_VERSION) | (requiredServicePack > MAXIMUM_VALID_SERVICEPACK_VERSION))
                throw new InvalidValueException("Service pack parameter", requiredServicePack.ToString(), MINIMUM_VALID_SERVICEPACK_VERSION.ToString(), MAXIMUM_VALID_SERVICEPACK_VERSION.ToString());

            if ((requiredBuild < MINIMUM_BUILD_NUMBER) | (requiredBuild > MAXIMUM_BUILD_NUMBER))
                throw new InvalidValueException("Build number parameter", requiredBuild.ToString(), MINIMUM_BUILD_NUMBER.ToString(), MAXIMUM_BUILD_NUMBER.ToString());

            // Create a version object from the supplied major and minor required version numbers
            requiredVersion = new Version(requiredMajorVersion, requiredMinorVersion, requiredServicePack, requiredBuild);

            LogMessage("IsMinimumRequiredVersion", $"platformVersion: {platformVersion}, Required version: {requiredVersion}");

            // Compare the two versions and respond accordingly
            if (platformVersion >= requiredVersion)
                return true; // Platform version is equal to or greater than the required version
            else
                return false;// Platform version is less than the required version
        }

        /// <summary>
        /// Determine whether the ASCOM Platform is installed.
        /// </summary>
        /// <returns>True if the ASCOM Platform is installed, otherwise false.</returns>
        /// <remarks>Assumes that the Platform is not installed if we cannot determine the version number.</remarks>
        public static bool IsPlatformInstalled()
        {
            try
            {
                CheckPlatformVersionIsOk(); // Throws an InvalidOperationException if the Platform version cannot be determined

                // If we get here a valid Platform version number has been found
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region CreateDynamicDriver

        /// <summary>
        /// Create an Alpaca Dynamic Driver to present an Alpaca device as a COM device. (Platform 6.5 or later)
        /// </summary>
        /// <param name="deviceType">ASCOM device type</param>
        /// <param name="deviceNumber">ALpaca device number of this device</param>
        /// <param name="description">Text description of the Alpaca device that will appear in the Chooser.</param>
        /// <param name="hostName">Host-name or IP address of the Alpaca device</param>
        /// <param name="ipPort">IP port of the Alpaca device</param>
        /// <param name="deviceUniqueId">The Alpaca device's unique ID.</param>
        /// <exception cref="InvalidOperationException">If the installed Platform version is earlier that 6.5.</exception>
        /// <exception cref="InvalidValueException">When supplied parameters are outside valid ranges.</exception>
        /// <returns>The COM ProgID of the created dynamic driver.</returns>
        /// <remarks>
        /// The Dynamic driver functionality was introduced in Platform 6.5, consequently, this method will throw an InvalidOperationException if invoked on previous platforms.
        /// <para>
        /// Microsoft security requires that the user provides Administrator level access in order to create and register the dynamic driver. The Platform's Dynamic Client Manager 
        /// will trigger the UAC request process automatically and will, or will not, run depending on whether permission is granted. As an application developer, you are not directly involved in 
        /// this automatic process. However, you should make your customers aware that the security dialogue will appear when creating the driver, and check that the modal UAC dialogue does not disrupt your application 
        /// or UI while displayed.
        /// /// </para>
        /// </remarks>
        public static string CreateDynamicDriver(DeviceTypes deviceType, int deviceNumber, string description, string hostName, int ipPort, string deviceUniqueId)
        {
            string newProgId = ""; // Holds the ProgID of the dynamically created driver

            CheckPlatformVersionIsOk(); // Throws an InvalidOperationException if the Platform version cannot be determined

            // Validate the Platform level: Must be 6.5 or later
            if (!IsMinimumRequiredVersion(6, 5, 0, 0)) throw new InvalidOperationException($"CreateDynamicDriver - This method requires Platform 6.5 or later. Installed Platform version: {PlatformVersion}");

            // Validate input parameters
            if (deviceNumber < 0) throw new InvalidValueException("CreateDynamicDriver - Device number", deviceNumber.ToString(), "0", int.MaxValue.ToString());
            if (description is null) throw new InvalidValueException("CreateDynamicDriver - Description must not be null.");
            if (description == "") throw new InvalidValueException("CreateDynamicDriver - Description must not be an empty string.");
            if (string.IsNullOrEmpty(hostName)) throw new InvalidValueException("CreateDynamicDriver - HostName must not be null or an empty string.");
            if ((ipPort < 1) | (ipPort > 65535)) throw new InvalidValueException("CreateDynamicDriver - IPPort", ipPort.ToString(), "1", "65535");
            if (string.IsNullOrEmpty(deviceUniqueId)) throw new InvalidValueException("CreateDynamicDriver - DeviceUniqueId must not be null or an empty string.");

            try
            {
                // Create a new Alpaca driver of the current ASCOM device type
                newProgId = CreateNewAlpacaDriver(deviceType, description);
                LogMessage("CreateDynamicDriver", $"Device type: {deviceType}, newProgId: {newProgId}");

                // Configure the IP address, port number and Alpaca device number in the newly registered driver
                Profile.SetValue(deviceType, newProgId, PROFILE_VALUE_NAME_IP_ADDRESS, hostName);
                Profile.SetValue(deviceType, newProgId, PROFILE_VALUE_NAME_PORT_NUMBER, ipPort.ToString());
                Profile.SetValue(deviceType, newProgId, PROFILE_VALUE_NAME_REMOTE_DEVICER_NUMBER, deviceNumber.ToString());
                Profile.SetValue(deviceType, newProgId, PROFILE_VALUE_NAME_UNIQUEID, deviceUniqueId);

                // Flag the driver as being already configured so that it can be used immediately
                using (RegistryKey localmachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    RegistryKey driverKey = localmachine32.CreateSubKey($"{PROFILE_ROOT_KEY}\\Chooser", true);
                    driverKey.SetValue($"{newProgId} Init", "True");
                }

                LogMessage("OK Click", $"Returning ProgID: '{newProgId}'");
            }
            catch (Win32Exception ex) when ((uint)ex.ErrorCode == 0x80004005)
            {
                LogMessage("CreateDynamicDriver", $"Security approval not given, returning empoty string: \r\n{ex}");
                // Security approval was not given by the user
                return "";
            }
            catch (Exception ex)
            {
                LogMessage("CreateDynamicDriver", $"Exception: \r\n{ex}");
                throw;
            }
            return newProgId;
        }

        private static string CreateNewAlpacaDriver(DeviceTypes deviceType, string deviceDescription)
        {
            string newProgId;
            int deviceNumber;
            Type typeFromProgId;

            CheckPlatformVersionIsOk(); // Throws an InvalidOperationException if the Platform version cannot be determined

            // Initialise to a starting value
            deviceNumber = 0;

            // Try successive ProgIDs until one is found that is not COM registered
            do
            {
                // Increment the device number
                deviceNumber += 1;

                // Create the new ProgID to be tested
                newProgId = $"{DRIVER_PROGID_BASE}{deviceNumber}.{Devices.DeviceTypeToString(deviceType)}";

                // Try to get the type with the new ProgID
                typeFromProgId = Type.GetTypeFromProgID(newProgId);
                LogMessage("CreateAlpacaClient", $"Testing ProgID: {newProgId} Type name: {typeFromProgId?.Name}");
            }
            while (!(typeFromProgId is null)); // Loop until the returned type is null indicating that this type is not COM registered
            LogMessage("CreateAlpacaClient", $"Creating new ProgID: {newProgId}");

            // Register the new ProgID so that the local server will register it
            Profile.Register(deviceType, newProgId, deviceDescription);

            // Create a new COM GUID for this driver if one does not already exist.
            // At this point, we aren't interested in the returned value, only that a value exists. This is ensured by use of the default value: Guid.NewGuid().
            Profile.GetValue(deviceType, newProgId, PROFILE_VALUE_NAME_COM_GUID, "", Guid.NewGuid().ToString());

            // Call the dynamic client manager to run the local server to register the new ProgID device
            RunDynamicClientManager($@"\CreateAlpacaClient {deviceType} {deviceNumber} {newProgId} ""{deviceDescription}""");

            // Return the new ProgID
            return newProgId;
        }

        #endregion

        #region OS Support

        /// <summary>
        /// Returns the Microsoft OS build name based on the build number stored in the Windows registry
        /// </summary>
        /// <remarks>
        /// This function only works on Microsoft operating systems. If you need this function on a non-Windows OS, use <see cref="OSBuildName(int)"/> instead and supply the build number as a parameter.
        /// </remarks>
        /// <returns>The descriptive name of the operating system e.g. Windows 11 (24H2).</returns>
        public static string OSBuildName()
        {
            try
            {
                // Open the OS version registry key
                using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
                {
                    // Get the OS build number
                    int currentBuildNumber = Int32.Parse(regKey.GetValue("currentBuildNumber", "0").ToString());
                    return OSBuildName(currentBuildNumber);
                }
            }
            catch (Exception ex)
            {
                return $"ASCOMLibrary.Tools.Utilities.OSBuildName - Exception occurred: {ex.Message}";
            }
        }

        /// <summary>
        /// Returns the Microsoft Windows OS build name based on the supplied build number
        /// </summary>
        /// <param name="buildNumber">The integer Windows build number for which a descriptive OS name is required.</param>
        /// <remarks></remarks>
        /// <returns>The descriptive name of the operating system e.g. Windows 11 (24H2).</returns>
        public static string OSBuildName(int buildNumber)
        {
            Dictionary<int, string> osBuildNames = new Dictionary<int, string>()
            {
                { 19041, "Windows 10 (2004)" },
                { 19042, "Windows 10 (20H2)" },
                { 19043, "Windows 10 (21H1)" },
                { 19044, "Windows 10 (21H2)" },
                { 19045, "Windows 10 (22H2)" },
                { 22000, "Windows 11 (21H2)" },
                { 22621, "Windows 11 (22H2)" },
                { 22631, "Windows 11 (23H2)" },
                { 26100, "Windows 11 (24H2)" }
            };

            try
            {
                // Use the build number to determine the OS name - Select the appropriate OS product label based on the build number
                if (osBuildNames.ContainsKey(buildNumber)) // This is a recognised build number
                    return osBuildNames[buildNumber];

                if (buildNumber == 0) // Something probably went wrong
                    return "Unknown OS version (0)";

                if ((buildNumber > 0) & (buildNumber < 10000))
                    return "Earlier than Windows 10 (build < 10000)";

                if ((buildNumber >= 10000) & (buildNumber < 19041))
                    return $"Windows 10 (build {buildNumber})";

                if (buildNumber > osBuildNames.Keys.Max())
                    return "Windows 11 (later than 24H2)";

                return $"ASCOMLibrary.OSBuildName - Unknown OS build number: {buildNumber}";
            }
            catch (Exception ex)
            {
                return $"ASCOMLibrary.OSBuildName - Exception: {ex.Message}";
            }
        }

        #endregion

        #region Support code

        /// <summary>
        /// Run the Alpaca dynamic client manager application with the supplied parameters
        /// </summary>
        /// <param name="parameterString">Parameter string to pass to the application</param>
        private static void RunDynamicClientManager(string parameterString)
        {
            string clientManagerWorkingDirectory, clientManagerExeFile;
            ProcessStartInfo clientManagerProcessStartInfo;
            Process clientManagerProcess;

            // Construct path to the executable that will dynamically create a new Alpaca COM client
            clientManagerWorkingDirectory = $@"{GetFolderPath(SpecialFolder.ProgramFilesX86)}\{ALPACA_DYNAMIC_CLIENT_MANAGER_RELATIVE_PATH}";
            clientManagerExeFile = $@"{clientManagerWorkingDirectory}\{ALPACA_DYNAMIC_CLIENT_MANAGER_EXE_NAME}";

            LogMessage("RunDynamicClientManager", $"Generator parameters: '{parameterString}'");
            LogMessage("RunDynamicClientManager", $"Managing drivers using the {clientManagerExeFile} executable in working directory {clientManagerWorkingDirectory}");

            if (!File.Exists(clientManagerExeFile))
            {
                LogMessage("RunDynamicClientManager", $"ERROR - Unable to find the client generator executable at {clientManagerExeFile}, cannot create a new Alpaca client.");
                throw new InvalidOperationException($"RunDynamicClientManager - Unable to find the client generator executable at {clientManagerExeFile}, cannot create a new Alpaca client.");
            }

            // Initialise the process complete flag to false
            driverGenerationComplete = false;

            // Set the process run time environment and parameters
            clientManagerProcessStartInfo = new ProcessStartInfo(clientManagerExeFile, parameterString); // Run the executable with no parameters in order to show the management GUI
            clientManagerProcessStartInfo.WorkingDirectory = clientManagerWorkingDirectory;
            clientManagerProcessStartInfo.UseShellExecute = true;
            clientManagerProcessStartInfo.Verb = "runas";

            // Create the management process
            clientManagerProcess = new Process();
            clientManagerProcess.StartInfo = clientManagerProcessStartInfo;
            clientManagerProcess.EnableRaisingEvents = true;
            clientManagerProcess.Exited += new EventHandler(DriverGeneration_Complete);

            // Run the process
            LogMessage("RunDynamicClientManager", $"Starting driver management process");
            clientManagerProcess.Start();

            // Wait for the process to complete at which point the process complete event will fire and driverGenerationComplete will be set true
            do
            {
                Thread.Sleep(10);
            }
            while (!driverGenerationComplete);

            LogMessage("RunDynamicClientManager", $"Completed driver management process");

            clientManagerProcess.Dispose();
        }

        /// <summary>
        /// Driver generation completion event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DriverGeneration_Complete(object sender, System.EventArgs e)
        {
            driverGenerationComplete = true; // Flag that driver generation is complete
        }

        /// <summary>
        /// Test whether the Platform version can be determined.
        /// </summary>
        /// <exception cref="InvalidOperationException">When the Platform version cannot be determined.</exception>
        private static void CheckPlatformVersionIsOk()
        {
            if (platformVersion is null) throw new InvalidOperationException("The Platform version number could not be determined, please make sure the ASCOM Platform is installed?");
        }

        /// <summary>
        /// Log a message to the screen, adding the current managed thread ID
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="message"></param>
        private static void LogMessage(string methodName, string message)
        {
            logger.LogMessage(LogLevel.Information, $"PlatformUtilities - {methodName}", message);
        }

        #endregion
    }
}
