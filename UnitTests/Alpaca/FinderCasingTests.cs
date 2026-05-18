using System;
using System.Linq;
using System.Threading.Tasks;
using ASCOM.Alpaca.Discovery;
using ASCOM.Tools;
using Xunit;

// Suppress the obsolete-member warning that is expected when exercising the deprecated
// Finder(bool strictCasing, ILogger) constructor.
#pragma warning disable CS0618

namespace ASCOM.Alpaca.Tests
{
    /// <summary>
    /// Verifies the JSON property-name case-sensitivity behaviour of <see cref="Finder"/> across all
    /// three public constructors and the <see cref="Finder.SetJsonNameCaseSensitivity"/> method.
    ///
    /// Each test variation binds its own <see cref="UdpTestResponder"/> to a dedicated UDP port so
    /// that tests can be run in parallel without port conflicts.  Port assignments are:
    ///
    ///   32241–32243  Default constructor, AnyCasing variants
    ///   32244–32246  ILogger constructor, AnyCasing variants
    ///   32247–32249  Obsolete constructor (strictCasing=true), AnyCasing variants
    ///   32250–32252  SetJsonNameCaseSensitivity(AnyCasing), variants
    ///   32253        SetJsonNameCaseSensitivity(StrictCasing), correct casing
    ///   32254–32256  SetJsonNameCaseSensitivity(StrictCasing), wrong casing variants
    ///
    /// Background on the Obsolete constructor
    /// ───────────────────────────────────────
    /// The original <c>Finder(bool strictCasing, ILogger)</c> implementation accidentally inverted
    /// the logic: passing <c>strictCasing = true</c> set <c>PropertyNameCaseInsensitive = true</c>
    /// (i.e. case-insensitive parsing).  The inversion has been preserved for backwards
    /// compatibility.  Tests in section 3 therefore assert case-INSENSITIVE behaviour when
    /// <c>strictCasing = true</c> is supplied.
    /// </summary>
    public class FinderCasingTests
    {
        // Maximum time to wait for a UDP round-trip on the local loopback interface.
        private const int ResponseTimeoutMs = 2000;

        // ─────────────────────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Polls <see cref="Finder.CachedEndpoints"/> until an entry whose port equals
        /// <paramref name="expectedPort"/> appears, or the timeout elapses.
        /// </summary>
        private static async Task<bool> WaitForPort(Finder finder, int expectedPort, int timeoutMs = ResponseTimeoutMs)
        {
            DateTime deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);
            while (DateTime.UtcNow < deadline)
            {
                if (finder.CachedEndpoints.Any(ep => ep.Port == expectedPort))
                    return true;
                await Task.Delay(25, TestContext.Current.CancellationToken);
            }
            return false;
        }

        /// <summary>
        /// Polls <see cref="Finder.BroadcastResponses"/> until at least one raw response has
        /// been recorded (confirming the UDP reply was received), or the timeout elapses.
        /// </summary>
        private static async Task<bool> WaitForBroadcastResponse(Finder finder, int timeoutMs = ResponseTimeoutMs)
        {
            DateTime deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);
            while (DateTime.UtcNow < deadline)
            {
                if (finder.BroadcastResponses.Count > 0)
                    return true;
                await Task.Delay(25, TestContext.Current.CancellationToken);
            }
            return false;
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // 1. Default constructor — AnyCasing (case-insensitive) by default
        // ─────────────────────────────────────────────────────────────────────────────

