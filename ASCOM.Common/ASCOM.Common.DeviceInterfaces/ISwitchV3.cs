namespace ASCOM.Common.DeviceInterfaces
{
    public interface ISwitchV3 : IAscomDeviceV2, ISwitchV2
    {
        /// <summary>
        /// Set a boolean switch's state asynchronously
        /// </summary>
        /// <param name="id">Switch number.</param>
        /// <param name="state">New boolean state.</param>
        void SetAsync(short id, bool state);

        /// <summary>
        /// Set a switch's value asynchronously
        /// </summary>
        /// <param name="id">Switch number.</param>
        /// <param name="value">New double value.</param>
        void SetAsyncValue(short id, double value);

        /// <summary>
        /// Flag indicating whether this switch can operate asynchronously.
        /// </summary>
        /// <param name="id">Switch number.</param>
        /// <returns>True if the switch can operate asynchronously.</returns>
        bool CanAsync(short id);

        /// <summary>
        /// Completion variable for asynchronous changes.
        /// </summary>
        /// <param name="id">Switch number.</param>
        /// <returns>False while an asynchronous operation is underway and true when it has completed.</returns>
        /// <exception cref="OperationCancelledException">When an in-progress operation is cancelled by the <see cref="CancelAsync(short)"/> method.</exception>
        bool StateChangeComplete(short id);

        /// <summary>
        /// Cancels an in-progress asynchronous operation.
        /// </summary>
        /// <param name="id">Switch number.</param>
        void CancelAsync(short id);
    }
}