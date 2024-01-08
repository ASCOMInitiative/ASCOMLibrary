using ASCOM.Common;
using ASCOM.Common.Alpaca;
using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.Interfaces;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Reflection;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// ASCOM Alpaca CoverCalibrator client
    /// </summary>
    public class AlpacaCoverCalibrator : AlpacaDeviceBaseClass, ICoverCalibratorV1
    {
        #region Variables and Constants

        #endregion

        #region Initialiser

        /// <summary>
        /// Create a client for an Alpaca CoverCalibrator device with all parameters set to default values
        /// </summary>
        public AlpacaCoverCalibrator()
        {
            Initialise();
        }

        /// <summary>
        /// Create an Alpaca CoverCalibrator device specifying all parameters
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
        /// <param name="userAgentProductName">Optional product name to include in the User-Agent HTTP header sent to the Alpaca device</param>
        /// <param name="userAgentProductVersion">Optional product version to include in the User-Agent HTTP header sent to the Alpaca device</param>
        /// <param name="logger">Optional ILogger instance that can be sued to record operational information during execution</param>
        /// <param name="trustUserGeneratedSslCertificates">Trust user generated SSL certificates</param>
        public AlpacaCoverCalibrator(ServiceType serviceType = AlpacaClient.CLIENT_SERVICETYPE_DEFAULT,
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
            this.trustUserGeneratedSslCertificates= trustUserGeneratedSslCertificates;

            Initialise();
        }

        /// <summary>
        /// Create a client for an Alpaca CoverCalibrator device specifying the minimum number of parameters
        /// </summary>
        /// <param name="serviceType">HTTP or HTTPS</param>
        /// <param name="ipAddressString">Alpaca device's IP Address</param>
        /// <param name="portNumber">Alpaca device's IP Port number</param>
        /// <param name="remoteDeviceNumber">Alpaca device's device number e.g. Telescope/0</param>
        /// <param name="strictCasing">Tolerate or throw exceptions  if the Alpaca device does not use strictly correct casing for JSON object element names.</param>
        /// <param name="logger">Optional ILogger instance that can be sued to record operational information during execution</param>
        public AlpacaCoverCalibrator(ServiceType serviceType,
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
                clientDeviceType = DeviceTypes.CoverCalibrator;

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
                    logger, userAgentProductName, userAgentProductVersion,trustUserGeneratedSslCertificates);
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), "Completed initialisation");
            }
            catch (Exception ex)
            {
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), ex.ToString());
            }
        }

        #endregion

        #region ICoverCalibratorV1 Implementation

        /// <summary>
        /// Returns the state of the device cover, if present, otherwise returns "NotPresent"
        /// </summary>
        /// <remarks>
        /// <para>This is a mandatory property that must return a value, it must not throw a <see cref="NotImplementedException"/>.</para>
        /// <para>The <see cref="CoverStatus.Unknown"/> state must only be returned if the device is unaware of the cover's state e.g. if the hardware does not report the open / closed state and the cover has just been powered on.
        /// Clients do not need to take special action if this state is returned, they must carry on as usual, issuing  <see cref="OpenCover"/> or <see cref="CloseCover"/> commands as required.</para>
        /// <para>If the cover hardware cannot report its state, the device could mimic this by recording the last configured state and returning this. Driver authors or device manufacturers may also wish to offer users
        /// the capability of powering up in a known state e.g. Open or Closed and driving the hardware to this state when Connected is set <see langword="true"/>.</para>
        /// </remarks>
        public CoverStatus CoverState
        {
            get
            {
                return DynamicClientDriver.GetValue<CoverStatus>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CoverState", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns the state of the calibration device, if present, otherwise returns "NotPresent"
        /// </summary>
        /// <remarks>
        /// <para>This is a mandatory property that must return a value, it must not throw a <see cref="NotImplementedException"/>.</para>
        /// <para>The <see cref="CalibratorStatus.Unknown"/> state must only be returned if the device is unaware of the calibrator's state e.g. if the hardware does not report the device's state and 
        /// the calibrator has just been powered on. Clients do not need to take special action if this state is returned, they must carry on as usual, issuing <see cref="CalibratorOn(int)"/> and 
        /// <see cref="CalibratorOff"/> commands as required.</para>
        /// <para>If the calibrator hardware cannot report its state, the device could mimic this by recording the last configured state and returning this. Driver authors or device manufacturers may also wish to offer users
        /// the capability of powering up in a known state and driving the hardware to this state when Connected is set <see langword="true"/>.</para>
        /// </remarks>
        public CalibratorStatus CalibratorState
        {
            get
            {
                return DynamicClientDriver.GetValue<CalibratorStatus>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CalibratorState", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Returns the current calibrator brightness in the range 0 (completely off) to <see cref="MaxBrightness"/> (fully on)
        /// </summary>
        /// <exception cref="NotImplementedException">When <see cref="CalibratorState"/> returns <see cref="CalibratorStatus.NotPresent"/>.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <para>This is a mandatory property that must always return a value for a calibrator device </para>
        /// <para>The brightness value must be 0 when the <see cref="CalibratorState"/> is <see cref="CalibratorStatus.Off"/></para>
        /// </remarks>
        public int Brightness
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Brightness", MemberTypes.Property);
            }
        }

        /// <summary>
        /// The Brightness value that makes the calibrator deliver its maximum illumination.
        /// </summary>
        /// <exception cref="NotImplementedException">When <see cref="CalibratorState"/> returns <see cref="CalibratorStatus.NotPresent"/>.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <para>This is a mandatory property for a calibrator device and must always return a value within the integer range 1 to 2,147,483,647</para>
        /// <para>A value of 1 indicates that the calibrator can only be "off" or "on".</para>
        /// <para>A value of 10 indicates that the calibrator has 10 discreet illumination levels in addition to "off".</para>
        /// <para>The value for this parameter should be determined by the driver author or device manufacturer based on the capabilities of the hardware used in the calibrator.</para>
        /// </remarks>
        public int MaxBrightness
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "MaxBrightness", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Initiates cover opening if a cover is present
        /// </summary>
        /// <exception cref="NotImplementedException">When <see cref="CoverState"/> returns <see cref="CoverStatus.NotPresent"/>.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <para>While the cover is opening <see cref="CoverState"/> must return <see cref="CoverStatus.Moving"/>.</para>
        /// <para>When the cover is open <see cref="CoverState"/> must return <see cref="CoverStatus.Open"/>.</para>
        /// <para>If an error condition arises while moving between states, <see cref="CoverState"/> must be set to <see cref="CoverStatus.Error"/> rather than <see cref="CoverStatus.Unknown"/>.</para>
        /// </remarks>
        public void OpenCover()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "OpenCover", MemberTypes.Method);
            LogMessage(logger, clientNumber, "AbortSlew", "Cover opened OK");
        }

        /// <summary>
        /// Initiates cover closing if a cover is present
        /// </summary>
        /// <exception cref="NotImplementedException">When <see cref="CoverState"/> returns <see cref="CoverStatus.NotPresent"/>.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <para>While the cover is closing <see cref="CoverState"/> must return <see cref="CoverStatus.Moving"/>.</para>
        /// <para>When the cover is closed <see cref="CoverState"/> must return <see cref="CoverStatus.Closed"/>.</para>
        /// <para>If an error condition arises while moving between states, <see cref="CoverState"/> must be set to <see cref="CoverStatus.Error"/> rather than <see cref="CoverStatus.Unknown"/>.</para>
        /// </remarks>
        public void CloseCover()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CloseCover", MemberTypes.Method);
            LogMessage(logger, clientNumber, "AbortSlew", "Cover closed OK");
        }

        /// <summary>
        /// Stops any cover movement that may be in progress if a cover is present and cover movement can be interrupted.
        /// </summary>
        /// <exception cref="NotImplementedException">When <see cref="CoverState"/> returns <see cref="CoverStatus.NotPresent"/> or if cover movement cannot be interrupted.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <para>This must stop any cover movement as soon as possible and set a <see cref="CoverState"/> of <see cref="CoverStatus.Open"/>, <see cref="CoverStatus.Closed"/> 
        /// or <see cref="CoverStatus.Unknown"/> as appropriate.</para>
        /// <para>If cover movement cannot be interrupted, a <see cref="NotImplementedException"/> must be thrown.</para>
        /// </remarks>
        public void HaltCover()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "HaltCover", MemberTypes.Method);
            LogMessage(logger, clientNumber, "AbortSlew", "Cover halted OK");
        }

        /// <summary>
        /// Turns the calibrator on at the specified brightness if the device has calibration capability
        /// </summary>
        /// <param name="Brightness">Sets the required calibrator illumination brightness in the range 0 (fully off) to <see cref="MaxBrightness"/> (fully on).</param>
        /// <exception cref="NotImplementedException">When <see cref="CalibratorState"/> returns <see cref="CalibratorStatus.NotPresent"/>.</exception>
        /// <exception cref="InvalidValueException">When the supplied brightness parameter is outside the range 0 to <see cref="MaxBrightness"/>.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <para>This is a mandatory method for a calibrator device that must be implemented.</para>
        /// <para>If the calibrator takes some time to stabilise, the <see cref="CalibratorState"/> must return <see cref="CalibratorStatus.NotReady"/>. When the 
        /// calibrator is ready for use <see cref="CalibratorState"/> must return <see cref="CalibratorStatus.Ready"/>.</para>
        /// <para>For devices with both cover and calibrator capabilities, this method may change the <see cref="CoverState"/>, if required.</para>
        /// <para>If an error condition arises while turning on the calibrator, <see cref="CalibratorState"/> must be set to <see cref="CalibratorStatus.Error"/> rather than <see cref="CalibratorStatus.Unknown"/>.</para>
        /// </remarks>
        public void CalibratorOn(int Brightness)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.BRIGHTNESS_PARAMETER_NAME, Brightness.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CalibratorOn", Parameters, HttpMethod.Put, MemberTypes.Method);
        }

        /// <summary>
        /// Turns the calibrator off if the device has calibration capability
        /// </summary>
        /// <exception cref="NotImplementedException">When <see cref="CalibratorState"/> returns <see cref="CalibratorStatus.NotPresent"/>.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <para>This is a mandatory method for a calibrator device.</para>
        /// <para>If the calibrator requires time to safely stabilise after use, <see cref="CalibratorState"/> must return <see cref="CalibratorStatus.NotReady"/>. When the 
        /// calibrator is safely off <see cref="CalibratorState"/> must return <see cref="CalibratorStatus.Off"/>.</para>
        /// <para>For devices with both cover and calibrator capabilities, this method will return the <see cref="CoverState"/> to its status prior to calling <see cref="CalibratorOn(int)"/>.</para>
        /// <para>If an error condition arises while turning off the calibrator, <see cref="CalibratorState"/> must be set to <see cref="CalibratorStatus.Error"/> rather than <see cref="CalibratorStatus.Unknown"/>.</para>
        /// </remarks>
        public void CalibratorOff()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CalibratorOff", MemberTypes.Method);
            LogMessage(logger, clientNumber, "AbortSlew", $"Calibrator off OK");
        }

        #endregion

    }
}