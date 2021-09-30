using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;

namespace ASCOM.Tools
{
    /// <summary>
    /// ASCOM support utilities
    /// </summary>
    public class Utilities : IDisposable
    {

        private const double ABSOLUTE_ZERO_CELSIUS = -273.15; // Absolute zero on the Celsius temperature scale
        private const double JULIAN_DAY_WHEN_GREGORIAN_CALENDAR_WAS_INTRODUCED = 2299161.0; // Julian day number of the day on which the Gregorian calendar was first used - 15th October 1582
        private static readonly DateTime GREGORIAN_CALENDAR_INTRODUCTION = new DateTime(1582, 10, 15, 0, 0, 0); // Date and time when the Gregorian calendar was first used 00:00:00 15th October 1582

        #region Sexagesimal Conversions

        /// <summary>
        /// Convert a coordinate expressed in sexagesimal degrees to a double value (degrees)
        /// </summary>
        /// <param name="DMSString">The coordinate expressed in sexagesimal notation (Degrees:Minutes:Seconds)</param>
        /// <exception cref="InvalidValueException">If the supplied coordinate contains an invalid character. Valid characters are: "+", "-", "0..9", ":" and "the decimal separator used by the current thread".</exception>
        /// <returns>The supplied coordinate as a double value (degrees)</returns>
        /// <remarks>
        /// <para>
        /// Only the first three numeric components are considered, the remainder are ignored. Left to right positionally, the components are interpreted as degrees, minutes, and seconds. 
        /// If only two components are present, they are assumed to be degrees and minutes, and if only one components is present, it is assumed to be degrees. Any components can have a fractional part. 
        /// </para>
        /// <para>Examples of valid input values: 127:27:45, 12:26, +345, -45:34:12</para>
        /// <para>Examples of valid input values in a locale where point is used as the decimal separator: 60:27:45.846, 12:1.349, +345.1840746, -45:34:12.422</para>
        /// <para>Examples of valid input values in a locale where comma is used as the decimal separator: 60:27:45,846, 12:1,349, +345,1840746, -45:34:12,422</para>
        /// </remarks>
        public double DMSToDegrees(string DMSString)
        {
            double returnValue = 0.0; // Return value
            double sign; // Sign of the supplied value

            // Get the decimal separator character that is used in this locale
            string threadDecimalSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            // Create a validation pattern to ensure that invalid characters are detected and the input rejected if necessary
            string validatorPattern = string.Format(@"^[\+\-\d\:\{0}]+$", threadDecimalSeparator); //  Note that {0} is a string.Format positional parameter, not part of the regex pattern!

            // Create the validator for the supplied string argument and reject the string if necessary
            Regex validator = new Regex(validatorPattern);
            if (!validator.IsMatch(DMSString)) throw new InvalidValueException(string.Format("The supplied DMS string '{0}' contains an invalid character. (This application expects '{1}' as the decimal separator.)", DMSString, threadDecimalSeparator));

            DMSString = DMSString.Trim(); // Remove any leading or trailing spaces

            // Extract the sign of the coordinate if supplied
            if (DMSString.Substring(0, 1) == "-") sign = -1.0; // Found a leading minus sign so this is a negative value
            else sign = 1; // Assume that no sign or a leading plus sign indicates positive value

            // Based on an idea by Chris Rowland to use regular expressions to extract the number components
            // Create a match pattern containing the digits 0..9 and the thread's decimal separator to ensure that integer and real numbers are detected
            string regexPattern = string.Format(@"[\d\{0}]+", threadDecimalSeparator); // Create the regex pattern including the thread's decimal separator. Note that {0} is a string.Format positional parameter, not part of the regex pattern!
            Regex rx = new Regex(regexPattern); // Create a new regular expression based on the pattern

            // Find all number groups in the supplied sexagesimal string that match the pattern
            MatchCollection ms = rx.Matches(DMSString);
            if (ms.Count > 0) // We have at least a degrees value
            {
                returnValue = Convert.ToDouble(ms[0].Value); // Include the degrees value
                Debug.WriteLine("Degrees: {0}", ms[0].Value);
                if (ms.Count > 1) // We have at least minutes and possibly seconds to deal with 
                {
                    returnValue += (Convert.ToDouble(ms[1].Value) / 60.0); // Include the minutes value
                    Debug.WriteLine("Minutes: {0}", ms[1].Value);
                    if (ms.Count > 2)
                    {
                        returnValue += (Convert.ToDouble(ms[2].Value) / 3600.0);// We have a seconds value so include this as well
                        Debug.WriteLine("Seconds: {0}", ms[2].Value);
                    }
                }
            }

            returnValue = sign * returnValue; // Apply the sign to create the correct return value
            return returnValue; // Return the decimal equivalent of the supplied sexagesimal string
        }

        /// <summary>
        /// Convert a coordinate expressed in sexagesimal degrees to a double value (hours)
        /// </summary>
        /// <param name="DMSString">The coordinate expressed in sexagesimal notation (Degrees:Minutes:Seconds)</param>
        /// <exception cref="InvalidValueException">If the supplied coordinate contains an invalid character. Valid characters are: "+", "-", "0..9", ":" and "the decimal separator used by the current thread".</exception>
        /// <returns>The supplied coordinate as a double value (hours)</returns>
        /// <remarks>
        /// <para>
        /// Only the first three numeric components are considered, the remainder are ignored. Left to right positionally, the components are interpreted as degrees, minutes, and seconds. 
        /// If only two components are present, they are assumed to be degrees and minutes, and if only one components is present, it is assumed to be degrees. Any components can have a fractional part. 
        /// </para>
        /// <para>Examples of valid input values: 127:27:45, 12:26, +345, -45:34:12</para>
        /// <para>Examples of valid input values in a locale where point is used as the decimal separator: 60:27:45.846, 12:1.349, +345.1840746, -45:34:12.422</para>
        /// <para>Examples of valid input values in a locale where comma is used as the decimal separator: 60:27:45,846, 12:1,349, +345,1840746, -45:34:12,422</para>
        /// </remarks>
        public double DMSToHours(string DMSString)
        {
            return DMSToDegrees(DMSString) / 15.0;
        }

