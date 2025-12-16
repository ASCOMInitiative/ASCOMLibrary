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
    /// ASCOM Alpaca Focuser client
    /// </summary>
    public class AlpacaFocuser : AlpacaDeviceBaseClass, IFocuserV4
    {
        #region Variables and Constants

        #endregion

        #region Initialiser

        /// <summary>
        /// Create a client for an Alpaca Focuser device with all parameters set to default values
        /// </summary>
        public AlpacaFocuser()
        {
            Initialise();
        }

        /// <summary>
        /// Create an Alpaca Focuser device specifying all parameters
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
        /// <param name="request100Continue"></param>Request 100-continue behaviour for HTTP requests. Defaults to false.
        public AlpacaFocuser(ServiceType serviceType = AlpacaClient.CLIENT_SERVICETYPE_DEFAULT,
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
                             bool trustUserGeneratedSslCertificates=AlpacaClient.TRUST_USER_GENERATED_SSL_CERTIFICATES_DEFAULT,
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
        /// Create a client for an Alpaca Focuser device specifying the minimum number of parameters
        /// </summary>
        /// <param name="serviceType">HTTP or HTTPS</param>
        /// <param name="ipAddressString">Alpaca device's IP Address</param>
        /// <param name="portNumber">Alpaca device's IP Port number</param>
        /// <param name="remoteDeviceNumber">Alpaca device's device number e.g. Telescope/0</param>
        /// <param name="strictCasing">Tolerate or throw exceptions  if the Alpaca device does not use strictly correct casing for JSON object element names.</param>
        /// <param name="logger">Optional ILogger instance that can be sued to record operational information during execution</param>
        public AlpacaFocuser(ServiceType serviceType,
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
                clientDeviceType = DeviceTypes.Focuser;

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

        /// <summary>
        /// Focuser device state
        /// </summary>
        public FocuserState FocuserState
        {
            get
            {
                // Create a state object to return.
                FocuserState focuserState = new FocuserState(DeviceState, logger);
                logger.LogMessage(LogLevel.Debug, nameof(FocuserState), $"Returning: '{focuserState.IsMoving}' '{focuserState.Position}' '{focuserState.Temperature}' '{focuserState.TimeStamp}'");

                // Return the device specific state class
                return focuserState;
            }
        }

        #endregion

        #region IFocuserV3 members

        /// <inheritdoc/>
        public bool Absolute
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Absolute", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool IsMoving
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "IsMoving", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public int MaxIncrement
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "MaxIncrement", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public int MaxStep
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "MaxStep", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public int Position
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Position", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double StepSize
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "StepSize", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool TempComp
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "TempComp", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetBool(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "TempComp", value, MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool TempCompAvailable
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "TempCompAvailable", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double Temperature
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Temperature", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public void Halt()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "Halt", MemberTypes.Method);
            LogMessage(logger, clientNumber, "Halt", "Halted OK");
        }

        /// <inheritdoc/>
        public void Move(int Position)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.POSITION_PARAMETER_NAME, Position.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "Move", Parameters, HttpMethod.Put, MemberTypes.Method);
        }

        #endregion

        #region IFocuserV4 implementation

        // No new members

        #endregion

    }
}
