using ASCOM.Standard.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCOM.Standard.AlpacaClients
{
    public class AlpacaClient
    {
        public static T GetDevice<T>(string serviceType, string ipAddressString, int portNumber, int remoteDeviceNumber, ILogger logger) where T : new()
        {
            return (T)Activator.CreateInstance(typeof(T), new object[] { serviceType, ipAddressString, portNumber, remoteDeviceNumber, logger });
        }

        public static T GetDevice<T>(string serviceType, string ipAddressString, int portNumber, int remoteDeviceNumber, int establishConnectionTimeout,
                                  int standardDeviceResponseTimeout, int longDeviceResponseTimeout, uint clientNumber, string userName, string password, ILogger logger) where T : new()
        {
            return (T)Activator.CreateInstance(typeof(T), new object[] { serviceType, ipAddressString, portNumber, remoteDeviceNumber, establishConnectionTimeout,
                                        standardDeviceResponseTimeout, longDeviceResponseTimeout, clientNumber, userName, password, logger });
        }



    }
}




