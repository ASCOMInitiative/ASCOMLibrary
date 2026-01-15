//test\ASCOMStandard.Tests\SOFA\Sofa_t_atci13.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_atci13
    {
        [Fact]
        public void Atci13()
        {
            double rc = 2.71;
            double dc = 0.174;
            double pr = 1e-5;
            double pd = 5e-6;
            double px = 0.1;
            double rv = 55.0;
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            double ri = 0, di = 0, eo = 0;

            Sofa.Atci13(rc, dc, pr, pd, px, rv, date1, date2, ref ri, ref di, ref eo);

            Assert.Equal(2.710121572968696744, ri, 12);
            Assert.Equal(0.1729371367219539137, di, 12);
            Assert.Equal(-0.002900618712657375647, eo, 14);
        }
    }
}