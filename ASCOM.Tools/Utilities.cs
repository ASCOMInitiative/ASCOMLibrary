using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;

namespace ASCOM.Tools
{
    /// <summary>
    /// ASCOM support utilities
    /// </summary>
    public class Utilities : IDisposable
    {
        private static double currentLeapSeconds;

        /// <summary>
        /// Information about the body
        /// </summary>
        private struct BodyInfo
        {
            /// <summary>
            /// Body altitude
            /// </summary>
            public double Altitude;

            /// <summary>
            /// Distance to body
            /// </summary>
            public double Distance;

            /// <summary>
            /// Radius of the body
            /// </summary>
            public double Radius;
        }

        #region Initialiser

        // Static initialiser - runs once to initialise static variables
        static Utilities()
        {
            currentLeapSeconds = Constants.LEAP_SECONDS_DEFAULT;
        }

        #endregion

        #region AstroUtils

        /// <summary>
        /// Function that returns a list of rise and set events of a particular type that occur on a particular day at a given latitude, longitude and time zone
        /// </summary>
        /// <param name="TypeofEvent">Type of event e.g. Sunrise or Astronomical twilight</param>
        /// <param name="Day">Integer Day number</param>
        /// <param name="Month">Integer Month number</param>
        /// <param name="Year">Integer Year number</param>
        /// <param name="SiteLatitude">Site latitude</param>
        /// <param name="SiteLongitude">Site longitude (West of Greenwich is negative)</param>
        /// <param name="SiteTimeZone">Site time zone offset (West of Greenwich is negative)</param>
        /// <returns>An arraylist of event information (see Remarks for arraylist structure).
        /// </returns>
        /// <exception cref="ASCOM.InvalidValueException">If the combination of day, month and year is invalid e.g. 31st September.</exception>
        /// <remarks>
        /// <para>The definitions of sunrise, sunset and the various twilights that are used in this method are taken from the 
        /// <a href="http://aa.usno.navy.mil/faq/docs/RST_defs.php">US Naval Observatory Definitions</a>.
        /// </para>
        /// <para>The dynamics of the sun, Earth and Moon can result at some latitudes in days where there may be no, 1 or 2 rise or set events during 
        /// a 24 hour period; in consequence, results are returned in the flexible form of arraylist.</para>
        /// <para>The returned zero based arraylist has the following values:
        /// <list type="Bullet">
        /// <item>Arraylist(0)                              - Boolean - True if the body is above the event limit at midnight (the beginning of the 24 hour day), false if it is below the event limit</item>
        /// <item>Arraylist(1)                              - Integer - Number of rise events in this 24 hour period</item>
        /// <item>Arraylist(2)                              - Integer - Number of set events in this 24 hour period</item>
        /// <item>Arraylist(3) onwards                      - Double  - Values of rise events in hours </item>
        /// <item>Arraylist(3 + NumberOfRiseEvents) onwards - Double  - Values of set events in hours </item>
        /// </list></para>
        /// <para>If the number of rise events is zero the first double value will be the first set event. If the numbers of both rise and set events
        /// are zero, there will be no double values and the arraylist will just contain elements 0, 1 and 2, the above/below horizon flag and the integer count values.</para>
        /// <para>The algorithm employed in this method is taken from Astronomy on the Personal Computer (Montenbruck and Pfleger) pp 46..56, 
        /// Springer Fourth Edition 2000, Fourth Printing 2009. The day is divided into twelve two hour intervals and a quadratic equation is fitted
        /// to the altitudes at the beginning, middle and end of each interval. The resulting equation coefficients are then processed to determine 
        /// the number of roots within the interval (each of which corresponds to a rise or set event) and their sense (rise or set). 
        /// These results are are then aggregated over the day and the resultant list of values returned as the function result.
        /// </para>
        /// <para>High precision ephemeredes for the Sun, Moon and Earth and other planets from the JPL DE421 series are employed as delivered by the 
        /// ASCOM NOVAS 3.1 component rather than using the lower precision ephemeredes employed by Montenbruck and Pfleger.
        /// </para>
        /// <para><b>Accuracy</b> Whole year almanacs for Sunrise/Sunset, Moonrise/Moonset and the various twilights every 5 degrees from the 
        /// North pole to the South Pole at a variety of longitudes, timezones and dates have been compared to data from
        /// the <a href="http://aa.usno.navy.mil/data/docs/RS_OneYear.php">US Naval Observatory Astronomical Data</a> web site. The RMS error has been found to be 
        /// better than 0.5 minute over the latitude range 80 degrees North to 80 degrees South and better than 5 minutes from 80 degrees to the relevant pole.
        /// Most returned values are within 1 minute of the USNO values although some very infrequent grazing event times at lattiudes from 67 to 90 degrees North and South can be up to 
        /// 10 minutes different.
        /// </para>
        /// <para>An Almanac program that creates a year's worth of information for a given event, lattitude, longitude and timezone is included in the 
        /// developer code examples elsewhere in this help file. This creates an output file with an almost identical format to that used by the USNO web site 
        /// and allows comprehensive checking of acccuracy for a given set of parameters.</para>
        /// </remarks>
        public static RiseSetTimes EventTimes(EventType TypeofEvent, int Day, int Month, int Year, double SiteLatitude, double SiteLongitude, double SiteTimeZone)
        {
            bool DoesRise, DoesSet, AboveHorizon = default;
            double CentreTime, AltitiudeMinus1, Altitiude0, AltitiudePlus1, a, b, c, XSymmetry, Discriminant, RefractionCorrection;
            double DeltaX, Zero1, Zero2, JD;
            int NZeros;
            var Observer = new OnSurface();
            RiseSetTimes Retval = new RiseSetTimes();
            List<double> BodyRises = new List<double>(), BodySets = new List<double>();
            BodyInfo BodyInfoMinus1, BodyInfo0, BodyInfoPlus1;
            DateTime TestDate;

            DoesRise = false;
            DoesSet = false;

            try
            {
                TestDate = DateTime.Parse(Month + "/" + Day + "/" + Year, System.Globalization.CultureInfo.InvariantCulture); // Test whether this is a valid date e.g is not the 31st of February
            }
            catch (FormatException) // Catch case where day exceeds the maximum number of days in the month
            {
                throw new InvalidValueException("Day or Month", Day.ToString() + " " + Month.ToString() + " " + Year.ToString(), "Day must not exceed the number of days in the month");
            }
            catch (Exception) // Throw all other exceptions as they are are received
            {
                throw;
            }

            // Calculate Julian date in the local timezone
            JD = Novas.JulianDate((short)Year, (short)Month, (short)Day, 0.0d) - SiteTimeZone / 24.0d;

            // Initialise observer structure and calculate the refraction at the hozrizon
            Observer.Latitude = SiteLatitude;
            Observer.Longitude = SiteLongitude;
            RefractionCorrection = Novas.Refract(Observer, RefractionOption.StandardRefraction, 90.0d);

            // Iterate over the day in two hour periods

            // Start at 01:00 as the centre time i.e. then time range will be 00:00 to 02:00
            CentreTime = 1.0d;

            do
            {
                // Calculate body positional information
                BodyInfoMinus1 = BodyAltitude(TypeofEvent, JD, CentreTime - 1d, SiteLatitude, SiteLongitude);
                BodyInfo0 = BodyAltitude(TypeofEvent, JD, CentreTime, SiteLatitude, SiteLongitude);
                BodyInfoPlus1 = BodyAltitude(TypeofEvent, JD, CentreTime + 1d, SiteLatitude, SiteLongitude);

                // Correct altitude for body's apparent size, parallax, required distance below horizon and refraction
                switch (TypeofEvent)
                {
                    case EventType.MoonRiseMoonSet:
                        {
                            // Parallax and apparent size are dynamically calculated for the Moon because it is so close and does not transcribe a circular orbit
                            AltitiudeMinus1 = BodyInfoMinus1.Altitude - Constants.EARTH_RADIUS * Constants.RAD2DEG / BodyInfoMinus1.Distance + BodyInfoMinus1.Radius * Constants.RAD2DEG / BodyInfoMinus1.Distance + RefractionCorrection;
                            Altitiude0 = BodyInfo0.Altitude - Constants.EARTH_RADIUS * Constants.RAD2DEG / BodyInfo0.Distance + BodyInfo0.Radius * Constants.RAD2DEG / BodyInfo0.Distance + RefractionCorrection;
                            AltitiudePlus1 = BodyInfoPlus1.Altitude - Constants.EARTH_RADIUS * Constants.RAD2DEG / BodyInfoPlus1.Distance + BodyInfoPlus1.Radius * Constants.RAD2DEG / BodyInfoPlus1.Distance + RefractionCorrection;
                            break;
                        }
                    case EventType.SunRiseSunset:
                        {
                            AltitiudeMinus1 = BodyInfoMinus1.Altitude - Constants.SUN_RISE;
                            Altitiude0 = BodyInfo0.Altitude - Constants.SUN_RISE;
                            AltitiudePlus1 = BodyInfoPlus1.Altitude - Constants.SUN_RISE;
                            break;
                        }
                    case EventType.CivilTwilight:
                        {
                            AltitiudeMinus1 = BodyInfoMinus1.Altitude - Constants.CIVIL_TWILIGHT;
                            Altitiude0 = BodyInfo0.Altitude - Constants.CIVIL_TWILIGHT;
                            AltitiudePlus1 = BodyInfoPlus1.Altitude - Constants.CIVIL_TWILIGHT;
                            break;
                        }
                    case EventType.NauticalTwilight:
                        {
                            AltitiudeMinus1 = BodyInfoMinus1.Altitude - Constants.NAUTICAL_TWILIGHT;
                            Altitiude0 = BodyInfo0.Altitude - Constants.NAUTICAL_TWILIGHT;
                            AltitiudePlus1 = BodyInfoPlus1.Altitude - Constants.NAUTICAL_TWILIGHT;
                            break;
                        }
                    case EventType.AmateurAstronomicalTwilight:
                        {
                            AltitiudeMinus1 = BodyInfoMinus1.Altitude - Constants.AMATEUR_ASRONOMICAL_TWILIGHT;
                            Altitiude0 = BodyInfo0.Altitude - Constants.AMATEUR_ASRONOMICAL_TWILIGHT;
                            AltitiudePlus1 = BodyInfoPlus1.Altitude - Constants.AMATEUR_ASRONOMICAL_TWILIGHT;
                            break;
                        }
                    case EventType.AstronomicalTwilight:
                        {
                            AltitiudeMinus1 = BodyInfoMinus1.Altitude - Constants.ASTRONOMICAL_TWILIGHT;
                            Altitiude0 = BodyInfo0.Altitude - Constants.ASTRONOMICAL_TWILIGHT;
                            AltitiudePlus1 = BodyInfoPlus1.Altitude - Constants.ASTRONOMICAL_TWILIGHT; // Planets so correct for radius of plant and refraction
                            break;
                        }

                    default:
                        {
                            AltitiudeMinus1 = BodyInfoMinus1.Altitude + RefractionCorrection + Constants.RAD2DEG * BodyInfo0.Radius / BodyInfo0.Distance;
                            Altitiude0 = BodyInfo0.Altitude + RefractionCorrection + Constants.RAD2DEG * BodyInfo0.Radius / BodyInfo0.Distance;
                            AltitiudePlus1 = BodyInfoPlus1.Altitude + RefractionCorrection + Constants.RAD2DEG * BodyInfo0.Radius / BodyInfo0.Distance;
                            break;
                        }
                }

                if (CentreTime == 1.0d)
                {
                    if (AltitiudeMinus1 < 0d)
                    {
                        AboveHorizon = false;
                    }
                    else
                    {
                        AboveHorizon = true;
                    }
                }

                // Assess quadratic equation
                c = Altitiude0;
                b = 0.5d * (AltitiudePlus1 - AltitiudeMinus1);
                a = 0.5d * (AltitiudePlus1 + AltitiudeMinus1) - Altitiude0;

                XSymmetry = -b / (2.0d * a);
                Discriminant = b * b - 4.0d * a * c;

                Zero1 = double.NaN;
                Zero2 = double.NaN;
                NZeros = 0;

                if (Discriminant > 0.0d)                 // there are zeros
                {
                    DeltaX = 0.5d * Math.Sqrt(Discriminant) / Math.Abs(a);
                    Zero1 = XSymmetry - DeltaX;
                    Zero2 = XSymmetry + DeltaX;
                    if (Math.Abs(Zero1) <= 1.0d)
                        NZeros++; // This zero is in interval
                    if (Math.Abs(Zero2) <= 1.0d)
                        NZeros++; // This zero is in interval

                    if (Zero1 < -1.0d)
                        Zero1 = Zero2;
                }

                switch (NZeros)
                {
                    // cases depend on values of discriminant - inner part of STEP 4
                    case 0: // nothing  - go to next time slot
                        {
                            break;
                        }
                    case 1:                      // simple rise / set event
                        {
                            if (AltitiudeMinus1 < 0.0d)       // The body is set at start of event so this must be a rising event
                            {
                                DoesRise = true;
                                BodyRises.Add(CentreTime + Zero1);
                            }
                            else                    // must be setting
                            {
                                DoesSet = true;
                                BodySets.Add(CentreTime + Zero1);
                            }

                            break;
                        }
                    case 2:                      // rises and sets within interval
                        {
                            if (AltitiudeMinus1 < 0.0d) // The body is set at start of event so it must rise first then set
                            {
                                BodyRises.Add(CentreTime + Zero1);
                                BodySets.Add(CentreTime + Zero2);
                            }
                            else                    // The body is risen at the start of the event so it must set first then rise
                            {
                                BodyRises.Add(CentreTime + Zero2);
                                BodySets.Add(CentreTime + Zero1);
                            }
                            DoesRise = true;
                            DoesSet = true;
                            break;
                        }
                        // Zero2 = 1
                }
                CentreTime += 2.0d; // Increment by 2 hours to get the next 2 hour slot in the day
            }

            while (!(DoesRise & DoesSet & Math.Abs(SiteLatitude) < 60.0d | CentreTime == 25.0d));

            Retval.IsRisen = AboveHorizon; // Add above horizon at midnight flag
            Retval.RiseEvents = BodyRises;
            Retval.SetEvents = BodySets;

            return Retval;
        }

