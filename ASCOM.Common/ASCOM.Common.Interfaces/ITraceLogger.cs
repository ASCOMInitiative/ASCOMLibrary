using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Common.Interfaces
{

    /// <summary>
    /// Trace logger interface definition
    /// </summary>
    /// <remarks>Inherits the ILogger interface and exposed the </remarks>
    public interface ITraceLogger : ILogger
    {
        void LogMessage(string method, string message);

        void BlankLine();

        bool Enabled { get; set; }

        int IdentifierWidth { get; set; }

        bool UseUtcTime { get; set; }

        bool RespectCrLf { get; set; }
    }
}
