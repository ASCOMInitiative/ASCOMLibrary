using ASCOM.Common.DeviceInterfaces;
using System.Collections.Generic;
using ASCOM.Common;

namespace ASCOM.Com.DriverAccess
{
    public class FilterWheel : ASCOMDevice, IFilterWheelV2
    {
        public static List<ASCOMRegistration> FilterWheels => Profile.GetDrivers(DeviceTypes.FilterWheel);

        public FilterWheel(string ProgID) : base(ProgID)
        {

        }

        public new string Description
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return string.Empty;
                }
                return base.Description;
            }
        }

        public new string DriverInfo
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return string.Empty;
                }
                return base.DriverInfo;
            }
        }

        public new string DriverVersion
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return string.Empty;
                }
                return base.DriverVersion;
            }
        }

        public new string Name
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return string.Empty;
                }
                return base.Name;
            }
        }

        public int[] FocusOffsets => base.Device.FocusOffsets;

        public string[] Names => base.Device.Names;

        public short Position { get => base.Device.Position; set => base.Device.Position = value; }
    }
}
