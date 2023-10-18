using ASCOM;
using static System.Math;
using ASCOM.Tools.Novas31;
using ASCOM.Tools;

namespace NovasCom
{
    /// <summary>
    /// NOVAS-COM: PositionVector Class
    /// </summary>
    /// <remarks>NOVAS-COM objects of class PositionVector contain vectors used for positions (earth, sites, 
    /// stars and planets) throughout NOVAS-COM. Of course, its properties include the x, y, and z 
    /// components of the position. Additional properties are right ascension and declination, distance, 
    /// and light time (applicable to star positions), and Alt/Az (available only in PositionVectors 
    /// returned by Star or Planet methods GetTopocentricPosition()). You can initialize a PositionVector 
    /// from a Star object (essentially an FK5 or HIP catalog entry) or a Site (lat/long/height). 
    /// PositionVector has methods that can adjust the coordinates for precession, Novas.Aberration and 
    /// proper motion. Thus, a PositionVector object gives access to some of the lower-level NOVAS functions. 
    /// <para><b>Note:</b> The equatorial coordinate properties of this object are dependent variables, and thus are read-only. Changing any Cartesian coordinate will cause the equatorial coordinates to be recalculated. 
    /// </para></remarks>
    public class PositionVector : IPositionVector, IPositionVectorExtra
    {
        internal const double C = 173.14463348d; // Speed of light in AU/Day.

        private bool xOk, yOk, zOk, RADecOk, AzElOk;
        private double[] PosVec = new double[3];
        private double m_RA;
        private double m_DEC;
        private double m_Dist;
        private double m_Light;
        private readonly double m_Alt;
        private readonly double m_Az;

        /// <summary>
        /// Create a new, uninitialised position vector
        /// </summary>
        /// <remarks></remarks>
        public PositionVector()
        {
            xOk = false;
            yOk = false;
            zOk = false;
            RADecOk = false;
            AzElOk = false;
        }

        /// <summary>
        /// Create a new position vector with supplied initial values
        /// </summary>
        /// <param name="x">Position vector x co-ordinate</param>
        /// <param name="y">Position vector y co-ordinate</param>
        /// <param name="z">Position vector z co-ordinate</param>
        /// <param name="RA">Right ascension (hours)</param>
        /// <param name="DEC">Declination (degrees)</param>
        /// <param name="Distance">Distance to object</param>
        /// <param name="Light">Light-time to object</param>
        /// <param name="Azimuth">Object azimuth</param>
        /// <param name="Altitude">Object altitude</param>
        /// <remarks></remarks>
        public PositionVector(double x, double y, double z, double RA, double DEC, double Distance, double Light, double Azimuth, double Altitude)
        {
            PosVec[0] = x;
            xOk = true;
            PosVec[1] = y;
            yOk = true;
            PosVec[2] = z;
            zOk = true;
            m_RA = RA;
            m_DEC = DEC;
            RADecOk = true;
            m_Dist = Distance;
            m_Light = Light;
            m_Az = Azimuth;
            m_Alt = Altitude;
            AzElOk = true;
        }

        /// <summary>
        /// Create a new position vector with supplied initial values
        /// </summary>
        /// <param name="x">Position vector x co-ordinate</param>
        /// <param name="y">Position vector y co-ordinate</param>
        /// <param name="z">Position vector z co-ordinate</param>
        /// <param name="RA">Right ascension (hours)</param>
        /// <param name="DEC">Declination (degrees)</param>
        /// <param name="Distance">Distance to object</param>
        /// <param name="Light">Light-time to object</param>
        /// <remarks></remarks>
        public PositionVector(double x, double y, double z, double RA, double DEC, double Distance, double Light)
        {
            PosVec[0] = x;
            xOk = true;
            PosVec[1] = y;
            yOk = true;
            PosVec[2] = z;
            zOk = true;
            m_RA = RA;
            m_DEC = DEC;
            RADecOk = true;
            m_Dist = Distance;
            m_Light = Light;
            AzElOk = false;
        }

