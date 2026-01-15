using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit;
using ASCOM.Tools;
using ASCOM.Tools.Novas31;
using System.Globalization;
using ASCOM.Com.DriverAccess;
using ASCOM;
using ASCOM.Alpaca.Tests;

namespace Astrometry.SolarSystem
{
    public class SolarSystemTests
    {
        private readonly ITestOutputHelper output;

        public SolarSystemTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void FailureNoOrbitSet()
        {
            Assert.Throws<HelperException>(() =>
            {
                SolarSystemBody comet = new SolarSystemBody();
                comet.AstrometricCoordinates(DateTime.Now);
            });
        }

        [Fact]
        public void FailureOrbitStringNull()
        {
            Assert.Throws<HelperException>(() =>
            {
                SolarSystemBody comet = new SolarSystemBody(SolarSystemBody.OrbitDataSource.MpcCometOrbit, null);
            });
        }

        [Fact]
        public void FailureOrbitStringEmpty()
        {
            Assert.Throws<HelperException>(() =>
            {
                SolarSystemBody comet = new SolarSystemBody(SolarSystemBody.OrbitDataSource.MpcCometOrbit, "");
            });
        }

        [Fact]
        public void FailureOrbitStringShort()
        {
            Assert.Throws<HelperException>(() =>
            {
                SolarSystemBody comet = new SolarSystemBody(SolarSystemBody.OrbitDataSource.MpcCometOrbit, "    CK23V010  2025 07 13.1576  5.092857  0.999309  103.3112   15.0240  102.0074  20231106   8.5  4.0");
            });
        }

        [Fact]
        public void FailureOrbitStringBadValue()
        {
            Assert.Throws<HelperException>(() =>
            {
                SolarSystemBody comet = new SolarSystemBody(SolarSystemBody.OrbitDataSource.MpcCometOrbit, "\"    CK23V010  2025 07 XX.YYYY  5.092857  0.999309  103.3112   15.0240  102.0074  20231106   8.5  4.0  C/2023 V1 (Lemmon)                                       MPEC 2023-V23\"");
            });
        }

        [Fact]
        public void EphemerisCometOrbit()
        {
            const double TEST_TOLERANCE = 0.0003; // Degrees

            Ephemeris ephemeris = new Ephemeris();
            ephemeris.BodyType = BodyType.Comet;
            ephemeris.Name = "C/2023 V1";

            ephemeris.Epoch = 2460869.6576;
            ephemeris.q_PerihelionDistance = 5.092857;
            ephemeris.e_OrbitalEccentricity = 0.999309;
            ephemeris.i_Inclination = 102.0074;
            ephemeris.w_PerihelionArgument = 103.3112;
            ephemeris.Node = 15.024;

            SolarSystemBody comet = new SolarSystemBody();
            comet.Ephemeris = ephemeris;

            DateTime targetTime = new DateTime(2023, 11, 6, 22, 00, 00, DateTimeKind.Utc);

            output.WriteLine(
                $"  Epoch: {comet.Ephemeris.Epoch} ({AstroUtilities.JulianDateToDateTime(comet.Ephemeris.Epoch)}) , \r\n" +
                $"q Perihelion distance: {comet.Ephemeris.q_PerihelionDistance}, \r\n" +
                $"e Orbit eccentricity: {comet.Ephemeris.e_OrbitalEccentricity}, \r\n" +
                $"i Inclination: {comet.Ephemeris.i_Inclination}, \r\n" +
                $"w Perihelion argument: {comet.Ephemeris.w_PerihelionArgument}, \r\n" +
                $"  Node: {comet.Ephemeris.Node}, \r\n" +
                $"");

            Coordinates cometCoordinates = comet.AstrometricCoordinates(targetTime);

            output.WriteLine($"Comet Name: {comet.Name}");
            output.WriteLine($"Comet RA on {targetTime} {targetTime.Kind} - {Utilities.HoursToHMS(cometCoordinates.RightAscension)} ({cometCoordinates.RightAscension}), Declination: {Utilities.DegreesToDMS(cometCoordinates.Declination)} ({cometCoordinates.Declination})");
            output.WriteLine($"Test tolerance: {Utilities.DegreesToDMS(TEST_TOLERANCE, ":", ":", "", 2)} Degrees, Minutes, Seconds");

            Assert.Equal(22.29345332973845, cometCoordinates.RightAscension, TEST_TOLERANCE);
            Assert.Equal(39.72864756380414, cometCoordinates.Declination, TEST_TOLERANCE);
            Assert.Equal("C/2023 V1", comet.Name);
        }

