using ASCOM.Common.DeviceInterfaces;
using System.Collections.Generic;
using ASCOM.Common;
using ASCOM.Common.Interfaces;
using ASCOM.Common.DeviceStateClasses;

namespace ASCOM.Com.DriverAccess
{
    /// <summary>
    /// ObservingConditions device class
    /// </summary>
    public class ObservingConditions : ASCOMDevice, IObservingConditionsV2
    {

        #region Convenience members

        /// <summary>
        /// Return a list of all ObservingConditions devices registered in the ASCOM Profile
        /// </summary>
        public static List<ASCOMRegistration> ObservingConditionDrivers => Profile.GetDrivers(DeviceTypes.ObservingConditions);

        /// <summary>
        /// ObservingConditions device state
        /// </summary>
        public ObservingConditionsState ObservingConditionsState
        {
            get
            {
                // Create a state object to return.
                ObservingConditionsState observingConditionsState = new ObservingConditionsState(DeviceState, TL);
                TL.LogMessage(LogLevel.Debug,nameof(ObservingConditionsState), $"Returning: " +
                    $"Cloud cover: '{observingConditionsState.CloudCover}', " +
                    $"Dew point: '{observingConditionsState.DewPoint}', " +
                    $"Humidity: '{observingConditionsState.Humidity}', " +
                    $"Pressure: '{observingConditionsState.Pressure}', " +
                    $"Rain rate: '{observingConditionsState.RainRate}', " +
                    $"Sky brightness: '{observingConditionsState.SkyBrightness}', " +
                    $"Sky quality: '{observingConditionsState.SkyQuality}', " +
                    $"Sky temperature'{observingConditionsState.SkyTemperature}', " +
                    $"Star FWHM: '{observingConditionsState.StarFWHM}', " +
                    $"Temperature: '{observingConditionsState.Temperature}', " +
                    $"Wind direction: '{observingConditionsState.WindDirection}', " +
                    $"Wind gust: '{observingConditionsState.WindGust}', " +
                    $"Wind speed: '{observingConditionsState.WindSpeed}', " +
                    $"Time stamp: '{observingConditionsState.TimeStamp}'");

                // Return the device specific state class
                return observingConditionsState;
            }
        }

        #endregion

        #region Initialisers
        /// <summary>
        /// Initialise ObservingConditions device
        /// </summary>
        /// <param name="ProgID">COM ProgID of the device.</param>
        public ObservingConditions(string ProgID) : base(ProgID)
        {
            deviceType = DeviceTypes.ObservingConditions;
        }

        /// <summary>
        /// Initialise ObservingConditions device with a debug logger
        /// </summary>
        /// <param name="ProgID">ProgID of the driver</param>
        /// <param name="logger">Logger instance to receive debug information.</param>
        public ObservingConditions(string ProgID, ILogger logger) : base(ProgID)
        {
            deviceType = DeviceTypes.ObservingConditions;
            TL = logger;
        }
        #endregion

        #region IObservingConditionsV1 and IObservingConditionsV2

        /// <inheritdoc/>
        public double AveragePeriod { get => Device.AveragePeriod; set => Device.AveragePeriod = value; }

        /// <inheritdoc/>
        public double CloudCover => Device.CloudCover;

        /// <inheritdoc/>
        public double DewPoint => Device.DewPoint;

        /// <inheritdoc/>
        public double Humidity => Device.Humidity;

        /// <inheritdoc/>
        public double Pressure => Device.Pressure;

        /// <inheritdoc/>
        public double RainRate => Device.RainRate;

        /// <inheritdoc/>
        public double SkyBrightness => Device.SkyBrightness;

        /// <inheritdoc/>
        public double SkyQuality => Device.SkyQuality;

        /// <inheritdoc/>
        public double StarFWHM => Device.StarFWHM;

        /// <inheritdoc/>
        public double SkyTemperature => Device.SkyTemperature;

        /// <inheritdoc/>
        public double Temperature => Device.Temperature;

        /// <inheritdoc/>
        public double WindDirection => Device.WindDirection;

        /// <inheritdoc/>
        public double WindGust => Device.WindGust;

        /// <inheritdoc/>
        public double WindSpeed => Device.WindSpeed;

        /// <inheritdoc/>
        public double TimeSinceLastUpdate(string PropertyName)
        {
            return Device.TimeSinceLastUpdate(PropertyName);
        }

        /// <inheritdoc/>
        public string SensorDescription(string PropertyName)
        {
            return Device.SensorDescription(PropertyName);
        }

        /// <inheritdoc/>
        public void Refresh()
        {
            Device.Refresh();
        }

        #endregion

    }
}
