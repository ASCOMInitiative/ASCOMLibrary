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
        /// Confirms that the supplied device type name is a valid ASCOM device type.
        /// </summary>
        /// <param name="deviceType">Device type name to assess.</param>
        /// <returns>Returns true if the supplied name is a valid ASCOM device type, otherwise returns false.</returns>
        public static bool IsValidDeviceType(DeviceTypes deviceType)
        {
            return Enum.IsDefined(typeof(DeviceTypes), deviceType);
        }
    }
}
