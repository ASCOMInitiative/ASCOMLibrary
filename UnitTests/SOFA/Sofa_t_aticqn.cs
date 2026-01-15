//test\ASCOMStandard.Tests\SOFA\Sofa_t_aticqn.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_aticqn
    {
        [Fact]
        public void Aticqn()
        {
            double date1 = 2456165.5;
            double date2 = 0.401182685;
            var astrom = new Sofa.Astrom();
            double eo = 0;
            Sofa.Apci13(date1, date2, ref astrom, ref eo);

            double ri = 2.709994899247599271;
            double di = 0.1728740720983623469;

            var b = new Sofa.LdBody[3];
            for (int i = 0; i < 3; i++) b[i] = new Sofa.LdBody { pv = new double[6] };
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

            double rc = 0, dc = 0;
            Sofa.Aticqn(ri, di, ref astrom, 3, b, ref rc, ref dc);

            Assert.Equal(2.709999575033027333, rc, 12);
            Assert.Equal(0.1739999656316469990, dc, 12);
        }
    }
}