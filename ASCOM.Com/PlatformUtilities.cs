using ASCOM.Tools;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Com
{
    /// <summary>
    /// Utilities relevant to Windows / COM Driver / ASCOM Platform development
    /// </summary>
    public static class PlatformUtilities
    {
        const int MINIMUM_VALID_PLATFORM_VERSION = 1;
        const int MAXIMUM_VALID_PLATFORM_VERSION = 6;
        const int MINIMUM_VALID_MINOR_VERSION = 0;
        const int MAXIMUM_VALID_MINOR_VERSION = 6;
        const int MINIMUM_VALID_SERVICEPACK_VERSION = 0;
        const int MAXIMUM_VALID_SERVICEPACK_VERSION = 3;
        const int MINIMUM_BUILD_NUMBER = 3000;
        const int MAXIMUM_BUILD_NUMBER = 65535;

        const string PROFILE_ROOT_KEY = "SOFTWARE\\ASCOM";

        private static Version platformVersion = null;
        private static readonly TraceLogger TL;

        #region Initialise

        static PlatformUtilities()
        {
            string platformVersionString;
            TL = new TraceLogger("PlatformUtilities", true);

            // Populate the Platform version variable when the class is initialised

            // Get the detailed (four part) Platform version from the "Platform Version" value from the Profile's "Platform" key.
            try
            {
                using (RegistryKey localmachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    RegistryKey driverKey = localmachine32.OpenSubKey($"{PROFILE_ROOT_KEY}\\Platform", false);
                    platformVersionString = (string)driverKey.GetValue("Platform Version");
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
                        platformVersionString = (string)driverKey.GetValue("PlatformVersion");
                    }
                }
                catch (Exception ex)
                {
                    // Not able to get the original Platform version either so throw an exception
                    throw new ValueNotSetException("ComUtilities.IsMinimumRequiredVersion - Unable to read the Platform version number. Is the ASCOM Platform installed? See inner exception for details.", ex);
                }
            }

            platformVersion = new Version(platformVersionString);

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

            TL.LogMessage("IsMinimumRequiredVersion", $"platformVersion: {platformVersion}, Required version: {requiredVersion}");

            // Compare the two versions and respond accordingly
            if (platformVersion >= requiredVersion)
                return true; // Platform version is equal to or greater than the required version
            else
                return false;// Platform version is less than the required version
        }

        #endregion

        #region CreateDynamicDriver



        #endregion

        #region Support code

        private static void CheckPlatformVersionIsOk()
        {
            if (platformVersion is null) throw new InvalidOperationException("The Platform version could not be determined, is the ASCOM Platform installed?");
        }

        #endregion
    }
}
