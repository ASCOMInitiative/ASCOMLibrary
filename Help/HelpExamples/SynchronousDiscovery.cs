using ASCOM.Alpaca.Discovery;
using ASCOM.Common;
using ASCOM.Common.Alpaca;
using System.Reflection.Metadata;

namespace HelpExamples
{
    internal class SynchronousDiscoveryClass
    {
        internal static void SynchronousDiscovery()
        {
            #region SynchronousDiscovery
            // Create a Discovery component that can be used for one or more discoveries
            using (AlpacaDiscovery alpacaDiscovery = new AlpacaDiscovery())
            {
                // Start a discovery with default parameter values
                alpacaDiscovery.StartDiscovery();

                // Wait for the discovery to complete, testing the completion variable every 50ms.
                do
                {
                    Thread.Sleep(50);
                } while (!alpacaDiscovery.DiscoveryComplete);

                // Get lists of discovered Telescope and FilterWheel devices and print counts of each
                List<AscomDevice> telescopeDevices1 = alpacaDiscovery.GetAscomDevices(DeviceTypes.Telescope);
                List<AscomDevice> filterWheelDevices1 = alpacaDiscovery.GetAscomDevices(DeviceTypes.FilterWheel);
                Console.WriteLine($"Found {telescopeDevices1.Count} Telescope devices and {filterWheelDevices1.Count} FilterWheel devices.");

                // Start a discovery with specified parameters
                alpacaDiscovery.StartDiscovery(2, 100, 32227, 1.5, false, true, false, ServiceType.Http);

                // Wait for the discovery to complete, testing the completion variable every 50ms.
                do
                {
                    Thread.Sleep(50);
                } while (!alpacaDiscovery.DiscoveryComplete);

                // Get lists of discovered Telescope and FilterWheel devices and print counts of each
                List<AscomDevice> telescopeDevices2 = alpacaDiscovery.GetAscomDevices(DeviceTypes.Telescope);
                List<AscomDevice> filterWheelDevices2 = alpacaDiscovery.GetAscomDevices(DeviceTypes.FilterWheel);
                Console.WriteLine($"Found {telescopeDevices2.Count} Telescope devices and {filterWheelDevices2.Count} FilterWheel devices.");
            }
            #endregion
        }
    }
}