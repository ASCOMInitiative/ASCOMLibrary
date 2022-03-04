namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// ASCOM Video Camera status values.
    /// </summary>
    public enum VideoCameraState
    {
        /// <summary>
        /// Camera status running. The video is receiving signal and video frames are available for viewing or recording.
        /// </summary>
        Running = 0,

        /// <summary>
        /// Camera status recording. The video camera is recording video to the file system. Video frames are available for viewing.
        /// </summary>
        Recording = 1,

        /// <summary>
        /// Camera status error. The video camera is in a state of an error and cannot continue its operation. Usually a reset will be required to resolve the error condition.
        /// </summary>
        Error = 2
    }
}