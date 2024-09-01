using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ASCOM.Common.DeviceStateClasses;
using ASCOM.Common.Interfaces;

namespace ASCOM.Com.DriverAccess
{
    /// <summary>
    /// Camera device class
    /// </summary>
    public class Camera : ASCOMDevice, ICameraV4
    {

        #region Convenience members

        /// <summary>
        /// Return a list of all Cameras registered in the ASCOM Profile
        /// </summary>
        public static List<ASCOMRegistration> Cameras => Profile.GetDrivers(DeviceTypes.Camera);

        /// <summary>
        /// Camera device state
        /// </summary>
        public CameraDeviceState CameraDeviceState
        {
            get
            {
                // Create a state object to return.
                CameraDeviceState cameraDeviceState = new CameraDeviceState(DeviceState, TL);
                TL.LogMessage(LogLevel.Debug, "CameraDeviceState", $"Returning: '{cameraDeviceState.CameraState}' '{cameraDeviceState.CCDTemperature}' '{cameraDeviceState.CoolerPower}' '{cameraDeviceState.HeatSinkTemperature}' '{cameraDeviceState.ImageReady}' '{cameraDeviceState.PercentCompleted}' '{cameraDeviceState.TimeStamp}'");

                // Return the device specific state class
                return cameraDeviceState;
            }
        }

        #endregion

        #region Initialisers

        /// <summary>
        /// Initialise Camera device
        /// </summary>
        /// <param name="ProgID">ProgID of the driver</param>
        public Camera(string ProgID) : base(ProgID)
        {
            deviceType = DeviceTypes.Camera;
            TL = null;
        }

        /// <summary>
        /// Initialise Camera device with a debug logger
        /// </summary>
        /// <param name="ProgID">ProgID of the driver</param>
        /// <param name="logger">Logger instance to receive debug information.</param>
        public Camera(string ProgID, ILogger logger) : base(ProgID)
        {
            deviceType = DeviceTypes.Camera;
            TL = logger;
        }

        #endregion

        #region ICameraV3 and ICameraV4

