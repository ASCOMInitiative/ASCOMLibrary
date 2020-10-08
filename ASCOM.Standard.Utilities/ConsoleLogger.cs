using ASCOM.Standard.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Standard.Utilities
{
    public class ConsoleLogger : ILogger
    {
        public LogLevel LoggingLevel
        {
            get;
            private set;
        } = LogLevel.Information;

        public void Log(LogLevel level, string message)
        {
            if (this.IsLevelActive(level))
            {
                Console.Write($"{DateTime.Now} ");
                switch (level)
                {
                    case LogLevel.Verbose:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    case LogLevel.Debug:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                    case LogLevel.Information:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case LogLevel.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LogLevel.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case LogLevel.Fatal:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        break;
                }

                Console.Write($"[{level}]");

                Console.ResetColor();

                Console.WriteLine($" - { message}");
            }
        }

        public void SetMinimumLoggingLevel(LogLevel level)
        {
            LoggingLevel = level;
        }
    }
}
