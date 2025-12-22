using ASCOM.Common.Alpaca;
using ASCOM.Common.Interfaces;
using ASCOM.Common;
using System;
using System.Reflection;
using System.Net.Http.Headers;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// Class presenting client specific configuration
    /// </summary>
    public class ClientConfiguration
    {
        // Pointer to the device base class so that its properties can be exposed and manipulated
        private readonly AlpacaDeviceBaseClass baseClass;
        
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

       /// <summary>
       /// Product name to include in the User-Agent HTTP header
       /// </summary>
        public string UserAgentProductName
        {
            get { return baseClass.userAgentProductName; }
            set
            {
                baseClass.userAgentProductName = value;
                baseClass.client.DefaultRequestHeaders.Clear();

                string productName = value;
                string productVersion = UserAgentProductVersion;

                // Make sure that the User-Agent product name has some content
                if (string.IsNullOrEmpty(productName))
                {
                    productName = "ASCOMLibrary";
                }

                // Make sure that the User-Agent product version has some content
                if (string.IsNullOrEmpty(productVersion))
                {
                    productVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
                }

                // Add the User-Agent header
                baseClass.client.DefaultRequestHeaders.UserAgent.Clear();
                baseClass.client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue(productName, productVersion)));
            }
        }

        /// <summary>
        /// Product version to include in the User-Agent HTTP header
        /// </summary>
        public string UserAgentProductVersion
        {
            get { return baseClass.userAgentProductVersion; }
            set 
            {
                baseClass.userAgentProductVersion = value;
                baseClass.client.DefaultRequestHeaders.Clear();

                string productName = UserAgentProductName;
                string productVersion = value;

                // Make sure that the User-Agent product name has some content
                if (string.IsNullOrEmpty(productName))
                {
                    productName = "ASCOMLibrary";
                }

                // Make sure that the User-Agent product version has some content
                if (string.IsNullOrEmpty(productVersion))
                {
                    productVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
                }

                // Add the User-Agent header
                baseClass.client.DefaultRequestHeaders.UserAgent.Clear();
                baseClass.client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue(productName, productVersion)));
            }
        }

        /// <summary>
        /// Image array compression level (Camera specific)
        /// </summary>
        public ImageArrayCompression ImageArrayCompression
        {
            get { return baseClass.imageArrayCompression; }
            set { baseClass.imageArrayCompression = value; }
        }

        /// <summary>
        /// Image array transfer type (Camera specific)
        /// </summary>
        public ImageArrayTransferType ImageArrayTransferType
        {
            get { return baseClass.imageArrayTransferType; }
            set { baseClass.imageArrayTransferType = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the client sends an Expect: 100-continue header with HTTP requests.
        /// </summary>
        /// <remarks>When this property is set to <see langword="true"/>, the client will wait for a
        /// 100-Continue response from the server before sending the request body. This can improve efficiency when
        /// sending large payloads, as it avoids sending data if the server is likely to reject the request. Not all
        /// servers support the Expect: 100-continue mechanism.</remarks>
        public bool Request100Continue
        {
            get { return baseClass.request100Continue; }
            set { baseClass.request100Continue = value; }
        }
    }
}
