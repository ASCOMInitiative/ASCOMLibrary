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
        /// Initializes a new instance of the AlpacaCamera class using the specified configuration settings.
        /// </summary>
        /// <param name="configuration">The configuration settings used to initialize the camera. Cannot be null.</param>
        public AlpacaDome(AlpacaConfiguration configuration)
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
        /// <param name="request100Continue">Request 100-continue behaviour for HTTP requests. Defaults to false.</param>
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

        /// <inheritdoc/>
        public void AbortSlew()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "AbortSlew", MemberTypes.Method);
        }

        /// <inheritdoc/>
        public void CloseShutter()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CloseShutter", MemberTypes.Method);
        }

        /// <inheritdoc/>
        public void FindHome()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "FindHome", MemberTypes.Method);
        }

        /// <inheritdoc/>
        public void OpenShutter()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "OpenShutter", MemberTypes.Method);
        }

        /// <inheritdoc/>
        public void Park()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Park", MemberTypes.Method);
        }

        /// <inheritdoc/>
        public void SetPark()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SetPark", MemberTypes.Method);
        }

        /// <inheritdoc/>
        public void SlewToAltitude(double Altitude)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.ALT_PARAMETER_NAME, Altitude.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SlewToAltitude", Parameters, HttpMethod.Put, MemberTypes.Method);
        }

        /// <inheritdoc/>
        public void SlewToAzimuth(double Azimuth)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.AZ_PARAMETER_NAME, Azimuth.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SlewToAzimuth", Parameters, HttpMethod.Put, MemberTypes.Method);
        }

        /// <inheritdoc/>
        public void SyncToAzimuth(double Azimuth)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.AZ_PARAMETER_NAME, Azimuth.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SyncToAzimuth", Parameters, HttpMethod.Put, MemberTypes.Method);
        }

        /// <inheritdoc/>
        public double Altitude
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Altitude", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool AtHome
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "AtHome", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool AtPark
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "AtPark", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double Azimuth
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Azimuth", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool CanFindHome
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanFindHome", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool CanPark
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanPark", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool CanSetAltitude
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSetAltitude", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool CanSetAzimuth
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSetAzimuth", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool CanSetPark
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSetPark", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool CanSetShutter
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSetShutter", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool CanSlave
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSlave", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool CanSyncAzimuth
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSyncAzimuth", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public ShutterState ShutterStatus
        {
            get
            {
                return DynamicClientDriver.GetValue<ShutterState>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "ShutterStatus", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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
