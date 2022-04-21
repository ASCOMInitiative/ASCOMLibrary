using ASCOM.Common.DeviceInterfaces;
using System.Collections.Generic;
using ASCOM.Common.Devices;

namespace ASCOM.Com.DriverAccess
{
    public class Focuser : ASCOMDevice, IFocuserV3
    {
        public static List<ASCOMRegistration> Focusers => Profile.GetDrivers(DeviceTypes.Focuser);

        public Focuser(string ProgID) : base(ProgID)
        {
        }

        public new bool Connected
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return base.Device.Link;
                }
                return base.Connected;
            }
            set
            {
                if (InterfaceVersion == 1)
                {
                    base.Device.Link = value;
                }
                base.Connected = value;
            }
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

        public bool Absolute => base.Device.Absolute;

        public bool IsMoving => base.Device.IsMoving;

        public int MaxIncrement => base.Device.MaxIncrement;

        public int MaxStep => base.Device.MaxStep;

        public int Position => base.Device.Position;

        public double StepSize => base.Device.StepSize;

        public bool TempComp { get => base.Device.TempComp; set => base.Device.TempComp = value; }

        public bool TempCompAvailable => base.Device.TempCompAvailable;

        public double Temperature => base.Device.Temperature;

        public void Halt()
        {
            base.Device.Halt();
        }

        public void Move(int Position)
        {
            base.Device.Move(Position);
        }
    }
}