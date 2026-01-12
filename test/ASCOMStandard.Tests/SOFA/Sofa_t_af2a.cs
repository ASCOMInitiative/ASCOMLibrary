//test\ASCOMStandard.Tests\SOFA\Sofa_t_af2a.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_af2a
    {
        [Fact]
        public void Af2a()
        {
            double a = 0;
            int j = Sofa.Af2a('-', 45, 13, 27.2, ref a);

            Assert.Equal(0, j);
            Assert.Equal(-0.7893115794313644842, a, 12);
        }
    }
}