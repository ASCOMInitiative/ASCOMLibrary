using System;
using System.Collections.Generic;
using System.Linq;

namespace ASCOM.Common.Devices
{
    public static class Devices
    {
        // List of ASCOM device types
        private static readonly IEnumerable<string> deviceTypes = new List<string>() {
            DeviceTypes.Camera.ToString(),
            DeviceTypes.CoverCalibrator.ToString(),
            DeviceTypes.Dome.ToString(),
            DeviceTypes.FilterWheel.ToString(),
            DeviceTypes.Focuser.ToString(),
            DeviceTypes.ObservingConditions.ToString(),
            DeviceTypes.Rotator.ToString(),
            DeviceTypes.SafetyMonitor.ToString(),
            DeviceTypes.Switch.ToString(),
            DeviceTypes.Telescope.ToString(),
            DeviceTypes.Video.ToString()
            };

        /// <summary>
        /// Returns a list of valid ASCOM device type names
        /// </summary>
        /// <returns>List of ASCOM device types.</returns>
        public static List<string> DeviceTypeNames()
        {
            return deviceTypes.ToList();
        }

        /// <summary>
        /// Confirms that the supplied device type name is a valid ASCOM device type.
        /// </summary>
        /// <param name="deviceType">Device type name to assess.</param>
        /// <returns>Returns true if the supplied name is a valid ASCOM device type, otherwise returns false.</returns>
        public static bool IsValidDeviceType(string deviceType)
        {
            return DeviceTypeNames().Contains<string>(deviceType, StringComparer.OrdinalIgnoreCase);
        }
    }
}
