using System;
using System.Drawing;
using System.Net.NetworkInformation;

namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Defines the ITelescopeV4 Interface
    /// </summary>
    public interface ITelescopeV4 : IAscomDeviceV2, ITelescopeV3
    {
        /// <summary>
        /// True if the telescope is stopped in the Home position. Set only following a <see cref="FindHome"></see> operation,
        /// and reset with any slew operation. This property must be False if the telescope does not support homing. 
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// <para>Further explanation is available in this link: <a href="https://ascom-standards.org/newdocs/telescope.html#telescope-athome" target="_blank">Master Interface Document</a>.</para>
        /// <pata>This is only available for telescope Interface Versions 2 and later.</pata>
        /// </remarks>
        new bool AtHome { get; }

        /// <summary>
        /// True if this telescope is capable of programmed slewing (synchronous or asynchronous) to equatorial coordinates
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// May raise an error if the telescope is not connected. 
        /// Synchronous methods are deprecated in this version (V4) of ITelescope and Clients should not use them. ASCOM COM Driver authors however must implement synchronous methods,
        /// if the mount can slew, to ensure backward compatibility. <a href="https://ascom-standards.org/newdocs/scope-slew-faq.html#scope-slew-faq" target="_blank">Synchronous Slewing in the Telescope Interface</a> See .
        /// </remarks>
        new bool CanSlew { get; }

        /// <summary>
        /// True if this telescope is capable of programmed slewing (synchronous or asynchronous) to local horizontal coordinates
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// May raise an error if the telescope is not connected. 
        /// Synchronous methods are deprecated in this version (V4) of ITelescope and Clients should not use them. ASCOM COM Driver authors however must implement synchronous methods,
        /// if the mount can slew, to ensure backward compatibility. <a href="https://ascom-standards.org/newdocs/scope-slew-faq.html#scope-slew-faq" target="_blank">Synchronous Slewing in the Telescope Interface</a> See .
        /// </remarks>
        new bool CanSlewAltAz { get; }

        /// <summary>
        /// True if this telescope is capable of programmed asynchronous slewing to local horizontal coordinates
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// May raise an error if the telescope is not connected. 
        /// <para>Further explanation is available in this link: <a href="https://ascom-standards.org/newdocs/telescope.html#telescope-canslewaltazasync" target="_blank">Master Interface Document</a>.</para>
        /// <para>If the mount can slew, driver authors must implement asynchronous slewing.</para>
        /// </remarks>
        new bool CanSlewAltAzAsync { get; }

        /// <summary>
        /// True if this telescope is capable of programmed asynchronous slewing to equatorial coordinates.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be 
        /// accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// May raise an error if the telescope is not connected.
        /// <para>Further explanation is available in this link: <a href="https://ascom-standards.org/newdocs/telescope.html#telescope-canslewasync" target="_blank">Master Interface Document</a>.</para>
        /// and will not be implemented at all by Alpaca devices.
        /// If the mount can slew, driver authors must implement asynchronous slewing.
        /// </remarks>
        new bool CanSlewAsync { get; }

        /// <summary>
        /// Locates the telescope's "home" position
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="ITelescopeV3.CanFindHome" /> is False</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <para>This is an asynchronous method: Use the <see cref="ITelescopeV3.Slewing"/> property to monitor the operation's progress. </para>
        /// <para>Further explanation is available in this link: <a href="https://ascom-standards.org/newdocs/telescope.html#telescope-findhome" target="_blank">Master Interface Document</a>.</para>
        /// <para>This is only available for telescope Interface Versions 2 and later.</para>
        /// </remarks>
        new void FindHome();

        /// <summary>
        /// Move the telescope in one axis at the given rate.
        /// </summary>
        /// <param name="Axis">The physical axis about which movement is desired</param>
        /// <param name="Rate">The rate of motion (deg/sec) about the specified axis</param>
        /// <exception cref="MethodNotImplementedException">If the method is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid axis or rate is given.</exception>
        /// <exception cref="NotConnectedException">If the device is not connected</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception> 
        /// <remarks>
        /// This method supports control of the mount about its mechanical axes.
        /// The telescope will start moving at the specified rate about the specified axis and continue indefinitely.
        /// This method can be called for each axis separately, and have them all operate concurrently at separate rates of motion.
        /// Set the rate for an axis to zero to restore the motion about that axis to the rate set by the <see cref="ITelescopeV3.Tracking"/> property.
        /// Tracking motion (if enabled, see note below) is suspended during this mode of operation.
        /// <para>
        /// Raises an error if <see cref="ITelescopeV3.AtPark" /> is true.
        /// This must be implemented for the if the <see cref="ITelescopeV3.CanMoveAxis" /> property returns True for the given axis.</para>
        /// <para>This is only available for telescope Interface version 2 and later.</para>
        /// <para>
        /// MoveAxis() is best seen as an override to however the mount is configured for Tracking, including its enabled/disabled state and any 
        /// current RightAscensionRate and DeclinationRate offsets. 
        /// </para>
        /// <para>
        /// While <see cref="MoveAxis" /> is in effect, TrackingRate, RightAscensionRate and DeclinationRate should retain their current values and will become 
        /// effective again when MoveAxis is set to zero for the relevant axis.
        /// </para>
        /// <para>
        /// When <see cref="MoveAxis" /> is reset to zero for an axis, its previous state must be restored as shown below:
        /// </para>
        /// <list type="bullet">
        /// <item><description><legacyBold>RA Axis with <see cref="ITelescopeV3.Tracking"/> is Enabled</legacyBold>: The current <see cref="ITelescopeV3.TrackingRate"/>, plus any <see cref="ITelescopeV3.RightAscensionRate" /> 
        /// ( the latter is valid only if <see cref="ITelescopeV3.TrackingRate"/> is <see cref="DriveRate.Sidereal"/> )</description></item>
        /// <item><description><legacyBold>RA Axis with <see cref="ITelescopeV3.Tracking"/> is Disabled</legacyBold>: 0</description></item>
        /// <item><description><legacyBold>Dec Axis with <see cref="ITelescopeV3.Tracking"/> is Enabled</legacyBold>: The <see cref="ITelescopeV3.DeclinationRate" /> if non-zero or 0</description></item>
        /// <item><description><legacyBold>Dec Axis with <see cref="ITelescopeV3.Tracking"/> is Disabled</legacyBold>: 0</description></item>
        /// </list>
        /// <b>NOTES:</b>
        /// <list type="bullet">
        /// <item><description>The Slewing property must remain <see langword="true"/> whenever <legacyBold>any</legacyBold> axis has a non-zero MoveAxis rate. E.g. Suppose
        /// MoveAxis is used to make both the RA and declination axes move at valid axis rates. If the declination axis rate is then set to zero, Slewing must remain
        /// <see langword="true"/> because the RA axis is still moving at a non-zero axis rate.</description></item>
        /// <item><description>The movement rate must be within the value(s) obtained from a <see cref="IRate" /> object in the
        /// the <see cref="ITelescopeV3.AxisRates" /> collection. This is a signed value with negative rates moving in the opposite direction to positive rates.</description></item>
        /// <item><description>The values specified in <see cref="ITelescopeV3.AxisRates" /> are absolute, unsigned values and apply to both directions, determined by the sign used in this command.</description></item>
        /// <item><description>MoveAxis can be used to simulate a hand-box by initiating motion with the MouseDown event and stopping the motion with the MouseUp event.</description></item>
        /// <item><description>It may be possible to implement satellite tracking by using the <see cref="MoveAxis" /> method to move the scope in the required manner to track a satellite.</description></item>
        /// </list>
        /// </remarks>
        new void MoveAxis(TelescopeAxis Axis, double Rate);

        /// <summary>
        /// Move the telescope to its park position, stop all motion (or restrict to a small safe range), and set <see cref="ITelescopeV3.AtPark" /> to True.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="ITelescopeV3.CanPark" /> is False</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <para>
        /// This is an asynchronous method: Use the <see cref="ITelescopeV3.Slewing"/> property to monitor the operation's progress. 
        /// <para>Further explanation is available in this link: <a href="https://ascom-standards.org/newdocs/telescope.html#telescope-park" target="_blank">Master Interface Document</a>.</para>
        /// </para>
        /// </remarks>
        new void Park();

        /// <summary>
        /// Moves the scope in the given direction for the given interval or time at 
        /// the rate given by the corresponding guide rate property 
        /// </summary>
        /// <param name="Direction">The direction in which the guide-rate motion is to be made</param>
        /// <param name="Duration">The duration of the guide-rate motion (milliseconds)</param>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="ITelescopeV3.CanPulseGuide" /> is False</exception>
        /// <exception cref="InvalidValueException">If an invalid direction or duration is given.</exception>
        /// <exception cref="InvalidOperationException">If the pulse guide cannot be effected e.g. if the telescope is ITelescopeV3.Slewing or is not ITelescopeV3.Tracking or a pulse guide is already in progress and a second cannot be started asynchronously.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <para>
        /// This method is asynchronous and should return quickly using IsPulseGuiding as the completion property. 
        /// </para>
        /// <para>
        /// If the device cannot have simultaneous PulseGuide operations in both RightAscension and Declination, it must throw InvalidOperationException when the overlapping operation is attempted.
        /// </para>
        /// <para>
        /// <para>Further explanation is available in this link: <a href="https://ascom-standards.org/newdocs/telescope.html#telescope-pulseguide" target="_blank">Master Interface Document</a>.</para>
        /// </para>
        /// </remarks>
        new void PulseGuide(GuideDirection Direction, int Duration);

        /// <summary>
        /// Indicates the pointing state of the mount.
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid side of pier is set.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <para>SideOfPier SET is an asynchronous method and <see cref="ITelescopeV3.Slewing"/> should be set True while the operation is in progress.</para>
        /// <para>Please note that "SideofPier" is a misnomer and that this method actually refers to the mount's pointing state. For German Equatorial mounts there is a complex
        /// relationship between pointing state and the physical side of the pier on which the mount resides.</para>
        /// <para>
        /// For example, suppose the mount is tracking on the east side of the pier, counterweights down, 
        /// observing a target on the celestial equator at hour angle +3.0.Now suppose that the observer
        /// wishes to observe a new target at hour angle -9.0. All the mount needs to do is to rotate the declination axis, 
        /// through the celestial pole where the hour angle will change from +3.0 to -9.0, and keep going until it gets
        /// to the required declination at hour angle -9.0. Other than tracking, the right ascension  axis has not moved.
        /// </para>
        /// <para>
        /// In this example the mount is still physically on the east side of the pier but the pointing state
        /// will have changed when the declination axis moved through the celestial pole.
        /// </para>
        /// </remarks>
        new PointingState SideOfPier { get; set; }

        /// <summary>
        /// Move the telescope to the given local horizontal coordinates, return when slew is complete
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanSlewAltAz" /> is False</exception>
        /// <exception cref="InvalidValueException">If an invalid azimuth or elevation is given.</exception>
		/// <exception cref="ParkedException">If the telescope is parked</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <param name="Azimuth">Target azimuth (degrees, North-referenced, positive East/clockwise).</param>
        /// <param name="Altitude">Target altitude (degrees, positive up)</param>
        /// <remarks>
        /// <p style="color:red"><b>Deprecated for client applications.</b></p>
        /// <para>This method must not be used by applications, use the asynchronous <see cref="SlewToAltAzAsync(double, double)"/> method instead.</para>
        /// <para>Further explanation is available in this link: <a href="https://ascom-standards.org/newdocs/telescope.html#telescope-slewtoaltaz" target="_blank">Master Interface Document</a>.</para>
        /// </remarks>
        new void SlewToAltAz(double Azimuth, double Altitude);

        /// <summary>
        /// This Method must be implemented if <see cref="CanSlewAltAzAsync" /> returns True.
        /// </summary>
        /// <param name="Azimuth">Azimuth to which to move</param>
        /// <param name="Altitude">Altitude to which to move to</param>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanSlewAltAzAsync" /> is False</exception>
        /// <exception cref="InvalidValueException">If an invalid azimuth or elevation is given.</exception>
        /// <exception cref="ParkedException">If the telescope is parked</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <para>Further explanation is available in this link: <a href="https://ascom-standards.org/newdocs/telescope.html#telescope-slewtoaltazasync" target="_blank">Master Interface Document</a>.</para>
        /// </remarks>
        new void SlewToAltAzAsync(double Azimuth, double Altitude);

        /// <summary>
        /// Move the telescope to the given equatorial coordinates, return when slew is complete
        /// </summary>
        /// <exception cref="InvalidValueException">If an invalid right ascension or declination is given.</exception>
        /// <param name="RightAscension">The destination right ascension (hours). Copied to <see cref="ITelescopeV3.TargetRightAscension" />.</param>
        /// <param name="Declination">The destination declination (degrees, positive North). Copied to <see cref="ITelescopeV3.TargetDeclination" />.</param>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanSlew" /> is False</exception>
        /// <exception cref="ParkedException">If the telescope is parked</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Deprecated for client applications.</b></p>
        /// <para>This method must not be used by applications, use the asynchronous <see cref="SlewToCoordinates(double, double)"/> method instead.</para>
        /// <para>Further explanation is available in this link: <a href="https://ascom-standards.org/newdocs/telescope.html#telescope-slewtocoordinates" target="_blank">Master Interface Document</a>.</para>
        /// </remarks>
        new void SlewToCoordinates(double RightAscension, double Declination);

        /// <summary>
        /// Move the telescope to the given equatorial coordinates, return immediately after starting the slew.
        /// </summary>
        /// <param name="RightAscension">The destination right ascension (hours). Copied to <see cref="ITelescopeV3.TargetRightAscension" />.</param>
        /// <param name="Declination">The destination declination (degrees, positive North). Copied to <see cref="ITelescopeV3.TargetDeclination" />.</param>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanSlewAsync" /> is False</exception>
        /// <exception cref="ParkedException">If the telescope is parked</exception>
        /// <exception cref="InvalidValueException">If an invalid right ascension or declination is given.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <para>Further explanation is available in this link: <a href="https://ascom-standards.org/newdocs/telescope.html#telescope-slewtocoordinatesasync" target="_blank">Master Interface Document</a>.</para>
        /// </remarks>
        new void SlewToCoordinatesAsync(double RightAscension, double Declination);

        /// <summary>
        /// Move the telescope to the <see cref="ITelescopeV3.TargetRightAscension" /> and <see cref="ITelescopeV3.TargetDeclination" /> coordinates, return when slew complete.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanSlew" /> is False</exception>
        /// <exception cref="ParkedException">If the telescope is parked</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Deprecated for client applications.</b></p>
        /// <para>This method must not be used by applications, use the asynchronous <see cref="SlewToTargetAsync"/> method instead.</para>
        /// <para>Further explanation is available in this link: <a href="https://ascom-standards.org/newdocs/telescope.html#telescope-slewtotarget" target="_blank">Master Interface Document</a>.</para>
        /// </remarks>
        new void SlewToTarget();

        /// <summary>
        /// Move the telescope to the <see cref="ITelescopeV3.TargetRightAscension" /> and <see cref="ITelescopeV3.TargetDeclination" />  coordinates,
        /// returns immediately after starting the slew.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanSlewAsync" /> is False</exception>
        /// <exception cref="ParkedException">If the telescope is parked</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <para>Further explanation is available in this link: <a href="https://ascom-standards.org/newdocs/telescope.html#telescope-slewtotargetasync" target="_blank">Master Interface Document</a>.</para>
        /// </remarks>
        new void SlewToTargetAsync();

        /// <summary>
        /// Takes telescope out of the Parked state.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="ITelescopeV3.CanUnpark" /> is False</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <para>
        /// This is an asynchronous method and <see cref="ITelescopeV3.Slewing"/> must be set True while the mount is unparking and False when the operation is complete. 
        /// <see cref="ITelescopeV3.AtPark"/> and <see cref="ITelescopeV3.Slewing"/> will be set False when the mount has unparked successfully.
        /// </para>
        /// <para>Further explanation is available in this link: <a href="https://ascom-standards.org/newdocs/telescope.html#telescope-unpark" target="_blank">Master Interface Document</a>.</para>
        /// </remarks>
        new void Unpark();
    }
}