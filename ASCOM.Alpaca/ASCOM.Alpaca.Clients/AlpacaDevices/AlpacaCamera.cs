using ASCOM.Common.Alpaca;
using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.Interfaces;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
                // Special handling for Getbase64Image transfers
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, DEVICE_TYPE, $"CameraBaseClass.ImageArray called...");
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, DEVICE_TYPE, $"CameraBaseClass.ImageArray called - canGetBase64Image.HasValue: {canGetBase64Image.HasValue}, imageArrayTransferType: {imageArrayTransferType}");

                // Determine whether we need to find out whether Getbase64Image functionality is provided by this driver
                if ((!canGetBase64Image.HasValue) & ((imageArrayTransferType == ImageArrayTransferType.GetBase64Image) | (imageArrayTransferType == ImageArrayTransferType.BestAvailable)))
                {
                    // Try to get an answer from the device, if anything goes wrong assume that the feature is not available
                    try
                    {
                        // Initialise the supported flag to false
                        canGetBase64Image = false;
                        IList<string> supportedActions = this.SupportedActions;
                        foreach (string action in supportedActions)
                        {
                            // Set the supported flag true if the device advertises that it supports GetBase64Image
                            if (action.ToLowerInvariant() == GETBASE64IMAGE_ACTION_NAME.ToLowerInvariant()) canGetBase64Image = true;
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "CameraBaseClass.ImageArray", $"CameraBaseClass.ImageArray Found action: {action}, canGetBase64Image: {canGetBase64Image.Value}");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Just log any errors but otherwise ignore them
                        AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "CameraBaseClass.ImageArray", $"Received an exception when trying to get the device's SupportedActions: {ex.Message}");
                    }
                }
                if (!canGetBase64Image.HasValue) canGetBase64Image = false; // Set false if we have no value at this point

                // Throw an exception if GetBase64Image mode is explicitly requested but the device does not support this mode
                if (imageArrayTransferType == ImageArrayTransferType.GetBase64Image & !canGetBase64Image.Value) throw new InvalidOperationException("GetBase64Image transfer mode has been requested by the device does not support this mode.");

                // Use GetBase64Image mode because it is definitely supported 
                if (canGetBase64Image.Value)
                {
                    Stopwatch sw = new Stopwatch();
                    Stopwatch swOverall = new Stopwatch();
                    swOverall.Start();
                    sw.Start();

                    // Call the GetBase64Image Action method to retrieve the image in base64 encoded form
                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "CameraBaseClass.ImageArray", $"Submitting Action command...");
                    string base64String = this.Action("GetBase64Image", "");
                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "CameraBaseClass.ImageArray", $"Received response!");
                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "CameraBaseClass.ImageArray", $"Received {base64String.Length} bytes in {sw.ElapsedMilliseconds}ms.");

                    sw.Restart();
                    byte[] base64ArrayByteArray = Convert.FromBase64String(base64String);
                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "CameraBaseClass.ImageArray", $"Converted string to byte array in {sw.ElapsedMilliseconds}ms.");

                    // Set the array type, rank and dimensions
                    int version = BitConverter.ToInt32(base64ArrayByteArray, 0);
                    ImageArrayElementTypes outputType = (ImageArrayElementTypes)BitConverter.ToInt32(base64ArrayByteArray, 4);
                    ImageArrayElementTypes transmissionType = (ImageArrayElementTypes)BitConverter.ToInt32(base64ArrayByteArray, 8);
                    int rank = BitConverter.ToInt32(base64ArrayByteArray, 12);
                    int dimension0 = BitConverter.ToInt32(base64ArrayByteArray, 16);
                    int dimension1 = BitConverter.ToInt32(base64ArrayByteArray, 20);
                    int dimension2 = BitConverter.ToInt32(base64ArrayByteArray, 24);
                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "CameraBaseClass.ImageArray", $"Version: {version}, Output Type: {outputType}, Transmission Type: {transmissionType}, Rank: {rank}, Dimension 0: {dimension0}, Dimension 1: {dimension1}, Dimension 2: {dimension2}");

                    // Validate returned metadata values
                    if (version != GETBASE64IMAGE_SUPPORTED_VERSION) throw new InvalidValueException($"GetBase64Image - The device returned an unsupported version: {version}, this Alpaca client supports version: {GETBASE64IMAGE_SUPPORTED_VERSION}");

                    sw.Restart();
                    // Convert the returned byte[] into the form that the client is expecting
                    if ((outputType == ImageArrayElementTypes.Int32) & (transmissionType == ImageArrayElementTypes.Int16)) // Handle the special case where Int32 has been converted to Int16 for transmission
                    {
                        switch (rank)
                        {
                            case 2: // Rank 2
                                short[,] short2dArray = new short[dimension0, dimension1];
                                Buffer.BlockCopy(base64ArrayByteArray, 48, short2dArray, 0, base64ArrayByteArray.Length - 48);
                                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "CameraBaseClass.ImageArray", $"Completed block copy of {base64ArrayByteArray.Length} bytes in {sw.ElapsedMilliseconds}ms");

                                int[,] int2dArray = new int[dimension0, dimension1];
                                Parallel.For(0, short2dArray.GetLength(0) - 1, (i) =>
                                {
                                    Parallel.For(0, short2dArray.GetLength(1) - 1, (j) =>
                                    {
                                        int2dArray[i, j] = short2dArray[i, j];
                                    });
                                });
                                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "CameraBaseClass.ImageArray", $"CONVERTED 2D INT16 ARRAY TO INT32 ARRAY - GetBase64Image time: {swOverall.ElapsedMilliseconds}ms, Input length: {short2dArray.Length}, Output length: {int2dArray.Length}");
                                return int2dArray;

                            case 3: // Rank 3
                                short[,,] short3dArray = new short[dimension0, dimension1, dimension2];
                                Buffer.BlockCopy(base64ArrayByteArray, 48, short3dArray, 0, base64ArrayByteArray.Length - 48);
                                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "CameraBaseClass.ImageArray", $"Completed block copy of {base64ArrayByteArray.Length} bytes in {sw.ElapsedMilliseconds}ms");

                                int[,,] int3dArray = new int[dimension0, dimension1, dimension2];
                                Parallel.For(0, short3dArray.GetLength(2) - 1, (k) =>
                                {
                                    Parallel.For(0, short3dArray.GetLength(1) - 1, (j) =>
                                    {
                                        Parallel.For(0, short3dArray.GetLength(0) - 1, (i) =>
                                        {
                                            int3dArray[i, j, k] = short3dArray[i, j, k];
                                        });
                                    });
                                });
                                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "CameraBaseClass.ImageArray", $"CONVERTED 3D INT16 ARRAY TO INT32 ARRAY - GetBase64Image time: {swOverall.ElapsedMilliseconds}ms, Input length: {short3dArray.Length}, Output length: {int3dArray.Length}");
                                return int3dArray;

                            default:
                                throw new InvalidValueException($"CameraBaseClass.ImageArray - Returned array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                        }
                    }
                    else // Handle all other cases where the expected array type and the transmitted array type are the same
                    {
                        if (outputType == transmissionType) // Required and transmitted array element types are the same
                        {
                            switch (outputType)
                            {
                                case ImageArrayElementTypes.Byte:
                                    switch (rank)
                                    {
                                        case 2: // Rank 2
                                            byte[,] byte2dArray = new byte[dimension0, dimension1];
                                            Buffer.BlockCopy(base64ArrayByteArray, 48, byte2dArray, 0, base64ArrayByteArray.Length - 48);
                                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "CameraBaseClass.ImageArray", $"Completed byte[,] block copy of {base64ArrayByteArray.Length} bytes in {sw.ElapsedMilliseconds}ms");
                                            return byte2dArray;

                                        case 3: // Rank 3
                                            byte[,,] byte3dArray = new byte[dimension0, dimension1, dimension2];
                                            Buffer.BlockCopy(base64ArrayByteArray, 48, byte3dArray, 0, base64ArrayByteArray.Length - 48);
                                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "CameraBaseClass.ImageArray", $"Completed byte[,,] block copy of {base64ArrayByteArray.Length} bytes in {sw.ElapsedMilliseconds}ms");
                                            return byte3dArray;

                                        default:
                                            throw new InvalidValueException($"ImageArray - Returned byte array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                                    }

                                case ImageArrayElementTypes.Int16:
                                    switch (rank)
                                    {
                                        case 2: // Rank 2
                                            short[,] short2dArray = new short[dimension0, dimension1];
                                            Buffer.BlockCopy(base64ArrayByteArray, 48, short2dArray, 0, base64ArrayByteArray.Length - 48);
                                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "CameraBaseClass.ImageArray", $"Completed Int16[,] block copy of {base64ArrayByteArray.Length} bytes in {sw.ElapsedMilliseconds}ms");
                                            return short2dArray;

                                        case 3: // Rank 3
                                            short[,,] short3dArray = new short[dimension0, dimension1, dimension2];
                                            Buffer.BlockCopy(base64ArrayByteArray, 48, short3dArray, 0, base64ArrayByteArray.Length - 48);
                                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "CameraBaseClass.ImageArray", $"Completed Int16[,,] block copy of {base64ArrayByteArray.Length} bytes in {sw.ElapsedMilliseconds}ms");
                                            return short3dArray;

                                        default:
                                            throw new InvalidValueException($"ImageArray - Returned Int16 array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                                    }

                                case ImageArrayElementTypes.Int32:
                                    switch (rank)
                                    {
                                        case 2: // Rank 2
                                            int[,] int2dArray = new int[dimension0, dimension1];
                                            Buffer.BlockCopy(base64ArrayByteArray, 48, int2dArray, 0, base64ArrayByteArray.Length - 48);
                                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "CameraBaseClass.ImageArray", $"Completed Int32[,] block copy of {base64ArrayByteArray.Length} bytes in {sw.ElapsedMilliseconds}ms");
                                            return int2dArray;

                                        case 3: // Rank 3
                                            int[,,] int3dArray = new int[dimension0, dimension1, dimension2];
                                            Buffer.BlockCopy(base64ArrayByteArray, 48, int3dArray, 0, base64ArrayByteArray.Length - 48);
                                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "CameraBaseClass.ImageArray", $"Completed Int32[,,] block copy of {base64ArrayByteArray.Length} bytes in {sw.ElapsedMilliseconds}ms");
                                            return int3dArray;

                                        default:
                                            throw new InvalidValueException($"ImageArray - Returned Int32 array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                                    }

                                case ImageArrayElementTypes.Int64:
                                    switch (rank)
                                    {
                                        case 2: // Rank 2
                                            Int64[,] int642dArray = new Int64[dimension0, dimension1];
                                            Buffer.BlockCopy(base64ArrayByteArray, 48, int642dArray, 0, base64ArrayByteArray.Length - 48);
                                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "CameraBaseClass.ImageArray", $"Completed Int64[,] block copy of {base64ArrayByteArray.Length} bytes in {sw.ElapsedMilliseconds}ms");
                                            return int642dArray;

                                        case 3: // Rank 3
                                            Int64[,,] int643dArray = new Int64[dimension0, dimension1, dimension2];
                                            Buffer.BlockCopy(base64ArrayByteArray, 48, int643dArray, 0, base64ArrayByteArray.Length - 48);
                                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "CameraBaseClass.ImageArray", $"Completed Int64[,,] block copy of {base64ArrayByteArray.Length} bytes in {sw.ElapsedMilliseconds}ms");
                                            return int643dArray;

                                        default:
                                            throw new InvalidValueException($"ImageArray - Returned Int64 array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                                    }

                                default:
                                    throw new InvalidValueException($"The device has returned an unsupported image array element type: {outputType}.");

                            }
                        }
                        else // An unsupported combination of array element types has been returned
                        {
                            throw new InvalidValueException($"The device has returned an unsupported combination of Output type: {outputType} and Transmission type: {transmissionType}.");
                        }
                    }
                }
                else
                {

                    DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
                    return DynamicClientDriver.GetValue<Array>(clientNumber, client, URIBase, strictCasing, TL, "ImageArray", imageArrayTransferType, imageArrayCompression, MemberTypes.Property);
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

    }
}
