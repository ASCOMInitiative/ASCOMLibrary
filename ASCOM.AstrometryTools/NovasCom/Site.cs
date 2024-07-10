using ASCOM.Tools.Interfaces;

namespace ASCOM.Tools.NovasCom
{
    /// <summary>
    /// NOVAS-COM: Site Class
    /// </summary>
    /// <remarks>NOVAS-COM objects of class Site contain the specifications for an observer's location on the Earth 
    /// ellipsoid. Properties are latitude, longitude, height above mean sea level, the ambient temperature 
    /// and the sea-level barometric pressure. The latter two are used only for optional refraction corrections. 
    /// Latitude and longitude are (common) geodetic, not geocentric. </remarks>
    public class Site
    {
        private double vHeight, vLatitude, vLongitude, vPressure, vTemperature;
        private bool HeightValid, LatitudeValid, LongitudeValid, PressureValid, TemperatureValid;

        /// <summary>
        /// Initialises a new site object
        /// </summary>
        /// <remarks></remarks>
        public Site()
        {
            HeightValid = false;
            LatitudeValid = false;
            LongitudeValid = false;
            PressureValid = false;
            TemperatureValid = false;
        }

        /// <summary>
        /// Height above mean sea level
        /// </summary>
        /// <value>Height above mean sea level</value>
        /// <returns>Meters</returns>
        /// <remarks></remarks>
        public double Height
        {
            get
            {
                if (!HeightValid)
                    throw new HelperException("Site.Height - Height has not yet been set");

                return vHeight;
            }
            set
            {
                vHeight = value;
                HeightValid = true;
            }
        }

        /// <summary>
        /// Geodetic latitude (degrees, + north)
        /// </summary>
        /// <value>Geodetic latitude</value>
        /// <returns>Degrees, + north</returns>
        /// <remarks></remarks>
        public double Latitude
        {
            get
            {
                if (!LatitudeValid)
                    throw new HelperException("Site.Height - Latitude has not yet been set");

                return vLatitude;
            }
            set
            {
                vLatitude = value;
                LatitudeValid = true;
            }
        }

        /// <summary>
        /// Geodetic longitude (degrees, + east)
        /// </summary>
        /// <value>Geodetic longitude</value>
        /// <returns>Degrees, + east</returns>
        /// <remarks></remarks>
        public double Longitude
        {
            get
            {
                if (!LongitudeValid)
                    throw new HelperException("Site.Height - Longitude has not yet been set");

                return vLongitude;
            }
            set
            {
                vLongitude = value;
                LongitudeValid = true;
            }
        }

        /// <summary>
        /// Barometric pressure (millibars)
        /// </summary>
        /// <value>Barometric pressure</value>
        /// <returns>Millibars</returns>
        /// <remarks></remarks>
        public double Pressure
        {
            get
            {
                if (!PressureValid)
                    throw new HelperException("Site.Height - Pressure has not yet been set");

                return vPressure;
            }
            set
            {
                vPressure = value;
                PressureValid = true;
            }
        }

        /// <summary>
        /// Set all site properties in one method call
        /// </summary>
        /// <param name="Latitude">The geodetic latitude (degrees, + north)</param>
        /// <param name="Longitude">The geodetic longitude (degrees, +east)</param>
        /// <param name="Height">Height above sea level (meters)</param>
        /// <remarks></remarks>
        public void Set(double Latitude, double Longitude, double Height)
        {
            vLatitude = Latitude;
            vLongitude = Longitude;
            vHeight = Height;
            LatitudeValid = true;
            LongitudeValid = true;
            HeightValid = true;
        }

        /// <summary>
        /// Ambient temperature (deg. Celsius)
        /// </summary>
        /// <value>Ambient temperature</value>
        /// <returns>Degrees Celsius)</returns>
        /// <remarks></remarks>
        public double Temperature
        {
            get
            {
                if (!TemperatureValid)
                    throw new HelperException("Site.Height - Temperature has not yet been set");

                return vTemperature;
            }
            set
            {
                vTemperature = value;
                TemperatureValid = true;
            }
        }
    }
}