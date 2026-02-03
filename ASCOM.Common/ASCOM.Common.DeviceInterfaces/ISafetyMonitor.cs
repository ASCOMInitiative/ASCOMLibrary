// -----------------------------------------------------------------------
// <summary>Defines the ISafetyMonitor Interface</summary>
// -----------------------------------------------------------------------
namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Defines the ISafetyMonitor Interface
    /// </summary>
    public interface ISafetyMonitor : IAscomDevice
    {

        /// <summary>
        /// Indicates whether the monitored state is safe for use.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <value>True if the state is safe, False if it is unsafe.</value>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/safetymonitor.html#SafetyMonitor.IsSafe">Canonical definition</see></remarks>
        bool IsSafe { get; }
    }
}