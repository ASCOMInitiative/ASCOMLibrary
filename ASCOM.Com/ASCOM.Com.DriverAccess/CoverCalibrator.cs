using ASCOM.Common.DeviceInterfaces;
using System.Collections.Generic;
using ASCOM.Common;
using ASCOM.Common.Interfaces;
using ASCOM.Common.DeviceStateClasses;

namespace ASCOM.Com.DriverAccess
{
    /// <summary>
    /// CoverCalibrator device class
    /// </summary>
#if NET8_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    public class CoverCalibrator : ASCOMDevice, ICoverCalibratorV2
    {

        #region Convenience members

        /// <summary>
        /// Return a list of all CoverCalibrator devices registered in the ASCOM Profile
        /// </summary>
        public static List<ASCOMRegistration> CoverCalibrators => Profile.GetDrivers(DeviceTypes.CoverCalibrator);

        /// <summary>
        /// CoverCalibrator device state
        /// </summary>
        public CoverCalibratorState CoverCalibratorState
        {
            get
            {
                // Create a state object to return.
                CoverCalibratorState deviceState = new CoverCalibratorState(DeviceState, TL);
                TL.LogMessage(LogLevel.Debug,"CoverCalibratorState", $"Returning: '{deviceState.Brightness}' '{deviceState.CalibratorChanging}' '{deviceState.CalibratorState}' '{deviceState.CoverMoving}' '{deviceState.CoverState}' '{deviceState.TimeStamp}'");

                // Return the device specific state class
                return deviceState;
            }
        }

        #endregion

        #region Initialisers

        /// <summary>
        /// Initialise CoverClaibrator device
        /// </summary>
        /// <param name="ProgID">COM ProgID of the device.</param>
        public CoverCalibrator(string ProgID) : base(ProgID)
        {
            deviceType = DeviceTypes.CoverCalibrator;
        }

        /// <summary>
        /// Initialise CoverCalibrator device with a debug logger
        /// </summary>
        /// <param name="ProgID">ProgID of the driver</param>
        /// <param name="logger">Logger instance to receive debug information.</param>
        public CoverCalibrator(string ProgID, ILogger logger) : base(ProgID)
        {
            deviceType = DeviceTypes.CoverCalibrator;
            TL = logger;
        }

        #endregion

        #region ICoverCalibratorV1 members

        /// <inheritdoc/>
        public CoverStatus CoverState => (CoverStatus)Device.CoverState;

        /// <inheritdoc/>
        public CalibratorStatus CalibratorState => (CalibratorStatus)Device.CalibratorState;

        /// <inheritdoc/>
        public int Brightness => Device.Brightness;

        /// <inheritdoc/>
        public int MaxBrightness => Device.MaxBrightness;

        /// <inheritdoc/>
        public void OpenCover()
        {
            Device.OpenCover();
        }

        /// <inheritdoc/>
        public void CloseCover()
        {
            Device.CloseCover();
        }

        /// <inheritdoc/>
        public void HaltCover()
        {
            Device.HaltCover();
        }

        /// <inheritdoc/>
        public void CalibratorOn(int Brightness)
        {
            Device.CalibratorOn(Brightness);
        }

        /// <inheritdoc/>
        public void CalibratorOff()
        {
            Device.CalibratorOff();
        }

        #endregion

        #region ICoverCalibratorV2 members

        /// <inheritdoc/>
        public bool CalibratorChanging
        {
            get
            {
                // Check whether this device supports Connect / Disconnect
                if (DeviceCapabilities.HasConnectAndDeviceState(deviceType, InterfaceVersion))
                {
                    // Platform 7 or later device so return the device's CalibratorChanging property
                    return Device.CalibratorChanging;
                }

                // Platform 6 or earlier device so use CalibratorState to determine the movement state.
                return CalibratorState == CalibratorStatus.NotReady;
            }
        }

        /// <inheritdoc/>
        public bool CoverMoving
        {
            get
            {
                // Check whether this device supports Connect / Disconnect
                if (DeviceCapabilities.HasConnectAndDeviceState(deviceType, InterfaceVersion))
                {
                    // Platform 7 or later device so return the device's CoverMoving property
                    return Device.CoverMoving;
                }

                // Platform 6 or earlier device so use CoverState to determine the movement state.
                return CoverState == CoverStatus.Moving;
            }
        }

        #endregion
    }
}