        /// <summary>
        /// Adjust the position vector of an object for Novas.Aberration of light
        /// </summary>
        /// <param name="vel">The velocity vector of the observer</param>
        /// <remarks>The algorithm includes relativistic terms</remarks>
        public void Aberration(VelocityVector vel)
        {
            double[] p = new double[3], v = new double[3];
            if (!(xOk & yOk & zOk))
                throw new HelperException("PositionVector:ProperMotion - x, y or z has not been set");

            CheckEq();
            p[0] = PosVec[0];
            p[1] = PosVec[1];
            p[2] = PosVec[2];

            try
            {
                v[0] = vel.x;
            }
            catch
            {
                throw new HelperException("PositionVector:Aberration - VelocityVector.x is not available");
            }
            try
            {
                v[1] = vel.y;
            }
            catch 
            {
                throw new HelperException("PositionVector:Aberration - VelocityVector.y is not available");

            }
            try
            {
                v[2] = vel.z;
            }
            catch
            {
                throw new HelperException("PositionVector:Aberration - VelocityVector.z is not available");
            }

            Novas.Aberration(p, v, m_Light, ref PosVec);
            RADecOk = false;
            AzElOk = false;
        }

        /// <summary>
        /// The azimuth coordinate (degrees, + east)
        /// </summary>
        /// <value>The azimuth coordinate</value>
        /// <returns>Degrees, + East</returns>
        /// <remarks></remarks>
        public double Azimuth
        {
            get
            {
                if (!AzElOk)
                    throw new HelperException("PositionVector:Azimuth - Azimuth is not available");

                return m_Az;
            }
        }

        /// <summary>
        /// Declination coordinate
        /// </summary>
        /// <value>Declination coordinate</value>
        /// <returns>Degrees</returns>
        /// <remarks></remarks>
        public double Declination
        {
            get
            {
                if (!(xOk & yOk & zOk))
                    throw new HelperException("PositionVector:Declination - x, y or z has not been set");

                CheckEq();
                return m_DEC;
            }
        }

        /// <summary>
        /// Distance/Radius coordinate
        /// </summary>
        /// <value>Distance/Radius coordinate</value>
        /// <returns>AU</returns>
        /// <remarks></remarks>
        public double Distance
        {
            get
            {
                if (!(xOk & yOk & zOk))
                    throw new HelperException("PositionVector:Distance - x, y or z has not been set");

                CheckEq();
                return m_Dist;
            }
        }

        /// <summary>
        /// The elevation (altitude) coordinate (degrees, + up)
        /// </summary>
        /// <value>The elevation (altitude) coordinate (degrees, + up)</value>
        /// <returns>(Degrees, + up</returns>
        /// <remarks>Elevation is available only in PositionVectors returned from calls to 
        /// Star.GetTopocentricPosition() and/or Planet.GetTopocentricPosition(). </remarks>
        /// <exception cref="HelperException">When the position vector has not been 
        /// initialised from Star.GetTopoCentricPosition and Planet.GetTopocentricPosition</exception>
        public double Elevation
        {
            get
            {
                if (!AzElOk)
                    throw new HelperException("PositionVector:Elevation - Elevation is not available");

                return m_Alt;
            }
        }

        /// <summary>
        /// Light time from Body to origin, days.
        /// </summary>
        /// <value>Light time from Body to origin</value>
        /// <returns>Days</returns>
        /// <remarks></remarks>
        public double LightTime
        {
            get
            {
                if (!(xOk & yOk & zOk))
                    throw new HelperException("PositionVector:LightTime - x, y or z has not been set");

                CheckEq();
                return m_Light;
            }
        }

        /// <summary>
        /// Adjust the position vector for precession of equinoxes between two given epochs
        /// </summary>
        /// <param name="tjd">The first epoch (Terrestrial Julian Date)</param>
        /// <param name="tjd2">The second epoch (Terrestrial Julian Date)</param>
        /// <remarks>The coordinates are referred to the mean equator and equinox of the two respective epochs.</remarks>
        public void Precess(double tjd, double tjd2)
        {
            var p = new double[3];
            if (!(xOk & yOk & zOk))
                throw new HelperException("PositionVector:Precess - x, y or z has not been set");

            p[0] = PosVec[0];
            p[1] = PosVec[1];
            p[2] = PosVec[2];
            Novas.Precession(tjd, p, tjd2, ref PosVec);

            RADecOk = false;
            AzElOk = false;
        }

