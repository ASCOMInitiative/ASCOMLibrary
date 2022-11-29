using ASCOM.Common;
using ASCOM.Common.Alpaca;
using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// ASCOM Alpaca Switch client
    /// </summary>
    public class AlpacaSwitch : AlpacaDeviceBaseClass, ISwitchV2
    {
        #region Variables and Constants

        #endregion

        #region Initialiser

        /// <summary>
        /// Create a client for an Alpaca Switch device with all parameters set to default values
        /// </summary>
        public AlpacaSwitch()
        {
            Initialise();
        }

        /// <summary>
        /// Create an Alpaca Switch device specifying all parameters
        /// </summary>
        /// <param name="serviceType">HTTP or HTTPS</param>
        /// <param name="ipAddressString">Alpaca device's IP Address</param>
        /// <param name="portNumber">Alpaca device's IP Port number</param>
        /// <param name="remoteDeviceNumber">Alpaca device's device number e.g. Telescope/0</param>
        /// <param name="establishConnectionTimeout">Timeout (seconds) to initially connect to the Alpaca device</param>
        /// <param name="standardDeviceResponseTimeout">Timeout (seconds) for transactions that are expected to complete quickly e.g. retrieving CanXXX properties</param>
        /// <param name="longDeviceResponseTimeout">Timeout (seconds) for transactions that are expected to take a long time to complete e.g. Camera.ImageArray</param>
        /// <param name="clientNumber">Arbitrary integer that represents this client. (Should be the same for all transactions from this client)</param>
        /// <param name="userName">Basic authentication user name for the Alpaca device</param>
        /// <param name="password">basic authentication password for the Alpaca device</param>
        /// <param name="strictCasing">Tolerate or throw exceptions  if the Alpaca device does not use strictly correct casing for JSON object element names.</param>
        /// <param name="logger">Optional ILogger instance that can be sued to record operational information during execution</param>
        /// <param name="userAgentProductName">Optional product name to include in the User-Agent HTTP header sent to the Alpaca device</param>
        /// <param name="userAgentProductVersion">Optional product version to include in the User-Agent HTTP header sent to the Alpaca device</param>
        public AlpacaSwitch(ServiceType serviceType = AlpacaClient.CLIENT_SERVICETYPE_DEFAULT,
                            string ipAddressString = AlpacaClient.CLIENT_IPADDRESS_DEFAULT,
                            int portNumber = AlpacaClient.CLIENT_IPPORT_DEFAULT,
                            int remoteDeviceNumber = AlpacaClient.CLIENT_REMOTEDEVICENUMBER_DEFAULT,
                            int establishConnectionTimeout = AlpacaClient.CLIENT_ESTABLISHCONNECTIONTIMEOUT_DEFAULT,
                            int standardDeviceResponseTimeout = AlpacaClient.CLIENT_STANDARDCONNECTIONTIMEOUT_DEFAULT,
                            int longDeviceResponseTimeout = AlpacaClient.CLIENT_LONGCONNECTIONTIMEOUT_DEFAULT,
                            uint clientNumber = AlpacaClient.CLIENT_CLIENTNUMBER_DEFAULT,
                            string userName = AlpacaClient.CLIENT_USERNAME_DEFAULT,
                            string password = AlpacaClient.CLIENT_PASSWORD_DEFAULT,
                            bool strictCasing = AlpacaClient.CLIENT_STRICTCASING_DEFAULT,
                            ILogger logger = AlpacaClient.CLIENT_LOGGER_DEFAULT,
                            string userAgentProductName = null,
                            string userAgentProductVersion = null
           )
        {
            this.serviceType = serviceType;
            this.ipAddressString = ipAddressString;
            this.portNumber = portNumber;
            this.remoteDeviceNumber = remoteDeviceNumber;
            this.establishConnectionTimeout = establishConnectionTimeout;
            this.standardDeviceResponseTimeout = standardDeviceResponseTimeout;
            this.longDeviceResponseTimeout = longDeviceResponseTimeout;
            this.clientNumber = clientNumber;
            this.userName = userName;
            this.password = password;
            this.strictCasing = strictCasing;
            this.logger = logger;
            this.userAgentProductName = userAgentProductName;
            this.userAgentProductName = userAgentProductVersion;

            Initialise();
        }

        /// <summary>
        /// Create a client for an Alpaca Switch device specifying the minimum number of parameters
        /// </summary>
        /// <param name="serviceType">HTTP or HTTPS</param>
        /// <param name="ipAddressString">Alpaca device's IP Address</param>
        /// <param name="portNumber">Alpaca device's IP Port number</param>
        /// <param name="remoteDeviceNumber">Alpaca device's device number e.g. Telescope/0</param>
        /// <param name="strictCasing">Tolerate or throw exceptions  if the Alpaca device does not use strictly correct casing for JSON object element names.</param>
        /// <param name="logger">Optional ILogger instance that can be sued to record operational information during execution</param>
        public AlpacaSwitch(ServiceType serviceType,
                         string ipAddressString,
                         int portNumber,
                         int remoteDeviceNumber,
                             bool strictCasing,
                      ILogger logger)
        {
            this.serviceType = serviceType;
            this.ipAddressString = ipAddressString;
            this.portNumber = portNumber;
            this.remoteDeviceNumber = remoteDeviceNumber;
            this.strictCasing = strictCasing;
            base.logger = logger;
            clientNumber = DynamicClientDriver.GetUniqueClientNumber();
            Initialise();
        }
        private void Initialise()
        {
            try
            {
                // Set the device type
                clientDeviceType = DeviceTypes.Switch;

                URIBase = $"{AlpacaConstants.API_URL_BASE}{AlpacaConstants.API_VERSION_V1}/{clientDeviceType}/{remoteDeviceNumber}/";
                Version version = Assembly.GetEntryAssembly().GetName().Version;

                // List parameter values
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Service type: {serviceType}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"IP address: {ipAddressString}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"IP port: {portNumber}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Remote device number: {remoteDeviceNumber}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Establish communications timeout: {establishConnectionTimeout}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Standard device response timeout: {standardDeviceResponseTimeout}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Long device response timeout: {longDeviceResponseTimeout}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Client number: {clientNumber}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"User name is Null or Empty: {string.IsNullOrEmpty(userName)}, User name is Null or White Space: {string.IsNullOrWhiteSpace(userName)}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"User name length: {password.Length}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Password is Null or Empty: {string.IsNullOrEmpty(password)}, Password is Null or White Space: {string.IsNullOrWhiteSpace(password)}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Password length: {password.Length}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Strict casing: {strictCasing}");

                DynamicClientDriver.ConnectToRemoteDevice(ref client, serviceType, ipAddressString, portNumber, clientNumber, clientDeviceType, standardDeviceResponseTimeout, userName, password, ImageArrayCompression.None, logger, userAgentProductName, userAgentProductVersion);
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), "Completed initialisation");
            }
            catch (Exception ex)
            {
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), ex.ToString());
            }
        }

        #endregion

        #region ISwitchV2 Implementation

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
        /// <para>This is a Version 2 method, version 1 switch devices can be assumed to be writeable.</para>
        /// </remarks>
        public bool CanWrite(short id)
        {
            return DynamicClientDriver.GetShortIndexedBool(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanWrite", id, MemberTypes.Method);
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
        /// <para>All devices must implement this. A multi-state device will return true if the device is at the maximum value, false if the value is at the minimum
        /// and either true or false as specified by the driver developer for intermediate values.</para>
        /// <para>Some devices do not support reading their state although they do allow state to be set. In these cases, on start-up, the driver can not know the hardware state and it is recommended that the 
        /// driver either:</para>
        /// <list type="bullet">
        /// <item><description>Sets the device to a known state on connection</description></item>
        /// <item><description>Throws an <see cref="InvalidOperationException"/> until the client software has set the device state for the first time</description></item>
        /// </list>
        /// <para>In both cases the driver should save a local copy of the state which it last set and return this through <see cref="GetSwitch" /> and <see cref="GetSwitchValue" /></para>
        /// <para>Devices are numbered from 0 to <see cref="MaxSwitch"/> - 1</para></remarks>
        public bool GetSwitch(short id)
        {
            return DynamicClientDriver.GetShortIndexedBool(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "GetSwitch", id, MemberTypes.Method);
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
            return DynamicClientDriver.GetShortIndexedString(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "GetSwitchDescription", id, MemberTypes.Method);
        }

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
            return DynamicClientDriver.GetShortIndexedString(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "GetSwitchName", id, MemberTypes.Method);
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
        /// <para>Some devices do not support reading their state although they do allow state to be set. In these cases, on start-up, the driver can not know the hardware state and it is recommended that the 
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
            return DynamicClientDriver.GetShortIndexedDouble(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "GetSwitchValue", id, MemberTypes.Method);
        }

        /// <summary>
        /// The number of switch devices managed by this driver
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <returns>The number of devices managed by this driver.</returns>
        /// <remarks><p style="color:red"><b>Must be implemented, must not throw a <see cref="NotImplementedException"/></b></p> 
        /// <p>Devices are numbered from 0 to <see cref="MaxSwitch"/> - 1</p></remarks>
        public short MaxSwitch
        {
            get
            {
                return DynamicClientDriver.GetValue<short>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "MaxSwitch", MemberTypes.Property);
            }
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
            return DynamicClientDriver.GetShortIndexedDouble(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "MaxSwitchValue", id, MemberTypes.Method);
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
            return DynamicClientDriver.GetShortIndexedDouble(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "MinSwitchValue", id, MemberTypes.Method);
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
            return DynamicClientDriver.GetShortIndexedDouble(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SwitchStep", id, MemberTypes.Method);
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
            DynamicClientDriver.SetStringWithShortParameter(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SetSwitchName", id, name, MemberTypes.Method);
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
            DynamicClientDriver.SetBoolWithShortParameter(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SetSwitch", id, state, MemberTypes.Method);
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
            DynamicClientDriver.SetDoubleWithShortParameter(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SetSwitchValue", id, value, MemberTypes.Method);
        }

        #endregion

    }
}