namespace ASCOM.Common.Alpaca
{
    // Enum used by the dynamic client to indicate what type of compression should be used in responses
    /// <summary>
    /// Compression options for ImageArray transfer,
    /// </summary>
    public enum ImageArrayCompression
    {
        None = 0,
        Deflate = 1,
        GZip = 2,
        GZipOrDeflate = 3
    }
}
