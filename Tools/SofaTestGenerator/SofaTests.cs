using Xunit;
using ASCOM.Tools;

namespace ASCOM.AstrometryTools.Tests
{
    public class SofaA2afTests
    {
        [Fact]
        public void TestA2af()
        {
            int[] idmsf = new int[4];
            char s = ' ';
            
            SOFA.A2af(4, 2.345, ref s, idmsf);
            
            Assert.Equal('+', s);
            Assert.Equal(134, idmsf[0]);
            Assert.Equal(21, idmsf[1]);
            Assert.Equal(30, idmsf[2]);
            Assert.Equal(9706, idmsf[3]);
        }
    }

    public class SofaA2tfTests
    {
        [Fact]
        public void TestA2tf()
        {
            int[] ihmsf = new int[4];
            char s = ' ';
            
            SOFA.A2tf(4, -3.01234, ref s, ihmsf);
            
            Assert.Equal('-', s);
            Assert.Equal(11, ihmsf[0]);
            Assert.Equal(30, ihmsf[1]);
            Assert.Equal(22, ihmsf[2]);
            Assert.Equal(6484, ihmsf[3]);
        }
    }

    public class SofaAbTests
    {
        [Fact]
        public void TestAb()
        {
            double[] pnat = { -0.76321968546737951, -0.60869453983060384, -0.21676408580639883 };
            double[] v = { 2.1044018893653786e-5, -8.9108923304429319e-5, -3.8633714797716569e-5 };
            double s = 0.99980921395708788;
            double bm1 = 0.99999999506209258;
            double[] ppr = new double[3];
            
            SOFA.Ab(pnat, v, s, bm1, ppr);
            
            Assert.Equal(-0.7631631094219556269, ppr[0], 12);
            Assert.Equal(-0.6087553082505590832, ppr[1], 12);
            Assert.Equal(-0.2167926269368471279, ppr[2], 12);
        }
    }

    public class SofaAe2hdTests
    {
        [Fact]
        public void TestAe2hd()
        {
            double a = 5.5;
            double e = 1.1;
            double p = 0.7;
            double h = 0, d = 0;
            
            SOFA.Ae2hd(a, e, p, ref h, ref d);
            
            Assert.Equal(0.5933291115507309663, h, 14);
            Assert.Equal(0.9613934761647817620, d, 14);
        }
    }

    public class SofaAf2aTests
    {
        [Fact]
        public void TestAf2a()
        {
            double a = 0;
            
            int j = SOFA.Af2a('-', 45, 13, 27.2, ref a);
            
            Assert.Equal(0, j);
            Assert.Equal(-0.7893115794313644842, a, 12);
        }
    }

    public class SofaAnpTests
    {
        [Fact]
        public void TestAnp()
        {
            double result = SOFA.Anp(-0.1);
            Assert.Equal(6.183185307179586477, result, 12);
        }
    }

    public class SofaAnpmTests
    {
        [Fact]
        public void TestAnpm()
        {
            double result = SOFA.Anpm(-4.0);
            Assert.Equal(2.283185307179586477, result, 12);
        }
    }

    public class SofaApcgTests
    {
        [Fact]
        public void TestApcg()
        {
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            double[][] ebpv = new double[][] { 
                new double[] { 0.901310875, -0.417402664, -0.180982288 },
                new double[] { 0.00742727954, 0.0140507459, 0.00609045792 }
            };
            double[] ehp = { 0.903358544, -0.415395237, -0.180084014 };
            SOFA.ASTROM astrom = new SOFA.ASTROM();
            
            SOFA.Apcg(date1, date2, ebpv, ehp, ref astrom);
            
            Assert.Equal(12.65133794027378508, astrom.Pmt, 11);
            Assert.Equal(0.901310875, astrom.Eb[0], 12);
            Assert.Equal(-0.417402664, astrom.Eb[1], 12);
            Assert.Equal(-0.180982288, astrom.Eb[2], 12);
        }
    }

    public class SofaApcg13Tests
    {
        [Fact]
        public void TestApcg13()
        {
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            SOFA.ASTROM astrom = new SOFA.ASTROM();
            
            SOFA.Apcg13(date1, date2, ref astrom);
            
            Assert.Equal(12.65133794027378508, astrom.Pmt, 11);
            Assert.Equal(0.9013108747340644755, astrom.Eb[0], 12);
            Assert.Equal(-0.4174026640406119957, astrom.Eb[1], 12);
            Assert.Equal(-0.1809822877867817771, astrom.Eb[2], 12);
        }
    }

