using System.Collections.Generic;

namespace ASCOM.Common.DeviceInterfaces
{
    // -----------------------------------------------------------------------
    // <summary>Defines the ICamera Interface</summary>
    // -----------------------------------------------------------------------
    /// <summary>
    /// Defines the ICamera Interface
    /// </summary>
    /// <remarks>The camera state diagram is shown here: <img src="../media/Camerav2 State Diagram.png"/></remarks>
    public interface ICameraV3 : IAscomDevice
    {

        #region ICameraV1 members

        /// <summary>
        /// Aborts the current exposure, if any, and returns the camera to Idle state.
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.AbortExposure">Canonical definition</see></remarks>
        /// <exception cref="NotImplementedException">If CanAbortExposure is false.</exception>
        /// <exception cref="InvalidOperationException">Thrown if abort is not currently possible (e.g. during download).</exception>
        /// <exception cref="NotConnectedException">Thrown if the driver is not connected.</exception>
        /// <exception cref="DriverException">Thrown if a communications error occurs, or if the abort fails.</exception>
        void AbortExposure();

        /// <summary>
        /// Sets the binning factor for the X axis, also returns the current value.
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.BinX">Canonical definition</see></remarks>
        /// <value>The X binning value</value>
        /// <exception cref="InvalidValueException">Must throw an exception for illegal binning values</exception>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        short BinX { get; set; }

        /// <summary>
        /// Sets the binning factor for the Y axis, also returns the current value.
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.BinY">Canonical definition</see></remarks>
        /// <value>The Y binning value.</value>
        /// <exception cref="InvalidValueException">Must throw an exception for illegal binning values</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        short BinY { get; set; }

        /// <summary>
        /// Returns the current camera operational state
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.CameraState">Canonical definition</see></remarks>
        /// <value>The state of the camera.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        CameraState CameraState { get; }

        /// <summary>
        /// Returns the width of the CCD camera chip in unbinned pixels.
        /// </summary>
        /// <value>The size of the camera X.</value>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        int CameraXSize { get; }

        /// <summary>
        /// Returns the height of the CCD camera chip in unbinned pixels.
        /// </summary>
        /// <value>The size of the camera Y.</value>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        int CameraYSize { get; }

        /// <summary>
        /// Returns <c>true</c> if the camera can abort exposures; <c>false</c> if not.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can abort exposure; otherwise, <c>false</c>.
        /// </value>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.CanAbortExposure">Canonical definition</see></remarks>
        bool CanAbortExposure { get; }

        /// <summary>
        /// Returns a flag showing whether this camera supports asymmetric binning
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.CanAsymmetricBin">Canonical definition</see></remarks>
        /// <value>
        /// <c>true</c> if this instance can asymmetric bin; otherwise, <c>false</c>.
        /// </value>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        bool CanAsymmetricBin { get; }

        /// <summary>
        /// If <c>true</c>, the camera's cooler power setting can be read.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can get cooler power; otherwise, <c>false</c>.
        /// </value>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.CanGetCoolerPower">Canonical definition</see></remarks>
        bool CanGetCoolerPower { get; }

        /// <summary>
        /// Returns a flag indicating whether this camera supports pulse guiding
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.CanPulseGuide">Canonical definition</see></remarks>
        /// <value>
        /// <c>true</c> if this instance can pulse guide; otherwise, <c>false</c>.
        /// </value>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        bool CanPulseGuide { get; }

        /// <summary>
        /// Returns a flag indicating whether this camera supports setting the CCD temperature
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.CanSetCCDTemperature">Canonical definition</see></remarks>
        /// <value>
        /// <c>true</c> if this instance can set CCD temperature; otherwise, <c>false</c>.
        /// </value>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        bool CanSetCCDTemperature { get; }

        /// <summary>
        /// Returns a flag indicating whether this camera can stop an exposure that is in progress
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.CanStopExposure">Canonical definition</see></remarks>
        /// <value>
        /// <c>true</c> if the camera can stop the exposure; otherwise, <c>false</c>.
        /// </value>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        bool CanStopExposure { get; }

        /// <summary>
        /// Returns the current CCD temperature in degrees Celsius.
        /// </summary>
        /// <value>The CCD temperature.</value>
        /// <exception cref="InvalidValueException">Must throw exception if data unavailable.</exception>
        /// <exception cref="NotImplementedException">Must throw exception if not supported.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        double CCDTemperature { get; }

