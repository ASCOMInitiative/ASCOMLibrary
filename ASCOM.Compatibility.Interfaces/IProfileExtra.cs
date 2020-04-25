using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Compatibility.Interfaces
{
    public interface IProfileExtra
    {
        void MigrateProfile(string CurrentPlatformVersion);

        void DeleteValue(string DriverID, string Name);

        string GetValue(string DriverID, string Name);

        string GetValue(string DriverID, string Name, string SubKey);

        ArrayList SubKeys(string DriverID);

        ArrayList Values(string DriverID);

        void WriteValue(string DriverID, string Name, string Value);

        //ASCOMProfile GetProfile(string DriverId);

        //void SetProfile(string DriverId, ASCOMProfile XmlProfileKey);
    }
}
