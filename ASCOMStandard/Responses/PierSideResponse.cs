using ASCOM.Alpaca.Devices.Telescope;

namespace ASCOM.Alpaca.Responses
{
    public class PierSideResponse : Response, IValueResponse<PierSide>
    {
        public PierSide Value { get; set; }
    }
}