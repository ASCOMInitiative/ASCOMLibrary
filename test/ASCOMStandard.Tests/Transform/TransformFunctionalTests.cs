using ASCOM.Tools;
using System;
using System.Xml.Linq;
using Xunit;

namespace TransformTests
{
    public class TransformFunctionalTests
    {
        readonly TraceLogger TL = new("TransformTest", true);
        Transform transform;

        const int TEST_YEAR = 2025; // UTC
        const int TEST_MONTH = 4;
        const int TEST_DAY = 28;
        const int TEST_HOUR = 12; // UTC
        const int TEST_MINUTE = 15;
        const int TEST_SECOND = 30;

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

        [Fact]
        public void ReciprocalTopoCentric()
        {
            transform = new Transform(TL);
            Assert.NotNull(transform);

            // Site parameters
            double SiteLat = 51.0 + (4.0 / 60.0) + (43.0 / 3600.0);
            double SiteLong = 0.0 - (17.0 / 60.0) - (40.0 / 3600.0);

            // Set up Transform component
            transform.SiteElevation = 80.0;
            transform.SiteLatitude = SiteLat;
            transform.SiteLongitude = SiteLong;
            transform.SiteTemperature = 10.0;
            transform.Refraction = false;
            transform.SetJ2000(5.0, 60.0);

            // Set a specific date for the calculation
            double testJulianDate = AstroUtilities.JulianDateFromDateTime(new DateTime(TEST_YEAR, TEST_MONTH, TEST_DAY, TEST_HOUR, TEST_MINUTE, TEST_SECOND, DateTimeKind.Utc));
            transform.JulianDateUTC = testJulianDate;

            TL.LogMessage("TransformTest", $"*** Test Julian Date: {testJulianDate}");

            // Test with refraction false
            double topocentricRa = transform.RATopocentric;
            double topocentricDec = transform.DECTopocentric;
            TL.LogMessage("TransformTest", $"*** Topocentric coordinates (refraction false): {Utilities.HoursToHMS(transform.RATopocentric, ":", ":", "", 3)} {Utilities.DegreesToDMS(transform.DECTopocentric, ":", ":", "", 3)}");

            transform.SetTopocentric(topocentricRa, topocentricDec);

            double j2000Ra = transform.RAJ2000;
            double j2000Dec = transform.DecJ2000;
            TL.LogMessage("TransformTest", $"*** Reciprocal J2000 coordinates (refraction false): {Utilities.HoursToHMS(j2000Ra, ":", ":", "", 3)} {Utilities.DegreesToDMS(j2000Dec, ":", ":", "", 3)}");

            Assert.Equal(5.0, transform.RAJ2000, 3);
            Assert.Equal(60.0, transform.DecJ2000, 3);
        }

        [Fact]
        public void ReciprocalTopoCentricRefracted()
        {
            transform = new Transform(TL);
            Assert.NotNull(transform);

            // Site parameters
            double SiteLat = 51.0 + (4.0 / 60.0) + (43.0 / 3600.0);
            double SiteLong = 0.0 - (17.0 / 60.0) - (40.0 / 3600.0);

            // Set up Transform component
            transform.SiteElevation = 80.0;
            transform.SiteLatitude = SiteLat;
            transform.SiteLongitude = SiteLong;
            transform.SiteTemperature = 10.0;
            transform.Refraction = true;
            transform.SetJ2000(5.0, 60.0);

            // Set a specific date for the calculation
            double testJulianDate = AstroUtilities.JulianDateFromDateTime(new DateTime(TEST_YEAR, TEST_MONTH, TEST_DAY, TEST_HOUR, TEST_MINUTE, TEST_SECOND, DateTimeKind.Utc));
            transform.JulianDateUTC = testJulianDate;

            TL.LogMessage("TransformTest", $"*** Test Julian Date: {testJulianDate}");

            // Test with refraction false
            double topocentricRa = transform.RATopocentric;
            double topocentricDec = transform.DECTopocentric;
            TL.LogMessage("TransformTest", $"*** Topocentric coordinates (refraction true): {Utilities.HoursToHMS(topocentricRa, ":", ":", "", 3)} {Utilities.DegreesToDMS(topocentricDec, ":", ":", "", 3)}");

            transform.SetTopocentric(topocentricRa, topocentricDec);

            double j2000Ra = transform.RAJ2000;
            double j2000Dec = transform.DecJ2000;
            TL.LogMessage("TransformTest", $"*** Reciprocal J2000 coordinates (refraction true): {Utilities.HoursToHMS(j2000Ra, ":", ":", "", 3)} {Utilities.DegreesToDMS(j2000Dec, ":", ":", "", 3)}");

            Assert.Equal(5.0, j2000Ra, 3);
            Assert.Equal(60.0, j2000Dec, 3);
        }

