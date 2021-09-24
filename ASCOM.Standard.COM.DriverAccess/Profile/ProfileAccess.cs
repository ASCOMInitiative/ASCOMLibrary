using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace ASCOM.Standard.COM.DriverAccess
{
    /// <summary>
    /// Functions used to find data in the ASCOM Profile which is stored in the Windows Registry.
    /// </summary>
    public class ProfileAccess
    {
        /// <summary>
        /// Converts a DriverTypes enum to the string equivalent
        /// </summary>
        /// <param name="driverType">The requested type</param>
        /// <returns>The equivalent in string form, often used to search the registry</returns>
        public static string DriverString(DriverTypes driverType)
        {
            switch (driverType)
            {
                case DriverTypes.Camera:
                    return "Camera";

                case DriverTypes.CoverCalibrator:
                    return "CoverCalibrator";

                case DriverTypes.Dome:
                    return "Dome";

                case DriverTypes.FilterWheel:
                    return "FilterWheel";

                case DriverTypes.Focuser:
                    return "Focuser";

                case DriverTypes.ObservingConditions:
                    return "ObservingConditions";

                case DriverTypes.Rotator:
                    return "Rotator";

                case DriverTypes.SafetyMonitor:
                    return "SafetyMonitor";

                case DriverTypes.Switch:
                    return "Switch";

                case DriverTypes.Telescope:
                    return "Telescope";

                case DriverTypes.Video:
                    return "Video";
            }
            throw new Exception("Unknown device type requested;");
        }

        /// <summary>
        /// Searches the ASCOM Registry for all drivers of a specified driver type
        /// </summary>
        /// <param name="DeviceType">The driver type to search for as a DriverType.</param>
        /// <returns>Returns a list of found ASCOM Devices, this includes ProgID and the friendly Name</returns>
        public static List<ASCOMRegistration> GetDrivers(DriverTypes DeviceType)
        {
            return GetDrivers(DriverString(DeviceType));
        }

        /// <summary>
        /// Searches the ASCOM Registry for all drivers of a specified driver type
        /// </summary>
        /// <param name="DeviceTypeName">The driver type to search for as a string.</param>
        /// <returns>Returns a list of found ASCOM Devices, this includes ProgID and the friendly Name</returns>
        public static List<ASCOMRegistration> GetDrivers(string DeviceTypeName)
        {
            List<ASCOMRegistration> Drivers = new List<ASCOMRegistration>();

            using (var localmachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                                            RegistryView.Registry32))
            {
                using (var ASCOMKeys = localmachine32.OpenSubKey($"SOFTWARE\\ASCOM\\{DeviceTypeName} Drivers", false))
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
    }
}