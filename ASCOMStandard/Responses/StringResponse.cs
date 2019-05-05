namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// Response that return the value as a string
    /// </summary>
    public class StringResponse : Response, IValueResponse<string>
    {
        /// <summary>
        /// String value returned by the device
        /// </summary>
        public string Value { get; set; }
    }
}