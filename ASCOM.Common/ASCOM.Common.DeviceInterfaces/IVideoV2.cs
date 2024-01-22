namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Video interface version 2, which incorporates the new members in IAscomDeviceV2 and the members present in IVideov1
    /// </summary>
    public interface IVideoV2 : IAscomDeviceV2, IVideo
    {
    }
}