using ASCOM.Common.DeviceInterfaces;
using System.Collections.Generic;
using ASCOM.Common;

namespace ASCOM.Com.DriverAccess
{
    public class SafetyMonitor : ASCOMDevice, ISafetyMonitor
    {
        public static List<ASCOMRegistration> SafetyMonitors => Profile.GetDrivers(DeviceTypes.SafetyMonitor);

        public SafetyMonitor(string ProgID) : base(ProgID)
        {

        }

        /// <summary>
        /// Indicates whether the monitored state is safe for use.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <value>True if the state is safe, False if it is unsafe.</value>
        /// <remarks>
        /// <p style="color:red;margin-bottom:0"><b>Must be implemented and must not throw a NotImplementedException. </b></p>
        /// </remarks>
        public bool IsSafe => base.Device.IsSafe;
    }
}
