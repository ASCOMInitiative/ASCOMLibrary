using System.Collections.Generic;

namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// Response that return the value as a collection of strings
    /// </summary>
    public class StringArrayResponse : Response, IValueResponse<IList<string>>
    {
        /// <summary>
        /// String collection returned by the device
        /// </summary>
        public IList<string> Value { get; set; }
    }
}