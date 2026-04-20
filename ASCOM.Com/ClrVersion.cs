#if NET8_0_OR_GREATER

namespace ASCOM.Com
{
    /// <summary>
    /// Identifies the major CLR (Common Language Runtime) version for which a managed assembly was built.
    /// </summary>
    public enum ClrVersion
    {
        /// <summary>Native (unmanaged) DLL, or the CLR version could not be determined.</summary>
        Unknown = 0,

        /// <summary>CLR v1.x (.NET Framework 1.0 / 1.1).</summary>
        Clr1 = 1,

        /// <summary>CLR v2.x (.NET Framework 2.0 / 3.0 / 3.5).</summary>
        Clr2 = 2,

        /// <summary>CLR v4.x (.NET Framework 4.x and all modern .NET versions: 5, 6, 7, 8, 9, 10+).</summary>
        Clr4 = 4,
    }
}

#endif