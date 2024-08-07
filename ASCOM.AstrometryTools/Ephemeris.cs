﻿using ASCOM.Tools.Interfaces;
using ASCOM.Tools.Novas31;
using ASCOM.Tools.Kepler;

namespace ASCOM.Tools
{

    // NOTE on changed behaviour for elliptical orbit comet calculations with - Peter Simpson January 2023

    // The original Kepler component assigned both the Kepler.a (semi-major axis) and Kepler.q (perihelion distance) properties to the same
    // Orbit structure variable 'a' which is defined here as the mean distance (semi-major axis).

    // However, the semi-major axis value is not available in MPC one-line orbit ephemeris data although it can be calculated from the perihelion distance for
    // elliptical orbits (eccentricity <1.0) by the formula: SemiMajorAxis = PerihelionDistance / (1 - OrbitalEccentricity)

    // Users tracking comets are being caught out because they are setting the perihelion distance from the MPC data but the Kepler component actually requires the semi-major axis in 
    // order to calculate the correct orbit for elliptical comets and currently is using the supplied perihelion distance and thus returns the wrong answer.

    // It is now necessary to disambiguate the Kepler.a and Kepler.q properties, but in a way that is backward compatible with previous behaviour. There are four scenarios to consider:
    // Kepler.q   Kepler.a
    // 1) Un-set     Un-set
    // 2) Set        Un-set
    // 3) Un-set     Set
    // 4) Set        Set

    // Original internal use of Orbit.a by elliptical comet scenario when Kepler.GetPositionAndVelocity is called:
    // 1) A value of 0.0 is used in calculation, which will result in a wrong answer
    // 2) The perihelion distance is used, which results in the wrong answer
    // 3) The semi-major axis value is used, which results in the correct answer
    // 4) Whichever property was set last is used, which will give the correct answer if semi-major axis was set last and the wrong answer if perihelion distance was set last

    // In addition there were some undesirable outcomes when reading the Kepler object a and q properties for elliptical orbit comets:
    // a) If a was set then q would have an incorrect value 
    // b) If q was set then a would have an incorrect value 

    // The code has been modified to behave like this in each scenario when Kepler.GetPositionAndVelocity is called:
    // 1) A value of 0.0 is used in the calculation, which will result in a wrong answer
    // 2) The semi-major axis value is calculated from the perihelion distance and used, which results in the correct answer
    // 3) The supplied semi-major axis value is used, which results in the correct answer
    // 4) The supplied semi-major axis value is used, which results in the correct answer

    // Improved property behaviours by scenario:
    // 1) Semi-major axis and perihelion distance return 0.0;
    // 2) Perihelion distance has the set value and semi-major axis returns a calculated value
    // 3) Semi-major axis has the set value and perihelion distance returns a calculated value
    // 4) Both perihelion distance and semi-major axis return their set values

    /// <summary>
    /// KEPLER: Ephemeris Object
    /// </summary>
    /// <remarks>
    /// The Kepler Ephemeris object contains an orbit engine which takes the orbital parameters of a solar system 
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
    /// is converted internally to other scales. Note that UTC seconds are based on the Cesium atom, 
    /// not planetary motions. In order to keep UTC in sync with planetary motion, leap seconds are 
    /// inserted periodically. The error is at most 900 milliseconds.
    /// <br /><br /><i>UT1 Time Scale </i><br />
    /// The UT1 time scale is the planetary equivalent of UTC. It it runs smoothly and varies a bit 
    /// with time, but it is never more than 900 milliseconds different from UTC. 
    /// <br /><br /><i>TT Time Scale </i><br />
    /// The Terrestrial Dynamical Time (TT) scale is used in solar system orbital calculations. 
    /// It is based completely on planetary motions; you can think of the solar system as a giant 
    /// TT clock. It differs from UT1 by an amount called "delta-t", which slowly increases with time, 
    /// and is about 60 seconds right now (2001). </para>
    /// </remarks>
    public class Ephemeris : IEphemeris
    {
        private const double DTVEL = 0.01d;

