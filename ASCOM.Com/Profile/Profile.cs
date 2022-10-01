using Microsoft.Win32;
using System;
using System.Collections.Generic;
using ASCOM.Common;

namespace ASCOM.Com
{
    /// <summary>
    /// ASCOM Profile utilities
    /// </summary>
    public class Profile : IDisposable
    {
        #region Profile information

        /// <summary>
        /// Searches the ASCOM Registry for all drivers of a specified driver type
        /// </summary>
        /// <param name="deviceType">The driver type to search for as a DriverType.</param>
        /// <returns>Returns a list of found ASCOM Devices, this includes ProgID and the friendly Name</returns>
        public static List<ASCOMRegistration> GetDrivers(DeviceTypes deviceType)
        {
            List<ASCOMRegistration> Drivers = new List<ASCOMRegistration>();

            using (var localmachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                                            RegistryView.Registry32))
            {
                using (var ASCOMKeys = localmachine32.OpenSubKey($"SOFTWARE\\ASCOM\\{Devices.DeviceTypeToString(deviceType)} Drivers", false))
                {
                    foreach (var key in ASCOMKeys.GetSubKeyNames())
                    {
                        string name = string.Empty;
                        using (var DriverKey = ASCOMKeys.OpenSubKey(key, false))
                        {
                            foreach (var subkey in DriverKey.GetValueNames())
                            {
                                if (subkey == string.Empty)
                                {
                                    name = DriverKey.GetValue(subkey).ToString();
                                }
                            }

                            if (name != string.Empty)
                            {
                                Drivers.Add(new ASCOMRegistration(key, name));
                            }
                        }
                    }
                }
            }

            return Drivers;
        }

        #endregion

        #region Profile registration

        /// <summary>
        /// Register an ASCOM device in the profile
        /// </summary>
        /// <param name="deviceType">ASCOM device type e.g. Telescope.</param>
        /// <param name="progId">COM ProgID of the driver</param>
        /// <param name="description">Device description that will appear in the Chooser list.</param>
        /// <exception cref="InvalidValueException">If the ASCOM device type or COM progId are null, empty or just contain white space.</exception>
        /// <exception cref="InvalidValueException">If the device is not a valid ASCOM device type.</exception>
        public static void Register(DeviceTypes deviceType, string progId, string description)
        {
            // Validate parameters
            if (!Devices.IsValidDeviceType(deviceType)) throw new InvalidValueException($"Profile.Register - Device type {deviceType} is not a valid device type.");
            if (string.IsNullOrWhiteSpace(progId)) throw new InvalidValueException("Profile.Register - Supplied ProgId is null or empty.");
            if (string.IsNullOrEmpty(description)) throw new InvalidValueException("Profile.Register - Supplied description is null or empty.");

            // Open the local machine hive in 32bit mode
            using (RegistryKey localmachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                RegistryKey driverKey = localmachine32.CreateSubKey($"SOFTWARE\\ASCOM\\{deviceType} Drivers\\{progId}", true);
                driverKey.SetValue(null, description);
            }
        }

        /// <summary>
        /// Unregister an ASCOM device from the Profile.
        /// </summary>
        /// <param name="deviceType">ASCOM device type e.g. Telescope.</param>
        /// <param name="progId">COM ProgID of the driver</param>
        /// <exception cref="InvalidValueException">If the ASCOM device type or COM progId are null, empty or just contain white space.</exception>
        /// <exception cref="InvalidValueException">If the device type is not a valid ASCOM device type.</exception>
        /// <remarks>This method will succeed (no exception will be thrown) regardless of whether or not the device is registered.</remarks>
        public static void UnRegister(DeviceTypes deviceType, string progId)
        {
            // Validate parameters
            if (!Devices.IsValidDeviceType(deviceType)) throw new InvalidValueException($"UnRegister.Register - Device type {deviceType} is not a valid device type.");
            if (string.IsNullOrWhiteSpace(progId)) throw new InvalidValueException("Profile.UnRegister - Supplied ProgId is null or empty.");

            // Open the local machine hive in 32bit mode
            using (RegistryKey localmachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                localmachine32.DeleteSubKeyTree($"SOFTWARE\\ASCOM\\{deviceType} Drivers\\{progId}", false);
            }
        }

