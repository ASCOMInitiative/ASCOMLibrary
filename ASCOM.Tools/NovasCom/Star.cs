using ASCOM;
using static System.Math;
using ASCOM.Tools.Novas31;
using ASCOM.Tools;
using System.Threading;
using ASCOM.Common.Interfaces;
using ASCOM.Common;


namespace NovasCom
{
    /// <summary>
    /// NOVAS-COM: Star Class
    /// </summary>
    /// <remarks>NOVAS-COM objects of class Star contain the specifications for a star's catalog position in either FK5 or Hipparcos units (both must be J2000). Properties are right ascension and declination, proper motions, parallax, radial velocity, catalog type (FK5 or HIP), catalog number, optional ephemeris engine to use for barycenter calculations, and an optional value for delta-T. Unless you specifically set the DeltaT property, calculations performed by this class which require the value of delta-T (TT - UT1) rely on an internal function to estimate delta-T. 
    /// <para>The high-level NOVAS astrometric functions are implemented as methods of Star: 
    /// GetTopocentricPosition(), GetLocalPosition(), GetApparentPosition(), GetVirtualPosition(), 
    /// and GetAstrometricPosition(). These methods operate on the properties of the Star, and produce 
    /// a PositionVector object. For example, to get the topocentric coordinates of a star, simply create 
    /// and initialize a Star, then call Star.GetTopocentricPosition(). The resulting vaPositionVector's 
    /// right ascension and declination properties are the topocentric equatorial coordinates, at the same 
    /// time, the (optionally refracted) alt-az coordinates are calculated, and are also contained within 
    /// the returned PositionVector. <b>Note that Alt/Az is available in PositionVectors returned from calling 
    /// GetTopocentricPosition().</b></para></remarks>
    public class Star : IStar
    {
        const double J2000BASE = 2451545.0d; // TDB Julian date of epoch J2000.0.

        private double m_rv, m_plx, m_pmdec, m_pmra, m_ra, m_dec, m_deltat;
        private bool m_rav, m_decv, m_bDTValid;
        private object m_earthephobj;
        private string m_cat, m_name;
        private int m_num;
        private readonly Body m_earth;
        private short hr;
        private ILogger logger;

        /// <summary>
        /// Initialise a new instance of the star class
        /// </summary>
        /// <remarks></remarks>
        public Star() : this(null) { }

        /// <summary>
        /// Initialise the star class with an ILogger instance to record debug information
        /// </summary>
        /// <param name="logger">ILogger instance to which debug information will be logged. (A null value suppresses logging)</param>
        public Star(ILogger logger)
        {
            this.logger = logger;

            m_rv = 0.0d; // Defaults to 0.0
            m_plx = 0.0d; // Defaults to 0.0
            m_pmdec = 0.0d; // Defaults to 0.0
            m_pmra = 0.0d; // Defaults to 0.0
            m_rav = false; // RA not valid
            m_ra = 0.0d;
            m_decv = false; // Dec not valid
            m_dec = 0.0d;
            m_cat = ""; // \0''No names
            m_name = ""; // \0'
            m_num = 0;
            m_earthephobj = null; // No Earth ephemeris [sentinel]
            m_bDTValid = false; // Calculate delta-t
            m_earth = Body.Earth;
        }

        /// <summary>
        /// Three character catalog code for the star's data
        /// </summary>
        /// <value>Three character catalog code for the star's data</value>
        /// <returns>Three character catalog code for the star's data</returns>
        /// <remarks>Typically "FK5" but may be "HIP". For information only.</remarks>
        public string Catalog
        {
            get
            {
                return m_cat;
            }
            set
            {
                if (value.Length > 3)
                    throw new InvalidValueException("Star.Catalog - Catlog > 3 characters long: " + value);

                m_cat = value;
            }
        }