        // Ephemeris variables
        private string bodyName; // Name of body
        private Body bodyNumber; // Number of body
        private bool bodyNumberIsValid;
        private BodyType bodyType; // Type of body
        private bool bodyTypeIsValid;
        private Orbit orbit = new Orbit(); // Elements, etc for minor planets/comets, etc.

        /// <summary>
        /// Create a new Ephemeris component and initialise it
        /// </summary>
        /// <remarks></remarks>
        public Ephemeris()
        {
            bodyTypeIsValid = false;
            bodyName = ""; // Sentinel
            bodyType = default;
            orbit.ptable.lon_tbl = new double[] { 0.0d }; // Initialise orbit arrays
            orbit.ptable.lat_tbl = new double[] { 0.0d };
        }

        /// <summary>
        /// Semi-major axis (AU)
        /// </summary>
        /// <value>Semi-major axis in AU</value>
        /// <returns>Semi-major axis in AU</returns>
        /// <remarks></remarks>
        public double a_SemiMajorAxis
        {
            get
            {
                if (double.IsNaN(orbit.semiMajorAxis))
                {
                    return 0.0d;
                }
                else
                {
                    return orbit.semiMajorAxis;
                }
            }
            set
            {
                orbit.semiMajorAxis = value;
                orbit.a = value;
            }
        }

        /// <summary>
        /// Perihelion distance (AU)
        /// </summary>
        /// <value>Perihelion distance</value>
        /// <returns>AU</returns>
        /// <remarks></remarks>
        public double q_PerihelionDistance
        {
            get
            {
                if (double.IsNaN(orbit.perihelionDistance))
                {
                    return 0.0d;
                }
                else
                {
                    return orbit.perihelionDistance;
                }
            }
            set
            {
                orbit.perihelionDistance = value;
                orbit.a = value;
            }
        }

        /// <summary>
        /// The type of solar system body represented by this instance of the ephemeris engine (enum)
        /// </summary>
        /// <value>The type of solar system body represented by this instance of the ephemeris engine (enum)</value>
        /// <returns>0 for major planet, 1 for minot planet and 2 for comet</returns>
        /// <remarks></remarks>
        public BodyType BodyType
        {
            get
            {
                if (!bodyTypeIsValid)
                    throw new HelperException("KEPLER:BodyType - BodyType has not been set");

                return bodyType;
            }
            set
            {
                bodyType = value;
                bodyTypeIsValid = true;
            }
        }

        /// <summary>
        /// Orbital eccentricity
        /// </summary>
        /// <value>Orbital eccentricity </value>
        /// <returns>Orbital eccentricity </returns>
        /// <remarks></remarks>
        public double e_OrbitalEccentricity
        {
            get
            {
                return orbit.ecc;
            }
            set
            {
                orbit.ecc = value;
                orbit.eccentricityHasBeenSet = true; // Record that an eccentricity value has been set (used for parameter validation in GetPositionAndVelocity())
            }
        }

        /// <summary>
        /// Epoch of osculation of the orbital elements (terrestrial Julian date)
        /// </summary>
        /// <value>Epoch of osculation of the orbital elements</value>
        /// <returns>Terrestrial Julian date</returns>
        /// <remarks></remarks>
        public double Epoch
        {
            get
            {
                return orbit.epoch;
            }
            set
            {
                orbit.epoch = value;
            }
        }

        /// <summary>
        /// Slope parameter for magnitude
        /// </summary>
        /// <value>Slope parameter for magnitude</value>
        /// <returns>Slope parameter for magnitude</returns>
        /// <remarks></remarks>
        public double G_SlopeForMagnitude
        {
            get
            {
                throw new HelperException("Kepler:G Read - Magnitude slope parameter calculation not implemented");
            }
            set
            {
                throw new HelperException("Kepler:G Write - Magnitude slope parameter calculation not implemented");
            }
        }

