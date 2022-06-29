using ASCOM.Common.Interfaces;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ASCOM.Alpaca.Clients
{
    public abstract class AlpacaDeviceBaseClass : IDisposable
    {
        // Variables common to all instances
        internal ServiceType serviceType = ServiceType.Http;
        internal string ipAddressString = "127.0.0.1";
        internal decimal portNumber = 11111;
        internal decimal remoteDeviceNumber = 0; // Device number in the URI on the remote Alpaca device
        internal int establishConnectionTimeout = 3;
        internal int standardDeviceResponseTimeout = 3;
        internal int longDeviceResponseTimeout = 100;
        internal uint clientNumber; // Unique number for this driver within the locaL server, i.e. across all drivers that the local server is serving
        internal string userName = "";
        internal string password = "";
        internal bool manageConnectLocally = false;
        internal bool strictCasing = true; // Strict or flexible interpretation of casing in device JSON responses
        internal string clientDeviceType = "UNINITIALISED_VALUE"; // Variable to hold the device type, which is set in each device type class

        internal ILogger TL; // Private variable to hold the trace logger object

        internal RestClient client; // Client to send and receive REST style messages to / from the remote device
        internal bool clientIsConnected;  // Connection state of this driver
        internal string URIBase; // URI base unique to this driver
        private bool disposedValue;

        #region Common properties and methods.

        public string Action(string actionName, string actionParameters)
        {
            DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
            LogMessage(TL, clientNumber, clientDeviceType, $"ACTION: About to submit Action: {actionName}");
            string response = DynamicClientDriver.Action(clientNumber, client, URIBase, strictCasing, TL, actionName, actionParameters);
            LogMessage(TL, clientNumber, clientDeviceType, $"ACTION: Received response of length: {response.Length}");
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
                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, $"The Connected property is being managed locally so the new value '{value}' will not be sent to the remote device");
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

        #region Support code

        internal static void LogMessage(ILogger logger, uint instance, string prefix, string message)
        {
            if (logger != null) logger.Log(LogLevel.Information, $"{prefix} {instance}".PadRight(30) + message);
        }

        internal static void LogBlankLine(ILogger logger)
        {
            if (logger != null) logger.Log(LogLevel.Information, $" ");
        }

        #endregion

        #region Dispose Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (!(client is null))
                    {
                        client.ClearHandlers();
                        client = null;
                    }
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put clean-up code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
