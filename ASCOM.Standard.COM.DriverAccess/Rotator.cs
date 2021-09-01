using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Standard.COM.DriverAccess
{
    public class Rotator : ASCOMDevice, ASCOM.Standard.Interfaces.IRotatorV3
    {
        public static List<ASCOMRegistration> Rotators => ProfileAccess.GetDrivers(DriverTypes.Rotator);

        public Rotator(string ProgID) : base(ProgID)
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

        public bool CanReverse => base.Device.CanReverse;

        public bool IsMoving => base.Device.IsMoving;

        public float Position => base.Device.Position;

        public bool Reverse { get => base.Device.Reverse; set => base.Device.Reverse = value; }

        public float StepSize => base.Device.StepSize;

        public float TargetPosition => base.Device.StepSize;

        public float MechanicalPosition
        {
            get
            {
                if (InterfaceVersion > 2)
                {
                    return base.Device.MechanicalPosition;
                }
                return base.Device.Position;
            }
        }

        public void Halt()
        {
            base.Device.Halt();
        }

        public void Move(float Position)
        {
            base.Device.Move(Position);
        }

        public void MoveAbsolute(float Position)
        {
            base.Device.MoveAbsolute(Position);
        }

        public void MoveMechanical(float Position)
        {
            if (InterfaceVersion > 2)
            {
                base.Device.MoveMechanical(Position);
            }
            throw new MethodNotImplementedException("MoveMechanical", "MoveMechanical is not implemented because the driver is IRotatorV2 or earlier.");
        }

        public void Sync(float Position)
        {
            if (InterfaceVersion > 2)
            {
                base.Device.MoveMechanical(Position);
            }
            throw new MethodNotImplementedException("Sync", "Sync is not implemented because the driver is IRotatorV2 or earlier.");
        }
    }
}