        /// <summary>
        /// Mean catalog J2000 declination coordinate (degrees)
        /// </summary>
        /// <value>Mean catalog J2000 declination coordinate</value>
        /// <returns>Degrees</returns>
        /// <remarks></remarks>
        public double Declination
        {
            get
            {
                if (!m_rav)
                    throw new HelperException("Star.Declination - Value not available");

                return m_dec;
            }
            set
            {
                m_dec = value;
                m_decv = true;
            }
        }

        /// <summary>
        /// The value of delta-T (TT - UT1) to use for reductions.
        /// </summary>
        /// <value>The value of delta-T (TT - UT1) to use for reductions.</value>
        /// <returns>Seconds</returns>
        /// <remarks>If this property is not set, calculations will use an internal function to estimate delta-T.</remarks>
        public double DeltaT
        {
            get
            {
                if (!m_bDTValid)
                    throw new HelperException("Star.DeltaT - Value not available");
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
        /// <value>Ephemeris object used to provide the position of the Earth.</value>
        /// <returns>Ephemeris object</returns>
        /// <remarks>If this value is not set, an internal Kepler object will be used to determine 
        /// Earth ephemeris</remarks>
        public object EarthEphemeris
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
        /// Get an apparent position for a given time
        /// </summary>
        /// <param name="tjd">Terrestrial Julian Date for the position</param>
        /// <returns>PositionVector for the apparent place.</returns>
        /// <remarks></remarks>
        public PositionVector GetApparentPosition(double tjd)
        {
            const double J2000BASE = 2451545.0d; // TDB Julian date of epoch J2000.0.
            var cat = new CatEntry3();
            var PV = new PositionVector();

            double tdb = default, time2 = default;
            double[] peb = new double[4], veb = new double[4], pes = new double[4], ves = new double[4], pos1 = new double[4], pos2 = new double[4], pos3 = new double[4], pos4 = new double[4], pos5 = new double[4], pos6 = new double[4], vel1 = new double[4], vec = new double[4];

            if (!(m_rav & m_decv))
                throw new HelperException("Star.GetApparentPosition - RA or DEC not available");

            // Get the position and velocity of the Earth w/r/t the solar system
            // barycenter and the center of mass of the Sun, on the mean equator
            // and equinox of J2000.0
            //
            // This also gets the barycentric terrestrial dynamical time (TDB).
            hr = GetEarth(tjd, m_earth, ref tdb, ref peb, ref veb, ref pes, ref ves);
            if (hr > 0)
            {
                vec[0] = 0.0d;
                vec[1] = 0.0d;
                vec[2] = 0.0d;
                throw new HelperException($"get_earth - Star.GetApparentPosition. RC: {hr}");
            }

            cat.RA = m_ra;
            cat.Dec = m_dec;
            cat.ProMoRA = m_pmra;
            cat.ProMoDec = m_pmdec;
            cat.Parallax = m_plx;
            cat.RadialVelocity = m_rv;

            Novas.StarVectors(cat, ref pos1, ref vel1);
            Novas.ProperMotion(J2000BASE, pos1, vel1, tdb, ref pos2);

            Novas.Bary2Obs(pos2, peb, ref pos3, ref time2);
            Novas.GravDef(tjd, EarthDeflection.AddEarthDeflection, Accuracy.Full, pos3, pes, ref pos4);
            Novas.Aberration(pos4, veb, time2, ref pos5);
            Novas.Precession(J2000BASE, pos5, tdb, ref pos6);
            Novas.Nutation(tdb, NutationDirection.MeanToTrue, Accuracy.Full, pos6, ref vec);

            PV.x = vec[0];
            PV.y = vec[1];
            PV.z = vec[2];

            return PV;
        }

        /// <summary>
        /// Obtains the barycentric and heliocentric positions and velocities of the Earth from the solar system ephemeris.
        /// </summary>
        /// <param name="tjd"></param>
        /// <param name="earth"></param>
        /// <param name="tdb"></param>
        /// <param name="bary_earthp"></param>
        /// <param name="bary_earthv"></param>
        /// <param name="helio_earthp"></param>
        /// <param name="helio_earthv"></param>
        /// <returns></returns>
        /// <remarks>
        /// PURPOSE:
        ///    Obtains the barycentric and heliocentric positions and velocities
        ///    of the Earth from the solar system ephemeris.
        ///
        /// REFERENCES:
        ///    None.
        ///
        /// INPUT
        /// ARGUMENTS:
        ///    tjd (double)
        ///      TT (or TDT) Julian date.
        ///    *earth (struct body)
        ///      Pointer to structure containing the body designation for the
        ///       Earth (defined in novas.h).
        /// OUTPUT
        /// ARGUMENTS:
        ///    *tdb (double)
        ///       TDB Julian date corresponding to 'tjd'.
        ///    *bary_earthp (double)
        ///       Barycentric position vector of Earth at 'tjd'; equatorial
        ///       rectangular coordinates in AU referred to the mean equator
        ///       and equinox of J2000.0.
        ///    *bary_earthv (double)
        ///       Barycentric velocity vector of Earth at 'tjd'; equatorial
        ///       rectangular system referred to the mean equator and equinox 
        ///       of J2000.0, in AU/Day.
        ///    *helio_earthp (double)
        ///       Heliocentric position vector of Earth at 'tjd'; equatorial
        ///       rectangular coordinates in AU referred to the mean equator
        ///       and equinox of J2000.0.
        ///    *helio_earthv (double)
        ///       Heliocentric velocity vector of Earth at 'tjd'; equatorial
        ///       rectangular system referred to the mean equator and equinox
        ///       of J2000.0, in AU/Day.
        ///
        /// RETURNED
        /// VALUE:
        ///    (short int)
        ///        0...Everything OK.
        ///       >0...Error code from function 'solarsystem'.
        ///
        /// GLOBALS
        /// USED:
        ///    BARYC, HELIOC
        /// FUNCTIONS
        /// CALLED:
        ///    tdb2tdt             novas.c
        ///    solarsystem         (user's choice)
        ///    fabs                math.h
        ///
        /// VER./DATE/
        /// PROGRAMMER:
        ///    V1.0/10-95/WTH (USNO/AA)
        ///    V1.1/06-97/JAB (USNO/AA): Incorporate 'body' structure in input.
        ///
        /// NOTES:
        ///    None.
        /// </remarks>
        public short GetEarth(double tjd, Body earth, ref double tdb, ref double[] bary_earthp, ref double[] bary_earthv, ref double[] helio_earthp, ref double[] helio_earthv)
        {
            short error = 0;
            short i;

            double time1;
            double[] peb = new double[3];
            double[] veb = new double[3];
            double[] pes = new double[3];
            double[] ves = new double[3];

            double dummy = 0.0, secdiff = 0.0;

            // Compute the TDB Julian date corresponding to 'tjd'.
            Novas.Tdb2Tt(tjd, ref dummy, ref secdiff);
            time1 = tjd + secdiff / 86400.0;

            // Get position and velocity of the Earth wrt barycenter of solar systemand wrt center of the sun.
            if (Novas.SolarSystem(time1, earth, Origin.Barycentric, ref peb, ref veb) > 0)
            {
                return error;
            }

            if (Novas.SolarSystem(time1, earth, Origin.Heliocentric, ref pes, ref ves) > 0)
            {
                return error;
            }

            tdb = time1;
            for (i = 0; i < 3; i++)
            {
                bary_earthp[i] = peb[i];
                bary_earthv[i] = veb[i];
                helio_earthp[i] = pes[i];
                helio_earthv[i] = ves[i];
            }

            return error;
        }

        /// <summary>
        /// This is the NOVAS-COM implementation of astro_star(). See the
        /// original NOVAS-C sources for more info.
        ///
        /// Get an astrometric position for a given time
        /// </summary>
        /// <param name="tjd">Terrestrial Julian Date for the position</param>
        /// <returns>PositionVector for the astrometric place.</returns>
        /// <remarks></remarks>
        public PositionVector GetAstrometricPosition(double tjd)
        {
            var cat = new CatEntry3();
            var PV = new PositionVector();
            double lighttime = default, tdb = default;
            double[] pos1 = new double[4], vel1 = new double[4], pos2 = new double[4], peb = new double[4], veb = new double[4], pes = new double[4], ves = new double[4], vec = new double[4];

            if (!(m_rav & m_decv))
                throw new HelperException("Star.GetAstrometricPosition - RA or DEC not available");

            // Get the position and velocity of the Earth w/r/t the solar system
            // barycenter and the center of mass of the Sun, on the mean equator
            // and equinox of J2000.0
            //
            // This also gets the barycentric terrestrial dynamical time (TDB).
            hr = GetEarth(tjd, m_earth, ref tdb, ref peb, ref veb, ref pes, ref ves);
            if (hr > 0)
            {
                vec[0] = 0.0d;
                vec[1] = 0.0d;
                vec[2] = 0.0d;
                throw new HelperException($"get_earth - Star.GetApparentPosition. RC: {hr}");
            }

            cat.RA = m_ra;
            cat.Dec = m_dec;
            cat.ProMoRA = m_pmra;
            cat.ProMoDec = m_pmdec;
            cat.Parallax = m_plx;
            cat.RadialVelocity = m_rv;

            // Compute astrometric place.
            Novas.StarVectors(cat, ref pos1, ref vel1);
            Novas.ProperMotion(J2000BASE, pos1, vel1, tdb, ref pos2);
            Novas.Bary2Obs(pos2, peb, ref vec, ref lighttime);

            PV.x = vec[0];
            PV.y = vec[1];
            PV.z = vec[2];

            return PV;

        }

        /// <summary>
        /// Get a local position for a given site and time
        /// </summary>
        /// <param name="tjd">Terrestrial Julian Date for the position</param>
        /// <param name="site">A Site object representing the observing site</param>
        /// <returns>PositionVector for the local place.</returns>
        /// <remarks></remarks>
        public PositionVector GetLocalPosition(double tjd, Site site)
        {
            var cat = new CatEntry3();
            var PV = new PositionVector();
            var st = new OnSurface();
            double gast = default, lighttime = default, ujd, tdb = default, oblm = default, oblt = default, eqeq = default, psi = default, eps = default;
            double[] pog = new double[4], vog = new double[4], pb = new double[4], vb = new double[4], ps = new double[4], vs = new double[4], pos1 = new double[4], vel1 = new double[4], pos2 = new double[4], vel2 = new double[4], pos3 = new double[4], pos4 = new double[4], peb = new double[4], veb = new double[4], pes = new double[4], ves = new double[4], vec = new double[4];
            int j;

            if (!(m_rav & m_decv))
                throw new HelperException("Star.GetLocalPosition - RA or DEC not available");

            // Compute 'ujd', the UT1 Julian date corresponding to 'tjd'.
            if (m_bDTValid)
            {
                ujd = tjd - m_deltat;
            }
            else
            {
                ujd = tjd - Utilities.DeltaT(tjd) / 86400.0d;
            }

            cat.RA = m_ra;
            cat.Dec = m_dec;
            cat.ProMoRA = m_pmra;
            cat.ProMoDec = m_pmdec;
            cat.Parallax = m_plx;
            cat.RadialVelocity = m_rv;

            try
            {
                st.Latitude = site.Latitude;
            }
            catch
            {
                throw new HelperException("Star:GetLocalPosition - Site.Latitude is not available");
            }

            try
            {
                st.Longitude = site.Longitude;
            }
            catch
            {
                throw new HelperException("Star:GetLocalPosition - Site.Longitude is not available");
            }

            try
            {
                st.Height = site.Height;
            }
            catch
            {
                throw new HelperException("Star:GetLocalPosition - Site.Height is not available");
            }

            st.Pressure = site.Pressure;
            st.Temperature = site.Temperature;

            // Compute position and velocity of the observer, on mean equator
            // and equinox of J2000.0, wrt the solar system barycenter and
            // wrt to the center of the Sun.
            //
            // This also gets the barycentric terrestrial dynamical time (TDB).
            hr = GetEarth(tjd, m_earth, ref tdb, ref peb, ref veb, ref pes, ref ves);
            if (hr > 0)
            {
                vec[0] = 0.0d;
                vec[1] = 0.0d;
                vec[2] = 0.0d;
                throw new HelperException($"get_earth - Star.GetApparentPosition. RC: {hr}");
            }

            Novas.ETilt(tdb, Accuracy.Full, ref oblm, ref oblt, ref eqeq, ref psi, ref eps);

            Novas.SiderealTime(ujd, 0.0d, DeltaT, GstType.GreenwichApparentSiderealTime, Method.CIOBased, Accuracy.Full, ref gast);
            Novas.Terra(st, gast, ref pos1, ref vel1);
            Novas.Nutation(tdb, NutationDirection.TrueToMean, Accuracy.Full, pos1, ref pos2);
            Novas.Precession(tdb, pos2, J2000BASE, ref pog);

            Novas.Nutation(tdb, NutationDirection.TrueToMean, Accuracy.Full, vel1, ref vel2);
            Novas.Precession(tdb, vel2, J2000BASE, ref vog);

            for (j = 0; j <= 2; j++)
            {
                pb[j] = peb[j] + pog[j];
                vb[j] = veb[j] + vog[j];
                ps[j] = pes[j] + pog[j];
                vs[j] = ves[j] + vog[j];
            }

            // Compute local place.
            Novas.StarVectors(cat, ref pos1, ref vel1);
            Novas.ProperMotion(J2000BASE, pos1, vel1, tdb, ref pos2);
            Novas.Bary2Obs(pos2, pb, ref pos3, ref lighttime);
            Novas.GravDef(tjd, EarthDeflection.AddEarthDeflection, Accuracy.Full, pos3, ps, ref pos4);
            Novas.Aberration(pos4, vb, lighttime, ref vec);

            PV.x = vec[0];
            PV.y = vec[1];
            PV.z = vec[2];

            return PV;
        }

        /// <summary>
        /// Get a topocentric position for a given site and time
        /// </summary>
        /// <param name="tjd">Terrestrial Julian Date for the position</param>
        /// <param name="site">A Site object representing the observing site</param>
        /// <param name="Refract">True to apply atmospheric refraction corrections</param>
        /// <returns>PositionVector for the topocentric place.</returns>
        /// <remarks></remarks>
        public PositionVector GetTopocentricPosition(double tjd, Site site, bool Refract)
        {
            const double C = 173.14463348d; // Speed of light in AU/Day.

            RefractionOption refractionOption;
            var cat = new CatEntry3();
            var st = new OnSurface();
            double ujd;
            double[] vec = new double[4];
            double ra = default, rra = default, dec = default, rdec = default, az = default, zd = default, dist;
            bool wx;

            if (!(m_rav & m_decv))
                throw new HelperException("Star.GetTopocentricPosition - RA or DEC not available");

            // Compute 'ujd', the UT1 Julian date corresponding to 'tjd'.
            if (m_bDTValid)
            {
                ujd = tjd - m_deltat / 86400.0d;
            }
            else
            {
                ujd = tjd - Utilities.DeltaT(tjd) / 86400.0d;
            }

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
            st.Pressure = site.Pressure;
            st.Temperature = site.Temperature;

            cat.RA = m_ra;
            cat.Dec = m_dec;
            cat.ProMoRA = m_pmra;
            cat.ProMoDec = m_pmdec;
            cat.Parallax = m_plx;
            cat.RadialVelocity = m_rv;

            Novas.TopoStar(tjd, m_deltat, cat, st, Accuracy.Full, ref ra, ref dec);
            Novas.RaDec2Vector(ra, dec, 2.062648062E14, ref vec); // New refracted vector
            dist = Sqrt(Pow(vec[0], 2.0d) + Pow(vec[1], 2.0d) + Pow(vec[2], 2.0d)); // And dist

            // Refract if requested
            refractionOption = RefractionOption.NoRefraction; // Assume no refraction
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
                catch  // Value unset so use standard refraction option
                {
                    wx = false;
                }

                if (wx) // Set refraction option
                {
                    refractionOption = RefractionOption.LocationRefraction;
                }
                else
                {
                    refractionOption = RefractionOption.StandardRefraction;
                }
            }

            // This calculates Alt/Az coordinates. If ref > 0 then it refracts
            // both the computed Alt/Az and the RA/Dec coordinates.
            if (m_bDTValid)
            {
                Novas.Equ2Hor(ujd, m_deltat, Accuracy.Full, 0.0d, 0.0d, st, ra, dec, refractionOption, ref zd, ref az, ref rra, ref rdec);
            }
            else
            {
                Novas.Equ2Hor(ujd, Utilities.DeltaT(tjd), Accuracy.Full, 0.0d, 0.0d, st, ra, dec, refractionOption, ref zd, ref az, ref rra, ref rdec);
            }

            // If we refracted, we now must compute new Cartesian components
            // Distance does not change...
            if ((int)refractionOption > 0) // If refracted, recompute 
            {
                Novas.RaDec2Vector(rra, rdec, dist, ref vec); // New refracted vector
            }

            // Create a new position vector with calculated values
            var PV = new PositionVector(vec[0], vec[1], vec[2], rra, rdec, dist, dist / C, az, 90.0d - zd);

            return PV;
        }

        /// <summary>
        /// Get a virtual position at a given time
        /// </summary>
        /// <param name="tjd">Terrestrial Julian Date for the position</param>
        /// <returns>PositionVector for the virtual place.</returns>
        /// <remarks></remarks>
        public PositionVector GetVirtualPosition(double tjd)
        {
            // This is the NOVAS-COM implementation of virtual_star(). See the
            // original NOVAS-C sources for more info.
            var cat = new CatEntry3();
            var PV = new PositionVector();

            double[] pos1 = new double[4], vel1 = new double[4], pos2 = new double[4], pos3 = new double[4], pos4 = new double[4], peb = new double[4], veb = new double[4], pes = new double[4], ves = new double[4], vec = new double[4];
            double tdb = default, lighttime = default;

            if (!(m_rav & m_decv))
                throw new HelperException("Star.GetVirtualPosition - RA or DEC not available");

            cat.RA = m_ra;
            cat.Dec = m_dec;
            cat.ProMoRA = m_pmra;
            cat.ProMoDec = m_pmdec;
            cat.Parallax = m_plx;
            cat.RadialVelocity = m_rv;

            // Compute position and velocity of the observer, on mean equator
            // and equinox of J2000.0, wrt the solar system barycenter and
            // wrt to the center of the Sun.
            //
            // This also gets the barycentric terrestrial dynamical time (TDB).
            hr = GetEarth(tjd, m_earth, ref tdb, ref peb, ref veb, ref pes, ref ves);
            if (hr > 0)
            {
                vec[0] = 0.0d;
                vec[1] = 0.0d;
                vec[2] = 0.0d;
                throw new HelperException($"get_earth - Star.GetApparentPosition. RC: {hr}");
            }

            // Compute virtual place.
            Novas.StarVectors(cat, ref pos1, ref vel1);
            Novas.ProperMotion(J2000BASE, pos1, vel1, tdb, ref pos2);
            Novas.Bary2Obs(pos2, peb, ref pos3, ref lighttime);
            Novas.GravDef(tjd, EarthDeflection.AddEarthDeflection, Accuracy.Full, pos3, pes, ref pos4);
            Novas.Aberration(pos4, veb, lighttime, ref vec);

            PV.x = vec[0];
            PV.y = vec[1];
            PV.z = vec[2];

            return PV;
        }

        /// <summary>
        /// The catalog name of the star (50 char max)
        /// </summary>
        /// <value>The catalog name of the star</value>
        /// <returns>Name (50 char max)</returns>
        /// <remarks></remarks>
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                if (value.Length > 50)
                    throw new ASCOM.InvalidValueException("Star.Name - Name > 50 characters long: " + value);

                m_name = value;
            }
        }

