using ASCOM.Common;
using ASCOM.Common.Alpaca;
using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.DeviceStateClasses;
using ASCOM.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

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
        /// Initializes a new instance of the AlpacaCamera class using the specified configuration settings.
        /// </summary>
        /// <param name="configuration">The configuration settings used to initialize the camera. Cannot be null.</param>
        public AlpacaCamera(AlpacaConfiguration configuration)
        {
            this.serviceType = configuration.ServiceType;
            this.ipAddressString = configuration.IpAddressString;
            this.portNumber = configuration.PortNumber;
            this.remoteDeviceNumber = configuration.RemoteDeviceNumber;
            this.establishConnectionTimeout = configuration.EstablishConnectionTimeout;
            this.standardDeviceResponseTimeout = configuration.StandardDeviceResponseTimeout;
            this.longDeviceResponseTimeout = configuration.LongDeviceResponseTimeout;
            this.clientNumber = configuration.ClientNumber;
            this.imageArrayTransferType = configuration.ImageArrayTransferType;
            this.imageArrayCompression = configuration.ImageArrayCompression;
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
        /// <param name="request100Continue">Request 100-continue behaviour for HTTP requests. Defaults to false.</param>
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
            clientNumber = RemoteDevice.GetUniqueClientNumber();
            Initialise();
        }
        private void Initialise()
        {
            try
            {
                // Set the device type
                clientDeviceType = Common.DeviceTypes.Camera;

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
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"ImageArray transfer type: {imageArrayTransferType}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"ImageArray compression: {imageArrayCompression}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"User name is Null or Empty: {string.IsNullOrEmpty(userName)}, User name is Null or White Space: {string.IsNullOrWhiteSpace(userName)}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"User name length: {password.Length}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Password is Null or Empty: {string.IsNullOrEmpty(password)}, Password is Null or White Space: {string.IsNullOrWhiteSpace(password)}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Password length: {password.Length}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Strict casing: {strictCasing}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Trust user generated SSL certificates: {trustUserGeneratedSslCertificates}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Request 100CONTINUE: {request100Continue}");

                RemoteDevice.CreateHttpClient(ref client, serviceType, ipAddressString, portNumber, clientNumber, clientDeviceType, userName, password, imageArrayCompression,
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
            RemoteDevice.CallNoParameters(CreateParameters(standardDeviceResponseTimeout, "AbortExposure", MemberTypes.Method));
        }

        ///<inheritdoc/>
        public void PulseGuide(GuideDirection Direction, int Duration)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.DIRECTION_PARAMETER_NAME, ((int)Direction).ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.DURATION_PARAMETER_NAME, Duration.ToString(CultureInfo.InvariantCulture) }
            };
            RemoteDevice.Send<NoReturnValue>(CreateParameters(standardDeviceResponseTimeout, "PulseGuide", MemberTypes.Method), Parameters, HttpMethod.Put);
        }

        ///<inheritdoc/>
        public void StartExposure(double Duration, bool Light)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.DURATION_PARAMETER_NAME, Duration.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.LIGHT_PARAMETER_NAME, Light.ToString() }
            };
            RemoteDevice.Send<NoReturnValue>(CreateParameters(standardDeviceResponseTimeout, "StartExposure", MemberTypes.Method), Parameters, HttpMethod.Put);
        }

        ///<inheritdoc/>
        public void StopExposure()
        {
            RemoteDevice.CallNoParameters(CreateParameters(standardDeviceResponseTimeout, "StopExposure", MemberTypes.Method));
        }

        ///<inheritdoc/>
        public short BinX
        {
            get
            {
                return RemoteDevice.GetValue<short>(CreateParameters(standardDeviceResponseTimeout, "BinX", MemberTypes.Property));
            }

            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "BinX", MemberTypes.Property), value);
            }
        }

        ///<inheritdoc/>
        public short BinY
        {
            get
            {
                return RemoteDevice.GetValue<short>(CreateParameters(standardDeviceResponseTimeout, "BinY", MemberTypes.Property));
            }

            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "BinY", MemberTypes.Property), value);
            }
        }

        ///<inheritdoc/>
        public CameraState CameraState
        {
            get
            {
                return RemoteDevice.GetValue<CameraState>(CreateParameters(standardDeviceResponseTimeout, "CameraState", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public int CameraXSize
        {
            get
            {
                return RemoteDevice.GetValue<int>(CreateParameters(standardDeviceResponseTimeout, "CameraXSize", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public int CameraYSize
        {
            get
            {
                return RemoteDevice.GetValue<int>(CreateParameters(standardDeviceResponseTimeout, "CameraYSize", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public bool CanAbortExposure
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "CanAbortExposure", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public bool CanAsymmetricBin
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "CanAsymmetricBin", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public bool CanGetCoolerPower
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "CanGetCoolerPower", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public bool CanPulseGuide
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "CanPulseGuide", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public bool CanSetCCDTemperature
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "CanSetCCDTemperature", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public bool CanStopExposure
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "CanStopExposure", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public double CCDTemperature
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "CCDTemperature", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public bool CoolerOn
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "CoolerOn", MemberTypes.Property));
            }

            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "CoolerOn", MemberTypes.Property), value);
            }
        }

        ///<inheritdoc/>
        public double CoolerPower
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "CoolerPower", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public double ElectronsPerADU
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "ElectronsPerADU", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public double FullWellCapacity
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "FullWellCapacity", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public bool HasShutter
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "HasShutter", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public double HeatSinkTemperature
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "HeatSinkTemperature", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public object ImageArray
        {
            get
            {
                var p = CreateParameters(longDeviceResponseTimeout, "ImageArray", MemberTypes.Property);
                p.ImageArrayTransferType = imageArrayTransferType;
                p.ImageArrayCompression = imageArrayCompression;
                return RemoteDevice.GetValue<Array>(p);
            }
        }

        ///<inheritdoc/>
        public object ImageArrayVariant
        {
            get
            {
                Array returnArray;
                object[,] objectArray2D;
                object[,,] objectArray3D;
                Stopwatch sw = new Stopwatch();

                var p = new Parameters(clientNumber, client, longDeviceResponseTimeout, uriBase, strictCasing, logger, "ImageArrayVariant", MemberTypes.Property)
                {
                    ImageArrayTransferType = imageArrayTransferType,
                    ImageArrayCompression = imageArrayCompression
                };
                returnArray = RemoteDevice.GetValue<Array>(p);

                try
                {
                    string variantType = returnArray.GetType().Name;
                    string elementType;
                    switch (returnArray.Rank) // Process 2D and 3D variant arrays, all other types are unsupported
                    {
                        case 2: // 2D Array
                            elementType = returnArray.GetValue(0, 0).GetType().Name;
                            break;
                        case 3: // 3D array
                            elementType = returnArray.GetValue(0, 0, 0).GetType().Name;
                            break;
                        default:
                            throw new InvalidValueException("ReturnImageArray: Received an unsupported return array rank: " + returnArray.Rank);
                    }

                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Received {variantType} array of Rank {returnArray.Rank} with Length {returnArray.Length} and element type {elementType}");

                    // convert to variant

                    switch (returnArray.Rank)
                    {
                        case 2:
                            objectArray2D = new object[returnArray.GetLength(0), returnArray.GetLength(1)];
                            switch (variantType)
                            {
                                case "Byte[,]":
                                    Byte[,] byte2DArray = (Byte[,])returnArray;

                                    sw.Restart();
                                    Parallel.For(0, returnArray.GetLength(0), (i) =>
                                    {
                                        for (int j = 0; j < returnArray.GetLength(1); j++)
                                        {
                                            objectArray2D[i, j] = byte2DArray[i, j];
                                        }
                                    });

                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying Byte[,] array in {sw.ElapsedMilliseconds}ms.");
                                    return objectArray2D;

                                case "Int16[,]":
                                    Int16[,] short2DArray = (Int16[,])returnArray;

                                    sw.Restart();
                                    Parallel.For(0, returnArray.GetLength(0), (i) =>
                                    {
                                        for (int j = 0; j < returnArray.GetLength(1); j++)
                                        {
                                            objectArray2D[i, j] = short2DArray[i, j];
                                        }
                                    });

                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying Int16[,] array in {sw.ElapsedMilliseconds}ms.");
                                    return objectArray2D;

                                case "UInt16[,]":
                                    UInt16[,] uint16Array2D = (UInt16[,])returnArray;

                                    sw.Restart();
                                    Parallel.For(0, returnArray.GetLength(0), (i) =>
                                    {
                                        for (int j = 0; j < returnArray.GetLength(1); j++)
                                        {
                                            objectArray2D[i, j] = uint16Array2D[i, j];
                                        }
                                    });

                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying UInt16[,] array in {sw.ElapsedMilliseconds}ms.");
                                    return objectArray2D;

                                case "Int32[,]":
                                    Int32[,] int2DArray = (Int32[,])returnArray;

                                    sw.Restart();
                                    Parallel.For(0, returnArray.GetLength(0), (i) =>
                                    {
                                        for (int j = 0; j < returnArray.GetLength(1); j++)
                                        {
                                            objectArray2D[i, j] = int2DArray[i, j];
                                        }
                                    });

                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying Int32[,] array in {sw.ElapsedMilliseconds}ms.");
                                    return objectArray2D;

                                case "UInt32[,]":
                                    UInt32[,] uint2DArray = (UInt32[,])returnArray;

                                    sw.Restart();
                                    Parallel.For(0, returnArray.GetLength(0), (i) =>
                                    {
                                        for (int j = 0; j < returnArray.GetLength(1); j++)
                                        {
                                            objectArray2D[i, j] = uint2DArray[i, j];
                                        }
                                    });

                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying UInt32[,] array in {sw.ElapsedMilliseconds}ms.");
                                    return objectArray2D;

                                case "Int64[,]":
                                    Int64[,] int64Array2D = (Int64[,])returnArray;

                                    sw.Restart();
                                    Parallel.For(0, returnArray.GetLength(0), (i) =>
                                    {
                                        for (int j = 0; j < returnArray.GetLength(1); j++)
                                        {
                                            objectArray2D[i, j] = int64Array2D[i, j];
                                        }
                                    });

                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying Int64[,] array in {sw.ElapsedMilliseconds}ms.");
                                    return objectArray2D;

                                case "UInt64[,]":
                                    UInt64[,] uint64Array2D = (UInt64[,])returnArray;

                                    sw.Restart();
                                    Parallel.For(0, returnArray.GetLength(0), (i) =>
                                    {
                                        for (int j = 0; j < returnArray.GetLength(1); j++)
                                        {
                                            objectArray2D[i, j] = uint64Array2D[i, j];
                                        }
                                    });

                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying UInt64[,] array in {sw.ElapsedMilliseconds}ms.");
                                    return objectArray2D;

                                case "Single[,]":
                                    Single[,] single2DArray = (Single[,])returnArray;

                                    sw.Restart();
                                    Parallel.For(0, returnArray.GetLength(0), (i) =>
                                    {
                                        for (int j = 0; j < returnArray.GetLength(1); j++)
                                        {
                                            objectArray2D[i, j] = single2DArray[i, j];
                                        }
                                    });

                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying Single[,] array in {sw.ElapsedMilliseconds}ms.");
                                    return objectArray2D;

                                case "Double[,]":
                                    Double[,] double2DArray = (Double[,])returnArray;

                                    sw.Restart();
                                    Parallel.For(0, returnArray.GetLength(0), (i) =>
                                    {
                                        for (int j = 0; j < returnArray.GetLength(1); j++)
                                        {
                                            objectArray2D[i, j] = double2DArray[i, j];
                                        }
                                    });

                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying Double[,] array in {sw.ElapsedMilliseconds}ms.");
                                    return objectArray2D;

                                case "Object[,]":
                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Received an Object[,] array, returning it directly to the client without further processing.");
                                    return returnArray;

                                default:
                                    throw new InvalidValueException("Alpaca client - Camera.ImageArrayVariant: Unsupported return array rank from DynamicClientDriver.GetValue<Array>: " + returnArray.Rank);
                            }
                        case 3:
                            objectArray3D = new object[returnArray.GetLength(0), returnArray.GetLength(1), returnArray.GetLength(2)];
                            switch (variantType)
                            {
                                case "Byte[,,]":
                                    Byte[,,] byte3DArray = (Byte[,,])returnArray;

                                    sw.Restart();
                                    Parallel.For(0, returnArray.GetLength(0), (i) =>
                                    {
                                        for (int j = 0; j < returnArray.GetLength(1); j++)
                                        {
                                            for (int k = 0; k < returnArray.GetLength(2); k++)
                                                objectArray3D[i, j, k] = byte3DArray[i, j, k];
                                        }
                                    });

                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying Byte[,,] array in {sw.ElapsedMilliseconds}ms.");
                                    return objectArray3D;

                                case "Int16[,,]":
                                    Int16[,,] short3DArray = (Int16[,,])returnArray;

                                    sw.Restart();
                                    Parallel.For(0, returnArray.GetLength(0), (i) =>
                                    {
                                        for (int j = 0; j < returnArray.GetLength(1); j++)
                                        {
                                            for (int k = 0; k < returnArray.GetLength(2); k++)
                                                objectArray3D[i, j, k] = short3DArray[i, j, k];
                                        }
                                    });

                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying Int16[,,] array in {sw.ElapsedMilliseconds}ms.");
                                    return objectArray3D;

                                case "UInt16[,,]":
                                    UInt16[,,] uint16Array3D = (UInt16[,,])returnArray;

                                    sw.Restart();
                                    Parallel.For(0, returnArray.GetLength(0), (i) =>
                                    {
                                        for (int j = 0; j < returnArray.GetLength(1); j++)
                                        {
                                            for (int k = 0; k < returnArray.GetLength(2); k++)
                                                objectArray3D[i, j, k] = uint16Array3D[i, j, k];
                                        }
                                    });

                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying UInt16[,,] array in {sw.ElapsedMilliseconds}ms.");
                                    return objectArray3D;

                                case "Int32[,,]":
                                    Int32[,,] int3DArray = (Int32[,,])returnArray;

                                    sw.Restart();
                                    Parallel.For(0, returnArray.GetLength(0), (i) =>
                                    {
                                        for (int j = 0; j < returnArray.GetLength(1); j++)
                                        {
                                            for (int k = 0; k < returnArray.GetLength(2); k++)
                                            {
                                                objectArray3D[i, j, k] = int3DArray[i, j, k];
                                            }
                                        }
                                    });

                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying Int32[,,] array in {sw.ElapsedMilliseconds}ms.");
                                    return objectArray3D;

                                case "UInt32[,,]":
                                    UInt32[,,] uint32Array3D = (UInt32[,,])returnArray;

                                    sw.Restart();
                                    Parallel.For(0, returnArray.GetLength(0), (i) =>
                                    {
                                        for (int j = 0; j < returnArray.GetLength(1); j++)
                                        {
                                            for (int k = 0; k < returnArray.GetLength(2); k++)
                                            {
                                                objectArray3D[i, j, k] = uint32Array3D[i, j, k];
                                            }
                                        }
                                    });

                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying UInt32[,,] array in {sw.ElapsedMilliseconds}ms.");
                                    return objectArray3D;

                                case "Int64[,,]":
                                    Int64[,,] int64Array3D = (Int64[,,])returnArray;

                                    sw.Restart();
                                    Parallel.For(0, returnArray.GetLength(0), (i) =>
                                    {
                                        for (int j = 0; j < returnArray.GetLength(1); j++)
                                        {
                                            for (int k = 0; k < returnArray.GetLength(2); k++)
                                            {
                                                objectArray3D[i, j, k] = int64Array3D[i, j, k];
                                            }
                                        }
                                    });

                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying Int64[,,] array in {sw.ElapsedMilliseconds}ms.");
                                    return objectArray3D;

                                case "UInt64[,,]":
                                    UInt64[,,] uint64Array3D = (UInt64[,,])returnArray;

                                    sw.Restart();
                                    Parallel.For(0, returnArray.GetLength(0), (i) =>
                                    {
                                        for (int j = 0; j < returnArray.GetLength(1); j++)
                                        {
                                            for (int k = 0; k < returnArray.GetLength(2); k++)
                                            {
                                                objectArray3D[i, j, k] = uint64Array3D[i, j, k];
                                            }
                                        }
                                    });

                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying UInt64[,,] array in {sw.ElapsedMilliseconds}ms.");
                                    return objectArray3D;

                                case "Single[,,]":
                                    Single[,,] single3DDArray = (Single[,,])returnArray;

                                    sw.Restart();
                                    Parallel.For(0, returnArray.GetLength(0), (i) =>
                                    {
                                        for (int j = 0; j < returnArray.GetLength(1); j++)
                                        {
                                            for (int k = 0; k < returnArray.GetLength(2); k++)
                                                objectArray3D[i, j, k] = single3DDArray[i, j, k];
                                        }
                                    });

                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying Single[,,] array in {sw.ElapsedMilliseconds}ms.");
                                    return objectArray3D;

                                case "Double[,,]":
                                    Double[,,] double3DDArray = (Double[,,])returnArray;

                                    sw.Restart();
                                    Parallel.For(0, returnArray.GetLength(0), (i) =>
                                    {
                                        for (int j = 0; j < returnArray.GetLength(1); j++)
                                        {
                                            for (int k = 0; k < returnArray.GetLength(2); k++)
                                                objectArray3D[i, j, k] = double3DDArray[i, j, k];
                                        }
                                    });

                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying Double[,,] array in {sw.ElapsedMilliseconds}ms.");
                                    return objectArray3D;

                                case "Object[,,]":
                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Received an Object[,,] array, returning it directly to the client without further processing.");
                                    return returnArray;

                                default:
                                    throw new InvalidValueException("DynamicRemoteClient Driver Camera.ImageArrayVariant: Unsupported return array rank from DynamicClientDriver.GetValue<Array>: " + returnArray.Rank);
                            }

                        default:
                            throw new InvalidValueException("DynamicRemoteClient Driver Camera.ImageArrayVariant: Unsupported return array rank from DynamicClientDriver.GetValue<Array>: " + returnArray.Rank);
                    }
                }
                catch (Exception ex)
                {
                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Exception: \r\n{ex}");
                    throw;
                }
            }
        }

        ///<inheritdoc/>
        public bool ImageReady
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "ImageReady", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public bool IsPulseGuiding
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "IsPulseGuiding", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public double LastExposureDuration
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "LastExposureDuration", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public string LastExposureStartTime
        {
            get
            {
                return RemoteDevice.GetValue<string>(CreateParameters(standardDeviceResponseTimeout, "LastExposureStartTime", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public int MaxADU
        {
            get
            {
                return RemoteDevice.GetValue<int>(CreateParameters(standardDeviceResponseTimeout, "MaxADU", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public short MaxBinX
        {
            get
            {
                return RemoteDevice.GetValue<short>(CreateParameters(standardDeviceResponseTimeout, "MaxBinX", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public short MaxBinY
        {
            get
            {
                return RemoteDevice.GetValue<short>(CreateParameters(standardDeviceResponseTimeout, "MaxBinY", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public int NumX
        {
            get
            {
                return RemoteDevice.GetValue<int>(CreateParameters(standardDeviceResponseTimeout, "NumX", MemberTypes.Property));
            }

            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "NumX", MemberTypes.Property), value);
            }
        }

        ///<inheritdoc/>
        public int NumY
        {
            get
            {
                return RemoteDevice.GetValue<int>(CreateParameters(standardDeviceResponseTimeout, "NumY", MemberTypes.Property));
            }

            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "NumY", MemberTypes.Property), value);
            }
        }

        ///<inheritdoc/>
        public double PixelSizeX
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "PixelSizeX", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public double PixelSizeY
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "PixelSizeY", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public double SetCCDTemperature
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "SetCCDTemperature", MemberTypes.Property));
            }

            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "SetCCDTemperature", MemberTypes.Property), value);
            }
        }

        ///<inheritdoc/>
        public int StartX
        {
            get
            {
                return RemoteDevice.GetValue<int>(CreateParameters(standardDeviceResponseTimeout, "StartX", MemberTypes.Property));
            }

            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "StartX", MemberTypes.Property), value);
            }
        }

        ///<inheritdoc/>
        public int StartY
        {
            get
            {
                return RemoteDevice.GetValue<int>(CreateParameters(standardDeviceResponseTimeout, "StartY", MemberTypes.Property));
            }

            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "StartY", MemberTypes.Property), value);
            }
        }

        ///<inheritdoc/>
        public short BayerOffsetX
        {
            get
            {
                return RemoteDevice.GetValue<short>(CreateParameters(standardDeviceResponseTimeout, "BayerOffsetX", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public short BayerOffsetY
        {
            get
            {
                return RemoteDevice.GetValue<short>(CreateParameters(standardDeviceResponseTimeout, "BayerOffsetY", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public bool CanFastReadout
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "CanFastReadout", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public double ExposureMax
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "ExposureMax", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public double ExposureMin
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "ExposureMin", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public double ExposureResolution
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "ExposureResolution", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public bool FastReadout
        {
            get
            {
                return RemoteDevice.GetValue<bool>(CreateParameters(standardDeviceResponseTimeout, "FastReadout", MemberTypes.Property));
            }

            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "FastReadout", MemberTypes.Property), value);
            }
        }

        ///<inheritdoc/>
        public short Gain
        {
            get
            {
                return RemoteDevice.GetValue<short>(CreateParameters(standardDeviceResponseTimeout, "Gain", MemberTypes.Property));
            }

            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "Gain", MemberTypes.Property), value);
            }
        }

        ///<inheritdoc/>
        public short GainMax
        {
            get
            {
                return RemoteDevice.GetValue<short>(CreateParameters(standardDeviceResponseTimeout, "GainMax", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public short GainMin
        {
            get
            {
                return RemoteDevice.GetValue<short>(CreateParameters(standardDeviceResponseTimeout, "GainMin", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public IList<string> Gains
        {
            get
            {
                List<string> gains = RemoteDevice.GetValue<List<string>>(CreateParameters(standardDeviceResponseTimeout, "Gains", MemberTypes.Property));
                LogMessage(logger, clientNumber, "Gains", $"Returning {gains.Count} gains");

                return gains;
            }
        }

        ///<inheritdoc/>
        public short PercentCompleted
        {
            get
            {
                return RemoteDevice.GetValue<short>(CreateParameters(standardDeviceResponseTimeout, "PercentCompleted", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public short ReadoutMode
        {
            get
            {
                return RemoteDevice.GetValue<short>(CreateParameters(standardDeviceResponseTimeout, "ReadoutMode", MemberTypes.Property));
            }

            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "ReadoutMode", MemberTypes.Property), value);
            }
        }

        ///<inheritdoc/>
        public IList<string> ReadoutModes
        {
            get
            {
                List<string> modes = RemoteDevice.GetValue<List<string>>(CreateParameters(standardDeviceResponseTimeout, "ReadoutModes", MemberTypes.Property));
                LogMessage(logger, clientNumber, "ReadoutModes", $"Returning {modes.Count} modes");

                return modes;
            }
        }

        ///<inheritdoc/>
        public string SensorName
        {
            get
            {
                return RemoteDevice.GetValue<string>(CreateParameters(standardDeviceResponseTimeout, "SensorName", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public SensorType SensorType
        {
            get
            {
                return RemoteDevice.GetValue<SensorType>(CreateParameters(standardDeviceResponseTimeout, "SensorType", MemberTypes.Property));
            }
        }

        #endregion

        #region ICameraV3 implementation

        ///<inheritdoc/>
        public int Offset
        {
            get
            {
                return RemoteDevice.GetValue<int>(CreateParameters(standardDeviceResponseTimeout, "Offset", MemberTypes.Property));
            }

            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "Offset", MemberTypes.Property), value);
            }
        }

        ///<inheritdoc/>
        public int OffsetMax
        {
            get
            {
                return RemoteDevice.GetValue<int>(CreateParameters(standardDeviceResponseTimeout, "OffsetMax", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public int OffsetMin
        {
            get
            {
                return RemoteDevice.GetValue<int>(CreateParameters(standardDeviceResponseTimeout, "OffsetMin", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public IList<string> Offsets
        {
            get
            {
                List<string> offsets = RemoteDevice.GetValue<List<string>>(CreateParameters(standardDeviceResponseTimeout, "Offsets", MemberTypes.Property));
                LogMessage(logger, clientNumber, "Offsets", $"Returning {offsets.Count} Offsets");

                return offsets;
            }
        }

        ///<inheritdoc/>
        public double SubExposureDuration
        {
            get
            {
                return RemoteDevice.GetValue<double>(CreateParameters(standardDeviceResponseTimeout, "SubExposureDuration", MemberTypes.Property));
            }

            set
            {
                RemoteDevice.SetValue(CreateParameters(standardDeviceResponseTimeout, "SubExposureDuration", MemberTypes.Property), value);
            }
        }

        #endregion

        #region ICameraV4 implementation

        // No new members

        #endregion

    }
}
