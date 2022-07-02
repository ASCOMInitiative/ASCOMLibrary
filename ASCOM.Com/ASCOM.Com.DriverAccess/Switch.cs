using ASCOM.Common.DeviceInterfaces;
using System.Collections.Generic;
using ASCOM.Common;

namespace ASCOM.Com.DriverAccess
{
    /// <summary>
    /// Switch device class
    /// </summary>
    public class Switch : ASCOMDevice, ISwitchV2
    {
        /// <summary>
        /// Return a list of all Switches registered in the ASCOM Profile
        /// </summary>
        public static List<ASCOMRegistration> Switches => Profile.GetDrivers(DeviceTypes.Switch);

        /// <summary>
        /// Initialise Switch device
        /// </summary>
        /// <param name="ProgID">COM ProgID of the device.</param>
        public Switch(string ProgID) : base(ProgID)
        {
        }

        /// <summary>
        /// The number of switch devices managed by this driver
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <returns>The number of devices managed by this driver.</returns>
        /// <remarks><p style="color:red"><b>Must be implemented, must not throw a <see cref="NotImplementedException"/></b></p> 
        /// <p>Devices are numbered from 0 to <see cref="MaxSwitch"/> - 1</p></remarks>
        public short MaxSwitch => base.Device.MaxSwitch;

        /// <summary>
        /// Return the name of switch device n.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <param name="id">The device number (0 to <see cref="MaxSwitch"/> - 1)</param>
        /// <returns>The name of the device</returns>
        /// <exception cref="InvalidValueException">If id is outside the range 0 to <see cref="MaxSwitch"/> - 1</exception>
        /// <remarks><p style="color:red"><b>Must be implemented, must not throw an ASCOM.NotImplementedException</b></p>
        /// <para>Devices are numbered from 0 to <see cref="MaxSwitch"/> - 1</para></remarks>
        public string GetSwitchName(short id)
        {
            return base.Device.GetSwitchName(id);
        }

        /// <summary>
        /// Set a switch device name to a specified value.
        /// </summary>
        /// <param name="id">The device number (0 to <see cref="MaxSwitch"/> - 1)</param>
        /// <param name="name">The name of the device</param>
        /// <exception cref="NotImplementedException">If the device name cannot be set in the application code.</exception>
        /// <exception cref="InvalidValueException">If id is outside the range 0 to <see cref="MaxSwitch"/> - 1</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Can throw a <see cref="NotImplementedException"/> if the device name can not be set by the application.</b></p>
        /// <para>Devices are numbered from 0 to <see cref="MaxSwitch"/> - 1</para>
        /// </remarks>
        public void SetSwitchName(short id, string name)
        {
            base.Device.SetSwitchName(id, name);
        }

        /// <summary>
        /// Gets the description of the specified switch device. This is to allow a fuller description of
        /// the device to be returned, for example for a tool tip.
        /// </summary>
        /// <param name="id">The device number (0 to <see cref="MaxSwitch"/> - 1)</param>
        /// <returns>
        ///   String giving the device description.
        /// </returns>
        /// <exception cref="InvalidValueException">If id is outside the range 0 to <see cref="MaxSwitch"/> - 1</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Must be implemented, must not throw an ASCOM.NotImplementedException</b></p>
        /// <para>Devices are numbered from 0 to <see cref="MaxSwitch"/> - 1</para>
        /// <para>This is a Version 2 method.</para>
        /// </remarks>
        public string GetSwitchDescription(short id)
        {
            return base.Device.GetSwitchDescription(id);
        }

        /// <summary>
        /// Reports if the specified switch device can be written to, default true.
        /// This is false if the device cannot be written to, for example a limit switch or a sensor.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <param name="id">The device number (0 to <see cref="MaxSwitch"/> - 1)</param>
        /// <returns>
        ///   <c>true</c> if the device can be written to, otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="InvalidValueException">If id is outside the range 0 to <see cref="MaxSwitch"/> - 1</exception>
        /// <remarks><p style="color:red"><b>Must be implemented, must not throw an ASCOM.NotImplementedException</b></p>
        /// <para>Devices are numbered from 0 to <see cref="MaxSwitch"/> - 1</para>
        /// <para>This is a Version 2 method, version 1 switch devices can be assumed to be writable.</para>
        /// </remarks>
        public bool CanWrite(short id)
        {
            return base.Device.CanWrite(id);
        }

