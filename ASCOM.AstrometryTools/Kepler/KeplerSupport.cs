using ASCOM.Tools.Novas31;
using static System.Math;

namespace ASCOM.Tools.Kepler
{
    /// <summary>
    /// Common items for the Kepler code
    /// </summary>
    static class KeplerSupport
    {
        #region Constants

        // Constant to indicate that a value has not been set.
        // This must not be changed to another value because tests are implemented using Double.IsNan() function.
        internal const double NOT_SET = double.NaN;

        internal const int NARGS = 18;

        // /* Conversion factors between degrees and radians */
        private const double DTR = 0.017453292519943295d;
        private const double STR = 0.00000484813681109536d; // /* radians per arc second */
        private const double PI = 3.1415926535897931d;
        private const double TPI = 2.0d * PI;

        // /* Standard epochs.  Note Julian epochs (J) are measured in
        // * years of 365.25 days.
        // */
        private const double J2000 = 2451545.0d; // /* 2000 January 1.5 */

        // /* Constants used elsewhere. These are DE403 values. */
        private const double emrat = 81.300585d; // /* Earth/Moon mass ratio.  */

        #endregion

        #region Utility Routines

        /// <summary>
        /// Obliquity of the ecliptic at Julian date J according to the DE403 values. Refer to S. Moshier's aa54e sources.
        /// </summary>
        /// <param name="J"></param>
        /// <param name="eps"></param>
        /// <param name="coseps"></param>
        /// <param name="sineps"></param>
        internal static void Epsiln(double J, ref double eps, ref double coseps, ref double sineps)
        {
            double T;

            T = (J - 2451545.0d) / 365250.0d; // T / 10
            eps = ((((((((((0.000000000245d * T + 0.00000000579d) * T + 0.0000002787d) * T + 0.000000712d) * T - 0.00003905d) * T - 0.0024967d) * T - 0.005138d) * T + 1.9989d) * T - 0.0175d) * T - 468.3396d) * T + 84381.406173d) * STR;


            coseps = Cos(eps);
            sineps = Sin(eps);
        }

        /// <summary>
        /// Precession of the equinox and ecliptic from epoch Julian date J to or from J2000.0
        /// </summary>
        /// <remarks>
        /// Program by Steve Moshier.  
        /// James G. Williams, "Contributions to the Earth's obliquity rate, precession, and nutation,"  Astron. J. 108, 711-724 (1994) 
        /// Corrections to Williams (1994) introduced in DE403. 
        /// </remarks>
        internal static double[] pAcof = new double[] { -0.000000000866d, -0.00000004759d, 0.0000002424d, 0.000013095d, 0.00017451d, -0.0018055d, -0.235316d, 0.076d, 110.5414d, 50287.91959d };

        internal static double[] nodecof = new double[] { 0.00000000000000066402d, -0.00000000000000269151d, -0.000000000001547021d, 0.000000000007521313d, 0.00000000019d, -0.00000000354d, -0.00000018103d, 0.000000126d, 0.00007436169d, -0.04207794833d, 3.052115282424d };

        internal static double[] inclcof = new double[] { 0.00000000000000012147d, 7.3759E-17d, -0.0000000000000826287d, 0.000000000000250341d, 0.000000000024650839d, -0.000000000054000441d, 0.00000000132115526d, -0.0000006012d, -0.0000162442d, 0.00227850649d, 0.0d };

        /// <summary>
        /// Precess
        /// </summary>
        /// <param name="R">rectangular equatorial coordinate vector to be precessed. The result is written back into the input vector.</param>
        /// <param name="J">Julian date</param>
        /// <param name="direction">Precession direction - 1: J to J2000, -1: J2000 to J</param>
        internal static void Precess(ref double[] R, double J, int direction)
        {
            double A, B, T, pA, W, z;
            var x = new double[4];
            double[] p;
            double eps = default, coseps = default, sineps = default;
            int i;

            if (J == J2000)
                return;

            // /* Each precession angle is specified by a polynomial in T = Julian centuries from J2000.0.  See AA page B18.
            T = (J - J2000) / 36525.0d;

            // /* Implementation by elementary rotations using Laskar's expansions. First rotate about the x axis from the initial equator to the ecliptic. (The input is equatorial.)
            if (direction == 1)
            {
                Epsiln(J, ref eps, ref coseps, ref sineps); // /* To J2000 */
            }
            else // From J2000
            {
                Epsiln(J2000, ref eps, ref coseps, ref sineps);
            }
            x[0] = R[0];
            z = coseps * R[1] + sineps * R[2];
            x[2] = -sineps * R[1] + coseps * R[2];
            x[1] = z;

            // Precession in longitude	 */
            T /= 10.0d; // thousands of years
            p = pAcof;
            pA = p[0];
            for (i = 1; i <= 9; i++)
                pA = pA * T + p[i];
            pA *= STR * T;

            // Node of the moving ecliptic on the J2000 ecliptic.
            p = nodecof;
            W = p[0];
            for (i = 1; i <= 10; i++)
                W = W * T + p[i];

            // Rotate about z axis to the node.
            if (direction == 1)
                z = W + pA;
            else
                z = W;

            B = Cos(z);
            A = Sin(z);
            z = B * x[0] + A * x[1];
            x[1] = -A * x[0] + B * x[1];
            x[0] = z;

            // Rotate about new x axis by the inclination of the moving ecliptic on the J2000 ecliptic.
            p = inclcof;
            z = p[0];
            for (i = 1; i <= 10; i++)
                z = z * T + p[i];
            if (direction == 1)
                z = -z;
            B = Cos(z);
            A = Sin(z);
            z = B * x[1] + A * x[2];
            x[2] = -A * x[1] + B * x[2];
            x[1] = z;

            // Rotate about new z axis back from the node.
            if (direction == 1)
                z = -W;
            else
                z = -W - pA;

            B = Cos(z);
            A = Sin(z);
            z = B * x[0] + A * x[1];
            x[1] = -A * x[0] + B * x[1];
            x[0] = z;

            // /* Rotate about x axis to final equator.	 */
            if (direction == 1)
                Epsiln(J2000, ref eps, ref coseps, ref sineps);
            else
                Epsiln(J, ref eps, ref coseps, ref sineps);

            z = coseps * x[1] - sineps * x[2];
            x[2] = sineps * x[1] + coseps * x[2];
            x[1] = z;

            for (i = 0; i <= 2; i++)
                R[i] = x[i];
        }

