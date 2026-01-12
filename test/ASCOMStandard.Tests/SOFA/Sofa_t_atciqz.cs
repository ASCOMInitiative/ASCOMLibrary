//test\ASCOMStandard.Tests\SOFA\Sofa_t_atciqz.cs
using ASCOM.Tools;
using System;
using System.Net.NetworkInformation;
using Xunit;
using static ASCOM.Tools.Sofa;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SOFATests
{
    public class Sofa_t_atciqz
    {
        [Fact]
        public void Atciqz()
        {
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            var astrom = new Sofa.Astrom();
            double eo = 0;
            Sofa.Apci13(date1, date2, ref astrom, ref eo);

            double rc = 2.71;
            double dc = 0.174;

            double ri = 0.0;
            double di = 0.0;

            Sofa.Atciqz(rc, dc, ref astrom, ref ri, ref di);

            Assert.Equal(2.709994899247256984, ri, 12);
            Assert.Equal(0.1728740720984931891, di, 12);
        }

        //     date1 = 2456165.5;
        //      date2 = 0.401182685;
        //      iauApci13(date1, date2, &astrom, &eo);
        //     rc = 2.71;
        //      dc = 0.174;

        //  iauAtciqz(rc, dc, &astrom, &ri, &di);

        //     vvd(ri, 2.709994899247256984, 1e-12, "iauAtciqz", "ri", status);
        //     vvd(di, 0.1728740720984931891, 1e-12, "iauAtciqz", "di", status);


    }
}