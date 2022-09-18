namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// JSON Response returned when base64 encoding will be used to return an image array
    /// </summary>
    public class Base64ArrayHandOffResponse : ImageArrayResponseBase
    {
        /// <summary>
        /// The length of the image array's X dimension: Array[X , Y] or Array[X, Y, Z] 
        /// </summary>
        public int Dimension0Length { get; set; } = 0;

        /// <summary>
        /// The length of the image array's Y dimension: Array[X , Y] or Array[X, Y, Z] 
        /// </summary>
        public int Dimension1Length { get; set; } = 0;

        /// <summary>
        /// The length of the image array's Z dimension if present. Returns 0 for a two dimension array: Array[X , Y]. Or >=1 for a three dimension array: Array[X , Y, Z]
        /// </summary>
        public int Dimension2Length { get; set; } = 0;
    }
}