        /// <summary>
        /// Compute rectangular (x/y/z) heliocentric J2000 equatorial coordinates of position (AU) and 
        /// velocity (KM/sec.).
        /// </summary>
        /// <param name="tjd">Terrestrial Julian date/time for which position and velocity is to be computed</param>
        /// <returns>Array of 6 values containing rectangular (x/y/z) heliocentric J2000 equatorial 
        /// coordinates of position (AU) and velocity (KM/sec.) for the Body.</returns>
        /// <remarks>The TJD parameter is the date/time as a Terrestrial Time Julian date. See below for 
        /// more info. If you are using ACP, there are functions available to convert between UTC and 
        /// Terrestrial time, and for estimating the current value of delta-T. See the Overview page for 
        /// the Kepler.Ephemeris class for more information on time keeping systems.</remarks>
        public double[] GetPositionAndVelocity(double tjd)
        {
            Orbit orbit = new Orbit();
            int i;
            double[] resultPositionVector = new double[6];

            if (!bodyTypeIsValid)
                throw new HelperException("Kepler:GetPositionAndVelocity - Body type has not been set");

            switch (bodyType)
            {
                case BodyType.MajorPlanet: // MAJOR PLANETS [unimpl. SUN, MOON]
                    switch (bodyNumber)
                    {
                        case Body.Mercury:
                            orbit = KeplerSupport.mercury;
                            break;
                        case Body.Venus:
                            orbit = KeplerSupport.venus;
                            break;
                        case Body.Earth:
                            orbit = KeplerSupport.earthplanet;
                            break;
                        case Body.Mars:
                            orbit = KeplerSupport.mars;
                            break;
                        case Body.Jupiter:
                            orbit = KeplerSupport.jupiter;
                            break;
                        case Body.Saturn:
                            orbit = KeplerSupport.saturn;
                            break;
                        case Body.Uranus:
                            orbit = KeplerSupport.uranus;
                            break;
                        case Body.Neptune:
                            orbit = KeplerSupport.neptune;
                            break;
                        case Body.Pluto:
                            orbit = KeplerSupport.pluto;
                            break;

                        default:
                            throw new InvalidValueException("Kepler:GetPositionAndVelocity - Invalid value for planet number: " + ((int)bodyNumber).ToString());
                    }

                    double[] position = new double[3] { 0.0,0.0,0.0};
                    double[] velocity = new double[3] { 0.0, 0.0, 0.0 };
                    short rc = Novas.SolarSystem(tjd, bodyNumber, Origin.Heliocentric, ref position, ref velocity);

                    resultPositionVector[0] = position[0];
                    resultPositionVector[1] = position[1];
                    resultPositionVector[2] = position[2];
                    resultPositionVector[3] = velocity[0];
                    resultPositionVector[4] = velocity[1];
                    resultPositionVector[5] = velocity[2];

                    return resultPositionVector;

                case BodyType.MinorPlanet: // MINOR PLANET
                    orbit = this.orbit;
                    break;

                case BodyType.Comet: // COMET
                                    
                    // Test whether this comet is in an elliptical orbit as opposed to parabolic or hyperbolic
                    if (this.orbit.ecc < 1.0d)
                    {
                        // TL?.LogMessage("GetPositionAndVelocity1", $"Perihelion distance: {m_e.perihelionDistance}, Semi-major axis: {m_e.semiMajorAxis}, m_e.a: {m_e.a}, Eccentricity has been set: {m_e.eccentricityHasBeenSet}, Eccentricity: {m_e.ecc}")
                        // For comets in elliptical orbits (ecc < 1.0) ensure that we use the semi-major axis instead of the perihelion distance.
                        // Handle the four possible scenarios for semi-major axis and perihelion distance
                        // 1) Un-set     Un-set
                        // 2) Set        Un-set
                        // 3) Un-set     Set
                        // 4) Set        Set
                        if (double.IsNaN(this.orbit.semiMajorAxis)) // Semi-major axis is not set
                        {
                            if (double.IsNaN(this.orbit.perihelionDistance)) // No semi-major axis or perihelion distance
                            {
                                // Throw an exception because we can't calculate the orbit without either the semi-major axis value or the perihelion distance value.
                                throw new InvalidOperationException($"Kepler.GetPositionAndVelocity - Cannot calculate comet position because neither the semi-major axis nor the perihelion distance have been provided.");
                            }
                            else
                            {
                                // No semi-major axis but we do have perihelion distance so calculate semi-major axis from the formula: SemiMajorAxis = PerihelionDistance / (1 - OrbitalEccentricity) and use this

                                // Validate that the calculation can be completed
                                if (!this.orbit.eccentricityHasBeenSet)
                                    throw new InvalidOperationException($"Kepler.GetPositionAndVelocity - Cannot calculate comet position because the orbit eccentricity has not been provided.");

                                this.orbit.a = this.orbit.perihelionDistance / (1.0d - this.orbit.ecc);
                                this.orbit.semiMajorAxis = this.orbit.a;
                            }
                        }
                        else // Semi-major axis has been set so use this
                        {
                            this.orbit.a = this.orbit.semiMajorAxis;

                            if (double.IsNaN(this.orbit.perihelionDistance))
                            {
                                // Update perihelion distance from the formula: PerihelionDistance  = SemiMajorAxis * (1 - OrbitalEccentricity) and use this

                                // Validate that the calculation can be completed, otherwise ignore because the orbit can still be calculated
                                if (this.orbit.eccentricityHasBeenSet)
                                {
                                    this.orbit.perihelionDistance = this.orbit.semiMajorAxis * (1.0d - this.orbit.ecc);
                                }
                            }

                            else
                            {
                                // The perihelion distance has been set so just leave it as is
                            }
                        }
                    }
                    else if (!double.IsNaN(this.orbit.semiMajorAxis)) // Eccentricity is >=1.0 and this is a parabolic or hyperbolic orbit so there is no major axis
                    {
                        throw new InvalidOperationException($"Kepler.GetPositionAndVelocity - Eccentricity is >=1.0 {this.orbit.ecc} (parabolic or hyperbolic trajectory, not an elliptical orbit) but a semi-major axis value has been set implying an orbit.");
                    }

                    orbit = this.orbit;
                    break;
            }

            var tempPosVec = new double[4, 4];

            // Calculate position vectors slightly before and slightly after the required time so that velocity can be calculated from change in distance in the give time 
            for (i = 0; i <= 2; i++)
            {
                double[] calculatedPositionVector = new double[3];
                double calculatedJulianDate = tjd + (i - 1) * DTVEL;

                KeplerSupport.KeplerCalc(calculatedJulianDate, ref orbit, ref calculatedPositionVector);

                tempPosVec[i, 0] = calculatedPositionVector[0];
                tempPosVec[i, 1] = calculatedPositionVector[1];
                tempPosVec[i, 2] = calculatedPositionVector[2];
            }


            // tempPosVec(1,x) contains the pos vector
            // tempPosVec(0,x) and tempPosVec(2,x) are used to determine the velocity based on position change with time!
            for (i = 0; i <= 2; i++)
            {
                resultPositionVector[i] = tempPosVec[1, i];
                resultPositionVector[3 + i] = (tempPosVec[2, i] - tempPosVec[0, i]) / (2.0d * DTVEL);
            }

            return resultPositionVector;
        }

