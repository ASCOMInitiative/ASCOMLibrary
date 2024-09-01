using ASCOM.Common.DeviceInterfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ASCOM.Common;
using ASCOM.Common.Interfaces;
using ASCOM.Common.DeviceStateClasses;

namespace ASCOM.Com.DriverAccess
{
    //Note: Currently Video is not a part of ASCOM Standard.
    /// <summary>
    /// Video device class
    /// </summary>
    public class Video : ASCOMDevice, IVideoV2
    {

        #region Convenience members

        /// <summary>
        /// Return a list of all Video devices registered in the ASCOM Profile
        /// </summary>
        public static List<ASCOMRegistration> Videos => Profile.GetDrivers(DeviceTypes.Video);

        /// <summary>
        /// VideoState device state
        /// </summary>
        public VideoState VideoState
        {
            get
            {
                // Create a state object to return.
                VideoState videoState = new VideoState(DeviceState, TL);
                TL.LogMessage(LogLevel.Debug,nameof(VideoState), $"Returning: " +
                    $"CameraState: '{videoState.CameraState}', " +
                    $"Time stamp: '{videoState.TimeStamp}'");

                // Return the device specific state class
                return videoState;
            }
        }

        #endregion

        #region Initialisers

        /// <summary>
        /// Initialise Video device
        /// </summary>
        /// <param name="ProgID">COM ProgID of the device.</param>
        public Video(string ProgID) : base(ProgID)
        {
            deviceType = DeviceTypes.Video;
        }

        /// <summary>
        /// Initialise Video device with a debug logger
        /// </summary>
        /// <param name="ProgID">ProgID of the driver</param>
        /// <param name="logger">Logger instance to receive debug information.</param>
        public Video(string ProgID, ILogger logger) : base(ProgID)
        {
            deviceType = DeviceTypes.Video;
            TL = logger;
        }

        #endregion

        #region IVideoV1 and IVideov2

        /// <inheritdoc/>
        public int BitDepth => Device.BitDepth;

        /// <inheritdoc/>
        public VideoCameraState CameraState => (VideoCameraState)Device.CameraState;

        /// <inheritdoc/>
        public bool CanConfigureDeviceProperties => Device.CanConfigureDeviceProperties;

        /// <inheritdoc/>
        public double ExposureMax => Device.ExposureMax;

        /// <inheritdoc/>
        public double ExposureMin => Device.ExposureMin;

        /// <inheritdoc/>
        public VideoCameraFrameRate FrameRate => (VideoCameraFrameRate)Device.FrameRate;

        /// <inheritdoc/>
        public short Gain
        {
            get => Device.Gain;
            set => Device.Gain = value;
        }

        /// <inheritdoc/>
        public short GainMax => Device.GainMax;

        /// <inheritdoc/>
        public short GainMin => Device.GainMin;

        /// <inheritdoc/>
        public IList<string> Gains => (Device.Gains as IEnumerable).Cast<string>().ToList();

        /// <inheritdoc/>
        public short Gamma
        {
            get => Device.Gamma;
            set => Device.Gamma = value;
        }

        /// <inheritdoc/>
        public short GammaMax => Device.GammaMax;

        /// <inheritdoc/>
        public short GammaMin => Device.GammaMin;

        /// <inheritdoc/>
        public IList<string> Gammas => (Device.Gammas as IEnumerable).Cast<string>().ToList();

        /// <inheritdoc/>
        public int Height => Device.Height;

        /// <inheritdoc/>
        public int IntegrationRate
        {
            get => Device.IntegrationRate;
            set => Device.IntegrationRate = value;
        }

        /// <inheritdoc/>
        public IVideoFrame LastVideoFrame
        {
            get
            {
                var frame = Device.LastVideoFrame;

                //Convert the ASCOM KeyValuePair to the .Net System version
                List<KeyValuePair<string, string>> metadata = new List<KeyValuePair<string, string>>();
                foreach (var pair in frame.ImageMetadata)
                {
                    metadata.Add(new KeyValuePair<string, string>(pair.Key(), pair.Value()));
                }

                // Supply a default 0.0 value if ExposureDuration (an optional member) isn't available
                double exposureDuration;
                try
                {
                    exposureDuration = frame.ExposureDuration;
                }
                catch (System.Exception)
                {
                    exposureDuration = 0.0;
                }

                // Supply a default empty string value if ExposureStartTime (an optional member) isn't available
                string exposureStartTime;
                try
                {
                    exposureStartTime = frame.ExposureStartTime;
                }
                catch (System.Exception)
                {
                    exposureStartTime = "";
                }

                return new VideoFrame(frame.ImageArray, frame.PreviewBitmap, frame.FrameNumber, exposureDuration, exposureStartTime, metadata);
            }
        }

        /// <inheritdoc/>
        public double PixelSizeX => Device.PixelSizeX;

        /// <inheritdoc/>
        public double PixelSizeY => Device.PixelSizeY;

        /// <inheritdoc/>
        public string SensorName => Device.SensorName;

        /// <inheritdoc/>
        public SensorType SensorType => (SensorType)Device.SensorType;

        /// <inheritdoc/>
        public IList<double> SupportedIntegrationRates => (Device.SupportedIntegrationRates as IEnumerable).Cast<double>().ToList();

        /// <inheritdoc/>
        public string VideoCaptureDeviceName => Device.VideoCaptureDeviceName;

        /// <inheritdoc/>
        public string VideoCodec => Device.VideoCodec;

        /// <inheritdoc/>
        public string VideoFileFormat => Device.VideoFileFormat;

        /// <inheritdoc/>
        public int VideoFramesBufferSize => Device.VideoFramesBufferSize;

        /// <inheritdoc/>
        public int Width => Device.Width;

        /// <inheritdoc/>
        public void ConfigureDeviceProperties()
        {
            Device.ConfigureDeviceProperties();
        }

        /// <inheritdoc/>
        public string StartRecordingVideoFile(string PreferredFileName)
        {
            return Device.StartRecordingVideoFile(PreferredFileName);
        }

        /// <inheritdoc/>
        public void StopRecordingVideoFile()
        {
            Device.StopRecordingVideoFile();
        }

        #endregion

    }
}