        /// <summary>
        /// Tests whether a given device is registered in the ASCOM Profile.
        /// </summary>
        /// <param name="deviceType">ASCOM device type e.g. Telescope.</param>
        /// <param name="progId">COM ProgID of the driver</param>
        /// <returns><see langword="true"/> if the device is registered, otherwise returns <see langword="false"/>.</returns>
        /// <exception cref="InvalidValueException">If the ASCOM device type or COM progId are null, empty or just contain white space.</exception>
        /// <exception cref="InvalidValueException">If the device type is not a valid ASCOM device type.</exception>
        public static bool IsRegistered(DeviceTypes deviceType, string progId)
        {
            // Validate parameters
            if (!Devices.IsValidDeviceType(deviceType)) throw new InvalidValueException($"Profile.IsRegistered - Device type {deviceType} is not a valid device type.");
            if (string.IsNullOrWhiteSpace(progId)) throw new InvalidValueException("Profile.IsRegistered - Supplied ProgId is null or empty.");

            // Assume failure
            bool returnValue = false;

            // Confirm that the specified driver is registered
            string[] keys;

            // Open the local machine hive in 32bit mode
            using (RegistryKey localmachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                // Open the ASCOM device type root key
                using (RegistryKey driversKey = localmachine32.OpenSubKey($"SOFTWARE\\ASCOM\\{deviceType} Drivers", false))
                {
                    // If the device type is not present return default false value
                    if (!(driversKey is null))
                    {
                        keys = driversKey.GetSubKeyNames();

                        // Iterate through all returned driver names comparing them to the required driver name, set a flag if the required driver is present
                        foreach (string id in keys) // Platform 6 version - makes the test case insensitive! ASCOM-235
                        {
                            if (id.ToUpperInvariant() == progId.ToUpperInvariant())
                            {
                                returnValue = true; // Found it
                            }
                        }
                    }
                }
            }
            return returnValue;
        }

        #endregion

        #region Get Value

        /// <summary>
        /// Reads a value from the root of the device's registry Profile.
        /// </summary>
        /// <param name="deviceType">Device type e.g. Telescope.</param>
        /// <param name="progId">COM ProgID of the device.</param>
        /// <param name="valueName">Name of this parameter.</param>
        /// <param name="defaultValue">Default value to be returned if this parameter has not yet been set. The default value will also be written to the Profile.</param>
        /// <returns>String value of the specified parameter.</returns>
        /// <exception cref="InvalidValueException">If the ASCOM device type or COM progId are null, empty or just contain white space.</exception>
        /// <exception cref="InvalidValueException">If the device is not registered or is not a valid ASCOM device type.</exception>
        /// <exception cref="ValueNotSetException">The requested parameter has not been set and no default value was provided.</exception>
        public static string GetValue(DeviceTypes deviceType, string progId, string valueName, string defaultValue)
        {
            return GetValue(deviceType, progId, valueName, defaultValue, null);
        }

