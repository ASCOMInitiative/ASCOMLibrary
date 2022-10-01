using ASCOM.Tools;
using System;
using Xunit;

namespace ASCOM.Alpaca.Tests.TransformTests
{
    public class TransformFunctionalTests
    {
        TraceLogger TL = new TraceLogger("TransformTest", true);
        Transform transform;

        [Fact]
        public void Deneb()
        {
            transform = new Transform();
            Assert.NotNull(transform);

            TL.LogMessage("Deneb", "Starting TransformTests");
            TransformTest2000("Deneb", "20:41:25.916", "45:16:49.23", "20:42:08.473", "45:21:34.925", 1, 1);
            TL.LogMessage("Deneb", "Completed TransformTests");
        }

        [Fact]
        public void Polaris()
        {
            transform = new Transform();
            Assert.NotNull(transform);

            TL.LogMessage("Polaris", "Starting TransformTests");
            TransformTest2000("Polaris", "02:31:51.263", "89:15:50.68", "03:00:25.433", "89:21:37.620", 1, 1);
            TL.LogMessage("Polaris", "Completed TransformTests");
        }

        [Fact]
        public void Arcturus()
        {
            transform = new Transform();
            Assert.NotNull(transform);

            TL.LogMessage("Arcturus", "Starting TransformTests");
            TransformTest2000("Arcturus", "14:15:38.943", "19:10:37.93", "14:16:39.565", "19:04:27.311", 1, 1); 
            TL.LogMessage("Arcturus", "Completed TransformTests");
        }

        private void TransformTest2000(string Name, string AstroRAString, string AstroDECString, string expectedRAString, string expectedDECString, int RATolerance, int DecTolerance)
        {
            double AstroRA, AstroDEC;
            double SiteLat, SiteLong;

            // RA and DEC
            AstroRA = Utilities.HMSToHours(AstroRAString);
            AstroDEC = Utilities.DMSToDegrees(AstroDECString);

            // Site parameters
            SiteLat = 51.0 + (4.0 / 60.0) + (43.0 / 3600.0);
            SiteLong = 0.0 - (17.0 / 60.0) - (40.0 / 3600.0);

            // Set up Transform component
            transform.SiteElevation = 80.0;
            transform.SiteLatitude = SiteLat;
            transform.SiteLongitude = SiteLong;
            transform.SiteTemperature = 10.0;
            transform.Refraction = false;
            transform.SetJ2000(AstroRA, AstroDEC);

            // Set a specific date for the calculation
            double testJulianDate = Utilities.JulianDateFromDateTime(new DateTime(2022, 1, 1, 03, 0, 0, DateTimeKind.Utc));
            transform.JulianDateUTC = testJulianDate;
            TL.LogMessage("TransformTest", $"Test Julian Date: {testJulianDate}");

            TL.LogMessage("TransformTest", Name + " Transform RA/DEC Astro: " + Utilities.HoursToHMS(AstroRA, ":", ":", "", 3) + " " + Utilities.DegreesToDMS(AstroDEC, ":", ":", "", 3));
            TL.LogMessage("TransformTest", Name + " Transform RA/DEC Topo:  " + Utilities.HoursToHMS(transform.RATopocentric, ":", ":", "", 3) + " " + Utilities.DegreesToDMS(transform.DECTopocentric, ":", ":", "", 3));

            Assert.Equal(Utilities.HMSToHours(expectedRAString), transform.RATopocentric, RATolerance);
            Assert.Equal(Utilities.DMSToDegrees(expectedDECString), transform.DECTopocentric, DecTolerance);
        }

    }
}
