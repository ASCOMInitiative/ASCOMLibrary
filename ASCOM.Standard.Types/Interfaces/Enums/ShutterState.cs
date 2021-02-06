namespace ASCOM.Standard.Interfaces
{
    /// <summary>
    /// ASCOM Dome ShutterState status values.
    /// </summary>
    public enum ShutterState
    {
        /// <summary>
        /// Dome shutter status open
        /// </summary>
        Open = 0,

        /// <summary>
        /// Dome shutter status closed
        /// </summary>
        Closed = 1,

        /// <summary>
        /// Dome shutter status opening
        /// </summary>
        Opening = 2,

        /// <summary>
        /// Dome shutter status closing
        /// </summary>
        Closing = 3,

        /// <summary>
        /// Dome shutter status error
        /// </summary>
        Error = 4
    }
}