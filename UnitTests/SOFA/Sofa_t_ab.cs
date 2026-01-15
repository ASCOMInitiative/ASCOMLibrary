//test\ASCOMStandard.Tests\SOFA\Sofa_t_ab.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_ab
    {
        [Fact]
        public void Ab()
        {
            double[] pnat = new double[3];
            double[] v = new double[3];
            double s, bm1;
            double[] ppr = new double[3];

            pnat[0] = -0.76321968546737951;
            pnat[1] = -0.60869453983060384;
            pnat[2] = -0.21676408580639883;
            v[0] = 2.1044018893653786e-5;
            v[1] = -8.9108923304429319e-5;
            v[2] = -3.8633714797716569e-5;
            s = 0.99980921395708788;
            bm1 = 0.99999999506209258;

            Sofa.Ab(pnat, v, s, bm1, ppr);

            Assert.Equal(-0.7631631094219556269, ppr[0], 12);
            Assert.Equal(-0.6087553082505590832, ppr[1], 12);
            Assert.Equal(-0.2167926269368471279, ppr[2], 12);
        }
    }
}