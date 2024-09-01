using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.Interfaces;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ASCOM.Common
{
    /// <summary>
    /// Asynchronous extensions for the Alpaca clients and COM clients that return awaitable tasks for long running methods.
    /// </summary>
    public static class ClientExtensions
    {

        #region IAscomDeviceV2 methods

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that connects to the device. (Polls IAscomDeviceV2.Connecting)
        /// </summary>
        /// <param name="device">A device that implements IAscomDeviceV2.</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the device has connected.</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="IAscomDeviceV2.Connect"/></para>
        /// <para>Complete when: <see cref="IAscomDeviceV2.Connecting"/> is  False </para>
        /// <para>Only available for IAscomDeviceV2 and later interfaces.</para>
        /// </remarks>
        public static async Task ConnectAsync(this IAscomDeviceV2 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.Connect(); }, () => { return device.Connecting; }, pollInterval, cancellationToken, logger, $"{nameof(IAscomDeviceV2)}.{nameof(ConnectAsync)}");
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that disconnects from the device. (Polls IAscomDeviceV2.Connecting)
        /// </summary>
        /// <param name="device">A device that implements IAscomDeviceV2.</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the device has dis connected.</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="IAscomDeviceV2.Disconnect"/></para>
        /// <para>Complete when: <see cref="IAscomDeviceV2.Connecting"/> is  False </para>
        /// <para>Only available for IAscomDeviceV2 and later interfaces.</para>
        /// </remarks>
        public static async Task DisconnectAsync(this IAscomDeviceV2 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.Disconnect(); }, () => { return device.Connecting; }, pollInterval, cancellationToken, logger, $"{nameof(IAscomDeviceV2)}.{nameof(DisconnectAsync)}");
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        #endregion

        #region ICameraV3 extensions

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that takes a camera image
        /// </summary>
        /// <param name="device">The Camera device</param>
        /// <param name="duration">Length of exposure</param>
        /// <param name="light"><see langword="true"/> for light frames, <see langword="false"/> for dark frames</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the exposure is complete</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="ICameraV3.StartExposure(double, bool)"/></para>
        /// <para>Complete when: <see cref="ICameraV3.CameraState"/> is <see cref="CameraState.Idle"/> or <see cref="CameraState.Error"/></para>
        /// </remarks>
        public static async Task StartExposureAsync(this ICameraV3 device, double duration, bool light, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.StartExposure(duration, light); }, () =>
                        { return (device.CameraState == CameraState.Waiting) | (device.CameraState == CameraState.Exposing) | (device.CameraState == CameraState.Reading); },
                        pollInterval, cancellationToken, logger, $"{nameof(ICameraV3)}.{nameof(StartExposureAsync)}");
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that stops the current camera exposure
        /// </summary>
        /// <param name="device">The Camera device</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the exposure is has stopped</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="ICameraV3.StopExposure"/></para>
        /// <para>Complete when: <see cref="ICameraV3.CameraState"/> is <see cref="CameraState.Idle"/> or <see cref="CameraState.Error"/></para>
        /// </remarks>
        public static async Task StopExposureAsync(this ICameraV3 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.StopExposure(); }, () =>
                        { return (device.CameraState == CameraState.Reading) | (device.CameraState == CameraState.Exposing) | (device.CameraState == CameraState.Waiting); },
                        pollInterval, cancellationToken, logger, $"{nameof(ICameraV3)}.{nameof(StopExposureAsync)}");
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        #endregion

        #region ICoverCalibratorV1 extensions

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that turns the calibrator off
        /// </summary>
        /// <param name="device">The CoverCalibrator device</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the calibrator is off</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="ICoverCalibratorV1.CalibratorOff"/></para>
        /// <para>Complete when: <see cref="ICoverCalibratorV1.CalibratorState"/> is <see cref="CalibratorStatus.NotPresent"/> or <see cref="CalibratorStatus.Off"/> <see cref="CalibratorStatus.Ready"/> or <see cref="CalibratorStatus.Unknown"/> or <see cref="CalibratorStatus.Error"/></para>
        /// </remarks>
        public static async Task CalibratorOffAsync(this ICoverCalibratorV1 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.CalibratorOff(); }, () => { return device.CalibratorState == CalibratorStatus.NotReady; }, pollInterval, cancellationToken, logger, $"{nameof(ICoverCalibratorV1)}.{nameof(CalibratorOffAsync)}");
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that turns the calibrator on
        /// </summary>
        /// <param name="device">The CoverCalibrator device</param>
        /// <param name="brightness">Required brightness level</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the calibrator is on</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="ICoverCalibratorV1.CalibratorOn(int)"/></para>
        /// <para>Complete when: <see cref="ICoverCalibratorV1.CalibratorState"/> is <see cref="CalibratorStatus.NotPresent"/> or <see cref="CalibratorStatus.Off"/> <see cref="CalibratorStatus.Ready"/> or <see cref="CalibratorStatus.Unknown"/> or <see cref="CalibratorStatus.Error"/></para>
        /// </remarks>
        public static async Task CalibratorOnAsync(this ICoverCalibratorV1 device, int brightness, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.CalibratorOn(brightness); }, () => { return device.CalibratorState == CalibratorStatus.NotReady; }, pollInterval, cancellationToken, logger, $"{nameof(ICoverCalibratorV1)}.{nameof(CalibratorOnAsync)}");
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that closes the cover
        /// </summary>
        /// <param name="device">The CoverCalibrator device</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the cover is closed</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="ICoverCalibratorV1.CloseCover"/></para>
        /// <para>Complete when: <see cref="ICoverCalibratorV1.CoverState"/> is <see cref="CoverStatus.NotPresent"/> or <see cref="CoverStatus.Closed"/> or <see cref="CoverStatus.Unknown"/> or <see cref="CoverStatus.Error"/></para>
        /// </remarks>
        public static async Task CloseCoverAsync(this ICoverCalibratorV1 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.CloseCover(); }, () => { return (device.CoverState == CoverStatus.Moving) | (device.CoverState == CoverStatus.Open); }, pollInterval, cancellationToken, logger, $"{nameof(ICoverCalibratorV1)}.{nameof(CloseCoverAsync)}");
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that halts cover movement
        /// </summary>
        /// <param name="device">The CoverCalibrator device</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when cover movement has stopped</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="ICoverCalibratorV1.HaltCover"/></para>
        /// <para>Complete when: <see cref="ICoverCalibratorV1.CoverState"/> is <see cref="CoverStatus.NotPresent"/> or <see cref="CoverStatus.Closed"/> <see cref="CoverStatus.Open"/> or <see cref="CoverStatus.Unknown"/> or <see cref="CoverStatus.Error"/></para>
        /// </remarks>
        public static async Task HaltCoverAsync(this ICoverCalibratorV1 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.HaltCover(); }, () => { return device.CoverState == CoverStatus.Moving; }, pollInterval, cancellationToken, logger, $"{nameof(ICoverCalibratorV1)}.{nameof(HaltCoverAsync)}");
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that opens the cover
        /// </summary>
        /// <param name="device">The CoverCalibrator device</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the cover is open</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="ICoverCalibratorV1.OpenCover"/></para>
        /// <para>Complete when: <see cref="ICoverCalibratorV1.CoverState"/> is <see cref="CoverStatus.NotPresent"/> or <see cref="CoverStatus.Open"/> or <see cref="CoverStatus.Unknown"/> or <see cref="CoverStatus.Error"/></para>
        /// </remarks>
        public static async Task OpenCoverAsync(this ICoverCalibratorV1 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.OpenCover(); }, () => { return (device.CoverState == CoverStatus.Moving) | (device.CoverState == CoverStatus.Closed); }, pollInterval, cancellationToken, logger, $"{nameof(ICoverCalibratorV1)}.{nameof(OpenCoverAsync)}");
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        #endregion

        #region IDomeV2 extensions

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that halts all dome movement
        /// </summary>
        /// <param name="device">The Dome device</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when all dome movement has stopped</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="IDomeV2.AbortSlew"/></para>
        /// <para>Complete when: <see cref="IDomeV2.Slewing"/> is <see langword="false"/></para>
        /// </remarks>
        public static async Task AbortSlewAsync(this IDomeV2 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.AbortSlew(); }, () => { return device.Slewing | (device.ShutterStatus == ShutterState.Opening) | (device.ShutterStatus == ShutterState.Closing); }, pollInterval, cancellationToken, logger, $"{nameof(IDomeV2)}.{nameof(AbortSlewAsync)}");
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that closes the dome shutter
        /// </summary>
        /// <param name="device">The Dome device</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when cover is closed</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="IDomeV2.CloseShutter"/></para>
        /// <para>Complete when: <see cref="IDomeV2.ShutterStatus"/> is <see cref="ShutterState.Closed"/> or <see cref="ShutterState.Error"/></para>
        /// </remarks>
        public static async Task CloseShutterAsync(this IDomeV2 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.CloseShutter(); }, () => { return device.ShutterStatus == ShutterState.Closing; }, pollInterval, cancellationToken, logger, $"{nameof(IDomeV2)}.{nameof(CloseShutterAsync)}");
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that moves the dome to the home position
        /// </summary>
        /// <param name="device">The Dome device</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <remarks>
        /// <para>Initiator: <see cref="IDomeV2.FindHome"/></para>
        /// <para>Complete when: <see cref="IDomeV2.Slewing"/> is <see langword="false"/></para>
        /// </remarks>
        /// <returns>Awaitable task that ends when the dome is at its home position</returns>
        public static async Task FindHomeAsync(this IDomeV2 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.FindHome(); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger, $"{nameof(IDomeV2)}.{nameof(FindHomeAsync)}");
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that opens the dome shutter
        /// </summary>
        /// <param name="device">The Dome device</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the shutter is open</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="IDomeV2.OpenShutter"/></para>
        /// <para>Complete when: <see cref="IDomeV2.ShutterStatus"/> is <see cref="ShutterState.Open"/> or <see cref="ShutterState.Error"/></para>
        /// </remarks>
        public static async Task OpenShutterAsync(this IDomeV2 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.OpenShutter(); }, () => { return device.ShutterStatus == ShutterState.Opening; }, pollInterval, cancellationToken, logger, $"{nameof(IDomeV2)}.{nameof(OpenShutterAsync)}");
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that parks the dome
        /// </summary>
        /// <param name="device">The Dome device</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the dome is at its park position</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="IDomeV2.Park"/></para>
        /// <para>Complete when: <see cref="IDomeV2.Slewing"/> is <see langword="false"/></para>
        /// </remarks>
        public static async Task ParkAsync(this IDomeV2 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.Park(); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger, $"{nameof(IDomeV2)}.{nameof(ParkAsync)}");
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that slews the dome to the specified altitude
        /// </summary>
        /// <param name="device">The Dome device</param>
        /// <param name="altitude">The CoverCalibrator device</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the dome is at the required altitude</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="IDomeV2.SlewToAltitude(double)"/></para>
        /// <para>Complete when: <see cref="IDomeV2.Slewing"/> is <see langword="false"/></para>
        /// </remarks>
        public static async Task SlewToAltitudeAsync(this IDomeV2 device, double altitude, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.SlewToAltitude(altitude); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger, $"{nameof(IDomeV2)}.{nameof(SlewToAltitudeAsync)}");
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that slews the dome to the specified azimuth
        /// </summary>
        /// <param name="device">The Dome device</param>
        /// <param name="azimuth">The CoverCalibrator device</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the dome is at the required azimuth</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="IDomeV2.SlewToAzimuth(double)"/></para>
        /// <para>Complete when: <see cref="IDomeV2.Slewing"/> is <see langword="false"/></para>
        /// </remarks>
        public static async Task SlewToAzimuthAsync(this IDomeV2 device, double azimuth, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.SlewToAzimuth(azimuth); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger, $"{nameof(IDomeV2)}.{nameof(SlewToAzimuthAsync)}");
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        #endregion

        #region IFilterWheelV2 extensions

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that moves the filter wheel to the specified filter wheel position
        /// </summary>
        /// <param name="device">The FilterWheel device</param>
        /// <param name="position">The required filter wheel position</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the filter wheel is at the required position</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="IFilterWheelV2.Position">Set Postion</see></para>
        /// <para>Complete when: <see cref="IFilterWheelV2.Position">Get Position</see> is zero or greater</para>
        /// </remarks>
        public static async Task PositionSetAsync(this IFilterWheelV2 device, int position, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.Position = Convert.ToInt16(position); }, () => { return device.Position == -1; }, pollInterval, cancellationToken, logger, $"{nameof(IFilterWheelV2)}.{nameof(PositionSetAsync)}");
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        #endregion

        #region IFocuserV3 extensions

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that halts focuser movement
        /// </summary>
        /// <param name="device">The Focuser device</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when focuser movement has halted</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="IFocuserV3.Halt"/></para>
        /// <para>Complete when: <see cref="IFocuserV3.IsMoving"/> is <see langword="false"/></para>
        /// </remarks>
        public static async Task HaltAsync(this IFocuserV3 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.Halt(); }, () => { return device.IsMoving; }, pollInterval, cancellationToken, logger, $"{nameof(IFocuserV3)}.{nameof(HaltAsync)}", () => { return $"Position: {device.Position}"; });
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that moves to the specified focuser position
        /// </summary>
        /// <param name="device">The Focuser device</param>
        /// <param name="position">The required filter wheel position</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the focuser is at the required position</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="IFocuserV3.Move(int)"/></para>
        /// <para>Complete when: <see cref="IFocuserV3.IsMoving"/> is <see langword="false"/></para>
        /// </remarks>
        public static async Task MoveAsync(this IFocuserV3 device, int position, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.Move(position); }, () => { return device.IsMoving; }, pollInterval, cancellationToken, logger, $"{nameof(IFocuserV3)}.{nameof(MoveAsync)}", () => { return $"Position: {device.Position}"; });
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        #endregion

        #region ObservingConditions extensions

        // No long running methods

        #endregion

        #region IRotatorV3 extensions

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that halts rotator movement
        /// </summary>
        /// <param name="device">The Rotator device</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when rotator movement has halted</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="IRotatorV3.Halt"/></para>
        /// <para>Complete when: <see cref="IRotatorV3.IsMoving"/> is <see langword="false"/></para>
        /// </remarks>
        public static async Task HaltAsync(this IRotatorV3 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.Halt(); }, () => { return device.IsMoving; }, pollInterval, cancellationToken, logger, $"{nameof(IRotatorV3)}.{nameof(HaltAsync)}");
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that moves the rotator to the specified relative position
        /// </summary>
        /// <param name="device">The Rotator device</param>
        /// <param name="position">The required filter wheel position</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the rotator has moved by the specified amount</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="IRotatorV3.Move(float)"/></para>
        /// <para>Complete when: <see cref="IRotatorV3.IsMoving"/> is <see langword="false"/></para>
        /// </remarks>
        public static async Task MoveAsync(this IRotatorV3 device, double position, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.Move(Convert.ToSingle(position)); }, () => { return device.IsMoving; }, pollInterval, cancellationToken, logger, $"{nameof(IRotatorV3)}.{nameof(MoveAsync)}", () => { return $"Position: {device.Position}"; });
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that moves the rotator to the specified absolute position
        /// </summary>
        /// <param name="device">The Rotator device</param>
        /// <param name="position">The required filter wheel position</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the rotator is at the required absolute position</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="IRotatorV3.MoveAbsolute(float)"/></para>
        /// <para>Complete when: <see cref="IRotatorV3.IsMoving"/> is <see langword="false"/></para>
        /// </remarks>
        public static async Task MoveAbsoluteAsync(this IRotatorV3 device, double position, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.MoveAbsolute(Convert.ToSingle(position)); }, () => { return device.IsMoving; }, pollInterval, cancellationToken, logger, $"{nameof(IRotatorV3)}.{nameof(MoveAbsoluteAsync)}", () => { return $"Position: {device.Position}"; });
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that moves the rotator to the specified mechanical position
        /// </summary>
        /// <param name="device">The Rotator device</param>
        /// <param name="position">The required filter wheel position</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the rotator is at the required mechanical position</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="IRotatorV3.MoveMechanical(float)"/></para>
        /// <para>Complete when: <see cref="IRotatorV3.IsMoving"/> is <see langword="false"/></para>
        /// </remarks>
        public static async Task MoveMechanicalAsync(this IRotatorV3 device, double position, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.MoveMechanical(Convert.ToSingle(position)); }, () => { return device.IsMoving; }, pollInterval, cancellationToken, logger, $"{nameof(IRotatorV3)}.{nameof(MoveMechanicalAsync)}", () => { return $"Position: {device.Position}, Mechanical position: {device.MechanicalPosition}"; });
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        #endregion

        #region SafetyMonitor extensions

        // No asynchronous methods

        #endregion

        #region ISwitchV3 extensions

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that sets the switch to a given boolean state
        /// </summary>
        /// <param name="device">The Switch device</param>
        /// <param name="id">The switch ID number</param>
        /// <param name="state">The desired boolean state</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the switch has changed to the designated state</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="ISwitchV3.SetAsync(short, bool)"/></para>
        /// <para>Complete when: <see cref="ISwitchV3.StateChangeComplete(short)"/> is <see langword="true"/></para>
        /// <para>Only available for ISwitchV3 and later devices.</para>
        /// </remarks>
        public static async Task SetAsync(this ISwitchV3 device, short id, bool state, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.SetAsync(id, state); }, () => { return !device.StateChangeComplete(id); }, pollInterval, cancellationToken, logger, $"{nameof(ISwitchV3)}.{nameof(SetAsync)}", () => { return $"State is changing: {!device.StateChangeComplete(id)}"; });
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that sets the switch to a given value
        /// </summary>
        /// <param name="device">The Switch device</param>
        /// <param name="id">The switch ID number</param>
        /// <param name="value">The new switch value</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the switch has changed to the designated state</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="ISwitchV3.SetAsync(short, bool)"/></para>
        /// <para>Complete when: <see cref="ISwitchV3.StateChangeComplete(short)"/> is <see langword="true"/></para>
        /// <para>Only available for ISwitchV3 and later devices.</para>
        /// </remarks>
        public static async Task SetAsyncValue(this ISwitchV3 device, short id, double value, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.SetAsyncValue(id, value); }, () => { return !device.StateChangeComplete(id); }, pollInterval, cancellationToken, logger, $"{nameof(ISwitchV3)}.{nameof(SetAsync)}", () => { return $"State is changing: {!device.StateChangeComplete(id)}"; });
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        #endregion

        #region ITelescopeV3 extensions

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that slews the telescope to the specified altitude / azimuth coordinates
        /// </summary>
        /// <param name="device">The Telescope device</param>
        /// <param name="azimuth">The required azimuth</param>
        /// <param name="altitude">The required altitude</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the telescope is at the required coordinates</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="ITelescopeV3.SlewToAltAzAsync(double, double)"/></para>
        /// <para>Complete when: <see cref="ITelescopeV3.Slewing"/> is <see langword="false"/></para>
        /// </remarks>
        public static async Task SlewToAltAzTaskAsync(this ITelescopeV3 device, double azimuth, double altitude, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.SlewToAltAzAsync(azimuth, altitude); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger, $"{nameof(ITelescopeV3)}.{nameof(SlewToAltAzTaskAsync)}", () => { return $"Altitude: {device.Altitude}, Azimuth: {device.Azimuth}"; });
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that slews the telescope to the specified RA/Dec coordinates
        /// </summary>
        /// <param name="device">The Telescope device</param>
        /// <param name="rightAscension">The required right ascension</param>
        /// <param name="declination">The required declination</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the telescope is at the required coordinates</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="ITelescopeV3.SlewToCoordinatesAsync(double, double)"/></para>
        /// <para>Complete when: <see cref="ITelescopeV3.Slewing"/> is <see langword="false"/></para>
        /// </remarks>
        public static async Task SlewToCoordinatesTaskAsync(this ITelescopeV3 device, double rightAscension, double declination, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.SlewToCoordinatesAsync(rightAscension, declination); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger, $"{nameof(ITelescopeV3)}.{nameof(SlewToCoordinatesTaskAsync)}", () => { return $"RA: {device.RightAscension}, Declination: {device.Declination}"; });
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that slews the telescope to the coordinates specified by the TargetRA and TargetDeclination properties
        /// </summary>
        /// <param name="device">The Telescope device</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when the telescope is at the required coordinates</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="ITelescopeV3.SlewToTargetAsync()"/></para>
        /// <para>Complete when: <see cref="ITelescopeV3.Slewing"/> is <see langword="false"/></para>
        /// </remarks>
        public static async Task SlewToTargetTaskAsync(this ITelescopeV3 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.SlewToTargetAsync(); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger, $"{nameof(ITelescopeV3)}.{nameof(SlewToTargetTaskAsync)}", () => { return $"RA: {device.RightAscension}, Declination: {device.Declination}"; });
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that stops telescope slewing movement
        /// </summary>
        /// <param name="device">The Telescope device</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when telescope slewing has stopped</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="ITelescopeV3.AbortSlew()"/></para>
        /// <para>Complete when: <see cref="ITelescopeV3.Slewing"/> is <see langword="false"/></para>
        /// </remarks>
        public static async Task AbortSlewAsync(this ITelescopeV3 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.AbortSlew(); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger, $"{nameof(ITelescopeV3)}.{nameof(AbortSlewAsync)}");
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that moves the telescope to the home position
        /// </summary>
        /// <param name="device">The Telescope device</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when telescope is at home</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="ITelescopeV3.FindHome()"/></para>
        /// <para>Complete when: <see cref="ITelescopeV3.Slewing"/> is <see langword="false"/></para>
        /// </remarks>
        public static async Task FindHomeAsync(this ITelescopeV3 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.FindHome(); }, () => { return device.Slewing; }, pollInterval, cancellationToken, logger, $"{nameof(ITelescopeV3)}.{nameof(FindHomeAsync)}", () => { return $"RA: {device.RightAscension}, Declination: {device.Declination}"; });
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that parks the telescope
        /// </summary>
        /// <param name="device">The Telescope device</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when telescope is at the park position</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="ITelescopeV3.Park()"/></para>
        /// <para>Complete when: <see cref="ITelescopeV3.AtPark"/> is <see langword="true"/></para>
        /// </remarks>
        public static async Task ParkAsync(this ITelescopeV3 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.Park(); }, () => { return !device.AtPark; }, pollInterval, cancellationToken, logger, $"{nameof(ITelescopeV3)}.{nameof(ParkAsync)}", () => { return $"RA: {device.RightAscension}, Declination: {device.Declination}"; });
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        #endregion

        #region ITelescopeV4 extensions

        /// <summary>
        /// Returns an awaitable, running, <see cref="Task"/> that un-parks the telescope
        /// </summary>
        /// <param name="device">The Telescope device</param>
        /// <param name="cancellationToken">Cancellation token - Default: <see cref="CancellationToken.None"/></param>
        /// <param name="pollInterval">Interval between polls of the completion variable (milliseconds) - Default: 1000 milliseconds.</param>
        /// <param name="logger">ILogger instance that will receive operation messages from the method - Default: No logger</param>
        /// <returns>Awaitable task that ends when telescope is un-parked</returns>
        /// <remarks>
        /// <para>Initiator: <see cref="ITelescopeV4.Unpark()"/></para>
        /// <para>Complete when: <see cref="ITelescopeV3.AtPark"/> is <see langword="false"/></para>
        /// </remarks>
        public static async Task UnparkAsync(this ITelescopeV4 device, CancellationToken cancellationToken = default, int pollInterval = 1000, ILogger logger = null)
        {
            Task processTask = null;
            string callingMethodName = $"[Lib].{GetCurrentMethod()}";

            await Task.Run(() =>
            {
                processTask = Task.Run(() =>
                {
                    ProcessTask(() => { device.Unpark(); }, () => { return device.AtPark; }, pollInterval, cancellationToken, logger, $"{nameof(ITelescopeV4)}.{nameof(UnparkAsync)}");
                });

                WaitForProcessTask(processTask, logger, callingMethodName, cancellationToken);
            });

            CheckOutcome(processTask, logger, callingMethodName, cancellationToken);
        }

        #endregion

        #region Video extensions

        #endregion

        #region Support methods

        /// <summary>
        /// Returns the current method name
        /// </summary>
        /// <param name="callerName">The caller's name.</param>
        /// <returns>Callers method name</returns>
        private static string GetCurrentMethod([CallerMemberName] string callerName = "")
        {
            return callerName;
        }

        /// <summary>
        /// Reports and handles the outcome of the async operation
        /// </summary>
        /// <param name="processTask">Task running the process</param>
        /// <param name="logger">ILogger to which operational debug information is written</param>
        /// <param name="callingMethodName">Name of the original async method that was called by the client application.</param>
        /// <param name="cancellationToken">Cancellation token supplied by the client application.</param>
        /// <exception cref="Exception">Throws any exception returned by the driver / device.</exception>
        /// <remarks>The async process will either have completed or the cancellation token will be set (by timeout or user action) by the time this method is called.</remarks>
        private static void CheckOutcome(Task processTask, ILogger logger, string callingMethodName, CancellationToken cancellationToken)
        {
            // Check whether the process is faulted i.e. it threw an exception
            if (processTask.IsFaulted) // Process threw an exception
            {
                // Log the information
                logger?.LogMessage(LogLevel.Information, callingMethodName, $"CheckOutcome - Exception message: {processTask?.Exception.Message}, " +
                                    $"Exception type: {processTask?.Exception.GetType().Name}, " +
                                    $"InnerException message: {processTask?.Exception?.InnerException?.Message}, " +
                                    $"Inner exception type: {processTask?.Exception?.InnerException?.GetType().Name}" +
                                    $"\r\n*****\r\n{processTask.Exception}\r\n*****\r\n");

                // Throw the most helpful exception back to the client application
                if (processTask.Exception is null) // Faulted but no exception so generate our own exception
                    throw new Exception($"The {callingMethodName} process reported an exception but no exception could be retrieved from the process task.");
                else // An exception was thrown
                {
                    // Return the exception's inner exception if possible otherwise return just return the exception
                    if (processTask.Exception.InnerException is null) // No inner exception
                    {
                        throw processTask.Exception;
                    }
                    else // Has an inner exception so return this
                    {
                        throw processTask.Exception.InnerException;
                    }
                }
            }

            // If we get here the process either ran to completion or was cancelled. Test whether it was cancelled and create a log message if this is the case.
            if (cancellationToken.IsCancellationRequested) // 
                logger?.LogMessage(LogLevel.Information, callingMethodName, $"{Thread.CurrentThread.ManagedThreadId:00} CheckOutcome - Cancellation requested, throwing OperationCanceledException.");

            // Throw an OperationCanceledException if the operation was cancelled.
            cancellationToken.ThrowIfCancellationRequested();

            // If we get here the operation completed normally so return to the caller.
        }

        /// <summary>
        /// Wait for the async process to complete, fault or time out.
        /// </summary>
        /// <param name="processTask">Task running the process</param>
        /// <param name="logger">ILogger to which operational debug information is written</param>
        /// <param name="callingMethodName">Name of the original async method that was called by the client application.</param>
        /// <param name="cancellationToken">Cancellation token supplied by the client application.</param>
        private static void WaitForProcessTask(Task processTask, ILogger logger, string callingMethodName, CancellationToken cancellationToken)
        {
            // Create and run a task to periodically test whether the cancellation token has been activated by the user or by timing out.
            Task timeoutTask = Task.Run(() =>
            {
                do
                {
                    logger?.LogMessage(LogLevel.Debug, callingMethodName, $"{Thread.CurrentThread.ManagedThreadId:00} {nameof(WaitForProcessTask)} - Loop - Process status: {processTask.Status}, Cancellation requested: {cancellationToken.IsCancellationRequested}");

                    // Test whether the operation has been cancelled or has timed out
                    if (cancellationToken.IsCancellationRequested) // The operation has been cancelled or has timed out so log the information and exit the polling loop.
                    {
                        logger?.LogMessage(LogLevel.Information, callingMethodName, $"{Thread.CurrentThread.ManagedThreadId:00} {nameof(WaitForProcessTask)} - Loop - Cancellation requested - Process status: {processTask.Status}, Cancellation requested: {cancellationToken.IsCancellationRequested}");
                        break;
                    }
                    if (processTask?.Status >= TaskStatus.RanToCompletion) // The operation has completed or faulted so log the information and exit the polling loop.
                    {
                        logger?.LogMessage(LogLevel.Information, callingMethodName, $"{Thread.CurrentThread.ManagedThreadId:00} {nameof(WaitForProcessTask)} - Loop - Process ran to completion - Process status: {processTask.Status}, Cancellation requested: {cancellationToken.IsCancellationRequested}");
                        break;
                    }

                    //Wait for 100ms before polling again
                    Thread.Sleep(100);
                } while (true); // Loop for ever until one of the if statements above becomes true

                // End of the timeout task
            });

            // Wait for either the process to complete or the timeout task to complete (Note that WaitAny does NOT automatically throw an exception if the process faults.)
            Task.WaitAny(processTask, timeoutTask);

            // Log the outcome
            logger?.LogMessage(LogLevel.Information, callingMethodName, $"{Thread.CurrentThread.ManagedThreadId:00} {nameof(WaitForProcessTask)} - Process task status: {processTask.Status}, Timeout task status: {timeoutTask.Status}");
        }

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
        private static void ProcessTask(Action initiatorMethod, Func<bool> processRunningFunction, int pollInterval, CancellationToken cancellationToken, ILogger logger, string callingMethodName, Func<string> updateFunction = null)
        {
            Stopwatch swLoop = new Stopwatch(); // Stopwatch to time each polling loop
            Stopwatch swOverall = new Stopwatch(); // Stopwatch to time the overall process

            int delayTime = pollInterval; // Adjustable delay time required to make the polling loop run at the pollInterval value. Initialised to pollInterval as a first approximation
            int loopTime; // Variable to record the loop time before the stopwatch is reset
            double totalLoopTime = 0.0; // Total time accumulated over all polling loops
            double loopCount = 0; // Number of polling loops

            // Start the overall timing stopwatch
            swOverall.Start();

            // Start the operation by calling the initiator method
            logger?.LogMessage(LogLevel.Information, callingMethodName, $"{Thread.CurrentThread.ManagedThreadId:00} Calling initiator method");
            initiatorMethod();
            logger?.LogMessage(LogLevel.Debug, callingMethodName, $"{Thread.CurrentThread.ManagedThreadId:00} Initiator completed");

            // Create and run an async task to effect the polling loop
            logger?.LogMessage(LogLevel.Information, callingMethodName, $"{Thread.CurrentThread.ManagedThreadId:00} Starting wait for process to complete");

            // Start the loop timer outside the loop so that an accurate loop time is received on the first loop
            swLoop.Restart();

            // Wait for the operation to complete
            while (processRunningFunction() & !cancellationToken.IsCancellationRequested)  // Test whether the operation has completed or whether it has been cancelled
            {
                // Wait for the required delay time
                Thread.Sleep(delayTime); // Delay will end early if the task is cancelled

                // Record the overall loop time and restart the stopwatch for the next loop
                loopTime = (int)swLoop.ElapsedMilliseconds;
                swLoop.Restart();

                // Calculate the average loop overhead
                loopCount += 1; // Increment the count of the number of loops
                totalLoopTime += loopTime - delayTime; // Add the loop time of this loop to the total
                int averageLoopTime = Convert.ToInt32(totalLoopTime / loopCount);

                // Calculate an updated delay time value for the next loop
                delayTime = Math.Max(0, pollInterval - averageLoopTime); // Ensure that delay is always 0 or greater

                // Log the loop outcome
                logger?.LogMessage(LogLevel.Debug, callingMethodName, $"{Thread.CurrentThread.ManagedThreadId:00} Waiting {pollInterval}ms for action to complete... Delay time{delayTime}, Average loop overhead: {averageLoopTime:0.0}ms, Loop time: {loopTime}{(updateFunction is null ? "" : $", {updateFunction()}")}");
            };

            // Log the final outcome
            logger?.LogMessage(LogLevel.Information, callingMethodName, $"{Thread.CurrentThread.ManagedThreadId:00} Process finished in {swOverall.Elapsed.TotalSeconds:0.000} seconds. Process was cancelled: {cancellationToken.IsCancellationRequested}.");
        }

        #endregion

    }
}
