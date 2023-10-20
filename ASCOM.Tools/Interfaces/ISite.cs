namespace ASCOM.Tools.Interfaces
{
    /// <summary>
    /// Interface to the NOVAS-COM Site Class
    /// </summary>
    /// <remarks>Objects of class Site contain the specifications for an observer's location on the Earth 
    /// ellipsoid. Properties are latitude, longitude, height above mean sea level, the ambient temperature 
    /// and the sea-level barometric pressure. The latter two are used only for optional refraction corrections. 
    /// Latitude and longitude are (common) geodetic, not geocentric. </remarks>
    public interface ISite
    {
        /// <summary>
        /// Set all site properties in one method call
        /// </summary>
        /// <param name="Latitude">The geodetic latitude (degrees, + north)</param>
        /// <param name="Longitude">The geodetic longitude (degrees, +east)</param>
        /// <param name="Height">Height above sea level (meters)</param>
        /// <remarks></remarks>
        void Set(double Latitude, double Longitude, double Height);

        /// <summary>
        /// Height above mean sea level
        /// </summary>
        /// <value>Height above mean sea level</value>
        /// <returns>Meters</returns>
        /// <remarks></remarks>
        double Height { get; set; }

        /// <summary>
        /// Geodetic latitude (degrees, + north)
        /// </summary>
        /// <value>Geodetic latitude</value>
        /// <returns>Degrees, + north</returns>
        /// <remarks></remarks>
        double Latitude { get; set; }

        /// <summary>
        /// Geodetic longitude (degrees, + east)
        /// </summary>
        /// <value>Geodetic longitude</value>
        /// <returns>Degrees, + east</returns>
        /// <remarks></remarks>
        double Longitude { get; set; }

        /// <summary>
        /// Barometric pressure (millibars)
        /// </summary>
        /// <value>Barometric pressure</value>
        /// <returns>Millibars</returns>
        /// <remarks></remarks>
        double Pressure { get; set; }

        /// <summary>
        /// Ambient temperature (deg. Celsius)
        /// </summary>
        /// <value>Ambient temperature</value>
        /// <returns>Degrees Celsius)</returns>
        /// <remarks></remarks>
        double Temperature { get; set; }
    }
}