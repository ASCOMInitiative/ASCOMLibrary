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

        /// <summary>
        /// Set True to enable the link. Set False to disable the link.
        /// You can also read the property to check whether it is connected.
        /// </summary>
        /// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public bool Connected { get => Device.Connected; set => Device.Connected = value; }

        /// <summary>
        /// Returns a description of the driver, such as manufacturer and model number.
        /// </summary>
        /// <value>The description.</value>
        /// <exception cref="NotConnectedException">When <see cref="Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Any ASCII characters may be used. The string must not exceed 68 characters (for compatibility with FITS headers).
        /// </remarks>
        public string Description => Device.Description;

        /// <summary>
        /// Descriptive and version information about this ASCOM driver.
        /// </summary>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// This string may contain line endings and may be hundreds to thousands of characters long.
        /// It is intended to display detailed information on the ASCOM driver, including version and copyright data.
        /// See the Description property for descriptive info on the telescope itself.
        /// To get the driver version in a parseable string, use the DriverVersion property.
        /// </remarks>
        public string DriverInfo => Device.DriverInfo;

        /// <summary>
        /// A string in the form "n.n" containing only the major and minor version of the driver.
        /// </summary>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// Not to be confused with the InterfaceVersion property, which is the version of this specification supported by the driver (currently 2). 
        /// </remarks>
        public string DriverVersion => Device.DriverVersion;

        /// <summary>
        /// The interface version number that this device implements.
        /// </summary>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <para>E.g. a device that implements IDeviceV3 should return 3.</para>
        /// <para>
        /// Clients can detect legacy V1 drivers by trying to read this property.
        /// If the driver raises an error, it is a V1 driver. V1 did not specify this property. A driver may also return a value of 1. 
        /// In other words, a raised error or a return value of 1 indicates that the driver is a V1 driver. 
        /// </para>
        /// </remarks>
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

        /// <summary>
        /// The short name of the driver, for display purposes
        /// </summary>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public string Name => Device.Name;

        /// <summary>
        /// Returns the list of action names supported by this driver.
        /// </summary>
        /// <value>An ArrayList of strings (SafeArray collection) containing the names of supported actions.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Must be implemented</b></p> This method must return an empty <see cref="IList{String}"/> object if no actions are supported. Please do not throw a <see cref="NotImplementedException" />.
        /// <para>SupportedActions is a "discovery" mechanism that enables clients to know which Actions a device supports without having to exercise the Actions themselves. This mechanism is necessary because there could be
        /// people / equipment safety issues if actions are called unexpectedly or out of a defined process sequence.
        /// It follows from this that SupportedActions must return names that match the spelling of Action names exactly, without additional descriptive text. However, returned names may use any casing
        /// because the <see cref="Action" /> ActionName parameter is case insensitive.</para>
        /// </remarks>
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

        /// <summary>
        /// Invokes the specified device-specific action.
        /// </summary>
        /// <param name="ActionName">
        /// A well known name agreed by interested parties that represents the action to be carried out. 
        /// </param>
        /// <param name="ActionParameters">List of required parameters or an <see cref="String.Empty">Empty String</see> if none are required.
        /// </param>
        /// <returns>A string response. The meaning of returned strings is set by the driver author.</returns>
        /// <exception cref="NotImplementedException">Throws this exception if an action name is not supported.
        /// of driver capabilities, but the driver must still throw an ASCOM.ActionNotImplemented exception if it is asked to 
        /// perform an action that it does not support.</exception>
        /// <exception cref="NotConnectedException">If the driver is not connected.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <exception cref="ActionNotImplementedException">It is intended that the <see cref="SupportedActions"/> method will inform clients of driver capabilities, but the driver must still throw 
        /// an <see cref="ASCOM.ActionNotImplementedException"/> exception  if it is asked to perform an action that it does not support.</exception>
        /// <example>Suppose filter wheels start to appear with automatic wheel changers; new actions could 
        /// be “FilterWheel:QueryWheels” and “FilterWheel:SelectWheel”. The former returning a 
        /// formatted list of wheel names and the second taking a wheel name and making the change, returning appropriate 
        /// values to indicate success or failure.
        /// </example>
        /// <remarks><p style="color:red"><b>Can throw a not implemented exception</b></p> 
        /// This method is intended for use in all current and future device types and to avoid name clashes, management of action names 
        /// is important from day 1. A two-part naming convention will be adopted - <b>DeviceType:UniqueActionName</b> where:
        /// <list type="bullet">
        /// <item><description>DeviceType is the string name of the device type e.g. Telescope, Camera, Switch etc.</description></item>
        /// <item><description>UniqueActionName is a single word, or multiple words joined by underscore characters, that sensibly describes the action to be performed.</description></item>
        /// </list>
        /// <para>
        /// It is recommended that UniqueActionNames should be a maximum of 16 characters for legibility.
        /// Should the same function and UniqueActionName be supported by more than one type of device, the reserved DeviceType of 
        /// “General” will be used. Action names will be case insensitive, so FilterWheel:SelectWheel, filterwheel:selectwheel 
        /// and FILTERWHEEL:SELECTWHEEL will all refer to the same action.</para>
        /// <para>The names of all supported actions must be returned in the <see cref="SupportedActions"/> property.</para>
        /// </remarks>
        public string Action(string ActionName, string ActionParameters)
        {
            return Device.Action(ActionName, ActionParameters);
        }

        /// <summary>
        /// Transmits an arbitrary string to the device and does not wait for a response.
        /// Optionally, protocol framing characters may be added to the string before transmission.
        /// </summary>
        /// <param name="Command">The literal command string to be transmitted.</param>
        /// <param name="Raw">
        /// if set to <c>true</c> the string is transmitted 'as-is'.
        /// If set to <c>false</c> then protocol framing characters may be added prior to transmission.
        /// </param>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="NotConnectedException">If the driver is not connected.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>May throw a NotImplementedException.</b></p>
        /// <para>The CommandXXX methods are a historic mechanic that provides clients with direct and unimpeded access to change device hardware configuration. While highly enabling for clients, this mechanic is inherently risky
        /// because clients can fundamentally change hardware operation without the driver being aware that a change is taking / has taken place.</para>
        /// <para>The newer Action / SupportedActions mechanic provides discrete, named, functions that can deliver any functionality required.They do need driver authors to make provision for them within the 
        /// driver, but this approach is much lower risk than using the CommandXXX methods because it enables the driver to resolve conflicts between standard device interface commands and extended commands 
        /// provided as Actions.The driver is always aware of what is happening and can adapt more effectively to client needs.</para>
        /// </remarks>
        public void CommandBlind(string Command, bool Raw = false)
        {
            Device.CommandBlind(Command, Raw);
        }

        /// <summary>
        /// Transmits an arbitrary string to the device and waits for a boolean response.
        /// Optionally, protocol framing characters may be added to the string before transmission.
        /// </summary>
        /// <param name="Command">The literal command string to be transmitted.</param>
        /// <param name="Raw">
        /// if set to <c>true</c> the string is transmitted 'as-is'.
        /// If set to <c>false</c> then protocol framing characters may be added prior to transmission.
        /// </param>
        /// <returns>
        /// Returns the interpreted boolean response received from the device.
        /// </returns>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="NotConnectedException">If the driver is not connected.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>May throw a NotImplementedException.</b></p>
        /// <para>The CommandXXX methods are a historic mechanic that provides clients with direct and unimpeded access to change device hardware configuration. While highly enabling for clients, this mechanic is inherently risky
        /// because clients can fundamentally change hardware operation without the driver being aware that a change is taking / has taken place.</para>
        /// <para>The newer Action / SupportedActions mechanic provides discrete, named, functions that can deliver any functionality required.They do need driver authors to make provision for them within the 
        /// driver, but this approach is much lower risk than using the CommandXXX methods because it enables the driver to resolve conflicts between standard device interface commands and extended commands 
        /// provided as Actions.The driver is always aware of what is happening and can adapt more effectively to client needs.</para>
        /// </remarks>
        public bool CommandBool(string Command, bool Raw = false)
        {
            return Device.CommandBool(Command, Raw);
        }

        /// <summary>
        /// Transmits an arbitrary string to the device and waits for a string response.
        /// Optionally, protocol framing characters may be added to the string before transmission.
        /// </summary>
        /// <param name="Command">The literal command string to be transmitted.</param>
        /// <param name="Raw">
        /// if set to <c>true</c> the string is transmitted 'as-is'.
        /// If set to <c>false</c> then protocol framing characters may be added prior to transmission.
        /// </param>
        /// <returns>
        /// Returns the string response received from the device.
        /// </returns>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="NotConnectedException">If the driver is not connected.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>May throw a NotImplementedException.</b></p>
        /// <para>The CommandXXX methods are a historic mechanic that provides clients with direct and unimpeded access to change device hardware configuration. While highly enabling for clients, this mechanic is inherently risky
        /// because clients can fundamentally change hardware operation without the driver being aware that a change is taking / has taken place.</para>
        /// <para>The newer Action / SupportedActions mechanic provides discrete, named, functions that can deliver any functionality required.They do need driver authors to make provision for them within the 
        /// driver, but this approach is much lower risk than using the CommandXXX methods because it enables the driver to resolve conflicts between standard device interface commands and extended commands 
        /// provided as Actions.The driver is always aware of what is happening and can adapt more effectively to client needs.</para>
        /// </remarks>
        public string CommandString(string Command, bool Raw = false)
        {
            return Device.CommandString(Command, Raw);
        }

        /// <summary>
        /// This method is a "clean-up" method that is primarily of use to drivers that are written in languages such as C# and VB.NET where resource clean-up is initially managed by the language's 
        /// runtime garbage collection mechanic. Driver authors should take care to ensure that a client or runtime calling Dispose() does not adversely affect other connected clients.
        /// Applications should not call this method.
        /// </summary>
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

        /// <summary>
        /// Connect to a device asynchronously
        /// </summary>
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

        /// <summary>
        /// Disconnect from a device asynchronously
        /// </summary>
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

        /// <summary>
        /// Asynchronous connection completion variable.
        /// </summary>
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

        /// <summary>
        /// Returns a List of device operational state values as IStateValue objects.
        /// </summary>
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
