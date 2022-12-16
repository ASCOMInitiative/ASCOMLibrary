using ASCOM.Alpaca.Discovery;
using ASCOM.Common;
using ASCOM.Common.Alpaca;
using ASCOM.Common.Interfaces;
using System;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// Helper functions to return an instance of an Alpaca client
    /// </summary>
    public static class AlpacaClient
    {
        #region Constants

        internal const int CLIENT_ESTABLISHCONNECTIONTIMEOUT_DEFAULT = 5; // Default establish connection timeout
        internal const int CLIENT_STANDARDCONNECTIONTIMEOUT_DEFAULT = 10; // Default standard command timeout
        internal const int CLIENT_LONGCONNECTIONTIMEOUT_DEFAULT = 100; // Default long command connection timeout
        internal const uint CLIENT_CLIENTNUMBER_DEFAULT = 1; // Default client number
        internal const string CLIENT_USERNAME_DEFAULT = ""; // Default basic authorisation user name
        internal const string CLIENT_PASSWORD_DEFAULT = ""; // Default basic authorisation password
        internal const bool CLIENT_STRICTCASING_DEFAULT = true; // Default for strict casing or flexible interpretation of casing in device JSON responses
        internal const ILogger CLIENT_LOGGER_DEFAULT = null; // Default to no operational logging
        internal const ServiceType CLIENT_SERVICETYPE_DEFAULT = ServiceType.Http; // Default for HTTP/HTTP service type
        internal const string CLIENT_IPADDRESS_DEFAULT = "127.0.0.1"; // Default Alpaca device IP address
        internal const int CLIENT_IPPORT_DEFAULT = 11111; // Default Alpaca device IP port
        internal const int CLIENT_REMOTEDEVICENUMBER_DEFAULT = 0; // Default device number in the URI on the remote Alpaca device
        internal const ImageArrayTransferType CLIENT_IMAGEARRAYTRANSFERTYPE_DEFAULT = ImageArrayTransferType.BestAvailable; // Default camera image array transfer type
        internal const ImageArrayCompression CLIENT_IMAGEARRAYCOMPRESSION_DEFAULT = ImageArrayCompression.None; // Default camera image array compression type
        internal const string CLIENT_USER_AGENT_PRODUCT_NAME = "ASCOMAlpacaClient";

        #endregion

        #region Public static methods

        /// <summary>
        /// Create an Alpaca client for a discovered ASCOM device specifying all parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ascomDevice">An AscomDevice instance representing the device to use (mandatory parameter)</param>
        /// <param name="establishConnectionTimeout">Communications timeout (seconds) when initially connecting to the client. Default: 3 seconds</param>
        /// <param name="standardDeviceResponseTimeout">Communications timeout (seconds) for commands that are expected to complete quickly. Default: 3 seconds</param>
        /// <param name="longDeviceResponseTimeout">Communications timeout (seconds) for commands that are expected to take a long time such as synchronous telescope slews. Default: 100 seconds</param>
        /// <param name="clientNumber">Arbitrary number identifying this particular client. Default: 1</param>
        /// <param name="userName">Basic authentication user name. Supply a <see langword="null"/> or empty string if basic authentication is not required. Default: Empty string</param>
        /// <param name="password"> Basic authentication password. Ignored if password is <see langword="null"/> or an empty string. Default: Empty string</param>
        /// <param name="strictCasing">Set true to require strict JSON casing in device responses.. Default: true</param>
        /// <param name="logger">ILogger instance or null. Default: null</param>
        /// <param name="imageArrayTransferType">Image array transfer type (Only relevant to Camera devices). Default: ImageArrayTransferType.BestAvailable</param>
        /// <param name="imageArrayCompression">Image array compression type (Only relevant to Camera devices). Default: ImageArrayCompression.None</param>
        /// <returns>An Alpaca client instance for the supplied device and configuration</returns>
        /// <remarks>ASCOM Camera client specific parameters can be set through the <see cref="ImageArrayTransferType"/> and <see cref="ImageArrayCompression"/> properties.</remarks>
        public static T GetDevice<T>(AscomDevice ascomDevice, int establishConnectionTimeout = CLIENT_ESTABLISHCONNECTIONTIMEOUT_DEFAULT,
                                  int standardDeviceResponseTimeout = CLIENT_STANDARDCONNECTIONTIMEOUT_DEFAULT, int longDeviceResponseTimeout = CLIENT_LONGCONNECTIONTIMEOUT_DEFAULT, uint clientNumber = CLIENT_CLIENTNUMBER_DEFAULT,
                                  string userName = CLIENT_USERNAME_DEFAULT, string password = CLIENT_PASSWORD_DEFAULT, bool strictCasing = CLIENT_STRICTCASING_DEFAULT, ILogger logger = CLIENT_LOGGER_DEFAULT,
                                  ImageArrayTransferType imageArrayTransferType = CLIENT_IMAGEARRAYTRANSFERTYPE_DEFAULT, ImageArrayCompression imageArrayCompression = CLIENT_IMAGEARRAYCOMPRESSION_DEFAULT) where T : AlpacaDeviceBaseClass, new()
        {
            // Validate that the ascomDevice parameter is not null
            if (ascomDevice is null)
                throw new InvalidValueException($"AlpacaClient.GetDevice - The supplied ascomDevice parameter is null");

            return (T)GetDevice<T>(ascomDevice.ServiceType, ascomDevice.IpAddress, ascomDevice.IpPort, ascomDevice.AlpacaDeviceNumber,
                establishConnectionTimeout, standardDeviceResponseTimeout, longDeviceResponseTimeout, clientNumber, userName, password, strictCasing, logger, imageArrayTransferType, imageArrayCompression);
        }

        /// <summary>
        /// Create an Alpaca client instance specifying all available parameters
        /// </summary>
        /// <typeparam name="T">.NET object type of the device to create e.g. AlpacaCamera, AlpacaTelescope,AlpacaFocuser etc.</typeparam>
        /// <param name="serviceType">HTTP or HTTPS as a ServiceType Default: ServiceType.Http</param>
        /// <param name="ipAddressString">The device's IP address Default: 127.0.0.1</param>
        /// <param name="portNumber">The device's IP port number Default: 11111</param>
        /// <param name="remoteDeviceNumber">The Alpaca device number of this ASCOM device Default: 0</param>
        /// <param name="establishConnectionTimeout">Communications timeout (seconds) when initially connecting to the client. Default: 3 seconds</param>
        /// <param name="standardDeviceResponseTimeout">Communications timeout (seconds) for commands that are expected to complete quickly. Default: 3 seconds</param>
        /// <param name="longDeviceResponseTimeout">Communications timeout (seconds) for commands that are expected to take a long time such as synchronous telescope slews. Default: 100 seconds</param>
        /// <param name="clientNumber">Arbitrary number identifying this particular client. Default: 1</param>
        /// <param name="userName">Basic authentication user name. Supply a <see langword="null"/> or empty string if basic authentication is not required. Default: Empty string</param>
        /// <param name="password"> Basic authentication password. Ignored if password is <see langword="null"/> or an empty string. Default: Empty string</param>
        /// <param name="strictCasing">Set true to require strict JSON casing in device responses.. Default: true</param>
        /// <param name="logger">ILogger instance or null. Default: null</param>
        /// <param name="imageArrayTransferType">Image array transfer type (Only relevant to Camera devices). Default: ImageArrayTransferType.BestAvailable</param>
        /// <param name="imageArrayCompression">Image array compression type (Only relevant to Camera devices). Default: ImageArrayCompression.None</param>
        /// <returns>An Alpaca client of the specified type</returns>
        public static T GetDevice<T>(ServiceType serviceType = CLIENT_SERVICETYPE_DEFAULT, string ipAddressString = CLIENT_IPADDRESS_DEFAULT, int portNumber = CLIENT_IPPORT_DEFAULT, int remoteDeviceNumber = CLIENT_REMOTEDEVICENUMBER_DEFAULT, int establishConnectionTimeout = CLIENT_ESTABLISHCONNECTIONTIMEOUT_DEFAULT,
                                  int standardDeviceResponseTimeout = CLIENT_STANDARDCONNECTIONTIMEOUT_DEFAULT, int longDeviceResponseTimeout = CLIENT_LONGCONNECTIONTIMEOUT_DEFAULT, uint clientNumber = CLIENT_CLIENTNUMBER_DEFAULT,
                                  string userName = CLIENT_USERNAME_DEFAULT, string password = CLIENT_PASSWORD_DEFAULT, bool strictCasing = CLIENT_STRICTCASING_DEFAULT, ILogger logger = CLIENT_LOGGER_DEFAULT,
                                  ImageArrayTransferType imageArrayTransferType = CLIENT_IMAGEARRAYTRANSFERTYPE_DEFAULT, ImageArrayCompression imageArrayCompression = CLIENT_IMAGEARRAYCOMPRESSION_DEFAULT) where T : AlpacaDeviceBaseClass, new()
        {
            if (typeof(T) == typeof(AlpacaCamera)) // Return a camera type with its additional parameters defaulted
            {
                return (T)Activator.CreateInstance(typeof(T), new object[]
                {
                    serviceType,
                    ipAddressString,
                    portNumber,
                    remoteDeviceNumber,
                    establishConnectionTimeout,
                    standardDeviceResponseTimeout,
                    longDeviceResponseTimeout,
                    clientNumber,
                    imageArrayTransferType,
                    imageArrayCompression,
                    userName,
                    password,
                    strictCasing,
                    logger,
                    null,
                    null
                });
            }
            else // Return a standard device client
            {
                return (T)Activator.CreateInstance(typeof(T), new object[]
                {
                    serviceType,
                    ipAddressString,
                    portNumber,
                    remoteDeviceNumber,
                    establishConnectionTimeout,
                    standardDeviceResponseTimeout,
                    longDeviceResponseTimeout,
                    clientNumber,
                    userName,
                    password,
                    strictCasing,
                    logger,
                    null,
                    null
                });
            }

        }

        #endregion
    }
}
