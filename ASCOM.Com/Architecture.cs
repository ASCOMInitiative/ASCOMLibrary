namespace ASCOM.Com
{
    /// <summary>Indicates the processor architecture of a COM driver DLL.</summary>
    public enum Architecture
    {
        /// <summary>The architecture could not be determined.</summary>
        Unknown,

        /// <summary>32-bit Intel/AMD (x86). Runs only in a 32-bit process.</summary>
        X86,

        /// <summary>64-bit Intel/AMD (x64). Runs only in a 64-bit process.</summary>
        X64,

        /// <summary>Managed Any-CPU (MSIL). Runs in both 32-bit and 64-bit processes.</summary>
        Msil,

        /// <summary>32-bit ARM. Runs only in a 32-bit ARM process.</summary>
        Arm,

        /// <summary>64-bit ARM. Runs only in a 64-bit ARM64 process.</summary>
        Arm64
    }
}