        /// <summary>
        /// Convert a coordinate expressed in sexagesimal hours to a double value (hours)
        /// </summary>
        /// <param name="HMSString">The coordinate expressed in sexagesimal notation (Hours:Minutes:Seconds)</param>
        /// <exception cref="InvalidValueException">If the supplied coordinate contains an invalid character. Valid characters are: "+", "-", "0..9", ":" and "the decimal separator used by the current thread".</exception>
        /// <returns>The supplied coordinate as a double value (hours)</returns>
        /// <remarks>
        /// <para>
        /// Only the first three numeric components are considered, the remainder are ignored. Left to right positionally, the components are interpreted as hours, minutes, and seconds. 
        /// If only two components are present, they are assumed to be hours and minutes, and if only one components is present, it is assumed to be hours. Any components can have a fractional part. 
        /// </para>
        /// <para>Examples of valid input values: 11:27:45, 12:26, +3, -5:34:12</para>
        /// <para>Examples of valid input values in a locale where point is used as the decimal separator: 6:27:45.846, 7:1.349, +8.1840746, -5:34:12.422</para>
        /// <para>Examples of valid input values in a locale where comma is used as the decimal separator: 6:27:45,846, 7:1,349, +8,1840746, -5:34:12,422</para>
        /// </remarks>
        public double HMSToHours(string HMSString)
        {
            double returnValue = 0.0; // Return value
            double sign; // Sign of the supplied value

            // Get the decimal separator character that is used in this locale
            string threadDecimalSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            // Create a validation pattern to ensure that invalid characters are detected and the input rejected if necessary
            string validatorPattern = string.Format(@"^[\+\-\d\:\{0}]+$", threadDecimalSeparator); //  Note that {0} is a string.Format positional parameter, not part of the regex pattern!

            // Create the validator for the supplied string argument and reject the string if necessary
            Regex validator = new Regex(validatorPattern);
            if (!validator.IsMatch(HMSString)) throw new InvalidValueException(string.Format("The supplied HMS string '{0}' contains an invalid character. (This application expects '{1}' as the decimal separator.)", HMSString, threadDecimalSeparator));

            HMSString = HMSString.Trim(); // Remove any leading or trailing spaces

            // Extract the sign of the coordinate if supplied
            if (HMSString.Substring(0, 1) == "-") sign = -1.0; // Found a leading minus sign so this is a negative value
            else sign = 1; // Assume that no sign or a leading plus sign indicates positive value

            // Based on an idea by Chris Rowland to use regular expressions to extract the number components
            // Create a match pattern containing the digits 0..9 and the thread's decimal separator to ensure that integer and real numbers are detected
            string regexPattern = string.Format(@"[\d\{0}]+", threadDecimalSeparator); // Create the regex pattern including the thread's decimal separator. Note that {0} is a string.Format positional parameter, not part of the regex pattern!
            Regex rx = new Regex(regexPattern); // Create a new regular expression based on the pattern

            // Find all number groups in the supplied sexagesimal string that match the pattern
            MatchCollection ms = rx.Matches(HMSString);
            if (ms.Count > 0) // We have at least a degrees value
            {
                returnValue = Convert.ToDouble(ms[0].Value); // Include the degrees value
                Debug.WriteLine("Hours: {0}", ms[0].Value);
                if (ms.Count > 1) // We have at least minutes and possibly seconds to deal with 
                {
                    returnValue += (Convert.ToDouble(ms[1].Value) / 60.0); // Include the minutes value
                    Debug.WriteLine("Minutes: {0}", ms[1].Value);
                    if (ms.Count > 2)
                    {
                        returnValue += (Convert.ToDouble(ms[2].Value) / 3600.0);// We have a seconds value so include this as well
                        Debug.WriteLine("Seconds: {0}", ms[2].Value);
                    }
                }
            }

            returnValue = sign * returnValue; // Apply the sign to create the correct return value
            return returnValue; // Return the decimal equivalent of the supplied sexagesimal string        }
        }

        /// <summary>
        /// Convert a coordinate expressed in sexagesimal hours to a double value (degrees)
        /// </summary>
        /// <param name="HMS">The coordinate expressed in sexagesimal notation (Hours:Minutes:Seconds)</param>
        /// <exception cref="InvalidValueException">If the supplied coordinate contains an invalid character. Valid characters are: "+", "-", "0..9", ":" and "the decimal separator used by the current thread".</exception>
        /// <returns>The supplied coordinate as a double value (hours)</returns>
        /// <remarks>
        /// <para>
        /// Only the first three numeric components are considered, the remainder are ignored. Left to right positionally, the components are interpreted as hours, minutes, and seconds. 
        /// If only two components are present, they are assumed to be hours and minutes, and if only one components is present, it is assumed to be hours. Any components can have a fractional part. 
        /// </para>
        /// <para>Examples of valid input values: 11:27:45, 12:26, +3, -5:34:12</para>
        /// <para>Examples of valid input values in a locale where point is used as the decimal separator: 6:27:45.846, 7:1.349, +8.1840746, -5:34:12.422</para>
        /// <para>Examples of valid input values in a locale where comma is used as the decimal separator: 6:27:45,846, 7:1,349, +8,1840746, -5:34:12,422</para>
        /// </remarks>
        public double HMSToDegrees(string HMS)
        {
            return HMSToHours(HMS) * 15.0;
        }

        #endregion

        #region DegreesToDMS Conversions

        // 
        // Convert a real number to sexagesimal whole, minutes, seconds. Allow
        // specifying the number of decimal digits on seconds. Called by
        // HoursToHMS below, which just has different default delimiters.
        // 
        /// <summary>
        /// Convert degrees to sexagesimal degrees, minutes and seconds with default delimiters DD° MM' SS" 
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <returns>Sexagesimal representation of degrees input value, degrees, minutes, and seconds</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single 
        /// characters.</para>
        /// <para>This overload is not available through COM, please use 
        /// "DegreesToDMS(ByVal Degrees As Double, ByVal DegDelim As String, ByVal MinDelim As String, ByVal SecDelim As String)"
        /// with suitable parameters to achieve this effect.</para>
        /// </remarks>
        public string DegreesToDMS(double Degrees)
        {
            return DoubleToSexagesimalSeconds(Degrees, @":", @":", @"", 0);
        }

