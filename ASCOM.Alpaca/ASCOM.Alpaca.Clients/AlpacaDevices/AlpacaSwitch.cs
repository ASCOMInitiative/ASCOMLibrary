using ASCOM.Common;
using ASCOM.Common.Alpaca;
using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// ASCOM Alpaca Switch client
    /// </summary>
    public class AlpacaSwitch : AlpacaDeviceBaseClass, ISwitchV3
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
        /// Initializes a new instance of the AlpacaCamera class using the specified configuration settings.
        /// </summary>
        /// <param name="configuration">The configuration settings used to initialize the camera. Cannot be null.</param>
        public AlpacaSwitch(AlpacaConfiguration configuration)
        {
            this.serviceType = configuration.ServiceType;
            this.ipAddressString = configuration.IpAddressString;
            this.portNumber = configuration.PortNumber;
            this.remoteDeviceNumber = configuration.RemoteDeviceNumber;
            this.establishConnectionTimeout = configuration.EstablishConnectionTimeout;
            this.standardDeviceResponseTimeout = configuration.StandardDeviceResponseTimeout;
            this.longDeviceResponseTimeout = configuration.LongDeviceResponseTimeout;
            this.clientNumber = configuration.ClientNumber;
            this.userName = configuration.UserName;
            this.password = configuration.Password;
            this.strictCasing = configuration.StrictCasing;
            this.logger = configuration.Logger;
            this.userAgentProductName = configuration.UserAgentProductName;
            this.userAgentProductVersion = configuration.UserAgentProductVersion;
            this.trustUserGeneratedSslCertificates = configuration.TrustUserGeneratedSslCertificates;
            this.request100Continue = configuration.Request100Continue;

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
        /// <param name="trustUserGeneratedSslCertificates">Trust user generated SSL certificates</param>
        /// <param name="request100Continue">Request 100-continue behaviour for HTTP requests. Defaults to false.</param>
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
                            string userAgentProductVersion = null, bool trustUserGeneratedSslCertificates = AlpacaClient.TRUST_USER_GENERATED_SSL_CERTIFICATES_DEFAULT,
                            bool request100Continue = AlpacaClient.CLIENT_REQUEST_100_CONTINUE_DEFAULT
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
            this.request100Continue = request100Continue;

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
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Request 100CONTINUE: {request100Continue}");

                DynamicClientDriver.CreateHttpClient(ref client, serviceType, ipAddressString, portNumber, clientNumber, clientDeviceType, userName, password, ImageArrayCompression.None,
                    logger, userAgentProductName, userAgentProductVersion, trustUserGeneratedSslCertificates, request100Continue);
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), "Completed initialisation");
            }
            catch (Exception ex)
            {
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), ex.ToString());
            }
        }

        #endregion

        #region Convenience members

        // There is no SwitchState member because the number of switches is a dynamic, user configured value as is switch naming, which makes it impossible to model in a class with fixed properties.

        #endregion

        #region ISwitchV2 Implementation

        /// <inheritdoc/>
        public bool CanWrite(short id)
        {
            return DynamicClientDriver.GetShortIndexedBool(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanWrite", id, MemberTypes.Method);
        }

        /// <inheritdoc/>
        public bool GetSwitch(short id)
        {
            return DynamicClientDriver.GetShortIndexedBool(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "GetSwitch", id, MemberTypes.Method);
        }

        /// <inheritdoc/>
        public string GetSwitchDescription(short id)
        {
            return DynamicClientDriver.GetShortIndexedString(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "GetSwitchDescription", id, MemberTypes.Method);
        }

        /// <inheritdoc/>
        public string GetSwitchName(short id)
        {
            return DynamicClientDriver.GetShortIndexedString(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "GetSwitchName", id, MemberTypes.Method);
        }

        /// <inheritdoc/>
        public double GetSwitchValue(short id)
        {
            return DynamicClientDriver.GetShortIndexedDouble(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "GetSwitchValue", id, MemberTypes.Method);
        }

        /// <inheritdoc/>
        public short MaxSwitch
        {
            get
            {
                return DynamicClientDriver.GetValue<short>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "MaxSwitch", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double MaxSwitchValue(short id)
        {
            return DynamicClientDriver.GetShortIndexedDouble(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "MaxSwitchValue", id, MemberTypes.Method);
        }

        /// <inheritdoc/>
        public double MinSwitchValue(short id)
        {
            return DynamicClientDriver.GetShortIndexedDouble(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "MinSwitchValue", id, MemberTypes.Method);
        }

        /// <inheritdoc/>
        public double SwitchStep(short id)
        {
            return DynamicClientDriver.GetShortIndexedDouble(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SwitchStep", id, MemberTypes.Method);
        }

        /// <inheritdoc/>
        public void SetSwitchName(short id, string name)
        {
            DynamicClientDriver.SetStringWithShortParameter(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SetSwitchName", id, name, MemberTypes.Method);
        }

        /// <inheritdoc/>
        public void SetSwitch(short id, bool state)
        {
            DynamicClientDriver.SetBoolWithShortParameter(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SetSwitch", id, state, MemberTypes.Method);
        }

        /// <inheritdoc/>
        public void SetSwitchValue(short id, double value)
        {
            DynamicClientDriver.SetDoubleWithShortParameter(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SetSwitchValue", id, value, MemberTypes.Method);
        }

        #endregion

        #region ISwitchV3 implementation

        /// <inheritdoc />
        public void SetAsync(short id, bool state)
        {
            // Check whether this device supports asynchronous methods
            if (DeviceCapabilities.HasConnectAndDeviceState(DeviceTypes.Switch, InterfaceVersion))
            {
                // Platform 7 or later device so use the device's method
                DynamicClientDriver.SetBoolWithShortParameter(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SetAsync", id, state, MemberTypes.Method);
                return;
            }

            // Platform 6 or earlier device
            throw new NotImplementedException($"DriverAccess.Switch - SetAsync is not supported by this device because it exposes interface ISwitchV{InterfaceVersion}.");
        }

        /// <inheritdoc />
        public void SetAsyncValue(short id, double value)
        {
            // Check whether this device supports asynchronous methods
            if (DeviceCapabilities.HasConnectAndDeviceState(DeviceTypes.Switch, InterfaceVersion))
            {
                // Platform 7 or later device so use the device's method
                DynamicClientDriver.SetDoubleWithShortParameter(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SetAsyncValue", id, value, MemberTypes.Method);
                return;
            }

            // Platform 6 or earlier device
            throw new NotImplementedException($"DriverAccess.Switch - SetAsyncValue is not supported by this device because it exposes interface ISwitchV{InterfaceVersion}.");
        }

        /// <inheritdoc />
        public bool CanAsync(short id)
        {
            // Check whether this device supports asynchronous methods
            if (DeviceCapabilities.HasConnectAndDeviceState(DeviceTypes.Switch, InterfaceVersion))
            {
                // Platform 7 or later device so use the device's method
                return DynamicClientDriver.GetShortIndexedBool(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanAsync", id, MemberTypes.Method);
            }

            // Platform 6 or earlier device - async is not supported so return false to show no async support.
            return false;
        }

        /// <inheritdoc />
        public bool StateChangeComplete(short id)
        {
            // Check whether this device supports asynchronous methods
            if (DeviceCapabilities.HasConnectAndDeviceState(DeviceTypes.Switch, InterfaceVersion))
            {
                // Platform 7 or later device so use the device's method
                return DynamicClientDriver.GetShortIndexedBool(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "StateChangeComplete", id, MemberTypes.Method);
            }

            // Platform 6 or earlier device
            throw new NotImplementedException($"DriverAccess.Switch - StateChangeComplete is not supported by this device because it exposes interface ISwitchV{InterfaceVersion}.");
        }

        /// <inheritdoc />
        public void CancelAsync(short id)
        {
            // Check whether this device supports asynchronous methods
            if (DeviceCapabilities.HasConnectAndDeviceState(DeviceTypes.Switch, InterfaceVersion))
            {
                // Platform 7 or later device so use the device's method
                Dictionary<string, string> Parameters = new Dictionary<string, string>() { { AlpacaConstants.ID_PARAMETER_NAME, id.ToString() } };
                DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CancelAsync", Parameters, HttpMethod.Put, MemberTypes.Method);
                return;
            }

            // Platform 6 or earlier device
            throw new NotImplementedException($"DriverAccess.Switch - CancelAsync is not supported by this device because it exposes interface ISwitchV{InterfaceVersion}.");
        }

        #endregion

    }
}