        /// <summary>
        /// Adjust the position vector for proper motion (including foreshortening effects)
        /// </summary>
        /// <param name="vel">The velocity vector of the object</param>
        /// <param name="tjd1">The first epoch (Terrestrial Julian Date)</param>
        /// <param name="tjd2">The second epoch (Terrestrial Julian Date)</param>
        /// <returns>True if successful or throws an exception.</returns>
        /// <remarks></remarks>
        /// <exception cref="ValueNotSetException">If the position vector x, y or z values has not been set</exception>
        /// <exception cref="HelperException">If the supplied velocity vector does not have valid x, y and z components</exception>
        public bool ProperMotion(VelocityVector vel, double tjd1, double tjd2)
        {
            double[] p = new double[3], v = new double[3];
            if (!(xOk & yOk & zOk))
                throw new HelperException("PositionVector:ProperMotion - x, y or z has not been set");

            p[0] = PosVec[0];
            p[1] = PosVec[1];
            p[2] = PosVec[2];
            try
            {
                v[0] = vel.x;
            }
            catch 
            {
                throw new HelperException("PositionVector:ProperMotion - VelocityVector.x is not available");
            }

            try
            {
                v[1] = vel.y;
            }
            catch 
            {
                throw new HelperException("PositionVector:ProperMotion - VelocityVector.y is not available");
            }

            try
            {
                v[2] = vel.z;
            }
            catch 
            {
                throw new HelperException("PositionVector:ProperMotion - VelocityVector.z is not available");
            }

            Novas.ProperMotion(tjd1, p, v, tjd2, ref PosVec);

            RADecOk = false;
            AzElOk = false;

            return default;
        }

        /// <summary>
        /// RightAscension coordinate, hours
        /// </summary>
        /// <value>RightAscension coordinate</value>
        /// <returns>Hours</returns>
        /// <remarks></remarks>
        public double RightAscension
        {
            get
            {
                if (!(xOk & yOk & zOk))
                    throw new HelperException("PositionVector:RA - x, y or z has not been set");

                CheckEq();
                return m_RA;
            }
        }

        /// <summary>
        /// Initialize the PositionVector from a Site object and Greenwich apparent sidereal time.
        /// </summary>
        /// <param name="site">The Site object from which to initialize</param>
        /// <param name="gast">Greenwich Apparent Sidereal Time</param>
        /// <returns>True if successful or throws an exception</returns>
        /// <remarks>The GAST parameter must be for Greenwich, not local. The time is rotated through the 
        /// site longitude. See SetFromSiteJD() for an equivalent method that takes UTC Julian Date and 
        /// Delta-T (eliminating the need for calculating hyper-accurate GAST yourself).</remarks>
        public bool SetFromSite(Site site, double gast)
        {
            const double f = 0.00335281d; // f = Earth ellipsoid flattening
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
                throw new HelperException("PositionVector:SetFromSite - Site.Latitude is not available");
            }

            t = DEG2RAD * t;
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
                throw new HelperException("PositionVector:SetFromSite - Site.Height is not available");
            }
            t /= 1000d; // Elevation in KM
            ach = EARTHRAD * c + t;
            ash = EARTHRAD * s + t;

            // Compute local sidereal time factors at the observer's longitude.
            try
            {
                t = site.Longitude;
            }
            catch 
            {
                throw new HelperException("PositionVector:SetFromSite - Site.Height is not available");
            }

            stlocl = (gast * 15.0d + t) * DEG2RAD;
            sinst = Sin(stlocl);
            cosst = Cos(stlocl);

            // Compute position vector components in AU
            PosVec[0] = ach * cosphi * cosst / KMAU;
            PosVec[1] = ach * cosphi * sinst / KMAU;
            PosVec[2] = ash * sinphi / KMAU;

            RADecOk = false; // These really aren't interesting anyway for site vector
            AzElOk = false;

            xOk = true; // Object is valid
            yOk = true;
            zOk = true;

