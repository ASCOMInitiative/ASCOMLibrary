using ASCOM.Alpaca.Clients;
using ASCOM.Alpaca.Discovery;
using ASCOM.Common;
using ASCOM.Common.Alpaca;
using ASCOM.Tools;

namespace HelpExamples
{
    internal class DetailedClientCreationClass
    {
        internal static async void DetailedClientCreation()
        {
            // This .NET 6 Console project example intentionally avoids using statements to illustrate use of the ASCOM Library namespaces
            // It also assumes that the operating system is Windows

            // Requires ASCOM Library packages: ASCOM.Alpaca.Components

            try
            {
                #region Detailed client creation using a discovered AscomDevice
                // Create a TraceLogger to record operational library messages
                using (TraceLogger logger = new TraceLogger("DetailedClient", true))
                {
                    // Get a list of available focuser devices
                    List<AscomDevice> focuserDevices = await AlpacaDiscovery.GetAscomDevicesAsync(DeviceTypes.Focuser, logger: logger);
                    logger.LogMessage("DetailedClient", $"Found {focuserDevices.Count} Focuser devices.");

                    // Create a focuser client for the first device in the list and use it to display device information
                    if (focuserDevices.Count > 0) // There is at least one Alpaca device
                    {
                        // Create a focuser client specifying all parameters
                        using (AlpacaFocuser focuserClient = AlpacaClient.GetDevice<AlpacaFocuser>(focuserDevices.First(), 3, 5, 100, 23549, "QuY89", "YYu8*9jK", true, logger))
                        {
                            // Connect to the Alpaca device
                            focuserClient.Connected = true;

                            // Record some information
                            logger.LogMessage("DetailedClient", $"Found device: {focuserClient.Name} - Driver: {focuserClient.DriverInfo}, Version: {focuserClient.DriverVersion}, Focuser position: {focuserClient.Position}.");

                            // Disconnect from the focuser
                            focuserClient.Connected = false;
                        }
                    }
                    else // No devices were discovered
                    {
                        logger.LogMessage("DetailedClient", $"No Focuser devices were discovered.");
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
