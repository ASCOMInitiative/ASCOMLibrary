using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Standard.COM.DriverAccess
{
    public class ObservingConditions : ASCOMDevice, ASCOM.Standard.Interfaces.IObservingConditions
    {
        public static List<ASCOMRegistration> ObservingConditionDrivers => ProfileAccess.GetDrivers(DriverTypes.ObservingConditions);

        public ObservingConditions(string ProgID) : base(ProgID)
        {

        }

        public double AveragePeriod { get => base.Device.AveragePeriod; set => base.Device.AveragePeriod = value; }

        public double CloudCover => base.Device.CloudCover;

        public double DewPoint => base.Device.DewPoint;

        public double Humidity => base.Device.Humidity;

        public double Pressure => base.Device.Pressure;

        public double RainRate => base.Device.RainRate;

        public double SkyBrightness => base.Device.SkyBrightness;

        public double SkyQuality => base.Device.SkyQuality;

        public double StarFWHM => base.Device.StarFWHM;

        public double SkyTemperature => base.Device.SkyTemperature;

        public double Temperature => base.Device.Temperature;

        public double WindDirection => base.Device.WindDirection;

        public double WindGust => base.Device.WindGust;

        public double WindSpeed => base.Device.WindSpeed;

        public double TimeSinceLastUpdate(string PropertyName)
        {
            return base.Device.TimeSinceLastUpdate(PropertyName);
        }

        public string SensorDescription(string PropertyName)
        {
            return base.Device.SensorDescription(PropertyName);
        }

        public void Refresh()
        {
            base.Device.Refresh();
        }
    }
}
