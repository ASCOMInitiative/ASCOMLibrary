using ASCOM.Alpaca.Devices.Telescope;

namespace ASCOM.Alpaca.Responses
{
    public class EquatorialCoordinateTypeResponse : Response, IValueResponse<EquatorialCoordinateType>
    {
        public EquatorialCoordinateType Value { get; set; }
    }
}