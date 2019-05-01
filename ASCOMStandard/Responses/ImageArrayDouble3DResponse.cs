namespace ASCOM.Alpaca.Responses
{
    public class ImageArrayDouble3DResponse : ImageArrayResponse<double[,,]>
    {
        public override double[,,] Value { get; set; }
    }
}