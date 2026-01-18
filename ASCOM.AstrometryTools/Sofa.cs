using System;
using System.Runtime.InteropServices;

namespace ASCOM.Tools
{
    /// <summary>
    /// Presentation facade for the IAU SOFA library
    /// </summary>
    /// <remarks>This component assumes that a native library called libsofa exists in the same folder as the ASCOM.Tools DLL. This library must be compiled from the IAU SOFA C code base 
    /// for each supported OS platform: linux64, arm32, win64 etc.</remarks>
    public class Sofa
    {
        // Name of the SOFA run-time library on all OS platforms
        const string SOFA_LIBRARY = "libsofa";

        // Release and revision constants
        private const int SOFA_RELEASE_NUMBER = 19;
        private const string SOFA_ISSUE_DATE = "2023-10-11";
        private const int SOFA_REVISION_NUMBER = 0;
        private const string SOFA_REVISION_DATE = "2023-10-11";

        #region Managed structs

        /// <summary>
        /// Return a fully initialised Astrom struct. Only required for frameworks earlier than .NET 8.
        /// </summary>
        /// <returns>
        /// An Astrom struct with the eb[3], eh[3], v[3] and bpn[9] arrays present and initialised to zero values.
        /// </returns>
        /// <remarks>
        /// The SOFA bpn array is a 3x3 matrix stored in row-major order. This is depicted as a C array: bpn[3][3]. The managed array is a single-dimensional array 
        /// of length 9 with elements in the order: [0] = bpn[0,0], [1] = bpn[0,1], [2] = bpn[0,2], [3] = bpn[1,0], [4] = bpn[1,1], [5] = bpn[1,2], 
        /// [6] = bpn[2,0], [7] = bpn[2,1] and [8] = bpn[2,2].
        ///</remarks>
        public static Astrom CreateAstrom()
        {
            return new Astrom
            {
                eb = new double[3],
                eh = new double[3],
                v = new double[3],
                bpn = new double[9]
            };
        }

        /// <summary>
        /// Returns a fully initialised LdBody struct. Only required for frameworks earlier than .NET 8.
        /// </summary>
        /// <returns>
        /// An LdBody struct with the pv[6] array present and initialised to zero values.
        /// </returns>
        /// <remarks>
        /// The SOFA pv array is a 2x3 matrix stored in row-major order. This is depicted as a C array: ld[2][3]. The managed array is a single-dimensional array 
        /// of length 6 with elements in the order: [0] = ld[0,0], [1] = ld[0,1], [2] = ld[0,2], [3] = ld[1,0], [4] = ld[1,1], [5] = ld[1,2].
        ///</remarks>
        public static LdBody CreateLdBody()
        {
            return new LdBody
            {
                pv = new double[6]
            };
        }

        /// <summary>
        /// Managed representation of the SOFA <c>iauASTROM</c> structure.
        /// </summary>
        /// <remarks>
        /// The SOFA C library requires that memory is pre-allocated for the array fields in this struct (eb,eh,v and bpn). For .NET 8 and later applications, the parameterless constructor 
        /// will allocate the arrays automatically. For earlier frameworks, use the static SOFA.CreateAstrom() method to get an initialised struct.
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        public struct Astrom
        {

#if NET8_0_OR_GREATER
            /// <summary>
            /// Parameterless constructor for .NET 8 clients onward. All other frameworks must use the <see cref="CreateAstrom"/> method to create a fully initialised struct.
            /// </summary>
            public Astrom()
            {
                eb = new double[3];
                eh = new double[3];
                v = new double[3];
                bpn = new double[9];
            }
#endif
            /// <summary>
            /// PM time interval (SSB, Julian years) 
            /// </summary>
            public double pmt;

            /// <summary>
            /// Gets or sets the Earth barycentric position vector components.
            /// </summary>
            /// <remarks>The array contains three elements representing the X, Y, and Z coordinates of
            /// the Earth's barycentric position, typically in astronomical units. The array must have exactly three
            /// elements.</remarks>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public double[] eb;

            /// <summary>
            /// Gets or sets the Earth heliocentric position vector components.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public double[] eh;

            /// <summary>
            /// Distance from the sun to the observer (AU)
            /// </summary>
            public double em;

            /// <summary>
            /// Velocity vector of the observer with respect to the solar system barycentre.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public double[] v;

            /// <summary>
            /// Sqrt(1-|v|^2): reciprocal of the Lorenz factor
            /// </summary>
            public double bm1;

            /// <summary>
            /// Bias-precession-nutation matrix
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
            public double[] bpn;

            /// <summary>
            /// Longitude + s' + dERA(DUT) (radians) 
            /// </summary>
            public double along;

            /// <summary>
            /// Geodetic latitude (radians)
            /// </summary>  
            public double phi;

            /// <summary>
            /// Polar motion xp wrt local meridian (radians)
            /// </summary>
            public double xpl;

            /// <summary>
            /// Polar motion yp wrt local meridian (radians)
            /// </summary>
            public double ypl;


            /// <summary>
            /// Sine of geodetic latitude
            /// </summary>
            public double sphi;

            /// <summary>
            /// Cosine of geodetic latitude
            /// </summary>
            public double cphi;

            /// <summary>
            /// Magnitude of diurnal aberration vector
            /// </summary>
            public double diurab;

            /// <summary>
            /// Local" Earth rotation angle (radians)
            /// </summary>
            public double eral;

            /// <summary>
            /// Refraction constant A (radians)
            /// </summary>
            public double refa;

            /// <summary>
            /// Refraction constant B (radians)
            /// </summary>
            public double refb;
        }

        /// <summary>
        /// Managed representation of the SOFA <c>iauLDBODY</c> structure.
        /// </summary>
        /// <remarks>
        /// Mirrors the C layout from sofa.h. The pv field (2x3) is marshalled as a 6-element row-major array.
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        public struct LdBody
        {
            /// <summary>
            /// Mass of the body (solar masses)
            /// </summary>
            public double bm;

            /// <summary>
            /// Deflection limiter (radians^2/2)
            /// </summary>
            public double dl;

#if NET8_0_OR_GREATER
            /// <summary>
            /// Parameterless constructor for .NET 8 clients onward. All other frameworks must use the <see cref="CreateLdBody"/> method to create a fully initialised struct.
            /// </summary>
            public LdBody()
            {
                pv = new double[6];
            }
#endif

            /// <summary>
            /// Barycentric position velocity vector [2,3] of the body (au, au/day)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public double[] pv;
        }

        #endregion

        #region Enums

        enum ReferenceElipsoids
        {
            WGS84 = 1,
            GRS80 = 2,
            WGS72 = 3
        }

        #endregion

        #region ASCOM Sofa component metadata members

        /// <summary>
        /// Major number of the SOFA issue currently used by this component.
        /// </summary>
        /// <returns>Integer issue number</returns>
        /// <remarks></remarks>
        public static int SofaReleaseNumber()
        {
            return SOFA_RELEASE_NUMBER; // The release number of the issue being used
        }

        /// <summary>
        /// Revision number of the SOFA issue currently used by this component.
        /// </summary>
        /// <returns>Integer revision number</returns>
        /// <remarks></remarks>
        public static int SofaRevisionNumber()
        {
            return SOFA_REVISION_NUMBER; // The revision number of the issue being used
        }

        /// <summary>
        /// Release date of the SOFA issue currently used by this component.
        /// </summary>
        /// <returns>String date in format yyyy-mm-dd</returns>
        /// <remarks></remarks>
        public static string SofaIssueDate()
        {
            return SOFA_ISSUE_DATE; // Release date of the fundamental software issue currently being used 
        }

        /// <summary>
        /// Release date of the revision to the SOFA Issue that is actually being used by this component.
        /// </summary>
        /// <returns>String date in format yyyy-mm-dd</returns>
        /// <remarks>When a new issue is employed that doesn't yet have a revision, this method will return the SofaIssueDate</remarks>
        public static string SofaRevisionDate()
        {
            return SOFA_REVISION_DATE; // Release date of the revision currently being used 
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Validates that an array parameter is not null and has the expected size.
        /// </summary>
        /// <param name="array">The array to validate.</param>
        /// <param name="expectedSize">The expected size of the array.</param>
        /// <param name="arrayName">The name of the parameter being validated.</param>
        /// <exception cref="ArgumentNullException">Thrown if the array is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the array does not have the expected size.</exception>
        private static void ValidateArray(Array array, int expectedSize, string arrayName)
        {
            if (array == null)
            {
                throw new ArgumentNullException(arrayName, $"Array {arrayName} cannot be null.");
            }

            if (array.Length != expectedSize)
            {
                throw new ArgumentException($"Array {arrayName} must have exactly {expectedSize} elements (length was {array.Length}).", arrayName);
            }
        }


        private static void ValidateString(string stringParameter, int expectedSize, string parameterName)
        {
            if (stringParameter is null)
            {
                throw new ArgumentNullException(parameterName, $"String {stringParameter} cannot be null and must be at least {expectedSize} characters long.");
            }
            if ((stringParameter.Length < expectedSize) & (stringParameter.Length > 0))
            {
                throw new ArgumentException($"String {parameterName} must have {expectedSize} or more elements (supplied string length was {stringParameter.Length}).", parameterName);
            }
        }
        #endregion

        #region Sofa entry points

        /// <summary>
        /// A2af (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauA2af", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauA2af(
            int ndp,
            double angle,
            out byte sign,               // '+' or '-'
            [Out, MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] int[] idmsf // length 4: h,m,s,fraction
        );

        /// <summary>
        /// Angle to degrees, arcminutes, arcseconds, fraction.
        /// </summary>
        /// <param name="ndp">Number of decimal places of arcseconds.</param>
        /// <param name="angle">Angle in radians.</param>
        /// <param name="sign">Returned sign ('+' or '-').</param>
        /// <param name="idmsf">Returned degrees, arcminutes, arcseconds, fraction (length 4).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void A2af(int ndp, double angle, out char sign, int[] idmsf)
        {
            ValidateArray(idmsf, 4, nameof(idmsf));

            iauA2af(ndp, angle, out byte signByte, idmsf);
            sign = Convert.ToChar(signByte);
        }

        /// <summary>
        /// A2tf (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauA2tf", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauA2tf(
            int ndp,
            double angle,
            out byte sign,               // '+' or '-'
            [Out, MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] int[] ihmsf // length 4: h,m,s,fraction
        );

        /// <summary>
        /// Angle to hours, minutes, seconds, fraction.
        /// </summary>
        /// <param name="ndp">Number of decimal places of seconds.</param>
        /// <param name="angle">Angle in radians.</param>
        /// <param name="sign">Returned sign ('+' or '-').</param>
        /// <param name="ihmsf">Returned hours, minutes, seconds, fraction (length 4).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void A2tf(int ndp, double angle, out char sign, int[] ihmsf)
        {
            ValidateArray(ihmsf, 4, nameof(ihmsf));

            iauA2tf(ndp, angle, out byte signByte, ihmsf);
            sign = Convert.ToChar(signByte);
        }

