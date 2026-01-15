using ASCOM.Tools;
using System.Net.NetworkInformation;
using Xunit;

namespace SOFATests
{
    public class SofaPppTests
    {
        [Fact]
        public void TestPpp()
        {
            double[] a = { 2.0, 2.0, 3.0 };
            double[] b = { 1.0, 3.0, 4.0 };
            double[] apb = new double[3];

            Sofa.Ppp(a, b, apb);

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

            Sofa.Ppsp(a, s, b, apsb);

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
            double[] pv = new double[] { 0.3, 1.2, -2.5, -0.5, 3.1, 0.9 };
            double[] p = new double[3];

            Sofa.Pv2p(pv, p);

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
            double[] pv = new double[] { -0.4514964673880165, 0.03093394277342585, 0.05594668105108779, 1.292270850663260e-5, 2.652814182060692e-6, 2.568431853930293e-6 };
            double theta = 0, phi = 0, r = 0, td = 0, pd = 0, rd = 0;

            Sofa.Pv2s(pv, ref theta, ref phi, ref r, ref td, ref pd, ref rd);

            Assert.Equal(3.073185307179586515, theta, 12);
            Assert.Equal(0.1229999999999999992, phi, 12);
            Assert.Equal(0.4559999999999999757, r, 12);
            Assert.Equal(-0.7800000000000000364e-5, td, 15);
            Assert.Equal(0.9010000000000001639e-5, pd, 15);
            Assert.Equal(-0.1229999999999999832e-4, rd, 15);
        }
    }

    public class SofaPvdpvTests
    {
        [Fact]
        public void TestPvdpv()
        {
            double[] a = new double[] { 2.0, 2.0, 3.0, 6.0, 0.0, 4.0 };
            double[] b = new double[] { 1.0, 3.0, 4.0, 0.0, 2.0, 8.0 };
            double[] adb = new double[2];

            Sofa.Pvdpv(a, b, adb);

            Assert.Equal(20.0, adb[0], 12);
            Assert.Equal(50.0, adb[1], 12);
        }
    }

    public class SofaPvmTests
    {
        [Fact]
        public void TestPvm()
        {
            double[] pv = new double[] { 0.3, 1.2, -2.5, 0.45, -0.25, 1.1 };
            double r = 0, s = 0;

            Sofa.Pvm(pv, ref r, ref s);

            Assert.Equal(2.789265136196270604, r, 12);
            Assert.Equal(1.214495780149111922, s, 12);
        }
    }

    public class SofaPvmpvTests
    {
        [Fact]
        public void TestPvmpv()
        {
            double[] a = new double[] { 2.0, 2.0, 3.0, 5.0, 6.0, 3.0 };
            double[] b = new double[] { 1.0, 3.0, 4.0, 3.0, 2.0, 1.0 };
            double[] amb = new double[6];

            Sofa.Pvmpv(a, b, amb);

            Assert.Equal(1.0, amb[0], 12);
            Assert.Equal(-1.0, amb[1], 12);
            Assert.Equal(-1.0, amb[2], 12);
            Assert.Equal(2.0, amb[3], 12);
            Assert.Equal(4.0, amb[4], 12);
            Assert.Equal(2.0, amb[5], 12);
        }
    }

    public class SofaPvppvTests
    {
        [Fact]
        public void TestPvppv()
        {
            double[] a = new double[] { 2.0, 2.0, 3.0, 5.0, 6.0, 3.0 };
            double[] b = new double[] { 1.0, 3.0, 4.0, 3.0, 2.0, 1.0 };
            double[] apb = new double[6];

            Sofa.Pvppv(a, b, apb);

            Assert.Equal(3.0, apb[0], 12);
            Assert.Equal(5.0, apb[1], 12);
            Assert.Equal(7.0, apb[2], 12);
            Assert.Equal(8.0, apb[3], 12);
            Assert.Equal(8.0, apb[4], 12);
            Assert.Equal(4.0, apb[5], 12);
        }
    }

    public class SofaPvxpvTests
    {
        [Fact]
        public void TestPvxpv()
        {
            double[] a = new double[] {2.0, 2.0, 3.0 , 6.0, 0.0, 4.0 };
            double[] b = new double[] {1.0, 3.0, 4.0 ,0.0, 2.0, 8.0 };
            double[] axb = new double[6];

            Sofa.Pvxpv(a, b, axb);

            Assert.Equal(-1.0, axb[0], 12);
            Assert.Equal(-5.0, axb[1], 12);
            Assert.Equal(4.0, axb[2], 12);
            Assert.Equal(-2.0, axb[3], 12);
            Assert.Equal(-36.0, axb[4], 12);
            Assert.Equal(22.0, axb[5], 12);
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

            Sofa.Pxp(a, b, axb);

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
            double[] r = new double[] { 0.00, -0.80, -0.60 ,0.80, -0.36, 0.48 ,0.60, 0.48, -0.64 };
            double[] w = new double[9];

            Sofa.Rm2v(r, w);

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
            double[] r = new double[9];

            Sofa.Rv2m(w, r);

            Assert.Equal(-0.7071067782221119905, r[0], 14);
            Assert.Equal(-0.5656854276809129651, r[1], 14);
            Assert.Equal(-0.4242640700104211225, r[2], 14);
            Assert.Equal(0.5656854276809129651, r[3], 14);
            Assert.Equal(-0.0925483394532274246, r[4], 14);
            Assert.Equal(-0.8194112531408833269, r[5], 14);
            Assert.Equal(0.4242640700104211225, r[6], 14);
            Assert.Equal(-0.8194112531408833269, r[7], 14);
            Assert.Equal(0.3854415612311154341, r[8], 14);
        }
    }

