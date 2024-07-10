using System;
using System.IO;
using Microsoft.Win32;

namespace ASCOM.Com
{
    static class Configuration
    {
        internal static bool GetBool(string keyName, bool defaultValue)
        {
            var value = default(bool);
            RegistryKey hkcuKey, settingsKey;

            hkcuKey = Registry.CurrentUser;
            hkcuKey.CreateSubKey(GlobalConstants.REGISTRY_UTILITIES_FOLDER);
            settingsKey = hkcuKey.OpenSubKey(GlobalConstants.REGISTRY_UTILITIES_FOLDER, true);

            try
            {
                if (settingsKey.GetValueKind(keyName) == RegistryValueKind.String) // Value does exist
                {
                    value = Convert.ToBoolean(settingsKey.GetValue(keyName));
                }
            }
            catch (IOException) // Value doesn't exist so create it
            {
                try
                {
                    SetName(keyName, defaultValue.ToString());
                    value = defaultValue;
                }
                catch (Exception) // Unable to create value so just return the default value
                {
                    value = defaultValue;
                }
            }
            catch (Exception)
            {
                value = defaultValue;
            }

            // Clean up registry keys
            settingsKey.Flush();
            settingsKey.Close();
            hkcuKey.Flush();
            hkcuKey.Close();

            return value;
        }

        internal static void SetName(string keyName, string value)
        {
            RegistryKey hkcuKey, settingsKey;

            hkcuKey = Registry.CurrentUser;
            hkcuKey.CreateSubKey(GlobalConstants.REGISTRY_UTILITIES_FOLDER);
            settingsKey = hkcuKey.OpenSubKey(GlobalConstants.REGISTRY_UTILITIES_FOLDER, true);

            settingsKey.SetValue(keyName, value.ToString(), RegistryValueKind.String);
            settingsKey.Flush(); // Clean up registry keys
            settingsKey.Close();
            hkcuKey.Flush();
            hkcuKey.Close();
        }
    }
}