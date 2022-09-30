using ASCOM.Common.Interfaces;
using System;
using System.Diagnostics;
using System.Reflection;

namespace ASCOM.Tools
{

    /// <summary>
    /// Coordinate transform component; J2000 - apparent - topocentric
    /// </summary>
    /// <remarks>Use this component to transform between J2000, apparent and topocentric (JNow) coordinates or 
    /// vice versa. To use the component, instantiate it, then use one of SetJ2000 or SetJNow or SetApparent to 
    /// initialise with known values. Now use the RAJ2000, DECJ200, RAJNow, DECJNow, RAApparent and DECApparent etc. 
    /// properties to read off the required transformed values.
    ///<para>The component can be reused simply by setting new co-ordinates with a Set command, there
    /// is no need to create a new component each time a transform is required.</para>
    /// <para>Transforms are effected through the ASCOM SOFA component that encapsulates the IAU SOFA library. 
    /// The IA SOFA reference web page is: 
    /// <href>https://www.iausofa.org/</href>, which includes links to the SOFA manual and handbook.
    /// </para>
    /// </remarks>
    public class Transform : IDisposable
    {
        private bool disposedValue;        // To detect redundant calls
        private double raJ2000Value, raTopoValue, decJ2000Value, decTopoValue, siteElevValue, siteLatValue, siteLongValue, siteTempValue, sitePressureValue;
        private double raApparentValue, decApparentValue, azimuthTopoValue, elevationTopoValue, julianDateTTValue, julianDateUTCValue, deltaUT1;
        private bool refracValue, requiresRecalculate;
        private SetBy lastSetBy;

        private readonly bool loggerIsTraceLogger = false;

        private TraceLogger traceLogger;
        private readonly ILogger iLogger;
        private Stopwatch sw, swRecalculate;

        private const double HOURS2RADIANS = Math.PI / 12.0;
        private const double DEGREES2RADIANS = Math.PI / 180.0;
        private const double RADIANS2HOURS = 12.0 / Math.PI;
        private const double RADIANS2DEGREES = 180.0 / Math.PI;

        private const string DATE_FORMAT = "dd/MM/yyyy HH:mm:ss.fff";

        private const double STANDARD_PRESSURE = 1013.25; // Standard atmospheric pressure (hPa)
        private const double ABSOLUTE_ZERO_CELSIUS = -273.15; // Absolute zero expressed in Celsius

        // Constants defining the minimum and maximum supported values for Julian dates: 1/1/100 to 31/12/9999
        private const double OLE_AUTOMATION_JULIAN_DATE_OFFSET = 2415018.5; // Offset of OLE automation dates from Julian dates
        private const double JULIAN_DATE_MINIMUM_VALUE = -657435.0 + OLE_AUTOMATION_JULIAN_DATE_OFFSET; // Minimum valid Julian date value (1/1/0100 00:00:00) - because DateTime.FromOADate has this limit
        private const double JULIAN_DATE_MAXIMUM_VALUE = 2958465.99999999 + OLE_AUTOMATION_JULIAN_DATE_OFFSET; // Maximum valid Julian date value (31/12/9999 23:59:59.999) - because DateTime.FromOADate has this limit

        private enum SetBy
        {
            Never,
            J2000,
            Apparent,
            Topocentric,
            AzimuthElevation,
            Refresh
        }

        #region New and Dispose

        /// <summary>
        /// Create a Transform component without a logger
        /// </summary>
        public Transform() : this(null)
        {
        }

        /// <summary>
        /// Create a Transform component with an ILogger logger
        /// </summary>
        /// <param name="logger">Optional ILogger instance that can be used to record operational messages from the Transform component</param>
        public Transform(ILogger logger)
        {
            // Populate an instance variable that we can use later as an ILogger object if the logger is not a TraceLogger
            iLogger = logger;

            // Determine what type of logger object was supplied, if any, so we can use either the TraceLogger.LogMessage format or the ILogger.Log format.
            if (!(logger is null)) // Only test for a TraceLogger instance if a logger object was supplied
            {
                // Test whether the supplied object is a TraceLogger
                if (logger.GetType() == typeof(TraceLogger)) // Supplied object is a TraceLogger
                {
                    traceLogger = logger as TraceLogger; // Populate an instance variable that we can use later as a TraceLogger
                    loggerIsTraceLogger = true; // Record that we have a TraceLogger
                }
            }
            LogMessage("New", "Logger processed OK");

            sw = new Stopwatch();
            swRecalculate = new Stopwatch();

            // Initialise to invalid values in case these are read before they are set
            raJ2000Value = double.NaN;
            decJ2000Value = double.NaN;
            raTopoValue = double.NaN;
            decTopoValue = double.NaN;
            siteElevValue = double.NaN;
            siteLatValue = double.NaN;
            siteLongValue = double.NaN;
            sitePressureValue = double.NaN;

            refracValue = false;
            lastSetBy = SetBy.Never;
            requiresRecalculate = true;
            julianDateTTValue = 0; // Initialise to a value that forces the current PC date time to be used in determining the TT Julian date of interest

            string strPath;
            strPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            LogMessage("New", "Assembly path: " + strPath);

            LogMessage("New", "Transform initialised OK");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                //if (!Utl == null)
                //{
                //    Utl.Dispose();
                //    Utl = null;
                //}
                //if (!AstroUtl == null)
                //{
                //    AstroUtl.Dispose();
                //    AstroUtl = null;
                //}
                if (!(sw == null))
                {
                    sw.Stop();
                    sw = null;
                }
                if (!(swRecalculate == null))
                {
                    swRecalculate.Stop();
                    swRecalculate = null;
                }
            }
            this.disposedValue = true;
        }

