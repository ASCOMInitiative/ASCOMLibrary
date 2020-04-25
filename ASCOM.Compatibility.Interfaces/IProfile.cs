using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Compatibility.Interfaces
{
    public interface IProfile
    {
        string DeviceType { get; set; }

        ArrayList RegisteredDeviceTypes { get; }

        ArrayList RegisteredDevices(string DeviceType);

        bool IsRegistered(string DriverID);

        void Register(string DriverID, string DescriptiveName);

        void Unregister(string DriverID);

        string GetValue(string DriverID, string Name, string SubKey, string DefaultValue);

        void WriteValue(string DriverID, string Name, string Value, string SubKey);

        ArrayList Values(string DriverID, string SubKey);

        void DeleteValue(string DriverID, string Name, string SubKey);

        void CreateSubKey(string DriverID, string SubKey);

        ArrayList SubKeys(string DriverID, string SubKey);

        void DeleteSubKey(string DriverID, string SubKey);

        string GetProfileXML(string deviceId);

        void SetProfileXML(string deviceId, string xml);
    }
}
