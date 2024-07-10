using ASCOM.Tools.Novas31;

namespace ASCOM.Tools.Interfaces
{
    /// <summary>
    /// Interface to the Kepler Ephemeris component
    /// </summary>
    /// <remarks>
    /// The Ephemeris object contains an orbit engine which takes the orbital parameters of a solar system 
    /// body, plus a a terrestrial date/time, and produces the heliocentric equatorial position and 
    /// velocity vectors of the body in Cartesian coordinates. Orbital parameters are not required for 
    /// the major planets, Kepler contains an ephemeris generator for these bodies that is within 0.05 
    /// arc seconds of the JPL DE404 over a wide range of times, Perturbations from major planets are applied 
    /// to ephemerides for minor planets. 
    /// <para>The results are passed back as an array containing the two vectors. 
    /// Note that this is the format expected for the ephemeris generator used by the NOVAS-COM vector 
    /// astrometry engine. For more information see the description of Ephemeris.GetPositionAndVelocity().</para>
    /// <para>
    /// <b>Ephemeris Calculations</b><br />
    /// The ephemeris calculations in Kepler draw heavily from the work of 
    /// Stephen Moshier moshier@world.std.com. kepler is released as a free software package, further 
    /// extending the work of Mr. Moshier.</para>
    /// <para>Kepler does not integrate orbits to the current epoch. If you want the accuracy resulting from 
    /// an integrated orbit, you must integrate separately and supply Kepler with elements of the current 
    /// epoch. Orbit integration is on the list of things for the next major version.</para>
    /// <para>Kepler uses polynomial approximations for the major planet ephemerides. The tables 
    /// of coefficients were derived by a least squares fit of periodic terms to JPL's DE404 ephemerides. 
    /// The periodic frequencies used were determined by spectral analysis and comparison with VSOP87 and 
    /// other analytical planetary theories. The least squares fit to DE404 covers the interval from -3000 
    /// to +3000 for the outer planets, and -1350 to +3000 for the inner planets. For details on the 
    /// accuracy of the major planet ephemerides, see the Accuracy Tables page. </para>
    /// <para>
    /// <b>Date and Time Systems</b><br /><br />
    /// For a detailed explanation of astronomical timekeeping systems, see A Time Tutorial on the NASA 
    /// Goddard Spaceflight Center site, and the USNO Systems of Time site. 
    /// <br /><br /><i>ActiveX Date values </i><br />
    /// These are the Windows standard "date serial" numbers, and are expressed in local time or 
    /// UTC (see below). The fractional part of these numbers represents time within a day. 
    /// They are used throughout applications such as Excel, Visual Basic, VBScript, and other 
    /// ActiveX capable environments. 
    /// <br /><br /><i>Julian dates </i><br />
    /// These are standard Julian "date serial" numbers, and are expressed in UTC time or Terrestrial 
    /// time. The fractional part of these numbers represents time within a day. The standard ActiveX 
    /// "Double" precision of 15 digits gives a resolution of about one millisecond in a full Julian date. 
    /// This is sufficient for the purposes of this program. 
    /// <br /><br /><i>Hourly Time Values </i><br />
    /// These are typically used to represent sidereal time and right ascension. They are simple real 
    /// numbers in units of hours. 
    /// <br /><br /><i>UTC Time Scale </i><br />
    /// Most of the ASCOM methods and properties that accept date/time values (either Date or Julian) 
    /// assume that the date/time is in Coordinated Universal Time (UTC). Where necessary, this time 
    /// is converted internally to other scales. Note that UTC seconds are based on the Caesium atom, 
    /// not planetary motions. In order to keep UTC in sync with planetary motion, leap seconds are 
    /// inserted periodically. The error is at most 900 milliseconds.
    /// <br /><br /><i>UT1 Time Scale </i><br />
    /// The UT1 time scale is the planetary equivalent of UTC. It runs smoothly and varies a bit 
    /// with time, but it is never more than 900 milliseconds different from UTC. 
    /// <br /><br /><i>TT Time Scale </i><br />
    /// The Terrestrial Dynamical Time (TT) scale is used in solar system orbital calculations. 
    /// It is based completely on planetary motions; you can think of the solar system as a giant 
    /// TT clock. It differs from UT1 by an amount called "delta-t", which slowly increases with time, 
    /// and is about 60 seconds right now (2001). </para>
    /// </remarks>
    public interface IEphemeris
    {
        /// <summary>
        /// Compute rectangular (x/y/z) heliocentric J2000 equatorial coordinates of position (AU) and 
        /// velocity (KM/sec.).
        /// </summary>
        /// <param name="tjd">Terrestrial Julian date/time for which position and velocity is to be computed</param>
        /// <returns>Array of 6 values containing rectangular (x/y/z) heliocentric J2000 equatorial 
        /// coordinates of position (AU) and velocity (KM/sec.) for the body.</returns>
        /// <remarks>The TJD parameter is the date/time as a Terrestrial Time Julian date. See below for 
        /// more info. If you are using ACP, there are functions available to convert between UTC and 
        /// Terrestrial time, and for estimating the current value of delta-T. See the Overview page for 
        /// the Kepler.Ephemeris class for more information on time keeping systems.</remarks>
        double[] GetPositionAndVelocity(double tjd);

