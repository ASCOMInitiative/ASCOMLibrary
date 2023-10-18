using ASCOM;
using static System.Math;
using ASCOM.Tools.Novas31;
using Kepler;
using ASCOM.Tools;

namespace NovasCom
{
    /// <summary>
    /// NOVAS-COM: Provide characteristics of a solar system body
    /// </summary>
    /// <remarks>NOVAS-COM objects of class Planet hold the characteristics of a solar system Body. Properties are 
    /// type (major or minor planet), number (for major and numbered minor planets), name (for unnumbered 
    /// minor planets and comets), the ephemeris object to be used for orbital calculations, an optional 
    /// ephemeris object to use for barycenter calculations, and an optional value for delta-T. 
    /// <para>The number values for major planets are 1 to 9 for Mercury to Pluto, 10 for Sun and 11 for Moon. The last two obviously 
    /// aren't planets, but this numbering is a NOVAS convention that enables us to retrieve useful information about these bodies.
    /// </para>
    /// <para>The high-level NOVAS astrometric functions are implemented as methods of Planet: 
    /// GetTopocentricPosition(), GetLocalPosition(), GetApparentPosition(), GetVirtualPosition(), 
    /// and GetAstrometricPosition(). These methods operate on the properties of the Planet, and produce 
    /// a PositionVector object. For example, to get the topocentric coordinates of a planet, create and 
    /// initialize a planet then call 
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
    public class Planet : IPlanet
    {
        internal const double C = 173.14463348d; // Speed of light in AU/Day.

        private double m_deltat;
        private bool m_bDTValid;
        private BodyType m_type;
        private int m_number;
        private string m_name;
        private IEphemeris m_ephobj;
        private readonly int[] m_ephdisps = new int[5], m_earthephdisps = new int[5];
        private IEphemeris m_earthephobj;

        /// <summary>
        /// Create a new instance of the Plant class
        /// </summary>
        /// <remarks>This assigns default Kepler instances for the Earth and Planet objects so it is
        /// not necessary to create and attach Kepler objects in order to get Kepler accuracy from this
        /// component</remarks>
        public Planet()
        {
            m_name = null;
            m_bDTValid = false;
            m_ephobj = new Ephemeris();
            m_earthephobj = new Ephemeris
            {
                BodyType = BodyType.MajorPlanet,
                Name = "Earth",
                Number = Body.Earth
            };
        }
        /// <summary>
        /// Planet delta-T
        /// </summary>
        /// <value>The value of delta-T (TT - UT1) to use for reductions</value>
        /// <returns>Seconds</returns>
        /// <remarks>Setting this value is optional. If no value is set, an internal delta-T generator is used.</remarks>
        public double DeltaT
        {
            get
            {
                if (!m_bDTValid)
                    throw new HelperException("Planet:DeltaT - DeltaT is not available");

                return m_deltat;
            }
            set
            {
                m_deltat = value;
                m_bDTValid = true;
            }
        }

        /// <summary>
        /// Ephemeris object used to provide the position of the Earth.
        /// </summary>
        /// <value>Earth ephemeris object</value>
        /// <returns>Earth ephemeris object</returns>
        /// <remarks>
        /// Setting this is optional, if not set, the internal Kepler engine will be used.</remarks>
        public IEphemeris EarthEphemeris
        {
            get
            {
                return m_earthephobj;
            }
            set
            {
                m_earthephobj = value;
            }
        }

        /// <summary>
        /// The Ephemeris object used to provide positions of solar system bodies.
        /// </summary>
        /// <value>Body ephemeris object</value>
        /// <returns>Body ephemeris object</returns>
        /// <remarks>
        /// Setting this is optional, if not set, the internal Kepler engine will be used.
        /// </remarks>
        public IEphemeris Ephemeris
        {
            get
            {
                return m_ephobj;
            }
            set
            {
                m_ephobj = value;
            }
        }

