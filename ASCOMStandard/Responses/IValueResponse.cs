namespace ASCOM.Alpaca.Responses
{
    public interface IValueResponse<out T> : IResponse
    {
        T Value { get; }
    }
}