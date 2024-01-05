namespace ASCOM.Tools
{

    public partial class SolarSystemBody
    {
        /// <summary>
        /// Sources of comet and asteroid orbit data
        /// </summary>
        public enum OrbitDataSource
        {
            /// <summary>
            /// Minor Planet Centre comet orbit
            /// </summary>
            MpcCometOrbit,
            /// <summary>
            /// Minor Planet Centre asteroid orbit
            /// </summary>
            MpcAsteroidOrbit,
            /// <summary>
            /// Jet Propulsion Lab comet orbit
            /// </summary>
            JplCometOrbit,
            /// <summary>
            /// Jet Propulsion Lab numbered asteroid orbit
            /// </summary>
            JplNumberedAsteroidOrbit,
            /// <summary>
            /// Jet Propulsion Lab un-numbered asteroid orbit
            /// </summary>
            JplUnNumberedAsteroidOrbit,
            /// <summary>
            /// Lowell observatory asteroid orbit
            /// </summary>
            LowellAsteroid
        }
    }
}
