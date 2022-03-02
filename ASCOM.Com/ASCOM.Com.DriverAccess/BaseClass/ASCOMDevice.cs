using ASCOM.Common.DeviceInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
namespace ASCOM.Com.DriverAccess
{
    public abstract class ASCOMDevice : IAscomDevice, IDisposable
    {
        private readonly dynamic device;
        internal dynamic Device
        {
            get => device;
        }

        private short? interfaceVersion = null;

        public ASCOMDevice(string progid)
        {
            device = new DynamicAccess(progid);
        }

        public bool Connected { get => Device.Connected; set => Device.Connected = value; }

        public string Description => Device.Description;

        public string DriverInfo => Device.DriverInfo;

        public string DriverVersion => Device.DriverVersion;

        public short InterfaceVersion
        {
            get
            {
                // Test whether the interface version has already been retrieved
                if (!interfaceVersion.HasValue) // This is the first time the method has been called so get the interface version number from the driver and cache it
                {
                    try { interfaceVersion = Device.InterfaceVersion; } // Get the interface version
                    catch { interfaceVersion = 1; } // The method failed so assume that the driver has a version 1 interface where the InterfaceVersion method is not implemented
                }

                return interfaceVersion.Value; // Return the newly retrieved or already cached value
            }
        }

        public string Name => Device.Name;

        public IList<string> SupportedActions
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return new List<string>();
                }
                return (Device.SupportedActions as IEnumerable).Cast<string>().ToList();
            }
        }

        public string Action(string ActionName, string ActionParameters)
        {
            return Device.Action(ActionName, ActionParameters);
        }

        public void CommandBlind(string Command, bool Raw = false)
        {
            Device.CommandBlind(Command, Raw);
        }

        public bool CommandBool(string Command, bool Raw = false)
        {
            return Device.CommandBool(Command, Raw);
        }

        public string CommandString(string Command, bool Raw = false)
        {
            return Device.CommandString(Command, Raw);
        }

        public void Dispose()
        {
            if (Device.Device != null)
            {
                try
                {
                    if (Device.IsComObject)
                    {
                        //run the COM object method
                        try
                        {
                            Device.Dispose();
                        }
                        catch (COMException ex)
                        {
                            if (ex.ErrorCode == int.Parse("80020006", NumberStyles.HexNumber, CultureInfo.InvariantCulture))
                            {
                                // "Driver does not have a Dispose method"
                            }
                        }
                        catch
                        {
                        }

                        //"This is a COM object so attempting to release it"
                        var releaseComObject = Marshal.ReleaseComObject(Device.Device);
                        if (releaseComObject == 0) Device.Device = null;
                    }
                    else // Should be a .NET object so lets dispose of it
                    {
                        Device.Dispose();
                    }
                }
                catch
                {
                    // Ignore any errors here as we are disposing
                }
            }
        }

        public void SetupDialog()
        {
            Device.SetupDialog();
        }

        internal void AssertMethodImplemented(int MinimumVersion, string Message)
        {
            if (this.InterfaceVersion < MinimumVersion)
            {
                throw new ASCOM.NotImplementedException(Message);
            }
        }

        internal void AssertPropertyImplemented(int MinimumVersion, string Message)
        {
            if (this.InterfaceVersion < MinimumVersion)
            {
                throw new ASCOM.NotImplementedException(Message);
            }
        }
    }
}
