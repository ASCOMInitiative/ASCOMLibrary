namespace ASCOM.Standard.Interfaces
{
    /// <summary>
    /// The telescope axes
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
        Tertiary = 2
    }
}