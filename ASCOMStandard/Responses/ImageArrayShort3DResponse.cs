namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// 3 dimension image array response
    /// </summary>
    public class ImageArrayShort3DResponse : Response, IImageResponse<short[,,]>
    {
        /// <summary>
        ///  3D image array of short (int16) values
        /// </summary>
        public short[,,] Value { get; set; }

        /// <summary>
        /// Image array type (int16)
        /// </summary>
        public ImageArrayType ArrayType { get; } = ImageArrayType.Short;

        /// <summary>
        /// The array's rank, will be 3 (multi plane image (colour)).
        /// </summary>
        public int Rank { get; } = 3;
    }
}