        /// <inheritdoc/>
        public new string DriverInfo
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return string.Empty;
                }
                return base.DriverInfo;
            }
        }

        /// <inheritdoc/>
        public new string DriverVersion
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return string.Empty;
                }
                return base.DriverVersion;
            }
        }

        /// <inheritdoc/>
        public new string Name
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return string.Empty;
                }
                return base.Name;
            }
        }

        /// <inheritdoc/>
        public short BinX { get => Device.BinX; set => Device.BinX = value; }

        /// <inheritdoc/>
        public short BinY { get => Device.BinY; set => Device.BinY = value; }

        /// <inheritdoc/>
        public CameraState CameraState => (CameraState)Device.CameraState;

        /// <inheritdoc/>
        public int CameraXSize => Device.CameraXSize;

        /// <inheritdoc/>
        public int CameraYSize => Device.CameraYSize;

        /// <inheritdoc/>
        public bool CanAbortExposure => Device.CanAbortExposure;

        /// <inheritdoc/>
        public bool CanAsymmetricBin => Device.CanAsymmetricBin;

        /// <inheritdoc/>
        public bool CanGetCoolerPower => Device.CanGetCoolerPower;

        /// <inheritdoc/>
        public bool CanPulseGuide => Device.CanPulseGuide;

        /// <inheritdoc/>
        public bool CanSetCCDTemperature => Device.CanSetCCDTemperature;

        /// <inheritdoc/>
        public bool CanStopExposure => Device.CanStopExposure;

        /// <inheritdoc/>
        public double CCDTemperature => Device.CCDTemperature;

        /// <inheritdoc/>
        public bool CoolerOn { get => Device.CoolerOn; set => Device.CoolerOn = value; }

        /// <inheritdoc/>
        public double CoolerPower => Device.CoolerPower;

        /// <inheritdoc/>
        public double ElectronsPerADU => Device.ElectronsPerADU;

        /// <inheritdoc/>
        public double FullWellCapacity => Device.FullWellCapacity;

        /// <inheritdoc/>
        public bool HasShutter => Device.HasShutter;

        /// <inheritdoc/>
        public double HeatSinkTemperature => Device.HeatSinkTemperature;

        /// <inheritdoc/>
        public object ImageArray => Device.ImageArray;

        /// <inheritdoc/>
        public object ImageArrayVariant => Device.ImageArrayVariant;

        /// <inheritdoc/>
        public bool ImageReady => Device.ImageReady;

        /// <inheritdoc/>
        public bool IsPulseGuiding => Device.IsPulseGuiding;

        /// <inheritdoc/>
        public double LastExposureDuration => Device.LastExposureDuration;

        /// <inheritdoc/>
        public string LastExposureStartTime => Device.LastExposureStartTime;

        /// <inheritdoc/>
        public int MaxADU => Device.MaxADU;

        /// <inheritdoc/>
        public short MaxBinX => Device.MaxBinX;

        /// <inheritdoc/>
        public short MaxBinY => Device.MaxBinY;

        /// <inheritdoc/>
        public int NumX { get => Device.NumX; set => Device.NumX = value; }

        /// <inheritdoc/>
        public int NumY { get => Device.NumY; set => Device.NumY = value; }

        /// <inheritdoc/>
        public double PixelSizeX => Device.PixelSizeX;

        /// <inheritdoc/>
        public double PixelSizeY => Device.PixelSizeY;

        /// <inheritdoc/>
        public double SetCCDTemperature { get => Device.SetCCDTemperature; set => Device.SetCCDTemperature = value; }

        /// <inheritdoc/>
        public int StartX { get => Device.StartX; set => Device.StartX = value; }

        /// <inheritdoc/>
        public int StartY { get => Device.StartY; set => Device.StartY = value; }

        /// <inheritdoc/>
        public short BayerOffsetX
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new ASCOM.NotImplementedException("BayerOffsetX is only supported by Interface Versions 2 and above.");
                }
                return Device.BayerOffsetX;
            }
        }

        /// <inheritdoc/>
        public short BayerOffsetY
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new ASCOM.NotImplementedException("BayerOffsetY is only supported by Interface Versions 2 and above.");
                }
                return Device.BayerOffsetY;
            }
        }

        /// <inheritdoc/>
        public bool CanFastReadout
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    return false;
                }
                return Device.CanFastReadout;
            }
        }

        /// <inheritdoc/>
        public double ExposureMax
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new ASCOM.NotImplementedException("ExposureMax is only supported by Interface Versions 2 and above.");
                }
                return Device.ExposureMax;
            }
        }

        /// <inheritdoc/>
        public double ExposureMin
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new ASCOM.NotImplementedException("ExposureMin is only supported by Interface Versions 2 and above.");
                }
                return Device.ExposureMin;
            }
        }

        /// <inheritdoc/>
        public double ExposureResolution
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new ASCOM.NotImplementedException("ExposureResolution is only supported by Interface Versions 2 and above.");
                }
                return Device.ExposureResolution;
            }
        }

        /// <inheritdoc/>
        public bool FastReadout
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new ASCOM.NotImplementedException("FastReadout is only supported by Interface Versions 2 and above.");
                }
                return Device.FastReadout;
            }

            set
            {
                if (InterfaceVersion < 2)
                {
                    throw new ASCOM.NotImplementedException("FastReadout is only supported by Interface Versions 2 and above.");
                }
                Device.FastReadout = value;
            }
        }

        /// <inheritdoc/>
        public short Gain
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new ASCOM.NotImplementedException("Gain is only supported by Interface Versions 2 and above.");
                }
                return Device.Gain;
            }

            set
            {
                if (InterfaceVersion < 2)
                {
                    throw new ASCOM.NotImplementedException("Gain is only supported by Interface Versions 2 and above.");
                }
                Device.Gain = value;
            }
        }

        /// <inheritdoc/>
        public short GainMax
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new ASCOM.NotImplementedException("GainMax is only supported by Interface Versions 2 and above.");
                }
                return Device.GainMax;
            }
        }

        /// <inheritdoc/>
        public short GainMin
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new ASCOM.NotImplementedException("GainMin is only supported by Interface Versions 2 and above.");
                }
                return Device.GainMin;
            }
        }

        /// <inheritdoc/>
        public IList<string> Gains
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new ASCOM.NotImplementedException("Gains is only supported by Interface Versions 2 and above.");
                }
                // ASCOM.DSLR driver returns an array of integers, not strings. ASCOM Platform hid this issue by converting to strings
                // so this code should forgive the same issue by converting the contents of whatever is returned by the underlying driver to strings
                return (Device.Gains as IEnumerable).OfType<object>().Select(x => x.ToString()).ToList();
            }
        }

        /// <inheritdoc/>
        public short PercentCompleted
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new ASCOM.NotImplementedException("PercentCompleted is only supported by Interface Versions 2 and above.");
                }
                return Device.PercentCompleted;
            }
        }

        /// <inheritdoc/>
        public short ReadoutMode
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new ASCOM.NotImplementedException("ReadoutMode is only supported by Interface Versions 2 and above.");
                }
                return Device.ReadoutMode;
            }

            set
            {
                if (InterfaceVersion < 2)
                {
                    throw new ASCOM.NotImplementedException("ReadoutMode is only supported by Interface Versions 2 and above.");
                }
                Device.ReadoutMode = value;
            }
        }

        /// <inheritdoc/>
        public IList<string> ReadoutModes
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new ASCOM.NotImplementedException("ReadoutModes is only supported by Interface Versions 2 and above.");
                }
                return (Device.ReadoutModes as IEnumerable).Cast<string>().ToList();
            }
        }

        /// <inheritdoc/>
        public string SensorName
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new ASCOM.NotImplementedException("SensorName is only supported by Interface Versions 2 and above.");
                }
                return Device.SensorName;
            }
        }

        /// <inheritdoc/>
        public SensorType SensorType
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new ASCOM.NotImplementedException("SensorType is only supported by Interface Versions 2 and above.");
                }
                return (SensorType)Device.SensorType;
            }
        }

        /// <inheritdoc/>
        public int Offset
        {
            get
            {
                if (InterfaceVersion < 3)
                {
                    throw new ASCOM.NotImplementedException("Offset is only supported by Interface Versions 3 and above.");
                }
                return Device.Offset;
            }

            set
            {
                if (InterfaceVersion < 3)
                {
                    throw new ASCOM.NotImplementedException("Offset is only supported by Interface Versions 3 and above.");
                }
                Device.Offset = value;
            }
        }

        /// <inheritdoc/>
        public int OffsetMax
        {
            get
            {
                if (InterfaceVersion < 3)
                {
                    throw new ASCOM.NotImplementedException("OffsetMax is only supported by Interface Versions 3 and above.");
                }
                return Device.OffsetMax;
            }
        }

        /// <inheritdoc/>
        public int OffsetMin
        {
            get
            {
                if (InterfaceVersion < 3)
                {
                    throw new ASCOM.NotImplementedException("OffsetMin is only supported by Interface Versions 3 and above.");
                }
                return Device.OffsetMin;
            }
        }

        /// <inheritdoc/>
        public IList<string> Offsets
        {
            get
            {
                if (InterfaceVersion < 3)
                {
                    throw new ASCOM.NotImplementedException("Offsets is only supported by Interface Versions 3 and above.");
                }
                return (Device.Offsets as IEnumerable).Cast<string>().ToList();
            }
        }

        /// <inheritdoc/>
        public double SubExposureDuration
        {
            get
            {
                if (InterfaceVersion < 3)
                {
                    throw new ASCOM.NotImplementedException("SubExposureDuration is only supported by Interface Versions 3 and above.");
                }
                return Device.SubExposureDuration;
            }

            set
            {
                if (InterfaceVersion < 3)
                {
                    throw new ASCOM.NotImplementedException("SubExposureDuration is only supported by Interface Versions 3 and above.");
                }
                Device.SubExposureDuration = value;
            }
        }

        /// <inheritdoc/>
        public void AbortExposure()
        {
            Device.AbortExposure();
        }

        /// <inheritdoc/>
        public void PulseGuide(GuideDirection Direction, int Duration)
        {
            Device.PulseGuide(Direction, Duration);
        }

        /// <inheritdoc/>
        public void StartExposure(double Duration, bool Light)
        {
            Device.StartExposure(Duration, Light);
        }

        /// <inheritdoc/>
        public void StopExposure()
        {
            Device.StopExposure();
        }

        #endregion

    }
}
