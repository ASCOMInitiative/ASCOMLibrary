using ASCOM.Common.DeviceInterfaces;
using System.Collections.Generic;
using ASCOM.Common;

namespace ASCOM.Com.DriverAccess
{
    /// <summary>
    /// Focuser device class
    /// </summary>
    public class Focuser : ASCOMDevice, IFocuserV3
    {
        /// <summary>
        /// Return a list of all Focusers registered in the ASCOM Profile
        /// </summary>
        public static List<ASCOMRegistration> Focusers => Profile.GetDrivers(DeviceTypes.Focuser);

        /// <summary>
        /// Initialise Focuser device
        /// </summary>
        /// <param name="ProgID">COM ProgID of the device.</param>
        public Focuser(string ProgID) : base(ProgID)
        {
        }

        /// <summary>
        /// Set True to enable the link. Set False to disable the link.
        /// You can also read the property to check whether it is connected.
        /// </summary>
        /// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        public new bool Connected
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return base.Device.Link;
                }
                return base.Connected;
            }
            set
            {
                if (InterfaceVersion == 1)
                {
                    base.Device.Link = value;
                }
                base.Connected = value;
            }
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
        /// True if the focuser is capable of absolute position; that is, being commanded to a specific step location.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Must be implemented</b></p> </remarks>
        public bool Absolute => base.Device.Absolute;

        /// <summary>
        /// True if the focuser is currently moving to a new position. False if the focuser is stationary.
        /// </summary>
        /// <exception cref="NotConnectedException">If the driver is not connected.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Must be implemented</b></p></remarks>
        public bool IsMoving => base.Device.IsMoving;

        /// <summary>
        /// Maximum increment size allowed by the focuser; 
        /// i.e. the maximum number of steps allowed in one move operation.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented</b></p>
        /// For most focusers this is the same as the <see cref="MaxStep" /> property. This is normally used to limit the Increment display in the host software.
        /// </remarks>
        public int MaxIncrement => base.Device.MaxIncrement;

        /// <summary>
        /// Maximum step position permitted.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented</b></p>
        /// The focuser can step between 0 and <see cref="MaxStep" />. If an attempt is made to move the focuser beyond these limits, it will automatically stop at the limit.
        /// </remarks>
        public int MaxStep => base.Device.MaxStep;

        /// <summary>
        /// Current focuser position, in steps.
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not available for this device.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Can throw a not implemented exception</b></p> Valid only for absolute positioning focusers (see the <see cref="Absolute" /> property).
        /// A <see cref="NotImplementedException">NotImplementedException</see> exception must be thrown if this device is a relative positioning focuser rather than an absolute position focuser.
        /// </remarks>
        public int Position => base.Device.Position;

        /// <summary>
        /// Step size (microns) for the focuser.
        /// </summary>
        /// <exception cref= "NotImplementedException">If the focuser does not intrinsically know what the step size is.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Can throw a not implemented exception</b></p> Must throw an exception if the focuser does not intrinsically know what the step size is.</remarks>
        public double StepSize => base.Device.StepSize;

        /// <summary>
        /// The state of temperature compensation mode (if available), else always False.
        /// </summary>
        /// <exception cref="NotImplementedException">If <see cref="TempCompAvailable" /> is False and an attempt is made to set <see cref="TempComp" /> to true.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red;margin-bottom:0"><b>TempComp Read must be implemented and must not throw a NotImplementedException. </b></p>
        /// <p style="color:red;margin-top:0"><b>TempComp Write can throw a NotImplementedException.</b></p>
        /// If the <see cref="TempCompAvailable" /> property is True, then setting <see cref="TempComp" /> to True puts the focuser into temperature tracking mode; setting it to False will turn off temperature tracking.
        /// <para>If temperature compensation is not available, this property must always return False.</para>
        /// <para> A <see cref="NotImplementedException" /> exception must be thrown if <see cref="TempCompAvailable" /> is False and an attempt is made to set <see cref="TempComp" /> to true.</para>
        /// <para><b>BEHAVIOURAL CHANGE - Platform 6.4</b></para>
        /// <para>Prior to Platform 6.4, the interface specification mandated that drivers must throw an <see cref="InvalidOperationException"/> if a move was attempted when <see cref="TempComp"/> was True, even if the focuser 
        /// was able to execute the move safely without disrupting temperature compensation.</para>
        /// <para>Following discussion on ASCOM-Talk in January 2018, the Focuser interface specification has been revised to IFocuserV3, removing the requrement to throw the InvalidOperationException exception. IFocuserV3 compliant drivers 
        /// are expected to execute Move requests when temperature compensation is active and to hide any specific actions required by the hardware from the client. For example this could be achieved by disabling temperature compensation, moving the focuser and re-enabling 
        /// temperature compensation or simply by moving the focuser with compensation enabled if the hardware supports this.</para>
        /// <para>Conform will continue to pass IFocuserV2 drivers that throw InvalidOperationException exceptions. However, Conform will now fail IFocuserV3 drivers that throw InvalidOperationException exceptions, in line with this revised specification.</para>
        /// </remarks>
        public bool TempComp { get => base.Device.TempComp; set => base.Device.TempComp = value; }

        /// <summary>
        /// True if focuser has temperature compensation available.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented</b></p>
        /// Will be True only if the focuser's temperature compensation can be turned on and off via the <see cref="TempComp" /> property. 
        /// </remarks>
        public bool TempCompAvailable => base.Device.TempCompAvailable;

        /// <summary>
        /// Current ambient temperature as measured by the focuser.
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not available for this device.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Can throw a not implemented exception</b></p> 
        /// Raises an exception if ambient temperature is not available. Commonly available on focusers with a built-in temperature compensation mode. 
        /// </remarks>
        public double Temperature => base.Device.Temperature;

        /// <summary>
        /// Immediately stop any focuser motion due to a previous <see cref="Move" /> method call.
        /// </summary>
        /// <exception cref="NotImplementedException">Focuser does not support this method.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Can throw a not implemented exception</b></p>Some focusers may not support this function, in which case an exception will be raised. 
        /// <para><b>Recommendation:</b> Host software should call this method upon initialization and,
        /// if it fails, disable the Halt button in the user interface.</para>
        /// </remarks>
        public void Halt()
        {
            base.Device.Halt();
        }

        /// <summary>
        ///  Moves the focuser by the specified amount or to the specified position depending on the value of the <see cref="Absolute" /> property.
        /// </summary>
        /// <param name="Position">Step distance or absolute position, depending on the value of the <see cref="Absolute" /> property.</param>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Must be implemented</b></p>
        /// <para>If the <see cref="Absolute" /> property is True, then this is an absolute positioning focuser. The <see cref="Move">Move</see> command tells the focuser to move to an exact step position, and the Position parameter 
        /// of the <see cref="Move">Move</see> method is an integer between 0 and <see cref="MaxStep" />.</para>
        /// <para>If the <see cref="Absolute" /> property is False, then this is a relative positioning focuser. The <see cref="Move">Move</see> command tells the focuser to move in a relative direction, and the Position parameter 
        /// of the <see cref="Move">Move</see> method (in this case, step distance) is an integer between minus <see cref="MaxIncrement" /> and plus <see cref="MaxIncrement" />.</para>
        /// <para><b>BEHAVIOURAL CHANGE - Platform 6.4</b></para>
        /// <para>Prior to Platform 6.4, the interface specification mandated that drivers must throw an <see cref="InvalidOperationException"/> if a move was attempted when <see cref="TempComp"/> was True, even if the focuser 
        /// was able to execute the move safely without disrupting temperature compensation.</para>
        /// <para>Following discussion on ASCOM-Talk in January 2018, the Focuser interface specification has been revised to IFocuserV3, removing the requrement to throw the InvalidOperationException exception. IFocuserV3 compliant drivers 
        /// are expected to execute Move requests when temperature compensation is active and to hide any specific actions required by the hardware from the client. For example this could be achieved by disabling temperature compensation, moving the focuser and re-enabling 
        /// temperature compensation or simply by moving the focuser with compensation enabled if the hardware supports this.</para>
        /// <para>Conform will continue to pass IFocuserV2 drivers that throw InvalidOperationException exceptions. However, Conform will now fail IFocuserV3 drivers that throw InvalidOperationException exceptions, in line with this revised specification.</para>
        /// </remarks>
        public void Move(int Position)
        {
            base.Device.Move(Position);
        }
    }
}