        /// <summary>
        /// Return the state of switch device id as a boolean
        /// </summary>
        /// <param name="id">The device number (0 to <see cref="MaxSwitch"/> - 1)</param>
        /// <returns>True or false</returns>
        /// <exception cref="InvalidOperationException">If there is a temporary condition that prevents the device value being returned.</exception>
        /// <exception cref="InvalidValueException">If id is outside the range 0 to <see cref="MaxSwitch"/> - 1</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Must be implemented, must not throw a <see cref="NotImplementedException"/>.</b></p> 
        /// <para>All devices must implement this. A multi-state device will return true if the device is at the maximum value, false if the value is at the minumum
        /// and either true or false as specified by the driver developer for intermediate values.</para>
        /// <para>Some devices do not support reading their state although they do allow state to be set. In these cases, on startup, the driver can not know the hardware state and it is recommended that the 
        /// driver either:</para>
        /// <list type="bullet">
        /// <item><description>Sets the device to a known state on connection</description></item>
        /// <item><description>Throws an <see cref="InvalidOperationException"/> until the client software has set the device state for the first time</description></item>
        /// </list>
        /// <para>In both cases the driver should save a local copy of the state which it last set and return this through <see cref="GetSwitch" /> and <see cref="GetSwitchValue" /></para>
        /// <para>Devices are numbered from 0 to <see cref="MaxSwitch"/> - 1</para></remarks>
        public bool GetSwitch(short id)
        {
            return base.Device.GetSwitch(id);
        }

        /// <summary>
        /// Sets a switch controller device to the specified state, true or false.
        /// </summary>
        /// <param name="id">The device number (0 to <see cref="MaxSwitch"/> - 1)</param>
        /// <param name="state">The required control state</param>
        /// <exception cref="NotImplementedException">If <see cref="CanWrite"/> is false.</exception>
        /// <exception cref="InvalidValueException">If id is outside the range 0 to <see cref="MaxSwitch"/> - 1</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Can throw a <see cref="NotImplementedException"/> if <see cref="CanWrite"/> is False.</b></p>
        /// <para><see cref="GetSwitchValue"/> must return <see cref="MaxSwitchValue" /> if the set state is true and <see cref="MinSwitchValue" /> if the set state is false.</para>
        /// <para>Devices are numbered from 0 to <see cref="MaxSwitch"/> - 1</para></remarks>
        public void SetSwitch(short id, bool state)
        {
            base.Device.SetSwitch(id, state);
        }

        /// <summary>
        /// Returns the maximum value for this switch device, this must be greater than <see cref="MinSwitchValue"/>.
        /// </summary>
        /// <param name="id">The device number (0 to <see cref="MaxSwitch"/> - 1)</param>
        /// <returns>The maximum value to which this device can be set or which a read only sensor will return.</returns>
        /// <exception cref="InvalidValueException">If id is outside the range 0 to <see cref="MaxSwitch"/> - 1</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Must be implemented, must not throw a <see cref="NotImplementedException"/>.</b></p> 
        /// <para>If a two state device cannot report its state,  <see cref="MaxSwitchValue"/> should return the value 1.0.</para>
        /// <para> Devices are numbered from 0 to <see cref="MaxSwitch"/> - 1.</para>
        /// <para>This is a Version 2 method.</para>
        /// </remarks>
        public double MaxSwitchValue(short id)
        {
            try
            {
                return base.Device.MaxSwitchValue(id);
            }
            catch (ASCOM.NotImplementedException)
            {
                return 1.0;
            }
        }

        /// <summary>
        /// Returns the minimum value for this switch device, this must be less than <see cref="MaxSwitchValue"/>
        /// </summary>
        /// <param name="id">The device number (0 to <see cref="MaxSwitch"/> - 1)</param>
        /// <returns>The minimum value to which this device can be set or which a read only sensor will return.</returns>
        /// <exception cref="InvalidValueException">If id is outside the range 0 to <see cref="MaxSwitch"/> - 1</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Must be implemented, must not throw a <see cref="NotImplementedException"/>.</b></p> 
        /// <para>If a two state device cannot report its state, <see cref="MinSwitchValue"/> should return the value 0.0.</para>
        /// <para> Devices are numbered from 0 to <see cref="MaxSwitch"/> - 1.</para>
        /// <para>This is a Version 2 method.</para>
        /// </remarks>
        public double MinSwitchValue(short id)
        {
            try
            {
                return base.Device.MinSwitchValue(id);
            }
            catch (ASCOM.NotImplementedException)
            {
                return 0.0;
            }
        }