            return default;
        }

        /// <summary>
        /// Initialize the PositionVector from a Site object using UTC Julian date
        /// </summary>
        /// <param name="site">The Site object from which to initialize</param>
        /// <param name="ujd">UTC Julian Date</param>
        /// <returns>True if successful or throws an exception</returns>
        /// <remarks>The Julian date must be UTC Julian date, not terrestrial. Calculations will use the internal delta-T tables and estimator to get 
        /// delta-T. 
        /// </remarks>
        public bool SetFromSiteJD(Site site, double ujd)
        {
            SetFromSiteJD(site, ujd, 0.0d);
            return default;
        }

        /// <summary>
        /// Initialize the PositionVector from a Site object using UTC Julian date and Delta-T
        /// </summary>
        /// <param name="site">The Site object from which to initialize</param>
        /// <param name="ujd">UTC Julian Date</param>
        /// <param name="delta_t">The value of Delta-T (TT - UT1) to use for reductions (seconds)</param>
        /// <returns>True if successful or throws an exception</returns>
        /// <remarks>The Julian date must be UTC Julian date, not terrestrial.</remarks>
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
            //
            // tjd = ujd + ((delta_t != 0.0) ? delta_t : deltat(ujd))
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

            return default;

        }

        /// <summary>
        /// Initialize the PositionVector from a Star object.
        /// </summary>
        /// <param name="star">The Star object from which to initialize</param>
        /// <returns>True if successful or throws an exception</returns>
        /// <remarks></remarks>
        /// <exception cref="HelperException">If Parallax, RightAScension or Declination is not available in the supplied star object.</exception>
        public bool SetFromStar(Star star)
        {
            const double DEG2RAD = 0.017453292519943295d;
            const double RAD2SEC = 206264.80624709636d; // Angle conversion constants.

            double paralx, r, d, cra, sra, cdc, sdc;

            // If parallax is unknown, undetermined, or zero, set it to 1e-7 second of arc, corresponding to a distance of 10 mega parsecs.
            try
            {
                paralx = star.Parallax;
            }
            catch 
            {
                throw new HelperException("PositionVector:SetFromStar - Star.Parallax is not available");
            }

            if (paralx <= 0.0d)
                paralx = 0.0000001d;

            // Convert right ascension, declination, and parallax to position vector in equatorial system with units of AU.
            m_Dist = RAD2SEC / paralx;
            try
            {
                m_RA = star.RightAscension;
            }
            catch 
            {
                throw new HelperException("PositionVector:SetFromStar - Star.RightAscension is not available");
            }

            r = m_RA * 15.0d * DEG2RAD; // hrs -> deg -> rad

            try
            {
                m_DEC = star.Declination;
            }
            catch 
            {
                throw new HelperException("PositionVector:SetFromStar - Star.Declination is not available");
            }

            d = m_DEC * DEG2RAD; // deg -> rad

            cra = Cos(r);
            sra = Sin(r);
            cdc = Cos(d);
            sdc = Sin(d);

            PosVec[0] = m_Dist * cdc * cra;
            PosVec[1] = m_Dist * cdc * sra;
            PosVec[2] = m_Dist * sdc;

            RADecOk = true; // Object is valid
            xOk = true;
            yOk = true;
            zOk = true;

            return default;
        }

        /// <summary>
        /// Position Cartesian x component
        /// </summary>
        /// <value>Cartesian x component</value>
        /// <returns>Cartesian x component</returns>
        /// <remarks></remarks>
        public double x
        {
            get
            {
                if (!xOk)
                    throw new HelperException("PositionVector:x - x has not been set");

                return PosVec[0];
            }
            set
            {
                PosVec[0] = value;
                xOk = true;
                RADecOk = false;
                AzElOk = false;
            }
        }

        /// <summary>
        /// Position Cartesian y component
        /// </summary>
        /// <value>Cartesian y component</value>
        /// <returns>Cartesian y component</returns>
        /// <remarks></remarks>
        public double y
        {
            get
            {
                if (!yOk)
                    throw new HelperException("PositionVector:y - y has not been set");

                return PosVec[1];
            }
            set
            {
                PosVec[1] = value;
                yOk = true;
                RADecOk = false;
                AzElOk = false;
            }
        }

        /// <summary>
        /// Position Cartesian z component
        /// </summary>
        /// <value>Cartesian z component</value>
        /// <returns>Cartesian z component</returns>
        /// <remarks></remarks>
        public double z
        {
            get
            {
                if (!zOk)
                    throw new HelperException("PositionVector:z - z has not been set");

                return PosVec[2];
            }
            set
            {
                PosVec[2] = value;
                zOk = true;
                RADecOk = false;
                AzElOk = false;
            }
        }

        #region PositionVector Support Code

        private void CheckEq()
        {
            if (RADecOk)
                return; // Equatorial data already OK

            Novas.Vector2RaDec(PosVec, ref m_RA, ref m_DEC); // Calculate RA/Dec
            m_Dist = Sqrt(Pow(PosVec[0], 2d) + Pow(PosVec[1], 2d) + Pow(PosVec[2], 2d));
            m_Light = m_Dist / C;
            RADecOk = true;
        }

        #endregion
    }
}