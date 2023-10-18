using System.Runtime.InteropServices;
using ASCOM;
using ASCOM.Tools.Novas31;
using Kepler;

namespace NovasCom
{
    /// <summary>
    /// Interface to an Earth object that represents the "state" of the Earth at a given Terrestrial Julian date
    /// </summary>
    /// <remarks>Objects of class Earth represent the "state" of the Earth at a given Terrestrial Julian date. 
    /// The state includes barycentric and heliocentric position vectors for the earth, plus obliquity, 
    /// nutation and the equation of the equinoxes. Unless set by the client, the Earth ephemeris used is 
    /// computed using an internal approximation. The client may optionally attach an ephemeris object for 
    /// increased accuracy. 
    /// <para><b>Ephemeris Generator</b><br />
    /// The ephemeris generator object used with NOVAS-COM must support a single 
    /// method GetPositionAndVelocity(tjd). This method must take a terrestrial Julian date (like the 
    /// NOVAS-COM methods) as its single parameter, and return an array of Double 
    /// containing the rectangular (x/y/z) heliocentric J2000 equatorial coordinates of position (AU) and velocity 
    /// (KM/sec.). In addition, it must support three read/write properties BodyType, Name, and Number, 
    /// which correspond to the Type, Name, and Number properties of Novas.Planet. 
    /// </para></remarks>
    public interface IEarth
    {
        /// <summary>
        /// Initialize the Earth object for given terrestrial Julian date
        /// </summary>
        /// <param name="tjd">Terrestrial Julian date</param>
        /// <returns>True if successful, else throws an exception</returns>
        /// <remarks></remarks>
        bool SetForTime(double tjd);

        /// <summary>
        /// Earth barycentric position
        /// </summary>
        /// <value>Barycentric position vector</value>
        /// <returns>AU (Ref J2000)</returns>
        /// <remarks></remarks>
        PositionVector BarycentricPosition { get; }

        /// <summary>
        /// Earth barycentric time 
        /// </summary>
        /// <value>Barycentric dynamical time for given Terrestrial Julian Date</value>
        /// <returns>Julian date</returns>
        /// <remarks></remarks>
        double BarycentricTime { get; }

        /// <summary>
        /// Earth barycentric velocity 
        /// </summary>
        /// <value>Barycentric velocity vector</value>
        /// <returns>AU/day (ref J2000)</returns>
        /// <remarks></remarks>
        VelocityVector BarycentricVelocity { get; }

        /// <summary>
        /// Ephemeris object used to provide the position of the Earth.
        /// </summary>
        /// <value>Earth ephemeris object </value>
        /// <returns>Earth ephemeris object</returns>
        /// <remarks>
        /// Setting this is optional, if not set, the internal Kepler engine will be used.</remarks>
        IEphemeris EarthEphemeris { get; set; }

        /// <summary>
        /// Earth equation of equinoxes 
        /// </summary>
        /// <value>Equation of the equinoxes</value>
        /// <returns>Seconds</returns>
        /// <remarks></remarks>
        double EquationOfEquinoxes { get; }

        /// <summary>
        /// Earth heliocentric position
        /// </summary>
        /// <value>Heliocentric position vector</value>
        /// <returns>AU (ref J2000)</returns>
        /// <remarks></remarks>
        PositionVector HeliocentricPosition { get; }

        /// <summary>
        /// Earth heliocentric velocity 
        /// </summary>
        /// <value>Heliocentric velocity</value>
        /// <returns>Velocity vector, AU/day (ref J2000)</returns>
        /// <remarks></remarks>
        VelocityVector HeliocentricVelocity { get; }

        /// <summary>
        /// Earth mean obliquity
        /// </summary>
        /// <value>Mean obliquity of the ecliptic</value>
        /// <returns>Degrees</returns>
        /// <remarks></remarks>
        double MeanObliquity { get; }

        /// <summary>
        /// Earth nutation in longitude 
        /// </summary>
        /// <value>Nutation in longitude</value>
        /// <returns>Degrees</returns>
        /// <remarks></remarks>
        double NutationInLongitude { get; }

        /// <summary>
        /// Earth nutation in obliquity 
        /// </summary>
        /// <value>Nutation in obliquity</value>
        /// <returns>Degrees</returns>
        /// <remarks></remarks>
        double NutationInObliquity { get; }

