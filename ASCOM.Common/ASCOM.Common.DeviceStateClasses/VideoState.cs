using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ASCOM.Common.DeviceStateClasses
{
    /// <summary>
    /// Class that presents the device's operation state as a set of nullable properties
    /// </summary>
    public class VideoState
    {
        // Assign the name of this class
        string className = nameof(FilterWheelState);

        /// <summary>
        /// Create a new VideoState instance
        /// </summary>
        public VideoState() { }

        /// <summary>
        /// Create a new VideoState instance from the device's DeviceState response.
        /// </summary>
        /// <param name="deviceStateArrayList">The device's DeviceState response.</param>
        /// <param name="TL">Debug TraceLogger instance.</param>
        public VideoState(ArrayList deviceStateArrayList, ILogger TL)
        {
            TL?.LogMessage(LogLevel.Debug, className, $"Received {deviceStateArrayList.Count} items");

            List<IStateValue> deviceState = new List<IStateValue>();

            // Handle null ArrayList
            if (deviceStateArrayList is null) // No ArrayList was supplied so return
            {
                TL?.LogMessage(LogLevel.Debug, className, $"Supplied device state ArrayList is null, all values will be unknown.");
                return;
            }

            TL?.LogMessage(LogLevel.Debug, className, $"ArrayList from device contained {deviceStateArrayList.Count} DeviceSate items.");

            // An ArrayList was supplied so process each supplied value
            foreach (IStateValue stateValue in deviceStateArrayList)
            {
                try
                {
                    TL?.LogMessage(LogLevel.Debug, className, $"{stateValue.Name} = {stateValue.Value}");
                    deviceState.Add(new StateValue(stateValue.Name, stateValue.Value));

                    switch (stateValue.Name)
                    {
                        case nameof(IVideoV2.CameraState):
                            try
                            {
                                CameraState = (VideoCameraState)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"CameraState - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"CameraState has value: {CameraState.HasValue}, Value: {CameraState}");
                            break;

                        case "TimeStamp":
                            try
                            {
                                TimeStamp = (DateTime)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"TimeStamp - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"TimeStamp has value: {TimeStamp.HasValue}, Value: {TimeStamp}");
                            break;

                        default:
                            TL?.LogMessage(LogLevel.Debug, className, $"Ignoring {stateValue.Name}");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    TL?.LogMessage(LogLevel.Debug, className, $"Exception: {ex.Message}.\r\n{ex}");
                }
            }
        }

        /// <summary>
        /// Focuser IsMoving state
        /// </summary>
        public VideoCameraState? CameraState { get; set; } = null;

        /// <summary>
        /// The time at which the state was recorded
        /// </summary>
        public DateTime? TimeStamp { get; set; } = null;
    }
}