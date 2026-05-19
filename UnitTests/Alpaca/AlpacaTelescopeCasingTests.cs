using ASCOM.Alpaca.Clients;
using ASCOM.Common.Alpaca;
using ASCOM.Common.Interfaces;
using ASCOM.Tools;
using Xunit;

namespace ASCOM.Alpaca.Tests
{
    /// <summary>
    /// Verifies the JSON property-name case-sensitivity behaviour of <see cref="AlpacaTelescope"/>
    /// when parsing the DoubleResponse returned for the <c>Altitude</c> property.
    ///
    /// <see cref="AlpacaTelescope"/> uses the <c>strictCasing</c> field (default <c>true</c>) to
    /// configure JSON deserialisation in <c>RemoteDevice.GetValue&lt;T&gt;</c>:
    ///
    ///   new JsonSerializerOptions { PropertyNameCaseInsensitive = !clientParameters.StrictCasing }
    ///
    /// This mapping (note the <c>!</c>) correctly reflects the intent of the parameter name:
    ///   strictCasing = true  → PropertyNameCaseInsensitive = false → case-SENSITIVE  (default)
    ///   strictCasing = false → PropertyNameCaseInsensitive = true  → case-INSENSITIVE
    ///
    /// Unlike the deprecated <c>Finder(bool strictCasing, ILogger)</c> and
    /// <c>AlpacaDiscovery(bool strictCasing, ILogger)</c> constructors, the
    /// <see cref="AlpacaTelescope"/> casing logic is NOT inverted.
    ///
    /// Test server
    /// ───────────
    /// An in-process <see cref="AlpacaTelescopeServer"/> listens on 127.0.0.1:33333 (or 33334)
    /// and returns a DoubleResponse JSON body whose "Value" key uses the casing under test.
    /// The Alpaca client requests:
    ///   GET http://127.0.0.1:{port}/api/v1/telescope/0/altitude?ClientID=1&amp;ClientTransactionID=N
    ///
    /// Expected outcomes
    /// ─────────────────
    ///   "Value"  (correct casing, strictCasing=true)  → Altitude returns 45.123
    ///   "value"  (wrong   casing, strictCasing=true)  → Altitude returns 0.0
    /// </summary>
    public class AlpacaTelescopeCasingTests
    {
        private const double AltitudeValue = 45.123;
        private const double DefaultAltitude = 0.0;
        private const int CorrectCasingPort = 33333;
        private const int WrongCasingPort = 33334;

        // ─────────────────────────────────────────────────────────────────────────────
        // 1. Correct casing "Value" — altitude should be returned as expected
        // ─────────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// When the server returns <c>{"Value": 45.123}</c> (correctly cased) and
        /// <c>strictCasing</c> is <c>true</c> (the default), <c>telescope.Altitude</c> returns
        /// the value embedded in the JSON (45.123).
        /// </summary>
        [Fact]
        public void CorrectValueCasing_AltitudeReturned()
        {
            TraceLogger logger = new TraceLogger("AlpacaTelescopeCorrectCasing", true);
            logger.SetMinimumLoggingLevel(LogLevel.Debug);

            using (var server = new AlpacaTelescopeServer(CorrectCasingPort, "Value", AltitudeValue))
            using (var telescope = new AlpacaTelescope(
                serviceType: ServiceType.Http,
                ipAddressString: "127.0.0.1",
                portNumber: CorrectCasingPort,
                remoteDeviceNumber: 0,
                strictCasing: true, logger: logger))
            {
                double altitude = telescope.Altitude;
                logger.LogMessage("Test", $"Received altitude: {altitude}");
                Assert.Equal(AltitudeValue, altitude, precision: 3);
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // 2. Wrong casing "value" — strict parsing rejects it; altitude defaults to 0.0
        // ─────────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// When the server returns <c>{"value": 45.123}</c> (wrong casing) and
        /// <c>strictCasing</c> is <c>true</c> (the default), the JSON deserialiser does not
        /// match "value" to the <c>Value</c> property of <see cref="ASCOM.Common.Alpaca.DoubleResponse"/>
        /// because <c>PropertyNameCaseInsensitive</c> is <c>false</c>.
        /// The property retains its default value of 0.0, so <c>telescope.Altitude</c> returns 0.0.
        /// </summary>
        [Fact]
        public void WrongValueCasing()
        {
            TraceLogger logger = new TraceLogger("AlpacaTelescopeWrongCasing", true);
            logger.SetMinimumLoggingLevel(LogLevel.Debug);

            using (var server = new AlpacaTelescopeServer(WrongCasingPort, "value", AltitudeValue))
            using (var telescope = new AlpacaTelescope(
                serviceType: ServiceType.Http,
                ipAddressString: "127.0.0.1",
                portNumber: WrongCasingPort,
                remoteDeviceNumber: 0,
                strictCasing: true, logger: logger))
            {
                double altitude = telescope.Altitude;
                logger.LogMessage("Test", $"Received altitude: {altitude}");
                Assert.Equal(DefaultAltitude, altitude, precision: 3);
            }
        }
    }
}
