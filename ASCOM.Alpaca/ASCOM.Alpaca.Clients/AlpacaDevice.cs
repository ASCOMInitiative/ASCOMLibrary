using ASCOM.Alpaca.Discovery;
using ASCOM.Common.Alpaca;
using ASCOM.Common.Interfaces;
using System;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// Helper functions to return an instance of an Alpaca client
    /// </summary>
    public class AlpacaClient
    {

        #region Public static methods

        /// <summary>
        /// Create an Alpaca client for a discovered ASCOM device with the minimum required parameters (time-outs, client number and user ID parameters will have default values)
        /// </summary>
        /// <typeparam name="T">.NET object type of the device to create e.g. AlpacaCamera, AlpacaTelescope,AlpacaFocuser etc.</typeparam>
        /// <param name="ascomDevice">An AscomDevice instance representing the device to use.</param>
        /// <returns>An Alpaca client of the specified type</returns>
        /// <remarks>All client configuration properties can be set, after client creation, using the <see cref="AlpacaDeviceBaseClass.ClientConfiguration"/> method and the
        /// Alpaca Camera specific properties <see cref="ImageArrayTransferType"/> and <see cref="ImageArrayCompression"/>.</remarks>
        public static T GetDevice<T>(AscomDevice ascomDevice) where T : AlpacaDeviceBaseClass, new()
        {
            // Validate that the ascomDevice parameter is not null
            if (ascomDevice is null)
                throw new InvalidValueException($"AlpacaClient.GetDevice - The supplied ascomDevice parameter is null");

                return (T)Activator.CreateInstance(typeof(T), new object[] { ascomDevice.ServiceType, ascomDevice.IpAddress, ascomDevice.IpPort, ascomDevice.AlpacaDeviceNumber, true, null });
        }

        /// <summary>
        /// Create an Alpaca client for a discovered ASCOM device specifying all parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ascomDevice">An AscomDevice instance representing the device to use.</param>
        /// <param name="strictCasing">Set true to require strict JSON casing in device responses.</param>
        /// <param name="establishConnectionTimeout">Communications timeout when initially connecting to the client</param>
        /// <param name="standardDeviceResponseTimeout">Communications timeout for commands that are expected to complete quickly</param>
        /// <param name="longDeviceResponseTimeout">Communications timeout for commands that are expected to take a long time such as synchronous telescope slews</param>
        /// <param name="clientNumber">Arbitrary number identifying this particular client</param>
        /// <param name="userName">Basic authentication user name. Supply a <see langword="null"/> or empty string if basic authentication is not required</param>
        /// <param name="password"> Basic authentication password. Ignored if password is <see langword="null"/> or an empty string</param>
        /// <param name="logger">Optional ILogger instance.</param>
        /// <returns>An Alpaca client instance for the supplied device and configuration</returns>
        /// <remarks>ASCOM Camera client specific parameters can be set through the <see cref="ImageArrayTransferType"/> and <see cref="ImageArrayCompression"/> properties.</remarks>
        public static T GetDevice<T>(AscomDevice ascomDevice, bool strictCasing, int establishConnectionTimeout,
                                  int standardDeviceResponseTimeout, int longDeviceResponseTimeout, uint clientNumber, string userName, string password, ILogger logger) where T : AlpacaDeviceBaseClass, new()
        {
            // Validate that the ascomDevice parameter is not null
            if (ascomDevice is null)
                throw new InvalidValueException($"AlpacaClient.GetDevice - The supplied ascomDevice parameter is null");

            if (typeof(T) == typeof(AlpacaCamera)) // Return a camera type with its additional parameters defaulted

            {
                return (T)Activator.CreateInstance(typeof(T), new object[] {ascomDevice.ServiceType, ascomDevice.IpAddress, ascomDevice.IpPort, ascomDevice.AlpacaDeviceNumber, establishConnectionTimeout, standardDeviceResponseTimeout, longDeviceResponseTimeout,
                clientNumber, ImageArrayTransferType.BestAvailable, ImageArrayCompression.None,userName, password, strictCasing, logger });
            }
            else // Return a standard device client
            {
                return (T)Activator.CreateInstance(typeof(T), new object[] {ascomDevice.ServiceType, ascomDevice.IpAddress, ascomDevice.IpPort, ascomDevice.AlpacaDeviceNumber, establishConnectionTimeout, standardDeviceResponseTimeout, longDeviceResponseTimeout,
                clientNumber, userName, password, strictCasing, logger });
            }
        }

        /// <summary>
        /// Create an Alpaca client instance with the minimum required parameters (time-outs, client number and user ID parameters will have default values)
        /// </summary>
        /// <typeparam name="T">.NET object type of the device to create e.g. AlpacaCamera, AlpacaTelescope,AlpacaFocuser etc.</typeparam>
        /// <param name="serviceType">HTTP or HTTPS as a ServiceType</param>
        /// <param name="ipAddressString">The device's IP address.</param>
        /// <param name="portNumber">The device's IP port number.</param>
        /// <param name="remoteDeviceNumber">The Alpaca device number of this ASCOM device within the overall Alpaca server.</param>
        /// <param name="logger">ILogger instance to which operational messages will be sent.</param>
        /// <returns>An Alpaca client of the specified type</returns>
        public static T GetDevice<T>(ServiceType serviceType, string ipAddressString, int portNumber, int remoteDeviceNumber, ILogger logger) where T : AlpacaDeviceBaseClass, new()
        {
            return (T)Activator.CreateInstance(typeof(T), new object[] { serviceType, ipAddressString, portNumber, remoteDeviceNumber, logger });
        }

        /// <summary>
        /// Create an Alpaca client instance specifying all available parameters
        /// </summary>
        /// <typeparam name="T">.NET object type of the device to create e.g. AlpacaCamera, AlpacaTelescope,AlpacaFocuser etc.</typeparam>
        /// <param name="serviceType">HTTP or HTTPS as a ServiceType</param>
        /// <param name="ipAddressString">The device's IP address.</param>
        /// <param name="portNumber">The device's IP port number.</param>
        /// <param name="remoteDeviceNumber">The Alpaca device number of this ASCOM device within the overall Alpaca server.</param>
        /// <param name="establishConnectionTimeout">Timeout for initial connection</param>
        /// <param name="standardDeviceResponseTimeout">Timeout for short lived methods (sec)</param>
        /// <param name="longDeviceResponseTimeout">Timeout for long lived methods (sec.)</param>
        /// <param name="clientNumber">Arbitrary unique number used to identify transactions from this client instance.</param>
        /// <param name="userName">Basic authentication user name if required</param>
        /// <param name="password">Basic authentication password if required</param>
        /// <param name="logger">ILogger instance to which operational messages will be sent.</param>
        /// <returns>An Alpaca client of the specified type</returns>
        public static T GetDevice<T>(ServiceType serviceType, string ipAddressString, int portNumber, int remoteDeviceNumber, int establishConnectionTimeout,
                                  int standardDeviceResponseTimeout, int longDeviceResponseTimeout, uint clientNumber, string userName, string password, ILogger logger) where T : AlpacaDeviceBaseClass, new()
        {
            return (T)Activator.CreateInstance(typeof(T), new object[] { serviceType, ipAddressString, portNumber, remoteDeviceNumber, establishConnectionTimeout,
                                        standardDeviceResponseTimeout, longDeviceResponseTimeout, clientNumber, userName, password, logger });
        }

        #endregion

    }
}
