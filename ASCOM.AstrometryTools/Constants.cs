using System;
using System.Runtime.InteropServices;

namespace ASCOM.Tools.Novas31
{

    static class Constants
    {
        internal const double LEAP_SECONDS_DEFAULT = 37.0; // Current number of leap seconds

        // Physical constants
        internal const double MOON_RADIUS = 1737.0d; // km
        internal const double EARTH_RADIUS = 6378.0d; // km
        internal const double SUN_RADIUS = 696342.0d; // km
        internal const double MERCURY_RADIUS = 2439.7d; // km
        internal const double VENUS_RADIUS = 2439.7d; // km
        internal const double MARS_RADIUS = 3396.2d; // km
        internal const double JUPITER_RADIUS = 69911.0d; // km
        internal const double SATURN_RADIUS = 6051.8d; // km
        internal const double NEPTUNE_RADIUS = 24767.0d; // km
        internal const double URANUS_RADIUS = 24973.0d; // km
        internal const double PLUTO_RADIUS = 1153.0d; // km

        // Fixed event definitions
        internal const double SUN_RISE = -50.0d / 60.0d; // Degrees
        internal const double CIVIL_TWILIGHT = -6.0d; // Degrees
        internal const double NAUTICAL_TWILIGHT = -12.0d; // Degrees
        internal const double AMATEUR_ASRONOMICAL_TWILIGHT = -15.0d; // Degrees
        internal const double ASTRONOMICAL_TWILIGHT = -18.0d; // Degrees

        // Conversion factors
        internal const double HOURS2DEG = 15.0d;
        internal const double DEG2HOURS = 1.0d / 15.0d;
        internal const double DEG2HOURSSOLSID = 1.0d / 15.04107d;
        internal const double SECONDS2DAYS = 1.0d / (60.0d * 60.0d * 24.0d);
        internal const double AU2KILOMETRE = 149597870.691d;

        // NOVAS.COM Constants
        internal const double DEG2RAD = 0.017453292519943295d;
        internal const double RAD2DEG = 57.295779513082323d;

        // General constants
        internal const double OLE_AUTOMATION_JULIAN_DATE_OFFSET = 2415018.5d; // Offset of OLE automation dates from Julian dates
        internal const double JULIAN_DAY_WHEN_GREGORIAN_CALENDAR_WAS_INTRODUCED = 2299161.0; // Julian day number of the day on which the Gregorian calendar was first used - 15th October 1582

        internal const double RACIO_DEFAULT_VALUE = double.NaN; // NOVAS3: Default value that if still present will indicate that this value was not updated

        // Profile store Key names
        internal const string ASTROMETRY_SUBKEY = "Astrometry";
    }

    #region Utilities Enums and Structures

    /// <summary>
    /// Type of event for which an ephemeris is required
    /// </summary>
    /// <remarks></remarks>
    public enum EventType : int
    {
        /// <summary>
        /// 
        /// </summary>
        SunRiseSunset = 0,
        /// <summary>
        /// 
        /// </summary>
        MoonRiseMoonSet = 1,
        /// <summary>
        /// 
        /// </summary>
        CivilTwilight = 2,
        /// <summary>
        /// 
        /// </summary>
        NauticalTwilight = 3,
        /// <summary>
        /// 
        /// </summary>
        AmateurAstronomicalTwilight = 4,
        /// <summary>
        /// 
        /// </summary>
        AstronomicalTwilight = 5,
        /// <summary>
        /// 
        /// </summary>
        MercuryRiseSet = 6,
        /// <summary>
        /// 
        /// </summary>
        VenusRiseSet = 7,
        /// <summary>
        /// 
        /// </summary>
        MarsRiseSet = 8,
        /// <summary>
        /// 
        /// </summary>
        JupiterRiseSet = 9,
        /// <summary>
        /// 
        /// </summary>
        SaturnRiseSet = 10,
        /// <summary>
        /// 
        /// </summary>
        UranusRiseSet = 11,
        /// <summary>
        /// 
        /// </summary>
        NeptuneRiseSet = 12,
        /// <summary>
        /// 
        /// </summary>
        PlutoRiseSet = 13
    }

