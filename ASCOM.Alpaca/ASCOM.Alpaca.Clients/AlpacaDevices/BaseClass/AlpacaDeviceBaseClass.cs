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
        internal string URIBase; // URI base unique to this driver
        private bool disposedValue; // Whether or not the client has been Disposed()
        private readonly ClientConfiguration clientConfiguration; // The client configuration

        internal string userAgentProductName;
        internal string userAgentProductVersion;

        internal bool trustUserGeneratedSslCertificates;

        private short? interfaceVersion;

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
        public ClientConfiguration ClientConfiguration
        {
            get { return clientConfiguration; }

        }

        #endregion

        #region IAscomDevice common properties and methods.

        /// <summary>
        /// Invokes the specified device-specific action.
        /// </summary>
        /// <param name="actionName">
        /// A well known name agreed by interested parties that represents the action to be carried out. 
        /// </param>
        /// <param name="actionParameters">List of required parameters or an <see cref="String.Empty">Empty String</see> if none are required.
        /// </param>
        /// <returns>A string response. The meaning of returned strings is set by the driver author.</returns>
        /// <exception cref="NotImplementedException">Throws this exception if an action name is not supported.
        /// of driver capabilities, but the driver must still throw an ASCOM.ActionNotImplemented exception if it is asked to 
        /// perform an action that it does not support.</exception>
        /// <exception cref="NotConnectedException">If the driver is not connected.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <exception cref="ActionNotImplementedException">It is intended that the <see cref="SupportedActions"/> method will inform clients of driver capabilities, but the driver must still throw 
        /// an <see cref="ASCOM.ActionNotImplementedException"/> exception  if it is asked to perform an action that it does not support.</exception>
        /// <example>Suppose filter wheels start to appear with automatic wheel changers; new actions could 
        /// be “FilterWheel:QueryWheels” and “FilterWheel:SelectWheel”. The former returning a 
        /// formatted list of wheel names and the second taking a wheel name and making the change, returning appropriate 
        /// values to indicate success or failure.
        /// </example>
        /// <remarks><p style="color:red"><b>Can throw a not implemented exception</b></p> 
        /// This method is intended for use in all current and future device types and to avoid name clashes, management of action names 
        /// is important from day 1. A two-part naming convention will be adopted - <b>DeviceType:UniqueActionName</b> where:
        /// <list type="bullet">
        /// <item><description>DeviceType is the string name of the device type e.g. Telescope, Camera, Switch etc.</description></item>
        /// <item><description>UniqueActionName is a single word, or multiple words joined by underscore characters, that sensibly describes the action to be performed.</description></item>
        /// </list>
        /// <para>
        /// It is recommended that UniqueActionNames should be a maximum of 16 characters for legibility.
        /// Should the same function and UniqueActionName be supported by more than one type of device, the reserved DeviceType of 
        /// “General” will be used. Action names will be case insensitive, so FilterWheel:SelectWheel, filterwheel:selectwheel 
        /// and FILTERWHEEL:SELECTWHEEL will all refer to the same action.</para>
        /// <para>The names of all supported actions must be returned in the <see cref="SupportedActions"/> property.</para>
        /// </remarks>
        public string Action(string actionName, string actionParameters)
        {
            LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"ACTION: About to submit Action: {actionName}");
            string response = DynamicClientDriver.Action(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, actionName, actionParameters);
            LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"ACTION: Received response of length: {response.Length}");
            return response;
        }

        /// <summary>
        /// Transmits an arbitrary string to the device and does not wait for a response.
        /// Optionally, protocol framing characters may be added to the string before transmission.
        /// </summary>
        /// <param name="command">The literal command string to be transmitted.</param>
        /// <param name="raw">
        /// if set to <c>true</c> the string is transmitted 'as-is'.
        /// If set to <c>false</c> then protocol framing characters may be added prior to transmission.
        /// </param>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="NotConnectedException">If the driver is not connected.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>May throw a NotImplementedException.</b></p>
        /// <para>The CommandXXX methods are a historic mechanic that provides clients with direct and unimpeded access to change device hardware configuration. While highly enabling for clients, this mechanic is inherently risky
        /// because clients can fundamentally change hardware operation without the driver being aware that a change is taking / has taken place.</para>
        /// <para>The newer Action / SupportedActions mechanic provides discrete, named, functions that can deliver any functionality required.They do need driver authors to make provision for them within the 
        /// driver, but this approach is much lower risk than using the CommandXXX methods because it enables the driver to resolve conflicts between standard device interface commands and extended commands 
        /// provided as Actions.The driver is always aware of what is happening and can adapt more effectively to client needs.</para>
        /// </remarks>
        public void CommandBlind(string command, bool raw = false)
        {
            DynamicClientDriver.CommandBlind(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, command, raw);
        }

        /// <summary>
        /// Transmits an arbitrary string to the device and waits for a boolean response.
        /// Optionally, protocol framing characters may be added to the string before transmission.
        /// </summary>
        /// <param name="command">The literal command string to be transmitted.</param>
        /// <param name="raw">
        /// if set to <c>true</c> the string is transmitted 'as-is'.
        /// If set to <c>false</c> then protocol framing characters may be added prior to transmission.
        /// </param>
        /// <returns>
        /// Returns the interpreted boolean response received from the device.
        /// </returns>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="NotConnectedException">If the driver is not connected.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>May throw a NotImplementedException.</b></p>
        /// <para>The CommandXXX methods are a historic mechanic that provides clients with direct and unimpeded access to change device hardware configuration. While highly enabling for clients, this mechanic is inherently risky
        /// because clients can fundamentally change hardware operation without the driver being aware that a change is taking / has taken place.</para>
        /// <para>The newer Action / SupportedActions mechanic provides discrete, named, functions that can deliver any functionality required.They do need driver authors to make provision for them within the 
        /// driver, but this approach is much lower risk than using the CommandXXX methods because it enables the driver to resolve conflicts between standard device interface commands and extended commands 
        /// provided as Actions.The driver is always aware of what is happening and can adapt more effectively to client needs.</para>
        /// </remarks>
        public bool CommandBool(string command, bool raw = false)
        {
            return DynamicClientDriver.CommandBool(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, command, raw);
        }

        /// <summary>
        /// Transmits an arbitrary string to the device and waits for a string response.
        /// Optionally, protocol framing characters may be added to the string before transmission.
        /// </summary>
        /// <param name="command">The literal command string to be transmitted.</param>
        /// <param name="raw">
        /// if set to <c>true</c> the string is transmitted 'as-is'.
        /// If set to <c>false</c> then protocol framing characters may be added prior to transmission.
        /// </param>
        /// <returns>
        /// Returns the string response received from the device.
        /// </returns>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="NotConnectedException">If the driver is not connected.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>May throw a NotImplementedException.</b></p>
        /// <para>The CommandXXX methods are a historic mechanic that provides clients with direct and unimpeded access to change device hardware configuration. While highly enabling for clients, this mechanic is inherently risky
        /// because clients can fundamentally change hardware operation without the driver being aware that a change is taking / has taken place.</para>
        /// <para>The newer Action / SupportedActions mechanic provides discrete, named, functions that can deliver any functionality required.They do need driver authors to make provision for them within the 
        /// driver, but this approach is much lower risk than using the CommandXXX methods because it enables the driver to resolve conflicts between standard device interface commands and extended commands 
        /// provided as Actions.The driver is always aware of what is happening and can adapt more effectively to client needs.</para>
        /// </remarks>
        public string CommandString(string command, bool raw = false)
        {
            return DynamicClientDriver.CommandString(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, command, raw);
        }

        /// <summary>
        /// Set True to enable the link. Set False to disable the link.
        /// You can also read the property to check whether it is connected.
        /// </summary>
        /// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public bool Connected
        {
            get
            {
                bool response = DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Connected", MemberTypes.Property);
                LogMessage(logger, clientNumber, "Connected", response.ToString());
                return response;
            }
            set
            {
                // Set the device's Connected property
                try
                {
                    LogMessage(logger, clientNumber, $"Connected", $"Setting {this.GetType().Name} Connected to {value}");
                    DynamicClientDriver.SetBool(clientNumber, client, establishConnectionTimeout, URIBase, strictCasing, logger, "Connected", value, MemberTypes.Property);
                    LogMessage(logger, clientNumber, $"Connected", $"{this.GetType().Name} Connected set to {value} OK");
                }
                catch (Exception ex)
                {
                    LogMessage(logger, clientNumber, $"Connected", $"Exception when connecting to {this.GetType().Name}: {ex.Message}\r\n{ex}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Returns a description of the driver, such as manufacturer and model
        /// number. Any ASCII characters may be used. The string shall not exceed 68
        /// characters (for compatibility with FITS headers).
        /// </summary>
        /// <value>The description.</value>
        /// <exception cref="NotConnectedException">When <see cref="Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public string Description
        {
            get
            {
                string response = DynamicClientDriver.Description(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger);
                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "Description", response);
                return response;
            }
        }

        /// <summary>
        /// Descriptive and version information about this ASCOM driver.
        /// This string may contain line endings and may be hundreds to thousands of characters long.
        /// It is intended to display detailed information on the ASCOM driver, including version and copyright data.
        /// See the Description property for descriptive info on the telescope itself.
        /// To get the driver version in a parseable string, use the DriverVersion property.
        /// </summary>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public string DriverInfo
        {
            get
            {
                return DynamicClientDriver.DriverInfo(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger);
            }
        }

        /// <summary>
        /// A string containing only the major and minor version of the driver.
        /// This must be in the form "n.n".
        /// Not to be confused with the InterfaceVersion property, which is the version of this specification supported by the driver (currently 2). 
        /// </summary>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public string DriverVersion
        {
            get
            {
                return DynamicClientDriver.DriverVersion(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger);
            }
        }

        /// <summary>
        /// The version of this interface. Will return 2 for this version.
        /// Clients can detect legacy V1 drivers by trying to read this property.
        /// If the driver raises an error, it is a V1 driver. V1 did not specify this property. A driver may also return a value of 1. 
        /// In other words, a raised error or a return value of 1 indicates that the driver is a V1 driver. 
        /// </summary>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public short InterfaceVersion
        {
            get
            {
                // Test whether the interface version has already been retrieved
                if (!interfaceVersion.HasValue) // This is the first time the method has been called so get the interface version number from the driver and cache it
                {
                    try { interfaceVersion = DynamicClientDriver.InterfaceVersion(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger); } // Get the interface version
                    catch { interfaceVersion = 1; } // The method failed so assume that the driver has a version 1 interface where the InterfaceVersion method is not implemented
                }

                return interfaceVersion.Value; // Return the newly retrieved or already cached value
            }
        }

        /// <summary>
        /// The short name of the driver, for display purposes
        /// </summary>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public string Name
        {
            get
            {
                string response = DynamicClientDriver.GetValue<string>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Name", MemberTypes.Property);
                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "Name", response);
                return response;
            }
        }

        /// <summary>
        /// Returns the list of action names supported by this driver.
        /// </summary>
        /// <value>An ArrayList of strings (SafeArray collection) containing the names of supported actions.</value>
        /// <exception cref="NotConnectedException">When <see cref="Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Must be implemented</b></p> This method must return an empty <see cref="IList{String}"/> object if no actions are supported. Please do not throw a <see cref="NotImplementedException" />.
        /// <para>SupportedActions is a "discovery" mechanism that enables clients to know which Actions a device supports without having to exercise the Actions themselves. This mechanism is necessary because there could be
        /// people / equipment safety issues if actions are called unexpectedly or out of a defined process sequence.
        /// It follows from this that SupportedActions must return names that match the spelling of Action names exactly, without additional descriptive text. However, returned names may use any casing
        /// because the <see cref="Action" /> ActionName parameter is case insensitive.</para>
        /// </remarks>
        public IList<string> SupportedActions
        {
            get
            {
                return DynamicClientDriver.SupportedActions(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger);
            }
        }

        #endregion

        #region IAscomDeviceV2 common properties and methods

        /// <summary>
        /// Connect to a device asynchronously
        /// </summary>
        public void Connect()
        {
            // Check whether this device supports Connect / Disconnect
            if (DeviceCapabilities.HasConnectAndDeviceState(clientDeviceType, InterfaceVersion))
            {
                // Platform 7 or later device so use the device's Connect method
                DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "Connect", MemberTypes.Method);
                return;
            }

            // Platform 6 or earlier device so use the Connected property
            DynamicClientDriver.SetBool(clientNumber, client, establishConnectionTimeout, URIBase, strictCasing, logger, "Connected", true, MemberTypes.Property);
        }

        /// <summary>
        /// Disconnect from a device asynchronously
        /// </summary>
        public void Disconnect()
        {
            // Check whether this device supports Connect / Disconnect
            if (DeviceCapabilities.HasConnectAndDeviceState(clientDeviceType, InterfaceVersion))
            {
                // Platform 7 or later device so use the device's Disconnect method
                DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "Disconnect", MemberTypes.Method);
                return;
            }

            // Platform 6 or earlier device so use the Connected property
            DynamicClientDriver.SetBool(clientNumber, client, establishConnectionTimeout, URIBase, strictCasing, logger, "Connected", false, MemberTypes.Property);
        }

        /// <summary>
        /// Asynchronous connection completion variable.
        /// </summary>
        public bool Connecting
        {
            get
            {
                // Check whether this device supports Connect / Disconnect
                if (DeviceCapabilities.HasConnectAndDeviceState(clientDeviceType, InterfaceVersion))
                {
                    // Platform 7 or later device so return the device's Connecting property
                    return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Connecting", MemberTypes.Property);
                }

                // Always return false for Platform 6 and earlier devices
                return false;
            }
        }

        /// <summary>
        /// Returns a List of device IStateValue state objects
        /// </summary>
        public List<StateValue> DeviceState
        {
            get
            {
                // Check whether this device supports Connect / Disconnect
                if (DeviceCapabilities.HasConnectAndDeviceState(clientDeviceType, InterfaceVersion))
                {
                    // Platform 7 or later device so return the device's value
                    // Note use of a concrete class here because System.Text.Json cannot de-serialise to an interface, it requires a concrete class
                    List<StateValue> response = DynamicClientDriver.GetValue<List<StateValue>>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "DeviceState", MemberTypes.Property);

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
            logger.LogMessage(LogLevel.Information, $"{prefix} {instance}", message);
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
