using ASCOM.Common;
using Xunit;
using static ASCOM.Common.DeviceInterfaces.DeviceCapabilities;
using static ASCOM.Common.Devices;

namespace ASCOM.Alpaca.Tests.Devices
{
    public class InterfaceHasMember
    {
        [Fact]
        public void Camera()
        {
            AssertTest(MemberNames.AbortExposure, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.Action, DeviceTypes.Camera, 2);
            AssertTest(MemberNames.BayerOffsetX, DeviceTypes.Camera, 2);
            AssertTest(MemberNames.BayerOffsetY, DeviceTypes.Camera, 2);
            AssertTest(MemberNames.BinX, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.BinY, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.CameraState, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.CameraXSize, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.CameraYSize, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.CanAbortExposure, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.CanAsymmetricBin, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.CanFastReadout, DeviceTypes.Camera, 2);
            AssertTest(MemberNames.CanGetCoolerPower, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.CanPulseGuide, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.CanSetCCDTemperature, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.CanStopExposure, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.CCDTemperature, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.CommandBlind, DeviceTypes.Camera, 2);
            AssertTest(MemberNames.CommandBool, DeviceTypes.Camera, 2);
            AssertTest(MemberNames.CommandString, DeviceTypes.Camera, 2);
            AssertTest(MemberNames.Connect, DeviceTypes.Camera, 4);
            AssertTest(MemberNames.Connected, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.Connecting, DeviceTypes.Camera, 4);
            AssertTest(MemberNames.CoolerOn, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.CoolerPower, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.Description, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.DeviceState, DeviceTypes.Camera, 4);
            AssertTest(MemberNames.Disconnect, DeviceTypes.Camera, 4);
            AssertTest(MemberNames.Dispose, DeviceTypes.Camera, 2);
            AssertTest(MemberNames.DriverInfo, DeviceTypes.Camera, 2);
            AssertTest(MemberNames.DriverVersion, DeviceTypes.Camera, 2);
            AssertTest(MemberNames.ElectronsPerADU, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.ExposureMax, DeviceTypes.Camera, 3);
            AssertTest(MemberNames.ExposureMin, DeviceTypes.Camera, 3);
            AssertTest(MemberNames.ExposureResolution, DeviceTypes.Camera, 3);
            AssertTest(MemberNames.FastReadout, DeviceTypes.Camera, 3);
            AssertTest(MemberNames.FullWellCapacity, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.Gain, DeviceTypes.Camera, 2);
            AssertTest(MemberNames.GainMax, DeviceTypes.Camera, 2);
            AssertTest(MemberNames.GainMin, DeviceTypes.Camera, 2);
            AssertTest(MemberNames.Gains, DeviceTypes.Camera, 2);
            AssertTest(MemberNames.HasShutter, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.HeatSinkTemperature, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.ImageArray, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.ImageArrayVariant, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.ImageReady, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.InterfaceVersion, DeviceTypes.Camera, 2);
            AssertTest(MemberNames.IsPulseGuiding, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.LastExposureDuration, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.LastExposureStartTime, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.MaxADU, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.MaxBinX, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.MaxBinY, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.Name, DeviceTypes.Camera, 2);
            AssertTest(MemberNames.NumX, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.NumY, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.Offset, DeviceTypes.Camera, 3);
            AssertTest(MemberNames.OffsetMax, DeviceTypes.Camera, 3);
            AssertTest(MemberNames.OffsetMin, DeviceTypes.Camera, 3);
            AssertTest(MemberNames.Offsets, DeviceTypes.Camera, 3);
            AssertTest(MemberNames.PercentCompleted, DeviceTypes.Camera, 2);
            AssertTest(MemberNames.PixelSizeX, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.PixelSizeY, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.PulseGuide, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.ReadoutMode, DeviceTypes.Camera, 2);
            AssertTest(MemberNames.ReadoutModes, DeviceTypes.Camera, 2);
            AssertTest(MemberNames.SensorName, DeviceTypes.Camera, 2);
            AssertTest(MemberNames.SensorType, DeviceTypes.Camera, 2);
            AssertTest(MemberNames.SetCCDTemperature, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.SetupDialog, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.StartExposure, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.StartX, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.StartY, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.StopExposure, DeviceTypes.Camera, 1);
            AssertTest(MemberNames.SubExposureDuration, DeviceTypes.Camera, 3);
            AssertTest(MemberNames.SupportedActions, DeviceTypes.Camera, 2);
        }