        [Fact]
        public void EphemerisAsteroidOrbit()
        {
            const double TEST_TOLERANCE = 0.0015; // Degrees

            Ephemeris ephemeris = new Ephemeris();
            ephemeris.BodyType = BodyType.MinorPlanet;
            ephemeris.Name = "Ceres";

            DateTime epochDateTime = new DateTime(2023, 11, 8, 0, 0, 0);
            epochDateTime = epochDateTime.AddDays(0.58333);

            ephemeris.Epoch = 2460200.5;
            ephemeris.a_SemiMajorAxis = 2.7672543;
            ephemeris.e_OrbitalEccentricity = 0.0789126;
            ephemeris.i_Inclination = 10.58688;
            ephemeris.w_PerihelionArgument = 73.42179;
            ephemeris.Node = 80.25496;
            ephemeris.M_MeanAnomalyAtEpoch = 60.07881;

            SolarSystemBody asteroid = new SolarSystemBody();
            asteroid.Ephemeris = ephemeris;
            output.WriteLine(
                $"  Epoch: {asteroid.Ephemeris.Epoch} ({AstroUtilities.JulianDateToDateTime(asteroid.Ephemeris.Epoch)}) , \r\n" +
                $"a Semi-major axis: {asteroid.Ephemeris.a_SemiMajorAxis}, \r\n" +
                $"e Orbit eccentricity: {asteroid.Ephemeris.e_OrbitalEccentricity}, \r\n" +
                $"i Inclination: {asteroid.Ephemeris.i_Inclination}, \r\n" +
                $"w Perihelion argument: {asteroid.Ephemeris.w_PerihelionArgument}, \r\n" +
                $"  Node: {asteroid.Ephemeris.Node}, \r\n" +
                $"M Mean anomaly at epoch: {asteroid.Ephemeris.M_MeanAnomalyAtEpoch}, \r\n" +
                $"n Mean daily motion: {asteroid.Ephemeris.n_MeanDailyMotion}, \r\n" +
                $"");

            DateTime targetTime = new DateTime(2023, 1, 8, 05, 22, 12, DateTimeKind.Utc);

            Coordinates asteroidCoordinates = asteroid.AstrometricCoordinates(targetTime);

            output.WriteLine($"Asteroid Name: {asteroid.Name}");
            output.WriteLine($"Asteroid RA on {targetTime} {targetTime.Kind} - {Utilities.HoursToHMS(asteroidCoordinates.RightAscension)} ({asteroidCoordinates.RightAscension}), Declination: {Utilities.DegreesToDMS(asteroidCoordinates.Declination)} ({asteroidCoordinates.Declination})");

            output.WriteLine($"Test tolerance: {Utilities.DegreesToDMS(TEST_TOLERANCE, ":", ":", "", 2)} Degrees, Minutes, Seconds");

            Assert.Equal(12.592041505741712, asteroidCoordinates.RightAscension, TEST_TOLERANCE / 15.0);
            Assert.Equal(9.761320088397163, asteroidCoordinates.Declination, TEST_TOLERANCE);
            Assert.Equal("Ceres", asteroid.Name);
        }

        [Fact]
        public void MpcCometOrbit()
        {
            const double TEST_TOLERANCE = 0.0003; // Degrees

            SolarSystemBody comet = new SolarSystemBody(SolarSystemBody.OrbitDataSource.MpcCometOrbit, "    CK23V010  2025 07 13.1576  5.092857  0.999309  103.3112   15.0240  102.0074  20231106   8.5  4.0  C/2023 V1 (Lemmon)                                       MPEC 2023-V23");
            DateTime targetTime = new DateTime(2023, 11, 6, 22, 00, 00, DateTimeKind.Utc);

            output.WriteLine(
                $"  Epoch: {comet.Ephemeris.Epoch} ({AstroUtilities.JulianDateToDateTime(comet.Ephemeris.Epoch)}) , \r\n" +
                $"q Perihelion distance: {comet.Ephemeris.q_PerihelionDistance}, \r\n" +
                $"e Orbit eccentricity: {comet.Ephemeris.e_OrbitalEccentricity}, \r\n" +
                $"i Inclination: {comet.Ephemeris.i_Inclination}, \r\n" +
                $"w Perihelion argument: {comet.Ephemeris.w_PerihelionArgument}, \r\n" +
                $"  Node: {comet.Ephemeris.Node}, \r\n" +
                $"");

            Coordinates cometCoordinates = comet.AstrometricCoordinates(targetTime);

            output.WriteLine($"Comet Name: {comet.Name}");
            output.WriteLine($"Comet RA on {targetTime} {targetTime.Kind} - {Utilities.HoursToHMS(cometCoordinates.RightAscension)} ({cometCoordinates.RightAscension}), Declination: {Utilities.DegreesToDMS(cometCoordinates.Declination)} ({cometCoordinates.Declination})");
            output.WriteLine($"Test tolerance: {Utilities.DegreesToDMS(TEST_TOLERANCE, ":", ":", "", 2)} Degrees, Minutes, Seconds");

            Assert.Equal(22.29345332973845, cometCoordinates.RightAscension, TEST_TOLERANCE);
            Assert.Equal(39.72864756380414, cometCoordinates.Declination, TEST_TOLERANCE);
            Assert.Equal("C/2023 V1", comet.Name);
        }

