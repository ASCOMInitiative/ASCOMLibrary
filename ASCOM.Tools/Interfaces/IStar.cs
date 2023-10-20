using System.Runtime.InteropServices;

namespace ASCOM.Tools.Interfaces
{
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
}