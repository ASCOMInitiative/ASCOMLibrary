namespace ASCOM.Alpaca.Interfaces
{
    /// <summary>
    /// Describe a rate at which the telescope may be moved about the specified axis by the MoveAxis(TelescopeAxes, Double) method.
    /// </summary>
    public class AxisRate
    {
        /// <summary>
        /// The minimum rate (degrees per second) This must always be a positive number. It indicates the maximum rate in either direction about the axis.
        /// </summary>
        public double Minimum { get; set; }
        
        /// <summary>
        /// The maximum rate (degrees per second) This must always be a positive number. It indicates the maximum rate in either direction about the axis.
        /// </summary>
        public double Maximum { get; set; }
    }
}