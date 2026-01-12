//test\ASCOMStandard.Tests\SOFA\Sofa_t_bp06.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_bp06
    {
        [Fact]
        public void Bp06()
        {
            double[] rb = new double[9];
            double[] rp = new double[9];
            double[] rbp = new double[9];

            Sofa.Bp06(2400000.5, 50123.9999, rb, rp, rbp);

            Assert.Equal(0.9999999999999942497, rb[0], 12);
            Assert.Equal(-0.7078368960971557145e-7, rb[1], 14);
            Assert.Equal(0.8056213977613185606e-7, rb[2], 14);

            Assert.Equal(0.9999995504864960278, rp[0], 12);
            Assert.Equal(0.8696112578855404832e-3, rp[1], 14);
            Assert.Equal(0.3778929293341390127e-3, rp[2], 14);

            Assert.Equal(0.9999995505176007047, rbp[0], 12);
            Assert.Equal(0.8695404617348208406e-3, rbp[1], 14);
            Assert.Equal(0.3779735201865589104e-3, rbp[2], 14);
        }
    }
}