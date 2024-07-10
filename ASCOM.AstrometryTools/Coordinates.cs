namespace ASCOM.Tools
{
    /// <summary>
    /// Solar system body coordinates.
    /// </summary>
    public class Coordinates
    {
        /// <summary>
        /// Right Ascension coordinate
        /// </summary>
        public double RightAscension = 0.0;

        /// <summary>
        /// Declination coordinate
        /// </summary>
        public double Declination = 0.0;

        /// <summary>
        /// Distance coordinate
        /// </summary>
        public double Distance = 0.0;

        /// <summary>
        /// Altitude coordinate
        /// </summary>
        public double Altitude = 0.0;

        /// <summary>
        /// Azimuth coordinate
        /// </summary>
        public double Azimuth = 0.0;

        /// <summary>
        /// Format the class's values
        /// </summary>
        /// <returns>Formatted string comprising the class's RA, declination, distance, azimuth and altitude values.</returns>
        public override string ToString()
        {
            return $"RA: {Utilities.HoursToHMS(RightAscension, ":", ":", "", 3)}, Declination: {Utilities.DegreesToDMS(Declination, ":", ":", "", 2)}, Distance: {Distance:0.000} AU, Azimuth: {Utilities.DegreesToDMS(Azimuth, ":", ":", "", 2)},Altitude: {Utilities.DegreesToDMS(Altitude, ":", ":", "", 2)}";
        }
    }
}
