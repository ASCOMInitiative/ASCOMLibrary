using ASCOM.Common.DeviceInterfaces;
using System.Collections.Generic;
using ASCOM.Common;
using ASCOM.Common.Interfaces;
using ASCOM.Common.DeviceStateClasses;

namespace ASCOM.Com.DriverAccess
{
    /// <summary>
    /// SafetyMonitor device class
    /// </summary>
    public class SafetyMonitor : ASCOMDevice, ISafetyMonitorV3
    {

        #region Convenience members

        /// <summary>
        /// Return a list of all SafetyMonitors registered in the ASCOM Profile
        /// </summary>
        public static List<ASCOMRegistration> SafetyMonitors => Profile.GetDrivers(DeviceTypes.SafetyMonitor);

        /// <summary>
        /// SafetyMonitor device state
        /// </summary>
        public SafetyMonitorState SafetyMonitorState
        {
            get
            {
                // Create a state object to return.
                SafetyMonitorState safetyMonitorState = new SafetyMonitorState(DeviceState, TL);
                TL.LogMessage(LogLevel.Debug,nameof(SafetyMonitorState), $"Returning: " +
                    $"Cloud cover: '{safetyMonitorState.IsSafe}', " +
                    $"Time stamp: '{safetyMonitorState.TimeStamp}'");

                // Return the device specific state class
                return safetyMonitorState;
            }
        }

        #endregion

        #region Initialisers

        /// <summary>
        /// Initialise SafetyMonitor device
        /// </summary>
        /// <param name="ProgID">COM ProgID of the device.</param>
        public SafetyMonitor(string ProgID) : base(ProgID)
        {
            deviceType = DeviceTypes.SafetyMonitor;
        }

        /// <summary>
        /// Initialise SafetyMonitor device with a debug logger
        /// </summary>
        /// <param name="ProgID">ProgID of the driver</param>
        /// <param name="logger">Logger instance to receive debug information.</param>
        public SafetyMonitor(string ProgID, ILogger logger) : base(ProgID)
        {
            deviceType = DeviceTypes.SafetyMonitor;
            TL = logger;
        }

        #endregion

        #region ISafetyMonitor V1, ISafetyMonitorV2 and ISafetyMonitorV3

        /// <inheritdoc/>
        public bool IsSafe => Device.IsSafe;

        #endregion

    }
}
