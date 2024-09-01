using ASCOM.Common.DeviceInterfaces;
using System.Collections.Generic;
using ASCOM.Common;
using ASCOM.Common.Interfaces;
using ASCOM.Common.DeviceStateClasses;

namespace ASCOM.Com.DriverAccess
{
    /// <summary>
    /// Dome device class
    /// </summary>
    public class Dome : ASCOMDevice, IDomeV3
    {

        #region Convenience members

        /// <summary>
        /// Return a list of all Dome devices registered in the ASCOM Profile
        /// </summary>
        public static List<ASCOMRegistration> Domes => Profile.GetDrivers(DeviceTypes.Dome);

        /// <summary>
        /// Dome device state
        /// </summary>
        public DomeState DomeState
        {
            get
            {
                // Create a state object to return.
                DomeState domeState = new DomeState(DeviceState, TL);
                TL.LogMessage(LogLevel.Debug, nameof(DomeState), $"Returning: '{domeState.Altitude}' '{domeState.AtHome}' '{domeState.AtPark}' '{domeState.Azimuth}' '{domeState.ShutterStatus}' '{domeState.Slewing}' '{domeState.TimeStamp}'");

                // Return the device specific state class
                return domeState;
            }
        }

        #endregion

        #region Initialisers

        /// <summary>
        /// Initialise Dome device
        /// </summary>
        /// <param name="ProgID">COM ProgID of the device.</param>
        public Dome(string ProgID) : base(ProgID)
        {
            deviceType = DeviceTypes.Dome;
        }

        /// <summary>
        /// Initialise Dome device with a debug logger
        /// </summary>
        /// <param name="ProgID">ProgID of the driver</param>
        /// <param name="logger">Logger instance to receive debug information.</param>
        public Dome(string ProgID, ILogger logger) : base(ProgID)
        {
            deviceType = DeviceTypes.Dome;
            TL = logger;
        }

        #endregion

        #region IDomeV2 and IDomeV3

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
        public double Altitude => Device.Altitude;

        /// <inheritdoc/>
        public bool AtHome => Device.AtHome;

        /// <inheritdoc/>
        public bool AtPark => Device.AtPark;

        /// <inheritdoc/>
        public double Azimuth => Device.Azimuth;

        /// <inheritdoc/>
        public bool CanFindHome => Device.CanFindHome;

        /// <inheritdoc/>
        public bool CanPark => Device.CanPark;

        /// <inheritdoc/>
        public bool CanSetAltitude => Device.CanSetAltitude;

        /// <inheritdoc/>
        public bool CanSetAzimuth => Device.CanSetAzimuth;

        /// <inheritdoc/>
        public bool CanSetPark => Device.CanSetPark;

        /// <inheritdoc/>
        public bool CanSetShutter => Device.CanSetShutter;

        /// <inheritdoc/>
        public bool CanSlave => Device.CanSlave;

        /// <inheritdoc/>
        public bool CanSyncAzimuth => Device.CanSyncAzimuth;

        /// <inheritdoc/>
        public ShutterState ShutterStatus => (ShutterState)Device.ShutterStatus;

        /// <inheritdoc/>
        public bool Slaved { get => Device.Slaved; set => Device.Slaved = value; }

        /// <inheritdoc/>
        public bool Slewing => Device.Slewing;

        /// <inheritdoc/>
        public void AbortSlew()
        {
            Device.AbortSlew();
        }

        /// <inheritdoc/>
        public void CloseShutter()
        {
            Device.CloseShutter();
        }

        /// <inheritdoc/>
        public void FindHome()
        {
            Device.FindHome();
        }

        /// <inheritdoc/>
        public void OpenShutter()
        {
            Device.OpenShutter();
        }

        /// <inheritdoc/>
        public void Park()
        {
            Device.Park();
        }

        /// <inheritdoc/>
        public void SetPark()
        {
            Device.SetPark();
        }

        /// <inheritdoc/>
        public void SlewToAltitude(double Altitude)
        {
            Device.SlewToAltitude(Altitude);
        }

        /// <inheritdoc/>
        public void SlewToAzimuth(double Azimuth)
        {
            Device.SlewToAzimuth(Azimuth);
        }

        /// <inheritdoc/>
        public void SyncToAzimuth(double Azimuth)
        {
            Device.SyncToAzimuth(Azimuth);
        }

        #endregion

    }
}
