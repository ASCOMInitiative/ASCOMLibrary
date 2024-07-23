using ASCOM.Common;
using ASCOM.Common.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace ASCOM.Tools.Novas31
{

    /// <summary>
    /// NOVAS31: Class presenting the contents of the USNO NOVAS 3.1 library. 
    /// NOVAS was developed by the Astronomical Applications department of the United States Naval 
    /// Observatory.
    /// </summary>
    /// <remarks>If you wish to explore or utilise NOVAS3.1 please see USNO's extensive help document "NOVAS 3.1 Users Guide" 
    /// (NOVAS C3.1 Guide.pdf) included in the ASCOM Platform Docs start menu folder. The latest revision is also available on the USNO web site at
    /// <href>http://www.usno.navy.mil/USNO/astronomical-applications/software-products/novas</href>
    /// in the "C Edition of NOVAS" link. 
    /// </remarks>
    public class Novas
    {
        private const string NOVAS_LIBRARY = "libnovas"; // Base name of the NOVAS library files. Relevant file extensions like .DLL and .so are added automatically by .NET when searching for the library.
        private const string RACIO_FILE = "cio_ra.bin"; // Name of the RA of CIO binary data file

        private const string JPL_EPHEM_FILE_NAME = "JPLEPH"; // Name of JPL ephemeredes file

        private static ILogger TL; // Logger instance for this component instance

        private static bool isInitialised = false; // Flag indicating whether the library initialisation has been run

        private static string raCioFile = "NotFound"; // Location of the ra_cio.bin file

        #region Initialiser

        /// <summary>
        /// Static initialiser
        /// </summary>
        /// <exception cref="HelperException">Thrown if the JPLEPH and cio_ra.bin support files are not in the application directory.</exception>
        /// <remarks></remarks>
        static Novas()
        {
        }

        /// <summary>
        /// Initialise the library only runs once
        /// </summary>
        /// <exception cref="HelperException">Thrown if the JPLEPH and cio_ra.bin support files are not in the application directory.</exception>
        /// <remarks>
        /// This delayed initialisation approach is used so that the logger can be assigned before the initialiser is called.
        /// This makes it possible to see debug logging from the initialiser.
        /// </remarks>
        private static void Initialise()
        {
            short rc1;
            string JPLEphFile;
            var DENumber = default(short);
            string aplicationPath;

            try
            {
                // Uncomment here to enable initiator debug logging (don't leave enabled in production!)
                //TL = new TraceLogger("NOVASLibrary", true);
                //TL.SetMinimumLoggingLevel(LogLevel.Debug);
                LogMessage("Initialise", "Initialise");

                // Create a string list in which to collect cio_ra.bin search directories
                SortedSet<string> searchPaths = new SortedSet<string>();

                // Add several application related directories
                try
                {
                    string searchPath = Environment.CurrentDirectory.TrimEnd('\\');
                    LogMessage("Initialise", $"Environment.CurrentDirectory: {searchPath}");
                    searchPaths.Add(searchPath);
                }
                catch (Exception ex)
                {
                    LogMessage("Initialise", $"Environment.CurrentDirectory: {ex.Message}");
                }

                try
                {
                    string searchPath = Directory.GetCurrentDirectory().TrimEnd('\\');
                    LogMessage("Initialise", $"Directory.GetCurrentDirectory(): {searchPath}");
                    searchPaths.Add(searchPath);
                }
                catch (Exception ex)
                {
                    LogMessage("Initialise", $"Directory.GetCurrentDirectory(): {ex.Message}");
                }

                try
                {
                    string searchPath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
                    LogMessage("Initialise", $"AppDomain.CurrentDomain.BaseDirectory: {searchPath}");
                    searchPaths.Add(searchPath);
                }
                catch (Exception ex)
                {
                    LogMessage("Initialise", $"AppDomain.CurrentDomain.BaseDirectory: {ex.Message}");
                }

                try
                {
                    string searchPath = AppContext.BaseDirectory.TrimEnd('\\');
                    LogMessage("Initialise", $"AppContext.BaseDirectory: {searchPath}");
                    searchPaths.Add(searchPath);
                }
                catch (Exception ex)
                {
                    LogMessage("Initialise", $"AppContext.BaseDirectory: {ex.Message}");
                }

                try
                {
                    // Get the locations of any binary library files
                    if (AppContext.GetData("NATIVE_DLL_SEARCH_DIRECTORIES") is string searchDirectoriesString) // Some directories were found
                    {
                        LogMessage("Initialise", $"Found binary search directories: {searchDirectoriesString}");

                        // Separate out the path names and add them to the search directories list
                        char separatorChar = Path.PathSeparator;

                        // Remove any leading or trailing separators
                        searchDirectoriesString = searchDirectoriesString.Trim(separatorChar);

                        string[] searchDirectories = searchDirectoriesString?.Split(separatorChar);
                        foreach (string directory in searchDirectories)
                        {
                            LogMessage("Initialise", $"Found binary search directory: {directory}");
                        }

                        searchPaths.UnionWith(searchDirectories);
                    }
                }
                catch (Exception ex)
                {
                    LogMessage("Initialise", $"NATIVE_DLL_SEARCH_DIRECTORIES: {ex.Message}");
                }

                // Search each path for the cio_ra.bin file
                foreach (string directoryPath in searchPaths)
                {
                    try
                    {
                        LogMessage("Initialise", $"Found native directory path: {directoryPath}");

                        // Create a full path to the cio_ra.bin fie
                        string possibleFile = Path.Combine(directoryPath, RACIO_FILE);

                        // Test whether the file exists
                        if (File.Exists(possibleFile)) // File does exist
                        {
                            // Save the full path for use later
                            raCioFile = possibleFile;

                            // Set the path in the NOVAS 3.1 binary DLL
                            LogMessage("Initialise", $"Found {RACIO_FILE} file at {raCioFile}!!!");
                            SetRACIOFile(raCioFile);
                            LogMessage("Initialise", $"Set {RACIO_FILE} file to {raCioFile}!!!!!");

                            // Exit the foreach loop and don't bother with any other paths
                            break;
                        }
                    }
                    catch { }
                }

                // Get the current directory
                aplicationPath = Directory.GetCurrentDirectory();

                // Create a path to the ephemeris file.
                JPLEphFile = Path.Combine(aplicationPath, JPL_EPHEM_FILE_NAME);
                LogMessage("Initialise", $"Current path: {aplicationPath}, RACIO file: {raCioFile}, JPL ephemeris file: {JPLEphFile}");

                // Validate that the planetary ephemeris file exists
                if (!File.Exists(JPLEphFile))
                {
                    LogMessage("Initialise", $"NOVAS31 Initialise - Unable to locate JPL ephemeris file: {JPLEphFile}");
                    throw new HelperException($"NOVAS31 Initialise - Unable to locate JPL ephemeris file: {JPLEphFile}");
                }

                // Open the ephemeris file and set its applicable date range
                LogMessage("Initialise", "Opening JPL ephemeris file: " + JPLEphFile);
                double ephStart = 0.0, ephEnd = 0.0;
                rc1 = EphemOpen(JPLEphFile, ref ephStart, ref ephEnd, ref DENumber);

                if (rc1 > 0)
                {
                    LogMessage("Initialise", "Unable to open ephemeris file: " + JPLEphFile + ", RC: " + rc1);
                    throw new HelperException($"NOVAS31 Initialisation - Unable to open ephemeris file: {JPLEphFile} RC: {rc1}");
                }

                LogMessage("Initialise", $"Ephemeris file {JPLEphFile} opened OK - DE number: {DENumber}, Start: {ephStart}, End: {ephEnd}");
                LogMessage("Initialise", "NOVAS31 initialised OK");
            }

            // Re-throw any HelperExceptions thrown above
            catch (HelperException)
            {
                throw;
            }

            // Log any other exceptions and re-throw.
            catch (Exception ex)
            {
                try { LogMessage("Initialise", $"Exception: {ex.Message}\r\n{ex}"); } catch { }
                throw;
            }

            // Set the initialised flag
            isInitialised = true;
        }

        #endregion

        #region Public helper members

        /// <summary>
        /// Assign a logger instance to the NOVAS component
        /// </summary>
        /// <param name="logger">an ILigger instance</param>
        public static void SetLogger(ILogger logger)
        {
            TL = logger;
        }

        #endregion

        #region Public NOVAS Interface - Ephemeris Members

        /// <summary>
        /// Get position and velocity of target with respect to the centre object. 
        /// </summary>
        /// <param name="Tjd"> Two-element array containing the Julian date, which may be split any way (although the first 
        /// element is usually the "integer" part, and the second element is the "fractional" part).  Julian date is in the 
        /// TDB or "T_eph" time scale.</param>
        /// <param name="Target">Target object</param>
        /// <param name="Center">Centre object</param>
        /// <param name="Position">Position vector array of target relative to center, measured in AU.</param>
        /// <param name="Velocity">Velocity vector array of target relative to center, measured in AU/day.</param>
        /// <returns><pre>
        /// 0   ...everything OK.
        /// 1,2 ...error returned from State.</pre>
        /// </returns>
        /// <remarks>This function accesses the JPL planetary ephemeris to give the position and velocity of the target 
        /// object with respect to the center object.</remarks>
        public static short PlanetEphemeris(ref double[] Tjd, Target Target, Target Center, ref double[] Position, ref double[] Velocity)
        {
            var JdHp = new JDHighPrecision();
            var VPos = new PosVector();
            var VVel = new VelVector();
            short rc;

            // Initialise if necessary
            if (!isInitialised) Initialise();

            JdHp.JDPart1 = Tjd[0];
            JdHp.JDPart2 = Tjd[1];
            rc = PlanetEphemerisLib(ref JdHp, Target, Center, ref VPos, ref VVel);

            PosVecToArr(VPos, ref Position);
            VelVecToArr(VVel, ref Velocity);
            return rc;
        }

        /// <summary>
        /// Produces the Cartesian heliocentric equatorial coordinates of the asteroid for the J2000.0 epoch 
        /// coordinate system from a set of Chebyshev polynomials read from a file.
        /// </summary>
        /// <param name="Mp">The number of the asteroid for which the position in desired.</param>
        /// <param name="Name">The name of the asteroid.</param>
        /// <param name="Jd"> The Julian date on which to find the position and velocity.</param>
        /// <param name="Err"><pre>
        /// = 0 ( No error )
        /// = 1 ( Memory allocation error )
        /// = 2 ( Mismatch between asteroid name and number )
        /// = 3 ( Julian date out of bounds )
        /// = 4 ( Cannot find Chebyshev polynomial file )
        /// </pre>
        /// </param>
        /// <returns> 6-element array of double containing position and velocity vector values.</returns>
        /// <remarks>The file name of the asteroid is taken from the name given.  It is	assumed that the name 
        /// is all in lower case characters.
        /// <para>
        /// This routine will search in the application's current directory for a file of Chebyshev 
        /// polynomial coefficients whose name is based on the provided Name parameter: Name.chby 
        /// </para>
        /// <para>Further information on using NOVAS with minor planet data is given here: 
        /// http://www.usno.navy.mil/USNO/astronomical-applications/software-products/usnoae98</para>
        /// </remarks>
        public static double[] ReadEph(int Mp, string Name, double Jd, ref int Err)
        {
            const int DOUBLE_LENGTH = 8;
            const int NUM_RETURN_VALUES = 6;

            var PosVec = new double[6];
            IntPtr EphPtr;
            var Bytes = new byte[49];

            EphPtr = ReadEphLib(Mp, Name, Jd, ref Err);

            if (Err == 0) // Get the returned values if the call was successful
            {
                if (EphPtr != IntPtr.Zero) // Only copy if the pointer is not NULL
                {
                    // Safely marshal unmanaged buffer to byte()
                    Marshal.Copy(EphPtr, Bytes, 0, NUM_RETURN_VALUES * DOUBLE_LENGTH);

                    // Convert to double()
                    for (int i = 0; i <= NUM_RETURN_VALUES - 1; i++)
                        PosVec[i] = BitConverter.ToDouble(Bytes, i * DOUBLE_LENGTH);
                }
                else
                {
                    for (int i = 0; i <= NUM_RETURN_VALUES - 1; i++)
                        PosVec[i] = double.NaN; // Return invalid values
                }
            }
            return PosVec;
        }

        /// <summary>
        /// Interface between the JPL direct-access solar system ephemerides and NOVAS-C.
        /// </summary>
        /// <param name="Tjd">Julian date of the desired time, on the TDB time scale.</param>
        /// <param name="Body">Body identification number for the solar system object of interest; 
        /// Mercury = 1, ..., Pluto= 9, Sun= 10, Moon = 11.</param>
        /// <param name="Origin">Origin code; solar system barycenter= 0, center of mass of the Sun = 1, center of Earth = 2.</param>
        /// <param name="Pos">Position vector of 'body' at tjd; equatorial rectangular coordinates in AU referred to the ICRS.</param>
        /// <param name="Vel">Velocity vector of 'body' at tjd; equatorial rectangular system referred to the ICRS.</param>
        /// <returns>Always returns 0</returns>
        /// <remarks></remarks>
        public static short SolarSystem(double Tjd, Body Body, Origin Origin, ref double[] Pos, ref double[] Vel)
        {

            var VPos = new PosVector();
            var VVel = new VelVector();
            short rc;

            // Initialise if necessary
            if (!isInitialised) Initialise();

            rc = SolarSystemLib(Tjd, (short)Body, (short)Origin, ref VPos, ref VVel);

            PosVecToArr(VPos, ref Pos);
            VelVecToArr(VVel, ref Vel);
            return rc;
        }

        /// <summary>
        /// Read and interpolate the JPL planetary ephemeris file.
        /// </summary>
        /// <param name="Jed">2-element Julian date (TDB) at which interpolation is wanted. Any combination of jed[0]+jed[1] which falls within the time span on the file is a permissible epoch.  See Note 1 below.</param>
        /// <param name="Target">The requested body to get data for from the ephemeris file.</param>
        /// <param name="TargetPos">The barycentric position vector array of the requested object, in AU. (If target object is the Moon, then the vector is geocentric.)</param>
        /// <param name="TargetVel">The barycentric velocity vector array of the requested object, in AU/Day.</param>
        /// <returns>
        /// <pre>
        /// 0 ...everything OK
        /// 1 ...error reading ephemeris file
        /// 2 ...epoch out of range.
        /// </pre></returns>
        /// <remarks>
        /// The target number designation of the astronomical bodies is:
        /// <pre>
        ///         = 0: Mercury,               1: Venus, 
        ///         = 2: Earth-Moon barycenter, 3: Mars, 
        ///         = 4: Jupiter,               5: Saturn, 
        ///         = 6: Uranus,                7: Neptune, 
        ///         = 8: Pluto,                 9: geocentric Moon, 
        ///         =10: Sun.
        /// </pre>
        /// <para>
        /// NOTE 1. For ease in programming, the user may put the entire epoch in jed[0] and set jed[1] = 0. 
        /// For maximum interpolation accuracy,  set jed[0] = the most recent midnight at or before interpolation epoch, 
        /// and set jed[1] = fractional part of a day elapsed between jed[0] and epoch. As an alternative, it may prove 
        /// convenient to set jed[0] = some fixed epoch, such as start of the integration and jed[1] = elapsed interval 
        /// between then and epoch.
        /// </para>
        /// </remarks>
        public static short State(ref double[] Jed, Target Target, ref double[] TargetPos, ref double[] TargetVel)
        {

            var JdHp = new JDHighPrecision();
            var VPos = new PosVector();
            var VVel = new VelVector();
            short rc;

            // Initialise if necessary
            if (!isInitialised) Initialise();

            JdHp.JDPart1 = Jed[0];
            JdHp.JDPart2 = Jed[1];
            rc = StateLib(ref JdHp, Target, ref VPos, ref VVel);

            PosVecToArr(VPos, ref TargetPos);
            VelVecToArr(VVel, ref TargetVel);
            return rc;
        }

        /// <summary>
        /// Open the supplied ephemeris file.
        /// </summary>
        /// <param name="EphemFileName">File name of the ephemeris file to open</param>
        /// <param name="JDBegin">Beginning Julian date from the ephemeris file</param>
        /// <param name="JDEnd">End Julian date from the ephemeris file.</param>
        /// <param name="DENumber">DE number from the ephemeris file e.g. DE405 or DE421.</param>
        /// <returns>0 for success otherwise an error code.</returns>
        public static short EphemOpen(string EphemFileName, ref double JDBegin, ref double JDEnd, ref short DENumber)
        {
            short rc;
            rc = EphemOpenLib(EphemFileName, ref JDBegin, ref JDEnd, ref DENumber);
            return rc;
        }

        /// <summary>
        /// Close the current ephemeris file
        /// </summary>
        /// <returns>0 for success otherwise an error code.</returns>
        public static short EphemClose()
        {
            return EphemCloseLib();
        }

        /// <summary>
        /// Close all open ephemeris files and release allocated memory.
        /// </summary>
        public static void CleanEph()
        {
            CleanEphLib();
        }

        #endregion

        #region Public NOVAS Interface Members
        /// <summary>
        /// Corrects position vector for aberration of light.  Algorithm includes relativistic terms.
        /// </summary>
        /// <param name="Pos"> Position vector, referred to origin at center of mass of the Earth, components in AU.</param>
        /// <param name="Vel"> Velocity vector of center of mass of the Earth, referred to origin at solar system barycenter, components in AU/day.</param>
        /// <param name="LightTime"> Light time from object to Earth in days.</param>
        /// <param name="Pos2"> Position vector, referred to origin at center of mass of the Earth, corrected for aberration, components in AU</param>
        /// <remarks>If 'lighttime' = 0 on input, this function will compute it.</remarks>
        public static void Aberration(double[] Pos, double[] Vel, double LightTime, ref double[] Pos2)
        {
            var VPos2 = default(PosVector);
            var argPos1 = ArrToPosVec(Pos);
            var argVel1 = ArrToVelVec(Vel);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            AberrationLib(ref argPos1, ref argVel1, LightTime, ref VPos2);
            PosVecToArr(VPos2, ref Pos2);
        }

        /// <summary>
        /// Compute the apparent place of a planet or other solar system body.
        /// </summary>
        /// <param name="JdTt"> TT Julian date for apparent place.</param>
        /// <param name="SsBody"> Pointer to structure containing the body designation for the solar system body </param>
        /// <param name="Accuracy"> Code specifying the relative accuracy of the output position.</param>
        /// <param name="Ra">Apparent right ascension in hours, referred to true equator and equinox of date.</param>
        /// <param name="Dec"> Apparent declination in degrees, referred to true equator and equinox of date.</param>
        /// <param name="Dis"> True distance from Earth to planet at 'JdTt' in AU.</param>
        /// <returns><pre>
        ///    0 ... Everything OK
        ///    1 ... Invalid value of 'Type' in structure 'SsBody'
        /// > 10 ... Error code from function 'Place'.
        /// </pre></returns>
        /// <remarks></remarks>
        public static short AppPlanet(double JdTt, Object3 SsBody, Accuracy Accuracy, ref double Ra, ref double Dec, ref double Dis)
        {
            var argSsBody1 = O3IFromObject3(SsBody);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            return AppPlanetLib(JdTt, ref argSsBody1, Accuracy, ref Ra, ref Dec, ref Dis);
        }

        /// <summary>
        /// Computes the apparent place of a star at date 'JdTt', given its catalog mean place, proper motion, parallax, and radial velocity.
        /// </summary>
        /// <param name="JdTt">TT Julian date for apparent place.</param>
        /// <param name="Star">Catalog entry structure containing catalog data forthe object in the ICRS </param>
        /// <param name="Accuracy">Code specifying the relative accuracy of the output position.</param>
        /// <param name="Ra">Apparent right ascension in hours, referred to true equator and equinox of date 'JdTt'.</param>
        /// <param name="Dec">Apparent declination in degrees, referred to true equator and equinox of date 'JdTt'.</param>
        /// <returns>
        /// <pre>
        ///    0 ... Everything OK
        /// > 10 ... Error code from function 'MakeObject'
        /// > 20 ... Error code from function 'Place'.
        /// </pre></returns>
        /// <remarks></remarks>
        public static short AppStar(double JdTt, CatEntry3 Star, Accuracy Accuracy, ref double Ra, ref double Dec)
        {
            short rc;

            // Initialise if necessary
            if (!isInitialised) Initialise();

            // Initialise if necessary
            if (!isInitialised) Initialise();

            try
            {
                LogMessage("AppStar", "JD Accuracy:        " + JdTt + " " + Accuracy.ToString());
                LogMessage("AppStar", "  Star.RA:          " + Utilities.HoursToHMS(Star.RA, ":", ":", "", 3));
                LogMessage("AppStar", "  Dec:              " + Utilities.DegreesToDMS(Star.Dec, ":", ":", "", 3));
                LogMessage("AppStar", "  Catalog:          " + Star.Catalog);
                LogMessage("AppStar", "  Parallax:         " + Star.Parallax);
                LogMessage("AppStar", "  ProMoDec:         " + Star.ProMoDec);
                LogMessage("AppStar", "  ProMoRA:          " + Star.ProMoRA);
                LogMessage("AppStar", "  RadialVelocity:   " + Star.RadialVelocity);
                LogMessage("AppStar", "  StarName:         " + Star.StarName);
                LogMessage("AppStar", "  StarNumber:       " + Star.StarNumber);
            }
            catch (Exception ex)
            {
                LogMessage("AppStar", "Exception: " + ex.ToString());
            }

            rc = AppStarLib(JdTt, ref Star, Accuracy, ref Ra, ref Dec);
            LogMessage("AppStar", "  Return Code: " + rc + ", RA Dec: " + Utilities.HoursToHMS(Ra, ":", ":", "", 3) + " " + Utilities.DegreesToDMS(Dec, ":", ":", "", 3));
            return rc;
        }

        /// <summary>
        /// Compute the astrometric place of a planet or other solar system body.
        /// </summary>
        /// <param name="JdTt">TT Julian date for astrometric place.</param>
        /// <param name="SsBody">structure containing the body designation for the solar system body </param>
        /// <param name="Accuracy">Code specifying the relative accuracy of the output position.</param>
        /// <param name="Ra">Astrometric right ascension in hours (referred to the ICRS, without light deflection or aberration).</param>
        /// <param name="Dec">Astrometric declination in degrees (referred to the ICRS, without light deflection or aberration).</param>
        /// <param name="Dis">True distance from Earth to planet in AU.</param>
        /// <returns>
        /// <pre>
        ///    0 ... Everything OK
        ///    1 ... Invalid value of 'Type' in structure 'SsBody'
        /// > 10 ... Error code from function 'Place'.
        /// </pre></returns>
        /// <remarks></remarks>
        public static short AstroPlanet(double JdTt, Object3 SsBody, Accuracy Accuracy, ref double Ra, ref double Dec, ref double Dis)
        {
            var argSsBody1 = O3IFromObject3(SsBody);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            return AstroPlanetLib(JdTt, ref argSsBody1, Accuracy, ref Ra, ref Dec, ref Dis);
        }

        /// <summary>
        /// Computes the astrometric place of a star at date 'JdTt', given its catalog mean place, proper motion, parallax, and radial velocity.
        /// </summary>
        /// <param name="JdTt">TT Julian date for astrometric place.</param>
        /// <param name="Star">Catalog entry structure containing catalog data for the object in the ICRS</param>
        /// <param name="Accuracy">Code specifying the relative accuracy of the output position.</param>
        /// <param name="Ra">Astrometric right ascension in hours (referred to the ICRS, without light deflection or aberration).</param>
        /// <param name="Dec">Astrometric declination in degrees (referred to the ICRS, without light deflection or aberration).</param>
        /// <returns><pre>
        ///    0 ... Everything OK
        /// > 10 ... Error code from function 'MakeObject'
        /// > 20 ... Error code from function 'Place'.
        /// </pre></returns>
        /// <remarks></remarks>
        public static short AstroStar(double JdTt, CatEntry3 Star, Accuracy Accuracy, ref double Ra, ref double Dec)
        {
            short rc;

            // Initialise if necessary
            if (!isInitialised) Initialise();

            try
            {
                LogMessage("AstroStar", "JD Accuracy:        " + JdTt + " " + Accuracy.ToString());
                LogMessage("AstroStar", "  Star.RA:          " + Utilities.HoursToHMS(Star.RA, ":", ":", "", 3));
                LogMessage("AstroStar", "  Dec:              " + Utilities.DegreesToDMS(Star.Dec, ":", ":", "", 3));
                LogMessage("AstroStar", "  Catalog:          " + Star.Catalog);
                LogMessage("AstroStar", "  Parallax:         " + Star.Parallax);
                LogMessage("AstroStar", "  ProMoDec:         " + Star.ProMoDec);
                LogMessage("AstroStar", "  ProMoRA:          " + Star.ProMoRA);
                LogMessage("AstroStar", "  RadialVelocity:   " + Star.RadialVelocity);
                LogMessage("AstroStar", "  StarName:         " + Star.StarName);
                LogMessage("AstroStar", "  StarNumber:       " + Star.StarNumber);
            }
            catch (Exception ex)
            {
                LogMessage("AstroStar", "Exception: " + ex.ToString());
            }

            rc = AstroStarLib(JdTt, ref Star, Accuracy, ref Ra, ref Dec);
            LogMessage("AstroStar", "  Return Code: " + rc + ", RA Dec: " + Utilities.HoursToHMS(Ra, ":", ":", "", 3) + " " + Utilities.DegreesToDMS(Dec, ":", ":", "", 3));
            return rc;
        }

        /// <summary>
        /// Move the origin of coordinates from the barycenter of the solar system to the observer (or the geocenter); i.e., this function accounts for parallax (annual+geocentric or justannual).
        /// </summary>
        /// <param name="Pos">Position vector, referred to origin at solar system barycenter, components in AU.</param>
        /// <param name="PosObs">Position vector of observer (or the geocenter), with respect to origin at solar system barycenter, components in AU.</param>
        /// <param name="Pos2"> Position vector, referred to origin at center of mass of the Earth, components in AU.</param>
        /// <param name="Lighttime">Light time from object to Earth in days.</param>
        /// <remarks></remarks>
        public static void Bary2Obs(double[] Pos, double[] PosObs, ref double[] Pos2, ref double Lighttime)
        {
            var PosV = new PosVector();
            var argPos1 = ArrToPosVec(Pos);
            var argPosObs1 = ArrToPosVec(PosObs);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            Bary2ObsLib(ref argPos1, ref argPosObs1, ref PosV, ref Lighttime);
            PosVecToArr(PosV, ref Pos2);
        }

        /// <summary>
        /// This function will compute a date on the Gregorian calendar given the Julian date.
        /// </summary>
        /// <param name="Tjd">Julian date.</param>
        /// <param name="Year">Year</param>
        /// <param name="Month">Month number</param>
        /// <param name="Day">day number</param>
        /// <param name="Hour">Fractional hour of the day</param>
        /// <remarks></remarks>
        public static void CalDate(double Tjd, ref short Year, ref short Month, ref short Day, ref double Hour)
        {
            // Initialise if necessary
            if (!isInitialised) Initialise();

            CalDateLib(Tjd, ref Year, ref Month, ref Day, ref Hour);
        }

        /// <summary>
        /// This function rotates a vector from the celestial to the terrestrial system.  Specifically, it transforms a vector in the
        /// GCRS (a local space-fixed system) to the ITRS (a rotating earth-fixed system) by applying rotations for the GCRS-to-dynamical
        /// frame tie, precession, nutation, Earth rotation, and polar motion.
        /// </summary>
        /// <param name="JdHigh">High-order part of UT1 Julian date.</param>
        /// <param name="JdLow">Low-order part of UT1 Julian date.</param>
        /// <param name="DeltaT">Value of Delta T (= TT - UT1) at the input UT1 Julian date.</param>
        /// <param name="Method"> Selection for method: 0 ... CIO-based method; 1 ... equinox-based method</param>
        /// <param name="Accuracy">Selection for accuracy</param>
        /// <param name="OutputOption">0 ... The output vector is referred to GCRS axes; 1 ... The output 
        /// vector is produced with respect to the equator and equinox of date. (See note 2 below)</param>
        /// <param name="xp">Conventionally-defined X coordinate of celestial intermediate pole with respect to 
        /// ITRS pole, in arcseconds.</param>
        /// <param name="yp">Conventionally-defined Y coordinate of celestial intermediate pole with respect to 
        /// ITRS pole, in arcseconds.</param>
        /// <param name="VecT">Position vector, geocentric equatorial rectangular coordinates,
        /// referred to GCRS axes (celestial system) or with respect to
        /// the equator and equinox of date, depending on 'option'.</param>
        /// <param name="VecC">Position vector, geocentric equatorial rectangular coordinates,
        /// referred to ITRS axes (terrestrial system).</param>
        /// <returns><pre>
        ///    0 ... everything is ok
        ///    1 ... invalid value of 'Accuracy'
        ///    2 ... invalid value of 'Method'
        /// > 10 ... 10 + error from function 'CioLocation'
        /// > 20 ... 20 + error from function 'CioBasis'
        /// </pre></returns>
        /// <remarks>Note 1: 'x' = 'y' = 0 means no polar motion transformation.
        /// <para>
        /// Note2: 'option' = 1 only works for the equinox-based method.
        /// </para></remarks>
        public static short Cel2Ter(double JdHigh, double JdLow, double DeltaT, Method Method, Accuracy Accuracy, OutputVectorOption OutputOption, double xp, double yp, double[] VecT, ref double[] VecC)
        {
            var VVecC = new PosVector();
            short rc;
            var argVecT1 = ArrToPosVec(VecT);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            rc = Cel2TerLib(JdHigh, JdLow, DeltaT, Method, Accuracy, OutputOption, xp, yp, ref argVecT1, ref VVecC);

            PosVecToArr(VVecC, ref VecC);
            return rc;
        }

        /// <summary>
        /// This function allows for the specification of celestial pole offsets for high-precision applications.  Each set of offsets is a correction to the modeled position of the pole for a specific date, derived from observations and published by the IERS.
        /// </summary>
        /// <param name="Tjd">TDB or TT Julian date for pole offsets.</param>
        /// <param name="Type"> Type of pole offset. 1 for corrections to angular coordinates of modeled pole referred to mean ecliptic of date, that is, delta-delta-psi and delta-delta-epsilon.  2 for corrections to components of modeled pole unit vector referred to GCRS axes, that is, dx and dy.</param>
        /// <param name="Dpole1">Value of celestial pole offset in first coordinate, (delta-delta-psi or dx) in milliarcseconds.</param>
        /// <param name="Dpole2">Value of celestial pole offset in second coordinate, (delta-delta-epsilon or dy) in milliarcseconds.</param>
        /// <returns><pre>
        /// 0 ... Everything OK
        /// 1 ... Invalid value of 'Type'.
        /// </pre></returns>
        /// <remarks></remarks>
        public static short CelPole(double Tjd, PoleOffsetCorrection Type, double Dpole1, double Dpole2)
        {
            // Initialise if necessary
            if (!isInitialised) Initialise();

            return CelPoleLib(Tjd, Type, Dpole1, Dpole2);
        }

        /// <summary>
        /// Calaculate an array of CIO RA values around a given date
        /// </summary>
        /// <param name="JdTdb">TDB Julian date.</param>
        /// <param name="NPts"> Number of Julian dates and right ascension values requested (not less than 2 or more than 20).</param>
        /// <param name="Cio"> An arraylist of RaOfCIO structures containing a time series of the right ascension of the 
        /// Celestial Intermediate Origin (CIO) with respect to the GCRS.</param>
        /// <returns><pre>
        /// 0 ... everything OK
        /// 1 ... error opening the 'cio_ra.bin' file
        /// 2 ... 'JdTdb' not in the range of the CIO file; 
        /// 3 ... 'NPts' out of range
        /// 4 ... unable to allocate memory for the internal 't' array; 
        /// 5 ... unable to allocate memory for the internal 'ra' array; 
        /// 6 ... 'JdTdb' is too close to either end of the CIO file; unable to put 'NPts' data points into the output object.
        /// </pre></returns>
        /// <remarks>
        /// <para>
        /// Given an input TDB Julian date and the number of data points desired, this function returns a set of 
        /// Julian dates and corresponding values of the GCRS right ascension of the celestial intermediate origin (CIO).  
        /// The range of dates is centered (at least approximately) on the requested date.  The function obtains 
        /// the data from an external data file.</para>
        /// <example>How to create and retrieve values from the arraylist
        /// <code>
        /// Dim CioList As New ArrayList, Nov3 As New ASCOM.Astrometry.NOVAS3
        /// 
        /// rc = Nov3.CioArray(2455251.5, 20, CioList) ' Get 20 values around date 00:00:00 February 24th 2010
        /// MsgBox("Nov3 RC= " <![CDATA[&]]>  rc)
        /// 
        /// For Each CioA As ASCOM.Astrometry.RAOfCio In CioList
        ///     MsgBox("CIO Array " <![CDATA[&]]> CioA.JdTdb <![CDATA[&]]> " " <![CDATA[&]]> CioA.RACio)
        /// Next
        /// </code>
        /// </example>
        /// </remarks>
        public static short CioArray(double JdTdb, int NPts, ref ArrayList Cio)
        {
            var CioStruct = new RAOfCioArray();
            short rc;

            // Initialise if necessary
            if (!isInitialised) Initialise();

            CioStruct.Initialise(); // Set internal default values so we can see which elements are changed by the NOVAS DLL.
            rc = CioArrayLib(JdTdb, NPts, ref CioStruct);

            RACioArrayStructureToArr(CioStruct, ref Cio); // Copy data from the CioStruct structure to the returning arraylist
            return rc;
        }

        /// <summary>
        /// Compute the orthonormal basis vectors of the celestial intermediate system.
        /// </summary>
        /// <param name="JdTdbEquionx">TDB Julian date of epoch.</param>
        /// <param name="RaCioEquionx">Right ascension of the CIO at epoch (hours).</param>
        /// <param name="RefSys">Reference system in which right ascension is given. 1 ... GCRS; 2 ... True equator and equinox of date.</param>
        /// <param name="Accuracy">Accuracy</param>
        /// <param name="x">Unit vector toward the CIO, equatorial rectangular coordinates, referred to the GCRS.</param>
        /// <param name="y">Unit vector toward the y-direction, equatorial rectangular coordinates, referred to the GCRS.</param>
        /// <param name="z">Unit vector toward north celestial pole (CIP), equatorial rectangular coordinates, referred to the GCRS.</param>
        /// <returns><pre>
        /// 0 ... everything OK
        /// 1 ... invalid value of input variable 'RefSys'.
        /// </pre></returns>
        /// <remarks>
        /// To compute the orthonormal basis vectors, with respect to the GCRS (geocentric ICRS), of the celestial 
        /// intermediate system defined by the celestial intermediate pole (CIP) (in the z direction) and 
        /// the celestial intermediate origin (CIO) (in the x direction).  A TDB Julian date and the 
        /// right ascension of the CIO at that date is required as input.  The right ascension of the CIO 
        /// can be with respect to either the GCRS origin or the true equinox of date -- different algorithms 
        /// are used in the two cases.</remarks>
        public static short CioBasis(double JdTdbEquionx, double RaCioEquionx, ReferenceSystem RefSys, Accuracy Accuracy, ref double x, ref double y, ref double z)
        {
            // Initialise if necessary
            if (!isInitialised) Initialise();

            return CioBasisLib(JdTdbEquionx, RaCioEquionx, RefSys, Accuracy, ref x, ref y, ref z);
        }

        /// <summary>
        /// Returns the location of the celestial intermediate origin (CIO) for a given Julian date, as a right ascension 
        /// </summary>
        /// <param name="JdTdb">TDB Julian date.</param>
        /// <param name="Accuracy">Selection for accuracy</param>
        /// <param name="RaCio">Right ascension of the CIO, in hours.</param>
        /// <param name="RefSys">Reference system in which right ascension is given</param>
        /// <returns><pre>
        ///    0 ... everything OK
        ///    1 ... unable to allocate memory for the 'cio' array
        /// > 10 ... 10 + the error code from function 'CioArray'.
        /// </pre></returns>
        /// <remarks>  This function returns the location of the celestial intermediate origin (CIO) for a given Julian date, as a right ascension with respect to either the GCRS (geocentric ICRS) origin or the true equinox of date.  The CIO is always located on the true equator (= intermediate equator) of date.</remarks>
        public static short CioLocation(double JdTdb, Accuracy Accuracy, ref double RaCio, ref ReferenceSystem RefSys)
        {
            // Initialise if necessary
            if (!isInitialised) Initialise();

            return CioLocationLib(JdTdb, Accuracy, ref RaCio, ref RefSys);
        }

        /// <summary>
        /// Computes the true right ascension of the celestial intermediate origin (CIO) at a given TT Julian date.  This is -(equation of the origins).
        /// </summary>
        /// <param name="JdTt">TT Julian date</param>
        /// <param name="Accuracy">Selection for accuracy</param>
        /// <param name="RaCio"> Right ascension of the CIO, with respect to the true equinox of date, in hours (+ or -).</param>
        /// <returns>
        /// <pre>
        ///   0  ... everything OK
        ///   1  ... invalid value of 'Accuracy'
        /// > 10 ... 10 + the error code from function 'CioLocation'
        /// > 20 ... 20 + the error code from function 'CioBasis'.
        /// </pre></returns>
        /// <remarks></remarks>
        public static short CioRa(double JdTt, Accuracy Accuracy, ref double RaCio)
        {
            // Initialise if necessary
            if (!isInitialised) Initialise();

            return CioRaLib(JdTt, Accuracy, ref RaCio);
        }

        /// <summary>
        /// Returns the difference in light-time, for a star, between the barycenter of the solar system and the observer (or the geocenter).
        /// </summary>
        /// <param name="Pos1">Position vector of star, with respect to origin at solar system barycenter.</param>
        /// <param name="PosObs">Position vector of observer (or the geocenter), with respect to origin at solar system barycenter, components in AU.</param>
        /// <returns>Difference in light time, in the sense star to barycenter minus star to earth, in days.</returns>
        /// <remarks>
        /// Alternatively, this function returns the light-time from the observer (or the geocenter) to a point on a 
        /// light ray that is closest to a specific solar system body.  For this purpose, 'Pos1' is the position 
        /// vector toward observed object, with respect to origin at observer (or the geocenter); 'PosObs' is 
        /// the position vector of solar system body, with respect to origin at observer (or the geocenter), 
        /// components in AU; and the returned value is the light time to point on line defined by 'Pos1' 
        /// that is closest to solar system body (positive if light passes body before hitting observer, i.e., if 
        /// 'Pos1' is within 90 degrees of 'PosObs').
        /// </remarks>
        public static double DLight(double[] Pos1, double[] PosObs)
        {
            var argPos11 = ArrToPosVec(Pos1);
            var argPosObs1 = ArrToPosVec(PosObs);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            return DLightLib(ref argPos11, ref argPosObs1);
        }

        /// <summary>
        /// Converts an ecliptic position vector to an equatorial position vector.
        /// </summary>
        /// <param name="JdTt">TT Julian date of equator, equinox, and ecliptic used for coordinates.</param>
        /// <param name="CoordSys">Coordinate system selection. 0 ... mean equator and equinox of date; 1 ... true equator and equinox of date; 2 ... ICRS</param>
        /// <param name="Accuracy">Selection for accuracy</param>
        /// <param name="Pos1"> Position vector, referred to specified ecliptic and equinox of date.  If 'CoordSys' = 2, 'pos1' must be on mean ecliptic and equinox of J2000.0; see Note 1 below.</param>
        /// <param name="Pos2">Position vector, referred to specified equator and equinox of date.</param>
        /// <returns><pre>
        /// 0 ... everything OK
        /// 1 ... invalid value of 'CoordSys'
        /// </pre></returns>
        /// <remarks>
        /// To convert an ecliptic vector (mean ecliptic and equinox of J2000.0 only) to an ICRS vector, 
        /// set 'CoordSys' = 2; the value of 'JdTt' can be set to anything, since J2000.0 is assumed. 
        /// Except for the output from this case, all vectors are assumed to be with respect to a dynamical system.
        /// </remarks>
        public static short Ecl2EquVec(double JdTt, CoordSys CoordSys, Accuracy Accuracy, double[] Pos1, ref double[] Pos2)
        {
            var VPos2 = new PosVector();
            short rc;
            var argPos11 = ArrToPosVec(Pos1);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            rc = Ecl2EquVecLib(JdTt, CoordSys, Accuracy, ref argPos11, ref VPos2);

            PosVecToArr(VPos2, ref Pos2);
            return rc;
        }

        /// <summary>
        /// Compute the "complementary terms" of the equation of the equinoxes consistent with IAU 2000 resolutions.
        /// </summary>
        /// <param name="JdHigh">High-order part of TT Julian date.</param>
        /// <param name="JdLow">Low-order part of TT Julian date.</param>
        /// <param name="Accuracy">Selection for accuracy</param>
        /// <returns>Complementary terms, in radians.</returns>
        /// <remarks>
        /// 1. The series used in this function was derived from Series from IERS Conventions (2003), Chapter 5, Table 5.2C.
        /// This same series was also adopted for use in the IAU's Standards of Fundamental Astronomy (SOFA) software (i.e., subroutine 
        /// eect00.for and function eect00.c).
        /// <para>2. The low-accuracy series used in this function is a simple implementation derived from the first reference, in which terms
        /// smaller than 2 microarcseconds have been omitted.</para>
        /// <para>3. This function is based on NOVAS Fortran routine 'eect2000', with the low-accuracy formula taken from NOVAS Fortran routine 'etilt'.</para>
        /// </remarks>
        public static double EeCt(double JdHigh, double JdLow, Accuracy Accuracy)
        {
            // Initialise if necessary
            if (!isInitialised) Initialise();

            return EeCtLib(JdHigh, JdLow, Accuracy);
        }

        /// <summary>
        /// Retrieves the position and velocity of a solar system body from a fundamental ephemeris.
        /// </summary>
        /// <param name="Jd"> TDB Julian date split into two parts, where the sum jd[0] + jd[1] is the TDB Julian date.</param>
        /// <param name="CelObj">Structure containing the designation of the body of interest </param>
        /// <param name="Origin"> Origin code; solar system barycenter = 0, center of mass of the Sun = 1.</param>
        /// <param name="Accuracy">Slection for accuracy</param>
        /// <param name="Pos">Position vector of the body at 'Jd'; equatorial rectangular coordinates in AU referred to the ICRS.</param>
        /// <param name="Vel">Velocity vector of the body at 'Jd'; equatorial rectangular system referred to the mean equator and equinox of the ICRS, in AU/Day.</param>
        /// <returns><pre>
        ///    0 ... Everything OK
        ///    1 ... Invalid value of 'Origin'
        ///    2 ... Invalid value of 'Type' in 'CelObj'; 
        ///    3 ... Unable to allocate memory
        /// 10+n ... where n is the error code from 'SolarSystem'; 
        /// 20+n ... where n is the error code from 'ReadEph'.
        /// </pre></returns>
        /// <remarks>It is recommended that the input structure 'cel_obj' be created using function 'MakeObject' in file novas.c.</remarks>
        public static short Ephemeris(double[] Jd, Object3 CelObj, Origin Origin, Accuracy Accuracy, ref double[] Pos, ref double[] Vel)
        {
            var VPos = new PosVector();
            var VVel = new VelVector();
            JDHighPrecision JdHp;
            short rc;
            JdHp.JDPart1 = Jd[0];
            JdHp.JDPart2 = Jd[1];

            // Initialise if necessary
            if (!isInitialised) Initialise();

            var argCelObj1 = O3IFromObject3(CelObj);
            rc = EphemerisLib(ref JdHp, ref argCelObj1, Origin, Accuracy, ref VPos, ref VVel);

            PosVecToArr(VPos, ref Pos);
            VelVecToArr(VVel, ref Vel);
            return rc;
        }

        /// <summary>
        /// To convert right ascension and declination to ecliptic longitude and latitude.
        /// </summary>
        /// <param name="JdTt">TT Julian date of equator, equinox, and ecliptic used for coordinates.</param>
        /// <param name="CoordSys"> Coordinate system: 0 ... mean equator and equinox of date 'JdTt'; 1 ... true equator and equinox of date 'JdTt'; 2 ... ICRS</param>
        /// <param name="Accuracy">Selection for accuracy</param>
        /// <param name="Ra">Right ascension in hours, referred to specified equator and equinox of date.</param>
        /// <param name="Dec">Declination in degrees, referred to specified equator and equinox of date.</param>
        /// <param name="ELon">Ecliptic longitude in degrees, referred to specified ecliptic and equinox of date.</param>
        /// <param name="ELat">Ecliptic latitude in degrees, referred to specified ecliptic and equinox of date.</param>
        /// <returns><pre>
        /// 0 ... everything OK
        /// 1 ... invalid value of 'CoordSys'
        /// </pre></returns>
        /// <remarks>
        /// To convert ICRS RA and dec to ecliptic coordinates (mean ecliptic and equinox of J2000.0), 
        /// set 'CoordSys' = 2; the value of 'JdTt' can be set to anything, since J2000.0 is assumed. 
        /// Except for the input to this case, all input coordinates are dynamical.
        /// </remarks>
        public static short Equ2Ecl(double JdTt, CoordSys CoordSys, Accuracy Accuracy, double Ra, double Dec, ref double ELon, ref double ELat)
        {

            // Initialise if necessary
            if (!isInitialised) Initialise();

            return Equ2EclLib(JdTt, CoordSys, Accuracy, Ra, Dec, ref ELon, ref ELat);
        }

        /// <summary>
        /// Converts an equatorial position vector to an ecliptic position vector.
        /// </summary>
        /// <param name="JdTt">TT Julian date of equator, equinox, and ecliptic used for</param>
        /// <param name="CoordSys"> Coordinate system selection. 0 ... mean equator and equinox of date 'JdTt'; 1 ... true equator and equinox of date 'JdTt'; 2 ... ICRS</param>
        /// <param name="Accuracy">Selection for accuracy</param>
        /// <param name="Pos1">Position vector, referred to specified equator and equinox of date.</param>
        /// <param name="Pos2">Position vector, referred to specified ecliptic and equinox of date.</param>
        /// <returns><pre>
        /// 0 ... everything OK
        /// 1 ... invalid value of 'CoordSys'
        /// </pre></returns>
        /// <remarks>To convert an ICRS vector to an ecliptic vector (mean ecliptic and equinox of J2000.0 only), 
        /// set 'CoordSys' = 2; the value of 'JdTt' can be set to anything, since J2000.0 is assumed. Except for 
        /// the input to this case, all vectors are assumed to be with respect to a dynamical system.</remarks>
        public static short Equ2EclVec(double JdTt, CoordSys CoordSys, Accuracy Accuracy, double[] Pos1, ref double[] Pos2)
        {
            var VPos2 = new PosVector();
            short rc;
            var argPos11 = ArrToPosVec(Pos1);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            rc = Equ2EclVecLib(JdTt, CoordSys, Accuracy, ref argPos11, ref VPos2);

            PosVecToArr(VPos2, ref Pos2);
            return rc;
        }

        /// <summary>
        /// Converts ICRS right ascension and declination to galactic longitude and latitude.
        /// </summary>
        /// <param name="RaI">ICRS right ascension in hours.</param>
        /// <param name="DecI">ICRS declination in degrees.</param>
        /// <param name="GLon">Galactic longitude in degrees.</param>
        /// <param name="GLat">Galactic latitude in degrees.</param>
        /// <remarks></remarks>
        public static void Equ2Gal(double RaI, double DecI, ref double GLon, ref double GLat)
        {

            // Initialise if necessary
            if (!isInitialised) Initialise();

            Equ2GalLib(RaI, DecI, ref GLon, ref GLat);
        }

        /// <summary>
        /// Transforms topocentric right ascension and declination to zenith distance and azimuth.  
        /// </summary>
        /// <param name="Jd_Ut1">UT1 Julian date.</param>
        /// <param name="DeltT">Difference TT-UT1 at 'jd_ut1', in seconds.</param>
        /// <param name="Accuracy">Selection for accuracy</param>
        /// <param name="xp">onventionally-defined x coordinate of celestial intermediate pole with respect to ITRS reference pole, in arcseconds.</param>
        /// <param name="yp">Conventionally-defined y coordinate of celestial intermediate pole with respect to ITRS reference pole, in arcseconds.</param>
        /// <param name="Location">Structure containing observer's location </param>
        /// <param name="Ra">Topocentric right ascension of object of interest, in hours, referred to true equator and equinox of date.</param>
        /// <param name="Dec">Topocentric declination of object of interest, in degrees, referred to true equator and equinox of date.</param>
        /// <param name="RefOption">Refraction option. 0 ... no refraction; 1 ... include refraction, using 'standard' atmospheric conditions;
        /// 2 ... include refraction, using atmospheric parametersinput in the 'Location' structure.</param>
        /// <param name="Zd">Topocentric zenith distance in degrees, affected by refraction if 'ref_option' is non-zero.</param>
        /// <param name="Az">Topocentric azimuth (measured east from north) in degrees.</param>
        /// <param name="RaR"> Topocentric right ascension of object of interest, in hours, referred to true equator and 
        /// equinox of date, affected by refraction if 'ref_option' is non-zero.</param>
        /// <param name="DecR">Topocentric declination of object of interest, in degrees, referred to true equator and 
        /// equinox of date, affected by refraction if 'ref_option' is non-zero.</param>
        /// <remarks>This function transforms topocentric right ascension and declination to zenith distance and azimuth.  
        /// It uses a method that properly accounts for polar motion, which is significant at the sub-arcsecond level.  
        /// This function can also adjust coordinates for atmospheric refraction.</remarks>
        public static void Equ2Hor(double Jd_Ut1, double DeltT, Accuracy Accuracy, double xp, double yp, OnSurface Location, double Ra, double Dec, RefractionOption RefOption, ref double Zd, ref double Az, ref double RaR, ref double DecR)
        {

            // Initialise if necessary
            if (!isInitialised) Initialise();

            try
            {

                LogMessage("Equ2Hor", "JD Accuracy RA DEC:     " + Jd_Ut1 + " " + Accuracy.ToString() + " " + Utilities.HoursToHMS(Ra, ":", ":", "", 3) + " " + Utilities.DegreesToDMS(Dec, ":", ":", "", 3));
                LogMessage("Equ2Hor", "  DeltaT:               " + DeltT);
                LogMessage("Equ2Hor", "  xp:                   " + xp);
                LogMessage("Equ2Hor", "  yp:                   " + yp);
                LogMessage("Equ2Hor", "  Refraction:           " + RefOption.ToString());
                LogMessage("Equ2Hor", "  Location.Height:      " + Location.Height);
                LogMessage("Equ2Hor", "  Location.Latitude:    " + Location.Latitude);
                LogMessage("Equ2Hor", "  Location.Longitude:   " + Location.Longitude);
                LogMessage("Equ2Hor", "  Location.Pressure:    " + Location.Pressure);
                LogMessage("Equ2Hor", "  Location.Temperature: " + Location.Temperature);
            }
            catch (Exception ex)
            {
                LogMessage("Equ2Hor", "Exception: " + ex.ToString());
            }

            Equ2HorLib(Jd_Ut1, DeltT, Accuracy, xp, yp, ref Location, Ra, Dec, RefOption, ref Zd, ref Az, ref RaR, ref DecR);
            LogMessage("Equ2Hor", $"  RA Dec: {Utilities.HoursToHMS(RaR, ":", ":", "", 3)} {Utilities.DegreesToDMS(DecR, ":", ":", "", 3)}, Az El: {Utilities.DegreesToDMS(Az, ":", ":", "", 3)} {Utilities.DegreesToDMS(90.0 - Zd, ":", ":", "", 3)}");

        }

        /// <summary>
        /// Returns the value of the Earth Rotation Angle (theta) for a given UT1 Julian date. 
        /// </summary>
        /// <param name="JdHigh">High-order part of UT1 Julian date.</param>
        /// <param name="JdLow">Low-order part of UT1 Julian date.</param>
        /// <returns>The Earth Rotation Angle (theta) in degrees.</returns>
        /// <remarks> The expression used is taken from the note to IAU Resolution B1.8 of 2000.  1. The algorithm used 
        /// here is equivalent to the canonical theta = 0.7790572732640 + 1.00273781191135448 * t, where t is the time 
        /// in days from J2000 (t = JdHigh + JdLow - T0), but it avoids many two-PI 'wraps' that 
        /// decrease precision (adopted from SOFA Fortran routine iau_era00; see also expression at top 
        /// of page 35 of IERS Conventions (1996)).</remarks>
        public static double Era(double JdHigh, double JdLow)
        {

            // Initialise if necessary
            if (!isInitialised) Initialise();

            return EraLib(JdHigh, JdLow);
        }

        /// <summary>
        /// Computes quantities related to the orientation of the Earth's rotation axis at Julian date 'JdTdb'.
        /// </summary>
        /// <param name="JdTdb">TDB Julian Date.</param>
        /// <param name="Accuracy">Selection for accuracy</param>
        /// <param name="Mobl">Mean obliquity of the ecliptic in degrees at 'JdTdb'.</param>
        /// <param name="Tobl">True obliquity of the ecliptic in degrees at 'JdTdb'.</param>
        /// <param name="Ee">Equation of the equinoxes in seconds of time at 'JdTdb'.</param>
        /// <param name="Dpsi">Nutation in longitude in arcseconds at 'JdTdb'.</param>
        /// <param name="Deps">Nutation in obliquity in arcseconds at 'JdTdb'.</param>
        /// <remarks>Values of the celestial pole offsets 'PSI_COR' and 'EPS_COR' are set using function 'cel_pole', 
        /// if desired.  See the prolog of 'cel_pole' for details.</remarks>
        public static void ETilt(double JdTdb, Accuracy Accuracy, ref double Mobl, ref double Tobl, ref double Ee, ref double Dpsi, ref double Deps)
        {

            // Initialise if necessary
            if (!isInitialised) Initialise();

            ETiltLib(JdTdb, Accuracy, ref Mobl, ref Tobl, ref Ee, ref Dpsi, ref Deps);
        }

        /// <summary>
        /// To transform a vector from the dynamical reference system to the International Celestial Reference System (ICRS), or vice versa.
        /// </summary>
        /// <param name="Pos1">Position vector, equatorial rectangular coordinates.</param>
        /// <param name="Direction">Set 'direction' <![CDATA[<]]> 0 for dynamical to ICRS transformation. Set 'direction' <![CDATA[>=]]> 0 for 
        /// ICRS to dynamical transformation.</param>
        /// <param name="Pos2">Position vector, equatorial rectangular coordinates.</param>
        /// <remarks></remarks>
        public static void FrameTie(double[] Pos1, FrameConversionDirection Direction, ref double[] Pos2)
        {
            var VPos2 = new PosVector();

            var argPos11 = ArrToPosVec(Pos1);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            FrameTieLib(ref argPos11, Direction, ref VPos2);
            PosVecToArr(VPos2, ref Pos2);
        }

        /// <summary>
        /// To compute the fundamental arguments (mean elements) of the Sun and Moon.
        /// </summary>
        /// <param name="t">TDB time in Julian centuries since J2000.0</param>
        /// <param name="a">Double array of fundamental arguments</param>
        /// <remarks>
        /// Fundamental arguments, in radians:
        /// <pre>
        ///   a[0] = l (mean anomaly of the Moon)
        ///   a[1] = l' (mean anomaly of the Sun)
        ///   a[2] = F (mean argument of the latitude of the Moon)
        ///   a[3] = D (mean elongation of the Moon from the Sun)
        ///   a[4] = a[4] (mean longitude of the Moon's ascending node);
        ///                from Simon section 3.4(b.3),
        ///                precession = 5028.8200 arcsec/cy)
        /// </pre>
        /// </remarks>
        public static void FundArgs(double t, ref double[] a)
        {
            var va = new FundamentalArgs();

            // Initialise if necessary
            if (!isInitialised) Initialise();

            FundArgsLib(t, ref va);

            a[0] = va.l;
            a[1] = va.ldash;
            a[2] = va.F;
            a[3] = va.D;
            a[4] = va.Omega;
        }

        /// <summary>
        /// Converts GCRS right ascension and declination to coordinates with respect to the equator of date (mean or true).
        /// </summary>
        /// <param name="JdTt">TT Julian date of equator to be used for output coordinates.</param>
        /// <param name="CoordSys"> Coordinate system selection for output coordinates.; 0 ... mean equator and 
        /// equinox of date; 1 ... true equator and equinox of date; 2 ... true equator and CIO of date</param>
        /// <param name="Accuracy">Selection for accuracy</param>
        /// <param name="RaG">GCRS right ascension in hours.</param>
        /// <param name="DecG">GCRS declination in degrees.</param>
        /// <param name="Ra"> Right ascension in hours, referred to specified equator and right ascension origin of date.</param>
        /// <param name="Dec">Declination in degrees, referred to specified equator of date.</param>
        /// <returns>
        /// <pre>
        ///    0 ... everything OK
        /// >  0 ... error from function 'Vector2RaDec'' 
        /// > 10 ... 10 + error from function 'CioLocation'
        /// > 20 ... 20 + error from function 'CioBasis'
        /// </pre>></returns>
        /// <remarks>For coordinates with respect to the true equator of date, the origin of right ascension can be either the true equinox or the celestial intermediate origin (CIO).
        /// <para> This function only supports the CIO-based method.</para></remarks>
        public static short Gcrs2Equ(double JdTt, CoordSys CoordSys, Accuracy Accuracy, double RaG, double DecG, ref double Ra, ref double Dec)
        {

            // Initialise if necessary
            if (!isInitialised) Initialise();

            return Gcrs2EquLib(JdTt, CoordSys, Accuracy, RaG, DecG, ref Ra, ref Dec);
        }

        /// <summary>
        /// This function computes the geocentric position and velocity of an observer on 
        /// the surface of the earth or on a near-earth spacecraft.</summary>
        /// <param name="JdTt">TT Julian date.</param>
        /// <param name="DeltaT">Value of Delta T (= TT - UT1) at 'JdTt'.</param>
        /// <param name="Accuracy">Selection for accuracy</param>
        /// <param name="Obs">Data specifying the location of the observer</param>
        /// <param name="Pos">Position vector of observer, with respect to origin at geocenter, 
        /// referred to GCRS axes, components in AU.</param>
        /// <param name="Vel">Velocity vector of observer, with respect to origin at geocenter, 
        /// referred to GCRS axes, components in AU/day.</param>
        /// <returns>
        /// <pre>
        /// 0 ... everything OK
        /// 1 ... invalid value of 'Accuracy'.
        /// </pre></returns>
        /// <remarks>The final vectors are expressed in the GCRS.</remarks>
        public static short GeoPosVel(double JdTt, double DeltaT, Accuracy Accuracy, Observer Obs, ref double[] Pos, ref double[] Vel)
        {
            var VPos = new PosVector();
            var VVel = new VelVector();
            short rc;

            // Initialise if necessary
            if (!isInitialised) Initialise();

            rc = GeoPosVelLib(JdTt, DeltaT, Accuracy, ref Obs, ref VPos, ref VVel);

            PosVecToArr(VPos, ref Pos);
            VelVecToArr(VVel, ref Vel);
            return rc;
        }

        /// <summary>
        /// Computes the total gravitational deflection of light for the observed object due to the major gravitating bodies in the solar system.
        /// </summary>
        /// <param name="JdTdb">TDB Julian date of observation.</param>
        /// <param name="LocCode">Code for location of observer, determining whether the gravitational deflection due to the earth itself is applied.</param>
        /// <param name="Accuracy">Selection for accuracy</param>
        /// <param name="Pos1"> Position vector of observed object, with respect to origin at observer (or the geocenter), 
        /// referred to ICRS axes, components in AU.</param>
        /// <param name="PosObs">Position vector of observer (or the geocenter), with respect to origin at solar 
        /// system barycenter, referred to ICRS axes, components in AU.</param>
        /// <param name="Pos2">Position vector of observed object, with respect to origin at observer (or the geocenter), 
        /// referred to ICRS axes, corrected for gravitational deflection, components in AU.</param>
        /// <returns><pre>
        ///    0 ... Everything OK
        /// <![CDATA[<]]> 30 ... Error from function 'Ephemeris'; 
        /// > 30 ... Error from function 'MakeObject'.
        /// </pre></returns>
        /// <remarks>This function valid for an observed body within the solar system as well as for a star.
        /// <para>
        /// If 'Accuracy' is set to zero (full accuracy), three bodies (Sun, Jupiter, and Saturn) are 
        /// used in the calculation.  If the reduced-accuracy option is set, only the Sun is used in the 
        /// calculation.  In both cases, if the observer is not at the geocenter, the deflection due to the Earth is included.
        /// </para>
        /// </remarks>
        public static short GravDef(double JdTdb, EarthDeflection LocCode, Accuracy Accuracy, double[] Pos1, double[] PosObs, ref double[] Pos2)
        {
            var VPos2 = new PosVector();
            short rc;

            var argPos11 = ArrToPosVec(Pos1);
            var argPosObs1 = ArrToPosVec(PosObs);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            rc = GravDefLib(JdTdb, LocCode, Accuracy, ref argPos11, ref argPosObs1, ref VPos2);

            PosVecToArr(VPos2, ref Pos2);
            return rc;
        }

        /// <summary>
        /// Corrects position vector for the deflection of light in the gravitational field of an arbitrary body.
        /// </summary>
        /// <param name="Pos1">Position vector of observed object, with respect to origin at observer 
        /// (or the geocenter), components in AU.</param>
        /// <param name="PosObs">Position vector of observer (or the geocenter), with respect to origin at 
        /// solar system barycenter, components in AU.</param>
        /// <param name="PosBody">Position vector of gravitating body, with respect to origin at solar system 
        /// barycenter, components in AU.</param>
        /// <param name="RMass">Reciprocal mass of gravitating body in solar mass units, that is, 
        /// Sun mass / body mass.</param>
        /// <param name="Pos2">Position vector of observed object, with respect to origin at observer 
        /// (or the geocenter), corrected for gravitational deflection, components in AU.</param>
        /// <remarks>This function valid for an observed body within the solar system as well as for a star.</remarks>
        public static void GravVec(double[] Pos1, double[] PosObs, double[] PosBody, double RMass, ref double[] Pos2)
        {
            var VPos2 = new PosVector();

            var argPos11 = ArrToPosVec(Pos1);
            var argPosObs1 = ArrToPosVec(PosObs);
            var argPosBody1 = ArrToPosVec(PosBody);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            GravVecLib(ref argPos11, ref argPosObs1, ref argPosBody1, RMass, ref VPos2);

            PosVecToArr(VPos2, ref Pos2);
        }

        /// <summary>
        /// Compute the intermediate right ascension of the equinox at the input Julian date
        /// </summary>
        /// <param name="JdTdb">TDB Julian date.</param>
        /// <param name="Equinox">Equinox selection flag: mean pr true</param>
        /// <param name="Accuracy">Selection for accuracy</param>
        /// <returns>Intermediate right ascension of the equinox, in hours (+ or -). If 'equinox' = 1 
        /// (i.e true equinox), then the returned value is the equation of the origins.</returns>
        /// <remarks></remarks>
        public static double IraEquinox(double JdTdb, EquinoxType Equinox, Accuracy Accuracy)
        {

            // Initialise if necessary
            if (!isInitialised) Initialise();

            return IraEquinoxLib(JdTdb, Equinox, Accuracy);
        }

        /// <summary>
        /// Compute the Julian date for a given calendar date (year, month, day, hour).
        /// </summary>
        /// <param name="Year">Year number</param>
        /// <param name="Month">Month number</param>
        /// <param name="Day">Day number</param>
        /// <param name="Hour">Fractional hour of the day</param>
        /// <returns>Computed Julian date.</returns>
        /// <remarks>This function makes no checks for a valid input calendar date. The input calendar date 
        /// must be Gregorian. The input time value can be based on any UT-like time scale (UTC, UT1, TT, etc.) 
        /// - output Julian date will have the same basis.</remarks>
        public static double JulianDate(short Year, short Month, short Day, double Hour)
        {

            // Initialise if necessary
            if (!isInitialised) Initialise();

            return JulianDateLib(Year, Month, Day, Hour);
        }

        /// <summary>
        /// Computes the geocentric position of a solar system body, as antedated for light-time.
        /// </summary>
        /// <param name="JdTdb">TDB Julian date of observation.</param>
        /// <param name="SsObject">Structure containing the designation for thesolar system body</param>
        /// <param name="PosObs">Position vector of observer (or the geocenter), with respect to origin 
        /// at solar system barycenter, referred to ICRS axes, components in AU.</param>
        /// <param name="TLight0">First approximation to light-time, in days (can be set to 0.0 if unknown)</param>
        /// <param name="Accuracy">Selection for accuracy</param>
        /// <param name="Pos">Position vector of body, with respect to origin at observer (or the geocenter), 
        /// referred to ICRS axes, components in AU.</param>
        /// <param name="TLight">Final light-time, in days.</param>
        /// <returns><pre>
        ///    0 ... everything OK
        ///    1 ... algorithm failed to converge after 10 iterations
        /// <![CDATA[>]]> 10 ... error is 10 + error from function 'SolarSystem'.
        /// </pre></returns>
        /// <remarks></remarks>
        public static short LightTime(double JdTdb, Object3 SsObject, double[] PosObs, double TLight0, Accuracy Accuracy, ref double[] Pos, ref double TLight)
        {
            var VPos = new PosVector();
            short rc;
            var argSsObject1 = O3IFromObject3(SsObject);
            var argPosObs1 = ArrToPosVec(PosObs);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            rc = LightTimeLib(JdTdb, ref argSsObject1, ref argPosObs1, TLight0, Accuracy, ref VPos, ref TLight);

            PosVecToArr(VPos, ref Pos);
            return rc;
        }

        /// <summary>
        /// Determines the angle of an object above or below the Earth's limb (horizon).
        /// </summary>
        /// <param name="PosObj">Position vector of observed object, with respect to origin at 
        /// geocenter, components in AU.</param>
        /// <param name="PosObs">Position vector of observer, with respect to origin at geocenter, 
        /// components in AU.</param>
        /// <param name="LimbAng">Angle of observed object above (+) or below (-) limb in degrees.</param>
        /// <param name="NadirAng">Nadir angle of observed object as a fraction of apparent radius of limb: <![CDATA[<]]> 1.0 ... 
        /// below the limb; = 1.0 ... on the limb;  <![CDATA[>]]> 1.0 ... above the limb</param>
        /// <remarks>The geometric limb is computed, assuming the Earth to be an airless sphere (no 
        /// refraction or oblateness is included).  The observer can be on or above the Earth.  
        /// For an observer on the surface of the Earth, this function returns the approximate unrefracted 
        /// altitude.</remarks>
        public static void LimbAngle(double[] PosObj, double[] PosObs, ref double LimbAng, ref double NadirAng)
        {
            var argPosObj1 = ArrToPosVec(PosObj);
            var argPosObs1 = ArrToPosVec(PosObs);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            LimbAngleLib(ref argPosObj1, ref argPosObs1, ref LimbAng, ref NadirAng);
        }

        /// <summary>
        /// Computes the local place of a solar system body.
        /// </summary>
        /// <param name="JdTt">TT Julian date for local place.</param>
        /// <param name="SsBody">structure containing the body designation for the solar system body</param>
        /// <param name="DeltaT">Difference TT-UT1 at 'JdTt', in seconds of time.</param>
        /// <param name="Position">Specifies the position of the observer</param>
        /// <param name="Accuracy">Specifies accuracy level</param>
        /// <param name="Ra">Local right ascension in hours, referred to the 'local GCRS'.</param>
        /// <param name="Dec">Local declination in degrees, referred to the 'local GCRS'.</param>
        /// <param name="Dis">True distance from Earth to planet in AU.</param>
        /// <returns><pre>
        ///    0 ... Everything OK
        ///    1 ... Invalid value of 'Where' in structure 'Location'; 
        /// <![CDATA[>]]> 10 ... Error code from function 'Place'.
        /// </pre></returns>
        /// <remarks></remarks>
        public static short LocalPlanet(double JdTt, Object3 SsBody, double DeltaT, OnSurface Position, Accuracy Accuracy, ref double Ra, ref double Dec, ref double Dis)
        {
            var argSsBody1 = O3IFromObject3(SsBody);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            return LocalPlanetLib(JdTt, ref argSsBody1, DeltaT, ref Position, Accuracy, ref Ra, ref Dec, ref Dis);
        }

        /// <summary>
        /// Computes the local place of a star at date 'JdTt', given its catalog mean place, proper motion, parallax, and radial velocity.
        /// </summary>
        /// <param name="JdTt">TT Julian date for local place. delta_t (double)</param>
        /// <param name="DeltaT">Difference TT-UT1 at 'JdTt', in seconds of time.</param>
        /// <param name="Star">catalog entry structure containing catalog data for the object in the ICRS</param>
        /// <param name="Position">Structure specifying the position of the observer </param>
        /// <param name="Accuracy">Specifies accuracy level.</param>
        /// <param name="Ra">Local right ascension in hours, referred to the 'local GCRS'.</param>
        /// <param name="Dec">Local declination in degrees, referred to the 'local GCRS'.</param>
        /// <returns><pre>
        ///    0 ... Everything OK
        ///    1 ... Invalid value of 'Where' in structure 'Location'
        /// > 10 ... Error code from function 'MakeObject'
        /// > 20 ... Error code from function 'Place'.
        /// </pre></returns>
        /// <remarks></remarks>
        public static short LocalStar(double JdTt, double DeltaT, CatEntry3 Star, OnSurface Position, Accuracy Accuracy, ref double Ra, ref double Dec)
        {
            // Initialise if necessary
            if (!isInitialised) Initialise();

            return LocalStarLib(JdTt, DeltaT, ref Star, ref Position, Accuracy, ref Ra, ref Dec);
        }

        /// <summary>
        /// Create a structure of type 'cat_entry' containing catalog data for a star or "star-like" object.
        /// </summary>
        /// <param name="StarName">Object name (50 characters maximum).</param>
        /// <param name="Catalog">Three-character catalog identifier (e.g. HIP = Hipparcos, TY2 = Tycho-2)</param>
        /// <param name="StarNum">Object number in the catalog.</param>
        /// <param name="Ra">Right ascension of the object (hours).</param>
        /// <param name="Dec">Declination of the object (degrees).</param>
        /// <param name="PmRa">Proper motion in right ascension (milliarcseconds/year).</param>
        /// <param name="PmDec">Proper motion in declination (milliarcseconds/year).</param>
        /// <param name="Parallax">Parallax (milliarcseconds).</param>
        /// <param name="RadVel">Radial velocity (kilometers/second).</param>
        /// <param name="Star">CatEntry3 structure containing the input data</param>
        /// <remarks></remarks>
        public static void MakeCatEntry(string StarName, string Catalog, int StarNum, double Ra, double Dec, double PmRa, double PmDec, double Parallax, double RadVel, ref CatEntry3 Star)
        {
            // Initialise if necessary
            if (!isInitialised) Initialise();

            MakeCatEntryLib(StarName, Catalog, StarNum, Ra, Dec, PmRa, PmDec, Parallax, RadVel, ref Star);
        }

        /// <summary>
        /// Makes a structure of type 'InSpace' - specifying the position and velocity of an observer situated 
        /// on a near-Earth spacecraft.
        /// </summary>
        /// <param name="ScPos">Geocentric position vector (x, y, z) in km.</param>
        /// <param name="ScVel">Geocentric velocity vector (x_dot, y_dot, z_dot) in km/s.</param>
        /// <param name="ObsSpace">InSpace structure containing the position and velocity of an observer situated 
        /// on a near-Earth spacecraft</param>
        /// <remarks></remarks>
        public static void MakeInSpace(double[] ScPos, double[] ScVel, ref InSpace ObsSpace)
        {
            var argScPos1 = ArrToPosVec(ScPos);
            var argScVel1 = ArrToVelVec(ScVel);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            MakeInSpaceLib(ref argScPos1, ref argScVel1, ref ObsSpace);
        }

        /// <summary>
        /// Makes a structure of type 'object' - specifying a celestial object - based on the input parameters.
        /// </summary>
        /// <param name="Type">Type of object: 0 ... major planet, Sun, or Moon;  1 ... minor planet; 
        /// 2 ... object located outside the solar system (e.g. star, galaxy, nebula, etc.)</param>
        /// <param name="Number">Body number: For 'Type' = 0: Mercury = 1,...,Pluto = 9, Sun = 10, Moon = 11; 
        /// For 'Type' = 1: minor planet numberFor 'Type' = 2: set to 0 (zero)</param>
        /// <param name="Name">Name of the object (50 characters maximum).</param>
        /// <param name="StarData">Structure containing basic astrometric data for any celestial object 
        /// located outside the solar system; the catalog data for a star</param>
        /// <param name="CelObj">Structure containing the object definition</param>
        /// <returns><pre>
        /// 0 ... everything OK
        /// 1 ... invalid value of 'Type'
        /// 2 ... 'Number' out of range
        /// </pre></returns>
        /// <remarks></remarks>
        public static short MakeObject(ObjectType Type, short Number, string Name, CatEntry3 StarData, ref Object3 CelObj)
        {
            var O3I = new Object3Internal();
            short rc;

            // Initialise if necessary
            if (!isInitialised) Initialise();

            rc = MakeObjectLib(Type, Number, Name, ref StarData, ref O3I);
            O3FromO3Internal(O3I, ref CelObj);
            return rc;
        }

        /// <summary>
        /// Makes a structure of type 'observer' - specifying the location of the observer.
        /// </summary>
        /// <param name="Where">Integer code specifying location of observer: 0: observer at geocenter; 
        /// 1: observer on surface of earth; 2: observer on near-earth spacecraft</param>
        /// <param name="ObsSurface">Structure containing data for an observer's location on the surface 
        /// of the Earth; used when 'Where' = 1</param>
        /// <param name="ObsSpace"> Structure containing an observer's location on a near-Earth spacecraft; 
        /// used when 'Where' = 2 </param>
        /// <param name="Obs">Structure specifying the location of the observer </param>
        /// <returns><pre>
        /// 0 ... everything OK
        /// 1 ... input value of 'Where' is out-of-range.
        /// </pre></returns>
        /// <remarks></remarks>
        public static short MakeObserver(ObserverLocation Where, OnSurface ObsSurface, InSpace ObsSpace, ref Observer Obs)
        {
            // Initialise if necessary
            if (!isInitialised) Initialise();

            return MakeObserverLib(Where, ref ObsSurface, ref ObsSpace, ref Obs);
        }

        /// <summary>
        /// Makes a structure of type 'observer' specifying an observer at the geocenter.
        /// </summary>
        /// <param name="ObsAtGeocenter">Structure specifying the location of the observer at the geocenter</param>
        /// <remarks></remarks>
        public static void MakeObserverAtGeocenter(ref Observer ObsAtGeocenter)
        {
            // Initialise if necessary
            if (!isInitialised) Initialise();

            MakeObserverAtGeocenterLib(ref ObsAtGeocenter);
        }

        /// <summary>
        /// Makes a structure of type 'observer' specifying the position and velocity of an observer 
        /// situated on a near-Earth spacecraft.
        /// </summary>
        /// <param name="ScPos">Geocentric position vector (x, y, z) in km.</param>
        /// <param name="ScVel">Geocentric position vector (x, y, z) in km.</param>
        /// <param name="ObsInSpace">Structure containing the position and velocity of an observer 
        /// situated on a near-Earth spacecraft</param>
        /// <remarks>Both input vectors are with respect to true equator and equinox of date.</remarks>
        public static void MakeObserverInSpace(double[] ScPos, double[] ScVel, ref Observer ObsInSpace)
        {
            var argScPos1 = ArrToPosVec(ScPos);
            var argScVel1 = ArrToVelVec(ScVel);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            MakeObserverInSpaceLib(ref argScPos1, ref argScVel1, ref ObsInSpace);
        }

        /// <summary>
        /// Makes a structure of type 'observer' specifying the location of and weather for an observer 
        /// on the surface of the Earth.
        /// </summary>
        /// <param name="Latitude">Geodetic (ITRS) latitude in degrees; north positive.</param>
        /// <param name="Longitude">Geodetic (ITRS) longitude in degrees; east positive.</param>
        /// <param name="Height">Height of the observer (meters).</param>
        /// <param name="Temperature">Temperature (degrees Celsius).</param>
        /// <param name="Pressure">Atmospheric pressure (millibars).</param>
        /// <param name="ObsOnSurface">Structure containing the location of and weather for an observer on 
        /// the surface of the Earth</param>
        /// <remarks></remarks>
        public static void MakeObserverOnSurface(double Latitude, double Longitude, double Height, double Temperature, double Pressure, ref Observer ObsOnSurface)
        {
            // Initialise if necessary
            if (!isInitialised) Initialise();

            MakeObserverOnSurfaceLib(Latitude, Longitude, Height, Temperature, Pressure, ref ObsOnSurface);
        }

        /// <summary>
        /// Makes a structure of type 'on_surface' - specifying the location of and weather for an 
        /// observer on the surface of the Earth.
        /// </summary>
        /// <param name="Latitude">Geodetic (ITRS) latitude in degrees; north positive.</param>
        /// <param name="Longitude">Geodetic (ITRS) latitude in degrees; north positive.</param>
        /// <param name="Height">Height of the observer (meters).</param>
        /// <param name="Temperature">Temperature (degrees Celsius).</param>
        /// <param name="Pressure">Atmospheric pressure (millibars).</param>
        /// <param name="ObsSurface">Structure containing the location of and weather for an 
        /// observer on the surface of the Earth.</param>
        /// <remarks></remarks>
        public static void MakeOnSurface(double Latitude, double Longitude, double Height, double Temperature, double Pressure, ref OnSurface ObsSurface)
        {
            // Initialise if necessary
            if (!isInitialised) Initialise();

            MakeOnSurfaceLib(Latitude, Longitude, Height, Temperature, Pressure, ref ObsSurface);
        }

        /// <summary>
        /// Compute the mean obliquity of the ecliptic.
        /// </summary>
        /// <param name="JdTdb">TDB Julian Date.</param>
        /// <returns>Mean obliquity of the ecliptic in arcseconds.</returns>
        /// <remarks></remarks>
        public static double MeanObliq(double JdTdb)
        {

            // Initialise if necessary
            if (!isInitialised) Initialise();

            return MeanObliqLib(JdTdb);
        }

        /// <summary>
        /// Computes the ICRS position of a star, given its apparent place at date 'JdTt'.  
        /// Proper motion, parallax and radial velocity are assumed to be zero.
        /// </summary>
        /// <param name="JdTt">TT Julian date of apparent place.</param>
        /// <param name="Ra">Apparent right ascension in hours, referred to true equator and equinox of date.</param>
        /// <param name="Dec">Apparent declination in degrees, referred to true equator and equinox of date.</param>
        /// <param name="Accuracy">Specifies accuracy level</param>
        /// <param name="IRa">ICRS right ascension in hours.</param>
        /// <param name="IDec">ICRS declination in degrees.</param>
        /// <returns><pre>
        ///    0 ... Everything OK
        ///    1 ... Iterative process did not converge after 30 iterations; 
        /// > 10 ... Error from function 'Vector2RaDec'
        /// > 20 ... Error from function 'AppStar'.
        /// </pre></returns>
        /// <remarks></remarks>
        public static short MeanStar(double JdTt, double Ra, double Dec, Accuracy Accuracy, ref double IRa, ref double IDec)
        {
            // Initialise if necessary
            if (!isInitialised) Initialise();

            return MeanStarLib(JdTt, Ra, Dec, Accuracy, ref IRa, ref IDec);
        }

        /// <summary>
        /// Normalize angle into the range 0 <![CDATA[<=]]> angle <![CDATA[<]]> (2 * pi).
        /// </summary>
        /// <param name="Angle">Input angle (radians).</param>
        /// <returns>The input angle, normalized as described above (radians).</returns>
        /// <remarks></remarks>
        public static double NormAng(double Angle)
        {
            // Initialise if necessary
            if (!isInitialised) Initialise();

            return NormAngLib(Angle);
        }

        /// <summary>
        /// Nutates equatorial rectangular coordinates from mean equator and equinox of epoch to true equator and equinox of epoch.
        /// </summary>
        /// <param name="JdTdb">TDB Julian date of epoch.</param>
        /// <param name="Direction">Flag determining 'direction' of transformation; direction  = 0 
        /// transformation applied, mean to true; direction != 0 inverse transformation applied, true to mean.</param>
        /// <param name="Accuracy">Selection for accuracy</param>
        /// <param name="Pos">Position vector, geocentric equatorial rectangular coordinates, referred to 
        /// mean equator and equinox of epoch.</param>
        /// <param name="Pos2">Position vector, geocentric equatorial rectangular coordinates, referred to 
        /// true equator and equinox of epoch.</param>
        /// <remarks> Inverse transformation may be applied by setting flag 'direction'</remarks>
        public static void Nutation(double JdTdb, NutationDirection Direction, Accuracy Accuracy, double[] Pos, ref double[] Pos2)
        {
            var VPOs2 = new PosVector();

            var argPos1 = ArrToPosVec(Pos);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            NutationLib(JdTdb, Direction, Accuracy, ref argPos1, ref VPOs2);
            PosVecToArr(VPOs2, ref Pos2);
        }

        /// <summary>
        /// Returns the values for nutation in longitude and nutation in obliquity for a given TDB Julian date.
        /// </summary>
        /// <param name="t">TDB time in Julian centuries since J2000.0</param>
        /// <param name="Accuracy">Selection for accuracy</param>
        /// <param name="DPsi">Nutation in longitude in arcseconds.</param>
        /// <param name="DEps">Nutation in obliquity in arcseconds.</param>
        /// <remarks>The nutation model selected depends upon the input value of 'Accuracy'.  See notes below for important details.
        /// <para>
        /// This function selects the nutation model depending first upon the input value of 'Accuracy'.  
        /// If 'Accuracy' = 0 (full accuracy), the IAU 2000A nutation model is used.  If 'Accuracy' = 1 
        /// a specially truncated (and therefore faster) version of IAU 2000A, called 'NU2000K' is used.
        /// </para>
        /// </remarks>
        public static void NutationAngles(double t, Accuracy Accuracy, ref double DPsi, ref double DEps)
        {
            // Initialise if necessary
            if (!isInitialised) Initialise();

            NutationAnglesLib(t, Accuracy, ref DPsi, ref DEps);
        }

        /// <summary>
        /// Computes the apparent direction of a star or solar system body at a specified time 
        /// and in a specified coordinate system.
        /// </summary>
        /// <param name="JdTt">TT Julian date for place.</param>
        /// <param name="CelObject"> Specifies the celestial object of interest</param>
        /// <param name="Location">Specifies the location of the observer</param>
        /// <param name="DeltaT"> Difference TT-UT1 at 'JdTt', in seconds of time.</param>
        /// <param name="CoordSys">Code specifying coordinate system of the output position. 0 ... GCRS or 
        /// "local GCRS"; 1 ... true equator and equinox of date; 2 ... true equator and CIO of date; 
        /// 3 ... astrometric coordinates, i.e., without light deflection or aberration.</param>
        /// <param name="Accuracy">Selection for accuracy</param>
        /// <param name="Output">Structure specifying object's place on the sky at time 'JdTt', 
        /// with respect to the specified output coordinate system</param>
        /// <returns>
        /// <pre>
        /// = 0         ... No problems.
        /// = 1         ... invalid value of 'CoordSys'
        /// = 2         ... invalid value of 'Accuracy'
        /// = 3         ... Earth is the observed object, and the observer is either at the geocenter or on the Earth's surface (not permitted)
        /// > 10, <![CDATA[<]]> 40  ... 10 + error from function 'Ephemeris'
        /// > 40, <![CDATA[<]]> 50  ... 40 + error from function 'GeoPosVel'
        /// > 50, <![CDATA[<]]> 70  ... 50 + error from function 'LightTime'
        /// > 70, <![CDATA[<]]> 80  ... 70 + error from function 'GravDef'
        /// > 80, <![CDATA[<]]> 90  ... 80 + error from function 'CioLocation'
        /// > 90, <![CDATA[<]]> 100 ... 90 + error from function 'CioBasis'
        /// </pre>
        /// </returns>
        /// Values of 'location->where' and 'CoordSys' dictate the various standard kinds of place:
        /// <pre>
        ///     Location->Where = 0 and CoordSys = 1: apparent place
        ///     Location->Where = 1 and CoordSys = 1: topocentric place
        ///     Location->Where = 0 and CoordSys = 0: virtual place
        ///     Location->Where = 1 and CoordSys = 0: local place
        ///     Location->Where = 0 and CoordSys = 3: astrometric place
        ///     Location->Where = 1 and CoordSys = 3: topocentric astrometric place
        /// </pre>
        /// <para>Input value of 'DeltaT' is used only when 'Location->Where' equals 1 or 2 (observer is 
        /// on surface of Earth or in a near-Earth satellite). </para>
        /// <remarks>
        /// </remarks>
        public static short Place(double JdTt, Object3 CelObject, Observer Location, double DeltaT, CoordSys CoordSys, Accuracy Accuracy, ref SkyPos Output)
        {
            // Initialise if necessary
            if (!isInitialised) Initialise();

            var argCelObject1 = O3IFromObject3(CelObject);
            return PlaceLib(JdTt, ref argCelObject1, ref Location, DeltaT, CoordSys, Accuracy, ref Output);
        }

        /// <summary>
        /// Precesses equatorial rectangular coordinates from one epoch to another.
        /// </summary>
        /// <param name="JdTdb1">TDB Julian date of first epoch.  See remarks below.</param>
        /// <param name="Pos1">Position vector, geocentric equatorial rectangular coordinates, referred to mean dynamical equator and equinox of first epoch.</param>
        /// <param name="JdTdb2">TDB Julian date of second epoch.  See remarks below.</param>
        /// <param name="Pos2">Position vector, geocentric equatorial rectangular coordinates, referred to mean dynamical equator and equinox of second epoch.</param>
        /// <returns><pre>
        /// 0 ... everything OK
        /// 1 ... Precession not to or from J2000.0; 'JdTdb1' or 'JdTdb2' not 2451545.0.
        /// </pre></returns>
        /// <remarks> One of the two epochs must be J2000.0.  The coordinates are referred to the mean dynamical equator and equinox of the two respective epochs.</remarks>
        public static short Precession(double JdTdb1, double[] Pos1, double JdTdb2, ref double[] Pos2)
        {
            var VPos2 = new PosVector();
            short rc;

            var argPos11 = ArrToPosVec(Pos1);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            rc = PrecessionLib(JdTdb1, ref argPos11, JdTdb2, ref VPos2);
            PosVecToArr(VPos2, ref Pos2);
            return rc;
        }

        /// <summary>
        /// Applies proper motion, including foreshortening effects, to a star's position.
        /// </summary>
        /// <param name="JdTdb1">TDB Julian date of first epoch.</param>
        /// <param name="Pos">Position vector at first epoch.</param>
        /// <param name="Vel">Velocity vector at first epoch.</param>
        /// <param name="JdTdb2">TDB Julian date of second epoch.</param>
        /// <param name="Pos2">Position vector at second epoch.</param>
        /// <remarks></remarks>
        public static void ProperMotion(double JdTdb1, double[] Pos, double[] Vel, double JdTdb2, ref double[] Pos2)
        {
            var VPos2 = new PosVector();

            var argPos1 = ArrToPosVec(Pos);
            var argVel1 = ArrToVelVec(Vel);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            ProperMotionLib(JdTdb1, ref argPos1, ref argVel1, JdTdb2, ref VPos2);

            PosVecToArr(VPos2, ref Pos2);
        }

        /// <summary>
        /// Converts equatorial spherical coordinates to a vector (equatorial rectangular coordinates).
        /// </summary>
        /// <param name="Ra">Right ascension (hours).</param>
        /// <param name="Dec">Declination (degrees).</param>
        /// <param name="Dist">Distance in AU</param>
        /// <param name="Vector">Position vector, equatorial rectangular coordinates (AU).</param>
        /// <remarks></remarks>
        public static void RaDec2Vector(double Ra, double Dec, double Dist, ref double[] Vector)
        {
            var VVector = new PosVector();


            // Initialise if necessary
            if (!isInitialised) Initialise();

            RaDec2VectorLib(Ra, Dec, Dist, ref VVector);
            PosVecToArr(VVector, ref Vector);
        }

        /// <summary>
        /// Predicts the radial velocity of the observed object as it would be measured by spectroscopic means.
        /// </summary>
        /// <param name="CelObject">Specifies the celestial object of interest</param>
        /// <param name="Pos"> Geometric position vector of object with respect to observer, corrected for light-time, in AU.</param>
        /// <param name="Vel">Velocity vector of object with respect to solar system barycenter, in AU/day.</param>
        /// <param name="VelObs">Velocity vector of observer with respect to solar system barycenter, in AU/day.</param>
        /// <param name="DObsGeo">Distance from observer to geocenter, in AU.</param>
        /// <param name="DObsSun">Distance from observer to Sun, in AU.</param>
        /// <param name="DObjSun">Distance from object to Sun, in AU.</param>
        /// <param name="Rv">The observed radial velocity measure times the speed of light, in kilometers/second.</param>
        /// <remarks> Radial velocity is here defined as the radial velocity measure (z) times the speed of light.  
        /// For a solar system body, it applies to a fictitious emitter at the center of the observed object, 
        /// assumed massless (no gravitational red shift), and does not in general apply to reflected light.  
        /// For stars, it includes all effects, such as gravitational red shift, contained in the catalog 
        /// barycentric radial velocity measure, a scalar derived from spectroscopy.  Nearby stars with a known 
        /// kinematic velocity vector (obtained independently of spectroscopy) can be treated like 
        /// solar system objects.</remarks>
        public static void RadVel(Object3 CelObject, double[] Pos, double[] Vel, double[] VelObs, double DObsGeo, double DObsSun, double DObjSun, ref double Rv)
        {
            var argCelObject1 = O3IFromObject3(CelObject);
            var argPos1 = ArrToPosVec(Pos);
            var argVel1 = ArrToVelVec(Vel);
            var argVelObs1 = ArrToVelVec(VelObs);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            RadVelLib(ref argCelObject1, ref argPos1, ref argVel1, ref argVelObs1, DObsGeo, DObsSun, DObjSun, ref Rv);
        }

        /// <summary>
        /// Computes atmospheric refraction in zenith distance. 
        /// </summary>
        /// <param name="Location">Structure containing observer's location.</param>
        /// <param name="RefOption">1 ... Use 'standard' atmospheric conditions; 2 ... Use atmospheric 
        /// parameters input in the 'Location' structure.</param>
        /// <param name="ZdObs">Observed zenith distance, in degrees.</param>
        /// <returns>Atmospheric refraction, in degrees.</returns>
        /// <remarks>This version computes approximate refraction for optical wavelengths. This function 
        /// can be used for planning observations or telescope pointing, but should not be used for the 
        /// reduction of precise observations.</remarks>
        public static double Refract(OnSurface Location, RefractionOption RefOption, double ZdObs)
        {
            // Initialise if necessary
            if (!isInitialised) Initialise();

            return RefractLib(ref Location, RefOption, ZdObs);
        }

        /// <summary>
        /// Computes the Greenwich sidereal time, either mean or apparent, at Julian date 'JdHigh' + 'JdLow'.
        /// </summary>
        /// <param name="JdHigh">High-order part of UT1 Julian date.</param>
        /// <param name="JdLow">Low-order part of UT1 Julian date.</param>
        /// <param name="DeltaT"> Difference TT-UT1 at 'JdHigh'+'JdLow', in seconds of time.</param>
        /// <param name="GstType">0 ... compute Greenwich mean sidereal time; 1 ... compute Greenwich apparent sidereal time</param>
        /// <param name="Method">Selection for method: 0 ... CIO-based method; 1 ... equinox-based method</param>
        /// <param name="Accuracy">Selection for accuracy</param>
        /// <param name="Gst">Greenwich apparent sidereal time, in hours.</param>
        /// <returns><pre>
        ///          0 ... everything OK
        ///          1 ... invalid value of 'Accuracy'
        ///          2 ... invalid value of 'Method'
        /// > 10, <![CDATA[<]]> 30 ... 10 + error from function 'CioRai'
        /// </pre></returns>
        /// <remarks> The Julian date may be split at any point, but for highest precision, set 'JdHigh' 
        /// to be the integral part of the Julian date, and set 'JdLow' to be the fractional part.</remarks>
        public static short SiderealTime(double JdHigh, double JdLow, double DeltaT, GstType GstType, Method Method, Accuracy Accuracy, ref double Gst)
        {
            // Initialise if necessary
            if (!isInitialised) Initialise();

            return SiderealTimeLib(JdHigh, JdLow, DeltaT, GstType, Method, Accuracy, ref Gst);
        }

        /// <summary>
        /// Transforms a vector from one coordinate system to another with same origin and axes rotated about the z-axis.
        /// </summary>
        /// <param name="Angle"> Angle of coordinate system rotation, positive counterclockwise when viewed from +z, in degrees.</param>
        /// <param name="Pos1">Position vector.</param>
        /// <param name="Pos2">Position vector expressed in new coordinate system rotated about z by 'angle'.</param>
        /// <remarks></remarks>
        public static void Spin(double Angle, double[] Pos1, ref double[] Pos2)
        {
            var VPOs2 = new PosVector();
            var argPos11 = ArrToPosVec(Pos1);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            SpinLib(Angle, ref argPos11, ref VPOs2);

            PosVecToArr(VPOs2, ref Pos2);
        }

        /// <summary>
        /// Converts angular quantities for stars to vectors.
        /// </summary>
        /// <param name="Star">Catalog entry structure containing ICRS catalog data </param>
        /// <param name="Pos">Position vector, equatorial rectangular coordinates, components in AU.</param>
        /// <param name="Vel">Velocity vector, equatorial rectangular coordinates, components in AU/Day.</param>
        /// <remarks></remarks>
        public static void StarVectors(CatEntry3 Star, ref double[] Pos, ref double[] Vel)
        {
            var VPos = new PosVector();
            var VVel = new VelVector();

            // Initialise if necessary
            if (!isInitialised) Initialise();

            StarVectorsLib(ref Star, ref VPos, ref VVel);

            PosVecToArr(VPos, ref Pos);
            VelVecToArr(VVel, ref Vel);
        }

        /// <summary>
        /// Computes the Terrestrial Time (TT) or Terrestrial Dynamical Time (TDT) Julian date corresponding 
        /// to a Barycentric Dynamical Time (TDB) Julian date.
        /// </summary>
        /// <param name="TdbJd">TDB Julian date.</param>
        /// <param name="TtJd">TT Julian date.</param>
        /// <param name="SecDiff">Difference 'tdb_jd'-'tt_jd', in seconds.</param>
        /// <remarks>Expression used in this function is a truncated form of a longer and more precise 
        /// series given in: Explanatory Supplement to the Astronomical Almanac, pp. 42-44 and p. 316. 
        /// The result is good to about 10 microseconds.</remarks>
        public static void Tdb2Tt(double TdbJd, ref double TtJd, ref double SecDiff)
        {

            // Initialise if necessary
            if (!isInitialised) Initialise();

            Tdb2TtLib(TdbJd, ref TtJd, ref SecDiff);
        }

        /// <summary>
        /// This function rotates a vector from the terrestrial to the celestial system. 
        /// </summary>
        /// <param name="JdHigh">High-order part of UT1 Julian date.</param>
        /// <param name="JdLow">Low-order part of UT1 Julian date.</param>
        /// <param name="DeltaT">Value of Delta T (= TT - UT1) at the input UT1 Julian date.</param>
        /// <param name="Method"> Selection for method: 0 ... CIO-based method; 1 ... equinox-based method</param>
        /// <param name="Accuracy">Selection for accuracy</param>
        /// <param name="OutputOption">0 ... The output vector is referred to GCRS axes; 1 ... The output 
        /// vector is produced with respect to the equator and equinox of date.</param>
        /// <param name="xp">Conventionally-defined X coordinate of celestial intermediate pole with respect to 
        /// ITRF pole, in arcseconds.</param>
        /// <param name="yp">Conventionally-defined Y coordinate of celestial intermediate pole with respect to 
        /// ITRF pole, in arcseconds.</param>
        /// <param name="VecT">Position vector, geocentric equatorial rectangular coordinates, referred to ITRF 
        /// axes (terrestrial system) in the normal case where 'option' = 0.</param>
        /// <param name="VecC"> Position vector, geocentric equatorial rectangular coordinates, referred to GCRS 
        /// axes (celestial system) or with respect to the equator and equinox of date, depending on 'Option'.</param>
        /// <returns><pre>
        ///    0 ... everything is ok
        ///    1 ... invalid value of 'Accuracy'
        ///    2 ... invalid value of 'Method'
        /// > 10 ... 10 + error from function 'CioLocation'
        /// > 20 ... 20 + error from function 'CioBasis'
        /// </pre></returns>
        /// <remarks>'x' = 'y' = 0 means no polar motion transformation.
        /// <para>
        /// The 'option' flag only works for the equinox-based method.
        /// </para></remarks>
        public static short Ter2Cel(double JdHigh, double JdLow, double DeltaT, Method Method, Accuracy Accuracy, OutputVectorOption OutputOption, double xp, double yp, double[] VecT, ref double[] VecC)
        {
            var VVecC = new PosVector();
            short rc;
            var argVecT1 = ArrToPosVec(VecT);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            rc = Ter2CelLib(JdHigh, JdLow, DeltaT, Method, Accuracy, OutputOption, xp, yp, ref argVecT1, ref VVecC);

            PosVecToArr(VVecC, ref VecC);
            return rc;
        }

        /// <summary>
        /// Computes the position and velocity vectors of a terrestrial observer with respect to the center of the Earth.
        /// </summary>
        /// <param name="Location">Structure containing observer's location </param>
        /// <param name="St">Local apparent sidereal time at reference meridian in hours.</param>
        /// <param name="Pos">Position vector of observer with respect to center of Earth, equatorial 
        /// rectangular coordinates, referred to true equator and equinox of date, components in AU.</param>
        /// <param name="Vel">Velocity vector of observer with respect to center of Earth, equatorial rectangular 
        /// coordinates, referred to true equator and equinox of date, components in AU/day.</param>
        /// <remarks>
        /// If reference meridian is Greenwich and st=0, 'pos' is effectively referred to equator and Greenwich.
        /// <para> This function ignores polar motion, unless the observer's longitude and latitude have been 
        /// corrected for it, and variation in the length of day (angular velocity of earth).</para>
        /// <para>The true equator and equinox of date do not form an inertial system.  Therefore, with respect 
        /// to an inertial system, the very small velocity component (several meters/day) due to the precession 
        /// and nutation of the Earth's axis is not accounted for here.</para>
        /// </remarks>
        public static void Terra(OnSurface Location, double St, ref double[] Pos, ref double[] Vel)
        {
            var VPos = new PosVector();
            var VVel = new VelVector();

            // Initialise if necessary
            if (!isInitialised) Initialise();

            TerraLib(ref Location, St, ref VPos, ref VVel);

            PosVecToArr(VPos, ref Pos);
            VelVecToArr(VVel, ref Vel);
        }

        /// <summary>
        /// Computes the topocentric place of a solar system body.
        /// </summary>
        /// <param name="JdTt">TT Julian date for topocentric place.</param>
        /// <param name="SsBody">structure containing the body designation for the solar system body</param>
        /// <param name="DeltaT">Difference TT-UT1 at 'JdTt', in seconds of time.</param>
        /// <param name="Position">Specifies the position of the observer</param>
        /// <param name="Accuracy">Selection for accuracy</param>
        /// <param name="Ra">Apparent right ascension in hours, referred to true equator and equinox of date.</param>
        /// <param name="Dec">Apparent declination in degrees, referred to true equator and equinox of date.</param>
        /// <param name="Dis">True distance from Earth to planet at 'JdTt' in AU.</param>
        /// <returns><pre>
        /// =  0 ... Everything OK.
        /// =  1 ... Invalid value of 'Where' in structure 'Location'.
        /// > 10 ... Error code from function 'Place'.
        /// </pre></returns>
        /// <remarks></remarks>
        public static short TopoPlanet(double JdTt, Object3 SsBody, double DeltaT, OnSurface Position, Accuracy Accuracy, ref double Ra, ref double Dec, ref double Dis)
        {
            var argSsBody1 = O3IFromObject3(SsBody);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            return TopoPlanetLib(JdTt, ref argSsBody1, DeltaT, ref Position, Accuracy, ref Ra, ref Dec, ref Dis);
        }

        /// <summary>
        /// Computes the topocentric place of a star at date 'JdTt', given its catalog mean place, proper motion, parallax, and radial velocity.
        /// </summary>
        /// <param name="JdTt">TT Julian date for topocentric place.</param>
        /// <param name="DeltaT">Difference TT-UT1 at 'JdTt', in seconds of time.</param>
        /// <param name="Star">Catalog entry structure containing catalog data for the object in the ICRS</param>
        /// <param name="Position">Specifies the position of the observer</param>
        /// <param name="Accuracy">Code specifying the relative accuracy of the output position.</param>
        /// <param name="Ra"> Topocentric right ascension in hours, referred to true equator and equinox of date 'JdTt'.</param>
        /// <param name="Dec">Topocentric declination in degrees, referred to true equator and equinox of date 'JdTt'.</param>
        /// <returns><pre>
        /// =  0 ... Everything OK.
        /// =  1 ... Invalid value of 'Where' in structure 'Location'.
        /// > 10 ... Error code from function 'MakeObject'.
        /// > 20 ... Error code from function 'Place'.
        /// </pre></returns>
        /// <remarks></remarks>
        public static short TopoStar(double JdTt, double DeltaT, CatEntry3 Star, OnSurface Position, Accuracy Accuracy, ref double Ra, ref double Dec)
        {

            short rc;

            // Initialise if necessary
            if (!isInitialised) Initialise();

            try
            {
                LogMessage("TopoStar", "JD Accuracy:            " + JdTt + " " + Accuracy.ToString());
                LogMessage("TopoStar", "  Star.RA:              " + Utilities.HoursToHMS(Star.RA, ":", ":", "", 3));
                LogMessage("TopoStar", "  Dec:                  " + Utilities.DegreesToDMS(Star.Dec, ":", ":", "", 3));
                LogMessage("TopoStar", "  Catalog:              " + Star.Catalog);
                LogMessage("TopoStar", "  Parallax:             " + Star.Parallax);
                LogMessage("TopoStar", "  ProMoDec:             " + Star.ProMoDec);
                LogMessage("TopoStar", "  ProMoRA:              " + Star.ProMoRA);
                LogMessage("TopoStar", "  RadialVelocity:       " + Star.RadialVelocity);
                LogMessage("TopoStar", "  StarName:             " + Star.StarName);
                LogMessage("TopoStar", "  StarNumber:           " + Star.StarNumber);
                LogMessage("TopoStar", "  Position.Height:      " + Position.Height);
                LogMessage("TopoStar", "  Position.Latitude:    " + Position.Latitude);
                LogMessage("TopoStar", "  Position.Longitude:   " + Position.Longitude);
                LogMessage("TopoStar", "  Position.Pressure:    " + Position.Pressure);
                LogMessage("TopoStar", "  Position.Temperature: " + Position.Temperature);
                rc = TopoStarLib(JdTt, DeltaT, ref Star, ref Position, Accuracy, ref Ra, ref Dec);
                LogMessage("TopoStar", "  Return Code: " + rc + ", RA Dec: " + Utilities.HoursToHMS(Ra, ":", ":", "", 3) + " " + Utilities.DegreesToDMS(Dec, ":", ":", "", 3));
                return rc;
            }
            catch (Exception ex)
            {
                LogMessage("TopoStar", "Exception: " + ex.ToString());
            }

            return default;
        }

        /// <summary>
        /// To transform a star's catalog quantities for a change of epoch and/or equator and equinox.
        /// </summary>
        /// <param name="TransformOption">Transformation option</param>
        /// <param name="DateInCat">TT Julian date, or year, of input catalog data.</param>
        /// <param name="InCat">An entry from the input catalog, with units as given in the struct definition </param>
        /// <param name="DateNewCat">TT Julian date, or year, of transformed catalog data.</param>
        /// <param name="NewCatId">Three-character abbreviated name of the transformed catalog. </param>
        /// <param name="NewCat"> The transformed catalog entry, with units as given in the struct definition </param>
        /// <returns>
        /// <pre>
        /// = 0 ... Everything OK.
        /// = 1 ... Invalid value of an input date for option 2 or 3 (see Note 1 below).
        /// = 2 ... Catalogue ID exceeds three characters
        /// </pre></returns>
        /// <remarks>Also used to rotate catalog quantities on the dynamical equator and equinox of J2000.0 to the ICRS or vice versa.
        /// <para>1. 'DateInCat' and 'DateNewCat' may be specified either as a Julian date (e.g., 2433282.5) or 
        /// a Julian year and fraction (e.g., 1950.0).  Values less than 10000 are assumed to be years. 
        /// For 'TransformOption' = 2 or 'TransformOption' = 3, either 'DateInCat' or 'DateNewCat' must be 2451545.0 or 
        /// 2000.0 (J2000.0).  For 'TransformOption' = 4 and 'TransformOption' = 5, 'DateInCat' and 'DateNewCat' are ignored.</para>
        /// <para>2. 'TransformOption' = 1 updates the star's data to account for the star's space motion between the first 
        /// and second dates, within a fixed reference frame. 'TransformOption' = 2 applies a rotation of the reference 
        /// frame corresponding to precession between the first and second dates, but leaves the star fixed in 
        /// space. 'TransformOption' = 3 provides both transformations. 'TransformOption' = 4 and 'TransformOption' = 5 provide a a 
        /// fixed rotation about very small angles (<![CDATA[<]]>0.1 arcsecond) to take data from the dynamical system 
        /// of J2000.0 to the ICRS ('TransformOption' = 4) or vice versa ('TransformOption' = 5).</para>
        /// <para>3. For 'TransformOption' = 1, input data can be in any fixed reference system. for 'TransformOption' = 2 or 
        /// 'TransformOption' = 3, this function assumes the input data is in the dynamical system and produces output 
        /// in the dynamical system.  for 'TransformOption' = 4, the input data must be on the dynamical equator and 
        /// equinox of J2000.0.  for 'TransformOption' = 5, the input data must be in the ICRS.</para>
        /// <para>4. This function cannot be properly used to bring data from old star catalogs into the 
        /// modern system, because old catalogs were compiled using a set of constants that are incompatible 
        /// with modern values.  In particular, it should not be used for catalogs whose positions and 
        /// proper motions were derived by assuming a precession constant significantly different from 
        /// the value implicit in function 'precession'.</para></remarks>
        public static short TransformCat(TransformationOption3 TransformOption, double DateInCat, CatEntry3 InCat, double DateNewCat, string NewCatId, ref CatEntry3 NewCat)
        {

            // Initialise if necessary
            if (!isInitialised) Initialise();

            return TransformCatLib(TransformOption, DateInCat, ref InCat, DateNewCat, NewCatId, ref NewCat);
        }

        /// <summary>
        /// Convert Hipparcos catalog data at epoch J1991.25 to epoch J2000.0, for use within NOVAS.
        /// </summary>
        /// <param name="Hipparcos">An entry from the Hipparcos catalog, at epoch J1991.25, with all members 
        /// having Hipparcos catalog units.  See Note 1 below </param>
        /// <param name="Hip2000">The transformed input entry, at epoch J2000.0.  See Note 2 below</param>
        /// <remarks>To be used only for Hipparcos or Tycho stars with linear space motion.  Both input and 
        /// output data is in the ICRS.
        /// <para>
        /// 1. Input (Hipparcos catalog) epoch and units:
        /// <list type="bullet">
        /// <item>Epoch: J1991.25</item>
        /// <item>Right ascension (RA): degrees</item>
        /// <item>Declination (Dec): degrees</item>
        /// <item>Proper motion in RA: milliarcseconds per year</item>
        /// <item>Proper motion in Dec: milliarcseconds per year</item>
        /// <item>Parallax: milliarcseconds</item>
        /// <item>Radial velocity: kilometers per second (not in catalog)</item>
        /// </list>
        /// </para>
        /// <para>
        /// 2. Output (modified Hipparcos) epoch and units:
        /// <list type="bullet">
        /// <item>Epoch: J2000.0</item>
        /// <item>Right ascension: hours</item>
        /// <item>Declination: degrees</item>
        /// <item>Proper motion in RA: milliarcseconds per year</item>
        /// <item>Proper motion in Dec: milliarcseconds per year</item>
        /// <item>Parallax: milliarcseconds</item>
        /// <item>Radial velocity: kilometers per second</item>
        /// </list>>
        /// </para>
        /// </remarks>
        public static void TransformHip(CatEntry3 Hipparcos, ref CatEntry3 Hip2000)
        {

            // Initialise if necessary
            if (!isInitialised) Initialise();

            TransformHipLib(ref Hipparcos, ref Hip2000);
        }

        /// <summary>
        /// Converts a vector in equatorial rectangular coordinates to equatorial spherical coordinates.
        /// </summary>
        /// <param name="Pos">Position vector, equatorial rectangular coordinates.</param>
        /// <param name="Ra">Right ascension in hours.</param>
        /// <param name="Dec">Declination in degrees.</param>
        /// <returns>
        /// <pre>
        /// = 0 ... Everything OK.
        /// = 1 ... All vector components are zero; 'Ra' and 'Dec' are indeterminate.
        /// = 2 ... Both Pos[0] and Pos[1] are zero, but Pos[2] is nonzero; 'Ra' is indeterminate.
        /// </pre></returns>
        /// <remarks></remarks>
        public static short Vector2RaDec(double[] Pos, ref double Ra, ref double Dec)
        {
            var argPos1 = ArrToPosVec(Pos);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            return Vector2RaDecLib(ref argPos1, ref Ra, ref Dec);
        }

        /// <summary>
        /// Compute the virtual place of a planet or other solar system body.
        /// </summary>
        /// <param name="JdTt">TT Julian date for virtual place.</param>
        /// <param name="SsBody">structure containing the body designation for the solar system body(</param>
        /// <param name="Accuracy">Code specifying the relative accuracy of the output position.</param>
        /// <param name="Ra">Virtual right ascension in hours, referred to the GCRS.</param>
        /// <param name="Dec">Virtual declination in degrees, referred to the GCRS.</param>
        /// <param name="Dis">True distance from Earth to planet in AU.</param>
        /// <returns>
        /// <pre>
        /// =  0 ... Everything OK.
        /// =  1 ... Invalid value of 'Type' in structure 'SsBody'.
        /// > 10 ... Error code from function 'Place'.
        /// </pre></returns>
        /// <remarks></remarks>
        public static short VirtualPlanet(double JdTt, Object3 SsBody, Accuracy Accuracy, ref double Ra, ref double Dec, ref double Dis)
        {
            var argSsBody1 = O3IFromObject3(SsBody);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            return VirtualPlanetLib(JdTt, ref argSsBody1, Accuracy, ref Ra, ref Dec, ref Dis);
        }

        /// <summary>
        /// Computes the virtual place of a star at date 'JdTt', given its catalog mean place, proper motion, parallax, and radial velocity.
        /// </summary>
        /// <param name="JdTt">TT Julian date for virtual place.</param>
        /// <param name="Star">catalog entry structure containing catalog data for the object in the ICRS</param>
        /// <param name="Accuracy">Code specifying the relative accuracy of the output position.</param>
        /// <param name="Ra">Virtual right ascension in hours, referred to the GCRS.</param>
        /// <param name="Dec">Virtual declination in degrees, referred to the GCRS.</param>
        /// <returns>
        /// <pre>
        /// =  0 ... Everything OK.
        /// > 10 ... Error code from function 'MakeObject'.
        /// > 20 ... Error code from function 'Place'
        /// </pre></returns>
        /// <remarks></remarks>
        public static short VirtualStar(double JdTt, CatEntry3 Star, Accuracy Accuracy, ref double Ra, ref double Dec)
        {
            // Initialise if necessary
            if (!isInitialised) Initialise();

            return VirtualStarLib(JdTt, ref Star, Accuracy, ref Ra, ref Dec);
        }

        /// <summary>
        /// Corrects a vector in the ITRF (rotating Earth-fixed system) for polar motion, and also corrects 
        /// the longitude origin (by a tiny amount) to the Terrestrial Intermediate Origin (TIO).
        /// </summary>
        /// <param name="Tjd">TT or UT1 Julian date.</param>
        ///       direction (short int)
        /// <param name="Direction">Flag determining 'direction' of transformation;
        /// direction  = 0 transformation applied, ITRS to terrestrial intermediate system
        /// direction != 0 inverse transformation applied, terrestrial intermediate system to ITRS</param>
        /// <param name="xp">Conventionally-defined X coordinate of Celestial Intermediate Pole with 
        /// respect to ITRF pole, in arcseconds.</param>
        /// <param name="yp">Conventionally-defined Y coordinate of Celestial Intermediate Pole with 
        /// respect to ITRF pole, in arcseconds.</param>
        /// <param name="Pos1">Position vector, geocentric equatorial rectangular coordinates, 
        /// referred to ITRF axes.</param>
        /// <param name="Pos2">Position vector, geocentric equatorial rectangular coordinates, 
        /// referred to true equator and TIO.</param>
        /// <remarks></remarks>
        public static void Wobble(double Tjd, TransformationDirection Direction, double xp, double yp, double[] Pos1, ref double[] Pos2)
        {
            var VPos2 = new PosVector();

            var argPos11 = ArrToPosVec(Pos1);

            // Initialise if necessary
            if (!isInitialised) Initialise();

            WobbleLib(Tjd, (short)Direction, xp, yp, ref argPos11, ref VPos2);

            PosVecToArr(VPos2, ref Pos2);
        }
        #endregion

        #region Library Entry Points for Ephemeris and RACIOFile

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "set_racio_file")]
        private static extern void SetRACIOFile([MarshalAs(UnmanagedType.LPStr)] string Name);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ephem_close")]
        private static extern short EphemCloseLib();

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ephem_open")]
        private static extern short EphemOpenLib([MarshalAs(UnmanagedType.LPStr)] string Ephem_Name, ref double JD_Begin, ref double JD_End, ref short DENumber);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "planet_ephemeris")]
        private static extern short PlanetEphemerisLib(ref JDHighPrecision Tjd, Target Target, Target Center, ref PosVector Position, ref VelVector Velocity);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "readeph")]
        private static extern IntPtr ReadEphLib(int Mp, [MarshalAs(UnmanagedType.LPStr)] string Name, double Jd, ref int Err);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cleaneph")]
        private static extern void CleanEphLib();

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "solarsystem")]
        private static extern short SolarSystemLib(double tjd, short body, short origin, ref PosVector pos, ref VelVector vel);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "state")]
        private static extern short StateLib(ref JDHighPrecision Jed, Target Target, ref PosVector TargetPos, ref VelVector TargetVel);
        #endregion

        #region Library Entry Points for NOVAS

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "aberration")]
        private static extern void AberrationLib(ref PosVector Pos, ref VelVector Vel, double LightTime, ref PosVector Pos2);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "app_planet")]
        private static extern short AppPlanetLib(double JdTt, ref Object3Internal SsBody, Accuracy Accuracy, ref double Ra, ref double Dec, ref double Dis);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "app_star")]
        private static extern short AppStarLib(double JdTt, ref CatEntry3 Star, Accuracy Accuracy, ref double Ra, ref double Dec);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "astro_planet")]
        private static extern short AstroPlanetLib(double JdTt, ref Object3Internal SsBody, Accuracy Accuracy, ref double Ra, ref double Dec, ref double Dis);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "astro_star")]
        private static extern short AstroStarLib(double JdTt, ref CatEntry3 Star, Accuracy Accuracy, ref double Ra, ref double Dec);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "bary2obs")]
        private static extern void Bary2ObsLib(ref PosVector Pos, ref PosVector PosObs, ref PosVector Pos2, ref double Lighttime);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cal_date")]
        private static extern void CalDateLib(double Tjd, ref short Year, ref short Month, ref short Day, ref double Hour);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cel2ter")]
        private static extern short Cel2TerLib(double JdHigh, double JdLow, double DeltaT, Method Method, Accuracy Accuracy, OutputVectorOption OutputOption, double x, double y, ref PosVector VecT, ref PosVector VecC);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cel_pole")]
        private static extern short CelPoleLib(double Tjd, PoleOffsetCorrection Type, double Dpole1, double Dpole2);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cio_array")]
        private static extern short CioArrayLib(double JdTdb, int NPts, ref RAOfCioArray Cio);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cio_basis")]
        private static extern short CioBasisLib(double JdTdbEquionx, double RaCioEquionx, ReferenceSystem RefSys, Accuracy Accuracy, ref double x, ref double y, ref double z);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cio_location")]
        private static extern short CioLocationLib(double JdTdb, Accuracy Accuracy, ref double RaCio, ref ReferenceSystem RefSys);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cio_ra")]
        private static extern short CioRaLib(double JdTt, Accuracy Accuracy, ref double RaCio);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "d_light")]
        private static extern double DLightLib(ref PosVector Pos1, ref PosVector PosObs);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "e_tilt")]
        private static extern void ETiltLib(double JdTdb, Accuracy Accuracy, ref double Mobl, ref double Tobl, ref double Ee, ref double Dpsi, ref double Deps);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ecl2equ_vec")]
        private static extern short Ecl2EquVecLib(double JdTt, CoordSys CoordSys, Accuracy Accuracy, ref PosVector Pos1, ref PosVector Pos2);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ee_ct")]
        private static extern double EeCtLib(double JdHigh, double JdLow, Accuracy Accuracy);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ephemeris")]
        private static extern short EphemerisLib(ref JDHighPrecision Jd, ref Object3Internal CelObj, Origin Origin, Accuracy Accuracy, ref PosVector Pos, ref VelVector Vel);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "equ2ecl")]
        private static extern short Equ2EclLib(double JdTt, CoordSys CoordSys, Accuracy Accuracy, double Ra, double Dec, ref double ELon, ref double ELat);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "equ2ecl_vec")]
        private static extern short Equ2EclVecLib(double JdTt, CoordSys CoordSys, Accuracy Accuracy, ref PosVector Pos1, ref PosVector Pos2);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "equ2gal")]
        private static extern void Equ2GalLib(double RaI, double DecI, ref double GLon, ref double GLat);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "equ2hor")]
        private static extern void Equ2HorLib(double Jd_Ut1, double DeltT, Accuracy Accuracy, double x, double y, ref OnSurface Location, double Ra, double Dec, RefractionOption RefOption, ref double Zd, ref double Az, ref double RaR, ref double DecR);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "era")]
        private static extern double EraLib(double JdHigh, double JdLow);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "frame_tie")]
        private static extern void FrameTieLib(ref PosVector Pos1, FrameConversionDirection Direction, ref PosVector Pos2);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fund_args")]
        private static extern void FundArgsLib(double t, ref FundamentalArgs a);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gcrs2equ")]
        private static extern short Gcrs2EquLib(double JdTt, CoordSys CoordSys, Accuracy Accuracy, double RaG, double DecG, ref double Ra, ref double Dec);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "geo_posvel")]
        private static extern short GeoPosVelLib(double JdTt, double DeltaT, Accuracy Accuracy, ref Observer Obs, ref PosVector Pos, ref VelVector Vel);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "grav_def")]
        private static extern short GravDefLib(double JdTdb, EarthDeflection LocCode, Accuracy Accuracy, ref PosVector Pos1, ref PosVector PosObs, ref PosVector Pos2);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "grav_vec")]
        private static extern void GravVecLib(ref PosVector Pos1, ref PosVector PosObs, ref PosVector PosBody, double RMass, ref PosVector Pos2);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ira_equinox")]
        private static extern double IraEquinoxLib(double JdTdb, EquinoxType Equinox, Accuracy Accuracy);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "julian_date")]
        private static extern double JulianDateLib(short Year, short Month, short Day, double Hour);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "light_time")]
        private static extern short LightTimeLib(double JdTdb, ref Object3Internal SsObject, ref PosVector PosObs, double TLight0, Accuracy Accuracy, ref PosVector Pos, ref double TLight);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "limb_angle")]
        private static extern void LimbAngleLib(ref PosVector PosObj, ref PosVector PosObs, ref double LimbAng, ref double NadirAng);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "local_planet")]
        private static extern short LocalPlanetLib(double JdTt, ref Object3Internal SsBody, double DeltaT, ref OnSurface Position, Accuracy Accuracy, ref double Ra, ref double Dec, ref double Dis);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "local_star")]
        private static extern short LocalStarLib(double JdTt, double DeltaT, ref CatEntry3 Star, ref OnSurface Position, Accuracy Accuracy, ref double Ra, ref double Dec);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "make_cat_entry")]
        private static extern void MakeCatEntryLib([MarshalAs(UnmanagedType.LPStr)] string StarName, [MarshalAs(UnmanagedType.LPStr)] string Catalog, int StarNum, double Ra, double Dec, double PmRa, double PmDec, double Parallax, double RadVel, ref CatEntry3 Star);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "make_in_space")]
        private static extern void MakeInSpaceLib(ref PosVector ScPos, ref VelVector ScVel, ref InSpace ObsSpace);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "make_object")]
        private static extern short MakeObjectLib(ObjectType Type, short Number, [MarshalAs(UnmanagedType.LPStr)] string Name, ref CatEntry3 StarData, ref Object3Internal CelObj);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "make_observer")]
        private static extern short MakeObserverLib(ObserverLocation Where, ref OnSurface ObsSurface, ref InSpace ObsSpace, ref Observer Obs);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "make_observer_at_geocenter")]
        private static extern void MakeObserverAtGeocenterLib(ref Observer ObsAtGeocenter);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "make_observer_in_space")]
        private static extern void MakeObserverInSpaceLib(ref PosVector ScPos, ref VelVector ScVel, ref Observer ObsInSpace);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "make_observer_on_surface")]
        private static extern void MakeObserverOnSurfaceLib(double Latitude, double Longitude, double Height, double Temperature, double Pressure, ref Observer ObsOnSurface);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "make_on_surface")]
        private static extern void MakeOnSurfaceLib(double Latitude, double Longitude, double Height, double Temperature, double Pressure, ref OnSurface ObsSurface);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mean_obliq")]
        private static extern double MeanObliqLib(double JdTdb);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mean_star")]
        private static extern short MeanStarLib(double JdTt, double Ra, double Dec, Accuracy Accuracy, ref double IRa, ref double IDec);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "norm_ang")]
        private static extern double NormAngLib(double Angle);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "nutation")]
        private static extern void NutationLib(double JdTdb, NutationDirection Direction, Accuracy Accuracy, ref PosVector Pos, ref PosVector Pos2);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "nutation_angles")]
        private static extern void NutationAnglesLib(double t, Accuracy Accuracy, ref double DPsi, ref double DEps);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "place")]
        private static extern short PlaceLib(double JdTt, ref Object3Internal CelObject, ref Observer Location, double DeltaT, CoordSys CoordSys, Accuracy Accuracy, ref SkyPos Output);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "precession")]
        private static extern short PrecessionLib(double JdTdb1, ref PosVector Pos1, double JdTdb2, ref PosVector Pos2);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "proper_motion")]
        private static extern void ProperMotionLib(double JdTdb1, ref PosVector Pos, ref VelVector Vel, double JdTdb2, ref PosVector Pos2);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "rad_vel")]
        private static extern void RadVelLib(ref Object3Internal CelObject, ref PosVector Pos, ref VelVector Vel, ref VelVector VelObs, double DObsGeo, double DObsSun, double DObjSun, ref double Rv);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "radec2vector")]
        private static extern void RaDec2VectorLib(double Ra, double Dec, double Dist, ref PosVector Vector);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "refract")]
        private static extern double RefractLib(ref OnSurface Location, RefractionOption RefOption, double ZdObs);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "sidereal_time")]
        private static extern short SiderealTimeLib(double JdHigh, double JdLow, double DeltaT, GstType GstType, Method Method, Accuracy Accuracy, ref double Gst);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "spin")]
        private static extern void SpinLib(double Angle, ref PosVector Pos1, ref PosVector Pos2);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "starvectors")]
        private static extern void StarVectorsLib(ref CatEntry3 Star, ref PosVector Pos, ref VelVector Vel);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tdb2tt")]
        private static extern void Tdb2TtLib(double TdbJd, ref double TtJd, ref double SecDiff);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ter2cel")]
        private static extern short Ter2CelLib(double JdHigh, double JdLow, double DeltaT, Method Method, Accuracy Accuracy, OutputVectorOption OutputOption, double x, double y, ref PosVector VecT, ref PosVector VecC);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "terra")]
        private static extern void TerraLib(ref OnSurface Location, double St, ref PosVector Pos, ref VelVector Vel);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "topo_planet")]
        private static extern short TopoPlanetLib(double JdTt, ref Object3Internal SsBody, double DeltaT, ref OnSurface Position, Accuracy Accuracy, ref double Ra, ref double Dec, ref double Dis);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "topo_star")]
        private static extern short TopoStarLib(double JdTt, double DeltaT, ref CatEntry3 Star, ref OnSurface Position, Accuracy Accuracy, ref double Ra, ref double Dec);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "transform_cat")]

        private static extern short TransformCatLib(TransformationOption3 TransformOption, double DateInCat, ref CatEntry3 InCat, double DateNewCat, [MarshalAs(UnmanagedType.LPStr)] string NewCatId, ref CatEntry3 NewCat);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "transform_hip")]
        private static extern void TransformHipLib(ref CatEntry3 Hipparcos, ref CatEntry3 Hip2000);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "vector2radec")]
        private static extern short Vector2RaDecLib(ref PosVector Pos, ref double Ra, ref double Dec);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "virtual_planet")]
        private static extern short VirtualPlanetLib(double JdTt, ref Object3Internal SsBody, Accuracy Accuracy, ref double Ra, ref double Dec, ref double Dis);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "virtual_star")]
        private static extern short VirtualStarLib(double JdTt, ref CatEntry3 Star, Accuracy Accuracy, ref double Ra, ref double Dec);

        [DllImport(NOVAS_LIBRARY, CallingConvention = CallingConvention.Cdecl, EntryPoint = "wobble")]
        private static extern void WobbleLib(double Tjd, short Direction, double x, double y, ref PosVector Pos1, ref PosVector Pos2);
        #endregion

        #region Private Support Code

        /// <summary>
        /// Log a message from the NOVAS DLL
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        private static void LogMessage(string context, string message)
        {
            if (!(TL is null))
            {
                if (TL is TraceLogger traceLogger)
                {
                    traceLogger.LogMessage(LogLevel.Debug, context, message);
                }
                else
                {
                    TL.Log(LogLevel.Debug, $"{context} - {message}");
                }
            }
        }

        private static PosVector ArrToPosVec(double[] Arr)
        {
            // Create a new vector having the values in the supplied double array
            var V = new PosVector
            {
                x = Arr[0],
                y = Arr[1],
                z = Arr[2]
            };
            return V;
        }

        private static void PosVecToArr(PosVector V, ref double[] Ar)
        {
            // Copy a vector structure to a returned double array
            Ar[0] = V.x;
            Ar[1] = V.y;
            Ar[2] = V.z;
        }

        private static VelVector ArrToVelVec(double[] Arr)
        {
            // Create a new vector having the values in the supplied double array
            var V = new VelVector
            {
                x = Arr[0],
                y = Arr[1],
                z = Arr[2]
            };
            return V;
        }

        private static void VelVecToArr(VelVector V, ref double[] Ar)
        {
            // Copy a vector structure to a returned double array
            Ar[0] = V.x;
            Ar[1] = V.y;
            Ar[2] = V.z;
        }

        private static void RACioArrayStructureToArr(RAOfCioArray C, ref ArrayList Ar)
        {
            // Transfer all RACio values that have actually been set by the NOVAS DLL to the arraylist for return to the client
            if (C.Value1.RACio != Constants.RACIO_DEFAULT_VALUE)
                Ar.Add(C.Value1);
            if (C.Value2.RACio != Constants.RACIO_DEFAULT_VALUE)
                Ar.Add(C.Value2);
            if (C.Value3.RACio != Constants.RACIO_DEFAULT_VALUE)
                Ar.Add(C.Value3);
            if (C.Value4.RACio != Constants.RACIO_DEFAULT_VALUE)
                Ar.Add(C.Value4);
            if (C.Value5.RACio != Constants.RACIO_DEFAULT_VALUE)
                Ar.Add(C.Value5);
            if (C.Value6.RACio != Constants.RACIO_DEFAULT_VALUE)
                Ar.Add(C.Value6);
            if (C.Value7.RACio != Constants.RACIO_DEFAULT_VALUE)
                Ar.Add(C.Value7);
            if (C.Value8.RACio != Constants.RACIO_DEFAULT_VALUE)
                Ar.Add(C.Value8);
            if (C.Value9.RACio != Constants.RACIO_DEFAULT_VALUE)
                Ar.Add(C.Value9);
            if (C.Value10.RACio != Constants.RACIO_DEFAULT_VALUE)
                Ar.Add(C.Value10);
            if (C.Value11.RACio != Constants.RACIO_DEFAULT_VALUE)
                Ar.Add(C.Value11);
            if (C.Value12.RACio != Constants.RACIO_DEFAULT_VALUE)
                Ar.Add(C.Value12);
            if (C.Value13.RACio != Constants.RACIO_DEFAULT_VALUE)
                Ar.Add(C.Value13);
            if (C.Value14.RACio != Constants.RACIO_DEFAULT_VALUE)
                Ar.Add(C.Value14);
            if (C.Value15.RACio != Constants.RACIO_DEFAULT_VALUE)
                Ar.Add(C.Value15);
            if (C.Value16.RACio != Constants.RACIO_DEFAULT_VALUE)
                Ar.Add(C.Value16);
            if (C.Value17.RACio != Constants.RACIO_DEFAULT_VALUE)
                Ar.Add(C.Value17);
            if (C.Value18.RACio != Constants.RACIO_DEFAULT_VALUE)
                Ar.Add(C.Value18);
            if (C.Value19.RACio != Constants.RACIO_DEFAULT_VALUE)
                Ar.Add(C.Value19);
            if (C.Value20.RACio != Constants.RACIO_DEFAULT_VALUE)
                Ar.Add(C.Value20);
        }

        private static void O3FromO3Internal(Object3Internal O3I, ref Object3 O3)
        {
            O3.Name = O3I.Name;
            O3.Number = (Body)O3I.Number;
            O3.Star = O3I.Star;
            O3.Type = O3I.Type;
        }

        private static Object3Internal O3IFromObject3(Object3 O3)
        {
            var O3I = new Object3Internal
            {
                Name = O3.Name,
                Number = (short)O3.Number,
                Star = O3.Star,
                Type = O3.Type
            };
            return O3I;
        }

        #endregion

    }
}