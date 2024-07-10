using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Tools.Kepler
{
    internal class Orbit
    {
        internal string objectName; // /* name of the object */
        internal double epoch; // /* epoch of orbital elements */
        internal double i; // /* inclination	*/
        internal double W; // /* longitude of the ascending node */
        internal double wp; // /* argument of the perihelion */
        internal double a; // /* mean distance (semimajor axis) */
        internal double dm;    // /* daily motion */
        internal double ecc;   // /* eccentricity */
        internal double M; // /* mean anomaly */
        internal double equinox;   // /* epoch of equinox and ecliptic */
        internal double mag;   // /* visual magnitude at 1AU from earth and sun */
        internal double sdiam; // /* equatorial semi-diameter at 1au, arc seconds */

        // /* The following used by perturbation formulas: */
        internal PlanetTable ptable;
        internal double L;  // /* computed mean longitude */
        internal double r;  // /* computed radius vector */
        internal double plat;   // /* perturbation in ecliptic latitude */

        internal double semiMajorAxis; // Placeholder for the semi-major axis to disambiguate it from the perihelion distance
        internal double perihelionDistance; // Placeholder for the perihelion distance to disambiguate it from the semi-major axis
        internal bool eccentricityHasBeenSet;

        /// <summary>
        /// Initialiser to set the semi-major axis and perihelion distance values to default "unset" states
        /// </summary>
        public Orbit()
        {
            objectName = "";
            epoch = 0.0;
            i = 0.0;
            W = 0.0;
            wp = 0.0;
            a = 0.0;
            dm = 0.0;
            ecc = 0.0;
            M = 0.0;
            equinox = 0.0;
            mag = 0.0;
            sdiam = 0.0;
            ptable = new PlanetTable();
            L = 0.0;
            r = 0.0;
            plat = 0.0;
            eccentricityHasBeenSet = false;

            // Initialize the semi-major axis to default 'unset' states
            semiMajorAxis = KeplerSupport.NOT_SET;
            perihelionDistance = KeplerSupport.NOT_SET;
        }

        internal Orbit(string obn, double ep, double i_p, double W_p, double wp_p, double a_p, double dm_p, double ecc_p, double M_p, double eq, double mg, double sd, PlanetTable pt, double L_p, double r_p, double pl)
        {
            objectName = obn;
            epoch = ep;
            i = i_p;
            W = W_p;
            wp = wp_p;
            a = a_p;
            dm = dm_p;
            ecc = ecc_p;
            M = M_p;
            equinox = eq;
            mag = mg;
            sdiam = sd;
            ptable = pt;
            L = L_p;
            r = r_p;
            plat = pl;
            eccentricityHasBeenSet = true;

            // Initialize the semi-major axis to default 'unset' states
            semiMajorAxis = KeplerSupport.NOT_SET;
            perihelionDistance = KeplerSupport.NOT_SET;
        }
    }
}

