using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ASCOM.Common.DeviceStateClasses
{
    /// <summary>
    /// Class that presents the device's operation state as a set of nullable properties
    /// </summary>
    public class VideoState
    {
        // Assign the name of this class
        readonly string className = nameof(FilterWheelState);

        /// <summary>
        /// Create a new VideoState instance
        /// </summary>
        public VideoState() { }

        /// <summary>
        /// Create a new VideoState instance from the device's DeviceState response.
        /// </summary>
        /// <param name="deviceState">The device's DeviceState response.</param>
        /// <param name="TL">Debug TraceLogger instance.</param>
        public VideoState(List<StateValue> deviceState, ILogger TL)
        {
            TL?.LogMessage(LogLevel.Debug, className, $"Received {deviceState.Count} items");

            // Handle null List
            if (deviceState is null) // No List was supplied so return
            {
                TL?.LogMessage(LogLevel.Debug, className, $"Supplied device state List is null, all values will be unknown.");
                return;
            }

            TL?.LogMessage(LogLevel.Debug, className, $"List from device contained {deviceState.Count} DeviceSate items.");

            // An List was supplied so process each supplied value
            foreach (IStateValue stateValue in deviceState)
            {
                try
                {
                    TL?.LogMessage(LogLevel.Debug, className, $"{stateValue.Name} = {stateValue.Value}");

                    switch (stateValue.Name)
                    {
                        case nameof(IVideoV2.CameraState):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    CameraState = (VideoCameraState)jsonElement.GetInt32();
                                else                                             // COM returns objects that can just be cast to the required type
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
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    TimeStamp = jsonElement.GetDateTime();
                                else                                             // COM returns objects that can just be cast to the required type
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