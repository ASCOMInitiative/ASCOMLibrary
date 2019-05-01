namespace ASCOM.Alpaca.Responses
{
    public class ImageArrayInt3DResponse : ImageArrayResponse<int[,,]>
    {
        public override int[,,] Value { get; set; }
    }
}