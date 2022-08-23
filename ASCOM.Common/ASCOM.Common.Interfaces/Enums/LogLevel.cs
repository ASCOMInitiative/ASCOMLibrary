namespace ASCOM.Common.Interfaces
{
    /// <summary>
    /// Supported log levels for ILogger devices
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// This level logs out everything. It may contain sensitive information. This will create a lot of noise in the logs and is disabled by default.
        /// </summary>
        Verbose,

        /// <summary>
        /// Log out information that is useful for debugging and development. This generally should be used temporarily.
        /// </summary>
        Debug,

        /// <summary>
        /// Log the general flow and behavior. This should typically be the default level.
        /// </summary>
        Information,

        /// <summary>
        /// Something abnormal, unexpected or bad has occurred but the application and all functions are still running
        /// </summary>
        Warning,

        /// <summary>
        /// Something has failed, however the application is still running, possibly in a degraded state. Some user intervention may be required to resume full operation.
        /// </summary>
        Error,

        /// <summary>
        /// A critical error has occurred. The application cannot recover, it has crashed or is otherwise not recoverable. It will require immediate attention.
        /// </summary>
        Fatal
    }
}
