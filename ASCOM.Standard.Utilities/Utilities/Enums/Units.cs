namespace ASCOM.Standard.Utilities
{

    /// <summary>
    /// List of units that can be converted by the ConvertUnits method
    /// </summary>
    public enum Unit : int
    {
        // Speed
        /// <summary>
        /// Metres per second
        /// </summary>
        metresPerSecond = 0,
        /// <summary>
        /// Miles per hour
        /// </summary>
        milesPerHour = 1,
        /// <summary>
        /// Knots
        /// </summary>
        knots = 2,

        // Temperature
        /// <summary>
        /// Degrees Celsius
        /// </summary>
        degreesCelsius = 10,
        /// <summary>
        /// Degrees Fahrenheit
        /// </summary>
        degreesFarenheit = 11,
        /// <summary>
        /// Degrees kelvin
        /// </summary>
        degreesKelvin = 12,

        // Pressure
        /// <summary>
        /// Hecto pascals
        /// </summary>
        hPa = 20,
        /// <summary>
        /// Millibar
        /// </summary>
        mBar = 21,
        /// <summary>
        /// Millimetres of mercury
        /// </summary>
        mmHg = 22,
        /// <summary>
        /// Inches of mercury
        /// </summary>
        inHg = 23,

        // RainRate
        /// <summary>
        /// Millimetres per hour
        /// </summary>
        mmPerHour = 30,
        /// <summary>
        /// Inches per hour
        /// </summary>
        inPerHour = 31
    }

}