    public class SofaC2sTests
    {
        [Fact]
        public void TestC2s()
        {
            double[] p = { 100.0, -50.0, 25.0 };
            double theta = 0, phi = 0;
            
            SOFA.C2s(p, ref theta, ref phi);
            
            Assert.Equal(-0.4636476090008061162, theta, 14);
            Assert.Equal(0.2199879773954594463, phi, 14);
        }
    }

    public class SofaCal2jdTests
    {
        [Fact]
        public void TestCal2jd()
        {
            double djm0 = 0, djm = 0;
            
            int j = SOFA.Cal2jd(2003, 6, 1, ref djm0, ref djm);
            
            Assert.Equal(0, j);
            Assert.Equal(2400000.5, djm0, 0);
            Assert.Equal(52791.0, djm, 0);
        }
    }

    public class SofaCpTests
    {
        [Fact]
        public void TestCp()
        {
            double[] p = { 0.3, 1.2, -2.5 };
            double[] c = new double[3];
            
            SOFA.Cp(p, c);
            
            Assert.Equal(0.3, c[0], 0);
            Assert.Equal(1.2, c[1], 0);
            Assert.Equal(-2.5, c[2], 0);
        }
    }

    public class SofaCpvTests
    {
        [Fact]
        public void TestCpv()
        {
            double[][] pv = new double[][] {
                new double[] { 0.3, 1.2, -2.5 },
                new double[] { -0.5, 3.1, 0.9 }
            };
            double[][] c = new double[2][];
            c[0] = new double[3];
            c[1] = new double[3];
            
            SOFA.Cpv(pv, c);
            
            Assert.Equal(0.3, c[0][0], 0);
            Assert.Equal(1.2, c[0][1], 0);
            Assert.Equal(-2.5, c[0][2], 0);
            Assert.Equal(-0.5, c[1][0], 0);
            Assert.Equal(3.1, c[1][1], 0);
            Assert.Equal(0.9, c[1][2], 0);
        }
    }

    public class SofaCrTests
    {
        [Fact]
        public void TestCr()
        {
            double[][] r = new double[][] {
                new double[] { 2.0, 3.0, 2.0 },
                new double[] { 3.0, 2.0, 3.0 },
                new double[] { 3.0, 4.0, 5.0 }
            };
            double[][] c = new double[3][];
            for (int i = 0; i < 3; i++) c[i] = new double[3];
            
            SOFA.Cr(r, c);
            
            Assert.Equal(2.0, c[0][0], 0);
            Assert.Equal(3.0, c[0][1], 0);
            Assert.Equal(2.0, c[0][2], 0);
            Assert.Equal(3.0, c[1][0], 0);
            Assert.Equal(2.0, c[1][1], 0);
            Assert.Equal(3.0, c[1][2], 0);
            Assert.Equal(3.0, c[2][0], 0);
            Assert.Equal(4.0, c[2][1], 0);
            Assert.Equal(5.0, c[2][2], 0);
        }
    }

    public class SofaIrTests
    {
        [Fact]
        public void TestIr()
        {
            double[][] r = new double[][] {
                new double[] { 2.0, 3.0, 2.0 },
                new double[] { 3.0, 2.0, 3.0 },
                new double[] { 3.0, 4.0, 5.0 }
            };
            
            SOFA.Ir(r);
            
            Assert.Equal(1.0, r[0][0], 0);
            Assert.Equal(0.0, r[0][1], 0);
            Assert.Equal(0.0, r[0][2], 0);
            Assert.Equal(0.0, r[1][0], 0);
            Assert.Equal(1.0, r[1][1], 0);
            Assert.Equal(0.0, r[1][2], 0);
            Assert.Equal(0.0, r[2][0], 0);
            Assert.Equal(0.0, r[2][1], 0);
            Assert.Equal(1.0, r[2][2], 0);
        }
    }

