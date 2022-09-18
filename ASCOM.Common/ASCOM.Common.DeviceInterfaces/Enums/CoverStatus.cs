namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// The CoverCalibrator device's cover state
    /// </summary>
    public enum CoverStatus
    {
        /// <summary>
        /// This device does not have a cover that can be closed independently 
        /// </summary>
        NotPresent = 0,

        /// <summary>
        /// The cover is closed
        /// </summary>
        Closed = 1,

        /// <summary>
        /// The cover is moving to a new position
        /// </summary>
        Moving = 2,

        /// <summary>
        /// The cover is open
        /// </summary>
        Open = 3,

        /// <summary>
        /// The state of the cover is unknown
        /// </summary>
        Unknown = 4,

        /// <summary>
        /// The device encountered an error when changing state
        /// </summary>
        Error = 5
    }
}
