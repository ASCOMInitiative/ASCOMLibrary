using ASCOM.Common.Interfaces;
using ASCOM.Tools;
using ASCOM.Tools.Novas31;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NOVAS
{
    public class NovasTests
    {
        private readonly ITestOutputHelper output;

        public NovasTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void AstroMercury()
        {
            double[] pVec = new double[3];
            double[] vVec = new double[3];
            double[] tJd = new double[2];
            tJd[0] = AstroUtilities.JulianDateFromDateTime(new DateTime(2024, 1, 5, 12, 0, 0, DateTimeKind.Utc));
            tJd[1] = 0.0;

            short rc = Novas.PlanetEphemeris(ref tJd, Target.Mercury, Target.Earth, ref pVec, ref vVec);
            Assert.Equal(0, rc);

            output.WriteLine($"pVec[0]: {pVec[0]}, pVec[1]: {pVec[1]}, pVec[2]: {pVec[2]}, ");
            double ra = 0.0, declination = 0.0;
            Novas.Vector2RaDec(pVec, ref ra, ref declination);

            double distance = Math.Sqrt(pVec[0] * pVec[0] + pVec[1] * pVec[1] + pVec[2] * pVec[2]);
            output.WriteLine($"RA: {Utilities.HoursToHMS(ra, ":", ":", "", 3)}, Declination: {Utilities.DegreesToDMS(declination, ":", ":", "", 2)}, Distance: {distance}.");
            output.WriteLine($"RA: {ra}, Declination: {declination}, Distance: {distance}.");

            // Checked against USNO calculated values
            Assert.Equal(17.47651673780692, ra, 0.00001);
            Assert.Equal(-20.60064716207346, declination, 0.00001);
            Assert.Equal(0.8692408902938233, distance, 0.00001);
        }

        [Fact]
        public void AstroVenus()
        {
            double[] pVec = new double[3];
            double[] vVec = new double[3];
            double[] tJd = new double[2];
            tJd[0] = AstroUtilities.JulianDateFromDateTime(new DateTime(2024, 1, 5, 12, 0, 0, DateTimeKind.Utc));
            tJd[1] = 0.0;

            short rc = Novas.PlanetEphemeris(ref tJd, Target.Venus, Target.Earth, ref pVec, ref vVec);
            Assert.Equal(0, rc);

            output.WriteLine($"pVec[0]: {pVec[0]}, pVec[1]: {pVec[1]}, pVec[2]: {pVec[2]}, ");
            double ra = 0.0, declination = 0.0;
            Novas.Vector2RaDec(pVec, ref ra, ref declination);

            double distance = Math.Sqrt(pVec[0] * pVec[0] + pVec[1] * pVec[1] + pVec[2] * pVec[2]);
            output.WriteLine($"RA: {Utilities.HoursToHMS(ra, ":", ":", "", 3)}, Declination: {Utilities.DegreesToDMS(declination, ":", ":", "", 2)}, Distance: {distance}.");
            output.WriteLine($"RA: {ra}, Declination: {declination}, Distance: {distance}.");

            // Checked against USNO calculated values
            Assert.Equal(16.419824330022884, ra, 0.00001);
            Assert.Equal(-19.820478040768513, declination, 0.00001);
            Assert.Equal(1.210012322577794, distance, 0.00001);
        }

        [Fact]
        public void AstroMars()
        {
            double[] pVec = new double[3];
            double[] vVec = new double[3];
            double[] tJd = new double[2];
            tJd[0] = AstroUtilities.JulianDateFromDateTime(new DateTime(2024, 1, 5, 12, 0, 0, DateTimeKind.Utc));
            tJd[1] = 0.0;

            short rc = Novas.PlanetEphemeris(ref tJd, Target.Mars, Target.Earth, ref pVec, ref vVec);
            Assert.Equal(0, rc);

            output.WriteLine($"pVec[0]: {pVec[0]}, pVec[1]: {pVec[1]}, pVec[2]: {pVec[2]}, ");
            double ra = 0.0, declination = 0.0;
            Novas.Vector2RaDec(pVec, ref ra, ref declination);

            double distance = Math.Sqrt(pVec[0] * pVec[0] + pVec[1] * pVec[1] + pVec[2] * pVec[2]);
            output.WriteLine($"RA: {Utilities.HoursToHMS(ra, ":", ":", "", 3)}, Declination: {Utilities.DegreesToDMS(declination, ":", ":", "", 2)}, Distance: {distance}.");
            output.WriteLine($"RA: {ra}, Declination: {declination}, Distance: {distance}.");

            // Checked against USNO calculated values
            Assert.Equal(18.023963294288805, ra, 0.00001);
            Assert.Equal(-24.03001586934732, declination, 0.00001);
            Assert.Equal(2.4098899813659913, distance, 0.00001);
        }

        [Fact]
        public void AstroJupiter()
        {
            double[] pVec = new double[3];
            double[] vVec = new double[3];
            double[] tJd = new double[2];
            tJd[0] = AstroUtilities.JulianDateFromDateTime(new DateTime(2024, 1, 5, 12, 0, 0, DateTimeKind.Utc));
            tJd[1] = 0.0;

            short rc = Novas.PlanetEphemeris(ref tJd, Target.Jupiter, Target.Earth, ref pVec, ref vVec);
            Assert.Equal(0, rc);

            output.WriteLine($"pVec[0]: {pVec[0]}, pVec[1]: {pVec[1]}, pVec[2]: {pVec[2]}, ");
            double ra = 0.0, declination = 0.0;
            Novas.Vector2RaDec(pVec, ref ra, ref declination);

            double distance = Math.Sqrt(pVec[0] * pVec[0] + pVec[1] * pVec[1] + pVec[2] * pVec[2]);
            output.WriteLine($"RA: {Utilities.HoursToHMS(ra, ":", ":", "", 3)}, Declination: {Utilities.DegreesToDMS(declination, ":", ":", "", 2)}, Distance: {distance}.");
            output.WriteLine($"RA: {ra}, Declination: {declination}, Distance: {distance}.");

            // Checked against USNO calculated values
            Assert.Equal(2.2268997644601067, ra, 0.00001);
            Assert.Equal(12.19002390258749, declination, 0.00001);
            Assert.Equal(4.548169812915969, distance, 0.00001);
        }

        [Fact]
        public void AstroSaturn()
        {
            double[] pVec = new double[3];
            double[] vVec = new double[3];
            double[] tJd = new double[2];
            tJd[0] = AstroUtilities.JulianDateFromDateTime(new DateTime(2024, 1, 5, 12, 0, 0, DateTimeKind.Utc));
            tJd[1] = 0.0;

            short rc = Novas.PlanetEphemeris(ref tJd, Target.Saturn, Target.Earth, ref pVec, ref vVec);
            Assert.Equal(0, rc);

            output.WriteLine($"pVec[0]: {pVec[0]}, pVec[1]: {pVec[1]}, pVec[2]: {pVec[2]}, ");
            double ra = 0.0, declination = 0.0;
            Novas.Vector2RaDec(pVec, ref ra, ref declination);

            double distance = Math.Sqrt(pVec[0] * pVec[0] + pVec[1] * pVec[1] + pVec[2] * pVec[2]);
            output.WriteLine($"RA: {Utilities.HoursToHMS(ra, ":", ":", "", 3)}, Declination: {Utilities.DegreesToDMS(declination, ":", ":", "", 2)}, Distance: {distance}.");
            output.WriteLine($"RA: {ra}, Declination: {declination}, Distance: {distance}.");

            // Checked against USNO calculated values
            Assert.Equal(22.3902048324913, ra, 0.00001);
            Assert.Equal(-11.80418186285705, declination, 0.00001);
            Assert.Equal(10.352680884850226, distance, 0.00001);
        }

        [Fact]
        public void AstroUranus()
        {
            double[] pVec = new double[3];
            double[] vVec = new double[3];
            double[] tJd = new double[2];

            TraceLogger logger = new TraceLogger("NovasUranus", true);
            logger.SetMinimumLoggingLevel(LogLevel.Debug);
            Novas.SetLogger(logger);

            tJd[0] = AstroUtilities.JulianDateFromDateTime(new DateTime(2024, 1, 5, 12, 0, 0, DateTimeKind.Utc));
            tJd[1] = 0.0;

            short rc = Novas.PlanetEphemeris(ref tJd, Target.Uranus, Target.Earth, ref pVec, ref vVec);
            Assert.Equal(0, rc);

            output.WriteLine($"pVec[0]: {pVec[0]}, pVec[1]: {pVec[1]}, pVec[2]: {pVec[2]}, ");
            double ra = 0.0, declination = 0.0;
            Novas.Vector2RaDec(pVec, ref ra, ref declination);

            double distance = Math.Sqrt(pVec[0] * pVec[0] + pVec[1] * pVec[1] + pVec[2] * pVec[2]);
            output.WriteLine($"RA: {Utilities.HoursToHMS(ra, ":", ":", "", 3)}, Declination: {Utilities.DegreesToDMS(declination, ":", ":", "", 2)}, Distance: {distance}.");
            output.WriteLine($"RA: {ra}, Declination: {declination}, Distance: {distance}.");

            // Checked against USNO calculated values
            Assert.Equal(3.1059876325655744, ra, 0.00001);
            Assert.Equal(17.162970029489234, declination, 0.00001);
            Assert.Equal(19.036799064973593, distance, 0.00001);
        }

        [Fact]
        public void AstroNeptune()
        {
            double[] pVec = new double[3];
            double[] vVec = new double[3];
            double[] tJd = new double[2];
            tJd[0] = AstroUtilities.JulianDateFromDateTime(new DateTime(2024, 1, 5, 12, 0, 0, DateTimeKind.Utc));
            tJd[1] = 0.0;

            short rc = Novas.PlanetEphemeris(ref tJd, Target.Neptune, Target.Earth, ref pVec, ref vVec);
            Assert.Equal(0, rc);

            output.WriteLine($"pVec[0]: {pVec[0]}, pVec[1]: {pVec[1]}, pVec[2]: {pVec[2]}, ");
            double ra = 0.0, declination = 0.0;
            Novas.Vector2RaDec(pVec, ref ra, ref declination);

            double distance = Math.Sqrt(pVec[0] * pVec[0] + pVec[1] * pVec[1] + pVec[2] * pVec[2]);
            output.WriteLine($"RA: {Utilities.HoursToHMS(ra, ":", ":", "", 3)}, Declination: {Utilities.DegreesToDMS(declination, ":", ":", "", 2)}, Distance: {distance}.");
            output.WriteLine($"RA: {ra}, Declination: {declination}, Distance: {distance}.");

            // Checked against USNO calculated values
            Assert.Equal(23.715465001277977, ra, 0.00001);
            Assert.Equal(-3.1931055649394424, declination, 0.00001);
            Assert.Equal(30.21711818951698, distance, 0.00001);
        }


        [Fact]
        public void AstroPluto()
        {
            double[] pVec = new double[3];
            double[] vVec = new double[3];
            double[] tJd = new double[2];
            tJd[0] = AstroUtilities.JulianDateFromDateTime(new DateTime(2024, 1, 5, 12, 0, 0, DateTimeKind.Utc));
            tJd[1] = 0.0;

            short rc = Novas.PlanetEphemeris(ref tJd, Target.Pluto, Target.Earth, ref pVec, ref vVec);
            Assert.Equal(0, rc);

            output.WriteLine($"pVec[0]: {pVec[0]}, pVec[1]: {pVec[1]}, pVec[2]: {pVec[2]}, ");
            double ra = 0.0, declination = 0.0;
            Novas.Vector2RaDec(pVec, ref ra, ref declination);

            double distance = Math.Sqrt(pVec[0] * pVec[0] + pVec[1] * pVec[1] + pVec[2] * pVec[2]);
            output.WriteLine($"RA: {Utilities.HoursToHMS(ra, ":", ":", "", 3)}, Declination: {Utilities.DegreesToDMS(declination, ":", ":", "", 2)}, Distance: {distance}.");
            output.WriteLine($"RA: {ra}, Declination: {declination}, Distance: {distance}.");

            // Checked against USNO calculated values
            Assert.Equal(20.129297063711935, ra, 0.00001);
            Assert.Equal(-23.03235598017168, declination, 0.00001);
            Assert.Equal(35.87423225530874, distance, 0.00001);
        }

        [Fact]
        public void PlanetDateBeforeEphemerisStart()
        {
            double[] pVec = new double[3];
            double[] vVec = new double[3];
            double[] tJd = new double[2];

            TraceLogger logger = new TraceLogger("NovasUranus", true);
            logger.SetMinimumLoggingLevel(LogLevel.Debug);
            Novas.SetLogger(logger);

            tJd[0] = AstroUtilities.JulianDateFromDateTime(new DateTime(2019, 1, 5, 12, 0, 0, DateTimeKind.Utc));
            tJd[1] = 0.0;

            short rc = Novas.PlanetEphemeris(ref tJd, Target.Uranus, Target.Earth, ref pVec, ref vVec);
            Assert.Equal(2, rc);

            output.WriteLine($"pVec[0]: {pVec[0]}, pVec[1]: {pVec[1]}, pVec[2]: {pVec[2]}, ");

            // Confirm values are 0.0
            Assert.Equal(0.0, pVec[0]);
            Assert.Equal(0.0, pVec[1]);
            Assert.Equal(0.0, pVec[2]);
        }

        [Fact]
        public void PlanetDateAfterEphemerisEnd()
        {
            double[] pVec = new double[3];
            double[] vVec = new double[3];
            double[] tJd = new double[2];

            TraceLogger logger = new TraceLogger("NovasUranus", true);
            logger.SetMinimumLoggingLevel(LogLevel.Debug);
            Novas.SetLogger(logger);

            tJd[0] = AstroUtilities.JulianDateFromDateTime(new DateTime(2050, 1, 5, 12, 0, 0, DateTimeKind.Utc));
            tJd[1] = 0.0;

            short rc = Novas.PlanetEphemeris(ref tJd, Target.Uranus, Target.Earth, ref pVec, ref vVec);
            Assert.Equal(2,rc); // RC 1 is a warning not an error

            output.WriteLine($"pVec[0]: {pVec[0]}, pVec[1]: {pVec[1]}, pVec[2]: {pVec[2]}, ");

            // Confirm values are 0.0
            Assert.Equal(0.0, pVec[0]);
            Assert.Equal(0.0, pVec[1]);
            Assert.Equal(0.0, pVec[2]);
        }
    }
}
