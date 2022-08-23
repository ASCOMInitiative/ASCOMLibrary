namespace ASCOM.Common.Alpaca
{
    // Enum used by the dynamic client to indicate what type of image array transfer should be used
    /// <summary>
    /// Type of image array transfer
    /// </summary>
    public enum ImageArrayTransferType
    {
        JSON = 0,
        Base64HandOff = 1,
        ImageBytes = 2,
        BestAvailable = 3
    }
}
