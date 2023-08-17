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
        /// <param name="progID">ProgID of the driver to be assessed</param>
        /// <param name="requiredBitness">Application bitness for which application compatibility should be tested</param>
        /// <param name="TL">Logging trace logger</param>
        /// <returns>String compatibility message or empty string if driver is fully compatible</returns>
        /// <remarks></remarks>
        internal static string DriverCompatibilityMessage(string progID, Bitness requiredBitness, ILogger TL)
        {
            string driverCompatibilityMessage = default;
            ReadPECharacteristics inProcServer = null;
            bool isRegistered64Bit;
            Bitness inprocServerBitness;
            RegistryKey RK, RKInprocServer32;
            string CLSID, inprocFilePath, codeBase;
            RegistryKey RK32 = null;
            RegistryKey RK64 = null;

#if NETFRAMEWORK
            string assemblyFullName;
            Assembly loadedAssembly;
            PortableExecutableKinds peKind;
            ImageFileMachine machine;
            Module[] modules;
#endif

            using (var profileStore = new RegistryAccess()) // Get access to the profile store
            {
                driverCompatibilityMessage = ""; // Set default return value as OK
                TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     ProgID: " + progID + ", Bitness: " + requiredBitness.ToString());

                // Parse the COM registry section to determine whether this ProgID is an in-process DLL server.
                // If it is then parse the executable to determine whether it is a 32bit only driver and give a suitable message if it is
                // Picks up some COM registration issues as well as a by-product.
                if (requiredBitness == Bitness.Bits64) // We have a 64bit application so check to see whether this is a 32bit only driver
                {
                    RK = Registry.ClassesRoot.OpenSubKey(progID + @"\CLSID", false); // Look in the 64bit section first
                    if (RK is not null) // ProgID is registered and has a CLSID!
                    {
                        CLSID = RK.GetValue("").ToString(); // Get the CLSID for this ProgID
                        RK.Close();

                        // Check the 64bit registry section for this CLSID
                        RK = Registry.ClassesRoot.OpenSubKey(@"CLSID\" + CLSID);
                        if (RK is null) // We don't have an entry in the 64bit CLSID registry section so try the 32bit section
                        {
                            // Check the 32bit registry section
                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     No entry in the 64bit registry, checking the 32bit registry");
                            RK = Registry.ClassesRoot.OpenSubKey(@"Wow6432Node\CLSID\" + CLSID);
                            isRegistered64Bit = false;
                        }
                        else
                        {
                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Found entry in the 64bit registry");
                            isRegistered64Bit = true;
                        }
                        if (RK is not null) // We have a CLSID entry so process it
                        {
                            RKInprocServer32 = RK.OpenSubKey("InprocServer32");
                            RK.Close();

                            if (RKInprocServer32 is not null) // This is an in process server so test for compatibility
                            {
                                inprocFilePath = RKInprocServer32.GetValue("", "").ToString(); // Get the file location from the default position
                                codeBase = RKInprocServer32.GetValue("CodeBase", "").ToString(); // Get the codebase if present to override the default value
                                if (!string.IsNullOrEmpty(codeBase))
                                    inprocFilePath = codeBase;

                                if (inprocFilePath.Trim().ToUpperInvariant() == "MSCOREE.DLL") // We have an assembly, most likely in the GAC so get the actual file location of the assembly
                                {
#if NETFRAMEWORK
                                    // If this assembly is in the GAC, we should have an "Assembly" registry entry with the full assembly name, 
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Found MSCOREE.DLL");

                                    assemblyFullName = RKInprocServer32.GetValue("Assembly", "").ToString(); // Get the full name
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Found full name: " + assemblyFullName);

                                    if (!string.IsNullOrEmpty(assemblyFullName)) // We did get an assembly full name so now try and load it to the reflection only context
                                    {
                                        try
                                        {
                                            loadedAssembly = Assembly.ReflectionOnlyLoad(assemblyFullName);
                                            // OK that went well so we have an MSIL version!
                                            inprocFilePath = loadedAssembly.CodeBase; // Get the codebase for testing below
                                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Found file path: " + inprocFilePath);
                                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Found full name: " + loadedAssembly.FullName + " ");
                                            modules = loadedAssembly.GetLoadedModules();
                                            modules[0].GetPEKind(out peKind, out machine);
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
                                                loadedAssembly = Assembly.ReflectionOnlyLoad(assemblyFullName + ", processorArchitecture=x86");

                                                // OK that went well so we have an x86 only version!

                                                // Get the codebase for testing below
                                                inprocFilePath = loadedAssembly.CodeBase;
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX86", "     Found file path: " + inprocFilePath);
                                                modules = loadedAssembly.GetLoadedModules();
                                                modules[0].GetPEKind(out peKind, out machine);
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
                                                    loadedAssembly = Assembly.ReflectionOnlyLoad(assemblyFullName + ", processorArchitecture=x64");

                                                    // OK that went well so we have an x64 only version!
                                                    inprocFilePath = loadedAssembly.CodeBase; // Get the codebase for testing below
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "     Found file path: " + inprocFilePath);

                                                    modules = loadedAssembly.GetLoadedModules();
                                                    modules[0].GetPEKind(out peKind, out machine);
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

                                        inprocFilePath = ""; // Set to null to bypass tests
                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Set InprocFilePath to null string");
                                    }
#else
                                    // This is .NET Core so we can't load the assembly, we'll just have to take a chance!
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "'Running .NET Core so we can't reflection load the assembly, we'll just have to take a chance!");
                                    inprocFilePath = ""; // Set to null to bypass tests
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Set InprocFilePath to null string");
#endif
                                }

                                if (inprocFilePath.Trim().Right(4).ToUpperInvariant() == ".DLL") // We have a path to the server and it is a dll
                                {
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Found DLL driver");

                                    // We have an assembly or other technology DLL, outside the GAC, in the file system
                                    try
                                    {
                                        inProcServer = new ReadPECharacteristics(inprocFilePath, TL); // Get hold of the executable so we can determine its characteristics
                                        inprocServerBitness = inProcServer.BitNess;
                                        if (inprocServerBitness == Bitness.Bits32) // 32bit driver executable
                                        {
                                            if (isRegistered64Bit) // 32bit driver executable registered in 64bit COM
                                            {
                                                driverCompatibilityMessage = "This 32bit only driver won't work in a 64bit application even though it is registered as a 64bit COM driver." + "\r\n" + GlobalConstants.DRIVER_AUTHOR_MESSAGE_DRIVER;
                                            }
                                            else // 32bit driver executable registered in 32bit COM
                                            {
                                                driverCompatibilityMessage = "This 32bit only driver won't work in a 64bit application even though it is correctly registered as a 32bit COM driver." + "\r\n" + GlobalConstants.DRIVER_AUTHOR_MESSAGE_DRIVER;
                                            }
                                        }
                                        else if (isRegistered64Bit) // 64bit driver
                                        {
                                            // OK - 64bit driver executable registered in 64bit COM section
                                            // This is the only OK combination, no message for this!
                                        }
                                        else // 64bit driver executable registered in 32bit COM
                                        {
                                            driverCompatibilityMessage = "This 64bit capable driver is only registered as a 32bit COM driver." + "\r\n" + GlobalConstants.DRIVER_AUTHOR_MESSAGE_INSTALLER;
                                        }
                                    }
                                    catch (FileNotFoundException) // Cannot open the file
                                    {
                                        driverCompatibilityMessage = "Cannot find the driver executable: " + "\r\n" + "\"" + inprocFilePath + "\"";
                                    }
                                    catch (Exception ex) // Some other exception so log it
                                    {
                                        EventLog.LogEvent("DriverCompatibilityMessage", "Exception parsing " + progID + ", \"" + inprocFilePath + "\"", EventLogEntryType.Error, GlobalConstants.EventLogErrors.DriverCompatibilityException, ex.ToString());
                                        driverCompatibilityMessage = "PEReader Exception, please check ASCOM application Event Log for details";
                                    }

                                    if (inProcServer is not null) // Clean up the PEReader class
                                    {
                                        inProcServer.Dispose();
                                        inProcServer = null;
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
                                // This is not an in-process DLL so no need to test further and no error message to return
                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Driver is not DLL");
                            }
                        }
                        else // Cannot find a CLSID entry
                        {
                            driverCompatibilityMessage = "Unable to find a CLSID entry for this driver, please re-install.";
                        }
                    }
                    else // No COM ProgID registry entry
                    {
                        driverCompatibilityMessage = "This driver is not registered for COM (can't find ProgID), please re-install.";
                    }
                }
                else // We are running a 32bit application test so make sure the executable is not 64bit only
                {
                    RK = Registry.ClassesRoot.OpenSubKey(progID + @"\CLSID", false); // Look in the 32bit registry

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
                                inprocFilePath = RKInprocServer32.GetValue("", "").ToString(); // Get the file location from the default position
                                codeBase = RKInprocServer32.GetValue("CodeBase", "").ToString(); // Get the codebase if present to override the default value
                                if (!string.IsNullOrEmpty(codeBase))
                                    inprocFilePath = codeBase;

                                if (inprocFilePath.Trim().ToUpperInvariant() == "MSCOREE.DLL") // We have an assembly, most likely in the GAC so get the actual file location of the assembly
                                {
#if NETFRAMEWORK
                                    // If this assembly is in the GAC, we should have an "Assembly" registry entry with the full assembly name, 
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Found MSCOREE.DLL");

                                    assemblyFullName = RKInprocServer32.GetValue("Assembly", "").ToString(); // Get the full name
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Found full name: " + assemblyFullName);
                                    if (!string.IsNullOrEmpty(assemblyFullName)) // We did get an assembly full name so now try and load it to the reflection only context
                                    {
                                        try
                                        {
                                            loadedAssembly = Assembly.ReflectionOnlyLoad(assemblyFullName);
                                            // OK that went well so we have an MSIL version!
                                            inprocFilePath = loadedAssembly.CodeBase; // Get the codebase for testing below
                                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Found file path: " + inprocFilePath);
                                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityMSIL", "     Found full name: " + loadedAssembly.FullName + " ");
                                            modules = loadedAssembly.GetLoadedModules();
                                            modules[0].GetPEKind(out peKind, out machine);
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
                                                loadedAssembly = Assembly.ReflectionOnlyLoad(assemblyFullName + ", processorArchitecture=x86");
                                                // OK that went well so we have an x86 only version!
                                                inprocFilePath = loadedAssembly.CodeBase; // Get the codebase for testing below
                                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX86", "     Found file path: " + inprocFilePath);
                                                modules = loadedAssembly.GetLoadedModules();
                                                modules[0].GetPEKind(out peKind, out machine);
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
                                                    loadedAssembly = Assembly.ReflectionOnlyLoad(assemblyFullName + ", processorArchitecture=x64");
                                                    // OK that went well so we have an x64 only version!
                                                    inprocFilePath = loadedAssembly.CodeBase; // Get the codebase for testing below
                                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibilityX64", "     Found file path: " + inprocFilePath);
                                                    modules = loadedAssembly.GetLoadedModules();
                                                    modules[0].GetPEKind(out peKind, out machine);
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
                                        inprocFilePath = ""; // Set to null to bypass tests
                                        TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Set InprocFilePath to null string");
                                    }
#else
                                    // This is .NET Core so we can't load the assembly, we'll just have to take a chance!
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "'Running .NET Core so we can't reflection load the assembly, we'll just have to take a chance!");
                                    inprocFilePath = ""; // Set to null to bypass tests
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Set InprocFilePath to null string");
#endif
                                }

                                if (inprocFilePath.Trim().Right(4).ToUpperInvariant() == ".DLL") // We do have a path to the server and it is a dll
                                {
                                    // We have an assembly or other technology DLL, outside the GAC, in the file system
                                    try
                                    {
                                        inProcServer = new ReadPECharacteristics(inprocFilePath, TL); // Get hold of the executable so we can determine its characteristics
                                        if (inProcServer.BitNess == Bitness.Bits64) // 64bit only driver executable
                                        {
                                            driverCompatibilityMessage = "This is a 64bit only driver and is not compatible with this 32bit application." + "\r\n" + GlobalConstants.DRIVER_AUTHOR_MESSAGE_DRIVER;
                                        }
                                    }
                                    catch (FileNotFoundException) // Cannot open the file
                                    {
                                        driverCompatibilityMessage = "Cannot find the driver executable: " + "\r\n" + "\"" + inprocFilePath + "\"";
                                    }
                                    catch (Exception ex) // Some other exception so log it
                                    {
                                        EventLog.LogEvent("DriverCompatibilityMessage", "Exception parsing " + progID + ", \"" + inprocFilePath + "\"", EventLogEntryType.Error, GlobalConstants.EventLogErrors.DriverCompatibilityException, ex.ToString());
                                        driverCompatibilityMessage = "PEReader Exception, please check ASCOM application Event Log for details";
                                    }

                                    if (inProcServer is not null) // Clean up the PEReader class
                                    {
                                        inProcServer.Dispose();
                                        inProcServer = null;
                                    }
                                }
                                else
                                {
                                    // No codebase or not a DLL so can't test this driver, don't give an error message, just have to take a chance!
                                    TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "No codebase or not a DLL so can't test this driver, don't give an error message, just have to take a chance!");
                                }
                                RKInprocServer32.Close(); // Clean up the InProcServer registry key
                            }
                            else // This is not an in-process DLL so no need to test further and no error message to return
                            {
                                // Please leave this empty clause here so the logic is clear!
                                TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "This is not an in-process DLL so no need to test further and no error message to return");
                            }
                        }
                        else // Cannot find a CLSID entry
                        {
                            driverCompatibilityMessage = "Unable to find a CLSID entry for this driver, please re-install.";
                            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Could not find CLSID entry!");
                        }
                    }
                    else // No COM ProgID registry entry
                    {
                        driverCompatibilityMessage = "This driver is not registered for COM (can't find ProgID), please re-install.";
                    }
                }
            }
            TL?.LogMessage(LogLevel.Debug, "DriverCompatibility", "     Returning: \"" + driverCompatibilityMessage + "\"");
            return driverCompatibilityMessage;
        }
    }
}
