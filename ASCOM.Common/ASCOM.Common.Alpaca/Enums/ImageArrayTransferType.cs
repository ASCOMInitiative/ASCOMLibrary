namespace ASCOM.Common.Alpaca
{
    // Enum used by the dynamic client to indicate what type of image array transfer should be used
    public enum ImageArrayTransferType
    {
        JSON = 0,
        Base64HandOff = 1,
        GetBase64Image = 2,
        GetImageBytes = 3,
        BestAvailable = 4
    }
}
