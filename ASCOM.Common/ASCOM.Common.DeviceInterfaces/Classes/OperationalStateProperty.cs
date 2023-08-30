using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// A reference class that describes valid operational state properties
    /// </summary>
    public class OperationalStateProperty
    {
        // Name of the time stamp operational property
        private const string TIME_STAMP = "TimeStamp";

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
            new OperationalStateProperty(DeviceTypes.CoverCalibrator, nameof(ICoverCalibratorV2.CalibratorReady), nameof(Boolean)),
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
    }
}
