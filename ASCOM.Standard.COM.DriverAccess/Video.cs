using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASCOM.Standard.COM.DriverAccess
{
    //Note: Currently Video is not a part of ASCOM Standard.
    public class Video : ASCOMDevice
    {
        public static List<ASCOMRegistration> Videos => ProfileAccess.GetDrivers(DriverTypes.Video);

        public Video(string ProgID) : base(ProgID)
        {

        }

        public int BitDepth => base.Device.BitDepth;

        //ToDo add this type to ASCOM Standard
        public object CameraState => base.Device.CameraState;

        public bool CanConfigureDeviceProperties => base.Device.CanConfigureDeviceProperties;

        public double ExposureMax => base.Device.ExposureMax;

        public double ExposureMin => base.Device.ExposureMin;

        //ToDo add this type to ASCOM Standard
        public object FrameRate => base.Device.FrameRate;

        public short Gain
        {
            get => base.Device.Gain;
            set => base.Device.Gain = value;
        }

        public short GainMax => base.Device.GainMax;

        public short GainMin => base.Device.GainMin;

        public List<string> Gains => (base.Device.Gains as IEnumerable).Cast<string>().ToList();

        public short Gamma
        {
            get => base.Device.Gamma;
            set => base.Device.Gamma = value;
        }

        public short GammaMax => base.Device.GammaMax;

        public short GammaMin => base.Device.GammaMin;

        public List<string> Gammas => (base.Device.Gammas as IEnumerable).Cast<string>().ToList();

        public int Height => base.Device.Height;

        public int IntegrationRate => base.Device.IntegrationRate;

        //ToDo add this type to ASCOM Standard
        public object LastVideoFrame => base.Device.LastVideoFrame;

        public double PixelSizeX => base.Device.PixelSizeX;

        public double PixelSizeY => base.Device.PixelSizeY;

        public string SensorName => base.Device.SensorName;

        //ToDo add this type to ASCOM Standard
        public object SensorType => base.Device.SensorType;

        public List<double> SupportedIntegrationRates => (base.Device.SupportedIntegrationRates as IEnumerable).Cast<double>().ToList();

        public string VideoCaptureDeviceName => base.Device.VideoCaptureDeviceName;

        public string VideoCodec => base.Device.VideoCodec;

        public string VideoFileFormat => base.Device.VideoFileFormat;

        public int VideoFramesBufferSize => base.Device.VideoFramesBufferSize;

        public int Width => base.Device.Width;

        public void ConfigureDeviceProperties()
        {
            base.Device.ConfigureDeviceProperties();
        }

        public void StartRecordingVideoFile(string PreferredFileName)
        {
            base.Device.StartRecordingVideoFile(PreferredFileName);
        }

        public void StopRecordingVideoFile()
        {
            base.Device.StopRecordingVideoFile();
        }
    }
}