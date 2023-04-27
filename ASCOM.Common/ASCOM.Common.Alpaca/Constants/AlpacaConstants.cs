namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// Constants used in Alpaca JSON messages and clients
    /// </summary>
    public static class AlpacaConstants
    {
        // HTTP Parameter names shared by driver and remote device

        /// <summary>
        /// RightAscension HTTP parameter name
        /// </summary>
        public const string RA_PARAMETER_NAME = "RightAscension";

        /// <summary>
        /// Declination HTTP parameter name
        /// </summary>
        public const string DEC_PARAMETER_NAME = "Declination";

        /// <summary>
        /// Altitude HTTP parameter name
        /// </summary>
        public const string ALT_PARAMETER_NAME = "Altitude";

        /// <summary>
        /// Azimuth HTTP parameter name
        /// </summary>
        public const string AZ_PARAMETER_NAME = "Azimuth";

        /// <summary>
        /// Axis HTTP parameter name
        /// </summary>
        public const string AXIS_PARAMETER_NAME = "Axis";

        /// <summary>
        /// Rate HTTP parameter name
        /// </summary>
        public const string RATE_PARAMETER_NAME = "Rate";

        /// <summary>
        /// Direction HTTP parameter name
        /// </summary>
        public const string DIRECTION_PARAMETER_NAME = "Direction";

        /// <summary>
        /// Duration HTTP parameter name
        /// </summary>
        public const string DURATION_PARAMETER_NAME = "Duration";

        /// <summary>
        /// ClientID HTTP parameter name
        /// </summary>
        public const string CLIENTID_PARAMETER_NAME = "ClientID";

        /// <summary>
        /// ClientTransactionID HTTP parameter name
        /// </summary>
        public const string CLIENTTRANSACTION_PARAMETER_NAME = "ClientTransactionID";

        /// <summary>
        /// Command HTTP parameter name
        /// </summary>
        public const string COMMAND_PARAMETER_NAME = "Command";

        /// <summary>
        /// Raw HTTP parameter name
        /// </summary>
        public const string RAW_PARAMETER_NAME = "Raw";

        /// <summary>
        /// Light HTTP parameter name
        /// </summary>
        public const string LIGHT_PARAMETER_NAME = "Light";

        /// <summary>
        /// Action HTTP parameter name
        /// </summary>
        public const string ACTION_COMMAND_PARAMETER_NAME = "Action";

        /// <summary>
        /// Parameters HTTP parameter name
        /// </summary>
        public const string ACTION_PARAMETERS_PARAMETER_NAME = "Parameters";

        /// <summary>
        /// ID HTTP parameter name
        /// </summary>
        public const string ID_PARAMETER_NAME = "Id";
        /// <summary>
        /// State HTTP parameter name
        /// </summary>
        public const string STATE_PARAMETER_NAME = "State";

        /// <summary>
        /// Name HTTP parameter name
        /// </summary>
        public const string NAME_PARAMETER_NAME = "Name";

        /// <summary>
        /// Value HTTP parameter name
        /// </summary>
        public const string VALUE_PARAMETER_NAME = "Value";

        /// <summary>
        /// Position HTTP parameter name
        /// </summary>
        public const string POSITION_PARAMETER_NAME = "Position";

        /// <summary>
        /// SideOfPier HTTP parameter name
        /// </summary>
        public const string SIDEOFPIER_PARAMETER_NAME = "SideOfPier";

        /// <summary>
        /// UTCDate HTTP parameter name
        /// </summary>
        public const string UTCDATE_PARAMETER_NAME = "UTCDate";
        /// <summary>
        /// SensorName HTTP parameter name
        /// </summary>
        public const string SENSORNAME_PARAMETER_NAME = "SensorName";
        /// <summary>
        /// Brightness HTTP parameter name
        /// </summary>
        public const string BRIGHTNESS_PARAMETER_NAME = "Brightness";

        /// <summary>
        /// Format string to create a date in ISO 8601 format
        /// </summary>
        public const string ISO8601_DATE_FORMAT_STRING = "yyyy-MM-ddTHH:mm:ss.fffffff";

        // Constants shared by Remote Client Drivers and the ASCOM remote device

        /// <summary>
        /// Alpaca API URL base string
        /// </summary>
        public const string API_URL_BASE = "api/"; // This constant must always be lower case to make the logic tests work properly. A leading / is not required because it is supplied by the client

        /// <summary>
        /// Alpaca API version number
        /// </summary>
        public const string API_VERSION_V1 = "v1"; // This constant must always be lower case to make the logic tests work properly.

        // Image array base64 hand-off and ImageBytes support constants

        /// <summary>
        /// Name of HTTP header used to affirm base64 binary serialisation support for image array data
        /// </summary>
        public const string BASE64_HANDOFF_HEADER = "base64handoff"; // Name of HTTP header used to affirm binary serialisation support for image array data

        /// <summary>
        /// Name of the HTTP header used to affirm base64 binary serialisation support for image array data
        /// </summary>
        public const string BASE64_HANDOFF_SUPPORTED = "true"; // Value of HTTP header to indicate support for binary serialised image array data
        /// <summary>
        /// Postpend to the ImageArray and ImageArrayVariant method names to form method names from which base64 serialised image files can be downloaded
        /// </summary>
        public const string BASE64_HANDOFF_FILE_DOWNLOAD_URI_EXTENSION = "base64"; // Addition to the ImageArray and ImageArrayVariant method names from which base64 serialised image files can be downloaded

        // Image array ImageBytes - basic mime-type values

        /// <summary>
        /// Mime type indicating an image bytes response
        /// </summary>
        public const string IMAGE_BYTES_MIME_TYPE = "application/imagebytes"; // Image bytes mime type

        /// <summary>
        /// Mime type indicating a JSON response
        /// </summary>
        public const string APPLICATION_JSON_MIME_TYPE = "application/json"; // Application/json mime type

    }
}
