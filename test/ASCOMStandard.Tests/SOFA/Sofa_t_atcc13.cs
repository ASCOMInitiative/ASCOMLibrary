//test\ASCOMStandard.Tests\SOFA\Sofa_t_atcc13.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_atcc13
    {
        [Fact]
        public void Atcc13()
        {
            double rc = 2.71;
            double dc = 0.174;
            double pr = 1e-5;
            double pd = 5e-6;
            double px = 0.1;
            double rv = 55.0;
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            double ra = 0, da = 0;

            Sofa.Atcc13(rc, dc, pr, pd, px, rv, date1, date2, ref ra, ref da);

            Assert.Equal(2.710126504531372384, ra, 12);
            Assert.Equal(0.1740632537628350152, da, 12);
        }
    }
}