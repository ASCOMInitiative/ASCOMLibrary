using System;
using System.Collections.Generic;
using System.Text;
    using System.Runtime.InteropServices;

namespace ASCOM.Common
{
    /// <summary>
    /// Provides helper methods for determining the operating system.
    /// </summary>
    public static class OsHelper
    {
        /// <summary>
        /// Gets the generic name of the operating system.
        /// </summary>
        /// <returns>A string representing the generic name of the operating system.</returns>
        public static string GetGenericOsName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "Windows";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "Linux";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "MacOS";

            return "Unknown";
        }
    }
}
