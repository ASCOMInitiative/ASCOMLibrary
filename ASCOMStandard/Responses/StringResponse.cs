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

        /// <summary>
        /// Return the value as a string
        /// </summary>
        /// <returns>String representation of the response value</returns>
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}