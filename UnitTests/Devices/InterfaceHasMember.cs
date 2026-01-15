using ASCOM;
using ASCOM.Common;
using ASCOM.Common.Alpaca;
using ASCOM.Common.DeviceInterfaces;
using Xunit;
using static ASCOM.Common.DeviceInterfaces.DeviceCapabilities;
using static ASCOM.Common.Devices;

namespace InterfaceHasMembers
{
    public class InterfaceHasMember
    {
        [Fact]
        public void Camera()
        {
            MemberIsPresentTest(MemberNames.AbortExposure, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.Action, DeviceTypes.Camera, 2);
            MemberIsPresentTest(MemberNames.BayerOffsetX, DeviceTypes.Camera, 2);
            MemberIsPresentTest(MemberNames.BayerOffsetY, DeviceTypes.Camera, 2);
            MemberIsPresentTest(MemberNames.BinX, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.BinY, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.CameraState, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.CameraXSize, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.CameraYSize, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.CanAbortExposure, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.CanAsymmetricBin, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.CanFastReadout, DeviceTypes.Camera, 2);
            MemberIsPresentTest(MemberNames.CanGetCoolerPower, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.CanPulseGuide, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.CanSetCCDTemperature, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.CanStopExposure, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.CCDTemperature, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.CommandBlind, DeviceTypes.Camera, 2);
            MemberIsPresentTest(MemberNames.CommandBool, DeviceTypes.Camera, 2);
            MemberIsPresentTest(MemberNames.CommandString, DeviceTypes.Camera, 2);
            MemberIsPresentTest(MemberNames.Connect, DeviceTypes.Camera, 4);
            MemberIsPresentTest(MemberNames.Connected, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.Connecting, DeviceTypes.Camera, 4);
            MemberIsPresentTest(MemberNames.CoolerOn, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.CoolerPower, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.Description, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.DeviceState, DeviceTypes.Camera, 4);
            MemberIsPresentTest(MemberNames.Disconnect, DeviceTypes.Camera, 4);
            MemberIsPresentTest(MemberNames.Dispose, DeviceTypes.Camera, 2);
            MemberIsPresentTest(MemberNames.DriverInfo, DeviceTypes.Camera, 2);
            MemberIsPresentTest(MemberNames.DriverVersion, DeviceTypes.Camera, 2);
            MemberIsPresentTest(MemberNames.ElectronsPerADU, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.ExposureMax, DeviceTypes.Camera, 3);
            MemberIsPresentTest(MemberNames.ExposureMin, DeviceTypes.Camera, 3);
            MemberIsPresentTest(MemberNames.ExposureResolution, DeviceTypes.Camera, 3);
            MemberIsPresentTest(MemberNames.FastReadout, DeviceTypes.Camera, 3);
            MemberIsPresentTest(MemberNames.FullWellCapacity, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.Gain, DeviceTypes.Camera, 2);
            MemberIsPresentTest(MemberNames.GainMax, DeviceTypes.Camera, 2);
            MemberIsPresentTest(MemberNames.GainMin, DeviceTypes.Camera, 2);
            MemberIsPresentTest(MemberNames.Gains, DeviceTypes.Camera, 2);
            MemberIsPresentTest(MemberNames.HasShutter, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.HeatSinkTemperature, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.ImageArray, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.ImageArrayVariant, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.ImageReady, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.InterfaceVersion, DeviceTypes.Camera, 2);
            MemberIsPresentTest(MemberNames.IsPulseGuiding, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.LastExposureDuration, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.LastExposureStartTime, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.MaxADU, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.MaxBinX, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.MaxBinY, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.Name, DeviceTypes.Camera, 2);
            MemberIsPresentTest(MemberNames.NumX, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.NumY, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.Offset, DeviceTypes.Camera, 3);
            MemberIsPresentTest(MemberNames.OffsetMax, DeviceTypes.Camera, 3);
            MemberIsPresentTest(MemberNames.OffsetMin, DeviceTypes.Camera, 3);
            MemberIsPresentTest(MemberNames.Offsets, DeviceTypes.Camera, 3);
            MemberIsPresentTest(MemberNames.PercentCompleted, DeviceTypes.Camera, 2);
            MemberIsPresentTest(MemberNames.PixelSizeX, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.PixelSizeY, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.PulseGuide, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.ReadoutMode, DeviceTypes.Camera, 2);
            MemberIsPresentTest(MemberNames.ReadoutModes, DeviceTypes.Camera, 2);
            MemberIsPresentTest(MemberNames.SensorName, DeviceTypes.Camera, 2);
            MemberIsPresentTest(MemberNames.SensorType, DeviceTypes.Camera, 2);
            MemberIsPresentTest(MemberNames.SetCCDTemperature, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.SetupDialog, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.StartExposure, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.StartX, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.StartY, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.StopExposure, DeviceTypes.Camera, 1);
            MemberIsPresentTest(MemberNames.SubExposureDuration, DeviceTypes.Camera, 3);
            MemberIsPresentTest(MemberNames.SupportedActions, DeviceTypes.Camera, 2);
        }

