﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using ASCOM.Common.Interfaces;
using ASCOM.Common;
using ASCOM.Tools.Novas31;

namespace ASCOM.Tools
{
    /// <summary>
    /// ASCOM support utilities
    /// </summary>
    public class AstroUtilities : IDisposable
    {
        private static double currentLeapSeconds;
        private static ILogger TL; // Logger instance for this component instance

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
        static AstroUtilities()
        {
            currentLeapSeconds = Constants.LEAP_SECONDS_DEFAULT;
        }

        #endregion

        #region Public helper members

        /// <summary>
        /// Assign a logger instance to the NOVAS component
        /// </summary>
        /// <param name="logger">an ILigger instance</param>
        public static void SetLogger(ILogger logger)
        {
            TL = logger;
        }

        #endregion

        #region AstroUtils

        /// <summary>
        /// Create a one year almanac for the given event, year and location.
        /// </summary>
        /// <param name="typeOfEvent">The rise-set or twilight times event that the almanac will describe.</param>
        /// <param name="year">Year of the almanac.</param>
        /// <param name="siteLatitude">Latitude of the site for which the almanac is required (-90.0 to +90.0). North positive, south negative.</param>
        /// <param name="siteLongitude">Longitude of the site for which the almanac is required (-180.0 to +180.0 ). East positive, west negative</param>
        /// <param name="siteTimeZone">Time zone of the site for which the almanac is required (-12.0 to +14.0). East positive, west negative</param>
        /// <param name="almanacLogger">An ILogger component to which the almanac will be written.</param>
        /// <remarks>
        /// <para>
        /// At latitudes greater than 60N or 60S it is possible for there to be two rises or sets in a given 24 hour day,
        /// hence results are aggregated into two EventTime lines. If, for a given day, all months have only 0 or 1 event
        /// only the first event times line is included. If any day has two events then the second event times line
        /// is included with the same day number as the first event times line.
        /// </para>
        /// <para>
        /// The following symbols are used in the almanac:
        /// <list type="bullet">
        /// <item><c>****</c> - Visible at all times - risen all day.</item>
        /// <item><c>----</c> - Never visible - set all day.</item>
        /// <item><c>////</c> - Lighter than the twilight threshold all day long.</item>
        /// <item><c>====</c> - Darker than the twilight threshold all day long.</item>
        /// <item>Blank space indicates no events on this day.</item>
        /// </list>
        /// For some event types it is possible that no event of that type occurs, in which case the event time is represented 
        /// as a blank space. At times of year when no events occur, because a body is "always risen" or "always set", rise and set times
        /// are shown as "****" and "----" respectively.
        /// </para>
        /// <para>
        /// The expected output for Moon rising / setting using the parameters: Year=2012, Latitude=75N, Longitude=75W, TimeZone=-5h is given at 
        /// the end of this example.
        /// </para>
        /// </remarks>
        public static void Almanac(EventType typeOfEvent, int year, double siteLatitude, double siteLongitude, double siteTimeZone, ILogger almanacLogger)
        {
            bool isRiseSet; // Flag indicating whether the event is a rise-set or twilight times type of event.
            RiseSetTimes events; // Returned list of events
            string eventTimes1, eventTimes2; // First and second lines of event times

            #region Parameter validation

            // Validate event type and determine whether this is a rise-set or twilight times event.
            switch (typeOfEvent)
            {
                case EventType.SunRiseSunset:
                case EventType.MoonRiseMoonSet:
                case EventType.MercuryRiseSet:
                case EventType.VenusRiseSet:
                case EventType.MarsRiseSet:
                case EventType.JupiterRiseSet:
                case EventType.SaturnRiseSet:
                case EventType.UranusRiseSet:
                case EventType.NeptuneRiseSet:
                case EventType.PlutoRiseSet:
                    isRiseSet = true;
                    break;

                case EventType.CivilTwilight:
                case EventType.NauticalTwilight:
                case EventType.AmateurAstronomicalTwilight:
                case EventType.AstronomicalTwilight:
                    isRiseSet = false;
                    break;

                default:
                    throw new InvalidValueException($"Almanac - Unknown event type: {typeOfEvent}");
            }

            // Validate year
            if ((year < 1900) | (year > 2052))
                throw new InvalidValueException($"Utilities.Almanac - The year parameter is outside the valid range of the DE421 ephemeris (1900 to 2052) that underlies the ephemeris calculations. Requested year: {year}");

            // Validate latitude
            if ((siteLatitude < -90.0) | (siteLatitude > +90.0))
                throw new InvalidValueException($"Utilities.Almanac - The latitude parameter is outside the valid range (-90.0 to +90.0): {siteTimeZone}");

            // Validate longitude
            if ((siteLongitude < -180.0) | (siteLongitude > +180.0))
                throw new InvalidValueException($"Utilities.Almanac - The longitude parameter is outside the valid range (-180.0 to +180.0): {siteLongitude}");

            // Validate time zone
            if ((siteTimeZone < -12.0) | (siteTimeZone > +14.0))
                throw new InvalidValueException($"Utilities.Almanac - The time zone parameter is outside the valid range (-12.0 to +14.0): {siteTimeZone}");

            if (almanacLogger is null)
                throw new InvalidValueException($"Utilities.Almanac - The almanac logger is null.");

            #endregion

            // Write the title
            LogMessageInfo("Almanac", $"                                                           ASCOM Almanac", almanacLogger);
            BlankLine(almanacLogger);

            // Write the almanac parameters
            LogMessageInfo("Almanac", $"Latitude: {Utilities.DegreesToDMS(Math.Abs(siteLatitude), ":", ":", "", 0)} {(siteLatitude >= 0.0 ? "N" : "S")},   Longitude: {Utilities.DegreesToDMS(Math.Abs(siteLongitude), ":", ":", "", 0)} {(siteLongitude >= 0.0 ? "E" : "W")},   Time Zone: {Math.Abs(siteTimeZone)} hours {(siteTimeZone <= 0.0 ? "West" : "East")} of Greenwich,   Year: {year},   Event: {typeOfEvent}", almanacLogger);
            BlankLine(almanacLogger);

            // Write title lines
            LogMessageInfo("Almanac", "       Jan.       Feb.       Mar.       Apr.       May        June       July       Aug.       Sept.      Oct.       Nov.       Dec.  ", almanacLogger);
            if (isRiseSet)
                LogMessageInfo("Almanac", "Day Rise  Set  Rise  Set  Rise  Set  Rise  Set  Rise  Set  Rise  Set  Rise  Set  Rise  Set  Rise  Set  Rise  Set  Rise  Set  Rise  Set", almanacLogger);
            else
                LogMessageInfo("Almanac", "Day Begin End  Begin End  Begin End  Begin End  Begin End  Begin End  Begin End  Begin End  Begin End  Begin End  Begin End  Begin End", almanacLogger);

            // Work through each day number in turn, processing all months having that day.
            // This approach is taken in order to create output in a similar format to that which comes from the USNO ephemeris data web
            // site: https://aa.usno.navy.mil/data/mrst 
            for (int dayOfMonth = 1; dayOfMonth <= 31; dayOfMonth++) // Process 1 day at a time to match the USNO online Almanac. Tries 31 days even for short months
            {
                eventTimes1 = ""; // Initialise event times
                eventTimes2 = "";

                // Process each month in turn for this day number
                for (int monthOfYear = 1; monthOfYear <= 12; monthOfYear++)
                {
                    try
                    {
                        events = EventTimes(typeOfEvent, dayOfMonth, monthOfYear, year, siteLatitude, siteLongitude, siteTimeZone); // Get the rise and set events list

                        if ((events.RiseEvents.Count > 0) | (events.SetEvents.Count > 0))
                        {
                            switch (events.RiseEvents.Count)
                            {
                                case 0: // No rises
                                    eventTimes1 += "    ";
                                    eventTimes2 += "    ";
                                    break;

                                case 1: // 1 rise so build up the first message line
                                    eventTimes1 += RoundHour(events.RiseEvents[0]);
                                    eventTimes2 += "    ";
                                    break;

                                case 2: // 2 rises so build up message lines 1 and 2
                                    eventTimes1 += RoundHour(events.RiseEvents[0]);
                                    eventTimes2 += RoundHour(events.RiseEvents[1]);
                                    break;

                                default:
                                    eventTimes1 += "????";
                                    eventTimes2 += "????";
                                    break;
                            }

                            eventTimes1 += " "; // Add spacer between rise and set value
                            eventTimes2 += " ";

                            switch (events.SetEvents.Count)
                            {
                                case 0: // No sets
                                    eventTimes1 += "    ";
                                    eventTimes2 += "    ";
                                    break;

                                case 1: // 1 set so build up the first message line
                                    eventTimes1 += RoundHour(events.SetEvents[0]);
                                    eventTimes2 += "    ";
                                    break;

                                case 2: // 2 sets so build up message lines 1 and 2
                                    eventTimes1 += RoundHour(events.SetEvents[0]);
                                    eventTimes2 += RoundHour(events.SetEvents[1]);
                                    break;

                                default:
                                    eventTimes1 += "????";
                                    eventTimes2 += "????";
                                    break;
                            }
                        }
                        else if (events.AboveHorizonAtMidnight) // No events so must always be above the horizon / threshold
                        {
                            if (isRiseSet)
                            {
                                eventTimes1 += "**** ****"; // Flag as above 
                                eventTimes2 += "         ";
                            }
                            else
                            {
                                eventTimes1 += "//// ////"; // Flag as above 
                                eventTimes2 += "         ";
                            }
                        }
                        else // No events so must always be below the horizon / threshold.
                        {
                            if (isRiseSet)
                            {
                                eventTimes1 += "---- ----"; // Flag as below
                                eventTimes2 += "         ";
                            }
                            else
                            {
                                eventTimes1 += "==== ===="; // Flag as above 
                                eventTimes2 += "         ";
                            }
                        }

                        // Add spacers between months
                        eventTimes1 += "  ";
                        eventTimes2 += "  ";
                    }
                    catch (InvalidValueException) // Invalid date so insert white space in place of a value
                    {
                        eventTimes1 += "           ";
                        eventTimes2 += "           ";

                    }
                    catch (Exception ex)
                    {
                        LogMessageInfo("Loop vales", $"Day of month: {dayOfMonth}, Month of year: {monthOfYear}", almanacLogger);
                        LogMessageInfo("Exception", ex.ToString(), almanacLogger);
                    }
                }

                // Print the whole day line and the line with second events if it has any values
                LogMessageInfo("Almanac", $"{dayOfMonth:00}  {eventTimes1}", almanacLogger);
                if (eventTimes2.Trim() != "")
                    LogMessageInfo("Almanac", $"{dayOfMonth:00}  {eventTimes2}", almanacLogger);
            }
            BlankLine(almanacLogger);

            // Write the explanatory legend
            if (isRiseSet) // select correct text for body rise/set almanac.
                LogMessageInfo("", "    **** Object continuously above horizon                            ---- Object continuously below horizon", almanacLogger);
            else // Select correct text for twilight almanac.
                LogMessageInfo("", "    //// Sun continuously above twilight limit                        ==== Sun continuously below twilight limit", almanacLogger);

            LogMessageInfo("", "         Spaces indicate no event                                          Multiple events in a day are shown as multiple day lines", almanacLogger);
            BlankLine(almanacLogger);
            LogMessageInfo("", "         Add one hour when daylight savings time is in effect", almanacLogger);
            BlankLine(almanacLogger);
        }

