namespace ASCOM.Common.DeviceInterfaces
{
    public interface ICoverCalibratorV2  : IAscomDeviceV2, ICoverCalibratorV1
    {
        /// <summary>
        /// False while the calibrator brightness is not stable.
        /// </summary>
        bool CalibratorReady { get; }

        /// <summary>
        /// True while the cover is in motion.
        /// </summary>
        bool CoverMoving { get; }
    }
}