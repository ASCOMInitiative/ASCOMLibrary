using ASCOM.Common.DeviceInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using ASCOM.Common;
using System.Linq;

namespace ASCOM.Com.DriverAccess
{
    /// <summary>
    /// Telescope device class
    /// </summary>
    public class Telescope : ASCOMDevice, ITelescopeV4
    {
        Operation currentOperation = Operation.None; // Current operation name

        /// <summary>
        /// Return a list of all Telescopes registered in the ASCOM Profile
        /// </summary>
        public static List<ASCOMRegistration> Telescopes => Profile.GetDrivers(DeviceTypes.Telescope);

        /// <summary>
        /// Initialise Telescope device
        /// </summary>
        /// <param name="ProgID">COM ProgID of the device.</param>
        public Telescope(string ProgID) : base(ProgID)
        {
            deviceType = DeviceTypes.Telescope;
        }

        #region ITelescopeV3

        /// <summary>
        /// Returns the list of action names supported by this driver.
        /// </summary>
        /// <value>An ArrayList of strings (SafeArray collection) containing the names of supported actions.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Must be implemented</b></p> This method must return an empty IList object if no actions are supported. Please do not throw a <see cref="NotImplementedException" />.
        /// <para>This is an aid to client authors and testers who would otherwise have to repeatedly poll the driver to determine its capabilities. 
        /// Returned action names may be in mixed case to enhance presentation but  will be recognised case insensitively in 
        /// the <see cref="Action">Action</see> method.</para>
        /// <para>An array list collection has been selected as the vehicle for  action names in order to make it easier for clients to
        /// determine whether a particular action is supported. This is easily done through the Contains method. Since the
        /// collection is also enumerable it is easy to use constructs such as For Each ... to operate on members without having to be concerned 
        /// about how many members are in the collection. </para>
        /// <para>Collections have been used in the Telescope specification for a number of years and are known to be compatible with COM. Within .NET
        /// the ArrayList is the correct implementation to use as the .NET Generic methods are not compatible with COM.</para>
        /// </remarks>
        public new IList<string> SupportedActions
        {
            get
            {
                if (InterfaceVersion < 3)
                {
                    return new List<string>();
                }
                return base.SupportedActions;
            }
        }

