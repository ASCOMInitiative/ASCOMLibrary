using ASCOM;
using ASCOM.Common;
using System.Collections.Generic;
using Xunit;

namespace Profile
{
#if NET8_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    public class ProfileReadTests
    {
        [Fact]
        public void ProfileReadFailTests()
        {
            string progId = Test.GetProgId(Test.TEST_DEVICE_TYPE);

            Test.ClearDeviceRegistration(progId);

            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.GetValue((DeviceTypes)Test.BAD_DEVICE_TYPE_VALUE, null, null, null, null));
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.GetValue(Test.TEST_DEVICE_TYPE, null, null, null, null));
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.GetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME1, null, null));
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.GetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME1, null, Test.TEST_SUBKEY1));
            Assert.Throws<InvalidValueException>(() => ASCOM.Com.Profile.GetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME1, Test.TEST_VALUE1, "  ")); // Fails because sub-key is white space
        }

        [Fact]
        public void ProfileReadDefaultValue()
        {
            string progId = Test.GetProgId(Test.TEST_DEVICE_TYPE);

            // Clear out any earlier debris
            Test.ClearDeviceRegistration(progId);

            Assert.Null(Test.ReadTestValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1, Test.TEST_VALUE_NAME1));

            // Register the device and show that the test value is not present
            ASCOM.Com.Profile.Register(Test.TEST_DEVICE_TYPE, progId, Test.TEST_DESCRIPTION);
            Assert.Null(Test.ReadTestValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1, Test.TEST_VALUE_NAME1));

            // Test that default value can be read
            ASCOM.Com.Profile.GetValue(Test.TEST_DEVICE_TYPE, progId, null, null, null);

            // Tidy up after test
            Test.ClearDeviceRegistration(progId);
        }

        [Fact]
        public void ProfileReadValueNotSetNoDefault()
        {

            string progId = Test.GetProgId(Test.TEST_DEVICE_TYPE);

            // Clear out any earlier debris
            Test.ClearDeviceRegistration(progId);

            Assert.Null(Test.ReadTestValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1, Test.TEST_VALUE_NAME1));

            // Register the device and show that the test value is not present
            ASCOM.Com.Profile.Register(Test.TEST_DEVICE_TYPE, progId, Test.TEST_DESCRIPTION);
            Assert.Null(Test.ReadTestValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1, Test.TEST_VALUE_NAME1));

            // Test for exception when value has not yet been set
            Assert.Throws<ValueNotSetException>(() => ASCOM.Com.Profile.GetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME1, null, Test.TEST_SUBKEY1));

            // Tidy up after test
            Test.ClearDeviceRegistration(progId);
        }

        [Fact]
        public void ProfileReadValueNotSetWithDefault()
        {

            string progId = Test.GetProgId(Test.TEST_DEVICE_TYPE);

            // Clear out any earlier debris
            Test.ClearDeviceRegistration(progId);

            Assert.Null(Test.ReadTestValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1, Test.TEST_VALUE_NAME1));

            // Register the device and show that the test value is not present
            ASCOM.Com.Profile.Register(Test.TEST_DEVICE_TYPE, progId, Test.TEST_DESCRIPTION);
            Assert.Null(Test.ReadTestValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1, Test.TEST_VALUE_NAME1));

            // Test whether the value is returned when a default is provided
            string getValue1 = ASCOM.Com.Profile.GetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME1, Test.TEST_VALUE1, Test.TEST_SUBKEY1);
            Assert.Equal(Test.TEST_VALUE1, getValue1);

            // Tidy up after test
            Test.ClearDeviceRegistration(progId);
        }

        [Fact]
        public void ProfileReadExistingValue()
        {

            string progId = Test.GetProgId(Test.TEST_DEVICE_TYPE);

            // Clear out any earlier debris
            Test.ClearDeviceRegistration(progId);

            Assert.Null(Test.ReadTestValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1, Test.TEST_VALUE_NAME1));

            // Register the device and show that the test value is not present
            ASCOM.Com.Profile.Register(Test.TEST_DEVICE_TYPE, progId, Test.TEST_DESCRIPTION);
            Assert.Null(Test.ReadTestValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1, Test.TEST_VALUE_NAME1));

            // Test whether the value is returned when a default is provided
            string getValue1 = ASCOM.Com.Profile.GetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME1, Test.TEST_VALUE1, Test.TEST_SUBKEY1);
            Assert.Equal(Test.TEST_VALUE1, getValue1);

            // Test whether correct value is returned when no default is provided and the value already exists. (This test depends on previous test outcome.)
            string getValue2 = ASCOM.Com.Profile.GetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME1, null, Test.TEST_SUBKEY1);
            Assert.Equal(Test.TEST_VALUE1, getValue2);

            // Tidy up after test
            Test.ClearDeviceRegistration(progId);
        }

        [Fact]
        public void ProfileReadValuesNoSubKey()
        {
            string progId = Test.GetProgId(Test.TEST_DEVICE_TYPE);

            // Clear out any earlier debris
            Test.ClearDeviceRegistration(progId);

            Assert.Null(Test.ReadTestValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1, Test.TEST_VALUE_NAME1));

            // Register the device and initialise values
            ASCOM.Com.Profile.Register(Test.TEST_DEVICE_TYPE, progId, Test.TEST_DESCRIPTION);
            ASCOM.Com.Profile.SetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME1, Test.TEST_VALUE1, null);
            ASCOM.Com.Profile.SetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME2, Test.TEST_VALUE2, null);
            ASCOM.Com.Profile.SetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME3, Test.TEST_VALUE3, null);
            ASCOM.Com.Profile.SetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME4, Test.TEST_VALUE4, null);
                
            Dictionary<string, string> values = ASCOM.Com.Profile.GetValues(Test.TEST_DEVICE_TYPE, progId);

            Assert.True(values.ContainsKey(""));
            Assert.True(values[""] == Test.TEST_DESCRIPTION);

            Assert.True(values.ContainsKey(Test.TEST_VALUE_NAME1));
            Assert.True(values[Test.TEST_VALUE_NAME1] == Test.TEST_VALUE1);

            Assert.True(values.ContainsKey(Test.TEST_VALUE_NAME2));
            Assert.True(values[Test.TEST_VALUE_NAME2] == Test.TEST_VALUE2);

            Assert.True(values.ContainsKey(Test.TEST_VALUE_NAME3));
            Assert.True(values[Test.TEST_VALUE_NAME3] == Test.TEST_VALUE3);

            Assert.True(values.ContainsKey(Test.TEST_VALUE_NAME4));
            Assert.True(values[Test.TEST_VALUE_NAME4] == Test.TEST_VALUE4);

            Dictionary<string, string> values1 = ASCOM.Com.Profile.GetValues(Test.TEST_DEVICE_TYPE, progId, null);

            Assert.True(values1.ContainsKey(""));
            Assert.True(values1[""] == Test.TEST_DESCRIPTION);

            Assert.True(values1.ContainsKey(Test.TEST_VALUE_NAME1));
            Assert.True(values1[Test.TEST_VALUE_NAME1] == Test.TEST_VALUE1);

            Assert.True(values1.ContainsKey(Test.TEST_VALUE_NAME2));
            Assert.True(values1[Test.TEST_VALUE_NAME2] == Test.TEST_VALUE2);

            Assert.True(values1.ContainsKey(Test.TEST_VALUE_NAME3));
            Assert.True(values1[Test.TEST_VALUE_NAME3] == Test.TEST_VALUE3);

            Assert.True(values1.ContainsKey(Test.TEST_VALUE_NAME4));
            Assert.True(values1[Test.TEST_VALUE_NAME4] == Test.TEST_VALUE4);

            // Tidy up after test
            Test.ClearDeviceRegistration(progId);
        }
        [Fact]
        public void ProfileReadValuesWithSubKey()
        {
            string progId = Test.GetProgId(Test.TEST_DEVICE_TYPE);

            // Clear out any earlier debris
            Test.ClearDeviceRegistration(progId);

            Assert.Null(Test.ReadTestValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1, Test.TEST_VALUE_NAME1));

            // Register the device and initialise values
            ASCOM.Com.Profile.Register(Test.TEST_DEVICE_TYPE, progId, Test.TEST_DESCRIPTION);
            ASCOM.Com.Profile.SetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME1, Test.TEST_VALUE1, Test.TEST_SUBKEY1);
            ASCOM.Com.Profile.SetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME2, Test.TEST_VALUE2, Test.TEST_SUBKEY1);
            ASCOM.Com.Profile.SetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME3, Test.TEST_VALUE3, Test.TEST_SUBKEY1);
            ASCOM.Com.Profile.SetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME4, Test.TEST_VALUE4, Test.TEST_SUBKEY1);

            Dictionary<string, string> values = ASCOM.Com.Profile.GetValues(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1);

            Assert.True(values.ContainsKey(Test.TEST_VALUE_NAME1));
            Assert.True(values[Test.TEST_VALUE_NAME1] == Test.TEST_VALUE1);

            Assert.True(values.ContainsKey(Test.TEST_VALUE_NAME2));
            Assert.True(values[Test.TEST_VALUE_NAME2] == Test.TEST_VALUE2);

            Assert.True(values.ContainsKey(Test.TEST_VALUE_NAME3));
            Assert.True(values[Test.TEST_VALUE_NAME3] == Test.TEST_VALUE3);

            Assert.True(values.ContainsKey(Test.TEST_VALUE_NAME4));
            Assert.True(values[Test.TEST_VALUE_NAME4] == Test.TEST_VALUE4);

            // Tidy up after test
            Test.ClearDeviceRegistration(progId);
        }

        [Fact]
        public void ProfileReadSubKeysNoSubKey()
        {
            string progId = Test.GetProgId(Test.TEST_DEVICE_TYPE);

            // Clear out any earlier debris
            Test.ClearDeviceRegistration(progId);

            Assert.Null(Test.ReadTestValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1, Test.TEST_VALUE_NAME1));

            // Register the device and initialise values
            ASCOM.Com.Profile.Register(Test.TEST_DEVICE_TYPE, progId, Test.TEST_DESCRIPTION);
            ASCOM.Com.Profile.SetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME1, Test.TEST_VALUE1, Test.TEST_SUBKEY1);
            ASCOM.Com.Profile.SetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME2, Test.TEST_VALUE2, Test.TEST_SUBKEY2);
            ASCOM.Com.Profile.SetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME3, Test.TEST_VALUE3, Test.TEST_SUBKEY3);
            ASCOM.Com.Profile.SetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME4, Test.TEST_VALUE4, Test.TEST_SUBKEY4);

            List<string> values = ASCOM.Com.Profile.GetSubKeys(Test.TEST_DEVICE_TYPE, progId);

            Assert.Contains(Test.TEST_SUBKEY1, values);
            Assert.Contains(Test.TEST_SUBKEY2, values);
            Assert.Contains(Test.TEST_SUBKEY3, values);
            Assert.Contains(Test.TEST_SUBKEY4, values);

            // Tidy up after test
            Test.ClearDeviceRegistration(progId);
        }
        [Fact]
        public void ProfileReadSubKeysWithSubKey()
        {
            string progId = Test.GetProgId(Test.TEST_DEVICE_TYPE);

            // Clear out any earlier debris
            Test.ClearDeviceRegistration(progId);

            Assert.Null(Test.ReadTestValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1, Test.TEST_VALUE_NAME1));

            // Register the device and initialise values
            ASCOM.Com.Profile.Register(Test.TEST_DEVICE_TYPE, progId, Test.TEST_DESCRIPTION);
            ASCOM.Com.Profile.SetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME1, Test.TEST_VALUE1, $"{Test.TEST_SUBKEY1}\\{Test.TEST_SUBKEY1}");
            ASCOM.Com.Profile.SetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME2, Test.TEST_VALUE2, $"{Test.TEST_SUBKEY1}\\{Test.TEST_SUBKEY2}");
            ASCOM.Com.Profile.SetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME3, Test.TEST_VALUE3, $"{Test.TEST_SUBKEY1}\\{Test.TEST_SUBKEY3}");
            ASCOM.Com.Profile.SetValue(Test.TEST_DEVICE_TYPE, progId, Test.TEST_VALUE_NAME4, Test.TEST_VALUE4, $"{Test.TEST_SUBKEY1}\\{Test.TEST_SUBKEY4}");

            List<string> values = ASCOM.Com.Profile.GetSubKeys(Test.TEST_DEVICE_TYPE, progId, Test.TEST_SUBKEY1);

            Assert.Contains(Test.TEST_SUBKEY1, values);
            Assert.Contains(Test.TEST_SUBKEY2, values);
            Assert.Contains(Test.TEST_SUBKEY3, values);
            Assert.Contains(Test.TEST_SUBKEY4, values);

            // Tidy up after test
            Test.ClearDeviceRegistration(progId);
        }
    }
}