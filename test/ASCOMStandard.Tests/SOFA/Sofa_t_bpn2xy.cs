//test\ASCOMStandard.Tests\SOFA\Sofa_t_bpn2xy.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_bpn2xy
    {
        [Fact]
        public void Bpn2xy()
        {
            double[] rbpn = new double[9];
            rbpn[0] = 9.999962358680738e-1;
            rbpn[1] = -2.516417057665452e-3;
            rbpn[2] = -1.093569785342370e-3;
            rbpn[3] = 2.516462370370876e-3;
            rbpn[4] = 9.999968329010883e-1;
            rbpn[5] = 4.006159587358310e-5;
            rbpn[6] = 1.093465510215479e-3;
            rbpn[7] = -4.281337229063151e-5;
            rbpn[8] = 9.999994012499173e-1;

            double x = 0, y = 0;

            Sofa.Bpn2xy(rbpn, ref x, ref y);

            Assert.Equal(1.093465510215479e-3, x, 12);
            Assert.Equal(-4.281337229063151e-5, y, 12);
        }
    }
}