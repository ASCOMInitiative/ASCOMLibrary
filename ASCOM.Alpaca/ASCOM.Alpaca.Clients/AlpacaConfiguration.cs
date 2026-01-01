using ASCOM.Common.Alpaca;
using ASCOM.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// Represents all configuration parameters required to create and operate an Alpaca client.
    /// </summary>
    public class AlpacaConfiguration
    {
        /// <summary>
        /// HTTP or HTTPS. Defaults to HTTP.
        /// </summary>
        public ServiceType ServiceType { get; set; } = AlpacaClient.CLIENT_SERVICETYPE_DEFAULT;

        /// <summary>
        /// Alpaca device's IP Address. Defaults to 127.0.0.1
        /// </summary>
        public string IpAddressString { get; set; } = AlpacaClient.CLIENT_IPADDRESS_DEFAULT;

        /// <summary>
        /// Alpaca device's IP Port number. Defaults to 11111
        /// </summary>
        public int PortNumber { get; set; } = AlpacaClient.CLIENT_IPPORT_DEFAULT;

        /// <summary>
        /// Alpaca device's device number, e.g. Telescope/0. Defaults to 0.
        /// </summary>
        public int RemoteDeviceNumber { get; set; } = AlpacaClient.CLIENT_REMOTEDEVICENUMBER_DEFAULT;

        /// <summary>
        /// Timeout (seconds) to initially connect to the Alpaca device. Defaults to 5 seconds.
        /// </summary>
        public int EstablishConnectionTimeout { get; set; } = AlpacaClient.CLIENT_ESTABLISHCONNECTIONTIMEOUT_DEFAULT;

        /// <summary>
        /// Timeout (seconds) for transactions expected to complete quickly,e.g. retrieving CanXXX properties. Defaults to 10 seconds.
        /// </summary>
        public int StandardDeviceResponseTimeout { get; set; } = AlpacaClient.CLIENT_STANDARDCONNECTIONTIMEOUT_DEFAULT;

        /// <summary>
        /// Timeout (seconds) for transactions expected to take a long time, e.g. Camera.ImageArray. Defaults to 100 seconds.
        /// </summary>
        public int LongDeviceResponseTimeout { get; set; } = AlpacaClient.CLIENT_LONGCONNECTIONTIMEOUT_DEFAULT;

        /// <summary>
        /// Arbitrary integer that represents this client. Should be the same for all transactions from this client. Defaults to 1.
        /// </summary>
        public uint ClientNumber { get; set; } = AlpacaClient.CLIENT_CLIENTNUMBER_DEFAULT;

        /// <summary>
        /// Only applicable to Camera devices - Specifies the method used to retrieve the ImageArray property value. Defaults to ImageArrayTransferType.BestAvailable.
        /// </summary>
        public ImageArrayTransferType ImageArrayTransferType { get; set; } = AlpacaClient.CLIENT_IMAGEARRAYTRANSFERTYPE_DEFAULT;

        /// <summary>
        /// Only applicable to Camera devices - Extent to which the ImageArray data stream should be compressed. Defaults to ImageArrayCompression.None.
        /// </summary>
        public ImageArrayCompression ImageArrayCompression { get; set; } = AlpacaClient.CLIENT_IMAGEARRAYCOMPRESSION_DEFAULT;

        /// <summary>
        /// Basic authentication user name for the Alpaca device. Defaults to empty string.
        /// </summary>
        public string UserName { get; set; } = AlpacaClient.CLIENT_USERNAME_DEFAULT;

        /// <summary>
        /// Basic authentication password for the Alpaca device. Defaults to empty string.
        /// </summary>
        public string Password { get; set; } = AlpacaClient.CLIENT_PASSWORD_DEFAULT;

        /// <summary>
        /// Whether to tolerate or throw exceptions if the Alpaca device does not use strictly correct casing for JSON element names. Defaults to true.
        /// </summary>
        public bool StrictCasing { get; set; } = AlpacaClient.CLIENT_STRICTCASING_DEFAULT;

        /// <summary>
        /// Optional ILogger instance used to record operational information. Defaults to null.
        /// </summary>
        public ILogger Logger { get; set; } = AlpacaClient.CLIENT_LOGGER_DEFAULT;

        /// <summary>
        /// Optional product name to include in the User-Agent HTTP header. Defaults to null.
        /// </summary>
        public string UserAgentProductName { get; set; } = null;

        /// <summary>
        /// Optional product version to include in the User-Agent HTTP header. Defaults to null.
        /// </summary>
        public string UserAgentProductVersion { get; set; } = null;

        /// <summary>
        /// Whether to trust user-generated SSL certificates. Defaults to false.
        /// </summary>
        public bool TrustUserGeneratedSslCertificates { get; set; } = AlpacaClient.TRUST_USER_GENERATED_SSL_CERTIFICATES_DEFAULT;

        /// <summary>
        /// Only applicable to Telescope devices - Throw an exception if a returned JSON ITelescope.UTCDate DateTime value is not a UTC value (has a trailing Z character). Defaults to false.
        /// </summary>
        public bool ThrowOnBadDateTimeJSON { get; set; } = AlpacaClient.THROW_ON_BAD_JSON_DATE_TIME_DEFAULT;

        /// <summary>
        /// Whether to request HTTP 100-continue behaviour. Changing this to true will slow your application and may break some devices. Defaults to false.
        /// </summary>
        /// <remarks>100-CONTINUE HTTP behaviour was unintentionally enabled in Library versions 2.x but adds no value in ASCOM's use. It was disabled in Library 3.x and onward. This option 
        /// is included in case any developers have made devices that depend on the client requesting 100-CONTINUE behaviour.
        /// See https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/100-continue for more information about HTTP 100-continue behaviour.</remarks>
        public bool Request100Continue { get; set; } = AlpacaClient.CLIENT_REQUEST_100_CONTINUE_DEFAULT;
    }
}
