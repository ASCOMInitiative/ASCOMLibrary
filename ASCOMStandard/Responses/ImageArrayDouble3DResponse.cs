namespace ASCOM.Alpaca.Responses
{
    public class ImageArrayDouble3DResponse : Response, IImageResponse<double[,,]>
    {
        public double[,,] Value { get; set; }
        public ImageArrayType ArrayType { get; } = ImageArrayType.Double;
        public int Rank { get; } = 3;
    }
}