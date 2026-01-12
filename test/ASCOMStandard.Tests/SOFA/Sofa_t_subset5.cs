using ASCOM.Tools;
using System;
using Xunit;

namespace SOFATests
{
    /// <summary>
    /// SOFA Tcbtdb function tests
    /// </summary>
    public class SofaTcbtdbTests
    {
        [Fact]
        public void Tcbtdb_WithTestValues_ReturnsCorrectB1()
        {
            double b1 = 0, b2 = 0;
            int j = Sofa.Tcbtdb(2453750.5, 0.893019599, ref b1, ref b2);

            Assert.Equal(0, j);
            Assert.Equal(2453750.5, b1, 6);
            Assert.Equal(0.8928551362746343397, b2, 12);
        }
    }

    /// <summary>
    /// SOFA Tcgtt function tests
    /// </summary>
    public class SofaTcgttTests
    {
        [Fact]
        public void Tcgtt_WithTestValues_ReturnsCorrectTt1()
        {
            double tt1 = 0, tt2 = 0;
            int j = Sofa.Tcgtt(2453750.5, 0.892862531, ref tt1, ref tt2);

            Assert.Equal(0, j);
            Assert.Equal(2453750.5, tt1, 6);
            Assert.Equal(0.8928551387488816828, tt2, 12);
        }
    }

    /// <summary>
    /// SOFA Tdbtcb function tests
    /// </summary>
    public class SofaTdbtcbTests
    {
        [Fact]
        public void Tdbtcb_WithTestValues_ReturnsCorrectTcb1()
        {
            double tcb1 = 0, tcb2 = 0;
            int j = Sofa.Tdbtcb(2453750.5, 0.892855137, ref tcb1, ref tcb2);

            Assert.Equal(0, j);
            Assert.Equal(2453750.5, tcb1, 6);
            Assert.Equal(0.8930195997253656716, tcb2, 12);
        }
    }

    /// <summary>
    /// SOFA Tdbtt function tests
    /// </summary>
    public class SofaTdbttTests
    {
        [Fact]
        public void Tdbtt_WithTestValues_ReturnsCorrectTt1()
        {
            double tt1 = 0, tt2 = 0;
            int j = Sofa.Tdbtt(2453750.5, 0.892855137, 0.0, ref tt1, ref tt2);

            Assert.Equal(0, j);
            Assert.Equal(2453750.5, tt1, 6);
            Assert.Equal(0.8928551393263888889, tt2, 8);
        }
    }

    /// <summary>
    /// SOFA Tf2a function tests
    /// </summary>
    public class SofaTf2aTests
    {
        [Fact]
        public void Tf2a_WithPositiveSign_ReturnsCorrectAngle()
        {
            double rad = 0;
            int j = Sofa.Tf2a('+', 4, 58, 20.2, ref rad);

            Assert.Equal(0, j);
            Assert.Equal(1.301739278189537429, rad, 12);
        }

        [Fact]
        public void Tf2a_WithNegativeSign_ReturnsCorrectAngle()
        {
            double rad = 0;
            int j = Sofa.Tf2a('-', 4, 58, 20.2, ref rad);

            Assert.Equal(0, j);
            Assert.Equal(-1.301739278189537429, rad, 12);
        }

        [Fact]
        public void Tf2a_WithInvalidHour_ReturnsErrorStatus()
        {
            double rad = 0;
            int j = Sofa.Tf2a('+', 25, 0, 0, ref rad);

            Assert.Equal(1, j);
        }
    }

    /// <summary>
    /// SOFA Tf2d function tests
    /// </summary>
    public class SofaTf2dTests
    {
        [Fact]
        public void Tf2d_WithTestValues_ReturnsCorrectDays()
        {
            double days = 0;
            int j = Sofa.Tf2d(' ', 23, 55, 10.9, ref days);

            Assert.Equal(0, j);
            Assert.Equal(0.9966539351851851852, days, 12);
        }
    }