        /// <summary>
        /// Get an apparent position for given time
        /// </summary>
        /// <param name="tjd">Terrestrial Julian Date for the position</param>
        /// <returns>PositionVector for the apparent place.</returns>
        /// <remarks></remarks>
        public PositionVector GetApparentPosition(double tjd)
        {
            const double J2000BASE = 2451545.0d; // TDB Julian date of epoch J2000.0.
            double tdb = default, t2, t3, lighttime = default;
            double[] peb = new double[4], veb = new double[4], pes = new double[4], ves = new double[4], pos1 = new double[4], vel1 = new double[4], pos2 = new double[4], pos3 = new double[4], pos4 = new double[4], pos5 = new double[4], vec = new double[9];
            int iter;
            PositionVector pv;

            // This gets the barycentric terrestrial dynamical time (TDB).
            NovasComSupport.get_earth_nov(ref m_earthephobj, tjd, ref tdb, ref peb, ref veb, ref pes, ref ves);

            // Get position and velocity of planet wrt barycenter of solar system.
            NovasComSupport.ephemeris_nov(ref m_ephobj, tdb, m_type, m_number, m_name, Origin.Barycentric, ref pos1, ref vel1);

            Novas.Bary2Obs(pos1, peb, ref pos2, ref lighttime);
            t3 = tdb - lighttime;

            iter = 0;
            do
            {
                t2 = t3;
                NovasComSupport.ephemeris_nov(ref m_ephobj, t2, m_type, m_number, m_name, Origin.Barycentric, ref pos1, ref vel1);
                Novas.Bary2Obs(pos1, peb, ref pos2, ref lighttime);
                t3 = tdb - lighttime;
                iter += 1;
            }
            while (Abs(t3 - t2) > 0.000001d & iter < 100);

            // Finish apparent place computation.
            Novas.GravDef(tjd, EarthDeflection.AddEarthDeflection, Accuracy.Full, pos2, pes, ref pos3);
            Novas.Aberration(pos3, veb, lighttime, ref pos4);
            Novas.Precession(J2000BASE, pos4, tdb, ref pos5);
            Novas.Nutation(tdb, NutationDirection.MeanToTrue, Accuracy.Full, pos5, ref vec);

            pv = new PositionVector
            {
                x = vec[0],
                y = vec[1],
                z = vec[2]
            };

            return pv;
        }

        /// <summary>
        /// Get an astrometric position for given time
        /// </summary>
        /// <param name="tjd">Terrestrial Julian Date for the position</param>
        /// <returns>PositionVector for the astrometric place.</returns>
        /// <remarks></remarks>
        public PositionVector GetAstrometricPosition(double tjd)
        {
            double t2, t3;
            double lighttime = default, tdb = default;
            double[] pos1 = new double[4], vel1 = new double[4], pos2 = new double[4], peb = new double[4], veb = new double[4], pes = new double[4], ves = new double[4];
            int iter;
            PositionVector RetVal;

            NovasComSupport.get_earth_nov(ref m_earthephobj, tjd, ref tdb, ref peb, ref veb, ref pes, ref ves);

            // Get position and velocity of planet wrt barycenter of solar system.
            NovasComSupport.ephemeris_nov(ref m_ephobj, tdb, m_type, m_number, m_name, Origin.Barycentric, ref pos1, ref vel1);

            Novas.Bary2Obs(pos1, peb, ref pos2, ref lighttime);
            t3 = tdb - lighttime;

            iter = 0;
            do
            {
                t2 = t3;
                NovasComSupport.ephemeris_nov(ref m_ephobj, t2, m_type, m_number, m_name, Origin.Barycentric, ref pos1, ref vel1);
                Novas.Bary2Obs(pos1, peb, ref pos2, ref lighttime);
                t3 = tdb - lighttime;
                iter += 1;
            }
            while (Abs(t3 - t2) > 0.000001d & iter < 100);

            if (iter >= 100)
                throw new HelperException("Planet:GetAstrometricPoition - ephemeris_nov did not converge in 100 iterations");

            // pos2 is astrometric place.
            RetVal = new PositionVector
            {
                x = pos2[0],
                y = pos2[1],
                z = pos2[2]
            };

            return RetVal;
        }