        /// <summary>
        ///  Convert degrees to sexagesimal degrees, minutes and seconds with specified second decimal places
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <param name="DegDelim">The delimiter string separating degrees and minutes </param>
        /// <param name="MinDelim">The delimiter string to append to the minutes part </param>
        /// <param name="SecDelim">The delimiter string to append to the seconds part</param>
        /// <param name="SecDecimalDigits">The number of digits after the decimal point on the seconds part </param>
        /// <returns>Sexagesimal representation of degrees input value, degrees, minutes, and seconds</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single 
        /// characters.</para>
        /// </remarks>
        public string DegreesToDMS(double Degrees, string DegDelim, string MinDelim, string SecDelim, int SecDecimalDigits)
        {

            return DoubleToSexagesimalSeconds(Degrees, DegDelim, MinDelim, SecDelim, SecDecimalDigits);
        }

        #endregion

        #region DegreesToDM Conversions

        /// <summary>
        /// Convert degrees to sexagesimal degrees and minutes with default delimiters DD° MM'
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <returns>Sexagesimal representation of degrees input value, as degrees and minutes</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters.</para>
        /// <para>This overload is not available through COM, please use 
        /// "DegreesToDM(ByVal Degrees As Double, ByVal DegDelim As String, ByVal MinDelim As String, ByVal MinDecimalDigits As Integer)"
        /// with suitable parameters to achieve this effect.</para>
        /// </remarks>
        public string DegreesToDM(double Degrees)
        {
            return DoubleToSexagesimalMinutes(Degrees, @":", @"", 0);
        }

        /// <summary>
        /// Convert degrees to sexagesimal degrees and minutes with the specified number of minute decimal places
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <param name="DegDelim">The delimiter string separating degrees </param>
        /// <param name="MinDelim">The delimiter string to append to the minutes </param>
        /// <param name="MinDecimalDigits">The number of digits after the decimal point on the minutes part </param>
        /// <returns>Sexagesimal representation of degrees input value, as degrees and minutes</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters.</para>
        /// </remarks>
        public string DegreesToDM(double Degrees, string DegDelim, string MinDelim, int MinDecimalDigits)
        {
            return DoubleToSexagesimalMinutes(Degrees, DegDelim, MinDelim, MinDecimalDigits);
        }

        #endregion

        #region DegreesToHMS Conversions

        /// <summary>
        /// Convert degrees to sexagesimal hours, minutes, and seconds with default delimiters of HH:MM:SS
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <returns>Sexagesimal representation of degrees input value, as hours, minutes, and seconds</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself.</para>
        /// <para>This overload is not available through COM, please use 
        /// "DegreesToHMS(ByVal Degrees As Double, ByVal HrsDelim As String, ByVal MinDelim As String, ByVal SecDelim As String, ByVal SecDecimalDigits As Integer)"
        /// with suitable parameters to achieve this effect.</para>
        /// </remarks>
        public string DegreesToHMS(double Degrees)
        {
            return DoubleToSexagesimalSeconds(Degrees / 15.0, @":", @":", @"", 0);
        }

        /// <summary>
        /// Convert degrees to sexagesimal hours, minutes, and seconds with the specified number of second decimal places
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <param name="HrsDelim">The delimiter string separating hours and minutes</param>
        /// <param name="MinDelim">The delimiter string separating minutes and seconds</param>
        /// <param name="SecDelim">The delimiter string to append to the seconds part </param>
        /// <param name="SecDecimalDigits">The number of digits after the decimal point on the seconds part </param>
        /// <returns>Sexagesimal representation of degrees input value, as hours, minutes, and seconds</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters. </para>
        /// </remarks>
        public string DegreesToHMS(double Degrees, string HrsDelim, string MinDelim, string SecDelim, int SecDecimalDigits)
        {
            return DoubleToSexagesimalSeconds(Degrees / 15.0, HrsDelim, MinDelim, SecDelim, SecDecimalDigits);
        }

        #endregion

        #region DegreesToHM Conversions

        /// <summary>
        /// Convert degrees to sexagesimal hours and minutes with default delimiters HH:MM
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <returns>Sexagesimal representation of degrees input value as hours and minutes</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters</para>
        /// <para>This overload is not available through COM, please use 
        /// "DegreesToHM(ByVal Degrees As Double, ByVal HrsDelim As String, ByVal MinDelim As String, ByVal MinDecimalDigits As Integer)"
        /// with suitable parameters to achieve this effect.</para>
        /// </remarks>
        public string DegreesToHM(double Degrees)
        {
            return DoubleToSexagesimalMinutes(Degrees / 15.0, @":", @"", 0);
        }

        /// <summary>
        /// Convert degrees to sexagesimal hours and minutes with supplied number of minute decimal places
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <param name="HrsDelim">The delimiter string separating hours and minutes</param>
        /// <param name="MinDelim">The delimiter string to append to the minutes part</param>
        /// <param name="MinDecimalDigits">Number of minutes decimal places</param>
        /// <returns>Sexagesimal representation of degrees input value as hours and minutes</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters</para>
        /// </remarks>
        public string DegreesToHM(double Degrees, string HrsDelim, string MinDelim, int MinDecimalDigits)
        {
            return DoubleToSexagesimalMinutes(Degrees / 15.0, HrsDelim, MinDelim, MinDecimalDigits);
        }

        #endregion

        #region HoursToHMS Conversions

        /// <summary>
        /// Convert hours to sexagesimal hours, minutes, and seconds with default delimiters HH:MM:SS
        /// </summary>
        /// <param name="Hours">The hours value to convert</param>
        /// <returns>Sexagesimal representation of hours input value, hours, minutes and seconds</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters.</para>
        /// <para>This overload is not available through COM, please use 
        /// "HoursToHMS(ByVal Hours As Double, ByVal HrsDelim As String, ByVal MinDelim As String, ByVal SecDelim As String, ByVal SecDecimalDigits As Integer)"
        /// with suitable parameters to achieve this effect.</para>
        /// </remarks>
        public string HoursToHMS(double Hours)
        {
            return DoubleToSexagesimalSeconds(Hours, @":", @":", @"", 0);
        }

