using ASCOM.Common.Helpers;
using Xunit;

// These tests document and expose reliability bugs in ExceptionHelpers.
// Every test in this file is expected to FAIL against the current code.
// They will pass once the corresponding bugs are fixed.

namespace ReliabilityTests
{
    public class ExceptionHelperReliabilityTests
    {
        // ── Bug: ExceptionFromResponse(null) dereferences the null argument ──
        //
        // ExceptionHelpers.cs:27-28:
        //   public static DriverException ExceptionFromResponse(IResponse response)
        //   {
        //       return ExceptionFromErrorCode(response.ErrorNumber, response.ErrorMessage);
        //   }
        //
        // There is no null guard before accessing response.ErrorNumber, so passing null
        // throws NullReferenceException instead of returning null (no error).

        [Fact]
        public void ExceptionFromResponse_NullResponse_ShouldReturnNullNotThrow()
        {
            // A null response means "no response received" — a defensible situation.
            // The method should return null (no exception) rather than crash.
            var ex = Record.Exception(() => ExceptionHelpers.ExceptionFromResponse(null));
            Assert.Null(ex);
            // FAILS: NullReferenceException is thrown (ExceptionHelpers.cs:28)
        }

        // ── Bug: ErrorCodeFromException(null) dereferences the null argument ──
        //
        // ExceptionHelpers.cs:86:
        //   int HResult = ex.HResult;
        //
        // There is no null guard before accessing ex.HResult, so passing null
        // throws NullReferenceException.

        [Fact]
        public void ErrorCodeFromException_NullException_ShouldReturnUnspecifiedError()
        {
            // A null exception should map to the safe default AlpacaErrors.AlpacaNoError
            // or AlpacaErrors.UnspecifiedError, not crash the caller.
            var ex = Record.Exception(() => ExceptionHelpers.ErrorCodeFromException(null));
            Assert.Null(ex);
            // FAILS: NullReferenceException is thrown (ExceptionHelpers.cs:86)
        }
    }
}
