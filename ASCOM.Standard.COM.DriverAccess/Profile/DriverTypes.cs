using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Com.DriverAccess
{
    /// <summary>
    /// The set of Driver types as of ASCOM 6.5. Rather then directly using strings these are used to request specific types.
    /// </summary>
    public enum DriverTypes
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
