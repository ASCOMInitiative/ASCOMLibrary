namespace ASCOM.Common.DeviceInterfaces
{
    // -----------------------------------------------------------------------
    // <summary>Defines the ISwitchV2 Interface</summary>
    // -----------------------------------------------------------------------
    /// <summary>
    /// Defines the ISwitchV2 Interface
    /// </summary>
    /// <remarks>
    /// <para>The Switch interface is used to define a number of 'switch devices'. A switch device can be used to control something, such as a power switch
    /// or may be used to sense the state of something, such as a limit switch.</para>
    /// <para>This SwitchV2 interface is an extension of the original Switch interface.  The changes allow devices to have more than two states and
    /// to distinguish between devices that are writeable and those that are read only.</para>
    /// <para><b>Compatibility between Switch and SwitchV2 interfaces:</b></para>
    /// <list type="bullet"><item>Switch devices that implemented the original Switch interface and
    /// client applications that use the original interface will still work together.</item>
    /// <item>Client applications that implement the original
    /// Switch interface should still work with drivers that implement the new interface.</item>
    /// <item>Client applications that use the new features in this interface
    /// will not work with drivers that do not implement the new interface.</item>
    /// </list>
    /// <para>Each device has a CanWrite method, this is true if it can be written to or false if the device can only be read.</para>
    /// <para>The new MinSwitchValue, MaxSwitchValue and SwitchStep methods are used to define the range and values that a device can handle.
    /// This also defines the number of different values - states - that a device can have, from two for a traditional on-off switch, through
    /// those with a small number of states to those which have many states.</para>
    /// <para>The SetSwitchValue and GetSwitchValue methods are used to set and get the value of a device as a double.</para>
    /// <para>There is no fundamental difference between devices with different numbers of states.</para>
    /// <para><b>Naming Conventions</b></para>
    /// <para>Each device handled by a Switch is known as a device or switch device for general cases,
    /// a controller device if it can alter the state of the device and a sensor device if it can only be read.</para>
    /// <para>For convenience devices are referred to as boolean if the device can only have two states, and multi-state if it can have more than two values.
    /// <b>These are treated the same in the interface definition</b>.</para>
    /// </remarks>
    public interface ISwitchV2 : IAscomDevice
    {

        /// <summary>
        /// The number of switch devices managed by this driver
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <returns>The number of devices managed by this driver.</returns>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/switch.html#Switch.MaxSwitch">Canonical definition</see></remarks>
        short MaxSwitch { get; }

        /// <summary>
        /// Return the name of switch device n.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <param name="id">The device number (0 to <see cref="MaxSwitch"/> - 1)</param>
        /// <returns>The name of the device</returns>
        /// <exception cref="InvalidValueException">If id is outside the range 0 to <see cref="MaxSwitch"/> - 1</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/switch.html#Switch.GetSwitchName">Canonical definition</see></remarks>
        string GetSwitchName(short id);

        /// <summary>
        /// Set a switch device name to a specified value.
        /// </summary>
        /// <param name="id">The device number (0 to <see cref="MaxSwitch"/> - 1)</param>
        /// <param name="name">The name of the device</param>
        /// <exception cref="NotImplementedException">If the device name cannot be set in the application code.</exception>
        /// <exception cref="InvalidValueException">If id is outside the range 0 to <see cref="MaxSwitch"/> - 1</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/switch.html#Switch.SetSwitchName">Canonical definition</see></remarks>
        void SetSwitchName(short id, string name);

        /// <summary>
        /// Gets the description of the specified switch device. This is to allow a fuller description of
        /// the device to be returned, for example for a tool tip.
        /// </summary>
        /// <param name="id">The device number (0 to <see cref="MaxSwitch"/> - 1)</param>
        /// <returns>
        ///  String giving the device description.
        /// </returns>
        /// <exception cref="InvalidValueException">If id is outside the range 0 to <see cref="MaxSwitch"/> - 1</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/switch.html#Switch.GetSwitchDescription">Canonical definition</see></remarks>
        string GetSwitchDescription(short id);

        /// <summary>
        /// Reports if the specified switch device can be written to, default true.
        /// This is false if the device cannot be written to, for example a limit switch or a sensor.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <param name="id">The device number (0 to <see cref="MaxSwitch"/> - 1)</param>
        /// <returns>
        ///  <c>true</c> if the device can be written to, otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="InvalidValueException">If id is outside the range 0 to <see cref="MaxSwitch"/> - 1</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/switch.html#Switch.CanWrite">Canonical definition</see></remarks>
        bool CanWrite(short id);

        /// <summary>
        /// Return the state of switch device id as a boolean
        /// </summary>
        /// <param name="id">The device number (0 to <see cref="MaxSwitch"/> - 1)</param>
        /// <returns>True or false</returns>
        /// <exception cref="InvalidOperationException">If there is a temporary condition that prevents the device value being returned.</exception>
        /// <exception cref="InvalidValueException">If id is outside the range 0 to <see cref="MaxSwitch"/> - 1</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/switch.html#Switch.GetSwitch">Canonical definition</see></remarks>
        bool GetSwitch(short id);

        /// <summary>
        /// Sets a switch controller device to the specified state, true or false.
        /// </summary>
        /// <param name="id">The device number (0 to <see cref="MaxSwitch"/> - 1)</param>
        /// <param name="state">The required control state</param>
         /// <exception cref="NotImplementedException">If <see cref="CanWrite"/> is false.</exception>
       /// <exception cref="InvalidValueException">If id is outside the range 0 to <see cref="MaxSwitch"/> - 1</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/switch.html#Switch.SetSwitch">Canonical definition</see></remarks>
        void SetSwitch(short id, bool state);

        /// <summary>
        /// Returns the maximum value for this switch device, this must be greater than <see cref="MinSwitchValue"/>.
        /// </summary>
        /// <param name="id">The device number (0 to <see cref="MaxSwitch"/> - 1)</param>
        /// <returns>The maximum value to which this device can be set or which a read only sensor will return.</returns>
        /// <exception cref="InvalidValueException">If id is outside the range 0 to <see cref="MaxSwitch"/> - 1</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/switch.html#Switch.MaxSwitchValue">Canonical definition</see></remarks>
        double MaxSwitchValue(short id);

        /// <summary>
        /// Returns the minimum value for this switch device, this must be less than <see cref="MaxSwitchValue"/>
        /// </summary>
        /// <param name="id">The device number (0 to <see cref="MaxSwitch"/> - 1)</param>
        /// <returns>The minimum value to which this device can be set or which a read only sensor will return.</returns>
        /// <exception cref="InvalidValueException">If id is outside the range 0 to <see cref="MaxSwitch"/> - 1</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/switch.html#Switch.MinSwitchValue">Canonical definition</see></remarks>
        double MinSwitchValue(short id);

        /// <summary>
        /// Returns the step size that this device supports (the difference between successive values of the device).
        /// </summary>
        /// <param name="id">The device number (0 to <see cref="MaxSwitch"/> - 1)</param>
        /// <returns>The step size for this device.</returns>
        /// <exception cref="InvalidValueException">If id is outside the range 0 to <see cref="MaxSwitch"/> - 1</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/switch.html#Switch.SwitchStep">Canonical definition</see></remarks>
        double SwitchStep(short id);

        /// <summary>
        /// Returns the value for switch device id as a double
        /// </summary>
        /// <param name="id">The device number (0 to <see cref="MaxSwitch"/> - 1)</param>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <returns>The value for this switch, this is expected to be between <see cref="MinSwitchValue"/> and
        /// <see cref="MaxSwitchValue"/>.</returns>
        /// <exception cref="InvalidOperationException">If there is a temporary condition that prevents the device value being returned.</exception>
        /// <exception cref="InvalidValueException">If id is outside the range 0 to <see cref="MaxSwitch"/> - 1</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/switch.html#Switch.GetSwitchValue">Canonical definition</see></remarks>
        double GetSwitchValue(short id);

        /// <summary>
        /// Set the value for this device as a double.
        /// </summary>
        /// <param name="id">The device number (0 to <see cref="MaxSwitch"/> - 1)</param>
        /// <param name="value">The value to be set, between <see cref="MinSwitchValue"/> and <see cref="MaxSwitchValue"/></param>
        /// <exception cref="NotImplementedException">If <see cref="CanWrite"/> is false.</exception>
        /// <exception cref="InvalidValueException">If id is outside the range 0 to <see cref="MaxSwitch"/> - 1</exception>
        /// <exception cref="InvalidValueException">If value is outside the range <see cref="MinSwitchValue"/> to <see cref="MaxSwitchValue"/></exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/switch.html#Switch.SetSwitchValue">Canonical definition</see></remarks>
        void SetSwitchValue(short id, double value);
    }
}