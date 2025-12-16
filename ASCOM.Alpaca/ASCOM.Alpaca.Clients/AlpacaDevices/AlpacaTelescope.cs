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
        /// <param name="request100Continue"></param>Request 100-continue behaviour for HTTP requests. Defaults to false.
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
            this.throwOnBadDateTimeJSON= throwOnBadDateTimeJSON;
            this.request100Continue = request100Continue;

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
            clientNumber = DynamicClientDriver.GetUniqueClientNumber();
            Initialise();
        }
        private void Initialise()
        {
            try
            {
                // Set the device type
                clientDeviceType = DeviceTypes.Telescope;

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
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "AbortSlew", MemberTypes.Method);
            LogMessage(logger, clientNumber, "AbortSlew", $"Slew aborted OK.");
        }

        /// <inheritdoc/>
        public AlignmentMode AlignmentMode
        {
            get
            {
                return DynamicClientDriver.GetValue<AlignmentMode>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "AlignmentMode", MemberTypes.Property);
            }
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
        public double ApertureArea
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "ApertureArea", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double ApertureDiameter
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "ApertureDiameter", MemberTypes.Property);
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
        public IAxisRates AxisRates(TelescopeAxis Axis)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.AXIS_PARAMETER_NAME, ((int)Axis).ToString(CultureInfo.InvariantCulture) }
            };
            return DynamicClientDriver.SendToRemoteDevice<IAxisRates>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "AxisRates", Parameters, HttpMethod.Get, MemberTypes.Method);
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
        public bool CanMoveAxis(TelescopeAxis Axis)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.AXIS_PARAMETER_NAME, ((int)Axis).ToString(CultureInfo.InvariantCulture) }
            };
            return DynamicClientDriver.SendToRemoteDevice<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanMoveAxis", Parameters, HttpMethod.Get, MemberTypes.Method);
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
        public bool CanPulseGuide
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanPulseGuide", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool CanSetDeclinationRate
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSetDeclinationRate", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool CanSetGuideRates
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSetGuideRates", MemberTypes.Property);
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
        public bool CanSetPierSide
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSetPierSide", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool CanSetRightAscensionRate
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSetRightAscensionRate", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool CanSetTracking
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSetTracking", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool CanSlew
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSlew", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool CanSlewAltAz
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSlewAltAz", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool CanSlewAltAzAsync
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSlewAltAzAsync", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool CanSlewAsync
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSlewAsync", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool CanSync
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSync", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool CanSyncAltAz
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSyncAltAz", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool CanUnpark
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanUnpark", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double Declination
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Declination", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double DeclinationRate
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "DeclinationRate", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetDouble(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "DeclinationRate", value, MemberTypes.Property);
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
            return DynamicClientDriver.SendToRemoteDevice<PointingState>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "DestinationSideOfPier", Parameters, HttpMethod.Get, MemberTypes.Method);
        }

        /// <inheritdoc/>
        public bool DoesRefraction
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "DoesRefraction", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetBool(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "DoesRefraction", value, MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public EquatorialCoordinateType EquatorialSystem
        {
            get
            {
                return DynamicClientDriver.GetValue<EquatorialCoordinateType>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "EquatorialSystem", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public void FindHome()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "FindHome", MemberTypes.Method);
            LogMessage(logger, clientNumber, "FindHome", "Home found OK");
        }

        /// <inheritdoc/>
        public double FocalLength
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "FocalLength", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double GuideRateDeclination
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "GuideRateDeclination", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetDouble(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "GuideRateDeclination", value, MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double GuideRateRightAscension
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "GuideRateRightAscension", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetDouble(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "GuideRateRightAscension", value, MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool IsPulseGuiding
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "IsPulseGuiding", MemberTypes.Property);
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
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "MoveAxis", Parameters, HttpMethod.Put, MemberTypes.Method);
        }

        /// <inheritdoc/>
        public void Park()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "Park", MemberTypes.Method);
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
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "PulseGuide", Parameters, HttpMethod.Put, MemberTypes.Method);
        }

        /// <inheritdoc/>
        public double RightAscension
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "RightAscension", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double RightAscensionRate
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "RightAscensionRate", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetDouble(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "RightAscensionRate", value, MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public void SetPark()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SetPark", MemberTypes.Method);
            LogMessage(logger, clientNumber, "SetPark", "Park set OK");
        }

        /// <inheritdoc/>
        public PointingState SideOfPier
        {
            get
            {
                return DynamicClientDriver.GetValue<PointingState>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SideOfPier", MemberTypes.Property);
            }
            set
            {
                Dictionary<string, string> Parameters = new Dictionary<string, string>
                {
                    { AlpacaConstants.SIDEOFPIER_PARAMETER_NAME, ((int)value).ToString(CultureInfo.InvariantCulture) }
                };
                DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "SideOfPier", Parameters, HttpMethod.Put, MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double SiderealTime
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SiderealTime", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double SiteElevation
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SiteElevation", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetDouble(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SiteElevation", value, MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double SiteLatitude
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SiteLatitude", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetDouble(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SiteLatitude", value, MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double SiteLongitude
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SiteLongitude", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetDouble(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SiteLongitude", value, MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public short SlewSettleTime
        {
            get
            {
                return DynamicClientDriver.GetValue<short>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SlewSettleTime", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetShort(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SlewSettleTime", value, MemberTypes.Property);
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
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "SlewToAltAz", Parameters, HttpMethod.Put, MemberTypes.Method);
        }

        /// <inheritdoc/>
        public void SlewToAltAzAsync(double Azimuth, double Altitude)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.AZ_PARAMETER_NAME, Azimuth.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.ALT_PARAMETER_NAME, Altitude.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "SlewToAltAzAsync", Parameters, HttpMethod.Put, MemberTypes.Method);
        }

        /// <inheritdoc/>
        public void SlewToCoordinates(double RightAscension, double Declination)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.RA_PARAMETER_NAME, RightAscension.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.DEC_PARAMETER_NAME, Declination.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "SlewToCoordinates", Parameters, HttpMethod.Put, MemberTypes.Method);
        }

        /// <inheritdoc/>
        public void SlewToCoordinatesAsync(double RightAscension, double Declination)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.RA_PARAMETER_NAME, RightAscension.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.DEC_PARAMETER_NAME, Declination.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "SlewToCoordinatesAsync", Parameters, HttpMethod.Put, MemberTypes.Method);
        }

        /// <inheritdoc/>
        public void SlewToTarget()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "SlewToTarget", MemberTypes.Method);
            LogMessage(logger, clientNumber, "SlewToTarget", "Slew completed OK");
        }

        /// <inheritdoc/>
        public void SlewToTargetAsync()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "SlewToTargetAsync", MemberTypes.Method);
            LogMessage(logger, clientNumber, "SlewToTargetAsync", "Slew completed OK");
        }

        /// <inheritdoc/>
        public bool Slewing
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Slewing", MemberTypes.Property);
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
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SyncToAltAz", Parameters, HttpMethod.Put, MemberTypes.Method);
        }

        /// <inheritdoc/>
        public void SyncToCoordinates(double RightAscension, double Declination)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.RA_PARAMETER_NAME, RightAscension.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.DEC_PARAMETER_NAME, Declination.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SyncToCoordinates", Parameters, HttpMethod.Put, MemberTypes.Method);
        }

        /// <inheritdoc/>
        public void SyncToTarget()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SyncToTarget", MemberTypes.Method);
            LogMessage(logger, clientNumber, "SyncToTarget", "Slew completed OK");
        }

        /// <inheritdoc/>
        public double TargetDeclination
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "TargetDeclination", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetDouble(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "TargetDeclination", value, MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public double TargetRightAscension
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "TargetRightAscension", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetDouble(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "TargetRightAscension", value, MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public bool Tracking
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Tracking", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetBool(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Tracking", value, MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public DriveRate TrackingRate
        {
            get
            {
                return DynamicClientDriver.GetValue<DriveRate>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "TrackingRate", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetInt(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "TrackingRate", (int)value, MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public ITrackingRates TrackingRates
        {
            get
            {
                return DynamicClientDriver.GetValue<ITrackingRates>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "TrackingRates", MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public DateTime UTCDate
        {
            get
            {
                return DynamicClientDriver.GetValue<DateTime>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "UTCDate", MemberTypes.Property, throwOnBadDateTimeJSON);
            }
            set
            {
                Dictionary<string, string> Parameters = new Dictionary<string, string>();
                string utcDateString = value.ToString(AlpacaConstants.ISO8601_DATE_FORMAT_STRING) + "Z";
                Parameters.Add(AlpacaConstants.UTCDATE_PARAMETER_NAME, utcDateString);
                LogMessage(logger, clientNumber, "UTCDate", "Sending date string: " + utcDateString);
                DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "UTCDate", Parameters, HttpMethod.Put, MemberTypes.Property);
            }
        }

        /// <inheritdoc/>
        public void Unpark()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "UnPark", MemberTypes.Method);
            LogMessage(logger, clientNumber, "UnPark", "Unparked OK");
        }

        #endregion

        #region ITelescopeV4 implementation

        // No new members

        #endregion

    }
}
