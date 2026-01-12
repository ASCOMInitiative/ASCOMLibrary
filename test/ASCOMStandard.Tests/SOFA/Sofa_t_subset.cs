using ASCOM.Tools;
using System.Net.NetworkInformation;
using System.Text;
using Xunit;

namespace SOFATests
{
    public class Sofa_t_c2ibpn
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void C2ibpn_ValidInputs_ReturnsExpectedMatrix()
        {
            // Arrange
            double[] rbpn = new double[9]
            {
                9.999962358680738e-1,
                -2.516417057665452e-3,
                -1.093569785342370e-3,
                2.516462370370876e-3,
                9.999968329010883e-1,
                4.006159587358310e-5,
                1.093465510215479e-3,
                -4.281337229063151e-5,
                9.999994012499173e-1
            };
            double date1 = 2400000.5;
            double date2 = 50123.9999;

            // Act
            double[] rc2i = new double[9];
            Sofa.C2ibpn(date1, date2, rbpn, rc2i);

            // Assert
            Assert.Equal(0.9999994021664089977, rc2i[0], TOLERANCE);
            Assert.Equal(-0.3869195948017503664e-8, rc2i[1], TOLERANCE);
            Assert.Equal(-0.1093465511383285076e-2, rc2i[2], TOLERANCE);
            Assert.Equal(0.5068413965715446111e-7, rc2i[3], TOLERANCE);
            Assert.Equal(0.9999999990835075686, rc2i[4], TOLERANCE);
            Assert.Equal(0.4281334246452708915e-4, rc2i[5], TOLERANCE);
            Assert.Equal(0.1093465510215479000e-2, rc2i[6], TOLERANCE);
            Assert.Equal(-0.4281337229063151000e-4, rc2i[7], TOLERANCE);
            Assert.Equal(0.9999994012499173103, rc2i[8], TOLERANCE);
        }
    }

    public class Sofa_t_c2ixy
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void C2ixy_ValidInputs_ReturnsExpectedMatrix()
        {
            // Arrange
            double x = 0.5791308486706011000e-3;
            double y = 0.4020579816732961219e-4;
            double date1 = 2400000.5;
            double date2 = 53736;

            // Act
            double[] rc2i = new double[9];
            Sofa.C2ixy(date1, date2, x, y, rc2i);

            // Assert
            Assert.Equal(0.9999998323037157138, rc2i[0], TOLERANCE);
            Assert.Equal(0.5581526349032241205e-9, rc2i[1], TOLERANCE);
            Assert.Equal(-0.5791308491611263745e-3, rc2i[2], TOLERANCE);
            Assert.Equal(-0.2384257057469842953e-7, rc2i[3], TOLERANCE);
            Assert.Equal(0.9999999991917468964, rc2i[4], TOLERANCE);
            Assert.Equal(-0.4020579110172324363e-4, rc2i[5], TOLERANCE);
            Assert.Equal(0.5791308486706011000e-3, rc2i[6], TOLERANCE);
            Assert.Equal(0.4020579816732961219e-4, rc2i[7], TOLERANCE);
            Assert.Equal(0.9999998314954627590, rc2i[8], TOLERANCE);
        }
    }

    public class Sofa_t_c2ixys
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void C2ixys_ValidInputs_ReturnsExpectedMatrix()
        {
            // Arrange
            double x = 0.5791308486706011000e-3;
            double y = 0.4020579816732961219e-4;
            double s = -0.1220040848472271978e-7;

            // Act
            double[] rc2i = new double[9];
            Sofa.C2ixys(x, y, s, rc2i);

            // Assert
            Assert.Equal(0.9999998323037157138, rc2i[0], TOLERANCE);
            Assert.Equal(0.5581984869168499149e-9, rc2i[1], TOLERANCE);
            Assert.Equal(-0.5791308491611282180e-3, rc2i[2], TOLERANCE);
            Assert.Equal(-0.2384261642670440317e-7, rc2i[3], TOLERANCE);
            Assert.Equal(0.9999999991917468964, rc2i[4], TOLERANCE);
            Assert.Equal(-0.4020579110169668931e-4, rc2i[5], TOLERANCE);
            Assert.Equal(0.5791308486706011000e-3, rc2i[6], TOLERANCE);
            Assert.Equal(0.4020579816732961219e-4, rc2i[7], TOLERANCE);
            Assert.Equal(0.9999998314954627590, rc2i[8], TOLERANCE);
        }
    }

    public class Sofa_t_c2s
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void C2s_ValidInputs_ReturnsExpectedSphericalCoordinates()
        {
            // Arrange
            double[] p = new double[3] { 100.0, -50.0, 25.0 };

            // Act
            double theta = 0;
            double phi = 0;
            Sofa.C2s(p, ref theta, ref phi);

            // Assert
            Assert.Equal(-0.4636476090008061162, theta, TOLERANCE);
            Assert.Equal(0.2199879773954594463, phi, TOLERANCE);
        }
    }

    public class Sofa_t_c2t00a
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void C2t00a_ValidInputs_ReturnsExpectedMatrix()
        {
            // Arrange
            double tta = 2400000.5;
            double ttb = 53736.0;
            double uta = 2400000.5;
            double utb = 53736.0;
            double xp = 2.55060238e-7;
            double yp = 1.860359247e-6;

            // Act
            double[] rc2t = new double[9];
            Sofa.C2t00a(tta, ttb, uta, utb, xp, yp, rc2t);

            // Assert
            Assert.Equal(-0.1810332128307182668, rc2t[0], TOLERANCE);
            Assert.Equal(0.9834769806938457836, rc2t[1], TOLERANCE);
            Assert.Equal(0.6555535638688341725e-4, rc2t[2], TOLERANCE);
            Assert.Equal(-0.9834768134135984552, rc2t[3], TOLERANCE);
            Assert.Equal(-0.1810332203649520727, rc2t[4], TOLERANCE);
            Assert.Equal(0.5749801116141056317e-3, rc2t[5], TOLERANCE);
            Assert.Equal(0.5773474014081406921e-3, rc2t[6], TOLERANCE);
            Assert.Equal(0.3961832391770163647e-4, rc2t[7], TOLERANCE);
            Assert.Equal(0.9999998325501692289, rc2t[8], TOLERANCE);
        }
    }

    public class Sofa_t_c2t00b
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void C2t00b_ValidInputs_ReturnsExpectedMatrix()
        {
            // Arrange
            double tta = 2400000.5;
            double ttb = 53736.0;
            double uta = 2400000.5;
            double utb = 53736.0;
            double xp = 2.55060238e-7;
            double yp = 1.860359247e-6;

            // Act
            double[] rc2t = new double[9];
            Sofa.C2t00b(tta, ttb, uta, utb, xp, yp, rc2t);

            // Assert
            Assert.Equal(-0.1810332128439678965, rc2t[0], TOLERANCE);
            Assert.Equal(0.9834769806913872359, rc2t[1], TOLERANCE);
            Assert.Equal(0.6555565082458415611e-4, rc2t[2], TOLERANCE);
            Assert.Equal(-0.9834768134115435923, rc2t[3], TOLERANCE);
            Assert.Equal(-0.1810332203784001946, rc2t[4], TOLERANCE);
            Assert.Equal(0.5749793922030017230e-3, rc2t[5], TOLERANCE);
            Assert.Equal(0.5773467471863534901e-3, rc2t[6], TOLERANCE);
            Assert.Equal(0.3961790411549945020e-4, rc2t[7], TOLERANCE);
            Assert.Equal(0.9999998325505635738, rc2t[8], TOLERANCE);
        }
    }

    public class Sofa_t_c2t06a
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void C2t06a_ValidInputs_ReturnsExpectedMatrix()
        {
            // Arrange
            double tta = 2400000.5;
            double ttb = 53736.0;
            double uta = 2400000.5;
            double utb = 53736.0;
            double xp = 2.55060238e-7;
            double yp = 1.860359247e-6;

            // Act
            double[] rc2t = new double[9];
            Sofa.C2t06a(tta, ttb, uta, utb, xp, yp, rc2t);

            // Assert
            Assert.Equal(-0.1810332128305897282, rc2t[0], TOLERANCE);
            Assert.Equal(0.9834769806938592296, rc2t[1], TOLERANCE);
            Assert.Equal(0.6555550962998436505e-4, rc2t[2], TOLERANCE);
            Assert.Equal(-0.9834768134136214897, rc2t[3], TOLERANCE);
            Assert.Equal(-0.1810332203649130832, rc2t[4], TOLERANCE);
            Assert.Equal(0.5749800844905594110e-3, rc2t[5], TOLERANCE);
            Assert.Equal(0.5773474024748545878e-3, rc2t[6], TOLERANCE);
            Assert.Equal(0.3961816829632690581e-4, rc2t[7], TOLERANCE);
            Assert.Equal(0.9999998325501747785, rc2t[8], TOLERANCE);
        }
    }

    public class Sofa_t_c2tcio
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void C2tcio_ValidInputs_ReturnsExpectedMatrix()
        {
            // Arrange
            double[] rc2i = new double[9]
            {
                0.9999998323037164738,
                0.5581526271714303683e-9,
                -0.5791308477073443903e-3,
                -0.2384266227524722273e-7,
                0.9999999991917404296,
                -0.4020594955030704125e-4,
                0.5791308472168153320e-3,
                0.4020595661593994396e-4,
                0.9999998314954572365
            };
            double era = 1.75283325530307;
            double[] rpom = new double[9]
            {
                0.9999999999999674705,
                -0.1367174580728847031e-10,
                0.2550602379999972723e-6,
                0.1414624947957029721e-10,
                0.9999999999982694954,
                -0.1860359246998866338e-5,
                -0.2550602379741215275e-6,
                0.1860359247002413923e-5,
                0.9999999999982369658
            };

            // Act
            double[] rc2t = new double[9];
            Sofa.C2tcio(rc2i, era, rpom, rc2t);

            // Assert
            Assert.Equal(-0.1810332128307110439, rc2t[0], TOLERANCE);
            Assert.Equal(0.9834769806938470149, rc2t[1], TOLERANCE);
            Assert.Equal(0.6555535638685466874e-4, rc2t[2], TOLERANCE);
            Assert.Equal(-0.9834768134135996657, rc2t[3], TOLERANCE);
            Assert.Equal(-0.1810332203649448367, rc2t[4], TOLERANCE);
            Assert.Equal(0.5749801116141106528e-3, rc2t[5], TOLERANCE);
            Assert.Equal(0.5773474014081407076e-3, rc2t[6], TOLERANCE);
            Assert.Equal(0.3961832391772658944e-4, rc2t[7], TOLERANCE);
            Assert.Equal(0.9999998325501691969, rc2t[8], TOLERANCE);
        }
    }

    public class Sofa_t_c2teqx
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void C2teqx_ValidInputs_ReturnsExpectedMatrix()
        {
            // Arrange
            double[] rbpn = new double[9]
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
            double gst = 1.754166138040730516;
            double[] rpom = new double[9]
            {
                0.9999999999999674705,
                -0.1367174580728847031e-10,
                0.2550602379999972723e-6,
                0.1414624947957029721e-10,
                0.9999999999982694954,
                -0.1860359246998866338e-5,
                -0.2550602379741215275e-6,
                0.1860359247002413923e-5,
                0.9999999999982369658
            };

            // Act
            double[] rc2t = new double[9];
            Sofa.C2teqx(rbpn, gst, rpom, rc2t);

            // Assert
            Assert.Equal(-0.1810332128528685730, rc2t[0], TOLERANCE);
            Assert.Equal(0.9834769806897685071, rc2t[1], TOLERANCE);
            Assert.Equal(0.6555535639982634449e-4, rc2t[2], TOLERANCE);
            Assert.Equal(-0.9834768134095211257, rc2t[3], TOLERANCE);
            Assert.Equal(-0.1810332203871023800, rc2t[4], TOLERANCE);
            Assert.Equal(0.5749801116126438962e-3, rc2t[5], TOLERANCE);
            Assert.Equal(0.5773474014081539467e-3, rc2t[6], TOLERANCE);
            Assert.Equal(0.3961832391768640871e-4, rc2t[7], TOLERANCE);
            Assert.Equal(0.9999998325501691969, rc2t[8], TOLERANCE);
        }
    }

    public class Sofa_t_c2tpe
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void C2tpe_ValidInputs_ReturnsExpectedMatrix()
        {
            // Arrange
            double tta = 2400000.5;
            double ttb = 53736.0;
            double uta = 2400000.5;
            double utb = 53736.0;
            double dpsi = -0.9630909107115582393e-5;
            double deps = 0.4090789763356509900;
            double xp = 2.55060238e-7;
            double yp = 1.860359247e-6;

            // Act
            double[] rc2t = new double[9];
            Sofa.C2tpe(tta, ttb, uta, utb, dpsi, deps, xp, yp, rc2t);

            // Assert
            Assert.Equal(-0.1813677995763029394, rc2t[0], TOLERANCE);
            Assert.Equal(0.9023482206891683275, rc2t[1], TOLERANCE);
            Assert.Equal(-0.3909902938641085751, rc2t[2], TOLERANCE);
            Assert.Equal(-0.9834147641476804807, rc2t[3], TOLERANCE);
            Assert.Equal(-0.1659883635434995121, rc2t[4], TOLERANCE);
            Assert.Equal(0.7309763898042819705e-1, rc2t[5], TOLERANCE);
            Assert.Equal(0.1059685430673215247e-2, rc2t[6], TOLERANCE);
            Assert.Equal(0.3977631855605078674, rc2t[7], TOLERANCE);
            Assert.Equal(0.9174875068792735362, rc2t[8], TOLERANCE);
        }
    }

    public class Sofa_t_c2txy
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void C2txy_ValidInputs_ReturnsExpectedMatrix()
        {
            // Arrange
            double tta = 2400000.5;
            double ttb = 53736.0;
            double uta = 2400000.5;
            double utb = 53736.0;
            double x = 0.5791308486706011000e-3;
            double y = 0.4020579816732961219e-4;
            double xp = 2.55060238e-7;
            double yp = 1.860359247e-6;

            // Act
            double[] rc2t = new double[9];
            Sofa.C2txy(tta, ttb, uta, utb, x, y, xp, yp, rc2t);

            // Assert
            Assert.Equal(-0.1810332128306279253, rc2t[0], TOLERANCE);
            Assert.Equal(0.9834769806938520084, rc2t[1], TOLERANCE);
            Assert.Equal(0.6555551248057665829e-4, rc2t[2], TOLERANCE);
            Assert.Equal(-0.9834768134136142314, rc2t[3], TOLERANCE);
            Assert.Equal(-0.1810332203649529312, rc2t[4], TOLERANCE);
            Assert.Equal(0.5749800843594139912e-3, rc2t[5], TOLERANCE);
            Assert.Equal(0.5773474028619264494e-3, rc2t[6], TOLERANCE);
            Assert.Equal(0.3961816546911624260e-4, rc2t[7], TOLERANCE);
            Assert.Equal(0.9999998325501746670, rc2t[8], TOLERANCE);
        }
    }

    public class Sofa_t_cal2jd
    {
        [Fact]
        public void Cal2jd_ValidDate_ReturnsJulianDateParts()
        {
            // Arrange
            int iy = 2003;
            int im = 6;
            int id = 1;

            // Act
            double djm0 = 0;
            double djm = 0;
            int j = Sofa.Cal2jd(iy, im, id, ref djm0, ref djm);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(2400000.5, djm0);
            Assert.Equal(52791.0, djm);
        }
    }

    public class Sofa_t_cp
    {
        [Fact]
        public void Cp_CopiesVector_Successfully()
        {
            // Arrange
            double[] p = new double[3] { 0.3, 1.2, -2.5 };

            // Act
            double[] c = new double[3];
            Sofa.Cp(p, c);

            // Assert
            Assert.Equal(0.3, c[0]);
            Assert.Equal(1.2, c[1]);
            Assert.Equal(-2.5, c[2]);
        }
    }

    public class Sofa_t_cpv
    {
        [Fact]
        public void Cpv_CopiesPositionVelocity_Successfully()
        {
            // Arrange
            double[] pv = new double[6] { 0.3, 1.2, -2.5, -0.5, 3.1, 0.9 };

            // Act
            double[] c = new double[6];
            Sofa.Cpv(pv, c);

            // Assert
            Assert.Equal(0.3, c[0]);
            Assert.Equal(1.2, c[1]);
            Assert.Equal(-2.5, c[2]);
            Assert.Equal(-0.5, c[3]);
            Assert.Equal(3.1, c[4]);
            Assert.Equal(0.9, c[5]);
        }
    }

    public class Sofa_t_cr
    {
        [Fact]
        public void Cr_CopiesMatrix_Successfully()
        {
            // Arrange
            double[] r = new double[9]
            {
                2.0, 3.0, 2.0,
                3.0, 2.0, 3.0,
                3.0, 4.0, 5.0
            };

            // Act
            double[] c = new double[9];
            Sofa.Cr(r, c);

            // Assert
            for (int i = 0; i < 9; i++)
            {
                Assert.Equal(r[i], c[i]);
            }
        }
    }

    public class Sofa_t_d2dtf
    {
        [Fact]
        public void D2dtf_ValidJD_ReturnsDateTimeFields()
        {
            // Arrange
            string scale = "UTC";
            int ndp = 5;
            double d1 = 2400000.5;
            double d2 = 49533.99999;

            // Act
            int iy = 0;
            int im = 0;
            int id = 0;
            int[] ihmsf = new int[4];
            int j = Sofa.D2dtf(scale, ndp, d1, d2, ref iy, ref im, ref id, ihmsf);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(1994, iy);
            Assert.Equal(6, im);
            Assert.Equal(30, id);
            Assert.Equal(23, ihmsf[0]);
            Assert.Equal(59, ihmsf[1]);
            Assert.Equal(60, ihmsf[2]);
            Assert.Equal(13599, ihmsf[3]);
        }
    }

    public class Sofa_t_d2tf
    {
        [Fact]
        public void D2tf_NegativeDays_ReturnsCorrectComponents()
        {
            // Arrange
            int ndp = 4;
            double days = -0.987654321;

            // Act
            StringBuilder sign = new StringBuilder(2);
            int[] ihmsf = new int[4];
            Sofa.D2tf(ndp, days, sign, ihmsf);

            // Assert
            Assert.Equal("-", sign.ToString().TrimEnd('\0'));
            Assert.Equal(23, ihmsf[0]);
            Assert.Equal(42, ihmsf[1]);
            Assert.Equal(13, ihmsf[2]);
            Assert.Equal(3333, ihmsf[3]);
        }
    }

    public class Sofa_t_dat
    {
        [Fact]
        public void Dat_2003June_ReturnsLeapSeconds()
        {
            // Arrange
            int year = 2003;
            int month = 6;
            int day = 1;
            double frac = 0.0;

            // Act
            double deltat = 0;
            short j = Sofa.Dat(year, month, day, frac, ref deltat);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(32.0, deltat);
        }

        [Fact]
        public void Dat_2008January_ReturnsLeapSeconds()
        {
            // Arrange
            int year = 2008;
            int month = 1;
            int day = 17;
            double frac = 0.0;

            // Act
            double deltat = 0;
            short j = Sofa.Dat(year, month, day, frac, ref deltat);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(33.0, deltat);
        }

        [Fact]
        public void Dat_2017September_ReturnsLeapSeconds()
        {
            // Arrange
            int year = 2017;
            int month = 9;
            int day = 1;
            double frac = 0.0;

            // Act
            double deltat = 0;
            short j = Sofa.Dat(year, month, day, frac, ref deltat);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(37.0, deltat);
        }
    }

    public class Sofa_t_dtdb
    {
        private const double TOLERANCE = 1e-15;

        [Fact]
        public void Dtdb_ValidInputs_ReturnsCorrectValue()
        {
            // Arrange
            double date1 = 2448939.5;
            double date2 = 0.123;
            double ut = 0.76543;
            double elong = 5.0123;
            double u = 5525.242;
            double v = 3190.0;

            // Act
            double dtdb = Sofa.Dtdb(date1, date2, ut, elong, u, v);

            // Assert
            Assert.Equal(-0.1280368005936998991e-2, dtdb, TOLERANCE);
        }
    }

    public class Sofa_t_dtf2d
    {
        private const double TOLERANCE = 1e-6;

        [Fact]
        public void Dtf2d_ValidUTC_ReturnsJulianDate()
        {
            // Arrange
            string scale = "UTC";
            int iy = 1994;
            int im = 6;
            int id = 30;
            int ihr = 23;
            int imn = 59;
            double sec = 60.13599;

            // Act
            double d1 = 0;
            double d2 = 0;
            short j = Sofa.Dtf2d(scale, iy, im, id, ihr, imn, sec, ref d1, ref d2);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(2449534.49999, d1 + d2, TOLERANCE);
        }
    }

    public class Sofa_t_eceq06
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void Eceq06_ValidInputs_ReturnsEquatorialCoordinates()
        {
            // Arrange
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            double dl = 5.1;
            double db = -0.9;

            // Act
            double dr = 0;
            double dd = 0;
            Sofa.Eceq06(date1, date2, dl, db, ref dr, ref dd);

            // Assert
            Assert.Equal(5.533459733613627767, dr, TOLERANCE);
            Assert.Equal(-1.246542932554480576, dd, TOLERANCE);
        }
    }

    public class Sofa_t_ecm06
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void Ecm06_ValidInputs_ReturnsRotationMatrix()
        {
            // Arrange
            double date1 = 2456165.5;
            double date2 = 0.401182685;

            // Act
            double[] rm = new double[9];
            Sofa.Ecm06(date1, date2, rm);

            // Assert
            Assert.Equal(0.9999952427708701137, rm[0], TOLERANCE);
            Assert.Equal(-0.2829062057663042347e-2, rm[1], TOLERANCE);
            Assert.Equal(-0.1229163741100017629e-2, rm[2], TOLERANCE);
            Assert.Equal(0.3084546876908653562e-2, rm[3], TOLERANCE);
            Assert.Equal(0.9174891871550392514, rm[4], TOLERANCE);
            Assert.Equal(0.3977487611849338124, rm[5], TOLERANCE);
            Assert.Equal(0.2488512951527405928e-5, rm[6], TOLERANCE);
            Assert.Equal(-0.3977506604161195467, rm[7], TOLERANCE);
            Assert.Equal(0.9174935488232863071, rm[8], TOLERANCE);
        }
    }

    public class Sofa_t_ee00
    {
        private const double TOLERANCE = 1e-18;

        [Fact]
        public void Ee00_ValidInputs_ReturnsEquationOfEquinoxes()
        {
            // Arrange
            double date1 = 2400000.5;
            double date2 = 53736.0;
            double epsa = 0.4090789763356509900;
            double dpsi = -0.9630909107115582393e-5;

            // Act
            double ee = Sofa.Ee00(date1, date2, epsa, dpsi);

            // Assert
            Assert.Equal(-0.8834193235367965479e-5, ee, TOLERANCE);
        }
    }

    public class Sofa_t_ee00a
    {
        private const double TOLERANCE = 1e-18;

        [Fact]
        public void Ee00a_ValidInputs_ReturnsEquationOfEquinoxes()
        {
            // Arrange
            double date1 = 2400000.5;
            double date2 = 53736.0;

            // Act
            double ee = Sofa.Ee00a(date1, date2);

            // Assert
            Assert.Equal(-0.8834192459222588227e-5, ee, TOLERANCE);
        }
    }

    public class Sofa_t_ee00b
    {
        private const double TOLERANCE = 1e-18;

        [Fact]
        public void Ee00b_ValidInputs_ReturnsEquationOfEquinoxes()
        {
            // Arrange
            double date1 = 2400000.5;
            double date2 = 53736.0;

            // Act
            double ee = Sofa.Ee00b(date1, date2);

            // Assert
            Assert.Equal(-0.8835700060003032831e-5, ee, TOLERANCE);
        }
    }

    public class Sofa_t_ee06a
    {
        private const double TOLERANCE = 1e-15;

        [Fact]
        public void Ee06a_ValidInputs_ReturnsEquationOfEquinoxes()
        {
            // Arrange
            double date1 = 2400000.5;
            double date2 = 53736.0;

            // Act
            double ee = Sofa.Ee06a(date1, date2);

            // Assert
            Assert.Equal(-0.8834195072043790156e-5, ee, TOLERANCE);
        }
    }

    public class Sofa_t_eect00
    {
        private const double TOLERANCE = 1e-20;

        [Fact]
        public void Eect00_ValidInputs_ReturnsEquinoctialComplementaryTerm()
        {
            // Arrange
            double date1 = 2400000.5;
            double date2 = 53736.0;

            // Act
            double eect = Sofa.Eect00(date1, date2);

            // Assert
            Assert.Equal(0.2046085004885125264e-8, eect, TOLERANCE);
        }
    }

    public class Sofa_t_eform
    {
        [Fact]
        public void Eform_InvalidId_ReturnsError()
        {
            // Arrange
            SofaReferenceEllipsoids id = 0;

            // Act
            double a = 0;
            double f = 0;
            int j = Sofa.Eform(id, ref a, ref f);

            // Assert
            Assert.Equal(-1, j);
        }

        [Fact]
        public void Eform_WGS84_ReturnsCorrectParameters()
        {
            // Arrange
            SofaReferenceEllipsoids id = SofaReferenceEllipsoids.WGS84;

            // Act
            double a = 0;
            double f = 0;
            int j = Sofa.Eform(id, ref a, ref f);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(6378137.0, a, 1e-10);
            Assert.Equal(0.3352810664747480720e-2, f, 1e-18);
        }

        [Fact]
        public void Eform_GRS80_ReturnsCorrectParameters()
        {
            // Arrange
            SofaReferenceEllipsoids id = SofaReferenceEllipsoids.GRS80;

            // Act
            double a = 0;
            double f = 0;
            int j = Sofa.Eform(id, ref a, ref f);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(6378137.0, a, 1e-10);
            Assert.Equal(0.3352810681182318935e-2, f, 1e-18);
        }

        [Fact]
        public void Eform_WGS72_ReturnsCorrectParameters()
        {
            // Arrange
            SofaReferenceEllipsoids id = SofaReferenceEllipsoids.WGS72;

            // Act
            double a = 0;
            double f = 0;
            int j = Sofa.Eform(id, ref a, ref f);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(6378135.0, a, 1e-10);
            Assert.Equal(0.3352779454167504862e-2, f, 1e-18);
        }

        [Fact]
        public void Eform_InvalidId4_ReturnsError()
        {
            // Arrange
            int id = 4;

            // Act
            double a = 0;
            double f = 0;
            int j = Sofa.Eform((SofaReferenceEllipsoids)id, ref a, ref f);

            // Assert
            Assert.Equal(-1, j);
        }
    }

    public class Sofa_t_eo06a
    {
        private const double TOLERANCE = 1e-15;

        [Fact]
        public void Eo06a_ValidInputs_ReturnsEquationOfOrigins()
        {
            // Arrange
            double date1 = 2400000.5;
            double date2 = 53736.0;

            // Act
            double eo = Sofa.Eo06a(date1, date2);

            // Assert
            Assert.Equal(-0.1332882371941833644e-2, eo, TOLERANCE);
        }
    }

    public class Sofa_t_eors
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void Eors_ValidInputs_ReturnsEquationOfOrigins()
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
            double s = -0.1220040848472271978e-7;

            // Act
            double eo = Sofa.Eors(rnpb, s);

            // Assert
            Assert.Equal(-0.1332882715130744606e-2, eo, TOLERANCE);
        }
    }

    public class Sofa_t_epb
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Epb_ValidJD_ReturnsBesselianEpoch()
        {
            // Arrange
            double dj1 = 2415019.8135;
            double dj2 = 30103.18648;

            // Act
            double epb = Sofa.Epb(dj1, dj2);

            // Assert
            Assert.Equal(1982.418424159278580, epb, TOLERANCE);
        }
    }

    public class Sofa_t_epb2jd
    {
        private const double TOLERANCE = 1e-9;

        [Fact]
        public void Epb2jd_BesselianEpoch_ReturnsJulianDate()
        {
            // Arrange
            double epb = 1957.3;

            // Act
            double djm0 = 0;
            double djm = 0;
            Sofa.Epb2jd(epb, ref djm0, ref djm);

            // Assert
            Assert.Equal(2400000.5, djm0, TOLERANCE);
            Assert.Equal(35948.1915101513, djm, TOLERANCE);
        }
    }

    public class Sofa_t_epj
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Epj_ValidJD_ReturnsJulianEpoch()
        {
            // Arrange
            double dj1 = 2451545;
            double dj2 = -7392.5;

            // Act
            double epj = Sofa.Epj(dj1, dj2);

            // Assert
            Assert.Equal(1979.760438056125941, epj, TOLERANCE);
        }
    }

    public class Sofa_t_epj2jd
    {
        private const double TOLERANCE = 1e-9;

        [Fact]
        public void Epj2jd_JulianEpoch_ReturnsJulianDate()
        {
            // Arrange
            double epj = 1996.8;

            // Act
            double djm0 = 0;
            double djm = 0;
            Sofa.Epj2jd(epj, ref djm0, ref djm);

            // Assert
            Assert.Equal(2400000.5, djm0, TOLERANCE);
            Assert.Equal(50375.7, djm, TOLERANCE);
        }
    }

    public class Sofa_t_epv00
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void Epv00_ValidDate_ReturnsHeliocentriAndBarycentric()
        {
            // Arrange
            double date1 = 2400000.5;
            double date2 = 53411.52501161;

            // Act
            double[] pvh = new double[6];
            double[] pvb = new double[6];
            int j = Sofa.Epv00(date1, date2, pvh, pvb);

            // Assert
            Assert.Equal(0, j);

            // Heliocentric
            Assert.Equal(-0.7757238809297706813, pvh[0], TOLERANCE);
            Assert.Equal(0.5598052241363340596, pvh[1], TOLERANCE);
            Assert.Equal(0.2426998466481686993, pvh[2], TOLERANCE);
            Assert.Equal(-0.1091891824147313846e-1, pvh[3], 1e-15);
            Assert.Equal(-0.1247187268440845008e-1, pvh[4], 1e-15);
            Assert.Equal(-0.5407569418065039061e-2, pvh[5], 1e-15);

            // Barycentric
            Assert.Equal(-0.7714104440491111971, pvb[0], TOLERANCE);
            Assert.Equal(0.5598412061824171323, pvb[1], TOLERANCE);
            Assert.Equal(0.2425996277722452400, pvb[2], TOLERANCE);
            Assert.Equal(-0.1091874268116823295e-1, pvb[3], 1e-15);
            Assert.Equal(-0.1246525461732861538e-1, pvb[4], 1e-15);
            Assert.Equal(-0.5404773180966231279e-2, pvb[5], 1e-15);
        }
    }

    public class Sofa_t_eqec06
    {
        private const double TOLERANCE = 1e-14;

        [Fact]
        public void Eqec06_ValidInputs_ReturnsEclipticCoordinates()
        {
            // Arrange
            double date1 = 1234.5;
            double date2 = 2440000.5;
            double dr = 1.234;
            double dd = 0.987;

            // Act
            double dl = 0;
            double db = 0;
            Sofa.Eqec06(date1, date2, dr, dd, ref dl, ref db);

            // Assert
            Assert.Equal(1.342509918994654619, dl, TOLERANCE);
            Assert.Equal(0.5926215259704608132, db, TOLERANCE);
        }
    }

    public class Sofa_t_eqeq94
    {
        private const double TOLERANCE = 1e-17;

        [Fact]
        public void Eqeq94_ValidInputs_ReturnsEquinoctialEquation()
        {
            // Arrange
            double date1 = 2400000.5;
            double date2 = 41234.0;

            // Act
            double eqeq = Sofa.Eqeq94(date1, date2);

            // Assert
            Assert.Equal(0.5357758254609256894e-4, eqeq, TOLERANCE);
        }
    }

    public class Sofa_t_era00
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Era00_ValidDate_ReturnsEarthRotationAngle()
        {
            // Arrange
            double dj1 = 2400000.5;
            double dj2 = 54388.0;

            // Act
            double era = Sofa.Era00(dj1, dj2);

            // Assert
            Assert.Equal(0.4022837240028158102, era, TOLERANCE);
        }
    }

    public class Sofa_t_fad03
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Fad03_ValidInput_ReturnsMeanElongation()
        {
            // Act
            double result = Sofa.Fad03(0.80);

            // Assert
            Assert.Equal(1.946709205396925672, result, TOLERANCE);
        }
    }

    public class Sofa_t_fae03
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Fae03_ValidInput_ReturnsMeanLongitudeEarth()
        {
            // Act
            double result = Sofa.Fae03(0.80);

            // Assert
            Assert.Equal(1.744713738913081846, result, TOLERANCE);
        }
    }

    public class Sofa_t_faf03
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Faf03_ValidInput_ReturnsMeanArgumentLatitude()
        {
            // Act
            double result = Sofa.Faf03(0.80);

            // Assert
            Assert.Equal(0.2597711366745499518, result, TOLERANCE);
        }
    }

    public class Sofa_t_faju03
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Faju03_ValidInput_ReturnsMeanArgumentLatitude()
        {
            // Act
            double result = Sofa.Faju03(0.80);

            // Assert
            Assert.Equal(5.275711665202481138, result, TOLERANCE);
        }
    }

    public class Sofa_t_fal03
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Fal03_ValidInput_ReturnsMeanLongitudeLunar()
        {
            // Act
            double result = Sofa.Fal03(0.80);

            // Assert
            Assert.Equal(5.132369751108684150, result, TOLERANCE);
        }
    }

    public class Sofa_t_falp03
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Falp03_ValidInput_ReturnsMeanPerigeeArgument()
        {
            // Act
            double result = Sofa.Falp03(0.80);

            // Assert
            Assert.Equal(6.226797973505507345, result, TOLERANCE);
        }
    }

    public class Sofa_t_fama03
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Fama03_ValidInput_ReturnsMeanElongationMoon()
        {
            // Act
            double result = Sofa.Fama03(0.80);

            // Assert
            Assert.Equal(3.275506840277781492, result, TOLERANCE);
        }
    }

    public class Sofa_t_fame03
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Fame03_ValidInput_ReturnsMeanLongitudeMoonAscendingNode()
        {
            // Act
            double result = Sofa.Fame03(0.80);

            // Assert
            Assert.Equal(5.417338184297289661, result, TOLERANCE);
        }
    }

    public class Sofa_t_fane03
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Fane03_ValidInput_ReturnsMeanLongitudeAscendingNode()
        {
            // Act
            double result = Sofa.Fane03(0.80);

            // Assert
            Assert.Equal(2.079343830860413523, result, TOLERANCE);
        }
    }

    public class Sofa_t_faom03
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Faom03_ValidInput_ReturnsMeanLongitudeMoonAscendingNodeAlt()
        {
            // Act
            double result = Sofa.Faom03(0.80);

            // Assert
            Assert.Equal(-5.973618440951302183, result, TOLERANCE);
        }
    }

    public class Sofa_t_fapa03
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Fapa03_ValidInput_ReturnsMoonPericenter()
        {
            // Act
            double result = Sofa.Fapa03(0.80);

            // Assert
            Assert.Equal(0.1950884762240000000e-1, result, TOLERANCE);
        }
    }

    public class Sofa_t_fasa03
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Fasa03_ValidInput_ReturnsMeanLongitudeAscendingNodeAlt()
        {
            // Act
            double result = Sofa.Fasa03(0.80);

            // Assert
            Assert.Equal(5.371574539440827046, result, TOLERANCE);
        }
    }

    public class Sofa_t_faur03
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Faur03_ValidInput_ReturnsMeanElongationMoonAlt()
        {
            // Act
            double result = Sofa.Faur03(0.80);

            // Assert
            Assert.Equal(5.180636450180413523, result, TOLERANCE);
        }
    }

    public class Sofa_t_fave03
    {
        private const double TOLERANCE = 1e-12;

        [Fact]
        public void Fave03_ValidInput_ReturnsMeanVenusElongation()
        {
            // Act
            double result = Sofa.Fave03(0.80);

            // Assert
            Assert.Equal(3.424900460533758000, result, TOLERANCE);
        }
    }
}