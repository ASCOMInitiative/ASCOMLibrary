namespace ASCOM.Alpaca.Responses
{
    public class ImageArrayInt3DResponse : Response, IImageResponse<int[,,]>
    {
        public int[,,] Value { get; set; }
        public ImageArrayType ArrayType { get; } = ImageArrayType.Int;
        public int Rank { get; } = 3;
    }
}