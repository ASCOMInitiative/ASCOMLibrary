namespace ASCOM.Alpaca.Responses
{
    public class IntResponse : Response, IValueResponse<int>
    {
        public int Value { get; set; }
    }
}