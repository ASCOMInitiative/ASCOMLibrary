using ASCOM.Alpaca.Devices.Camera;

namespace ASCOM.Alpaca.Responses
{
    public class SensorTypeResponse : Response, IValueResponse<SensorType>
    {
        public SensorType Value { get; set; }
    }
}