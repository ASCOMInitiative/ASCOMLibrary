using ASCOM.Common.Interfaces;
using System;
using System.Collections.Generic;

namespace ASCOM.Compatibility.Utilities
{
    public class ProfileCompat : IProfile
    {
        public string DriverID
        {
            get;
            set;
        } = string.Empty;

        readonly string DeviceType = string.Empty;
        public ProfileCompat(string driverID, string deviceType)
        {
            DriverID = driverID;
            DeviceType = deviceType;
        }

        public void DeleteValue(string Name)
        {
            using (ASCOM.Utilities.Profile profile = new ASCOM.Utilities.Profile())
            {
                profile.DeviceType = DeviceType;
                profile.DeleteValue(DriverID, Name);
            }
        }

        public string GetValue(string Name)
        {
            using (ASCOM.Utilities.Profile profile = new ASCOM.Utilities.Profile())
            {
                profile.DeviceType = DeviceType;
                return profile.GetValue(DriverID, Name);
            }
        }

        public string GetValue(string Name, string DefaultValue)
        {
            using (ASCOM.Utilities.Profile profile = new ASCOM.Utilities.Profile())
            {
                profile.DeviceType = DeviceType;
                return profile.GetValue(DriverID, Name, string.Empty, DefaultValue);
            }
        }

        public List<string> Values()
        {
            using (ASCOM.Utilities.Profile profile = new ASCOM.Utilities.Profile())
            {
                profile.DeviceType = DeviceType;

                List<string> retValues = new List<string>();
                var values = profile.Values(DriverID);

                foreach (var value in values)
                {
                    retValues.Add(value.ToString());
                }

                return retValues;
            }
        }

        public void WriteValue(string Name, string Value)
        {
            using (ASCOM.Utilities.Profile profile = new ASCOM.Utilities.Profile())
            {
                profile.DeviceType = DeviceType;
                profile.WriteValue(DriverID, Name, Value);
            }
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            using (ASCOM.Utilities.Profile profile = new ASCOM.Utilities.Profile())
            {
                profile.DeviceType = DeviceType;
                return profile.SubKeys(DriverID).Contains(key);
            }
        }

        public string GetProfile()
        {
            using (ASCOM.Utilities.Profile profile = new ASCOM.Utilities.Profile())
            {
                profile.DeviceType = DeviceType;
                return profile.GetProfileXML(DriverID);
            }
        }

        public void SetProfile(string rawProfile)
        {
            using (ASCOM.Utilities.Profile profile = new ASCOM.Utilities.Profile())
            {
                profile.DeviceType = DeviceType;
                profile.SetProfileXML(DriverID, rawProfile);
            }
        }

        public List<string> Keys()
        {
            using (ASCOM.Utilities.Profile profile = new ASCOM.Utilities.Profile())
            {
                profile.DeviceType = DeviceType;
                List<string> retValues = new List<string>();
                var values = profile.SubKeys(DriverID);

                foreach (var value in values)
                {
                    retValues.Add(value.ToString());

                    var subvalues = profile.SubKeys(DriverID, value.ToString());

                    foreach (var subvalue in subvalues)
                    {
                        retValues.Add(subvalue.ToString());
                    }
                }

                return retValues;
            }
        }
    }
}
