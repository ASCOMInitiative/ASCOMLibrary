namespace ASCOM.Alpaca.Devices.Telescope
{
    /// <summary>
    /// Equatorial coordinate systems used by telescopes.
    /// Only used with telescope interface versions 2 and 3
    /// </summary>
    /// <remarks>
    /// In June 2018 the name equLocalTopocentric was deprecated in favour of equTopocentric, both names return the same value (1).
    /// The rationale for this change is set out in the <conceptualLink target="72A95B28-BBE2-4C7D-BC03-2D6AB324B6F7">Astronomical Coordinates</conceptualLink> section.
    /// </remarks>
    public enum EquatorialCoordinateType
    {
        /// <summary>
        /// Custom or unknown equinox and/or reference frame.
        /// </summary>
        Other = 0,

        /// <summary>
        /// Topocentric coordinates.Coordinates of the object at the current date having allowed for annual aberration,
        /// precession and nutation.This is the most common coordinate type for amateur telescopes.
        /// </summary>
        Topocentric = 1,

        /// <summary>
        /// J2000 equator/equinox.Coordinates of the object at mid-day on 1st January 2000, ICRS reference frame.
        /// </summary>
        J2000 = 2,

        /// <summary>
        /// J2050 equator/equinox, ICRS reference frame.
        /// </summary>
        J2050 = 3,

        /// <summary>
        /// B1950 equinox, FK4 reference frame.
        /// </summary>
        B1950 = 4
    }
}