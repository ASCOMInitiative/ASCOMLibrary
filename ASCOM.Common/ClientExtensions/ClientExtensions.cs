using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ASCOM.Common
{
    public static class ClientExtensions
    {

        #region Common methods (IAscomDevice)

        // No long running common methods

        #endregion

        #region Camera extensions

        public static async Task StartExposureAsync(this ICameraV3 camera, double duration, bool light, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { camera.StartExposure(duration, light); },
                () =>
                {
                    return (camera.CameraState == CameraState.Waiting) |
                    (camera.CameraState == CameraState.Exposing) |
                    (camera.CameraState == CameraState.Reading);
                },
                pollInterval, cancellationToken, logger);
        }

        public static async Task StopExposureAsync(this ICameraV3 camera, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { camera.StopExposure(); }, () => { return camera.CameraState == CameraState.Reading; }, pollInterval, cancellationToken, logger);
        }

        #endregion

        #region CoverCalibrator extensions

        public static async Task CalibratorOnAsync(this ICoverCalibratorV1 device, int brightness, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.CalibratorOn(brightness); }, () => { return device.CalibratorState == CalibratorStatus.NotReady; }, pollInterval, cancellationToken, logger);
        }

        public static async Task CalibratorOffAsync(this ICoverCalibratorV1 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.CalibratorOff(); }, () => { return device.CalibratorState == CalibratorStatus.NotReady; }, pollInterval, cancellationToken, logger);
        }

        public static async Task OpenCoverAsync(this ICoverCalibratorV1 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.OpenCover(); }, () => { return device.CoverState == CoverStatus.Moving; }, pollInterval, cancellationToken, logger);
        }

        public static async Task CloseCoverAsync(this ICoverCalibratorV1 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.CloseCover(); }, () => { return device.CoverState == CoverStatus.Moving; }, pollInterval, cancellationToken, logger);
        }

        public static async Task HaltCoverAsync(this ICoverCalibratorV1 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.HaltCover(); }, () => { return device.CoverState == CoverStatus.Moving; }, pollInterval, cancellationToken, logger);
        }

        #endregion

        #region Dome extensions

        public static async Task AbortSlewAsync(this IDomeV2 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.AbortSlew(); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger);
        }

        public static async Task CloseHutterAsync(this IDomeV2 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.CloseShutter(); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger);
        }

        public static async Task OpenShutterAsync(this IDomeV2 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.OpenShutter(); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger);
        }

        public static async Task FindHomeAsync(this IDomeV2 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.FindHome(); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger);
        }

        public static async Task ParkAsync(this IDomeV2 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.Park(); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger);
        }

        public static async Task SlewToAltitudeAsync(this IDomeV2 device, double altitude, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.SlewToAltitude(altitude); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger);
        }

        public static async Task SlewToAzimuthAsync(this IDomeV2 device, double azimuth, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.SlewToAzimuth(azimuth); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger);
        }

        #endregion

        #region FilterWheel extensions

        public static async Task PositionSetAsync(this IFilterWheelV2 device, int position, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.Position = Convert.ToInt16(position); }, () => { return device.Position == -1; }, pollInterval, cancellationToken, logger);
        }

        #endregion

        #region Focuser extensions

        public static async Task HaltAsync(this IFocuserV3 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.Halt(); }, () => { return device.IsMoving; }, pollInterval, cancellationToken, logger);
        }

        public static async Task MoveAsync(this IFocuserV3 device, int position, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.Move(position); }, () => { return device.IsMoving; }, pollInterval, cancellationToken, logger);
        }

        #endregion

        #region ObservingConditions extensions

        // No long running methods

        #endregion

        #region Rotator extensions

        public static async Task HaltAsync(this IRotatorV3 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.Halt(); }, () => { return device.IsMoving; }, pollInterval, cancellationToken, logger);
        }

        public static async Task MoveAsync(this IRotatorV3 device, int position, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.Move(position); }, () => { return device.IsMoving; }, pollInterval, cancellationToken, logger);
        }

        public static async Task MoveAbsoluteAsync(this IRotatorV3 device, int position, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.MoveAbsolute(position); }, () => { return device.IsMoving; }, pollInterval, cancellationToken, logger);
        }

        public static async Task MoveMechanicalAsync(this IRotatorV3 device, int position, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.MoveMechanical(position); }, () => { return device.IsMoving; }, pollInterval, cancellationToken, logger);
        }

        #endregion

        #region SafetyMonitor extensions

        // No asynchronous methods

        #endregion

        #region Switch extensions

        // No asynchronous methods

        #endregion

        #region Telescope extensions

        public static async Task AbortSlewAsync(this ITelescopeV3 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.AbortSlew(); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger);
        }

        public static async Task FindHomeAsync(this ITelescopeV3 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.FindHome(); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger);
        }

        public static async Task ParkAsync(this ITelescopeV3 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.Park(); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger);
        }

        public static async Task AsyncSlewToAltAz(this ITelescopeV3 device, double azimuth, double altitude, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.SlewToAltAzAsync(azimuth, altitude); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger);
        }

        public static async Task AsyncSlewToCoordinates(this ITelescopeV3 device, double rightAscension, double declination, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.SlewToCoordinates(rightAscension, declination); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger);
        }

        public static async Task AsyncSlewToCTarget(this ITelescopeV3 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.SlewToTarget(); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger);
        }

        public static async Task UnparkAsync(this ITelescopeV3 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.Unpark(); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger);
        }

        #endregion

        #region Video extensions

        #endregion

        #region Support methods
        internal static async Task ProcessTask(Action initiatorMethod, Func<bool> processRunningFunction, int pollInterval, CancellationToken cancellationToken, ILogger logger)
        {
            TimeSpan processRunningCompletionFunctionDuration = TimeSpan.Zero;
            int processRunningFunctionCallCount = 0;
            bool processRunningFunctionValue = false;
            Stopwatch sw = new Stopwatch();
            int processRunningFunctionDuration = pollInterval; // ms
            Stopwatch swLoop = new Stopwatch();
            double loopTime = 0.0;

            // Start the operation by calling the initiator method
            initiatorMethod();

            // Asynchronously wait for the operation to complete
            await Task.Run(async () =>
            {
                do
                {
                    // Wait for the reuqired delay time
                    await Task.Delay(pollInterval - processRunningFunctionDuration);

                    // Determine the process running function value
                    sw.Restart();
                    processRunningFunctionValue = processRunningFunction();
                    sw.Stop();

                    // Calculate the average time that the process running function takes to complete
                    processRunningFunctionCallCount += 1;
                    processRunningCompletionFunctionDuration += TimeSpan.FromTicks(sw.ElapsedTicks);
                    processRunningFunctionDuration = Convert.ToInt32(processRunningCompletionFunctionDuration.TotalMilliseconds / processRunningFunctionCallCount);

                    // Measure the overall loop time
                    loopTime = swLoop.ElapsedMilliseconds;
                    swLoop.Restart();

                    // Log the outcome
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} Waiting {pollInterval}ms for action to complete... Delay time{processRunningFunctionDuration}, Call count: {processRunningFunctionCallCount}, Loop time: {loopTime:0.0}ms");
                    logger?.LogMessage(LogLevel.Debug, "AsyncTask", $"Waiting {pollInterval}ms for action to complete... Delay time{processRunningFunctionDuration}, Call count: {processRunningFunctionCallCount}, Loop time: {loopTime:0.0}ms");

                } while (processRunningFunctionValue & !cancellationToken.IsCancellationRequested); // Test whether the operation has completed or whether it has been cancelled
            });

            // Throw an operation cancelled exception if the operation was cancelled
            cancellationToken.ThrowIfCancellationRequested();
        }

        #endregion

    }
}
