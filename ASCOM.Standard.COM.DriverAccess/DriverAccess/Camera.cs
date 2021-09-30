using ASCOM.Common.DeviceInterfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ASCOM.Com.DriverAccess
{
    public class Camera : ASCOMDevice, ICameraV3
    {
        public static List<ASCOMRegistration> Cameras => ProfileAccess.GetDrivers(DriverTypes.Camera);

        public Camera(string ProgID) : base(ProgID)
        {

        }

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

        public short BinX { get => base.Device.BinX; set => base.Device.BinX = value; }
        public short BinY { get => base.Device.BinY; set => base.Device.BinY = value; }

        public CameraState CameraState => (CameraState)base.Device.CameraState;

        public int CameraXSize => base.Device.CameraXSize;

        public int CameraYSize => base.Device.CameraYSize;

        public bool CanAbortExposure => base.Device.CanAbortExposure;

        public bool CanAsymmetricBin => base.Device.CanAsymmetricBin;

        public bool CanGetCoolerPower => base.Device.CanGetCoolerPower;

        public bool CanPulseGuide => base.Device.CanPulseGuide;

        public bool CanSetCCDTemperature => base.Device.CanSetCCDTemperature;

        public bool CanStopExposure => base.Device.CanStopExposure;

        public double CCDTemperature => base.Device.CCDTemperature;

        public bool CoolerOn { get => base.Device.CoolerOn; set => base.Device.CoolerOn = value; }

        public double CoolerPower => base.Device.CoolerPower;

        public double ElectronsPerADU => base.Device.ElectronsPerADU;

        public double FullWellCapacity => base.Device.FullWellCapacity;

        public bool HasShutter => base.Device.HasShutter;

        public double HeatSinkTemperature => base.Device.HeatSinkTemperature;

        public object ImageArray => base.Device.ImageArray;

        public object ImageArrayVariant => base.Device.ImageArrayVariant;

        public bool ImageReady => base.Device.ImageReady;

        public bool IsPulseGuiding => base.Device.IsPulseGuiding;

        public double LastExposureDuration => base.Device.LastExposureDuration;

        public string LastExposureStartTime => base.Device.LastExposureStartTime;

        public int MaxADU => base.Device.MaxADU;

        public short MaxBinX => base.Device.MaxBinX;

        public short MaxBinY => base.Device.MaxBinY;

        public int NumX { get => base.Device.NumX; set => base.Device.NumX = value; }
        public int NumY { get => base.Device.NumY; set => base.Device.NumY = value; }

        public double PixelSizeX => base.Device.PixelSizeX;

        public double PixelSizeY => base.Device.PixelSizeY;

        public double SetCCDTemperature { get => base.Device.SetCCDTemperature; set => base.Device.SetCCDTemperature = value; }
        public int StartX { get => base.Device.StartX; set => base.Device.StartX = value; }
        public int StartY { get => base.Device.StartY; set => base.Device.StartY = value; }

        public short BayerOffsetX
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new PropertyNotImplementedException("BayerOffsetX is only supported by Interface Versions 2 and above.");
                }
                return base.Device.BayerOffsetX;
            }
        }

        public short BayerOffsetY
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new PropertyNotImplementedException("BayerOffsetY is only supported by Interface Versions 2 and above.");
                }
                return base.Device.BayerOffsetY;
            }
        }


        public bool CanFastReadout
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    return false;
                }
                return base.Device.CanFastReadout;
            }
        }

        public double ExposureMax
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new PropertyNotImplementedException("ExposureMax is only supported by Interface Versions 2 and above.");
                }
                return base.Device.ExposureMax;
            }
        }

        public double ExposureMin
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new PropertyNotImplementedException("ExposureMin is only supported by Interface Versions 2 and above.");
                }
                return base.Device.ExposureMin;
            }
        }

        public double ExposureResolution
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new PropertyNotImplementedException("ExposureResolution is only supported by Interface Versions 2 and above.");
                }
                return base.Device.ExposureResolution;
            }
        }

        public bool FastReadout
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new PropertyNotImplementedException("FastReadout is only supported by Interface Versions 2 and above.");
                }
                return base.Device.FastReadout;
            }

            set
            {
                if (InterfaceVersion < 2)
                {
                    throw new PropertyNotImplementedException("FastReadout is only supported by Interface Versions 2 and above.");
                }
                base.Device.FastReadout = value;
            }
        }

        public short Gain
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new PropertyNotImplementedException("Gain is only supported by Interface Versions 2 and above.");
                }
                return base.Device.Gain;
            }

            set
            {
                if (InterfaceVersion < 2)
                {
                    throw new PropertyNotImplementedException("Gain is only supported by Interface Versions 2 and above.");
                }
                base.Device.Gain = value;
            }
        }

        public short GainMax
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new PropertyNotImplementedException("GainMax is only supported by Interface Versions 2 and above.");
                }
                return base.Device.GainMax;
            }
        }

        public short GainMin
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new PropertyNotImplementedException("GainMin is only supported by Interface Versions 2 and above.");
                }
                return base.Device.GainMin;
            }
        }

        public IList<string> Gains
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new PropertyNotImplementedException("Gains is only supported by Interface Versions 2 and above.");
                }
                return (Device.Gains as IEnumerable).Cast<string>().ToList();
            }
        }

        public short PercentCompleted
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new PropertyNotImplementedException("PercentCompleted is only supported by Interface Versions 2 and above.");
                }
                return base.Device.PercentCompleted;
            }
        }

        public short ReadoutMode
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new PropertyNotImplementedException("ReadoutMode is only supported by Interface Versions 2 and above.");
                }
                return base.Device.ReadoutMode;
            }

            set
            {
                if (InterfaceVersion < 2)
                {
                    throw new PropertyNotImplementedException("ReadoutMode is only supported by Interface Versions 2 and above.");
                }
                base.Device.ReadoutMode = value;
            }
        }

        public IList<string> ReadoutModes
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new PropertyNotImplementedException("ReadoutModes is only supported by Interface Versions 2 and above.");
                }
                return (Device.ReadoutModes as IEnumerable).Cast<string>().ToList();
            }
        }

        public string SensorName
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new PropertyNotImplementedException("SensorName is only supported by Interface Versions 2 and above.");
                }
                return base.Device.SensorName;
            }
        }

        public SensorType SensorType
        {
            get
            {
                if (InterfaceVersion < 2)
                {
                    throw new PropertyNotImplementedException("SensorType is only supported by Interface Versions 2 and above.");
                }
                return (SensorType)base.Device.SensorType;
            }
        }

        public int Offset
        {
            get
            {
                if (InterfaceVersion < 3)
                {
                    throw new PropertyNotImplementedException("Offset is only supported by Interface Versions 3 and above.");
                }
                return base.Device.Offset;
            }

            set
            {
                if (InterfaceVersion < 3)
                {
                    throw new PropertyNotImplementedException("Offset is only supported by Interface Versions 3 and above.");
                }
                base.Device.Offset = value;
            }
        }

        public int OffsetMax
        {
            get
            {
                if (InterfaceVersion < 3)
                {
                    throw new PropertyNotImplementedException("OffsetMax is only supported by Interface Versions 3 and above.");
                }
                return base.Device.OffsetMax;
            }
        }

        public int OffsetMin
        {
            get
            {
                if (InterfaceVersion < 3)
                {
                    throw new PropertyNotImplementedException("OffsetMin is only supported by Interface Versions 3 and above.");
                }
                return base.Device.OffsetMin;
            }
        }

        public IList<string> Offsets
        {
            get
            {
                if (InterfaceVersion < 3)
                {
                    throw new PropertyNotImplementedException("Offsets is only supported by Interface Versions 3 and above.");
                }
                return (Device.Offsets as IEnumerable).Cast<string>().ToList();
            }
        }

        public double SubExposureDuration
        {
            get
            {
                if (InterfaceVersion < 3)
                {
                    throw new PropertyNotImplementedException("SubExposureDuration is only supported by Interface Versions 3 and above.");
                }
                return base.Device.SubExposureDuration;
            }

            set
            {
                if (InterfaceVersion < 3)
                {
                    throw new PropertyNotImplementedException("SubExposureDuration is only supported by Interface Versions 3 and above.");
                }
                base.Device.SubExposureDuration = value;
            }
        }

        public void AbortExposure()
        {
            base.Device.AbortExposure();
        }

        public void PulseGuide(GuideDirection Direction, int Duration)
        {
            base.Device.PulseGuide(Direction, Duration);
        }

        public void StartExposure(double Duration, bool Light)
        {
            base.Device.StartExposure(Duration, Light);
        }

        public void StopExposure()
        {
            base.Device.StopExposure();
        }
    }
}
