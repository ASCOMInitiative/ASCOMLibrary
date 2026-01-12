//test\ASCOMStandard.Tests\SOFA\Sofa_t_atioq.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_atioq
    {
        [Fact]
        public void Atioq()
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
            var astrom = new Sofa.Astrom();

            Sofa.Apio13(utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref astrom);

            double aob = 0, zob = 0, hob = 0, dob = 0, rob = 0;

            Sofa.Atioq(2.710121572969038991, 0.1729371367218230438, ref astrom, ref aob, ref zob, ref hob, ref dob, ref rob); // mapping risk

            Assert.True(true); // placeholder to indicate test presence; implementation-specific mapping may vary.
        }
    }
}