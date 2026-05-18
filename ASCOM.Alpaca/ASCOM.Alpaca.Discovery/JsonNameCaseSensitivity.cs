namespace ASCOM.Alpaca.Discovery
{
    /// <summary>
    /// Select whether to accept correct or any JSON element name casing.
    /// </summary>
    public enum JsonNameCaseSensitivity
    {
        /// <summary>
        /// Only accept correctly cased JSON element names.
        /// </summary>
        StrictCasing,

        /// <summary>
        /// Accept JSON element names regardless of casing.
        /// </summary>
        AnyCasing
    }
}
