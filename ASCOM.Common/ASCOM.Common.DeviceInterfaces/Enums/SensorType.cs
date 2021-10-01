namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Sensor type, identifies the type of colour sensor
    /// </summary>
    public enum SensorType
    {
        ///<summary>
        ///Camera produces monochrome array with no Bayer encoding
        ///</summary>
        Monochrome = 0,
        ///<summary>
        ///Camera produces color image directly, requiring not Bayer decoding
        ///</summary>
        Color = 1,
        ///<summary>
        ///Camera produces RGGB encoded Bayer array images
        ///</summary>
        RGGB = 2,
        ///<summary>
        ///Camera produces CMYG encoded Bayer array images
        ///</summary>
        CMYG = 3,
        ///<summary>
        ///Camera produces CMYG2 encoded Bayer array images
        ///</summary>
        CMYG2 = 4,
        ///<summary>
        ///Camera produces Kodak TRUESENSE Bayer LRGB array images
        ///</summary>
        LRGB = 5,
    }
}