    #endregion

    #region NOVAS Enums

    /// <summary>
    /// Type of body, Major Planet, Moon, Sun or Minor Planet
    /// </summary>
    /// <remarks></remarks>
    public enum BodyType : int
    {
        /// <summary>
        /// Luna
        /// </summary>
        /// <remarks></remarks>
        Moon = 0,

        /// <summary>
        /// The Sun
        /// </summary>
        /// <remarks></remarks>
        Sun = 0,

        /// <summary>
        /// Major planet
        /// </summary>
        /// <remarks></remarks>
        MajorPlanet = 0,

        /// <summary>
        /// Minor planet
        /// </summary>
        /// <remarks></remarks>
        MinorPlanet = 1,

        /// <summary>
        /// Comet
        /// </summary>
        /// <remarks></remarks>
        Comet = 2
    }

    /// <summary>
    /// Co-ordinate origin: centre of Sun or solar system barycentre
    /// </summary>
    /// <remarks></remarks>
    public enum Origin : int
    {
        /// <summary>
        /// Centre of mass of the solar system
        /// </summary>
        Barycentric = 0,
        /// <summary>
        /// Centre of mass of the Sun
        /// </summary>
        Heliocentric = 1,
        /// <summary>
        /// Centre of mass of the earth
        /// </summary>
        GeoCentric=2
    }

    /// <summary>
    /// Type of refraction correction
    /// </summary>
    /// <remarks></remarks>
    public enum RefractionOption : int
    {
        /// <summary>
        /// No refraction correction will be applied
        /// </summary>
        /// <remarks></remarks>
        NoRefraction = 0,
        /// <summary>
        /// Refraction will be applied based on "standard" weather values of temperature = 10.0C and sea level pressure = 1010 millibar
        /// </summary>
        /// <remarks></remarks>
        StandardRefraction = 1,
        /// <summary>
        /// Refraction will be applied based on the temperature and pressure supplied in the site location structure
        /// </summary>
        /// <remarks></remarks>
        LocationRefraction = 2
    }

    /// <summary>
    /// Direction of nutation correction
    /// </summary>
    /// <remarks></remarks>
    public enum NutationDirection : int
    {
        /// <summary>
        /// Convert mean equator and equinox to true equator and equinox
        /// </summary>
        /// <remarks></remarks>
        MeanToTrue = 0,
        /// <summary>
        /// Convert true equator and equinox to mean equator and equinox
        /// </summary>
        /// <remarks></remarks>
        TrueToMean = 1
    }

    /// <summary>
    /// Direction of transformation: ITRS to Terrestrial Intermediate or vice versa
    /// </summary>
    /// <remarks></remarks>
    public enum TransformationDirection : short
    {
        /// <summary>
        /// 
        /// </summary>
        ITRSToTerrestrialIntermediate = 0,
        /// <summary>
        /// 
        /// </summary>
        TerrestrialIntermediateToITRS = 1
    }
    /// <summary>
    /// Location of observer
    /// </summary>
    /// <remarks></remarks>
    public enum ObserverLocation : short
    {
        /// <summary>
        /// Observer at centre of the earth
        /// </summary>
        /// <remarks></remarks>
        EarthGeoCenter = 0,
        /// <summary>
        /// Observer on earth's surface
        /// </summary>
        /// <remarks></remarks>
        EarthSurface = 1,
        /// <summary>
        /// Observer in near-earth spacecraft
        /// </summary>
        /// <remarks></remarks>
        SpaceNearEarth = 2
    }

