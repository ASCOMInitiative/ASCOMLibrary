#region How to Create a Chooser
using ASCOM.Com;
using ASCOM.Common;

namespace HelpExamples
{
    internal class HowtToCreateAChooserClass
    {
        internal void HowToCreateAChooser()
        {
            const string DEVICE_TYPE_NAME = "ObservingConditions";

            // Create a stand-alone Chooser instance to select a telescope driver
            ChooserSA chooserSA = new()
            {
                // Set the device type to Telescope
                DeviceType = DeviceTypes.Telescope
            };

            // Retrieve the selected telescope driver ProgID
            string selectedProgId = chooserSA.Choose("ASCOM.Simulator.Telescope");

            // Display the returned ProgID
            Console.WriteLine($"Selected Telescope ProgID: {selectedProgId}");

            // When implementing ChooserSA you may find the following .ToDeviceType() and .ToDeviceString() Library extension methods useful
            // to convert between string device type names and DeviceTypes enum values.

            // Illustrate the extension method for converting a string device type name to a DeviceTypes enum value.
            DeviceTypes deviceType = DEVICE_TYPE_NAME.ToDeviceType();
            Console.WriteLine($"The DeviceTypes enum value of the string device type name: {DEVICE_TYPE_NAME} is: {deviceType}");

            // Illustrate the extension method for converting a DeviceTypes enum value to a string device type name.
            string deviceTypeString = deviceType.ToDeviceString();
            Console.WriteLine($"The DeviceTypes enum value of the string device type name: {deviceType} is: {deviceTypeString}");

            // Wait for a key press before closing the program
            Console.ReadKey();
        }
    }
}
#endregion