using ASCOM.Common.DeviceInterfaces;
using System.Collections.Generic;
using ASCOM.Common;
using ASCOM.Common.Interfaces;
using ASCOM.Common.DeviceStateClasses;

namespace ASCOM.Com.DriverAccess
{
    /// <summary>
    /// Rotator device class
    /// </summary>
#if NET8_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    public class Rotator : ASCOMDevice, IRotatorV4
    {

        #region Convenience members

        /// <summary>
        /// Return a list of all Rotators registered in the ASCOM Profile
        /// </summary>
        public static List<ASCOMRegistration> Rotators => Profile.GetDrivers(DeviceTypes.Rotator);

        /// <summary>
        /// Rotator device state
        /// </summary>
        public RotatorState RotatorState
        {
            get
            {
                // Create a state object to return.
                RotatorState rotatorState = new RotatorState(DeviceState, TL);
                TL.LogMessage(LogLevel.Debug,nameof(RotatorState), $"Returning: " +
                    $"Cloud cover: '{rotatorState.IsMoving}', " +
                    $"Dew point: '{rotatorState.MechanicalPosition}', " +
                    $"Humidity: '{rotatorState.Position}', " +
                    $"Time stamp: '{rotatorState.TimeStamp}'");

                // Return the device specific state class
                return rotatorState;
            }
        }

        #endregion

        #region Initialisers

        /// <summary>
        /// Initialise Rotator device
        /// </summary>
        /// <param name="ProgID">COM ProgID of the device.</param>
        public Rotator(string ProgID) : base(ProgID)
        {
            deviceType = DeviceTypes.Rotator;
        }

        /// <summary>
        /// Initialise Rotator device with a debug logger
        /// </summary>
        /// <param name="ProgID">ProgID of the driver</param>
        /// <param name="logger">Logger instance to receive debug information.</param>
        public Rotator(string ProgID, ILogger logger) : base(ProgID)
        {
            deviceType = DeviceTypes.Rotator;
            TL = logger;
        }
        #endregion

        #region IRotatorV3 and IRotatorV4

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public bool CanReverse => Device.CanReverse;

        /// <inheritdoc/>
        public bool IsMoving => Device.IsMoving;

        /// <inheritdoc/>
        public float Position => Device.Position;

        /// <inheritdoc/>
        public bool Reverse { get => Device.Reverse; set => Device.Reverse = value; }

        /// <inheritdoc/>
        public float StepSize => Device.StepSize;

        /// <inheritdoc/>
        public float TargetPosition => Device.TargetPosition;

        /// <inheritdoc/>
        public float MechanicalPosition
        {
            get
            {
                AssertMethodImplemented(3, "MechanicalPosition is not implemented because the driver is IRotatorV2 or earlier.");
                return Device.MechanicalPosition;
            }
        }

        /// <inheritdoc/>
        public void Halt()
        {
            Device.Halt();
        }

        /// <inheritdoc/>
        public void Move(float Position)
        {
            Device.Move(Position);
        }

        /// <inheritdoc/>
        public void MoveAbsolute(float Position)
        {
            Device.MoveAbsolute(Position);
        }

        /// <inheritdoc/>
        public void MoveMechanical(float Position)
        {
            AssertMethodImplemented(3, "MoveMechanical is not implemented because the driver is IRotatorV2 or earlier.");
            Device.MoveMechanical(Position);
        }

        /// <inheritdoc/>
        public void Sync(float Position)
        {
            AssertMethodImplemented(3, "Sync is not implemented because the driver is IRotatorV2 or earlier.");
            Device.Sync(Position);
        }

        #endregion

    }
}
