using ASCOM.Common;
using Microsoft.Win32;
using System;
using System.Threading;

namespace ASCOM.Alpaca.Tests.Profile
{
    internal static class Test
    {
        static Random random = new Random();

        public const int BAD_DEVICE_TYPE_VALUE = 99;
        public const DeviceTypes TEST_DEVICE_TYPE = DeviceTypes.Camera;
        public const string TEST_DESCRIPTION = "Test description";
        public const string TEST_SUBKEY1 = "TestSubkey1";
        public const string TEST_SUBKEY2 = "TestSubkey2";
        public const string TEST_SUBKEY3 = "TestSubkey3";
        public const string TEST_SUBKEY4 = "TestSubkey4";
        public const string TEST_VALUE_NAME1 = "Test Value 1";
        public const string TEST_VALUE1 = "Contents of test value 1";
        public const string TEST_VALUE_NAME2 = "Test Value 2";
        public const string TEST_VALUE2 = "Contents of test value 2";
        public const string TEST_VALUE_NAME3 = "Test Value 3";
        public const string TEST_VALUE3 = "Contents of test value 3";
        public const string TEST_VALUE_NAME4 = "Test Value 4";
        public const string TEST_VALUE4 = "Contents of test value 4";

        public static Mutex TestMutex = new Mutex(false, "ProfileTestMutex");

        #region Support Code

        /// <summary>
        /// Create a random ProgId of the form TestDeviceXXXXXXX.YYYYYYYY where XXXXXXX is a random integer between 1,000,000 and 9,999,999 and YYYYYYYYY is the supplied device type.
        /// </summary>
        /// <param name="deviceType">The ASCOM device type for the device.</param>
        /// <returns>Random ProgId string.</returns>
        public static string GetProgId(DeviceTypes deviceType)
        {
            return $"TestDevice{random.Next(1000000, 10000000)}.{deviceType}";
        }

        public static void ClearDeviceRegistration(string progId)
        {
            // Open the local machine hive in 32bit mode
            using (RegistryKey localmachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                localmachine32.DeleteSubKeyTree($"SOFTWARE\\ASCOM\\{Test.TEST_DEVICE_TYPE} Drivers\\{progId}", false);
            }
        }

        /// <summary>
        /// Reads a value from the device Profile specified by the supplied parameters 
        /// </summary>
        /// <param name="deviceType">ASCOM device type e.g. Telescope.</param>
        /// <param name="progId">Device ProgId</param>
        /// <param name="subKey">Sub-key to read from.</param>
        /// <param name="valueName">Name of the value to be read.</param>
        /// <returns>String value.</returns>
        public static string ReadTestValue(DeviceTypes deviceType, string progId, string subKey, string valueName)
        {
            object registryValue = null;

            // Open the local machine hive in 32bit mode
            using (RegistryKey localmachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                // Open the required registry key
                using (RegistryKey driversKey = localmachine32.OpenSubKey($"SOFTWARE\\ASCOM\\{deviceType} Drivers\\{progId}\\{subKey}", false))
                {
                    if (driversKey != null) registryValue = driversKey.GetValue(valueName);
                }
            }

            if (registryValue is null) return null;

            return (string)registryValue;
        }

        #endregion
    }

}
