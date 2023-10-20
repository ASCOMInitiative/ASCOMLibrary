using System.Runtime.InteropServices;

namespace ASCOM.Tools.Interfaces
{
    /// <summary>
    /// Interface to the NOVAS-COM PositionVector Class
    /// </summary>
    /// <remarks>Objects of class PositionVector contain vectors used for positions (earth, sites, 
    /// stars and planets) throughout NOVAS-COM. Of course, its properties include the x, y, and z 
    /// components of the position. Additional properties are right ascension and declination, distance, 
    /// and light time (applicable to star positions), and Alt/Az (available only in PositionVectors 
    /// returned by Star or Planet methods GetTopocentricPosition()). You can initialize a PositionVector 
    /// from a Star object (essentially an FK5 or HIP catalog entry) or a Site (lat/long/height). 
    /// PositionVector has methods that can adjust the coordinates for precession, aberration and 
    /// proper motion. Thus, a PositionVector object gives access to some of the lower-level NOVAS functions. 
    /// <para><b>Note:</b> The equatorial coordinate properties of this object are dependent variables, and thus are read-only. 
    /// Changing any Cartesian coordinate will cause the equatorial coordinates to be recalculated. 
    /// </para></remarks>
    public interface IPositionVector
    {
        /// <summary>
        /// Adjust the position vector of an object for aberration of light
        /// </summary>
        /// <param name="vel">The velocity vector of the observer</param>
        /// <remarks>The algorithm includes relativistic terms</remarks>
        void Aberration([MarshalAs(UnmanagedType.IDispatch)] VelocityVector vel);

        /// <summary>
        /// Adjust the position vector for precession of equinoxes between two given epochs
        /// </summary>
        /// <param name="tjd">The first epoch (Terrestrial Julian Date)</param>
        /// <param name="tjd2">The second epoch (Terrestrial Julian Date)</param>
        /// <remarks>The coordinates are referred to the mean equator and equinox of the two respective epochs.</remarks>
        void Precess(double tjd, double tjd2);

        /// <summary>
        /// Adjust the position vector for proper motion (including foreshortening effects)
        /// </summary>
        /// <param name="vel">The velocity vector of the object</param>
        /// <param name="tjd1">The first epoch (Terrestrial Julian Date)</param>
        /// <param name="tjd2">The second epoch (Terrestrial Julian Date)</param>
        /// <returns>True if successful or throws an exception.</returns>
        /// <remarks></remarks>
        /// <exception cref="ValueNotSetException">If the position vector x, y or z values has not been set</exception>
        /// <exception cref="HelperException">If the supplied velocity vector does not have valid x, y and z components</exception>
        bool ProperMotion([MarshalAs(UnmanagedType.IDispatch)] VelocityVector vel, double tjd1, double tjd2);

        /// <summary>
        /// Initialize the PositionVector from a Site object and Greenwich apparent sidereal time.
        /// </summary>
        /// <param name="site">The Site object from which to initialize</param>
        /// <param name="gast">Greenwich Apparent Sidereal Time</param>
        /// <returns>True if successful or throws an exception</returns>
        /// <remarks>The GAST parameter must be for Greenwich, not local. The time is rotated through the 
        /// site longitude. See SetFromSiteJD() for an equivalent method that takes UTC Julian Date and 
        /// Delta-T (eliminating the need for calculating hyper-accurate GAST yourself).</remarks>
        bool SetFromSite([MarshalAs(UnmanagedType.IDispatch)] Site site, double gast);

        /// <summary>
        /// Initialize the PositionVector from a Site object using UTC Julian date and Delta-T
        /// </summary>
        /// <param name="site">The Site object from which to initialize</param>
        /// <param name="ujd">UTC Julian Date</param>
        /// <param name="delta_t">The value of Delta-T (TT - UT1) to use for reductions (seconds)</param>
        /// <returns>True if successful or throws an exception</returns>
        /// <remarks>The Julian date must be UTC Julian date, not terrestrial.
        /// </remarks>
        bool SetFromSiteJD([MarshalAs(UnmanagedType.IDispatch)] Site site, double ujd, double delta_t);

