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
    /// ASCOM Alpaca ObservingConditions client
    /// </summary>
    public class AlpacaObservingConditions : AlpacaDeviceBaseClass, IObservingConditions
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
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Trust user generated SSL certificates: {trustUserGeneratedSslCertificates}");

                DynamicClientDriver.ConnectToRemoteDevice(ref client, serviceType, ipAddressString, portNumber, clientNumber, clientDeviceType, standardDeviceResponseTimeout, userName, password, ImageArrayCompression.None, 
                    logger, userAgentProductName, userAgentProductVersion, trustUserGeneratedSslCertificates);
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), "Completed initialisation");
            }
            catch (Exception ex)
            {
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), ex.ToString());
            }
        }

        #endregion

        #region IObservingConditions Implementation

        /// <summary>
        /// Provides the time since the sensor value was last updated
        /// </summary>
        /// <param name="PropertyName">Name of the property whose time since last update is required</param>
        /// <returns>Time in seconds since the last sensor update for this property</returns>
        /// <exception cref="NotImplementedException">If the sensor is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid property name parameter is supplied.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must Not throw a NotImplementedException when the specified sensor Is implemented but must throw a NotImplementedException when the specified sensor Is Not implemented.</b></p>
        /// <para>PropertyName must be the name of one of the sensor properties specified in the <see cref="IObservingConditions"/> interface. If the caller supplies some other value, throw an InvalidValueException.</para>
        /// <para>Return a negative value to indicate that no valid value has ever been received from the hardware.</para>
        /// <para>If an empty string is supplied as the PropertyName, the driver must return the time since the most recent update of any sensor. A NotImplementedException must not be thrown in this circumstance.</para>
        /// </remarks>
        public double TimeSinceLastUpdate(string PropertyName)
        {
            return DynamicClientDriver.GetStringIndexedDouble(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "TimeSinceLastUpdate", PropertyName, MemberTypes.Method);
        }

        /// <summary>
        /// Provides a description of the sensor providing the requested property
        /// </summary>
        /// <param name="PropertyName">Name of the sensor whose description is required</param>
        /// <returns>The description of the specified sensor.</returns>
        /// <exception cref="NotImplementedException">If the sensor is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid property name parameter is supplied.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must Not throw a NotImplementedException when the specified sensor Is implemented 
        /// but must throw a NotImplementedException when the specified sensor Is Not implemented.</b></p>
        /// <para>PropertyName must be the name of one of the sensor properties specified in the <see cref="IObservingConditions"/> interface. If the caller supplies some other value, throw an InvalidValueException.</para>
        /// <para>If the sensor is implemented, this must return a valid string, even if the driver is not connected, so that applications can use this to determine what sensors are available.</para>
        /// <para>If the sensor is not implemented, this must throw a NotImplementedException.</para>
        /// </remarks>
        public string SensorDescription(string PropertyName)
        {
            return DynamicClientDriver.GetStringIndexedString(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SensorDescription", PropertyName, MemberTypes.Method);
        }

        /// <summary>
        /// Forces the driver to immediately query its attached hardware to refresh sensor values
        /// </summary>
        /// <exception cref="NotImplementedException">If this method is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Optional method, can throw a NotImplementedException</b></p>
        /// </remarks>
        public void Refresh()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Refresh", MemberTypes.Method);
        }

        /// <summary>
        /// Gets And sets the time period over which observations will be averaged
        /// </summary>
        /// <value>Time period (hours) over which to average sensor readings</value>
        /// <exception cref="InvalidValueException">If the value set is not available for this driver. All drivers must accept 0.0 to specify that an instantaneous value is available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Mandatory property, must be implemented, can NOT throw a NotImplementedException</b></p>
        /// <para>This property should return the time period (hours) over which sensor readings will be averaged. If your driver is delivering instantaneous sensor readings this property should return a value of 0.0.</para>
        /// <para>Please resist the temptation to throw exceptions when clients query sensor properties when insufficient time has passed to get a true average reading. 
        /// A best estimate of the average sensor value should be returned in these situations. </para> 
        /// </remarks>
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

        /// <summary>
        /// Amount of sky obscured by cloud
        /// </summary>
        /// <value>percentage of the sky covered by cloud</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException</b></p>
        /// This property should return a value between 0.0 and 100.0 where 0.0 = clear sky and 100.0 = 100% cloud coverage
        /// </remarks>
        public double CloudCover
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CloudCover", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Atmospheric dew point at the observatory
        /// </summary>
        /// <value>Atmospheric dew point reported in °C.</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException when the <see cref="Humidity"/> property also throws a NotImplementedException.</b></p>
        /// <p style="color:red"><b>Mandatory property, must NOT throw a NotImplementedException when the <see cref="Humidity"/> property is implemented.</b></p>
        /// <para>The units of this property are degrees Celsius. Driver and application authors can use the ASCOM.Tools.Utilities.ConvertUnits method to convert these units to and from degrees Fahrenheit.</para>
        /// <para>The ASCOM specification requires that DewPoint and Humidity are either both implemented or both throw NotImplementedExceptions. It is not allowed for 
        /// one to be implemented and the other to throw a NotImplementedException. The ASCOM.Tools.Utilities component contains methods DewPoint2Humidity and 
        /// Humidity2DewPoint to convert DewPoint to Humidity and vice versa given the ambient temperature.</para>
        /// </remarks>
        public double DewPoint
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "DewPoint", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Atmospheric humidity at the observatory
        /// </summary>
        /// <value>Atmospheric humidity (%)</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException when the <see cref="DewPoint"/> property also throws a NotImplementedException.</b></p>
        /// <p style="color:red"><b>Mandatory property, must NOT throw a NotImplementedException when the <see cref="DewPoint"/> property is implemented.</b></p>
        /// <para>The ASCOM specification requires that DewPoint and Humidity are either both implemented or both throw NotImplementedExceptions. It is not allowed for 
        /// one to be implemented and the other to throw a NotImplementedException. The ASCOM.Tools.Utilities component contains methods DewPoint2Humidity and 
        /// Humidity2DewPoint to convert DewPoint to Humidity and vice versa given the ambient temperature.</para>
        /// <para>This property should return a value between 0.0 and 100.0 where 0.0 = 0% relative humidity and 100.0 = 100% relative humidity.</para>
        /// </remarks>
        public double Humidity
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Humidity", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Atmospheric pressure at the observatory
        /// </summary>
        /// <value>Atmospheric pressure at the observatory (hPa)</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException</b></p>
        /// <para>The units of this property are hectoPascals. Client and driver authors can use the method ASCOM.Tools.Utilities.ConvertUnits
        /// to convert these units to and from milliBar, mm of mercury and inches of mercury.</para>
        /// <para>This must be the pressure at the observatory altitude and not the adjusted pressure at sea level.
        /// Please check whether your pressure sensor delivers local observatory pressure or sea level pressure and, if it returns sea level pressure, 
        /// adjust this to actual pressure at the observatory's altitude before returning a value to the client.
        /// The ASCOM.Tools.Utilities.ConvertPressure method can be used to effect this adjustment.
        /// </para>
        /// </remarks>
        public double Pressure
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Pressure", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Rain rate at the observatory
        /// </summary>
        /// <value>Rain rate (mm / hour)</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException</b></p>
        /// <para>The units of this property are millimetres per hour. Client and driver authors can use the methodASCOM.Tools.Utilities.ConvertUnits
        /// to convert these units to and from inches per hour.</para>
        /// <para>This property can be interpreted as 0.0 = Dry any positive non-zero value = wet.</para>
        /// <para>Rainfall intensity is classified according to the rate of precipitation:</para>
        /// <list type="bullet">
        /// <item><description>Light rain — when the precipitation rate is less than 2.5 mm (0.098 in) per hour</description></item>
        /// <item><description>Moderate rain — when the precipitation rate is between 2.5 mm (0.098 in) and 10 mm (0.39 in) per hour</description></item>
        /// <item><description>Heavy rain — when the precipitation rate is between 10 mm (0.39 in) and 50 mm (2.0 in) per hour</description></item>
        /// <item><description>Violent rain — when the precipitation rate is &gt; 50 mm (2.0 in) per hour</description></item>
        /// </list>
        /// </remarks>
        public double RainRate
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "RainRate", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Sky brightness at the observatory
        /// </summary>
        /// <value>Sky brightness (Lux)</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException</b></p>
        /// This property returns the sky brightness measured in Lux.
        /// <para>Luminance Examples in Lux</para>
        /// <list type="table">
        /// <listheader>
        /// <term>Illuminance</term><term>Surfaces illuminated by:</term>
        /// </listheader>
        /// <item><description>0.0001 lux</description><description>Moonless, overcast night sky (starlight)</description></item>
        /// <item><description>0.002 lux</description><description>Moonless clear night sky with airglow</description></item>
        /// <item><description>0.27–1.0 lux</description><description>Full moon on a clear night</description></item>
        /// <item><description>3.4 lux</description><description>Dark limit of civil twilight under a clear sky</description></item>
        /// <item><description>50 lux</description><description>Family living room lights (Australia, 1998)</description></item>
        /// <item><description>80 lux</description><description>Office building hallway/toilet lighting</description></item>
        /// <item><description>100 lux</description><description>Very dark overcast day</description></item>
        /// <item><description>320–500 lux</description><description>Office lighting</description></item>
        /// <item><description>400 lux</description><description>Sunrise or sunset on a clear day.</description></item>
        /// <item><description>1000 lux</description><description>Overcast day; typical TV studio lighting</description></item>
        /// <item><description>10000–25000 lux</description><description>Full daylight (not direct sun)</description></item>
        /// <item><description>32000–100000 lux</description><description>Direct sunlight</description></item>
        /// </list>
        /// </remarks>
        public double SkyBrightness
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SkyBrightness", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Sky quality at the observatory
        /// </summary>
        /// <value>Sky quality measured in magnitudes per square arc second</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException</b></p>
        /// <para>Sky quality is typically measured in units of magnitudes per square arc second. A sky quality of 20 magnitudes per square arc second means that the
        /// overall sky appears with a brightness equivalent to having 1 magnitude 20 star in each square arc second of sky.</para>
        /// <para >Examples of typical sky quality values were published by Sky and Telescope (<a href="http://www.skyandtelescope.com/astronomy-resources/rate-your-skyglow/">http://www.skyandtelescope.com/astronomy-resources/rate-your-skyglow/</a>) and, in slightly adapted form, are reproduced below:</para>
        /// <para>
        /// <table style="width:80.0%;" cellspacing="0" width="80.0%">
        /// <col style="width: 20.0%;"></col>
        /// <col style="width: 80.0%;"></col>
        /// <tr>
        /// <td colspan="1" rowspan="1" style="width: 20.0%; padding-right: 10px; padding-left: 10px; 
        /// text-align: center;vertical-align: middle;
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-top-color: #000000; border-top-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid;
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; 
        /// background-color: #00ffff;" width="10.0">
        /// <b>Sky Quality (mag/arcsec<sup>2</sup>)</b></td>
        /// <td colspan="1" rowspan="1" style="width: 80.0%; padding-right: 10px; padding-left: 10px; 
        /// text-align: center;vertical-align: middle;
        /// border-top-color: #000000; border-top-style: Solid; 
        /// border-right-style: Solid; border-right-color: #000000; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; 
        /// background-color: #00ffff;" width="90.0">
        /// <b>Description</b></td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// text-align: center;vertical-align: middle;
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 22.0</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// By convention, this is often assumed to be the average brightness of a moonless night sky that's completely free of artificial light pollution.</td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// text-align: center;vertical-align: middle;
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 21.0</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// This is typical for a rural area with a medium-sized city not far away. It's comparable to the glow of the brightest section of the northern Milky Way, from Cygnus through Perseus. </td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// text-align: center;vertical-align: middle;
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 20.0</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// This is typical for the outer suburbs of a major metropolis. The summer Milky Way is readily visible but severely washed out.</td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// text-align: center;vertical-align: middle;
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 19.0</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Typical for a suburb with widely spaced single-family homes. It's a little brighter than a remote rural site at the end of nautical twilight, when the Sun is 12° below the horizon.</td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// text-align: center;vertical-align: middle;
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 18.0</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Bright suburb or dark urban neighborhood. It's also a typical zenith skyglow at a rural site when the Moon is full. The Milky Way is invisible, or nearly so.</td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// text-align: center;vertical-align: middle;
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 17.0</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Typical near the center of a major city.</td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// text-align: center;vertical-align: middle;
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 13.0</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// The zenith skyglow at the end of civil twilight, roughly a half hour after sunset, when the Sun is 6° below the horizon. Venus and Jupiter are easy to see, but bright stars are just beginning to appear.</td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// text-align: center;vertical-align: middle;
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 7.0</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// The zenith skyglow at sunrise or sunset</td>
        /// </tr>
        /// </table>
        /// </para>
        /// </remarks>
        public double SkyQuality
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SkyQuality", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Seeing at the observatory measured as star full width half maximum (FWHM) in arc secs.
        /// </summary>
        /// <value>Seeing reported as star full width half maximum (arc seconds)</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException</b></p>
        /// </remarks>
        public double StarFWHM
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "StarFWHM", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Sky temperature at the observatory
        /// </summary>
        /// <value>Sky temperature in °C</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException</b></p>
        /// <para>The units of this property are degrees Celsius. Driver and application authors can use the ASCOM.Tools.Utilities.ConvertUnits method
        /// to convert these units to and from degrees Fahrenheit.</para>
        /// <para>This is expected to be returned by an infra-red sensor looking at the sky. The lower the temperature the more the sky is likely to be clear.</para>
        /// </remarks>
        public double SkyTemperature
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SkyTemperature", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Temperature at the observatory
        /// </summary>
        /// <value>Temperature in °C</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException</b></p>
        /// <para>The units of this property are degrees Celsius. Driver and application authors can use the ASCOM.Tools.Utilities.ConvertUnits method
        /// to convert these units to and from degrees Fahrenheit.</para>
        /// <para>This is expected to be the ambient temperature at the observatory.</para>
        /// </remarks>
        public double Temperature
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Temperature", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Wind direction at the observatory
        /// </summary>
        /// <value>Wind direction (degrees, 0..360.0)</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException</b></p>
        /// The returned value must be between 0.0 and 360.0, interpreted according to the meteorological standard, where a special value of 0.0 is returned when the wind speed is 0.0. 
        /// Wind direction is measured clockwise from north, through east, where East=90.0, South=180.0, West=270.0 and North=360.0.
        /// </remarks>
        public double WindDirection
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "WindDirection", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Peak 3 second wind gust at the observatory over the last 2 minutes
        /// </summary>
        /// <value>Wind gust (m/s) Peak 3 second wind speed over the last 2 minutes</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException</b></p>
        /// The units of this property are metres per second. Driver and application authors can use the ASCOM.Tools.Utilities.ConvertUnits method
        /// to convert these units to and from miles per hour or knots.
        /// </remarks>
        public double WindGust
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "WindGust", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Wind speed at the observatory
        /// </summary>
        /// <value>Wind speed (m/s)</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException</b></p>
        /// The units of this property are metres per second. Driver and application authors can use the ASCOM.Tools.Utilities.ConvertUnits method
        /// to convert these units to and from miles per hour or knots.
        /// </remarks>
        public double WindSpeed
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "WindSpeed", MemberTypes.Property);
            }
        }

        #endregion

    }
}