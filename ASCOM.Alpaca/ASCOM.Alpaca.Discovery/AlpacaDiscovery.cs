using ASCOM.Alpaca.Clients;
using ASCOM.Common;
using ASCOM.Common.Alpaca;
using ASCOM.Common.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ASCOM.Alpaca.Discovery
{

    /// <summary>
    /// Enables clients to discover Alpaca devices by sending one or more discovery polls. Returns information on discovered <see cref="AlpacaDevice">Alpaca devices</see> and the <see cref="AscomDevice">ASCOM devices</see> that are available.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The discovery process is asynchronous and is initiated by the <see cref="StartDiscovery(int, int, int, double, bool, bool, bool, ServiceType)"/> method. Clients can then either work synchronously by looping and periodically 
    /// polling the <see cref="DiscoveryComplete"/> property or work asynchronously by handling the <see cref="AlpacaDevicesUpdated"/> and <see cref="DiscoveryCompleted"/> events while doing other work.
    /// </para>
    /// <para>
    /// The <see cref="StartDiscovery(int, int, int, double, bool, bool, bool, ServiceType)"/> method is used to set the character of the discovery e.g. the discovery duration and whether to search for IPv4 and/or IPv6 devices. 
    /// After the specified discovery duration, the <see cref="DiscoveryComplete"/> event fires and the <see cref="DiscoveryCompleted"/> property returns True.
    /// </para>
    /// <para>
    /// Once discovery is complete, .NET clients can retrieve details of discovered Alpaca devices and associated ASCOM interface devices through the <see cref="GetAlpacaDevices"/> and <see cref="GetAscomDevices(DeviceTypes?)"/> methods.
    /// COM clients must use the <see cref="GetAlpacaDevicesAsArrayList"/> and <see cref="GetAscomDevicesAsArrayList(DeviceTypes?)"/> properties because COM does not support the generic classes used 
    /// in the <see cref="GetAlpacaDevices"/> and <see cref="GetAscomDevices(DeviceTypes?)"/> methods. 
    /// </para>
    /// </remarks>
    public class AlpacaDiscovery : IDisposable
    {

        #region Variables and Constants

        // Constants for discovery default values

        const int NUMBER_OF_POLLS_DEFAULT = 1;
        const int DISCOVERY_POLL_INTERVAL_DEFAULT = 100;
        const int DISCOVERY_PORT_DEFAULT = 32227;
        const double DISCOVERY_DURATION_DEFAULT = 1.0;
        const bool RESOLVE_DNS_NAME_DEFAULT = false;
        const bool USE_IP_V4_DEFAULT = true;
        const bool USE_IP_V6_DEFAULT = false;
        const ServiceType SERVICE_TYPE_DEFAULT = ServiceType.Http;

        // Constants to support discovery
        internal const string TRYING_TO_CONTACT_MANAGEMENT_API_MESSAGE = "Trying to contact Alpaca management API";
        internal const string FAILED_TO_CONTACT_MANAGEMENT_API_MESSAGE = "The Alpaca management API did not respond within the discovery response time";
        internal const double MINIMUM_TIME_REMAINING_TO_UNDERTAKE_DNS_RESOLUTION = 0.1d; // Minimum discovery time (seconds) that must remain if a DNS IP to host name resolution is to be attempted
        internal const int NUMBER_OF_THREAD_MESSAGE_INDENT_SPACES = 2;

        // Utility objects
        private readonly ILogger logger;
        private Finder finder;
        private Timer discoveryCompleteTimer;
        private HttpClient httpClient;

        // Private variables
        private readonly Dictionary<IPEndPoint, AlpacaDevice> alpacaDeviceList = new Dictionary<IPEndPoint, AlpacaDevice>(); // List of discovered Alpaca devices keyed on IP:Port
        private bool disposedValue = false; // To detect redundant Dispose() method calls
        private double discoveryTime; // Length of the discovery phase before it times out
        private bool tryDnsNameResolution; // Flag indicating whether to attempt name resolution on the host IP address
        private DateTime discoveryStartTime; // Time at which the start discovery command was received
        private bool discoveryCompleteValue; // Discovery completion status
        private readonly object deviceListLockObject = new object(); // Lock object to synchronise access to the Alpaca device list collection, which is not a thread safe collection
        private readonly bool strictCasing; // Flag indicating whether case sensitive or case insensitive de-serialisation will be used.

        private ServiceType serviceType; // Holds the service type for management API calls: HTTP or HTTPS


        internal static ILogger iLogger;

        #endregion

        #region New and IDisposable Support

        /// <summary>
        /// Initialise the Alpaca discovery component
        /// </summary>
        public AlpacaDiscovery()
        {
            InitialiseClass(); // Initialise without a trace logger
        }

        /// <summary>
        /// Initialiser that takes a trace logger (Can only be used from .NET clients)
        /// </summary>
        /// <param name="strictCasing">Trace logger instance to use for activity logging</param>
        /// <param name="traceLogger">Trace logger instance to use for activity logging</param>
        public AlpacaDiscovery(bool strictCasing, ILogger traceLogger)
        {
            logger = traceLogger; // Save the supplied trace logger object
            this.strictCasing = strictCasing;
            InitialiseClass(); // Initialise using the trace logger
        }

        private void InitialiseClass()
        {
            try
            {
                // Initialise variables
                tryDnsNameResolution = false; // Initialise so that there is no host name resolution by default
                discoveryCompleteValue = true; // Initialise so that discoveries can be run

                // Initialise utility objects
                discoveryCompleteTimer = new Timer(OnDiscoveryCompleteTimer);
                if (finder != null)
                {
                    finder.Dispose();
                    finder = null;
                }

                // Get a new broadcast response finder
                //finder = new Finder(FoundDeviceEventHandler, strictCasing,TL);
                finder = new Finder(strictCasing, logger);
                finder.ResponseReceivedEvent += FoundDeviceEventHandler;

                LogMessage("AlpacaDiscoveryInitialise", $"Complete - Running on thread {Thread.CurrentThread.ManagedThreadId}");
            }
            catch (Exception ex)
            {
                LogMessage("AlpacaDiscoveryInitialise", $"Exception{ex}");
            }
        }

        /// <summary>
        /// Call used by the CLR during disposal. Do not call this method, use <see cref="Dispose()"/> instead.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // The trace logger is not disposed here because it is supplied by the client, which is response for disposing of it as appropriate.

                    // Unhook the Finder event handler
                    try { if (finder != null) finder.ResponseReceivedEvent -= FoundDeviceEventHandler; } catch { }

                    // Dispose of the Finder
                    try { if (finder != null) finder.Dispose(); } catch { }

                    // Dispose of the discovery completion timer
                    try { if (discoveryCompleteTimer != null) discoveryCompleteTimer.Dispose(); } catch { }

                    // Dispose of the HTTP client
                    try { if (httpClient != null) httpClient.Dispose(); } catch { }
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Disposes of the discovery component and cleans up resources
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put clean-up code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Raised every time information about discovered devices is updated
        /// </summary>
        /// <remarks>This event is only available to .NET clients, there is no equivalent for COM clients.</remarks>
        public event EventHandler AlpacaDevicesUpdated;

        /// <summary>
        /// Raised when the discovery is complete
        /// </summary>
        /// <remarks>This event is only available to .NET clients. COM clients should poll the <see cref="DiscoveryComplete"/> property periodically to determine when discovery is complete.</remarks>
        public event EventHandler DiscoveryCompleted;

        #endregion

        #region Public Properties

        /// <summary>
        /// Flag that indicates when a discovery cycle is complete
        /// </summary>
        /// <returns>True when discovery is complete.</returns>
        /// <remarks>The discovery is considered complete when the time period specified on the <see cref="StartDiscovery(int, int, int, double, bool, bool, bool, ServiceType)"/> method is exceeded.</remarks>
        public bool DiscoveryComplete
        {
            get
            {
                return discoveryCompleteValue;
            }

            private set
            {
                discoveryCompleteValue = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns an ArrayList of discovered Alpaca devices for use by COM clients
        /// </summary>
        /// <returns>ArrayList of <see cref="AlpacaDevice"/>classes</returns>
        /// <remarks>This method is for use by COM clients because it is not possible to pass a generic list as used in <see cref="GetAlpacaDevices"/> through a COM interface. 
        /// .NET clients should use <see cref="GetAlpacaDevices()"/> instead of this method.</remarks>
        public ArrayList GetAlpacaDevicesAsArrayList()
        {
            ArrayList alpacaDevicesAsArrayList; // Variable to hold the ArrayList analogue of the generic list of Alpaca devices
            alpacaDevicesAsArrayList = new ArrayList(); // Create a new array-list

            // populate the array-list with data from the generic list
            foreach (AlpacaDevice alpacaDevice in GetAlpacaDevices())
                alpacaDevicesAsArrayList.Add(alpacaDevice);
            return alpacaDevicesAsArrayList; // Return the Alpaca devices list as an ArrayList
        }

        /// <summary>
        /// Returns an ArrayList of discovered ASCOM devices, of the specified device type, for use by COM clients
        /// </summary>
        /// <param name="deviceType">The device type for which to search e.g. Telescope, Use <see langword="null"/> to return devices of all types.</param>
        /// <returns>ArrayList of <see cref="AscomDevice"/>classes</returns>
        /// <remarks>
        /// <para>
        /// This method is for use by COM clients because it is not possible to return a generic list, as used in <see cref="GetAscomDevices(DeviceTypes?)"/>, through a COM interface. 
        /// .NET clients should use <see cref="GetAscomDevices(DeviceTypes?)"/> instead of this method.
        /// </para>
        /// <para>
        /// This method will return every discovered device, regardless of device type, if the supplied "deviceType" parameter is null.
        /// </para>
        /// </remarks>
        public ArrayList GetAscomDevicesAsArrayList(DeviceTypes? deviceType)
        {
            return new ArrayList(GetAscomDevices(deviceType)); // Return the ASCOM devices list as an ArrayList
        }

        /// <summary>
        /// Returns a generic List of discovered Alpaca devices.
        /// </summary>
        /// <returns>List of <see cref="AlpacaDevice"/>classes</returns>
        /// <remarks>This method is only available to .NET clients because COM cannot handle generic types. COM clients should use <see cref="GetAlpacaDevicesAsArrayList()"/>.</remarks>
        public List<AlpacaDevice> GetAlpacaDevices()
        {
            lock (deviceListLockObject) // Make sure that the device list dictionary can't change while copying it to the list
                return alpacaDeviceList.Values.ToList(); // Create a copy of the dynamically changing alpacaDeviceList ConcurrentDictionary of discovered devices
        }

        /// <summary>
        /// Returns a generic list of discovered ASCOM devices of the specified device type.
        /// </summary>
        /// <param name="deviceType">The device type for which to search e.g. Telescope, Focuser. Use <see langword="null"/> to return devices of all types.</param>
        /// <returns>List of AscomDevice classes</returns>
        /// <remarks>
        /// <para>
        /// This method is only available to .NET clients because COM cannot handle generic types. COM clients should use <see cref="GetAlpacaDevicesAsArrayList()"/>.
        /// </para>
        /// <para>
        /// This method will return every discovered device, regardless of device type, if the supplied "deviceType" parameter is null.
        /// </para>
        /// </remarks>
        public List<AscomDevice> GetAscomDevices(DeviceTypes? deviceType)
        {
            var ascomDeviceList = new List<AscomDevice>(); // List of discovered ASCOM devices to support Chooser-like functionality
            lock (deviceListLockObject) // Make sure that the device list dictionary can't change while processing this command
            {

                // Iterate over the discovered Alpaca devices
                foreach (KeyValuePair<IPEndPoint, AlpacaDevice> alpacaDevice in alpacaDeviceList)
                {

                    // Iterate over each Alpaca interface version that the Alpaca device supports
                    foreach (int alpacaDeviceInterfaceVersion in alpacaDevice.Value.SupportedInterfaceVersions)
                    {

                        // Iterate over the ASCOM devices presented by this Alpaca device adding them to the return dictionary
                        foreach (AlpacaConfiguredDevice ascomDevice in alpacaDevice.Value.ConfiguredDevices)
                        {

                            // Test whether all devices or only devices of a specific device type are required
                            if (!deviceType.HasValue) // Return a full list of every discovered device regardless of device type 
                            {
                                ascomDeviceList.Add(new AscomDevice(ascomDevice.DeviceName, Devices.StringToDeviceType(ascomDevice.DeviceType), ascomDevice.DeviceNumber, ascomDevice.UniqueID, alpacaDevice.Value.ServiceType, alpacaDevice.Value.IPEndPoint, alpacaDevice.Value.HostName, alpacaDeviceInterfaceVersion)); // ASCOM device information 
                            }
                            else
                            {
                                if (Devices.StringToDeviceType(ascomDevice.DeviceType) == deviceType.Value) // Return only devices of the specified type
                                {
                                    ascomDeviceList.Add(new AscomDevice(ascomDevice.DeviceName, Devices.StringToDeviceType(ascomDevice.DeviceType), ascomDevice.DeviceNumber, ascomDevice.UniqueID, alpacaDevice.Value.ServiceType, alpacaDevice.Value.IPEndPoint, alpacaDevice.Value.HostName, alpacaDeviceInterfaceVersion)); // ASCOM device information 
                                }
                            }
                        } // Next Ascom Device
                    } // Next interface version
                } // Next Alpaca device

                // Return the information requested
                return ascomDeviceList; // Return the list of ASCOM devices
            }
        }

        /// <summary>
        /// Start an Alpaca device discovery based on the supplied parameters
        /// </summary>
        /// <param name="numberOfPolls">Number of polls to send in the range 1 to 5 (Default 1)</param>
        /// <param name="pollInterval">Interval between each poll in the range 10 to 5000 milliseconds (Default 100ms) </param>
        /// <param name="discoveryPort">Discovery port on which to send the broadcast (normally 32227) in the range 1025 to 65535 (Default 32227)</param>
        /// <param name="discoveryDuration">Length of time (seconds) to wait for devices to respond (Default 1.0 seconds)</param>
        /// <param name="resolveDnsName">Attempt to resolve host IP addresses to DNS names (Default <see langword="true"/>)</param>
        /// <param name="useIpV4">Search for Alpaca devices that use IPv4 addresses. One or both of useIpV4 and useIpV6 must be True. (Default <see langword="true"/>)</param>
        /// <param name="useIpV6">Search for Alpaca devices that use IPv6 addresses. One or both of useIpV4 and useIpV6 must be True. (Default <see langword="false"/>)</param>
        /// <param name="serviceType"><see cref="ServiceType.Http"/> or <see cref="ServiceType.Https"/></param>
        public void StartDiscovery(int numberOfPolls = NUMBER_OF_POLLS_DEFAULT,
            int pollInterval = DISCOVERY_POLL_INTERVAL_DEFAULT,
            int discoveryPort = DISCOVERY_PORT_DEFAULT,
            double discoveryDuration = DISCOVERY_DURATION_DEFAULT,
            bool resolveDnsName = RESOLVE_DNS_NAME_DEFAULT,
            bool useIpV4 = USE_IP_V4_DEFAULT,
            bool useIpV6 = USE_IP_V6_DEFAULT,
            ServiceType serviceType = SERVICE_TYPE_DEFAULT)
        {

            // Validate parameters
            if (numberOfPolls < 1 | numberOfPolls > 5)
                throw new InvalidValueException($"StartDiscovery - NumberOfPolls: {numberOfPolls} is not within the valid range of 1::5");
            if (pollInterval < 10 | pollInterval > 60000)
                throw new InvalidValueException($"StartDiscovery - PollInterval: {pollInterval} is not within the valid range of 10::5000");
            if (discoveryPort < 1025 | discoveryPort > 65535)
                throw new InvalidValueException($"StartDiscovery - DiscoveryPort: {discoveryPort} is not within the valid range of 1025::65535");
            if (discoveryDuration < 0.0d)
                throw new InvalidValueException($"StartDiscovery - DiscoverDuration: {discoveryDuration} must be greater than 0.0");
            if (!(useIpV4 | useIpV6))
                throw new InvalidValueException("StartDiscovery: Both the use IPv4 and use IPv6 flags are false. At least one of these must be set True.");
            if (!discoveryCompleteValue)
                throw new global::ASCOM.InvalidOperationException("Cannot start a new discovery because a previous discovery is still running.");

            // Save supplied parameters for use within the application 
            discoveryTime = discoveryDuration;
            tryDnsNameResolution = resolveDnsName;
            this.serviceType = serviceType;

            // Prepare for a new search
            LogMessage("StartDiscovery", $"Starting search for Alpaca devices with timeout: {discoveryTime} Broadcast polls: {numberOfPolls} sent every {pollInterval} milliseconds");
            finder.ClearCache();

            // Clear the device list dictionary
            lock (deviceListLockObject) // Make sure that the clear operation is not interrupted by other threads
                alpacaDeviceList.Clear();
            discoveryCompleteTimer.Change(Convert.ToInt32(discoveryTime * 1000d), Timeout.Infinite);
            discoveryCompleteValue = false;
            discoveryStartTime = DateTime.Now; // Save the start time

            // Create a new HTTP Client so that its timeout property can be set

            if (httpClient != null)
            {
                httpClient.Dispose();
            }
            httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(discoveryDuration)
            };

            // Send the broadcast polls
            for (int i = 1, loopTo = numberOfPolls; i <= loopTo; i++)
            {
                LogMessage("StartDiscovery", $"Sending poll {i}...");
                finder.Search(discoveryPort, useIpV4, useIpV6);
                LogMessage("StartDiscovery", $"Poll {i} sent.");
                if (i < numberOfPolls)
                    Thread.Sleep(pollInterval); // Sleep after sending the poll, except for the last one
            }

            LogMessage("StartDiscovery", "Alpaca device broadcast polls completed, discovery started");
        }

        #endregion

        #region Asynchronous methods

        /// <summary>
        /// Returns an awaitable Task that provides a list of discovered ASCOM devices of the specified device type 
        /// </summary>
        /// <param name="deviceTypes">ASCOM device type to discover e.g. <see cref="DeviceTypes.Telescope"/> or <see cref="DeviceTypes.Camera"/> </param>
        /// <param name="numberOfPolls">Number of polls to send in the range 1 to 5</param>
        /// <param name="pollInterval">Interval between each poll in the range 10 to 5000 milliseconds.</param>
        /// <param name="discoveryPort">Discovery port on which to send the broadcast (normally 32227) in the range 1025 to 65535.</param>
        /// <param name="discoveryDuration">Length of time (seconds) to wait for devices to respond.</param>
        /// <param name="resolveDnsName">Attempt to resolve host IP addresses to DNS names</param>
        /// <param name="useIpV4">Search for Alpaca devices that use IPv4 addresses. One or both of useIpV4 and useIpV6 must be True.</param>
        /// <param name="useIpV6">Search for Alpaca devices that use IPv6 addresses. One or both of useIpV4 and useIpV6 must be True.</param>
        /// <param name="serviceType"><see cref="ServiceType.Http"/> or <see cref="ServiceType.Https"/></param>
        /// <param name="logger"></param>
        /// <returns>Returns an awaitable Task</returns>
        public static async Task<List<AscomDevice>> GetAscomDevicesAsync(DeviceTypes ?deviceTypes,
            int numberOfPolls = NUMBER_OF_POLLS_DEFAULT,
            int pollInterval = DISCOVERY_POLL_INTERVAL_DEFAULT,
            int discoveryPort = DISCOVERY_PORT_DEFAULT,
            double discoveryDuration = DISCOVERY_DURATION_DEFAULT,
            bool resolveDnsName = RESOLVE_DNS_NAME_DEFAULT,
            bool useIpV4 = USE_IP_V4_DEFAULT,
            bool useIpV6 = USE_IP_V6_DEFAULT,
            ServiceType serviceType = SERVICE_TYPE_DEFAULT,
            ILogger logger = null)
        {
            try
            {
                logger.LogMessage(LogLevel.Information, "GetAscomDevicesAsync", $"Parameters - DeviceType: {deviceTypes}, Number of polls: {numberOfPolls}, Poll interval: {pollInterval}, Discovery port: {discoveryPort}");
                logger.LogMessage(LogLevel.Information, "GetAscomDevicesAsync", $"Parameters - Discovery duration: {discoveryDuration}, Resolve DNS names: {resolveDnsName}, Use IPv4: {useIpV4}, Use IP v6: {useIpV6}, Service type: {serviceType}");

                iLogger = logger;
                logger.LogMessage(LogLevel.Information, "GetAscomDevicesAsync", $"Entered method");

                // Create and use a discovery instance to look for ALpaca devices
                using (AlpacaDiscovery discovery = new AlpacaDiscovery(true, logger))
                {
                    logger.LogMessage(LogLevel.Information, "GetAscomDevicesAsync", $"Created discovery device");

                    // Create and run an async task to effect the discovery
                    await Task.Run(async () =>
                    {
                        logger.LogMessage(LogLevel.Information, "GetAscomDevicesAsync", $"About to start discovery");

                        // Start discovery using the AlpacaDiscovery instance
                        discovery.StartDiscovery(numberOfPolls, pollInterval, discoveryPort, discoveryDuration, resolveDnsName, useIpV4, useIpV6, serviceType);
                        logger.LogMessage(LogLevel.Information, "GetAscomDevicesAsync", $"Discovery started");

                        // Run the DiscoveryCompletedTask task and wait for it to be marked complete when the DiscoveryCompleted fires
                        await DiscoveryCompletedTask(discovery);
                        logger.LogMessage(LogLevel.Information, "GetAscomDevicesAsync", $"Discovery Completed Task has fired");

                        // Remove event handlers attached to the event
                        foreach (Delegate discoveryCompletedDelegate in discovery.DiscoveryCompleted.GetInvocationList())
                        {
                            discovery.DiscoveryCompleted -= (EventHandler)discoveryCompletedDelegate;
                            logger.LogMessage(LogLevel.Information, "GetAscomDevicesAsync", $"Removing event handler: {discoveryCompletedDelegate.Method.Name}");
                        }

                    });

                    // Log the outcome
                    logger.LogMessage(LogLevel.Information, "GetAscomDevicesAsync", $"Returning {discovery.GetAscomDevices(deviceTypes).Count} devices.");

                    // Return the discovered device list to the caller
                    return discovery.GetAscomDevices(deviceTypes);
                }
            }
            catch (Exception ex)
            {
                logger.LogMessage(LogLevel.Error, "GetAscomDevicesAsync", $"Exception: {ex}");
                throw;
            }
        }

        /// <summary>
        /// Returns an awaitable Task that provides a list of discovered ASCOM devices of the specified device type 
        /// </summary>
        /// <param name="numberOfPolls">Number of polls to send in the range 1 to 5</param>
        /// <param name="pollInterval">Interval between each poll in the range 10 to 5000 milliseconds.</param>
        /// <param name="discoveryPort">Discovery port on which to send the broadcast (normally 32227) in the range 1025 to 65535.</param>
        /// <param name="discoveryDuration">Length of time (seconds) to wait for devices to respond.</param>
        /// <param name="resolveDnsName">Attempt to resolve host IP addresses to DNS names</param>
        /// <param name="useIpV4">Search for Alpaca devices that use IPv4 addresses. One or both of useIpV4 and useIpV6 must be True.</param>
        /// <param name="useIpV6">Search for Alpaca devices that use IPv6 addresses. One or both of useIpV4 and useIpV6 must be True.</param>
        /// <param name="serviceType"><see cref="ServiceType.Http"/> or <see cref="ServiceType.Https"/></param>
        /// <param name="logger"></param>
        /// <returns>Returns an awaitable Task</returns>
        public static async Task<List<AlpacaDevice>> GetAlpacaDevicesAsync(int numberOfPolls = NUMBER_OF_POLLS_DEFAULT,
            int pollInterval = DISCOVERY_POLL_INTERVAL_DEFAULT,
            int discoveryPort = DISCOVERY_PORT_DEFAULT,
            double discoveryDuration = DISCOVERY_DURATION_DEFAULT,
            bool resolveDnsName = RESOLVE_DNS_NAME_DEFAULT,
            bool useIpV4 = USE_IP_V4_DEFAULT,
            bool useIpV6 = USE_IP_V6_DEFAULT,
            ServiceType serviceType = SERVICE_TYPE_DEFAULT,
            ILogger logger = null)
        {
            try
            {
                logger.LogMessage(LogLevel.Information, "GetAlpacaDevicesAsync", $"Parameters - Number of polls: {numberOfPolls}, Poll interval: {pollInterval}, Discovery port: {discoveryPort}");
                logger.LogMessage(LogLevel.Information, "GetAlpacaDevicesAsync", $"Parameters - Discovery duration: {discoveryDuration}, Resolve DNS names: {resolveDnsName}, Use IPv4: {useIpV4}, Use IP v6: {useIpV6}, Service type: {serviceType}");

                iLogger = logger;
                logger.LogMessage(LogLevel.Information, "GetAlpacaDevicesAsync", $"Entered method");

                // Create and use a discovery instance to look for ALpaca devices
                using (AlpacaDiscovery discovery = new AlpacaDiscovery(true, logger))
                {
                    logger.LogMessage(LogLevel.Information, "GetAlpacaDevicesAsync", $"Created discovery device");

                    // Create and run an async task to effect the discovery
                    await Task.Run(async () =>
                    {
                        logger.LogMessage(LogLevel.Information, "GetAlpacaDevicesAsync", $"About to start discovery");

                        // Start discovery using the AlpacaDiscovery instance
                        discovery.StartDiscovery(numberOfPolls, pollInterval, discoveryPort, discoveryDuration, resolveDnsName, useIpV4, useIpV6, serviceType);
                        logger.LogMessage(LogLevel.Information, "GetAlpacaDevicesAsync", $"Discovery started");

                        // Run the DiscoveryCompletedTask task and wait for it to be marked complete when the DiscoveryCompleted fires
                        await DiscoveryCompletedTask(discovery);
                        logger.LogMessage(LogLevel.Information, "GetAlpacaDevicesAsync", $"Discovery Completed Task has fired");

                        // Remove event handlers attached to the event
                        foreach (Delegate discoveryCompletedDelegate in discovery.DiscoveryCompleted.GetInvocationList())
                        {
                            discovery.DiscoveryCompleted -= (EventHandler)discoveryCompletedDelegate;
                            logger.LogMessage(LogLevel.Information, "GetAlpacaDevicesAsync", $"Removing event handler: {discoveryCompletedDelegate.Method.Name}");
                        }

                    });

                    // Log the outcome
                    logger.LogMessage(LogLevel.Information, "GetAlpacaDevicesAsync", $"Returning {discovery.GetAlpacaDevices().Count} devices.");

                    // Return the discovered device list to the caller
                    return discovery.GetAlpacaDevices();
                }
            }
            catch (Exception ex)
            {
                logger.LogMessage(LogLevel.Error, "GetAlpacaDevicesAsync", $"Exception: {ex}");
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alpacaDiscovery"></param>
        /// <returns></returns>
        private static Task DiscoveryCompletedTask(AlpacaDiscovery alpacaDiscovery)
        {
            try
            {
                iLogger.LogMessage(LogLevel.Information, "DiscoveryCompletedTask", $"Creating TaskCompletionSource");

                TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
                iLogger.LogMessage(LogLevel.Information, "DiscoveryCompletedTask", $"Adding DiscoveryCompleted event handler.");

                alpacaDiscovery.DiscoveryCompleted += (sender, eventArgs) =>
                {
                    iLogger.LogMessage(LogLevel.Information, "DiscoveryCompletedTask", "Setting TaskCompletionSource task result.");
                    tcs.SetResult(null);
                };
                iLogger.LogMessage(LogLevel.Information, "DiscoveryCompletedTask", $"Returning TaskCompletionSource task.");

                return tcs.Task;
            }
            catch (Exception ex)
            {
                iLogger.LogMessage(LogLevel.Error, "DiscoveryCompletedTask", $"Exception: {ex}");

                throw;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Raise an Alpaca devices updated event
        /// </summary>
        private void RaiseAnAlpacaDevicesChangedEvent()
        {
            AlpacaDevicesUpdated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Discovery timer event handler - called when the allocated discovery period has ended
        /// </summary>
        /// <param name="state">Timer state</param>
        private void OnDiscoveryCompleteTimer(object state)
        {
            LogMessage("OnTimeOutTimerFired", $"Firing discovery complete event");
            discoveryCompleteValue = true; // Flag that the timer out has expired
            bool statusMessagesUpdated = false;

            // Update the status messages of management API calls that didn't connect in time
            lock (deviceListLockObject) // Make sure that the device list dictionary can't change while being read and that only one thread can update it at a time
            {
                foreach (KeyValuePair<IPEndPoint, AlpacaDevice> alpacaDevice in alpacaDeviceList)
                {
                    if (ReferenceEquals(alpacaDevice.Value.StatusMessage, TRYING_TO_CONTACT_MANAGEMENT_API_MESSAGE))
                    {
                        alpacaDevice.Value.StatusMessage = FAILED_TO_CONTACT_MANAGEMENT_API_MESSAGE;
                        statusMessagesUpdated = true;
                    }
                }
            }

            if (statusMessagesUpdated)
                RaiseAnAlpacaDevicesChangedEvent(); // Raise a devices changed event if any status messages have been updated
            DiscoveryCompleted?.Invoke(this, EventArgs.Empty); // Raise an event to indicate that discovery is complete
        }

        /// <summary>
        /// Handler for device responses coming from the Finder
        /// </summary>
        /// <param name="caller">Initiating object.</param>
        /// <param name="responderIPEndPoint">Responder's IP address and port</param>
        private void FoundDeviceEventHandler(object caller, IPEndPoint responderIPEndPoint)
        {
            try
            {
                LogMessage("FoundDeviceEventHandler", $"FOUND Alpaca device at {responderIPEndPoint.Address}:{responderIPEndPoint.Port}"); // Log reception of the broadcast response

                // Add the new device or ignore this duplicate if it already exists
                lock (deviceListLockObject) // Make sure that the device list dictionary can't change while being read and that only one thread can update it at a time
                {
                    if (!alpacaDeviceList.ContainsKey(responderIPEndPoint))
                    {
                        alpacaDeviceList.Add(responderIPEndPoint, new AlpacaDevice(serviceType, responderIPEndPoint, TRYING_TO_CONTACT_MANAGEMENT_API_MESSAGE));
                        RaiseAnAlpacaDevicesChangedEvent(); // Device was added so set the changed flag
                    }
                }

                // Create a task to query this device's DNS name, if configured to do so
                if (tryDnsNameResolution)
                {
                    LogMessage("FoundDeviceEventHandler", $"Creating task to retrieve DNS information for device {responderIPEndPoint}:{responderIPEndPoint.Port}");
                    var dnsResolutionThread = new Thread(ResolveIpAddressToHostName)
                    {
                        IsBackground = true
                    };
                    dnsResolutionThread.Start(responderIPEndPoint);
                }

                // Create a task to query this device's Alpaca management API
                LogMessage("FoundDeviceEventHandler", $"Creating thread to retrieve Alpaca management description for device {responderIPEndPoint}:{responderIPEndPoint.Port}");
                var descriptionThread = new Thread(GetAlpacaDeviceInformation)
                {
                    IsBackground = true
                };
                descriptionThread.Start(responderIPEndPoint);
            }
            catch (Exception ex)
            {
                LogMessage("FoundDeviceEventHandler", $"AddresssFound Exception: {ex}");
            }
        }

        /// <summary>
        /// Get Alpaca device information from the management API
        /// </summary>
        /// <param name="deviceIpEndPointObject"></param>
        private async void GetAlpacaDeviceInformation(object deviceIpEndPointObject)
        {
            IPEndPoint deviceIpEndPoint = deviceIpEndPointObject as IPEndPoint;
            string hostIpAndPort;

            // Create a text version of the host IP address and port
            switch (deviceIpEndPoint.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                    hostIpAndPort = $"{serviceType.ToString().ToLowerInvariant()}://{deviceIpEndPoint}";
                    break;

                case AddressFamily.InterNetworkV6:
                    string scopeId = $"%{deviceIpEndPoint.Address.ScopeId}"; // Obtain the IPv6 scope ID in text form (if present)
                    hostIpAndPort = $"{serviceType.ToString().ToLowerInvariant()}://{deviceIpEndPoint.ToString().Replace(scopeId, string.Empty)}"; // Create the overall URI
                    break;

                default:
                    hostIpAndPort = $"{serviceType.ToString().ToLowerInvariant()}://{deviceIpEndPoint}";
                    break;
            }

            try
            {
                LogMessage("GetAlpacaDeviceInformation", $"Host URL: {hostIpAndPort} DISCOVERY TIMEOUT: {discoveryTime} ({discoveryTime * 1000d})");

                // Wait for API version result and process it
                LogMessage("GetAlpacaDeviceInformation", $"About to get version information from {hostIpAndPort}/management/apiversions at IP endpoint {deviceIpEndPoint.Address} {deviceIpEndPoint.AddressFamily}");
                string apiVersionsJsonResponse = await httpClient.GetStringAsync($"{hostIpAndPort}/management/apiversions");
                LogMessage("GetAlpacaDeviceInformation", $"Received JSON response from {hostIpAndPort}: {apiVersionsJsonResponse}");
                IntArray1DResponse apiVersionsResponse = JsonSerializer.Deserialize<IntArray1DResponse>(apiVersionsJsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = strictCasing });
                lock (deviceListLockObject) // Make sure that only one thread can update the device list dictionary at a time
                {
                    alpacaDeviceList[deviceIpEndPoint].SupportedInterfaceVersions = apiVersionsResponse.Value;
                    alpacaDeviceList[deviceIpEndPoint].StatusMessage = ""; // Clear the status field to indicate that this first call was successful
                }

                RaiseAnAlpacaDevicesChangedEvent(); // Device list was changed so set the changed flag

                // Wait for device description result and process it
                string deviceDescriptionJsonResponse = await httpClient.GetStringAsync($"{hostIpAndPort}/management/v1/description");
                LogMessage("GetAlpacaDeviceInformation", $"Received JSON response from {hostIpAndPort}: {deviceDescriptionJsonResponse}");
                var deviceDescriptionResponse = JsonSerializer.Deserialize<AlpacaDescriptionResponse>(deviceDescriptionJsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = strictCasing });
                lock (deviceListLockObject) // Make sure that only one thread can update the device list dictionary at a time
                {
                    alpacaDeviceList[deviceIpEndPoint].ServerName = deviceDescriptionResponse.Value.ServerName;
                    alpacaDeviceList[deviceIpEndPoint].Manufacturer = deviceDescriptionResponse.Value.Manufacturer;
                    alpacaDeviceList[deviceIpEndPoint].ManufacturerVersion = deviceDescriptionResponse.Value.ManufacturerVersion;
                    alpacaDeviceList[deviceIpEndPoint].Location = deviceDescriptionResponse.Value.Location;
                }

                RaiseAnAlpacaDevicesChangedEvent(); // Device list was changed so set the changed flag

                // Wait for configured devices result and process it
                string configuredDevicesJsonResponse = await httpClient.GetStringAsync($"{hostIpAndPort}/management/v1/configureddevices");
                LogMessage("GetAlpacaDeviceInformation", $"Received JSON response from {hostIpAndPort}: {configuredDevicesJsonResponse}");
                AlpacaConfiguredDevicesResponse configuredDevicesResponse = JsonSerializer.Deserialize<AlpacaConfiguredDevicesResponse>(configuredDevicesJsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = strictCasing });
                lock (deviceListLockObject) // Make sure that only one thread can update the device list dictionary at a time
                {
                    alpacaDeviceList[deviceIpEndPoint].ConfiguredDevices = configuredDevicesResponse.Value;
                    LogMessage("GetAlpacaDeviceInformation", $"Listing configured devices");
                    foreach (AlpacaConfiguredDevice configuredDevce in alpacaDeviceList[deviceIpEndPoint].ConfiguredDevices)
                        LogMessage("GetAlpacaDeviceInformation", $"Found configured device: {configuredDevce.DeviceName} {configuredDevce.DeviceType} {configuredDevce.UniqueID}");
                    LogMessage("GetAlpacaDeviceInformation", $"Completed list of configured devices");
                }

                RaiseAnAlpacaDevicesChangedEvent(); // Device list was changed so set the changed flag

                LogMessage("GetAlpacaDeviceInformation", $"COMPLETED API tasks for {hostIpAndPort}");
            }
            catch (TaskCanceledException)
            {
                LogMessage("GetAlpacaDeviceInformation", $"Task cancelled while getting information from {hostIpAndPort}");
            }
            catch (TimeoutException)
            {
                LogMessage("GetAlpacaDeviceInformation", $"Timed out getting information from {hostIpAndPort}");
            }
            catch (Exception ex)
            {
                // Something went wrong so log the issue and sent a message to the user
                LogMessage("GetAlpacaDeviceInformation", $"GetAlpacaDescriptions exception: {ex}");
                lock (deviceListLockObject) // Make sure that only one thread can update the device list dictionary at a time
                {
                    alpacaDeviceList[deviceIpEndPoint].StatusMessage = ex.Message;
                    RaiseAnAlpacaDevicesChangedEvent(); // Device list was changed so set the changed flag
                }
            }
        }

        /// <summary>
        /// Resolve a host IP address to a host name
        /// </summary>
        /// <remarks>This first makes a DNS query and uses the result if found. If not found it then tries a Microsoft DNS call which also searches the local hosts and makes a netbios query.
        /// If this returns an answer it is use. Otherwise the IP address is returned as the host name</remarks>
        private void ResolveIpAddressToHostName(object deviceIpEndPointObject)
        {
            IPEndPoint deviceIpEndPoint = null;
            try
            {
                deviceIpEndPoint = deviceIpEndPointObject as IPEndPoint; // Get the supplied device endpoint as an IPEndPoint

                // test whether the cast was successful
                if (deviceIpEndPoint is object) // The cast was successful so we can try to search for the host name
                {
                    var dnsResponse = new DnsResponse(); // Create a new DnsResponse to hold and return the 

                    // Calculate the remaining time before this discovery needs to finish and only undertake DNS resolution if sufficient time remains
                    var timeOutTime = TimeSpan.FromSeconds(discoveryTime).Subtract(DateTime.Now - discoveryStartTime).Subtract(TimeSpan.FromSeconds(0.2d));
                    if (timeOutTime.TotalSeconds > MINIMUM_TIME_REMAINING_TO_UNDERTAKE_DNS_RESOLUTION) // We have more than the configured time left so we will attempt a reverse DNS name resolution
                    {
                        LogMessage("ResolveIpAddressToHostName", $"Resolving IP address: {deviceIpEndPoint.Address}, Timeout: {timeOutTime}");
                        Dns.BeginGetHostEntry(deviceIpEndPoint.Address.ToString(), new AsyncCallback(GetHostEntryCallback), dnsResponse);

                        // Wait here until the resolve completes and the callback calls .Set()
                        bool dnsWasResolved = dnsResponse.CallComplete.WaitOne(timeOutTime); // Wait for the remaining discovery time

                        // Execution continues here after either a DNS response is found or the request times out
                        if (dnsWasResolved) // A response was received rather than timing out
                        {
                            LogMessage("ResolveIpAddressToHostName", $"{deviceIpEndPoint} has host name: {dnsResponse.HostName} IP address count: {dnsResponse.AddressList.Length} Alias count: {dnsResponse.Aliases.Length}");
                            foreach (IPAddress address in dnsResponse.AddressList)
                                LogMessage("ResolveIpAddressToHostName", $"  Received {address.AddressFamily} address: {address}");
                            foreach (string hostAlias in dnsResponse.Aliases)
                                LogMessage("ResolveIpAddressToHostName", $"  Received alias: {hostAlias}");
                            if (dnsResponse.AddressList.Length > 0) // We got a reply that contains host addresses so there may be a valid host name
                            {
                                lock (deviceListLockObject)
                                {
                                    if (!string.IsNullOrEmpty(dnsResponse.HostName))
                                        alpacaDeviceList[deviceIpEndPoint].HostName = dnsResponse.HostName;
                                }

                                RaiseAnAlpacaDevicesChangedEvent(); // Device list was changed so set the changed flag
                            }
                            else
                            {
                                LogMessage("ResolveIpAddressToHostName", $"***** DNS responded with a name ({dnsResponse.HostName}) but this has no associated IP addresses and is probably a NETBIOS name *****");
                            }

                            foreach (IPAddress address in dnsResponse.AddressList)
                                LogMessage("ResolveIpAddressToHostName", $"Address: {address}");
                            foreach (string alias in dnsResponse.Aliases)
                                LogMessage("ResolveIpAddressToHostName", $"Alias: {alias}");
                        }
                        else // DNS did not respond in time
                        {
                            LogMessage("ResolveIpAddressToHostName", $"***** DNS did not respond within timeout - unable to resolve IP address to host name *****");
                        }
                    }
                    else // There was insufficient time to query DNS
                    {
                        LogMessage("ResolveIpAddressToHostName", $"***** Insufficient time remains ({timeOutTime.TotalSeconds} seconds) to conduct a DNS query, ignoring request *****");
                    }
                }
                else // The IPEndPoint cast was not successful so we cannot carry out a DNS name search because we don't have the device's IP address
                {
                    LogMessage("ResolveIpAddressToHostName", $"DNS resolution could not be undertaken - It was not possible to cast the supplied IPEndPoint object to an IPEndPoint type: {deviceIpEndPoint}.");
                }
            }
            catch (TimeoutException)
            {
                LogMessage("ResolveIpAddressToHostName", $"Timed out trying to resolve the DNS name for {(deviceIpEndPoint is null ? "Unknown IP address" : deviceIpEndPoint.ToString())}");
            }
            catch (Exception ex)
            {
                // Something went wrong, so log the issue and sent a message to the user
                LogMessage("ResolveIpAddressToHostName", $"Exception: {ex}");
            }
        }

        /// <summary>
        /// Record the IPs in the state object for later use.
        /// </summary>
        private void GetHostEntryCallback(IAsyncResult ar)
        {
            try
            {
                DnsResponse dnsResponse = (DnsResponse)ar.AsyncState; // Turn the state object into the DnsResponse type
                dnsResponse.IpHostEntry = Dns.EndGetHostEntry(ar); // Save the returned IpHostEntry and populate other fields based on its parameters
                dnsResponse.CallComplete.Set(); // Set the wait handle so that the caller knows that the asynchronous call has completed and that the response has been updated
            }
            catch (SocketException ex)
            {
                LogMessage("GetHostEntryCallback", $"Socket Exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                LogMessage("GetHostEntryCallback", $"Exception: {ex}");
            } // Log exceptions but don't throw them
        }

        /// <summary>
        /// Log a message to the screen, adding the current managed thread ID
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="message"></param>
        private void LogMessage(string methodName, string message)
        {
            logger.LogMessage(LogLevel.Information, $"AlpacaDiscovery - {methodName}", $"{Thread.CurrentThread.ManagedThreadId,2} {message}");
        }

        #endregion

    }
}