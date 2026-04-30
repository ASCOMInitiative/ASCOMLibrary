using ASCOM.Common;
using ASCOM.Common.Alpaca;
using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// Base class for Alpaca client devices
    /// </summary>
    public abstract class AlpacaDeviceBaseClass : IAlpacaClientV2, IDisposable
    {
        // Variables common to all instances
        internal ServiceType serviceType = AlpacaClient.CLIENT_SERVICETYPE_DEFAULT;
        internal string ipAddressString = AlpacaClient.CLIENT_IPADDRESS_DEFAULT;
        internal decimal portNumber = AlpacaClient.CLIENT_IPPORT_DEFAULT;
        internal decimal remoteDeviceNumber = AlpacaClient.CLIENT_REMOTEDEVICENUMBER_DEFAULT; // Device number in the URI on the remote Alpaca device
        internal int establishConnectionTimeout = AlpacaClient.CLIENT_ESTABLISHCONNECTIONTIMEOUT_DEFAULT;
        internal int standardDeviceResponseTimeout = AlpacaClient.CLIENT_STANDARDCONNECTIONTIMEOUT_DEFAULT;
        internal int longDeviceResponseTimeout = AlpacaClient.CLIENT_LONGCONNECTIONTIMEOUT_DEFAULT;
        internal string userName = AlpacaClient.CLIENT_USERNAME_DEFAULT;
        internal string password = AlpacaClient.CLIENT_PASSWORD_DEFAULT;
        internal bool strictCasing = true; // Strict or flexible interpretation of casing in device JSON responses
        internal ILogger logger = AlpacaClient.CLIENT_LOGGER_DEFAULT; // Private variable to hold the trace logger object
        internal DeviceTypes clientDeviceType = DeviceTypes.Telescope; // Variable to hold the device type, which is set in each device type class
        internal uint clientNumber = AlpacaClient.CLIENT_CLIENTNUMBER_DEFAULT; // Unique number for this driver within the locaL server, i.e. across all drivers that the local server is serving

        internal HttpClient client; // Client to send and receive REST style messages to / from the remote device
        internal string uriBase; // URI base unique to this driver
        private bool disposedValue; // Whether or not the client has been Disposed()
        private readonly ClientConfiguration clientConfiguration; // The client configuration

        internal string userAgentProductName; // Initialised elsewhere
        internal string userAgentProductVersion; // Initialised elsewhere

        internal bool trustUserGeneratedSslCertificates = AlpacaClient.TRUST_USER_GENERATED_SSL_CERTIFICATES_DEFAULT;
        internal bool throwOnBadDateTimeJSON = AlpacaClient.THROW_ON_BAD_JSON_DATE_TIME_DEFAULT;
        internal bool request100Continue = AlpacaClient.CLIENT_REQUEST_100_CONTINUE_DEFAULT;

        private short? interfaceVersion;

        #region Variables and Constants specific to the Camera class

        // Variables specific to the Camera type
        internal ImageArrayTransferType imageArrayTransferType = AlpacaClient.CLIENT_IMAGEARRAYTRANSFERTYPE_DEFAULT;
        internal ImageArrayCompression imageArrayCompression = AlpacaClient.CLIENT_IMAGEARRAYCOMPRESSION_DEFAULT;

        #endregion

        /// <summary>
        /// Create a new instance of the AlpacaDeviceBaseClass passing the instance to the client configuration class
        /// </summary>
        public AlpacaDeviceBaseClass()
        {
            clientConfiguration = new ClientConfiguration(this);
        }

        #region Configuration properties

        /// <summary>
        /// Provide access to the client configuration
        /// </summary>
        /// <remarks>
        /// The returned <see cref="ClientConfiguration"/> instance properties can be used to read and change the client's configuration. 
        /// If any values are changed, the <see cref="RefreshClient"/> method must be called to bring the changes into effect.
        /// </remarks>
        public ClientConfiguration ClientConfiguration
        {
            get { return clientConfiguration; }

        }

        /// <summary>
        /// Updates the internal HTTP client with a new instance.
        /// </summary>
        /// <remarks>This method must be called after changing the client configuration through the <see cref="ClientConfiguration"/> property.</remarks>
        public void RefreshClient()
        {
            DynamicClientDriver.CreateHttpClient(ref client, ClientConfiguration.ServiceType, ClientConfiguration.IpAddress, ClientConfiguration.PortNumber, ClientConfiguration.ClientNumber,
                ClientConfiguration.DeviceType, ClientConfiguration.UserName, ClientConfiguration.Password, ClientConfiguration.ImageArrayCompression,
                logger, ClientConfiguration.UserAgentProductName, ClientConfiguration.UserAgentProductVersion, trustUserGeneratedSslCertificates, ClientConfiguration.Request100Continue);

            // Reset the URI base in case the remote device number has changed
            uriBase = $"{AlpacaConstants.API_URL_BASE}{AlpacaConstants.API_VERSION_V1}/{clientDeviceType}/{ClientConfiguration.RemoteDeviceNumber}/";
        }

        #endregion

        /// <summary>Creates a Parameters instance from this client's connection fields.</summary>
        internal Parameters CreateParameters(int timeout, string method, MemberTypes memberType)
        {
            return new Parameters(clientNumber, client, timeout, uriBase, strictCasing, logger, method, memberType);
        }

        #region IAscomDevice common properties and methods.

        ///<inheritdoc/>
        public string Action(string actionName, string actionParameters)
        {
            LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"ACTION: About to submit Action: {actionName}");
            Dictionary<string, string> formParameters = new Dictionary<string, string>
            {
                { AlpacaConstants.ACTION_COMMAND_PARAMETER_NAME, actionName },
                { AlpacaConstants.ACTION_PARAMETERS_PARAMETER_NAME, actionParameters }
            };
            string remoteString = DynamicClientDriver.SendToRemoteDevice<string>(new Parameters(clientNumber, client, longDeviceResponseTimeout, uriBase, strictCasing, logger, "Action", MemberTypes.Method), formParameters, HttpMethod.Put);
            LogMessage(logger, clientNumber, "Action", $"Response length: {remoteString.Length}");
            LogMessage(logger, clientNumber, "Action", $"Response: {((remoteString.Length <= 100) ? remoteString : remoteString.Substring(0, 100))}");
            return remoteString;
        }

        ///<inheritdoc/>
        public void CommandBlind(string command, bool raw = false)
        {
            Dictionary<string, string> formParameters = new Dictionary<string, string>
            {
                { AlpacaConstants.COMMAND_PARAMETER_NAME, command },
                { AlpacaConstants.RAW_PARAMETER_NAME, raw.ToString() }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(new Parameters(clientNumber, client, longDeviceResponseTimeout, uriBase, strictCasing, logger, "CommandBlind", MemberTypes.Method), formParameters, HttpMethod.Put);
            LogMessage(logger, clientNumber, "CommandBlind", "Completed OK");
        }

        ///<inheritdoc/>
        public bool CommandBool(string command, bool raw = false)
        {
            Dictionary<string, string> formParameters = new Dictionary<string, string>
            {
                { AlpacaConstants.COMMAND_PARAMETER_NAME, command },
                { AlpacaConstants.RAW_PARAMETER_NAME, raw.ToString() }
            };
            bool remoteBool = DynamicClientDriver.SendToRemoteDevice<bool>(new Parameters(clientNumber, client, longDeviceResponseTimeout, uriBase, strictCasing, logger, "CommandBool", MemberTypes.Method), formParameters, HttpMethod.Put);
            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "CommandBool", remoteBool.ToString());
            return remoteBool;
        }

        ///<inheritdoc/>
        public string CommandString(string command, bool raw = false)
        {
            Dictionary<string, string> formParameters = new Dictionary<string, string>
            {
                { AlpacaConstants.COMMAND_PARAMETER_NAME, command },
                { AlpacaConstants.RAW_PARAMETER_NAME, raw.ToString() }
            };
            string remoteString = DynamicClientDriver.SendToRemoteDevice<string>(new Parameters(clientNumber, client, longDeviceResponseTimeout, uriBase, strictCasing, logger, "CommandString", MemberTypes.Method), formParameters, HttpMethod.Put);
            LogMessage(logger, clientNumber, "CommandString", remoteString);
            return remoteString;
        }

        ///<inheritdoc/>
        public bool Connected
        {
            get
            {
                bool response = DynamicClientDriver.GetValue<bool>(CreateParameters(establishConnectionTimeout, "Connected", MemberTypes.Property));
                LogMessage(logger, clientNumber, "Connected", response.ToString());
                return response;
            }
            set
            {
                // Set the device's Connected property
                try
                {
                    LogMessage(logger, clientNumber, $"Connected", $"Setting {this.GetType().Name} Connected to {value}");
                    DynamicClientDriver.SetValue(CreateParameters(establishConnectionTimeout, "Connected", MemberTypes.Property), value);
                    LogMessage(logger, clientNumber, $"Connected", $"{this.GetType().Name} Connected set to {value} OK");
                }
                catch (Exception ex)
                {
                    LogMessage(logger, clientNumber, $"Connected", $"Exception when connecting to {this.GetType().Name}: {ex.Message}\r\n{ex}");
                    throw;
                }
            }
        }

        ///<inheritdoc/>
        public string Description
        {
            get
            {
                string response = DynamicClientDriver.GetValue<string>(new Parameters(clientNumber, client, standardDeviceResponseTimeout, uriBase, strictCasing, logger, "Description", MemberTypes.Property));
                LogMessage(logger, clientNumber, "Description", response);

                return response;
            }
        }

        ///<inheritdoc/>
        public string DriverInfo
        {
            get
            {
                return DynamicClientDriver.GetValue<string>(new Parameters(clientNumber, client, standardDeviceResponseTimeout, uriBase, strictCasing, logger, "DriverInfo", MemberTypes.Property));
            }
        }

        ///<inheritdoc/>
        public string DriverVersion
        {
            get
            {
                string remoteString = DynamicClientDriver.GetValue<string>(new Parameters(clientNumber, client, standardDeviceResponseTimeout, uriBase, strictCasing, logger, "DriverVersion", MemberTypes.Property));
                LogMessage(logger, clientNumber, "DriverVersion", remoteString);
                return remoteString;
            }
        }

        ///<inheritdoc/>
        public short InterfaceVersion
        {
            get
            {
                // Test whether the interface version has already been retrieved
                if (!interfaceVersion.HasValue) // This is the first time the method has been called so get the interface version number from the driver and cache it
                {
                    // Get the interface version
                    try
                    {
                        interfaceVersion = DynamicClientDriver.GetValue<short>(new Parameters(clientNumber, client, establishConnectionTimeout, uriBase, strictCasing, logger, "InterfaceVersion", MemberTypes.Property));
                        LogMessage(logger, clientNumber, "InterfaceVersion", interfaceVersion.ToString());
                    }
                    catch (Exception ex) // The method failed so assume that the driver has a version 1 interface where the InterfaceVersion method is not implemented
                    {
                        interfaceVersion = 1;
                        LogMessage(logger, clientNumber, "InterfaceVersion", $"Threw exception: {ex.Message}. Returning default interface version: {interfaceVersion}.");
                    }
                }

                return interfaceVersion.Value; // Return the newly retrieved or already cached value
            }
        }

        ///<inheritdoc/>
        public string Name
        {
            get
            {
                string response = DynamicClientDriver.GetValue<string>(CreateParameters(standardDeviceResponseTimeout, "Name", MemberTypes.Property));
                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "Name", response);
                return response;
            }
        }

        ///<inheritdoc/>
        public IList<string> SupportedActions
        {
            get
            {
                List<string> supportedActions = DynamicClientDriver.GetValue<List<string>>(new Parameters(clientNumber, client, standardDeviceResponseTimeout, uriBase, strictCasing, logger, "SupportedActions", MemberTypes.Property));
                LogMessage(logger, clientNumber, "SupportedActions", $"Returning {supportedActions.Count} actions");

                List<string> returnValues = new List<string>();
                foreach (string action in supportedActions)
                {
                    returnValues.Add(action);
                    LogMessage(logger, clientNumber, "SupportedActions", $"Returning action: {action}");
                }

                return returnValues;
            }
        }

        #endregion

        #region IAscomDeviceV2 common properties and methods

        ///<inheritdoc/>
        public void Connect()
        {
            // Check whether this device supports Connect / Disconnect
            if (DeviceCapabilities.HasConnectAndDeviceState(clientDeviceType, InterfaceVersion))
            {
                // Platform 7 or later device so use the device's Connect method
                DynamicClientDriver.CallMethodWithNoParameters(CreateParameters(establishConnectionTimeout, "Connect", MemberTypes.Method));
                return;
            }

            // Platform 6 or earlier device so use the Connected property
            DynamicClientDriver.SetValue(CreateParameters(establishConnectionTimeout, "Connected", MemberTypes.Property), true);
        }

        ///<inheritdoc/>
        public void Disconnect()
        {
            // Check whether this device supports Connect / Disconnect
            if (DeviceCapabilities.HasConnectAndDeviceState(clientDeviceType, InterfaceVersion))
            {
                // Platform 7 or later device so use the device's Disconnect method
                DynamicClientDriver.CallMethodWithNoParameters(CreateParameters(establishConnectionTimeout, "Disconnect", MemberTypes.Method));
                return;
            }

            // Platform 6 or earlier device so use the Connected property
            DynamicClientDriver.SetValue(CreateParameters(establishConnectionTimeout, "Connected", MemberTypes.Property), false);
        }

        ///<inheritdoc/>
        public bool Connecting
        {
            get
            {
                // Check whether this device supports Connect / Disconnect
                if (DeviceCapabilities.HasConnectAndDeviceState(clientDeviceType, InterfaceVersion))
                {
                    // Platform 7 or later device so return the device's Connecting property
                    return DynamicClientDriver.GetValue<bool>(CreateParameters(establishConnectionTimeout, "Connecting", MemberTypes.Property));
                }

                // Always return false for Platform 6 and earlier devices
                return false;
            }
        }

        /// <inheritdoc/>
        public List<StateValue> DeviceState
        {
            get
            {
                // Check whether this device supports Connect / Disconnect
                if (DeviceCapabilities.HasConnectAndDeviceState(clientDeviceType, InterfaceVersion))
                {
                    // Platform 7 or later device so return the device's value
                    // Note use of a concrete class here because System.Text.Json cannot de-serialise to an interface, it requires a concrete class
                    List<StateValue> response = DynamicClientDriver.GetValue<List<StateValue>>(CreateParameters(standardDeviceResponseTimeout, "DeviceState", MemberTypes.Property));

                    // Here we convert the device response to a type that can be returned as a LIst<IStateValue> object.
                    // This is done by using LINQ to cast the returned List<StateValue> objects to the StateValue type and then returning them as a list. Convoluted, but it works!
                    // The list is also cleaned to rid it of JsonElement types that are created by System.Text.Json de-serialisation
                    List<StateValue> returnValue = OperationalStateProperty.Clean(response.Cast<StateValue>().ToList(), clientDeviceType, logger);

                    foreach (IStateValue value in returnValue)
                    {
                        logger.LogMessage(LogLevel.Debug, "DeviceState", $"{value.Name} = {value.Value} - Type: {value.Value.GetType().Name}");
                    }
                    return returnValue;
                }

                // Return an empty list for Platform 6 and earlier devices
                return new List<StateValue>();
            }
        }

        #endregion

        #region Support code

        internal static void LogMessage(ILogger logger, uint instance, string prefix, string message)
        {
            logger.LogMessage(LogLevel.Debug, $"{prefix} {instance}", message);
        }

        internal static void LogBlankLine(ILogger logger)
        {
            logger.BlankLine(LogLevel.Information);
        }

        #endregion

        #region Dispose Support

        /// <summary>
        /// Dispose call used by the CLR during clean-up. Do not use this method, use <see cref="Dispose()"/> instead.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (!(client is null))
                    {
                        client.Dispose();
                        client = null;
                    }
                }
                disposedValue = true;
            }
        }

        /// <summary>
        /// This method is a "clean-up" method that is primarily of use to drivers that are written in languages such as C# and VB.NET where resource clean-up is initially managed by the language's 
        /// runtime garbage collection mechanic. Driver authors should take care to ensure that a client or runtime calling Dispose() does not adversely affect other connected clients.
        /// Applications should not call this method.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put clean-up code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
