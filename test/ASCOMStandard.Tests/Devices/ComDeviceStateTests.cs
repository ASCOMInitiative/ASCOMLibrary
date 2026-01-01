using ASCOM.Com.DriverAccess;
using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.DeviceStateClasses;
using ASCOM.Tools;
using Xunit;

namespace ClientToolkitTests
{
    [Collection("CameraTests")]
#if NET8_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    public class ComCameraStateTest
    {
        [Fact]
        public void Camera()
        {
            using (Camera device = new("ASCOM.Simulator.Camera"))
            {
                device.Connect();
                do
                {
                    System.Threading.Thread.Sleep(100);
                } while (device.Connecting);
                Assert.True(device.Connected);

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

    [Collection("CoverCalibratorTests")]
#if NET8_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    public class ComCoverCalibratorStateTest
    {
        [Fact]
        public void CoverCalibrator()
        {
            using (CoverCalibrator device = new("ASCOM.Simulator.CoverCalibrator"))
            {
                device.Connect();
                do
                {
                    System.Threading.Thread.Sleep(100);
                } while (device.Connecting);
                Assert.True(device.Connected);

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

    [Collection("DomeTests")]
#if NET8_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    public class ComDomeStateTest
    {
        [Fact]
        public void Dome()
        {
            using (TraceLogger traceLogger = new("ComDomeStateTest", true))
            {
                traceLogger.LogMessage("ComDomeStateTest", $"Created logger");
                using (Dome device = new("ASCOM.Simulator.Dome", traceLogger))
                {
                    traceLogger.LogMessage("ComDomeStateTest", $"Created Dome device");
                    device.Connect();
                    traceLogger.LogMessage("ComDomeStateTest", $"Connecting to device");

                    do
                    {
                        System.Threading.Thread.Sleep(100);
                    } while (device.Connecting);
                    Assert.True(device.Connected);
                    traceLogger.LogMessage("ComDomeStateTest", $"Connected to device");
                    DomeState deviceState=new DomeState();
                    try
                    {
                        deviceState = device.DomeState;
                        traceLogger.LogMessage("ComDomeStateTest", $"Got device state");

                    }
                    catch (System.Exception ex)
                    {
                        traceLogger.LogMessage("ComDomeStateTest", $"Exception: {ex}");
                    }

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
#if NET8_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    public class ComFilterWheelStateTest
    {
        [Fact]
        public void FilterWheel()
        {
            using (FilterWheel device = new("ASCOM.Simulator.FilterWheel"))
            {
                device.Connect();
                do
                {
                    System.Threading.Thread.Sleep(100);
                } while (device.Connecting);
                Assert.True(device.Connected);

                FilterWheelState deviceState = device.FilterWheelState;

                Assert.True(deviceState.Position.HasValue);
                Assert.True(deviceState.TimeStamp.HasValue);

                device.Disconnect();
            }
        }
    }

    [Collection("FocuserTests")]
#if NET8_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    public class ComFocuserStateTest
    {
        [Fact]
        public void Focuser()
        {
            using (Focuser device = new("ASCOM.Simulator.Focuser"))
            {
                device.Connect();
                do
                {
                    System.Threading.Thread.Sleep(100);
                } while (device.Connecting);
                Assert.True(device.Connected);

                FocuserState deviceState = device.FocuserState;

                Assert.True(deviceState.IsMoving.HasValue);
                Assert.True(deviceState.Position.HasValue);
                Assert.True(deviceState.Temperature.HasValue);
                Assert.True(deviceState.TimeStamp.HasValue);

                device.Disconnect();
            }
        }

    }

    [Collection("ObservingConditionsTests")]
    public class ComObservingConditionsStateTest
    {
        [Fact]
#if NET8_0_OR_GREATER
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
        public void ObservingConditions()
        {
            using (ObservingConditions device = new("ASCOM.Simulator.ObservingConditions"))
            {
                device.Connect();
                do
                {
                    System.Threading.Thread.Sleep(100);
                } while (device.Connecting);
                Assert.True(device.Connected);

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

    [Collection("RotatorTests")]
#if NET8_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    public class ComRotatorStateTest
    {
        [Fact]
        public void Rotator()
        {
            using (Rotator device = new("ASCOM.Simulator.Rotator"))
            {
                device.Connect();
                do
                {
                    System.Threading.Thread.Sleep(100);
                } while (device.Connecting);
                Assert.True(device.Connected);

                RotatorState deviceState = device.RotatorState;

                Assert.True(deviceState.IsMoving.HasValue);
                Assert.True(deviceState.MechanicalPosition.HasValue);
                Assert.True(deviceState.Position.HasValue);
                Assert.True(deviceState.TimeStamp.HasValue);

                device.Disconnect();
            }
        }

    }

    [Collection("SafetyMonitorTests")]
#if NET8_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    public class ComSafetyMonitorStateTest
    {
        [Fact]
        public void SafetyMonitor()
        {
            using (SafetyMonitor device = new("ASCOM.Simulator.SafetyMonitor"))
            {
                device.Connect();
                do
                {
                    System.Threading.Thread.Sleep(100);
                } while (device.Connecting);
                Assert.True(device.Connected);

                SafetyMonitorState deviceState = device.SafetyMonitorState;

                Assert.True(deviceState.IsSafe.HasValue);
                Assert.True(deviceState.TimeStamp.HasValue);

                device.Disconnect();
            }
        }

    }

    [Collection("TelescopeTests")]
#if NET8_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    public class ComTelescopeStateTest
    {
        [Fact]
        public void Telescope()
        {
            using (Telescope device = new("ASCOM.Simulator.Telescope"))
            {
                device.Connect();
                do
                {
                    System.Threading.Thread.Sleep(100);
                } while (device.Connecting);
                Assert.True(device.Connected);

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

    [Collection("VideoTests")]
#if NET8_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    public class ComVideoStateTest
    {
        [Fact]
        public void Video()
        {
            using (Video device = new("ASCOM.Simulator.Video"))
            {
                device.Connect();
                do
                {
                    System.Threading.Thread.Sleep(100);
                } while (device.Connecting);
                Assert.True(device.Connected);

                VideoState deviceState = device.VideoState;

                Assert.True(deviceState.CameraState.HasValue);
                Assert.Equal(nameof(IVideoV2.CameraState), nameof(VideoState.CameraState));
                Assert.True(deviceState.TimeStamp.HasValue);

                device.Disconnect();
            }
        }

    }

}
