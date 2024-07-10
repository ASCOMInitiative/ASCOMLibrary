using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ASCOM.Com
{
    internal partial class ChooserForm
    {
        #region Windows Form Designer generated code 
        // Required by the Windows Form Designer
        private System.ComponentModel.IContainer components=null;
        public PictureBox picASCOM;
        public Button BtnCancel;
        public Button BtnOK;
        public Button BtnProperties;
        public ComboBox CmbDriverSelector;
        public Label Label1;
        public Label lblTitle;
        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.
        // Do not modify it using the code editor.
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooserForm));
            this.picASCOM = new System.Windows.Forms.PictureBox();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnOK = new System.Windows.Forms.Button();
            this.BtnProperties = new System.Windows.Forms.Button();
            this.CmbDriverSelector = new System.Windows.Forms.ComboBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.ChooserMenu = new System.Windows.Forms.MenuStrip();
            this.MnuTrace = new System.Windows.Forms.ToolStripMenuItem();
            this.NormallyLeaveTheseDisabledToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuSerialTraceEnabled = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuProfileTraceEnabled = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuRegistryTraceEnabled = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuUtilTraceEnabled = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSimulatorTraceEnabled = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuDriverAccessTraceEnabled = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuTransformTraceEnabled = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuNovasTraceEnabled = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuAstroUtilsTraceEnabled = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuCacheTraceEnabled = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuEarthRotationDataFormTraceEnabled = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuAlpaca = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.MnuDiscoverNow = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.MnuEnableDiscovery = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuDisableDiscovery = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuManageAlpacaDevices = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuCreateAlpacaDriver = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuConfigureChooser = new System.Windows.Forms.ToolStripMenuItem();
            this.SerialTraceFileName = new System.Windows.Forms.SaveFileDialog();
            this.LblAlpacaDiscovery = new System.Windows.Forms.Label();
            this.AlpacaStatus = new System.Windows.Forms.PictureBox();
            this.DividerLine = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).BeginInit();
            this.ChooserMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AlpacaStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // picASCOM
            // 
            this.picASCOM.BackColor = System.Drawing.SystemColors.Control;
            this.picASCOM.ForeColor = System.Drawing.SystemColors.ControlText;
            this.picASCOM.Image = ((System.Drawing.Image)(resources.GetObject("picASCOM.Image")));
            this.picASCOM.Location = new System.Drawing.Point(15, 115);
            this.picASCOM.Name = "picASCOM";
            this.picASCOM.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picASCOM.Size = new System.Drawing.Size(48, 56);
            this.picASCOM.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picASCOM.TabIndex = 5;
            this.picASCOM.TabStop = false;
            this.picASCOM.Click += new System.EventHandler(this.PicASCOM_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.BtnCancel.Cursor = System.Windows.Forms.Cursors.Default;
            this.BtnCancel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.BtnCancel.Location = new System.Drawing.Point(242, 144);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.BtnCancel.Size = new System.Drawing.Size(79, 23);
            this.BtnCancel.TabIndex = 4;
            this.BtnCancel.Text = "&Cancel";
            this.BtnCancel.UseVisualStyleBackColor = false;
            this.BtnCancel.Click += new System.EventHandler(this.CmdCancel_Click);
            // 
            // BtnOK
            // 
            this.BtnOK.BackColor = System.Drawing.SystemColors.Control;
            this.BtnOK.Cursor = System.Windows.Forms.Cursors.Default;
            this.BtnOK.Enabled = false;
            this.BtnOK.ForeColor = System.Drawing.SystemColors.ControlText;
            this.BtnOK.Location = new System.Drawing.Point(242, 115);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.BtnOK.Size = new System.Drawing.Size(79, 23);
            this.BtnOK.TabIndex = 3;
            this.BtnOK.Text = "&OK";
            this.BtnOK.UseVisualStyleBackColor = false;
            this.BtnOK.Click += new System.EventHandler(this.CmdOK_Click);
            // 
            // BtnProperties
            // 
            this.BtnProperties.BackColor = System.Drawing.SystemColors.Control;
            this.BtnProperties.Cursor = System.Windows.Forms.Cursors.Default;
            this.BtnProperties.Enabled = false;
            this.BtnProperties.ForeColor = System.Drawing.SystemColors.ControlText;
            this.BtnProperties.Location = new System.Drawing.Point(242, 69);
            this.BtnProperties.Name = "BtnProperties";
            this.BtnProperties.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.BtnProperties.Size = new System.Drawing.Size(79, 23);
            this.BtnProperties.TabIndex = 1;
            this.BtnProperties.Text = "&Properties...";
            this.BtnProperties.UseVisualStyleBackColor = false;
            this.BtnProperties.Click += new System.EventHandler(this.CmdProperties_Click);
            // 
            // CmbDriverSelector
            // 
            this.CmbDriverSelector.BackColor = System.Drawing.SystemColors.Window;
            this.CmbDriverSelector.Cursor = System.Windows.Forms.Cursors.Default;
            this.CmbDriverSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbDriverSelector.ForeColor = System.Drawing.SystemColors.WindowText;
            this.CmbDriverSelector.Location = new System.Drawing.Point(15, 71);
            this.CmbDriverSelector.Name = "CmbDriverSelector";
            this.CmbDriverSelector.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CmbDriverSelector.Size = new System.Drawing.Size(214, 21);
            this.CmbDriverSelector.Sorted = true;
            this.CmbDriverSelector.TabIndex = 0;
            this.CmbDriverSelector.SelectionChangeCommitted += new System.EventHandler(this.CbDriverSelector_SelectionChangeCommitted);
            // 
            // Label1
            // 
            this.Label1.BackColor = System.Drawing.SystemColors.Control;
            this.Label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.Location = new System.Drawing.Point(69, 117);
            this.Label1.Name = "Label1";
            this.Label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label1.Size = new System.Drawing.Size(160, 54);
            this.Label1.TabIndex = 6;
            this.Label1.Text = "Click the logo to learn more about ASCOM, a set of standards for inter-operation " +
    "of astronomy software.";
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.SystemColors.Control;
            this.lblTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblTitle.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblTitle.Location = new System.Drawing.Point(12, 24);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblTitle.Size = new System.Drawing.Size(321, 42);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ChooserMenu
            // 
            this.ChooserMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnuTrace,
            this.MnuAlpaca});
            this.ChooserMenu.Location = new System.Drawing.Point(0, 0);
            this.ChooserMenu.Name = "ChooserMenu";
            this.ChooserMenu.Size = new System.Drawing.Size(333, 24);
            this.ChooserMenu.TabIndex = 7;
            this.ChooserMenu.Text = "ChooserMenu";
            // 
            // MnuTrace
            // 
            this.MnuTrace.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NormallyLeaveTheseDisabledToolStripMenuItem,
            this.ToolStripSeparator1,
            this.MenuSerialTraceEnabled,
            this.MenuProfileTraceEnabled,
            this.MenuRegistryTraceEnabled,
            this.MenuUtilTraceEnabled,
            this.MenuSimulatorTraceEnabled,
            this.MenuDriverAccessTraceEnabled,
            this.MenuTransformTraceEnabled,
            this.MenuNovasTraceEnabled,
            this.MenuAstroUtilsTraceEnabled,
            this.MenuCacheTraceEnabled,
            this.MenuEarthRotationDataFormTraceEnabled});
            this.MnuTrace.Name = "MnuTrace";
            this.MnuTrace.Size = new System.Drawing.Size(46, 20);
            this.MnuTrace.Text = "Trace";
            this.MnuTrace.DropDownOpening += new System.EventHandler(this.MenuTrace_DropDownOpening);
            // 
            // NormallyLeaveTheseDisabledToolStripMenuItem
            // 
            this.NormallyLeaveTheseDisabledToolStripMenuItem.Name = "NormallyLeaveTheseDisabledToolStripMenuItem";
            this.NormallyLeaveTheseDisabledToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
            this.NormallyLeaveTheseDisabledToolStripMenuItem.Text = "Normally leave these disabled";
            // 
            // ToolStripSeparator1
            // 
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            this.ToolStripSeparator1.Size = new System.Drawing.Size(279, 6);
            // 
            // MenuSerialTraceEnabled
            // 
            this.MenuSerialTraceEnabled.Name = "MenuSerialTraceEnabled";
            this.MenuSerialTraceEnabled.Size = new System.Drawing.Size(282, 22);
            this.MenuSerialTraceEnabled.Text = "Serial Trace Enabled";
            this.MenuSerialTraceEnabled.Click += new System.EventHandler(this.MenuSerialTraceEnabled_Click);
            // 
            // MenuProfileTraceEnabled
            // 
            this.MenuProfileTraceEnabled.Name = "MenuProfileTraceEnabled";
            this.MenuProfileTraceEnabled.Size = new System.Drawing.Size(282, 22);
            this.MenuProfileTraceEnabled.Text = "Profile Trace Enabled";
            this.MenuProfileTraceEnabled.Click += new System.EventHandler(this.MenuProfileTraceEnabled_Click_1);
            // 
            // MenuRegistryTraceEnabled
            // 
            this.MenuRegistryTraceEnabled.Name = "MenuRegistryTraceEnabled";
            this.MenuRegistryTraceEnabled.Size = new System.Drawing.Size(282, 22);
            this.MenuRegistryTraceEnabled.Text = "Registry Trace Enabled";
            this.MenuRegistryTraceEnabled.Click += new System.EventHandler(this.MenuRegistryTraceEnabled_Click);
            // 
            // MenuUtilTraceEnabled
            // 
            this.MenuUtilTraceEnabled.Name = "MenuUtilTraceEnabled";
            this.MenuUtilTraceEnabled.Size = new System.Drawing.Size(282, 22);
            this.MenuUtilTraceEnabled.Text = "Util Trace Enabled";
            this.MenuUtilTraceEnabled.Click += new System.EventHandler(this.MenuUtilTraceEnabled_Click_1);
            // 
            // MenuSimulatorTraceEnabled
            // 
            this.MenuSimulatorTraceEnabled.Name = "MenuSimulatorTraceEnabled";
            this.MenuSimulatorTraceEnabled.Size = new System.Drawing.Size(282, 22);
            this.MenuSimulatorTraceEnabled.Text = "Simulator Trace Enabled";
            this.MenuSimulatorTraceEnabled.Click += new System.EventHandler(this.MenuSimulatorTraceEnabled_Click);
            // 
            // MenuDriverAccessTraceEnabled
            // 
            this.MenuDriverAccessTraceEnabled.Name = "MenuDriverAccessTraceEnabled";
            this.MenuDriverAccessTraceEnabled.Size = new System.Drawing.Size(282, 22);
            this.MenuDriverAccessTraceEnabled.Text = "DriverAccess Trace Enabled";
            this.MenuDriverAccessTraceEnabled.Click += new System.EventHandler(this.MenuDriverAccessTraceEnabled_Click);
            // 
            // MenuTransformTraceEnabled
            // 
            this.MenuTransformTraceEnabled.Name = "MenuTransformTraceEnabled";
            this.MenuTransformTraceEnabled.Size = new System.Drawing.Size(282, 22);
            this.MenuTransformTraceEnabled.Text = "Transform Trace Enabled";
            this.MenuTransformTraceEnabled.Click += new System.EventHandler(this.MenuTransformTraceEnabled_Click);
            // 
            // MenuNovasTraceEnabled
            // 
            this.MenuNovasTraceEnabled.Name = "MenuNovasTraceEnabled";
            this.MenuNovasTraceEnabled.Size = new System.Drawing.Size(282, 22);
            this.MenuNovasTraceEnabled.Text = "NOVAS (Partial) Trace Enabled";
            this.MenuNovasTraceEnabled.Click += new System.EventHandler(this.MenuNovasTraceEnabled_Click);
            // 
            // MenuAstroUtilsTraceEnabled
            // 
            this.MenuAstroUtilsTraceEnabled.Name = "MenuAstroUtilsTraceEnabled";
            this.MenuAstroUtilsTraceEnabled.Size = new System.Drawing.Size(282, 22);
            this.MenuAstroUtilsTraceEnabled.Text = "AstroUtils Trace Enabled";
            this.MenuAstroUtilsTraceEnabled.Click += new System.EventHandler(this.MenuAstroUtilsTraceEnabled_Click);
            // 
            // MenuCacheTraceEnabled
            // 
            this.MenuCacheTraceEnabled.Name = "MenuCacheTraceEnabled";
            this.MenuCacheTraceEnabled.Size = new System.Drawing.Size(282, 22);
            this.MenuCacheTraceEnabled.Text = "Cache Trace Enabled";
            this.MenuCacheTraceEnabled.Click += new System.EventHandler(this.MenuCacheTraceEnabled_Click);
            // 
            // MenuEarthRotationDataFormTraceEnabled
            // 
            this.MenuEarthRotationDataFormTraceEnabled.Name = "MenuEarthRotationDataFormTraceEnabled";
            this.MenuEarthRotationDataFormTraceEnabled.Size = new System.Drawing.Size(282, 22);
            this.MenuEarthRotationDataFormTraceEnabled.Text = "Earth Rotation Data Form Trace Enabled";
            this.MenuEarthRotationDataFormTraceEnabled.Click += new System.EventHandler(this.MenuEarthRotationDataTraceEnabled_Click);
            // 
            // MnuAlpaca
            // 
            this.MnuAlpaca.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripSeparator3,
            this.MnuDiscoverNow,
            this.ToolStripSeparator4,
            this.MnuEnableDiscovery,
            this.MnuDisableDiscovery,
            this.MnuManageAlpacaDevices,
            this.MnuCreateAlpacaDriver,
            this.MnuConfigureChooser});
            this.MnuAlpaca.Name = "MnuAlpaca";
            this.MnuAlpaca.Size = new System.Drawing.Size(55, 20);
            this.MnuAlpaca.Text = "Alpaca";
            // 
            // ToolStripSeparator3
            // 
            this.ToolStripSeparator3.Name = "ToolStripSeparator3";
            this.ToolStripSeparator3.Size = new System.Drawing.Size(248, 6);
            // 
            // MnuDiscoverNow
            // 
            this.MnuDiscoverNow.Name = "MnuDiscoverNow";
            this.MnuDiscoverNow.Size = new System.Drawing.Size(251, 22);
            this.MnuDiscoverNow.Text = "Discover Now";
            this.MnuDiscoverNow.Click += new System.EventHandler(this.MnuDiscoverNow_Click);
            // 
            // ToolStripSeparator4
            // 
            this.ToolStripSeparator4.Name = "ToolStripSeparator4";
            this.ToolStripSeparator4.Size = new System.Drawing.Size(248, 6);
            // 
            // MnuEnableDiscovery
            // 
            this.MnuEnableDiscovery.Name = "MnuEnableDiscovery";
            this.MnuEnableDiscovery.Size = new System.Drawing.Size(251, 22);
            this.MnuEnableDiscovery.Text = "Enable DIscovery";
            this.MnuEnableDiscovery.Click += new System.EventHandler(this.MnuEnableDiscovery_Click);
            // 
            // MnuDisableDiscovery
            // 
            this.MnuDisableDiscovery.Name = "MnuDisableDiscovery";
            this.MnuDisableDiscovery.Size = new System.Drawing.Size(251, 22);
            this.MnuDisableDiscovery.Text = "Disable Discovery";
            this.MnuDisableDiscovery.Click += new System.EventHandler(this.MnuDisableDiscovery_Click);
            // 
            // MnuManageAlpacaDevices
            // 
            this.MnuManageAlpacaDevices.Name = "MnuManageAlpacaDevices";
            this.MnuManageAlpacaDevices.Size = new System.Drawing.Size(251, 22);
            this.MnuManageAlpacaDevices.Text = "Manage Devices (Admin)";
            this.MnuManageAlpacaDevices.Click += new System.EventHandler(this.MnuManageAlpacaDevices_Click);
            // 
            // MnuCreateAlpacaDriver
            // 
            this.MnuCreateAlpacaDriver.Name = "MnuCreateAlpacaDriver";
            this.MnuCreateAlpacaDriver.Size = new System.Drawing.Size(251, 22);
            this.MnuCreateAlpacaDriver.Text = "Create Alpaca Driver (Admin)";
            this.MnuCreateAlpacaDriver.Click += new System.EventHandler(this.MnuCreateAlpacaDriver_Click);
            // 
            // MnuConfigureChooser
            // 
            this.MnuConfigureChooser.Name = "MnuConfigureChooser";
            this.MnuConfigureChooser.Size = new System.Drawing.Size(251, 22);
            this.MnuConfigureChooser.Text = "Configure Chooser and Discovery";
            this.MnuConfigureChooser.Click += new System.EventHandler(this.MnuConfigureDiscovery_Click);
            // 
            // LblAlpacaDiscovery
            // 
            this.LblAlpacaDiscovery.AutoSize = true;
            this.LblAlpacaDiscovery.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.LblAlpacaDiscovery.ForeColor = System.Drawing.SystemColors.Highlight;
            this.LblAlpacaDiscovery.Location = new System.Drawing.Point(211, 5);
            this.LblAlpacaDiscovery.Name = "LblAlpacaDiscovery";
            this.LblAlpacaDiscovery.Size = new System.Drawing.Size(90, 13);
            this.LblAlpacaDiscovery.TabIndex = 9;
            this.LblAlpacaDiscovery.Text = "Alpaca Discovery";
            // 
            // AlpacaStatus
            // 
            this.AlpacaStatus.BackColor = System.Drawing.Color.CornflowerBlue;
            this.AlpacaStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.AlpacaStatus.Location = new System.Drawing.Point(305, 8);
            this.AlpacaStatus.Name = "AlpacaStatus";
            this.AlpacaStatus.Size = new System.Drawing.Size(16, 8);
            this.AlpacaStatus.TabIndex = 10;
            this.AlpacaStatus.TabStop = false;
            // 
            // DividerLine
            // 
            this.DividerLine.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.DividerLine.Location = new System.Drawing.Point(15, 102);
            this.DividerLine.Name = "DividerLine";
            this.DividerLine.Size = new System.Drawing.Size(306, 1);
            this.DividerLine.TabIndex = 11;
            // 
            // ChooserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(333, 181);
            this.Controls.Add(this.DividerLine);
            this.Controls.Add(this.LblAlpacaDiscovery);
            this.Controls.Add(this.AlpacaStatus);
            this.Controls.Add(this.picASCOM);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnOK);
            this.Controls.Add(this.BtnProperties);
            this.Controls.Add(this.CmbDriverSelector);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.ChooserMenu);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(3, 22);
            this.MainMenuStrip = this.ChooserMenu;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChooserForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ASCOM <runtime> Chooser (CLR4!)";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ChooserForm_FormClosed);
            this.Load += new System.EventHandler(this.ChooserForm_Load);
            this.Move += new System.EventHandler(this.ChooserFormMoveEventHandler);
            ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).EndInit();
            this.ChooserMenu.ResumeLayout(false);
            this.ChooserMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AlpacaStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal MenuStrip ChooserMenu;
        internal ToolStripMenuItem MnuTrace;
        internal SaveFileDialog SerialTraceFileName;
        internal ToolStripMenuItem MenuSerialTraceEnabled;
        internal ToolStripMenuItem MenuProfileTraceEnabled;
        internal ToolStripMenuItem NormallyLeaveTheseDisabledToolStripMenuItem;
        internal ToolStripSeparator ToolStripSeparator1;
        internal ToolStripMenuItem MenuTransformTraceEnabled;
        internal ToolStripMenuItem MenuUtilTraceEnabled;
        internal ToolStripMenuItem MenuSimulatorTraceEnabled;
        internal ToolStripMenuItem MenuDriverAccessTraceEnabled;
        internal ToolStripMenuItem MenuAstroUtilsTraceEnabled;
        internal ToolStripMenuItem MenuNovasTraceEnabled;
        internal ToolStripMenuItem MenuCacheTraceEnabled;
        internal ToolStripMenuItem MenuEarthRotationDataFormTraceEnabled;
        internal ToolStripMenuItem MnuAlpaca;
        internal ToolStripMenuItem MnuEnableDiscovery;
        internal ToolStripMenuItem MnuDiscoverNow;
        internal ToolStripMenuItem MnuConfigureChooser;
        internal Label LblAlpacaDiscovery;
        internal ToolStripSeparator ToolStripSeparator3;
        internal ToolStripSeparator ToolStripSeparator4;
        internal ToolStripMenuItem MnuDisableDiscovery;
        internal PictureBox AlpacaStatus;
        internal ToolStripMenuItem MenuRegistryTraceEnabled;
        internal Panel DividerLine;
        internal ToolStripMenuItem MnuManageAlpacaDevices;
        internal ToolStripMenuItem MnuCreateAlpacaDriver;
        #endregion
    }
}