// -----------------------------------------------------------------------
// <summary>Defines the IVideo Interface</summary>
// -----------------------------------------------------------------------
using System.Collections.Generic;

namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Defines the IVideo Interface
    /// Note: this applies to COM only
    /// </summary>
    public interface IVideo : IAscomDevice
    {
        /// <summary>
        /// The name of the video capture device when such a device is used.
        /// </summary>
        /// <exception cref="NotImplementedException">Must throw an exception if not implemented.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>For analogue video this is usually the video capture card or dongle attached to the computer.
        /// </remarks>
        string VideoCaptureDeviceName { get; }


        /// <summary>
        /// The maximum supported exposure (integration time) in seconds.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw an ASCOM.NotImplementedException.</b></p>
        /// This value is for information purposes only. The exposure cannot be set directly in seconds, use <see cref="IntegrationRate"/> property to change the exposure.
        /// </remarks>
        double ExposureMax { get; }

        /// <summary>
        /// The minimum supported exposure (integration time) in seconds.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw an ASCOM.NotImplementedException.</b></p>
        /// This value is for information purposes only. The exposure cannot be set directly in seconds, use <see cref="IntegrationRate"/> property to change the exposure.
        /// </remarks>
        double ExposureMin { get; }

        /// <summary>
        /// The frame rate at which the camera is running.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw an ASCOM.NotImplementedException.</b></p>
        /// Analogue cameras run in one of the two fixed frame rates - 25fps for PAL video and 29.97fps for NTSC video.
        /// Digital cameras usually can run at a variable frame rate. This value is for information purposes only and cannot be set. The FrameRate has the same value during the entire operation of the device.
        /// Changing the <see cref="IntegrationRate"/> property may change the actual variable frame rate but cannot change the return value of this property.
        /// </remarks>
        VideoCameraFrameRate FrameRate { get; }

        /// <summary>
        /// Returns the list of integration rates supported by the video camera.
        /// </summary>
        /// <value>The list of supported integration rates in seconds.</value>
        /// <exception cref="NotImplementedException">Must throw exception if camera supports only one integration rate (exposure) that cannot be changed.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// Digital and integrating analogue video cameras allow the effective exposure of a frame to be changed. If the camera supports setting the exposure directly i.e. 2.153 sec then the driver must only
        /// return a range of useful supported exposures. For many video cameras the supported exposures (integration rates) increase by a factor of 2 from a base exposure e.g. 1, 2, 4, 8, 16 sec or 0.04, 0.08, 0.16, 0.32, 0.64, 1.28, 2.56, 5.12, 10.24 sec.
        /// If the cameras supports only one exposure that cannot be changed (such as all non integrating PAL or NTSC video cameras) then this property must throw <see cref="NotImplementedException"/>.
        /// </remarks>
        IList<double> SupportedIntegrationRates { get; }

        /// <summary>
        /// Index into the <see cref="SupportedIntegrationRates"/> array for the selected camera integration rate.
        /// </summary>
        /// <value>Integer index for the current camera integration rate in the <see cref="SupportedIntegrationRates"/> string array.</value>
        /// <returns>Index into the SupportedIntegrationRates array for the selected camera integration rate.</returns>
        /// <exception cref="NotImplementedException">Must throw an exception if the camera supports only one integration rate (exposure) that cannot be changed.</exception>
        /// <exception cref="InvalidValueException">Must throw an exception if not valid.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <see cref="IntegrationRate"/> can be used to adjust the integration rate (exposure) of the camera, if supported. A 0-based array of strings - <see cref="SupportedIntegrationRates"/>,
        /// which correspond to different discrete integration rate settings supported by the camera will be returned. <see cref="IntegrationRate"/> must be set to an integer in this range.
        /// <para>The driver must default <see cref="IntegrationRate"/> to a valid value when integration rate is supported by the camera. </para>
        /// </remarks>
        int IntegrationRate { get; set; }

        /// <summary>
        /// Returns an <see cref="IVideoFrame"/> with its <see cref="IVideoFrame.ImageArray"/> property populated.
        /// </summary>
        /// <value>The current video frame.</value>
        /// <exception cref="InvalidOperationException">If called before any video frame has been taken.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw an ASCOM.NotImplementedException.</b></p>
        /// </remarks>
        IVideoFrame LastVideoFrame { get; }

        /// <summary>
        /// Sensor name.
        /// </summary>
        /// <returns>The name of sensor used within the camera.</returns>
        /// <exception cref="NotImplementedException">Must throw exception if not implemented.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>Returns the name (data sheet part number) of the sensor, e.g. ICX285AL.  The format is to be exactly as shown on
        /// manufacturer data sheet, subject to the following rules. All letter shall be upper case.  Spaces shall not be included.
        /// <para>Any extra suffixes that define region codes, package types, temperature range, coatings, grading, color/monochrome,
        /// etc. shall not be included. For color sensors, if a suffix differentiates different Bayer matrix encodings, it shall be
        /// included.</para>
        /// <para>Examples:</para>
        /// <list type="bullet">
        /// <item><description>ICX285AL-F shall be reported as ICX285</description></item>
        /// <item><description>KAF-8300-AXC-CD-AA shall be reported as KAF-8300</description></item>
        /// </list>
        /// <para><b>Note:</b></para>
        /// <para>The most common usage of this property is to select approximate color balance parameters to be applied to
        /// the Bayer matrix of one-shot color sensors.  Application authors should assume that an appropriate IR cut-off filter is
        /// in place for color sensors.</para>
        /// <para>It is recommended that this function be called only after a <see cref="IAscomDevice.Connected">connection</see> is established with
        /// the camera hardware, to ensure that the driver is aware of the capabilities of the specific camera model.</para>
        /// </remarks>
        string SensorName { get; }

        /// <summary>
        /// Type of colour information returned by the camera sensor.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="SensorType"/> enum value of the camera sensor</returns>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw an ASCOM.NotImplementedException.</b></p>
        /// <para><see cref="SensorType"/> returns a value indicating whether the sensor is monochrome, or what Bayer matrix it encodes.
        /// The following values are defined:</para>
        /// <para>
        /// <table style="width:76.24%;" cellspacing="0" width="76.24%">
        /// <col style="width: 11.701%;"></col>
        /// <col style="width: 20.708%;"></col>
        /// <col style="width: 67.591%;"></col>
        /// <tr>
        /// <td colspan="1" rowspan="1" style="width: 11.701%; padding-right: 10px; padding-left: 10px; &#xA; border-left-color: #000000; border-left-style: Solid; &#xA; border-top-color: #000000; border-top-style: Solid; &#xA; border-right-color: #000000; border-right-style: Solid;&#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; &#xA; background-color: #00ffff;" width="11.701%">
        /// <b>Value</b></td>
        /// <td colspan="1" rowspan="1" style="width: 20.708%; padding-right: 10px; padding-left: 10px; &#xA; border-top-color: #000000; border-top-style: Solid; &#xA; border-right-style: Solid; border-right-color: #000000; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; &#xA; background-color: #00ffff;" width="20.708%">
        /// <b>Enumeration</b></td>
        /// <td colspan="1" rowspan="1" style="width: 67.591%; padding-right: 10px; padding-left: 10px; &#xA; border-top-color: #000000; border-top-style: Solid; &#xA; border-right-style: Solid; border-right-color: #000000; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; &#xA; background-color: #00ffff;" width="67.591%">
        /// <b>Meaning</b></td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px; &#xA; border-left-color: #000000; border-left-style: Solid; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 0</td>
        /// <td style="padding-right: 10px; padding-left: 10px; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Monochrome</td>
        /// <td style="padding-right: 10px; padding-left: 10px; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Camera produces monochrome array with no Bayer encoding</td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px; &#xA; border-left-color: #000000; border-left-style: Solid; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 1</td>
        /// <td style="padding-right: 10px; padding-left: 10px; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Colour</td>
        /// <td style="padding-right: 10px; padding-left: 10px; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Camera produces color image directly, requiring not Bayer decoding. The monochrome pixels for the R, G and B channels are returned in this order in the <see cref="IVideoFrame.ImageArray"/>.</td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px; &#xA; border-left-color: #000000; border-left-style: Solid; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 2</td>
        /// <td style="padding-right: 10px; padding-left: 10px; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// RGGB</td>
        /// <td style="padding-right: 10px; padding-left: 10px; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Camera produces RGGB encoded Bayer array images</td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px; &#xA; border-left-color: #000000; border-left-style: Solid; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 3</td>
        /// <td style="padding-right: 10px; padding-left: 10px; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// CMYG</td>
        /// <td style="padding-right: 10px; padding-left: 10px; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Camera produces CMYG encoded Bayer array images</td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px; &#xA; border-left-color: #000000; border-left-style: Solid; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 4</td>
        /// <td style="padding-right: 10px; padding-left: 10px; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// CMYG2</td>
        /// <td style="padding-right: 10px; padding-left: 10px; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Camera produces CMYG2 encoded Bayer array images</td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px; &#xA; border-left-color: #000000; border-left-style: Solid; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 5</td>
        /// <td style="padding-right: 10px; padding-left: 10px; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// LRGB</td>
        /// <td style="padding-right: 10px; padding-left: 10px; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Camera produces Kodak TRUESENSE Bayer LRGB array images</td>
        /// </tr>
        /// </table>
        /// </para>
        /// <para>Please note that additional values may be defined in future updates of the standard, as new Bayer matrices may be created
        /// by sensor manufacturers in the future.  If this occurs, then a new enumeration value shall be defined. The pre-existing enumeration
        /// values shall not change.
        /// <para>In the following definitions, R = red, G = green, B = blue, C = cyan, M = magenta, Y = yellow.  The Bayer matrix is
        /// defined with X increasing from left to right, and Y increasing from top to bottom. The pattern repeats every N x M pixels for the
        /// entire pixel array, where N is the height of the Bayer matrix, and M is the width.</para>
        /// <para>RGGB indicates the following matrix:</para>
        /// </para>
        /// <para>
        /// <table style="width:41.254%;" cellspacing="0" width="41.254%">
        /// <col style="width: 10%;"></col>
        /// <col style="width: 10%;"></col>
        /// <col style="width: 10%;"></col>
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #ffffff" width="10%">
        /// </td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;&#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff;" width="10%">
        /// <b>X = 0</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; background-color: #00ffff;" width="10%">
        /// <b>X = 1</b></td>
        /// </tr>
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff" width="10%">
        /// <b>Y = 0</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
        /// R</td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-top-color: #000000; border-top-style: Solid;  border-top-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; " width="10%">
        /// G</td>
        /// </tr>
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; background-color: #00ffff;" width="10%">
        /// <b>Y = 1</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; " width="10%">
        /// G</td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; " width="10%">
        /// B</td>
        /// </tr>
        /// </table>
        /// </para>
        /// <para>CMYG indicates the following matrix:</para>
        /// <para>
        /// <table style="width:41.254%;" cellspacing="0" width="41.254%">
        /// <col style="width: 10%;"></col>
        /// <col style="width: 10%;"></col>
        /// <col style="width: 10%;"></col>
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #ffffff" width="10%">
        /// </td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;&#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff;" width="10%">
        /// <b>X = 0</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; background-color: #00ffff;" width="10%">
        /// <b>X = 1</b></td>
        /// </tr>
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff" width="10%">
        /// <b>Y = 0</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
        /// Y</td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-top-color: #000000; border-top-style: Solid;  border-top-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; " width="10%">
        /// C</td>
        /// </tr>
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; background-color: #00ffff;" width="10%">
        /// <b>Y = 1</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; " width="10%">
        /// G</td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; " width="10%">
        /// M</td>
        /// </tr>
        /// </table>
        /// </para>
        /// <para>CMYG2 indicates the following matrix:</para>
        /// <para>
        /// <table style="width:41.254%;" cellspacing="0" width="41.254%">
        /// <col style="width: 10%;"></col>
        /// <col style="width: 10%;"></col>
        /// <col style="width: 10%;"></col>
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #ffffff" width="10%">
        /// </td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;&#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff;" width="10%">
        /// <b>X = 0</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; background-color: #00ffff;" width="10%">
        /// <b>X = 1</b></td>
        /// </tr>
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff" width="10%">
        /// <b>Y = 0</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
        /// C</td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-top-color: #000000; border-top-style: Solid;  border-top-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; " width="10%">
        /// Y</td>
        /// </tr>
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff;" width="10%">
        /// <b>Y = 1</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
        /// M</td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; " width="10%">
        /// G</td>
        /// </tr>
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff" width="10%">
        /// <b>Y = 2</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
        /// C</td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-top-color: #000000; border-top-style: Solid;  border-top-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; " width="10%">
        /// Y</td>
        /// </tr>
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; background-color: #00ffff;" width="10%">
        /// <b>Y = 3</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; " width="10%">
        /// G</td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; " width="10%">
        /// M</td>
        /// </tr>
        /// </table>
        /// </para>
        /// <para>LRGB indicates the following matrix (Kodak TRUESENSE):</para>
        /// <para>
        /// <table style="width:68.757%;" cellspacing="0" width="68.757%">
        /// <col style="width: 10%;"></col>
        /// <col style="width: 10%;"></col>
        /// <col style="width: 10%;"></col>
        /// <col style="width: 10%;"></col>
        /// <col style="width: 10%;"></col>
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #ffffff" width="10%">
        /// </td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;&#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff;" width="10%">
        /// <b>X = 0</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff;" width="10%">
        /// <b>X = 1</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff;" width="10%">
        /// <b>X = 2</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; background-color: #00ffff;" width="10%">
        /// <b>X = 3</b></td>
        /// </tr>
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff" width="10%">
        /// <b>Y = 0</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
        /// L</td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
        /// R</td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
        /// L</td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-top-color: #000000; border-top-style: Solid;  border-top-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; " width="10%">
        /// G</td>
        /// </tr>
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff;" width="10%">
        /// <b>Y = 1</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
        /// R</td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
        /// L</td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
        /// G</td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; " width="10%">
        /// L</td>
        /// </tr>
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff" width="10%">
        /// <b>Y = 2</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
        /// L</td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
        /// G</td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
        /// L</td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-top-color: #000000; border-top-style: Solid;  border-top-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; " width="10%">
        /// B</td>
        /// </tr>
        /// <tr valign="top" align="center">
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; background-color: #00ffff;" width="10%">
        /// <b>Y = 3</b></td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; " width="10%">
        /// G</td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; " width="10%">
        /// L</td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; " width="10%">
        /// B</td>
        /// <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; " width="10%">
        /// L</td>
        /// </tr>
        /// </table>
        /// </para>
        /// <para>It is recommended that this function be called only after a <see cref="IAscomDevice.Connected">connection</see> is established with the camera hardware, to ensure that
        /// the driver is aware of the capabilities of the specific camera model.</para>
        /// </remarks>
        SensorType SensorType { get; }

        /// <summary>
        /// Returns the width of the video frame in pixels.
        /// </summary>
        /// <value>The video frame width.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw an ASCOM.NotImplementedException.</b></p>
        /// For analogue video cameras working via a frame grabber the dimensions of the video frames may be different than the dimension of the CCD chip
        /// </remarks>
        int Width { get; }

        /// <summary>
        /// Returns the height of the video frame in pixels.
        /// </summary>
        /// <value>The video frame height.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw an ASCOM.NotImplementedException.</b></p>
        /// For analogue video cameras working via a frame grabber the dimensions of the video frames may be different than the dimension of the CCD chip
        /// </remarks>
        int Height { get; }

        /// <summary>
        /// Returns the width of the CCD chip pixels in microns.
        /// </summary>
        /// <value>The pixel size X if known.</value>
        /// <exception cref="NotImplementedException">Must throw exception if not implemented.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        double PixelSizeX { get; }

        /// <summary>
        /// Returns the height of the CCD chip pixels in microns.
        /// </summary>
        /// <value>The pixel size Y if known.</value>
        /// <exception cref="NotImplementedException">Must throw exception if not implemented.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        double PixelSizeY { get; }

        /// <summary>
        /// Reports the bit depth the camera can produce.
        /// </summary>
        /// <value>The bit depth per pixel. Typical analogue videos are 8-bit while some digital cameras can provide 12, 14 or 16-bit images.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw an ASCOM.NotImplementedException.</b></p>
        /// </remarks>
        int BitDepth { get; }

        /// <summary>
        /// Returns the video codec used to record the video file.
        /// </summary>
        /// <exception cref="NotImplementedException">Must throw exception if not implemented.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>For AVI files this is usually the FourCC identifier of the codec- e.g. XVID, DVSD, YUY2, HFYU etc.
        /// If the recorded video file doesn't use codecs an empty string must be returned.
        /// </remarks>
        string VideoCodec { get; }

        /// <summary>
        /// Returns the file format of the recorded video file, e.g. AVI, MPEG, ADV etc.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw an ASCOM.NotImplementedException.</b></p>
        /// </remarks>
        string VideoFileFormat { get; }

        /// <summary>
        /// The size of the video frame buffer.
        /// </summary>
        /// <value>The size of the video frame buffer. </value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks><p style="color:red"><b>Must be implemented, must not throw an ASCOM.NotImplementedException.</b></p> When retrieving video frames using the <see cref="LastVideoFrame" /> property
        /// the driver may use a buffer to queue the frames waiting to be read by the client. This property returns the size of the buffer in frames or
        /// if no buffering is supported then the value of less than 2 should be returned. The size of the buffer can be controlled by the end user from the driver setup dialog.
        /// </remarks>
        int VideoFramesBufferSize { get; }

        /// <summary>
        /// Starts recording a new video file.
        /// </summary>
        /// <param name="PreferredFileName">The file name requested by the client. Some systems may not allow the file name to be controlled directly and they should ignore this parameter.</param>
        /// <returns>The actual file name of the file that is being recorded.</returns>
        /// <exception cref="NotImplementedException">Must throw exception if not implemented.</exception>
        /// <exception cref="InvalidOperationException">Must throw exception if the current camera state doesn't allow to begin recording a file.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        string StartRecordingVideoFile(string PreferredFileName);

        /// <summary>
        /// Stops the recording of a video file.
        /// </summary>
        /// <exception cref="NotImplementedException">Must throw exception if not implemented.</exception>
        /// <exception cref="InvalidOperationException">Must throw exception if the current camera state doesn't allow to stop recording the file or no file is currently being recorded.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        void StopRecordingVideoFile();

        /// <summary>
        /// Returns the current camera operational state.
        /// </summary>
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw an ASCOM.NotImplementedException.</b></p>
        /// Returns one of the following status information:
        /// <list type="bullet">
        /// <listheader><description>Value  State           Meaning</description></listheader>
        /// <item><description>0      CameraRunning	  The camera is running and video frames are available for viewing and recording</description></item>
        /// <item><description>1      CameraRecording The camera is running and recording a video</description></item>
        /// <item><description>2      CameraError     Camera error condition serious enough to prevent further operations (connection fail, etc.).</description></item>
        /// </list>
        /// <para>CameraIdle and CameraBusy are optional states. Free running cameras cannot be stopped and don't have a CameraIdle state. When those cameras are powered they immediately enter CameraRunning state.
        /// Some digital cameras or video systems may support operations that take longer to complete. While those longer operations are running the camera will remain in the state it was before the operation started.</para>
        /// <para>The video camera state diagram is shown below: </para>
        /// <para>
        /// <img src="../media/VideoCamera State Diagram.png"/></para>
        /// </remarks>
        /// <value>The state of the camera.</value>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        VideoCameraState CameraState { get; }

        /// <summary>
        /// Maximum value of <see cref="Gain"/>.
        /// </summary>
        /// <value>Short integer representing the maximum gain value supported by the camera.</value>
        /// <returns>The maximum gain value that this camera supports</returns>
        /// <exception cref="NotImplementedException">Must throw an exception if GainMax is not supported.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>When specifying the gain setting with an integer value, <see cref="GainMax"/> is used in conjunction with <see cref="GainMin"/> to
        /// specify the range of valid settings.
        /// <para><see cref="GainMax"/> shall be greater than <see cref="GainMin"/>. If either is available, then both must be available.</para>
        /// <para>Please see <see cref="Gain"/> for more information.</para>
        /// <para>It is recommended that this function be called only after a <see cref="IAscomDevice.Connected">connection</see> is established with the camera hardware, to ensure
        /// that the driver is aware of the capabilities of the specific camera model.</para>
        /// </remarks>
        short GainMax { get; }

        /// <summary>
        /// Minimum value of <see cref="Gain"/>.
        /// </summary>
        /// <returns>The minimum gain value that this camera supports</returns>
        /// <exception cref="NotImplementedException">Must throw an exception if GainMin is not supported.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>When specifying the gain setting with an integer value, <see cref="GainMin"/> is used in conjunction with <see cref="GainMax"/> to
        /// specify the range of valid settings.
        /// <para><see cref="GainMax"/> shall be greater than <see cref="GainMin"/>. If either is available, then both must be available.</para>
        /// <para>Please see <see cref="Gain"/> for more information.</para>
        /// <para>It is recommended that this function be called only after a <see cref="IAscomDevice.Connected">connection</see> is established with the camera hardware, to ensure
        /// that the driver is aware of the capabilities of the specific camera model.</para>
        /// </remarks>
        short GainMin { get; }

        /// <summary>
        /// Index into the <see cref="Gains"/> array for the selected camera gain.
        /// </summary>
        /// <value>Short integer index for the current camera gain in the <see cref="Gains"/> string array.</value>
        /// <returns>Index into the Gains array for the selected camera gain</returns>
        /// <exception cref="NotImplementedException">Must throw an exception if gain is not supported.</exception>
        /// <exception cref="InvalidValueException">Must throw an exception if not valid.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <see cref="Gain"/> can be used to adjust the gain setting of the camera, if supported. There are two typical usage scenarios:
        /// <ul>
        /// <li>Discrete gain video cameras will return a 0-based array of strings - <see cref="Gains"/>, which correspond to different discrete gain settings supported by the camera. <see cref="Gain"/> must be set to an integer in this range. <see cref="GainMin"/> and <see cref="GainMax"/> must thrown an exception if
        /// this mode is used.</li>
        /// <li>Adjustable gain video cameras - <see cref="GainMin"/> and <see cref="GainMax"/> return integers, which specify the valid range for <see cref="Gain"/>.</li>
        /// </ul>
        /// <para>The driver must default <see cref="Gain"/> to a valid value. </para>
        /// </remarks>
        short Gain { get; set; }

        /// <summary>
        /// Gains supported by the camera.
        /// </summary>
        /// <returns>An ArrayList of gain names or values</returns>
        /// <exception cref="NotImplementedException">Must throw an exception if Gains is not supported</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks><see cref="Gains"/> provides a 0-based array of available gain settings.
        /// Typically the application software will display the available gain settings in a drop list. The application will then supply
        /// the selected index to the driver via the <see cref="Gain"/> property.
        /// <para>The <see cref="Gain"/> setting may alternatively be specified using integer values; if this mode is used then <see cref="Gains"/> is invalid
        /// and must throw an exception. Please see <see cref="GainMax"/> and <see cref="GainMin"/> for more information.</para>
        /// <para>It is recommended that this function be called only after a <see cref="IAscomDevice.Connected">connection</see> is established with the camera hardware,
        /// to ensure that the driver is aware of the capabilities of the specific camera model.</para>
        /// </remarks>
        IList<string> Gains { get; }

        /// <summary>
        /// Maximum value of <see cref="Gamma"/>.
        /// </summary>
        /// <value>Short integer representing the maximum gamma value supported by the camera.</value>
        /// <returns>The maximum gain value that this camera supports</returns>
        /// <exception cref="NotImplementedException">Must throw an exception if GammaMax is not supported</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>When specifying the gamma setting with an integer value, <see cref="GammaMax"/> is used in conjunction with <see cref="GammaMin"/> to
        /// specify the range of valid settings.
        /// <para><see cref="GammaMax"/> shall be greater than <see cref="GammaMin"/>. If either is available, then both must be available.</para>
        /// <para>Please see <see cref="Gamma"/> for more information.</para>
        /// <para>It is recommended that this function be called only after a <see cref="IAscomDevice.Connected">connection</see> is established with the camera hardware, to ensure
        /// that the driver is aware of the capabilities of the specific camera model.</para>
        /// </remarks>
        short GammaMax { get; }

        /// <summary>
        /// Minimum value of <see cref="Gamma"/>.
        /// </summary>
        /// <returns>The minimum gamma value that this camera supports</returns>
        /// <exception cref="NotImplementedException">Must throw an exception if GammaMin is not supported.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>When specifying the gamma setting with an integer value, <see cref="GammaMin"/> is used in conjunction with <see cref="GammaMax"/> to
        /// specify the range of valid settings.
        /// <para><see cref="GammaMax"/> shall be greater than <see cref="GammaMin"/>. If either is available, then both must be available.</para>
        /// <para>Please see <see cref="Gamma"/> for more information.</para>
        /// <para>It is recommended that this function be called only after a <see cref="IAscomDevice.Connected">connection</see> is established with the camera hardware, to ensure
        /// that the driver is aware of the capabilities of the specific camera model.</para>
        /// </remarks>
        short GammaMin { get; }

        /// <summary>
        /// Index into the <see cref="Gammas"/> array for the selected camera gamma.
        /// </summary>
        /// <value>Short integer index for the current camera gamma in the <see cref="Gammas"/> string array.</value>
        /// <returns>Index into the Gammas array for the selected camera gamma</returns>
        /// <exception cref="NotImplementedException">Must throw an exception if Gamma is not supported.</exception>
        /// <exception cref="InvalidValueException">Must throw an exception if not valid.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <see cref="Gamma"/> can be used to adjust the gamma setting of the camera, if supported. There are two typical usage scenarios:
        /// <ul>
        /// <li>Discrete gamma video cameras will return a 0-based array of strings - <see cref="Gammas"/>, which correspond to different discrete gamma settings supported by the camera. <see cref="Gamma"/> must be set to an integer in this range. <see cref="GammaMin"/> and <see cref="GammaMax"/> must thrown an exception if
        /// this mode is used.</li>
        /// <li>Adjustable gain video cameras - <see cref="GammaMin"/> and <see cref="GammaMax"/> return integers, which specify the valid range for <see cref="Gamma"/>.</li>
        /// </ul>
        /// <para>The driver must default <see cref="Gamma"/> to a valid value. </para>
        /// </remarks>
        short Gamma { get; set; }

        /// <summary>
        /// Gammas supported by the camera.
        /// </summary>
        /// <returns>An ArrayList of gamma names or values</returns>
        /// <exception cref="NotImplementedException">Must throw an exception if Gammas is not supported</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks><see cref="Gammas"/> provides a 0-based array of available gamma settings. This list can contain the widely used values of <b>OFF</b>, <b>LO</b> and <b>HI</b> that correspond to gammas of <b>1.00</b>, <b>0.45</b> and <b>0.35</b> as well as other extended values.
        /// Typically the application software will display the available gamma settings in a drop list. The application will then supply
        /// the selected index to the driver via the <see cref="Gamma"/> property.
        /// <para>The <see cref="Gamma"/> setting may alternatively be specified using integer values; if this mode is used then <see cref="Gammas"/> is invalid
        /// and must throw an exception. Please see <see cref="GammaMax"/> and <see cref="GammaMin"/> for more information.</para>
        /// <para>It is recommended that this function be called only after a <see cref="IAscomDevice.Connected">connection</see> is established with the camera hardware,
        /// to ensure that the driver is aware of the capabilities of the specific camera model.</para>
        /// </remarks>
        IList<string> Gammas { get; }

        /// <summary>
        /// Returns True if the driver supports custom device properties configuration via the <see cref="ConfigureDeviceProperties"/> method.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks><p style="color:red"><b>Must be implemented, must not throw an ASCOM.NotImplementedException.</b></p>
        /// </remarks>
        bool CanConfigureDeviceProperties { get; }

        /// <summary>
        /// Displays a device properties configuration dialog that allows the configuration of specialized settings.
        /// </summary>
        /// <exception cref="NotImplementedException">Must throw an exception if not implemented.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <para>The dialog could also provide buttons for cameras that can be controlled via 'on screen display' menus and a set of navigation buttons such as Up, Down, Left, Right and Enter.
        /// This dialog is not intended to be used in unattended mode but can give greater control over video cameras that provide special features. The dialog may also allow
        /// changing standard <see cref="IVideo"/> interface settings such as Gamma and Gain. If a client software
        /// displays any <see cref="IVideo"/> interface settings then it should take care to keep in sync the values changed by this method and those changed directly via the interface.</para>
        /// <para>To support automated and unattended control over the specialized device settings or functions available on this dialog the driver should also allow their control via <see cref="IAscomDevice.SupportedActions"/>.
        /// This dialog is meant to be used by the applications to allow the user to adjust specialized device settings when those applications don't specifically use the specialized settings in their functionality.</para>
        /// <para>Examples for specialized settings that could be supported are white balance and sharpness.</para>
        /// </remarks>
        void ConfigureDeviceProperties();
    }
}