        [Fact]
        public void CoverCalibrator()
        {
            MemberIsPresentTest(MemberNames.Action, DeviceTypes.CoverCalibrator, 1);
            MemberIsPresentTest(MemberNames.Brightness, DeviceTypes.CoverCalibrator, 1);
            MemberIsPresentTest(MemberNames.CalibratorChanging, DeviceTypes.CoverCalibrator, 2);
            MemberIsPresentTest(MemberNames.CalibratorOff, DeviceTypes.CoverCalibrator, 1);
            MemberIsPresentTest(MemberNames.CalibratorOn, DeviceTypes.CoverCalibrator, 1);
            MemberIsPresentTest(MemberNames.CalibratorState, DeviceTypes.CoverCalibrator, 1);
            MemberIsPresentTest(MemberNames.CloseCover, DeviceTypes.CoverCalibrator, 1);
            MemberIsPresentTest(MemberNames.CommandBlind, DeviceTypes.CoverCalibrator, 1);
            MemberIsPresentTest(MemberNames.CommandBool, DeviceTypes.CoverCalibrator, 1);
            MemberIsPresentTest(MemberNames.CommandString, DeviceTypes.CoverCalibrator, 1);
            MemberIsPresentTest(MemberNames.Connect, DeviceTypes.CoverCalibrator, 2);
            MemberIsPresentTest(MemberNames.Connected, DeviceTypes.CoverCalibrator, 1);
            MemberIsPresentTest(MemberNames.Connecting, DeviceTypes.CoverCalibrator, 2);
            MemberIsPresentTest(MemberNames.CoverMoving, DeviceTypes.CoverCalibrator, 2);
            MemberIsPresentTest(MemberNames.CoverState, DeviceTypes.CoverCalibrator, 1);
            MemberIsPresentTest(MemberNames.Description, DeviceTypes.CoverCalibrator, 1);
            MemberIsPresentTest(MemberNames.DeviceState, DeviceTypes.CoverCalibrator, 2);
            MemberIsPresentTest(MemberNames.Disconnect, DeviceTypes.CoverCalibrator, 2);
            MemberIsPresentTest(MemberNames.Dispose, DeviceTypes.CoverCalibrator, 1);
            MemberIsPresentTest(MemberNames.DriverInfo, DeviceTypes.CoverCalibrator, 1);
            MemberIsPresentTest(MemberNames.DriverVersion, DeviceTypes.CoverCalibrator, 1);
            MemberIsPresentTest(MemberNames.HaltCover, DeviceTypes.CoverCalibrator, 1);
            MemberIsPresentTest(MemberNames.InterfaceVersion, DeviceTypes.CoverCalibrator, 1);
            MemberIsPresentTest(MemberNames.MaxBrightness, DeviceTypes.CoverCalibrator, 1);
            MemberIsPresentTest(MemberNames.Name, DeviceTypes.CoverCalibrator, 1);
            MemberIsPresentTest(MemberNames.OpenCover, DeviceTypes.CoverCalibrator, 1);
            MemberIsPresentTest(MemberNames.SetupDialog, DeviceTypes.CoverCalibrator, 1);
            MemberIsPresentTest(MemberNames.SupportedActions, DeviceTypes.CoverCalibrator, 1);
        }

