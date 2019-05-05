namespace ASCOM.Alpaca.Devices
{
    /// <summary>
    /// Device type enumeration
    /// </summary>
    public enum DeviceType
    {
        /// <summary>
        /// Switch device
        /// </summary>
        Switch = 0,

        /// <summary>
        /// SafetyMonitor device
        /// </summary>
        SafetyMonitor = 1,

        /// <summary>
        /// Dome device
        /// </summary>
        Dome = 2,

        /// <summary>
        /// Camera device
        /// </summary>
        Camera = 3,

        /// <summary>
        /// ObservingConditions device
        /// </summary>
        ObservingConditions = 4,

        /// <summary>
        /// FilterWheel device
        /// </summary>
        FilterWheel = 5,

        /// <summary>
        /// Focuser device
        /// </summary>
        Focuser = 6,

        /// <summary>
        /// Rotator device
        /// </summary>
        Rotator = 7,

        /// <summary>
        /// Telescope device
        /// </summary>
        Telescope = 8
    }
}