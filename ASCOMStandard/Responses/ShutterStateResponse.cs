using ASCOM.Alpaca.Devices.Dome;

namespace ASCOM.Alpaca.Responses
{
    public class ShutterStateResponse : Response, IValueResponse<ShutterState>
    {
        public ShutterState Value { get; set; }
    }
}