        /// <summary>
        /// Earth true obliquity 
        /// </summary>
        /// <value>True obliquity of the ecliptic</value>
        /// <returns>Degrees</returns>
        /// <remarks></remarks>
        double TrueObliquity { get; }
    }

    /// <summary>
    /// Interface to a Planet component that provides characteristics of a solar system body
    /// </summary>
    /// <remarks>Objects of class Planet hold the characteristics of a solar system body. Properties are 
    /// type (major or minor planet), number (for major and numbered minor planets), name (for unnumbered 
    /// minor planets and comets), the ephemeris object to be used for orbital calculations, an optional 
    /// ephemeris object to use for barycenter calculations, and an optional value for delta-T. 
    /// <para>The high-level NOVAS astrometric functions are implemented as methods of Planet: 
    /// GetTopocentricPosition(), GetLocalPosition(), GetApparentPosition(), GetVirtualPosition(), 
    /// and GetAstrometricPosition(). These methods operate on the properties of the Planet, and produce 
    /// a PositionVector object. For example, to get the topocentric coordinates of a planet, create and 
    /// initialize a planet, create initialize and attach an ephemeris object, then call 
    /// Planet.GetTopocentricPosition(). The resulting PositionVector's right ascension and declination 
    /// properties are the topocentric equatorial coordinates, at the same time, the (optionally 
    /// refracted) alt-az coordinates are calculated, and are also contained within the returned 
    /// PositionVector. <b>Note that Alt/Az is available in PositionVectors returned from calling 
    /// GetTopocentricPosition().</b> The accuracy of these calculations is typically dominated by the accuracy 
    /// of the attached ephemeris generator. </para>
    /// <para><b>Ephemeris Generator</b><br />
    /// By default, Kepler instances are attached for both Earth and Planet objects so it is
    /// not necessary to create and attach these in order to get Kepler accuracy from this
    /// component</para>
    /// <para>The ephemeris generator object used with NOVAS-COM must support a single 
    /// method GetPositionAndVelocity(tjd). This method must take a terrestrial Julian date (like the 
    /// NOVAS-COM methods) as its single parameter, and return an array of Double 
    /// containing the rectangular (x/y/z) heliocentric J2000 equatorial coordinates of position (AU) and velocity 
    /// (KM/sec.). In addition, it must support three read/write properties BodyType, Name, and Number, 
    /// which correspond to the Type, Name, and Number properties of Novas.Planet. 
    /// </para>
    /// </remarks>
    public interface IPlanet
    {
        /// <summary>
        /// Get an apparent position for given time
        /// </summary>
        /// <param name="tjd">Terrestrial Julian Date for the position</param>
        /// <returns>PositionVector for the apparent place.</returns>
        /// <remarks></remarks>
        PositionVector GetApparentPosition(double tjd);
        
        /// <summary>
        /// Get an astrometric position for given time
        /// </summary>
        /// <param name="tjd">Terrestrial Julian Date for the position</param>
        /// <returns>PositionVector for the astrometric place.</returns>
        /// <remarks></remarks>
        PositionVector GetAstrometricPosition(double tjd);

        /// <summary>
        /// Get an local position for given time
        /// </summary>
        /// <param name="tjd">Terrestrial Julian Date for the position</param>
        /// <param name="site">The observing site</param>
        /// <returns>PositionVector for the local place.</returns>
        /// <remarks></remarks>
        PositionVector GetLocalPosition(double tjd, [MarshalAs(UnmanagedType.IDispatch)] Site site);

        /// <summary>
        /// Get a topocentric position for given time
        /// </summary>
        /// <param name="tjd">Terrestrial Julian Date for the position</param>
        /// <param name="site">The observing site</param>
        /// <param name="Refract">Apply refraction correction</param>
        /// <returns>PositionVector for the topocentric place.</returns>
        /// <remarks></remarks>
        PositionVector GetTopocentricPosition(double tjd, [MarshalAs(UnmanagedType.IDispatch)] Site site, bool Refract);

        /// <summary>
        /// Get a virtual position for given time
        /// </summary>
        /// <param name="tjd">Terrestrial Julian Date for the position</param>
        /// <returns>PositionVector for the virtual place.</returns>
        /// <remarks></remarks>
        PositionVector GetVirtualPosition(double tjd);