        /// <summary>
        /// Reads a value from the given sub-key in the device's registry Profile.
        /// </summary>
        /// <param name="deviceType">Device type e.g. Telescope.</param>
        /// <param name="progId">COM ProgID of the device.</param>
        /// <param name="valueName">Name of this parameter.</param>
        /// <param name="defaultValue">If the requested value does not exist, this value will be written to the profile and returned to the caller. Use null to indicate that there is no default value</param>
        /// <param name="subKey">Name of the sub-key under which to read this value. Use null or empty string to write to the device's profile root.</param>
        /// <returns>String value of the specified parameter.</returns>
        /// <exception cref="InvalidValueException">If the ASCOM device type or COM progId are null, empty or just contain white space.</exception>
        /// <exception cref="InvalidValueException">If the device is not registered or is not a valid ASCOM device type.</exception>
        /// <exception cref="ValueNotSetException">The requested parameter has not been set and the supplied default value is null.</exception>
        public static string GetValue(DeviceTypes deviceType, string progId, string valueName, string defaultValue, string subKey)
        {
            object returnValue = null;

            // Validate parameters
            if (!Devices.IsValidDeviceType(deviceType)) throw new InvalidValueException($"Profile.GetValue - Device type {deviceType} is not a valid device type.");
            if (string.IsNullOrWhiteSpace(progId)) throw new InvalidValueException("Profile.GetValue - Supplied ProgId is null or empty.");

            // Confirm that the specified driver is registered
            if (!IsRegistered(deviceType, progId)) throw new InvalidValueException($"Profile.GetValue - Device {progId} is not registered.");

            // Name can be null or empty or a text string but must not comprise only of white space
            if (!(valueName is null))
            {
                if (string.IsNullOrWhiteSpace(valueName) & (valueName.Length > 0)) throw new InvalidValueException("Profile.GetValue - Supplied value name comprises only white space.");
            }

            // Sub-key can be null or empty or a text string but must not comprise only of white space
            if (!(subKey is null))
            {
                if (string.IsNullOrWhiteSpace(subKey) & (subKey.Length > 0)) throw new InvalidValueException("Profile.GetValue - Supplied sub-key name -comprises only white space.");
            }

            string registryKeyName;

            // Create the registry key name with or without the sub-key name as required 
            if (string.IsNullOrEmpty(subKey)) // Device's profile root
            {
                registryKeyName = $"SOFTWARE\\ASCOM\\{deviceType} Drivers\\{progId}";
            }
            else // Sub-key under the device's profile root.
            {
                registryKeyName = $"SOFTWARE\\ASCOM\\{deviceType} Drivers\\{progId}\\{subKey}";
            }

            // Open the local machine hive in 32bit mode
            using (RegistryKey localmachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                // Open the required registry key
                using (RegistryKey driversKey = localmachine32.OpenSubKey(registryKeyName, false))
                {
                    if (!(driversKey == null)) returnValue = driversKey.GetValue(valueName);
                }
            }

            // Handle the value not found condition
            if (returnValue == null) // No value was found. (Note: An empty string is a valid value hence use of the "returnValue == null" test condition
            {
                if (defaultValue == null) // No default was supplied so throw an exception saying that the value was not found. (See note above.)
                {
                    throw new ValueNotSetException($"Profile.GetValue - Attempt to read the {valueName} value in {registryKeyName} before it has been set.");
                }
                else // A default was supplied so write it to the registry and return it
                {
                    SetValue(deviceType, progId, valueName, defaultValue, subKey);
                    returnValue = defaultValue;
                }
            }

            // Return the value read from the registry
            return (string)returnValue;
        }

        #endregion

        #region Get Values

        /// <summary>
        /// Returns a dictionary of the named and unnamed values in the device's profile root. 
        /// </summary>
        /// <param name="deviceType">Device type e.g. Telescope.</param>
        /// <param name="progId">COM ProgID of the device.</param>
        /// <returns>Dictionary of name:value string pairs.</returns>
        /// <exception cref="InvalidValueException">If the ASCOM device type or COM progId are null, empty or just contain white space.</exception>
        /// <exception cref="InvalidValueException">If the device is not registered or is not a valid ASCOM device type.</exception>
        public static Dictionary<string, string> GetValues(DeviceTypes deviceType, string progId)
        {
            return GetValues(deviceType, progId, null);
        }