        /// <summary>
        /// The catalog number of the star
        /// </summary>
        /// <value>The catalog number of the star</value>
        /// <returns>The catalog number of the star</returns>
        /// <remarks></remarks>
        public int Number
        {
            get
            {
                return m_num;
            }
            set
            {
                m_num = value;
            }
        }

        /// <summary>
        /// Catalog mean J2000 parallax (arcsec)
        /// </summary>
        /// <value>Catalog mean J2000 parallax</value>
        /// <returns>Arc seconds</returns>
        /// <remarks></remarks>
        public double Parallax
        {
            get
            {
                return m_plx;
            }
            set
            {
                m_plx = value;
            }
        }

        /// <summary>
        /// Catalog mean J2000 proper motion in declination (arcsec/century)
        /// </summary>
        /// <value>Catalog mean J2000 proper motion in declination</value>
        /// <returns>Arc seconds per century</returns>
        /// <remarks></remarks>
        public double ProperMotionDec
        {
            get
            {
                return m_pmdec;
            }
            set
            {
                m_pmdec = value;
            }
        }

        /// <summary>
        /// Catalog mean J2000 proper motion in right ascension (sec/century)
        /// </summary>
        /// <value>Catalog mean J2000 proper motion in right ascension</value>
        /// <returns>Seconds per century</returns>
        /// <remarks></remarks>
        public double ProperMotionRA
        {
            get
            {
                return m_pmra;
            }
            set
            {
                m_pmra = value;
            }
        }

