﻿using ASCOM;
using ASCOM.Common;
using Xunit;

namespace Profile
{
    public class ProfileWriteTests
    {
        [Fact]
        public void ProfileWriteFailTests()
        {
            string progId = Test.GetProgId(Test.TEST_DEVICE_TYPE);

            Test.ClearDeviceRegistration(progId);

            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.SetValue((DeviceTypes)Test.BAD_DEVICE_TYPE_VALUE, null, null, null, null));
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.SetValue(Test.TEST_DEVICE_TYPE, null, null, null, null));
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.SetValue(Test.TEST_DEVICE_TYPE, progId, null, null, null));
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.SetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME1, null, null));
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.SetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME1, null, Test.TEST_SUBKEY1));
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.SetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME1, Test.TEST_VALUE1, "  ")); // Fails because sub-key is white space
        }

        [Fact]
        public void ProfileWriteSuccessTests()
        {
            string progId = Test.GetProgId(Test.TEST_DEVICE_TYPE);

            // Clear out any earlier debris
            Test.ClearDeviceRegistration(progId);

            Assert.Null(Test.ReadTestValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1, Test.TEST_VALUE_NAME1));

            // Register the device and show that the test value is not present
            ASCOM.Com.Profile.Register(Test.TEST_DEVICE_TYPE, progId, Test.TEST_DESCRIPTION);
            Assert.Null(Test.ReadTestValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1, Test.TEST_VALUE_NAME1));

            // Write the test value and confirm that it exists and is as expected
            ASCOM.Com.Profile.SetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME1, Test.TEST_VALUE1, Test.TEST_SUBKEY1);
            string readValue = Test.ReadTestValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1, Test.TEST_VALUE_NAME1);
            Assert.Equal(Test.TEST_VALUE1, readValue);

            // Tidy up after test
            Test.ClearDeviceRegistration(progId);
        }
    }
}
