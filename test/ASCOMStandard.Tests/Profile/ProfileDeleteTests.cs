using ASCOM.Common;
using Xunit;

namespace ASCOM.Alpaca.Tests.Profile
{
    public class ProfileDeleteTests
    {
        [Fact]
        public void ProfileDeleteFailTests()
        {
            string progId = Test.GetProgId(Test.TEST_DEVICE_TYPE);

            Test.ClearDeviceRegistration(progId);

            Assert.Throws<InvalidValueException>(() => Com.Profile.DeleteValue((DeviceTypes)Test.BAD_DEVICE_TYPE_VALUE, null, null));
            Assert.Throws<InvalidValueException>(() => Com.Profile.DeleteValue(Test.TEST_DEVICE_TYPE, null, null));
            Assert.Throws<InvalidValueException>(() => Com.Profile.DeleteValue(Test.TEST_DEVICE_TYPE, progId, null));
            Assert.Throws<InvalidValueException>(() => Com.Profile.DeleteValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME1));
            Assert.Throws<InvalidValueException>(() => Com.Profile.DeleteValue(Test.TEST_DEVICE_TYPE, progId, "  ")); // Fails because value name is white space
        }

        [Fact]
        public void ProfileDeleteSuccessTests()
        {
            string progId = Test.GetProgId(Test.TEST_DEVICE_TYPE);

            // Clear out any earlier debris
            Test.ClearDeviceRegistration(progId);

            Assert.Null(Test.ReadTestValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1, Test.TEST_VALUE_NAME1));

            // Register the device and show that the test value is not present
            Com.Profile.Register(Test.TEST_DEVICE_TYPE, progId, Test.TEST_DESCRIPTION);
            Assert.Null(Test.ReadTestValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1, Test.TEST_VALUE_NAME1));

            // Write the test value and confirm that it exists and is as expected
            Com.Profile.SetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME1, Test.TEST_VALUE1, Test.TEST_SUBKEY1);
            string readValue = Test.ReadTestValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1, Test.TEST_VALUE_NAME1);
            Assert.Equal(Test.TEST_VALUE1, readValue);

            Com.Profile.DeleteValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME1, Test.TEST_SUBKEY1);
            string readValue1 = Test.ReadTestValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1, Test.TEST_VALUE_NAME1);
            Assert.Null(readValue1);

            // Tidy up after test
            Test.ClearDeviceRegistration(progId);
        }

    }
}