        /// <summary>
        /// Returns the step size that this device supports (the difference between successive values of the device).
        /// </summary>
        /// <param name="id">The device number (0 to <see cref="MaxSwitch"/> - 1)</param>
        /// <returns>The step size for this device.</returns>
        /// <exception cref="InvalidValueException">If id is outside the range 0 to <see cref="MaxSwitch"/> - 1</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Must be implemented, must not throw <see cref="NotImplementedException"/>.</b></p>
        /// <para>SwitchStep, MinSwitchValue and MaxSwitchValue can be used to determine the way the device is controlled and/or displayed,
        /// for example by setting the number of decimal places or number of states for a display.</para>
        /// <para><see cref="SwitchStep"/> must be greater than zero and the number of steps can be calculated as:
        /// ((<see cref="MaxSwitchValue"/> - <see cref="MinSwitchValue"/>) / <see cref="SwitchStep"/>) + 1.</para>
        /// <para>The switch range (<see cref="MaxSwitchValue"/> - <see cref="MinSwitchValue"/>) must be an exact multiple of <see cref="SwitchStep"/>.</para>
        /// <para>If a two state device cannot report its state, <see cref="SwitchStep"/> should return the value 1.0.</para>
        /// <para>Devices are numbered from 0 to <see cref="MaxSwitch"/> - 1.</para>
        /// <para>This is a Version 2 method.</para>
        /// </remarks>
        public double SwitchStep(short id)
        {
            try
            {
                return base.Device.SwitchStep(id);
            }
            catch (ASCOM.NotImplementedException)
            {
                return 1.0;
            }
        }

        /// <summary>
        /// Returns the value for switch device id as a double
        /// </summary>
        /// <param name="id">The device number (0 to <see cref="MaxSwitch"/> - 1)</param>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <returns>The value for this switch, this is expected to be between <see cref="MinSwitchValue"/> and
        /// <see cref="MaxSwitchValue"/>.</returns>
        /// <exception cref="InvalidOperationException">If there is a temporary condition that prevents the device value being returned.</exception>
        /// <exception cref="InvalidValueException">If id is outside the range 0 to <see cref="MaxSwitch"/> - 1</exception>
        /// <remarks><p style="color:red"><b>Must be implemented, must not throw a <see cref="NotImplementedException"/>.</b></p> 
        /// <para>Some devices do not support reading their state although they do allow state to be set. In these cases, on startup, the driver can not know the hardware state and it is recommended that the 
        /// driver either:</para>
        /// <list type="bullet">
        /// <item><description>Sets the device to a known state on connection</description></item>
        /// <item><description>Throws an <see cref="InvalidOperationException"/> until the client software has set the device state for the first time</description></item>
        /// </list>
        /// <para>In both cases the driver should save a local copy of the state which it last set and return this through <see cref="GetSwitch" /> and <see cref="GetSwitchValue" /></para>
        /// <para>Devices are numbered from 0 to <see cref="MaxSwitch"/> - 1.</para>
        /// <para>This is a Version 2 method.</para>
        /// </remarks>
        public double GetSwitchValue(short id)
        {
            try
            {
                return base.Device.GetSwitchValue(id);
            }
            catch (ASCOM.NotImplementedException)
            {
                return this.GetSwitch(id) ? 1.0 : 0.0;
            }
        }

        /// <summary>
        /// Set the value for this device as a double.
        /// </summary>
        /// <param name="id">The device number (0 to <see cref="MaxSwitch"/> - 1)</param>
        /// <param name="value">The value to be set, between <see cref="MinSwitchValue"/> and <see cref="MaxSwitchValue"/></param>
        /// <exception cref="NotImplementedException">If <see cref="CanWrite"/> is false.</exception>
        /// <exception cref="InvalidValueException">If id is outside the range 0 to <see cref="MaxSwitch"/> - 1</exception>
        /// <exception cref="InvalidValueException">If value is outside the range <see cref="MinSwitchValue"/> to <see cref="MaxSwitchValue"/></exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Can throw a <see cref="NotImplementedException"/> if <see cref="CanWrite"/> is False.</b></p>
        /// <para>If the value is more than <see cref="MaxSwitchValue"/> or less than <see cref="MinSwitchValue"/>
        /// then the method must throw an <see cref="InvalidValueException"/>.</para>
        /// <para>A set value that is intermediate between the values specified by <see cref="SwitchStep"/> should result in the device being set to an achievable value close to the requested set value.</para>
        /// <para>Devices are numbered from 0 to <see cref="MaxSwitch"/> - 1.</para>
        /// <para>This is a Version 2 method.</para>
        /// </remarks>
        public void SetSwitchValue(short id, double value)
        {
            try
            {
                base.Device.SetSwitchValue(id, value);
            }
            catch (ASCOM.NotImplementedException)
            {
                bool bv = value >= 0.5;
                this.SetSwitch(id, bv);
            }
        }
    }
}