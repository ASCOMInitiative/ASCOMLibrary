using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.DeviceStateClasses;
using ASCOM.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

// These tests document and expose reliability bugs in the DeviceState classes.
// Every test in this file is expected to FAIL against the current code.
// They will pass once the corresponding bugs are fixed.

namespace ReliabilityTests
{
    public class DeviceStateNullListTests
    {
        // ── Bug: deviceState.Count is accessed on the first log line BEFORE the null guard ──
        // Affected files (all share the same pattern):
        //   FocuserState.cs:29, VideoState.cs:29, CameraDeviceState.cs:29,
        //   RotatorState.cs:29, TelescopeState.cs:29, DomeState.cs:29,
        //   FilterWheelState.cs:29, SafetyMonitorState.cs:29,
        //   CoverCalibratorState.cs:29, ObservingConditionsState.cs:29
        //
        // The null guard is on the NEXT line (e.g. FocuserState.cs:32), so a null list
        // always throws NullReferenceException before the safe-return path is reached.

        // NOTE: the logger MUST be non-null to trigger the bug.
        // When null is passed, the null-conditional `TL?.LogMessage(...)` short-circuits
        // before evaluating `deviceState.Count`, accidentally masking the NRE.
        // The bug only fires when a logger is provided (the normal production usage).

        [Fact]
        public void FocuserState_NullList_ShouldReturnSafelyWithoutThrowing()
        {
            var ex = Record.Exception(() => new FocuserState(null, new CapturingLogger()));
            Assert.Null(ex); // FAILS: NullReferenceException at FocuserState.cs:29 — deviceState.Count evaluated before null guard
        }

        [Fact]
        public void VideoState_NullList_ShouldReturnSafelyWithoutThrowing()
        {
            var ex = Record.Exception(() => new VideoState(null, new CapturingLogger()));
            Assert.Null(ex); // FAILS: NullReferenceException at VideoState.cs:29
        }

        [Fact]
        public void CameraDeviceState_NullList_ShouldReturnSafelyWithoutThrowing()
        {
            var ex = Record.Exception(() => new CameraDeviceState(null, new CapturingLogger()));
            Assert.Null(ex); // FAILS: NullReferenceException at CameraDeviceState.cs:29
        }

        [Fact]
        public void RotatorState_NullList_ShouldReturnSafelyWithoutThrowing()
        {
            var ex = Record.Exception(() => new RotatorState(null, new CapturingLogger()));
            Assert.Null(ex); // FAILS: NullReferenceException at RotatorState.cs:29
        }

        [Fact]
        public void TelescopeState_NullList_ShouldReturnSafelyWithoutThrowing()
        {
            var ex = Record.Exception(() => new TelescopeState(null, new CapturingLogger()));
            Assert.Null(ex); // FAILS: NullReferenceException at TelescopeState.cs:29
        }

        [Fact]
        public void DomeState_NullList_ShouldReturnSafelyWithoutThrowing()
        {
            var ex = Record.Exception(() => new DomeState(null, new CapturingLogger()));
            Assert.Null(ex); // FAILS: NullReferenceException at DomeState.cs:29
        }

        [Fact]
        public void FilterWheelState_NullList_ShouldReturnSafelyWithoutThrowing()
        {
            var ex = Record.Exception(() => new FilterWheelState(null, new CapturingLogger()));
            Assert.Null(ex); // FAILS: NullReferenceException at FilterWheelState.cs:29
        }

        [Fact]
        public void SafetyMonitorState_NullList_ShouldReturnSafelyWithoutThrowing()
        {
            var ex = Record.Exception(() => new SafetyMonitorState(null, new CapturingLogger()));
            Assert.Null(ex); // FAILS: NullReferenceException at SafetyMonitorState.cs:29
        }

        [Fact]
        public void CoverCalibratorState_NullList_ShouldReturnSafelyWithoutThrowing()
        {
            var ex = Record.Exception(() => new CoverCalibratorState(null, new CapturingLogger()));
            Assert.Null(ex); // FAILS: NullReferenceException at CoverCalibratorState.cs:29
        }

        [Fact]
        public void ObservingConditionsState_NullList_ShouldReturnSafelyWithoutThrowing()
        {
            var ex = Record.Exception(() => new ObservingConditionsState(null, new CapturingLogger()));
            Assert.Null(ex); // FAILS: NullReferenceException at ObservingConditionsState.cs:29
        }
    }

    public class DeviceStateClassNameTests
    {
        // ── Bug: FocuserState and VideoState use the wrong className value ──
        // FocuserState.cs:15  → readonly string className = nameof(FilterWheelState);
        // VideoState.cs:15    → readonly string className = nameof(FilterWheelState);
        //
        // All diagnostic log messages emitted by these two classes are attributed to
        // "FilterWheelState", making log analysis misleading.

        [Fact]
        public void FocuserState_ClassName_ShouldBeNameOfFocuserState()
        {
            var field = typeof(FocuserState).GetField("className", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(field); // verify the field still exists (guards against rename)

            string className = (string)field.GetValue(new FocuserState());
            Assert.Equal(nameof(FocuserState), className);
            // FAILS: actual value is "FilterWheelState"  (FocuserState.cs:15)
        }

        [Fact]
        public void VideoState_ClassName_ShouldBeNameOfVideoState()
        {
            var field = typeof(VideoState).GetField("className", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(field);

            string className = (string)field.GetValue(new VideoState());
            Assert.Equal(nameof(VideoState), className);
            // FAILS: actual value is "FilterWheelState"  (VideoState.cs:15)
        }
    }

    /// <summary>
    /// Minimal ILogger that records every message for test assertions.
    /// </summary>
    internal sealed class CapturingLogger : ILogger
    {
        public List<string> Messages { get; } = new List<string>();

        public LogLevel LoggingLevel => LogLevel.Debug;

        public void SetMinimumLoggingLevel(LogLevel level) { }

        public void Log(LogLevel level, string message) => Messages.Add(message);
    }
}
