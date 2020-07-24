namespace ASCOM.Standard.Responses
{
    /// <summary>
    /// Defines an Alpaca response that returns a value.
    /// </summary>
    public interface IValueResponse<T> : IResponse
    {
        /// <summary>
        /// The value of the response
        /// </summary>
        T Value { get; set; }
    }
}