        /// <summary>
        /// Turns on and off the camera cooler, and returns the current on/off state.
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.CoolerOn">Canonical definition</see></remarks>
        /// <value><c>true</c> if the cooler is on; otherwise, <c>false</c>.</value>
        /// <exception cref=" NotImplementedException">not supported</exception>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        bool CoolerOn { get; set; }

        /// <summary>
        /// Returns the present cooler power level, in percent.
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.CoolerPower">Canonical definition</see></remarks>
        /// <value>The cooler power.</value>
        /// <exception cref=" NotImplementedException">not supported</exception>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        double CoolerPower { get; }

        /// <summary>
        /// Returns the gain of the camera in photoelectrons per A/D unit.
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.ElectronsPerADU">Canonical definition</see></remarks>
        /// <value>The electrons per ADU.</value>
        /// <exception cref="PropertyNotImplementedException">Not supported</exception>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        double ElectronsPerADU { get; }

        /// <summary>
        /// Reports the full well capacity of the camera in electrons, at the current camera settings (binning, SetupDialog settings, etc.)
        /// </summary>
        /// <value>The full well capacity.</value>
        /// <exception cref="PropertyNotImplementedException">Not supported</exception>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        double FullWellCapacity { get; }

        /// <summary>
        /// Returns a flag indicating whether this camera has a mechanical shutter
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.HasShutter">Canonical definition</see></remarks>
        /// <value>
        /// <c>true</c> if this instance has shutter; otherwise, <c>false</c>.
        /// </value>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        bool HasShutter { get; }

        /// <summary>
        /// Returns the current heat sink temperature (called "ambient temperature" by some manufacturers) in degrees Celsius.
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.HeatSinkTemperature">Canonical definition</see></remarks>
        /// <value>The heat sink temperature.</value>
        /// <exception cref="PropertyNotImplementedException">Not supported</exception>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        double HeatSinkTemperature { get; }

        /// <summary>
        /// Returns a safearray of int of size <see cref="NumX" /> * <see cref="NumY" /> containing the pixel values from the last exposure.
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.ImageArray">Canonical definition</see></remarks>
        /// <value>The image array.</value>
        /// <exception cref="InvalidOperationException">If no image data is available.</exception>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        object ImageArray { get; }

        /// <summary>
        /// Returns a safearray of Variant of size <see cref="NumX" /> * <see cref="NumY" /> containing the pixel values from the last exposure.
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.ImageArrayVariant">Canonical definition</see></remarks>
        /// <value>The image array variant.</value>
        /// <exception cref="PropertyNotImplementedException">Not supported</exception>
        /// <exception cref="InvalidOperationException">If no image data is available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        object ImageArrayVariant { get; }

        /// <summary>
        /// Returns a flag indicating whether the image is ready to be downloaded from the camera
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.ImageReady">Canonical definition</see></remarks>
        /// <value><c>true</c> if [image ready]; otherwise, <c>false</c>.</value>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        bool ImageReady { get; }

        /// <summary>
        /// Returns a flag indicating whether the camera is currently in a <see cref="PulseGuide">PulseGuide</see> operation.
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.IsPulseGuiding">Canonical definition</see></remarks>
        /// <value>
        /// <c>true</c> if this instance is pulse guiding; otherwise, <c>false</c>.
        /// </value>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        bool IsPulseGuiding { get; }

        /// <summary>
        /// Reports the actual exposure duration in seconds (i.e. shutter open time).
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.LastExposureDuration">Canonical definition</see></remarks>
        /// <value>The last duration of the exposure.</value>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="InvalidOperationException">If called before any exposure has been taken</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        double LastExposureDuration { get; }

        /// <summary>
        /// Reports the actual exposure start in the FITS-standard CCYY-MM-DDThh:mm:ss[.sss...] format.
        /// The start time must be UTC.
        /// </summary>
        /// <value>The last exposure start time in UTC.</value>
        /// <exception cref="NotImplementedException">If the property is not implemented</exception>
        /// <exception cref="InvalidOperationException">If called before any exposure has been taken</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        string LastExposureStartTime { get; }

        /// <summary>
        /// Reports the maximum ADU value the camera can produce.
        /// </summary>
        /// <value>The maximum ADU.</value>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        int MaxADU { get; }

