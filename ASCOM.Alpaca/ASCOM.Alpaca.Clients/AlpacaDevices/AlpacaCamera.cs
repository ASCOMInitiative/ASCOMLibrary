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
    /// ASCOM Alpaca Camera client
    /// </summary>
    public class AlpacaCamera : AlpacaDeviceBaseClass, ICameraV4
    {
        #region Initialiser

        /// <summary>
        /// Create a client for an Alpaca Camera device with all parameters set to default values
        /// </summary>
        public AlpacaCamera()
        {
            Initialise();
        }

        /// <summary>
        /// Create a client for an Alpaca Camera device specifying all parameters
        /// </summary>
        /// <param name="serviceType">HTTP or HTTPS</param>
        /// <param name="ipAddressString">Alpaca device's IP Address</param>
        /// <param name="portNumber">Alpaca device's IP Port number</param>
        /// <param name="remoteDeviceNumber">Alpaca device's device number e.g. Telescope/0</param>
        /// <param name="establishConnectionTimeout">Timeout (seconds) to initially connect to the Alpaca device</param>
        /// <param name="standardDeviceResponseTimeout">Timeout (seconds) for transactions that are expected to complete quickly e.g. retrieving CanXXX properties</param>
        /// <param name="longDeviceResponseTimeout">Timeout (seconds) for transactions that are expected to take a long time to complete e.g. Camera.ImageArray</param>
        /// <param name="clientNumber">Arbitrary integer that represents this client. (Should be the same for all transactions from this client)</param>
        /// <param name="imageArrayTransferType">Specifies the method to be used to retrieve the ImageArray property value.</param>
        /// <param name="imageArrayCompression">Extent to which the ImageArray data stream should be compressed.</param>
        /// <param name="userName">Basic authentication user name for the Alpaca device</param>
        /// <param name="password">basic authentication password for the Alpaca device</param>
        /// <param name="strictCasing">Tolerate or throw exceptions  if the Alpaca device does not use strictly correct casing for JSON object element names.</param>
        /// <param name="logger">Optional ILogger instance that can be sued to record operational information during execution</param>
        /// <param name="userAgentProductName">Optional product name to include in the User-Agent HTTP header sent to the Alpaca device</param>
        /// <param name="userAgentProductVersion">Optional product version to include in the User-Agent HTTP header sent to the Alpaca device</param>
        /// <param name="trustUserGeneratedSslCertificates">Trust user generated SSL certificates</param>
        public AlpacaCamera(ServiceType serviceType = AlpacaClient.CLIENT_SERVICETYPE_DEFAULT,
                            string ipAddressString = AlpacaClient.CLIENT_IPADDRESS_DEFAULT,
                            int portNumber = AlpacaClient.CLIENT_IPPORT_DEFAULT,
                            int remoteDeviceNumber = AlpacaClient.CLIENT_REMOTEDEVICENUMBER_DEFAULT,
                            int establishConnectionTimeout = AlpacaClient.CLIENT_ESTABLISHCONNECTIONTIMEOUT_DEFAULT,
                            int standardDeviceResponseTimeout = AlpacaClient.CLIENT_STANDARDCONNECTIONTIMEOUT_DEFAULT,
                            int longDeviceResponseTimeout = AlpacaClient.CLIENT_LONGCONNECTIONTIMEOUT_DEFAULT,
                            uint clientNumber = AlpacaClient.CLIENT_CLIENTNUMBER_DEFAULT,
                            ImageArrayTransferType imageArrayTransferType = AlpacaClient.CLIENT_IMAGEARRAYTRANSFERTYPE_DEFAULT,
                            ImageArrayCompression imageArrayCompression = AlpacaClient.CLIENT_IMAGEARRAYCOMPRESSION_DEFAULT,
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
            this.imageArrayTransferType = imageArrayTransferType;
            this.imageArrayCompression = imageArrayCompression;
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
        /// Create a client for an Alpaca Camera device specifying the minimum number of parameters
        /// </summary>
        /// <param name="serviceType">HTTP or HTTPS</param>
        /// <param name="ipAddressString">Alpaca device's IP Address</param>
        /// <param name="portNumber">Alpaca device's IP Port number</param>
        /// <param name="remoteDeviceNumber">Alpaca device's device number e.g. Telescope/0</param>
        /// <param name="strictCasing">Tolerate or throw exceptions  if the Alpaca device does not use strictly correct casing for JSON object element names.</param>
        /// <param name="logger">Optional ILogger instance that can be sued to record operational information during execution</param>
        public AlpacaCamera(ServiceType serviceType,
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
                clientDeviceType = Common.DeviceTypes.Camera;

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
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"ImageArray transfer type: {imageArrayTransferType}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"ImageArray compression: {imageArrayCompression}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"User name is Null or Empty: {string.IsNullOrEmpty(userName)}, User name is Null or White Space: {string.IsNullOrWhiteSpace(userName)}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"User name length: {password.Length}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Password is Null or Empty: {string.IsNullOrEmpty(password)}, Password is Null or White Space: {string.IsNullOrWhiteSpace(password)}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Password length: {password.Length}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Strict casing: {strictCasing}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Trust user generated SSL certificates: {trustUserGeneratedSslCertificates}");

                DynamicClientDriver.CreateHttpClient(ref client, serviceType, ipAddressString, portNumber, clientNumber, clientDeviceType, userName, password, imageArrayCompression,
                    logger, userAgentProductName, userAgentProductVersion, trustUserGeneratedSslCertificates);
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), "Completed initialisation");
            }
            catch (Exception ex)
            {
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), ex.ToString());
            }
        }

        /// <summary>
        /// Configure the device and establish an HTTP connection
        /// </summary>
        /// <param name="serviceType">HTTP or HTTPS</param>
        /// <param name="ipAddressString">Alpaca device's IP Address</param>
        /// <param name="portNumber">Alpaca device's IP Port number</param>
        /// <param name="remoteDeviceNumber">Alpaca device's device number e.g. Telescope/0</param>
        /// <param name="establishConnectionTimeout">Timeout (seconds) to initially connect to the Alpaca device</param>
        /// <param name="standardDeviceResponseTimeout">Timeout (seconds) for transactions that are expected to complete quickly e.g. retrieving CanXXX properties</param>
        /// <param name="longDeviceResponseTimeout">Timeout (seconds) for transactions that are expected to take a long time to complete e.g. Camera.ImageArray</param>
        /// <param name="clientNumber">Arbitrary integer that represents this client. (Should be the same for all transactions from this client)</param>
        /// <param name="imageArrayTransferType">Specifies the method to be used to retrieve the ImageArray property value.</param>
        /// <param name="imageArrayCompression">Extent to which the ImageArray data stream should be compressed.</param>
        /// <param name="userName">Basic authentication user name for the Alpaca device</param>
        /// <param name="password">basic authentication password for the Alpaca device</param>
        /// <param name="strictCasing">Tolerate or throw exceptions  if the Alpaca device does not use strictly correct casing for JSON object element names.</param>
        /// <param name="logger">Optional ILogger instance that can be sued to record operational information during execution</param>
        public void ConfigureAndConnect(ServiceType serviceType,
                          string ipAddressString,
                          int portNumber,
                          int remoteDeviceNumber,
                          int establishConnectionTimeout,
                          int standardDeviceResponseTimeout,
                          int longDeviceResponseTimeout,
                          uint clientNumber,
                          ImageArrayTransferType imageArrayTransferType,
                          ImageArrayCompression imageArrayCompression,
                          string userName,
                          string password,
                          bool strictCasing,
                          ILogger logger
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
            this.imageArrayTransferType = imageArrayTransferType;
            this.imageArrayCompression = imageArrayCompression;
            this.userName = userName;
            this.password = password;
            this.strictCasing = strictCasing;
            this.logger = logger;

            Initialise();
        }

        #endregion

        #region Client configuration properties

        /// <summary>
        /// Alpaca device IP address as a string
        /// </summary>
        public ImageArrayTransferType ImageArrayTransferType
        {
            get { return imageArrayTransferType; }
            set { imageArrayTransferType = value; }
        }

        /// <summary>
        /// Alpaca device IP address as a string
        /// </summary>
        public ImageArrayCompression ImageArrayCompression
        {
            get { return imageArrayCompression; }
            set { imageArrayCompression = value; }
        }

        #endregion

        #region Convenience members

        /// <summary>
        /// Camera device state
        /// </summary>
        public CameraDeviceState CameraDeviceState
        {
            get
            {
                // Create a state object to return.
                CameraDeviceState cameraDeviceState = new CameraDeviceState(DeviceState, logger);
                logger.LogMessage(LogLevel.Debug, "CameraDeviceState", $"Returning: '{cameraDeviceState.CameraState}' '{cameraDeviceState.CCDTemperature}' '{cameraDeviceState.CoolerPower}' '{cameraDeviceState.HeatSinkTemperature}' '{cameraDeviceState.ImageReady}' '{cameraDeviceState.PercentCompleted}' '{cameraDeviceState.TimeStamp}'");

                // Return the device specific state class
                return cameraDeviceState;
            }
        }

        #endregion

        #region ICameraV2 Implementation

        /// <summary>
        /// Aborts the current exposure, if any, and returns the camera to Idle state.
        /// </summary>
        /// <remarks>
        /// <p style="color:red"><b>May throw a not implemented exception if CanAbortExpsoure is false.</b></p>
        /// <para><b>NOTES:</b>
        /// <list type="bullet">
        /// <item><description>Must throw exception if camera is not idle and abort is unsuccessful (or not possible, e.g. during download).</description></item>
        /// <item><description>Must throw exception if hardware or communications error occurs.</description></item>
        /// <item><description>Must NOT throw an exception if the camera is already idle.</description></item>
        /// </list> </para>
        /// </remarks>
        /// <exception cref="NotImplementedException">If CanAbortExposure is false.</exception>
        /// <exception cref="InvalidOperationException">Thrown if abort is not currently possible (e.g. during download).</exception>
        /// <exception cref="NotConnectedException">Thrown if the driver is not connected.</exception>
        /// <exception cref="DriverException">Thrown if a communications error occurs, or if the abort fails.</exception>
        public void AbortExposure()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "AbortExposure", MemberTypes.Method);
        }

        /// <summary>
        /// Activates the Camera's mount control system to instruct the mount to move in a particular direction for a given period of time
        /// </summary>
        /// <remarks>
        /// <p style="color:red"><b>May throw a not implemented exception if this camera does not support PulseGuide</b></p>
        /// <para>This method must be implemented asynchronously using <see cref="IsPulseGuiding"/>  as the completion property.</para>
        /// <para>
        /// The (symbolic) values for GuideDirections are:
        /// <list type="bullet">
        /// <listheader><description>Constant     Value      Description</description></listheader>
        /// <item><description>guideNorth     0        North (+ declination/elevation)</description></item>
        /// <item><description>guideSouth     1        South (- declination/elevation)</description></item>
        /// <item><description>guideEast      2        East (+ right ascension/azimuth)</description></item>
        /// <item><description>guideWest      3        West (+ right ascension/azimuth)</description></item>
        /// </list>
        /// </para>
        /// <para>Note: directions are nominal and may depend on exact mount wiring.
        /// <see cref="GuideDirection.North" /> must be opposite <see cref="GuideDirection.South" />, and 
        /// <see cref="GuideDirection.East" /> must be opposite <see cref="GuideDirection.West" />.</para>
        /// </remarks>
        /// <param name="Direction">The direction of movement.</param>
        /// <param name="Duration">The duration of movement in milli-seconds.</param>
        /// <exception cref="NotImplementedException">PulseGuide command is unsupported</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public void PulseGuide(GuideDirection Direction, int Duration)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.DIRECTION_PARAMETER_NAME, ((int)Direction).ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.DURATION_PARAMETER_NAME, Duration.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "PulseGuide", Parameters, HttpMethod.Put, MemberTypes.Method);
        }

        /// <summary>
        /// Starts an exposure. Use <see cref="ImageReady" /> to check when the exposure is complete.
        /// </summary>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// <para>Must be implemented asynchronously using <see cref="ImageReady"/> to determine if the exposure has been successfully completed and the image data is ready for access via <see cref="ImageArray"/>.</para>
        /// <para>A dark frame or bias exposure may be shorter than the V2 <see cref="ExposureMin" /> value and for a bias frame can be zero.
        /// Check the value of <see cref="StartExposure">Light</see> and allow exposures down to 0 seconds
        /// if <see cref="StartExposure">Light</see> is <c>false</c>.  If the hardware will not
        /// support an exposure duration of zero then, for dark and bias frames, set it to the minimum that is possible.</para>
        /// <para>Some applications will set an exposure time of zero for bias frames so it's important that the driver allows this.</para>
        /// </remarks>
        /// <param name="Duration">Duration of exposure in seconds, can be zero if <see cref="StartExposure">Light</see> is <c>false</c></param>
        /// <param name="Light"><c>true</c> for light frame, <c>false</c> for dark frame (ignored if no shutter)</param>
        /// <exception cref=" InvalidValueException"><see cref="NumX" />, <see cref="NumY" />, <see cref="BinX" />,
        /// <see cref="BinY" />, <see cref="StartX" />, <see cref="StartY" />, or <see cref="StartExposure">Duration</see> parameters are invalid.</exception>
        /// <exception cref=" InvalidOperationException"><see cref="CanAsymmetricBin" /> is <c>false</c> and <see cref="BinX" /> != <see cref="BinY" /></exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public void StartExposure(double Duration, bool Light)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.DURATION_PARAMETER_NAME, Duration.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.LIGHT_PARAMETER_NAME, Light.ToString() }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "StartExposure", Parameters, HttpMethod.Put, MemberTypes.Method);
        }

        /// <summary>
        /// Stops the current exposure, if any.
        /// </summary>
        /// <remarks>
        /// <p style="color:red"><b>May throw a not implemented exception</b></p>
        /// <para>Must be implemented asynchronously using <see cref="ImageReady"/> to determine if the exposure has been successfully completed and the image data is ready for access via <see cref="ImageArray"/>.</para>
        /// <para>If an exposure is in progress, the readout process is initiated.  Ignored if readout is already in process.</para>
        /// </remarks>
        /// <exception cref="NotImplementedException">Must throw an exception if CanStopExposure is <c>false</c></exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public void StopExposure()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "StopExposure", MemberTypes.Method);
        }

        /// <summary>
        /// Sets the binning factor for the X axis, also returns the current value.
        /// </summary>
        /// <remarks>
        /// Should default to 1 when the camera connection is established.  Note:  driver does not check
        /// for compatible subframe values when this value is set; rather they are checked upon <see cref="StartExposure">StartExposure</see>.
        /// </remarks>
        /// <value>The X binning value</value>
        /// <exception cref="InvalidValueException">Must throw an exception for illegal binning values</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public short BinX
        {
            get
            {
                return DynamicClientDriver.GetValue<short>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "BinX", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetShort(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "BinX", value, MemberTypes.Property);
            }
        }

        /// <summary>
        /// Sets the binning factor for the Y axis, also returns the current value.
        /// </summary>
        /// <remarks>
        /// Should default to 1 when the camera connection is established.  Note:  driver does not check
        /// for compatible subframe values when this value is set; rather they are checked upon <see cref="StartExposure">StartExposure</see>.
        /// </remarks>
        /// <value>The Y binning value.</value>
        /// <exception cref="InvalidValueException">Must throw an exception for illegal binning values</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public short BinY
        {
            get
            {
                return DynamicClientDriver.GetValue<short>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "BinY", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetShort(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "BinY", value, MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns the current camera operational state
        /// </summary>
        /// <remarks>
        /// Returns one of the following status information:
        /// <list type="bullet">
        /// <listheader><description>Value  State           Meaning</description></listheader>
        /// <item><description>0      CameraIdle      At idle state, available to start exposure</description></item>
        /// <item><description>1      CameraWaiting   Exposure started but waiting (for shutter, trigger, filter wheel, etc.)</description></item>
        /// <item><description>2      CameraExposing  Exposure currently in progress</description></item>
        /// <item><description>3      CameraReading   CCD array is being read out (digitized)</description></item>
        /// <item><description>4      CameraDownload  Downloading data to PC</description></item>
        /// <item><description>5      CameraError     Camera error condition serious enough to prevent further operations (connection fail, etc.).</description></item>
        /// </list>
        /// </remarks>
        /// <value>The state of the camera.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public CameraState CameraState
        {
            get
            {
                return DynamicClientDriver.GetValue<CameraState>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CameraState", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns the width of the CCD camera chip in unbinned pixels.
        /// </summary>
        /// <value>The size of the camera X.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public int CameraXSize
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CameraXSize", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns the height of the CCD camera chip in unbinned pixels.
        /// </summary>
        /// <value>The size of the camera Y.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public int CameraYSize
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CameraYSize", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns <c>true</c> if the camera can abort exposures; <c>false</c> if not.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can abort exposure; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// </remarks>
        public bool CanAbortExposure
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanAbortExposure", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns a flag showing whether this camera supports asymmetric binning
        /// </summary>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// <para>If <c>true</c>, the camera can have different binning on the X and Y axes, as
        /// determined by <see cref="BinX" /> and <see cref="BinY" />. If <c>false</c>, the binning must be equal on the X and Y axes.</para>
        /// </remarks>
        /// <value>
        /// <c>true</c> if this instance can asymmetric bin; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public bool CanAsymmetricBin
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanAsymmetricBin", MemberTypes.Property);
            }
        }

        /// <summary>
        /// If <c>true</c>, the camera's cooler power setting can be read.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can get cooler power; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// </remarks>
        public bool CanGetCoolerPower
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanGetCoolerPower", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns a flag indicating whether this camera supports pulse guiding
        /// </summary>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// <para>Returns <c>true</c> if the camera can send auto-guider pulses to the telescope mount; <c>false</c> if not.
        /// Note: this does not provide any indication of whether the auto-guider cable is actually connected.</para>
        /// </remarks>
        /// <value>
        /// <c>true</c> if this instance can pulse guide; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public bool CanPulseGuide
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanPulseGuide", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns a flag indicating whether this camera supports setting the CCD temperature
        /// </summary>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// <para>If <c>true</c>, the camera's cooler set-point can be adjusted. If <c>false</c>, the camera
        /// either uses open-loop cooling or does not have the ability to adjust temperature
        /// from software, and setting the <see cref="SetCCDTemperature" /> property has no effect.</para>
        /// </remarks>
        /// <value>
        /// <c>true</c> if this instance can set CCD temperature; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public bool CanSetCCDTemperature
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSetCCDTemperature", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns a flag indicating whether this camera can stop an exposure that is in progress
        /// </summary>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// <para>Some cameras support <see cref="StopExposure" />, which allows the exposure to be terminated
        /// before the exposure timer completes, but will still read out the image.  Returns
        /// <c>true</c> if <see cref="StopExposure" /> is available, <c>false</c> if not.</para>
        /// </remarks>
        /// <value>
        /// <c>true</c> if the camera can stop the exposure; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public bool CanStopExposure
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanStopExposure", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns the current CCD temperature in degrees Celsius.
        /// </summary>
        /// <value>The CCD temperature.</value>
        /// <exception cref="InvalidValueException">Must throw exception if data unavailable.</exception>
        /// <exception cref="NotImplementedException">Must throw exception if not supported.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public double CCDTemperature
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CCDTemperature", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Turns on and off the camera cooler, and returns the current on/off state.
        /// </summary>
        /// <remarks>
        /// <b>Warning:</b> turning the cooler off when the cooler is operating at high delta-T
        /// (typically &gt;20C below ambient) may result in thermal shock.  Repeated thermal
        /// shock may lead to damage to the sensor or cooler stack.  Please consult the
        /// documentation supplied with the camera for further information.
        /// </remarks>
        /// <value><c>true</c> if the cooler is on; otherwise, <c>false</c>.</value>
        /// <exception cref=" NotImplementedException">not supported</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public bool CoolerOn
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CoolerOn", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetBool(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CoolerOn", value, MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns the present cooler power level, in percent.
        /// </summary>
        /// <remarks>
        /// Returns zero if <see cref="CoolerOn" /> is <c>false</c>.
        /// </remarks>
        /// <value>The cooler power.</value>
        /// <exception cref=" NotImplementedException">not supported</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public double CoolerPower
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CoolerPower", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns the gain of the camera in photoelectrons per A/D unit.
        /// </summary>
        /// <remarks>
        /// Some cameras have multiple gain modes; these should be selected via the configuration dialogue and thus are
        /// static during a session.
        /// </remarks>
        /// <value>The electrons per ADU.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public double ElectronsPerADU
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "ElectronsPerADU", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Reports the full well capacity of the camera in electrons, at the current camera settings (binning, SetupDialog settings, etc.)
        /// </summary>
        /// <value>The full well capacity.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public double FullWellCapacity
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "FullWellCapacity", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns a flag indicating whether this camera has a mechanical shutter
        /// </summary>
        /// <remarks>
        /// If <c>true</c>, the camera has a mechanical shutter. If <c>false</c>, the camera does not have
        /// a shutter.  If there is no shutter, the  <see cref="StartExposure">StartExposure</see> command will ignore the
        /// Light parameter.
        /// </remarks>
        /// <value>
        /// <c>true</c> if this instance has shutter; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public bool HasShutter
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "HasShutter", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns the current heat sink temperature (called "ambient temperature" by some manufacturers) in degrees Celsius.
        /// </summary>
        /// <remarks>
        /// Only valid if  <see cref="CanSetCCDTemperature" /> is <c>true</c>.
        /// </remarks>
        /// <value>The heat sink temperature.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public double HeatSinkTemperature
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "HeatSinkTemperature", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns an array of int of size <see cref="NumX" /> * <see cref="NumY" /> containing the pixel values from the last exposure.
        /// </summary>
        /// <remarks>
        /// <para>This is a synchronous call and clients should be prepared for it to take a long time to complete when large images are being transferred.</para>
        /// <para>Drivers written in C++ must return the image as a SafeArray.</para>
        /// <para>Developers of Alpaca camera devices are strongly advised to implement the ImageBytes mechanic, which is specified in the Alpaca API Reference, to ensure fast image transfer to the client.</para>>
        /// The application must inspect the Safearray parameters to determine the dimensions.
        /// <para>Note: if <see cref="NumX" /> or <see cref="NumY" /> is changed after a call to <see cref="StartExposure">StartExposure</see> it will
        /// have no effect on the size of this array. This is the preferred method for programs (not scripts) to download
        /// images since it requires much less memory.</para>
        /// <para>For colour or multispectral cameras, will produce an array of  <see cref="NumX" /> * <see cref="NumY" /> *
        /// NumPlanes.  If the application cannot handle multispectral images, it should use just the first plane.</para>
        /// </remarks>
        /// <value>The image array.</value>
        /// <exception cref="InvalidOperationException">If no image data is available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public object ImageArray
        {
            get
            {
                return DynamicClientDriver.GetValue<Array>(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "ImageArray", imageArrayTransferType, imageArrayCompression, MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns an array of COM-Variant of size <see cref="NumX" /> * <see cref="NumY" /> containing the pixel values from the last exposure.
        /// </summary>
        /// <value>The image array variant.</value>
        /// <remarks>
        /// <para>This property is used only in Windows ASCOM/COM drivers. Alpaca drivers and stand-alone Alpaca cameras must raise a <see cref="PropertyNotImplementedException"/>.</para>
        /// <para>This is a synchronous call and clients should be prepared for it to take a long time to complete when large images are being transferred.</para>
        /// <para>Drivers written in C++ must return the image as a SafeArray.</para>
        /// <para>Note: if <see cref="NumX" /> or <see cref="NumY" /> is changed after a call to
        /// <see cref="StartExposure">StartExposure</see> it will have no effect on the size of this array. This property
        /// should only be used from scripts due to the extremely high memory utilization on
        /// large image arrays (26 bytes per pixel). Pixels values should be in Short, int,
        /// or Double format.</para>
        /// <para>For colour or multispectral cameras, will produce an array of <see cref="NumX" /> * <see cref="NumY" /> *
        /// NumPlanes.  If the application cannot handle multispectral images, it should use
        /// just the first plane.</para>
        /// </remarks>
        /// <value>The image array variant.</value>
        /// <exception cref="InvalidOperationException">If no image data is available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public object ImageArrayVariant
        {
            get
            {
                return DynamicClientDriver.ImageArrayVariant(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, imageArrayTransferType, imageArrayCompression);
            }
        }

        /// <summary>
        /// Returns a flag indicating whether the image is ready to be downloaded from the camera
        /// </summary>
        /// <remarks>
        /// If <c>true</c>, there is an image from the camera available. If <c>false</c>, no image
        /// is available and attempts to use the <see cref="ImageArray" /> method will produce an exception
        /// </remarks>.
        /// <value><c>true</c> if [image ready]; otherwise, <c>false</c>.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public bool ImageReady
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "ImageReady", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns a flag indicating whether the camera is currently in a <see cref="PulseGuide">PulseGuide</see> operation.
        /// </summary>
        /// <remarks>
        /// If <c>true</c>, pulse guiding is in progress. Required if the <see cref="PulseGuide">PulseGuide</see> method
        /// (which is non-blocking) is implemented. See the <see cref="PulseGuide">PulseGuide</see> method.
        /// </remarks>
        /// <value>
        /// <c>true</c> if this instance is pulse guiding; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public bool IsPulseGuiding
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "IsPulseGuiding", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Reports the actual exposure duration in seconds (i.e. shutter open time).
        /// </summary>
        /// <remarks>
        /// This may differ from the exposure time requested due to shutter latency, camera timing precision, etc.
        /// </remarks>
        /// <value>The last duration of the exposure.</value>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="InvalidOperationException">If called before any exposure has been taken</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public double LastExposureDuration
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "LastExposureDuration", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Reports the actual exposure start in the FITS-standard CCYY-MM-DDThh:mm:ss[.sss...] format.
        /// The start time must be UTC.
        /// </summary>
        /// <value>The last exposure start time in UTC.</value>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="InvalidOperationException">If called before any exposure has been taken</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public string LastExposureStartTime
        {
            get
            {
                return DynamicClientDriver.GetValue<string>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "LastExposureStartTime", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Reports the maximum ADU value the camera can produce.
        /// </summary>
        /// <value>The maximum ADU.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public int MaxADU
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "MaxADU", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns the maximum allowed binning for the X camera axis
        /// </summary>
        /// <remarks>
        /// If <see cref="CanAsymmetricBin" /> = <c>false</c>, returns the maximum allowed binning factor. If
        /// <see cref="CanAsymmetricBin" /> = <c>true</c>, returns the maximum allowed binning factor for the X axis.
        /// </remarks>
        /// <value>The max bin X.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public short MaxBinX
        {
            get
            {
                return DynamicClientDriver.GetValue<short>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "MaxBinX", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns the maximum allowed binning for the Y camera axis
        /// </summary>
        /// <remarks>
        /// If <see cref="CanAsymmetricBin" /> = <c>false</c>, equals <see cref="MaxBinX" />. If <see cref="CanAsymmetricBin" /> = <c>true</c>,
        /// returns the maximum allowed binning factor for the Y axis.
        /// </remarks>
        /// <value>The max bin Y.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public short MaxBinY
        {
            get
            {
                return DynamicClientDriver.GetValue<short>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "MaxBinY", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Sets the subframe width. Also returns the current value.
        /// </summary>
        /// <remarks>
        /// If binning is active, value is in binned pixels.  No error check is performed when the value is set.
        /// Should default to <see cref="CameraXSize" />.
        /// </remarks>
        /// <value>The num X.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public int NumX
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "NumX", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetInt(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "NumX", value, MemberTypes.Property);
            }
        }

        /// <summary>
        /// Sets the subframe height. Also returns the current value.
        /// </summary>
        /// <remarks>
        /// If binning is active,
        /// value is in binned pixels.  No error check is performed when the value is set.
        /// Should default to <see cref="CameraYSize" />.
        /// </remarks>
        /// <value>The num Y.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public int NumY
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "NumY", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetInt(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "NumY", value, MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns the width of the CCD chip pixels in microns.
        /// </summary>
        /// <value>The pixel size X.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public double PixelSizeX
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "PixelSizeX", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns the height of the CCD chip pixels in microns.
        /// </summary>
        /// <value>The pixel size Y.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public double PixelSizeY
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "PixelSizeY", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Sets the camera cooler set-point in degrees Celsius, and returns the current set-point.
        /// </summary>
        /// <remarks>
        /// <para>The driver should throw an <see cref="InvalidValueException" /> if an attempt is made to set <see cref="SetCCDTemperature" />
        /// outside the valid range for the camera. As an assistance to driver authors, to protect equipment and prevent harm to individuals,
        /// Conform will report an issue if it is possible to set <see cref="SetCCDTemperature" /> below -280C or above +100C.</para>
        /// <b>Note:</b>  Camera hardware and/or driver should perform cooler ramping, to prevent
        /// thermal shock and potential damage to the CCD array or cooler stack.
        /// </remarks>
        /// <value>The set CCD temperature.</value>
        /// <exception cref="InvalidValueException">Must throw an InvalidValueException if an attempt is made to set a value is outside the 
        /// camera's valid temperature setpoint range.</exception>
        /// <exception cref="NotImplementedException">Must throw exception if <see cref="CanSetCCDTemperature" /> is <c>false</c>.</exception>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public double SetCCDTemperature
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SetCCDTemperature", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetDouble(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SetCCDTemperature", value, MemberTypes.Property);
            }
        }

        /// <summary>
        /// Sets the subframe start position for the X axis (0 based) and returns the current value.
        /// </summary>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// <para>If binning is active, value is in binned pixels.</para>
        /// </remarks>
        /// <value>The start X.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public int StartX
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "StartX", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetInt(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "StartX", value, MemberTypes.Property);
            }
        }

        /// <summary>
        /// Sets the subframe start position for the Y axis (0 based). Also returns the current value.
        /// </summary>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// <para>If binning is active, value is in binned pixels.</para>
        /// </remarks>
        /// <value>The start Y.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public int StartY
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "StartY", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetInt(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "StartY", value, MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns the X offset of the Bayer matrix, as defined in <see cref="SensorType" />.
        /// </summary>
        /// <returns>The Bayer colour matrix X offset, as defined in <see cref="SensorType" />.</returns>
        /// <exception cref="NotImplementedException">Monochrome cameras must throw this exception, colour cameras must not.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Must be implemented by colour cameras, monochrome cameras must throw a NotImplementedException</b></p>
        /// <para>Since monochrome cameras don't have a Bayer colour matrix by definition, such cameras should throw a <see cref="NotImplementedException" />.
        /// Colour cameras should always return a value and must not throw a <see cref="NotImplementedException" /></para>
        /// <para>The value returned must be in the range 0 to M-1 where M is the width of the Bayer matrix. The offset is relative to the 0,0 pixel in
        /// the sensor array, and does not change to reflect subframe settings.</para>
        /// <para>It is recommended that this function be called only after a <see cref="IAscomDevice.Connected">connection</see> is established with 
        /// the camera hardware, to ensure that the driver is aware of the capabilities of the specific camera model.</para>
        /// <para>This is only available for the Camera Interface Version 2 and later.</para>
        /// </remarks>
        public short BayerOffsetX
        {
            get
            {
                return DynamicClientDriver.GetValue<short>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "BayerOffsetX", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns the Y offset of the Bayer matrix, as defined in <see cref="SensorType" />.
        /// </summary>
        /// <returns>The Bayer colour matrix Y offset, as defined in <see cref="SensorType" />.</returns>
        /// <exception cref="NotImplementedException">Monochrome cameras must throw this exception, colour cameras must not.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Must be implemented by colour cameras, monochrome cameras must throw a NotImplementedException</b></p>
        /// <para>Since monochrome cameras don't have a Bayer colour matrix by definition, such cameras should throw a <see cref="NotImplementedException" />.
        /// Colour cameras should always return a value and must not throw a <see cref="NotImplementedException" /></para>
        /// <para>The value returned must be in the range 0 to M-1 where M is the width of the Bayer matrix. The offset is relative to the 0,0 pixel in
        /// the sensor array, and does not change to reflect subframe settings.</para>
        /// <para>It is recommended that this function be called only after a <see cref="IAscomDevice.Connected">connection</see> is established with 
        /// the camera hardware, to ensure that the driver is aware of the capabilities of the specific camera model.</para>
        /// <para>This is only available for the Camera Interface Version 2 and later.</para>
        /// </remarks>
        public short BayerOffsetY
        {
            get
            {
                return DynamicClientDriver.GetValue<short>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "BayerOffsetY", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Camera has a fast readout mode
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <returns><c>true</c> when the camera supports a fast readout mode</returns>
        /// <remarks><p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// It is recommended that this function be called only after a <see cref="IAscomDevice.Connected">connection</see> is established with the camera hardware, to 
        /// ensure that the driver is aware of the capabilities of the specific camera model.
        /// <para>This is only available for the Camera Interface Version 2 and later.</para>
        /// </remarks>
        public bool CanFastReadout
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanFastReadout", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns the maximum exposure time supported by <see cref="StartExposure">StartExposure</see>.
        /// </summary>
        /// <returns>The maximum exposure time, in seconds, that the camera supports</returns>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// It is recommended that this function be called only after
        /// a <see cref="IAscomDevice.Connected">connection</see> is established with the camera hardware, to ensure that the driver is aware of the capabilities of the 
        /// specific camera model.
        /// <para>This is only available for the Camera Interface Version 2 and later.</para>
        /// </remarks>
        public double ExposureMax
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "ExposureMax", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Minimum exposure time
        /// </summary>
        /// <returns>The minimum exposure time, in seconds, that the camera supports through <see cref="StartExposure">StartExposure</see></returns>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// This must be a non-zero number representing the shortest possible exposure time supported by the camera model.
        /// <para>Please note that for bias frame acquisition an even shorter exposure may be possible; please see <see cref="StartExposure">StartExposure</see>
        /// for more information.</para>
        /// <para>It is recommended that this function be called only after a <see cref="IAscomDevice.Connected">connection</see> is established with the camera hardware, to ensure 
        /// that the driver is aware of the capabilities of the specific camera model.</para>
        /// <para>This is only available for the Camera Interface Version 2 and later.</para>
        /// </remarks>
        public double ExposureMin
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "ExposureMin", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Exposure resolution
        /// </summary>
        /// <returns>The smallest increment in exposure time supported by <see cref="StartExposure">StartExposure</see>.</returns>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// This can be used, for example, to specify the resolution of a user interface "spin control" used to dial in the exposure time.
        /// <para>Please note that the Duration provided to <see cref="StartExposure">StartExposure</see> does not have to be an exact multiple of this number;
        /// the driver should choose the closest available value. Also in some cases the resolution may not be constant over the full range
        /// of exposure times; in this case the smallest increment would be appropriate. A value of 0.0 shall indicate that there is no minimum resolution
        /// except that imposed by the resolution of the double value itself.</para>
        /// <para>It is recommended that this function be called only after a <see cref="IAscomDevice.Connected">connection</see> is established with the camera hardware, to ensure 
        /// that the driver is aware of the capabilities of the specific camera model.</para>
        /// <para>This is only available for the Camera Interface Version 2 and later.</para>
        /// </remarks>
        public double ExposureResolution
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "ExposureResolution", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Gets or sets Fast Readout Mode
        /// </summary>
        /// <value><c>true</c> for fast readout mode, <c>false</c> for normal mode</value>
        /// <exception cref="NotImplementedException">Thrown if <see cref="CanFastReadout" /> is <c>false</c>.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Must throw a NotImplementedException if CanFastReadout is false or
        /// return a boolean value if CanFastReadout is true.</b></p>
        /// Must thrown an exception if no <see cref="IAscomDevice.Connected">connection</see> is established to the camera. Must throw
        /// a <see cref="NotImplementedException" /> if <see cref="CanFastReadout" /> returns <c>false</c>.
        /// <para>Many cameras have a "fast mode" intended for use in focusing. When set to <c>true</c>, the camera will operate in Fast mode; when
        /// set <c>false</c>, the camera will operate normally. This property, if implemented, should default to <c>False</c>.</para>
        /// <para>Please note that this function may in some cases interact with <see cref="ReadoutModes" />; for example, there may be modes where
        /// the Fast/Normal switch is meaningless. In this case, it may be preferable to use the <see cref="ReadoutModes" /> function to control
        /// fast/normal switching.</para>
        /// <para>If this feature is not available, then <see cref="CanFastReadout" /> must return <c>false</c>.</para>
        /// <para>This is only available for the Camera Interface Version 2 and later.</para>
        /// </remarks>
        public bool FastReadout
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "FastReadout", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetBool(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "FastReadout", value, MemberTypes.Property);
            }
        }

        /// <summary>
        /// The camera's gain (GAIN VALUE MODE) OR the index of the selected camera gain description in the <see cref="Gains" /> array (GAINS INDEX MODE)
        /// </summary>
        /// <returns><para><b> GAIN VALUE MODE:</b> The current gain value.</para>
        /// <p style="color:red"><b>OR</b></p>
        /// <b>GAINS INDEX MODE:</b> Index into the Gains array for the current camera gain
        /// </returns>
        /// <exception cref="NotImplementedException">When neither <b>GAINS INDEX</b> mode nor <b>GAIN VALUE</b> mode are supported.</exception>
        /// <exception cref="InvalidValueException">When the supplied value is not valid.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>This is an optional property and can throw a NotImplementedException if Gain is not supported by the camera.</b></p>
        /// The <see cref="Gain" /> property is used to adjust the gain setting of the camera and has <b>two modes of operation</b>:
        /// <ul>
        /// <li><b>GAIN VALUE MODE</b> - The <see cref="Gain" /> property is a direct numeric representation of the camera's gain.
        /// <ul>
        /// <li>In this mode the <see cref="GainMin" /> and <see cref="GainMax" /> properties must return integers specifying the valid range for <see cref="Gain" /></li>
        /// <li>The <see cref="Gains"/> property must return a <see cref="NotImplementedException"/>.</li>
        /// </ul>
        /// </li>
        /// <li><b>GAINS INDEX MODE</b> - The <see cref="Gain" /> property is the selected gain's index within the <see cref="Gains"/> array of textual gain descriptions.
        /// <ul>
        /// <li>In this  mode the <see cref="Gains" /> method returns a 0-based array of strings, which describe available gain settings e.g. "ISO 200", "ISO 1600" </li>
        /// <li><see cref="GainMin" /> and <see cref="GainMax" /> must throw <see cref="NotImplementedException"/>s.</li>
        /// <li>Please note that the <see cref="Gains"/> array is zero based.</li>
        /// </ul>
        /// </li>
        /// </ul>
        /// <para>A driver can support none, one or both gain modes depending on the camera's capabilities. However, only one mode can be active at any one moment because both modes share
        /// the <see cref="Gain"/> property to return the gain value. Client applications can determine which mode is operational by reading the <see cref="GainMin"/>, <see cref="GainMax"/> and 
        /// <see cref="Gain"/> properties. If a property can be read then its associated mode is active, if it throws a <see cref="NotImplementedException"/> then the mode is not active.</para>
        /// <para>If a driver supports both modes the astronomer must be able to select the required mode through the driver Setup dialogue.</para>
        /// <para>During driver initialisation the driver must set <see cref="Gain" /> to a valid value.</para>
        /// <para>Please note that <see cref="ReadoutMode" /> may in some cases affect the gain of the camera; if so, the driver must be ensure that the two properties do not conflict if both are used.</para>
        /// <para>This is only available in Camera Interface Version 2 and later.</para>
        /// </remarks>
        public short Gain
        {
            get
            {
                return DynamicClientDriver.GetValue<short>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Gain", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetShort(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Gain", value, MemberTypes.Property);
            }
        }

        /// <summary>
        /// Maximum <see cref="Gain" /> value of that this camera supports
        /// </summary>
        /// <returns>The maximum gain value that this camera supports</returns>
        /// <exception cref="NotImplementedException">When the <see cref="Gain"/> property is not implemented or is operating in <b>GAINS INDEX</b> mode.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>This is an optional property and can throw a NotImplementedException.</b></p>
        /// When <see cref="Gain"/> is operating in <b><see cref="Gain">GAIN VALUE</see></b> mode:
        /// <ul>
        /// <li><see cref="GainMax" /> must return the camera's highest valid <see cref="Gain" /> setting.</li>
        /// <li><see cref="GainMax" /> must be equal to or greater than <see cref="GainMin" />.</li>
        /// <li><see cref="Gains"/> must throw a <see cref="NotImplementedException"/></li>
        /// </ul>
        /// <para>Please note that <see cref="GainMin"/> and <see cref="GainMax"/> act together and that either both must be implemented or both must throw <see cref="NotImplementedException"/>s.</para>
        /// <para>It is recommended that this function be called only after a connection is established with the camera hardware to ensure that the driver is aware of the capabilities of the specific camera model.</para>
        /// <para>This property is only available in Camera Interface Version 2 and later.</para>
        /// </remarks>
        public short GainMax
        {
            get
            {
                return DynamicClientDriver.GetValue<short>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "GainMax", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Minimum <see cref="Gain" /> value of that this camera supports
        /// </summary>
        /// <returns>The minimum gain value that this camera supports</returns>
        /// <exception cref="NotImplementedException">When the <see cref="Gain"/> property is not implemented or is operating in <b>GAINS INDEX</b> mode.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>This is an optional property and can throw a NotImplementedException.</b></p>
        /// When <see cref="Gain"/> is operating in <b><see cref="Gain">GAIN VALUE</see></b> mode:
        /// <ul>
        /// <li><see cref="GainMin" /> must return the camera's lowest valid <see cref="Gain" /> setting.</li>
        /// <li><see cref="GainMin" /> must be less than or equal to <see cref="GainMax" />.</li>
        /// <li><see cref="Gains"/> must throw a <see cref="NotImplementedException"/></li>
        /// </ul>
        /// <para>Please note that <see cref="GainMin"/> and <see cref="GainMax"/> act together and that either both must be implemented or both must throw <see cref="NotImplementedException"/>s.</para>
        /// <para>It is recommended that this function be called only after a connection is established with the camera hardware to ensure that the driver is aware of the capabilities of the specific camera model.</para>
        /// <para>This property is only available in Camera Interface Version 2 and later.</para>
        /// </remarks>
        public short GainMin
        {
            get
            {
                return DynamicClientDriver.GetValue<short>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "GainMin", MemberTypes.Property);
            }
        }

        /// <summary>
        /// List of Gain names supported by the camera
        /// </summary>
        /// <returns>The list of supported gain names as an ArrayList of strings</returns>
        /// <exception cref="NotImplementedException">When the <see cref="Gain"/> property is not implemented or is operating in <b>GAIN VALUE</b> mode.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>This is an optional property and can throw a NotImplementedException.</b></p>
        /// When <see cref="Gain"/> is operating in <b><see cref="Gain">GAINS INDEX</see></b> mode:
        /// <ul>
        /// <li>The <see cref="Gains" /> property must return a zero-based ArrayList of available gain setting names.</li>
        /// <li>The <see cref="GainMin"/> and <see cref="GainMax"/> properties must throw <see cref="NotImplementedException"/>s.</li>
        /// </ul>
        /// <para>The returned gain names could, for example, be a list of ISO settings for a DSLR camera or a list of gain names for a CMOS camera.
        /// Typically the application software will display the returned gain names in a drop list, from which the astronomer can select the required value.
        /// The application can then configure the required gain by setting the camera's <see cref="Gain"/> property to the array index of the selected description.</para>
        /// <para>It is recommended that this function be called only after a connection is established with the camera hardware to ensure that the driver is aware of the capabilities of the specific camera model.</para>
        /// <para>This is only available in Camera Interface Version 2 and later.</para>
        /// </remarks>
        public IList<string> Gains
        {
            get
            {
                List<string> gains = DynamicClientDriver.GetValue<List<string>>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Gains", MemberTypes.Property);
                LogMessage(logger, clientNumber, "Gains", $"Returning {gains.Count} gains");

                return gains;
            }
        }

        /// <summary>
        /// Percent completed, Interface Version 2 only
        /// </summary>
        /// <returns>A value between 0 and 100% indicating the completeness of this operation</returns>
        /// <exception cref="InvalidOperationException">Thrown when it is inappropriate to call <see cref="PercentCompleted" /></exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>May throw a NotImplementedException if PercentCompleted is not supported by the camera.</b></p>
        /// If valid, returns an integer between 0 and 100, where 0 indicates 0% progress (function just started) and
        /// 100 indicates 100% progress (i.e. completion).
        /// <para>At the discretion of the driver author, <see cref="PercentCompleted" /> may optionally be valid
        /// when <see cref="CameraState" /> is in any or all of the following
        /// states: <see cref="CameraState.Exposing" />, 
        /// <see cref="CameraState.Waiting" />, <see cref="CameraState.Reading" /> 
        /// or <see cref="CameraState.Download" />. In all other states an exception shall be thrown.</para>
        /// <para>Typically the application user interface will show a progress bar based on the <see cref="PercentCompleted" /> value.</para>
        /// <para>Please note that client applications are not required to use this value, and in some cases may display status
        /// information based on other information, such as time elapsed.</para>
        /// <para>This is only available for the Camera Interface Version 2 and later.</para>
        /// </remarks>
        public short PercentCompleted
        {
            get
            {
                return DynamicClientDriver.GetValue<short>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "PercentCompleted", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Readout mode, Interface Version 2 only
        /// </summary>
        /// <value></value>
        /// <returns>Short integer index into the <see cref="ReadoutModes">ReadoutModes</see> array of string readout mode names indicating
        /// the camera's current readout mode.</returns>
        /// <exception cref="InvalidValueException">Must throw an exception if set to an illegal or unavailable mode.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Must be implemented if CanFastReadout is false, must throw a NotImplementedException if
        /// CanFastReadout is true.</b></p>
        /// <see cref="ReadoutMode" /> is an index into the array <see cref="ReadoutModes" />, and selects the desired readout mode for the camera.
        /// Defaults to 0 if not set.  Throws an exception if the selected mode is not available.
        /// <para>It is strongly recommended, but not required, that driver authors make the 0-index mode suitable for standard imaging operations,
        /// since it is the default.</para>
        /// <para>Please see <see cref="ReadoutModes" /> for additional information.</para>
        /// <para>This is only available for the Camera Interface Version 2 and later.</para>
        /// </remarks>
        public short ReadoutMode
        {
            get
            {
                return DynamicClientDriver.GetValue<short>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "ReadoutMode", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetShort(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "ReadoutMode", value, MemberTypes.Property);
            }
        }

        /// <summary>
        /// List of available readout modes, Interface Version 2 only
        /// </summary>
        /// <returns>An ArrayList of readout mode names</returns>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Must be implemented if CanFastReadout is false, must throw a NotImplementedException if
        /// CanFastReadout is true.</b></p>
        /// This property provides an array of strings, each of which describes an available readout mode of the camera.
        /// At least one string must be present in the list. The user interface of a control application will typically present to the
        /// user a drop-list of modes.  The choice of available modes made available is entirely at the discretion of the driver author.
        /// Please note that if the camera has many different modes of operation, then the most commonly adjusted settings should be in
        /// <see cref="ReadoutModes" />; additional settings may be provided using a set-up dialog.
        /// <para>To select a mode, the application will set <see cref="ReadoutMode" /> to the index of the desired mode.  The index is zero-based.</para>
        /// <para>This property should only be read while a <see cref="IAscomDevice.Connected">connection</see> to the camera is actually established.  Drivers often support
        /// multiple cameras with different capabilities, which are not known until the <see cref="IAscomDevice.Connected">connection</see> is made.  If the available readout modes
        /// are not known because no <see cref="IAscomDevice.Connected">connection</see> has been established, this property shall throw an exception.</para>
        /// <para>Please note that the default <see cref="ReadoutMode" /> setting is 0. It is strongly recommended, but not required, that
        /// driver authors use the 0-index mode for standard imaging operations, since it is the default.</para>
        /// <para>This feature may be used in parallel with <see cref="FastReadout" />; however, care should be taken to ensure that the two
        /// features work together consistently. If there are modes that are inconsistent having a separate fast/normal switch, then it
        /// may be better to simply list Fast as one of the <see cref="ReadoutModes" />.</para>
        /// <para>It is recommended that this function be called only after a <see cref="IAscomDevice.Connected">connection</see> is established with
        /// the camera hardware, to ensure that the driver is aware of the capabilities of the specific camera model.</para>
        /// <para>This is only available for the Camera Interface Version 2 and later.</para>
        /// </remarks>
        public IList<string> ReadoutModes
        {
            get
            {
                List<string> modes = DynamicClientDriver.GetValue<List<string>>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "ReadoutModes", MemberTypes.Property);
                LogMessage(logger, clientNumber, "ReadoutModes", $"Returning {modes.Count} modes");

                return modes;
            }
        }

        /// <summary>
        /// Sensor name, Interface Version 2 only
        /// ## Mandatory must return an empty string if the sensor is unknown
        /// </summary>
        /// <returns>The name of the sensor used within the camera.</returns>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>May throw a NotImplementedException if the sensor's name is not known.</b></p>
        /// <para>Returns the name (data sheet part number) of the sensor, e.g. ICX285AL.  The format is to be exactly as shown on
        /// manufacturer data sheet, subject to the following rules:
        /// <list type="bullet">
        /// <item>All letters shall be upper-case.</item>
        /// <item>Spaces shall not be included.</item>
        /// <item>Any extra suffixes that define region codes, package types, temperature range, coatings, grading, colour/monochrome,
        /// etc. shall not be included.</item>
        /// <item>For colour sensors, if a suffix differentiates different Bayer matrix encodings, it shall be included.</item>
        /// <item>The call shall return an empty string if the sensor name is not known.</item>
        /// </list>  </para>
        /// <para>Examples:</para>
        /// <list type="bullet">
        /// <item><description>ICX285AL-F shall be reported as ICX285</description></item>
        /// <item><description>KAF-8300-AXC-CD-AA shall be reported as KAF-8300</description></item>
        /// </list>
        /// <para><b>Note:</b></para>
        /// <para>The most common usage of this property is to select approximate colour balance parameters to be applied to
        /// the Bayer matrix of one-shot colour sensors.  Application authors should assume that an appropriate IR cut-off filter is
        /// in place for colour sensors.</para>
        /// <para>It is recommended that this function be called only after a <see cref="IAscomDevice.Connected">connection</see> is established with
        /// the camera hardware, to ensure that the driver is aware of the capabilities of the specific camera model.</para>
        /// <para>This is only available for the Camera Interface Version 2 and later.</para>
        /// </remarks>
        public string SensorName
        {
            get
            {
                return DynamicClientDriver.GetValue<string>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SensorName", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Type of colour information returned by the camera sensor, Interface Version 2 only
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="SensorType" /> enum value of the camera sensor</returns>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>May throw a NotImplementedException if the sensor type is not known.</b></p>
        /// <para>This is only available for the Camera Interface Version 2 and later.</para>
        /// <para><see cref="SensorType" /> returns a value indicating whether the sensor is monochrome, or what Bayer matrix it encodes. If this value
        /// cannot be determined by interrogating the camera, the appropriate value may be set through the user set-up dialogue or the property may
        /// return a <see cref="NotImplementedException" />. Please note that for some cameras, changing <see cref="BinX" />,
        /// <see cref="BinY" /> or <see cref="ReadoutMode" /> may change the apparent type of the sensor and so you should change the value returned here
        /// to match if this is the case for your camera.</para>
        /// <para>The following values are defined:</para>
        /// <para>
        /// <table style="width:76.24%;" cellspacing="0" width="76.24%">
        /// <col style="width: 11.701%;"></col>
        /// <col style="width: 20.708%;"></col>
        /// <col style="width: 67.591%;"></col>
        /// <tr>
        /// <td colspan="1" rowspan="1" style="width: 11.701%; padding-right: 10px; padding-left: 10px;
        /// border-left-color: #000000; border-left-style: Solid;
        /// border-top-color: #000000; border-top-style: Solid;
        /// border-right-color: #000000; border-right-style: Solid;
        /// border-bottom-color: #000000; border-bottom-style: Solid;
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px;
        /// background-color: #00ffff;" width="11.701%">
        /// <b>Value</b></td>
        /// <td colspan="1" rowspan="1" style="width: 20.708%; padding-right: 10px; padding-left: 10px;
        /// border-top-color: #000000; border-top-style: Solid;
        /// border-right-style: Solid; border-right-color: #000000;
        /// border-bottom-color: #000000; border-bottom-style: Solid;
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px;
        /// background-color: #00ffff;" width="20.708%">
        /// <b>Enumeration</b></td>
        /// <td colspan="1" rowspan="1" style="width: 67.591%; padding-right: 10px; padding-left: 10px;
        /// border-top-color: #000000; border-top-style: Solid;
        /// border-right-style: Solid; border-right-color: #000000;
        /// border-bottom-color: #000000; border-bottom-style: Solid;
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px;
        /// background-color: #00ffff;" width="67.591%">
        /// <b>Meaning</b></td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px;
        /// border-left-color: #000000; border-left-style: Solid;
        /// border-right-color: #000000; border-right-style: Solid;
        /// border-bottom-color: #000000; border-bottom-style: Solid;
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 0</td>
        /// <td style="padding-right: 10px; padding-left: 10px;
        /// border-right-color: #000000; border-right-style: Solid;
        /// border-bottom-color: #000000; border-bottom-style: Solid;
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Monochrome</td>
        /// <td style="padding-right: 10px; padding-left: 10px;
        /// border-right-color: #000000; border-right-style: Solid;
        /// border-bottom-color: #000000; border-bottom-style: Solid;
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Camera produces monochrome array with no Bayer encoding</td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px;
        /// border-left-color: #000000; border-left-style: Solid;
        /// border-right-color: #000000; border-right-style: Solid;
        /// border-bottom-color: #000000; border-bottom-style: Solid;
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 1</td>
        /// <td style="padding-right: 10px; padding-left: 10px;
        /// border-right-color: #000000; border-right-style: Solid;
        /// border-bottom-color: #000000; border-bottom-style: Solid;
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Colour</td>
        /// <td style="padding-right: 10px; padding-left: 10px;
        /// border-right-color: #000000; border-right-style: Solid;
        /// border-bottom-color: #000000; border-bottom-style: Solid;
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Camera produces color image directly, requiring not Bayer decoding</td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px;
        /// border-left-color: #000000; border-left-style: Solid;
        /// border-right-color: #000000; border-right-style: Solid;
        /// border-bottom-color: #000000; border-bottom-style: Solid;
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 2</td>
        /// <td style="padding-right: 10px; padding-left: 10px;
        /// border-right-color: #000000; border-right-style: Solid;
        /// border-bottom-color: #000000; border-bottom-style: Solid;
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// RGGB</td>
        /// <td style="padding-right: 10px; padding-left: 10px;
        /// border-right-color: #000000; border-right-style: Solid;
        /// border-bottom-color: #000000; border-bottom-style: Solid;
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Camera produces RGGB encoded Bayer array images</td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px;
        /// border-left-color: #000000; border-left-style: Solid;
        /// border-right-color: #000000; border-right-style: Solid;
        /// border-bottom-color: #000000; border-bottom-style: Solid;
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 3</td>
        /// <td style="padding-right: 10px; padding-left: 10px;
        /// border-right-color: #000000; border-right-style: Solid;
        /// border-bottom-color: #000000; border-bottom-style: Solid;
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// CMYG</td>
        /// <td style="padding-right: 10px; padding-left: 10px;
        /// border-right-color: #000000; border-right-style: Solid;
        /// border-bottom-color: #000000; border-bottom-style: Solid;
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Camera produces CMYG encoded Bayer array images</td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px;
        /// border-left-color: #000000; border-left-style: Solid;
        /// border-right-color: #000000; border-right-style: Solid;
        /// border-bottom-color: #000000; border-bottom-style: Solid;
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 4</td>
        /// <td style="padding-right: 10px; padding-left: 10px;
        /// border-right-color: #000000; border-right-style: Solid;
        /// border-bottom-color: #000000; border-bottom-style: Solid;
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// CMYG2</td>
        /// <td style="padding-right: 10px; padding-left: 10px;
        /// border-right-color: #000000; border-right-style: Solid;
        /// border-bottom-color: #000000; border-bottom-style: Solid;
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Camera produces CMYG2 encoded Bayer array images</td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px;
        /// border-left-color: #000000; border-left-style: Solid;
        /// border-right-color: #000000; border-right-style: Solid;
        /// border-bottom-color: #000000; border-bottom-style: Solid;
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 5</td>
        /// <td style="padding-right: 10px; padding-left: 10px;
        /// border-right-color: #000000; border-right-style: Solid;
        /// border-bottom-color: #000000; border-bottom-style: Solid;
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// LRGB</td>
        /// <td style="padding-right: 10px; padding-left: 10px;
        /// border-right-color: #000000; border-right-style: Solid;
        /// border-bottom-color: #000000; border-bottom-style: Solid;
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Camera produces Kodak TRUESENSE Bayer LRGB array images</td>
        /// </tr>
        /// </table>
        /// </para>
        /// <para>Please note that additional values may be defined in future updates of the standard, as new Bayer matrices may be created
        /// by sensor manufacturers in the future.  If this occurs, then a new enumeration value shall be defined. The pre-existing enumeration
        /// values shall not change.
        /// <para><see cref="SensorType" /> can possibly change between exposures, for example if <see cref="ReadoutMode">Camera.ReadoutMode</see> is changed, and should always be checked after each exposure.</para>
        /// <para>In the following definitions, R = red, G = green, B = blue, C = cyan, M = magenta, Y = yellow.  The Bayer matrix is
        /// defined with X increasing from left to right, and Y increasing from top to bottom. The pattern repeats every N x M pixels for the
        /// entire pixel array, where N is the height of the Bayer matrix, and M is the width.</para>
        /// <para>RGGB indicates the following matrix:</para>
        /// </para>
        /// <para>
        /// <table style="width:41.254%;" cellspacing="0" width="41.254%">
        /// <col style="width: 10%;"></col>
        /// <col style="width: 10%;"></col>
        /// <col style="width: 10%;"></col>
        /// 
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// background-color: #ffffff" width="10%">
        /// </td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// background-color: #00ffff;" width="10%">
        /// <b>X = 0</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-right-color: #000000; border-right-style: Solid; border-right-width: 1px;
        /// background-color: #00ffff;" width="10%">
        /// <b>X = 1</b></td>
        /// </tr>
        /// 
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// background-color: #00ffff" width="10%">
        /// <b>Y = 0</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// " width="10%">
        /// R</td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-top-color: #000000; border-top-style: Solid;  border-top-width: 1px;
        /// border-right-color: #000000; border-right-style: Solid; border-right-width: 1px;
        /// " width="10%">
        /// G</td>
        /// </tr>
        /// 
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;
        /// background-color: #00ffff;" width="10%">
        /// <b>Y = 1</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;
        /// " width="10%">
        /// G</td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;
        /// border-right-color: #000000; border-right-style: Solid; border-right-width: 1px;
        /// " width="10%">
        /// B</td>
        /// </tr>
        /// </table>
        /// </para>
        /// 
        /// <para>CMYG indicates the following matrix:</para>
        /// <para>
        /// <table style="width:41.254%;" cellspacing="0" width="41.254%">
        /// <col style="width: 10%;"></col>
        /// <col style="width: 10%;"></col>
        /// <col style="width: 10%;"></col>
        /// 
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// background-color: #ffffff" width="10%">
        /// </td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// background-color: #00ffff;" width="10%">
        /// <b>X = 0</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-right-color: #000000; border-right-style: Solid; border-right-width: 1px;
        /// background-color: #00ffff;" width="10%">
        /// <b>X = 1</b></td>
        /// </tr>
        /// 
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// background-color: #00ffff" width="10%">
        /// <b>Y = 0</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// " width="10%">
        /// Y</td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-top-color: #000000; border-top-style: Solid;  border-top-width: 1px;
        /// border-right-color: #000000; border-right-style: Solid; border-right-width: 1px;
        /// " width="10%">
        /// C</td>
        /// </tr>
        /// 
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;
        /// background-color: #00ffff;" width="10%">
        /// <b>Y = 1</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;
        /// " width="10%">
        /// G</td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;
        /// border-right-color: #000000; border-right-style: Solid; border-right-width: 1px;
        /// " width="10%">
        /// M</td>
        /// </tr>
        /// 
        /// </table>
        /// </para>
        /// <para>CMYG2 indicates the following matrix:</para>
        /// <para>
        /// <table style="width:41.254%;" cellspacing="0" width="41.254%">
        /// <col style="width: 10%;"></col>
        /// <col style="width: 10%;"></col>
        /// <col style="width: 10%;"></col>
        /// 
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// background-color: #ffffff" width="10%">
        /// </td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// background-color: #00ffff;" width="10%">
        /// <b>X = 0</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-right-color: #000000; border-right-style: Solid; border-right-width: 1px;
        /// background-color: #00ffff;" width="10%">
        /// <b>X = 1</b></td>
        /// </tr>
        /// 
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// background-color: #00ffff" width="10%">
        /// <b>Y = 0</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// " width="10%">
        /// C</td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-top-color: #000000; border-top-style: Solid;  border-top-width: 1px;
        /// border-right-color: #000000; border-right-style: Solid; border-right-width: 1px;
        /// " width="10%">
        /// Y</td>
        /// </tr>
        /// 
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// background-color: #00ffff;" width="10%">
        /// <b>Y = 1</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// " width="10%">
        /// M</td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-right-color: #000000; border-right-style: Solid; border-right-width: 1px;
        /// " width="10%">
        /// G</td>
        /// </tr>
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// background-color: #00ffff" width="10%">
        /// <b>Y = 2</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// " width="10%">
        /// C</td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-top-color: #000000; border-top-style: Solid;  border-top-width: 1px;
        /// border-right-color: #000000; border-right-style: Solid; border-right-width: 1px;
        /// " width="10%">
        /// Y</td>
        /// </tr>
        /// 
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;
        /// background-color: #00ffff;" width="10%">
        /// <b>Y = 3</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;
        /// " width="10%">
        /// G</td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-right-color: #000000; border-right-style: Solid; border-right-width: 1px;
        /// border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;
        /// " width="10%">
        /// M</td>
        /// </tr>
        /// </table>
        /// </para>
        /// 
        /// <para>LRGB indicates the following matrix (Kodak TRUESENSE):</para>
        /// <para>
        /// <table style="width:68.757%;" cellspacing="0" width="68.757%">
        /// <col style="width: 10%;"></col>
        /// <col style="width: 10%;"></col>
        /// <col style="width: 10%;"></col>
        /// <col style="width: 10%;"></col>
        /// <col style="width: 10%;"></col>
        /// 
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// background-color: #ffffff" width="10%">
        /// </td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// background-color: #00ffff;" width="10%">
        /// <b>X = 0</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// background-color: #00ffff;" width="10%">
        /// <b>X = 1</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// background-color: #00ffff;" width="10%">
        /// <b>X = 2</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-right-color: #000000; border-right-style: Solid; border-right-width: 1px;
        /// background-color: #00ffff;" width="10%">
        /// <b>X = 3</b></td>
        /// </tr>
        /// 
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// background-color: #00ffff" width="10%">
        /// <b>Y = 0</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// " width="10%">
        /// L</td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// " width="10%">
        /// R</td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// " width="10%">
        /// L</td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-top-color: #000000; border-top-style: Solid;  border-top-width: 1px;
        /// border-right-color: #000000; border-right-style: Solid; border-right-width: 1px;
        /// " width="10%">
        /// G</td>
        /// </tr>
        /// 
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// background-color: #00ffff;" width="10%">
        /// <b>Y = 1</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// " width="10%">
        /// R</td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// " width="10%">
        /// L</td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// " width="10%">
        /// G</td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-right-color: #000000; border-right-style: Solid; border-right-width: 1px;
        /// " width="10%">
        /// L</td>
        /// </tr>
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// background-color: #00ffff" width="10%">
        /// <b>Y = 2</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// " width="10%">
        /// L</td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// " width="10%">
        /// G</td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// " width="10%">
        /// L</td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-top-color: #000000; border-top-style: Solid;  border-top-width: 1px;
        /// border-right-color: #000000; border-right-style: Solid; border-right-width: 1px;
        /// " width="10%">
        /// B</td>
        /// </tr>
        /// 
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;
        /// background-color: #00ffff;" width="10%">
        /// <b>Y = 3</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;
        /// " width="10%">
        /// G</td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;
        /// " width="10%">
        /// L</td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;
        /// " width="10%">
        /// B</td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-right-color: #000000; border-right-style: Solid; border-right-width: 1px;
        /// border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;
        /// " width="10%">
        /// L</td>
        /// </tr>
        /// </table>
        /// </para>
        /// 
        /// <para>The alignment of the array may be modified by <see cref="BayerOffsetX" /> and <see cref="BayerOffsetY" />.
        /// The offset is measured from the 0,0 position in the sensor array to the upper left corner of the Bayer matrix table.
        /// Please note that the Bayer offset values are not affected by subframe settings.</para>
        /// <para>For example, if a CMYG2 sensor has a Bayer matrix offset as shown below, <see cref="BayerOffsetX" /> is 0 and <see cref="BayerOffsetY" /> is 1:</para>
        /// <para>
        /// <table style="width:41.254%;" cellspacing="0" width="41.254%">
        /// <col style="width: 10%;"></col>
        /// <col style="width: 10%;"></col>
        /// <col style="width: 10%;"></col>
        /// 
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// background-color: #ffffff" width="10%">
        /// </td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// background-color: #00ffff;" width="10%">
        /// <b>X = 0</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-right-color: #000000; border-right-style: Solid; border-right-width: 1px;
        /// background-color: #00ffff;" width="10%">
        /// <b>X = 1</b></td>
        /// </tr>
        /// 
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// background-color: #00ffff" width="10%">
        /// <b>Y = 0</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// " width="10%">
        /// G</td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-top-color: #000000; border-top-style: Solid;  border-top-width: 1px;
        /// border-right-color: #000000; border-right-style: Solid; border-right-width: 1px;
        /// " width="10%">
        /// M</td>
        /// </tr>
        /// 
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// background-color: #00ffff;" width="10%">
        /// <b>Y = 1</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// " width="10%">
        /// C</td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-right-color: #000000; border-right-style: Solid; border-right-width: 1px;
        /// " width="10%">
        /// Y</td>
        /// </tr>
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// background-color: #00ffff" width="10%">
        /// <b>Y = 2</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// " width="10%">
        /// M</td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-top-color: #000000; border-top-style: Solid;  border-top-width: 1px;
        /// border-right-color: #000000; border-right-style: Solid; border-right-width: 1px;
        /// " width="10%">
        /// G</td>
        /// </tr>
        /// 
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;
        /// background-color: #00ffff;" width="10%">
        /// <b>Y = 3</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;
        /// " width="10%">
        /// C</td>
        /// <td colspan="1" rowspan="1" style="width:10%;
        /// border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;
        /// border-left-color: #000000; border-left-style: Solid; border-left-width: 1px;
        /// border-right-color: #000000; border-right-style: Solid; border-right-width: 1px;
        /// border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;
        /// " width="10%">
        /// Y</td>
        /// </tr>
        /// </table>
        /// </para>
        /// <para>It is recommended that this function be called only after a <see cref="IAscomDevice.Connected">connection</see> is established with the camera hardware, to ensure that
        /// the driver is aware of the capabilities of the specific camera model.</para>
        /// </remarks>
        public SensorType SensorType
        {
            get
            {
                return DynamicClientDriver.GetValue<SensorType>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SensorType", MemberTypes.Property);
            }
        }

        #endregion

        #region ICameraV3 implementation

        /// <summary>
        /// The camera's offset (OFFSET VALUE MODE) OR the index of the selected camera offset description in the <see cref="Offsets" /> array (OFFSETS INDEX MODE)
        /// </summary>
        /// <returns><para><b> OFFSET VALUE MODE:</b> The current offset value.</para>
        /// <p style="color:red"><b>OR</b></p>
        /// <b>OFFSETS INDEX MODE:</b> Index into the Offsets array for the current camera offset
        /// </returns>
        /// <exception cref="NotImplementedException">When neither <b>OFFSETS INDEX</b> mode nor <b>OFFSET VALUE</b> mode are supported.</exception>
        /// <exception cref="InvalidValueException">When the supplied value is not valid.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>This is an optional property and can throw a NotImplementedException if Offset is not supported by the camera.</b></p>
        /// The <see cref="Offset" /> property is used to adjust the offset setting of the camera and has <b>two modes of operation</b>:
        /// <ul>
        /// <li><b>OFFSET VALUE MODE</b> - The <see cref="Offset" /> property is a direct numeric representation of the camera's offset.
        /// <ul>
        /// <li>In this mode the <see cref="OffsetMin" /> and <see cref="OffsetMax" /> properties must return integers specifying the valid range for <see cref="Offset" /></li>
        /// <li>The <see cref="Offsets"/> property must return a <see cref="NotImplementedException"/>.</li>
        /// </ul>
        /// </li>
        /// <li><b>OFFSETS INDEX MODE</b> - The <see cref="Offset" /> property is the selected offset's index within the <see cref="Offsets"/> array of textual offset descriptions.
        /// <ul>
        /// <li>In this  mode the <see cref="Offsets" /> method returns a 0-based array of strings, which describe available offset settings e.g. "ISO 200", "ISO 1600" </li>
        /// <li><see cref="OffsetMin" /> and <see cref="OffsetMax" /> must throw <see cref="NotImplementedException"/>s.</li>
        /// <li>Please note that the <see cref="Offsets"/> array is zero based.</li>
        /// </ul>
        /// </li>
        /// </ul>
        /// <para>A driver can support none, one or both offset modes depending on the camera's capabilities. However, only one mode can be active at any one moment because both modes share
        /// the <see cref="Offset"/> property to return the offset value. Client applications can determine which mode is operational by reading the <see cref="OffsetMin"/>, <see cref="OffsetMax"/> and 
        /// <see cref="Offset"/> properties. If a property can be read then its associated mode is active, if it throws a <see cref="NotImplementedException"/> then the mode is not active.</para>
        /// <para>If a driver supports both modes the astronomer must be able to select the required mode through the driver Setup dialogue.</para>
        /// <para>During driver initialisation the driver must set <see cref="Offset" /> to a valid value.</para>
        /// <para>Please note that <see cref="ReadoutMode" /> may in some cases affect the offset of the camera; if so, the driver must be ensure that the two properties do not conflict if both are used.</para>
        /// <para>This is only available in Camera Interface Version 3 and later.</para>
        /// </remarks>
        public int Offset
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Offset", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetInt(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Offset", value, MemberTypes.Property);
            }
        }

        /// <summary>
        /// Maximum <see cref="Offset" /> value of that this camera supports
        /// </summary>
        /// <returns>The maximum offset value that this camera supports</returns>
        /// <exception cref="NotImplementedException">When the <see cref="Offset"/> property is not implemented or is operating in <b>OFFSETS INDEX</b> mode.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>This is an optional property and can throw a NotImplementedException.</b></p>
        /// When <see cref="Offset"/> is operating in <b><see cref="Offset">OFFSET VALUE</see></b> mode:
        /// <ul>
        /// <li><see cref="OffsetMax" /> must return the camera's highest valid <see cref="Offset" /> setting.</li>
        /// <li><see cref="OffsetMax" /> must be equal to or greater than <see cref="OffsetMin" />.</li>
        /// <li><see cref="Offsets"/> must throw a <see cref="NotImplementedException"/></li>
        /// </ul>
        /// <para>Please note that <see cref="OffsetMin"/> and <see cref="OffsetMax"/> act together and that either both must be implemented or both must throw <see cref="NotImplementedException"/>s.</para>
        /// <para>It is recommended that this function be called only after a connection is established with the camera hardware to ensure that the driver is aware of the capabilities of the specific camera model.</para>
        /// <para>This property is only available in Camera Interface Version 3 and later.</para>
        /// </remarks>
        public int OffsetMax
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "OffsetMax", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Minimum <see cref="Offset" /> value of that this camera supports
        /// </summary>
        /// <returns>The minimum offset value that this camera supports</returns>
        /// <exception cref="NotImplementedException">When the <see cref="Offset"/> property is not implemented or is operating in <b>OFFSETS INDEX</b> mode.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>This is an optional property and can throw a NotImplementedException.</b></p>
        /// When <see cref="Offset"/> is operating in <b><see cref="Offset">OFFSET VALUE</see></b> mode:
        /// <ul>
        /// <li><see cref="OffsetMin" /> must return the camera's lowest valid <see cref="Offset" /> setting.</li>
        /// <li><see cref="OffsetMin" /> must be less than or equal to <see cref="OffsetMax" />.</li>
        /// <li><see cref="Offsets"/> must throw a <see cref="NotImplementedException"/></li>
        /// </ul>
        /// <para>Please note that <see cref="OffsetMin"/> and <see cref="OffsetMax"/> act together and that either both must be implemented or both must throw <see cref="NotImplementedException"/>s.</para>
        /// <para>It is recommended that this function be called only after a connection is established with the camera hardware to ensure that the driver is aware of the capabilities of the specific camera model.</para>
        /// <para>This property is only available in Camera Interface Version 3 and later.</para>
        /// </remarks>
        public int OffsetMin
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "OffsetMin", MemberTypes.Property);
            }
        }

        /// <summary>
        /// List of Offset names supported by the camera
        /// </summary>
        /// <returns>The list of supported offset names as an ArrayList of strings</returns>
        /// <exception cref="NotImplementedException">When the <see cref="Offset"/> property is not implemented or is operating in <b>OFFSET VALUE</b> mode.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>This is an optional property and can throw a NotImplementedException.</b></p>
        /// When <see cref="Offset"/> is operating in <b><see cref="Offset">OFFSETS INDEX</see></b> mode:
        /// <ul>
        /// <li>The <see cref="Offsets" /> property must return a zero-based ArrayList of available offset setting names.</li>
        /// <li>The <see cref="OffsetMin"/> and <see cref="OffsetMax"/> properties must throw <see cref="NotImplementedException"/>s.</li>
        /// </ul>
        /// <para>The returned offset names are at the manufacturer / driver author's discretion and could for example be: "Low gain", "Medium gain" and "High gain"to match the offset to different camera use scenarios.
        /// Typically the application software will display the returned offset names in a drop list, from which the astronomer can select the required value.
        /// The application can then configure the required offset by setting the camera's <see cref="Offset"/> property to the array index of the selected description.</para>
        /// <para>It is recommended that this function be called only after a connection is established with the camera hardware to ensure that the driver is aware of the capabilities of the specific camera model.</para>
        /// <para>This is only available in Camera Interface Version 3 and later.</para>
        /// </remarks>
        public IList<string> Offsets
        {
            get
            {
                List<string> offsets = DynamicClientDriver.GetValue<List<string>>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Offsets", MemberTypes.Property);
                LogMessage(logger, clientNumber, "Offsets", $"Returning {offsets.Count} Offsets");

                return offsets;
            }
        }

        /// <summary>
        /// Camera's sub-exposure interval
        /// </summary>
        /// <exception cref="NotImplementedException">When the camera does not support sub exposure configuration.</exception>
        /// <exception cref="InvalidValueException">When the supplied value is not valid.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>This is an optional property and can throw a NotImplementedException.</b></p>
        /// <para>This is only available in Camera Interface Version 3 and later.</para>
        /// </remarks>
        public double SubExposureDuration
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SubExposureDuration", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetDouble(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SubExposureDuration", value, MemberTypes.Property);
            }
        }

        #endregion

        #region ICameraV4 implementation

        // No new members

        #endregion

    }
}
