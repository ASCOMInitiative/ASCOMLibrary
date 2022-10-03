#region Seamless client access
// This .NET 6 Console project example intentionally avoids using statements to illustrate use of the ASCOM Library namespaces
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
    TL.LogMessage("Main", "Created trace logger OK");

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
#endregion

#region Synchronous Discovery
// This .NET 6 Console project example intentionally avoids using statements to illustrate use of the ASCOM Library namespaces
// It also assumes that the operating system is Windows

// Requires ASCOM Library packages: ASCOM.Alpaca.Components and ASCOM.COM.Components

try
{
    // Create a Discovery component that can be used for one or more discoveries
    using (ASCOM.Alpaca.Discovery.AlpacaDiscovery alpacaDiscovery = new ASCOM.Alpaca.Discovery.AlpacaDiscovery())
    {
        // Initiate a discovery using these parameters:
        //   Send 2 discovery poll packets
        //   Leave a 100ms gap between polls 
        //   Send the polls on the standard Alpaca discovery port 32227
        //   Wait 1.5 seconds before sopping the discovery
        //   Don't attempt to resolve IP addresses to DNS names
        //   Send discovery packets as IPv4 broadcasts
        //   Don't send any IPv6 multi-cast Alpaca discovery packets
        //   Communicate with Alpaca devices using HTTP
        alpacaDiscovery.StartDiscovery(2, 100, 32227, 1.5, false, true, false, ASCOM.Common.Alpaca.ServiceType.Http);

        // Wait for the discovery to complete, testing the completion variable every 50ms.
        do
        {
            Thread.Sleep(50);
        } while (!alpacaDiscovery.DiscoveryComplete);

        // Print a list of available ASCOM FilterWheel devices
        List<ASCOM.Alpaca.Discovery.AscomDevice> filterWheelDevices = alpacaDiscovery.GetAscomDevices(ASCOM.Common.DeviceTypes.FilterWheel);
        Console.WriteLine($"Found {filterWheelDevices.Count} FilterWheel devices.");
        foreach (ASCOM.Alpaca.Discovery.AscomDevice ascomDevice in filterWheelDevices)
        {
            Console.WriteLine($"Found device: {ascomDevice.AscomDeviceName} at {ascomDevice.IpAddress}:{ascomDevice.IpPort}.");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}
#endregion

#region Asynchronous Discovery
// This .NET 6 Console project example intentionally avoids using statements to illustrate use of the ASCOM Library namespaces
// It also assumes that the operating system is Windows

// Requires ASCOM Library packages: ASCOM.Alpaca.Components and ASCOM.COM.Components

try
{
    // Create a Discovery component that can be used for one or more discoveries
    ASCOM.Alpaca.Discovery.AlpacaDiscovery alpacaDiscovery = new ASCOM.Alpaca.Discovery.AlpacaDiscovery();

    // Add an event handler that will be called when discovery is complete.
    alpacaDiscovery.DiscoveryCompleted += DiscoveryCompletedEventHandler;

    // Initiate a discovery using these parameters:
    //   Send 2 discovery poll packets
    //   Leave a 100ms gap between polls 
    //   Send the polls on the standard Alpaca discovery port 32227
    //   Wait 1.5 seconds before sopping the discovery
    //   Don't attempt to resolve IP addresses to DNS names
    //   Send discovery packets as IPv4 broadcasts
    //   Don't send any IPv6 multi-cast Alpaca discovery packets
    //   Communicate with Alpaca devices using HTTP
    alpacaDiscovery.StartDiscovery(2, 100, 32227, 1.5, false, true, false, ASCOM.Common.Alpaca.ServiceType.Http);

    // Continue with other processing while the discovery is running
    // The DiscoveryHasCompleted method will be called when the discovery completes

    //TODO: Dispose of the AlpacaDiscovery object when it is no longer required
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

///<summary>
///Event handler called when the configured discovery time is reached
///</summary>
void DiscoveryCompletedEventHandler(object? sender, EventArgs e)
{
    try
    {
        if (sender is not null)
        {
            ASCOM.Alpaca.Discovery.AlpacaDiscovery alpacaDiscovery = (ASCOM.Alpaca.Discovery.AlpacaDiscovery)sender;
            // Print a list of available ASCOM FilterWheel devices
            List<ASCOM.Alpaca.Discovery.AscomDevice> filterWheelDevices = alpacaDiscovery.GetAscomDevices(ASCOM.Common.DeviceTypes.FilterWheel);
            Console.WriteLine($"Found {filterWheelDevices.Count} FilterWheel devices.");
            foreach (ASCOM.Alpaca.Discovery.AscomDevice ascomDevice in filterWheelDevices)
            {
                Console.WriteLine($"Found device: {ascomDevice.AscomDeviceName} at {ascomDevice.IpAddress}:{ascomDevice.IpPort}.");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
}
#endregion

#region Simple client creation using a discovered AscomDevice
// This .NET 6 Console project example intentionally avoids using statements to illustrate use of the ASCOM Library namespaces
// It also assumes that the operating system is Windows

// Requires ASCOM Library packages: ASCOM.Alpaca.Components and ASCOM.COM.Components

try
{
    // Create a Discovery component that can be used for one or more discoveries
    using (ASCOM.Alpaca.Discovery.AlpacaDiscovery alpacaDiscovery = new ASCOM.Alpaca.Discovery.AlpacaDiscovery())
    {
        // Initiate a discovery 
        alpacaDiscovery.StartDiscovery(2, 100, 32227, 1.5, false, true, false, ASCOM.Common.Alpaca.ServiceType.Http);

        // Wait for the discovery to complete, testing the completion variable every 50ms.
        do
        {
            Thread.Sleep(50);
        } while (!alpacaDiscovery.DiscoveryComplete);

        // Print the number of ASCOM FilterWheel devices discovered
        List<ASCOM.Alpaca.Discovery.AscomDevice> filterWheelDevices = alpacaDiscovery.GetAscomDevices(ASCOM.Common.DeviceTypes.FilterWheel);
        Console.WriteLine($"Found {filterWheelDevices.Count} FilterWheel devices.");

        // Create an Alpaca client for the first device and use it to display the driver description
        if (filterWheelDevices.Count > 0)
        {
            // Create the filter wheel Alpaca Client
            using (ASCOM.Alpaca.Clients.AlpacaFilterWheel filterWheelClient = ASCOM.Alpaca.Clients.AlpacaClient.GetDevice<ASCOM.Alpaca.Clients.AlpacaFilterWheel>(filterWheelDevices[0]))
            {
                filterWheelClient.Connected = true;
                Console.WriteLine($"Found device: {filterWheelClient.Name} - Driver: {filterWheelClient.DriverInfo}, Version: {filterWheelClient.DriverVersion} containing {filterWheelClient.Names.Count()} filters.");
                filterWheelClient.Connected = false;
            }
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}
#endregion

#region Detailed client creation using a discovered AscomDevice
// This .NET 6 Console project example intentionally avoids using statements to illustrate use of the ASCOM Library namespaces
// It also assumes that the operating system is Windows

// Requires ASCOM Library packages: ASCOM.Alpaca.Components and ASCOM.COM.Components

try
{
    // Create a Discovery component that can be used for one or more discoveries
    using (ASCOM.Alpaca.Discovery.AlpacaDiscovery alpacaDiscovery = new ASCOM.Alpaca.Discovery.AlpacaDiscovery())
    {
        // Initiate a discovery 
        alpacaDiscovery.StartDiscovery(2, 100, 32227, 1.5, false, true, false, ASCOM.Common.Alpaca.ServiceType.Http);

        // Wait for the discovery to complete, testing the completion variable every 50ms.
        do
        {
            Thread.Sleep(50);
        } while (!alpacaDiscovery.DiscoveryComplete);

        // Print the number of ASCOM FilterWheel devices discovered
        List<ASCOM.Alpaca.Discovery.AscomDevice> filterWheelDevices = alpacaDiscovery.GetAscomDevices(ASCOM.Common.DeviceTypes.FilterWheel);
        Console.WriteLine($"Found {filterWheelDevices.Count} FilterWheel devices.");

        // Create an Alpaca client for the first device and use it to display the driver description
        if (filterWheelDevices.Count > 0)
        {
            // Create a trace logger
            using (ASCOM.Tools.TraceLogger logger = new ASCOM.Tools.TraceLogger("FilterWheelTest", true))
            {
                // Create the filter wheel Alpaca Client with:
                //   Connect command timeout = 3 seconds
                //   Standard command timeout = 5 seconds
                //   Long command timeout = 100 seconds
                //   Client ID = 12345
                //   Basic authorisation user name = QuY89
                //   Basic authorisation password = YYu8*9jK
                //   Strict JSON parsing enabled
                //   Use of the logger object to record the client's operational messages
                using (ASCOM.Alpaca.Clients.AlpacaFilterWheel filterWheelClient = ASCOM.Alpaca.Clients.AlpacaClient.GetDevice<ASCOM.Alpaca.Clients.AlpacaFilterWheel>(filterWheelDevices[0], 3, 5, 100, 12345, "QuY89", "YYu8*9jK", true, logger))
                {
                    filterWheelClient.Connected = true;
                    Console.WriteLine($"Found device: {filterWheelClient.Name} - Driver: {filterWheelClient.DriverInfo}, Version: {filterWheelClient.DriverVersion} containing {filterWheelClient.Names.Count()} filters.");
                    logger.LogMessage("FilterWheelTest", $"Found device: {filterWheelClient.Name} - Driver: {filterWheelClient.DriverInfo}, Version: {filterWheelClient.DriverVersion} containing {filterWheelClient.Names.Count()} filters.");
                    filterWheelClient.Connected = false;
                }
            }
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}
#endregion

#region Detailed manual client creation
// This .NET 6 Console project example intentionally avoids using statements to illustrate use of the ASCOM Library namespaces
// It also assumes that the operating system is Windows

// Requires ASCOM Library packages: ASCOM.Alpaca.Components and ASCOM.COM.Components

try
{
    // Create a trace logger
    using (ASCOM.Tools.TraceLogger logger = new ASCOM.Tools.TraceLogger("FilterWheelTest2", true))
    {
        // Create the filter wheel Alpaca Client with:
        //   Service type = HTTP
        //   IP address = 127.0.0.1
        //   IP port = 11111
        //   Alpaca device number = 0
        //   Connect command timeout = 3 seconds
        //   Standard command timeout = 5 seconds
        //   Long command timeout = 100 seconds
        //   Client ID = 12345
        //   Basic authorisation user name = QuY89
        //   Basic authorisation password = YYu8*9jK
        //   Strict JSON parsing enabled
        //   Use of the logger object to record the client's operational messages
        using (ASCOM.Alpaca.Clients.AlpacaFilterWheel filterWheelClient = ASCOM.Alpaca.Clients.AlpacaClient.GetDevice<ASCOM.Alpaca.Clients.AlpacaFilterWheel>(ASCOM.Common.Alpaca.ServiceType.Http, "127.0.0.1", 11111, 0, 3, 5, 100, 12345, "QuY89", "YYu8*9jK", true, logger))
        {
            filterWheelClient.Connected = true;
            Console.WriteLine($"Found device: {filterWheelClient.Name} - Driver: {filterWheelClient.DriverInfo}, Version: {filterWheelClient.DriverVersion} containing {filterWheelClient.Names.Count()} filters.");
            logger.LogMessage("FilterWheelTest", $"Found device: {filterWheelClient.Name} - Driver: {filterWheelClient.DriverInfo}, Version: {filterWheelClient.DriverVersion} containing {filterWheelClient.Names.Count()} filters.");
            filterWheelClient.Connected = false;
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}
#endregion
