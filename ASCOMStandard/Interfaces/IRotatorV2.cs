namespace ASCOM.Alpaca.Interfaces
{
    // -----------------------------------------------------------------------
    // <summary>Defines the IRotator Interface</summary>
    // -----------------------------------------------------------------------
    /// <summary>
    /// Defines the IRotator Interface
    /// </summary>
    public interface IRotatorV2 : IAscomDriver
    {

        /// <summary>
        /// Indicates whether the Rotator supports the <see cref="Reverse" /> method.
        /// </summary>
        /// <returns>
        /// True if the Rotator supports the <see cref="Reverse" /> method.
        /// </returns>
        /// <remarks>
        /// <p style="color:red;margin-bottom:0"><b>Must be implemented and must not throw a NotImplementedException. </b></p>
        /// </remarks>
        bool CanReverse { get; }

        /// <summary>
        /// Immediately stop any Rotator motion due to a previous <see cref="Move">Move</see> or <see cref="MoveAbsolute">MoveAbsolute</see> method call.
        /// </summary>
        /// <remarks>This is an optional method. Raises an error if not supported.</remarks>
        /// <exception cref="NotImplementedException">Throw a NotImplementedException if the rotator cannot halt.</exception>
        void Halt();

        /// <summary>
        /// Indicates whether the rotator is currently moving
        /// </summary>
        /// <returns>True if the Rotator is moving to a new position. False if the Rotator is stationary.</returns>
        /// <exception cref="NotImplementedException">If the property is not implemented.</exception>
        /// <remarks>Rotation is asynchronous, that is, when the <see cref="Move">Move</see> method is called, it starts the rotation, then returns 
        /// immediately. During rotation, <see cref="IsMoving" /> must be True, else it must be False.</remarks>
        bool IsMoving { get; }

        /// <summary>
        /// Causes the rotator to move Position degrees relative to the current <see cref="Position" /> value.
        /// </summary>
        /// <exception cref="NotImplementedException">If the method is not implemented.</exception>
        /// <exception cref="InvalidValueException">If Position is invalid.</exception>
        /// <param name="Position">Relative position to move in degrees from current <see cref="Position" />.</param>
        /// <remarks>
        /// Calling <see cref="Move">Move</see> causes the <see cref="TargetPosition" /> property to change to the sum of the current angular position 
        /// and the value of the <see cref="Position" /> parameter (modulo 360 degrees), then starts rotation to <see cref="TargetPosition" />.
        /// </remarks>
        void Move(float Position);

        /// <summary>
        /// Causes the rotator to move the absolute position of <see cref="Position" /> degrees.
        /// </summary>
        /// <param name="Position">Absolute position in degrees.</param>
        /// <exception cref="NotImplementedException">If the method is not implemented.</exception>
        /// <exception cref="InvalidValueException">If Position is invalid.</exception>
        /// <remarks>Calling <see cref="MoveAbsolute">MoveAbsolute</see> causes the <see cref="TargetPosition" /> property to change to the value of the
        /// <see cref="Position" /> parameter, then starts rotation to <see cref="TargetPosition" />. </remarks>
        void MoveAbsolute(float Position);

        /// <summary>
        /// Current instantaneous Rotator position, in degrees.
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not implemented.</exception>
        /// <remarks>
        /// The position is expressed as an angle from 0 up to but not including 360 degrees, counter-clockwise against the 
        /// sky. This is the standard definition of Position Angle. However, the rotator does not need to (and in general will not) 
        /// report the true Equatorial Position Angle, as the attached imager may not be precisely aligned with the rotator's indexing. 
        /// It is up to the client to determine any offset between mechanical rotator position angle and the true Equatorial Position 
        /// Angle of the imager, and compensate for any difference. 
        /// <para>The optional <see cref="Reverse" /> property is provided in order to manage rotators being used on optics with odd or 
        /// even number of reflections. With the Reverse switch in the correct position for the optics, the reported position angle must 
        /// be counter-clockwise against the sky.</para>
        /// </remarks>
        float Position { get; }

        /// <summary>
        /// Sets or Returns the rotator’s Reverse state.
        /// </summary>
        /// <value>True if the rotation and angular direction must be reversed for the optics</value>
        /// <exception cref="NotImplementedException">Throw a NotImplementedException if the rotator cannot reverse and CanReverse is False.</exception>
        /// <remarks>See the definition of <see cref="Position" />. Raises an error if not supported. </remarks>
        bool Reverse { get; set; }

        /// <summary>
        /// The minimum StepSize, in degrees.
        /// </summary>
        /// <exception cref="NotImplementedException">Throw a NotImplementedException if the rotator does not know its step size.</exception>
        /// <remarks>
        /// Raises an exception if the rotator does not intrinsically know what the step size is.
        /// </remarks>
        float StepSize { get; }

        /// <summary>
        /// The destination position angle for Move() and MoveAbsolute().
        /// </summary>
        /// <value>The destination position angle for<see cref="Move">Move</see> and <see cref="MoveAbsolute">MoveAbsolute</see>.</value>
        /// <exception cref="NotImplementedException">If the property is not implemented.</exception>
        /// <remarks>
        /// Upon calling <see cref="Move">Move</see> or <see cref="MoveAbsolute">MoveAbsolute</see>, this property immediately changes to the position angle to which the rotator is moving. The value is retained until a subsequent call 
        /// to <see cref="Move">Move</see> or <see cref="MoveAbsolute">MoveAbsolute</see>.
        /// </remarks>
        float TargetPosition { get; }
    }
}