        internal static double Atan4(double x, double y)
        {
            double z, w = default;
            int code;

            code = 0;
            if (x < 0.0d)
                code = 2;
            if (y < 0.0d)
                code |= 1;

            if (x == 0.0d)
            {
                if ((code & 1) > 0)
                    return 1.5d * PI;
                if (y == 0.0d)
                    return 0.0d;
                return 0.5d * PI;
            }

            if (y == 0.0d)
            {
                if ((code & 2) > 0)
                    return PI;
                return 0.0d;
            }

            switch (code)
            {
                case 0:
                    w = 0.0d;
                    break;

                case 1:
                    w = 2.0d * PI;
                    break;

                case 2:
                    w = PI;
                    break;

                case 3:
                    w = PI;
                    break;

                default:
                    break;
            }

            z = Atan(y / x);

            return w + z;
        }

        /// <summary>
        /// Reduce x modulo 2 pi
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        internal static double ModTwoPi(double x)
        {

            double y;

            y = Floor(x / TPI);
            y = x - y * TPI;
            while (y < 0.0d)
                y += TPI;
            while (y >= TPI)
                y -= TPI;

            return y;
        }

        /// <summary>
        /// Reduce x modulo 360 degrees
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        internal static double Mod360(double x)
        {

            int k;
            double y;

            k = (int)Round(x / 360.0d);
            y = x - k * 360.0d;
            while (y < 0.0d)
                y += 360.0d;
            while (y > 360.0d)
                y -= 360.0d;
            return y;
        }

