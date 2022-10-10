using ASCOM.Common;
using ASCOM.Common.Alpaca;
using System.Net;
using System.Net.Sockets;

namespace ASCOM.Alpaca.Discovery
{

    /// <summary>
    /// Data object describing an ASCOM device that is served by an Alpaca device as returned by the <see cref="AlpacaDiscovery"/> component.
    /// </summary>
    public class AscomDevice
    {

        #region Initialisers

        /// <summary>
        /// Initialises the class with default values
        /// </summary>
        /// <remarks>COM clients should use this initialiser and set the properties individually because COM only supports parameterless initialisers.</remarks>
        public AscomDevice()
        {
            AscomDeviceType = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ascomDdeviceName">ASCOM device name</param>
        /// <param name="ascomDeviceType">ASCOM device type</param>
        /// <param name="alpacaDeviceNumber">Alpaca API device number</param>
        /// <param name="uniqueId">ASCOM device unique ID</param>
        /// <param name="alpacaDevice">AlpacaDevice instance that servers this ASCOM device.</param>
        /// <param name="interfaceVersion">Supported Alpaca interface version</param>
        public AscomDevice(string ascomDdeviceName, DeviceTypes ascomDeviceType, int alpacaDeviceNumber, string uniqueId, AlpacaDevice alpacaDevice, int interfaceVersion) :
            this(ascomDdeviceName, ascomDeviceType, alpacaDeviceNumber, uniqueId, alpacaDevice.ServiceType, alpacaDevice.IPEndPoint, alpacaDevice.HostName, interfaceVersion,
                alpacaDevice.ServerName, alpacaDevice.Manufacturer, alpacaDevice.ManufacturerVersion, alpacaDevice.Location)
        { }


        /// <summary>
        /// Initialise the ASCOM device name, ASCOM device type and ASCOM device unique ID, plus
        /// the Alpaca API device number, unique ID, service type, device IP endpoint, Alpaca unique ID, interface version and status message
        /// </summary>
        /// <param name="ascomDdeviceName">ASCOM device name</param>
        /// <param name="ascomDeviceType">ASCOM device type</param>
        /// <param name="alpacaDeviceNumber">Alpaca API device number</param>
        /// <param name="uniqueId">ASCOM device unique ID</param>
        /// <param name="serviceType">HTTP or HTTPS service type</param>
        /// <param name="ipEndPoint">Alpaca device IP endpoint</param>
        /// <param name="hostName">ALapca device host name</param>
        /// <param name="interfaceVersion">Supported Alpaca interface version</param>
        /// <param name="serverName">ALapca device host name</param>
        /// <param name="manufacturer">ALapca device host name</param>
        /// <param name="manufacturerVersion">ALapca device host name</param>
        /// <param name="location">ALapca device host name</param>
        /// <remarks>This can only be used by .NET clients because COM only supports parameterless initialisers.</remarks>
        public AscomDevice(string ascomDdeviceName, DeviceTypes ascomDeviceType, int alpacaDeviceNumber, string uniqueId, ServiceType serviceType, IPEndPoint ipEndPoint, string hostName, int interfaceVersion, string serverName, string manufacturer, string manufacturerVersion, string location)
        {
            AscomDeviceName = ascomDdeviceName;
            AscomDeviceType = ascomDeviceType;
            AlpacaDeviceNumber = alpacaDeviceNumber;
            UniqueId = uniqueId;
            IPEndPoint = ipEndPoint;
            HostName = hostName;
            InterfaceVersion = interfaceVersion;
            ServiceType = serviceType;
            ServerName = serverName;
            Manufacturer = manufacturer;
            ManufacturerVersion = manufacturerVersion;
            Location = location;

            // Populate the IP address based on the supplied IPEndPoint value and address type
            if (ipEndPoint.AddressFamily == AddressFamily.InterNetwork) // IPv4 address
            {
                IpAddress = ipEndPoint.Address.ToString();
            }
            else if (ipEndPoint.AddressFamily == AddressFamily.InterNetworkV6) // IPv6 address so save it in canonical form
            {
                IpAddress = $"[{ipEndPoint.Address}]";
            }
            else
            {
                throw new InvalidValueException($"Unsupported network type {ipEndPoint.AddressFamily} when creating a new ASCOMDevice");
            }

            // Populate the IP port number from the supplied IPAddress
            IpPort = ipEndPoint.Port;
        }

        #endregion

        #region Public members

        /// <summary>
        /// ASCOM device name
        /// </summary>
        public string AscomDeviceName { get; set; }

        /// <summary>
        /// ASCOM device type
        /// </summary>
        public DeviceTypes? AscomDeviceType { get; set; }

        /// <summary>
        /// Alpaca API device number
        /// </summary>
        public int AlpacaDeviceNumber { get; set; }

        /// <summary>
        /// HTTP or HTTP service type
        /// </summary>
        public ServiceType ServiceType { get; set; }

        /// <summary>
        /// ASCOM device unique ID
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>
        /// The ASCOM device's DNS host name, if available, otherwise its IP address. IPv6 addresses will be in canonical form.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// The ASCOM device's IP address. IPv6 addresses will be in canonical form.
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// The ASCOM device's IP port.
        /// </summary>
        public int IpPort { get; set; }

        /// <summary>
        /// Supported Alpaca interface version
        /// </summary>
        public int InterfaceVersion { get; set; }

        /// <summary>
        /// The Alpaca device's configured name
        /// </summary>
        /// <returns>String server name</returns>
        public string ServerName { get; set; }

        /// <summary>
        /// The device manufacturer's name
        /// </summary>
        /// <returns>String manufacturer name</returns>
        public string Manufacturer { get; set; }

        /// <summary>
        /// The device's version as set by the manufacturer
        /// </summary>
        /// <returns>String server version</returns>
        public string ManufacturerVersion { get; set; }

        /// <summary>
        /// The Alpaca device's configured location
        /// </summary>
        /// <returns>String location</returns>
        public string Location { get; set; }

        #endregion

        #region Private members

        /// <summary>
        /// Alpaca device IP endpoint
        /// </summary>
        internal IPEndPoint IPEndPoint { get; set; }

        #endregion
    }
}