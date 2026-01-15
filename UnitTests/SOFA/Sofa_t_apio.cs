//test\ASCOMStandard.Tests\SOFA\Sofa_t_apio.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_apio
    {
        [Fact]
        public void Apio()
        {
            double sp = -3.01974337e-11;
            double theta = 3.14540971;
            double elong = -0.527800806;
            double phi = -1.2345856;
            double hm = 2738.0;
            double xp = 2.47230737e-7;
            double yp = 1.82640464e-6;
            double refa = 0.000201418779;
            double refb = -2.36140831e-7;
            var astrom = new Sofa.Astrom();

            Sofa.Apio(sp, theta, elong, phi, hm, xp, yp, refa, refb, ref astrom);

            Assert.Equal(-0.5278008060295995734, astrom.along, 12);
            Assert.Equal(0.1133427418130752958e-5, astrom.xpl, 15);
            Assert.Equal(0.1453347595780646207e-5, astrom.ypl, 15);
            Assert.Equal(-0.9440115679003211329, astrom.sphi, 12);
            Assert.Equal(0.3299123514971474711, astrom.cphi, 12);
            Assert.Equal(0.5135843661699913529e-6, astrom.diurab, 12);
            Assert.Equal(2.617608903970400427, astrom.eral, 12);
            Assert.Equal(0.2014187790000000000e-3, astrom.refa, 15);
            Assert.Equal(-0.2361408310000000000e-6, astrom.refb, 15);
        }
    }
}