        /// <summary>
        /// Planet delta-T
        /// </summary>
        /// <value>The value of delta-T (TT - UT1) to use for reductions</value>
        /// <returns>Seconds</returns>
        /// <remarks>Setting this value is optional. If no value is set, an internal delta-T generator is used.</remarks>
        double DeltaT { get; set; }

        /// <summary>
        /// Ephemeris object used to provide the position of the Earth.
        /// </summary>
        /// <value>Earth ephemeris object</value>
        /// <returns>Earth ephemeris object</returns>
        /// <remarks>
        /// Setting this is optional, if not set, the internal Kepler engine will be used.</remarks>
        IEphemeris EarthEphemeris { get; set; }

        /// <summary>
        /// The Ephemeris object used to provide positions of solar system bodies.
        /// </summary>
        /// <value>Body ephemeris object</value>
        /// <returns>Body ephemeris object</returns>
        /// <remarks>
        /// Setting this is optional, if not set, the internal Kepler engine will be used.
        /// </remarks>
        IEphemeris Ephemeris { get; set; }

        /// <summary>
        /// Planet name
        /// </summary>
        /// <value>For unnumbered minor planets, (Type=nvMinorPlanet and Number=0), the packed designation 
        /// for the minor planet. For other types, this is not significant, but may be used to store 
        /// a name.</value>
        /// <returns>Name of planet</returns>
        /// <remarks></remarks>
        string Name { get; set; }

        /// <summary>
        /// Planet number
        /// </summary>
        /// <value>For major planets (Type=nvMajorPlanet), a PlanetNumber value. For minor planets 
        /// (Type=nvMinorPlanet), the number of the minor planet or 0 for unnumbered minor planet.</value>
        /// <returns>Planet number</returns>
        /// <remarks>The major planet number is its number out from the sun starting with Mercury = 1</remarks>
        int Number { get; set; }

        /// <summary>
        /// The type of solar system body
        /// </summary>
        /// <value>The type of solar system body</value>
        /// <returns>Value from the BodyType enum</returns>
        /// <remarks></remarks>
        BodyType Type { get; set; }
    }

    /// <summary>
    /// Interface to the NOVAS-COM PositionVector Class
    /// </summary>
    /// <remarks>Objects of class PositionVector contain vectors used for positions (earth, sites, 
    /// stars and planets) throughout NOVAS-COM. Of course, its properties include the x, y, and z 
    /// components of the position. Additional properties are right ascension and declination, distance, 
    /// and light time (applicable to star positions), and Alt/Az (available only in PositionVectors 
    /// returned by Star or Planet methods GetTopocentricPosition()). You can initialize a PositionVector 
    /// from a Star object (essentially an FK5 or HIP catalog entry) or a Site (lat/long/height). 
    /// PositionVector has methods that can adjust the coordinates for precession, aberration and 
    /// proper motion. Thus, a PositionVector object gives access to some of the lower-level NOVAS functions. 
    /// <para><b>Note:</b> The equatorial coordinate properties of this object are dependent variables, and thus are read-only. 
    /// Changing any Cartesian coordinate will cause the equatorial coordinates to be recalculated. 
    /// </para></remarks>
    public interface IPositionVector
    {
        /// <summary>
        /// Adjust the position vector of an object for aberration of light
        /// </summary>
        /// <param name="vel">The velocity vector of the observer</param>
        /// <remarks>The algorithm includes relativistic terms</remarks>
        void Aberration([MarshalAs(UnmanagedType.IDispatch)] VelocityVector vel);

        /// <summary>
        /// Adjust the position vector for precession of equinoxes between two given epochs
        /// </summary>
        /// <param name="tjd">The first epoch (Terrestrial Julian Date)</param>
        /// <param name="tjd2">The second epoch (Terrestrial Julian Date)</param>
        /// <remarks>The coordinates are referred to the mean equator and equinox of the two respective epochs.</remarks>
        void Precess(double tjd, double tjd2);

        /// <summary>
        /// Adjust the position vector for proper motion (including foreshortening effects)
        /// </summary>
        /// <param name="vel">The velocity vector of the object</param>
        /// <param name="tjd1">The first epoch (Terrestrial Julian Date)</param>
        /// <param name="tjd2">The second epoch (Terrestrial Julian Date)</param>
        /// <returns>True if successful or throws an exception.</returns>
        /// <remarks></remarks>
        /// <exception cref="ValueNotSetException">If the position vector x, y or z values has not been set</exception>
        /// <exception cref="HelperException">If the supplied velocity vector does not have valid x, y and z components</exception>
        bool ProperMotion([MarshalAs(UnmanagedType.IDispatch)] VelocityVector vel, double tjd1, double tjd2);