        [Fact]
        public void Dome()
        {
            MemberIsPresentTest(MemberNames.AbortSlew, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.Action, DeviceTypes.Dome, 2);
            MemberIsPresentTest(MemberNames.Altitude, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.AtHome, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.AtPark, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.Azimuth, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.CanFindHome, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.CanPark, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.CanSetAltitude, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.CanSetAzimuth, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.CanSetPark, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.CanSetShutter, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.CanSlave, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.CanSyncAzimuth, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.CloseShutter, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.CommandBlind, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.CommandBool, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.CommandString, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.Connect, DeviceTypes.Dome, 3);
            MemberIsPresentTest(MemberNames.Connected, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.Connecting, DeviceTypes.Dome, 3);
            MemberIsPresentTest(MemberNames.Description, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.DeviceState, DeviceTypes.Dome, 3);
            MemberIsPresentTest(MemberNames.Disconnect, DeviceTypes.Dome, 3);
            MemberIsPresentTest(MemberNames.Dispose, DeviceTypes.Dome, 2);
            MemberIsPresentTest(MemberNames.DriverInfo, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.DriverVersion, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.FindHome, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.InterfaceVersion, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.Name, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.OpenShutter, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.Park, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.SetPark, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.SetupDialog, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.ShutterStatus, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.Slewing, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.SlewToAltitude, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.SlewToAzimuth, DeviceTypes.Dome, 1);
            MemberIsPresentTest(MemberNames.SupportedActions, DeviceTypes.Dome, 2);
            MemberIsPresentTest(MemberNames.SyncToAzimuth, DeviceTypes.Dome, 1);
        }

        [Fact]
        public void FilterWheel()
        {
            MemberIsPresentTest(MemberNames.Action, DeviceTypes.FilterWheel, 2);
            MemberIsPresentTest(MemberNames.CommandBlind, DeviceTypes.FilterWheel, 2);
            MemberIsPresentTest(MemberNames.CommandBool, DeviceTypes.FilterWheel, 2);
            MemberIsPresentTest(MemberNames.CommandString, DeviceTypes.FilterWheel, 2);
            MemberIsPresentTest(MemberNames.Connect, DeviceTypes.FilterWheel, 3);
            MemberIsPresentTest(MemberNames.Connected, DeviceTypes.FilterWheel, 1);
            MemberIsPresentTest(MemberNames.Connecting, DeviceTypes.FilterWheel, 3);
            MemberIsPresentTest(MemberNames.Description, DeviceTypes.FilterWheel, 2);
            MemberIsPresentTest(MemberNames.DeviceState, DeviceTypes.FilterWheel, 3);
            MemberIsPresentTest(MemberNames.Disconnect, DeviceTypes.FilterWheel, 3);
            MemberIsPresentTest(MemberNames.DriverInfo, DeviceTypes.FilterWheel, 2);
            MemberIsPresentTest(MemberNames.DriverVersion, DeviceTypes.FilterWheel, 2);
            MemberIsPresentTest(MemberNames.FocusOffsets, DeviceTypes.FilterWheel, 1);
            MemberIsPresentTest(MemberNames.InterfaceVersion, DeviceTypes.FilterWheel, 2);
            MemberIsPresentTest(MemberNames.Name, DeviceTypes.FilterWheel, 2);
            MemberIsPresentTest(MemberNames.Names, DeviceTypes.FilterWheel, 1);
            MemberIsPresentTest(MemberNames.Position, DeviceTypes.FilterWheel, 1);
            MemberIsPresentTest(MemberNames.SetupDialog, DeviceTypes.FilterWheel, 1);
            MemberIsPresentTest(MemberNames.SupportedActions, DeviceTypes.FilterWheel, 2);
        }

