//test\ASCOMStandard.Tests\SOFA\Sofa_t_c2i06a.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_c2i06a
    {
        [Fact]
        public void C2i06a()
        {
            double[] rc2i = new double[9];
             Sofa.C2i06a(2400000.5, 53736.0, rc2i);

            Assert.Equal(0.9999998323037159379, rc2i[0], 12);
            Assert.Equal(0.5581121329587613787e-9, rc2i[1], 12);
            Assert.Equal(-0.5791308487740529749e-3, rc2i[2], 12);

            Assert.Equal(-0.2384253169452306581e-7, rc2i[3], 12);
            Assert.Equal(0.9999999991917467827, rc2i[4], 12);
            Assert.Equal(-0.4020579392895682558e-4, rc2i[5], 12);

            Assert.Equal(0.5791308482835292617e-3, rc2i[6], 12);
            Assert.Equal(0.4020580099454020310e-4, rc2i[7], 12);
            Assert.Equal(0.9999998314954628695, rc2i[8], 12);
        }
    }
}