    /// <summary>
    /// Calculation accuracy
    /// </summary>
    /// <remarks>
    /// In full-accuracy mode,
    /// <list type="bullet">
    /// <item>nutation calculations use the IAU 2000A model [iau2000a, nutation_angles];</item>
    /// <item>gravitational deflection is calculated using three bodies: Sun, Jupiter, and Saturn [grav_def];</item>
    /// <item>the equation of the equinoxes includes the entire series when computing the “complementary terms" [ee_ct];</item>
    /// <item>geocentric positions of solar system bodies are adjusted for light travel time using split, or two-part, 
    /// Julian dates in calls to ephemeris and iterate with a convergence tolerance of 10-12 days [light_time, ephemeris];</item>
    /// <item>ephemeris calls the appropriate solar system ephemeris using split, or two-part, Julian dates primarily to support 
    /// light-time calculations [ephemeris, solarsystem_hp, light_time].</item>
    /// </list>
    /// <para>In reduced-accuracy mode,</para>
    /// <list type="bullet">
    /// <item>nutation calculations use the 2000K model, which is the default for this mode;</item>
    /// <item>gravitational deflection is calculated using only one body, the Sun [grav_def];</item>
    /// <item>the equation of the equinoxes excludes terms smaller than 2 micro arc seconds when computing the "complementary terms" [ee_ct];</item>
    /// <item>geocentric positions of solar system bodies are adjusted for light travel time using single-value Julian dates 
    /// in calls to ephemeris and iterate with a convergence tolerance of 10-9 days [light-time, ephemeris, solar system];</item>
    /// <item>ephemeris calls the appropriate solar system ephemeris using single-value Julian dates [ephemeris, solar system].</item>
    /// </list>
    /// <para>In full-accuracy mode, the IAU 2000A nutation series (1,365 terms) is used [iau2000a]. Evaluating the series for nutation is 
    /// usually the main computational burden in NOVAS, so using reduced-accuracy mode improves execution time, often noticeably. 
    /// In reduced-accuracy mode, the NOVAS 2000K nutation series (488 terms) is used by default [nu2000k]. This mode can be used 
    /// when the accuracy requirements are not better than 0.1 milliarcsecond for stars or 3.5 milli arc-seconds for solar system bodies. 
    /// Selecting this approach can reduce the time required for Earth-rotation computations by about two-thirds.</para>
    /// </remarks>
    public enum Accuracy : short
    {
        /// <summary>
        /// Full accuracy
        /// </summary>
        /// <remarks>Suitable when precision of better than 0.1 milli arc-second for stars or 3.5 milli arc-seconds for solar system bodies is required.</remarks>
        Full = 0, // ... full accuracy
        /// <summary>
        /// Reduced accuracy
        /// </summary>
        /// <remarks>Suitable when precision of less than 0.1 milli arc-second for stars or 3.5 milli arc-seconds for solar system bodies is required.</remarks>
        Reduced = 1 // ... reduced accuracy
    }

    /// <summary>
    /// Coordinate system of the output position
    /// </summary>
    /// <remarks>Used by function Place</remarks>
    public enum CoordSys : short
    {
        /// <summary>
        /// GCRS or "local GCRS"
        /// </summary>
        /// <remarks></remarks>
        GCRS = 0,
        /// <summary>
        /// True equator and equinox of date
        /// </summary>
        /// <remarks></remarks>
        EquinoxOfDate = 1,
        /// <summary>
        /// True equator and CIO of date
        /// </summary>
        /// <remarks></remarks>
        CIOOfDate = 2,
        /// <summary>
        /// Astrometric coordinates, i.e., without light deflection or aberration.
        /// </summary>
        /// <remarks></remarks>
        Astrometric = 3
    }

    /// <summary>
    /// Type of sidereal time
    /// </summary>
    /// <remarks></remarks>
    public enum GstType : short
    {
        /// <summary>
        /// Greenwich mean sidereal time
        /// </summary>
        /// <remarks></remarks>
        GreenwichMeanSiderealTime = 0,
        /// <summary>
        /// Greenwich apparent sidereal time
        /// </summary>
        /// <remarks></remarks>
        GreenwichApparentSiderealTime = 1
    }