        /// <summary>
        /// Function that returns a list of rise and set events of a particular type that occur on a particular day at a given latitude, longitude and time zone
        /// </summary>
        /// <param name="typeOfEvent">Type of event e.g. Sunrise or Astronomical twilight</param>
        /// <param name="dayOfWeek">Integer Day number</param>
        /// <param name="monthOfYear">Integer Month number</param>
        /// <param name="year">Integer Year number</param>
        /// <param name="siteLatitude">Site latitude</param>
        /// <param name="siteLongitude">Site longitude (West of Greenwich is negative)</param>
        /// <param name="siteTimeZone">Site time zone offset (West of Greenwich is negative)</param>
        /// <returns>An <see cref="RiseSetTimes"/> class containing event information.
        /// </returns>
        /// <exception cref="ASCOM.InvalidValueException">When the supplied combination of day, month and year is invalid e.g. 31st September.</exception>
        /// <remarks>
        /// <para>The definitions of sunrise, sunset and the various twilights that are used in this method are taken from the 
        /// <a href="https://aa.usno.navy.mil/faq/RST_defs">US Naval Observatory Definitions</a>.
        /// </para>
        /// <para>The dynamics of the Sun, Earth and Moon can result at some latitudes in days where there may be no, 1 or 2 rise or set events during a 24 SI hour period.</para>
        /// <para> Rise-set or start-end times are available for the following events: 
        /// <list type="Bullet">
        /// <item>Sun rise-set.</item>
        /// <item>Moon rise-set.</item>
        /// <item>Civil twilight start-end.</item>
        /// <item>Nautical twilight start-end.</item>
        /// <item>Amateur astronomical twilight start-end.</item>
        /// <item>Astronomical twilight start-end.</item>
        /// <item>Mercury rise-set.</item>
        /// <item>Venus rise-set.</item>
        /// <item>Mars rise-set.</item>
        /// <item>Jupiter rise-set.</item>
        /// <item>Saturn rise-set.</item>
        /// <item>Uranus rise-set.</item>
        /// <item>Neptune rise-set.</item>
        /// <item>Pluto rise-set.</item>
        /// </list>
        /// </para>
        /// <para>The returned <see cref="RiseSetTimes"/> class presents these members:
        /// <list type="Bullet">
        /// <item>AboveHorizonAtMidnight                  - Boolean - True if the body is above the event limit at midnight (the beginning of the 24 hour day), false if it is below the event limit</item>
        /// <item>RiseEvents                              - Generic List of double - List of rise times in this 24 hour period</item>
        /// <item>SetEvents                               - Generic List of double - List of set times in this 24 hour period</item>
        /// </list></para>
        /// <para>The algorithm employed in this method is taken from Astronomy on the Personal Computer (Montenbruck and Pfleger) pp 46..56, 
        /// Springer Fourth Edition 2000, Fourth Printing 2009. The day is divided into twelve two hour intervals and a quadratic equation is fitted
        /// to the altitudes at the beginning, middle and end of each interval. The resulting equation coefficients are then processed to determine 
        /// the number of roots within the interval (each of which corresponds to a rise or set event) and their sense (rise or set). 
        /// These results are then aggregated over the day and the resultant list of values returned as the function result.
        /// </para>
        /// <para>High precision ephemerides for the Sun, Moon and Earth and other planets from the JPL DE421 series are employed as delivered by the 
        /// ASCOM <see cref="Novas"/> component rather than using the lower precision ephemerides employed by Montenbruck and Pfleger.
        /// </para>
        /// <para><b>Accuracy</b> Whole year almanacs for Sunrise/Sunset, Moonrise/Moonset and the various twilights every 5 degrees from the 
        /// North pole to the South Pole at a variety of longitudes, time-zones and dates have been compared to data from
        /// the <a href="https://aa.usno.navy.mil/data/RS_OneYear">US Naval Observatory Astronomical Data</a> web site.
        /// Most returned event times are within 1 minute of the USNO values although some very infrequent grazing event times at latitudes from 67 to 90 degrees North and South can be up to 
        /// 10 minutes different.
        /// </para>
        /// <para>An Almanac program that creates a year's worth of information for a given event, latitude, longitude and time-zone is included in the 
        /// developer code examples elsewhere in this help file. This creates an output file with an almost identical format to that used by the USNO web site 
        /// and allows comprehensive checking of accuracy for a given set of parameters.</para>
        /// </remarks>
        public static RiseSetTimes EventTimes(EventType typeOfEvent, int dayOfWeek, int monthOfYear, int year, double siteLatitude, double siteLongitude, double siteTimeZone)
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

