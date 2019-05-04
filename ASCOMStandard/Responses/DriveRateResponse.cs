using ASCOM.Alpaca.Devices.Telescope;

namespace ASCOM.Alpaca.Responses
{
    public class DriveRateResponse : Response, IValueResponse<DriveRate>
    {
        public DriveRate Value { get; set; }
    }
}