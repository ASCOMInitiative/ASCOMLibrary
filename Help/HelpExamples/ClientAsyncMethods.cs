using ASCOM.Com.DriverAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASCOM.Common;
using ASCOM.Tools;
using ASCOM.Common.Interfaces;
using ASCOM.Alpaca.Clients;
using ASCOM.Common.Alpaca;

namespace HelpExamples
{
    internal class ClientAsyncMethods
    {
        internal static async Task AsyncMethods()
        {
            TraceLogger TL = new TraceLogger("AsyncMethods", true, logLevel: LogLevel.Debug, identifierWidth: 64);

            try
            {
                // Create devices
                AlpacaTelescope telescope = new AlpacaTelescope();
                telescope.Connected = true;
                telescope.Unpark();
                telescope.Tracking = true;
                double targetRa = (telescope.SiderealTime + 25.0) % 24.0;

                // Connect to devices
                AlpacaFilterWheel filterWheel = new AlpacaFilterWheel(TL); //ServiceType.Http, "127.0.0.1", 11111, 0, true, null);
                filterWheel.Connected = true;

                AlpacaRotator rotator = new AlpacaRotator();
                rotator.Connected = true;

                AlpacaFocuser focuser = new AlpacaFocuser();
                focuser.Connected = true;

                AlpacaCamera camera = new AlpacaCamera();
                camera.Connected = true;

                // Start setup operations on all devices except camera
                TL.LogMessage("Main", "Starting device setup tasks");
                Task slewScope = telescope.SlewToCoordinatesTaskAsync(targetRa, 0.0, pollInterval: 100, logger: TL);
                Task setFilterWheel = filterWheel.PositionSetAsync(1, pollInterval: 100, logger: TL);
                Task setRotator = rotator.MoveAbsoluteAsync(45.0, pollInterval: 100, logger: TL);
                Task setFocuser = focuser.MoveAsync(focuser.MaxIncrement / 3, pollInterval: 100, logger: TL);

                // Wait for all devices, which are moving at the same time to be ready
                await Task.WhenAll(slewScope, setFilterWheel, setRotator, setFocuser);
                TL.LogMessage("Main", "Devices setup, taking image");

                // Wait for the camera to complete it's exposure
                await Task.WhenAll(camera.StartExposureAsync(2.0, true, pollInterval: 100, logger: TL));
                TL.LogMessage("Main", "Image taken");

                // Clean-up
                camera.Connected = false; //camera.Dispose();
                focuser.Connected = false; //focuser.Dispose();
                rotator.Connected = false; //rotator.Dispose();
                filterWheel.Connected = false; //filterWheel.Dispose();
                telescope.Connected = false; //telescope.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
            TL.LogMessage("Main", "Finished");

        }

    }
}
