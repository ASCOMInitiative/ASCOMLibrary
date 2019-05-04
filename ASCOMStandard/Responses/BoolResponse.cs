namespace ASCOM.Alpaca.Responses
{
    public class BoolResponse : Response, IValueResponse<bool>
    {
        public bool Value { get; set; }
    }
}