        /// <summary>
        /// Catalog mean J2000 radial velocity (km/sec)
        /// </summary>
        /// <value>Catalog mean J2000 radial velocity</value>
        /// <returns>Kilometers per second</returns>
        /// <remarks></remarks>
        public double RadialVelocity
        {
            get
            {
                return m_rv;
            }
            set
            {
                m_rv = value;
            }
        }

        /// <summary>
        /// Catalog mean J2000 right ascension coordinate (hours)
        /// </summary>
        /// <value>Catalog mean J2000 right ascension coordinate</value>
        /// <returns>Hours</returns>
        /// <remarks></remarks>
        public double RightAscension
        {
            get
            {
                if (!m_rav)
                    throw new HelperException("Star.RightAscension - Value not available");

                return m_ra;
            }
            set
            {
                m_ra = value;
                m_rav = true;
            }
        }

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
        public void Set(double RA, double Dec, double ProMoRA, double ProMoDec, double Parallax, double RadVel)
        {
            m_ra = RA;
            m_dec = Dec;
            m_pmra = ProMoRA;
            m_pmdec = ProMoDec;
            m_plx = Parallax;
            m_rv = RadVel;
            m_rav = true;
            m_decv = true;
            m_num = 0;
            m_name = ""; // \0';
            m_cat = ""; // \0';
        }

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
        public void SetHipparcos(double RA, double Dec, double ProMoRA, double ProMoDec, double Parallax, double RadVel)
        {
            CatEntry3 hip = new CatEntry3
            {
                RA = RA,
                Dec = Dec,
                ProMoRA = ProMoRA,
                ProMoDec = ProMoDec,
                Parallax = Parallax,
                RadialVelocity = RadVel
            }, fk5 = new CatEntry3();

            Novas.TransformHip(hip, ref fk5);

            m_ra = fk5.RA;
            m_dec = fk5.Dec;
            m_pmra = fk5.ProMoRA;
            m_pmdec = fk5.ProMoDec;
            m_plx = fk5.Parallax;
            m_rv = fk5.RadialVelocity;
            m_rav = true;
            m_decv = true;
            m_num = 0;
            m_name = ""; // \0';
            m_cat = ""; // \0';
        }

        private void LogMessage(string message)
        {
            logger?.LogMessage(LogLevel.Debug, "NovasCom.Star", message);
        }
    }

}