        [Fact]
        public void MpcAsteroidOrbit()
        {
            const double TEST_TOLERANCE = 0.0015; // Degrees
            SolarSystemBody asteroid = new SolarSystemBody(SolarSystemBody.OrbitDataSource.MpcAsteroidOrbit, "00001    3.33  0.15 K239D  60.07881   73.42179   80.25496   10.58688  0.0789126  0.21410680   2.7672543  0 E2023-F87  7283 123 1801-2023 0.65 M-v 30k MPCLINUX   0000      (1) Ceres              20230321");
            output.WriteLine(
                $"  Epoch: {asteroid.Ephemeris.Epoch} ({AstroUtilities.JulianDateToDateTime(asteroid.Ephemeris.Epoch)}) , \r\n" +
                $"a Semi-major axis: {asteroid.Ephemeris.a_SemiMajorAxis}, \r\n" +
                $"e Orbit eccentricity: {asteroid.Ephemeris.e_OrbitalEccentricity}, \r\n" +
                $"i Inclination: {asteroid.Ephemeris.i_Inclination}, \r\n" +
                $"w Perihelion argument: {asteroid.Ephemeris.w_PerihelionArgument}, \r\n" +
                $"  Node: {asteroid.Ephemeris.Node}, \r\n" +
                $"M Mean anomaly at epoch: {asteroid.Ephemeris.M_MeanAnomalyAtEpoch}, \r\n" +
                $"n Mean daily motion: {asteroid.Ephemeris.n_MeanDailyMotion}, \r\n" +
                $"");

            asteroid.SiteLatitude = Utilities.DMSToDegrees("51:04:43"); ;
            asteroid.SiteLongitude = -Utilities.DMSToDegrees("00:17:40");
            asteroid.SiteHeight = 80;
            output.WriteLine($"Observing site: Latitude: {asteroid.SiteLatitude}, Longitude: {asteroid.SiteLongitude}");

            DateTime targetTime = new DateTime(2023, 11, 9, 11, 00, 00, DateTimeKind.Utc);

            Coordinates asteroidCoordinates = asteroid.AstrometricCoordinates(targetTime);

            output.WriteLine($"Asteroid Name: {asteroid.Name}");
            output.WriteLine($"Asteroid RA on {targetTime} {targetTime.Kind} - {asteroidCoordinates.RightAscension.ToHMS()} ({asteroidCoordinates.RightAscension}), Declination: {asteroidCoordinates.Declination.ToDMS()} ({asteroidCoordinates.Declination})");

            output.WriteLine($"Test tolerance: {Utilities.DegreesToDMS(TEST_TOLERANCE, ":", ":", "", 2)} Degrees, Minutes, Seconds");

            Assert.Equal(15.42613095698802, asteroidCoordinates.RightAscension, TEST_TOLERANCE / 15.0);
            Assert.Equal(-15.294339760927821, asteroidCoordinates.Declination, TEST_TOLERANCE);
            Assert.Equal("(1) Ceres", asteroid.Name);
        }

        [Fact]
        public void MpcAsteroidTopoOrbit()
        {
            const double TEST_TOLERANCE = 0.0015; // Degrees
            //00001    3.33  0.15 K239D  60.07881   73.42179   80.25496   10.58688  0.0789126  0.21410680   2.7672543  0 E2023-F87  7283 123 1801-2023 0.65 M-v 30k MPC        0000              (1) Ceres      20230321
            //00001    3.33  0.15 K239D  60.07881   73.42179   80.25496   10.58688  0.0789126  0.21410680   2.7672543  0 E2023-F87  7283 123 1801-2023 0.65 M-v 30k MPC        0000              (1) Ceres      20230321
            //00001    3.33  0.15 K239D  60.07881   73.42179   80.25496   10.58688  0.0789126  0.21410680   2.7672543  0 E2023-F87  7283 123 1801-2023 0.65 M-v 30k MPCLINUX   0000      (1) Ceres              20230321
            SolarSystemBody asteroid = new SolarSystemBody(SolarSystemBody.OrbitDataSource.MpcAsteroidOrbit, "00001    3.33  0.15 K239D  60.07881   73.42179   80.25496   10.58688  0.0789126  0.21410680   2.7672543  0 E2023-F87  7283 123 1801-2023 0.65 M-v 30k MPCLINUX   0000      (1) Ceres              20230321");
            output.WriteLine(
                $"  Epoch: {asteroid.Ephemeris.Epoch} ({AstroUtilities.JulianDateToDateTime(asteroid.Ephemeris.Epoch)}) , \r\n" +
                $"a Semi-major axis: {asteroid.Ephemeris.a_SemiMajorAxis}, \r\n" +
                $"e Orbit eccentricity: {asteroid.Ephemeris.e_OrbitalEccentricity}, \r\n" +
                $"i Inclination: {asteroid.Ephemeris.i_Inclination}, \r\n" +
                $"w Perihelion argument: {asteroid.Ephemeris.w_PerihelionArgument}, \r\n" +
                $"  Node: {asteroid.Ephemeris.Node}, \r\n" +
                $"M Mean anomaly at epoch: {asteroid.Ephemeris.M_MeanAnomalyAtEpoch}, \r\n" +
                $"n Mean daily motion: {asteroid.Ephemeris.n_MeanDailyMotion}, \r\n" +
                $"");

            asteroid.SiteLatitude = Utilities.DMSToDegrees("51:04:43"); ;
            asteroid.SiteLongitude = -Utilities.DMSToDegrees("00:17:40");
            asteroid.SiteHeight = 80;
            output.WriteLine($"Observing site: Latitude: {asteroid.SiteLatitude}, Longitude: {asteroid.SiteLongitude}");

            DateTime targetTime = new DateTime(2023, 11, 8, 14, 00, 00, DateTimeKind.Utc);

            double epochJulianDay = AstroUtilities.JulianDateFromDateTime(targetTime);
            output.WriteLine($"Target time Julian day number: {epochJulianDay}");

            Coordinates asteroidCoordinates = asteroid.TopocentricCoordinates(targetTime);

            output.WriteLine($"Asteroid Name: {asteroid.Name}");
            output.WriteLine($"Asteroid RA on {targetTime} {targetTime.Kind} - {asteroidCoordinates.RightAscension.ToHMS()} ({asteroidCoordinates.RightAscension}), Declination: {asteroidCoordinates.Declination.ToDMS()} ({asteroidCoordinates.Declination})");

            output.WriteLine($"Test tolerance: {Utilities.DegreesToDMS(TEST_TOLERANCE, ":", ":", "", 2)} Degrees, Minutes, Seconds");

            Assert.Equal(15.423141062080871, asteroidCoordinates.RightAscension, TEST_TOLERANCE / 15.0);
            Assert.Equal(-15.259376380692105, asteroidCoordinates.Declination, TEST_TOLERANCE);
            Assert.Equal("(1) Ceres", asteroid.Name);

            Transform transform = new Transform()
            {
                SiteLatitude = Utilities.DMSToDegrees("51:04:43"),
                SiteLongitude = -Utilities.DMSToDegrees("00:17:40"),
                SiteElevation = 80,
                SitePressure = 1010,
                SiteTemperature = 10.0
            };

            asteroidCoordinates = asteroid.AstrometricCoordinates(targetTime);
            output.WriteLine($"Astrometric RA on {targetTime} {targetTime.Kind} - {asteroidCoordinates.RightAscension.ToHMS()} ({asteroidCoordinates.RightAscension}), Declination: {asteroidCoordinates.Declination.ToDMS()} ({asteroidCoordinates.Declination})");

            transform.JulianDateUTC = epochJulianDay;
            transform.SetJ2000(asteroidCoordinates.RightAscension, asteroidCoordinates.Declination);
            output.WriteLine($"Transform RA on {targetTime} {targetTime.Kind} - {transform.RATopocentric.ToHMS()} ({transform.RATopocentric}), Declination: {transform.DECTopocentric.ToDMS()} ({transform.DECTopocentric})");




        }

