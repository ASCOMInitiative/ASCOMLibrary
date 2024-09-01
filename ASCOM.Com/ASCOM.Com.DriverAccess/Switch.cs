using ASCOM.Common.DeviceInterfaces;
using System.Collections.Generic;
using ASCOM.Common;
using ASCOM.Common.Interfaces;

namespace ASCOM.Com.DriverAccess
{
    /// <summary>
    /// Switch device class
    /// </summary>
    public class Switch : ASCOMDevice, ISwitchV3
    {

        #region Convenience members

        /// <summary>
        /// Return a list of all Switches registered in the ASCOM Profile
        /// </summary>
        public static List<ASCOMRegistration> Switches => Profile.GetDrivers(DeviceTypes.Switch);

        // There is no SwitchState member because the number of switches is a dynamic, user configured value as is switch naming, which makes it impossible to model in a class with fixed properties.

        #endregion

        #region Initialisers
        /// <summary>
        /// Initialise Switch device
        /// </summary>
        /// <param name="ProgID">COM ProgID of the device.</param>
        public Switch(string ProgID) : base(ProgID)
        {
            deviceType = DeviceTypes.Switch;
        }

        /// <summary>
        /// Initialise Switch device with a debug logger
        /// </summary>
        /// <param name="ProgID">ProgID of the driver</param>
        /// <param name="logger">Logger instance to receive debug information.</param>
        public Switch(string ProgID, ILogger logger) : base(ProgID)
        {
            deviceType = DeviceTypes.Switch;
            TL = logger;
        }

        #endregion

        #region ISwitchV2

        /// <inheritdoc/>
        public short MaxSwitch => Device.MaxSwitch;

        /// <inheritdoc/>
        public string GetSwitchName(short id)
        {
            return Device.GetSwitchName(id);
        }

        /// <inheritdoc/>
        public void SetSwitchName(short id, string name)
        {
            Device.SetSwitchName(id, name);
        }

        /// <inheritdoc/>
        public string GetSwitchDescription(short id)
        {
            return Device.GetSwitchDescription(id);
        }

        /// <inheritdoc/>
        public bool CanWrite(short id)
        {
            return Device.CanWrite(id);
        }

        /// <inheritdoc/>
        public bool GetSwitch(short id)
        {
            return Device.GetSwitch(id);
        }

        /// <inheritdoc/>
        public void SetSwitch(short id, bool state)
        {
            Device.SetSwitch(id, state);
        }

        /// <inheritdoc/>
        public double MaxSwitchValue(short id)
        {
            try
            {
                return Device.MaxSwitchValue(id);
            }
            catch (ASCOM.NotImplementedException)
            {
                return 1.0;
            }
        }

        /// <inheritdoc/>
        public double MinSwitchValue(short id)
        {
            try
            {
                return Device.MinSwitchValue(id);
            }
            catch (ASCOM.NotImplementedException)
            {
                return 0.0;
            }
        }

        /// <inheritdoc/>
        public double SwitchStep(short id)
        {
            try
            {
                return Device.SwitchStep(id);
            }
            catch (ASCOM.NotImplementedException)
            {
                return 1.0;
            }
        }

        /// <inheritdoc/>
        public double GetSwitchValue(short id)
        {
            try
            {
                return Device.GetSwitchValue(id);
            }
            catch (ASCOM.NotImplementedException)
            {
                return this.GetSwitch(id) ? 1.0 : 0.0;
            }
        }

        /// <inheritdoc/>
        public void SetSwitchValue(short id, double value)
        {
            try
            {
                Device.SetSwitchValue(id, value);
            }
            catch (ASCOM.NotImplementedException)
            {
                bool bv = value >= 0.5;
                this.SetSwitch(id, bv);
            }
        }

        #endregion

        #region ISwitchV3

        /// <inheritdoc />
        public void SetAsync(short id, bool state)
        {
            // Check whether this device supports asynchronous methods
            if (DeviceCapabilities.HasConnectAndDeviceState(deviceType, InterfaceVersion))
            {
                // Platform 7 or later device so use the device's method
                Device.SetAsync(id, state);
                return;
            }

            // Platform 6 or earlier device
            throw new NotImplementedException($"DriverAccess.Switch - SetAsync is not supported by this device because it exposes interface ISwitchV{InterfaceVersion}.");
        }

        /// <inheritdoc />
        public void SetAsyncValue(short id, double value)
        {
            // Check whether this device supports asynchronous methods
            if (DeviceCapabilities.HasConnectAndDeviceState(deviceType, InterfaceVersion))
            {
                // Platform 7 or later device so use the device's method
                Device.SetAsyncValue(id, value);
                return;
            }

            // Platform 6 or earlier device
            throw new NotImplementedException($"DriverAccess.Switch - SetAsyncValue is not supported by this device because it exposes interface ISwitchV{InterfaceVersion}.");
        }

        /// <inheritdoc />
        public bool CanAsync(short id)
        {
            // Check whether this device supports asynchronous methods
            if (DeviceCapabilities.HasConnectAndDeviceState(deviceType, InterfaceVersion))
            {
                // Platform 7 or later device so use the device's method
                return Device.CanAsync(id);
            }

            // Platform 6 or earlier device - async is not supported so return false to show no async support.
            return false;
        }

        /// <inheritdoc />
        public bool StateChangeComplete(short id)
        {
            // Check whether this device supports asynchronous methods
            if (DeviceCapabilities.HasConnectAndDeviceState(deviceType, InterfaceVersion))
            {
                // Platform 7 or later device so use the device's method
                return Device.StateChangeComplete(id);
            }

            // Platform 6 or earlier device
            throw new NotImplementedException($"DriverAccess.Switch - StateChangeComplete is not supported by this device because it exposes interface ISwitchV{InterfaceVersion}.");
        }

        /// <inheritdoc />
        public void CancelAsync(short id)
        {
            // Check whether this device supports asynchronous methods
            if (DeviceCapabilities.HasConnectAndDeviceState(deviceType, InterfaceVersion))
            {
                // Platform 7 or later device so use the device's method
                Device.CancelAsync(id);
                return;
            }

            // Platform 6 or earlier device
            throw new NotImplementedException($"DriverAccess.Switch - CancelAsync is not supported by this device because it exposes interface ISwitchV{InterfaceVersion}.");
        }

        #endregion

    }
}