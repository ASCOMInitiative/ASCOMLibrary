namespace ASCOM.Standard
{
    /// <summary>
    /// Error number limits related to Windows COM exceptions (HResult values)
    /// </summary>
    public enum ComErrorCodes
    {
        /// <summary>
        /// Offset value that relates the ASCOM Alpaca reserved error number range to the ASCOM COM HResult error number range
        /// </summary>
        ComErrorNumberOffset = unchecked((int)0x80040000), // Offset value that relates the ASCOM Alpaca reserved error number range to the ASCOM COM HResult error number range
        ComErrorNumberBase = unchecked((int)0x80040400), // Lowest ASCOM error number
        ComErrorNumberMax = unchecked((int)0x80040FFF) // Highest ASCOM error number

    }
}
