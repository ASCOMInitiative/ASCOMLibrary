using ASCOM.Alpaca.Discovery;
using ASCOM.Common;
using ASCOM.Common.Alpaca;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpExamples
{
    internal class AsyncMethodsAwaitClass
    {
        internal static async void AsyncMethodsAwait()
        {
            try
            {
                #region AsynchronousMethodsAwait1
                // Get a list of Alpaca devices using default values
                List<AlpacaDevice> alpacaDevices1 = await AlpacaDiscovery.GetAlpacaDevicesAsync();
                Console.WriteLine($"Discovered {alpacaDevices1.Count} Alpaca devices.");

                // Get a list of ASCOM devices of a specified device type using default values
                List<AscomDevice> ascomTelescopeDevicesAsync1 = await AlpacaDiscovery.GetAscomDevicesAsync(DeviceTypes.Telescope);
                Console.WriteLine($"Discovered {ascomTelescopeDevicesAsync1.Count} ASCOM Telescope devices.");

                // Get a list of ASCOM devices of all types using default values
                List<AscomDevice> allAscomDevicesAsync = await AlpacaDiscovery.GetAscomDevicesAsync(null);
                Console.WriteLine($"Discovered a total of {allAscomDevicesAsync.Count} ASCOM devices.");

                // Get a list of ASCOM Telescope devices by issuing 2 discovery packets with an interval of 200ms, on port 32227, with a discovery duration of 2.5 seconds,
                // without DNS name resolution and using IPv4 and IPv6 over HTTPS with no operational message logger.
                List<AscomDevice> devices = await AlpacaDiscovery.GetAscomDevicesAsync(DeviceTypes.Telescope, 2, 200, 32227, 2.5, false, true, true, ServiceType.Https, null);
                Console.WriteLine($"Discovered {devices.Count} ASCOM Telescope devices.");
                #endregion

                #region AsynchronousMethodsAwait2
                // Get a list of Alpaca devices using default values, apart from enabling discovery of IPv6 devices
                List<AlpacaDevice> alpacaDevices2 = await AlpacaDiscovery.GetAlpacaDevicesAsync(useIpV6: true);
                Console.WriteLine($"Discovered {alpacaDevices2.Count} Alpaca devices.");
                #endregion

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
    }
}
