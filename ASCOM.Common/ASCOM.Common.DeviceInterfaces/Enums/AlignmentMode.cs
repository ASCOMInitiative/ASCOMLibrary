namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// The alignment mode of the mount.
    /// </summary>
    public enum AlignmentMode
    {
        /// <summary>
        /// Altitude-Azimuth alignment.
        /// </summary>
        AltAz = 0,

        /// <summary>
        /// Polar(equatorial) mount other than German equatorial.
        /// </summary>
        Polar = 1,

        /// <summary>
        /// German equatorial mount.
        /// </summary>
        GermanPolar = 2
    }
}