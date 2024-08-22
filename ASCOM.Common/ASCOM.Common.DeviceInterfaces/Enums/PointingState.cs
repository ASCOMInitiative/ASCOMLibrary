namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// The pointing state of the mount
    /// </summary>
    /// <remarks>
    /// <para>Please see <see cref="ITelescopeV3.SideOfPier"/> for more information on pointing state and physical side of pier for German equatorial mounts.</para>
    /// </remarks>
    public enum PointingState
    {
        /// <summary>
        /// Normal pointing state
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Unknown or indeterminate.
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// Through the pole pointing state
        /// </summary>
        ThroughThePole = 1
    }
}