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
        ILogger TL = null;

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

        /// <summary>
        /// A string containing only the major and minor version of the driver.
        /// This must be in the form "n.n".
        /// Not to be confused with the InterfaceVersion property, which is the version of this specification supported by the driver (currently 2). 
        /// </summary>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
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

        /// <summary>
        /// The dome altitude (degrees, horizon zero and increasing positive to 90 zenith).
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Raises an error only if no altitude control. If actual dome altitude can not be read, then reports back the last slew position. 
        /// </remarks>
        public double Altitude => Device.Altitude;

        /// <summary>
        /// Indicates whether the dome is in the home position. Raises an error if not supported. 
        /// <para>
        /// This is normally used following a <see cref="FindHome" /> operation. The value is reset with any azimuth slew operation that moves the dome away from the home position.
        /// </para>
        /// <para>
        /// <see cref="AtHome" /> may also become true during normal slew operations, if the dome passes through the home position and the dome controller hardware is capable of detecting that; 
        /// or at the end of a slew operation if the dome comes to rest at the home position.
        /// </para>
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <para>The home position is normally defined by a hardware sensor positioned around the dome circumference and represents a fixed, known azimuth reference.</para>
        /// <para>For some devices, the home position may represent a small range of azimuth values, rather than a discrete value, since dome inertia, the resolution of the home position sensor and/or the azimuth encoder may be
        /// insufficient to return the exact same azimuth value on each occasion. Some dome controllers, on the other hand, will always force the azimuth reading to a fixed value whenever the home position sensor is active.
        /// Because of these potential differences in behaviour, applications should not rely on the reported azimuth position being identical each time <see cref="AtHome" /> is set <c>true</c>.</para>
        /// </remarks>
        /// [ASCOM-135] TPL - Updated documentation
        public bool AtHome => Device.AtHome;

        /// <summary>
        /// True if the dome is in the programmed park position.
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Set only following a <see cref="Park" /> operation and reset with any slew operation. Raises an error if not supported. 
        /// </remarks>
        public bool AtPark => Device.AtPark;

        /// <summary>
        /// The dome azimuth (degrees, North zero and increasing clockwise, i.e., 90 East, 180 South, 270 West)
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>Raises an error only if no azimuth control. If actual dome azimuth can not be read, then reports back last slew position</remarks>
        public double Azimuth => Device.Azimuth;

        /// <summary>
        /// True if driver can do a search for home position.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// </remarks>
        public bool CanFindHome => Device.CanFindHome;

        /// <summary>
        /// True if driver is capable of setting dome altitude.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// </remarks>
        public bool CanPark => Device.CanPark;

        /// <summary>
        /// True if driver is capable of setting dome altitude.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// </remarks>
        public bool CanSetAltitude => Device.CanSetAltitude;

        /// <summary>
        /// True if driver is capable of setting dome azimuth.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// </remarks>
        public bool CanSetAzimuth => Device.CanSetAzimuth;

        /// <summary>
        /// True if driver can set the dome park position.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// </remarks>
        public bool CanSetPark => Device.CanSetPark;

        /// <summary>
        /// True if driver is capable of automatically operating shutter.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// </remarks>
        public bool CanSetShutter => Device.CanSetShutter;

        /// <summary>
        /// True if the dome hardware supports slaving to a telescope.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// See the notes for the <see cref="Slaved" /> property.
        /// </remarks>
        public bool CanSlave => Device.CanSlave;

        /// <summary>
        /// True if driver is capable of synchronizing the dome azimuth position using the <see cref="SyncToAzimuth" /> method.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// </remarks>
        public bool CanSyncAzimuth => Device.CanSyncAzimuth;

        /// <summary>
        /// Status of the dome shutter or roll-off roof.
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Raises an error only if no shutter control. If actual shutter status can not be read, then reports back the last shutter state. 
        /// </remarks>
        public ShutterState ShutterStatus => (ShutterState)Device.ShutterStatus;

        /// <summary>
        /// True if the dome is slaved to the telescope in its hardware, else False.
        /// </summary>
        /// <exception cref="NotImplementedException">If Slaved can not be set.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red;margin-bottom:0"><b>Slaved Read must be implemented and must not throw a NotImplementedException. </b></p>
        /// <p style="color:red;margin-top:0"><b>Slaved Write can throw a NotImplementedException.</b></p>
        /// Set this property to True to enable dome-telescope hardware slaving, if supported (see <see cref="CanSlave" />). Raises an exception on any attempt to set 
        /// this property if hardware slaving is not supported). Always returns False if hardware slaving is not supported. 
        /// </remarks>
        public bool Slaved { get => Device.Slaved; set => Device.Slaved = value; }

        /// <summary>
        /// True if any part of the dome is currently moving, False if all dome components are steady.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red;margin-bottom:0"><b>Slewing must be implemented and must not throw a NotImplementedException. </b></p>
        /// Raises an error if <see cref="Slaved" /> is True, if not supported, if a communications failure occurs, or if the dome can not reach indicated azimuth. 
        /// </remarks>
        public bool Slewing => Device.Slewing;

        /// <summary>
        /// Immediately cancel current dome operation.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// Calling this method will immediately disable hardware slewing (<see cref="Slaved" /> will become False). Raises an error if a communications failure occurs, or if the command is known to have failed. 
        /// </remarks>
        public void AbortSlew()
        {
            Device.AbortSlew();
        }

        /// <summary>
        /// Close shutter or otherwise shield telescope from the sky.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public void CloseShutter()
        {
            Device.CloseShutter();
        }

        /// <summary>
        /// Start operation to search for the dome home position.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="SlavedException">Thrown if <see cref="Slaved"/> is <see langword="true"/>.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// After Home position is established initializes <see cref="Azimuth" /> to the default value and sets the <see cref="AtHome" /> flag. 
        /// Exception if not supported or communications failure. Raises an error if <see cref="Slaved" /> is True.
        /// </remarks>
        public void FindHome()
        {
            Device.FindHome();
        }

        /// <summary>
        /// Open shutter or otherwise expose telescope to the sky.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Raises an error if not supported or if a communications failure occurs. 
        /// </remarks>
        public void OpenShutter()
        {
            Device.OpenShutter();
        }

        /// <summary>
        /// Rotate dome in azimuth to park position.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// After assuming programmed park position, sets <see cref="AtPark" /> flag. Raises an error if <see cref="Slaved" /> is True, or if not supported, or if a communications failure has occurred. 
        /// </remarks>
        public void Park()
        {
            Device.Park();
        }

        /// <summary>
        /// Set the current azimuth, altitude position of dome to be the park position.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Raises an error if not supported or if a communications failure occurs. 
        /// </remarks>
        public void SetPark()
        {
            Device.SetPark();
        }

        /// <summary>
        /// Slew the dome to the given altitude position.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="InvalidValueException">If the supplied altitude is outside the range 0..90 degrees.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Raises an error if <see cref="Slaved" /> is True, if not supported, if a communications failure occurs, or if the dome can not reach indicated altitude. 
        /// </remarks>
        /// <param name="Altitude">Target dome altitude (degrees, horizon zero and increasing positive to 90 zenith)</param>
        public void SlewToAltitude(double Altitude)
        {
            Device.SlewToAltitude(Altitude);
        }

        /// <summary>
        /// Slew the dome to the given azimuth position.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="InvalidValueException">If the supplied azimuth is outside the range 0..360 degrees.</exception>
        /// <exception cref="SlavedException">Thrown if <see cref="Slaved"/> is <see langword="true"/>.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Raises an error if <see cref="Slaved" /> is True, if not supported, if a communications failure occurs, or if the dome can not reach indicated azimuth. 
        /// </remarks>
        /// <param name="Azimuth">Target azimuth (degrees, North zero and increasing clockwise. i.e., 90 East, 180 South, 270 West)</param>
        public void SlewToAzimuth(double Azimuth)
        {
            Device.SlewToAzimuth(Azimuth);
        }

        /// <summary>
        /// Synchronize the current position of the dome to the given azimuth.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="InvalidValueException">If the supplied azimuth is outside the range 0..360 degrees.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Raises an error if not supported or if a communications failure occurs. 
        /// </remarks>
        /// <param name="Azimuth">Target azimuth (degrees, North zero and increasing clockwise. i.e., 90 East, 180 South, 270 West)</param>
        public void SyncToAzimuth(double Azimuth)
        {
            Device.SyncToAzimuth(Azimuth);
        }

        #endregion

    }
}
