using ASCOM.Common.DeviceInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using ASCOM.Common;
using ASCOM.Common.Interfaces;
using ASCOM.Common.DeviceStateClasses;

namespace ASCOM.Com.DriverAccess
{
    /// <summary>
    /// Telescope device class
    /// </summary>
#if NET8_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    public class Telescope : ASCOMDevice, ITelescopeV4
    {
        #region Convenience members

        /// <summary>
        /// Return a list of all Telescopes registered in the ASCOM Profile
        /// </summary>
        public static List<ASCOMRegistration> Telescopes => Profile.GetDrivers(DeviceTypes.Telescope);

        /// <summary>
		/// State response from the device
		/// </summary>
		public TelescopeState TelescopeState
        {
            get
            {
                // Create a state object to return.
                TelescopeState state = new TelescopeState(DeviceState, TL);
                TL.LogMessage(LogLevel.Debug,nameof(TelescopeState), $"Returning: '{state.Altitude}' '{state.AtHome}' '{state.AtPark}' '{state.Azimuth}' '{state.Declination}' '{state.IsPulseGuiding}' " +
                    $"'{state.RightAscension}' '{state.SideOfPier}' '{state.SiderealTime}' '{state.Slewing}' '{state.Tracking}' '{state.UTCDate}' '{state.TimeStamp}'");

                // Return the device specific state class
                return state;
            }
        }

        #endregion

        #region Initialisers

        /// <summary>
        /// Initialise Telescope device
        /// </summary>
        /// <param name="ProgID">COM ProgID of the device.</param>
        public Telescope(string ProgID) : base(ProgID)
        {
            deviceType = DeviceTypes.Telescope;
        }
        /// <summary>
        /// Initialise Telescope device with a debug logger
        /// </summary>
        /// <param name="ProgID">ProgID of the driver</param>
        /// <param name="logger">Logger instance to receive debug information.</param>
        public Telescope(string ProgID, ILogger logger) : base(ProgID)
        {
            deviceType = DeviceTypes.Telescope;
            TL = logger;
        }

        #endregion

        #region ITelescopeV3

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public double Altitude => Device.Altitude;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public bool AtHome => Device.AtHome;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public double Azimuth => Device.Azimuth;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public bool CanPulseGuide => Device.CanPulseGuide;

        /// <inheritdoc/>
        public bool CanSetDeclinationRate => Device.CanSetDeclinationRate;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public bool CanSetRightAscensionRate => Device.CanSetRightAscensionRate;

        /// <inheritdoc/>
        public bool CanSetTracking => Device.CanSetTracking;

        /// <inheritdoc/>
        public bool CanSlew => Device.CanSlew;

        /// <inheritdoc/>
        public bool CanSlewAltAz => Device.CanSlewAltAz;

        /// <inheritdoc/>
        public bool CanSlewAltAzAsync => Device.CanSlewAltAzAsync;

        /// <inheritdoc/>
        public bool CanSlewAsync => Device.CanSlewAsync;

        /// <inheritdoc/>
        public bool CanSync => Device.CanSync;

        /// <inheritdoc/>
        public bool CanSyncAltAz => Device.CanSyncAltAz;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public double Declination => Device.Declination;

        /// <inheritdoc/>
        public double DeclinationRate { get => Device.DeclinationRate; set => Device.DeclinationRate = value; }

        /// <inheritdoc/>
        public bool DoesRefraction { get => Device.DoesRefraction; set => Device.DoesRefraction = value; }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public bool IsPulseGuiding => Device.IsPulseGuiding;

        /// <inheritdoc/>
        public double RightAscension => Device.RightAscension;

        /// <inheritdoc/>
        public double RightAscensionRate { get => Device.RightAscensionRate; set => Device.RightAscensionRate = value; }

        /// <inheritdoc/>
        public PointingState SideOfPier
        {
            get => (PointingState)Device.SideOfPier;
            set
            {
                Device.SideOfPier = value;
            }
        }

        /// <inheritdoc/>
        public double SiderealTime => Device.SiderealTime;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public bool Slewing => Device.Slewing;

        /// <inheritdoc/>
        public short SlewSettleTime { get => Device.SlewSettleTime; set => Device.SlewSettleTime = value; }

        /// <inheritdoc/>
        public double TargetDeclination { get => Device.TargetDeclination; set => Device.TargetDeclination = value; }

        /// <inheritdoc/>
        public double TargetRightAscension { get => Device.TargetRightAscension; set => Device.TargetRightAscension = value; }

        /// <inheritdoc/>
        public bool Tracking { get => Device.Tracking; set => Device.Tracking = value; }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public DateTime UTCDate { get => Device.UTCDate; set => Device.UTCDate = value; }

        /// <inheritdoc/>
        public void AbortSlew()
        {
            Device.AbortSlew();
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public bool CanMoveAxis(TelescopeAxis Axis)
        {
            return Device.CanMoveAxis(Axis);
        }

        /// <inheritdoc/>
        public PointingState DestinationSideOfPier(double RightAscension, double Declination)
        {
            if (InterfaceVersion == 1)
            {
                throw new ASCOM.NotImplementedException("AtPark is only supported by Interface Versions 2 and above.");
            }
            return (PointingState)Device.DestinationSideOfPier(RightAscension, Declination);
        }

        /// <inheritdoc/>
        public void FindHome()
        {
            if (InterfaceVersion == 1)
            {
                throw new ASCOM.NotImplementedException("FindHome is only supported by Interface Versions 2 and above.");
            }
            Device.FindHome();
        }

        /// <inheritdoc/>
        public void MoveAxis(TelescopeAxis Axis, double Rate)
        {
            Device.MoveAxis(Axis, Rate);
        }

        /// <inheritdoc/>
        public void Park()
        {
            Device.Park();
        }

        /// <inheritdoc/>
        public void PulseGuide(GuideDirection Direction, int Duration)
        {
            Device.PulseGuide(Direction, Duration);
        }

        /// <inheritdoc/>
        public void SetPark()
        {
            Device.SetPark();
        }

        /// <inheritdoc/>
        public void SlewToAltAz(double Azimuth, double Altitude)
        {
            if (InterfaceVersion == 1)
            {
                throw new ASCOM.NotImplementedException("SlewToAltAz is only supported by Interface Versions 2 and above.");
            }
            Device.SlewToAltAz(Azimuth, Altitude);
        }

        /// <inheritdoc/>
        public void SlewToAltAzAsync(double Azimuth, double Altitude)
        {
            if (InterfaceVersion == 1)
            {
                throw new ASCOM.NotImplementedException("SlewToAltAzAsync is only supported by Interface Versions 2 and above.");
            }
            Device.SlewToAltAzAsync(Azimuth, Altitude);
        }

        /// <inheritdoc/>
        public void SlewToCoordinates(double RightAscension, double Declination)
        {
            Device.SlewToCoordinates(RightAscension, Declination);
        }

        /// <inheritdoc/>
        public void SlewToCoordinatesAsync(double RightAscension, double Declination)
        {
            Device.SlewToCoordinatesAsync(RightAscension, Declination);
        }

        /// <inheritdoc/>
        public void SlewToTarget()
        {
            Device.SlewToTarget();
        }

        /// <inheritdoc/>
        public void SlewToTargetAsync()
        {
            Device.SlewToTargetAsync();
        }

        /// <inheritdoc/>
        public void SyncToAltAz(double Azimuth, double Altitude)
        {
            if (InterfaceVersion == 1)
            {
                throw new ASCOM.NotImplementedException("SyncToAltAz is only supported by Interface Versions 2 and above.");
            }
            Device.SyncToAltAz(Azimuth, Altitude);
        }

        /// <inheritdoc/>
        public void SyncToCoordinates(double RightAscension, double Declination)
        {
            Device.SyncToCoordinates(RightAscension, Declination);
        }

        /// <inheritdoc/>
        public void SyncToTarget()
        {
            Device.SyncToTarget();
        }

        /// <inheritdoc/>
        public void Unpark()
        {
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