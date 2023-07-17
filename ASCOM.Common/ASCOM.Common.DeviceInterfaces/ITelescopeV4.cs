using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Common.DeviceInterfaces
{
    public interface ITelescopeV4 : IAscomDeviceV2, ITelescopeV3
    {
        /// <summary>
        /// True when an asynchronous operation has completed
        /// </summary>
        bool OperationComplete { get; }

        /// <summary>
        /// True when an asynchronous operation has been interrupted
        /// </summary>
        bool InterruptionComplete { get; }
    }
}