            #region Parameter validation

            // Validate event type and determine whether this is a rise-set or twilight times event.
            switch (typeOfEvent)
            {
                case EventType.SunRiseSunset:
                case EventType.MoonRiseMoonSet:
                case EventType.MercuryRiseSet:
                case EventType.VenusRiseSet:
                case EventType.MarsRiseSet:
                case EventType.JupiterRiseSet:
                case EventType.SaturnRiseSet:
                case EventType.UranusRiseSet:
                case EventType.NeptuneRiseSet:
                case EventType.PlutoRiseSet:
                case EventType.CivilTwilight:
                case EventType.NauticalTwilight:
                case EventType.AmateurAstronomicalTwilight:
                case EventType.AstronomicalTwilight:
                    // No action required.
                    break;

                default:
                    throw new InvalidValueException($"Almanac - Unknown event type: {typeOfEvent}");
            }

            // Validate year
            if ((year < 1900) | (year > 2052))
                throw new InvalidValueException($"Utilities.Almanac - The year parameter is outside the valid range of the DE421 ephemeris (1900 to 2052) that underlies the ephemeris calculations. Requested year: {year}");

            // Validate latitude
            if ((siteLatitude < -90.0) | (siteLatitude > +90.0))
                throw new InvalidValueException($"Utilities.Almanac - The latitude parameter is outside the valid range (-90.0 to +90.0): {siteLatitude}");