        /// <summary>
        /// Convert hours to sexagesimal hours, minutes, and seconds with specified number of second decimal places
        /// </summary>
        /// <param name="Hours">The hours value to convert</param>
        /// <param name="HrsDelim">The delimiter string separating hours and minutes </param>
        /// <param name="MinDelim">The delimiter string separating minutes and seconds </param>
        /// <param name="SecDelim">The delimiter string to append to the seconds part </param>
        /// <param name="SecDecimalDigits">The number of digits after the decimal point on the seconds part </param>
        /// <returns>Sexagesimal representation of hours input value, hours, minutes and seconds</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters.</para>
        /// </remarks>
        public string HoursToHMS(double Hours, string HrsDelim, string MinDelim, string SecDelim, int SecDecimalDigits)
        {
            return DoubleToSexagesimalSeconds(Hours, HrsDelim, MinDelim, SecDelim, SecDecimalDigits);
        }

        #endregion

        #region HoursToHM Conversions

        /// <summary>
        /// Convert hours to sexagesimal hours and minutes with default delimiters HH:MM
        /// </summary>
        /// <param name="Hours">The hours value to convert</param>
        /// <returns>Sexagesimal representation of hours input value as hours and minutes</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters.</para>
        /// <para>This overload is not available through COM, please use 
        /// "HoursToHM(ByVal Hours As Double, ByVal HrsDelim As String, ByVal MinDelim As String, ByVal MinDecimalDigits As Integer)"
        /// with an suitable parameters to achieve this effect.</para>
        /// </remarks>
        public string HoursToHM(double Hours)
        {
            return DoubleToSexagesimalMinutes(Hours, @":", @"", 0);
        }

        /// <summary>
        /// Convert hours to sexagesimal hours and minutes with supplied number of minute decimal places
        /// </summary>
        /// <param name="Hours">The hours value to convert</param>
        /// <param name="HrsDelim">The delimiter string separating hours </param>
        /// <param name="MinDelim">The delimiter string to append to the minutes part </param>
        /// <param name="MinDecimalDigits">The number of digits after the decimal point on the minutes part </param>
        /// <returns>Sexagesimal representation of hours input value as hours and minutes</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters.</para>
        /// </remarks>
        public string HoursToHM(double Hours, string HrsDelim, string MinDelim, int MinDecimalDigits)
        {
            return DoubleToSexagesimalMinutes(Hours, HrsDelim, MinDelim, MinDecimalDigits);
        }

        #endregion

        #region Julian Date Functions

        /// <summary>
        /// Convert a Julian date to a UTC DateTime value on the Gregorian time scale
        /// </summary>
        /// <param name="JD">Julian date</param>
        /// <returns>UTC DateTime value corresponding to the supplied Julian date</returns>
        /// <exception cref="InvalidValueException">If the Julian date corresponds to a date before 15th October 1582 when the Gregorian calendar was introduced.</exception>
        /// <remarks>
        /// <para>Julian dates are always in UTC.</para>
        /// <para>The algorithm is from the Explanatory Supplement to the Astronomical Almanac 3rd Edition 2013 edited by Urban and Seidelmann pages 617-619 and has been validated against
        /// the USNO Julian date calculator at https://aa.usno.navy.mil/data/docs/JulianDate.php </para>
        /// </remarks>
        public DateTime JulianDateToDateTime(double JD)
        {
            // The algorithm employed here is taken from the Explanatory Supplement to the USNO/HMNAO Astronomical Almanac 3rd Edition 2013 edited by Urban and Seidelmann, pages 617 - 619.
            // This implementation has been validated against the USNO Julian date calculator at https://aa.usno.navy.mil/data/docs/JulianDate.php 

            // Validate the incoming Julian date because it is not possible for a DateTime value to represent a date/time earlier that 00:00:00 1st January 0001
            if (JD < JULIAN_DAY_WHEN_GREGORIAN_CALENDAR_WAS_INTRODUCED) throw new InvalidValueException($"JulianDateToDateTime: The supplied Julian date {JD} precedes introduction of the Gregorian calendar on 15th October 1582.");

            // Defined constants for the Gregorian calendar, taken from the Explanatory Supplement to the Astronomical Almanac.
            const int y = 4716;
            const int j = 1401;
            const int m = 2;
            const int n = 12;
            const int r = 4;
            const int p = 1461;
            const int v = 3;
            const int u = 5;
            const int s = 153;
            const int w = 2;
            const int B = 274277;
            const int C = -38;

            // Calculate the Gregorian year, month and day corresponding to the supplied Julian date using the Explanatory Supplement formulae

            int JDInt = (int)Math.Floor(JD); // Extract the integer part of the supplied Julian date
            decimal JDFraction = (decimal)JD % 1.0M; // Extract the fractional part of the supplied Julian date, using a decimal type for the fractional part to increase precision

            // Correct for the half day offset inherent in the Julian day system
            if (JDFraction >= 0.5M) // We are in the following Gregorian day even though we are still in the same Julian day!
            {
                JDInt += 1; // Increase the Julian day number by one in order to calculate the correct Gregorian day
                JDFraction -= 1.0M; // Decrease the fractional part by 1.0 to ensure that the day fraction is always less that 1.0 in the calculation below
            }

            int f = JDInt + j; // Equation 1
            f = f + (((4 * JDInt + B) / 146097) * 3) / 4 + C; // Equation 1a
            int e = r * f + v; // Equation 2
            int g = (e % p) / r; // Equation 3
            int h = u * g + w; // Equation 4
            int D = ((h % s) / u) + 1; // Equation 5 - Day number
            int M = (((h / s) + m) % n) + 1; // Equation 6 - Month number
            int Y = e / p - y + ((n + m - M) / n); // Equation 7 - Year number

            // Calculate the Julian date time components: hours, minutes, seconds and milliseconds
            double dayFraction = (double)JDFraction + 0.5; // Half a day is added here because a Julian day starts at 12:00 mid-day rather than 00:00 midnight. 
            int H = (int)(dayFraction * 24.0);

            double hourFraction = dayFraction * 24.0 - H;
            int MIN = (int)(hourFraction * 60.0);

            double minuteFraction = hourFraction * 60 - MIN;
            int S = (int)(minuteFraction * 60.0);

            double secondFraction = minuteFraction * 60 - S;
            int MS = (int)(secondFraction * 1000.0);

            return new DateTime(Y, M, D, H, MIN, S, MS, DateTimeKind.Utc);
        }

        
        /// <summary>
        /// Calculate the Julian date from a provided DateTime value
        /// </summary>
        /// <param name="ObservationDateTime">DateTime in UTC</param>
        /// <returns>Julian date</returns>
        /// <remarks>Julian dates should always be in UTC </remarks>
        public double JulianDateFromDateTime(DateTime ObservationDateTime)
        {
            // The algorithm employed here is taken from the Explanatory Supplement to the USNO/HMNAO Astronomical Almanac 3rd Edition 2013 edited by Urban and Seidelmann, pages 617 - 619.
            // This implementation has been validated against the USNO Julian date calculator at : https://aa.usno.navy.mil/data/docs/JulianDate.php 

            // Validate the supplied date / time to make sure it is on or after introduction of the Gregorian calendar in 15th October 1582
            if (ObservationDateTime < GREGORIAN_CALENDAR_INTRODUCTION) throw new InvalidValueException($"JulianDateToDateTime: The supplied date {ObservationDateTime.ToString("hh:mm:ss.fff dd MMMM yyyy")} precedes introduction of the Gregorian calendar on 18th October 1582.");

            // Defined constants for the Gregorian calendar, taken from the Explanatory Supplement to the Astronomical Almanac.
            const int y = 4716;
            const int j = 1401;
            const int m = 2;
            const int n = 12;
            const int r = 4;
            const int p = 1461;
            const int q = 0;
            const int u = 5;
            const int s = 153;
            const int t = 2;
            const int A = 184;
            const int C = -38;

            int D = ObservationDateTime.Day;
            int M = ObservationDateTime.Month;
            int Y = ObservationDateTime.Year;

            int h = M - m; // Equation 1
            int g = Y + y - (n - h) / n;
            int f = (h - 1 + n) % n;
            int e = (p * g + q) / r + D - 1 - j;
            int J = e + (s * f + t) / u;
            J = J - (3 * ((g + A) / 100)) / 4 - C;

            // J is the number of the Julian day that starts at 12:00 on the supplied Gregorian observation date/time. 
            // If the time of the observation is earlier that 12:00 then the Julian day will actually be the preceding day, one less than calculated.

            double dayFraction = ObservationDateTime.TimeOfDay.TotalDays; // Calculate the day fraction corresponding of the Gregorian observation date/time i.e. 00:00:00 is dayFraction 0.0 and 18:00:00 is dayFraction 0.75

            if (dayFraction < 0.5) // We are actually in the preceding Julian day
            {
                J -= 1; // Decrement the Julian day number
                dayFraction += 1.0; // Increment the day fraction to compensate
            }

            // Now compensate for the fact that Julian days start at 12:00:00 on each Gregorian day
            dayFraction -= 0.5;

            return (double)J + dayFraction;

            //return DateTime.ToOADate() + 2415018.5;
        }

