using ASCOM.Tools.Novas31;
using Kepler;

namespace NovasCom
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
        private readonly PositionVector m_BaryPos = new PositionVector(), m_HeliPos = new PositionVector();
        private readonly VelocityVector m_BaryVel = new VelocityVector(), m_HeliVel = new VelocityVector();
        private double m_BaryTime, m_MeanOb, m_EquOfEqu, m_NutLong, m_NutObl, m_TrueOb;
        private IEphemeris m_EarthEph;
        private bool m_Valid; // Object has valid values

        /// <summary>
        /// Create a new instance of the Earth object
        /// </summary>
        /// <remarks></remarks>
        public Earth()
        {
            m_EarthEph = new Ephemeris
            {
                BodyType = BodyType.MajorPlanet,
                Number = Body.Earth,
                Name = "Earth"
            };
            m_Valid = false; // Object is invalid
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
                return m_BaryPos;
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
                return m_BaryTime;
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
                return m_BaryVel;
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
                return m_EarthEph;
            }
            set
            {
                m_EarthEph = value;
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
                return m_EquOfEqu;
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
                return m_HeliPos;
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
                return m_HeliVel;
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
                return m_MeanOb;
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
                return m_NutLong;
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
                return m_NutObl;
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
            double[] m_peb = new double[3], m_veb = new double[3], m_pes = new double[3], m_ves = new double[3];

            NovasComSupport.get_earth_nov(ref m_EarthEph, tjd, ref m_BaryTime, ref m_peb, ref m_veb, ref m_pes, ref m_ves);
            Novas.ETilt(tjd, Accuracy.Full, ref m_MeanOb, ref m_TrueOb, ref m_EquOfEqu, ref m_NutLong, ref m_NutObl);

            m_BaryPos.x = m_peb[0];
            m_BaryPos.y = m_peb[1];
            m_BaryPos.z = m_peb[2];
            m_BaryVel.x = m_veb[0];
            m_BaryVel.y = m_veb[1];
            m_BaryVel.z = m_veb[2];

            m_HeliPos.x = m_pes[0];
            m_HeliPos.y = m_pes[1];
            m_HeliPos.z = m_pes[2];
            m_HeliVel.x = m_ves[0];
            m_HeliVel.y = m_ves[1];
            m_HeliVel.z = m_ves[2];

            m_Valid = true;

            return m_Valid;
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
                return m_TrueOb;
            }
        }
    }
}