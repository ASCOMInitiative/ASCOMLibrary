namespace ASCOM.Common.DeviceInterfaces
{
    public interface ICoverCalibratorV2  : IAscomDeviceV2, ICoverCalibratorV1
    {
        /// <summary>
        /// True while the calibrator brightness is changing.
        /// </summary>
        bool CalibratorChanging { get; }

        /// <summary>
        /// True while the cover is in motion.
        /// </summary>
        bool CoverMoving { get; }
    }
}