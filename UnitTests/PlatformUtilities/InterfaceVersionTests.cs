using ASCOM;
using ASCOM.Common;
using ASCOM.Common.DeviceInterfaces;
using ASCOM.Tools;
using System;
using Xunit;
using Xunit.Abstractions;

namespace InterfaceVersionTests
{
    public class InterfaceVersionTests
    {
        private readonly ITestOutputHelper output;

        public InterfaceVersionTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void HasConnectAndDeviceState()
        {
            Assert.False(DeviceCapabilities.HasConnectAndDeviceState(DeviceTypes.Camera, 3));
            Assert.True(DeviceCapabilities.HasConnectAndDeviceState(DeviceTypes.Camera, 4));
            Assert.True(DeviceCapabilities.HasConnectAndDeviceState(DeviceTypes.Camera, 5));
        }

        [Fact]
        public void HasFunctions()
        {
            Assert.True(DeviceCapabilities.HasCalibratorChanging(2));
            Assert.True(DeviceCapabilities.HasCoverMoving(2));
            Assert.True(DeviceCapabilities.HasAsyncSwitch(3));
        }

        [Fact]
        public void IsPlatform6Interface()
        {
            Assert.False(DeviceCapabilities.IsPlatform6Interface(DeviceTypes.Camera, 2));
            Assert.True(DeviceCapabilities.IsPlatform6Interface(DeviceTypes.Camera, 3));
            Assert.False(DeviceCapabilities.IsPlatform6Interface(DeviceTypes.Camera, 4));
            Assert.False(DeviceCapabilities.IsPlatform6Interface(DeviceTypes.Camera, 5));
        }

        [Fact]
        public void IsValidAscomInterface()
        {
            Assert.False(DeviceCapabilities.IsValidAscomInterface(DeviceTypes.Camera, -1));
            Assert.False(DeviceCapabilities.IsValidAscomInterface(DeviceTypes.Camera, 0));
            Assert.True(DeviceCapabilities.IsValidAscomInterface(DeviceTypes.Camera, 1));
            Assert.True(DeviceCapabilities.IsValidAscomInterface(DeviceTypes.Camera, 2));
            Assert.True(DeviceCapabilities.IsValidAscomInterface(DeviceTypes.Camera, 3));
            Assert.True(DeviceCapabilities.IsValidAscomInterface(DeviceTypes.Camera, 4));
            Assert.False(DeviceCapabilities.IsValidAscomInterface(DeviceTypes.Camera, 5));
        }

        [Fact]
        public void IsPlatform7OrLater()
        {
            Assert.False(DeviceCapabilities.IsPlatform7OrLater(DeviceTypes.Camera, 2));
            Assert.False(DeviceCapabilities.IsPlatform7OrLater(DeviceTypes.Camera, 3));
            Assert.True(DeviceCapabilities.IsPlatform7OrLater(DeviceTypes.Camera, 4));
            Assert.True(DeviceCapabilities.IsPlatform7OrLater(DeviceTypes.Camera, 5));
        }

        [Fact]
        public void Camera()
        {
            Assert.Equal(3, DeviceCapabilities.LatestPlatform6Interface[DeviceTypes.Camera]);
            Assert.Equal(4, DeviceCapabilities.InitialPlatform7Interface[DeviceTypes.Camera]);
        }

        [Fact]
        public void CoverCalibrator()
        {
            Assert.Equal(1, DeviceCapabilities.LatestPlatform6Interface[DeviceTypes.CoverCalibrator]);
            Assert.Equal(2, DeviceCapabilities.InitialPlatform7Interface[DeviceTypes.CoverCalibrator]);
        }

        [Fact]
        public void Dome()
        {
            Assert.Equal(2, DeviceCapabilities.LatestPlatform6Interface[DeviceTypes.Dome]);
            Assert.Equal(3, DeviceCapabilities.InitialPlatform7Interface[DeviceTypes.Dome]);
        }

        [Fact]
        public void FilterWheel()
        {
            Assert.Equal(2, DeviceCapabilities.LatestPlatform6Interface[DeviceTypes.FilterWheel]);
            Assert.Equal(3, DeviceCapabilities.InitialPlatform7Interface[DeviceTypes.FilterWheel]);
        }

        [Fact]
        public void Focuser()
        {
            Assert.False(DeviceCapabilities.HasConnected(DeviceTypes.Focuser, 1));
            Assert.Equal(3, DeviceCapabilities.LatestPlatform6Interface[DeviceTypes.Focuser]);
            Assert.Equal(4, DeviceCapabilities.InitialPlatform7Interface[DeviceTypes.Focuser]);
        }

        [Fact]
        public void ObservingConditions()
        {
            Assert.Equal(1, DeviceCapabilities.LatestPlatform6Interface[DeviceTypes.ObservingConditions]);
            Assert.Equal(2, DeviceCapabilities.InitialPlatform7Interface[DeviceTypes.ObservingConditions]);
        }

        [Fact]
        public void Rotator()
        {
            Assert.Equal(3, DeviceCapabilities.LatestPlatform6Interface[DeviceTypes.Rotator]);
            Assert.Equal(4, DeviceCapabilities.InitialPlatform7Interface[DeviceTypes.Rotator]);
        }

        [Fact]
        public void SafetyMonitor()
        {
            Assert.Equal(1, DeviceCapabilities.LatestPlatform6Interface[DeviceTypes.SafetyMonitor]);
            Assert.Equal(3, DeviceCapabilities.InitialPlatform7Interface[DeviceTypes.SafetyMonitor]);
        }

        [Fact]
        public void Switch()
        {
            Assert.Equal(2, DeviceCapabilities.LatestPlatform6Interface[DeviceTypes.Switch]);
            Assert.Equal(3, DeviceCapabilities.InitialPlatform7Interface[DeviceTypes.Switch]);
        }

        [Fact]
        public void Telescope()
        {
            Assert.True(DeviceCapabilities.HasConnected(DeviceTypes.Telescope, 1));
            Assert.Equal(3, DeviceCapabilities.LatestPlatform6Interface[DeviceTypes.Telescope]);
            Assert.Equal(4, DeviceCapabilities.InitialPlatform7Interface[DeviceTypes.Telescope]);
        }

        [Fact]
        public void Video()
        {
            Assert.Equal(1, DeviceCapabilities.LatestPlatform6Interface[DeviceTypes.Video]);
            Assert.Equal(2, DeviceCapabilities.InitialPlatform7Interface[DeviceTypes.Video]);
        }
    }
}
