using ASCOM.Common;
using ASCOM.Common.Interfaces;
using System;

namespace ASCOM.Tools
{
    /// <summary>
    /// Simple logger to write to the console
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// Return the current logging level
        /// </summary>
        public LogLevel LoggingLevel
        {
            get;
            private set;
        } = LogLevel.Information;

        /// <summary>
        /// Write a message to the console
        /// </summary>
        /// <param name="level">Logging level</param>
        /// <param name="message">Message text</param>
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

                Console.WriteLine($" - {message}");
            }
        }

        /// <summary>
        /// Minimum logging level to display.
        /// </summary>
        /// <param name="level">Required logging level.</param>
        public void SetMinimumLoggingLevel(LogLevel level)
        {
            LoggingLevel = level;
        }
    }
}