        [Fact]
        public void CoverCalibrator()
        {
            AssertTest(MemberNames.Action, DeviceTypes.CoverCalibrator, 1);
            AssertTest(MemberNames.Brightness, DeviceTypes.CoverCalibrator, 1);
            AssertTest(MemberNames.CalibratorChanging, DeviceTypes.CoverCalibrator, 2);
            AssertTest(MemberNames.CalibratorOff, DeviceTypes.CoverCalibrator, 1);
            AssertTest(MemberNames.CalibratorOn, DeviceTypes.CoverCalibrator, 1);
            AssertTest(MemberNames.CalibratorState, DeviceTypes.CoverCalibrator, 1);
            AssertTest(MemberNames.CloseCover, DeviceTypes.CoverCalibrator, 1);
            AssertTest(MemberNames.CommandBlind, DeviceTypes.CoverCalibrator, 1);
            AssertTest(MemberNames.CommandBool, DeviceTypes.CoverCalibrator, 1);
            AssertTest(MemberNames.CommandString, DeviceTypes.CoverCalibrator, 1);
            AssertTest(MemberNames.Connect, DeviceTypes.CoverCalibrator, 2);
            AssertTest(MemberNames.Connected, DeviceTypes.CoverCalibrator, 1);
            AssertTest(MemberNames.Connecting, DeviceTypes.CoverCalibrator, 2);
            AssertTest(MemberNames.CoverMoving, DeviceTypes.CoverCalibrator, 2);
            AssertTest(MemberNames.CoverState, DeviceTypes.CoverCalibrator, 1);
            AssertTest(MemberNames.Description, DeviceTypes.CoverCalibrator, 1);
            AssertTest(MemberNames.DeviceState, DeviceTypes.CoverCalibrator, 2);
            AssertTest(MemberNames.Disconnect, DeviceTypes.CoverCalibrator, 2);
            AssertTest(MemberNames.Dispose, DeviceTypes.CoverCalibrator, 1);
            AssertTest(MemberNames.DriverInfo, DeviceTypes.CoverCalibrator, 1);
            AssertTest(MemberNames.DriverVersion, DeviceTypes.CoverCalibrator, 1);
            AssertTest(MemberNames.HaltCover, DeviceTypes.CoverCalibrator, 1);
            AssertTest(MemberNames.InterfaceVersion, DeviceTypes.CoverCalibrator, 1);
            AssertTest(MemberNames.MaxBrightness, DeviceTypes.CoverCalibrator, 1);
            AssertTest(MemberNames.Name, DeviceTypes.CoverCalibrator, 1);
            AssertTest(MemberNames.OpenCover, DeviceTypes.CoverCalibrator, 1);
            AssertTest(MemberNames.SetupDialog, DeviceTypes.CoverCalibrator, 1);
            AssertTest(MemberNames.SupportedActions, DeviceTypes.CoverCalibrator, 1);
        }

        [Fact]
        public void Dome()
        {
            AssertTest(MemberNames.AbortSlew, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.Action, DeviceTypes.Dome, 2);
            AssertTest(MemberNames.Altitude, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.AtHome, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.AtPark, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.Azimuth, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.CanFindHome, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.CanPark, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.CanSetAltitude, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.CanSetAzimuth, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.CanSetPark, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.CanSetShutter, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.CanSlave, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.CanSyncAzimuth, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.CloseShutter, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.CommandBlind, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.CommandBool, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.CommandString, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.Connect, DeviceTypes.Dome, 3);
            AssertTest(MemberNames.Connected, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.Connecting, DeviceTypes.Dome, 3);
            AssertTest(MemberNames.Description, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.DeviceState, DeviceTypes.Dome, 3);
            AssertTest(MemberNames.Disconnect, DeviceTypes.Dome, 3);
            AssertTest(MemberNames.Dispose, DeviceTypes.Dome, 2);
            AssertTest(MemberNames.DriverInfo, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.DriverVersion, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.FindHome, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.InterfaceVersion, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.Name, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.OpenShutter, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.Park, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.SetPark, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.SetupDialog, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.ShutterStatus, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.Slewing, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.SlewToAltitude, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.SlewToAzimuth, DeviceTypes.Dome, 1);
            AssertTest(MemberNames.SupportedActions, DeviceTypes.Dome, 2);
            AssertTest(MemberNames.SyncToAzimuth, DeviceTypes.Dome, 1);
        }

