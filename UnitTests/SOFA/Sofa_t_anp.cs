//test\ASCOMStandard.Tests\SOFA\Sofa_t_anp.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_anp
    {
        [Fact]
        public void Anp()
        {
            double r = Sofa.Anp(-0.1);
            Assert.Equal(6.183185307179586477, r, 12);
        }
    }
}