        /// <summary>
        /// Absolute visual magnitude
        /// </summary>
        /// <value>Absolute visual magnitude</value>
        /// <returns>Absolute visual magnitude</returns>
        /// <remarks></remarks>
        public double H_AbsoluteVisualMagnitude
        {
            get
            {
                throw new HelperException("Kepler:H Read - Visual magnitude calculation not implemented");
            }
            set
            {
                throw new HelperException("Kepler:H Write - Visual magnitude calculation not implemented");
            }
        }

        /// <summary>
        /// The J2000.0 inclination (deg.)
        /// </summary>
        /// <value>The J2000.0 inclination</value>
        /// <returns>Degrees</returns>
        /// <remarks></remarks>
        public double i_Inclination
        {
            get
            {
                return orbit.i;
            }
            set
            {
                orbit.i = value;
            }
        }

        /// <summary>
        /// Mean anomaly at the epoch
        /// </summary>
        /// <value>Mean anomaly at the epoch</value>
        /// <returns>Mean anomaly at the epoch</returns>
        /// <remarks></remarks>
        public double M_MeanAnomalyAtEpoch
        {
            get
            {
                return orbit.M;
            }
            set
            {
                orbit.M = value;
            }
        }

        /// <summary>
        /// Mean daily motion (deg/day)
        /// </summary>
        /// <value>Mean daily motion</value>
        /// <returns>Degrees per day</returns>
        /// <remarks></remarks>
        public double n_MeanDailyMotion
        {
            get
            {
                return orbit.dm;
            }
            set
            {
                orbit.dm = value;
            }
        }

