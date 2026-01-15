using ASCOM.Alpaca.Clients;
using ASCOM.Common.Alpaca;
using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.DeviceStateClasses;
using ASCOM.Tools;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ClientToolkitTests
{
    public class AlpacaCameraStateTest
    {
        [Fact]
        public void Camera()
        {
            using (TraceLogger logger = new TraceLogger("AlpacaCamera", true))
            {
                logger.SetMinimumLoggingLevel(ASCOM.Common.Interfaces.LogLevel.Debug);

                using (AlpacaCamera device = new AlpacaCamera(ServiceType.Http, "127.0.0.1", 32323, 0, true, logger))
                {
                    device.Connect();
                    do
                    {
                        System.Threading.Thread.Sleep(100);
                    } while (device.Connecting);

                    Assert.True(device.Connected);
                    logger.LogMessage("AlpacaCamera", $"Connected OK");
                    List<StateValue> values = device.DeviceState;
                    logger.LogMessage("AlpacaCamera", $"Got device state OK - Count: {values.Count}");
                    Assert.True(values.Count > 0);

                    CameraDeviceState deviceState = device.CameraDeviceState;

                    Assert.True(deviceState.CameraState.HasValue);
                    Assert.True(deviceState.CCDTemperature.HasValue);
                    Assert.True(deviceState.CoolerPower.HasValue);
                    Assert.True(deviceState.HeatSinkTemperature.HasValue);
                    Assert.True(deviceState.ImageReady.HasValue);
                    Assert.True(deviceState.IsPulseGuiding.HasValue);
                    Assert.True(deviceState.PercentCompleted.HasValue);
                    Assert.True(deviceState.TimeStamp.HasValue);

                    device.Disconnect();
                }
            }
        }
    }

    [Collection("CoverCalibratorTests")]
    public class AlpacaCoverCalibratorStateTest
    {
        [Fact]
        public void CoverCalibrator()
        {
            using (TraceLogger logger = new TraceLogger("AlpacaCoverCalibrator", true))
            {
                logger.SetMinimumLoggingLevel(ASCOM.Common.Interfaces.LogLevel.Debug);

                using (AlpacaCoverCalibrator device = new AlpacaCoverCalibrator(ServiceType.Http, "127.0.0.1", 32323, 0, true, logger))
                {
                    device.Connect();
                    do
                    {
                        System.Threading.Thread.Sleep(100);
                    } while (device.Connecting);

                    Assert.True(device.Connected);
                    logger.LogMessage("AlpacaCoverCalibrator", $"Connected OK");

                    int count = device.DeviceState.Count();

                    Assert.True(count > 0);
                    logger.LogMessage("AlpacaCoverCalibrator", $"Device state count: {count}");
                    CoverCalibratorState deviceState = device.CoverCalibratorState;

                    Assert.True(deviceState.Brightness.HasValue);
                    Assert.True(deviceState.CalibratorChanging.HasValue);
                    Assert.True(deviceState.CalibratorState.HasValue);
                    Assert.True(deviceState.CoverMoving.HasValue);
                    Assert.True(deviceState.CoverState.HasValue);
                    Assert.True(deviceState.TimeStamp.HasValue);

                    device.Disconnect();
                }
            }
        }
    }

    [Collection("DomeTests")]
    public class AlpacaDomeStateTest
    {
        [Fact]
        public void Dome()
        {
            using (TraceLogger logger = new TraceLogger("AlpacaDome", true))
            {
                logger.SetMinimumLoggingLevel(ASCOM.Common.Interfaces.LogLevel.Debug);

                using (AlpacaDome device = new AlpacaDome(ServiceType.Http, "127.0.0.1", 32323, 0, true, logger))
                {
                    device.Connect();
                    do
                    {
                        System.Threading.Thread.Sleep(100);
                    } while (device.Connecting);

                    Assert.True(device.Connected);
                    logger.LogMessage("AlpacaDome", $"Connected OK");

                    int count = device.DeviceState.Count();

                    Assert.True(count > 0);
                    logger.LogMessage("AlpacaDome", $"Device state count: {count}");
                    DomeState deviceState = device.DomeState;

                    Assert.True(deviceState.Altitude.HasValue);
                    Assert.True(deviceState.AtHome.HasValue);
                    Assert.True(deviceState.AtPark.HasValue);
                    Assert.True(deviceState.Azimuth.HasValue);
                    Assert.True(deviceState.ShutterStatus.HasValue);
                    Assert.True(deviceState.Slewing.HasValue);
                    Assert.True(deviceState.TimeStamp.HasValue);

                    device.Disconnect();
                }
            }
        }
    }

    [Collection("FilterWheelTests")]
    public class AlpacaFilterWheelStateTest
    {
        [Fact]
        public void FilterWheel()
        {
            using (TraceLogger logger = new TraceLogger("AlpacaFilterWheel", true))
            {
                logger.SetMinimumLoggingLevel(ASCOM.Common.Interfaces.LogLevel.Debug);

                using (AlpacaFilterWheel device = new AlpacaFilterWheel(ServiceType.Http, "127.0.0.1", 32323, 0, true, logger))
                {
                    device.Connect();
                    do
                    {
                        System.Threading.Thread.Sleep(100);
                    } while (device.Connecting);

                    Assert.True(device.Connected);
                    logger.LogMessage("AlpacaFilterWheel", $"Connected OK");

                    int count = device.DeviceState.Count();

                    Assert.True(count > 0);
                    logger.LogMessage("AlpacaFilterWheel", $"Device state count: {count}");
                    FilterWheelState deviceState = device.FilterWheelState;

                    Assert.True(deviceState.Position.HasValue);
                    Assert.True(deviceState.TimeStamp.HasValue);

                    device.Disconnect();
                }
            }
        }
    }

    [Collection("FocuserTests")]
    public class AlpacaFocuserStateTest
    {
        [Fact]
        public void Focuser()
        {
            using (TraceLogger logger = new TraceLogger("AlpacaFocuser", true))
            {
                logger.SetMinimumLoggingLevel(ASCOM.Common.Interfaces.LogLevel.Debug);

                using (AlpacaFocuser device = new AlpacaFocuser(ServiceType.Http, "127.0.0.1", 32323, 0, true, logger))
                {
                    device.Connect();
                    do
                    {
                        System.Threading.Thread.Sleep(100);
                    } while (device.Connecting);

                    Assert.True(device.Connected);
                    logger.LogMessage("AlpacaFocuser", $"Connected OK");

                    int count = device.DeviceState.Count();

                    Assert.True(count > 0);
                    logger.LogMessage("AlpacaFocuser", $"Device state count: {count}");

                    FocuserState deviceState = device.FocuserState;

                    Assert.True(deviceState.IsMoving.HasValue);
                    Assert.True(deviceState.Position.HasValue);
                    Assert.True(deviceState.Temperature.HasValue);
                    Assert.True(deviceState.TimeStamp.HasValue);

                    device.Disconnect();
                }
            }
        }
    }

    [Collection("ObservingConditionsTests")]
    public class AlpacaObservingConditionsStateTest
    {
        [Fact]
        public void ObservingConditions()
        {
            using (TraceLogger logger = new TraceLogger("AlpacaObservingConditions", true))
            {
                logger.SetMinimumLoggingLevel(ASCOM.Common.Interfaces.LogLevel.Debug);

                using (AlpacaObservingConditions device = new AlpacaObservingConditions(ServiceType.Http, "127.0.0.1", 32323, 0, true, logger))
                {
                    device.Connect();
                    do
                    {
                        System.Threading.Thread.Sleep(100);
                    } while (device.Connecting);

                    Assert.True(device.Connected);
                    logger.LogMessage("AlpacaObservingConditions", $"Connected OK");

                    int count = device.DeviceState.Count();

                    Assert.True(count > 0);
                    logger.LogMessage("AlpacaObservingConditions", $"Device state count: {count}");
                    ObservingConditionsState deviceState = device.ObservingConditionsState;

                    Assert.True(deviceState.CloudCover.HasValue);
                    Assert.True(deviceState.DewPoint.HasValue);
                    Assert.True(deviceState.Humidity.HasValue);
                    Assert.True(deviceState.Pressure.HasValue);
                    Assert.True(deviceState.RainRate.HasValue);
                    Assert.True(deviceState.SkyBrightness.HasValue);
                    Assert.True(deviceState.SkyQuality.HasValue);
                    Assert.True(deviceState.SkyTemperature.HasValue);
                    Assert.True(deviceState.StarFWHM.HasValue);
                    Assert.True(deviceState.Temperature.HasValue);
                    Assert.True(deviceState.WindDirection.HasValue);
                    Assert.True(deviceState.WindGust.HasValue);
                    Assert.True(deviceState.WindSpeed.HasValue);
                    Assert.True(deviceState.TimeStamp.HasValue);

                    device.Disconnect();
                }
            }
        }
    }

    [Collection("RotatorTests")]
    public class AlpacaRotatorStateTest
    {
        [Fact]
        public void Rotator()
        {
            using (TraceLogger logger = new TraceLogger("AlpacaRotator", true))
            {
                logger.SetMinimumLoggingLevel(ASCOM.Common.Interfaces.LogLevel.Debug);

                using (AlpacaRotator device = new AlpacaRotator(ServiceType.Http, "127.0.0.1", 32323, 0, true, logger))
                {
                    device.Connect();
                    do
                    {
                        System.Threading.Thread.Sleep(100);
                    } while (device.Connecting);

                    Assert.True(device.Connected);
                    logger.LogMessage("AlpacaRotator", $"Connected OK");

                    int count = device.DeviceState.Count();

                    Assert.True(count > 0);
                    logger.LogMessage("AlpacaRotator", $"Device state count: {count}"); RotatorState deviceState = device.RotatorState;

                    Assert.True(deviceState.IsMoving.HasValue);
                    Assert.True(deviceState.MechanicalPosition.HasValue);
                    Assert.True(deviceState.Position.HasValue);
                    Assert.True(deviceState.TimeStamp.HasValue);

                    device.Disconnect();
                }
            }
        }
    }

    [Collection("SafetyMonitorTests")]
    public class AlpacaSafetyMonitorStateTest
    {
        [Fact]
        public void SafetyMonitor()
        {
            using (TraceLogger logger = new TraceLogger("AlpacaSafetyMonitor", true))
            {
                logger.SetMinimumLoggingLevel(ASCOM.Common.Interfaces.LogLevel.Debug);

                using (AlpacaSafetyMonitor device = new AlpacaSafetyMonitor(ServiceType.Http, "127.0.0.1", 32323, 0, true, logger))
                {
                    device.Connect();
                    do
                    {
                        System.Threading.Thread.Sleep(100);
                    } while (device.Connecting);

                    Assert.True(device.Connected);
                    logger.LogMessage("AlpacaSafetyMonitor", $"Connected OK");

                    int count = device.DeviceState.Count();

                    Assert.True(count > 0);
                    logger.LogMessage("AlpacaSafetyMonitor", $"Device state count: {count}");
                    SafetyMonitorState deviceState = device.SafetyMonitorState;

                    Assert.True(deviceState.IsSafe.HasValue);
                    Assert.True(deviceState.TimeStamp.HasValue);

                    device.Disconnect();
                }
            }
        }
    }

    [Collection("TelescopeTests")]
    public class AlpacaTelescopeStateTest
    {
        [Fact]
        public void Telescope()
        {
            using (TraceLogger logger = new TraceLogger("AlpacaTelescope", true))
            {
                logger.SetMinimumLoggingLevel(ASCOM.Common.Interfaces.LogLevel.Debug);

                using (AlpacaTelescope device = new AlpacaTelescope(ServiceType.Http, "127.0.0.1", 32323, 0, true, logger))
                {
                    device.Connect();
                    do
                    {
                        System.Threading.Thread.Sleep(100);
                    } while (device.Connecting);

                    Assert.True(device.Connected);
                    logger.LogMessage("AlpacaTelescope", $"Connected OK");

                    int count = device.DeviceState.Count();

                    Assert.True(count > 0);
                    logger.LogMessage("AlpacaTelescope", $"Device state count: {count}");
                    TelescopeState deviceState = device.TelescopeState;

                    Assert.True(deviceState.Altitude.HasValue);
                    Assert.True(deviceState.AtHome.HasValue);
                    Assert.True(deviceState.AtPark.HasValue);
                    Assert.True(deviceState.Azimuth.HasValue);
                    Assert.True(deviceState.Declination.HasValue);
                    Assert.True(deviceState.IsPulseGuiding.HasValue);
                    Assert.True(deviceState.RightAscension.HasValue);
                    Assert.True(deviceState.SideOfPier.HasValue);
                    Assert.True(deviceState.SiderealTime.HasValue);
                    Assert.True(deviceState.Slewing.HasValue);
                    Assert.True(deviceState.Tracking.HasValue);
                    Assert.True(deviceState.UTCDate.HasValue);
                    Assert.True(deviceState.TimeStamp.HasValue);

                    device.Disconnect();
                }
            }
        }
    }
}
