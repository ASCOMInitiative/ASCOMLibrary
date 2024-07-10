namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Safety monitor interface version 3, which incorporates the new members in IAscomDeviceV2 and the members present in ISafetyMonitorV1
    /// </summary>
    public interface ISafetyMonitorV3 : IAscomDeviceV2, ISafetyMonitor
    {
    }
}