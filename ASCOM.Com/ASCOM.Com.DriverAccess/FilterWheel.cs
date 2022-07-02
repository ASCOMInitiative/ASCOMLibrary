using ASCOM.Common.DeviceInterfaces;
using System.Collections.Generic;
using ASCOM.Common;

namespace ASCOM.Com.DriverAccess
{
    /// <summary>
    /// FilterWheel device class
    /// </summary>
    public class FilterWheel : ASCOMDevice, IFilterWheelV2
    {
        /// <summary>
        /// Return a list of all FilterWheels registered in the ASCOM Profile
        /// </summary>
        public static List<ASCOMRegistration> FilterWheels => Profile.GetDrivers(DeviceTypes.FilterWheel);

        /// <summary>
        /// Initialise FilterWheel device
        /// </summary>
        /// <param name="ProgID">COM ProgID of the device.</param>
        public FilterWheel(string ProgID) : base(ProgID)
        {

        }

        /// <summary>
        /// Returns a description of the driver, such as manufacturer and model
        /// number. Any ASCII characters may be used. The string shall not exceed 68
        /// characters (for compatibility with FITS headers).
        /// </summary>
        /// <value>The description.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public new string Description
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return string.Empty;
                }
                return base.Description;
            }
        }

        /// <summary>
        /// Descriptive and version information about this ASCOM driver.
        /// This string may contain line endings and may be hundreds to thousands of characters long.
        /// It is intended to display detailed information on the ASCOM driver, including version and copyright data.
        /// See the Description property for descriptive info on the telescope itself.
        /// To get the driver version in a parseable string, use the DriverVersion property.
        /// </summary>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public new string DriverInfo
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return string.Empty;
                }
                return base.DriverInfo;
            }
        }

        /// <summary>
        /// A string containing only the major and minor version of the driver.
        /// This must be in the form "n.n".
        /// Not to be confused with the InterfaceVersion property, which is the version of this specification supported by the driver (currently 2). 
        /// </summary>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public new string DriverVersion
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return string.Empty;
                }
                return base.DriverVersion;
            }
        }

        /// <summary>
        /// The short name of the driver, for display purposes
        /// </summary>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public new string Name
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return string.Empty;
                }
                return base.Name;
            }
        }

        /// <summary>
        /// Focus offset of each filter in the wheel
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// For each valid slot number (from 0 to N-1), reports the focus offset for the given filter position.  These values are focuser and filter dependent, and  would usually be set up by the user via 
        /// the SetupDialog. The number of slots N can be determined from the length of the array. If focuser offsets are not available, then it should report back 0 for all array values.
        /// </remarks>
        public int[] FocusOffsets => base.Device.FocusOffsets;

        /// <summary>
        /// Name of each filter in the wheel
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// For each valid slot number (from 0 to N-1), reports the name given to the filter position.  These names would usually be set up by the user via the
        /// SetupDialog.  The number of slots N can be determined from the length of the array.  If filter names are not available, then it should report back "Filter 1", "Filter 2", etc.
        /// </remarks>
        public string[] Names => base.Device.Names;

        /// <summary>
        /// Sets or returns the current filter wheel position
        /// </summary>
        /// <exception cref="InvalidValueException">Must throw an InvalidValueException if an invalid position is set</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// Write a position number between 0 and N-1, where N is the number of filter slots (see <see cref="Names"/>). Starts filter wheel rotation immediately when written. Reading
        /// the property gives current slot number (if wheel stationary) or -1 if wheel is moving. 
        /// <para>Returning a position of -1 is <b>mandatory</b> while the filter wheel is in motion; valid slot numbers must not be reported back while the filter wheel is rotating past filter positions.</para>
        /// <para><b>Note</b></para>
        /// <para>Some filter wheels are built into the camera (one driver, two interfaces).  Some cameras may not actually rotate the wheel until the exposure is triggered.  In this case, the written value is available
        /// immediately as the read value, and -1 is never produced.</para>
        /// </remarks>
        public short Position { get => base.Device.Position; set => base.Device.Position = value; }
    }
}