        [Fact]
        public void Focuser()
        {
            MemberIsPresentTest(MemberNames.Absolute, DeviceTypes.Focuser, 1);
            MemberIsPresentTest(MemberNames.Action, DeviceTypes.Focuser, 2);
            MemberIsPresentTest(MemberNames.CommandBlind, DeviceTypes.Focuser, 2);
            MemberIsPresentTest(MemberNames.Action, DeviceTypes.Focuser, 2);
            MemberIsPresentTest(MemberNames.CommandBlind, DeviceTypes.Focuser, 2);
            MemberIsPresentTest(MemberNames.CommandBool, DeviceTypes.Focuser, 2);
            MemberIsPresentTest(MemberNames.CommandString, DeviceTypes.Focuser, 2);
            MemberIsPresentTest(MemberNames.Connect, DeviceTypes.Focuser, 4);
            MemberIsPresentTest(MemberNames.Connected, DeviceTypes.Focuser, 2);
            MemberIsPresentTest(MemberNames.Connecting, DeviceTypes.Focuser, 4);
            MemberIsPresentTest(MemberNames.Description, DeviceTypes.Focuser, 2);
            MemberIsPresentTest(MemberNames.DeviceState, DeviceTypes.Focuser, 4);
            MemberIsPresentTest(MemberNames.Disconnect, DeviceTypes.Focuser, 4);
            MemberIsPresentTest(MemberNames.Dispose, DeviceTypes.Focuser, 2);
            MemberIsPresentTest(MemberNames.DriverInfo, DeviceTypes.Focuser, 2);
            MemberIsPresentTest(MemberNames.DriverVersion, DeviceTypes.Focuser, 2);
            MemberIsPresentTest(MemberNames.Halt, DeviceTypes.Focuser, 1);
            MemberIsPresentTest(MemberNames.InterfaceVersion, DeviceTypes.Focuser, 2);
            MemberIsPresentTest(MemberNames.IsMoving, DeviceTypes.Focuser, 1);
            MemberIsPresentTest(MemberNames.Link, DeviceTypes.Focuser, 1);
            MemberIsPresentTest(MemberNames.MaxIncrement, DeviceTypes.Focuser, 1);
            MemberIsPresentTest(MemberNames.MaxStep, DeviceTypes.Focuser, 1);
            MemberIsPresentTest(MemberNames.Move, DeviceTypes.Focuser, 1);
            MemberIsPresentTest(MemberNames.Name, DeviceTypes.Focuser, 2);
            MemberIsPresentTest(MemberNames.Position, DeviceTypes.Focuser, 1);
            MemberIsPresentTest(MemberNames.SetupDialog, DeviceTypes.Focuser, 1);
            MemberIsPresentTest(MemberNames.StepSize, DeviceTypes.Focuser, 1);
            MemberIsPresentTest(MemberNames.SupportedActions, DeviceTypes.Focuser, 2);
            MemberIsPresentTest(MemberNames.TempComp, DeviceTypes.Focuser, 1);
            MemberIsPresentTest(MemberNames.TempCompAvailable, DeviceTypes.Focuser, 1);
            MemberIsPresentTest(MemberNames.Temperature, DeviceTypes.Focuser, 1);
        }

        [Fact]
        public void ObservingConditions()
        {
            MemberIsPresentTest(MemberNames.Action, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.AveragePeriod, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.CloudCover, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.CommandBlind, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.CommandBool, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.CommandString, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.Connect, DeviceTypes.ObservingConditions, 2);
            MemberIsPresentTest(MemberNames.Connected, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.Connecting, DeviceTypes.ObservingConditions, 2);
            MemberIsPresentTest(MemberNames.Description, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.DeviceState, DeviceTypes.ObservingConditions, 2);
            MemberIsPresentTest(MemberNames.DewPoint, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.Disconnect, DeviceTypes.ObservingConditions, 2);
            MemberIsPresentTest(MemberNames.Dispose, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.DriverInfo, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.DriverVersion, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.Humidity, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.InterfaceVersion, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.Name, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.Pressure, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.RainRate, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.Refresh, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.SensorDescription, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.SetupDialog, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.SkyBrightness, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.SkyQuality, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.SkyTemperature, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.StarFWHM, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.SupportedActions, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.Temperature, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.TimeSinceLastUpdate, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.WindDirection, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.WindGust, DeviceTypes.ObservingConditions, 1);
            MemberIsPresentTest(MemberNames.WindSpeed, DeviceTypes.ObservingConditions, 1);
        }

