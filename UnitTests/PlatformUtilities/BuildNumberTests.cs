using Xunit;

namespace PlatformUtilitiesTests
{
#if NET8_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    public class BuildNumberTests
    {
        private readonly ITestOutputHelper output;

        public BuildNumberTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Build_0()
        {
            Assert.Equal("Unknown OS version (0)", ASCOM.Com.PlatformUtilities.OSBuildName(0));
        }

        [Fact]
        public void Build_5000()
        {
            Assert.Equal("Earlier than Windows 10 (build < 10000)", ASCOM.Com.PlatformUtilities.OSBuildName(5000));
        }
        
        [Fact]
        public void Build_26200()
        {
            Assert.Equal("Windows 11 (25H2)", ASCOM.Com.PlatformUtilities.OSBuildName(26200));
        }
        
        [Fact]
        public void Build_26300()
        {
            Assert.Equal($"Windows 11 (26H2)", ASCOM.Com.PlatformUtilities.OSBuildName(26300));
        }
        [Fact]
        public void Build_28000()
        {
            Assert.Equal("Windows 11 (26H1)", ASCOM.Com.PlatformUtilities.OSBuildName(28000));
        }

        [Fact]
        public void Build_28020()
        {
            Assert.Equal($"Windows 11 (26H1)", ASCOM.Com.PlatformUtilities.OSBuildName(28020));
        }
        
        [Fact]
        public void Build_28100()
        {
            Assert.Equal($"Windows 11 (26H1)", ASCOM.Com.PlatformUtilities.OSBuildName(28100));
        }

        [Fact]
        public void Build_29020()
        {
            Assert.Equal($"Windows 11 (27H2)", ASCOM.Com.PlatformUtilities.OSBuildName(29020));
        }

        [Fact]
        public void Build_99999()
        {
            Assert.Equal($"Windows 11 (27H2) or later", ASCOM.Com.PlatformUtilities.OSBuildName(99999));
        }
    }
}