        /// <summary>
        /// Semi-major axis (AU)
        /// </summary>
        /// <value>Semi-major axis in AU</value>
        /// <returns>Semi-major axis in AU</returns>
        /// <remarks></remarks>
        double a_SemiMajorAxis { get; set; }

        /// <summary>
        /// The type of solar system body represented by this instance of the ephemeris engine (enum)
        /// </summary>
        /// <value>The type of solar system body represented by this instance of the ephemeris engine (enum)</value>
        /// <returns>0 for major planet, 1 for minor planet and 2 for comet</returns>
        /// <remarks></remarks>
        BodyType BodyType { get; set; }

        /// <summary>
        /// Orbital eccentricity
        /// </summary>
        /// <value>Orbital eccentricity </value>
        /// <returns>Orbital eccentricity </returns>
        /// <remarks></remarks>
        double e_OrbitalEccentricity { get; set; }

        /// <summary>
        /// Epoch of osculation of the orbital elements (terrestrial Julian date)
        /// </summary>
        /// <value>Epoch of osculation of the orbital elements</value>
        /// <returns>Terrestrial Julian date</returns>
        /// <remarks></remarks>
        double Epoch { get; set; }

        /// <summary>
        /// Slope parameter for magnitude
        /// </summary>
        /// <value>Slope parameter for magnitude</value>
        /// <returns>Slope parameter for magnitude</returns>
        /// <remarks></remarks>
        double G_SlopeForMagnitude { get; set; }

        /// <summary>
        /// Absolute visual magnitude
        /// </summary>
        /// <value>Absolute visual magnitude</value>
        /// <returns>Absolute visual magnitude</returns>
        /// <remarks></remarks>
        double H_AbsoluteVisualMagnitude { get; set; }

        /// <summary>
        /// The J2000.0 inclination (deg.)
        /// </summary>
        /// <value>The J2000.0 inclination</value>
        /// <returns>Degrees</returns>
        /// <remarks></remarks>
        double i_Inclination { get; set; }

        /// <summary>
        /// Mean anomaly at the epoch
        /// </summary>
        /// <value>Mean anomaly at the epoch</value>
        /// <returns>Mean anomaly at the epoch</returns>
        /// <remarks></remarks>
        double M_MeanAnomalyAtEpoch { get; set; }

        /// <summary>
        /// Mean daily motion (deg/day)
        /// </summary>
        /// <value>Mean daily motion</value>
        /// <returns>Degrees per day</returns>
        /// <remarks></remarks>
        double n_MeanDailyMotion { get; set; }

        /// <summary>
        /// The name of the body.
        /// </summary>
        /// <value>The name of the body or packed MPC designation</value>
        /// <returns>The name of the body or packed MPC designation</returns>
        /// <remarks>If this instance represents an unnumbered minor planet, Ephemeris.Name must be the 
        /// packed MPC designation. For other types, this is for display only.</remarks>
        string Name { get; set; }

        /// <summary>
        /// The J2000.0 longitude of the ascending node (deg.)
        /// </summary>
        /// <value>The J2000.0 longitude of the ascending node</value>
        /// <returns>Degrees</returns>
        /// <remarks></remarks>
        double Node { get; set; }

        /// <summary>
        /// The major or minor planet number
        /// </summary>
        /// <value>The major or minor planet number</value>
        /// <returns>Number or zero if not numbered</returns>
        /// <remarks></remarks>
        Body Number { get; set; }

        /// <summary>
        /// Orbital period (years)
        /// </summary>
        /// <value>Orbital period</value>
        /// <returns>Years</returns>
        /// <remarks></remarks>
        double P_OrbitalPeriod { get; set; }

        /// <summary>
        /// The J2000.0 argument of perihelion (deg.)
        /// </summary>
        /// <value>The J2000.0 argument of perihelion</value>
        /// <returns>Degrees</returns>
        /// <remarks></remarks>
        double w_PerihelionArgument { get; set; }

        /// <summary>
        /// Perihelion distance (AU)
        /// </summary>
        /// <value>Perihelion distance</value>
        /// <returns>AU</returns>
        /// <remarks></remarks>
        double q_PerihelionDistance { get; set; }

        /// <summary>
        /// Reciprocal semi-major axis (1/AU)
        /// </summary>
        /// <value>Reciprocal semi-major axis</value>
        /// <returns>1/AU</returns>
        /// <remarks></remarks>
        double z_ReciprocalSemiMajorAxis { get; set; }
    }
}