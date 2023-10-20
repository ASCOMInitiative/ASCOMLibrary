using System.Runtime.InteropServices;

namespace ASCOM.Tools.Interfaces
{
    /// <summary>
    /// interface to the NOVAS_COM VelocityVector Class
    /// </summary>
    /// <remarks>Objects of class VelocityVector contain vectors used for velocities (earth, sites, 
    /// planets, and stars) throughout NOVAS-COM. Of course, its properties include the x, y, and z 
    /// components of the velocity. Additional properties are the velocity in equatorial coordinates of 
    /// right ascension dot, declination dot and radial velocity. You can initialize a PositionVector from 
    /// a Star object (essentially an FK5 or HIP catalog entry) or a Site (lat/long/height). For the star 
    /// object the proper motions, distance and radial velocity are used, for a site, the velocity is that 
    /// of the observer with respect to the Earth's center of mass. </remarks>
    public interface IVelocityVector
    {
        /// <summary>
        /// Initialize the VelocityVector from a Site object and Greenwich Apparent Sidereal Time.
        /// </summary>
        /// <param name="site">The Site object from which to initialize</param>
        /// <param name="gast">Greenwich Apparent Sidereal Time</param>
        /// <returns>True if OK or throws an exception</returns>
        /// <remarks>The velocity vector is that of the observer with respect to the Earth's center 
        /// of mass. The GAST parameter must be for Greenwich, not local. The time is rotated through 
        /// the site longitude. See SetFromSiteJD() for an equivalent method that takes UTC Julian 
        /// Date and optionally Delta-T (eliminating the need for calculating hyper-accurate GAST yourself). </remarks>
        bool SetFromSite([MarshalAs(UnmanagedType.IDispatch)] Site site, double gast);

        /// <summary>
        /// Initialize the VelocityVector from a Site object using UTC Julian Date and Delta-T
        /// </summary>
        /// <param name="site">The Site object from which to initialize</param>
        /// <param name="ujd">UTC Julian Date</param>
        /// <param name="delta_t">The optional value of Delta-T (TT - UT1) to use for reductions (seconds)</param>
        /// <returns>True if OK otherwise throws an exception</returns>
        /// <remarks>The velocity vector is that of the observer with respect to the Earth's center 
        /// of mass. The Julian date must be UTC Julian date, not terrestrial.</remarks>
        bool SetFromSiteJD([MarshalAs(UnmanagedType.IDispatch)] Site site, double ujd, double delta_t);

        /// <summary>
        /// Initialize the VelocityVector from a Star object.
        /// </summary>
        /// <param name="star">The Star object from which to initialize</param>
        /// <returns>True if OK otherwise throws an exception</returns>
        /// <remarks>The proper motions, distance and radial velocity are used in the velocity calculation. </remarks>
        /// <exception cref="HelperException">If any of: Parallax, RightAscension, Declination, 
        /// ProperMotionRA, ProperMotionDec or RadialVelocity are not available in the star object</exception>
        bool SetFromStar([MarshalAs(UnmanagedType.IDispatch)] Star star);

        /// <summary>
        /// Linear velocity along the declination direction (AU/day)
        /// </summary>
        /// <value>Linear velocity along the declination direction</value>
        /// <returns>AU/day</returns>
        /// <remarks>This is not the proper motion (which is an angular rate and is dependent on the distance to the object).</remarks>
        double DecVelocity { get; }

        /// <summary>
        /// Linear velocity along the radial direction (AU/day)
        /// </summary>
        /// <value>Linear velocity along the radial direction</value>
        /// <returns>AU/day</returns>
        /// <remarks></remarks>
        double RadialVelocity { get; }

        /// <summary>
        /// Linear velocity along the right ascension direction (AU/day)
        /// </summary>
        /// <value>Linear velocity along the right ascension direction</value>
        /// <returns>AU/day</returns>
        /// <remarks></remarks>
        double RAVelocity { get; }

        /// <summary>
        /// Cartesian x component of velocity (AU/day)
        /// </summary>
        /// <value>Cartesian x component of velocity</value>
        /// <returns>AU/day</returns>
        /// <remarks></remarks>
        double x { get; set; }

        /// <summary>
        /// Cartesian y component of velocity (AU/day)
        /// </summary>
        /// <value>Cartesian y component of velocity</value>
        /// <returns>AU/day</returns>
        /// <remarks></remarks>
        double y { get; set; }

        /// <summary>
        /// Cartesian z component of velocity (AU/day)
        /// </summary>
        /// <value>Cartesian z component of velocity</value>
        /// <returns>AU/day</returns>
        /// <remarks></remarks>
        double z { get; set; }

        /// <summary>
        /// Initialize the VelocityVector from a Site object using UTC Julian Date
        /// </summary>
        /// <param name="site">The Site object from which to initialize</param>
        /// <param name="ujd">UTC Julian Date</param>
        /// <returns>True if OK otherwise throws an exception</returns>
        /// <remarks>The velocity vector is that of the observer with respect to the Earth's center 
        /// of mass. The Julian date must be UTC Julian date, not terrestrial. This call will use 
        /// the internal tables and estimator to get delta-T.
        /// This overload is not available through COM, please use 
        /// "SetFromSiteJD(ByVal site As Site, ByVal ujd As Double, ByVal delta_t As Double)"
        /// with delta_t set to 0.0 to achieve this effect.
        /// </remarks>
        bool SetFromSiteJD([MarshalAs(UnmanagedType.IDispatch)] Site site, double ujd);
    }
}