        [Fact]
        public void JPLCometOrbit()
        {
            const double TEST_TOLERANCE = 0.0003; // Degrees

            //SolarSystemBody comet = new SolarSystemBody(SolarSystemBody.OrbitData.JplCometOrbit, "   C/2023 H2 (Lemmon)                         60189  0.89441501 0.99633108 113.75399 150.64913 217.04455 20231029.18992 JPL 18");
            SolarSystemBody comet = new SolarSystemBody(SolarSystemBody.OrbitDataSource.JplCometOrbit, "   C/2023 V1 (Lemmon)                         60215  5.09304502 0.99918231 102.00676 103.30969  15.02344 20250713.18186 JPL 1");

            DateTime targetTime = new DateTime(2023, 11, 6, 22, 00, 00, DateTimeKind.Utc);

            output.WriteLine(
                $"  Epoch: {comet.Ephemeris.Epoch} ({AstroUtilities.JulianDateToDateTime(comet.Ephemeris.Epoch)}) , \r\n" +
                $"q Perihelion distance: {comet.Ephemeris.q_PerihelionDistance}, \r\n" +
                $"e Orbit eccentricity: {comet.Ephemeris.e_OrbitalEccentricity}, \r\n" +
                $"i Inclination: {comet.Ephemeris.i_Inclination}, \r\n" +
                $"w Perihelion argument: {comet.Ephemeris.w_PerihelionArgument}, \r\n" +
                $"  Node: {comet.Ephemeris.Node}, \r\n" +
                $"");

            Coordinates cometCoordinates = comet.AstrometricCoordinates(targetTime);

            output.WriteLine($"Comet Name: {comet.Name}");
            output.WriteLine($"Comet RA on {targetTime} {targetTime.Kind} - {Utilities.HoursToHMS(cometCoordinates.RightAscension)} ({cometCoordinates.RightAscension}), Declination: {Utilities.DegreesToDMS(cometCoordinates.Declination)} ({cometCoordinates.Declination})");
            output.WriteLine($"Test tolerance: {Utilities.DegreesToDMS(TEST_TOLERANCE, ":", ":", "", 2)} Degrees, Minutes, Seconds");

            Assert.Equal(22.29345332973845, cometCoordinates.RightAscension, TEST_TOLERANCE);
            Assert.Equal(39.72864756380414, cometCoordinates.Declination, TEST_TOLERANCE);
            Assert.Equal("C/2023 V1", comet.Name);
        }

