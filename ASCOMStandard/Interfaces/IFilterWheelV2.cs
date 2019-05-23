namespace ASCOM.Alpaca.Interfaces
{
    // Imports ASCOM.Conform
    // -----------------------------------------------------------------------
    // <summary>Defines the IFilterWheel Interface</summary>
    // -----------------------------------------------------------------------
    /// <summary>
    /// Defines the IFilterWheel Interface
    /// </summary>
    public interface IFilterWheelV2 : IAscomDevice
    {

        /// <summary>
        /// Focus offset of each filter in the wheel
        ///     '''</summary>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// For each valid slot number (from 0 to N-1), reports the focus offset for the given filter position.  These values are focuser and filter dependent, and  would usually be set up by the user via 
        /// the SetupDialog. The number of slots N can be determined from the length of the array. If focuser offsets are not available, then it should report back 0 for all array values.
        /// </remarks>
        int[] FocusOffsets { get; }

        /// <summary>
        /// Name of each filter in the wheel
        ///     '''</summary>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// For each valid slot number (from 0 to N-1), reports the name given to the filter position.  These names would usually be set up by the user via the
        /// SetupDialog.  The number of slots N can be determined from the length of the array.  If filter names are not available, then it should report back "Filter 1", "Filter 2", etc.
        /// </remarks>
        string[] Names { get; }

        /// <summary>
        /// Sets or returns the current filter wheel position
        /// </summary>
        /// <exception cref="InvalidValueException">Must throw an InvalidValueException if an invalid position is set</exception>
        /// <exception cref="NotConnectedException">Must throw an exception if the Filter Wheel is not connected</exception>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// Write a position number between 0 and N-1, where N is the number of filter slots (see <see cref="Names"/>). Starts filter wheel rotation immediately when written. Reading
        /// the property gives current slot number (if wheel stationary) or -1 if wheel is moving. 
        /// <para>Returning a position of -1 is <b>mandatory</b> while the filter wheel is in motion; valid slot numbers must not be reported back while the filter wheel is rotating past filter positions.</para>
        /// <para><b>Note</b></para>
        /// <para>Some filter wheels are built into the camera (one driver, two interfaces).  Some cameras may not actually rotate the wheel until the exposure is triggered.  In this case, the written value is available
        /// immediately as the read value, and -1 is never produced.</para>
        /// </remarks>
        short Position { get; set; }
    }
}