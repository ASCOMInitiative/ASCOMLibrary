namespace ASCOM.Standard.Interfaces
{

    // -----------------------------------------------------------------------
    // <summary>Defines the IDome Interface</summary>
    // -----------------------------------------------------------------------
    /// <summary>
    /// Defines the IDome Interface
    /// </summary>
    /// <remarks>
    /// This interface is used to handle a dome, with or without a controllable shutter, and also a roll off roof.
    /// <para>The dome implentation should be self explanatory.</para>
    /// <para>A roll off roof is implemented using the shutter control as the roof.  The properties and methods shoud be implented as follows:
    /// <list>
    /// <item><description>OpenShutter and CloseShutter open and close the roof.</description></item>
    /// <item><description>CanFindHome, CanPark,CanSetAltitude, CanSetAzimuth, CanSetPark, CanSlave and CanSyncAzimuth all return false.</description></item>
    /// <item><description>CanSetShutter returns true.</description></item>
    /// <item><description>ShutterStatus is implemented.</description></item>
    /// <item><description>Slewing always returns false.</description></item>
    /// <item><description>AbortSlew should stop the shutter moving.</description></item>
    /// <item><description>FindHome, Park, SetPark, SlewToAltitude, SlewToAzimuth and SyncToAzimuth all throw the <see cref="NotImplementedException" /></description></item>
    /// <item><description>Altitude and Azimuth throw the <see cref="NotImplementedException"/>.</description></item>
    /// </list></para>
    /// </remarks>
    public interface IDomeV2 : IAscomDevice
    {

        /// <summary>
        /// Immediately cancel current dome operation.
        /// </summary>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// Calling this method will immediately disable hardware slewing (<see cref="Slaved" /> will become False). Raises an error if a communications failure occurs, or if the command is known to have failed. 
        /// </remarks>
        void AbortSlew();

        /// <summary>
        /// The dome altitude (degrees, horizon zero and increasing positive to 90 zenith).
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <remarks>
        /// Raises an error only if no altitude control. If actual dome altitude can not be read, then reports back the last slew position. 
        /// </remarks>
        double Altitude { get; }

        /// <summary>
        /// Indicates whether the dome is in the home position. Raises an error if not supported. 
        /// <para>
        /// This is normally used following a <see cref="FindHome" /> operation. The value is reset with any azimuth slew operation that moves the dome away from the home position.
        /// </para>
        /// <para>
        /// <see cref="AtHome" /> may also become true durng normal slew operations, if the dome passes through the home position and the dome controller hardware is capable of detecting that; 
        /// or at the end of a slew operation if the dome comes to rest at the home position.
        /// </para>
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <remarks>
        /// <para>The home position is normally defined by a hardware sensor positioned around the dome circumference and represents a fixed, known azimuth reference.</para>
        /// <para>For some devices, the home position may represent a small range of azimuth values, rather than a discrete value, since dome inertia, the resolution of the home position sensor and/or the azimuth encoder may be
        /// insufficient to return the exact same azimuth value on each occasion. Some dome controllers, on the other hand, will always force the azimuth reading to a fixed value whenever the home position sensor is active.
        /// Because of these potential differences in behaviour, applications should not rely on the reported azimuth position being identical each time <see cref="AtHome" /> is set <c>true</c>.</para>
        /// </remarks>
        /// [ASCOM-135] TPL - Updated documentation
        bool AtHome { get; }

        /// <summary>
        /// True if the dome is in the programmed park position.
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <remarks>
        /// Set only following a <see cref="Park" /> operation and reset with any slew operation. Raises an error if not supported. 
        /// </remarks>
        bool AtPark { get; }

        /// <summary>
        /// The dome azimuth (degrees, North zero and increasing clockwise, i.e., 90 East, 180 South, 270 West)
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <remarks>Raises an error only if no azimuth control. If actual dome azimuth can not be read, then reports back last slew position</remarks>
        double Azimuth { get; }

        /// <summary>
        /// True if driver can do a search for home position.
        /// </summary>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// </remarks>
        bool CanFindHome { get; }

        /// <summary>
        /// True if driver is capable of setting dome altitude.
        /// </summary>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// </remarks>
        bool CanPark { get; }

        /// <summary>
        /// True if driver is capable of setting dome altitude.
        /// </summary>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// </remarks>
        bool CanSetAltitude { get; }

        /// <summary>
        /// True if driver is capable of setting dome azimuth.
        /// </summary>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// </remarks>
        bool CanSetAzimuth { get; }

        /// <summary>
        /// True if driver can set the dome park position.
        /// </summary>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// </remarks>
        bool CanSetPark { get; }

        /// <summary>
        /// True if driver is capable of automatically operating shutter.
        /// </summary>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// </remarks>
        bool CanSetShutter { get; }

        /// <summary>
        /// True if the dome hardware supports slaving to a telescope.
        /// </summary>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// See the notes for the <see cref="Slaved" /> property.
        /// </remarks>
        bool CanSlave { get; }

        /// <summary>
        /// True if driver is capable of synchronizing the dome azimuth position using the <see cref="SyncToAzimuth" /> method.
        /// </summary>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// </remarks>
        bool CanSyncAzimuth { get; }

        /// <summary>
        /// Close shutter or otherwise shield telescope from the sky.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        void CloseShutter();

        /// <summary>
        /// Start operation to search for the dome home position.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <remarks>
        /// After Home position is established initializes <see cref="Azimuth" /> to the default value and sets the <see cref="AtHome" /> flag. 
        /// Exception if not supported or communications failure. Raises an error if <see cref="Slaved" /> is True.
        /// </remarks>
        void FindHome();

        /// <summary>
        /// Open shutter or otherwise expose telescope to the sky.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <remarks>
        /// Raises an error if not supported or if a communications failure occurs. 
        /// </remarks>
        void OpenShutter();

        /// <summary>
        /// Rotate dome in azimuth to park position.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <remarks>
        /// After assuming programmed park position, sets <see cref="AtPark" /> flag. Raises an error if <see cref="Slaved" /> is True, or if not supported, or if a communications failure has occurred. 
        /// </remarks>
        void Park();

        /// <summary>
        /// Set the current azimuth, altitude position of dome to be the park position.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <remarks>
        /// Raises an error if not supported or if a communications failure occurs. 
        /// </remarks>
        void SetPark();

        /// <summary>
        /// Status of the dome shutter or roll-off roof.
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <remarks>
        /// Raises an error only if no shutter control. If actual shutter status can not be read, then reports back the last shutter state. 
        /// </remarks>
        ShutterState ShutterStatus { get; }

        /// <summary>
        /// True if the dome is slaved to the telescope in its hardware, else False.
        /// </summary>
        /// <remarks>
        /// <p style="color:red;margin-bottom:0"><b>Slaved Read must be implemented and must not throw a NotImplementedException. </b></p>
        /// <p style="color:red;margin-top:0"><b>Slaved Write can throw a NotImplementedException.</b></p>
        /// Set this property to True to enable dome-telescope hardware slaving, if supported (see <see cref="CanSlave" />). Raises an exception on any attempt to set 
        /// this property if hardware slaving is not supported). Always returns False if hardware slaving is not supported. 
        /// </remarks>
        bool Slaved { get; set; }

        /// <summary>
        /// True if any part of the dome is currently moving, False if all dome components are steady.
        /// </summary>
        /// <remarks>
        /// <p style="color:red;margin-bottom:0"><b>Slewing must be implemented and must not throw a NotImplementedException. </b></p>
        /// Raises an error if <see cref="Slaved" /> is True, if not supported, if a communications failure occurs, or if the dome can not reach indicated azimuth. 
        /// </remarks>
        bool Slewing { get; }

        /// <summary>
        /// Slew the dome to the given altitude position.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="InvalidValueException">If the supplied altitude is outside the range 0..90 degrees.</exception>
        /// <remarks>
        /// Raises an error if <see cref="Slaved" /> is True, if not supported, if a communications failure occurs, or if the dome can not reach indicated altitude. 
        /// </remarks>
        /// <param name="Altitude">Target dome altitude (degrees, horizon zero and increasing positive to 90 zenith)</param>
        void SlewToAltitude(double Altitude);

        /// <summary>
        /// Slew the dome to the given azimuth position.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="InvalidValueException">If the supplied azimuth is outside the range 0..360 degrees.</exception>
        /// <remarks>
        /// Raises an error if <see cref="Slaved" /> is True, if not supported, if a communications failure occurs, or if the dome can not reach indicated azimuth. 
        /// </remarks>
        /// <param name="Azimuth">Target azimuth (degrees, North zero and increasing clockwise. i.e., 90 East, 180 South, 270 West)</param>
        void SlewToAzimuth(double Azimuth);

        /// <summary>
        /// Synchronize the current position of the dome to the given azimuth.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented</exception>
        /// <exception cref="InvalidValueException">If the supplied azimuth is outside the range 0..360 degrees.</exception>
        /// <remarks>
        /// Raises an error if not supported or if a communications failure occurs. 
        /// </remarks>
        /// <param name="Azimuth">Target azimuth (degrees, North zero and increasing clockwise. i.e., 90 East, 180 South, 270 West)</param>
        void SyncToAzimuth(double Azimuth);
    }
}