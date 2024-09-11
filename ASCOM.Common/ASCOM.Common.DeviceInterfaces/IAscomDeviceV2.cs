using System.Collections.Generic;

namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Defines additional properties and methods that are common to all ASCOM device interfaces in Platform 7 and later.
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
        /// Returns the device's operational state in a single call
        /// </summary>
        /// <remarks>
        /// <para>Returns all the device's operational state properties in a single call to reduce polling overhead for clients and devices.</para>
        /// <para>See <see href="https://ascom-standards.org/newdocs/interfaces.html">Master Help Document - Interfaces</see> for further information.</para>
        /// </remarks>
        List<StateValue> DeviceState { get; }
    }
}