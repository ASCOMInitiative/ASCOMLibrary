using ASCOM.Common.Alpaca;
using ASCOM.Common.Com;
using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.Interfaces;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ASCOM.Alpaca.Clients
{
    internal static class DynamicClientDriver
    {
        #region Private variables and constants

        // Private constants
        private const int DYNAMIC_DRIVER_ERROR_NUMBER = 4095; // Alpaca error number that will be returned when a required JSON "Value" element is either absent from the response or is set to "null"

        //Private variables
        private static uint uniqueTransactionNumber = 0; // Unique number that increments on each call to TransactionNumber

        // Lock objects
        private readonly static object connectLockObject = new object();
        private readonly static object transactionCountlockObject = new object();

        private static readonly ConcurrentDictionary<long, bool> connectStates;

        #endregion

        #region Initialiser

        /// <summary>
        /// Static initialiser to set up the objects we need at run time
        /// </summary>
        static DynamicClientDriver()
        {
            connectStates = new ConcurrentDictionary<long, bool>();
        }

        #endregion

        #region Utility code

        /// <summary>
        /// Returns a unique client number to the calling instance in the range 1::65536
        /// </summary>
        public static uint GetUniqueClientNumber()
        {
            uint randomvalue;

            using (RNGCryptoServiceProvider rg = new RNGCryptoServiceProvider())
            {
                byte[] rno = new byte[5]; // Create a four byte array
                rg.GetBytes(rno); // Fill the array with four random bytes
                rno[2] = 0; // Zero the higher two bytes to limit the resulting integer to the range 0::65535
                rno[3] = 0;
                rno[4] = 0;
                randomvalue = BitConverter.ToUInt32(rno, 0) + 1; // Convert the bytes to an integer in the range 0::65535 and add 1 to get an integer in the range 1::65536
            }

            return randomvalue;
        }

        /// <summary>
        /// Returns a unique client number to the calling instance
        /// </summary>
        public static uint TransactionNumber()
        {
            lock (transactionCountlockObject)
            {
                uniqueTransactionNumber += 1;
            }
            return uniqueTransactionNumber;
        }

        /// <summary>
        /// Test whether a particular client is already connected
        /// </summary>
        /// <param name="clientNumber">Number of the calling client</param>
        /// <returns></returns>
        public static bool IsClientConnected(uint clientNumber, ILogger TL)
        {
            foreach (KeyValuePair<long, bool> kvp in connectStates)
            {
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "IsClientConnected", $"This device ClientID is in the ConnectedStates list: {kvp.Key}, Value: {kvp.Value}");
            }

            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "IsClientConnected", "Number of connected devices: " + connectStates.Count + ", Returning: " + connectStates.ContainsKey(clientNumber).ToString());

            return connectStates.ContainsKey(clientNumber);
        }

        /// <summary>
        /// Returns the number of connected clients
        /// </summary>
        public static uint ConnectionCount(ILogger TL)
        {
            AlpacaDeviceBaseClass.LogMessage(TL, 0, "ConnectionCount", connectStates.Count.ToString());
            return (uint)connectStates.Count;
        }

        /// <summary>
        /// Return name of current method
        /// </summary>
        /// <returns>Name of current method</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }

        /// <summary>
        /// Set the REST client communications timeout for the next transaction
        /// </summary>
        /// <param name="client">REST client to use</param>
        /// <param name="deviceResponseTimeout">Timeout to be set</param>
        public static void SetClientTimeout(RestClient client, int deviceResponseTimeout)
        {
            client.Timeout = deviceResponseTimeout * 1000;
            client.ReadWriteTimeout = deviceResponseTimeout * 1000;
        }

        /// <summary>
        /// Create and configure a REST client to communicate with the Alpaca device
        /// </summary>
        /// <param name="client"></param>
        /// <param name="ipAddressString"></param>
        /// <param name="portNumber"></param>
        /// <param name="connectionTimeout"></param>
        /// <param name="serviceType"></param>
        /// <param name="TL"></param>
        /// <param name="clientNumber"></param>
        /// <param name="deviceType"></param>
        /// <param name="deviceResponseTimeout"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="uniqueId"></param>
        /// <remarks>This method will attempt to re-discover the Alpaca device if it is not possible to establish a TCP connection with the device at the specified address and port.</remarks>
        public static void ConnectToRemoteDevice(ref RestClient client, ServiceType serviceType, string ipAddressString, decimal portNumber,
                                                 uint clientNumber, string deviceType, int deviceResponseTimeout, string userName, string password, ILogger TL)
        {
            string clientHostAddress = $"{serviceType.ToString().ToLowerInvariant()}://{ipAddressString}:{portNumber}";
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, deviceType, $"Connecting to device: {ipAddressString}:{portNumber} through URL: {clientHostAddress}");

            #region Commented automatic Alpaca device rediscovery code
            // Test whether automatic Alpaca device rediscovery is enabled for this device
            //if (enableRediscovery) // Automatic rediscovery is enabled
            //{
            //    AlpacaDeviceBaseClass.LogMessage(TL,clientNumber, deviceType, $"Testing whether client at address {clientHostAddress} can be contacted.");

            //    // Test whether there is a device at the configured IP address and port by trying to open a TCP connection to it
            //    if (!ClientIsUp(ipAddressString, portNumber, connectionTimeout, TL, clientNumber)) // It was not possible to establish TCP communication with a device at the IP address provided
            //    {
            //        // Attempt to "re-discover" the device and use it's new address and / or port
            //        AlpacaDeviceBaseClass.LogMessage(TL,clientNumber, deviceType, $"The device at the configured IP address and port {ipAddressString} cannot be contacted, attempting to re-discover it");

            //        // Create an AlapcaDiscovery component to conduct the search
            //        using (AlpacaDiscovery alpacaDiscovery = new AlpacaDiscovery())
            //        {
            //            // Start a discovery using two polls, 100ms apart, timing out after 2 seconds, don't attempt to resolve the IP address to a DNS name use the discovery port and IP settings of this device
            //            alpacaDiscovery.StartDiscovery(2, 100, discoveryPort, 2.0, false, ipV4Enabled, ipV6Enabled);

            //            // Wait for the discovery cycle to complete, making sure that the UI remains responsive
            //            do
            //            {
            //                Thread.Sleep(10);
            //            } while (!alpacaDiscovery.DiscoveryComplete);

            //            // Get a list of the discovered Alpaca devices
            //            List<AlpacaDevice> discoveredDevices = alpacaDiscovery.GetAlpacaDevices();

            //            // Iterate over these to find which ASCOM devices are served by them
            //            foreach (AlpacaDevice alpacaDevice in discoveredDevices)
            //            {
            //                AlpacaDeviceBaseClass.LogMessage(TL,clientNumber, deviceType, $"Found Alpaca device {alpacaDevice.HostName}:{alpacaDevice.Port} - {alpacaDevice.ServerName}");

            //                // Iterate over the devices served by the Alpaca device
            //                foreach (ConfiguredDevice ascomDevice in alpacaDevice.ConfiguredDevices)
            //                {
            //                    AlpacaDeviceBaseClass.LogMessage(TL,clientNumber, deviceType, $"Found ASCOM device {ascomDevice.DeviceName}:{ascomDevice.DeviceType} - {ascomDevice.UniqueID} at {alpacaDevice.HostName}:{alpacaDevice.Port}");

            //                    // Test whether the found ASCOM device has the same unique ID as the device for which we are looking
            //                    if (ascomDevice.UniqueID.ToLowerInvariant() == uniqueId.ToLowerInvariant()) // We have a match so we can use this address and port instead of the configured values that no longer work
            //                    {
            //                        AlpacaDeviceBaseClass.LogMessage(TL,clientNumber, deviceType, $"  *** Found REQUIRED ASCOM device ***");

            //                        // Get the IP address as a big endian byte array
            //                        byte[] addressBytes = IPAddress.Parse(alpacaDevice.HostName).GetAddressBytes();

            //                        // Create an array large enough to hold an IPv6 address (16 bytes) plus one extra byte at the high end that will always be 0.
            //                        // This ensures that the IPv6 address will not be interpreted as a negative number if its top bit is set
            //                        byte[] hostBytes = new byte[17];

            //                        // Re-order the network address byte array to little endian as used in Windows
            //                        Array.Copy(addressBytes.Reverse().ToArray<byte>(), hostBytes, addressBytes.Length);

            //                        // Create a big integer from the little endian byte array
            //                        BigInteger bigIntegerAddress = new BigInteger(hostBytes);

            //                        // Create a new structure to hold the interface information and add it to the list of interfaces
            //                        AvailableInterface availableInterface = new AvailableInterface();
            //                        availableInterface.HostName = alpacaDevice.HostName;
            //                        availableInterface.Port = alpacaDevice.Port;
            //                        availableInterface.IpAddress = bigIntegerAddress;
            //                        availableInterfaces.Add(availableInterface);

            //                    }
            //                }
            //                TL.BlankLine();
            //            }

            //        }

            //        // Search the discovered interfaces for the one whose network address is closest to the original address
            //        // This will ensure that we pick an address on the original subnet if this is available.
            //        switch (availableInterfaces.Count)
            //        {
            //            case 0:
            //                AlpacaDeviceBaseClass.LogMessage(TL,clientNumber, deviceType, $"No ASCOM device was discovered that had a UniqueD of {uniqueId}");
            //                TL.BlankLine();
            //                break;

            //            case 1:
            //                // Update the client host address with the newly discovered address and port
            //                clientHostAddress = $"{serviceType}://{availableInterfaces[0].HostName}:{availableInterfaces[0].Port}";
            //                AlpacaDeviceBaseClass.LogMessage(TL,clientNumber, deviceType, $"One ASCOM device was discovered that had a UniqueD of {uniqueId}. Now using URL: {clientHostAddress}");

            //                // Write the new value to the driver's Profile so it is found immediately in future
            //                using (Profile profile = new Profile())
            //                {
            //                    profile.DeviceType = deviceType;
            //                    profile.WriteValue(driverProgId, SharedConstants.IPADDRESS_PROFILENAME, availableInterfaces[0].HostName);
            //                    profile.WriteValue(driverProgId, SharedConstants.PORTNUMBER_PROFILENAME, availableInterfaces[0].Port.ToString());
            //                    AlpacaDeviceBaseClass.LogMessage(TL,clientNumber, deviceType, $"Written new values {availableInterfaces[0].HostName} and {availableInterfaces[0].Port} to profile {driverProgId}");
            //                }

            //                TL.BlankLine();
            //                break;

            //            default:
            //                AlpacaDeviceBaseClass.LogMessage(TL,clientNumber, deviceType, $"{availableInterfaces.Count} ASCOM devices were discovered that had a UniqueD of {uniqueId}.");

            //                // Get the original IP address as a big endian byte array
            //                byte[] addressBytes = new byte[0]; // Create a zero length array in case its not possible to parse the IP address string (it may be a host name or may just be corrupted)

            //                try
            //                {
            //                    addressBytes = IPAddress.Parse(ipAddressString).GetAddressBytes();
            //                }
            //                catch { }

            //                // Create an array large enough to hold an IPv6 address (16 bytes) plus one extra byte at the high end that will always be 0.
            //                // This ensures that the IPv6 address will not be interpreted as a negative number if its top bit is set
            //                byte[] hostBytes = new byte[17];

            //                // Re-order the network address byte array to little endian as used in Windows
            //                Array.Copy(addressBytes.Reverse().ToArray<byte>(), hostBytes, addressBytes.Length);

            //                // Create a big integer from the little endian byte array
            //                BigInteger currentIpAddress = new BigInteger(hostBytes);

            //                // Iterate over the discovered interfaces to find the one that is closest to the original IP address
            //                for (int i = 0; i < availableInterfaces.Count; i++)
            //                {
            //                    AvailableInterface ai = new AvailableInterface();
            //                    ai.IpAddress = availableInterfaces[i].IpAddress;
            //                    ai.Port = availableInterfaces[i].Port;
            //                    ai.HostName = availableInterfaces[i].HostName;
            //                    ai.AddressDistance = BigInteger.Abs(BigInteger.Subtract(currentIpAddress, ai.IpAddress));
            //                    availableInterfaces[i] = ai;
            //                }

            //                // Initialise a big integer variable with an impossibly large address to ensure that the first iterated value will be used
            //                // The following number requires a leading zero to ensure that it is not interpreted as a negative number because its most significant bit is set
            //                // Hex number character count                    1234567890123456789012345678901234 = 34 hex characters = 17 bytes = a leading 0 byte plus 16 bytes of value 255
            //                BigInteger largestDifference = BigInteger.Parse("00FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            //                AlpacaDeviceBaseClass.LogMessage(TL,clientNumber, deviceType, $"Initialised largest value: {largestDifference} = {largestDifference:X34}");

            //                // Now iterate over the values and pick the entry with the smallest difference in IP address
            //                foreach (AvailableInterface availableInterface in availableInterfaces)
            //                {
            //                    if (availableInterface.AddressDistance < largestDifference)
            //                    {
            //                        largestDifference = availableInterface.AddressDistance;
            //                        clientHostAddress = $"{serviceType}://{availableInterface.HostName}:{availableInterface.Port}";

            //                        AlpacaDeviceBaseClass.LogMessage(TL,clientNumber, deviceType, $"New lowest address difference found: {availableInterface.AddressDistance} ({availableInterface.AddressDistance:X32}) for UniqueD {uniqueId}. Now using URL: {clientHostAddress}");

            //                        // Write the new value to the driver's Profile so it is found immediately in future
            //                        using (Profile profile = new Profile())
            //                        {
            //                            profile.DeviceType = deviceType;
            //                            profile.WriteValue(driverProgId, SharedConstants.IPADDRESS_PROFILENAME, availableInterface.HostName);
            //                            profile.WriteValue(driverProgId, SharedConstants.PORTNUMBER_PROFILENAME, availableInterface.Port.ToString());
            //                            AlpacaDeviceBaseClass.LogMessage(TL,clientNumber, deviceType, $"Written new values {availableInterface.HostName} and {availableInterface.Port} to profile {driverProgId}");
            //                        }
            //                    }
            //                }


            //                TL.BlankLine();
            //                break;
            //        }
            //    }
            //}
            #endregion

            // Remove any old client, if present
            if (client != null)
            {
                client.ClearHandlers();
            }

            // Create a new client pointing at the alpaca device
            client = new RestClient(clientHostAddress)
            {
                PreAuthenticate = true
            };

            // Add an HTTP basic authenticator configured with the user name and password to the client
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, deviceType, "Creating Authenticator");
            client.Authenticator = new HttpBasicAuthenticator(userName, password);

            // Set the client timeout
            SetClientTimeout(client, deviceResponseTimeout);
        }

        /// <summary>
        /// test whether there is a device at the specified IP address and port by opening a TCP connection to it
        /// </summary>
        /// <param name="ipAddressString">IP address of the device</param>
        /// <param name="portNumber">IP port number on the device</param>
        /// <param name="connectionTimeout">Time to wait before timing out</param>
        /// <param name="TL">Trace logger in which to report progress</param>
        /// <param name="clientNumber">The client's number</param>
        /// <returns></returns>
        //private static bool ClientIsUp(string ipAddressString, decimal portNumber, int connectionTimeout, ILogger TL, uint clientNumber)
        //{
        //    TcpClient tcpClient = null;

        //    bool returnValue = false; // Assume a bad outcome in case there is an exception 

        //    try
        //    {
        //        // Create a TcpClient 
        //        if (IPAddress.TryParse(ipAddressString, out IPAddress ipAddress))
        //        {
        //            // Create an IPv4 or IPv6 TCP client as required
        //            if (ipAddress.AddressFamily == AddressFamily.InterNetwork) tcpClient = new TcpClient(AddressFamily.InterNetwork); // Test IPv4 addresses
        //            else tcpClient = new TcpClient(AddressFamily.InterNetworkV6);
        //            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "ClientIsUp", $"Created an {ipAddress.AddressFamily} TCP client");
        //        }
        //        else
        //        {
        //            tcpClient = new TcpClient(); // Create a generic TcpClient that can work with host names
        //        }

        //        // Create a task that will return True if a connection to the device can be established or False if the connection is rejected or not possible
        //        Task<bool> connectionTask = tcpClient.ConnectAsync(ipAddressString, (int)portNumber).ContinueWith(task => { return !task.IsFaulted; }, TaskContinuationOptions.ExecuteSynchronously);
        //        AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "ClientIsUp", $"Created connection task");

        //        // Create a task that will time out after the specified time and return a value of False
        //        Task<bool> timeoutTask = Task.Delay(connectionTimeout * 1000).ContinueWith<bool>(task => false, TaskContinuationOptions.ExecuteSynchronously);
        //        AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "ClientIsUp", $"Created timeout task");

        //        // Create a task that will wait until either of the two preceding tasks completes
        //        Task<bool> resultTask = Task.WhenAny(connectionTask, timeoutTask).Unwrap();
        //        AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "ClientIsUp", $"Waiting for a task to complete");

        //        // Wait for one of the tasks to complete
        //        resultTask.Wait();
        //        AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "ClientIsUp", $"A task has completed");

        //        // Test whether or not we connected OK within the timeout period
        //        if (resultTask.Result) // We did connect OK within the timeout period
        //        {
        //            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "ClientIsUp", $"Contacted client OK!");
        //            tcpClient.Close();
        //            returnValue = true;
        //        }
        //        else // We did not connect successfully within the timeout period
        //        {
        //            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "ClientIsUp", $"Unable to contact client....");
        //            returnValue = false;
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "ClientIsUp", $"Exception: {ex}");

        //    }
        //    finally
        //    {
        //        tcpClient.Dispose();
        //    }
        //    return returnValue;
        //}

        #endregion

        #region Remote access methods

        public static void CallMethodWithNoParameters(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL, string method, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>();
            SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, method, Parameters, Method.PUT, memberType);
        }

        /// <summary>
        /// Overload used by methods other than ImageArray and ImageArrayVariant
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="clientNumber"></param>
        /// <param name="client"></param>
        /// <param name="URIBase"></param>
        /// <param name="TL"></param>
        /// <param name="method"></param>
        /// <param name="memberType"></param>
        /// <returns></returns>
        public static T GetValue<T>(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL, string method, MemberTypes memberType)
        {
            return GetValue<T>(clientNumber, client, URIBase, strictCasing, TL, method, SharedConstants.IMAGE_ARRAY_TRANSFER_TYPE_DEFAULT, SharedConstants.IMAGE_ARRAY_COMPRESSION_DEFAULT, memberType); // Set an arbitrary value for ImageArrayTransferType
        }

        /// <summary>
        /// Overload for use by the ImageArray and ImageArrayVariant methods
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="clientNumber"></param>
        /// <param name="client"></param>
        /// <param name="URIBase"></param>
        /// <param name="TL"></param>
        /// <param name="method"></param>
        /// <param name="imageArrayTransferType"></param>
        /// <param name="imageArrayCompression"></param>
        /// <param name="memberType"></param>
        /// <returns></returns>
        public static T GetValue<T>(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL, string method, ImageArrayTransferType imageArrayTransferType, ImageArrayCompression imageArrayCompression, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>();
            return SendToRemoteDevice<T>(clientNumber, client, URIBase, strictCasing, TL, method, Parameters, Method.GET, imageArrayTransferType, imageArrayCompression, memberType);
        }

        public static void SetBool(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL, string method, bool parmeterValue, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { method, parmeterValue.ToString(CultureInfo.InvariantCulture) }
            };
            SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, method, Parameters, Method.PUT, memberType);
        }

        public static void SetInt(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL, string method, int parmeterValue, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { method, parmeterValue.ToString(CultureInfo.InvariantCulture) }
            };
            SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, method, Parameters, Method.PUT, memberType);
        }

        public static void SetShort(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL, string method, short parmeterValue, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { method, parmeterValue.ToString(CultureInfo.InvariantCulture) }
            };
            SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, method, Parameters, Method.PUT, memberType);
        }

        public static void SetDouble(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL, string method, double parmeterValue, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { method, parmeterValue.ToString(CultureInfo.InvariantCulture) }
            };
            SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, method, Parameters, Method.PUT, memberType);
        }

        public static void SetDoubleWithShortParameter(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL, string method, short index, double parmeterValue, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { SharedConstants.ID_PARAMETER_NAME, index.ToString(CultureInfo.InvariantCulture) },
                { SharedConstants.VALUE_PARAMETER_NAME, parmeterValue.ToString(CultureInfo.InvariantCulture) }
            };
            SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, method, Parameters, Method.PUT, memberType);
        }

        public static void SetBoolWithShortParameter(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL, string method, short index, bool parmeterValue, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { SharedConstants.ID_PARAMETER_NAME, index.ToString(CultureInfo.InvariantCulture) },
                { SharedConstants.STATE_PARAMETER_NAME, parmeterValue.ToString(CultureInfo.InvariantCulture) }
            };
            SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, method, Parameters, Method.PUT, memberType);
        }

        public static void SetStringWithShortParameter(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL, string method, short index, string parmeterValue, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { SharedConstants.ID_PARAMETER_NAME, index.ToString(CultureInfo.InvariantCulture) },
                { SharedConstants.NAME_PARAMETER_NAME, parmeterValue.ToString(CultureInfo.InvariantCulture) }
            };
            SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, method, Parameters, Method.PUT, memberType);
        }

        public static string GetStringIndexedString(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL, string method, string parameterValue, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { SharedConstants.SENSORNAME_PARAMETER_NAME, parameterValue }
            };
            return SendToRemoteDevice<string>(clientNumber, client, URIBase, strictCasing, TL, method, Parameters, Method.GET, memberType);
        }

        public static double GetStringIndexedDouble(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL, string method, string parameterValue, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { SharedConstants.SENSORNAME_PARAMETER_NAME, parameterValue }
            };
            return SendToRemoteDevice<double>(clientNumber, client, URIBase, strictCasing, TL, method, Parameters, Method.GET, memberType);
        }

        public static double GetShortIndexedDouble(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL, string method, short parameterValue, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { SharedConstants.ID_PARAMETER_NAME, parameterValue.ToString(CultureInfo.InvariantCulture) }
            };
            return SendToRemoteDevice<double>(clientNumber, client, URIBase, strictCasing, TL, method, Parameters, Method.GET, memberType);
        }

        public static bool GetShortIndexedBool(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL, string method, short parameterValue, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { SharedConstants.ID_PARAMETER_NAME, parameterValue.ToString(CultureInfo.InvariantCulture) }
            };
            return SendToRemoteDevice<bool>(clientNumber, client, URIBase, strictCasing, TL, method, Parameters, Method.GET, memberType);
        }

        public static string GetShortIndexedString(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL, string method, short parameterValue, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { SharedConstants.ID_PARAMETER_NAME, parameterValue.ToString(CultureInfo.InvariantCulture) }
            };
            return SendToRemoteDevice<string>(clientNumber, client, URIBase, strictCasing, TL, method, Parameters, Method.GET, memberType);
        }

        /// <summary>
        /// Send a command to the remote device, retrying a given number of times if a socket exception is received
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="clientNumber"></param>
        /// <param name="client"></param>
        /// <param name="URIBase"></param>
        /// <param name="TL"></param>
        /// <param name="method"></param>
        /// <param name="Parameters"></param>
        /// <param name="HttpMethod"></param>
        /// <returns></returns>
        public static T SendToRemoteDevice<T>(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL, string method, Dictionary<string, string> Parameters, Method HttpMethod, MemberTypes memberType)
        {
            return SendToRemoteDevice<T>(clientNumber, client, URIBase, strictCasing, TL, method, Parameters, HttpMethod, SharedConstants.IMAGE_ARRAY_TRANSFER_TYPE_DEFAULT, SharedConstants.IMAGE_ARRAY_COMPRESSION_DEFAULT, memberType);
        }

        /// <summary>
        /// Send a command to the remote device, retrying a given number of times if a socket exception is received, specifying an image array transfer type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="clientNumber"></param>
        /// <param name="client"></param>
        /// <param name="uriBase"></param>
        /// <param name="TL"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <param name="httpMethod"></param>
        /// <param name="imageArrayTransferType"></param>
        /// <returns></returns>
        public static T SendToRemoteDevice<T>(uint clientNumber, RestClient client, string uriBase, bool strictCasing, ILogger TL, string method, Dictionary<string, string> parameters, Method httpMethod, ImageArrayTransferType imageArrayTransferType, ImageArrayCompression imageArrayCompression, MemberTypes memberType)
        {
            int retryCounter = 0; // Initialise the socket error retry counter
            Stopwatch sw = new Stopwatch(); // Stopwatch to time activities
            long lastTime = 0; // Holder for the accumulated elapsed time, used when reporting intermediate step timings
            Array remoteArray = null;

            sw.Start();

            do // Socket communications error retry loop
            {
                try
                {
                    const string LOG_FORMAT_STRING = "Client Txn ID: {0}, Server Txn ID: {1}, Value: {2}";

                    Response restResponseBase = null; // This has to be the base class of the data type classes in order for exception and error responses to be handled generically
                    RestRequest request = new RestRequest((uriBase + method).ToLowerInvariant(), httpMethod);
                    {
                        request.RequestFormat = DataFormat.Json;
                    };

                    // Set the default JSON compression behaviour to None
                    client.ConfigureWebRequest(wr => wr.AutomaticDecompression = DecompressionMethods.None); // Prevent any decompression

                    // Apply appropriate headers to control image array transfer
                    if (typeof(T) == typeof(Array))
                    {
                        switch (imageArrayCompression)
                        {
                            case ImageArrayCompression.None:
                                client.ConfigureWebRequest(wr => wr.AutomaticDecompression = DecompressionMethods.None); // Prevent any decompression
                                break;
                            case ImageArrayCompression.Deflate:
                                client.ConfigureWebRequest(wr => wr.AutomaticDecompression = DecompressionMethods.Deflate); // Allow only Deflate decompression
                                break;
                            case ImageArrayCompression.GZip:
                                client.ConfigureWebRequest(wr => wr.AutomaticDecompression = DecompressionMethods.GZip); // Allow only GZip decompression
                                break;
                            case ImageArrayCompression.GZipOrDeflate:
                                client.ConfigureWebRequest(wr => wr.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip); // Allow both Deflate and GZip decompression
                                break;
                            default:
                                throw new InvalidValueException($"Invalid image array compression type: {imageArrayCompression} - Correct this in the Dynamic Client setup dialogue.");
                        }

                        switch (imageArrayTransferType)
                        {
                            case ImageArrayTransferType.JSON:
                                // No extra action because "accepts = application/json" will be applied automatically by the client
                                break;

                            case ImageArrayTransferType.GetImageBytes:
                            case ImageArrayTransferType.Base64HandOff:
                            case ImageArrayTransferType.GetBase64Image:
                            case ImageArrayTransferType.BestAvailable:
                                request.AddHeader(SharedConstants.BASE64_HANDOFF_HEADER, SharedConstants.BASE64_HANDOFF_SUPPORTED);
                                break;

                            default:
                                throw new InvalidValueException($"Invalid image array transfer type: {imageArrayTransferType} - Correct this in the Dynamic Client setup dialogue.");
                        }
                    }

                    // Add the transaction number and client ID parameters
                    uint transaction = TransactionNumber();
                    request.AddParameter(SharedConstants.CLIENTTRANSACTION_PARAMETER_NAME, transaction.ToString());
                    request.AddParameter(SharedConstants.CLIENTID_PARAMETER_NAME, clientNumber.ToString());

                    // Add any supplied parameters to the request
                    foreach (KeyValuePair<string, string> parameter in parameters)
                    {
                        request.AddParameter(parameter.Key, parameter.Value);
                    }

                    lastTime = sw.ElapsedMilliseconds;
                    // Call the remote device and get the response
                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, "Client Txn ID: " + transaction.ToString() + ", Sending command to remote device");
                    IRestResponse deviceJsonResponse = client.Execute(request);
                    long timeDeviceResponse = sw.ElapsedMilliseconds - lastTime;

                    string responseContent;
                    if (deviceJsonResponse.Content.Length > 1000) responseContent = deviceJsonResponse.Content.Substring(0, 1000);
                    else responseContent = deviceJsonResponse.Content;
                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Response Status: '{deviceJsonResponse.StatusDescription}', Response: {responseContent}");

                    // Assess success at the communications level and handle accordingly 
                    if ((deviceJsonResponse.ResponseStatus == ResponseStatus.Completed) & (deviceJsonResponse.StatusCode == System.Net.HttpStatusCode.OK))
                    {
                        // GENERAL MULTI-DEVICE TYPES
                        if (typeof(T) == typeof(bool))
                        {
                            BoolResponse boolResponse = JsonSerializer.Deserialize<BoolResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });// JsonSerializer.Deserialize<BoolResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, boolResponse.ClientTransactionID, boolResponse.ServerTransactionID, boolResponse.Value.ToString()));
                            if (CallWasSuccessful(TL, boolResponse)) return (T)((object)boolResponse.Value);
                            restResponseBase = (Response)boolResponse;
                        }
                        if (typeof(T) == typeof(float))
                        {
                            // Handle float as double over the web, remembering to convert the returned value to float
                            DoubleResponse doubleResponse = JsonSerializer.Deserialize<DoubleResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, doubleResponse.ClientTransactionID, doubleResponse.ServerTransactionID, doubleResponse.Value.ToString()));
                            float floatValue = (float)doubleResponse.Value;
                            if (CallWasSuccessful(TL, doubleResponse)) return (T)((object)floatValue);
                            restResponseBase = (Response)doubleResponse;
                        }
                        if (typeof(T) == typeof(double))
                        {
                            DoubleResponse doubleResponse = JsonSerializer.Deserialize<DoubleResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, doubleResponse.ClientTransactionID, doubleResponse.ServerTransactionID, doubleResponse.Value.ToString()));
                            if (CallWasSuccessful(TL, doubleResponse)) return (T)((object)doubleResponse.Value);
                            restResponseBase = (Response)doubleResponse;
                        }
                        if (typeof(T) == typeof(string))
                        {
                            StringResponse stringResponse = JsonSerializer.Deserialize<StringResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, stringResponse.ClientTransactionID, stringResponse.ServerTransactionID, (stringResponse.Value is null) ? "NO VALUE OR NULL VALUE RETURNED" : (stringResponse.Value.Length <= 500 ? stringResponse.Value : stringResponse.Value.Substring(0, 500))));
                            if (CallWasSuccessful(TL, stringResponse)) return (T)((object)stringResponse.Value);
                            restResponseBase = (Response)stringResponse;
                        }
                        if (typeof(T) == typeof(string[]))
                        {
                            StringArrayResponse stringArrayResponse = JsonSerializer.Deserialize<StringArrayResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, stringArrayResponse.ClientTransactionID, stringArrayResponse.ServerTransactionID, (stringArrayResponse.Value is null) ? "NO VALUE OR NULL VALUE RETURNED" : stringArrayResponse.Value.Count().ToString()));
                            if (CallWasSuccessful(TL, stringArrayResponse)) return (T)((object)stringArrayResponse.Value);
                            restResponseBase = (Response)stringArrayResponse;
                        }
                        if (typeof(T) == typeof(short))
                        {
                            ShortResponse shortResponse = JsonSerializer.Deserialize<ShortResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, shortResponse.ClientTransactionID, shortResponse.ServerTransactionID, shortResponse.Value.ToString()));
                            if (CallWasSuccessful(TL, shortResponse)) return (T)((object)shortResponse.Value);
                            restResponseBase = (Response)shortResponse;
                        }
                        if (typeof(T) == typeof(int))
                        {
                            IntResponse intResponse = JsonSerializer.Deserialize<IntResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, intResponse.ClientTransactionID, intResponse.ServerTransactionID, intResponse.Value.ToString()));
                            if (CallWasSuccessful(TL, intResponse)) return (T)((object)intResponse.Value);
                            restResponseBase = (Response)intResponse;
                        }
                        if (typeof(T) == typeof(int[]))
                        {
                            IntArray1DResponse intArrayResponse = JsonSerializer.Deserialize<IntArray1DResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, intArrayResponse.ClientTransactionID, intArrayResponse.ServerTransactionID, (intArrayResponse.Value is null) ? "NO VALUE OR NULL VALUE RETURNED" : intArrayResponse.Value.Count().ToString()));
                            if (CallWasSuccessful(TL, intArrayResponse)) return (T)((object)intArrayResponse.Value);
                            restResponseBase = (Response)intArrayResponse;
                        }
                        if (typeof(T) == typeof(DateTime))
                        {
                            DateTimeResponse dateTimeResponse = JsonSerializer.Deserialize<DateTimeResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, dateTimeResponse.ClientTransactionID, dateTimeResponse.ServerTransactionID, dateTimeResponse.Value.ToString()));
                            if (CallWasSuccessful(TL, dateTimeResponse)) return (T)((object)dateTimeResponse.Value);
                            restResponseBase = (Response)dateTimeResponse;
                        }
                        if (typeof(T) == typeof(List<string>)) // Used for ArrayLists of string
                        {
                            StringListResponse stringListResponse = JsonSerializer.Deserialize<StringListResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, stringListResponse.ClientTransactionID, stringListResponse.ServerTransactionID, (stringListResponse.Value is null) ? "NO VALUE OR NULL VALUE RETURNED" : stringListResponse.Value.Count.ToString()));
                            if (CallWasSuccessful(TL, stringListResponse)) return (T)((object)stringListResponse.Value);
                            restResponseBase = (Response)stringListResponse;
                        }
                        if (typeof(T) == typeof(NoReturnValue)) // Used for Methods that have no response and Property Set members
                        {
                            MethodResponse deviceResponse = JsonSerializer.Deserialize<MethodResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, deviceResponse.ClientTransactionID.ToString(), deviceResponse.ServerTransactionID.ToString(), "No response"));
                            if (CallWasSuccessful(TL, deviceResponse)) return (T)((object)new NoReturnValue());
                            restResponseBase = (Response)deviceResponse;
                        }

                        // DEVICE SPECIFIC TYPES
                        if (typeof(T) == typeof(PointingState))
                        {
                            IntResponse PointingStateResponse = JsonSerializer.Deserialize<IntResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, PointingStateResponse.ClientTransactionID, PointingStateResponse.ServerTransactionID, PointingStateResponse.Value.ToString()));
                            if (CallWasSuccessful(TL, PointingStateResponse)) return (T)((object)PointingStateResponse.Value);
                            restResponseBase = (Response)PointingStateResponse;
                        }
                        if (typeof(T) == typeof(ITrackingRates))
                        {
                            TrackingRatesResponse trackingRatesResponse = JsonSerializer.Deserialize<TrackingRatesResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            if (trackingRatesResponse.Value != null) // A TrackingRates object was returned so process the response normally
                            {
                                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, trackingRatesResponse.ClientTransactionID, trackingRatesResponse.ServerTransactionID, trackingRatesResponse.Value.Count));
                                List<DriveRate> rates = new List<DriveRate>();
                                DriveRate[] ratesArray = new DriveRate[trackingRatesResponse.Value.Count];
                                int i = 0;
                                foreach (DriveRate rate in trackingRatesResponse.Value)
                                {
                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Rate: {rate}");
                                    ratesArray[i] = rate;
                                    i++;
                                }

                                TrackingRates trackingRates = new TrackingRates();
                                trackingRates.SetRates(ratesArray);
                                if (CallWasSuccessful(TL, trackingRatesResponse))
                                {
                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Returning {trackingRates.Count} tracking rates to the client - now measured from trackingRates");
                                    return (T)((object)trackingRates);
                                }
                            }
                            else // No TrackingRates object was returned so handle this as an error
                            {
                                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, trackingRatesResponse.ClientTransactionID, trackingRatesResponse.ServerTransactionID, "NO VALUE OR NULL VALUE RETURNED"));

                                // Now force an error return
                                trackingRatesResponse = new TrackingRatesResponse();
                                trackingRatesResponse.ErrorNumber = (AlpacaErrors)DYNAMIC_DRIVER_ERROR_NUMBER;
                                trackingRatesResponse.ErrorMessage = "Dynamic driver generated error: the Alpaca device returned no value or a null value for TrackingRates";
                            }
                            restResponseBase = (Response)trackingRatesResponse;
                        }
                        if (typeof(T) == typeof(EquatorialCoordinateType))
                        {
                            IntResponse equatorialCoordinateResponse = JsonSerializer.Deserialize<IntResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, equatorialCoordinateResponse.ClientTransactionID, equatorialCoordinateResponse.ServerTransactionID, equatorialCoordinateResponse.Value.ToString()));
                            if (CallWasSuccessful(TL, equatorialCoordinateResponse)) return (T)((object)equatorialCoordinateResponse.Value);
                            restResponseBase = (Response)equatorialCoordinateResponse;
                        }
                        if (typeof(T) == typeof(AlignmentMode))
                        {
                            IntResponse alignmentModesResponse = JsonSerializer.Deserialize<IntResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, alignmentModesResponse.ClientTransactionID, alignmentModesResponse.ServerTransactionID, alignmentModesResponse.Value.ToString()));
                            if (CallWasSuccessful(TL, alignmentModesResponse)) return (T)((object)alignmentModesResponse.Value);
                            restResponseBase = (Response)alignmentModesResponse;
                        }
                        if (typeof(T) == typeof(DriveRate))
                        {
                            IntResponse DriveRateResponse = JsonSerializer.Deserialize<IntResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, DriveRateResponse.ClientTransactionID, DriveRateResponse.ServerTransactionID, DriveRateResponse.Value.ToString()));
                            if (CallWasSuccessful(TL, DriveRateResponse)) return (T)((object)DriveRateResponse.Value);
                            restResponseBase = (Response)DriveRateResponse;
                        }
                        if (typeof(T) == typeof(SensorType))
                        {
                            IntResponse sensorTypeResponse = JsonSerializer.Deserialize<IntResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, sensorTypeResponse.ClientTransactionID, sensorTypeResponse.ServerTransactionID, sensorTypeResponse.Value.ToString()));
                            if (CallWasSuccessful(TL, sensorTypeResponse)) return (T)((object)sensorTypeResponse.Value);
                            restResponseBase = (Response)sensorTypeResponse;
                        }
                        if (typeof(T) == typeof(CameraState))
                        {
                            IntResponse cameraStatesResponse = JsonSerializer.Deserialize<IntResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, cameraStatesResponse.ClientTransactionID, cameraStatesResponse.ServerTransactionID, cameraStatesResponse.Value.ToString()));
                            if (CallWasSuccessful(TL, cameraStatesResponse)) return (T)((object)cameraStatesResponse.Value);
                            restResponseBase = (Response)cameraStatesResponse;
                        }
                        if (typeof(T) == typeof(ShutterState))
                        {
                            IntResponse domeShutterStateResponse = JsonSerializer.Deserialize<IntResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, domeShutterStateResponse.ClientTransactionID, domeShutterStateResponse.ServerTransactionID, domeShutterStateResponse.Value.ToString()));
                            if (CallWasSuccessful(TL, domeShutterStateResponse)) return (T)((object)domeShutterStateResponse.Value);
                            restResponseBase = (Response)domeShutterStateResponse;
                        }
                        if (typeof(T) == typeof(CoverStatus))
                        {
                            IntResponse coverStatusResponse = JsonSerializer.Deserialize<IntResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, coverStatusResponse.ClientTransactionID, coverStatusResponse.ServerTransactionID, coverStatusResponse.Value.ToString()));
                            if (CallWasSuccessful(TL, coverStatusResponse)) return (T)((object)coverStatusResponse.Value);
                            restResponseBase = (Response)coverStatusResponse;
                        }
                        if (typeof(T) == typeof(CalibratorStatus))
                        {
                            IntResponse calibratorStatusResponse = JsonSerializer.Deserialize<IntResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, calibratorStatusResponse.ClientTransactionID, calibratorStatusResponse.ServerTransactionID, calibratorStatusResponse.Value.ToString()));
                            if (CallWasSuccessful(TL, calibratorStatusResponse)) return (T)((object)calibratorStatusResponse.Value);
                            restResponseBase = (Response)calibratorStatusResponse;
                        }
                        if (typeof(T) == typeof(IAxisRates))
                        {
                            AxisRatesResponse axisRatesResponse = JsonSerializer.Deserialize<AxisRatesResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            if (axisRatesResponse.Value != null) // A AxisRates object was returned so process the response normally
                            {
                                AxisRates axisRates = new AxisRates((TelescopeAxis)(Convert.ToInt32(parameters[SharedConstants.AXIS_PARAMETER_NAME])), TL);
                                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, axisRatesResponse.ClientTransactionID.ToString(), axisRatesResponse.ServerTransactionID.ToString(), axisRatesResponse.Value.Count.ToString()));
                                foreach (AxisRate rr in axisRatesResponse.Value)
                                {
                                    axisRates.Add(rr.Minimum, rr.Maximum, TL);
                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Found rate: {rr.Minimum} - {rr.Maximum}");
                                }

                                if (CallWasSuccessful(TL, axisRatesResponse)) return (T)((object)axisRates);
                            }
                            else // No AxisRates object was returned so handle this as an error
                            {
                                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, axisRatesResponse.ClientTransactionID, axisRatesResponse.ServerTransactionID, "NO VALUE OR NULL VALUE RETURNED"));

                                // Now force an error return
                                axisRatesResponse = new AxisRatesResponse();
                                axisRatesResponse.ErrorNumber = (AlpacaErrors)DYNAMIC_DRIVER_ERROR_NUMBER;
                                axisRatesResponse.ErrorMessage = "Dynamic driver generated error: the Alpaca device returned no value or a null value for AxisRates";
                            }

                            restResponseBase = (Response)axisRatesResponse;
                        }
                        if (typeof(T) == typeof(Array)) // Used for Camera.ImageArray and Camera.ImageArrayVariant
                        {
                            // Include some debug logging
                            foreach (var header in deviceJsonResponse.Headers)
                            {
                                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Response header {header.Name} = {header.Value}");
                            }

                            // Handle base64 hand-off image transfer mechanic
                            if (deviceJsonResponse.Headers.Any(t => t.Name.ToString() == SharedConstants.BASE64_HANDOFF_HEADER)) // Base64 format header is present so the server supports base64 serialised transfer
                            {
                                // De-serialise the JSON image array hand-off response 
                                sw.Restart(); // Clear and start the stopwatch
                                Base64ArrayHandOffResponse base64HandOffresponse = JsonSerializer.Deserialize<Base64ArrayHandOffResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                                if (CallWasSuccessful(TL, base64HandOffresponse))
                                {

                                    ImageArrayElementTypes arrayType = (ImageArrayElementTypes)base64HandOffresponse.Type; // Extract the array type from the JSON response

                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Base64 - Extracted array information in {sw.ElapsedMilliseconds}ms. Array Type: {arrayType}, Rank: {base64HandOffresponse.Rank}, Dimension 0 length: {base64HandOffresponse.Dimension0Length}, Dimension 1 length: {base64HandOffresponse.Dimension1Length}, Dimension 2 length: {base64HandOffresponse.Dimension2Length}");
                                    sw.Restart();

                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Base64 - Downloading base64 serialised image");

                                    // Construct an HTTP request to get the base 64 encoded image
                                    string base64Uri = (client.BaseUrl + uriBase.TrimStart('/') + method.ToLowerInvariant() + SharedConstants.BASE64_HANDOFF_FILE_DOWNLOAD_URI_EXTENSION).ToLowerInvariant(); // Create the download URI from the REST client elements
                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Base64 URI: {base64Uri}");

                                    // Create a variable to hold the returned base 64 string
                                    string base64ArrayString = "";

                                    // Create a handler to indicate the compression levels supported by this client
                                    using (HttpClientHandler imageDownloadHandler = new HttpClientHandler())
                                    {
                                        switch (imageArrayCompression)
                                        {
                                            case ImageArrayCompression.None:
                                                imageDownloadHandler.AutomaticDecompression = DecompressionMethods.None;
                                                break;
                                            case ImageArrayCompression.Deflate:
                                                imageDownloadHandler.AutomaticDecompression = DecompressionMethods.Deflate;
                                                break;
                                            case ImageArrayCompression.GZip:
                                                imageDownloadHandler.AutomaticDecompression = DecompressionMethods.GZip;
                                                break;
                                            case ImageArrayCompression.GZipOrDeflate:
                                                imageDownloadHandler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip; // Allow both Deflate and GZip decompression
                                                break;
                                            default:
                                                throw new InvalidValueException($"Unknown ImageArrayCompression value: {imageArrayCompression} - Can't proceed further!");
                                        }

                                        // Create an HTTP client  to download the base64 string
                                        using (HttpClient httpClient = new HttpClient(imageDownloadHandler))
                                        {
                                            // Get the async stream from the HTTPClient
                                            Stream base64ArrayStream = httpClient.GetStreamAsync(base64Uri).Result;
                                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Downloaded base64 stream obtained in {sw.ElapsedMilliseconds}ms"); sw.Restart();

                                            // Read the stream contents into the string variable ready for further processing
                                            using (StreamReader sr = new StreamReader(base64ArrayStream, System.Text.Encoding.ASCII, false))
                                            {
                                                base64ArrayString = sr.ReadToEnd();
                                            }
                                        }
                                    }

                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Read base64 string from stream ({base64ArrayString.Length} bytes) in {sw.ElapsedMilliseconds}ms"); sw.Restart();
                                    try { AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Base64 string start: {base64ArrayString.Substring(0, 300)}"); } catch { }
                                    try { AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Base64 string end: {base64ArrayString.Substring(60000000, 300)}"); } catch { }

                                    // Convert the array from base64 encoding to a byte array
                                    byte[] base64ArrayByteArray = Convert.FromBase64String(base64ArrayString);
                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Converted base64 string of length {base64ArrayString.Length} to byte array of length {base64ArrayByteArray.Length} in {sw.ElapsedMilliseconds}ms"); sw.Restart();
                                    string byteLine = "";
                                    try
                                    {
                                        for (int i = 0; i < 300; i++)
                                        {
                                            byteLine += base64ArrayByteArray[i].ToString() + " ";
                                        }
                                        AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Converted base64 bytes: {byteLine}");
                                    }
                                    catch { }

                                    // Now create and populate an appropriate array to return to the client that mirrors the array type returned by the device
                                    switch (arrayType) // Handle the different array return types
                                    {
                                        case ImageArrayElementTypes.Int32:
                                            switch (base64HandOffresponse.Rank)
                                            {
                                                case 2:
                                                    remoteArray = new int[base64HandOffresponse.Dimension0Length, base64HandOffresponse.Dimension1Length];
                                                    break;

                                                case 3:
                                                    remoteArray = new int[base64HandOffresponse.Dimension0Length, base64HandOffresponse.Dimension1Length, base64HandOffresponse.Dimension2Length];
                                                    break;

                                                default:
                                                    throw new InvalidOperationException("Arrays of Rank " + base64HandOffresponse.Rank + " are not supported.");
                                            }

                                            // Copy the array bytes to the response array that will return to the client
                                            Buffer.BlockCopy(base64ArrayByteArray, 0, remoteArray, 0, base64ArrayByteArray.Length);
                                            break;

                                        case ImageArrayElementTypes.Int16:
                                            switch (base64HandOffresponse.Rank)
                                            {
                                                case 2:
                                                    remoteArray = new short[base64HandOffresponse.Dimension0Length, base64HandOffresponse.Dimension1Length];
                                                    break;

                                                case 3:
                                                    remoteArray = new short[base64HandOffresponse.Dimension0Length, base64HandOffresponse.Dimension1Length, base64HandOffresponse.Dimension2Length];
                                                    break;

                                                default:
                                                    throw new InvalidOperationException("Arrays of Rank " + base64HandOffresponse.Rank + " are not supported.");
                                            }
                                            Buffer.BlockCopy(base64ArrayByteArray, 0, remoteArray, 0, base64ArrayByteArray.Length); // Copy the array bytes to the response array that will return to the client
                                            break;

                                        case ImageArrayElementTypes.Double:
                                            switch (base64HandOffresponse.Rank)
                                            {
                                                case 2:
                                                    remoteArray = new double[base64HandOffresponse.Dimension0Length, base64HandOffresponse.Dimension1Length];
                                                    break;

                                                case 3:
                                                    remoteArray = new double[base64HandOffresponse.Dimension0Length, base64HandOffresponse.Dimension1Length, base64HandOffresponse.Dimension2Length];
                                                    break;

                                                default:
                                                    throw new InvalidOperationException("Arrays of Rank " + base64HandOffresponse.Rank + " are not supported.");
                                            }
                                            Buffer.BlockCopy(base64ArrayByteArray, 0, remoteArray, 0, base64ArrayByteArray.Length); // Copy the array bytes to the response array that will return to the client
                                            break;

                                        default:
                                            throw new InvalidOperationException($"SendToRemoteDevice Base64HandOff - Image array element type {arrayType} is not supported. The device returned this value: {base64HandOffresponse.Type}");
                                    }

                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Created and copied the array in {sw.ElapsedMilliseconds}ms"); sw.Restart();

                                    return (T)(object)remoteArray;
                                }
                                restResponseBase = (Response)base64HandOffresponse;
                            }

                            // Handle conventional JSON response with integer array elements individually serialised
                            else
                            {
                                sw.Restart(); // Clear and start the stopwatch
                                ImageArrayResponseBase responseBase = JsonSerializer.Deserialize<ImageArrayResponseBase>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                                if (CallWasSuccessful(TL, responseBase))
                                {

                                    ImageArrayElementTypes arrayType = (ImageArrayElementTypes)responseBase.Type;
                                    int arrayRank = responseBase.Rank;

                                    // Include some debug logging
                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Extracted array type and rank by JsonSerializer.Deserialize in {sw.ElapsedMilliseconds}ms. Type: {arrayType}, Rank: {arrayRank}, Response values - Type: {responseBase.Type}, Rank: {responseBase.Rank}");

                                    sw.Restart(); // Clear and start the stopwatch
                                    switch (arrayType) // Handle the different return types that may come from ImageArrayVariant
                                    {
                                        case ImageArrayElementTypes.Int32:
                                            switch (arrayRank)
                                            {
                                                case 2:
                                                    //IntArray2DResponse intArray2DResponse = JsonSerializer.Deserialize<IntArray2DResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                                                    IntArray2DResponse intArray2DResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<IntArray2DResponse>(deviceJsonResponse.Content);
                                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, intArray2DResponse.ClientTransactionID, intArray2DResponse.ServerTransactionID, intArray2DResponse.Rank.ToString())); //, intArray2DResponse.Method));
                                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Array was deserialised in {sw.ElapsedMilliseconds} ms, Type: {(ImageArrayElementTypes)intArray2DResponse.Type}, Rank: {intArray2DResponse.Rank}");
                                                    if (CallWasSuccessful(TL, intArray2DResponse)) return (T)((object)intArray2DResponse.Value);
                                                    restResponseBase = (Response)intArray2DResponse;
                                                    break;

                                                case 3:
                                                    //IntArray3DResponse intArray3DResponse = JsonSerializer.Deserialize<IntArray3DResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                                                    IntArray3DResponse intArray3DResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<IntArray3DResponse>(deviceJsonResponse.Content);
                                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, intArray3DResponse.ClientTransactionID, intArray3DResponse.ServerTransactionID, intArray3DResponse.Rank.ToString())); //, intArray3DResponse.Method));
                                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Array was deserialised in {sw.ElapsedMilliseconds} ms, Type: {(ImageArrayElementTypes)intArray3DResponse.Type}, Rank: {intArray3DResponse.Rank}");
                                                    if (CallWasSuccessful(TL, intArray3DResponse)) return (T)((object)intArray3DResponse.Value);
                                                    restResponseBase = (Response)intArray3DResponse;
                                                    break;

                                                default:
                                                    throw new InvalidOperationException("Arrays of Rank " + arrayRank + " are not supported.");
                                            }
                                            break;

                                        case ImageArrayElementTypes.Int16:
                                            switch (arrayRank)
                                            {
                                                case 2:
                                                    //ShortArray2DResponse shortArray2DResponse = JsonSerializer.Deserialize<ShortArray2DResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                                                    ShortArray2DResponse shortArray2DResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ShortArray2DResponse>(deviceJsonResponse.Content);
                                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, shortArray2DResponse.ClientTransactionID, shortArray2DResponse.ServerTransactionID, shortArray2DResponse.Rank.ToString())); //, shortArray2DResponse.Method));
                                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Array was deserialised in {sw.ElapsedMilliseconds} ms, Type: {(ImageArrayElementTypes)shortArray2DResponse.Type}, Rank: {shortArray2DResponse.Rank}");
                                                    if (CallWasSuccessful(TL, shortArray2DResponse)) return (T)((object)shortArray2DResponse.Value);
                                                    restResponseBase = (Response)shortArray2DResponse;
                                                    break;

                                                case 3:
                                                    //ShortArray3DResponse shortArray3DResponse = JsonSerializer.Deserialize<ShortArray3DResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                                                    ShortArray3DResponse shortArray3DResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ShortArray3DResponse>(deviceJsonResponse.Content);
                                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, shortArray3DResponse.ClientTransactionID, shortArray3DResponse.ServerTransactionID, shortArray3DResponse.Rank.ToString())); //, shortArray3DResponse.Method));
                                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Array was deserialised in {sw.ElapsedMilliseconds} ms, Type: {(ImageArrayElementTypes)shortArray3DResponse.Type}, Rank: {shortArray3DResponse.Rank}");
                                                    if (CallWasSuccessful(TL, shortArray3DResponse)) return (T)((object)shortArray3DResponse.Value);
                                                    restResponseBase = (Response)shortArray3DResponse;
                                                    break;

                                                default:
                                                    throw new InvalidOperationException("Arrays of Rank " + arrayRank + " are not supported.");
                                            }
                                            break;

                                        case ImageArrayElementTypes.Double:
                                            switch (arrayRank)
                                            {
                                                case 2:
                                                    // DoubleArray2DResponse doubleArray2DResponse = JsonSerializer.Deserialize<DoubleArray2DResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                                                    DoubleArray2DResponse doubleArray2DResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<DoubleArray2DResponse>(deviceJsonResponse.Content);
                                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, doubleArray2DResponse.ClientTransactionID, doubleArray2DResponse.ServerTransactionID, doubleArray2DResponse.Rank.ToString())); //, doubleArray2DResponse.Method));
                                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Array was deserialised in {sw.ElapsedMilliseconds} ms, Type: {(ImageArrayElementTypes)doubleArray2DResponse.Type}, Rank: {doubleArray2DResponse.Rank}");
                                                    if (CallWasSuccessful(TL, doubleArray2DResponse)) return (T)((object)doubleArray2DResponse.Value);
                                                    restResponseBase = (Response)doubleArray2DResponse;
                                                    break;

                                                case 3:
                                                    //DoubleArray3DResponse doubleArray3DResponse = JsonSerializer.Deserialize<DoubleArray3DResponse>(deviceJsonResponse.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                                                    DoubleArray3DResponse doubleArray3DResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<DoubleArray3DResponse>(deviceJsonResponse.Content);
                                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, string.Format(LOG_FORMAT_STRING, doubleArray3DResponse.ClientTransactionID, doubleArray3DResponse.ServerTransactionID, doubleArray3DResponse.Rank.ToString())); //, doubleArray3DResponse.Method));
                                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Array was deserialised in {sw.ElapsedMilliseconds} ms, Type: {(ImageArrayElementTypes)doubleArray3DResponse.Type}, Rank: {doubleArray3DResponse.Rank}");
                                                    if (CallWasSuccessful(TL, doubleArray3DResponse)) return (T)((object)doubleArray3DResponse.Value);
                                                    restResponseBase = (Response)doubleArray3DResponse;
                                                    break;

                                                default:
                                                    throw new InvalidOperationException("Arrays of Rank " + arrayRank + " are not supported.");
                                            }
                                            break;

                                        default:
                                            throw new InvalidOperationException($"SendToRemoteDevice JSON - Image array element type {arrayType} is not supported. The device returned this value: {responseBase.Type}");
                                    }
                                }
                                restResponseBase = (Response)responseBase;
                            } // remote device has used JSON encoding
                        }

                        // HANDLE COM EXCEPTIONS THROWN BY WINDOWS BASED DRIVERS RUNNING IN THE REMOTE DEVICE
                        if (restResponseBase.DriverException != null)
                        {
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Exception Message: \"{restResponseBase.ErrorMessage}\", Exception Number: 0x{restResponseBase.DriverException.HResult:X8}");
                            throw restResponseBase.DriverException;
                        }

                        // HANDLE ERRORS REPORTED BY ALPACA DEVICES THAT USE THE ERROR NUMBER AND ERROR MESSAGE FIELDS
                        if ((restResponseBase.ErrorMessage != "") || (restResponseBase.ErrorNumber != 0))
                        {
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Received an Alpaca error - ErrorMessage: \"{restResponseBase.ErrorMessage}\", ErrorNumber: 0x{(int)restResponseBase.ErrorNumber:X8}");

                            // Handle ASCOM Alpaca reserved error numbers between 0x400 and 0xFFF by translating these to the COM HResult error number range: 0x80040400 to 0x80040FFF and throwing the translated value as an exception
                            if ((restResponseBase.ErrorNumber >= AlpacaErrors.AlpacaErrorCodeBase) & (restResponseBase.ErrorNumber <= AlpacaErrors.AlpacaErrorCodeMax)) // This error is within the ASCOM Alpaca reserved error number range
                            {
                                // Calculate the equivalent COM HResult error number from the supplied Alpaca error number so that comparison can be made with the original ASCOM COM exception HResult numbers that Windows clients expect in their exceptions
                                int ascomCOMErrorNumber = (int)(restResponseBase.ErrorNumber + (int)ComErrorCodes.ComErrorNumberOffset);
                                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Received Alpaca error code: {restResponseBase.ErrorNumber} (0x{(int)restResponseBase.ErrorNumber:X4}), the equivalent COM error HResult error code is {ascomCOMErrorNumber} (0x{ascomCOMErrorNumber:X8})");

                                // Now check whether the COM HResult matches any of the built-in ASCOM exception types. If so, we throw that exception type otherwise we throw a generic DriverException
                                if (ascomCOMErrorNumber == ASCOM.ErrorCodes.ActionNotImplementedException) // Handle ActionNotImplementedException
                                {
                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Alpaca action not implemented error, throwing ActionNotImplementedException - ErrorMessage: \"{restResponseBase.ErrorMessage}\", ErrorNumber: 0x{ascomCOMErrorNumber:X8}");
                                    throw new ActionNotImplementedException(restResponseBase.ErrorMessage);
                                }
                                else if (ascomCOMErrorNumber == ASCOM.ErrorCodes.InvalidOperationException) // Handle InvalidOperationException
                                {
                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Alpaca invalid operation error, throwing InvalidOperationException - ErrorMessage: \"{restResponseBase.ErrorMessage}\", ErrorNumber: 0x{ascomCOMErrorNumber:X8}");
                                    throw new InvalidOperationException(restResponseBase.ErrorMessage);
                                }
                                else if (ascomCOMErrorNumber == ASCOM.ErrorCodes.InvalidValue) // Handle InvalidValueException
                                {
                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Alpaca invalid value error, throwing InvalidValueException - ErrorMessage: \"{restResponseBase.ErrorMessage}\", ErrorNumber: 0x{ascomCOMErrorNumber:X8}");
                                    throw new InvalidValueException(restResponseBase.ErrorMessage);
                                }
                                else if (ascomCOMErrorNumber == ASCOM.ErrorCodes.InvalidWhileParked) // Handle ParkedException
                                {
                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Alpaca invalid while parked error, throwing ParkedException - ErrorMessage: \"{restResponseBase.ErrorMessage}\", ErrorNumber: 0x{ascomCOMErrorNumber:X8}");
                                    throw new ParkedException(restResponseBase.ErrorMessage);
                                }
                                else if (ascomCOMErrorNumber == ASCOM.ErrorCodes.InvalidWhileSlaved) // Handle SlavedException
                                {
                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $" Alpaca invalid while slaved error, throwing SlavedException - ErrorMessage: \"{restResponseBase.ErrorMessage}\", ErrorNumber: 0x{ascomCOMErrorNumber:X8}");
                                    throw new SlavedException(restResponseBase.ErrorMessage);
                                }
                                else if (ascomCOMErrorNumber == ASCOM.ErrorCodes.NotConnected) // Handle NotConnectedException
                                {
                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $" Alpaca not connected error, throwing NotConnectedException - ErrorMessage: \"{restResponseBase.ErrorMessage}\", ErrorNumber: 0x{ascomCOMErrorNumber:X8}");
                                    throw new NotConnectedException(restResponseBase.ErrorMessage);
                                }
                                else if (ascomCOMErrorNumber == ASCOM.ErrorCodes.NotImplemented) // Handle PropertyNotImplementedException and MethodNotImplementedException (both have the same error code)
                                {
                                    // Throw the relevant exception depending on whether this is a property or a method
                                    if (memberType == MemberTypes.Property) // Calling member is a property so throw a PropertyNotImplementedException
                                    {
                                        AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Alpaca property not implemented error, throwing PropertyNotImplementedException - ErrorMessage: \"{restResponseBase.ErrorMessage}\", ErrorNumber: 0x{ascomCOMErrorNumber:X8}");
                                        throw new PropertyNotImplementedException(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(method), httpMethod == Method.PUT, restResponseBase.ErrorMessage);
                                    }
                                    else // Calling member is a method so throw a MethodNotImplementedException
                                    {
                                        AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $" Alpaca method not implemented error, throwing MethodNotImplementedException - ErrorMessage: \"{restResponseBase.ErrorMessage}\", ErrorNumber: 0x{ascomCOMErrorNumber:X8}");
                                        throw new MethodNotImplementedException(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(method), restResponseBase.ErrorMessage);
                                    }
                                }
                                else if (ascomCOMErrorNumber == ASCOM.ErrorCodes.ValueNotSet) // Handle ValueNotSetException
                                {
                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $" Alpaca value not set error, throwing ValueNotSetException - ErrorMessage: \"{restResponseBase.ErrorMessage}\", ErrorNumber: 0x{ascomCOMErrorNumber:X8}");
                                    throw new ValueNotSetException(restResponseBase.ErrorMessage);
                                }
                                else // The exception is inside the ASCOM Alpaca reserved range but is not one of those with their own specific exception types above, so wrap it in a DriverException and throw this to the client
                                {
                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Alpaca undefined ASCOM error, throwing DriverException - ErrorMessage: \"{restResponseBase.ErrorMessage}\", ErrorNumber: 0x{ascomCOMErrorNumber:X8}");
                                    throw new DriverException(restResponseBase.ErrorMessage, ascomCOMErrorNumber);
                                }
                            }
                            else // An exception has been thrown with an error number outside the ASCOM Alpaca reserved range, so wrap it in a DriverException and throw this to the client.
                            {
                                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Alpaca error outside ASCOM reserved range, throwing DriverException - ErrorMessage: \"{restResponseBase.ErrorMessage}\", ErrorNumber: 0x{restResponseBase.ErrorNumber:X8}");
                                throw new DriverException(restResponseBase.ErrorMessage, (int)restResponseBase.ErrorNumber);
                            }
                        }

                        // Internal error if an unsupported type is requested - should only occur during development and not in production operation!
                        throw new InvalidOperationException("Type " + typeof(T).ToString() + " is not supported. You should never see this message, if you do, please report it on the ASCOM Talk Forum!");
                    }
                    else
                    {
                        if (deviceJsonResponse.ErrorException != null)
                        {
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, "RestClient exception: " + deviceJsonResponse.ErrorMessage + "\r\n " + deviceJsonResponse.ErrorException.ToString());
                            throw new DriverException($"Communications exception: {deviceJsonResponse.ErrorMessage} - {deviceJsonResponse.ResponseStatus}", deviceJsonResponse.ErrorException);
                        }
                        else
                        {
                            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method + " Error", $"RestRequest response status: {deviceJsonResponse.ResponseStatus}, HTTP response code: {deviceJsonResponse.StatusCode}, HTTP response description: {deviceJsonResponse.StatusDescription}");
                            throw new DriverException($"Error calling method: {method}, HTTP Completion Status: {deviceJsonResponse.ResponseStatus}, Error Message:\r\n{deviceJsonResponse.Content}");
                        }
                    }
                }
                catch (Exception ex) // Process unexpected exceptions
                {
                    if (ex is System.Net.WebException) // Received a WebException, this could indicate that the remote device actively refused the connection so test for this and retry if appropriate
                    {
                        if (ex.InnerException != null) // First make sure the is an inner exception
                        {
                            if (ex.InnerException is System.Net.Sockets.SocketException) // There is an inner exception and it is a SocketException so apply the retry logic
                            {
                                retryCounter += 1; // Increment the retry counter
                                if (retryCounter <= SharedConstants.SOCKET_ERROR_MAXIMUM_RETRIES) // The retry count is less than or equal to the maximum allowed so retry the command
                                {
                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, typeof(T).Name + " " + ex.Message);
                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, "SocketException: " + ex.ToString());

                                    // Log that we are retrying the command and wait a short time in the hope that the transient condition clears
                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, $"Socket exception, retrying command - retry-count {retryCounter}/{SharedConstants.SOCKET_ERROR_MAXIMUM_RETRIES}");
                                    Thread.Sleep(SharedConstants.SOCKET_ERROR_RETRY_DELAY_TIME);
                                }
                                else // The retry count exceeds the maximum allowed so throw the exception to the client
                                {
                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, typeof(T).Name + " " + ex.Message);
                                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, "SocketException: " + ex.ToString());
                                    throw;
                                }

                            }
                            else  // There is an inner exception but it is not a SocketException so log it and throw it  to the client
                            {
                                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, typeof(T).Name + " " + ex.Message);
                                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, "WebException: " + ex.ToString());
                                throw;
                            }

                        }
                    }
                    else // Some other type of exception that isn't System.Net.WebException so log it and throw it to the client
                    {
                        AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, typeof(T).Name + " " + ex.Message);
                        AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, method, "Exception: " + ex.ToString());
                        throw;
                    }
                }
            } while (true); // Execution will only reach here if a communications retry is required, all other conditions are handled by return statements or by throwing exceptions

            // Execution will never reach this point
        }

        /// <summary>
        /// Test whether an error occurred in the driver
        /// </summary>
        /// <param name="response">The driver's response </param>
        /// <returns>True if the call was successful otherwise returns false.</returns>
        private static bool CallWasSuccessful(ILogger TL, Response response)
        {
            if (response is null)
            {
                AlpacaDeviceBaseClass.LogMessage(TL, 0, "CallWasNotSuccessful", "No response from device - Returning False");
                AlpacaDeviceBaseClass.LogBlankLine(TL);
                return false; // No response so return false

            }

            AlpacaDeviceBaseClass.LogMessage(TL, 0, "CallWasSuccessful", $"DriverException == null: {response.DriverException == null}, ErrorMessage: '{response.ErrorMessage}', ErrorNumber: 0x{(int)response.ErrorNumber:X8}");
            if (response.DriverException != null) AlpacaDeviceBaseClass.LogMessage(TL, 0, "CallWasSuccessfulEx", response.DriverException.ToString());
            if ((response.DriverException == null) & (response.ErrorMessage == "") & (response.ErrorNumber == AlpacaErrors.AlpacaNoError))
            {
                AlpacaDeviceBaseClass.LogMessage(TL, 0, "CallWasSuccessful", "Returning True");
                AlpacaDeviceBaseClass.LogBlankLine(TL);
                return true; // All was OK so return true
            }
            else
            {
                AlpacaDeviceBaseClass.LogMessage(TL, 0, "CallWasNotSuccessful", "Returning False");
                AlpacaDeviceBaseClass.LogBlankLine(TL);
                return false; // Some sort of issue so return false
            }
        }

        #endregion

        #region ASCOM Common members

        public static string Action(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL, string actionName, string actionParameters)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { SharedConstants.ACTION_COMMAND_PARAMETER_NAME, actionName },
                { SharedConstants.ACTION_PARAMETERS_PARAMETER_NAME, actionParameters }
            };
            string remoteString = SendToRemoteDevice<string>(clientNumber, client, URIBase, strictCasing, TL, "Action", Parameters, Method.PUT, MemberTypes.Method);
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "Action", $"Response length: {remoteString.Length}");
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "Action", $"Response: {((remoteString.Length <= 100) ? remoteString : remoteString.Substring(0, 100))}");
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "Action", $"Returning response length: {remoteString.Length}");
            return remoteString;
        }

        public static void CommandBlind(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL, string command, bool raw)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { SharedConstants.COMMAND_PARAMETER_NAME, command },
                { SharedConstants.RAW_PARAMETER_NAME, raw.ToString() }
            };
            SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, "CommandBlind", Parameters, Method.PUT, MemberTypes.Method);
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "CommandBlind", "Completed OK");
        }

        public static bool CommandBool(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL, string command, bool raw)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { SharedConstants.COMMAND_PARAMETER_NAME, command },
                { SharedConstants.RAW_PARAMETER_NAME, raw.ToString() }
            };
            bool remoteBool = SendToRemoteDevice<bool>(clientNumber, client, URIBase, strictCasing, TL, "CommandBool", Parameters, Method.PUT, MemberTypes.Method);

            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "CommandBool", remoteBool.ToString());
            return remoteBool;
        }

        public static string CommandString(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL, string command, bool raw)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { SharedConstants.COMMAND_PARAMETER_NAME, command },
                { SharedConstants.RAW_PARAMETER_NAME, raw.ToString() }
            };
            string remoteString = SendToRemoteDevice<string>(clientNumber, client, URIBase, strictCasing, TL, "CommandString", Parameters, Method.PUT, MemberTypes.Method);

            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "CommandString", remoteString);
            return remoteString;
        }

        public static void Connect(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL)
        {
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "Connect", "Acquiring connection lock");
            lock (connectLockObject) // Ensure that only one connection attempt can happen at a time
            {
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "Connect", "Has connection lock");
                if (IsClientConnected(clientNumber, TL)) // If we are already connected then just log this 
                {
                    AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "Connect", "Already connected, just incrementing connection count.");
                }
                else // We are not connected so connect now
                {
                    try
                    {
                        AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "Connect", "This is the first connection so set Connected to True");
                        SetBool(clientNumber, client, URIBase, strictCasing, TL, "Connected", true, MemberTypes.Property);
                        bool notAlreadyPresent = connectStates.TryAdd(clientNumber, true);
                        AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "Connect", "Successfully connected, AlreadyConnected: " + (!notAlreadyPresent).ToString() + ", number of connections: " + connectStates.Count);
                    }
                    catch (Exception ex)
                    {
                        AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "Connect", "Exception: " + ex.ToString());
                        throw;
                    }
                }
            }
        }

        public static string Description(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL)
        {
            return GetValue<string>(clientNumber, client, URIBase, strictCasing, TL, "Description", MemberTypes.Property);
        }

        public static void Disconnect(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL)
        {

            if (IsClientConnected(clientNumber, TL)) // If we are already connected then disconnect, otherwise ignore disconnect 
            {
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "Disconnect", "We are connected, setting Connected to False on remote driver");
                SetBool(clientNumber, client, URIBase, strictCasing, TL, "Connected", false, MemberTypes.Property);
                bool successfullyRemoved = connectStates.TryRemove(clientNumber, out bool lastValue);
                AlpacaDeviceBaseClass.LogMessage(TL, 0, "Disconnect", $"Set Connected to: False, Successfully removed: {successfullyRemoved}, previous value: {lastValue}");
            }
            else
            {
                AlpacaDeviceBaseClass.LogMessage(TL, 0, "Disconnect", "Already disconnected, not sending command to driver");
            }
        }

        public static string DriverInfo(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL)
        {
            return GetValue<string>(clientNumber, client, URIBase, strictCasing, TL, "DriverInfo", MemberTypes.Property);
        }

        public static string DriverVersion(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL)
        {
            string remoteString = GetValue<string>(clientNumber, client, URIBase, strictCasing, TL, "DriverVersion", MemberTypes.Property);
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "DriverVersion", remoteString);
            return remoteString;
        }

        public static short InterfaceVersion(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL)
        {
            short interfaceVersion = GetValue<short>(clientNumber, client, URIBase, strictCasing, TL, "InterfaceVersion", MemberTypes.Property);
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "InterfaceVersion", interfaceVersion.ToString());
            return interfaceVersion;
        }

        public static IList<string> SupportedActions(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL)
        {
            List<string> supportedActions = GetValue<List<string>>(clientNumber, client, URIBase, strictCasing, TL, "SupportedActions", MemberTypes.Property);
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "SupportedActions", $"Returning {supportedActions.Count} actions");

            List<string> returnValues = new List<string>();
            foreach (string action in supportedActions)
            {
                returnValues.Add(action);
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "SupportedActions", $"Returning action: {action}");
            }

            return returnValues;
        }

        #endregion

        #region Complex Camera Properties

        public static object ImageArrayVariant(uint clientNumber, RestClient client, string URIBase, bool strictCasing, ILogger TL, ImageArrayTransferType imageArrayTransferType, ImageArrayCompression imageArrayCompression)
        {
            Array returnArray;
            object[,] objectArray2D;
            object[,,] objectArray3D;
            Stopwatch sw = new Stopwatch();

            returnArray = GetValue<Array>(clientNumber, client, URIBase, strictCasing, TL, "ImageArrayVariant", imageArrayTransferType, imageArrayCompression, MemberTypes.Property);

            try
            {
                string variantType = returnArray.GetType().Name;
                string elementType;
                switch (returnArray.Rank) // Process 2D and 3D variant arrays, all other types are unsupported
                {
                    case 2: // 2D Array
                        elementType = returnArray.GetValue(0, 0).GetType().Name;
                        break;
                    case 3: // 3D array
                        elementType = returnArray.GetValue(0, 0, 0).GetType().Name;
                        break;
                    default:
                        throw new InvalidValueException("ReturnImageArray: Received an unsupported return array rank: " + returnArray.Rank);
                }

                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "ImageArrayVariant", $"Received {variantType} array of Rank {returnArray.Rank} with Length {returnArray.Length} and element type {elementType}");

                // convert to variant

                switch (returnArray.Rank)
                {
                    case 2:
                        objectArray2D = new object[returnArray.GetLength(0), returnArray.GetLength(1)];
                        switch (variantType)
                        {
                            case "Int16[,]":
                                Int16[,] short2DArray = (Int16[,])returnArray;

                                sw.Restart();
                                Parallel.For(0, short2DArray.GetLength(0), (i) =>
                                {
                                    for (int j = 0; j < returnArray.GetLength(1); j++)
                                    {
                                        objectArray2D[i, j] = short2DArray[i, j];
                                    }
                                });

                                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "ImageArrayVariant", $"Finished copying Int16[,] array in {sw.ElapsedMilliseconds}ms.");
                                return objectArray2D;

                            case "Int32[,]":
                                int[,] int2DArray = (int[,])returnArray;

                                sw.Restart();
                                Parallel.For(0, int2DArray.GetLength(0), (i) =>
                                {
                                    for (int j = 0; j < returnArray.GetLength(1); j++)
                                    {
                                        objectArray2D[i, j] = int2DArray[i, j];
                                    }
                                });

                                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "ImageArrayVariant", $"Finished copying Int32[,] array in {sw.ElapsedMilliseconds}ms.");
                                return objectArray2D;

                            case "Double[,]":
                                for (int i = 0; i < returnArray.GetLength(1); i++)
                                {
                                    for (int j = 0; j < returnArray.GetLength(0); j++)
                                    {
                                        objectArray2D[j, i] = ((double[,])returnArray)[j, i];
                                    }
                                }
                                double[,] double2DArray = (double[,])returnArray;

                                sw.Restart();
                                Parallel.For(0, double2DArray.GetLength(0), (i) =>
                                {
                                    for (int j = 0; j < returnArray.GetLength(1); j++)
                                    {
                                        objectArray2D[i, j] = double2DArray[i, j];
                                    }
                                });

                                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "ImageArrayVariant", $"Finished copying Double[,] array in {sw.ElapsedMilliseconds}ms.");

                                return objectArray2D;

                            case "Object[,]":
                                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "ImageArrayVariant", $"Returning Object[,] array to client");
                                return returnArray;

                            default:
                                throw new InvalidValueException("DynamicRemoteClient Driver Camera.ImageArrayVariant: Unsupported return array rank from DynamicClientDriver.GetValue<Array>: " + returnArray.Rank);
                        }
                    case 3:
                        objectArray3D = new object[returnArray.GetLength(0), returnArray.GetLength(1), returnArray.GetLength(2)];
                        switch (variantType)
                        {
                            case "Int16[,,]":
                                for (int i = 0; i < returnArray.GetLength(1); i++)
                                {
                                    for (int j = 0; j < returnArray.GetLength(0); j++)
                                    {
                                        for (int k = 0; k < returnArray.GetLength(2); k++)
                                            objectArray3D[j, i, k] = ((short[,,])returnArray)[j, i, k];
                                    }
                                }
                                return objectArray3D;

                            case "Int32[,,]":
                                sw.Restart();
                                int[,,] int3DArray = (int[,,])returnArray;

                                for (int k = 0; k < returnArray.GetLength(2); k++)
                                {
                                    for (int j = 0; j < returnArray.GetLength(1); j++)
                                    {
                                        for (int i = 0; i < returnArray.GetLength(0); i++)
                                        {
                                            objectArray3D[i, j, k] = int3DArray[i, j, k];
                                        }
                                    }
                                }

                                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "ImageArrayVariant", $"Finished copying Int32[,,] array in {sw.ElapsedMilliseconds}ms.");

                                return objectArray3D;

                            case "Double[,,]":
                                for (int i = 0; i < returnArray.GetLength(1); i++)
                                {
                                    for (int j = 0; j < returnArray.GetLength(0); j++)
                                    {
                                        for (int k = 0; k < returnArray.GetLength(2); k++)
                                            objectArray3D[j, i, k] = ((double[,,])returnArray)[j, i, k];
                                    }
                                }
                                return objectArray3D;

                            case "Object[,,]":
                                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "ImageArrayVariant", $"Returning Object[,,] array to client");
                                return returnArray;

                            default:
                                throw new InvalidValueException("DynamicRemoteClient Driver Camera.ImageArrayVariant: Unsupported return array rank from DynamicClientDriver.GetValue<Array>: " + returnArray.Rank);
                        }

                    default:
                        throw new InvalidValueException("DynamicRemoteClient Driver Camera.ImageArrayVariant: Unsupported return array rank from DynamicClientDriver.GetValue<Array>: " + returnArray.Rank);
                }
            }
            catch (Exception ex)
            {
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "ImageArrayVariant", $"Exception: \r\n{ex}");
                throw;
            }
        }

        #endregion

    }
}
