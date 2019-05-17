using System.Collections.Generic;

namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// Response that return the value as a collection of integer
    /// </summary>
    public class IntListResponse : Response, IValueResponse<IList<int>>
    {
        /// <summary>
        /// Integer collection returned by the device
        /// </summary>
        public IList<int> Value { get; set; }
    }
}