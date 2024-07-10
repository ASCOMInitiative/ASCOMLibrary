using ASCOM.Alpaca.Clients;
using ASCOM.Common.Alpaca;
using ASCOM.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpExamples
{
    internal class ManualClientCreationClass
    {
        internal static void ManualClientCreation()
        {
            try
            {
            #region Detailed manual client creation
                // Create a trace logger
                using (TraceLogger logger = new TraceLogger("ManualClient", true))
                {
                    // Create the telescope Alpaca Client with specified parameters
                    using (AlpacaTelescope telescopeClient = AlpacaClient.GetDevice<AlpacaTelescope>(ServiceType.Http, "127.0.0.1", 11111, 0, 3, 5, 100, 34892, "QuY89", "YYu8*9jK", true, logger))
                    {
                        // Connect to the Alpaca device
                        telescopeClient.Connected = true;

                        // Record some information
                        logger.LogMessage("ManualClient", $"Found device: {telescopeClient.Name} - Driver: {telescopeClient.DriverInfo}, Version: {telescopeClient.DriverVersion} Telescope is tracking: {telescopeClient.Tracking}.");

                        // Disconnect from the filter wheel
                        telescopeClient.Connected = false;
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
