using System;

namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Methods that report whether a capability is present in a given device and interface version
    /// </summary>
    public static class DeviceCapabilities
    {
        /// <summary>
        /// Returns <see langword="true"/> for all devices except IFocuserV1 devices that do not have the Connected property
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="interfaceVersion">Interface version of this device (Int16)</param>
        /// <returns>True for all device interfaces except IFocuserV1</returns>
        /// <exception cref="InvalidValueException"></exception>
        public static bool HasConnected(DeviceTypes? deviceType, short interfaceVersion)
        {
            return HasConnected(deviceType, Convert.ToInt32(interfaceVersion));
        }

        /// <summary>
        /// Returns <see langword="true"/> for all devices except IFocuserV1 devices that do not have the Connected property
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="interfaceVersion">Interface version of this device (Int32)</param>
        /// <returns>True for all device interfaces except IFocuserV1</returns>
        /// <exception cref="InvalidValueException"></exception>
        public static bool HasConnected(DeviceTypes? deviceType, int interfaceVersion)
        {
            // Throw an exception if no device type is supplied
            if (!deviceType.HasValue)
                throw new InvalidValueException("ASCOMLibrary.DeviceCapabilities.HasConnected - Supplied device type is null.");

            // Switch on the device type 
            switch (deviceType)
            {
                // Focuser only has Connected in IFocuserV2 and later
                case DeviceTypes.Focuser: // Focuser device
                    if (interfaceVersion ==1) // IFocuserV1 so return false
                        return false;
                    else // IFocuserV2 or later so return true
                        return true;

                // All other device types and interface versions have Connected so return true.
                default: // All other device types
                    return true;
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> if the device has a Platform 7 or later interface that supports asynchronous Switch methods
        /// </summary>
        /// <param name="interfaceVersion">Interface version of this device (Int16)</param>
        /// <returns>True when the interface version supports AsyncSwitch methods.</returns>
        /// <exception cref="InvalidValueException">The supplied interface version is 0 or less.</exception>
        public static bool HasAsyncSwitch(int interfaceVersion)
        {
            // Validate parameter
            if (interfaceVersion < 1)
                throw new InvalidValueException($"ASCOMLibrary.DeviceCapabilities.HasAsyncSwitch - Supplied interface version is 0 or negative: {interfaceVersion}");

            return interfaceVersion >= 3;
        }

        /// <summary>
        /// Returns <see langword="true"/> if the device has a Platform 7 or later interface that supports the CoverCalibrator.CalibratorChanging property
        /// </summary>
        /// <param name="interfaceVersion">Interface version of this device</param>
        /// <returns>True when the interface version supports CoverCalibrator.CalibratorChanging.</returns>
        /// <exception cref="InvalidValueException">The supplied interface version is 0 or less.</exception>
        public static bool HasCalibratorChanging(int interfaceVersion)
        {
            // Validate parameter
            if (interfaceVersion < 1)
                throw new InvalidValueException($"ASCOMLibrary.DeviceCapabilities.HasCalibratorChanging - Supplied interface version is 0 or negative: {interfaceVersion}");

            return interfaceVersion >= 2;
        }

        /// <summary>
        /// Returns <see langword="true"/> if the device has a Platform 7 or later interface that supports the CoverCalibrator.HasCoverMoving property
        /// </summary>
        /// <param name="interfaceVersion">Interface version of this device</param>
        /// <returns>True when the interface version supports CoverCalibrator.HasCoverMoving.</returns>
        /// <exception cref="InvalidValueException">The supplied interface version is 0 or less.</exception>
        public static bool HasCoverMoving(int interfaceVersion)
        {
            // Validate parameter
            if (interfaceVersion < 1)
                throw new InvalidValueException($"ASCOMLibrary.DeviceCapabilities.HasCoverMoving - Supplied interface version is 0 or negative: {interfaceVersion}");

            return interfaceVersion >= 2;
        }

        /// <summary>
        /// Returns <see langword="true"/> if the device has a Platform 7 or later interface that supports Connect / Disconnect and DeviceState
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="interfaceVersion">Interface version of this device (Int16)</param>
        /// <returns>True when the interface version supports Connect / Disconnect</returns>
        public static bool HasConnectAndDeviceState(DeviceTypes? deviceType, short interfaceVersion)
        {
            return HasConnectAndDeviceState(deviceType, Convert.ToInt32(interfaceVersion));
        }

        /// <summary>
        /// Returns <see langword="true"/> if the device has a Platform 7 or later interface that supports Connect / Disconnect and DeviceState
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="interfaceVersion">Interface version of this device (Int32)</param>
        /// <returns>True when the interface version supports Connect / Disconnect</returns>
        public static bool HasConnectAndDeviceState(DeviceTypes? deviceType, int interfaceVersion)
        {
            if (!deviceType.HasValue)
            {
                throw new InvalidValueException("ASCOMLibrary.DeviceCapabilities.HasConnectAndDeviceState - Supplied device type is null.");
            }

            // Switch on the type of this device
            switch (deviceType)
            {
                // True if interface version is greater than 3
                case DeviceTypes.Camera:
                    if (interfaceVersion > 3)
                        return true;
                    break;

                // True if interface version is greater than 1
                case DeviceTypes.CoverCalibrator:
                    if (interfaceVersion > 1)
                        return true;
                    break;

                // True if interface version is greater than 2
                case DeviceTypes.Dome:
                    if (interfaceVersion > 2)
                        return true;
                    break;

                // True if interface version is greater than 2
                case DeviceTypes.FilterWheel:
                    if (interfaceVersion > 2)
                        return true;
                    break;

                // True if interface version is greater than 3
                case DeviceTypes.Focuser:
                    if (interfaceVersion > 3)
                        return true;
                    break;

                // True if interface version is greater than 1
                case DeviceTypes.ObservingConditions:
                    if (interfaceVersion > 1)
                        return true;
                    break;

                // True if interface version is greater than 3
                case DeviceTypes.Rotator:
                    if (interfaceVersion > 3)
                        return true;
                    break;

                // True if interface version is greater than 1
                case DeviceTypes.SafetyMonitor:
                    if (interfaceVersion > 2)
                        return true;
                    break;

                // True if interface version is greater than 2
                case DeviceTypes.Switch:
                    if (interfaceVersion > 2)
                        return true;
                    break;

                // True if interface version is greater than 3
                case DeviceTypes.Telescope:
                    if (interfaceVersion > 3)
                        return true;
                    break;

                // True if interface version is greater than 1
                case DeviceTypes.Video:
                    if (interfaceVersion > 1)
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
