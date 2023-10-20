using static System.Math;
using ASCOM.Tools.Novas31;
using ASCOM.Tools.Interfaces;

namespace ASCOM.Tools
{
    /// <summary>
    /// NOVAS-COM: VelocityVector Class
    /// </summary>
    /// <remarks>NOVAS-COM objects of class VelocityVector contain vectors used for velocities (earth, sites, 
    /// planets, and stars) throughout NOVAS-COM. Of course, its properties include the x, y, and z 
    /// components of the velocity. Additional properties are the velocity in equatorial coordinates of 
    /// right ascension dot, declination dot and radial velocity. You can initialize a PositionVector from 
    /// a Star object (essentially an FK5 or HIP catalog entry) or a Site (lat/long/height). For the star 
    /// object the proper motions, distance and radial velocity are used, for a site, the velocity is that 
    /// of the observer with respect to the Earth's center of mass. </remarks>
    public class VelocityVector : IVelocityVector
    {
        private bool m_xv, m_yv, m_zv, m_cv;
        private readonly double[] m_v = new double[3];
        private double m_VRA, m_RadVel, m_VDec;

        /// <summary>
        /// Creates a new velocity vector object
        /// </summary>
        /// <remarks> </remarks>
        public VelocityVector()
        {
            m_xv = false; // Vector is not valid
            m_yv = false;
            m_zv = false;
            m_cv = false; // Coordinate velocities not valid
        }

        /// <summary>
        /// Linear velocity along the declination direction (AU/day)
        /// </summary>
        /// <value>Linear velocity along the declination direction</value>
        /// <returns>AU/day</returns>
        /// <remarks>This is not the proper motion (which is an angular rate and is dependent on the distance to the object).</remarks>
        public double DecVelocity
        {
            get
            {
                if (!(m_xv & m_yv & m_zv))
                    throw new HelperException("VelocityVector:DecVelocity - x, y or z has not been set");

                CheckEq();
                return m_VDec;
            }
        }

        /// <summary>
        /// Linear velocity along the radial direction (AU/day)
        /// </summary>
        /// <value>Linear velocity along the radial direction</value>
        /// <returns>AU/day</returns>
        /// <remarks></remarks>
        public double RadialVelocity
        {
            get
            {
                if (!(m_xv & m_yv & m_zv))
                    throw new HelperException("VelocityVector:RadialVelocity - x, y or z has not been set");

                CheckEq();
                return m_RadVel;
            }
        }

        /// <summary>
        /// Linear velocity along the right ascension direction (AU/day)
        /// </summary>
        /// <value>Linear velocity along the right ascension direction</value>
        /// <returns>AU/day</returns>
        /// <remarks></remarks>
        public double RAVelocity
        {
            get
            {
                if (!(m_xv & m_yv & m_zv))
                    throw new HelperException("VelocityVector:RAVelocity - x, y or z has not been set");

                CheckEq();
                return m_VRA;
            }
        }

