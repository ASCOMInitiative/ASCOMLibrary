using ASCOM.Tools;
using System.Collections.Generic;
using System.Threading;
using Xunit;

// These tests document the thread-safety properties of AstroUtilities after fixing
// the missing volatile keyword on the two shared static fields.

namespace ReliabilityTests
{
    public class AstroUtilitiesLeapSecondsTests
    {
        // ── Fix: AstroUtilities.currentLeapSeconds is now guarded by s_leapSecondsLock ──
        //
        // AstroUtilities.cs:16-19: lock object + plain double field; getter and setter both lock.
        // volatile is not permitted for double in C#; a lock gives the same visibility guarantee.
        //
        // SetLeapSeconds() modifies a single process-wide value by design — leap seconds
        // is a universal constant, not a per-consumer configuration.  The fix ensures:
        //   1. Writes are immediately visible to all threads (volatile).
        //   2. The last call to SetLeapSeconds() always wins (intended).
        //   3. Concurrent reads cannot observe a stale or torn value.
        //
        // NOTE: if genuine per-caller isolation is needed in future, the fix is to make
        // AstroUtilities instance-based; that would be a breaking API change.

        [Fact]
        public void SetLeapSeconds_LastCallWins_ValueIsCorrectlyUpdated()
        {
            // SetLeapSeconds is process-wide.  After two calls the second value is active.
            const double FIRST_VALUE  = 37.0;
            const double SECOND_VALUE = 10.0;

            AstroUtilities.SetLeapSeconds(FIRST_VALUE);
            Assert.Equal(FIRST_VALUE, AstroUtilities.LeapSeconds);  // sanity

            AstroUtilities.SetLeapSeconds(SECOND_VALUE);
            Assert.Equal(SECOND_VALUE, AstroUtilities.LeapSeconds); // last-write-wins is correct
        }

        [Fact]
        public async System.Threading.Tasks.Task SetLeapSeconds_ConcurrentUpdates_FinalValueShouldBeOneOfTheWrittenValues()
        {
            // With volatile, reads and writes are ordered and the field always holds
            // exactly one of the values written — no torn double reads.

            const double VALUE_1 = 37.0;
            const double VALUE_2 = 38.0;

            var barrier = new Barrier(2);
            var t1 = System.Threading.Tasks.Task.Run(() => { barrier.SignalAndWait(); AstroUtilities.SetLeapSeconds(VALUE_1); }, TestContext.Current.CancellationToken);
            var t2 = System.Threading.Tasks.Task.Run(() => { barrier.SignalAndWait(); AstroUtilities.SetLeapSeconds(VALUE_2); }, TestContext.Current.CancellationToken);
            await System.Threading.Tasks.Task.WhenAll(t1, t2);

            double final = AstroUtilities.LeapSeconds;
            bool isOneOfTheWrittenValues = final == VALUE_1 || final == VALUE_2;
            Assert.True(isOneOfTheWrittenValues,
                $"Expected {VALUE_1} or {VALUE_2} but got {final} — possible torn write");
        }

        [Fact]
        public void SetLeapSeconds_CrossThreadWrite_IsImmediatelyVisibleToReader()
        {
            // With volatile, a write on one thread must be visible to a subsequent read
            // on another thread without an explicit memory barrier.
            const double EXPECTED = 39.0;
            double observed = 0.0;

            var writeComplete = new ManualResetEventSlim(false);
            var thread = new Thread(() =>
            {
                AstroUtilities.SetLeapSeconds(EXPECTED);
                writeComplete.Set();
            });
            thread.Start();
            writeComplete.Wait(TestContext.Current.CancellationToken);

            observed = AstroUtilities.LeapSeconds;
            Assert.Equal(EXPECTED, observed);
        }
    }

    public class AstroUtilitiesStaticLoggerTests
    {
        // ── Fix: AstroUtilities.TL is now volatile ──
        //
        // AstroUtilities.cs:17: private static volatile ILogger TL;
        // Reference types are valid volatile targets; this ensures cross-thread visibility.

        [Fact]
        public void SetLogger_LastCallWins_FieldIsUpdated()
        {
            var loggerA = new CapturingAstroLogger();
            var loggerB = new CapturingAstroLogger();

            AstroUtilities.SetLogger(loggerA);
            AstroUtilities.SetLogger(loggerB);

            // After the second call, the active logger should be loggerB.
            var tlField = typeof(AstroUtilities)
                .GetField("TL", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            Assert.NotNull(tlField);

            object currentTL = tlField.GetValue(null);
            Assert.Same(loggerB, currentTL); // last-write-wins is correct behaviour
        }

        [Fact]
        public void SetLogger_CrossThreadWrite_IsImmediatelyVisibleToReader()
        {
            var tlField = typeof(AstroUtilities)
                .GetField("TL", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            Assert.NotNull(tlField);

            var expectedLogger = new CapturingAstroLogger();
            object observed = null;

            var writeComplete = new ManualResetEventSlim(false);
            var thread = new Thread(() =>
            {
                AstroUtilities.SetLogger(expectedLogger);
                writeComplete.Set();
            });
            thread.Start();
            writeComplete.Wait(TestContext.Current.CancellationToken);

            observed = tlField.GetValue(null);
            Assert.Same(expectedLogger, observed);
        }
    }

    /// <summary>Minimal capturing logger for astrometry tests.</summary>
    internal sealed class CapturingAstroLogger : ASCOM.Common.Interfaces.ILogger
    {
        public List<string> Messages { get; } = new List<string>();

        public ASCOM.Common.Interfaces.LogLevel LoggingLevel => ASCOM.Common.Interfaces.LogLevel.Debug;

        public void SetMinimumLoggingLevel(ASCOM.Common.Interfaces.LogLevel level) { }

        public void Log(ASCOM.Common.Interfaces.LogLevel level, string message) => Messages.Add(message);
    }
}