    public class SofaP2sTests
    {
        [Fact]
        public void TestP2s()
        {
            double[] p = { 100.0, -50.0, 25.0 };
            double theta = 0, phi = 0, r = 0;
            
            SOFA.P2s(p, ref theta, ref phi, ref r);
            
            Assert.Equal(-0.4636476090008061162, theta, 12);
            Assert.Equal(0.2199879773954594463, phi, 12);
            Assert.Equal(114.5643923738960002, r, 9);
        }
    }

    public class SofaPmTests
    {
        [Fact]
        public void TestPm()
        {
            double[] p = { 0.3, 1.2, -2.5 };
            
            double r = SOFA.Pm(p);
            
            Assert.Equal(2.789265136196270604, r, 12);
        }
    }

    public class SofaPmpTests
    {
        [Fact]
        public void TestPmp()
        {
            double[] a = { 2.0, 2.0, 3.0 };
            double[] b = { 1.0, 3.0, 4.0 };
            double[] amb = new double[3];
            
            SOFA.Pmp(a, b, amb);
            
            Assert.Equal(1.0, amb[0], 12);
            Assert.Equal(-1.0, amb[1], 12);
            Assert.Equal(-1.0, amb[2], 12);
        }
    }

    public class SofaPnTests
    {
        [Fact]
        public void TestPn()
        {
            double[] p = { 0.3, 1.2, -2.5 };
            double r = 0;
            double[] u = new double[3];
            
            SOFA.Pn(p, ref r, u);
            
            Assert.Equal(2.789265136196270604, r, 12);
            Assert.Equal(0.1075552109073112058, u[0], 12);
            Assert.Equal(0.4302208436292448232, u[1], 12);
            Assert.Equal(-0.8962934242275933816, u[2], 12);
        }
    }

    public class SofaPppTests
    {
        [Fact]
        public void TestPpp()
        {
            double[] a = { 2.0, 2.0, 3.0 };
            double[] b = { 1.0, 3.0, 4.0 };
            double[] apb = new double[3];
            
            SOFA.Ppp(a, b, apb);
            
            Assert.Equal(3.0, apb[0], 12);
            Assert.Equal(5.0, apb[1], 12);
            Assert.Equal(7.0, apb[2], 12);
        }
    }

    public class SofaPpspTests
    {
        [Fact]
        public void TestPpsp()
        {
            double[] a = { 2.0, 2.0, 3.0 };
            double s = 5.0;
            double[] b = { 1.0, 3.0, 4.0 };
            double[] apsb = new double[3];
            
            SOFA.Ppsp(a, s, b, apsb);
            
            Assert.Equal(7.0, apsb[0], 12);
            Assert.Equal(17.0, apsb[1], 12);
            Assert.Equal(23.0, apsb[2], 12);
        }
    }

    public class SofaPv2pTests
    {
        [Fact]
        public void TestPv2p()
        {
            double[][] pv = new double[][] {
                new double[] { 0.3, 1.2, -2.5 },
                new double[] { -0.5, 3.1, 0.9 }
            };
            double[] p = new double[3];
            
            SOFA.Pv2p(pv, p);
            
            Assert.Equal(0.3, p[0], 0);
            Assert.Equal(1.2, p[1], 0);
            Assert.Equal(-2.5, p[2], 0);
        }
    }

    public class SofaPv2sTests
    {
        [Fact]
        public void TestPv2s()
        {
            double[][] pv = new double[][] {
                new double[] { -0.4514964673880165, 0.03093394277342585, 0.05594668105108779 },
                new double[] { 1.292270850663260e-5, 2.652814182060692e-6, 2.568431853930293e-6 }
            };
            double theta = 0, phi = 0, r = 0, td = 0, pd = 0, rd = 0;
            
            SOFA.Pv2s(pv, ref theta, ref phi, ref r, ref td, ref pd, ref rd);
            
            Assert.Equal(3.073185307179586515, theta, 12);
            Assert.Equal(0.1229999999999999992, phi, 12);
            Assert.Equal(0.4559999999999999757, r, 12);
            Assert.Equal(-0.7800000000000000364e-5, td, 16);
            Assert.Equal(0.9010000000000001639e-5, pd, 16);
            Assert.Equal(-0.1229999999999999832e-4, rd, 16);
        }
    }