        [Fact]
        public void FilterWheel()
        {
            AssertTest(MemberNames.Action, DeviceTypes.FilterWheel, 2);
            AssertTest(MemberNames.CommandBlind, DeviceTypes.FilterWheel, 2);
            AssertTest(MemberNames.CommandBool, DeviceTypes.FilterWheel, 2);
            AssertTest(MemberNames.CommandString, DeviceTypes.FilterWheel, 2);
            AssertTest(MemberNames.Connect, DeviceTypes.FilterWheel, 3);
            AssertTest(MemberNames.Connected, DeviceTypes.FilterWheel, 1);
            AssertTest(MemberNames.Connecting, DeviceTypes.FilterWheel, 3);
            AssertTest(MemberNames.Description, DeviceTypes.FilterWheel, 2);
            AssertTest(MemberNames.DeviceState, DeviceTypes.FilterWheel, 3);
            AssertTest(MemberNames.Disconnect, DeviceTypes.FilterWheel, 3);
            AssertTest(MemberNames.DriverInfo, DeviceTypes.FilterWheel, 2);
            AssertTest(MemberNames.DriverVersion, DeviceTypes.FilterWheel, 2);
            AssertTest(MemberNames.FocusOffsets, DeviceTypes.FilterWheel, 1);
            AssertTest(MemberNames.InterfaceVersion, DeviceTypes.FilterWheel, 2);
            AssertTest(MemberNames.Name, DeviceTypes.FilterWheel, 2);
            AssertTest(MemberNames.Names, DeviceTypes.FilterWheel, 1);
            AssertTest(MemberNames.Position, DeviceTypes.FilterWheel, 1);
            AssertTest(MemberNames.SetupDialog, DeviceTypes.FilterWheel, 1);
            AssertTest(MemberNames.SupportedActions, DeviceTypes.FilterWheel, 2);
        }

        [Fact]
        public void Focuser()
        {
            AssertTest(MemberNames.Absolute, DeviceTypes.Focuser, 1);
            AssertTest(MemberNames.Action, DeviceTypes.Focuser, 2);
            AssertTest(MemberNames.CommandBlind, DeviceTypes.Focuser, 2);
            AssertTest(MemberNames.Action, DeviceTypes.Focuser, 2);
            AssertTest(MemberNames.CommandBlind, DeviceTypes.Focuser, 2);
            AssertTest(MemberNames.CommandBool, DeviceTypes.Focuser, 2);
            AssertTest(MemberNames.CommandString, DeviceTypes.Focuser, 2);
            AssertTest(MemberNames.Connect, DeviceTypes.Focuser, 4);
            AssertTest(MemberNames.Connected, DeviceTypes.Focuser, 2);
            AssertTest(MemberNames.Connecting, DeviceTypes.Focuser, 4);
            AssertTest(MemberNames.Description, DeviceTypes.Focuser, 2);
            AssertTest(MemberNames.DeviceState, DeviceTypes.Focuser, 4);
            AssertTest(MemberNames.Disconnect, DeviceTypes.Focuser, 4);
            AssertTest(MemberNames.Dispose, DeviceTypes.Focuser, 2);
            AssertTest(MemberNames.DriverInfo, DeviceTypes.Focuser, 2);
            AssertTest(MemberNames.DriverVersion, DeviceTypes.Focuser, 2);
            AssertTest(MemberNames.Halt, DeviceTypes.Focuser, 1);
            AssertTest(MemberNames.InterfaceVersion, DeviceTypes.Focuser, 2);
            AssertTest(MemberNames.IsMoving, DeviceTypes.Focuser, 1);
            AssertTest(MemberNames.Link, DeviceTypes.Focuser, 1);
            AssertTest(MemberNames.MaxIncrement, DeviceTypes.Focuser, 1);
            AssertTest(MemberNames.MaxStep, DeviceTypes.Focuser, 1);
            AssertTest(MemberNames.Move, DeviceTypes.Focuser, 1);
            AssertTest(MemberNames.Name, DeviceTypes.Focuser, 2);
            AssertTest(MemberNames.Position, DeviceTypes.Focuser, 1);
            AssertTest(MemberNames.SetupDialog, DeviceTypes.Focuser, 1);
            AssertTest(MemberNames.StepSize, DeviceTypes.Focuser, 1);
            AssertTest(MemberNames.SupportedActions, DeviceTypes.Focuser, 2);
            AssertTest(MemberNames.TempComp, DeviceTypes.Focuser, 1);
            AssertTest(MemberNames.TempCompAvailable, DeviceTypes.Focuser, 1);
            AssertTest(MemberNames.Temperature, DeviceTypes.Focuser, 1);
        }

