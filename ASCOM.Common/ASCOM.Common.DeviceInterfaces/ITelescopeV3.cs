using System;

namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Defines the ITelescope Interface
    /// </summary>
    public interface ITelescopeV3 : IAscomDevice
    {
        /// <summary>
        /// Stops a slew in progress.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.AbortSlew">Canonical Definition</see></remarks>
        void AbortSlew();

        /// <summary>
        /// The alignment mode of the mount (Alt/Az, Polar, German Polar).
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.AlignmentMode">Canonical Definition</see></remarks>
        AlignmentMode AlignmentMode { get; }

        /// <summary>
        /// The Altitude above the local horizon of the telescope's current position (degrees, positive up)
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.Altitude">Canonical Definition</see></remarks>
        double Altitude { get; }

        /// <summary>
        /// The area of the telescope's aperture, taking into account any obstructions (square meters)
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.ApertureArea">Canonical Definition</see></remarks>
        double ApertureArea { get; }

        /// <summary>
        /// The telescope's effective aperture diameter (meters)
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.ApertureDiameter">Canonical Definition</see></remarks>
        double ApertureDiameter { get; }

        /// <summary>
        /// True if the telescope is stopped in the Home position. Set only following a <see cref="FindHome"></see> operation,
        /// and reset with any slew operation. This property must be False if the telescope does not support homing. 
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.AtHome">Canonical Definition</see></remarks>
        bool AtHome { get; }

        /// <summary>
        /// True if the telescope has been put into the parked state by the see <see cref="Park" /> method. Set False by calling the Unpark() method.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.AtPark">Canonical Definition</see></remarks>
        bool AtPark { get; }

        /// <summary>
        /// Determine the rates at which the telescope may be moved about the specified axis by the <see cref="MoveAxis" /> method.
        /// </summary>
        /// <param name="Axis">The axis about which rate information is desired (TelescopeAxes value)</param>
        /// <returns>Collection of <see cref="IRate" /> rate objects</returns>
        /// <exception cref="InvalidValueException">If an invalid Axis is specified.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.AxisRates">Canonical Definition</see></remarks>
        IAxisRates AxisRates(TelescopeAxis Axis);

        /// <summary>
        /// The azimuth at the local horizon of the telescope's current position (degrees, North-referenced, positive East/clockwise).
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.Azimuth">Canonical Definition</see></remarks>
        double Azimuth { get; }

        /// <summary>
        /// True if this telescope is capable of programmed finding its home position (<see cref="FindHome" /> method).
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.CanFindHome">Canonical Definition</see></remarks>
        bool CanFindHome { get; }

        /// <summary>
        /// True if this telescope can move the requested axis
        /// </summary>
        /// <exception cref="InvalidValueException">If an invalid Axis is specified.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <param name="Axis">Primary, Secondary or Tertiary axis</param>
        /// <returns>Boolean indicating can or can not move the requested axis</returns>
        /// <exception cref="InvalidValueException">If an invalid Axis is specified.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.CanMoveAxis">Canonical Definition</see></remarks>
        bool CanMoveAxis(TelescopeAxis Axis);

        /// <summary>
        /// True if this telescope is capable of programmed parking (<see cref="Park" />method)
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.CanPark">Canonical Definition</see></remarks>
        bool CanPark { get; }

        /// <summary>
        /// True if this telescope is capable of software-pulsed guiding (via the <see cref="PulseGuide" /> method)
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.CanPulseGuide">Canonical Definition</see></remarks>
        bool CanPulseGuide { get; }

        /// <summary>
        /// True if the <see cref="DeclinationRate" /> property can be changed to provide offset tracking in the declination axis.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.CanSetDeclinationRate">Canonical Definition</see></remarks>
        bool CanSetDeclinationRate { get; }

        /// <summary>
        /// True if the guide rate properties used for <see cref="PulseGuide" /> can be adjusted.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.CanSetGuideRates">Canonical Definition</see></remarks>
        bool CanSetGuideRates { get; }

        /// <summary>
        /// True if this telescope is capable of programmed setting of its park position (<see cref="SetPark" /> method)
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.CanSetPark">Canonical Definition</see></remarks>
        bool CanSetPark { get; }

        /// <summary>
        /// True if the <see cref="SideOfPier" /> property can be set, meaning that the mount can be forced to flip.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.CanSetPierSide">Canonical Definition</see></remarks>
        bool CanSetPierSide { get; }

        /// <summary>
        /// True if the <see cref="RightAscensionRate" /> property can be changed to provide offset tracking in the right ascension axis.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.CanSetRightAscensionRate">Canonical Definition</see></remarks>
        bool CanSetRightAscensionRate { get; }

        /// <summary>
        /// True if the <see cref="Tracking" /> property can be changed, turning telescope sidereal tracking on and off.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.CanSetTracking">Canonical Definition</see></remarks>
        bool CanSetTracking { get; }

        /// <summary>
        /// True if this telescope is capable of programmed slewing (synchronous or asynchronous) to equatorial coordinates
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.CanSlew">Canonical Definition</see></remarks>
        bool CanSlew { get; }

        /// <summary>
        /// True if this telescope is capable of programmed slewing (synchronous or asynchronous) to local horizontal coordinates
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.CanSlewAltAz">Canonical Definition</see></remarks>
        bool CanSlewAltAz { get; }

        /// <summary>
        /// True if this telescope is capable of programmed asynchronous slewing to local horizontal coordinates
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.CanSlewAltAzAsync">Canonical Definition</see></remarks>
        bool CanSlewAltAzAsync { get; }

        /// <summary>
        /// True if this telescope is capable of programmed asynchronous slewing to equatorial coordinates.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.CanSlewAsync">Canonical Definition</see></remarks>
        bool CanSlewAsync { get; }

        /// <summary>
        /// True if this telescope is capable of programmed synching to equatorial coordinates.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.CanSync">Canonical Definition</see></remarks>
        bool CanSync { get; }

        /// <summary>
        /// True if this telescope is capable of programmed synching to local horizontal coordinates
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.CanSyncAltAz">Canonical Definition</see></remarks>
        bool CanSyncAltAz { get; }

        /// <summary>
        /// True if this telescope is capable of programmed unparking (<see cref="Unpark" /> method).
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.CanUnpark">Canonical Definition</see></remarks>
        bool CanUnpark { get; }

        /// <summary>
        /// The declination (degrees) of the telescope's current equatorial coordinates, in the coordinate system given by the <see cref="EquatorialSystem" /> property.
        /// Reading the property will raise an error if the value is unavailable. 
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.Declination">Canonical Definition</see></remarks>
        double Declination { get; }

        /// <summary>
        /// The declination tracking rate (arcseconds per SI second, default = 0.0)
        /// </summary>
        /// <exception cref="NotImplementedException">If DeclinationRate Write is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid DeclinationRate is specified</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.DeclinationRate">Canonical Definition</see></remarks>
        double DeclinationRate { get; set; }

        /// <summary>
        /// Predict side of pier for German equatorial mounts
        /// </summary>
        /// <param name="RightAscension">The destination right ascension (hours).</param>
        /// <param name="Declination">The destination declination (degrees, positive North).</param>
        /// <returns>The side of the pier on which the telescope would be on if a slew to the given equatorial coordinates is performed at the current instant of time.</returns>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="InvalidValueException">If an invalid RightAscension or Declination is specified.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.DestinationSideOfPier">Canonical Definition</see></remarks>
        PointingState DestinationSideOfPier(double RightAscension, double Declination);

        /// <summary>
        /// True if the telescope or driver applies atmospheric refraction to coordinates.
        /// </summary>
        /// <exception cref="NotImplementedException">Either read or write or both properties can throw NotImplementedException if not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.DoesRefraction">Canonical Definition</see></remarks>
        bool DoesRefraction { get; set; }

        /// <summary>
        /// Equatorial coordinate system used by this telescope (e.g. Topocentric or J2000).
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.EquatorialSystem">Canonical Definition</see></remarks>
        EquatorialCoordinateType EquatorialSystem { get; }

        /// <summary>
        /// Locates the telescope's "home" position (synchronous)
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanFindHome" /> is False</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.FindHome">Canonical Definition</see></remarks>
        void FindHome();

        /// <summary>
        /// The telescope's focal length, meters
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.FocalLength">Canonical Definition</see></remarks>
        double FocalLength { get; }

        /// <summary>
        /// The current Declination movement rate offset for telescope guiding (degrees/sec)
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="InvalidValueException">If an invalid guide rate is set.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.GuideRateDeclination">Canonical Definition</see></remarks>
        double GuideRateDeclination { get; set; }

        /// <summary>
        /// The current Right Ascension movement rate offset for telescope guiding (degrees/sec)
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="InvalidValueException">If an invalid guide rate is set.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.GuideRateRightAscension">Canonical Definition</see></remarks>
        double GuideRateRightAscension { get; set; }

        /// <summary>
        /// True if a <see cref="PulseGuide" /> command is in progress, False otherwise
        /// </summary>
        /// <exception cref="NotImplementedException">If <see cref="CanPulseGuide" /> is False</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.IsPulseGuiding">Canonical Definition</see></remarks>
        bool IsPulseGuiding { get; }

        /// <summary>
        /// Move the telescope in one axis at the given rate.
        /// </summary>
        /// <param name="Axis">The physical axis about which movement is desired</param>
        /// <param name="Rate">The rate of motion (deg/sec) about the specified axis</param>
        /// <exception cref="NotImplementedException">If the method is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid axis or rate is given.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.MoveAxis">Canonical Definition</see></remarks>
        void MoveAxis(TelescopeAxis Axis, double Rate);

        /// <summary>
        /// Move the telescope to its park position, stop all motion (or restrict to a small safe range), and set <see cref="AtPark" /> to True.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanPark" /> is False</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.Park">Canonical Definition</see></remarks>
        void Park();

        /// <summary>
        /// Moves the scope in the given direction for the given interval or time at 
        /// the rate given by the corresponding guide rate property 
        /// </summary>
        /// <param name="Direction">The direction in which the guide-rate motion is to be made</param>
        /// <param name="Duration">The duration of the guide-rate motion (milliseconds)</param>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanPulseGuide" /> is False</exception>
        /// <exception cref="InvalidValueException">If an invalid direction or duration is given.</exception>
        /// <exception cref="InvalidOperationException">If the pulse guide cannot be effected e.g. if the telescope is slewing or is not tracking.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.PulseGuide">Canonical Definition</see></remarks>
        void PulseGuide(GuideDirection Direction, int Duration);

        /// <summary>
        /// The right ascension (hours) of the telescope's current equatorial coordinates,
        /// in the coordinate system given by the EquatorialSystem property
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.RightAscension">Canonical Definition</see></remarks>
        double RightAscension { get; }

        /// <summary>
        /// The right ascension tracking rate offset from sidereal (seconds per sidereal second, default = 0.0)
        /// </summary>
        /// <exception cref="NotImplementedException">If RightAscensionRate Write is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid rate is set.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.RightAscensionRate">Canonical Definition</see></remarks>
        double RightAscensionRate { get; set; }

        /// <summary>
        /// Sets the telescope's park position to be its current position.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanPark" /> is False</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.SetPark">Canonical Definition</see></remarks>
        void SetPark();

        /// <summary>
        /// Indicates the pointing state of the mount.
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid side of pier is set.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.SideOfPier">Canonical Definition</see></remarks>
        PointingState SideOfPier { get; set; }

        /// <summary>
        /// The local apparent sidereal time from the telescope's internal clock (hours, sidereal)
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.SiderealTime">Canonical Definition</see></remarks>
        double SiderealTime { get; }

        /// <summary>
        /// The elevation above mean sea level (meters) of the site at which the telescope is located
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid elevation is set.</exception>
        /// <exception cref="InvalidOperationException">If the application must set the elevation before reading it, but has not.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.SiteElevation">Canonical Definition</see></remarks>
        double SiteElevation { get; set; }

        /// <summary>
        /// The geodetic(map) latitude (degrees, positive North, WGS84) of the site at which the telescope is located.
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid latitude is set.</exception>
        /// <exception cref="InvalidOperationException">If the application must set the latitude before reading it, but has not.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.SiteLatitude">Canonical Definition</see></remarks>
        double SiteLatitude { get; set; }

        /// <summary>
        /// The longitude (degrees, positive East, WGS84) of the site at which the telescope is located.
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid longitude is set.</exception>
        /// <exception cref="InvalidOperationException">If the application must set the longitude before reading it, but has not.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.SiteLongitude">Canonical Definition</see></remarks>
        double SiteLongitude { get; set; }

        /// <summary>
        /// True if telescope is currently moving in response to one of the
        /// Slew methods or the <see cref="MoveAxis" /> method, False at all other times.
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.Slewing">Canonical Definition</see></remarks>
        bool Slewing { get; }

        /// <summary>
        /// Specifies a post-slew settling time (sec.).
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid settle time is set.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.SlewSettleTime">Canonical Definition</see></remarks>
        short SlewSettleTime { get; set; }

        /// <summary>
        /// Move the telescope to the given local horizontal coordinates, return when slew is complete
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanSlewAltAz" /> is False</exception>
        /// <exception cref="InvalidValueException">If an invalid azimuth or elevation is given.</exception>
		/// <exception cref="ParkedException">If the telescope is parked</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.SlewToAltAz">Canonical Definition</see></remarks>
        /// <param name="Azimuth">Target azimuth (degrees, North-referenced, positive East/clockwise).</param>
        /// <param name="Altitude">Target altitude (degrees, positive up)</param>
        void SlewToAltAz(double Azimuth, double Altitude);

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
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.SlewToAltAzAsync">Canonical Definition</see></remarks>
        void SlewToAltAzAsync(double Azimuth, double Altitude);

        /// <summary>
        /// Move the telescope to the given equatorial coordinates, return when slew is complete
        /// </summary>
        /// <exception cref="InvalidValueException">If an invalid right ascension or declination is given.</exception>
        /// <param name="RightAscension">The destination right ascension (hours). Copied to <see cref="TargetRightAscension" />.</param>
        /// <param name="Declination">The destination declination (degrees, positive North). Copied to <see cref="TargetDeclination" />.</param>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanSlew" /> is False</exception>
		/// <exception cref="ParkedException">If the telescope is parked</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.SlewToCoordinates">Canonical Definition</see></remarks>
        void SlewToCoordinates(double RightAscension, double Declination);

        /// <summary>
        /// Move the telescope to the given equatorial coordinates, return immediately after starting the slew.
        /// </summary>
        /// <param name="RightAscension">The destination right ascension (hours). Copied to <see cref="TargetRightAscension" />.</param>
        /// <param name="Declination">The destination declination (degrees, positive North). Copied to <see cref="TargetDeclination" />.</param>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanSlewAsync" /> is False</exception>
        /// <exception cref="ParkedException">If the telescope is parked</exception>
        /// <exception cref="InvalidValueException">If an invalid right ascension or declination is given.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.SlewToCoordinatesAsync">Canonical Definition</see></remarks>
        void SlewToCoordinatesAsync(double RightAscension, double Declination);

        /// <summary>
        /// Move the telescope to the <see cref="TargetRightAscension" /> and <see cref="TargetDeclination" /> coordinates, return when slew complete.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanSlew" /> is False</exception>
        /// <exception cref="ParkedException">If the telescope is parked</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.SlewToTarget">Canonical Definition</see></remarks>
        void SlewToTarget();

        /// <summary>
        /// Move the telescope to the <see cref="TargetRightAscension" /> and <see cref="TargetDeclination" />  coordinates,
        /// returns immediately after starting the slew.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanSlewAsync" /> is False</exception>
        /// <exception cref="ParkedException">If the telescope is parked</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.SlewToTargetAsync">Canonical Definition</see></remarks>
        void SlewToTargetAsync();

        /// <summary>
        /// Matches the scope's local horizontal coordinates to the given local horizontal coordinates.
        /// </summary>
        /// <param name="Azimuth">Target azimuth (degrees, North-referenced, positive East/clockwise)</param>
        /// <param name="Altitude">Target altitude (degrees, positive up)</param>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanSyncAltAz" /> is False</exception>
        /// <exception cref="InvalidValueException">If an invalid azimuth or altitude is given.</exception>
        /// <exception cref="ParkedException">If the telescope is parked</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.SyncToAltAz">Canonical Definition</see></remarks>
        void SyncToAltAz(double Azimuth, double Altitude);

        /// <summary>
        /// Matches the scope's equatorial coordinates to the given equatorial coordinates.
        /// </summary>
        /// <param name="RightAscension">The corrected right ascension (hours). Copied to the <see cref="TargetRightAscension" /> property.</param>
        /// <param name="Declination">The corrected declination (degrees, positive North). Copied to the <see cref="TargetDeclination" /> property.</param>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanSync" /> is False</exception>
        /// <exception cref="InvalidValueException">If an invalid right ascension or declination is given.</exception>
        /// <exception cref="ParkedException">If the telescope is parked</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.SyncToCoordinates">Canonical Definition</see></remarks>
        void SyncToCoordinates(double RightAscension, double Declination);

        /// <summary>
        /// Matches the scope's equatorial coordinates to the given equatorial coordinates.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanSync" /> is False</exception>
        /// <exception cref="ParkedException">If the telescope is parked</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.SyncToTarget">Canonical Definition</see></remarks>
        void SyncToTarget();

        /// <summary>
        /// The declination (degrees, positive North) for the target of an equatorial slew or sync operation
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid declination is set.</exception>
        /// <exception cref="InvalidOperationException">If the property is read before being set for the first time.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.TargetDeclination">Canonical Definition</see></remarks>
        double TargetDeclination { get; set; }

        /// <summary>
        /// The right ascension (hours) for the target of an equatorial slew or sync operation
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid right ascension is set.</exception>
        /// <exception cref="InvalidOperationException">If the property is read before being set for the first time.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.TargetRightAscension">Canonical Definition</see></remarks>
        double TargetRightAscension { get; set; }

        /// <summary>
        /// The state of the telescope's sidereal tracking drive.
        /// </summary>
        /// <exception cref="NotImplementedException">If Tracking Write is not implemented.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.Tracking">Canonical Definition</see></remarks>
        bool Tracking { get; set; }

        /// <summary>
        /// The current tracking rate of the telescope's sidereal drive
        /// </summary>
        /// <exception cref="NotImplementedException">If TrackingRate Write is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid drive rate is set.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.TrackingRate">Canonical Definition</see></remarks>
        DriveRate TrackingRate { get; set; }

        /// <summary>
        /// Returns a collection of supported <see cref="DriveRate" /> values that describe the permissible
        /// values of the <see cref="TrackingRate" /> property for this telescope type.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.TrackingRates">Canonical Definition</see></remarks>
        ITrackingRates TrackingRates { get; }

        /// <summary>
        /// Takes telescope out of the Parked state.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanUnpark" /> is False</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.Unpark">Canonical Definition</see></remarks>
        void Unpark();

        /// <summary>
        /// The UTC date/time of the telescope's internal clock
        /// </summary>
        /// <exception cref="NotImplementedException">If UTCDate Write is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid <see cref="DateTime" /> is set.</exception>
        /// <exception cref="InvalidOperationException">When UTCDate is read and the mount cannot provide this property itself and a value has not yet be established by writing to the property.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/telescope.html#Telescope.UTCDate">Canonical Definition</see></remarks>
        DateTime UTCDate { get; set; }
    }
}