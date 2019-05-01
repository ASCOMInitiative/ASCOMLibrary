namespace ASCOM.Alpaca.Responses
{
    public class ImageArrayShort2DResponse : ImageArrayResponse<short[,]>
    {
        public override short[,] Value { get; set; }
    }
}