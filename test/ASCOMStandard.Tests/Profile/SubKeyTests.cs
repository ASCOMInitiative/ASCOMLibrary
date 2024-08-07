﻿using ASCOM;
using ASCOM.Common;
using Xunit;

namespace Profile
{
    public class SubKeyTests
    {
        [Fact]
        public void CreateSubKeyFailTests()
        {
            string progId = Test.GetProgId(Test.TEST_DEVICE_TYPE);

            Test.ClearDeviceRegistration(progId);

            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.CreateSubKey((DeviceTypes)Test.BAD_DEVICE_TYPE_VALUE, null, null));
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.CreateSubKey(Test.TEST_DEVICE_TYPE, null, null));
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.CreateSubKey(Test.TEST_DEVICE_TYPE, progId, null));
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.CreateSubKey(Test.TEST_DEVICE_TYPE, progId, "  ")); // Fails because sub-key is white space
        }

        [Fact]
        public void CreateSubKeySuccessTests()
        {
            string progId = Test.GetProgId(Test.TEST_DEVICE_TYPE);

            // Clear out any earlier debris
            Test.ClearDeviceRegistration(progId);

            Assert.Null(Test.ReadTestValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1, Test.TEST_VALUE_NAME1));

            // Register the device and show that the test value is not present
            ASCOM.Com.Profile.Register(Test.TEST_DEVICE_TYPE, progId, Test.TEST_DESCRIPTION);
            Assert.DoesNotContain(Test.TEST_SUBKEY1, ASCOM.Com.Profile.GetSubKeys(Test.TEST_DEVICE_TYPE, progId, null));

            // Write the test value and confirm that it exists and is as expected
            ASCOM.Com.Profile.CreateSubKey(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1);
            Assert.Contains(Test.TEST_SUBKEY1, ASCOM.Com.Profile.GetSubKeys(Test.TEST_DEVICE_TYPE, progId, null));

            // Tidy up after test
            Test.ClearDeviceRegistration(progId);
        }
        [Fact]
        public void DeleteSubKeyFailTests()
        {
            string progId = Test.GetProgId(Test.TEST_DEVICE_TYPE);

            Test.ClearDeviceRegistration(progId);

            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.DeleteSubKey((DeviceTypes)Test.BAD_DEVICE_TYPE_VALUE, null, null));
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.DeleteSubKey(Test.TEST_DEVICE_TYPE, null, null));
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.DeleteSubKey(Test.TEST_DEVICE_TYPE, progId, null));
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.DeleteSubKey(Test.TEST_DEVICE_TYPE, progId, "  ")); // Fails because sub-key is white space
        }

        [Fact]
        public void DeleteSubKeySuccessTests()
        {
            string progId = Test.GetProgId(Test.TEST_DEVICE_TYPE);

            // Clear out any earlier debris
            Test.ClearDeviceRegistration(progId);

            Assert.Null(Test.ReadTestValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1, Test.TEST_VALUE_NAME1));

            // Register the device and show that the test value is not present
            ASCOM.Com.Profile.Register(Test.TEST_DEVICE_TYPE, progId, Test.TEST_DESCRIPTION);

            // Write the test value and confirm that it exists and is as expected
            ASCOM.Com.Profile.CreateSubKey(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1);
            Assert.Contains(Test.TEST_SUBKEY1, ASCOM.Com.Profile.GetSubKeys(Test.TEST_DEVICE_TYPE, progId, null));

            // Write the test value and confirm that it exists and is as expected
            ASCOM.Com.Profile.DeleteSubKey(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1);
            Assert.DoesNotContain(Test.TEST_SUBKEY1, ASCOM.Com.Profile.GetSubKeys(Test.TEST_DEVICE_TYPE, progId, null));

            // Tidy up after test
            Test.ClearDeviceRegistration(progId);
        }
    }

}