        [Fact]
        public void JPLNumberedAsteroidOrbit()
        {
            const double TEST_TOLERANCE = 0.0015; // Degrees
            SolarSystemBody asteroid = new SolarSystemBody(SolarSystemBody.OrbitDataSource.JplNumberedAsteroidOrbit, "     1 Ceres             60200  2.7672544 0.07891253  10.58688  73.42180  80.25498  60.0787728  3.33  0.12 JPL 48");

            DateTime targetTime = new DateTime(2023, 1, 8, 05, 22, 12, DateTimeKind.Utc);

            Coordinates asteroidCoordinates = asteroid.AstrometricCoordinates(targetTime);

            output.WriteLine($"Asteroid Name: {asteroid.Name}");
            output.WriteLine($"Asteroid RA on {targetTime} {targetTime.Kind} - {Utilities.HoursToHMS(asteroidCoordinates.RightAscension)} ({asteroidCoordinates.RightAscension}), Declination: {Utilities.DegreesToDMS(asteroidCoordinates.Declination)} ({asteroidCoordinates.Declination})");

            output.WriteLine($"Test tolerance: {Utilities.DegreesToDMS(TEST_TOLERANCE, ":", ":", "", 2)} Degrees, Minutes, Seconds");

            Assert.Equal(12.592041505741712, asteroidCoordinates.RightAscension, TEST_TOLERANCE / 15.0);
            Assert.Equal(9.761320088397163, asteroidCoordinates.Declination, TEST_TOLERANCE);
            Assert.Equal("1 Ceres", asteroid.Name);
        }

        [Fact]
        public void JPLUnNumberedAsteroidOrbit()
        {
            const double TEST_TOLERANCE = 0.0015; // Degrees
            SolarSystemBody asteroid = new SolarSystemBody(SolarSystemBody.OrbitDataSource.JplUnNumberedAsteroidOrbit, "  1993 BC16   60200   5.2120059 0.13592842  17.34267 226.41108 132.56253 333.9285365 13.13 0.15 JPL 24");
            output.WriteLine(
                $"  Epoch: {asteroid.Ephemeris.Epoch} ({AstroUtilities.JulianDateToDateTime(asteroid.Ephemeris.Epoch)}) , \r\n" +
                $"a Semi-major axis: {asteroid.Ephemeris.a_SemiMajorAxis}, \r\n" +
                $"e Orbit eccentricity: {asteroid.Ephemeris.e_OrbitalEccentricity}, \r\n" +
                $"i Inclination: {asteroid.Ephemeris.i_Inclination}, \r\n" +
                $"w Perihelion argument: {asteroid.Ephemeris.w_PerihelionArgument}, \r\n" +
                $"  Node: {asteroid.Ephemeris.Node}, \r\n" +
                $"M Mean anomaly at epoch: {asteroid.Ephemeris.M_MeanAnomalyAtEpoch}, \r\n" +
                $"n Mean daily motion: {asteroid.Ephemeris.n_MeanDailyMotion}, \r\n" +
                $"");

            DateTime targetTime = new DateTime(2023, 11, 8, 11, 00, 00, DateTimeKind.Utc);

            Coordinates asteroidCoordinates = asteroid.AstrometricCoordinates(targetTime);

            output.WriteLine($"Asteroid Name: {asteroid.Name}");
            output.WriteLine($"Asteroid RA on {targetTime} {targetTime.Kind} - {asteroidCoordinates.RightAscension.ToHMS()} ({asteroidCoordinates.RightAscension}), Declination: {asteroidCoordinates.Declination.ToDMS()} ({asteroidCoordinates.Declination})");

            output.WriteLine($"Test tolerance: {Utilities.DegreesToDMS(TEST_TOLERANCE, ":", ":", "", 2)} Degrees, Minutes, Seconds");

            Assert.Equal(21.449879628691946, asteroidCoordinates.RightAscension, TEST_TOLERANCE / 15.0);
            Assert.Equal(-20.861027262388767, asteroidCoordinates.Declination, TEST_TOLERANCE);
            Assert.Equal("1993 BC16", asteroid.Name);
        }

        [Fact]
        public void LowellAsteroidOrbit()
        {
            const double TEST_TOLERANCE = 0.0015; // Degrees
            SolarSystemBody asteroid = new SolarSystemBody(SolarSystemBody.OrbitDataSource.LowellAsteroid, "     1 Ceres              L.H. Wasserman   3.33  0.15 0.72 848.4 G?      0   0   0   0   0   0 81218 6833 20231222  81.513198  73.391274  80.253704 10.587314 0.07897659   2.76719926 20230804 1.1E-02 -2.8E-06 20231107 2.3E-02 20240707 2.7E-02 20320228 2.7E-02 20320228");
            output.WriteLine(
                $"  Epoch: {asteroid.Ephemeris.Epoch} ({AstroUtilities.JulianDateToDateTime(asteroid.Ephemeris.Epoch)}) , \r\n" +
                $"a Semi-major axis: {asteroid.Ephemeris.a_SemiMajorAxis}, \r\n" +
                $"e Orbit eccentricity: {asteroid.Ephemeris.e_OrbitalEccentricity}, \r\n" +
                $"i Inclination: {asteroid.Ephemeris.i_Inclination}, \r\n" +
                $"w Perihelion argument: {asteroid.Ephemeris.w_PerihelionArgument}, \r\n" +
                $"  Node: {asteroid.Ephemeris.Node}, \r\n" +
                $"M Mean anomaly at epoch: {asteroid.Ephemeris.M_MeanAnomalyAtEpoch}, \r\n" +
                $"n Mean daily motion: {asteroid.Ephemeris.n_MeanDailyMotion}, \r\n" +
                $"");

            DateTime targetTime = new DateTime(2023, 11, 8, 10, 00, 00, DateTimeKind.Utc);

            Coordinates asteroidCoordinates = asteroid.AstrometricCoordinates(targetTime);

            output.WriteLine($"Asteroid Name: {asteroid.Name}");
            output.WriteLine($"Asteroid RA on {targetTime} {targetTime.Kind} - {asteroidCoordinates.RightAscension.ToHMS()} ({asteroidCoordinates.RightAscension}), Declination: {asteroidCoordinates.Declination.ToDMS()} ({asteroidCoordinates.Declination})");

            output.WriteLine($"Test tolerance: {Utilities.DegreesToDMS(TEST_TOLERANCE, ":", ":", "", 2)} Degrees, Minutes, Seconds");

            Assert.Equal(15.396740150207924, asteroidCoordinates.RightAscension, TEST_TOLERANCE / 15.0);
            Assert.Equal(-15.153844052217968, asteroidCoordinates.Declination, TEST_TOLERANCE);
            Assert.Equal("Ceres", asteroid.Name);
        }

