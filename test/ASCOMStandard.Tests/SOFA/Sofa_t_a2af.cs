//test\ASCOMStandard.Tests\SOFA\Sofa_t_a2af.cs
using Xunit;
using ASCOM.Tools;
using System;
using System.Text;

namespace SOFATests
{
    public class Sofa_t_a2af
    {
        [Fact]
        public void A2af()
        {
            StringBuilder sign = new(10);
            int[] idmsf = new int[4];

            Sofa.A2af(4, 2.345, sign, idmsf);

            Assert.Equal('+', sign[0]);
            Assert.Equal(134, idmsf[0]);
            Assert.Equal(21, idmsf[1]);
            Assert.Equal(30, idmsf[2]);
            Assert.Equal(9706, idmsf[3]);
        }
    }
}