        [Theory]
        [InlineData("alpacaport", 11100, 32241)]
        [InlineData("ALPACAPORT", 11200, 32242)]
        [InlineData("aLPACApORT", 11300, 32243)]
        public async Task DefaultConstructor_AnyCasing_ParsesPortCorrectly(
            string jsonPropertyName, int alpacaPort, int discoveryPort)
        {
            using (var responder = new UdpTestResponder(discoveryPort, jsonPropertyName, alpacaPort))
            using (var finder = new Finder())
            {
                finder.Search(discoveryPort, true, false);

                bool received = await WaitForPort(finder, alpacaPort);
                Assert.True(received,
                    $"Default constructor: expected port {alpacaPort} in CachedEndpoints for JSON property '{jsonPropertyName}'.");
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // 2. ILogger constructor — AnyCasing (case-insensitive) by default
        // ─────────────────────────────────────────────────────────────────────────────

        [Theory]
        [InlineData("alpacaport", 12100, 32244)]
        [InlineData("ALPACAPORT", 12200, 32245)]
        [InlineData("aLPACApORT", 12300, 32246)]
        public async Task LoggerConstructor_AnyCasing_ParsesPortCorrectly(
            string jsonPropertyName, int alpacaPort, int discoveryPort)
        {
            using (var responder = new UdpTestResponder(discoveryPort, jsonPropertyName, alpacaPort))
            using (var finder = new Finder(new ConsoleLogger()))
            {
                finder.Search(discoveryPort, true, false);

                bool received = await WaitForPort(finder, alpacaPort);
                Assert.True(received,
                    $"ILogger constructor: expected port {alpacaPort} in CachedEndpoints for JSON property '{jsonPropertyName}'.");
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // 3. Obsolete constructor Finder(bool strictCasing, ILogger)
        //    strictCasing=true → PropertyNameCaseInsensitive=true (inverted logic)
        //    Result: case-INSENSITIVE parsing despite the parameter name.
        // ─────────────────────────────────────────────────────────────────────────────

        [Theory]
        [InlineData("alpacaport", 13100, 32247)]
        [InlineData("ALPACAPORT", 13200, 32248)]
        [InlineData("aLPACApORT", 13300, 32249)]
        public async Task ObsoleteConstructor_StrictCasingTrue_IsCaseInsensitive(
            string jsonPropertyName, int alpacaPort, int discoveryPort)
        {
            using (var responder = new UdpTestResponder(discoveryPort, jsonPropertyName, alpacaPort))
            using (var finder = new Finder(true, new ConsoleLogger()))
            {
                finder.Search(discoveryPort, true, false);

                bool received = await WaitForPort(finder, alpacaPort);
                Assert.True(received,
                    $"Obsolete constructor with strictCasing=true (which applies case-insensitive logic due to preserved inversion): " +
                    $"expected port {alpacaPort} in CachedEndpoints for JSON property '{jsonPropertyName}'.");
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // 4. SetJsonNameCaseSensitivity(AnyCasing) — retains case-insensitive behaviour
        // ─────────────────────────────────────────────────────────────────────────────

        [Theory]
        [InlineData("alpacaport", 14100, 32250)]
        [InlineData("ALPACAPORT", 14200, 32251)]
        [InlineData("aLPACApORT", 14300, 32252)]
        public async Task SetAnyCasing_RetainsCaseInsensitiveBehaviour(
            string jsonPropertyName, int alpacaPort, int discoveryPort)
        {
            using (var responder = new UdpTestResponder(discoveryPort, jsonPropertyName, alpacaPort))
            using (var finder = new Finder())
            {
                finder.SetJsonNameCaseSensitivity(JsonNameCaseSensitivity.AnyCasing);
                finder.Search(discoveryPort, true, false);

                bool received = await WaitForPort(finder, alpacaPort);
                Assert.True(received,
                    $"After SetJsonNameCaseSensitivity(AnyCasing): expected port {alpacaPort} in CachedEndpoints for JSON property '{jsonPropertyName}'.");
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // 5a. SetJsonNameCaseSensitivity(StrictCasing) — correctly-cased key accepted
        // ─────────────────────────────────────────────────────────────────────────────

        [Fact]
        public async Task SetStrictCasing_CorrectCasing_ParsesPort()
        {
            const int alpacaPort = 15100;
            const int discoveryPort = 32253;

            using (var responder = new UdpTestResponder(discoveryPort, "AlpacaPort", alpacaPort))
            using (var finder = new Finder())
            {
                finder.SetJsonNameCaseSensitivity(JsonNameCaseSensitivity.CorrectCasingOnly);
                finder.Search(discoveryPort, true, false);

                bool received = await WaitForPort(finder, alpacaPort);
                Assert.True(received,
                    $"After SetJsonNameCaseSensitivity(StrictCasing): correct key 'AlpacaPort' should return port {alpacaPort}.");
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // 5b. SetJsonNameCaseSensitivity(StrictCasing) — incorrectly-cased keys rejected
        //
        //     The response IS received (visible in BroadcastResponses), but with strict
        //     JSON parsing the wrong-cased key deserialises to AlpacaPort=0, which causes
        //     ReceiveCallback to throw internally.  The endpoint is therefore NOT added to
        //     CachedEndpoints — the effective returned port number is zero.
        // ─────────────────────────────────────────────────────────────────────────────

        [Theory]
        [InlineData("alpacaport", 16100, 32254)]
        [InlineData("ALPACAPORT", 16200, 32255)]
        [InlineData("aLPACApORT", 16300, 32256)]
        public async Task SetStrictCasing_WrongCasing_PortIsNotParsed(
            string jsonPropertyName, int alpacaPort, int discoveryPort)
        {
            using (var responder = new UdpTestResponder(discoveryPort, jsonPropertyName, alpacaPort))
            using (var finder = new Finder())
            {
                finder.SetJsonNameCaseSensitivity(JsonNameCaseSensitivity.CorrectCasingOnly);
                finder.Search(discoveryPort, true, false);

                // First confirm the responder actually replied; this rules out a UDP connectivity
                // issue masking a genuine parsing failure.
                bool responseReceived = await WaitForBroadcastResponse(finder);
                Assert.True(responseReceived,
                    $"No raw broadcast response was recorded — check UDP broadcast connectivity on port {discoveryPort}.");

                // Allow a short settling period for the async ReceiveCallback to finish processing.
                await Task.Delay(200, TestContext.Current.CancellationToken);

                // The wrong-cased JSON property deserialises to AlpacaPort=0 under strict parsing.
                // ReceiveCallback throws and catches the "Failed to parse" exception internally,
                // so nothing should be added to CachedEndpoints.
                Assert.Empty(finder.CachedEndpoints);
            }
        }
    }
}

#pragma warning restore CS0618
