namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Observing conditions interface version 2, which incorporates the new members in IAscomDeviceV2 and the members present in IObservingConditionsV1
    /// </summary>
    public interface IObservingConditionsV2 : IAscomDeviceV2, IObservingConditions
    {
    }
}