using System.Collections.Generic;

namespace ASCOM.Standard.Interfaces
{
    public interface IProfile
    {
        void Clear();

        bool ContainsValue(string key);

        string GetValue(string key);

        string GetValue(string key, string defaultValue);

        string GetProfile();

        void DeleteValue(string key);

        void SetProfile(string rawProfile);

        List<string> Values();

        void WriteValue(string key, string value);
    }
}