        [Fact]
        public void NormalMode()
        {
            transform = new Transform(TL);
            Assert.NotNull(transform);

            // Site parameters
            double SiteLat = 51.0 + (4.0 / 60.0) + (43.0 / 3600.0);
            double SiteLong = 0.0 - (17.0 / 60.0) - (40.0 / 3600.0);

            // Set up Transform component
            transform.SiteElevation = 80.0;
            transform.SiteLatitude = SiteLat;
            transform.SiteLongitude = SiteLong;
            transform.SiteTemperature = 10.0;
            transform.Refraction = false;
            transform.SetTopocentric(1.0, 50.0);

            // Set a specific date for the calculation
            double testJulianDate = AstroUtilities.JulianDateFromDateTime(new DateTime(TEST_YEAR, TEST_MONTH, TEST_DAY, TEST_HOUR, TEST_MINUTE, TEST_SECOND, DateTimeKind.Utc));
            transform.JulianDateUTC = testJulianDate;

            TL.LogMessage("TransformTest", $"Test Julian Date: {testJulianDate}");

            // Test with refraction false
            TL.LogMessage("TransformTest", "NormalMode Transform RA/DEC topocentric (refraction false):  " + Utilities.HoursToHMS(transform.RATopocentric, ":", ":", "", 3) + " " + Utilities.DegreesToDMS(transform.DECTopocentric, ":", ":", "", 3));
            Assert.Equal(1.0, transform.RATopocentric, 3);
            Assert.Equal(50.0, transform.DECTopocentric, 3);

            // Test with refraction true
            transform.Refraction = false;
            TL.LogMessage("TransformTest", "NormalMode Transform RA/DEC topocentric (refraction true):  " + Utilities.HoursToHMS(transform.RATopocentric, ":", ":", "", 3) + " " + Utilities.DegreesToDMS(transform.DECTopocentric, ":", ":", "", 3));
            Assert.Equal(1.0, transform.RATopocentric, 3);
            Assert.Equal(50.0, transform.DECTopocentric, 3);

            Assert.Throws<ASCOM.TransformInvalidOperationException>(() => transform.RAObserved);
            Assert.Throws<ASCOM.TransformInvalidOperationException>(() => transform.DECObserved);
            Assert.Throws<ASCOM.TransformInvalidOperationException>(() => transform.AzimuthObserved);
            Assert.Throws<ASCOM.TransformInvalidOperationException>(() => transform.ElevationObserved);
        }

        [Fact]
        public void ObservedMode()
        {
            transform = new Transform();
            Assert.NotNull(transform);
            // Site parameters
            double SiteLat = 51.0 + (4.0 / 60.0) + (43.0 / 3600.0);
            double SiteLong = 0.0 - (17.0 / 60.0) - (40.0 / 3600.0);

            // Set up Transform component
            transform.SiteElevation = 80.0;
            transform.SiteLatitude = SiteLat;
            transform.SiteLongitude = SiteLong;
            transform.SiteTemperature = 10.0;
            transform.ObservedMode = true;
            transform.SetTopocentric(1.0, 50.0);

            // Set a specific date for the calculation
            double testJulianDate = AstroUtilities.JulianDateFromDateTime(new DateTime(TEST_YEAR, TEST_MONTH, TEST_DAY, TEST_HOUR, TEST_MINUTE, TEST_SECOND, DateTimeKind.Utc));
            transform.JulianDateUTC = testJulianDate;

            TL.LogMessage("TransformTest", $"Test Julian Date: {testJulianDate}");

            // Test unrefracted values
            TL.LogMessage("TransformTest", "ObservedMode Transform RA/DEC Topocentric (unrefracted):  " + Utilities.HoursToHMS(transform.RATopocentric, ":", ":", "", 3) + " " + Utilities.DegreesToDMS(transform.DECTopocentric, ":", ":", "", 3));
            Assert.Equal(1.0, transform.RATopocentric, 3);
            Assert.Equal(50.0, transform.DECTopocentric, 3);

            // Test refracted values
            TL.LogMessage("TransformTest", "ObservedMode Transform RA/DEC Observed (refracted):  " + Utilities.HoursToHMS(transform.RAObserved, ":", ":", "", 3) + " " + Utilities.DegreesToDMS(transform.DECObserved, ":", ":", "", 3));
            Assert.Equal(1.0, transform.RAObserved, 1);
            Assert.Equal(50.021, transform.DECObserved, 1);

            Assert.Throws<ASCOM.TransformInvalidOperationException>(() => transform.Refraction = true);
            Assert.Throws<ASCOM.TransformInvalidOperationException>(() => transform.Refraction = false);
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
            double testJulianDate = AstroUtilities.JulianDateFromDateTime(new DateTime(2022, 1, 1, 03, 0, 0, DateTimeKind.Utc));
            transform.JulianDateUTC = testJulianDate;
            TL.LogMessage("TransformTest", $"Test Julian Date: {testJulianDate}");

            TL.LogMessage("TransformTest", Name + " Transform RA/DEC Astro: " + Utilities.HoursToHMS(AstroRA, ":", ":", "", 3) + " " + Utilities.DegreesToDMS(AstroDEC, ":", ":", "", 3));
            TL.LogMessage("TransformTest", Name + " Transform RA/DEC Topo:  " + Utilities.HoursToHMS(transform.RATopocentric, ":", ":", "", 3) + " " + Utilities.DegreesToDMS(transform.DECTopocentric, ":", ":", "", 3));

            Assert.Equal(Utilities.HMSToHours(expectedRAString), transform.RATopocentric, RATolerance);
            Assert.Equal(Utilities.DMSToDegrees(expectedDECString), transform.DECTopocentric, DecTolerance);
        }

    }
}