        /// <summary>
        /// Initialize the PositionVector from a Site object and Greenwich apparent sidereal time.
        /// </summary>
        /// <param name="site">The Site object from which to initialize</param>
        /// <param name="gast">Greenwich Apparent Sidereal Time</param>
        /// <returns>True if successful or throws an exception</returns>
        /// <remarks>The GAST parameter must be for Greenwich, not local. The time is rotated through the 
        /// site longitude. See SetFromSiteJD() for an equivalent method that takes UTC Julian Date and 
        /// Delta-T (eliminating the need for calculating hyper-accurate GAST yourself).</remarks>
        bool SetFromSite([MarshalAs(UnmanagedType.IDispatch)] Site site, double gast);

        /// <summary>
        /// Initialize the PositionVector from a Site object using UTC Julian date and Delta-T
        /// </summary>
        /// <param name="site">The Site object from which to initialize</param>
        /// <param name="ujd">UTC Julian Date</param>
        /// <param name="delta_t">The value of Delta-T (TT - UT1) to use for reductions (seconds)</param>
        /// <returns>True if successful or throws an exception</returns>
        /// <remarks>The Julian date must be UTC Julian date, not terrestrial.
        /// </remarks>
        bool SetFromSiteJD([MarshalAs(UnmanagedType.IDispatch)] Site site, double ujd, double delta_t);

        /// <summary>
        /// Initialize the PositionVector from a Star object.
        /// </summary>
        /// <param name="star">The Star object from which to initialize</param>
        /// <returns>True if successful or throws an exception</returns>
        /// <remarks></remarks>
        /// <exception cref="HelperException">If Parallax, RightAScension or Declination is not available in the supplied star object.</exception>
        bool SetFromStar([MarshalAs(UnmanagedType.IDispatch)] Star star);

        /// <summary>
        /// The azimuth coordinate (degrees, + east)
        /// </summary>
        /// <value>The azimuth coordinate</value>
        /// <returns>Degrees, + East</returns>
        /// <remarks></remarks>
        double Azimuth { get; }

        /// <summary>
        /// Declination coordinate
        /// </summary>
        /// <value>Declination coordinate</value>
        /// <returns>Degrees</returns>
        /// <remarks></remarks>
        double Declination { get; }

        /// <summary>
        /// Distance/Radius coordinate
        /// </summary>
        /// <value>Distance/Radius coordinate</value>
        /// <returns>AU</returns>
        /// <remarks></remarks>
        double Distance { get; }

        /// <summary>
        /// The elevation (altitude) coordinate (degrees, + up)
        /// </summary>
        /// <value>The elevation (altitude) coordinate (degrees, + up)</value>
        /// <returns>(Degrees, + up</returns>
        /// <remarks>Elevation is available only in PositionVectors returned from calls to 
        /// Star.GetTopocentricPosition() and/or Planet.GetTopocentricPosition(). </remarks>
        /// <exception cref="HelperException">When the position vector has not been 
        /// initialised from Star.GetTopoCentricPosition and Planet.GetTopocentricPosition</exception>
        double Elevation { get; }

        /// <summary>
        /// Light time from body to origin, days.
        /// </summary>
        /// <value>Light time from body to origin</value>
        /// <returns>Days</returns>
        /// <remarks></remarks>
        double LightTime { get; }

        /// <summary>
        /// RightAscension coordinate, hours
        /// </summary>
        /// <value>RightAscension coordinate</value>
        /// <returns>Hours</returns>
        /// <remarks></remarks>
        double RightAscension { get; }

        /// <summary>
        /// Position Cartesian x component
        /// </summary>
        /// <value>Cartesian x component</value>
        /// <returns>Cartesian x component</returns>
        /// <remarks></remarks>
        double x { get; set; }

        /// <summary>
        /// Position Cartesian y component
        /// </summary>
        /// <value>Cartesian y component</value>
        /// <returns>Cartesian y component</returns>
        /// <remarks></remarks>
        double y { get; set; }

