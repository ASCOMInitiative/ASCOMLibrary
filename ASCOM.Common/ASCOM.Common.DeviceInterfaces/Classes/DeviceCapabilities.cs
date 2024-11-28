using System;
using System.Collections.Generic;

namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Methods that report whether a capability is present in a given device and interface version
    /// </summary>
    public static class DeviceCapabilities
    {
        /// <summary>
        /// Dictionary of the latest interface versions supported by Platform 6
        /// </summary>
        public static Dictionary<DeviceTypes, short> LatestPlatform6Interface = new Dictionary<DeviceTypes, short>()
        {
            { DeviceTypes.Camera, 3 },
            { DeviceTypes.CoverCalibrator, 1 },
            { DeviceTypes.Dome, 2 },
            { DeviceTypes.FilterWheel, 2 },
            { DeviceTypes.Focuser, 3 },
            { DeviceTypes.ObservingConditions, 1 },
            { DeviceTypes.Rotator, 3 },
            { DeviceTypes.SafetyMonitor, 1 },
            { DeviceTypes.Switch, 2 },
            { DeviceTypes.Telescope, 3 },
            { DeviceTypes.Video, 1 }
        };

        /// <summary>
        /// Dictionary of the interface versions at launch of Platform 7
        /// </summary>
        /// <remarks>
        /// These values must not change when new interfaces are added, update the LatestInterface dictionary instead
        /// </remarks>
        public static Dictionary<DeviceTypes, short> InitialPlatform7Interface = new Dictionary<DeviceTypes, short>()
        {
            { DeviceTypes.Camera, 4 },
            { DeviceTypes.CoverCalibrator, 2 },
            { DeviceTypes.Dome, 3 },
            { DeviceTypes.FilterWheel, 3 },
            { DeviceTypes.Focuser, 4 },
            { DeviceTypes.ObservingConditions, 2 },
            { DeviceTypes.Rotator, 4 },
            { DeviceTypes.SafetyMonitor, 3 },
            { DeviceTypes.Switch, 3 },
            { DeviceTypes.Telescope, 4 },
            { DeviceTypes.Video, 2 }
        };

        /// <summary>
        /// Dictionary of the latest interface versions supported by Platform 7
        /// </summary>
        /// <remarks>
        /// Update these values as new interface versions are included in future Platforms
        /// </remarks>
        public static Dictionary<DeviceTypes, short> LatestInterface = new Dictionary<DeviceTypes, short>()
        {
            { DeviceTypes.Camera, 4 },
            { DeviceTypes.CoverCalibrator, 2 },
            { DeviceTypes.Dome, 3 },
            { DeviceTypes.FilterWheel, 3 },
            { DeviceTypes.Focuser, 4 },
            { DeviceTypes.ObservingConditions, 2 },
            { DeviceTypes.Rotator, 4 },
            { DeviceTypes.SafetyMonitor, 3 },
            { DeviceTypes.Switch, 3 },
            { DeviceTypes.Telescope, 4 },
            { DeviceTypes.Video, 2 }
        };

        /// <summary>
        /// Returns <see langword="true"/> for all devices except IFocuserV1 devices that do not have the Connected property
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="interfaceVersion">Interface version of this device (Int16)</param>
        /// <returns><see langword="true"/> for all device interfaces except IFocuserV1</returns>
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
        /// <returns><see langword="true"/> for all device interfaces except IFocuserV1</returns>
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
                    if (interfaceVersion == 1) // IFocuserV1 so return false
                        return false;
                    else // IFocuserV2 or later so return true
                        return true;

                // All other device types and interface versions have Connected so return true.
                default: // All other device types
                    return true;
            }
        }

        /// <summary>
        /// Indicates whether this Switch interface version supports asynchronous Switch methods
        /// </summary>
        /// <param name="interfaceVersion">Interface version of this device (Int16)</param>
        /// <returns><see langword="true"/> when the interface version supports AsyncSwitch methods.</returns>
        /// <exception cref="InvalidValueException">The supplied interface version is 0 or less.</exception>
        public static bool HasAsyncSwitch(int interfaceVersion)
        {
            // Validate parameter
            if (interfaceVersion < 1)
                throw new InvalidValueException($"ASCOMLibrary.DeviceCapabilities.HasAsyncSwitch - Supplied interface version is 0 or negative: {interfaceVersion}");

            return interfaceVersion >= InitialPlatform7Interface[DeviceTypes.Switch];
        }

        /// <summary>
        /// Indicates whether this CoverCalibrator interface version supports the CoverCalibrator.CalibratorChanging property
        /// </summary>
        /// <param name="interfaceVersion">Interface version of this device</param>
        /// <returns><see langword="true"/> when the interface version supports CoverCalibrator.CalibratorChanging.</returns>
        /// <exception cref="InvalidValueException">The supplied interface version is 0 or less.</exception>
        public static bool HasCalibratorChanging(int interfaceVersion)
        {
            // Validate parameter
            if (interfaceVersion < 1)
                throw new InvalidValueException($"ASCOMLibrary.DeviceCapabilities.HasCalibratorChanging - Supplied interface version is 0 or negative: {interfaceVersion}");

            return interfaceVersion >= InitialPlatform7Interface[DeviceTypes.CoverCalibrator];
        }

        /// <summary>
        /// Indicates whether this CoverCalibrator interface version supports the CoverCalibrator.CoverMoving property
        /// Returns <see langword="true"/> if the device has a Platform 7 or later interface that supports the CoverCalibrator.CoverMoving property
        /// </summary>
        /// <param name="interfaceVersion">Interface version of this device</param>
        /// <returns><see langword="true"/> when the interface version supports CoverCalibrator.CoverMoving.</returns>
        /// <exception cref="InvalidValueException">The supplied interface version is 0 or less.</exception>
        public static bool HasCoverMoving(int interfaceVersion)
        {
            // Validate parameter
            if (interfaceVersion < 1)
                throw new InvalidValueException($"ASCOMLibrary.DeviceCapabilities.HasCoverMoving - Supplied interface version is 0 or negative: {interfaceVersion}");

            return interfaceVersion >= InitialPlatform7Interface[DeviceTypes.CoverCalibrator];
        }

        /// <summary>
        /// Returns <see langword="true"/> if the interface version of the specified device type supports Connect / Disconnect and DeviceState
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="interfaceVersion">Interface version of this device (Int16)</param>
        /// <returns><see langword="true"/> when the interface version supports Connect / Disconnect</returns>
        public static bool HasConnectAndDeviceState(DeviceTypes? deviceType, short interfaceVersion)
        {
            return HasConnectAndDeviceState(deviceType, Convert.ToInt32(interfaceVersion));
        }

        /// <summary>
        /// Returns <see langword="true"/> if the interface version of the specified device type supports Connect / Disconnect and DeviceState
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="interfaceVersion">Interface version of this device (Int32)</param>
        /// <exception cref="InvalidValueException">When deviceType is null.</exception>
        /// <exception cref="InvalidValueException">When interfaceVersion is 0 or negative.</exception>
        /// <returns><see langword="true"/> when the interface version supports Connect / Disconnect</returns>
        public static bool HasConnectAndDeviceState(DeviceTypes? deviceType, int interfaceVersion)
        {
            // Validate inputs
            if (!deviceType.HasValue) // The device type is a null value
            {
                throw new InvalidValueException("ASCOMLibrary.DeviceCapabilities.HasConnectAndDeviceState - The device type parameter is null.");
            }

            if (interfaceVersion < 1) // The interface version is 0 or negative
            {
                throw new InvalidValueException($"ASCOMLibrary.DeviceCapabilities.HasConnectAndDeviceState - The Interface version parameter is 0 or negative: {interfaceVersion}.");
            }

            return interfaceVersion >= InitialPlatform7Interface[deviceType.Value];
        }

        /// <summary>
        /// Returns <see langword="true"/> if the interface version of the specified device type is a Platform 6 interface version
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="interfaceVersion">Interface version of this device (Int16, short)</param>
        /// <exception cref="InvalidValueException">When deviceType is null.</exception>
        /// <exception cref="InvalidValueException">When interfaceVersion is 0 or negative.</exception>
        /// <returns><see langword="true"/> when the interface version is a Platform 6 interface version.</returns>
        public static bool IsPlatform6Interface(DeviceTypes? deviceType, short interfaceVersion)
        {
            return IsPlatform6Interface(deviceType, (int)interfaceVersion);
        }

        /// <summary>
        /// Returns <see langword="true"/> if the interface version of the specified device type is a Platform 6 interface version
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="interfaceVersion">Interface version of this device (Int32, int)</param>
        /// <exception cref="InvalidValueException">When deviceType is null.</exception>
        /// <exception cref="InvalidValueException">When interfaceVersion is 0 or negative.</exception>
        /// <returns><see langword="true"/> when the interface version is a Platform 6 interface version.</returns>
        public static bool IsPlatform6Interface(DeviceTypes? deviceType, int interfaceVersion)
        {
            // Validate inputs
            if (!deviceType.HasValue) // The device type is a null value
            {
                throw new InvalidValueException("ASCOMLibrary.DeviceCapabilities.IsPlatform6Interface - The device type parameter is null.");
            }

            if (interfaceVersion < 1) // The interface version is 0 or negative
            {
                throw new InvalidValueException($"ASCOMLibrary.DeviceCapabilities.IsPlatform6Interface - The Interface version parameter is 0 or negative: {interfaceVersion}.");
            }

            // Compensate for safety monitor possibly being interface version 1 or 2 by forcing the value to 2 for this test
            if (deviceType == DeviceTypes.SafetyMonitor)
                interfaceVersion = 2;

            // Compare the supplied interface version with the reference list
            return interfaceVersion == LatestPlatform6Interface[deviceType.Value];
        }

        /// <summary>
        /// Indicates whether the interface version of the specified device type is Platform 7 or later
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="interfaceVersion">Interface version of this device (Int32)</param>
        /// <returns><see langword="true"/> when the device implements a Platform 7 or later interface</returns>
        public static bool IsPlatform7OrLater(DeviceTypes? deviceType, int interfaceVersion)
        {
            return HasConnectAndDeviceState(deviceType, interfaceVersion);
        }

        /// <summary>
        /// Indicates whether the interface version of the specified device type is Platform 7 or later
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="interfaceVersion">Interface version of this device (Int16)</param>
        /// <returns><see langword="true"/> when the device implements a Platform 7 or later interface</returns>
        public static bool IsPlatform7OrLater(DeviceTypes? deviceType, short interfaceVersion)
        {
            return HasConnectAndDeviceState(deviceType, Convert.ToInt32(interfaceVersion));
        }

        /// <summary>
        /// Returns <see langword="true"/> if the interface version of the specified device type is a Platform 6 interface version
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="interfaceVersion">Interface version of this device (Int16, short)</param>
        /// <exception cref="InvalidValueException">When deviceType is null.</exception>
        /// <exception cref="InvalidValueException">When interfaceVersion is 0 or negative.</exception>
        /// <returns><see langword="true"/> when the interface version is a Platform 6 interface version.</returns>
        public static bool IsSupportedInterface(DeviceTypes? deviceType, short interfaceVersion)
        {
            return IsValidAscomInterface(deviceType, (int)interfaceVersion);
        }

        /// <summary>
        /// Returns <see langword="true"/> if the interface version of the specified device type is a valid ASCOM interface version on any Platform
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="interfaceVersion">Interface version of this device (Int32, int).</param>
        /// <exception cref="InvalidValueException">When deviceType is null.</exception>
        /// <exception cref="InvalidValueException">When interfaceVersion is 0 or negative.</exception>
        /// <returns><see langword="true"/> when the interface version is a Platform 6 interface version.</returns>
        /// <remarks>
        /// Supply an interface version of 1 (valid) rather than zero (invalid) for very early drivers that do not have an InterfaceVersion property.
        /// </remarks>
        public static bool IsValidAscomInterface(DeviceTypes? deviceType, int interfaceVersion)
        {
            // Validate inputs
            if (!deviceType.HasValue) // The device type is a null value
            {
                throw new InvalidValueException("ASCOMLibrary.DeviceCapabilities.IsSupportedInterface - The device type parameter is null.");
            }

            // Check for invalid low interface version numbers
            if (interfaceVersion < 1) // The interface version is 0 or negative
                return false; // Not supported

            // Check whether the interface version is equal to or lower than the interface version in the latest Platform release
            if (interfaceVersion <= LatestInterface[deviceType.Value]) // The interface version is supported
                return true; // Supported

            // All other interface versions i.e. those above the interface version in the latest Platform release
            return false; // Not supported
        }
    }
}
