using ASCOM.Common;
using ASCOM.Common.Alpaca;
using ASCOM.Common.Com;
using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.Interfaces;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
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

        // Default image array transfer constants
        private const ImageArrayCompression IMAGE_ARRAY_COMPRESSION_DEFAULT = ImageArrayCompression.None;
        private const ImageArrayTransferType IMAGE_ARRAY_TRANSFER_TYPE_DEFAULT = ImageArrayTransferType.Base64HandOff;

        // Dynamic client configuration constants
        private const int SOCKET_ERROR_MAXIMUM_RETRIES = 1; // The number of retries that the client will make when it receives a socket actively refused error from the remote device
        private const int SOCKET_ERROR_RETRY_DELAY_TIME = 100; // The delay time (milliseconds) between socket actively refused retries
        private const string CONTENT_TYPE_HEADER_NAME = "Content-Type"; // Name of HTTP header used to affirm the type of data returned by the device

        //Private variables
        private static uint uniqueTransactionNumber = 0; // Unique number that increments on each call to TransactionNumber

        // Lock objects
        private readonly static object transactionCountlockObject = new object();

        #endregion

        #region Initialiser

        /// <summary>
        /// Static initialiser to set up the objects we need at run time
        /// </summary>
        static DynamicClientDriver()
        {
        }

        #endregion

        #region Utility code

        /// <summary>
        /// Returns a unique client number to the calling instance in the range 1::65536
        /// </summary>
        internal static uint GetUniqueClientNumber()
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
        internal static uint GetNextTransactionNumber()
        {
            lock (transactionCountlockObject)
            {
                uniqueTransactionNumber += 1;
            }
            return uniqueTransactionNumber;
        }

        /// <summary>
        /// Create and configure a REST client to communicate with the Alpaca device
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="ipAddressString"></param>
        /// <param name="portNumber"></param>
        /// <param name="serviceType"></param>
        /// <param name="logger"></param>
        /// <param name="clientNumber"></param>
        /// <param name="deviceType"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="imageArrayCompression"></param>
        /// <param name="userAgentProductName"></param>
        /// <param name="userAgentProductVersion"></param>
        /// <param name="trustUnsignedSslCertificates"></param>
        internal static void CreateHttpClient(ref HttpClient httpClient, ServiceType serviceType, string ipAddressString, decimal portNumber,
                                                 uint clientNumber, DeviceTypes deviceType, string userName, string password, ImageArrayCompression imageArrayCompression, ILogger logger,
                                                 string userAgentProductName, string userAgentProductVersion, bool trustUnsignedSslCertificates)
        {
            string clientHostAddress = $"{serviceType.ToString().ToLowerInvariant()}://{ipAddressString}:{portNumber}";

            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, Devices.DeviceTypeToString(deviceType), $"Connecting to device: {ipAddressString}:{portNumber} through URL: {clientHostAddress}");

            #region Commented automatic Alpaca device rediscovery code
            // Test whether automatic Alpaca device rediscovery is enabled for this device
            //if (enableRediscovery) // Automatic rediscovery is enabled
            //{
            //    AlpacaDeviceBaseClass.LogMessage(,clientNumber, deviceType, $"Testing whether client at address {clientHostAddress} can be contacted.");

            //    // Test whether there is a device at the configured IP address and port by trying to open a TCP connection to it
            //    if (!ClientIsUp(ipAddressString, portNumber, connectionTimeout, , clientNumber)) // It was not possible to establish TCP communication with a device at the IP address provided
            //    {
            //        // Attempt to "re-discover" the device and use it's new address and / or port
            //        AlpacaDeviceBaseClass.LogMessage(,clientNumber, deviceType, $"The device at the configured IP address and port {ipAddressString} cannot be contacted, attempting to re-discover it");

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
            //                AlpacaDeviceBaseClass.LogMessage(,clientNumber, deviceType, $"Found Alpaca device {alpacaDevice.HostName}:{alpacaDevice.Port} - {alpacaDevice.ServerName}");

            //                // Iterate over the devices served by the Alpaca device
            //                foreach (ConfiguredDevice ascomDevice in alpacaDevice.ConfiguredDevices)
            //                {
            //                    AlpacaDeviceBaseClass.LogMessage(,clientNumber, deviceType, $"Found ASCOM device {ascomDevice.DeviceName}:{ascomDevice.DeviceType} - {ascomDevice.UniqueID} at {alpacaDevice.HostName}:{alpacaDevice.Port}");

            //                    // Test whether the found ASCOM device has the same unique ID as the device for which we are looking
            //                    if (ascomDevice.UniqueID.ToLowerInvariant() == uniqueId.ToLowerInvariant()) // We have a match so we can use this address and port instead of the configured values that no longer work
            //                    {
            //                        AlpacaDeviceBaseClass.LogMessage(,clientNumber, deviceType, $"  *** Found REQUIRED ASCOM device ***");

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
            //                logger.BlankLine();
            //            }

            //        }

            //        // Search the discovered interfaces for the one whose network address is closest to the original address
            //        // This will ensure that we pick an address on the original subnet if this is available.
            //        switch (availableInterfaces.Count)
            //        {
            //            case 0:
            //                AlpacaDeviceBaseClass.LogMessage(,clientNumber, deviceType, $"No ASCOM device was discovered that had a UniqueD of {uniqueId}");
            //                logger.BlankLine();
            //                break;

            //            case 1:
            //                // Update the client host address with the newly discovered address and port
            //                clientHostAddress = $"{serviceType}://{availableInterfaces[0].HostName}:{availableInterfaces[0].Port}";
            //                AlpacaDeviceBaseClass.LogMessage(,clientNumber, deviceType, $"One ASCOM device was discovered that had a UniqueD of {uniqueId}. Now using URL: {clientHostAddress}");

            //                // Write the new value to the driver's Profile so it is found immediately in future
            //                using (Profile profile = new Profile())
            //                {
            //                    profile.DeviceType = deviceType;
            //                    profile.WriteValue(driverProgId, SharedConstants.IPADDRESS_PROFILENAME, availableInterfaces[0].HostName);
            //                    profile.WriteValue(driverProgId, SharedConstants.PORTNUMBER_PROFILENAME, availableInterfaces[0].Port.ToString());
            //                    AlpacaDeviceBaseClass.LogMessage(,clientNumber, deviceType, $"Written new values {availableInterfaces[0].HostName} and {availableInterfaces[0].Port} to profile {driverProgId}");
            //                }

            //                logger.BlankLine();
            //                break;

            //            default:
            //                AlpacaDeviceBaseClass.LogMessage(,clientNumber, deviceType, $"{availableInterfaces.Count} ASCOM devices were discovered that had a UniqueD of {uniqueId}.");

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
            //                AlpacaDeviceBaseClass.LogMessage(,clientNumber, deviceType, $"Initialised largest value: {largestDifference} = {largestDifference:X34}");

            //                // Now iterate over the values and pick the entry with the smallest difference in IP address
            //                foreach (AvailableInterface availableInterface in availableInterfaces)
            //                {
            //                    if (availableInterface.AddressDistance < largestDifference)
            //                    {
            //                        largestDifference = availableInterface.AddressDistance;
            //                        clientHostAddress = $"{serviceType}://{availableInterface.HostName}:{availableInterface.Port}";

            //                        AlpacaDeviceBaseClass.LogMessage(,clientNumber, deviceType, $"New lowest address difference found: {availableInterface.AddressDistance} ({availableInterface.AddressDistance:X32}) for UniqueD {uniqueId}. Now using URL: {clientHostAddress}");

            //                        // Write the new value to the driver's Profile so it is found immediately in future
            //                        using (Profile profile = new Profile())
            //                        {
            //                            profile.DeviceType = deviceType;
            //                            profile.WriteValue(driverProgId, SharedConstants.IPADDRESS_PROFILENAME, availableInterface.HostName);
            //                            profile.WriteValue(driverProgId, SharedConstants.PORTNUMBER_PROFILENAME, availableInterface.Port.ToString());
            //                            AlpacaDeviceBaseClass.LogMessage(,clientNumber, deviceType, $"Written new values {availableInterface.HostName} and {availableInterface.Port} to profile {driverProgId}");
            //                        }
            //                    }
            //                }


            //                logger.BlankLine();
            //                break;
            //        }
            //    }
            //}
            #endregion

            // Remove any old client, if present
            httpClient?.Dispose();

            // Convert from the Alpaca decompression enum to the HttpClient decompression enum
            DecompressionMethods decompressionMethods;
            switch (imageArrayCompression)
            {
                case ImageArrayCompression.None:
                    decompressionMethods = DecompressionMethods.None;
                    break;
                case ImageArrayCompression.GZip:
                    decompressionMethods = DecompressionMethods.GZip;
                    break;
                case ImageArrayCompression.Deflate:
                    decompressionMethods = DecompressionMethods.Deflate;
                    break;
                case ImageArrayCompression.GZipOrDeflate:
                    decompressionMethods = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                    break;
                default:
                    throw new InvalidValueException($"Invalid image array compression value: {imageArrayCompression}");
            }

            // Create a new http handler to control authentication and automatic decompression
            HttpClientHandler httpClientHandler = new HttpClientHandler
            {
                PreAuthenticate = true,
                AutomaticDecompression = decompressionMethods,
                ClientCertificateOptions = ClientCertificateOption.Manual
            };

            // Trust self-signed certificates if requested to do so
            if (trustUnsignedSslCertificates)
            {
                httpClientHandler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };
            }

            // Create a new client pointing at the alpaca device
            httpClient = new HttpClient(httpClientHandler);

            // Add a basic authenticator if the user name is not null
            if (!string.IsNullOrEmpty(userName))
            {
                byte[] authenticationBytes;
                // Deal with null passwords
                if (string.IsNullOrEmpty(password)) // Handle the special case of a null string password
                {
                    // Create authenticator bytes configured with the user name and empty password
                    authenticationBytes = Encoding.ASCII.GetBytes($"{userName}:");
                }
                else // Handle the normal case of a non-empty string username and password
                {
                    // Create authenticator bytes configured with the user name and provided password
                    authenticationBytes = Encoding.ASCII.GetBytes($"{userName}:{password}");
                }

                // Set the authentication header for all requests
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authenticationBytes));
            }

            // Set the base URI for the device
            httpClient.BaseAddress = new Uri(clientHostAddress);

            string userproductName = userAgentProductName;
            string productVersion = userAgentProductVersion;

            if (string.IsNullOrEmpty(userproductName))
            {
                userproductName = AlpacaClient.CLIENT_USER_AGENT_PRODUCT_NAME;
            }

            if (string.IsNullOrEmpty(productVersion))
            {
                productVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            }

            // Add default headers for JSON
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(AlpacaConstants.APPLICATION_JSON_MIME_TYPE));
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(userproductName, productVersion));
            httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
            httpClient.DefaultRequestHeaders.ConnectionClose = false;
        }

        // /// <summary>
        // /// test whether there is a device at the specified IP address and port by opening a TCP connection to it
        // /// </summary>
        // /// <param name="ipAddressString">IP address of the device</param>
        // /// <param name="portNumber">IP port number on the device</param>
        // /// <param name="connectionTimeout">Time to wait before timing out</param>
        // /// <param name="TL">Trace logger in which to report progress</param>
        // /// <param name="clientNumber">The client's number</param>
        // /// <returns></returns>
        //private static bool ClientIsUp(string ipAddressString, decimal portNumber, int connectionTimeout, ILogger logger, uint clientNumber)
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
        //            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ClientIsUp", $"Created an {ipAddress.AddressFamily} TCP client");
        //        }
        //        else
        //        {
        //            tcpClient = new TcpClient(); // Create a generic TcpClient that can work with host names
        //        }

        //        // Create a task that will return True if a connection to the device can be established or False if the connection is rejected or not possible
        //        Task<bool> connectionTask = tcpClient.ConnectAsync(ipAddressString, (int)portNumber).ContinueWith(task => { return !task.IsFaulted; }, TaskContinuationOptions.ExecuteSynchronously);
        //        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ClientIsUp", $"Created connection task");

        //        // Create a task that will time out after the specified time and return a value of False
        //        Task<bool> timeoutTask = Task.Delay(connectionTimeout * 1000).ContinueWith<bool>(task => false, TaskContinuationOptions.ExecuteSynchronously);
        //        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ClientIsUp", $"Created timeout task");

        //        // Create a task that will wait until either of the two preceding tasks completes
        //        Task<bool> resultTask = Task.WhenAny(connectionTask, timeoutTask).Unwrap();
        //        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ClientIsUp", $"Waiting for a task to complete");

        //        // Wait for one of the tasks to complete
        //        resultTask.Wait();
        //        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ClientIsUp", $"A task has completed");

        //        // Test whether or not we connected OK within the timeout period
        //        if (resultTask.Result) // We did connect OK within the timeout period
        //        {
        //            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ClientIsUp", $"Contacted client OK!");
        //            tcpClient.Close();
        //            returnValue = true;
        //        }
        //        else // We did not connect successfully within the timeout period
        //        {
        //            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ClientIsUp", $"Unable to contact client....");
        //            returnValue = false;
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ClientIsUp", $"Exception: {ex}");

        //    }
        //    finally
        //    {
        //        tcpClient.Dispose();
        //    }
        //    return returnValue;
        //}

        #endregion

        #region Remote access methods

        internal static void CallMethodWithNoParameters(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger, string method, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>();
            SendToRemoteDevice<NoReturnValue>(clientNumber, client, timeout, URIBase, strictCasing, logger, method, Parameters, HttpMethod.Put, memberType);
        }

        /// <summary>
        /// Overload used by methods other than ImageArray and ImageArrayVariant
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="clientNumber"></param>
        /// <param name="client"></param>
        /// <param name="timeout"></param>
        /// <param name="URIBase"></param>
        /// <param name="strictCasing"></param>
        /// <param name="logger"></param>
        /// <param name="method"></param>
        /// <param name="memberType"></param>
        /// <returns></returns>
        internal static T GetValue<T>(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger, string method, MemberTypes memberType)
        {
            return GetValue<T>(clientNumber, client, timeout, URIBase, strictCasing, logger, method, IMAGE_ARRAY_TRANSFER_TYPE_DEFAULT, IMAGE_ARRAY_COMPRESSION_DEFAULT, memberType); // Set an arbitrary value for ImageArrayTransferType
        }

        /// <summary>
        /// Overload for use by the ImageArray and ImageArrayVariant methods
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="clientNumber"></param>
        /// <param name="client"></param>
        /// <param name="timeout"></param>
        /// <param name="URIBase"></param>
        /// <param name="strictCasing"></param>
        /// <param name="logger"></param>
        /// <param name="method"></param>
        /// <param name="imageArrayTransferType"></param>
        /// <param name="imageArrayCompression"></param>
        /// <param name="memberType"></param>
        /// <returns></returns>
        internal static T GetValue<T>(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger, string method, ImageArrayTransferType imageArrayTransferType, ImageArrayCompression imageArrayCompression, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>();
            return SendToRemoteDevice<T>(clientNumber, client, timeout, URIBase, strictCasing, logger, method, Parameters, HttpMethod.Get, imageArrayTransferType, imageArrayCompression, memberType);
        }

        internal static void SetBool(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger, string method, bool parmeterValue, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { method, parmeterValue.ToString(CultureInfo.InvariantCulture) }
            };
            SendToRemoteDevice<NoReturnValue>(clientNumber, client, timeout, URIBase, strictCasing, logger, method, Parameters, HttpMethod.Put, memberType);
        }

        internal static void SetInt(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger, string method, int parmeterValue, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { method, parmeterValue.ToString(CultureInfo.InvariantCulture) }
            };
            SendToRemoteDevice<NoReturnValue>(clientNumber, client, timeout, URIBase, strictCasing, logger, method, Parameters, HttpMethod.Put, memberType);
        }

        internal static void SetShort(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger, string method, short parmeterValue, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { method, parmeterValue.ToString(CultureInfo.InvariantCulture) }
            };
            SendToRemoteDevice<NoReturnValue>(clientNumber, client, timeout, URIBase, strictCasing, logger, method, Parameters, HttpMethod.Put, memberType);
        }

        internal static void SetDouble(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger, string method, double parmeterValue, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { method, parmeterValue.ToString(CultureInfo.InvariantCulture) }
            };
            SendToRemoteDevice<NoReturnValue>(clientNumber, client, timeout, URIBase, strictCasing, logger, method, Parameters, HttpMethod.Put, memberType);
        }

        internal static void SetDoubleWithShortParameter(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger, string method, short index, double parmeterValue, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.ID_PARAMETER_NAME, index.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.VALUE_PARAMETER_NAME, parmeterValue.ToString(CultureInfo.InvariantCulture) }
            };
            SendToRemoteDevice<NoReturnValue>(clientNumber, client, timeout, URIBase, strictCasing, logger, method, Parameters, HttpMethod.Put, memberType);
        }

        internal static void SetBoolWithShortParameter(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger, string method, short index, bool parmeterValue, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.ID_PARAMETER_NAME, index.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.STATE_PARAMETER_NAME, parmeterValue.ToString(CultureInfo.InvariantCulture) }
            };
            SendToRemoteDevice<NoReturnValue>(clientNumber, client, timeout, URIBase, strictCasing, logger, method, Parameters, HttpMethod.Put, memberType);
        }

        internal static void SetStringWithShortParameter(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger, string method, short index, string parmeterValue, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.ID_PARAMETER_NAME, index.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.NAME_PARAMETER_NAME, parmeterValue.ToString(CultureInfo.InvariantCulture) }
            };
            SendToRemoteDevice<NoReturnValue>(clientNumber, client, timeout, URIBase, strictCasing, logger, method, Parameters, HttpMethod.Put, memberType);
        }

        internal static string GetStringIndexedString(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger, string method, string parameterValue, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.SENSORNAME_PARAMETER_NAME, parameterValue }
            };
            return SendToRemoteDevice<string>(clientNumber, client, timeout, URIBase, strictCasing, logger, method, Parameters, HttpMethod.Get, memberType);
        }

        internal static double GetStringIndexedDouble(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger, string method, string parameterValue, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.SENSORNAME_PARAMETER_NAME, parameterValue }
            };
            return SendToRemoteDevice<double>(clientNumber, client, timeout, URIBase, strictCasing, logger, method, Parameters, HttpMethod.Get, memberType);
        }

        internal static double GetShortIndexedDouble(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger, string method, short parameterValue, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.ID_PARAMETER_NAME, parameterValue.ToString(CultureInfo.InvariantCulture) }
            };
            return SendToRemoteDevice<double>(clientNumber, client, timeout, URIBase, strictCasing, logger, method, Parameters, HttpMethod.Get, memberType);
        }

        internal static bool GetShortIndexedBool(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger, string method, short parameterValue, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.ID_PARAMETER_NAME, parameterValue.ToString(CultureInfo.InvariantCulture) }
            };
            return SendToRemoteDevice<bool>(clientNumber, client, timeout, URIBase, strictCasing, logger, method, Parameters, HttpMethod.Get, memberType);
        }

        internal static string GetShortIndexedString(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger, string method, short parameterValue, MemberTypes memberType)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.ID_PARAMETER_NAME, parameterValue.ToString(CultureInfo.InvariantCulture) }
            };
            return SendToRemoteDevice<string>(clientNumber, client, timeout, URIBase, strictCasing, logger, method, Parameters, HttpMethod.Get, memberType);
        }

        /// <summary>
        /// Send a command to the remote device, retrying a given number of times if a socket exception is received
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="clientNumber"></param>
        /// <param name="client"></param>
        /// <param name="timeout"></param>
        /// <param name="URIBase"></param>
        /// <param name="strictCasing"></param>
        /// <param name="logger"></param>
        /// <param name="method"></param>
        /// <param name="Parameters"></param>
        /// <param name="HttpMethod"></param>
        /// <param name="memberType"></param>
        /// <returns></returns>
        internal static T SendToRemoteDevice<T>(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger, string method, Dictionary<string, string> Parameters, HttpMethod HttpMethod, MemberTypes memberType)
        {
            return SendToRemoteDevice<T>(clientNumber, client, timeout, URIBase, strictCasing, logger, method, Parameters, HttpMethod, IMAGE_ARRAY_TRANSFER_TYPE_DEFAULT, IMAGE_ARRAY_COMPRESSION_DEFAULT, memberType);
        }

        /// <summary>
        /// Send a command to the remote device, retrying a given number of times if a socket exception is received, specifying an image array transfer type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="clientNumber"></param>
        /// <param name="client"></param>
        /// <param name="timeout">Timeout in seconds</param>
        /// <param name="uriBase"></param>
        /// <param name="strictCasing"></param>
        /// <param name="logger"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <param name="httpMethod"></param>
        /// <param name="imageArrayTransferType"></param>
        /// <param name="imageArrayCompression"></param>
        /// <param name="memberType"></param>
        /// <returns></returns>
        internal static T SendToRemoteDevice<T>(uint clientNumber, HttpClient client, int timeout, string uriBase, bool strictCasing, ILogger logger, string method, Dictionary<string, string> parameters, HttpMethod httpMethod, ImageArrayTransferType imageArrayTransferType, ImageArrayCompression imageArrayCompression, MemberTypes memberType)
        {
            int retryCounter = 0; // Initialise the socket error retry counter
            Stopwatch sw = new Stopwatch(); // Stopwatch to time activities
            Stopwatch swOverall = new Stopwatch();
            long lastTime = 0; // Holder for the accumulated elapsed time, used when reporting intermediate step timings
            Array remoteArray = null;
            string responseContentType = "NoContentTypeProvided";
            string responseJson = "";
            HttpRequestMessage request; // HTTP request definition
            byte[] rawBytes = new byte[0];

            sw.Start();
            swOverall.Start();

            do // Socket communications error retry loop
            {
                try
                {
                    const string LOG_FORMAT_STRING = "Client Transaction ID: {0}, Server Transaction ID: {1}, Value: {2}";

                    Response restResponseBase = null; // This has to be the base class of the data type classes in order for exception and error responses to be handled generically

                    // Create a new transaction number
                    uint transactionId = GetNextTransactionNumber();

                    // Create the URI for this transaction and apply it to the request, adding "client id" and "transaction number" query parameters
                    UriBuilder transactionUri = new UriBuilder($"{client.BaseAddress}{uriBase}{method}".ToLowerInvariant());

                    // Create a new request message to be sent to the device
                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"HTTP {httpMethod} - UriBase: '{uriBase}', Device URI: {transactionUri.Uri}");

                    // Process HTTP GET and PUT methods
                    if (httpMethod == HttpMethod.Get) // HTTP GET methods
                    {
                        // Add client id and transaction id query parameters
                        transactionUri.Query = $"{AlpacaConstants.CLIENTID_PARAMETER_NAME}={clientNumber}&{AlpacaConstants.CLIENTTRANSACTION_PARAMETER_NAME}={transactionId}";

                        // Add to the query string any further required parameters for HTTP GET methods
                        if (parameters.Count > 0)
                        {
                            foreach (KeyValuePair<string, string> parameter in parameters)
                            {
                                transactionUri.Query = $"{transactionUri.Query}&{parameter.Key}={parameter.Value}";
                            }
                        }

                        // Create a new request based on the transaction Uri
                        request = new HttpRequestMessage(HttpMethod.Get, transactionUri.Uri);

                        // If required, apply headers to control camera image array retrieval
                        if (typeof(T) == typeof(Array))
                        {
                            switch (imageArrayTransferType)
                            {
                                case ImageArrayTransferType.JSON:
                                    // No extra action because "accepts = application/json" will be applied automatically by the client
                                    break;

                                case ImageArrayTransferType.Base64HandOff:
                                    request.Headers.Add(AlpacaConstants.BASE64_HANDOFF_HEADER, AlpacaConstants.BASE64_HANDOFF_SUPPORTED);
                                    break;

                                case ImageArrayTransferType.ImageBytes:
                                    //request.Headers.Add(ACCEPT_HEADER_NAME, AlpacaConstants.IMAGE_BYTES_MIME_TYPE);
                                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(AlpacaConstants.IMAGE_BYTES_MIME_TYPE));
                                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(AlpacaConstants.APPLICATION_JSON_MIME_TYPE));
                                    break;

                                case ImageArrayTransferType.BestAvailable:
                                    request.Headers.Add(AlpacaConstants.BASE64_HANDOFF_HEADER, AlpacaConstants.BASE64_HANDOFF_SUPPORTED);
                                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(AlpacaConstants.IMAGE_BYTES_MIME_TYPE));
                                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(AlpacaConstants.APPLICATION_JSON_MIME_TYPE));
                                    //request.Headers.Add(ACCEPT_HEADER_NAME, AlpacaConstants.IMAGE_BYTES_MIME_TYPE);
                                    break;

                                default:
                                    throw new InvalidValueException($"Invalid image array transfer type: {imageArrayTransferType} - Correct this in the Dynamic Client setup dialogue.");
                            }
                        }
                    }
                    else if (httpMethod == HttpMethod.Put) // HTTP Put methods
                    {
                        // Create a new request based on the transaction Uri
                        request = new HttpRequestMessage(HttpMethod.Put, transactionUri.Uri);

                        // Add the client id and transaction id parameters to the body parameter list
                        if (!parameters.ContainsKey(AlpacaConstants.CLIENTID_PARAMETER_NAME)) // The key may already be present in the dictionary from a previous retry loop, hence this conditional test to avoid an exception being thrown
                        {
                            parameters.Add(AlpacaConstants.CLIENTID_PARAMETER_NAME, clientNumber.ToString());
                        }
                        if (!parameters.ContainsKey(AlpacaConstants.CLIENTTRANSACTION_PARAMETER_NAME)) // The key may already be present in the dictionary from a previous retry loop, hence this conditional test to avoid an exception being thrown
                        {
                            parameters.Add(AlpacaConstants.CLIENTTRANSACTION_PARAMETER_NAME, transactionId.ToString());
                        }

                        // Add all parameters to the request body as form URL encoded content
                        if (parameters.Count > 0)
                        {
                            FormUrlEncodedContent formParameters = new FormUrlEncodedContent(parameters);
                            request.Content = formParameters;
                        }
                        else
                        {
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"No form parameters to include in this request.");
                        }

                        // Log the PUT parameters
                        foreach (KeyValuePair<string, string> parameter in parameters)
                        {
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Sending form parameter {parameter.Key} = {parameter.Value}");
                        }

                    }
                    else
                    {
                        throw new InvalidValueException($"DynamicClientDriver only supports the GET and PUT methods. It does not support the {httpMethod} method.");
                    }

                    // Log the default headers 
                    foreach (KeyValuePair<string, IEnumerable<string>> headerCollection in client.DefaultRequestHeaders)
                    {
                        foreach (string headerValue in headerCollection.Value)
                        {
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Sending header {headerCollection.Key} = {headerValue}");
                        }
                    }

                    // Log any headers specifically added to this request
                    foreach (KeyValuePair<string, IEnumerable<string>> headerCollection in request.Headers)
                    {
                        foreach (string headerValue in headerCollection.Value)
                        {
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Sending header {headerCollection.Key} = {headerValue}");
                        }
                    }

                    // Log the URI
                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Client Transaction ID: {transactionId}, Remote device: {client.BaseAddress}, Full URI: {request.RequestUri}");

                    // Create a cancellation token that will time out after the required retry interval
                    lastTime = sw.ElapsedMilliseconds;
                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                    cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(timeout));

                    // Send the request to the remote device and wait synchronously for the response
                    HttpResponseMessage deviceResponse = client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationTokenSource.Token).Result;

                    // Assess success at the HTTP status level and handle accordingly 
                    if (deviceResponse.IsSuccessStatusCode) // Success - HTTP status is in the range 200::299
                    {
                        // Get the response headers
                        HttpContentHeaders responseHeaders = deviceResponse.Content.Headers;

                        // Throw an exception if there is no content-type header indicating what has been returned
                        if (responseHeaders is null) throw new InvalidValueException("The device did not return any headers. Expected a Content-Type header with a value of 'application/json' or 'application/imagebytes'.");

                        // List the headers received
                        foreach (var header in responseHeaders)
                        {
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Received header {header.Key} = {header.Value.FirstOrDefault()}.");
                        }

                        // Extract the content type from the returned headers
                        if (responseHeaders.TryGetValues(CONTENT_TYPE_HEADER_NAME, out IEnumerable<string> contentTypeValues))
                        {
                            responseContentType = contentTypeValues.First().ToLowerInvariant();
                        }
                        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Returned data content type: '{responseContentType}'");

                        long timeDeviceResponse = sw.ElapsedMilliseconds - lastTime;

                        // Get the returned data as a byte[] (could be JSON text or ImageBytes image data)
                        sw.Restart();
                        rawBytes = deviceResponse.Content.ReadAsByteArrayAsync().Result;
                        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Received {rawBytes.Length} bytes, ReadAsByteArrayAsync time: {sw.ElapsedMilliseconds}ms, Overall time: {swOverall.ElapsedMilliseconds}ms.");

                        // Log whatever bytes were returned
                        //StringBuilder rawBytesString = new StringBuilder();

                        //if (rawBytes.Length <= 500)
                        //{
                        //    foreach (byte b in rawBytes)
                        //    {
                        //        rawBytesString.Append($"[{b:X2}] ");
                        //    }
                        //}
                        //else
                        //{
                        //    rawBytesString.Append("More than 500 bytes were returned.");
                        //}
                        //AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Raw bytes: {rawBytesString}");

                        // Log the device's response
                        if (responseContentType.ToLowerInvariant().Contains(AlpacaConstants.IMAGE_BYTES_MIME_TYPE)) // Image bytes response
                        {
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Received an ImageBytes response: '{deviceResponse.ReasonPhrase}', Content type: {responseContentType}");
                        }
                        else if (responseContentType.ToLowerInvariant().Contains(AlpacaConstants.APPLICATION_JSON_MIME_TYPE)) // JSON response
                        {
                            // Populate the JSON response variable with a string converted from the received byte[] 
                            responseJson = Encoding.UTF8.GetString(rawBytes);
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"JSON response: {(responseJson.Length < 1000 ? responseJson : responseJson.Substring(0, 1000))}");

                            // Check whether some characters were returned, if not flag this as an error
                            if (string.IsNullOrWhiteSpace(responseJson))
                            {
                                throw new InvalidOperationException($"The device did not return a JSON string, it returned: {(responseJson is null ? "a null value" : $"'{responseJson}'")}");
                            }

                            // De-serialise just the ErrorNumber and ErrorMessage fields, ignoring any content in the Value, ClientTransactionID and ServertransactionID fields 
                            ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });

                            // CHECK FOR AND THROW ALPACA ERRORS REPORTED BY ALPACA DEVICES THAT USE THE ERROR NUMBER AND ERROR MESSAGE FIELDS
                            if ((errorResponse.ErrorMessage != "") || (errorResponse.ErrorNumber != 0)) // An error has been returned so report it
                            {
                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Received an Alpaca error: {errorResponse.ErrorNumber}. Error message: {errorResponse.ErrorMessage}");

                                // Handle ASCOM Alpaca reserved error numbers between 0x400 and 0xFFF by translating these to the COM HResult error number range: 0x80040400 to 0x80040FFF and throwing the translated value as an exception
                                if ((errorResponse.ErrorNumber >= AlpacaErrors.AlpacaErrorCodeBase) & (errorResponse.ErrorNumber <= AlpacaErrors.AlpacaErrorCodeMax)) // This error is within the ASCOM Alpaca reserved error number range
                                {
                                    // Calculate the equivalent COM HResult error number from the supplied Alpaca error number so that comparison can be made with the original ASCOM COM exception HResult numbers that Windows clients expect in their exceptions
                                    int ascomCOMErrorNumber = (int)(errorResponse.ErrorNumber + (int)ComErrorCodes.ComErrorNumberOffset);
                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Received Alpaca error code: {errorResponse.ErrorNumber} (0x{(int)errorResponse.ErrorNumber:X4}), the equivalent COM error HResult error code is {ascomCOMErrorNumber} (0x{ascomCOMErrorNumber:X8})");

                                    // Now check whether the COM HResult matches any of the built-in ASCOM exception types. If so, we throw that exception type otherwise we throw a generic DriverException
                                    if (ascomCOMErrorNumber == ASCOM.ErrorCodes.ActionNotImplementedException) // Handle ActionNotImplementedException
                                    {
                                        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Alpaca action not implemented error, throwing ActionNotImplementedException - ErrorMessage: \"{errorResponse.ErrorMessage}\", ErrorNumber: 0x{ascomCOMErrorNumber:X8}");
                                        throw new ActionNotImplementedException(errorResponse.ErrorMessage);
                                    }
                                    else if (ascomCOMErrorNumber == ASCOM.ErrorCodes.InvalidOperationException) // Handle InvalidOperationException
                                    {
                                        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Alpaca invalid operation error, throwing InvalidOperationException - ErrorMessage: \"{errorResponse.ErrorMessage}\", ErrorNumber: 0x{ascomCOMErrorNumber:X8}");
                                        throw new InvalidOperationException(errorResponse.ErrorMessage);
                                    }
                                    else if (ascomCOMErrorNumber == ASCOM.ErrorCodes.InvalidValue) // Handle InvalidValueException
                                    {
                                        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Alpaca invalid value error, throwing InvalidValueException - ErrorMessage: \"{errorResponse.ErrorMessage}\", ErrorNumber: 0x{ascomCOMErrorNumber:X8}");
                                        throw new InvalidValueException(errorResponse.ErrorMessage);
                                    }
                                    else if (ascomCOMErrorNumber == ASCOM.ErrorCodes.InvalidWhileParked) // Handle ParkedException
                                    {
                                        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Alpaca invalid while parked error, throwing ParkedException - ErrorMessage: \"{errorResponse.ErrorMessage}\", ErrorNumber: 0x{ascomCOMErrorNumber:X8}");
                                        throw new ParkedException(errorResponse.ErrorMessage);
                                    }
                                    else if (ascomCOMErrorNumber == ASCOM.ErrorCodes.InvalidWhileSlaved) // Handle SlavedException
                                    {
                                        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $" Alpaca invalid while slaved error, throwing SlavedException - ErrorMessage: \"{errorResponse.ErrorMessage}\", ErrorNumber: 0x{ascomCOMErrorNumber:X8}");
                                        throw new SlavedException(errorResponse.ErrorMessage);
                                    }
                                    else if (ascomCOMErrorNumber == ASCOM.ErrorCodes.NotConnected) // Handle NotConnectedException
                                    {
                                        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $" Alpaca not connected error, throwing NotConnectedException - ErrorMessage: \"{errorResponse.ErrorMessage}\", ErrorNumber: 0x{ascomCOMErrorNumber:X8}");
                                        throw new NotConnectedException(errorResponse.ErrorMessage);
                                    }
                                    else if (ascomCOMErrorNumber == ASCOM.ErrorCodes.NotImplemented) // Handle PropertyNotImplementedException and MethodNotImplementedException (both have the same error code)
                                    {
                                        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Alpaca member not implemented error, throwing NotImplementedException - ErrorMessage: \"{errorResponse.ErrorMessage}\", ErrorNumber: 0x{ascomCOMErrorNumber:X8}");
                                        throw new NotImplementedException(errorResponse.ErrorMessage);
                                    }
                                    else if (ascomCOMErrorNumber == ASCOM.ErrorCodes.OperationCancelled) // Handle OperationCancelledException
                                    {
                                        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $" Alpaca operation cancelled error, throwing OperationCancelledException - ErrorMessage: \"{errorResponse.ErrorMessage}\", ErrorNumber: 0x{ascomCOMErrorNumber:X8}");
                                        throw new OperationCancelledException(errorResponse.ErrorMessage);
                                    }
                                    else if (ascomCOMErrorNumber == ASCOM.ErrorCodes.ValueNotSet) // Handle ValueNotSetException
                                    {
                                        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $" Alpaca value not set error, throwing ValueNotSetException - ErrorMessage: \"{errorResponse.ErrorMessage}\", ErrorNumber: 0x{ascomCOMErrorNumber:X8}");
                                        throw new ValueNotSetException(errorResponse.ErrorMessage);
                                    }
                                    else // The exception is inside the ASCOM Alpaca reserved range but is not one of those with their own specific exception types above, so wrap it in a DriverException and throw this to the client
                                    {
                                        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Alpaca undefined ASCOM error, throwing DriverException - ErrorMessage: \"{errorResponse.ErrorMessage}\", ErrorNumber: 0x{ascomCOMErrorNumber:X8}");
                                        throw new DriverException(errorResponse.ErrorMessage, ascomCOMErrorNumber);
                                    }
                                }
                                else // An exception has been thrown with an error number outside the ASCOM Alpaca reserved range, so wrap it in a DriverException and throw this to the client.
                                {
                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Alpaca error outside ASCOM reserved range, throwing DriverException - ErrorMessage: \"{errorResponse.ErrorMessage}\", ErrorNumber: 0x{(int)errorResponse.ErrorNumber:X8}");
                                    throw new DriverException(errorResponse.ErrorMessage, (int)errorResponse.ErrorNumber);
                                }
                            }

                            // CHECK FOR AND THROW EXCEPTIONS RETURNED BY WINDOWS BASED DRIVERS RUNNING IN THE REMOTE DEVICE
                            if (errorResponse.DriverException != null)
                            {
                                try
                                {
                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"JSON Exception found - Error message: \"{errorResponse.ErrorMessage}\", Error number: {errorResponse.ErrorNumber}");
                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Exception Message: \"{errorResponse.DriverException.Message}\", Exception Number: 0x{(int)errorResponse.DriverException.HResult:X8}");
                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Exception returned by device: {errorResponse.DriverException}");
                                }
                                catch (Exception ex1)
                                {
                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Exception logging error message! : {ex1}");
                                }
                                throw errorResponse.DriverException;
                            }
                        }
                        else // We didn't receive a content type header or received an unsupported content type so throw an exception to indicate the problem.
                        {
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "GetResponse", $"Did not find expected content type of 'application.json' or 'application/imagebytes'. Found: {responseContentType}");
                            throw new InvalidValueException($"The device did not return a content type or returned an unsupported content type: '{responseContentType}'");
                        }

                        // GENERAL MULTI-DEVICE TYPES
                        if (typeof(T) == typeof(bool))
                        {
                            BoolResponse boolResponse = JsonSerializer.Deserialize<BoolResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });// JsonSerializer.Deserialize<BoolResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, boolResponse.ClientTransactionID, boolResponse.ServerTransactionID, boolResponse.Value.ToString()));
                            return (T)(object)boolResponse.Value;
                        }
                        if (typeof(T) == typeof(float))
                        {
                            // Handle float as double over the web, remembering to convert the returned value to float
                            DoubleResponse doubleResponse = JsonSerializer.Deserialize<DoubleResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, doubleResponse.ClientTransactionID, doubleResponse.ServerTransactionID, doubleResponse.Value.ToString()));
                            float floatValue = (float)doubleResponse.Value;
                            return (T)(object)floatValue;
                        }
                        if (typeof(T) == typeof(double))
                        {
                            DoubleResponse doubleResponse = JsonSerializer.Deserialize<DoubleResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, doubleResponse.ClientTransactionID, doubleResponse.ServerTransactionID, doubleResponse.Value.ToString()));
                            return (T)(object)doubleResponse.Value;
                        }
                        if (typeof(T) == typeof(string))
                        {
                            StringResponse stringResponse = JsonSerializer.Deserialize<StringResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, stringResponse.ClientTransactionID, stringResponse.ServerTransactionID, (stringResponse.Value is null) ? "NO VALUE OR NULL VALUE RETURNED" : (stringResponse.Value.Length <= 500 ? stringResponse.Value : stringResponse.Value.Substring(0, 500))));
                            return (T)(object)stringResponse.Value;
                        }
                        if (typeof(T) == typeof(string[]))
                        {
                            StringArrayResponse stringArrayResponse = JsonSerializer.Deserialize<StringArrayResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, stringArrayResponse.ClientTransactionID, stringArrayResponse.ServerTransactionID, (stringArrayResponse.Value is null) ? "NO VALUE OR NULL VALUE RETURNED" : stringArrayResponse.Value.Count().ToString()));
                            return (T)(object)stringArrayResponse.Value;
                        }
                        if (typeof(T) == typeof(short))
                        {
                            ShortResponse shortResponse = JsonSerializer.Deserialize<ShortResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, shortResponse.ClientTransactionID, shortResponse.ServerTransactionID, shortResponse.Value.ToString()));
                            return (T)(object)shortResponse.Value;
                        }
                        if (typeof(T) == typeof(int))
                        {
                            IntResponse intResponse = JsonSerializer.Deserialize<IntResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, intResponse.ClientTransactionID, intResponse.ServerTransactionID, intResponse.Value.ToString()));
                            return (T)(object)intResponse.Value;
                        }
                        if (typeof(T) == typeof(int[]))
                        {
                            IntArray1DResponse intArrayResponse = JsonSerializer.Deserialize<IntArray1DResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, intArrayResponse.ClientTransactionID, intArrayResponse.ServerTransactionID, (intArrayResponse.Value is null) ? "NO VALUE OR NULL VALUE RETURNED" : intArrayResponse.Value.Count().ToString()));
                            return (T)(object)intArrayResponse.Value;
                        }
                        if (typeof(T) == typeof(DateTime))
                        {
                            DateTimeResponse dateTimeResponse = JsonSerializer.Deserialize<DateTimeResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, dateTimeResponse.ClientTransactionID, dateTimeResponse.ServerTransactionID, dateTimeResponse.Value.ToString()));
                            return (T)(object)dateTimeResponse.Value;
                        }
                        if (typeof(T) == typeof(List<string>)) // Used for ArrayLists of string
                        {
                            StringListResponse stringListResponse = JsonSerializer.Deserialize<StringListResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, stringListResponse.ClientTransactionID, stringListResponse.ServerTransactionID, (stringListResponse.Value is null) ? "NO VALUE OR NULL VALUE RETURNED" : stringListResponse.Value.Count.ToString()));
                            return (T)(object)stringListResponse.Value;
                        }
                        if (typeof(T) == typeof(List<StateValue>)) // Used for DeviceState property
                        {
                            DeviceStateResponse deviceStateResponse = JsonSerializer.Deserialize<DeviceStateResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, deviceStateResponse.ClientTransactionID, deviceStateResponse.ServerTransactionID, (deviceStateResponse.Value is null) ? "NO VALUE OR NULL VALUE RETURNED" : deviceStateResponse.Value.Count.ToString()));
                            return (T)(object)deviceStateResponse.Value;
                        }
                        if (typeof(T) == typeof(NoReturnValue)) // Used for Methods that have no response and Property Set members
                        {
                            MethodResponse methodResponse = JsonSerializer.Deserialize<MethodResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, methodResponse.ClientTransactionID.ToString(), methodResponse.ServerTransactionID.ToString(), "No response"));
                            return (T)(object)new NoReturnValue();
                        }

                        // DEVICE SPECIFIC TYPES
                        if (typeof(T) == typeof(PointingState))
                        {
                            IntResponse PointingStateResponse = JsonSerializer.Deserialize<IntResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, PointingStateResponse.ClientTransactionID, PointingStateResponse.ServerTransactionID, PointingStateResponse.Value.ToString()));
                            return (T)(object)PointingStateResponse.Value;
                        }
                        if (typeof(T) == typeof(ITrackingRates))
                        {
                            TrackingRatesResponse trackingRatesResponse = JsonSerializer.Deserialize<TrackingRatesResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            if (trackingRatesResponse.Value != null) // A TrackingRates object was returned so process the response normally
                            {
                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, trackingRatesResponse.ClientTransactionID, trackingRatesResponse.ServerTransactionID, trackingRatesResponse.Value.Count));
                                List<DriveRate> rates = new List<DriveRate>();
                                DriveRate[] ratesArray = new DriveRate[trackingRatesResponse.Value.Count];
                                int i = 0;
                                foreach (DriveRate rate in trackingRatesResponse.Value)
                                {
                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Rate: {rate}");
                                    ratesArray[i] = rate;
                                    i++;
                                }

                                TrackingRates trackingRates = new TrackingRates();
                                trackingRates.SetRates(ratesArray);
                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Returning {trackingRates.Count} tracking rates to the client - now measured from trackingRates");
                                return (T)(object)trackingRates;
                            }
                            else // No TrackingRates object was returned so handle this as an error
                            {
                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, trackingRatesResponse.ClientTransactionID, trackingRatesResponse.ServerTransactionID, "NO VALUE OR NULL VALUE RETURNED"));

                                // Now force an error return
                                trackingRatesResponse = new TrackingRatesResponse
                                {
                                    ErrorNumber = (AlpacaErrors)DYNAMIC_DRIVER_ERROR_NUMBER,
                                    ErrorMessage = "Dynamic driver generated error: the Alpaca device returned no value or a null value for TrackingRates"
                                };
                            }
                            restResponseBase = (Response)trackingRatesResponse;
                        }
                        if (typeof(T) == typeof(EquatorialCoordinateType))
                        {
                            IntResponse equatorialCoordinateResponse = JsonSerializer.Deserialize<IntResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, equatorialCoordinateResponse.ClientTransactionID, equatorialCoordinateResponse.ServerTransactionID, equatorialCoordinateResponse.Value.ToString()));
                            return (T)(object)equatorialCoordinateResponse.Value;
                        }
                        if (typeof(T) == typeof(AlignmentMode))
                        {
                            IntResponse alignmentModesResponse = JsonSerializer.Deserialize<IntResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, alignmentModesResponse.ClientTransactionID, alignmentModesResponse.ServerTransactionID, alignmentModesResponse.Value.ToString()));
                            return (T)(object)alignmentModesResponse.Value;
                        }
                        if (typeof(T) == typeof(DriveRate))
                        {
                            IntResponse DriveRateResponse = JsonSerializer.Deserialize<IntResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, DriveRateResponse.ClientTransactionID, DriveRateResponse.ServerTransactionID, DriveRateResponse.Value.ToString()));
                            return (T)(object)DriveRateResponse.Value;
                        }
                        if (typeof(T) == typeof(SensorType))
                        {
                            IntResponse sensorTypeResponse = JsonSerializer.Deserialize<IntResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, sensorTypeResponse.ClientTransactionID, sensorTypeResponse.ServerTransactionID, sensorTypeResponse.Value.ToString()));
                            return (T)(object)sensorTypeResponse.Value;
                        }
                        if (typeof(T) == typeof(CameraState))
                        {
                            IntResponse cameraStatesResponse = JsonSerializer.Deserialize<IntResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, cameraStatesResponse.ClientTransactionID, cameraStatesResponse.ServerTransactionID, cameraStatesResponse.Value.ToString()));
                            return (T)(object)cameraStatesResponse.Value;
                        }
                        if (typeof(T) == typeof(ShutterState))
                        {
                            IntResponse domeShutterStateResponse = JsonSerializer.Deserialize<IntResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, domeShutterStateResponse.ClientTransactionID, domeShutterStateResponse.ServerTransactionID, domeShutterStateResponse.Value.ToString()));
                            return (T)(object)domeShutterStateResponse.Value;
                        }
                        if (typeof(T) == typeof(CoverStatus))
                        {
                            IntResponse coverStatusResponse = JsonSerializer.Deserialize<IntResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, coverStatusResponse.ClientTransactionID, coverStatusResponse.ServerTransactionID, coverStatusResponse.Value.ToString()));
                            return (T)(object)coverStatusResponse.Value;
                        }
                        if (typeof(T) == typeof(CalibratorStatus))
                        {
                            IntResponse calibratorStatusResponse = JsonSerializer.Deserialize<IntResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, calibratorStatusResponse.ClientTransactionID, calibratorStatusResponse.ServerTransactionID, calibratorStatusResponse.Value.ToString()));
                            return (T)(object)calibratorStatusResponse.Value;
                        }
                        if (typeof(T) == typeof(IAxisRates))
                        {
                            AxisRatesResponse axisRatesResponse = JsonSerializer.Deserialize<AxisRatesResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                            if (axisRatesResponse.Value != null) // A AxisRates object was returned so process the response normally
                            {
                                AxisRates axisRates = new AxisRates((TelescopeAxis)(Convert.ToInt32(parameters[AlpacaConstants.AXIS_PARAMETER_NAME])), logger);
                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, axisRatesResponse.ClientTransactionID.ToString(), axisRatesResponse.ServerTransactionID.ToString(), axisRatesResponse.Value.Count.ToString()));
                                foreach (AxisRate rr in axisRatesResponse.Value)
                                {
                                    axisRates.Add(rr.Minimum, rr.Maximum, logger);
                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Found rate: {rr.Minimum} - {rr.Maximum}");
                                }

                                return (T)(object)axisRates;
                            }
                            else // No AxisRates object was returned so handle this as an error
                            {
                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, axisRatesResponse.ClientTransactionID, axisRatesResponse.ServerTransactionID, "NO VALUE OR NULL VALUE RETURNED"));

                                // Now force an error return
                                axisRatesResponse = new AxisRatesResponse
                                {
                                    ErrorNumber = (AlpacaErrors)DYNAMIC_DRIVER_ERROR_NUMBER,
                                    ErrorMessage = "Dynamic driver generated error: the Alpaca device returned no value or a null value for AxisRates"
                                };
                            }

                            restResponseBase = (Response)axisRatesResponse;
                        }
                        if (typeof(T) == typeof(Array)) // Used for Camera.ImageArray and Camera.ImageArrayVariant
                        {
                            // Include some debug logging
                            foreach (var header in deviceResponse.Headers)
                            {
                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Response header {header.Key} = {header.Value}");
                            }

                            // Handle the ImageBytes image transfer mechanic (data is in the rawBytes array)
                            if (responseContentType.ToLowerInvariant().Contains(AlpacaConstants.IMAGE_BYTES_MIME_TYPE)) // Image bytes have been returned so the server supports raw array data transfer
                            {
                                sw.Stop();
                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageBytes", $"Downloaded {rawBytes.Length} bytes in {sw.ElapsedMilliseconds}ms.");

                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageBytes", $"Metadata bytes: " +
                                    $"[ {rawBytes[0]:X2} {rawBytes[1]:X2} {rawBytes[2]:X2} {rawBytes[3]:X2} ] [ {rawBytes[4]:X2} {rawBytes[5]:X2} {rawBytes[6]:X2} {rawBytes[7]:X2} ] " +
                                    $"[ {rawBytes[8]:X2} {rawBytes[9]:X2} {rawBytes[10]:X2} {rawBytes[11]:X2} ] [ {rawBytes[12]:X2} {rawBytes[13]:X2} {rawBytes[14]:X2} {rawBytes[15]:X2} ] " +
                                    $"[ {rawBytes[16]:X2} {rawBytes[17]:X2} {rawBytes[18]:X2} {rawBytes[19]:X2} ] [ {rawBytes[20]:X2} {rawBytes[21]:X2} {rawBytes[22]:X2} {rawBytes[23]:X2} ] " +
                                    $"[ {rawBytes[24]:X2} {rawBytes[25]:X2} {rawBytes[26]:X2} {rawBytes[27]:X2} ] [ {rawBytes[28]:X2} {rawBytes[29]:X2} {rawBytes[30]:X2} {rawBytes[31]:X2} ] " +
                                    $"[ {rawBytes[32]:X2} {rawBytes[33]:X2} {rawBytes[34]:X2} {rawBytes[35]:X2} ] [ {rawBytes[36]:X2} {rawBytes[37]:X2} {rawBytes[38]:X2} {rawBytes[39]:X2} ] " +
                                    $"[ {rawBytes[40]:X2} {rawBytes[41]:X2} {rawBytes[42]:X2} {rawBytes[43]:X2} ]");

                                int metadataVersion = rawBytes.GetMetadataVersion();
                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageBytes", $"Metadata version: {metadataVersion}");

                                AlpacaErrors errorNumber;
                                switch (metadataVersion)
                                {
                                    case 1:
                                        ArrayMetadataV1 metadataV1 = rawBytes.GetMetadataV1();
                                        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayBytes", $"Received array: Metadata version: {metadataV1.MetadataVersion}, " +
                                                                                                              $"Error number: {metadataV1.ErrorNumber}, " +
                                                                                                              $"Client transaction ID: {metadataV1.ClientTransactionID}, " +
                                                                                                              $"Server transaction ID: {metadataV1.ServerTransactionID}, " +
                                                                                                              $"Image element type: {metadataV1.ImageElementType}, " +
                                                                                                              $"Transmission element type: {metadataV1.TransmissionElementType}, " +
                                                                                                              $"Array rank: {metadataV1.Rank}, " +
                                                                                                              $"Dimension 1: {metadataV1.Dimension1}, " +
                                                                                                              $"Dimension 2: {metadataV1.Dimension2}, " +
                                                                                                              $"Dimension 3: {metadataV1.Dimension3}.");

                                        errorNumber = metadataV1.ErrorNumber;
                                        break;

                                    default:
                                        throw new InvalidValueException($"ImageArray - ImageArrayBytes - Received an unsupported metadata version number: {metadataVersion} from the Alpaca device.");
                                }

                                // Convert the byte[] back to an image array
                                sw.Restart();
                                Array returnArray = rawBytes.ToImageArray();
                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageBytes", $"Converted byte[] to image array in {sw.ElapsedMilliseconds}ms. Overall time: {swOverall.ElapsedMilliseconds}ms.");
                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "", $"");

                                return (T)(Object)returnArray;

                                // No need for error handling here because any error will have been returned as a JSON response rather than as this ImageBytes response.
                            }

                            // Handle the base64 hand-off image transfer mechanic
                            else if (deviceResponse.Headers.Any(t => t.Key.ToString() == AlpacaConstants.BASE64_HANDOFF_HEADER)) // Base64 format header is present so the server supports base64 serialised transfer
                            {
                                // De-serialise the JSON image array hand-off response 
                                sw.Restart(); // Clear and start the stopwatch
                                Base64ArrayHandOffResponse base64HandOffresponse = JsonSerializer.Deserialize<Base64ArrayHandOffResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                                ImageArrayElementTypes arrayType = (ImageArrayElementTypes)base64HandOffresponse.Type; // Extract the array type from the JSON response

                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Base64 - Extracted array information in {sw.ElapsedMilliseconds}ms. Array Type: {arrayType}, Rank: {base64HandOffresponse.Rank}, Dimension 0 length: {base64HandOffresponse.Dimension0Length}, Dimension 1 length: {base64HandOffresponse.Dimension1Length}, Dimension 2 length: {base64HandOffresponse.Dimension2Length}");
                                sw.Restart();

                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Base64 - Downloading base64 serialised image");

                                // Construct an HTTP request to get the base 64 encoded image
                                string base64Uri = (client.BaseAddress + uriBase.TrimStart('/') + method.ToLowerInvariant() + AlpacaConstants.BASE64_HANDOFF_FILE_DOWNLOAD_URI_EXTENSION).ToLowerInvariant(); // Create the download URI from the REST client elements
                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Base64 URI: {base64Uri}");

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
                                        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Downloaded base64 stream obtained in {sw.ElapsedMilliseconds}ms"); sw.Restart();

                                        // Read the stream contents into the string variable ready for further processing
                                        using (StreamReader sr = new StreamReader(base64ArrayStream, System.Text.Encoding.ASCII, false))
                                        {
                                            base64ArrayString = sr.ReadToEnd();
                                        }
                                    }
                                }

                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Read base64 string from stream ({base64ArrayString.Length} bytes) in {sw.ElapsedMilliseconds}ms"); sw.Restart();
                                try { AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Base64 string start: {base64ArrayString.Substring(0, 300)}"); } catch { }
                                try { AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Base64 string end: {base64ArrayString.Substring(60000000, 300)}"); } catch { }

                                // Convert the array from base64 encoding to a byte array
                                byte[] base64ArrayByteArray = Convert.FromBase64String(base64ArrayString);
                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Converted base64 string of length {base64ArrayString.Length} to byte array of length {base64ArrayByteArray.Length} in {sw.ElapsedMilliseconds}ms"); sw.Restart();
                                string byteLine = "";
                                try
                                {
                                    for (int i = 0; i < 300; i++)
                                    {
                                        byteLine += base64ArrayByteArray[i].ToString() + " ";
                                    }
                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Converted base64 bytes: {byteLine}");
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

                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Created and copied the array in {sw.ElapsedMilliseconds}ms"); sw.Restart();

                                return (T)(object)remoteArray;
                            }

                            // Handle a conventional JSON response with integer array elements individually serialised
                            else
                            {
                                sw.Restart(); // Clear and start the stopwatch
                                ImageArrayResponseBase responseBase = JsonSerializer.Deserialize<ImageArrayResponseBase>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                                ImageArrayElementTypes arrayType = (ImageArrayElementTypes)responseBase.Type;
                                int arrayRank = responseBase.Rank;

                                // Include some debug logging
                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Extracted array type and rank by JsonSerializer.Deserialize in {sw.ElapsedMilliseconds}ms. Type: {arrayType}, Rank: {arrayRank}, Response values - Type: {responseBase.Type}, Rank: {responseBase.Rank}");

                                sw.Restart(); // Clear and start the stopwatch
                                switch (arrayType) // Handle the different return types that may come from ImageArrayVariant
                                {
                                    case ImageArrayElementTypes.Int32:
                                        switch (arrayRank)
                                        {
                                            case 2:
                                                IntJaggedArray2DResponse intArray2DResponse = JsonSerializer.Deserialize<IntJaggedArray2DResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, intArray2DResponse.ClientTransactionID, intArray2DResponse.ServerTransactionID, intArray2DResponse.Rank.ToString())); //, intArray2DResponse.Method));

                                                // Get the array dimensions
                                                int dimension0Length = intArray2DResponse.Value.GetLength(0);
                                                int dimension1Length = intArray2DResponse.Value[0].GetLength(0);

                                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Array was de serialised in {sw.ElapsedMilliseconds} ms, Type: {(ImageArrayElementTypes)intArray2DResponse.Type}, Rank: {intArray2DResponse.Rank}, Dimension 1 Length: {dimension0Length}, Dimension 2 Length: {dimension1Length}");

                                                sw.Restart();
                                                int[,] intArray2D = new int[dimension0Length, dimension1Length];

                                                // Calculate the number of bytes in dimension 1
                                                int bytesPerDimension1 = dimension1Length * sizeof(int);

                                                // Convert the jagged array into a rectangular array
                                                Parallel.For(0, dimension0Length, i =>
                                                {
                                                    Buffer.BlockCopy(intArray2DResponse.Value[i], 0, intArray2D, i * bytesPerDimension1, bytesPerDimension1);
                                                });
                                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Array was coped in {sw.ElapsedMilliseconds} ms");

                                                return (T)(object)intArray2D;

                                            case 3:
                                                IntJaggedArray3DResponse intArray3DResponse = JsonSerializer.Deserialize<IntJaggedArray3DResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, intArray3DResponse.ClientTransactionID, intArray3DResponse.ServerTransactionID, intArray3DResponse.Rank.ToString())); //, intArray3DResponse.Method));
                                                                                                                                                                                                                                                                      // Get the array dimensions
                                                dimension0Length = intArray3DResponse.Value.GetLength(0);
                                                dimension1Length = intArray3DResponse.Value[0].GetLength(0);
                                                int dimension2Length = intArray3DResponse.Value[0][0].GetLength(0);

                                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Array was de serialised in {sw.ElapsedMilliseconds} ms, Type: {(ImageArrayElementTypes)intArray3DResponse.Type}, Rank: {intArray3DResponse.Rank}, Dimension 1 Length: {dimension0Length}, Dimension 2 Length: {dimension1Length}, Dimension 3 Length: {dimension2Length}");

                                                sw.Restart();
                                                int[,,] intArray3D = new int[dimension0Length, dimension1Length, dimension2Length];

                                                // Calculate the number of bytes in dimension 1
                                                int bytesPerDimension2 = dimension2Length * sizeof(int);

                                                // Convert the jagged array into a rectangular array
                                                Parallel.For(0, dimension0Length, i =>
                                                {
                                                    for (int j = 0; j < dimension1Length; j++)
                                                    {
                                                        Buffer.BlockCopy(intArray3DResponse.Value[i][j], 0, intArray3D, i * bytesPerDimension2, bytesPerDimension2);
                                                    }
                                                });
                                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Array was coped in {sw.ElapsedMilliseconds} ms");

                                                return (T)(object)intArray3D;

                                            default:
                                                throw new InvalidOperationException("Arrays of Rank " + arrayRank + " are not supported.");
                                        }

                                    case ImageArrayElementTypes.Int16:
                                        switch (arrayRank)
                                        {
                                            case 2:
                                                ShortJaggedArray2DResponse shortArray2DResponse = JsonSerializer.Deserialize<ShortJaggedArray2DResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, shortArray2DResponse.ClientTransactionID, shortArray2DResponse.ServerTransactionID, shortArray2DResponse.Rank.ToString())); //, shortArray2DResponse.Method));

                                                // Get the array dimensions
                                                int dimension0Length = shortArray2DResponse.Value.GetLength(0);
                                                int dimension1Length = shortArray2DResponse.Value[0].GetLength(0);

                                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Array was de serialised in {sw.ElapsedMilliseconds} ms, Type: {(ImageArrayElementTypes)shortArray2DResponse.Type}, Rank: {shortArray2DResponse.Rank}, Dimension 1 Length: {dimension0Length}, Dimension 2 Length: {dimension1Length}");

                                                sw.Restart();
                                                short[,] shortArray2D = new short[dimension0Length, dimension1Length];

                                                // Calculate the number of bytes in dimension 1
                                                int bytesPerDimension1 = dimension1Length * sizeof(short);

                                                // Convert the jagged array into a rectangular array
                                                Parallel.For(0, dimension0Length, i =>
                                                {
                                                    Buffer.BlockCopy(shortArray2DResponse.Value[i], 0, shortArray2D, i * bytesPerDimension1, bytesPerDimension1);
                                                });

                                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Array was coped in {sw.ElapsedMilliseconds} ms");

                                                return (T)(object)shortArray2D;

                                            case 3:
                                                ShortJaggedArray3DResponse shortArray3DResponse = JsonSerializer.Deserialize<ShortJaggedArray3DResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, shortArray3DResponse.ClientTransactionID, shortArray3DResponse.ServerTransactionID, shortArray3DResponse.Rank.ToString())); //, shortArray3DResponse.Method));

                                                dimension0Length = shortArray3DResponse.Value.GetLength(0);
                                                dimension1Length = shortArray3DResponse.Value[0].GetLength(0);
                                                int dimension2Length = shortArray3DResponse.Value[0][0].GetLength(0);

                                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Array was de-serialised in {sw.ElapsedMilliseconds} ms, Type: {(ImageArrayElementTypes)shortArray3DResponse.Type}, Rank: {shortArray3DResponse.Rank}, Dimension 1 Length: {dimension0Length}, Dimension 2 Length: {dimension1Length}, Dimension 3 Length: {dimension2Length}");

                                                sw.Restart();
                                                short[,,] shortArray3D = new short[dimension0Length, dimension1Length, dimension2Length];

                                                // Calculate the number of bytes in dimension 1
                                                int bytesPerDimension2 = dimension2Length * sizeof(short);

                                                // Convert the jagged array into a rectangular array
                                                Parallel.For(0, dimension0Length, i =>
                                                {
                                                    for (int j = 0; j < dimension1Length; j++)
                                                    {
                                                        Buffer.BlockCopy(shortArray3DResponse.Value[i][j], 0, shortArray3D, i * bytesPerDimension2, bytesPerDimension2);
                                                    }
                                                });
                                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Array was coped in {sw.ElapsedMilliseconds} ms");

                                                return (T)(object)shortArray3D;

                                            default:
                                                throw new InvalidOperationException("Arrays of Rank " + arrayRank + " are not supported.");
                                        }

                                    case ImageArrayElementTypes.Double:
                                        switch (arrayRank)
                                        {
                                            case 2:
                                                DoubleJaggedArray2DResponse doubleArray2DResponse = JsonSerializer.Deserialize<DoubleJaggedArray2DResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, doubleArray2DResponse.ClientTransactionID, doubleArray2DResponse.ServerTransactionID, doubleArray2DResponse.Rank.ToString())); //, doubleArray2DResponse.Method));

                                                // Get the array dimensions
                                                int dimension0Length = doubleArray2DResponse.Value.GetLength(0);
                                                int dimension1Length = doubleArray2DResponse.Value[0].GetLength(0);

                                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Array was de-serialised in {sw.ElapsedMilliseconds} ms, Type: {(ImageArrayElementTypes)doubleArray2DResponse.Type}, Rank: {doubleArray2DResponse.Rank}, Dimension 1 Length: {dimension0Length}, Dimension 2 Length: {dimension1Length}");

                                                sw.Restart();
                                                double[,] doubleArray2D = new double[dimension0Length, dimension1Length];

                                                // Calculate the number of bytes in dimension 1
                                                int bytesPerDimension1 = dimension1Length * sizeof(double);

                                                // Convert the jagged array into a rectangular array
                                                Parallel.For(0, dimension0Length, i =>
                                                {
                                                    Buffer.BlockCopy(doubleArray2DResponse.Value[i], 0, doubleArray2D, i * bytesPerDimension1, bytesPerDimension1);
                                                });

                                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Array was coped in {sw.ElapsedMilliseconds} ms");

                                                return (T)(object)doubleArray2D;

                                            case 3:
                                                DoubleJaggedArray3DResponse doubleArray3DResponse = JsonSerializer.Deserialize<DoubleJaggedArray3DResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = !strictCasing });
                                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, string.Format(LOG_FORMAT_STRING, doubleArray3DResponse.ClientTransactionID, doubleArray3DResponse.ServerTransactionID, doubleArray3DResponse.Rank.ToString())); //, doubleArray3DResponse.Method));

                                                dimension0Length = doubleArray3DResponse.Value.GetLength(0);
                                                dimension1Length = doubleArray3DResponse.Value[0].GetLength(0);
                                                int dimension2Length = doubleArray3DResponse.Value[0][0].GetLength(0);

                                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Array was de-serialised in {sw.ElapsedMilliseconds} ms, Type: {(ImageArrayElementTypes)doubleArray3DResponse.Type}, Rank: {doubleArray3DResponse.Rank}, Dimension 1 Length: {dimension0Length}, Dimension 2 Length: {dimension1Length}, Dimension 3 Length: {dimension2Length}");

                                                sw.Restart();
                                                double[,,] doubleArray3D = new double[dimension0Length, dimension1Length, dimension2Length];

                                                // Calculate the number of bytes in dimension 1
                                                int bytesPerDimension2 = dimension2Length * sizeof(double);

                                                // Convert the jagged array into a rectangular array
                                                Parallel.For(0, dimension0Length, i =>
                                                {
                                                    for (int j = 0; j < dimension1Length; j++)
                                                    {
                                                        Buffer.BlockCopy(doubleArray3DResponse.Value[i][j], 0, doubleArray3D, i * bytesPerDimension2, bytesPerDimension2);
                                                    }
                                                });
                                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Array was coped in {sw.ElapsedMilliseconds} ms");

                                                return (T)(object)doubleArray3D;

                                            default:
                                                throw new InvalidOperationException("Arrays of Rank " + arrayRank + " are not supported.");
                                        }

                                    default:
                                        throw new InvalidOperationException($"SendToRemoteDevice JSON - Image array element type {arrayType} is not supported. The device returned this value: {responseBase.Type}");
                                }
                            } // remote device has used JSON encoding
                        }

                        // Internal error if an unsupported type is requested - should only occur during development and not in production operation!
                        throw new InvalidOperationException("Type " + typeof(T).ToString() + " is not supported. You should never see this message, if you do, please report it on the ASCOM Talk Forum!");
                    }
                    else // ERROR - HTTP Status code is not in the range 200::299...
                    {
                        // Extract any error message text returned in the body of the response
                        string errorMessage = deviceResponse.Content.ReadAsStringAsync().Result;

                        // Log the error
                        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method + " Error", $"RestRequest response status: {deviceResponse.ReasonPhrase}, HTTP response code: {deviceResponse.StatusCode}, Error message: {errorMessage}");

                        // Throw an exception back to the client describing the error
                        throw new DriverException($"Error calling method: {method}, HTTP Completion Status: {deviceResponse.StatusCode}, Error Message:\r\n{errorMessage}");
                    }
                }
                catch (Exception ex) // Process unexpected exceptions
                {
                    if (ex is AggregateException aggregateException) // Received a WebException, this could indicate that the remote device actively refused the connection so test for this and retry if appropriate
                    {
                        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"{typeof(T).Name} - AggregateException - Number of exceptions: {aggregateException.InnerExceptions.Count}");
                        foreach (Exception ex1 in aggregateException.InnerExceptions)
                        {
                            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"{typeof(T).Name} - AggregateException - Found exception: {ex1.GetType().Name}, Message: {ex1.Message}");

                            if (ex1 is TaskCanceledException) // Handle communications timeout exceptions using retries
                            {
                                retryCounter += 1; // Increment the retry counter
                                if (retryCounter <= SOCKET_ERROR_MAXIMUM_RETRIES) // The retry count is less than or equal to the maximum allowed so retry the command
                                {
                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, typeof(T).Name + " " + ex1.Message);
                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, "Timeout exception: " + ex1.ToString());

                                    // Log that we are retrying the command and wait a short time in the hope that the transient condition clears
                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Timeout exception - retry-count {retryCounter}/{SOCKET_ERROR_MAXIMUM_RETRIES}");
                                    Thread.Sleep(SOCKET_ERROR_RETRY_DELAY_TIME);
                                }
                                else // The retry count exceeds the maximum allowed so throw the exception to the client
                                {
                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, typeof(T).Name + " " + ex1.Message);
                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, "Timeout exception: " + ex1.ToString());
                                    throw new TimeoutException($"Dynamic client timeout for method {typeof(T).Name}: {client.BaseAddress}");
                                }
                            }
                            else if (ex1 is HttpRequestException) // A communications error of some kind
                            {
                                if (ex1.InnerException is SocketException) // There is an inner exception and it is a SocketException so apply the retry logic
                                {
                                    retryCounter += 1; // Increment the retry counter
                                    if (retryCounter <= SOCKET_ERROR_MAXIMUM_RETRIES) // The retry count is less than or equal to the maximum allowed so retry the command
                                    {
                                        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, typeof(T).Name + " " + ex1.Message);
                                        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, "SocketException: " + ex1.ToString());

                                        // Log that we are retrying the command and wait a short time in the hope that the transient condition clears
                                        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, $"Socket exception, retrying command - retry-count {retryCounter}/{SOCKET_ERROR_MAXIMUM_RETRIES}");
                                        Thread.Sleep(SOCKET_ERROR_RETRY_DELAY_TIME);
                                    }
                                    else // The retry count exceeds the maximum allowed so throw the exception to the client
                                    {
                                        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, typeof(T).Name + " " + ex1.Message);
                                        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, "SocketException: " + ex1.ToString());
                                        throw ex1.InnerException;
                                    }
                                }
                                else  // There is an inner exception but it is not a SocketException so log it and throw it  to the client
                                {
                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, typeof(T).Name + " " + ex1.Message);
                                    AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, "WebException: " + ex1.ToString());
                                    throw ex1.InnerException;
                                }
                            }
                            else
                            {
                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, typeof(T).Name + " " + ex1.Message);
                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, "WebException: " + ex1.ToString());
                                throw ex1.InnerException;
                            }
                        }
                    }
                    else // Some other type of exception that isn't an AggregateException so log it and throw it to the client
                    {
                        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, typeof(T).Name + " " + ex.Message);
                        AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, method, "Exception: " + ex.ToString());
                        throw;
                    }
                }
            } while (true); // Execution will only reach here if a communications retry is required, all other conditions are handled by return statements or by throwing exceptions

            // Execution will never reach this point
        }

        #endregion

        #region ASCOM Common members

        internal static string Action(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger, string actionName, string actionParameters)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.ACTION_COMMAND_PARAMETER_NAME, actionName },
                { AlpacaConstants.ACTION_PARAMETERS_PARAMETER_NAME, actionParameters }
            };
            string remoteString = SendToRemoteDevice<string>(clientNumber, client, timeout, URIBase, strictCasing, logger, "Action", Parameters, HttpMethod.Put, MemberTypes.Method);
            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "Action", $"Response length: {remoteString.Length}");
            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "Action", $"Response: {((remoteString.Length <= 100) ? remoteString : remoteString.Substring(0, 100))}");
            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "Action", $"Returning response length: {remoteString.Length}");
            return remoteString;
        }

        internal static void CommandBlind(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger, string command, bool raw)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.COMMAND_PARAMETER_NAME, command },
                { AlpacaConstants.RAW_PARAMETER_NAME, raw.ToString() }
            };
            SendToRemoteDevice<NoReturnValue>(clientNumber, client, timeout, URIBase, strictCasing, logger, "CommandBlind", Parameters, HttpMethod.Put, MemberTypes.Method);
            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "CommandBlind", "Completed OK");
        }

        internal static bool CommandBool(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger, string command, bool raw)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.COMMAND_PARAMETER_NAME, command },
                { AlpacaConstants.RAW_PARAMETER_NAME, raw.ToString() }
            };
            bool remoteBool = SendToRemoteDevice<bool>(clientNumber, client, timeout, URIBase, strictCasing, logger, "CommandBool", Parameters, HttpMethod.Put, MemberTypes.Method);

            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "CommandBool", remoteBool.ToString());
            return remoteBool;
        }

        internal static string CommandString(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger, string command, bool raw)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.COMMAND_PARAMETER_NAME, command },
                { AlpacaConstants.RAW_PARAMETER_NAME, raw.ToString() }
            };
            string remoteString = SendToRemoteDevice<string>(clientNumber, client, timeout, URIBase, strictCasing, logger, "CommandString", Parameters, HttpMethod.Put, MemberTypes.Method);

            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "CommandString", remoteString);
            return remoteString;
        }

        internal static string Description(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger)
        {
            return GetValue<string>(clientNumber, client, timeout, URIBase, strictCasing, logger, "Description", MemberTypes.Property);
        }

        internal static string DriverInfo(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger)
        {
            return GetValue<string>(clientNumber, client, timeout, URIBase, strictCasing, logger, "DriverInfo", MemberTypes.Property);
        }

        internal static string DriverVersion(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger)
        {
            string remoteString = GetValue<string>(clientNumber, client, timeout, URIBase, strictCasing, logger, "DriverVersion", MemberTypes.Property);
            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "DriverVersion", remoteString);
            return remoteString;
        }

        internal static short InterfaceVersion(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger)
        {
            short interfaceVersion = GetValue<short>(clientNumber, client, timeout, URIBase, strictCasing, logger, "InterfaceVersion", MemberTypes.Property);
            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "InterfaceVersion", interfaceVersion.ToString());
            return interfaceVersion;
        }

        internal static IList<string> SupportedActions(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger)
        {
            List<string> supportedActions = GetValue<List<string>>(clientNumber, client, timeout, URIBase, strictCasing, logger, "SupportedActions", MemberTypes.Property);
            AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "SupportedActions", $"Returning {supportedActions.Count} actions");

            List<string> returnValues = new List<string>();
            foreach (string action in supportedActions)
            {
                returnValues.Add(action);
                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "SupportedActions", $"Returning action: {action}");
            }

            return returnValues;
        }

        #endregion

        #region Complex Camera Properties

        internal static object ImageArrayVariant(uint clientNumber, HttpClient client, int timeout, string URIBase, bool strictCasing, ILogger logger, ImageArrayTransferType imageArrayTransferType, ImageArrayCompression imageArrayCompression)
        {
            Array returnArray;
            object[,] objectArray2D;
            object[,,] objectArray3D;
            Stopwatch sw = new Stopwatch();

            returnArray = GetValue<Array>(clientNumber, client, timeout, URIBase, strictCasing, logger, "ImageArrayVariant", imageArrayTransferType, imageArrayCompression, MemberTypes.Property);

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

                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Received {variantType} array of Rank {returnArray.Rank} with Length {returnArray.Length} and element type {elementType}");

                // convert to variant

                switch (returnArray.Rank)
                {
                    case 2:
                        objectArray2D = new object[returnArray.GetLength(0), returnArray.GetLength(1)];
                        switch (variantType)
                        {
                            case "Byte[,]":
                                Byte[,] byte2DArray = (Byte[,])returnArray;

                                sw.Restart();
                                Parallel.For(0, returnArray.GetLength(0), (i) =>
                                {
                                    for (int j = 0; j < returnArray.GetLength(1); j++)
                                    {
                                        objectArray2D[i, j] = byte2DArray[i, j];
                                    }
                                });

                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying Byte[,] array in {sw.ElapsedMilliseconds}ms.");
                                return objectArray2D;

                            case "Int16[,]":
                                Int16[,] short2DArray = (Int16[,])returnArray;

                                sw.Restart();
                                Parallel.For(0, returnArray.GetLength(0), (i) =>
                                {
                                    for (int j = 0; j < returnArray.GetLength(1); j++)
                                    {
                                        objectArray2D[i, j] = short2DArray[i, j];
                                    }
                                });

                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying Int16[,] array in {sw.ElapsedMilliseconds}ms.");
                                return objectArray2D;

                            case "UInt16[,]":
                                UInt16[,] uint16Array2D = (UInt16[,])returnArray;

                                sw.Restart();
                                Parallel.For(0, returnArray.GetLength(0), (i) =>
                                {
                                    for (int j = 0; j < returnArray.GetLength(1); j++)
                                    {
                                        objectArray2D[i, j] = uint16Array2D[i, j];
                                    }
                                });

                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying UInt16[,] array in {sw.ElapsedMilliseconds}ms.");
                                return objectArray2D;

                            case "Int32[,]":
                                Int32[,] int2DArray = (Int32[,])returnArray;

                                sw.Restart();
                                Parallel.For(0, returnArray.GetLength(0), (i) =>
                                {
                                    for (int j = 0; j < returnArray.GetLength(1); j++)
                                    {
                                        objectArray2D[i, j] = int2DArray[i, j];
                                    }
                                });

                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying Int32[,] array in {sw.ElapsedMilliseconds}ms.");
                                return objectArray2D;

                            case "UInt32[,]":
                                UInt32[,] uint2DArray = (UInt32[,])returnArray;

                                sw.Restart();
                                Parallel.For(0, returnArray.GetLength(0), (i) =>
                                {
                                    for (int j = 0; j < returnArray.GetLength(1); j++)
                                    {
                                        objectArray2D[i, j] = uint2DArray[i, j];
                                    }
                                });

                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying UInt32[,] array in {sw.ElapsedMilliseconds}ms.");
                                return objectArray2D;

                            case "Int64[,]":
                                Int64[,] int64Array2D = (Int64[,])returnArray;

                                sw.Restart();
                                Parallel.For(0, returnArray.GetLength(0), (i) =>
                                {
                                    for (int j = 0; j < returnArray.GetLength(1); j++)
                                    {
                                        objectArray2D[i, j] = int64Array2D[i, j];
                                    }
                                });

                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying Int64[,] array in {sw.ElapsedMilliseconds}ms.");
                                return objectArray2D;

                            case "UInt64[,]":
                                UInt64[,] uint64Array2D = (UInt64[,])returnArray;

                                sw.Restart();
                                Parallel.For(0, returnArray.GetLength(0), (i) =>
                                {
                                    for (int j = 0; j < returnArray.GetLength(1); j++)
                                    {
                                        objectArray2D[i, j] = uint64Array2D[i, j];
                                    }
                                });

                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying UInt64[,] array in {sw.ElapsedMilliseconds}ms.");
                                return objectArray2D;

                            case "Single[,]":
                                Single[,] single2DArray = (Single[,])returnArray;

                                sw.Restart();
                                Parallel.For(0, returnArray.GetLength(0), (i) =>
                                {
                                    for (int j = 0; j < returnArray.GetLength(1); j++)
                                    {
                                        objectArray2D[i, j] = single2DArray[i, j];
                                    }
                                });

                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying Single[,] array in {sw.ElapsedMilliseconds}ms.");
                                return objectArray2D;

                            case "Double[,]":
                                Double[,] double2DArray = (Double[,])returnArray;

                                sw.Restart();
                                Parallel.For(0, returnArray.GetLength(0), (i) =>
                                {
                                    for (int j = 0; j < returnArray.GetLength(1); j++)
                                    {
                                        objectArray2D[i, j] = double2DArray[i, j];
                                    }
                                });

                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying Double[,] array in {sw.ElapsedMilliseconds}ms.");
                                return objectArray2D;

                            case "Object[,]":
                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Received an Object[,] array, returning it directly to the client without further processing.");
                                return returnArray;

                            default:
                                throw new InvalidValueException("DynamicRemoteClient Driver Camera.ImageArrayVariant: Unsupported return array rank from DynamicClientDriver.GetValue<Array>: " + returnArray.Rank);
                        }
                    case 3:
                        objectArray3D = new object[returnArray.GetLength(0), returnArray.GetLength(1), returnArray.GetLength(2)];
                        switch (variantType)
                        {
                            case "Byte[,,]":
                                Byte[,,] byte3DArray = (Byte[,,])returnArray;

                                sw.Restart();
                                Parallel.For(0, returnArray.GetLength(0), (i) =>
                                {
                                    for (int j = 0; j < returnArray.GetLength(1); j++)
                                    {
                                        for (int k = 0; k < returnArray.GetLength(2); k++)
                                            objectArray3D[i, j, k] = byte3DArray[i, j, k];
                                    }
                                });

                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying Byte[,,] array in {sw.ElapsedMilliseconds}ms.");
                                return objectArray3D;

                            case "Int16[,,]":
                                Int16[,,] short3DArray = (Int16[,,])returnArray;

                                sw.Restart();
                                Parallel.For(0, returnArray.GetLength(0), (i) =>
                                {
                                    for (int j = 0; j < returnArray.GetLength(1); j++)
                                    {
                                        for (int k = 0; k < returnArray.GetLength(2); k++)
                                            objectArray3D[i, j, k] = short3DArray[i, j, k];
                                    }
                                });

                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying Int16[,,] array in {sw.ElapsedMilliseconds}ms.");
                                return objectArray3D;

                            case "UInt16[,,]":
                                UInt16[,,] uint16Array3D = (UInt16[,,])returnArray;

                                sw.Restart();
                                Parallel.For(0, returnArray.GetLength(0), (i) =>
                                {
                                    for (int j = 0; j < returnArray.GetLength(1); j++)
                                    {
                                        for (int k = 0; k < returnArray.GetLength(2); k++)
                                            objectArray3D[i, j, k] = uint16Array3D[i, j, k];
                                    }
                                });

                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying UInt16[,,] array in {sw.ElapsedMilliseconds}ms.");
                                return objectArray3D;

                            case "Int32[,,]":
                                Int32[,,] int3DArray = (Int32[,,])returnArray;

                                sw.Restart();
                                Parallel.For(0, returnArray.GetLength(0), (i) =>
                                {
                                    for (int j = 0; j < returnArray.GetLength(1); j++)
                                    {
                                        for (int k = 0; k < returnArray.GetLength(2); k++)
                                        {
                                            objectArray3D[i, j, k] = int3DArray[i, j, k];
                                        }
                                    }
                                });

                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying Int32[,,] array in {sw.ElapsedMilliseconds}ms.");
                                return objectArray3D;

                            case "UInt32[,,]":
                                UInt32[,,] uint32Array3D = (UInt32[,,])returnArray;

                                sw.Restart();
                                Parallel.For(0, returnArray.GetLength(0), (i) =>
                                {
                                    for (int j = 0; j < returnArray.GetLength(1); j++)
                                    {
                                        for (int k = 0; k < returnArray.GetLength(2); k++)
                                        {
                                            objectArray3D[i, j, k] = uint32Array3D[i, j, k];
                                        }
                                    }
                                });

                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying UInt32[,,] array in {sw.ElapsedMilliseconds}ms.");
                                return objectArray3D;

                            case "Int64[,,]":
                                Int64[,,] int64Array3D = (Int64[,,])returnArray;

                                sw.Restart();
                                Parallel.For(0, returnArray.GetLength(0), (i) =>
                                {
                                    for (int j = 0; j < returnArray.GetLength(1); j++)
                                    {
                                        for (int k = 0; k < returnArray.GetLength(2); k++)
                                        {
                                            objectArray3D[i, j, k] = int64Array3D[i, j, k];
                                        }
                                    }
                                });

                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying Int64[,,] array in {sw.ElapsedMilliseconds}ms.");
                                return objectArray3D;

                            case "UInt64[,,]":
                                UInt64[,,] uint64Array3D = (UInt64[,,])returnArray;

                                sw.Restart();
                                Parallel.For(0, returnArray.GetLength(0), (i) =>
                                {
                                    for (int j = 0; j < returnArray.GetLength(1); j++)
                                    {
                                        for (int k = 0; k < returnArray.GetLength(2); k++)
                                        {
                                            objectArray3D[i, j, k] = uint64Array3D[i, j, k];
                                        }
                                    }
                                });

                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying UInt64[,,] array in {sw.ElapsedMilliseconds}ms.");
                                return objectArray3D;

                            case "Single[,,]":
                                Single[,,] single3DDArray = (Single[,,])returnArray;

                                sw.Restart();
                                Parallel.For(0, returnArray.GetLength(0), (i) =>
                                {
                                    for (int j = 0; j < returnArray.GetLength(1); j++)
                                    {
                                        for (int k = 0; k < returnArray.GetLength(2); k++)
                                            objectArray3D[i, j, k] = single3DDArray[i, j, k];
                                    }
                                });

                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying Single[,,] array in {sw.ElapsedMilliseconds}ms.");
                                return objectArray3D;

                            case "Double[,,]":
                                Double[,,] double3DDArray = (Double[,,])returnArray;

                                sw.Restart();
                                Parallel.For(0, returnArray.GetLength(0), (i) =>
                                {
                                    for (int j = 0; j < returnArray.GetLength(1); j++)
                                    {
                                        for (int k = 0; k < returnArray.GetLength(2); k++)
                                            objectArray3D[i, j, k] = double3DDArray[i, j, k];
                                    }
                                });

                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Finished copying Double[,,] array in {sw.ElapsedMilliseconds}ms.");
                                return objectArray3D;

                            case "Object[,,]":
                                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Received an Object[,,] array, returning it directly to the client without further processing.");
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
                AlpacaDeviceBaseClass.LogMessage(logger, clientNumber, "ImageArrayVariant", $"Exception: \r\n{ex}");
                throw;
            }
        }

        #endregion

    }
}
