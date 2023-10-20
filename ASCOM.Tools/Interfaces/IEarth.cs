namespace ASCOM.Tools.Interfaces
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
}