//test\ASCOMStandard.Tests\SOFA\Sofa_t_atciqn.cs
using ASCOM.Tools;
using System;
using System.Net.NetworkInformation;
using Xunit;
using static ASCOM.Tools.Sofa;

namespace SOFATests
{
    public class Sofa_t_atciqn
    {
        [Fact]
        public void Atciqn()
        {
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            var astrom = new Sofa.Astrom();
            double eo = 0;
            Sofa.Apci13(date1, date2, ref astrom, ref eo);

            double ri = 2.709994899247599271;
            double di = 0.1728740720983623469;

            // prepare b array of LdBody
            var b = new Sofa.LdBody[3];
            for (int i = 0; i < 3; i++) b[i] = new Sofa.LdBody();

            double rc = 2.71;
            double dc = 0.174;
            double pr = 1e-5;
            double pd = 5e-6;
            double px = 0.1;
            double rv = 55.0;
            b[0].bm = 0.00028574;
            b[0].dl = 3e-10;
            b[0].pv[0] = -7.81014427;
            b[0].pv[1] = -5.60956681;
            b[0].pv[2] = -1.98079819;
            b[0].pv[3] = 0.0030723249;
            b[0].pv[4] = -0.00406995477;
            b[0].pv[5] = -0.00181335842;
            b[1].bm = 0.00095435;
            b[1].dl = 3e-9;
            b[1].pv[0] = 0.738098796;
            b[1].pv[1] = 4.63658692;
            b[1].pv[2] = 1.9693136;
            b[1].pv[3] = -0.00755816922;
            b[1].pv[4] = 0.00126913722;
            b[1].pv[5] = 0.000727999001;
            b[2].bm = 1.0;
            b[2].dl = 6e-6;
            b[2].pv[0] = -0.000712174377;
            b[2].pv[1] = -0.00230478303;
            b[2].pv[2] = -0.00105865966;
            b[2].pv[3] = 6.29235213e-6;
            b[2].pv[4] = -3.30888387e-7;
            b[2].pv[5] = -2.96486623e-7;

            Sofa.Atciqn(rc, dc, pr, pd, px, rv, ref astrom, 3, b, ref ri, ref di);

            Assert.Equal(2.710122008104983335, ri, 12);
            Assert.Equal(0.1729371916492767821, di, 12);
        }
    }
}
