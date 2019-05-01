namespace ASCOM.Alpaca.Responses
{
    public class ImageArrayShort3DResponse : ImageArrayResponse<short[,,]>
    {
        public override short[,,] Value { get; set; }
    }
}