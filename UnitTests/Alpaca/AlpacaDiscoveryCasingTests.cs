using System;
using System.Threading.Tasks;
using ASCOM.Alpaca.Discovery;
using ASCOM.Common.Alpaca;
using ASCOM.Tools;
using Xunit;

namespace ASCOM.Alpaca.Tests
{
    /// <summary>
    /// Verifies the JSON property-name case-sensitivity behaviour of <see cref="AlpacaDiscovery"/>
    /// across its two public constructors when parsing the three management API responses.
    ///
    /// Each test variation runs an in-process <see cref="UdpTestResponder"/> that instructs the
    /// discovery component to contact a local <see cref="HttpManagementApiServer"/>.  The HTTP
    /// server serves:
    ///   /management/apiversions          → {"&lt;valueCasing&gt;": [1]}
    ///   /management/v1/description       → {"&lt;valueCasing&gt;": { ... }}
    ///   /management/v1/configureddevices → {"&lt;valueCasing&gt;": [ ... ]}
    /// with the "Value" JSON key using a configurable casing.
    ///
    /// Port assignments
    /// ──────────────────────────────────────────────────────────────────────────────
    ///   UDP discovery port  HTTP port    Casing       Constructor
    ///   32261               40001        "Value"      AlpacaDiscovery()  — correct casing
    ///   32262               40002        "value"      AlpacaDiscovery()  — wrong casing (strict, rejected)
    ///   32263               40003        "VALUE"      AlpacaDiscovery()  — wrong casing (strict, rejected)
    ///   32264               40004        "vALUE"      AlpacaDiscovery()  — wrong casing (strict, rejected)
    ///   32265               40005        "Value"      AlpacaDiscovery(strictCasing=true, logger)
    ///   32266               40006        "value"      AlpacaDiscovery(strictCasing=true, logger)
    ///   32267               40007        "VALUE"      AlpacaDiscovery(strictCasing=true, logger)
    ///   32268               40008        "vALUE"      AlpacaDiscovery(strictCasing=true, logger)
    ///
    /// Background on the AlpacaDiscovery casing behaviour
    /// ────────────────────────────────────────────────────
    /// AlpacaDiscovery passes its internal strictCasing field directly to JsonSerializerOptions:
    ///   new JsonSerializerOptions { PropertyNameCaseInsensitive = strictCasing }
    ///
    /// This creates the same inverted mapping that exists in the deprecated
    /// Finder(bool strictCasing, ILogger) constructor:
    ///   strictCasing = true  → PropertyNameCaseInsensitive = true  → case-INSENSITIVE parsing
    ///   strictCasing = false → PropertyNameCaseInsensitive = false → case-SENSITIVE  parsing
    ///
    /// Consequently:
    ///   AlpacaDiscovery(strictCasing=true, logger) is case-INSENSITIVE and accepts any "Value" casing.
    ///   AlpacaDiscovery() defaults strictCasing to false → case-SENSITIVE: only "Value" is accepted.
    ///
    /// Note: this differs from Finder(), which defaults to case-INSENSITIVE via JSON_NAME_CASING_DEFAULT.
    /// </summary>
    public class AlpacaDiscoveryCasingTests
    {
        private const double DiscoveryDuration = 2.0;

