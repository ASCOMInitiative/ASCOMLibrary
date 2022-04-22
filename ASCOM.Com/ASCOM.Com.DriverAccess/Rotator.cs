using ASCOM.Common.DeviceInterfaces;
using System.Collections.Generic;
using ASCOM.Common;

namespace ASCOM.Com.DriverAccess
{
    public class Rotator : ASCOMDevice, IRotatorV3
    {
        public static List<ASCOMRegistration> Rotators => Profile.GetDrivers(DeviceTypes.Rotator);

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

        public float TargetPosition => base.Device.TargetPosition;

        public float MechanicalPosition
        {
            get
            {
                AssertMethodImplemented(3, "MechanicalPosition is not implemented because the driver is IRotatorV2 or earlier.");
                return base.Device.MechanicalPosition;
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
            AssertMethodImplemented(3, "MoveMechanical is not implemented because the driver is IRotatorV2 or earlier.");
            base.Device.MoveMechanical(Position);
        }

        public void Sync(float Position)
        {
            AssertMethodImplemented(3, "Sync is not implemented because the driver is IRotatorV2 or earlier.");
            base.Device.Sync(Position);
        }
    }
}