    public class SofaRxTests
    {
        [Fact]
        public void TestRx()
        {
            double phi = 0.3456789;
            double[] r = new double[] { 2.0, 3.0, 2.0 ,3.0, 2.0, 3.0 ,3.0, 4.0, 5.0 };

            Sofa.Rx(phi, r);

            Assert.Equal(2.0, r[0], 0);
            Assert.Equal(3.0, r[1], 0);
            Assert.Equal(2.0, r[2], 0);
            Assert.Equal(3.839043388235612460, r[3], 12);
            Assert.Equal(3.237033249594111899, r[4], 12);
            Assert.Equal(4.516714379005982719, r[5], 12);
            Assert.Equal(1.806030415924501684, r[6], 12);
            Assert.Equal(3.085711545336372503, r[7], 12);
            Assert.Equal(3.687721683977873065, r[8], 12);
        }
    }

    public class SofaRxpTests
    {
        [Fact]
        public void TestRxp()
        {
            double[] r = new double[] {2.0, 3.0, 2.0 ,3.0, 2.0, 3.0 ,3.0, 4.0, 5.0 };
            double[] p = { 0.2, 1.5, 0.1 };
            double[] rp = new double[3];

            Sofa.Rxp(r, p, rp);

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
            double[] r = new double[] {2.0, 3.0, 2.0 ,3.0, 2.0, 3.0 ,3.0, 4.0, 5.0 };
            double[] pv = new double[] {0.2, 1.5, 0.1 ,1.5, 0.2, 0.1 };
            double[] rpv = new double[6];

            Sofa.Rxpv(r, pv, rpv);

            Assert.Equal(5.1, rpv[0], 12);
            Assert.Equal(3.9, rpv[1], 12);
            Assert.Equal(7.1, rpv[2], 12);
            Assert.Equal(3.8, rpv[3], 12);
            Assert.Equal(5.2, rpv[4], 12);
            Assert.Equal(5.8, rpv[5], 12);
        }
    }

    public class SofaRxrTests
    {
        [Fact]
        public void TestRxr()
        {
            double[] a = new double[] {2.0, 3.0, 2.0 ,3.0, 2.0, 3.0 ,3.0, 4.0, 5.0 };
            double[] b = new double[] {1.0, 2.0, 2.0 ,4.0, 1.0, 1.0 ,3.0, 0.0, 1.0 };
            double[] atb = new double[9];

            Sofa.Rxr(a, b, atb);

            Assert.Equal(20.0, atb[0], 12);
            Assert.Equal(7.0, atb[1], 12);
            Assert.Equal(9.0, atb[2], 12);
            Assert.Equal(20.0, atb[3], 12);
            Assert.Equal(8.0, atb[4], 12);
            Assert.Equal(11.0, atb[5], 12);
            Assert.Equal(34.0, atb[6], 12);
            Assert.Equal(10.0, atb[7], 12);
            Assert.Equal(15.0, atb[8], 12);
        }
    }

    public class SofaRyTests
    {
        [Fact]
        public void TestRy()
        {
            double theta = 0.3456789;
            double[] r = new double[] {2.0, 3.0, 2.0 ,3.0, 2.0, 3.0 ,3.0, 4.0, 5.0 };

            Sofa.Ry(theta, r);

            Assert.Equal(0.8651847818978159930, r[0], 12);
            Assert.Equal(1.467194920539316554, r[1], 12);
            Assert.Equal(0.1875137911274457342, r[2], 12);
            Assert.Equal(3, r[3], 12);
            Assert.Equal(2, r[4], 12);
            Assert.Equal(3, r[5], 12);
            Assert.Equal(3.500207892850427330, r[6], 12);
            Assert.Equal(4.779889022262298150, r[7], 12);
            Assert.Equal(5.381899160903798712, r[8], 12);
        }
    }

    public class SofaRzTests
    {
        [Fact]
        public void TestRz()
        {
            double psi = 0.3456789;
            double[] r = new double[] {2.0, 3.0, 2.0 ,3.0, 2.0, 3.0 ,3.0, 4.0, 5.0 };

            Sofa.Rz(psi, r);

            Assert.Equal(2.898197754208926769, r[0], 12);
            Assert.Equal(3.500207892850427330, r[1], 12);
            Assert.Equal(2.898197754208926769, r[2], 12);
            Assert.Equal(2.144865911309686813, r[3], 12);
            Assert.Equal(0.865184781897815993, r[4], 12);
            Assert.Equal(2.144865911309686813, r[5], 12);
            Assert.Equal(3.0, r[6], 12);
            Assert.Equal(4.0, r[7], 12);
            Assert.Equal(5.0, r[8], 12);
        }
    }

