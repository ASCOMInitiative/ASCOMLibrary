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
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.Connect">Canonical Definition</see>
        /// <para><b>Please Note</b>: The Camera definition in the link applies to all device types.</para></remarks>
        void Connect();

        /// <summary>
        /// Disconnect from device asynchronously
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.Disconnect">Canonical Definition</see>
        /// <para><b>Please Note</b>: The Camera definition in the link applies to all device types.</para></remarks>
        void Disconnect();

        /// <summary>
        /// Completion variable for asynchronous connect and disconnect operations
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.Connecting">Canonical Definition</see>
        /// <para><b>Please Note</b>: The Camera definition in the link applies to all device types.</para></remarks>
        bool Connecting { get; }

        /// <summary>
        /// Returns the device's operational state in a single call
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.DeviceState">Canonical Definition</see>
        /// <para><b>Please Note</b>: The Camera definition in the link applies to all device types.</para></remarks>
        List<StateValue> DeviceState { get; }
    }
}