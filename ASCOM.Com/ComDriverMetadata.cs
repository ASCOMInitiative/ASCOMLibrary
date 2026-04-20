#if NET8_0_OR_GREATER
using System.Runtime.Versioning;

namespace ASCOM.Com
{

    /// <summary>
    /// Immutable snapshot of the properties discovered for a COM driver.
    /// Created by <see cref="ComDriverProperties.GetComDriverMetadata"/>.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class ComDriverMetadata
    {
        /// <summary>
        /// Initialises a new instance of <see cref="ComDriverMetadata"/> with all property values.
        /// </summary>
        internal ComDriverMetadata(
            string progId,
            bool isRegistered,
            ComType comType,
            string? dllPath,
            string? dllVersion,
            Architecture dllArchitecture,
            bool is32BitCompatible,
            bool is64BitCompatible,
            ClrVersion clrVersion)
        {
            ProgId = progId;
            IsRegistered = isRegistered;
            ComType = comType;
            DllPath = dllPath;
            DllVersion = dllVersion;
            DllArchitecture = dllArchitecture;
            Is32BitCompatible = is32BitCompatible;
            Is64BitCompatible = is64BitCompatible;
            ClrVersion = clrVersion;
        }

        /// <summary>The ProgID supplied when creating this metadata.</summary>
        public string ProgId { get; }

        /// <summary>True if the ProgID is registered on this system.</summary>
        public bool IsRegistered { get; }

        /// <summary>Whether the driver is an in-process (DLL) or out-of-process (EXE) COM server.</summary>
        public ComType ComType { get; }

        /// <summary>
        /// Full path of the DLL implementing the COM driver.
        /// Only populated for <see cref="ComType.InProcess"/> drivers.
        /// </summary>
        public string? DllPath { get; }

        /// <summary>
        /// File version of the COM driver DLL.
        /// Only populated for <see cref="ComType.InProcess"/> drivers.
        /// </summary>
        public string? DllVersion { get; }

        /// <summary>
        /// Processor architecture of the COM driver DLL.
        /// Only populated for <see cref="ComType.InProcess"/> drivers.
        /// </summary>
        public Architecture DllArchitecture { get; }

        /// <summary>
        /// True if the DLL can run on a 32-bit operating system.
        /// Only meaningful for <see cref="ComType.InProcess"/> drivers.
        /// </summary>
        public bool Is32BitCompatible { get; }

        /// <summary>
        /// True if the DLL can run on a 64-bit operating system.
        /// Only meaningful for <see cref="ComType.InProcess"/> drivers.
        /// </summary>
        public bool Is64BitCompatible { get; }

        /// <summary>
        /// The CLR version for which the DLL was built.
        /// Returns <see cref="ClrVersion.Unknown"/> for native (unmanaged) DLLs or if the version cannot be determined.
        /// Only populated for <see cref="ComType.InProcess"/> drivers.
        /// </summary>
        public ClrVersion ClrVersion { get; }
    }
}

#endif