        [Fact]
        public void Rotator()
        {
            MemberIsPresentTest(MemberNames.Action, DeviceTypes.Rotator, 2);
            MemberIsPresentTest(MemberNames.CanReverse, DeviceTypes.Rotator, 2);
            MemberIsPresentTest(MemberNames.CommandBlind, DeviceTypes.Rotator, 2);
            MemberIsPresentTest(MemberNames.CommandBool, DeviceTypes.Rotator, 2);
            MemberIsPresentTest(MemberNames.CommandString, DeviceTypes.Rotator, 2);
            MemberIsPresentTest(MemberNames.Connect, DeviceTypes.Rotator, 4);
            MemberIsPresentTest(MemberNames.Connected, DeviceTypes.Rotator, 1);
            MemberIsPresentTest(MemberNames.Connecting, DeviceTypes.Rotator, 4);
            MemberIsPresentTest(MemberNames.Description, DeviceTypes.Rotator, 2);
            MemberIsPresentTest(MemberNames.DeviceState, DeviceTypes.Rotator, 4);
            MemberIsPresentTest(MemberNames.Disconnect, DeviceTypes.Rotator, 4);
            MemberIsPresentTest(MemberNames.Dispose, DeviceTypes.Rotator, 2);
            MemberIsPresentTest(MemberNames.DriverInfo, DeviceTypes.Rotator, 2);
            MemberIsPresentTest(MemberNames.DriverVersion, DeviceTypes.Rotator, 2);
            MemberIsPresentTest(MemberNames.Halt, DeviceTypes.Rotator, 1);
            MemberIsPresentTest(MemberNames.InterfaceVersion, DeviceTypes.Rotator, 2);
            MemberIsPresentTest(MemberNames.IsMoving, DeviceTypes.Rotator, 1);
            MemberIsPresentTest(MemberNames.MechanicalPosition, DeviceTypes.Rotator, 3);
            MemberIsPresentTest(MemberNames.Move, DeviceTypes.Rotator, 1);
            MemberIsPresentTest(MemberNames.MoveAbsolute, DeviceTypes.Rotator, 1);
            MemberIsPresentTest(MemberNames.MoveMechanical, DeviceTypes.Rotator, 3);
            MemberIsPresentTest(MemberNames.Name, DeviceTypes.Rotator, 2);
            MemberIsPresentTest(MemberNames.Position, DeviceTypes.Rotator, 1);
            MemberIsPresentTest(MemberNames.Reverse, DeviceTypes.Rotator, 1);
            MemberIsPresentTest(MemberNames.SetupDialog, DeviceTypes.Rotator, 1);
            MemberIsPresentTest(MemberNames.StepSize, DeviceTypes.Rotator, 1);
            MemberIsPresentTest(MemberNames.SupportedActions, DeviceTypes.Rotator, 2);
            MemberIsPresentTest(MemberNames.Sync, DeviceTypes.Rotator, 3);
            MemberIsPresentTest(MemberNames.TargetPosition, DeviceTypes.Rotator, 1);
        }

