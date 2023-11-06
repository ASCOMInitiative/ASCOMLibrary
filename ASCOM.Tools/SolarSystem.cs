using ASCOM.Tools.Novas31;
using System;
using System.Collections.Generic;
using System.Text;
using ASCOM.Common;
using System.Diagnostics;
using System.Globalization;

namespace ASCOM.Tools
{

    /// <summary>
    /// Provides astrometric and topocentric coordinates for the planets, sun and moon.
    /// </summary>
    /// <remarks>
    /// This is an easy to use tool that provides coordinates for major solar system bodies. Internally it uses the JPL DE421 ephemeris data contained within the NOVAS component.
    /// Pluto is included for convenience even though it is no longer recognised as a major planet by the IAU.
    /// </remarks>
    public class SolarSystem
    {
        private readonly Body body;
        private SetBy setBy = SetBy.None;

        private enum SetBy
        {
            None = 0,
            PlanetSunMoon = 1,
            Comet = 2,
            MinorPlanet = 3
        }


        /// <summary>
        /// Create a SolarSystem object for a specified target body
        /// </summary>
        /// <param name="body"></param>
        /// <exception cref="HelperException">When earth is specified as the target body.</exception>
        public SolarSystem(Body body)
        {
            // Validate the body parameter
            if (body == Body.Uninitialised)
                throw new HelperException($"The supplied body value is invalid: {body}.");

            if (((int)body < 1) | ((int)body > 11))
                throw new HelperException($"The supplied body value is invalid: {(int)body}.");

            this.body = body;
            setBy = SetBy.PlanetSunMoon;
        }

        public SolarSystem(BodyType bodyType, string MpcOrbitString)
        {
            switch (bodyType)
            {
                case BodyType.Comet:
                    setBy = SetBy.Comet;
                    break;

                case BodyType.MinorPlanet:
                    setBy = SetBy.MinorPlanet;
                    break;

                default:
                    throw new HelperException($"This constructor only supports comets and minor planets. Please use the SolarSystem(Body body) constructor for the major planets, sun and moon.");
            }
        }


        /// <summary>
        /// Site latitude (North positive)
        /// </summary>
        public double? SiteLatitude { get; set; }

        /// <summary>
        /// Site longitude (East positive)
        /// </summary>
        public double? SiteLongitude { get; set; }

        /// <summary>
        /// Site height (metres)
        /// </summary>
        public double? SiteHeight { get; set; }

        /// <summary>
        /// Site pressure (Hectopascal or millibar)
        /// </summary>
        public double SitePressure { get; set; } = 1010;

        /// <summary>
        /// Site temperature (Celsius)
        /// </summary>
        public double SiteTemperature { get; set; } = 10.0;

        /// <summary>
        /// True to create coordinates corrected for refraction. (Only used by <see cref="AltAzCoordinates(DateTime)"/>.)
        /// </summary>
        public bool Refraction { get; set; } = false;

        /// <summary>
        /// Return the body's astrometric coordinates at a given time
        /// </summary>
        /// <param name="observationTime">Observation time (local or UTC)</param>
        /// <returns>Coordinates class containing the astrometric right ascension, declination and distance to the object.</returns>
        /// <remarks>The supplied DateTime value can be either a local time (DateTime.Kind = Local or Unspecified) or a UTC time (DateTime.Kind = Utc)</remarks>
        public Coordinates AstrometricCoordinates(DateTime observationTime)
        {
            // Calculate UTC and Terrestrial time Julian day numbers
            double julianDateUtc = Utilities.JulianDateFromDateTime(observationTime.ToUniversalTime());
            double julianDateTT = julianDateUtc + Utilities.DeltaT(julianDateUtc) / 86400.0;

            // Validate that the selected body is not earth
            ValidateBodyIsNotEarth(nameof(AstrometricCoordinates));

            // Create a planet object
            Object3 planet = new Object3
            {
                Type = ObjectType.MajorPlanetSunOrMoon,
                Number = body
            };

            // Initialise a Coordinates object to receive the result
            Coordinates result = new Coordinates();

            //Calculate the astrometric position of the planet, placing the result RA/Dec in results object
            short rc = Novas.AstroPlanet(julianDateTT, planet, Accuracy.Full, ref result.RightAscension, ref result.Declination, ref result.Distance);

            // Throw an exception if the calculation failed for some reason
            if (rc != 0)
                throw new HelperException($"SolarSystem.{nameof(AstrometricCoordinates)} - Error in Novas.AstroPlanet, return code: {rc}");

            // Return the result
            return result;
        }

        /// <summary>
        /// Return the body's topocentric coordinates at a given time
        /// </summary>
        /// <param name="observationTime">Observation time</param>
        /// <returns>Coordinates struct containing the topocentric right ascension and declination</returns>
        public Coordinates TopocentricCoordinates(DateTime observationTime)
        {
            // Validate that the selected body is not earth
            ValidateBodyIsNotEarth(nameof(TopocentricCoordinates));

            // Confirm that required parameters have been set
            ValidateSiteLocation(nameof(TopocentricCoordinates));

            // Calculate UTC and Terrestrial time Julian day numbers
            double julianDateUtc = Utilities.JulianDateFromDateTime(observationTime.ToUniversalTime());
            double julianDateTT = julianDateUtc + Utilities.DeltaT(julianDateUtc) / 86400.0;

            // Create an OnSurface object
            OnSurface location = new OnSurface
            {
                Latitude = SiteLatitude.Value,
                Longitude = SiteLongitude.Value,
                Height = SiteHeight.Value,
                Temperature = SiteTemperature,
                Pressure = SitePressure
            };

            // Create a planet object
            Object3 planet = new Object3
            {
                Type = ObjectType.MajorPlanetSunOrMoon,
                Number = body
            };

            // Initialise a Coordinates object to receive the result
            Coordinates result = new Coordinates();

            //Calculate the topocentric position of the planet, placing the result RA/Dec in results object
            short rc = Novas.TopoPlanet(julianDateTT, planet, Utilities.DeltaT(julianDateUtc), location, Accuracy.Full, ref result.RightAscension, ref result.Declination, ref result.Distance);

            // Throw an exception if the calculation failed for some reason
            if (rc != 0)
                throw new HelperException($"SolarSystem.{nameof(TopocentricCoordinates)} - Error in Novas.TopoPlanet, return code: {rc}");

            // Return the result
            return result;
        }

