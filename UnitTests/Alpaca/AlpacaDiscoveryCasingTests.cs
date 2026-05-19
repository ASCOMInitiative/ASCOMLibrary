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
        private const double DiscoveryDuration = 0.2;

        // ─────────────────────────────────────────────────────────────────────────────
        // Helper
        // ─────────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Waits for <paramref name="discovery"/> to signal completion, then allows an additional
        /// 500 ms for any in-flight <c>GetAlpacaDeviceInformation</c> HTTP calls to finish.
        /// </summary>
        private static async Task WaitForDiscoveryAsync(AlpacaDiscovery discovery)
        {
            DateTime deadline = DateTime.UtcNow.AddSeconds(DiscoveryDuration + 0.1  );
            while (!discovery.DiscoveryComplete && DateTime.UtcNow < deadline)
                await Task.Delay(50, TestContext.Current.CancellationToken);

            await Task.Delay(50, TestContext.Current.CancellationToken);
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // 1b. Default constructor — AlpacaDiscovery()
        //     Operates in a case insensitive manner
        // ─────────────────────────────────────────────────────────────────────────────
        [Theory]
        [InlineData("Value", 40001, 32261)]
        [InlineData("value", 40002, 32262)]
        [InlineData("VALUE", 40003, 32263)]
        [InlineData("vALUE", 40004, 32264)]
        public async Task DefaultConstructor(string valueCasing, int httpPort, int discoveryPort)
        {
            using (var httpServer = new HttpManagementApiServer(httpPort, valueCasing))
            using (var udpResponder = new UdpTestResponder(discoveryPort, "AlpacaPort", httpPort))
            using (var discovery = new AlpacaDiscovery())
            {
                discovery.StartDiscovery(1, 100, discoveryPort, DiscoveryDuration, false, true, false, ServiceType.Http);
                await WaitForDiscoveryAsync(discovery);

                // UDP discovery works regardless of HTTP parsing behaviour.
                Assert.NotEmpty(discovery.GetAlpacaDevices());
                // Case-insensitive parsing accepts the wrong-cased "Value" key so no ASCOM devices are populated.
                Assert.NotEmpty(discovery.GetAscomDevices(null));
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // 2. AlpacaDiscovery(bool strictCasing, ILogger) constructor with strictCasing = true
        //    → PropertyNameCaseInsensitive = true → case-INSENSITIVE JSON parsing
        //    → any casing of "Value" is accepted. This logic error is why the constructor is deprecated.
        // ─────────────────────────────────────────────────────────────────────────────

#pragma warning disable CS0618 // We have to test this API call even though it's marked as obsolete.

        [Theory]
        [InlineData("Value", 40005, 32265)]
        [InlineData("value", 40006, 32266)]
        [InlineData("VALUE", 40007, 32267)]
        [InlineData("vALUE", 40008, 32268)]
        public async Task ObsoleteStrictCasingTrueConstructor(string valueCasing, int httpPort, int discoveryPort)
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

        // ─────────────────────────────────────────────────────────────────────────────
        // The strictCasing logic error means that setting strictCasing = false results in case-SENSITIVE parsing, which only accepts "Value".
        // ─────────────────────────────────────────────────────────────────────────────
        [Theory]
        [InlineData("Value", 40005, 32265)]
        public async Task ObsoleteStrictCasingFalseConstructorCorrectCasing(string valueCasing, int httpPort, int discoveryPort)
        {
            using (var httpServer = new HttpManagementApiServer(httpPort, valueCasing))
            using (var udpResponder = new UdpTestResponder(discoveryPort, "AlpacaPort", httpPort))
            using (var discovery = new AlpacaDiscovery(false, new ConsoleLogger()))
            {
                discovery.StartDiscovery(1, 100, discoveryPort, DiscoveryDuration, false, true, false, ServiceType.Http);
                await WaitForDiscoveryAsync(discovery);

                Assert.NotEmpty(discovery.GetAlpacaDevices());
                Assert.NotEmpty(discovery.GetAscomDevices(null));
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // The strictCasing logic error means that setting strictCasing = false results in case-SENSITIVE parsing, so incorrectly cased names are ignored..
        // ─────────────────────────────────────────────────────────────────────────────
        [Theory]
        [InlineData("value", 40006, 32266)]
        [InlineData("VALUE", 40007, 32267)]
        [InlineData("vALUE", 40008, 32268)]
        public async Task ObsoleteStrictCasingFalseConstructorIncorrectCasing(string valueCasing, int httpPort, int discoveryPort)
        {
            using (var httpServer = new HttpManagementApiServer(httpPort, valueCasing))
            using (var udpResponder = new UdpTestResponder(discoveryPort, "AlpacaPort", httpPort))
            using (var discovery = new AlpacaDiscovery(false, new ConsoleLogger()))
            {
                discovery.StartDiscovery(1, 100, discoveryPort, DiscoveryDuration, false, true, false, ServiceType.Http);
                await WaitForDiscoveryAsync(discovery);

                Assert.NotEmpty(discovery.GetAlpacaDevices());
                Assert.Empty(discovery.GetAscomDevices(null));
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // The JsonNameCaseSensitivity constructor with an AnyCasing parameter accepts all values regardless of casing.
        // ─────────────────────────────────────────────────────────────────────────────
        [Theory]
        [InlineData("Value", 40009, 32269)]
        [InlineData("value", 40010, 32270)]
        [InlineData("VALUE", 40011, 32271)]
        [InlineData("vALUE", 40012, 32272)]
        public async Task AnyCasingConstructor(string valueCasing, int httpPort, int discoveryPort)
        {
            using (var httpServer = new HttpManagementApiServer(httpPort, valueCasing))
            using (var udpResponder = new UdpTestResponder(discoveryPort, "AlpacaPort", httpPort))
            using (var discovery = new AlpacaDiscovery(JsonNameCaseSensitivity.AnyCasing, new ConsoleLogger()))
            {
                discovery.StartDiscovery(1, 100, discoveryPort, DiscoveryDuration, false, true, false, ServiceType.Http);
                await WaitForDiscoveryAsync(discovery);

                Assert.NotEmpty(discovery.GetAlpacaDevices());
                Assert.NotEmpty(discovery.GetAscomDevices(null));
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // The JsonNameCaseSensitivity constructor with a CorrectCasingOnly parameter only accepts correctly cased names.
        // ─────────────────────────────────────────────────────────────────────────────
        [Theory]
        [InlineData("Value", 40013, 32273)]
        public async Task CorrectCasingOnlyConstructorPass(string valueCasing, int httpPort, int discoveryPort)
        {
            using (var httpServer = new HttpManagementApiServer(httpPort, valueCasing))
            using (var udpResponder = new UdpTestResponder(discoveryPort, "AlpacaPort", httpPort))
            using (var discovery = new AlpacaDiscovery(JsonNameCaseSensitivity.CorrectCasingOnly, new ConsoleLogger()))
            {
                discovery.StartDiscovery(1, 100, discoveryPort, DiscoveryDuration, false, true, false, ServiceType.Http);
                await WaitForDiscoveryAsync(discovery);

                Assert.NotEmpty(discovery.GetAlpacaDevices());
                Assert.NotEmpty(discovery.GetAscomDevices(null));
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // The JsonNameCaseSensitivity constructor with a CorrectCasingOnly parameter rejects incorrectly cased names.
        // ─────────────────────────────────────────────────────────────────────────────
        [Theory]
        [InlineData("value", 40014, 32274)]
        [InlineData("VALUE", 40015, 32275)]
        [InlineData("vALUE", 40016, 32276)]
        public async Task CorrectCasingOnlyConstructorFail(string valueCasing, int httpPort, int discoveryPort)
        {
            using (var httpServer = new HttpManagementApiServer(httpPort, valueCasing))
            using (var udpResponder = new UdpTestResponder(discoveryPort, "AlpacaPort", httpPort))
            using (var discovery = new AlpacaDiscovery(JsonNameCaseSensitivity.CorrectCasingOnly, new ConsoleLogger()))
            {
                discovery.StartDiscovery(1, 100, discoveryPort, DiscoveryDuration, false, true, false, ServiceType.Http);
                await WaitForDiscoveryAsync(discovery);

                Assert.NotEmpty(discovery.GetAlpacaDevices());
                Assert.Empty(discovery.GetAscomDevices(null));
            }
        }

#pragma warning restore CS0618

    }
}