        /// <summary>
        /// Returns the fraction of the Moon's surface that is illuminated 
        /// </summary>
        /// <param name="JD">Julian day (UTC) for which the Moon illumination is required</param>
        /// <returns>Percentage illumination of the Moon</returns>
        /// <remarks> The algorithm used is that given in Astronomical Algorithms (Second Edition, Corrected to August 2009) 
        /// Chapter 48 p345 by Jean Meeus (Willmann-Bell 1991). The Sun and Moon positions are calculated by high precision NOVAS 3.1 library using JPL DE 421 ephemeredes.
        /// </remarks>
        public static double MoonIllumination(double JD)
        {
            var Obj3 = new Object3();
            var Location = new OnSurface();
            var Cat = new CatEntry3();
            SkyPos SunPosition = new SkyPos(), MoonPosition = new SkyPos();
            var Obs = new Observer();
            double Phi, Inc, k, deltaT;

            // DeltaT = DeltaTCalc(JD)
            deltaT = DeltaT(JD);

            // Calculate Moon RA, Dec and distance
            Obj3.Name = "Moon";
            Obj3.Number = Body.Moon;
            Obj3.Star = Cat;
            Obj3.Type = ObjectType.MajorPlanetSunOrMoon;

            Obs.OnSurf = Location;
            Obs.Where = ObserverLocation.EarthGeoCenter;

            Novas.Place(JD + deltaT * Constants.SECONDS2DAYS, Obj3, Obs, deltaT, CoordSys.EquinoxOfDate, Accuracy.Full, ref MoonPosition);

            // Calculate Sun RA, Dec and distance
            Obj3.Name = "Sun";
            Obj3.Number = Body.Sun;
            Obj3.Star = Cat;
            Obj3.Type = ObjectType.MajorPlanetSunOrMoon;

            Novas.Place(JD + deltaT * Constants.SECONDS2DAYS, Obj3, Obs, deltaT, CoordSys.EquinoxOfDate, Accuracy.Full, ref SunPosition);

            // Calculate geocentriic elongation of the Moon
            Phi = Math.Acos(Math.Sin(SunPosition.Dec * Constants.DEG2RAD) * Math.Sin(MoonPosition.Dec * Constants.DEG2RAD) + Math.Cos(SunPosition.Dec * Constants.DEG2RAD) * Math.Cos(MoonPosition.Dec * Constants.DEG2RAD) * Math.Cos((SunPosition.RA - MoonPosition.RA) * Constants.HOURS2DEG * Constants.DEG2RAD));

            // Calculate the phase angle of the Moon
            Inc = Math.Atan2(SunPosition.Dis * Math.Sin(Phi), MoonPosition.Dis - SunPosition.Dis * Math.Cos(Phi));

            // Calculate the illuminated fraction of the Moon's disc
            k = (1.0d + Math.Cos(Inc)) / 2.0d;

            return k;
        }

        /// <summary>
        /// Returns the Moon phase as an angle
        /// </summary>
        /// <param name="JD">Julian day (UTC) for which the Moon phase is required</param>
        /// <returns>Moon phase as an angle between -180.0 amd +180.0 (see Remarks for further description)</returns>
        /// <remarks>To allow maximum freedom in displaying the Moon phase, this function returns the excess of the apparent geocentric longitude
        /// of the Moon over the apparent geocentric longitude of the Sun, expressed as an angle in the range -180.0 to +180.0 degrees.
        /// This definition is taken from Astronomical Algorithms (Second Edition, Corrected to August 2009) Chapter 49 p349
        /// by Jean Meeus (Willmann-Bell 1991).
        /// <para>The frequently used eight phase description for phases of the Moon can be easily constructed from the results of this function
        /// using logic similar to the following:
        /// <code>
        /// Select Case MoonPhase
        ///     Case -180.0 To -135.0
        ///         Phase = "Full Moon"
        ///     Case -135.0 To -90.0
        ///         Phase = "Waning Gibbous"
        ///     Case -90.0 To -45.0
        ///         Phase = "Last Quarter"
        ///     Case -45.0 To 0.0
        ///         Phase = "Waning Crescent"
        ///     Case 0.0 To 45.0
        ///         Phase = "New Moon"
        ///     Case 45.0 To 90.0
        ///         Phase = "Waxing Crescent"
        ///     Case 90.0 To 135.0
        ///         Phase = "First Quarter"
        ///     Case 135.0 To 180.0
        ///         Phase = "Waxing Gibbous"
        /// End Select
        /// </code></para>
        /// <para>Other representations can be easily constructed by changing the angle ranges and text descriptors as desired. The result range -180 to +180
        /// was chosen so that negative values represent the Moon waning and positive values represent the Moon waxing.</para>
        /// </remarks>
        public static double MoonPhase(double JD)
        {
            var Obj3 = new Object3();
            var Location = new OnSurface();
            var Cat = new CatEntry3();
            SkyPos SunPosition = new SkyPos(), MoonPosition = new SkyPos();
            var Obs = new Observer();
            double PositionAngle, deltaT;

            // DeltaT = DeltaTCalc(JD)
            deltaT = DeltaT(JD);

            // Calculate Moon RA, Dec and distance
            Obj3.Name = "Moon";
            Obj3.Number = Body.Moon;
            Obj3.Star = Cat;
            Obj3.Type = ObjectType.MajorPlanetSunOrMoon;

            Obs.OnSurf = Location;
            Obs.Where = ObserverLocation.EarthGeoCenter;

            Novas.Place(JD + deltaT * Constants.SECONDS2DAYS, Obj3, Obs, deltaT, CoordSys.EquinoxOfDate, Accuracy.Full, ref MoonPosition);

            // Calculate Sun RA, Dec and distance
            Obj3.Name = "Sun";
            Obj3.Number = Body.Sun;
            Obj3.Star = Cat;
            Obj3.Type = ObjectType.MajorPlanetSunOrMoon;

            Novas.Place(JD + deltaT * Constants.SECONDS2DAYS, Obj3, Obs, deltaT, CoordSys.EquinoxOfDate, Accuracy.Full, ref SunPosition);

            // Return the difference between the sun and moon RA's expressed as degrees from -180 to +180
            PositionAngle = Range((MoonPosition.RA - SunPosition.RA) * Constants.HOURS2DEG, -180.0d, false, 180.0d, true);

            return PositionAngle;

        }

        #endregion

        #region Leap seconds and DeltaT Calculation

        /// <summary>
        /// Current number of leap seconds
        /// </summary>
        public static double LeapSeconds
        {
            get
            {
                // Return the current leap seconds value
                return currentLeapSeconds;
            }
        }

