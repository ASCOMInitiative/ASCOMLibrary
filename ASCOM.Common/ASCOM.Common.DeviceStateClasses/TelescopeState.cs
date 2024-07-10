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
    public class TelescopeState
    {
        // Assign the name of this class
        readonly string className = nameof(TelescopeState);

        /// <summary>
        /// Create a new TelescopeState instance
        /// </summary>
        public TelescopeState() { }

        /// <summary>
        /// Create a new TelescopeState instance from the device's DeviceState response.
        /// </summary>
        /// <param name="deviceState">The device's DeviceState response.</param>
        /// <param name="TL">Debug TraceLogger instance.</param>
        public TelescopeState(List<StateValue> deviceState, ILogger TL)
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
                        case nameof(ITelescopeV4.Altitude):
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

                        case nameof(ITelescopeV4.AtHome):
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

                        case nameof(ITelescopeV4.AtPark):
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

                        case nameof(ITelescopeV4.Azimuth):
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

                        case nameof(ITelescopeV4.Declination):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    Declination = jsonElement.GetDouble();
                                else                                             // COM returns objects that can just be cast to the required type
                                    Declination = (double)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"Declination - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"Declination has value: {Declination.HasValue}, Value: {Declination}");
                            break;

                        case nameof(ITelescopeV4.IsPulseGuiding):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    IsPulseGuiding = jsonElement.GetBoolean();
                                else                                             // COM returns objects that can just be cast to the required type
                                    IsPulseGuiding = (bool)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"IsPulseGuiding - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"IsPulseGuiding has value: {IsPulseGuiding.HasValue}, Value: {IsPulseGuiding}");
                            break;

                        case nameof(ITelescopeV4.RightAscension):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    RightAscension = jsonElement.GetDouble();
                                else                                             // COM returns objects that can just be cast to the required type
                                    RightAscension = (double)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"RightAscension - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"RightAscension has value: {RightAscension.HasValue}, Value: {RightAscension}");
                            break;

                        case nameof(ITelescopeV4.SideOfPier):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    SideOfPier = (PointingState)jsonElement.GetInt32();
                                else                                             // COM returns objects that can just be cast to the required type
                                    SideOfPier = (PointingState)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"SideOfPier - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"SideOfPier has value: {SideOfPier.HasValue}, Value: {SideOfPier}");
                            break;

                        case nameof(ITelescopeV4.SiderealTime):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    SiderealTime = jsonElement.GetDouble();
                                else                                             // COM returns objects that can just be cast to the required type
                                    SiderealTime = (double)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"SiderealTime - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"SiderealTime has value: {SiderealTime.HasValue}, Value: {SiderealTime}");
                            break;

                        case nameof(ITelescopeV4.Slewing):
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
                            TL?.LogMessage(LogLevel.Debug, className, $"Slewing has value: {Slewing.HasValue}, Value: {Slewing}");
                            break;

                        case nameof(ITelescopeV4.Tracking):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    Tracking = jsonElement.GetBoolean();
                                else                                             // COM returns objects that can just be cast to the required type
                                    Tracking = (bool)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"Tracking - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"Tracking has value: {Tracking.HasValue}, Value: {Tracking}");
                            break;

                        case nameof(ITelescopeV4.UTCDate):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    UTCDate = jsonElement.GetDateTime();
                                else                                             // COM returns objects that can just be cast to the required type
                                    UTCDate = (DateTime)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"UTCDate - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"UTCDate has value: {UTCDate.HasValue}, Value: {UTCDate}");
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
        /// Telescope altitude
        /// </summary>
        public double? Altitude { get; set; } = null;

        /// <summary>
        /// Telescope is at home
        /// </summary>
        public bool? AtHome { get; set; } = null;

        /// <summary>
        /// Telescope is parked
        /// </summary>
        public bool? AtPark { get; set; } = null;

        /// <summary>
        /// Telescope azimuth
        /// </summary>
        public double? Azimuth { get; set; } = null;

        /// <summary>
        /// Telescope declination
        /// </summary>
        public double? Declination { get; set; } = null;

        /// <summary>
        /// Telescope is pulse guiding
        /// </summary>
        public bool? IsPulseGuiding { get; set; } = null;

        /// <summary>
        /// Telescope right ascension
        /// </summary>
        public double? RightAscension { get; set; } = null;

        /// <summary>
        /// Telescope pointing state
        /// </summary>
        public PointingState? SideOfPier { get; set; } = null;

        /// <summary>
        /// Telescope sidereal time
        /// </summary>
        public double? SiderealTime { get; set; } = null;

        /// <summary>
        /// Telescope is slewing
        /// </summary>
        public bool? Slewing { get; set; } = null;

        /// <summary>
        /// Telescope  is tracking
        /// </summary>
        public bool? Tracking { get; set; } = null;

        /// <summary>
        /// Telescope UTC date and time
        /// </summary>
        public DateTime? UTCDate { get; set; } = null;

        /// <summary>
        /// The time at which the state was recorded
        /// </summary>
        public DateTime? TimeStamp { get; set; } = null;
    }
}