        [Fact]
        public void ObservingConditions()
        {
            AssertTest(MemberNames.Action, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.AveragePeriod, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.CloudCover, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.CommandBlind, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.CommandBool, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.CommandString, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.Connect, DeviceTypes.ObservingConditions, 2);
            AssertTest(MemberNames.Connected, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.Connecting, DeviceTypes.ObservingConditions, 2);
            AssertTest(MemberNames.Description, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.DeviceState, DeviceTypes.ObservingConditions, 2);
            AssertTest(MemberNames.DewPoint, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.Disconnect, DeviceTypes.ObservingConditions, 2);
            AssertTest(MemberNames.Dispose, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.DriverInfo, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.DriverVersion, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.Humidity, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.InterfaceVersion, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.Name, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.Pressure, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.RainRate, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.Refresh, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.SensorDescription, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.SetupDialog, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.SkyBrightness, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.SkyQuality, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.SkyTemperature, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.StarFWHM, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.SupportedActions, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.Temperature, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.TimeSinceLastUpdate, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.WindDirection, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.WindGust, DeviceTypes.ObservingConditions, 1);
            AssertTest(MemberNames.WindSpeed, DeviceTypes.ObservingConditions, 1);
        }

        [Fact]
        public void Rotator()
        {
            AssertTest(MemberNames.Action, DeviceTypes.Rotator, 2);
            AssertTest(MemberNames.CanReverse, DeviceTypes.Rotator, 2);
            AssertTest(MemberNames.CommandBlind, DeviceTypes.Rotator, 2);
            AssertTest(MemberNames.CommandBool, DeviceTypes.Rotator, 2);
            AssertTest(MemberNames.CommandString, DeviceTypes.Rotator, 2);
            AssertTest(MemberNames.Connect, DeviceTypes.Rotator, 4);
            AssertTest(MemberNames.Connected, DeviceTypes.Rotator, 1);
            AssertTest(MemberNames.Connecting, DeviceTypes.Rotator, 4);
            AssertTest(MemberNames.Description, DeviceTypes.Rotator, 2);
            AssertTest(MemberNames.DeviceState, DeviceTypes.Rotator, 4);
            AssertTest(MemberNames.Disconnect, DeviceTypes.Rotator, 4);
            AssertTest(MemberNames.Dispose, DeviceTypes.Rotator, 2);
            AssertTest(MemberNames.DriverInfo, DeviceTypes.Rotator, 2);
            AssertTest(MemberNames.DriverVersion, DeviceTypes.Rotator, 2);
            AssertTest(MemberNames.Halt, DeviceTypes.Rotator, 1);
            AssertTest(MemberNames.InterfaceVersion, DeviceTypes.Rotator, 2);
            AssertTest(MemberNames.IsMoving, DeviceTypes.Rotator, 1);
            AssertTest(MemberNames.MechanicalPosition, DeviceTypes.Rotator, 3);
            AssertTest(MemberNames.Move, DeviceTypes.Rotator, 1);
            AssertTest(MemberNames.MoveAbsolute, DeviceTypes.Rotator, 1);
            AssertTest(MemberNames.MoveMechanical, DeviceTypes.Rotator, 3);
            AssertTest(MemberNames.Name, DeviceTypes.Rotator, 2);
            AssertTest(MemberNames.Position, DeviceTypes.Rotator, 1);
            AssertTest(MemberNames.Reverse, DeviceTypes.Rotator, 1);
            AssertTest(MemberNames.SetupDialog, DeviceTypes.Rotator, 1);
            AssertTest(MemberNames.StepSize, DeviceTypes.Rotator, 1);
            AssertTest(MemberNames.SupportedActions, DeviceTypes.Rotator, 2);
            AssertTest(MemberNames.Sync, DeviceTypes.Rotator, 3);
            AssertTest(MemberNames.TargetPosition, DeviceTypes.Rotator, 1);
        }