        /// <summary>
        /// Cleans up resources used by the Transform component
        /// </summary>
        /// <remarks></remarks>
        public void Dispose()
        {
            // Do not change this code.  Put clean-up code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Public members
        /// <summary>
        /// Set the delta UT1 value to be used by Transform, defaults to 0.0
        /// </summary>
        public double DeltaUT1
        {
            get
            {
                return deltaUT1;
            }
            set
            {
                deltaUT1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the site latitude
        /// </summary>
        /// <value>Site latitude (-90.0 to +90.0)</value>
        /// <returns>Latitude in degrees</returns>
        /// <remarks>Positive numbers north of the equator, negative numbers south.</remarks>
        public double SiteLatitude
        {
            get
            {
                CheckSet("SiteLatitude", siteLatValue, "Site latitude has not been set");
                LogMessage("SiteLatitude Get", FormatDec(siteLatValue));
                return siteLatValue;
            }
            set
            {
                if ((value < -90.0) | (value > 90.0))
                    throw new ASCOM.InvalidValueException("SiteLatitude", value.ToString(), "-90.0 degrees", "+90.0 degrees");
                if (siteLatValue != value)
                    requiresRecalculate = true;
                siteLatValue = value;
                LogMessage("SiteLatitude Set", FormatDec(value));
            }
        }

        /// <summary>
        /// Gets or sets the site longitude
        /// </summary>
        /// <value>Site longitude (-180.0 to +180.0)</value>
        /// <returns>Longitude in degrees</returns>
        /// <remarks>Positive numbers east of the Greenwich meridian, negative numbers west of the Greenwich meridian.</remarks>
        public double SiteLongitude
        {
            get
            {
                CheckSet("SiteLongitude", siteLongValue, "Site longitude has not been set");
                LogMessage("SiteLongitude Get", FormatDec(siteLongValue));
                return siteLongValue;
            }
            set
            {
                if ((value < -180.0) | (value > 180.0))
                    throw new ASCOM.InvalidValueException("SiteLongitude", value.ToString(), "-180.0 degrees", "+180.0 degrees");
                if (siteLongValue != value)
                    requiresRecalculate = true;
                siteLongValue = value;
                LogMessage("SiteLongitude Set", FormatDec(value));
            }
        }

        /// <summary>
        /// Gets or sets the site elevation above sea level
        /// </summary>
        /// <value>Site elevation (-300.0 to +10,000.0 metres)</value>
        /// <returns>Elevation in metres</returns>
        /// <remarks></remarks>
        public double SiteElevation
        {
            get
            {
                CheckSet("SiteElevation", siteElevValue, "Site elevation has not been set");
                LogMessage("SiteElevation Get", siteElevValue.ToString());
                return siteElevValue;
            }
            set
            {
                if ((value < -300.0) | (value > 10000.0))
                    throw new ASCOM.InvalidValueException("SiteElevation", value.ToString(), "-300.0 metres", "+10000.0 metres");
                if (siteElevValue != value)
                    requiresRecalculate = true;
                siteElevValue = value;
                LogMessage("SiteElevation Set", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the site ambient temperature (not reduced to sea level)
        /// </summary>
        /// <value>Site ambient temperature (-273.15 to 100.0 Celsius)</value>
        /// <returns>Temperature in degrees Celsius</returns>
        /// <remarks>This property represents the air temperature as measured by a thermometer at the observing site. It must not be a "reduced to sea level" value.</remarks>
        public double SiteTemperature
        {
            get
            {
                CheckSet("SiteTemperature", siteTempValue, "Site temperature has not been set");
                LogMessage("SiteTemperature Get", siteTempValue.ToString());
                return siteTempValue;
            }
            set
            {
                if ((value < -273.15) | (value > 100.0))
                    throw new ASCOM.InvalidValueException("SiteTemperature", value.ToString(), "-273.15 Celsius", "+100.0 Celsius");
                if (siteTempValue != value)
                    requiresRecalculate = true;
                siteTempValue = value;
                LogMessage("SiteTemperature Set", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the site atmospheric pressure (not reduced to sea level)
        /// </summary>
        /// <value>Site atmospheric pressure (0.0 to 1200.0 hPa (mbar))</value>
        /// <returns>Atmospheric pressure (hPa)</returns>
        /// <remarks>This property represents the atmospheric pressure as measured by a barometer at the observing site. It must not be a "reduced to sea level" value.</remarks>
        public double SitePressure
        {
            get
            {
                CheckSet("SitePressure", sitePressureValue, "Site atmospheric pressure has not been set");
                LogMessage("SitePressure Get", sitePressureValue.ToString());
                return sitePressureValue;
            }
            set
            {
                if ((value < 0.0) | (value > 1200.0))
                    throw new ASCOM.InvalidValueException("SitePressure", value.ToString(), "0.0hPa (mbar)", "+1200.0hPa (mbar)");
                if (sitePressureValue != value)
                    requiresRecalculate = true;
                sitePressureValue = value;
                LogMessage("SitePressure Set", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether refraction is calculated for topocentric co-ordinates
        /// </summary>
        /// <value>True / false flag indicating refraction is included / omitted from topocentric co-ordinates</value>
        /// <returns>Boolean flag</returns>
        /// <remarks></remarks>
        public bool Refraction
        {
            get
            {
                LogMessage("Refraction Get", refracValue.ToString());
                return refracValue;
            }
            set
            {
                if (refracValue != value)
                    requiresRecalculate = true;
                refracValue = value;
                LogMessage("Refraction Set", value.ToString());
            }
        }

        /// <summary>
        /// Causes the transform component to recalculate values derived from the last Set command
        /// </summary>
        /// <remarks>Use this when you have set J2000 co-ordinates and wish to ensure that the mount points to the same 
        /// co-ordinates allowing for local effects that change with time such as refraction.
        /// <para><b style="color:red">Note:</b> As of Platform 6 SP2 use of this method is not required, refresh is always performed automatically when required.</para></remarks>
        public void Refresh()
        {
            LogMessage("Refresh", "");
            Recalculate();
        }

        /// <summary>
        /// Sets the known J2000 Right Ascension and Declination coordinates that are to be transformed
        /// </summary>
        /// <param name="ra">RA in J2000 co-ordinates (0.0 to 23.999 hours)</param>
        /// <param name="dec">DEC in J2000 co-ordinates (-90.0 to +90.0)</param>
        /// <remarks></remarks>
        public void SetJ2000(double ra, double dec)
        {
            if ((ra != raJ2000Value) | (dec != decJ2000Value))
            {
                raJ2000Value = ValidateRA("SetJ2000", ra);
                decJ2000Value = ValidateDec("SetJ2000", dec);
                requiresRecalculate = true;
            }

            lastSetBy = SetBy.J2000;
            LogMessage("SetJ2000", "RA: " + FormatRA(ra) + ", DEC: " + FormatDec(dec));
        }

        /// <summary>
        /// Sets the known apparent Right Ascension and Declination coordinates that are to be transformed
        /// </summary>
        /// <param name="ra">RA in apparent co-ordinates (0.0 to 23.999 hours)</param>
        /// <param name="dec">DEC in apparent co-ordinates (-90.0 to +90.0)</param>
        /// <remarks></remarks>
        public void SetApparent(double ra, double dec)
        {
            if ((ra != raApparentValue) | (dec != decApparentValue))
            {
                raApparentValue = ValidateRA("SetApparent", ra);
                decApparentValue = ValidateDec("SetApparent", dec);
                requiresRecalculate = true;
            }

            lastSetBy = SetBy.Apparent;
            LogMessage("SetApparent", "RA: " + FormatRA(ra) + ", DEC: " + FormatDec(dec));
        }

        /// <summary>
        /// Sets the known topocentric Right Ascension and Declination coordinates that are to be transformed
        /// </summary>
        /// <param name="ra">RA in topocentric co-ordinates (0.0 to 23.999 hours)</param>
        /// <param name="dec">DEC in topocentric co-ordinates (-90.0 to +90.0)</param>
        /// <remarks></remarks>
        public void SetTopocentric(double ra, double dec)
        {
            if ((ra != raTopoValue) | (dec != decTopoValue))
            {
                raTopoValue = ValidateRA("SetTopocentric", ra);
                decTopoValue = ValidateDec("SetTopocentric", dec);
                requiresRecalculate = true;
            }

            lastSetBy = SetBy.Topocentric;
            LogMessage("SetTopocentric", "RA: " + FormatRA(ra) + ", DEC: " + FormatDec(dec));
        }

        /// <summary>
        /// Sets the topocentric azimuth and elevation
        /// </summary>
        /// <param name="azimuth">Topocentric Azimuth in degrees (0.0 to 359.999999 - north zero, east 90 deg etc.)</param>
        /// <param name="elevation">Topocentric elevation in degrees (-90.0 to +90.0)</param>
        /// <remarks></remarks>
        public void SetAzimuthElevation(double azimuth, double elevation)
        {
            if ((azimuth < 0.0) | (azimuth >= 360.0))
                throw new ASCOM.InvalidValueException("SetAzimuthElevation Azimuth", azimuth.ToString(), "0.0 hours", "23.9999999... hours");
            if ((elevation < -90.0) | (elevation > 90.0))
                throw new ASCOM.InvalidValueException("SetAzimuthElevation Elevation", elevation.ToString(), "-90.0 degrees", "+90.0 degrees");

            azimuthTopoValue = azimuth;
            elevationTopoValue = elevation;
            requiresRecalculate = true;

            lastSetBy = SetBy.AzimuthElevation;
            LogMessage("SetAzimuthElevation", "Azimuth: " + FormatDec(azimuth) + ", Elevation: " + FormatDec(elevation));
        }

        /// <summary>
        /// Returns the Right Ascension in J2000 co-ordinates
        /// </summary>
        /// <value>J2000 Right Ascension</value>
        /// <returns>Right Ascension in hours</returns>
        /// <exception cref="TransformUninitialisedException">Exception thrown if an attempt is made
        /// to read a value before any of the Set methods has been used or if the value can not be derived from the
        /// information in the last Set method used. E.g. topocentric values will be unavailable if the last Set was
        /// a SetApparent and one of the Site properties has not been set.</exception>
        /// <remarks></remarks>
        ///
        public double RAJ2000
        {
            get
            {
                if (lastSetBy == SetBy.Never)
                    throw new TransformUninitialisedException("Attempt to read RAJ2000 before a SetXX method has been called");
                Recalculate();
                CheckSet("RAJ2000", raJ2000Value, "RA J2000 can not be derived from the information provided. Are site parameters set?");
                LogMessage("RAJ2000 Get", FormatRA(raJ2000Value));
                return raJ2000Value;
            }
        }

        /// <summary>
        /// Returns the Declination in J2000 co-ordinates
        /// </summary>
        /// <value>J2000 Declination</value>
        /// <returns>Declination in degrees</returns>
        /// <exception cref="TransformUninitialisedException">Exception thrown if an attempt is made
        /// to read a value before any of the Set methods has been used or if the value can not be derived from the
        /// information in the last Set method used. E.g. topocentric values will be unavailable if the last Set was
        /// a SetApparent and one of the Site properties has not been set.</exception>
        /// <remarks></remarks>
        public double DecJ2000
        {
            get
            {
                if (lastSetBy == SetBy.Never)
                    throw new TransformUninitialisedException("Attempt to read DECJ2000 before a SetXX method has been called");
                Recalculate();
                CheckSet("DecJ2000", decJ2000Value, "DEC J2000 can not be derived from the information provided. Are site parameters set?");
                LogMessage("DecJ2000 Get", FormatDec(decJ2000Value));
                return decJ2000Value;
            }
        }

        /// <summary>
        /// Returns the Right Ascension in topocentric co-ordinates
        /// </summary>
        /// <value>Topocentric Right Ascension</value>
        /// <returns>Topocentric Right Ascension in hours</returns>
        /// <exception cref="TransformUninitialisedException">Exception thrown if an attempt is made
        /// to read a value before any of the Set methods has been used or if the value can not be derived from the
        /// information in the last Set method used. E.g. topocentric values will be unavailable if the last Set was
        /// a SetApparent and one of the Site properties has not been set.</exception>
        /// <remarks></remarks>
        public double RATopocentric
        {
            get
            {
                if (lastSetBy == SetBy.Never)
                    throw new TransformUninitialisedException("Attempt to read RATopocentric before a SetXX method  has been called");
                Recalculate();
                CheckSet("RATopocentric", raTopoValue, "RA topocentric can not be derived from the information provided. Are site parameters set?");
                LogMessage("RATopocentric Get", FormatRA(raTopoValue));
                return raTopoValue;
            }
        }

        /// <summary>
        /// Returns the Declination in topocentric co-ordinates
        /// </summary>
        /// <value>Topocentric Declination</value>
        /// <returns>Declination in degrees</returns>
        /// <exception cref="TransformUninitialisedException">Exception thrown if an attempt is made
        /// to read a value before any of the Set methods has been used or if the value can not be derived from the
        /// information in the last Set method used. E.g. topocentric values will be unavailable if the last Set was
        /// a SetApparent and one of the Site properties has not been set.</exception>
        /// <remarks></remarks>
        public double DECTopocentric
        {
            get
            {
                if (lastSetBy == SetBy.Never)
                    throw new TransformUninitialisedException("Attempt to read DECTopocentric before a SetXX method has been called");
                Recalculate();
                CheckSet("DECTopocentric", decTopoValue, "DEC topocentric can not be derived from the information provided. Are site parameters set?");
                LogMessage("DECTopocentric Get", FormatDec(decTopoValue));
                return decTopoValue;
            }
        }

        /// <summary>
        /// Returns the Right Ascension in apparent co-ordinates
        /// </summary>
        /// <value>Apparent Right Ascension</value>
        /// <returns>Right Ascension in hours</returns>
        /// <exception cref="TransformUninitialisedException">Exception thrown if an attempt is made
        /// to read a value before any of the Set methods has been used or if the value can not be derived from the
        /// information in the last Set method used. E.g. topocentric values will be unavailable if the last Set was
        /// a SetApparent and one of the Site properties has not been set.</exception>
        /// <remarks></remarks>
        public double RAApparent
        {
            get
            {
                if (lastSetBy == SetBy.Never)
                    throw new TransformUninitialisedException("Attempt to read DECApparent before a SetXX method has been called");
                Recalculate();
                LogMessage("RAApparent Get", FormatRA(raApparentValue));
                return raApparentValue;
            }
        }

        /// <summary>
        /// Returns the Declination in apparent co-ordinates
        /// </summary>
        /// <value>Apparent Declination</value>
        /// <returns>Declination in degrees</returns>
        /// <exception cref="TransformUninitialisedException">Exception thrown if an attempt is made
        /// to read a value before any of the Set methods has been used or if the value can not be derived from the
        /// information in the last Set method used. E.g. topocentric values will be unavailable if the last Set was
        /// a SetApparent and one of the Site properties has not been set.</exception>
        /// <remarks></remarks>
        public double DECApparent
        {
            get
            {
                if (lastSetBy == SetBy.Never)
                    throw new TransformUninitialisedException("Attempt to read DECApparent before a SetXX method has been called");
                Recalculate();
                LogMessage("DECApparent Get", FormatDec(decApparentValue));
                return decApparentValue;
            }
        }

        /// <summary>
        /// Returns the topocentric azimuth angle of the target
        /// </summary>
        /// <value>Topocentric azimuth angle</value>
        /// <returns>Azimuth angle in degrees</returns>
        /// <exception cref="TransformUninitialisedException">Exception thrown if an attempt is made
        /// to read a value before any of the Set methods has been used or if the value can not be derived from the
        /// information in the last Set method used. E.g. topocentric values will be unavailable if the last Set was
        /// a SetApparent and one of the Site properties has not been set.</exception>
        /// <remarks></remarks>
        public double AzimuthTopocentric
        {
            get
            {
                if (lastSetBy == SetBy.Never)
                    throw new TransformUninitialisedException("Attempt to read AzimuthTopocentric before a SetXX method has been called");
                requiresRecalculate = true; // Force a recalculation of Azimuth
                Recalculate();
                CheckSet("AzimuthTopocentric", azimuthTopoValue, "Azimuth topocentric can not be derived from the information provided. Are site parameters set?");
                LogMessage("AzimuthTopocentric Get", FormatDec(azimuthTopoValue));
                return azimuthTopoValue;
            }
        }

        /// <summary>
        /// Returns the topocentric elevation of the target
        /// </summary>
        /// <value>Topocentric elevation angle</value>
        /// <returns>Elevation angle in degrees</returns>
        /// <exception cref="TransformUninitialisedException">Exception thrown if an attempt is made
        /// to read a value before any of the Set methods has been used or if the value can not be derived from the
        /// information in the last Set method used. E.g. topocentric values will be unavailable if the last Set was
        /// a SetApparent and one of the Site properties has not been set.</exception>
        /// <remarks></remarks>
        public double ElevationTopocentric
        {
            get
            {
                if (lastSetBy == SetBy.Never)
                    throw new TransformUninitialisedException("Attempt to read ElevationTopocentric before a SetXX method has been called");
                requiresRecalculate = true; // Force a recalculation of Elevation
                Recalculate();
                CheckSet("ElevationTopocentric", elevationTopoValue, "Elevation topocentric can not be derived from the information provided. Are site parameters set?");
                LogMessage("ElevationTopocentric Get", FormatDec(elevationTopoValue));
                return elevationTopoValue;
            }
        }

        /// <summary>
        /// Sets or returns the Julian date on the Terrestrial Time timescale for which the transform will be made
        /// </summary>
        /// <value>Julian date (Terrestrial Time) of the transform (1757583.5 to 5373484.499999 = 00:00:00 1/1/0100 to 23:59:59.999 31/12/9999)</value>
        /// <returns>Terrestrial Time Julian date that will be used by Transform or zero if the PC's current clock value will be used to calculate the Julian date.</returns>
        /// <remarks>This method was introduced in May 2012. Previously, Transform used the current date-time of the PC when calculating transforms; 
        /// this remains the default behaviour for backward compatibility.
        /// The initial value of this parameter is 0.0, which is a special value that forces Transform to replicate original behaviour by determining the  
        /// Julian date from the PC's current date and time. If this property is non zero, that particular terrestrial time Julian date is used in preference 
        /// to the value derived from the PC's clock.
        /// <para>Only one of JulianDateTT or JulianDateUTC needs to be set. Use whichever is more readily available, there is no
        /// need to set both values. Transform will use the last set value of either JulianDateTT or JulianDateUTC as the basis for its calculations.</para></remarks>
        public double JulianDateTT
        {
            get
            {
                return julianDateTTValue;
            }
            set
            {
                double tai1 = 0.0, tai2 = 0.0, utc1 = 0.0, utc2 = 0.0;

                // Validate the supplied value, it must be 0.0 or within the permitted range
                if ((value != 0.0) & ((value < JULIAN_DATE_MINIMUM_VALUE) | (value > JULIAN_DATE_MAXIMUM_VALUE)))
                    throw new InvalidValueException("JulianDateTT", value.ToString(), JULIAN_DATE_MINIMUM_VALUE.ToString(), JULIAN_DATE_MAXIMUM_VALUE.ToString());

                julianDateTTValue = value;
                requiresRecalculate = true; // Force a recalculation because the Julian date has changed

                if (julianDateTTValue != 0.0)
                {
                    // Calculate UTC
                    if ((Sofa.Tttai(julianDateTTValue, 0.0, ref tai1, ref tai2) != 0))
                        LogMessage("JulianDateTT Set", "TtTai - Bad return code");
                    if ((Sofa.Taiutc(tai1, tai2, ref utc1, ref utc2) != 0))
                        LogMessage("JulianDateTT Set", "TaiUtc - Bad return code");
                    julianDateUTCValue = utc1 + utc2;

                    LogMessage("JulianDateTT Set", julianDateTTValue.ToString() + " " + Utilities.JulianDateToDateTime(julianDateTTValue).ToString(DATE_FORMAT) + ", JDUTC: " + Utilities.JulianDateToDateTime(julianDateUTCValue).ToString(DATE_FORMAT));
                }
                else
                {
                    julianDateUTCValue = 0.0;
                    LogMessage("JulianDateTT Set", "Calculations will now be based on PC the DateTime");
                }
            }
        }

        /// <summary>
        /// Sets or returns the Julian date on the UTC timescale for which the transform will be made
        /// </summary>
        /// <value>Julian date (UTC) of the transform (1757583.5 to 5373484.499999 = 00:00:00 1/1/0100 to 23:59:59.999 31/12/9999)</value>
        /// <returns>UTC Julian date that will be used by Transform or zero if the PC's current clock value will be used to calculate the Julian date.</returns>
        /// <remarks>Introduced in April 2014 as an alternative to JulianDateTT. Only one of JulianDateTT or JulianDateUTC needs to be set. Use whichever is more readily available, there is no
        /// need to set both values. Transform will use the last set value of either JulianDateTT or JulianDateUTC as the basis for its calculations.</remarks>
        public double JulianDateUTC
        {
            get
            {
                return julianDateUTCValue;
            }
            set
            {
                double tai1 = 0.0, tai2 = 0.0, tt1 = 0.0, tt2 = 0.0;

                // Validate the supplied value, it must be 0.0 or within the permitted range
                if ((value != 0.0) & ((value < JULIAN_DATE_MINIMUM_VALUE) | (value > JULIAN_DATE_MAXIMUM_VALUE)))
                    throw new InvalidValueException("JulianDateUTC", value.ToString(), JULIAN_DATE_MINIMUM_VALUE.ToString(), JULIAN_DATE_MAXIMUM_VALUE.ToString());

                julianDateUTCValue = value;
                requiresRecalculate = true; // Force a recalculation because the Julian date has changed

                if (julianDateUTCValue != 0.0)
                {
                    // Calculate Terrestrial Time equivalent
                    if ((Sofa.Utctai(julianDateUTCValue, 0.0, ref tai1, ref tai2) != 0))
                        LogMessage("JulianDateUTC Set", "UtcTai - Bad return code");
                    if ((Sofa.Taitt(tai1, tai2, ref tt1, ref tt2) != 0))
                        LogMessage("JulianDateUTC Set", "TaiTt - Bad return code");
                    julianDateTTValue = tt1 + tt2;

                    LogMessage("JulianDateUTC Set", $"JDUTC: {julianDateUTCValue} ({Utilities.JulianDateToDateTime(julianDateUTCValue).ToString(DATE_FORMAT)}), JDTT: {julianDateTTValue} ({Utilities.JulianDateToDateTime(julianDateTTValue).ToString(DATE_FORMAT)})");
                }
                else
                {
                    julianDateTTValue = 0.0;
                    LogMessage("JulianDateUTC Set", "Calculations will now be based on PC the DateTime");
                }
            }
        }

        #endregion

        #region Support code
        private void CheckSet(string caller, double value, string errMsg)
        {
            if (double.IsNaN(value))
            {
                LogMessage(caller, "Throwing TransformUninitialisedException: " + errMsg);
                throw new TransformUninitialisedException(errMsg);
            }
        }

        private void J2000ToTopo()
        {
            double JDUTCSofa;
            double aob = 0.0, zob = 0.0, hob = 0.0, dob = 0.0, rob = 0.0, eo = 0.0;

            if (double.IsNaN(siteElevValue))
                throw new TransformUninitialisedException("Site elevation has not been set");
            if (double.IsNaN(siteLatValue))
                throw new TransformUninitialisedException("Site latitude has not been set");
            if (double.IsNaN(siteLongValue))
                throw new TransformUninitialisedException("Site longitude has not been set");
            if (double.IsNaN(siteTempValue))
                throw new TransformUninitialisedException("Site temperature has not been set");

            // Calculate site pressure at site elevation if this has not been provided
            CalculateSitePressureIfRequired();

            sw.Reset(); sw.Start();

            JDUTCSofa = GetJDUTCSofa();

            sw.Reset(); sw.Start();

            if (refracValue)
                Sofa.Atco13(raJ2000Value * HOURS2RADIANS, decJ2000Value * DEGREES2RADIANS, 0.0, 0.0, 0.0, 0.0, JDUTCSofa, 0.0, deltaUT1, siteLongValue * DEGREES2RADIANS, siteLatValue * DEGREES2RADIANS, siteElevValue, 0.0, 0.0, sitePressureValue, siteTempValue, 0.8, 0.57, ref aob, ref zob, ref hob, ref dob, ref rob, ref eo);
            else
                Sofa.Atco13(raJ2000Value * HOURS2RADIANS, decJ2000Value * DEGREES2RADIANS, 0.0, 0.0, 0.0, 0.0, JDUTCSofa, 0.0, deltaUT1, siteLongValue * DEGREES2RADIANS, siteLatValue * DEGREES2RADIANS, siteElevValue, 0.0, 0.0, 0.0, 0.0, 0.0, 0.57, ref aob, ref zob, ref hob, ref dob, ref rob, ref eo);

            raTopoValue = Sofa.Anp(rob - eo) * RADIANS2HOURS; // // Convert CIO RA to equinox of date RA by subtracting the equation of the origins and convert from radians to hours
            decTopoValue = dob * RADIANS2DEGREES; // Convert Dec from radians to degrees
            azimuthTopoValue = aob * RADIANS2DEGREES;
            elevationTopoValue = 90.0 - zob * RADIANS2DEGREES;

            LogMessage("  J2000 To Topo", "  Topocentric RA/DEC (including refraction if specified):  " + FormatRA(raTopoValue) + " " + FormatDec(decTopoValue) + " Refraction: " + refracValue.ToString() + ", " + sw.Elapsed.TotalMilliseconds.ToString("0.00") + "ms");
            LogMessage("  J2000 To Topo", "  Azimuth/Elevation: " + FormatDec(azimuthTopoValue) + " " + FormatDec(elevationTopoValue) + ", " + sw.Elapsed.TotalMilliseconds.ToString("0.00") + "ms");
            LogMessage("  J2000 To Topo", "  Completed");
            LogMessage("", "");
        }

        private void J2000ToApparent()
        {
            double ri = 0.0, di = 0.0, eo = 0.0;
            double JDTTSofa;

            sw.Reset(); sw.Start();
            JDTTSofa = GetJDTTSofa();

            Sofa.Atci13(raJ2000Value * HOURS2RADIANS, decJ2000Value * DEGREES2RADIANS, 0.0, 0.0, 0.0, 0.0, JDTTSofa, 0.0, ref ri, ref di, ref eo);
            raApparentValue = Sofa.Anp(ri - eo) * RADIANS2HOURS; // // Convert CIO RA to equinox of date RA by subtracting the equation of the origins and convert from radians to hours
            decApparentValue = di * RADIANS2DEGREES; // Convert Dec from radians to degrees

            LogMessage("  J2000 To Apparent", "  Apparent RA/Dec:   " + FormatRA(raApparentValue) + " " + FormatDec(decApparentValue) + ", " + sw.Elapsed.TotalMilliseconds.ToString("0.00") + "ms");
        }

        private void TopoToJ2000()
        {
            double RACelestrial = 0.0, DecCelestial = 0.0, JDTTSofa, JDUTCSofa;
            int RetCode;
            double aob = 0.0, zob = 0.0, hob = 0.0, dob = 0.0, rob = 0.0, eo = 0.0;

            if (double.IsNaN(siteElevValue))
                throw new TransformUninitialisedException("Site elevation has not been set");
            if (double.IsNaN(siteLatValue))
                throw new TransformUninitialisedException("Site latitude has not been set");
            if (double.IsNaN(siteLongValue))
                throw new TransformUninitialisedException("Site longitude has not been set");
            if (double.IsNaN(siteTempValue))
                throw new TransformUninitialisedException("Site temperature has not been set");

            // Calculate site pressure at site elevation if this has not been provided
            CalculateSitePressureIfRequired();

            sw.Reset(); sw.Start();

            JDUTCSofa = GetJDUTCSofa();
            JDTTSofa = GetJDTTSofa();

            sw.Reset(); sw.Start();
            if (refracValue)
                RetCode = Sofa.Atoc13("R", Sofa.Anp(raTopoValue * HOURS2RADIANS + Sofa.Eo06a(JDTTSofa, 0.0)), decTopoValue * DEGREES2RADIANS, JDUTCSofa, 0.0, deltaUT1, siteLongValue * DEGREES2RADIANS, siteLatValue * DEGREES2RADIANS, siteElevValue, 0.0, 0.0, sitePressureValue, siteTempValue, 0.85, 0.57, ref RACelestrial, ref DecCelestial);
            else
                RetCode = Sofa.Atoc13("R", Sofa.Anp(raTopoValue * HOURS2RADIANS + Sofa.Eo06a(JDTTSofa, 0.0)), decTopoValue * DEGREES2RADIANS, JDUTCSofa, 0.0, deltaUT1, siteLongValue * DEGREES2RADIANS, siteLatValue * DEGREES2RADIANS, siteElevValue, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, ref RACelestrial, ref DecCelestial);

            raJ2000Value = RACelestrial * RADIANS2HOURS;
            decJ2000Value = DecCelestial * RADIANS2DEGREES;
            LogMessage("  Topo To J2000", "  J2000 RA/Dec:" + FormatRA(raJ2000Value) + " " + FormatDec(decJ2000Value) + ", " + sw.Elapsed.TotalMilliseconds.ToString("0.00") + "ms");

            // Now calculate the corresponding AzEl values from the J2000 values
            sw.Reset(); sw.Start();
            if (refracValue)
                Sofa.Atco13(raJ2000Value * HOURS2RADIANS, decJ2000Value * DEGREES2RADIANS, 0.0, 0.0, 0.0, 0.0, JDUTCSofa, 0.0, deltaUT1, siteLongValue * DEGREES2RADIANS, siteLatValue * DEGREES2RADIANS, siteElevValue, 0.0, 0.0, sitePressureValue, siteTempValue, 0.8, 0.57, ref aob, ref zob, ref hob, ref dob, ref rob, ref eo);
            else
                Sofa.Atco13(raJ2000Value * HOURS2RADIANS, decJ2000Value * DEGREES2RADIANS, 0.0, 0.0, 0.0, 0.0, JDUTCSofa, 0.0, deltaUT1, siteLongValue * DEGREES2RADIANS, siteLatValue * DEGREES2RADIANS, siteElevValue, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, ref aob, ref zob, ref hob, ref dob, ref rob, ref eo);

            azimuthTopoValue = aob * RADIANS2DEGREES;
            elevationTopoValue = 90.0 - zob * RADIANS2DEGREES;

            LogMessage("  Topo To J2000", "  Azimuth/Elevation: " + FormatDec(azimuthTopoValue) + " " + FormatDec(elevationTopoValue) + ", " + sw.Elapsed.TotalMilliseconds.ToString("0.00") + "ms");
        }

        private void ApparentToJ2000()
        {
            double JulianDateTTSofa, RACelestial = 0.0, DecCelestial = 0.0, JulianDateUTCSofa, eo = 0.0;

            sw.Reset(); sw.Start();

            JulianDateTTSofa = GetJDTTSofa();
            JulianDateUTCSofa = GetJDUTCSofa();

            Sofa.Atic13(Sofa.Anp(raApparentValue * HOURS2RADIANS + Sofa.Eo06a(JulianDateUTCSofa, 0.0)), decApparentValue * DEGREES2RADIANS, JulianDateTTSofa, 0.0, ref RACelestial, ref DecCelestial, ref eo);
            raJ2000Value = RACelestial * RADIANS2HOURS;
            decJ2000Value = DecCelestial * RADIANS2DEGREES;
            LogMessage("  Apparent To J2000", "  J2000 RA/Dec" + FormatRA(raJ2000Value) + " " + FormatDec(decJ2000Value) + ", " + sw.Elapsed.TotalMilliseconds.ToString("0.00") + "ms");
        }

        private void Recalculate() // Calculate values for derived co-ordinates
        {
            swRecalculate.Reset(); swRecalculate.Start();
            if (requiresRecalculate | (refracValue == true))
            {
                LogMessage("Recalculate", $"Requires Recalculate: {requiresRecalculate}, Refraction: {refracValue}, Latitude: {siteLatValue}, Longitude: {siteLongValue}, Elevation: {siteElevValue}, Temperature: {siteTempValue}");
                switch (lastSetBy)
                {
                    case SetBy.J2000 // J2000 coordinates have bee set so calculate apparent and topocentric coordinates
                   :
                        {
                            LogMessage("  Recalculate", "  Values last set by SetJ2000");
                            // Check whether required topo values have been set
                            if ((!double.IsNaN(siteLatValue)) & (!double.IsNaN(siteLongValue)) & (!double.IsNaN(siteElevValue)) & (!double.IsNaN(siteTempValue)))
                                J2000ToTopo(); // All required site values present so calculate Topo values
                            else
                            {
                                raTopoValue = double.NaN;
                                decTopoValue = double.NaN;
                                azimuthTopoValue = double.NaN;
                                elevationTopoValue = double.NaN;
                            }
                            J2000ToApparent();
                            break;
                        }

                    case SetBy.Topocentric // Topocentric co-ordinates have been set so calculate J2000 and apparent coordinates
             :
                        {
                            LogMessage("  Recalculate", "  Values last set by SetTopocentric");
                            // Check whether required topo values have been set
                            if ((!double.IsNaN(siteLatValue)) & (!double.IsNaN(siteLongValue)) & (!double.IsNaN(siteElevValue)) & (!double.IsNaN(siteTempValue)))
                            {
                                TopoToJ2000();
                                J2000ToApparent();
                            }
                            else
                            {
                                raJ2000Value = double.NaN;
                                decJ2000Value = double.NaN;
                                raApparentValue = double.NaN;
                                decApparentValue = double.NaN;
                                azimuthTopoValue = double.NaN;
                                elevationTopoValue = double.NaN;
                            }

                            break;
                        }

                    case SetBy.Apparent // Apparent values have been set so calculate J2000 values and topo values if appropriate
             :
                        {
                            LogMessage("  Recalculate", "  Values last set by SetApparent");
                            ApparentToJ2000(); // Calculate J2000 value
                                               // Check whether required topo values have been set
                            if ((!double.IsNaN(siteLatValue)) & (!double.IsNaN(siteLongValue)) & (!double.IsNaN(siteElevValue)) & (!double.IsNaN(siteTempValue)))
                                J2000ToTopo(); // All required site values present so calculate Topo values
                            else
                            {
                                raTopoValue = double.NaN;
                                decTopoValue = double.NaN;
                                azimuthTopoValue = double.NaN;
                                elevationTopoValue = double.NaN;
                            }

                            break;
                        }

                    case SetBy.AzimuthElevation:
                        {
                            LogMessage("  Recalculate", "  Values last set by AzimuthElevation");
                            if ((!double.IsNaN(siteLatValue)) & (!double.IsNaN(siteLongValue)) & (!double.IsNaN(siteElevValue)) & (!double.IsNaN(siteTempValue)))
                            {
                                AzElToJ2000();
                                J2000ToTopo();
                                J2000ToApparent();
                            }
                            else
                            {
                                raJ2000Value = double.NaN;
                                decJ2000Value = double.NaN;
                                raApparentValue = double.NaN;
                                decApparentValue = double.NaN;
                                raTopoValue = double.NaN;
                                decTopoValue = double.NaN;
                            }

                            break;
                        }

                    default:
                        {
                            LogMessage("Recalculate", "Neither SetJ2000 nor SetTopocentric nor SetApparent have been called. Throwing TransforUninitialisedException");
                            throw new TransformUninitialisedException("Can't recalculate Transform object values because neither SetJ2000 nor SetTopocentric nor SetApparent have been called");
                        }
                }
                LogMessage("  Recalculate", "  Completed in " + swRecalculate.Elapsed.TotalMilliseconds.ToString("0.00") + "ms");
                requiresRecalculate = false; // Reset the recalculate flag
            }
            else
                LogMessage("  Recalculate", "No parameters have changed, refraction is " + refracValue + ", recalculation not required");
            swRecalculate.Stop();
        }

        private void AzElToJ2000()
        {
            int RetCode;
            double JulianDateUTCSofa, RACelestial = 0.0, DecCelestial = 0.0;

            sw.Reset(); sw.Start();

            if (double.IsNaN(siteElevValue))
                throw new TransformUninitialisedException("Site elevation has not been set");
            if (double.IsNaN(siteLatValue))
                throw new TransformUninitialisedException("Site latitude has not been set");
            if (double.IsNaN(siteLongValue))
                throw new TransformUninitialisedException("Site longitude has not been set");
            if (double.IsNaN(siteTempValue))
                throw new TransformUninitialisedException("Site temperature has not been set");

            // Calculate site pressure at site elevation if this has not been provided
            CalculateSitePressureIfRequired();

            JulianDateUTCSofa = GetJDUTCSofa();

            if (refracValue)
                RetCode = Sofa.Atoc13("A", azimuthTopoValue * DEGREES2RADIANS, (90.0 - elevationTopoValue) * DEGREES2RADIANS, JulianDateUTCSofa, 0.0, deltaUT1, siteLongValue * DEGREES2RADIANS, siteLatValue * DEGREES2RADIANS, siteElevValue, 0.0, 0.0, sitePressureValue, siteTempValue, 0.85, 0.57, ref RACelestial, ref DecCelestial);
            else
                RetCode = Sofa.Atoc13("A", azimuthTopoValue * DEGREES2RADIANS, (90.0 - elevationTopoValue) * DEGREES2RADIANS, JulianDateUTCSofa, 0.0, deltaUT1, siteLongValue * DEGREES2RADIANS, siteLatValue * DEGREES2RADIANS, siteElevValue, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, ref RACelestial, ref DecCelestial);

            raJ2000Value = RACelestial * RADIANS2HOURS;
            decJ2000Value = DecCelestial * RADIANS2DEGREES;

            LogMessage("  AzEl To J2000", "  SOFA RA: " + FormatRA(raJ2000Value) + ", Declination: " + FormatDec(decJ2000Value));

            sw.Stop();
            LogMessage("", "");
        }

        private double GetJDUTCSofa()
        {
            double Retval, utc1 = 0.0, utc2 = 0.0;
            DateTime Now;

            if (julianDateUTCValue == 0.0)
            {
                Now = DateTime.UtcNow;
                if (Sofa.Dtf2d("UTC", Now.Year, Now.Month, Now.Day, Now.Hour, Now.Minute, Convert.ToDouble(Now.Second) + Convert.ToDouble(Now.Millisecond) / 1000.0, ref utc1, ref utc2) != 0)
                {
                    LogMessage("Dtf2d", "Bad return code");
                }

                Retval = utc1 + utc2;
                LogMessage("  GetJDUTCSofa", "  Current Julian Date: " + Retval.ToString() + " " + Utilities.JulianDateToDateTime(Retval).ToString(DATE_FORMAT));
            }
            else
            {
                Retval = julianDateUTCValue;
                LogMessage("  GetJDUTCSofa", "  Set Julian Date: " + Retval.ToString() + " " + Utilities.JulianDateToDateTime(Retval).ToString(DATE_FORMAT));
            }

            return Retval;
        }

        private double GetJDTTSofa()
        {
            double Retval, utc1 = 0.0, utc2 = 0.0, tai1 = 0.0, tai2 = 0.0, tt1 = 0.0, tt2 = 0.0;
            DateTime Now;

            if (julianDateTTValue == 0.0)
            {
                Now = DateTime.UtcNow;

                // First calculate the UTC Julian date, then convert this to the equivalent TAI Julian date then convert this to the equivalent TT Julian date
                if ((Sofa.Dtf2d("UTC", Now.Year, Now.Month, Now.Day, Now.Hour, Now.Minute, System.Convert.ToDouble(Now.Second) + Convert.ToDouble(Now.Millisecond) / 1000.0, ref utc1, ref utc2) != 0))
                    LogMessage("Dtf2d", "Bad return code");
                if ((Sofa.Utctai(utc1, utc2, ref tai1, ref tai2) != 0))
                    LogMessage("GetJDTTSofa", "UtcTai - Bad return code");
                if ((Sofa.Taitt(tai1, tai2, ref tt1, ref tt2) != 0))
                    LogMessage("GetJDTTSofa", "TaiTt - Bad return code");

                Retval = tt1 + tt2;
            }
            else
                Retval = julianDateTTValue;
            LogMessage("  GetJDTTSofa", "  " + Retval.ToString() + " " + Utilities.JulianDateToDateTime(Retval).ToString(DATE_FORMAT));
            return Retval;
        }

        private double ValidateRA(string caller, double ra)
        {
            if ((ra < 0.0) | (ra >= 24.0))
                throw new InvalidValueException(caller, ra.ToString(), "0 to 23.9999");
            return ra;
        }

        private double ValidateDec(string caller, double dec)
        {
            if ((dec < -90.0) | (dec > 90.0))
                throw new InvalidValueException(caller, dec.ToString(), "-90.0 to 90.0");
            return dec;
        }

        private string FormatRA(double ra)
        {
            return Utilities.HoursToHMS(ra, ":", ":", "", 3);
        }

        private string FormatDec(double Dec)
        {
            return Utilities.DegreesToDMS(Dec, ":", ":", "", 3);
        }

        private void CalculateSitePressureIfRequired()
        {
            // Derive the site pressure from the site elevation if the pressure has not been set explicitly
            if (!double.IsNaN(sitePressureValue))
                LogMessage("  CalculateSitePressure", $"  Site pressure has been set to {sitePressureValue}hPa.");
            else
            {
                // phpa = 1013.25 * exp ( −hm / ( 29.3 * tsl ) ); NOTE this equation calculates the site pressure and uses the site temperature REDUCED TO SEA LEVEL MESURED IN DEGREES KELVIN
                // tsl = tSite − 0.0065(0 − hsite);  NOTE this equation reduces the site temperature to sea level
                sitePressureValue = STANDARD_PRESSURE * Math.Exp(-siteElevValue / (29.3 * (siteTempValue + 0.0065 * siteElevValue - ABSOLUTE_ZERO_CELSIUS)));
                LogMessage("  CalculateSitePressure", $"  Site pressure has not been set by user, calculated value: {sitePressureValue}hPa.");
            }
        }

        private void LogMessage(string method, string message)
        {
            if (loggerIsTraceLogger) traceLogger?.LogMessage(method, message);
            else iLogger?.Log(LogLevel.Debug, $"{method} - {message}");
        }

        #endregion
    }
}
