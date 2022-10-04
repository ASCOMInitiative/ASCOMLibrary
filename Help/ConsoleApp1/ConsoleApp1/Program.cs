using ASCOM.Common.Interfaces;
using ASCOM.Tools;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        ILogger TL = new TraceLogger("LogTest", true);
        TL.LogMessage("test method", "test message");

        ConsoleLogger consoleLogger = new ConsoleLogger();
        consoleLogger.LogMessage("TestConsole", "Test console message.");
    }
}


/// <summary>
/// This is a standard set of extensions that add functionality to an ILogger. Because these can be implemented in a standard way they are not part of the interface
/// </summary>
public static class LoggerExtensions2
{

    /// <summary>
    /// Log a message at verbose level
    /// </summary>
    /// <param name="logger">ILogger device instance</param>
    /// <param name="message">Message to log</param>
    public static void LogMessage(this ILogger logger, string method, string message)
    {
        if (logger is ITraceLogger traceLogger)
        {
            traceLogger.LogMessage(method, message);
        }
        else
        {
            logger?.Log(LogLevel.Fatal, message);
        }
    }

}
