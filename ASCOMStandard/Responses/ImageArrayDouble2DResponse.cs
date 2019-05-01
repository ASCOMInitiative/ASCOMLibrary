namespace ASCOM.Alpaca.Responses
{
    public class ImageArrayDouble2DResponse : ImageArrayResponse<double[,]>
    {
        public override double[,] Value { get; set; }
    }
}