    /// <summary>
    /// Computation method
    /// </summary>
    /// <remarks></remarks>
    public enum Method : short
    {
        /// <summary>
        /// Based on CIO
        /// </summary>
        /// <remarks></remarks>
        CIOBased = 0,
        /// <summary>
        /// Based on equinox
        /// </summary>
        /// <remarks></remarks>
        EquinoxBased = 1
    }

    /// <summary>
    /// Output vector reference system
    /// </summary>
    /// <remarks></remarks>
    public enum OutputVectorOption : short
    {
        /// <summary>
        /// Referred to GCRS axes
        /// </summary>
        /// <remarks></remarks>
        ReferredToGCRSAxes = 0,
        /// <summary>
        /// Referred to the equator and equinox of date
        /// </summary>
        /// <remarks></remarks>
        ReferredToEquatorAndEquinoxOfDate = 1
    }

    /// <summary>
    /// Type of pole offset
    /// </summary>
    /// <remarks>Used by CelPole.</remarks>
    public enum PoleOffsetCorrection : short
    {
        /// <summary>
        /// For corrections to angular coordinates of modelled pole referred to mean ecliptic of date, that is, delta-delta-psi 
        /// and delta-delta-epsilon. 
        /// </summary>
        /// <remarks></remarks>
        ReferredToMeanEclipticOfDate = 1,
        /// <summary>
        /// For corrections to components of modelled pole unit vector referred to GCRS axes, that is, dx and dy.
        /// </summary>
        /// <remarks></remarks>
        ReferredToGCRSAxes = 2
    }

    /// <summary>
    /// Direction of frame conversion
    /// </summary>
    /// <remarks>Used by FrameTie method.</remarks>
    public enum FrameConversionDirection : short
    {
        /// <summary>
        /// Dynamical to ICRS transformation.
        /// </summary>
        /// <remarks></remarks>
        DynamicalToICRS = -1,
        /// <summary>
        /// ICRS to dynamical transformation.
        /// </summary>
        /// <remarks></remarks>
        ICRSToDynamical = 1
    }

    /// <summary>
    /// Location of observer, determining whether the gravitational deflection due to the earth itself is applied.
    /// </summary>
    /// <remarks>Used by GravDef method.</remarks>
    public enum EarthDeflection : short
    {
        /// <summary>
        /// No earth deflection (normally means observer is at geocenter)
        /// </summary>
        /// <remarks></remarks>
        NoEarthDeflection = 0,
        /// <summary>
        /// Add in earth deflection (normally means observer is on or above surface of earth, including earth orbit)
        /// </summary>
        /// <remarks></remarks>
        AddEarthDeflection = 1
    }

    /// <summary>
    /// Reference system in which right ascension is given
    /// </summary>
    /// <remarks></remarks>
    public enum ReferenceSystem : short
    {
        /// <summary>
        /// GCRS
        /// </summary>
        /// <remarks></remarks>
        GCRS = 1,
        /// <summary>
        /// True equator and equinox of date
        /// </summary>
        /// <remarks></remarks>
        TrueEquatorAndEquinoxOfDate = 2
    }

    /// <summary>
    /// Type of equinox
    /// </summary>
    /// <remarks></remarks>
    public enum EquinoxType : short
    {
        /// <summary>
        /// Mean equinox
        /// </summary>
        /// <remarks></remarks>
        MeanEquinox = 0,
        /// <summary>
        /// True equinox
        /// </summary>
        /// <remarks></remarks>
        TrueEquinox = 1
    }

