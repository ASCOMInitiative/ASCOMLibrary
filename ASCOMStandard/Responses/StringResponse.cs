namespace ASCOM.Alpaca.Responses
{
    public class StringResponse : Response, IValueResponse<string>
    {
        public string Value { get; set; }
    }
}