        /// <summary>
        /// Get an local position for given time
        /// </summary>
        /// <param name="tjd">Terrestrial Julian Date for the position</param>
        /// <param name="site">The observing site</param>
        /// <returns>PositionVector for the local place.</returns>
        /// <remarks></remarks>
        public PositionVector GetLocalPosition(double tjd, Site site)
        {
            const double J2000BASE = 2451545.0d; // TDB Julian date of epoch J2000.0.
            int j, iter;
            var st = default(OnSurface);
            double t2, t3;
            double gast = default, lighttime = default, ujd, tdb = default, oblm = default, oblt = default, eqeq = default, psi = default, eps = default;
            double[] pog = new double[4], vog = new double[4], pb = new double[4], vb = new double[4], ps = new double[4], vs = new double[4], pos1 = new double[4], vel1 = new double[4], pos2 = new double[4], vel2 = new double[4], pos3 = new double[4], vec = new double[4], peb = new double[4], veb = new double[4], pes = new double[4], ves = new double[4];
            PositionVector pv;

            // Compute 'ujd', the UT1 Julian date corresponding to 'tjd'.
            if (!m_bDTValid) // April 2012 - corrected bug, delta t was not treated as seconds and also adapted to work with Novas31
            {
                m_deltat = Utilities.DeltaT(tjd);
            }
            ujd = tjd - m_deltat / 86400.0d;

            // Get the observer's site info
            try
            {
                st.Latitude = site.Latitude;
            }
            catch
            {
                throw new HelperException("Star:GetTopocentricPosition - Site.Latitude is not available");
            }
            try
            {
                st.Longitude = site.Longitude;
            }
            catch
            {
                throw new HelperException("Star:GetTopocentricPosition - Site.Longitude is not available");
            }
            try
            {
                st.Height = site.Height;
            }
            catch
            {
                throw new HelperException("Star:GetTopocentricPosition - Site.Height is not available");
            }

            // Get position of Earth wrt the center of the Sun and the barycenter
            // of solar system.
            //
            // This also gets the barycentric terrestrial dynamical time (TDB).
            NovasComSupport.get_earth_nov(ref m_earthephobj, tjd, ref tdb, ref peb, ref veb, ref pes, ref ves);

            Novas.ETilt(tdb, Accuracy.Full, ref oblm, ref oblt, ref eqeq, ref psi, ref eps);

            // Get position and velocity of observer wrt center of the Earth.
            //
            Novas.SiderealTime(ujd, 0.0d, DeltaT, GstType.GreenwichApparentSiderealTime, Method.CIOBased, Accuracy.Full, ref gast);

            Novas.Terra(st, gast, ref pos1, ref vel1);
            Novas.Nutation(tdb, NutationDirection.TrueToMean, Accuracy.Full, pos1, ref pos2);
            Novas.Precession(tdb, pos2, J2000BASE, ref pog);

            Novas.Nutation(tdb, NutationDirection.TrueToMean, Accuracy.Full, vel1, ref vel2);
            Novas.Precession(tdb, vel2, J2000BASE, ref vog);

            // Get position and velocity of observer wrt barycenter of solar 
            // system and wrt center of the sun.
            for (j = 0; j <= 2; j++)
            {
                pb[j] = peb[j] + pog[j];
                vb[j] = veb[j] + vog[j];
                ps[j] = pes[j] + pog[j];
                vs[j] = ves[j] + vog[j];
            }

            // Get position of planet wrt barycenter of solar system.
            NovasComSupport.ephemeris_nov(ref m_ephobj, tdb, m_type, m_number, m_name, Origin.Barycentric, ref pos1, ref vel1);

            Novas.Bary2Obs(pos1, pb, ref pos2, ref lighttime);
            t3 = tdb - lighttime;

            iter = 0;
            do
            {
                t2 = t3;
                NovasComSupport.ephemeris_nov(ref m_ephobj, t2, m_type, m_number, m_name, Origin.Barycentric, ref pos1, ref vel1);
                Novas.Bary2Obs(pos1, pb, ref pos2, ref lighttime);
                t3 = tdb - lighttime;
                iter += 1;
            }
            while (Abs(t3 - t2) > 0.000001d & iter < 100);

            if (iter >= 100)
                throw new HelperException("Planet:GetLocalPoition - ephemeris_nov did not converge in 100 iterations");

            // Finish local place calculation.
            Novas.GravDef(tjd, EarthDeflection.AddEarthDeflection, Accuracy.Full, pos2, ps, ref pos3);
            Novas.Aberration(pos3, vb, lighttime, ref vec);

            pv = new PositionVector
            {
                x = vec[0],
                y = vec[1],
                z = vec[2]
            };

            return pv;
        }

