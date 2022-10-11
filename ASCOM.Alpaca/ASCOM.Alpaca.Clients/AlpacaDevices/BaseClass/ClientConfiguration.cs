using ASCOM.Common.Alpaca;
using ASCOM.Common.Interfaces;
using ASCOM.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// Class presenting client specific configuration
    /// </summary>
    public class ClientConfiguration
    {
        private  readonly AlpacaDeviceBaseClass baseClass; // Pointer to the device base class so that its properties can be exposed and manipulated
        
        /// <summary>
        /// Initialise the configuration class, saving the supplied base class reference for use within the class
        /// </summary>
        /// <param name="baseClass">Base class reference for this device</param>
        internal ClientConfiguration(AlpacaDeviceBaseClass baseClass)
        {
            this.baseClass=baseClass;
        }

        /// <summary>
        /// HTTP or HTTPS service type used to communicate with the Alpaca device
        /// </summary>
        public ServiceType ServiceType
        {
            get { return baseClass.serviceType; }
            set { baseClass.serviceType = value; }
        }

        /// <summary>
        /// Alpaca device IP address as a string
        /// </summary>
        public string IpAddress
        {
            get { return baseClass.ipAddressString; }
            set { baseClass.ipAddressString = value; }
        }

        /// <summary>
        /// Alpaca device IP port number.
        /// </summary>
        public int PortNumber
        {
            get { return Convert.ToInt32(baseClass.portNumber); }
            set { baseClass.portNumber = value; }
        }

        /// <summary>
        /// Alpaca device number of the ASCOM device
        /// </summary>
        public int RemoteDeviceNumber
        {
            get { return Convert.ToInt32(baseClass.remoteDeviceNumber); }
            set { baseClass.remoteDeviceNumber = value; }
        }

        /// <summary>
        /// Communications timeout (seconds) when initially connecting to the client
        /// </summary>
        public int EstablishConnectionTimeout
        {
            get { return baseClass.establishConnectionTimeout; }
            set { baseClass.establishConnectionTimeout = value; }
        }

        /// <summary>
        /// Communications timeout (seconds) for commands that are expected to complete quickly
        /// </summary>
        public int StandardDeviceResponseTimeout
        {
            get { return baseClass.standardDeviceResponseTimeout; }
            set { baseClass.standardDeviceResponseTimeout = value; }
        }

        /// <summary>
        /// Communications timeout (seconds) for commands that are expected to take a long time such as synchronous telescope slews.
        /// </summary>
        public int LongDeviceResponseTimeout
        {
            get { return baseClass.longDeviceResponseTimeout; }
            set { baseClass.longDeviceResponseTimeout = value; }
        }

        /// <summary>
        /// Arbitrary number identifying this particular client
        /// </summary>
        public uint ClientNumber
        {
            get { return baseClass.clientNumber; }
            set { baseClass.clientNumber = value; }
        }

        /// <summary>
        /// Basic authentication user name.
        /// </summary>
        /// <remarks>Supply a <see langword="null"/> or empty string if basic authentication is not required</remarks>
        public string UserName
        {
            get { return baseClass.userName; }
            set { baseClass.userName = value; }
        }

        /// <summary>
        /// Basic authentication password
        /// </summary>
        /// <remarks>Ignored if password is <see langword="null"/> or an empty string</remarks>
        public string Password
        {
            get { return baseClass.password; }
            set { baseClass.password = value; }
        }

        /// <summary>
        /// Set <see langword="true"/> to require that clients case JSON variables in accordance with the Alpaca standard. Set <see langword="false"/> to accept any casing.
        /// </summary>
        public bool StrictCasing
        {
            get { return baseClass.strictCasing; }
            set { baseClass.strictCasing = value; }
        }

        /// <summary>
        /// Device type of this client
        /// </summary>
        public DeviceTypes DeviceType
        {
            get { return baseClass.clientDeviceType; }
        }

        /// <summary>
        /// ILogger instance to which operational debug messages will be sent.
        /// </summary>
        /// <remarks>Supply a <see langword="null"/> value to suppress operational logging.</remarks>
        public ILogger Logger
        {
            get { return baseClass.logger; }
            set { baseClass.logger = value; }
        }

    }
}
