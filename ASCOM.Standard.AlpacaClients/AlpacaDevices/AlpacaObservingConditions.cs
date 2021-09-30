using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// ASCOM DynamicRemoteClients ObservingConditions base class
    /// </summary>
    public class AlpacaObservingConditions : AlpacaDeviceBaseClass, IObservingConditions
    {
        #region Variables and Constants

        // Set the device type
        private const string DEVICE_TYPE = "ObservingConditions";

        #endregion

        #region Initialiser

        public AlpacaObservingConditions()
        {
            Initialise();
        }

        public AlpacaObservingConditions(ServiceType serviceType,
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

                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, DEVICE_TYPE, "Starting initialisation, Version: " + version.ToString());
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, DEVICE_TYPE, "This instance's unique client number: " + clientNumber);
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, DEVICE_TYPE, "This devices's base URI: " + URIBase);
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, DEVICE_TYPE, "Establish communications timeout: " + establishConnectionTimeout.ToString());
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, DEVICE_TYPE, "Standard device response timeout: " + standardDeviceResponseTimeout.ToString());
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, DEVICE_TYPE, "Long device response timeout: " + longDeviceResponseTimeout.ToString());
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

        #endregion

        #region Common properties and methods.

        public string Action(string actionName, string actionParameters)
        {
            DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
            return DynamicClientDriver.Action(clientNumber, client, URIBase, strictCasing, TL, actionName, actionParameters);
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

        #region IObservingConditions Implementation

        public double TimeSinceLastUpdate(string PropertyName)
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            return DynamicClientDriver.GetStringIndexedDouble(clientNumber, client, URIBase, strictCasing, TL, "TimeSinceLastUpdate", PropertyName, MemberTypes.Method);
        }

        public string SensorDescription(string PropertyName)
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            return DynamicClientDriver.GetStringIndexedString(clientNumber, client, URIBase, strictCasing, TL, "SensorDescription", PropertyName, MemberTypes.Method);
        }

        public void Refresh()
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, URIBase, strictCasing, TL, "Refresh", MemberTypes.Method);
        }

        public double AveragePeriod
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "AveragePeriod", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetDouble(clientNumber, client, URIBase, strictCasing, TL, "AveragePeriod", value, MemberTypes.Property);
            }
        }

        public double CloudCover
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "CloudCover", MemberTypes.Property);
            }
        }

        public double DewPoint
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "DewPoint", MemberTypes.Property);
            }
        }

        public double Humidity
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "Humidity", MemberTypes.Property);
            }
        }

        public double Pressure
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "Pressure", MemberTypes.Property);
            }
        }

        public double RainRate
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "RainRate", MemberTypes.Property);
            }
        }

        public double SkyBrightness
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "SkyBrightness", MemberTypes.Property);
            }
        }

        public double SkyQuality
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "SkyQuality", MemberTypes.Property);
            }
        }

        public double StarFWHM
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "StarFWHM", MemberTypes.Property);
            }
        }

        public double SkyTemperature
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "SkyTemperature", MemberTypes.Property);
            }
        }

        public double Temperature
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "Temperature", MemberTypes.Property);
            }
        }

        public double WindDirection
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "WindDirection", MemberTypes.Property);
            }
        }

        public double WindGust
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "WindGust", MemberTypes.Property);
            }
        }

        public double WindSpeed
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "WindSpeed", MemberTypes.Property);
            }
        }

        #endregion

    }
}