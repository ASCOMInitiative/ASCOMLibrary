namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// 3 dimension image array response
    /// </summary>
    public class ImageArrayInt3DResponse : Response, IImageResponse<int[,,]>
    {
        /// <summary>
        /// 3D image array of int32 values
        /// </summary>
        public int[,,] Value { get; set; }

        /// <summary>
        /// Image array type (int32)
        /// </summary>
        public ImageArrayType ArrayType { get; } = ImageArrayType.Int;

        /// <summary>
        /// The array's rank, will be 3 (multi plane image (colour)).
        /// </summary>
        public int Rank { get; } = 3;
    }
}