        [Fact]
        public void SafetyMonitor()
        {
            AssertTest(MemberNames.Action, DeviceTypes.SafetyMonitor, 1);
            AssertTest(MemberNames.CommandBlind, DeviceTypes.SafetyMonitor, 1);
            AssertTest(MemberNames.CommandBool, DeviceTypes.SafetyMonitor, 1);
            AssertTest(MemberNames.CommandString, DeviceTypes.SafetyMonitor, 1);
            AssertTest(MemberNames.Connect, DeviceTypes.SafetyMonitor, 3);
            AssertTest(MemberNames.Connected, DeviceTypes.SafetyMonitor, 1);
            AssertTest(MemberNames.Connecting, DeviceTypes.SafetyMonitor, 3);
            AssertTest(MemberNames.Description, DeviceTypes.SafetyMonitor, 1);
            AssertTest(MemberNames.DeviceState, DeviceTypes.SafetyMonitor, 3);
            AssertTest(MemberNames.Disconnect, DeviceTypes.SafetyMonitor, 3);
            AssertTest(MemberNames.Dispose, DeviceTypes.SafetyMonitor, 1);
            AssertTest(MemberNames.DriverInfo, DeviceTypes.SafetyMonitor, 1);
            AssertTest(MemberNames.DriverVersion, DeviceTypes.SafetyMonitor, 1);
            AssertTest(MemberNames.InterfaceVersion, DeviceTypes.SafetyMonitor, 1);
            AssertTest(MemberNames.IsSafe, DeviceTypes.SafetyMonitor, 1);
            AssertTest(MemberNames.Name, DeviceTypes.SafetyMonitor, 1);
            AssertTest(MemberNames.SetupDialog, DeviceTypes.SafetyMonitor, 1);
            AssertTest(MemberNames.SupportedActions, DeviceTypes.SafetyMonitor, 1);
        }

        [Fact]
        public void Switch()
        {
            AssertTest(MemberNames.Action, DeviceTypes.Switch, 2);
            AssertTest(MemberNames.CanWrite, DeviceTypes.Switch, 2);
            AssertTest(MemberNames.CommandBlind, DeviceTypes.Switch, 2);
            AssertTest(MemberNames.CommandBool, DeviceTypes.Switch, 2);
            AssertTest(MemberNames.CommandString, DeviceTypes.Switch, 2);
            AssertTest(MemberNames.Connect, DeviceTypes.Switch, 3);
            AssertTest(MemberNames.Connected, DeviceTypes.Switch, 1);
            AssertTest(MemberNames.Connecting, DeviceTypes.Switch, 3);
            AssertTest(MemberNames.Description, DeviceTypes.Switch, 1);
            AssertTest(MemberNames.DeviceState, DeviceTypes.Switch, 3);
            AssertTest(MemberNames.Disconnect, DeviceTypes.Switch, 3);
            AssertTest(MemberNames.Dispose, DeviceTypes.Switch, 2);
            AssertTest(MemberNames.DriverInfo, DeviceTypes.Switch, 1);
            AssertTest(MemberNames.DriverVersion, DeviceTypes.Switch, 1);
            AssertTest(MemberNames.GetSwitch, DeviceTypes.Switch, 1);
            AssertTest(MemberNames.GetSwitchDescription, DeviceTypes.Switch, 2);
            AssertTest(MemberNames.GetSwitchName, DeviceTypes.Switch, 1);
            AssertTest(MemberNames.GetSwitchValue, DeviceTypes.Switch, 2);
            AssertTest(MemberNames.InterfaceVersion, DeviceTypes.Switch, 1);
            AssertTest(MemberNames.MaxSwitch, DeviceTypes.Switch, 1);
            AssertTest(MemberNames.MaxSwitchValue, DeviceTypes.Switch, 2);
            AssertTest(MemberNames.MinSwitchValue, DeviceTypes.Switch, 2);
            AssertTest(MemberNames.Name, DeviceTypes.Switch, 1);
            AssertTest(MemberNames.SetAsync, DeviceTypes.Switch, 3);
            AssertTest(MemberNames.SetAsyncValue, DeviceTypes.Switch, 3);
            AssertTest(MemberNames.SetSwitch, DeviceTypes.Switch, 1);
            AssertTest(MemberNames.SetSwitchName, DeviceTypes.Switch, 2);
            AssertTest(MemberNames.SetSwitchValue, DeviceTypes.Switch, 2);
            AssertTest(MemberNames.SetupDialog, DeviceTypes.Switch, 1);
            AssertTest(MemberNames.StateChangeComplete, DeviceTypes.Switch, 3);
            AssertTest(MemberNames.SupportedActions, DeviceTypes.Switch, 2);
            AssertTest(MemberNames.SwitchStep, DeviceTypes.Switch, 2);
        }

