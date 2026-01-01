using ASCOM.Common.DeviceInterfaces;
using System.Collections.Generic;
using ASCOM.Common;
using ASCOM.Common.Interfaces;
using ASCOM.Common.DeviceStateClasses;

namespace ASCOM.Com.DriverAccess
{
    /// <summary>
    /// FilterWheel device class
    /// </summary>
#if NET8_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    public class FilterWheel : ASCOMDevice, IFilterWheelV3
    {

        #region Convenience members

        /// <summary>
        /// Return a list of all FilterWheels registered in the ASCOM Profile
        /// </summary>
        public static List<ASCOMRegistration> FilterWheels => Profile.GetDrivers(DeviceTypes.FilterWheel);

        /// <summary>
        /// FilterWheel device state
        /// </summary>
        public FilterWheelState FilterWheelState
        {
            get
            {
                // Create a state object to return.
                FilterWheelState filterWheelState = new FilterWheelState(DeviceState, TL);
                TL.LogMessage(LogLevel.Debug,nameof(FilterWheelState), $"Returning: '{filterWheelState.Position}' '{filterWheelState.TimeStamp}'");

                // Return the device specific state class
                return filterWheelState;
            }
        }

        #endregion

        #region Initialisers

        /// <summary>
        /// Initialise FilterWheel device
        /// </summary>
        /// <param name="ProgID">COM ProgID of the device.</param>
        public FilterWheel(string ProgID) : base(ProgID)
        {
            deviceType = DeviceTypes.FilterWheel; ;
        }

        /// <summary>
        /// Initialise FilterWheel device with a debug logger
        /// </summary>
        /// <param name="ProgID">ProgID of the driver</param>
        /// <param name="logger">Logger instance to receive debug information.</param>
        public FilterWheel(string ProgID, ILogger logger) : base(ProgID)
        {
            deviceType = DeviceTypes.FilterWheel;
            TL = logger;
        }

        #endregion

        #region IFilterWheelV2 and IFilterWheelV3

        /// <inheritdoc/>
        public new string Description
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return string.Empty;
                }
                return base.Description;
            }
        }

        /// <inheritdoc/>
        public new string DriverInfo
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return string.Empty;
                }
                return base.DriverInfo;
            }
        }

        /// <inheritdoc/>
        public new string DriverVersion
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return string.Empty;
                }
                return base.DriverVersion;
            }
        }

        /// <inheritdoc/>
        public new string Name
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return string.Empty;
                }
                return base.Name;
            }
        }

        /// <inheritdoc/>
        public int[] FocusOffsets => Device.FocusOffsets;

        /// <inheritdoc/>
        public string[] Names => Device.Names;

        /// <inheritdoc/>
        public short Position { get => Device.Position; set => Device.Position = value; }

        #endregion
    }
}
