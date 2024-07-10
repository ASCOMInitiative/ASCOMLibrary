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
    public class DomeState
    {
        // Assign the name of this class
        readonly string className = nameof(DomeState);

        /// <summary>
        /// Create a new DomeState instance
        /// </summary>
        public DomeState() { }

        /// <summary>
        /// Create a new DomeState instance from the device's DeviceState response.
        /// </summary>
        /// <param name="deviceState">The device's DeviceState response.</param>
        /// <param name="TL">Debug TraceLogger instance.</param>
        public DomeState(List<StateValue> deviceState, ILogger TL)
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
                        case nameof(IDomeV3.Altitude):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    Altitude = jsonElement.GetDouble();
                                else                                             // COM returns objects that can just be cast to the required type
                                    Altitude = (double)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"Altitude - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"Altitude has value: {Altitude.HasValue}, Value: {Altitude}");
                            break;

                        case nameof(IDomeV3.AtHome):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    AtHome = jsonElement.GetBoolean();
                                else                                             // COM returns objects that can just be cast to the required type
                                    AtHome = (bool)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"AtHome - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"AtHome has value: {AtHome.HasValue}, Value: {AtHome}");
                            break;

                        case nameof(IDomeV3.AtPark):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    AtPark = jsonElement.GetBoolean();
                                else                                             // COM returns objects that can just be cast to the required type
                                    AtPark = (bool)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"AtPark - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"AtPark has value: {AtPark.HasValue}, Value: {AtPark}");
                            break;

                        case nameof(IDomeV3.Azimuth):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    Azimuth = jsonElement.GetDouble();
                                else                                             // COM returns objects that can just be cast to the required type
                                    Azimuth = (double)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"Azimuth - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"Azimuth has value: {Azimuth.HasValue}, Value: {Azimuth}");
                            break;

                        case nameof(IDomeV3.ShutterStatus):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    ShutterStatus = (ShutterState)jsonElement.GetInt32();
                                else                                             // COM returns objects that can just be cast to the required type
                                    ShutterStatus = (ShutterState)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"Declination - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"ShutterStatus has value: {ShutterStatus.HasValue}, Value: {ShutterStatus}");
                            break;

                        case nameof(IDomeV3.Slewing):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    Slewing = jsonElement.GetBoolean();
                                else                                             // COM returns objects that can just be cast to the required type
                                    Slewing = (bool)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"Slewing - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"Slewing has value: {Slewing.HasValue}, Value: {Slewing.Value}");
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
        /// Dome altitude
        /// </summary>
        public double? Altitude { get; set; } = null;

        /// <summary>
        /// Dome is at home
        /// </summary>
        public bool? AtHome { get; set; } = null;

        /// <summary>
        /// Dome is parked
        /// </summary>
        public bool? AtPark { get; set; } = null;

        /// <summary>
        /// Dome azimuth
        /// </summary>
        public double? Azimuth { get; set; } = null;

        /// <summary>
        /// Dome shutter state
        /// </summary>
        public ShutterState? ShutterStatus { get; set; } = null;

        /// <summary>
        /// Dome is slewing
        /// </summary>
        public bool? Slewing { get; set; } = null;

        /// <summary>
        /// The time at which the state was recorded
        /// </summary>
        public DateTime? TimeStamp { get; set; } = null;
    }
}