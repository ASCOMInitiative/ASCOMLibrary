using ASCOM.Common.Alpaca;
using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// ASCOM Alpaca Switch client
    /// </summary>
    public class AlpacaSwitch : AlpacaDeviceBaseClass, ISwitchV2
    {
        #region Variables and Constants

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
                // Set the device type
                clientDeviceType = "Switch";

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
            DynamicClientDriver.SetBoolWithShortParameter(clientNumber, client, URIBase, strictCasing, TL, "SetSwitch", id, state, MemberTypes.Method);
        }

        public void SetSwitchValue(short id, double value)
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            DynamicClientDriver.SetDoubleWithShortParameter(clientNumber, client, URIBase, strictCasing, TL, "SetSwitchValue", id, value, MemberTypes.Method);
        }

        #endregion

    }
}