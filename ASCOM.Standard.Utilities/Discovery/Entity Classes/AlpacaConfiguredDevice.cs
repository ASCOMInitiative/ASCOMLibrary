namespace ASCOM.Standard.Discovery
{
    /// <summary>
    /// Returns an array of device description objects, providing unique information for each served device, enabling them to be accessed through the Alpaca Device API.
    /// </summary>
    public class AlpacaConfiguredDevice
    {
        /// <summary>
        /// Create a new AlpacaConfiguredDevice with default values.
        /// </summary>
        public AlpacaConfiguredDevice()
        {
        }

        /// <summary>
        /// Create a new AlpacaConfiguredDevice with set values.
        /// </summary>
        /// <param name="deviceName">A short name for this device that a user would expect to see in a list of available devices.</param>
        /// <param name="deviceType">One of the supported ASCOM Devices types such as Telescope, Camera, Focuser etc.</param>
        /// <param name="deviceNumber">The device number that must be used to access this device through the Alpaca Device API.</param>
        /// <param name="uniqueID">This should be the ProgID for COM devices or a GUID for native Alpaca devices.</param>
        public AlpacaConfiguredDevice(string deviceName, string deviceType, int deviceNumber, string uniqueID)
        {
            DeviceName = deviceName;
            DeviceType = deviceType;
            DeviceNumber = deviceNumber;
            UniqueID = uniqueID;
        }

        /// <summary>
        /// A short name for this device that a user would expect to see in a list of available devices.
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// One of the supported ASCOM Devices types such as Telescope, Camera, Focuser etc.
        /// </summary>
        public string DeviceType { get; set; }
        /// <summary>
        /// The device number that must be used to access this device through the Alpaca Device API.
        /// </summary>
        public int DeviceNumber { get; set; }
        /// <summary>
        /// "This should be the ProgID for COM devices or a GUID for native Alpaca devices."
        /// </summary>
        public string UniqueID { get; set; }
    }
}