        /// <summary>
        /// Program to solve Keplerian orbit given orbital parameters and the time.
        /// </summary>
        /// <param name="J"></param>
        /// <param name="e"></param>
        /// <param name="rect">Returns Heliocentric equatorial rectangular coordinates of the object.</param>
        /// <remarks>
        /// This program detects several cases of given orbital elements. If a program for perturbations is pointed to, it is called to calculate all the elements.
        /// If there is no program, then the mean longitude is calculated from the mean anomaly and daily motion.If the daily motion is not given, it is calculated by Kepler's law.
        /// If the eccentricity is given to be 1.0, it means that mean distance is really the perihelion distance, as in a comet specification, and the orbit is parabolic.
        /// Reference: Taff, L.G., "Celestial Mechanics, A Computational Guide for the Practitioner."  Wiley, 1985.
        /// </remarks>
        internal static void KeplerCalc(double J, ref Orbit e, ref double[] rect)
        {

            var polar = new double[4];
            double alat, E1, M, W, temp;
            double epoch, inclination, ascendingNode, argumentOfPerihelion;
            double meanDistance, dailyMotion, eccent, meanAnomaly;
            double r, coso, sino, cosa;
            double eps = default, coseps = default, sineps = default;

            // Call program to compute position, if one is supplied.
            if (e.ptable.lon_tbl[0] != 0.0d)
            {
                if (e.objectName == "Earth")
                    G3Plan(J, ref e.ptable, ref polar, 3);
                else
                    GPlan(J, ref e.ptable, ref polar);

                E1 = polar[0]; // longitude
                e.L = E1;
                W = polar[1]; // latitude
                r = polar[2]; // radius 
                e.r = r;
                e.epoch = J;
                e.equinox = J2000;
            }
            else
            {
                // Compute from orbital elements 
                e.equinox = J2000; // Always J2000 coordinates
                epoch = e.epoch;
                inclination = e.i;
                ascendingNode = e.W * DTR;
                argumentOfPerihelion = e.wp;
                meanDistance = e.a; // semimajor axis
                dailyMotion = e.dm;
                eccent = e.ecc;
                meanAnomaly = e.M;

                if (eccent == 1.0d) // Parabolic
                {
                    // meanDistance = perihelion distance, q
                    // epoch = perihelion passage date
                    temp = meanDistance * Sqrt(meanDistance);
                    W = (J - epoch) * 0.0364911624d / temp;

                    // The constant above is 3 k / sqrt(2),
                    // k = Gaussian gravitational constant = 0.01720209895
                    E1 = 0.0d;
                    M = 1.0d;
                    while (Abs(M) > 0.00000000001d)
                    {
                        temp = E1 * E1;
                        temp = (2.0d * E1 * temp + W) / (3.0d * (1.0d + temp));
                        M = temp - E1;
                        if (temp != 0.0d)
                            M /= temp;
                        E1 = temp;
                    }
                    r = meanDistance * (1.0d + E1 * E1);
                    M = Atan(E1);
                    M = 2.0d * M;
                    alat = M + DTR * argumentOfPerihelion;
                }
                else if (eccent > 1.0d) // Hyperbolic
                {
                    // The equation of the hyperbola in polar coordinates r, theta is r = a(e^2 - 1)/(1 + e cos(theta)) so the perihelion  distance q = a(e-1), the "mean distance"  a = q/(e-1).
                    meanDistance /= (eccent - 1.0d);
                    temp = meanDistance * Sqrt(meanDistance);
                    W = (J - epoch) * 0.01720209895d / temp;

                    // solve M = -E + e sinh E
                    E1 = W / (eccent - 1.0d);
                    M = 1.0d;
                    while (Abs(M) > 0.00000000001d)
                    {

                        M = -E1 + eccent * Sinh(E1) - W;
                        E1 += M / (1.0d - eccent * Cosh(E1));
                    }
                    r = meanDistance * (-1.0d + eccent * Cosh(E1));
                    temp = (eccent + 1.0d) / (eccent - 1.0d);
                    M = Sqrt(temp) * Tanh(0.5d * E1);
                    M = 2.0d * Atan(M);
                    alat = M + DTR * argumentOfPerihelion;
                }
                else // Ellipsoidal eccent < 1.0)
                {
                    // Calculate the daily motion, if it is not given.
                    if (dailyMotion == 0.0d)
                    {
                        // The constant is 180 k / pi, k = Gaussian gravitational constant. Assumes object in heliocentric orbit is massless.
                        dailyMotion = 0.9856076686d / (e.a * Sqrt(e.a));
                    }
                    dailyMotion *= J - epoch;

                    // M is proportional to the area swept out by the radius vector of a circular orbit during the time between perihelion passage and Julian date J. It is the mean anomaly at time J.
                    M = DTR * (meanAnomaly + dailyMotion);
                    M = ModTwoPi(M);

                    // If mean longitude was calculated, adjust it also for motion since epoch of elements.
                    if (e.L != 0.0d)
                    {
                        e.L += dailyMotion;
                        e.L = Mod360(e.L);
                    }

                    // By Kepler's second law, M must be equal to the area swept out in the same time by an elliptical orbit of same total area.
                    // Integrate the ellipse expressed in polar coordinates r = a(1-e^2)/(1 + e cosW) with respect to the angle W to get an expression for the area swept out by the radius vector.
                    // The area is given by the mean anomaly; the angle is solved numerically.
                    // 
                    // The answer is obtained in two steps.  We first solve Kepler's equation M = E - eccent*sin(E) for the eccentric anomaly E.  Then there is a closed form solution for W in terms of E.

                    E1 = M; // /* Initial guess is same as circular orbit. */
                    do
                    {
                        // The approximate area swept out in the ellipse minus the area swept out in the circle should be zero.  Use the derivative of the error to converge to solution by Newton's method.
                        temp = E1 - eccent * Sin(E1) - M;
                        E1 -= temp / (1.0d - eccent * Cos(E1));
                    }
                    while (Abs(temp) > 0.00000000001d);

                    // The exact formula for the area in the ellipse is 2.0*atan(c2*tan(0.5*W)) - c1*eccent*sin(W)/(1+e*cos(W)) where
                    //    c1 = sqrt( 1.0 - eccent*eccent )
                    //    c2 = sqrt( (1.0-eccent)/(1.0+eccent) ).
                    // Substituting the following value of W yields the exact solution.
                    temp = Sqrt((1.0d + eccent) / (1.0d - eccent));
                    W = 2.0d * Atan(temp * Tan(0.5d * E1));

                    // The true anomaly.
                    W = ModTwoPi(W);

                    meanAnomaly *= DTR;

                    // Orbital longitude measured from node (argument of latitude)
                    if (e.L != 0.0d) // Mean longitude given
                    {
                        alat = e.L * DTR + W - meanAnomaly - ascendingNode;
                    }
                    else // Mean longitude not given

                    {
                        alat = W + DTR * argumentOfPerihelion;
                    }

                    // From the equation of the ellipse, get the radius from central focus to the object.
                    r = meanDistance * (1.0d - eccent * eccent) / (1.0d + eccent * Cos(W));
                }

                inclination *= DTR; // Convert inclination to radians

                // At this point:
                //		alat		= argument of latitude (rad)
                //		inclination	= inclination (rad)
                //		r			= radius from central focus

                // The heliocentric ecliptic longitude of the object is given by: tan(longitude - ascnode)  =  cos(inclination) * tan(alat)
                coso = Cos(alat);
                sino = Sin(alat);
                W = sino * Cos(inclination);
                E1 = Atan4(coso, W) + ascendingNode;

                // The ecliptic latitude of the object
                W = Asin(sino * Sin(inclination));
            }

            // At this point we have the heliocentric ecliptic polar coordinates of the body.

            // Convert to heliocentric ecliptic rectangular coordinates, using the perturbed latitude.
            rect[2] = r * Sin(W);
            cosa = Cos(W);
            rect[1] = r * cosa * Sin(E1);
            rect[0] = r * cosa * Cos(E1);

            // Convert from heliocentric ecliptic rectangular to heliocentric equatorial rectangular coordinates by rotating epsilon radians about the x axis.
            Epsiln(e.equinox, ref eps, ref coseps, ref sineps);
            W = coseps * rect[1] - sineps * rect[2];
            M = sineps * rect[1] + coseps * rect[2];
            rect[1] = W;
            rect[2] = M;

            // Precess the equatorial (rectangular) coordinates to the ecliptic & equinox of J2000.0, if not already there.
            //Precess(ref rect, e.equinox, 1);
            Novas.Precession(e.equinox, rect, J2000, ref rect);

            // If earth, adjust from earth-moon barycenter to earth by AA page E2. See embofs() below
            if (e.objectName == "Earth")
                Embofs(J, ref rect, ref r);
        }

        /// <summary>
        /// Adjust position from Earth-Moon barycenter to Earth
        /// </summary>
        /// <param name="J">Julian day number</param>
        /// <param name="ea">Equatorial rectangular coordinates of EMB.</param>
        /// <param name="pr"> Earth's distance to the Sun (au)</param>
        internal static void Embofs(double J, ref double[] ea, ref double pr)
        {
            double[] pm = new double[4], polm = new double[4];
            double a, b;
            int i;

            // Compute the vector Moon - Earth.
            GMoon(J, ref pm, ref polm);

            // Precess the lunar position to ecliptic and equinox of J2000.0
            Precess(ref pm, J, 1);

            // Adjust the coordinates of the Earth
            a = 1.0d / (emrat + 1.0d);
            b = 0.0d;
            for (i = 0; i <= 2; i++)
            {
                ea[i] = ea[i] - a * pm[i];
                b += ea[i] * ea[i];
            }

            // Sun-Earth distance.
            pr = Sqrt(b);
        }

        #endregion

        #region MajElems

        // Orbits for each planet.  The indicated orbital elements are not actually used, since the positions are  now calculated from a formula.
        // Magnitude and semi-diameter are still used.


        // /* January 5.0, 1987 */
        internal static Orbit mercury = new Orbit("Mercury", 2446800.5d, 7.0048d, 48.177d, 29.074d, 0.387098d, 4.09236d, 0.205628d, 198.7199d, 2446800.5d, -0.42d, 3.36d, Mer404Data.mer404, 0.0d, 0.0d, 0.0d);

