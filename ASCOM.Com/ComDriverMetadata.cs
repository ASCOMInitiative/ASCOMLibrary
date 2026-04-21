#if NET8_0_OR_GREATER
#nullable enable

using System.Runtime.Versioning;

namespace ASCOM.Com
{
    /// <summary>
    /// Immutable snapshot of the properties discovered for a COM driver.
    /// </summary>
    /// <remarks>
    /// <para>This class supports the <c>PlatformUtilities.GetComDriverMetadata</c> method, which is only available in projects targeting .NET 8 and later.</para>
    /// </remarks>
    [SupportedOSPlatform("windows")]
    public class ComDriverMetadata
    {
        /// <summary>
        /// Initialises a new instance of <see cref="ComDriverMetadata"/> with all property values.
        /// </summary>
        internal ComDriverMetadata(string progId, bool isRegistered, ComType comType, string? dllPath, string? dllVersion, Architecture dllArchitecture, bool is32BitCompatible, bool is64BitCompatible, ClrVersion clrVersion)
        {
            ProgId = progId;
            IsRegistered = isRegistered;
            ComType = comType;

            if (dllPath is null)
                dllPath = "";

            if (dllVersion is null)
                dllVersion = "";

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
        /// </summary>
        /// <remarks>
        /// Only populated for <see cref="ComType.InProcess"/> drivers. Returns an empty string for out-of-process drivers or if the path cannot be determined.
        /// </remarks>
        public string? DllPath { get; }

        /// <summary>
        /// File version of the COM driver DLL.
        /// </summary>
        /// <remarks>
        /// Only populated for <see cref="ComType.InProcess"/> drivers. Returns an empty string for out-of-process drivers or if the version cannot be determined.
        /// </remarks>
        public string? DllVersion { get; }

        /// <summary>
        /// Processor architecture of the COM driver DLL.
        /// </summary>
        /// <remarks>
        /// Only populated for <see cref="ComType.InProcess"/> drivers. Returns <see cref="Architecture.Unknown"/> for out-of-process drivers or if the architecture cannot be determined.
        /// </remarks>
        public Architecture DllArchitecture { get; }

        /// <summary>
        /// True if the COM Object can be used by a 32-bit application.
        /// </summary>
        public bool Is32BitCompatible { get; }

        /// <summary>
        /// True if the COM Object can be used by a 64-bit application.
        /// </summary>
        public bool Is64BitCompatible { get; }

        /// <summary>
        /// The CLR version for which the DLL was built.
        /// </summary>
        /// <remarks>
        /// Returns <see cref="ClrVersion.Unknown"/> for out-of-process drivers or if the CLR version cannot be determined.
        /// </remarks>
        public ClrVersion ClrVersion { get; }
    }
}

#endif
