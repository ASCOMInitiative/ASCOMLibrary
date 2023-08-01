﻿using ASCOM.Com.DriverAccess;
using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.DeviceStateClasses;
using System;
using Xunit;

namespace DriverAccessTests
{
    [Collection("CameraTests")]
    public class CameraStateTest
    {
        [Fact]
        public void Camera()
        {
            using (Camera device = new Camera("ASCOM.Simulator.Camera"))
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
    public class CoverCalibratorStateTest
    { 
        [Fact]
        public void CoverCalibrator()
        {
            using (CoverCalibrator device = new CoverCalibrator("ASCOM.Simulator.CoverCalibrator"))
            {
                device.Connect();
                do
                {
                    System.Threading.Thread.Sleep(100);
                } while (device.Connecting);
                Assert.True(device.Connected);

                CoverCalibratorState deviceState = device.CoverCalibratorState;

                Assert.True(deviceState.Brightness.HasValue);
                Assert.True(deviceState.CalibratorReady.HasValue);
                Assert.True(deviceState.CalibratorState.HasValue);
                Assert.True(deviceState.CoverMoving.HasValue);
                Assert.True(deviceState.CoverState.HasValue);
                Assert.True(deviceState.TimeStamp.HasValue);

                device.Disconnect();
            }
        }

    }

    [Collection("DomeTests")]
    public class DomeStateTest
    {
        [Fact]
        public void Dome()
        {
            using (Dome device = new Dome("ASCOM.Simulator.Dome"))
            {
                device.Connect();
                do
                {
                    System.Threading.Thread.Sleep(100);
                } while (device.Connecting);
                Assert.True(device.Connected);

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

    [Collection("FilterWheelTests")]
    public class FilterWheelStateTest
    {
        [Fact]
        public void FilterWheel()
        {
            using (FilterWheel device = new FilterWheel("ASCOM.Simulator.FilterWheel"))
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
    public class FocuserStateTest
    {
        [Fact]
        public void Focuser()
        {
            using (Focuser device = new Focuser("ASCOM.Simulator.Focuser"))
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
    public class ObservingConditionsStateTest
    {
        [Fact]
        public void ObservingConditions()
        {
            using (ObservingConditions device = new ObservingConditions("ASCOM.Simulator.ObservingConditions"))
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
    public class RotatorStateTest
    {
        [Fact]
        public void Rotator()
        {
            using (Rotator device = new Rotator("ASCOM.Simulator.Rotator"))
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
    public class SafetyMonitorStateTest
    {
        [Fact]
        public void SafetyMonitor()
        {
            using (SafetyMonitor device = new SafetyMonitor("ASCOM.Simulator.SafetyMonitor"))
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
    public class TelescopeStateTest
    {
        [Fact]
        public void Telescope()
        {
            using (Telescope device = new Telescope("ASCOM.Simulator.Telescope"))
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
    public class VideoStateTest
    {
        [Fact]
        public void Video()
        {
            using (Video device = new Video("ASCOM.Simulator.Video"))
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
