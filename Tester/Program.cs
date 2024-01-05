using ASCOM.Tools.Novas31;
using ASCOM.Tools;
using System.Globalization;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using ASCOM.Tools.NovasCom;

namespace Tester
{
    internal class Program
    {
        const double AU2KM = 149597870.7;
        static TraceLogger? logger;
        static void Main(string[] args)
        {
            try
            {
                logger = new("LibraryTester", true);

                DateTime dateTime = DateTime.Parse("2023-07-01T12:00:00.0000000Z", CultureInfo.CreateSpecificCulture("en-uk"), DateTimeStyles.AssumeUniversal);
                DateTime dateTimeUtc = dateTime.ToUniversalTime();
                SolarSystemBody venus = new SolarSystemBody(Body.Venus);

                BodyPositionVelocity bodyPV = venus.HelioCentricPosition(dateTimeUtc);
                Console.WriteLine($"Sun distance on {dateTimeUtc} {dateTimeUtc.Kind} - {bodyPV.Distance} AU, {bodyPV.X} {bodyPV.Y} {bodyPV.Z} {bodyPV.VelocityX} {bodyPV.VelocityY} {bodyPV.VelocityZ}");

                TestKeplerComet();




            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.ReadKey();

        }

        static void TestKeplerComet()
        {
            // Define the comet to be examined
            //string text = "    CK22U020  2023 01 14.2204  1.328037  0.986161  147.9085  304.4758   48.2504  20230113  16.0  4.0  C / 2022 U2(ATLAS)                                        MPEC 2023 - A16";
            DateTime targetTime = new DateTime(2023, 1, 8, 14, 22, 12, DateTimeKind.Utc);

            string text = "    CK23H020  2023 10 29.1890  0.894406  0.996374  150.6494  217.0444  113.7538  20231031  14.0  4.0  C/2023 H2 (Lemmon)                                       MPEC 2023-UR6";
            //DateTime targetTime = DateTime.UtcNow;


            // Set test parameters
            double targetJd = AstroUtilities.JulianDateFromDateTime(targetTime);
            double targetJdTT = targetJd + AstroUtilities.DeltaT(targetJd) / 86400;

            LogMessage($"Target time is {targetTime}, JD = {targetJd:f5}, JDTT = {targetJdTT:f5}");



            // Extract orbital elements from the reference text line
            OrbitalElements elements = new OrbitalElements(text);
            LogMessage($"Orbital elements:\r\n{elements}\r\n");

            Ephemeris kt = new Ephemeris();

            kt.BodyType = BodyType.Comet;

            kt.Name = elements.Name;


            kt.Epoch = AstroUtilities.JulianDateFromDateTime(elements.PerihelionPassage);

            kt.e_OrbitalEccentricity = elements.OrbitalEccentricity;
            //kt.G = 0;
            //kt.H = 0;
            kt.M_MeanAnomalyAtEpoch = 0;
            kt.n_MeanDailyMotion = 0;
            kt.w_PerihelionArgument = elements.ArgOfPerihelion;
            kt.Node = elements.LongitudeOfAscNode;
            kt.i_Inclination = elements.Inclination;
            kt.q_PerihelionDistance = elements.PeriDistance;

            // Extra code to set the semi-major axis
            //if (semiMajorAxis != 0.0) kt.a = semiMajorAxis;
            LogMessage($"Perihelion distance: {kt.q_PerihelionDistance} - Semi-major axis: {kt.a_SemiMajorAxis}");

            double[] cometPv = kt.GetPositionAndVelocity(targetJdTT);

            double cometSunDistance = Math.Sqrt(cometPv[0] * cometPv[0] + cometPv[1] * cometPv[1] + cometPv[2] * cometPv[2]);
            LogMessage($"Comet-Sun distance: {cometSunDistance} AU = {cometSunDistance * AU2KM:0}km");

            SolarSystemBody earth = new(Body.Earth);

            BodyPositionVelocity earthPv = earth.HelioCentricPosition(targetTime);
            double earthSunDistance = Math.Sqrt(earthPv.X * earthPv.X + earthPv.Y * earthPv.Y + earthPv.Z * earthPv.Z);
            LogMessage($"Earth-Sun distance: {earthSunDistance}");

            double cometEarthDistance = Math.Sqrt((cometPv[0] - earthPv.X) * (cometPv[0] - earthPv.X) + (cometPv[1] - earthPv.Y) * (cometPv[1] - earthPv.Y) + (cometPv[2] - earthPv.Z) * (cometPv[2] - earthPv.Z));

            LogMessage($"Earth-comet distance: {cometEarthDistance} AU = {cometEarthDistance * AU2KM:0}km");
            double[] earthCometVector = new double[3];
            earthCometVector[0] = cometPv[0] - earthPv.X;
            earthCometVector[1] = cometPv[1] - earthPv.Y;
            earthCometVector[2] = cometPv[2] - earthPv.Z;

            double cometRa = 0.0;
            double cometDec = 0.0;

            short rc = Novas.Vector2RaDec(earthCometVector, ref cometRa, ref cometDec);

            if (rc > 0)
                throw new Exception($"Vector2RaDec returned a non-zero code: {rc}");

            LogMessage($"Comet Astrometric RA: {Utilities.HoursToHMS(cometRa)}, Declination: {Utilities.DegreesToDMS(cometDec)} ");




            string asteroidString = "00001    3.33  0.15 K232P  17.21569   73.47045   80.26013   10.58634  0.0788175  0.21411523   2.7671817  0 MPO719049  7258 123 1801-2022 0.65 M-v 30l MPC        0000              (1) Ceres";
            //string asteroidString = "00004    3.21  0.15 K232P 115.13302  151.59911  103.75733    7.13926  0.0887575  0.27133009   2.3630382  0 MPO719049  7511 108 1821-2022 0.63 M-p 18l MPC        0000              (4) Vesta";

            LogMessage($"Asteroid string: {asteroidString}");

            DateTime epoch = DateTime.Parse("2023 02 25", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);


            double julianEpoch = AstroUtilities.JulianDateFromDateTime(epoch);
            double argumentOfperihelion = double.Parse(asteroidString.Substring(37, 9));
            double longitudeofAscendingNode = double.Parse(asteroidString.Substring(48, 9));
            double inclination = double.Parse(asteroidString.Substring(59, 9));
            double eccentricity = double.Parse(asteroidString.Substring(70, 9));
            double meanDailyMotion = double.Parse(asteroidString.Substring(80, 11));
            double semiMajorAxis = double.Parse(asteroidString.Substring(92, 11));

            double meanAnomaly = GetParameter("Mean anomaly", 26, 9, asteroidString);

            LogMessage($"Asteroid parameters at epoch {epoch} Julian day: {julianEpoch}\r\n" +
                $"Mean anomaly: {meanAnomaly}\r\n" +
                $"Argument of perihelion: {argumentOfperihelion}\r\n" +
                $"Longitude of ascending node: {longitudeofAscendingNode}\r\n" +
                $"Inclination: {inclination}\r\n" +
                $"Eccentricity: {eccentricity}\r\n" +
                $"Mean daily motion: {meanDailyMotion}\r\n" +
                $"Semi major axis: {semiMajorAxis}");

            Ephemeris asteroidEphemeris = new Ephemeris();
            asteroidEphemeris.w_PerihelionArgument = argumentOfperihelion;
            asteroidEphemeris.BodyType = BodyType.MinorPlanet;
            asteroidEphemeris.Epoch = julianEpoch;
            asteroidEphemeris.i_Inclination = inclination;
            asteroidEphemeris.M_MeanAnomalyAtEpoch = meanAnomaly;
            asteroidEphemeris.n_MeanDailyMotion = meanDailyMotion;
            asteroidEphemeris.Name = "Ceres";
            asteroidEphemeris.Node = longitudeofAscendingNode;
            asteroidEphemeris.e_OrbitalEccentricity = eccentricity;
            asteroidEphemeris.a_SemiMajorAxis = semiMajorAxis;


            double[] asteroidPv = asteroidEphemeris.GetPositionAndVelocity(targetJdTT);

            double[] asteroidPvPrecessed = new double[3] { 0.0, 0.0, 0.0 };

            double asteroidtSunDistance = Math.Sqrt(asteroidPv[0] * asteroidPv[0] + asteroidPv[1] * asteroidPv[1] + asteroidPv[2] * asteroidPv[2]);
            LogMessage($"Asteroid-Sun distance: {asteroidtSunDistance} AU = {asteroidtSunDistance * AU2KM:0}km");

            LogMessage($"Asteroid-Sun vector: {asteroidPv[0]} {asteroidPv[1]} {asteroidPv[2]}");


            earthPv = earth.HelioCentricPosition(targetTime);
            double earthSunDistance2 = Math.Sqrt(earthPv.X * earthPv.X + earthPv.Y * earthPv.Y + earthPv.Z * earthPv.Z);

            LogMessage($"Earth-Sun distance: {earthSunDistance2} AU = {earthSunDistance2 * AU2KM:0}km");


            double asteroidEarthDistance = Math.Sqrt((asteroidPv[0] - earthPv.X) * (asteroidPv[0] - earthPv.X) + (asteroidPv[1] - earthPv.Y) * (asteroidPv[1] - earthPv.Y) + (asteroidPv[2] - earthPv.Z) * (asteroidPv[2] - earthPv.Z));

            LogMessage($"Earth-asteroid distance: {asteroidEarthDistance} AU = {asteroidEarthDistance * AU2KM:0}km");
            double[] earthAsteroidVector = new double[3];
            earthAsteroidVector[0] = asteroidPv[0] - earthPv.X;
            earthAsteroidVector[1] = asteroidPv[1] - earthPv.Y;
            earthAsteroidVector[2] = asteroidPv[2] - earthPv.Z;

            double asteroidRa = 0.0;
            double asteroidDec = 0.0;

            short asteroidRc = Novas.Vector2RaDec(earthAsteroidVector, ref asteroidRa, ref asteroidDec);

            if (asteroidRc > 0)
                throw new Exception($"Vector2RaDec returned a non-zero code: {rc}");

            LogMessage($"Asteroid Astrometric RA: {Utilities.HoursToHMS(asteroidRa, ":", ":", "", 3)}, Declination: {Utilities.DegreesToDMS(asteroidDec, ":", ":", "", 3)} ");

            Planet planet = new Planet();

            planet.Name = "Ceres";
            planet.Type = BodyType.MinorPlanet;
            planet.Ephemeris = asteroidEphemeris;
            planet.Number = 1;
            PositionVector positionVector = planet.GetAstrometricPosition(targetJdTT);
            LogMessage($"Asteroid Astrometric RA (TT):  {Utilities.HoursToHMS(positionVector.RightAscension, ":", ":", "", 3)}, Declination: {Utilities.DegreesToDMS(positionVector.Declination, ":", ":", "", 3)} ");

            positionVector = planet.GetAstrometricPosition(targetJd);
            LogMessage($"Asteroid Astrometric RA (UTC): {Utilities.HoursToHMS(positionVector.RightAscension, ":", ":", "", 3)}, Declination: {Utilities.DegreesToDMS(positionVector.Declination, ":", ":", "", 3)} ");
        }

        private static double GetParameter(string name, int start, int length, string orbitstring)
        {
            if (!double.TryParse(orbitstring.AsSpan(start, length), CultureInfo.InvariantCulture, out double parsedValue))
                throw new Exception($"Unable to parse the {name} value starting at position {start} for {length} characters from {orbitstring}");
            return parsedValue;
        }

        static void LogMessage(string message)
        {
            Console.WriteLine(message);
            logger?.LogMessage("Tester", message);
        }
    }
}