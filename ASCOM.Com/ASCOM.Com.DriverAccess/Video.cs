using ASCOM.Common.DeviceInterfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ASCOM.Common;

namespace ASCOM.Com.DriverAccess
{
    //Note: Currently Video is not a part of ASCOM Standard.
    public class Video : ASCOMDevice, IVideo
    {
        public static List<ASCOMRegistration> Videos => Profile.GetDrivers(DeviceTypes.Video);

        public Video(string ProgID) : base(ProgID)
        {

        }

        public int BitDepth => base.Device.BitDepth;

        public VideoCameraState CameraState => (VideoCameraState)base.Device.CameraState;

        public bool CanConfigureDeviceProperties => base.Device.CanConfigureDeviceProperties;

        public double ExposureMax => base.Device.ExposureMax;

        public double ExposureMin => base.Device.ExposureMin;

        public VideoCameraFrameRate FrameRate => (VideoCameraFrameRate)base.Device.FrameRate;

        public short Gain
        {
            get => base.Device.Gain;
            set => base.Device.Gain = value;
        }

        public short GainMax => base.Device.GainMax;

        public short GainMin => base.Device.GainMin;

        public IList<string> Gains => (base.Device.Gains as IEnumerable).Cast<string>().ToList();

        public short Gamma
        {
            get => base.Device.Gamma;
            set => base.Device.Gamma = value;
        }

        public short GammaMax => base.Device.GammaMax;

        public short GammaMin => base.Device.GammaMin;

        public IList<string> Gammas => (base.Device.Gammas as IEnumerable).Cast<string>().ToList();

        public int Height => base.Device.Height;

        public int IntegrationRate
        {
            get => base.Device.IntegrationRate;
            set => base.Device.IntegrationRate = value;
        }

        public IVideoFrame LastVideoFrame
        {
            get 
            {
                var frame = base.Device.LastVideoFrame;

                //Convert the ASCOM KeyValuePair to the .Net System version
                List<KeyValuePair<string, string>> metadata = new List<KeyValuePair<string, string>>();
                foreach(var pair in frame.ImageMetadata)
                {
                    metadata.Add(new KeyValuePair<string, string>(pair.Key(), pair.Value()));
                }

                return new VideoFrame(frame.ImageArray, frame.PreviewBitmap, frame.FrameNumber, frame.ExposureDuration, frame.ExposureStartTime, metadata);
            }
        }

        public double PixelSizeX => base.Device.PixelSizeX;

        public double PixelSizeY => base.Device.PixelSizeY;

        public string SensorName => base.Device.SensorName;

        public SensorType SensorType => (SensorType)base.Device.SensorType;

        public IList<double> SupportedIntegrationRates => (base.Device.SupportedIntegrationRates as IEnumerable).Cast<double>().ToList();

        public string VideoCaptureDeviceName => base.Device.VideoCaptureDeviceName;

        public string VideoCodec => base.Device.VideoCodec;

        public string VideoFileFormat => base.Device.VideoFileFormat;

        public int VideoFramesBufferSize => base.Device.VideoFramesBufferSize;

        public int Width => base.Device.Width;

        public void ConfigureDeviceProperties()
        {
            base.Device.ConfigureDeviceProperties();
        }

        public string StartRecordingVideoFile(string PreferredFileName)
        {
            return base.Device.StartRecordingVideoFile(PreferredFileName);
        }

        public void StopRecordingVideoFile()
        {
            base.Device.StopRecordingVideoFile();
        }
    }
}