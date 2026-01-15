//test\ASCOMStandard.Tests\SOFA\Sofa_t_atic13.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_atic13
    {
        [Fact]
        public void Atic13()
        {
            double ri = 2.710121572969038991;
            double di = 0.1729371367218230438;
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            double rc = 0, dc = 0, eo = 0;

            Sofa.Atic13(ri, di, date1, date2, ref rc, ref dc, ref eo);

            Assert.Equal(2.710126504531716819, rc, 12);
            Assert.Equal(0.1740632537627034482, dc, 12);
            Assert.Equal(-0.002900618712657375647, eo, 14);
        }
    }
}