        /// <summary>
        /// Get a topocentric position for given time
        /// </summary>
        /// <param name="tjd">Terrestrial Julian Date for the position</param>
        /// <param name="site">The observing site</param>
        /// <param name="Refract">Apply refraction correction</param>
        /// <returns>PositionVector for the topocentric place.</returns>
        /// <remarks></remarks>
        public PositionVector GetTopocentricPosition(double tjd, Site site, bool Refract)
        {
            const double J2000BASE = 2451545.0d; // TDB Julian date of epoch J2000.0.
            short j;
            int iter;
            RefractionOption @ref;
            var st = default(OnSurface);
            double ujd, t2, t3, gast = default, lighttime = default, tdb = default, oblm = default, oblt = default, eqeq = default, psi = default, eps = default;
            double[] pos1 = new double[4], pos2 = new double[4], pos4 = new double[4], pos5 = new double[4], pos6 = new double[4], vel1 = new double[4], vel2 = new double[4], pog = new double[4], vog = new double[4], pob = new double[4], vec = new double[4], vob = new double[4], pos = new double[4], peb = new double[4], veb = new double[4], pes = new double[4], ves = new double[4];
            double ra = default, rra = default, dec = default, rdec = default, az = default, zd = default, dist = default;
            bool wx;
            PositionVector pv;

            // Compute 'ujd', the UT1 Julian date corresponding to 'tjd'.
            if (!m_bDTValid) // April 2012 - corrected bug, delta t was not treated as seconds and also adapted to work with Novas31
            {
                m_deltat = Utilities.DeltaT(tjd);
            }
            ujd = tjd - m_deltat / 86400.0d;

            // Get the observer's site info
            try
            {
                st.Latitude = site.Latitude;
            }
            catch
            {
                throw new HelperException("Star:GetTopocentricPosition - Site.Latitude is not available");
            }
            try
            {
                st.Longitude = site.Longitude;
            }
            catch
            {
                throw new HelperException("Star:GetTopocentricPosition - Site.Longitude is not available");
            }
            try
            {
                st.Height = site.Height;
            }
            catch
            {
                throw new HelperException("Star:GetTopocentricPosition - Site.Height is not available");
            }

            // This also gets the barycentric terrestrial dynamical time (TDB).
            NovasComSupport.get_earth_nov(ref m_earthephobj, tjd, ref tdb, ref peb, ref veb, ref pes, ref ves);

            Novas.ETilt(tdb, Accuracy.Full, ref oblm, ref oblt, ref eqeq, ref psi, ref eps);

            // Get position and velocity of observer wrt center of the Earth.
            Novas.SiderealTime(ujd, 0.0d, DeltaT, GstType.GreenwichApparentSiderealTime, Method.CIOBased, Accuracy.Full, ref gast);
            Novas.Terra(st, gast, ref pos1, ref vel1);

            Novas.Nutation(tdb, NutationDirection.TrueToMean, Accuracy.Full, pos1, ref pos2);
            Novas.Precession(tdb, pos2, J2000BASE, ref pog);

            Novas.Nutation(tdb, NutationDirection.TrueToMean, Accuracy.Full, vel1, ref vel2);
            Novas.Precession(tdb, vel2, J2000BASE, ref vog);

            // Get position and velocity of observer wrt barycenter of solar system
            // and wrt center of the sun.
            for (j = 0; j <= 2; j++)
            {

                pob[j] = peb[j] + pog[j];
                vob[j] = veb[j] + vog[j];
                pos[j] = pes[j] + pog[j];
            }

            // Compute the apparent place of the planet using the position and
            // velocity of the observer.
            //
            // First, get the position of the planet wrt barycenter of solar system.
            NovasComSupport.ephemeris_nov(ref m_ephobj, tdb, m_type, m_number, m_name, Origin.Barycentric, ref pos1, ref vel1);

            Novas.Bary2Obs(pos1, pob, ref pos2, ref lighttime);
            t3 = tdb - lighttime;

            iter = 0;
            do
            {
                t2 = t3;
                NovasComSupport.ephemeris_nov(ref m_ephobj, t2, m_type, m_number, m_name, Origin.Barycentric, ref pos1, ref vel1);
                Novas.Bary2Obs(pos1, pob, ref pos2, ref lighttime);
                t3 = tdb - lighttime;
                iter += 1;
            }
            while (Abs(t3 - t2) > 0.000001d & iter < 100);

            if (iter >= 100)
                throw new HelperException("Planet:GetTopocentricPoition - ephemeris_nov did not converge in 100 iterations");

            // Finish topocentric place calculation.
            Novas.GravDef(tjd, EarthDeflection.AddEarthDeflection, Accuracy.Full, pos2, pos, ref pos4);
            Novas.Aberration(pos4, vob, lighttime, ref pos5);
            Novas.Precession(J2000BASE, pos5, tdb, ref pos6);
            Novas.Nutation(tdb, NutationDirection.MeanToTrue, Accuracy.Full, pos6, ref vec);

            // Calculate equatorial coordinates and distance
            Novas.Vector2RaDec(vec, ref ra, ref dec); // Get topo RA/Dec
            dist = Sqrt(Pow(vec[0], 2.0d) + Pow(vec[1], 2.0d) + Pow(vec[2], 2.0d)); // And dist

            // Refract if requested
            @ref = RefractionOption.NoRefraction; // Assume no refraction
            if (Refract)
            {
                wx = true; // Assume site weather
                try
                {
                    st.Temperature = site.Temperature;
                }
                catch // Value unset so use standard refraction option
                {
                    wx = false;
                }

                try
                {
                    st.Pressure = site.Pressure;
                }
                catch // Value unset so use standard refraction option
                {
                    wx = false;
                }

                if (wx) // Set refraction option
                    @ref = RefractionOption.LocationRefraction;
                else
                    @ref = RefractionOption.StandardRefraction;
            }

            // This calculates Alt/Az coordinates. If ref > 0 then it refracts both the computed Alt/Az and the RA/Dec coordinates.
            if (m_bDTValid)
            {
                Novas.Equ2Hor(ujd, m_deltat, Accuracy.Full, 0.0d, 0.0d, st, ra, dec, @ref, ref zd, ref az, ref rra, ref rdec);
            }
            else
            {
                Novas.Equ2Hor(ujd, Utilities.DeltaT(tjd), Accuracy.Full, 0.0d, 0.0d, st, ra, dec, @ref, ref zd, ref az, ref rra, ref rdec);
            }

            // If we refracted, we now must compute new Cartesian components - Distance does not change...
            if (@ref != RefractionOption.NoRefraction)
                Novas.RaDec2Vector(rra, rdec, dist, ref vec); // If refracted, recompute New refracted vector

            // Create a new position vector with calculated values
            pv = new PositionVector(vec[0], vec[1], vec[2], rra, rdec, dist, dist / C, az, 90.0d - zd);

            return pv;
        }

