namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// Defines an Alpaca response that returns a value.
    /// </summary>
    /// <typeparam name="T">Type of the response</typeparam>
    public interface IValueResponse<T> : IResponse
    {
        /// <summary>
        /// The value of the response
        /// </summary>
        T Value { get; set; }
    }
}