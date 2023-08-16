internal class SeamlessClientAccessClass
{
    internal static void SeamlessClientAccess()
    {
        #region SeamlessClientAccess
        // This example intentionally avoids using statements to illustrate use of the ASCOM Library namespaces

        // Define an IRotatorV3 variable that can hold both an Alpaca toolkit object and a COM client toolkit object
        ASCOM.Common.DeviceInterfaces.IRotatorV3 rotator;

        // Define a COM client toolkit object
        ASCOM.Com.DriverAccess.Rotator comRotator;

        // Define an Alpaca client toolkit object
        ASCOM.Alpaca.Clients.AlpacaRotator alpacaRotator;

        // Create and activate a TraceLogger device to receive operational information from the Library components
        ASCOM.Tools.TraceLogger logger = new ASCOM.Tools.TraceLogger("ToolkitTester", true);
        logger.SetMinimumLoggingLevel(ASCOM.Common.Interfaces.LogLevel.Verbose);
        logger.LogMessage("Main", "Created trace logger OK");

        // Set some arbitrary Alpaca device configuration values
        string deviceIpAddress = "127.0.0.1"; // Device IP address
        int deviceIpPort = 11111; // Device's IP port number
        int remoteDeviceNumber = 0; // This rotator's device number on the Alpaca device
        bool strictCasing = true; // Require the decide to strictly adhere to the Alpaca protocol name casing rules
        Console.WriteLine($"Chosen Alpaca device: {deviceIpAddress}:{deviceIpPort}/api/v1/rotator/{remoteDeviceNumber}");

        // Create an Alpaca client to represent the an Alpaca device
        alpacaRotator = new ASCOM.Alpaca.Clients.AlpacaRotator(ASCOM.Common.Alpaca.ServiceType.Http, deviceIpAddress, deviceIpPort, remoteDeviceNumber, strictCasing, logger);

        // Assign the Alpaca client object to the IRotatorV3 variable and use it 
        rotator = alpacaRotator;
        rotator.Connected = true;
        string driverVersion = rotator.DriverVersion;
        Console.WriteLine($"Alpaca device driver version: {driverVersion}\r\n");

        rotator.Connected = false;
        rotator.Dispose();

        // Create an arbitrary COM device ProgID
        string deviceProgId = "ASCOM.Simulator.Rotator";
        Console.WriteLine($"Chosen COM device: {deviceProgId}");

        // Create a COM client toolkit object to represent the rotator driver
        comRotator = new ASCOM.Com.DriverAccess.Rotator(deviceProgId);

        // Assign the COM client object to the IRotatorV3 variable and use it 
        rotator = comRotator;
        rotator.Connected = true;
        string description = rotator.Description;
        Console.WriteLine($"COM device description: {description}");

        rotator.Connected = false;
        rotator.Dispose();
        #endregion
    }
}