namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// 2 dimension image array response
    /// </summary>
    public class ImageArrayInt2DResponse : Response, IImageResponse<int[,]>
    {
        /// <summary>
        /// 2D image array of int32 values
        /// </summary>
        public int[,] Value { get; set; }

        /// <summary>
        /// Image array type (int32)
        /// </summary>
        public ImageArrayType ArrayType { get; } = ImageArrayType.Int;

        /// <summary>
        /// The array's rank, will be 2 (single plane image (monochrome)).
        /// </summary>
        public int Rank { get; } = 2;
    }
}