using ASCOM.Common;
using ASCOM.Tools;
using System;
using Xunit;

namespace ASCOM.Alpaca.Tests.PlatformUtilities
{
    public class PlatformUtilitiesTests
    {
        [Fact]
        public void CurrentPlatformVersion()
        {
            Assert.True(Com.PlatformUtilities.PlatformVersion == "6.6");
        }

        [Fact]
        public void IsPlatformVersionOK()
        {
            Assert.True(Com.PlatformUtilities.IsMinimumRequiredVersion(6, 6, 1, 3001));
        }

        [Fact]
        public void IsPlatformVersionBad()
        {
            Assert.False(Com.PlatformUtilities.IsMinimumRequiredVersion(6, 6, 3, 3001));
        }

        [Fact]
        public void IsPlatformVersionBadLowMajor()
        {
            Assert.Throws<InvalidValueException>(() => Com.PlatformUtilities.IsMinimumRequiredVersion(0, 0, 0, 0));
        }

        [Fact]
        public void IsPlatformVersionBadHighMajor()
        {
            Assert.Throws<InvalidValueException>(() => Com.PlatformUtilities.IsMinimumRequiredVersion(7, 0, 0, 0));
        }

        [Fact]
        public void IsPlatformVersionBadLowMinor()
        {
            Assert.Throws<InvalidValueException>(() => Com.PlatformUtilities.IsMinimumRequiredVersion(6, -1, 0, 0));
        }

        [Fact]
        public void IsPlatformVersionBadHighMinor()
        {
            Assert.Throws<InvalidValueException>(() => Com.PlatformUtilities.IsMinimumRequiredVersion(6, 7, 0, 0));
        }
        [Fact]
        public void IsPlatformVersionBadLowServicePack()
        {
            Assert.Throws<InvalidValueException>(() => Com.PlatformUtilities.IsMinimumRequiredVersion(6, 6, -1, 0));
        }

        [Fact]
        public void IsPlatformVersionBadHighServicePack()
        {
            Assert.Throws<InvalidValueException>(() => Com.PlatformUtilities.IsMinimumRequiredVersion(6, 6, 10, 0));
        }
        [Fact]
        public void IsPlatformVersionBadLowBuild()
        {
            Assert.Throws<InvalidValueException>(() => Com.PlatformUtilities.IsMinimumRequiredVersion(6, 6, 2, -1));
        }

        [Fact]
        public void IsPlatformVersionBadHighBuild()
        {
            Assert.Throws<InvalidValueException>(() => Com.PlatformUtilities.IsMinimumRequiredVersion(6, 6, 2, 65536));
        }

        [Fact]
        public void PlatformMajor()
        {
            Assert.True(Com.PlatformUtilities.MajorVersion == 6);
        }

        [Fact]
        public void PlatformMinor()
        {
            Assert.True(Com.PlatformUtilities.MinorVersion == 6);
        }

        [Fact]
        public void PlatformServicePack()
        {
            Assert.True(Com.PlatformUtilities.ServicePack == 2);
        }

        [Fact]
        public void PlatformBuild()
        {
            Assert.True(Com.PlatformUtilities.BuildNumber > 3000);
        }

        [Fact]
        public void CreateAlpacaDevice()
        {
            string uniqueIdGuid = Guid.NewGuid().ToString();
            TraceLogger TL = new("PlatformUtilities", true);
            TL.LogMessage("CreateAlpacaDevice", "Before SetLogger");
            Com.PlatformUtilities.SetLogger(TL);
            TL.LogMessage("CreateAlpacaDevice", "Before CreateDynamicDriver");
            string progId = Com.PlatformUtilities.CreateDynamicDriver(DeviceTypes.SafetyMonitor, 0, "Unit test Safety Monitor description", "127.0.0.1", 11111, uniqueIdGuid);
            TL.LogMessage("CreateAlpacaDevice", $"After CreateDynamicDriver - ProgId: '{progId}'");

            Assert.Contains("SafetyMonitor", progId,StringComparison.CurrentCultureIgnoreCase);
        }


    }
}