        [Fact]
        public void SafetyMonitor()
        {
            MemberIsPresentTest(MemberNames.Action, DeviceTypes.SafetyMonitor, 1);
            MemberIsPresentTest(MemberNames.CommandBlind, DeviceTypes.SafetyMonitor, 1);
            MemberIsPresentTest(MemberNames.CommandBool, DeviceTypes.SafetyMonitor, 1);
            MemberIsPresentTest(MemberNames.CommandString, DeviceTypes.SafetyMonitor, 1);
            MemberIsPresentTest(MemberNames.Connect, DeviceTypes.SafetyMonitor, 3);
            MemberIsPresentTest(MemberNames.Connected, DeviceTypes.SafetyMonitor, 1);
            MemberIsPresentTest(MemberNames.Connecting, DeviceTypes.SafetyMonitor, 3);
            MemberIsPresentTest(MemberNames.Description, DeviceTypes.SafetyMonitor, 1);
            MemberIsPresentTest(MemberNames.DeviceState, DeviceTypes.SafetyMonitor, 3);
            MemberIsPresentTest(MemberNames.Disconnect, DeviceTypes.SafetyMonitor, 3);
            MemberIsPresentTest(MemberNames.Dispose, DeviceTypes.SafetyMonitor, 1);
            MemberIsPresentTest(MemberNames.DriverInfo, DeviceTypes.SafetyMonitor, 1);
            MemberIsPresentTest(MemberNames.DriverVersion, DeviceTypes.SafetyMonitor, 1);
            MemberIsPresentTest(MemberNames.InterfaceVersion, DeviceTypes.SafetyMonitor, 1);
            MemberIsPresentTest(MemberNames.IsSafe, DeviceTypes.SafetyMonitor, 1);
            MemberIsPresentTest(MemberNames.Name, DeviceTypes.SafetyMonitor, 1);
            MemberIsPresentTest(MemberNames.SetupDialog, DeviceTypes.SafetyMonitor, 1);
            MemberIsPresentTest(MemberNames.SupportedActions, DeviceTypes.SafetyMonitor, 1);
        }

        [Fact]
        public void Switch()
        {
            MemberIsPresentTest(MemberNames.Action, DeviceTypes.Switch, 2);
            MemberIsPresentTest(MemberNames.CanWrite, DeviceTypes.Switch, 2);
            MemberIsPresentTest(MemberNames.CommandBlind, DeviceTypes.Switch, 2);
            MemberIsPresentTest(MemberNames.CommandBool, DeviceTypes.Switch, 2);
            MemberIsPresentTest(MemberNames.CommandString, DeviceTypes.Switch, 2);
            MemberIsPresentTest(MemberNames.Connect, DeviceTypes.Switch, 3);
            MemberIsPresentTest(MemberNames.Connected, DeviceTypes.Switch, 1);
            MemberIsPresentTest(MemberNames.Connecting, DeviceTypes.Switch, 3);
            MemberIsPresentTest(MemberNames.Description, DeviceTypes.Switch, 1);
            MemberIsPresentTest(MemberNames.DeviceState, DeviceTypes.Switch, 3);
            MemberIsPresentTest(MemberNames.Disconnect, DeviceTypes.Switch, 3);
            MemberIsPresentTest(MemberNames.Dispose, DeviceTypes.Switch, 2);
            MemberIsPresentTest(MemberNames.DriverInfo, DeviceTypes.Switch, 1);
            MemberIsPresentTest(MemberNames.DriverVersion, DeviceTypes.Switch, 1);
            MemberIsPresentTest(MemberNames.GetSwitch, DeviceTypes.Switch, 1);
            MemberIsPresentTest(MemberNames.GetSwitchDescription, DeviceTypes.Switch, 2);
            MemberIsPresentTest(MemberNames.GetSwitchName, DeviceTypes.Switch, 1);
            MemberIsPresentTest(MemberNames.GetSwitchValue, DeviceTypes.Switch, 2);
            MemberIsPresentTest(MemberNames.InterfaceVersion, DeviceTypes.Switch, 1);
            MemberIsPresentTest(MemberNames.MaxSwitch, DeviceTypes.Switch, 1);
            MemberIsPresentTest(MemberNames.MaxSwitchValue, DeviceTypes.Switch, 2);
            MemberIsPresentTest(MemberNames.MinSwitchValue, DeviceTypes.Switch, 2);
            MemberIsPresentTest(MemberNames.Name, DeviceTypes.Switch, 1);
            MemberIsPresentTest(MemberNames.SetAsync, DeviceTypes.Switch, 3);
            MemberIsPresentTest(MemberNames.SetAsyncValue, DeviceTypes.Switch, 3);
            MemberIsPresentTest(MemberNames.SetSwitch, DeviceTypes.Switch, 1);
            MemberIsPresentTest(MemberNames.SetSwitchName, DeviceTypes.Switch, 2);
            MemberIsPresentTest(MemberNames.SetSwitchValue, DeviceTypes.Switch, 2);
            MemberIsPresentTest(MemberNames.SetupDialog, DeviceTypes.Switch, 1);
            MemberIsPresentTest(MemberNames.StateChangeComplete, DeviceTypes.Switch, 3);
            MemberIsPresentTest(MemberNames.SupportedActions, DeviceTypes.Switch, 2);
            MemberIsPresentTest(MemberNames.SwitchStep, DeviceTypes.Switch, 2);
        }