    public class SofaPvdpvTests
    {
        [Fact]
        public void TestPvdpv()
        {
            double[][] a = new double[][] {
                new double[] { 2.0, 2.0, 3.0 },
                new double[] { 6.0, 0.0, 4.0 }
            };
            double[][] b = new double[][] {
                new double[] { 1.0, 3.0, 4.0 },
                new double[] { 0.0, 2.0, 8.0 }
            };
            double[] adb = new double[2];
            
            SOFA.Pvdpv(a, b, adb);
            
            Assert.Equal(20.0, adb[0], 12);
            Assert.Equal(50.0, adb[1], 12);
        }
    }

    public class SofaPvmTests
    {
        [Fact]
        public void TestPvm()
        {
            double[][] pv = new double[][] {
                new double[] { 0.3, 1.2, -2.5 },
                new double[] { 0.45, -0.25, 1.1 }
            };
            double r = 0, s = 0;
            
            SOFA.Pvm(pv, ref r, ref s);
            
            Assert.Equal(2.789265136196270604, r, 12);
            Assert.Equal(1.214495780149111922, s, 12);
        }
    }

    public class SofaPvmpvTests
    {
        [Fact]
        public void TestPvmpv()
        {
            double[][] a = new double[][] {
                new double[] { 2.0, 2.0, 3.0 },
                new double[] { 5.0, 6.0, 3.0 }
            };
            double[][] b = new double[][] {
                new double[] { 1.0, 3.0, 4.0 },
                new double[] { 3.0, 2.0, 1.0 }
            };
            double[][] amb = new double[2][];
            for (int i = 0; i < 2; i++) amb[i] = new double[3];
            
            SOFA.Pvmpv(a, b, amb);
            
            Assert.Equal(1.0, amb[0][0], 12);
            Assert.Equal(-1.0, amb[0][1], 12);
            Assert.Equal(-1.0, amb[0][2], 12);
            Assert.Equal(2.0, amb[1][0], 12);
            Assert.Equal(4.0, amb[1][1], 12);
            Assert.Equal(2.0, amb[1][2], 12);
        }
    }

    public class SofaPvppvTests
    {
        [Fact]
        public void TestPvppv()
        {
            double[][] a = new double[][] {
                new double[] { 2.0, 2.0, 3.0 },
                new double[] { 5.0, 6.0, 3.0 }
            };
            double[][] b = new double[][] {
                new double[] { 1.0, 3.0, 4.0 },
                new double[] { 3.0, 2.0, 1.0 }
            };
            double[][] apb = new double[2][];
            for (int i = 0; i < 2; i++) apb[i] = new double[3];
            
            SOFA.Pvppv(a, b, apb);
            
            Assert.Equal(3.0, apb[0][0], 12);
            Assert.Equal(5.0, apb[0][1], 12);
            Assert.Equal(7.0, apb[0][2], 12);
            Assert.Equal(8.0, apb[1][0], 12);
            Assert.Equal(8.0, apb[1][1], 12);
            Assert.Equal(4.0, apb[1][2], 12);
        }
    }

    public class SofaPvxpvTests
    {
        [Fact]
        public void TestPvxpv()
        {
            double[][] a = new double[][] {
                new double[] { 2.0, 2.0, 3.0 },
                new double[] { 6.0, 0.0, 4.0 }
            };
            double[][] b = new double[][] {
                new double[] { 1.0, 3.0, 4.0 },
                new double[] { 0.0, 2.0, 8.0 }
            };
            double[][] axb = new double[2][];
            for (int i = 0; i < 2; i++) axb[i] = new double[3];
            
            SOFA.Pvxpv(a, b, axb);
            
            Assert.Equal(-1.0, axb[0][0], 12);
            Assert.Equal(-5.0, axb[0][1], 12);
            Assert.Equal(4.0, axb[0][2], 12);
            Assert.Equal(-2.0, axb[1][0], 12);
            Assert.Equal(-36.0, axb[1][1], 12);
            Assert.Equal(22.0, axb[1][2], 12);
        }
    }

    public class SofaPxpTests
    {
        [Fact]
        public void TestPxp()
        {
            double[] a = { 2.0, 2.0, 3.0 };
            double[] b = { 1.0, 3.0, 4.0 };
            double[] axb = new double[3];
            
            SOFA.Pxp(a, b, axb);
            
            Assert.Equal(-1.0, axb[0], 12);
            Assert.Equal(-5.0, axb[1], 12);
            Assert.Equal(4.0, axb[2], 12);
        }
    }

