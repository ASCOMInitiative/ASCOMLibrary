using ASCOM.Alpaca.Discovery;
using ASCOM.Common;
using ASCOM.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ASCOM.Com
{

    /// <summary>
    /// Form displayed to enable the user to select an ASCOM device.
    /// </summary>
    internal partial class ChooserForm : Form
    {
        #region Constants

        // General constants
        private const string ALERT_MESSAGEBOX_TITLE = "ASCOM Chooser";
        private const string DRIVER_INITIALISATION_ERROR_MESSAGEBOX_TITLE = "Driver Initialization Error";
        private const string SETUP_DIALOGUE_ERROR_MESSAGEBOX_TITLE = "Driver Setup Dialog Error";
        private const int PROPERTIES_TOOLTIP_DISPLAY_TIME = 5000; // Time to display the Properties tooltip (milliseconds)
        private const int FORM_LOAD_WARNING_MESSAGE_DELAY_TIME = 250; // Delay time before any warning message is displayed on form load
        private const int ALPACA_STATUS_BLINK_TIME = 100; // Length of time the Alpaca status indicator spends in the on and off state (ms)
        private const string TOOLTIP_PROPERTIES_TITLE = "Driver Setup";
        private const string TOOLTIP_PROPERTIES_MESSAGE = "Check or change driver Properties (configuration)";
        private const string TOOLTIP_PROPERTIES_FIRST_TIME_MESSAGE = "You must check driver configuration before first time use, please click the Properties... button.\r\nThe OK button will remain greyed out until this is done.";
        private const string TOOLTIP_CREATE_ALPACA_DEVICE_TITLE = "Alpaca Device Selected";
        private const string TOOLTIP_CREATE_ALPACA_DEVICE_MESSAGE = "Please click this button to create the Alpaca Dynamic driver";
        private const int TOOLTIP_CREATE_ALPACA_DEVICE_DISPLAYTIME = 5; // Number of seconds to display the Create Alpaca Device informational message
        private const int CHOOSER_LIST_WIDTH_NEW_ALPACA = 600; // Width of the Chooser list when new Alpaca devices are present

        // Chooser persistence constants
        internal const string CONFIGRATION_SUBKEY = @"Chooser\Configuration"; // Store configuration in a subkey under the Chooser key
        private const string ALPACA_ENABLED = "Alpaca enabled";
        private const bool ALPACA_ENABLED_DEFAULT = false;
        internal const string ALPACA_DISCOVERY_PORT = "Alpaca discovery port";
        internal const int ALPACA_DISCOVERY_PORT_DEFAULT = 32227;
        private const string ALPACA_NUMBER_OF_BROADCASTS = "Alpaca number of broadcasts";
        private const int ALPACA_NUMBER_OF_BROADCASTS_DEFAULT = 2;
        private const string ALPACA_TIMEOUT = "Alpaca timeout";
        private const double ALPACA_TIMEOUT_DEFAULT = 1.0d;
        private const string ALPACA_DNS_RESOLUTION = "Alpaca DNS resolution";
        private const bool ALPACA_DNS_RESOLUTION_DEFAULT = false;
        private const string ALPACA_SHOW_DISCOVERED_DEVICES = "Show discovered Alpaca devices";
        private const bool ALPACA_SHOW_DISCOVERED_DEVICES_DEFAULT = false;
        private const string ALPACA_SHOW_DEVICE_DETAILS = "Show Alpaca device details";
        private const bool ALPACA_SHOW_DEVICE_DETAILS_DEFAULT = false;
        private const string ALPACA_CHOOSER_WIDTH = "Alpaca Chooser width";
        private const int ALPACA_CHOOSER_WIDTH_DEFAULT = 0;
        private const string ALPACA_USE_IPV4 = "Use IPv4";
        private const bool ALPACA_USE_IPV4_DEFAULT = true;
        private const string ALPACA_USE_IPV6 = "Use IPv6";
        private const bool ALPACA_USE_IPV6_DEFAULT = false;
        private const string ALPACA_MULTI_THREADED_CHOOSER = "Multi Threaded Chooser";
        private const bool ALPACA_MULTI_THREADED_CHOOSER_DEFAULT = true;

        // Alpaca integration constants
        private const string ALPACA_DYNAMIC_CLIENT_MANAGER_RELATIVE_PATH = @"ASCOM\Platform 6\Tools\AlpacaDynamicClientManager";
        private const string ALPACA_DYNAMIC_CLIENT_MANAGER_EXE_NAME = "ASCOM.AlpacaDynamicClientManager.exe";
        private const string DRIVER_PROGID_BASE = "ASCOM.AlpacaDynamic";

        // Alpaca driver Profile store value names
        private const string PROFILE_VALUE_NAME_UNIQUEID = "UniqueID"; // Prefix applied to all COM drivers created to front Alpaca devices
        private const string PROFILE_VALUE_NAME_IP_ADDRESS = "IP Address";
        private const string PROFILE_VALUE_NAME_PORT_NUMBER = "Port Number";
        private const string PROFILE_VALUE_NAME_REMOTE_DEVICER_NUMBER = "Remote Device Number";

        #endregion

        #region Variables

        // Chooser variables
        private string deviceTypeValue, selectedProgIdValue;
        private List<ChooserItem> chooserList;
        private string driverIsCompatible = "";
        private string currentWarningTitle, currentWarningMesage;
        private List<AscomDevice> alpacaDevices = new();
        private ChooserItem selectedChooserItem;
        private Process _ClientManagerProcess;

        private Process ClientManagerProcess
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _ClientManagerProcess;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_ClientManagerProcess != null)
                {
                    _ClientManagerProcess.Exited -= DriverGeneration_Complete;
                }

                _ClientManagerProcess = value;
                if (_ClientManagerProcess != null)
                {
                    _ClientManagerProcess.Exited += DriverGeneration_Complete;
                }
            }
        }
        private bool driverGenerationComplete;
        private bool currentOkButtonEnabledState;
        private bool currentPropertiesButtonEnabledState;

        // Component variables
        private readonly ILogger TL;
        private ToolTip chooserWarningToolTip;
        private ToolTip chooserPropertiesToolTip;
        private ToolTip createAlpacaDeviceToolTip;
        private ToolStripLabel alpacaStatusToolstripLabel;
        private System.Windows.Forms.Timer _AlpacaStatusIndicatorTimer;

        private System.Windows.Forms.Timer AlpacaStatusIndicatorTimer
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _AlpacaStatusIndicatorTimer;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_AlpacaStatusIndicatorTimer != null)
                {
                    _AlpacaStatusIndicatorTimer.Tick -= AlpacaStatusIndicatorTimerEventHandler;
                }

                _AlpacaStatusIndicatorTimer = value;
                if (_AlpacaStatusIndicatorTimer != null)
                {
                    _AlpacaStatusIndicatorTimer.Tick += AlpacaStatusIndicatorTimerEventHandler;
                }
            }
        }
        private readonly RegistryAccess registryAccess;

        // Persistence variables
        internal bool AlpacaEnabled;
        internal int AlpacaDiscoveryPort;
        internal int AlpacaNumberOfBroadcasts;
        internal double AlpacaTimeout;
        internal bool AlpacaDnsResolution;
        internal bool AlpacaShowDiscoveredDevices;
        internal bool AlpacaShowDeviceDetails;
        internal int AlpacaChooserIncrementalWidth;
        internal bool AlpacaUseIpV4;
        internal bool AlpacaUseIpV6;
        internal bool AlpacaMultiThreadedChooser;

        // Delegates
        private readonly MethodInvoker PopulateDriverComboBoxDelegate;
        private readonly MethodInvoker SetStateNoAlpacaDelegate;
        private readonly MethodInvoker SetStateAlpacaDiscoveringDelegate;
        private readonly MethodInvoker SetStateAlpacaDiscoveryCompleteFoundDevicesDelegate;
        private readonly MethodInvoker SetStateAlpacaDiscoveryCompleteNoDevicesDelegate;

        // Chooser form control positions
        private readonly int OriginalForm1Width;
        private Point OriginalBtnCancelPosition;
        private Point OriginalBtnOKPosition;
        private Point OriginalBtnPropertiesPosition;
        private readonly int OriginalCmbDriverSelectorWidth;
        private readonly int OriginalLblAlpacaDiscoveryPosition;
        private readonly int OriginalAlpacaStatusPosition;
        private readonly int OriginalDividerLineWidth;

        #endregion

        #region Form load, close, paint and dispose event handlers

        public ChooserForm(ILogger logger) : base()
        {
            TL = logger;
            PopulateDriverComboBoxDelegate = PopulateDriverComboBox; // Device list combo box delegate
            SetStateNoAlpacaDelegate = SetStateNoAlpaca;
            SetStateAlpacaDiscoveringDelegate = SetStateAlpacaDiscovering;
            SetStateAlpacaDiscoveryCompleteFoundDevicesDelegate = SetStateAlpacaDiscoveryCompleteFoundDevices;
            SetStateAlpacaDiscoveryCompleteNoDevicesDelegate = SetStateAlpacaDiscoveryCompleteNoDevices;
            displayCreateAlpacDeviceTooltip = new NoParameterDelegate(DisplayAlpacaDeviceToolTip);
            InitializeComponent();

            // Record initial control positions
            OriginalForm1Width = Width;
            OriginalBtnCancelPosition = BtnCancel.Location;
            OriginalBtnOKPosition = BtnOK.Location;
            OriginalBtnPropertiesPosition = BtnProperties.Location;
            OriginalCmbDriverSelectorWidth = CmbDriverSelector.Width;
            OriginalLblAlpacaDiscoveryPosition = LblAlpacaDiscovery.Left;
            OriginalAlpacaStatusPosition = AlpacaStatus.Left;
            OriginalDividerLineWidth = DividerLine.Width;

            // Get access to the profile registry area
            registryAccess = new RegistryAccess();

            ReadState(); // Read in the state variables from persisted storage
            ResizeChooser();

        }

        private void ChooserForm_Load(object eventSender, EventArgs eventArgs)
        {

            try
            {

                // Initialise form title and message text
                Text = "ASCOM " + deviceTypeValue + " Chooser";
                lblTitle.Text = "Select the type of " + deviceTypeValue.ToLowerInvariant() + " you have, then be " + "sure to click the Properties... button to configure the driver for your " + deviceTypeValue.ToLowerInvariant() + ".";

                // Initialise the tooltip warning for 32/64bit driver compatibility messages
                chooserWarningToolTip = new ToolTip();

                CmbDriverSelector.DropDownWidth = CHOOSER_LIST_WIDTH_NEW_ALPACA;

                // Configure the Properties button tooltip
                chooserPropertiesToolTip = new ToolTip
                {
                    IsBalloon = true,
                    ToolTipIcon = ToolTipIcon.Info,
                    UseFading = true,
                    ToolTipTitle = TOOLTIP_PROPERTIES_TITLE
                };
                chooserPropertiesToolTip.SetToolTip(BtnProperties, TOOLTIP_PROPERTIES_MESSAGE);

                // Create Alpaca information tooltip 
                createAlpacaDeviceToolTip = new ToolTip();

                // Set a custom rendered for the tool strip so that colours and appearance can be controlled better
                ChooserMenu.Renderer = new ChooserCustomToolStripRenderer();

                // Create a tool strip label whose background colour can  be changed and add it at the top of the Alpaca menu
                alpacaStatusToolstripLabel = new ToolStripLabel("Discovery status unknown");
                MnuAlpaca.DropDownItems.Insert(0, alpacaStatusToolstripLabel);

                RefreshTraceMenu(); // Refresh the trace menu

                // Set up the Alpaca status blink timer but make sure its not running
                AlpacaStatusIndicatorTimer = new System.Windows.Forms.Timer
                {
                    Interval = ALPACA_STATUS_BLINK_TIME // Set it to fire after 250ms
                };
                AlpacaStatusIndicatorTimer.Stop();

                TL.LogMessage(LogLevel.Debug, "ChooserForm_Load", $"UI thread: {Environment.CurrentManagedThreadId}");

                InitialiseComboBox(); // ' Kick off a discover and populate the combo box or just populate the combo box if no discovery is required
            }

            catch (Exception ex)
            {
                MessageBox.Show("ChooserForm Load " + ex.ToString());
                EventLogCode.LogEvent("ChooserForm Load ", ex.ToString(), EventLogEntryType.Error, GlobalConstants.EventLogErrors.ChooserFormLoad, ex.ToString());
            }
        }

        private void ChooserForm_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        /// <summary>
        /// Dispose of disposable components
        /// </summary>
        /// <param name="Disposing"></param>
        protected override void Dispose(bool Disposing)
        {
            if (Disposing)
            {
                components?.Dispose();
                if (TL is not null)
                {
                }
                if (chooserWarningToolTip is not null)
                {
                    try
                    {
                        chooserWarningToolTip.Dispose();
                    }
                    catch
                    {
                    }
                }
                if (chooserPropertiesToolTip is not null)
                {
                    try
                    {
                        chooserPropertiesToolTip.Dispose();
                    }
                    catch
                    {
                    }
                }
                if (alpacaStatusToolstripLabel is not null)
                {
                    try
                    {
                        alpacaStatusToolstripLabel.Dispose();
                    }
                    catch
                    {
                    }
                }
                if (registryAccess is not null)
                {
                    try
                    {
                        registryAccess.Dispose();
                    }
                    catch
                    {
                    }
                }
            }
            base.Dispose(Disposing);
        }

        #endregion

        #region Public methods

        public string DeviceType
        {
            set
            {

                // Clean up the supplied device type to consistent values
                switch (value.ToLowerInvariant() ?? "")
                {
                    case "camera":
                        {
                            deviceTypeValue = "Camera";
                            break;
                        }
                    case "covercalibrator":
                        {
                            deviceTypeValue = "CoverCalibrator";
                            break;
                        }
                    case "dome":
                        {
                            deviceTypeValue = "Dome";
                            break;
                        }
                    case "filterwheel":
                        {
                            deviceTypeValue = "FilterWheel";
                            break;
                        }
                    case "focuser":
                        {
                            deviceTypeValue = "Focuser";
                            break;
                        }
                    case "observingconditions":
                        {
                            deviceTypeValue = "ObservingConditions";
                            break;
                        }
                    case "rotator":
                        {
                            deviceTypeValue = "Rotator";
                            break;
                        }
                    case "safetymonitor":
                        {
                            deviceTypeValue = "SafetyMonitor";
                            break;
                        }
                    case "switch":
                        {
                            deviceTypeValue = "Switch";
                            break;
                        }
                    case "telescope":
                        {
                            deviceTypeValue = "Telescope";
                            break;
                        }
                    case "video":
                        {
                            deviceTypeValue = "Video"; // If not recognised just use as supplied for backward compatibility
                            break;
                        }

                    default:
                        {
                            deviceTypeValue = value;
                            break;
                        }
                }

                TL.LogMessage(LogLevel.Debug, "DeviceType Set", deviceTypeValue);
                ReadState(deviceTypeValue);
            }
        }

        public string SelectedProgId
        {
            get
            {
                return selectedProgIdValue;
            }
            set
            {
                selectedProgIdValue = value;
                TL.LogMessage(LogLevel.Debug, "InitiallySelectedProgId Set", selectedProgIdValue);
            }
        }

        #endregion

        #region Form, button, control and timer event handlers

        private void ComboProduct_DrawItem(object sender, DrawItemEventArgs e) // Handles CmbDriverSelector.DrawItem
        {
            Brush brush;
            Color colour;
            ComboBox combo;
            string text = "";

            try
            {
                e.DrawBackground();
                combo = (ComboBox)sender;

                brush = Brushes.White;
                colour = Color.White;

                if (combo.SelectedIndex >= 0)
                {
                    ChooserItem chooseritem = (ChooserItem)combo.Items[e.Index];

                    TL.LogMessage(LogLevel.Debug, "comboProduct_DrawItem", $"IsComDriver: {chooseritem.IsComDriver} {chooseritem.AscomName}");
                    text = chooseritem.AscomName;
                    if (chooseritem.IsComDriver)
                    {
                        if (chooseritem.ProgID.ToLowerInvariant().StartsWith(DRIVER_PROGID_BASE.ToLowerInvariant()))
                        {
                            brush = Brushes.Red;
                            colour = Color.Red;
                        }
                        else
                        {
                            brush = Brushes.LightPink;
                            colour = Color.LightPink;
                        }
                    }
                    else
                    {
                        brush = Brushes.LightGreen;
                        colour = Color.LightGreen;
                    }
                }

                e.Graphics.DrawRectangle(new Pen(Color.Black), e.Bounds);
                e.Graphics.FillRectangle(brush, e.Bounds);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawString(text, combo.Font, Brushes.Black, e.Bounds.X, e.Bounds.Y);
            }

            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private void ChooserFormMoveEventHandler(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentWarningMesage))
                WarningToolTipShow(currentWarningTitle, currentWarningMesage);
            DisplayAlpacaDeviceToolTip();
        }

        private void AlpacaStatusIndicatorTimerEventHandler(object myObject, EventArgs myEventArgs)
        {
            if (AlpacaStatus.BackColor == Color.Orange)
            {
                AlpacaStatus.BackColor = Color.DimGray;
            }
            else
            {
                AlpacaStatus.BackColor = Color.Orange;
            }

        }

        /// <summary>
        /// Click in Properties... button. Loads the currently selected driver and activate its setup dialogue.
        /// </summary>
        /// <param name="eventSender"></param>
        /// <param name="eventArgs"></param>
        private void CmdProperties_Click(object eventSender, EventArgs eventArgs)
        {
            dynamic oDrv = null; // The driver
            bool bConnected;
            string sProgID;
            Type ProgIdType;

            // Find ProgID corresponding to description
            sProgID = ((ChooserItem)CmbDriverSelector.SelectedItem).ProgID;

            TL.LogMessage(LogLevel.Debug, "PropertiesClick", "ProgID:" + sProgID);
            try
            {
                // New Platform 6 behaviour
                ProgIdType = Type.GetTypeFromProgID(sProgID);
                oDrv = Activator.CreateInstance(ProgIdType);

                // Here we try to see if a device is already connected. If so, alert and just turn on the OK button.
                bConnected = false;
                try
                {
                    bConnected = (bool)oDrv.Connected;
                }
                catch
                {
                    try
                    {
                        bConnected = (bool)oDrv.Link;
                    }
                    catch
                    {
                    }
                }

                if (bConnected) // Already connected so cannot show the Setup dialogue
                {
                    MessageBox.Show("The device is already connected. Just click OK.", ALERT_MESSAGEBOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else // Not connected, so call the SetupDialog method
                {
                    try
                    {
                        WarningTooltipClear(); // Clear warning tool tip before entering setup so that the dialogue doesn't interfere with or obscure the setup dialogue.
                        oDrv.SetupDialog();
                    }
                    catch (Exception ex) // Something went wrong in the SetupDialog method so display an error message.
                    {
                        MessageBox.Show($"The SetupDialog method of driver \"{sProgID}\" threw an exception when called.\r\n\r\n" +
                            $"This means that the setup dialogue would not start properly.\r\n\r\n" +
                            $"Please screen print or use CTRL+C to copy all of this message and report it to the driver author with a request for assistance.\r\n" +
                            $"{ex.GetType().Name} - {ex.Message}", SETUP_DIALOGUE_ERROR_MESSAGEBOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        EventLogCode.LogEvent("ChooserForm", "Driver setup method failed for driver: \"" + sProgID + "\"", EventLogEntryType.Error, GlobalConstants.EventLogErrors.ChooserSetupFailed, $"{ex.GetType().Name} - {ex.Message}");
                    }
                }

                registryAccess.WriteProfile("Chooser", sProgID + " Init", "True"); // Remember it has been initialized
                EnableOkButton(true);
                WarningTooltipClear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"The driver \"{sProgID}\" threw an exception when loaded.\r\n\r\n" +
                    $"This means that the driver would not start properly.\r\n\r\n" +
                    $"Please screen print or use CTRL+C to copy all of this message and report it to the driver author with a request for assistance.\r\n\r\n" +
                    $"{ex}", DRIVER_INITIALISATION_ERROR_MESSAGEBOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                EventLogCode.LogEvent("ChooserForm", "Failed to load driver:  \"" + sProgID + "\"", EventLogEntryType.Error, GlobalConstants.EventLogErrors.ChooserDriverFailed, ex.ToString());
            }

            // Clean up and release resources
            try
            {
                oDrv.Dispose();
            }
            catch (Exception)
            {
            }
            try
            {
                Marshal.ReleaseComObject(oDrv);
            }
            catch (Exception)
            {
            }

        }

        private void CmdCancel_Click(object eventSender, EventArgs eventArgs)
        {
            selectedProgIdValue = "";
            Hide();
        }

        private void CmdOK_Click(object eventSender, EventArgs eventArgs)
        {
            string newProgId;
            DialogResult userResponse;

            TL.LogMessage(LogLevel.Debug, "OK Click", $"Combo box selected index = {CmbDriverSelector.SelectedIndex}");

            if (selectedChooserItem.IsComDriver) // User has selected an existing COM driver so return its ProgID
            {
                selectedProgIdValue = selectedChooserItem.ProgID;

                TL.LogMessage(LogLevel.Debug, "OK Click", $"Returning ProgID: '{selectedProgIdValue}'");

                // Close the UI because the COM driver is selected
                Hide();
            }
            else // User has selected a new Alpaca device so we need to create a new COM driver for this
            {

                // SHow the admin request dialogue if it has not been suppressed by the user
                if (!RegistryCommonCode.GetBool(GlobalConstants.SUPPRESS_ALPACA_DRIVER_ADMIN_DIALOGUE, GlobalConstants.SUPPRESS_ALPACA_DRIVER_ADMIN_DIALOGUE_DEFAULT)) // The admin request coming dialogue has not been suppressed so show the dialogue
                {
                    using (var checkedMessageBox = new CheckedMessageBox())
                    {
                        userResponse = checkedMessageBox.ShowDialog();
                    }
                }
                else // The admin request coming dialogue has been suppressed so flag the user response as OK
                {
                    userResponse = DialogResult.OK;
                }

                // Test whether the user clicked the OK button or pressed the "x" cancel icon in the top right of the form
                if (userResponse == DialogResult.OK) // User pressed the OK button
                {

                    try
                    {
                        // Create a new Alpaca driver of the current ASCOM device type
                        newProgId = CreateNewAlpacaDriver(selectedChooserItem.AscomName);

                        // Configure the IP address, port number and Alpaca device number in the newly registered driver
                        Profile.SetValue(Devices.StringToDeviceType(deviceTypeValue), newProgId, PROFILE_VALUE_NAME_IP_ADDRESS, selectedChooserItem.HostName);
                        Profile.SetValue(Devices.StringToDeviceType(deviceTypeValue), newProgId, PROFILE_VALUE_NAME_PORT_NUMBER, selectedChooserItem.Port.ToString());
                        Profile.SetValue(Devices.StringToDeviceType(deviceTypeValue), newProgId, PROFILE_VALUE_NAME_REMOTE_DEVICER_NUMBER, selectedChooserItem.DeviceNumber.ToString());
                        Profile.SetValue(Devices.StringToDeviceType(deviceTypeValue), newProgId, PROFILE_VALUE_NAME_UNIQUEID, selectedChooserItem.DeviceUniqueID.ToString());

                        // Flag the driver as being already configured so that it can be used immediately
                        registryAccess.WriteProfile("Chooser", $"{newProgId} Init", "True");

                        // Select the new driver in the Chooser combo box list
                        selectedProgIdValue = newProgId;
                        InitialiseComboBox();

                        TL.LogMessage(LogLevel.Debug, "OK Click", $"Returning ProgID: '{selectedProgIdValue}'");
                    }
                    catch (Win32Exception ex) when (ex.ErrorCode == int.MinValue + 0x00004005)
                    {
                        TL.LogMessage(LogLevel.Debug, "OK Click", $"Driver creation cancelled: {ex.Message}");
                        MessageBox.Show($"Driver creation cancelled: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{ex}");
                    }
                }

                // Don't exit the Chooser but instead return to the UI so that the user can see that a new driver has been created and selected
            }
        }

        /// <summary>
        /// Driver generation completion event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DriverGeneration_Complete(object sender, EventArgs e)
        {
            driverGenerationComplete = true; // Flag that driver generation is complete
        }

        private void CbDriverSelector_SelectionChangeCommitted(object eventSender, EventArgs eventArgs)
        {
            if (CmbDriverSelector.SelectedIndex >= 0)
            {

                // Save the newly selected chooser item
                selectedChooserItem = (ChooserItem)CmbDriverSelector.SelectedItem;
                selectedProgIdValue = selectedChooserItem.ProgID;

                // Validate the driver if it is a COM driver
                if (selectedChooserItem.IsComDriver) // This is a COM driver
                {
                    TL.LogMessage(LogLevel.Debug, "SelectedIndexChanged", $"New COM driver selected. ProgID: {selectedChooserItem.ProgID} {selectedChooserItem.AscomName}");
                    ValidateDriver(selectedChooserItem.ProgID);
                }
                else // This is a new Alpaca driver
                {
                    TL.LogMessage(LogLevel.Debug, "SelectedIndexChanged", $"New Alpaca driver selected : {selectedChooserItem.AscomName}");
                    EnablePropertiesButton(false); // Disable the Properties button because there is not yet a COM driver to configure
                    WarningTooltipClear();
                    EnableOkButton(true);
                    DisplayAlpacaDeviceToolTip();
                }
            }

            else // Selected index is negative
            {
                TL.LogMessage(LogLevel.Debug, "SelectedIndexChanged", $"Ignoring index changed event because no item is selected: {CmbDriverSelector.SelectedIndex}");
            }
        }

        private void PicASCOM_Click(object eventSender, EventArgs eventArgs)
        {
            try
            {
                Process.Start("https://ASCOM-Standards.org/");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to display ASCOM-Standards web site in your browser: " + ex.Message, ALERT_MESSAGEBOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        #endregion

        #region Menu code and event handlers

        private void RefreshTraceMenu()
        {
            string TraceFileName;


            TraceFileName = registryAccess.GetProfile("", GlobalConstants.SERIAL_FILE_NAME_VARNAME);
            switch (TraceFileName ?? "") // Trace is disabled
            {
                case var @case when @case == "":
                    {
                        MenuSerialTraceEnabled.Checked = false; // The trace enabled flag is unchecked and disabled
                        MenuSerialTraceEnabled.Enabled = true;
                        break;
                    }
                case GlobalConstants.SERIAL_AUTO_FILENAME: // Tracing is on using an automatic filename
                    {
                        MenuSerialTraceEnabled.Checked = true; // The trace enabled flag is checked and enabled
                        MenuSerialTraceEnabled.Enabled = true; // Tracing using some other fixed filename
                        break;
                    }

                default:
                    {
                        MenuSerialTraceEnabled.Checked = true; // The trace enabled flag is checked and enabled
                        MenuSerialTraceEnabled.Enabled = true;
                        break;
                    }
            }

            // Set Profile trace checked state on menu item 
            MenuProfileTraceEnabled.Checked = RegistryCommonCode.GetBool(GlobalConstants.TRACE_PROFILE, GlobalConstants.TRACE_PROFILE_DEFAULT);
            MenuRegistryTraceEnabled.Checked = RegistryCommonCode.GetBool(GlobalConstants.TRACE_XMLACCESS, GlobalConstants.TRACE_XMLACCESS_DEFAULT);
            MenuUtilTraceEnabled.Checked = RegistryCommonCode.GetBool(GlobalConstants.TRACE_UTIL, GlobalConstants.TRACE_UTIL_DEFAULT);
            MenuTransformTraceEnabled.Checked = RegistryCommonCode.GetBool(GlobalConstants.TRACE_TRANSFORM, GlobalConstants.TRACE_TRANSFORM_DEFAULT);
            MenuSimulatorTraceEnabled.Checked = RegistryCommonCode.GetBool(GlobalConstants.SIMULATOR_TRACE, GlobalConstants.SIMULATOR_TRACE_DEFAULT);
            MenuDriverAccessTraceEnabled.Checked = RegistryCommonCode.GetBool(GlobalConstants.DRIVERACCESS_TRACE, GlobalConstants.DRIVERACCESS_TRACE_DEFAULT);
            MenuAstroUtilsTraceEnabled.Checked = RegistryCommonCode.GetBool(GlobalConstants.ASTROUTILS_TRACE, GlobalConstants.ASTROUTILS_TRACE_DEFAULT);
            MenuNovasTraceEnabled.Checked = RegistryCommonCode.GetBool(GlobalConstants.NOVAS_TRACE, GlobalConstants.NOVAS_TRACE_DEFAULT);
            MenuCacheTraceEnabled.Checked = RegistryCommonCode.GetBool(GlobalConstants.TRACE_CACHE, GlobalConstants.TRACE_CACHE_DEFAULT);
            MenuEarthRotationDataFormTraceEnabled.Checked = RegistryCommonCode.GetBool(GlobalConstants.TRACE_EARTHROTATION_DATA_FORM, GlobalConstants.TRACE_EARTHROTATION_DATA_FORM_DEFAULT);

        }

        private void MenuAutoTraceFilenames_Click(object sender, EventArgs e)
        {
            // Auto filenames currently disabled, so enable them
            MenuSerialTraceEnabled.Enabled = true; // Set the trace enabled flag
            MenuSerialTraceEnabled.Checked = true; // Enable the trace enabled flag
            registryAccess.WriteProfile("", GlobalConstants.SERIAL_FILE_NAME_VARNAME, GlobalConstants.SERIAL_AUTO_FILENAME);
        }

        private void MenuSerialTraceFile_Click(object sender, EventArgs e)
        {
            DialogResult RetVal;

            RetVal = SerialTraceFileName.ShowDialog();
            switch (RetVal)
            {
                case DialogResult.OK:
                    {
                        // Save the result
                        registryAccess.WriteProfile("", GlobalConstants.SERIAL_FILE_NAME_VARNAME, SerialTraceFileName.FileName);
                        // Check and enable the serial trace enabled flag
                        MenuSerialTraceEnabled.Enabled = true;
                        // Enable manual serial trace file flag
                        MenuSerialTraceEnabled.Checked = true; // Ignore everything else
                        break;
                    }

                default:
                    {
                        break;
                    }

            }
        }

        private void MenuSerialTraceEnabled_Click(object sender, EventArgs e)
        {

            if (MenuSerialTraceEnabled.Checked) // Auto serial trace is on so turn it off
            {
                MenuSerialTraceEnabled.Checked = false;
                registryAccess.WriteProfile("", GlobalConstants.SERIAL_FILE_NAME_VARNAME, "");
            }
            else // Auto serial trace is off so turn it on
            {
                MenuSerialTraceEnabled.Checked = true;
                registryAccess.WriteProfile("", GlobalConstants.SERIAL_FILE_NAME_VARNAME, GlobalConstants.SERIAL_AUTO_FILENAME);
            }
        }

        private void MenuProfileTraceEnabled_Click_1(object sender, EventArgs e)
        {
            MenuProfileTraceEnabled.Checked = !MenuProfileTraceEnabled.Checked; // Invert the selection
            RegistryCommonCode.SetName(GlobalConstants.TRACE_PROFILE, MenuProfileTraceEnabled.Checked.ToString());
        }

        private void MenuRegistryTraceEnabled_Click(object sender, EventArgs e)
        {
            MenuRegistryTraceEnabled.Checked = !MenuRegistryTraceEnabled.Checked; // Invert the selection
            RegistryCommonCode.SetName(GlobalConstants.TRACE_XMLACCESS, MenuRegistryTraceEnabled.Checked.ToString());
        }

        private void MenuUtilTraceEnabled_Click_1(object sender, EventArgs e)
        {
            MenuUtilTraceEnabled.Checked = !MenuUtilTraceEnabled.Checked; // Invert the selection
            RegistryCommonCode.SetName(GlobalConstants.TRACE_UTIL, MenuUtilTraceEnabled.Checked.ToString());
        }

        private void MenuTransformTraceEnabled_Click(object sender, EventArgs e)
        {
            MenuTransformTraceEnabled.Checked = !MenuTransformTraceEnabled.Checked; // Invert the selection
            RegistryCommonCode.SetName(GlobalConstants.TRACE_TRANSFORM, MenuTransformTraceEnabled.Checked.ToString());
        }

        private void MenuIncludeSerialTraceDebugInformation_Click(object sender, EventArgs e)
        {
            // MenuIncludeSerialTraceDebugInformation.Checked = Not MenuIncludeSerialTraceDebugInformation.Checked 'Invert selection
            // SetName(SERIAL_TRACE_DEBUG, MenuIncludeSerialTraceDebugInformation.Checked.ToString)
        }

        private void MenuSimulatorTraceEnabled_Click(object sender, EventArgs e)
        {
            MenuSimulatorTraceEnabled.Checked = !MenuSimulatorTraceEnabled.Checked; // Invert selection
            RegistryCommonCode.SetName(GlobalConstants.SIMULATOR_TRACE, MenuSimulatorTraceEnabled.Checked.ToString());
        }

        private void MenuCacheTraceEnabled_Click(object sender, EventArgs e)
        {
            MenuCacheTraceEnabled.Checked = !MenuCacheTraceEnabled.Checked; // Invert selection
            RegistryCommonCode.SetName(GlobalConstants.TRACE_CACHE, MenuCacheTraceEnabled.Checked.ToString());
        }

        private void MenuEarthRotationDataTraceEnabled_Click(object sender, EventArgs e)
        {
            MenuEarthRotationDataFormTraceEnabled.Checked = !MenuEarthRotationDataFormTraceEnabled.Checked; // Invert selection
            RegistryCommonCode.SetName(GlobalConstants.TRACE_EARTHROTATION_DATA_FORM, MenuEarthRotationDataFormTraceEnabled.Checked.ToString());
        }

        private void MenuTrace_DropDownOpening(object sender, EventArgs e)
        {
            RefreshTraceMenu();
        }

        private void MenuDriverAccessTraceEnabled_Click(object sender, EventArgs e)
        {
            MenuDriverAccessTraceEnabled.Checked = !MenuDriverAccessTraceEnabled.Checked; // Invert selection
            RegistryCommonCode.SetName(GlobalConstants.DRIVERACCESS_TRACE, MenuDriverAccessTraceEnabled.Checked.ToString());
        }

        private void MenuAstroUtilsTraceEnabled_Click(object sender, EventArgs e)
        {
            MenuAstroUtilsTraceEnabled.Checked = !MenuAstroUtilsTraceEnabled.Checked; // Invert selection
            RegistryCommonCode.SetName(GlobalConstants.ASTROUTILS_TRACE, MenuAstroUtilsTraceEnabled.Checked.ToString());
        }

        private void MenuNovasTraceEnabled_Click(object sender, EventArgs e)
        {
            MenuNovasTraceEnabled.Checked = !MenuNovasTraceEnabled.Checked; // Invert selection
            RegistryCommonCode.SetName(GlobalConstants.NOVAS_TRACE, MenuNovasTraceEnabled.Checked.ToString());
        }

        private void MnuEnableDiscovery_Click(object sender, EventArgs e)
        {
            AlpacaEnabled = true;
            WriteState(deviceTypeValue);
            InitialiseComboBox();
        }

        private void MnuDisableDiscovery_Click(object sender, EventArgs e)
        {
            AlpacaEnabled = false;
            WriteState(deviceTypeValue);
            InitialiseComboBox();
            SetStateNoAlpaca();
        }

        private void MnuDiscoverNow_Click(object sender, EventArgs e)
        {
            InitialiseComboBox();
        }

        private void MnuConfigureDiscovery_Click(object sender, EventArgs e)
        {
            ChooserAlpacaConfigurationForm alpacaConfigurationForm;

            TL.LogMessage(LogLevel.Debug, "ConfigureDiscovery", $"About to create Alpaca configuration form");
            alpacaConfigurationForm = new ChooserAlpacaConfigurationForm(this); // Create a new configuration form
            alpacaConfigurationForm.ShowDialog(); // Display the form as a modal dialogue box
            TL.LogMessage(LogLevel.Debug, "ConfigureDiscovery", $"Exited Alpaca configuration form. Result: {alpacaConfigurationForm.DialogResult}");

            if (alpacaConfigurationForm.DialogResult == DialogResult.OK) // If the user clicked OK then persist the new state
            {
                TL.LogMessage(LogLevel.Debug, "ConfigureDiscovery", $"Persisting new configuration for {deviceTypeValue}");
                WriteState(deviceTypeValue);

                ResizeChooser(); // Resize the chooser to reflect any configuration change

                InitialiseComboBox(); // ' Kick off a discover and populate the combo box or just populate the combo box if no discovery is required

            }

            alpacaConfigurationForm.Dispose(); // Dispose of the configuration form

        }

        private void MnuManageAlpacaDevices_Click(object sender, EventArgs e)
        {
            bool deviceWasRegistered;

            // Get the current registration state for the selected ProgID
            deviceWasRegistered = Profile.IsRegistered(Devices.StringToDeviceType(deviceTypeValue), selectedProgIdValue);

            TL.LogMessage(LogLevel.Debug, "ManageAlpacaDevicesClick", $"ProgID {selectedProgIdValue} of type {deviceTypeValue} is registered: {deviceWasRegistered}");

            // Run the client manager in manage mode
            RunDynamicClientManager("ManageDevices");

            // Test whether the selected ProgID has just been deleted and if so unselect the ProgID
            if (deviceWasRegistered)
            {
                // Unselect the ProgID if it has just been deleted
                if (!Profile.IsRegistered(Devices.StringToDeviceType(deviceTypeValue), selectedProgIdValue))
                {
                    selectedChooserItem = null;
                    TL.LogMessage(LogLevel.Debug, "ManageAlpacaDevicesClick", $"ProgID {selectedProgIdValue} was registered but has been deleted");
                }
                else
                {
                    TL.LogMessage(LogLevel.Debug, "ManageAlpacaDevicesClick", $"ProgID {selectedProgIdValue} is still registered - no action");
                }
            }
            else
            {
                TL.LogMessage(LogLevel.Debug, "ManageAlpacaDevicesClick", $"ProgID {selectedProgIdValue} was NOT registered - no action");
            }

            // Refresh the driver list after any changes made by the management tool
            InitialiseComboBox();

        }

        /// <summary>
        /// Creates a new Alpaca driver instance with the given descriptive name
        /// </summary>
        /// <param name="deviceDescription"></param>
        /// <returns></returns>
        private string CreateNewAlpacaDriver(string deviceDescription)
        {
            string newProgId;
            int deviceNumber;
            Type typeFromProgId;

            // Initialise to a starting value
            deviceNumber = 0;

            // Try successive ProgIDs until one is found that is not COM registered
            do
            {
                deviceNumber += 1; // Increment the device number
                newProgId = $"{DRIVER_PROGID_BASE}{deviceNumber}.{deviceTypeValue}"; // Create the new ProgID to be tested
                typeFromProgId = Type.GetTypeFromProgID(newProgId); // Try to get the type with the new ProgID
                TL.LogMessage(LogLevel.Debug, "CreateAlpacaClient", $"Testing ProgID: {newProgId} Type name: {typeFromProgId?.Name}");
            }
            while (typeFromProgId is not null); // Loop until the returned type is null indicating that this type is not COM registered

            TL.LogMessage(LogLevel.Debug, "CreateAlpacaClient", $"Creating new ProgID: {newProgId}");

            // Create the new Alpaca Client appending the device description if required 
            if (string.IsNullOrEmpty(deviceDescription))
            {
                RunDynamicClientManager($@"\CreateNamedClient {deviceTypeValue} {deviceNumber} {newProgId}");
            }
            else
            {
                RunDynamicClientManager($@"\CreateAlpacaClient {deviceTypeValue} {deviceNumber} {newProgId} ""{deviceDescription}""");
            }

            return newProgId; // Return the new ProgID
        }

        private void MnuCreateAlpacaDriver_Click(object sender, EventArgs e)
        {
            string newProgId;

            // Create a new Alpaca driver of the current ASCOM device type
            newProgId = CreateNewAlpacaDriver("");

            // Select the new driver in the Chooser combo box list
            selectedProgIdValue = newProgId;
            InitialiseComboBox();

            TL.LogMessage(LogLevel.Debug, "OK Click", $"Returning ProgID: '{selectedProgIdValue}'");

        }

        #endregion

        #region State Persistence

        private void ReadState()
        {
            ReadState("Telescope");
        }

        private void ReadState(string DeviceType)
        {
            try
            {
                TL?.LogMessage(LogLevel.Debug, "ChooserReadState", $"Reading state for device type: {DeviceType}. Configuration key: {CONFIGRATION_SUBKEY}, Alpaca enabled: {$"{DeviceType} {ALPACA_ENABLED}"}, ALapca default: {ALPACA_ENABLED_DEFAULT}");

                // The enabled state is per device type
                AlpacaEnabled = Convert.ToBoolean(registryAccess.GetProfile(CONFIGRATION_SUBKEY, $"{DeviceType} {ALPACA_ENABLED}", ALPACA_ENABLED_DEFAULT.ToString()), CultureInfo.InvariantCulture);

                // These values are for all Alpaca devices
                AlpacaDiscoveryPort = Convert.ToInt32(registryAccess.GetProfile(CONFIGRATION_SUBKEY, ALPACA_DISCOVERY_PORT, ALPACA_DISCOVERY_PORT_DEFAULT.ToString()), CultureInfo.InvariantCulture);
                AlpacaNumberOfBroadcasts = Convert.ToInt32(registryAccess.GetProfile(CONFIGRATION_SUBKEY, ALPACA_NUMBER_OF_BROADCASTS, ALPACA_NUMBER_OF_BROADCASTS_DEFAULT.ToString()), CultureInfo.InvariantCulture);
                AlpacaTimeout = Convert.ToInt32(registryAccess.GetProfile(CONFIGRATION_SUBKEY, ALPACA_TIMEOUT, ALPACA_TIMEOUT_DEFAULT.ToString()), CultureInfo.InvariantCulture);
                AlpacaDnsResolution = Convert.ToBoolean(registryAccess.GetProfile(CONFIGRATION_SUBKEY, ALPACA_DNS_RESOLUTION, ALPACA_DNS_RESOLUTION_DEFAULT.ToString()), CultureInfo.InvariantCulture);
                AlpacaShowDeviceDetails = Convert.ToBoolean(registryAccess.GetProfile(CONFIGRATION_SUBKEY, ALPACA_SHOW_DEVICE_DETAILS, ALPACA_SHOW_DEVICE_DETAILS_DEFAULT.ToString()), CultureInfo.InvariantCulture);
                AlpacaShowDiscoveredDevices = Convert.ToBoolean(registryAccess.GetProfile(CONFIGRATION_SUBKEY, ALPACA_SHOW_DISCOVERED_DEVICES, ALPACA_SHOW_DISCOVERED_DEVICES_DEFAULT.ToString()), CultureInfo.InvariantCulture);
                AlpacaChooserIncrementalWidth = Convert.ToInt32(registryAccess.GetProfile(CONFIGRATION_SUBKEY, ALPACA_CHOOSER_WIDTH, ALPACA_CHOOSER_WIDTH_DEFAULT.ToString()), CultureInfo.InvariantCulture);
                AlpacaUseIpV4 = Convert.ToBoolean(registryAccess.GetProfile(CONFIGRATION_SUBKEY, ALPACA_USE_IPV4, ALPACA_USE_IPV4_DEFAULT.ToString()), CultureInfo.InvariantCulture);
                AlpacaUseIpV6 = Convert.ToBoolean(registryAccess.GetProfile(CONFIGRATION_SUBKEY, ALPACA_USE_IPV6, ALPACA_USE_IPV6_DEFAULT.ToString()), CultureInfo.InvariantCulture);
                AlpacaMultiThreadedChooser = Convert.ToBoolean(registryAccess.GetProfile(CONFIGRATION_SUBKEY, ALPACA_MULTI_THREADED_CHOOSER, ALPACA_MULTI_THREADED_CHOOSER_DEFAULT.ToString()), CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chooser Read State " + ex.ToString());
                EventLogCode.LogEvent("Chooser Read State ", ex.ToString(), EventLogEntryType.Error, GlobalConstants.EventLogErrors.ChooserFormLoad, ex.ToString());
                TL?.LogMessage(LogLevel.Debug, "ChooserReadState", ex.ToString());
            }
        }

        private void WriteState(string DeviceType)
        {

            try
            {

                // Save the enabled state per "device type" 
                registryAccess.WriteProfile(CONFIGRATION_SUBKEY, $"{DeviceType} {ALPACA_ENABLED}", AlpacaEnabled.ToString(CultureInfo.InvariantCulture));

                // Save other states for all Alpaca devices 
                registryAccess.WriteProfile(CONFIGRATION_SUBKEY, ALPACA_DISCOVERY_PORT, AlpacaDiscoveryPort.ToString(CultureInfo.InvariantCulture));
                registryAccess.WriteProfile(CONFIGRATION_SUBKEY, ALPACA_NUMBER_OF_BROADCASTS, AlpacaNumberOfBroadcasts.ToString(CultureInfo.InvariantCulture));
                registryAccess.WriteProfile(CONFIGRATION_SUBKEY, ALPACA_TIMEOUT, AlpacaTimeout.ToString(CultureInfo.InvariantCulture));
                registryAccess.WriteProfile(CONFIGRATION_SUBKEY, ALPACA_DNS_RESOLUTION, AlpacaDnsResolution.ToString(CultureInfo.InvariantCulture));
                registryAccess.WriteProfile(CONFIGRATION_SUBKEY, ALPACA_SHOW_DEVICE_DETAILS, AlpacaShowDeviceDetails.ToString(CultureInfo.InvariantCulture));
                registryAccess.WriteProfile(CONFIGRATION_SUBKEY, ALPACA_SHOW_DISCOVERED_DEVICES, AlpacaShowDiscoveredDevices.ToString(CultureInfo.InvariantCulture));
                registryAccess.WriteProfile(CONFIGRATION_SUBKEY, ALPACA_CHOOSER_WIDTH, AlpacaChooserIncrementalWidth.ToString(CultureInfo.InvariantCulture));
                registryAccess.WriteProfile(CONFIGRATION_SUBKEY, ALPACA_USE_IPV4, AlpacaUseIpV4.ToString(CultureInfo.InvariantCulture));
                registryAccess.WriteProfile(CONFIGRATION_SUBKEY, ALPACA_USE_IPV6, AlpacaUseIpV6.ToString(CultureInfo.InvariantCulture));
                registryAccess.WriteProfile(CONFIGRATION_SUBKEY, ALPACA_MULTI_THREADED_CHOOSER, AlpacaMultiThreadedChooser.ToString(CultureInfo.InvariantCulture));
            }

            catch (Exception ex)
            {
                MessageBox.Show("Chooser Write State " + ex.ToString());
                EventLogCode.LogEvent("Chooser Write State ", ex.ToString(), EventLogEntryType.Error, GlobalConstants.EventLogErrors.ChooserFormLoad, ex.ToString());
                TL?.LogMessage(LogLevel.Debug, "ChooserWriteState", ex.ToString());
            }

        }

        #endregion

        #region Support code

        /// <summary>
        /// Run the Alpaca dynamic client manager application with the supplied parameters
        /// </summary>
        /// <param name="parameterString">Parameter string to pass to the application</param>
        private void RunDynamicClientManager(string parameterString)
        {
            string clientManagerWorkingDirectory, clientManagerExeFile;
            ProcessStartInfo clientManagerProcessStartInfo;

            // Construct path to the executable that will dynamically create a new Alpaca COM client
            clientManagerWorkingDirectory = $@"{Get32BitProgramFilesPath()}\{ALPACA_DYNAMIC_CLIENT_MANAGER_RELATIVE_PATH}";
            clientManagerExeFile = $@"{clientManagerWorkingDirectory}\{ALPACA_DYNAMIC_CLIENT_MANAGER_EXE_NAME}";

            TL.LogMessage(LogLevel.Debug, "RunDynamicClientManager", $"Generator parameters: '{parameterString}'");
            TL.LogMessage(LogLevel.Debug, "RunDynamicClientManager", $"Managing drivers using the {clientManagerExeFile} executable in working directory {clientManagerWorkingDirectory}");

            if (!File.Exists(clientManagerExeFile))
            {
                MessageBox.Show("The client generator executable can not be found, please repair the ASCOM Platform.", "Alpaca Client Generator Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                TL.LogMessage(LogLevel.Debug, "RunDynamicClientManager", $"ERROR - Unable to find the client generator executable at {clientManagerExeFile}, cannot create a new Alpaca client.");
                selectedProgIdValue = "";
                return;
            }

            // Set the process run time environment and parameters
            clientManagerProcessStartInfo = new ProcessStartInfo(clientManagerExeFile, parameterString)
            {
                WorkingDirectory = clientManagerWorkingDirectory,

                // Make the application request elevation. For some reason this wasn't required in the .NET Framework version.
                UseShellExecute = true,
                Verb = "runas"
            }; // Run the executable with no parameters in order to show the management GUI

            // Create the management process
            ClientManagerProcess = new Process
            {
                StartInfo = clientManagerProcessStartInfo,
                EnableRaisingEvents = true
            };

            // Initialise the process complete flag to false
            driverGenerationComplete = false;

            // Run the process
            TL.LogMessage(LogLevel.Debug, "RunDynamicClientManager", $"Starting driver management process");
            ClientManagerProcess.Start();

            // Wait for the process to complete at which point the process complete event will fire and driverGenerationComplete will be set true
            do
            {
                Thread.Sleep(10);
                Application.DoEvents();
            }
            while (!driverGenerationComplete);

            TL.LogMessage(LogLevel.Debug, "RunDynamicClientManager", $"Completed driver management process");

            ClientManagerProcess.Dispose();

        }

        /// <summary>
        /// Get the 32bit ProgramFiles path on both 32bit and 64bit systems
        /// </summary>
        /// <returns></returns>
        private string Get32BitProgramFilesPath()
        {
            // Try to get the 64bit path
            string returnValue = Environment.GetEnvironmentVariable("ProgramFiles(x86)");

            // If no path is returned get the 32bit path
            if (string.IsNullOrEmpty(returnValue))
            {
                returnValue = Environment.GetEnvironmentVariable("ProgramFiles");
            }

            TL.LogMessage(LogLevel.Debug, "Get32BitProgramFilesPath", $"Returned path: {returnValue}");
            return returnValue;
        }

        private void InitialiseComboBox()
        {

            TL.LogMessage(LogLevel.Debug, "InitialiseComboBox", $"Arrived at InitialiseComboBox - Running On thread: {Environment.CurrentManagedThreadId}.");

            if (AlpacaMultiThreadedChooser) // Multi-threading behaviour where the Chooser UI is displayed immediately while discovery runs in the background
            {
                TL.LogMessage(LogLevel.Debug, "InitialiseComboBox", $"Creating discovery thread...");
                var discoveryThread = new Thread(DiscoverAlpacaDevicesAndPopulateDriverComboBox);
                TL.LogMessage(LogLevel.Debug, "InitialiseComboBox", $"Successfully created discovery thread, about to start discovery on thread {discoveryThread.ManagedThreadId}...");
                discoveryThread.Start();
                TL.LogMessage(LogLevel.Debug, "InitialiseComboBox", $"Discovery thread started OK");
            }
            else // Single threaded behaviour where the Chooser UI is not displayed until discovery completes
            {
                TL.LogMessage(LogLevel.Debug, "InitialiseComboBox", $"Starting single threaded discovery...");
                DiscoverAlpacaDevicesAndPopulateDriverComboBox();
                TL.LogMessage(LogLevel.Debug, "InitialiseComboBox", $"Completed single threaded discovery");
            }

            TL.LogMessage(LogLevel.Debug, "InitialiseComboBox", $"Exiting InitialiseComboBox on thread: {Environment.CurrentManagedThreadId}.");
        }

        private void DiscoverAlpacaDevicesAndPopulateDriverComboBox()
        {
            try
            {

                TL.LogMessage(LogLevel.Debug, "DiscoverAlpacaDevices", $"Running On thread: {Environment.CurrentManagedThreadId}.");

                chooserList = new List<ChooserItem>();

                // Enumerate the available drivers, and load their descriptions and ProgIDs into the driversList generic sorted list collection. Key is ProgID, value is friendly name.
                try
                {
                    // Get Key-Class pairs in the subkey "{DeviceType} Drivers" e.g. "Telescope Drivers"
                    var driverList = registryAccess.EnumKeys(deviceTypeValue + " Drivers");
                    TL.LogMessage(LogLevel.Debug, "DiscoverAlpacaDevices", $"Returned {driverList.Count} COM drivers");

                    foreach (KeyValuePair<string, string> driver in driverList)
                    {
                        string driverProgId, driverName;

                        driverProgId = driver.Key;
                        driverName = driver.Value;

                        TL.LogMessage(LogLevel.Debug, "PopulateDriverComboBox", $"Found ProgID: {driverProgId} , Description: '{driverName}'");

                        if (string.IsNullOrEmpty(driverName)) // Description Is missing
                        {
                            TL.LogMessage(LogLevel.Debug, "PopulateDriverComboBox", $"  ***** Description missing for ProgID: {driverProgId}");
                        }

                        // Annotate the device description as configured
                        if (driverProgId.ToLowerInvariant().StartsWith(DRIVER_PROGID_BASE.ToLowerInvariant())) // This is a COM driver for an Alpaca device
                        {
                            if (AlpacaShowDeviceDetails) // Get device details from the Profile and display these
                            {
                                driverName = $"{driverName}    ({driverProgId} ==> {Profile.GetValue(Devices.StringToDeviceType(deviceTypeValue), driverProgId, PROFILE_VALUE_NAME_IP_ADDRESS, null)}:" + $"{Profile.GetValue(Devices.StringToDeviceType(deviceTypeValue), driverProgId, PROFILE_VALUE_NAME_PORT_NUMBER, null)}/api/v1/{deviceTypeValue}/{Profile.GetValue(Devices.StringToDeviceType(deviceTypeValue), driverProgId, PROFILE_VALUE_NAME_REMOTE_DEVICER_NUMBER, null)}" + $") - {Profile.GetValue(Devices.StringToDeviceType(deviceTypeValue), driverProgId, PROFILE_VALUE_NAME_UNIQUEID, "")}"; // Annotate as Alpaca Dynamic driver to differentiate from other COM drivers
                            }
                            else // Just annotate as an Alpaca device
                            {
                                driverName = $"{driverName}    (Alpaca)";
                            } // Annotate as an Alpaca device
                        }
                        else if (AlpacaShowDeviceDetails) // This is not an Alpaca COM driver
                                                          // Get device details from the Profile and display these
                        {
                            driverName = $"{driverName}    ({driverProgId})"; // Annotate with ProgID
                        }
                        else
                        {
                            // No action just use the driver description as is
                        }

                        chooserList.Add(new ChooserItem(driverProgId, driverName));
                    }
                }

                catch (Exception ex1)
                {
                    TL.LogMessage(LogLevel.Debug, "DiscoverAlpacaDevices", "Exception: " + ex1.ToString());
                    // Ignore any exceptions from this call e.g. if there are no devices of that type installed just create an empty list
                }

                TL.LogMessage(LogLevel.Debug, "DiscoverAlpacaDevices", $"Completed COM driver enumeration");

                if (AlpacaEnabled)
                {
                    alpacaDevices = new List<AscomDevice>(); // Initialise to a clear list with no Alpaca devices

                    // Render the user interface unresponsive while discovery is underway, except for the Cancel button.
                    SetStateAlpacaDiscovering();

                    // Initiate discovery and wait for it to complete
                    using (var discovery = new AlpacaDiscovery(true, TL, nameof(ChooserSA), this.GetType().Assembly.GetName().Version.ToString()))
                    {
                        TL.LogMessage(LogLevel.Debug, "DiscoverAlpacaDevices", $"AlpacaDiscovery created");
                        discovery.StartDiscovery(AlpacaNumberOfBroadcasts, 200, AlpacaDiscoveryPort, AlpacaTimeout, AlpacaDnsResolution, AlpacaUseIpV4, AlpacaUseIpV6);
                        TL.LogMessage(LogLevel.Debug, "DiscoverAlpacaDevices", $"AlpacaDiscovery started");

                        // Keep the UI alive while the discovery is running
                        do
                        {
                            Thread.Sleep(10);
                            Application.DoEvents();
                        }
                        while (!discovery.DiscoveryComplete);
                        TL.LogMessage(LogLevel.Debug, "DiscoverAlpacaDevices", $"Discovery phase has finished");

                        TL.LogMessage(LogLevel.Debug, "DiscoverAlpacaDevices", $"Discovered {discovery.GetAscomDevices(null).Count} devices");

                        // List discovered devices to the log
                        foreach (AscomDevice ascomDevice in discovery.GetAscomDevices(null))
                            TL.LogMessage(LogLevel.Debug, "DiscoverAlpacaDevices", $"FOUND {ascomDevice.AscomDeviceType} {ascomDevice.AscomDeviceName}");

                        TL.LogMessage(LogLevel.Debug, "DiscoverAlpacaDevices", $"Discovered {discovery.GetAscomDevices(deviceTypeValue.ToDeviceType()).Count} {deviceTypeValue} devices");

                        // Get discovered devices of the requested ASCOM device type
                        alpacaDevices = discovery.GetAscomDevices(deviceTypeValue.ToDeviceType());
                    }

                    // Add any Alpaca devices to the list
                    foreach (AscomDevice device in alpacaDevices)
                    {
                        TL.LogMessage(LogLevel.Debug, "DiscoverAlpacaDevices", $"Discovered Alpaca device: {device.AscomDeviceType} {device.AscomDeviceName} {device.UniqueId} at  http://{device.HostName}:{device.IpPort}/api/v1/{deviceTypeValue}/{device.AlpacaDeviceNumber}");

                        string displayHostName = (device.HostName ?? "") == (device.IpAddress ?? "") ? device.IpAddress : $"{device.HostName} ({device.IpAddress})";
                        string displayName;

                        string deviceUniqueId, deviceHostName;
                        int deviceIPPort, deviceNumber;

                        // Get a list of dynamic drivers already configured on the system
                        bool foundDriver = false;

                        foreach (ASCOMRegistration arrayListDevice in Profile.GetDrivers(Devices.StringToDeviceType(deviceTypeValue))) // Iterate over a list of all devices of the current device type
                        {
                            if (arrayListDevice.ProgID.ToLowerInvariant().StartsWith(DRIVER_PROGID_BASE.ToLowerInvariant())) // This is a dynamic Alpaca COM driver
                            {

                                // Get and validate the device values to compare with the discovered device
                                try
                                {
                                    deviceUniqueId = Profile.GetValue(Devices.StringToDeviceType(deviceTypeValue), arrayListDevice.ProgID, PROFILE_VALUE_NAME_UNIQUEID, "");
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show($"{arrayListDevice.ProgID} - Unable to read the device unique ID. This driver should be deleted and re-created", "Dynamic Driver Corrupted", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    continue; // Don't process this driver further, move on to the next driver
                                }

                                try
                                {
                                    deviceHostName = Profile.GetValue(Devices.StringToDeviceType(deviceTypeValue), arrayListDevice.ProgID, PROFILE_VALUE_NAME_IP_ADDRESS, "");
                                    if (string.IsNullOrEmpty(deviceHostName))
                                    {
                                        MessageBox.Show($"{arrayListDevice.ProgID} - The device IP address is blank. This driver should be deleted and re-created", "Dynamic Driver Corrupted", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        continue; // Don't process this driver further, move on to the next driver
                                    }
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show($"{arrayListDevice.ProgID} - Unable to read the device IP address. This driver should be deleted and re-created", "Dynamic Driver Corrupted", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    continue; // Don't process this driver further, move on to the next driver
                                }

                                try
                                {
                                    deviceIPPort = Convert.ToInt32(Profile.GetValue(Devices.StringToDeviceType(deviceTypeValue), arrayListDevice.ProgID, PROFILE_VALUE_NAME_PORT_NUMBER, ""));
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show($"{arrayListDevice.ProgID} - Unable to read the device IP Port. This driver should be deleted and re-created", "Dynamic Driver Corrupted", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    continue; // Don't process this driver further, move on to the next driver
                                }

                                try
                                {
                                    deviceNumber = Convert.ToInt32(Profile.GetValue(Devices.StringToDeviceType(deviceTypeValue), arrayListDevice.ProgID, PROFILE_VALUE_NAME_REMOTE_DEVICER_NUMBER, ""));
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show($"{arrayListDevice.ProgID} - Unable to read the device number. This driver should be deleted and re-created", "Dynamic Driver Corrupted", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    continue; // Don't process this driver further, move on to the next driver
                                }

                                TL.LogMessage(LogLevel.Debug, "DiscoverAlpacaDevices", $"Found existing COM dynamic driver for device {deviceUniqueId} at http://{deviceHostName}:{deviceIPPort}/api/v1/{deviceTypeValue}/{deviceNumber}");
                                TL.LogMessage(LogLevel.Debug, "DiscoverAlpacaDevices", $"{device.UniqueId} {deviceUniqueId} {(device.UniqueId ?? "") == (deviceUniqueId ?? "")} {(device.HostName ?? "") == (deviceHostName ?? "")} {device.IpPort == deviceIPPort} {device.AlpacaDeviceNumber == deviceNumber}");

                                if ((device.UniqueId ?? "") == (deviceUniqueId ?? "") & (device.HostName ?? "") == (deviceHostName ?? "") & device.IpPort == deviceIPPort & device.AlpacaDeviceNumber == deviceNumber)
                                {
                                    foundDriver = true;
                                    TL.LogMessage(LogLevel.Debug, "DiscoverAlpacaDevices", $"    Found existing COM driver match!");
                                }
                            }
                        }

                        if (foundDriver)
                        {
                            TL.LogMessage(LogLevel.Debug, "DiscoverAlpacaDevices", $"Found driver match for {device.AscomDeviceName}");
                            if (AlpacaShowDiscoveredDevices)
                            {
                                TL.LogMessage(LogLevel.Debug, "DiscoverAlpacaDevices", $"Showing KNOWN ALPACA DEVICE entry for {device.AscomDeviceName}");
                                displayName = $"* KNOWN ALPACA DEVICE   {device.AscomDeviceName}   {displayHostName}:{device.IpPort}/api/v1/{deviceTypeValue}/{device.AlpacaDeviceNumber} - {device.UniqueId}";
                                chooserList.Add(new ChooserItem(device.UniqueId, device.AlpacaDeviceNumber, device.HostName, device.IpPort, device.AscomDeviceName, displayName));
                            }
                            else
                            {
                                TL.LogMessage(LogLevel.Debug, "DiscoverAlpacaDevices", $"This device MATCHES an existing COM driver so NOT adding it to the Combo box list");
                            }
                        }

                        else
                        {
                            TL.LogMessage(LogLevel.Debug, "DiscoverAlpacaDevices", $"This device does NOT match an existing COM driver so ADDING it to the Combo box list");
                            displayName = $"* NEW ALPACA DEVICE   {device.AscomDeviceName}   {displayHostName}:{device.IpPort}/api/v1/{deviceTypeValue}/{device.AlpacaDeviceNumber} - {device.UniqueId}";
                            chooserList.Add(new ChooserItem(device.UniqueId, device.AlpacaDeviceNumber, device.HostName, device.IpPort, device.AscomDeviceName, displayName));
                        }

                    }
                }

                // List the ChooserList contents
                TL.LogMessage(LogLevel.Debug, "DiscoverAlpacaDevices", $"Start of Chooser List");
                foreach (ChooserItem item in chooserList)
                    TL.LogMessage(LogLevel.Debug, "DiscoverAlpacaDevices", $"List includes device {item.AscomName}");
                TL.LogMessage(LogLevel.Debug, "DiscoverAlpacaDevices", $"End of Chooser List");

                // Populate the device list combo box with COM and Alpaca devices.
                // This Is implemented as an independent method because it interacts with UI controls And will self invoke if required
                PopulateDriverComboBox();
            }

            catch (Exception ex)
            {
                TL.LogMessage(LogLevel.Debug, "DiscoverAlpacaDevices", ex.ToString());
            }
            finally
            {
                // Restore a usable user interface
                if (AlpacaEnabled)
                {
                    if (alpacaDevices.Count > 0)
                    {
                        SetStateAlpacaDiscoveryCompleteFoundDevices();
                    }
                    else
                    {
                        SetStateAlpacaDiscoveryCompleteNoDevices();
                    }
                }
                else
                {
                    SetStateNoAlpaca();
                }
                DisplayAlpacaDeviceToolTip();
            }
        }

        private void PopulateDriverComboBox()
        {
            // Only proceed if there are drivers or Alpaca devices to display
            if (chooserList.Count == 0 & alpacaDevices.Count == 0) // No drivers to add to the combo box 
            {
                MessageBox.Show("There are no ASCOM " + deviceTypeValue + " drivers installed.", ALERT_MESSAGEBOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (CmbDriverSelector.InvokeRequired) // We are not running on the UI thread
            {
                TL.LogMessage(LogLevel.Debug, "PopulateDriverComboBox", $"InvokeRequired from thread {Environment.CurrentManagedThreadId}");
                CmbDriverSelector.Invoke(PopulateDriverComboBoxDelegate);
            }
            else // We are running on the UI thread
            {
                try
                {
                    TL.LogMessage(LogLevel.Debug, "PopulateDriverComboBox", $"Running on thread: {Environment.CurrentManagedThreadId}");

                    // Clear the combo box list, sort the discovered drivers / devices and add them to the Chooser's combo box list
                    CmbDriverSelector.Items.Clear(); // Clear the combo box list
                    CmbDriverSelector.SelectedIndex = -1;
                    chooserList.Sort(); // Sort the ChooserItem instances into alphabetical order
                    CmbDriverSelector.Items.AddRange(chooserList.ToArray()); // Add the ChooserItem instances to the combo box
                    CmbDriverSelector.DisplayMember = "DisplayName"; // Set the name of the ChooserItem property whose contents should be displayed in the drop-down list

                    CmbDriverSelector.DropDownWidth = DropDownWidth(CmbDriverSelector); // AutoSize the combo box width

                    // If a ProgID has been provided, test whether it matches a ProgID in the driver list
                    if (!string.IsNullOrEmpty(selectedProgIdValue)) // A progID was provided
                    {

                        // Select the current device in the list
                        foreach (ChooserItem driver in CmbDriverSelector.Items)
                        {
                            TL.LogMessage(LogLevel.Debug, "PopulateDriverComboBox", $"Searching for ProgID: {selectedProgIdValue}, found ProgID: {driver.ProgID}");
                            if ((driver.ProgID.ToLowerInvariant() ?? "") == (selectedProgIdValue.ToLowerInvariant() ?? ""))
                            {
                                TL.LogMessage(LogLevel.Debug, "PopulateDriverComboBox", $"*** Found ProgID: {selectedProgIdValue}");
                                CmbDriverSelector.SelectedItem = driver;
                                selectedChooserItem = driver;
                                EnableOkButton(true); // Enable the OK button
                            }
                        }
                    }

                    if (selectedChooserItem is null) // The requested driver was not found so display a blank Chooser item
                    {
                        TL.LogMessage(LogLevel.Debug, "PopulateDriverComboBox", $"Selected ProgID {selectedProgIdValue} WAS NOT found, displaying a blank combo list item");

                        CmbDriverSelector.ResetText();
                        CmbDriverSelector.SelectedIndex = -1;

                        EnablePropertiesButton(false);
                        EnableOkButton(false);
                    }
                    else
                    {
                        TL.LogMessage(LogLevel.Debug, "PopulateDriverComboBox", $"Selected ProgID {selectedProgIdValue} WAS found. Device is: {selectedChooserItem.AscomName}, Is COM driver: {selectedChooserItem.IsComDriver}");

                        // Validate the selected driver if it is a COM driver
                        if (selectedChooserItem.IsComDriver) // This is a COM driver so validate that it is functional
                        {
                            ValidateDriver(selectedChooserItem.ProgID);
                        }
                        else // This is a new Alpaca driver
                        {
                            WarningTooltipClear();
                            EnablePropertiesButton(false); // Disable the Properties button because there is not yet a COM driver to configure
                            EnableOkButton(true);

                        }
                    }
                }

                catch (Exception ex)
                {
                    TL.LogMessage(LogLevel.Debug, "PopulateDriverComboBox Top", "Exception: " + ex.ToString());
                }
            }
        }

        /// <summary>
        /// Return the maximum width of a combo box's drop-down items
        /// </summary>
        /// <param name="comboBox">Combo box to inspect</param>
        /// <returns>Maximum width of supplied combo box drop-down items</returns>
        private int DropDownWidth(ComboBox comboBox)
        {
            int maxWidth;
            int temp;
            var label1 = new Label();

            maxWidth = comboBox.Width; // Ensure that the minimum width is the width of the combo box
            TL.LogMessage(LogLevel.Debug, "DropDownWidth", $"Combo box: {comboBox.Name} Number of items: {comboBox.Items.Count} ");

            foreach (ChooserItem obj in comboBox.Items)
            {
                label1.Text = obj.AscomName;
                temp = label1.PreferredWidth;

                if (temp > maxWidth)
                {
                    maxWidth = temp;
                }
            }

            label1.Dispose();

            return maxWidth;
        }

        private void SetStateNoAlpaca()
        {
            if (CmbDriverSelector.InvokeRequired)
            {
                TL.LogMessage(LogLevel.Debug, "SetStateNoAlpaca", $"InvokeRequired from thread {Environment.CurrentManagedThreadId}");
                CmbDriverSelector.Invoke(SetStateNoAlpacaDelegate);
            }
            else
            {
                TL.LogMessage(LogLevel.Debug, "SetStateNoAlpaca", $"Running on thread {Environment.CurrentManagedThreadId}");

                LblAlpacaDiscovery.Visible = false;
                CmbDriverSelector.Enabled = true;
                alpacaStatusToolstripLabel.Text = "Discovery Disabled";
                alpacaStatusToolstripLabel.BackColor = Color.Salmon;
                MnuDiscoverNow.Enabled = false;
                MnuEnableDiscovery.Enabled = true;
                MnuDisableDiscovery.Enabled = false;
                MnuConfigureChooser.Enabled = true;
                BtnProperties.Enabled = currentPropertiesButtonEnabledState;
                BtnOK.Enabled = currentOkButtonEnabledState;
                AlpacaStatus.Visible = false;
                AlpacaStatusIndicatorTimer.Stop();
            }
        }

        private void SetStateAlpacaDiscovering()
        {
            if (CmbDriverSelector.InvokeRequired)
            {
                TL.LogMessage(LogLevel.Debug, "SetStateAlpacaDiscovering", $"InvokeRequired from thread {Environment.CurrentManagedThreadId}");
                CmbDriverSelector.Invoke(SetStateAlpacaDiscoveringDelegate);
            }
            else
            {
                TL.LogMessage(LogLevel.Debug, "SetStateAlpacaDiscovering", $"Running on thread {Environment.CurrentManagedThreadId} OK button enabled state: {currentOkButtonEnabledState}");
                LblAlpacaDiscovery.Visible = true;
                CmbDriverSelector.Enabled = false;
                alpacaStatusToolstripLabel.Text = "Discovery Enabled";
                alpacaStatusToolstripLabel.BackColor = Color.LightGreen;
                MnuDiscoverNow.Enabled = false;
                MnuEnableDiscovery.Enabled = false;
                MnuDisableDiscovery.Enabled = false;
                MnuConfigureChooser.Enabled = false;
                BtnProperties.Enabled = false;
                BtnOK.Enabled = false;
                AlpacaStatus.Visible = true;
                AlpacaStatus.BackColor = Color.Orange;
                AlpacaStatusIndicatorTimer.Start();
            }
        }

        private void SetStateAlpacaDiscoveryCompleteFoundDevices()
        {
            if (CmbDriverSelector.InvokeRequired)
            {
                TL.LogMessage(LogLevel.Debug, "SetStateAlpacaDiscoveryCompleteFoundDevices", $"InvokeRequired from thread {Environment.CurrentManagedThreadId}");
                CmbDriverSelector.Invoke(SetStateAlpacaDiscoveryCompleteFoundDevicesDelegate);
            }
            else
            {
                TL.LogMessage(LogLevel.Debug, "SetStateAlpacaDiscoveryCompleteFoundDevices", $"Running on thread {Environment.CurrentManagedThreadId}");
                LblAlpacaDiscovery.Visible = true;
                alpacaStatusToolstripLabel.Text = "Discovery Enabled";
                alpacaStatusToolstripLabel.BackColor = Color.LightGreen;
                CmbDriverSelector.Enabled = true;
                MnuDiscoverNow.Enabled = true;
                MnuEnableDiscovery.Enabled = false;
                MnuDisableDiscovery.Enabled = true;
                MnuConfigureChooser.Enabled = true;
                BtnProperties.Enabled = currentPropertiesButtonEnabledState;
                BtnOK.Enabled = currentOkButtonEnabledState;
                AlpacaStatus.Visible = true;
                AlpacaStatus.BackColor = Color.Lime;
                AlpacaStatusIndicatorTimer.Stop();
            }
        }

        private void SetStateAlpacaDiscoveryCompleteNoDevices()
        {
            if (CmbDriverSelector.InvokeRequired)
            {
                TL.LogMessage(LogLevel.Debug, "SetStateAlpacaDiscoveryCompleteNoDevices", $"InvokeRequired from thread {Environment.CurrentManagedThreadId}");
                CmbDriverSelector.Invoke(SetStateAlpacaDiscoveryCompleteNoDevicesDelegate);
            }
            else
            {
                TL.LogMessage(LogLevel.Debug, "SetStateAlpacaDiscoveryCompleteNoDevices", $"Running on thread {Environment.CurrentManagedThreadId}");
                LblAlpacaDiscovery.Visible = true;
                alpacaStatusToolstripLabel.Text = "Discovery Enabled";
                alpacaStatusToolstripLabel.BackColor = Color.LightGreen;
                CmbDriverSelector.Enabled = true;
                MnuDiscoverNow.Enabled = true;
                MnuEnableDiscovery.Enabled = false;
                MnuDisableDiscovery.Enabled = true;
                MnuConfigureChooser.Enabled = true;
                BtnProperties.Enabled = currentPropertiesButtonEnabledState;
                BtnOK.Enabled = currentOkButtonEnabledState;
                AlpacaStatus.Visible = true;
                AlpacaStatus.BackColor = Color.Red;
                AlpacaStatusIndicatorTimer.Stop();
            }
        }

        private void ValidateDriver(string progId)
        {
            string deviceInitialised;

            if (!string.IsNullOrEmpty(progId))
            {

                if (!string.IsNullOrEmpty(progId)) // Something selected
                {

                    WarningTooltipClear(); // Hide any previous message

                    TL.LogMessage(LogLevel.Debug, "ValidateDriver", "ProgID:" + progId + ", Bitness: " + (Environment.Is64BitProcess ? "64bit" : "32bit"));
                    driverIsCompatible = VersionCode.DriverCompatibilityMessage(progId, VersionCode.ApplicationBits(), TL); // Get compatibility warning message, if any

                    if (!string.IsNullOrEmpty(driverIsCompatible)) // This is an incompatible driver so we need to prevent access
                    {
                        EnablePropertiesButton(false);
                        EnableOkButton(false);
                        TL.LogMessage(LogLevel.Debug, "ValidateDriver", "Showing incompatible driver message");
                        WarningToolTipShow("Incompatible Driver (" + progId + ")", driverIsCompatible);
                    }
                    else // This is a compatible driver
                    {
                        EnablePropertiesButton(true); // Turn on Properties
                        deviceInitialised = registryAccess.GetProfile("Chooser", progId + " Init");
                        if (deviceInitialised.ToLowerInvariant() == "true") // This device has been initialized
                        {
                            EnableOkButton(true);
                            currentWarningMesage = "";
                            TL.LogMessage(LogLevel.Debug, "ValidateDriver", "Driver is compatible and configured so no message");
                        }
                        else // This device has not been initialised
                        {
                            selectedProgIdValue = "";
                            EnableOkButton(false); // Ensure OK is disabled
                            TL.LogMessage(LogLevel.Debug, "ValidateDriver", "Showing first time configuration required message");
                            WarningToolTipShow(TOOLTIP_PROPERTIES_TITLE, TOOLTIP_PROPERTIES_FIRST_TIME_MESSAGE);
                        }
                    }
                }
                else // Nothing has been selected
                {
                    TL.LogMessage(LogLevel.Debug, "ValidateDriver", "Nothing has been selected");
                    selectedProgIdValue = "";
                    EnablePropertiesButton(false);
                    EnableOkButton(false);
                } // Ensure OK is disabled
            }

        }

        private void WarningToolTipShow(string Title, string Message)
        {
            const int MESSAGE_X_POSITION = 120; // Was 18

            WarningTooltipClear();
            chooserWarningToolTip.UseAnimation = true;
            chooserWarningToolTip.UseFading = false;
            chooserWarningToolTip.ToolTipIcon = ToolTipIcon.Warning;
            chooserWarningToolTip.AutoPopDelay = 5000;
            chooserWarningToolTip.InitialDelay = 0;
            chooserWarningToolTip.IsBalloon = false;
            chooserWarningToolTip.ReshowDelay = 0;
            chooserWarningToolTip.OwnerDraw = false;
            chooserWarningToolTip.ToolTipTitle = Title;
            currentWarningTitle = Title;
            currentWarningMesage = Message;

            if (Message.Contains("\r\n"))
            {
                chooserWarningToolTip.Show(Message, this, MESSAGE_X_POSITION, 24); // Display at position for a two line message
            }
            else
            {
                chooserWarningToolTip.Show(Message, this, MESSAGE_X_POSITION, 50);
            } // Display at position for a one line message
        }

        private delegate void NoParameterDelegate();

        private readonly NoParameterDelegate displayCreateAlpacDeviceTooltip;

        private void DisplayAlpacaDeviceToolTip()
        {
            ChooserItem selectedItem;

            // Only consider displaying the tooltip if it has been instantiated
            if (createAlpacaDeviceToolTip is not null)
            {

                // Only display the tooltip if Alpaca discovery is enabled and the Alpaca dialogues have NOT been suppressed
                if (AlpacaEnabled & !RegistryCommonCode.GetBool(GlobalConstants.SUPPRESS_ALPACA_DRIVER_ADMIN_DIALOGUE, GlobalConstants.SUPPRESS_ALPACA_DRIVER_ADMIN_DIALOGUE_DEFAULT))
                {

                    // The tooltip code must be executed by the UI thread so invoke this if required
                    if (BtnOK.InvokeRequired)
                    {
                        TL.LogMessage(LogLevel.Debug, "DisplayAlpacaDeviceToolTip", $"Invoke required on thread {Environment.CurrentManagedThreadId}");
                        BtnOK.Invoke(displayCreateAlpacDeviceTooltip);
                    }
                    // Only display the tooltip if a device has been selected
                    else if (CmbDriverSelector.SelectedItem is not null)
                    {
                        selectedItem = (ChooserItem)CmbDriverSelector.SelectedItem;

                        // Only display the tooltip if the an Alpaca driver has been selected
                        if (!selectedItem.IsComDriver)
                        {

                            createAlpacaDeviceToolTip.RemoveAll();

                            createAlpacaDeviceToolTip.UseAnimation = true;
                            createAlpacaDeviceToolTip.UseFading = false;
                            createAlpacaDeviceToolTip.ToolTipIcon = ToolTipIcon.Info;
                            createAlpacaDeviceToolTip.AutoPopDelay = 5000;
                            createAlpacaDeviceToolTip.InitialDelay = 0;
                            createAlpacaDeviceToolTip.IsBalloon = true;
                            createAlpacaDeviceToolTip.ReshowDelay = 0;
                            createAlpacaDeviceToolTip.OwnerDraw = false;
                            createAlpacaDeviceToolTip.ToolTipTitle = TOOLTIP_CREATE_ALPACA_DEVICE_TITLE;

                            createAlpacaDeviceToolTip.Show(TOOLTIP_CREATE_ALPACA_DEVICE_MESSAGE, BtnOK, 45, -60, TOOLTIP_CREATE_ALPACA_DEVICE_DISPLAYTIME * 1000); // Display at position for a two line message
                            TL.LogMessage(LogLevel.Debug, "DisplayAlpacaDeviceToolTip", $"Set tooltip on thread {Environment.CurrentManagedThreadId}");
                        }

                    }
                }
            }
        }

        private void WarningTooltipClear()
        {
            chooserWarningToolTip.RemoveAll();
            currentWarningTitle = "";
            currentWarningMesage = "";
        }


        private void ResizeChooser()
        {
            // Position controls if the Chooser has an increased width
            Width = OriginalForm1Width + AlpacaChooserIncrementalWidth;
            BtnCancel.Location = new Point(OriginalBtnCancelPosition.X + AlpacaChooserIncrementalWidth, OriginalBtnCancelPosition.Y);
            BtnOK.Location = new Point(OriginalBtnOKPosition.X + AlpacaChooserIncrementalWidth, OriginalBtnOKPosition.Y);
            BtnProperties.Location = new Point(OriginalBtnPropertiesPosition.X + AlpacaChooserIncrementalWidth, OriginalBtnPropertiesPosition.Y);
            CmbDriverSelector.Width = OriginalCmbDriverSelectorWidth + AlpacaChooserIncrementalWidth;
            LblAlpacaDiscovery.Left = OriginalLblAlpacaDiscoveryPosition + AlpacaChooserIncrementalWidth;
            AlpacaStatus.Left = OriginalAlpacaStatusPosition + AlpacaChooserIncrementalWidth;
            DividerLine.Width = OriginalDividerLineWidth + AlpacaChooserIncrementalWidth;

        }

        /// <summary>
        /// Set the enabled state of the OK button and record this as the current state
        /// </summary>
        /// <param name="state"></param>
        private void EnableOkButton(bool state)
        {
            BtnOK.Enabled = state;
            currentOkButtonEnabledState = state;
        }

        /// <summary>
        /// Set the enabled state of the Properties button and record this as the current state
        /// </summary>
        /// <param name="state"></param>
        private void EnablePropertiesButton(bool state)
        {
            BtnProperties.Enabled = state;
            currentPropertiesButtonEnabledState = state;
        }

        #endregion

    }
}
