using System.Runtime.InteropServices;
using ASCOM.Tools.Novas31;

namespace ASCOM.Tools.Interfaces
{
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
}