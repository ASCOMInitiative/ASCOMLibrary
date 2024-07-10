namespace ASCOM.Tools.Kepler
{
    internal class PlanetTable
    {
        internal int maxargs;
        internal int[] max_harmonic;
        internal int max_power_of_t;
        internal int[] arg_tbl;
        internal double[] lon_tbl;
        internal double[] lat_tbl;
        internal double[] rad_tbl;
        internal double distance;
        internal double timescale;
        internal double trunclvl;

        internal PlanetTable() { }

        internal PlanetTable(int ma, int[] mh, int mpt, int[] at, double[] lot, double[] lat, double[] rat, double dis, double ts, double tl)
        {
            maxargs = ma;
            max_harmonic = mh;
            max_power_of_t = mpt;
            arg_tbl = at;
            lon_tbl = lot;
            lat_tbl = lat;
            rad_tbl = rat;
            distance = dis;
            timescale = ts;
            trunclvl = tl;
        }

    }
}
