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
        /// <param name="request100Continue"></param>Request 100-continue behaviour for HTTP requests. Defaults to false.
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
            this.imageArrayTransferType = imageArrayTransferType;
            this.imageArrayCompression = imageArrayCompression;
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
                    logger, userAgentProductName, userAgentProductVersion, trustUserGeneratedSslCertificates, request100Continue);
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

        ///<inheritdoc/>
        public void AbortExposure()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "AbortExposure", MemberTypes.Method);
        }

        ///<inheritdoc/>
        public void PulseGuide(GuideDirection Direction, int Duration)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.DIRECTION_PARAMETER_NAME, ((int)Direction).ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.DURATION_PARAMETER_NAME, Duration.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "PulseGuide", Parameters, HttpMethod.Put, MemberTypes.Method);
        }

        ///<inheritdoc/>
        public void StartExposure(double Duration, bool Light)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.DURATION_PARAMETER_NAME, Duration.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.LIGHT_PARAMETER_NAME, Light.ToString() }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "StartExposure", Parameters, HttpMethod.Put, MemberTypes.Method);
        }

        ///<inheritdoc/>
        public void StopExposure()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "StopExposure", MemberTypes.Method);
        }

        ///<inheritdoc/>
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

        ///<inheritdoc/>
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

        ///<inheritdoc/>
        public CameraState CameraState
        {
            get
            {
                return DynamicClientDriver.GetValue<CameraState>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CameraState", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public int CameraXSize
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CameraXSize", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public int CameraYSize
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CameraYSize", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public bool CanAbortExposure
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanAbortExposure", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public bool CanAsymmetricBin
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanAsymmetricBin", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public bool CanGetCoolerPower
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanGetCoolerPower", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public bool CanPulseGuide
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanPulseGuide", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public bool CanSetCCDTemperature
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanSetCCDTemperature", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public bool CanStopExposure
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanStopExposure", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public double CCDTemperature
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CCDTemperature", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
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

        ///<inheritdoc/>
        public double CoolerPower
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CoolerPower", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public double ElectronsPerADU
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "ElectronsPerADU", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public double FullWellCapacity
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "FullWellCapacity", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public bool HasShutter
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "HasShutter", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public double HeatSinkTemperature
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "HeatSinkTemperature", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public object ImageArray
        {
            get
            {
                return DynamicClientDriver.GetValue<Array>(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "ImageArray", imageArrayTransferType, imageArrayCompression, MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public object ImageArrayVariant
        {
            get
            {
                return DynamicClientDriver.ImageArrayVariant(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, imageArrayTransferType, imageArrayCompression);
            }
        }

        ///<inheritdoc/>
        public bool ImageReady
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "ImageReady", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public bool IsPulseGuiding
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "IsPulseGuiding", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public double LastExposureDuration
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "LastExposureDuration", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public string LastExposureStartTime
        {
            get
            {
                return DynamicClientDriver.GetValue<string>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "LastExposureStartTime", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public int MaxADU
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "MaxADU", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public short MaxBinX
        {
            get
            {
                return DynamicClientDriver.GetValue<short>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "MaxBinX", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public short MaxBinY
        {
            get
            {
                return DynamicClientDriver.GetValue<short>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "MaxBinY", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
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

        ///<inheritdoc/>
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

        ///<inheritdoc/>
        public double PixelSizeX
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "PixelSizeX", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public double PixelSizeY
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "PixelSizeY", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
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

        ///<inheritdoc/>
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

        ///<inheritdoc/>
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

        ///<inheritdoc/>
        public short BayerOffsetX
        {
            get
            {
                return DynamicClientDriver.GetValue<short>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "BayerOffsetX", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public short BayerOffsetY
        {
            get
            {
                return DynamicClientDriver.GetValue<short>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "BayerOffsetY", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public bool CanFastReadout
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanFastReadout", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public double ExposureMax
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "ExposureMax", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public double ExposureMin
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "ExposureMin", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public double ExposureResolution
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "ExposureResolution", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
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

        ///<inheritdoc/>
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

        ///<inheritdoc/>
        public short GainMax
        {
            get
            {
                return DynamicClientDriver.GetValue<short>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "GainMax", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public short GainMin
        {
            get
            {
                return DynamicClientDriver.GetValue<short>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "GainMin", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public IList<string> Gains
        {
            get
            {
                List<string> gains = DynamicClientDriver.GetValue<List<string>>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Gains", MemberTypes.Property);
                LogMessage(logger, clientNumber, "Gains", $"Returning {gains.Count} gains");

                return gains;
            }
        }

        ///<inheritdoc/>
        public short PercentCompleted
        {
            get
            {
                return DynamicClientDriver.GetValue<short>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "PercentCompleted", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
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

        ///<inheritdoc/>
        public IList<string> ReadoutModes
        {
            get
            {
                List<string> modes = DynamicClientDriver.GetValue<List<string>>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "ReadoutModes", MemberTypes.Property);
                LogMessage(logger, clientNumber, "ReadoutModes", $"Returning {modes.Count} modes");

                return modes;
            }
        }

        ///<inheritdoc/>
        public string SensorName
        {
            get
            {
                return DynamicClientDriver.GetValue<string>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SensorName", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public SensorType SensorType
        {
            get
            {
                return DynamicClientDriver.GetValue<SensorType>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "SensorType", MemberTypes.Property);
            }
        }

        #endregion

        #region ICameraV3 implementation

        ///<inheritdoc/>
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

        ///<inheritdoc/>
        public int OffsetMax
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "OffsetMax", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public int OffsetMin
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "OffsetMin", MemberTypes.Property);
            }
        }

        ///<inheritdoc/>
        public IList<string> Offsets
        {
            get
            {
                List<string> offsets = DynamicClientDriver.GetValue<List<string>>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Offsets", MemberTypes.Property);
                LogMessage(logger, clientNumber, "Offsets", $"Returning {offsets.Count} Offsets");

                return offsets;
            }
        }

        ///<inheritdoc/>
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