        /// <summary>
        /// Returns the maximum allowed binning for the X camera axis
        /// </summary>
        /// <remarks>
        /// If <see cref="CanAsymmetricBin" /> = <c>false</c>, returns the maximum allowed binning factor. If
        /// <see cref="CanAsymmetricBin" /> = <c>true</c>, returns the maximum allowed binning factor for the X axis.
        /// </remarks>
        /// <value>The max bin X.</value>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        short MaxBinX { get; }

        /// <summary>
        /// Returns the maximum allowed binning for the Y camera axis
        /// </summary>
        /// <remarks>
        /// If <see cref="CanAsymmetricBin" /> = <c>false</c>, equals <see cref="MaxBinX" />. If <see cref="CanAsymmetricBin" /> = <c>true</c>,
        /// returns the maximum allowed binning factor for the Y axis.
        /// </remarks>
        /// <value>The max bin Y.</value>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        short MaxBinY { get; }

        /// <summary>
        /// Sets the subframe width. Also returns the current value.
        /// </summary>
        /// <remarks>
        /// If binning is active, value is in binned pixels.  No error check is performed when the value is set.
        /// Should default to <see cref="CameraXSize" />.
        /// </remarks>
        /// <value>The num X.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        int NumX { get; set; }

        /// <summary>
        /// Sets the subframe height. Also returns the current value.
        /// </summary>
        /// <remarks>
        /// If binning is active,
        /// value is in binned pixels.  No error check is performed when the value is set.
        /// Should default to <see cref="CameraYSize" />.
        /// </remarks>
        /// <value>The num Y.</value>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        int NumY { get; set; }

        /// <summary>
        /// Returns the width of the CCD chip pixels in microns.
        /// </summary>
        /// <value>The pixel size X.</value>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        double PixelSizeX { get; }

        /// <summary>
        /// Returns the height of the CCD chip pixels in microns.
        /// </summary>
        /// <value>The pixel size Y.</value>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        double PixelSizeY { get; }

        /// <summary>
        /// Activates the Camera's mount control system to instruct the mount to move in a particular direction for a given period of time
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.PulseGuide">Canonical definition</see></remarks>
        /// <param name="Direction">The direction of movement.</param>
        /// <param name="Duration">The duration of movement in milli-seconds.</param>
        /// <exception cref="NotImplementedException">PulseGuide command is unsupported</exception>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        void PulseGuide(GuideDirection Direction, int Duration);

        /// <summary>
        /// Sets the camera cooler setpoint in degrees Celsius, and returns the current setpoint.
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.SetCCDTemperature">Canonical definition</see></remarks>
        /// <value>The set CCD temperature.</value>
        /// <exception cref="InvalidValueException">Must throw an InvalidValueException if an attempt is made to set a value is outside the 
        /// camera's valid temperature setpoint range.</exception>
        /// <exception cref="NotImplementedException">Must throw exception if <see cref="CanSetCCDTemperature" /> is <c>false</c>.</exception>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        double SetCCDTemperature { get; set; }

        /// <summary>
        /// Starts an exposure. Use <see cref="ImageReady" /> to check when the exposure is complete.
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.StartExposure">Canonical definition</see></remarks>
        /// <param name="Duration">Duration of exposure in seconds, can be zero if <see cref="StartExposure">Light</see> is <c>false</c></param>
        /// <param name="Light"><c>true</c> for light frame, <c>false</c> for dark frame (ignored if no shutter)</param>
        /// <exception cref=" InvalidValueException"><see cref="NumX" />, <see cref="NumY" />, <see cref="BinX" />,
        /// <see cref="BinY" />, <see cref="StartX" />, <see cref="StartY" />, or <see cref="StartExposure">Duration</see> parameters are invalid.</exception>
        /// <exception cref=" InvalidOperationException"><see cref="CanAsymmetricBin" /> is <c>false</c> and <see cref="BinX" /> != <see cref="BinY" /></exception>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        void StartExposure(double Duration, bool Light);

        /// <summary>
        /// Sets the subframe start position for the X axis (0 based) and returns the current value.
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.StartX">Canonical definition</see></remarks>
        /// <value>The start X.</value>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        int StartX { get; set; }

        /// <summary>
        /// Sets the subframe start position for the Y axis (0 based). Also returns the current value.
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.StartY">Canonical definition</see></remarks>
        /// <value>The start Y.</value>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        int StartY { get; set; }

