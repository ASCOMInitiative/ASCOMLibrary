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
    /// ASCOM Alpaca Camera client
    /// </summary>
    public class AlpacaCamera : AlpacaDeviceBaseClass, ICameraV3
    {
        #region Variables and Constants

        // Variables specific to this device type
        private ImageArrayTransferType imageArrayTransferType = ImageArrayTransferType.JSON;
        private ImageArrayCompression imageArrayCompression = ImageArrayCompression.None;

        #endregion

        #region Initialiser

        public AlpacaCamera()
        {
            Initialise();
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
                // Set the device type
                clientDeviceType = "Camera";

                URIBase = $"{AlpacaConstants.API_URL_BASE}{AlpacaConstants.API_VERSION_V1}/{clientDeviceType}/{remoteDeviceNumber}/";
                Version version = Assembly.GetEntryAssembly().GetName().Version;

                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, "Alpaca Camera Client starting initialisation, Version: " + version.ToString());
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, "This instance's unique client number: " + clientNumber);
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, "This devices's base URI: " + URIBase);
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, "Establish communications timeout: " + establishConnectionTimeout.ToString());
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, "Standard device response timeout: " + standardDeviceResponseTimeout.ToString());
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, "Long device response timeout: " + longDeviceResponseTimeout.ToString());
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, $"ImageArray transfer type: {imageArrayTransferType}");
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, $"ImageArray compression: {imageArrayCompression}");
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, $"User name is Null or Empty: {string.IsNullOrEmpty(userName)}, User name is Null or White Space: {string.IsNullOrWhiteSpace(userName)}");
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, $"User name length: {password.Length}");
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, $"Password is Null or Empty: {string.IsNullOrEmpty(password)}, Password is Null or White Space: {string.IsNullOrWhiteSpace(password)}");
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, $"Password length: {password.Length}");

                DynamicClientDriver.ConnectToRemoteDevice(ref client, serviceType, ipAddressString, portNumber, clientNumber, clientDeviceType, standardDeviceResponseTimeout, userName, password, TL);
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, "Completed initialisation");
            }
            catch (Exception ex)
            {
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, ex.ToString());
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
                { AlpacaConstants.DIRECTION_PARAMETER_NAME, ((int)Direction).ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.DURATION_PARAMETER_NAME, Duration.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, "PulseGuide", Parameters, Method.PUT, MemberTypes.Method);
        }

        public void StartExposure(double Duration, bool Light)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.DURATION_PARAMETER_NAME, Duration.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.LIGHT_PARAMETER_NAME, Light.ToString() }
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
                DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<Array>(clientNumber, client, URIBase, strictCasing, TL, AlpacaConstants.IMAGE_ARRAY_METHOD_NAME, imageArrayTransferType, imageArrayCompression, MemberTypes.Property);
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
