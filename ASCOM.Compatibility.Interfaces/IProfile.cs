using System.Collections.Generic;

namespace ASCOM.Compatibility.Interfaces
{
    public interface IProfile
    {
        void Clear();

        bool Contains(string key);

        string Get(string key);

        string Get(string key, string defaultValue);

        string GetProfile();

        void Remove(string key);

        void SetProfile(string rawProfile);

        List<string> Values();

        void Write(string key, string value);
    }
}