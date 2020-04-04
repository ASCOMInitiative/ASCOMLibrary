using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Alpaca.Interfaces
{
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

    /// <summary>
    /// Describes the state of a calibration device
    /// </summary>
    /// <remarks>This has an "Unknown" state because the device may not be able to determine the state of the hardware at power up if it doesn't provide this feedback and needs 
    /// to be commanded into a known state before use.</remarks>
    public enum CalibratorStatus
    {
        /// <summary>
        /// This device does not have a calibration capability
        /// </summary>
        NotPresent = 0,

        /// <summary>
        /// The calibrator is off
        /// </summary>
        Off = 1,

        /// <summary>
        /// The calibrator is stabilising or is not yet in the commanded state
        /// </summary>
        NotReady = 2,

        /// <summary>
        /// The calibrator is ready for use
        /// </summary>
        Ready = 3,

        /// <summary>
        /// The calibrator state is unknown
        /// </summary>
        Unknown = 4,

        /// <summary>
        /// The calibrator encountered an error when changing state
        /// </summary>
        Error = 5
    }
}
