using System.Collections.Generic;
using ASCOM.Alpaca.Devices.Telescope;

namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// Response that return the value as a collection of <see cref="AxisRate"/>
    /// </summary>
    public class AxisRatesResponse : Response, IValueResponse<List<AxisRate>>
    {
        /// <summary>
        /// Axis rate collection returned by the device
        /// </summary>
        public List<AxisRate> Value { get; set; }
    }
}