        [Fact]
        public void Telescope()
        {
            AssertTest(MemberNames.AbortSlew, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.Action, DeviceTypes.Telescope, 3);
            AssertTest(MemberNames.AlignmentMode, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.Altitude, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.ApertureArea, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.ApertureDiameter, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.AtHome, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.AtPark, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.AxisRates, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.Azimuth, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.CanFindHome, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.CanMoveAxis, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.CanPark, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.CanPulseGuide, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.CanSetDeclinationRate, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.CanSetGuideRates, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.CanSetPark, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.CanSetPierSide, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.CanSetRightAscensionRate, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.CanSetTracking, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.CanSlew, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.CanSlewAltAz, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.CanSlewAltAzAsync, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.CanSlewAsync, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.CanSync, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.CanSyncAltAz, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.CanUnpark, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.CommandBlind, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.CommandBool, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.CommandString, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.Connect, DeviceTypes.Telescope, 4);
            AssertTest(MemberNames.Connected, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.Connecting, DeviceTypes.Telescope, 4);
            AssertTest(MemberNames.Declination, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.DeclinationRate, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.Description, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.DestinationSideOfPier, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.DeviceState, DeviceTypes.Telescope, 4);
            AssertTest(MemberNames.Disconnect, DeviceTypes.Telescope, 4);
            AssertTest(MemberNames.Dispose, DeviceTypes.Telescope, 3);
            AssertTest(MemberNames.DoesRefraction, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.DriverInfo, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.DriverVersion, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.EquatorialSystem, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.FindHome, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.FocalLength, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.GuideRateDeclination, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.GuideRateRightAscension, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.InterfaceVersion, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.IsPulseGuiding, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.MoveAxis, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.Name, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.Park, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.PulseGuide, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.RightAscension, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.RightAscensionRate, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.SetPark, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.SetupDialog, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.SideOfPier, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.SiderealTime, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.SiteElevation, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.SiteLatitude, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.SiteLongitude, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.Slewing, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.SlewSettleTime, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.SlewToAltAz, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.SlewToAltAzAsync, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.SlewToCoordinates, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.SlewToCoordinatesAsync, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.SlewToTarget, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.SlewToTargetAsync, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.SupportedActions, DeviceTypes.Telescope, 3);
            AssertTest(MemberNames.SyncToAltAz, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.SyncToCoordinates, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.SyncToTarget, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.TargetDeclination, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.TargetRightAscension, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.Tracking, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.TrackingRate, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.TrackingRates, DeviceTypes.Telescope, 2);
            AssertTest(MemberNames.Unpark, DeviceTypes.Telescope, 1);
            AssertTest(MemberNames.UTCDate, DeviceTypes.Telescope, 1);
        }

