namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// 2 dimension image array response
    /// </summary>
    public class ImageArrayDouble2DResponse : Response, IImageResponse<double[,]>
    {
        /// <summary>
        /// 2D image array of double values
        /// </summary>
        public double[,] Value { get; set; }

        /// <summary>
        /// Image array type (double)
        /// </summary>
        public ImageArrayType ArrayType { get; } = ImageArrayType.Double;

        /// <summary>
        /// The array's rank, will be 2 (single plane image (monochrome)).
        /// </summary>
        public int Rank { get; } = 2;
    }
}