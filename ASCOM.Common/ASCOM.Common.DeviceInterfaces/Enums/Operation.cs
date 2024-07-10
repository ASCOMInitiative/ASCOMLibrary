namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Device operation names
    /// </summary>
    public enum Operation
    {
        /// <summary>
        /// Uninitialised operation
        /// </summary>
        Uninitialised = 0,

        /// <summary>
        /// All device operations
        /// </summary>
        All = 65535,

        /// <summary>
        /// No operation
        /// </summary>
        None = 1,

        /// <summary>
        /// Connect operation
        /// </summary>
        Connect = 2,

        /// <summary>
        /// Disconnect operation
        /// </summary>
        Disconnect = 3,

        /// <summary>
        /// StartExposure operation
        /// </summary>
        StartExposure = 4,

        /// <summary>
        /// StopExposure operation
        /// </summary>
        StopExposure = 5,

        /// <summary>
        /// AbortExposure operation
        /// </summary>
        AbortExposure = 6,

        /// <summary>
        /// PulseGuide operation
        /// </summary>
        PulseGuide = 7,

        /// <summary>
        /// CalibratorOff operation
        /// </summary>
        CalibratorOff = 8,

        /// <summary>
        /// CalibratorOn operation
        /// </summary>
        CalibratorOn = 9,

        /// <summary>
        /// CloseCover operation
        /// </summary>
        CloseCover = 10,

        /// <summary>
        /// OpenCover operation
        /// </summary>
        OpenCover = 11,

        /// <summary>
        /// HaltCover operation
        /// </summary>
        HaltCover = 12,

        /// <summary>
        /// FindHome operation
        /// </summary>
        FindHome = 13,

        /// <summary>
        /// Park operation
        /// </summary>
        Park = 14,

        /// <summary>
        /// SlewToAzimuth operation
        /// </summary>
        SlewToAzimuth = 15,

        /// <summary>
        /// AbortSlew operation
        /// </summary>
        AbortSlew = 16,

        /// <summary>
        /// AbortSlew operation
        /// </summary>
        CloseShutter = 17,

        /// <summary>
        /// OpenShutter operation
        /// </summary>
        OpenShutter = 18,

        /// <summary>
        /// SlewToAltitude operation
        /// </summary>
        SlewToAltitude = 19,

        /// <summary>
        /// Position operation
        /// </summary>
        Position = 20,

        /// <summary>
        /// Move operation
        /// </summary>
        Move = 21,

        /// <summary>
        /// Halt operation
        /// </summary>
        Halt = 22,

        /// <summary>
        /// MoveAbsolute operation
        /// </summary>
        MoveAbsolute = 23,

        /// <summary>
        /// MoveAbsolute operation
        /// </summary>
        MoveMechanical = 24,

        /// <summary>
        /// SetSwitch operation
        /// </summary>
        SetSwitch = 25,

        /// <summary>
        /// SetSwitchValue operation
        /// </summary>
        SetSwitchValue = 26,

        /// <summary>
        /// Unpark operation
        /// </summary>
        Unpark = 27,

        /// <summary>
        /// MoveAxis operation
        /// </summary>
        MoveAxis = 28,

        /// <summary>
        /// SideOfPier operation
        /// </summary>
        SideOfPier = 29,

        /// <summary>
        /// SlewToAltAzAsync operation
        /// </summary>
        SlewToAltAzAsync = 30,

        /// <summary>
        /// SlewToCoordinatesAsync operation
        /// </summary>
        SlewToCoordinatesAsync = 31,

        /// <summary>
        /// SlewToTargetAsync operation
        /// </summary>
        SlewToTargetAsync = 32
    }
}
