using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ASCOM.Common.DeviceInterfaces
{
    public static class DeviceCapabilities
    {
        /// <summary>
        /// Returns <see langword="true"/> if the device has a Platform 7 or later interface that supports asynchronous Switch methods
        /// </summary>
        /// <param name="driverInterfaceVersion">Interface version of this driver (Int16)</param>
        /// <returns></returns>
        /// <exception cref="InvalidValueException">The supplied interface version is 0 or less.</exception>
        public static bool HasAsyncSwitch(int driverInterfaceVersion)
        {
            // Validate parameter
            if (driverInterfaceVersion < 1)
                throw new InvalidValueException($"ASCOMLibrary.DeviceCapabilities.HasAsyncSwitch - Supplied interface version is 0 or negative: {driverInterfaceVersion}");

            return driverInterfaceVersion >= 3;
        }


        /// <summary>
        /// Returns <see langword="true"/> if the device has a Platform 7 or later interface that supports Connect / Disconnect and DeviceState
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="driverInterfaceVersion">Interface version of this driver (Int16)</param>
        public static bool HasConnectAndDeviceState(DeviceTypes? deviceType, short driverInterfaceVersion)
        {
            return HasConnectAndDeviceState(deviceType, Convert.ToInt32(driverInterfaceVersion));
        }

        /// <summary>
        /// Returns <see langword="true"/> if the device has a Platform 7 or later interface that supports Connect / Disconnect and DeviceState
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="driverInterfaceVersion">Interface version of this driver (Int32)</param>
        public static bool HasConnectAndDeviceState(DeviceTypes? deviceType, int driverInterfaceVersion)
        {
            if (!deviceType.HasValue)
            {
                throw new InvalidValueException("ASCOMLibrary.DeviceCapabilities.HasConnectAndDeviceState - Supplied device type is null.");
            }

            // Switch on the type of this DriverAccess object
            switch (deviceType)
            {
                // True if interface version is greater than 3
                case DeviceTypes.Camera:
                    if (driverInterfaceVersion > 3)
                        return true;
                    break;

                // True if interface version is greater than 1
                case DeviceTypes.CoverCalibrator:
                    if (driverInterfaceVersion > 1)
                        return true;
                    break;

                // True if interface version is greater than 2
                case DeviceTypes.Dome:
                    if (driverInterfaceVersion > 2)
                        return true;
                    break;

                // True if interface version is greater than 2
                case DeviceTypes.FilterWheel:
                    if (driverInterfaceVersion > 2)
                        return true;
                    break;

                // True if interface version is greater than 3
                case DeviceTypes.Focuser:
                    if (driverInterfaceVersion > 3)
                        return true;
                    break;

                // True if interface version is greater than 1
                case DeviceTypes.ObservingConditions:
                    if (driverInterfaceVersion > 1)
                        return true;
                    break;

                // True if interface version is greater than 3
                case DeviceTypes.Rotator:
                    if (driverInterfaceVersion > 3)
                        return true;
                    break;

                // True if interface version is greater than 1
                case DeviceTypes.SafetyMonitor:
                    if (driverInterfaceVersion > 2)
                        return true;
                    break;

                // True if interface version is greater than 2
                case DeviceTypes.Switch:
                    if (driverInterfaceVersion > 2)
                        return true;
                    break;

                // True if interface version is greater than 3
                case DeviceTypes.Telescope:
                    if (driverInterfaceVersion > 3)
                        return true;
                    break;

                // True if interface version is greater than 1
                case DeviceTypes.Video:
                    if (driverInterfaceVersion > 1)
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
