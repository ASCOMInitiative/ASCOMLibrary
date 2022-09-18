using System.Collections.Generic;

namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Describe a rate at which the telescope may be moved about the specified axis by the MoveAxis(TelescopeAxes, Double) method.
    /// </summary>
    public class VideoFrame : IVideoFrame
    {
        /// <summary>
        /// Represents a single video frame
        /// </summary>
        /// <param name="ImageArray">The image array from the frame</param>
        /// <param name="PreviewBitmap">The frame's preview bitmap</param>
        /// <param name="FrameNumber">The frame's frame number</param>
        /// <param name="ExposureDuration">The frame's exposure duration</param>
        /// <param name="ExposureStartTime">The frame's start time</param>
        /// <param name="ImageMetadata">The frame's descriptive metadata</param>
        public VideoFrame(object ImageArray, byte[] PreviewBitmap, long FrameNumber, double ExposureDuration, string ExposureStartTime, IList<KeyValuePair<string, string>> ImageMetadata)
        {
            this.ImageArray = ImageArray;
            this.PreviewBitmap = PreviewBitmap;
            this.FrameNumber = FrameNumber;
            this.ExposureDuration = ExposureDuration;
            this.ExposureStartTime = ExposureStartTime;
            this.ImageMetadata = ImageMetadata;
        }

        /// <summary>
        /// The frame as an image array
        /// </summary>
        public object ImageArray
        {
            get;
            private set;
        }

        /// <summary>
        /// The frame's preview bitmap
        /// </summary>
        public byte[] PreviewBitmap
        {
            get;
            private set;
        }

        /// <summary>
        /// The frame's frame number
        /// </summary>
        public long FrameNumber
        {
            get;
            private set;
        }

        /// <summary>
        /// The frame's exposure duration
        /// </summary>
        public double ExposureDuration
        {
            get;
            private set;
        }

        /// <summary>
        /// The frame's start time
        /// </summary>
        public string ExposureStartTime
        {
            get;
            private set;
        }

        /// <summary>
        /// The frame's descriptive metadata
        /// </summary>
        public IList<KeyValuePair<string, string>> ImageMetadata
        {
            get;
            private set;
        }
    }
}