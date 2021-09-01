using ASCOM.Standard.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ASCOM.Standard.COM.DriverAccess
{
    public class Telescope : ASCOMDevice, ASCOM.Standard.Interfaces.ITelescopeV3
    {
        public static List<ASCOMRegistration> Telescopes => ProfileAccess.GetDrivers(DriverTypes.Telescope);

        public Telescope(string ProgID) : base(ProgID)
        {

        }

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

        public AlignmentMode AlignmentMode
        {
            get
            {
                if(InterfaceVersion == 1)
                {
                    throw new ASCOM.PropertyNotImplementedException("AlignmentMode is only supported by Interface Versions 2 and above.");
                }
                return (AlignmentMode)base.Device.AlignmentMode;
            }
        }

        public double Altitude => base.Device.Altitude;

        public double ApertureArea
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.PropertyNotImplementedException("ApertureArea is only supported by Interface Versions 2 and above.");
                }
                return base.Device.ApertureArea;
            }
        }

        public double ApertureDiameter
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.PropertyNotImplementedException("ApertureDiameter is only supported by Interface Versions 2 and above.");
                }
                return base.Device.ApertureDiameter;
            }
        }

        public bool AtHome => base.Device.AtHome;

        public bool AtPark
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return false;
                }
                return base.Device.AtPark;
            }
        }

        public double Azimuth => base.Device.Azimuth;

        public bool CanFindHome
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return false;
                }
                return base.Device.CanFindHome;
            }
        }

        public bool CanPark
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return false;
                }
                return base.Device.CanPark;
            }
        }

        public bool CanPulseGuide => base.Device.CanPulseGuide;

        public bool CanSetDeclinationRate => base.Device.CanSetDeclinationRate;

        public bool CanSetGuideRates
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return false;
                }
                return base.Device.CanSetGuideRates;
            }
        }

        public bool CanSetPark
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return false;
                }
                return base.Device.CanSetPark;
            }
        }

        public bool CanSetPierSide
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return false;
                }
                return base.Device.CanSetPierSide;
            }
        }

        public bool CanSetRightAscensionRate => base.Device.CanSetRightAscensionRate;

        public bool CanSetTracking => base.Device.CanSetTracking;

        public bool CanSlew => base.Device.CanSlew;

        public bool CanSlewAltAz => base.Device.CanSlewAltAz;

        public bool CanSlewAltAzAsync => base.Device.CanSlewAltAzAsync;

        public bool CanSlewAsync => base.Device.CanSlewAsync;

        public bool CanSync => base.Device.CanSync;

        public bool CanSyncAltAz => base.Device.CanSyncAltAz;

        public bool CanUnpark
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return false;
                }
                return base.Device.CanUnpark;
            }
        }

        public double Declination => base.Device.Declination;

        public double DeclinationRate { get => base.Device.DeclinationRate; set => base.Device.DeclinationRate = value; }

        public bool DoesRefraction { get => base.Device.DoesRefraction; set => base.Device.DoesRefraction = value; }

        public EquatorialCoordinateType EquatorialSystem
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.PropertyNotImplementedException("EquatorialSystem is only supported by Interface Versions 2 and above.");
                }
                return (EquatorialCoordinateType)base.Device.EquatorialSystem;
            }
        }

        public double FocalLength 
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.PropertyNotImplementedException("FocalLength is only supported by Interface Versions 2 and above.");
                }
                return base.Device.FocalLength;
            }
        }

        public double GuideRateDeclination
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.PropertyNotImplementedException("GuideRateDeclination is only supported by Interface Versions 2 and above.");
                }
                return base.Device.GuideRateDeclination;
            }
            set
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.PropertyNotImplementedException("GuideRateDeclination is only supported by Interface Versions 2 and above.");
                }
                base.Device.GuideRateDeclination = value;
            }
        }
        public double GuideRateRightAscension
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.PropertyNotImplementedException("GuideRateRightAscension is only supported by Interface Versions 2 and above.");
                }
                return base.Device.GuideRateRightAscension;
            }
            set
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.PropertyNotImplementedException("GuideRateRightAscension is only supported by Interface Versions 2 and above.");
                }
                base.Device.GuideRateRightAscension = value;
            }
        }

        public bool IsPulseGuiding => base.Device.IsPulseGuiding;

        public double RightAscension => base.Device.RightAscension;

        public double RightAscensionRate { get => base.Device.RightAscensionRate; set => base.Device.RightAscensionRate = value; }
        public PointingState SideOfPier { get => (PointingState)base.Device.SideOfPier; set => base.Device.SideOfPier = value; }

        public double SiderealTime => base.Device.SiderealTime;

        public double SiteElevation
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.PropertyNotImplementedException("SiteElevation is only supported by Interface Versions 2 and above.");
                }
                return base.Device.SiteElevation;
            }
            set
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.PropertyNotImplementedException("SiteElevation is only supported by Interface Versions 2 and above.");
                }
                base.Device.SiteElevation = value;
            }
        }

        public double SiteLatitude
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.PropertyNotImplementedException("SiteLatitude is only supported by Interface Versions 2 and above.");
                }
                return base.Device.SiteLatitude;
            }
            set
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.PropertyNotImplementedException("SiteLatitude is only supported by Interface Versions 2 and above.");
                }
                base.Device.SiteLatitude = value;
            }
        }
        public double SiteLongitude
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.PropertyNotImplementedException("SiteLongitude is only supported by Interface Versions 2 and above.");
                }
                return base.Device.SiteLongitude;
            }
            set
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.PropertyNotImplementedException("SiteLongitude is only supported by Interface Versions 2 and above.");
                }
                base.Device.SiteLongitude = value;
            }
        }

        public bool Slewing => base.Device.Slewing;

        public short SlewSettleTime { get => base.Device.SlewSettleTime; set => base.Device.SlewSettleTime = value; }
        public double TargetDeclination { get => base.Device.TargetDeclination; set => base.Device.TargetDeclination = value; }
        public double TargetRightAscension { get => base.Device.TargetRightAscension; set => base.Device.TargetRightAscension = value; }
        public bool Tracking { get => base.Device.Tracking; set => base.Device.Tracking = value; }
        public DriveRate TrackingRate
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.PropertyNotImplementedException("TrackingRate is only supported by Interface Versions 2 and above.");
                }
                return (DriveRate)base.Device.TrackingRate;
            }
            set
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.PropertyNotImplementedException("TrackingRate is only supported by Interface Versions 2 and above.");
                }
                base.Device.TrackingRate = value;
            }
        }

        public ITrackingRates TrackingRates 
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new ASCOM.PropertyNotImplementedException("TrackingRates is only supported by Interface Versions 2 and above.");
                }

                TrackingRates rates = new TrackingRates();

                foreach (var rate in (base.Device.TrackingRates))
                {
                    rates.Add((DriveRate)rate);
                }
                return rates;
            }
        }

        public DateTime UTCDate { get => base.Device.UTCDate; set => base.Device.UTCDate = value; }

        public void AbortSlew()
        {
            base.Device.AbortSlew();
        }

        public IAxisRates AxisRates(TelescopeAxis Axis)
        {
            if (InterfaceVersion == 1)
            {
                throw new ASCOM.MethodNotImplementedException("AxisRates is only supported by Interface Versions 2 and above.");
            }
            AxisRates rates = new AxisRates();

            foreach (var rate in base.Device.AxisRates(Axis))
            {
                rates.Add(rate.Minimum, rate.Maximum);
            }

            return rates;
        }

        public bool CanMoveAxis(TelescopeAxis Axis)
        {
            return base.Device.CanMoveAxis(Axis);
        }

        public PointingState DestinationSideOfPier(double RightAscension, double Declination)
        {
            if (InterfaceVersion == 1)
            {
                throw new ASCOM.MethodNotImplementedException("AtPark is only supported by Interface Versions 2 and above.");
            }
            return (PointingState) base.Device.DestinationSideOfPier(RightAscension, Declination);
        }

        public void FindHome()
        {
            if (InterfaceVersion == 1)
            {
                throw new ASCOM.MethodNotImplementedException("FindHome is only supported by Interface Versions 2 and above.");
            }
            base.Device.FindHome();
        }

        public void MoveAxis(TelescopeAxis Axis, double Rate)
        {
            base.Device.MoveAxis(Axis, Rate);
        }

        public void Park()
        {
            base.Device.Park();
        }

        public void PulseGuide(GuideDirection Direction, int Duration)
        {
            base.Device.PulseGuide(Direction, Duration);
        }

        public void SetPark()
        {
            base.Device.SetPark();
        }

        public void SlewToAltAz(double Azimuth, double Altitude)
        {
            if (InterfaceVersion == 1)
            {
                throw new ASCOM.MethodNotImplementedException("SlewToAltAz is only supported by Interface Versions 2 and above.");
            }
            base.Device.SlewToAltAz(Azimuth, Altitude);
        }

        public void SlewToAltAzAsync(double Azimuth, double Altitude)
        {
            if (InterfaceVersion == 1)
            {
                throw new ASCOM.MethodNotImplementedException("SlewToAltAzAsync is only supported by Interface Versions 2 and above.");
            }
            base.Device.SlewToAltAzAsync(Azimuth, Altitude);
        }

        public void SlewToCoordinates(double RightAscension, double Declination)
        {
            base.Device.SlewToCoordinates(RightAscension, Declination);
        }

        public void SlewToCoordinatesAsync(double RightAscension, double Declination)
        {
            base.Device.SlewToCoordinatesAsync(RightAscension, Declination);
        }

        public void SlewToTarget()
        {
            base.Device.SlewToTarget();
        }

        public void SlewToTargetAsync()
        {
            base.Device.SlewToTargetAsync();
        }

        public void SyncToAltAz(double Azimuth, double Altitude)
        {
            if (InterfaceVersion == 1)
            {
                throw new ASCOM.MethodNotImplementedException("SyncToAltAz is only supported by Interface Versions 2 and above.");
            }
            base.Device.SyncToAltAz(Azimuth, Altitude);
        }

        public void SyncToCoordinates(double RightAscension, double Declination)
        {
            base.Device.SyncToCoordinates(RightAscension, Declination);
        }

        public void SyncToTarget()
        {
            base.Device.SyncToTarget();
        }

        public void UnPark()
        {
            base.Device.Unpark();
        }
    }

    public class TrackingRates : ITrackingRates, IEnumerable, IEnumerator, IDisposable
    {
        private List<DriveRate> m_TrackingRates = new List<DriveRate>();
        private int _pos = -1;
        //
        // Default constructor - Internal prevents public creation
        // of instances. Returned by Telescope.AxisRates.
        //
        public TrackingRates()
        {
        }

        public void Add(DriveRate rate)
        {
            m_TrackingRates.Add((DriveRate)rate);
        }

        #region ITrackingRates Members

        public int Count
        {
            get { return m_TrackingRates.Count; }
        }

        public IEnumerator GetEnumerator()
        {
            _pos = -1; //Reset pointer as this is assumed by .NET enumeration
            return this as IEnumerator;
        }


        public DriveRate this[int index]
        {
            get
            {
                if (index < 1 || index > this.Count)
                    throw new InvalidValueException("TrackingRates.this", index.ToString(CultureInfo.CurrentCulture), string.Format(CultureInfo.CurrentCulture, "1 to {0}", this.Count));
                return m_TrackingRates[index - 1];
            }   // 1-based
        }
        #endregion

        #region IEnumerator implementation

        public bool MoveNext()
        {
            if (++_pos >= m_TrackingRates.Count) return false;
            return true;
        }

        public void Reset()
        {
            _pos = -1;
        }

        public object Current
        {
            get
            {
                if (_pos < 0 || _pos >= m_TrackingRates.Count) throw new System.InvalidOperationException();
                return m_TrackingRates[_pos];
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
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
        #endregion
    }

    public class AxisRates : IAxisRates, IEnumerable, IEnumerator, IDisposable
    {
        private List<AxisRate> m_Rates = new List<AxisRate>();
        private int pos = -1;

        //
        // Constructor - Internal prevents public creation
        // of instances. Returned by Telescope.AxisRates.
        //
        public AxisRates()
        {
        }

        public void Add(double Minimum, double Maximum)
        {
            m_Rates.Add(new AxisRate(Minimum, Maximum));
        }

        #region IAxisRates Members

        public int Count
        {
            get { return m_Rates.Count; }
        }

        public IEnumerator GetEnumerator()
        {
            pos = -1; //Reset pointer as this is assumed by .NET enumeration
            return this as IEnumerator;
        }

        public IRate this[int index]
        {
            get
            {
                if (index < 1 || index > this.Count)
                    throw new InvalidValueException("AxisRates.index", index.ToString(CultureInfo.CurrentCulture), string.Format(CultureInfo.CurrentCulture, "1 to {0}", this.Count));
                return (IRate)m_Rates[index - 1];   // 1-based
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                m_Rates = null;
            }
        }

        #endregion

        #region IEnumerator implementation

        public bool MoveNext()
        {
            if (++pos >= m_Rates.Count) return false;
            return true;
        }

        public void Reset()
        {
            pos = -1;
        }

        public object Current
        {
            get
            {
                if (pos < 0 || pos >= m_Rates.Count) throw new System.InvalidOperationException();
                return m_Rates[pos];
            }
        }

        #endregion
    }
}