        [Fact]
        public void Telescope()
        {
            MemberIsPresentTest(MemberNames.AbortSlew, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.Action, DeviceTypes.Telescope, 3);
            MemberIsPresentTest(MemberNames.AlignmentMode, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.Altitude, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.ApertureArea, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.ApertureDiameter, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.AtHome, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.AtPark, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.AxisRates, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.Azimuth, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.CanFindHome, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.CanMoveAxis, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.CanPark, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.CanPulseGuide, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.CanSetDeclinationRate, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.CanSetGuideRates, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.CanSetPark, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.CanSetPierSide, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.CanSetRightAscensionRate, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.CanSetTracking, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.CanSlew, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.CanSlewAltAz, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.CanSlewAltAzAsync, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.CanSlewAsync, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.CanSync, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.CanSyncAltAz, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.CanUnpark, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.CommandBlind, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.CommandBool, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.CommandString, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.Connect, DeviceTypes.Telescope, 4);
            MemberIsPresentTest(MemberNames.Connected, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.Connecting, DeviceTypes.Telescope, 4);
            MemberIsPresentTest(MemberNames.Declination, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.DeclinationRate, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.Description, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.DestinationSideOfPier, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.DeviceState, DeviceTypes.Telescope, 4);
            MemberIsPresentTest(MemberNames.Disconnect, DeviceTypes.Telescope, 4);
            MemberIsPresentTest(MemberNames.Dispose, DeviceTypes.Telescope, 3);
            MemberIsPresentTest(MemberNames.DoesRefraction, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.DriverInfo, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.DriverVersion, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.EquatorialSystem, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.FindHome, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.FocalLength, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.GuideRateDeclination, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.GuideRateRightAscension, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.InterfaceVersion, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.IsPulseGuiding, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.MoveAxis, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.Name, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.Park, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.PulseGuide, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.RightAscension, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.RightAscensionRate, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.SetPark, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.SetupDialog, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.SideOfPier, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.SiderealTime, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.SiteElevation, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.SiteLatitude, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.SiteLongitude, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.Slewing, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.SlewSettleTime, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.SlewToAltAz, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.SlewToAltAzAsync, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.SlewToCoordinates, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.SlewToCoordinatesAsync, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.SlewToTarget, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.SlewToTargetAsync, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.SupportedActions, DeviceTypes.Telescope, 3);
            MemberIsPresentTest(MemberNames.SyncToAltAz, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.SyncToCoordinates, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.SyncToTarget, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.TargetDeclination, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.TargetRightAscension, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.Tracking, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.TrackingRate, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.TrackingRates, DeviceTypes.Telescope, 2);
            MemberIsPresentTest(MemberNames.Unpark, DeviceTypes.Telescope, 1);
            MemberIsPresentTest(MemberNames.UTCDate, DeviceTypes.Telescope, 1);
        }

