using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Common.DeviceInterfaces
{
    public interface ITelescopeV4 : ITelescopeV3
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
        /// True when an asynchronous operation has completed
        /// </summary>
        bool OperationComplete { get; }

        /// <summary>
        /// Device state
        /// </summary>
        IList<IStateValue> DeviceState { get; }

    }
}
