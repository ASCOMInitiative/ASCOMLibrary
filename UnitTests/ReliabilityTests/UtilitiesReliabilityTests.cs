using ASCOM;
using ASCOM.Tools;
using Xunit;

// These tests document and expose reliability bugs in Utilities.
// Every test in this file is expected to FAIL against the current code.
// They will pass once the corresponding bugs are fixed.

namespace ReliabilityTests
{
    public class UtilitiesRangeTests
    {
        // ── Bug: Range() while-condition exits after one step regardless of direction ──
        //
        // Each do-while variant has a condition equivalent to:
        //   while (Value < LowerBound)   [only]
        //
        // The condition does NOT loop again when Value is still above UpperBound, so
        // one subtraction of ModuloValue is applied and the loop exits — leaving a
        // still-out-of-range result for inputs that are more than one period above
        // the upper bound.
        //
        // Utilities.cs:797-840

        [Theory]
        [InlineData(48.5, 0.0, 24.0, 0.5)]   // 2 periods above: 48.5 - 24 - 24 = 0.5
        [InlineData(72.0, 0.0, 24.0, 0.0)]   // exactly 3 periods: should wrap to 0
        [InlineData(720.5, 0.0, 360.0, 0.5)] // degree equivalent: 720.5 mod 360 = 0.5
        [InlineData(1080.0, 0.0, 360.0, 0.0)]// exactly 3 full circles
        public void Range_ValueMoreThanOnePeriodAboveUpperBound_ReturnsCorrectlyWrappedValue(
            double input, double lower, double upper, double expected)
        {
            // [LowerEqual=true, UpperEqual=false] — the most common usage (e.g. RA [0,24), angle [0,360))
            double result = Utilities.Range(input, lower, true, upper, false);
            Assert.Equal(expected, result, precision: 9);
            // FAILS: the loop exits after one subtraction so result is still > UpperBound
            // e.g. Range(48.5, 0, true, 24, false) returns 24.5 instead of 0.5
        }

        [Theory]
        [InlineData(49.0, 0.0, 24.0, 1.0)]   // 49 - 24 - 24 = 1; LowerEqual AND UpperEqual
        [InlineData(721.0, 0.0, 360.0, 1.0)] // 721 - 360 - 360 = 1
        public void Range_ValueMoreThanOnePeriodAboveUpperBound_BothBoundsInclusive_ReturnsCorrectValue(
            double input, double lower, double upper, double expected)
        {
            double result = Utilities.Range(input, lower, true, upper, true);
            Assert.Equal(expected, result, precision: 9);
            // FAILS: same early-exit bug
        }

        // ── Bug: Range() hangs forever on ±Infinity ──
        //
        // Adding or subtracting ModuloValue from ±Infinity leaves Infinity unchanged,
        // so the loop never terminates.  Utilities.cs:797-840
        //
        // Fix: the method now throws InvalidValueException for non-finite inputs.

        [Fact]
        public void Range_PositiveInfinity_ShouldThrowInvalidValueException()
        {
            // +Infinity cannot be wrapped into a finite range; the method should reject it.
            Assert.Throws<InvalidValueException>(() =>
                Utilities.Range(double.PositiveInfinity, 0.0, true, 24.0, false));
        }

        [Fact]
        public void Range_NegativeInfinity_ShouldThrowInvalidValueException()
        {
            // -Infinity cannot be wrapped; the method should reject it without hanging.
            Assert.Throws<InvalidValueException>(() =>
                Utilities.Range(double.NegativeInfinity, 0.0, true, 24.0, false));
        }
    }

    public class UtilitiesNullInputTests
    {
        // ── Bug: DMSToDegrees / HMSToHours throw a raw framework exception on null ──
        //
        // Both methods construct a Regex and immediately call validator.IsMatch(input).
        // When input is null, Regex.IsMatch(null) throws ArgumentNullException from
        // System.Text.RegularExpressions rather than the ASCOM library's own
        // InvalidValueException.  Callers that handle ASCOM exceptions will not catch
        // this and will see an unhandled framework exception.
        //
        // Utilities.cs:53, Utilities.cs:133

        [Fact]
        public void DMSToDegrees_NullInput_ThrowsInvalidValueException()
        {
            // After the fix, the library should validate the argument and throw its own
            // exception BEFORE constructing the Regex.
            Assert.Throws<InvalidValueException>(() => Utilities.DMSToDegrees(null));
            // FAILS: ArgumentNullException is thrown instead of InvalidValueException
        }

        [Fact]
        public void HMSToHours_NullInput_ThrowsInvalidValueException()
        {
            Assert.Throws<InvalidValueException>(() => Utilities.HMSToHours(null));
            // FAILS: ArgumentNullException is thrown instead of InvalidValueException
        }
    }
}
