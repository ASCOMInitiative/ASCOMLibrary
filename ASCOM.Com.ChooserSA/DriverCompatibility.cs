using ASCOM.Common;
using ASCOM.Common.Interfaces;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ASCOM.Com
{
    static class DriverCompatibility
    {
        internal enum Bitness
        {
            Bits32,
            Bits64,
            BitsMSIL,
            BitsUnknown
        }

        internal static Bitness ApplicationBits()
        {
            switch (IntPtr.Size)
            {
                case 4:
                    {
                        return Bitness.Bits32;
                    }
                case 8:
                    {
                        return Bitness.Bits64;
                    }

                default:
                    {
                        return Bitness.BitsUnknown;
                    }
            }
        }

        /// <summary>
        /// Return a message when a driver is not compatible with the requested 32/64bit application type. Returns an empty string if the driver is compatible
        /// </summary>
        /// <param name="ProgID">ProgID of the driver to be assessed</param>
        /// <param name="RequiredBitness">Application bitness for which application compatibility should be tested</param>
        /// <param name="TL">Logging trace logger</param>
        /// <returns>String compatibility message or empty string if driver is fully compatible</returns>
        /// <remarks></remarks>
        internal static string DriverCompatibilityMessage(string ProgID, Bitness RequiredBitness, ILogger TL)
        {
            string DriverCompatibilityMessageRet = default;
            ReadPECharacteristics InProcServer = null;
            bool Registered64Bit;
            Bitness InprocServerBitness;
            RegistryKey RK, RKInprocServer32;
            string CLSID, InprocFilePath, CodeBase;
            RegistryKey RK32 = null;
            RegistryKey RK64 = null;
#if NETFRAMEWORK
            string AssemblyFullName;
            Assembly LoadedAssembly;
            PortableExecutableKinds peKind;
            ImageFileMachine machine;
            Module[] Modules;
#endif
            using (var ProfileStore = new RegistryAccess()) // Get access to the profile store
            {
                DriverCompatibilityMessageRet = ""; // Set default return value as OK
                TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     ProgID: " + ProgID + ", Bitness: " + RequiredBitness.ToString());
                // Parse the COM registry section to determine whether this ProgID is an in-process DLL server.
                // If it is then parse the executable to determine whether it is a 32bit only driver and gie a suitable message if it is
                // Picks up some COM registration issues as well as a by-product.
                if (RequiredBitness == Bitness.Bits64) // We have a 64bit application so check to see whether this is a 32bit only driver
                {
                    RK = Registry.ClassesRoot.OpenSubKey(ProgID + @"\CLSID", false); // Look in the 64bit section first
                    if (RK is not null) // ProgID is registered and has a CLSID!
                    {
                        CLSID = RK.GetValue("").ToString(); // Get the CLSID for this ProgID
                        RK.Close();

                        RK = Registry.ClassesRoot.OpenSubKey(@"CLSID\" + CLSID); // Check the 64bit registry section for this CLSID
                        if (RK is null) // We don't have an entry in the 64bit CLSID registry section so try the 32bit section
                        {
                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     No entry in the 64bit registry, checking the 32bit registry");
                            RK = Registry.ClassesRoot.OpenSubKey(@"Wow6432Node\CLSID\" + CLSID); // Check the 32bit registry section
                            Registered64Bit = false;
                        }
                        else
                        {
                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Found entry in the 64bit registry");
                            Registered64Bit = true;
                        }
                        if (RK is not null) // We have a CLSID entry so process it
                        {
                            RKInprocServer32 = RK.OpenSubKey("InprocServer32");
                            RK.Close();
                            if (RKInprocServer32 is not null) // This is an in process server so test for compatibility
                            {
                                InprocFilePath = RKInprocServer32.GetValue("", "").ToString(); // Get the file location from the default position
                                CodeBase = RKInprocServer32.GetValue("CodeBase", "").ToString(); // Get the codebase if present to override the default value
                                if (!string.IsNullOrEmpty(CodeBase))
                                    InprocFilePath = CodeBase;

                                if (InprocFilePath.Trim().ToUpperInvariant() == "MSCOREE.DLL") // We have an assembly, most likely in the GAC so get the actual file location of the assembly
                                {
#if NETFRAMEWORK
                                    // If this assembly is in the GAC, we should have an "Assembly" registry entry with the full assmbly name, 
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Found MSCOREE.DLL");

                                    AssemblyFullName = RKInprocServer32.GetValue("Assembly", "").ToString(); // Get the full name
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Found full name: " + AssemblyFullName);
                                    if (!string.IsNullOrEmpty(AssemblyFullName)) // We did get an assembly full name so now try and load it to the reflection only context
                                    {
                                        try
                                        {
                                            LoadedAssembly = Assembly.ReflectionOnlyLoad(AssemblyFullName);
                                            // OK that wen't well so we have an MSIL version!
                                            InprocFilePath = LoadedAssembly.CodeBase; // Get the codebase for testing below
                                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Found file path: " + InprocFilePath);
                                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Found full name: " + LoadedAssembly.FullName + " ");
                                            Modules = LoadedAssembly.GetLoadedModules();
                                            Modules[0].GetPEKind(out peKind, out machine);
                                            if ((peKind & PortableExecutableKinds.Required32Bit) != 0)
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Kind Required32bit");
                                            if ((peKind & PortableExecutableKinds.PE32Plus) != 0)
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Kind PE32Plus");
                                            if ((peKind & PortableExecutableKinds.ILOnly) != 0)
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Kind ILOnly");
                                            if ((peKind & PortableExecutableKinds.NotAPortableExecutableImage) != 0)
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Kind Not PE Executable");
                                        }

                                        catch (IOException ex)
                                        {
                                            // That failed so try to load an x86 version
                                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "Could not find file, trying x86 version - " + ex.Message);

                                            try
                                            {
                                                LoadedAssembly = Assembly.ReflectionOnlyLoad(AssemblyFullName + ", processorArchitecture=x86");
                                                // OK that wen't well so we have an x86 only version!
                                                InprocFilePath = LoadedAssembly.CodeBase; // Get the codebase for testing below
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX86", "     Found file path: " + InprocFilePath);
                                                Modules = LoadedAssembly.GetLoadedModules();
                                                Modules[0].GetPEKind(out peKind, out machine);
                                                if ((peKind & PortableExecutableKinds.Required32Bit) != 0)
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX86", "     Kind Required32bit");
                                                if ((peKind & PortableExecutableKinds.PE32Plus) != 0)
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX86", "     Kind PE32Plus");
                                                if ((peKind & PortableExecutableKinds.ILOnly) != 0)
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX86", "     Kind ILOnly");
                                                if ((peKind & PortableExecutableKinds.NotAPortableExecutableImage) != 0)
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX86", "     Kind Not PE Executable");
                                            }

                                            catch (IOException ex1)
                                            {
                                                // That failed so try to load an x64 version
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "Could not find file, trying x64 version - " + ex1.Message);

                                                try
                                                {
                                                    LoadedAssembly = Assembly.ReflectionOnlyLoad(AssemblyFullName + ", processorArchitecture=x64");
                                                    // OK that wen't well so we have an x64 only version!
                                                    InprocFilePath = LoadedAssembly.CodeBase; // Get the codebase for testing below
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "     Found file path: " + InprocFilePath);
                                                    Modules = LoadedAssembly.GetLoadedModules();
                                                    Modules[0].GetPEKind(out peKind, out machine);
                                                    if ((peKind & PortableExecutableKinds.Required32Bit) != 0)
                                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "     Kind Required32bit");
                                                    if ((peKind & PortableExecutableKinds.PE32Plus) != 0)
                                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "     Kind PE32Plus");
                                                    if ((peKind & PortableExecutableKinds.ILOnly) != 0)
                                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "     Kind ILOnly");
                                                    if ((peKind & PortableExecutableKinds.NotAPortableExecutableImage) != 0)
                                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "     Kind Not PE Executable");
                                                }

                                                catch (Exception ex2)
                                                {
                                                    // Ignore exceptions here and leave MSCOREE.DLL as the InprocFilePath, this will fail below and generate an "incompatible driver" message
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", ex2.ToString());
                                                }
                                            }

                                            catch (Exception ex1)
                                            {
                                                // Ignore exceptions here and leave MSCOREE.DLL as the InprocFilePath, this will fail below and generate an "incompatible driver" message
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX32", ex1.ToString());
                                            }
                                        }

                                        catch (Exception ex)
                                        {
                                            // Ignore exceptions here and leave MSCOREE.DLL as the InprocFilePath, this will fail below and generate an "incompatible driver" message
                                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", ex.ToString());
                                        }
                                    }
                                    else
                                    {
                                        // No Assembly entry so we can't load the assembly, we'll just have to take a chance!
                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "'AssemblyFullName is null so we can't load the assembly, we'll just have to take a chance!");
                                        InprocFilePath = ""; // Set to null to bypass tests
                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Set InprocFilePath to null string");
                                    }
#else
                                    // This is .NET Core so we can't load the assembly, we'll just have to take a chance!
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "'Running .NET Core so we can't reflection load the assembly, we'll just have to take a chance!");
                                    InprocFilePath = ""; // Set to null to bypass tests
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Set InprocFilePath to null string");
#endif
                                }

                                if (InprocFilePath.Trim().Right(4).ToUpperInvariant() == ".DLL") // We have a path to the server and it is a dll
                                {
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Found DLL driver");

                                    // We have an assembly or other technology DLL, outside the GAC, in the file system
                                    try
                                    {
                                        InProcServer = new ReadPECharacteristics(InprocFilePath, TL); // Get hold of the executable so we can determine its characteristics
                                        InprocServerBitness = InProcServer.BitNess;
                                        if (InprocServerBitness == Bitness.Bits32) // 32bit driver executable
                                        {
                                            if (Registered64Bit) // 32bit driver executable registered in 64bit COM
                                            {
                                                DriverCompatibilityMessageRet = "This 32bit only driver won't work in a 64bit application even though it is registered as a 64bit COM driver." + "\r\n" + GlobalConstants.DRIVER_AUTHOR_MESSAGE_DRIVER;
                                            }
                                            else // 32bit driver executable registered in 32bit COM
                                            {
                                                DriverCompatibilityMessageRet = "This 32bit only driver won't work in a 64bit application even though it is correctly registered as a 32bit COM driver." + "\r\n" + GlobalConstants.DRIVER_AUTHOR_MESSAGE_DRIVER;
                                            }
                                        }
                                        else if (Registered64Bit) // 64bit driver
                                                                  // 64bit driver executable registered in 64bit COM section
                                        {
                                        }
                                        // This is the only OK combination, no message for this!
                                        else // 64bit driver executable registered in 32bit COM
                                        {
                                            DriverCompatibilityMessageRet = "This 64bit capable driver is only registered as a 32bit COM driver." + "\r\n" + GlobalConstants.DRIVER_AUTHOR_MESSAGE_INSTALLER;
                                        }
                                    }
                                    catch (FileNotFoundException) // Cannot open the file
                                    {
                                        DriverCompatibilityMessageRet = "Cannot find the driver executable: " + "\r\n" + "\"" + InprocFilePath + "\"";
                                    }
                                    catch (Exception ex) // Some other exception so log it
                                    {
                                        EventLog.LogEvent("DriverCompatibilityMessage", "Exception parsing " + ProgID + ", \"" + InprocFilePath + "\"", EventLogEntryType.Error, GlobalConstants.EventLogErrors.DriverCompatibilityException, ex.ToString());
                                        DriverCompatibilityMessageRet = "PEReader Exception, please check ASCOM application Event Log for details";
                                    }

                                    if (InProcServer is not null) // Clean up the PEReader class
                                    {
                                        InProcServer.Dispose();
                                        InProcServer = null;
                                    }
                                }
                                else
                                {
                                    // No codebase so can't test this driver, don't give an error message, just have to take a chance!
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "No codebase so can't test this driver, don't give an error message, just have to take a chance!");
                                }
                                RKInprocServer32.Close(); // Clean up the InProcServer registry key
                            }
                            else
                            {
                                // Please leave this empty clause here so the logic is clear!
                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Driver is not DLL");

                            } // This is not an inprocess DLL so no need to test further and no error message to return
                        }
                        else // Cannot find a CLSID entry
                        {
                            DriverCompatibilityMessageRet = "Unable to find a CLSID entry for this driver, please re-install.";
                        }
                    }
                    else // No COM ProgID registry entry
                    {
                        DriverCompatibilityMessageRet = "This driver is not registered for COM (can't find ProgID), please re-install.";
                    }
                }
                else // We are running a 32bit application test so make sure the executable is not 64bit only
                {
                    RK = Registry.ClassesRoot.OpenSubKey(ProgID + @"\CLSID", false); // Look in the 32bit registry

                    if (RK is not null) // ProgID is registered and has a CLSID!
                    {
                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Found 32bit ProgID registration");
                        CLSID = RK.GetValue("").ToString(); // Get the CLSID for this ProgID
                        RK.Close();
                        RK = null;

                        if (Environment.Is64BitOperatingSystem) // We want to test as if we are a 32bit app on a 64bit OS
                        {
                            try
                            {
                                RK32 = RegistryAccess.OpenSubKey3264(RegistryHive.ClassesRoot, @"CLSID\" + CLSID, false, RegistryView.Registry32);
                            }
                            catch (Exception) // Ignore any exceptions, they just mean the operation wasn't successful
                            {
                            }

                            try
                            {
                                RK64 = RegistryAccess.OpenSubKey3264(RegistryHive.ClassesRoot, @"CLSID\" + CLSID, false, RegistryView.Registry64);
                            }
                            catch (Exception) // Ignore any exceptions, they just mean the operation wasn't successful
                            {
                            }
                        }

                        else // We are running on a 32bit OS
                        {
                            RK = Registry.ClassesRoot.OpenSubKey(@"CLSID\" + CLSID); // Check the 32bit registry section for this CLSID
                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Running on a 32bit OS, 32Bit Registered: " + (RK is not null));
                        }

                        if (Environment.Is64BitOperatingSystem)
                        {
                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Running on a 64bit OS, 32bit Registered: " + (RK32 is not null) + ", 64Bit Registered: " + (RK64 is not null));
                            if (RK32 is not null) // We are testing as a 32bit app so if there is a 32bit key return this
                            {
                                RK = RK32;
                            }
                            else // Otherwise return the 64bit key
                            {
                                RK = RK64;
                            }
                        }

                        if (RK is not null) // We have a CLSID entry so process it
                        {
                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Found CLSID entry");
                            RKInprocServer32 = RK.OpenSubKey("InprocServer32");
                            RK.Close();
                            if (RKInprocServer32 is not null) // This is an in process server so test for compatibility
                            {
                                InprocFilePath = RKInprocServer32.GetValue("", "").ToString(); // Get the file location from the default position
                                CodeBase = RKInprocServer32.GetValue("CodeBase", "").ToString(); // Get the codebase if present to override the default value
                                if (!string.IsNullOrEmpty(CodeBase))
                                    InprocFilePath = CodeBase;

                                if (InprocFilePath.Trim().ToUpperInvariant() == "MSCOREE.DLL") // We have an assembly, most likely in the GAC so get the actual file location of the assembly
                                {
#if NETFRAMEWORK
                                    // If this assembly is in the GAC, we should have an "Assembly" registry entry with the full assmbly name, 
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Found MSCOREE.DLL");

                                    AssemblyFullName = RKInprocServer32.GetValue("Assembly", "").ToString(); // Get the full name
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Found full name: " + AssemblyFullName);
                                    if (!string.IsNullOrEmpty(AssemblyFullName)) // We did get an assembly full name so now try and load it to the reflection only context
                                    {
                                        try
                                        {
                                            LoadedAssembly = Assembly.ReflectionOnlyLoad(AssemblyFullName);
                                            // OK that wen't well so we have an MSIL version!
                                            InprocFilePath = LoadedAssembly.CodeBase; // Get the codebase for testing below
                                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Found file path: " + InprocFilePath);
                                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Found full name: " + LoadedAssembly.FullName + " ");
                                            Modules = LoadedAssembly.GetLoadedModules();
                                            Modules[0].GetPEKind(out peKind, out machine);
                                            if ((peKind & PortableExecutableKinds.Required32Bit) != 0)
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Kind Required32bit");
                                            if ((peKind & PortableExecutableKinds.PE32Plus) != 0)
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Kind PE32Plus");
                                            if ((peKind & PortableExecutableKinds.ILOnly) != 0)
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Kind ILOnly");
                                            if ((peKind & PortableExecutableKinds.NotAPortableExecutableImage) != 0)
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Kind Not PE Executable");
                                        }

                                        catch (IOException ex)
                                        {
                                            // That failed so try to load an x86 version
                                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "Could not find file, trying x86 version - " + ex.Message);

                                            try
                                            {
                                                LoadedAssembly = Assembly.ReflectionOnlyLoad(AssemblyFullName + ", processorArchitecture=x86");
                                                // OK that wen't well so we have an x86 only version!
                                                InprocFilePath = LoadedAssembly.CodeBase; // Get the codebase for testing below
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX86", "     Found file path: " + InprocFilePath);
                                                Modules = LoadedAssembly.GetLoadedModules();
                                                Modules[0].GetPEKind(out peKind, out machine);
                                                if ((peKind & PortableExecutableKinds.Required32Bit) != 0)
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX86", "     Kind Required32bit");
                                                if ((peKind & PortableExecutableKinds.PE32Plus) != 0)
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX86", "     Kind PE32Plus");
                                                if ((peKind & PortableExecutableKinds.ILOnly) != 0)
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX86", "     Kind ILOnly");
                                                if ((peKind & PortableExecutableKinds.NotAPortableExecutableImage) != 0)
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX86", "     Kind Not PE Executable");
                                            }

                                            catch (IOException ex1)
                                            {
                                                // That failed so try to load an x64 version
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "Could not find file, trying x64 version - " + ex.Message);

                                                try
                                                {
                                                    LoadedAssembly = Assembly.ReflectionOnlyLoad(AssemblyFullName + ", processorArchitecture=x64");
                                                    // OK that wen't well so we have an x64 only version!
                                                    InprocFilePath = LoadedAssembly.CodeBase; // Get the codebase for testing below
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "     Found file path: " + InprocFilePath);
                                                    Modules = LoadedAssembly.GetLoadedModules();
                                                    Modules[0].GetPEKind(out peKind, out machine);
                                                    if ((peKind & PortableExecutableKinds.Required32Bit) != 0)
                                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "     Kind Required32bit");
                                                    if ((peKind & PortableExecutableKinds.PE32Plus) != 0)
                                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "     Kind PE32Plus");
                                                    if ((peKind & PortableExecutableKinds.ILOnly) != 0)
                                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "     Kind ILOnly");
                                                    if ((peKind & PortableExecutableKinds.NotAPortableExecutableImage) != 0)
                                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "     Kind Not PE Executable");
                                                }

                                                catch (Exception)
                                                {
                                                    // Ignore exceptions here and leave MSCOREE.DLL as the InprocFilePath, this will fail below and generate an "incompatible driver" message
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", ex1.ToString());
                                                }
                                            }

                                            catch (Exception ex1)
                                            {
                                                // Ignore exceptions here and leave MSCOREE.DLL as the InprocFilePath, this will fail below and generate an "incompatible driver" message
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX32", ex1.ToString());
                                            }
                                        }

                                        catch (Exception ex)
                                        {
                                            // Ignore exceptions here and leave MSCOREE.DLL as the InprocFilePath, this will fail below and generate an "incompatible driver" message
                                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", ex.ToString());
                                        }
                                    }
                                    else
                                    {
                                        // No Assembly entry so we can't load the assembly, we'll just have to take a chance!
                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "'AssemblyFullName is null so we can't load the assembly, we'll just have to take a chance!");
                                        InprocFilePath = ""; // Set to null to bypass tests
                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Set InprocFilePath to null string");
                                    }