        /// <summary>
        /// Initialize the PositionVector from a Site object using UTC Julian date
        /// </summary>
        /// <param name="site">The Site object from which to initialize</param>
        /// <param name="ujd">UTC Julian Date</param>
        /// <returns>True if successful or throws an exception</returns>
        /// <remarks>The Julian date must be UTC Julian date, not terrestrial. Calculations will use the internal delta-T tables and estimator to get 
        /// delta-T. 
        /// This overload is not available through COM, please use 
        /// "SetFromSiteJD(ByVal site As Site, ByVal ujd As Double, ByVal delta_t As Double)"
        /// with delta_t set to 0.0 to achieve this effect.
        /// </remarks>
        bool SetFromSiteJD([MarshalAs(UnmanagedType.IDispatch)] Site site, double ujd);

        /// <summary>
        /// Initialize the PositionVector from a Star object.
        /// </summary>
        /// <param name="star">The Star object from which to initialize</param>
        /// <returns>True if successful or throws an exception</returns>
        /// <remarks></remarks>
        /// <exception cref="HelperException">If Parallax, RightAScension or Declination is not available in the supplied star object.</exception>
        bool SetFromStar([MarshalAs(UnmanagedType.IDispatch)] Star star);

        /// <summary>
        /// The azimuth coordinate (degrees, + east)
        /// </summary>
        /// <value>The azimuth coordinate</value>
        /// <returns>Degrees, + East</returns>
        /// <remarks></remarks>
        double Azimuth { get; }

        /// <summary>
        /// Declination coordinate
        /// </summary>
        /// <value>Declination coordinate</value>
        /// <returns>Degrees</returns>
        /// <remarks></remarks>
        double Declination { get; }

        /// <summary>
        /// Distance/Radius coordinate
        /// </summary>
        /// <value>Distance/Radius coordinate</value>
        /// <returns>AU</returns>
        /// <remarks></remarks>
        double Distance { get; }

        /// <summary>
        /// The elevation (altitude) coordinate (degrees, + up)
        /// </summary>
        /// <value>The elevation (altitude) coordinate (degrees, + up)</value>
        /// <returns>(Degrees, + up</returns>
        /// <remarks>Elevation is available only in PositionVectors returned from calls to 
        /// Star.GetTopocentricPosition() and/or Planet.GetTopocentricPosition(). </remarks>
        /// <exception cref="HelperException">When the position vector has not been 
        /// initialised from Star.GetTopoCentricPosition and Planet.GetTopocentricPosition</exception>
        double Elevation { get; }

        /// <summary>
        /// Light time from body to origin, days.
        /// </summary>
        /// <value>Light time from body to origin</value>
        /// <returns>Days</returns>
        /// <remarks></remarks>
        double LightTime { get; }

        /// <summary>
        /// RightAscension coordinate, hours
        /// </summary>
        /// <value>RightAscension coordinate</value>
        /// <returns>Hours</returns>
        /// <remarks></remarks>
        double RightAscension { get; }

        /// <summary>
        /// Position Cartesian x component
        /// </summary>
        /// <value>Cartesian x component</value>
        /// <returns>Cartesian x component</returns>
        /// <remarks></remarks>
        double x { get; set; }

        /// <summary>
        /// Position Cartesian y component
        /// </summary>
        /// <value>Cartesian y component</value>
        /// <returns>Cartesian y component</returns>
        /// <remarks></remarks>
        double y { get; set; }

        /// <summary>
        /// Position Cartesian z component
        /// </summary>
        /// <value>Cartesian z component</value>
        /// <returns>Cartesian z component</returns>
        /// <remarks></remarks>
        double z { get; set; }
    }
}