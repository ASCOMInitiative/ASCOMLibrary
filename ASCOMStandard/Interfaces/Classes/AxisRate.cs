namespace ASCOM.Alpaca.Interfaces
{
    /// <summary>
    /// Describe a rate at which the telescope may be moved about the specified axis by the MoveAxis(TelescopeAxes, Double) method.
    /// </summary>
    public class AxisRate
    {
        /// <summary>
        /// Create a new AxisRate object with minimum and maximum rates set to 0.0
        /// </summary>
        public AxisRate()
        { }

        /// <summary>
        /// Create a new AxisRate object with the specified minimum and maximum rates
        /// </summary>
        /// <param name="minimum">The lowest axis movement rate (must be >= 0.0)</param>
        /// <param name="maximum">The highest axis movement rate (must be >= the minimum rate)</param>
        public AxisRate(double minimum, double maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        /// <summary>
        /// The minimum rate (degrees per second) This must always be a positive number. It indicates the maximum rate in either direction about the axis.
        /// </summary>
        public double Minimum { get; set; }
        
        /// <summary>
        /// The maximum rate (degrees per second) This must always be a positive number. It indicates the maximum rate in either direction about the axis.
        /// </summary>
        public double Maximum { get; set; }

        /// <summary>
        /// Return the minimum and maximum values as a string
        /// </summary>
        /// <returns>String representation of AxisRate minimum and maximum values.</returns>
        public override string ToString()
        {
            return $"AxisRate range: {Minimum} to {Maximum}";
        }
    }
}