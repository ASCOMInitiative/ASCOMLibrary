using ASCOM.Tools.Novas31;
using System;
using System.Globalization;

namespace ASCOM.Tools
{

    /// <summary>
    /// Provides astrometric and topocentric coordinates for the planets, sun and moon.
    /// </summary>
    /// <remarks>
    /// This is an easy to use tool that provides coordinates for major solar system bodies. Internally it uses the JPL DE421 ephemeris data contained within the NOVAS component.
    /// Pluto is included for convenience even though it is no longer recognised as a major planet by the IAU.
    /// </remarks>
    public partial class SolarSystemBody
    {
        const double AU2KM = 149597870.7;
        const double MODIFIED_JULIAN_DATE_OFFSET = 2400000.5;
        const double J2000 = 2451545.0;

        private readonly Body body;
        private SetBy setBy = SetBy.None;
        private Ephemeris ephemeris;

        private enum SetBy
        {
            None = 0,
            PlanetSunMoon = 1,
            CometAsteroidOrbit = 2,
        }

        #region Initiators

        /// <summary>
        /// Creates a SolarSystem object to which a pre-configured Ephemeris object can be added
        /// </summary>
        public SolarSystemBody()
        {
            Ephemeris = new Ephemeris();
        }

        /// <summary>
        /// Create a SolarSystem object for a specified target body
        /// </summary>
        /// <param name="body"></param>
        /// <exception cref="HelperException">When earth is specified as the target body.</exception>
        public SolarSystemBody(Body body) : this()
        {
            // Validate the body parameter
            if (body == Body.Uninitialised)
                throw new HelperException($"SolarSystem.Initiator - The supplied body value is invalid: {body}.");

            if (((int)body < 1) | ((int)body > 11))
                throw new HelperException($"SolarSystem.Initiator - The supplied body value is invalid: {(int)body}.");

            this.body = body;
            setBy = SetBy.PlanetSunMoon;
            Name = body.ToString();

            ephemeris = new Ephemeris();
            switch (body)
            {
                case Body.Mercury:
                case Body.Venus:
                case Body.Earth:
                case Body.Mars:
                case Body.Jupiter:
                case Body.Saturn:
                case Body.Uranus:
                case Body.Neptune:
                case Body.Pluto:
                    ephemeris.BodyType = BodyType.MajorPlanet;
                    ephemeris.Number = body;
                    break;

                case Body.Sun:
                    ephemeris.BodyType = BodyType.Sun;
                    ephemeris.Number = body;
                    break;

                case Body.Moon:
                    ephemeris.BodyType = BodyType.Moon;
                    ephemeris.Number = body;
                    break;

                default:
                    throw new HelperException($"SolarSystem.Initiator - Unsupported body type: {(int)body}.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orbitDataSource">Organisation providing the orbit data and the data format specification</param>
        /// <param name="orbitString">Orbit data as a a string in the selected organisation's data format.</param>
        /// <exception cref="HelperException">If the data elements defined by the organisation's format cannot be extracted from the supplied orbit data string.</exception>
        /// <remarks>The internal ephemeris object orbit parameters are populated by parsing the orbit string provided in this constructor. <see cref="HelperException"/>s are raised 
        /// if fields specified in the selected data format cannot be read.</remarks>
        public SolarSystemBody(OrbitDataSource orbitDataSource, string orbitString) : this()
        {
            if (string.IsNullOrEmpty(orbitString))
                throw new HelperException("SolarSystem.Initiator - Orbit string is null or empty.");

            switch (orbitDataSource)
            {
                case OrbitDataSource.MpcCometOrbit:
                    if (orbitString.Length < 168)
                        throw new HelperException("SolarSystem.Initiator - The MPC comet data string is expected to be 168 characters long");

                    //0         1         2         3         4         5         6         7         8         9         0         1         2         3         4         5         6         7
                    //0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
                    //    DH66G010  2024 10 28.979   0.40276   0.85213   175.688    93.542     8.716   20231105   9.0  4.0  D/1766 G1 (Helfenzrieder)                                 26,  114
                    //0419P         2021 10 27.9227  2.549307  0.276801  187.7887   40.3976    2.8025  20231105  12.5  4.0  419P/PANSTARRS                                           MPEC 2023-UR6

                    // Set the body type
                    Ephemeris.BodyType = BodyType.Comet;

                    // Set the comet name
                    string nameString = orbitString.Substring(102, 56);

                    // Trim the discoverer from the comet name if present e.g. "D/1766 G1 (Helfenzrieder)" becomes "D/1766 G11"
                    int nameEnd = nameString.IndexOf("(") - 1;
                    if (nameEnd > 0) // A discoverer name is present
                        Ephemeris.Name = nameString.Substring(0, nameEnd).TrimEnd();
                    else // No discoverer name
                        Ephemeris.Name = nameString.TrimEnd();

                    Name = Ephemeris.Name;

                    // Set the date of perihelion passage
                    int year = GetIntParameter("Perihelion year", 14, 4, orbitString);
                    int month = GetIntParameter("Perihelion month", 19, 2, orbitString);
                    double day = GetParameter("Perihelion day", 22, 7, orbitString);
                    DateTime perihelionPassageTT = new DateTime(year, month, 1).AddDays(day - 1);
                    Ephemeris.Epoch = Utilities.JulianDateFromDateTime(perihelionPassageTT);

                    // Set remaining parameters
                    Ephemeris.q_PerihelionDistance = GetParameter("Perihelion distance (q)", 30, 9, orbitString); // Double.Parse(orbitString.Substring(30, 9));
                    Ephemeris.e_OrbitalEccentricity = GetParameter("Orbital eccentricity (e)", 41, 8, orbitString); ;
                    Ephemeris.w_PerihelionArgument = GetParameter("Argument of perihelion", 51, 8, orbitString); ;
                    Ephemeris.Node = GetParameter("Longitude of ascending node", 61, 8, orbitString); ;
                    Ephemeris.i_Inclination = GetParameter("Inclination", 71, 8, orbitString); ;
                    break;

                case OrbitDataSource.MpcAsteroidOrbit:
                    if (orbitString.Length < 202)
                        throw new HelperException("SolarSystem.Initiator - The MPC asteroid data string is expected to be 202 characters long");

                    //0         1         2         3         4         5         6         7         8         9        10        11        12        13        14        15        16        17        18        19        20        21
                    //0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
                    //00001    3.33  0.15 K239D  60.07881   73.42179   80.25496   10.58688  0.0789126  0.21410680   2.7672543  0 E2023-F87  7283 123 1801-2023 0.65 M-v 30k MPCLINUX   0000      (1) Ceres              20230321

                    // Set the body type
                    Ephemeris.BodyType = BodyType.MinorPlanet;

                    // Set asteroid name
                    Ephemeris.Name = orbitString.Substring(166, Math.Min(28, orbitString.Length - 166)).Trim();
                    Name = Ephemeris.Name;

                    // Set the asteroid orbit epoch
                    Ephemeris.Epoch = PackedToJulian(orbitString.Substring(20, 5));

                    // Set the asteroid orbit parameters
                    Ephemeris.a_SemiMajorAxis = GetParameter("Semi-major axis", 92, 11, orbitString);
                    Ephemeris.e_OrbitalEccentricity = GetParameter("Orbital eccentricity", 70, 9, orbitString);
                    Ephemeris.i_Inclination = GetParameter("Inclination", 59, 9, orbitString); // double.Parse(orbitString.Substring(59, 9));
                    Ephemeris.w_PerihelionArgument = GetParameter("Argument of perihelion", 37, 9, orbitString); // double.Parse(orbitString.Substring(37, 9));
                    Ephemeris.Node = GetParameter("Longitude of ascending node", 48, 9, orbitString);
                    Ephemeris.M_MeanAnomalyAtEpoch = GetParameter("Mean anomaly", 26, 9, orbitString);
                    Ephemeris.n_MeanDailyMotion = GetParameter("Mean daily motion", 80, 11, orbitString);  //double.Parse(orbitString.Substring(80, 11));
                    break;

                case OrbitDataSource.JplCometOrbit:
                    if (orbitString.Length < 119)
                        throw new HelperException("SolarSystem.Initiator - The JPL comet data string is expected to be 119 characters long");

                    //0         1         2         3         4         5         6         7         8         9         0         1         2         3         4         5         6         7
                    //0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
                    //Num  Name                                     Epoch      q           e        i         w        Node          Tp       Ref
                    //------------------------------------------- ------- ----------- ---------- --------- --------- --------- -------------- ------------
                    //  1P/Halley                                   39857  0.57471580 0.96794279 162.18787 112.25779  59.11448 19860208.64890 JPL 73
                    //  2P/Encke                                    57744  0.33590434 0.84833343  11.77827 186.56208 334.56033 20170310.09261 JPL K235/30
                    //  3D/Biela                                    -9480  0.87907300 0.75129900  13.21640 221.65880 250.66900 18321126.61520 IAUCAT03

                    // Set the body type
                    Ephemeris.BodyType = BodyType.Comet;

                    // Extract the comet name
                    nameString = orbitString.Substring(0, 43).Trim();

                    // Trim the discoverer from the comet name if present e.g. "D/1766 G1 (Helfenzrieder)" becomes "D/1766 G11"
                    nameEnd = nameString.IndexOf("(") - 1;
                    if (nameEnd > 0) // A discoverer name is present
                        Ephemeris.Name = nameString.Substring(0, nameEnd).TrimEnd();
                    else // No discoverer name
                        Ephemeris.Name = nameString.TrimEnd();

                    Name = Ephemeris.Name;

                    // Set the date of perihelion passage
                    year = GetIntParameter("Perihelion year", 105, 4, orbitString);
                    month = GetIntParameter("Perihelion month", 109, 2, orbitString);
                    day = GetParameter("Perihelion day", 111, 8, orbitString);
                    perihelionPassageTT = new DateTime(year, month, 1).AddDays(day - 1);
                    Ephemeris.Epoch = Utilities.JulianDateFromDateTime(perihelionPassageTT);

                    // Set remaining parameters
                    Ephemeris.q_PerihelionDistance = GetParameter("Perihelion distance (q)", 52, 11, orbitString); // Double.Parse(orbitString.Substring(30, 9));
                    Ephemeris.e_OrbitalEccentricity = GetParameter("Orbital eccentricity (e)", 64, 10, orbitString); ;
                    Ephemeris.i_Inclination = GetParameter("Inclination", 75, 9, orbitString); ;
                    Ephemeris.w_PerihelionArgument = GetParameter("Argument of perihelion", 85, 9, orbitString); ;
                    Ephemeris.Node = GetParameter("Longitude of ascending node", 95, 9, orbitString); ;
                    break;

                case OrbitDataSource.JplNumberedAsteroidOrbit:
                    if (orbitString.Length < 105)
                        throw new HelperException("SolarSystem.Initiator - The JPL numbered asteroid data string is expected to be at least 105 characters long");

                    //0         1         2         3         4         5         6         7         8         9        10        11        12        13        14        15        16        17        18        19        20        21
                    //0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
                    //------ ----------------- ----- ---------- ---------- --------- --------- --------- ----------- ----- ----- ----------
                    // Num   Name              Epoch      a          e        i         w        Node        M         H     G   Ref
                    //     1 Ceres             60200  2.7672544 0.07891253  10.58688  73.42180  80.25498  60.0787728  3.33  0.12 JPL 48

                    // Set the body type
                    Ephemeris.BodyType = BodyType.MinorPlanet;

                    // Set asteroid name
                    Ephemeris.Name = orbitString.Substring(0, 24).Trim();
                    Name = Ephemeris.Name;

                    // Set the asteroid orbit epoch
                    Ephemeris.Epoch = MODIFIED_JULIAN_DATE_OFFSET + GetIntParameter("Modified Julian date", 25, 5, orbitString);

                    // Set the asteroid orbit parameters
                    Ephemeris.a_SemiMajorAxis = GetParameter("Semi-major axis", 31, 10, orbitString);
                    Ephemeris.e_OrbitalEccentricity = GetParameter("Orbital eccentricity", 42, 10, orbitString);
                    Ephemeris.i_Inclination = GetParameter("Inclination", 53, 9, orbitString); // double.Parse(orbitString.Substring(59, 9));
                    Ephemeris.w_PerihelionArgument = GetParameter("Argument of perihelion", 63, 9, orbitString); // double.Parse(orbitString.Substring(37, 9));
                    Ephemeris.Node = GetParameter("Longitude of ascending node", 73, 9, orbitString);
                    Ephemeris.M_MeanAnomalyAtEpoch = GetParameter("Mean anomaly", 83, 11, orbitString);
                    break;

                case OrbitDataSource.JplUnNumberedAsteroidOrbit:
                    if (orbitString.Length < 95)
                        throw new HelperException("SolarSystem.Initiator - The JPL un-numbered asteroid data string is expected to be 95 characters long");

                    //0         1         2         3         4         5         6         7         8         9        10        11        12        13        14        15        16        17        18        19        20        21
                    //0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
                    //Designation   Epoch      a           e        i         w        Node        M         H    G   Ref
                    //------------- ----- ----------- ---------- --------- --------- --------- ----------- ----- ---- ----------
                    //  1927 LA     25051   3.3440715 0.33361825  17.63150 341.10952 191.71742  45.7201688 11.00 0.15 JPL 11
                    //  2023 VA     60200   1.5209582 0.43368284   2.97641  55.88542  39.06217 311.7082881 28.59 0.15 JPL 4

                    // Set the body type
                    Ephemeris.BodyType = BodyType.MinorPlanet;

                    // Set asteroid name
                    Ephemeris.Name = orbitString.Substring(0, 13).Trim();
                    Name = Ephemeris.Name;

                    // Set the asteroid orbit epoch
                    Ephemeris.Epoch = MODIFIED_JULIAN_DATE_OFFSET + GetParameter("Modified Julian date", 14, 5, orbitString);
                    // Set the asteroid orbit parameters
                    Ephemeris.a_SemiMajorAxis = GetParameter("Semi-major axis", 20, 11, orbitString);
                    Ephemeris.e_OrbitalEccentricity = GetParameter("Orbital eccentricity", 32, 10, orbitString);
                    Ephemeris.i_Inclination = GetParameter("Inclination", 43, 9, orbitString);
                    Ephemeris.w_PerihelionArgument = GetParameter("Argument of perihelion", 53, 9, orbitString);
                    Ephemeris.Node = GetParameter("Longitude of ascending node", 63, 9, orbitString);
                    Ephemeris.M_MeanAnomalyAtEpoch = GetParameter("Mean anomaly", 73, 11, orbitString);
                    break;

                case OrbitDataSource.LowellAsteroid:
                    if (orbitString.Length < 267)
                        throw new HelperException("SolarSystem.Initiator - The Lowell asteroid data string is expected to be 267 characters long");

                    //(1)    (2)                (3)             (4)    (5)  (6)  (7)   (8)     (9)                   (10) (11)  (12)     (13)       (14)       (15)       (16)      (17)      (18)          (19)     (20)     (21)    (22)     (23)             (24)             (25)
                    //A6     A18                A15             F5.2  A4                                             I5   I5    I4  I2I2 F10.6      F10.6      F10.6     F10.6      F10.8     F13.8
                    //______X__________________X_______________X_____X____                                           _____#####X____##__X__________X__________X__________##########X__________#############X____##__X
                    //     1 Ceres              E. Bowell        3.34  0.12 0.72 913.0 G?      0   0   0   0   0   0 56959 4750 19960427  80.477333  71.802404  80.659857 10.600303 0.07604100   2.76788714 19960414 2.3E-02  1.4E-04 19960416 2.7E-02 19960530 3.1E-02 20040111 3.1E-02 20040111
                    //         0         0         0         0         0         0         0         0         0         1         1         1         1         1         1         1         1         1         1         2         2         2         2         2         2         2
                    //         1         2         3         4         5         6         7         8         9         0         1         2         3         4         5         6         7         8         9         0         1         2         3         4         5         6
                    //123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567

                    // Set the body type
                    Ephemeris.BodyType = BodyType.MinorPlanet;

                    // Set asteroid name
                    Ephemeris.Name = orbitString.Substring(7, 18).Trim();
                    Name = Ephemeris.Name;

                    // Set the asteroid orbit epoch
                    year = GetIntParameter("Epoch year", 106, 4, orbitString);
                    month = GetIntParameter("Epoch month", 110, 2, orbitString);
                    day = GetParameter("Epoch day", 112, 2, orbitString);
                    DateTime epoch = new DateTime(year, month, 1).AddDays(day - 1);
                    Ephemeris.Epoch = Utilities.JulianDateFromDateTime(epoch);

                    // Set the asteroid orbit parameters
                    Ephemeris.a_SemiMajorAxis = GetParameter("Semi-major axis", 168, 13, orbitString);
                    Ephemeris.e_OrbitalEccentricity = GetParameter("Orbital eccentricity", 158, 10, orbitString);
                    Ephemeris.i_Inclination = GetParameter("Inclination", 147, 10, orbitString);
                    Ephemeris.w_PerihelionArgument = GetParameter("Argument of perihelion", 126, 10, orbitString);
                    Ephemeris.Node = GetParameter("Longitude of ascending node", 137, 10, orbitString);
                    Ephemeris.M_MeanAnomalyAtEpoch = GetParameter("Mean anomaly", 115, 10, orbitString);
                    break;

                default:
                    throw new HelperException($"SolarSystem.Initiator - Unsupported orbit source: {orbitDataSource}.");
            }

            setBy = SetBy.CometAsteroidOrbit;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Ephemeris object used by comet and asteroid calculations.
        /// </summary>
        /// <remarks> This is not used for planet, sun and moon calculations.</remarks>
        public Ephemeris Ephemeris
        {
            get
            {
                return ephemeris;
            }

            set
            {
                ephemeris = value;
                setBy = SetBy.CometAsteroidOrbit;

                // Try to set the name if it has been set in the ephemeris object
                try
                {
                    Name = value.Name;
                }
                catch { }
            }
        }

        /// <summary>
        /// Site latitude (North positive)
        /// </summary>
        public double? SiteLatitude { get; set; }

        /// <summary>
        /// Site longitude (East positive)
        /// </summary>
        public double? SiteLongitude { get; set; }

        /// <summary>
        /// Site height (metres)
        /// </summary>
        public double? SiteHeight { get; set; }

        /// <summary>
        /// Site pressure (Hectopascal or millibar)
        /// </summary>
        public double SitePressure { get; set; } = 1010;

        /// <summary>
        /// Site temperature (Celsius)
        /// </summary>
        public double SiteTemperature { get; set; } = 10.0;

        /// <summary>
        /// True to create coordinates corrected for refraction. (Only used by <see cref="AltAzCoordinates(DateTime)"/>.)
        /// </summary>
        public bool Refraction { get; set; } = false;

        /// <summary>
        /// Name of the body
        /// </summary>
        public string Name { get; set; } = "";

        #endregion

        #region Public methods

        /// <summary>
        /// Return the body's astrometric coordinates at a given time
        /// </summary>
        /// <param name="observationTime">Observation time (local or UTC)</param>
        /// <returns>Coordinates class containing the astrometric right ascension, declination and distance to the object.</returns>
        /// <remarks>The supplied DateTime value can be either a local time (DateTime.Kind = Local or Unspecified) or a UTC time (DateTime.Kind = Utc)</remarks>
        public Coordinates AstrometricCoordinates(DateTime observationTime)
        {
            // Calculate UTC and Terrestrial time Julian day numbers
            double julianDateUtc = Utilities.JulianDateFromDateTime(observationTime.ToUniversalTime());
            double julianDateTT = julianDateUtc + Utilities.DeltaT(julianDateUtc) / 86400.0;

            // Calculate Terrestrial time date-time value
            DateTime observationTimeTT = observationTime.AddSeconds(Utilities.DeltaT(julianDateUtc));

            // Initialise a Coordinates object to receive the result
            Coordinates result = new Coordinates();

            switch (setBy)
            {
                case SetBy.None:
                    throw new HelperException($"SolarSystem.{nameof(AstrometricCoordinates)} - No planet comet or asteroid has been set.");

                case SetBy.CometAsteroidOrbit:
                    // Get earth's heliocentric position
                    SolarSystemBody earth = new SolarSystemBody(Body.Earth);
                    BodyPositionVelocity sunToEarthPv = earth.HelioCentricPosition(observationTimeTT);

                    // Get the body's heliocentric position
                    double[] SunToBodyPv = Ephemeris.GetPositionAndVelocity(julianDateTT);

                    // Get earth's heliocentric position 
                    double[] earthToBodyPv = new double[3];

                    // Calculate the body position relative to earth geocentre
                    earthToBodyPv[0] = SunToBodyPv[0] - sunToEarthPv.X;
                    earthToBodyPv[1] = SunToBodyPv[1] - sunToEarthPv.Y;
                    earthToBodyPv[2] = SunToBodyPv[2] - sunToEarthPv.Z;

                    // Correct the object's position vector for the observer's position relative to the earth geocentre
                    try
                    {
                        // Check whether site parameters are available.
                        // If not, an exception is thrown, this step is abandoned and the geocentric position vector is used in place of the observer-centric position vector
                        ValidateSiteLocation(nameof(AstrometricCoordinates));

                        // Create an observer structure on the surface of the earth
                        Observer observer = new Observer();
                        Novas.MakeObserverOnSurface(SiteLatitude.Value, SiteLongitude.Value, SiteHeight.Value, SiteTemperature, SitePressure, ref observer);

                        double[] earthToObserverPv = new double[3] { 0.0, 0.0, 0.0 };
                        double[] observerVelocity = new double[3] { 0.0, 0.0, 0.0 };

                        // Get the position vector of the observer on the earth's surface relative to the geocentre
                        short geoPosVelRc = Novas.GeoPosVel(julianDateTT, Utilities.DeltaT(julianDateUtc), Accuracy.Full, observer, ref earthToObserverPv, ref observerVelocity);

                        // Throw an exception if something went wrong with the calculation
                        if (geoPosVelRc > 0)
                            throw new HelperException($"SolarSystem.{nameof(AstrometricCoordinates)} - GeoPosVel returned a non-zero code: {geoPosVelRc}");

                        // Update the earth to body position vector with the observer to body position vector
                        earthToBodyPv[0] = earthToBodyPv[0] - earthToObserverPv[0];
                        earthToBodyPv[1] = earthToBodyPv[1] - earthToObserverPv[1];
                        earthToBodyPv[2] = earthToBodyPv[2] - earthToObserverPv[2];
                    }
                    catch { }

                    short vector2RaDecRc = Novas.Vector2RaDec(earthToBodyPv, ref result.RightAscension, ref result.Declination);
                    if (vector2RaDecRc > 0)
                        throw new HelperException($"SolarSystem.{nameof(AstrometricCoordinates)} - Vector2RaDec returned a non-zero code: {vector2RaDecRc}");

                    // Return the result
                    return result;

                case SetBy.PlanetSunMoon:

                    // Validate that the selected body is not earth
                    ValidateBodyIsNotEarth(nameof(AstrometricCoordinates));

                    // Create a planet object
                    Object3 planet = new Object3
                    {
                        Type = ObjectType.MajorPlanetSunOrMoon,
                        Number = body
                    };

                    //Calculate the astrometric position of the planet, placing the result RA/Dec in results object
                    short rc = Novas.AstroPlanet(julianDateTT, planet, Accuracy.Full, ref result.RightAscension, ref result.Declination, ref result.Distance);

                    // Throw an exception if the calculation failed for some reason
                    if (rc != 0)
                        throw new HelperException($"SolarSystem.{nameof(AstrometricCoordinates)} - Error in Novas.AstroPlanet, return code: {rc}");

                    // Return the result
                    return result;

                default:
                    throw new HelperException($"SolarSystem.{nameof(AstrometricCoordinates)} - The body type has not been set");
            }
        }

        /// <summary>
        /// Return the body's topocentric coordinates at a given time
        /// </summary>
        /// <param name="observationTime">Observation time</param>
        /// <returns>Coordinates struct containing the topocentric right ascension and declination</returns>
        public Coordinates TopocentricCoordinates(DateTime observationTime)
        {
            // Calculate UTC and Terrestrial time Julian day numbers
            double julianDateUtc = Utilities.JulianDateFromDateTime(observationTime.ToUniversalTime());
            double julianDateTT = julianDateUtc + Utilities.DeltaT(julianDateUtc) / 86400.0;

            switch (setBy)
            {
                case SetBy.None:
                    throw new HelperException($"SolarSystem.{nameof(AstrometricCoordinates)} - No planet comet or asteroid has been set.");

                case SetBy.PlanetSunMoon:
                    // Validate that the selected body is not earth
                    ValidateBodyIsNotEarth(nameof(TopocentricCoordinates));

                    // Confirm that required parameters have been set
                    ValidateSiteLocation(nameof(TopocentricCoordinates));

                    // Create an OnSurface object
                    OnSurface location = new OnSurface
                    {
                        Latitude = SiteLatitude.Value,
                        Longitude = SiteLongitude.Value,
                        Height = SiteHeight.Value,
                        Temperature = SiteTemperature,
                        Pressure = SitePressure
                    };

                    // Create a planet object
                    Object3 planet = new Object3
                    {
                        Type = ObjectType.MajorPlanetSunOrMoon,
                        Number = body
                    };

                    // Initialise a Coordinates object to receive the result
                    Coordinates topocentricCoordinates = new Coordinates();

                    //Calculate the topocentric position of the planet, placing the result RA/Dec in results object
                    short rc = Novas.TopoPlanet(julianDateTT, planet, Utilities.DeltaT(julianDateUtc), location, Accuracy.Full, ref topocentricCoordinates.RightAscension, ref topocentricCoordinates.Declination, ref topocentricCoordinates.Distance);

                    // Throw an exception if the calculation failed for some reason
                    if (rc != 0)
                        throw new HelperException($"SolarSystem.{nameof(TopocentricCoordinates)} - Error in Novas.TopoPlanet, return code: {rc}");

                    // Return the result
                    return topocentricCoordinates;

                case SetBy.CometAsteroidOrbit:
                    // Confirm that required parameters have been set
                    ValidateSiteLocation(nameof(TopocentricCoordinates));

                    // Get the astrometric RA/Dec
                    Coordinates astrometriCoordinates = AstrometricCoordinates(observationTime);

                    // Create an OnSurface object
                    OnSurface onSurface = new OnSurface
                    {
                        Latitude = SiteLatitude.Value,
                        Longitude = SiteLongitude.Value,
                        Height = SiteHeight.Value,
                        Temperature = SiteTemperature,
                        Pressure = SitePressure
                    };

                    // Create a cat entry object
                    CatEntry3 catEntry3 = new CatEntry3()
                    {
                        RA = astrometriCoordinates.RightAscension,
                        Dec = astrometriCoordinates.Declination
                    };

                    // Initialise a Coordinates object to receive the result
                    topocentricCoordinates = new Coordinates();

                    //Calculate the topocentric position of the asteroid, placing the result RA/Dec in results object
                    short rcAsteroid = Novas.TopoStar(julianDateTT, Utilities.DeltaT(julianDateUtc), catEntry3, onSurface, Accuracy.Full, ref topocentricCoordinates.RightAscension, ref topocentricCoordinates.Declination);

                    // Throw an exception if the calculation failed for some reason
                    if (rcAsteroid != 0)
                        throw new HelperException($"SolarSystem.{nameof(TopocentricCoordinates)} - Error in Novas.TopoPlanet, return code: {rcAsteroid}");

                    // Return the result
                    return topocentricCoordinates;

                default:
                    throw new HelperException($"SolarSystem.{nameof(TopocentricCoordinates)} - The body type has not been set");
            }
        }

        /// <summary>
        /// Return the body's topocentric coordinates at a given time
        /// </summary>
        /// <param name="observationTime">Observation time</param>
        /// <returns>Coordinates struct containing the topocentric right ascension and declination</returns>
        public Coordinates AltAzCoordinates(DateTime observationTime)
        {
            // Validate that the selected body is not earth
            ValidateBodyIsNotEarth(nameof(AltAzCoordinates));

            // Confirm that required parameters have been set
            ValidateSiteLocation(nameof(AltAzCoordinates));

            // Calculate UTC and Terrestrial time Julian day numbers
            double julianDateUtc = Utilities.JulianDateFromDateTime(observationTime.ToUniversalTime());
            double julianDateTT = julianDateUtc + Utilities.DeltaT(julianDateUtc) / 86400.0;

            // Create an OnSurface object
            OnSurface location = new OnSurface
            {
                Latitude = SiteLatitude.Value,
                Longitude = SiteLongitude.Value,
                Height = SiteHeight.Value,
                Temperature = SiteTemperature,
                Pressure = SitePressure
            };

            // Create a planet object
            Object3 planet = new Object3
            {
                Type = ObjectType.MajorPlanetSunOrMoon,
                Number = body
            };

            // Get the object topocentric coordinates
            Coordinates topocentricCoordinates = TopocentricCoordinates(observationTime);

            // Create a NOVAS refraction option value depending on the state of the Refraction field
            RefractionOption refractionOption = Refraction ? RefractionOption.LocationRefraction : RefractionOption.NoRefraction;

            // Initialise a Coordinates class to receive the result
            Coordinates result = new Coordinates();

            // The Equ2Hor method returns a zenith distance rather than an altitude so initialise a variable to receive this
            double zenithDistance = 0.0;

            // Calculate AltAz coordinates from the topocentric coordinates
            Novas.Equ2Hor(julianDateUtc, Utilities.DeltaT(julianDateUtc), Accuracy.Full, 0.0, 0.0, location, topocentricCoordinates.RightAscension, topocentricCoordinates.Declination, refractionOption, ref zenithDistance, ref result.Azimuth, ref result.RightAscension, ref result.Declination);

            // Add the altitude calculated from the returned zenith distance
            result.Altitude = 90.0 - zenithDistance;

            //Add the distance from the topocentric coordinate calculation
            result.Distance = topocentricCoordinates.Distance;

            // Return the result
            return result;
        }

        /// <summary>
        /// Returns the body's heliocentric position and velocity vectors
        /// </summary>
        /// <param name="observationTime">Observation time</param>
        /// <returns>BodyPositionAndVelocity object</returns>
        public BodyPositionVelocity HelioCentricPosition(DateTime observationTime)
        {
            double[] position = new double[3] { 0.0, 0.0, 0.0 };
            double[] velocity = new double[3] { 0.0, 0.0, 0.0 };

            // Calculate UTC and Terrestrial time Julian day numbers
            double julianDateUtc = Utilities.JulianDateFromDateTime(observationTime.ToUniversalTime());
            double julianDateTT = julianDateUtc + Utilities.DeltaT(julianDateUtc) / 86400.0;

            double[] helioCentricPositionVelocity = ephemeris.GetPositionAndVelocity(julianDateTT);

            // Return the body's heliocentric vectors
            return new BodyPositionVelocity(helioCentricPositionVelocity);
        }

        /// <summary>
        /// Returns the body's geocentric position and velocity vectors
        /// </summary>
        /// <param name="observationTime">Observation time</param>
        /// <returns>BodyPositionAndVelocity object</returns>
        /// <exception cref="HelperException">If the NOVAS calculation fails.</exception>
        public BodyPositionVelocity GeoCentricPosition(DateTime observationTime)
        {
            double[] earthHeliocentricPv = new double[3] { 0.0, 0.0, 0.0 };
            double[] earthHeliocentricVv = new double[3] { 0.0, 0.0, 0.0 };

            double[] earthBodyPVv = new double[6];

            // Calculate UTC and Terrestrial time Julian day numbers
            double julianDateUtc = Utilities.JulianDateFromDateTime(observationTime.ToUniversalTime());
            double julianDateTT = julianDateUtc + Utilities.DeltaT(julianDateUtc) / 86400.0;

            // Get the body's heliocentric position and velocity
            double[] bodyHelioCentricPVv = ephemeris.GetPositionAndVelocity(julianDateTT);

            // Get earth's heliocentric position and velocity and throw an exception if the calculation failed for some reason
            short rc = Novas.SolarSystem(julianDateTT, Body.Earth, Origin.Heliocentric, ref earthHeliocentricPv, ref earthHeliocentricVv);
            if (rc != 0)
                throw new HelperException($"SolarSystem.{nameof(GeoCentricPosition)} - Error in Novas.SolarSystem, return code: {rc}");

            // Calculate the body's geocentric position and velocity
            earthBodyPVv[0] = bodyHelioCentricPVv[0] - earthHeliocentricPv[0];
            earthBodyPVv[1] = bodyHelioCentricPVv[1] - earthHeliocentricPv[1];
            earthBodyPVv[2] = bodyHelioCentricPVv[2] - earthHeliocentricPv[2];
            earthBodyPVv[3] = bodyHelioCentricPVv[3] - earthHeliocentricVv[0];
            earthBodyPVv[4] = bodyHelioCentricPVv[4] - earthHeliocentricVv[1];
            earthBodyPVv[5] = bodyHelioCentricPVv[5] - earthHeliocentricVv[2];

            // Return the body's geocentric vectors
            return new BodyPositionVelocity(earthBodyPVv);
        }

        #endregion

        #region Support code

        private void ValidateSiteLocation(string methodName)
        {
            if (!SiteLatitude.HasValue)
                throw new InvalidOperationException($"SolarSystem.{methodName} - Site latitude has not been set.");

            if (!SiteLongitude.HasValue)
                throw new InvalidOperationException($"SolarSystem.{methodName} - Site longitude has not been set.");

            if (!SiteHeight.HasValue)
                throw new InvalidOperationException($"SolarSystem.{methodName} - Site height has not been set.");
        }

        private void ValidateBodyIsNotEarth(string methodName)
        {
            if (body == Body.Earth)
                throw new InvalidOperationException($"SolarSystem.{methodName} - Cannot use {methodName} when the target body is Earth.");
        }

        private static double GetParameter(string parameterName, int start, int length, string orbitString)
        {
            if (!Double.TryParse(orbitString.Substring(start, length), NumberStyles.Number, CultureInfo.InvariantCulture, out double parsedValue))
                throw new HelperException($"SolarSystem.{nameof(GetParameter)} - Unable to parse the {parameterName} value at position {start} for {length} characters '{orbitString.Substring(start, length)}' from '{orbitString}'");
            return parsedValue;
        }

        private static int GetIntParameter(string parameterName, int start, int length, string orbitString)
        {
            if (!Int32.TryParse(orbitString.Substring(start, length), NumberStyles.Number, CultureInfo.InvariantCulture, out int parsedValue))
                throw new HelperException($"SolarSystem.{nameof(GetIntParameter)} - Unable to parse the {parameterName} value starting at position {start} for {length} characters from {orbitString}");
            return parsedValue;
        }

        private static double PackedToJulian(string packed)
        {
            const string PCODE = "123456789ABCDEFGHIJKLMNOPQRSTUV";
            const string YCODE = "IJK";

            int year = (18 + YCODE.IndexOf(packed[0])) * 100;
            year += int.Parse(packed.Substring(1, 2));
            int month = PCODE.IndexOf(packed[3]) + 1;
            int day = PCODE.IndexOf(packed[4]) + 1;
            double julianDate = Utilities.JulianDateFromDateTime(new DateTime(year, month, day));
            return julianDate;
        }

        static void LogMessage(string message)
        {
            Console.WriteLine(message);
        }

        #endregion

    }
}
