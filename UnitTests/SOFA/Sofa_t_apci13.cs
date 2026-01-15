//test\ASCOMStandard.Tests\SOFA\Sofa_t_apci13.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_apci13
    {
        [Fact]
        public void Apci13()
        {
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            var astrom = new Sofa.Astrom();
            double eo = 0;

            Sofa.Apci13(date1, date2, ref astrom, ref eo);

            Assert.Equal(12.65133794027378508, astrom.pmt, 11);
            Assert.Equal(0.9013108747340644755, astrom.eb[0], 12);
            Assert.Equal(-0.4174026640406119957, astrom.eb[1], 12);
            Assert.Equal(-0.1809822877867817771, astrom.eb[2], 12);
            Assert.Equal(0.8940025429255499549, astrom.eh[0], 12);
            Assert.Equal(-0.4110930268331896318, astrom.eh[1], 12);
            Assert.Equal(-0.1782189006019749850, astrom.eh[2], 12);
            Assert.Equal(1.010465295964664178, astrom.em, 12);
            Assert.Equal(0.4289638912941341125e-4, astrom.v[0], 15);
            Assert.Equal(0.8115034032405042132e-4, astrom.v[1], 15);
            Assert.Equal(0.3517555135536470279e-4, astrom.v[2], 15);
            Assert.Equal(0.9999999951686013142, astrom.bm1, 12);
            Assert.Equal(0.9999992060376761710, astrom.bpn[0], 12);
            Assert.Equal(0.4124244860106037157e-7, astrom.bpn[3], 12);
            Assert.Equal(0.1260128571051709670e-2, astrom.bpn[6], 12);
            Assert.Equal(-0.1282291987222130690e-7, astrom.bpn[1], 12);
            Assert.Equal(0.9999999997456835325, astrom.bpn[4], 12);
            Assert.Equal(-0.2255288829420524935e-4, astrom.bpn[7], 12);
            Assert.Equal(-0.1260128571661374559e-2, astrom.bpn[2], 12);
            Assert.Equal(0.2255285422953395494e-4, astrom.bpn[5], 12);
            Assert.Equal(0.9999992057833604343, astrom.bpn[8], 12);
            Assert.Equal(-0.2900618712657375647e-2, eo, 12);
        }
    }
}