        /// <summary>
        /// Returns a dictionary of the named and unnamed values under the given sub-key from the device's profile root. 
        /// </summary>
        /// <param name="deviceType">Device type e.g. Telescope.</param>
        /// <param name="progId">COM ProgID of the device.</param>
        /// <param name="subKey">Name of the sub-key from which to read the values.</param>
        /// <returns>Dictionary of name:value string pairs.</returns>
        /// <exception cref="InvalidValueException">If the ASCOM device type or COM progId are null, empty or just contain white space.</exception>
        /// <exception cref="InvalidValueException">If the device is not registered or is not a valid ASCOM device type.</exception>
        /// <exception cref="InvalidValueException">If the sub-key only contains white space.</exception>
        public static Dictionary<string, string> GetValues(DeviceTypes deviceType, string progId, string subKey)
        {
            Dictionary<string, string> returnValue = new Dictionary<string, string>();

            // Validate parameters
            if (string.IsNullOrWhiteSpace(progId)) throw new InvalidValueException("Profile.GetValues - Supplied ProgId is null or empty.");
            if (!Devices.IsValidDeviceType(deviceType)) throw new InvalidValueException($"Profile.GetValues - Device type {deviceType} is not a valid device type.");

            // Confirm that the specified driver is registered
            if (!IsRegistered(deviceType, progId)) throw new InvalidValueException($"Profile.GetValues - Device {progId} is not registered.");

            // Sub-key can be null or empty or a text string but must not comprise only of white space
            if (!(subKey is null))
            {
                if (string.IsNullOrWhiteSpace(subKey) & (subKey.Length > 0)) throw new InvalidValueException("Profile.GetValues - Supplied sub-key name -comprises only white space.");
            }
            string registryKeyName;

            // Create the registry key name with or without the sub-key name as required 
            if (string.IsNullOrEmpty(subKey)) // Device's profile root
            {
                registryKeyName = $"SOFTWARE\\ASCOM\\{deviceType} Drivers\\{progId}";
            }
            else // Sub-key under the device's profile root.
            {
                registryKeyName = $"SOFTWARE\\ASCOM\\{deviceType} Drivers\\{progId}\\{subKey}";
            }

            // Open the local machine hive in 32bit mode
            using (RegistryKey localmachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                // Open the required registry key
                using (RegistryKey profileKey = localmachine32.OpenSubKey(registryKeyName, false))
                {
                    if (!(profileKey == null))
                    {
                        string[] valueNames = profileKey.GetValueNames();
                        foreach (string valueName in valueNames)
                        {
                            returnValue.Add(valueName, profileKey.GetValue(valueName).ToString());
                        }
                    }
                }
            }

            return returnValue;
        }

        #endregion

        #region Get SubKeys

        /// <summary>
        /// Returns a list of the sub-keys under the profile root.
        /// </summary>
        /// <param name="deviceType">Device type e.g. Telescope.</param>
        /// <param name="progId">COM ProgID of the device.</param>
        /// <returns>Dictionary of name:value string pairs.</returns>
        /// <exception cref="InvalidValueException">If device type, progId or sub-key are null or invalid.</exception>
        public static List<string> GetSubKeys(DeviceTypes deviceType, string progId)
        {
            return GetSubKeys(deviceType, progId, null);
        }

        /// <summary>
        /// Returns a list of the sub-keys under the given sub-key from the profile root. 
        /// </summary>
        /// <param name="deviceType">Device type e.g. Telescope.</param>
        /// <param name="progId">COM ProgID of the device.</param>
        /// <param name="subKey">Name of the sub-key from which to read the sub-keys.</param>
        /// <returns>Dictionary of name:value string pairs.</returns>
        /// <exception cref="InvalidValueException">If the ASCOM device type or COM progId are null, empty or just contain white space.</exception>
        /// <exception cref="InvalidValueException">If the device is not registered or is not a valid ASCOM device type.</exception>
        /// <exception cref="InvalidValueException">If the sub-key only contains white space.</exception>
        public static List<string> GetSubKeys(DeviceTypes deviceType, string progId, string subKey)
        {
            List<string> returnValue = new List<string>();

            // Validate parameters
            if (!Devices.IsValidDeviceType(deviceType)) throw new InvalidValueException($"Profile.GetSubKeys - Device type {deviceType} is not a valid device type.");
            if (string.IsNullOrWhiteSpace(progId)) throw new InvalidValueException("Profile.GetSubKeys - Supplied ProgId is null or empty.");

            // Confirm that the specified driver is registered
            if (!IsRegistered(deviceType, progId)) throw new InvalidValueException($"Profile.GetSubKeys - Device {progId} is not registered.");

            // Sub-key can be null or empty or a text string but must not comprise only of white space
            if (!(subKey is null))
            {
                if (string.IsNullOrWhiteSpace(subKey) & (subKey.Length > 0)) throw new InvalidValueException("Profile.GetSubKeys - Supplied sub-key name -comprises only white space.");
            }
            string registryKeyName;

            // Create the registry key name with or without the sub-key name as required 
            if (string.IsNullOrEmpty(subKey)) // Device's profile root
            {
                registryKeyName = $"SOFTWARE\\ASCOM\\{deviceType} Drivers\\{progId}";
            }
            else // Sub-key under the device's profile root.
            {
                registryKeyName = $"SOFTWARE\\ASCOM\\{deviceType} Drivers\\{progId}\\{subKey}";
            }

            // Open the local machine hive in 32bit mode
            using (RegistryKey localmachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                // Open the required registry key
                using (RegistryKey profileKey = localmachine32.OpenSubKey(registryKeyName, false))
                {
                    if (!(profileKey == null))
                    {
                        string[] subKeyNames = profileKey.GetSubKeyNames();
                        foreach (string subKeyName in subKeyNames)
                        {
                            returnValue.Add(subKeyName);
                        }
                    }
                }
            }

            return returnValue;
        }

