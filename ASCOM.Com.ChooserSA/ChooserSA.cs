using ASCOM.Com.Exceptions;
using ASCOM.Common;
using ASCOM.Common.Interfaces;
using System;
using System.Diagnostics;
using System.Windows.Forms;

#if NET6_0_OR_GREATER
[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif

namespace ASCOM.Com
{
    /// <summary>
    /// The Chooser object provides a way for your application to let your user select the telescope to use.
    /// </summary>
    /// <remarks>
    /// <para>Calling Chooser.Choose() causes a chooser window to appear, with a drop down selector list containing all of the registered Telescope 
    /// drivers, listed by the driver's friendly/display name. The user sees a list of telescope types and selects one. 
    /// Before the OK button will light up, however, the user must click the Setup button, causing the selected driver's setup dialog to appear 
    /// (it calls the driver's Telescope.SetupDialog() method). When the setup dialog is closed, the OK button will light and allow the user 
    /// to close the Chooser itself.</para>
    /// 
    /// <para>The Choose() method returns the DriverID of the selected driver. Choose() allows you to optionally pass the DriverID of a "current" 
    /// driver (you probably save this in the registry), and the corresponding telescope type is pre-selected in the Chooser's list. In this case, 
    /// the OK button starts out enabled (lit-up); the assumption is that the pre-selected driver has already been configured. </para>
    /// </remarks>
    public class ChooserSA : IDisposable
    {
        private readonly ILogger logger;

        #region  New and IDisposable Support 

        private bool disposedValue = false;        // To detect redundant calls

        /// <summary>
        /// Creates a new Chooser object
        /// </summary>
        /// <param name="logger">Optional <see cref="ILogger"/> instance to which operational debug information will be reported.</param>
        /// <remarks></remarks>
        public ChooserSA(ILogger logger = null)
        {
            // Validate that the Platform is installed
            if (!PlatformUtilities.IsPlatformInstalled())
            {
                logger?.LogMessage(LogLevel.Error, "ChooserSA Init", "The ASCOM Platform is not installed on this device - throwing an InvalidOperationException. The ASCOM Stand Alone Chooser requires the ASCOM Platform to be installed.");
                throw new InvalidOperationException("The ASCOM Platform is not installed on this device; the ASCOM Stand Alone Chooser requires the ASCOM Platform to be installed.");
            }

            // Assign the debug logger if supplied
            this.logger = logger;
        }

        /// <summary>
        /// Does the work of cleaning up objects used by Chooser
        /// </summary>
        /// <param name="disposing">True if called by the user, false if called by the system</param>
        /// <remarks>You can't call this directly, use Dispose with no arguments instead.</remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

            }
            disposedValue = true;
        }

        /// <summary>
        /// Cleans up and disposes objects used by Chooser
        /// </summary>
        /// <remarks></remarks>
        public void Dispose()
        {
            // Do not change this code.  Put clean-up code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Public static members

        /// <summary>
        /// Static method to select a device without having to create a ChooserSA instance.
        /// </summary>
        /// <remarks>If a logger is provided, diagnostic information about the selection process will be
        /// logged. The method uses the specified <paramref name="deviceType"/> to determine the device type and uses
        /// the <paramref name="progId"/> to select a default driver if required.</remarks>
        /// <param name="deviceType">The type of device to be selected.</param>
        /// <param name="progId">An optional programmatic identifier (ProgID) used to refine the selection. Defaults to an empty string.</param>
        /// <param name="logger">An optional logger instance for capturing diagnostic information during the selection process. Can be <see
        /// langword="null"/>.</param>
        /// <returns>The identifier of the selected device as a string.</returns>
        public static string Choose(DeviceTypes deviceType, string progId = "", ILogger logger = null)
        {
            using ChooserSA chooser = new(logger)
            {
                DeviceType = deviceType
            };
            return chooser.Choose(progId);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// The type of device for which the Chooser will select a driver. (String, default = "Telescope")
        /// </summary>
        /// <value>The type of device for which the Chooser will select a driver. (String, default = "Telescope") 
        /// </value>
        /// <returns>The device type that has been set</returns>
        /// <exception cref="InvalidValueException">Thrown on setting the device type to empty string</exception>
        /// <remarks>This property changes the "personality" of the Chooser, allowing it to be used to select a driver for any arbitrary 
        /// ASCOM device type. The default value for this is "Telescope", but it could be "Focuser", "Camera", etc. 
        /// <para>This property is independent of the Profile object's DeviceType property. Setting Chooser's DeviceType 
        /// property doesn't set the DeviceType property in Profile, you must set that also when needed.</para>
        /// </remarks>
        public DeviceTypes DeviceType { get; set; } = DeviceTypes.Telescope;

        /// <summary>
        /// Select an ASCOM driver to use, pre-selecting one in the drop-down list
        /// </summary>
        /// <param name="progId">Driver to preselect in the chooser dialogue</param>
        /// <returns>Driver ID of chosen driver</returns>
        /// <remarks>The supplied driver will be pre-selected in the Chooser's list when the chooser window is first opened.
        /// <para>The device type will default to Telescope if no device type is set prior to calling this method.</para>
        /// </remarks>
        public string Choose(string progId)
        {
            string selectedProgId;
            ChooserForm chooserFormInstance;

            try
            {
                chooserFormInstance = new ChooserForm(logger)
                {
                    DeviceType = DeviceType,
                    SelectedProgId = progId
                }; // Initially hidden

                chooserFormInstance.ShowDialog(); // Display MODAL Chooser dialogue

                selectedProgId = chooserFormInstance.SelectedProgId;

                chooserFormInstance.Dispose();
            }

            catch (DriverNotRegisteredException ex)
            {
                MessageBox.Show("Chooser Exception: " + ex.Message);
                EventLog.LogEvent("Chooser", "Exception", EventLogEntryType.Error, GlobalConstants.EventLogErrors.ChooserException, ex.ToString());
                selectedProgId = "";
            }

            catch (Exception ex)
            {
                MessageBox.Show("Chooser Exception: " + ex.ToString());
                EventLog.LogEvent("Chooser", "Exception", EventLogEntryType.Error, GlobalConstants.EventLogErrors.ChooserException, ex.ToString());
                selectedProgId = "";
            }

            return selectedProgId;
        }

        /// <summary>
        /// Select ASCOM driver to use without pre-selecting in the dropdown list
        /// </summary>
        /// <returns>Driver ID of chosen driver</returns>
        /// <remarks>
        /// No driver will be pre-selected in the Chooser's list when the chooser window is first opened. 
        /// <para>The device type will default to Telescope if no device type is set prior to calling this method.</para>
        /// </remarks>
        public string Choose()
        {
            return Choose("");
        }

        #endregion

    }
}
