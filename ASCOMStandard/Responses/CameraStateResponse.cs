using ASCOM.Alpaca.Devices.Camera;

namespace ASCOM.Alpaca.Responses
{
    public class CameraStateResponse : Response, IValueResponse<CameraState>
    {
        public CameraState Value { get; set; }
    }
}