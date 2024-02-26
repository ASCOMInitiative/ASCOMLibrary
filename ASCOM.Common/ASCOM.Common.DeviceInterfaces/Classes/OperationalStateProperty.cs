using ASCOM.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// A reference class that describes valid operational state properties
    /// </summary>
    public class OperationalStateProperty
    {
        // Name of the time stamp operational property
        private const string TIME_STAMP = "TimeStamp";

        // field to hold any supplied operational message logger
        private static ILogger logger;

        /// <summary>
        /// Default initialiser for the OperationalStateProperty class
        /// </summary>
        public OperationalStateProperty()
        {
            DeviceType = DeviceTypes.Telescope;
            StateName = "State name not set";
            TypeName = "Type name not set";
        }

        /// <summary>
        /// Initialise the device type, operational state name  and data type
        /// </summary>
        /// <param name="deviceType">ASCOM device type</param>
        /// <param name="stateName">operational state name.</param>
        /// <param name="typeName">Date type name e.g. Boolean, Double, DateTime etc. This must be the CLR type name not the C# shortcut name e.g. It must be "Boolean" and not "bool".</param>
        public OperationalStateProperty(DeviceTypes deviceType, string stateName, string typeName)
        {
            DeviceType = deviceType;
            StateName = stateName;
            TypeName = typeName;
        }

        /// <summary>
        /// ASCOM device type
        /// </summary>
        public DeviceTypes DeviceType;

        /// <summary>
        /// operational state name
        /// </summary>
        public string StateName;

        /// <summary>
        /// Date type name e.g. Boolean, Double, DateTime etc. This must be the CLR type name not the C# shortcut name e.g. It must be "Boolean" and not "bool".
        /// </summary>
        public string TypeName;

        /// <summary>
        /// Definitive list of valid operational state names and associated data types for each ASCOM device type
        /// </summary>
        public static List<OperationalStateProperty> Members = new List<OperationalStateProperty>()
        {
            // Camera operational state properties
            new OperationalStateProperty(DeviceTypes.Camera, nameof(ICameraV4.CameraState), nameof(CameraState)),
            new OperationalStateProperty(DeviceTypes.Camera, nameof(ICameraV4.CCDTemperature), nameof(Double)),
            new OperationalStateProperty(DeviceTypes.Camera, nameof(ICameraV4.CoolerPower), nameof(Double)),
            new OperationalStateProperty(DeviceTypes.Camera, nameof(ICameraV4.HeatSinkTemperature), nameof(Double)),
            new OperationalStateProperty(DeviceTypes.Camera, nameof(ICameraV4.ImageReady), nameof(Boolean)),
            new OperationalStateProperty(DeviceTypes.Camera, nameof(ICameraV4.IsPulseGuiding), nameof(Boolean)),
            new OperationalStateProperty(DeviceTypes.Camera, nameof(ICameraV4.PercentCompleted), nameof(Int16)),
            new OperationalStateProperty(DeviceTypes.Camera, TIME_STAMP, nameof(DateTime)),

            // CoverCalibrator operational state properties
            new OperationalStateProperty(DeviceTypes.CoverCalibrator, nameof(ICoverCalibratorV2.Brightness), nameof(Int32)),
            new OperationalStateProperty(DeviceTypes.CoverCalibrator, nameof(ICoverCalibratorV2.CalibratorState), nameof(CalibratorStatus)),
            new OperationalStateProperty(DeviceTypes.CoverCalibrator, nameof(ICoverCalibratorV2.CoverState), nameof(CoverStatus)),
            new OperationalStateProperty(DeviceTypes.CoverCalibrator, nameof(ICoverCalibratorV2.CalibratorChanging), nameof(Boolean)),
            new OperationalStateProperty(DeviceTypes.CoverCalibrator, nameof(ICoverCalibratorV2.CoverMoving), nameof(Boolean)),
            new OperationalStateProperty(DeviceTypes.CoverCalibrator, TIME_STAMP, nameof(DateTime)),

            // Dome operational state properties
            new OperationalStateProperty(DeviceTypes.Dome, nameof(IDomeV3.Altitude), nameof(Double)),
            new OperationalStateProperty(DeviceTypes.Dome, nameof(IDomeV3.AtHome), nameof(Boolean)),
            new OperationalStateProperty(DeviceTypes.Dome, nameof(IDomeV3.AtPark), nameof(Boolean)),
            new OperationalStateProperty(DeviceTypes.Dome, nameof(IDomeV3.Azimuth), nameof(Double)),
            new OperationalStateProperty(DeviceTypes.Dome, nameof(IDomeV3.ShutterStatus), nameof(ShutterState)),
            new OperationalStateProperty(DeviceTypes.Dome, nameof(IDomeV3.Slewing), nameof(Boolean)),
            new OperationalStateProperty(DeviceTypes.Dome, TIME_STAMP, nameof(DateTime)),

            // FilterWheel operational state properties
            new OperationalStateProperty(DeviceTypes.FilterWheel, nameof(IFilterWheelV3.Position), nameof(Int16)),
            new OperationalStateProperty(DeviceTypes.FilterWheel, TIME_STAMP, nameof(DateTime)),

            // Focuser operational state properties
            new OperationalStateProperty(DeviceTypes.Focuser, nameof(IFocuserV4.IsMoving), nameof(Boolean)),
            new OperationalStateProperty(DeviceTypes.Focuser, nameof(IFocuserV4.Position), nameof(Int32)),
            new OperationalStateProperty(DeviceTypes.Focuser, nameof(IFocuserV4.Temperature), nameof(Double)),
            new OperationalStateProperty(DeviceTypes.Focuser, TIME_STAMP, nameof(DateTime)),

            // ObservingConditions operational state properties
            new OperationalStateProperty(DeviceTypes.ObservingConditions, nameof(IObservingConditionsV2.CloudCover), nameof(Double)),
            new OperationalStateProperty(DeviceTypes.ObservingConditions, nameof(IObservingConditionsV2.DewPoint), nameof(Double)),
            new OperationalStateProperty(DeviceTypes.ObservingConditions, nameof(IObservingConditionsV2.Humidity), nameof(Double)),
            new OperationalStateProperty(DeviceTypes.ObservingConditions, nameof(IObservingConditionsV2.Pressure), nameof(Double)),
            new OperationalStateProperty(DeviceTypes.ObservingConditions, nameof(IObservingConditionsV2.RainRate), nameof(Double)),
            new OperationalStateProperty(DeviceTypes.ObservingConditions, nameof(IObservingConditionsV2.SkyBrightness), nameof(Double)),
            new OperationalStateProperty(DeviceTypes.ObservingConditions, nameof(IObservingConditionsV2.SkyQuality), nameof(Double)),
            new OperationalStateProperty(DeviceTypes.ObservingConditions, nameof(IObservingConditionsV2.SkyTemperature), nameof(Double)),
            new OperationalStateProperty(DeviceTypes.ObservingConditions, nameof(IObservingConditionsV2.StarFWHM), nameof(Double)),
            new OperationalStateProperty(DeviceTypes.ObservingConditions, nameof(IObservingConditionsV2.Temperature), nameof(Double)),
            new OperationalStateProperty(DeviceTypes.ObservingConditions, nameof(IObservingConditionsV2.WindDirection), nameof(Double)),
            new OperationalStateProperty(DeviceTypes.ObservingConditions, nameof(IObservingConditionsV2.WindGust), nameof(Double)),
            new OperationalStateProperty(DeviceTypes.ObservingConditions, nameof(IObservingConditionsV2.WindSpeed), nameof(Double)),
            new OperationalStateProperty(DeviceTypes.ObservingConditions, TIME_STAMP, nameof(DateTime)),

            // Rotator operational state properties
            new OperationalStateProperty(DeviceTypes.Rotator, nameof(IRotatorV4.IsMoving), nameof(Boolean)),
            new OperationalStateProperty(DeviceTypes.Rotator, nameof(IRotatorV4.MechanicalPosition), nameof(Single)),
            new OperationalStateProperty(DeviceTypes.Rotator, nameof(IRotatorV4.Position), nameof(Single)),
            new OperationalStateProperty(DeviceTypes.Rotator, TIME_STAMP, nameof(DateTime)),

            // SafetyMonitor operational state properties
            new OperationalStateProperty(DeviceTypes.SafetyMonitor, nameof(ISafetyMonitorV3.IsSafe), nameof(Boolean)),
            new OperationalStateProperty(DeviceTypes.SafetyMonitor, TIME_STAMP, nameof(DateTime)),

            // Switch operational state properties
            new OperationalStateProperty(DeviceTypes.Switch, TIME_STAMP, nameof(DateTime)),

            // Telescope operational state properties
            new OperationalStateProperty(DeviceTypes.Telescope, nameof(ITelescopeV4.Altitude), nameof(Double)),
            new OperationalStateProperty(DeviceTypes.Telescope, nameof(ITelescopeV4.AtHome), nameof(Boolean)),
            new OperationalStateProperty(DeviceTypes.Telescope, nameof(ITelescopeV4.AtPark), nameof(Boolean)),
            new OperationalStateProperty(DeviceTypes.Telescope, nameof(ITelescopeV4.Azimuth), nameof(Double)),
            new OperationalStateProperty(DeviceTypes.Telescope, nameof(ITelescopeV4.Declination), nameof(Double)),
            new OperationalStateProperty(DeviceTypes.Telescope, nameof(ITelescopeV4.IsPulseGuiding), nameof(Boolean)),
            new OperationalStateProperty(DeviceTypes.Telescope, nameof(ITelescopeV4.RightAscension), nameof(Double)),
            new OperationalStateProperty(DeviceTypes.Telescope, nameof(ITelescopeV4.SideOfPier), nameof(PointingState)),
            new OperationalStateProperty(DeviceTypes.Telescope, nameof(ITelescopeV4.SiderealTime), nameof(Double)),
            new OperationalStateProperty(DeviceTypes.Telescope, nameof(ITelescopeV4.Slewing), nameof(Boolean)),
            new OperationalStateProperty(DeviceTypes.Telescope, nameof(ITelescopeV4.Tracking), nameof(Boolean)),
            new OperationalStateProperty(DeviceTypes.Telescope, nameof(ITelescopeV4.UTCDate), nameof(DateTime)),
            new OperationalStateProperty(DeviceTypes.Telescope, TIME_STAMP, nameof(DateTime)),

            // Video operational state properties
            new OperationalStateProperty(DeviceTypes.Video, nameof(IVideoV2.CameraState), nameof(VideoCameraState)),
            new OperationalStateProperty(DeviceTypes.Video, TIME_STAMP, nameof(DateTime))
        };

        /// <summary>
        /// Ensures that returned state values are of the expected type
        /// </summary>
        /// <param name="deviceState">List of device state values returned by the device</param>
        /// <param name="deviceType">The device type (Camera,Telescope etc.)</param>
        /// <param name="TL">Optional ILogger instance to receive operational log messages (defaults to null)</param>
        /// <returns>A cleaned list of device state values</returns>
        /// <exception cref="InvalidValueException">If the supplied device type is not supported.</exception>
        /// <remarks>When de-serialising a List of StateValue objects, System.Text.JSON returns list entries with the object variable set to the 
        /// JsonElement type rather than the expected object type. This method parses the list entries and converts all JsonElement types to the expected 
        /// int, double, string, DateTime etc. types.
        /// </remarks>
        public static List<StateValue> Clean(List<StateValue> deviceState, DeviceTypes deviceType, ILogger TL = null)
        {
            // Define supported value types
            bool boolValue;
            string stringValue;
            float floatValue;
            double doubleValue;
            short shortValue;
            int intValue;
            DateTime dateTimeValue;
            CameraState cameraStateValue;
            CalibratorStatus calibrationStatusValue;
            CoverStatus coverStatusValue;
            ShutterState shutterStateValue;
            PointingState pointingStateValue;
            VideoCameraState videoCameraStateValue;

            // Save the supplied logger instance, if any
            logger = TL;

            // Handle null List
            if (deviceState is null) // No list was supplied
            {
                LogMessage($"The supplied device state list was null for device type: {deviceType} throwing an InvalidValueExcepetion");
                throw new InvalidValueException($"StateValue.Clean - The supplied device state list was null for device type: {deviceType}");
            }

            LogMessage($"Device type: {deviceType}, list from device contained {deviceState.Count} DeviceSate items.");

            // Create a new list to contain cleaned values
            List<StateValue> cleaned = new List<StateValue>();

            // Iterate over the supplied state values
            foreach (StateValue stateValue in deviceState)
            {
                // Find the matching member definition based on the device type and the name of the state value
                OperationalStateProperty member = OperationalStateProperty.Members.Where<OperationalStateProperty>(x => x.DeviceType == deviceType & x.StateName == stateValue.Name).FirstOrDefault();

                // Process the member if one is found, otherwise just add it to the list
                if (member != null) // A supported state value was found
                {
                    // Handle the different supported types
                    switch (member.TypeName)
                    {
                        case nameof(Boolean): // This is a bool value
                            try
                            {
                                // Do any necessary type conversion
                                if (stateValue.Value is JsonElement jsonElement) // Handle System.Text.JSON, which returns JsonElement types instead of object
                                    boolValue = jsonElement.GetBoolean();
                                else                                             // Handle COM objects that can just be cast to the required type
                                    boolValue = (bool)stateValue.Value;

                                // Add the cleaned value to the return list
                                cleaned.Add(new StateValue(member.StateName, boolValue));
                                LogMessage($"Cleaned {member.TypeName} {member.StateName} has value: {boolValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.StateName} - Ignoring exception: {ex.Message}");
                            }
                            break;

                        case nameof(String): // This is a string value
                            try
                            {
                                // Do any necessary type conversion
                                if (stateValue.Value is JsonElement jsonElement) // Handle System.Text.JSON, which returns JsonElement types instead of object
                                    stringValue = jsonElement.GetString();
                                else                                             // Handle COM objects that can just be cast to the required type
                                    stringValue = (string)stateValue.Value;

                                // Add the cleaned value to the return list
                                cleaned.Add(new StateValue(member.StateName, stringValue));
                                LogMessage($"Cleaned {member.TypeName} {member.StateName} has value: {stringValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.StateName} - Ignoring exception: {ex.Message}");
                            }
                            break;

                        case nameof(Int16): // This is a short value
                            try
                            {
                                // Do any necessary type conversion
                                if (stateValue.Value is JsonElement jsonElement) // Handle System.Text.JSON, which returns JsonElement types instead of object
                                    shortValue = jsonElement.GetInt16();
                                else                                             // Handle COM objects that can just be cast to the required type
                                    shortValue = (short)stateValue.Value;

                                // Add the cleaned value to the return list
                                cleaned.Add(new StateValue(member.StateName, shortValue));
                                LogMessage($"Cleaned {member.TypeName} {member.StateName} has value: {shortValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.StateName} - Ignoring exception: {ex.Message}");
                            }
                            break;

                        case nameof(Int32): // This is an int value
                            try
                            {
                                // Do any necessary type conversion
                                if (stateValue.Value is JsonElement jsonElement) // Handle System.Text.JSON, which returns JsonElement types instead of object
                                    intValue = jsonElement.GetInt32();
                                else                                             // Handle COM objects that can just be cast to the required type
                                    intValue = (int)stateValue.Value;

                                // Add the cleaned value to the return list
                                cleaned.Add(new StateValue(member.StateName, intValue));
                                LogMessage($"Cleaned {member.TypeName} {member.StateName} has value: {intValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.StateName} - Ignoring exception: {ex.Message}");
                            }
                            break;

                        case nameof(Single): // This is a float value
                            try
                            {
                                // Do any necessary type conversion
                                if (stateValue.Value is JsonElement jsonElement) // Handle System.Text.JSON, which returns JsonElement types instead of object
                                    floatValue = jsonElement.GetSingle();
                                else                                             // Handle COM objects that can just be cast to the required type
                                    floatValue = (float)stateValue.Value;

                                // Add the cleaned value to the return list
                                cleaned.Add(new StateValue(member.StateName, floatValue));
                                LogMessage($"Cleaned {member.TypeName} {member.StateName} has value: {floatValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.StateName} - Ignoring exception: {ex.Message}");
                            }
                            break;

                        case nameof(Double): // This is a double value
                            try
                            {
                                // Do any necessary type conversion
                                if (stateValue.Value is JsonElement jsonElement) // Handle System.Text.JSON, which returns JsonElement types instead of object
                                    doubleValue = jsonElement.GetDouble();
                                else                                             // Handle COM objects that can just be cast to the required type
                                    doubleValue = (double)stateValue.Value;

                                // Add the cleaned value to the return list
                                cleaned.Add(new StateValue(member.StateName, doubleValue));
                                LogMessage($"Cleaned {member.TypeName} {member.StateName} has value: {doubleValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.StateName} - Ignoring exception: {ex.Message}");
                            }
                            break;

                        case nameof(DateTime): // This is a date/time value
                            try
                            {
                                // Do any necessary type conversion
                                if (stateValue.Value is JsonElement jsonElement) // Handle System.Text.JSON, which returns JsonElement types instead of object
                                    dateTimeValue = jsonElement.GetDateTime();
                                else                                             // Handle COM objects that can just be cast to the required type
                                    dateTimeValue = (DateTime)stateValue.Value;

                                // Add the cleaned value to the return list
                                cleaned.Add(new StateValue(member.StateName, dateTimeValue));
                                LogMessage($"Cleaned {member.StateName} has value: {dateTimeValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.StateName} - Ignoring exception: {ex.Message}");
                            }
                            break;

                        case nameof(CameraState): // This is a CameraStste value
                            try
                            {
                                // Do any necessary type conversion
                                if (stateValue.Value is JsonElement jsonElement) // Handle System.Text.JSON, which returns JsonElement types instead of object
                                    cameraStateValue = (CameraState)jsonElement.GetInt32();
                                else                                             // Handle COM objects that can just be cast to the required type
                                    cameraStateValue = (CameraState)stateValue.Value;

                                // Add the cleaned value to the return list
                                cleaned.Add(new StateValue(member.StateName, cameraStateValue));
                                LogMessage($"Cleaned {member.StateName} has value: {cameraStateValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.StateName} - Ignoring exception: {ex.Message}");
                            }
                            break;

                        case nameof(CalibratorStatus): // This is a CalibratorStatus value
                            try
                            {
                                // Do any necessary type conversion
                                if (stateValue.Value is JsonElement jsonElement) // Handle System.Text.JSON, which returns JsonElement types instead of object
                                    calibrationStatusValue = (CalibratorStatus)jsonElement.GetInt32();
                                else                                             // Handle COM objects that can just be cast to the required type
                                    calibrationStatusValue = (CalibratorStatus)stateValue.Value;

                                // Add the cleaned value to the return list
                                cleaned.Add(new StateValue(member.StateName, calibrationStatusValue));
                                LogMessage($"Cleaned {member.StateName} has value: {calibrationStatusValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.StateName} - Ignoring exception: {ex.Message}");
                            }
                            break;

                        case nameof(CoverStatus): // This is a CoverStatus value
                            try
                            {
                                // Do any necessary type conversion
                                if (stateValue.Value is JsonElement jsonElement) // Handle System.Text.JSON, which returns JsonElement types instead of object
                                    coverStatusValue = (CoverStatus)jsonElement.GetInt32();
                                else                                             // Handle COM objects that can just be cast to the required type
                                    coverStatusValue = (CoverStatus)stateValue.Value;

                                // Add the cleaned value to the return list
                                cleaned.Add(new StateValue(member.StateName, coverStatusValue));
                                LogMessage($"Cleaned {member.StateName} has value: {coverStatusValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.StateName} - Ignoring exception: {ex.Message}");
                            }
                            break;

                        case nameof(ShutterState): // This is a ShutterState value
                            try
                            {
                                // Do any necessary type conversion
                                if (stateValue.Value is JsonElement jsonElement) // Handle System.Text.JSON, which returns JsonElement types instead of object
                                    shutterStateValue = (ShutterState)jsonElement.GetInt32();
                                else                                             // Handle COM objects that can just be cast to the required type
                                    shutterStateValue = (ShutterState)stateValue.Value;

                                // Add the cleaned value to the return list
                                cleaned.Add(new StateValue(member.StateName, shutterStateValue));
                                LogMessage($"Cleaned {member.StateName} has value: {shutterStateValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.StateName} - Ignoring exception: {ex.Message}");
                            }
                            break;

                        case nameof(PointingState): // This is a ShutterState value
                            try
                            {
                                // Do any necessary type conversion
                                if (stateValue.Value is JsonElement jsonElement) // Handle System.Text.JSON, which returns JsonElement types instead of object
                                    pointingStateValue = (PointingState)jsonElement.GetInt32();
                                else                                             // Handle COM objects that can just be cast to the required type
                                    pointingStateValue = (PointingState)stateValue.Value;

                                // Add the cleaned value to the return list
                                cleaned.Add(new StateValue(member.StateName, pointingStateValue));
                                LogMessage($"Cleaned {member.StateName} has value: {pointingStateValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.StateName} - Ignoring exception: {ex.Message}");
                            }
                            break;

                        case nameof(VideoCameraState): // This is a ShutterState value
                            try
                            {
                                // Do any necessary type conversion
                                if (stateValue.Value is JsonElement jsonElement) // Handle System.Text.JSON, which returns JsonElement types instead of object
                                    videoCameraStateValue = (VideoCameraState)jsonElement.GetInt32();
                                else                                             // Handle COM objects that can just be cast to the required type
                                    videoCameraStateValue = (VideoCameraState)stateValue.Value;

                                // Add the cleaned value to the return list
                                cleaned.Add(new StateValue(member.StateName, videoCameraStateValue));
                                LogMessage($"Cleaned {member.StateName} has value: {videoCameraStateValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.StateName} - Ignoring exception: {ex.Message}");
                            }
                            break;

                        default:
                            throw new InvalidValueException($"Unsupported member type: {member.TypeName}");
                    }
                }
                else // Not recognised so just add it to the list
                {
                    cleaned.Add(stateValue);
                }
            }

            // Special cleaning for Switch device because its members vary depending on the number of switches
            if (deviceType == DeviceTypes.Switch)
            {
                // Create a list to hold cleaned switch values
                List<StateValue> switchReturnValue = new List<StateValue>();

                // Iterate over all the values in the list
                foreach (StateValue stateValue in cleaned)
                {
                    // Log the state value
                    logger.LogMessage(LogLevel.Debug, "DeviceState", $"Found Switch state value {stateValue.Name} = {stateValue.Value}");

                    // If the value is a JsonElement convert it to its required type, otherwise add the member to the cleaned list
                    if (stateValue.Value is JsonElement element) // This is a JsonElement type
                    {
                        // Handle the different supported types
                        switch (element.ValueKind)
                        {
                            case JsonValueKind.False: // This is a bool value
                            case JsonValueKind.True: // This is a bool value
                                try
                                {
                                    // Do any necessary type conversion
                                    if (stateValue.Value is JsonElement jsonElement) // Handle System.Text.JSON, which returns JsonElement types instead of object
                                        boolValue = jsonElement.GetBoolean();
                                    else                                             // Handle COM objects that can just be cast to the required type
                                        boolValue = (bool)stateValue.Value;

                                    // Add the cleaned value to the return list
                                    switchReturnValue.Add(new StateValue(stateValue.Name, boolValue));
                                    logger.LogMessage(LogLevel.Debug, "DeviceState", $"Cleaned {stateValue.Name} has value: {boolValue}");

                                }
                                catch (Exception ex)
                                {
                                    // Log any exception and don't add the value to the cleaned list
                                    logger.LogMessage(LogLevel.Debug, "DeviceState", $"{stateValue.Name} - Ignoring exception: {ex.Message}");
                                }
                                break;

                            case JsonValueKind.Number: // This is a number value
                                try
                                {
                                    // Do any necessary type conversion
                                    if (stateValue.Value is JsonElement jsonElement) // Handle System.Text.JSON, which returns JsonElement types instead of object
                                        doubleValue = jsonElement.GetDouble();
                                    else                                             // Handle COM objects that can just be cast to the required type
                                        doubleValue = (double)stateValue.Value;

                                    // Add the cleaned value to the return list
                                    switchReturnValue.Add(new StateValue(stateValue.Name, doubleValue));
                                    logger.LogMessage(LogLevel.Debug, "DeviceState", $"Cleaned {stateValue.Name} has value: {doubleValue}");
                                }
                                catch (Exception ex)
                                {
                                    // Log any exception and don't add the value to the cleaned list
                                    logger.LogMessage(LogLevel.Debug, "DeviceState", $"{stateValue.Name} - Ignoring exception: {ex.Message}");
                                }
                                break;

                            default:
                                throw new InvalidValueException($"DeviceState - Unsupported member type: {element.ValueKind}");
                        }
                    }
                    else // Not a JsonElement so just add the value to the cleaned list
                    {
                        switchReturnValue.Add(stateValue);
                    }
                }

                // Assign the cleaned switch values to the return value
                cleaned = switchReturnValue;
            }

            // Return the cleaned device state values
            return cleaned;
        }

        /// <summary>
        /// Private method to simplify log messages in the class
        /// </summary>
        /// <param name="message"></param>
        private static void LogMessage(string message)
        {
            logger?.LogMessage(LogLevel.Debug, nameof(Clean), message);
        }

    }
}
