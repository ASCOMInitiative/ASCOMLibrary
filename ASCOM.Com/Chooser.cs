using ASCOM.Common;
using ASCOM.Common.Interfaces;
using System;
using System.Runtime.InteropServices;

namespace ASCOM.Com
{
    /// <summary>
    /// The Chooser provides a way for your application to let your user select which device to use.
    /// </summary>
    /// <remarks>
    /// This component is a light wrapper for the Platform Chooser COM component. In time it will be re-written as a native .NET Core component
    /// </remarks>
    public class Chooser : IDisposable
    {
        // COM ProgID of the Platform Chooser component
        private const string CHOOSER_PROGID = "ASCOM.Utilities.Chooser";

        private readonly dynamic chooser; // Holds the Chooser COM object reference
        private bool disposedValue; // Indicates whether the object has been Disposed
        private readonly ILogger logger;

        #region New and Dispose

        /// <summary>
        /// Creates a new Chooser object with no logger
        /// </summary>
        public Chooser() : this(null)
        {
        }

        /// <summary>
        /// Creates a new Chooser object with a logger
        /// </summary>
        /// <param name="logger">Optional ILogger object to which operational messages will be sent by the Chooser component.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public Chooser(ILogger logger)
        {
            // SAve the supplied logger (if any)
            this.logger = logger;

            // This will only work on Windows so validate the OS here
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                LogMessage(LogLevel.Error, "Initialise", $"Chooser is only supported on Windows, throwing InvalidOperationException. This OS is: {RuntimeInformation.OSDescription}");
                throw new InvalidOperationException($"Chooser.Initialiser - Chooser is only supported on Windows. This OS is: {RuntimeInformation.OSDescription}");
            }

            // Get the Chooser's Type from its ProgID
            LogMessage(LogLevel.Debug, "Initialise", $"About to get Chooser type");
            Type chooserType = Type.GetTypeFromProgID(CHOOSER_PROGID);


            // Create a Chooser COM object and save the reference to the chooser variable
            LogMessage(LogLevel.Debug, "Initialise", $"FOund type {chooserType.FullName}.");
            chooser = Activator.CreateInstance(chooserType);
            LogMessage(LogLevel.Debug, "Initialise", $"Created Chooser OK.");
        }
        /// <summary>
        /// Chooser destructor, called by the runtime during garbage collection
        /// </summary>
        /// <remarks>This method ensures that Dispose is called during garbage collection even if it has not been called it from the application.</remarks>
        ~Chooser()
        {
            // Do not change this code. Put clean-up code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        /// <summary>
        /// Dispose of this components Chooser object.
        /// </summary>
        /// <param name="disposing">True if called by the application, false if called by the garbage collector.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // No objects to dispose here.
                    // The logger will be released by the calling application rather than us
                }

                // Dispose of the Chooser COM object, ensuring that no exception is thrown
                try { Marshal.ReleaseComObject(chooser); } catch { }

                // Flag that Dispose has been called and the resources have been released
                disposedValue = true;
            }
        }

        /// <summary>
        /// Release the Chooser component's Chooser COM object
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put clean-up code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// The type of device from which the Chooser will select a driver. (default = "Telescope") 
        /// </summary>
        public DeviceTypes DeviceType
        {
            get
            {
                CheckOK("DeviceType Get");

                // Get the current device type from the Chooser as a string
                string deviceTypeString = chooser.DeviceType;

                // Convert the string to its equivalent DeviceTypes enum value
                DeviceTypes deviceTypeEnum = Devices.StringToDeviceType(deviceTypeString);


                LogMessage(LogLevel.Debug, "DeviceType", $"Returning Enum: {deviceTypeEnum}, String: {deviceTypeString}");
                return deviceTypeEnum;
            }
            set
            {
                CheckOK("DeviceType Set");

                // Convert the supplied device type to its string value

                string deviceTypeString = Devices.DeviceTypeToString(value);
                LogMessage(LogLevel.Debug, "DeviceType", $"Setting string value: {deviceTypeString}, Enum: {value}");

                chooser.DeviceType = deviceTypeString;
            }
        }

        /// <summary>
        /// Select the ASCOM driver to use without pre-selecting one in the drop-down list 
        /// </summary>
        /// <returns>The ProgID of the selected device or an empty string if no device was chosen</returns>
        public string Choose()
        {
            CheckOK("Choose(\"\")");

            string progId = chooser.Choose("Telescope");
            LogMessage(LogLevel.Debug, "Choose", $"Returning: {((progId == null) ? "Null - No device selected" : progId)}.");
            return progId;
        }

        /// <summary>
        /// Display the Chooser dialogue enabling the user to select a driver
        /// </summary>
        /// <param name="progId">The driver ProgId to pre-select in the Chooser drop-down list</param>
        /// <returns>The ProgID of the selected device or an empty string if no device was chosen</returns>
        public string Choose(string progId)
        {
            CheckOK($"Choose(\"{progId})\"");
            string newProgId = chooser.Choose(progId);
            LogMessage(LogLevel.Debug, "Choose", $"Returning: {((newProgId == null) ? "Null - No device selected" : newProgId)}.");
            return newProgId;
        }

        #endregion

        #region Support code
        /// <summary>
        /// Validate that this object has not been disposed. If this component has been disposed, throw an InvalidOperationException.
        /// </summary>
        /// <param name="method">Name of the called method</param>
        /// <exception cref="InvalidOperationException">When the Chooser has already been disposed.</exception>
        private void CheckOK(string method)
        {
            if (disposedValue)
            {
                throw new InvalidOperationException($"Cannot call Chooser.{method} because it has been disposed.");
            }

            if (chooser == null)
            {
                throw new InvalidOperationException($"Cannot call Chooser.{method} because the Chooser object is null.");
            }

        }

        /// <summary>
        /// Log a message, dealing with the possibility that the logger is null
        /// </summary>
        /// <param name="level">Log level</param>
        /// <param name="method">Calling method name</param>
        /// <param name="message">Message</param>
        private void LogMessage(LogLevel level, string method, string message)
        {
            logger.LogMessage(level, $"Chooser - {method}", message);
        }

        #endregion

    }
}
