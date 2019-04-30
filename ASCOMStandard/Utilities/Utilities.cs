using System;

namespace ASCOM.Alpaca
{
    /// <summary>
    /// ASCOM support utilities
    /// </summary>
    public class Utilities
    {
        /// <summary>
        /// List of units that can be converted by the ConvertUnits method
        /// </summary>
        public enum Units : int
        {
            // Speed
            /// <summary>
            /// Metres per second
            /// </summary>
            metresPerSecond = 0,
            /// <summary>
            /// Miles per hour
            /// </summary>
            milesPerHour = 1,
            /// <summary>
            /// Knots
            /// </summary>
            knots = 2,

            // Temperature
            /// <summary>
            /// Degrees Celsius
            /// </summary>
            degreesCelsius = 10,
            /// <summary>
            /// Degrees Fahrenheit
            /// </summary>
            degreesFarenheit = 11,
            /// <summary>
            /// Degrees kelvin
            /// </summary>
            degreesKelvin = 12,

            // Pressure
            /// <summary>
            /// Hecto pascals
            /// </summary>
            hPa = 20,
            /// <summary>
            /// Millibar
            /// </summary>
            mBar = 21,
            /// <summary>
            /// Millimetres of mercury
            /// </summary>
            mmHg = 22,
            /// <summary>
            /// Inches of mercury
            /// </summary>
            inHg = 23,

            // RainRate
            /// <summary>
            /// Millimetres per hour
            /// </summary>
            mmPerHour = 30,
            /// <summary>
            /// Inches per hour
            /// </summary>
            inPerHour = 31
        }

        // 
        // This will convert virtually anything resembling a sexagesimal
        // format number into a real number. The input may even be missing
        // the seconds or even the minutes part.
        // 
        /// <summary>
        /// Convert sexagesimal degrees to binary double-precision degrees
        /// </summary>
        /// <param name="DMS">The sexagesimal input string (degrees)</param>
        /// <returns>The double-precision binary value (degrees) represented by the sexagesimal input</returns>
        /// <remarks><para>The sexagesimal to real conversion methods such as this one are flexible enough to convert just 
        /// about anything that resembles sexagesimal. Thee way they operate is to first separate the input string 
        /// into numeric "tokens", strings consisting only of the numerals 0-9, plus and minus, and period. All other 
        /// characters are considered delimiters. Once the input string is parsed into tokens they are converted to 
        /// numerics. </para>
        /// <para>If there are more than three numeric tokens, only the first three are considered, the remainder 
        /// are ignored. Left to right positionally, the tokens are assumed to represent units (degrees or hours), 
        /// minutes, and seconds. If only two tokens are present, they are assumed to be units and minutes, and if 
        /// only one token is present, it is assumed to be units. Any token can have a fractional part. Of course it 
        /// would not be normal (for example) for both the minutes and seconds parts to have fractional parts, but it 
        /// would be legal. So 00:30.5:30 would convert to 1.0 unit. </para>
        /// <para>Note that plain units, for example 23.128734523 are acceptable to the method. </para>
        /// </remarks>
        public double DMSToDegrees(string DMS)
        {
            return 0.0;
        }

        /// <summary>
        /// Convert sexagesimal hours to binary double-precision hours
        /// </summary>
        /// <param name="HMS">The sexagesimal input string (hours)</param>
        /// <returns>The double-precision binary value (hours) represented by the sexagesimal input </returns>
        /// <remarks>
        /// <para>The sexagesimal to real conversion methods such as this one are flexible enough to convert just about 
        /// anything that resembles sexagesimal. Thee way they operate is to first separate the input string into 
        /// numeric "tokens", strings consisting only of the numerals 0-9, plus and minus, and period. All other 
        /// characters are considered delimiters. Once the input string is parsed into tokens they are converted to 
        /// numerics. </para>
        /// 
        /// <para>If there are more than three numeric tokens, only the first three are considered, the remainder 
        /// are ignored. Left to right positionally, the tokens are assumed to represent units (degrees or hours), 
        /// minutes, and seconds. If only two tokens are present, they are assumed to be units and minutes, and if 
        /// only one token is present, it is assumed to be units. Any token can have a fractional part. </para>
        /// 
        /// <para>Of course it would not be normal (for example) for both the minutes and seconds parts to have 
        /// fractional parts, but it would be legal. So 00:30.5:30 would convert to 1.0 unit. Note that plain units, 
        /// for example 23.128734523 are acceptable to the method. </para>
        /// </remarks>
        public double HMSToHours(string HMS)
        {
            return 0.0;
        }

