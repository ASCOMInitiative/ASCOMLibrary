namespace ASCOM.Alpaca.Responses
{
    public class ValueResponse<T> : Response, IValueResponse<T>
    {
        public T Value { get; set; }
    }
}