    /// <summary>
    /// SOFA Tpors function tests
    /// </summary>
    public class SofaTporsTests
    {
        [Fact]
        public void Tpors_WithTestValues_ReturnsCorrectA01()
        {
            double xi = -0.03;
            double eta = 0.07;
            double ra = 1.3;
            double dec = 1.5;

            double az1 = 0, bz1 = 0, az2 = 0, bz2 = 0;
            int j = Sofa.Tpors(xi, eta, ra, dec, ref az1, ref bz1, ref az2, ref bz2);

            Assert.Equal(1.736621577783208748, az1, 13);
            Assert.Equal(1.436736561844090323, bz1, 13);
            Assert.Equal(4.004971075806584490, az2, 13);
            Assert.Equal(1.565084088476417917, bz2, 13);
            Assert.Equal(2, j);
        }
    }

    /// <summary>
    /// SOFA Tporv function tests
    /// </summary>
    public class SofaTporvTests
    {
        [Fact]
        public void Tporv_WithTestValues_ReturnsCorrectV01()
        {
            double[] v = new double[] { 0.0, 0.0, 0.0 };
            double[] vz1 = new double[3];
            double[] vz2 = new double[3];

            double xi = -0.03;
            double eta = 0.07;
            double ra = 1.3;
            double dec = 1.5;

            Sofa.S2c(ra, dec, v);

            int j = Sofa.Tporv(xi, eta, v, vz1, vz2);

            Assert.Equal(2, j);
            Assert.Equal(-0.02206252822366888610, vz1[0], 15);
            Assert.Equal(0.1318251060359645016, vz1[1], 14);
            Assert.Equal(0.9910274397144543895, vz1[2], 14);
            Assert.Equal(-0.003712211763801968173, vz2[0], 15);
            Assert.Equal(-0.004341519956299836813, vz2[1], 15);
            Assert.Equal(0.9999836852110587012, vz2[2], 14);
        }
    }

    /// <summary>
    /// SOFA Tpsts function tests
    /// </summary>
    public class SofaTpstsTests
    {
        [Fact]
        public void Tpsts_WithTestValues_ReturnsCorrectA()
        {
            double a = 0, b = 0;
            Sofa.Tpsts(-0.03, 0.07, 2.3, 1.5, ref a, ref b);

            Assert.Equal(0.7596127167359629775, a, 14);
            Assert.Equal(1.540864645109263028, b, 13);
        }
    }

    /// <summary>
    /// SOFA Tpstv function tests
    /// </summary>
    public class SofaTpstvTests
    {
        [Fact]
        public void Tpstv_WithTestValues_ReturnsCorrectV()
        {
            double[] vz = new double[3];
            double[] v = new double[3];
            Sofa.S2c(2.3, 1.5, vz);

            Sofa.Tpstv(-0.03, 0.07, vz, v);

            Assert.Equal(0.02170030454907376677, v[0], 15);
            Assert.Equal(0.02060909590535367447, v[1], 15);
            Assert.Equal(0.9995520806583523804, v[2], 14);
        }
    }

    /// <summary>
    /// SOFA Tpxes function tests
    /// </summary>
    public class SofaTpxesTests
    {
        [Fact]
        public void Tpxes_WithTestValues_ReturnsCorrectXi()
        {
            double xi = 0, eta = 0;
            int j = Sofa.Tpxes(1.3, 1.55, 2.3, 1.5, ref xi, ref eta);

            Assert.Equal(0, j);
            Assert.Equal(-0.01753200983236980595, xi, 15);
            Assert.Equal(0.05962940005778712891, eta, 15);
        }
    }