        /// <summary>
        /// Position Cartesian z component
        /// </summary>
        /// <value>Cartesian z component</value>
        /// <returns>Cartesian z component</returns>
        /// <remarks></remarks>
        double z { get; set; }
    }

    /// <summary>
    /// Interface for PositionVector methods that are only accessible through .NET and not through COM
    /// </summary>
    /// <remarks></remarks>
    public interface IPositionVectorExtra
    {
        /// <summary>
        /// Initialize the PositionVector from a Site object using UTC Julian date
        /// </summary>
        /// <param name="site">The Site object from which to initialize</param>
        /// <param name="ujd">UTC Julian Date</param>
        /// <returns>True if successful or throws an exception</returns>
        /// <remarks>The Julian date must be UTC Julian date, not terrestrial. Calculations will use the internal delta-T tables and estimator to get 
        /// delta-T. 
        /// This overload is not available through COM, please use 
        /// "SetFromSiteJD(ByVal site As Site, ByVal ujd As Double, ByVal delta_t As Double)"
        /// with delta_t set to 0.0 to achieve this effect.
        /// </remarks>
        bool SetFromSiteJD([MarshalAs(UnmanagedType.IDispatch)] Site site, double ujd);
    }

    /// <summary>
    /// Interface to the NOVAS-COM Site Class
    /// </summary>
    /// <remarks>Objects of class Site contain the specifications for an observer's location on the Earth 
    /// ellipsoid. Properties are latitude, longitude, height above mean sea level, the ambient temperature 
    /// and the sea-level barometric pressure. The latter two are used only for optional refraction corrections. 
    /// Latitude and longitude are (common) geodetic, not geocentric. </remarks>
    public interface ISite
    {
        /// <summary>
        /// Set all site properties in one method call
        /// </summary>
        /// <param name="Latitude">The geodetic latitude (degrees, + north)</param>
        /// <param name="Longitude">The geodetic longitude (degrees, +east)</param>
        /// <param name="Height">Height above sea level (meters)</param>
        /// <remarks></remarks>
        void Set(double Latitude, double Longitude, double Height);

        /// <summary>
        /// Height above mean sea level
        /// </summary>
        /// <value>Height above mean sea level</value>
        /// <returns>Meters</returns>
        /// <remarks></remarks>
        double Height { get; set; }

        /// <summary>
        /// Geodetic latitude (degrees, + north)
        /// </summary>
        /// <value>Geodetic latitude</value>
        /// <returns>Degrees, + north</returns>
        /// <remarks></remarks>
        double Latitude { get; set; }

        /// <summary>
        /// Geodetic longitude (degrees, + east)
        /// </summary>
        /// <value>Geodetic longitude</value>
        /// <returns>Degrees, + east</returns>
        /// <remarks></remarks>
        double Longitude { get; set; }

        /// <summary>
        /// Barometric pressure (millibars)
        /// </summary>
        /// <value>Barometric pressure</value>
        /// <returns>Millibars</returns>
        /// <remarks></remarks>
        double Pressure { get; set; }

        /// <summary>
        /// Ambient temperature (deg. Celsius)
        /// </summary>
        /// <value>Ambient temperature</value>
        /// <returns>Degrees Celsius)</returns>
        /// <remarks></remarks>
        double Temperature { get; set; }
    }

