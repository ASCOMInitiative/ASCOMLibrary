using ASCOM.Alpaca.Discovery;
using ASCOM.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpExamples
{
    internal class AsyncMethodsTaskClass
    {
        internal static void AsyncMethodsTask()
        {
            try
            {
                #region AsynchronousMethodsTask
                // Get a list of Alpaca devices asynchronously
                Task<List<AlpacaDevice>> alpacadevicesTask = AlpacaDiscovery.GetAlpacaDevicesAsync();

                // Control returns quickly from AlpacaDiscovery.GetAlpacaDevicesAsync() and there is the opportunity to do other work
                DoWork();

                // Eventually, wait for the discovery task to complete
                Task.WaitAll(alpacadevicesTask);

                // Retrieve the list of discovered Alpaca devices
                List<AlpacaDevice> alpacaDevices = alpacadevicesTask.Result;
                Console.WriteLine("AsynchronousTask", $"Discovered {alpacaDevices.Count} Alpaca devices.");
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        static void DoWork()
        {
        }
    }
}