        /// <summary>
        /// Current Julian date based on the UTC time scale
        /// </summary>
        /// <returns>Current Julian date on the UTC time scale</returns>
        /// <remarks></remarks>
        public double JulianDateUtc
        {
            get
            {
                return JulianDateFromDateTime(DateTime.UtcNow);
            }
        }

        #endregion

        #region ObservingConditions Property Conversions

        /// <summary>
        /// Convert from one set of speed / temperature / pressure rain rate units to another
        /// </summary>
        /// <param name="InputValue">Value to convert</param>
        /// <param name="FromUnits">Integer value from the Units enum indicating the value's current units</param>
        /// <param name="ToUnits">Integer value from the Units enum indicating the units to which the input value should be converted</param>
        /// <returns>Input value expressed in the new units</returns>
        /// <exception cref="InvalidOperationException">When the specified from and to units can not refer to the same value. e.g. attempting to convert miles per hour to degrees Celsius</exception>
        /// <remarks>
        /// <para>Conversions available:</para>
        /// <list type="bullet">
        /// <item>metres per second &lt;==&gt; miles per hour &lt;==&gt; knots</item>
        /// <item>Celsius &lt;==&gt; Fahrenheit &lt;==&gt; Kelvin</item>
        /// <item>hecto Pascals (hPa) &lt;==&gt; milli bar(mbar) &lt;==&gt; mm of mercury &lt;==&gt; inches of mercury</item>
        /// <item>mm per hour &lt;==&gt; inches per hour</item>
        /// </list>
        /// <para>Knots conversions use the international nautical mile definition (1 nautical mile = 1852m) rather than the original UK or US Admiralty definitions.</para>
        /// <para>For convenience, milli bar and hecto Pascals are shown as separate units even though they have numerically identical values and there is a 1:1 conversion between them.</para>
        /// </remarks>
        public double ConvertUnits(double InputValue, Unit FromUnits, Unit ToUnits)
        {
            double intermediateValue, finalValue;

            if ((FromUnits >= Unit.metresPerSecond) & (FromUnits <= Unit.knots) & (ToUnits >= Unit.metresPerSecond) & (ToUnits <= Unit.knots))
            {
                // First convert the input to metres per second
                switch (FromUnits)
                {
                    case Unit.metresPerSecond:
                        {
                            intermediateValue = InputValue;
                            break;
                        }

                    case Unit.milesPerHour:
                        {
                            intermediateValue = InputValue * 0.44704;
                            break;
                        }

                    case Unit.knots:
                        {
                            intermediateValue = InputValue * 0.514444444;
                            break;
                        }

                    default:
                        {
                            throw new InvalidValueException("Unknown \"From\" speed units: " + FromUnits.ToString());
                        }
                }

                // Now convert metres per second to the output value
                switch (ToUnits)
                {
                    case Unit.metresPerSecond:
                        {
                            finalValue = intermediateValue;
                            break;
                        }

                    case Unit.milesPerHour:
                        {
                            finalValue = intermediateValue / 0.44704;
                            break;
                        }

                    case Unit.knots:
                        {
                            finalValue = intermediateValue / 0.514444444;
                            break;
                        }

                    default:
                        {
                            throw new InvalidValueException("Unknown \"To\" speed units: " + ToUnits.ToString());
                        }
                }

                return finalValue;
            }
            else if ((FromUnits >= Unit.degreesCelsius) & (FromUnits <= Unit.degreesKelvin) & (ToUnits >= Unit.degreesCelsius) & (ToUnits <= Unit.degreesKelvin))
            {

                // First convert the input to degrees K
                switch (FromUnits)
                {
                    case Unit.degreesCelsius:
                        {
                            intermediateValue = InputValue - ABSOLUTE_ZERO_CELSIUS;
                            break;
                        }

                    case Unit.degreesFarenheit:
                        {
                            intermediateValue = ((InputValue + 459.67) * 5.0) / 9.0;
                            break;
                        }

                    case Unit.degreesKelvin:
                        {
                            intermediateValue = InputValue;
                            break;
                        }

                    default:
                        {
                            throw new InvalidValueException("Unknown \"From\" temperature units: " + FromUnits.ToString());
                        }
                }

                // Now convert degrees K to the output value
                switch (ToUnits)
                {
                    case Unit.degreesCelsius:
                        {
                            finalValue = intermediateValue + ABSOLUTE_ZERO_CELSIUS;
                            break;
                        }

                    case Unit.degreesFarenheit:
                        {
                            finalValue = ((intermediateValue * 9.0) / 5.0) - 459.67;
                            break;
                        }

                    case Unit.degreesKelvin:
                        {
                            finalValue = intermediateValue;
                            break;
                        }

                    default:
                        {
                            throw new InvalidValueException("Unknown \"To\" temperature units: " + ToUnits.ToString());
                        }
                }

                return finalValue;
            }
            else if ((FromUnits >= Unit.hPa) & (FromUnits <= Unit.inHg) & (ToUnits >= Unit.hPa) & (ToUnits <= Unit.inHg))
            {
                // First convert the input to hPa
                switch (FromUnits)
                {
                    case Unit.hPa:
                        {
                            intermediateValue = InputValue;
                            break;
                        }

                    case Unit.mBar:
                        {
                            intermediateValue = InputValue;
                            break;
                        }

                    case Unit.mmHg:
                        {
                            intermediateValue = InputValue * 1.33322368;
                            break;
                        }

                    case Unit.inHg:
                        {
                            intermediateValue = InputValue * 33.8638816;
                            break;
                        }

                    default:
                        {
                            throw new InvalidValueException("Unknown \"From\" pressure units: " + FromUnits.ToString());
                        }
                }

                // Now convert hPa to the output value
                switch (ToUnits)
                {
                    case Unit.hPa:
                        {
                            finalValue = intermediateValue;
                            break;
                        }

                    case Unit.mBar:
                        {
                            finalValue = intermediateValue;
                            break;
                        }

                    case Unit.mmHg:
                        {
                            finalValue = intermediateValue / 1.33322368;
                            break;
                        }

                    case Unit.inHg:
                        {
                            finalValue = intermediateValue / 33.8638816;
                            break;
                        }

                    default:
                        {
                            throw new InvalidValueException("Unknown \"To\" pressure units: " + ToUnits.ToString());
                        }
                }

                return finalValue;
            }
            else if ((FromUnits >= Unit.mmPerHour) & (FromUnits <= Unit.inPerHour) & (ToUnits >= Unit.mmPerHour) & (ToUnits <= Unit.inPerHour))
            {
                // First convert the input to mm
                switch (FromUnits)
                {
                    case Unit.mmPerHour:
                        {
                            intermediateValue = InputValue;
                            break;
                        }

                    case Unit.inPerHour:
                        {
                            intermediateValue = InputValue * 25.4;
                            break;
                        }

                    default:
                        {
                            throw new InvalidValueException("Unknown \"From\" rain rate units: " + FromUnits.ToString());
                        }
                }

                // Now convert mm to the output value
                switch (ToUnits)
                {
                    case Unit.mmPerHour:
                        {
                            finalValue = intermediateValue;
                            break;
                        }

                    case Unit.inPerHour:
                        {
                            finalValue = intermediateValue / 25.4;
                            break;
                        }

                    default:
                        {
                            throw new InvalidValueException("Unknown \"To\" rain rate units: " + ToUnits.ToString());
                        }
                }

                return finalValue;
            }
            else
                throw new ASCOM.InvalidOperationException("From and to units are not of the same type. From: " + FromUnits.ToString() + ", To: " + ToUnits.ToString());
        }

