using ASCOM.Alpaca.Devices.Telescope;

namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// Response that return the value as a <see cref="PierSide"/>
    /// </summary>
    public class PierSideResponse : Response, IValueResponse<PierSide>
    {
        /// <summary>
        /// Pier side returned by the device
        /// </summary>
        public PierSide Value { get; set; }
    }
}