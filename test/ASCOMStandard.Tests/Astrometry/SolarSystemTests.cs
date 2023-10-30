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

namespace ASCOM.Alpaca.Tests.Astrometry
{
    public class SolarSystemTests
    {
        private readonly ITestOutputHelper output;

        public SolarSystemTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Position()
        {
            DateTime dateTime = DateTime.Parse("2023-07-01T12:00:00.0000000Z", CultureInfo.CreateSpecificCulture("en-uk"), DateTimeStyles.AssumeUniversal);
            DateTime dateTimeUtc = dateTime.ToUniversalTime();
            SolarSystem venus = new SolarSystem(Body.Venus);

            BodyPositionVelocity venusPV = venus.Position(dateTimeUtc, Origin.Heliocentric);
            output.WriteLine($"Sun - Venus distance on {dateTimeUtc} {dateTimeUtc.Kind} - {venusPV.Distance} AU, {venusPV.X} {venusPV.Y} {venusPV.Z} {venusPV.VelocityX} {venusPV.VelocityY} {venusPV.VelocityZ}");

            SolarSystem earth = new SolarSystem(Body.Earth);

            BodyPositionVelocity earthPV = earth.Position(dateTimeUtc, Origin.Heliocentric);
            output.WriteLine($"Sun - Earth distance on {dateTimeUtc} {dateTimeUtc.Kind} - {earthPV.Distance} AU, {earthPV.X} {earthPV.Y} {earthPV.Z} {venusPV.VelocityX} {venusPV.VelocityY} {earthPV.VelocityZ}");

            double earthVenusDistance = Math.Sqrt(
                (venusPV.X - earthPV.X) * (venusPV.X - earthPV.X) +
                (venusPV.Y - earthPV.Y) * (venusPV.Y - earthPV.Y) +
                (venusPV.Z - earthPV.Z) * (venusPV.Z - earthPV.Z)
                );

            output.WriteLine($"Venus - Earth    distance on {dateTimeUtc} {dateTimeUtc.Kind} - {earthVenusDistance}");

            BodyPositionVelocity venusToEarth = venus.Position(dateTimeUtc, Origin.GeoCentric);
            output.WriteLine($"Venus - Earth v2 distance on {dateTimeUtc} {dateTimeUtc.Kind} - {venusToEarth.Distance}");



        }




        [Fact]
        public void VenusAstroUtc()
        {
            DateTime dateTime = DateTime.Parse("2023-07-01T12:00:00.0000000Z", CultureInfo.CreateSpecificCulture("en-uk"), DateTimeStyles.AssumeUniversal);
            DateTime dateTimeUtc = dateTime.ToUniversalTime();
            SolarSystem venus = new SolarSystem(Body.Venus);

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
            SolarSystem venus = new SolarSystem(Body.Venus);
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
            SolarSystem body = new SolarSystem(Body.Venus)
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
            SolarSystem body = new SolarSystem(Body.Venus)
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
            SolarSystem body = new SolarSystem(Body.Venus)
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
            SolarSystem jupiter = new SolarSystem(Body.Jupiter);
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
            SolarSystem jupiter = new SolarSystem(Body.Jupiter)
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
            SolarSystem body = new SolarSystem(Body.Moon)
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
            SolarSystem body = new SolarSystem(Body.Moon)
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
            Assert.Throws<HelperException>(() =>
            {
                SolarSystem body = new SolarSystem(Body.Earth)
                {
                    SiteLatitude = Utilities.DMSToDegrees("51:04:43"),
                    SiteLongitude = -Utilities.DMSToDegrees("00:17:40"),
                    SiteHeight = 80,
                };
            });
        }

        [Fact]
        public void PlutoAstro()
        {
            SolarSystem body = new SolarSystem(Body.Pluto)
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
            SolarSystem body = new SolarSystem(Body.Pluto)
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
