namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Provides universal access to Focuser drivers - Updated to IFocuserV3 - see remarks below
    /// </summary>
    /// <remarks>
    /// <para><b>SPECIFICATION REVISION - Platform 6.4</b></para>
    /// <para>The method signatures in the revised interface specification are identical to the preceeding IFocuserV2, however, the IFocuserV3.Move command must
    /// no longer throw an InvalidOperationException exception if a Move is attempted when temperature compensation is enabled.</para>
    /// </remarks>
    public interface IFocuserV3 : IAscomDevice
    {

        /// <summary>
        /// True if the focuser is capable of absolute position; that is, being commanded to a specific step location.
        /// </summary>
        /// <exception cref="NotConnectedException">If the driver must be connected in order to determine the property value.</exception>
        /// <exception cref="AlpacaException">Must throw an exception if the call was not successful</exception>
        /// <remarks><p style="color:red"><b>Must be implemented</b></p> </remarks>
        bool Absolute { get; }

        /// <summary>
        /// Immediately stop any focuser motion due to a previous <see cref="Move" /> method call.
        /// </summary>
        /// <exception cref="NotImplementedException">Focuser does not support this method.</exception>
        /// <exception cref="NotConnectedException">If the driver is not connected.</exception>
        /// <exception cref="AlpacaException">Must throw an exception if the call was not successful</exception>
        /// <remarks>
        /// <p style="color:red"><b>Can throw a not implemented exception</b></p>Some focusers may not support this function, in which case an exception will be raised. 
        /// <para><b>Recommendation:</b> Host software should call this method upon initialization and,
        /// if it fails, disable the Halt button in the user interface.</para>
        /// </remarks>
        void Halt();

        /// <summary>
        /// True if the focuser is currently moving to a new position. False if the focuser is stationary.
        /// </summary>
        /// <exception cref="NotConnectedException">If the driver is not connected.</exception>
        /// <exception cref="AlpacaException">Must throw an exception if the call was not successful</exception>
        /// <remarks><p style="color:red"><b>Must be implemented</b></p></remarks>
        bool IsMoving { get; }

        /// <summary>
        /// State of the connection to the focuser.
        /// </summary>
        /// <remarks>
        /// <exception cref="AlpacaException">Must throw an exception if the call was not successful</exception>
        /// <p style="color:red"><b>Must be implemented</b></p> Set True to start the connection to the focuser; set False to terminate the connection. 
        /// The current connection status can also be read back through this property. 
        /// An exception will be raised if the link fails to change state for any reason. 
        /// <para><b>Note</b></para>
        /// <para>The FocuserV1 interface was the only interface to name its <i>"Connect"</i> method "Link" all others named 
        /// their <i>"Connect"</i> method as "Connected". All interfaces including Focuser now have a <see cref="IAscomDevice.Connected"></see> method and this is 
        /// the recommended method to use to <i>"Connect"</i> to Focusers exposing the V2 and later interfaces.</para>
        /// <para>Do not use a NotConnectedException here, that exception is for use in other methods that require a connection in order to succeed.</para>
        /// </remarks>
        bool Link { get; set; }

        /// <summary>
        /// Maximum increment size allowed by the focuser; 
        /// i.e. the maximum number of steps allowed in one move operation.
        /// </summary>
        /// <exception cref="NotConnectedException">If the device is not connected and this information is only available when connected.</exception>
        /// <exception cref="AlpacaException">Must throw an exception if the call was not successful</exception>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented</b></p>
        /// For most focusers this is the same as the <see cref="MaxStep" /> property. This is normally used to limit the Increment display in the host software.
        /// </remarks>
        int MaxIncrement { get; }

        /// <summary>
        /// Maximum step position permitted.
        /// </summary>
        /// <exception cref="NotConnectedException">If the device is not connected and this information is only available when connected.</exception>
        /// <exception cref="AlpacaException">Must throw an exception if the call was not successful</exception>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented</b></p>
        /// The focuser can step between 0 and <see cref="MaxStep" />. If an attempt is made to move the focuser beyond these limits, it will automatically stop at the limit.
        /// </remarks>
        int MaxStep { get; }

        /// <summary>
        ///  Moves the focuser by the specified amount or to the specified position depending on the value of the <see cref="Absolute" /> property.
        /// </summary>
        /// <param name="Position">Step distance or absolute position, depending on the value of the <see cref="Absolute" /> property.</param>
        /// <exception cref="NotConnectedException">If the device is not connected.</exception>
        /// <exception cref="AlpacaException">Must throw an exception if the call was not successful</exception>
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
        ///     '''</remarks>
        void Move(int Position);

        /// <summary>
        /// Current focuser position, in steps.
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not available for this device.</exception>
        /// <exception cref="NotConnectedException">If the device is not connected and this information is only available when connected.</exception>
        /// <exception cref="AlpacaException">Must throw an exception if the call was not successful</exception>
        /// <remarks>
        /// <p style="color:red"><b>Can throw a not implemented exception</b></p> Valid only for absolute positioning focusers (see the <see cref="Absolute" /> property).
        /// A <see cref="NotImplementedException">NotImplementedException</see> exception must be thrown if this device is a relative positioning focuser rather than an absolute position focuser.
        /// </remarks>
        int Position { get; }

        /// <summary>
        /// Step size (microns) for the focuser.
        /// </summary>
        /// <exception cref= "NotImplementedException">If the focuser does not intrinsically know what the step size is.</exception>
        /// <exception cref="NotConnectedException">If the device is not connected and this information is only available when connected.</exception>
        /// <exception cref="AlpacaException">Must throw an exception if the call was not successful</exception>
        /// <remarks><p style="color:red"><b>Can throw a not implemented exception</b></p> Must throw an exception if the focuser does not intrinsically know what the step size is.</remarks>
        double StepSize { get; }

        /// <summary>
        /// The state of temperature compensation mode (if available), else always False.
        /// </summary>
        /// <exception cref="NotImplementedException">If <see cref="TempCompAvailable" /> is False and an attempt is made to set <see cref="TempComp" /> to true.</exception>
        /// <exception cref="NotConnectedException">If the device is not connected and this information is only available when connected.</exception>
        /// <exception cref="AlpacaException">Must throw an exception if the call was not successful</exception>
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
        bool TempComp { get; set; }

        /// <summary>
        /// True if focuser has temperature compensation available.
        /// </summary>
        /// <exception cref="NotConnectedException">If the device is not connected and this information is only available when connected.</exception>
        /// <exception cref="AlpacaException">Must throw an exception if the call was not successful</exception>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented</b></p>
        /// Will be True only if the focuser's temperature compensation can be turned on and off via the <see cref="TempComp" /> property. 
        /// </remarks>
        bool TempCompAvailable { get; }

        /// <summary>
        /// Current ambient temperature as measured by the focuser.
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not available for this device.</exception>
        /// <exception cref="NotConnectedException">If the device is not connected and this information is only available when connected.</exception>
        /// <exception cref="AlpacaException">Must throw an exception if the call was not successful</exception>
        /// <remarks><p style="color:red"><b>Can throw a not implemented exception</b></p> 
        /// Raises an exception if ambient temperature is not available. Commonly available on focusers with a built-in temperature compensation mode. 
        /// </remarks>
        double Temperature { get; }
    }
}