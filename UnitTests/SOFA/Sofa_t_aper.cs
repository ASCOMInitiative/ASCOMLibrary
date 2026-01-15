//test\ASCOMStandard.Tests\SOFA\Sofa_t_aper.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_aper
    {
        [Fact]
        public void Aper()
        {
            var astrom = new Sofa.Astrom();
            astrom.along = 1.234;
            double theta = 5.678;

            Sofa.Aper(theta, ref astrom);

            Assert.Equal(6.912000000000000000, astrom.eral, 12);
        }
    }
}