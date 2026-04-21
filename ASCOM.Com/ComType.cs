namespace ASCOM.Com
{
    /// <summary>Indicates the COM server type of the driver.</summary>
    /// <remarks>
    /// <para>This enum supports the <c>PlatformUtilities.GetComDriverMetadata</c> method, which is only available in projects targeting .NET 8 and later.</para>
    /// </remarks>
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
