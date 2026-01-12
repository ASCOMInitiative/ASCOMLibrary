//test\ASCOMStandard.Tests\SOFA\Sofa_t_anpm.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_anpm
    {
        [Fact]
        public void Anpm()
        {
            double r = Sofa.Anpm(-4.0);
            Assert.Equal(2.283185307179586477, r, 12);
        }
    }
}