        /// <summary>
        /// Convert sexagesimal hours to binary double-precision hours
        /// </summary>
        /// <param name="HMS">The sexagesimal input string (hours)</param>
        /// <returns>The double-precision binary value (hours) represented by the sexagesimal input</returns>
        /// <remarks>
        /// <para>The sexagesimal to real conversion methods such as this one are flexible enough to convert just about 
        /// anything that resembles sexagesimal. Thee way they operate is to first separate the input string into 
        /// numeric "tokens", strings consisting only of the numerals 0-9, plus and minus, and period. All other 
        /// characters are considered delimiters. Once the input string is parsed into tokens they are converted to 
        /// numerics. </para>
        /// 
        /// <para>If there are more than three numeric tokens, only the first three are considered, the remainder 
        /// are ignored. Left to right positionally, the tokens are assumed to represent units (degrees or hours), 
        /// minutes, and seconds. If only two tokens are present, they are assumed to be units and minutes, and if 
        /// only one token is present, it is assumed to be units. Any token can have a fractional part. </para>
        /// 
        /// <para>Of course it would not be normal (for example) for both the minutes and seconds parts to have 
        /// fractional parts, but it would be legal. So 00:30.5:30 would convert to 1.0 unit. Note that plain units, 
        /// for example 23.128734523 are acceptable to the method. </para>
        /// </remarks>
        public double HMSToDegrees(string HMS)
        {
            return 0.0;
        }

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
            return "";
        }

        /// <summary>
        ///  Convert degrees to sexagesimal degrees, minutes and seconds with default minute and second delimiters MM' SS" 
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <param name="DegDelim">The delimiter string separating degrees and minutes </param>
        /// <returns>Sexagesimal representation of degrees input value, degrees, minutes, and seconds</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single 
        /// characters.</para>
        /// <para>This overload is not available through COM, please use 
        /// "DegreesToDMS(ByVal Degrees As Double, ByVal DegDelim As String, ByVal MinDelim As String, ByVal SecDelim As String)"
        /// with suitable parameters to achieve this effect.</para>
        /// </remarks>
        public string DegreesToDMS(double Degrees, string DegDelim)
        {
            return "";
        }

        /// <summary>
        ///  Convert degrees to sexagesimal degrees, minutes and seconds with default second delimiter SS" 
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <param name="DegDelim">The delimiter string separating degrees and minutes </param>
        /// <param name="MinDelim">The delimiter string to append to the minutes part </param>
        /// <returns>Sexagesimal representation of degrees input value, degrees, minutes, and seconds</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single 
        /// characters.</para>
        /// <para>This overload is not available through COM, please use 
        /// "DegreesToDMS(ByVal Degrees As Double, ByVal DegDelim As String, ByVal MinDelim As String, ByVal SecDelim As String)"
        /// with suitable parameters to achieve this effect.</para>
        /// </remarks>
        public string DegreesToDMS(double Degrees, string DegDelim, string MinDelim)
        {
            return "";
        }

