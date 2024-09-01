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
        /// <remarks>This is the completion variable used to monitor progress of the <see cref="ICoverCalibratorV1.Brightness"/> method.</remarks>
        bool CalibratorChanging { get; }

        /// <summary>
        /// True while the cover is in motion.
        /// </summary>
        /// <remarks>This is the completion variable used to monitor progress of the <see cref="ICoverCalibratorV1.OpenCover"/> and <see cref="ICoverCalibratorV1.CloseCover"/> methods.</remarks>
        bool CoverMoving { get; }
    }
}