    public class SofaRm2vTests
    {
        [Fact]
        public void TestRm2v()
        {
            double[][] r = new double[][] {
                new double[] { 0.00, -0.80, -0.60 },
                new double[] { 0.80, -0.36, 0.48 },
                new double[] { 0.60, 0.48, -0.64 }
            };
            double[] w = new double[3];
            
            SOFA.Rm2v(r, w);
            
            Assert.Equal(0.0, w[0], 12);
            Assert.Equal(1.413716694115406957, w[1], 12);
            Assert.Equal(-1.884955592153875943, w[2], 12);
        }
    }

    public class SofaRv2mTests
    {
        [Fact]
        public void TestRv2m()
        {
            double[] w = { 0.0, 1.41371669, -1.88495559 };
            double[][] r = new double[3][];
            for (int i = 0; i < 3; i++) r[i] = new double[3];
            
            SOFA.Rv2m(w, r);
            
            Assert.Equal(-0.7071067782221119905, r[0][0], 14);
            Assert.Equal(-0.5656854276809129651, r[0][1], 14);
            Assert.Equal(-0.4242640700104211225, r[0][2], 14);
            Assert.Equal(0.5656854276809129651, r[1][0], 14);
            Assert.Equal(-0.0925483394532274246, r[1][1], 14);
            Assert.Equal(-0.8194112531408833269, r[1][2], 14);
            Assert.Equal(0.4242640700104211225, r[2][0], 14);
            Assert.Equal(-0.8194112531408833269, r[2][1], 14);
            Assert.Equal(0.3854415612311154341, r[2][2], 14);
        }
    }

    public class SofaRxTests
    {
        [Fact]
        public void TestRx()
        {
            double phi = 0.3456789;
            double[][] r = new double[][] {
                new double[] { 2.0, 3.0, 2.0 },
                new double[] { 3.0, 2.0, 3.0 },
                new double[] { 3.0, 4.0, 5.0 }
            };
            
            SOFA.Rx(phi, r);
            
            Assert.Equal(2.0, r[0][0], 0);
            Assert.Equal(3.0, r[0][1], 0);
            Assert.Equal(2.0, r[0][2], 0);
            Assert.Equal(3.839043388235612460, r[1][0], 12);
            Assert.Equal(3.237033249594111899, r[1][1], 12);
            Assert.Equal(4.516714379005982719, r[1][2], 12);
            Assert.Equal(1.806030415924501684, r[2][0], 12);
            Assert.Equal(3.085711545336372503, r[2][1], 12);
            Assert.Equal(3.687721683977873065, r[2][2], 12);
        }
    }

    public class SofaRxpTests
    {
        [Fact]
        public void TestRxp()
        {
            double[][] r = new double[][] {
                new double[] { 2.0, 3.0, 2.0 },
                new double[] { 3.0, 2.0, 3.0 },
                new double[] { 3.0, 4.0, 5.0 }
            };
            double[] p = { 0.2, 1.5, 0.1 };
            double[] rp = new double[3];
            
            SOFA.Rxp(r, p, rp);
            
            Assert.Equal(5.1, rp[0], 12);
            Assert.Equal(3.9, rp[1], 12);
            Assert.Equal(7.1, rp[2], 12);
        }
    }

    public class SofaRxpvTests
    {
        [Fact]
        public void TestRxpv()
        {
            double[][] r = new double[][] {
                new double[] { 2.0, 3.0, 2.0 },
                new double[] { 3.0, 2.0, 3.0 },
                new double[] { 3.0, 4.0, 5.0 }
            };
            double[][] pv = new double[][] {
                new double[] { 0.2, 1.5, 0.1 },
                new double[] { 1.5, 0.2, 0.1 }
            };
            double[][] rpv = new double[2][];
            for (int i = 0; i < 2; i++) rpv[i] = new double[3];
            
            SOFA.Rxpv(r, pv, rpv);
            
            Assert.Equal(5.1, rpv[0][0], 12);
            Assert.Equal(3.9, rpv[0][1], 12);
            Assert.Equal(7.1, rpv[0][2], 12);
            Assert.Equal(3.8, rpv[1][0], 12);
            Assert.Equal(5.2, rpv[1][1], 12);
            Assert.Equal(5.8, rpv[1][2], 12);
        }
    }

