using ASCOM.Tools;
using System;
using System.Net.NetworkInformation;
using System.Text;
using Xunit;

namespace SOFATests
{
    /// <summary>
    /// xUnit tests for SOFA class functions based on t_sofa_c.c test cases.
    /// Tests include functions from FunctionSubset.txt for coordinate transformations,
    /// time conversions, and astronomical calculations.
    /// </summary>

    public class Sofa_t_fk425
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void Fk425_ValidInput_ReturnsFK5Coordinates()
        {
            // Arrange
            double r1950 = 0.07626899753879587532;
            double d1950 = -1.137405378399605780;
            double dr1950 = 0.1973749217849087460e-4;
            double dd1950 = 0.5659714913272723189e-5;
            double p1950 = 0.134;
            double v1950 = 8.7;

            // Act
            double r2000 = 0, d2000 = 0, dr2000 = 0, dd2000 = 0, p2000 = 0, v2000 = 0;
            Sofa.Fk425(r1950, d1950, dr1950, dd1950, p1950, v1950,
                ref r2000, ref d2000, ref dr2000, ref dd2000, ref p2000, ref v2000);

            // Assert
            Assert.Equal(0.08757989933556446040, r2000, TOLERANCE);
            Assert.Equal(-1.132279113042091895, d2000, 1e-12);
            Assert.Equal(0.1953670614474396139e-4, dr2000, 1e-17);
            Assert.Equal(0.5637686678659640164e-5, dd2000, 1e-18);
            Assert.Equal(0.1339919950582767871, p2000, 1e-13);
            Assert.Equal(8.736999669183529069, v2000, 1e-12);
        }
    }

    public class Sofa_t_fk45z
    {
        private const double TOLERANCE = 1e-15;

        [Fact]
        public void Fk45z_ValidInput_ReturnsFK5Coordinates()
        {
            // Arrange
            double r1950 = 0.01602284975382960982;
            double d1950 = -0.1164347929099906024;
            double bepoch = 1954.677617625256806;

            // Act
            double r2000 = 0, d2000 = 0;
            Sofa.Fk45z(r1950, d1950, bepoch, ref r2000, ref d2000);

            // Assert
            Assert.Equal(0.02719295911606862303, r2000, TOLERANCE);
            Assert.Equal(-0.1115766001565926892, d2000, 1e-13);
        }
    }

    public class Sofa_t_fk524
    {
        private const double TOLERANCE = 1e-13;

        [Fact]
        public void Fk524_ValidInput_ReturnsFK4Coordinates()
        {
            // Arrange
            double r2000 = 0.8723503576487275595;
            double d2000 = -0.7517076365138887672;
            double dr2000 = 0.2019447755430472323e-4;
            double dd2000 = 0.3541563940505160433e-5;
            double p2000 = 0.1559;
            double v2000 = 86.87;

            // Act
            double r1950 = 0, d1950 = 0, dr1950 = 0, dd1950 = 0, p1950 = 0, v1950 = 0;
            Sofa.Fk524(r2000, d2000, dr2000, dd2000, p2000, v2000,
                ref r1950, ref d1950, ref dr1950, ref dd1950, ref p1950, ref v1950);

            // Assert
            Assert.Equal(0.8636359659799603487, r1950, TOLERANCE);
            Assert.Equal(-0.7550281733160843059, d1950, TOLERANCE);
            Assert.Equal(0.2023628192747172486e-4, dr1950, 1e-17);
            Assert.Equal(0.3624459754935334718e-5, dd1950, 1e-18);
            Assert.Equal(0.1560079963299390241, p1950, 1e-13);
            Assert.Equal(86.79606353469163751, v1950, 1e-11);
        }
    }

    public class Sofa_t_fk52h
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void Fk52h_ValidInput_ReturnsHipparcosCatalogCoordinates()
        {
            // Arrange
            double r5 = 1.76779433;
            double d5 = -0.2917517103;
            double dr5 = -1.91851572e-7;
            double dd5 = -5.8468475e-6;
            double px5 = 0.379210;
            double rv5 = -7.6;

            // Act
            double rh = 0, dh = 0, drh = 0, ddh = 0, pxh = 0, rvh = 0;
            Sofa.Fk52h(r5, d5, dr5, dd5, px5, rv5, ref rh, ref dh, ref drh, ref ddh, ref pxh, ref rvh);

            // Assert
            Assert.Equal(1.767794226299947632, rh, TOLERANCE);
            Assert.Equal(-0.2917516070530391757, dh, TOLERANCE);
            Assert.Equal(-0.1961874125605721270e-6, drh, 1e-19);
            Assert.Equal(-0.58459905176693911e-5, ddh, 1e-19);
            Assert.Equal(0.37921, pxh, TOLERANCE);
            Assert.Equal(-7.6000000940000254, rvh, 1e-11);
        }
    }

    public class Sofa_t_fk54z
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void Fk54z_ValidInput_ReturnsFK4Coordinates()
        {
            // Arrange
            double r2000 = 0.02719026625066316119;
            double d2000 = -0.1115815170738754813;
            double bepoch = 1954.677308160316374;

            // Act
            double r1950 = 0, d1950 = 0, dr1950 = 0, dd1950 = 0;
            Sofa.Fk54z(r2000, d2000, bepoch, ref r1950, ref d1950, ref dr1950, ref dd1950);

            // Assert
            Assert.Equal(0.01602015588390065476, r1950, TOLERANCE);
            Assert.Equal(-0.1164397101110765346, d1950, 1e-13);
            Assert.Equal(-0.1175712648471090704e-7, dr1950, 1e-20);
            Assert.Equal(0.2108109051316431056e-7, dd1950, 1e-20);
        }
    }

    public class Sofa_t_fk5hip
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void Fk5hip_ReturnsRotationMatrixAndVector()
        {
            // Act
            double[] r5h = new double[9];
            double[] s5h = new double[3];
            Sofa.Fk5hip(r5h, s5h);

            // Assert
            Assert.Equal(0.9999999999999928638, r5h[0], TOLERANCE);
            Assert.Equal(0.1110223351022919694e-6, r5h[1], 1e-17);
            Assert.Equal(0.4411803962536558154e-7, r5h[2], 1e-17);

            Assert.Equal(-0.1454441043328607981e-8, s5h[0], 1e-17);
            Assert.Equal(0.2908882086657215962e-8, s5h[1], 1e-17);
            Assert.Equal(0.3393695767766751955e-8, s5h[2], 1e-17);
        }
    }

    public class Sofa_t_fk5hz
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Fk5hz_ValidInput_ReturnsHipparcocCoordinates()
        {
            // Arrange
            double r5 = 1.76779433;
            double d5 = -0.2917517103;

            // Act
            double rh = 0, dh = 0;
            Sofa.Fk5hz(r5, d5, 2400000.5, 54479.0, ref rh, ref dh);

            // Assert
            Assert.Equal(1.767794191464423978, rh, TOLERANCE);
            Assert.Equal(-0.2917516001679884419, dh, TOLERANCE);
        }
    }

    public class Sofa_t_fw2m
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Fw2m_FrameBiasAngles_ReturnsRotationMatrix()
        {
            // Arrange
            double gamb = -0.2243387670997992368e-5;
            double phib = 0.4091014602391312982;
            double psi = -0.9501954178013015092e-3;
            double eps = 0.4091014316587367472;

            // Act
            double[] r = new double[9];
            Sofa.Fw2m(gamb, phib, psi, eps, r);

            // Assert
            Assert.Equal(0.9999995505176007047, r[0], TOLERANCE);
            Assert.Equal(0.8695404617348192957e-3, r[1], TOLERANCE);
            Assert.Equal(0.3779735201865582571e-3, r[2], TOLERANCE);
        }
    }

    public class Sofa_t_fw2xy
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void Fw2xy_FrameBiasAngles_ReturnsCelestialIntermediateCoordinates()
        {
            // Arrange
            double gamb = -0.2243387670997992368e-5;
            double phib = 0.4091014602391312982;
            double psi = -0.9501954178013015092e-3;
            double eps = 0.4091014316587367472;

            // Act
            double x = 0, y = 0;
            Sofa.Fw2xy(gamb, phib, psi, eps, ref x, ref y);

            // Assert
            Assert.Equal(-0.3779734957034082790e-3, x, TOLERANCE);
            Assert.Equal(-0.1924880848087615651e-6, y, TOLERANCE);
        }
    }

    public class Sofa_t_g2icrs
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void G2icrs_GalacticCoordinates_ReturnsICRSCoordinates()
        {
            // Arrange
            double dl = 5.5850536063818546461558105;
            double db = -0.7853981633974483096156608;

            // Act
            double dr = 0, dd = 0;
            Sofa.G2icrs(dl, db, ref dr, ref dd);

            // Assert
            Assert.Equal(5.9338074302227188048671, dr, TOLERANCE);
            Assert.Equal(-1.1784870613579944551541, dd, TOLERANCE);
        }
    }

    public class Sofa_t_gc2gd
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void Gc2gd_WGS84_ReturnsGeodeticCoordinates()
        {
            // Arrange
            double[] xyz = { 2e6, 3e6, 5.244e6 };

            // Act
            double e = 0, p = 0, h = 0;
            int j = Sofa.Gc2gd(SofaReferenceEllipsoids.WGS84, xyz, ref e, ref p, ref h);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(0.9827937232473290680, e, TOLERANCE);
            Assert.Equal(0.97160184819075459, p, TOLERANCE);
            Assert.Equal(331.4172461426059892, h, 1e-8);
        }
    }

    public class Sofa_t_gd2gc
    {
        private const double TOLERANCE = 1e-7;

        [Fact]
        public void Gd2gc_WGS84_ReturnsGeocentricCoordinates()
        {
            // Arrange
            double e = 3.1;
            double p = -0.5;
            double h = 2500.0;

            // Act
            double[] xyz = new double[3];
            int j = Sofa.Gd2gc(SofaReferenceEllipsoids.WGS84, e, p, h, xyz);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(-5599000.5577049947, xyz[0], TOLERANCE);
            Assert.Equal(233011.67223479203, xyz[1], TOLERANCE);
            Assert.Equal(-3040909.4706983363, xyz[2], TOLERANCE);
        }
    }

    public class Sofa_t_gmst00
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Gmst00_ValidDates_ReturnsGreenwichMeanSiderealTime()
        {
            // Act
            double theta = Sofa.Gmst00(2400000.5, 53736.0, 2400000.5, 53736.0);

            // Assert
            Assert.Equal(1.754174972210740592, theta, TOLERANCE);
        }
    }

    public class Sofa_t_gmst06
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Gmst06_ValidDates_ReturnsGreenwichMeanSiderealTime()
        {
            // Act
            double theta = Sofa.Gmst06(2400000.5, 53736.0, 2400000.5, 53736.0);

            // Assert
            Assert.Equal(1.754174971870091203, theta, TOLERANCE);
        }
    }

    public class Sofa_t_gmst82
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Gmst82_ValidDate_ReturnsGreenwichMeanSiderealTime()
        {
            // Act
            double theta = Sofa.Gmst82(2400000.5, 53736.0);

            // Assert
            Assert.Equal(1.754174981860675096, theta, TOLERANCE);
        }
    }

    public class Sofa_t_gst00a
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Gst00a_ValidDates_ReturnsGreenwichApparentSiderealTime()
        {
            // Act
            double theta = Sofa.Gst00a(2400000.5, 53736.0, 2400000.5, 53736.0);

            // Assert
            Assert.Equal(1.754166138018281369, theta, TOLERANCE);
        }
    }

    public class Sofa_t_gst00b
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Gst00b_ValidDate_ReturnsGreenwichApparentSiderealTime()
        {
            // Act
            double theta = Sofa.Gst00b(2400000.5, 53736.0);

            // Assert
            Assert.Equal(1.754166136510680589, theta, TOLERANCE);
        }
    }

    public class Sofa_t_gst06
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Gst06_ValidInput_ReturnsGreenwichApparentSiderealTime()
        {
            // Arrange
            double[] rnpb = new double[9]
            {
                0.9999989440476103608,
                -0.1332881761240011518e-2,
                -0.5790767434730085097e-3,
                0.1332858254308954453e-2,
                0.9999991109044505944,
                -0.4097782710401555759e-4,
                0.5791308472168153320e-3,
                0.4020595661593994396e-4,
                0.9999998314954572365
            };

            // Act
            double theta = Sofa.Gst06(2400000.5, 53736.0, 2400000.5, 53736.0, rnpb);

            // Assert
            Assert.Equal(1.754166138018167568, theta, TOLERANCE);
        }
    }

    public class Sofa_t_gst06a
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Gst06a_ValidDates_ReturnsGreenwichApparentSiderealTime()
        {
            // Act
            double theta = Sofa.Gst06a(2400000.5, 53736.0, 2400000.5, 53736.0);

            // Assert
            Assert.Equal(1.754166137675019159, theta, TOLERANCE);
        }
    }

    public class Sofa_t_gst94
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Gst94_ValidDate_ReturnsGreenwichApparentSiderealTime()
        {
            // Act
            double theta = Sofa.Gst94(2400000.5, 53736.0);

            // Assert
            Assert.Equal(1.754166136020645203, theta, TOLERANCE);
        }
    }

    public class Sofa_t_h2fk5
    {
        private const double TOLERANCE = 1e-13;

        [Fact]
        public void H2fk5_HipparcocToFK5_ReturnsFK5Coordinates()
        {
            // Arrange
            double rh = 1.767794352;
            double dh = -0.2917512594;
            double drh = -2.76413026e-6;
            double ddh = -5.92994449e-6;
            double pxh = 0.379210;
            double rvh = -7.6;

            // Act
            double r5 = 0, d5 = 0, dr5 = 0, dd5 = 0, px5 = 0, rv5 = 0;
            Sofa.H2fk5(rh, dh, drh, ddh, pxh, rvh, ref r5, ref d5, ref dr5, ref dd5, ref px5, ref rv5);

            // Assert
            Assert.Equal(1.767794455700065506, r5, TOLERANCE);
            Assert.Equal(-0.2917513626469638890, d5, TOLERANCE);
            Assert.Equal(-0.27597945024511204e-5, dr5, 1e-18);
            Assert.Equal(-0.59308014093262838e-5, dd5, 1e-18);
            Assert.Equal(0.37921, px5, TOLERANCE);
            Assert.Equal(-7.6000001309071126, rv5, 1e-11);
        }
    }

    public class Sofa_t_hd2ae
    {
        private const double TOLERANCE = 1e-13;

        [Fact]
        public void Hd2ae_HourAngleDeclinationToAzimuthElevation_ReturnsCoordinates()
        {
            // Arrange
            double h = 1.1;
            double d = 1.2;
            double p = 0.3;

            // Act
            double a = 0, e = 0;
            Sofa.Hd2ae(h, d, p, ref a, ref e);

            // Assert
            Assert.Equal(5.916889243730066194, a, TOLERANCE);
            Assert.Equal(0.4472186304990486228, e, 1e-14);
        }
    }

    public class Sofa_t_hd2pa
    {
        private const double TOLERANCE = 1e-13;

        [Fact]
        public void Hd2pa_HourAngleDeclinationToParallacticAngle_ReturnsAngle()
        {
            // Arrange
            double h = 1.1;
            double d = 1.2;
            double p = 0.3;

            // Act
            double q = Sofa.Hd2pa(h, d, p);

            // Assert
            Assert.Equal(1.906227428001995580, q, TOLERANCE);
        }
    }

    public class Sofa_t_hfk5z
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void Hfk5z_HipparcocToFK5_ReturnsFK5Coordinates()
        {
            // Arrange
            double rh = 1.767794352;
            double dh = -0.2917512594;

            // Act
            double r5 = 0, d5 = 0, dr5 = 0, dd5 = 0;
            Sofa.Hfk5z(rh, dh, 2400000.5, 54479.0, ref r5, ref d5, ref dr5, ref dd5);

            // Assert
            Assert.Equal(1.767794490535581026, r5, 1e-13);
            Assert.Equal(-0.2917513695320114258, d5, TOLERANCE);
            Assert.Equal(0.4335890983539243029e-8, dr5, 1e-22);
            Assert.Equal(-0.8569648841237745902e-9, dd5, 1e-23);
        }
    }

    public class Sofa_t_icrs2g
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void Icrs2g_ICRSToGalactic_ReturnsGalacticCoordinates()
        {
            // Arrange
            double dr = 5.9338074302227188048671087;
            double dd = -1.1784870613579944551540570;

            // Act
            double dl = 0, db = 0;
            Sofa.Icrs2g(dr, dd, ref dl, ref db);

            // Assert
            Assert.Equal(5.5850536063818546461558, dl, TOLERANCE);
            Assert.Equal(-0.7853981633974483096157, db, TOLERANCE);
        }
    }

    public class Sofa_t_ir
    {
        [Fact]
        public void Ir_InitializesIdentityMatrix()
        {
            // Arrange
            double[] r = new double[9]
            {
                2.0, 3.0, 2.0,
                3.0, 2.0, 3.0,
                3.0, 4.0, 5.0
            };

            // Act
            Sofa.Ir(r);

            // Assert
            Assert.Equal(1.0, r[0]);
            Assert.Equal(0.0, r[1]);
            Assert.Equal(0.0, r[2]);
            Assert.Equal(0.0, r[3]);
            Assert.Equal(1.0, r[4]);
            Assert.Equal(0.0, r[5]);
            Assert.Equal(0.0, r[6]);
            Assert.Equal(0.0, r[7]);
            Assert.Equal(1.0, r[8]);
        }
    }

    public class Sofa_t_jd2cal
    {
        private const double TOLERANCE = 1e-7;

        [Fact]
        public void Jd2cal_JulianDate_ReturnsCalendarDate()
        {
            // Arrange
            double dj1 = 2400000.5;
            double dj2 = 50123.9999;

            // Act
            int iy = 0, im = 0, id = 0;
            double fd = 0;
            int j = Sofa.Jd2cal(dj1, dj2, ref iy, ref im, ref id, ref fd);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(1996, iy);
            Assert.Equal(2, im);
            Assert.Equal(10, id);
            Assert.Equal(0.9999, fd, TOLERANCE);
        }
    }

    public class Sofa_t_jdcalf
    {
        [Fact]
        public void Jdcalf_JulianDate_ReturnsCalendarDateFraction()
        {
            // Arrange
            double dj1 = 2400000.5;
            double dj2 = 50123.9999;

            // Act
            int[] iydmf = new int[4];
            int j = Sofa.Jdcalf(4, dj1, dj2, iydmf);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(1996, iydmf[0]);
            Assert.Equal(2, iydmf[1]);
            Assert.Equal(10, iydmf[2]);
            Assert.Equal(9999, iydmf[3]);
        }
    }

    public class Sofa_t_ld
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Ld_GravitationalDeflection_ReturnsAberratedCoordinates()
        {
            // Arrange
            double bm = 0.00028574;
            double[] p = { -0.763276255, -0.608633767, -0.216735543 };
            double[] q = { -0.763276255, -0.608633767, -0.216735543 };
            double[] e = { 0.76700421, 0.605629598, 0.211937094 };
            double em = 8.91276983;
            double dlim = 3e-10;

            // Act
            double[] p1 = new double[3];
            Sofa.Ld(bm, p, q, e, em, dlim, p1);

            // Assert
            Assert.Equal(-0.7632762548968159627, p1[0], TOLERANCE);
            Assert.Equal(-0.6086337670823762701, p1[1], TOLERANCE);
            Assert.Equal(-0.2167355431320546947, p1[2], TOLERANCE);
        }
    }

    public class Sofa_t_ldn
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Ldn_MultipleGravitationalDeflections_ReturnsAberratedCoordinates()
        {
            // Arrange
            int n = 3;
            var b = new Sofa.LdBody[3];
            b[0].bm = 0.00028574;
            b[0].dl = 3e-10;
            b[0].pv = new double[6] { -7.81014427, -5.60956681, -1.98079819, 0.0030723249, -0.00406995477, -0.00181335842 };

            b[1].bm = 0.00095435;
            b[1].dl = 3e-9;
            b[1].pv = new double[6] { 0.738098796, 4.63658692, 1.9693136, -0.00755816922, 0.00126913722, 0.000727999001 };

            b[2].bm = 1.0;
            b[2].dl = 6e-6;
            b[2].pv = new double[6] { -0.000712174377, -0.00230478303, -0.00105865966, 6.29235213e-6, -3.30888387e-7, -2.96486623e-7 };

            // Additional body data...

            double[] ob = { -0.974170437, -0.2115201, -0.0917583114 };
            double[] sc = { -0.763276255, -0.608633767, -0.216735543 };

            // Act
            double[] sn = new double[3];
            Sofa.Ldn(n, b, ob, sc, sn);

            // Assert
            Assert.Equal(-0.7632762579693333866, sn[0], TOLERANCE);
            Assert.Equal(-0.6086337636093002660, sn[1], TOLERANCE);
            Assert.Equal(-0.2167355420646328159, sn[2], TOLERANCE);
        }
    }

    public class Sofa_t_ldsun
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Ldsun_SolarGravitationalDeflection_ReturnsAberratedCoordinates()
        {
            // Arrange
            double[] p = { -0.763276255, -0.608633767, -0.216735543 };
            double[] e = { -0.973644023, -0.20925523, -0.0907169552 };
            double em = 0.999809214;

            // Act
            double[] p1 = new double[3];
            Sofa.Ldsun(p, e, em, p1);

            // Assert
            Assert.Equal(-0.7632762580731413169, p1[0], TOLERANCE);
            Assert.Equal(-0.6086337635262647900, p1[1], TOLERANCE);
            Assert.Equal(-0.2167355419322321302, p1[2], TOLERANCE);
        }
    }

    public class Sofa_t_lteceq
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void Lteceq_LocalTrueEclipticToEquatorial_ReturnsEquatorialCoordinates()
        {
            // Arrange
            double epj = 2500.0;
            double dl = 1.5;
            double db = 0.6;

            // Act
            double dr = 0, dd = 0;
            Sofa.Lteceq(epj, dl, db, ref dr, ref dd);

            // Assert
            Assert.Equal(1.275156021861921167, dr, TOLERANCE);
            Assert.Equal(0.9966573543519204791, dd, TOLERANCE);
        }
    }

    public class Sofa_t_ltecm
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void Ltecm_LocalTrueEclipticMatrix_ReturnsRotationMatrix()
        {
            // Arrange
            double epj = -3000.0;

            // Act
            double[] rm = new double[9];
            Sofa.Ltecm(epj, rm);

            // Assert
            Assert.Equal(0.3564105644859788825, rm[0], TOLERANCE);
            Assert.Equal(0.8530575738617682284, rm[1], TOLERANCE);
            Assert.Equal(0.3811355207795060435, rm[2], TOLERANCE);
        }
    }

    public class Sofa_t_lteqec
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void Lteqec_LocalTrueEquatorialToEcliptic_ReturnsEclipticCoordinates()
        {
            // Arrange
            double epj = -1500.0;
            double dr = 1.234;
            double dd = 0.987;

            // Act
            double dl = 0, db = 0;
            Sofa.Lteqec(epj, dr, dd, ref dl, ref db);

            // Assert
            Assert.Equal(0.5039483649047114859, dl, TOLERANCE);
            Assert.Equal(0.5848534459726224882, db, TOLERANCE);
        }
    }

    public class Sofa_t_ltp
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void Ltp_LocalTruePolesMatrix_ReturnsRotationMatrix()
        {
            // Arrange
            double epj = 1666.666;

            // Act
            double[] rp = new double[9];
            Sofa.Ltp(epj, rp);

            // Assert
            Assert.Equal(0.9967044141159213819, rp[0], TOLERANCE);
            Assert.Equal(0.7437801893193210840e-1, rp[1], TOLERANCE);
            Assert.Equal(0.3237624409345603401e-1, rp[2], TOLERANCE);
        }
    }

    public class Sofa_t_ltpb
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void Ltpb_LocalTruePolesBaryCentricMatrix_ReturnsRotationMatrix()
        {
            // Arrange
            double epj = 1666.666;

            // Act
            double[] rpb = new double[9];
            Sofa.Ltpb(epj, rpb);

            // Assert
            Assert.Equal(0.9967044167723271851, rpb[0], TOLERANCE);
            Assert.Equal(0.7437794731203340345e-1, rpb[1], TOLERANCE);
            Assert.Equal(0.3237632684841625547e-1, rpb[2], TOLERANCE);
        }
    }

    public class Sofa_t_ltpecl
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void Ltpecl_LocalTruePolesEcliptic_ReturnsEclipticVector()
        {
            // Arrange
            double epj = -1500.0;

            // Act
            double[] vec = new double[3];
            Sofa.Ltpecl(epj, vec);

            // Assert
            Assert.Equal(0.4768625676477096525e-3, vec[0], TOLERANCE);
            Assert.Equal(-0.4052259533091875112, vec[1], TOLERANCE);
            Assert.Equal(0.9142164401096448012, vec[2], TOLERANCE);
        }
    }

    public class Sofa_t_ltpequ
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void Ltpequ_LocalTruePolesEquatorial_ReturnsEquatorialVector()
        {
            // Arrange
            double epj = -2500.0;

            // Act
            double[] veq = new double[3];
            Sofa.Ltpequ(epj, veq);

            // Assert
            Assert.Equal(-0.3586652560237326659, veq[0], TOLERANCE);
            Assert.Equal(-0.1996978910771128475, veq[1], TOLERANCE);
            Assert.Equal(0.9118552442250819624, veq[2], TOLERANCE);
        }
    }

    public class Sofa_t_moon98
    {
        private const double TOLERANCE = 1e-11;

        [Fact]
        public void Moon98_ValidDate_ReturnsMoonPositionVelocity()
        {
            // Act
            double[] pv = new double[6];
            Sofa.Moon98(2400000.5, 43999.9, pv);

            // Assert
            Assert.Equal(-0.2601295959971044180e-2, pv[0], TOLERANCE);
            Assert.Equal(0.6139750944302742189e-3, pv[1], TOLERANCE);
            Assert.Equal(0.2640794528229828909e-3, pv[2], TOLERANCE);
        }
    }

    public class Sofa_t_num00a
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Num00a_NutationMatrix2000A_ReturnsRotationMatrix()
        {
            // Act
            double[] rmatn = new double[9];
            Sofa.Num00a(2400000.5, 53736.0, rmatn);

            // Assert
            Assert.Equal(0.9999999999536227949, rmatn[0], TOLERANCE);
            Assert.Equal(0.8836238544090873336e-5, rmatn[1], TOLERANCE);
            Assert.Equal(0.3830835237722400669e-5, rmatn[2], TOLERANCE);
        }
    }

    public class Sofa_t_num00b
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Num00b_NutationMatrix2000B_ReturnsRotationMatrix()
        {
            // Act
            double[] rmatn = new double[9];
            Sofa.Num00b(2400000.5, 53736, rmatn);

            // Assert
            Assert.Equal(0.9999999999536069682, rmatn[0], TOLERANCE);
            Assert.Equal(0.8837746144871248011e-5, rmatn[1], TOLERANCE);
            Assert.Equal(0.3831488838252202945e-5, rmatn[2], TOLERANCE);
        }
    }

    public class Sofa_t_num06a
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Num06a_NutationMatrix2006A_ReturnsRotationMatrix()
        {
            // Act
            double[] rmatn = new double[9];
            Sofa.Num06a(2400000.5, 53736, rmatn);

            // Assert
            Assert.Equal(0.9999999999536227668, rmatn[0], TOLERANCE);
            Assert.Equal(0.8836241998111535233e-5, rmatn[1], TOLERANCE);
            Assert.Equal(0.3830834608415287707e-5, rmatn[2], TOLERANCE);
        }
    }

    public class Sofa_t_numat
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Numat_NutationMatrixFromComponents_ReturnsRotationMatrix()
        {
            // Arrange
            double epsa = 0.4090789763356509900;
            double dpsi = -0.9630909107115582393e-5;
            double deps = 0.4063239174001678826e-4;

            // Act
            double[] rmatn = new double[9];
            Sofa.Numat(epsa, dpsi, deps, rmatn);

            // Assert
            Assert.Equal(0.9999999999536227949, rmatn[0], TOLERANCE);
            Assert.Equal(0.8836239320236250577e-5, rmatn[1], TOLERANCE);
            Assert.Equal(0.3830833447458251908e-5, rmatn[2], TOLERANCE);
        }
    }

    public class Sofa_t_nut00a
    {
        private const double TOLERANCE = 1e-13;

        [Fact]
        public void Nut00a_Nutation2000A_ReturnsNutationComponents()
        {
            // Act
            double dpsi = 0, deps = 0;
            Sofa.Nut00a(2400000.5, 53736.0, ref dpsi, ref deps);

            // Assert
            Assert.Equal(-0.9630909107115518431e-5, dpsi, TOLERANCE);
            Assert.Equal(0.4063239174001678710e-4, deps, TOLERANCE);
        }
    }

    public class Sofa_t_nut00b
    {
        private const double TOLERANCE = 1e-13;

        [Fact]
        public void Nut00b_Nutation2000B_ReturnsNutationComponents()
        {
            // Act
            double dpsi = 0, deps = 0;
            Sofa.Nut00b(2400000.5, 53736.0, ref dpsi, ref deps);

            // Assert
            Assert.Equal(-0.9632552291148362783e-5, dpsi, TOLERANCE);
            Assert.Equal(0.4063197106621159367e-4, deps, TOLERANCE);
        }
    }

    public class Sofa_t_nut06a
    {
        private const double TOLERANCE = 1e-13;

        [Fact]
        public void Nut06a_Nutation2006A_ReturnsNutationComponents()
        {
            // Act
            double dpsi = 0, deps = 0;
            Sofa.Nut06a(2400000.5, 53736.0, ref dpsi, ref deps);

            // Assert
            Assert.Equal(-0.9630912025820308797e-5, dpsi, TOLERANCE);
            Assert.Equal(0.4063238496887249798e-4, deps, TOLERANCE);
        }
    }

    public class Sofa_t_nut80
    {
        private const double TOLERANCE = 1e-13;

        [Fact]
        public void Nut80_Nutation1980_ReturnsNutationComponents()
        {
            // Act
            double dpsi = 0, deps = 0;
            Sofa.Nut80(2400000.5, 53736.0, ref dpsi, ref deps);

            // Assert
            Assert.Equal(-0.9643658353226563966e-5, dpsi, TOLERANCE);
            Assert.Equal(0.4060051006879713322e-4, deps, TOLERANCE);
        }
    }

    public class Sofa_t_nutm80
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Nutm80_NutationMatrix1980_ReturnsRotationMatrix()
        {
            // Act
            double[] rmatn = new double[9];
            Sofa.Nutm80(2400000.5, 53736.0, rmatn);

            // Assert
            Assert.Equal(0.9999999999534999268, rmatn[0], TOLERANCE);
            Assert.Equal(0.8847935789636432161e-5, rmatn[1], TOLERANCE);
            Assert.Equal(0.3835906502164019142e-5, rmatn[2], TOLERANCE);
        }
    }

    public class Sofa_t_obl06
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void Obl06_ObliquityOfEcliptic2006_ReturnsAngle()
        {
            // Act
            double obl = Sofa.Obl06(2400000.5, 54388.0);

            // Assert
            Assert.Equal(0.4090749229387258204, obl, TOLERANCE);
        }
    }

    public class Sofa_t_obl80
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void Obl80_ObliquityOfEcliptic1980_ReturnsAngle()
        {
            // Act
            double eps0 = Sofa.Obl80(2400000.5, 54388.0);

            // Assert
            Assert.Equal(0.4090751347643816218, eps0, TOLERANCE);
        }
    }

    public class Sofa_t_p06e
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void P06e_PrecessionElements2006_ReturnsMultipleParameters()
        {
            // Act
            double eps0 = 0, psia = 0, oma = 0, bpa = 0, bqa = 0, pia = 0, bpia = 0,
                   epsa = 0, chia = 0, za = 0, zetaa = 0, thetaa = 0, pa = 0, gam = 0, phi = 0, psi = 0;

            Sofa.P06e(2400000.5, 52541.0, ref eps0, ref psia, ref oma, ref bpa,
                      ref bqa, ref pia, ref bpia, ref epsa, ref chia, ref za,
                      ref zetaa, ref thetaa, ref pa, ref gam, ref phi, ref psi);

            // Assert
            Assert.Equal(0.4090926006005828715, eps0, TOLERANCE);
            Assert.Equal(0.6664369630191613431e-3, psia, TOLERANCE);
            Assert.Equal(0.4090925973783255982, oma, TOLERANCE);
        }
    }

    public class Sofa_t_p2pv
    {
        [Fact]
        public void P2pv_PositionToPositionVelocity_ReturnsPositionVelocity()
        {
            // Arrange
            double[] p = { 0.25, 1.2, 3.0 };
            double[] pv = new double[6]
            {
                0.3, 1.2, -2.5,
                -0.5, 3.1, 0.9
            };

            // Act
            Sofa.P2pv(p, pv);

            // Assert
            Assert.Equal(0.25, pv[0]);
            Assert.Equal(1.2, pv[1]);
            Assert.Equal(3.0, pv[2]);
            Assert.Equal(0.0, pv[3]);
            Assert.Equal(0.0, pv[4]);
            Assert.Equal(0.0, pv[5]);
        }
    }

    public class Sofa_t_p2s
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void P2s_CartesianToSpherical_ReturnsSphericalCoordinates()
        {
            // Arrange
            double[] p = { 100.0, -50.0, 25.0 };

            // Act
            double theta = 0, phi = 0, r = 0;
            Sofa.P2s(p, ref theta, ref phi, ref r);

            // Assert
            Assert.Equal(-0.4636476090008061162, theta, TOLERANCE);
            Assert.Equal(0.2199879773954594463, phi, TOLERANCE);
            Assert.Equal(114.5643923738960002, r, 1e-9);
        }
    }

    public class Sofa_t_pap
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Pap_PositionAngleBetweenVectors_ReturnsAngle()
        {
            // Arrange
            double[] a = { 1.0, 0.1, 0.2 };
            double[] b = { -3.0, 1e-3, 0.2 };

            // Act
            double theta = Sofa.Pap(a, b);

            // Assert
            Assert.Equal(0.3671514267841113674, theta, TOLERANCE);
        }
    }

    public class Sofa_t_pas
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Pas_PositionAngleFromSpherical_ReturnsAngle()
        {
            // Arrange
            double al = 1.0;
            double ap = 0.1;
            double bl = 0.2;
            double bp = -1.0;

            // Act
            double theta = Sofa.Pas(al, ap, bl, bp);

            // Assert
            Assert.Equal(-2.724544922932270424, theta, TOLERANCE);
        }
    }

    public class Sofa_t_pb06
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Pb06_PrecessionElements_ReturnsPrecessionParameters()
        {
            // Act
            double bzeta = 0, bz = 0, btheta = 0;
            Sofa.Pb06(2400000.5, 50123.9999, ref bzeta, ref bz, ref btheta);

            // Assert
            Assert.Equal(-0.5092634016326478238e-3, bzeta, TOLERANCE);
            Assert.Equal(-0.3602772060566044413e-3, bz, TOLERANCE);
            Assert.Equal(-0.3779735537167811177e-3, btheta, TOLERANCE);
        }
    }

    public class Sofa_t_pdp
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Pdp_VectorDotProduct_ReturnsScalarProduct()
        {
            // Arrange
            double[] a = { 2.0, 2.0, 3.0 };
            double[] b = { 1.0, 3.0, 4.0 };

            // Act
            double adb = Sofa.Pdp(a, b);

            // Assert
            Assert.Equal(20, adb, TOLERANCE);
        }
    }

    public class Sofa_t_pfw06
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Pfw06_PrecessionFrameBias_ReturnsAngles()
        {
            // Act
            double gamb = 0, phib = 0, psib = 0, epsa = 0;
            Sofa.Pfw06(2400000.5, 50123.9999, ref gamb, ref phib, ref psib, ref epsa);

            // Assert
            Assert.Equal(-0.2243387670997995690e-5, gamb, 1e-16);
            Assert.Equal(0.4091014602391312808, phib, TOLERANCE);
            Assert.Equal(-0.9501954178013015895e-3, psib, 1e-14);
            Assert.Equal(0.4091014316587367491, epsa, TOLERANCE);
        }
    }

    public class Sofa_t_plan94
    {
        private const double TOLERANCE = 1e-11;

        [Fact]
        public void Plan94_PlanetPosition_ReturnsPositionVelocity()
        {
            // Act
            double[] pv = new double[6];
            int j = Sofa.Plan94(2400000.5, 43999.9, 1, pv);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(0.2945293959257430832, pv[0], TOLERANCE);
            Assert.Equal(-0.2452204176601049596, pv[1], TOLERANCE);
            Assert.Equal(-0.1615427700571978153, pv[2], TOLERANCE);
        }
    }

    public class Sofa_t_pm
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Pm_VectorMagnitude_ReturnsMagnitude()
        {
            // Arrange
            double[] p = { 0.3, 1.2, -2.5 };

            // Act
            double r = Sofa.Pm(p);

            // Assert
            Assert.Equal(2.789265136196270604, r, TOLERANCE);
        }
    }

    public class Sofa_t_pmp
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Pmp_VectorDifference_ReturnsResultVector()
        {
            // Arrange
            double[] a = { 2.0, 2.0, 3.0 };
            double[] b = { 1.0, 3.0, 4.0 };

            // Act
            double[] amb = new double[3];
            Sofa.Pmp(a, b, amb);

            // Assert
            Assert.Equal(1.0, amb[0], TOLERANCE);
            Assert.Equal(-1.0, amb[1], TOLERANCE);
            Assert.Equal(-1.0, amb[2], TOLERANCE);
        }
    }

    public class Sofa_t_pmpx
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Pmpx_StarMotionAndProperMotion_ReturnsPosition()
        {
            // Arrange
            double rc = 1.234;
            double dc = 0.789;
            double pr = 1e-5;
            double pd = -2e-5;
            double px = 1e-2;
            double rv = 10.0;
            double pmt = 8.75;
            double[] pob = { 0.9, 0.4, 0.1 };

            // Act
            double[] pco = new double[3];
            Sofa.Pmpx(rc, dc, pr, pd, px, rv, pmt, pob, pco);

            // Assert
            Assert.Equal(0.2328137623960308438, pco[0], TOLERANCE);
            Assert.Equal(0.6651097085397855328, pco[1], TOLERANCE);
            Assert.Equal(0.7095257765896359837, pco[2], TOLERANCE);
        }
    }

    public class Sofa_t_pmsafe
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Pmsafe_SafeProperMotion_ReturnsPropagatedCoordinates()
        {
            // Arrange
            double ra1 = 1.234;
            double dec1 = 0.789;
            double pmr1 = 1e-5;
            double pmd1 = -2e-5;
            double px1 = 1e-2;
            double rv1 = 10.0;
            double ep1a = 2400000.5;
            double ep1b = 48348.5625;
            double ep2a = 2400000.5;
            double ep2b = 51544.5;

            // Act
            double ra2 = 0, dec2 = 0, pmr2 = 0, pmd2 = 0, px2 = 0, rv2 = 0;
            int j = Sofa.Pmsafe(ra1, dec1, pmr1, pmd1, px1, rv1,
                                ep1a, ep1b, ep2a, ep2b,
                                ref ra2, ref dec2, ref pmr2, ref pmd2, ref px2, ref rv2);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(1.234087484501017061, ra2, TOLERANCE);
            Assert.Equal(0.7888249982450468567, dec2, TOLERANCE);
            Assert.Equal(0.9996457663586073988e-5, pmr2, TOLERANCE);
            Assert.Equal(-0.2000040085106754565e-4, pmd2, 1e-16);
            Assert.Equal(0.9999997295356830666e-2, px2, TOLERANCE);
            Assert.Equal(10.38468380293920069, rv2, 1e-10);
        }
    }

    public class Sofa_t_pn
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Pn_VectorNormalize_ReturnsNormalizedVector()
        {
            // Arrange
            double[] p = { 0.3, 1.2, -2.5 };

            // Act
            double r = 0;
            double[] u = new double[3];
            Sofa.Pn(p, ref r, u);

            // Assert
            Assert.Equal(2.789265136196270604, r, TOLERANCE);
            Assert.Equal(0.1075552109073112058, u[0], TOLERANCE);
            Assert.Equal(0.4302208436292448232, u[1], TOLERANCE);
            Assert.Equal(-0.8962934242275933816, u[2], TOLERANCE);
        }
    }

    public class Sofa_t_pn00
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Pn00_PrecessionNutation2000_ReturnsMatrices()
        {
            // Arrange
            double dpsi = -0.9632552291149335877e-5;
            double deps = 0.4063197106621141414e-4;

            // Act
            double epsa = 0;
            double[] rb = new double[9];
            double[] rp = new double[9];
            double[] rbp = new double[9];
            double[] rn = new double[9];
            double[] rbpn = new double[9];

            Sofa.Pn00(2400000.5, 53736.0, dpsi, deps, ref epsa, rb, rp, rbp, rn, rbpn);

            // Assert
            Assert.Equal(0.4090791789404229916, epsa, TOLERANCE);
            Assert.Equal(0.9999999999999942498, rb[0], TOLERANCE);
        }
    }

    public class Sofa_t_pn00a
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Pn00a_PrecessionNutation2000A_ReturnsMatrices()
        {
            // Act
            double dpsi = 0, deps = 0, epsa = 0;
            double[] rb = new double[9];
            double[] rp = new double[9];
            double[] rbp = new double[9];
            double[] rn = new double[9];
            double[] rbpn = new double[9];

            Sofa.Pn00a(2400000.5, 53736.0, ref dpsi, ref deps, ref epsa, rb, rp, rbp, rn, rbpn);

            // Assert
            Assert.Equal(-0.9630909107115518431e-5, dpsi, 1e-12);
            Assert.Equal(0.4063239174001678710e-4, deps, 1e-12);
        }
    }

    public class Sofa_t_pn00b
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Pn00b_PrecessionNutation2000B_ReturnsMatrices()
        {
            // Act
            double dpsi = 0, deps = 0, epsa = 0;
            double[] rb = new double[9];
            double[] rp = new double[9];
            double[] rbp = new double[9];
            double[] rn = new double[9];
            double[] rbpn = new double[9];

            Sofa.Pn00b(2400000.5, 53736.0, ref dpsi, ref deps, ref epsa, rb, rp, rbp, rn, rbpn);

            // Assert
            Assert.Equal(-0.9632552291148362783e-5, dpsi, 1e-12);
            Assert.Equal(0.4063197106621159367e-4, deps, 1e-12);
        }
    }

    public class Sofa_t_pn06a
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Pn06a_PrecessionNutation2006A_ReturnsMatrices()
        {
            // Act
            double dpsi = 0, deps = 0, epsa = 0;
            double[] rb = new double[9];
            double[] rp = new double[9];
            double[] rbp = new double[9];
            double[] rn = new double[9];
            double[] rbpn = new double[9];

            Sofa.Pn06a(2400000.5, 53736.0, ref dpsi, ref deps, ref epsa, rb, rp, rbp, rn, rbpn);

            // Assert
            Assert.Equal(-0.9630912025820308797e-5, dpsi, 1e-12);
            Assert.Equal(0.4063238496887249798e-4, deps, 1e-12);
        }
    }

}