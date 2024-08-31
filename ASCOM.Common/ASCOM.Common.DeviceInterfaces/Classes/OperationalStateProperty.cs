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

        // Definitive list of valid operational state names and associated data types for each ASCOM device type
        private static readonly List<OperationalStateProperty> members = new List<OperationalStateProperty>()
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

        #region Initialisers

        /// <summary>
        /// Default initialiser for the OperationalStateProperty class
        /// </summary>
        internal OperationalStateProperty()
        {
            DeviceType = DeviceTypes.Telescope;
            Name = "State name not set";
            DataType = "Type name not set";
        }

        /// <summary>
        /// Initialise the device type, operational state name  and data type
        /// </summary>
        /// <param name="deviceType">ASCOM device type</param>
        /// <param name="stateName">operational state name.</param>
        /// <param name="typeName">Date type name e.g. Boolean, Double, DateTime etc. This must be the CLR type name not the C# shortcut name e.g. It must be "Boolean" and not "bool".</param>
        internal OperationalStateProperty(DeviceTypes deviceType, string stateName, string typeName)
        {
            DeviceType = deviceType;
            Name = stateName;
            DataType = typeName;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// ASCOM device type that supplies this operational property.
        /// </summary>
        public DeviceTypes DeviceType { get; private set; }

        /// <summary>
        /// Name of the operational state property.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Data type of the operational state property.
        /// </summary>
        /// <remarks>
        /// This will be the CLR type name not the C# shortcut name e.g. It will be <see cref="Boolean"/> and not <see langword="bool"/>, <see cref="Int32"/> and not <see langword="int"/>.</remarks>
        public string DataType { get; private set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Helper method to ensure that <see cref="StateValue.Value"/> properties are of the expected type after de=serialisation by <see cref="JsonSerializer.Deserialize{TValue}(string, JsonSerializerOptions)"/>
        /// </summary>
        /// <param name="deviceState">List of device state values returned by the device</param>
        /// <param name="deviceType">The device type (Camera,Telescope etc.)</param>
        /// <param name="logger">Optional ILogger instance to receive operational log messages (defaults to null)</param>
        /// <returns>A cleaned list of device state values</returns>
        /// <exception cref="InvalidValueException">If the supplied device type is not supported.</exception>
        /// <remarks>When de-serialising StateValue objects in a DeviceState response, System.Text.JSON returns the <see cref="StateValue.Value"/> property as type <see cref="JsonElement"/> 
        /// rather than the type sent by the device, which may have been <see langword="int"/>, <see langword="double"/>, <see langword="string"/>, <see cref="DateTime"/> etc.
        /// This method parses a list of <see cref="StateValue"/> objects and converts all <see cref="JsonElement"/> types in <see cref="StateValue.Value"/> to the expected data type.
        /// </remarks>
        public static List<StateValue> Clean(List<StateValue> deviceState, DeviceTypes deviceType, ILogger logger = null)
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
            OperationalStateProperty.logger = logger;

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
                OperationalStateProperty member = OperationalStateProperty.members.Where<OperationalStateProperty>(x => x.DeviceType == deviceType & x.Name == stateValue.Name).FirstOrDefault();

                // Process the member if one is found, otherwise just add it to the list
                if (member != null) // A supported state value was found
                {
                    // Handle the different supported types
                    switch (member.DataType)
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
                                cleaned.Add(new StateValue(member.Name, boolValue));
                                LogMessage($"Cleaned {member.DataType} {member.Name} has value: {boolValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.Name} - Ignoring exception: {ex.Message}");
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
                                cleaned.Add(new StateValue(member.Name, stringValue));
                                LogMessage($"Cleaned {member.DataType} {member.Name} has value: {stringValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.Name} - Ignoring exception: {ex.Message}");
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
                                cleaned.Add(new StateValue(member.Name, shortValue));
                                LogMessage($"Cleaned {member.DataType} {member.Name} has value: {shortValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.Name} - Ignoring exception: {ex.Message}");
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
                                cleaned.Add(new StateValue(member.Name, intValue));
                                LogMessage($"Cleaned {member.DataType} {member.Name} has value: {intValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.Name} - Ignoring exception: {ex.Message}");
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
                                cleaned.Add(new StateValue(member.Name, floatValue));
                                LogMessage($"Cleaned {member.DataType} {member.Name} has value: {floatValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.Name} - Ignoring exception: {ex.Message}");
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
                                cleaned.Add(new StateValue(member.Name, doubleValue));
                                LogMessage($"Cleaned {member.DataType} {member.Name} has value: {doubleValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.Name} - Ignoring exception: {ex.Message}");
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
                                cleaned.Add(new StateValue(member.Name, dateTimeValue));
                                LogMessage($"Cleaned {member.Name} has value: {dateTimeValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.Name} - Ignoring exception: {ex.Message}");
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
                                cleaned.Add(new StateValue(member.Name, cameraStateValue));
                                LogMessage($"Cleaned {member.Name} has value: {cameraStateValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.Name} - Ignoring exception: {ex.Message}");
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
                                cleaned.Add(new StateValue(member.Name, calibrationStatusValue));
                                LogMessage($"Cleaned {member.Name} has value: {calibrationStatusValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.Name} - Ignoring exception: {ex.Message}");
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
                                cleaned.Add(new StateValue(member.Name, coverStatusValue));
                                LogMessage($"Cleaned {member.Name} has value: {coverStatusValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.Name} - Ignoring exception: {ex.Message}");
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
                                cleaned.Add(new StateValue(member.Name, shutterStateValue));
                                LogMessage($"Cleaned {member.Name} has value: {shutterStateValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.Name} - Ignoring exception: {ex.Message}");
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
                                cleaned.Add(new StateValue(member.Name, pointingStateValue));
                                LogMessage($"Cleaned {member.Name} has value: {pointingStateValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.Name} - Ignoring exception: {ex.Message}");
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
                                cleaned.Add(new StateValue(member.Name, videoCameraStateValue));
                                LogMessage($"Cleaned {member.Name} has value: {videoCameraStateValue}");
                            }
                            catch (Exception ex)
                            {
                                // Log any exception and don't add the value to the cleaned list
                                LogMessage($"{member.Name} - Ignoring exception: {ex.Message}");
                            }
                            break;

                        default:
                            throw new InvalidValueException($"Unsupported member type: {member.DataType}");
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
                    OperationalStateProperty.logger.LogMessage(LogLevel.Debug, "DeviceState", $"Found Switch state value {stateValue.Name} = {stateValue.Value}");

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
                                    OperationalStateProperty.logger.LogMessage(LogLevel.Debug, "DeviceState", $"Cleaned {stateValue.Name} has value: {boolValue}");

                                }
                                catch (Exception ex)
                                {
                                    // Log any exception and don't add the value to the cleaned list
                                    OperationalStateProperty.logger.LogMessage(LogLevel.Debug, "DeviceState", $"{stateValue.Name} - Ignoring exception: {ex.Message}");
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
                                    OperationalStateProperty.logger.LogMessage(LogLevel.Debug, "DeviceState", $"Cleaned {stateValue.Name} has value: {doubleValue}");
                                }
                                catch (Exception ex)
                                {
                                    // Log any exception and don't add the value to the cleaned list
                                    OperationalStateProperty.logger.LogMessage(LogLevel.Debug, "DeviceState", $"{stateValue.Name} - Ignoring exception: {ex.Message}");
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
        /// Returns the list of operational state property names and return types for all devices as a list of <see cref="OperationalStateProperty"/> values
        /// </summary>
        /// <returns>A generic list of <see cref="OperationalStateProperty"/> values for all devices.</returns>
        public static List<OperationalStateProperty> GetAllOperationalProperties()
        {
            // Return a copy of the member list so that the master list cannot be changed from the outside
            return new List<OperationalStateProperty>(members);
        }

        /// <summary>
        /// Returns a list of operational state property names and return types for the specified device type as a list of <see cref="OperationalStateProperty"/> values
        /// </summary>
        /// <param name="deviceType">The device type whose operational state property names are required.</param>
        /// <returns>A generic list of <see cref="OperationalStateProperty"/> values for the specified device type.</returns>
        public static List<OperationalStateProperty> GetOperationalPropertiesForDeviceType(DeviceTypes deviceType)
        {
            // Return a copy of the member sub-set list so that the master list cannot be changed from the outside
            // NOTE: LINQ.Where filters the original object, it does not create a clone of the object containing only the sub-setted members, hence we need to create the copy to maintain data integrity.
            List < OperationalStateProperty > memberSubset= new List<OperationalStateProperty>((List<OperationalStateProperty>)members.Where<OperationalStateProperty>(x => (x.DeviceType == deviceType)));
            return memberSubset;
        }

        #endregion

        #region Private code

        /// <summary>
        /// Private method to simplify log messages in the class
        /// </summary>
        /// <param name="message"></param>
        private static void LogMessage(string message)
        {
            logger?.LogMessage(LogLevel.Debug, nameof(Clean), message);
        }

        #endregion

    }
}
