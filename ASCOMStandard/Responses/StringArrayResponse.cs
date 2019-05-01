using System.Collections.Generic;

namespace ASCOM.Alpaca.Responses
{
    public class StringArrayResponse : Response, IValueResponse<List<string>>
    {
        public List<string> Value { get; set; }
    }
}