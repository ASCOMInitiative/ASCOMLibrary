namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Provides support for polar axis aligners and optical axis aligners that measure movement in degrees, millimeters and counts. 
    /// </summary>
    /// <remarks>
    /// <para>The interface is intentionally flexible to support up to three degrees of freedom and allow for future aligner types.</para>
    /// <para>In addition, all position related properties are optional to provide support for a wide range of devices from those that only have relative movement and no knowledge of their position 
    /// to those that know exactly where they are and can move to any specified coordinates within their supported range.</para>
    /// </remarks>
    public interface IAlignerV1 : IAscomDeviceV2
    {
        #region Device Properties

        /// <summary>
        /// Returns a configuration object that implements <see cref="IAlignerConfigurationV1"/> and contains all configuration values for the device.
        /// </summary>
        /// <exception cref="NotConnectedException">If the device is not connected</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception> 
        /// <remarks>
        /// <para><b style="color:red">Must be implemented</b> Cannot throw a <see cref="PropertyNotImplementedException"/>.</para>
        /// <para><b>TRIAL - </b >Historically we received feedback that retrieving many individual configuration values is inefficient; this new approach is an attempt to address that feedback. Instead of individual properties for each configuration value, the <see cref="Configuration"/> property 
        /// returns an object that contains all configuration values. In the case of the <see cref="IAlignerV1"/> interface it reduces eighteen individual calls to one.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        IAlignerConfigurationV1 Configuration { get; }

        /// <summary>
        /// True while any move or homing is in progress.
        /// </summary>
        /// <exception cref="NotConnectedException">When the device is not connected.</exception>
        /// <exception cref="DriverException">When a driver error occurs.</exception>
        /// <remarks>
        /// <para><b style="color:red">Must be implemented</b> Cannot throw a <see cref="PropertyNotImplementedException"/>.</para>
        /// <para>This is the completion property for the <see cref="Move"/>, <see cref="MoveAbsolute"/> and <see cref="FindHome"/> methods.</para>
        /// <para><see cref="IsMoving"/> must stay true until the axes are at rest, including any backlash take-up the device performs after reaching the target. 
        /// If the operation fails, reading this property returns an error until the next operation that uses it starts; see <see href="https://ascom-standards.org/newdocs/async-faq.html">Asynchronous Operations in ASCOM</see> for further information.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        bool IsMoving { get; }

        /// <summary>
        /// Returns the position of the X axis from the device's zero in <see cref="AlignerUnits"/>.
        /// </summary>
        /// <exception cref="PropertyNotImplementedException">When the device does not track its position or can not control that axis.</exception>
        /// <exception cref="NotConnectedException">When the device is not connected.</exception>
        /// <exception cref="DriverException">When a driver error occurs.</exception>
        /// <remarks>
        /// <para>See the <see href="AlignerFAQ.htm">Aligner FAQ</see> for specific <see cref="AlignerType"/> implementation requirements.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        double XPosition { get; }

        /// <summary>
        /// Returns the position of the Y axis from the device's zero in <see cref="AlignerUnits"/>.
        /// </summary>
        /// <exception cref="PropertyNotImplementedException">When the device does not track its position or can not control that axis.</exception>
        /// <exception cref="NotConnectedException">When the device is not connected.</exception>
        /// <exception cref="DriverException">When a driver error occurs.</exception>
        /// <remarks>
        /// <para>See the <see href="AlignerFAQ.htm">Aligner FAQ</see> for specific <see cref="AlignerType"/> implementation requirements.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        double YPosition { get; }

        /// <summary>
        /// Returns the position of the Z axis from the device's zero in <see cref="AlignerUnits"/>.
        /// </summary>
        /// <exception cref="PropertyNotImplementedException">When the device does not track its position or can not control that axis.</exception>
        /// <exception cref="NotConnectedException">When the device is not connected.</exception>
        /// <exception cref="DriverException">When a driver error occurs.</exception>
        /// <remarks>
        /// <para>See the <see href="AlignerFAQ.htm">Aligner FAQ</see> for specific <see cref="AlignerType"/> implementation requirements.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        double ZPosition { get; }

        #endregion

        #region Device methods

        /// <summary>
        /// Starts movement of all axes to their home positions.
        /// </summary>
        /// <exception cref="MethodNotImplementedException">When <see cref="IAlignerConfigurationV1.CanFindHome"/> is false.</exception>
        /// <exception cref="NotConnectedException">When the device is not connected.</exception>
        /// <exception cref="DriverException">When a driver error occurs.</exception>
        /// <remarks>
        /// <para>Operates asynchronously, setting IsMoving true before returning. On completion IsMoving becomes false. A device that cannot move axes concurrently moves them one after another within the same operation.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        void FindHome();

        /// <summary>
        /// Halts movement on all axes.
        /// </summary>
        /// <exception cref="NotConnectedException">When the device is not connected.</exception>
        /// <exception cref="DriverException">When a driver error occurs.</exception>
        /// <remarks>
        /// <para><b style="color:red">Must be implemented</b> Cannot throw a <see cref="MethodNotImplementedException"/>.</para>
        /// <para>Operates synchronously, returning when all motion has stopped. IsMoving must be False on return. Calling <see cref="Halt"/> while idle does nothing and does not throw.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        void Halt();

        /// <summary>
        /// Starts a relative move of every axis by the given amounts in <see cref="AlignerUnits"/> units.
        /// </summary>
        /// <param name="X">The amount to move the X axis in <see cref="AlignerUnits"/> units. Use 0.0 when <see cref="IAlignerConfigurationV1.CanMoveX"/> is false.</param>
        /// <param name="Y">The amount to move the Y axis in <see cref="AlignerUnits"/> units. Use 0.0 when <see cref="IAlignerConfigurationV1.CanMoveY"/> is false.</param>
        /// <param name="Z">The amount to move the Z axis in <see cref="AlignerUnits"/> units. Use 0.0 when <see cref="IAlignerConfigurationV1.CanMoveZ"/> is false.</param>
        /// <exception cref="InvalidValueException">When any of the requested movements cannot be achieved.</exception>
        /// <exception cref="InvalidOperationException">When the operation cannot be performed e.g., when the device is already moving or when it is uncalibrated.</exception>
        /// <exception cref="NotConnectedException">When the device is not connected.</exception>
        /// <exception cref="DriverException">When a driver error occurs.</exception>
        /// <remarks>
        /// <para><b style="color:red">Must be implemented</b> Cannot throw a <see cref="MethodNotImplementedException"/>.</para>
        /// <para>Operates asynchronously, setting IsMoving true before returning. On completion IsMoving becomes false. A device that cannot move axes concurrently moves them one after another within the same operation.</para>
        /// <para>See the <see href="AlignerFAQ.htm">Aligner FAQ</see> for <see cref="IAlignerConfigurationV1.AlignerType"/> specific <see cref="AlignerUnits"/> implementation requirements.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        void Move(double X, double Y, double Z);

        /// <summary>
        /// Starts a move of every axis to the given positions in <see cref="AlignerUnits"/> units.
        /// </summary>
        /// <param name="X">The position to which the X axis should move in <see cref="AlignerUnits"/> units. Use 0.0 when <see cref="IAlignerConfigurationV1.CanMoveX"/> is false.</param>
        /// <param name="Y">The position to which the Y axis should move in <see cref="AlignerUnits"/> units. Use 0.0 when <see cref="IAlignerConfigurationV1.CanMoveY"/> is false.</param>
        /// <param name="Z">The position to which the Z axis should move in <see cref="AlignerUnits"/> units. Use 0.0 when <see cref="IAlignerConfigurationV1.CanMoveZ"/> is false.</param>
        /// <exception cref="MethodNotImplementedException">When <see cref="IAlignerConfigurationV1.CanMoveAbsolute"/> is false.</exception>
        /// <exception cref="InvalidValueException">When any of the requested positions cannot be reached.</exception>
        /// <exception cref="InvalidOperationException">When the operation cannot be performed e.g., when the device is already moving or when it is uncalibrated.</exception>
        /// <exception cref="NotConnectedException">When the device is not connected.</exception>
        /// <exception cref="DriverException">When a driver error occurs.</exception>
        /// <remarks>
        /// <para>See the <see href="AlignerFAQ.htm">Aligner FAQ</see> for <see cref="IAlignerConfigurationV1.AlignerType"/> specific <see cref="AlignerUnits"/> implementation requirements.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        void MoveAbsolute(double X, double Y, double Z);

        /// <summary>
        /// Sets the device coordinates to the provided values in <see cref="AlignerUnits"/> without moving the device.
        /// </summary>
        /// <param name="X">The X axis value of the current device position in <see cref="AlignerUnits"/> units. Use 0.0 when <see cref="IAlignerConfigurationV1.CanMoveX"/> is false.</param>
        /// <param name="Y">The Y axis value of the current device position in <see cref="AlignerUnits"/> units. Use 0.0 when <see cref="IAlignerConfigurationV1.CanMoveY"/> is false.</param>
        /// <param name="Z">The Z axis value of the current device position in <see cref="AlignerUnits"/> units. Use 0.0 when <see cref="IAlignerConfigurationV1.CanMoveZ"/> is false.</param>
        /// <exception cref="MethodNotImplementedException">When <see cref="IAlignerConfigurationV1.CanSync"/> is false.</exception>
        /// <exception cref="InvalidValueException">When any of the requested coordinates cannot be set.</exception>
        /// <exception cref="InvalidOperationException">When the operation cannot be performed e.g., when the device is already moving or when it is uncalibrated.</exception>
        /// <exception cref="NotConnectedException">When the device is not connected.</exception>
        /// <exception cref="DriverException">When a driver error occurs.</exception>
        /// <remarks>
        /// <para>Operates synchronously. IsMoving must be False on return.</para>
        /// <para>See the <see href="AlignerFAQ.htm">Aligner FAQ</see> for <see cref="IAlignerConfigurationV1.AlignerType"/> specific <see cref="AlignerUnits"/> implementation requirements.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        void Sync(double X, double Y, double Z);

        #endregion
    }
}