        // /* Note the calculated apparent visual magnitude for Venus is not very accurate. */
        internal static Orbit venus = new Orbit("Venus", 2446800.5d, 3.3946d, 76.561d, 54.889d, 0.723329d, 1.60214d, 0.006757d, 9.0369d, 2446800.5d, -4.4d, 8.34d, Ven404Data.ven404, 0.0d, 0.0d, 0.0d);

        // /* Fixed numerical values will be used for earth if read in from a file named earth.orb.  See kfiles.c, kep.h. */
        internal static Orbit earthplanet = new Orbit("Earth", 2446800.5d, 0.0d, 0.0d, 102.884d, 0.999999d, 0.985611d, 0.016713d, 1.1791d, 2446800.5d, -3.86d, 0.0d, Ear404Data.ear404, 0.0d, 0.0d, 0.0d);

        internal static Orbit mars = new Orbit("Mars", 2446800.5d, 1.8498d, 49.457d, 286.343d, 1.52371d, 0.524023d, 0.093472d, 53.1893d, 2446800.5d, -1.52d, 4.68d, Mar404Data.mar404, 0.0d, 0.0d, 0.0d);

        internal static Orbit jupiter = new Orbit("Jupiter", 2446800.5d, 1.3051d, 100.358d, 275.129d, 5.20265d, 0.0830948d, 0.0481d, 344.5086d, 2446800.5d, -9.4d, 98.44d, Jup404Data.jup404, 0.0d, 0.0d, 0.0d);

        internal static Orbit saturn = new Orbit("Saturn", 2446800.5d, 2.4858d, 113.555d, 337.969d, 9.5405d, 0.033451d, 0.052786d, 159.6327d, 2446800.5d, -8.88d, 82.73d, Sat404Data.sat404, 0.0d, 0.0d, 0.0d);

        internal static Orbit uranus = new Orbit("Uranus", 2446800.5d, 0.7738d, 73.994d, 98.746d, 19.2233d, 0.0116943d, 0.045682d, 84.8516d, 2446800.5d, -7.19d, 35.02d, Ura404Data.ura404, 0.0d, 0.0d, 0.0d);

        internal static Orbit neptune = new Orbit("Neptune", 2446800.5d, 1.7697d, 131.677d, 250.623d, 30.1631d, 0.00594978d, 0.009019d, 254.2568d, 2446800.5d, -6.87d, 33.5d, Nep404Data.nep404, 0.0d, 0.0d, 0.0d);

        internal static Orbit pluto = new Orbit("Pluto", 2446640.5d, 17.1346d, 110.204d, 114.21d, 39.4633d, 0.0039757d, 0.248662d, 355.0554d, 2446640.5d, -1.0d, 2.07d, Plu404Data.plu404, 0.0d, 0.0d, 0.0d);

        #endregion

        #region GPlan

        private static readonly double[,] ss = new double[19, 32];
        private static readonly double[,] cc = new double[19, 32];
        private static readonly double[] Args = new double[19];
        private static double LP_equinox;

        // Routines to chew through tables of perturbations.
        internal static double Mods3600(double x)
        {
            return x - 1296000.0d * Floor(x / 1296000.0d);
        }

        // From Simon et al (1994)  Arc sec per 10000 Julian years.
        internal static double[] freqs = new double[] { 53810162868.8982d, 21066413643.3548d, 12959774228.3429d, 6890507749.3988d, 1092566037.7991d, 439960985.5372d, 154248119.3933d, 78655032.0744d, 52272245.1795d };

        // Arc sec.
        internal static double[] phases = new double[] { 252.25090552d * 3600.0d, 181.97980085d * 3600.0d, 100.46645683d * 3600.0d, 355.43299958d * 3600.0d, 34.35151874d * 3600.0d, 50.0774443d * 3600.0d, 314.05500511d * 3600.0d, 304.34866548d * 3600.0d, 860492.1546d };

