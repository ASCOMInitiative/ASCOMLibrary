//test\ASCOMStandard.Tests\SOFA\Sofa_t_apcg13.cs
using ASCOM.Tools;
using System;
using System.Net.NetworkInformation;
using Xunit;
using static ASCOM.Tools.Sofa;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SOFATests
{
    public class Sofa_t_apcg13
    {
        [Fact]
        public void Apcg13()
        {
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            var astrom = new Sofa.Astrom();

            Sofa.Apcg13(date1, date2, ref astrom);

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
            
            Assert.Equal(1.0, astrom.bpn[0], 15);          
            Assert.Equal(0.0, astrom.bpn[3], 15);
            Assert.Equal(0.0, astrom.bpn[6], 15);
            Assert.Equal(0.0, astrom.bpn[1], 15);
            Assert.Equal(1.0, astrom.bpn[4], 15);
            Assert.Equal(0.0, astrom.bpn[7], 15);
            Assert.Equal(0.0, astrom.bpn[2], 15);
            Assert.Equal(0.0, astrom.bpn[5], 15);
            Assert.Equal(1.0, astrom.bpn[8], 15);
        }
    }
}