        /// <summary>
        /// Calculate the dew point (°Celsius) given the ambient temperature (°Celsius) and relative humidity (%)
        /// </summary>
        /// <param name="RelativeHumidity">Relative humidity expressed in percent (0.0 .. 100.0)</param>
        /// <param name="AmbientTemperature">Ambient temperature (°Celsius)</param>
        /// <returns>Dew point (°Celsius)</returns>
        /// <exception cref="InvalidOperationException">When relative humidity &lt; 0.0% or &gt; 100.0%></exception>
        /// <exception cref="InvalidOperationException">When ambient temperature &lt; absolute zero or &gt; 100.0C></exception>
        ///  <remarks>'Calculation uses Vaisala formula for water vapour saturation pressure and is accurate to 0.083% over -20C - +50°C
        /// <para>http://www.vaisala.com/Vaisala%20Documents/Application%20notes/Humidity_Conversion_Formulas_B210973EN-F.pdf </para>
        /// </remarks>
        public double Humidity2DewPoint(double RelativeHumidity, double AmbientTemperature)
        {
            // Formulae taken from Vaisala: http://www.vaisala.com/Vaisala%20Documents/Application%20notes/Humidity_Conversion_Formulas_B210973EN-F.pdf 
            double Pws, Pw, Td;

            // Constants from Vaisala document
            const double A = 6.116441;
            const double m = 7.591386;
            const double Tn = 240.7263;

            // Validate input values
            if ((RelativeHumidity < 0.0) | (RelativeHumidity > 100.0))
                throw new InvalidValueException("Humidity2DewPoint - Relative humidity is < 0.0% or > 100.0%: " + RelativeHumidity.ToString());
            if ((AmbientTemperature < ABSOLUTE_ZERO_CELSIUS) | (AmbientTemperature > 100.0))
                throw new InvalidValueException("Humidity2DewPoint - Ambient temperature is < " + ABSOLUTE_ZERO_CELSIUS + "C or > 100.0C: " + AmbientTemperature.ToString());

            Pws = A * Math.Pow(10.0, m * AmbientTemperature / (AmbientTemperature + Tn)); // Calculate water vapour saturation pressure, Pws, from Vaisala formula (6) - In hPa
            Pw = Pws * RelativeHumidity / 100.0; // Calculate measured vapour pressure, Pw
            Td = Tn / ((m / Math.Log10(Pw / A)) - 1.0); // Finally, calculate dew-point in °C

            return Td;
        }

