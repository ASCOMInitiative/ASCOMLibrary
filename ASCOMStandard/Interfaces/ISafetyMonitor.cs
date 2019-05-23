// -----------------------------------------------------------------------
// <summary>Defines the ISafetyMonitor Interface</summary>
// -----------------------------------------------------------------------
namespace ASCOM.Alpaca.Interfaces
{
    /// <summary>
    /// Defines the ISafetyMonitor Interface
    /// </summary>
    public interface ISafetyMonitor : IAscomDevice
    {

        /// <summary>
        /// Indicates whether the monitored state is safe for use.
        /// </summary>
        /// <value>True if the state is safe, False if it is unsafe.</value>
        /// <remarks>
        /// <p style="color:red;margin-bottom:0"><b>Must be implemented and must not throw a NotImplementedException. </b></p>
        /// </remarks>
        bool IsSafe { get; }
    }
}