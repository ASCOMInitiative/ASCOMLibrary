//test\ASCOMStandard.Tests\SOFA\Sofa_t_bp00.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_bp00
    {
        [Fact]
        public void Bp00()
        {
            double[] rb = new double[9];
            double[] rp = new double[9];
            double[] rbp = new double[9];

            Sofa.Bp00(2400000.5, 50123.9999, rb, rp, rbp);

            Assert.Equal(0.9999999999999942498, rb[0], 12);
            Assert.Equal(-0.7078279744199196626e-7, rb[1], 15);
            Assert.Equal(0.8056217146976134152e-7, rb[2], 15);

            Assert.Equal(0.9999995504864048241, rp[0], 12);
            Assert.Equal(0.8696113836207084411e-3, rp[1], 14);
            Assert.Equal(0.3778928813389333402e-3, rp[2], 14);

            Assert.Equal(0.9999995505175087260, rbp[0], 12);
            Assert.Equal(0.8695405883617884705e-3, rbp[1], 14);
            Assert.Equal(0.3779734722239007105e-3, rbp[2], 14);
        }
    }
}