        /// <summary>
        /// Stops the current exposure, if any.
        /// </summary>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.StopExposure">Canonical definition</see></remarks>
        /// <exception cref="NotImplementedException">Must throw an exception if CanStopExposure is <c>false</c></exception>
		/// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
		/// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        void StopExposure();

        #endregion

        #region ICameraV2 members

        /// <summary>
        /// Returns the X offset of the Bayer matrix, as defined in <see cref="SensorType" />.
        /// </summary>
        /// <returns>The Bayer colour matrix X offset, as defined in <see cref="SensorType" />.</returns>
        /// <exception cref="NotImplementedException">Monochrome cameras must throw this exception, colour cameras must not.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.BayerOffsetX">Canonical definition</see></remarks>
        short BayerOffsetX { get; }

        /// <summary>
        /// Returns the Y offset of the Bayer matrix, as defined in <see cref="SensorType" />.
        /// </summary>
        /// <returns>The Bayer colour matrix Y offset, as defined in <see cref="SensorType" />.</returns>
        /// <exception cref="NotImplementedException">Monochrome cameras must throw this exception, colour cameras must not.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.BayerOffsetY">Canonical definition</see></remarks>
        short BayerOffsetY { get; }

        /// <summary>
        /// Camera has a fast readout mode
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <returns><c>true</c> when the camera supports a fast readout mode</returns>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.CanFastReadout">Canonical definition</see></remarks>
        bool CanFastReadout { get; }

        /// <summary>
        /// Returns the maximum exposure time supported by <see cref="StartExposure">StartExposure</see>.
        /// </summary>
        /// <returns>The maximum exposure time, in seconds, that the camera supports</returns>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.ExposureMax">Canonical definition</see></remarks>
        double ExposureMax { get; }

        /// <summary>
        /// Minimum exposure time
        /// </summary>
        /// <returns>The minimum exposure time, in seconds, that the camera supports through <see cref="StartExposure">StartExposure</see></returns>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.ExposureMin">Canonical definition</see></remarks>
        double ExposureMin { get; }

        /// <summary>
        /// Exposure resolution
        /// </summary>
        /// <returns>The smallest increment in exposure time supported by <see cref="StartExposure">StartExposure</see>.</returns>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.ExposureResolution">Canonical definition</see></remarks>
        double ExposureResolution { get; }

        /// <summary>
        /// Gets or sets Fast Readout Mode
        /// </summary>
        /// <value><c>true</c> for fast readout mode, <c>false</c> for normal mode</value>
        /// <exception cref="NotImplementedException">Thrown if <see cref="CanFastReadout" /> is <c>false</c>.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.FastReadout">Canonical definition</see></remarks>
        bool FastReadout { get; set; }

        /// <summary>
        /// The camera's gain (GAIN VALUE MODE) OR the index of the selected camera gain description in the <see cref="Gains" /> array (GAINS INDEX MODE)
        /// </summary>
        /// <returns><para><b> GAIN VALUE MODE:</b> The current gain value.</para>
        /// <p style="color:red"><b>OR</b></p>
        /// <b>GAINS INDEX MODE:</b> Index into the Gains array for the current camera gain
        /// </returns>
        /// <exception cref="NotImplementedException">When neither <b>GAINS INDEX</b> mode nor <b>GAIN VALUE</b> mode are supported.</exception>
        /// <exception cref="InvalidValueException">When the supplied value is not valid.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.Gain">Canonical definition</see></remarks>
        short Gain { get; set; }

        /// <summary>
        /// Maximum <see cref="Gain" /> value of that this camera supports
        /// </summary>
        /// <returns>The maximum gain value that this camera supports</returns>
        /// <exception cref="NotImplementedException">When the <see cref="Gain"/> property is not implemented or is operating in <b>GAINS INDEX</b> mode.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.GainMax">Canonical definition</see></remarks>
        short GainMax { get; }

        /// <summary>
        /// Minimum <see cref="Gain" /> value of that this camera supports
        /// </summary>
        /// <returns>The minimum gain value that this camera supports</returns>
        /// <exception cref="NotImplementedException">When the <see cref="Gain"/> property is not implemented or is operating in <b>GAINS INDEX</b> mode.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.GainMin">Canonical definition</see></remarks>
        short GainMin { get; }

        /// <summary>
        /// List of Gain names supported by the camera
        /// </summary>
        /// <returns>The list of supported gain names as an ArrayList of strings</returns>
        /// <exception cref="NotImplementedException">When the <see cref="Gain"/> property is not implemented or is operating in <b>GAIN VALUE</b> mode.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.Gains">Canonical definition</see></remarks>
        IList<string> Gains { get; }

