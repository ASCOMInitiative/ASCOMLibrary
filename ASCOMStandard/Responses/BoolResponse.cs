namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// Response that return the value as a boolean
    /// </summary>
    public class BoolResponse : Response, IValueResponse<bool>
    {
        /// <summary>
        /// boolean value returned by the device
        /// </summary>
        public bool Value { get; set; }
    }
}