using ASCOM.Common.Alpaca;
using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.Interfaces;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        public AlpacaCoverCalibrator()
        {
            Initialise();
        }

        public AlpacaCoverCalibrator(ServiceType serviceType,
                          string ipAddressString,
                          int portNumber,
                          int remoteDeviceNumber,
                          int establishConnectionTimeout,
                          int standardDeviceResponseTimeout,
                          int longDeviceResponseTimeout,
                          uint clientNumber,
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
            this.userName = userName;
            this.password = password;
            this.strictCasing = strictCasing;
            this.TL = TL;

            Initialise();
        }

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
            TL = logger;
            clientNumber = DynamicClientDriver.GetUniqueClientNumber();
            Initialise();
        }
        private void Initialise()
        {
            try
            {
                // Set the device type
                clientDeviceType = "CoverCalibrator";

                URIBase = $"{AlpacaConstants.API_URL_BASE}{AlpacaConstants.API_VERSION_V1}/{clientDeviceType}/{remoteDeviceNumber}/";
                Version version = Assembly.GetEntryAssembly().GetName().Version;

                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, "Starting initialisation, Version: " + version.ToString());
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, "This instance's unique client number: " + clientNumber);
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, "This devices's base URI: " + URIBase);
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, "Establish communications timeout: " + establishConnectionTimeout.ToString());
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, "Standard device response timeout: " + standardDeviceResponseTimeout.ToString());
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, "Long device response timeout: " + longDeviceResponseTimeout.ToString());
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

        #endregion

        #region ICoverCalibratorV1 Implementation

        public CoverStatus CoverState
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<CoverStatus>(clientNumber, client, URIBase, strictCasing, TL, "CoverState", MemberTypes.Property);
            }
        }

        public CalibratorStatus CalibratorState
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<CalibratorStatus>(clientNumber, client, URIBase, strictCasing, TL, "CalibratorState", MemberTypes.Property);
            }
        }

        public int Brightness
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<int>(clientNumber, client, URIBase, strictCasing, TL, "Brightness", MemberTypes.Property);
            }
        }

        public int MaxBrightness
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<int>(clientNumber, client, URIBase, strictCasing, TL, "MaxBrightness", MemberTypes.Property);
            }
        }

        public void OpenCover()
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, URIBase, strictCasing, TL, "OpenCover", MemberTypes.Method);
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "AbortSlew", "Cover opened OK");
        }

        public void CloseCover()
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, URIBase, strictCasing, TL, "CloseCover", MemberTypes.Method);
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "AbortSlew", "Cover closed OK");
        }

        public void HaltCover()
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, URIBase, strictCasing, TL, "HaltCover", MemberTypes.Method);
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "AbortSlew", "Cover halted OK");
        }

        public void CalibratorOn(int Brightness)
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.BRIGHTNESS_PARAMETER_NAME, Brightness.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, "CalibratorOn", Parameters, Method.PUT, MemberTypes.Method);
        }

        public void CalibratorOff()
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, URIBase, strictCasing, TL, "CalibratorOff", MemberTypes.Method);
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "AbortSlew", $"Calibrator off OK");
        }

        #endregion

    }
}