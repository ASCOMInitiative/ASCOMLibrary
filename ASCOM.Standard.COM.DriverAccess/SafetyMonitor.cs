using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Standard.COM.DriverAccess
{
    public class SafetyMonitor : ASCOMDevice, ASCOM.Standard.Interfaces.ISafetyMonitor
    {
        public static List<ASCOMRegistration> SafetyMonitors => ProfileAccess.GetDrivers(DriverTypes.SafetyMonitor);

        public SafetyMonitor(string ProgID) : base(ProgID)
        {

        }

        public bool IsSafe => base.Device.IsSafe;
    }
}
