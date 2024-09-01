using ASCOM.Common.DeviceInterfaces;
using System.Collections.Generic;
using ASCOM.Common;
using ASCOM.Common.Interfaces;
using ASCOM.Common.DeviceStateClasses;

namespace ASCOM.Com.DriverAccess
{
    /// <summary>
    /// Focuser device class
    /// </summary>
    public class Focuser : ASCOMDevice, IFocuserV4
    {

        #region Convenience members

        /// <summary>
        /// Return a list of all Focusers registered in the ASCOM Profile
        /// </summary>
        public static List<ASCOMRegistration> Focusers => Profile.GetDrivers(DeviceTypes.Focuser);

        /// <summary>
        /// Focuser device state
        /// </summary>
        public FocuserState FocuserState
        {
            get
            {
                // Create a state object to return.
                FocuserState focuserState = new FocuserState(DeviceState, TL);
                TL.LogMessage(LogLevel.Debug, nameof(FocuserState), $"Returning: '{focuserState.IsMoving}' '{focuserState.Position}' '{focuserState.Temperature}' '{focuserState.TimeStamp}'");

                // Return the device specific state class
                return focuserState;
            }
        }

        #endregion

        #region Initialisers

        /// <summary>
        /// Initialise Focuser device
        /// </summary>
        /// <param name="ProgID">COM ProgID of the device.</param>
        public Focuser(string ProgID) : base(ProgID)
        {
            deviceType = DeviceTypes.Focuser;
        }

        /// <summary>
        /// Initialise Focuser device with a debug logger
        /// </summary>
        /// <param name="ProgID">ProgID of the driver</param>
        /// <param name="logger">Logger instance to receive debug information.</param>
        public Focuser(string ProgID, ILogger logger) : base(ProgID)
        {
            deviceType = DeviceTypes.Focuser;
            TL = logger;
        }

        #endregion

        #region IFocuserV3 and IFocuserV4

        /// <inheritdoc/>
        public new bool Connected
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return Device.Link;
                }
                return base.Connected;
            }
            set
            {
                //Interface Version 1 focusers do not have a Connected property and must use Link instead.
                if (InterfaceVersion == 1)
                {
                    Device.Link = value;
                }
                else
                {
                    base.Connected = value;
                }
            }
        }

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
        public bool Absolute => Device.Absolute;

        /// <inheritdoc/>
        public bool IsMoving => Device.IsMoving;

        /// <inheritdoc/>
        public int MaxIncrement => Device.MaxIncrement;

        /// <inheritdoc/>
        public int MaxStep => Device.MaxStep;

        /// <inheritdoc/>
        public int Position => Device.Position;

        /// <inheritdoc/>
        public double StepSize => Device.StepSize;

        /// <inheritdoc/>
        public bool TempComp { get => Device.TempComp; set => Device.TempComp = value; }

        /// <inheritdoc/>
        public bool TempCompAvailable => Device.TempCompAvailable;

        /// <inheritdoc/>
        public double Temperature => Device.Temperature;

        /// <inheritdoc/>
        public void Halt()
        {
            Device.Halt();
        }

        /// <inheritdoc/>
        public void Move(int Position)
        {
            Device.Move(Position);
        }

        #endregion

    }
}