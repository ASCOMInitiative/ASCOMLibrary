namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Focuser interface version 4, which incorporates the new members in IAscomDeviceV2 and the members present in IFocuserV3
    /// </summary>
    public interface IFocuserV4 : IAscomDeviceV2, IFocuserV3
    {
    }
}