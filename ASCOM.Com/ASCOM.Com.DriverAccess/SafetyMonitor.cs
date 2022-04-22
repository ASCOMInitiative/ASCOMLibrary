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

        public bool IsSafe => base.Device.IsSafe;
    }
}