    public class SofaRxrTests
    {
        [Fact]
        public void TestRxr()
        {
            double[][] a = new double[][] {
                new double[] { 2.0, 3.0, 2.0 },
                new double[] { 3.0, 2.0, 3.0 },
                new double[] { 3.0, 4.0, 5.0 }
            };
            double[][] b = new double[][] {
                new double[] { 1.0, 2.0, 2.0 },
                new double[] { 4.0, 1.0, 1.0 },
                new double[] { 3.0, 0.0, 1.0 }
            };
            double[][] atb = new double[3][];
            for (int i = 0; i < 3; i++) atb[i] = new double[3];
            
            SOFA.Rxr(a, b, atb);
            
            Assert.Equal(20.0, atb[0][0], 12);
            Assert.Equal(7.0, atb[0][1], 12);
            Assert.Equal(9.0, atb[0][2], 12);
            Assert.Equal(20.0, atb[1][0], 12);
            Assert.Equal(8.0, atb[1][1], 12);
            Assert.Equal(11.0, atb[1][2], 12);
            Assert.Equal(34.0, atb[2][0], 12);
            Assert.Equal(10.0, atb[2][1], 12);
            Assert.Equal(15.0, atb[2][2], 12);
        }
    }

    public class SofaRyTests
    {
        [Fact]
        public void TestRy()
        {
            double theta = 0.3456789;
            double[][] r = new double[][] {
                new double[] { 2.0, 3.0, 2.0 },
                new double[] { 3.0, 2.0, 3.0 },
                new double[] { 3.0, 4.0, 5.0 }
            };
            
            SOFA.Ry(theta, r);
            
            Assert.Equal(0.8651847818978159930, r[0][0], 12);
            Assert.Equal(1.467194920539316554, r[0][1], 12);
            Assert.Equal(0.1875137911274457342, r[0][2], 12);
            Assert.Equal(3, r[1][0], 12);
            Assert.Equal(2, r[1][1], 12);
            Assert.Equal(3, r[1][2], 12);
            Assert.Equal(3.500207892850427330, r[2][0], 12);
            Assert.Equal(4.779889022262298150, r[2][1], 12);
            Assert.Equal(5.381899160903798712, r[2][2], 12);
        }
    }

    public class SofaRzTests
    {
        [Fact]
        public void TestRz()
        {
            double psi = 0.3456789;
            double[][] r = new double[][] {
                new double[] { 2.0, 3.0, 2.0 },
                new double[] { 3.0, 2.0, 3.0 },
                new double[] { 3.0, 4.0, 5.0 }
            };
            
            SOFA.Rz(psi, r);
            
            Assert.Equal(2.898197754208926769, r[0][0], 12);
            Assert.Equal(3.500207892850427330, r[0][1], 12);
            Assert.Equal(2.898197754208926769, r[0][2], 12);
            Assert.Equal(2.144865911309686813, r[1][0], 12);
            Assert.Equal(0.865184781897815993, r[1][1], 12);
            Assert.Equal(2.144865911309686813, r[1][2], 12);
            Assert.Equal(3.0, r[2][0], 12);
            Assert.Equal(4.0, r[2][1], 12);
            Assert.Equal(5.0, r[2][2], 12);
        }
    }

    public class SofaS2cTests
    {
        [Fact]
        public void TestS2c()
        {
            double[] c = new double[3];
            
            SOFA.S2c(3.0123, -0.999, c);
            
            Assert.Equal(-0.5366267667260523906, c[0], 12);
            Assert.Equal(0.0697711109765145365, c[1], 12);
            Assert.Equal(-0.8409302618566214041, c[2], 12);
        }
    }

    public class SofaS2pTests
    {
        [Fact]
        public void TestS2p()
        {
            double[] p = new double[3];
            
            SOFA.S2p(-3.21, 0.123, 0.456, p);
            
            Assert.Equal(-0.4514964673880165228, p[0], 12);
            Assert.Equal(0.0309339427734258688, p[1], 12);
            Assert.Equal(0.0559466810510877933, p[2], 12);
        }
    }

