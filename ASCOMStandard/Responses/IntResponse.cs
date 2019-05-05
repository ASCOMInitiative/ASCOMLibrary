namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// Response that return the value as an integer
    /// </summary>
    public class IntResponse : Response, IValueResponse<int>
    {
        /// <summary>
        /// Integer value returned by the device
        /// </summary>
        public int Value { get; set; }
    }
}