    /// <summary>
    /// Interface to the NOVAS-COM Star Class
    /// </summary>
    /// <remarks>Objects of class Site contain the specifications for a star's catalog position in either FK5 or Hipparcos units (both must be J2000). 
    /// Properties are right ascension and declination, proper motions, parallax, radial velocity, catalog type (FK5 or HIP), catalog number, optional ephemeris engine to use for 
    /// barycenter calculations, and an optional value for delta-T. Unless you specifically set the DeltaT property, calculations performed by this class which require the value of 
    /// delta-T (TT - UT1) rely on an internal function to estimate delta-T. 
    /// <para>The high-level NOVAS astrometric functions are implemented as methods of Star: 
    /// GetTopocentricPosition(), GetLocalPosition(), GetApparentPosition(), GetVirtualPosition(), 
    /// and GetAstrometricPosition(). These methods operate on the properties of the Star, and produce 
    /// a PositionVector object. For example, to get the topocentric coordinates of a star, simply create 
    /// and initialize a Star, then call Star.GetTopocentricPosition(). The resulting vaPositionVector's 
    /// right ascension and declination properties are the topocentric equatorial coordinates, at the same 
    /// time, the (optionally refracted) alt-az coordinates are calculated, and are also contained within 
    /// the returned PositionVector. <b>Note that Alt/Az is available in PositionVectors returned from calling 
    /// GetTopocentricPosition().</b></para></remarks>
    public interface IStar
    {
        /// <summary>
        /// Initialize all star properties with one call
        /// </summary>
        /// <param name="RA">Catalog mean right ascension (hours)</param>
        /// <param name="Dec">Catalog mean declination (degrees)</param>
        /// <param name="ProMoRA">Catalog mean J2000 proper motion in right ascension (sec/century)</param>
        /// <param name="ProMoDec">Catalog mean J2000 proper motion in declination (arcsec/century)</param>
        /// <param name="Parallax">Catalog mean J2000 parallax (arcsec)</param>
        /// <param name="RadVel">Catalog mean J2000 radial velocity (km/sec)</param>
        /// <remarks>Assumes positions are FK5. If Parallax is set to zero, NOVAS-COM assumes the object 
        /// is on the "celestial sphere", which has a distance of 10 mega parsecs. </remarks>
        void Set(double RA, double Dec, double ProMoRA, double ProMoDec, double Parallax, double RadVel);

        /// <summary>
        /// Initialise all star properties in one call using Hipparcos data. Transforms to FK5 standard used by NOVAS.
        /// </summary>
        /// <param name="RA">Catalog mean right ascension (hours)</param>
        /// <param name="Dec">Catalog mean declination (degrees)</param>
        /// <param name="ProMoRA">Catalog mean J2000 proper motion in right ascension (sec/century)</param>
        /// <param name="ProMoDec">Catalog mean J2000 proper motion in declination (arcsec/century)</param>
        /// <param name="Parallax">Catalog mean J2000 parallax (arcsec)</param>
        /// <param name="RadVel">Catalog mean J2000 radial velocity (km/sec)</param>
        /// <remarks>Assumes positions are Hipparcos standard and transforms to FK5 standard used by NOVAS. 
        /// <para>If Parallax is set to zero, NOVAS-COM assumes the object is on the "celestial sphere", 
        /// which has a distance of 10 mega parsecs.</para>
        /// </remarks>
        void SetHipparcos(double RA, double Dec, double ProMoRA, double ProMoDec, double Parallax, double RadVel);

        /// <summary>
        /// Get an apparent position for a given time
        /// </summary>
        /// <param name="tjd">Terrestrial Julian Date for the position</param>
        /// <returns>PositionVector for the apparent place.</returns>
        /// <remarks></remarks>
        PositionVector GetApparentPosition(double tjd);

        /// <summary>
        /// Get an astrometric position for a given time
        /// </summary>
        /// <param name="tjd">Terrestrial Julian Date for the position</param>
        /// <returns>PositionVector for the astrometric place.</returns>
        /// <remarks></remarks>
        PositionVector GetAstrometricPosition(double tjd);

        /// <summary>
        /// Get a local position for a given site and time
        /// </summary>
        /// <param name="tjd">Terrestrial Julian Date for the position</param>
        /// <param name="site">A Site object representing the observing site</param>
        /// <returns>PositionVector for the local place.</returns>
        /// <remarks></remarks>
        PositionVector GetLocalPosition(double tjd, [MarshalAs(UnmanagedType.IDispatch)] Site site);

        /// <summary>
        /// Get a topocentric position for a given site and time
        /// </summary>
        /// <param name="tjd">Terrestrial Julian Date for the position</param>
        /// <param name="site">A Site object representing the observing site</param>
        /// <param name="Refract">True to apply atmospheric refraction corrections</param>
        /// <returns>PositionVector for the topocentric place.</returns>
        /// <remarks></remarks>
        PositionVector GetTopocentricPosition(double tjd, [MarshalAs(UnmanagedType.IDispatch)] Site site, bool Refract);

        /// <summary>
        /// Get a virtual position at a given time
        /// </summary>
        /// <param name="tjd">Terrestrial Julian Date for the position</param>
        /// <returns>PositionVector for the virtual place.</returns>
        /// <remarks></remarks>
        PositionVector GetVirtualPosition(double tjd);

