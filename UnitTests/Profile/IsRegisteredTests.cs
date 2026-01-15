using Microsoft.Win32;
using Xunit;
using ASCOM.Common;
using ASCOM;

namespace Profile
{
#if NET8_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("Windows")]
#endif
    public class ProfileComponentTests
    {
        [Fact]
        public void IsRegisteredFailTests()
        {
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.IsRegistered((DeviceTypes)Test.BAD_DEVICE_TYPE_VALUE, ""));
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.IsRegistered((DeviceTypes)Test.BAD_DEVICE_TYPE_VALUE, "Garbage"));
        }

        [Fact]
        public void IsRegisteredSuccessTests()
        {
            Assert.True(ASCOM.Com.Profile.IsRegistered(DeviceTypes.Telescope, "ASCOM.Simulator.Telescope"));
            Assert.False(ASCOM.Com.Profile.IsRegistered(DeviceTypes.Telescope, "Garbage.Garbage.Garbage.Telescope"));
        }

        [Fact]
        public void RegisterFailTests()
        {
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.Register((DeviceTypes)Test.BAD_DEVICE_TYPE_VALUE, "", ""));
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.Register((DeviceTypes)Test.BAD_DEVICE_TYPE_VALUE, "Garbage", ""));
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.Register((DeviceTypes)Test.BAD_DEVICE_TYPE_VALUE, "Garbage", "Garbage"));
        }

        [Fact]
        public void RegisterSuccessTests()
        {
            string progId = Test.GetProgId(Test.TEST_DEVICE_TYPE);
            Test.ClearDeviceRegistration(progId);
            // Register the test device
            ASCOM.Com.Profile.Register(Test.TEST_DEVICE_TYPE, progId, Test.TEST_DESCRIPTION);

            // Now read the Profile registry to make sure the registration entry exists
            // Open the local machine hive in 32bit mode
            using (RegistryKey localmachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                // Open the ASCOM device type root key
                using (RegistryKey driversKey = localmachine32.OpenSubKey($"SOFTWARE\\ASCOM\\{Test.TEST_DEVICE_TYPE} Drivers\\{progId}", false))
                {
                    Assert.NotNull(driversKey); // Make sure that it is not null, i.e. that the key has been created by the register command
                    string description = driversKey.GetValue(null).ToString();
                    Assert.Equal(Test.TEST_DESCRIPTION, description);
                }
            }

            // Clean up after the test
            Test.ClearDeviceRegistration(progId);
        }

        [Fact]
        public void UnRegisterFailTests()
        {
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.UnRegister((DeviceTypes)Test.BAD_DEVICE_TYPE_VALUE, ""));
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.UnRegister((DeviceTypes)Test.BAD_DEVICE_TYPE_VALUE, "Garbage"));
        }

        [Fact]
        public void UnRegisterSuccessTests()
        {
            string progId = Test.GetProgId(Test.TEST_DEVICE_TYPE);

            Test.ClearDeviceRegistration(progId);

            // Register the test device
            ASCOM.Com.Profile.Register(Test.TEST_DEVICE_TYPE, progId, Test.TEST_DESCRIPTION);

            // Now read the Profile registry to make sure the registration entry exists
            // Open the local machine hive in 32bit mode
            using (RegistryKey localmachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                // Open the ASCOM device type root key
                using (RegistryKey driversKey = localmachine32.OpenSubKey($"SOFTWARE\\ASCOM\\{Test.TEST_DEVICE_TYPE} Drivers\\{progId}", false))
                {
                    Assert.NotNull(driversKey); // Make sure that it is not null, i.e. that the key has been created by the register command
                }
            }

            // Now unregister the device
            ASCOM.Com.Profile.UnRegister(Test.TEST_DEVICE_TYPE, progId);

            // Make sure the entry no longer exists
            // Open the local machine hive in 32bit mode
            using (RegistryKey localmachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                // Open the ASCOM device type root key
                using (RegistryKey driversKey = localmachine32.OpenSubKey($"SOFTWARE\\ASCOM\\{Test.TEST_DEVICE_TYPE} Drivers\\{progId}", false))
                {
                    Assert.Null(driversKey); // Make sure that it is null, i.e. that the key has been removed by the register command
                }
            }

            // Clean up any debris after the test
            Test.ClearDeviceRegistration(progId);
        }

    }
}
