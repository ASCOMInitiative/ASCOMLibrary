using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Compatibility.Interfaces
{
    public interface IProfile
    {
        void WriteValue(string Name, string Value);

        string GetValue(string Name);

        string GetValue(string Name, string DefaultValue);

        void DeleteValue(string Name);

        List<string> Values();
    }
}
