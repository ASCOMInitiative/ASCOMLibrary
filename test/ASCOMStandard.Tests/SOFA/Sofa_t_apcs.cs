//test\ASCOMStandard.Tests\SOFA\Sofa_t_apcs.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_apcs
    {
        [Fact]
        public void Apcs()
        {
            double date1 = 2456384.5;
            double date2 = 0.970031644;
            double[] pv = new double[6];
            double[] ebpv = new double[6];
            double[] ehp = new double[3];
            var astrom = new Sofa.Astrom();

            pv[0] = -1836024.09;
            pv[1] = 1056607.72;
            pv[2] = -5998795.26;
            pv[3] = -77.0361767;
            pv[4] = -133.310856;
            pv[5] = 0.0971855934;

            ebpv[0] = -0.974170438;
            ebpv[1] = -0.211520082;
            ebpv[2] = -0.0917583024;
            ebpv[3] = 0.00364365824;
            ebpv[4] = -0.0154287319;
            ebpv[5] = -0.00668922024;

            ehp[0] = -0.973458265;
            ehp[1] = -0.209215307;
            ehp[2] = -0.0906996477;

            Sofa.Apcs(date1, date2, pv, ebpv, ehp, ref astrom);

            Assert.Equal(13.25248468622587269, astrom.pmt, 11);
            Assert.Equal(-0.9741827110629881886, astrom.eb[0], 12);
            Assert.Equal(-0.2115130190136415986, astrom.eb[1], 12);
            Assert.Equal(-0.09179840186954412099, astrom.eb[2], 12);
            Assert.Equal(-0.9736425571689454706, astrom.eh[0], 12);
            Assert.Equal(-0.2092452125850435930, astrom.eh[1], 12);
            Assert.Equal(-0.09075578152248299218, astrom.eh[2], 12);
            Assert.Equal(0.9998233241709796859, astrom.em, 12);
            Assert.Equal(0.2078704993282685510e-4, astrom.v[0], 15);
            Assert.Equal(-0.8955360106989405683e-4, astrom.v[1], 15);
            Assert.Equal(-0.3863338994289409097e-4, astrom.v[2], 15);
            Assert.Equal(0.9999999950277561237, astrom.bm1, 12);
            Assert.Equal(1, astrom.bpn[0], 0);
            Assert.Equal(0, astrom.bpn[3], 0);
            Assert.Equal(0, astrom.bpn[6], 0);
            Assert.Equal(0, astrom.bpn[1], 0);
            Assert.Equal(1, astrom.bpn[4], 0);
            Assert.Equal(0, astrom.bpn[7], 0);
            Assert.Equal(0, astrom.bpn[2], 0);
            Assert.Equal(0, astrom.bpn[5], 0);
            Assert.Equal(1, astrom.bpn[8], 0);
        }
    }
}