        [Fact]
        public void Position()
        {
            DateTime dateTime = DateTime.Parse("2023-11-09T12:00:00.0000000Z", CultureInfo.CreateSpecificCulture("en-uk"), DateTimeStyles.AssumeUniversal);
            DateTime dateTimeUtc = dateTime.ToUniversalTime();
            SolarSystemBody venus = new SolarSystemBody(Body.Venus);

            BodyPositionVelocity venusPV = venus.HelioCentricPosition(dateTimeUtc);
            output.WriteLine($"Sun - Venus distance on {dateTimeUtc} {dateTimeUtc.Kind} - {venusPV.Distance} AU, {venusPV.X} {venusPV.Y} {venusPV.Z} {venusPV.VelocityX} {venusPV.VelocityY} {venusPV.VelocityZ}");
            Assert.Equal(0.7191297389456842, venusPV.Distance, 0.0000001);

            SolarSystemBody earth = new SolarSystemBody(Body.Earth);

            BodyPositionVelocity earthPV = earth.HelioCentricPosition(dateTimeUtc);
            output.WriteLine($"Sun - Earth distance on {dateTimeUtc} {dateTimeUtc.Kind} - {earthPV.Distance} AU, {earthPV.X} {earthPV.Y} {earthPV.Z} {venusPV.VelocityX} {venusPV.VelocityY} {earthPV.VelocityZ}");
            Assert.Equal(0.9906241598812772, earthPV.Distance, 0.0000001);

            double earthVenusDistance = Math.Sqrt(
                (venusPV.X - earthPV.X) * (venusPV.X - earthPV.X) +
                (venusPV.Y - earthPV.Y) * (venusPV.Y - earthPV.Y) +
                (venusPV.Z - earthPV.Z) * (venusPV.Z - earthPV.Z)
                );

            output.WriteLine($"Venus - Earth    distance on {dateTimeUtc} {dateTimeUtc.Kind} - {earthVenusDistance}");
            Assert.Equal(0.818367547890153, earthVenusDistance, 0.0000001);
            
            BodyPositionVelocity venusToEarth = venus.GeoCentricPosition(dateTimeUtc);
            output.WriteLine($"Venus - Earth v2 distance on {dateTimeUtc} {dateTimeUtc.Kind} - {venusToEarth.Distance}");

            Assert.Equal(venusToEarth.Distance, earthVenusDistance, 0.0000001);
        }

        [Fact]
        public void VenusAstroUtc()
        {
            DateTime dateTime = DateTime.Parse("2023-07-01T12:00:00.0000000Z", CultureInfo.CreateSpecificCulture("en-uk"), DateTimeStyles.AssumeUniversal);
            DateTime dateTimeUtc = dateTime.ToUniversalTime();
            SolarSystemBody venus = new SolarSystemBody(Body.Venus);

            Coordinates coordinates = venus.AstrometricCoordinates(dateTimeUtc);
            output.WriteLine($"Venus Astrometric on {dateTimeUtc} {dateTimeUtc.Kind} - {coordinates}");
            output.WriteLine($"RA: {coordinates.RightAscension} Declination: {coordinates.Declination}");

            Assert.InRange(coordinates.RightAscension, 9.5526, 9.5527);
            Assert.InRange(coordinates.Declination, 14.9330, 14.9340);
            Assert.InRange(coordinates.Distance, 0.49, 0.50);
        }

        [Fact]
        public void VenusAstroLocal()
        {
            SolarSystemBody venus = new SolarSystemBody(Body.Venus);
            DateTime dateTime = DateTime.Parse("2023-07-01T12:00:00.0000000Z");

            Coordinates coordinates = venus.AstrometricCoordinates(dateTime);
            output.WriteLine($"Venus Astrometric on {dateTime} {dateTime.Kind} - {coordinates}");
            output.WriteLine($"RA: {coordinates.RightAscension} Declination: {coordinates.Declination}");

            Assert.InRange(coordinates.RightAscension, 9.5526, 9.5527);
            Assert.InRange(coordinates.Declination, 14.9330, 14.9340);
            Assert.InRange(coordinates.Distance, 0.49, 0.50);
        }

