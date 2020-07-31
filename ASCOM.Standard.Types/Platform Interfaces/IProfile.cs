using System.Collections.Generic;

namespace ASCOM.Standard.Interfaces
{
    /// <summary>
    /// This is the standard interface for a settings profile. The constructor will contain any specific settings needed for the storage provider.
    /// </summary>
    public interface IProfile
    {
        /// <summary>
        /// This will clear all settings. In addition it should remove any artifacts from the computer. For example it should delete any created files, database, or registry keys.
        /// This is useful for a clean uninstall.
        /// </summary>
        void Clear();

        /// <summary>
        /// Check if the profile provider has a given key
        /// </summary>
        /// <param name="key">The key to check for</param>
        /// <returns>true if the profile contains the key, other wise false</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Gets the value associated with a key. Throws an exception if that key does not exist.
        /// </summary>
        /// <param name="key">The key to lookup</param>
        /// <returns>The stored value.</returns>
        string GetValue(string key);

        /// <summary>
        /// Gets the value associated with a key. Returns the defaultValue if that key does not exist. This will not store that key / value pair in the profile.
        /// </summary>
        /// <param name="key">The key to lookup</param>
        /// <param name="defaultValue">The value to return if the key does not exist</param>
        /// <returns>The stored value.</returns>
        string GetValue(string key, string defaultValue);

        /// <summary>
        /// This returns a string version of all of the profile settings. If this is passed into SetProfile it will recreate the profile in its current state. This can be used during upgrades to preserve settings or to back them up.
        /// </summary>
        /// <returns></returns>
        string GetProfile();

        /// <summary>
        /// Deletes a key / value pair from the profile.
        /// </summary>
        /// <param name="key">The key to remove</param>
        void DeleteValue(string key);

        /// <summary>
        /// This sets and saves the profile from the rawProfile string returned from GetProfile. This can be used to restore after an install or from a backup.
        /// </summary>
        /// <param name="rawProfile"></param>
        void SetProfile(string rawProfile);

        /// <summary>
        /// Returns all Values within the profile
        /// </summary>
        /// <returns>A list of all of the current values</returns>
        List<string> Values();

        /// <summary>
        /// Returns all Keys used in the profile
        /// </summary>
        /// <returns></returns>
        List<string> Keys();

        /// <summary>
        /// Write a value to the profile
        /// </summary>
        /// <param name="key">The key of the value</param>
        /// <param name="value">The value to store</param>
        void WriteValue(string key, string value);
    }
}