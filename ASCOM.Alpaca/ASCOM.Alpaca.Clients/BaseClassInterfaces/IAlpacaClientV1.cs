using ASCOM.Common.DeviceInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// Interface to expose the common ASCOM methods plus the Alpaca device configuration property
    /// </summary>
    public interface IAlpacaClientV1 : IAscomDevice
    {
        /// <summary>
        /// Client configuration state
        /// </summary>
        ClientConfiguration ClientConfiguration { get; }
    }
}
