namespace ASCOM.Common
{
    // NOTE
    // NOTE - Member spelling in this enumeration must be the same as the expected spelling when using the device type as a string.
    // NOTE - If a new device type is added to this enumeration, a corresponding entry must be added to the DeviceTypeNames class
    // NOTE

    /// <summary>
    /// The set of Driver types as of ASCOM 6.5. Rather then directly using strings these are used to request specific types.
    /// </summary>
    public enum DeviceTypes
    {
        Camera,
        CoverCalibrator,
        Dome,
        FilterWheel,
        Focuser,
        ObservingConditions,
        Rotator,
        SafetyMonitor,
        Switch,
        Telescope,
        Video
    }
}
