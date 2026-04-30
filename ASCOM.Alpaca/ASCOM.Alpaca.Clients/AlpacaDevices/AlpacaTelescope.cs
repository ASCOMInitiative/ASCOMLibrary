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
    /// ASCOM Alpaca Telescope client
    /// </summary>
    public class AlpacaTelescope : AlpacaDeviceBaseClass, ITelescopeV4
    {
        #region Variables and Constants

        #endregion

        #region Initialiser

        /// <summary>
        /// Create a client for an Alpaca Telescope device with all parameters set to default values
        /// </summary>
        public AlpacaTelescope()
        {
            Initialise();
        }

        /// <summary>
        /// Initializes a new instance of the AlpacaCamera class using the specified configuration settings.
        /// </summary>
        /// <param name="configuration">The configuration settings used to initialize the camera. Cannot be null.</param>
        public AlpacaTelescope(AlpacaConfiguration configuration)
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
            this.throwOnBadDateTimeJSON=configuration.ThrowOnBadDateTimeJSON;
            this.request100Continue = configuration.Request100Continue;
            this.numberOfRetries = configuration.NumberOfRetries;

            Initialise();
        }

        /// <summary>
        /// Create an Alpaca Telescope device specifying all parameters
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
        /// <param name="throwOnBadDateTimeJSON">Throw an exception if a returned JSON DateTime value is not a UTC value (has a trailing Z character). Defaults to false.</param>
        /// <param name="request100Continue">Request 100-continue behaviour for HTTP requests. Defaults to false.</param>
        /// <param name="numberOfRetries">Number of communication retries this client will make. Defaults to 1.</param>
        public AlpacaTelescope(ServiceType serviceType = AlpacaClient.CLIENT_SERVICETYPE_DEFAULT,
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
                               bool throwOnBadDateTimeJSON = AlpacaClient.THROW_ON_BAD_JSON_DATE_TIME_DEFAULT,
                               bool request100Continue = AlpacaClient.CLIENT_REQUEST_100_CONTINUE_DEFAULT,
                               int numberOfRetries = AlpacaClient.NUMBER_OF_RETRIES_DEFAULT
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
            this.throwOnBadDateTimeJSON= throwOnBadDateTimeJSON;
            this.request100Continue = request100Continue;
            this.numberOfRetries = numberOfRetries;

            Initialise();
        }

        /// <summary>
        /// Create a client for an Alpaca Telescope device specifying the minimum number of parameters
        /// </summary>
        /// <param name="serviceType">HTTP or HTTPS</param>
        /// <param name="ipAddressString">Alpaca device's IP Address</param>
        /// <param name="portNumber">Alpaca device's IP Port number</param>
        /// <param name="remoteDeviceNumber">Alpaca device's device number e.g. Telescope/0</param>
        /// <param name="strictCasing">Tolerate or throw exceptions  if the Alpaca device does not use strictly correct casing for JSON object element names.</param>
        /// <param name="logger">Optional ILogger instance that can be sued to record operational information during execution</param>
        public AlpacaTelescope(ServiceType serviceType,
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
            clientNumber = RemoteDevice.GetUniqueClientNumber();
            Initialise();
        }
        private void Initialise()
        {
            try
            {
                // Set the device type
                clientDeviceType = DeviceTypes.Telescope;

                uriBase = $"{AlpacaConstants.API_URL_BASE}{AlpacaConstants.API_VERSION_V1}/{clientDeviceType}/{remoteDeviceNumber}/";

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
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Throw on bad JSON DateTime: {throwOnBadDateTimeJSON}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Request 100CONTINUE: {request100Continue}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Number of retries: {numberOfRetries}");

                RemoteDevice.CreateHttpClient(ref client, serviceType, ipAddressString, portNumber, clientNumber, clientDeviceType, userName, password, ImageArrayCompression.None,
                    logger, userAgentProductName, userAgentProductVersion, trustUserGeneratedSslCertificates,request100Continue);
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
        /// State response from the device
        /// </summary>
        public TelescopeState TelescopeState
        {
            get
            {
                // Create a state object to return.
                TelescopeState state = new TelescopeState(DeviceState, logger);
                logger.LogMessage(LogLevel.Debug, nameof(TelescopeState), $"Returning: '{state.Altitude}' '{state.AtHome}' '{state.AtPark}' '{state.Azimuth}' '{state.Declination}' '{state.IsPulseGuiding}' " +
                    $"'{state.RightAscension}' '{state.SideOfPier}' '{state.SiderealTime}' '{state.Slewing}' '{state.Tracking}' '{state.UTCDate}' '{state.TimeStamp}'");

                // Return the device specific state class
                return state;
            }
        }

        #endregion

        #region ITelescopeV3 Implementation

        /// <inheritdoc/>
        public void AbortSlew()
        {
            RemoteDevice.CallNoParameters(CreateParameters(longDeviceResponseTimeout, "AbortSlew", MemberTypes.Method));
            LogMessage(logger, clientNumber, "AbortSlew", $"Slew aborted OK.");
        }

        /// <inheritdoc/>
        public AlignmentMode AlignmentMode
        {
            get
            {
                return RemoteDevice.GetValue<AlignmentMode>(CreateParameters(standardDeviceResponseTimeout, "AlignmentMode", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public double Altitude
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "Altitude", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public double ApertureArea
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "ApertureArea", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public double ApertureDiameter
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "ApertureDiameter", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public bool AtHome
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "AtHome", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public bool AtPark
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "AtPark", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public IAxisRates AxisRates(TelescopeAxis Axis)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.AXIS_PARAMETER_NAME, ((int)Axis).ToString(CultureInfo.InvariantCulture) }
            };
            return RemoteDevice.Send<IAxisRates>(CreateParameters(standardDeviceResponseTimeout, "AxisRates", MemberTypes.Method), Parameters, HttpMethod.Get);
        }

        /// <inheritdoc/>
        public double Azimuth
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "Azimuth", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public bool CanFindHome
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "CanFindHome", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public bool CanMoveAxis(TelescopeAxis Axis)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.AXIS_PARAMETER_NAME, ((int)Axis).ToString(CultureInfo.InvariantCulture) }
            };
            return RemoteDevice.Send<bool>(CreateParameters(standardDeviceResponseTimeout, "CanMoveAxis", MemberTypes.Method), Parameters, HttpMethod.Get);
        }

        /// <inheritdoc/>
        public bool CanPark
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "CanPark", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public bool CanPulseGuide
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "CanPulseGuide", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public bool CanSetDeclinationRate
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "CanSetDeclinationRate", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public bool CanSetGuideRates
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "CanSetGuideRates", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public bool CanSetPark
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "CanSetPark", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public bool CanSetPierSide
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "CanSetPierSide", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public bool CanSetRightAscensionRate
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "CanSetRightAscensionRate", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public bool CanSetTracking
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "CanSetTracking", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public bool CanSlew
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "CanSlew", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public bool CanSlewAltAz
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "CanSlewAltAz", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public bool CanSlewAltAzAsync
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "CanSlewAltAzAsync", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public bool CanSlewAsync
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "CanSlewAsync", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public bool CanSync
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "CanSync", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public bool CanSyncAltAz
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "CanSyncAltAz", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public bool CanUnpark
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "CanUnpark", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public double Declination
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "Declination", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public double DeclinationRate
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "DeclinationRate", MemberTypes.Property));
            }
            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "DeclinationRate", MemberTypes.Property), value);
            }
        }

        /// <inheritdoc/>
        public PointingState DestinationSideOfPier(double RightAscension, double Declination)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.RA_PARAMETER_NAME, RightAscension.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.DEC_PARAMETER_NAME, Declination.ToString(CultureInfo.InvariantCulture) }
            };
            return RemoteDevice.Send<PointingState>(CreateParameters(standardDeviceResponseTimeout, "DestinationSideOfPier", MemberTypes.Method), Parameters, HttpMethod.Get);
        }

        /// <inheritdoc/>
        public bool DoesRefraction
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "DoesRefraction", MemberTypes.Property));
            }
            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "DoesRefraction", MemberTypes.Property), value);
            }
        }

        /// <inheritdoc/>
        public EquatorialCoordinateType EquatorialSystem
        {
            get
            {
                return RemoteDevice.GetValue<EquatorialCoordinateType>(CreateParameters(standardDeviceResponseTimeout, "EquatorialSystem", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public void FindHome()
        {
            RemoteDevice.CallNoParameters(CreateParameters(longDeviceResponseTimeout, "FindHome", MemberTypes.Method));
            LogMessage(logger, clientNumber, "FindHome", "Home found OK");
        }

        /// <inheritdoc/>
        public double FocalLength
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "FocalLength", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public double GuideRateDeclination
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "GuideRateDeclination", MemberTypes.Property));
            }
            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "GuideRateDeclination", MemberTypes.Property), value);
            }
        }

        /// <inheritdoc/>
        public double GuideRateRightAscension
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "GuideRateRightAscension", MemberTypes.Property));
            }
            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "GuideRateRightAscension", MemberTypes.Property), value);
            }
        }

        /// <inheritdoc/>
        public bool IsPulseGuiding
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "IsPulseGuiding", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public void MoveAxis(TelescopeAxis Axis, double Rate)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.AXIS_PARAMETER_NAME, ((int)Axis).ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.RATE_PARAMETER_NAME, Rate.ToString(CultureInfo.InvariantCulture) }
            };
            RemoteDevice.Send<NoReturnValue>(CreateParameters(longDeviceResponseTimeout, "MoveAxis", MemberTypes.Method), Parameters, HttpMethod.Put);
        }

        /// <inheritdoc/>
        public void Park()
        {
            RemoteDevice.CallNoParameters(CreateParameters(longDeviceResponseTimeout, "Park", MemberTypes.Method));
            LogMessage(logger, clientNumber, "Park", "Parked OK");
        }

        /// <inheritdoc/>
        public void PulseGuide(GuideDirection Direction, int Duration)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.DIRECTION_PARAMETER_NAME, ((int)Direction).ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.DURATION_PARAMETER_NAME, Duration.ToString(CultureInfo.InvariantCulture) }
            };
            RemoteDevice.Send<NoReturnValue>(CreateParameters(longDeviceResponseTimeout, "PulseGuide", MemberTypes.Method), Parameters, HttpMethod.Put);
        }

        /// <inheritdoc/>
        public double RightAscension
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "RightAscension", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public double RightAscensionRate
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "RightAscensionRate", MemberTypes.Property));
            }
            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "RightAscensionRate", MemberTypes.Property), value);
            }
        }

        /// <inheritdoc/>
        public void SetPark()
        {
            RemoteDevice.CallNoParameters(CreateParameters(standardDeviceResponseTimeout, "SetPark", MemberTypes.Method));
            LogMessage(logger, clientNumber, "SetPark", "Park set OK");
        }

        /// <inheritdoc/>
        public PointingState SideOfPier
        {
            get
            {
                return RemoteDevice.GetValue<PointingState>(CreateParameters(standardDeviceResponseTimeout, "SideOfPier", MemberTypes.Property));
            }
            set
            {
                Dictionary<string, string> Parameters = new Dictionary<string, string>
                {
                    { AlpacaConstants.SIDEOFPIER_PARAMETER_NAME, ((int)value).ToString(CultureInfo.InvariantCulture) }
                };
                RemoteDevice.Send<NoReturnValue>(CreateParameters(longDeviceResponseTimeout, "SideOfPier", MemberTypes.Property), Parameters, HttpMethod.Put);
            }
        }

        /// <inheritdoc/>
        public double SiderealTime
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "SiderealTime", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public double SiteElevation
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "SiteElevation", MemberTypes.Property));
            }
            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "SiteElevation", MemberTypes.Property), value);
            }
        }

        /// <inheritdoc/>
        public double SiteLatitude
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "SiteLatitude", MemberTypes.Property));
            }
            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "SiteLatitude", MemberTypes.Property), value);
            }
        }

        /// <inheritdoc/>
        public double SiteLongitude
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "SiteLongitude", MemberTypes.Property));
            }
            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "SiteLongitude", MemberTypes.Property), value);
            }
        }

        /// <inheritdoc/>
        public short SlewSettleTime
        {
            get
            {
                return RemoteDevice.GetValue<short>(CreateParameters(standardDeviceResponseTimeout, "SlewSettleTime", MemberTypes.Property));
            }
            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "SlewSettleTime", MemberTypes.Property), value);
            }
        }

        /// <inheritdoc/>
        public void SlewToAltAz(double Azimuth, double Altitude)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.AZ_PARAMETER_NAME, Azimuth.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.ALT_PARAMETER_NAME, Altitude.ToString(CultureInfo.InvariantCulture) }
            };
            RemoteDevice.Send<NoReturnValue>(CreateParameters(longDeviceResponseTimeout, "SlewToAltAz", MemberTypes.Method), Parameters, HttpMethod.Put);
        }

        /// <inheritdoc/>
        public void SlewToAltAzAsync(double Azimuth, double Altitude)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.AZ_PARAMETER_NAME, Azimuth.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.ALT_PARAMETER_NAME, Altitude.ToString(CultureInfo.InvariantCulture) }
            };
            RemoteDevice.Send<NoReturnValue>(CreateParameters(longDeviceResponseTimeout, "SlewToAltAzAsync", MemberTypes.Method), Parameters, HttpMethod.Put);
        }

        /// <inheritdoc/>
        public void SlewToCoordinates(double RightAscension, double Declination)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.RA_PARAMETER_NAME, RightAscension.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.DEC_PARAMETER_NAME, Declination.ToString(CultureInfo.InvariantCulture) }
            };
            RemoteDevice.Send<NoReturnValue>(CreateParameters(longDeviceResponseTimeout, "SlewToCoordinates", MemberTypes.Method), Parameters, HttpMethod.Put);
        }

        /// <inheritdoc/>
        public void SlewToCoordinatesAsync(double RightAscension, double Declination)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.RA_PARAMETER_NAME, RightAscension.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.DEC_PARAMETER_NAME, Declination.ToString(CultureInfo.InvariantCulture) }
            };
            RemoteDevice.Send<NoReturnValue>(CreateParameters(longDeviceResponseTimeout, "SlewToCoordinatesAsync", MemberTypes.Method), Parameters, HttpMethod.Put);
        }

        /// <inheritdoc/>
        public void SlewToTarget()
        {
            RemoteDevice.CallNoParameters(CreateParameters(longDeviceResponseTimeout, "SlewToTarget", MemberTypes.Method));
            LogMessage(logger, clientNumber, "SlewToTarget", "Slew completed OK");
        }

        /// <inheritdoc/>
        public void SlewToTargetAsync()
        {
            RemoteDevice.CallNoParameters(CreateParameters(longDeviceResponseTimeout, "SlewToTargetAsync", MemberTypes.Method));
            LogMessage(logger, clientNumber, "SlewToTargetAsync", "Slew completed OK");
        }

        /// <inheritdoc/>
        public bool Slewing
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "Slewing", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public void SyncToAltAz(double Azimuth, double Altitude)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.AZ_PARAMETER_NAME, Azimuth.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.ALT_PARAMETER_NAME, Altitude.ToString(CultureInfo.InvariantCulture) }
            };
            RemoteDevice.Send<NoReturnValue>(CreateParameters(standardDeviceResponseTimeout, "SyncToAltAz", MemberTypes.Method), Parameters, HttpMethod.Put);
        }

        /// <inheritdoc/>
        public void SyncToCoordinates(double RightAscension, double Declination)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.RA_PARAMETER_NAME, RightAscension.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.DEC_PARAMETER_NAME, Declination.ToString(CultureInfo.InvariantCulture) }
            };
            RemoteDevice.Send<NoReturnValue>(CreateParameters(standardDeviceResponseTimeout, "SyncToCoordinates", MemberTypes.Method), Parameters, HttpMethod.Put);
        }

        /// <inheritdoc/>
        public void SyncToTarget()
        {
            RemoteDevice.CallNoParameters(CreateParameters(standardDeviceResponseTimeout, "SyncToTarget", MemberTypes.Method));
            LogMessage(logger, clientNumber, "SyncToTarget", "Slew completed OK");
        }

        /// <inheritdoc/>
        public double TargetDeclination
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "TargetDeclination", MemberTypes.Property));
            }
            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "TargetDeclination", MemberTypes.Property), value);
            }
        }

        /// <inheritdoc/>
        public double TargetRightAscension
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "TargetRightAscension", MemberTypes.Property));
            }
            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "TargetRightAscension", MemberTypes.Property), value);
            }
        }

        /// <inheritdoc/>
        public bool Tracking
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "Tracking", MemberTypes.Property));
            }
            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "Tracking", MemberTypes.Property), value);
            }
        }

        /// <inheritdoc/>
        public DriveRate TrackingRate
        {
            get
            {
                return RemoteDevice.GetValue<DriveRate>(CreateParameters(standardDeviceResponseTimeout, "TrackingRate", MemberTypes.Property));
            }
            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "TrackingRate", MemberTypes.Property), (int)value);
            }
        }

        /// <inheritdoc/>
        public ITrackingRates TrackingRates
        {
            get
            {
                return RemoteDevice.GetValue<ITrackingRates>(CreateParameters(standardDeviceResponseTimeout, "TrackingRates", MemberTypes.Property));
            }
        }

        /// <inheritdoc/>
        public DateTime UTCDate
        {
            get
            {
                var p = CreateParameters(standardDeviceResponseTimeout, "UTCDate", MemberTypes.Property);
                p.ThrowOnBadDateTimeJson = throwOnBadDateTimeJSON;
                return RemoteDevice.GetValue<DateTime>(p);
            }
            set
            {
                Dictionary<string, string> Parameters = new Dictionary<string, string>();
                string utcDateString = value.ToString(AlpacaConstants.ISO8601_DATE_FORMAT_STRING) + "Z";
                Parameters.Add(AlpacaConstants.UTCDATE_PARAMETER_NAME, utcDateString);
                LogMessage(logger, clientNumber, "UTCDate", "Sending date string: " + utcDateString);
                RemoteDevice.Send<NoReturnValue>(CreateParameters(standardDeviceResponseTimeout, "UTCDate", MemberTypes.Property), Parameters, HttpMethod.Put);
            }
        }

        /// <inheritdoc/>
        public void Unpark()
        {
            RemoteDevice.CallNoParameters(CreateParameters(longDeviceResponseTimeout, "UnPark", MemberTypes.Method));
            LogMessage(logger, clientNumber, "UnPark", "Unparked OK");
        }

        #endregion

        #region ITelescopeV4 implementation

        // No new members

        #endregion

    }
}