        internal static int GPlan(double JD, ref PlanetTable plan, ref double[] pobj)
        {
            double su, cu, sv, cv, TI;
            double t, sl, sb, sr;
            int i, j, k, m, n, k1, ip, np, nt;
            int p, pl, pb, pr;


            TI = (JD - J2000) / plan.timescale;
            n = plan.maxargs;

            // Calculate sin( i*MM ), etc. for needed multiple angles.
            var loopTo = n - 1;
            for (i = 0; i <= loopTo; i++)
            {
                j = plan.max_harmonic[i];
                if (j > 0)
                {
                    sr = (Mods3600(freqs[i] * TI) + phases[i]) * STR;
                    Sscc(i, sr, j);
                }
            }

            // /* Point to start of table of arguments. */

            p = 0; // p = plan.arg_tbl

            // /* Point to tabulated cosine and sine amplitudes.  */
            pl = 0; // pl = plan.lon_tbl
            pb = 0; // pb = plan.lat_tbl
            pr = 0; // pr = plan.rad_tbl

            sl = 0.0d;
            sb = 0.0d;
            sr = 0.0d;

            do
            {
                // argument of sine and cosine Number of periodic arguments.
                np = plan.arg_tbl[p];
                p += 1;
                if (np < 0)
                    break;

                if (np == 0)  // It is a polynomial term.
                {
                    nt = plan.arg_tbl[p];
                    p += 1;
                    cu = plan.lon_tbl[pl];
                    pl += 1;  // Longitude polynomial.
                    var loopTo1 = nt - 1;
                    for (ip = 0; ip <= loopTo1; ip++)
                    {
                        cu = cu * TI + plan.lon_tbl[pl];
                        pl += 1;
                    }
                    sl += Mods3600(cu);

                    cu = plan.lat_tbl[pb];
                    pb += 1; // Latitude polynomial.
                    var loopTo2 = nt - 1;
                    for (ip = 0; ip <= loopTo2; ip++)
                    {
                        cu = cu * TI + plan.lat_tbl[pb];
                        pb += 1;
                    }
                    sb += cu;

                    cu = plan.rad_tbl[pr];
                    pr += 1; // Radius polynomial.
                    var loopTo3 = nt - 1;
                    for (ip = 0; ip <= loopTo3; ip++)
                    {
                        cu = cu * TI + plan.rad_tbl[pr];
                        pr += 1;
                    }
                    sr += cu;
                }
                else
                {
                    k1 = 0;
                    cv = 0.0d;
                    sv = 0.0d;
                    var loopTo4 = np - 1;
                    for (ip = 0; ip <= loopTo4; ip++)
                    {
                        j = plan.arg_tbl[p];
                        p += 1; //What harmonic.
                        m = plan.arg_tbl[p] - 1;
                        p += 1;  // Which planet.
                        if (j != 0)
                        {
                            k = j;
                            if (j < 0)
                                k = -k;
                            k -= 1;
                            su = ss[m, k]; //sin(k*angle)
                            if (j < 0)
                                su = -su;
                            cu = cc[m, k];

                            if (k1 == 0) // set first angle
                            {
                                sv = su;
                                cv = cu;
                                k1 = 1;
                            }
                            else // combine angles
                            {
                                t = su * cv + cu * sv;
                                cv = cu * cv - su * sv;
                                sv = t;
                            }
                        }
                    }

                    // Highest power of T.
                    nt = plan.arg_tbl[p];
                    p += 1;
                    cu = plan.lon_tbl[pl];
                    pl += 1; // Longitude.
                    su = plan.lon_tbl[pl];
                    pl += 1;
                    var loopTo5 = nt - 1;
                    for (ip = 0; ip <= loopTo5; ip++)
                    {
                        cu = cu * TI + plan.lon_tbl[pl];
                        pl += 1;
                        su = su * TI + plan.lon_tbl[pl];
                        pl += 1;
                    }
                    sl += cu * cv + su * sv;

                    cu = plan.lat_tbl[pb];
                    pb += 1; // Latitiude.
                    su = plan.lat_tbl[pb];
                    pb += 1;
                    var loopTo6 = nt;
                    for (ip = 1; ip <= loopTo6; ip++)
                    {
                        cu = cu * TI + plan.lat_tbl[pb];
                        pb += 1;
                        su = su * TI + plan.lat_tbl[pb];
                        pb += 1;
                    }
                    sb += cu * cv + su * sv;

                    cu = plan.rad_tbl[pr];
                    pr += 1; // Radius.
                    su = plan.rad_tbl[pr];
                    pr += 1;
                    var loopTo7 = nt;
                    for (ip = 1; ip <= loopTo7; ip++)
                    {
                        cu = cu * TI + plan.rad_tbl[pr];
                        pr += 1;
                        su = su * TI + plan.rad_tbl[pr];
                        pr += 1;
                    }
                    sr += cu * cv + su * sv;
                }
            }
            while (true);

            pobj[0] = STR * sl;
            pobj[1] = STR * sb;
            pobj[2] = STR * plan.distance * sr + plan.distance;

            return 0;
        }


        // Prepare lookup table of sin and cos ( i*Lj ) for required multiple angles
        internal static int Sscc(int k, double arg, int n)
        {
            double cu, su, cv, sv, s;
            int i;

            su = Sin(arg);
            cu = Cos(arg);
            ss[k, 0] = su; // /* sin(L) */
            cc[k, 0] = cu; // /* cos(L) */
            sv = 2.0d * su * cu;
            cv = cu * cu - su * su;
            ss[k, 1] = sv; // /* sin(2L) */
            cc[k, 1] = cv;
            var loopTo = n - 1;
            for (i = 2; i <= loopTo; i++)
            {
                s = su * cv + cu * sv;
                cv = cu * cv - su * sv;
                sv = s;
                ss[k, i] = sv; // /* sin( i+1 L ) */
                cc[k, i] = cv;
            }
            return 0;
        }
        // Compute mean elements at Julian date J.

