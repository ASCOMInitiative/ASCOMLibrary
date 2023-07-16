using System;
using System.Collections.Generic;

namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Defines additional properties and methods that are common to all ASCOM devices.
    /// </summary>
    public interface IAscomDeviceV2 : IAscomDevice
    {
        /// <summary>
        /// Connect to device asynchronously
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnect from device asynchronously
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Completion variable for asynchronous connect and disconnect operations
        /// </summary>
        bool Connecting { get; }

        /// <summary>
        /// Device state
        /// </summary>
        IList<IStateValue> DeviceState { get; }
    }
}