    /// <summary>
    /// Type of transformation
    /// </summary>
    /// <remarks></remarks>
    public enum TransformationOption3 : short
    {
        /// <summary>
        /// Change epoch only
        /// </summary>
        /// <remarks></remarks>
        ChangeEpoch = 1,
        /// <summary>
        /// Change equator and equinox; sane epoch
        /// </summary>
        /// <remarks></remarks>
        ChangeEquatorAndEquinox = 2,
        /// <summary>
        /// Change equator, equinox and epoch
        /// </summary>
        /// <remarks></remarks>
        ChangeEquatorAndEquinoxAndEpoch = 3,
        /// <summary>
        /// change equator and equinox J2000.0 to ICRS
        /// </summary>
        /// <remarks></remarks>
        ChangeEquatorAndEquinoxJ2000ToICRS = 4,
        /// <summary>
        /// change ICRS to equator and equinox of J2000.0
        /// </summary>
        /// <remarks></remarks>
        ChangeICRSToEquatorAndEquinoxOfJ2000 = 5
    }

    /// <summary>
    /// Type of object
    /// </summary>
    /// <remarks></remarks>
    public enum ObjectType : short
    {
        /// <summary>
        /// Major planet, sun or moon
        /// </summary>
        /// <remarks></remarks>
        MajorPlanetSunOrMoon = 0,
        /// <summary>
        /// Minor planet
        /// </summary>
        /// <remarks></remarks>
        MinorPlanet = 1,
        /// <summary>
        /// Object located outside the solar system
        /// </summary>
        /// <remarks></remarks>
        ObjectLocatedOutsideSolarSystem = 2
    }

    /// <summary>
    /// Body or location
    /// </summary>
    /// <remarks>This numbering convention is used by ephemeris routines; do not confuse with the Body enum, which is used in most 
    /// other places within NOVAS3.
    /// <para>
    /// The numbering convention for 'target' and 'centre' is:
    /// <pre>
    ///             0  =  Mercury           7 = Neptune
    ///             1  =  Venus             8 = Pluto
    ///             2  =  Earth             9 = Moon
    ///             3  =  Mars             10 = Sun
    ///             4  =  Jupiter          11 = Solar system barycentre.
    ///             5  =  Saturn           12 = Earth-Moon barycentre.
    ///             6  =  Uranus           13 = Nutations (long. and oblique.)</pre>
    /// </para>
    /// <para>
    /// If nutations are desired, set 'target' = 14; 'centre' will be ignored on that call.
    /// </para>
    /// </remarks>
    public enum Target : short
    {
        /// <summary>
        /// Mercury
        /// </summary>
        /// <remarks></remarks>
        Mercury = 0,
        /// <summary>
        /// Venus
        /// </summary>
        /// <remarks></remarks>
        Venus = 1,
        /// <summary>
        /// Earth
        /// </summary>
        /// <remarks></remarks>
        Earth = 2,
        /// <summary>
        /// Mars
        /// </summary>
        /// <remarks></remarks>
        Mars = 3,
        /// <summary>
        /// Jupiter
        /// </summary>
        /// <remarks></remarks>
        Jupiter = 4,
        /// <summary>
        /// Saturn
        /// </summary>
        /// <remarks></remarks>
        Saturn = 5,
        /// <summary>
        /// Uranus
        /// </summary>
        /// <remarks></remarks>
        Uranus = 6,
        /// <summary>
        /// Neptune
        /// </summary>
        /// <remarks></remarks>
        Neptune = 7,
        /// <summary>
        /// Pluto
        /// </summary>
        /// <remarks></remarks>
        Pluto = 8,
        /// <summary>
        /// Moon
        /// </summary>
        /// <remarks></remarks>
        Moon = 9,
        /// <summary>
        /// Sun
        /// </summary>
        /// <remarks></remarks>
        Sun = 10,
        /// <summary>
        /// Solar system barycentre
        /// </summary>
        /// <remarks></remarks>
        SolarSystemBarycentre = 11,
        /// <summary>
        /// Earth moon barycentre
        /// </summary>
        /// <remarks></remarks>
        EarthMoonBarycentre = 12,
        /// <summary>
        /// Nutations
        /// </summary>
        /// <remarks></remarks>
        Nutations = 13
    }

