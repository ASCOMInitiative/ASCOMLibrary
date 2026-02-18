namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// CoverCalibrtator interface version 2, which incorporates the new members in IAscomDeviceV2, the members present in ICameraV3 plus new CalibratorRead and CoverMoving properties
    /// </summary>
    public interface ICoverCalibratorV2  : IAscomDeviceV2, ICoverCalibratorV1
    {
        /// <summary>
        /// True while the calibrator brightness is not stable.
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/covercalibrator.html#CoverCalibrator.CalibratorChanging">Canonical Definition</see></remarks>
        bool CalibratorChanging { get; }

        /// <summary>
        /// True while the cover is in motion.
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/covercalibrator.html#CoverCalibrator.CoverMoving">Canonical Definition</see></remarks>
        bool CoverMoving { get; }
    }
}