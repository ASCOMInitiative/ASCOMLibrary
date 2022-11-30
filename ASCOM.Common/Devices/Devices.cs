using System;
using System.Collections.Generic;
using System.Linq;

namespace ASCOM.Common
{
    /// <summary>
    /// Device type name functions
    /// </summary>
    public static class Devices
    {
        /// <summary>
        /// Returns a list of valid ASCOM device type names
        /// </summary>
        /// <returns>String list of ASCOM device types.</returns>
        public static List<string> DeviceTypeNames()
        {
            List<String> deviceNames = new List<string>();
            foreach (string deviceName in Enum.GetNames(typeof(DeviceTypes)))
            {
                deviceNames.Add(deviceName);
            }

            return deviceNames;
        }

        /// <summary>
        /// Confirms that the supplied device type is a valid ASCOM device type.
        /// </summary>
        /// <param name="deviceType">Device type name to assess.</param>
        /// <returns>Returns true if the supplied type is a valid ASCOM device type, otherwise returns false.</returns>
        public static bool IsValidDeviceType(DeviceTypes deviceType)
        {
            return Enum.IsDefined(typeof(DeviceTypes), deviceType);
        }

        /// <summary>
        /// Confirms that the supplied device type string name is valid
        /// </summary>
        /// <param name="deviceTypeName">Device type name as a string</param>
        /// <returns>True if the supplied name is a valid ASCOM device type, otherwise returns false.</returns>
        public static bool IsValidDeviceTypeName(string deviceTypeName)
        {
            try
            {
                DeviceTypes deviceType = StringToDeviceType(deviceTypeName);
                return true;
            }
            catch (InvalidValueException)
            {
                return false;
            }
        }

        /// <summary>
        /// Convert a string device name to a <see cref="DeviceTypes"/> enum value.
        /// </summary>
        /// <param name="device">Device type</param>
        /// <returns>DeviceTypes enum value corresponding to the given string device name</returns>
        /// <exception cref="InvalidValueException">If the supplied device type is not valid.</exception>
        public static DeviceTypes StringToDeviceType(string device)
        {
            // Validate the supplied string device name
            if (Enum.TryParse<DeviceTypes>(device, true, out DeviceTypes deviceType))
            {
                return deviceType; // OK
            }

            // Bad value to return an exception
            throw new InvalidValueException($"Devices.StringToDeviceType - Device type: {device} is not an ASCOM device type.");
        }

        /// <summary>
        /// Convert a <see cref="DeviceTypes"/> enum value to a string
        /// </summary>
        /// <param name="deviceType">Device type</param>
        /// <returns>String device type name corresponding to the given DeviceTypes enum value</returns>
        /// <exception cref="InvalidValueException">If the supplied device type is not valid.</exception>
        public static string DeviceTypeToString(DeviceTypes deviceType)
        {
            string deviceTypeString= Enum.GetName(typeof(DeviceTypes), deviceType);

            // Validate the supplied DeviceTypes enum value
            if (deviceTypeString != null) return deviceTypeString; // OK

            // Bad value to return an exception
            throw new InvalidValueException($"Devices.DeviceTypeToString - Supplied DeviceTypes enum value {(int) deviceType} is not a valid member of the DeviceTypes enum.");
        }
    }
}
