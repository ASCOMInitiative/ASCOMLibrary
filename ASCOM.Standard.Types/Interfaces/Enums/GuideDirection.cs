namespace ASCOM.Standard.Interfaces
{
    /// <summary>
    /// The direction in which the guide-rate motion is to be made.
    /// </summary>
    public enum GuideDirection
    {
        /// <summary>
        /// North (+ declination/altitude).
        /// </summary>
        North = 0, 

        /// <summary>
        /// South (- declination/altitude).
        /// </summary>
        South = 1, 

        /// <summary>
        /// East (+ right ascension/azimuth).
        /// </summary>
        East = 2, 

        /// <summary>
        /// West (- right ascension/azimuth)
        /// </summary>
        West = 3
    }
}