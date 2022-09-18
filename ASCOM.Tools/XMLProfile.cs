using ASCOM.Common;
using ASCOM.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;

namespace ASCOM.Tools
{
    /// <summary>
    /// Creates and manages an ASCOM Profile as an XML file
    /// </summary>
    public class XMLProfile : IProfile
    {
        private const string SettingsVersion = "v1";

        private readonly string FilePath = string.Empty;

        private List<SettingsPair> Settings = new List<SettingsPair>();

        private ILogger Logger
        {
            get;
            set;
        }

        private static string FileName
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return "Instance";
                }
                else
                {
                    //Lowercase on *nix
                    return "instance";
                }
            }
        }

        /// <summary>
        /// Return the Alpaca folder referenced from the ApplicationDataPath
        /// </summary>
        public static string AlpacaDataPath
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return Path.Combine(ApplicationDataPath, "Alpaca/");
                }
                else
                {
                    //Lower-case on *nix
                    return Path.Combine(ApplicationDataPath, "alpaca/");
                }
            }
        }

        /// <summary>
        /// Return ASCOM's application data folder on this Base machine
        /// </summary>
        public static string ApplicationDataPath
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // Store in USER/.ASCOM
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ASCOM");
                }
                else
                {
                    // Store in $HOME/.config/ascom
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ascom");
                }
            }
        }

        /// <summary>
        /// Creates an XML profile, loading what exists at the path. This is Home or Documents /ASCOM/Alpaca/{driverID}/{deviceType}/v1/Instance-{deviceID}.xml or /ascom/alpaca/{driverID}/{deviceType}/v1/instance-{deviceID}.xml
        /// It is not recommended to access the same file from two different instances of this Profile at the same time 
        /// </summary>
        /// <param name="driverID">A unique name for your driver. Must be allowed to be in the path.</param>
        /// <param name="deviceType">The ASCOM / Alpaca device type IE focuser, camera, telescope, etc.  Must be allowed to be in the path.</param>
        /// <param name="deviceNumber">The Alpaca device number. Defaults to 0 for drivers with only one device.</param>
        /// <param name="logger">The logging device to be used (can be null).</param>
        public XMLProfile(string driverID, string deviceType, uint deviceNumber = 0, ILogger logger = null) : this(Path.Combine(AlpacaDataPath, driverID, deviceType, SettingsVersion, $"{FileName}-{deviceNumber}.xml"), logger)
        {
        }

        /// <summary>
        /// Creates an XML profile, loading what exists at the specified path. It will save any changes at the path
        /// It is not recommended to access the same file from two different instances of this Profile at the same time
        /// </summary>
        /// <param name="pathAndFileName">The path and filename to store the profile at</param>
        /// <param name="logger">The logging device to be used (can be null).</param>
        public XMLProfile(string pathAndFileName, ILogger logger = null)
        {
            //ToDo
            //Save last X versions of file
            //Handle updates
            //Handle multiple corrupt files
            //Create file with correct permissions
            Logger = logger;

            FilePath = pathAndFileName;

            if (File.Exists(pathAndFileName)) //Settings already exist, use them
            {
                try
                {
                    Settings = DeSerializeObjectFromFile<List<SettingsPair>>(FilePath);
                }
                catch (Exception ex)
                {
                    //File exists but is corrupt.
                    Logger?.LogError(ex.Message);
                    try
                    {
                        var newName = FilePath + ".old";
                        if (File.Exists(newName))
                        {
                            File.Delete(newName);
                        }
                        File.Move(FilePath, newName);
                    }
                    catch (Exception e)
                    {
                        Logger?.LogError(e.Message);
                        throw;
                    }
                }
            }
            else //Settings do not exist, create a new file
            {
                SerializeObject(Settings.ToList(), FilePath);
            }
        }

        /// <summary>
        /// Clears all setting and deletes the XML Profile file
        /// </summary>
        public void Clear()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
            Settings.Clear();
        }

        /// <summary>
        /// Determines whether a settings key already exists
        /// </summary>
        /// <param name="key">Name of the key</param>
        /// <returns>True if the settings key already exists, otherwise false</returns>
        public bool ContainsKey(string key)
        {
            return Settings.Any(s => s.Key == key);
        }

        /// <summary>
        /// Gets a key's current value, returning a KeyNotFound exception if the key doesn't exist
        /// </summary>
        /// <param name="key">Key name</param>
        /// <returns>String key value.</returns>
        /// <exception cref="KeyNotFoundException">If the specified key does not exist</exception>
        public string GetValue(string key)
        {
            if (ContainsKey(key))
            {
                return Settings.First(s => s.Key == key).Value;
            }
            else
            {
                throw new KeyNotFoundException(string.Format("{0} was not found in the Settings file", key));
            }
        }

        /// <summary>
        /// Gets a key's current value, setting and returning the supplied default value if the key does not already exist
        /// </summary>
        /// <param name="key">Key name</param>
        /// <param name="defaultValue">Value to be set and returned if the key does not already exist.</param>
        /// <returns>String key value.</returns>
        public string GetValue(string key, string defaultValue)
        {
            if (ContainsKey(key))
            {
                return Settings.First(s => s.Key == key).Value;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Returns the whole Profile as an XML document
        /// </summary>
        /// <returns>string XML document</returns>
        /// <exception cref="NullReferenceException">No settings have been loaded.</exception>
        public string GetProfile()
        {
            if (Settings == null)
            {
                throw new NullReferenceException("The Settings object must not be null.");
            }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(Settings.GetType());

                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, Settings);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    stream.Close();
                    return xmlDocument.OuterXml;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Deletes a key from the Profile
        /// </summary>
        /// <param name="key">Key name to delete</param>
        public void DeleteValue(string key)
        {
            try
            {
                Settings.RemoveAll(r => r.Key == key);
                Save();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Sets a profile from an XML document
        /// </summary>
        /// <param name="rawProfile">A raw XML profile string returned by <see cref="GetProfile"/></param>
        /// <exception cref="ArgumentNullException">If the supplied profile string is null or empty</exception>
        public void SetProfile(string rawProfile)
        {
            if (string.IsNullOrEmpty(rawProfile))
            {
                throw new ArgumentNullException("The rawProfile must not be null or empty.");
            }

            try
            {
                var settings = new List<SettingsPair>();

                //We are going to de-serialize this to make sure that it is valid XML
                using (StringReader read = new StringReader(rawProfile))
                {
                    Type outType = Settings.GetType();

                    XmlSerializer serializer = new XmlSerializer(outType);
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        settings = (List<SettingsPair>)serializer.Deserialize(reader);
                        reader.Close();
                    }

                    read.Close();
                }

                Settings = settings;
                Save();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Returns a list of Profile values
        /// </summary>
        /// <returns>String list of values</returns>
        public List<string> Values()
        {
            var retList = new List<string>();

            foreach (var value in Settings)
            {
                retList.Add(value.Value);
            }
            return retList;
        }

        /// <summary>
        /// Returns a list of Profile keys
        /// </summary>
        /// <returns>String list of keys</returns>
        public List<string> Keys()
        {
            var retList = new List<string>();

            foreach (var value in Settings)
            {
                retList.Add(value.Key);
            }
            return retList;
        }

        /// <summary>
        /// Write a key and value to the XML Profile.
        /// </summary>
        /// <param name="key">Key name.</param>
        /// <param name="value">Key value.</param>
        public void WriteValue(string key, string value)
        {
            if (ContainsKey(key))
            {
                Settings.RemoveAll(s => s.Key == key);
            }
            Settings.Add(new SettingsPair(key, value));

            Save();
        }

        /// <summary>
        /// De-serializes a file and returns an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static T DeSerializeObjectFromFile<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { return default; }

            T objectOut = default;

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(fileName);

                using (StringReader read = new StringReader(xmlDocument.OuterXml))
                {
                    Type outType = typeof(T);

                    XmlSerializer serializer = new XmlSerializer(outType);
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        objectOut = (T)serializer.Deserialize(reader);
                        reader.Close();
                    }

                    read.Close();
                }
            }
            catch
            {
                throw;
            }

            return objectOut;
        }

        /// <summary>
        /// Serializes a file and returns an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializableObject"></param>
        /// <param name="filePathAndName"></param>
        private static void SerializeObject<T>(T serializableObject, string filePathAndName)
        {
            if (serializableObject == null) { return; }

            try
            {
                //ToDo
                //Create file with correct permissions

                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
                System.IO.FileInfo file = new System.IO.FileInfo(filePathAndName);
                file.Directory.Create();
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, serializableObject);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(filePathAndName);
                    stream.Close();
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// persist the current Profile to the backing file store
        /// </summary>
        private void Save()
        {
            SerializeObject(Settings, FilePath);
        }

        /// <summary>
        /// Class representing an ASCOM Profile Key-Value pair.
        /// </summary>
        public class SettingsPair
        {
            /// <summary>
            /// Initialise the SettingsPair as an empty object.
            /// </summary>
            public SettingsPair()
            {
            }

            /// <summary>
            /// Initialise the SettingsPair with the supplied key name and value.
            /// </summary>
            /// <param name="key">The key name for this setting</param>
            /// <param name="value">The value of this setting</param>
            public SettingsPair(string key, string value)
            {
                Key = key;
                Value = value;
            }

            /// <summary>
            /// This setting's Key name.
            /// </summary>
            public string Key
            {
                get;
                set;
            }

            /// <summary>
            /// This setting's value.
            /// </summary>
            public string Value
            {
                get;
                set;
            }
        }
    }
}