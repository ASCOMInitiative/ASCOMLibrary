//test\ASCOMStandard.Tests\SOFA\Sofa_t_c2i00a.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_c2i00a
    {
        [Fact]
        public void C2i00a()
        {
            double[] rc2i = new double[9];

            Sofa.C2i00a(2400000.5, 53736.0, rc2i);

            Assert.Equal(0.9999998323037165557, rc2i[0], 12);
            Assert.Equal(0.5581526348992140183e-9, rc2i[1], 12);
            Assert.Equal(-0.5791308477073443415e-3, rc2i[2], 12);

            Assert.Equal(-0.2384266227870752452e-7, rc2i[3], 12);
            Assert.Equal(0.9999999991917405258, rc2i[4], 12);
            Assert.Equal(-0.4020594955028209745e-4, rc2i[5], 12);

            Assert.Equal(0.5791308472168152904e-3, rc2i[6], 12);
            Assert.Equal(0.4020595661591500259e-4, rc2i[7], 12);
            Assert.Equal(0.9999998314954572304, rc2i[8], 12);
        }
    }
}