            // Validate longitude
            if ((siteLongitude < -180.0) | (siteLongitude > +180.0))
                throw new InvalidValueException($"Utilities.Almanac - The longitude parameter is outside the valid range (-180.0 to +180.0): {siteLongitude}");

            // Validate time zone
            if ((siteTimeZone < -12.0) | (siteTimeZone > +14.0))
                throw new InvalidValueException($"Utilities.Almanac - The time zone parameter is outside the valid range (-12.0 to +14.0): {siteTimeZone}");

            #endregion


            try
            {
                TestDate = DateTime.Parse(monthOfYear + "/" + dayOfWeek + "/" + year, System.Globalization.CultureInfo.InvariantCulture); // Test whether this is a valid date e.g is not the 31st of February
                LogMessageDebug("RiseSetTimes", $"Day: {dayOfWeek}, Month: {monthOfYear}, Year: {year} - Test date: {TestDate}");
            }
            catch (FormatException ex) // Catch case where day exceeds the maximum number of days in the month
            {
                LogMessageDebug("RiseSetTimes", $"Caught FormatException: {ex.Message} for {dayOfWeek} - {monthOfYear} - {year}");
                throw new InvalidValueException("Day or Month", dayOfWeek.ToString() + " " + monthOfYear.ToString() + " " + year.ToString(), "Day must not exceed the number of days in the month");
            }
            catch (Exception) // Throw all other exceptions as they are received
            {
                throw;
            }

