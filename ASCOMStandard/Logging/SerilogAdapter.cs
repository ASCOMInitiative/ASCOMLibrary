using System;

namespace ASCOM.Alpaca.Logging
{
    public class SerilogAdapter : ILogger
    {
        private readonly Serilog.ILogger _logger;

        public SerilogAdapter(Serilog.ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

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
                    _logger.Warning(logEvent.Message, logEvent.Exception, logEvent.PropertyValues);
                    break;
                
                case LogLevel.Information:
                    _logger.Information(logEvent.Exception, logEvent.Message, logEvent.PropertyValues);
                    break;
                
                case LogLevel.Debug:
                    _logger.Debug(logEvent.Exception, logEvent.Message, logEvent.PropertyValues);
                    break;
                
                default:
                    _logger.Verbose(logEvent.Exception, logEvent.Message, logEvent.PropertyValues);
                    break;
            }
        }
    }
}