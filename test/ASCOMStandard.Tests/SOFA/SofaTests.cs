using Xunit;
using ASCOM.Tools;

namespace ASCOM.Alpaca.Tests.SOFA
{
    public class SofaTests
    {
        double t1, t2, date1, date2;
        int j;
        double rc, dc, pr, pd, px, rv, utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, aob, zob, hob, dob, rob, eo;
        double ri, di, a, u1, u2, a1, a2, ob1, ob2, anp;

        [Fact]
        public void ReleaseNumber()
        {
            Assert.Equal("18", Sofa.SofaReleaseNumber().ToString());
        }

        [Fact]
        public void IssueDate()
        {
            Assert.Equal("2021-05-12", Sofa.SofaIssueDate());
        }

        [Fact]
        public void RevisionDate()
        {
            Assert.Equal("2021-05-12", Sofa.SofaRevisionDate());
        }
        [Fact]
        public void RevisionNumber()
        {
            Assert.Equal("0", Sofa.SofaRevisionNumber().ToString());
        }

        [Fact]
        public void Af2a()
        {
            j = Sofa.Af2a('-', 45, 13, 27.2, ref a);

            Assert.Equal(0, j);
            Assert.Equal(-0.7893115794313644842, a, 12);
        }

        [Fact]
        public void Anp()
        {
            anp = Sofa.Anp(-0.1);
            Assert.Equal(6.183185307179586477, anp, 12);
        }

        [Fact]
        public void Atci13()
        {
            // Atci13 tests
            rc = 2.71;
            dc = 0.174;
            pr = 0.00001;
            pd = 0.000005;
            px = 0.1;
            rv = 55.0;
            date1 = 2456165.5;
            date2 = 0.401182685;

            Sofa.Atci13(rc, dc, pr, pd, px, rv, date1, date2, ref ri, ref di, ref eo);
            Assert.Equal(2.710121572968696744, ri, 12);
            Assert.Equal(0.1729371367219539137, di, 12);
            Assert.Equal(-0.002900618712657375647, eo, 14);
        }

        [Fact]
        public void Atco13()
        {
            // Atco13 tests
            rc = 2.71;
            dc = 0.174;
            pr = 0.00001;
            pd = 0.000005;
            px = 0.1;
            rv = 55.0;
            utc1 = 2456384.5;
            utc2 = 0.969254051;
            dut1 = 0.1550675;
            elong = -0.527800806;
            phi = -1.2345856;
            hm = 2738.0;
            xp = 0.000000247230737;
            yp = 0.00000182640464;
            phpa = 731.0;
            tc = 12.8;
            rh = 0.59;
            wl = 0.55;

            j = Sofa.Atco13(rc, dc, pr, pd, px, rv, utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref aob, ref zob, ref hob, ref dob, ref rob, ref eo);

            Assert.Equal(0, j);
            Assert.Equal(0.9251774485485515207e-1, aob, 12);
            Assert.Equal(1.407661405256499357, zob, 12);
            Assert.Equal(-0.9265154431529724692e-1, hob, 12);
            Assert.Equal(0.1716626560072526200, dob, 12);
            Assert.Equal(2.710260453504961012, rob, 12);
            Assert.Equal(-0.003020548354802412839, eo, 14);
        }

        [Fact]
        public void Dtf2d()
        {
            // Dtf2d tests

            j = Sofa.Dtf2d("UTC", 1994, 6, 30, 23, 59, 60.13599, ref u1, ref u2);
            Assert.Equal(0, j);
            Assert.Equal(2449534.49999, u1 + u2, 6);
        }

        [Fact]
        public void Eo06a()
        {
            // Eo06a tests
            eo = Sofa.Eo06a(2400000.5, 53736.0);
            Assert.Equal(-0.1332882371941833644e-2, eo,15);
        }

        [Fact]
        public void Atic13()
        {
            // Atic13 tests
            ri = 2.710121572969038991;
            di = 0.1729371367218230438;
            date1 = 2456165.5;
            date2 = 0.401182685;

            Sofa.Atic13(ri, di, date1, date2, ref rc, ref dc, ref eo);

            Assert.Equal(2.710126504531716819, rc, 12);
            Assert.Equal(0.1740632537627034482, dc, 12);
            Assert.Equal(-0.002900618712657375647, eo, 14);
        }

        [Fact]
        public void Atio13()
        {
            // Atio13 tests
            ri = 2.710121572969038991;
            di = 0.1729371367218230438;
            utc1 = 2456384.5;
            utc2 = 0.969254051;
            dut1 = 0.1550675;
            elong = -0.527800806;
            phi = -1.2345856;
            hm = 2738.0;
            xp = 2.47230737e-7;
            yp = 1.82640464e-6;
            phpa = 731.0;
            tc = 12.8;
            rh = 0.59;
            wl = 0.55;

            j = Sofa.Atio13(ri, di, utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref aob, ref zob, ref hob, ref dob, ref rob);

            Assert.Equal(0, j);
            Assert.Equal(0.9233952224895122499e-1, aob, 12);
            Assert.Equal(1.407758704513549991, zob, 12);
            Assert.Equal(-0.9247619879881698140e-1, hob, 12);
            Assert.Equal(0.1717653435756234676, dob, 12);
            Assert.Equal(2.710085107988480746, rob, 12);
        }

