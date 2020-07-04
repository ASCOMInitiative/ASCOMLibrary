using ASCOM.Compatibility.Interfaces;
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
        public class SettingsPair
        {
            public SettingsPair()
            {

            }

            public SettingsPair(string key, string data)
            {
                Key = key;
                Value = data;
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

        public static string ApplicationDataPath
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                }
                else
                {
                    return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                }
            }
        }

        public static string AlpacaDataPath
        {
            get
            {
                return Path.Combine(ApplicationDataPath, "ASCOM/Alpaca/");
            }
        }

        private string FilePath = string.Empty;

        private const string SettingsVersion = "v1";

        private List<SettingsPair> Settings = new List<SettingsPair>();

        public XMLProfile(string driverID, string deviceType, int deviceID = 0) : this(Path.Combine(AlpacaDataPath, driverID, deviceType, SettingsVersion, string.Format("Instance-{0}.xml", deviceID)))
        {
        }

        public XMLProfile(string pathAndFileName)
        {
            //ToDo
            //Save last X versions of file
            //Handle updates
            //Handle multiple corrupt files

            FilePath = pathAndFileName;

            if (File.Exists(pathAndFileName)) //Settings already exist, use them
            {
                try
                {
                    Settings = DeSerializeObjectFromFile<List<SettingsPair>>(FilePath);
                }
                catch(Exception ex)
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
                    catch(Exception e)
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

        public void WriteValue(string Name, string Value)
        {
            if (Settings.Any(s=>s.Key == Name))
            {
                Settings.RemoveAll(s=>s.Key == Value);
            }
                Settings.Add(new SettingsPair(Name, Value));
            
            Save();
        }

        public string GetValue(string Name)
        {
            if (Settings.Any(s=>s.Key == Name))
            {
                return Settings.First(s=>s.Key == Name).Value;
            }
            else
            {
                throw new KeyNotFoundException(string.Format("{0} was not found in the Settings file", Name));
            }
        }

        public string GetValue(string Name, string DefaultValue)
        {
            if (Settings.Any(s => s.Key == Name))
            {
                return Settings.First(s => s.Key == Name).Value;
            }
            else
            {
                return DefaultValue;
            }
        }

        public void DeleteValue(string Name)
        {
            try
            {
                Settings.RemoveAll(r=>r.Key == Name);
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

            foreach(var value in Settings)
            {
                retList.Add(value.Value);
            }
            return retList;
        }

        private void Save()
        {
            SerializeObject(Settings.ToList(), FilePath);
        }

        private static void SerializeObject<T>(T serializableObject, string filePathAndName)
        {
            if (serializableObject == null) { return; }

            try
            {
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
                string xmlString = xmlDocument.OuterXml;

                using (StringReader read = new StringReader(xmlString))
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
    }
}