        [Fact]
        public void VenusTopo()
        {
            SolarSystemBody body = new SolarSystemBody(Body.Venus)
            {
                SiteLatitude = Utilities.DMSToDegrees("51:04:43"),
                SiteLongitude = -Utilities.DMSToDegrees("00:17:40"),
                SiteHeight = 80,
            };

            DateTime dateTime = DateTime.Parse("2023-07-01T12:00:00.0000000Z");

            Coordinates coordinates = body.TopocentricCoordinates(dateTime);
            output.WriteLine($"Venus Topocentric on {dateTime} {dateTime.Kind} - {coordinates}");
            output.WriteLine($"Topocentric RA: {coordinates.RightAscension} Declination: {coordinates.Declination}, Distance: {coordinates.Distance}");

            Assert.Equal(9.573884818285455, coordinates.RightAscension, 5);
            Assert.Equal(14.8282996899911, coordinates.Declination, 4);
            Assert.Equal(0.49489073528931743, coordinates.Distance, 4);
        }

        [Fact]
        public void VenusAltAzNoRefract()
        {
            SolarSystemBody body = new SolarSystemBody(Body.Venus)
            {
                SiteLatitude = Utilities.DMSToDegrees("51:04:43"),
                SiteLongitude = -Utilities.DMSToDegrees("00:17:40"),
                SiteHeight = 80,
            };

            DateTime dateTime = DateTime.Parse("2023-07-01T12:00:00.0000000Z");

            Coordinates coordinates = body.AltAzCoordinates(dateTime);
            output.WriteLine($"Venus AltAz on {dateTime} {dateTime.Kind} - {coordinates}");
            output.WriteLine($"Topocentric RA - No refraction: {coordinates.RightAscension} Declination: {coordinates.Declination}, Distance: {coordinates.Distance}");
            output.WriteLine($"Topocentric Azimuth: {coordinates.Azimuth} Altitude: {coordinates.Altitude}");

            Assert.Equal(9.573884818285455, coordinates.RightAscension, 5);
            Assert.Equal(14.8282996899911, coordinates.Declination, 4);
            Assert.Equal(0.49489073528931743, coordinates.Distance, 4);
            Assert.Equal(118.88283590995069, coordinates.Azimuth, 4);
            Assert.Equal(39.154075401896634, coordinates.Altitude, 4);
        }

        [Fact]
        public void VenusAltAzWithRefract()
        {
            SolarSystemBody body = new SolarSystemBody(Body.Venus)
            {
                SiteLatitude = Utilities.DMSToDegrees("51:04:43"),
                SiteLongitude = -Utilities.DMSToDegrees("00:17:40"),
                SiteHeight = 80,
            };
            body.Refraction = true;

            DateTime dateTime = DateTime.Parse("2023-07-01T12:00:00.0000000Z");

            Coordinates coordinates = body.AltAzCoordinates(dateTime);
            output.WriteLine($"Venus AltAz on {dateTime} {dateTime.Kind} - {coordinates}");
            output.WriteLine($"Topocentric RA - With refraction: {coordinates.RightAscension} Declination: {coordinates.Declination}, Distance: {coordinates.Distance}");
            output.WriteLine($"Topocentric Azimuth: {coordinates.Azimuth} Altitude: {coordinates.Altitude}");

            Assert.Equal(9.573087390287036, coordinates.RightAscension, 5);
            Assert.Equal(14.845006995447209, coordinates.Declination, 4);
            Assert.Equal(0.49489073528931743, coordinates.Distance, 4);
            Assert.Equal(118.88283590995069, coordinates.Azimuth, 4);
            Assert.Equal(39.174393578932836, coordinates.Altitude, 4);
        }

        [Fact]
        public void JupiterAstro()
        {
            SolarSystemBody jupiter = new SolarSystemBody(Body.Jupiter);
            DateTime dateTime = DateTime.Parse("2023-07-01T12:00:00.0000000Z");

            Coordinates coordinates = jupiter.AstrometricCoordinates(dateTime);
            output.WriteLine($"Jupiter Astrometric on {dateTime} {dateTime.Kind} - {coordinates}");
            output.WriteLine($"Astro RA: {coordinates.RightAscension} Declination: {coordinates.Declination}");

            Assert.InRange(coordinates.RightAscension, 2.4686, 2.4687);
            Assert.InRange(coordinates.Declination, 13.4311, 13.4312);
            Assert.InRange(coordinates.Distance, 5.386, 5.388);
        }

