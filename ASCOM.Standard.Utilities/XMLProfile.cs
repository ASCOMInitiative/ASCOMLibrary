using ASCOM.Standard.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;

namespace ASCOM.Standard.Utilities
{
    public class XMLProfile : IProfile
    {
        private const string SettingsVersion = "v1";

        private string FilePath = string.Empty;

        private List<SettingsPair> Settings = new List<SettingsPair>();

        /// <summary>
        /// Creates an XML profile, loading what exists at the path. This is Home or Documents /ASCOM/Alpaca/{driverID}/{deviceType}/v1/Instance-{deviceID}.xml 
        /// It is not recommended to access the same file from two different instances of this Profile at the same time 
        /// </summary>
        /// <param name="driverID">A unique name for your driver. Must be allowed to be in the path.</param>
        /// <param name="deviceType">The ASCOM / Alpaca device type IE focuser, camera, telescope, etc.  Must be allowed to be in the path.</param>
        /// <param name="deviceNumber">The Alpaca device number. Defaults to 0 for drivers with only one device.</param>
        public XMLProfile(string driverID, string deviceType, uint deviceNumber = 0) : this(Path.Combine(AlpacaDataPath, driverID, deviceType, SettingsVersion, string.Format("Instance-{0}.xml", deviceNumber)))
        {
        }

        /// <summary>
        /// Creates an XML profile, loading what exists at the specified path. It will save any changes at the path
        /// It is not recommended to access the same file from two different instances of this Profile at the same time
        /// </summary>
        /// <param name="pathAndFileName">The path and filename to store the profile at</param>
        public XMLProfile(string pathAndFileName)
        {
            //ToDo
            //Save last X versions of file
            //Handle updates
            //Handle multiple corrupt files
            //Create file with correct permissions

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
                    //Todo Log this if appropriate.
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
                        Console.WriteLine(e.Message);
                        throw;
                    }
                }
            }
            else //Settings do not exist, create a new file
            {
                SerializeObject(Settings.ToList(), FilePath);
            }
        }

        public static string AlpacaDataPath
        {
            get
            {
                return Path.Combine(ApplicationDataPath, "ASCOM/Alpaca/");
            }
        }

        public static string ApplicationDataPath
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
                else
                {
                    //Double check that this is the canonical path for Linux config. If not switch to HOME.
                    return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                }
            }
        }

        public void Clear()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
            Settings.Clear();
        }

        public bool ContainsValue(string key)
        {
            return Settings.Any(s => s.Key == key);
        }

        public string GetValue(string key)
        {
            if (ContainsValue(key))
            {
                return Settings.First(s => s.Key == key).Value;
            }
            else
            {
                throw new KeyNotFoundException(string.Format("{0} was not found in the Settings file", key));
            }
        }

        public string GetValue(string key, string defaultValue)
        {
            if (ContainsValue(key))
            {
                return Settings.First(s => s.Key == key).Value;
            }
            else
            {
                return defaultValue;
            }
        }

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

        public void SetProfile(string rawProfile)
        {
            if (string.IsNullOrEmpty(rawProfile))
            {
                throw new ArgumentNullException("The rawProfile must not be null or empty.");
            }

            try
            {
                var settings = new List<SettingsPair>();

                //We are going to deserialize this to make sure that it is valid xml
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

        public List<string> Values()
        {
            var retList = new List<string>();

            foreach (var value in Settings)
            {
                retList.Add(value.Value);
            }
            return retList;
        }

        public List<string> Keys()
        {
            var retList = new List<string>();

            foreach (var value in Settings)
            {
                retList.Add(value.Key);
            }
            return retList;
        }


        public void WriteValue(string key, string value)
        {
            if (ContainsValue(key))
            {
                Settings.RemoveAll(s => s.Key == value);
            }
            Settings.Add(new SettingsPair(key, value));

            Save();
        }

        /// <summary>
        /// Deserializes a file and returns an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static T DeSerializeObjectFromFile<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { return default(T); }

            T objectOut = default(T);

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

        private void Save()
        {
            SerializeObject(Settings, FilePath);
        }

        public class SettingsPair
        {
            public SettingsPair()
            {
            }

            public SettingsPair(string key, string value)
            {
                Key = key;
                Value = value;
            }

            public string Key
            {
                get;
                set;
            }

            public string Value
            {
                get;
                set;
            }
        }
    }
}