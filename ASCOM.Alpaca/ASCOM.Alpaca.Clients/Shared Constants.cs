using ASCOM.Common.Alpaca;

namespace ASCOM.Alpaca.Clients
{
    internal static class SharedConstants
    {
        // Regular expressions to validate IP addresses and host names
        public const string ValidIpAddressRegex = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
        public const string ValidHostnameRegex = @"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$";

        // HTTP Parameter names shared by driver and remote device
        public const string RA_PARAMETER_NAME = "RightAscension";
        public const string DEC_PARAMETER_NAME = "Declination";
        public const string ALT_PARAMETER_NAME = "Altitude";
        public const string AZ_PARAMETER_NAME = "Azimuth";
        public const string AXIS_PARAMETER_NAME = "Axis";
        public const string RATE_PARAMETER_NAME = "Rate";
        public const string DIRECTION_PARAMETER_NAME = "Direction";
        public const string DURATION_PARAMETER_NAME = "Duration";
        public const string CLIENTID_PARAMETER_NAME = "ClientID";
        public const string CLIENTTRANSACTION_PARAMETER_NAME = "ClientTransactionID";
        public const string COMMAND_PARAMETER_NAME = "Command";
        public const string RAW_PARAMETER_NAME = "Raw";
        public const string LIGHT_PARAMETER_NAME = "Light";
        public const string ACTION_COMMAND_PARAMETER_NAME = "Action";
        public const string ACTION_PARAMETERS_PARAMETER_NAME = "Parameters";
        public const string ID_PARAMETER_NAME = "ID";
        public const string STATE_PARAMETER_NAME = "State";
        public const string NAME_PARAMETER_NAME = "Name";
        public const string VALUE_PARAMETER_NAME = "Value";
        public const string POSITION_PARAMETER_NAME = "Position";
        public const string SIDEOFPIER_PARAMETER_NAME = "SideOfPier";
        public const string UTCDATE_PARAMETER_NAME = "UTCDate";
        public const string SENSORNAME_PARAMETER_NAME = "SensorName";
        public const string BRIGHTNESS_PARAMETER_NAME = "Brightness";

        public const string ISO8601_DATE_FORMAT_STRING = "yyyy-MM-ddTHH:mm:ss.fffffff";

        // Dynamic client configuration constants
        public const int SOCKET_ERROR_MAXIMUM_RETRIES = 2; // The number of retries that the client will make when it receives a socket actively refused error from the remote device
        public const int SOCKET_ERROR_RETRY_DELAY_TIME = 1000; // The delay time (milliseconds) between socket actively refused retries

        // remote device setup form constants
        public const string LOCALHOST_NAME_IPV4 = "localhost";
        public const string LOCALHOST_ADDRESS_IPV4 = "127.0.0.1"; // Get the localhost loop back address
        public const string STRONG_WILDCARD_NAME = "+"; // Get the localhost loop back address

        // Constants shared by Remote Client Drivers and the ASCOM remote device
        public const string API_URL_BASE = "/api/"; // This constant must always be lower case to make the logic tests work properly 
        public const string API_VERSION_V1 = "v1"; // This constant must always be lower case to make the logic tests work properly

        // Default image array transfer constants
        public const ImageArrayCompression IMAGE_ARRAY_COMPRESSION_DEFAULT = ImageArrayCompression.None;
        public const ImageArrayTransferType IMAGE_ARRAY_TRANSFER_TYPE_DEFAULT = ImageArrayTransferType.Base64HandOff;

        // Image array base64 hand-off and ImageBytes support constants
        public const string BASE64_HANDOFF_HEADER = "base64handoff"; // Name of HTTP header used to affirm binary serialisation support for image array data
        public const string BASE64_HANDOFF_SUPPORTED = "true"; // Value of HTTP header to indicate support for binary serialised image array data
        public const string BASE64_HANDOFF_FILE_DOWNLOAD_URI_EXTENSION = "base64"; // Addition to the ImageArray and ImageArrayVariant method names from which base64 serialised image files can be downloaded
        public const string IMAGE_ARRAY_METHOD_NAME = "ImageArray";

        // Image array ImageBytes - basic mime-type values
        public const string ACCEPT_HEADER_NAME = "Accept"; // Name of HTTP header used to affirm ImageBytes support for image array data
        public const string CONTENT_TYPE_HEADER_NAME = "Content-Type"; // Name of HTTP header used to affirm the type of data returned by the device
        public const string IMAGE_BYTES_MIME_TYPE = "application/imagebytes"; // Image bytes mime type
        public const string APPLICATION_JSON_MIME_TYPE = "application/json"; // Application/json mime type
        public const string TEXT_JSON_MIME_TYPE = "text/json"; // Text/json mime type

        // Image array ImageBytes - combined mime-type values
        public const string JSON_MIME_TYPES = APPLICATION_JSON_MIME_TYPE + ", " + TEXT_JSON_MIME_TYPE; // JSON mime types
        public const string IMAGE_BYTES_ACCEPT_HEADER = IMAGE_BYTES_MIME_TYPE + ", " + JSON_MIME_TYPES; // Value of HTTP header to indicate support for the GetImageBytes mechanic

    }
}
