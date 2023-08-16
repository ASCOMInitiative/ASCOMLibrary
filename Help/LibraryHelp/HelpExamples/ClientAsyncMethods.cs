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
            //TraceLogger TL = new TraceLogger("AsyncMethods", true, logLevel: LogLevel.Debug, identifierWidth: 64);

            try
            {
                #region ClientAsync1
                // Create Alpaca clients using default device parameters (service type: http, IP address 127.0.0.1, IP port: 11111, alpaca device number: 0)
                // and assuming that the device serves telescope, filter wheel, rotator, focuser and camera devices)
                using (AlpacaTelescope telescope = new AlpacaTelescope())
                using (AlpacaFilterWheel filterWheel = new AlpacaFilterWheel())
                using (AlpacaRotator rotator = new AlpacaRotator())
                using (AlpacaFocuser focuser = new AlpacaFocuser())
                using (AlpacaCamera camera = new AlpacaCamera())
                {
                    // Connect to devices
                    telescope.Connected = true;
                    filterWheel.Connected = true;
                    rotator.Connected = true;
                    focuser.Connected = true;
                    camera.Connected = true;

                    // Prepare telescope and calculate a target RA 1 hour from the meridian based on local sidereal time
                    await telescope.UnparkAsync();
                    telescope.Tracking = true;
                    double targetRa = (telescope.SiderealTime + 1.0) % 24.0;

                    // Start concurrent operations on all devices to prepare for imaging
                    Task slewScope = telescope.SlewToCoordinatesTaskAsync(targetRa, 0.0);
                    Task setFilterWheel = filterWheel.PositionSetAsync(1);
                    Task setRotator = rotator.MoveAbsoluteAsync(45.0);
                    Task setFocuser = focuser.MoveAsync(focuser.MaxIncrement / 3);

                    // Wait for all devices to complete setup
                    await Task.WhenAll(slewScope, setFilterWheel, setRotator, setFocuser);

                    // Make a camera exposure and wait for it to complete
                    await Task.WhenAny(camera.StartExposureAsync(2.0, true));

                    // Clean-up
                    camera.Connected = false;
                    focuser.Connected = false;
                    rotator.Connected = false;
                    filterWheel.Connected = false;
                    telescope.Connected = false;
                }
                #endregion

                #region ClientAsync2
                // Create a trace logger to record operational messages. (This implements the ILogger interface)
                TraceLogger TL = new TraceLogger("ClientAsync", true, logLevel: LogLevel.Debug, identifierWidth: 64);

                // Create a cancellation token that will time out after 30 seconds
                CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(30.0));
                CancellationToken cancelToken = cts.Token;

                // Create a rotator device and move it by 45.0 degrees
                using (AlpacaRotator rotator = new AlpacaRotator())
                {
                    // POSITIONAL PARAMETER EXAMPLES

                    // Move using defaults: no cancellation, polling every second, no operational messages
                    await rotator.MoveAsync(45.0); // Equivalent to: MoveAsync(45.0, CancellationToken.None, 1000, null);

                    // Move using a cancellation token and defaults: polling every second, no operational messages
                    await rotator.MoveAsync(45.0, cancelToken); // Equivalent to: MoveAsync(45.0, cancelToken, 1000, null);

                    // Move using a cancellation token, polling every 100 milliseconds and default: no operational messages
                    await rotator.MoveAsync(45.0, cancelToken, 100); // Equivalent to: MoveAsync(45.0, cancelToken, 100, null);

                    // Move using a cancellation token, polling every 100 milliseconds and logging operational messages
                    await rotator.MoveAsync(45.0, cancelToken, 100, TL);

                    // NAMED PARAMETER EXAMPLES

                    // Move and log operational messages using defaults: no cancellation, polling every second 
                    await rotator.MoveAsync(45.0, logger: TL); // Equivalent to: MoveAsync(45.0, CancellationToken.None, 1000, TL);

                    // Move polling every 250 milliseconds with defaults: no cancellation, no operational messages
                    await rotator.MoveAsync(45.0, pollInterval: 250); // Equivalent to: await rotator.MoveAsync(45.0, CancellationToken.None, 250, null);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
            //TL.LogMessage("Main", "Finished");

        }

    }
}
