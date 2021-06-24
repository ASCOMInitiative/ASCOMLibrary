using ASCOM.Standard.Interfaces;
using System;

namespace ASCOM.Standard.Utilities
{
    public static class Logger
    {
        //ToDo verify this works for different deployment types. Alternatively System.Reflection.Assembly.GetEntryAssembly().GetName().Name may work
        public static ILogger LogProvider
        {
            get;
            private set;
        } = new TraceLogger(null, null, AppDomain.CurrentDomain.FriendlyName, true);

        public static void SetLogProvider(ILogger logger)
        {
            LogProvider = logger;
        }

        public static void LogVerbose(string message)
        {
            try
            {
                LogProvider?.LogVerbose(message);
            }
            catch
            {
                //This method must never throw;
            }
        }

        public static void LogDebug(string message)
        {
            try
            {
                LogProvider?.LogDebug(message);
            }
            catch
            {
                //This method must never throw;
            }
        }

        public static void LogInformation(string message)
        {
            try
            {
                LogProvider?.LogInformation(message);
            }
            catch
            {
                //This method must never throw;
            }
        }

        public static void LogWarning(string message)
        {
            try
            {
                LogProvider?.LogWarning(message);
            }
            catch
            {
                //This method must never throw;
            }
        }

        public static void LogError(string message)
        {
            try
            {
                LogProvider?.LogError(message);
            }
            catch
            {
                //This method must never throw;
            }
        }

        public static void LogFatal(string message)
        {
            try
            {
                LogProvider?.LogFatal(message);
            }
            catch
            {
                //This method must never throw;
            }
        }
    }
}