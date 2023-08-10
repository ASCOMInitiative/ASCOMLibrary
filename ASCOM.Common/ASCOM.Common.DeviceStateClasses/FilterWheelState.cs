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
    public class FilterWheelState
    {
        // Assign the name of this class
        readonly string className = nameof(FilterWheelState);

        /// <summary>
        /// Create a new FilterWheelState instance
        /// </summary>
        public FilterWheelState() { }

        /// <summary>
        /// Create a new FilterWheelState instance from the device's DeviceState response.
        /// </summary>
        /// <param name="deviceState">The device's DeviceState response.</param>
        /// <param name="TL">Debug TraceLogger instance.</param>
        public FilterWheelState(IList<IStateValue> deviceState, ILogger TL)
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
                        case nameof(IFilterWheelV3.Position):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    Position = jsonElement.GetInt16();
                                else                                             // COM returns objects that can just be cast to the required type
                                    Position = (short)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"Position - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"Position has value: {Position.HasValue}, Value: {Position}");
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
        /// FilterWheel position
        /// </summary>
        public double? Position { get; set; } = null;

        /// <summary>
        /// The time at which the state was recorded
        /// </summary>
        public DateTime? TimeStamp { get; set; } = null;
    }
}

