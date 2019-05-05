using System;

namespace ASCOM.Alpaca.Logging
{
    /// <summary>
    /// Adpater to support NLog
    /// </summary>
    public class NLogAdapter : ILogger
    {
        private readonly NLog.ILogger _logger;

        /// <summary>
        /// Initialise the NLog adpater
        /// </summary>
        /// <param name="logger"></param>
        public NLogAdapter(NLog.ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Override for the Log event
        /// </summary>
        /// <param name="logEvent"></param>
        public void Log(LogEvent logEvent)
        {
            switch (logEvent.LogLevel)
            {
                case LogLevel.Fatal:
                    _logger.Fatal(logEvent.Message, logEvent.Exception, logEvent.PropertyValues);
                    break;
                
                case LogLevel.Error:
                    _logger.Error(logEvent.Message, logEvent.Exception, logEvent.PropertyValues);
                    break;
                
                case LogLevel.Warning:
                    _logger.Warn(logEvent.Message, logEvent.Exception, logEvent.PropertyValues);
                    break;
                
                case LogLevel.Information:
                    _logger.Info(logEvent.Exception, logEvent.Message, logEvent.PropertyValues);
                    break;
                
                case LogLevel.Debug:
                    _logger.Debug(logEvent.Exception, logEvent.Message, logEvent.PropertyValues);
                    break;
                
                default:
                    _logger.Trace(logEvent.Exception, logEvent.Message, logEvent.PropertyValues);
                    break;
            }
        }
    }
}