        [Fact]
        public void Atoc13()
        {
            // Atoc13 tests
            utc1 = 2456384.5;
            utc2 = 0.969254051;
            dut1 = 0.1550675;
            elong = -0.527800806;
            phi = -1.2345856;
            hm = 2738.0;
            xp = 2.47230737e-7;
            yp = 1.82640464e-6;
            phpa = 731.0;
            tc = 12.8;
            rh = 0.59;
            wl = 0.55;

            ob1 = 2.710085107986886201;
            ob2 = 0.1717653435758265198;
            j = Sofa.Atoc13("R", ob1, ob2, utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref rc, ref dc);

            Assert.Equal(0, j);
            Assert.Equal(2.709956744659136129, rc, 12);
            Assert.Equal(0.1741696500898471362, dc, 12);

            ob1 = -0.09247619879782006106;
            ob2 = 0.1717653435758265198;
            j = Sofa.Atoc13("H", ob1, ob2, utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref rc, ref dc);
            Assert.Equal(0, j);
            Assert.Equal(2.709956744659734086, rc, 12);
            Assert.Equal(0.1741696500898471362, dc, 12);

            ob1 = 0.09233952224794989993;
            ob2 = 1.407758704513722461;
            j = Sofa.Atoc13("A", ob1, ob2, utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref rc, ref dc);
            Assert.Equal(0, j);
            Assert.Equal(2.709956744659734086, rc, 12);
            Assert.Equal(0.1741696500898471366, dc, 12);
        }

        [Fact]
        public void Atoi13()
        {
            // Atoi13 tests
            utc1 = 2456384.5;
            utc2 = 0.969254051;
            dut1 = 0.1550675;
            elong = -0.527800806;
            phi = -1.2345856;
            hm = 2738.0;
            xp = 2.47230737e-7;
            yp = 1.82640464e-6;
            phpa = 731.0;
            tc = 12.8;
            rh = 0.59;
            wl = 0.55;

            ob1 = 2.710085107986886201;
            ob2 = 0.1717653435758265198;
            j = Sofa.Atoi13("R", ob1, ob2, utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref ri, ref di);
            Assert.Equal(0, j);
            Assert.Equal(2.710121574447540810, ri, 12);
            Assert.Equal(0.1729371839116608778, di, 12);

            ob1 = -0.09247619879782006106;
            ob2 = 0.1717653435758265198;
            j = Sofa.Atoi13("H", ob1, ob2, utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref ri, ref di);
            Assert.Equal(0, j);
            Assert.Equal(2.710121574448138676, ri, 12);
            Assert.Equal(0.1729371839116608778, di, 12);

            ob1 = 0.09233952224794989993;
            ob2 = 1.407758704513722461;
            j = Sofa.Atoi13("A", ob1, ob2, utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref ri, ref di);
            Assert.Equal(0, j);
            Assert.Equal(2.710121574448138676, ri, 12);
            Assert.Equal(0.1729371839116608781, di, 12);

        }

        [Fact]
        public void Taitt()
        {
            // TaiTT tests
            j = Sofa.Taitt(2453750.5, 0.892482639, ref t1, ref t2);
            Assert.Equal(0, j);
            Assert.Equal(2453750.5, t1, 6);
            Assert.Equal(0.892855139, t2, 12);
        }

        [Fact]
        public void Taiutc()
        {
            // TaiUtc tests
            j = Sofa.Taiutc(2453750.5, 0.892482639, ref u1, ref u2);
            Assert.Equal(0, j);
            Assert.Equal(2453750.5, u1, 6);
            Assert.Equal(0.8921006945555555556, u2, 12);
        }

        [Fact]
        public void Tf2a()
        {
            // Tf2a tests
            j = Sofa.Tf2a('+', 4, 58, 20.2, ref a);

            Assert.Equal(0, j);
            Assert.Equal(1.301739278189537429, a, 12);
        }

        [Fact]
        public void Tttai()
        {
            // TTTai tests
            j = Sofa.Tttai(2453750.5, 0.892482639, ref a1, ref a2);
            Assert.Equal(0, j);
            Assert.Equal(2453750.5, a1, 6);
            Assert.Equal(0.892110139, a2, 12);
        }

        [Fact]
        public void Utctai()
        {
            // UtcTai tests
            j = Sofa.Utctai(2453750.5, 0.892100694, ref u1, ref u2);

            Assert.Equal(0, j);
            Assert.Equal(2453750.5, u1, 6);
            Assert.Equal(0.8924826384444444444, u2, 12);
        }

    }
}
