using ASCOM.Common.DeviceStateClasses;
using ASCOM.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Class to hold a state name:value pair.
    /// </summary>
    public class StateValue : IStateValue
    {
        // field to hold any supplied operational message logger
        private static ILogger logger;

        /// <summary>
        /// Create a new state value object
        /// </summary>
        public StateValue() { }

        /// <summary>
        /// Create a new state value object with the given name and value
        /// </summary>
        /// <param name="name">State name</param>
        /// <param name="value">State value</param>
        public StateValue(string name, object value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Create a StateValue object whose Name property is "TimeStamp" and whose Value property is the supplied date-time value.
        /// </summary>
        /// <param name="dateTime">This time-stamp date-time value</param>
        public StateValue(DateTime dateTime)
        {
            Name = "TimeStamp";
            Value = dateTime;
        }

        /// <summary>
        /// State name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// State value
        /// </summary>
        public object Value { get; set; }

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
