using ASCOM.Common.DeviceInterfaces;
using System.Collections.Generic;

namespace ASCOM.Com.DriverAccess
{
    public class SafetyMonitor : ASCOMDevice, ISafetyMonitor
    {
        public static List<ASCOMRegistration> SafetyMonitors => ProfileAccess.GetDrivers(DriverTypes.SafetyMonitor);

        public SafetyMonitor(string ProgID) : base(ProgID)
        {

        }

        public bool IsSafe => base.Device.IsSafe;
    }
}
