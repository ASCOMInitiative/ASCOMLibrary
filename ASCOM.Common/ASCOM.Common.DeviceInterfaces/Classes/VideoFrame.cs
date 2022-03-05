using System.Collections.Generic;

namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Describe a rate at which the telescope may be moved about the specified axis by the MoveAxis(TelescopeAxes, Double) method.
    /// </summary>
    public class VideoFrame : IVideoFrame
    {
        public VideoFrame(object ImageArray, byte[] PreviewBitmap, long FrameNumber, double ExposureDuration, string ExposureStartTime, IList<KeyValuePair<string, string>> ImageMetadata)
        {
            this.ImageArray = ImageArray;
            this.PreviewBitmap = PreviewBitmap;
            this.FrameNumber = FrameNumber;
            this.ExposureDuration = ExposureDuration;
            this.ExposureStartTime = ExposureStartTime;
            this.ImageMetadata = ImageMetadata;
        }

        public object ImageArray
        {
            get;
            private set;
        }

        public byte[] PreviewBitmap
        {
            get;
            private set;
        }
        public long FrameNumber
        {
            get;
            private set;
        }

        public double ExposureDuration
        {
            get;
            private set;
        }

        public string ExposureStartTime
        {
            get;
            private set;
        }

        public IList<KeyValuePair<string, string>> ImageMetadata
        {
            get;
            private set;
        }
    }
}