        /// <summary>
        /// Initialize the VelocityVector from a Site object and Greenwich Apparent Sdereal Time.
        /// </summary>
        /// <param name="site">The Site object from which to initialize</param>
        /// <param name="gast">Greenwich Apparent Sidereal Time</param>
        /// <returns>True if OK or throws an exception</returns>
        /// <remarks>The velocity vector is that of the observer with respect to the Earth's center 
        /// of mass. The GAST parameter must be for Greenwich, not local. The time is rotated through 
        /// the site longitude. See SetFromSiteJD() for an equivalent method that takes UTC Julian 
        /// Date and optionally Delta-T (eliminating the need for calculating hyper-accurate GAST yourself). </remarks>
        public bool SetFromSite(Site site, double gast)
        {
            const double f = 0.00335281d; // f = Earth ellipsoid flattening
            const double omega = 0.000072921151467d; // omega = Earth angular velocity rad/sec
            const double DEG2RAD = 0.017453292519943295d;
            const double EARTHRAD = 6378.14d; // Radius of Earth in kilometres.
            const double KMAU = 149597870.0d; // Astronomical Unit in kilometres.
            double df2, t, sinphi, cosphi, c, s, ach, ash, stlocl, sinst, cosst;

            // Compute parameters relating to geodetic to geocentric conversion.
            df2 = Pow(1.0d - f, 2d);
            try
            {
                t = site.Latitude;
            }
            catch
            {
                throw new HelperException("VelocityVector:SetFromSite - Site.Latitude is not available");
            }

            t *= DEG2RAD;
            sinphi = Sin(t);
            cosphi = Cos(t);
            c = 1.0d / Sqrt(Pow(cosphi, 2.0d) + df2 * Pow(sinphi, 2.0d));
            s = df2 * c;
            try
            {
                t = site.Height;
            }
            catch 
            {
                throw new HelperException("VelocityVector:SetFromSite - Site.Height is not available");
            }

            t /= 1000d; // Elevation in KM
            ach = EARTHRAD * c + t;
            ash = EARTHRAD * s + t;

            //
            // Compute local sidereal time factors at the observer's longitude.
            //
            try
            {
                t = site.Longitude;
            }
            catch 
            {
                throw new HelperException("VelocityVector:SetFromSite - Site.Longitude is not available");
            }

            stlocl = (gast * 15.0d + t) * DEG2RAD;
            sinst = Sin(stlocl);
            cosst = Cos(stlocl);

            // Compute velocity vector components in AU/Day
            m_v[0] = -omega * ach * cosphi * sinst * 86400.0d / KMAU;
            m_v[1] = omega * ach * cosphi * cosst * 86400.0d / KMAU;
            m_v[2] = 0.0d;

            m_xv = true;
            m_yv = true;
            m_zv = true; // Vector is complete
            m_cv = false; // Not interesting for Site vector anyway

            return true;
        }

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
        public bool SetFromSiteJD(Site site, double ujd)
        {
            SetFromSiteJD(site, ujd, 0.0d);
            return default;
        }

        /// <summary>
        /// Initialize the VelocityVector from a Site object using UTC Julian Date and Delta-T
        /// </summary>
        /// <param name="site">The Site object from which to initialize</param>
        /// <param name="ujd">UTC Julian Date</param>
        /// <param name="delta_t">The optional value of Delta-T (TT - UT1) to use for reductions (seconds)</param>
        /// <returns>True if OK otherwise throws an exception</returns>
        /// <remarks>The velocity vector is that of the observer with respect to the Earth's center 
        /// of mass. The Julian date must be UTC Julian date, not terrestrial.</remarks>
        public bool SetFromSiteJD(Site site, double ujd, double delta_t)
        {
            double dummy = default, secdiff = default, tdb, tjd, gast = default;
            double oblm = default, oblt = default, eqeq = default, psi = default, eps = default;

            // Convert UTC Julian date to Terrestrial Julian Date then
            // convert that to barycentric for earthtilt(), which we use
            // to get the equation of equinoxes for sidereal_time(). Note
            // that we're using UJD as input to the deltat(), but that is
            // OK as the difference in time (~70 sec) is insignificant.
            // For precise applications, the caller must specify delta_t.
            if (delta_t != 0.0d)
            {
                tjd = ujd + delta_t;
            }
            else
            {
                tjd = ujd + Utilities.DeltaT(ujd);
            }

            Novas.Tdb2Tt(tjd, ref dummy, ref secdiff);
            tdb = tjd + secdiff / 86400.0d;
            Novas.ETilt(tdb, Accuracy.Full, ref oblm, ref oblt, ref eqeq, ref psi, ref eps);

            // Get the Greenwich Apparent Sidereal Time and call our SetFromSite() method.
            Novas.SiderealTime(ujd, 0.0d, Utilities.DeltaT(ujd), GstType.GreenwichApparentSiderealTime, Method.CIOBased, Accuracy.Full, ref gast);

            SetFromSite(site, gast);
            return true;
        }

