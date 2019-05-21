using ASCOM.Alpaca;

namespace ASCOM.Alpaca
{
    /// <summary>
    ///   Error numbers for use by Alpaca applications and drivers in the range 0x400 to 0xFFF.
    /// </summary>
    /// <remarks>
    ///   The range 0x400 to 0x4FF is reserved for ASCOM Alpaca defined exceptions and the range 0x500 to 0xFFF is available for user defined application or driver errors
    /// </remarks>
    public static class ErrorCodes
    {
        /// <summary>
        /// Convert an Alpaca error number into its equivalent ASCOM COM exception hResult number
        /// </summary>
        /// <param name="alpacaErrorCode">The Alpaca error number to convert</param>
        /// <returns>The equivalent ASCOM COM exception hResult number</returns>
        public static int ConvertToAscomComErrorCode(this int alpacaErrorCode)
        {
            return alpacaErrorCode + ASCOMErrorNumberOffset;
        }

        /// <summary>
        /// Offset value that relates the ASCOM Alpaca reserved error number range to the ASCOM COM HResult error number range
        /// </summary>
        public static readonly int ASCOMErrorNumberOffset = unchecked((int)0x80040000);
        
        /// <summary>
        /// Start of the Alpaca error code range 0x400 to 0xFFF
        /// </summary>
        public static readonly int AlpacaErrorCodeBase = 0x400;
        
        /// <summary>
        /// End of Alpaca error code range 0x400 to 0xFFF
        /// </summary>
        public static readonly int AlpacaErrorCodeMax = 0xFFF;
        
        /// <summary>
        /// Reserved error code (0x400) for property or method not implemented.
        /// </summary>
        /// <seealso cref="NotImplementedException"/>
        public static readonly int NotImplemented = 0x400;

        /// <summary>
        /// Reserved error code (0x401) for reporting an invalid value.
        /// </summary>
        /// <seealso cref="InvalidValueException"/>
        public static readonly int InvalidValue = 0x401;

        /// <summary>
        /// Reserved error code (0x402) for reporting that a value has not been set.
        /// </summary>
        /// <seealso cref="ValueNotSetException"/>
        public static readonly int ValueNotSet = 0x402;

        /// <summary>
        /// Reserved error code (0x407) used to indicate that the communications channel is not connected.
        /// </summary>
        public static readonly int NotConnected = 0x407;

        /// <summary>
        /// Reserved error code (0x408) used to indicate that the attempted operation is invalid because the mount
        /// is currently in a Parked state.
        /// </summary>
        public static readonly int InvalidWhileParked = 0x408;

        /// <summary>
        /// Reserved error code (0x409) used to indicate that the attempted operation is invalid because the mount
        /// is currently in a Slaved state.
        /// </summary>
        public static readonly int InvalidWhileSlaved = 0x409;

        /// <summary>
        /// Reserved error code (0x40A) related to settings.
        /// </summary>
        public static readonly int SettingsProviderError = 0x40A;

        /// <summary>
        /// Reserved error code (0x40B) to indicate that the requested operation can not be undertaken at this time.
        /// </summary>
        public static readonly int InvalidOperationException = 0x40B;

        /*
        /// <summary>
        /// Reserved error code (0x40C) to indicate that the requested action is not implemented in this driver.
        /// </summary>
        public static read-only int ActionNotImplementedException = 0x40C; Rationalised to a single NotImplementedException - to avoid confusion don't reuse this error number for new exceptions!
        */

        /// <summary>
        /// Reserved error code (0x40D) to indicate that the requested item is not present in the ASCOM cache.
        /// </summary>
        /// <remarks>
        /// The exception is defined in the ASCOM.Cache component rather than ASCOM.Exceptions.
        /// </remarks>
        public static readonly int NotInCacheException = 0x40D;

        /// <summary>
        /// Reserved 'catch-all' error code (0x4FF) used when nothing else was specified.
        /// </summary>
        public static readonly int UnspecifiedError = 0x4FF;

        /// <summary>
        /// The starting value (0x500) for driver/application specific error numbers.
        /// </summary>
        /// <remarks>
        /// Drivers are free to choose their own numbers starting with DriverBase, up to and including DriverMax.
        /// </remarks>
        public static readonly int DriverBase = 0x500;

        /// <summary>
        /// The maximum value (0xFFF) for driver/application specific error numbers.
        /// </summary>
        /// <remarks>
        /// Drivers are free to choose their own numbers starting with DriverBase, up to and including DriverMax.
        /// </remarks>
        public static readonly int DriverMax = 0xFFF;
    }
}