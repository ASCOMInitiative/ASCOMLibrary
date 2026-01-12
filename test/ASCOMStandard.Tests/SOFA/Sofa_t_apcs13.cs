//test\ASCOMStandard.Tests\SOFA\Sofa_t_apcs13.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_apcs13
    {
        [Fact]
        public void Apcs13()
        {
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            double[] pv = new double[6];
            var astrom = new Sofa.Astrom();

            pv[0] = -6241497.16;
            pv[1] = 401346.896;
            pv[2] = -1251136.04;
            pv[3] = -29.264597;
            pv[4] = -455.021831;
            pv[5] = 0.0266151194;

            Sofa.Apcs13(date1, date2, pv, ref astrom);

            Assert.Equal(12.65133794027378508, astrom.pmt, 11);
            Assert.Equal(0.9012691529025250644, astrom.eb[0], 12);
            Assert.Equal(-0.4173999812023194317, astrom.eb[1], 12);
            Assert.Equal(-0.1809906511146429670, astrom.eb[2], 12);
            Assert.Equal(0.8939939101760130792, astrom.eh[0], 12);
            Assert.Equal(-0.4111053891734021478, astrom.eh[1], 12);
            Assert.Equal(-0.1782336880636997374, astrom.eh[2], 12);
            Assert.Equal(1.010428384373491095, astrom.em, 12);
            Assert.Equal(0.4279877294121697570e-4, astrom.v[0], 15);
            Assert.Equal(0.7963255087052120678e-4, astrom.v[1], 15);
            Assert.Equal(0.3517564013384691531e-4, astrom.v[2], 15);
            Assert.Equal(0.9999999952947980978, astrom.bm1, 12);
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