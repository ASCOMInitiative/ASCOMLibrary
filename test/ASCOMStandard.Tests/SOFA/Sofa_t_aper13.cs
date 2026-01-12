//test\ASCOMStandard.Tests\SOFA\Sofa_t_aper13.cs
using Xunit;
using ASCOM.Tools;
using System;

namespace SOFATests
{
    public class Sofa_t_aper13
    {
        [Fact]
        public void Aper13()
        {
            var astrom = new Sofa.Astrom();
            astrom.along = 1.234;
            double ut11 = 2456165.5;
            double ut12 = 0.401182685;

            Sofa.Aper13(ut11, ut12, ref astrom);

            Assert.Equal(3.316236661789694933, astrom.eral, 12);
        }
    }
}