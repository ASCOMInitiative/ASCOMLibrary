//test\ASCOMStandard.Tests\SOFA\Sofa_t_a2tf.cs
using Xunit;
using ASCOM.Tools;
using System;
using System.Text;

namespace SOFATests
{
    public class Sofa_t_a2tf
    {
        [Fact]
        public void A2tf()
        {
            StringBuilder sign = new(10);
            int[] ihmsf = new int[4];

            Sofa.A2tf(4, -3.01234, sign, ihmsf);

            Assert.Equal('-', sign[0]);
            Assert.Equal(11, ihmsf[0]);
            Assert.Equal(30, ihmsf[1]);
            Assert.Equal(22, ihmsf[2]);
            Assert.Equal(6484, ihmsf[3]);
        }
    }
}