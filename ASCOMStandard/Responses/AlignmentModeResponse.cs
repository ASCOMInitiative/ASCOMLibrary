using ASCOM.Alpaca.Devices.Telescope;

namespace ASCOM.Alpaca.Responses
{
    public class AlignmentModeResponse : Response, IValueResponse<AlignmentMode>
    {
        public AlignmentMode Value { get; set; }
    }
}