namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// 3 dimension image array response
    /// </summary>
    public class ImageArrayDouble3DResponse : Response, IImageResponse<double[,,]>
    {
        /// <summary>
        /// 3D image array of double values
        /// </summary>
        public double[,,] Value { get; set; }

        /// <summary>
        /// Image array type (double)
        /// </summary>
        public ImageArrayType ArrayType { get; } = ImageArrayType.Double;

        /// <summary>
        /// The array's rank, will be 3 (multi plane image (colour)).
        /// </summary>
        public int Rank { get; } = 3;
    }
}