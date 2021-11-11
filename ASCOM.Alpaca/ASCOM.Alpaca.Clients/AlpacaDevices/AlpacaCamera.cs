using ASCOM.Common.Alpaca;
using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.Helpers;
using ASCOM.Common.Interfaces;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// ASCOM DynamicRemoteClients Camera base class.
    /// </summary>
    public class AlpacaCamera : AlpacaDeviceBaseClass, ICameraV3
    {
        #region Variables and Constants

        // Set the device type
        private const string DEVICE_TYPE = "Camera";
        public const string GETBASE64IMAGE_ACTION_NAME = "GetBase64Image";
        public const int GETBASE64IMAGE_SUPPORTED_VERSION = 1;

        // Variables specific to this device type
        private ImageArrayTransferType imageArrayTransferType = ImageArrayTransferType.JSON;
        private ImageArrayCompression imageArrayCompression = ImageArrayCompression.None;
        private bool? canGetBase64Image = null; // Indicator of whether the remote device supports GetBase64Image functionality
        private bool? canGetImageBytes = null; // Indicator of whether the remote device supports GetBase64Image functionality

        #endregion

        #region Initialiser

        public AlpacaCamera()
        {
        }

        public AlpacaCamera(ServiceType serviceType,
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
                          ILogger TL
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
            this.TL = TL;

            Initialise();
        }

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
            TL = logger;
            clientNumber = DynamicClientDriver.GetUniqueClientNumber();
            Initialise();
        }
        private void Initialise()
        {
            try
            {
                URIBase = $"{SharedConstants.API_URL_BASE}{SharedConstants.API_VERSION_V1}/{DEVICE_TYPE}/{remoteDeviceNumber}/";
                Version version = Assembly.GetEntryAssembly().GetName().Version;

                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, DEVICE_TYPE, "Alpaca Camera Client starting initialisation, Version: " + version.ToString());
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, DEVICE_TYPE, "This instance's unique client number: " + clientNumber);
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, DEVICE_TYPE, "This devices's base URI: " + URIBase);
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, DEVICE_TYPE, "Establish communications timeout: " + establishConnectionTimeout.ToString());
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, DEVICE_TYPE, "Standard device response timeout: " + standardDeviceResponseTimeout.ToString());
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, DEVICE_TYPE, "Long device response timeout: " + longDeviceResponseTimeout.ToString());
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, DEVICE_TYPE, $"ImageArray transfer type: {imageArrayTransferType}");
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, DEVICE_TYPE, $"ImageArray compression: {imageArrayCompression}");
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, DEVICE_TYPE, $"User name is Null or Empty: {string.IsNullOrEmpty(userName)}, User name is Null or White Space: {string.IsNullOrWhiteSpace(userName)}");
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, DEVICE_TYPE, $"User name length: {password.Length}");
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, DEVICE_TYPE, $"Password is Null or Empty: {string.IsNullOrEmpty(password)}, Password is Null or White Space: {string.IsNullOrWhiteSpace(password)}");
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, DEVICE_TYPE, $"Password length: {password.Length}");

                DynamicClientDriver.ConnectToRemoteDevice(ref client, serviceType, ipAddressString, portNumber, clientNumber, DEVICE_TYPE, standardDeviceResponseTimeout, userName, password, TL);
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, DEVICE_TYPE, "Completed initialisation");
            }
            catch (Exception ex)
            {
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, DEVICE_TYPE, ex.ToString());
            }
        }

        /// <summary>
        /// Configure the device and establish an HTTP connection
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="ipAddressString"></param>
        /// <param name="portNumber"></param>
        /// <param name="remoteDeviceNumber"></param>
        /// <param name="establishConnectionTimeout"></param>
        /// <param name="standardDeviceResponseTimeout"></param>
        /// <param name="longDeviceResponseTimeout"></param>
        /// <param name="clientNumber"></param>
        /// <param name="imageArrayTransferType"></param>
        /// <param name="imageArrayCompression"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="strictCasing"></param>
        /// <param name="TL"></param>
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
                          ILogger TL
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
            this.TL = TL;

            Initialise();
        }


        #endregion

        #region Common properties and methods.

        public string Action(string actionName, string actionParameters)
        {
            DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
            LogMessage(TL, clientNumber, DEVICE_TYPE, $"ACTION: About to submit Action: {actionName}");
            string response = DynamicClientDriver.Action(clientNumber, client, URIBase, strictCasing, TL, actionName, actionParameters);
            LogMessage(TL, clientNumber, DEVICE_TYPE, $"ACTION: Received response of length: {response.Length}");
            return response;
        }

        public void CommandBlind(string command, bool raw = false)
        {
            DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
            DynamicClientDriver.CommandBlind(clientNumber, client, URIBase, strictCasing, TL, command, raw);
        }

        public bool CommandBool(string command, bool raw = false)
        {
            DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
            return DynamicClientDriver.CommandBool(clientNumber, client, URIBase, strictCasing, TL, command, raw);
        }

        public string CommandString(string command, bool raw = false)
        {
            DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
            return DynamicClientDriver.CommandString(clientNumber, client, URIBase, strictCasing, TL, command, raw);
        }

        public bool Connected
        {
            get
            {
                return clientIsConnected;
            }
            set
            {
                clientIsConnected = value;
                if (manageConnectLocally)
                {
                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, DEVICE_TYPE, $"The Connected property is being managed locally so the new value '{value}' will not be sent to the remote device");
                }
                else // Send the command to the remote device
                {
                    DynamicClientDriver.SetClientTimeout(client, establishConnectionTimeout);
                    if (value) DynamicClientDriver.Connect(clientNumber, client, URIBase, strictCasing, TL);
                    else DynamicClientDriver.Disconnect(clientNumber, client, URIBase, strictCasing, TL);
                }
            }
        }

        public string Description
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                string response = DynamicClientDriver.Description(clientNumber, client, URIBase, strictCasing, TL);
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "Description", response);
                return response;
            }
        }

        public string DriverInfo
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.DriverInfo(clientNumber, client, URIBase, strictCasing, TL);
            }
        }

        public string DriverVersion
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.DriverVersion(clientNumber, client, URIBase, strictCasing, TL);
            }
        }

        public short InterfaceVersion
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.InterfaceVersion(clientNumber, client, URIBase, strictCasing, TL);
            }
        }

        public string Name
        {
            get
            {
                string response = DynamicClientDriver.GetValue<string>(clientNumber, client, URIBase, strictCasing, TL, "Name", MemberTypes.Property);
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "Name", response);
                return response;
            }
        }

        public IList<string> SupportedActions
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.SupportedActions(clientNumber, client, URIBase, strictCasing, TL);
            }
        }

        #endregion

        #region ICameraV2 Implementation

        public void AbortExposure()
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, URIBase, strictCasing, TL, "AbortExposure", MemberTypes.Method);
        }

        public void PulseGuide(GuideDirection Direction, int Duration)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { SharedConstants.DIRECTION_PARAMETER_NAME, ((int)Direction).ToString(CultureInfo.InvariantCulture) },
                { SharedConstants.DURATION_PARAMETER_NAME, Duration.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, "PulseGuide", Parameters, Method.PUT, MemberTypes.Method);
        }

        public void StartExposure(double Duration, bool Light)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { SharedConstants.DURATION_PARAMETER_NAME, Duration.ToString(CultureInfo.InvariantCulture) },
                { SharedConstants.LIGHT_PARAMETER_NAME, Light.ToString() }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, "StartExposure", Parameters, Method.PUT, MemberTypes.Method);
        }

        public void StopExposure()
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, URIBase, strictCasing, TL, "StopExposure", MemberTypes.Method);
        }

        public short BinX
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<short>(clientNumber, client, URIBase, strictCasing, TL, "BinX", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetShort(clientNumber, client, URIBase, strictCasing, TL, "BinX", value, MemberTypes.Property);
            }
        }

        public short BinY
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<short>(clientNumber, client, URIBase, strictCasing, TL, "BinY", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetShort(clientNumber, client, URIBase, strictCasing, TL, "BinY", value, MemberTypes.Property);
            }
        }

        public CameraState CameraState
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<CameraState>(clientNumber, client, URIBase, strictCasing, TL, "CameraState", MemberTypes.Property);
            }
        }

        public int CameraXSize
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<int>(clientNumber, client, URIBase, strictCasing, TL, "CameraXSize", MemberTypes.Property);
            }
        }

        public int CameraYSize
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<int>(clientNumber, client, URIBase, strictCasing, TL, "CameraYSize", MemberTypes.Property);
            }
        }

        public bool CanAbortExposure
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanAbortExposure", MemberTypes.Property);
            }
        }

        public bool CanAsymmetricBin
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanAsymmetricBin", MemberTypes.Property);
            }
        }

        public bool CanGetCoolerPower
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanGetCoolerPower", MemberTypes.Property);
            }
        }

        public bool CanPulseGuide
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanPulseGuide", MemberTypes.Property);
            }
        }

        public bool CanSetCCDTemperature
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanSetCCDTemperature", MemberTypes.Property);
            }
        }

        public bool CanStopExposure
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanStopExposure", MemberTypes.Property);
            }
        }

        public double CCDTemperature
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "CCDTemperature", MemberTypes.Property);
            }
        }

        public bool CoolerOn
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CoolerOn", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetBool(clientNumber, client, URIBase, strictCasing, TL, "CoolerOn", value, MemberTypes.Property);
            }
        }

        public double CoolerPower
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "CoolerPower", MemberTypes.Property);
            }
        }

        public double ElectronsPerADU
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "ElectronsPerADU", MemberTypes.Property);
            }
        }

        public double FullWellCapacity
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "FullWellCapacity", MemberTypes.Property);
            }
        }

        public bool HasShutter
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "HasShutter", MemberTypes.Property);
            }
        }

        public double HeatSinkTemperature
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "HeatSinkTemperature", MemberTypes.Property);
            }
        }

        public object ImageArray
        {
            get
            {
                object returnArray;

                try
                {
                    // Special handling for GetBase64Image transfers
                    LogMessage(TL, clientNumber, "ImageArray", $"ImageArray called - canGetBase64Image: {(canGetBase64Image.HasValue ? canGetBase64Image.Value.ToString() : "VALUE NOT SET")}, canGetImageBytes: {(canGetImageBytes.HasValue ? canGetImageBytes.Value.ToString() : "VALUE NOT SET")} , imageArrayTransferType: {imageArrayTransferType}");

                    // Determine whether we need to find out whether Getbase64Image functionality is provided by this driver
                    if (
                        (!canGetBase64Image.HasValue) &
                        ((imageArrayTransferType == ImageArrayTransferType.GetBase64Image) |
                         (imageArrayTransferType == ImageArrayTransferType.BestAvailable) |
                         (imageArrayTransferType == ImageArrayTransferType.GetImageBytes)
                        )
                       )
                    {
                        // Determine whether the remote device supports GetBase64Image
                        // Try to get a SupportedActions response from the device, if anything goes wrong assume that the feature is not available
                        try
                        {
                            // Initialise the supported flag to false
                            canGetBase64Image = false;
                            IList<string> supportedActions = this.SupportedActions;
                            foreach (string action in supportedActions)
                            {
                                // Set the supported flag true if the device advertises that it supports GetBase64Image
                                if (action.ToLowerInvariant() == GETBASE64IMAGE_ACTION_NAME.ToLowerInvariant()) canGetBase64Image = true;
                                LogMessage(TL, clientNumber, "ImageArray", $"Found SupportedAction: {action}, canGetBase64Image: {canGetBase64Image.Value}.");
                            }
                        }
                        catch (Exception ex)
                        {
                            // Just log any errors but otherwise ignore them
                            LogMessage(TL, clientNumber, "ImageArray", $"Received an exception when trying to get the device's SupportedActions: {ex.Message}");
                        }

                        // Determine whether the remote device supports the ImageArrayBytes property
                        // Try to get an answer from the device, if anything goes wrong assume that the feature is not available
                        try
                        {
                            // Initialise the supported flag to false
                            canGetImageBytes = false;

                            // Call the CanGetImageBytes property and store the value
                            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                            canGetImageBytes = DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanImageArrayBytes", MemberTypes.Property);

                            LogMessage(TL, clientNumber, "ImageArray", $"CanImageArrayBytes returned {canGetImageBytes}");
                        }
                        catch (Exception ex)
                        {
                            // Just log any errors but otherwise ignore them
                            LogMessage(TL, clientNumber, "ImageArray", $"Received an exception when calling the device's CanImageArrayBytes property : {ex.Message}");
                        }
                    }

                    // As a precaution, set values false if we have no value at this point
                    if (!canGetBase64Image.HasValue) canGetBase64Image = false;
                    if (!canGetImageBytes.HasValue) canGetImageBytes = false;

                    // Throw an exception if GetBase64Image mode is explicitly requested but the device does not support this mode
                    if ((imageArrayTransferType == ImageArrayTransferType.GetBase64Image) & !canGetBase64Image.Value) throw new InvalidOperationException("GetBase64Image transfer mode has been requested but the device does not support this mode.");

                    // Throw an exception if GetBase64Image mode is explicitly requested but the device does not support this mode
                    if ((imageArrayTransferType == ImageArrayTransferType.GetImageBytes) & !canGetImageBytes.Value) throw new InvalidOperationException("GetImageBytes transfer mode has been requested but the device does not support this mode.");

                    // Use a fast transfer mode if possible
                    LogMessage(TL, clientNumber, "ImageArray", $"ImageArray 1 called - canGetBase64Image: {canGetBase64Image.Value}, canGetImageBytes: {canGetImageBytes.Value} , imageArrayTransferType: {imageArrayTransferType}");

                    // Use the GetImageBytes mechanic if available
                    if (canGetImageBytes.Value & ((imageArrayTransferType == ImageArrayTransferType.GetImageBytes) | (imageArrayTransferType == ImageArrayTransferType.BestAvailable)))
                    {
                        Stopwatch sw = new Stopwatch();
                        Stopwatch swOverall = new Stopwatch();

                        // Configure communications
                        sw.Start();
                        swOverall.Start();
                        //RestRequest request = new RestRequest((URIBase + "ImageArrayBytes").ToLowerInvariant(), Method.GET)
                        //{
                        //    RequestFormat = DataFormat.None
                        //};
                        //client.ConfigureWebRequest(wr => wr.AutomaticDecompression = DecompressionMethods.None); // Prevent any decompression

                        //// Add the transaction number and client ID parameters
                        //uint transaction = DynamicClientDriver.TransactionNumber();
                        //request.AddParameter(SharedConstants.CLIENTTRANSACTION_PARAMETER_NAME, transaction.ToString());
                        //request.AddParameter(SharedConstants.CLIENTID_PARAMETER_NAME, clientNumber.ToString());

                        //// Download the binary byte[] data
                        string imageArrayBytesUri = $"{serviceType.ToString().ToLowerInvariant()}://{ipAddressString}:{portNumber}{URIBase}ImageArrayBytes".ToLowerInvariant();
                        LogMessage(TL, clientNumber, "ImageArrayBytes", $"Downloading byte[] from {imageArrayBytesUri}");

                        // Create a task to download the byte[]
                        Task<byte[]> getByteArrayTask = GetByteArray(imageArrayBytesUri);

                        // Wait for the task to complete
                        Task.WaitAll(getByteArrayTask);

                        // Get the byte array from the task
                        byte[] imageBytes = getByteArrayTask.Result;

                        sw.Stop();
                        LogMessage(TL, clientNumber, "ImageArrayBytes", $"Downloaded {imageBytes.Length} bytes in {sw.ElapsedMilliseconds}ms.");

                        LogMessage(TL, clientNumber, "ImageArrayBytes", $"Metadata bytes: " +
                            $"[ {imageBytes[0]:X2} {imageBytes[1]:X2} {imageBytes[2]:X2} {imageBytes[3]:X2} ] [ {imageBytes[4]:X2} {imageBytes[5]:X2} {imageBytes[6]:X2} {imageBytes[7]:X2} ] " +
                            $"[ {imageBytes[8]:X2} {imageBytes[9]:X2} {imageBytes[10]:X2} {imageBytes[11]:X2} ] [ {imageBytes[12]:X2} {imageBytes[13]:X2} {imageBytes[14]:X2} {imageBytes[15]:X2} ] " +
                            $"[ {imageBytes[16]:X2} {imageBytes[17]:X2} {imageBytes[18]:X2} {imageBytes[19]:X2} ] [ {imageBytes[20]:X2} {imageBytes[21]:X2} {imageBytes[22]:X2} {imageBytes[23]:X2} ] " +
                            $"[ {imageBytes[24]:X2} {imageBytes[25]:X2} {imageBytes[26]:X2} {imageBytes[27]:X2} ] [ {imageBytes[28]:X2} {imageBytes[29]:X2} {imageBytes[30]:X2} {imageBytes[31]:X2} ] " +
                            $"[ {imageBytes[32]:X2} {imageBytes[33]:X2} {imageBytes[34]:X2} {imageBytes[35]:X2} ]");

                        int metadataVersion = imageBytes.GetMetadataVersion();
                        LogMessage(TL, clientNumber, "ImageArrayBytes", $"Metadata version: {metadataVersion}");

                        AlpacaErrors errorNumber;
                        switch (metadataVersion)
                        {
                            case 1:
                                ArrayMetadataV1 metadataV1 = imageBytes.GetMetadataV1();
                                LogMessage(TL, clientNumber, "ImageArrayBytes", $"Received array: Metadata version: {metadataV1.MetadataVersion} Image element type: {metadataV1.ImageElementType} Transmission element type: {metadataV1.TransmissionElementType} Array rank: {metadataV1.Rank} Dimension 0: {metadataV1.Dimension0} Dimension 1: {metadataV1.Dimension1} Dimension 2: {metadataV1.Dimension2} Error number: {metadataV1.ErrorNumber}.");
                                errorNumber = metadataV1.ErrorNumber;
                                break;

                            default:
                                throw new InvalidValueException($"ImageArray - ImageArrayBytes - Received an unsupported metadata version number: {metadataVersion} from the Alpaca device.");
                        }

                        // Handle success or failure of the call
                        if (errorNumber == AlpacaErrors.AlpacaNoError) // Success so return the image array
                        {
                            // Convert the byte[] back to an image array
                            sw.Restart();
                            returnArray = imageBytes.ToImageArray();
                            LogMessage(TL, clientNumber, "ImageArrayBytes", $"Converted byte[] to image array in {sw.ElapsedMilliseconds}ms. Overall time: {swOverall.ElapsedMilliseconds}ms.");
                            return returnArray;
                        }
                        else // An error occurred so throw an exception
                        {
                            string errorMessage = imageBytes.GetErrrorMessage();
                            LogMessage(TL, clientNumber, "ImageArrayBytes", $"The Alpaca device returned an error: {errorNumber} - {errorMessage}.");
                            throw ExceptionHelpers.ExceptionFromErrorCode(errorNumber, errorMessage);
                        }

                    }
                    else if (canGetBase64Image.Value & ((imageArrayTransferType == ImageArrayTransferType.GetBase64Image) | (imageArrayTransferType == ImageArrayTransferType.BestAvailable)))
                    // Fall back to GetBase64Image if available
                    {
                        Stopwatch sw = new Stopwatch();
                        Stopwatch swOverall = new Stopwatch();

                        // Call the GetBase64Image Action method to retrieve the image in base64 encoded form
                        swOverall.Start();
                        sw.Start();
                        string base64String = this.Action(AlpacaTools.BASE64RESPONSE_COMMAND_NAME, "");
                        sw.Stop();
                        LogMessage(TL, clientNumber, "GetBase64Image", $"Received {base64String.Length} bytes in {sw.ElapsedMilliseconds}ms.");

                        // Convert from base 64 encoding to byte[]
                        sw.Restart();
                        byte[] base64ArrayByteArray = Convert.FromBase64String(base64String);
                        sw.Stop();
                        LogMessage(TL, clientNumber, "GetBase64Image", $"Converted string to byte array in {sw.ElapsedMilliseconds}ms.");


                        int metadataVersion = base64ArrayByteArray.GetMetadataVersion();
                        LogMessage(TL, clientNumber, "GetBase64Image", $"Metadata version: {metadataVersion}");

                        switch (metadataVersion)
                        {
                            case 1:
                                ArrayMetadataV1 metadataV1 = base64ArrayByteArray.GetMetadataV1();
                                LogMessage(TL, clientNumber, "GetBase64Image", $"Received array: Metadata version: {metadataV1.MetadataVersion} Image element type: {metadataV1.ImageElementType} Transmission element type: {metadataV1.TransmissionElementType} Array rank: {metadataV1.Rank} Dimension 0: {metadataV1.Dimension0} Dimension 1: {metadataV1.Dimension1} Dimension 2: {metadataV1.Dimension2}");
                                break;

                            default:
                                throw new InvalidValueException($"GetBase64Image - ImageArrayBytes - Received an unsupported metadata version number: {metadataVersion} from the Alpaca device.");
                        }

                        // Convert the byte[] back to an image array
                        sw.Restart();
                        returnArray = base64ArrayByteArray.ToImageArray();
                        LogMessage(TL, clientNumber, "GetBase64Image", $"Converted byte[] to image array in {sw.ElapsedMilliseconds}ms. Overall time: {swOverall.ElapsedMilliseconds}ms.");

                        return returnArray;
                    }
                    else if (!canGetImageBytes.Value & (imageArrayTransferType == ImageArrayTransferType.GetImageBytes)) // Throw an exception if GetImageBytes is specified but not available
                    {
                        throw new InvalidOperationException("ImageArrayBytes - The GetImageBytes transfer mechanic was specified but is not supported by this device.");
                    }
                    else if (!canGetBase64Image.Value & (imageArrayTransferType == ImageArrayTransferType.GetBase64Image)) // Throw an exception if GetBase64Image is specified but not available
                    {
                        throw new InvalidOperationException("GetBase64Image - The GetBase64Image transfer mechanic was specified but is not supported by this device.");
                    }
                    else
                    // Fall back to Base64 hand-off if available, otherwise use JSON
                    {
                        DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
                        return DynamicClientDriver.GetValue<Array>(clientNumber, client, URIBase, strictCasing, TL, "ImageArray", imageArrayTransferType, imageArrayCompression, MemberTypes.Property);
                    }
                }
                catch (Exception ex)
                {
                    LogMessage(TL, clientNumber, "ImageArray", $"CameraBaseClass.ImageArray exception: {ex}");
                    throw;
                }
            }
        }

        public object ImageArrayVariant
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
                return DynamicClientDriver.ImageArrayVariant(clientNumber, client, URIBase, strictCasing, TL, imageArrayTransferType, imageArrayCompression);
            }
        }

        public bool ImageReady
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "ImageReady", MemberTypes.Property);
            }
        }

        public bool IsPulseGuiding
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "IsPulseGuiding", MemberTypes.Property);
            }
        }

        public double LastExposureDuration
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "LastExposureDuration", MemberTypes.Property);
            }
        }

        public string LastExposureStartTime
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<string>(clientNumber, client, URIBase, strictCasing, TL, "LastExposureStartTime", MemberTypes.Property);
            }
        }

        public int MaxADU
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<int>(clientNumber, client, URIBase, strictCasing, TL, "MaxADU", MemberTypes.Property);
            }
        }

        public short MaxBinX
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<short>(clientNumber, client, URIBase, strictCasing, TL, "MaxBinX", MemberTypes.Property);
            }
        }

        public short MaxBinY
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<short>(clientNumber, client, URIBase, strictCasing, TL, "MaxBinY", MemberTypes.Property);
            }
        }

        public int NumX
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<int>(clientNumber, client, URIBase, strictCasing, TL, "NumX", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetInt(clientNumber, client, URIBase, strictCasing, TL, "NumX", value, MemberTypes.Property);
            }
        }

        public int NumY
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<int>(clientNumber, client, URIBase, strictCasing, TL, "NumY", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetInt(clientNumber, client, URIBase, strictCasing, TL, "NumY", value, MemberTypes.Property);
            }
        }

        public double PixelSizeX
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "PixelSizeX", MemberTypes.Property);
            }
        }

        public double PixelSizeY
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "PixelSizeY", MemberTypes.Property);
            }
        }

        public double SetCCDTemperature
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "SetCCDTemperature", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetDouble(clientNumber, client, URIBase, strictCasing, TL, "SetCCDTemperature", value, MemberTypes.Property);
            }
        }

        public int StartX
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<int>(clientNumber, client, URIBase, strictCasing, TL, "StartX", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetInt(clientNumber, client, URIBase, strictCasing, TL, "StartX", value, MemberTypes.Property);
            }
        }

        public int StartY
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<int>(clientNumber, client, URIBase, strictCasing, TL, "StartY", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetInt(clientNumber, client, URIBase, strictCasing, TL, "StartY", value, MemberTypes.Property);
            }
        }

        public short BayerOffsetX
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<short>(clientNumber, client, URIBase, strictCasing, TL, "BayerOffsetX", MemberTypes.Property);
            }
        }

        public short BayerOffsetY
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<short>(clientNumber, client, URIBase, strictCasing, TL, "BayerOffsetY", MemberTypes.Property);
            }
        }

        public bool CanFastReadout
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanFastReadout", MemberTypes.Property);
            }
        }

        public double ExposureMax
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "ExposureMax", MemberTypes.Property);
            }
        }

        public double ExposureMin
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "ExposureMin", MemberTypes.Property);
            }
        }

        public double ExposureResolution
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "ExposureResolution", MemberTypes.Property);
            }
        }

        public bool FastReadout
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "FastReadout", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetBool(clientNumber, client, URIBase, strictCasing, TL, "FastReadout", value, MemberTypes.Property);
            }
        }

        public short Gain
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<short>(clientNumber, client, URIBase, strictCasing, TL, "Gain", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetShort(clientNumber, client, URIBase, strictCasing, TL, "Gain", value, MemberTypes.Property);
            }
        }

        public short GainMax
        {
            get
            {
                return DynamicClientDriver.GetValue<short>(clientNumber, client, URIBase, strictCasing, TL, "GainMax", MemberTypes.Property);
            }
        }

        public short GainMin
        {
            get
            {
                return DynamicClientDriver.GetValue<short>(clientNumber, client, URIBase, strictCasing, TL, "GainMin", MemberTypes.Property);
            }
        }

        public IList<string> Gains
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                List<string> gains = DynamicClientDriver.GetValue<List<string>>(clientNumber, client, URIBase, strictCasing, TL, "Gains", MemberTypes.Property);
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "Gains", $"Returning {gains.Count} gains");

                return gains;
            }
        }

        public short PercentCompleted
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<short>(clientNumber, client, URIBase, strictCasing, TL, "PercentCompleted", MemberTypes.Property);
            }
        }

        public short ReadoutMode
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<short>(clientNumber, client, URIBase, strictCasing, TL, "ReadoutMode", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetShort(clientNumber, client, URIBase, strictCasing, TL, "ReadoutMode", value, MemberTypes.Property);
            }
        }

        public IList<string> ReadoutModes
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                List<string> modes = DynamicClientDriver.GetValue<List<string>>(clientNumber, client, URIBase, strictCasing, TL, "ReadoutModes", MemberTypes.Property);
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "ReadoutModes", $"Returning {modes.Count} modes");

                return modes;
            }
        }

        public string SensorName
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<string>(clientNumber, client, URIBase, strictCasing, TL, "SensorName", MemberTypes.Property);
            }
        }

        public SensorType SensorType
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<SensorType>(clientNumber, client, URIBase, strictCasing, TL, "SensorType", MemberTypes.Property);
            }
        }

        #endregion

        #region ICameraV3 implementation

        public int Offset
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<int>(clientNumber, client, URIBase, strictCasing, TL, "Offset", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetInt(clientNumber, client, URIBase, strictCasing, TL, "Offset", value, MemberTypes.Property);
            }
        }

        public int OffsetMax
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, URIBase, strictCasing, TL, "OffsetMax", MemberTypes.Property);
            }
        }

        public int OffsetMin
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, URIBase, strictCasing, TL, "OffsetMin", MemberTypes.Property);
            }
        }

        public IList<string> Offsets
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                List<string> offsets = DynamicClientDriver.GetValue<List<string>>(clientNumber, client, URIBase, strictCasing, TL, "Offsets", MemberTypes.Property);
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "Offsets", $"Returning {offsets.Count} Offsets");

                return offsets;
            }
        }

        public double SubExposureDuration
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "SubExposureDuration", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetDouble(clientNumber, client, URIBase, strictCasing, TL, "SubExposureDuration", value, MemberTypes.Property);
            }
        }

        #endregion

        #region Support code

        private async Task<byte[]> GetByteArray(string url)
        {
            HttpClient wClient = new HttpClient();
            byte[] imageBytes = await wClient.GetByteArrayAsync(url);
            return imageBytes;
        }

        #endregion
    }
}
