using ASCOM.Common;
using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ASCOM.Com.DriverAccess
{
    /// <summary>
    /// Base class for COM DriverAccess devices
    /// </summary>
    public abstract class ASCOMDevice : IAscomDeviceV2, IDisposable
    {
        private readonly dynamic device; // COM device

        internal DeviceTypes deviceType; // Device's device type

        internal ILogger TL = null;

        private bool connecting;
        private Exception connectException;

        internal dynamic Device
        {
            get => device;
        }

        private short? interfaceVersion = null;

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="progid">ProgId of the driver</param>
        public ASCOMDevice(string progid)
        {
            device = new DynamicAccess(progid);
        }

        #region IAscomDevice members

        /// <inheritdoc/>
        public bool Connected { get => Device.Connected; set => Device.Connected = value; }

        /// <inheritdoc/>
        public string Description => Device.Description;

        /// <inheritdoc/>
        public string DriverInfo => Device.DriverInfo;

        /// <inheritdoc/>
        public string DriverVersion => Device.DriverVersion;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public string Name => Device.Name;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public string Action(string actionName, string actionParameters)
        {
            return Device.Action(actionName, actionParameters);
        }

        /// <inheritdoc/>
        public void CommandBlind(string command, bool raw = false)
        {
            Device.CommandBlind(command, raw);
        }

        /// <inheritdoc/>
        public bool CommandBool(string command, bool raw = false)
        {
            return Device.CommandBool(command, raw);
        }

        /// <inheritdoc/>
        public string CommandString(string command, bool raw = false)
        {
            return Device.CommandString(command, raw);
        }

        /// <inheritdoc/>
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

        /// <summary>
        /// Show the device set-up dialog
        /// </summary>
        public void SetupDialog()
        {
            Device.SetupDialog();
        }

        #endregion

        #region IAscomDeviceV2 members

        /// <inheritdoc/>
        public void Connect()
        {
            // Check whether this device supports Connect / Disconnect
            if (DeviceCapabilities.HasConnectAndDeviceState(deviceType, InterfaceVersion))
            {
                // Platform 7 or later device so use the device's Connect method
                Device.Connect();
                return;
            }

            // Platform 6 or earlier device so emulate Connect() using the Connected property
            connecting = true;
            connectException = null;

            // Run a task to set the Connected property to True
            Task connectingTask = Task.Factory.StartNew(() =>
            {
                // Ensure that no exceptions can escape
                try
                {
                    // Set Connected True
                    Device.Connected = true;
                }
                catch (Exception ex)
                {
                    // Something went wrong so log the issue and save the exception
                    connectException = ex;
                }
                // Ensure that Connecting is always set False at the end of the task
                finally
                {
                    connecting = false;
                }
            });
        }

        /// <inheritdoc/>
        public void Disconnect()
        {
            // Check whether this device supports Connect / Disconnect
            if (DeviceCapabilities.HasConnectAndDeviceState(deviceType, InterfaceVersion))
            {
                // Platform 7 or later device so use the device's Disconnect method
                Device.Disconnect();
                return;
            }

            // Platform 6 or earlier device so use the Connected property

            // Set Connecting to true and clear any previous exception
            connecting = true;
            connectException = null;

            // Run a task to set the Connected property to False
            Task disConnectingTask = Task.Factory.StartNew(() =>
            {
                // Ensure that no exceptions can escape
                try
                {
                    // Set Connected False
                    Device.Connected = false;
                }
                catch (Exception ex)
                {
                    // Something went wrong so save the exception
                    connectException = ex;
                }
                // Ensure that Connecting is always set False at the end of the task
                finally
                {
                    connecting = false;
                }
            });
        }

        /// <inheritdoc/>
        public bool Connecting
        {
            get
            {
                // Check whether this device supports Connect / Disconnect
                if (DeviceCapabilities.HasConnectAndDeviceState(deviceType, InterfaceVersion))
                {
                    // Platform 7 or later device so return the device's Connecting property
                    return Device.Connecting;
                }

                // If Connected or disconnected threw an exception, throw this to the client
                if (!(connectException is null))
                {
                    throw connectException;
                }

                // Platform 6 or earlier device so always return false.
                return connecting;
            }
        }

        /// <inheritdoc/>
        public List<StateValue> DeviceState
        {
            get
            {
                // Check whether this device supports Connect / Disconnect
                if (DeviceCapabilities.HasConnectAndDeviceState(deviceType, InterfaceVersion))
                {
                    // Platform 7 or later device so return the device's value
                    List<StateValue> deviceState = new List<StateValue>();
                    foreach (dynamic item in Device.DeviceState)
                    {
                        deviceState.Add(new StateValue(item.Name, item.Value));
                    }

                    // Clean the returned values to make sure they are of the correct types
                    List<StateValue> cleaned = OperationalStateProperty.Clean(deviceState, deviceType);
                    return cleaned;
                }

                // Return an empty list for Platform 6 and earlier devices
                return new List<StateValue>();
            }
        }

        #endregion

        #region Support code

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

        #endregion

    }
}
