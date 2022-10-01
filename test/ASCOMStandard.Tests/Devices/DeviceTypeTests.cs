using ASCOM.Common;
using Xunit;
using static ASCOM.Common.Devices;


namespace ASCOM.Alpaca.Tests.Devices
{
    public class DeviceTypeTests
    {
        [Fact]
        public void StrToDeviceTypeCorrectCase()
        {
            Assert.True(StringToDeviceType("Telescope") == DeviceTypes.Telescope);
        }

        [Fact]
        public void StrToDeviceTypeWrongCase()
        {
            Assert.True(StringToDeviceType("cAMERA") == DeviceTypes.Camera);
        }

        [Fact]
        public void StrToDeviceTypeBadValue()
        {
            Assert.Throws<InvalidValueException>(() => StringToDeviceType( "InvalidDeviceName"));
        }

        [Fact]
        public void DeviceTypeToStrCorrectCase1()
        {
            Assert.True(DeviceTypeToString(DeviceTypes.Telescope) == "Telescope");
        }

        [Fact]
        public void DeviceTypeToStrCorrectCase2()
        {
            Assert.True(DeviceTypeToString(DeviceTypes.Camera) == "Camera");
        }

        [Fact]
        public void DeviceTypeToStrBadValue()
        {
            Assert.Throws<InvalidValueException>(() => DeviceTypeToString((DeviceTypes) 9999));
        }

        [Fact]
        public void IsValidDeviceTypeCamera()
        {
            Assert.True(IsValidDeviceType(DeviceTypes.Camera));
        }

        [Fact]
        public void IsValidDeviceTypeCoverCalibrator()
        {
            Assert.True(IsValidDeviceType(DeviceTypes.CoverCalibrator));
        }

        [Fact]
        public void IsValidDeviceTypeDome()
        {
            Assert.True(IsValidDeviceType(DeviceTypes.Dome));
        }

        [Fact]
        public void IsValidDeviceTypeFilterWheel()
        {
            Assert.True(IsValidDeviceType(DeviceTypes.FilterWheel));
        }

        [Fact]
        public void IsValidDeviceTypeFocuse()
        {
            Assert.True(IsValidDeviceType(DeviceTypes.Focuser));
        }

        [Fact]
        public void IsValidDeviceTypeObservingConditions()
        {
            Assert.True(IsValidDeviceType(DeviceTypes.ObservingConditions));
        }
        [Fact]
        public void IsValidDeviceTypeRotator()
        {
            Assert.True(IsValidDeviceType(DeviceTypes.Rotator));
        }
        [Fact]
        public void IsValidDeviceTypeSafetyMonitor()
        {
            Assert.True(IsValidDeviceType(DeviceTypes.SafetyMonitor));
        }
        [Fact]
        public void IsValidDeviceTypeSwitch()
        {
            Assert.True(IsValidDeviceType(DeviceTypes.Switch));
        }
        [Fact]
        public void IsValidDeviceTypeTelescope()
        {
            Assert.True(IsValidDeviceType(DeviceTypes.Telescope));
        }
        [Fact]
        public void IsValidDeviceTypeVideo()
        {
            Assert.True(IsValidDeviceType(DeviceTypes.Video));
        }
        [Fact]
        public void IsValidDeviceTypeBadValue()
        {
            Assert.False(IsValidDeviceType((DeviceTypes)Profile.Test.BAD_DEVICE_TYPE_VALUE));
        }

        [Fact]
        public void DeviceTypeNamesOk()
        {
            Assert.Collection(DeviceTypeNames(),
                item => Assert.Equal("Camera", item),
                item => Assert.Equal("CoverCalibrator", item),
                item => Assert.Equal("Dome", item),
                item => Assert.Equal("FilterWheel", item),
                item => Assert.Equal("Focuser", item),
                item => Assert.Equal("ObservingConditions", item),
                item => Assert.Equal("Rotator", item),
                item => Assert.Equal("SafetyMonitor", item),
                item => Assert.Equal("Switch", item),
                item => Assert.Equal("Telescope", item),
                item => Assert.Equal("Video", item)
            );
        }

        [Fact]
        public void DeviceTypeNamesBad()
        {
            Assert.DoesNotContain("BadDeviceName", DeviceTypeNames());
        }
    }
}
