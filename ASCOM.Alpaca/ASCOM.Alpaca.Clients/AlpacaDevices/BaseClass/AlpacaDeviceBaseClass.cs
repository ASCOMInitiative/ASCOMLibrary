using ASCOM.Common.Interfaces;
using RestSharp;
using System;

namespace ASCOM.Alpaca.Clients
{
    public abstract class AlpacaDeviceBaseClass : IDisposable
    {
        // Variables common to all instances
        internal ServiceType serviceType = ServiceType.Http;
        internal string ipAddressString = "127.0.0.1";
        internal decimal portNumber = 11111;
        internal decimal remoteDeviceNumber = 0; // Device number in the URI on the remote Alpaca device
        internal int establishConnectionTimeout = 3;
        internal int standardDeviceResponseTimeout = 3;
        internal int longDeviceResponseTimeout = 100;
        internal uint clientNumber; // Unique number for this driver within the locaL server, i.e. across all drivers that the local server is serving
        internal string userName = "";
        internal string password = "";
        internal bool manageConnectLocally = false;
        internal bool strictCasing = true; // Strict or flexible interpretation of casing in device JSON responses

        internal ILogger TL; // Private variable to hold the trace logger object

        internal RestClient client; // Client to send and receive REST style messages to / from the remote device
        internal bool clientIsConnected;  // Connection state of this driver
        internal string URIBase; // URI base unique to this driver
        private bool disposedValue;

        #region Support code

        internal static void LogMessage(ILogger logger, uint instance, string prefix, string message)
        {
            if (logger != null) logger.Log(LogLevel.Information, $"{prefix} {instance}".PadRight(30) + message);
        }

        internal static void LogBlankLine(ILogger logger)
        {
            if (logger != null) logger.Log(LogLevel.Information, $" ");
        }

        #endregion

        #region Dispose Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (!(client is null))
                    {
                        client.ClearHandlers();
                        client = null;
                    }
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put clean-up code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
