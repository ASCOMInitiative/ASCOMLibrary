namespace ASCOM.Alpaca.Responses
{
    public class ImageArrayShort2DResponse : Response, IImageResponse<short[,]>
    {
        public short[,] Value { get; set; }
        public ImageArrayType ArrayType { get; } = ImageArrayType.Short;
        public int Rank { get; } = 2;
    }
}