using ASCOM.Com.Exceptions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace ASCOM.Com
{

    internal class RegistryAccess : IDisposable
    {
        private RegistryKey ProfileRegKey;

        private bool disposedValue = false;        // To detect redundant calls to IDisposable

        /// <summary>
        /// Enum containing all the possible registry access rights values. The built-in RegistryRights enum only has a partial collection
        /// and often returns values such as -1 or large positive and negative integer values when converted to a string
        /// The Flags attribute ensures that the ToString operation returns an aggregate list of discrete values
        /// </summary>
        [Flags()]
        public enum AccessRights
        {
            Query = 1,
            SetKey = 2,
            CreateSubKey = 4,
            EnumSubkey = 8,
            Notify = 0x10,
            CreateLink = 0x20,
            Unknown40 = 0x40,
            Unknown80 = 0x80,

            Wow64_64Key = 0x100,
            Wow64_32Key = 0x200,
            Unknown400 = 0x400,
            Unknown800 = 0x800,
            Unknown1000 = 0x1000,
            Unknown2000 = 0x2000,
            Unknown4000 = 0x4000,
            Unknown8000 = 0x8000,

            StandardDelete = 0x10000,
            StandardReadControl = 0x20000,
            StandardWriteDAC = 0x40000,
            StandardWriteOwner = 0x80000,
            StandardSynchronize = 0x100000,
            Unknown200000 = 0x200000,
            Unknown400000 = 0x400000,
            AuditAccess = 0x800000,

            AccessSystemSecurity = 0x1000000,
            MaximumAllowed = 0x2000000,
            Unknown4000000 = 0x4000000,
            Unknown8000000 = 0x8000000,
            GenericAll = 0x10000000,
            GenericExecute = 0x20000000,
            GenericWrite = 0x40000000,
            GenericRead = int.MinValue + 0x00000000
        }

        #region New and IDisposable Support
        public RegistryAccess() // Create but respect any exceptions thrown
        {
            NewCode(false);
        }

        /// <summary>
        /// Common code for the new method
        /// </summary>
        /// <param name="p_IgnoreChecks">If true, suppresses the exception normally thrown if a valid profile is not present</param>
        /// <remarks></remarks>
        public void NewCode(bool p_IgnoreChecks)
        {
            string PlatformVersion;

            try
            {
                ProfileRegKey = RegistryAccess.OpenSubKey3264(RegistryHive.LocalMachine, GlobalConstants.REGISTRY_ROOT_KEY_NAME, true, RegistryView.Registry32);
                PlatformVersion = GetProfile(@"\", "PlatformVersion");
            }
            // OK, no exception so assume that we are initialised
            catch (System.ComponentModel.Win32Exception ex) // This occurs when the key does not exist and is OK if we are ignoring checks
            {
                if (p_IgnoreChecks)
                {
                    ProfileRegKey = null;
                }
                else
                {
                    throw new ProfilePersistenceException(@"RegistryAccess.New - Profile not found in registry at HKLM\" + GlobalConstants.REGISTRY_ROOT_KEY_NAME, ex);
                }
            }
            catch (Exception ex)
            {
                if (p_IgnoreChecks) // Ignore all checks
                {
                }
                else
                {
                    throw new ProfilePersistenceException("RegistryAccess.New - Unexpected exception", ex);
                }
            }
        }

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                try { ProfileRegKey.Close(); } catch { }
                try { ProfileRegKey.Close(); } catch { }
                try { ProfileRegKey = null; } catch { }

            }
            disposedValue = true;
        }

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put clean-up code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
        }

        #endregion

        #region IAccess Implementation

        internal SortedList<string, string> EnumKeys(string p_SubKeyName)
        {
            // Return a sorted list of sub-keys
            var RetValues = new SortedList<string, string>();
            string[] SubKeys;
            string Value;

            SubKeys = ProfileRegKey.OpenSubKey(RegistryAccess.CleanSubKey(p_SubKeyName)).GetSubKeyNames();

            foreach (string SubKey in SubKeys) // Process each key in turn
            {
                try // If there is an error reading the data don't include in the returned list
                {
                    // Create the new subkey and get a handle to it
                    switch (p_SubKeyName ?? "")
                    {
                        case var @case when @case == "":
                        case @"\":
                            {
                                Value = ProfileRegKey.OpenSubKey(RegistryAccess.CleanSubKey(SubKey)).GetValue("", "").ToString();
                                break;
                            }

                        default:
                            {
                                Value = ProfileRegKey.OpenSubKey(RegistryAccess.CleanSubKey(p_SubKeyName) + @"\" + SubKey).GetValue("", "").ToString();
                                break;
                            }
                    }
                    RetValues.Add(SubKey, Value); // Add the Key name and default value to the hash table
                }
                catch (Exception ex)
                {
                    throw new ProfilePersistenceException("RegistryAccess.EnumKeys exception", ex);
                }
            }

            return RetValues;
        }

        /// <summary>
        /// Read a single value from a key
        /// </summary>
        /// <param name="p_SubKeyName"></param>
        /// <param name="p_ValueName"></param>
        /// <param name="p_DefaultValue"></param>
        /// <returns></returns>
        internal string GetProfile(string p_SubKeyName, string p_ValueName, string p_DefaultValue)
        {
            string RetVal;
            object profileValue;
            RegistryKey registrySubKey;

            RetVal = string.Empty; // Initialise return value to empty string
            try
            {
                // This section re-written to avoid NullReferenceExceptions when the specified subkey does not exist and when the requested value is missing

                // Get a registry handle to the specified subkey. This may be null if the subkey doesn't exist
                registrySubKey = ProfileRegKey.OpenSubKey(RegistryAccess.CleanSubKey(p_SubKeyName));

                // Test whether the registry handle is null, i.e. whether or not the registry subkey exists
                if (!(registrySubKey == null)) // The subkey does exist so retrieve the specified value
                {
                    profileValue = registrySubKey.GetValue(p_ValueName);

                    // Test whether we received something, if not the value is not present
                    if (!(profileValue == null)) // We did receive something so ToString() will work
                    {
                        RetVal = profileValue.ToString();
                    }
                    else if (p_DefaultValue is not null) // We received null so don't try and ToString() this because it will generate a NullReferenceException. Instead return the default value if supplied, otherwise an empty string
                                                         // We have been supplied a default value so set it and then return it
                    {
                        WriteProfile(p_SubKeyName, p_ValueName, p_DefaultValue);
                        RetVal = p_DefaultValue;
                    }
                    else
                    {
                        // Value not yet set and no default value supplied, returning empty string
                    }
                }
                else if (p_DefaultValue is not null) // The subkey doesn't exist so test whether we have been supplied with a default value
                                                     // We have been supplied a default value so set it and then return it
                {
                    WriteProfile(p_SubKeyName, p_ValueName, p_DefaultValue);
                    RetVal = p_DefaultValue;
                }
                else
                {
                    // Value not yet set and no default value supplied, returning empty string
                }
            }
            catch (NullReferenceException)
            {
                if (p_DefaultValue is not null) // We have been supplied a default value so set it and then return it
                {
                    WriteProfile(p_SubKeyName, p_ValueName, p_DefaultValue);
                    RetVal = p_DefaultValue;
                }
                else
                {
                    // Value not yet set and no default value supplied, returning empty string
                }
            }
            catch (Exception ex) // Any other exception
            {
                if (p_DefaultValue is not null) // We have been supplied a default value so set it and then return it
                {
                    WriteProfile(RegistryAccess.CleanSubKey(p_SubKeyName), RegistryAccess.CleanSubKey(p_ValueName), p_DefaultValue);
                    RetVal = p_DefaultValue;
                }
                else
                {
                    throw new ProfilePersistenceException("GetProfile Exception", ex);
                }
            }

            return RetVal;
        }

        internal string GetProfile(string p_SubKeyName, string p_ValueName)
        {
            return GetProfile(p_SubKeyName, p_ValueName, null);
        }

        internal void WriteProfile(string p_SubKeyName, string p_ValueName, string p_ValueData)
        {
            // Write a single value to a key

            try
            {
                if (string.IsNullOrEmpty(p_SubKeyName))
                {
                    ProfileRegKey.SetValue(p_ValueName, p_ValueData, RegistryValueKind.String);
                }
                else
                {
                    ProfileRegKey.CreateSubKey(RegistryAccess.CleanSubKey(p_SubKeyName)).SetValue(p_ValueName, p_ValueData, RegistryValueKind.String);
                }
                ProfileRegKey.Flush();
            }
            catch (Exception ex) // Any other exception
            {
                throw new ProfilePersistenceException("RegistryAccess.WriteProfile exception", ex);
            }
        }

        #endregion

        #region Support Functions

        private static string CleanSubKey(string SubKey)
        {
            // Remove leading "\" if it exists as this is not legal in a subkey name. "\" in the middle of a subkey name is legal however
            if (!string.IsNullOrEmpty(SubKey))
            {
                SubKey = SubKey.Trim();
                if (SubKey.Length > 0)
                {
                    if (SubKey.Substring(0, 1) == @"\")
                        SubKey = SubKey.Substring(1);
                }
            }

            return SubKey;
        }

        internal static RegistryKey OpenSubKey3264(RegistryHive ParentKey, string SubKeyName, bool Writeable, RegistryView Options)
        {
            RegistryKey registryKey;

            registryKey = RegistryKey.OpenBaseKey(ParentKey, Options);
            return registryKey.OpenSubKey(SubKeyName, Writeable);

        }

        #endregion

    }
}