        #endregion

        #region Set Value
        /// <summary>
        /// Set a value in the device's registry Profile.
        /// </summary>
        /// <param name="deviceType">Device type e.g. Telescope.</param>
        /// <param name="progId">COM ProgID of the device.</param>
        /// <param name="valueName">Name of this parameter.</param>
        /// <param name="value">Value to be set.</param>
        /// <exception cref="InvalidValueException">If device type, progId, name, value or sub-key are null or invalid.</exception>
        public static void SetValue(DeviceTypes deviceType, string progId, string valueName, string value)
        {
            SetValue(deviceType, progId, valueName, value, null);
        }

        /// <summary>
        /// Set a value in the device's registry Profile.
        /// </summary>
        /// <param name="deviceType">Device type e.g. Telescope.</param>
        /// <param name="progId">COM ProgID of the device.</param>
        /// <param name="valueName">Name of this parameter.</param>
        /// <param name="value">Value to be set.</param>
        /// <param name="subkey">Name of the sub-key under which to place this value.</param>
        /// <exception cref="InvalidValueException">If device type, progId, name or value are null or invalid.</exception>
        /// <exception cref="InvalidValueException">If the sub-key only contains white space.</exception>
        public static void SetValue(DeviceTypes deviceType, string progId, string valueName, string value, string subkey)
        {
            // Validate parameters
            if (!Devices.IsValidDeviceType(deviceType)) throw new InvalidValueException($"Profile.SetValue - Device type {deviceType} is not a valid device type.");
            if (string.IsNullOrWhiteSpace(progId)) throw new InvalidValueException("Profile.SetValue - Supplied ProgId is null or empty.");

            // Confirm that the specified driver is registered
            if (!IsRegistered(deviceType, progId)) throw new InvalidValueException($"Profile.SetValue - Device {progId} is not registered.");

            // Name can be null or empty or a text string but must not comprise only of white space
            if (!(valueName is null))
            {
                if (string.IsNullOrWhiteSpace(valueName) & (valueName.Length > 0)) throw new InvalidValueException("Profile.SetValue - Supplied value name comprises only white space.");
            }

            // The sub-key can be null or empty or a text string but must not comprise only of white space
            if (!(subkey is null))
            {
                if (string.IsNullOrWhiteSpace(subkey) & (subkey.Length > 0)) throw new InvalidValueException("Profile.SetValue - Supplied sub-key name -comprises only white space.");
            }

            // The value must not be null
            if (value is null) throw new InvalidValueException("Profile.SetValue - Supplied value is null, it must be at least an empty string.");

            string registryKeyName;

            // Create the registry key name with or without the sub-key name as required 
            if (string.IsNullOrEmpty(subkey)) // Use the device's profile root
            {
                registryKeyName = $"SOFTWARE\\ASCOM\\{deviceType} Drivers\\{progId}";
            }
            else // Use the sub-key under the device's profile root.
            {
                registryKeyName = $"SOFTWARE\\ASCOM\\{deviceType} Drivers\\{progId}\\{subkey}";
            }

            // Open the local machine hive in 32bit mode
            using (RegistryKey localmachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                // Open the required registry key in write mode
                using (RegistryKey driversKey = localmachine32.CreateSubKey(registryKeyName, true))
                {
                    driversKey.SetValue(valueName, value);
                }
            }
        }

