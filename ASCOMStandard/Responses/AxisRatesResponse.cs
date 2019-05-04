using System.Collections.Generic;
using ASCOM.Alpaca.Devices.Telescope;

namespace ASCOM.Alpaca.Responses
{
    public class AxisRatesResponse : Response, IValueResponse<List<AxisRate>>
    {
        public List<AxisRate> Value { get; set; }
    }
}