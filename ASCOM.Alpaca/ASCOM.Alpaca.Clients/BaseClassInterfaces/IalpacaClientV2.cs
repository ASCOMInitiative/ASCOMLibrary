using ASCOM.Alpaca.Clients;
using ASCOM.Common.DeviceInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// Interface to expose the common Platform 7 ASCOM methods plus the Alpaca device configuration property
    /// </summary>
    public interface IAlpacaClientV2 : IAscomDeviceV2
    {
        /// <summary>
        /// Client configuration state
        /// </summary>
        ClientConfiguration ClientConfiguration { get; }

        /// <summary>
        /// Updates the internal HTTP client with a new instance.
        /// </summary>
        /// <remarks>This method must be called after changing the client configuration through the <see cref="ClientConfiguration"/> property.</remarks>
        void Refresh();
    }
}
