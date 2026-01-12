//test\ASCOMStandard.Tests\SOFA\Sofa_t_aticq.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_aticq
    {
        [Fact]
        public void Aticq()
        {
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            var astrom = new Sofa.Astrom();
            double eo = 0;
            Sofa.Apci13(date1, date2, ref astrom, ref eo);

            double ri = 2.710121572969038991;
            double di = 0.1729371367218230438;
            double rc = 0, dc = 0;

            Sofa.Aticq(ri, di, ref astrom, ref rc, ref dc);

            Assert.Equal(2.710126504531716819, rc, 12);
            Assert.Equal(0.1740632537627034482, dc, 12);
        }
    }
}