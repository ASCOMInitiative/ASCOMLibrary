#if NET8_0_OR_GREATER

namespace ASCOM.Com
{
    /// <summary>Indicates the COM server type of the driver.</summary>
    public enum ComType
    {
        /// <summary>The COM driver type could not be determined.</summary>
        Unknown,

        /// <summary>The driver is an in-process (DLL) COM server registered under InprocServer32.</summary>
        InProcess,

        /// <summary>The driver is an out-of-process (EXE) COM server registered under LocalServer32.</summary>
        OutOfProcess
    }
}

#endif
