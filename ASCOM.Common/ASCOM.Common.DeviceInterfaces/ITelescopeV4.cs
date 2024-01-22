namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Telescope interface version 4, which incorporates the new members in IAscomDeviceV2 and the members present in ITelescopeV3
    /// </summary>
    public interface ITelescopeV4 : IAscomDeviceV2, ITelescopeV3
    {
    }
}