        /// <summary>
        /// The name of the Body.
        /// </summary>
        /// <value>The name of the body or packed MPC designation</value>
        /// <returns>The name of the body or packed MPC designation</returns>
        /// <remarks>If this instance represents an unnumbered minor planet, Ephemeris.Name must be the 
        /// packed MPC designation. For other types, this is for display only.</remarks>
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(bodyName))
                    throw new HelperException("Kepler:Name - Name has not been set");

                return bodyName;
            }
            set
            {
                bodyName = value;
            }
        }

        /// <summary>
        /// The J2000.0 longitude of the ascending node (deg.)
        /// </summary>
        /// <value>The J2000.0 longitude of the ascending node</value>
        /// <returns>Degrees</returns>
        /// <remarks></remarks>
        public double Node
        {
            get
            {
                return orbit.W;
            }
            set
            {
                orbit.W = value;
            }
        }

        /// <summary>
        /// The major or minor planet number
        /// </summary>
        /// <value>The major or minor planet number</value>
        /// <returns>Number or zero if not numbered</returns>
        /// <remarks></remarks>
        public Body Number
        {
            get
            {
                if (!bodyNumberIsValid)
                    throw new HelperException("KEPLER:Number - Planet number has not been set");

                return bodyNumber;
            }
            set
            {
                bodyNumber = value;
                bodyNumberIsValid = true;
            }
        }

        /// <summary>
        /// Orbital period (years)
        /// </summary>
        /// <value>Orbital period</value>
        /// <returns>Years</returns>
        /// <remarks></remarks>
        public double P_OrbitalPeriod
        {
            get
            {
                throw new HelperException("Kepler:P Read - Orbital period calculation not implemented");
            }
            set
            {
                throw new HelperException("Kepler:P Write - Orbital period calculation not implemented");
            }
        }

        /// <summary>
        /// The J2000.0 argument of perihelion (deg.)
        /// </summary>
        /// <value>The J2000.0 argument of perihelion</value>
        /// <returns>Degrees</returns>
        /// <remarks></remarks>
        public double w_PerihelionArgument
        {
            get
            {
                return orbit.wp;
            }
            set
            {
                orbit.wp = value;
            }
        }

        /// <summary>
        /// Reciprocal semi-major axis (1/AU)
        /// </summary>
        /// <value>Reciprocal semi-major axis</value>
        /// <returns>1/AU</returns>
        /// <remarks></remarks>
        public double z_ReciprocalSemiMajorAxis
        {
            get
            {
                return 1.0d / orbit.a;
            }
            set
            {
                orbit.a = 1.0d / value;
            }
        }
    }
}