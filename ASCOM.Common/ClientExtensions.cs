using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Xml;
using ASCOM.Common.Alpaca;
using System.Reflection;
using System.Linq;

namespace ASCOM.Common
{
    /// <summary>
    /// Asynchronous extensions for the Alpaca clients and COM clients that return awaitable tasks for long running methods.
    /// </summary>
    public static class ClientExtensions
    {

        #region Common methods (IAscomDevice)

        // No long running common methods

        #endregion

        #region Camera extensions

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that takes a camera image
        /// </summary>
        /// <param name="device">The Camera device</param>
        /// <param name="duration">Length of exposure</param>
        /// <param name="light"><see langword="true"/> for light frames, <see langword="false"/> for dark frames</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the exposure is complete</returns>
        public static async Task StartExposureAsync(this ICameraV3 device, double duration, bool light, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.StartExposure(duration, light); },
                () =>
                {
                    return (device.CameraState == CameraState.Waiting) |
                    (device.CameraState == CameraState.Exposing) |
                    (device.CameraState == CameraState.Reading);
                },
                pollInterval, cancellationToken, logger, $"{nameof(ICameraV3)}.{nameof(StartExposureAsync)}");
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that stops the current camera exposure
        /// </summary>
        /// <param name="device">The Camera device</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the exposure is has stopped</returns>
        public static async Task StopExposureAsync(this ICameraV3 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.StopExposure(); }, () => { return device.CameraState == CameraState.Reading; }, pollInterval, cancellationToken, logger, $"{nameof(ICameraV3)}.{nameof(StopExposureAsync)}");
        }

        #endregion

        #region CoverCalibrator extensions

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that turns the calibrator off
        /// </summary>
        /// <param name="device">The CoverCalibrator device</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the calibrator is off</returns>
        public static async Task CalibratorOffAsync(this ICoverCalibratorV1 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.CalibratorOff(); }, () => { return device.CalibratorState == CalibratorStatus.NotReady; }, pollInterval, cancellationToken, logger, $"{nameof(ICoverCalibratorV1)}.{nameof(CalibratorOffAsync)}");
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that turns the calibrator on
        /// </summary>
        /// <param name="device">The CoverCalibrator device</param>
        /// <param name="brightness">Required brightness level</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the calibrator is on</returns>
        public static async Task CalibratorOnAsync(this ICoverCalibratorV1 device, int brightness, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.CalibratorOn(brightness); }, () => { return device.CalibratorState == CalibratorStatus.NotReady; }, pollInterval, cancellationToken, logger, $"{nameof(ICoverCalibratorV1)}.{nameof(CalibratorOnAsync)}");
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that closes the cover
        /// </summary>
        /// <param name="device">The CoverCalibrator device</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the cover is closed</returns>
        public static async Task CloseCoverAsync(this ICoverCalibratorV1 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.CloseCover(); }, () => { return device.CoverState == CoverStatus.Moving; }, pollInterval, cancellationToken, logger, $"{nameof(ICoverCalibratorV1)}.{nameof(CloseCoverAsync)}");
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that halts cover movement
        /// </summary>
        /// <param name="device">The CoverCalibrator device</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when cover movement has stopped</returns>
        public static async Task HaltCoverAsync(this ICoverCalibratorV1 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.HaltCover(); }, () => { return device.CoverState == CoverStatus.Moving; }, pollInterval, cancellationToken, logger, $"{nameof(ICoverCalibratorV1)}.{nameof(HaltCoverAsync)}");
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that opens the cover
        /// </summary>
        /// <param name="device">The CoverCalibrator device</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the cover is open</returns>
        public static async Task OpenCoverAsync(this ICoverCalibratorV1 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.OpenCover(); }, () => { return device.CoverState == CoverStatus.Moving; }, pollInterval, cancellationToken, logger, $"{nameof(ICoverCalibratorV1)}.{nameof(OpenCoverAsync)}");
        }

        #endregion

        #region Dome extensions

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that halts all dome movement
        /// </summary>
        /// <param name="device">The Dome device</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when all dome movement has stopped</returns>

        public static async Task AbortSlewAsync(this IDomeV2 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.AbortSlew(); }, () => { return device.Slewing | (device.ShutterStatus == ShutterState.Opening) | (device.ShutterStatus == ShutterState.Closing); }, pollInterval, cancellationToken, logger, $"{nameof(IDomeV2)}.{nameof(AbortSlewAsync)}");
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that closes the dome shutter
        /// </summary>
        /// <param name="device">The Dome device</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when cover is closed</returns>
        public static async Task CloseShutterAsync(this IDomeV2 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.CloseShutter(); }, () => { return device.ShutterStatus == ShutterState.Closing; }, pollInterval, cancellationToken, logger, $"{nameof(IDomeV2)}.{nameof(CloseShutterAsync)}");
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that moves the dome to the home position
        /// </summary>
        /// <param name="device">The Dome device</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the dome is at its home position</returns>
        public static async Task FindHomeAsync(this IDomeV2 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.FindHome(); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger, $"{nameof(IDomeV2)}.{nameof(FindHomeAsync)}");
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that opens the dome shutter
        /// </summary>
        /// <param name="device">The Dome device</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the shutter is open</returns>
        public static async Task OpenShutterAsync(this IDomeV2 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.OpenShutter(); }, () => { return device.ShutterStatus == ShutterState.Opening; }, pollInterval, cancellationToken, logger, $"{nameof(IDomeV2)}.{nameof(OpenShutterAsync)}");
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that parks the dome
        /// </summary>
        /// <param name="device">The Dome device</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the dome is at its park position</returns>

        public static async Task ParkAsync(this IDomeV2 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.Park(); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger, $"{nameof(IDomeV2)}.{nameof(ParkAsync)}");
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that slews the dome to the specified altitude
        /// </summary>
        /// <param name="device">The Dome device</param>
        /// <param name="altitude">The CoverCalibrator device</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the dome is at the required altitude</returns>
        public static async Task SlewToAltitudeAsync(this IDomeV2 device, double altitude, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.SlewToAltitude(altitude); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger, $"{nameof(IDomeV2)}.{nameof(SlewToAltitudeAsync)}");
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that slews the dome to the specified azimuth
        /// </summary>
        /// <param name="device">The Dome device</param>
        /// <param name="azimuth">The CoverCalibrator device</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the dome is at the required azimuth</returns>
        public static async Task SlewToAzimuthAsync(this IDomeV2 device, double azimuth, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.SlewToAzimuth(azimuth); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger, $"{nameof(IDomeV2)}.{nameof(SlewToAzimuthAsync)}");
        }

        #endregion

        #region FilterWheel extensions

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that moves the filter wheel to the specified filter wheel position
        /// </summary>
        /// <param name="device">The FilterWheel device</param>
        /// <param name="position">The required filter wheel position</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the filter wheel is at the required position</returns>
        public static async Task PositionSetAsync(this IFilterWheelV2 device, int position, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.Position = Convert.ToInt16(position); }, () => { return device.Position == -1; }, pollInterval, cancellationToken, logger, $"{nameof(IFilterWheelV2)}.{nameof(PositionSetAsync)}");
        }

        #endregion

        #region Focuser extensions

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that halts focuser movement
        /// </summary>
        /// <param name="device">The Focuser device</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when focuser movement has halted</returns>
        public static async Task HaltAsync(this IFocuserV3 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.Halt(); }, () => { return device.IsMoving; }, pollInterval, cancellationToken, logger, $"{nameof(IFocuserV3)}.{nameof(HaltAsync)}", () => { return $"Position: {device.Position}"; });
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that moves to the specified focuser position
        /// </summary>
        /// <param name="device">The Focuser device</param>
        /// <param name="position">The required filter wheel position</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the focuser is at the required position</returns>
        public static async Task MoveAsync(this IFocuserV3 device, int position, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.Move(position); }, () => { return device.IsMoving; }, pollInterval, cancellationToken, logger, $"{nameof(IFocuserV3)}.{nameof(MoveAsync)}", () => { return $"Position: {device.Position}"; });
        }

        #endregion

        #region ObservingConditions extensions

        // No long running methods

        #endregion

        #region Rotator extensions

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that halts rotator movement
        /// </summary>
        /// <param name="device">The Rotator device</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when rotator movement has halted</returns>
        public static async Task HaltAsync(this IRotatorV3 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.Halt(); }, () => { return device.IsMoving; }, pollInterval, cancellationToken, logger, $"{nameof(IRotatorV3)}.{nameof(HaltAsync)}");
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that moves the rotator to the specified relative position
        /// </summary>
        /// <param name="device">The Rotator device</param>
        /// <param name="position">The required filter wheel position</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the rotator has moved by the specified amount</returns>
        public static async Task MoveAsync(this IRotatorV3 device, double position, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.Move(Convert.ToSingle(position)); }, () => { return device.IsMoving; }, pollInterval, cancellationToken, logger, $"{nameof(IRotatorV3)}.{nameof(MoveAsync)}", () => { return $"Position: {device.Position}"; });
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that moves the rotator to the specified absolute position
        /// </summary>
        /// <param name="device">The Rotator device</param>
        /// <param name="position">The required filter wheel position</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the rotator is at the required absolute position</returns>
        public static async Task MoveAbsoluteAsync(this IRotatorV3 device, double position, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.MoveAbsolute(Convert.ToSingle(position)); }, () => { return device.IsMoving; }, pollInterval, cancellationToken, logger, $"{nameof(IRotatorV3)}.{nameof(MoveAbsoluteAsync)}", () => { return $"Position: {device.Position}"; });
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that moves the rotator to the specified mechanical position
        /// </summary>
        /// <param name="device">The Rotator device</param>
        /// <param name="position">The required filter wheel position</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the rotator is at the required mechanical position</returns>
        public static async Task MoveMechanicalAsync(this IRotatorV3 device, double position, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.MoveMechanical(Convert.ToSingle(position)); }, () => { return device.IsMoving; }, pollInterval, cancellationToken, logger, $"{nameof(IRotatorV3)}.{nameof(MoveMechanicalAsync)}", () => { return $"Position: {device.Position}, Mechanical position: {device.MechanicalPosition}"; });
        }

        #endregion

        #region SafetyMonitor extensions

        // No asynchronous methods

        #endregion

        #region Switch extensions

        // No asynchronous methods

        #endregion

        #region Telescope extensions

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that slews the telescope to the specified altitude / azimuth coordinates
        /// </summary>
        /// <param name="device">The Telescope device</param>
        /// <param name="azimuth">The required azimuth</param>
        /// <param name="altitude">The required altitude</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the telescope is at the required coordinates</returns>
        public static async Task SlewToAltAzTaskAsync(this ITelescopeV3 device, double azimuth, double altitude, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.SlewToAltAzAsync(azimuth, altitude); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger, $"{nameof(ITelescopeV3)}.{nameof(SlewToAltAzTaskAsync)}", () => { return $"Altitude: {device.Altitude}, Azimuth: {device.Azimuth}"; });
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that slews the telescope to the specified RA/Dec coordinates
        /// </summary>
        /// <param name="device">The Telescope device</param>
        /// <param name="rightAscension">The required right ascension</param>
        /// <param name="declination">The required declination</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the telescope is at the required coordinates</returns>
        public static async Task SlewToCoordinatesTaskAsync(this ITelescopeV3 device, double rightAscension, double declination, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.SlewToCoordinatesAsync(rightAscension, declination); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger, $"{nameof(ITelescopeV3)}.{nameof(SlewToCoordinatesTaskAsync)}", () => { return $"RA: {device.RightAscension}, Declination: {device.Declination}"; });
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that slews the telescope to the coordinates specified by the TargetRA and TargetDeclination properties
        /// </summary>
        /// <param name="device">The Telescope device</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the telescope is at the required coordinates</returns>
        public static async Task SlewToTargetTaskAsync(this ITelescopeV3 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.SlewToTargetAsync(); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger, $"{nameof(ITelescopeV3)}.{nameof(SlewToTargetTaskAsync)}", () => { return $"RA: {device.RightAscension}, Declination: {device.Declination}"; });
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that stops telescope slewing movement
        /// </summary>
        /// <param name="device">The Telescope device</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when telescope slewing has stopped</returns>
        public static async Task AbortSlewAsync(this ITelescopeV3 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.AbortSlew(); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger, $"{nameof(ITelescopeV3)}.{nameof(AbortSlewAsync)}");
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that moves the telescope to the home position
        /// </summary>
        /// <param name="device">The Telescope device</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when telescope is at home</returns>
        public static async Task FindHomeAsync(this ITelescopeV3 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.FindHome(); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger, $"{nameof(ITelescopeV3)}.{nameof(FindHomeAsync)}", () => { return $"RA: {device.RightAscension}, Declination: {device.Declination}"; });
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that parks the telescope
        /// </summary>
        /// <param name="device">The Telescope device</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when telescope is at the park position</returns>
        public static async Task ParkAsync(this ITelescopeV3 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.Park(); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger, $"{nameof(ITelescopeV3)}.{nameof(ParkAsync)}", () => { return $"RA: {device.RightAscension}, Declination: {device.Declination}"; });
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that un-parks the telescope
        /// </summary>
        /// <param name="device">The Telescope device</param>
        /// <param name="cancellationToken">Cancellation token - Default: No cancellation token</param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when telescope is un-parked</returns>
        public static async Task UnparkAsync(this ITelescopeV3 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            await ProcessTask(() => { device.Unpark(); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger, $"{nameof(ITelescopeV3)}.{nameof(UnparkAsync)}");
        }

        #endregion

        #region Video extensions

        #endregion

        #region Support methods
        /// <summary>
        /// Runs a process asynchronously and waits for it to complete as determined by the associated completion variable
        /// </summary>
        /// <param name="initiatorMethod"></param>
        /// <param name="processRunningFunction"></param>
        /// <param name="pollInterval"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="logger"></param>
        /// <param name="callingMethodName"></param>
        /// <param name="updateFunction"></param>
        /// <exception cref="OperationCanceledException">If the task is cancelled</exception>
        /// <returns></returns>
        internal static async Task ProcessTask(Action initiatorMethod, Func<bool> processRunningFunction, int pollInterval, CancellationToken cancellationToken, ILogger logger, string callingMethodName, Func<string> updateFunction = null)
        {
            Stopwatch swLoop = new Stopwatch(); // Stopwatch to time each polling loop
            Stopwatch swOverall = new Stopwatch(); // Stopwatch to time the overall process

            int delayTime = pollInterval; // Adjustable delay time required to make the polling loop run at the pollInterval value. Initialised to pollInterval as a first approximation
            int loopTime; // Variable to record the loop time before the stopwatch is reset
            double totalLoopTime = 0.0; // Total time accumulated over all polling loops
            int averageLoopTime = 0; // Average time per polling loop
            double loopCount = 0; // Number of polling loops

            CancellationTokenRegistration cancellationTokenRegistration; // CancellationTokenRegistration to enable the cancellation event attached to the cancellationToken to be removed

            // Start the overall timing stopwatch
            swOverall.Start();

            // Start the operation by calling the initiator method
            logger?.LogMessage(LogLevel.Information, callingMethodName, $"{Thread.CurrentThread.ManagedThreadId:00} Calling initiator method");
            initiatorMethod();
            logger?.LogMessage(LogLevel.Debug, callingMethodName, $"{Thread.CurrentThread.ManagedThreadId:00} Initiator completed");

            // Create a completion source that enables the task to be set to a complete status
            TaskCompletionSource<object> taskhasBeenCancelled = new TaskCompletionSource<object>();

            // Add an event handler to the cancellation token that will fire when the task is cancelled.
            using (cancellationTokenRegistration = cancellationToken.Register(() =>
            {
                // Set the taskhasBeenCancelled task result, which will end this task
                logger?.LogMessage(LogLevel.Debug, callingMethodName, $"{Thread.CurrentThread.ManagedThreadId:00} Task cancelled, setting cancellation task result...");
                taskhasBeenCancelled.SetResult(null);
                logger?.LogMessage(LogLevel.Debug, callingMethodName, $"{Thread.CurrentThread.ManagedThreadId:00} Cancellation task result set");
            }))
            {
                // Create and run an async task to effect the polling loop
                Task completionTask = Task.Run(async () =>
                {
                    logger?.LogMessage(LogLevel.Information, callingMethodName, $"{Thread.CurrentThread.ManagedThreadId:00} Starting wait for process to complete");

                    // Start the loop timer outside the loop so that an accurate loop time is received on the first loop
                    swLoop.Restart();

                    // Wait for the operation to complete
                    while (processRunningFunction() & !cancellationToken.IsCancellationRequested)  // Test whether the operation has completed or whether it has been cancelled
                    {
                        // Wait for the required delay time
                        await Task.Delay(delayTime, cancellationToken); // Delay will end early if the task is cancelled

                        // Record the overall loop time and restart the stopwatch for the next loop
                        loopTime = (int)swLoop.ElapsedMilliseconds;
                        swLoop.Restart();

                        // Calculate the average loop overhead
                        loopCount += 1; // Increment the count of the number of loops
                        totalLoopTime += loopTime - delayTime; // Add the loop time of this loop to the total
                        averageLoopTime = Convert.ToInt32(totalLoopTime / loopCount); // Calculate the average loop time from the total loop time and the number of loops

                        // Calculate an updated delay time value for the next loop
                        delayTime = Math.Max(0, pollInterval - averageLoopTime); // Ensure that delay is always 0 or greater

                        // Log the loop outcome
                        logger?.LogMessage(LogLevel.Debug, callingMethodName, $"{Thread.CurrentThread.ManagedThreadId:00} Waiting {pollInterval}ms for action to complete... Delay time{delayTime}, Average loop overhead: {averageLoopTime:0.0}ms, Loop time: {loopTime}{(updateFunction is null ? "" : $", {updateFunction()}")}");
                    };

                }, cancellationToken);

                // Wait for either the polling task to complete or for the task to be cancelled
                await Task.WhenAny(completionTask, taskhasBeenCancelled.Task);

                // Log the final outcome
                if (cancellationToken.IsCancellationRequested)
                {
                    logger?.LogMessage(LogLevel.Information, callingMethodName, $"{Thread.CurrentThread.ManagedThreadId:00} PROCESS CANCELLED after {swOverall.Elapsed.TotalSeconds:0.0} seconds ({swOverall.ElapsedMilliseconds}ms)");

                    // Throw an operation cancelled exception if the operation was cancelled
                    cancellationToken.ThrowIfCancellationRequested();
                }
                else
                {
                    // Check whether the polling process completed OK or faulted
                    switch (completionTask.Status)
                    {
                        case TaskStatus.RanToCompletion:
                            {
                                logger?.LogMessage(LogLevel.Information, callingMethodName, $"{Thread.CurrentThread.ManagedThreadId:00} Process completed in {swOverall.Elapsed.TotalSeconds:0.0} seconds ({swOverall.ElapsedMilliseconds}ms)");
                                break;
                            }

                        case TaskStatus.Faulted:
                            {
                                logger?.LogMessage(LogLevel.Information, callingMethodName, $"{Thread.CurrentThread.ManagedThreadId:00} Process FAULTED in {swOverall.Elapsed.TotalSeconds:0.0} seconds ({swOverall.ElapsedMilliseconds}ms)");

                                // Log and throw the returned exception
                                logger?.LogMessage(LogLevel.Debug, callingMethodName, $"{Thread.CurrentThread.ManagedThreadId:00} Exception: {completionTask.Exception?.Flatten().InnerExceptions.First()}");
                                throw completionTask.Exception?.Flatten().InnerExceptions.First();
                            }

                        default:
                            {
                                logger?.LogMessage(LogLevel.Information, callingMethodName, $"{Thread.CurrentThread.ManagedThreadId:00} Process status: {completionTask.Status}, finished in {swOverall.Elapsed.TotalSeconds:0.0} seconds ({swOverall.ElapsedMilliseconds}ms)");
                                break;
                            }
                    }
                }
            }
        }

        #endregion

    }
}
