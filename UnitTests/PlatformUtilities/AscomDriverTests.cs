#if NET8_0_OR_GREATER

using System;
using Xunit;
using ASCOM.Com;


namespace PlatformUtilitiesTests
{
    /// <summary>
    /// Integration tests using ASCOM COM drivers that are registered on this development system.
    /// Tests verify the full pipeline: registry lookup → DLL path resolution → PE analysis.
    /// </summary>
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public class AscomDrivertests : IDisposable
    {
        private static readonly ASCOM.Tools.TraceLogger logger;

        static AscomDrivertests()
        {
            logger = new ASCOM.Tools.TraceLogger("AscomDrivertests", true);
        }

        public void Dispose()
        {
        }

        [Fact]
        public void UnknownProgId()
        {
            ComDriverMetadata tester = PlatformUtilities.GetComDriverMetadata("RubbishComId.NonExistentComId", logger);
            Assert.Equal("RubbishComId.NonExistentComId", tester.ProgId);
            Assert.False(tester.IsRegistered);
            Assert.Equal(ClrVersion.Unknown, tester.ClrVersion);
            Assert.False(tester.Is32BitCompatible);
            Assert.False(tester.Is64BitCompatible);
            Assert.Equal(ComType.Unknown, tester.ComType);
            Assert.Equal(Architecture.Unknown, tester.DllArchitecture);
            Assert.Empty(tester.DllPath);
            Assert.Empty(tester.DllVersion);
        }

        [Fact]
        public void InProcesssCamera()
        {
            ComDriverMetadata tester =  PlatformUtilities.GetComDriverMetadata("ASCOM.Simulator.Camera", logger);
            Assert.Equal("ASCOM.Simulator.Camera", tester.ProgId);
            Assert.True(tester.IsRegistered);
            Assert.Equal(ClrVersion.Clr4, tester.ClrVersion);
            Assert.True(tester.Is32BitCompatible);
            Assert.True(tester.Is64BitCompatible);
            Assert.Equal(ComType.InProcess, tester.ComType);
            Assert.Equal(Architecture.Msil, tester.DllArchitecture);
            Assert.EndsWith("ASCOM.Simulator.Camera.dll", tester.DllPath, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(tester.DllVersion);
        }

        [Fact]
        public void InProcesssCameraNoLogger()
        {
            ComDriverMetadata tester = PlatformUtilities.GetComDriverMetadata("ASCOM.Simulator.Camera", null);
            Assert.Equal("ASCOM.Simulator.Camera", tester.ProgId);
            Assert.True(tester.IsRegistered);
            Assert.Equal(ClrVersion.Clr4, tester.ClrVersion);
            Assert.True(tester.Is32BitCompatible);
            Assert.True(tester.Is64BitCompatible);
            Assert.Equal(ComType.InProcess, tester.ComType);
            Assert.Equal(Architecture.Msil, tester.DllArchitecture);
            Assert.EndsWith("ASCOM.Simulator.Camera.dll", tester.DllPath, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(tester.DllVersion);
        }

        [Fact]
        public void SiTechTelescope()
        {
            ComDriverMetadata tester = PlatformUtilities.GetComDriverMetadata("Sitech.Telescope", logger);
            Assert.Equal("Sitech.Telescope", tester.ProgId);
            Assert.True(tester.IsRegistered);
            Assert.Equal(ClrVersion.Clr1, tester.ClrVersion); // This is a CLR1 assembly.
            Assert.True(tester.Is32BitCompatible);
            Assert.False(tester.Is64BitCompatible);
            Assert.Equal(ComType.InProcess, tester.ComType);
            Assert.Equal(Architecture.Msil, tester.DllArchitecture);
            Assert.EndsWith("Sitech.dll", tester.DllPath, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(tester.DllVersion);
        }

        [Fact]
        public void SiTechTelescopeNoLogger()
        {
            ComDriverMetadata tester = PlatformUtilities.GetComDriverMetadata("Sitech.Telescope", null);
            Assert.Equal("Sitech.Telescope", tester.ProgId);
            Assert.True(tester.IsRegistered);
            Assert.Equal(ClrVersion.Clr1, tester.ClrVersion); // This is a CLR1 assembly.
            Assert.True(tester.Is32BitCompatible);
            Assert.False(tester.Is64BitCompatible);
            Assert.Equal(ComType.InProcess, tester.ComType);
            Assert.Equal(Architecture.Msil, tester.DllArchitecture);
            Assert.EndsWith("Sitech.dll", tester.DllPath, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(tester.DllVersion);
        }

        [Fact]
        public void OutOfProcesssTelescope()
        {
            ComDriverMetadata tester = PlatformUtilities.GetComDriverMetadata("ASCOM.Simulator.Telescope", logger);
            Assert.Equal("ASCOM.Simulator.Telescope", tester.ProgId);
            Assert.True(tester.IsRegistered);
            Assert.Equal(ComType.OutOfProcess, tester.ComType);
            Assert.True(tester.Is32BitCompatible);
            Assert.True(tester.Is64BitCompatible);
            Assert.Equal(ClrVersion.Unknown, tester.ClrVersion);
            Assert.Equal(Architecture.Unknown, tester.DllArchitecture);
            Assert.Empty(tester.DllPath);
            Assert.Empty(tester.DllVersion);
        }

        [Fact]
        public void OutOfProcesssTelescopeNoLogger()
        {
            ComDriverMetadata tester = PlatformUtilities.GetComDriverMetadata("ASCOM.Simulator.Telescope", null);
            Assert.Equal("ASCOM.Simulator.Telescope", tester.ProgId);
            Assert.True(tester.IsRegistered);
            Assert.Equal(ComType.OutOfProcess, tester.ComType);
            Assert.True(tester.Is32BitCompatible);
            Assert.True(tester.Is64BitCompatible);
            Assert.Equal(ClrVersion.Unknown, tester.ClrVersion);
            Assert.Equal(Architecture.Unknown, tester.DllArchitecture);
            Assert.Empty(tester.DllPath);
            Assert.Empty(tester.DllVersion);
        }
    }
}

#endif
