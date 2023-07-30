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
    public class CoverCalibratorState
    {
        // Assign the name of this class
        string className = nameof(CoverCalibratorState);

        /// <summary>
        /// Create a new CoverCalibratorState instance
        /// </summary>
        public CoverCalibratorState() { }

        /// <summary>
        /// Create a new CoverCalibratorState instance from the device's DeviceState response.
        /// </summary>
        /// <param name="deviceState">The device's DeviceState response.</param>
        /// <param name="TL">Debug TraceLogger instance.</param>
        public CoverCalibratorState(IList<IStateValue> deviceState, ILogger TL)
        {
            TL?.LogMessage(LogLevel.Debug, className, $"Received {deviceState.Count} items");

            // Handle null List
            if (deviceState is null) // No List was supplied so return
            {
                TL?.LogMessage(LogLevel.Debug, className, $"Supplied device state List is null, all values will be unknown.");
                return;
            }

            // An ArrayList was supplied so process each supplied value
            foreach (IStateValue stateValue in deviceState)
            {
                try
                {
                    TL?.LogMessage(LogLevel.Debug, className, $"{stateValue.Name} = {stateValue.Value}");

                    switch (stateValue.Name)
                    {
                        case nameof(ICoverCalibratorV2.Brightness):
                            try
                            {
                                Brightness = (int)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"Brightness - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"Brightness has value: {Brightness.HasValue}, Value: {Brightness}");
                            break;

                        case nameof(ICoverCalibratorV2.CalibratorState):
                            try
                            {
                                CalibratorState = (CalibratorStatus)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"CalibratorState - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"CalibratorState has value: {CalibratorState.HasValue}, Value: {CalibratorState}");
                            break;

                        case nameof(ICoverCalibratorV2.CoverState):
                            try
                            {
                                CoverState = (CoverStatus)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"CoverState - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"CoverState has value: {CoverState.HasValue}, Value: {CoverState}");
                            break;

                        case nameof(ICoverCalibratorV2.CalibratorReady):
                            try
                            {
                                CalibratorReady = (bool)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"CalibratorReady - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"CalibratorReady has value: {CalibratorReady.HasValue}, Value: {CalibratorReady}");
                            break;

                        case nameof(ICoverCalibratorV2.CoverMoving):
                            try
                            {
                                CoverMoving = (bool)stateValue.Value;
                            }
                            catch (Exception ex)
                            {
                                TL?.LogMessage(LogLevel.Debug, className, $"CoverMoving - Ignoring exception: {ex.Message}");
                            }
                            TL?.LogMessage(LogLevel.Debug, className, $"CoverMoving has value: {CoverMoving.HasValue}, Value: {CoverMoving}");
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
        /// The device's Brightness
        /// </summary>
        public double? Brightness { get; set; } = null;

        /// <summary>
        /// The device's CalibratorState
        /// </summary>
        public CalibratorStatus? CalibratorState { get; set; } = null;

        /// <summary>
        /// The device's CoverState
        /// </summary>
        public CoverStatus? CoverState { get; set; } = null;

        /// <summary>
        /// The device's CalibratorReady state
        /// </summary>
        public bool? CalibratorReady { get; set; } = null;

        /// <summary>
        /// The device's CoverMoving state
        /// </summary>
        public bool? CoverMoving { get; set; } = null;

        /// <summary>
        /// The time at which the state was recorded
        /// </summary>
        public DateTime? TimeStamp { get; set; } = null;

    }
}
