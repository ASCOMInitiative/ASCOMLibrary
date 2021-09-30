using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using ASCOM.Standard.Interfaces;
using RestSharp;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// ASCOM DynamicRemoteClients Switch base class
    /// </summary>
    public class AlpacaSwitch : AlpacaDeviceBaseClass, ISwitchV2
    {
        #region Variables and Constants

        // Set the device type
        private const string DEVICE_TYPE = "Switch";

        #endregion

        #region Initialiser

        public AlpacaSwitch()
        {
            Initialise();
        }

        public AlpacaSwitch(ServiceType serviceType,
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

        public AlpacaSwitch(ServiceType serviceType,
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
                    AlpacaDeviceBaseClass.LogMessage(TL,clientNumber, DEVICE_TYPE, $"The Connected property is being managed locally so the new value '{value}' will not be sent to the remote device");
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
                AlpacaDeviceBaseClass.LogMessage(TL,clientNumber, "Description", response);
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
                AlpacaDeviceBaseClass.LogMessage(TL,clientNumber, "Name", response);
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

        #region ISwitchV2 Implementation

        public bool CanWrite(short id)
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            return DynamicClientDriver.GetShortIndexedBool(clientNumber, client, URIBase, strictCasing, TL, "CanWrite", id, MemberTypes.Method);
        }

        public bool GetSwitch(short id)
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            return DynamicClientDriver.GetShortIndexedBool(clientNumber, client, URIBase, strictCasing, TL, "GetSwitch", id, MemberTypes.Method);
        }

        public string GetSwitchDescription(short id)
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            return DynamicClientDriver.GetShortIndexedString(clientNumber, client, URIBase, strictCasing, TL, "GetSwitchDescription", id, MemberTypes.Method);
        }

        public string GetSwitchName(short id)
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            return DynamicClientDriver.GetShortIndexedString(clientNumber, client, URIBase, strictCasing, TL, "GetSwitchName", id, MemberTypes.Method);
        }

        public double GetSwitchValue(short id)
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            return DynamicClientDriver.GetShortIndexedDouble(clientNumber, client, URIBase, strictCasing, TL, "GetSwitchValue", id, MemberTypes.Method);
        }

        public short MaxSwitch
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<short>(clientNumber, client, URIBase, strictCasing, TL, "MaxSwitch", MemberTypes.Property);
            }
        }

        public double MaxSwitchValue(short id)
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            return DynamicClientDriver.GetShortIndexedDouble(clientNumber, client, URIBase, strictCasing, TL, "MaxSwitchValue", id, MemberTypes.Method);
        }

        public double MinSwitchValue(short id)
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            return DynamicClientDriver.GetShortIndexedDouble(clientNumber, client, URIBase, strictCasing, TL, "MinSwitchValue", id, MemberTypes.Method);
        }

        public double SwitchStep(short id)
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            return DynamicClientDriver.GetShortIndexedDouble(clientNumber, client, URIBase, strictCasing, TL, "SwitchStep", id, MemberTypes.Method);
        }

        public void SetSwitchName(short id, string name)
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            DynamicClientDriver.SetStringWithShortParameter(clientNumber, client, URIBase, strictCasing, TL, "SetSwitchName", id, name, MemberTypes.Method);
        }

        public void SetSwitch(short id, bool state)
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            DynamicClientDriver.SetBoolWithShortParameter(clientNumber, client, URIBase,strictCasing, TL, "SetSwitch", id, state, MemberTypes.Method);
        }

        public void SetSwitchValue(short id, double value)
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            DynamicClientDriver.SetDoubleWithShortParameter(clientNumber, client, URIBase, strictCasing, TL, "SetSwitchValue", id, value, MemberTypes.Method);
        }

        #endregion

    }
}