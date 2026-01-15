//test\ASCOMStandard.Tests\SOFA\Sofa_t_ae2hd.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_ae2hd
    {
        [Fact]
        public void Ae2hd()
        {
            double a, e, p, h = 0, d = 0;

            a = 5.5;
            e = 1.1;
            p = 0.7;

            Sofa.Ae2hd(a, e, p, ref h, ref d);

            Assert.Equal(0.5933291115507309663, h, 14);
            Assert.Equal(0.9613934761647817620, d, 14);
        }
    }
}