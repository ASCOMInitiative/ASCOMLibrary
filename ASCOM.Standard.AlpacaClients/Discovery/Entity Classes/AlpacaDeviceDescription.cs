namespace ASCOM.Alpaca.Discovery
{
    /// <summary>
    /// Returns cross-cutting information that applies to all devices available at this URL:Port.
    /// </summary>
    public class AlpacaDeviceDescription
    {
        /// <summary>
        /// Create a new AlpacaDeviceDescription with default values.
        /// </summary>
        public AlpacaDeviceDescription()
        {
        }

        /// <summary>
        ///  Create a new AlpacaDeviceDescription with set values.
        /// </summary>
        /// <param name="serverName">The device or server's overall name.</param>
        /// <param name="manufacturer">The manufacturer's name.</param>
        /// <param name="manufacturerVersion">The device or server's version number.</param>
        /// <param name="location">The device or server's location.</param>
        public AlpacaDeviceDescription(string serverName, string manufacturer, string manufacturerVersion, string location)
        {
            ServerName = serverName;
            Manufacturer = manufacturer;
            ManufacturerVersion = manufacturerVersion;
            Location = location;
        }

        /// <summary>
        /// The device or server's overall name.
        /// </summary>
        public string ServerName { get; set; } = string.Empty;
        /// <summary>
        /// The manufacturer's name.
        /// </summary>
        public string Manufacturer { get; set; } = string.Empty;
        /// <summary>
        /// The device or server's version number.
        /// </summary>
        public string ManufacturerVersion { get; set; } = string.Empty;
        /// <summary>
        /// The device or server's location.
        /// </summary>
        public string Location { get; set; } = string.Empty;
    }
}