    public class SofaS2pvTests
    {
        [Fact]
        public void TestS2pv()
        {
            double[][] pv = new double[2][];
            pv[0] = new double[3];
            pv[1] = new double[3];
            
            SOFA.S2pv(-3.21, 0.123, 0.456, -7.8e-6, 9.01e-6, -1.23e-5, pv);
            
            Assert.Equal(-0.4514964673880165228, pv[0][0], 12);
            Assert.Equal(0.0309339427734258688, pv[0][1], 12);
            Assert.Equal(0.0559466810510877933, pv[0][2], 12);
            Assert.Equal(0.1292270850663260170e-4, pv[1][0], 16);
            Assert.Equal(0.2652814182060691422e-5, pv[1][1], 16);
            Assert.Equal(0.2568431853930292259e-5, pv[1][2], 16);
        }
    }

    public class SofaSxpTests
    {
        [Fact]
        public void TestSxp()
        {
            double s = 2.0;
            double[] p = { 0.3, 1.2, -2.5 };
            double[] sp = new double[3];
            
            SOFA.Sxp(s, p, sp);
            
            Assert.Equal(0.6, sp[0], 0);
            Assert.Equal(2.4, sp[1], 0);
            Assert.Equal(-5.0, sp[2], 0);
        }
    }

    public class SofaSxpvTests
    {
        [Fact]
        public void TestSxpv()
        {
            double s = 2.0;
            double[][] pv = new double[][] {
                new double[] { 0.3, 1.2, -2.5 },
                new double[] { 0.5, 3.2, -0.7 }
            };
            double[][] spv = new double[2][];
            for (int i = 0; i < 2; i++) spv[i] = new double[3];
            
            SOFA.Sxpv(s, pv, spv);
            
            Assert.Equal(0.6, spv[0][0], 0);
            Assert.Equal(2.4, spv[0][1], 0);
            Assert.Equal(-5.0, spv[0][2], 0);
            Assert.Equal(1.0, spv[1][0], 0);
            Assert.Equal(6.4, spv[1][1], 0);
            Assert.Equal(-1.4, spv[1][2], 0);
        }
    }

    public class SofaTrTests
    {
        [Fact]
        public void TestTr()
        {
            double[][] r = new double[][] {
                new double[] { 2.0, 3.0, 2.0 },
                new double[] { 3.0, 2.0, 3.0 },
                new double[] { 3.0, 4.0, 5.0 }
            };
            double[][] rt = new double[3][];
            for (int i = 0; i < 3; i++) rt[i] = new double[3];
            
            SOFA.Tr(r, rt);
            
            Assert.Equal(2.0, rt[0][0], 0);
            Assert.Equal(3.0, rt[0][1], 0);
            Assert.Equal(3.0, rt[0][2], 0);
            Assert.Equal(3.0, rt[1][0], 0);
            Assert.Equal(2.0, rt[1][1], 0);
            Assert.Equal(4.0, rt[1][2], 0);
            Assert.Equal(2.0, rt[2][0], 0);
            Assert.Equal(3.0, rt[2][1], 0);
            Assert.Equal(5.0, rt[2][2], 0);
        }
    }

    public class SofaTrxpTests
    {
        [Fact]
        public void TestTrxp()
        {
            double[][] r = new double[][] {
                new double[] { 2.0, 3.0, 2.0 },
                new double[] { 3.0, 2.0, 3.0 },
                new double[] { 3.0, 4.0, 5.0 }
            };
            double[] p = { 0.2, 1.5, 0.1 };
            double[] trp = new double[3];
            
            SOFA.Trxp(r, p, trp);
            
            Assert.Equal(5.2, trp[0], 12);
            Assert.Equal(4.0, trp[1], 12);
            Assert.Equal(5.4, trp[2], 12);
        }
    }

