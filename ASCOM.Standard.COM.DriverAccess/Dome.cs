using ASCOM.Standard.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Standard.COM.DriverAccess
{
    public class Dome : ASCOMDevice, ASCOM.Standard.Interfaces.IDomeV2
    {
        public static List<ASCOMRegistration> Domes => ProfileAccess.GetDrivers(DriverTypes.Dome);

        public Dome(string ProgID) : base(ProgID)
        {

        }

        public new string DriverVersion
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    return string.Empty;
                }
                return base.DriverVersion;
            }
        }

        public double Altitude => base.Device.Altitude;

        public bool AtHome => base.Device.AtHome;

        public bool AtPark => base.Device.AtPark;

        public double Azimuth => base.Device.Azimuth;

        public bool CanFindHome => base.Device.CanFindHome;

        public bool CanPark => base.Device.CanPark;

        public bool CanSetAltitude => base.Device.CanSetAltitude;

        public bool CanSetAzimuth => base.Device.CanSetAzimuth;

        public bool CanSetPark => base.Device.CanSetPark;

        public bool CanSetShutter => base.Device.CanSetShutter;

        public bool CanSlave => base.Device.CanSlave;

        public bool CanSyncAzimuth => base.Device.CanSyncAzimuth;

        public ShutterState ShutterStatus => (ShutterState) base.Device.ShutterStatus;

        public bool Slaved { get => base.Device.Slaved; set => base.Device.Slaved = value; }

        public bool Slewing => base.Device.Slewing;

        public void AbortSlew()
        {
            base.Device.AbortSlew();
        }

        public void CloseShutter()
        {
            base.Device.CloseShutter();
        }

        public void FindHome()
        {
            base.Device.FindHome();
        }

        public void OpenShutter()
        {
            base.Device.OpenShutter();
        }

        public void Park()
        {
            base.Device.Park();
        }

        public void SetPark()
        {
            base.Device.SetPark();
        }

        public void SlewToAltitude(double Altitude)
        {
            base.Device.SlewToAltitude(Altitude);
        }

        public void SlewToAzimuth(double Azimuth)
        {
            base.Device.SlewToAzimuth(Azimuth);
        }

        public void SyncToAzimuth(double Azimuth)
        {
            base.Device.SyncToAzimuth(Azimuth);
        }
    }
}