        public static void MeanElements(double J)
        {
            double x, T, T2;

            // /* Time variables.  T is in Julian centuries.  */
            T = (J - 2451545.0d) / 36525.0d;
            T2 = T * T;

            // /* Mean longitudes of planets (Simon et al, 1994) .047" subtracted from constant term for offset to DE403 origin. */

            // /* Mercury */
            x = Mods3600(538101628.68898189d * T + 908103.213d);
            x += (0.00000639d * T - 0.0192789d) * T2;
            Args[0] = STR * x;

            // /* Venus */
            x = Mods3600(210664136.43354821d * T + 655127.236d);
            x += (-0.00000627d * T + 0.0059381d) * T2;
            Args[1] = STR * x;

            // /* Earth  */
            x = Mods3600(129597742.283429d * T + 361679.198d);
            x += (-0.00000523d * T - 0.0204411d) * T2;
            Args[2] = STR * x;

            // /* Mars */
            x = Mods3600(68905077.493988d * T + 1279558.751d);
            x += (-0.00001043d * T + 0.0094264d) * T2;
            Args[3] = STR * x;

            // /* Jupiter */
            x = Mods3600(10925660.377991d * T + 123665.42d);
            x += ((((-0.00000000034d * T + 0.0000000591d) * T + 0.000004667d) * T + 0.00005706d) * T - 0.3060378d) * T2;
            Args[4] = STR * x;

            // /* Saturn */
            x = Mods3600(4399609.855372d * T + 180278.752d);
            x += ((((0.00000000083d * T - 0.0000001452d) * T - 0.000011484d) * T - 0.00016618d) * T + 0.7561614d) * T2;
            Args[5] = STR * x;

            // /* Uranus */
            x = Mods3600(1542481.193933d * T + 1130597.971d) + (0.00002156d * T - 0.0175083d) * T2;
            Args[6] = STR * x;

            // /* Neptune */
            x = Mods3600(786550.320744d * T + 1095655.149d) + (-0.00000895d * T + 0.0021103d) * T2;
            Args[7] = STR * x;

            // /* Copied from cmoon.c, DE404 version.  */
            // /* Mean elongation of moon = D */
            x = Mods3600(1602961600.9939659d * T + 1072261.2202445078d);
            x += (((((-0.0000000000003207663637426d * T + 0.00000000002555243317839d) * T + 0.000000002560078201452d) * T - 0.00003702060118571d) * T + 0.0069492746836058421d) * T - 6.7352202374457519d) * T2;
            // /* D, t^2 */ 
            Args[9] = STR * x;

            // /* Mean distance of moon from its ascending node = F */
            x = Mods3600(1739527262.8437717d * T + 335779.5141288474d);
            x += (((((0.0000000000004474984866301d * T + 0.00000000004189032191814d) * T - 0.000000002790392351314d) * T - 0.000002165750777942d) * T - 0.00075311878482337989d) * T - 13.117809789650071d) * T2;
            // /* F, t^2 */
            Args[10] = STR * x;

            // /* Mean anomaly of sun = l' (J. Laskar) */
            x = Mods3600(129596581.0230432d * T + 1287102.7407441526d);
            x += ((((((((1.62E-20d * T - 1.039E-17d) * T - 0.00000000000000383508d) * T + 0.0000000000004237343d) * T + 0.000000000088555011d) * T - 0.0000000477258489d) * T - 0.000011297037031d) * T + 0.0000874737173673247d) * T - 0.55281306421783094d) * T2;

            Args[11] = STR * x;

            // /* Mean anomaly of moon = l */
            x = Mods3600(1717915922.8846793d * T + 485868.17465825332d);
            x += ((((-0.000000000001755312760154d * T + 0.00000000003452144225877d * T - 0.00000002506365935364d) * T - 0.0002536291235258d) * T + 0.052099641302735818d) * T + 31.501359071894147d) * T2;
            // /* l, t^2 */
            Args[12] = STR * x;

            // /* Mean longitude of moon, re mean ecliptic and equinox of date = L  */
            x = Mods3600(1732564372.0442266d * T + 785939.8092105242d);
            x += (((((0.00000000000007200592540556d * T + 0.0000000002235210987108d) * T - 0.00000001024222633731d) * T - 0.00006073960534117d) * T + 0.006901724852838049d) * T - 5.65504600274714d) * T2;
            // /* L, t^2 */
            LP_equinox = x;
            Args[13] = STR * x;

            // /* Precession of the equinox  */
            x = (((((((((-8.66E-20d * T - 4.759E-17d) * T + 0.000000000000002424d) * T + 0.0000000000013095d) * T + 0.00000000017451d) * T - 0.000000018055d) * T - 0.0000235316d) * T + 0.000076d) * T + 1.105414d) * T + 5028.791959d) * T;

            // /* Moon's longitude re fixed J2000 equinox.  */
            // /*
            // Args(13) -= x;
            // */

            // /* Free librations.  */
            // /* longitudinal libration 2.891725 years */
            x = Mods3600(44817540.9d * T + 806045.7d);
            Args[14] = STR * x;
            // /* libration P, 24.2 years */
            x = Mods3600(5364867.87d * T - 391702.8d);
            Args[15] = STR * x;

            // Args(16) = 0.0

            // /* libration W, 74.7 years. */
            x = Mods3600(1735730.0d * T);
            Args[17] = STR * x;
        }


        // /* Generic program to accumulate sum of trigonometric series
        // in three variables (e.g., longitude, latitude, radius)
        // of the same list of arguments.  */

