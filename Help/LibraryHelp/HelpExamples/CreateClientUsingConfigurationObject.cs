using ASCOM.Alpaca.Clients;
using ASCOM.Common.Alpaca;
using ASCOM.Tools;

namespace HelpExamples
{
    internal class CreateClientUsingConfigurationObjectClass
    {
        internal static void CreateClientUsingConfigurationObject()
        {
            try
            {
                #region Create client with configuration object
                // Create a trace logger
                using (TraceLogger logger = new TraceLogger("ClientConfiguration", true))
                {
                    // Create the configuration object as required
                    AlpacaConfiguration configuration = new()
                    {
                        ServiceType = ServiceType.Http,
                        IpAddressString = "127.0.0.1",
                        PortNumber = 11111,
                        RemoteDeviceNumber = 0,
                        EstablishConnectionTimeout = 3,
                        StandardDeviceResponseTimeout = 5,
                        LongDeviceResponseTimeout = 100,
                        ClientNumber = 34892,
                        ImageArrayCompression = ImageArrayCompression.None,
                        ImageArrayTransferType = ImageArrayTransferType.BestAvailable,
                        UserName = "QuY89",
                        Password = "YYu8*9jK",
                        StrictCasing = true,
                        Logger = logger,
                        Request100Continue = false,
                        ThrowOnBadDateTimeJSON = true,
                        UserAgentProductName = "ASCOM.HelpExamples",
                        UserAgentProductVersion = "1.0",
                        TrustUserGeneratedSslCertificates = false
                    };

                    // Create the camera Alpaca Client with specified configuration
                    using (AlpacaCamera cameraClient = AlpacaClient.GetDevice<AlpacaCamera>(configuration))
                    {
                        // Connect to the Alpaca device
                        cameraClient.Connected = true;

                        // Record some information
                        logger.LogMessage("ClientConfiguration", $"Found device: {cameraClient.Name} - Driver: {cameraClient.DriverInfo}, Version: {cameraClient.DriverVersion} Camera state: {cameraClient.CameraState}.");

                        // Disconnect from the camera
                        cameraClient.Connected = false;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
    }
}
