namespace ASCOM.Alpaca.Responses
{
    public interface IValueResponse<T> : IResponse
    {
        T Value { get; set; }
    }
}