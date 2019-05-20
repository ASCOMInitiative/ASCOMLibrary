using System.Collections.Generic;
using ASCOM.Alpaca.Interfaces;

namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// Response that return the value as a collection of <see cref="DriveRate"/>
    /// </summary>
    public class DriveRatesResponse : Response, IValueResponse<IList<DriveRate>>
    {
        /// <summary>
        /// Drive rate collection returned by the device
        /// </summary>
        public IList<DriveRate> Value { get; set; }
    }
}