    public class SofaS2cTests
    {
        [Fact]
        public void TestS2c()
        {
            double[] c = new double[3];

            Sofa.S2c(3.0123, -0.999, c);

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

            Sofa.S2p(-3.21, 0.123, 0.456, p);

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
            double[] pv = new double[6];

            Sofa.S2pv(-3.21, 0.123, 0.456, -7.8e-6, 9.01e-6, -1.23e-5, pv);

            Assert.Equal(-0.4514964673880165228, pv[0], 12);
            Assert.Equal(0.0309339427734258688, pv[1], 12);
            Assert.Equal(0.0559466810510877933, pv[2], 12);
            Assert.Equal(0.1292270850663260170e-4, pv[3], 15);
            Assert.Equal(0.2652814182060691422e-5, pv[4], 15);
            Assert.Equal(0.2568431853930292259e-5, pv[5], 15);
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

            Sofa.Sxp(s, p, sp);

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
            double[] pv = new double[] {0.3, 1.2, -2.5 ,0.5, 3.2, -0.7};
            double[] spv = new double[6];

            Sofa.Sxpv(s, pv, spv);

            Assert.Equal(0.6, spv[0], 0);
            Assert.Equal(2.4, spv[1], 0);
            Assert.Equal(-5.0, spv[2], 0);
            Assert.Equal(1.0, spv[3], 0);
            Assert.Equal(6.4, spv[4], 0);
            Assert.Equal(-1.4, spv[5], 0);
        }
    }

    public class SofaTrTests
    {
        [Fact]
        public void TestTr()
        {
            double[] r = new double[] {2.0, 3.0, 2.0 ,3.0, 2.0, 3.0 ,3.0, 4.0, 5.0 };
            double[] rt = new double[9];

            Sofa.Tr(r, rt);

            Assert.Equal(2.0, rt[0], 0);
            Assert.Equal(3.0, rt[1], 0);
            Assert.Equal(3.0, rt[2], 0);
            Assert.Equal(3.0, rt[3], 0);
            Assert.Equal(2.0, rt[4], 0);
            Assert.Equal(4.0, rt[5], 0);
            Assert.Equal(2.0, rt[6], 0);
            Assert.Equal(3.0, rt[7], 0);
            Assert.Equal(5.0, rt[8], 0);
        }
    }

    public class SofaTrxpTests
    {
        [Fact]
        public void TestTrxp()
        {
            double[] r = new double[] {2.0, 3.0, 2.0 ,3.0, 2.0, 3.0 ,3.0, 4.0, 5.0 };
            double[] p = { 0.2, 1.5, 0.1 };
            double[] trp = new double[3];

            Sofa.Trxp(r, p, trp);

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
            double[] r = new double[] {2.0, 3.0, 2.0 ,3.0, 2.0, 3.0 ,3.0, 4.0, 5.0 };
            double[] pv = new double[] { 0.2, 1.5, 0.1, 1.5, 0.2, 0.1 };
            double[] trpv = new double[6];

            Sofa.Trxpv(r, pv, trpv);

            Assert.Equal(5.2, trpv[0], 12);
            Assert.Equal(4.0, trpv[1], 12);
            Assert.Equal(5.4, trpv[2], 12);
            Assert.Equal(3.9, trpv[3], 12);
            Assert.Equal(5.3, trpv[4], 12);
            Assert.Equal(4.1, trpv[5], 12);
        }
    }

    public class SofaZpTests
    {
        [Fact]
        public void TestZp()
        {
            double[] p = { 0.3, 1.2, -2.5 };

            Sofa.Zp(p);

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
            double[] pv = new double[] {0.3, 1.2, -2.5 ,-0.5, 3.1, 0.9 };

            Sofa.Zpv(pv);

            Assert.Equal(0.0, pv[0], 0);
            Assert.Equal(0.0, pv[1], 0);
            Assert.Equal(0.0, pv[2], 0);
            Assert.Equal(0.0, pv[3], 0);
            Assert.Equal(0.0, pv[4], 0);
            Assert.Equal(0.0, pv[5], 0);
        }
    }

    public class SofaZrTests
    {
        [Fact]
        public void TestZr()
        {
            double[] r = new double[] {2.0, 3.0, 3.0 ,3.0, 2.0, 4.0 ,2.0, 3.0, 5.0 };

            Sofa.Zr(r);

            for (int i = 0; i < 9; i++)
            {
                    Assert.Equal(0.0, r[i], 0);
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

            double adb = Sofa.Pdp(a, b);

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

            double theta = Sofa.Pap(a, b);

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

            double theta = Sofa.Pas(al, ap, bl, bp);

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

            double s = Sofa.Sepp(a, b);

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

            double s = Sofa.Seps(al, ap, bl, bp);

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

            Sofa.Refco(phpa, tc, rh, wl, ref refa, ref refb);

            Assert.Equal(0.2264949956241415009e-3, refa, 15);
            Assert.Equal(-0.2598658261729343970e-6, refb, 15);
        }
    }
}