    /// <summary>
    /// SOFA Tpxev function tests
    /// </summary>
    public class SofaTpxevTests
    {
        [Fact]
        public void Tpxev_WithTestValues_ReturnsCorrectXi()
        {
            double[] v = new double[3];
            double[] vz = new double[3];
            double xi = 0, eta = 0;
            Sofa.S2c(1.3, 1.55, v);
            Sofa.S2c(2.3, 1.5, vz);

            int j = Sofa.Tpxev(v, vz, ref xi, ref eta);

            Assert.Equal(0, j);
            Assert.Equal(-0.01753200983236980595, xi, 15);
            Assert.Equal(0.05962940005778712891, eta, 15);
        }
    }

    /// <summary>
    /// SOFA Tttai function tests
    /// </summary>
    public class SofaTttaiTests
    {
        [Fact]
        public void Tttai_WithTestValues_ReturnsCorrectTai1()
        {
            double tai1 = 0, tai2 = 0;
            int j = Sofa.Tttai(2453750.5, 0.892482639, ref tai1, ref tai2);

            Assert.Equal(0, j);
            Assert.Equal(2453750.5, tai1, 6);
            Assert.Equal(0.892110139, tai2, 9);
        }
    }

    /// <summary>
    /// SOFA Tttcg function tests
    /// </summary>
    public class SofaTttcgTests
    {
        [Fact]
        public void Tttcg_WithTestValues_ReturnsCorrectTcg1()
        {
            double tcg1 = 0, tcg2 = 0;
            int j = Sofa.Tttcg(2453750.5, 0.892482639, ref tcg1, ref tcg2);

            Assert.Equal(0, j);
            Assert.Equal(2453750.5, tcg1, 6);
            Assert.NotEqual(0.892482639, tcg2);
        }
    }

    /// <summary>
    /// SOFA Tttdb function tests
    /// </summary>
    public class SofaTttdbTests
    {
        [Fact]
        public void Tttdb_WithTestValues_ReturnsCorrectTdb1()
        {
            double tdb1 = 0, tdb2 = 0;
            int j = Sofa.Tttdb(2453750.5, 0.892855139, -0.000201, ref tdb1, ref tdb2);

            Assert.Equal(0, j);
            Assert.Equal(2453750.5, tdb1, 6);
            Assert.Equal(0.8928551366736111111, tdb2, 12);
        }
    }

    /// <summary>
    /// SOFA Ttut1 function tests
    /// </summary>
    public class SofaTtut1Tests
    {
        [Fact]
        public void Ttut1_WithTestValues_ReturnsCorrectUt11()
        {
            double ut11 = 0, ut12 = 0;
            int j = Sofa.Ttut1(2453750.5, 0.892855139, 64.8499, ref ut11, ref ut12);

            Assert.Equal(0, j);
            Assert.Equal(2453750.5, ut11, 6);
            Assert.Equal(0.8921045614537037037, ut12, 12);
        }
    }

    /// <summary>
    /// SOFA Ut1tai function tests
    /// </summary>
    public class SofaUt1taiTests
    {
        [Fact]
        public void Ut1tai_WithTestValues_ReturnsCorrectTai1()
        {
            double tai1 = 0, tai2 = 0;
            int j = Sofa.Ut1tai(2453750.5, 0.892104561, -32.6659, ref tai1, ref tai2);

            Assert.Equal(0, j);
            Assert.Equal(2453750.5, tai1, 6);
            Assert.Equal(0.8924826385462962963, tai2, 12);
        }
    }

    /// <summary>
    /// SOFA Ut1tt function tests
    /// </summary>
    public class SofaUt1ttTests
    {
        [Fact]
        public void Ut1tt_WithTestValues_ReturnsCorrectTt1()
        {
            double tt1 = 0, tt2 = 0;
            int j = Sofa.Ut1tt(2453750.5, 0.892104561, 64.8499, ref tt1, ref tt2);

            Assert.Equal(0, j);
            Assert.Equal(2453750.5, tt1, 6);
            Assert.Equal(0.8928551385462962963, tt2, 12);
        }
    }

