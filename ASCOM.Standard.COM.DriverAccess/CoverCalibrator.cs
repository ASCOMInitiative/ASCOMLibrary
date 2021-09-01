using ASCOM.Standard.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Standard.COM.DriverAccess
{
    public class CoverCalibrator : ASCOMDevice, ASCOM.Standard.Interfaces.ICoverCalibratorV1
    {
        public static List<ASCOMRegistration> CoverCalibrators => ProfileAccess.GetDrivers(DriverTypes.CoverCalibrator);

        public CoverCalibrator(string ProgID) : base(ProgID)
        {

        }

        public CoverStatus CoverState => (CoverStatus) base.Device.CoverState;

        public CalibratorStatus CalibratorState => (CalibratorStatus) base.Device.CalibratorState;

        public int Brightness => base.Device.Brightness;

        public int MaxBrightness => base.Device.MaxBrightness;

        public void OpenCover()
        {
            base.Device.OpenCover();
        }

        public void CloseCover()
        {
            base.Device.CloseCover();
        }

        public void HaltCover()
        {
            base.Device.HaltCover();
        }

        public void CalibratorOn(int Brightness)
        {
            base.Device.CalibratorOn(Brightness);
        }

        public void CalibratorOff()
        {
            base.Device.CalibratorOff();
        }
    }
}
