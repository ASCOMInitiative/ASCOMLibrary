namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// The pointing state of the mount (Please note that this enum is called PierSide in traditional Platform code)
    /// </summary>
    /// <remarks>
    /// <para>
    /// For historical reasons, the original Platform enum name PierSide does not reflect its true meaning.
    /// The opportunity of a fresh start with the ASCOM Library was taken to rename the PierSide enum to PointingState. The three PointingState values equate to:
    /// PointingState.Normal = PierSide.pierEast, PointingState.ThroughThePole = PierSide.pierWest and PointingState.Unknown = PierSide.pierUnknown.
    /// </para>
    /// <para>Please see <see cref="ITelescopeV3.SideOfPier"/> for more information on pointing state and physical side of pier for German equatorial mounts.</para>
    /// <para>The original PierSide enum was named before the property's true significance was understood and the opportunity of a fresh start with the 
    /// ASCOM Library was taken to give this enum a more appropriate name.</para>
    /// </remarks>
    public enum PointingState
    {
        /// <summary>
        /// Normal pointing state
        /// </summary>
        /// <remarks>Equates to the Platform's Pierside.pierEast value.</remarks>
        Normal = 0,

        /// <summary>
        /// Unknown or indeterminate.
        /// </summary>
        /// <remarks>Equates to the Platform's Pierside.pierUnknown value.</remarks>
        Unknown = -1,

        /// <summary>
        /// Through the pole pointing state
        /// </summary>
        /// <remarks>Equates to the Platform's Pierside.pierWest value.</remarks>
        ThroughThePole = 1
    }
}