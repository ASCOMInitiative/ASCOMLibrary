using ASCOM.Common;
using ASCOM.Common.Alpaca;
using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.DeviceStateClasses;
using ASCOM.Common.Interfaces;
using System;
using System.Reflection;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// ASCOM Alpaca ObservingConditions client
    /// </summary>
    public class AlpacaObservingConditions : AlpacaDeviceBaseClass, IObservingConditionsV2
    {
        #region Variables and Constants

        #endregion

        #region Initialiser

        /// <summary>
        /// Create a client for an Alpaca ObservingConditions device with all parameters set to default values
        /// </summary>
        public AlpacaObservingConditions()
        {
            Initialise();
        }

        /// <summary>
        /// Initializes a new instance of the AlpacaCamera class using the specified configuration settings.
        /// </summary>
        /// <param name="configuration">The configuration settings used to initialize the camera. Cannot be null.</param>
        public AlpacaObservingConditions(AlpacaConfiguration configuration)
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
        /// Create an Alpaca ObservingConditions device specifying all parameters
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
        public AlpacaObservingConditions(ServiceType serviceType = AlpacaClient.CLIENT_SERVICETYPE_DEFAULT,
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
        /// Create a client for an Alpaca ObservingConditions device specifying the minimum number of parameters
        /// </summary>
        /// <param name="serviceType">HTTP or HTTPS</param>
        /// <param name="ipAddressString">Alpaca device's IP Address</param>
        /// <param name="portNumber">Alpaca device's IP Port number</param>
        /// <param name="remoteDeviceNumber">Alpaca device's device number e.g. Telescope/0</param>
        /// <param name="strictCasing">Tolerate or throw exceptions  if the Alpaca device does not use strictly correct casing for JSON object element names.</param>
        /// <param name="logger">Optional ILogger instance that can be sued to record operational information during execution</param>
        public AlpacaObservingConditions(ServiceType serviceType,
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
                clientDeviceType = DeviceTypes.ObservingConditions;

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
        /// ObservingConditions device state
        /// </summary>
        public ObservingConditionsState ObservingConditionsState
        {
            get
            {
                // Create a state object to return.
                ObservingConditionsState observingConditionsState = new ObservingConditionsState(DeviceState, logger);
                logger.LogMessage(LogLevel.Debug, nameof(ObservingConditionsState), $"Returning: " +
                    $"Cloud cover: '{observingConditionsState.CloudCover}', " +
                    $"Dew point: '{observingConditionsState.DewPoint}', " +
                    $"Humidity: '{observingConditionsState.Humidity}', " +
                    $"Pressure: '{observingConditionsState.Pressure}', " +
                    $"Rain rate: '{observingConditionsState.RainRate}', " +
                    $"Sky brightness: '{observingConditionsState.SkyBrightness}', " +
                    $"Sky quality: '{observingConditionsState.SkyQuality}', " +
                    $"Sky temperature'{observingConditionsState.SkyTemperature}', " +
                    $"Star FWHM: '{observingConditionsState.StarFWHM}', " +
                    $"Temperature: '{observingConditionsState.Temperature}', " +
                    $"Wind direction: '{observingConditionsState.WindDirection}', " +
                    $"Wind gust: '{observingConditionsState.WindGust}', " +
                    $"Wind speed: '{observingConditionsState.WindSpeed}', " +
                    $"Time stamp: '{observingConditionsState.TimeStamp}'");

                // Return the device specific state class
                return observingConditionsState;
            }
        }

        #endregion

        #region IObservingConditionsV1 Implementation

        /// <inheritdoc/>
        public double TimeSinceLastUpdate(string PropertyName)
        {
            return DynamicClientDriver.GetStringIndexedDouble(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "TimeSinceLastUpdate", PropertyName, MemberTypes.Method);
        }

        /// <inheritdoc/>
        public string SensorDescription(string PropertyName)
        {
            return DynamicClientDriver.GetStringIndexedString(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SensorDescription", PropertyName, MemberTypes.Method);
        }

        /// <inheritdoc/>
        public void Refresh()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Refresh", MemberTypes.Method);
        }

        /// <inheritdoc/>
        public double AveragePeriod
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "AveragePeriod", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetDouble(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "AveragePeriod", value, MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double CloudCover
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CloudCover", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double DewPoint
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "DewPoint", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double Humidity
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Humidity", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double Pressure
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Pressure", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double RainRate
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "RainRate", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double SkyBrightness
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SkyBrightness", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double SkyQuality
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SkyQuality", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double StarFWHM
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "StarFWHM", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double SkyTemperature
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SkyTemperature", MemberTypes.Property);
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
        public double WindDirection
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "WindDirection", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double WindGust
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "WindGust", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double WindSpeed
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "WindSpeed", MemberTypes.Property);
            }
        }

        #endregion

        #region IObservingConditionsV2 implementation

        // No new members

        #endregion

    }
}