#else
                                    // This is .NET Core so we can't load the assembly, we'll just have to take a chance!
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "'Running .NET Core so we can't reflection load the assembly, we'll just have to take a chance!");
                                    InprocFilePath = ""; // Set to null to bypass tests
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Set InprocFilePath to null string");
#endif
                                }

                                if (InprocFilePath.Trim().Right(4).ToUpperInvariant() == ".DLL") // We do have a path to the server and it is a dll
                                {
                                    // We have an assembly or other technology DLL, outside the GAC, in the file system
                                    try
                                    {
                                        InProcServer = new ReadPECharacteristics(InprocFilePath, TL); // Get hold of the executable so we can determine its characteristics
                                        if (InProcServer.BitNess == Bitness.Bits64) // 64bit only driver executable
                                        {
                                            DriverCompatibilityMessageRet = "This is a 64bit only driver and is not compatible with this 32bit application." + "\r\n" + GlobalConstants.DRIVER_AUTHOR_MESSAGE_DRIVER;
                                        }
                                    }
                                    catch (FileNotFoundException) // Cannot open the file
                                    {
                                        DriverCompatibilityMessageRet = "Cannot find the driver executable: " + "\r\n" + "\"" + InprocFilePath + "\"";
                                    }
                                    catch (Exception ex) // Some other exception so log it
                                    {
                                        EventLog.LogEvent("DriverCompatibilityMessage", "Exception parsing " + ProgID + ", \"" + InprocFilePath + "\"", EventLogEntryType.Error, GlobalConstants.EventLogErrors.DriverCompatibilityException, ex.ToString());
                                        DriverCompatibilityMessageRet = "PEReader Exception, please check ASCOM application Event Log for details";
                                    }

                                    if (InProcServer is not null) // Clean up the PEReader class
                                    {
                                        InProcServer.Dispose();
                                        InProcServer = null;
                                    }
                                }
                                else
                                {
                                    // No codebase or not a DLL so can't test this driver, don't give an error message, just have to take a chance!
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "No codebase or not a DLL so can't test this driver, don't give an error message, just have to take a chance!");
                                }
                                RKInprocServer32.Close(); // Clean up the InProcServer registry key
                            }
                            else // This is not an inprocess DLL so no need to test further and no error message to return
                            {
                                // Please leave this empty clause here so the logic is clear!
                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "This is not an inprocess DLL so no need to test further and no error message to return");
                            }
                        }
                        else // Cannot find a CLSID entry
                        {
                            DriverCompatibilityMessageRet = "Unable to find a CLSID entry for this driver, please re-install.";
                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Could not find CLSID entry!");
                        }
                    }
                    else // No COM ProgID registry entry
                    {
                        DriverCompatibilityMessageRet = "This driver is not registered for COM (can't find ProgID), please re-install.";
                    }
                }
            }
            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Returning: \"" + DriverCompatibilityMessageRet + "\"");
            return DriverCompatibilityMessageRet;
        }
    }
}
