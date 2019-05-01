namespace ASCOM.Alpaca.Devices.Telescope
{
    /// <summary>
    /// The telescope axes
    /// Only used with if the telescope interface version is 2 or 3
    /// </summary>
    public enum TelescopeAxis
    {
        /// <summary>
        /// Primary axis (e.g., Right Ascension or Azimuth).
        /// </summary>
        Primary = 0,

        /// <summary>
        /// Secondary axis (e.g., Declination or Altitude).
        /// </summary>
        Secondary = 1,

        /// <summary>
        /// Tertiary axis (e.g. imager rotator/de-rotator).
        /// </summary>
        Tertiary = 3
    }
}