    #endregion

    #region Public NOVAS Structures
    /// <summary>
    /// Structure to hold a position vector
    /// </summary>
    /// <remarks>Object position vector
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct PosVector
    {
        /// <summary>
        /// x co-ordinate
        /// </summary>
        /// <remarks></remarks>
        public double x;
        /// <summary>
        /// y co-ordinate
        /// </summary>
        /// <remarks></remarks>
        public double y;
        /// <summary>
        /// z co-ordinate
        /// </summary>
        /// <remarks></remarks>
        public double z;
    }

    /// <summary>
    /// Structure to hold a velocity vector
    /// </summary>
    /// <remarks>Object velocity vector
    /// </remarks>
    public struct VelVector
    {
        /// <summary>
        /// x velocity component 
        /// </summary>
        /// <remarks></remarks>
        public double x;
        /// <summary>
        /// y velocity component
        /// </summary>
        /// <remarks></remarks>
        public double y;
        /// <summary>
        /// z velocity component
        /// </summary>
        /// <remarks></remarks>
        public double z;
    }

    /// <summary>
    /// Structure to hold Sun and Moon fundamental arguments
    /// </summary>
    /// <remarks>Fundamental arguments, in radians
    /// </remarks>
    public struct FundamentalArgs
    {
        /// <summary>
        /// l (mean anomaly of the Moon)
        /// </summary>
        /// <remarks></remarks>
        public double l;
        /// <summary>
        /// l' (mean anomaly of the Sun)
        /// </summary>
        /// <remarks></remarks>
        public double ldash;
        /// <summary>
        /// F (L - omega; L = mean longitude of the Moon)
        /// </summary>
        /// <remarks></remarks>
        public double F;
        /// <summary>
        /// D (mean elongation of the Moon from the Sun)
        /// </summary>
        /// <remarks></remarks>
        public double D;
        /// <summary>
        /// Omega (mean longitude of the Moon's ascending node)
        /// </summary>
        /// <remarks></remarks>
        public double Omega;
    }

    /// <summary>
    /// Catalogue entry structure
    /// </summary>
    /// <remarks>Basic astrometric data for any celestial object located outside the solar system; the catalogue data for a star.
    /// <para>This structure is identical to the NOVAS2 CatEntry structure expect that, for some reason, the StarName and Catalogue fields
    /// have been swapped in the NOVAS3 structure.</para>
    /// <para>
    /// Please note that some units have changed from those used in NOVAS2 as follows:
    /// <list type="bullet">
    /// <item>proper motion in right ascension: from seconds per century to milliarcseconds per year</item>
    /// <item>proper motion in declination: from arc seconds per century to milliarcseconds per year</item>
    /// <item>parallax: from arc seconds to milliarcseconds</item>
    /// </list>
    /// </para>
    /// </remarks>
    public struct CatEntry3
    {
        /// <summary>
        /// Name of celestial object. (maximum 50 characters)
        /// </summary>
        /// <remarks></remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 51)]
        public string StarName;

        /// <summary>
        /// 3-character catalogue designator. 
        /// </summary>
        /// <remarks></remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string Catalog;

        /// <summary>
        /// Integer identifier assigned to object.
        /// </summary>
        /// <remarks></remarks>
        public int StarNumber;

        /// <summary>
        /// ICRS right ascension (hours)
        /// </summary>
        /// <remarks></remarks>
        public double RA;

        /// <summary>
        /// ICRS declination (degrees)
        /// </summary>
        /// <remarks></remarks>
        public double Dec;

        /// <summary>
        /// ICRS proper motion in right ascension (milliarcseconds/year)
        /// </summary>
        /// <remarks></remarks>
        public double ProMoRA;