        internal static int G3Plan(double JD, ref PlanetTable plan, ref double[] pobj, int objnum)
        {
            int i, j, k, m, n, k1, ip, np, nt;
            int p, pl, pb, pr;
            double su, cu, sv, cv;
            double TI, t, sl, sb, sr;

            MeanElements(JD);
            // #If 0 Then
            // /* For librations, moon's longitude is sidereal.  */
            // If (flag) Then
            // Args(13) -= pA_precession;
            // #End If

            TI = (JD - J2000) / plan.timescale;
            n = plan.maxargs;
            // /* Calculate sin( i*MM ), etc. for needed multiple angles.  */
            var loopTo = n - 1;
            for (i = 0; i <= loopTo; i++)
            {
                j = plan.max_harmonic[i];
                if (j > 0)
                    Sscc(i, Args[i], j);
            }

            // /* Point to start of table of arguments. */
            p = 0; // plan.arg_tbl
                   // /* Point to tabulated cosine and sine amplitudes.  */
            pl = 0; // plan.lon_tbl
            pb = 0; // plan.lat_tbl
            pr = 0; // plan.rad_tbl
            sl = 0.0d;
            sb = 0.0d;
            sr = 0.0d;

            do
            {
                // /* argument of sine and cosine */
                // /* Number of periodic arguments. */
                np = plan.arg_tbl[p];
                p += 1;
                if (np < 0)
                    break;
                if (np == 0)  // /* It is a polynomial term.  */
                {
                    nt = plan.arg_tbl[p];
                    p += 1;
                    cu = plan.lon_tbl[pl];
                    pl += 1; // /* "Longitude" polynomial (phi). */
                    var loopTo1 = nt - 1;
                    for (ip = 0; ip <= loopTo1; ip++)
                    {
                        cu = cu * TI + plan.lon_tbl[pl];
                        pl += 1;
                    }
                    // /*	  sl +=  mods3600 (cu); */
                    sl += cu;

                    cu = plan.lat_tbl[pb];
                    pb += 1; // /* "Latitude" polynomial (theta). */
                    var loopTo2 = nt - 1;
                    for (ip = 0; ip <= loopTo2; ip++)
                    {
                        cu = cu * TI + plan.lat_tbl[pb];
                        pb += 1;
                    }
                    sb += cu;

                    cu = plan.rad_tbl[pr];
                    pr += 1; // /* Radius polynomial (psi). */
                    var loopTo3 = nt - 1;
                    for (ip = 0; ip <= loopTo3; ip++)
                    {
                        cu = cu * TI + plan.rad_tbl[pr];
                        pr += 1;
                    }
                    sr += cu;
                }
                else
                {
                    k1 = 0;
                    cv = 0.0d;
                    sv = 0.0d;
                    var loopTo4 = np - 1;
                    for (ip = 0; ip <= loopTo4; ip++)
                    {
                        j = plan.arg_tbl[p];
                        p += 1;  // /* What harmonic.  */
                        m = plan.arg_tbl[p] - 1;
                        p += 1; // /* Which planet.  */
                        if (j != 0)
                        {
                            // /*	      k = abs (j); */
                            if (j < 0)
                            {
                                k = -j;
                            }
                            else
                            {
                                k = j;
                            }
                            k -= 1;
                            su = ss[m, k]; // /* sin(k*angle) */
                            if (j < 0)
                                su = -su;
                            cu = cc[m, k];
                            if (k1 == 0) // /* set first angle */
                            {
                                sv = su;
                                cv = cu;
                                k1 = 1;
                            }
                            else
                            {
                                // /* combine angles */	
                                t = su * cv + cu * sv;
                                cv = cu * cv - su * sv;
                                sv = t;
                            }
                        }
                    }
                    // /* Highest power of T.  */
                    nt = plan.arg_tbl[p];
                    p += 1;

                    // /* Longitude. */
                    cu = plan.lon_tbl[pl];
                    pl += 1;
                    su = plan.lon_tbl[pl];
                    pl += 1;
                    var loopTo5 = nt - 1;
                    for (ip = 0; ip <= loopTo5; ip++)
                    {
                        cu = cu * TI + plan.lon_tbl[pl];
                        pl += 1;
                        su = su * TI + plan.lon_tbl[pl];
                        pl += 1;
                    }
                    sl += cu * cv + su * sv;

                    // /* Latitude. */
                    cu = plan.lat_tbl[pb];
                    pb += 1;
                    su = plan.lat_tbl[pb];
                    pb += 1;
                    var loopTo6 = nt - 1;
                    for (ip = 0; ip <= loopTo6; ip++)
                    {
                        cu = cu * TI + plan.lat_tbl[pb];
                        pb += 1;
                        su = su * TI + plan.lat_tbl[pb];
                        pb += 1;
                    }
                    sb += cu * cv + su * sv;

                    // /* Radius. */
                    cu = plan.rad_tbl[pr];
                    pr += 1;
                    su = plan.rad_tbl[pr];
                    pr += 1;
                    var loopTo7 = nt - 1;
                    for (ip = 0; ip <= loopTo7; ip++)
                    {
                        cu = cu * TI + plan.rad_tbl[pr];
                        pr += 1;
                        su = su * TI + plan.rad_tbl[pr];
                        pr += 1;
                    }
                    sr += cu * cv + su * sv;
                }
            }
            while (true);
            t = plan.trunclvl;
            pobj[0] = Args[objnum - 1] + STR * t * sl;
            pobj[1] = STR * t * sb;
            pobj[2] = plan.distance * (1.0d + STR * t * sr);
            return 0;
        }

        // /* Generic program to accumulate sum of trigonometric series
        // in two variables (e.g., longitude, radius)
        // of the same list of arguments.  */
        internal static int G2Plan(double JD, ref PlanetTable plan, ref double[] pobj)
        {
            int i, j, k, m, n, k1, ip, np, nt;
            int p, pl, pr;
            double su, cu, sv, cv;
            double TI, t, sl, sr;

            MeanElements(JD);
            // #If 0 Then
            // /* For librations, moon's longitude is sidereal.  */
            // If (flag) Then
            // Args(13) -= pA_precession;
            // #End If
            TI = (JD - J2000) / plan.timescale;
            n = plan.maxargs;
            // /* Calculate sin( i*MM ), etc. for needed multiple angles.  */
            var loopTo = n - 1;
            for (i = 0; i <= loopTo; i++)
            {
                j = plan.max_harmonic[i];
                if (j > 0)
                    Sscc(i, Args[i], j);
            }

            // /* Point to start of table of arguments. */
            p = 0; // plan.arg_tbl
                   // /* Point to tabulated cosine and sine amplitudes.  */
            pl = 0; // (long *) plan.lon_tbl;
            pr = 0; // (long *) plan.rad_tbl;
            sl = 0.0d;
            sr = 0.0d;

            do
            {
                // /* argument of sine and cosine */
                // /* Number of periodic arguments. */
                np = plan.arg_tbl[p];
                p += 1; // *p++;
                if (np < 0)
                    break;

                if (np == 0)  // /* It is a polynomial term.  */
                {
                    nt = plan.arg_tbl[p];
                    p += 1;
                    cu = plan.lon_tbl[pl];
                    pl += 1; // *pl++; '/* Longitude polynomial. */
                    var loopTo1 = nt - 1;
                    for (ip = 0; ip <= loopTo1; ip++)
                    {
                        cu = cu * TI + plan.lon_tbl[pl];
                        pl += 1; // *pl++;
                    }
                    // /*	  sl +=  mods3600 (cu); */
                    sl += cu;
                    // /* Radius polynomial. */
                    cu = plan.rad_tbl[pr];
                    pr += 1; // *pr++;
                    var loopTo2 = nt - 1;
                    for (ip = 0; ip <= loopTo2; ip++)
                    {
                        cu = cu * TI + plan.rad_tbl[pr];
                        pr += 1;
                    }
                    sr += cu;
                }
                else
                {
                    k1 = 0;
                    cv = 0.0d;
                    sv = 0.0d;
                    var loopTo3 = np - 1;
                    for (ip = 0; ip <= loopTo3; ip++)
                    {
                        j = plan.arg_tbl[p];
                        p += 1; // /* What harmonic.  */
                        m = plan.arg_tbl[p] - 1;
                        p += 1;  // /* Which planet.  */
                        if (j != 0)
                        {
                            // /*	      k = abs (j); */
                            if (j < 0)
                            {
                                k = -j;
                            }
                            else
                            {
                                k = j;
                            }
                            k -= 1;
                            su = ss[m, k];  // /* sin(k*angle) */
                            if (j < 0)
                                su = -su;
                            cu = cc[m, k];
                            if (k1 == 0)
                            {
                                // /* set first angle */
                                sv = su;
                                cv = cu;
                                k1 = 1;
                            }
                            else
                            {
                                // /* combine angles */
                                t = su * cv + cu * sv;
                                cv = cu * cv - su * sv;
                                sv = t;
                            }
                        }
                    }
                    // /* Highest power of T.  */
                    nt = plan.arg_tbl[p];
                    p += 1; // *p++;
                            // /* Longitude. */
                    cu = plan.lon_tbl[pl];
                    pl += 1;
                    su = plan.lon_tbl[pl];
                    pl += 1;
                    var loopTo4 = nt - 1;
                    for (ip = 0; ip <= loopTo4; ip++)
                    {
                        cu = cu * TI + plan.lon_tbl[pl];
                        pl += 1;
                        su = su * TI + plan.lon_tbl[pl];
                        pl += 1;
                    }
                    sl += cu * cv + su * sv;
                    // /* Radius. */
                    cu = plan.rad_tbl[pr];
                    pr += 1;
                    su = plan.rad_tbl[pr];
                    pr += 1;
                    var loopTo5 = nt - 1;
                    for (ip = 0; ip <= loopTo5; ip++)
                    {
                        cu = cu * TI + plan.rad_tbl[pr];
                        pr += 1;
                        su = su * TI + plan.rad_tbl[pr];
                        pr += 1;
                    }
                    sr += cu * cv + su * sv;
                }
            }
            while (true);
            t = plan.trunclvl;
            pobj[0] = t * sl;
            pobj[2] = t * sr;
            return 0;
        }