        [Fact]
        public void JupiterTopo()
        {
            SolarSystemBody jupiter = new SolarSystemBody(Body.Jupiter)
            {
                SiteLatitude = Utilities.DMSToDegrees("51:04:43"),
                SiteLongitude = -Utilities.DMSToDegrees("00:17:40"),
                SiteHeight = 80,
            };

            DateTime dateTime = DateTime.Parse("2023-07-01T12:00:00.0000000Z");

            Coordinates coordinates = jupiter.TopocentricCoordinates(dateTime);
            output.WriteLine($"Jupiter Topocentric on {dateTime} {dateTime.Kind} - {coordinates}");
            output.WriteLine($"Topocentric RA: {coordinates.RightAscension} Declination: {coordinates.Declination}, Distance: {coordinates.Distance}");

            Assert.Equal(2.4896114162734233, coordinates.RightAscension, 5);
            Assert.Equal(13.534710967271877, coordinates.Declination, 4);
            Assert.Equal(5.387154538525765, coordinates.Distance, 4);
        }

        [Fact]
        public void MoonAstro()
        {
            SolarSystemBody body = new SolarSystemBody(Body.Moon)
            {
                SiteLatitude = Utilities.DMSToDegrees("51:04:43"),
                SiteLongitude = -Utilities.DMSToDegrees("00:17:40"),
                SiteHeight = 80,
            };

            DateTime dateTime = DateTime.Parse("2023-07-01T12:00:00.0000000Z");

            Coordinates coordinates = body.AstrometricCoordinates(dateTime);
            output.WriteLine($"Moon Astrometric on {dateTime} {dateTime.Kind} - {coordinates}");
            output.WriteLine($"Astrometric RA: {coordinates.RightAscension} Declination: {coordinates.Declination}, Distance: {coordinates.Distance}");

            Assert.Equal(16.664758149288403, coordinates.RightAscension, 5);
            Assert.Equal(-25.478169289074486, coordinates.Declination, 4);
            Assert.Equal(0.00247099882382454, coordinates.Distance, 4);
        }

        [Fact]
        public void MoonTopo()
        {
            SolarSystemBody body = new SolarSystemBody(Body.Moon)
            {
                SiteLatitude = Utilities.DMSToDegrees("51:04:43"),
                SiteLongitude = -Utilities.DMSToDegrees("00:17:40"),
                SiteHeight = 80,
            };

            DateTime dateTime = DateTime.Parse("2023-07-01T12:00:00.0000000Z");

            Coordinates coordinates = body.TopocentricCoordinates(dateTime);
            output.WriteLine($"Moon Topocentric on {dateTime} {dateTime.Kind} - {coordinates}");
            output.WriteLine($"Topocentric RA: {coordinates.RightAscension} Declination: {coordinates.Declination}, Distance: {coordinates.Distance}");

            Assert.Equal(16.710725888900686, coordinates.RightAscension, 5);
            Assert.Equal(-25.973636895902157, coordinates.Declination, 4);
            Assert.Equal(0.0025065896894229636, coordinates.Distance, 4);
        }

        [Fact]
        public void EarthInit()
        {
            Assert.Throws<ASCOM.InvalidOperationException>(() =>
            {
                SolarSystemBody body = new SolarSystemBody(Body.Earth)
                {
                    SiteLatitude = Utilities.DMSToDegrees("51:04:43"),
                    SiteLongitude = -Utilities.DMSToDegrees("00:17:40"),
                    SiteHeight = 80,
                };

                body.AstrometricCoordinates(DateTime.Now);
            });
        }

        [Fact]
        public void PlutoAstro()
        {
            SolarSystemBody body = new SolarSystemBody(Body.Pluto)
            {
                SiteLatitude = Utilities.DMSToDegrees("51:04:43"),
                SiteLongitude = -Utilities.DMSToDegrees("00:17:40"),
                SiteHeight = 80,
            };

            DateTime dateTime = DateTime.Parse("2023-07-01T12:00:00.0000000Z");

            Coordinates coordinates = body.AstrometricCoordinates(dateTime);
            output.WriteLine($"Pluto Astrometric on {dateTime} {dateTime.Kind} - {coordinates}");
            output.WriteLine($"Astrometric RA: {coordinates.RightAscension} Declination: {coordinates.Declination}, Distance: {coordinates.Distance}");

            Assert.Equal(20.1346496828432, coordinates.RightAscension, 5);
            Assert.Equal(-22.890896430045007, coordinates.Declination, 4);
            Assert.Equal(33.84422968714076, coordinates.Distance, 4);
        }

        [Fact]
        public void PlutoTopo()
        {
            SolarSystemBody body = new SolarSystemBody(Body.Pluto)
            {
                SiteLatitude = Utilities.DMSToDegrees("51:04:43"),
                SiteLongitude = -Utilities.DMSToDegrees("00:17:40"),
                SiteHeight = 80,
            };

            DateTime dateTime = DateTime.Parse("2023-07-01T12:00:00.0000000Z");

            Coordinates coordinates = body.TopocentricCoordinates(dateTime);
            output.WriteLine($"Pluto Topocentric on {dateTime} {dateTime.Kind} - {coordinates}");
            output.WriteLine($"Topocentric RA: {coordinates.RightAscension} Declination: {coordinates.Declination}, Distance: {coordinates.Distance}");

            Assert.Equal(20.15806012993297, coordinates.RightAscension, 5);
            Assert.Equal(-22.82224238337633, coordinates.Declination, 4);
            Assert.Equal(33.84426520210103, coordinates.Distance, 4);
        }






    }
}
