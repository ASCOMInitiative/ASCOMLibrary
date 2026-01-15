//test\ASCOMStandard.Tests\SOFA\Sofa_t_apco13.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_apco13
    {
        [Fact]
        public void Apco13()
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
            double eo = 0;
            int j = Sofa.Apco13(utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref astrom, ref eo);

            Assert.Equal(13.25248468622475727, astrom.pmt, 11);
            Assert.Equal(-0.9741827107320875162, astrom.eb[0], 12);
            Assert.Equal(-0.2115130190489716682, astrom.eb[1], 12);
            Assert.Equal(-0.09179840189496755339, astrom.eb[2], 12);
            Assert.Equal(-0.9736425572586935247, astrom.eh[0], 12);
            Assert.Equal(-0.2092452121603336166, astrom.eh[1], 12);
            Assert.Equal(-0.09075578153885665295, astrom.eh[2], 12);
            Assert.Equal(0.9998233240913898141, astrom.em, 12);
            Assert.Equal(0.2078704994520489246e-4, astrom.v[0], 15);
            Assert.Equal(-0.8955360133238868938e-4, astrom.v[1], 15);
            Assert.Equal(-0.3863338993055887398e-4, astrom.v[2], 15);
            Assert.Equal(0.9999999950277561004, astrom.bm1, 12);
            Assert.Equal(0.9999991390295147999, astrom.bpn[0], 12);
            Assert.Equal(0.4978650075315529277e-7, astrom.bpn[3], 12);
            Assert.Equal(0.001312227200850293372, astrom.bpn[6], 12);
            Assert.Equal(-0.1136336652812486604e-7, astrom.bpn[1], 12);
            Assert.Equal(0.9999999995713154865, astrom.bpn[4], 12);
            Assert.Equal(-0.2928086230975367296e-4, astrom.bpn[7], 12);
            Assert.Equal(-0.001312227201745553566, astrom.bpn[2], 12);
            Assert.Equal(0.2928082218847679162e-4, astrom.bpn[5], 12);
            Assert.Equal(0.9999991386008312212, astrom.bpn[8], 12);
            Assert.Equal(-0.003020548354802412839, eo, 14);
            Assert.Equal(0, j);
        }
    }
}