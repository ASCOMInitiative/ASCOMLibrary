using ASCOM.Tools.Interfaces;
using ASCOM.Tools.Novas31;

namespace ASCOM.Tools
{
    /// <summary>
    /// NOVAS-COM: Represents the "state" of the Earth at a given Terrestrial Julian date
    /// </summary>
    /// <remarks>NOVAS-COM objects of class Earth represent the "state" of the Earth at a given Terrestrial Julian date. 
    /// The state includes barycentric and heliocentric position vectors for the earth, plus obliquity, 
    /// nutation and the equation of the equinoxes. Unless set by the client, the Earth ephemeris used is 
    /// computed using an internal approximation. The client may optionally attach an ephemeris object for 
    /// increased Accuracy. 
    /// <para><b>Ephemeris Generator</b><br />
    /// The ephemeris generator object used with NOVAS-COM must support a single 
    /// method GetPositionAndVelocity(tjd). This method must take a terrestrial Julian date (like the 
    /// NOVAS-COM methods) as its single parameter, and return an array of Double 
    /// containing the rectangular (x/y/z) heliocentric J2000 equatorial coordinates of position (AU) and velocity 
    /// (KM/sec.). In addition, it must support three read/write properties BodyType, Name, and Number, 
    /// which correspond to the Type, Name, and Number properties of Novas.Planet. 
    /// </para></remarks>
    public class Earth : IEarth
    {
        private readonly PositionVector barycentricPosition = new PositionVector(), heliocentricPosition = new PositionVector();
        private readonly VelocityVector barycentricVelocity = new VelocityVector(), heliocentricVelicity = new VelocityVector();
        private double barycentricTime, meanObliquity, equationOfEquinoxes, nutationLongitude, nutationObliquity, trueObliquity;
        private IEphemeris earthEphemeris;
        private bool isValid; // Object has valid values

        /// <summary>
        /// Create a new instance of the Earth object
        /// </summary>
        /// <remarks></remarks>
        public Earth()
        {
            earthEphemeris = new Ephemeris
            {
                BodyType = BodyType.MajorPlanet,
                Number = Body.Earth,
                Name = "Earth"
            };
            isValid = false; // Object is invalid
        }

        /// <summary>
        /// Earth barycentric position
        /// </summary>
        /// <value>Barycentric position vector</value>
        /// <returns>AU (Ref J2000)</returns>
        /// <remarks></remarks>
        public PositionVector BarycentricPosition
        {
            get
            {
                return barycentricPosition;
            }
        }

        /// <summary>
        /// Earth barycentric time 
        /// </summary>
        /// <value>Barycentric dynamical time for given Terrestrial Julian Date</value>
        /// <returns>Julian date</returns>
        /// <remarks></remarks>
        public double BarycentricTime
        {
            get
            {
                return barycentricTime;
            }
        }

        /// <summary>
        /// Earth barycentric velocity 
        /// </summary>
        /// <value>Barycentric velocity vector</value>
        /// <returns>AU/day (ref J2000)</returns>
        /// <remarks></remarks>
        public VelocityVector BarycentricVelocity
        {
            get
            {
                return barycentricVelocity;
            }
        }

        /// <summary>
        /// Ephemeris object used to provide the position of the Earth.
        /// </summary>
        /// <value>Earth ephemeris object </value>
        /// <returns>Earth ephemeris object</returns>
        /// <remarks>
        /// Setting this is optional, if not set, the internal Kepler engine will be used.</remarks>
        public IEphemeris EarthEphemeris
        {
            get
            {
                return earthEphemeris;
            }
            set
            {
                earthEphemeris = value;
            }
        }

        /// <summary>
        /// Earth equation of equinoxes 
        /// </summary>
        /// <value>Equation of the equinoxes</value>
        /// <returns>Seconds</returns>
        /// <remarks></remarks>
        public double EquationOfEquinoxes
        {
            get
            {
                return equationOfEquinoxes;
            }
        }

        /// <summary>
        /// Earth heliocentric position
        /// </summary>
        /// <value>Heliocentric position vector</value>
        /// <returns>AU (ref J2000)</returns>
        /// <remarks></remarks>
        public PositionVector HeliocentricPosition
        {
            get
            {
                return heliocentricPosition;
            }
        }

        /// <summary>
        /// Earth heliocentric velocity 
        /// </summary>
        /// <value>Heliocentric velocity</value>
        /// <returns>Velocity vector, AU/day (ref J2000)</returns>
        /// <remarks></remarks>
        public VelocityVector HeliocentricVelocity
        {
            get
            {
                return heliocentricVelicity;
            }
        }

        /// <summary>
        /// Earth mean obliquity
        /// </summary>
        /// <value>Mean obliquity of the ecliptic</value>
        /// <returns>Degrees</returns>
        /// <remarks></remarks>
        public double MeanObliquity
        {
            get
            {
                return meanObliquity;
            }
        }

        /// <summary>
        /// Earth nutation in longitude 
        /// </summary>
        /// <value>Nutation in longitude</value>
        /// <returns>Degrees</returns>
        /// <remarks></remarks>
        public double NutationInLongitude
        {
            get
            {
                return nutationLongitude;
            }
        }

        /// <summary>
        /// Earth nutation in obliquity 
        /// </summary>
        /// <value>Nutation in obliquity</value>
        /// <returns>Degrees</returns>
        /// <remarks></remarks>
        public double NutationInObliquity
        {
            get
            {
                return nutationObliquity;
            }
        }

        /// <summary>
        /// Initialize the Earth object for given terrestrial Julian date
        /// </summary>
        /// <param name="tjd">Terrestrial Julian date</param>
        /// <returns>True if successful, else throws an exception</returns>
        /// <remarks></remarks>
        public bool SetForTime(double tjd)
        {
            double[] basycentricPosition = new double[3], barycentricVelocity = new double[3], heliocentricPosition = new double[3], heliocentricVelocity = new double[3];

            NovasComSupport.get_earth_nov(ref earthEphemeris, tjd, ref barycentricTime, ref basycentricPosition, ref barycentricVelocity, ref heliocentricPosition, ref heliocentricVelocity);
            Novas.ETilt(tjd, Accuracy.Full, ref meanObliquity, ref trueObliquity, ref equationOfEquinoxes, ref nutationLongitude, ref nutationObliquity);

            barycentricPosition.x = basycentricPosition[0];
            barycentricPosition.y = basycentricPosition[1];
            barycentricPosition.z = basycentricPosition[2];
            this.barycentricVelocity.x = barycentricVelocity[0];
            this.barycentricVelocity.y = barycentricVelocity[1];
            this.barycentricVelocity.z = barycentricVelocity[2];

            this.heliocentricPosition.x = heliocentricPosition[0];
            this.heliocentricPosition.y = heliocentricPosition[1];
            this.heliocentricPosition.z = heliocentricPosition[2];
            heliocentricVelicity.x = heliocentricVelocity[0];
            heliocentricVelicity.y = heliocentricVelocity[1];
            heliocentricVelicity.z = heliocentricVelocity[2];

            isValid = true;

            return isValid;
        }

        /// <summary>
        /// Earth true obliquity 
        /// </summary>
        /// <value>True obliquity of the ecliptic</value>
        /// <returns>Degrees</returns>
        /// <remarks></remarks>
        public double TrueObliquity
        {
            get
            {
                return trueObliquity;
            }
        }
    }
}