        /// <summary>
        /// Calculate the relative humidity (%) given the ambient temperature (°Celsius) and dew point (°Celsius)
        /// </summary>
        /// <param name="DewPoint">Dewpoint in (°Celsius)</param>
        /// <param name="AmbientTemperature">Ambient temperature (°Celsius)</param>
        /// <returns>Humidity expressed in percent (0.0 .. 100.0)</returns>
        /// <exception cref="InvalidOperationException">When dew point &lt; absolute zero or &gt; 100.0C></exception>
        /// <exception cref="InvalidOperationException">When ambient temperature &lt; absolute zero or &gt; 100.0C></exception>
        /// <remarks>'Calculation uses the Vaisala formula for water vapour saturation pressure and is accurate to 0.083% over -20C - +50°C
        /// <para>http://www.vaisala.com/Vaisala%20Documents/Application%20notes/Humidity_Conversion_Formulas_B210973EN-F.pdf </para>
        /// </remarks>
        public double DewPoint2Humidity(double DewPoint, double AmbientTemperature)
        {
            // Formulae taken from Vaisala: http://www.vaisala.com/Vaisala%20Documents/Application%20notes/Humidity_Conversion_Formulas_B210973EN-F.pdf 
            double RH;

            // Constants from Vaisala document
            const double m = 7.591386;
            const double Tn = 240.7263;

            // Validate input values
            if ((DewPoint < ABSOLUTE_ZERO_CELSIUS) | (DewPoint > 100.0))
                throw new InvalidValueException("DewPoint2Humidity - Dew point is < " + ABSOLUTE_ZERO_CELSIUS + "C or > 100.0C: " + DewPoint.ToString());
            if ((AmbientTemperature < ABSOLUTE_ZERO_CELSIUS) | (AmbientTemperature > 100.0))
                throw new InvalidValueException("DewPoint2Humidity - Ambient temperature is < " + ABSOLUTE_ZERO_CELSIUS + "C or > 100.0C: " + AmbientTemperature.ToString());

            RH = 100.0 * Math.Pow(10.0, m * ((DewPoint / (DewPoint + Tn)) - (AmbientTemperature / (AmbientTemperature + Tn))));

            return RH;
        }

        /// <summary>
        /// Convert atmospheric pressure from one altitude above mean sea level to another
        /// </summary>
        /// <param name="Pressure">Measured pressure in hPa (mBar) at the "From" altitude</param>
        /// <param name="FromAltitudeAboveMeanSeaLevel">"Altitude at which the input pressure was measured (metres)</param>
        /// <param name="ToAltitudeAboveMeanSeaLevel">Altitude to which the pressure is to be converted (metres)</param>
        /// <returns>Pressure in hPa at the "To" altitude</returns>
        /// <remarks>Uses the equation: p = p0 * (1.0 - 2.25577E-05 h)^5.25588</remarks>
        public double ConvertPressure(double Pressure, double FromAltitudeAboveMeanSeaLevel, double ToAltitudeAboveMeanSeaLevel)
        {
            // Convert supplied pressure to sea level then convert again to the required altitude using this equation: p = p0 (1 - 2.25577 10-5 h)5.25588

            double SeaLevelPressure, ActualPressure;

            SeaLevelPressure = Pressure / Math.Pow(1.0 - 0.0000225577 * FromAltitudeAboveMeanSeaLevel, 5.25588);
            ActualPressure = SeaLevelPressure * Math.Pow(1.0 - 0.0000225577 * ToAltitudeAboveMeanSeaLevel, 5.25588);

            return ActualPressure;
        }

        #endregion

        #region Coordinate Validation and Management Functions

        /// <summary>
        /// Flexible routine to range a number into a given range between a lower and an higher bound.
        /// </summary>
        /// <param name="Value">Value to be ranged</param>
        /// <param name="LowerBound">Lowest value of the range</param>
        /// <param name="LowerEqual">Boolean flag indicating whether the ranged value can have the lower bound value</param>
        /// <param name="UpperBound">Highest value of the range</param>
        /// <param name="UpperEqual">Boolean flag indicating whether the ranged value can have the upper bound value</param>
        /// <returns>The ranged number as a double</returns>
        /// <exception cref="InvalidOperationException">Thrown if the lower bound is greater than the upper bound.</exception>
        /// <exception cref="InvalidOperationException">Thrown if LowerEqual and UpperEqual are both false and the ranged value equals
        /// one of these values. This is impossible to handle as the algorithm will always violate one of the rules!</exception>
        /// <remarks>
        /// UpperEqual and LowerEqual switches control whether the ranged value can be equal to either the upper and lower bounds. So, 
        /// to range an hour angle into the range 0 to 23.999999.. hours, use this call: 
        /// <code>RangedValue = Range(InputValue, 0.0, True, 24.0, False)</code>
        /// <para>The input value will be returned in the range where 0.0 is an allowable value and 24.0 is not i.e. in the range 0..23.999999..</para>
        /// <para>It is not permissible for both LowerEqual and UpperEqual to be false because it will not be possible to return a value that is exactly equal 
        /// to either lower or upper bounds. An exception is thrown if this scenario is requested.</para>
        /// </remarks>
        public double Range(double Value, double LowerBound, bool LowerEqual, double UpperBound, bool UpperEqual)
        {
            double ModuloValue;
            if (LowerBound >= UpperBound)
                throw new ASCOM.InvalidValueException("Range - LowerBound is >= UpperBound LowerBound must be less than UpperBound");

            ModuloValue = UpperBound - LowerBound;

            if (LowerEqual)
            {
                if (UpperEqual)
                {
                    do
                    {
                        if (Value < LowerBound)
                            Value += ModuloValue;
                        if (Value > UpperBound)
                            Value -= ModuloValue;
                    }
                    while (!(Value >= LowerBound) & (Value <= UpperBound));
                }
                else
                    do
                    {
                        if (Value < LowerBound)
                            Value += ModuloValue;
                        if (Value >= UpperBound)
                            Value -= ModuloValue;
                    }
                    while (!(Value >= LowerBound) & (Value < UpperBound));
            }
            else if (UpperEqual)
            {
                do
                {
                    if (Value <= LowerBound)
                        Value += ModuloValue;
                    if (Value > UpperBound)
                        Value -= ModuloValue;
                }
                while (!(Value > LowerBound) & (Value <= UpperBound));
            }
            else
            {
                if ((Value == LowerBound))
                    throw new InvalidValueException("Range - The supplied value equals the LowerBound. This can not be ranged when LowerEqual and UpperEqual are both false.");
                if ((Value == UpperBound))
                    throw new InvalidValueException("Range - The supplied value equals the UpperBound. This can not be ranged when LowerEqual and UpperEqual are both false.");
                do
                {
                    if (Value <= LowerBound)
                        Value += ModuloValue;
                    if (Value >= UpperBound)
                        Value -= ModuloValue;
                }
                while (!(Value > LowerBound) & (Value < UpperBound));
            }
            return Value;
        }

