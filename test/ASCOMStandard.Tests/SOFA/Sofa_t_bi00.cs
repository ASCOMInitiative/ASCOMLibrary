//test\ASCOMStandard.Tests\SOFA\Sofa_t_bi00.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_bi00
    {
        [Fact]
        public void Bi00()
        {
            double dpsibi = 0, depsbi = 0, dra = 0;

            // ToDo: Implement Sofa.Bi00 and remove the following line
            Sofa.Bi00(ref dpsibi, ref depsbi, ref dra);

            Assert.Equal(-0.2025309152835086613e-6, dpsibi, 12);
            Assert.Equal(-0.3306041454222147847e-7, depsbi, 12);
            Assert.Equal(-0.7078279744199225506e-7, dra, 12);
        }
    }
}