        /// <summary>
        ///  Convert degrees to sexagesimal degrees, minutes and seconds
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <param name="DegDelim">The delimiter string separating degrees and minutes </param>
        /// <param name="MinDelim">The delimiter string to append to the minutes part </param>
        /// <param name="SecDelim">The delimiter string to append to the seconds part</param>
        /// <returns>Sexagesimal representation of degrees input value, degrees, minutes, and seconds</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single 
        /// characters.</para>
        /// <para>This overload is not available through COM, please use 
        /// "DegreesToDMS(ByVal Degrees As Double, ByVal DegDelim As String, ByVal MinDelim As String, ByVal SecDelim As String)"
        /// with suitable parameters to achieve this effect.</para>
        /// </remarks>
        public string DegreesToDMS(double Degrees, string DegDelim, string MinDelim, string SecDelim)
        {
            return "";
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
            return "";
        }

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
            return "";
        }

        /// <summary>
        /// Convert hours to sexagesimal hours, minutes, and seconds with default minutes and seconds delimiters MM:SS
        /// </summary>
        /// <param name="Hours">The hours value to convert</param>
        /// <param name="HrsDelim">The delimiter string separating hours and minutes </param>
        /// <returns>Sexagesimal representation of hours input value, hours, minutes and seconds</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters.</para>
        /// <para>This overload is not available through COM, please use 
        /// "HoursToHMS(ByVal Hours As Double, ByVal HrsDelim As String, ByVal MinDelim As String, ByVal SecDelim As String, ByVal SecDecimalDigits As Integer)"
        /// with suitable parameters to achieve this effect.</para>
        /// </remarks>
        public string HoursToHMS(double Hours, string HrsDelim)
        {
            return "";
        }

        /// <summary>
        /// Convert hours to sexagesimal hours, minutes, and seconds with default second delimiter of null string
        /// </summary>
        /// <param name="Hours">The hours value to convert</param>
        /// <param name="HrsDelim">The delimiter string separating hours and minutes </param>
        /// <param name="MinDelim">The delimiter string separating minutes and seconds </param>
        /// <returns>Sexagesimal representation of hours input value, hours, minutes and seconds</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters.</para>
        /// <para>This overload is not available through COM, please use 
        /// "HoursToHMS(ByVal Hours As Double, ByVal HrsDelim As String, ByVal MinDelim As String, ByVal SecDelim As String, ByVal SecDecimalDigits As Integer)"
        /// with suitable parameters to achieve this effect.</para>
        /// </remarks>
        public string HoursToHMS(double Hours, string HrsDelim, string MinDelim)
        {
            return "";
        }

        /// <summary>
        /// Convert hours to sexagesimal hours, minutes, and seconds
        /// </summary>
        /// <param name="Hours">The hours value to convert</param>
        /// <param name="HrsDelim">The delimiter string separating hours and minutes </param>
        /// <param name="MinDelim">The delimiter string separating minutes and seconds </param>
        /// <param name="SecDelim">The delimiter string to append to the seconds part </param>
        /// <returns>Sexagesimal representation of hours input value, hours, minutes and seconds</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters.</para>
        /// <para>This overload is not available through COM, please use 
        /// "HoursToHMS(ByVal Hours As Double, ByVal HrsDelim As String, ByVal MinDelim As String, ByVal SecDelim As String, ByVal SecDecimalDigits As Integer)"
        /// with suitable parameters to achieve this effect.</para>
        /// </remarks>
        public string HoursToHMS(double Hours, string HrsDelim, string MinDelim, string SecDelim)
        {
            return "";
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
            return "";
        }

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
            return "";
        }

        /// <summary>
        /// Convert degrees to sexagesimal hours, minutes, and seconds with the default second and minute delimiters of MM:SS
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <param name="HrsDelim">The delimiter string separating hours and minutes</param>
        /// <returns>Sexagesimal representation of degrees input value, as hours, minutes, and seconds</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters. </para>
        /// <para>This overload is not available through COM, please use 
        /// "DegreesToHMS(ByVal Degrees As Double, ByVal HrsDelim As String, ByVal MinDelim As String, ByVal SecDelim As String, ByVal SecDecimalDigits As Integer)"
        /// with suitable parameters to achieve this effect.</para>
        /// </remarks>
        public string DegreesToHMS(double Degrees, string HrsDelim)
        {
            return "";
        }

        /// <summary>
        /// Convert degrees to sexagesimal hours, minutes, and seconds with the default second delimiter SS (null string)
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <param name="HrsDelim">The delimiter string separating hours and minutes</param>
        /// <param name="MinDelim">The delimiter string separating minutes and seconds</param>
        /// <returns>Sexagesimal representation of degrees input value, as hours, minutes, and seconds</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters. </para>
        /// <para>This overload is not available through COM, please use 
        /// "DegreesToHMS(ByVal Degrees As Double, ByVal HrsDelim As String, ByVal MinDelim As String, ByVal SecDelim As String, ByVal SecDecimalDigits As Integer)"
        /// with suitable parameters to achieve this effect.</para>
        /// </remarks>
        public string DegreesToHMS(double Degrees, string HrsDelim, string MinDelim)
        {
            return "";
        }

        /// <summary>
        /// Convert degrees to sexagesimal hours, minutes, and seconds
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <param name="HrsDelim">The delimiter string separating hours and minutes</param>
        /// <param name="MinDelim">The delimiter string separating minutes and seconds</param>
        /// <param name="SecDelim">The delimiter string to append to the seconds part </param>
        /// <returns>Sexagesimal representation of degrees input value, as hours, minutes, and seconds</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters. </para>
        /// <para>This overload is not available through COM, please use 
        /// "DegreesToHMS(ByVal Degrees As Double, ByVal HrsDelim As String, ByVal MinDelim As String, ByVal SecDelim As String, ByVal SecDecimalDigits As Integer)"
        /// with suitable parameters to achieve this effect.</para>
        /// </remarks>
        public string DegreesToHMS(double Degrees, string HrsDelim, string MinDelim, string SecDelim)
        {
            return "";
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
            return "";
        }

        // Convert a real number to sexagesimal whole, minutes. Allow
        // specifying the number of decimal digits on minutes. Called by
        // HoursToHM below, which just has different default delimiters.

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
            return "";
        }

        /// <summary>
        /// Convert degrees to sexagesimal degrees and minutes with the default minutes delimiter MM'
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <param name="DegDelim">The delimiter string separating degrees </param>
        /// <returns>Sexagesimal representation of degrees input value, as degrees and minutes</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters.</para>
        /// <para>This overload is not available through COM, please use 
        /// "DegreesToDM(ByVal Degrees As Double, ByVal DegDelim As String, ByVal MinDelim As String, ByVal MinDecimalDigits As Integer)"
        /// with suitable parameters to achieve this effect.</para>
        /// </remarks>
        public string DegreesToDM(double Degrees, string DegDelim)
        {
            return "";
        }

        /// <summary>
        /// Convert degrees to sexagesimal degrees and minutes
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <param name="DegDelim">The delimiter string separating degrees </param>
        /// <param name="MinDelim">The delimiter string to append to the minutes </param>
        /// <returns>Sexagesimal representation of degrees input value, as degrees and minutes</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters.</para>
        /// <para>This overload is not available through COM, please use 
        /// "DegreesToDM(ByVal Degrees As Double, ByVal DegDelim As String, ByVal MinDelim As String, ByVal MinDecimalDigits As Integer)"
        /// with suitable parameters to achieve this effect.</para>
        /// </remarks>
        public string DegreesToDM(double Degrees, string DegDelim, string MinDelim)
        {
            return "";
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
            return "";
        }

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
            return "";
        }

        /// <summary>
        /// Convert hours to sexagesimal hours and minutes with default minutes delimiter MM (null string)
        /// </summary>
        /// <param name="Hours">The hours value to convert</param>
        /// <param name="HrsDelim">The delimiter string separating hours and minutes</param>
        /// <returns>Sexagesimal representation of hours input value as hours and minutes</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters.</para>
        /// <para>This overload is not available through COM, please use 
        /// "HoursToHM(ByVal Hours As Double, ByVal HrsDelim As String, ByVal MinDelim As String, ByVal MinDecimalDigits As Integer)"
        /// with an suitable parameters to achieve this effect.</para>
        /// </remarks>
        public string HoursToHM(double Hours, string HrsDelim)
        {
            return "";
        }

        /// <summary>
        /// Convert hours to sexagesimal hours and minutes
        /// </summary>
        /// <param name="Hours">The hours value to convert</param>
        /// <param name="HrsDelim">The delimiter string separating hours and minutes</param>
        /// <param name="MinDelim">The delimiter string to append to the minutes part </param>
        /// <returns>Sexagesimal representation of hours input value as hours and minutes</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters.</para>
        /// <para>This overload is not available through COM, please use 
        /// "HoursToHM(ByVal Hours As Double, ByVal HrsDelim As String, ByVal MinDelim As String, ByVal MinDecimalDigits As Integer)"
        /// with an suitable parameters to achieve this effect.</para>
        /// </remarks>
        public string HoursToHM(double Hours, string HrsDelim, string MinDelim)
        {
            return "";
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
            return "";
        }


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
            return "";
        }

        /// <summary>
        /// Convert degrees to sexagesimal hours and minutes with default minute delimiter MM (null string)
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <param name="HrsDelim">The delimiter string separating hours and minutes</param>
        /// <returns>Sexagesimal representation of degrees input value as hours and minutes</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters</para>
        /// <para>This overload is not available through COM, please use 
        /// "DegreesToHM(ByVal Degrees As Double, ByVal HrsDelim As String, ByVal MinDelim As String, ByVal MinDecimalDigits As Integer)"
        /// with suitable parameters to achieve this effect.</para>
        /// </remarks>
        public string DegreesToHM(double Degrees, string HrsDelim)
        {
            return "";
        }

        /// <summary>
        /// Convert degrees to sexagesimal hours and minutes
        /// </summary>
        /// <param name="Degrees">The degrees value to convert</param>
        /// <param name="HrsDelim">The delimiter string separating hours and minutes</param>
        /// <param name="MinDelim">The delimiter string to append to the minutes part</param>
        /// <returns>Sexagesimal representation of degrees input value as hours and minutes</returns>
        /// <remarks>
        /// <para>If you need a leading plus sign, you must prepend it yourself. The delimiters are not restricted to single characters</para>
        /// <para>This overload is not available through COM, please use 
        /// "DegreesToHM(ByVal Degrees As Double, ByVal HrsDelim As String, ByVal MinDelim As String, ByVal MinDecimalDigits As Integer)"
        /// with suitable parameters to achieve this effect.</para>
        /// </remarks>
        public string DegreesToHM(double Degrees, string HrsDelim, string MinDelim)
        {
            return "";
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
            return "";
        }

        /// <summary>
        /// Current Julian date
        /// </summary>
        /// <returns>Current Julian date</returns>
        /// <remarks>This is quantised to the second in the COM component but to a small decimal fraction in the .NET component</remarks>
        public double JulianDate
        {
            get
            {
                return 0.0;
            }
        }

        /// <summary>
        /// Convert local-time Date to Julian date
        /// </summary>
        /// <param name="LocalDate">Date in local-time</param>
        /// <returns>Julian date</returns>
        /// <remarks>Julian dates are always in UTC </remarks>
        public double DateLocalToJulian(DateTime LocalDate)
        {
            return 0.0;
        }

        /// <summary>
        /// Convert Julian date to local-time Date
        /// </summary>
        /// <param name="JD">Julian date to convert</param>
        /// <returns>Date in local-time for the given Julian date</returns>
        /// <remarks>Julian dates are always in UTC</remarks>
        public DateTime DateJulianToLocal(double JD)
        {
            return new DateTime();
        }

        /// <summary>
        /// Convert UTC Date to Julian date
        /// </summary>
        /// <param name="UTCDate">UTC date to convert</param>
        /// <returns>Julian date</returns>
        /// <remarks>Julian dates are always in UTC </remarks>
        public double DateUTCToJulian(DateTime UTCDate)
        {
            return 0.0;
        }

        /// <summary>
        /// Convert Julian date to UTC Date
        /// </summary>
        /// <param name="JD">Julian date</param>
        /// <returns>Date in UTC for the given Julian date</returns>
        /// <remarks>Julian dates are always in UTC </remarks>
        public DateTime DateJulianToUTC(double JD)
        {
            return new DateTime();
        }


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
        public double ConvertUnits(double InputValue, Units FromUnits, Units ToUnits)
        {
            return 0.0;
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
            return 0.0;
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
            return 0.0;
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
            return 0.0;
        }

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
            return 0.0;
        }

        /// <summary>
        /// Conditions an hour angle to be in the range -12.0 to +12.0 by adding or subtracting 24.0 hours
        /// </summary>
        /// <param name="HA">Hour angle to condition</param>
        /// <returns>Hour angle in the range -12.0 to +12.0</returns>
        /// <remarks></remarks>
        public double ConditionHA(double HA)
        {
            return 0.0;
        }

        /// <summary>
        /// Conditions a Right Ascension value to be in the range 0 to 23.999999.. hours 
        /// </summary>
        /// <param name="RA">Right ascension to be conditioned</param>
        /// <returns>Right ascension in the range 0 to 23.999999...</returns>
        /// <remarks></remarks>
        public double ConditionRA(double RA)
        {
            return 0.0;
        }

        /// <summary>
        /// Current Julian date based on the UTC time scale
        /// </summary>
        /// <value>Julian day</value>
        /// <returns></returns>
        /// <remarks></remarks>
        public double JulianDateUtc
        {
            get
            {
                return 0.0;
            }
        }

        /// <summary>
        /// Converts a calendar day, month, year to a modified Julian date
        /// </summary>
        /// <param name="Day">Integer day of the month</param>
        /// <param name="Month">Integer month of the year</param>
        /// <param name="Year">Integer year</param>
        /// <returns>Double modified Julian date</returns>
        /// <remarks></remarks>
        public double CalendarToMJD(int Day, int Month, int Year)
        {
            return 0.0;
        }

        /// <summary>
        /// Returns a modified Julian date as a string formatted according to the supplied presentation format
        /// </summary>
        /// <param name="MJD">Modified Julian date</param>
        /// <param name="PresentationFormat">Format representation</param>
        /// <returns>Date string</returns>
        /// <exception cref="FormatException">Thrown if the provided PresentationFormat is not valid.</exception>
        /// <remarks>This expects the standard Microsoft date and time formatting characters as described 
        /// in http://msdn.microsoft.com/en-us/library/362btx8f(v=VS.90).aspx
        /// </remarks>
        public string FormatMJD(double MJD, string PresentationFormat)
        {
            return "";
        }

        /// <summary>
        /// Returns a Julian date as a string formatted according to the supplied presentation format
        /// </summary>
        /// <param name="JD">Julian date</param>
        /// <param name="PresentationFormat">Format representation</param>
        /// <returns>Date as a string</returns>
        /// <remarks>This expects the standard Microsoft date and time formatting characters as described 
        /// in http://msdn.microsoft.com/en-us/library/362btx8f(v=VS.90).aspx
        /// </remarks>
        public string FormatJD(double JD, string PresentationFormat)
        {
            return "";
        }

    }
}
