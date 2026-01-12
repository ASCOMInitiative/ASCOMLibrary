using ASCOM.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace SOFATests
{
    /// <summary>
    /// xUnit tests for SOFA pn06 function
    /// </summary>
    public class SofaPn06Tests
    {
        [Fact]
        public void Pn06_WithTestValues_ReturnsCorrectEpsa()
        {
            // Arrange
            double dpsi = -0.9632552291149335877e-5;
            double deps = 0.4063197106621141414e-4;
            double[] rb = new double[9];
            double[] rp = new double[9];
            double[] rbp = new double[9];
            double[] rn = new double[9];
            double[] rbpn = new double[9];
            double epsa = 0.0;

            // Act
            Sofa.Pn06(2400000.5, 53736.0, dpsi, deps, ref epsa, rb, rp, rbp, rn, rbpn);

            // Assert
            Assert.Equal(0.4090789763356509926, epsa, 12);
        }

        [Fact]
        public void Pn06_WithTestValues_ReturnsCorrectRbMatrix()
        {
            // Arrange
            double dpsi = -0.9632552291149335877e-5;
            double deps = 0.4063197106621141414e-4;
            double[] rb = new double[9];
            double[] rp = new double[9];
            double[] rbp = new double[9];
            double[] rn = new double[9];
            double[] rbpn = new double[9];
            double epsa = 0.0;

            // Act
            Sofa.Pn06(2400000.5, 53736.0, dpsi, deps, ref epsa, rb, rp, rbp, rn, rbpn);

            // Assert - check key matrix elements
            Assert.Equal(0.9999999999999942497, rb[0], 12);
            Assert.Equal(-0.7078368960971557145e-7, rb[1], 14);
            Assert.Equal(0.8056213977613185606e-7, rb[2], 14);

            Assert.Equal(0.7078368694637674333e-7, rb[3], 14);
            Assert.Equal(0.9999999999999969484, rb[4], 12);
            Assert.Equal(0.3305943742989134124e-7, rb[5], 14);

            Assert.Equal(-0.8056214211620056792e-7, rb[6], 14);
            Assert.Equal(-0.3305943172740586950e-7, rb[7], 14);
            Assert.Equal(0.9999999999999962084, rb[8], 12);


            Assert.Equal(0.9999989300536854831, rp[0], 12);
            Assert.Equal(-0.1341646886204443795e-2, rp[1], 14);
            Assert.Equal(-0.5829880933488627759e-3, rp[2], 14);

            Assert.Equal(0.1341646890569782183e-2, rp[3], 14);
            Assert.Equal(0.9999990999913319321, rp[4], 12);
            Assert.Equal(-0.3835944216374477457e-6, rp[5], 14);

            Assert.Equal(0.5829880833027867368e-3, rp[6], 14);
            Assert.Equal(-0.3985701514686976112e-6, rp[7], 14);
            Assert.Equal(0.9999998300623534950, rp[8], 12);

            Assert.Equal(0.9999989300056797893, rbp[0], 12);
            Assert.Equal(-0.1341717650545059598e-2, rbp[1], 14);
            Assert.Equal(-0.5829075756493728856e-3, rbp[2], 14);

            Assert.Equal(0.1341717674223918101e-2, rbp[3], 14);
            Assert.Equal(0.9999990998963748448, rbp[4], 12);
            Assert.Equal(-0.3504269280170069029e-6, rbp[5], 14);

            Assert.Equal(0.5829075211461454599e-3, rbp[6], 14);
            Assert.Equal(-0.4316708436255949093e-6, rbp[7], 14);
            Assert.Equal(0.9999998301093032943, rbp[8], 12);

            Assert.Equal(0.9999999999536069682, rn[0], 12);
            Assert.Equal(0.8837746921149881914e-5, rn[1], 14);
            Assert.Equal(0.3831487047682968703e-5, rn[2], 14);

            Assert.Equal(-0.8837591232983692340e-5, rn[3], 14);
            Assert.Equal(0.9999999991354692664, rn[4], 12);
            Assert.Equal(-0.4063198798558931215e-4, rn[5], 14);

            Assert.Equal(-0.3831846139597250235e-5, rn[6], 14);
            Assert.Equal(0.4063195412258792914e-4, rn[7], 14);
            Assert.Equal(0.9999999991671806293, rn[8], 12);

            Assert.Equal(0.9999989440504506688, rbpn[0], 12);
            Assert.Equal(-0.1332879913170492655e-2, rbpn[1], 14);
            Assert.Equal(-0.5790760923225655753e-3, rbpn[2], 14);

            Assert.Equal(0.1332856406595754748e-2, rbpn[3], 14);
            Assert.Equal(0.9999991109069366795, rbpn[4], 12);
            Assert.Equal(-0.4097725651142641812e-4, rbpn[5], 14);

            Assert.Equal(0.5791301952321296716e-3, rbpn[6], 14);
            Assert.Equal(0.4020538796195230577e-4, rbpn[7], 14);
            Assert.Equal(0.9999998314958576778, rbpn[8], 12);
        }
    }

    /// <summary>
    /// xUnit tests for SOFA pn06a function
    /// </summary>
    public class SofaPn06aTests
    {
        [Fact]
        public void Pn06a_WithTestValues_ReturnsCorrectDpsi()
        {
            // Arrange
            double dpsi = 0.0;
            double deps = 0.0;
            double epsa = 0.0;
            double[] rb = new double[9];
            double[] rp = new double[9];
            double[] rbp = new double[9];
            double[] rn = new double[9];
            double[] rbpn = new double[9];

            // Act
            Sofa.Pn06a(2400000.5, 53736.0, ref dpsi, ref deps, ref epsa, rb, rp, rbp, rn, rbpn);

            // Assert
            Assert.Equal(-0.9630912025820308797e-5, dpsi, 12);
        }

        [Fact]
        public void Pn06a_WithTestValues_ReturnsCorrectDeps()
        {
            // Arrange
            double dpsi = 0.0;
            double deps = 0.0;
            double epsa = 0.0;
            double[] rb = new double[9];
            double[] rp = new double[9];
            double[] rbp = new double[9];
            double[] rn = new double[9];
            double[] rbpn = new double[9];

            // Act
            Sofa.Pn06a(2400000.5, 53736.0, ref dpsi, ref deps, ref epsa, rb, rp, rbp, rn, rbpn);

            // Assert
            Assert.Equal(0.4063238496887249798e-4, deps, 12);
        }

        [Fact]
        public void Pn06a_WithTestValues_ReturnsCorrectEpsa()
        {
            // Arrange
            double dpsi = 0.0;
            double deps = 0.0;
            double epsa = 0.0;
            double[] rb = new double[9];
            double[] rp = new double[9];
            double[] rbp = new double[9];
            double[] rn = new double[9];
            double[] rbpn = new double[9];

            // Act
            Sofa.Pn06a(2400000.5, 53736.0, ref dpsi, ref deps, ref epsa, rb, rp, rbp, rn, rbpn);

            // Assert
            Assert.Equal(0.4090789763356509926, epsa, 12);
        }
    }

    /// <summary>
    /// xUnit tests for SOFA pnm00a function
    /// </summary>
    public class SofaPnm00aTests
    {
        [Fact]
        public void Pnm00a_WithTestValues_ReturnsCorrectMatrix()
        {
            // Arrange
            double[] rbpn = new double[9];

            // Act
            Sofa.Pnm00a(2400000.5, 50123.9999, rbpn);

            // Assert - check key matrix elements
            Assert.Equal(0.9999995832793134257, rbpn[0], 12);
            Assert.Equal(0.8372384254137809439e-3, rbpn[1], 14);
            Assert.Equal(0.3639684306407150645e-3, rbpn[2], 14);
            Assert.Equal(0.9999999329094390695, rbpn[8], 12);
        }
    }

    /// <summary>
    /// xUnit tests for SOFA pnm00b function
    /// </summary>
    public class SofaPnm00bTests
    {
        [Fact]
        public void Pnm00b_WithTestValues_ReturnsCorrectMatrix()
        {
            // Arrange
            double[] rbpn = new double[9];

            // Act
            Sofa.Pnm00b(2400000.5, 50123.9999, rbpn);

            // Assert - check key matrix elements
            Assert.Equal(0.9999995832776208280, rbpn[0], 12);
            Assert.Equal(0.8372401264429654837e-3, rbpn[1], 14);
            Assert.Equal(0.3639691681450271771e-3, rbpn[2], 14);
        }
    }

    /// <summary>
    /// xUnit tests for SOFA pnm06a function
    /// </summary>
    public class SofaPnm06aTests
    {
        [Fact]
        public void Pnm06a_WithTestValues_ReturnsCorrectMatrix()
        {
            // Arrange
            double[] rbpn = new double[9];

            // Act
            Sofa.Pnm06a(2400000.5, 50123.9999, rbpn);

            // Assert - check key matrix elements
            Assert.Equal(0.9999995832794205484, rbpn[0], 12);
            Assert.Equal(0.8372382772630962111e-3, rbpn[1], 14);
            Assert.Equal(0.3639684771140623099e-3, rbpn[2], 14);
        }
    }

    /// <summary>
    /// xUnit tests for SOFA pnm80 function
    /// </summary>
    public class SofaPnm80Tests
    {
        [Fact]
        public void Pnm80_WithTestValues_ReturnsCorrectMatrix()
        {
            // Arrange
            double[] rmatpn = new double[9];

            // Act
            Sofa.Pnm80(2400000.5, 50123.9999, rmatpn);

            // Assert - check key matrix elements
            Assert.Equal(0.9999995831934611169, rmatpn[0], 12);
            Assert.Equal(0.8373654045728124011e-3, rmatpn[1], 14);
            Assert.Equal(0.3639121916933106191e-3, rmatpn[2], 14);
        }
    }

    /// <summary>
    /// xUnit tests for SOFA pom00 function
    /// </summary>
    public class SofaPom00Tests
    {
        [Fact]
        public void Pom00_WithTestValues_ReturnsCorrectMatrix()
        {
            // Arrange
            double xp = 2.55060238e-7;
            double yp = 1.860359247e-6;
            double sp = -0.1367174580728891460e-10;
            double[] rpom = new double[9];

            // Act
            Sofa.Pom00(xp, yp, sp, rpom);

            // Assert - check key matrix elements
            Assert.Equal(0.9999999999999674721, rpom[0], 12);
            Assert.Equal(-0.1367174580728846989e-10, rpom[1], 15);
            Assert.Equal(0.2550602379999972345e-6, rpom[2], 15);
        }
    }

    /// <summary>
    /// xUnit tests for SOFA pr00 function
    /// </summary>
    public class SofaPr00Tests
    {
        [Fact]
        public void Pr00_WithTestValues_ReturnsCorrectDpsipr()
        {
            // Arrange
            double dpsipr = 0.0;
            double depspr = 0.0;

            // Act
            Sofa.Pr00(2400000.5, 53736, ref dpsipr, ref depspr);

            // Assert
            Assert.Equal(-0.8716465172668347629e-7, dpsipr, 15);
        }

        [Fact]
        public void Pr00_WithTestValues_ReturnsCorrectDepspr()
        {
            // Arrange
            double dpsipr = 0.0;
            double depspr = 0.0;

            // Act
            Sofa.Pr00(2400000.5, 53736, ref dpsipr, ref depspr);

            // Assert
            Assert.Equal(-0.7342018386722813087e-8, depspr, 15);
        }
    }

    /// <summary>
    /// xUnit tests for SOFA prec76 function
    /// </summary>
    public class SofaPrec76Tests
    {
        [Fact]
        public void Prec76_WithTestValues_ReturnsCorrectZeta()
        {
            // Arrange
            double zeta = 0.0;
            double z = 0.0;
            double theta = 0.0;

            // Act
            Sofa.Prec76(2400000.5, 33282.0, 2400000.5, 51544.0, ref zeta, ref z, ref theta);

            // Assert
            Assert.Equal(0.5588961642000161243e-2, zeta, 12);
        }

        [Fact]
        public void Prec76_WithTestValues_ReturnsCorrectZ()
        {
            // Arrange
            double zeta = 0.0;
            double z = 0.0;
            double theta = 0.0;

            // Act
            Sofa.Prec76(2400000.5, 33282.0, 2400000.5, 51544.0, ref zeta, ref z, ref theta);

            // Assert
            Assert.Equal(0.5589922365870680624e-2, z, 12);
        }

        [Fact]
        public void Prec76_WithTestValues_ReturnsCorrectTheta()
        {
            // Arrange
            double zeta = 0.0;
            double z = 0.0;
            double theta = 0.0;

            // Act
            Sofa.Prec76(2400000.5, 33282.0, 2400000.5, 51544.0, ref zeta, ref z, ref theta);

            // Assert
            Assert.Equal(0.4858945471687296760e-2, theta, 12);
        }
    }

    /// <summary>
    /// xUnit tests for SOFA pvstar function
    /// </summary>
    public class SofaPvstarTests
    {
        [Fact]
        public void Pvstar_WithTestValues_ReturnsCorrectRa()
        {
            // Arrange
            double[] pv = new double[6]
            {
                126668.5912743160601, 2136.792716839935195, -245251.2339876830091,
                -0.4051854035740712739e-2, -0.6253919754866173866e-2, 0.1189353719774107189e-1
            };
            double ra = 0.0;
            double dec = 0.0;
            double pmr = 0.0;
            double pmd = 0.0;
            double px = 0.0;
            double rv = 0.0;

            // Act
            int j = Sofa.Pvstar(pv, ref ra, ref dec, ref pmr, ref pmd, ref px, ref rv);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(0.1686756e-1, ra, 12);
        }

        [Fact]
        public void Pvstar_WithTestValues_ReturnsCorrectDec()
        {
            // Arrange
            double[] pv = new double[6]
            {
                126668.5912743160601, 2136.792716839935195, -245251.2339876830091,
                -0.4051854035740712739e-2, -0.6253919754866173866e-2, 0.1189353719774107189e-1
            };
            double ra = 0.0;
            double dec = 0.0;
            double pmr = 0.0;
            double pmd = 0.0;
            double px = 0.0;
            double rv = 0.0;

            // Act
            Sofa.Pvstar(pv, ref ra, ref dec, ref pmr, ref pmd, ref px, ref rv);

            // Assert
            Assert.Equal(-1.093989828, dec, 12);
        }

        [Fact]
        public void Pvstar_WithTestValues_ReturnsCorrectPmr()
        {
            // Arrange
            double[] pv = new double[6]
            {
                126668.5912743160601, 2136.792716839935195, -245251.2339876830091,
                -0.4051854035740712739e-2, -0.6253919754866173866e-2, 0.1189353719774107189e-1
            };
            double ra = 0.0;
            double dec = 0.0;
            double pmr = 0.0;
            double pmd = 0.0;
            double px = 0.0;
            double rv = 0.0;

            // Act
            Sofa.Pvstar(pv, ref ra, ref dec, ref pmr, ref pmd, ref px, ref rv);

            // Assert
            Assert.Equal(-0.1783235160000472788e-4, pmr, 15);
        }

        [Fact]
        public void Pvstar_WithTestValues_ReturnsCorrectPmd()
        {
            // Arrange
            double[] pv = new double[6]
            {
                126668.5912743160601, 2136.792716839935195, -245251.2339876830091,
                -0.4051854035740712739e-2, -0.6253919754866173866e-2, 0.1189353719774107189e-1
            };
            double ra = 0.0;
            double dec = 0.0;
            double pmr = 0.0;
            double pmd = 0.0;
            double px = 0.0;
            double rv = 0.0;

            // Act
            Sofa.Pvstar(pv, ref ra, ref dec, ref pmr, ref pmd, ref px, ref rv);

            // Assert
            Assert.Equal(0.2336024047000619347e-5, pmd, 15);
        }

        [Fact]
        public void Pvstar_WithTestValues_ReturnsCorrectPx()
        {
            // Arrange
            double[] pv = new double[6]
            {
                126668.5912743160601, 2136.792716839935195, -245251.2339876830091,
                -0.4051854035740712739e-2, -0.6253919754866173866e-2, 0.1189353719774107189e-1
            };
            double ra = 0.0;
            double dec = 0.0;
            double pmr = 0.0;
            double pmd = 0.0;
            double px = 0.0;
            double rv = 0.0;

            // Act
            Sofa.Pvstar(pv, ref ra, ref dec, ref pmr, ref pmd, ref px, ref rv);

            // Assert
            Assert.Equal(0.74723, px, 12);
        }

        [Fact]
        public void Pvstar_WithTestValues_ReturnsCorrectRv()
        {
            // Arrange
            double[] pv = new double[6]
            {
                126668.5912743160601, 2136.792716839935195, -245251.2339876830091,
                -0.4051854035740712739e-2, -0.6253919754866173866e-2, 0.1189353719774107189e-1
            };
            double ra = 0.0;
            double dec = 0.0;
            double pmr = 0.0;
            double pmd = 0.0;
            double px = 0.0;
            double rv = 0.0;

            // Act
            Sofa.Pvstar(pv, ref ra, ref dec, ref pmr, ref pmd, ref px, ref rv);

            // Assert
            Assert.Equal(-21.60000010107306010, rv, 11);
        }
    }

    /// <summary>
    /// xUnit tests for SOFA pvtob function
    /// </summary>
    public class SofaPvtobTests
    {
        [Fact]
        public void Pvtob_WithTestValues_ReturnsCorrectPositionX()
        {
            // Arrange
            double[] pv = new double[6];

            // Act
            Sofa.Pvtob(2.0, 0.5, 3000.0, 1e-6, -0.5e-6, 1e-8, 5.0, pv);

            // Assert
            Assert.Equal(4225081.367071159207, pv[0], 5);
        }

        [Fact]
        public void Pvtob_WithTestValues_ReturnsCorrectPositionY()
        {
            // Arrange
            double[] pv = new double[6];

            // Act
            Sofa.Pvtob(2.0, 0.5, 3000.0, 1e-6, -0.5e-6, 1e-8, 5.0, pv);

            // Assert
            Assert.Equal(3681943.215856198144, pv[1], 5);
        }

        [Fact]
        public void Pvtob_WithTestValues_ReturnsCorrectPositionZ()
        {
            // Arrange
            double[] pv = new double[6];

            // Act
            Sofa.Pvtob(2.0, 0.5, 3000.0, 1e-6, -0.5e-6, 1e-8, 5.0, pv);

            // Assert
            Assert.Equal(3041149.399241260785, pv[2], 5);
        }

        [Fact]
        public void Pvtob_WithTestValues_ReturnsCorrectVelocityX()
        {
            // Arrange
            double[] pv = new double[6];

            // Act
            Sofa.Pvtob(2.0, 0.5, 3000.0, 1e-6, -0.5e-6, 1e-8, 5.0, pv);

            // Assert
            Assert.Equal(-268.4915389365998787, pv[3], 9);
        }

        [Fact]
        public void Pvtob_WithTestValues_ReturnsCorrectVelocityY()
        {
            // Arrange
            double[] pv = new double[6];

            // Act
            Sofa.Pvtob(2.0, 0.5, 3000.0, 1e-6, -0.5e-6, 1e-8, 5.0, pv);

            // Assert
            Assert.Equal(308.0977983288903123, pv[4], 9);
        }

        [Fact]
        public void Pvtob_WithTestValues_ReturnsCorrectVelocityZ()
        {
            // Arrange
            double[] pv = new double[6];

            // Act
            Sofa.Pvtob(2.0, 0.5, 3000.0, 1e-6, -0.5e-6, 1e-8, 5.0, pv);

            // Assert
            Assert.Equal(0, pv[5], 0);
        }
    }

    /// <summary>
    /// xUnit tests for SOFA pvu function
    /// </summary>
    public class SofaPvuTests
    {
        [Fact]
        public void Pvu_WithTestValues_ReturnsCorrectUpdatedPositionX()
        {
            // Arrange
            double dt = 2920.0;
            double[] pv = new double[6]
            {
                126668.5912743160734, 2136.792716839935565, -245251.2339876830229,
                -0.4051854035740713039e-2, -0.6253919754866175788e-2, 0.1189353719774107615e-1
            };
            double[] upv = new double[6];

            // Act
            Sofa.Pvu(dt, pv, upv);

            // Assert
            Assert.Equal(126656.7598605317105, upv[0], 6);
        }

        [Fact]
        public void Pvu_WithTestValues_ReturnsCorrectUpdatedPositionY()
        {
            // Arrange
            double dt = 2920.0;
            double[] pv = new double[6]
            {
                126668.5912743160734, 2136.792716839935565, -245251.2339876830229,
                -0.4051854035740713039e-2, -0.6253919754866175788e-2, 0.1189353719774107615e-1
            };
            double[] upv = new double[6];

            // Act
            Sofa.Pvu(dt, pv, upv);

            // Assert
            Assert.Equal(2118.531271155726332, upv[1], 8);
        }

        [Fact]
        public void Pvu_WithTestValues_ReturnsCorrectUpdatedPositionZ()
        {
            // Arrange
            double dt = 2920.0;
            double[] pv = new double[6]
            {
                126668.5912743160734, 2136.792716839935565, -245251.2339876830229,
                -0.4051854035740713039e-2, -0.6253919754866175788e-2, 0.1189353719774107615e-1
            };
            double[] upv = new double[6];

            // Act
            Sofa.Pvu(dt, pv, upv);

            // Assert
            Assert.Equal(-245216.5048590656190, upv[2], 6);
        }

        [Fact]
        public void Pvu_WithTestValues_ReturnsUnchangedVelocity()
        {
            // Arrange
            double dt = 2920.0;
            double[] pv = new double[6]
            {
                126668.5912743160734, 2136.792716839935565, -245251.2339876830229,
                -0.4051854035740713039e-2, -0.6253919754866175788e-2, 0.1189353719774107615e-1
            };
            double[] upv = new double[6];

            // Act
            Sofa.Pvu(dt, pv, upv);

            // Assert
            Assert.Equal(pv[3], upv[3], 12);
            Assert.Equal(pv[4], upv[4], 12);
            Assert.Equal(pv[5], upv[5], 12);
        }
    }

    /// <summary>
    /// xUnit tests for SOFA pvup function
    /// </summary>
    public class SofaPvupTests
    {
        [Fact]
        public void Pvup_WithTestValues_ReturnsCorrectPositionX()
        {
            // Arrange
            double dt = 2920.0;
            double[] pv = new double[6]
            {
                126668.5912743160734, 2136.792716839935565, -245251.2339876830229,
                -0.4051854035740713039e-2, -0.6253919754866175788e-2, 0.1189353719774107615e-1
            };
            double[] p = new double[3];

            // Act
            Sofa.Pvup(dt, pv, p);

            // Assert
            Assert.Equal(126656.7598605317105, p[0], 6);
        }

        [Fact]
        public void Pvup_WithTestValues_ReturnsCorrectPositionY()
        {
            // Arrange
            double dt = 2920.0;
            double[] pv = new double[6]
            {
                126668.5912743160734, 2136.792716839935565, -245251.2339876830229,
                -0.4051854035740713039e-2, -0.6253919754866175788e-2, 0.1189353719774107615e-1
            };
            double[] p = new double[3];

            // Act
            Sofa.Pvup(dt, pv, p);

            // Assert
            Assert.Equal(2118.531271155726332, p[1], 8);
        }

        [Fact]
        public void Pvup_WithTestValues_ReturnsCorrectPositionZ()
        {
            // Arrange
            double dt = 2920.0;
            double[] pv = new double[6]
            {
                126668.5912743160734, 2136.792716839935565, -245251.2339876830229,
                -0.4051854035740713039e-2, -0.6253919754866175788e-2, 0.1189353719774107615e-1
            };
            double[] p = new double[3];

            // Act
            Sofa.Pvup(dt, pv, p);

            // Assert
            Assert.Equal(-245216.5048590656190, p[2], 6);
        }
    }

    /// <summary>
    /// xUnit tests for SOFA s00 function
    /// </summary>
    public class SofaS00Tests
    {
        [Fact]
        public void S00_WithTestValues_ReturnsCorrectValue()
        {
            // Arrange
            double x = 0.5791308486706011000e-3;
            double y = 0.4020579816732961219e-4;

            // Act
            double s = Sofa.S00(2400000.5, 53736.0, x, y);

            // Assert
            Assert.Equal(-0.1220036263270905693e-7, s, 15);
        }
    }

    /// <summary>
    /// xUnit tests for SOFA s00a function
    /// </summary>
    public class SofaS00aTests
    {
        [Fact]
        public void S00a_WithTestValues_ReturnsCorrectValue()
        {
            // Arrange & Act
            double s = Sofa.S00a(2400000.5, 52541.0);

            // Assert
            Assert.Equal(-0.1340684448919163584e-7, s, 15);
        }
    }

    /// <summary>
    /// xUnit tests for SOFA s00b function
    /// </summary>
    public class SofaS00bTests
    {
        [Fact]
        public void S00b_WithTestValues_ReturnsCorrectValue()
        {
            // Arrange & Act
            double s = Sofa.S00b(2400000.5, 52541.0);

            // Assert
            Assert.Equal(-0.1340695782951026584e-7, s, 15);
        }
    }

    /// <summary>
    /// xUnit tests for SOFA s06 function
    /// </summary>
    public class SofaS06Tests
    {
        [Fact]
        public void S06_WithTestValues_ReturnsCorrectValue()
        {
            // Arrange
            double x = 0.5791308486706011000e-3;
            double y = 0.4020579816732961219e-4;

            // Act
            double s = Sofa.S06(2400000.5, 53736.0, x, y);

            // Assert
            Assert.Equal(-0.1220032213076463117e-7, s, 15);
        }
    }

    /// <summary>
    /// xUnit tests for SOFA s06a function
    /// </summary>
    public class SofaS06aTests
    {
        [Fact]
        public void S06a_WithTestValues_ReturnsCorrectValue()
        {
            // Arrange & Act
            double s = Sofa.S06a(2400000.5, 52541.0);

            // Assert
            Assert.Equal(-0.1340680437291812383e-7, s, 15);
        }
    }

    /// <summary>
    /// xUnit tests for SOFA s2xpv function
    /// </summary>
    public class SofaS2xpvTests
    {
        [Fact]
        public void S2xpv_WithTestValues_ReturnsCorrectPositionScaling()
        {
            // Arrange
            double s1 = 2.0;
            double s2 = 3.0;
            double[] pv = new double[6] { 0.3, 1.2, -2.5, 0.5, 2.3, -0.4 };
            double[] spv = new double[6];

            // Act
            Sofa.S2xpv(s1, s2, pv, spv);

            // Assert
            Assert.Equal(0.6, spv[0], 12);
            Assert.Equal(2.4, spv[1], 12);
            Assert.Equal(-5.0, spv[2], 12);
        }

        [Fact]
        public void S2xpv_WithTestValues_ReturnsCorrectVelocityScaling()
        {
            // Arrange
            double s1 = 2.0;
            double s2 = 3.0;
            double[] pv = new double[6] { 0.3, 1.2, -2.5, 0.5, 2.3, -0.4 };
            double[] spv = new double[6];

            // Act
            Sofa.S2xpv(s1, s2, pv, spv);

            // Assert
            Assert.Equal(1.5, spv[3], 12);
            Assert.Equal(6.9, spv[4], 12);
            Assert.Equal(-1.2, spv[5], 12);
        }
    }

    /// <summary>
    /// xUnit tests for SOFA sp00 function
    /// </summary>
    public class SofaSp00Tests
    {
        [Fact]
        public void Sp00_WithTestValues_ReturnsCorrectValue()
        {
            // Arrange & Act
            double sp = Sofa.Sp00(2400000.5, 52541.0);

            // Assert
            Assert.Equal(-0.6216698469981019309e-11, sp, 12);
        }
    }

    /// <summary>
    /// xUnit tests for SOFA starpm function
    /// </summary>
    public class SofaStarpmTests
    {
        [Fact]
        public void Starpm_WithTestValues_ReturnsCorrectRa()
        {
            // Arrange
            double ra1 = 0.01686756;
            double dec1 = -1.093989828;
            double pmr1 = -1.78323516e-5;
            double pmd1 = 2.336024047e-6;
            double px1 = 0.74723;
            double rv1 = -21.6;
            double ra2 = 0.0;
            double dec2 = 0.0;
            double pmr2 = 0.0;
            double pmd2 = 0.0;
            double px2 = 0.0;
            double rv2 = 0.0;

            // Act
            int j = Sofa.Starpm(ra1, dec1, pmr1, pmd1, px1, rv1,
                                2400000.5, 50083.0, 2400000.5, 53736.0,
                                ref ra2, ref dec2, ref pmr2, ref pmd2, ref px2, ref rv2);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(0.01668919069414256149, ra2, 13);
        }

        [Fact]
        public void Starpm_WithTestValues_ReturnsCorrectDec()
        {
            // Arrange
            double ra1 = 0.01686756;
            double dec1 = -1.093989828;
            double pmr1 = -1.78323516e-5;
            double pmd1 = 2.336024047e-6;
            double px1 = 0.74723;
            double rv1 = -21.6;
            double ra2 = 0.0;
            double dec2 = 0.0;
            double pmr2 = 0.0;
            double pmd2 = 0.0;
            double px2 = 0.0;
            double rv2 = 0.0;

            // Act
            Sofa.Starpm(ra1, dec1, pmr1, pmd1, px1, rv1,
                        2400000.5, 50083.0, 2400000.5, 53736.0,
                        ref ra2, ref dec2, ref pmr2, ref pmd2, ref px2, ref rv2);

            // Assert
            Assert.Equal(-1.093966454217127897, dec2, 13);
        }

        [Fact]
        public void Starpm_WithTestValues_ReturnsCorrectPmr()
        {
            // Arrange
            double ra1 = 0.01686756;
            double dec1 = -1.093989828;
            double pmr1 = -1.78323516e-5;
            double pmd1 = 2.336024047e-6;
            double px1 = 0.74723;
            double rv1 = -21.6;
            double ra2 = 0.0;
            double dec2 = 0.0;
            double pmr2 = 0.0;
            double pmd2 = 0.0;
            double px2 = 0.0;
            double rv2 = 0.0;

            // Act
            Sofa.Starpm(ra1, dec1, pmr1, pmd1, px1, rv1,
                        2400000.5, 50083.0, 2400000.5, 53736.0,
                        ref ra2, ref dec2, ref pmr2, ref pmd2, ref px2, ref rv2);

            // Assert
            Assert.Equal(-0.1783662682153176524e-4, pmr2, 15);
        }

        [Fact]
        public void Starpm_WithTestValues_ReturnsCorrectPmd()
        {
            // Arrange
            double ra1 = 0.01686756;
            double dec1 = -1.093989828;
            double pmr1 = -1.78323516e-5;
            double pmd1 = 2.336024047e-6;
            double px1 = 0.74723;
            double rv1 = -21.6;
            double ra2 = 0.0;
            double dec2 = 0.0;
            double pmr2 = 0.0;
            double pmd2 = 0.0;
            double px2 = 0.0;
            double rv2 = 0.0;

            // Act
            Sofa.Starpm(ra1, dec1, pmr1, pmd1, px1, rv1,
                        2400000.5, 50083.0, 2400000.5, 53736.0,
                        ref ra2, ref dec2, ref pmr2, ref pmd2, ref px2, ref rv2);

            // Assert
            Assert.Equal(0.2338092915983989595e-5, pmd2, 15);
        }

        [Fact]
        public void Starpm_WithTestValues_ReturnsCorrectPx()
        {
            // Arrange
            double ra1 = 0.01686756;
            double dec1 = -1.093989828;
            double pmr1 = -1.78323516e-5;
            double pmd1 = 2.336024047e-6;
            double px1 = 0.74723;
            double rv1 = -21.6;
            double ra2 = 0.0;
            double dec2 = 0.0;
            double pmr2 = 0.0;
            double pmd2 = 0.0;
            double px2 = 0.0;
            double rv2 = 0.0;

            // Act
            Sofa.Starpm(ra1, dec1, pmr1, pmd1, px1, rv1,
                        2400000.5, 50083.0, 2400000.5, 53736.0,
                        ref ra2, ref dec2, ref pmr2, ref pmd2, ref px2, ref rv2);

            // Assert
            Assert.Equal(0.7473533835317719243, px2, 13);
        }

        [Fact]
        public void Starpm_WithTestValues_ReturnsCorrectRv()
        {
            // Arrange
            double ra1 = 0.01686756;
            double dec1 = -1.093989828;
            double pmr1 = -1.78323516e-5;
            double pmd1 = 2.336024047e-6;
            double px1 = 0.74723;
            double rv1 = -21.6;
            double ra2 = 0.0;
            double dec2 = 0.0;
            double pmr2 = 0.0;
            double pmd2 = 0.0;
            double px2 = 0.0;
            double rv2 = 0.0;

            // Act
            Sofa.Starpm(ra1, dec1, pmr1, pmd1, px1, rv1,
                        2400000.5, 50083.0, 2400000.5, 53736.0,
                        ref ra2, ref dec2, ref pmr2, ref pmd2, ref px2, ref rv2);

            // Assert
            Assert.Equal(-21.59905170476417175, rv2, 11);
        }
    }

    /// <summary>
    /// xUnit tests for SOFA starpv function
    /// </summary>
    public class SofaStarpvTests
    {
        [Fact]
        public void Starpv_WithTestValues_ReturnsCorrectPositionX()
        {
            // Arrange
            double ra = 0.01686756;
            double dec = -1.093989828;
            double pmr = -1.78323516e-5;
            double pmd = 2.336024047e-6;
            double px = 0.74723;
            double rv = -21.6;
            double[] pv = new double[6];

            // Act
            int j = Sofa.Starpv(ra, dec, pmr, pmd, px, rv, pv);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(126668.5912743160601, pv[0], 10, MidpointRounding.ToPositiveInfinity);
            Assert.Equal(2136.792716839935195, pv[1], 10);
            Assert.Equal(-245251.2339876830091, pv[2], 10);
            Assert.Equal(-0.4051854008955659551e-2, pv[3], 13);
            Assert.Equal(-0.6253919754414777970e-2, pv[4], 15);
            Assert.Equal(0.1189353714588109341e-1, pv[5], 13);
        }
    }

    /// <summary>
    /// xUnit tests for SOFA taiut1 function
    /// </summary>
    public class SofaTaiut1Tests
    {
        [Fact]
        public void Taiut1_WithTestValues_ReturnsCorrectU1()
        {
            // Arrange
            double u1 = 0.0;
            double u2 = 0.0;

            // Act
            int j = Sofa.Taiut1(2453750.5, 0.892482639, -32.6659, ref u1, ref u2);

            // Assert
            Assert.Equal(0, j);
            Assert.Equal(2453750.5, u1, 6);
        }

        [Fact]
        public void Taiut1_WithTestValues_ReturnsCorrectU2()
        {
            // Arrange
            double u1 = 0.0;
            double u2 = 0.0;

            // Act
            Sofa.Taiut1(2453750.5, 0.892482639, -32.6659, ref u1, ref u2);

            // Assert
            Assert.Equal(0.8921045614537037037, u2, 12);
        }
    }
}