        /// <summary>
        /// The alignment mode of the mount (Alt/Az, Polar, German Polar).
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// This is only available for telescope Interface Versions 2 and later.
        /// </remarks>
        public AlignmentMode AlignmentMode
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.NotImplementedException("AlignmentMode is only supported by Interface Versions 2 and above.");
                }
                return (AlignmentMode)Device.AlignmentMode;
            }
        }

        /// <summary>
        /// The Altitude above the local horizon of the telescope's current position (degrees, positive up)
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public double Altitude => Device.Altitude;

        /// <summary>
        /// The area of the telescope's aperture, taking into account any obstructions (square meters)
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// This is only available for telescope Interface Versions 2 and later.
        /// </remarks>
        public double ApertureArea
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.NotImplementedException("ApertureArea is only supported by Interface Versions 2 and above.");
                }
                return Device.ApertureArea;
            }
        }

        /// <summary>
        /// The telescope's effective aperture diameter (meters)
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// This is only available for telescope Interface Versions 2 and later.
        /// </remarks>
        public double ApertureDiameter
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.NotImplementedException("ApertureDiameter is only supported by Interface Versions 2 and above.");
                }
                return Device.ApertureDiameter;
            }
        }

        /// <summary>
        /// True if the telescope is stopped in the Home position. Set only following a <see cref="FindHome"></see> operation,
        ///  and reset with any slew operation. This property must be False if the telescope does not support homing. 
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// This is only available for telescope Interface Versions 2 and later.
        /// </remarks>
        public bool AtHome => Device.AtHome;

        /// <summary>
        /// True if the telescope has been put into the parked state by the see <see cref="Park" /> method. Set False by calling the Unpark() method.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// <para>AtPark is True when the telescope is in the parked state. This is achieved by calling the <see cref="Park" /> method. When AtPark is true, 
        /// the telescope movement is stopped (or restricted to a small safe range of movement) and all calls that would cause telescope 
        /// movement (e.g. slewing, changing Tracking state) must not do so, and must raise an error.</para>
        /// <para>The telescope is taken out of parked state by calling the <see cref="Unpark" /> method. If the telescope cannot be parked, 
        /// then AtPark must always return False.</para>
        /// <para>This is only available for telescope Interface Versions 2 and later.</para>
        /// </remarks>
        public bool AtPark
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return false;
                }
                return Device.AtPark;
            }
        }

        /// <summary>
        /// The azimuth at the local horizon of the telescope's current position (degrees, North-referenced, positive East/clockwise).
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public double Azimuth => Device.Azimuth;

        /// <summary>
        /// True if this telescope is capable of programmed finding its home position (<see cref="FindHome" /> method).
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// May raise an error if the telescope is not connected. 
        /// <para>This is only available for telescope Interface Versions 2 and later.</para>
        /// </remarks>
        public bool CanFindHome
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return false;
                }
                return Device.CanFindHome;
            }
        }

        /// <summary>
        /// True if this telescope is capable of programmed parking (<see cref="Park" />method)
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// May raise an error if the telescope is not connected. 
        /// <para>This is only available for telescope Interface Versions 2 and later.</para>
        /// </remarks>
        public bool CanPark
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return false;
                }
                return Device.CanPark;
            }
        }

        /// <summary>
        /// True if this telescope is capable of software-pulsed guiding (via the <see cref="PulseGuide" /> method)
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// May raise an error if the telescope is not connected. 
        /// </remarks>
        public bool CanPulseGuide => Device.CanPulseGuide;

        /// <summary>
        /// True if the <see cref="DeclinationRate" /> property can be changed to provide offset tracking in the declination axis.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// May raise an error if the telescope is not connected. 
        /// </remarks>
        public bool CanSetDeclinationRate => Device.CanSetDeclinationRate;

        /// <summary>
        /// True if the guide rate properties used for <see cref="PulseGuide" /> can be adjusted.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// May raise an error if the telescope is not connected. 
        /// <para>This is only available for telescope Interface Versions 2 and later.</para>
        /// </remarks>
        public bool CanSetGuideRates
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return false;
                }
                return Device.CanSetGuideRates;
            }
        }

        /// <summary>
        /// True if this telescope is capable of programmed setting of its park position (<see cref="SetPark" /> method)
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// May raise an error if the telescope is not connected. 
        /// <para>This is only available for telescope Interface Versions 2 and later.</para>
        /// </remarks>
        public bool CanSetPark
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return false;
                }
                return Device.CanSetPark;
            }
        }

        /// <summary>
        /// True if the <see cref="SideOfPier" /> property can be set, meaning that the mount can be forced to flip.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// This will always return False for mounts that do not have to be flipped. 
        /// May raise an error if the telescope is not connected. 
        /// <para>This is only available for telescope Interface Versions 2 and later.</para>
        /// </remarks>
        public bool CanSetPierSide
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return false;
                }
                return Device.CanSetPierSide;
            }
        }

        /// <summary>
        /// True if the <see cref="RightAscensionRate" /> property can be changed to provide offset tracking in the right ascension axis.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// May raise an error if the telescope is not connected. 
        /// </remarks>
        public bool CanSetRightAscensionRate => Device.CanSetRightAscensionRate;

        /// <summary>
        /// True if the <see cref="Tracking" /> property can be changed, turning telescope sidereal tracking on and off.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// May raise an error if the telescope is not connected. 
        /// </remarks>
        public bool CanSetTracking => Device.CanSetTracking;

        /// <summary>
        /// True if this telescope is capable of programmed slewing (synchronous or asynchronous) to equatorial coordinates
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// If this is true, then only the synchronous equatorial slewing methods are guaranteed to be supported.
        /// See the <see cref="CanSlewAsync" /> property for the asynchronous slewing capability flag. 
        /// May raise an error if the telescope is not connected. 
        /// </remarks>
        public bool CanSlew => Device.CanSlew;

        /// <summary>
        /// True if this telescope is capable of programmed slewing (synchronous or asynchronous) to local horizontal coordinates
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// If this is true, then only the synchronous local horizontal slewing methods are guaranteed to be supported.
        /// See the <see cref="CanSlewAltAzAsync" /> property for the asynchronous slewing capability flag. 
        /// May raise an error if the telescope is not connected. 
        /// </remarks>
        public bool CanSlewAltAz => Device.CanSlewAltAz;

        /// <summary>
        /// True if this telescope is capable of programmed asynchronous slewing to local horizontal coordinates
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// This indicates the asynchronous local horizontal slewing methods are supported.
        /// If this is True, then <see cref="CanSlewAltAz" /> will also be true. 
        /// May raise an error if the telescope is not connected. 
        /// </remarks>
        public bool CanSlewAltAzAsync => Device.CanSlewAltAzAsync;

        /// <summary>
        /// True if this telescope is capable of programmed asynchronous slewing to equatorial coordinates.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// This indicates the asynchronous equatorial slewing methods are supported.
        /// If this is True, then <see cref="CanSlew" /> will also be true.
        /// May raise an error if the telescope is not connected. 
        /// </remarks>
        public bool CanSlewAsync => Device.CanSlewAsync;

        /// <summary>
        /// True if this telescope is capable of programmed synching to equatorial coordinates.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// May raise an error if the telescope is not connected. 
        /// </remarks>
        public bool CanSync => Device.CanSync;

        /// <summary>
        /// True if this telescope is capable of programmed synching to local horizontal coordinates
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// May raise an error if the telescope is not connected. 
        /// </remarks>
        public bool CanSyncAltAz => Device.CanSyncAltAz;

        /// <summary>
        /// True if this telescope is capable of programmed unparking (<see cref="Unpark" /> method).
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// If this is true, then <see cref="CanPark" /> will also be true. May raise an error if the telescope is not connected.
        /// <para>This is only available for telescope Interface Versions 2 and later.</para>
        /// </remarks>
        public bool CanUnpark
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return false;
                }
                return Device.CanUnpark;
            }
        }

        /// <summary>
        /// The declination (degrees) of the telescope's current equatorial coordinates, in the coordinate system given by the <see cref="EquatorialSystem" /> property.
        /// Reading the property will raise an error if the value is unavailable. 
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// </remarks>
        public double Declination => Device.Declination;

        /// <summary>
        /// The declination tracking rate (arcseconds per SI second, default = 0.0)
        /// </summary>
        /// <exception cref="NotImplementedException">If DeclinationRate Write is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid DeclinationRate is specified</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red;margin-bottom:0"><b>DeclinationRate Read must be implemented and must not throw a NotImplementedException. </b></p>
        /// <p style="color:red;margin-top:0"><b>DeclinationRate Write can throw a NotImplementedException.</b></p>
        /// This property, together with <see cref="RightAscensionRate" />, provides support for "offset tracking".
        /// Offset tracking is used primarily for tracking objects that move relatively slowly against the equatorial coordinate system.
        /// It also may be used by a software guiding system that controls rates instead of using the <see cref="PulseGuide">PulseGuide</see> method. 
        /// <para>
        /// <b>NOTES:</b>
        /// <list type="bullet">
        /// <list></list>
        /// <item><description>The property value represents an offset from zero motion.</description></item>
        /// <item><description>If <see cref="CanSetDeclinationRate" /> is False, this property will always return 0.</description></item>
        /// <item><description>To discover whether this feature is supported, test the <see cref="CanSetDeclinationRate" /> property.</description></item>
        /// <item><description>The supported range of this property is telescope specific, however, if this feature is supported,
        /// it can be expected that the range is sufficient to allow correction of guiding errors caused by moderate misalignment 
        /// and periodic error.</description></item>
        /// <item><description>If this property is non-zero when an equatorial slew is initiated, the telescope should continue to update the slew 
        /// destination coordinates at the given offset rate.</description></item>
        /// <item><description>This will allow precise slews to a fast-moving target with a slow-slewing telescope.</description></item>
        /// <item><description>When the slew completes, the <see cref="TargetRightAscension" /> and <see cref="TargetDeclination" /> properties should reflect the final (adjusted) destination.</description></item>
        /// <item><description>The units of this property are arcseconds per SI (atomic) second. Please note that for historic reasons the units of the <see cref="RightAscensionRate" /> property are seconds of RA per sidereal second.</description></item>
        /// </list>
        /// </para>
        /// <para>
        ///     This is not a required feature of this specification, however it is desirable. 
        /// </para>
        /// </remarks>
        public double DeclinationRate { get => Device.DeclinationRate; set => Device.DeclinationRate = value; }

        /// <summary>
        /// True if the telescope or driver applies atmospheric refraction to coordinates.
        /// </summary>
        /// <exception cref="NotImplementedException">Either read or write or both properties can throw NotImplementedException if not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// If this property is True, the coordinates sent to, and retrieved from, the telescope are unrefracted. 
        /// <para>This is only available for telescope Interface Versions 2 and later.</para>
        /// <para>
        /// <b>NOTES:</b>
        /// <list type="bullet">
        /// <item><description>If the driver does not know whether the attached telescope does its own refraction, and if the driver does not itself calculate 
        /// refraction, this property (if implemented) must raise an error when read.</description></item>
        /// <item><description>Writing to this property is optional. Often, a telescope (or its driver) calculates refraction using standard atmospheric parameters.</description></item>
        /// <item><description>If the client wishes to calculate a more accurate refraction, then this property could be set to False and these 
        /// client-refracted coordinates used.</description></item>
        /// <item><description>If disabling the telescope or driver's refraction is not supported, the driver must raise an error when an attempt to set 
        /// this property to False is made.</description></item> 
        /// <item><description>Setting this property to True for a telescope or driver that does refraction, or to False for a telescope or driver that 
        /// does not do refraction, shall not raise an error. It shall have no effect.</description></item> 
        /// </list>
        /// </para>
        /// </remarks>
        public bool DoesRefraction { get => Device.DoesRefraction; set => Device.DoesRefraction = value; }

        /// <summary>
        /// Equatorial coordinate system used by this telescope (e.g. Topocentric or J2000).
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// Most amateur telescopes use topocentric coordinates. This coordinate system is simply the apparent position in the sky
        /// (possibly uncorrected for atmospheric refraction) for "here and now", thus these are the coordinates that one would use with digital setting
        /// circles and most amateur scopes. More sophisticated telescopes use one of the standard reference systems established by professional astronomers.
        /// The most common is the Julian Epoch 2000 (J2000). These instruments apply corrections for precession,nutation, aberration, etc. to adjust the coordinates 
        /// from the standard system to the pointing direction for the time and location of "here and now". 
        /// <para>This is only available for telescope Interface Versions 2 and later.</para>
        /// </remarks>
        public EquatorialCoordinateType EquatorialSystem
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.NotImplementedException("EquatorialSystem is only supported by Interface Versions 2 and above.");
                }
                return (EquatorialCoordinateType)Device.EquatorialSystem;
            }
        }

        /// <summary>
        /// The telescope's focal length, meters
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// This property may be used by clients to calculate telescope field of view and plate scale when combined with detector pixel size and geometry. 
        /// <para>This is only available for telescope Interface Versions 2 and later.</para>
        /// </remarks>
        public double FocalLength
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.NotImplementedException("FocalLength is only supported by Interface Versions 2 and above.");
                }
                return Device.FocalLength;
            }
        }

        /// <summary>
        /// The current Declination movement rate offset for telescope guiding (degrees/sec)
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="InvalidValueException">If an invalid guide rate is set.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks> 
        /// This is the rate for both hardware/relay guiding and the PulseGuide() method. 
        /// <para>This is only available for telescope Interface Versions 2 and later.</para>
        /// <para>
        /// <b>NOTES:</b>
        /// <list type="bullet">
        /// <item><description>To discover whether this feature is supported, test the <see cref="CanSetGuideRates" /> property.</description></item> 
        /// <item><description>The supported range of this property is telescope specific, however, if this feature is supported, it can be expected that the range is sufficient to
        /// allow correction of guiding errors caused by moderate misalignment and periodic error.</description></item> 
        /// <item><description>If a telescope does not support separate guiding rates in Right Ascension and Declination, then it is permissible for <see cref="GuideRateRightAscension" /> and GuideRateDeclination to be tied together.
        /// In this case, changing one of the two properties will cause a change in the other.</description></item> 
        /// <item><description>Mounts must start up with a known or default declination guide rate, and this property must return that known/default guide rate until changed.</description></item> 
        /// </list>
        /// </para>
        /// </remarks>
        public double GuideRateDeclination
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.NotImplementedException("GuideRateDeclination is only supported by Interface Versions 2 and above.");
                }
                return Device.GuideRateDeclination;
            }
            set
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.NotImplementedException("GuideRateDeclination is only supported by Interface Versions 2 and above.");
                }
                Device.GuideRateDeclination = value;
            }
        }

        /// <summary>
        /// The current Right Ascension movement rate offset for telescope guiding (degrees/sec)
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="InvalidValueException">If an invalid guide rate is set.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// This is the rate for both hardware/relay guiding and the PulseGuide() method. 
        /// <para>This is only available for telescope Interface Versions 2 and later.</para>
        /// <para>
        /// <b>NOTES:</b>
        /// <list type="bullet">
        /// <item><description>To discover whether this feature is supported, test the <see cref="CanSetGuideRates" /> property.</description></item>  
        /// <item><description>The supported range of this property is telescope specific, however, if this feature is supported, it can be expected that the range is sufficient to allow correction of guiding errors caused by moderate
        /// misalignment and periodic error.</description></item>  
        /// <item><description>If a telescope does not support separate guiding rates in Right Ascension and Declination, then it is permissible for GuideRateRightAscension and <see cref="GuideRateDeclination" /> to be tied together. 
        /// In this case, changing one of the two properties will cause a change in the other.</description></item>  
        ///     <item><description> Mounts must start up with a known or default right ascension guide rate, and this property must return that known/default guide rate until changed.</description></item>  
        /// </list>
        /// </para>
        /// </remarks>
        public double GuideRateRightAscension
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.NotImplementedException("GuideRateRightAscension is only supported by Interface Versions 2 and above.");
                }
                return Device.GuideRateRightAscension;
            }
            set
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.NotImplementedException("GuideRateRightAscension is only supported by Interface Versions 2 and above.");
                }
                Device.GuideRateRightAscension = value;
            }
        }

        /// <summary>
        /// True if a <see cref="PulseGuide" /> command is in progress, False otherwise
        /// </summary>
        /// <exception cref="NotImplementedException">If <see cref="CanPulseGuide" /> is False</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Raises an error if the value of the <see cref="CanPulseGuide" /> property is false (the driver does not support the <see cref="PulseGuide" /> method). 
        /// </remarks>
        public bool IsPulseGuiding => Device.IsPulseGuiding;

        /// <summary>
        /// The right ascension (hours) of the telescope's current equatorial coordinates,
        /// in the coordinate system given by the EquatorialSystem property
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// Reading the property will raise an error if the value is unavailable. 
        /// </remarks>
        public double RightAscension => Device.RightAscension;

        /// <summary>
        /// The right ascension tracking rate offset from sidereal (seconds per sidereal second, default = 0.0)
        /// </summary>
        /// <exception cref="NotImplementedException">If RightAscensionRate Write is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid rate is set.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red;margin-bottom:0"><b>RightAscensionRate Read must be implemented and must not throw a NotImplementedException. </b></p>
        /// <p style="color:red;margin-top:0"><b>RightAscensionRate Write can throw a NotImplementedException.</b></p>
        /// This property, together with <see cref="DeclinationRate" />, provides support for "offset tracking". Offset tracking is used primarily for tracking objects that move relatively slowly
        /// against the equatorial coordinate system. It also may be used by a software guiding system that controls rates instead of using the <see cref="PulseGuide">PulseGuide</see> method.
        /// <para>
        /// <b>NOTES:</b>
        /// The property value represents an offset from the currently selected <see cref="TrackingRate" />. 
        /// <list type="bullet">
        /// <item><description>If this property is zero, tracking will be at the selected <see cref="TrackingRate" />.</description></item>
        /// <item><description>If <see cref="CanSetRightAscensionRate" /> is False, this property must always return 0.</description></item> 
        /// To discover whether this feature is supported, test the <see cref="CanSetRightAscensionRate" />property. 
        /// <item><description>The units of this property are seconds of right ascension per sidereal second. Please note that for historic reasons the units of the <see cref="DeclinationRate" /> property are arcseconds per SI second.</description></item> 
        /// <item><description>To convert a given rate in (the more common) units of sidereal seconds per UTC (clock) second, multiply the value by 0.9972695677 
        /// (the number of UTC seconds in a sidereal second) then set the property. Please note that these units were chosen for the Telescope V1 standard,
        /// and in retrospect, this was an unfortunate choice. However, to maintain backwards compatibility, the units cannot be changed.
        /// A simple multiplication is all that's needed, as noted. The supported range of this property is telescope specific, however,
        /// if this feature is supported, it can be expected that the range is sufficient to allow correction of guiding errors
        /// caused by moderate misalignment and periodic error. </description></item>
        /// <item><description>If this property is non-zero when an equatorial slew is initiated, the telescope should continue to update the slew destination coordinates 
        /// at the given offset rate. This will allow precise slews to a fast-moving target with a slow-slewing telescope. When the slew completes, 
        /// the <see cref="TargetRightAscension" /> and <see cref="TargetDeclination" /> properties should reflect the final (adjusted) destination. This is not a required
        /// feature of this specification, however it is desirable. </description></item>
        /// <item><description>Use the <see cref="Tracking" /> property to enable and disable sidereal tracking (if supported). </description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public double RightAscensionRate { get => Device.RightAscensionRate; set => Device.RightAscensionRate = value; }

        /// <summary>
        /// Indicates the pointing state of the mount.
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid side of pier is set.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <para>For historical reasons, this property's name does not reflect its true meaning. The name will not be changed (so as to preserve 
        /// compatibility), but the meaning has since become clear. All conventional mounts have two pointing states for a given equatorial (sky) position. 
        /// Mechanical limitations often make it impossible for the mount to position the optics at given HA/Dec in one of the two pointing 
        /// states, but there are places where the same point can be reached sensibly in both pointing states (e.g. near the pole and 
        /// close to the meridian). In order to understand these pointing states, consider the following (thanks to Patrick Wallace for this info):</para>
        /// <para>All conventional telescope mounts have two axes nominally at right angles. For an equatorial, the longitude axis is mechanical 
        /// hour angle and the latitude axis is mechanical declination. Sky coordinates and mechanical coordinates are two completely separate arenas. 
        /// This becomes rather more obvious if your mount is an altaz, but it's still true for an equatorial. Both mount axes can in principle 
        /// move over a range of 360 deg. This is distinct from sky HA/Dec, where Dec is limited to a 180 deg range (+90 to -90).  Apart from 
        /// practical limitations, any point in the sky can be seen in two mechanical orientations. To get from one to the other the HA axis 
        /// is moved 180 deg and the Dec axis is moved through the pole a distance twice the sky codeclination (90 - sky declination).</para>
        /// <para>Mechanical zero HA/Dec will be one of the two ways of pointing at the intersection of the celestial equator and the local meridian. 
        /// In order to support Dome slaving, where it is important to know which side of the pier the mount is actually on, ASCOM has adopted the 
        /// convention that the Normal pointing state will be the state where a German Equatorial mount is on the East side of the pier, looking West, with the 
        /// counterweights below the optical assembly and that <see cref="PointingState.Normal"></see> will represent this pointing state.</para>
        /// <para>Move your scope to this position and consider the two mechanical encoders zeroed. The two pointing states are, then:
        /// <list type="table">
        /// <item><term><b>Normal (<see cref="PointingState.Normal"></see>)</b></term><description>Where the mechanical Dec is in the range -90 deg to +90 deg</description></item>
        /// <item><term><b>Beyond the pole (<see cref="PointingState.ThroughThePole"></see>)</b></term><description>Where the mechanical Dec is in the range -180 deg to -90 deg or +90 deg to +180 deg.</description></item>
        /// </list>
        /// </para>
        /// <para>"Side of pier" is a "consequence" of the former definition, not something fundamental. 
        /// Apart from mechanical interference, the telescope can move from one side of the pier to the other without the mechanical Dec 
        /// having changed: you could track Polaris forever with the telescope moving from west of pier to east of pier or vice versa every 12h. 
        /// Thus, "side of pier" is, in general, not a useful term (except perhaps in a loose, descriptive, explanatory sense). 
        /// All this applies to a fork mount just as much as to a GEM, and it would be wrong to make the "beyond pole" state illegal for the 
        /// former. Your mount may not be able to get there if your camera hits the fork, but it's possible on some mounts. Whether this is useful 
        /// depends on whether you're in Hawaii or Finland.</para>
        /// <para>To first order, the relationship between sky and mechanical HA/Dec is as follows:</para>
        /// <para><b>Normal state:</b>
        /// <list type="bullet">
        /// <item><description>HA_sky  = HA_mech</description></item>
        /// <item><description>Dec_sky = Dec_mech</description></item>
        /// </list>
        /// </para>
        /// <para><b>Beyond the pole</b>
        /// <list type="bullet">
        /// <item><description>HA_sky  = HA_mech + 12h, expressed in range ± 12h</description></item>
        /// <item><description>Dec_sky = 180d - Dec_mech, expressed in range ± 90d</description></item>
        /// </list>
        /// </para>
        /// <para>Astronomy software often needs to know which pointing state the mount is in. Examples include setting guiding polarities 
        /// and calculating dome opening azimuth/altitude. The meaning of the SideOfPier property, then is:
        /// <list type="table">
        /// <item><term><b>pierEast</b></term><description>Normal pointing state</description></item>
        /// <item><term><b>pierWest</b></term><description>Beyond the pole pointing state</description></item>
        /// </list>
        /// </para>
        /// <para>If the mount hardware reports neither the true pointing state (or equivalent) nor the mechanical declination axis position 
        /// (which varies from -180 to +180), a driver cannot calculate the pointing state, and *must not* implement SideOfPier.
        /// If the mount hardware reports only the mechanical declination axis position (-180 to +180) then a driver can calculate SideOfPier as follows:
        /// <list type="bullet">
        /// <item><description>pierEast = abs(mechanical dec) &lt;= 90 deg</description></item>
        /// <item><description>pierWest = abs(mechanical Dec) &gt; 90 deg</description></item>
        /// </list>
        /// </para>
        /// <para>It is allowed (though not required) that this property may be written to force the mount to flip. Doing so, however, may change 
        /// the right ascension of the telescope. During flipping, Telescope.Slewing must return True.</para>
        /// <para>This property is only available in telescope Interface Versions 2 and later.</para>
        /// <para><b>Pointing State and Side of Pier - Help for Driver Developers</b></para>
        /// <para>A further document, "Pointing State and Side of Pier", is installed in the Developer Documentation folder by the ASCOM Developer 
        /// Components installer. This further explains the pointing state concept and includes diagrams illustrating how it relates 
        /// to physical side of pier for German equatorial telescopes. It also includes details of the tests performed by Conform to determine whether 
        /// the driver correctly reports the pointing state as defined above.</para>
        /// </remarks>
        public PointingState SideOfPier
        {
            get => (PointingState)Device.SideOfPier;
            set
            {
                currentOperation = Operation.SideOfPier;
                Device.SideOfPier = value;
            }
        }

        /// <summary>
        /// The local apparent sidereal time from the telescope's internal clock (hours, sidereal)
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// It is required for a driver to calculate this from the system clock if the telescope 
        /// has no accessible source of sidereal time. Local Apparent Sidereal Time is the sidereal 
        /// time used for pointing telescopes, and thus must be calculated from the Greenwich Mean
        /// Sidereal time, longitude, nutation in longitude and true ecliptic obliquity. 
        /// </remarks>
        public double SiderealTime => Device.SiderealTime;

        /// <summary>
        /// The elevation above mean sea level (meters) of the site at which the telescope is located
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid elevation is set.</exception>
        /// <exception cref="InvalidOperationException">If the application must set the elevation before reading it, but has not.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Setting this property will raise an error if the given value is outside the range -300 through +10000 metres.
        /// Reading the property will raise an error if the value has never been set or is otherwise unavailable. 
        /// <para>This is only available for telescope Interface Versions 2 and later.</para>
        /// </remarks>
        public double SiteElevation
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.NotImplementedException("SiteElevation is only supported by Interface Versions 2 and above.");
                }
                return Device.SiteElevation;
            }
            set
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.NotImplementedException("SiteElevation is only supported by Interface Versions 2 and above.");
                }
                Device.SiteElevation = value;
            }
        }

        /// <summary>
        /// The geodetic(map) latitude (degrees, positive North, WGS84) of the site at which the telescope is located.
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid latitude is set.</exception>
        /// <exception cref="InvalidOperationException">If the application must set the latitude before reading it, but has not.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Setting this property will raise an error if the given value is outside the range -90 to +90 degrees.
        /// Reading the property will raise an error if the value has never been set or is otherwise unavailable. 
        /// <para>This is only available for telescope Interface Versions 2 and later.</para>
        /// </remarks>
        public double SiteLatitude
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.NotImplementedException("SiteLatitude is only supported by Interface Versions 2 and above.");
                }
                return Device.SiteLatitude;
            }
            set
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.NotImplementedException("SiteLatitude is only supported by Interface Versions 2 and above.");
                }
                Device.SiteLatitude = value;
            }
        }

        /// <summary>
        /// The longitude (degrees, positive East, WGS84) of the site at which the telescope is located.
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid longitude is set.</exception>
        /// <exception cref="InvalidOperationException">If the application must set the longitude before reading it, but has not.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Setting this property will raise an error if the given value is outside the range -180 to +180 degrees.
        /// Reading the property will raise an error if the value has never been set or is otherwise unavailable.
        /// Note that West is negative! 
        /// <para>This is only available for telescope Interface Versions 2 and later.</para>
        /// </remarks>
        public double SiteLongitude
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.NotImplementedException("SiteLongitude is only supported by Interface Versions 2 and above.");
                }
                return Device.SiteLongitude;
            }
            set
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.NotImplementedException("SiteLongitude is only supported by Interface Versions 2 and above.");
                }
                Device.SiteLongitude = value;
            }
        }

        /// <summary>
        /// True if telescope is currently moving in response to one of the
        /// Slew methods or the <see cref="MoveAxis" /> method, False at all other times.
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Reading the property will raise an error if the value is unavailable. If the telescope is not capable of asynchronous slewing, this property will always be False. 
        /// The definition of "slewing" excludes motion caused by sidereal tracking, <see cref="PulseGuide">PulseGuide</see>, <see cref="RightAscensionRate" />, and <see cref="DeclinationRate" />.
        /// It reflects only motion caused by one of the Slew commands, flipping caused by changing the <see cref="SideOfPier" /> property, or <see cref="MoveAxis" />. 
        /// </remarks>
        public bool Slewing => Device.Slewing;

        /// <summary>
        /// Specifies a post-slew settling time (sec.).
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid settle time is set.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Adds additional time to slew operations. Slewing methods will not return, 
        /// and the <see cref="Slewing" /> property will not become False, until the slew completes and the SlewSettleTime has elapsed.
        /// This feature (if supported) may be used with mounts that require extra settling time after a slew. 
        /// </remarks>
        public short SlewSettleTime { get => Device.SlewSettleTime; set => Device.SlewSettleTime = value; }

        /// <summary>
        /// The declination (degrees, positive North) for the target of an equatorial slew or sync operation
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid declination is set.</exception>
        /// <exception cref="InvalidOperationException">If the property is read before being set for the first time.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Setting this property will raise an error if the given value is outside the range -90 to +90 degrees. Reading the property will raise an error if the value has never been set or is otherwise unavailable. 
        /// </remarks>
        public double TargetDeclination { get => Device.TargetDeclination; set => Device.TargetDeclination = value; }

        /// <summary>
        /// The right ascension (hours) for the target of an equatorial slew or sync operation
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid right ascension is set.</exception>
        /// <exception cref="InvalidOperationException">If the property is read before being set for the first time.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Setting this property will raise an error if the given value is outside the range 0 to 24 hours. Reading the property will raise an error if the value has never been set or is otherwise unavailable. 
        /// </remarks>
        public double TargetRightAscension { get => Device.TargetRightAscension; set => Device.TargetRightAscension = value; }

        /// <summary>
        /// The state of the telescope's sidereal tracking drive.
        /// </summary>
        /// <exception cref="NotImplementedException">If Tracking Write is not implemented.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red;margin-bottom:0"><b>Tracking Read must be implemented and must not throw a NotImplementedException. </b></p>
        /// <p style="color:red;margin-top:0"><b>Tracking Write can throw a NotImplementedException.</b></p>
        /// Changing the value of this property will turn the sidereal drive on and off.
        /// However, some telescopes may not support changing the value of this property
        /// and thus may not support turning tracking on and off.
        /// See the <see cref="CanSetTracking" /> property. 
        /// </remarks>
        public bool Tracking { get => Device.Tracking; set => Device.Tracking = value; }

        /// <summary>
        /// The current tracking rate of the telescope's sidereal drive
        /// </summary>
        /// <exception cref="NotImplementedException">If TrackingRate Write is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid drive rate is set.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red;margin-bottom:0"><b>TrackingRate Read must be implemented and must not throw a NotImplementedException. </b></p>
        /// <p style="color:red;margin-top:0"><b>TrackingRate Write can throw a NotImplementedException.</b></p>
        /// Supported rates (one of the <see cref="DriveRate" />  values) are contained within the <see cref="TrackingRates" /> collection.
        /// Values assigned to TrackingRate must be one of these supported rates. If an unsupported value is assigned to this property, it will raise an error. 
        /// The currently selected tracking rate can be further adjusted via the <see cref="RightAscensionRate" /> and <see cref="DeclinationRate" /> properties. These rate offsets are applied to the currently 
        /// selected tracking rate. Mounts must start up with a known or default tracking rate, and this property must return that known/default tracking rate until changed.
        /// <para>If the mount's current tracking rate cannot be determined (for example, it is a write-only property of the mount's protocol), 
        /// it is permitted for the driver to force and report a default rate on connect. In this case, the preferred default is Sidereal rate.</para>
        /// <para>This is only available for telescope Interface Versions 2 and later.</para>
        /// </remarks>
        public DriveRate TrackingRate
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.NotImplementedException("TrackingRate is only supported by Interface Versions 2 and above.");
                }
                return (DriveRate)Device.TrackingRate;
            }
            set
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.NotImplementedException("TrackingRate is only supported by Interface Versions 2 and above.");
                }
                Device.TrackingRate = value;
            }
        }

        /// <summary>
        /// Returns a collection of supported <see cref="DriveRate" /> values that describe the permissible
        /// values of the <see cref="TrackingRate" /> property for this telescope type.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented and must not throw a NotImplementedException.</b></p>
        /// At a minimum, this must contain an item for <see cref="DriveRate.Sidereal" />.
        /// <para>This is only available for telescope Interface Versions 2 and later.</para>
        /// </remarks>
        public ITrackingRates TrackingRates
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.NotImplementedException("TrackingRates is only supported by Interface Versions 2 and above.");
                }

                TrackingRates rates = new TrackingRates();

                foreach (var rate in (Device.TrackingRates))
                {
                    try
                    {
                        if (rate != null && Enum.IsDefined(typeof(DriveRate), rate))
                        {
                            rates.Add((DriveRate)rate);
                        }
                    }
                    catch
                    {

                    }
                }
                return rates;
            }
        }

        /// <summary>
        /// The UTC date/time of the telescope's internal clock
        /// </summary>
        /// <exception cref="NotImplementedException">If UTCDate Write is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid <see cref="DateTime" /> is set.</exception>
        /// <exception cref="InvalidOperationException">When UTCDate is read and the mount cannot provide this property itself and a value has not yet be established by writing to the property.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red;margin-bottom:0"><b>UTCDate Read must be implemented and must not throw a NotImplementedException. </b></p>
        /// <p style="color:red;margin-top:0"><b>UTCDate Write can throw a NotImplementedException.</b></p>
        /// The driver must calculate this from the system clock if the telescope has no accessible source of UTC time. In this case, the property must not be writeable (this would change the system clock!) and will instead raise an error.
        /// However, it is permitted to change the telescope's internal UTC clock if it is being used for this property. This allows clients to adjust the telescope's UTC clock as needed for accuracy. Reading the property
        /// will raise an error if the value has never been set or is otherwise unavailable. 
        /// </remarks>
        public DateTime UTCDate { get => Device.UTCDate; set => Device.UTCDate = value; }

        /// <summary>
        /// Stops a slew in progress.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Effective only after a call to <see cref="SlewToTargetAsync" />, <see cref="SlewToCoordinatesAsync" />, <see cref="SlewToAltAzAsync" />, or <see cref="MoveAxis" />.
        /// Does nothing if no slew/motion is in progress. Tracking is returned to its pre-slew state. Raises an error if <see cref="AtPark" /> is true. 
        /// </remarks>
        public void AbortSlew()
        {
            currentOperation = Operation.AbortSlew;
            Device.AbortSlew();
        }

        /// <summary>
        /// Determine the rates at which the telescope may be moved about the specified axis by the <see cref="MoveAxis" /> method.
        /// </summary>
        /// <param name="Axis">The axis about which rate information is desired (TelescopeAxes value)</param>
        /// <returns>Collection of <see cref="IRate" /> rate objects</returns>
        /// <exception cref="InvalidValueException">If an invalid Axis is specified.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// See the description of <see cref="MoveAxis" /> for more information. This method must return an empty collection if <see cref="MoveAxis" /> is not supported. 
        /// <para>This is only available for telescope Interface Versions 2 and later.</para>
        /// <para>
        /// Please note that the rate objects must contain absolute non-negative values only. Applications determine the direction by applying a
        /// positive or negative sign to the rates provided. This obviates the need for the driver to present a duplicate set of negative rates 
        /// as well as the positive rates.</para>
        /// </remarks>
        public IAxisRates AxisRates(TelescopeAxis Axis)
        {
            if (InterfaceVersion == 1)
            {
                throw new ASCOM.NotImplementedException("AxisRates is only supported by Interface Versions 2 and above.");
            }
            AxisRates rates = new AxisRates();

            foreach (var rate in Device.AxisRates(Axis))
            {
                rates.Add(rate.Minimum, rate.Maximum);
            }

            return rates;
        }

        /// <summary>
        /// True if this telescope can move the requested axis
        /// </summary>
        /// <exception cref="InvalidValueException">If an invalid Axis is specified.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <param name="Axis">Primary, Secondary or Tertiary axis</param>
        /// <returns>Boolean indicating can or can not move the requested axis</returns>
        /// <exception cref="InvalidValueException">If an invalid Axis is specified.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// This is only available for telescope Interface Versions 2 and later.
        /// </remarks>
        public bool CanMoveAxis(TelescopeAxis Axis)
        {
            return Device.CanMoveAxis(Axis);
        }

        /// <summary>
        /// Predict side of pier for German equatorial mounts
        /// </summary>
        /// <param name="RightAscension">The destination right ascension (hours).</param>
        /// <param name="Declination">The destination declination (degrees, positive North).</param>
        /// <returns>The side of the pier on which the telescope would be on if a slew to the given equatorial coordinates is performed at the current instant of time.</returns>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="InvalidValueException">If an invalid RightAscension or Declination is specified.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// This is only available for telescope Interface Versions 2 and later.
        /// </remarks>
        public PointingState DestinationSideOfPier(double RightAscension, double Declination)
        {
            if (InterfaceVersion == 1)
            {
                throw new ASCOM.NotImplementedException("AtPark is only supported by Interface Versions 2 and above.");
            }
            return (PointingState)Device.DestinationSideOfPier(RightAscension, Declination);
        }

        /// <summary>
        /// Locates the telescope's "home" position (synchronous)
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanFindHome" /> is False</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Returns only after the home position has been found.
        /// At this point the <see cref="AtHome" /> property will be True.
        /// Raises an error if there is a problem. 
        /// Raises an error if AtPark is true. 
        /// <para>This is only available for telescope Interface Versions 2 and later.</para>
        /// </remarks>
        public void FindHome()
        {
            if (InterfaceVersion == 1)
            {
                throw new ASCOM.NotImplementedException("FindHome is only supported by Interface Versions 2 and above.");
            }

            currentOperation = Operation.FindHome;
            Device.FindHome();
        }

        /// <summary>
        /// Move the telescope in one axis at the given rate.
        /// </summary>
        /// <param name="Axis">The physical axis about which movement is desired</param>
        /// <param name="Rate">The rate of motion (deg/sec) about the specified axis</param>
        /// <exception cref="NotImplementedException">If the method is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid axis or rate is given.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// This method supports control of the mount about its mechanical axes.
        /// The telescope will start moving at the specified rate about the specified axis and continue indefinitely.
        /// This method can be called for each axis separately, and have them all operate concurrently at separate rates of motion. 
        /// Set the rate for an axis to zero to restore the motion about that axis to the rate set by the <see cref="Tracking"/> property.
        /// Tracking motion (if enabled, see note below) is suspended during this mode of operation. 
        /// <para>
        /// Raises an error if <see cref="AtPark" /> is true. 
        /// This must be implemented for the if the <see cref="CanMoveAxis" /> property returns True for the given axis.</para>
        /// <para>This is only available for telescope Interface Versions 2 and later.</para>
        /// <para>
        /// <b>NOTES:</b>
        /// <list type="bullet">
        /// <item><description>The movement rate must be within the value(s) obtained from a <see cref="IRate" /> object in the 
        /// the <see cref="AxisRates" /> collection. This is a signed value with negative rates moving in the opposite direction to positive rates.</description></item>
        /// <item><description>The values specified in <see cref="AxisRates" /> are absolute, unsigned values and apply to both directions, determined by the sign used in this command.</description></item>
        /// <item><description>The value of <see cref="Slewing" /> must be True if the telescope is moving about any of its axes as a result of this method being called. 
        /// This can be used to simulate a handbox by initiating motion with the MouseDown event and stopping the motion with the MouseUp event.</description></item>
        /// <item><description>When the motion is stopped by setting the rate to zero the scope will be set to the previous <see cref="TrackingRate" /> or to no movement, depending on the state of the <see cref="Tracking" /> property.</description></item>
        /// <item><description>It may be possible to implement satellite tracking by using the <see cref="MoveAxis" /> method to move the scope in the required manner to track a satellite.</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public void MoveAxis(TelescopeAxis Axis, double Rate)
        {
            currentOperation = Operation.MoveAxis;
            Device.MoveAxis(Axis, Rate);
        }

        /// <summary>
        /// Move the telescope to its park position, stop all motion (or restrict to a small safe range), and set <see cref="AtPark" /> to True.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanPark" /> is False</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Raises an error if there is a problem communicating with the telescope or if parking fails. Parking should put the telescope into a state where its pointing accuracy 
        /// will not be lost if it is power-cycled (without moving it).Some telescopes must be power-cycled before unparking. Others may be unparked by simply calling the <see cref="Unpark" /> method.
        /// Calling this with <see cref="AtPark" /> = True does nothing (harmless) 
        /// </remarks>
        public void Park()
        {
            currentOperation = Operation.Park;
            Device.Park();
        }

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
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// This method returns immediately if the hardware is capable of back-to-back moves,
        /// i.e. dual-axis moves. For hardware not having the dual-axis capability,
        /// the method returns only after the move has completed. 
        /// <para>
        /// <b>NOTES:</b>
        /// <list type="bullet">
        /// <item><description>Raises an error if <see cref="AtPark" /> is true.</description></item>
        /// <item><description>The <see cref="IsPulseGuiding" /> property must be True during pulse-guiding.</description></item>
        /// <item><description>The rate of motion for movements about the right ascension axis is 
        /// specified by the <see cref="GuideRateRightAscension" /> property. The rate of motion
        /// for movements about the declination axis is specified by the 
        /// <see cref="GuideRateDeclination" /> property. These two rates may be tied together
        /// into a single rate, depending on the driver's implementation
        /// and the capabilities of the telescope.</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public void PulseGuide(GuideDirection Direction, int Duration)
        {
            currentOperation = Operation.PulseGuide;
            Device.PulseGuide(Direction, Duration);
        }

        /// <summary>
        /// Sets the telescope's park position to be its current position.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanPark" /> is False</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public void SetPark()
        {
            Device.SetPark();
        }

        /// <summary>
        /// Move the telescope to the given local horizontal coordinates, return when slew is complete
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanSlewAltAz" /> is False</exception>
        /// <exception cref="InvalidValueException">If an invalid azimuth or elevation is given.</exception>
        /// <exception cref="ParkedException">If the telescope is parked</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// This Method must be implemented if <see cref="CanSlewAltAz" /> returns True. Raises an error if the slew fails. The slew may fail if the target coordinates are beyond limits imposed within the driver component.
        /// Such limits include mechanical constraints imposed by the mount or attached instruments, building or dome enclosure restrictions, etc.
        /// <para>The <see cref="TargetRightAscension" /> and <see cref="TargetDeclination" /> properties are not changed by this method. 
        /// Raises an error if <see cref="AtPark" /> is True, or if <see cref="Tracking" /> is True. This is only available for telescope Interface Versions 2 and later.</para>
        /// </remarks>
        /// <param name="Azimuth">Target azimuth (degrees, North-referenced, positive East/clockwise).</param>
        /// <param name="Altitude">Target altitude (degrees, positive up)</param>
        public void SlewToAltAz(double Azimuth, double Altitude)
        {
            if (InterfaceVersion == 1)
            {
                throw new ASCOM.NotImplementedException("SlewToAltAz is only supported by Interface Versions 2 and above.");
            }
            Device.SlewToAltAz(Azimuth, Altitude);
        }

        /// <summary>
        /// This Method must be implemented if <see cref="CanSlewAltAzAsync" /> returns True.
        /// </summary>
        /// <param name="Azimuth">Azimuth to which to move</param>
        /// <param name="Altitude">Altitude to which to move to</param>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanSlewAltAzAsync" /> is False</exception>
        /// <exception cref="InvalidValueException">If an invalid azimuth or elevation is given.</exception>
        /// <exception cref="ParkedException">If the telescope is parked</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// This method should only be implemented if the properties <see cref="Altitude" />, <see cref="Azimuth" />,
        /// <see cref="RightAscension" />, <see cref="Declination" /> and <see cref="Slewing" /> can be read while the scope is slewing. Raises an error if starting the slew fails. Returns immediately after starting the slew.
        /// The client may monitor the progress of the slew by reading the <see cref="Azimuth" />, <see cref="Altitude" />, and <see cref="Slewing" /> properties during the slew. When the slew completes, Slewing becomes False. 
        /// The slew may fail if the target coordinates are beyond limits imposed within the driver component. Such limits include mechanical constraints imposed by the mount or attached instruments, building or dome enclosure restrictions, etc. 
        /// The <see cref="TargetRightAscension" /> and <see cref="TargetDeclination" /> properties are not changed by this method. 
        /// <para>Raises an error if <see cref="AtPark" /> is True, or if <see cref="Tracking" /> is True.</para>
        /// <para>This is only available for telescope Interface Versions 2 and later.</para>
        /// </remarks>
        public void SlewToAltAzAsync(double Azimuth, double Altitude)
        {
            if (InterfaceVersion == 1)
            {
                throw new ASCOM.NotImplementedException("SlewToAltAzAsync is only supported by Interface Versions 2 and above.");
            }

            currentOperation = Operation.SlewToAltAzAsync;
            Device.SlewToAltAzAsync(Azimuth, Altitude);
        }

        /// <summary>
        /// Move the telescope to the given equatorial coordinates, return when slew is complete
        /// </summary>
        /// <exception cref="InvalidValueException">If an invalid right ascension or declination is given.</exception>
        /// <param name="RightAscension">The destination right ascension (hours). Copied to <see cref="TargetRightAscension" />.</param>
        /// <param name="Declination">The destination declination (degrees, positive North). Copied to <see cref="TargetDeclination" />.</param>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanSlew" /> is False</exception>
		/// <exception cref="ParkedException">If the telescope is parked</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// This Method must be implemented if <see cref="CanSlew" /> returns True. Raises an error if the slew fails. 
        /// The slew may fail if the target coordinates are beyond limits imposed within the driver component.
        /// Such limits include mechanical constraints imposed by the mount or attached instruments,
        /// building or dome enclosure restrictions, etc. The target coordinates are copied to
        /// <see cref="TargetRightAscension" /> and <see cref="TargetDeclination" /> whether or not the slew succeeds. 
        /// <para>Raises an error if <see cref="AtPark" /> is True, or if <see cref="Tracking" /> is False.</para>
        /// </remarks>
        public void SlewToCoordinates(double RightAscension, double Declination)
        {
            Device.SlewToCoordinates(RightAscension, Declination);
        }

        /// <summary>
        /// Move the telescope to the given equatorial coordinates, return immediately after starting the slew.
        /// </summary>
        /// <param name="RightAscension">The destination right ascension (hours). Copied to <see cref="TargetRightAscension" />.</param>
        /// <param name="Declination">The destination declination (degrees, positive North). Copied to <see cref="TargetDeclination" />.</param>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanSlewAsync" /> is False</exception>
        /// <exception cref="ParkedException">If the telescope is parked</exception>
        /// <exception cref="InvalidValueException">If an invalid right ascension or declination is given.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// This method must be implemented if <see cref="CanSlewAsync" /> returns True. Raises an error if starting the slew failed. 
        /// Returns immediately after starting the slew. The client may monitor the progress of the slew by reading
        /// the <see cref="RightAscension" />, <see cref="Declination" />, and <see cref="Slewing" /> properties during the slew. When the slew completes,
        /// <see cref="Slewing" /> becomes False. The slew may fail to start if the target coordinates are beyond limits
        /// imposed within the driver component. Such limits include mechanical constraints imposed
        /// by the mount or attached instruments, building or dome enclosure restrictions, etc. 
        /// <para>The target coordinates are copied to <see cref="TargetRightAscension" /> and <see cref="TargetDeclination" />
        /// whether or not the slew succeeds. 
        /// Raises an error if <see cref="AtPark" /> is True, or if <see cref="Tracking" /> is False.</para>
        /// </remarks>
        public void SlewToCoordinatesAsync(double RightAscension, double Declination)
        {
            currentOperation = Operation.SlewToCoordinatesAsync;
            Device.SlewToCoordinatesAsync(RightAscension, Declination);
        }

        /// <summary>
        /// Move the telescope to the <see cref="TargetRightAscension" /> and <see cref="TargetDeclination" /> coordinates, return when slew complete.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanSlew" /> is False</exception>
        /// <exception cref="ParkedException">If the telescope is parked</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// This Method must be implemented if <see cref="CanSlew" /> returns True. Raises an error if the slew fails. 
        /// The slew may fail if the target coordinates are beyond limits imposed within the driver component.
        /// Such limits include mechanical constraints imposed by the mount or attached
        /// instruments, building or dome enclosure restrictions, etc. 
        /// Raises an error if <see cref="AtPark" /> is True, or if <see cref="Tracking" /> is False. 
        /// </remarks>
        public void SlewToTarget()
        {
            Device.SlewToTarget();
        }

        /// <summary>
        /// Move the telescope to the <see cref="TargetRightAscension" /> and <see cref="TargetDeclination" />  coordinates,
        /// returns immediately after starting the slew.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanSlewAsync" /> is False</exception>
        /// <exception cref="ParkedException">If the telescope is parked</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// This Method must be implemented if  <see cref="CanSlewAsync" /> returns True.
        /// Raises an error if starting the slew failed. Returns immediately after starting the slew. The client may monitor the progress of the slew by reading the RightAscension, Declination,
        /// and Slewing properties during the slew. When the slew completes,  <see cref="Slewing" /> becomes False. The slew may fail to start if the target coordinates are beyond limits imposed within 
        /// the driver component. Such limits include mechanical constraints imposed by the mount or attached instruments, building or dome enclosure restrictions, etc. 
        /// Raises an error if <see cref="AtPark" /> is True, or if <see cref="Tracking" /> is False. 
        /// </remarks>
        public void SlewToTargetAsync()
        {
            currentOperation = Operation.SlewToTargetAsync;
            Device.SlewToTargetAsync();
        }

        /// <summary>
        /// Matches the scope's local horizontal coordinates to the given local horizontal coordinates.
        /// </summary>
        /// <param name="Azimuth">Target azimuth (degrees, North-referenced, positive East/clockwise)</param>
        /// <param name="Altitude">Target altitude (degrees, positive up)</param>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanSyncAltAz" /> is False</exception>
        /// <exception cref="InvalidValueException">If an invalid azimuth or altitude is given.</exception>
        /// <exception cref="ParkedException">If the telescope is parked</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// This must be implemented if the <see cref="CanSyncAltAz" /> property is True. Raises an error if matching fails. 
        /// <para>Raises an error if <see cref="AtPark" /> is True, or if <see cref="Tracking" /> is True.</para>
        /// <para>This is only available for telescope Interface Versions 2 and later.</para>
        /// </remarks>
        public void SyncToAltAz(double Azimuth, double Altitude)
        {
            if (InterfaceVersion == 1)
            {
                throw new ASCOM.NotImplementedException("SyncToAltAz is only supported by Interface Versions 2 and above.");
            }
            Device.SyncToAltAz(Azimuth, Altitude);
        }

        /// <summary>
        /// Matches the scope's equatorial coordinates to the given equatorial coordinates.
        /// </summary>
        /// <param name="RightAscension">The corrected right ascension (hours). Copied to the <see cref="TargetRightAscension" /> property.</param>
        /// <param name="Declination">The corrected declination (degrees, positive North). Copied to the <see cref="TargetDeclination" /> property.</param>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanSync" /> is False</exception>
        /// <exception cref="InvalidValueException">If an invalid right ascension or declination is given.</exception>
        /// <exception cref="ParkedException">If the telescope is parked</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// This must be implemented if the <see cref="CanSync" /> property is True. Raises an error if matching fails. 
        /// Raises an error if <see cref="AtPark" /> AtPark is True, or if <see cref="Tracking" /> is False. 
        /// The way that Sync is implemented is mount dependent and it should only be relied on to improve pointing for positions close to
        /// the position at which the sync is done.
        /// </remarks>
        public void SyncToCoordinates(double RightAscension, double Declination)
        {
            Device.SyncToCoordinates(RightAscension, Declination);
        }

        /// <summary>
        /// Matches the scope's equatorial coordinates to the given equatorial coordinates.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanSync" /> is False</exception>
        /// <exception cref="ParkedException">If the telescope is parked</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// This must be implemented if the <see cref="CanSync" /> property is True. Raises an error if matching fails. 
        /// Raises an error if <see cref="AtPark" /> AtPark is True, or if <see cref="Tracking" /> is False. 
        /// The way that Sync is implemented is mount dependent and it should only be relied on to improve pointing for positions close to
        /// the position at which the sync is done.
        /// </remarks>
        public void SyncToTarget()
        {
            Device.SyncToTarget();
        }

        /// <summary>
        /// Takes telescope out of the Parked state.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented and <see cref="CanUnpark" /> is False</exception>
        /// <exception cref="ParkedException">If the telescope is parked</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// The state of <see cref="Tracking" /> after unparking is undetermined. Valid only after <see cref="Park" />. Applications must check and change Tracking as needed after unparking. 
        /// Raises an error if unparking fails. Calling this with <see cref="AtPark" /> = False does nothing (harmless) 
        /// </remarks>
        public void Unpark()
        {
            currentOperation = Operation.Unpark;

            Device.Unpark();
        }

        #endregion

        #region ITelescopeV4

        // No new telescope members

        #endregion
    }

    /// <summary>
    /// Returns a collection of supported DriveRate values that describe the permissible values of the TrackingRate property for this telescope type.
    /// </summary>
    public class TrackingRates : ITrackingRates, IEnumerable, IEnumerator, IDisposable
    {
        private readonly List<DriveRate> m_TrackingRates = new List<DriveRate>();
        private int _pos = -1;

        /// <summary>
        /// Initialiser
        /// </summary>
        public TrackingRates()
        {
        }

        /// <summary>
        /// Add a new drive rate to the collection
        /// </summary>
        /// <param name="rate">TrackingRate to add to the TrackingRates collection</param>
        public void Add(DriveRate rate)
        {
            m_TrackingRates.Add((DriveRate)rate);
        }

        #region ITrackingRates Members

        /// <summary>
        /// Number of DriveRates supported by the Telescope
        /// </summary>
        /// <value>Number of DriveRates supported by the Telescope</value>
        /// <returns>Integer count</returns>
        public int Count
        {
            get { return m_TrackingRates.Count; }
        }

        /// <summary>
        /// Returns an enumerator for the collection
        /// </summary>
        /// <returns>An enumerator</returns>
        public IEnumerator GetEnumerator()
        {
            _pos = -1; //Reset pointer as this is assumed by .NET enumeration
            return this as IEnumerator;
        }

        /// <summary>
        /// Returns a specified item from the collection
        /// </summary>
        /// <param name="index">Number of the item to return</param>
        /// <value>A collection of supported DriveRate values that describe the permissible values of the TrackingRate property for this telescope type.</value>
        /// <returns>Returns a collection of supported DriveRate values</returns>
        /// <remarks>This is only used by telescope interface versions 2 and 3</remarks>
        public DriveRate this[int index]
        {
            get
            {
                if (index < 1 || index > this.Count)
                    throw new InvalidValueException("TrackingRates.this", index.ToString(CultureInfo.CurrentCulture), string.Format(CultureInfo.CurrentCulture, "1 to {0}", this.Count));
                return m_TrackingRates[index - 1];
            }   // 1-based
        }

        #endregion ITrackingRates Members

        #region IEnumerator implementation

        /// <summary>
        /// Move to the next member in the collection
        /// </summary>
        /// <returns>True if the enumerator was successfully advanced to the next element; False if the enumerator has passed the end of the collection.</returns>
        public bool MoveNext()
        {
            if (++_pos >= m_TrackingRates.Count) return false;
            return true;
        }

        /// <summary>
        /// Reset the enumerator to the start of the collection
        /// </summary>
        public void Reset()
        {
            _pos = -1;
        }

        /// <summary>
        /// Return the current member of the collection
        /// </summary>
        public object Current
        {
            get
            {
                if (_pos < 0 || _pos >= m_TrackingRates.Count) throw new System.InvalidOperationException();
                return m_TrackingRates[_pos];
            }
        }

        #endregion IEnumerator implementation

        #region IDisposable Members

        /// <summary>
        /// Dispose of the TrackingRates object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// method used by the CLR, do not call this method, instead use <see cref="Dispose()"/>.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                /* Following code commented out in Platform 6.4 because m_TrackingRates is a global variable for the whole driver and there could be more than one
                 * instance of the TrackingRates class (created by the calling application). One instance should not invalidate the variable that could be in use
                 * by other instances of which this one is unaware.

                m_TrackingRates = null;

                */
            }
        }

        #endregion IDisposable Members
    }

    /// <summary>
    /// A collection of rates at which the telescope may be moved about the specified axis by the <see cref="ITelescopeV3.MoveAxis" /> method.
    /// This is only used if the telescope interface version is 2 or 3
    /// </summary>
    /// <remarks><para>See the description of the <see cref="ITelescopeV3.MoveAxis" /> method for more information.</para>
    /// <para>This method must return an empty collection if <see cref="ITelescopeV3.MoveAxis" /> is not supported.</para>
    /// <para>The values used in <see cref="IRate" /> members must be non-negative; forward and backward motion is achieved by the application
    /// applying an appropriate sign to the returned <see cref="IRate" /> values in the <see cref="ITelescopeV3.MoveAxis" /> command.</para>
    /// </remarks>
    public class AxisRates : IAxisRates, IEnumerable, IEnumerator, IDisposable
    {
        private List<AxisRate> m_Rates = new List<AxisRate>();
        private int pos = -1;

        /// <summary>
        /// Initialise an AxisRates object
        /// </summary>
        public AxisRates()
        {
        }

        /// <summary>
        /// Add a new member to the AxisRates collection
        /// </summary>
        /// <param name="Minimum">Minimum movement rate.</param>
        /// <param name="Maximum">Maximum movement rate.</param>
        public void Add(double Minimum, double Maximum)
        {
            m_Rates.Add(new AxisRate(Minimum, Maximum));
        }

        #region IAxisRates Members

        /// <summary>
        /// Number of AxisRate items in the returned collection
        /// </summary>
        /// <value>Number of items</value>
        /// <returns>Integer number of items</returns>
        /// <remarks>Number of AxisRate items in the AxisRates collection</remarks>
        public int Count
        {
            get { return m_Rates.Count; }
        }

        /// <summary>
        /// Returns an enumerator for the AxisRates collection.
        /// </summary>
        /// <returns>An enumerator for the AxisRates collection</returns>
        public IEnumerator GetEnumerator()
        {
            pos = -1; //Reset pointer as this is assumed by .NET enumeration
            return this as IEnumerator;
        }

        /// <summary>
        /// Return information about the rates at which the telescope may be moved about the specified axis by the <see cref="ITelescopeV3.MoveAxis" /> method.
        /// </summary>
        /// <param name="index">The axis about which rate information is desired</param>
        /// <value>Collection of Rate objects describing the supported rates of motion that can be supplied to the <see cref="ITelescopeV3.MoveAxis" /> method for the specified axis.</value>
        /// <returns>Collection of Rate objects </returns>
        /// <remarks><para>The (symbolic) values for Index (<see cref="TelescopeAxis" />) are:</para>
        /// <bl>
        /// <li><see cref="TelescopeAxis.Primary"/> 0 Primary axis (e.g., Hour Angle or Azimuth)</li>
        /// <li><see cref="TelescopeAxis.Secondary"/> 1 Secondary axis (e.g., Declination or Altitude)</li>
        /// <li><see cref="TelescopeAxis.Tertiary"/> 2 Tertiary axis (e.g. imager rotator/de-rotator)</li> 
        /// </bl>
        /// </remarks>
        public IRate this[int index]
        {
            get
            {
                if (index < 1 || index > this.Count)
                    throw new InvalidValueException("AxisRates.index", index.ToString(CultureInfo.CurrentCulture), string.Format(CultureInfo.CurrentCulture, "1 to {0}", this.Count));
                return (IRate)m_Rates[index - 1];   // 1-based
            }
        }

        #endregion IAxisRates Members

        #region IDisposable Members

        /// <summary>
        /// DIspose of the AxisRates object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// method used by the CLR, do not call this method, instead use <see cref="Dispose()"/>.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                m_Rates = null;
            }
        }

        #endregion IDisposable Members

        #region IEnumerator implementation

        /// <summary>
        /// Move to the next member of the collection.
        /// </summary>
        /// <returns>True if the enumerator was successfully advanced to the next element; False if the enumerator has passed the end of the collection.</returns>
        public bool MoveNext()
        {
            if (++pos >= m_Rates.Count) return false;
            return true;
        }

        /// <summary>
        /// Reset the enumerator to the first member of the collection
        /// </summary>
        public void Reset()
        {
            pos = -1;
        }

        /// <summary>
        /// Return the current member of the AxisRates collection.
        /// </summary>
        public object Current
        {
            get
            {
                if (pos < 0 || pos >= m_Rates.Count) throw new System.InvalidOperationException();
                return m_Rates[pos];
            }
        }

        #endregion IEnumerator implementation
    }
}