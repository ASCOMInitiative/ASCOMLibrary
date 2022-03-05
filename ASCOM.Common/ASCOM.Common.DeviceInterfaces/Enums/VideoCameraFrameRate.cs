namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// ASCOM Video Camera supported frame rates.
    /// </summary>
    public enum VideoCameraFrameRate
    {
        /// <summary>
        /// This is a video camera that supports variable frame rates.
        /// </summary>
        Variable = 0,

        /// <summary>
        /// 25 frames per second (fps) corresponding to a <b>PAL</b> (colour) or <b>CCIR</b> (black and white) video standard.
        /// </summary>
        PAL = 1,

        /// <summary>
        /// 29.97  frames per second (fps) corresponding to an <b>NTSC</b> (colour) or <b>EIA</b> (black and white) video standard.
        /// </summary>
        NTSC = 2
    }
}