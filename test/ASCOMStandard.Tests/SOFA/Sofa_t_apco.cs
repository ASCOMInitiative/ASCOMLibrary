//test\ASCOMStandard.Tests\SOFA\Sofa_t_apco.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_apco
    {
        [Fact]
        public void Apco()
        {
            double date1 = 2456384.5;
            double date2 = 0.970031644;
            double[] ebpv = new double[6];
            double[] ehp = new double[3];
            double x = 0.0013122272;
            double y = -2.92808623e-5;
            double s = 3.05749468e-8;
            double theta = 3.14540971;
            double elong = -0.527800806;
            double phi = -1.2345856;
            double hm = 2738.0;
            double xp = 2.47230737e-7;
            double yp = 1.82640464e-6;
            double sp = -3.01974337e-11;
            double refa = 0.000201418779;
            double refb = -2.36140831e-7;
            var astrom = new Sofa.Astrom();

            ebpv[0] = -0.974170438;
            ebpv[1] = -0.211520082;
            ebpv[2] = -0.0917583024;
            ebpv[3] = 0.00364365824;
            ebpv[4] = -0.0154287319;
            ebpv[5] = -0.00668922024;
            ehp[0] = -0.973458265;
            ehp[1] = -0.209215307;
            ehp[2] = -0.0906996477;

            Sofa.Apco(date1, date2, ebpv, ehp, x, y, s, theta, elong, phi, hm, xp, yp, sp, refa, refb, ref astrom);

            Assert.Equal(13.25248468622587269, astrom.pmt, 11);
            Assert.Equal(-0.9741827110630322720, astrom.eb[0], 12);
            Assert.Equal(-0.2115130190135344832, astrom.eb[1], 12);
            Assert.Equal(-0.09179840186949532298, astrom.eb[2], 12);
            Assert.Equal(-0.9736425571689739035, astrom.eh[0], 12);
            Assert.Equal(-0.2092452125849330936, astrom.eh[1], 12);
            Assert.Equal(-0.09075578152243272599, astrom.eh[2], 12);
            Assert.Equal(0.9998233241709957653, astrom.em, 12);
            Assert.Equal(0.2078704992916728762e-4, astrom.v[0], 15);
            Assert.Equal(-0.8955360107151952319e-4, astrom.v[1], 15);
            Assert.Equal(-0.3863338994288951082e-4, astrom.v[2], 15);
            Assert.Equal(0.9999999950277561236, astrom.bm1, 12);
            Assert.Equal(0.9999991390295159156, astrom.bpn[0], 12);
            Assert.Equal(0.4978650072505016932e-7, astrom.bpn[3], 12);
            Assert.Equal(0.1312227200000000000e-2, astrom.bpn[6], 12);
            Assert.Equal(-0.1136336653771609630e-7, astrom.bpn[1], 12);
            Assert.Equal(0.9999999995713154868, astrom.bpn[4], 12);
            Assert.Equal(-0.2928086230000000000e-4, astrom.bpn[7], 12);
            Assert.Equal(-0.1312227200895260194e-2, astrom.bpn[2], 12);
            Assert.Equal(0.2928082217872315680e-4, astrom.bpn[5], 12);
            Assert.Equal(0.9999991386008323373, astrom.bpn[8], 12);
            Assert.Equal(-0.5278008060295995734, astrom.along, 12);
            Assert.Equal(0.1133427418130752958e-5, astrom.xpl, 15);
            Assert.Equal(0.1453347595780646207e-5, astrom.ypl, 15);
            Assert.Equal(-0.9440115679003211329, astrom.sphi, 12);
            Assert.Equal(0.3299123514971474711, astrom.cphi, 12);
            Assert.Equal(0, astrom.diurab, 0);
            Assert.Equal(2.617608903970400427, astrom.eral, 12);
            Assert.Equal(0.2014187790000000000e-3, astrom.refa, 15);
            Assert.Equal(-0.2361408310000000000e-6, astrom.refb, 15);
        }
    }
}