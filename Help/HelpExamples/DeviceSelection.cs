using ASCOM.Alpaca.Clients;
using ASCOM.Alpaca.Discovery;
using ASCOM.Common;

namespace HelpExamples
{
    internal class DeviceSelectionClass
    {
        internal static async void DeviceSelection()
        {
            #region AscomDeviceSelectionOption1
            // Get a list of ASCOM devices of a specified type asynchronously
            List<AscomDevice> ascomdevices = await AlpacaDiscovery.GetAscomDevicesAsync(DeviceTypes.Telescope);
            #endregion

            #region AscomDeviceSelectionOption2
            // Get a list of ASCOM devices of all types asynchronously
            List<AscomDevice> allAscomDevices = await AlpacaDiscovery.GetAscomDevicesAsync(null);

            // Use LIONQ to select ASCOM devices of a specific device type
            var ascomDevices = allAscomDevices.Where(device => device.AscomDeviceType == DeviceTypes.Telescope);
            #endregion

            #region AscomDeviceGroupExample
            // Group devices by location using LINQ
            var groupedDevices = ascomDevices.GroupBy(device => device.Location, device => device);
            #endregion

            #region AlpacaDeviceSelection
            // Get a list of Alpaca devices asynchronously
            List<AlpacaDevice> alpacaDevices = await AlpacaDiscovery.GetAlpacaDevicesAsync();
            #endregion

            #region AscomDeviceChooseExample
            // Create a dictionary of Description:AscomDevice key value pairs.
            Dictionary<string, AscomDevice> ascomDevicesDictionary = ascomDevices.ToDictionary(ascomDevice => $"{ascomDevice.Location} ({ascomDevice.IpAddress}:{ascomDevice.IpPort}): {ascomDevice.AscomDeviceName} (Alpaca device number: {ascomDevice.AlpacaDeviceNumber})", ascomDevice => ascomDevice);

            // Ask the user to select an ASCOM device
            AscomDevice? selectedAscomDevice = ChooseAscomDevice(ascomDevicesDictionary);
            #endregion

            #region AlpacaDeviceChooseExample
            // Create a dictionary of Description:AlpacaDevice key value pairs.
            Dictionary<string, AlpacaDevice> alpacaDevicesDictionary = alpacaDevices.ToDictionary(alpacaDevice => $"{alpacaDevice.ServerName} - {alpacaDevice.Manufacturer} ({alpacaDevice.ManufacturerVersion}) at {alpacaDevice.IpAddress}:{alpacaDevice.Port}", alpacaDevice => alpacaDevice);

            // Ask the user to select an ASCOM device from the supplied Alpaca devices
            AscomDevice? chosenbAscomDevice = ChooseFromAlpacaDevice(alpacaDevicesDictionary);
            #endregion

            #region CreateAlpacaDevice
            // Create an Alpaca Client to communicate with the ASCOM device if the user selected once
            if (selectedAscomDevice != null) // The user did select a device
            {
                // Create and use an Alpaca client
                using (AlpacaTelescope alpacaTelescope = AlpacaClient.GetDevice<AlpacaTelescope>(selectedAscomDevice))
                {
                    // Get the device's description
                    alpacaTelescope.Connected = true;
                    string deviceDescription = alpacaTelescope.Description;

                    // Disconnect form the Alpaca device
                    alpacaTelescope.Connected = false;
                }
            }
            #endregion

        }

        #region Support code
        /// <summary>
        /// Present device descriptions to the user and return a selected AscomDevice instance
        /// </summary>
        /// <Returns>An AscomDevice instance or null if no device was selected.</Returns>
        internal static AscomDevice? ChooseAscomDevice(Dictionary<string, AscomDevice> devices)
        {
            // Return null to indicate that the user did not select a device
            return null;
        }

        /// <summary>
        /// Present device descriptions to the user and return a selected AscomDevice instance
        /// </summary>
        /// <Returns>An AscomDevice instance or null if no device was selected.</Returns>
        internal static AscomDevice? ChooseFromAlpacaDevice(Dictionary<string, AlpacaDevice> devices)
        {
            // Return null to indicate that the user did not select a device
            return null;
        }

        #endregion
    }
}