        /// <summary>
        /// Three character catalog code for the star's data
        /// </summary>
        /// <value>Three character catalog code for the star's data</value>
        /// <returns>Three character catalog code for the star's data</returns>
        /// <remarks>Typically "FK5" but may be "HIP". For information only.</remarks>
        string Catalog { get; set; }

        /// <summary>
        /// Mean catalog J2000 declination coordinate (degrees)
        /// </summary>
        /// <value>Mean catalog J2000 declination coordinate</value>
        /// <returns>Degrees</returns>
        /// <remarks></remarks>
        double Declination { get; set; }

        /// <summary>
        /// The value of delta-T (TT - UT1) to use for reductions.
        /// </summary>
        /// <value>The value of delta-T (TT - UT1) to use for reductions.</value>
        /// <returns>Seconds</returns>
        /// <remarks>If this property is not set, calculations will use an internal function to estimate delta-T.</remarks>
        double DeltaT { get; set; }

        /// <summary>
        /// Ephemeris object used to provide the position of the Earth.
        /// </summary>
        /// <value>Ephemeris object used to provide the position of the Earth.</value>
        /// <returns>Ephemeris object</returns>
        /// <remarks>If this value is not set, an internal Kepler object will be used to determine 
        /// Earth ephemeris</remarks>
        object EarthEphemeris { get; set; }

        /// <summary>
        /// The catalog name of the star (50 char max)
        /// </summary>
        /// <value>The catalog name of the star</value>
        /// <returns>Name (50 char max)</returns>
        /// <remarks></remarks>
        string Name { get; set; }

        /// <summary>
        /// The catalog number of the star
        /// </summary>
        /// <value>The catalog number of the star</value>
        /// <returns>The catalog number of the star</returns>
        /// <remarks></remarks>
        int Number { get; set; }

        /// <summary>
        /// Catalog mean J2000 parallax (arcsec)
        /// </summary>
        /// <value>Catalog mean J2000 parallax</value>
        /// <returns>Arc seconds</returns>
        /// <remarks></remarks>
        double Parallax { get; set; }

        /// <summary>
        /// Catalog mean J2000 proper motion in declination (arcsec/century)
        /// </summary>
        /// <value>Catalog mean J2000 proper motion in declination</value>
        /// <returns>Arc seconds per century</returns>
        /// <remarks></remarks>
        double ProperMotionDec { get; set; }

        /// <summary>
        /// Catalog mean J2000 proper motion in right ascension (sec/century)
        /// </summary>
        /// <value>Catalog mean J2000 proper motion in right ascension</value>
        /// <returns>Seconds per century</returns>
        /// <remarks></remarks>
        double ProperMotionRA { get; set; }

        /// <summary>
        /// Catalog mean J2000 radial velocity (km/sec)
        /// </summary>
        /// <value>Catalog mean J2000 radial velocity</value>
        /// <returns>Kilometers per second</returns>
        /// <remarks></remarks>
        double RadialVelocity { get; set; }

        /// <summary>
        /// Catalog mean J2000 right ascension coordinate (hours)
        /// </summary>
        /// <value>Catalog mean J2000 right ascension coordinate</value>
        /// <returns>Hours</returns>
        /// <remarks></remarks>
        double RightAscension { get; set; }
    }

    /// <summary>
    /// interface to the NOVAS_COM VelocityVector Class
    /// </summary>
    /// <remarks>Objects of class VelocityVector contain vectors used for velocities (earth, sites, 
    /// planets, and stars) throughout NOVAS-COM. Of course, its properties include the x, y, and z 
    /// components of the velocity. Additional properties are the velocity in equatorial coordinates of 
    /// right ascension dot, declination dot and radial velocity. You can initialize a PositionVector from 
    /// a Star object (essentially an FK5 or HIP catalog entry) or a Site (lat/long/height). For the star 
    /// object the proper motions, distance and radial velocity are used, for a site, the velocity is that 
    /// of the observer with respect to the Earth's center of mass. </remarks>
    public interface IVelocityVector
    {
        /// <summary>
        /// Initialize the VelocityVector from a Site object and Greenwich Apparent Sidereal Time.
        /// </summary>
        /// <param name="site">The Site object from which to initialize</param>
        /// <param name="gast">Greenwich Apparent Sidereal Time</param>
        /// <returns>True if OK or throws an exception</returns>
        /// <remarks>The velocity vector is that of the observer with respect to the Earth's center 
        /// of mass. The GAST parameter must be for Greenwich, not local. The time is rotated through 
        /// the site longitude. See SetFromSiteJD() for an equivalent method that takes UTC Julian 
        /// Date and optionally Delta-T (eliminating the need for calculating hyper-accurate GAST yourself). </remarks>
        bool SetFromSite([MarshalAs(UnmanagedType.IDispatch)] Site site, double gast);

