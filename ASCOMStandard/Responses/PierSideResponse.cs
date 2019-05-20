using ASCOM.Alpaca.Interfaces;

namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// Response that return the value as a <see cref="PointingState"/>
    /// </summary>
    public class PierSideResponse : Response, IValueResponse<PointingState>
    {
        /// <summary>
        /// Pier side returned by the device
        /// </summary>
        public PointingState Value { get; set; }
    }
}