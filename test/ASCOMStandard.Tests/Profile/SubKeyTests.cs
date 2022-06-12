using ASCOM.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ASCOM.Alpaca.Tests.Profile
{
    public class SubKeyTests
    {
        [Fact]
        public void CreateSubKeyFailTests()
        {
            string progId = Test.GetProgId(Test.TEST_DEVICE_TYPE);

            Test.ClearDeviceRegistration(progId);

            Assert.Throws<InvalidValueException>(() => Com.Profile.CreateSubKey((DeviceTypes)Test.BAD_DEVICE_TYPE_VALUE, null, null));
            Assert.Throws<InvalidValueException>(() => Com.Profile.CreateSubKey(Test.TEST_DEVICE_TYPE, null, null));
            Assert.Throws<InvalidValueException>(() => Com.Profile.CreateSubKey(Test.TEST_DEVICE_TYPE, progId, null));
            Assert.Throws<InvalidValueException>(() => Com.Profile.CreateSubKey(Test.TEST_DEVICE_TYPE, progId, "  ")); // Fails because sub-key is white space
        }

        [Fact]
        public void CreateSubKeySuccessTests()
        {
            string progId = Test.GetProgId(Test.TEST_DEVICE_TYPE);

            // Clear out any earlier debris
            Test.ClearDeviceRegistration(progId);

            Assert.Null(Test.ReadTestValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1, Test.TEST_VALUE_NAME1));

            // Register the device and show that the test value is not present
            Com.Profile.Register(Test.TEST_DEVICE_TYPE, progId, Test.TEST_DESCRIPTION);
            Assert.DoesNotContain(Test.TEST_SUBKEY1, Com.Profile.GetSubKeys(Test.TEST_DEVICE_TYPE, progId, null));

            // Write the test value and confirm that it exists and is as expected
            Com.Profile.CreateSubKey(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1);
            Assert.Contains(Test.TEST_SUBKEY1, Com.Profile.GetSubKeys(Test.TEST_DEVICE_TYPE, progId, null));

            // Tidy up after test
            Test.ClearDeviceRegistration(progId);
        }
        [Fact]
        public void DeleteSubKeyFailTests()
        {
            string progId = Test.GetProgId(Test.TEST_DEVICE_TYPE);

            Test.ClearDeviceRegistration(progId);

            Assert.Throws<InvalidValueException>(() => Com.Profile.DeleteSubKey((DeviceTypes)Test.BAD_DEVICE_TYPE_VALUE, null, null));
            Assert.Throws<InvalidValueException>(() => Com.Profile.DeleteSubKey(Test.TEST_DEVICE_TYPE, null, null));
            Assert.Throws<InvalidValueException>(() => Com.Profile.DeleteSubKey(Test.TEST_DEVICE_TYPE, progId, null));
            Assert.Throws<InvalidValueException>(() => Com.Profile.DeleteSubKey(Test.TEST_DEVICE_TYPE, progId, "  ")); // Fails because sub-key is white space
        }

        [Fact]
        public void DeleteSubKeySuccessTests()
        {
            string progId = Test.GetProgId(Test.TEST_DEVICE_TYPE);

            // Clear out any earlier debris
            Test.ClearDeviceRegistration(progId);

            Assert.Null(Test.ReadTestValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1, Test.TEST_VALUE_NAME1));

            // Register the device and show that the test value is not present
            Com.Profile.Register(Test.TEST_DEVICE_TYPE, progId, Test.TEST_DESCRIPTION);

            // Write the test value and confirm that it exists and is as expected
            Com.Profile.CreateSubKey(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1);
            Assert.Contains(Test.TEST_SUBKEY1, Com.Profile.GetSubKeys(Test.TEST_DEVICE_TYPE, progId, null));

            // Write the test value and confirm that it exists and is as expected
            Com.Profile.DeleteSubKey(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1);
            Assert.DoesNotContain(Test.TEST_SUBKEY1, Com.Profile.GetSubKeys(Test.TEST_DEVICE_TYPE, progId, null));

            // Tidy up after test
            Test.ClearDeviceRegistration(progId);
        }
    }

}