        /// <summary>
        /// Return the body's topocentric coordinates at a given time
        /// </summary>
        /// <param name="observationTime">Observation time</param>
        /// <returns>Coordinates struct containing the topocentric right ascension and declination</returns>
        public Coordinates AltAzCoordinates(DateTime observationTime)
        {
            // Validate that the selected body is not earth
            ValidateBodyIsNotEarth(nameof(AltAzCoordinates));

            // Confirm that required parameters have been set
            ValidateSiteLocation(nameof(AltAzCoordinates));

            // Calculate UTC and Terrestrial time Julian day numbers
            double julianDateUtc = Utilities.JulianDateFromDateTime(observationTime.ToUniversalTime());
            double julianDateTT = julianDateUtc + Utilities.DeltaT(julianDateUtc) / 86400.0;

            // Create an OnSurface object
            OnSurface location = new OnSurface
            {
                Latitude = SiteLatitude.Value,
                Longitude = SiteLongitude.Value,
                Height = SiteHeight.Value,
                Temperature = SiteTemperature,
                Pressure = SitePressure
            };

            // Create a planet object
            Object3 planet = new Object3
            {
                Type = ObjectType.MajorPlanetSunOrMoon,
                Number = body
            };

            // Initialise a Coordinates object to receive the result
            Coordinates topocentricCoordinates = new Coordinates();

            //Calculate the topocentric position of the planet, placing the result RA/Dec in results object
            short rc = Novas.TopoPlanet(julianDateTT, planet, Utilities.DeltaT(julianDateUtc), location, Accuracy.Full, ref topocentricCoordinates.RightAscension, ref topocentricCoordinates.Declination, ref topocentricCoordinates.Distance);

            // Throw an exception if the calculation failed for some reason
            if (rc != 0)
                throw new HelperException($"SolarSystem.{nameof(AltAzCoordinates)} - Error in Novas.TopoPlanet, return code: {rc}");

            // Create a NOVAS refraction option value depending on the state of the Refraction field
            RefractionOption refractionOption = Refraction ? RefractionOption.LocationRefraction : RefractionOption.NoRefraction;

            // Initialise a Coordinates class to receive the result
            Coordinates result = new Coordinates();

            // The Equ2Hor method returns a zenith distance rather than an altitude so initialise a variable to receive this
            double zenithDistance = 0.0;

            // Calculate AltAz coordinates from the topocentric coordinates
            Novas.Equ2Hor(julianDateUtc, Utilities.DeltaT(julianDateUtc), Accuracy.Full, 0.0, 0.0, location, topocentricCoordinates.RightAscension, topocentricCoordinates.Declination, refractionOption, ref zenithDistance, ref result.Azimuth, ref result.RightAscension, ref result.Declination);

            // Add the altitude calculated from the returned zenith distance
            result.Altitude = 90.0 - zenithDistance;

            //Add the distance from the topocentric coordinate calculation
            result.Distance = topocentricCoordinates.Distance;

            // Return the result
            return result;
        }

        /// <summary>
        /// Returns heliocentric or barycentric position and velocity vectors for the body
        /// </summary>
        /// <param name="observationTime">Observation time</param>
        /// <param name="origin">Origin reference point: solar system barycentre or centre of the sun</param>
        /// <returns>BodyPositionAndVelocity object</returns>
        /// <exception cref="HelperException">If the NOVAS calculation fails.</exception>
        public BodyPositionVelocity Position(DateTime observationTime, Origin origin)
        {
            double[] position = new double[3] { 0.0, 0.0, 0.0 };
            double[] velocity = new double[3] { 0.0, 0.0, 0.0 };

            // Calculate UTC and Terrestrial time Julian day numbers
            double julianDateUtc = Utilities.JulianDateFromDateTime(observationTime.ToUniversalTime());
            double julianDateTT = julianDateUtc + Utilities.DeltaT(julianDateUtc) / 86400.0;

            // Get the body's position and velocity
            short rc = Novas.SolarSystem(julianDateTT, body, origin, ref position, ref velocity);

            // Throw an exception if the calculation failed for some reason
            if (rc != 0)
                throw new HelperException($"SolarSystem.{nameof(Position)} - Error in Novas.SolarSystem, return code: {rc}");

            // Return the result
            return new BodyPositionVelocity(position, velocity);
        }

        #region Support code

        private void ValidateSiteLocation(string methodName)
        {
            if (!SiteLatitude.HasValue)
                throw new InvalidOperationException($"SolarSystem.{methodName} - The site latitude has not been set.");

            if (!SiteLongitude.HasValue)
                throw new InvalidOperationException($"SolarSystem.{methodName} - The site longitude has not been set.");

            if (!SiteHeight.HasValue)
                throw new InvalidOperationException($"SolarSystem.{methodName} - The site height has not been set.");
        }

        private void ValidateBodyIsNotEarth(string methodName)
        {
            if (body == Body.Earth)
                throw new InvalidOperationException($"SolarSystem.{methodName} - Cannot use {methodName} when the target body is Earth.");
        }
        #endregion

    }
}