        /// <summary>
        /// Get a virtual position for given time
        /// </summary>
        /// <param name="tjd">Terrestrial Julian Date for the position</param>
        /// <returns>PositionVector for the virtual place.</returns>
        /// <remarks></remarks>
        public PositionVector GetVirtualPosition(double tjd)
        {
            double t2, t3;
            double lighttime = default, tdb = default, oblm = default, oblt = default, eqeq = default, psi = default, eps = default;
            double[] pos1 = new double[4], vel1 = new double[4], pos2 = new double[4], pos3 = new double[4], vec = new double[4], peb = new double[4], veb = new double[4], pes = new double[4], ves = new double[4];
            int iter;
            var pv = new PositionVector();

            // Get position and velocity of Earth wrt barycenter of solar system.
            //
            // This also gets the barycentric terrestrial dynamical time (TDB).
            NovasComSupport.get_earth_nov(ref m_earthephobj, tjd, ref tdb, ref peb, ref veb, ref pes, ref ves);

            Novas.ETilt(tdb, Accuracy.Full, ref oblm, ref oblt, ref eqeq, ref psi, ref eps);

            // Get position and velocity of planet wrt barycenter of solar system.
            var km_type = default(BodyType);
            switch (m_type)
            {
                case BodyType.Comet:
                    km_type = BodyType.Comet;
                    break;
                case BodyType.MajorPlanet:
                    km_type = BodyType.MajorPlanet;
                    break;
                case BodyType.MinorPlanet:
                    km_type = BodyType.MinorPlanet;
                    break;
            }

            NovasComSupport.ephemeris_nov(ref m_ephobj, tdb, km_type, m_number, m_name, Origin.Barycentric, ref pos1, ref vel1);
            Novas.Bary2Obs(pos1, peb, ref pos2, ref lighttime);

            t3 = tdb - lighttime;

            iter = 0;
            do
            {
                t2 = t3;
                NovasComSupport.ephemeris_nov(ref m_ephobj, t2, km_type, m_number, m_name, Origin.Barycentric, ref pos1, ref vel1);
                Novas.Bary2Obs(pos1, peb, ref pos2, ref lighttime);
                t3 = tdb - lighttime;
                iter += 1;
            }
            while (Abs(t3 - t2) > 0.000001d & iter < 100);

            if (iter >= 100)
                throw new HelperException("Planet:GetVirtualPoition ephemeris_nov did not converge in 100 iterations");

            // Finish virtual place computation.
            Novas.GravDef(tjd, EarthDeflection.AddEarthDeflection, Accuracy.Full, pos2, pes, ref pos3);

            Novas.Aberration(pos3, veb, lighttime, ref vec);

            pv.x = vec[0];
            pv.y = vec[1];
            pv.z = vec[2];

            return pv;
        }