            // Calculate Julian date in the local time-zone
            JD = Novas.JulianDate((short)year, (short)monthOfYear, (short)dayOfWeek, 0.0d) - siteTimeZone / 24.0d;

            // Initialise observer structure and calculate the refraction at the horizon
            Observer.Latitude = siteLatitude;
            Observer.Longitude = siteLongitude;
            RefractionCorrection = Novas.Refract(Observer, RefractionOption.StandardRefraction, 90.0d);

            // Iterate over the day in two hour periods

            // Start at 01:00 as the centre time i.e. then time range will be 00:00 to 02:00
            CentreTime = 1.0d;

            do
            {
                // Calculate body positional information
                BodyInfoMinus1 = BodyAltitude(typeOfEvent, JD, CentreTime - 1d, siteLatitude, siteLongitude);
                BodyInfo0 = BodyAltitude(typeOfEvent, JD, CentreTime, siteLatitude, siteLongitude);
                BodyInfoPlus1 = BodyAltitude(typeOfEvent, JD, CentreTime + 1d, siteLatitude, siteLongitude);

                // Correct altitude for body's apparent size, parallax, required distance below horizon and refraction
                switch (typeOfEvent)
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

            while (!(DoesRise & DoesSet & Math.Abs(siteLatitude) < 60.0d | CentreTime == 25.0d));

            Retval.AboveHorizonAtMidnight = AboveHorizon; // Add above horizon at midnight flag
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
        /// Chapter 48 p345 by Jean Meeus (Willmann-Bell 1991). The Sun and Moon positions are calculated by high precision NOVAS 3.1 library using JPL DE 421 ephemerides.
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
        /// <returns>Moon phase as an angle between -180.0 and +180.0 (see Remarks for further description)</returns>
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
            PositionAngle = Utilities.Range((MoonPosition.RA - SunPosition.RA) * Constants.HOURS2DEG, -180.0d, false, 180.0d, true);

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
        /// Set the current number of leap seconds (overrides the built-in leap second value)
        /// </summary>
        /// <param name="leapSeconds">Current number of leap seconds (37.0 at July 2023)</param>
        public static void SetLeapSeconds(double leapSeconds)
        {
            // Set the internal leap seconds value to the supplied value
            if (leapSeconds < 0.0)
                throw new InvalidValueException($"Utlities.SetLeapSeconds - Supplied value is zero or less: {leapSeconds}");

            // Save the provided value
            currentLeapSeconds = leapSeconds;
        }

        /// <summary>
        /// Returns the value of DeltaT (Terrestrial time minus Universal Time = TT-UT1) at the given Julian date.
        /// </summary>
        /// <param name="julianDateUTC">Julian date for which the DeltaT value is required.</param>
        /// <returns>The number of seconds difference between terrestrial time and universal time.</returns>
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

            // DATE RANGE 31st December 2024 onwards - This is beyond the sensible extrapolation range of the most recent data analysis so revert to the basic formula: DeltaT = LeapSeconds + 32.184
            if ((yearFraction >= 2025.0))
            {
                try
                {
                    double currentLeapSeconds = 0.0;

                    // Get the leap second value from the SOFA library
                    DateTime utcTime = DateTime.UtcNow;
                    short rc = Sofa.Dat(utcTime.Year, utcTime.Month, utcTime.Day, utcTime.TimeOfDay.TotalHours / 24.0, ref currentLeapSeconds);

                    // Test whether the call worked correctly
                    if (rc == 0)
                        // Call worked correctly so use the returned value
                        retval = currentLeapSeconds + TT_TAI_OFFSET;
                    else
                        // Call failed so use the ultimate fallback value
                        retval = LEAP_SECOND_ULTIMATE_FALLBACK_VALUE + TT_TAI_OFFSET;
                }
                catch (Exception)
                {
                    // Use the ultimate fallback value
                    retval = LEAP_SECOND_ULTIMATE_FALLBACK_VALUE + TT_TAI_OFFSET;
                }
            }
            else if ((yearFraction >= 2023.6))
                retval = (+0.0 * modifiedJulianDay * modifiedJulianDay * modifiedJulianDay * modifiedJulianDay * modifiedJulianDay)
                    + (+0.0 * modifiedJulianDay * modifiedJulianDay * modifiedJulianDay * modifiedJulianDay)
                    + (-8.3655273366064300E-09 * modifiedJulianDay * modifiedJulianDay * modifiedJulianDay)
                    + (+1.5133847966003900E-03 * modifiedJulianDay * modifiedJulianDay)
                    + (-9.1260465097482900E+01 * modifiedJulianDay)
                    + (+1.8344658890493000E+06);
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
            rc = Sofa.Dtf2d("UTC", ObservationDateTime.Year, ObservationDateTime.Month, ObservationDateTime.Day, ObservationDateTime.Hour, ObservationDateTime.Minute, ObservationDateTime.Second + Convert.ToDouble(ObservationDateTime.Millisecond) / 1000.0d, ref jd1, ref jd2);
            if (rc > 1) // rc==0 is OK, rc==1 is a warning that the data may not be reliable but a valid result is returned
                throw new HelperException($"UTCJulianDate- Bad return code from Sofa.Dtf2d: {rc} for date: {ObservationDateTime:dddd dd MMMM yyyy HH:mm:ss.fff}");

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

        #region Private support code

        /// <summary>
        /// Rounds an hour value up or down to the nearest minute
        /// </summary>
        /// <param name="Moment">Time of day expressed in hours</param>
        /// <returns>String in HH:mm format rounded to the neaest minute</returns>
        /// <remarks>.NET rounding, when going from doubles to HH:mm format, always rounds down to the nearest minute e.g. 11:32:58 will be
        /// returned as 11:58 rather than 11:59. This function adds 30 seconds to the supplied date and rounds that value in order to 
        /// achieve rounding where XX:YY:00 to XX:YY:29 becomes XX:YY and XX:YY:30 to XX:YY:59 becomes XX:YY+1.
        /// <para>Rounding is omitted for minute 23:59 in order to prevent the value flipping over into the next day</para></remarks>
        private static string RoundHour(double Moment)
        {
            string Retval;
            try
            {
                if (Moment >= 23.9833333)
                    Retval = new DateTime().AddHours(Moment).ToString("HHmm"); // Util.HoursToHM(Moment, "")
                else
                    Retval = new DateTime().AddHours(Moment).AddSeconds(30).ToString("HHmm");
            }
            catch (Exception)
            {
                Retval = "XXXX";
            }
            return Retval;
        }

        /// <summary>
        /// Log a message at Debug level
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        private static void LogMessageDebug(string context, string message)
        {
            if (!(TL is null))
            {
                if (TL is TraceLogger traceLogger)
                {
                    traceLogger.LogMessage(LogLevel.Debug, context, message);
                }
                else
                {
                    TL.LogDebug($"{context} - {message}");
                }
            }
        }

        /// <summary>
        /// Log a message at Info level
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        /// <param name="logger"></param>
        private static void LogMessageInfo(string context, string message, ILogger logger)
        {
            if (!(logger is null))
            {
                if (logger is TraceLogger traceLogger)
                {
                    traceLogger.LogMessage(LogLevel.Information, context, message, false);
                }
                else
                {
                    logger.LogInformation($"{message}");
                }
            }
        }

        /// <summary>
        /// Log a blank line
        /// </summary>
        private static void BlankLine(ILogger logger)
        {
            if (!(logger is null))
            {
                if (logger is TraceLogger traceLogger)
                {
                    traceLogger.BlankLine();
                }
                else
                {
                    logger.LogInformation($" ");
                }
            }
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

            Tau = Constants.HOURS2DEG * (Utilities.Range(Gmst + Longitude * Constants.DEG2HOURS, 0d, true, 24.0d, false) - SkyPosition.RA); // East longitude is  positive
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