        // ─────────────────────────────────────────────────────────────────────────────
        // Helper
        // ─────────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Waits for <paramref name="discovery"/> to signal completion, then allows an additional
        /// 500 ms for any in-flight <c>GetAlpacaDeviceInformation</c> HTTP calls to finish.
        /// </summary>
        private static async Task WaitForDiscoveryAsync(AlpacaDiscovery discovery)
        {
            DateTime deadline = DateTime.UtcNow.AddSeconds(DiscoveryDuration + 2.0);
            while (!discovery.DiscoveryComplete && DateTime.UtcNow < deadline)
                await Task.Delay(50, TestContext.Current.CancellationToken);

            await Task.Delay(500, TestContext.Current.CancellationToken);
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // 1a. Default constructor — AlpacaDiscovery()
        //     strictCasing defaults to false → PropertyNameCaseInsensitive = false
        //     → case-SENSITIVE JSON parsing; only the correctly-cased "Value" is accepted.
        // ─────────────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task DefaultConstructor_CorrectValueCasing_AscomDevicesReturned()
        {
            using (var httpServer = new HttpManagementApiServer(40001, "Value"))
            using (var udpResponder = new UdpTestResponder(32261, "AlpacaPort", 40001))
            using (var discovery = new AlpacaDiscovery())
            {
                discovery.StartDiscovery(1, 100, 32261, DiscoveryDuration, false, true, false, ServiceType.Http);
                await WaitForDiscoveryAsync(discovery);

                Assert.NotEmpty(discovery.GetAlpacaDevices());
                Assert.NotEmpty(discovery.GetAscomDevices(null));
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // 1b. Default constructor — AlpacaDiscovery()
        //     Wrong-cased "Value" names are REJECTED because strictCasing=false →
        //     PropertyNameCaseInsensitive=false → strict parsing.
        //     The UDP broadcast succeeds so an AlpacaDevice is found, but the
        //     AlpacaDescriptionResponse.Value stays null and triggers an early exit
        //     in GetAlpacaDeviceInformation, leaving GetAscomDevices empty.
        // ─────────────────────────────────────────────────────────────────────────────

        [Theory]
        [InlineData("value", 40002, 32262)]
        [InlineData("VALUE", 40003, 32263)]
        [InlineData("vALUE", 40004, 32264)]
        public async Task DefaultConstructor_WrongValueCasing_AlpacaDeviceFoundButNoAscomDevices(
            string valueCasing, int httpPort, int discoveryPort)
        {
            using (var httpServer = new HttpManagementApiServer(httpPort, valueCasing))
            using (var udpResponder = new UdpTestResponder(discoveryPort, "AlpacaPort", httpPort))
            using (var discovery = new AlpacaDiscovery())
            {
                discovery.StartDiscovery(1, 100, discoveryPort, DiscoveryDuration, false, true, false, ServiceType.Http);
                await WaitForDiscoveryAsync(discovery);

                // UDP discovery works regardless of HTTP parsing behaviour.
                Assert.NotEmpty(discovery.GetAlpacaDevices());
                // Case-sensitive parsing rejects the wrong-cased "Value" key so no ASCOM devices are populated.
                Assert.Empty(discovery.GetAscomDevices(null));
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // 2. AlpacaDiscovery(bool strictCasing, ILogger) constructor with strictCasing = true
        //    → PropertyNameCaseInsensitive = true → case-INSENSITIVE JSON parsing
        //    → any casing of "Value" is accepted.
        // ─────────────────────────────────────────────────────────────────────────────

#pragma warning disable CS0618 // We have to test this API call even though it's marked as obsolete.

        [Theory]
        [InlineData("Value", 40005, 32265)]
        [InlineData("value", 40006, 32266)]
        [InlineData("VALUE", 40007, 32267)]
        [InlineData("vALUE", 40008, 32268)]
        public async Task StrictCasingTrueConstructor_AllValueCasings_AscomDevicesReturned(
            string valueCasing, int httpPort, int discoveryPort)
        {
            using (var httpServer = new HttpManagementApiServer(httpPort, valueCasing))
            using (var udpResponder = new UdpTestResponder(discoveryPort, "AlpacaPort", httpPort))
            using (var discovery = new AlpacaDiscovery(true, new ConsoleLogger()))
            {
                discovery.StartDiscovery(1, 100, discoveryPort, DiscoveryDuration, false, true, false, ServiceType.Http);
                await WaitForDiscoveryAsync(discovery);

                Assert.NotEmpty(discovery.GetAlpacaDevices());
                Assert.NotEmpty(discovery.GetAscomDevices(null));
            }
        }

#pragma warning restore CS0618

    }
}