    public class SofaTrxpvTests
    {
        [Fact]
        public void TestTrxpv()
        {
            double[][] r = new double[][] {
                new double[] { 2.0, 3.0, 2.0 },
                new double[] { 3.0, 2.0, 3.0 },
                new double[] { 3.0, 4.0, 5.0 }
            };
            double[][] pv = new double[][] {
                new double[] { 0.2, 1.5, 0.1 },
                new double[] { 1.5, 0.2, 0.1 }
            };
            double[][] trpv = new double[2][];
            for (int i = 0; i < 2; i++) trpv[i] = new double[3];
            
            SOFA.Trxpv(r, pv, trpv);
            
            Assert.Equal(5.2, trpv[0][0], 12);
            Assert.Equal(4.0, trpv[0][1], 12);
            Assert.Equal(5.4, trpv[0][2], 12);
            Assert.Equal(3.9, trpv[1][0], 12);
            Assert.Equal(5.3, trpv[1][1], 12);
            Assert.Equal(4.1, trpv[1][2], 12);
        }
    }

    public class SofaZpTests
    {
        [Fact]
        public void TestZp()
        {
            double[] p = { 0.3, 1.2, -2.5 };
            
            SOFA.Zp(p);
            
            Assert.Equal(0.0, p[0], 0);
            Assert.Equal(0.0, p[1], 0);
            Assert.Equal(0.0, p[2], 0);
        }
    }

    public class SofaZpvTests
    {
        [Fact]
        public void TestZpv()
        {
            double[][] pv = new double[][] {
                new double[] { 0.3, 1.2, -2.5 },
                new double[] { -0.5, 3.1, 0.9 }
            };
            
            SOFA.Zpv(pv);
            
            Assert.Equal(0.0, pv[0][0], 0);
            Assert.Equal(0.0, pv[0][1], 0);
            Assert.Equal(0.0, pv[0][2], 0);
            Assert.Equal(0.0, pv[1][0], 0);
            Assert.Equal(0.0, pv[1][1], 0);
            Assert.Equal(0.0, pv[1][2], 0);
        }
    }

    public class SofaZrTests
    {
        [Fact]
        public void TestZr()
        {
            double[][] r = new double[][] {
                new double[] { 2.0, 3.0, 3.0 },
                new double[] { 3.0, 2.0, 4.0 },
                new double[] { 2.0, 3.0, 5.0 }
            };
            
            SOFA.Zr(r);
            
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Assert.Equal(0.0, r[i][j], 0);
                }
            }
        }
    }

    public class SofaPdpTests
    {
        [Fact]
        public void TestPdp()
        {
            double[] a = { 2.0, 2.0, 3.0 };
            double[] b = { 1.0, 3.0, 4.0 };
            
            double adb = SOFA.Pdp(a, b);
            
            Assert.Equal(20, adb, 12);
        }
    }

    public class SofaPapTests
    {
        [Fact]
        public void TestPap()
        {
            double[] a = { 1.0, 0.1, 0.2 };
            double[] b = { -3.0, 1e-3, 0.2 };
            
            double theta = SOFA.Pap(a, b);
            
            Assert.Equal(0.3671514267841113674, theta, 12);
        }
    }

    public class SofaPasTests
    {
        [Fact]
        public void TestPas()
        {
            double al = 1.0;
            double ap = 0.1;
            double bl = 0.2;
            double bp = -1.0;
            
            double theta = SOFA.Pas(al, ap, bl, bp);
            
            Assert.Equal(-2.724544922932270424, theta, 12);
        }
    }

    public class SofaSeppTests
    {
        [Fact]
        public void TestSepp()
        {
            double[] a = { 1.0, 0.1, 0.2 };
            double[] b = { -3.0, 1e-3, 0.2 };
            
            double s = SOFA.Sepp(a, b);
            
            Assert.Equal(2.860391919024660768, s, 12);
        }
    }

    public class SofaSepsTests
    {
        [Fact]
        public void TestSeps()
        {
            double al = 1.0;
            double ap = 0.1;
            double bl = 0.2;
            double bp = -3.0;
            
            double s = SOFA.Seps(al, ap, bl, bp);
            
            Assert.Equal(2.346722016996998842, s, 14);
        }
    }

    public class SofaRefcoTests
    {
        [Fact]
        public void TestRefco()
        {
            double phpa = 800.0;
            double tc = 10.0;
            double rh = 0.9;
            double wl = 0.4;
            double refa = 0, refb = 0;
            
            SOFA.Refco(phpa, tc, rh, wl, ref refa, ref refb);
            
            Assert.Equal(0.2264949956241415009e-3, refa, 15);
            Assert.Equal(-0.2598658261729343970e-6, refb, 18);
        }
    }
}