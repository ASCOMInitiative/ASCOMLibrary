using ASCOM.Common.Interfaces;
using System;

namespace ASCOM.Alpaca.Clients
{
   /// <summary>
   /// Helper functions to return an instance of an Alpaca client
   /// </summary>
    public class AlpacaClient
    {
        /// <summary>
        /// Create an Alpaca client instance with the minimum required parameters (time-outs, client number and user ID parameters will have default values)
        /// </summary>
        /// <typeparam name="T">.NET object type of the device to create e.g. AlpacaCamera, AlpacaTelescope,AlpacaFocuser etc.</typeparam>
        /// <param name="serviceType">HTTP or HTTPS</param>
        /// <param name="ipAddressString">The device's IP address.</param>
        /// <param name="portNumber">The device's IP port number.</param>
        /// <param name="remoteDeviceNumber">The Alpaca device number of this ASCOM device within the overall Alpaca server.</param>
        /// <param name="logger">ILogger instance to which operational messages will be sent.</param>
        /// <returns></returns>
        public static T GetDevice<T>(string serviceType, string ipAddressString, int portNumber, int remoteDeviceNumber, ILogger logger) where T : new()
        {
            return (T)Activator.CreateInstance(typeof(T), new object[] { serviceType, ipAddressString, portNumber, remoteDeviceNumber, logger });
        }

        /// <summary>
        /// Create an Alpaca client instance specifying all available parameters
        /// </summary>
        /// <typeparam name="T">.NET object type of the device to create e.g. AlpacaCamera, AlpacaTelescope,AlpacaFocuser etc.</typeparam>
        /// <param name="serviceType">HTTP or HTTPS</param>
        /// <param name="ipAddressString">The device's IP address.</param>
        /// <param name="portNumber">The device's IP port number.</param>
        /// <param name="remoteDeviceNumber">The Alpaca device number of this ASCOM device within the overall Alpaca server.</param>
        /// <param name="establishConnectionTimeout">Timeout for initial connection</param>
        /// <param name="standardDeviceResponseTimeout">Timeout for short lived methods (sec)</param>
        /// <param name="longDeviceResponseTimeout">Timeout for long lived methods (sec.)</param>
        /// <param name="clientNumber"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="logger">ILogger instance to which operational messages will be sent.</param>
        /// <returns></returns>
        public static T GetDevice<T>(string serviceType, string ipAddressString, int portNumber, int remoteDeviceNumber, int establishConnectionTimeout,
                                  int standardDeviceResponseTimeout, int longDeviceResponseTimeout, uint clientNumber, string userName, string password, ILogger logger) where T : new()
        {
            return (T)Activator.CreateInstance(typeof(T), new object[] { serviceType, ipAddressString, portNumber, remoteDeviceNumber, establishConnectionTimeout,
                                        standardDeviceResponseTimeout, longDeviceResponseTimeout, clientNumber, userName, password, logger });
        }
    }
}