        // /* Generic program to accumulate sum of trigonometric series
        // in one variable.  */

        internal static double G1Plan(double JD, ref PlanetTable plan)
        {
            int i, j, k, m, k1, ip, np, nt;
            int p, pl;
            double su, cu, sv, cv;
            double TI, t, sl;

            TI = (JD - J2000) / plan.timescale;
            MeanElements(JD);
            // /* Calculate sin( i*MM ), etc. for needed multiple angles.  */
            for (i = 0; i <= NARGS - 1; i++)
            {
                j = plan.max_harmonic[i];
                if (j > 0)
                    Sscc(i, Args[i], j);
            }

            // /* Point to start of table of arguments. */
            p = 0; // plan.arg_tbl;
                   // /* Point to tabulated cosine and sine amplitudes.  */
            pl = 0; // (long *) plan.lon_tbl;
            sl = 0.0d;

            do    // /* argument of sine and cosine */
            {
                // /* Number of periodic arguments. */
                np = plan.arg_tbl[p];
                p += 1; // *p++;
                if (np < 0)
                    break;
                if (np == 0)
                {
                    // /* It is a polynomial term.  */
                    nt = plan.arg_tbl[p];
                    p += 1; // *p++;
                    cu = plan.lon_tbl[pl];
                    pl += 1; // *pl++;
                    var loopTo = nt - 1;
                    for (ip = 0; ip <= loopTo; ip++)
                    {
                        cu = cu * TI + plan.lon_tbl[pl];
                        pl += 1; // *pl++;
                    }
                    // /*	  sl +=  mods3600 (cu); */
                    sl += cu;
                }
                else
                {
                    k1 = 0;
                    cv = 0.0d;
                    sv = 0.0d;
                    var loopTo1 = np - 1;
                    for (ip = 0; ip <= loopTo1; ip++)
                    {
                        j = plan.arg_tbl[p];
                        p += 1;   // /* What harmonic.  */
                        m = plan.arg_tbl[p] - 1;
                        p += 1; // /* Which planet.  */
                        if (j != 0)
                        {
                            // /*	      k = abs (j); */
                            if (j < 0)
                            {
                                k = -j;
                            }
                            else
                            {
                                k = j;
                            }
                            k -= 1;
                            su = ss[m, k];  // /* sin(k*angle) */
                            if (j < 0)
                                su = -su;
                            cu = cc[m, k];
                            if (k1 == 0)
                            {
                                // /* set first angle */
                                sv = su;
                                cv = cu;
                                k1 = 1;
                            }
                            else
                            {
                                // /* combine angles */
                                t = su * cv + cu * sv;
                                cv = cu * cv - su * sv;
                                sv = t;
                            }
                        }
                    }
                    // /* Highest power of T.  */
                    nt = plan.arg_tbl[p];
                    p += 1;
                    // /* Cosine and sine coefficients.  */
                    cu = plan.lon_tbl[pl];
                    pl += 1;
                    su = plan.lon_tbl[pl];
                    pl += 1;
                    var loopTo2 = nt - 1;
                    for (ip = 0; ip <= loopTo2; ip++)
                    {
                        cu = cu * TI + plan.lon_tbl[pl];
                        pl += 1;
                        su = su * TI + plan.lon_tbl[pl];
                        pl += 1;
                    }
                    sl += cu * cv + su * sv;
                }
            }
            while (true);
            return plan.trunclvl * sl;
        }

        internal static int GMoon(double J, ref double[] rect, ref double[] pol)
        {
            double x, cosB, sinB, cosL, sinL, eps = default, coseps = default, sineps = default;
            G2Plan(J, ref Mlr404Data.moonlr, ref pol);
            x = pol[0];
            x += LP_equinox;
            if (x < -645000.0d)
                x += 1296000.0d;
            if (x > 645000.0d)
                x -= 1296000.0d;
            pol[0] = STR * x;
            x = G1Plan(J, ref Mlat404Data.moonlat);
            pol[1] = STR * x;
            x = (1.0d + STR * pol[2]) * Mlr404Data.moonlr.distance;
            pol[2] = x;
            // /* Convert ecliptic polar to equatorial rectangular coordinates.  */
            Epsiln(J, ref eps, ref coseps, ref sineps);
            cosB = Cos(pol[1]);
            sinB = Sin(pol[1]);
            cosL = Cos(pol[0]);
            sinL = Sin(pol[0]);
            rect[0] = cosB * cosL * x;
            rect[1] = (coseps * cosB * sinL - sineps * sinB) * x;
            rect[2] = (sineps * cosB * sinL + coseps * sinB) * x;

            return 0;
        }

        #endregion

    }
}