        [Fact]
        public void Video()
        {
            MemberIsPresentTest(MemberNames.Action, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.BitDepth, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.CameraState, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.CanConfigureDeviceProperties, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.CommandBlind, DeviceTypes.Video, 2);
            MemberIsPresentTest(MemberNames.CommandBool, DeviceTypes.Video, 2);
            MemberIsPresentTest(MemberNames.CommandString, DeviceTypes.Video, 2);
            MemberIsPresentTest(MemberNames.ConfiguureDeviceProperties, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.Connect, DeviceTypes.Video, 2);
            MemberIsPresentTest(MemberNames.Connected, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.Connecting, DeviceTypes.Video, 2);
            MemberIsPresentTest(MemberNames.Description, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.DeviceState, DeviceTypes.Video, 2);
            MemberIsPresentTest(MemberNames.Disconnect, DeviceTypes.Video, 2);
            MemberIsPresentTest(MemberNames.Dispose, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.DriverInfo, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.DriverVersion, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.ExposureMax, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.ExposureMin, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.FrameRate, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.Gain, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.GainMax, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.GainMin, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.Gains, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.Gamma, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.GammaMax, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.GammaMin, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.Height, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.IntegrationRate, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.InterfaceVersion, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.LastVideoFrame, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.Name, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.PixelSizeX, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.PixelSizeY, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.SensorName, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.SensorType, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.SetupDialog, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.StartRecordingVideoFile, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.StopRecordingVideoFile, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.SupportedActions, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.SupportedIntegrationRates, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.VideoCaptureDeviceName, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.VideoCodec, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.VideoFileFormat, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.VideoFramesBufferSize, DeviceTypes.Video, 1);
            MemberIsPresentTest(MemberNames.Width, DeviceTypes.Video, 1);
        }

        [Fact]
        public void UnknownMembers()
        {
            // A small selection of bad values to confirm rejection
            Assert.Throws<InvalidValueException>(() => InterfaceHasMember(MemberNames.AbortExposure, DeviceTypes.Telescope, 1));
            Assert.Throws<InvalidValueException>(() => InterfaceHasMember(MemberNames.SlewToAltitude, DeviceTypes.Telescope, 1));
            Assert.Throws<InvalidValueException>(() => InterfaceHasMember(MemberNames.RainRate, DeviceTypes.Camera, 1));
            Assert.Throws<InvalidValueException>(() => InterfaceHasMember(MemberNames.IsSafe, DeviceTypes.Rotator, 1));
            Assert.Throws<InvalidValueException>(() => InterfaceHasMember(MemberNames.GetSwitch, DeviceTypes.CoverCalibrator, 1));
            Assert.Throws<InvalidValueException>(() => InterfaceHasMember(MemberNames.Move, DeviceTypes.SafetyMonitor, 1));
        }

        [Fact]
        public void VersionIntroduced()
        {
            // Basic tests only because full data coverage is provided by the member present tests
            Assert.NotEqual(1, DeviceCapabilities.VersionIntroduced(MemberNames.Action, DeviceTypes.Camera));
            Assert.Equal(2, DeviceCapabilities.VersionIntroduced(MemberNames.Action, DeviceTypes.Camera));
            Assert.NotEqual(3, DeviceCapabilities.VersionIntroduced(MemberNames.Action, DeviceTypes.Camera));
            Assert.Throws<InvalidValueException>(() => InterfaceHasMember(MemberNames.GetSwitch, DeviceTypes.CoverCalibrator, 1));
        }

        private static void MemberIsPresentTest(MemberNames memberName, DeviceTypes deviceType, int interfaceVersion)
        {
            // Check that the member is present in the interface version and not present in the previous version
            if (interfaceVersion > 1) // Interface version is 2 or more
            {
                Assert.True(InterfaceHasMember(memberName, deviceType, interfaceVersion));
                Assert.False(InterfaceHasMember(memberName, deviceType, interfaceVersion - 1));
            }
            else // InterfaceVersion version is 1 or less
            {
                Assert.True(InterfaceHasMember(memberName, deviceType, interfaceVersion));
                Assert.Throws<InvalidValueException>(() => InterfaceHasMember(memberName, deviceType, interfaceVersion - 1));
            }

            // Make sure that an interface version number higher than the current latest version fails
            Assert.Throws<InvalidValueException>(() => InterfaceHasMember(memberName, deviceType, 99));
        }
    }
}