    /// <summary>
    /// SOFA Ut1utc function tests
    /// </summary>
    public class SofaUt1utcTests
    {
        [Fact]
        public void Ut1utc_WithTestValues_ReturnsCorrectUtc1()
        {
            double utc1 = 0, utc2 = 0;
            int j = Sofa.Ut1utc(2453750.5, 0.892104561, 0.3341, ref utc1, ref utc2);

            Assert.Equal(0, j);
            Assert.Equal(2453750.5, utc1, 6);
            Assert.Equal(0.8921006941018518519, utc2, 12);
        }
    }

    /// <summary>
    /// SOFA Utctai function tests
    /// </summary>
    public class SofaUtctaiTests
    {
        [Fact]
        public void Utctai_WithTestValues_ReturnsCorrectTai1()
        {
            double tai1 = 0, tai2 = 0;
            int j = Sofa.Utctai(2453750.5, 0.892100694, ref tai1, ref tai2);

            Assert.Equal(0, j);
            Assert.Equal(2453750.5, tai1, 6);
            Assert.Equal(0.8924826384444444444, tai2, 12);
        }
    }

    /// <summary>
    /// SOFA Utcut1 function tests
    /// </summary>
    public class SofaUtcut1Tests
    {
        [Fact]
        public void Utcut1_WithTestValues_ReturnsCorrectUt11()
        {
            double ut11 = 0, ut12 = 0;
            int j = Sofa.Utcut1(2453750.5, 0.892100694, 0.3341, ref ut11, ref ut12);

            Assert.Equal(0, j);
            Assert.Equal(2453750.5, ut11, 6);
            Assert.Equal(0.8921045608981481481, ut12, 12);
        }
    }

    /// <summary>
    /// SOFA Xy06 function tests
    /// </summary>
    public class SofaXy06Tests
    {
        [Fact]
        public void Xy06_WithTestValues_ReturnsCorrectX()
        {
            double x = 0, y = 0;
            Sofa.Xy06(2400000.5, 53736.0, ref x, ref y);

            Assert.Equal(0.5791308486706010975e-3, x, 15);
            Assert.Equal(0.4020579816732958141e-4, y, 15);
        }
    }

    /// <summary>
    /// SOFA Xys00a function tests
    /// </summary>
    public class SofaXys00aTests
    {
        [Fact]
        public void Xys00a_WithTestValues_ReturnsCorrectX()
        {
            double x = 0, y = 0, s = 0;
            Sofa.Xys00a(2400000.5, 53736.0, ref x, ref y, ref s);

            Assert.Equal(0.5791308472168152904e-3, x, 14);
            Assert.Equal(0.4020595661591500259e-4, y, 15);
            Assert.Equal(-0.1220040848471549623e-7, s, 15);
        }
    }

    /// <summary>
    /// SOFA Xys00b function tests
    /// </summary>
    public class SofaXys00bTests
    {
        [Fact]
        public void Xys00b_WithTestValues_ReturnsCorrectX()
        {
            double x = 0, y = 0, s = 0;
            Sofa.Xys00b(2400000.5, 53736.0, ref x, ref y, ref s);

            Assert.Equal(0.5791301929950208873e-3, x, 14);
            Assert.Equal(0.4020553681373720832e-4, y, 15);
            Assert.Equal(-0.1220027377285083189e-7, s, 15);
        }
    }

    /// <summary>
    /// SOFA Xys06a function tests
    /// </summary>
    public class SofaXys06aTests
    {
        [Fact]
        public void Xys06a_WithTestValues_ReturnsCorrectX()
        {
            double x = 0, y = 0, s = 0;
            Sofa.Xys06a(2400000.5, 53736.0, ref x, ref y, ref s);

            Assert.Equal(0.5791308482835292617e-3, x, 14);
            Assert.Equal(0.4020580099454020310e-4, y, 15);
            Assert.Equal(-0.1220032294164579896e-7, s, 15);
        }
    }
}