        /// <summary>
        /// Conditions an hour angle to be in the range -12.0 to +12.0 by adding or subtracting 24.0 hours
        /// </summary>
        /// <param name="HA">Hour angle to condition</param>
        /// <returns>Hour angle in the range -12.0 to +12.0</returns>
        /// <remarks></remarks>
        public double ConditionHA(double HA)
        {
            double ReturnValue;

            ReturnValue = Range(HA, -12.0, true, +12.0, true);
            //TL.LogMessage("ConditionHA", "Conditioned HA: " + Utl.HoursToHMS(HA, ":", ":", "", 3) + " to: " + Utl.HoursToHMS(ReturnValue, ":", ":", "", 3));

            return ReturnValue;
        }

        /// <summary>
        /// Conditions a Right Ascension value to be in the range 0 to 23.999999.. hours 
        /// </summary>
        /// <param name="RA">Right ascension to be conditioned</param>
        /// <returns>Right ascension in the range 0 to 23.999999...</returns>
        /// <remarks></remarks>
        public double ConditionRA(double RA)
        {
            double ReturnValue;

            ReturnValue = Range(RA, 0.0, true, 24.0, false);
            //TL.LogMessage("ConditionRA", "Conditioned RA: " + Utl.HoursToHMS(RA, ":", ":", "", 3) + " to: " + Utl.HoursToHMS(ReturnValue, ":", ":", "", 3));

            return ReturnValue;
        }

        #endregion

        #region Private support code

        private static string DoubleToSexagesimalSeconds(double Units, string DegDelim, string MinDelim, string SecDelim, int SecDecimalDigits)
        {
            string wholeUnits, wholeMinutes, seconds, secondsFormatString;
            bool inputIsNegative;

            // Convert the input value to a positive number if required and store the sign
            if (Units < 0.0)
            {
                Units = -Units;
                inputIsNegative = true;
            }
            else inputIsNegative = false;

            // Extract the number of whole units and save the remainder
            wholeUnits = Math.Floor(Units).ToString().PadLeft(2, '0');
            double remainderInMinutes = (Units - Convert.ToDouble(wholeUnits)) * 60.0; // Remainder in minutes

            // Extract the number of whole minutes and save the remainder
            wholeMinutes = Math.Floor(remainderInMinutes).ToString().PadLeft(2, '0');// Integral minutes
            double remainderInSeconds = (remainderInMinutes - System.Convert.ToDouble(wholeMinutes)) * 60.0; // Remainder in seconds

            if (SecDecimalDigits == 0) secondsFormatString = "00"; // No decimal point or decimal digits
            else secondsFormatString = "00." + new String('0', SecDecimalDigits); // Format$ string of form 00.0000

            seconds = remainderInSeconds.ToString(secondsFormatString); // Format seconds with requested number of decimal digits

            // Check to see whether rounding has pushed the number of whole seconds or minutes to 60
            if (seconds.Substring(0, 2) == "60") // The rounded seconds value is 60 so we need to add one to the minutes count and make the seconds 0
            {
                seconds = 0.0.ToString(secondsFormatString); // Seconds are 0.0 formatted as required
                wholeMinutes = (Convert.ToInt32(wholeMinutes) + 1).ToString("00"); // Add one to the to the minutes count
                if (wholeMinutes == "60")// The minutes value is 60 so we need to add one to the units count and make the minutes 0
                {
                    wholeMinutes = "00"; // Minutes are 0.0
                    wholeUnits = (Convert.ToInt32(wholeUnits) + 1).ToString("00"); // Add one to the units count
                }
            }

            // Create the full formatted string from the units, minute and seconds parts and add a leading negative sign if required
            string returnValue = wholeUnits + DegDelim + wholeMinutes + MinDelim + seconds + SecDelim;
            if (inputIsNegative) returnValue = $"-{returnValue}";

            return returnValue;
        }

        private static string DoubleToSexagesimalMinutes(double Units, string DegDelim, string MinDelim, int MinDecimalDigits)
        {
            string wholeUnits, minutes, minutesFormatString;
            bool inputIsNegative;

            // Convert the input value to a positive number if required and store the sign
            if (Units < 0.0)
            {
                Units = -Units;
                inputIsNegative = true;
            }
            else inputIsNegative = false;

            // Extract the number of whole units and save the remainder
            wholeUnits = Math.Floor(Units).ToString().PadLeft(2, '0');
            double remainderInMinutes = (Units - Convert.ToDouble(wholeUnits)) * 60.0; // Remainder in minutes

            if (MinDecimalDigits == 0) minutesFormatString = "00"; // No decimal point or decimal digits
            else minutesFormatString = "00." + new String('0', MinDecimalDigits); // Format$ string of form 00.0000

            minutes = remainderInMinutes.ToString(minutesFormatString); // Format minutes with requested number of decimal digits

            // Check to see whether rounding has pushed the number of whole minutes to 60
            if (minutes.Substring(0, 2) == "60") // The rounded minutes value is 60 so we need to add one to the units count and make the minutes 0
            {
                minutes = 0.0.ToString(minutesFormatString); // Minutes are 0.0 formatted as required
                wholeUnits = (Convert.ToInt32(wholeUnits) + 1).ToString("00"); // Add one to the to the units count
            }

            // Create the full formatted string from the units, minutes and seconds parts and add a leading negative sign if required
            string returnValue = wholeUnits + DegDelim + minutes + MinDelim;
            if (inputIsNegative) returnValue = $"-{returnValue}";

            return returnValue;
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Currently no resources to be disposed.
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose of the Utilities object
        /// </summary>
        /// <remarks>This method is present to implement the IDisposable pattern, which enables the Utilities component to be referenced within a Using statement.</remarks>
        public void Dispose()
        {
            // Do not change this code. Put clean-up code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

    }
}