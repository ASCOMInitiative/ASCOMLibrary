﻿namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Defines the IObservingConditions Interface.
    /// This interface provides a limited set of values that are useful
    /// for astronomical purposes for things such as determining if it is safe to open or operate the observing system,
    /// for recording astronomical data or determining refraction corrections.
    /// </summary>
    /// <remarks>It is NOT intended as a general purpose environmental sensor system. The <see cref="IAscomDevice.Action">Action</see> method and 
    /// <see cref="IAscomDevice.SupportedActions">SupportedActions</see> property can be used to extend your driver to present any further sensors that you need.
    /// </remarks>
    public interface IObservingConditions : IAscomDevice
    {

        /// <summary>
        /// Gets And sets the time period over which observations will be averaged
        /// </summary>
        /// <value>Time period (hours) over which to average sensor readings</value>
        /// <exception cref="InvalidValueException">If the value set is not available for this driver. All drivers must accept 0.0 to specify that an instantaneous value is available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Mandatory property, must be implemented, can NOT throw a NotImplementedException</b></p>
        /// <para>This property should return the time period (hours) over which sensor readings will be averaged. If your driver is delivering instantaneous sensor readings this property should return a value of 0.0.</para>
        /// <para>Please resist the temptation to throw exceptions when clients query sensor properties when insufficient time has passed to get a true average reading. 
        /// A best estimate of the average sensor value should be returned in these situations. </para> 
        /// </remarks>
        double AveragePeriod { get; set; }

        /// <summary>
        /// Amount of sky obscured by cloud
        /// </summary>
        /// <value>percentage of the sky covered by cloud</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException</b></p>
        /// This property should return a value between 0.0 and 100.0 where 0.0 = clear sky and 100.0 = 100% cloud coverage
        /// </remarks>
        double CloudCover { get; }

        /// <summary>
        /// Atmospheric dew point at the observatory
        /// </summary>
        /// <value>Atmospheric dew point reported in °C.</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException when the <see cref="Humidity"/> property also throws a NotImplementedException.</b></p>
        /// <p style="color:red"><b>Mandatory property, must NOT throw a NotImplementedException when the <see cref="Humidity"/> property is implemented.</b></p>
        /// <para>The units of this property are degrees Celsius. Driver and application authors can use the ASCOM.Tools.Utilities.ConvertUnits method to convert these units to and from degrees Fahrenheit.</para>
        /// <para>The ASCOM specification requires that DewPoint and Humidity are either both implemented or both throw NotImplementedExceptions. It is not allowed for 
        /// one to be implemented and the other to throw a NotImplementedException. The ASCOM.Tools.Utilities component contains methods DewPoint2Humidity and 
        /// Humidity2DewPoint to convert DewPoint to Humidity and vice versa given the ambient temperature.</para>
        /// </remarks>
        double DewPoint { get; }

        /// <summary>
        /// Atmospheric humidity at the observatory
        /// </summary>
        /// <value>Atmospheric humidity (%)</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException when the <see cref="DewPoint"/> property also throws a NotImplementedException.</b></p>
        /// <p style="color:red"><b>Mandatory property, must NOT throw a NotImplementedException when the <see cref="DewPoint"/> property is implemented.</b></p>
        /// <para>The ASCOM specification requires that DewPoint and Humidity are either both implemented or both throw NotImplementedExceptions. It is not allowed for 
        /// one to be implemented and the other to throw a NotImplementedException. The ASCOM.Tools.Utilities component contains methods DewPoint2Humidity and 
        /// Humidity2DewPoint to convert DewPoint to Humidity and vice versa given the ambient temperature.</para>
        /// <para>This property should return a value between 0.0 and 100.0 where 0.0 = 0% relative humidity and 100.0 = 100% relative humidity.</para>
        /// </remarks>
        double Humidity { get; }

        /// <summary>
        /// Atmospheric pressure at the observatory
        /// </summary>
        /// <value>Atmospheric pressure at the observatory (hPa)</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException</b></p>
        /// <para>The units of this property are hectoPascals. Client and driver authors can use the method ASCOM.Tools.Utilities.ConvertUnits
        /// to convert these units to and from milliBar, mm of mercury and inches of mercury.</para>
        /// <para>This must be the pressure at the observatory altitude and not the adjusted pressure at sea level.
        /// Please check whether your pressure sensor delivers local observatory pressure or sea level pressure and, if it returns sea level pressure, 
        /// adjust this to actual pressure at the observatory's altitude before returning a value to the client.
        /// The ASCOM.Tools.Utilities.ConvertPressure method can be used to effect this adjustment.
        /// </para>
        /// </remarks>
        double Pressure { get; }

        /// <summary>
        /// Rain rate at the observatory
        /// </summary>
        /// <value>Rain rate (mm / hour)</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException</b></p>
        /// <para>The units of this property are millimetres per hour. Client and driver authors can use the methodASCOM.Tools.Utilities.ConvertUnits
        /// to convert these units to and from inches per hour.</para>
        /// <para>This property can be interpreted as 0.0 = Dry any positive non-zero value = wet.</para>
        /// <para>Rainfall intensity is classified according to the rate of precipitation:</para>
        /// <list type="bullet">
        /// <item><description>Light rain — when the precipitation rate is less than 2.5 mm (0.098 in) per hour</description></item>
        /// <item><description>Moderate rain — when the precipitation rate is between 2.5 mm (0.098 in) and 10 mm (0.39 in) per hour</description></item>
        /// <item><description>Heavy rain — when the precipitation rate is between 10 mm (0.39 in) and 50 mm (2.0 in) per hour</description></item>
        /// <item><description>Violent rain — when the precipitation rate is &gt; 50 mm (2.0 in) per hour</description></item>
        /// </list>
        /// </remarks>
        double RainRate { get; }

        /// <summary>
        /// Sky brightness at the observatory
        /// </summary>
        /// <value>Sky brightness (Lux)</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException</b></p>
        /// This property returns the sky brightness measured in Lux.
        /// <para>Luminance Examples in Lux</para>
        /// <list type="table">
        /// <listheader>
        /// <term>Illuminance</term><term>Surfaces illuminated by:</term>
        /// </listheader>
        /// <item><description>0.0001 lux</description><description>Moonless, overcast night sky (starlight)</description></item>
        /// <item><description>0.002 lux</description><description>Moonless clear night sky with airglow</description></item>
        /// <item><description>0.27–1.0 lux</description><description>Full moon on a clear night</description></item>
        /// <item><description>3.4 lux</description><description>Dark limit of civil twilight under a clear sky</description></item>
        /// <item><description>50 lux</description><description>Family living room lights (Australia, 1998)</description></item>
        /// <item><description>80 lux</description><description>Office building hallway/toilet lighting</description></item>
        /// <item><description>100 lux</description><description>Very dark overcast day</description></item>
        /// <item><description>320–500 lux</description><description>Office lighting</description></item>
        /// <item><description>400 lux</description><description>Sunrise or sunset on a clear day.</description></item>
        /// <item><description>1000 lux</description><description>Overcast day; typical TV studio lighting</description></item>
        /// <item><description>10000–25000 lux</description><description>Full daylight (not direct sun)</description></item>
        /// <item><description>32000–100000 lux</description><description>Direct sunlight</description></item>
        /// </list>
        /// </remarks>
        double SkyBrightness { get; }

        /// <summary>
        /// Sky quality at the observatory
        /// </summary>
        /// <value>Sky quality measured in magnitudes per square arc second</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException</b></p>
        /// <para>Sky quality is typically measured in units of magnitudes per square arc second. A sky quality of 20 magnitudes per square arc second means that the
        /// overall sky appears with a brightness equivalent to having 1 magnitude 20 star in each square arc second of sky.</para>
        /// <para >Examples of typical sky quality values were published by Sky and Telescope (<a href="http://www.skyandtelescope.com/astronomy-resources/rate-your-skyglow/">http://www.skyandtelescope.com/astronomy-resources/rate-your-skyglow/</a>) and, in slightly adapted form, are reproduced below:</para>
        /// <para>
        /// <table style="width:80.0%;" cellspacing="0" width="80.0%">
        /// <col style="width: 20.0%;"></col>
        /// <col style="width: 80.0%;"></col>
        /// <tr>
        /// <td colspan="1" rowspan="1" style="width: 20.0%; padding-right: 10px; padding-left: 10px; 
        /// text-align: center;vertical-align: middle;
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-top-color: #000000; border-top-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid;
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; 
        /// background-color: #00ffff;" width="10.0">
        /// <b>Sky Quality (mag/arcsec<sup>2</sup>)</b></td>
        /// <td colspan="1" rowspan="1" style="width: 80.0%; padding-right: 10px; padding-left: 10px; 
        /// text-align: center;vertical-align: middle;
        /// border-top-color: #000000; border-top-style: Solid; 
        /// border-right-style: Solid; border-right-color: #000000; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; 
        /// background-color: #00ffff;" width="90.0">
        /// <b>Description</b></td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// text-align: center;vertical-align: middle;
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 22.0</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// By convention, this is often assumed to be the average brightness of a moonless night sky that's completely free of artificial light pollution.</td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// text-align: center;vertical-align: middle;
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 21.0</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// This is typical for a rural area with a medium-sized city not far away. It's comparable to the glow of the brightest section of the northern Milky Way, from Cygnus through Perseus. </td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// text-align: center;vertical-align: middle;
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 20.0</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// This is typical for the outer suburbs of a major metropolis. The summer Milky Way is readily visible but severely washed out.</td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// text-align: center;vertical-align: middle;
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 19.0</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Typical for a suburb with widely spaced single-family homes. It's a little brighter than a remote rural site at the end of nautical twilight, when the Sun is 12° below the horizon.</td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// text-align: center;vertical-align: middle;
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 18.0</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Bright suburb or dark urban neighborhood. It's also a typical zenith skyglow at a rural site when the Moon is full. The Milky Way is invisible, or nearly so.</td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// text-align: center;vertical-align: middle;
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 17.0</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// Typical near the center of a major city.</td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// text-align: center;vertical-align: middle;
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 13.0</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// The zenith skyglow at the end of civil twilight, roughly a half hour after sunset, when the Sun is 6° below the horizon. Venus and Jupiter are easy to see, but bright stars are just beginning to appear.</td>
        /// </tr>
        /// <tr>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// text-align: center;vertical-align: middle;
        /// border-left-color: #000000; border-left-style: Solid; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// 7.0</td>
        /// <td style="padding-right: 10px; padding-left: 10px; 
        /// border-right-color: #000000; border-right-style: Solid; 
        /// border-bottom-color: #000000; border-bottom-style: Solid; 
        /// border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
        /// The zenith skyglow at sunrise or sunset</td>
        /// </tr>
        /// </table>
        /// </para>
        /// </remarks>
        double SkyQuality { get; }

        /// <summary>
        /// Seeing at the observatory measured as star full width half maximum (FWHM) in arc secs.
        /// </summary>
        /// <value>Seeing reported as star full width half maximum (arc seconds)</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException</b></p>
        /// </remarks>
        double StarFWHM { get; }

        /// <summary>
        /// Sky temperature at the observatory
        /// </summary>
        /// <value>Sky temperature in °C</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException</b></p>
        /// <para>The units of this property are degrees Celsius. Driver and application authors can use the ASCOM.Tools.Utilities.ConvertUnits method
        /// to convert these units to and from degrees Fahrenheit.</para>
        /// <para>This is expected to be returned by an infra-red sensor looking at the sky. The lower the temperature the more the sky is likely to be clear.</para>
        /// </remarks>
        double SkyTemperature { get; }

        /// <summary>
        /// Temperature at the observatory
        /// </summary>
        /// <value>Temperature in °C</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException</b></p>
        /// <para>The units of this property are degrees Celsius. Driver and application authors can use the ASCOM.Tools.Utilities.ConvertUnits method
        /// to convert these units to and from degrees Fahrenheit.</para>
        /// <para>This is expected to be the ambient temperature at the observatory.</para>
        /// </remarks>
        double Temperature { get; }

        /// <summary>
        /// Wind direction at the observatory
        /// </summary>
        /// <value>Wind direction (degrees, 0..360.0)</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException</b></p>
        /// The returned value must be between 0.0 and 360.0, interpreted according to the meteorological standard, where a special value of 0.0 is returned when the wind speed is 0.0. 
        /// Wind direction is measured clockwise from north, through east, where East=90.0, South=180.0, West=270.0 and North=360.0.
        /// </remarks>
        double WindDirection { get; }

        /// <summary>
        /// Peak 3 second wind gust at the observatory over the last 2 minutes
        /// </summary>
        /// <value>Wind gust (m/s) Peak 3 second wind speed over the last 2 minutes</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException</b></p>
        /// The units of this property are metres per second. Driver and application authors can use the ASCOM.Tools.Utilities.ConvertUnits method
        /// to convert these units to and from miles per hour or knots.
        /// </remarks>
        double WindGust { get; }

        /// <summary>
        /// Wind speed at the observatory
        /// </summary>
        /// <value>Wind speed (m/s)</value>
        /// <exception cref="NotImplementedException">If this property is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Optional property, can throw a NotImplementedException</b></p>
        /// The units of this property are metres per second. Driver and application authors can use the ASCOM.Tools.Utilities.ConvertUnits method
        /// to convert these units to and from miles per hour or knots.
        /// </remarks>
        double WindSpeed { get; }


        /// <summary>
        /// Provides the time since the sensor value was last updated
        /// </summary>
        /// <param name="PropertyName">Name of the property whose time since last update is required</param>
        /// <returns>Time in seconds since the last sensor update for this property</returns>
        /// <exception cref="NotImplementedException">If the sensor is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid property name parameter is supplied.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Must Not throw a NotImplementedException when the specified sensor Is implemented but must throw a NotImplementedException when the specified sensor Is Not implemented.</b></p>
        /// <para>PropertyName must be the name of one of the sensor properties specified in the <see cref="IObservingConditions"/> interface. If the caller supplies some other value, throw an InvalidValueException.</para>
        /// <para>Return a negative value to indicate that no valid value has ever been received from the hardware.</para>
        /// <para>If an empty string is supplied as the PropertyName, the driver must return the time since the most recent update of any sensor. A NotImplementedException must not be thrown in this circumstance.</para>
        /// </remarks>
        double TimeSinceLastUpdate(string PropertyName);

        /// <summary>
        /// Provides a description of the sensor providing the requested property
        /// </summary>
        /// <param name="PropertyName">Name of the sensor whose description is required</param>
        /// <returns>The description of the specified sensor.</returns>
        /// <exception cref="NotImplementedException">If the sensor is not implemented.</exception>
        /// <exception cref="InvalidValueException">If an invalid property name parameter is supplied.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Must Not throw a NotImplementedException when the specified sensor Is implemented 
        /// but must throw a NotImplementedException when the specified sensor Is Not implemented.</b></p>
        /// <para>PropertyName must be the name of one of the sensor properties specified in the <see cref="IObservingConditions"/> interface. If the caller supplies some other value, throw an InvalidValueException.</para>
        /// <para>If the sensor is implemented, this must return a valid string, even if the driver is not connected, so that applications can use this to determine what sensors are available.</para>
        /// <para>If the sensor is not implemented, this must throw a NotImplementedException.</para>
        /// </remarks>
        string SensorDescription(string PropertyName);

        /// <summary>
        /// Forces the driver to immediately query its attached hardware to refresh sensor values
        /// </summary>
        /// <exception cref="NotImplementedException">If this method is not available.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. Include sufficient detail in the message text to enable the issue to be accurately diagnosed by someone other than yourself.</exception>
        /// <remarks>
        /// <p style="color:red"><b>Optional method, can throw a NotImplementedException</b></p>
        /// </remarks>
        void Refresh();
    }
}