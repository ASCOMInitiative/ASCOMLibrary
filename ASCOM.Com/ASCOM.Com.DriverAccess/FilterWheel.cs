﻿using ASCOM.Common.DeviceInterfaces;
using System.Collections.Generic;
using ASCOM.Common;
using ASCOM.Common.Interfaces;
using ASCOM.Common.DeviceStateClasses;

namespace ASCOM.Com.DriverAccess
{
    /// <summary>
    /// FilterWheel device class
    /// </summary>
    public class FilterWheel : ASCOMDevice, IFilterWheelV3
    {

        #region Convenience members

        /// <summary>
        /// Return a list of all FilterWheels registered in the ASCOM Profile
        /// </summary>
        public static List<ASCOMRegistration> FilterWheels => Profile.GetDrivers(DeviceTypes.FilterWheel);

        /// <summary>
        /// FilterWheel device state
        /// </summary>
        public FilterWheelState FilterWheelState
        {
            get
            {
                // Create a state object to return.
                FilterWheelState filterWheelState = new FilterWheelState(DeviceState, TL);
                TL.LogMessage(LogLevel.Debug,nameof(FilterWheelState), $"Returning: '{filterWheelState.Position}' '{filterWheelState.TimeStamp}'");

                // Return the device specific state class
                return filterWheelState;
            }
        }

        #endregion

        #region Initialisers

        /// <summary>
        /// Initialise FilterWheel device
        /// </summary>
        /// <param name="ProgID">COM ProgID of the device.</param>
        public FilterWheel(string ProgID) : base(ProgID)
        {
            deviceType = DeviceTypes.FilterWheel; ;
        }

        /// <summary>
        /// Initialise FilterWheel device with a debug logger
        /// </summary>
        /// <param name="ProgID">ProgID of the driver</param>
        /// <param name="logger">Logger instance to receive debug information.</param>
        public FilterWheel(string ProgID, ILogger logger) : base(ProgID)
        {
            deviceType = DeviceTypes.FilterWheel;
            TL = logger;
        }

        #endregion

        #region IFilterWheelV2 and IFilterWheelV3

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
        public int[] FocusOffsets => Device.FocusOffsets;

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
        public string[] Names => Device.Names;

        /// <summary>
        /// Sets or returns the current filter wheel position
        /// </summary>
        /// <exception cref="InvalidValueException">Must throw an InvalidValueException if an invalid position is set</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a PropertyNotImplementedException.</b></p>
        ///<para>This is an asynchronous operation: Writing to Position must return as soon as the filter change operation has been successfully started. Reading the Position property must return -1 while the change
        /// is in progress. After the requested position has been successfully reached and motion stops, reading Position must return the requested new filter number.</para>
		/// Write a position number between 0 and N-1, where N is the number of filter slots (see <see cref="Names"/>). Starts filter wheel rotation immediately. Reading
		/// the property gives current slot number (if wheel stationary) or -1 if wheel is moving.
		/// <para>Returning a position of -1 is <b>mandatory</b> while the filter wheel is in motion; valid slot numbers must not be reported back while the filter wheel is rotating past filter positions.</para>
        /// <para><b>Note</b></para>
        /// <para>Some filter wheels are built into the camera (one driver, two interfaces).  Some cameras may not actually rotate the wheel until the exposure is triggered.  In this case, the written value is available
        /// immediately as the read value, and -1 is never produced.</para>
        /// </remarks>
        public short Position { get => Device.Position; set => Device.Position = value; }

        #endregion
    }
}