        #endregion

        #region Delete Value

        /// <summary>
        /// Deletes a value from the root of the device's registry Profile.
        /// </summary>
        /// <param name="deviceType">Device type e.g. Telescope.</param>
        /// <param name="progId">COM ProgID of the device.</param>
        /// <param name="valueName">Name of this parameter.</param>
        /// <exception cref="InvalidValueException">If device type, progId, name, value or sub-key are null or invalid.</exception>
        public static void DeleteValue(DeviceTypes deviceType, string progId, string valueName)
        {
            DeleteValue(deviceType, progId, valueName, null);
        }

        /// <summary>
        /// Deletes a value from the given sub-key in the device's registry Profile.
        /// </summary>
        /// <param name="deviceType">Device type e.g. Telescope.</param>
        /// <param name="progId">COM ProgID of the device.</param>
        /// <param name="valueName">Name of this parameter.</param>
        /// <param name="subKey">Name of the sub-key under which to read this value.</param>
        /// <exception cref="InvalidValueException">If device type, progId, name or value are null or invalid.</exception>
        /// <exception cref="InvalidValueException">If the sub-key only contains white space.</exception>
        public static void DeleteValue(DeviceTypes deviceType, string progId, string valueName, string subKey)
        {
            // Validate parameters
            if (!Devices.IsValidDeviceType(deviceType)) throw new InvalidValueException($"Profile.DeleteValue - Device type {deviceType} is not a valid device type.");
            if (string.IsNullOrWhiteSpace(progId)) throw new InvalidValueException("Profile.DeleteValue - Supplied ProgId is null or empty.");

            // Confirm that the specified driver is registered
            if (!IsRegistered(deviceType, progId)) throw new InvalidValueException($"Profile.DeleteValue - Device {progId} is not registered.");

            // Name can be null or empty or a text string but must not comprise only of white space
            if (!(valueName is null))
            {
                if (string.IsNullOrWhiteSpace(valueName) & (valueName.Length > 0)) throw new InvalidValueException("Profile.DeleteValue - Supplied value name comprises only white space.");
            }

            // Sub-key can be null or empty or a text string but must not comprise only of white space
            if (!(subKey is null))
            {
                if (string.IsNullOrWhiteSpace(subKey) & (subKey.Length > 0)) throw new InvalidValueException("Profile.DeleteValue - Supplied sub-key name -comprises only white space.");
            }

            string registryKeyName;

            // Create the registry key name with or without the sub-key name as required 
            if (string.IsNullOrEmpty(subKey)) // Device's profile root
            {
                registryKeyName = $"SOFTWARE\\ASCOM\\{deviceType} Drivers\\{progId}";
            }
            else // Sub-key under the device's profile root.
            {
                registryKeyName = $"SOFTWARE\\ASCOM\\{deviceType} Drivers\\{progId}\\{subKey}";
            }

            // Open the local machine hive in 32bit mode
            using (RegistryKey localmachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                // Open the required registry key
                using (RegistryKey profileKey = localmachine32.OpenSubKey(registryKeyName, true))
                {
                    if (profileKey != null) profileKey.DeleteValue(valueName, false); // Delete but do not throw an exception if the value is not present.
                }
            }
        }

        #endregion

        #region Create and Delete SubKeys

