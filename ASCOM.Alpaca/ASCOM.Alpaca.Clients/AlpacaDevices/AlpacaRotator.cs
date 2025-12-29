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
    /// ASCOM Alpaca Rotator client
    /// </summary>
    public class AlpacaRotator : AlpacaDeviceBaseClass, IRotatorV4
    {
        #region Variables and Constants

        #endregion

        #region Initialiser

        /// <summary>
        /// Create a client for an Alpaca Rotator device with all parameters set to default values
        /// </summary>
        public AlpacaRotator()
        {
            Initialise();
        }

        /// <summary>
        /// Create an Alpaca Rotator device specifying all parameters
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
        public AlpacaRotator(ServiceType serviceType = AlpacaClient.CLIENT_SERVICETYPE_DEFAULT,
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
                             bool trustUserGeneratedSslCertificates = AlpacaClient.TRUST_USER_GENERATED_SSL_CERTIFICATES_DEFAULT,
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
        /// Create a client for an Alpaca Rotator device specifying the minimum number of parameters
        /// </summary>
        /// <param name="serviceType">HTTP or HTTPS</param>
        /// <param name="ipAddressString">Alpaca device's IP Address</param>
        /// <param name="portNumber">Alpaca device's IP Port number</param>
        /// <param name="remoteDeviceNumber">Alpaca device's device number e.g. Telescope/0</param>
        /// <param name="strictCasing">Tolerate or throw exceptions  if the Alpaca device does not use strictly correct casing for JSON object element names.</param>
        /// <param name="logger">Optional ILogger instance that can be sued to record operational information during execution</param>
        public AlpacaRotator(ServiceType serviceType,
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
                clientDeviceType = DeviceTypes.Rotator;

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

        /// <summary>
        /// Rotator device state
        /// </summary>
        public RotatorState RotatorState
        {
            get
            {
                // Create a state object to return.
                RotatorState rotatorState = new RotatorState(DeviceState, logger);
                logger.LogMessage(LogLevel.Debug, nameof(RotatorState), $"Returning: " +
                    $"Cloud cover: '{rotatorState.IsMoving}', " +
                    $"Dew point: '{rotatorState.MechanicalPosition}', " +
                    $"Humidity: '{rotatorState.Position}', " +
                    $"Time stamp: '{rotatorState.TimeStamp}'");

                // Return the device specific state class
                return rotatorState;
            }
        }

        #endregion

        #region IRotator Implementation

        /// <inheritdoc/>
        public bool CanReverse
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanReverse", MemberTypes.Property);
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
        public float Position
        {
            get
            {
                return DynamicClientDriver.GetValue<float>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Position", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool Reverse
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Reverse", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetBool(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Reverse", value, MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public float StepSize
        {
            get
            {
                return DynamicClientDriver.GetValue<float>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "StepSize", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public float TargetPosition
        {
            get
            {
                return DynamicClientDriver.GetValue<float>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "TargetPosition", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public void Halt()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "Halt", MemberTypes.Method);
            LogMessage(logger, clientNumber, "Halt", "Rotator halted OK");
        }

        /// <inheritdoc/>
        public void Move(float Position)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.POSITION_PARAMETER_NAME, Position.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "Move", Parameters, HttpMethod.Put, MemberTypes.Method);
            LogMessage(logger, clientNumber, "Move", $"Rotator moved to relative position {Position} OK");
        }

        /// <inheritdoc/>
        public void MoveAbsolute(float Position)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.POSITION_PARAMETER_NAME, Position.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "MoveAbsolute", Parameters, HttpMethod.Put, MemberTypes.Method);
            LogMessage(logger, clientNumber, "MoveAbsolute", $"Rotator moved to absolute position {Position} OK");
        }

        #endregion

        #region IRotatorV3 Properties and Methods

        /// <inheritdoc/>
        public float MechanicalPosition
        {
            get
            {
                return DynamicClientDriver.GetValue<float>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "MechanicalPosition", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public void Sync(float Position)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.POSITION_PARAMETER_NAME, Position.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "Sync", Parameters, HttpMethod.Put, MemberTypes.Method);
            LogMessage(logger, clientNumber, "Sync", $"Rotator synced to sky position {Position} OK");
        }

        /// <inheritdoc/>
        public void MoveMechanical(float Position)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.POSITION_PARAMETER_NAME, Position.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "MoveMechanical", Parameters, HttpMethod.Put, MemberTypes.Method);
            LogMessage(logger, clientNumber, "MoveMechanical", $"Rotator moved to mechanical position {Position} OK");
        }

        #endregion

        #region IRotatorV4 implementation

        // No new members

        #endregion

    }
}