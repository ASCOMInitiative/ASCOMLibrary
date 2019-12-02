namespace ASCOM.Alpaca.Interfaces
{
    /// <summary>
    /// ASCOM Camera status values.
    /// </summary>
    public enum CameraState
    {
        /// <summary>
        /// Camera status idle
        /// </summary>
        Idle = 0, 
        /// <summary>
        /// Camera status waiting
        /// </summary>
        Waiting = 1, 
        /// <summary>
        /// Camera status exposing
        /// </summary>
        Exposing = 2,
        /// <summary>
        /// Camera status reading
        /// </summary>
        Reading = 3, 
        /// <summary>
        /// Camera status download
        /// </summary>
        Download = 4, 
        /// <summary>
        /// Camera status error
        /// </summary>
        Error = 5
    }
}