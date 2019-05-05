namespace ASCOM.Alpaca.Devices.Telescope
{
    /// <summary>
    /// The pointing state of the mount
    /// </summary>
    /// <remarks>
    /// <para><c>Pier side</c> is a GEM-specific term that has historically caused much confusion. 
    /// As of Platform 6, the PierSide property is defined to refer to the telescope pointing state. Please see the Platform Developer's Help file for 
    /// much more information on this topic.</para>
    /// <para>In order to support Dome slaving, where it is important to know on which side of the pier the mount is actually located, ASCOM has adopted the 
    /// convention that the Normal pointing state will be the state where the mount is on the East side of the pier, looking West with the counterweights below 
    /// the optical assembly.</para>
    /// </remarks>
    public enum PierSide
    {
        /// <summary>
        /// Normal pointing state - Mount on the East side of pier (looking West)
        /// </summary>
        East = 0,

        /// <summary>
        /// Unknown or indeterminate.
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// Through the pole pointing state - Mount on the West side of pier (looking East)
        /// </summary>
        West = 1
    }
}