        /// <summary>
        /// Initialize the VelocityVector from a Site object using UTC Julian Date and Delta-T
        /// </summary>
        /// <param name="site">The Site object from which to initialize</param>
        /// <param name="ujd">UTC Julian Date</param>
        /// <param name="delta_t">The optional value of Delta-T (TT - UT1) to use for reductions (seconds)</param>
        /// <returns>True if OK otherwise throws an exception</returns>
        /// <remarks>The velocity vector is that of the observer with respect to the Earth's center 
        /// of mass. The Julian date must be UTC Julian date, not terrestrial.</remarks>
        bool SetFromSiteJD([MarshalAs(UnmanagedType.IDispatch)] Site site, double ujd, double delta_t);

        /// <summary>
        /// Initialize the VelocityVector from a Star object.
        /// </summary>
        /// <param name="star">The Star object from which to initialize</param>
        /// <returns>True if OK otherwise throws an exception</returns>
        /// <remarks>The proper motions, distance and radial velocity are used in the velocity calculation. </remarks>
        /// <exception cref="HelperException">If any of: Parallax, RightAscension, Declination, 
        /// ProperMotionRA, ProperMotionDec or RadialVelocity are not available in the star object</exception>
        bool SetFromStar([MarshalAs(UnmanagedType.IDispatch)] Star star);

        /// <summary>
        /// Linear velocity along the declination direction (AU/day)
        /// </summary>
        /// <value>Linear velocity along the declination direction</value>
        /// <returns>AU/day</returns>
        /// <remarks>This is not the proper motion (which is an angular rate and is dependent on the distance to the object).</remarks>
        double DecVelocity { get; }

        /// <summary>
        /// Linear velocity along the radial direction (AU/day)
        /// </summary>
        /// <value>Linear velocity along the radial direction</value>
        /// <returns>AU/day</returns>
        /// <remarks></remarks>
        double RadialVelocity { get; }

        /// <summary>
        /// Linear velocity along the right ascension direction (AU/day)
        /// </summary>
        /// <value>Linear velocity along the right ascension direction</value>
        /// <returns>AU/day</returns>
        /// <remarks></remarks>
        double RAVelocity { get; }

        /// <summary>
        /// Cartesian x component of velocity (AU/day)
        /// </summary>
        /// <value>Cartesian x component of velocity</value>
        /// <returns>AU/day</returns>
        /// <remarks></remarks>
        double x { get; set; }

        /// <summary>
        /// Cartesian y component of velocity (AU/day)
        /// </summary>
        /// <value>Cartesian y component of velocity</value>
        /// <returns>AU/day</returns>
        /// <remarks></remarks>
        double y { get; set; }

        /// <summary>
        /// Cartesian z component of velocity (AU/day)
        /// </summary>
        /// <value>Cartesian z component of velocity</value>
        /// <returns>AU/day</returns>
        /// <remarks></remarks>
        double z { get; set; }

        /// <summary>
        /// Initialize the VelocityVector from a Site object using UTC Julian Date
        /// </summary>
        /// <param name="site">The Site object from which to initialize</param>
        /// <param name="ujd">UTC Julian Date</param>
        /// <returns>True if OK otherwise throws an exception</returns>
        /// <remarks>The velocity vector is that of the observer with respect to the Earth's center 
        /// of mass. The Julian date must be UTC Julian date, not terrestrial. This call will use 
        /// the internal tables and estimator to get delta-T.
        /// This overload is not available through COM, please use 
        /// "SetFromSiteJD(ByVal site As Site, ByVal ujd As Double, ByVal delta_t As Double)"
        /// with delta_t set to 0.0 to achieve this effect.
        /// </remarks>
        bool SetFromSiteJD([MarshalAs(UnmanagedType.IDispatch)] Site site, double ujd);
    }
}