using ASCOM.Common;
using ASCOM.Common.Alpaca;
using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.DeviceStateClasses;
using ASCOM.Common.Interfaces;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Reflection;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// ASCOM Alpaca Dome client.
    /// </summary>
    public class AlpacaDome : AlpacaDeviceBaseClass, IDomeV3
    {
        #region Variables and Constants

        #endregion

        #region Initialiser

        /// <summary>
        /// Create a client for an Alpaca Dome device with all parameters set to default values
        /// </summary>
        public AlpacaDome()
        {
            Initialise();
        }

        /// <summary>
        /// Create an Alpaca Dome device specifying all parameters
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
        /// <param name="trustUserGeneratedSslCertificates">Trust user generated SSL certificates</param>
        /// <param name="userAgentProductVersion">Optional product version to include in the User-Agent HTTP header sent to the Alpaca device</param>
        public AlpacaDome(ServiceType serviceType = AlpacaClient.CLIENT_SERVICETYPE_DEFAULT,
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
                          string userAgentProductVersion = null,
                          bool trustUserGeneratedSslCertificates=AlpacaClient.TRUST_USER_GENERATED_SSL_CERTIFICATES_DEFAULT
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
            this.userAgentProductVersion = userAgentProductVersion;
            this.trustUserGeneratedSslCertificates = trustUserGeneratedSslCertificates;

            Initialise();
        }

        /// <summary>
        /// Create a client for an Alpaca Dome device specifying the minimum number of parameters
        /// </summary>
        /// <param name="serviceType">HTTP or HTTPS</param>
        /// <param name="ipAddressString">Alpaca device's IP Address</param>
        /// <param name="portNumber">Alpaca device's IP Port number</param>
        /// <param name="remoteDeviceNumber">Alpaca device's device number e.g. Telescope/0</param>
        /// <param name="strictCasing">Tolerate or throw exceptions  if the Alpaca device does not use strictly correct casing for JSON object element names.</param>
        /// <param name="logger">Optional ILogger instance that can be sued to record operational information during execution</param>
        public AlpacaDome(ServiceType serviceType,
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
                clientDeviceType = DeviceTypes.Dome;

                URIBase = $"{AlpacaConstants.API_URL_BASE}{AlpacaConstants.API_VERSION_V1}/{clientDeviceType}/{remoteDeviceNumber}/";

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
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Trust user generated SSL certificates: {trustUserGeneratedSslCertificates}");

                DynamicClientDriver.CreateHttpClient(ref client, serviceType, ipAddressString, portNumber, clientNumber, clientDeviceType, userName, password, ImageArrayCompression.None,
                    logger, userAgentProductName, userAgentProductVersion, trustUserGeneratedSslCertificates);
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), "Completed initialisation");
            }
            catch (Exception ex)
            {
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), ex.ToString());
            }
        }

        #endregion

        #region Convenience members

        /// <summary>
        /// Dome device state
        /// </summary>
        public DomeState DomeState
        {
            get
            {
                // Create a state object to return.
                DomeState domeState = new DomeState(DeviceState, logger);
                logger.LogMessage(LogLevel.Debug, nameof(DomeState), $"Returning: '{domeState.Altitude}' '{domeState.AtHome}' '{domeState.AtPark}' '{domeState.Azimuth}' '{domeState.ShutterStatus}' '{domeState.Slewing}' '{domeState.TimeStamp}'");

                // Return the device specific state class
                return domeState;
            }
        }

        #endregion

        #region IDomeV2 Implementation

        /// <summary>
        /// Immediately cancel current dome operation.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// Calling this method will immediately disable hardware slewing (<see cref="Slaved" /> will become False). Raises an error if a communications failure occurs, or if the command is known to have failed. 
        /// </remarks>
        public void AbortSlew()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "AbortSlew", MemberTypes.Method);
        }

        /// <summary>
        /// Close shutter or otherwise shield telescope from the sky.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public void CloseShutter()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CloseShutter", MemberTypes.Method);
        }

        /// <summary>
        /// Start operation to search for the dome home position.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="SlavedException">Thrown if <see cref="Slaved"/> is <see langword="true"/>.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// After Home position is established initializes <see cref="Azimuth" /> to the default value and sets the <see cref="AtHome" /> flag. 
        /// Exception if not supported or communications failure. Raises an error if <see cref="Slaved" /> is True.
        /// </remarks>
        public void FindHome()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "FindHome", MemberTypes.Method);
        }

        /// <summary>
        /// Open shutter or otherwise expose telescope to the sky.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Raises an error if not supported or if a communications failure occurs. 
        /// </remarks>
        public void OpenShutter()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "OpenShutter", MemberTypes.Method);
        }

        /// <summary>
        /// Rotate dome in azimuth to park position.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// After assuming programmed park position, sets <see cref="AtPark" /> flag. Raises an error if <see cref="Slaved" /> is True, or if not supported, or if a communications failure has occurred. 
        /// </remarks>
        public void Park()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Park", MemberTypes.Method);
        }

        /// <summary>
        /// Set the current azimuth, altitude position of dome to be the park position.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Raises an error if not supported or if a communications failure occurs. 
        /// </remarks>
        public void SetPark()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SetPark", MemberTypes.Method);
        }

        /// <summary>
        /// Slew the dome to the given altitude position.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="InvalidValueException">If the supplied altitude is outside the range 0..90 degrees.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Raises an error if <see cref="Slaved" /> is True, if not supported, if a communications failure occurs, or if the dome can not reach indicated altitude. 
        /// </remarks>
        /// <param name="Altitude">Target dome altitude (degrees, horizon zero and increasing positive to 90 zenith)</param>
        public void SlewToAltitude(double Altitude)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.ALT_PARAMETER_NAME, Altitude.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SlewToAltitude", Parameters, HttpMethod.Put, MemberTypes.Method);
        }

        /// <summary>
        /// Slew the dome to the given azimuth position.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="SlavedException">Thrown if <see cref="Slaved"/> is <see langword="true"/>.</exception>
        /// <exception cref="InvalidValueException">If the supplied azimuth is outside the range 0..360 degrees.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Raises an error if <see cref="Slaved" /> is True, if not supported, if a communications failure occurs, or if the dome can not reach indicated azimuth. 
        /// </remarks>
        /// <param name="Azimuth">Target azimuth (degrees, North zero and increasing clockwise. i.e., 90 East, 180 South, 270 West)</param>
        public void SlewToAzimuth(double Azimuth)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.AZ_PARAMETER_NAME, Azimuth.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SlewToAzimuth", Parameters, HttpMethod.Put, MemberTypes.Method);
        }

        /// <summary>
        /// Synchronize the current position of the dome to the given azimuth.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="InvalidValueException">If the supplied azimuth is outside the range 0..360 degrees.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Raises an error if not supported or if a communications failure occurs. 
        /// </remarks>
        /// <param name="Azimuth">Target azimuth (degrees, North zero and increasing clockwise. i.e., 90 East, 180 South, 270 West)</param>
        public void SyncToAzimuth(double Azimuth)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.AZ_PARAMETER_NAME, Azimuth.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SyncToAzimuth", Parameters, HttpMethod.Put, MemberTypes.Method);
        }

        /// <summary>
        /// The dome altitude (degrees, horizon zero and increasing positive to 90 zenith).
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Raises an error only if no altitude control. If actual dome altitude can not be read, then reports back the last slew position. 
        /// </remarks>
        public double Altitude
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Altitude", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Indicates whether the dome is in the home position. Raises an error if not supported. 
        /// <para>
        /// This is normally used following a <see cref="FindHome" /> operation. The value is reset with any azimuth slew operation that moves the dome away from the home position.
        /// </para>
        /// <para>
        /// <see cref="AtHome" /> may also become true during normal slew operations, if the dome passes through the home position and the dome controller hardware is capable of detecting that; 
        /// or at the end of a slew operation if the dome comes to rest at the home position.
        /// </para>
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <para>The home position is normally defined by a hardware sensor positioned around the dome circumference and represents a fixed, known azimuth reference.</para>
        /// <para>For some devices, the home position may represent a small range of azimuth values, rather than a discrete value, since dome inertia, the resolution of the home position sensor and/or the azimuth encoder may be
        /// insufficient to return the exact same azimuth value on each occasion. Some dome controllers, on the other hand, will always force the azimuth reading to a fixed value whenever the home position sensor is active.
        /// Because of these potential differences in behaviour, applications should not rely on the reported azimuth position being identical each time <see cref="AtHome" /> is set <c>true</c>.</para>
        /// </remarks>
        /// [ASCOM-135] TPL - Updated documentation
        public bool AtHome
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "AtHome", MemberTypes.Property);
            }
        }

        /// <summary>
        /// True if the dome is in the programmed park position.
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Set only following a <see cref="Park" /> operation and reset with any slew operation. Raises an error if not supported. 
        /// </remarks>
        public bool AtPark
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "AtPark", MemberTypes.Property);
            }
        }

        /// <summary>
        /// The dome azimuth (degrees, North zero and increasing clockwise, i.e., 90 East, 180 South, 270 West)
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>Raises an error only if no azimuth control. If actual dome azimuth can not be read, then reports back last slew position</remarks>
        public double Azimuth
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Azimuth", MemberTypes.Property);
            }
        }

        /// <summary>
        /// True if driver can do a search for home position.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// </remarks>
        public bool CanFindHome
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanFindHome", MemberTypes.Property);
            }
        }

        /// <summary>
        /// True if driver is capable of setting dome altitude.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// </remarks>
        public bool CanPark
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanPark", MemberTypes.Property);
            }
        }

        /// <summary>
        /// True if driver is capable of setting dome altitude.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// </remarks>
        public bool CanSetAltitude
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSetAltitude", MemberTypes.Property);
            }
        }

        /// <summary>
        /// True if driver is capable of setting dome azimuth.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// </remarks>
        public bool CanSetAzimuth
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSetAzimuth", MemberTypes.Property);
            }
        }

        /// <summary>
        /// True if driver can set the dome park position.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// </remarks>
        public bool CanSetPark
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSetPark", MemberTypes.Property);
            }
        }

        /// <summary>
        /// True if driver is capable of automatically operating shutter.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// </remarks>
        public bool CanSetShutter
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSetShutter", MemberTypes.Property);
            }
        }

        /// <summary>
        /// True if the dome hardware supports slaving to a telescope.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// See the notes for the <see cref="Slaved" /> property.
        /// </remarks>
        public bool CanSlave
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSlave", MemberTypes.Property);
            }
        }

        /// <summary>
        /// True if driver is capable of synchronizing the dome azimuth position using the <see cref="SyncToAzimuth" /> method.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// </remarks>
        public bool CanSyncAzimuth
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSyncAzimuth", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Status of the dome shutter or roll-off roof.
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Raises an error only if no shutter control. If actual shutter status can not be read, then reports back the last shutter state. 
        /// </remarks>
        public ShutterState ShutterStatus
        {
            get
            {
                return DynamicClientDriver.GetValue<ShutterState>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "ShutterStatus", MemberTypes.Property);
            }
        }

        /// <summary>
        /// True if the dome is slaved to the telescope in its hardware, else False.
        /// </summary>
        /// <exception cref="NotImplementedException">If Slaved can not be set.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red;margin-bottom:0"><b>Slaved Read must be implemented and must not throw a NotImplementedException. </b></p>
        /// <p style="color:red;margin-top:0"><b>Slaved Write can throw a NotImplementedException.</b></p>
        /// Set this property to True to enable dome-telescope hardware slaving, if supported (see <see cref="CanSlave" />). Raises an exception on any attempt to set 
        /// this property if hardware slaving is not supported). Always returns False if hardware slaving is not supported. 
        /// </remarks>
        public bool Slaved
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Slaved", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetBool(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Slaved", value, MemberTypes.Property);
            }
        }

        /// <summary>
        /// True if any part of the dome is currently moving, False if all dome components are steady.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red;margin-bottom:0"><b>Slewing must be implemented and must not throw a NotImplementedException. </b></p>
        /// Raises an error if <see cref="Slaved" /> is True, if not supported, if a communications failure occurs, or if the dome can not reach indicated azimuth. 
        /// </remarks>
        public bool Slewing
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Slewing", MemberTypes.Property);
            }
        }

        #endregion

        #region IDomeV3 implementation

        // No new members

        #endregion

    }
}
