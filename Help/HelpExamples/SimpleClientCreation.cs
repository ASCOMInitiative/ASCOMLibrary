using ASCOM.Alpaca.Clients;
using ASCOM.Alpaca.Discovery;
using ASCOM.Common;
using ASCOM.Common.Alpaca;
using ASCOM.Tools;

namespace HelpExamples
{
    internal class SimpleClientCreationClass
    {
        internal static async void SimpleClientCreation()
        {
            try
            {
                #region Simple client creation using a discovered AscomDevice
                // Create a TraceLogger to record operational library messages
                using (TraceLogger logger = new TraceLogger("SimpleClient", true))
                {

                    // Get a list of filter wheel devices and record the number of devices discovered
                    List<AscomDevice> filterWheelDevices = await AlpacaDiscovery.GetAscomDevicesAsync(DeviceTypes.FilterWheel);
                    logger.LogMessage("SimpleClient", $"Found {filterWheelDevices.Count} FilterWheel devices.");

                    // Create an Alpaca client for the first device and use it to display the driver description
                    if (filterWheelDevices.Count > 0)
                    {
                        // Create the filter wheel Alpaca Client
                        using (AlpacaFilterWheel filterWheelClient = AlpacaClient.GetDevice<AlpacaFilterWheel>(filterWheelDevices.First(), logger: logger))
                        {
                            // Connect to the Alpaca device
                            filterWheelClient.Connected = true;

                            // Record some information
                            logger.LogMessage("SimpleClient", $"Found device: {filterWheelClient.Name} - Driver: {filterWheelClient.DriverInfo}, Version: {filterWheelClient.DriverVersion} containing {filterWheelClient.Names.Count()} filters.");

                            // Disconnect from the filter wheel
                            filterWheelClient.Connected = false;
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