        /// <summary>
        /// ICRS proper motion in declination (milliarcseconds/year)
        /// </summary>
        /// <remarks></remarks>
        public double ProMoDec;

        /// <summary>
        /// Parallax (milliarcseconds)
        /// </summary>
        /// <remarks></remarks>
        public double Parallax;

        /// <summary>
        /// Radial velocity (km/s)
        /// </summary>
        /// <remarks></remarks>
        public double RadialVelocity;
    }

    /// <summary>
    /// Celestial object structure
    /// </summary>
    /// <remarks>Designates a celestial object</remarks>
    public struct Object3
    {
        /// <summary>
        /// Type of object
        /// </summary>
        /// <remarks></remarks>
        public ObjectType Type;
        /// <summary>
        /// Object identification number
        /// </summary>
        /// <remarks></remarks>
        public Body Number;
        /// <summary>
        /// Name of object(maximum 50 characters)
        /// </summary>
        /// <remarks></remarks>
        public string Name;
        /// <summary>
        /// Catalogue entry for the object
        /// </summary>
        /// <remarks></remarks>
        public CatEntry3 Star;
    }

    /// <summary>
    /// Celestial object's place in the sky
    /// </summary>
    /// <remarks></remarks>
    public struct SkyPos
    {
        /// <summary>
        /// Unit vector toward object (dimensionless)
        /// </summary>
        /// <remarks></remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.R8)]
        public double[] RHat;
        /// <summary>
        /// Apparent, topocentric, or astrometric right ascension (hours)
        /// </summary>
        /// <remarks></remarks>
        public double RA;
        /// <summary>
        /// Apparent, topocentric, or astrometric declination (degrees)
        /// </summary>
        /// <remarks></remarks>
        public double Dec;
        /// <summary>
        /// True (geometric, Euclidean) distance to solar system body or 0.0 for star (AU)
        /// </summary>
        /// <remarks></remarks>
        public double Dis;
        /// <summary>
        /// Radial velocity (km/s)
        /// </summary>
        /// <remarks></remarks>
        public double RV;
    }

    /// <summary>
    /// Observer’s position and velocity in a near-Earth spacecraft.
    /// </summary>
    /// <remarks></remarks>
    public struct InSpace
    {
        /// <summary>
        /// Geocentric position vector (x, y, z), components in km with respect to true equator and equinox of date
        /// </summary>
        /// <remarks></remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.R8)]
        public double[] ScPos;
        /// <summary>
        /// Geocentric velocity vector (x_dot, y_dot, z_dot), components in km/s with respect to true equator and equinox of date
        /// </summary>
        /// <remarks></remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.R8)]
        public double[] ScVel;
    }

    /// <summary>
    /// Right ascension of the Celestial Intermediate Origin (CIO) with respect to the GCRS.
    /// </summary>
    /// <remarks></remarks>
    public struct RAOfCio
    {
        /// <summary>
        /// TDB Julian date
        /// </summary>
        /// <remarks></remarks>
        public double JdTdb;
        /// <summary>
        /// Right ascension of the CIO with respect to the GCRS (arc seconds)
        /// </summary>
        /// <remarks></remarks>
        public double RACio;
    }

    /// <summary>
    /// Parameters of observer's location
    /// </summary>
    /// <remarks>This structure is identical to the NOVAS2 SiteInfo structure but is included so that NOVAS3 naming
    /// conventions are maintained, making it easier to relate this code to the NOVAS3 documentation and C code.</remarks>
    public struct OnSurface
    {
        /// <summary>
        /// Geodetic (ITRS) latitude; north positive (degrees)
        /// </summary>
        /// <remarks></remarks>
        public double Latitude;
        /// <summary>
        /// Geodetic (ITRS) longitude; east positive (degrees)
        /// </summary>
        /// <remarks></remarks>
        public double Longitude;
        /// <summary>
        /// Observer's height above sea level
        /// </summary>
        /// <remarks></remarks>
        public double Height;
        /// <summary>
        /// Observer's location's ambient temperature (degrees Celsius)
        /// </summary>
        /// <remarks></remarks>
        public double Temperature;
        /// <summary>
        /// Observer's location's atmospheric pressure (millibars)
        /// </summary>
        /// <remarks></remarks>
        public double Pressure;
    }

