using ASCOM.Alpaca.Discovery;
using ASCOM.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpExamples
{
    internal class AsynchronousDiscoveryClass
    {
        #region Asynchronous Discovery
        internal static void AsynchronousDiscovery()
        {
            // Create a Discovery component that can be used for one or more discoveries
            AlpacaDiscovery alpacaDiscovery = new AlpacaDiscovery();

            // Add an event handler that will be called when discovery is complete.
            alpacaDiscovery.DiscoveryCompleted += DiscoveryCompletedEventHandler;

            // Start a discovery using specified parameters:
            alpacaDiscovery.StartDiscovery(2, 100, 32227, 1.5, false, true, false, ASCOM.Common.Alpaca.ServiceType.Http);

            // Continue with other processing while the discovery is running
            // The DiscoveryCompletedEventHandler method will be called when discovery completes

            //TODO: Dispose of the alpacaDiscovery object when it is no longer required
        }

        ///<summary>
        ///Event handler called when the configured discovery time is reached
        ///</summary>
        static void DiscoveryCompletedEventHandler(object? sender, EventArgs e)
        {
            // Ensure that the sender is an AlpacaDiscovery object
            if (sender is AlpacaDiscovery alpacaDiscovery)
            {
                 // Get a list of available FilterWheel devices and print the number of devices found
                List<AscomDevice> filterWheelDevices = alpacaDiscovery.GetAscomDevices(DeviceTypes.FilterWheel);
                Console.WriteLine($"Found {filterWheelDevices.Count} FilterWheel devices.");
            }
        }
        #endregion
    }
}