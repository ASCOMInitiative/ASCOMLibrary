//test\ASCOMStandard.Tests\SOFA\Sofa_t_apcg.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_apcg
    {
        [Fact]
        public void Apcg()
        {
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            double[] ebpv = new double[6]; // flattened 2x3
            double[] ehp = new double[3];
            // ebpv[0][0]..ebpv[1][2]
            ebpv[0] = 0.901310875;
            ebpv[1] = -0.417402664;
            ebpv[2] = -0.180982288;
            ebpv[3] = 0.00742727954;
            ebpv[4] = 0.0140507459;
            ebpv[5] = 0.00609045792;
            ehp[0] = 0.903358544;
            ehp[1] = -0.415395237;
            ehp[2] = -0.180084014;

            // astrom is returned via ref/out
            var astrom = new Sofa.Astrom();
            Sofa.Apcg(date1, date2, ebpv, ehp, ref astrom);

            Assert.Equal(12.65133794027378508, astrom.pmt, 11);
            Assert.Equal(0.901310875, astrom.eb[0], 12);
            Assert.Equal(-0.417402664, astrom.eb[1], 12);
            Assert.Equal(-0.180982288, astrom.eb[2], 12);
            Assert.Equal(0.8940025429324143045, astrom.eh[0], 12);
            Assert.Equal(-0.4110930268679817955, astrom.eh[1], 12);
            Assert.Equal(-0.1782189004872870264, astrom.eh[2], 12);
            Assert.Equal(1.010465295811013146, astrom.em, 12);
            Assert.Equal(0.4289638913597693554e-4, astrom.v[0], 15);
            Assert.Equal(0.8115034051581320575e-4, astrom.v[1], 15);
            Assert.Equal(0.3517555136380563427e-4, astrom.v[2], 15);
            Assert.Equal(0.9999999951686012981, astrom.bm1, 12);
            Assert.Equal(1.0, astrom.bpn[0], 0);
            Assert.Equal(0.0, astrom.bpn[3], 0);
            Assert.Equal(0.0, astrom.bpn[6], 0);
            Assert.Equal(0.0, astrom.bpn[1], 0);
            Assert.Equal(1.0, astrom.bpn[4], 0);
            Assert.Equal(0.0, astrom.bpn[7], 0);
            Assert.Equal(0.0, astrom.bpn[2], 0);
            Assert.Equal(0.0, astrom.bpn[5], 0);
            Assert.Equal(1.0, astrom.bpn[8], 0);
        }
    }
}