        /// <summary>
        /// Set the current number of leap seconds
        /// </summary>
        /// <param name="leapSeconds"></param>
        public static void SetLeapSeconds(double leapSeconds)
        {
            // Set the internal leap seconds value to the supplied value
            if (leapSeconds < 0.0)
                throw new InvalidValueException($"Utlities.SetLeapSeconds - Supplied value is zero or less: {leapSeconds}");

            // Save the provided value
            currentLeapSeconds = leapSeconds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="julianDateUTC"></param>
        /// <returns></returns>
        public static double DeltaT(double julianDateUTC)
        {
            const double TAB_START_1620 = 1620.0;
            const int TAB_SIZE = 392;
            const double MODIFIED_JULIAN_DAY_OFFSET = 2400000.5; // This is the offset of Modified Julian dates from true Julian dates
            const double J2000_BASE = 2451545.0; // TDB Julian date of epoch J2000.0.
            const double TROPICAL_YEAR_IN_DAYS = 365.24219;
            const double TT_TAI_OFFSET = 32.184; // '32.184 seconds
            const double LEAP_SECOND_ULTIMATE_FALLBACK_VALUE = 37.0; // Ultimate fallback-back value for number of leap seconds if all else fails

            double yearFraction, b, retval, modifiedJulianDay;

            yearFraction = 2000.0 + (julianDateUTC - J2000_BASE) / (double)TROPICAL_YEAR_IN_DAYS; // This calculation is accurate enough for our purposes here (T0 = 2451545.0 is TDB Julian date of epoch J2000.0)
            modifiedJulianDay = julianDateUTC - MODIFIED_JULIAN_DAY_OFFSET;

            // NOTE: Starting April 2018 - Please note the use of modified Julian date in the formula rather than year fraction as in previous formulae

            // DATE RANGE 18th July 2023 Onwards - This is beyond the sensible extrapolation range of the most recent data analysis so revert to the basic formula: DeltaT = LeapSeconds + 32.184
            if ((yearFraction >= 2023.55))
            {
                // Ultimate fallback value if all else fails!
                retval = LEAP_SECOND_ULTIMATE_FALLBACK_VALUE + TT_TAI_OFFSET;
            }
            else if ((yearFraction >= 2022.55))
                retval = (-0.000000000000528908084762244 * modifiedJulianDay * modifiedJulianDay * modifiedJulianDay * modifiedJulianDay * modifiedJulianDay)
                    + (+0.000000158529137391645 * modifiedJulianDay * modifiedJulianDay * modifiedJulianDay * modifiedJulianDay)
                    + (-0.0190063060965729 * modifiedJulianDay * modifiedJulianDay * modifiedJulianDay)
                    + (+1139.34719487418 * modifiedJulianDay * modifiedJulianDay)
                    + (-34149488.355673 * modifiedJulianDay)
                    + (+409422822837.639);
            else if ((yearFraction >= 2021.79))
                retval = (0.000000000000926333089959963 * modifiedJulianDay * modifiedJulianDay * modifiedJulianDay * modifiedJulianDay * modifiedJulianDay)
                    + (-0.000000276351646101278 * modifiedJulianDay * modifiedJulianDay * modifiedJulianDay * modifiedJulianDay)
                    + (0.0329773938043592 * modifiedJulianDay * modifiedJulianDay * modifiedJulianDay)
                    + (-1967.61450470546 * modifiedJulianDay * modifiedJulianDay)
                    + (58699325.5212533 * modifiedJulianDay)
                    - 700463653286.072;
            else if ((yearFraction >= 2020.79))
                retval = (0.0000000000526391114738186 * modifiedJulianDay * modifiedJulianDay * modifiedJulianDay * modifiedJulianDay)
                    + (-0.0000124987447353606 * modifiedJulianDay * modifiedJulianDay * modifiedJulianDay)
                    + (1.1128953517557 * modifiedJulianDay * modifiedJulianDay)
                    + (-44041.1402447551 * modifiedJulianDay)
                    + 653571203.42671;
            else if ((yearFraction >= 2020.5))
                retval = (0.0000000000234066661113585 * modifiedJulianDay * modifiedJulianDay * modifiedJulianDay * modifiedJulianDay)
                    - (0.00000555556956413194 * modifiedJulianDay * modifiedJulianDay * modifiedJulianDay)
                    + (0.494477925757861 * modifiedJulianDay * modifiedJulianDay)
                    - (19560.53496991 * modifiedJulianDay)
                    + 290164271.563078;
            else if ((yearFraction >= 2018.3) & (yearFraction < double.MaxValue))
                retval = (0.00000161128367083801 * modifiedJulianDay * modifiedJulianDay)
                    + (-0.187474214389602 * modifiedJulianDay)
                    + 5522.26034874982;
            else if ((yearFraction >= 2018) & (yearFraction < double.MaxValue))
                retval = (0.0024855297566049 * yearFraction * yearFraction * yearFraction)
                    + (-15.0681141702439 * yearFraction * yearFraction)
                    + (30449.647471213 * yearFraction)
                    - 20511035.5077593;
            else if ((yearFraction >= 2017.0) & (yearFraction < double.MaxValue))
                retval = (0.02465436 * yearFraction * yearFraction)
                    + (-98.92626556 * yearFraction)
                    + 99301.85784308;
            else if ((yearFraction >= 2015.75) & (yearFraction < double.MaxValue))
                retval = (0.02002376 * yearFraction * yearFraction)
                    + (-80.27921003 * yearFraction)
                    + 80529.32;
            else if ((yearFraction >= 2011.75) & (yearFraction < 2015.75))
                retval = (0.00231189 * yearFraction * yearFraction)
                    + (-8.85231952 * yearFraction)
                    + 8518.54;
            else if ((yearFraction >= 2011.0) & (yearFraction < 2011.75))
            {
                // Following now superseded by above for 2012-16, this is left in for consistency with previous behaviour
                // Use polynomial given at http://sunearth.gsfc.nasa.gov/eclipse/SEcat5/deltatpoly.html as retrieved on 11-Jan-2009
                b = yearFraction - 2000.0;
                retval = 62.92 + (b * (0.32217 + (b * 0.005589)));
            }
            else
            {
                // Setup for pre 2011 calculations using Bob Denny's original code

                // /* Note, Stephenson and Morrison's table starts at the year 1630.
                // * The Chapronts' table does not agree with the Almanac prior to 1630.
                // * The actual accuracy decreases rapidly prior to 1780.
                // */
                // static short dt[] = {
                int[] dt = new[] { 12400, 11900, 11500, 11000, 10600, 10200, 9800, 9500, 9100, 8800, 8500, 8200, 7900, 7700, 7400, 7200, 7000, 6700, 6500, 6300, 6200, 6000, 5800, 5700, 5500, 5400, 5300, 5100, 5000, 4900, 4800, 4700, 4600, 4500, 4400, 4300, 4200, 4100, 4000, 3800, 3700, 3600, 3500, 3400, 3300, 3200, 3100, 3000, 2800, 2700, 2600, 2500, 2400, 2300, 2200, 2100, 2000, 1900, 1800, 1700, 1600, 1500, 1400, 1400, 1300, 1200, 1200, 1100, 1100, 1000, 1000, 1000, 900, 900, 900, 900, 900, 900, 900, 900, 900, 900, 900, 900, 900, 900, 900, 900, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1100, 1100, 1100, 1100, 1100, 1100, 1100, 1100, 1100, 1100, 1100, 1100, 1100, 1100, 1100, 1100, 1100, 1200, 1200, 1200, 1200, 1200, 1200, 1200, 1200, 1200, 1200, 1300, 1300, 1300, 1300, 1300, 1300, 1300, 1400, 1400, 1400, 1400, 1400, 1400, 1400, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1600, 1600, 1600, 1600, 1600, 1600, 1600, 1600, 1600, 1600, 1700, 1700, 1700, 1700, 1700, 1700, 1700, 1700, 1700, 1700, 1700, 1700, 1700, 1700, 1700, 1700, 1700, 1600, 1600, 1600, 1600, 1500, 1500, 1400, 1400, 1370, 1340, 1310, 1290, 1270, 1260, 1250, 1250, 1250, 1250, 1250, 1250, 1250, 1250, 1250, 1250, 1250, 1240, 1230, 1220, 1200, 1170, 1140, 1110, 1060, 1020, 960, 910, 860, 800, 750, 700, 660, 630, 600, 580, 570, 560, 560, 560, 570, 580, 590, 610, 620, 630, 650, 660, 680, 690, 710, 720, 730, 740, 750, 760, 770, 770, 780, 780, 788, 782, 754, 697, 640, 602, 541, 410, 292, 182, 161, 10, -102, -128, -269, -324, -364, -454, -471, -511, -540, -542, -520, -546, -546, -579, -563, -564, -580, -566, -587, -601, -619, -664, -644, -647, -609, -576, -466, -374, -272, -154, -2, 124, 264, 386, 537, 614, 775, 913, 1046, 1153, 1336, 1465, 1601, 1720, 1824, 1906, 2025, 2095, 2116, 2225, 2241, 2303, 2349, 2362, 2386, 2449, 2434, 2408, 2402, 2400, 2387, 2395, 2386, 2393, 2373, 2392, 2396, 2402, 2433, 2483, 2530, 2570, 2624, 2677, 2728, 2778, 2825, 2871, 2915, 2957, 2997, 3036, 3072, 3107, 3135, 3168, 3218, 3268, 3315, 3359, 3400, 3447, 3503, 3573, 3654, 3743, 3829, 3920, 4018, 4117, 4223, 4337, 4449, 4548, 4646, 4752, 4853, 4959, 5054, 5138, 5217, 5296, 5379, 5434, 5487, 5532, 5582, 5630, 5686, 5757, 5831, 5912, 5998, 6078, 6163, 6230, 6296, 6347, 6383, 6409, 6430, 6447, 6457, 6469, 6485, 6515, 6546, 6570, 6650, 6710 };
                // Change TABEND and TABSIZ if you add/delete anything

                // Calculate  DeltaT = ET - UT in seconds.  Describes the irregularities of the Earth rotation rate in the ET time scale.
                double p;
                int[] d = new int[7];
                int i, iy, k;

                // DATE RANGE <1620
                if ((yearFraction < TAB_START_1620))
                {
                    if ((yearFraction >= 948.0))
                    {
                        // /* Stephenson and Morrison, stated domain is 948 to 1600:
                        // * 25.5(centuries from 1800)^2 - 1.9159(centuries from 1955)^2
                        // */
                        b = 0.01 * (yearFraction - 2000.0);
                        retval = (23.58 * b + 100.3) * b + 101.6;
                    }
                    else
                    {
                        // /* Borkowski */
                        b = 0.01 * (yearFraction - 2000.0) + 3.75;
                        retval = 35.0 * b * b + 40.0;
                    }
                }
                else
                {

                    // DATE RANGE 1620 to 2011

                    // Besselian interpolation from tabulated values. See AA page K11.
                    // Index into the table.
                    p = Math.Floor(yearFraction);
                    iy = System.Convert.ToInt32(p - TAB_START_1620);            // // rbd - added cast
                                                                                // /* Zeroth order estimate is value at start of year */
                    retval = dt[iy];
                    k = iy + 1;
                    if ((k >= TAB_SIZE))
                        goto done; // /* No data, can't go on. */

                    // /* The fraction of tabulation interval */
                    p = yearFraction - p;

                    // /* First order interpolated value */
                    retval += p * (dt[k] - dt[iy]);
                    if (((iy - 1 < 0) | (iy + 2 >= TAB_SIZE)))
                        goto done; // /* can't do second differences */

                    // /* Make table of first differences */
                    k = iy - 2;
                    for (i = 0; i <= 4; i++)
                    {
                        if (((k < 0) | (k + 1 >= TAB_SIZE)))
                            d[i] = 0;
                        else
                            d[i] = dt[k + 1] - dt[k];
                        k += 1;
                    }
                    // /* Compute second differences */
                    for (i = 0; i <= 3; i++)
                        d[i] = d[i + 1] - d[i];
                    b = 0.25 * p * (p - 1.0);
                    retval += b * (d[1] + d[2]);
                    if ((iy + 2 >= TAB_SIZE))
                        goto done;

                    // /* Compute third differences */
                    for (i = 0; i <= 2; i++)
                        d[i] = d[i + 1] - d[i];
                    b = 2.0 * b / 3.0;
                    retval += (p - 0.5) * b * d[1];
                    if (((iy - 2 < 0) | (iy + 3 > TAB_SIZE)))
                        goto done;

                    // /* Compute fourth differences */
                    for (i = 0; i <= 1; i++)
                        d[i] = d[i + 1] - d[i];
                    b = 0.125 * b * (p + 1.0) * (p - 2.0);
                    retval += b * (d[0] + d[1]);

                // /* Astronomical Almanac table is corrected by adding the expression
                // *     -0.000091 (ndot + 26)(year-1955)^2  seconds
                // * to entries prior to 1955 (AA page K8), where ndot is the secular
                // * tidal term in the mean motion of the Moon.
                // *
                // * Entries after 1955 are referred to atomic time standards and
                // * are not affected by errors in Lunar or planetary theory.
                // */
                done:
                    ;
                    retval *= 0.01;
                    if ((yearFraction < 1955.0))
                    {
                        b = (yearFraction - 1955.0);
                        retval += -0.000091 * (-25.8 + 26.0) * b * b;
                    }
                }
            }

            return retval;
        }

        #endregion

        #region Sexagesimal Conversions

        /// <summary>
        /// Convert a coordinate expressed in sexagesimal degrees to a double value (degrees)
        /// </summary>
        /// <param name="DMSString">The coordinate expressed in sexagesimal notation (Degrees:Minutes:Seconds)</param>
        /// <exception cref="InvalidValueException">If the supplied coordinate contains an invalid character. Valid characters are: "+", "-", "0..9", ":" and "the decimal separator used by the current thread".</exception>
        /// <returns>The supplied coordinate as a double value (degrees)</returns>
        /// <remarks>
        /// <para>
        /// Only the first three numeric components are considered, the remainder are ignored. Left to right positionally, the components are interpreted as degrees, minutes, and seconds. 
        /// If only two components are present, they are assumed to be degrees and minutes, and if only one components is present, it is assumed to be degrees. Any components can have a fractional part. 
        /// </para>
        /// <para>Examples of valid input values: 127:27:45, 12:26, +345, -45:34:12</para>
        /// <para>Examples of valid input values in a locale where point is used as the decimal separator: 60:27:45.846, 12:1.349, +345.1840746, -45:34:12.422</para>
        /// <para>Examples of valid input values in a locale where comma is used as the decimal separator: 60:27:45,846, 12:1,349, +345,1840746, -45:34:12,422</para>
        /// </remarks>
        public static double DMSToDegrees(string DMSString)
        {
            double returnValue = 0.0; // Return value
            double sign; // Sign of the supplied value

            // Get the decimal separator character that is used in this locale
            string threadDecimalSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            // Create a validation pattern to ensure that invalid characters are detected and the input rejected if necessary
            string validatorPattern = string.Format(@"^[\+\-\d\:\{0}]+$", threadDecimalSeparator); //  Note that {0} is a string.Format positional parameter, not part of the regex pattern!

            // Create the validator for the supplied string argument and reject the string if necessary
            Regex validator = new Regex(validatorPattern);
            if (!validator.IsMatch(DMSString)) throw new InvalidValueException(string.Format("The supplied DMS string '{0}' contains an invalid character. (This application expects '{1}' as the decimal separator.)", DMSString, threadDecimalSeparator));

            DMSString = DMSString.Trim(); // Remove any leading or trailing spaces

            // Extract the sign of the coordinate if supplied
            if (DMSString.Substring(0, 1) == "-") sign = -1.0; // Found a leading minus sign so this is a negative value
            else sign = 1; // Assume that no sign or a leading plus sign indicates positive value

            // Based on an idea by Chris Rowland to use regular expressions to extract the number components
            // Create a match pattern containing the digits 0..9 and the thread's decimal separator to ensure that integer and real numbers are detected
            string regexPattern = string.Format(@"[\d\{0}]+", threadDecimalSeparator); // Create the regex pattern including the thread's decimal separator. Note that {0} is a string.Format positional parameter, not part of the regex pattern!
            Regex rx = new Regex(regexPattern); // Create a new regular expression based on the pattern

            // Find all number groups in the supplied sexagesimal string that match the pattern
            MatchCollection ms = rx.Matches(DMSString);
            if (ms.Count > 0) // We have at least a degrees value
            {
                returnValue = Convert.ToDouble(ms[0].Value); // Include the degrees value
                if (ms.Count > 1) // We have at least minutes and possibly seconds to deal with 
                {
                    returnValue += (Convert.ToDouble(ms[1].Value) / 60.0); // Include the minutes value
                    if (ms.Count > 2)
                    {
                        returnValue += (Convert.ToDouble(ms[2].Value) / 3600.0);// We have a seconds value so include this as well
                    }
                }
            }

            returnValue = sign * returnValue; // Apply the sign to create the correct return value
            return returnValue; // Return the decimal equivalent of the supplied sexagesimal string
        }

        /// <summary>
        /// Convert a coordinate expressed in sexagesimal degrees to a double value (hours)
        /// </summary>
        /// <param name="DMSString">The coordinate expressed in sexagesimal notation (Degrees:Minutes:Seconds)</param>
        /// <exception cref="InvalidValueException">If the supplied coordinate contains an invalid character. Valid characters are: "+", "-", "0..9", ":" and "the decimal separator used by the current thread".</exception>
        /// <returns>The supplied coordinate as a double value (hours)</returns>
        /// <remarks>
        /// <para>
        /// Only the first three numeric components are considered, the remainder are ignored. Left to right positionally, the components are interpreted as degrees, minutes, and seconds. 
        /// If only two components are present, they are assumed to be degrees and minutes, and if only one components is present, it is assumed to be degrees. Any components can have a fractional part. 
        /// </para>
        /// <para>Examples of valid input values: 127:27:45, 12:26, +345, -45:34:12</para>
        /// <para>Examples of valid input values in a locale where point is used as the decimal separator: 60:27:45.846, 12:1.349, +345.1840746, -45:34:12.422</para>
        /// <para>Examples of valid input values in a locale where comma is used as the decimal separator: 60:27:45,846, 12:1,349, +345,1840746, -45:34:12,422</para>
        /// </remarks>
        public static double DMSToHours(string DMSString)
        {
            return DMSToDegrees(DMSString) / 15.0;
        }

        /// <summary>
        /// Convert a coordinate expressed in sexagesimal hours to a double value (hours)
        /// </summary>
        /// <param name="HMSString">The coordinate expressed in sexagesimal notation (Hours:Minutes:Seconds)</param>
        /// <exception cref="InvalidValueException">If the supplied coordinate contains an invalid character. Valid characters are: "+", "-", "0..9", ":" and "the decimal separator used by the current thread".</exception>
        /// <returns>The supplied coordinate as a double value (hours)</returns>
        /// <remarks>
        /// <para>
        /// Only the first three numeric components are considered, the remainder are ignored. Left to right positionally, the components are interpreted as hours, minutes, and seconds. 
        /// If only two components are present, they are assumed to be hours and minutes, and if only one components is present, it is assumed to be hours. Any components can have a fractional part. 
        /// </para>
        /// <para>Examples of valid input values: 11:27:45, 12:26, +3, -5:34:12</para>
        /// <para>Examples of valid input values in a locale where point is used as the decimal separator: 6:27:45.846, 7:1.349, +8.1840746, -5:34:12.422</para>
        /// <para>Examples of valid input values in a locale where comma is used as the decimal separator: 6:27:45,846, 7:1,349, +8,1840746, -5:34:12,422</para>
        /// </remarks>
        public static double HMSToHours(string HMSString)
        {
            double returnValue = 0.0; // Return value
            double sign; // Sign of the supplied value

            // Get the decimal separator character that is used in this locale
            string threadDecimalSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            // Create a validation pattern to ensure that invalid characters are detected and the input rejected if necessary
            string validatorPattern = string.Format(@"^[\+\-\d\:\{0}]+$", threadDecimalSeparator); //  Note that {0} is a string.Format positional parameter, not part of the regex pattern!

            // Create the validator for the supplied string argument and reject the string if necessary
            Regex validator = new Regex(validatorPattern);
            if (!validator.IsMatch(HMSString)) throw new InvalidValueException(string.Format("The supplied HMS string '{0}' contains an invalid character. (This application expects '{1}' as the decimal separator.)", HMSString, threadDecimalSeparator));

            HMSString = HMSString.Trim(); // Remove any leading or trailing spaces

            // Extract the sign of the coordinate if supplied
            if (HMSString.Substring(0, 1) == "-") sign = -1.0; // Found a leading minus sign so this is a negative value
            else sign = 1; // Assume that no sign or a leading plus sign indicates positive value

            // Based on an idea by Chris Rowland to use regular expressions to extract the number components
            // Create a match pattern containing the digits 0..9 and the thread's decimal separator to ensure that integer and real numbers are detected
            string regexPattern = string.Format(@"[\d\{0}]+", threadDecimalSeparator); // Create the regex pattern including the thread's decimal separator. Note that {0} is a string.Format positional parameter, not part of the regex pattern!
            Regex rx = new Regex(regexPattern); // Create a new regular expression based on the pattern

            // Find all number groups in the supplied sexagesimal string that match the pattern
            MatchCollection ms = rx.Matches(HMSString);
            if (ms.Count > 0) // We have at least a degrees value
            {
                returnValue = Convert.ToDouble(ms[0].Value); // Include the degrees value
                if (ms.Count > 1) // We have at least minutes and possibly seconds to deal with 
                {
                    returnValue += (Convert.ToDouble(ms[1].Value) / 60.0); // Include the minutes value
                    if (ms.Count > 2)
                    {
                        returnValue += (Convert.ToDouble(ms[2].Value) / 3600.0);// We have a seconds value so include this as well
                    }
                }
            }

            returnValue = sign * returnValue; // Apply the sign to create the correct return value
            return returnValue; // Return the decimal equivalent of the supplied sexagesimal string        }
        }

        /// <summary>
        /// Convert a coordinate expressed in sexagesimal hours to a double value (degrees)
        /// </summary>
        /// <param name="HMS">The coordinate expressed in sexagesimal notation (Hours:Minutes:Seconds)</param>
        /// <exception cref="InvalidValueException">If the supplied coordinate contains an invalid character. Valid characters are: "+", "-", "0..9", ":" and "the decimal separator used by the current thread".</exception>
        /// <returns>The supplied coordinate as a double value (hours)</returns>
        /// <remarks>
        /// <para>
        /// Only the first three numeric components are considered, the remainder are ignored. Left to right positionally, the components are interpreted as hours, minutes, and seconds. 
        /// If only two components are present, they are assumed to be hours and minutes, and if only one components is present, it is assumed to be hours. Any components can have a fractional part. 
        /// </para>
        /// <para>Examples of valid input values: 11:27:45, 12:26, +3, -5:34:12</para>
        /// <para>Examples of valid input values in a locale where point is used as the decimal separator: 6:27:45.846, 7:1.349, +8.1840746, -5:34:12.422</para>
        /// <para>Examples of valid input values in a locale where comma is used as the decimal separator: 6:27:45,846, 7:1,349, +8,1840746, -5:34:12,422</para>
        /// </remarks>
        public static double HMSToDegrees(string HMS)
        {
            return HMSToHours(HMS) * 15.0;
        }

        #endregion

        #region DegreesToDMS Conversions

        // 
        // Convert a real number to sexagesimal whole, minutes, seconds. Allow
        // specifying the number of decimal digits on seconds. Called by
        // HoursToHMS below, which just has different default delimiters.
        // 
        /// <summary>
        /// Convert degrees to sexagesimal degrees, minutes and seconds with default delimiters DD° MM' SS" 
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <returns>Sexagesimal representation of degrees input value, degrees, minutes, and seconds</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single 
        /// characters.</para>
        /// <para>This overload is not available through COM, please use 
        /// "DegreesToDMS(ByVal Degrees As Double, ByVal DegDelim As String, ByVal MinDelim As String, ByVal SecDelim As String)"
        /// with suitable parameters to achieve this effect.</para>
        /// </remarks>
        public static string DegreesToDMS(double Degrees)
        {
            return DoubleToSexagesimalSeconds(Degrees, @":", @":", @"", 0);
        }

        /// <summary>
        ///  Convert degrees to sexagesimal degrees, minutes and seconds with specified second decimal places
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <param name="DegDelim">The delimiter string separating degrees and minutes </param>
        /// <param name="MinDelim">The delimiter string to append to the minutes part </param>
        /// <param name="SecDelim">The delimiter string to append to the seconds part</param>
        /// <param name="SecDecimalDigits">The number of digits after the decimal point on the seconds part </param>
        /// <returns>Sexagesimal representation of degrees input value, degrees, minutes, and seconds</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single 
        /// characters.</para>
        /// </remarks>
        public static string DegreesToDMS(double Degrees, string DegDelim, string MinDelim, string SecDelim, int SecDecimalDigits)
        {

            return DoubleToSexagesimalSeconds(Degrees, DegDelim, MinDelim, SecDelim, SecDecimalDigits);
        }

        #endregion

        #region DegreesToDM Conversions

        /// <summary>
        /// Convert degrees to sexagesimal degrees and minutes with default delimiters DD° MM'
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <returns>Sexagesimal representation of degrees input value, as degrees and minutes</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters.</para>
        /// <para>This overload is not available through COM, please use 
        /// "DegreesToDM(ByVal Degrees As Double, ByVal DegDelim As String, ByVal MinDelim As String, ByVal MinDecimalDigits As Integer)"
        /// with suitable parameters to achieve this effect.</para>
        /// </remarks>
        public static string DegreesToDM(double Degrees)
        {
            return DoubleToSexagesimalMinutes(Degrees, @":", @"", 0);
        }

        /// <summary>
        /// Convert degrees to sexagesimal degrees and minutes with the specified number of minute decimal places
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <param name="DegDelim">The delimiter string separating degrees </param>
        /// <param name="MinDelim">The delimiter string to append to the minutes </param>
        /// <param name="MinDecimalDigits">The number of digits after the decimal point on the minutes part </param>
        /// <returns>Sexagesimal representation of degrees input value, as degrees and minutes</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters.</para>
        /// </remarks>
        public static string DegreesToDM(double Degrees, string DegDelim, string MinDelim, int MinDecimalDigits)
        {
            return DoubleToSexagesimalMinutes(Degrees, DegDelim, MinDelim, MinDecimalDigits);
        }

        #endregion

        #region DegreesToHMS Conversions

        /// <summary>
        /// Convert degrees to sexagesimal hours, minutes, and seconds with default delimiters of HH:MM:SS
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <returns>Sexagesimal representation of degrees input value, as hours, minutes, and seconds</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself.</para>
        /// <para>This overload is not available through COM, please use 
        /// "DegreesToHMS(ByVal Degrees As Double, ByVal HrsDelim As String, ByVal MinDelim As String, ByVal SecDelim As String, ByVal SecDecimalDigits As Integer)"
        /// with suitable parameters to achieve this effect.</para>
        /// </remarks>
        public static string DegreesToHMS(double Degrees)
        {
            return DoubleToSexagesimalSeconds(Degrees / 15.0, @":", @":", @"", 0);
        }

        /// <summary>
        /// Convert degrees to sexagesimal hours, minutes, and seconds with the specified number of second decimal places
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <param name="HrsDelim">The delimiter string separating hours and minutes</param>
        /// <param name="MinDelim">The delimiter string separating minutes and seconds</param>
        /// <param name="SecDelim">The delimiter string to append to the seconds part </param>
        /// <param name="SecDecimalDigits">The number of digits after the decimal point on the seconds part </param>
        /// <returns>Sexagesimal representation of degrees input value, as hours, minutes, and seconds</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters. </para>
        /// </remarks>
        public static string DegreesToHMS(double Degrees, string HrsDelim, string MinDelim, string SecDelim, int SecDecimalDigits)
        {
            return DoubleToSexagesimalSeconds(Degrees / 15.0, HrsDelim, MinDelim, SecDelim, SecDecimalDigits);
        }

        #endregion

        #region DegreesToHM Conversions

        /// <summary>
        /// Convert degrees to sexagesimal hours and minutes with default delimiters HH:MM
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <returns>Sexagesimal representation of degrees input value as hours and minutes</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters</para>
        /// <para>This overload is not available through COM, please use 
        /// "DegreesToHM(ByVal Degrees As Double, ByVal HrsDelim As String, ByVal MinDelim As String, ByVal MinDecimalDigits As Integer)"
        /// with suitable parameters to achieve this effect.</para>
        /// </remarks>
        public static string DegreesToHM(double Degrees)
        {
            return DoubleToSexagesimalMinutes(Degrees / 15.0, @":", @"", 0);
        }

        /// <summary>
        /// Convert degrees to sexagesimal hours and minutes with supplied number of minute decimal places
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <param name="HrsDelim">The delimiter string separating hours and minutes</param>
        /// <param name="MinDelim">The delimiter string to append to the minutes part</param>
        /// <param name="MinDecimalDigits">Number of minutes decimal places</param>
        /// <returns>Sexagesimal representation of degrees input value as hours and minutes</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters</para>
        /// </remarks>
        public static string DegreesToHM(double Degrees, string HrsDelim, string MinDelim, int MinDecimalDigits)
        {
            return DoubleToSexagesimalMinutes(Degrees / 15.0, HrsDelim, MinDelim, MinDecimalDigits);
        }

        #endregion

        #region HoursToHMS Conversions

        /// <summary>
        /// Convert hours to sexagesimal hours, minutes, and seconds with default delimiters HH:MM:SS
        /// </summary>
        /// <param name="Hours">The hours value to convert</param>
        /// <returns>Sexagesimal representation of hours input value, hours, minutes and seconds</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters.</para>
        /// <para>This overload is not available through COM, please use 
        /// "HoursToHMS(ByVal Hours As Double, ByVal HrsDelim As String, ByVal MinDelim As String, ByVal SecDelim As String, ByVal SecDecimalDigits As Integer)"
        /// with suitable parameters to achieve this effect.</para>
        /// </remarks>
        public static string HoursToHMS(double Hours)
        {
            return DoubleToSexagesimalSeconds(Hours, @":", @":", @"", 0);
        }

        /// <summary>
        /// Convert hours to sexagesimal hours, minutes, and seconds with specified number of second decimal places
        /// </summary>
        /// <param name="Hours">The hours value to convert</param>
        /// <param name="HrsDelim">The delimiter string separating hours and minutes </param>
        /// <param name="MinDelim">The delimiter string separating minutes and seconds </param>
        /// <param name="SecDelim">The delimiter string to append to the seconds part </param>
        /// <param name="SecDecimalDigits">The number of digits after the decimal point on the seconds part </param>
        /// <returns>Sexagesimal representation of hours input value, hours, minutes and seconds</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters.</para>
        /// </remarks>
        public static string HoursToHMS(double Hours, string HrsDelim, string MinDelim, string SecDelim, int SecDecimalDigits)
        {
            return DoubleToSexagesimalSeconds(Hours, HrsDelim, MinDelim, SecDelim, SecDecimalDigits);
        }

        #endregion

        #region HoursToHM Conversions

        /// <summary>
        /// Convert hours to sexagesimal hours and minutes with default delimiters HH:MM
        /// </summary>
        /// <param name="Hours">The hours value to convert</param>
        /// <returns>Sexagesimal representation of hours input value as hours and minutes</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters.</para>
        /// <para>This overload is not available through COM, please use 
        /// "HoursToHM(ByVal Hours As Double, ByVal HrsDelim As String, ByVal MinDelim As String, ByVal MinDecimalDigits As Integer)"
        /// with an suitable parameters to achieve this effect.</para>
        /// </remarks>
        public static string HoursToHM(double Hours)
        {
            return DoubleToSexagesimalMinutes(Hours, @":", @"", 0);
        }

        /// <summary>
        /// Convert hours to sexagesimal hours and minutes with supplied number of minute decimal places
        /// </summary>
        /// <param name="Hours">The hours value to convert</param>
        /// <param name="HrsDelim">The delimiter string separating hours </param>
        /// <param name="MinDelim">The delimiter string to append to the minutes part </param>
        /// <param name="MinDecimalDigits">The number of digits after the decimal point on the minutes part </param>
        /// <returns>Sexagesimal representation of hours input value as hours and minutes</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters.</para>
        /// </remarks>
        public static string HoursToHM(double Hours, string HrsDelim, string MinDelim, int MinDecimalDigits)
        {
            return DoubleToSexagesimalMinutes(Hours, HrsDelim, MinDelim, MinDecimalDigits);
        }

        #endregion

        #region Julian Date Functions

        /// <summary>
        /// Convert a Julian date to a UTC DateTime value on the Gregorian time scale
        /// </summary>
        /// <param name="JD">Julian date</param>
        /// <returns>UTC DateTime value corresponding to the supplied Julian date</returns>
        /// <exception cref="InvalidValueException">If the Julian date corresponds to a date before 15th October 1582 when the Gregorian calendar was introduced.</exception>
        /// <remarks>
        /// <para>Julian dates are always in UTC.</para>
        /// <para>The algorithm is from the Explanatory Supplement to the Astronomical Almanac 3rd Edition 2013 edited by Urban and Seidelmann pages 617-619 and has been validated against
        /// the USNO Julian date calculator at https://aa.usno.navy.mil/data/docs/JulianDate.php </para>
        /// </remarks>
        public static DateTime JulianDateToDateTime(double JD)
        {
            // The algorithm employed here is taken from the Explanatory Supplement to the USNO/HMNAO Astronomical Almanac 3rd Edition 2013 edited by Urban and Seidelmann, pages 617 - 619.
            // This implementation has been validated against the USNO Julian date calculator at https://aa.usno.navy.mil/data/docs/JulianDate.php 

            // Validate the incoming Julian date because it is not possible for a DateTime value to represent a date/time earlier that 00:00:00 1st January 0001
            if (JD < Constants.JULIAN_DAY_WHEN_GREGORIAN_CALENDAR_WAS_INTRODUCED)
                throw new InvalidValueException($"JulianDateToDateTime: The supplied Julian date {JD} precedes introduction of the Gregorian calendar on 15th October 1582.");

            // Defined constants for the Gregorian calendar, taken from the Explanatory Supplement to the Astronomical Almanac.
            const int y = 4716;
            const int j = 1401;
            const int m = 2;
            const int n = 12;
            const int r = 4;
            const int p = 1461;
            const int v = 3;
            const int u = 5;
            const int s = 153;
            const int w = 2;
            const int B = 274277;
            const int C = -38;

            // Calculate the Gregorian year, month and day corresponding to the supplied Julian date using the Explanatory Supplement formulae

            int JDInt = (int)Math.Floor(JD); // Extract the integer part of the supplied Julian date
            decimal JDFraction = (decimal)JD % 1.0M; // Extract the fractional part of the supplied Julian date, using a decimal type for the fractional part to increase precision

            // Correct for the half day offset inherent in the Julian day system
            if (JDFraction >= 0.5M) // We are in the following Gregorian day even though we are still in the same Julian day!
            {
                JDInt += 1; // Increase the Julian day number by one in order to calculate the correct Gregorian day
                JDFraction -= 1.0M; // Decrease the fractional part by 1.0 to ensure that the day fraction is always less that 1.0 in the calculation below
            }

            int f = JDInt + j; // Equation 1
            f = f + (((4 * JDInt + B) / 146097) * 3) / 4 + C; // Equation 1a
            int e = r * f + v; // Equation 2
            int g = (e % p) / r; // Equation 3
            int h = u * g + w; // Equation 4
            int D = ((h % s) / u) + 1; // Equation 5 - Day number
            int M = (((h / s) + m) % n) + 1; // Equation 6 - Month number
            int Y = e / p - y + ((n + m - M) / n); // Equation 7 - Year number

            // Calculate the Julian date time components: hours, minutes, seconds and milliseconds
            double dayFraction = (double)JDFraction + 0.5; // Half a day is added here because a Julian day starts at 12:00 mid-day rather than 00:00 midnight. 
            int H = (int)(dayFraction * 24.0);

            double hourFraction = dayFraction * 24.0 - H;
            int MIN = (int)(hourFraction * 60.0);

            double minuteFraction = hourFraction * 60 - MIN;
            int S = (int)(minuteFraction * 60.0);

            double secondFraction = minuteFraction * 60 - S;
            int MS = (int)(secondFraction * 1000.0);

            return new DateTime(Y, M, D, H, MIN, S, MS, DateTimeKind.Utc);
        }


        /// <summary>
        /// Calculate the Julian date from a provided DateTime value. The value is assumed to be a UTC time
        /// </summary>
        /// <param name="ObservationDateTime">DateTime in UTC</param>
        /// <returns>Julian date</returns>
        /// <remarks>Julian dates should always be in UTC </remarks>
        public static double JulianDateFromDateTime(DateTime ObservationDateTime)
        {
            double jd1 = default, jd2 = default;
            int rc;

            // Revised to use SOFA to calculate the Julian date
            rc = Sofa.Dtf2d("UTC", ObservationDateTime.Year, ObservationDateTime.Month, ObservationDateTime.Day, ObservationDateTime.Hour, ObservationDateTime.Minute, ObservationDateTime.Second + ObservationDateTime.Millisecond / 1000.0d, ref jd1, ref jd2);
            if (rc != 0)
                throw new HelperException($"UTCJulianDate- Bad return code from Sofa.Dtf2d: {rc} for date: {ObservationDateTime.ToString("dddd dd MMMM yyyy HH:mm:ss.fff")}");

            return jd1 + jd2;


        }

        /// <summary>
        /// Current Julian date based on the UTC time scale
        /// </summary>
        /// <returns>Current Julian date on the UTC time scale</returns>
        /// <remarks></remarks>
        public static double JulianDateUtc
        {
            get
            {
                return JulianDateFromDateTime(DateTime.UtcNow);
            }
        }

        #endregion

        #region ObservingConditions Property Conversions

        /// <summary>
        /// Convert from one set of speed / temperature / pressure rain rate units to another
        /// </summary>
        /// <param name="InputValue">Value to convert</param>
        /// <param name="FromUnits">Integer value from the Units enum indicating the value's current units</param>
        /// <param name="ToUnits">Integer value from the Units enum indicating the units to which the input value should be converted</param>
        /// <returns>Input value expressed in the new units</returns>
        /// <exception cref="InvalidOperationException">When the specified from and to units can not refer to the same value. e.g. attempting to convert miles per hour to degrees Celsius</exception>
        /// <remarks>
        /// <para>Conversions available:</para>
        /// <list type="bullet">
        /// <item>metres per second &lt;==&gt; miles per hour &lt;==&gt; knots</item>
        /// <item>Celsius &lt;==&gt; Fahrenheit &lt;==&gt; Kelvin</item>
        /// <item>hecto Pascals (hPa) &lt;==&gt; milli bar(mbar) &lt;==&gt; mm of mercury &lt;==&gt; inches of mercury</item>
        /// <item>mm per hour &lt;==&gt; inches per hour</item>
        /// </list>
        /// <para>Knots conversions use the international nautical mile definition (1 nautical mile = 1852m) rather than the original UK or US Admiralty definitions.</para>
        /// <para>For convenience, milli bar and hecto Pascals are shown as separate units even though they have numerically identical values and there is a 1:1 conversion between them.</para>
        /// </remarks>
        public static double ConvertUnits(double InputValue, Unit FromUnits, Unit ToUnits)
        {
            double intermediateValue, finalValue;

            if ((FromUnits >= Unit.metresPerSecond) & (FromUnits <= Unit.knots) & (ToUnits >= Unit.metresPerSecond) & (ToUnits <= Unit.knots))
            {
                // First convert the input to metres per second
                switch (FromUnits)
                {
                    case Unit.metresPerSecond:
                        {
                            intermediateValue = InputValue;
                            break;
                        }

                    case Unit.milesPerHour:
                        {
                            intermediateValue = InputValue * 0.44704;
                            break;
                        }

                    case Unit.knots:
                        {
                            intermediateValue = InputValue * 0.514444444;
                            break;
                        }

                    default:
                        {
                            throw new InvalidValueException("Unknown \"From\" speed units: " + FromUnits.ToString());
                        }
                }

                // Now convert metres per second to the output value
                switch (ToUnits)
                {
                    case Unit.metresPerSecond:
                        {
                            finalValue = intermediateValue;
                            break;
                        }

                    case Unit.milesPerHour:
                        {
                            finalValue = intermediateValue / 0.44704;
                            break;
                        }

                    case Unit.knots:
                        {
                            finalValue = intermediateValue / 0.514444444;
                            break;
                        }

                    default:
                        {
                            throw new InvalidValueException("Unknown \"To\" speed units: " + ToUnits.ToString());
                        }
                }

                return finalValue;
            }
            else if ((FromUnits >= Unit.degreesCelsius) & (FromUnits <= Unit.degreesKelvin) & (ToUnits >= Unit.degreesCelsius) & (ToUnits <= Unit.degreesKelvin))
            {

                // First convert the input to degrees K
                switch (FromUnits)
                {
                    case Unit.degreesCelsius:
                        {
                            intermediateValue = InputValue - Constants.ABSOLUTE_ZERO_CELSIUS;
                            break;
                        }

                    case Unit.degreesFarenheit:
                        {
                            intermediateValue = ((InputValue + 459.67) * 5.0) / 9.0;
                            break;
                        }

                    case Unit.degreesKelvin:
                        {
                            intermediateValue = InputValue;
                            break;
                        }

                    default:
                        {
                            throw new InvalidValueException("Unknown \"From\" temperature units: " + FromUnits.ToString());
                        }
                }

                // Now convert degrees K to the output value
                switch (ToUnits)
                {
                    case Unit.degreesCelsius:
                        {
                            finalValue = intermediateValue + Constants.ABSOLUTE_ZERO_CELSIUS;
                            break;
                        }

                    case Unit.degreesFarenheit:
                        {
                            finalValue = ((intermediateValue * 9.0) / 5.0) - 459.67;
                            break;
                        }

                    case Unit.degreesKelvin:
                        {
                            finalValue = intermediateValue;
                            break;
                        }

                    default:
                        {
                            throw new InvalidValueException("Unknown \"To\" temperature units: " + ToUnits.ToString());
                        }
                }

                return finalValue;
            }
            else if ((FromUnits >= Unit.hPa) & (FromUnits <= Unit.inHg) & (ToUnits >= Unit.hPa) & (ToUnits <= Unit.inHg))
            {
                // First convert the input to hPa
                switch (FromUnits)
                {
                    case Unit.hPa:
                        {
                            intermediateValue = InputValue;
                            break;
                        }

                    case Unit.mBar:
                        {
                            intermediateValue = InputValue;
                            break;
                        }

                    case Unit.mmHg:
                        {
                            intermediateValue = InputValue * 1.33322368;
                            break;
                        }

                    case Unit.inHg:
                        {
                            intermediateValue = InputValue * 33.8638816;
                            break;
                        }

                    default:
                        {
                            throw new InvalidValueException("Unknown \"From\" pressure units: " + FromUnits.ToString());
                        }
                }

                // Now convert hPa to the output value
                switch (ToUnits)
                {
                    case Unit.hPa:
                        {
                            finalValue = intermediateValue;
                            break;
                        }

                    case Unit.mBar:
                        {
                            finalValue = intermediateValue;
                            break;
                        }

                    case Unit.mmHg:
                        {
                            finalValue = intermediateValue / 1.33322368;
                            break;
                        }

                    case Unit.inHg:
                        {
                            finalValue = intermediateValue / 33.8638816;
                            break;
                        }

                    default:
                        {
                            throw new InvalidValueException("Unknown \"To\" pressure units: " + ToUnits.ToString());
                        }
                }

                return finalValue;
            }
            else if ((FromUnits >= Unit.mmPerHour) & (FromUnits <= Unit.inPerHour) & (ToUnits >= Unit.mmPerHour) & (ToUnits <= Unit.inPerHour))
            {
                // First convert the input to mm
                switch (FromUnits)
                {
                    case Unit.mmPerHour:
                        {
                            intermediateValue = InputValue;
                            break;
                        }

                    case Unit.inPerHour:
                        {
                            intermediateValue = InputValue * 25.4;
                            break;
                        }

                    default:
                        {
                            throw new InvalidValueException("Unknown \"From\" rain rate units: " + FromUnits.ToString());
                        }
                }

                // Now convert mm to the output value
                switch (ToUnits)
                {
                    case Unit.mmPerHour:
                        {
                            finalValue = intermediateValue;
                            break;
                        }

                    case Unit.inPerHour:
                        {
                            finalValue = intermediateValue / 25.4;
                            break;
                        }

                    default:
                        {
                            throw new InvalidValueException("Unknown \"To\" rain rate units: " + ToUnits.ToString());
                        }
                }

                return finalValue;
            }
            else
                throw new ASCOM.InvalidOperationException("From and to units are not of the same type. From: " + FromUnits.ToString() + ", To: " + ToUnits.ToString());
        }

        /// <summary>
        /// Calculate the dew point (°Celsius) given the ambient temperature (°Celsius) and relative humidity (%)
        /// </summary>
        /// <param name="RelativeHumidity">Relative humidity expressed in percent (0.0 .. 100.0)</param>
        /// <param name="AmbientTemperature">Ambient temperature (°Celsius)</param>
        /// <returns>Dew point (°Celsius)</returns>
        /// <exception cref="InvalidOperationException">When relative humidity &lt; 0.0% or &gt; 100.0%></exception>
        /// <exception cref="InvalidOperationException">When ambient temperature &lt; absolute zero or &gt; 100.0C></exception>
        ///  <remarks>'Calculation uses Vaisala formula for water vapour saturation pressure and is accurate to 0.083% over -20C - +50°C
        /// <para>http://www.vaisala.com/Vaisala%20Documents/Application%20notes/Humidity_Conversion_Formulas_B210973EN-F.pdf </para>
        /// </remarks>
        public static double Humidity2DewPoint(double RelativeHumidity, double AmbientTemperature)
        {
            // Formulae taken from Vaisala: http://www.vaisala.com/Vaisala%20Documents/Application%20notes/Humidity_Conversion_Formulas_B210973EN-F.pdf 
            double Pws, Pw, Td;

            // Constants from Vaisala document
            const double A = 6.116441;
            const double m = 7.591386;
            const double Tn = 240.7263;

            // Validate input values
            if ((RelativeHumidity < 0.0) | (RelativeHumidity > 100.0))
                throw new InvalidValueException("Humidity2DewPoint - Relative humidity is < 0.0% or > 100.0%: " + RelativeHumidity.ToString());
            if ((AmbientTemperature < Constants.ABSOLUTE_ZERO_CELSIUS) | (AmbientTemperature > 100.0))
                throw new InvalidValueException("Humidity2DewPoint - Ambient temperature is < " + Constants.ABSOLUTE_ZERO_CELSIUS + "C or > 100.0C: " + AmbientTemperature.ToString());

            Pws = A * Math.Pow(10.0, m * AmbientTemperature / (AmbientTemperature + Tn)); // Calculate water vapour saturation pressure, Pws, from Vaisala formula (6) - In hPa
            Pw = Pws * RelativeHumidity / 100.0; // Calculate measured vapour pressure, Pw
            Td = Tn / ((m / Math.Log10(Pw / A)) - 1.0); // Finally, calculate dew-point in °C

            return Td;
        }

        /// <summary>
        /// Calculate the relative humidity (%) given the ambient temperature (°Celsius) and dew point (°Celsius)
        /// </summary>
        /// <param name="DewPoint">Dewpoint in (°Celsius)</param>
        /// <param name="AmbientTemperature">Ambient temperature (°Celsius)</param>
        /// <returns>Humidity expressed in percent (0.0 .. 100.0)</returns>
        /// <exception cref="InvalidOperationException">When dew point &lt; absolute zero or &gt; 100.0C></exception>
        /// <exception cref="InvalidOperationException">When ambient temperature &lt; absolute zero or &gt; 100.0C></exception>
        /// <remarks>'Calculation uses the Vaisala formula for water vapour saturation pressure and is accurate to 0.083% over -20C - +50°C
        /// <para>http://www.vaisala.com/Vaisala%20Documents/Application%20notes/Humidity_Conversion_Formulas_B210973EN-F.pdf </para>
        /// </remarks>
        public static double DewPoint2Humidity(double DewPoint, double AmbientTemperature)
        {
            // Formulae taken from Vaisala: http://www.vaisala.com/Vaisala%20Documents/Application%20notes/Humidity_Conversion_Formulas_B210973EN-F.pdf 
            double RH;

            // Constants from Vaisala document
            const double m = 7.591386;
            const double Tn = 240.7263;

            // Validate input values
            if ((DewPoint < Constants.ABSOLUTE_ZERO_CELSIUS) | (DewPoint > 100.0))
                throw new InvalidValueException("DewPoint2Humidity - Dew point is < " + Constants.ABSOLUTE_ZERO_CELSIUS + "C or > 100.0C: " + DewPoint.ToString());
            if ((AmbientTemperature < Constants.ABSOLUTE_ZERO_CELSIUS) | (AmbientTemperature > 100.0))
                throw new InvalidValueException("DewPoint2Humidity - Ambient temperature is < " + Constants.ABSOLUTE_ZERO_CELSIUS + "C or > 100.0C: " + AmbientTemperature.ToString());

            RH = 100.0 * Math.Pow(10.0, m * ((DewPoint / (DewPoint + Tn)) - (AmbientTemperature / (AmbientTemperature + Tn))));

            return RH;
        }

        /// <summary>
        /// Convert atmospheric pressure from one altitude above mean sea level to another
        /// </summary>
        /// <param name="Pressure">Measured pressure in hPa (mBar) at the "From" altitude</param>
        /// <param name="FromAltitudeAboveMeanSeaLevel">"Altitude at which the input pressure was measured (metres)</param>
        /// <param name="ToAltitudeAboveMeanSeaLevel">Altitude to which the pressure is to be converted (metres)</param>
        /// <returns>Pressure in hPa at the "To" altitude</returns>
        /// <remarks>Uses the equation: p = p0 * (1.0 - 2.25577E-05 h)^5.25588</remarks>
        public static double ConvertPressure(double Pressure, double FromAltitudeAboveMeanSeaLevel, double ToAltitudeAboveMeanSeaLevel)
        {
            // Convert supplied pressure to sea level then convert again to the required altitude using this equation: p = p0 (1 - 2.25577 10-5 h)5.25588

            double SeaLevelPressure, ActualPressure;

            SeaLevelPressure = Pressure / Math.Pow(1.0 - 0.0000225577 * FromAltitudeAboveMeanSeaLevel, 5.25588);
            ActualPressure = SeaLevelPressure * Math.Pow(1.0 - 0.0000225577 * ToAltitudeAboveMeanSeaLevel, 5.25588);

            return ActualPressure;
        }

        #endregion

        #region Coordinate Validation and Management Functions

        /// <summary>
        /// Flexible routine to range a number into a given range between a lower and an higher bound.
        /// </summary>
        /// <param name="Value">Value to be ranged</param>
        /// <param name="LowerBound">Lowest value of the range</param>
        /// <param name="LowerEqual">Boolean flag indicating whether the ranged value can have the lower bound value</param>
        /// <param name="UpperBound">Highest value of the range</param>
        /// <param name="UpperEqual">Boolean flag indicating whether the ranged value can have the upper bound value</param>
        /// <returns>The ranged number as a double</returns>
        /// <exception cref="InvalidOperationException">Thrown if the lower bound is greater than the upper bound.</exception>
        /// <exception cref="InvalidOperationException">Thrown if LowerEqual and UpperEqual are both false and the ranged value equals
        /// one of these values. This is impossible to handle as the algorithm will always violate one of the rules!</exception>
        /// <remarks>
        /// UpperEqual and LowerEqual switches control whether the ranged value can be equal to either the upper and lower bounds. So, 
        /// to range an hour angle into the range 0 to 23.999999.. hours, use this call: 
        /// <code>RangedValue = Range(InputValue, 0.0, True, 24.0, False)</code>
        /// <para>The input value will be returned in the range where 0.0 is an allowable value and 24.0 is not i.e. in the range 0..23.999999..</para>
        /// <para>It is not permissible for both LowerEqual and UpperEqual to be false because it will not be possible to return a value that is exactly equal 
        /// to either lower or upper bounds. An exception is thrown if this scenario is requested.</para>
        /// </remarks>
        public static double Range(double Value, double LowerBound, bool LowerEqual, double UpperBound, bool UpperEqual)
        {
            double ModuloValue;
            if (LowerBound >= UpperBound)
                throw new ASCOM.InvalidValueException("Range - LowerBound is >= UpperBound LowerBound must be less than UpperBound");

            ModuloValue = UpperBound - LowerBound;

            if (LowerEqual)
            {
                if (UpperEqual)
                {
                    do
                    {
                        if (Value < LowerBound)
                            Value += ModuloValue;
                        if (Value > UpperBound)
                            Value -= ModuloValue;
                    }
                    while (!(Value >= LowerBound) & (Value <= UpperBound));
                }
                else
                    do
                    {
                        if (Value < LowerBound)
                            Value += ModuloValue;
                        if (Value >= UpperBound)
                            Value -= ModuloValue;
                    }
                    while (!(Value >= LowerBound) & (Value < UpperBound));
            }
            else if (UpperEqual)
            {
                do
                {
                    if (Value <= LowerBound)
                        Value += ModuloValue;
                    if (Value > UpperBound)
                        Value -= ModuloValue;
                }
                while (!(Value > LowerBound) & (Value <= UpperBound));
            }
            else
            {
                if ((Value == LowerBound))
                    throw new InvalidValueException("Range - The supplied value equals the LowerBound. This can not be ranged when LowerEqual and UpperEqual are both false.");
                if ((Value == UpperBound))
                    throw new InvalidValueException("Range - The supplied value equals the UpperBound. This can not be ranged when LowerEqual and UpperEqual are both false.");
                do
                {
                    if (Value <= LowerBound)
                        Value += ModuloValue;
                    if (Value >= UpperBound)
                        Value -= ModuloValue;
                }
                while (!(Value > LowerBound) & (Value < UpperBound));
            }
            return Value;
        }

        /// <summary>
        /// Conditions an hour angle to be in the range -12.0 to +12.0 by adding or subtracting 24.0 hours
        /// </summary>
        /// <param name="HA">Hour angle to condition</param>
        /// <returns>Hour angle in the range -12.0 to +12.0</returns>
        /// <remarks></remarks>
        public static double ConditionHA(double HA)
        {
            double ReturnValue;

            ReturnValue = Range(HA, -12.0, true, +12.0, true);

            return ReturnValue;
        }

        /// <summary>
        /// Conditions a Right Ascension value to be in the range 0 to 23.999999.. hours 
        /// </summary>
        /// <param name="RA">Right ascension to be conditioned</param>
        /// <returns>Right ascension in the range 0 to 23.999999...</returns>
        /// <remarks></remarks>
        public static double ConditionRA(double RA)
        {
            double ReturnValue;

            ReturnValue = Range(RA, 0.0, true, 24.0, false);

            return ReturnValue;
        }

        #endregion

        #region Private support code

        private static string DoubleToSexagesimalSeconds(double Units, string DegDelim, string MinDelim, string SecDelim, int SecDecimalDigits)
        {
            string wholeUnits, wholeMinutes, seconds, secondsFormatString;
            bool inputIsNegative;

            // Convert the input value to a positive number if required and store the sign
            if (Units < 0.0)
            {
                Units = -Units;
                inputIsNegative = true;
            }
            else inputIsNegative = false;

            // Extract the number of whole units and save the remainder
            wholeUnits = Math.Floor(Units).ToString().PadLeft(2, '0');
            double remainderInMinutes = (Units - Convert.ToDouble(wholeUnits)) * 60.0; // Remainder in minutes

            // Extract the number of whole minutes and save the remainder
            wholeMinutes = Math.Floor(remainderInMinutes).ToString().PadLeft(2, '0');// Integral minutes
            double remainderInSeconds = (remainderInMinutes - System.Convert.ToDouble(wholeMinutes)) * 60.0; // Remainder in seconds

            if (SecDecimalDigits == 0) secondsFormatString = "00"; // No decimal point or decimal digits
            else secondsFormatString = "00." + new String('0', SecDecimalDigits); // Format$ string of form 00.0000

            seconds = remainderInSeconds.ToString(secondsFormatString); // Format seconds with requested number of decimal digits

            // Check to see whether rounding has pushed the number of whole seconds or minutes to 60
            if (seconds.Substring(0, 2) == "60") // The rounded seconds value is 60 so we need to add one to the minutes count and make the seconds 0
            {
                seconds = 0.0.ToString(secondsFormatString); // Seconds are 0.0 formatted as required
                wholeMinutes = (Convert.ToInt32(wholeMinutes) + 1).ToString("00"); // Add one to the to the minutes count
                if (wholeMinutes == "60")// The minutes value is 60 so we need to add one to the units count and make the minutes 0
                {
                    wholeMinutes = "00"; // Minutes are 0.0
                    wholeUnits = (Convert.ToInt32(wholeUnits) + 1).ToString("00"); // Add one to the units count
                }
            }

            // Create the full formatted string from the units, minute and seconds parts and add a leading negative sign if required
            string returnValue = wholeUnits + DegDelim + wholeMinutes + MinDelim + seconds + SecDelim;
            if (inputIsNegative) returnValue = $"-{returnValue}";

            return returnValue;
        }

        private static string DoubleToSexagesimalMinutes(double Units, string DegDelim, string MinDelim, int MinDecimalDigits)
        {
            string wholeUnits, minutes, minutesFormatString;
            bool inputIsNegative;

            // Convert the input value to a positive number if required and store the sign
            if (Units < 0.0)
            {
                Units = -Units;
                inputIsNegative = true;
            }
            else inputIsNegative = false;

            // Extract the number of whole units and save the remainder
            wholeUnits = Math.Floor(Units).ToString().PadLeft(2, '0');
            double remainderInMinutes = (Units - Convert.ToDouble(wholeUnits)) * 60.0; // Remainder in minutes

            if (MinDecimalDigits == 0) minutesFormatString = "00"; // No decimal point or decimal digits
            else minutesFormatString = "00." + new String('0', MinDecimalDigits); // Format$ string of form 00.0000

            minutes = remainderInMinutes.ToString(minutesFormatString); // Format minutes with requested number of decimal digits

            // Check to see whether rounding has pushed the number of whole minutes to 60
            if (minutes.Substring(0, 2) == "60") // The rounded minutes value is 60 so we need to add one to the units count and make the minutes 0
            {
                minutes = 0.0.ToString(minutesFormatString); // Minutes are 0.0 formatted as required
                wholeUnits = (Convert.ToInt32(wholeUnits) + 1).ToString("00"); // Add one to the to the units count
            }

            // Create the full formatted string from the units, minutes and seconds parts and add a leading negative sign if required
            string returnValue = wholeUnits + DegDelim + minutes + MinDelim;
            if (inputIsNegative) returnValue = $"-{returnValue}";

            return returnValue;
        }

        /// <summary>
        /// Returns the altitude of the body given the input parameters
        /// </summary>
        /// <param name="TypeOfEvent">Type of event to be calaculated</param>
        /// <param name="JD">UTC Julian date</param>
        /// <param name="Hour">Hour of Julian day</param>
        /// <param name="Latitude">Site Latitude</param>
        /// <param name="Longitude">Site Longitude</param>
        /// <returns>The altitude of the body (degrees)</returns>
        /// <remarks></remarks>
        private static BodyInfo BodyAltitude(EventType TypeOfEvent, double JD, double Hour, double Latitude, double Longitude)
        {
            double Instant, Tau, Gmst = default, deltaT;
            short rc;
            var Obj3 = new Object3();
            var Location = new OnSurface();
            var Cat = new CatEntry3();
            var SkyPosition = new SkyPos();
            var Obs = new Observer();
            var Retval = new BodyInfo();

            Instant = JD + Hour / 24.0d; // Add the hour to the whole Julian day number
            // DeltaT = DeltaTCalc(JD)
            deltaT = DeltaT(JD);

            switch (TypeOfEvent)
            {
                case EventType.MercuryRiseSet:
                    {
                        Obj3.Name = "Mercury";
                        Obj3.Number = Body.Mercury;
                        break;
                    }
                case EventType.VenusRiseSet:
                    {
                        Obj3.Name = "Venus";
                        Obj3.Number = Body.Venus;
                        break;
                    }
                case EventType.MarsRiseSet:
                    {
                        Obj3.Name = "Mars";
                        Obj3.Number = Body.Mars;
                        break;
                    }
                case EventType.JupiterRiseSet:
                    {
                        Obj3.Name = "Jupiter";
                        Obj3.Number = Body.Jupiter;
                        break;
                    }
                case EventType.SaturnRiseSet:
                    {
                        Obj3.Name = "Saturn";
                        Obj3.Number = Body.Saturn;
                        break;
                    }
                case EventType.UranusRiseSet:
                    {
                        Obj3.Name = "Uranus";
                        Obj3.Number = Body.Uranus;
                        break;
                    }
                case EventType.NeptuneRiseSet:
                    {
                        Obj3.Name = "Neptune";
                        Obj3.Number = Body.Neptune;
                        break;
                    }
                case EventType.PlutoRiseSet:
                    {
                        Obj3.Name = "Pluto";
                        Obj3.Number = Body.Pluto;
                        break;
                    }
                case EventType.MoonRiseMoonSet:
                    {
                        Obj3.Name = "Moon";
                        Obj3.Number = Body.Moon;
                        break;
                    }
                case EventType.SunRiseSunset:
                case EventType.AmateurAstronomicalTwilight:
                case EventType.AstronomicalTwilight:
                case EventType.CivilTwilight:
                case EventType.NauticalTwilight:
                    {
                        Obj3.Name = "Sun";
                        Obj3.Number = Body.Sun;
                        break;
                    }

                default:
                    {
                        throw new ASCOM.InvalidValueException("TypeOfEvent", TypeOfEvent.ToString(), "Unknown type of event");
                    }
            }

            Obj3.Star = Cat;
            Obj3.Type = ObjectType.MajorPlanetSunOrMoon;

            Obs.OnSurf = Location;
            Obs.Where = ObserverLocation.EarthGeoCenter;

            Novas.Place(Instant + deltaT * Constants.SECONDS2DAYS, Obj3, Obs, deltaT, CoordSys.EquinoxOfDate, Accuracy.Full, ref SkyPosition);
            Retval.Distance = SkyPosition.Dis * Constants.AU2KILOMETRE; // Distance is in AU so save it in km

            rc = Novas.SiderealTime(Instant, 0.0d, deltaT, GstType.GreenwichApparentSiderealTime, Method.EquinoxBased, Accuracy.Full, ref Gmst);

            if (rc != 0)
                throw new HelperException($"BodyAltitude - Novas.SiderealTime returned error code {rc}, zero was expected.");

            Tau = Constants.HOURS2DEG * (Range(Gmst + Longitude * Constants.DEG2HOURS, 0d, true, 24.0d, false) - SkyPosition.RA); // East longitude is  positive
            Retval.Altitude = Math.Asin(Math.Sin(Latitude * Constants.DEG2RAD) * Math.Sin(SkyPosition.Dec * Constants.DEG2RAD) + Math.Cos(Latitude * Constants.DEG2RAD) * Math.Cos(SkyPosition.Dec * Constants.DEG2RAD) * Math.Cos(Tau * Constants.DEG2RAD)) * Constants.RAD2DEG;

            switch (TypeOfEvent)
            {
                case EventType.MercuryRiseSet:
                    {
                        Retval.Radius = Constants.MERCURY_RADIUS; // km
                        break;
                    }
                case EventType.VenusRiseSet:
                    {
                        Retval.Radius = Constants.VENUS_RADIUS; // km
                        break;
                    }
                case EventType.MarsRiseSet:
                    {
                        Retval.Radius = Constants.MARS_RADIUS; // km
                        break;
                    }
                case EventType.JupiterRiseSet:
                    {
                        Retval.Radius = Constants.JUPITER_RADIUS; // km
                        break;
                    }
                case EventType.SaturnRiseSet:
                    {
                        Retval.Radius = Constants.SATURN_RADIUS; // km
                        break;
                    }
                case EventType.UranusRiseSet:
                    {
                        Retval.Radius = Constants.URANUS_RADIUS; // km
                        break;
                    }
                case EventType.NeptuneRiseSet:
                    {
                        Retval.Radius = Constants.NEPTUNE_RADIUS; // km
                        break;
                    }
                case EventType.PlutoRiseSet:
                    {
                        Retval.Radius = Constants.PLUTO_RADIUS; // km
                        break;
                    }
                case EventType.MoonRiseMoonSet:
                    {
                        Retval.Radius = Constants.MOON_RADIUS; // km
                        break;
                    }

                default:
                    {
                        Retval.Radius = Constants.SUN_RADIUS; // km
                        break;
                    }
            }

            return Retval;
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Currently no resources to be disposed.
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose of the Utilities object
        /// </summary>
        /// <remarks>This method is present to implement the IDisposable pattern, which enables the Utilities component to be referenced within a Using statement.</remarks>
        public void Dispose()
        {
            // Do not change this code. Put clean-up code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

    }
}