        /// <summary>
        /// Initialize the VelocityVector from a Star object.
        /// </summary>
        /// <param name="star">The Star object from which to initialize</param>
        /// <returns>True if OK otherwise throws an exception</returns>
        /// <remarks>The proper motions, distance and radial velocity are used in the velocity calculation. </remarks>
        /// <exception cref="HelperException">If any of: Parallax, RightAscension, Declination, 
        /// ProperMotionRA, ProperMotionDec or RadialVelocity are not available in the star object</exception>
        public bool SetFromStar(Star star)
        {
            const double DEG2RAD = 0.017453292519943295d;
            const double KMAU = 149597870.0d; // Astronomical Unit in kilometres.
            double t, paralx, r, d, cra, sra, cdc, sdc;

            // If parallax is unknown, undetermined, or zero, set it to 1e-7 second of arc, corresponding to a distance of 10 megaparsecs.
            try
            {
                paralx = star.Parallax;
            }
            catch
            {
                throw new HelperException("VelocityVector:SetFromStar - Star.Parallax is not available");
            }

            if (paralx <= 0.0d)
                paralx = 0.0000001d;

            // Convert right ascension, declination, and parallax to position vector in equatorial system with units of AU.
            try
            {
                r = star.RightAscension;
            }
            catch
            { 
                throw new HelperException("VelocityVector:SetFromStar - Star.RightAscension is not available");
            }

            try
            {
                d = star.Declination;
            }
            catch
            {
                throw new HelperException("VelocityVector:SetFromStar - Star.Declination is not available");
            }

            d *= DEG2RAD;

            cra = Cos(r);
            sra = Sin(r);
            cdc = Cos(d);
            sdc = Sin(d);

            // Convert proper motion and radial velocity to orthogonal components of motion with units of AU/Day.
            try
            {
                t = star.ProperMotionRA;
            }
            catch 
            {
                throw new HelperException("VelocityVector:SetFromStar - Star.ProperMotionRA is not available");
            }

            m_VRA = t * 15.0d * cdc / (paralx * 36525.0d);
            try
            {
                t = star.ProperMotionDec;
            }
            catch 
            {
                throw new HelperException("VelocityVector:SetFromStar - Star.ProperMotionDec is not available");
            }
            m_VDec = t / (paralx * 36525.0d);
            try
            {
                t = star.RadialVelocity;
            }
            catch 
            {
                throw new HelperException("VelocityVector:SetFromStar - Star.RadialVelocity is not available");
            }

            m_RadVel = t * 86400.0d / KMAU;

            //
            // Transform motion vector to equatorial system.
            //
            m_v[0] = -m_VRA * sra - m_VDec * sdc * cra + m_RadVel * cdc * cra;
            m_v[1] = m_VRA * cra - m_VDec * sdc * sra + m_RadVel * cdc * sra;
            m_v[2] = m_VDec * cdc + m_RadVel * sdc;

            m_xv = true;
            m_yv = true;
            m_zv = true; // Vector is complete
            m_cv = true; // We have it all!

            return true;
        }

        /// <summary>
        /// Cartesian x component of velocity (AU/day)
        /// </summary>
        /// <value>Cartesian x component of velocity</value>
        /// <returns>AU/day</returns>
        /// <remarks></remarks>
        public double x
        {
            get
            {
                if (!m_xv)
                    throw new HelperException("VelocityVector:x - x value has not been set");

                return m_v[0];
            }
            set
            {
                m_v[0] = value;
                m_xv = true;
            }
        }

        /// <summary>
        /// Cartesian y component of velocity (AU/day)
        /// </summary>
        /// <value>Cartesian y component of velocity</value>
        /// <returns>AU/day</returns>
        /// <remarks></remarks>
        public double y
        {
            get
            {
                if (!m_yv)
                    throw new HelperException("VelocityVector:y - y value has not been set");

                return m_v[1];
            }
            set
            {
                m_v[1] = value;
                m_yv = true;
            }
        }

        /// <summary>
        /// Cartesian z component of velocity (AU/day)
        /// </summary>
        /// <value>Cartesian z component of velocity</value>
        /// <returns>AU/day</returns>
        /// <remarks></remarks>
        public double z
        {
            get
            {
                if (!m_zv)
                    throw new HelperException("VelocityVector:z - z value has not been set");

                return m_v[2];
            }
            set
            {
                m_v[2] = value;
                m_zv = true;
            }
        }

        #region VelocityVector Support Code
        private void CheckEq()
        {
            if (m_cv)
                return; // Equatorial data already OK
            Novas.Vector2RaDec(m_v, ref m_VRA, ref m_VDec); // Calculate VRA/VDec
            m_RadVel = Sqrt(Pow(m_v[0], 2d) + Pow(m_v[1], 2d) + Pow(m_v[2], 2d));
            m_cv = true;
        }

    }
    #endregion

}