
// This .NET 6 Console project example deliberately avoids using statements to illustrate use of the ASCOM Library namespaces
// It also assumes that the operating system is Windows

// Requires ASCOM Library packages: ASCOM.Alpaca.Components and ASCOM.COM.Components

try
{
    // Define an IRotatorV3 variable that can hold both an Alpaca toolkit object and a COM client toolkit object
    ASCOM.Common.DeviceInterfaces.IRotatorV3 rotator;

    // Define a COM client toolkit object
    ASCOM.Com.DriverAccess.Rotator comRotator;

    // Define an Alpaca client toolkit object
    ASCOM.Alpaca.Clients.AlpacaRotator alpacaRotator;

    // Create and activate a TraceLogger device to receive operational information from the Library components
    ASCOM.Tools.TraceLogger TL = new ASCOM.Tools.TraceLogger("ToolkitTester", true);
    TL.SetMinimumLoggingLevel(ASCOM.Common.Interfaces.LogLevel.Verbose);
    TL.LogMessage("Main","Created trace logger OK");

    // Set arbitrary Alpaca device configuration values
    string deviceIpAddress = "127.0.0.1"; // Device IP address
    int deviceIpPort = 11111; // Device's IP port number
    int remoteDeviceNumber = 0; // This rotator's device number on the Alpaca device
    bool strictCasing = true; // Require the decide to strictly adhere to the Alpaca protocol name casing rules
    Console.WriteLine($"Chosen Alpaca device: {deviceIpAddress}:{deviceIpPort}/api/v1/rotator/{remoteDeviceNumber}");

    // Create an Alpaca client toolkit object to represent an Alpaca device (assumes that the device has already been discovered)
    alpacaRotator = new ASCOM.Alpaca.Clients.AlpacaRotator(ASCOM.Common.Alpaca.ServiceType.Http, deviceIpAddress, deviceIpPort, remoteDeviceNumber, strictCasing, TL);

    // Assign the Alpaca client object to the IRotatorV3 variable and use it 
    rotator = alpacaRotator;
    rotator.Connected = true;
    string driverVersion = rotator.DriverVersion;
    Console.WriteLine($"Alpaca device driver version: {driverVersion}\r\n");

    rotator.Connected = false;
    rotator.Dispose();

    // Set arbitrary COM device configuration value
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
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}