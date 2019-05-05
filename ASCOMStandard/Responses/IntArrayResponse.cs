using System.Collections.Generic;

namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// Response that return the value as a collection of integer
    /// </summary>
    public class IntArrayResponse : Response, IValueResponse<List<int>>
    {
        /// <summary>
        /// Integer collection returned by the device
        /// </summary>
        public List<int> Value { get; set; }
    }
}