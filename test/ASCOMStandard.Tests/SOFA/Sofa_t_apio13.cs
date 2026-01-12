//test\ASCOMStandard.Tests\SOFA\Sofa_t_apio13.cs
using ASCOM.Tools;
using System;
using System.Net.NetworkInformation;
using Xunit;
using static ASCOM.Tools.Sofa;

namespace SOFATests
{
    public class Sofa_t_apio13
    {
        [Fact]
        public void Apio13()
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
            int j;
            var astrom = new Sofa.Astrom();

            j = Sofa.Apio13(utc1, utc2, dut1, elong, phi, hm, xp, yp, phpa, tc, rh, wl, ref astrom);


            Assert.Equal(-0.5278008060295995733, astrom.along, 12);
            Assert.Equal(0.1133427418130752958e-5, astrom.xpl, 15);
            Assert.Equal(0.1453347595780646207e-5, astrom.ypl, 15);
            Assert.Equal(-0.9440115679003211329, astrom.sphi, 12);
            Assert.Equal(0.3299123514971474711, astrom.cphi, 12);
            Assert.Equal(0.5135843661699913529e-6, astrom.diurab, 12);
            Assert.Equal(2.617608909189664000, astrom.eral, 12);
            Assert.Equal(0.2014187785940396921e-3, astrom.refa, 15);
            Assert.Equal(-0.2361408314943696227e-6, astrom.refb, 15);
            Assert.Equal(0, j);
        }

   //     utc1 = 2456384.5;
   //utc2 = 0.969254051;
   //dut1 = 0.1550675;
   //elong = -0.527800806;
   //phi = -1.2345856;
   //hm = 2738.0;
   //xp = 2.47230737e-7;
   //yp = 1.82640464e-6;
   //phpa = 731.0;
   //tc = 12.8;
   //rh = 0.59;
   //wl = 0.55;

   //j = iauApio13(utc1, utc2, dut1, elong, phi, hm, xp, yp,
   //              phpa, tc, rh, wl, &astrom);

   //     vvd(astrom.along, -0.5278008060295995733, 1e-12,
   //                       "iauApio13", "along", status);
   //     vvd(astrom.xpl, 0.1133427418130752958e-5, 1e-17,
   //                     "iauApio13", "xpl", status);
   //     vvd(astrom.ypl, 0.1453347595780646207e-5, 1e-17,
   //                     "iauApio13", "ypl", status);
   //     vvd(astrom.sphi, -0.9440115679003211329, 1e-12,
   //                      "iauApio13", "sphi", status);
   //     vvd(astrom.cphi, 0.3299123514971474711, 1e-12,
   //                      "iauApio13", "cphi", status);
   //     vvd(astrom.diurab, 0.5135843661699913529e-6, 1e-12,
   //                        "iauApio13", "diurab", status);
   //     vvd(astrom.eral, 2.617608909189664000, 1e-12,
   //                      "iauApio13", "eral", status);
   //     vvd(astrom.refa, 0.2014187785940396921e-3, 1e-15,
   //                      "iauApio13", "refa", status);
   //     vvd(astrom.refb, -0.2361408314943696227e-6, 1e-18,
   //                      "iauApio13", "refb", status);
   //     viv(j, 0, "iauApio13", "j", status);

    }
}