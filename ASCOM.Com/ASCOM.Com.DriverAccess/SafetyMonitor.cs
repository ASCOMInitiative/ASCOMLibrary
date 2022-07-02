using ASCOM.Common.DeviceInterfaces;
using System.Collections.Generic;
using ASCOM.Common;

namespace ASCOM.Com.DriverAccess
{
    /// <summary>
    /// SafetyMonitor device class
    /// </summary>
    public class SafetyMonitor : ASCOMDevice, ISafetyMonitor
    {
        /// <summary>
        /// Return a list of all SafetyMonitors registered in the ASCOM Profile
        /// </summary>
        public static List<ASCOMRegistration> SafetyMonitors => Profile.GetDrivers(DeviceTypes.SafetyMonitor);

        /// <summary>
        /// Initialise SafetyMonitor device
        /// </summary>
        /// <param name="ProgID">COM ProgID of the device.</param>
        public SafetyMonitor(string ProgID) : base(ProgID)
        {

        }

        /// <summary>
        /// Indicates whether the monitored state is safe for use.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <value>True if the state is safe, False if it is unsafe.</value>
        /// <remarks>
        /// <p style="color:red;margin-bottom:0"><b>Must be implemented and must not throw a NotImplementedException. </b></p>
        /// </remarks>
        public bool IsSafe => base.Device.IsSafe;
    }
}
