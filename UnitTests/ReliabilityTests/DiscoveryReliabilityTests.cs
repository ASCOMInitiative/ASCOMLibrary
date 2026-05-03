using ASCOM.Alpaca.Discovery;
using Xunit;

// Corrected analysis: the Finder null-logger NRE was a false positive.
//
// Finder.LogMessage (Finder.cs:511) calls the LoggerExtensions.LogMessage extension method.
// That extension method explicitly handles a null logger with `logger?.Log(...)` (LoggerExtensions.cs:31),
// so calling the default Finder() constructor (which leaves logger = null) does NOT cause a
// NullReferenceException when Search() is invoked.
//
// The real discovery-layer bugs (AlpacaDiscovery JSON null de-reference at lines 753 and 768,
// and the double BeginReceive in Responder.cs:255,266) require network/socket infrastructure
// and cannot be expressed as pure unit tests without mocking.

namespace ReliabilityTests
{
    public class FinderReliabilityTests
    {
        // Regression guard: the default no-arg constructor + Search() must NOT throw any
        // exception at all (the null logger must be handled gracefully, which the
        // LoggerExtensions already do).  This is expected to PASS and serves as a
        // canary for any future regressions that re-introduce a raw null de-reference.

        [Fact]
        public void Finder_DefaultConstructor_SearchIPv4_ShouldNotThrow()
        {
            using (var finder = new Finder())
            {
                var ex = Record.Exception(() => finder.Search(IPv4: true, IPv6: false));
                Assert.Null(ex);
            }
        }

        [Fact]
        public void Finder_DefaultConstructor_SearchIPv6_ShouldNotThrow()
        {
            using (var finder = new Finder())
            {
                var ex = Record.Exception(() => finder.Search(IPv4: false, IPv6: true));
                Assert.Null(ex);
            }
        }
    }
}
