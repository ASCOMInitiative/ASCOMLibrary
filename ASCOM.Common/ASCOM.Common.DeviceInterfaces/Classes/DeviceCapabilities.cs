using System;
using System.Collections.Generic;

namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Methods that report whether a capability is present in a given device and interface version
    /// </summary>
    public static class DeviceCapabilities
    {
        #region Data Tables

        /// <summary>
        /// Dictionary of the latest interface versions supported by Platform 6
        /// </summary>
        public static Dictionary<DeviceTypes, short> LatestPlatform6Interface = new Dictionary<DeviceTypes, short>()
        {
            { DeviceTypes.Camera, 3 },
            { DeviceTypes.CoverCalibrator, 1 },
            { DeviceTypes.Dome, 2 },
            { DeviceTypes.FilterWheel, 2 },
            { DeviceTypes.Focuser, 3 },
            { DeviceTypes.ObservingConditions, 1 },
            { DeviceTypes.Rotator, 3 },
            { DeviceTypes.SafetyMonitor, 1 },
            { DeviceTypes.Switch, 2 },
            { DeviceTypes.Telescope, 3 },
            { DeviceTypes.Video, 1 }
        };

        /// <summary>
        /// Dictionary of the interface versions at launch of Platform 7
        /// </summary>
        /// <remarks>
        /// These values must not change when new interfaces are added, update the LatestInterface dictionary instead
        /// </remarks>
        public static Dictionary<DeviceTypes, short> InitialPlatform7Interface = new Dictionary<DeviceTypes, short>()
        {
            { DeviceTypes.Camera, 4 },
            { DeviceTypes.CoverCalibrator, 2 },
            { DeviceTypes.Dome, 3 },
            { DeviceTypes.FilterWheel, 3 },
            { DeviceTypes.Focuser, 4 },
            { DeviceTypes.ObservingConditions, 2 },
            { DeviceTypes.Rotator, 4 },
            { DeviceTypes.SafetyMonitor, 3 },
            { DeviceTypes.Switch, 3 },
            { DeviceTypes.Telescope, 4 },
            { DeviceTypes.Video, 2 }
        };

        /// <summary>
        /// Dictionary of the latest interface versions supported by Platform 7
        /// </summary>
        /// <remarks>
        /// Update these values as new interface versions are included in future Platforms
        /// </remarks>
        public static Dictionary<DeviceTypes, short> LatestInterface = new Dictionary<DeviceTypes, short>()
        {
            { DeviceTypes.Camera, 4 },
            { DeviceTypes.CoverCalibrator, 2 },
            { DeviceTypes.Dome, 3 },
            { DeviceTypes.FilterWheel, 3 },
            { DeviceTypes.Focuser, 4 },
            { DeviceTypes.ObservingConditions, 2 },
            { DeviceTypes.Rotator, 4 },
            { DeviceTypes.SafetyMonitor, 3 },
            { DeviceTypes.Switch, 3 },
            { DeviceTypes.Telescope, 4 },
            { DeviceTypes.Video, 2 }
        };

        /// <summary>
        /// Structure representing a specific interface member in a given device interface
        /// </summary>
        public struct Interfacemember
        {
            // Fields
            public DeviceTypes DeviceType;
            public MemberNames MemberName;

            // Initialiser
            public Interfacemember(DeviceTypes deviceType, MemberNames memberName)
            {
                DeviceType = deviceType;
                MemberName = memberName;
            }
        }

        /// <summary>
        /// Returns the interface version in which a given member was introduced. The member is defined by a combination of device type and member name
        /// </summary>
        private static readonly Dictionary<Interfacemember, short> VersionHistory = new Dictionary<Interfacemember, short>()
        { 
            // Camera
            { new Interfacemember(DeviceTypes.Camera, MemberNames.AbortExposure), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.Action), 2 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.BayerOffsetX), 2 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.BayerOffsetY), 2 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.BinX), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.BinY), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.CameraState), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.CameraXSize), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.CameraYSize), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.CanAbortExposure), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.CanAsymmetricBin), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.CanFastReadout), 2 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.CanGetCoolerPower), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.CanPulseGuide), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.CanSetCCDTemperature), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.CanStopExposure), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.CCDTemperature), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.CommandBlind), 2 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.CommandBool), 2 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.CommandString), 2 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.Connect), 4 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.Connected), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.Connecting),4 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.CoolerOn), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.CoolerPower), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.Description), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.DeviceState), 4 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.Disconnect), 4 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.Dispose), 2 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.DriverInfo), 2 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.DriverVersion), 2 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.ElectronsPerADU), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.ExposureMax), 3 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.ExposureMin), 3 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.ExposureResolution), 3 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.FastReadout), 3 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.FullWellCapacity), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.Gain), 2 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.GainMax), 2 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.GainMin), 2 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.Gains), 2 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.HasShutter), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.HeatSinkTemperature), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.ImageArray), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.ImageArrayVariant), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.ImageReady), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.InterfaceVersion), 2 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.IsPulseGuiding), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.LastExposureDuration), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.LastExposureStartTime), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.MaxADU), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.MaxBinX), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.MaxBinY), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.Name), 2 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.NumX), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.NumY), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.Offset), 3 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.OffsetMax), 3 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.OffsetMin), 3 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.Offsets), 3 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.PercentCompleted), 2 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.PixelSizeX), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.PixelSizeY), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.PulseGuide), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.ReadoutMode), 2 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.ReadoutModes), 2 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.SensorName), 2 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.SensorType), 2 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.SetCCDTemperature), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.SetupDialog), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.StartExposure), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.StartX), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.StartY), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.StopExposure), 1 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.SubExposureDuration), 3 },
            { new Interfacemember(DeviceTypes.Camera, MemberNames.SupportedActions), 2 },

            // CoverCalibrator
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.Action), 1 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.Brightness), 1 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.CalibratorChanging), 2 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.CalibratorOff), 1 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.CalibratorOn), 1 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.CalibratorState), 1 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.CloseCover), 1 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.CommandBlind), 1 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.CommandBool), 1 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.CommandString), 1 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.Connect), 2 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.Connected), 1 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.Connecting), 2 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.CoverMoving), 2 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.CoverState), 1 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.Description), 1 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.DeviceState), 2 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.Disconnect), 2 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.Dispose), 1 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.DriverInfo), 1 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.DriverVersion), 1 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.HaltCover), 1 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.InterfaceVersion), 1 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.MaxBrightness), 1 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.Name), 1 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.OpenCover), 1 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.SetupDialog), 1 },
            { new Interfacemember(DeviceTypes.CoverCalibrator, MemberNames.SupportedActions), 1 },

            //Dome
            { new Interfacemember(DeviceTypes.Dome, MemberNames.AbortSlew), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.Action), 2 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.Altitude), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.AtHome), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.AtPark), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.Azimuth), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.CanFindHome), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.CanPark), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.CanSetAltitude), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.CanSetAzimuth), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.CanSetPark), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.CanSetShutter), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.CanSlave), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.CanSyncAzimuth), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.CloseShutter), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.CommandBlind), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.CommandBool), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.CommandString), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.Connect), 3 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.Connected), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.Connecting), 3 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.Description), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.DeviceState), 3 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.Disconnect), 3 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.Dispose), 2 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.DriverInfo), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.DriverVersion), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.FindHome), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.InterfaceVersion), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.Name), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.OpenShutter), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.Park), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.SetPark), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.SetupDialog), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.ShutterStatus), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.Slaved), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.Slewing), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.SlewToAltitude), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.SlewToAzimuth), 1 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.SupportedActions), 2 },
            { new Interfacemember(DeviceTypes.Dome, MemberNames.SyncToAzimuth), 1 },

            // FilterWheel
            { new Interfacemember(DeviceTypes.FilterWheel, MemberNames.Action), 2 },
            { new Interfacemember(DeviceTypes.FilterWheel, MemberNames.CommandBlind), 2 },
            { new Interfacemember(DeviceTypes.FilterWheel, MemberNames.CommandBool), 2 },
            { new Interfacemember(DeviceTypes.FilterWheel, MemberNames.CommandString), 2 },
            { new Interfacemember(DeviceTypes.FilterWheel, MemberNames.Connect), 3 },
            { new Interfacemember(DeviceTypes.FilterWheel, MemberNames.Connected), 1 },
            { new Interfacemember(DeviceTypes.FilterWheel, MemberNames.Connecting), 3 },
            { new Interfacemember(DeviceTypes.FilterWheel, MemberNames.Description), 2 },
            { new Interfacemember(DeviceTypes.FilterWheel, MemberNames.DeviceState), 3 },
            { new Interfacemember(DeviceTypes.FilterWheel, MemberNames.Disconnect), 3 },
            { new Interfacemember(DeviceTypes.FilterWheel, MemberNames.Dispose), 2 },
            { new Interfacemember(DeviceTypes.FilterWheel, MemberNames.DriverInfo), 2 },
            { new Interfacemember(DeviceTypes.FilterWheel, MemberNames.DriverVersion), 2 },
            { new Interfacemember(DeviceTypes.FilterWheel, MemberNames.FocusOffsets), 1 },
            { new Interfacemember(DeviceTypes.FilterWheel, MemberNames.InterfaceVersion), 2 },
            { new Interfacemember(DeviceTypes.FilterWheel, MemberNames.Name), 2 },
            { new Interfacemember(DeviceTypes.FilterWheel, MemberNames.Names), 1 },
            { new Interfacemember(DeviceTypes.FilterWheel, MemberNames.Position), 1 },
            { new Interfacemember(DeviceTypes.FilterWheel, MemberNames.SetupDialog), 1 },
            { new Interfacemember(DeviceTypes.FilterWheel, MemberNames.SupportedActions), 2 },

            //Focuser
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.Absolute), 1 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.Action), 2 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.CommandBlind), 2 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.CommandBool), 2 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.CommandString), 2 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.Connect), 4 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.Connected), 2 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.Connecting), 4 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.Description), 2 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.DeviceState), 4 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.Disconnect), 4 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.Dispose), 2 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.DriverInfo), 2 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.DriverVersion), 2 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.Halt), 1 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.InterfaceVersion), 2 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.IsMoving), 1 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.Link), 1 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.MaxIncrement), 1 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.MaxStep), 1 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.Move), 1 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.Name), 2 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.Position), 1 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.SetupDialog), 1 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.StepSize), 1 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.SupportedActions), 2 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.TempComp), 1 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.TempCompAvailable), 1 },
            { new Interfacemember(DeviceTypes.Focuser, MemberNames.Temperature), 1 },

            // ObservingConditions
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.Action), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.AveragePeriod), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.CloudCover), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.CommandBlind), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.CommandBool), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.CommandString), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.Connect), 2 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.Connected), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.Connecting), 2 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.Description), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.DeviceState), 2 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.DewPoint), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.Disconnect), 2 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.Dispose), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.DriverInfo), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.DriverVersion), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.Humidity), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.InterfaceVersion), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.Name), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.Pressure), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.RainRate), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.Refresh), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.SensorDescription), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.SetupDialog), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.SkyBrightness), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.SkyQuality), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.SkyTemperature), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.StarFWHM), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.SupportedActions), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.Temperature), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.TimeSinceLastUpdate), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.WindDirection), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.WindGust), 1 },
            { new Interfacemember(DeviceTypes.ObservingConditions, MemberNames.WindSpeed), 1 },

            // Rotator
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.Action), 2 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.CanReverse), 2 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.CommandBlind), 2 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.CommandBool), 2 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.CommandString), 2 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.Connect), 4 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.Connected), 1 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.Connecting), 4 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.Description), 2 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.DeviceState), 4 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.Disconnect), 4 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.Dispose), 2 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.DriverInfo), 2 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.DriverVersion), 2 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.Halt), 1 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.InterfaceVersion), 2 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.IsMoving), 1 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.MechanicalPosition), 3 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.Move), 1 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.MoveAbsolute), 1 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.MoveMechanical), 3 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.Name), 2 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.Position), 1 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.Reverse), 1 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.SetupDialog), 1 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.StepSize), 1 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.SupportedActions), 2 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.Sync), 3 },
            { new Interfacemember(DeviceTypes.Rotator, MemberNames.TargetPosition), 1 },

            // SafetyMonitor
            { new Interfacemember(DeviceTypes.SafetyMonitor, MemberNames.Action), 1 },
            { new Interfacemember(DeviceTypes.SafetyMonitor, MemberNames.CommandBlind), 1 },
            { new Interfacemember(DeviceTypes.SafetyMonitor, MemberNames.CommandBool), 1 },
            { new Interfacemember(DeviceTypes.SafetyMonitor, MemberNames.CommandString), 1 },
            { new Interfacemember(DeviceTypes.SafetyMonitor, MemberNames.Connect), 3 },
            { new Interfacemember(DeviceTypes.SafetyMonitor, MemberNames.Connected), 1 },
            { new Interfacemember(DeviceTypes.SafetyMonitor, MemberNames.Connecting), 3 },
            { new Interfacemember(DeviceTypes.SafetyMonitor, MemberNames.Description), 1 },
            { new Interfacemember(DeviceTypes.SafetyMonitor, MemberNames.DeviceState), 3 },
            { new Interfacemember(DeviceTypes.SafetyMonitor, MemberNames.Disconnect), 3 },
            { new Interfacemember(DeviceTypes.SafetyMonitor, MemberNames.Dispose), 1 },
            { new Interfacemember(DeviceTypes.SafetyMonitor, MemberNames.DriverInfo), 1 },
            { new Interfacemember(DeviceTypes.SafetyMonitor, MemberNames.DriverVersion), 1 },
            { new Interfacemember(DeviceTypes.SafetyMonitor, MemberNames.InterfaceVersion), 1 },
            { new Interfacemember(DeviceTypes.SafetyMonitor, MemberNames.IsSafe), 1 },
            { new Interfacemember(DeviceTypes.SafetyMonitor, MemberNames.Name), 1 },
            { new Interfacemember(DeviceTypes.SafetyMonitor, MemberNames.SetupDialog), 1 },
            { new Interfacemember(DeviceTypes.SafetyMonitor, MemberNames.SupportedActions), 1 },

            // Switch
            { new Interfacemember(DeviceTypes.Switch, MemberNames.Action), 2 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.CanWrite), 2 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.CommandBlind), 2 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.CommandBool), 2 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.CommandString), 2 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.Connect), 3 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.Connected), 1 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.Connecting), 3 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.Description), 1 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.DeviceState), 3 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.Disconnect), 3 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.Dispose), 2 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.DriverInfo), 1 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.DriverVersion), 1 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.GetSwitch), 1 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.GetSwitchDescription), 2 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.GetSwitchName), 1 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.GetSwitchValue), 2 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.InterfaceVersion), 1 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.MaxSwitch), 1 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.MaxSwitchValue), 2 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.MinSwitchValue), 2 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.Name), 1 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.SetAsync), 3 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.SetAsyncValue), 3 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.SetSwitch), 1 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.SetSwitchName), 2 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.SetSwitchValue), 2 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.SetupDialog), 1 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.StateChangeComplete), 3 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.SupportedActions), 2 },
            { new Interfacemember(DeviceTypes.Switch, MemberNames.SwitchStep), 2 },

            // Telescope
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.AbortSlew), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.Action), 3 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.AlignmentMode), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.Altitude), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.ApertureArea), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.ApertureDiameter), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.AtHome), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.AtPark), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.AxisRates), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.Azimuth), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.CanFindHome), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.CanMoveAxis), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.CanPark), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.CanPulseGuide), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.CanSetDeclinationRate), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.CanSetGuideRates), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.CanSetPark), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.CanSetPierSide), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.CanSetRightAscensionRate), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.CanSetTracking), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.CanSlew), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.CanSlewAltAz), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.CanSlewAltAzAsync), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.CanSlewAsync), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.CanSync), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.CanSyncAltAz), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.CanUnpark), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.CommandBlind), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.CommandBool), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.CommandString), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.Connect), 4 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.Connected), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.Connecting), 4 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.Declination), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.DeclinationRate), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.Description), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.DestinationSideOfPier), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.DeviceState), 4 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.Disconnect), 4 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.Dispose), 3 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.DoesRefraction), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.DriverInfo), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.DriverVersion), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.EquatorialSystem), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.FindHome), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.FocalLength), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.GuideRateDeclination), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.GuideRateRightAscension), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.InterfaceVersion), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.IsPulseGuiding), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.MoveAxis), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.Name), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.Park), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.PulseGuide), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.RightAscension), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.RightAscensionRate), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.SetPark), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.SetupDialog), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.SideOfPier), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.SiderealTime), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.SiteElevation), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.SiteLatitude), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.SiteLongitude), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.Slewing), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.SlewSettleTime), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.SlewToAltAz), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.SlewToAltAzAsync), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.SlewToCoordinates), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.SlewToCoordinatesAsync), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.SlewToTarget), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.SlewToTargetAsync), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.SupportedActions), 3 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.SyncToAltAz), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.SyncToCoordinates), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.SyncToTarget), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.TargetDeclination), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.TargetRightAscension), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.Tracking), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.TrackingRate), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.TrackingRates), 2 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.Unpark), 1 },
            { new Interfacemember(DeviceTypes.Telescope, MemberNames.UTCDate), 1 },

            // Video
            { new Interfacemember(DeviceTypes.Video, MemberNames.Action), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.BitDepth), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.CameraState), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.CanConfigureDeviceProperties), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.CommandBlind), 2 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.CommandBool), 2 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.CommandString), 2 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.ConfiguureDeviceProperties), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.Connect), 2 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.Connected), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.Connecting), 2 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.Description), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.DeviceState), 2 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.Disconnect), 2 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.Dispose), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.DriverInfo), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.DriverVersion), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.ExposureMax), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.ExposureMin), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.FrameRate), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.Gain), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.GainMax), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.GainMin), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.Gains), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.Gamma), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.GammaMax), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.GammaMin), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.Height), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.IntegrationRate), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.InterfaceVersion), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.LastVideoFrame), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.Name), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.PixelSizeX), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.PixelSizeY), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.SensorName), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.SensorType), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.SetupDialog), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.StartRecordingVideoFile), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.StopRecordingVideoFile), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.SupportedActions), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.SupportedIntegrationRates), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.VideoCaptureDeviceName), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.VideoCodec), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.VideoFileFormat), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.VideoFramesBufferSize), 1 },
            { new Interfacemember(DeviceTypes.Video, MemberNames.Width), 1 },
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Return the interface version in which a member was introduced
        /// </summary>
        /// <param name="member">Member name</param>
        /// <param name="deviceType">Device type</param>
        /// <returns>Interface version in which the member was introduced as a short (Int16)</returns>
        /// <exception cref="InvalidValueException"></exception>
        public static short VersionIntroduced(MemberNames member, DeviceTypes deviceType)
        {
            // Create a key into the lookup table
            Interfacemember key = new Interfacemember(deviceType, member);

            // Check if the key is in the lookup table
            if (VersionHistory.ContainsKey(key)) // Key is in the lookup table
            {
                // Return the interface version in which the member was introduced
                return VersionHistory[key];
            }
            else // Key is not in the lookup table so return an error
            {
                throw new InvalidValueException($"ASCOM Library DeviceCapabilities.VersionIntroduced - Member {member} is not defined in device type {deviceType}.");
            }
        }

        /// <summary>
        /// Determine whether a given member is present in the specified device type and interface version.
        /// </summary>
        /// <param name="member">Member name</param>
        /// <param name="deviceType">Device type</param>
        /// <param name="interfaceVersion">Interface version</param>
        /// <returns>True if the specified member is present in the given device interface version</returns>
        /// <exception cref="InvalidValueException"></exception>
        public static bool InterfaceHasMember(MemberNames member, DeviceTypes deviceType, int interfaceVersion)
        {
            // Validate inputs
            if (!IsValidAscomInterface(deviceType, interfaceVersion))
            {
                throw new InvalidValueException($"ASCOM Library DeviceCapabilities.InterfaceHasMember - The Interface version parameter is 0 or negative or greater than the current supported version: {interfaceVersion}");
            }

            // Create a key into the lookup table
            Interfacemember key = new Interfacemember(deviceType, member);

            // Check if the key is in the lookup table
            if (VersionHistory.ContainsKey(key)) // Key is in the lookup table
            {
                // Determine whether the member was introduced before or in the interface version number provided
                return interfaceVersion >= VersionHistory[key];
            }
            else // Key is not in the lookup table so return an error
            {
                throw new InvalidValueException($"ASCOM Library DeviceCapabilities.InterfaceHasMember - Member {member} is not defined in device type {deviceType}.");
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> for all devices except IFocuserV1 devices that do not have the Connected property
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="interfaceVersion">Interface version of this device (Int16)</param>
        /// <returns><see langword="true"/> for all device interfaces except IFocuserV1</returns>
        /// <exception cref="InvalidValueException"></exception>
        public static bool HasConnected(DeviceTypes? deviceType, short interfaceVersion)
        {
            return HasConnected(deviceType, Convert.ToInt32(interfaceVersion));
        }

        /// <summary>
        /// Returns <see langword="true"/> for all devices except IFocuserV1 devices that do not have the Connected property
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="interfaceVersion">Interface version of this device (Int32)</param>
        /// <returns><see langword="true"/> for all device interfaces except IFocuserV1</returns>
        /// <exception cref="InvalidValueException"></exception>
        public static bool HasConnected(DeviceTypes? deviceType, int interfaceVersion)
        {
            // Throw an exception if no device type is supplied
            if (!deviceType.HasValue)
                throw new InvalidValueException("ASCOMLibrary.DeviceCapabilities.HasConnected - Supplied device type is null.");

            // Switch on the device type 
            switch (deviceType)
            {
                // Focuser only has Connected in IFocuserV2 and later
                case DeviceTypes.Focuser: // Focuser device
                    if (interfaceVersion == 1) // IFocuserV1 so return false
                        return false;
                    else // IFocuserV2 or later so return true
                        return true;

                // All other device types and interface versions have Connected so return true.
                default: // All other device types
                    return true;
            }
        }

        /// <summary>
        /// Indicates whether this Switch interface version supports asynchronous Switch methods
        /// </summary>
        /// <param name="interfaceVersion">Interface version of this device (Int16)</param>
        /// <returns><see langword="true"/> when the interface version supports AsyncSwitch methods.</returns>
        /// <exception cref="InvalidValueException">The supplied interface version is 0 or less.</exception>
        public static bool HasAsyncSwitch(int interfaceVersion)
        {
            // Validate parameter
            if (interfaceVersion < 1)
                throw new InvalidValueException($"ASCOMLibrary.DeviceCapabilities.HasAsyncSwitch - Supplied interface version is 0 or negative: {interfaceVersion}");

            return interfaceVersion >= InitialPlatform7Interface[DeviceTypes.Switch];
        }

        /// <summary>
        /// Indicates whether this CoverCalibrator interface version supports the CoverCalibrator.CalibratorChanging property
        /// </summary>
        /// <param name="interfaceVersion">Interface version of this device</param>
        /// <returns><see langword="true"/> when the interface version supports CoverCalibrator.CalibratorChanging.</returns>
        /// <exception cref="InvalidValueException">The supplied interface version is 0 or less.</exception>
        public static bool HasCalibratorChanging(int interfaceVersion)
        {
            // Validate parameter
            if (interfaceVersion < 1)
                throw new InvalidValueException($"ASCOMLibrary.DeviceCapabilities.HasCalibratorChanging - Supplied interface version is 0 or negative: {interfaceVersion}");

            return interfaceVersion >= InitialPlatform7Interface[DeviceTypes.CoverCalibrator];
        }

        /// <summary>
        /// Indicates whether this CoverCalibrator interface version supports the CoverCalibrator.CoverMoving property
        /// Returns <see langword="true"/> if the device has a Platform 7 or later interface that supports the CoverCalibrator.CoverMoving property
        /// </summary>
        /// <param name="interfaceVersion">Interface version of this device</param>
        /// <returns><see langword="true"/> when the interface version supports CoverCalibrator.CoverMoving.</returns>
        /// <exception cref="InvalidValueException">The supplied interface version is 0 or less.</exception>
        public static bool HasCoverMoving(int interfaceVersion)
        {
            // Validate parameter
            if (interfaceVersion < 1)
                throw new InvalidValueException($"ASCOMLibrary.DeviceCapabilities.HasCoverMoving - Supplied interface version is 0 or negative: {interfaceVersion}");

            return interfaceVersion >= InitialPlatform7Interface[DeviceTypes.CoverCalibrator];
        }

        /// <summary>
        /// Returns <see langword="true"/> if the interface version of the specified device type supports Connect / Disconnect and DeviceState
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="interfaceVersion">Interface version of this device (Int16)</param>
        /// <returns><see langword="true"/> when the interface version supports Connect / Disconnect</returns>
        public static bool HasConnectAndDeviceState(DeviceTypes? deviceType, short interfaceVersion)
        {
            return HasConnectAndDeviceState(deviceType, Convert.ToInt32(interfaceVersion));
        }

        /// <summary>
        /// Returns <see langword="true"/> if the interface version of the specified device type supports Connect / Disconnect and DeviceState
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="interfaceVersion">Interface version of this device (Int32)</param>
        /// <exception cref="InvalidValueException">When deviceType is null.</exception>
        /// <exception cref="InvalidValueException">When interfaceVersion is 0 or negative.</exception>
        /// <returns><see langword="true"/> when the interface version supports Connect / Disconnect</returns>
        public static bool HasConnectAndDeviceState(DeviceTypes? deviceType, int interfaceVersion)
        {
            // Validate inputs
            if (!deviceType.HasValue) // The device type is a null value
            {
                throw new InvalidValueException("ASCOMLibrary.DeviceCapabilities.HasConnectAndDeviceState - The device type parameter is null.");
            }

            if (interfaceVersion < 1) // The interface version is 0 or negative
            {
                throw new InvalidValueException($"ASCOMLibrary.DeviceCapabilities.HasConnectAndDeviceState - The Interface version parameter is 0 or negative: {interfaceVersion}.");
            }

            return interfaceVersion >= InitialPlatform7Interface[deviceType.Value];
        }

        /// <summary>
        /// Returns <see langword="true"/> if the interface version of the specified device type is a Platform 6 interface version
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="interfaceVersion">Interface version of this device (Int16, short)</param>
        /// <exception cref="InvalidValueException">When deviceType is null.</exception>
        /// <exception cref="InvalidValueException">When interfaceVersion is 0 or negative.</exception>
        /// <returns><see langword="true"/> when the interface version is a Platform 6 interface version.</returns>
        public static bool IsPlatform6Interface(DeviceTypes? deviceType, short interfaceVersion)
        {
            return IsPlatform6Interface(deviceType, (int)interfaceVersion);
        }

        /// <summary>
        /// Returns <see langword="true"/> if the interface version of the specified device type is a Platform 6 interface version
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="interfaceVersion">Interface version of this device (Int32, int)</param>
        /// <exception cref="InvalidValueException">When deviceType is null.</exception>
        /// <exception cref="InvalidValueException">When interfaceVersion is 0 or negative.</exception>
        /// <returns><see langword="true"/> when the interface version is a Platform 6 interface version.</returns>
        public static bool IsPlatform6Interface(DeviceTypes? deviceType, int interfaceVersion)
        {
            // Validate inputs
            if (!deviceType.HasValue) // The device type is a null value
            {
                throw new InvalidValueException("ASCOMLibrary.DeviceCapabilities.IsPlatform6Interface - The device type parameter is null.");
            }

            if (interfaceVersion < 1) // The interface version is 0 or negative
            {
                throw new InvalidValueException($"ASCOMLibrary.DeviceCapabilities.IsPlatform6Interface - The Interface version parameter is 0 or negative: {interfaceVersion}.");
            }

            // Compensate for safety monitor possibly being interface version 1 or 2 by forcing the value to 2 for this test
            if (deviceType == DeviceTypes.SafetyMonitor)
                interfaceVersion = 2;

            // Compare the supplied interface version with the reference list
            return interfaceVersion == LatestPlatform6Interface[deviceType.Value];
        }

        /// <summary>
        /// Indicates whether the interface version of the specified device type is Platform 7 or later
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="interfaceVersion">Interface version of this device (Int32)</param>
        /// <returns><see langword="true"/> when the device implements a Platform 7 or later interface</returns>
        public static bool IsPlatform7OrLater(DeviceTypes? deviceType, int interfaceVersion)
        {
            return HasConnectAndDeviceState(deviceType, interfaceVersion);
        }

        /// <summary>
        /// Indicates whether the interface version of the specified device type is Platform 7 or later
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="interfaceVersion">Interface version of this device (Int16)</param>
        /// <returns><see langword="true"/> when the device implements a Platform 7 or later interface</returns>
        public static bool IsPlatform7OrLater(DeviceTypes? deviceType, short interfaceVersion)
        {
            return HasConnectAndDeviceState(deviceType, Convert.ToInt32(interfaceVersion));
        }

        /// <summary>
        /// Returns <see langword="true"/> if the interface version of the specified device type is a Platform 6 interface version
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="interfaceVersion">Interface version of this device (Int16, short)</param>
        /// <exception cref="InvalidValueException">When deviceType is null.</exception>
        /// <exception cref="InvalidValueException">When interfaceVersion is 0 or negative.</exception>
        /// <returns><see langword="true"/> when the interface version is a Platform 6 interface version.</returns>
        public static bool IsSupportedInterface(DeviceTypes? deviceType, short interfaceVersion)
        {
            return IsValidAscomInterface(deviceType, (int)interfaceVersion);
        }

        /// <summary>
        /// Returns <see langword="true"/> if the interface version of the specified device type is a valid ASCOM interface version on any Platform
        /// </summary>
        /// <param name="deviceType">Device type.</param>
        /// <param name="interfaceVersion">Interface version of this device (Int32, int).</param>
        /// <exception cref="InvalidValueException">When deviceType is null.</exception>
        /// <exception cref="InvalidValueException">When interfaceVersion is 0 or negative.</exception>
        /// <returns><see langword="true"/> when the interface version is a Platform 6 interface version.</returns>
        /// <remarks>
        /// Supply an interface version of 1 (valid) rather than zero (invalid) for very early drivers that do not have an InterfaceVersion property.
        /// </remarks>
        public static bool IsValidAscomInterface(DeviceTypes? deviceType, int interfaceVersion)
        {
            // Validate inputs
            if (!deviceType.HasValue) // The device type is a null value
            {
                throw new InvalidValueException("ASCOMLibrary.DeviceCapabilities.IsSupportedInterface - The device type parameter is null.");
            }

            // Check for invalid low interface version numbers
            if (interfaceVersion < 1) // The interface version is 0 or negative
                return false; // Not supported

            // Check whether the interface version is equal to or lower than the interface version in the latest Platform release
            if (interfaceVersion <= LatestInterface[deviceType.Value]) // The interface version is supported
                return true; // Supported

            // All other interface versions i.e. those above the interface version in the latest Platform release
            return false; // Not supported
        }

        #endregion
    }
}
