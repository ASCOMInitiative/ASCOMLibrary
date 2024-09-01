namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Switch interface version 3, which incorporates the new members in IAscomDeviceV2 and the members present in ISwitchV2 plus the new ISwitchV3 asynchronous methods.
    /// </summary>
    public interface ISwitchV3 : IAscomDeviceV2, ISwitchV2
    {
        /// <summary>
        /// Set a boolean switch's state asynchronously
        /// </summary>
        /// <exception cref="MethodNotImplementedException">When CanAsync(id) is false.</exception>
        /// <param name="id">Switch number.</param>
        /// <param name="state">New boolean state.</param>
        /// <remarks>
        /// <p style="color:red"><b>This is an optional method and can throw a <see cref="MethodNotImplementedException"/> when <see cref="CanAsync(short)"/> is <see langword="false"/>.</b></p>
        /// <para>Further explanation is available in this link: <a href="https://ascom-standards.org/newdocs/telescope.html#switch-setasync" target="_blank">Master Interface Document</a>.</para>
        /// </remarks>
        void SetAsync(short id, bool state);

        /// <summary>
        /// Set a switch's value asynchronously
        /// </summary>
        /// <param name="id">Switch number.</param>
        /// <param name="value">New double value.</param>
        /// <p style="color:red"><b>This is an optional method and can throw a <see cref="MethodNotImplementedException"/> when <see cref="CanAsync(short)"/> is <see langword="false"/>.</b></p>
        /// <exception cref="MethodNotImplementedException">When CanAsync(id) is false.</exception>
        /// <remarks>
        /// <p style="color:red"><b>This is an optional method and can throw a <see cref="MethodNotImplementedException"/> when <see cref="CanAsync(short)"/> is <see langword="false"/>.</b></p>
        /// <para>Further explanation is available in this link: <a href="https://ascom-standards.org/newdocs/telescope.html#switch-setasyncvalue" target="_blank">Master Interface Document</a>.</para>
        /// </remarks>
        void SetAsyncValue(short id, double value);

        /// <summary>
        /// Flag indicating whether this switch can operate asynchronously.
        /// </summary>
        /// <param name="id">Switch number.</param>
        /// <returns>True if the switch can operate asynchronously.</returns>
		/// <remarks>
        /// <p style="color:red"><b>This is a mandatory method and must not throw a <see cref="MethodNotImplementedException"/>.</b></p>
		/// </remarks>
        bool CanAsync(short id);

        /// <summary>
        /// Completion variable for asynchronous changes.
        /// </summary>
        /// <param name="id">Switch number.</param>
        /// <returns>False while an asynchronous operation is underway and true when it has completed.</returns>
        /// <exception cref="OperationCancelledException">When an in-progress operation is cancelled by the <see cref="CancelAsync(short)"/> method.</exception>
        /// <remarks>
        /// <p style="color:red"><b>This is a mandatory method and must not throw a <see cref="MethodNotImplementedException"/>.</b></p>
        /// </remarks>
        bool StateChangeComplete(short id);

        /// <summary>
        /// Cancels an in-progress asynchronous operation.
        /// </summary>
        /// <param name="id">Switch number.</param>
        /// <remarks>
        /// <p style="color:red"><b>This is an optional method and can throw a <see cref="MethodNotImplementedException"/>.</b></p>
        /// This method must be implemented if it is possible for the device to cancel an asynchronous state change operation, otherwise it must throw a <see cref="MethodNotImplementedException"/>.
        /// </remarks>
        void CancelAsync(short id);
    }
}