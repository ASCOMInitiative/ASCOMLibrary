//test\ASCOMStandard.Tests\SOFA\Sofa_t_atoc13.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_atoc13
    {
        [Fact]
        public void Atoc13()
        {
            double utc1 = 2456384.5;
            double utc2 = 0.969254051;
            double dut1 = 0.1550675;
            double elong = -0.527800806;
            double phi = -1.2345856;
            double hm = 2738.0;
            double xp = 2.47230737e-7;
            double yp = 1.82640464e-6;
            double phpa = 731.0;
            double tc = 12.8;
            double rh = 0.59;
            double wl = 0.55;

            double ob1 = 2.710085107986886201;
            double ob2 = 0.1717653435758265198;
            double rc = 0, dc = 0;
            int j = Sofa.Atoc13("R", ob1, ob2, utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref rc, ref dc);

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
    }
}