        /// <summary>
        /// Creates a sub-key under the device's Profile root. Nested sub-keys can be created by separating levels with "\" characters in the sub-key string.
        /// </summary>
        /// <param name="deviceType">Device type e.g. Telescope.</param>
        /// <param name="progId">COM ProgID of the device.</param>
        /// <param name="subKey">Name of the sub-key to create based from the device's profile root.</param>
        /// <exception cref="InvalidValueException">If device type or progId are null or invalid.</exception>
        /// <exception cref="InvalidValueException">If the sub-key only contains white space.</exception>
        /// <exception cref="InvalidOperationException">If the Device's Profile root key cannot be opened for writing.</exception>
        public static void CreateSubKey(DeviceTypes deviceType, string progId, string subKey)
        {
            // Validate parameters
            if (!Devices.IsValidDeviceType(deviceType)) throw new InvalidValueException($"Profile.CreateSubKey - Device type {deviceType} is not a valid device type.");
            if (string.IsNullOrWhiteSpace(progId)) throw new InvalidValueException("Profile.CreateSubKey - Supplied ProgId is null or empty.");
            // Confirm that the specified driver is registered
            if (!IsRegistered(deviceType, progId)) throw new InvalidValueException($"Profile.CreateSubKey - Device {progId} is not registered.");

            if (string.IsNullOrWhiteSpace(subKey)) throw new InvalidValueException($"Profile.CreateSubKey - Supplied sub-key name is null or white space.");

            // Create the device's root registry key 
            string registryKeyName = $"SOFTWARE\\ASCOM\\{deviceType} Drivers\\{progId}";

            // Open the local machine hive in 32bit mode
            using (RegistryKey localmachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                // Open the device's root registry key
                using (RegistryKey profileKey = localmachine32.OpenSubKey(registryKeyName, true))
                {
                    if (profileKey != null)
                    {
                        RegistryKey subKeyKey = profileKey.CreateSubKey(subKey);
                        subKeyKey.Dispose();
                    }
                    else
                    {
                        throw new InvalidOperationException($"Profile.CreateSubKey - Unable to open the device's root profile key: {registryKeyName}");
                    }
                }
            }
        }

        /// <summary>
        /// Deletes a sub-key under the device's Profile root. Nested sub-keys can be deleted by separating levels with "\" characters in the sub-key string.
        /// </summary>
        /// <param name="deviceType">Device type e.g. Telescope.</param>
        /// <param name="progId">COM ProgID of the device.</param>
        /// <param name="subKey">Name of the sub-key to delete based on the device's Profile root.</param>
        /// <exception cref="InvalidValueException">If device type or progId are null or invalid.</exception>
        /// <exception cref="InvalidValueException">If the sub-key only contains white space.</exception>
        /// <exception cref="InvalidOperationException">If the Device's Profile root key cannot be opened for writing.</exception>
        public static void DeleteSubKey(DeviceTypes deviceType, string progId, string subKey)
        {
            // Validate parameters
            if (!Devices.IsValidDeviceType(deviceType)) throw new InvalidValueException($"Profile.DeleteSubKey - Device type {deviceType} is not a valid device type.");
            if (string.IsNullOrWhiteSpace(progId)) throw new InvalidValueException("Profile.DeleteSubKey - Supplied ProgId is null or empty.");
            // Confirm that the specified driver is registered
            if (!IsRegistered(deviceType, progId)) throw new InvalidValueException($"Profile.DeleteSubKey - Device {progId} is not registered.");

            if (string.IsNullOrWhiteSpace(subKey)) throw new InvalidValueException($"Profile.DeleteSubKey - Supplied sub-key name is null or white space.");

            // Create the device's root registry key 
            string registryKeyName = $"SOFTWARE\\ASCOM\\{deviceType} Drivers\\{progId}";

            // Open the local machine hive in 32bit mode
            using (RegistryKey localmachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                // Open the device's root registry key
                using (RegistryKey profileKey = localmachine32.OpenSubKey(registryKeyName, true))
                {
                    if (profileKey != null)
                    {
                        profileKey.DeleteSubKeyTree(subKey, false);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Profile.DeleteSubKey - Unable to open the device's root profile key: {registryKeyName}");
                    }
                }
            }
        }

        #endregion

        #region Private support code


        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Currently no resources to be disposed.
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose of the Profile object
        /// </summary>
        /// <remarks>This method is present to implement the IDisposable pattern, which enables the Profile component to be referenced within a Using statement.</remarks>
        public void Dispose()
        {
            // Do not change this code. Put clean-up code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

    }
}
