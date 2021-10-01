namespace ASCOM.Alpaca
{
    /// <summary>
    /// Describes available connection service types used by the Alpaca Clients and by the Discovery component
    /// </summary>
    /// <remarks>The Enum codes must be valid service types because they are converted to strings and used directly in device URLs of form: {ServiceType}://{Host}:{Port}/api...}in the </remarks>
    public enum ServiceType
    {
        Http = 0, // Unencrypted HTTP protocol
        Https = 1 // Encrypted HTTPS protocol
    }
}
