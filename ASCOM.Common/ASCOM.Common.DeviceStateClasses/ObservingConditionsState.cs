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
    public class ObservingConditionsState
    {
        // Assign the name of this class
        readonly string className = nameof(ObservingConditionsState);

        /// <summary>
        /// Create a new ObservingConditionsState instance
        /// </summary>
        public ObservingConditionsState() { }

        /// <summary>
        /// Create a new ObservingConditionsState instance from the device's DeviceState response.
        /// </summary>
        /// <param name="deviceState">The device's DeviceState response.</param>
        /// <param name="TL">Debug TraceLogger instance.</param>
        public ObservingConditionsState(IList<IStateValue> deviceState, ILogger TL)
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
                        case nameof(IObservingConditionsV2.CloudCover):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    CloudCover = jsonElement.GetDouble();
                                else                                             // COM returns objects that can just be cast to the required type
                                    CloudCover = (double)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"CloudCover - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"CloudCover has value: {CloudCover.HasValue}, Value: {CloudCover}");
                            break;

                        case nameof(IObservingConditionsV2.DewPoint):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    DewPoint = jsonElement.GetDouble();
                                else                                             // COM returns objects that can just be cast to the required type
                                    DewPoint = (double)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"DewPoint - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"DewPoint has value: {DewPoint.HasValue} , Value:  {DewPoint}");
                            break;

                        case nameof(IObservingConditionsV2.Humidity):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    Humidity = jsonElement.GetDouble();
                                else                                             // COM returns objects that can just be cast to the required type
                                    Humidity = (double)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"Humidity - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"Humidity has value: {Humidity.HasValue} , Value:  {Humidity}");
                            break;

                        case nameof(IObservingConditionsV2.Pressure):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    Pressure = jsonElement.GetDouble();
                                else                                             // COM returns objects that can just be cast to the required type
                                    Pressure = (double)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"Pressure - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"Pressure has value: {Pressure.HasValue} , Value:  {Pressure}");
                            break;

                        case nameof(IObservingConditionsV2.RainRate):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    RainRate = jsonElement.GetDouble();
                                else                                             // COM returns objects that can just be cast to the required type
                                    RainRate = (double)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"RainRate - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"RainRate has value: {RainRate.HasValue}, Value: {RainRate}");
                            break;

                        case nameof(IObservingConditionsV2.SkyBrightness):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    SkyBrightness = jsonElement.GetDouble();
                                else                                             // COM returns objects that can just be cast to the required type
                                    SkyBrightness = (double)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"SkyBrightness - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"SkyBrightness has value: {SkyBrightness.HasValue}, Value: {SkyBrightness}");
                            break;

                        case nameof(IObservingConditionsV2.SkyQuality):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    SkyQuality = jsonElement.GetDouble();
                                else                                             // COM returns objects that can just be cast to the required type
                                    SkyQuality = (double)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"SkyQuality - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"SkyQuality has value: {SkyQuality.HasValue}, Value: {SkyQuality}");
                            break;

                        case nameof(IObservingConditionsV2.SkyTemperature):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    SkyTemperature = jsonElement.GetDouble();
                                else                                             // COM returns objects that can just be cast to the required type
                                    SkyTemperature = (double)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"SkyTemperature - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"SkyTemperature has value: {SkyTemperature.HasValue}, Value: {SkyTemperature}");
                            break;

                        case nameof(IObservingConditionsV2.StarFWHM):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    StarFWHM = jsonElement.GetDouble();
                                else                                             // COM returns objects that can just be cast to the required type
                                    StarFWHM = (double)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"StarFWHM - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"StarFWHM has value: {StarFWHM.HasValue}, Value: {StarFWHM}");
                            break;

                        case nameof(IObservingConditionsV2.Temperature):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    Temperature = jsonElement.GetDouble();
                                else                                             // COM returns objects that can just be cast to the required type
                                    Temperature = (double)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"Temperature - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"Temperature has value: {Temperature.HasValue}, Value: {Temperature}");
                            break;

                        case nameof(IObservingConditionsV2.WindDirection):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    WindDirection = jsonElement.GetDouble();
                                else                                             // COM returns objects that can just be cast to the required type
                                    WindDirection = (double)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"WindDirection - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"WindDirection has value: {WindDirection.HasValue}, Value: {WindDirection}");
                            break;

                        case nameof(IObservingConditionsV2.WindGust):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    WindGust = jsonElement.GetDouble();
                                else                                             // COM returns objects that can just be cast to the required type
                                    WindGust = (double)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"WindGust - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"WindGust has value: {WindGust.HasValue}, Value: {WindGust}");
                            break;

                        case nameof(IObservingConditionsV2.WindSpeed):
                            try
                            {
                                if (stateValue.Value is JsonElement jsonElement) // Deal with Alpaca, which returns JsonElement types instead of object
                                    WindSpeed = jsonElement.GetDouble();
                                else                                             // COM returns objects that can just be cast to the required type
                                    WindSpeed = (double)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"WindSpeed - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"WindSpeed has value: {WindSpeed.HasValue}, Value: {WindSpeed}");
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
        public double? CloudCover { get; set; } = null;

        /// <summary>
        /// Telescope is at home
        /// </summary>
        public double? DewPoint { get; set; } = null;

        /// <summary>
        /// Telescope is parked
        /// </summary>
        public double? Humidity { get; set; } = null;

        /// <summary>
        /// Telescope azimuth
        /// </summary>
        public double? Pressure { get; set; } = null;

        /// <summary>
        /// Telescope declination
        /// </summary>
        public double? RainRate { get; set; } = null;

        /// <summary>
        /// Telescope is pulse guiding
        /// </summary>
        public double? SkyBrightness { get; set; } = null;

        /// <summary>
        /// Telescope right ascension
        /// </summary>
        public double? SkyQuality { get; set; } = null;

        /// <summary>
        /// Telescope pointing state
        /// </summary>
        public double? SkyTemperature { get; set; } = null;

        /// <summary>
        /// Telescope sidereal time
        /// </summary>
        public double? StarFWHM { get; set; } = null;

        /// <summary>
        /// Telescope is slewing
        /// </summary>
        public double? Temperature { get; set; } = null;

        /// <summary>
        /// Telescope  is tracking
        /// </summary>
        public double? WindDirection { get; set; } = null;

        /// <summary>
        /// Telescope UTC date and time
        /// </summary>
        public double? WindGust { get; set; } = null;

        /// <summary>
        /// Telescope UTC date and time
        /// </summary>
        public double? WindSpeed { get; set; } = null;
        /// <summary>
        /// The time at which the state was recorded
        /// </summary>
        public DateTime? TimeStamp { get; set; } = null;
    }
}

