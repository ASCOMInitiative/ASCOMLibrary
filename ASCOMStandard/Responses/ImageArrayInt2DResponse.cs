namespace ASCOM.Alpaca.Responses
{
    public class ImageArrayInt2DResponse : ImageArrayResponse<int[,]>
    {
        public override int[,] Value { get; set; }
    }
}