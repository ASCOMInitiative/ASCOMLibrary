using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Standard.COM.DriverAccess
{
    public class FilterWheel : ASCOMDevice, ASCOM.Standard.Interfaces.IFilterWheelV2
    {
        public static List<ASCOMRegistration> FilterWheels => ProfileAccess.GetDrivers(DriverTypes.FilterWheel);

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