        [Fact]
        public void Video()
        {
            AssertTest(MemberNames.Action, DeviceTypes.Video, 1);
            AssertTest(MemberNames.BitDepth, DeviceTypes.Video, 1);
            AssertTest(MemberNames.CameraState, DeviceTypes.Video, 1);
            AssertTest(MemberNames.CanConfigureDeviceProperties, DeviceTypes.Video, 1);
            AssertTest(MemberNames.CommandBlind, DeviceTypes.Video, 2);
            AssertTest(MemberNames.CommandBool, DeviceTypes.Video, 2);
            AssertTest(MemberNames.CommandString, DeviceTypes.Video, 2);
            AssertTest(MemberNames.ConfiguureDeviceProperties, DeviceTypes.Video, 1);
            AssertTest(MemberNames.Connect, DeviceTypes.Video, 2);
            AssertTest(MemberNames.Connected, DeviceTypes.Video, 1);
            AssertTest(MemberNames.Connecting, DeviceTypes.Video, 2);
            AssertTest(MemberNames.Description, DeviceTypes.Video, 1);
            AssertTest(MemberNames.DeviceState, DeviceTypes.Video, 2);
            AssertTest(MemberNames.Disconnect, DeviceTypes.Video, 2);
            AssertTest(MemberNames.Dispose, DeviceTypes.Video, 1);
            AssertTest(MemberNames.DriverInfo, DeviceTypes.Video, 1);
            AssertTest(MemberNames.DriverVersion, DeviceTypes.Video, 1);
            AssertTest(MemberNames.ExposureMax, DeviceTypes.Video, 1);
            AssertTest(MemberNames.ExposureMin, DeviceTypes.Video, 1);
            AssertTest(MemberNames.FrameRate, DeviceTypes.Video, 1);
            AssertTest(MemberNames.Gain, DeviceTypes.Video, 1);
            AssertTest(MemberNames.GainMax, DeviceTypes.Video, 1);
            AssertTest(MemberNames.GainMin, DeviceTypes.Video, 1);
            AssertTest(MemberNames.Gains, DeviceTypes.Video, 1);
            AssertTest(MemberNames.Gamma, DeviceTypes.Video, 1);
            AssertTest(MemberNames.GammaMax, DeviceTypes.Video, 1);
            AssertTest(MemberNames.GammaMin, DeviceTypes.Video, 1);
            AssertTest(MemberNames.Height, DeviceTypes.Video, 1);
            AssertTest(MemberNames.IntegrationRate, DeviceTypes.Video, 1);
            AssertTest(MemberNames.InterfaceVersion, DeviceTypes.Video, 1);
            AssertTest(MemberNames.LastVideoFrame, DeviceTypes.Video, 1);
            AssertTest(MemberNames.Name, DeviceTypes.Video, 1);
            AssertTest(MemberNames.PixelSizeX, DeviceTypes.Video, 1);
            AssertTest(MemberNames.PixelSizeY, DeviceTypes.Video, 1);
            AssertTest(MemberNames.SensorName, DeviceTypes.Video, 1);
            AssertTest(MemberNames.SensorType, DeviceTypes.Video, 1);
            AssertTest(MemberNames.SetupDialog, DeviceTypes.Video, 1);
            AssertTest(MemberNames.StartRecordingVideoFile, DeviceTypes.Video, 1);
            AssertTest(MemberNames.StopRecordingVideoFile, DeviceTypes.Video, 1);
            AssertTest(MemberNames.SupportedActions, DeviceTypes.Video, 1);
            AssertTest(MemberNames.SupportedIntegrationRates, DeviceTypes.Video, 1);
            AssertTest(MemberNames.VideoCaptureDeviceName, DeviceTypes.Video, 1);
            AssertTest(MemberNames.VideoCodec, DeviceTypes.Video, 1);
            AssertTest(MemberNames.VideoFileFormat, DeviceTypes.Video, 1);
            AssertTest(MemberNames.VideoFramesBufferSize, DeviceTypes.Video, 1);
            AssertTest(MemberNames.Width, DeviceTypes.Video, 1);
        }

        private static void AssertTest(MemberNames memberNames, DeviceTypes deviceTypes, int interfaceVersion)
        {
            // Check that the member is present in the interface version and not present in the previous version
            if (interfaceVersion > 1) // Interface version is 2 or more
            {
                Assert.True(InterfaceHasMember(memberNames, deviceTypes, interfaceVersion));
                Assert.False(InterfaceHasMember(memberNames, deviceTypes, interfaceVersion - 1));
            }
            else // InterfaceVersion version is 1 or less
            {
                Assert.True(InterfaceHasMember(memberNames, deviceTypes, interfaceVersion));
                Assert.Throws<InvalidValueException>(() => InterfaceHasMember(memberNames, deviceTypes, interfaceVersion - 1));
            }

            // Make sure that an interface version number higher than the current latest version fails
            Assert.Throws<InvalidValueException>(() => InterfaceHasMember(memberNames, deviceTypes, 99));
        }
    }
}