        /// <summary>
        /// Percent completed, Interface Version 2 only
        /// </summary>
        /// <returns>A value between 0 and 100% indicating the completeness of this operation</returns>
        /// <exception cref="InvalidOperationException">Thrown when it is inappropriate to call <see cref="PercentCompleted" /></exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.PercentCompleted">Canonical definition</see></remarks>
        short PercentCompleted { get; }

        /// <summary>
        /// Readout mode, Interface Version 2 only
        /// </summary>
        /// <value></value>
        /// <returns>Short integer index into the <see cref="ReadoutModes">ReadoutModes</see> array of string readout mode names indicating
        /// the camera's current readout mode.</returns>
        /// <exception cref="InvalidValueException">Must throw an exception if set to an illegal or unavailable mode.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.ReadoutMode">Canonical definition</see></remarks>
        short ReadoutMode { get; set; }

        /// <summary>
        /// List of available readout modes, Interface Version 2 only
        /// </summary>
        /// <returns>An ArrayList of readout mode names</returns>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.ReadoutModes">Canonical definition</see></remarks>
        IList<string> ReadoutModes { get; }

        /// <summary>
        /// Sensor name, Interface Version 2 only
        /// ## Mandatory must return an empty string if the sensor is unknown
        /// </summary>
        /// <returns>The name of the sensor used within the camera.</returns>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.SensorName">Canonical definition</see></remarks>
        string SensorName { get; }

        /// <summary>
        /// Type of colour information returned by the camera sensor, Interface Version 2 only
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="SensorType" /> enum value of the camera sensor</returns>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.SensorType">Canonical definition</see></remarks>
        SensorType SensorType { get; }

        #endregion

        #region ICameraV3 members

        /// <summary>
        /// The camera's offset (OFFSET VALUE MODE) OR the index of the selected camera offset description in the <see cref="Offsets" /> array (OFFSETS INDEX MODE)
        /// </summary>
        /// <returns><para><b> OFFSET VALUE MODE:</b> The current offset value.</para>
        /// <p style="color:red"><b>OR</b></p>
        /// <b>OFFSETS INDEX MODE:</b> Index into the Offsets array for the current camera offset
        /// </returns>
        /// <exception cref="NotImplementedException">When neither <b>OFFSETS INDEX</b> mode nor <b>OFFSET VALUE</b> mode are supported.</exception>
        /// <exception cref="InvalidValueException">When the supplied value is not valid.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.Offset">Canonical definition</see></remarks>
        int Offset { get; set; }

        /// <summary>
        /// Maximum <see cref="Offset" /> value of that this camera supports
        /// </summary>
        /// <returns>The maximum offset value that this camera supports</returns>
        /// <exception cref="NotImplementedException">When the <see cref="Offset"/> property is not implemented or is operating in <b>OFFSETS INDEX</b> mode.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.OffsetMax">Canonical definition</see></remarks>
        int OffsetMax { get; }

        /// <summary>
        /// Minimum <see cref="Offset" /> value of that this camera supports
        /// </summary>
        /// <returns>The minimum offset value that this camera supports</returns>
        /// <exception cref="NotImplementedException">When the <see cref="Offset"/> property is not implemented or is operating in <b>OFFSETS INDEX</b> mode.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.OffsetMin">Canonical definition</see></remarks>
        int OffsetMin { get; }

        /// <summary>
        /// List of Offset names supported by the camera
        /// </summary>
        /// <returns>The list of supported offset names as an ArrayList of strings</returns>
        /// <exception cref="NotImplementedException">When the <see cref="Offset"/> property is not implemented or is operating in <b>OFFSET VALUE</b> mode.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.Offsets">Canonical definition</see></remarks>
        IList<string> Offsets { get; }

        /// <summary>
        /// Camera's sub-exposure interval
        /// </summary>
        /// <exception cref="NotImplementedException">When the camera does not support sub exposure configuration.</exception>
        /// <exception cref="InvalidValueException">When the supplied value is not valid.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>See this link for the canonical definition, which may include further information: <see href="https://ascom-standards.org/newdocs/camera.html#Camera.SubExposureDuration">Canonical definition</see></remarks>
        double SubExposureDuration { get; set; }

        #endregion
    }
}