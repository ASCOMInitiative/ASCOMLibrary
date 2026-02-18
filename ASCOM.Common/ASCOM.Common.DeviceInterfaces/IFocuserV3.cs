namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Provides universal access to Focuser drivers - Updated to IFocuserV3 - see remarks below
    /// </summary>
    /// <remarks>
    /// <para><b>SPECIFICATION REVISION - Platform 6.4</b></para>
    /// <para>The method signatures in the revised interface specification are identical to the preceding IFocuserV2, however, the IFocuserV3.Move command must
    /// no longer throw an InvalidOperationException exception if a Move is attempted when temperature compensation is enabled.</para>
    /// </remarks>
    public interface IFocuserV3 : IAscomDevice
    {

        /// <summary>
        /// True if the focuser is capable of absolute position; that is, being commanded to a specific step location.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/focuser.html#Focuser.Absolute">Canonical Definition</see></remarks>
        bool Absolute { get; }

        /// <summary>
        /// Immediately stop any focuser motion due to a previous <see cref="Move" /> method call.
        /// </summary>
        /// <exception cref="NotImplementedException">Focuser does not support this method.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/focuser.html#Focuser.Halt">Canonical Definition</see></remarks>
        void Halt();

        /// <summary>
        /// True if the focuser is currently moving to a new position. False if the focuser is stationary.
        /// </summary>
        /// <exception cref="NotConnectedException">If the driver is not connected.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/focuser.html#Focuser.IsMoving">Canonical Definition</see></remarks>
        bool IsMoving { get; }

        /// <summary>
        /// Maximum increment size allowed by the focuser; 
        /// i.e. the maximum number of steps allowed in one move operation.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/focuser.html#Focuser.MaxIncrement">Canonical Definition</see></remarks>
        int MaxIncrement { get; }

        /// <summary>
        /// Maximum step position permitted.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/focuser.html#Focuser.MaxStep">Canonical Definition</see></remarks>
        int MaxStep { get; }

        /// <summary>
        /// Moves the focuser by the specified amount or to the specified position depending on the value of the <see cref="Absolute" /> property.
        /// </summary>
        /// <param name="Position">Step distance or absolute position, depending on the value of the <see cref="Absolute" /> property.</param>
        /// <exception cref="InvalidValueException">If Position would result in a movement beyond <see cref="MaxStep"/> or otherwise out of range for the focuser.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/focuser.html#Focuser.Move">Canonical Definition</see></remarks>
        void Move(int Position);

        /// <summary>
        /// Current focuser position, in steps.
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not available for this device.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/focuser.html#Focuser.Position">Canonical Definition</see></remarks>
        int Position { get; }

        /// <summary>
        /// Step size (microns) for the focuser.
        /// </summary>
        /// <exception cref= "NotImplementedException">If the focuser does not intrinsically know what the step size is.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/focuser.html#Focuser.StepSize">Canonical Definition</see></remarks>
        double StepSize { get; }

        /// <summary>
        /// The state of temperature compensation mode (if available), else always False.
        /// </summary>
        /// <exception cref="NotImplementedException">If <see cref="TempCompAvailable" /> is False and an attempt is made to set <see cref="TempComp" /> to true.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/focuser.html#Focuser.TempComp">Canonical Definition</see></remarks>
        bool TempComp { get; set; }

        /// <summary>
        /// True if focuser has temperature compensation available.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/focuser.html#Focuser.TempCompAvailable">Canonical Definition</see></remarks>
        bool TempCompAvailable { get; }

        /// <summary>
        /// Current ambient temperature as measured by the focuser.
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not available for this device.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information and implementation requirements: <see href="https://ascom-standards.org/newdocs/focuser.html#Focuser.Temperature">Canonical Definition</see></remarks>
        double Temperature { get; }
    }
}