        /// <summary>
        /// Planet name
        /// </summary>
        /// <value>For unnumbered minor planets, (Type=nvMinorPlanet and Number=0), the packed designation 
        /// for the minor planet. For other types, this is not significant, but may be used to store 
        /// a name.</value>
        /// <returns>Name of planet</returns>
        /// <remarks></remarks>
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        /// <summary>
        /// Planet number
        /// </summary>
        /// <value>For major planets (Type = <see cref="BodyType.MajorPlanet" />, a PlanetNumber value from 1 to 11. For minor planets 
        /// (Type = <see cref="BodyType.MinorPlanet" />, the number of the minor planet or 0 for unnumbered minor planet.</value>
        /// <returns>Planet number</returns>
        /// <remarks>The major planet number is its number out from the sun starting with Mercury = 1, ending at Pluto = 9. Planet 10 gives 
        /// values for the Sun and planet 11 gives values for the Moon</remarks>
        public int Number
        {
            get
            {
                return m_number;
            }
            set
            {
                // April 2012 - corrected to disallow planet number 0
                if (m_type == BodyType.MajorPlanet & (value < 1 | value > 11))
                    throw new ASCOM.InvalidValueException($"Planet.Number - MajorPlanet number is < 1 or > 11 - {value}");

                m_number = value;
            }
        }

        /// <summary>
        /// The type of solar system body
        /// </summary>
        /// <value>The type of solar system body</value>
        /// <returns>Value from the BodyType enum</returns>
        /// <remarks></remarks>
        public BodyType Type
        {
            get
            {
                return m_type;
            }
            set
            {
                if ((int)value < 0 | (int)value > 2)
                    throw new ASCOM.InvalidValueException($"Planet.Type - BodyType is < 0 or > 2: {(int)value}");

                m_type = value;
            }
        }

        private static Body NumberToBody(int Number)
        {
            switch (Number)
            {
                case 1:
                    return Body.Mercury;
                case 2:
                    return Body.Venus;
                case 3:
                    return Body.Earth;
                case 4:
                    return Body.Mars;
                case 5:
                    return Body.Jupiter;
                case 6:
                    return Body.Saturn;
                case 7:
                    return Body.Uranus;
                case 8:
                    return Body.Neptune;
                case 9:
                    return Body.Pluto;
                case 10:
                    return Body.Sun;
                case 11:
                    return Body.Moon;

                default:
                    throw new ASCOM.InvalidValueException("PlanetNumberToBody", Number.ToString(), "1 to 11");
            }
        }
    }
}