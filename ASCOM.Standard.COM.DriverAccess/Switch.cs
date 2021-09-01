using System;
using System.Collections.Generic;

namespace ASCOM.Standard.COM.DriverAccess
{
    public class Switch : ASCOMDevice, ASCOM.Standard.Interfaces.ISwitchV2
    {
        public static List<ASCOMRegistration> Switches => ProfileAccess.GetDrivers(DriverTypes.Switch);

        public Switch(string ProgID) : base(ProgID)
        {
        }

        public short MaxSwitch => base.Device.MaxSwitch;

        public string GetSwitchName(short id)
        {
            return base.Device.GetSwitchName(id);
        }

        public void SetSwitchName(short id, string name)
        {
            base.Device.SetSwitchName(id, name);
        }

        public string GetSwitchDescription(short id)
        {
            return base.Device.GetSwitchDescription(id);
        }

        public bool CanWrite(short id)
        {
            return base.Device.CanWrite(id);
        }

        public bool GetSwitch(short id)
        {
            return base.Device.GetSwitch(id);
        }

        public void SetSwitch(short id, bool state)
        {
            base.Device.SetSwitch(id, state);
        }

        public double MaxSwitchValue(short id)
        {
            try
            {
                return base.Device.MaxSwitchValue(id);
            }
            catch (System.NotImplementedException)
            { 
                return 1.0; 
            }
        }

        public double MinSwitchValue(short id)
        {
            try
            {
                return base.Device.MinSwitchValue(id);
            }
            catch (System.NotImplementedException)
            {
                return 0.0;
            }
        }

        public double SwitchStep(short id)
        {
            try
            {
                return base.Device.SwitchStep(id);
            }
            catch (System.NotImplementedException)
            {
                return 1.0;
            }
        }

        public double GetSwitchValue(short id)
        {
            try
            {
                return base.Device.GetSwitchValue(id);
            }
            catch (System.NotImplementedException)
            {
                return this.GetSwitch(id) ? 1.0 : 0.0;
            }
        }

        public void SetSwitchValue(short id, double value)
        {
            try
            {
                base.Device.SetSwitchValue(id, value);
            }
            catch (System.NotImplementedException)
            {
                bool bv = value >= 0.5 ? true : false;
                this.SetSwitch(id, bv);
            }
        }
    }
}