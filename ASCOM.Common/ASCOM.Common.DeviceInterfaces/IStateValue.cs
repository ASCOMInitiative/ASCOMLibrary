namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Interface definition for DeviceState objects
    /// </summary>
    public interface IStateValue
    {
        /// <summary>
        /// Property name with casing that must match the casing in the relevant interface definition
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Property value
        /// </summary>
        object Value { get; }
    }
}
