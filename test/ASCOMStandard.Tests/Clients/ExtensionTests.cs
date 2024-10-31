using ASCOM.Common.DeviceInterfaces;
using System;
using Xunit;
using ASCOM.Common;
using System.Threading.Tasks;
using ASCOM.Tools;
using ASCOM.Com.DriverAccess;
using ASCOM.Common.Interfaces;
using System.Threading;
using System.Diagnostics;
using Xunit.Abstractions;

namespace ASCOM.Alpaca.Tests.Clients
{
    [Collection("CommonTests")]
    public class CommonTests()    //public class CommonTests(ITestOutputHelper output)
    {
        // private readonly ITestOutputHelper output = output;

        [Fact]
        public static async Task ConnectTestPlatform7()
        {
            TraceLogger TL = new("ConnectTest7", true, 64, LogLevel.Debug);

            // Create a task completion source and token so that the task can be cancelled
            CancellationTokenSource cancellationTokenSource = new();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Camera client = new("ASCOM.Simulator.Camera");
            TL.LogMessage("Main", $"Device created");

            // Confirm that the device was created and is not connected
            Assert.NotNull(client);
            Assert.False(client.Connected);

            // Connect asynchronously
            TL.LogMessage("Main", $"Connecting to device...");
            await client.ConnectAsync(DeviceTypes.Camera, client.InterfaceVersion, logger: TL);
            TL.LogMessage("Main", $"Connection complete");

            // Confirm, that the device is connected
            Assert.True(client.Connected);
            TL.LogMessage("Main", $"Connected OK");

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task ConnectTestPlatform6()
        {
            TraceLogger TL = new("ConnectTest6", true, 64, LogLevel.Debug);

            // Create a task completion source and token so that the task can be cancelled
            CancellationTokenSource cancellationTokenSource = new();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Camera client = new("ASCOM.Simulator.Camera");
            TL.LogMessage("Main", $"Device created");

            // Confirm that the device was created and is not connected
            Assert.NotNull(client);
            Assert.False(client.Connected);

            // Connect asynchronously
            TL.LogMessage("Main", $"Connecting to device...");
            await client.ConnectAsync(DeviceTypes.Camera, client.InterfaceVersion - 1, logger: TL);
            TL.LogMessage("Main", $"Connection complete");

            // Confirm, that the device is connected
            Assert.True(client.Connected);
            TL.LogMessage("Main", $"Connected OK");

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task DisconnectTest7()
        {
            TraceLogger TL = new("DisconnectTest7", true, 64, LogLevel.Debug);

            // Create a task completion source and token so that the task can be cancelled
            CancellationTokenSource cancellationTokenSource = new();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Camera client = new("ASCOM.Simulator.Camera");
            TL.LogMessage("Main", $"Device created");

            // Confirm that the device was created and is not connected
            Assert.NotNull(client);
            Assert.False(client.Connected);

            // Connect to the device
            TL.LogMessage("Main", $"Connecting to device...");
            client.Connected = true;
            TL.LogMessage("Main", $"Connection complete");

            // Disconnect asynchronously
            TL.LogMessage("Main", $"Disconnecting from device...");
            await client.DisconnectAsync(DeviceTypes.Camera, client.InterfaceVersion, logger: TL);
            TL.LogMessage("Main", $"Disconnection complete");

            // Confirm, that the device is disconnected
            Assert.False(client.Connected);
            TL.LogMessage("Main", $"Disconnected OK");

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task DisconnectTest6()
        {
            TraceLogger TL = new("DisconnectTest6", true, 64, LogLevel.Debug);

            // Create a task completion source and token so that the task can be cancelled
            CancellationTokenSource cancellationTokenSource = new();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Camera client = new("ASCOM.Simulator.Camera");
            TL.LogMessage("Main", $"Device created");

            // Confirm that the device was created and is not connected
            Assert.NotNull(client);
            Assert.False(client.Connected);

            // Connect to the device
            TL.LogMessage("Main", $"Connecting to device...");
            client.Connected = true;
            TL.LogMessage("Main", $"Connection complete");

            // Disconnect asynchronously
            TL.LogMessage("Main", $"Disconnecting from device...");
            await client.DisconnectAsync(DeviceTypes.Camera, client.InterfaceVersion - 1, logger: TL);
            TL.LogMessage("Main", $"Disconnection complete");

            // Confirm, that the device is disconnected
            Assert.False(client.Connected);
            TL.LogMessage("Main", $"Disconnected OK");

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }
    }

    [Collection("CameraTests")]
    //public class MiscellaneousTests(ITestOutputHelper output)
    public class MiscellaneousTests()
    {
        // private readonly ITestOutputHelper output = output;

        [Fact]
        public static async Task BadCameraStartExposureTest()
        {
            TraceLogger TL = new("BadCameraStartExposure", true, 64, LogLevel.Debug);

            // Create a task completion source and token so that the task can be cancelled
            CancellationTokenSource cancellationTokenSource = new();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Camera client = new("ASCOM.Simulator.Camera");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            await Assert.ThrowsAsync<InvalidValueException>(async () =>
            {
                await client.StartExposureAsync(-10.0, true, CancellationToken.None, 100, TL);
            });

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task DefaultValuesTest()
        {
            TraceLogger TL = new("DefaultValues", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Camera client = new("ASCOM.Simulator.Camera");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            TL.LogMessage("Main", $"Before exposure: ImageReady:{client.ImageReady}");
            Assert.False(client.ImageReady);

            await client.StartExposureAsync(2.0, true);
            Thread.Sleep(100);

            TL.LogMessage("Main", $"After exposure: ImageReady:{client.ImageReady}");
            Assert.True(client.ImageReady);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }
    }

    [Collection("CameraTests")]
    public static class CameraTests
    {
        [Fact]
        public static async Task CameraStartExposureTest()
        {
            // Create a TraceLogger to record activity
            TraceLogger TL = new("CameraStartExposure", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Camera client = new("ASCOM.Simulator.Camera");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Test the client
            TL.LogMessage("Main", $"About to await method");
            await client.StartExposureAsync(1.0, true, pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Await complete");
            Assert.NotEqual(CameraState.Exposing, client.CameraState);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task CameraStopExposureTest()
        {
            Stopwatch sw = new();
            // Create a TraceLogger to record activity
            TraceLogger TL = new("CameraStopExposure", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Camera client = new("ASCOM.Simulator.Camera");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Start a task that will stop the exposure after 1 second
            Task stopExposureTask = new(async () =>
            {
                TL.LogMessage("StopExposureTask", $"Starting thread sleep");
                await Task.Delay(1000);
                TL.LogMessage("StopExposureTask", $"Sleep completed, stopping exposure.");
                client.StopExposure();
                TL.LogMessage("StopExposureTask", $"Exposure stopped.");
            });
            stopExposureTask.Start();

            // Test the client
            TL.LogMessage("Main", $"About to await method");
            sw.Start();
            await client.StartExposureAsync(5.0, true, pollInterval: 100, logger: TL);
            sw.Stop();
            TL.LogMessage("Main", $"Await complete");
            Assert.NotEqual(CameraState.Exposing, client.CameraState);
            Assert.True(sw.Elapsed.TotalSeconds < 2.0);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }
    }

    [Collection("CoverCalibratorTests")]
    public static class CoverCalibratorCalibratorTests
    {
        [Fact]
        public static async Task CoverCalibratorCalibratorOnTest()
        {
            // Create a TraceLogger to record activity
            TraceLogger TL = new("CoverCalibratorCalibratorOn", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            CoverCalibrator client = new("ASCOM.Simulator.CoverCalibrator");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Test the client
            TL.LogMessage("Main", $"About to await method");
            await client.CalibratorOnAsync(100, pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Await complete");
            Assert.Equal(CalibratorStatus.Ready, client.CalibratorState);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task CoverCalibratorCalibratorOffTest()
        {
            // Create a TraceLogger to record activity
            TraceLogger TL = new("CoverCalibratorCalibratorOff", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            CoverCalibrator client = new("ASCOM.Simulator.CoverCalibrator");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Test the client - turn the calibrator on first
            TL.LogMessage("Main", $"About to start calibrator on await");
            await client.CalibratorOnAsync(100, pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"On await complete");
            Assert.Equal(CalibratorStatus.Ready, client.CalibratorState);

            // Test the client- turn off the calibrator
            TL.LogMessage("Main", $"About to start calibrator off await method");
            await client.CalibratorOffAsync(pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Off await complete");
            Assert.Equal(CalibratorStatus.Off, client.CalibratorState);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }
    }

    [Collection("CoverCalibratorTests")]
    public static class CoverCalibratorCoverTests
    {
        [Fact]
        public static async Task CoverCalibratorOpenCoverTest()
        {
            // Create a TraceLogger to record activity
            TraceLogger TL = new("CoverCalibratorOpenCover", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            CoverCalibrator client = new("ASCOM.Simulator.CoverCalibrator");
            TL.LogMessage("Main", $"{Environment.CurrentManagedThreadId:00} Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"{Environment.CurrentManagedThreadId:00} Connected set true");

            // Test the client
            TL.LogMessage("Main", $"{Environment.CurrentManagedThreadId:00} About to await method");
            await client.OpenCoverAsync(pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"{Environment.CurrentManagedThreadId:00} Await complete");
            Assert.Equal(CoverStatus.Open, client.CoverState);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"{Environment.CurrentManagedThreadId:00} Finished");
        }

        [Fact]
        public static async Task CoverCalibratorHaltCoverTest()
        {
            // Create a TraceLogger to record activity
            TraceLogger TL = new("CoverCalibratorHaltCover", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            CoverCalibrator client = new("ASCOM.Simulator.CoverCalibrator");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Start a task that will halt the open after 1 second
            Task stopOpenTask = new(async () =>
            {
                TL.LogMessage("StopOpenTask", $"Starting thread sleep");
                await Task.Delay(1000);
                TL.LogMessage("StopOpenTask", $"Sleep completed, stopping open.");
                await client.HaltCoverAsync(pollInterval: 100, logger: TL);
                TL.LogMessage("StopOpenTask", $"Open halted.");
            });
            stopOpenTask.Start();

            // Test the client
            TL.LogMessage("Main", $"About to await method");
            await client.OpenCoverAsync(pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Await complete. Cover state: {client.CoverState}");
            Assert.Equal(CoverStatus.Unknown, client.CoverState);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task CoverCalibratorCloseCoverTest()
        {
            // Create a TraceLogger to record activity
            TraceLogger TL = new("CoverCalibratorCloseCover", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            CoverCalibrator client = new("ASCOM.Simulator.CoverCalibrator");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Test the client - open the cover first
            TL.LogMessage("Main", $"About to start open cover await");
            await client.OpenCoverAsync(pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Await complete");
            Assert.Equal(CoverStatus.Open, client.CoverState);

            // Test the client- turn off the calibrator
            TL.LogMessage("Main", $"About to start close cover await");
            await client.CloseCoverAsync(pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Close await complete");
            Assert.Equal(CoverStatus.Closed, client.CoverState);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }
    }

    [Collection("DomeTests")]
    public static class DomeTests
    {
        [Fact]
        public static async Task DomeSlewToAzimuthTest()
        {
            // Arbitrary azimuth to which to slew before the Park() test
            const double SLEW_AZIMUTH = 296.4;

            // Create a TraceLogger to record activity
            TraceLogger TL = new("DomeSlewToAzimuth", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Dome client = new("ASCOM.Simulator.Dome");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Slew somewhere that is not likely to be the park position
            TL.LogMessage("Main", $"Slewing to somewhere that is not likely to be the home or park positions");
            await client.SlewToAzimuthAsync(SLEW_AZIMUTH, pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Slew await complete, Is parked: {client.AtPark}, Is at home: {client.AtHome}, Azimuth: {client.Azimuth}");
            Assert.False(client.AtHome);
            Assert.False(client.AtPark);
            Assert.Equal(SLEW_AZIMUTH, client.Azimuth);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task DomeSlewToAltitude()
        {
            // Arbitrary azimuth to which to slew before the Park() test
            const double SLEW_ALTITUDE = 76.4;

            // Create a TraceLogger to record activity
            TraceLogger TL = new("DomeSlewToAzimuth", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Dome client = new("ASCOM.Simulator.Dome");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Open the shutter first otherwise the slew will fail
            TL.LogMessage("Main", $"About to await OpenShutter method");
            await client.OpenShutterAsync(pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Await complete, shutter state: {client.ShutterStatus}");
            Assert.Equal(ShutterState.Open, client.ShutterStatus);

            // Slew somewhere that is not likely to be the current altitude
            TL.LogMessage("Main", $"Slewing to altitude {SLEW_ALTITUDE} degrees");
            await client.SlewToAltitudeAsync(SLEW_ALTITUDE, pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Slew await complete, Altitude: {client.Altitude}");
            Assert.Equal(SLEW_ALTITUDE, client.Altitude);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task DomeOpenShutterTest()
        {
            // Create a TraceLogger to record activity
            TraceLogger TL = new("DomeOpenShutter", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Dome client = new("ASCOM.Simulator.Dome");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Test the client
            TL.LogMessage("Main", $"About to await method");
            await client.OpenShutterAsync(pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Await complete, shutter state: {client.ShutterStatus}");
            Assert.Equal(ShutterState.Open, client.ShutterStatus);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task DomeCloseShutterTest()
        {
            // Create a TraceLogger to record activity
            TraceLogger TL = new("DomeCloseShutter", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Dome client = new("ASCOM.Simulator.Dome");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Test the client
            TL.LogMessage("Main", $"About to await open shutter method");
            await client.OpenShutterAsync(pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Await complete, shutter state: {client.ShutterStatus}");
            Assert.Equal(ShutterState.Open, client.ShutterStatus);

            // Test the client
            TL.LogMessage("Main", $"About to await close shutter method");
            await client.CloseShutterAsync(pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Await complete, shutter state: {client.ShutterStatus}");
            Assert.Equal(ShutterState.Closed, client.ShutterStatus);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task DomeAbortShutterOpenTest()
        {
            // Create a TraceLogger to record activity
            //TraceLogger TL = new("DomeAbortShutterOpen", true, 64, LogLevel.Debug);
            TraceLogger TL = new("DomeAbortShutterOpen", true, 64, LogLevel.Information);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Dome client = new("ASCOM.Simulator.Dome");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Start a task that will abort the open after 1 second
            Task abortOpenTask = new(async () =>
            {
                TL.LogMessage("AbortOpenTask", $"Starting thread sleep");
                await Task.Delay(1000);
                TL.LogMessage("AbortOpenTask", $"Sleep completed, aborting open.");
                await client.AbortSlewAsync(pollInterval: 100, logger: TL);
                TL.LogMessage("AbortOpenTask", $"Open aborted.");
            });
            abortOpenTask.Start();

            // Test the client
            TL.LogMessage("Main", $"About to await method");
            await client.OpenShutterAsync(pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Await complete, shutter state: {client.ShutterStatus}");
            Assert.Equal(ShutterState.Error, client.ShutterStatus);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task DomeParkTest()
        {
            // Arbitrary azimuth to which to slew before the Park() test
            const double SLEW_AZIMUTH = 297.6;

            // Create a TraceLogger to record activity
            TraceLogger TL = new("DomePark", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Dome client = new("ASCOM.Simulator.Dome");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Slew somewhere that is not likely to be the park position
            TL.LogMessage("Main", $"Slewing to somewhere that is not likely to be the park position");
            await client.SlewToAzimuthAsync(SLEW_AZIMUTH, pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Slew await complete, Is parked: {client.AtPark}, Is at home: {client.AtHome}, Azimuth: {client.Azimuth}");
            Assert.False(client.AtPark);
            Assert.Equal(SLEW_AZIMUTH, client.Azimuth);

            // Test Park()
            TL.LogMessage("Main", $"About to await Park method");
            await client.ParkAsync(pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Await complete, Is parked: {client.AtPark}, Is at home: {client.AtHome}");
            Assert.True(client.AtPark);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task DomeFindHomeTest()
        {
            // Arbitrary azimuth to which to slew before the Park() test
            const double SLEW_AZIMUTH = 248.5;

            // Create a TraceLogger to record activity
            TraceLogger TL = new("DomeFindHome", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Dome client = new("ASCOM.Simulator.Dome");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Slew somewhere that is not likely to be the park position
            TL.LogMessage("Main", $"Slewing to somewhere that is not likely to be the home position");
            await client.SlewToAzimuthAsync(SLEW_AZIMUTH, pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Slew await complete, Is parked: {client.AtPark}, Is at home: {client.AtHome}, Azimuth: {client.Azimuth}");
            Assert.False(client.AtHome);
            Assert.Equal(SLEW_AZIMUTH, client.Azimuth);

            // Test FindHome()
            TL.LogMessage("Main", $"About to await FindHome method");
            await client.FindHomeAsync(pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Await complete, Is parked: {client.AtPark}, Is at home: {client.AtHome}");
            Assert.True(client.AtHome);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }
    }

    [Collection("FilterWheelTests")]
    public static class FilterWheelTests
    {
        [Fact]
        public static async Task FilterWheelPositionTest()
        {
            // Create a TraceLogger to record activity
            TraceLogger TL = new("FilterWheelPosition", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            FilterWheel client = new("ASCOM.Simulator.FilterWheel");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Move to the first filter

            // Test Position Set 0
            TL.LogMessage("Main", $"About to await setting Position 0 ");
            await client.PositionSetAsync(0, pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Await complete, Position: {client.Position}");
            Assert.Equal(0, client.Position);


            // Get the number of configured filters
            int highestFilterNumber = client.FocusOffsets.Length - 1; // Filter numbers start at 0
                                                                      // Test Position highest filter wheel position
            TL.LogMessage("Main", $"About to await setting Position {highestFilterNumber} ");
            await client.PositionSetAsync(highestFilterNumber, pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Await complete, Position: {client.Position}");
            Assert.Equal(highestFilterNumber, client.Position);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }
    }

    [Collection("FocuserTests")]
    public static class FocuserTests
    {
        [Fact]
        public static async Task FocuserMoveTest()
        {
            // Plus/minus tolerance on position that will be accept4ed as a pass
            const int POSITION_TOLERANCE = 10;

            int testPosition;

            // Create a TraceLogger to record activity
            TraceLogger TL = new("FocuserMove", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Focuser client = new("ASCOM.Simulator.Focuser");
            //AlpacaFocuser client = new AlpacaFocuser();
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true, Position: {client.Position}");

            // Find a target position to which the focuser will be commanded
            if (client.Position < client.MaxIncrement / 2) // Focuser is in the lower half of its range
            {
                testPosition = Math.Abs(client.Position + 2000);
            }
            else // Focuser is in the upper half of its range
            {
                testPosition = Math.Abs(client.Position - 2000);
            }
            Assert.NotEqual(testPosition, client.Position);

            // Test Position Set
            TL.LogMessage("Main", $"About to await setting Position property");
            await client.MoveAsync(testPosition, pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Await complete, Position: {client.Position}, Target: {testPosition}");
            Assert.True((client.Position > testPosition - POSITION_TOLERANCE) & (client.Position < testPosition + POSITION_TOLERANCE)); // Allow a position tolerance

            // Disconnect from the client and dispose
            //client.Connected = false;
            //client.Dispose();
            GC.Collect();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task FocuserAbortMoveTest()
        {
            // Create a TraceLogger to record activity
            TraceLogger TL = new("FocuserAbortMove", true, 64, LogLevel.Debug);

            try
            {

                // Plus/minus tolerance on position that will be accept4ed as a pass
                const int POSITION_TOLERANCE = 10;

                int testPosition;

                // Create a COM client
                TL.LogMessage("Main", $"About to create device");
                Focuser client = new("ASCOM.Simulator.Focuser");

                //AlpacaFocuser client = new AlpacaFocuser();
                TL.LogMessage("Main", $"Device created");
                Assert.NotNull(client);

                // Set connected to true
                client.Connected = true;
                TL.LogMessage("Main", $"Connected set true, Position: {client.Position}");

                // Start a task that will halt the move after 1 second
                Task haltMoveTask = new(async () =>
                {
                    TL.LogMessage("HaltMoveTask", $"Starting thread sleep");
                    await Task.Delay(1000);
                    TL.LogMessage("HaltMoveTask", $"Sleep completed, halting move.");
                    await client.HaltAsync(pollInterval: 100, logger: TL);
                    TL.LogMessage("HaltMoveTask", $"Move halted.");
                });
                haltMoveTask.Start();

                // Find a target position to which the focuser will be commanded
                if (client.Position < client.MaxIncrement / 2) // Focuser is in the lower half of its range
                {
                    testPosition = Math.Abs(client.Position + 2000);
                }
                else // Focuser is in the upper half of its range
                {
                    testPosition = Math.Abs(client.Position - 2000);
                }
                Assert.NotEqual(testPosition, client.Position);

                // Test Position Set
                TL.LogMessage("Main", $"About to await setting Position property");
                await client.MoveAsync(testPosition, pollInterval: 100, logger: TL);
                TL.LogMessage("Main", $"Await complete, Position: {client.Position}, Target: {testPosition}");
                Assert.True((client.Position <= testPosition - POSITION_TOLERANCE) | (client.Position >= testPosition + POSITION_TOLERANCE)); // Allow a position tolerance

                // Disconnect from the client and dispose
                //client.Connected = false;
                //client.Dispose();
                GC.Collect();

                TL.LogMessage("Main", $"Finished");
            }
            catch (Exception ex)
            {
                TL.LogMessage("Exception", ex.ToString());
            }
        }
    }

    [Collection("RotatorTests")]
    public static class RotatorTests
    {
        [Fact]
        public static async Task RotatorMoveTest()
        {
            const double RELATIVE_MOVE = 45.0;

            // Create a TraceLogger to record activity
            TraceLogger TL = new("RotatorMove", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Rotator client = new("ASCOM.Simulator.Rotator");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Find a target position to which the focuser will be commanded

            double startPosition = client.Position;
            double expectedPosition = (client.Position + RELATIVE_MOVE) % 360.0;
            Assert.NotEqual(expectedPosition, startPosition);

            // Slew somewhere that is not likely to be the park position
            TL.LogMessage("Main", $"Moving to {expectedPosition} degrees");
            await client.MoveAsync(RELATIVE_MOVE, pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Move await complete, Position: {client.Position}, Is moving: {client.IsMoving}");
            Assert.Equal(expectedPosition, client.Position);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task RotatorMoveAbsoluteTest()
        {
            const double RELATIVE_MOVE = 27.0;

            // Create a TraceLogger to record activity
            TraceLogger TL = new("RotatorMoveAbsolute", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Rotator client = new("ASCOM.Simulator.Rotator");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Find a target position to which the focuser will be commanded

            double startPosition = client.Position;
            double expectedPosition = (client.Position + RELATIVE_MOVE) % 360.0;
            Assert.NotEqual(expectedPosition, startPosition);

            // Slew somewhere that is not likely to be the park position
            TL.LogMessage("Main", $"Moving to {expectedPosition} degrees");
            await client.MoveAbsoluteAsync(expectedPosition, pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Move await complete, Position: {client.Position}, Target position: {expectedPosition}, Is moving: {client.IsMoving}");
            Assert.Equal(expectedPosition, client.Position);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task RotatorMoveMechanicalTest()
        {
            const double RELATIVE_MOVE = 59.0;

            // Create a TraceLogger to record activity
            TraceLogger TL = new("RotatorMoveMechanical", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Rotator client = new("ASCOM.Simulator.Rotator");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Find a target position to which the focuser will be commanded

            double startPosition = client.MechanicalPosition;
            double expectedPosition = (client.MechanicalPosition + RELATIVE_MOVE) % 360.0;
            Assert.NotEqual(expectedPosition, startPosition);

            // Slew somewhere that is not likely to be the park position
            TL.LogMessage("Main", $"Moving to mechanical {expectedPosition} degrees");
            await client.MoveMechanicalAsync(expectedPosition, pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Move await complete, Mechanical position: {client.MechanicalPosition}, Target position: {expectedPosition}, Is moving: {client.IsMoving}, Position: {client.Position}");
            Assert.Equal(expectedPosition, client.MechanicalPosition);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task RotatorHaltTest()
        {
            const double RELATIVE_MOVE = 170.0;

            // Create a TraceLogger to record activity
            TraceLogger TL = new("RotatorHalt", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Rotator client = new("ASCOM.Simulator.Rotator");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Find a target position to which the focuser will be commanded

            double startPosition = client.MechanicalPosition;
            double expectedPosition = (client.MechanicalPosition + RELATIVE_MOVE) % 360.0;
            Assert.NotEqual(expectedPosition, startPosition);

            // Start a task that will halt the move after 500ms
            Task haltMoveTask = new(async () =>
            {
                TL.LogMessage("HaltMoveTask", $"Starting thread sleep");
                await Task.Delay(250);
                TL.LogMessage("HaltMoveTask", $"Sleep completed, halting move.");
                await client.HaltAsync(pollInterval: 100, logger: TL);
                TL.LogMessage("HaltMoveTask", $"Move halted.");
            });
            haltMoveTask.Start();

            // Move to new position
            TL.LogMessage("Main", $"Moving to mechanical {expectedPosition} degrees");
            await client.MoveMechanicalAsync(expectedPosition, pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Move await complete, Mechanical position: {client.MechanicalPosition}, Target position: {expectedPosition}, Is moving: {client.IsMoving}, Position: {client.Position}");
            Assert.NotEqual(expectedPosition, client.MechanicalPosition);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }
    }

    [Collection("TelescopeTests")]
    public static class TelescopeTests
    {
        [Fact]
        public static async Task TelescopeSlewToAltAzTest()
        {
            // Create a TraceLogger to record activity
            TraceLogger TL = new("TelescopeSlewToAltAz", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Telescope client = new("ASCOM.Simulator.Telescope");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Unpark
            TL.LogMessage("Main", $"Unparking scope");
            await client.UnparkAsync(pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Scope un-parked- Is parked: {client.AtPark}, Is at home: {client.AtHome}");

            client.Tracking = false;
            TL.LogMessage("Main", $"Tracking set false");

            // Slew somewhere that is not likely to be the test position
            TL.LogMessage("Main", $"Slewing to 50, 60 that is not the test position");
            await client.SlewToAltAzTaskAsync(50.0, 60.0, pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Slew await complete, Is parked: {client.AtPark}, Is at home: {client.AtHome}, Azimuth: {client.Azimuth}, Altitude: {client.Altitude}");

            // Slew to the target 
            TL.LogMessage("Main", $"Slewing to the target ALt/Az (40.0, 50.0)");
            await client.SlewToAltAzTaskAsync(40.0, 50.0, pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Slew await complete, Is parked: {client.AtPark}, Is at home: {client.AtHome}, Azimuth: {client.Azimuth}, Altitude: {client.Altitude}");
            Assert.False(client.AtHome);
            Assert.False(client.AtPark);
            Assert.Equal(40.0, Math.Round(client.Azimuth, 1));
            Assert.InRange<double>(client.Altitude, 49.8, 50.2);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task TelescopeSlewToCoordinatesTest()
        {
            // Create a TraceLogger to record activity
            TraceLogger TL = new("TelescopeSlewToCoordinates", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Telescope client = new("ASCOM.Simulator.Telescope");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Unpark
            TL.LogMessage("Main", $"Unparking scope");
            await client.UnparkAsync(pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Scope un-parked- Is parked: {client.AtPark}, Is at home: {client.AtHome}");

            client.Tracking = true;
            TL.LogMessage("Main", $"Tracking set true");

            double targetRa = (client.SiderealTime - 2.456 + 24.0) % 24.0;

            // Slew somewhere that is not likely to be the test position
            TL.LogMessage("Main", $"Slewing to somewhere that is not likely to be the test position: RA: {targetRa}, Sidereal time: {client.SiderealTime}");
            await client.SlewToCoordinatesTaskAsync(targetRa, 5.0, pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Slew await complete, Is parked: {client.AtPark}, Is at home: {client.AtHome}, RA: {client.RightAscension}, Declination: {client.Declination}");

            // Slew to the target 
            TL.LogMessage("Main", $"Slewing to the target ALt/Az (0.0, 0.0)");
            await client.SlewToCoordinatesTaskAsync(targetRa, 0.0, pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Slew await complete, Is parked: {client.AtPark}, Is at home: {client.AtHome}, RA: {client.RightAscension}, Declination: {client.Declination}");
            Assert.False(client.AtHome);
            Assert.False(client.AtPark);
            Assert.Equal(Math.Round(targetRa, 3), Math.Round(client.RightAscension, 3));
            Assert.Equal(0.0, Math.Round(client.Declination, 3));

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task TelescopeSlewToTargetTest()
        {
            // Create a TraceLogger to record activity
            TraceLogger TL = new("TelescopeSlewToTarget", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Telescope client = new("ASCOM.Simulator.Telescope");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Unpark
            TL.LogMessage("Main", $"Unparking scope");
            await client.UnparkAsync(pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Scope un-parked- Is parked: {client.AtPark}, Is at home: {client.AtHome}");

            client.Tracking = true;
            TL.LogMessage("Main", $"Tracking set true");

            double targetRa = (client.SiderealTime - 2.456 + 24.0) % 24.0;

            // Slew somewhere that is not likely to be the test position
            TL.LogMessage("Main", $"Slewing to somewhere that is not likely to be the test position");
            await client.SlewToCoordinatesTaskAsync(targetRa, 3.0, pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Slew await complete, Is parked: {client.AtPark}, Is at home: {client.AtHome}, RA: {client.RightAscension}, Declination: {client.Declination}");

            client.TargetRightAscension = targetRa;
            client.TargetDeclination = 1.0;

            // Slew to the target 
            TL.LogMessage("Main", $"Slewing to the target RA/Dec ({client.TargetRightAscension}, {client.TargetDeclination})");
            await client.SlewToCoordinatesTaskAsync(targetRa, 0.0, pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Slew await complete, Is parked: {client.AtPark}, Is at home: {client.AtHome}, RA: {client.RightAscension}, Declination: {client.Declination}");
            Assert.False(client.AtHome);
            Assert.False(client.AtPark);
            Assert.Equal(Math.Round(targetRa, 3), Math.Round(client.RightAscension, 3));
            Assert.Equal(client.TargetDeclination, Math.Round(client.Declination, 3));

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task TelescopeFindHomeTest()
        {
            // Create a TraceLogger to record activity
            TraceLogger TL = new("TelescopeFindHome", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Telescope client = new("ASCOM.Simulator.Telescope");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Unpark
            TL.LogMessage("Main", $"Unparking scope");
            await client.UnparkAsync(pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Scope un-parked- Is parked: {client.AtPark}, Is at home: {client.AtHome}");

            client.Tracking = true;
            TL.LogMessage("Main", $"Tracking set true");

            double targetRa = (client.SiderealTime - 2.456 + 24.0) % 24.0;

            // Slew somewhere that is not likely to be the home position
            TL.LogMessage("Main", $"Slewing to somewhere that is not likely to be the test position");
            await client.SlewToCoordinatesTaskAsync(targetRa, 3.0, pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Slew await complete, Is parked: {client.AtPark}, Is at home: {client.AtHome}, RA: {client.RightAscension}, Declination: {client.Declination}");

            // Slew to the target 
            TL.LogMessage("Main", $"finding home");
            await client.FindHomeAsync(pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Slew await complete, Is parked: {client.AtPark}, Is at home: {client.AtHome}, RA: {client.RightAscension}, Declination: {client.Declination}");
            Assert.True(client.AtHome);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task TelescopeParkTest()
        {
            // Create a TraceLogger to record activity
            TraceLogger TL = new("TelescopePark", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Telescope client = new("ASCOM.Simulator.Telescope");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Unpark
            TL.LogMessage("Main", $"Unparking scope");
            await client.UnparkAsync(pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Scope un-parked- Is parked: {client.AtPark}, Is at home: {client.AtHome}");

            client.Tracking = true;
            TL.LogMessage("Main", $"Tracking set true");

            double targetRa = (client.SiderealTime - 2.456 + 24.0) % 24.0;

            // Slew somewhere that is not likely to be the park position
            TL.LogMessage("Main", $"Slewing to somewhere that is not likely to be the test position");
            await client.SlewToCoordinatesTaskAsync(targetRa, 3.0, pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Slew await complete, Is parked: {client.AtPark}, Is at home: {client.AtHome}, RA: {client.RightAscension}, Declination: {client.Declination}");

            // Park the scope
            TL.LogMessage("Main", $"Parking scope");
            await client.ParkAsync(pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Park await complete, Is parked: {client.AtPark}, Is at home: {client.AtHome}, RA: {client.RightAscension}, Declination: {client.Declination}");
            Assert.True(client.AtPark);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task TelescopeUnParkTest()
        {
            // Create a TraceLogger to record activity
            TraceLogger TL = new("TelescopeUnPark", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Telescope client = new("ASCOM.Simulator.Telescope");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Unpark
            TL.LogMessage("Main", $"Unparking scope");
            await client.UnparkAsync(pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Scope un-parked- Is parked: {client.AtPark}, Is at home: {client.AtHome}");

            client.Tracking = true;
            TL.LogMessage("Main", $"Tracking set true");

            double targetRa = (client.SiderealTime - 2.456 + 24.0) % 24.0;

            // Slew somewhere that is not likely to be the park position
            TL.LogMessage("Main", $"Slewing to somewhere that is not likely to be the test position");
            await client.SlewToCoordinatesTaskAsync(targetRa, 3.0, pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Slew await complete, Is parked: {client.AtPark}, Is at home: {client.AtHome}, RA: {client.RightAscension}, Declination: {client.Declination}");

            // Park the scope
            TL.LogMessage("Main", $"Parking scope");
            await client.ParkAsync(pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Park await complete, Is parked: {client.AtPark}, Is at home: {client.AtHome}, RA: {client.RightAscension}, Declination: {client.Declination}");
            Assert.True(client.AtPark);

            // Unpark the scope
            TL.LogMessage("Main", $"Unparking scope");
            await client.UnparkAsync(pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Unpark await complete, Is parked: {client.AtPark}, Is at home: {client.AtHome}, RA: {client.RightAscension}, Declination: {client.Declination}");
            Assert.False(client.AtPark);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task TelescopeAbortSlewTest()
        {
            // Create a TraceLogger to record activity
            TraceLogger TL = new("TelescopeAbortSlew", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            Telescope client = new("ASCOM.Simulator.Telescope");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Unpark
            TL.LogMessage("Main", $"Unparking scope");
            await client.UnparkAsync(pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Scope un-parked- Is parked: {client.AtPark}, Is at home: {client.AtHome}");

            client.Tracking = true;
            TL.LogMessage("Main", $"Tracking set true");

            // Create a target that is 3 hours away from the current RA
            double targetRa = (client.RightAscension - 3.0 + 24.0) % 24.0;

            // Start a task that will halt the slew after 1 second
            Task abortSlewTask = new(async () =>
            {
                TL.LogMessage("AbortSlewTask", $"Starting thread sleep");
                await Task.Delay(1000);
                TL.LogMessage("AbortSlewTask", $"Sleep completed, aborting slew.");
                await client.AbortSlewAsync(pollInterval: 100, logger: TL);
                TL.LogMessage("AbortSlewTask", $"Slew aborted.");
            });
            abortSlewTask.Start();

            // Start the slew that is to be aborted
            TL.LogMessage("Main", $"Slewing to somewhere that is 3 hours away form the current RA");
            await client.SlewToCoordinatesTaskAsync(targetRa, 5.0, pollInterval: 100, logger: TL);
            TL.LogMessage("Main", $"Slew await complete, Is parked: {client.AtPark}, Is at home: {client.AtHome}, RA: {client.RightAscension}, Declination: {client.Declination}");

            Assert.False(client.AtHome);
            Assert.False(client.AtPark);
            Assert.NotEqual<double>(targetRa, client.RightAscension);
            Assert.NotEqual<double>(0.0, client.Declination);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }
    }

    [Collection("CoverCalibratorTests")]
    public static class CancelTests
    {
        [Fact]
        public static async Task CancelTaskTest()
        {
            // Create a TraceLogger to record activity
            TraceLogger TL = new("CancelTask", true, 64, LogLevel.Debug);

            // Create a task completion source and token so that the task can be cancelled
            CancellationTokenSource cancellationTokenSource = new();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            CoverCalibrator client = new("ASCOM.Simulator.CoverCalibrator");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Ensure that the cover is in the closed state
            await client.CloseCoverAsync(pollInterval: 1000, logger: TL);

            // Start a task that will cancel the cover open after 1 second
            Task cancelOpenTask = new(async () =>
            {
                TL.LogMessage("CancelOpenTask", $"Starting thread sleep");
                await Task.Delay(3500);
                TL.LogMessage("CancelOpenTask", $"Sleep completed, cancelling open.");
                cancellationTokenSource.Cancel();
                TL.LogMessage("CancelOpenTask", $"Open cancelled.");
            });
            cancelOpenTask.Start();

            // Test the cancel
            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                TL.LogMessage("Main", $"About to await method");
                await client.OpenCoverAsync(cancellationToken, 1000, TL);
                TL.LogMessage("Main", $"Await complete - should never get here...");

            });
            TL.LogMessage("Main", $"Await complete, Cover state: {client.CoverState}");
            Assert.NotEqual(CoverStatus.Open, client.CoverState);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task CancelTaskTimeoutTest()
        {
            // Create a TraceLogger to record activity
            TraceLogger TL = new("CancelTaskTimeout", true, 64, LogLevel.Debug);

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            CoverCalibrator client = new("ASCOM.Simulator.CoverCalibrator");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Ensure that the cover is in the closed state
            await client.CloseCoverAsync(pollInterval: 1000, logger: TL);

            // Test the cancel
            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                // Create a task completion source and token so that the task can be cancelled
                CancellationTokenSource cancellationTokenSource = new();
                cancellationTokenSource.CancelAfter(1500);
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                TL.LogMessage("Main", $"About to await method");
                await client.OpenCoverAsync(cancellationToken, 1000, TL);
                TL.LogMessage("Main", $"Await complete - should never get here...");

            });
            TL.LogMessage("Main", $"Await complete, Cover state: {client.CoverState}");
            Assert.NotEqual(CoverStatus.Open, client.CoverState);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }

        [Fact]
        public static async Task CancelTaskWhileWaitingTest()
        {
            TraceLogger TL = new("CancelTaskWhileWaiting", true, 64, LogLevel.Debug);

            // Create a task completion source and token so that the task can be cancelled
            CancellationTokenSource cancellationTokenSource = new();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            // Create a COM client
            TL.LogMessage("Main", $"About to create device");
            CoverCalibrator client = new("ASCOM.Simulator.CoverCalibrator");
            TL.LogMessage("Main", $"Device created");
            Assert.NotNull(client);

            // Set connected to true
            client.Connected = true;
            TL.LogMessage("Main", $"Connected set true");

            // Ensure that the cover is in the closed state
            TL.LogMessage("Main", $"Closing cover...");
            await client.CloseCoverAsync(pollInterval: 1000, logger: TL);
            TL.LogMessage("Main", $"Cover closed.");

            // Start a task that will cancel the cover open after 1.5 seconds
            TL.LogMessage("Main", $"Opening cover");
            Task cancelOpenTask = new(async () =>
            {
                TL.LogMessage("CancelOpenTask", $"Starting 1 second sleep");
                await Task.Delay(3000);
                TL.LogMessage("CancelOpenTask", $"Sleep completed, cancelling cover open task.");
                cancellationTokenSource.Cancel();
                TL.LogMessage("CancelOpenTask", $"Open task cancelled.");
            });
            cancelOpenTask.Start();

            // Test the cancel
            TL.LogMessage("Main", $"About to create open cover task");
            Task openCoverTask = client.OpenCoverAsync(cancellationToken, 1000, TL);
            TL.LogMessage("Main", $"Task started.");

            for (int i = 0; i < 5; i++)
            {
                await Task.Delay(100);
                TL.LogMessage("Main", $"Doing some work: {i}");
            }

            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                TL.LogMessage("Main", $"Waiting for task to finish...");
                await openCoverTask;
            });

            TL.LogMessage("Main", $"Open cover task status: {openCoverTask.Status}, Exception: {openCoverTask.Exception}");
            Assert.Equal(TaskStatus.Canceled, openCoverTask.Status);

            TL.LogMessage("Main", $"Await complete, Cover state: {client.CoverState}");
            Assert.NotEqual(CoverStatus.Open, client.CoverState);

            // Disconnect from the client and dispose
            client.Connected = false;
            client.Dispose();
            TL.LogMessage("Main", $"Finished");
        }
    }

}