    /// <summary>
    /// General specification for the observer's location
    /// </summary>
    /// <remarks></remarks>
    public struct Observer
    {
        /// <summary>
        /// Code specifying the location of the observer: 0=at geocenter; 1=surface of earth; 2=near-earth spacecraft
        /// </summary>
        /// <remarks></remarks>
        public ObserverLocation Where;
        /// <summary>
        /// Data for an observer's location on the surface of the Earth (where = 1)
        /// </summary>
        /// <remarks></remarks>
        public OnSurface OnSurf;
        /// <summary>
        /// Data for an observer's location on a near-Earth spacecraft (where = 2)
        /// </summary>
        /// <remarks></remarks>
        public InSpace NearEarth;
    }

    #endregion

    #region Internal NOVAS3 Structures
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct JDHighPrecision
    {
        public double JDPart1;
        public double JDPart2;
    }

    // Internal version of Object3 with correct marshalling hints and type for Number field
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct Object3Internal
    {
        public ObjectType Type;
        public short Number;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 51)]
        public string Name;
        public CatEntry3 Star;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RAOfCioArray
    {

        internal RAOfCio Value1;
        internal RAOfCio Value2;
        internal RAOfCio Value3;
        internal RAOfCio Value4;
        internal RAOfCio Value5;
        internal RAOfCio Value6;
        internal RAOfCio Value7;
        internal RAOfCio Value8;
        internal RAOfCio Value9;
        internal RAOfCio Value10;
        internal RAOfCio Value11;
        internal RAOfCio Value12;
        internal RAOfCio Value13;
        internal RAOfCio Value14;
        internal RAOfCio Value15;
        internal RAOfCio Value16;
        internal RAOfCio Value17;
        internal RAOfCio Value18;
        internal RAOfCio Value19;
        internal RAOfCio Value20;

        internal void Initialise()
        {
            Value1.RACio = Constants.RACIO_DEFAULT_VALUE;
            Value2.RACio = Constants.RACIO_DEFAULT_VALUE;
            Value3.RACio = Constants.RACIO_DEFAULT_VALUE;
            Value4.RACio = Constants.RACIO_DEFAULT_VALUE;
            Value5.RACio = Constants.RACIO_DEFAULT_VALUE;
            Value6.RACio = Constants.RACIO_DEFAULT_VALUE;
            Value7.RACio = Constants.RACIO_DEFAULT_VALUE;
            Value8.RACio = Constants.RACIO_DEFAULT_VALUE;
            Value9.RACio = Constants.RACIO_DEFAULT_VALUE;
            Value10.RACio = Constants.RACIO_DEFAULT_VALUE;
            Value11.RACio = Constants.RACIO_DEFAULT_VALUE;
            Value12.RACio = Constants.RACIO_DEFAULT_VALUE;
            Value13.RACio = Constants.RACIO_DEFAULT_VALUE;
            Value14.RACio = Constants.RACIO_DEFAULT_VALUE;
            Value15.RACio = Constants.RACIO_DEFAULT_VALUE;
            Value16.RACio = Constants.RACIO_DEFAULT_VALUE;
            Value17.RACio = Constants.RACIO_DEFAULT_VALUE;
            Value18.RACio = Constants.RACIO_DEFAULT_VALUE;
            Value19.RACio = Constants.RACIO_DEFAULT_VALUE;
            Value20.RACio = Constants.RACIO_DEFAULT_VALUE;
        }
    }

    #endregion

}
