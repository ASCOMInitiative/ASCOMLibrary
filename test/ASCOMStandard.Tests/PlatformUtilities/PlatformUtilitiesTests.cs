using ASCOM;
using ASCOM.Common;
using ASCOM.Tools;
using System;
using Xunit;
using Xunit.Abstractions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UtilitityTests
{
    public class UtilityTests
    {
        private readonly ITestOutputHelper output;

        public UtilityTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void IsPlatformInstalled()
        {
            Assert.True(ASCOM.Com.PlatformUtilities.IsPlatformInstalled());
        }

        [Fact]
        public void CurrentPlatformVersion()
        {
            Assert.True(ASCOM.Com.PlatformUtilities.PlatformVersion == "7.1");
        }

        [Fact]
        public void IsPlatformVersionOK()
        {
            Assert.True(ASCOM.Com.PlatformUtilities.IsMinimumRequiredVersion(7, 1, 1, 3001));
        }

        [Fact]
        public void IsPlatformVersionBad()
        {
            Assert.False(ASCOM.Com.PlatformUtilities.IsMinimumRequiredVersion(7, 1, 3, 3001));
        }

        [Fact]
        public void IsPlatformVersionBadLowMajor()
        {
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.PlatformUtilities.IsMinimumRequiredVersion(0, 0, 0, 0));
        }

        [Fact]
        public void IsPlatformVersionBadHighMajor()
        {
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.PlatformUtilities.IsMinimumRequiredVersion(8, 0, 0, 0));
        }

        [Fact]
        public void IsPlatformVersionBadLowMinor()
        {
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.PlatformUtilities.IsMinimumRequiredVersion(7, -1, 0, 0));
        }

        [Fact]
        public void IsPlatformVersionBadHighMinor()
        {
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.PlatformUtilities.IsMinimumRequiredVersion(7, 7, 0, 0));
        }
        [Fact]
        public void IsPlatformVersionBadLowServicePack()
        {
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.PlatformUtilities.IsMinimumRequiredVersion(7, 0, -1, 0));
        }

        [Fact]
        public void IsPlatformVersionBadHighServicePack()
        {
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.PlatformUtilities.IsMinimumRequiredVersion(7, 0, 10, 0));
        }
        [Fact]
        public void IsPlatformVersionBadLowBuild()
        {
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.PlatformUtilities.IsMinimumRequiredVersion(7, 0, 0, -1));
        }

        [Fact]
        public void IsPlatformVersionBadHighBuild()
        {
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.PlatformUtilities.IsMinimumRequiredVersion(7, 0, 0, 65536));
        }

        [Fact]
        public void PlatformMajor()
        {
            Assert.True(ASCOM.Com.PlatformUtilities.MajorVersion == 7);
        }

        [Fact]
        public void PlatformMinor()
        {
            Assert.True(ASCOM.Com.PlatformUtilities.MinorVersion == 1);
        }

        [Fact]
        public void PlatformServicePack()
        {
            Assert.True(ASCOM.Com.PlatformUtilities.ServicePack == 2);
        }

        [Fact]
        public void PlatformBuild()
        {
            Assert.True(ASCOM.Com.PlatformUtilities.BuildNumber > 3000);
        }

        [Fact]
        public void CreateAlpacaDevice()
        {
            string uniqueIdGuid = Guid.NewGuid().ToString();
            TraceLogger TL = new("PlatformUtilities", true);
            TL.LogMessage("CreateAlpacaDevice", "Before SetLogger");
            ASCOM.Com.PlatformUtilities.SetLogger(TL);
            TL.LogMessage("CreateAlpacaDevice", "Before CreateDynamicDriver");
            string progId = ASCOM.Com.PlatformUtilities.CreateDynamicDriver(DeviceTypes.SafetyMonitor, 0, "Unit test Safety Monitor description", "127.0.0.1", 11111, uniqueIdGuid);
            TL.LogMessage("CreateAlpacaDevice", $"After CreateDynamicDriver - ProgId: '{progId}'");

            Assert.Contains("SafetyMonitor", progId, StringComparison.CurrentCultureIgnoreCase);
        }

        [Fact]
        public void JulianDateUtc()
        {
            double julianDateUtc = AstroUtilities.JulianDateUtc;
            Assert.Equal(DateTime.UtcNow.ToOADate() + 2415018.5, julianDateUtc, 6);
        }

        [Fact]
        public void DeltaT()
        {
            // Current leap second value - needs to be updated whenever the number of leap seconds changes
            const double CURRENT_LEAP_SECONDS = 37.0;

            double deltatDifference = Math.Abs(AstroUtilities.DeltaT(AstroUtilities.JulianDateUtc) - CURRENT_LEAP_SECONDS - 32.184);
            output.WriteLine($"Delta difference: {deltatDifference}");

            Assert.True(deltatDifference < 0.5);
        }

    }
}
