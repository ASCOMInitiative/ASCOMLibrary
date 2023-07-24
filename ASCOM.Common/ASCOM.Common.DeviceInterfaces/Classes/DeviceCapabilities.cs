using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ASCOM.Common.DeviceInterfaces
{
    public static class DeviceCapabilities
    {
        /// <summary>
        /// Returns <see langword="true"/> if the device has a Platform 7 or later interface that supports Connect / Disconnect and DeviceState
        /// </summary>
        public static bool HasConnectAndDeviceState(DeviceTypes deviceType, short DriverInterfaceVersion)
        {
            // Switch on the type of this DriverAccess object
            switch (deviceType)
            {
                // True if interface version is greater than 3
                case DeviceTypes.Camera :
                    if (DriverInterfaceVersion > 3)
                        return true;
                    break;

                // True if interface version is greater than 1
                case DeviceTypes.CoverCalibrator:
                    if (DriverInterfaceVersion > 1)
                        return true;
                    break;

                // True if interface version is greater than 2
                case DeviceTypes.Dome:
                    if (DriverInterfaceVersion > 2)
                        return true;
                    break;

                // True if interface version is greater than 2
                case DeviceTypes.FilterWheel:
                    if (DriverInterfaceVersion > 2)
                        return true;
                    break;

                // True if interface version is greater than 3
                case DeviceTypes.Focuser:
                    if (DriverInterfaceVersion > 3)
                        return true;
                    break;

                // True if interface version is greater than 1
                case DeviceTypes.ObservingConditions:
                    if (DriverInterfaceVersion > 1)
                        return true;
                    break;

                // True if interface version is greater than 3
                case DeviceTypes.Rotator:
                    if (DriverInterfaceVersion > 3)
                        return true;
                    break;

                // True if interface version is greater than 1
                case DeviceTypes.SafetyMonitor:
                    if (DriverInterfaceVersion > 2)
                        return true;
                    break;

                // True if interface version is greater than 2
                case DeviceTypes.Switch:
                    if (DriverInterfaceVersion > 2)
                        return true;
                    break;

                // True if interface version is greater than 3
                case DeviceTypes.Telescope:
                    if (DriverInterfaceVersion > 3)
                        return true;
                    break;

                // True if interface version is greater than 1
                case DeviceTypes.Video:
                    if (DriverInterfaceVersion > 1)
                        return true;
                    break;

                default:
                    throw new InvalidValueException($"DeviceCapabillities.HasConnectAndDeviceState - Unsupported device type: {deviceType}. Please update the Library code to add support.");
            }

            // Device has a Platform 6 or earlier interface
            return false;
        }


    }
}
