using System.Collections.Generic;
using ASCOM.Alpaca.Devices.Telescope;

namespace ASCOM.Alpaca.Responses
{
    public class DriveRatesResponse : Response, IValueResponse<List<DriveRate>>
    {
        public List<DriveRate> Value { get; set; }
    }
}