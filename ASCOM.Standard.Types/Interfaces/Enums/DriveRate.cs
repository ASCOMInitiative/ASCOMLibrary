namespace ASCOM.Standard.Interfaces
{
    /// <summary>
    /// Well-known telescope tracking rates.
    /// </summary>
    public enum DriveRate
    {
        /// <summary>
        /// Sidereal tracking rate (15.041 arcseconds per second).
        /// </summary>
        DriveSidereal = 0,

        /// <summary>
        /// Lunar tracking rate (14.685 arcseconds per second).
        /// </summary>
        DriveLunar = 1,

        /// <summary>
        /// Solar tracking rate (15.0 arcseconds per second).
        /// </summary>
        DriveSolar = 2,

        /// <summary>
        /// King tracking rate (15.0369 arcseconds per second).
        /// </summary>
        DriveKing = 3
    }
}