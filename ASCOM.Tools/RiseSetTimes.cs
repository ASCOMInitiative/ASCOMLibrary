using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Tools
{
    /// <summary>
    /// Rise and set information for an astronomical event.
    /// </summary>
    public class RiseSetTimes
    {
        /// <summary>
        /// Create instance and initialise the RiseEvents and SetEvents lists
        /// </summary>
        public RiseSetTimes()
        {
            RiseEvents = new List<double>();
            SetEvents = new List<double>();
        }

        /// <summary>
        /// True if the body is above the horizon at midnight
        /// </summary>
        public bool AboveHorizonAtMidnight { get; set; }

        /// <summary>
        /// List of rise event times.
        /// </summary>
        public List<double> RiseEvents { get; set; }

        /// <summary>
        /// List of set event times
        /// </summary>
        public List<double> SetEvents { get; set; }
    }
}