        /// <summary>
        /// Ab (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauAb", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauAb([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] pnat,
                                     [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] v,
                                     double s,
                                     double bm1,
                                     [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] ppr);

        /// <summary>
        /// Aberration helper: convert position vector in star's natural system to proper
        /// place with observer velocity etc.
        /// </summary>
        /// <param name="pnat">Natural direction to the star.</param>
        /// <param name="v">Observer barycentric velocity (vector).</param>
        /// <param name="s">Distance between Sun and observer (AU).</param>
        /// <param name="bm1">Lorentz factor: sqrt(1-|v|^2).</param>
        /// <param name="ppr">Proper direction to the star.</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Ab(double[] pnat, double[] v, double s, double bm1, double[] ppr)
        {
            ValidateArray(pnat, 3, nameof(pnat));
            ValidateArray(v, 3, nameof(v));
            ValidateArray(ppr, 3, nameof(ppr));

            iauAb(pnat, v, s, bm1, ppr);
        }

        /// <summary>
        /// Ae2hd (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauAe2hd", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauAe2hd(double az, double el, double phi, ref double ha, ref double dec);

        /// <summary>
        /// Azimuth and altitude to hour angle and declination.
        /// </summary>
        /// <param name="az">Azimuth (radians).</param>
        /// <param name="el">Elevation/altitude (radians).</param>
        /// <param name="phi">Observer geodetic latitude (radians).</param>
        /// <param name="ha">Returned hour angle (radians).</param>
        /// <param name="dec">Returned declination (radians).</param>
        public static void Ae2hd(double az, double el, double phi, ref double ha, ref double dec)
        {
            iauAe2hd(az, el, phi, ref ha, ref dec);
        }

        /// <summary>
        /// Af2a (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauAf2a", CallingConvention = CallingConvention.Cdecl)]
        private static extern short iauAf2a(char s, short ideg, short iamin, double asec, ref double rad);

        /// <summary>
        /// Convert degrees, arcminutes, arcseconds to radians.
        /// </summary>
        /// <param name="s">Sign:  '-' = negative, otherwise positive</param>
        /// <param name="ideg">Degrees</param>
        /// <param name="iamin">Arcminutes</param>
        /// <param name="asec">Arcseconds</param>
        /// <param name="rad">Angle in radian</param>
        /// <returns>Status:  0 = OK, 1 = ideg outside range 0-359, 2 = iamin outside range 0-59, 3 = asec outside range 0-59.999...</returns>
        /// <remarks>
        /// Notes:
        /// <list type="number">
        /// <item><description>The result is computed even if any of the range checks fail.</description></item>
        /// <item><description>Negative ideg, iamin and/or asec produce a warning status, but the absolute value is used in the conversion.</description></item>
        /// <item><description>If there are multiple errors, the status value reflects only the first, the smallest taking precedence.</description></item>
        /// </list>
        /// </remarks>
        /// <returns>Return value from Af2a</returns>
        public static short Af2a(char s, short ideg, short iamin, double asec, ref double rad)
        {
            return iauAf2a(s, ideg, iamin, asec, ref rad);
        }

        /// <summary>
        /// Anp (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauAnp", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauAnp(double a);

        /// <summary>
        /// Normalize angle into the range 0 &lt;= a &lt; 2pi.
        /// </summary>
        /// <param name="a">Angle (radians)</param>
        /// <returns>Angle in range 0-2pi</returns>
        /// <remarks></remarks>
        /// <returns>Return value from Anp</returns>
        public static double Anp(double a)
        {
            return iauAnp(a);
        }

        /// <summary>
        /// Anpm (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauAnpm", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauAnpm(double a);

        /// <summary>
        /// Normalize angle into the range -pi to +pi.
        /// </summary>
        /// <param name="a">Angle (radians).</param>
        /// <returns>Normalized angle in radians.</returns>
        /// <returns>Return value from Anpm</returns>
        public static double Anpm(double a)
        {
            return iauAnpm(a);
        }

        /// <summary>
        /// Apcg (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauApcg", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauApcg(double date1,
                                       double date2,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] ebpv,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] ehp,
                                       ref Astrom astrom);

        /// <summary>
        /// Prepare star-independent astrometry parameters using observer EBPV and Sun vector.
        /// </summary>
        /// <param name="date1">TDB as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TDB as a 2-part Julian Date (part 2).</param>
        /// <param name="ebpv">Earth barycentric position/velocity (length 6).</param>
        /// <param name="ehp">Earth heliocentric position (length 3).</param>
        /// <param name="astrom">Returned star-independent astrometry parameters.</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Apcg(double date1, double date2, double[] ebpv, double[] ehp, ref Astrom astrom)
        {
            ValidateArray(ebpv, 6, nameof(ebpv));
            ValidateArray(ehp, 3, nameof(ehp));

            iauApcg(date1, date2, ebpv, ehp, ref astrom);
        }

        /// <summary>
        /// Apcg13 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauApcg13", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauApcg13(double date1, double date2, ref Astrom astrom);

        /// <summary>
        /// Prepare star-independent astrometry parameters (IAU 2000/2006) using internal Earth ephemeris.
        /// </summary>
        /// <param name="date1">TDB as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TDB as a 2-part Julian Date (part 2).</param>
        /// <param name="astrom">Returned star-independent astrometry parameters.</param>
        public static void Apcg13(double date1, double date2, ref Astrom astrom)
        {
            iauApcg13(date1, date2, ref astrom);
        }

        /// <summary>
        /// Apci (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauApci", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauApci(double date1,
                                       double date2,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] ebpv,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] ehp,
                                       double x,
                                       double y,
                                       double s,
                                       ref Astrom astrom);

        /// <summary>
        /// Prepare astrometry parameters given observer PV and Sun vector.
        /// </summary>
        /// <param name="date1">TDB as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TDB as a 2-part Julian Date (part 2).</param>
        /// <param name="ebpv">Earth barycentric position/velocity (length 6).</param>
        /// <param name="ehp">Earth heliocentric position (length 3).</param>
        /// <param name="x">CIP X coordinate.</param>
        /// <param name="y">CIP Y coordinate.</param>
        /// <param name="s">CIO locator s.</param>
        /// <param name="astrom">Returned star-independent astrometry parameters.</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Apci(double date1, double date2, double[] ebpv, double[] ehp, double x, double y, double s, ref Astrom astrom)
        {
            ValidateArray(ebpv, 6, nameof(ebpv));
            ValidateArray(ehp, 3, nameof(ehp));

            iauApci(date1, date2, ebpv, ehp, x, y, s, ref astrom);
        }

        /// <summary>
        /// Apci13 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauApci13", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauApci13(double date1, double date2, ref Astrom astrom, ref double eo);

        /// <summary>
        /// Prepare astrometry parameters (IAU 2000/2006) and return equation of the origins.
        /// </summary>
        /// <param name="date1">TDB as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TDB as a 2-part Julian Date (part 2).</param>
        /// <param name="astrom">Returned star-independent astrometry parameters.</param>
        /// <param name="eo">Returned equation of the origins.</param>
        public static void Apci13(double date1, double date2, ref Astrom astrom, ref double eo)
        {
            iauApci13(date1, date2, ref astrom, ref eo);
        }

        /// <summary>
        /// Apco (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauApco", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauApco(double date1,
                                       double date2,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] ebpv,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] ehp,
                                       double x,
                                       double y,
                                       double s,
                                       double theta,
                                       double elong,
                                       double phi,
                                       double hm,
                                       double xp,
                                       double yp,
                                       double sp,
                                       double refa,
                                       double refb,
                                       ref Astrom astrom);

        /// <summary>
        /// Prepare astrometry parameters for observed place (with refraction constants).
        /// </summary>
        /// <param name="date1">TDB as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TDB as a 2-part Julian Date (part 2).</param>
        /// <param name="ebpv">Earth barycentric position/velocity (length 6).</param>
        /// <param name="ehp">Earth heliocentric position (length 3).</param>
        /// <param name="x">CIP X coordinate.</param>
        /// <param name="y">CIP Y coordinate.</param>
        /// <param name="s">CIO locator s.</param>
        /// <param name="theta">Earth rotation angle (radians).</param>
        /// <param name="elong">Observer longitude (radians, east positive).</param>
        /// <param name="phi">Observer geodetic latitude (radians).</param>
        /// <param name="hm">Observer height above ellipsoid (m).</param>
        /// <param name="xp">Polar motion X (radians).</param>
        /// <param name="yp">Polar motion Y (radians).</param>
        /// <param name="sp">TIO locator s' (radians).</param>
        /// <param name="refa">Refraction constant A.</param>
        /// <param name="refb">Refraction constant B.</param>
        /// <param name="astrom">Returned astrometry parameters.</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Apco(double date1, double date2, double[] ebpv, double[] ehp, double x, double y, double s, double theta, double elong, double phi, double hm, double xp, double yp, double sp, double refa, double refb, ref Astrom astrom)
        {
            ValidateArray(ebpv, 6, nameof(ebpv));
            ValidateArray(ehp, 3, nameof(ehp));

            iauApco(date1, date2, ebpv, ehp, x, y, s, theta, elong, phi, hm, xp, yp, sp, refa, refb, ref astrom);
        }

        /// <summary>
        /// Apco13 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauApco13", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauApco13(double utc1,
                                        double utc2,
                                        double dut1,
                                        double elong,
                                        double phi,
                                        double hm,
                                        double xp,
                                        double yp,
                                        double phpa,
                                        double tc,
                                        double rh,
                                        double wl,
                                        ref Astrom astrom,
                                        ref double eo);

        /// <summary>
        /// Convenience wrapper to prepare astrometry parameters for observed place (IAU 2000/2006).
        /// Returns status: 0 = ok, non-zero signals problems (see SOFA docs).
        /// </summary>
        /// <param name="utc1">UTC as a 2-part quasi Julian Date (part 1).</param>
        /// <param name="utc2">UTC as a 2-part quasi Julian Date (part 2).</param>
        /// <param name="dut1">UT1-UTC (seconds).</param>
        /// <param name="elong">Observer longitude (radians, east positive).</param>
        /// <param name="phi">Observer geodetic latitude (radians).</param>
        /// <param name="hm">Observer height above ellipsoid (m).</param>
        /// <param name="xp">Polar motion X (radians).</param>
        /// <param name="yp">Polar motion Y (radians).</param>
        /// <param name="phpa">Pressure at observer (hPa).</param>
        /// <param name="tc">Ambient temperature (deg C).</param>
        /// <param name="rh">Relative humidity (0-1).</param>
        /// <param name="wl">Wavelength (micrometers).</param>
        /// <param name="astrom">Returned astrometry parameters.</param>
        /// <param name="eo">Returned equation of the origins.</param>
        /// <returns>Status code: 0 = OK, non-zero signals a problem (see SOFA documentation).</returns>
        /// <returns>Return value from Apco13</returns>
        public static int Apco13(double utc1, double utc2, double dut1, double elong, double phi, double hm, double xp, double yp, double phpa, double tc, double rh, double wl, ref Astrom astrom, ref double eo)
        {
            return iauApco13(utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref astrom, ref eo);
        }

        /// <summary>
        /// Apcs (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauApcs", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauApcs(double date1,
                                       double date2,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] pv,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] ebpv,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] ehp,
                                       ref Astrom astrom);

        /// <summary>
        /// Prepare astrometry parameters using pv and Earth heliocentric/barycentric vectors.
        /// </summary>
        /// <param name="date1">TDB as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TDB as a 2-part Julian Date (part 2).</param>
        /// <param name="pv">Observer barycentric position/velocity (length 6).</param>
        /// <param name="ebpv">Earth barycentric position/velocity (length 6).</param>
        /// <param name="ehp">Earth heliocentric position (length 3).</param>
        /// <param name="astrom">Returned star-independent astrometry parameters.</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Apcs(double date1, double date2, double[] pv, double[] ebpv, double[] ehp, ref Astrom astrom)
        {
            ValidateArray(pv, 6, nameof(pv));
            ValidateArray(ebpv, 6, nameof(ebpv));
            ValidateArray(ehp, 3, nameof(ehp));

            iauApcs(date1, date2, pv, ebpv, ehp, ref astrom);
        }

        /// <summary>
        /// Apcs13 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauApcs13", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauApcs13(double date1, double date2, [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] pv, ref Astrom astrom);

        /// <summary>
        /// Prepare astrometry parameters using pv (IAU 2000/2006).
        /// </summary>
        /// <param name="date1">TDB as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TDB as a 2-part Julian Date (part 2).</param>
        /// <param name="pv">Observer barycentric position/velocity (length 6).</param>
        /// <param name="astrom">Returned star-independent astrometry parameters.</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Apcs13(double date1, double date2, double[] pv, ref Astrom astrom)
        {
            ValidateArray(pv, 6, nameof(pv));

            iauApcs13(date1, date2, pv, ref astrom);
        }

        /// <summary>
        /// Aper (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauAper", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauAper(double theta, ref Astrom astrom);

        /// <summary>
        /// Apply refraction/perception corrections to astrometry parameters.
        /// </summary>
        /// <param name="theta">Earth rotation angle (radians).</param>
        /// <param name="astrom">Astrometry parameters to update.</param>
        public static void Aper(double theta, ref Astrom astrom)
        {
            iauAper(theta, ref astrom);
        }

        /// <summary>
        /// Aper13 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauAper13", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauAper13(double ut11, double ut12, ref Astrom astrom);

        /// <summary>
        /// Apply refraction/perception corrections (IAU 2000/2006).
        /// </summary>
        /// <param name="ut11">UT1 as a 2-part Julian Date (part 1).</param>
        /// <param name="ut12">UT1 as a 2-part Julian Date (part 2).</param>
        /// <param name="astrom">Astrometry parameters to update.</param>
        public static void Aper13(double ut11, double ut12, ref Astrom astrom)
        {
            iauAper13(ut11, ut12, ref astrom);
        }

        /// <summary>
        /// Apio (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauApio", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauApio(double sp, double theta, double elong, double phi, double hm, double xp, double yp, double refa, double refb, ref Astrom astrom);

        /// <summary>
        /// Prepare observer-related astrometry parameters.
        /// </summary>
        /// <param name="sp">TIO locator s' (radians).</param>
        /// <param name="theta">Earth rotation angle (radians).</param>
        /// <param name="elong">Observer longitude (radians, east positive).</param>
        /// <param name="phi">Observer geodetic latitude (radians).</param>
        /// <param name="hm">Observer height above ellipsoid (m).</param>
        /// <param name="xp">Polar motion X (radians).</param>
        /// <param name="yp">Polar motion Y (radians).</param>
        /// <param name="refa">Refraction constant A.</param>
        /// <param name="refb">Refraction constant B.</param>
        /// <param name="astrom">Returned astrometry parameters.</param>
        public static void Apio(double sp, double theta, double elong, double phi, double hm, double xp, double yp, double refa, double refb, ref Astrom astrom)
        {
            iauApio(sp, theta, elong, phi, hm, xp, yp, refa, refb, ref astrom);
        }

        /// <summary>
        /// Apio13 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauApio13", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauApio13(double utc1,
                                        double utc2,
                                        double dut1,
                                        double elong,
                                        double phi,
                                        double hm,
                                        double xp,
                                        double yp,
                                        double phpa,
                                        double tc,
                                        double rh,
                                        double wl,
                                        ref Astrom astrom);

        /// <summary>
        /// Convenience Apio (IAU 2000/2006) returning status.
        /// </summary>
        /// <param name="utc1">UTC as a 2-part quasi Julian Date (part 1).</param>
        /// <param name="utc2">UTC as a 2-part quasi Julian Date (part 2).</param>
        /// <param name="dut1">UT1-UTC (seconds).</param>
        /// <param name="elong">Observer longitude (radians, east positive).</param>
        /// <param name="phi">Observer geodetic latitude (radians).</param>
        /// <param name="hm">Observer height above ellipsoid (m).</param>
        /// <param name="xp">Polar motion X (radians).</param>
        /// <param name="yp">Polar motion Y (radians).</param>
        /// <param name="phpa">Pressure at observer (hPa).</param>
        /// <param name="tc">Ambient temperature (deg C).</param>
        /// <param name="rh">Relative humidity (0-1).</param>
        /// <param name="wl">Wavelength (micrometers).</param>
        /// <param name="astrom">Returned astrometry parameters.</param>
        /// <returns>Status code: 0 = OK, non-zero signals a problem (see SOFA documentation).</returns>
        /// <returns>Return value from Apio13</returns>
        public static int Apio13(double utc1, double utc2, double dut1, double elong, double phi, double hm, double xp, double yp, double phpa, double tc, double rh, double wl, ref Astrom astrom)
        {
            return iauApio13(utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref astrom);
        }

        /// <summary>
        /// Atcc13 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauAtcc13", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauAtcc13(double rc, double dc, double pr, double pd, double px, double rv, double date1, double date2, ref double ra, ref double da);

        /// <summary>
        /// Catalog-to-catalog transformation (example).
        /// </summary>
        /// <param name="rc">Catalog right ascension (radians).</param>
        /// <param name="dc">Catalog declination (radians).</param>
        /// <param name="pr">Proper motion in RA (radians/year).</param>
        /// <param name="pd">Proper motion in Dec (radians/year).</param>
        /// <param name="px">Parallax (arcsec).</param>
        /// <param name="rv">Radial velocity (km/s).</param>
        /// <param name="date1">TDB as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TDB as a 2-part Julian Date (part 2).</param>
        /// <param name="ra">Returned transformed right ascension (radians).</param>
        /// <param name="da">Returned transformed declination (radians).</param>
        public static void Atcc13(double rc, double dc, double pr, double pd, double px, double rv, double date1, double date2, ref double ra, ref double da)
        {
            iauAtcc13(rc, dc, pr, pd, px, rv, date1, date2, ref ra, ref da);
        }

        /// <summary>
        /// Atccq (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauAtccq", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauAtccq(double rc, double dc, double pr, double pd, double px, double rv, ref Astrom astrom, ref double ra, ref double da);

        /// <summary>
        /// Catalog-to-catalog using prepared astrometry.
        /// </summary>
        /// <param name="rc">Catalog right ascension (radians).</param>
        /// <param name="dc">Catalog declination (radians).</param>
        /// <param name="pr">Proper motion in RA (radians/year).</param>
        /// <param name="pd">Proper motion in Dec (radians/year).</param>
        /// <param name="px">Parallax (arcsec).</param>
        /// <param name="rv">Radial velocity (km/s).</param>
        /// <param name="astrom">Star-independent astrometry parameters.</param>
        /// <param name="ra">Returned transformed right ascension (radians).</param>
        /// <param name="da">Returned transformed declination (radians).</param>
        public static void Atccq(double rc, double dc, double pr, double pd, double px, double rv, ref Astrom astrom, ref double ra, ref double da)
        {
            iauAtccq(rc, dc, pr, pd, px, rv, ref astrom, ref ra, ref da);
        }

        /// <summary>
        /// Atci13 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauAtci13", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauAtci13(double rc,
                                         double dc,
                                         double pr,
                                         double pd,
                                         double px,
                                         double rv,
                                         double date1,
                                         double date2,
                                         ref double ri,
                                         ref double di,
                                         ref double eo);

        /// <summary>
        /// Transform ICRS star data, epoch J2000.0, to CIRS using the SOFA Atci13 function.
        /// </summary>
        /// <param name="rc">ICRS right ascension at J2000.0 (radians, Note 1)</param>
        /// <param name="dc">ICRS declination at J2000.0 (radians, Note 1)</param>
        /// <param name="pr">RA proper motion (radians/year; Note 2)</param>
        /// <param name="pd">Dec proper motion (radians/year)</param>
        /// <param name="px">parallax (arcsec)</param>
        /// <param name="rv">radial velocity (km/s, +ve if receding)</param>
        /// <param name="date1">TDB as a 2-part Julian Date (Note 3)</param>
        /// <param name="date2">TDB as a 2-part Julian Date (Note 3)</param>
        /// <param name="ri">CIRS geocentric RA (radians)</param>
        /// <param name="di">CIRS geocentric Dec (radians)</param>
        /// <param name="eo">equation of the origins (ERA-GST, Note 5)</param>
        /// <remarks>
        /// Notes:
        /// <list type="number">
        /// <item><description>Star data for an epoch other than J2000.0 (for example from the Hipparcos catalog, which has an epoch of J1991.25) will require a preliminary call to iauPmsafe before use.</description></item>
        /// <item><description>The proper motion in RA is dRA/dt rather than cos(Dec)*dRA/dt.</description></item>
        /// <item><description> The TDB date date1+date2 is a Julian Date, apportioned in any convenient way between the two arguments.  For example, JD(TDB)=2450123.8g could be expressed in any of these ways, among others:
        /// <table style="width:340px;" cellspacing="0">
        /// <col style="width:80px;"></col>
        /// <col style="width:80px;"></col>
        /// <col style="width:180px;"></col>
        /// <tr>
        /// <td colspan="1" align="center" rowspan="1" style="width: 80px; padding-right: 10px; padding-left: 10px; 
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-top-color: #000000; border-top-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid;
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; 
        /// background-color: #00ffff;" width="110px">
        /// <b>Date 1</b></td>
        /// <td colspan="1" rowspan="1" align="center" style="width: 80px; padding-right: 10px; padding-left: 10px; 
        /// border-top-color: #000000; border-top-style: Solid; 
        /// border-right-style: Solid; border-right-color: #000000; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; 
        /// background-color: #00ffff;" width="110px">
        /// <b>Date 2</b></td>
        /// <td colspan="1" rowspan="1" align="center" style="width: 180px; padding-right: 10px; padding-left: 10px; 
        /// border-top-color: #000000; border-top-style: Solid; 
        /// border-right-style: Solid; border-right-color: #000000; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; 
        /// background-color: #00ffff;" width="220px">
        /// <b>Method</b></td>
        /// </tr>
        /// <tr>
        /// <td align="right" style="padding-right: 10px; padding-left: 10px; 
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        ///  2450123.8</td>
        /// <td align="right" style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 0.0</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// JD method</td>
        /// </tr>
        /// <tr>
        /// <td align="right" style="padding-right: 10px; padding-left: 10px; 
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 2451545.0</td>
        /// <td align="right" style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// -1421.3</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// J2000 method</td>
        /// </tr>
        /// <tr>
        /// <td align="right" style="padding-right: 10px; padding-left: 10px; 
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 2400000.5</td>
        /// <td align="right" style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 50123.2</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// MJD method</td>
        /// </tr>
        /// <tr>
        /// <td align="right" style="padding-right: 10px; padding-left: 10px; 
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 2450123.5</td>
        /// <td align="right" style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 0.2</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Date and time method</td>
        /// </tr>
        /// </table>
        /// <para>The JD method is the most natural and convenient to use in cases where the loss of several decimal digits of resolution is acceptable.  The J2000 method is best matched to the way the argument is handled internally 
        /// and will deliver the optimum resolution.  The MJD method and the date and time methods are both good compromises between resolution and convenience.  For most applications of this function the choice will not be at all critical.</para>
        /// <para>TT can be used instead of TDB without any significant impact on accuracy.</para>
        /// </description></item>
        /// <item><description>The available accuracy is better than 1 milliarcsecond, limited mainly by the precession-nutation model that is used, namely IAU 2000A/2006.  Very close to solar system bodies, additional 
        /// errors of up to several milliarcseconds can occur because of unmodelled light deflection;  however, the Sun's contribution is taken into account, to first order.The accuracy limitations of 
        /// the SOFA function iauEpv00 (used to compute Earth position and velocity) can contribute aberration errors of up to 5 microarcseconds.  Light deflection at the Sun's limb is uncertain at the 0.4 mas level.</description></item>
        /// <item><description>Should the transformation to (equinox based) apparent place be required rather than (CIO based) intermediate place, subtract the equation of the origins from the returned right ascension:
        /// RA = RI - EO. (The Anp function can then be applied, as required, to keep the result in the conventional 0-2pi range.)</description></item>
        /// </list>
        /// </remarks>
        public static void Atci13(double rc, double dc, double pr, double pd, double px, double rv, double date1, double date2, ref double ri, ref double di, ref double eo)
        {
            iauAtci13(rc, dc, pr, pd, px, rv, date1, date2, ref ri, ref di, ref eo);
        }

        /// <summary>
        /// Atciq (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauAtciq", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauAtciq(double rc, double dc, double pr, double pd, double px, double rv, ref Astrom astrom, ref double ri, ref double di);

        /// <summary>
        /// ICRS->CIRS using prepared astrometry (inverse of atci).
        /// </summary>
        /// <param name="rc">ICRS right ascension (radians).</param>
        /// <param name="dc">ICRS declination (radians).</param>
        /// <param name="pr">Proper motion in RA (radians/year).</param>
        /// <param name="pd">Proper motion in Dec (radians/year).</param>
        /// <param name="px">Parallax (arcsec).</param>
        /// <param name="rv">Radial velocity (km/s).</param>
        /// <param name="astrom">Star-independent astrometry parameters.</param>
        /// <param name="ri">Returned CIRS right ascension (radians).</param>
        /// <param name="di">Returned CIRS declination (radians).</param>
        public static void Atciq(double rc, double dc, double pr, double pd, double px, double rv, ref Astrom astrom, ref double ri, ref double di)
        {
            iauAtciq(rc, dc, pr, pd, px, rv, ref astrom, ref ri, ref di);
        }

        /// <summary>
        /// Atciqn (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauAtciqn", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauAtciqn(double rc,
                                         double dc,
                                         double pr,
                                         double pd,
                                         double px,
                                         double rv,
                                         ref Astrom astrom,
                                         int n,
                                         LdBody[] b,
                                         ref double ri,
                                         ref double di);

        /// <summary>
        /// Variant with bodies for light deflection corrections.
        /// </summary>
        /// <param name="rc">ICRS right ascension (radians).</param>
        /// <param name="dc">ICRS declination (radians).</param>
        /// <param name="pr">Proper motion in RA (radians/year).</param>
        /// <param name="pd">Proper motion in Dec (radians/year).</param>
        /// <param name="px">Parallax (arcsec).</param>
        /// <param name="rv">Radial velocity (km/s).</param>
        /// <param name="astrom">Star-independent astrometry parameters.</param>
        /// <param name="n">Number of bodies in <paramref name="b"/>.</param>
        /// <param name="b">Bodies for light deflection.</param>
        /// <param name="ri">Returned CIRS right ascension (radians).</param>
        /// <param name="di">Returned CIRS declination (radians).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Atciqn(double rc, double dc, double pr, double pd, double px, double rv, ref Astrom astrom, int n, LdBody[] b, ref double ri, ref double di)
        {

            iauAtciqn(rc, dc, pr, pd, px, rv, ref astrom, n, b, ref ri, ref di);
        }

        /// <summary>
        /// Atciqz (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauAtciqz", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauAtciqz(double rc, double dc, ref Astrom astrom, ref double ri, ref double di);

        /// <summary>
        /// Quick form of atci for zeroing proper motion/parallax.
        /// </summary>
        /// <param name="rc">ICRS right ascension (radians).</param>
        /// <param name="dc">ICRS declination (radians).</param>
        /// <param name="astrom">Star-independent astrometry parameters.</param>
        /// <param name="ri">Returned CIRS right ascension (radians).</param>
        /// <param name="di">Returned CIRS declination (radians).</param>
        public static void Atciqz(double rc, double dc, ref Astrom astrom, ref double ri, ref double di)
        {
            iauAtciqz(rc, dc, ref astrom, ref ri, ref di);
        }

        /// <summary>
        /// Atco13 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauAtco13", CallingConvention = CallingConvention.Cdecl)]
        private static extern short iauAtco13(double rc,
                                          double dc,
                                          double pr,
                                          double pd,
                                          double px,
                                          double rv,
                                          double utc1,
                                          double utc2,
                                          double dut1,
                                          double elong,
                                          double phi,
                                          double hm,
                                          double xp,
                                          double yp,
                                          double phpa,
                                          double tc,
                                          double rh,
                                          double wl,
                                          ref double aob,
                                          ref double zob,
                                          ref double hob,
                                          ref double dob,
                                          ref double rob,
                                          ref double eo);

        /// <summary>
        /// ICRS RA,Dec to observed place using the SOFA Atco13 function.
        /// </summary>
        /// <param name="rc">ICRS RA (radians, note 1)</param>
        /// <param name="dc">ICRS Dec (radians, note 2)</param>
        /// <param name="pr">RA Proper motion (radians/year)</param>
        /// <param name="pd">Dec Proper motion (radians/year</param>
        /// <param name="px">Parallax (arcsec)</param>
        /// <param name="rv">Radial velocity (Km/s, +ve if receding</param>
        /// <param name="utc1">UTC Julian date (part 1, notes 3,4)</param>
        /// <param name="utc2">UTC Julian date (part 2, notes 3,4)</param>
        /// <param name="dut1">UT1 - UTC (seconds, note 5)</param>
        /// <param name="elong">Site longitude (radians, note 6)</param>
        /// <param name="phi">Site Latitude (radians, note 6)</param>
        /// <param name="hm">Site Height (meters, notes 6,8)</param>
        /// <param name="xp">Polar motion co-ordinate (radians, note 7)</param>
        /// <param name="yp">Polar motion co-ordinate (radians,note 7)</param>
        /// <param name="phpa">Site Pressure (hPa = mB, note 8)</param>
        /// <param name="tc">Site Temperature (C)</param>
        /// <param name="rh">Site relative humidity (fraction in the range: 0.0 to 1.0)</param>
        /// <param name="wl">Observation wavem=length (micrometres, note 9)</param>
        /// <param name="aob">Observed Azimuth (radians)</param>
        /// <param name="zob">Observed Zenith distance (radians)</param>
        /// <param name="hob">Observed Hour Angle (radians)</param>
        /// <param name="dob">Observed Declination (radians)</param>
        /// <param name="rob">Observed RA (radians)</param>
        /// <param name="eo">Equation of the origins (ERA-GST)</param>
        /// <returns>+1 = dubious year (Note 4), 0 = OK, -1 = unacceptable date</returns>
        /// <remarks>
        /// Notes:
        /// <list type="number">
        /// <item><description>Star data for an epoch other than J2000.0 (for example from the Hipparcos catalog, which has an epoch of J1991.25) will require a preliminary call to iauPmsafe before use.</description></item>
        /// <item><description>The proper motion in RA is dRA/dt rather than cos(Dec)*dRA/dt.</description></item>
        /// <item><description>utc1+utc2 is quasi Julian Date (see Note 2), apportioned in any convenient way between the two arguments, for example where utc1 is the Julian Day Number and utc2 is the fraction of a day.
        /// <para>However, JD cannot unambiguously represent UTC during a leap second unless special measures are taken.  The convention in the present function is that the JD day represents UTC days whether the length is 86399, 86400 or 86401 SI seconds.</para>
        /// <para>Applications should use the function iauDtf2d to convert from calendar date and time of day into 2-part quasi Julian Date, as it implements the leap-second-ambiguity convention just described.</para></description></item>
        /// <item><description>The warning status "dubious year" flags UTCs that predate the introduction of the time scale or that are too far in the future to be trusted.  See iauDat for further details.</description></item>
        /// <item><description>UT1-UTC is tabulated in IERS bulletins.  It increases by exactly one second at the end of each positive UTC leap second, introduced in order to keep UT1-UTC within +/- 0.9s.  n.b. This practice is under review, and in the future UT1-UTC may grow essentially without limit.</description></item>
        /// <item><description>The geographical coordinates are with respect to the WGS84 reference ellipsoid.  TAKE CARE WITH THE LONGITUDE SIGN:  the longitude required by the present function is east-positive (i.e. right-handed), in accordance with geographical convention.</description></item>
        /// <item><description>The polar motion xp,yp can be obtained from IERS bulletins.  The values are the coordinates (in radians) of the Celestial Intermediate Pole with respect to the International Terrestrial Reference System (see IERS Conventions 2003), measured along the meridians 0 and 90 deg west respectively.  For many applications, xp and yp can be set to zero.</description></item>
        /// <item><description>If hm, the height above the ellipsoid of the observing station in meters, is not known but phpa, the pressure in hPa (=mB), is available, an adequate estimate of hm can be obtained from the expression:
        /// <p style="margin-left:25px;font-family:Lucida Conosle,Monospace"><b>hm = -29.3 * tsl * log ( phpa / 1013.25 );</b></p>
        /// <para>where tsl is the approximate sea-level air temperature in K (See Astrophysical Quantities, C.W.Allen, 3rd edition, section 52).  Similarly, if the pressure phpa is not known, it can be estimated from the height of the observing station, hm, as follows:</para>
        /// <p style="margin-left:25px;font-family:Lucida Conosle,Monospace"><b>phpa = 1013.25 * exp ( -hm / ( 29.3 * tsl ) );</b></p>
        /// <para>Note, however, that the refraction is nearly proportional to the pressure and that an accurate phpa value is important for precise work.</para></description></item>
        /// <item><description>The argument wl specifies the observing wavelength in micrometers.  The transition from optical to radio is assumed to occur at 100 micrometers (about 3000 GHz).</description></item>
        /// <item><description>The accuracy of the result is limited by the corrections for refraction, which use a simple A*tan(z) + B*tan^3(z) model. Providing the meteorological parameters are known accurately and there are no gross local effects, the predicted observed coordinates should be within 0.05 arcsec (optical) or 1 arcsec (radio) for a zenith distance of less than 70 degrees, better than 30 arcsec (optical or radio) at 85 degrees and better than 20 arcmin (optical) or 30 arcmin (radio) at the horizon.
        /// <para>Without refraction, the complementary functions iauAtco13 and iauAtoc13 are self-consistent to better than 1 microarcsecond all over the celestial sphere.  With refraction included, consistency falls off at high zenith distances, but is still better than 0.05 arcsec at 85 degrees.</para></description></item>
        /// <item><description>"Observed" Az,ZD means the position that would be seen by a perfect geodetically aligned theodolite.  (Zenith distance is used rather than altitude in order to reflect the fact that no allowance is made for depression of the horizon.)  This is related to the observed HA,Dec via the standard rotation, using the geodetic latitude (corrected for polar motion), while the observed HA and RA are related simply through the Earth rotation angle and the site longitude.  "Observed" RA,Dec or HA,Dec thus means the position that would be seen by a perfect equatorial with its polar axis aligned to the Earth's axis of rotation.</description></item>
        /// <item><description>It is advisable to take great care with units, as even unlikely values of the input parameters are accepted and processed in accordance with the models used.</description></item>
        /// </list>
        /// </remarks>
        /// <returns>Return value from Atco13</returns>
        public static short Atco13(double rc, double dc, double pr, double pd, double px, double rv, double utc1, double utc2, double dut1, double elong, double phi, double hm, double xp, double yp, double phpa, double tc, double rh, double wl, ref double aob, ref double zob, ref double hob, ref double dob, ref double rob, ref double eo)
        {
            return iauAtco13(rc, dc, pr, pd, px, rv, utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref aob, ref zob, ref hob, ref dob, ref rob, ref eo);
        }

        /// <summary>
        /// Atic13 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauAtic13", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauAtic13(double ri, double di, double date1, double date2, ref double rc, ref double dc, ref double eo);

        /// <summary>
        /// Transform star RA,Dec from geocentric CIRS to ICRS astrometric using the SOFA Atic13 function.
        /// </summary>
        /// <param name="ri">CIRS geocentric RA (radians)</param>
        /// <param name="di">CIRS geocentric Dec (radians)</param>
        /// <param name="date1">TDB as a 2-part Julian Date (Note 1)</param>
        /// <param name="date2">TDB as a 2-part Julian Date (Note 1)</param>
        /// <param name="rc">ICRS astrometric RA (radians)</param>
        /// <param name="dc">ICRS astrometric Dec (radians)</param>
        /// <param name="eo">equation of the origins (ERA-GST, Note 4)</param>
        /// <remarks>
        /// Notes:
        /// <list type="number">
        /// <item><description> The TDB date date1+date2 is a Julian Date, apportioned in any convenient way between the two arguments.  For example, JD(TDB)=2450123.8g could be expressed in any of these ways, among others:
        /// <table style="width:340px;" cellspacing="0">
        /// <col style="width:80px;"></col>
        /// <col style="width:80px;"></col>
        /// <col style="width:180px;"></col>
        /// <tr>
        /// <td colspan="1" align="center" rowspan="1" style="width: 80px; padding-right: 10px; padding-left: 10px; 
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-top-color: #000000; border-top-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid;
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; 
        /// background-color: #00ffff;" width="110px">
        /// <b>Date 1</b></td>
        /// <td colspan="1" rowspan="1" align="center" style="width: 80px; padding-right: 10px; padding-left: 10px; 
        /// border-top-color: #000000; border-top-style: Solid; 
        /// border-right-style: Solid; border-right-color: #000000; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; 
        /// background-color: #00ffff;" width="110px">
        /// <b>Date 2</b></td>
        /// <td colspan="1" rowspan="1" align="center" style="width: 180px; padding-right: 10px; padding-left: 10px; 
        /// border-top-color: #000000; border-top-style: Solid; 
        /// border-right-style: Solid; border-right-color: #000000; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; 
        /// background-color: #00ffff;" width="220px">
        /// <b>Method</b></td>
        /// </tr>
        /// <tr>
        /// <td align="right" style="padding-right: 10px; padding-left: 10px; 
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        ///  2450123.8</td>
        /// <td align="right" style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 0.0</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// JD method</td>
        /// </tr>
        /// <tr>
        /// <td align="right" style="padding-right: 10px; padding-left: 10px; 
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 2451545.0</td>
        /// <td align="right" style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// -1421.3</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// J2000 method</td>
        /// </tr>
        /// <tr>
        /// <td align="right" style="padding-right: 10px; padding-left: 10px; 
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 2400000.5</td>
        /// <td align="right" style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 50123.2</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// MJD method</td>
        /// </tr>
        /// <tr>
        /// <td align="right" style="padding-right: 10px; padding-left: 10px; 
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 2450123.5</td>
        /// <td align="right" style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 0.2</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Date and time method</td>
        /// </tr>
        /// </table>
        /// <para>The JD method is the most natural and convenient to use in cases where the loss of several decimal digits of resolution is acceptable.  The J2000 method is best matched to the way the argument is handled internally 
        /// and will deliver the optimum resolution.  The MJD method and the date and time methods are both good compromises between resolution and convenience.  For most applications of this function the choice will not be at all critical.</para>
        /// <para>TT can be used instead of TDB without any significant impact on accuracy.</para>
        /// </description></item>
        /// <item><description>Iterative techniques are used for the aberration and light deflection corrections so that the functions Atic13 and Atci13 are accurate inverses; 
        /// even at the edge of the Sun's disk the discrepancy is only about 1 nanoarcsecond.</description></item>
        /// <item><description>The available accuracy is better than 1 milliarcsecond, limited mainly by the precession-nutation model that is used, namely IAU 2000A/2006.  Very close to solar system bodies, additional 
        /// errors of up to several milliarcseconds can occur because of unmodelled light deflection;  however, the Sun's contribution is taken into account, to first order.The accuracy limitations of 
        /// the SOFA function iauEpv00 (used to compute Earth position and velocity) can contribute aberration errors of up to 5 microarcseconds.  Light deflection at the Sun's limb is uncertain at the 0.4 mas level.</description></item>
        /// <item><description>Should the transformation to (equinox based) J2000.0 mean place be required rather than (CIO based) ICRS coordinates, subtract the equation of the origins from the returned right ascension:
        /// RA = RI - EO.  (The Anp function can then be applied, as required, to keep the result in the conventional 0-2pi range.)</description></item>
        /// </list>
        /// </remarks>
        public static void Atic13(double ri, double di, double date1, double date2, ref double rc, ref double dc, ref double eo)
        {
            iauAtic13(ri, di, date1, date2, ref rc, ref dc, ref eo);
        }

        /// <summary>
        /// Aticq (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauAticq", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauAticq(double ri, double di, ref Astrom astrom, ref double rc, ref double dc);

        /// <summary>
        /// CIRS->ICRS using prepared astrometry.
        /// </summary>
        /// <param name="ri">CIRS right ascension (radians).</param>
        /// <param name="di">CIRS declination (radians).</param>
        /// <param name="astrom">Star-independent astrometry parameters.</param>
        /// <param name="rc">Returned ICRS right ascension (radians).</param>
        /// <param name="dc">Returned ICRS declination (radians).</param>
        public static void Aticq(double ri, double di, ref Astrom astrom, ref double rc, ref double dc)
        {
            iauAticq(ri, di, ref astrom, ref rc, ref dc);
        }

        /// <summary>
        /// Aticqn (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauAticqn", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauAticqn(double ri, double di, ref Astrom astrom, int n, LdBody[] b, ref double rc, ref double dc);

        /// <summary>
        /// CIRS->ICRS with body corrections.
        /// </summary>
        /// <param name="ri">CIRS right ascension (radians).</param>
        /// <param name="di">CIRS declination (radians).</param>
        /// <param name="astrom">Star-independent astrometry parameters.</param>
        /// <param name="n">Number of bodies in <paramref name="b"/>.</param>
        /// <param name="b">Bodies for light deflection.</param>
        /// <param name="rc">Returned ICRS right ascension (radians).</param>
        /// <param name="dc">Returned ICRS declination (radians).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Aticqn(double ri, double di, ref Astrom astrom, int n, LdBody[] b, ref double rc, ref double dc)
        {

            iauAticqn(ri, di, ref astrom, n, b, ref rc, ref dc);
        }

        /// <summary>
        /// Atio13 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauAtio13", CallingConvention = CallingConvention.Cdecl)]
        private static extern short iauAtio13(double ri,
                                          double di,
                                          double utc1,
                                          double utc2,
                                          double dut1,
                                          double elong,
                                          double phi,
                                          double hm,
                                          double xp,
                                          double yp,
                                          double phpa,
                                          double tc,
                                          double rh,
                                          double wl,
                                          ref double aob,
                                          ref double zob,
                                          ref double hob,
                                          ref double dob,
                                          ref double rob);

        /// <summary>
        /// CIRS RA,Dec to observed place using the SOFA Atio13 funciton.
        /// </summary>
        /// <param name="ri">CIRS right ascension (CIO-based, radians)</param>
        /// <param name="di">CIRS declination (radians)</param>
        /// <param name="utc1">UTC as a 2-part quasi Julian Date (Notes 1,2)</param>
        /// <param name="utc2">UTC as a 2-part quasi Julian Date (Notes 1,2)</param>
        /// <param name="dut1">UT1-UTC (seconds, Note 3)</param>
        /// <param name="elong">longitude (radians, east +ve, Note 4)</param>
        /// <param name="phi">geodetic latitude (radians, Note 4)</param>
        /// <param name="hm">height above ellipsoid (m, geodetic Notes 4,6)</param>
        /// <param name="xp">polar motion coordinates (radians, Note 5)</param>
        /// <param name="yp">polar motion coordinates (radians, Note 5)</param>
        /// <param name="phpa">pressure at the observer (hPa = mB, Note 6)</param>
        /// <param name="tc">ambient temperature at the observer (deg C)</param>
        /// <param name="rh">relative humidity at the observer (range 0-1)</param>
        /// <param name="wl">wavelength (micrometers, Note 7)</param>
        /// <param name="aob">observed azimuth (radians: N=0,E=90)</param>
        /// <param name="zob">observed zenith distance (radians)</param>
        /// <param name="hob">observed hour angle (radians)</param>
        /// <param name="dob">observed declination (radians)</param>
        /// <param name="rob">observed right ascension (CIO-based, radians)</param>
        /// <returns> Status: +1 = dubious year (Note 2), 0 = OK, -1 = unacceptable date</returns>
        /// <remarks>
        /// <para>Notes:</para>
        /// <list type="number">
        /// <item><description>utc1+utc2 is quasi Julian Date (see Note 2), apportioned in any convenient way between the two arguments, for example where utc1 is the Julian Day Number and utc2 is the fraction of a day.
        /// <para>However, JD cannot unambiguously represent UTC during a leap second unless special measures are taken.  The convention in the present function is that the JD day represents UTC days whether the length is 86399, 86400 or 86401 SI seconds.</para>
        /// <para>Applications should use the function iauDtf2d to convert from calendar date and time of day into 2-part quasi Julian Date, as it implements the leap-second-ambiguity convention just described.</para></description></item>
        /// <item><description>The warning status "dubious year" flags UTCs that predate the introduction of the time scale or that are too far in the future to be trusted.  See iauDat for further details.</description></item>
        /// <item><description>UT1-UTC is tabulated in IERS bulletins.  It increases by exactly one second at the end of each positive UTC leap second, introduced in order to keep UT1-UTC within +/- 0.9s.  n.b. This practice is under review, and in the future UT1-UTC may grow essentially without limit.</description></item>
        /// <item><description>The geographical coordinates are with respect to the WGS84 reference ellipsoid.  TAKE CARE WITH THE LONGITUDE SIGN:  the longitude required by the present function is east-positive (i.e. right-handed), in accordance with geographical convention.</description></item>
        /// <item><description>The polar motion xp,yp can be obtained from IERS bulletins.  The values are the coordinates (in radians) of the Celestial Intermediate Pole with respect to the International Terrestrial
        /// Reference System (see IERS Conventions 2003), measured along the meridians 0 and 90 deg west respectively.  For many applications, xp and yp can be set to zero.</description></item>
        /// <item><description>If hm, the height above the ellipsoid of the observing station in meters, is not known but phpa, the pressure in hPa (=mB), is available, an adequate estimate of hm can be obtained from the expression:
        /// <p style="margin-left:25px;font-family:Lucida Conosle,Monospace"><b>hm = -29.3 * tsl * log ( phpa / 1013.25 );</b></p>
        /// <para>where tsl is the approximate sea-level air temperature in K (See Astrophysical Quantities, C.W.Allen, 3rd edition, section 52).  Similarly, if the pressure phpa is not known, it can be estimated from the height of the observing station, hm, as follows:</para>
        /// <p style="margin-left:25px;font-family:Lucida Conosle,Monospace"><b>phpa = 1013.25 * exp ( -hm / ( 29.3 * tsl ) );</b></p>
        /// <para>Note, however, that the refraction is nearly proportional to the pressure and that an accurate phpa value is important for precise work.</para></description></item>
        /// <item><description>The argument wl specifies the observing wavelength in micrometers.  The transition from optical to radio is assumed to occur at 100 micrometers (about 3000 GHz).</description></item>
        /// <item><description>"Observed" Az,ZD means the position that would be seen by a perfect geodetically aligned theodolite.  (Zenith distance is used rather than altitude in order to reflect the fact that no
        /// allowance is made for depression of the horizon.)  This is related to the observed HA,Dec via the standard rotation, using the geodetic latitude (corrected for polar motion), while the observed HA and RA are related simply through the Earth rotation
        /// angle and the site longitude.  "Observed" RA,Dec or HA,Dec thus means the position that would be seen by a perfect equatorial with its polar axis aligned to the Earth's axis of rotation.</description></item>
        /// <item><description>The accuracy of the result is limited by the corrections for refraction, which use a simple A*tan(z) + B*tan^3(z) model. Providing the meteorological parameters are known accurately and there are no gross local effects, the predicted astrometric
        /// coordinates should be within 0.05 arcsec (optical) or 1 arcsec (radio) for a zenith distance of less than 70 degrees, better than 30 arcsec (optical or radio) at 85 degrees and better than 20 arcmin (optical) or 30 arcmin (radio) at the horizon.</description></item>
        /// <item><description>The complementary functions iauAtio13 and iauAtoi13 are self-consistent to better than 1 microarcsecond all over the celestial sphere.</description></item>
        /// <item><description>It is advisable to take great care with units, as even unlikely values of the input parameters are accepted and processed in accordance with the models used.</description></item>
        /// </list>
        /// </remarks>
        /// <returns>Return value from Atio13</returns>
        public static short Atio13(double ri, double di, double utc1, double utc2, double dut1, double elong, double phi, double hm, double xp, double yp, double phpa, double tc, double rh, double wl, ref double aob, ref double zob, ref double hob, ref double dob, ref double rob)
        {
            return iauAtio13(ri, di, utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref aob, ref zob, ref hob, ref dob, ref rob);
        }

        /// <summary>
        /// Atioq (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauAtioq", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauAtioq(double ri, double di, ref Astrom astrom, ref double aob, ref double zob, ref double hob, ref double dob, ref double rob);

        /// <summary>
        /// CIRS to observed place using prepared astrometry.
        /// </summary>
        /// <param name="ri">CIRS right ascension (radians).</param>
        /// <param name="di">CIRS declination (radians).</param>
        /// <param name="astrom">Prepared astrometry parameters.</param>
        /// <param name="aob">Returned observed azimuth (radians).</param>
        /// <param name="zob">Returned observed zenith distance (radians).</param>
        /// <param name="hob">Returned observed hour angle (radians).</param>
        /// <param name="dob">Returned observed declination (radians).</param>
        /// <param name="rob">Returned observed right ascension (radians).</param>
        public static void Atioq(double ri, double di, ref Astrom astrom, ref double aob, ref double zob, ref double hob, ref double dob, ref double rob)
        {
            iauAtioq(ri, di, ref astrom, ref aob, ref zob, ref hob, ref dob, ref rob);
        }

        /// <summary>
        /// Atoc13 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauAtoc13", CallingConvention = CallingConvention.Cdecl)]
        private static extern short iauAtoc13(
                                          [MarshalAs(UnmanagedType.LPStr)] string type,
                                          double ob1,
                                          double ob2,
                                          double utc1,
                                          double utc2,
                                          double dut1,
                                          double elong,
                                          double phi,
                                          double hm,
                                          double xp,
                                          double yp,
                                          double phpa,
                                          double tc,
                                          double rh,
                                          double wl,
                                          ref double rc,
                                          ref double dc);

        /// <summary>
        /// Observed place at a ground based site to ICRS astrometric RA,Dec using the SOFA Atoc13 function.
        /// </summary>
        /// <param name="type">type of coordinates - "R", "H" or "A" (Notes 1,2)</param>
        /// <param name="ob1">observed Az, HA or RA (radians; Az is N=0,E=90)</param>
        /// <param name="ob2"> observed ZD or Dec (radians)</param>
        /// <param name="utc1">UTC as a 2-part quasi Julian Date (Notes 3,4)</param>
        /// <param name="utc2">UTC as a 2-part quasi Julian Date (Notes 3,4)</param>
        /// <param name="dut1">UT1-UTC (seconds, Note 5)</param>
        /// <param name="elong">longitude (radians, east +ve, Note 6)</param>
        /// <param name="phi">geodetic latitude (radians, Note 6)</param>
        /// <param name="hm">height above ellipsoid (m, geodetic Notes 6,8)</param>
        /// <param name="xp">polar motion coordinates (radians, Note 7)</param>
        /// <param name="yp">polar motion coordinates (radians, Note 7)</param>
        /// <param name="phpa">pressure at the observer (hPa = mB, Note 8)</param>
        /// <param name="tc">ambient temperature at the observer (deg C)</param>
        /// <param name="rh">relative humidity at the observer (range 0-1)</param>
        /// <param name="wl">wavelength (micrometers, Note 9)</param>
        /// <param name="rc">ICRS astrometric RA (radians)</param>
        /// <param name="dc">ICRS astrometric Dec (radians)</param>
        /// <returns>Status: +1 = dubious year (Note 4), 0 = OK, -1 = unacceptable date</returns>
        /// <remarks>
        /// <para>Notes:</para>
        /// <list type="number">
        /// <item><description>"Observed" Az,ZD means the position that would be seen by a perfect geodetically aligned theodolite.  (Zenith distance is used rather than altitude in order to reflect the fact that no
        /// allowance is made for depression of the horizon.)  This is related to the observed HA,Dec via the standard rotation, using the geodetic latitude (corrected for polar motion), while the
        /// observed HA and RA are related simply through the Earth rotation angle and the site longitude.  "Observed" RA,Dec or HA,Dec thus means the position that would be seen by a perfect equatorial with its polar axis aligned to the Earth's axis of rotation.</description></item>
        /// <item><description>Only the first character of the type argument is significant. "R" or "r" indicates that ob1 and ob2 are the observed right ascension and declination;  "H" or "h" indicates that they are hour angle (west +ve) and declination;  anything else ("A" or
        /// "a" is recommended) indicates that ob1 and ob2 are azimuth (north zero, east 90 deg) and zenith distance.</description></item>
        /// <item><description>utc1+utc2 is quasi Julian Date (see Note 2), apportioned in any convenient way between the two arguments, for example where utc1 is the Julian Day Number and utc2 is the fraction of a day.
        /// <para>However, JD cannot unambiguously represent UTC during a leap second unless special measures are taken.  The convention in the present function is that the JD day represents UTC days whether the length is 86399, 86400 or 86401 SI seconds.</para>
        /// <para>Applications should use the function iauDtf2d to convert from calendar date and time of day into 2-part quasi Julian Date, as it implements the leap-second-ambiguity convention just described.</para></description></item>
        /// <item><description>The warning status "dubious year" flags UTCs that predate the introduction of the time scale or that are too far in the future to be trusted.  See iauDat for further details.</description></item>
        /// <item><description>UT1-UTC is tabulated in IERS bulletins.  It increases by exactly one second at the end of each positive UTC leap second, introduced in order to keep UT1-UTC within +/- 0.9s.  n.b. This practice is under review, and in the future UT1-UTC may grow essentially without limit.</description></item>
        /// <item><description>The geographical coordinates are with respect to the WGS84 reference ellipsoid.  TAKE CARE WITH THE LONGITUDE SIGN:  the longitude required by the present function is east-positive (i.e. right-handed), in accordance with geographical convention.</description></item>
        /// <item><description>The polar motion xp,yp can be obtained from IERS bulletins.  The values are the coordinates (in radians) of the Celestial Intermediate Pole with respect to the International Terrestrial Reference System (see IERS Conventions 2003), measured along the
        /// meridians 0 and 90 deg west respectively.  For many applications, xp and yp can be set to zero.</description></item>
        /// <item><description>If hm, the height above the ellipsoid of the observing station in meters, is not known but phpa, the pressure in hPa (=mB), is available, an adequate estimate of hm can be obtained from the expression:
        /// <p style="margin-left:25px;font-family:Lucida Conosle,Monospace"><b>hm = -29.3 * tsl * log ( phpa / 1013.25 );</b></p>
        /// <para>where tsl is the approximate sea-level air temperature in K (See Astrophysical Quantities, C.W.Allen, 3rd edition, section 52).  Similarly, if the pressure phpa is not known, it can be estimated from the height of the observing station, hm, as follows:</para>
        /// <p style="margin-left:25px;font-family:Lucida Conosle,Monospace"><b>phpa = 1013.25 * exp ( -hm / ( 29.3 * tsl ) );</b></p>
        /// <para>Note, however, that the refraction is nearly proportional to the pressure and that an accurate phpa value is important for precise work.</para></description></item>
        /// <item><description>The argument wl specifies the observing wavelength in micrometers.  The transition from optical to radio is assumed to occur at 100 micrometers (about 3000 GHz).</description></item>
        /// <item><description>The accuracy of the result is limited by the corrections for refraction, which use a simple A*tan(z) + B*tan^3(z) model. Providing the meteorological parameters are known accurately and
        /// there are no gross local effects, the predicted astrometric coordinates should be within 0.05 arcsec (optical) or 1 arcsec (radio) for a zenith distance of less than 70 degrees, better than 30 arcsec (optical or radio) at 85 degrees and better
        /// than 20 arcmin (optical) or 30 arcmin (radio) at the horizon.
        ///<para>Without refraction, the complementary functions iauAtco13 and iauAtoc13 are self-consistent to better than 1 microarcsecond all over the celestial sphere.  With refraction included, consistency falls off at high zenith distances, but is still better than 0.05 arcsec at 85 degrees.</para></description></item>
        /// <item><description>It is advisable to take great care with units, as even unlikely values of the input parameters are accepted and processed in accordance with the models used.</description></item>
        /// </list>
        /// </remarks>
        /// <returns>Return value from Atoc13</returns>
        public static short Atoc13(string type, double ob1, double ob2, double utc1, double utc2, double dut1, double elong, double phi, double hm, double xp, double yp, double phpa, double tc, double rh, double wl, ref double rc, ref double dc)
        {
            ValidateString(type, 1, nameof(type));
            return iauAtoc13(type, ob1, ob2, utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref rc, ref dc);
        }

        /// <summary>
        /// Atoi13 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauAtoi13", CallingConvention = CallingConvention.Cdecl)]
        private static extern short iauAtoi13(
                                          [MarshalAs(UnmanagedType.LPStr)] string type,
                                          double ob1,
                                          double ob2,
                                          double utc1,
                                          double utc2,
                                          double dut1,
                                          double elong,
                                          double phi,
                                          double hm,
                                          double xp,
                                          double yp,
                                          double phpa,
                                          double tc,
                                          double rh,
                                          double wl,
                                          ref double ri,
                                          ref double di);

        /// <summary>
        ///  Observed place to CIRS using the SOFA Atoi13 function.
        /// </summary>
        /// <param name="type">type of coordinates - "R", "H" or "A" (Notes 1,2)</param>
        /// <param name="ob1">observed Az, HA or RA (radians; Az is N=0,E=90)</param>
        /// <param name="ob2">observed ZD or Dec (radians)</param>
        /// <param name="utc1">UTC as a 2-part quasi Julian Date (Notes 3,4)</param>
        /// <param name="utc2">UTC as a 2-part quasi Julian Date (Notes 3,4)</param>
        /// <param name="dut1">UT1-UTC (seconds, Note 5)</param>
        /// <param name="elong">longitude (radians, east +ve, Note 6)</param>
        /// <param name="phi">geodetic latitude (radians, Note 6)</param>
        /// <param name="hm">height above the ellipsoid (meters, Notes 6,8)</param>
        /// <param name="xp">polar motion coordinates (radians, Note 7)</param>
        /// <param name="yp">polar motion coordinates (radians, Note 7)</param>
        /// <param name="phpa">pressure at the observer (hPa = mB, Note 8)</param>
        /// <param name="tc">ambient temperature at the observer (deg C)</param>
        /// <param name="rh">relative humidity at the observer (range 0-1)</param>
        /// <param name="wl">wavelength (micrometers, Note 9)</param>
        /// <param name="ri">CIRS right ascension (CIO-based, radians)</param>
        /// <param name="di">CIRS declination (radians)</param>
        /// <returns>Status: +1 = dubious year (Note 2), 0 = OK, -1 = unacceptable date</returns>
        /// <remarks>
        /// <para>Notes:</para>
        /// <list type="number">
        /// <item><description>"Observed" Az,ZD means the position that would be seen by a perfect geodetically aligned theodolite.  (Zenith distance is used rather than altitude in order to reflect the fact that no
        /// allowance is made for depression of the horizon.)  This is related to the observed HA,Dec via the standard rotation, using the geodetic latitude (corrected for polar motion), while the
        /// observed HA and RA are related simply through the Earth rotation angle and the site longitude.  "Observed" RA,Dec or HA,Dec thus means the position that would be seen by a perfect equatorial
        /// with its polar axis aligned to the Earth's axis of rotation.</description></item>
        /// <item><description>Only the first character of the type argument is significant. "R" or "r" indicates that ob1 and ob2 are the observed right ascension and declination;  "H" or "h" indicates that they are
        /// hour angle (west +ve) and declination;  anything else ("A" or "a" is recommended) indicates that ob1 and ob2 are azimuth (north zero, east 90 deg) and zenith distance.</description></item>
        /// <item><description>utc1+utc2 is quasi Julian Date (see Note 2), apportioned in any convenient way between the two arguments, for example where utc1 is the Julian Day Number and utc2 is the fraction of a day.
        /// <para>However, JD cannot unambiguously represent UTC during a leap second unless special measures are taken.  The convention in the present function is that the JD day represents UTC days whether the length is 86399, 86400 or 86401 SI seconds.</para>
        /// <para>Applications should use the function iauDtf2d to convert from calendar date and time of day into 2-part quasi Julian Date, as it implements the leap-second-ambiguity convention just described.</para></description></item>
        /// <item><description>The warning status "dubious year" flags UTCs that predate the introduction of the time scale or that are too far in the future to be trusted.  See iauDat for further details.</description></item>
        /// <item><description>UT1-UTC is tabulated in IERS bulletins.  It increases by exactly one second at the end of each positive UTC leap second, introduced in order to keep UT1-UTC within +/- 0.9s.  n.b. This
        /// practice is under review, and in the future UT1-UTC may grow essentially without limit.</description></item>
        /// <item><description>The geographical coordinates are with respect to the WGS84 reference ellipsoid.  TAKE CARE WITH THE LONGITUDE SIGN:  the longitude required by the present function is east-positive
        /// (i.e. right-handed), in accordance with geographical convention.</description></item>
        /// <item><description>The polar motion xp,yp can be obtained from IERS bulletins.  The values are the coordinates (in radians) of the Celestial Intermediate Pole with respect to the International Terrestrial
        /// Reference System (see IERS Conventions 2003), measured along the meridians 0 and 90 deg west respectively.  For many applications, xp and yp can be set to zero.</description></item>
        /// <item><description>If hm, the height above the ellipsoid of the observing station in meters, is not known but phpa, the pressure in hPa (=mB), is available, an adequate estimate of hm can be obtained from the expression:
        /// <p style="margin-left:25px;font-family:Lucida Conosle,Monospace"><b>hm = -29.3 * tsl * log ( phpa / 1013.25 );</b></p>
        /// <para>where tsl is the approximate sea-level air temperature in K (See Astrophysical Quantities, C.W.Allen, 3rd edition, section 52).  Similarly, if the pressure phpa is not known, it can be estimated from the height of the observing station, hm, as follows:</para>
        /// <p style="margin-left:25px;font-family:Lucida Conosle,Monospace"><b>phpa = 1013.25 * exp ( -hm / ( 29.3 * tsl ) );</b></p>
        /// <para>Note, however, that the refraction is nearly proportional to the pressure and that an accurate phpa value is important for precise work.</para></description></item>
        /// <item><description>The argument wl specifies the observing wavelength in micrometers.  The transition from optical to radio is assumed to occur at 100 micrometers (about 3000 GHz).</description></item>
        /// <item><description>The accuracy of the result is limited by the corrections for refraction, which use a simple A*tan(z) + B*tan^3(z) model. Providing the meteorological parameters are known accurately and
        /// there are no gross local effects, the predicted astrometric coordinates should be within 0.05 arcsec (optical) or 1 arcsec (radio) for a zenith distance of less than 70 degrees, better
        /// than 30 arcsec (optical or radio) at 85 degrees and better than 20 arcmin (optical) or 30 arcmin (radio) at the horizon.
        /// <para>Without refraction, the complementary functions iauAtio13 and iauAtoi13 are self-consistent to better than 1 microarcsecond all over the celestial sphere.  With refraction included,
        /// consistency falls off at high zenith distances, but is still better than 0.05 arcsec at 85 degrees.</para></description></item>
        /// <item><description>It is advisable to take great care with units, as even unlikely values of the input parameters are accepted and processed in accordance with the models used.</description></item>
        /// </list>
        /// </remarks>
        /// <returns>Return value from Atoi13</returns>
        public static short Atoi13(string type, double ob1, double ob2, double utc1, double utc2, double dut1, double elong, double phi, double hm, double xp, double yp, double phpa, double tc, double rh, double wl, ref double ri, ref double di)
        {
            ValidateString(type, 1, nameof(type));
            return iauAtoi13(type, ob1, ob2, utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref ri, ref di);
        }

        /// <summary>
        /// Atoiq (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauAtoiq", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauAtoiq([MarshalAs(UnmanagedType.LPStr)] string type, double ob1, double ob2, ref Astrom astrom, ref double ri, ref double di);

        /// <summary>
        /// Observed place to CIRS using prepared astrometry.
        /// </summary>
        /// <param name="type">Coordinate type: "R"/"H"/"A".</param>
        /// <param name="ob1">Observed coordinate 1 (radians).</param>
        /// <param name="ob2">Observed coordinate 2 (radians).</param>
        /// <param name="astrom">Prepared astrometry parameters.</param>
        /// <param name="ri">Returned CIRS right ascension (radians).</param>
        /// <param name="di">Returned CIRS declination (radians).</param>
        public static void Atoiq(string type, double ob1, double ob2, ref Astrom astrom, ref double ri, ref double di)
        {
            ValidateString(type, 1, nameof(type));
            iauAtoiq(type, ob1, ob2, ref astrom, ref ri, ref di);
        }

        /// <summary>
        /// Bi00 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauBi00", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauBi00(ref double dpsibi, ref double depsbi, ref double dra);

        /// <summary>
        /// Fundamental arguments: Delaunay arguments and mean longitude of the ascending node of the Moon.
        /// </summary>
        /// <param name="dpsibi">Returned bias-precession in longitude.</param>
        /// <param name="depsbi">Returned bias-precession in obliquity.</param>
        /// <param name="dra">Returned frame bias.</param>
        public static void Bi00(ref double dpsibi, ref double depsbi, ref double dra)
        {
            iauBi00(ref dpsibi, ref depsbi, ref dra);
        }

        /// <summary>
        /// Bp00 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauBp00", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauBp00(double date1, double date2, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rb,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rp, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rbp);

        /// <summary>
        /// Frame bias and precession, IAU 2000.
        /// </summary>
        /// <remarks>
        /// Computes the frame bias matrix (<paramref name="rb"/>), the precession matrix (<paramref name="rp"/>) and the
        /// combined bias-precession matrix (<paramref name="rbp"/>) for a given TT date.
        /// </remarks>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="rb">Returned 3×3 frame-bias matrix (row-major, length 9).</param>
        /// <param name="rp">Returned 3×3 precession matrix (row-major, length 9).</param>
        /// <param name="rbp">Returned 3×3 bias-precession matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Bp00(double date1, double date2, double[] rb, double[] rp, double[] rbp)
        {
            ValidateArray(rb, 9, nameof(rb));
            ValidateArray(rp, 9, nameof(rp));
            ValidateArray(rbp, 9, nameof(rbp));

            iauBp00(date1, date2, rb, rp, rbp);
        }

        /// <summary>
        /// Bp06 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauBp06", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauBp06(double date1, double date2, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rb,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rp, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rbp);

        /// <summary>
        /// Frame bias and precession, IAU 2006.
        /// </summary>
        /// <remarks>
        /// Computes the frame bias matrix (<paramref name="rb"/>), the precession matrix (<paramref name="rp"/>) and the
        /// combined bias-precession matrix (<paramref name="rbp"/>) for a given TT date.
        /// </remarks>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="rb">Returned 3×3 frame-bias matrix (row-major, length 9).</param>
        /// <param name="rp">Returned 3×3 precession matrix (row-major, length 9).</param>
        /// <param name="rbp">Returned 3×3 bias-precession matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Bp06(double date1, double date2, double[] rb, double[] rp, double[] rbp)
        {
            ValidateArray(rb, 9, nameof(rb));
            ValidateArray(rp, 9, nameof(rp));
            ValidateArray(rbp, 9, nameof(rbp));

            iauBp06(date1, date2, rb, rp, rbp);
        }

        /// <summary>
        /// Bpn2xy (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauBpn2xy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauBpn2xy([MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rbpn, ref double x, ref double y);

        /// <summary>
        /// Extract the CIP X,Y coordinates from a bias-precession-nutation matrix.
        /// </summary>
        /// <remarks>
        /// The input matrix transforms vectors from GCRS to the true equator (and CIO/equinox) of date; the CIP unit vector is the
        /// bottom row of the matrix.
        /// </remarks>
        /// <param name="rbpn">Celestial-to-true rotation matrix (row-major, length 9).</param>
        /// <param name="x">Returned X coordinate of the Celestial Intermediate Pole (GCRS).</param>
        /// <param name="y">Returned Y coordinate of the Celestial Intermediate Pole (GCRS).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Bpn2xy(double[] rbpn, ref double x, ref double y)
        {
            ValidateArray(rbpn, 9, nameof(rbpn));

            iauBpn2xy(rbpn, ref x, ref y);
        }

        /// <summary>
        /// C2i00a (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauC2i00a", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauC2i00a(double date1, double date2, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rc2);

        /// <summary>
        /// Form the celestial-to-intermediate matrix using the IAU 2000A precession-nutation model.
        /// </summary>
        /// <remarks>
        /// This matrix is the first stage in the transformation from celestial (GCRS) to terrestrial (ITRS) coordinates.
        /// </remarks>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="rc2">Returned celestial-to-intermediate matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void C2i00a(double date1, double date2, double[] rc2)
        {
            ValidateArray(rc2, 9, nameof(rc2));

            iauC2i00a(date1, date2, rc2);
        }

        /// <summary>
        /// C2i00b (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauC2i00b", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauC2i00b(double date1, double date2, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rc2i);

        /// <summary>
        /// Form the celestial-to-intermediate matrix using the IAU 2000B precession-nutation model.
        /// </summary>
        /// <remarks>
        /// This routine is faster, but slightly less accurate (about 1 mas), than <see cref="C2i00a"/>.
        /// </remarks>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="rc2i">Returned celestial-to-intermediate matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void C2i00b(double date1, double date2, double[] rc2i)
        {
            ValidateArray(rc2i, 9, nameof(rc2i));

            iauC2i00b(date1, date2, rc2i);
        }

        /// <summary>
        /// C2i06a (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauC2i06a", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauC2i06a(double date1, double date2, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rc2i);

        /// <summary>
        /// Bias-precession-nutation matrix, IAU 2000/2006, using ICRS X,Y.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="rc2i">Returned celestial-to-intermediate matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void C2i06a(double date1, double date2, double[] rc2i)
        {
            ValidateArray(rc2i, 9, nameof(rc2i));

            iauC2i06a(date1, date2, rc2i);
        }

        /// <summary>
        /// C2ibpn (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauC2ibpn", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauC2ibpn(double date1,
                                         double date2,
                                         [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rbpn,
                                         [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rc2i);

        /// <summary>
        /// CIO-based bias-precession-nutation matrix (IAU 2006 precession, IAU 2000A nutation).
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="rbpn">Bias-precession-nutation matrix (row-major, length 9).</param>
        /// <param name="rc2i">Returned celestial-to-intermediate matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void C2ibpn(double date1, double date2, double[] rbpn, double[] rc2i)
        {
            ValidateArray(rbpn, 9, nameof(rbpn));
            ValidateArray(rc2i, 9, nameof(rc2i));

            iauC2ibpn(date1, date2, rbpn, rc2i);
        }

        /// <summary>
        /// C2ixy (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauC2ixy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauC2ixy(double date1, double date2, double x, double y, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rc2i);

        /// <summary>
        /// CIO coordinates from X,Y.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="x">CIP X coordinate.</param>
        /// <param name="y">CIP Y coordinate.</param>
        /// <param name="rc2i">Returned celestial-to-intermediate matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void C2ixy(double date1, double date2, double x, double y, double[] rc2i)
        {
            ValidateArray(rc2i, 9, nameof(rc2i));

            iauC2ixy(date1, date2, x, y, rc2i);
        }

        /// <summary>
        /// C2ixys (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauC2ixys", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauC2ixys(double x, double y, double s, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rc2i);

        /// <summary>
        /// CIO coordinates from CIO locator.
        /// </summary>
        /// <param name="x">CIP X coordinate.</param>
        /// <param name="y">CIP Y coordinate.</param>
        /// <param name="s">CIO locator s.</param>
        /// <param name="rc2i">Returned celestial-to-intermediate matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void C2ixys(double x, double y, double s, double[] rc2i)
        {
            ValidateArray(rc2i, 9, nameof(rc2i));

            iauC2ixys(x, y, s, rc2i);
        }

        /// <summary>
        /// C2s (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauC2s", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauC2s([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] p, ref double theta, ref double phi);

        /// <summary>
        /// Cartesian to spherical coordinates.
        /// </summary>
        /// <param name="p">Cartesian vector (length 3).</param>
        /// <param name="theta">Returned longitude angle (radians).</param>
        /// <param name="phi">Returned latitude angle (radians).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void C2s(double[] p, ref double theta, ref double phi)
        {
            ValidateArray(p, 3, nameof(p));

            iauC2s(p, ref theta, ref phi);
        }

        /// <summary>
        /// C2t00a (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauC2t00a", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauC2t00a(double tta,
                                         double ttb,
                                         double uta,
                                         double utb,
                                         double xp,
                                         double yp,
                                         [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rc2t);

        /// <summary>
        /// ICRS to ITRS matrix (IAU 2000/2006).
        /// </summary>
        /// <param name="tta">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="ttb">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="uta">UT1 as a 2-part Julian Date (part 1).</param>
        /// <param name="utb">UT1 as a 2-part Julian Date (part 2).</param>
        /// <param name="xp">Polar motion X (radians).</param>
        /// <param name="yp">Polar motion Y (radians).</param>
        /// <param name="rc2t">Returned ICRS-to-ITRS matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void C2t00a(double tta, double ttb, double uta, double utb, double xp, double yp, double[] rc2t)
        {
            ValidateArray(rc2t, 9, nameof(rc2t));

            iauC2t00a(tta, ttb, uta, utb, xp, yp, rc2t);
        }

        /// <summary>
        /// C2t00b (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauC2t00b", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauC2t00b(double tta,
                                         double ttb,
                                         double uta,
                                         double utb,
                                         double xp,
                                         double yp,
                                         [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rc2t);

        /// <summary>
        /// ICRS to ITRS matrix (IAU 2000B).
        /// </summary>
        /// <param name="tta">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="ttb">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="uta">UT1 as a 2-part Julian Date (part 1).</param>
        /// <param name="utb">UT1 as a 2-part Julian Date (part 2).</param>
        /// <param name="xp">Polar motion X (radians).</param>
        /// <param name="yp">Polar motion Y (radians).</param>
        /// <param name="rc2t">Returned ICRS-to-ITRS matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void C2t00b(double tta, double ttb, double uta, double utb, double xp, double yp, double[] rc2t)
        {
            ValidateArray(rc2t, 9, nameof(rc2t));

            iauC2t00b(tta, ttb, uta, utb, xp, yp, rc2t);
        }

        /// <summary>
        /// C2t06a (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauC2t06a", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauC2t06a(double tta,
                                         double ttb,
                                         double uta,
                                         double utb,
                                         double xp,
                                         double yp,
                                         [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rc2t);

        /// <summary>
        /// ICRS to ITRS matrix (IAU 2006/2000A).
        /// </summary>
        /// <param name="tta">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="ttb">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="uta">UT1 as a 2-part Julian Date (part 1).</param>
        /// <param name="utb">UT1 as a 2-part Julian Date (part 2).</param>
        /// <param name="xp">Polar motion X (radians).</param>
        /// <param name="yp">Polar motion Y (radians).</param>
        /// <param name="rc2t">Returned ICRS-to-ITRS matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void C2t06a(double tta, double ttb, double uta, double utb, double xp, double yp, double[] rc2t)
        {
            ValidateArray(rc2t, 9, nameof(rc2t));

            iauC2t06a(tta, ttb, uta, utb, xp, yp, rc2t);
        }

        /// <summary>
        /// C2tcio (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauC2tcio", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauC2tcio(
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rc2i,
            double era,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rpom,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rc2t);

        /// <summary>
        /// Form ICRS to ITRS matrix from CIO and polar motion.
        /// </summary>
        /// <param name="rc2i">Celestial-to-intermediate matrix (row-major, length 9).</param>
        /// <param name="era">Earth rotation angle (radians).</param>
        /// <param name="rpom">Polar motion matrix (row-major, length 9).</param>
        /// <param name="rc2t">Returned ICRS-to-ITRS matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void C2tcio(double[] rc2i, double era, double[] rpom, double[] rc2t)
        {
            ValidateArray(rc2i, 9, nameof(rc2i));
            ValidateArray(rpom, 9, nameof(rpom));
            ValidateArray(rc2t, 9, nameof(rc2t));

            iauC2tcio(rc2i, era, rpom, rc2t);
        }

        /// <summary>
        /// C2teqx (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauC2teqx", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauC2teqx(
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rbpn,
            double gst,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rpom,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rc2t);

        /// <summary>
        /// Form ICRS to ITRS matrix from bias-precession-nutation and Greenwich Apparent Sidereal Time.
        /// </summary>
        /// <param name="rbpn">Bias-precession-nutation matrix (row-major, length 9).</param>
        /// <param name="gst">Greenwich apparent sidereal time (radians).</param>
        /// <param name="rpom">Polar motion matrix (row-major, length 9).</param>
        /// <param name="rc2t">Returned ICRS-to-ITRS matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void C2teqx(double[] rbpn, double gst, double[] rpom, double[] rc2t)
        {
            ValidateArray(rbpn, 9, nameof(rbpn));
            ValidateArray(rpom, 9, nameof(rpom));
            ValidateArray(rc2t, 9, nameof(rc2t));

            iauC2teqx(rbpn, gst, rpom, rc2t);
        }

        /// <summary>
        /// C2tpe (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauC2tpe", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauC2tpe(
            double tta,
            double ttb,
            double uta,
            double utb,
            double dpsi,
            double deps,
            double xp,
            double yp,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rc2t);

        /// <summary>
        /// ICRS to ITRS matrix given nutation.
        /// </summary>
        /// <param name="tta">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="ttb">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="uta">UT1 as a 2-part Julian Date (part 1).</param>
        /// <param name="utb">UT1 as a 2-part Julian Date (part 2).</param>
        /// <param name="dpsi">Nutation in longitude (radians).</param>
        /// <param name="deps">Nutation in obliquity (radians).</param>
        /// <param name="xp">Polar motion X (radians).</param>
        /// <param name="yp">Polar motion Y (radians).</param>
        /// <param name="rc2t">Returned ICRS-to-ITRS matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void C2tpe(double tta, double ttb, double uta, double utb, double dpsi, double deps, double xp, double yp, double[] rc2t)
        {
            ValidateArray(rc2t, 9, nameof(rc2t));

            iauC2tpe(tta, ttb, uta, utb, dpsi, deps, xp, yp, rc2t);
        }

        /// <summary>
        /// C2txy (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauC2txy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauC2txy(
            double tta,
            double ttb,
            double uta,
            double utb,
            double x,
            double y,
            double xp,
            double yp,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rc2t);

        /// <summary>
        /// ICRS to ITRS matrix given CIO coordinates.
        /// </summary>
        /// <param name="tta">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="ttb">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="uta">UT1 as a 2-part Julian Date (part 1).</param>
        /// <param name="utb">UT1 as a 2-part Julian Date (part 2).</param>
        /// <param name="x">CIP X coordinate.</param>
        /// <param name="y">CIP Y coordinate.</param>
        /// <param name="xp">Polar motion X (radians).</param>
        /// <param name="yp">Polar motion Y (radians).</param>
        /// <param name="rc2t">Returned ICRS-to-ITRS matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void C2txy(double tta, double ttb, double uta, double utb, double x, double y, double xp, double yp, double[] rc2t)
        {
            ValidateArray(rc2t, 9, nameof(rc2t));

            iauC2txy(tta, ttb, uta, utb, x, y, xp, yp, rc2t);
        }

        /// <summary>
        /// Cal2jd (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauCal2jd", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauCal2jd(int iy, int im, int id, ref double djm0, ref double djm);

        /// <summary>
        /// Convert calendar date to 2-part Julian Date.
        /// </summary>
        /// <param name="iy">Year in Gregorian calendar.</param>
        /// <param name="im">Month in Gregorian calendar.</param>
        /// <param name="id">Day in Gregorian calendar.</param>
        /// <param name="djm0">Returned Julian Date zero-point (MJD convention).</param>
        /// <param name="djm">Returned Modified Julian Date for the supplied calendar date.</param>
        /// <returns>Status code: 0 = OK, &lt;0 = error.</returns>
        /// <remarks>
        /// This is a P/Invoke wrapper for the SOFA <c>iauCal2jd</c> routine.
        /// </remarks>
        /// <returns>Return value from Cal2jd</returns>
        public static int Cal2jd(int iy, int im, int id, ref double djm0, ref double djm)
        {
            return iauCal2jd(iy, im, id, ref djm0, ref djm);
        }

        /// <summary>
        /// Cp (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauCp", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauCp([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] p, [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] c);

        /// <summary>
        /// Copies the contents of a 3-element position vector to another 3-element vector.
        /// </summary>
        /// <remarks>Both arrays must have a length of 3. The method overwrites the contents of the
        /// destination array with the values from the source array. This method is a direct wrapper for the SOFA
        /// library function 'iauCp'.</remarks>
        /// <param name="p">The source array containing the position vector to copy. Must be a double array of length 3.</param>
        /// <param name="c">The destination array that receives the copied vector. Must be a double array of length 3.</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Cp(double[] p, double[] c)
        {
            ValidateArray(p, 3, nameof(p));
            ValidateArray(c, 3, nameof(c));

            iauCp(p, c);
        }

        /// <summary>
        /// Cpv (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauCpv", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauCpv([MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] pv, [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] c);

        /// <summary>
        /// Copy a position vector.
        /// </summary>
        /// <param name="pv">Source position-velocity vector (length 6).</param>
        /// <param name="c">Destination position-velocity vector (length 6).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Cpv(double[] pv, double[] c)
        {
            ValidateArray(pv, 6, nameof(pv));
            ValidateArray(c, 6, nameof(c));

            iauCpv(pv, c);
        }

        /// <summary>
        /// Cr (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauCr", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauCr([MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] r, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] c);

        /// <summary>
        /// Copies the elements of a 3×3 rotation matrix from one array to another.
        /// </summary>
        /// <remarks>This method does not perform validation on the input arrays. Both arrays must be
        /// pre-allocated with exactly 9 elements. The method overwrites the contents of the destination array with the
        /// values from the source array.</remarks>
        /// <param name="r">An array of 9 elements representing the source 3×3 rotation matrix in row-major order. Must not be null and
        /// must have a length of 9.</param>
        /// <param name="c">An array of 9 elements that receives the copied rotation matrix. Must not be null and must have a length of
        /// 9.</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Cr(double[] r, double[] c)
        {
            ValidateArray(r, 9, nameof(r));
            ValidateArray(c, 9, nameof(c));

            iauCr(r, c);
        }

        /// <summary>
        /// D2dtf (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauD2dtf", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauD2dtf([MarshalAs(UnmanagedType.LPStr)] string scale, int ndp, double d1, double d2, ref int iy, ref int im, ref int id, int[] ihmsf);

        /// <summary>
        /// Format a 2-part Julian Date into Gregorian calendar date and time fields.
        /// </summary>
        /// <remarks>
        /// If <paramref name="scale"/> is "UTC", this routine applies the SOFA quasi-JD convention that handles leap seconds.
        /// </remarks>
        /// <param name="scale">Time scale ID (only "UTC" is significant for leap-second handling).</param>
        /// <param name="ndp">Number of decimal places in the seconds field (can be negative for coarse rounding).</param>
        /// <param name="d1">First part of the date as a 2-part Julian Date.</param>
        /// <param name="d2">Second part of the date as a 2-part Julian Date.</param>
        /// <param name="iy">Returned year in the Gregorian calendar.</param>
        /// <param name="im">Returned month in the Gregorian calendar.</param>
        /// <param name="id">Returned day in the Gregorian calendar.</param>
        /// <param name="ihmsf">Returned time fields: hours, minutes, seconds, fraction.</param>
        /// <returns>Status: +1 = dubious year, 0 = OK, -1 = unacceptable date.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        /// <returns>Return value from D2dtf</returns>
        public static int D2dtf(string scale, int ndp, double d1, double d2, ref int iy, ref int im, ref int id, int[] ihmsf)
        {
            ValidateString(scale, 0, nameof(scale));
            return iauD2dtf(scale, ndp, d1, d2, ref iy, ref im, ref id, ihmsf);
        }

        /// <summary>
        /// D2tf (P/Invoke the SOFA library).
        /// </summary>
        [DllImport("libsofa", EntryPoint = "iauD2tf", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauD2tf(
            int ndp,
            double days,
            out byte sign, // '+' or '-'
            [Out, MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] int[] ihmsf // length 4: h,m,s,fraction
        );

        /// <summary>
        /// Decompose days into hours, minutes, seconds, fraction.
        /// </summary>
        /// <param name="ndp">Number of decimal places in the seconds field.</param>
        /// <param name="days">Interval in days.</param>
        /// <param name="sign">Returned sign ('+' or '-').</param>
        /// <param name="ihmsf">Returned fields (length 4): hours, minutes, seconds, fraction.</param>
        /// <remarks>
        /// The sign is returned as a byte representing the ASCII character '+' (0x2B) or '-' (0x2D). 
        /// The returned value can be converted to a char for comparison or display by casting it to char.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void D2tf(int ndp, double days, out char sign, int[] ihmsf)
        {
            ValidateArray(ihmsf, 4, nameof(ihmsf));

            iauD2tf(ndp, days, out byte signByte, ihmsf);
            sign=Convert.ToChar(signByte);
        }

        /// <summary>
        /// Dat (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauDat", CallingConvention = CallingConvention.Cdecl)]
        private static extern short iauDat(int Year, int Month, int Day, double DayFraction, ref double ReturnedLeapSeconds);

        /// <summary>
        /// For a given UTC date, calculate Delta(AT) = TAI−UTC  (number of leap seconds).
        /// </summary>
        /// <param name="Year">Year</param>
        /// <param name="Month">Month</param>
        /// <param name="Day">Day</param>
        /// <param name="DayFraction">Fraction of a day</param>
        /// <param name="ReturnedLeapSeconds">Out: Leap seconds</param>
        /// <returns>status: 1 = dubious year, 0 = OK, −1 = bad year, −2 = bad month, −3 = bad day, −4 = bad fraction, −5 = internal error</returns>
        /// <returns>Return value from Dat</returns>
        public static short Dat(int Year, int Month, int Day, double DayFraction, ref double ReturnedLeapSeconds)
        {
            return iauDat(Year, Month, Day, DayFraction, ref ReturnedLeapSeconds);
        }

        /// <summary>
        /// Dtdb (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauDtdb", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauDtdb(double date1, double date2, double ut, double elong, double u, double v);

        /// <summary>
        /// Approximate TDB−TT for an observer on the Earth.
        /// </summary>
        /// <remarks>
        /// This is a model of the quasi-periodic part of the TT/TCB relationship, dominated by an annual term (~1.7 ms).
        /// Providing zero for <paramref name="u"/> and <paramref name="v"/> removes the topocentric contribution.
        /// </remarks>
        /// <param name="date1">First part of the date as a 2-part Julian Date (TDB; TT is acceptable in practice).</param>
        /// <param name="date2">Second part of the date as a 2-part Julian Date (TDB; TT is acceptable in practice).</param>
        /// <param name="ut">Universal time UT1 as a fraction of one day.</param>
        /// <param name="elong">Observer longitude (east positive, radians).</param>
        /// <param name="u">Distance from Earth spin axis (km).</param>
        /// <param name="v">Distance north of the equatorial plane (km).</param>
        /// <returns>TDB−TT in seconds.</returns>
        /// <returns>Return value from Dtdb</returns>
        public static double Dtdb(double date1, double date2, double ut, double elong, double u, double v)
        {
            return iauDtdb(date1, date2, ut, elong, u, v);
        }

        /// <summary>
        /// Dtf2d (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauDtf2d", CallingConvention = CallingConvention.Cdecl)]
        private static extern short iauDtf2d([MarshalAs(UnmanagedType.LPStr)] string scale, int iy, int im, int id, int ihr, int imn, double sec, ref double d1, ref double d2);

        /// <summary>
        /// Encode date and time fields into 2-part Julian Date (or in the case of UTC a quasi-JD form that includes special provision for leap seconds).
        /// </summary>
        /// <param name="scale">Time scale ID (Note 1)</param>
        /// <param name="iy">Year in Gregorian calendar (Note 2)</param>
        /// <param name="im">Month in Gregorian calendar (Note 2)</param>
        /// <param name="id">Day in Gregorian calendar (Note 2)</param>
        /// <param name="ihr">Hour</param>
        /// <param name="imn">Minute</param>
        /// <param name="sec">Seconds</param>
        /// <param name="d1">2-part Julian Date (Notes 3, 4)</param>
        /// <param name="d2">2-part Julian Date (Notes 3, 4)</param>
        /// <returns>Status: +3 = both of next two, +2 = time is after end of day (Note 5), +1 = dubious year (Note 6), 0 = OK, -1 = bad year, -2 = bad month, -3 = bad day, -4 = bad hour, -5 = bad minute, -6 = bad second (&lt;0)</returns>
        /// <remarks>
        /// Notes:
        /// <list type="number">
        /// <item><description>Scale identifies the time scale.  Only the value "UTC" (in upper case) is significant, and enables handling of leap seconds (see Note 4).</description></item>
        /// <item><description>For calendar conventions and limitations, see iauCal2jd.</description></item>
        /// <item><description>The sum of the results, d1+d2, is Julian Date, where normally d1 is the Julian Day Number and d2 is the fraction of a day.  In the case of UTC, where the use of JD is problematical, special conventions apply:  see the next note.</description></item>
        /// <item><description>JD cannot unambiguously represent UTC during a leap second unless special measures are taken.  The SOFA internal convention is that the quasi-JD day represents UTC days whether the length is 86399,
        /// 86400 or 86401 SI seconds.  In the 1960-1972 era there were smaller jumps (in either direction) each time the linear UTC(TAI) expression was changed, and these "mini-leaps" are also included in the SOFA convention.</description></item>
        /// <item><description>The warning status "time is after end of day" usually means that the sec argument is greater than 60.0.  However, in a day ending in a leap second the limit changes to 61.0 (or 59.0 in the case of a negative leap second).</description></item>
        /// <item><description>The warning status "dubious year" flags UTCs that predate the introduction of the time scale or that are too far in the future to be trusted.  See iauDat for further details.</description></item>
        /// <item><description>Only in the case of continuous and regular time scales (TAI, TT, TCG, TCB and TDB) is the result d1+d2 a Julian Date, strictly speaking.  In the other cases (UT1 and UTC) the result must be
        /// used with circumspection;  in particular the difference between two such results cannot be interpreted as a precise time interval.</description></item>
        /// </list>
        /// </remarks>
        /// <returns>Return value from Dtf2d</returns>
        public static short Dtf2d(string scale, int iy, int im, int id, int ihr, int imn, double sec, ref double d1, ref double d2)
        {
            ValidateString(scale, 0, nameof(scale));
            return iauDtf2d(scale, iy, im, id, ihr, imn, sec, ref d1, ref d2);
        }

        /// <summary>
        /// Eceq06 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauEceq06", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauEceq06(double date1, double date2, double dl, double db, ref double dr, ref double dd);

        /// <summary>
        /// Transform ecliptic to equatorial coordinates.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="dl">Ecliptic longitude (radians).</param>
        /// <param name="db">Ecliptic latitude (radians).</param>
        /// <param name="dr">Returned right ascension (radians).</param>
        /// <param name="dd">Returned declination (radians).</param>
        public static void Eceq06(double date1, double date2, double dl, double db, ref double dr, ref double dd)
        {
            iauEceq06(date1, date2, dl, db, ref dr, ref dd);
        }

        /// <summary>
        /// Ecm06 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauEcm06", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauEcm06(double date1, double date2, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rm);

        /// <summary>
        /// Ecliptic to equatorial matrix (IAU 2006).
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="rm">Returned rotation matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Ecm06(double date1, double date2, double[] rm)
        {
            ValidateArray(rm, 9, nameof(rm));

            iauEcm06(date1, date2, rm);
        }

        /// <summary>
        /// Ee00 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauEe00", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauEe00(double date1, double date2, double epsa, double dpsi);

        /// <summary>
        /// Equation of the Equinoxes, IAU 2000 model.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="epsa">Mean obliquity (radians).</param>
        /// <param name="dpsi">Nutation in longitude (radians).</param>
        /// <returns>Equation of the equinoxes in radians.</returns>
        /// <returns>Return value from Ee00</returns>
        public static double Ee00(double date1, double date2, double epsa, double dpsi)
        {
            return iauEe00(date1, date2, epsa, dpsi);
        }

        /// <summary>
        /// Ee00a (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauEe00a", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauEe00a(double date1, double date2);

        /// <summary>
        /// Equation of the Equinoxes, IAU 2000 model.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <returns>Equation of the equinoxes in radians.</returns>
        /// <returns>Return value from Ee00a</returns>
        public static double Ee00a(double date1, double date2)
        {
            return iauEe00a(date1, date2);
        }

        /// <summary>
        /// Ee00b (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauEe00b", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauEe00b(double date1, double date2);

        /// <summary>
        /// Equation of the Equinoxes, IAU 2000B model.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <returns>Equation of the equinoxes in radians.</returns>
        /// <returns>Return value from Ee00b</returns>
        public static double Ee00b(double date1, double date2)
        {
            return iauEe00b(date1, date2);
        }

        /// <summary>
        /// Ee06a (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauEe06a", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauEe06a(double date1, double date2);

        /// <summary>
        /// Equation of the Equinoxes, IAU 2000B model.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <returns>Equation of the equinoxes in radians.</returns>
        /// <returns>Return value from Ee06a</returns>
        public static double Ee06a(double date1, double date2)
        {
            return iauEe06a(date1, date2);
        }

        /// <summary>
        /// Eect00 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauEect00", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauEect00(double date1, double date2);

        /// <summary>
        /// Equation of the Equinoxes Complement, IAU 2000.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <returns>Equation of the equinoxes complement in radians.</returns>
        /// <returns>Return value from Eect00</returns>
        public static double Eect00(double date1, double date2)
        {
            return iauEect00(date1, date2);
        }

        /// <summary>
        /// Eform (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauEform", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauEform(SofaReferenceEllipsoids n, ref double a, ref double f);

        /// <summary>
        /// Reference ellipsoid parameters.
        /// </summary>
        /// <param name="n">Reference ellipsoid identifier.</param>
        /// <param name="a">Returned equatorial radius (meters).</param>
        /// <param name="f">Returned flattening.</param>
        /// <returns>Status code: 0 = OK, &lt;0 indicates an error condition.</returns>
        /// <returns>Return value from Eform</returns>
        public static int Eform(SofaReferenceEllipsoids n, ref double a, ref double f)
        {
            return iauEform(n, ref a, ref f);
        }

        /// <summary>
        /// Eo06a (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauEo06a", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauEo06a(double date1, double date2);

        /// <summary>
        /// Equation of the origins, IAU 2006 precession and IAU 2000A nutation.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (Note 1)</param>
        /// <param name="date2">TT as a 2-part Julian Date (Note 1)</param>
        /// <returns>Equation of the origins in radians (Note 2)</returns>
        /// <remarks>
        /// Notes:
        /// <list type="number">
        /// <item><description> The TT date date1+date2 is a Julian Date, apportioned in any convenient way between the two arguments.  For example, JD(TT)=2450123.7 could be expressed in any of these ways, among others:
        /// <table style="width:340px;" cellspacing="0">
        /// <col style="width:80px;"></col>
        /// <col style="width:80px;"></col>
        /// <col style="width:180px;"></col>
        /// <tr>
        /// <td colspan="1" align="center" rowspan="1" style="width: 80px; padding-right: 10px; padding-left: 10px; 
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-top-color: #000000; border-top-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid;
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; 
        /// background-color: #00ffff;" width="110px">
        /// <b>Date 1</b></td>
        /// <td colspan="1" rowspan="1" align="center" style="width: 80px; padding-right: 10px; padding-left: 10px; 
        /// border-top-color: #000000; border-top-style: Solid; 
        /// border-right-style: Solid; border-right-color: #000000; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; 
        /// background-color: #00ffff;" width="110px">
        /// <b>Date 2</b></td>
        /// <td colspan="1" rowspan="1" align="center" style="width: 180px; padding-right: 10px; padding-left: 10px; 
        /// border-top-color: #000000; border-top-style: Solid; 
        /// border-right-style: Solid; border-right-color: #000000; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; 
        /// background-color: #00ffff;" width="220px">
        /// <b>Method</b></td>
        /// </tr>
        /// <tr>
        /// <td align="right" style="padding-right: 10px; padding-left: 10px; 
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        ///  2450123.7</td>
        /// <td align="right" style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 0.0</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// JD method</td>
        /// </tr>
        /// <tr>
        /// <td align="right" style="padding-right: 10px; padding-left: 10px; 
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 2451545.0</td>
        /// <td align="right" style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// -1421.3</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// J2000 method</td>
        /// </tr>
        /// <tr>
        /// <td align="right" style="padding-right: 10px; padding-left: 10px; 
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 2400000.5</td>
        /// <td align="right" style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 50123.2</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// MJD method</td>
        /// </tr>
        /// <tr>
        /// <td align="right" style="padding-right: 10px; padding-left: 10px; 
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 2450123.5</td>
        /// <td align="right" style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 0.2</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Date and time method</td>
        /// </tr>
        /// </table>
        /// <para>The JD method is the most natural and convenient to use in cases where the loss of several decimal digits of resolution is acceptable.  The J2000 method is best matched to the way the argument is handled internally 
        /// and will deliver the optimum resolution.  The MJD method and the date and time methods are both good compromises between resolution and convenience.  For most applications of this function the choice will not be at all critical.</para>
        /// </description></item>
        /// <item><description> The equation of the origins is the distance between the true equinox and the celestial intermediate origin and, equivalently, the difference between Earth rotation angle and Greenwich
        /// apparent sidereal time (ERA-GST).  It comprises the precession (since J2000.0) in right ascension plus the equation of the equinoxes (including the small correction terms).</description></item>
        /// </list>
        /// </remarks>
        /// <returns>Return value from Eo06a</returns>
        public static double Eo06a(double date1, double date2)
        {
            return iauEo06a(date1, date2);
        }

        /// <summary>
        /// Eors (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauEors", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauEors(
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rnpb,
            double s);

        /// <summary>
        /// Equation of the origins given nutation matrix.
        /// </summary>
        /// <param name="rnpb">Bias-precession-nutation matrix (row-major, length 9).</param>
        /// <param name="s">CIO locator.</param>
        /// <returns>Equation of the origins in radians.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        /// <returns>Return value from Eors</returns>
        public static double Eors(double[] rnpb, double s)
        {
            ValidateArray(rnpb, 9, nameof(rnpb));

            return iauEors(rnpb, s);
        }

        /// <summary>
        /// Epb (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauEpb", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauEpb(double dj1, double dj2);

        /// <summary>
        /// Return Besselian epoch for given JD pair.
        /// </summary>
        /// <param name="dj1">First part of the Julian Date.</param>
        /// <param name="dj2">Second part of the Julian Date.</param>
        /// <remarks>
        /// This is a P/Invoke wrapper for the SOFA <c>iauEpb</c> routine.
        /// </remarks>
        /// <returns>Besselian epoch.</returns>
        /// <returns>Return value from Epb</returns>
        public static double Epb(double dj1, double dj2)
        {
            return iauEpb(dj1, dj2);
        }

        /// <summary>
        /// Epb2jd (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauEpb2jd", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauEpb2jd(double epb, ref double djm0, ref double djm);

        /// <summary>
        /// Split Besselian epoch into JD pair.
        /// </summary>
        /// <param name="epb">Besselian epoch.</param>
        /// <param name="djm0">Returned Julian Date zero-point (MJD convention).</param>
        /// <param name="djm">Returned Modified Julian Date corresponding to <paramref name="epb"/>.</param>
        /// <remarks>
        /// This is a P/Invoke wrapper for the SOFA <c>iauEpb2jd</c> routine.
        /// </remarks>
        public static void Epb2jd(double epb, ref double djm0, ref double djm)
        {
            iauEpb2jd(epb, ref djm0, ref djm);
        }

        /// <summary>
        /// Epj (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauEpj", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauEpj(double dj1, double dj2);

        /// <summary>
        /// Return Julian epoch for given JD pair.
        /// </summary>
        /// <param name="dj1">First part of the Julian Date.</param>
        /// <param name="dj2">Second part of the Julian Date.</param>
        /// <remarks>
        /// This is a P/Invoke wrapper for the SOFA <c>iauEpj</c> routine.
        /// </remarks>
        /// <returns>Julian epoch.</returns>
        /// <returns>Return value from Epj</returns>
        public static double Epj(double dj1, double dj2)
        {
            return iauEpj(dj1, dj2);
        }

        /// <summary>
        /// Epj2jd (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauEpj2jd", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauEpj2jd(double epj, ref double djm0, ref double djm);

        /// <summary>
        /// Split Julian epoch into JD pair.
        /// </summary>
        /// <param name="epj">Julian epoch.</param>
        /// <param name="djm0">Returned Julian Date zero-point (MJD convention).</param>
        /// <param name="djm">Returned Modified Julian Date corresponding to <paramref name="epj"/>.</param>
        /// <remarks>
        /// This is a P/Invoke wrapper for the SOFA <c>iauEpj2jd</c> routine.
        /// </remarks>
        public static void Epj2jd(double epj, ref double djm0, ref double djm)
        {
            iauEpj2jd(epj, ref djm0, ref djm);
        }

        /// <summary>
        /// Epv00 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauEpv00", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauEpv00(double date1,
                                       double date2,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] pvh,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] pvb);

        /// <summary>
        /// Earth position and velocity (heliocentric and barycentric).
        /// </summary>
        /// <param name="date1">TDB as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TDB as a 2-part Julian Date (part 2).</param>
        /// <param name="pvh">Returned heliocentric Earth position/velocity (length 6).</param>
        /// <param name="pvb">Returned barycentric Earth position/velocity (length 6).</param>
        /// <returns>Status code: 0 = OK, &lt;0 indicates an error condition.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        /// <returns>Return value from Epv00</returns>
        public static int Epv00(double date1, double date2, double[] pvh, double[] pvb)
        {
            ValidateArray(pvh, 6, nameof(pvh));
            ValidateArray(pvb, 6, nameof(pvb));

            return iauEpv00(date1, date2, pvh, pvb);
        }

        /// <summary>
        /// Eqec06 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauEqec06", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauEqec06(double date1, double date2, double dr, double dd, ref double dl, ref double db);

        /// <summary>
        /// Transform equatorial to ecliptic coordinates.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="dr">Right ascension (radians).</param>
        /// <param name="dd">Declination (radians).</param>
        /// <param name="dl">Returned ecliptic longitude (radians).</param>
        /// <param name="db">Returned ecliptic latitude (radians).</param>
        public static void Eqec06(double date1, double date2, double dr, double dd, ref double dl, ref double db)
        {
            iauEqec06(date1, date2, dr, dd, ref dl, ref db);
        }

        /// <summary>
        /// Eqeq94 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauEqeq94", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauEqeq94(double date1, double date2);

        /// <summary>
        /// Equation of the Equinoxes, IAU 1994 model.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <returns>Equation of the equinoxes in radians.</returns>
        /// <returns>Return value from Eqeq94</returns>
        public static double Eqeq94(double date1, double date2)
        {
            return iauEqeq94(date1, date2);
        }

        /// <summary>
        /// Era00 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauEra00", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauEra00(double dj1, double dj2);

        /// <summary>
        ///  Earth rotation angle (IAU 2000 model)
        /// </summary>
        /// <param name="dj1">Julian date component 1</param>
        /// <param name="dj2">Julian date component 2</param>
        /// <returns>Earth rotation angle in radians.</returns>
        /// <returns>Return value from Era00</returns>
        public static double Era00(double dj1, double dj2)
        {
            return iauEra00(dj1, dj2);
        }

        /// <summary>
        /// Fad03 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauFad03", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauFad03(double t);

        /// <summary>
        /// Mean elongation of the Moon from the Sun (IERS Conventions 2003).
        /// </summary>
        /// <param name="t">TDB, Julian centuries since J2000.0. Note: It is usually more convenient to use TT, which makes no significant difference.</param>
        /// <returns>Angle in radians.</returns>
        /// <returns>Return value from Fad03</returns>
        public static double Fad03(double t)
        {
            return iauFad03(t);
        }

        /// <summary>
        /// Fae03 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauFae03", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauFae03(double t);

        /// <summary>
        /// Mean longitude of Earth (IERS Conventions 2003).
        /// </summary>
        /// <param name="t">TDB, Julian centuries since J2000.0. Note: It is usually more convenient to use TT, which makes no significant difference.</param>
        /// <returns>Angle in radians.</returns>
        /// <returns>Return value from Fae03</returns>
        public static double Fae03(double t)
        {
            return iauFae03(t);
        }

        /// <summary>
        /// Faf03 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauFaf03", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauFaf03(double t);

        /// <summary>
        /// Mean longitude of the Moon minus that of the ascending node (IERS Conventions 2003).
        /// </summary>
        /// <param name="t">TDB, Julian centuries since J2000.0. Note: It is usually more convenient to use TT, which makes no significant difference.</param>
        /// <returns>Angle in radians.</returns>
        /// <returns>Return value from Faf03</returns>
        public static double Faf03(double t)
        {
            return iauFaf03(t);
        }

        /// <summary>
        /// Faju03 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauFaju03", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauFaju03(double t);

        /// <summary>
        /// Mean longitude of Jupiter (IERS Conventions 2003).
        /// </summary>
        /// <param name="t">TDB, Julian centuries since J2000.0. Note: It is usually more convenient to use TT, which makes no significant difference.</param>
        /// <returns>Angle in radians.</returns>
        /// <returns>Return value from Faju03</returns>
        public static double Faju03(double t)
        {
            return iauFaju03(t);
        }

        /// <summary>
        /// Fal03 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauFal03", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauFal03(double t);

        /// <summary>
        /// Mean anomaly of the Moon (IERS Conventions 2003).
        /// </summary>
        /// <param name="t">TDB, Julian centuries since J2000.0. Note: It is usually more convenient to use TT, which makes no significant difference.</param>
        /// <returns>Angle in radians.</returns>
        /// <returns>Return value from Fal03</returns>
        public static double Fal03(double t)
        {
            return iauFal03(t);
        }

        /// <summary>
        /// Falp03 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauFalp03", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauFalp03(double t);

        /// <summary>
        /// Mean anomaly of the Sun (IERS Conventions 2003).
        /// </summary>
        /// <param name="t">TDB, Julian centuries since J2000.0. Note: It is usually more convenient to use TT, which makes no significant difference.</param>
        /// <returns>Angle in radians.</returns>
        /// <returns>Return value from Falp03</returns>
        public static double Falp03(double t)
        {
            return iauFalp03(t);
        }

        /// <summary>
        /// Fama03 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauFama03", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauFama03(double t);

        /// <summary>
        /// Mean longitude of Mars (IERS Conventions 2003).
        /// </summary>
        /// <param name="t">TDB, Julian centuries since J2000.0. Note: It is usually more convenient to use TT, which makes no significant difference.</param>
        /// <returns>Angle in radians.</returns>
        /// <returns>Return value from Fama03</returns>
        public static double Fama03(double t)
        {
            return iauFama03(t);
        }

        /// <summary>
        /// Fame03 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauFame03", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauFame03(double t);

        /// <summary>
        /// Mean longitude of Mercury (IERS Conventions 2003).
        /// </summary>
        /// <param name="t">TDB, Julian centuries since J2000.0. Note: It is usually more convenient to use TT, which makes no significant difference.</param>
        /// <returns>Angle in radians.</returns>
        /// <returns>Return value from Fame03</returns>
        public static double Fame03(double t)
        {
            return iauFame03(t);
        }

        /// <summary>
        /// Fane03 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauFane03", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauFane03(double t);

        /// <summary>
        /// Mean longitude of Neptune (IERS Conventions 2003).
        /// </summary>
        /// <param name="t">TDB, Julian centuries since J2000.0. Note: It is usually more convenient to use TT, which makes no significant difference.</param>
        /// <returns>Angle in radians.</returns>
        /// <returns>Return value from Fane03</returns>
        public static double Fane03(double t)
        {
            return iauFane03(t);
        }

        /// <summary>
        /// Faom03 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauFaom03", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauFaom03(double t);

        /// <summary>
        /// Mean longitude of the Moon's ascending node (IERS Conventions 2003).                   
        /// </summary>
        /// <param name="t">TDB, Julian centuries since J2000.0. Note: It is usually more convenient to use TT, which makes no significant difference.</param>
        /// <returns>Angle in radians.</returns>
        /// <returns>Return value from Faom03</returns>
        public static double Faom03(double t)
        {
            return iauFaom03(t);
        }

        /// <summary>
        /// Fapa03 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauFapa03", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauFapa03(double t);

        /// <summary>
        /// General accumulated precession in longitude.
        /// </summary>
        /// <param name="t">TDB, Julian centuries since J2000.0. Note: It is usually more convenient to use TT, which makes no significant difference.</param>
        /// <returns>Angle in radians.</returns>
        /// <returns>Return value from Fapa03</returns>
        public static double Fapa03(double t)
        {
            return iauFapa03(t);
        }

        /// <summary>
        /// Fasa03 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauFasa03", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauFasa03(double t);

        /// <summary>
        /// Mean longitude of Saturn (IERS Conventions 2003).
        /// </summary>
        /// <param name="t">TDB, Julian centuries since J2000.0. Note: It is usually more convenient to use TT, which makes no significant difference.</param>
        /// <returns>Angle in radians.</returns>
        /// <returns>Return value from Fasa03</returns>
        public static double Fasa03(double t)
        {
            return iauFasa03(t);
        }

        /// <summary>
        /// Faur03 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauFaur03", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauFaur03(double t);

        /// <summary>
        /// Mean longitude of Uranus (IERS Conventions 2003).
        /// </summary>
        /// <param name="t">TDB, Julian centuries since J2000.0. Note: It is usually more convenient to use TT, which makes no significant difference.</param>
        /// <returns>Angle in radians.</returns>
        /// <returns>Return value from Faur03</returns>
        public static double Faur03(double t)
        {
            return iauFaur03(t);
        }

        /// <summary>
        /// Fave03 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauFave03", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauFave03(double t);

        /// <summary>
        /// Mean longitude of Venus (IERS Conventions 2003).
        /// </summary>
        /// <param name="t">TDB, Julian centuries since J2000.0. Note: It is usually more convenient to use TT, which makes no significant difference.</param>
        /// <returns>Angle in radians.</returns>
        /// <returns>Return value from Fave03</returns>
        public static double Fave03(double t)
        {
            return iauFave03(t);
        }

        /// <summary>
        /// Fk425 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauFk425", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauFk425(
            double r1950,
            double d1950,
            double dr1950,
            double dd1950,
            double p1950,
            double v1950,
            ref double r2000,
            ref double d2000,
            ref double dr2000,
            ref double dd2000,
            ref double p2000,
            ref double v2000);

        /// <summary>
        /// Transform between FK4 and FK5 star catalog systems.
        /// </summary>
        /// <param name="r1950">FK4 right ascension (radians).</param>
        /// <param name="d1950">FK4 declination (radians).</param>
        /// <param name="dr1950">FK4 proper motion in RA (radians/year).</param>
        /// <param name="dd1950">FK4 proper motion in Dec (radians/year).</param>
        /// <param name="p1950">FK4 parallax (arcsec).</param>
        /// <param name="v1950">FK4 radial velocity (km/s).</param>
        /// <param name="r2000">Returned FK5 right ascension (radians).</param>
        /// <param name="d2000">Returned FK5 declination (radians).</param>
        /// <param name="dr2000">Returned FK5 proper motion in RA (radians/year).</param>
        /// <param name="dd2000">Returned FK5 proper motion in Dec (radians/year).</param>
        /// <param name="p2000">Returned FK5 parallax (arcsec).</param>
        /// <param name="v2000">Returned FK5 radial velocity (km/s).</param>
        public static void Fk425(double r1950, double d1950, double dr1950, double dd1950, double p1950, double v1950, ref double r2000, ref double d2000, ref double dr2000, ref double dd2000, ref double p2000, ref double v2000)
        {
            iauFk425(r1950, d1950, dr1950, dd1950, p1950, v1950, ref r2000, ref d2000, ref dr2000, ref dd2000, ref p2000, ref v2000);
        }

        /// <summary>
        /// Fk45z (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauFk45z", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauFk45z(double r1950, double d1950, double bepoch, ref double r2000, ref double d2000);

        /// <summary>
        /// Transform from FK4 to FK5 (catalog).
        /// </summary>
        /// <param name="r1950">FK4 right ascension (radians).</param>
        /// <param name="d1950">FK4 declination (radians).</param>
        /// <param name="bepoch">Besselian epoch.</param>
        /// <param name="r2000">Returned FK5 right ascension (radians).</param>
        /// <param name="d2000">Returned FK5 declination (radians).</param>
        public static void Fk45z(double r1950, double d1950, double bepoch, ref double r2000, ref double d2000)
        {
            iauFk45z(r1950, d1950, bepoch, ref r2000, ref d2000);
        }

        /// <summary>
        /// Fk524 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauFk524", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauFk524(
            double r2000,
            double d2000,
            double dr2000,
            double dd2000,
            double p2000,
            double v2000,
            ref double r1950,
            ref double d1950,
            ref double dr1950,
            ref double dd1950,
            ref double p1950,
            ref double v1950);

        /// <summary>
        /// Transform between FK5 and FK4 star catalog systems.
        /// </summary>
        /// <param name="r2000">FK5 right ascension (radians).</param>
        /// <param name="d2000">FK5 declination (radians).</param>
        /// <param name="dr2000">FK5 proper motion in RA (radians/year).</param>
        /// <param name="dd2000">FK5 proper motion in Dec (radians/year).</param>
        /// <param name="p2000">FK5 parallax (arcsec).</param>
        /// <param name="v2000">FK5 radial velocity (km/s).</param>
        /// <param name="r1950">Returned FK4 right ascension (radians).</param>
        /// <param name="d1950">Returned FK4 declination (radians).</param>
        /// <param name="dr1950">Returned FK4 proper motion in RA (radians/year).</param>
        /// <param name="dd1950">Returned FK4 proper motion in Dec (radians/year).</param>
        /// <param name="p1950">Returned FK4 parallax (arcsec).</param>
        /// <param name="v1950">Returned FK4 radial velocity (km/s).</param>
        public static void Fk524(double r2000, double d2000, double dr2000, double dd2000, double p2000, double v2000, ref double r1950, ref double d1950, ref double dr1950, ref double dd1950, ref double p1950, ref double v1950)
        {
            iauFk524(r2000, d2000, dr2000, dd2000, p2000, v2000, ref r1950, ref d1950, ref dr1950, ref dd1950, ref p1950, ref v1950);
        }

        /// <summary>
        /// Fk52h (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauFk52h", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauFk52h(
            double r5,
            double d5,
            double dr5,
            double dd5,
            double px5,
            double rv5,
            ref double rh,
            ref double dh,
            ref double drh,
            ref double ddh,
            ref double pxh,
            ref double rvh);

        /// <summary>
        /// Transform from FK5 (J2000.0) to Hipparcos.
        /// </summary>
        /// <param name="r5">FK5 right ascension (radians).</param>
        /// <param name="d5">FK5 declination (radians).</param>
        /// <param name="dr5">FK5 proper motion in RA (radians/year).</param>
        /// <param name="dd5">FK5 proper motion in Dec (radians/year).</param>
        /// <param name="px5">FK5 parallax (arcsec).</param>
        /// <param name="rv5">FK5 radial velocity (km/s).</param>
        /// <param name="rh">Returned Hipparcos right ascension (radians).</param>
        /// <param name="dh">Returned Hipparcos declination (radians).</param>
        /// <param name="drh">Returned Hipparcos proper motion in RA (radians/year).</param>
        /// <param name="ddh">Returned Hipparcos proper motion in Dec (radians/year).</param>
        /// <param name="pxh">Returned Hipparcos parallax (arcsec).</param>
        /// <param name="rvh">Returned Hipparcos radial velocity (km/s).</param>
        public static void Fk52h(double r5, double d5, double dr5, double dd5, double px5, double rv5, ref double rh, ref double dh, ref double drh, ref double ddh, ref double pxh, ref double rvh)
        {
            iauFk52h(r5, d5, dr5, dd5, px5, rv5, ref rh, ref dh, ref drh, ref ddh, ref pxh, ref rvh);
        }

        /// <summary>
        /// Fk54z (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauFk54z", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauFk54z(double r2000, double d2000, double bepoch, ref double r1950, ref double d1950, ref double dr1950, ref double dd1950);

        /// <summary>
        /// Transform from FK5 to FK4 (catalog).
        /// </summary>
        /// <param name="r2000">FK5 right ascension (radians).</param>
        /// <param name="d2000">FK5 declination (radians).</param>
        /// <param name="bepoch">Besselian epoch.</param>
        /// <param name="r1950">Returned FK4 right ascension (radians).</param>
        /// <param name="d1950">Returned FK4 declination (radians).</param>
        /// <param name="dr1950">Returned FK4 proper motion in RA (radians/year).</param>
        /// <param name="dd1950">Returned FK4 proper motion in Dec (radians/year).</param>
        public static void Fk54z(double r2000, double d2000, double bepoch, ref double r1950, ref double d1950, ref double dr1950, ref double dd1950)
        {
            iauFk54z(r2000, d2000, bepoch, ref r1950, ref d1950, ref dr1950, ref dd1950);
        }

        /// <summary>
        /// Fk5hip (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauFk5hip", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauFk5hip([MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] r5h, [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] s5h);

        /// <summary>
        /// FK5 to Hipparcos rotation matrix.
        /// </summary>
        /// <param name="r5h">Returned FK5-to-Hipparcos rotation matrix (row-major, length 9).</param>
        /// <param name="s5h">Returned FK5-to-Hipparcos spin vector (length 3).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Fk5hip(double[] r5h, double[] s5h)
        {
            ValidateArray(r5h, 9, nameof(r5h));
            ValidateArray(s5h, 3, nameof(s5h));

            iauFk5hip(r5h, s5h);
        }

        /// <summary>
        /// Fk5hz (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauFk5hz", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauFk5hz(double r5, double d5, double date1, double date2, ref double rh, ref double dh);

        /// <summary>
        /// Transform from FK5 to Hipparcos (catalog).
        /// </summary>
        /// <param name="r5">FK5 right ascension (radians).</param>
        /// <param name="d5">FK5 declination (radians).</param>
        /// <param name="date1">TDB as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TDB as a 2-part Julian Date (part 2).</param>
        /// <param name="rh">Returned Hipparcos right ascension (radians).</param>
        /// <param name="dh">Returned Hipparcos declination (radians).</param>
        public static void Fk5hz(double r5, double d5, double date1, double date2, ref double rh, ref double dh)
        {
            iauFk5hz(r5, d5, date1, date2, ref rh, ref dh);
        }

        /// <summary>
        /// Fw2m (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauFw2m", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauFw2m(
            double gamb,
            double phib,
            double psi,
            double eps,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] r);

        /// <summary>
        /// Frame Tie and precession, IAU 1976 (Bessel epoch related parameters).
        /// </summary>
        /// <param name="gamb">Fukushima-Williams angle gamma_bar (radians).</param>
        /// <param name="phib">Fukushima-Williams angle phi_bar (radians).</param>
        /// <param name="psi">Fukushima-Williams angle psi (radians).</param>
        /// <param name="eps">Obliquity epsilon (radians).</param>
        /// <param name="r">Returned rotation matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Fw2m(double gamb, double phib, double psi, double eps, double[] r)
        {
            ValidateArray(r, 9, nameof(r));

            iauFw2m(gamb, phib, psi, eps, r);
        }

        /// <summary>
        /// Fw2xy (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauFw2xy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauFw2xy(double gamb, double phib, double psi, double eps, ref double x, ref double y);

        /// <summary>
        /// CIO coordinates from Frame Tie parameters.
        /// </summary>
        /// <param name="gamb">Fukushima-Williams angle gamma_bar (radians).</param>
        /// <param name="phib">Fukushima-Williams angle phi_bar (radians).</param>
        /// <param name="psi">Fukushima-Williams angle psi (radians).</param>
        /// <param name="eps">Obliquity epsilon (radians).</param>
        /// <param name="x">Returned CIP X coordinate.</param>
        /// <param name="y">Returned CIP Y coordinate.</param>
        public static void Fw2xy(double gamb, double phib, double psi, double eps, ref double x, ref double y)
        {
            iauFw2xy(gamb, phib, psi, eps, ref x, ref y);
        }

        /// <summary>
        /// G2icrs (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauG2icrs", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauG2icrs(double dl, double db, ref double dr, ref double dd);

        /// <summary>
        /// Transform Galactic to ICRS coordinates.
        /// </summary>
        /// <param name="dl">Galactic longitude (radians).</param>
        /// <param name="db">Galactic latitude (radians).</param>
        /// <param name="dr">Returned ICRS right ascension (radians).</param>
        /// <param name="dd">Returned ICRS declination (radians).</param>
        public static void G2icrs(double dl, double db, ref double dr, ref double dd)
        {
            iauG2icrs(dl, db, ref dr, ref dd);
        }

        /// <summary>
        /// Gc2gd (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauGc2gd", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauGc2gd(SofaReferenceEllipsoids n,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] xyz,
                                       ref double elong,
                                       ref double phi,
                                       ref double height);

        /// <summary>
        /// Geocentric to geodetic coordinates.
        /// </summary>
        /// <param name="n">Reference ellipsoid identifier.</param>
        /// <param name="xyz">Geocentric Cartesian coordinates (meters, length 3).</param>
        /// <param name="elong">Returned longitude (east positive, radians).</param>
        /// <param name="phi">Returned geodetic latitude (radians).</param>
        /// <param name="height">Returned height above ellipsoid (meters).</param>
        /// <returns>Status code: 0 = OK, &lt;0 indicates an error condition.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        /// <returns>Return value from Gc2gd</returns>
        public static int Gc2gd(SofaReferenceEllipsoids n, double[] xyz, ref double elong, ref double phi, ref double height)
        {
            ValidateArray(xyz, 3, nameof(xyz));

            return iauGc2gd(n, xyz, ref elong, ref phi, ref height);
        }

        /// <summary>
        /// Gc2gde (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauGc2gde", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauGc2gde(double a,
                                        double f,
                                        [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] xyz,
                                        ref double elong,
                                        ref double phi,
                                        ref double height);

        /// <summary>
        /// Geocentric to geodetic coordinates (given ellipsoid).
        /// </summary>
        /// <param name="a">Equatorial radius (meters).</param>
        /// <param name="f">Flattening.</param>
        /// <param name="xyz">Geocentric Cartesian coordinates (meters, length 3).</param>
        /// <param name="elong">Returned longitude (east positive, radians).</param>
        /// <param name="phi">Returned geodetic latitude (radians).</param>
        /// <param name="height">Returned height above ellipsoid (meters).</param>
        /// <returns>Status code: 0 = OK, &lt;0 indicates an error condition.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        /// <returns>Return value from Gc2gde</returns>
        public static int Gc2gde(double a, double f, double[] xyz, ref double elong, ref double phi, ref double height)
        {
            ValidateArray(xyz, 3, nameof(xyz));

            return iauGc2gde(a, f, xyz, ref elong, ref phi, ref height);
        }

        /// <summary>
        /// Gd2gc (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauGd2gc", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauGd2gc(SofaReferenceEllipsoids n,
                                       double elong,
                                       double phi,
                                       double height,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] xyz);

        /// <summary>
        /// Geodetic to geocentric coordinates.
        /// </summary>
        /// <param name="n">Reference ellipsoid identifier.</param>
        /// <param name="elong">Longitude (east positive, radians).</param>
        /// <param name="phi">Geodetic latitude (radians).</param>
        /// <param name="height">Height above ellipsoid (meters).</param>
        /// <param name="xyz">Returned geocentric Cartesian coordinates (meters, length 3).</param>
        /// <returns>Status code: 0 = OK, &lt;0 indicates an error condition.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        /// <returns>Return value from Gd2gc</returns>
        public static int Gd2gc(SofaReferenceEllipsoids n, double elong, double phi, double height, double[] xyz)
        {
            ValidateArray(xyz, 3, nameof(xyz));

            return iauGd2gc(n, elong, phi, height, xyz);
        }

        /// <summary>
        /// Gd2gce (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauGd2gce", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauGd2gce(double a, double f, double elong, double phi, double height, [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] xyz);

        /// <summary>
        /// Geodetic to geocentric coordinates (given ellipsoid).
        /// </summary>
        /// <param name="a">Equatorial radius (meters).</param>
        /// <param name="f">Flattening.</param>
        /// <param name="elong">Longitude (east positive, radians).</param>
        /// <param name="phi">Geodetic latitude (radians).</param>
        /// <param name="height">Height above ellipsoid (meters).</param>
        /// <param name="xyz">Returned geocentric Cartesian coordinates (meters, length 3).</param>
        /// <returns>Status code: 0 = OK, &lt;0 indicates an error condition.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        /// <returns>Return value from Gd2gce</returns>
        public static int Gd2gce(double a, double f, double elong, double phi, double height, double[] xyz)
        {
            ValidateArray(xyz, 3, nameof(xyz));

            return iauGd2gce(a, f, elong, phi, height, xyz);
        }

        /// <summary>
        /// Gmst00 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauGmst00", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauGmst00(double uta, double utb, double tta, double ttb);

        /// <summary>
        /// Greenwich mean sidereal time (model consistent with IAU 2000 resolutions
        /// </summary>
        /// <param name="uta">UT1 Julian date component 1</param>
        /// <param name="utb">UT1 Julian date component 2</param>
        /// <param name="tta">Terrestrial time Julian date component 1</param>
        /// <param name="ttb">Terrestrial time UT1 Julian date component 2</param>
        /// <returns>GMST in radians.</returns>
        /// <returns>Return value from Gmst00</returns>
        public static double Gmst00(double uta, double utb, double tta, double ttb)
        {
            return iauGmst00(uta, utb, tta, ttb);
        }

        /// <summary>
        /// Gmst06 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauGmst06", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauGmst06(double uta, double utb, double tta, double ttb);

        /// <summary>
        /// Greenwich Mean Sidereal Time (UT1 to TT).
        /// </summary>
        /// <param name="uta">UT1 Julian date component 1.</param>
        /// <param name="utb">UT1 Julian date component 2.</param>
        /// <param name="tta">TT Julian date component 1.</param>
        /// <param name="ttb">TT Julian date component 2.</param>
        /// <returns>GMST in radians.</returns>
        /// <returns>Return value from Gmst06</returns>
        public static double Gmst06(double uta, double utb, double tta, double ttb)
        {
            return iauGmst06(uta, utb, tta, ttb);
        }

        /// <summary>
        /// Gmst82 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauGmst82", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauGmst82(double dj1, double dj2);

        /// <summary>
        /// Greenwich Mean Sidereal Time (UT1 only).
        /// </summary>
        /// <param name="dj1">UT1 Julian date component 1.</param>
        /// <param name="dj2">UT1 Julian date component 2.</param>
        /// <returns>GMST in radians.</returns>
        /// <returns>Return value from Gmst82</returns>
        public static double Gmst82(double dj1, double dj2)
        {
            return iauGmst82(dj1, dj2);
        }

        /// <summary>
        /// Gst00a (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauGst00a", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauGst00a(double uta, double utb, double tta, double ttb);

        /// <summary>
        /// Calculates the Greenwich apparent sidereal time (GAST) at 0h UT1, using the IAU 2000A precession-nutation
        /// model.
        /// </summary>
        /// <remarks>This method is typically used in astronomical calculations that require precise
        /// sidereal time, such as transforming between celestial and terrestrial reference frames. The two-part Julian
        /// Date representation allows for high-precision time specification and is recommended for applications
        /// requiring full double-precision accuracy.</remarks>
        /// <param name="uta">The UT1 Julian Date, part A. This value, when added to <paramref name="utb"/>, specifies the UT1 Julian Date
        /// as a two-part value for increased precision.</param>
        /// <param name="utb">The UT1 Julian Date, part B. This value, when added to <paramref name="uta"/>, specifies the UT1 Julian Date
        /// as a two-part value for increased precision.</param>
        /// <param name="tta">The TT (Terrestrial Time) Julian Date, part A. This value, when added to <paramref name="ttb"/>, specifies
        /// the TT Julian Date as a two-part value for increased precision.</param>
        /// <param name="ttb">The TT (Terrestrial Time) Julian Date, part B. This value, when added to <paramref name="tta"/>, specifies
        /// the TT Julian Date as a two-part value for increased precision.</param>
        /// <returns>The Greenwich apparent sidereal time, in radians, in the range 0 to 2π.</returns>
        /// <returns>Return value from Gst00a</returns>
        public static double Gst00a(double uta, double utb, double tta, double ttb)
        {
            return iauGst00a(uta, utb, tta, ttb);
        }

        /// <summary>
        /// Gst00b (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauGst00b", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauGst00b(double uta, double utb);

        /// <summary>
        /// Greenwich Apparent Sidereal Time (UT1 only, precedes IAU 2000).
        /// </summary>
        /// <param name="uta">UT1 Julian date component 1.</param>
        /// <param name="utb">UT1 Julian date component 2.</param>
        /// <returns>GAST in radians.</returns>
        /// <returns>Return value from Gst00b</returns>
        public static double Gst00b(double uta, double utb)
        {
            return iauGst00b(uta, utb);
        }

        /// <summary>
        /// Gst06 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauGst06", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauGst06(double uta, double utb, double tta, double ttb, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rnpb);

        /// <summary>
        /// Greenwich Apparent Sidereal Time (IAU 2000/2006).
        /// </summary>
        /// <param name="uta">UT1 Julian date component 1.</param>
        /// <param name="utb">UT1 Julian date component 2.</param>
        /// <param name="tta">TT Julian date component 1.</param>
        /// <param name="ttb">TT Julian date component 2.</param>
        /// <param name="rnpb">Celestial-to-true matrix (row-major, length 9).</param>
        /// <returns>GAST in radians.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        /// <returns>Return value from Gst06</returns>
        public static double Gst06(double uta, double utb, double tta, double ttb, double[] rnpb)
        {
            ValidateArray(rnpb, 9, nameof(rnpb));

            return iauGst06(uta, utb, tta, ttb, rnpb);
        }

        /// <summary>
        /// Gst06a (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauGst06a", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauGst06a(double uta, double utb, double tta, double ttb);

        /// <summary>
        /// Greenwich Apparent Sidereal Time (IAU 2000/2006).
        /// </summary>
        /// <param name="uta">UT1 Julian date component 1.</param>
        /// <param name="utb">UT1 Julian date component 2.</param>
        /// <param name="tta">TT Julian date component 1.</param>
        /// <param name="ttb">TT Julian date component 2.</param>
        /// <returns>GAST in radians.</returns>
        /// <returns>Return value from Gst06a</returns>
        public static double Gst06a(double uta, double utb, double tta, double ttb)
        {
            return iauGst06a(uta, utb, tta, ttb);
        }

        /// <summary>
        /// Gst94 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauGst94", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauGst94(double uta, double utb);

        /// <summary>
        /// Greenwich Apparent Sidereal Time (UT1 only, precedes IAU 2000).
        /// </summary>
        /// <param name="uta">UT1 Julian date component 1.</param>
        /// <param name="utb">UT1 Julian date component 2.</param>
        /// <returns>GAST in radians.</returns>
        /// <returns>Return value from Gst94</returns>
        public static double Gst94(double uta, double utb)
        {
            return iauGst94(uta, utb);
        }

        /// <summary>
        /// H2fk5 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauH2fk5", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauH2fk5(
            double rh,
            double dh,
            double drh,
            double ddh,
            double pxh,
            double rvh,
            ref double r5,
            ref double d5,
            ref double dr5,
            ref double dd5,
            ref double px5,
            ref double rv5);

        /// <summary>
        /// Transform from Hipparcos to FK5 (J2000.0).
        /// </summary>
        /// <param name="rh">Hipparcos right ascension (radians).</param>
        /// <param name="dh">Hipparcos declination (radians).</param>
        /// <param name="drh">Hipparcos proper motion in RA (radians/year).</param>
        /// <param name="ddh">Hipparcos proper motion in Dec (radians/year).</param>
        /// <param name="pxh">Hipparcos parallax (arcsec).</param>
        /// <param name="rvh">Hipparcos radial velocity (km/s).</param>
        /// <param name="r5">Returned FK5 right ascension (radians).</param>
        /// <param name="d5">Returned FK5 declination (radians).</param>
        /// <param name="dr5">Returned FK5 proper motion in RA (radians/year).</param>
        /// <param name="dd5">Returned FK5 proper motion in Dec (radians/year).</param>
        /// <param name="px5">Returned FK5 parallax (arcsec).</param>
        /// <param name="rv5">Returned FK5 radial velocity (km/s).</param>
        public static void H2fk5(double rh, double dh, double drh, double ddh, double pxh, double rvh, ref double r5, ref double d5, ref double dr5, ref double dd5, ref double px5, ref double rv5)
        {
            iauH2fk5(rh, dh, drh, ddh, pxh, rvh, ref r5, ref d5, ref dr5, ref dd5, ref px5, ref rv5);
        }

        /// <summary>
        /// Hd2ae (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauHd2ae", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauHd2ae(double ha, double dec, double phi, ref double az, ref double el);

        /// <summary>
        /// Hour angle and declination to azimuth and altitude.
        /// </summary>
        /// <param name="ha">Hour angle (radians).</param>
        /// <param name="dec">Declination (radians).</param>
        /// <param name="phi">Observer geodetic latitude (radians).</param>
        /// <param name="az">Returned azimuth (radians).</param>
        /// <param name="el">Returned elevation/altitude (radians).</param>
        public static void Hd2ae(double ha, double dec, double phi, ref double az, ref double el)
        {
            iauHd2ae(ha, dec, phi, ref az, ref el);
        }

        /// <summary>
        /// Hd2pa (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauHd2pa", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauHd2pa(double ha, double dec, double phi);

        /// <summary>
        /// Hour angle and declination to parallactic angle.
        /// </summary>
        /// <param name="ha">Hour angle (radians).</param>
        /// <param name="dec">Declination (radians).</param>
        /// <param name="phi">Observer geodetic latitude (radians).</param>
        /// <returns>Parallactic angle (radians).</returns>
        /// <returns>Return value from Hd2pa</returns>
        public static double Hd2pa(double ha, double dec, double phi)
        {
            return iauHd2pa(ha, dec, phi);
        }

        /// <summary>
        /// Hfk5z (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauHfk5z", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauHfk5z(double rh, double dh, double date1, double date2, ref double r5, ref double d5, ref double dr5, ref double dd5);

        /// <summary>
        /// Transform from Hipparcos to FK5 (catalog).
        /// </summary>
        /// <param name="rh">Hipparcos right ascension (radians).</param>
        /// <param name="dh">Hipparcos declination (radians).</param>
        /// <param name="date1">TDB as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TDB as a 2-part Julian Date (part 2).</param>
        /// <param name="r5">Returned FK5 right ascension (radians).</param>
        /// <param name="d5">Returned FK5 declination (radians).</param>
        /// <param name="dr5">Returned FK5 proper motion in RA (radians/year).</param>
        /// <param name="dd5">Returned FK5 proper motion in Dec (radians/year).</param>
        public static void Hfk5z(double rh, double dh, double date1, double date2, ref double r5, ref double d5, ref double dr5, ref double dd5)
        {
            iauHfk5z(rh, dh, date1, date2, ref r5, ref d5, ref dr5, ref dd5);
        }

        /// <summary>
        /// Icrs2g (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauIcrs2g", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauIcrs2g(double dr, double dd, ref double dl, ref double db);

        /// <summary>
        /// Transform ICRS to Galactic coordinates.
        /// </summary>
        /// <param name="dr">ICRS right ascension (radians).</param>
        /// <param name="dd">ICRS declination (radians).</param>
        /// <param name="dl">Returned galactic longitude (radians).</param>
        /// <param name="db">Returned galactic latitude (radians).</param>
        public static void Icrs2g(double dr, double dd, ref double dl, ref double db)
        {
            iauIcrs2g(dr, dd, ref dl, ref db);
        }

        /// <summary>
        /// Ir (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauIr", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauIr([MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] r);

        /// <summary>
        /// Initialize a rotation matrix to the identity matrix.
        /// </summary>
        /// <param name="r">A 9-element array that receives the 3×3 identity rotation matrix in row-major order.</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Ir(double[] r)
        {
            ValidateArray(r, 9, nameof(r));

            iauIr(r);
        }

        /// <summary>
        /// Jd2cal (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauJd2cal", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauJd2cal(double dj1, double dj2, ref int iy, ref int im, ref int id, ref double fd);

        /// <summary>
        /// Convert JD pair to calendar date.
        /// </summary>
        /// <param name="dj1">First part of the Julian Date.</param>
        /// <param name="dj2">Second part of the Julian Date.</param>
        /// <param name="iy">Returned year in Gregorian calendar.</param>
        /// <param name="im">Returned month in Gregorian calendar.</param>
        /// <param name="id">Returned day in Gregorian calendar.</param>
        /// <param name="fd">Returned fraction of the day.</param>
        /// <remarks>
        /// This is a P/Invoke wrapper for the SOFA <c>iauJd2cal</c> routine.
        /// </remarks>
        /// <returns>Status code: 0 = OK, &lt;0 = error.</returns>
        /// <returns>Return value from Jd2cal</returns>
        public static int Jd2cal(double dj1, double dj2, ref int iy, ref int im, ref int id, ref double fd)
        {
            return iauJd2cal(dj1, dj2, ref iy, ref im, ref id, ref fd);
        }

        /// <summary>
        /// Jdcalf (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauJdcalf", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauJdcalf(int ndp, double dj1, double dj2, int[] iymdf);

        /// <summary>
        /// Julian date conversion with specified decimal places.
        /// </summary>
        /// <param name="ndp">Number of decimal places in the fraction field.</param>
        /// <param name="dj1">First part of the Julian Date.</param>
        /// <param name="dj2">Second part of the Julian Date.</param>
        /// <param name="iymdf">Returned year, month, day, fraction array (length 4).</param>
        /// <remarks>
        /// This is a P/Invoke wrapper for the SOFA <c>iauJdcalf</c> routine.
        /// </remarks>
        /// <returns>Status code: 0 = OK, &lt;0 = error.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        /// <returns>Return value from Jdcalf</returns>
        public static int Jdcalf(int ndp, double dj1, double dj2, int[] iymdf)
        {

            return iauJdcalf(ndp, dj1, dj2, iymdf);
        }

        /// <summary>
        /// Ld (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauLd", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauLd(double bm,
                                     [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] p,
                                     [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] q,
                                     [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] e,
                                     double em,
                                     double dlim,
                                     [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] p1);

        /// <summary>
        /// Light deflection by a single body.
        /// </summary>
        /// <param name="bm">Mass of the body (solar masses).</param>
        /// <param name="p">Direction from observer to source (length 3).</param>
        /// <param name="q">Direction from body to source (length 3).</param>
        /// <param name="e">Direction from body to observer (length 3).</param>
        /// <param name="em">Distance from body to observer.</param>
        /// <param name="dlim">Deflection limiter.</param>
        /// <param name="p1">Returned deflected direction (length 3).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Ld(double bm, double[] p, double[] q, double[] e, double em, double dlim, double[] p1)
        {
            ValidateArray(p, 3, nameof(p));
            ValidateArray(q, 3, nameof(q));
            ValidateArray(e, 3, nameof(e));
            ValidateArray(p1, 3, nameof(p1));

            iauLd(bm, p, q, e, em, dlim, p1);
        }

        /// <summary>
        /// Ldn (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauLdn", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauLdn(int n,
                                      LdBody[] b,
                                      [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] ob,
                                      [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] sc,
                                      [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] sn);

        /// <summary>
        /// Light deflection for list of bodies.
        /// </summary>
        /// <param name="n">Number of bodies in <paramref name="b"/>.</param>
        /// <param name="b">Body parameters array.</param>
        /// <param name="ob">Observer barycentric position (length 3).</param>
        /// <param name="sc">Coordinate direction (length 3).</param>
        /// <param name="sn">Returned coordinate direction, corrected (length 3).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Ldn(int n, LdBody[] b, double[] ob, double[] sc, double[] sn)
        {
            ValidateArray(ob, 3, nameof(ob));
            ValidateArray(sc, 3, nameof(sc));
            ValidateArray(sn, 3, nameof(sn));

            iauLdn(n, b, ob, sc, sn);
        }

        /// <summary>
        /// Ldsun (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauLdsun", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauLdsun([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] p,
                                        [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] e,
                                        double em,
                                        [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] p1);

        /// <summary>
        /// Sun-specific light deflection helper.
        /// </summary>
        /// <param name="p">Direction from observer to source (length 3).</param>
        /// <param name="e">Direction from Sun to observer (length 3).</param>
        /// <param name="em">Distance from Sun to observer.</param>
        /// <param name="p1">Returned deflected direction (length 3).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Ldsun(double[] p, double[] e, double em, double[] p1)
        {
            ValidateArray(p, 3, nameof(p));
            ValidateArray(e, 3, nameof(e));
            ValidateArray(p1, 3, nameof(p1));

            iauLdsun(p, e, em, p1);
        }

        /// <summary>
        /// Lteceq (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauLteceq", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauLteceq(double epj, double dl, double db, ref double dr, ref double dd);

        /// <summary>
        /// Transform ecliptic to equatorial (FK4 epoch related).
        /// </summary>
        /// <param name="epj">Besselian epoch.</param>
        /// <param name="dl">Ecliptic longitude (radians).</param>
        /// <param name="db">Ecliptic latitude (radians).</param>
        /// <param name="dr">Returned right ascension (radians).</param>
        /// <param name="dd">Returned declination (radians).</param>
        public static void Lteceq(double epj, double dl, double db, ref double dr, ref double dd)
        {
            iauLteceq(epj, dl, db, ref dr, ref dd);
        }

        /// <summary>
        /// Ltecm (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauLtecm", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauLtecm(double epj, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rm);

        /// <summary>
        /// Ecliptic to equatorial matrix (FK4 epoch related).
        /// </summary>
        /// <param name="epj">Besselian epoch.</param>
        /// <param name="rm">Returned rotation matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Ltecm(double epj, double[] rm)
        {
            ValidateArray(rm, 9, nameof(rm));

            iauLtecm(epj, rm);
        }

        /// <summary>
        /// Lteqec (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauLteqec", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauLteqec(double epj, double dr, double dd, ref double dl, ref double db);

        /// <summary>
        /// Transform equatorial to ecliptic (FK4 epoch related).
        /// </summary>
        /// <param name="epj">Besselian epoch.</param>
        /// <param name="dr">Right ascension (radians).</param>
        /// <param name="dd">Declination (radians).</param>
        /// <param name="dl">Returned ecliptic longitude (radians).</param>
        /// <param name="db">Returned ecliptic latitude (radians).</param>
        public static void Lteqec(double epj, double dr, double dd, ref double dl, ref double db)
        {
            iauLteqec(epj, dr, dd, ref dl, ref db);
        }

        /// <summary>
        /// Ltp (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauLtp", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauLtp(double epj, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rp);

        /// <summary>
        /// Precession matrix, Besselian epoch.
        /// </summary>
        /// <param name="epj">Besselian epoch.</param>
        /// <param name="rp">Returned precession matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Ltp(double epj, double[] rp)
        {
            ValidateArray(rp, 9, nameof(rp));

            iauLtp(epj, rp);
        }

        /// <summary>
        /// Ltpb (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauLtpb", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauLtpb(double epj, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rpb);

        /// <summary>
        /// Precession matrix, Besselian epoch, including E-terms.
        /// </summary>
        /// <param name="epj">Besselian epoch.</param>
        /// <param name="rpb">Returned precession matrix including E-terms (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Ltpb(double epj, double[] rpb)
        {
            ValidateArray(rpb, 9, nameof(rpb));

            iauLtpb(epj, rpb);
        }

        /// <summary>
        /// Ltpecl (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauLtpecl", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauLtpecl(double epj, [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] vec);

        /// <summary>
        /// Transform ecliptic coordinates to FK4 J1900.0 (flat Earth model).
        /// </summary>
        /// <param name="epj">Besselian epoch.</param>
        /// <param name="vec">Returned transformed vector (length 3).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Ltpecl(double epj, double[] vec)
        {
            ValidateArray(vec, 3, nameof(vec));

            iauLtpecl(epj, vec);
        }

        /// <summary>
        /// Ltpequ (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauLtpequ", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauLtpequ(double epj, [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] veq);

        /// <summary>
        /// Transform equatorial coordinates to FK4 J1900.0 (flat Earth model).
        /// </summary>
        /// <param name="epj">Besselian epoch.</param>
        /// <param name="veq">Returned transformed vector (length 3).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Ltpequ(double epj, double[] veq)
        {
            ValidateArray(veq, 3, nameof(veq));

            iauLtpequ(epj, veq);
        }

        /// <summary>
        /// Moon98 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauMoon98", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauMoon98(double date1, double date2, [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] pv);

        /// <summary>
        /// Moon position helper.
        /// </summary>
        /// <param name="date1">TDB as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TDB as a 2-part Julian Date (part 2).</param>
        /// <param name="pv">Returned Moon position/velocity (length 6).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Moon98(double date1, double date2, double[] pv)
        {
            ValidateArray(pv, 6, nameof(pv));

            iauMoon98(date1, date2, pv);
        }

        /// <summary>
        /// Num00a (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauNum00a", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauNum00a(double date1, double date2, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rmatn);

        /// <summary>
        /// Computes the nutation matrix for a given date using the IAU 2000A nutation model.
        /// </summary>
        /// <param name="date1">The first part of the Julian Date representing the Terrestrial Time (TT) of the desired date. Typically,
        /// this is the integer part of the Julian Date.</param>
        /// <param name="date2">The second part of the Julian Date representing the Terrestrial Time (TT) of the desired date. This is
        /// usually the fractional part of the Julian Date. The sum of date1 and date2 gives the full Julian Date.</param>
        /// <param name="rmatn">When the method returns, contains a 3×3 nutation matrix in row-major order. The array must have a length of
        /// at least 9 elements.</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Num00a(double date1, double date2, double[] rmatn)
        {
            ValidateArray(rmatn, 9, nameof(rmatn));

            iauNum00a(date1, date2, rmatn);
        }

        /// <summary>
        /// Num00b (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauNum00b", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauNum00b(double date1, double date2, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rmatn);

        /// <summary>
        /// Bias-precession-nutation matrix, precession IAU 2000 or IAU 1976, nutation IAU 2000B.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="rmatn">Returned nutation matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Num00b(double date1, double date2, double[] rmatn)
        {
            ValidateArray(rmatn, 9, nameof(rmatn));

            iauNum00b(date1, date2, rmatn);
        }

        /// <summary>
        /// Num06a (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauNum06a", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauNum06a(double date1, double date2, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rmatn);

        /// <summary>
        /// Computes the nutation, precession, and frame bias rotation matrix for a given date using the IAU 2006
        /// precession and IAU 2000A nutation models.
        /// </summary>
        /// <remarks>This method is a P/Invoke wrapper for the SOFA library function 'iauNum06a'. The
        /// input date should be supplied as a two-part Julian Date to preserve precision. The resulting rotation matrix
        /// can be used to transform celestial coordinates between reference frames. This method does not perform
        /// validation on the input array length.</remarks>
        /// <param name="date1">The first part of the Julian Date representing the Terrestrial Time (TT) of the desired epoch. This value is
        /// typically the integer part of the Julian Date.</param>
        /// <param name="date2">The second part of the Julian Date representing the Terrestrial Time (TT) of the desired epoch. This value
        /// is typically the fractional part of the Julian Date.</param>
        /// <param name="rmatn">When the method returns, contains a 3×3 rotation matrix (as a 9-element array in row-major order) that
        /// transforms vectors from the mean equator and equinox of J2000.0 to the true equator and equinox of date. The
        /// array must have a length of at least 9.</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Num06a(double date1, double date2, double[] rmatn)
        {
            ValidateArray(rmatn, 9, nameof(rmatn));

            iauNum06a(date1, date2, rmatn);
        }

        /// <summary>
        /// Numat (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauNumat", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauNumat(double epsa, double dpsi, double deps, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rmatn);

        /// <summary>
        /// Bias-precession-nutation matrix, IAU 2000 precession with IAU 2000A or 2000B nutation.
        /// </summary>
        /// <param name="epsa">Mean obliquity (radians).</param>
        /// <param name="dpsi">Nutation in longitude (radians).</param>
        /// <param name="deps">Nutation in obliquity (radians).</param>
        /// <param name="rmatn">Returned bias-precession-nutation matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Numat(double epsa, double dpsi, double deps, double[] rmatn)
        {
            ValidateArray(rmatn, 9, nameof(rmatn));

            iauNumat(epsa, dpsi, deps, rmatn);
        }

        /// <summary>
        /// Nut00a (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauNut00a", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauNut00a(double date1, double date2, ref double dpsi, ref double deps);

        /// <summary>
        /// Computes the nutation in longitude and obliquity for a given date using the IAU 2000A model.
        /// </summary>
        /// <remarks>This method is a P/Invoke wrapper for the SOFA library function 'iauNut00a', which implements the IAU
        /// 2000A nutation model. The date should be supplied as a two-part Julian Date to preserve precision. The results are
        /// suitable for high-precision astronomical calculations.</remarks>
        /// <param name="date1">The first part of the Julian Date representing the date for which to calculate nutation. Typically the integer part.</param>
        /// <param name="date2">The second part of the Julian Date representing the date for which to calculate nutation. Typically the fractional
        /// part.</param>
        /// <param name="dpsi">When this method returns, contains the nutation in longitude, in radians.</param>
        /// <param name="deps">When this method returns, contains the nutation in obliquity, in radians.</param>
        public static void Nut00a(double date1, double date2, ref double dpsi, ref double deps)
        {
            iauNut00a(date1, date2, ref dpsi, ref deps);
        }

        /// <summary>
        /// Nut00b (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauNut00b", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauNut00b(double date1, double date2, ref double dpsi, ref double deps);

        /// <summary>
        /// Bias-precession-nutation matrix, precession IAU 2000 or IAU 1976, nutation IAU 2000B.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="dpsi">Returned nutation in longitude (radians).</param>
        /// <param name="deps">Returned nutation in obliquity (radians).</param>
        public static void Nut00b(double date1, double date2, ref double dpsi, ref double deps)
        {
            iauNut00b(date1, date2, ref dpsi, ref deps);
        }

        /// <summary>
        /// Nut06a (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauNut06a", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauNut06a(double date1, double date2, ref double dpsi, ref double deps);

        /// <summary>
        /// Nutation: IAU 2000B model.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="dpsi">Returned nutation in longitude (radians).</param>
        /// <param name="deps">Returned nutation in obliquity (radians).</param>
        public static void Nut06a(double date1, double date2, ref double dpsi, ref double deps)
        {
            iauNut06a(date1, date2, ref dpsi, ref deps);
        }

        /// <summary>
        /// Nut80 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauNut80", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauNut80(double date1, double date2, ref double dpsi, ref double deps);

        /// <summary>
        /// Nutation: IAU 1980 model.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="dpsi">Returned nutation in longitude (radians).</param>
        /// <param name="deps">Returned nutation in obliquity (radians).</param>
        public static void Nut80(double date1, double date2, ref double dpsi, ref double deps)
        {
            iauNut80(date1, date2, ref dpsi, ref deps);
        }

        /// <summary>
        /// Nutm80 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauNutm80", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauNutm80(double date1, double date2, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rmatn);

        /// <summary>
        /// Nutation-matrix: IAU 1980.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="rmatn">Returned nutation matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Nutm80(double date1, double date2, double[] rmatn)
        {
            ValidateArray(rmatn, 9, nameof(rmatn));

            iauNutm80(date1, date2, rmatn);
        }

        /// <summary>
        /// Obl06 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauObl06", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauObl06(double date1, double date2);

        /// <summary>
        /// Calculates the mean obliquity of the ecliptic for a given date using the IAU 2006 precession model.
        /// </summary>
        /// <remarks>This function is intended for high-precision astronomical calculations and is based
        /// on the IAU 2006 precession model. The date should be supplied as a two-part Julian Date to preserve
        /// precision, especially for dates far from the present epoch.</remarks>
        /// <param name="date1">The first part of the Julian Date representing the date for which to compute the mean obliquity. Typically
        /// the larger (integral) part of the Julian Date.</param>
        /// <param name="date2">The second part of the Julian Date representing the date for which to compute the mean obliquity. Typically
        /// the fractional part of the Julian Date.</param>
        /// <returns>The mean obliquity of the ecliptic, in radians, at the specified date according to the IAU 2006 precession
        /// model.</returns>
        /// <returns>Return value from Obl06</returns>
        public static double Obl06(double date1, double date2)
        {
            return iauObl06(date1, date2);
        }

        /// <summary>
        /// Obl80 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauObl80", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauObl80(double date1, double date2);

        /// <summary>
        /// Mean obliquity of the ecliptic, IAU 1980 model.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <returns>Mean obliquity in radians.</returns>
        /// <returns>Return value from Obl80</returns>
        public static double Obl80(double date1, double date2)
        {
            return iauObl80(date1, date2);
        }

        /// <summary>
        /// P06e (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauP06e", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauP06e(
            double date1,
            double date2,
            ref double eps0,
            ref double psia,
            ref double oma,
            ref double bpa,
            ref double bqa,
            ref double pia,
            ref double bpia,
            ref double epsa,
            ref double chia,
            ref double za,
            ref double zetaa,
            ref double thetaa,
            ref double pa,
            ref double gam,
            ref double phi,
            ref double psi);

        /// <summary>
        /// CIO RA and Earth Orientation parameters (high precision).
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="eps0">Returned obliquity at J2000.0 (radians).</param>
        /// <param name="psia">Returned angle psi_A (radians).</param>
        /// <param name="oma">Returned angle omega_A (radians).</param>
        /// <param name="bpa">Returned precession parameter bpa.</param>
        /// <param name="bqa">Returned precession parameter bqa.</param>
        /// <param name="pia">Returned precession parameter pia.</param>
        /// <param name="bpia">Returned precession parameter bpia.</param>
        /// <param name="epsa">Returned mean obliquity (radians).</param>
        /// <param name="chia">Returned precession angle chi_A (radians).</param>
        /// <param name="za">Returned precession angle z_A (radians).</param>
        /// <param name="zetaa">Returned precession angle zeta_A (radians).</param>
        /// <param name="thetaa">Returned precession angle theta_A (radians).</param>
        /// <param name="pa">Returned precession angle p_A (radians).</param>
        /// <param name="gam">Returned precession angle gamma (radians).</param>
        /// <param name="phi">Returned precession angle phi (radians).</param>
        /// <param name="psi">Returned precession angle psi (radians).</param>
        public static void P06e(double date1, double date2, ref double eps0, ref double psia, ref double oma, ref double bpa, ref double bqa, ref double pia, ref double bpia, ref double epsa, ref double chia, ref double za, ref double zetaa, ref double thetaa, ref double pa, ref double gam, ref double phi, ref double psi)
        {
            iauP06e(date1, date2, ref eps0, ref psia, ref oma, ref bpa, ref bqa, ref pia, ref bpia, ref epsa, ref chia, ref za, ref zetaa, ref thetaa, ref pa, ref gam, ref phi, ref psi);
        }

        /// <summary>
        /// P2pv (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauP2pv", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauP2pv([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] p, [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] pv);

        /// <summary>
        /// Extend a 3D position vector to a 6D position-velocity vector by copying the position to the velocity.
        /// </summary>
        /// <param name="p">Position vector (length 3).</param>
        /// <param name="pv">Returned position-velocity vector (length 6).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void P2pv(double[] p, double[] pv)
        {
            ValidateArray(p, 3, nameof(p));
            ValidateArray(pv, 6, nameof(pv));

            iauP2pv(p, pv);
        }

        /// <summary>
        /// P2s (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauP2s", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauP2s([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] p, ref double theta, ref double phi, ref double r);

        /// <summary>
        /// Cartesian to spherical polar coordinates.
        /// </summary>
        /// <param name="p">Cartesian vector (length 3).</param>
        /// <param name="theta">Returned longitude angle (radians).</param>
        /// <param name="phi">Returned latitude angle (radians).</param>
        /// <param name="r">Returned radius.</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void P2s(double[] p, ref double theta, ref double phi, ref double r)
        {
            ValidateArray(p, 3, nameof(p));

            iauP2s(p, ref theta, ref phi, ref r);
        }

        /// <summary>
        /// Pap (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPap", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauPap([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] a, [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] b);

        /// <summary>
        /// Parallactic angle for a star.
        /// </summary>
        /// <param name="a">Direction 1 (length 3).</param>
        /// <param name="b">Direction 2 (length 3).</param>
        /// <returns>Parallactic angle (radians).</returns>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        /// <returns>Return value from Pap</returns>
        public static double Pap(double[] a, double[] b)
        {
            ValidateArray(a, 3, nameof(a));
            ValidateArray(b, 3, nameof(b));

            return iauPap(a, b);
        }

        /// <summary>
        /// Pas (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPas", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauPas(double al, double ap, double bl, double bp);

        /// <summary>
        /// Parallactic angle for two directions.
        /// </summary>
        /// <param name="al">RA/longitude of first direction (radians).</param>
        /// <param name="ap">Dec/latitude of first direction (radians).</param>
        /// <param name="bl">RA/longitude of second direction (radians).</param>
        /// <param name="bp">Dec/latitude of second direction (radians).</param>
        /// <returns>Parallactic angle (radians).</returns>
        /// <returns>Return value from Pas</returns>
        public static double Pas(double al, double ap, double bl, double bp)
        {
            return iauPas(al, ap, bl, bp);
        }

        /// <summary>
        /// Pb06 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPb06", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPb06(double date1, double date2, ref double bzeta, ref double bz, ref double btheta);

        /// <summary>
        /// Precession matrix, IAU 2006 (Besselian epoch related parameters).
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="bzeta">Returned Fukushima-Williams angle zeta (radians).</param>
        /// <param name="bz">Returned Fukushima-Williams angle z (radians).</param>
        /// <param name="btheta">Returned Fukushima-Williams angle theta (radians).</param>
        public static void Pb06(double date1, double date2, ref double bzeta, ref double bz, ref double btheta)
        {
            iauPb06(date1, date2, ref bzeta, ref bz, ref btheta);
        }

        /// <summary>
        /// Pdp (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPdp", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauPdp([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] a, [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] b);

        /// <summary>
        /// Scalar product of two 3D vectors.
        /// </summary>
        /// <param name="a">First vector (length 3).</param>
        /// <param name="b">Second vector (length 3).</param>
        /// <returns>Dot product of <paramref name="a"/> and <paramref name="b"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        /// <returns>Return value from Pdp</returns>
        public static double Pdp(double[] a, double[] b)
        {
            ValidateArray(a, 3, nameof(a));
            ValidateArray(b, 3, nameof(b));

            return iauPdp(a, b);
        }

        /// <summary>
        /// Pfw06 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPfw06", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPfw06(double date1, double date2, ref double gamb, ref double phib, ref double psib, ref double epsa);

        /// <summary>
        /// Precession matrix elements (Fukushima-Williams precession angles).
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="gamb">Returned gamma_bar (radians).</param>
        /// <param name="phib">Returned phi_bar (radians).</param>
        /// <param name="psib">Returned psi (radians).</param>
        /// <param name="epsa">Returned epsilon_A (radians).</param>
        public static void Pfw06(double date1, double date2, ref double gamb, ref double phib, ref double psib, ref double epsa)
        {
            iauPfw06(date1, date2, ref gamb, ref phib, ref psib, ref epsa);
        }

        /// <summary>
        /// Plan94 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPlan94", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauPlan94(double date1, double date2, int np, [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] pv);

        /// <summary>
        /// Planetary ephemeris (approximate).
        /// </summary>
        /// <param name="date1">TDB as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TDB as a 2-part Julian Date (part 2).</param>
        /// <param name="np">Planet number.</param>
        /// <param name="pv">Returned planet position/velocity (length 6).</param>
        /// <returns>Status code: 0 = OK, &lt;0 indicates an error condition.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        /// <returns>Return value from Plan94</returns>
        public static int Plan94(double date1, double date2, int np, double[] pv)
        {
            ValidateArray(pv, 6, nameof(pv));

            return iauPlan94(date1, date2, np, pv);
        }

        /// <summary>
        /// Pm (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPm", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauPm([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] p);

        /// <summary>
        /// Magnitude of a 3D vector.
        /// </summary>
        /// <param name="p">Vector (length 3).</param>
        /// <returns>Magnitude of the vector.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        /// <returns>Return value from Pm</returns>
        public static double Pm(double[] p)
        {
            ValidateArray(p, 3, nameof(p));

            return iauPm(p);
        }

        /// <summary>
        /// Pmat00 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPmat00", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPmat00(double date1, double date2, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rbp);

        /// <summary>
        /// Precession matrix, IAU 2000.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="rbp">Returned bias-precession matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pmat00(double date1, double date2, double[] rbp)
        {
            ValidateArray(rbp, 9, nameof(rbp));

            iauPmat00(date1, date2, rbp);
        }

        /// <summary>
        /// Pmat06 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPmat06", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPmat06(double date1, double date2, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rbp);

        /// <summary>
        /// Precession matrix, IAU 2006.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="rbp">Returned bias-precession matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pmat06(double date1, double date2, double[] rbp)
        {
            ValidateArray(rbp, 9, nameof(rbp));

            iauPmat06(date1, date2, rbp);
        }

        /// <summary>
        /// Pmat76 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPmat76", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPmat76(double date1, double date2, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rmatp);

        /// <summary>
        /// Precession matrix, IAU 1976.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="rmatp">Returned precession matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pmat76(double date1, double date2, double[] rmatp)
        {
            ValidateArray(rmatp, 9, nameof(rmatp));

            iauPmat76(date1, date2, rmatp);
        }

        /// <summary>
        /// Pmp (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPmp", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPmp([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] a,
                                      [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] b,
                                      [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] amb);

        /// <summary>
        /// Subtract two 3D vectors.
        /// </summary>
        /// <param name="a">First vector (length 3).</param>
        /// <param name="b">Second vector (length 3).</param>
        /// <param name="amb">Returned vector <c>a-b</c> (length 3).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pmp(double[] a, double[] b, double[] amb)
        {
            ValidateArray(a, 3, nameof(a));
            ValidateArray(b, 3, nameof(b));
            ValidateArray(amb, 3, nameof(amb));

            iauPmp(a, b, amb);
        }

        /// <summary>
        /// Pmpx (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPmpx", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPmpx(double rc,
                                       double dc,
                                       double pr,
                                       double pd,
                                       double px,
                                       double rv,
                                       double pmt,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] pob,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] pco);

        /// <summary>
        /// Proper motion an parallax propagation helper.
        /// </summary>
        /// <param name="rc">Right ascension (radians).</param>
        /// <param name="dc">Declination (radians).</param>
        /// <param name="pr">Proper motion in RA (radians/year).</param>
        /// <param name="pd">Proper motion in Dec (radians/year).</param>
        /// <param name="px">Parallax (arcsec).</param>
        /// <param name="rv">Radial velocity (km/s).</param>
        /// <param name="pmt">Proper motion time interval (Julian years).</param>
        /// <param name="pob">Observer barycentric position (length 3).</param>
        /// <param name="pco">Returned coordinate direction (length 3).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pmpx(double rc, double dc, double pr, double pd, double px, double rv, double pmt, double[] pob, double[] pco)
        {
            ValidateArray(pob, 3, nameof(pob));
            ValidateArray(pco, 3, nameof(pco));

            iauPmpx(rc, dc, pr, pd, px, rv, pmt, pob, pco);
        }

        /// <summary>
        /// Pmsafe (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPmsafe", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauPmsafe(double ra1,
                                        double dec1,
                                        double pmr1,
                                        double pmd1,
                                        double px1,
                                        double rv1,
                                        double ep1a,
                                        double ep1b,
                                        double ep2a,
                                        double ep2b,
                                        ref double ra2,
                                        ref double dec2,
                                        ref double pmr2,
                                        ref double pmd2,
                                        ref double px2,
                                        ref double rv2);

        /// <summary>
        /// Safe proper-motion propagation (returns status).
        /// </summary>
        /// <param name="ra1">RA at epoch 1 (radians).</param>
        /// <param name="dec1">Dec at epoch 1 (radians).</param>
        /// <param name="pmr1">Proper motion in RA (radians/year).</param>
        /// <param name="pmd1">Proper motion in Dec (radians/year).</param>
        /// <param name="px1">Parallax (arcsec).</param>
        /// <param name="rv1">Radial velocity (km/s).</param>
        /// <param name="ep1a">Epoch 1 (part A).</param>
        /// <param name="ep1b">Epoch 1 (part B).</param>
        /// <param name="ep2a">Epoch 2 (part A).</param>
        /// <param name="ep2b">Epoch 2 (part B).</param>
        /// <param name="ra2">Returned RA at epoch 2 (radians).</param>
        /// <param name="dec2">Returned Dec at epoch 2 (radians).</param>
        /// <param name="pmr2">Returned proper motion in RA (radians/year).</param>
        /// <param name="pmd2">Returned proper motion in Dec (radians/year).</param>
        /// <param name="px2">Returned parallax (arcsec).</param>
        /// <param name="rv2">Returned radial velocity (km/s).</param>
        /// <returns>Status code: 0 = OK, &lt;0 indicates an error condition.</returns>
        /// <returns>Return value from Pmsafe</returns>
        public static int Pmsafe(double ra1, double dec1, double pmr1, double pmd1, double px1, double rv1, double ep1a, double ep1b, double ep2a, double ep2b, ref double ra2, ref double dec2, ref double pmr2, ref double pmd2, ref double px2, ref double rv2)
        {
            return iauPmsafe(ra1, dec1, pmr1, pmd1, px1, rv1, ep1a, ep1b, ep2a, ep2b, ref ra2, ref dec2, ref pmr2, ref pmd2, ref px2, ref rv2);
        }

        /// <summary>
        /// Pn (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPn", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPn([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] p,
                                     ref double r,
                                     [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] u);

        /// <summary>
        /// Normalize a 3D vector.
        /// </summary>
        /// <param name="p">Vector to normalize (length 3).</param>
        /// <param name="r">Returned magnitude.</param>
        /// <param name="u">Returned unit vector (length 3).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pn(double[] p, ref double r, double[] u)
        {
            ValidateArray(p, 3, nameof(p));
            ValidateArray(u, 3, nameof(u));

            iauPn(p, ref r, u);
        }

        /// <summary>
        /// Pn00 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPn00", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPn00(
            double date1,
            double date2,
            double dpsi,
            double deps,
            ref double epsa,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rb,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rp,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rbp,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rn,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rbpn);

        /// <summary>
        /// Precession-nutation matrix (full precision).
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="dpsi">Nutation in longitude (radians).</param>
        /// <param name="deps">Nutation in obliquity (radians).</param>
        /// <param name="epsa">Returned mean obliquity (radians).</param>
        /// <param name="rb">Returned frame bias matrix (row-major, length 9).</param>
        /// <param name="rp">Returned precession matrix (row-major, length 9).</param>
        /// <param name="rbp">Returned bias-precession matrix (row-major, length 9).</param>
        /// <param name="rn">Returned nutation matrix (row-major, length 9).</param>
        /// <param name="rbpn">Returned bias-precession-nutation matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pn00(double date1, double date2, double dpsi, double deps, ref double epsa, double[] rb, double[] rp, double[] rbp, double[] rn, double[] rbpn)
        {
            ValidateArray(rb, 9, nameof(rb));
            ValidateArray(rp, 9, nameof(rp));
            ValidateArray(rbp, 9, nameof(rbp));
            ValidateArray(rn, 9, nameof(rn));
            ValidateArray(rbpn, 9, nameof(rbpn));

            iauPn00(date1, date2, dpsi, deps, ref epsa, rb, rp, rbp, rn, rbpn);
        }

        /// <summary>
        /// Pn00a (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPn00a", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPn00a(
            double date1,
            double date2,
            ref double dpsi,
            ref double deps,
            ref double epsa,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rb,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rp,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rbp,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rn,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rbpn);

        /// <summary>
        /// Precession-nutation matrix (IAU 2000A).
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="dpsi">Returned nutation in longitude (radians).</param>
        /// <param name="deps">Returned nutation in obliquity (radians).</param>
        /// <param name="epsa">Returned mean obliquity (radians).</param>
        /// <param name="rb">Returned frame bias matrix (row-major, length 9).</param>
        /// <param name="rp">Returned precession matrix (row-major, length 9).</param>
        /// <param name="rbp">Returned bias-precession matrix (row-major, length 9).</param>
        /// <param name="rn">Returned nutation matrix (row-major, length 9).</param>
        /// <param name="rbpn">Returned bias-precession-nutation matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pn00a(double date1, double date2, ref double dpsi, ref double deps, ref double epsa, double[] rb, double[] rp, double[] rbp, double[] rn, double[] rbpn)
        {
            ValidateArray(rb, 9, nameof(rb));
            ValidateArray(rp, 9, nameof(rp));
            ValidateArray(rbp, 9, nameof(rbp));
            ValidateArray(rn, 9, nameof(rn));
            ValidateArray(rbpn, 9, nameof(rbpn));

            iauPn00a(date1, date2, ref dpsi, ref deps, ref epsa, rb, rp, rbp, rn, rbpn);
        }

        /// <summary>
        /// Pn00b (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPn00b", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPn00b(
            double date1,
            double date2,
            ref double dpsi,
            ref double deps,
            ref double epsa,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rb,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rp,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rbp,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rn,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rbpn);

        /// <summary>
        /// Precession-nutation matrix (IAU 2000B).
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="dpsi">Returned nutation in longitude (radians).</param>
        /// <param name="deps">Returned nutation in obliquity (radians).</param>
        /// <param name="epsa">Returned mean obliquity (radians).</param>
        /// <param name="rb">Returned frame bias matrix (row-major, length 9).</param>
        /// <param name="rp">Returned precession matrix (row-major, length 9).</param>
        /// <param name="rbp">Returned bias-precession matrix (row-major, length 9).</param>
        /// <param name="rn">Returned nutation matrix (row-major, length 9).</param>
        /// <param name="rbpn">Returned bias-precession-nutation matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pn00b(double date1, double date2, ref double dpsi, ref double deps, ref double epsa, double[] rb, double[] rp, double[] rbp, double[] rn, double[] rbpn)
        {
            ValidateArray(rb, 9, nameof(rb));
            ValidateArray(rp, 9, nameof(rp));
            ValidateArray(rbp, 9, nameof(rbp));
            ValidateArray(rn, 9, nameof(rn));
            ValidateArray(rbpn, 9, nameof(rbpn));

            iauPn00b(date1, date2, ref dpsi, ref deps, ref epsa, rb, rp, rbp, rn, rbpn);
        }

        /// <summary>
        /// Pn06 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPn06", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPn06(
            double date1,
            double date2,
            double dpsi,
            double deps,
            ref double epsa,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rb,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rp,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rbp,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rn,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rbpn);

        /// <summary>
        /// Precession-nutation matrix (full precision, given nutation).
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="dpsi">Nutation in longitude (radians).</param>
        /// <param name="deps">Nutation in obliquity (radians).</param>
        /// <param name="epsa">Returned mean obliquity (radians).</param>
        /// <param name="rb">Returned frame bias matrix (row-major, length 9).</param>
        /// <param name="rp">Returned precession matrix (row-major, length 9).</param>
        /// <param name="rbp">Returned bias-precession matrix (row-major, length 9).</param>
        /// <param name="rn">Returned nutation matrix (row-major, length 9).</param>
        /// <param name="rbpn">Returned bias-precession-nutation matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pn06(double date1, double date2, double dpsi, double deps, ref double epsa, double[] rb, double[] rp, double[] rbp, double[] rn, double[] rbpn)
        {
            ValidateArray(rb, 9, nameof(rb));
            ValidateArray(rp, 9, nameof(rp));
            ValidateArray(rbp, 9, nameof(rbp));
            ValidateArray(rn, 9, nameof(rn));
            ValidateArray(rbpn, 9, nameof(rbpn));

            iauPn06(date1, date2, dpsi, deps, ref epsa, rb, rp, rbp, rn, rbpn);
        }

        /// <summary>
        /// Pn06a (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPn06a", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPn06a(
            double date1,
            double date2,
            ref double dpsi,
            ref double deps,
            ref double epsa,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rb,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rp,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rbp,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rn,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rbpn);

        /// <summary>
        /// Precession-nutation matrix (IAU 2006/2000A).
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="dpsi">Returned nutation in longitude (radians).</param>
        /// <param name="deps">Returned nutation in obliquity (radians).</param>
        /// <param name="epsa">Returned mean obliquity (radians).</param>
        /// <param name="rb">Returned frame bias matrix (row-major, length 9).</param>
        /// <param name="rp">Returned precession matrix (row-major, length 9).</param>
        /// <param name="rbp">Returned bias-precession matrix (row-major, length 9).</param>
        /// <param name="rn">Returned nutation matrix (row-major, length 9).</param>
        /// <param name="rbpn">Returned bias-precession-nutation matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pn06a(double date1, double date2, ref double dpsi, ref double deps, ref double epsa, double[] rb, double[] rp, double[] rbp, double[] rn, double[] rbpn)
        {
            ValidateArray(rb, 9, nameof(rb));
            ValidateArray(rp, 9, nameof(rp));
            ValidateArray(rbp, 9, nameof(rbp));
            ValidateArray(rn, 9, nameof(rn));
            ValidateArray(rbpn, 9, nameof(rbpn));

            iauPn06a(date1, date2, ref dpsi, ref deps, ref epsa, rb, rp, rbp, rn, rbpn);
        }

        /// <summary>
        /// Pnm00a (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPnm00a", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPnm00a(double date1, double date2, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rbpn);

        /// <summary>
        /// Precession-nutation matrix, IAU 1976 precession and IAU 1980 nutation.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="rbpn">Returned bias-precession-nutation matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pnm00a(double date1, double date2, double[] rbpn)
        {
            ValidateArray(rbpn, 9, nameof(rbpn));

            iauPnm00a(date1, date2, rbpn);
        }

        /// <summary>
        /// Pnm00b (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPnm00b", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPnm00b(double date1, double date2, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rbpn);

        /// <summary>
        /// Precession-nutation matrix, IAU 2000 or IAU 1976 precession, IAU 2000B nutation.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="rbpn">Returned bias-precession-nutation matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pnm00b(double date1, double date2, double[] rbpn)
        {
            ValidateArray(rbpn, 9, nameof(rbpn));

            iauPnm00b(date1, date2, rbpn);
        }

        /// <summary>
        /// Pnm06a (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPnm06a", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPnm06a(double date1, double date2, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rnpb);

        /// <summary>
        /// Precession-nutation matrix, IAU 2000/2006 using precession IAU 2006 and nutation IAU 2000A.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="rnpb">Returned bias-precession-nutation matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pnm06a(double date1, double date2, double[] rnpb)
        {
            ValidateArray(rnpb, 9, nameof(rnpb));

            iauPnm06a(date1, date2, rnpb);
        }

        /// <summary>
        /// Pnm80 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPnm80", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPnm80(double date1, double date2, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rmatpn);

        /// <summary>
        /// Precession-nutation matrix, IAU 1976 precession and IAU 1980 nutation.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="rmatpn">Returned precession-nutation matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pnm80(double date1, double date2, double[] rmatpn)
        {
            ValidateArray(rmatpn, 9, nameof(rmatpn));

            iauPnm80(date1, date2, rmatpn);
        }

        /// <summary>
        /// Pom00 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPom00", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPom00(double xp, double yp, double sp, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rpom);

        /// <summary>
        /// Polar motion matrix.
        /// </summary>
        /// <param name="xp">Polar motion X (radians).</param>
        /// <param name="yp">Polar motion Y (radians).</param>
        /// <param name="sp">TIO locator s' (radians).</param>
        /// <param name="rpom">Returned polar motion matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pom00(double xp, double yp, double sp, double[] rpom)
        {
            ValidateArray(rpom, 9, nameof(rpom));

            iauPom00(xp, yp, sp, rpom);
        }

        /// <summary>
        /// Ppp (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPpp", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPpp([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] a,
                                      [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] b,
                                      [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] apb);

        /// <summary>
        /// Add two 3D vectors.
        /// </summary>
        /// <param name="a">First vector (length 3).</param>
        /// <param name="b">Second vector (length 3).</param>
        /// <param name="apb">Returned vector <c>a+b</c> (length 3).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Ppp(double[] a, double[] b, double[] apb)
        {
            ValidateArray(a, 3, nameof(a));
            ValidateArray(b, 3, nameof(b));
            ValidateArray(apb, 3, nameof(apb));

            iauPpp(a, b, apb);
        }

        /// <summary>
        /// Ppsp (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPpsp", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPpsp([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] a,
                                       double s,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] b,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] apsb);

        /// <summary>
        /// Add a scaled 3D vector to another 3D vector.
        /// </summary>
        /// <param name="a">First vector (length 3).</param>
        /// <param name="s">Scale factor applied to <paramref name="b"/>.</param>
        /// <param name="b">Second vector (length 3).</param>
        /// <param name="apsb">Returned vector <c>a + s*b</c> (length 3).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Ppsp(double[] a, double s, double[] b, double[] apsb)
        {
            ValidateArray(a, 3, nameof(a));
            ValidateArray(b, 3, nameof(b));
            ValidateArray(apsb, 3, nameof(apsb));

            iauPpsp(a, s, b, apsb);
        }

        /// <summary>
        /// Pr00 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPr00", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPr00(double date1, double date2, ref double dpsipr, ref double depspr);

        /// <summary>
        /// Fundamental arguments (mean elements of lunar orbit).
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="dpsipr">Returned precession in longitude (radians).</param>
        /// <param name="depspr">Returned precession in obliquity (radians).</param>
        public static void Pr00(double date1, double date2, ref double dpsipr, ref double depspr)
        {
            iauPr00(date1, date2, ref dpsipr, ref depspr);
        }

        /// <summary>
        /// Prec76 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPrec76", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPrec76(double date01, double date02, double date11, double date12, ref double zeta, ref double z, ref double theta);

        /// <summary>
        /// Precession matrix from Besselian epoch to Besselian epoch.
        /// </summary>
        /// <param name="date01">Starting Besselian epoch (part 1).</param>
        /// <param name="date02">Starting Besselian epoch (part 2).</param>
        /// <param name="date11">Ending Besselian epoch (part 1).</param>
        /// <param name="date12">Ending Besselian epoch (part 2).</param>
        /// <param name="zeta">Returned Fukushima-Williams angle zeta (radians).</param>
        /// <param name="z">Returned Fukushima-Williams angle z (radians).</param>
        /// <param name="theta">Returned Fukushima-Williams angle theta (radians).</param>
        public static void Prec76(double date01, double date02, double date11, double date12, ref double zeta, ref double z, ref double theta)
        {
            iauPrec76(date01, date02, date11, date12, ref zeta, ref z, ref theta);
        }

        /// <summary>
        /// Pv2p (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPv2p", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPv2p([MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] pv, [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] p);

        /// <summary>
        /// Extract a 3D position vector from a 6D position-velocity vector.
        /// </summary>
        /// <param name="pv">Position-velocity vector (length 6).</param>
        /// <param name="p">Returned position vector (length 3).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pv2p(double[] pv, double[] p)
        {
            ValidateArray(pv, 6, nameof(pv));
            ValidateArray(p, 3, nameof(p));

            iauPv2p(pv, p);
        }

        /// <summary>
        /// Pv2s (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPv2s", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPv2s([MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] pv,
                                       ref double theta,
                                       ref double phi,
                                       ref double r,
                                       ref double td,
                                       ref double pd,
                                       ref double rd);

        /// <summary>
        /// Position-velocity vector to spherical coordinates.
        /// </summary>
        /// <param name="pv">Position-velocity vector (length 6).</param>
        /// <param name="theta">Returned longitude angle (radians).</param>
        /// <param name="phi">Returned latitude angle (radians).</param>
        /// <param name="r">Returned radius.</param>
        /// <param name="td">Returned rate of change of <paramref name="theta"/>.</param>
        /// <param name="pd">Returned rate of change of <paramref name="phi"/>.</param>
        /// <param name="rd">Returned rate of change of <paramref name="r"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pv2s(double[] pv, ref double theta, ref double phi, ref double r, ref double td, ref double pd, ref double rd)
        {
            ValidateArray(pv, 6, nameof(pv));

            iauPv2s(pv, ref theta, ref phi, ref r, ref td, ref pd, ref rd);
        }

        /// <summary>
        /// Pvdpv (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPvdpv", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPvdpv([MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] a,
                                        [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] b,
                                        [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)] double[] adb);

        /// <summary>
        /// Scalar product of two 6D position-velocity vectors.
        /// </summary>
        /// <param name="a">First position-velocity vector (length 6).</param>
        /// <param name="b">Second position-velocity vector (length 6).</param>
        /// <param name="adb">Returned dot products (length 2): position·position and velocity·velocity.</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pvdpv(double[] a, double[] b, double[] adb)
        {
            ValidateArray(a, 6, nameof(a));
            ValidateArray(b, 6, nameof(b));
            ValidateArray(adb, 2, nameof(adb));

            iauPvdpv(a, b, adb);
        }

        /// <summary>
        /// Pvm (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPvm", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPvm([MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] pv, ref double r, ref double s);

        /// <summary>
        /// Magnitude and unit vector of a 6D position-velocity vector.
        /// </summary>
        /// <param name="pv">Position-velocity vector (length 6).</param>
        /// <param name="r">Returned magnitude of the position component.</param>
        /// <param name="s">Returned magnitude of the velocity component.</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pvm(double[] pv, ref double r, ref double s)
        {
            ValidateArray(pv, 6, nameof(pv));

            iauPvm(pv, ref r, ref s);
        }

        /// <summary>
        /// Pvmpv (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPvmpv", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPvmpv([MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] a,
                                        [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] b,
                                        [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] amb);

        /// <summary>
        /// Subtract two 6D position-velocity vectors.
        /// </summary>
        /// <param name="a">First position-velocity vector (length 6).</param>
        /// <param name="b">Second position-velocity vector (length 6).</param>
        /// <param name="amb">Returned vector <c>a-b</c> (length 6).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pvmpv(double[] a, double[] b, double[] amb)
        {
            ValidateArray(a, 6, nameof(a));
            ValidateArray(b, 6, nameof(b));
            ValidateArray(amb, 6, nameof(amb));

            iauPvmpv(a, b, amb);
        }

        /// <summary>
        /// Pvppv (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPvppv", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPvppv([MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] a,
                                        [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] b,
                                        [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] apb);

        /// <summary>
        /// Add two 6D position-velocity vectors.
        /// </summary>
        /// <param name="a">First position-velocity vector (length 6).</param>
        /// <param name="b">Second position-velocity vector (length 6).</param>
        /// <param name="apb">Returned vector <c>a+b</c> (length 6).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pvppv(double[] a, double[] b, double[] apb)
        {
            ValidateArray(a, 6, nameof(a));
            ValidateArray(b, 6, nameof(b));
            ValidateArray(apb, 6, nameof(apb));

            iauPvppv(a, b, apb);
        }

        /// <summary>
        /// Pvstar (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPvstar", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauPvstar(
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] pv,
            ref double ra,
            ref double dec,
            ref double pmr,
            ref double pmd,
            ref double px,
            ref double rv);

        /// <summary>
        /// Position-velocity vector to spherical polar coordinates.
        /// </summary>
        /// <param name="pv">Position-velocity vector (length 6).</param>
        /// <param name="ra">Returned right ascension (radians).</param>
        /// <param name="dec">Returned declination (radians).</param>
        /// <param name="pmr">Returned proper motion in RA (radians/year).</param>
        /// <param name="pmd">Returned proper motion in Dec (radians/year).</param>
        /// <param name="px">Returned parallax (arcsec).</param>
        /// <param name="rv">Returned radial velocity (km/s).</param>
        /// <returns>Status code: 0 = OK, &lt;0 indicates an error condition.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        /// <returns>Return value from Pvstar</returns>
        public static int Pvstar(double[] pv, ref double ra, ref double dec, ref double pmr, ref double pmd, ref double px, ref double rv)
        {
            ValidateArray(pv, 6, nameof(pv));

            return iauPvstar(pv, ref ra, ref dec, ref pmr, ref pmd, ref px, ref rv);
        }

        /// <summary>
        /// Pvtob (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPvtob", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPvtob(double elong,
                                        double phi,
                                        double height,
                                        double xp,
                                        double yp,
                                        double sp,
                                        double theta,
                                        [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] pv);

        /// <summary>
        /// Convert site geodetic coordinates to PV.
        /// </summary>
        /// <param name="elong">Observer longitude (radians).</param>
        /// <param name="phi">Observer geodetic latitude (radians).</param>
        /// <param name="height">Observer height above ellipsoid (m).</param>
        /// <param name="xp">Polar motion X (radians).</param>
        /// <param name="yp">Polar motion Y (radians).</param>
        /// <param name="sp">TIO locator s' (radians).</param>
        /// <param name="theta">Earth rotation angle (radians).</param>
        /// <param name="pv">Returned position/velocity vector (length 6).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pvtob(double elong, double phi, double height, double xp, double yp, double sp, double theta, double[] pv)
        {
            ValidateArray(pv, 6, nameof(pv));

            iauPvtob(elong, phi, height, xp, yp, sp, theta, pv);
        }

        /// <summary>
        /// Pvu (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPvu", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPvu(double dt,
                                      [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] pv,
                                      [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] upv);

        /// <summary>
        /// Update a 6D position-velocity vector by adding a constant velocity step.
        /// </summary>
        /// <param name="dt">Time interval.</param>
        /// <param name="pv">Position-velocity vector at the start epoch (length 6).</param>
        /// <param name="upv">Returned updated position-velocity vector (length 6).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pvu(double dt, double[] pv, double[] upv)
        {
            ValidateArray(pv, 6, nameof(pv));
            ValidateArray(upv, 6, nameof(upv));

            iauPvu(dt, pv, upv);
        }

        /// <summary>
        /// Pvup (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPvup", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPvup(double dt,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] pv,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] p);

        /// <summary>
        /// Update a 6D position-velocity vector by interpolating to a different time.
        /// </summary>
        /// <param name="dt">Time interval.</param>
        /// <param name="pv">Position-velocity vector at the start epoch (length 6).</param>
        /// <param name="p">Returned position vector at the shifted epoch (length 3).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pvup(double dt, double[] pv, double[] p)
        {
            ValidateArray(pv, 6, nameof(pv));
            ValidateArray(p, 3, nameof(p));

            iauPvup(dt, pv, p);
        }

        /// <summary>
        /// Pvxpv (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPvxpv", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPvxpv([MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] a,
                                        [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] b,
                                        [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] axb);

        /// <summary>
        /// Cross product of two 6D position-velocity vectors.
        /// </summary>
        /// <param name="a">First position-velocity vector (length 6).</param>
        /// <param name="b">Second position-velocity vector (length 6).</param>
        /// <param name="axb">Returned cross product (length 6).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pvxpv(double[] a, double[] b, double[] axb)
        {
            ValidateArray(a, 6, nameof(a));
            ValidateArray(b, 6, nameof(b));
            ValidateArray(axb, 6, nameof(axb));

            iauPvxpv(a, b, axb);
        }

        /// <summary>
        /// Pxp (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauPxp", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauPxp([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] a,
                                      [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] b,
                                      [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] axb);

        /// <summary>
        /// Cross product of two 3D vectors.
        /// </summary>
        /// <param name="a">First vector (length 3).</param>
        /// <param name="b">Second vector (length 3).</param>
        /// <param name="axb">Returned cross product (length 3).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Pxp(double[] a, double[] b, double[] axb)
        {
            ValidateArray(a, 3, nameof(a));
            ValidateArray(b, 3, nameof(b));
            ValidateArray(axb, 3, nameof(axb));

            iauPxp(a, b, axb);
        }

        /// <summary>
        /// Refco (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauRefco", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauRefco(double phpa, double tc, double rh, double wl, ref double refa, ref double refb);

        /// <summary>
        /// Refraction constants from meteorology and wavelength.
        /// </summary>
        /// <param name="phpa">Pressure at observer (hPa).</param>
        /// <param name="tc">Ambient temperature (deg C).</param>
        /// <param name="rh">Relative humidity (0-1).</param>
        /// <param name="wl">Wavelength (micrometers).</param>
        /// <param name="refa">Returned refraction constant A.</param>
        /// <param name="refb">Returned refraction constant B.</param>
        public static void Refco(double phpa, double tc, double rh, double wl, ref double refa, ref double refb)
        {
            iauRefco(phpa, tc, rh, wl, ref refa, ref refb);
        }

        /// <summary>
        /// Rm2v (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauRm2v", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauRm2v([MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] r, [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] w);

        /// <summary>
        /// Convert a rotation matrix to a rotation vector.
        /// </summary>
        /// <param name="r">Rotation matrix (row-major, length 9).</param>
        /// <param name="w">Returned rotation vector (length 3).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Rm2v(double[] r, double[] w)
        {
            ValidateArray(r, 9, nameof(r));
            ValidateArray(w, 3, nameof(w));

            iauRm2v(r, w);
        }

        /// <summary>
        /// Rv2m (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauRv2m", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauRv2m([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] w, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] r);

        /// <summary>
        /// Convert a rotation vector to a rotation matrix.
        /// </summary>
        /// <param name="w">Rotation vector (length 3).</param>
        /// <param name="r">Returned rotation matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Rv2m(double[] w, double[] r)
        {
            ValidateArray(w, 3, nameof(w));
            ValidateArray(r, 9, nameof(r));

            iauRv2m(w, r);
        }

        /// <summary>
        /// Rx (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauRx", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauRx(double phi, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] r);

        /// <summary>
        /// Applies a rotation around the X-axis to a 3×3 rotation matrix by a specified angle, in radians.
        /// </summary>
        /// <remarks>This method is a P/Invoke wrapper for the SOFA library function 'iauRx'. The resulting
        /// matrix represents a right-handed rotation. The input array must have a length of at least 9 elements. No
        /// input validation is performed; passing an array of incorrect length or a null reference may result in
        /// undefined behavior.</remarks>
        /// <param name="phi">The angle of rotation, in radians, to apply about the X-axis.</param>
        /// <param name="r">A 9-element array that receives the resulting 3×3 rotation matrix in row-major order. The array must not be
        /// null.</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Rx(double phi, double[] r)
        {
            ValidateArray(r, 9, nameof(r));

            iauRx(phi, r);
        }

        /// <summary>
        /// Rxp (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauRxp", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauRxp([MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] r,
                                      [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] p,
                                      [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] rp);

        /// <summary>
        /// Multiply a 3x3 matrix by a 3D vector.
        /// </summary>
        /// <param name="r">3×3 matrix (row-major, length 9).</param>
        /// <param name="p">Vector (length 3).</param>
        /// <param name="rp">Returned vector (length 3).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Rxp(double[] r, double[] p, double[] rp)
        {
            ValidateArray(r, 9, nameof(r));
            ValidateArray(p, 3, nameof(p));
            ValidateArray(rp, 3, nameof(rp));

            iauRxp(r, p, rp);
        }

        /// <summary>
        /// Rxpv (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauRxpv", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauRxpv([MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] r,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] pv,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] rpv);

        /// <summary>
        /// Multiply a 3x3 matrix by a 6D position-velocity vector.
        /// </summary>
        /// <param name="r">3×3 matrix (row-major, length 9).</param>
        /// <param name="pv">Position-velocity vector (length 6).</param>
        /// <param name="rpv">Returned position-velocity vector (length 6).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Rxpv(double[] r, double[] pv, double[] rpv)
        {
            ValidateArray(r, 9, nameof(r));
            ValidateArray(pv, 6, nameof(pv));
            ValidateArray(rpv, 6, nameof(rpv));

            iauRxpv(r, pv, rpv);
        }

        /// <summary>
        /// Rxr (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauRxr", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauRxr([MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] a,
                                      [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] b,
                                      [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] atb);

        /// <summary>
        /// Multiply two 3x3 matrices.
        /// </summary>
        /// <param name="a">First 3×3 matrix (row-major, length 9).</param>
        /// <param name="b">Second 3×3 matrix (row-major, length 9).</param>
        /// <param name="atb">Returned product matrix (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Rxr(double[] a, double[] b, double[] atb)
        {
            ValidateArray(a, 9, nameof(a));
            ValidateArray(b, 9, nameof(b));
            ValidateArray(atb, 9, nameof(atb));

            iauRxr(a, b, atb);
        }

        /// <summary>
        /// Ry (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauRy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauRy(double theta, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] r);

        /// <summary>
        /// Applies a rotation about the Y-axis by the specified angle and returns the corresponding rotation matrix.
        /// </summary>
        /// <remarks>This method is a P/Invoke wrapper for the SOFA library function 'iauRy'. The
        /// resulting matrix can be used to transform 3D vectors by applying a rotation about the Y-axis. The input
        /// array is overwritten with the computed matrix values.</remarks>
        /// <param name="theta">The angle of rotation in radians. Positive values represent a right-handed rotation about the Y-axis.</param>
        /// <param name="r">An array of nine elements that receives the resulting 3×3 rotation matrix in row-major order. The array must
        /// not be null and must have a length of at least 9.</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Ry(double theta, double[] r)
        {
            ValidateArray(r, 9, nameof(r));

            iauRy(theta, r);
        }

        /// <summary>
        /// Rz (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauRz", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauRz(double psi, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] r);

        /// <summary>
        /// Rotates a 3x3 rotation matrix about the Z-axis by a specified angle.
        /// </summary>
        /// <remarks>The input array must contain exactly 9 elements, representing the matrix in row-major
        /// order. The method overwrites the contents of the array with the rotated matrix. This function is a wrapper
        /// for the IAU SOFA library's iauRz routine.</remarks>
        /// <param name="psi">The angle of rotation, in radians, to apply about the Z-axis.</param>
        /// <param name="r">A 9-element array representing the 3x3 rotation matrix to be rotated. The matrix is modified in place.</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Rz(double psi, double[] r)
        {
            ValidateArray(r, 9, nameof(r));

            iauRz(psi, r);
        }

        /// <summary>
        /// S00 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauS00", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauS00(double date1, double date2, double x, double y);

        /// <summary>
        /// CIO locator.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="x">CIP X coordinate.</param>
        /// <param name="y">CIP Y coordinate.</param>
        /// <returns>CIO locator s in radians.</returns>
        /// <returns>Return value from S00</returns>
        public static double S00(double date1, double date2, double x, double y)
        {
            return iauS00(date1, date2, x, y);
        }

        /// <summary>
        /// S00a (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauS00a", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauS00a(double date1, double date2);

        /// <summary>
        /// Calculates the CIO locator s, given the IAU 2000A precession-nutation model and the specified Terrestrial
        /// Time (TT) date.
        /// </summary>
        /// <remarks>This function implements the IAU 2000A precession-nutation model as defined by the
        /// International Astronomical Union. The date should be supplied as a two-part Julian Date to preserve
        /// precision, typically with <paramref name="date1"/> containing the larger value (e.g., the Julian Day Number)
        /// and <paramref name="date2"/> the fractional day. The CIO locator s is used in high-precision Earth
        /// orientation and celestial mechanics calculations.</remarks>
        /// <param name="date1">The first part of the Terrestrial Time (TT) Julian Date. This value, when combined with <paramref
        /// name="date2"/>, specifies the TT date for which to compute the CIO locator.</param>
        /// <param name="date2">The second part of the Terrestrial Time (TT) Julian Date. This value, when combined with <paramref
        /// name="date1"/>, specifies the TT date for which to compute the CIO locator.</param>
        /// <returns>The CIO locator s, in radians, for the specified TT date.</returns>
        /// <returns>Return value from S00a</returns>
        public static double S00a(double date1, double date2)
        {
            return iauS00a(date1, date2);
        }

        /// <summary>
        /// S00b (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauS00b", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauS00b(double date1, double date2);

        /// <summary>
        /// CIO locator (IAU 2000B).
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <returns>CIO locator s in radians.</returns>
        /// <returns>Return value from S00b</returns>
        public static double S00b(double date1, double date2)
        {
            return iauS00b(date1, date2);
        }

        /// <summary>
        /// S06 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauS06", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauS06(double date1, double date2, double x, double y);

        /// <summary>
        /// CIO locator (IAU 2006).
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="x">CIP X coordinate.</param>
        /// <param name="y">CIP Y coordinate.</param>
        /// <returns>CIO locator s in radians.</returns>
        /// <returns>Return value from S06</returns>
        public static double S06(double date1, double date2, double x, double y)
        {
            return iauS06(date1, date2, x, y);
        }

        /// <summary>
        /// S06a (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauS06a", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauS06a(double date1, double date2);

        /// <summary>
        /// CIO locator (IAU 2006, high precision).
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <returns>CIO locator s in radians.</returns>
        /// <returns>Return value from S06a</returns>
        public static double S06a(double date1, double date2)
        {
            return iauS06a(date1, date2);
        }

        /// <summary>
        /// S2c (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauS2c", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauS2c(double theta, double phi, [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] c);

        /// <summary>
        /// Spherical to Cartesian coordinates.
        /// </summary>
        /// <param name="theta">Longitude angle (radians).</param>
        /// <param name="phi">Latitude angle (radians).</param>
        /// <param name="c">Returned Cartesian vector (length 3).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void S2c(double theta, double phi, double[] c)
        {
            ValidateArray(c, 3, nameof(c));

            iauS2c(theta, phi, c);
        }

        /// <summary>
        /// S2p (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauS2p", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauS2p(double theta, double phi, double r, [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] p);

        /// <summary>
        /// Spherical to Cartesian polar coordinates.
        /// </summary>
        /// <param name="theta">Longitude angle (radians).</param>
        /// <param name="phi">Latitude angle (radians).</param>
        /// <param name="r">Radius.</param>
        /// <param name="p">Returned Cartesian vector (length 3).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void S2p(double theta, double phi, double r, double[] p)
        {
            ValidateArray(p, 3, nameof(p));

            iauS2p(theta, phi, r, p);
        }

        /// <summary>
        /// S2pv (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauS2pv", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauS2pv(double theta, double phi, double r, double td, double pd, double rd, [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] pv);

        /// <summary>
        /// Spherical to position-velocity vector.
        /// </summary>
        /// <param name="theta">Longitude angle (radians).</param>
        /// <param name="phi">Latitude angle (radians).</param>
        /// <param name="r">Radius.</param>
        /// <param name="td">Rate of change of <paramref name="theta"/>.</param>
        /// <param name="pd">Rate of change of <paramref name="phi"/>.</param>
        /// <param name="rd">Rate of change of <paramref name="r"/>.</param>
        /// <param name="pv">Returned position-velocity vector (length 6).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void S2pv(double theta, double phi, double r, double td, double pd, double rd, double[] pv)
        {
            ValidateArray(pv, 6, nameof(pv));

            iauS2pv(theta, phi, r, td, pd, rd, pv);
        }

        /// <summary>
        /// S2xpv (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauS2xpv", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauS2xpv(double s1,
                                        double s2,
                                        [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] pv,
                                        [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] spv);

        /// <summary>
        /// Multiply a 6D position-velocity vector by a scalar and another 6D position-velocity vector.
        /// </summary>
        /// <param name="s1">Scale factor for the position component.</param>
        /// <param name="s2">Scale factor for the velocity component.</param>
        /// <param name="pv">Input position-velocity vector (length 6).</param>
        /// <param name="spv">Returned scaled position-velocity vector (length 6).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void S2xpv(double s1, double s2, double[] pv, double[] spv)
        {
            ValidateArray(pv, 6, nameof(pv));
            ValidateArray(spv, 6, nameof(spv));

            iauS2xpv(s1, s2, pv, spv);
        }

        /// <summary>
        /// Sepp (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauSepp", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauSepp([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] a, [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] b);

        /// <summary>
        /// Separation between two 3D vectors.
        /// </summary>
        /// <param name="a">Direction 1 (length 3).</param>
        /// <param name="b">Direction 2 (length 3).</param>
        /// <returns>Separation angle (radians).</returns>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        /// <returns>Return value from Sepp</returns>
        public static double Sepp(double[] a, double[] b)
        {
            ValidateArray(a, 3, nameof(a));
            ValidateArray(b, 3, nameof(b));

            return iauSepp(a, b);
        }

        /// <summary>
        /// Seps (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauSeps", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauSeps(double al, double ap, double bl, double bp);

        /// <summary>
        /// Separation between two 2D spherical positions.
        /// </summary>
        /// <param name="al">Longitude of first position (radians).</param>
        /// <param name="ap">Latitude of first position (radians).</param>
        /// <param name="bl">Longitude of second position (radians).</param>
        /// <param name="bp">Latitude of second position (radians).</param>
        /// <returns>Separation angle (radians).</returns>
        /// <returns>Return value from Seps</returns>
        public static double Seps(double al, double ap, double bl, double bp)
        {
            return iauSeps(al, ap, bl, bp);
        }

        /// <summary>
        /// Sp00 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauSp00", CallingConvention = CallingConvention.Cdecl)]
        private static extern double iauSp00(double date1, double date2);

        /// <summary>
        /// The TIO locator (sp).
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <returns>TIO locator s' in radians.</returns>
        /// <returns>Return value from Sp00</returns>
        public static double Sp00(double date1, double date2)
        {
            return iauSp00(date1, date2);
        }

        /// <summary>
        /// Starpm (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauStarpm", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauStarpm(
            double ra1,
            double dec1,
            double pmr1,
            double pmd1,
            double px1,
            double rv1,
            double ep1a,
            double ep1b,
            double ep2a,
            double ep2b,
            ref double ra2,
            ref double dec2,
            ref double pmr2,
            ref double pmd2,
            ref double px2,
            ref double rv2);

        /// <summary>
        /// Proper motion and parallax propagation.
        /// </summary>
        /// <param name="ra1">RA at epoch 1 (radians).</param>
        /// <param name="dec1">Dec at epoch 1 (radians).</param>
        /// <param name="pmr1">Proper motion in RA (radians/year).</param>
        /// <param name="pmd1">Proper motion in Dec (radians/year).</param>
        /// <param name="px1">Parallax at epoch 1 (arcsec).</param>
        /// <param name="rv1">Radial velocity at epoch 1 (km/s).</param>
        /// <param name="ep1a">Epoch 1 (part A).</param>
        /// <param name="ep1b">Epoch 1 (part B).</param>
        /// <param name="ep2a">Epoch 2 (part A).</param>
        /// <param name="ep2b">Epoch 2 (part B).</param>
        /// <param name="ra2">Returned RA at epoch 2 (radians).</param>
        /// <param name="dec2">Returned Dec at epoch 2 (radians).</param>
        /// <param name="pmr2">Returned proper motion in RA (radians/year).</param>
        /// <param name="pmd2">Returned proper motion in Dec (radians/year).</param>
        /// <param name="px2">Returned parallax at epoch 2 (arcsec).</param>
        /// <param name="rv2">Returned radial velocity at epoch 2 (km/s).</param>
        /// <returns>
        /// Status code: 0 = OK, -1 = system error, 1 = distance overridden, 2 = excessive velocity, 4 = solution didn't converge. Else = logical OR of the previous warnings.
        /// </returns>
        /// <returns>Return value from Starpm</returns>
        public static int Starpm(double ra1, double dec1, double pmr1, double pmd1, double px1, double rv1, double ep1a, double ep1b, double ep2a, double ep2b, ref double ra2, ref double dec2, ref double pmr2, ref double pmd2, ref double px2, ref double rv2)
        {
            return iauStarpm(ra1, dec1, pmr1, pmd1, px1, rv1, ep1a, ep1b, ep2a, ep2b, ref ra2, ref dec2, ref pmr2, ref pmd2, ref px2, ref rv2);
        }

        /// <summary>
        /// Starpv (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauStarpv", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauStarpv(
            double ra,
            double dec,
            double pmr,
            double pmd,
            double px,
            double rv,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] pv);

        /// <summary>
        /// Spherical polar coordinates to position-velocity vector.
        /// </summary>
        /// <param name="ra">Right ascension (radians).</param>
        /// <param name="dec">Declination (radians).</param>
        /// <param name="pmr">Proper motion in RA (radians/year).</param>
        /// <param name="pmd">Proper motion in Dec (radians/year).</param>
        /// <param name="px">Parallax (arcsec).</param>
        /// <param name="rv">Radial velocity (km/s).</param>
        /// <param name="pv">Returned position-velocity vector (length 6).</param>
        /// <returns>Status code: 0 = OK, &lt;0 indicates an error condition.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        /// <returns>Return value from Starpv</returns>
        public static int Starpv(double ra, double dec, double pmr, double pmd, double px, double rv, double[] pv)
        {
            ValidateArray(pv, 6, nameof(pv));

            return iauStarpv(ra, dec, pmr, pmd, px, rv, pv);
        }

        /// <summary>
        /// Sxp (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauSxp", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauSxp(double s,
                                      [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] p,
                                      [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] sp);

        /// <summary>
        /// Multiply a 3D vector by a scalar.
        /// </summary>
        /// <param name="s">Scale factor.</param>
        /// <param name="p">Input vector (length 3).</param>
        /// <param name="sp">Returned scaled vector (length 3).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Sxp(double s, double[] p, double[] sp)
        {
            ValidateArray(p, 3, nameof(p));
            ValidateArray(sp, 3, nameof(sp));

            iauSxp(s, p, sp);
        }

        /// <summary>
        /// Sxpv (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauSxpv", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauSxpv(double s,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] pv,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] spv);

        /// <summary>
        /// Multiply a 6D position-velocity vector by a scalar.
        /// </summary>
        /// <param name="s">Scale factor.</param>
        /// <param name="pv">Input position-velocity vector (length 6).</param>
        /// <param name="spv">Returned scaled position-velocity vector (length 6).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Sxpv(double s, double[] pv, double[] spv)
        {
            ValidateArray(pv, 6, nameof(pv));
            ValidateArray(spv, 6, nameof(spv));

            iauSxpv(s, pv, spv);
        }

        /// <summary>
        /// Taitt (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauTaitt", CallingConvention = CallingConvention.Cdecl)]
        private static extern short iauTaitt(double tai1, double tai2, ref double tt1, ref double tt2);

        /// <summary>
        /// Time scale transformation:  International Atomic Time, TAI, to Terrestrial Time, TT.
        /// </summary>
        /// <param name="tai1">TAI as a 2-part Julian Date</param>
        /// <param name="tai2">TAI as a 2-part Julian Date</param>
        /// <param name="tt1">TT as a 2-part Julian Date</param>
        /// <param name="tt2">TT as a 2-part Julian Date</param>
        /// <returns>Status:  0 = OK</returns>
        /// <remarks>
        /// Notes:
        /// <list type="number">
        /// <item><description> tai1+tai2 is Julian Date, apportioned in any convenient way between the two arguments, for example where tai1 is the Julian Day Number and tai2 is the fraction of a day.  The returned
        /// tt1,tt2 follow suit.</description></item>
        /// </list>
        /// </remarks>
        /// <returns>Return value from Taitt</returns>
        public static short Taitt(double tai1, double tai2, ref double tt1, ref double tt2)
        {
            return iauTaitt(tai1, tai2, ref tt1, ref tt2);
        }

        /// <summary>
        /// Taiut1 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauTaiut1", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauTaiut1(double tai1, double tai2, double dta, ref double ut11, ref double ut12);

        /// <summary>
        /// Time scale transformation: International Atomic Time (TAI) to Universal Time (UT1).
        /// </summary>
        /// <remarks>
        /// The <paramref name="dta"/> argument is UT1−TAI (seconds), available from IERS tabulations.
        /// </remarks>
        /// <param name="tai1">TAI as a 2-part Julian Date (part 1).</param>
        /// <param name="tai2">TAI as a 2-part Julian Date (part 2).</param>
        /// <param name="dta">UT1−TAI in seconds.</param>
        /// <param name="ut11">Returned UT1 as a 2-part Julian Date (part 1).</param>
        /// <param name="ut12">Returned UT1 as a 2-part Julian Date (part 2).</param>
        /// <returns>Status: 0 = OK.</returns>
        /// <returns>Return value from Taiut1</returns>
        public static int Taiut1(double tai1, double tai2, double dta, ref double ut11, ref double ut12)
        {
            return iauTaiut1(tai1, tai2, dta, ref ut11, ref ut12);
        }

        /// <summary>
        /// Taiutc (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauTaiutc", CallingConvention = CallingConvention.Cdecl)]
        private static extern short iauTaiutc(double tai1, double tai2, ref double utc1, ref double utc2);

        /// <summary>
        /// Time scale transformation:  International Atomic Time, TAI, to Coordinated Universal Time, UTC.
        /// </summary>
        /// <param name="tai1">TAI as a 2-part Julian Date (Note 1)</param>
        /// <param name="tai2">TAI as a 2-part Julian Date (Note 1)</param>
        /// <param name="utc1">UTC as a 2-part quasi Julian Date (Notes 1-3)</param>
        /// <param name="utc2">UTC as a 2-part quasi Julian Date (Notes 1-3)</param>
        /// <returns>Status: +1 = dubious year (Note 4), 0 = OK, -1 = unacceptable date</returns>
        /// <remarks>
        /// Notes:
        /// <list type="number">
        /// <item><description>tai1+tai2 is Julian Date, apportioned in any convenient way between the two arguments, for example where tai1 is the Julian Day Number and tai2 is the fraction of a day.  The returned utc1
        /// and utc2 form an analogous pair, except that a special convention is used, to deal with the problem of leap seconds - see the next note.</description></item>
        /// <item><description>JD cannot unambiguously represent UTC during a leap second unless special measures are taken.  The convention in the present function is that the JD day represents UTC days whether the
        /// length is 86399, 86400 or 86401 SI seconds.  In the 1960-1972 era there were smaller jumps (in either direction) each time the linear UTC(TAI) expression was changed, and these "mini-leaps are also included in the SOFA convention.</description></item>
        /// <item><description>The function iauD2dtf can be used to transform the UTC quasi-JD into calendar date and clock time, including UTC leap second handling.</description></item>
        /// <item><description>The warning status "dubious year" flags UTCs that predate the introduction of the time scale or that are too far in the future to be trusted.  See iauDat for further details.</description></item>
        /// </list>
        /// </remarks>
        /// <returns>Return value from Taiutc</returns>
        public static short Taiutc(double tai1, double tai2, ref double utc1, ref double utc2)
        {
            return iauTaiutc(tai1, tai2, ref utc1, ref utc2);
        }

        /// <summary>
        /// Tcbtdb (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauTcbtdb", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauTcbtdb(double tcb1, double tcb2, ref double tdb1, ref double tdb2);

        /// <summary>
        /// Time scale transformation: Barycentric Coordinate Time (TCB) to Barycentric Dynamical Time (TDB).
        /// </summary>
        /// <remarks>
        /// Uses the conventional linear relationship adopted by the IAU (2006) to keep TDB approximately centered on TT.
        /// </remarks>
        /// <param name="tcb1">TCB as a 2-part Julian Date (part 1).</param>
        /// <param name="tcb2">TCB as a 2-part Julian Date (part 2).</param>
        /// <param name="tdb1">Returned TDB as a 2-part Julian Date (part 1).</param>
        /// <param name="tdb2">Returned TDB as a 2-part Julian Date (part 2).</param>
        /// <returns>Status: 0 = OK.</returns>
        /// <returns>Return value from Tcbtdb</returns>
        public static int Tcbtdb(double tcb1, double tcb2, ref double tdb1, ref double tdb2)
        {
            return iauTcbtdb(tcb1, tcb2, ref tdb1, ref tdb2);
        }

        /// <summary>
        /// Tcgtt (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauTcgtt", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauTcgtt(double tcg1, double tcg2, ref double tt1, ref double tt2);

        /// <summary>
        /// Time scale transformation: Geocentric Coordinate Time (TCG) to Terrestrial Time (TT).
        /// </summary>
        /// <param name="tcg1">TCG as a 2-part Julian Date (part 1).</param>
        /// <param name="tcg2">TCG as a 2-part Julian Date (part 2).</param>
        /// <param name="tt1">Returned TT as a 2-part Julian Date (part 1).</param>
        /// <param name="tt2">Returned TT as a 2-part Julian Date (part 2).</param>
        /// <returns>Status: 0 = OK.</returns>
        /// <returns>Return value from Tcgtt</returns>
        public static int Tcgtt(double tcg1, double tcg2, ref double tt1, ref double tt2)
        {
            return iauTcgtt(tcg1, tcg2, ref tt1, ref tt2);
        }

        /// <summary>
        /// Tdbtcb (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauTdbtcb", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauTdbtcb(double tdb1, double tdb2, ref double tcb1, ref double tcb2);

        /// <summary>
        /// Time scale transformation: Barycentric Dynamical Time (TDB) to Terrestrial Coordinate Time (TCB).
        /// </summary>
        /// <param name="tdb1">TDB as a 2-part Julian Date (part 1).</param>
        /// <param name="tdb2">TDB as a 2-part Julian Date (part 2).</param>
        /// <param name="tcb1">Returned TCB as a 2-part Julian Date (part 1).</param>
        /// <param name="tcb2">Returned TCB as a 2-part Julian Date (part 2).</param>
        /// <returns>Status code: 0 = OK.</returns>
        /// <returns>Return value from Tdbtcb</returns>
        public static int Tdbtcb(double tdb1, double tdb2, ref double tcb1, ref double tcb2)
        {
            return iauTdbtcb(tdb1, tdb2, ref tcb1, ref tcb2);
        }

        /// <summary>
        /// Tdbtt (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauTdbtt", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauTdbtt(double tdb1, double tdb2, double dtr, ref double tt1, ref double tt2);

        /// <summary>
        /// Time scale transformation: Barycentric Dynamical Time (TDB) to Terrestrial Time (TT).
        /// </summary>
        /// <remarks>
        /// The <paramref name="dtr"/> argument is the TDB−TT quasi-periodic component (seconds), obtainable via a time ephemeris
        /// or a model such as <see cref="Dtdb"/>.
        /// </remarks>
        /// <param name="tdb1">TDB as a 2-part Julian Date (part 1).</param>
        /// <param name="tdb2">TDB as a 2-part Julian Date (part 2).</param>
        /// <param name="dtr">TDB−TT in seconds.</param>
        /// <param name="tt1">Returned TT as a 2-part Julian Date (part 1).</param>
        /// <param name="tt2">Returned TT as a 2-part Julian Date (part 2).</param>
        /// <returns>Status: 0 = OK.</returns>
        /// <returns>Return value from Tdbtt</returns>
        public static int Tdbtt(double tdb1, double tdb2, double dtr, ref double tt1, ref double tt2)
        {
            return iauTdbtt(tdb1, tdb2, dtr, ref tt1, ref tt2);
        }

        /// <summary>
        /// Tf2a (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauTf2a", CallingConvention = CallingConvention.Cdecl)]
        private static extern short iauTf2a(char s, short ihour, short imin, double sec, ref double rad);

        /// <summary>
        /// Convert hours, minutes, seconds to radians.
        /// </summary>
        /// <param name="s">sign:  '-' = negative, otherwise positive</param>
        /// <param name="ihour">Hours</param>
        /// <param name="imin">Minutes</param>
        /// <param name="sec">Seconds</param>
        /// <param name="rad">Angle in radians</param>
        /// <returns>Status:  0 = OK, 1 = ihour outside range 0-23, 2 = imin outside range 0-59, 3 = sec outside range 0-59.999...</returns>
        /// <remarks>
        /// Notes:
        /// <list type="number">
        /// <item><description>The result is computed even if any of the range checks fail.</description></item>
        /// <item><description>Negative ihour, imin and/or sec produce a warning status, but the absolute value is used in the conversion.</description></item>
        /// <item><description>If there are multiple errors, the status value reflects only the first, the smallest taking precedence.</description></item>
        /// </list>
        /// </remarks>
        /// <returns>Return value from Tf2a</returns>
        public static short Tf2a(char s, short ihour, short imin, double sec, ref double rad)
        {
            return iauTf2a(s, ihour, imin, sec, ref rad);
        }

        /// <summary>
        /// Tf2d (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauTf2d", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauTf2d(char s, int ihour, int imin, double sec, ref double days);

        /// <summary>
        /// Convert hours, minutes, seconds to days.
        /// </summary>
        /// <param name="s">Sign ('-' = negative, otherwise positive).</param>
        /// <param name="ihour">Hours.</param>
        /// <param name="imin">Minutes.</param>
        /// <param name="sec">Seconds.</param>
        /// <param name="days">Returned interval in days.</param>
        /// <returns>Status code: 0 = OK, &lt;0 = error.</returns>
        /// <returns>Return value from Tf2d</returns>
        public static int Tf2d(char s, int ihour, int imin, double sec, ref double days)
        {
            return iauTf2d(s, ihour, imin, sec, ref days);
        }

        /// <summary>
        /// Tpors (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauTpors", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauTpors(double xi, double eta, double a, double b, ref double a01, ref double b01, ref double a02, ref double b02);

        /// <summary>
        /// Gnomonic projection: (ξ,η) to (α,δ).
        /// </summary>
        /// <param name="xi">Projection coordinate ξ.</param>
        /// <param name="eta">Projection coordinate η.</param>
        /// <param name="a">RA of tangent point (radians).</param>
        /// <param name="b">Dec of tangent point (radians).</param>
        /// <param name="a01">Returned solution 1 RA (radians).</param>
        /// <param name="b01">Returned solution 1 Dec (radians).</param>
        /// <param name="a02">Returned solution 2 RA (radians).</param>
        /// <param name="b02">Returned solution 2 Dec (radians).</param>
        /// <returns>Status code: 0 = OK, &lt;0 indicates an error condition.</returns>
        /// <returns>Return value from Tpors</returns>
        public static int Tpors(double xi, double eta, double a, double b, ref double a01, ref double b01, ref double a02, ref double b02)
        {
            return iauTpors(xi, eta, a, b, ref a01, ref b01, ref a02, ref b02);
        }

        /// <summary>
        /// Tporv (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauTporv", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauTporv(
            double xi,
            double eta,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] v,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] v01,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] v02);

        /// <summary>
        /// Gnomonic projection: (ξ,η) to unit vector.
        /// </summary>
        /// <param name="xi">Projection coordinate ξ.</param>
        /// <param name="eta">Projection coordinate η.</param>
        /// <param name="v">Direction cosines of tangent point (length 3).</param>
        /// <param name="v01">Returned unit vector solution 1 (length 3).</param>
        /// <param name="v02">Returned unit vector solution 2 (length 3).</param>
        /// <returns>Status code: 0 = OK, &lt;0 indicates an error condition.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        /// <returns>Return value from Tporv</returns>
        public static int Tporv(double xi, double eta, double[] v, double[] v01, double[] v02)
        {
            ValidateArray(v, 3, nameof(v));
            ValidateArray(v01, 3, nameof(v01));
            ValidateArray(v02, 3, nameof(v02));

            return iauTporv(xi, eta, v, v01, v02);
        }

        /// <summary>
        /// Tpsts (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauTpsts", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauTpsts(double xi, double eta, double a0, double b0, ref double a, ref double b);

        /// <summary>
        /// Gnomonic projection: (α,δ) to (ξ,η).
        /// </summary>
        /// <param name="xi">Projection coordinate ξ of tangent point.</param>
        /// <param name="eta">Projection coordinate η of tangent point.</param>
        /// <param name="a0">RA of target point (radians).</param>
        /// <param name="b0">Dec of target point (radians).</param>
        /// <param name="a">Returned ξ coordinate.</param>
        /// <param name="b">Returned η coordinate.</param>
        public static void Tpsts(double xi, double eta, double a0, double b0, ref double a, ref double b)
        {
            iauTpsts(xi, eta, a0, b0, ref a, ref b);
        }

        /// <summary>
        /// Tpstv (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauTpstv", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauTpstv(
            double xi,
            double eta,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] v0,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] v);

        /// <summary>
        /// Gnomonic projection: unit vector to (ξ,η).
        /// </summary>
        /// <param name="xi">Projection coordinate ξ of tangent point.</param>
        /// <param name="eta">Projection coordinate η of tangent point.</param>
        /// <param name="v0">Unit vector of tangent point (length 3).</param>
        /// <param name="v">Returned unit vector on projection plane (length 3).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Tpstv(double xi, double eta, double[] v0, double[] v)
        {
            ValidateArray(v0, 3, nameof(v0));
            ValidateArray(v, 3, nameof(v));

            iauTpstv(xi, eta, v0, v);
        }

        /// <summary>
        /// Tpxes (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauTpxes", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauTpxes(double a, double b, double a0, double b0, ref double xi, ref double eta);

        /// <summary>
        /// Gnomonic projection: (α,δ) to (ξ,η) plane coordinates.
        /// </summary>
        /// <param name="a">RA of target point (radians).</param>
        /// <param name="b">Dec of target point (radians).</param>
        /// <param name="a0">RA of tangent point (radians).</param>
        /// <param name="b0">Dec of tangent point (radians).</param>
        /// <param name="xi">Returned ξ coordinate.</param>
        /// <param name="eta">Returned η coordinate.</param>
        /// <returns>Status code: 0 = OK, &lt;0 indicates an error condition.</returns>
        /// <returns>Return value from Tpxes</returns>
        public static int Tpxes(double a, double b, double a0, double b0, ref double xi, ref double eta)
        {
            return iauTpxes(a, b, a0, b0, ref xi, ref eta);
        }

        /// <summary>
        /// Tpxev (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauTpxev", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauTpxev(
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] v,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] v0,
            ref double xi,
            ref double eta);

        /// <summary>
        /// Gnomonic projection: unit vector to (ξ,η) plane coordinates.
        /// </summary>
        /// <param name="v">Unit vector of target point (length 3).</param>
        /// <param name="v0">Unit vector of tangent point (length 3).</param>
        /// <param name="xi">Returned ξ coordinate.</param>
        /// <param name="eta">Returned η coordinate.</param>
        /// <returns>Status code: 0 = OK, &lt;0 indicates an error condition.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        /// <returns>Return value from Tpxev</returns>
        public static int Tpxev(double[] v, double[] v0, ref double xi, ref double eta)
        {
            ValidateArray(v, 3, nameof(v));
            ValidateArray(v0, 3, nameof(v0));

            return iauTpxev(v, v0, ref xi, ref eta);
        }

        /// <summary>
        /// Tr (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauTr", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauTr([MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] r, [MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] rt);

        /// <summary>
        /// Transpose a 3x3 matrix.
        /// </summary>
        /// <param name="r">Input 3×3 matrix (row-major, length 9).</param>
        /// <param name="rt">Returned transpose (row-major, length 9).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Tr(double[] r, double[] rt)
        {
            ValidateArray(r, 9, nameof(r));
            ValidateArray(rt, 9, nameof(rt));

            iauTr(r, rt);
        }

        /// <summary>
        /// Trxp (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauTrxp", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauTrxp([MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] r,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] p,
                                       [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] trp);

        /// <summary>
        /// Multiply the transpose of a 3x3 matrix by a 3D vector.
        /// </summary>
        /// <param name="r">3×3 matrix (row-major, length 9).</param>
        /// <param name="p">Vector (length 3).</param>
        /// <param name="trp">Returned vector (length 3).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Trxp(double[] r, double[] p, double[] trp)
        {
            ValidateArray(r, 9, nameof(r));
            ValidateArray(p, 3, nameof(p));
            ValidateArray(trp, 3, nameof(trp));

            iauTrxp(r, p, trp);
        }

        /// <summary>
        /// Trxpv (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauTrxpv", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauTrxpv([MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] r,
                                        [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] pv,
                                        [MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] trpv);

        /// <summary>
        /// Multiply the transpose of a 3x3 matrix by a 6D position-velocity vector.
        /// </summary>
        /// <param name="r">3×3 matrix (row-major, length 9).</param>
        /// <param name="pv">Position-velocity vector (length 6).</param>
        /// <param name="trpv">Returned position-velocity vector (length 6).</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Trxpv(double[] r, double[] pv, double[] trpv)
        {
            ValidateArray(r, 9, nameof(r));
            ValidateArray(pv, 6, nameof(pv));
            ValidateArray(trpv, 6, nameof(trpv));

            iauTrxpv(r, pv, trpv);
        }

        /// <summary>
        /// Tttai (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauTttai", CallingConvention = CallingConvention.Cdecl)]
        private static extern short iauTttai(double tt1, double tt2, ref double tai1, ref double tai2);

        /// <summary>
        /// Time scale transformation:  Terrestrial Time, TT, to International Atomic Time, TAI.
        /// </summary>
        /// <param name="tt1">TT as a 2-part Julian Date</param>
        /// <param name="tt2">TT as a 2-part Julian Date</param>
        /// <param name="tai1">TAI as a 2-part Julian Date</param>
        /// <param name="tai2">TAI as a 2-part Julian Date</param>
        /// <returns>Status:  0 = OK</returns>
        /// <remarks>
        /// Note
        /// <list type="number">
        /// <item><description>tt1+tt2 is Julian Date, apportioned in any convenient way between the two arguments, for example where tt1 is the Julian Day Number and tt2 is the fraction of a day.  The returned tai1,tai2 follow suit.</description></item>
        /// </list>
        /// </remarks>
        /// <returns>Return value from Tttai</returns>
        public static short Tttai(double tt1, double tt2, ref double tai1, ref double tai2)
        {
            return iauTttai(tt1, tt2, ref tai1, ref tai2);
        }

        /// <summary>
        /// Tttcg (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauTttcg", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauTttcg(double tt1, double tt2, ref double tcg1, ref double tcg2);

        /// <summary>
        /// Time scale transformation: Terrestrial Time (TT) to Geocentric Coordinate Time (TCG).
        /// </summary>
        /// <param name="tt1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="tt2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="tcg1">Returned TCG as a 2-part Julian Date (part 1).</param>
        /// <param name="tcg2">Returned TCG as a 2-part Julian Date (part 2).</param>
        /// <returns>Status: 0 = OK.</returns>
        /// <returns>Return value from Tttcg</returns>
        public static int Tttcg(double tt1, double tt2, ref double tcg1, ref double tcg2)
        {
            return iauTttcg(tt1, tt2, ref tcg1, ref tcg2);
        }

        /// <summary>
        /// Tttdb (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauTttdb", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauTttdb(double tt1, double tt2, double dtr, ref double tdb1, ref double tdb2);

        /// <summary>
        /// Time scale transformation: Terrestrial Time (TT) to Barycentric Dynamical Time (TDB).
        /// </summary>
        /// <remarks>
        /// The <paramref name="dtr"/> argument is the TDB−TT quasi-periodic component (seconds), obtainable via a time ephemeris
        /// or a model such as <see cref="Dtdb"/>.
        /// </remarks>
        /// <param name="tt1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="tt2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="dtr">TDB−TT in seconds.</param>
        /// <param name="tdb1">Returned TDB as a 2-part Julian Date (part 1).</param>
        /// <param name="tdb2">Returned TDB as a 2-part Julian Date (part 2).</param>
        /// <returns>Status: 0 = OK.</returns>
        /// <returns>Return value from Tttdb</returns>
        public static int Tttdb(double tt1, double tt2, double dtr, ref double tdb1, ref double tdb2)
        {
            return iauTttdb(tt1, tt2, dtr, ref tdb1, ref tdb2);
        }

        /// <summary>
        /// Ttut1 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauTtut1", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauTtut1(double tt1, double tt2, double dt, ref double ut11, ref double ut12);

        /// <summary>
        /// Time scale transformation: Terrestrial Time (TT) to Universal Time (UT1).
        /// </summary>
        /// <remarks>
        /// The <paramref name="dt"/> argument is classical ΔT (TT−UT1) in seconds.
        /// </remarks>
        /// <param name="tt1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="tt2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="dt">TT−UT1 in seconds.</param>
        /// <param name="ut11">Returned UT1 as a 2-part Julian Date (part 1).</param>
        /// <param name="ut12">Returned UT1 as a 2-part Julian Date (part 2).</param>
        /// <returns>Status: 0 = OK.</returns>
        /// <returns>Return value from Ttut1</returns>
        public static int Ttut1(double tt1, double tt2, double dt, ref double ut11, ref double ut12)
        {
            return iauTtut1(tt1, tt2, dt, ref ut11, ref ut12);
        }

        /// <summary>
        /// Ut1tai (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauUt1tai", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauUt1tai(double ut11, double ut12, double dta, ref double tai1, ref double tai2);

        /// <summary>
        /// Time scale transformation: Universal Time (UT1) to International Atomic Time (TAI).
        /// </summary>
        /// <remarks>
        /// The <paramref name="dta"/> argument is UT1−TAI (seconds), available from IERS tabulations.
        /// </remarks>
        /// <param name="ut11">UT1 as a 2-part Julian Date (part 1).</param>
        /// <param name="ut12">UT1 as a 2-part Julian Date (part 2).</param>
        /// <param name="dta">UT1−TAI in seconds.</param>
        /// <param name="tai1">Returned TAI as a 2-part Julian Date (part 1).</param>
        /// <param name="tai2">Returned TAI as a 2-part Julian Date (part 2).</param>
        /// <returns>Status: 0 = OK.</returns>
        /// <returns>Return value from Ut1tai</returns>
        public static int Ut1tai(double ut11, double ut12, double dta, ref double tai1, ref double tai2)
        {
            return iauUt1tai(ut11, ut12, dta, ref tai1, ref tai2);
        }

        /// <summary>
        /// Ut1tt (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauUt1tt", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauUt1tt(double ut11, double ut12, double dt, ref double tt1, ref double tt2);

        /// <summary>
        /// Time scale transformation: Universal Time (UT1) to Terrestrial Time (TT).
        /// </summary>
        /// <remarks>
        /// The <paramref name="dt"/> argument is classical ΔT (TT−UT1) in seconds.
        /// </remarks>
        /// <param name="ut11">UT1 as a 2-part Julian Date (part 1).</param>
        /// <param name="ut12">UT1 as a 2-part Julian Date (part 2).</param>
        /// <param name="dt">TT−UT1 in seconds.</param>
        /// <param name="tt1">Returned TT as a 2-part Julian Date (part 1).</param>
        /// <param name="tt2">Returned TT as a 2-part Julian Date (part 2).</param>
        /// <returns>Status: 0 = OK.</returns>
        /// <returns>Return value from Ut1tt</returns>
        public static int Ut1tt(double ut11, double ut12, double dt, ref double tt1, ref double tt2)
        {
            return iauUt1tt(ut11, ut12, dt, ref tt1, ref tt2);
        }

        /// <summary>
        /// Ut1utc (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauUt1utc", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauUt1utc(double ut11, double ut12, double dut1, ref double utc1, ref double utc2);

        /// <summary>
        /// Time scale transformation: Universal Time (UT1) to Coordinated Universal Time (UTC).
        /// </summary>
        /// <remarks>
        /// Uses a quasi-JD UTC convention to handle leap seconds (see SOFA notes for details).
        /// </remarks>
        /// <param name="ut11">UT1 as a 2-part Julian Date (part 1).</param>
        /// <param name="ut12">UT1 as a 2-part Julian Date (part 2).</param>
        /// <param name="dut1">UT1−UTC in seconds.</param>
        /// <param name="utc1">Returned UTC as a 2-part quasi Julian Date (part 1).</param>
        /// <param name="utc2">Returned UTC as a 2-part quasi Julian Date (part 2).</param>
        /// <returns>Status: +1 = dubious year, 0 = OK, -1 = unacceptable date.</returns>
        /// <returns>Return value from Ut1utc</returns>
        public static int Ut1utc(double ut11, double ut12, double dut1, ref double utc1, ref double utc2)
        {
            return iauUt1utc(ut11, ut12, dut1, ref utc1, ref utc2);
        }

        /// <summary>
        /// Utctai (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauUtctai", CallingConvention = CallingConvention.Cdecl)]
        private static extern short iauUtctai(double utc1, double utc2, ref double tai1, ref double tai2);

        /// <summary>
        /// Time scale transformation:  Coordinated Universal Time, UTC, to International Atomic Time, TAI.
        /// </summary>
        /// <param name="utc1">UTC as a 2-part quasi Julian Date (Notes 1-4)</param>
        /// <param name="utc2">UTC as a 2-part quasi Julian Date (Notes 1-4)</param>
        /// <param name="tai1">TAI as a 2-part Julian Date (Note 5)</param>
        /// <param name="tai2">TAI as a 2-part Julian Date (Note 5)</param>
        /// <returns>Status: +1 = dubious year (Note 3) 0 = OK -1 = unacceptable date</returns>
        /// <remarks>
        /// Notes:
        /// <list type="number">
        /// <item><description>utc1+utc2 is quasi Julian Date (see Note 2), apportioned in any convenient way between the two arguments, for example where utc1 is the Julian Day Number and utc2 is the fraction of a day.</description></item>
        /// <item><description>JD cannot unambiguously represent UTC during a leap second unless special measures are taken.  The convention in the present function is that the JD day represents UTC days whether the
        /// length is 86399, 86400 or 86401 SI seconds.  In the 1960-1972 era there were smaller jumps (in either direction) each time the linear UTC(TAI) expression was changed, and these "mini-leaps" are also included in the SOFA convention.</description></item>
        /// <item><description>The warning status "dubious year" flags UTCs that predate the introduction of the time scale or that are too far in the future to be trusted.  See iauDat for further details.</description></item>
        /// <item><description>The function iauDtf2d converts from calendar date and time of day into 2-part Julian Date, and in the case of UTC implements the leap-second-ambiguity convention described above.</description></item>
        /// <item><description>The returned TAI1,TAI2 are such that their sum is the TAI Julian Date.</description></item>
        /// </list>
        /// </remarks>
        /// <returns>Return value from Utctai</returns>
        public static short Utctai(double utc1, double utc2, ref double tai1, ref double tai2)
        {
            return iauUtctai(utc1, utc2, ref tai1, ref tai2);
        }

        /// <summary>
        /// Utcut1 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauUtcut1", CallingConvention = CallingConvention.Cdecl)]
        private static extern int iauUtcut1(double utc1, double utc2, double dut1, ref double ut11, ref double ut12);

        /// <summary>
        /// Time scale transformation: Coordinated Universal Time (UTC) to Universal Time (UT1).
        /// </summary>
        /// <remarks>
        /// The caller must supply <paramref name="dut1"/> (UT1−UTC) appropriate for the given UTC.
        /// </remarks>
        /// <param name="utc1">UTC as a 2-part quasi Julian Date (part 1).</param>
        /// <param name="utc2">UTC as a 2-part quasi Julian Date (part 2).</param>
        /// <param name="dut1">UT1−UTC in seconds.</param>
        /// <param name="ut11">Returned UT1 as a 2-part Julian Date (part 1).</param>
        /// <param name="ut12">Returned UT1 as a 2-part Julian Date (part 2).</param>
        /// <returns>Status: +1 = dubious year, 0 = OK, -1 = unacceptable date.</returns>
        /// <returns>Return value from Utcut1</returns>
        public static int Utcut1(double utc1, double utc2, double dut1, ref double ut11, ref double ut12)
        {
            return iauUtcut1(utc1, utc2, dut1, ref ut11, ref ut12);
        }

        /// <summary>
        /// Xy06 (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauXy06", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauXy06(double date1, double date2, ref double x, ref double y);

        /// <summary>
        /// CIO RA and related parameters.
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="x">Returned CIP X coordinate.</param>
        /// <param name="y">Returned CIP Y coordinate.</param>
        public static void Xy06(double date1, double date2, ref double x, ref double y)
        {
            iauXy06(date1, date2, ref x, ref y);
        }

        /// <summary>
        /// Xys00a (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauXys00a", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauXys00a(double date1, double date2, ref double x, ref double y, ref double s);

        /// <summary>
        /// CIO coordinates (IAU 2000A).
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="x">Returned CIP X coordinate.</param>
        /// <param name="y">Returned CIP Y coordinate.</param>
        /// <param name="s">Returned CIO locator s.</param>
        public static void Xys00a(double date1, double date2, ref double x, ref double y, ref double s)
        {
            iauXys00a(date1, date2, ref x, ref y, ref s);
        }

        /// <summary>
        /// Xys00b (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauXys00b", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauXys00b(double date1, double date2, ref double x, ref double y, ref double s);

        /// <summary>
        /// CIO coordinates (IAU 2000B).
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="x">Returned CIP X coordinate.</param>
        /// <param name="y">Returned CIP Y coordinate.</param>
        /// <param name="s">Returned CIO locator s.</param>
        public static void Xys00b(double date1, double date2, ref double x, ref double y, ref double s)
        {
            iauXys00b(date1, date2, ref x, ref y, ref s);
        }

        /// <summary>
        /// Xys06a (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauXys06a", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauXys06a(double date1, double date2, ref double x, ref double y, ref double s);

        /// <summary>
        /// CIO coordinates (IAU 2006/2000A, high precision).
        /// </summary>
        /// <param name="date1">TT as a 2-part Julian Date (part 1).</param>
        /// <param name="date2">TT as a 2-part Julian Date (part 2).</param>
        /// <param name="x">Returned CIP X coordinate.</param>
        /// <param name="y">Returned CIP Y coordinate.</param>
        /// <param name="s">Returned CIO locator s.</param>
        public static void Xys06a(double date1, double date2, ref double x, ref double y, ref double s)
        {
            iauXys06a(date1, date2, ref x, ref y, ref s);
        }

        /// <summary>
        /// Zp (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauZp", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauZp([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] p);

        /// <summary>
        /// Zero a 3-element position vector.
        /// </summary>
        /// <param name="p">A 3-element array that receives the zero vector.</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Zp(double[] p)
        {
            ValidateArray(p, 3, nameof(p));

            iauZp(p);
        }

        /// <summary>
        /// Zpv (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauZpv", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauZpv([MarshalAs(UnmanagedType.LPArray, SizeConst = 6)] double[] pv);

        /// <summary>
        /// Initialize a position-velocity vector to zero.
        /// </summary>
        /// <param name="pv">A 6-element array that receives the zero position-velocity vector.</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Zpv(double[] pv)
        {
            ValidateArray(pv, 6, nameof(pv));

            iauZpv(pv);
        }

        /// <summary>
        /// Zr (P/Invoke the SOFA library).
        /// </summary>
        [DllImport(SOFA_LIBRARY, EntryPoint = "iauZr", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iauZr([MarshalAs(UnmanagedType.LPArray, SizeConst = 9)] double[] r);

        /// <summary>
        /// Initialize a rotation matrix to the null (all zeros) matrix.
        /// </summary>
        /// <param name="r">A 9-element array that receives the 3×3 null rotation matrix in row-major order.</param>
        /// <exception cref="ArgumentNullException">Thrown if any array parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown if any array parameter has incorrect length.</exception>
        public static void Zr(double[] r)
        {
            ValidateArray(r, 9, nameof(r));

            iauZr(r);
        }

        #endregion

    }
}
