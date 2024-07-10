using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ASCOM.Com
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ChooserAlpacaConfigurationForm : Form
    {

        // Form overrides dispose to clean up the component list.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components is not null)
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        // Required by the Windows Form Designer
        private System.ComponentModel.IContainer components = null;

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.  
        // Do not modify it using the code editor.
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooserAlpacaConfigurationForm));
            this.BtnOK = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.NumDiscoveryIpPort = new System.Windows.Forms.NumericUpDown();
            this.ChkDNSResolution = new System.Windows.Forms.CheckBox();
            this.NumDiscoveryBroadcasts = new System.Windows.Forms.NumericUpDown();
            this.NumDiscoveryDuration = new System.Windows.Forms.NumericUpDown();
            this.Label1 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.PictureBox1 = new System.Windows.Forms.PictureBox();
            this.ChkListAllDiscoveredDevices = new System.Windows.Forms.CheckBox();
            this.ChkShowDeviceDetails = new System.Windows.Forms.CheckBox();
            this.NumExtraChooserWidth = new System.Windows.Forms.NumericUpDown();
            this.Label4 = new System.Windows.Forms.Label();
            this.ChkShowCreateNewAlpacaDriverMessage = new System.Windows.Forms.CheckBox();
            this.GrpIpVersion = new System.Windows.Forms.GroupBox();
            this.RadIpV4AndV6 = new System.Windows.Forms.RadioButton();
            this.RadIpV6 = new System.Windows.Forms.RadioButton();
            this.RadIpV4 = new System.Windows.Forms.RadioButton();
            this.ChkMultiThreadedChooser = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.NumDiscoveryIpPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumDiscoveryBroadcasts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumDiscoveryDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumExtraChooserWidth)).BeginInit();
            this.GrpIpVersion.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnOK
            // 
            this.BtnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnOK.Location = new System.Drawing.Point(516, 311);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.Size = new System.Drawing.Size(75, 23);
            this.BtnOK.TabIndex = 5;
            this.BtnOK.Text = "OK";
            this.BtnOK.UseVisualStyleBackColor = true;
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(598, 311);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 6;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // NumDiscoveryIpPort
            // 
            this.NumDiscoveryIpPort.Location = new System.Drawing.Point(165, 73);
            this.NumDiscoveryIpPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.NumDiscoveryIpPort.Minimum = new decimal(new int[] {
            32227,
            0,
            0,
            0});
            this.NumDiscoveryIpPort.Name = "NumDiscoveryIpPort";
            this.NumDiscoveryIpPort.Size = new System.Drawing.Size(120, 20);
            this.NumDiscoveryIpPort.TabIndex = 1;
            this.NumDiscoveryIpPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumDiscoveryIpPort.Value = new decimal(new int[] {
            32227,
            0,
            0,
            0});
            // 
            // ChkDNSResolution
            // 
            this.ChkDNSResolution.AutoSize = true;
            this.ChkDNSResolution.Location = new System.Drawing.Point(269, 193);
            this.ChkDNSResolution.Name = "ChkDNSResolution";
            this.ChkDNSResolution.Size = new System.Drawing.Size(233, 17);
            this.ChkDNSResolution.TabIndex = 4;
            this.ChkDNSResolution.Text = "Attempt DNS name resolution (Default false)";
            this.ChkDNSResolution.UseVisualStyleBackColor = true;
            // 
            // NumDiscoveryBroadcasts
            // 
            this.NumDiscoveryBroadcasts.Location = new System.Drawing.Point(165, 99);
            this.NumDiscoveryBroadcasts.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumDiscoveryBroadcasts.Name = "NumDiscoveryBroadcasts";
            this.NumDiscoveryBroadcasts.Size = new System.Drawing.Size(120, 20);
            this.NumDiscoveryBroadcasts.TabIndex = 2;
            this.NumDiscoveryBroadcasts.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumDiscoveryBroadcasts.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // NumDiscoveryDuration
            // 
            this.NumDiscoveryDuration.Location = new System.Drawing.Point(165, 125);
            this.NumDiscoveryDuration.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            65536});
            this.NumDiscoveryDuration.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumDiscoveryDuration.Name = "NumDiscoveryDuration";
            this.NumDiscoveryDuration.Size = new System.Drawing.Size(120, 20);
            this.NumDiscoveryDuration.TabIndex = 3;
            this.NumDiscoveryDuration.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumDiscoveryDuration.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(291, 75);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(205, 13);
            this.Label1.TabIndex = 6;
            this.Label1.Text = "Discovery IP Port Number (Default 32227)";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(291, 101);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(214, 13);
            this.Label2.TabIndex = 7;
            this.Label2.Text = "Number of Discovery Broadcasts (Default 2)";
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(291, 127);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(187, 13);
            this.Label3.TabIndex = 8;
            this.Label3.Text = "Discovery Duration (Default 1 second)";
            // 
            // PictureBox1
            // 
            this.PictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("PictureBox1.Image")));
            this.PictureBox1.ImageLocation = "";
            this.PictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("PictureBox1.InitialImage")));
            this.PictureBox1.Location = new System.Drawing.Point(12, 12);
            this.PictureBox1.Name = "PictureBox1";
            this.PictureBox1.Size = new System.Drawing.Size(100, 76);
            this.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PictureBox1.TabIndex = 9;
            this.PictureBox1.TabStop = false;
            // 
            // ChkListAllDiscoveredDevices
            // 
            this.ChkListAllDiscoveredDevices.AutoSize = true;
            this.ChkListAllDiscoveredDevices.Location = new System.Drawing.Point(269, 216);
            this.ChkListAllDiscoveredDevices.Name = "ChkListAllDiscoveredDevices";
            this.ChkListAllDiscoveredDevices.Size = new System.Drawing.Size(218, 17);
            this.ChkListAllDiscoveredDevices.TabIndex = 10;
            this.ChkListAllDiscoveredDevices.Text = "List all discovered devices (Default false)";
            this.ChkListAllDiscoveredDevices.UseVisualStyleBackColor = true;
            // 
            // ChkShowDeviceDetails
            // 
            this.ChkShowDeviceDetails.AutoSize = true;
            this.ChkShowDeviceDetails.Location = new System.Drawing.Point(269, 239);
            this.ChkShowDeviceDetails.Name = "ChkShowDeviceDetails";
            this.ChkShowDeviceDetails.Size = new System.Drawing.Size(189, 17);
            this.ChkShowDeviceDetails.TabIndex = 11;
            this.ChkShowDeviceDetails.Text = "Show device details (Default false)";
            this.ChkShowDeviceDetails.UseVisualStyleBackColor = true;
            // 
            // NumExtraChooserWidth
            // 
            this.NumExtraChooserWidth.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NumExtraChooserWidth.Location = new System.Drawing.Point(165, 151);
            this.NumExtraChooserWidth.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.NumExtraChooserWidth.Name = "NumExtraChooserWidth";
            this.NumExtraChooserWidth.Size = new System.Drawing.Size(120, 20);
            this.NumExtraChooserWidth.TabIndex = 12;
            this.NumExtraChooserWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(291, 153);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(175, 13);
            this.Label4.TabIndex = 13;
            this.Label4.Text = "Additional Chooser width (Default 0)";
            // 
            // ChkShowCreateNewAlpacaDriverMessage
            // 
            this.ChkShowCreateNewAlpacaDriverMessage.AutoSize = true;
            this.ChkShowCreateNewAlpacaDriverMessage.Location = new System.Drawing.Point(269, 262);
            this.ChkShowCreateNewAlpacaDriverMessage.Name = "ChkShowCreateNewAlpacaDriverMessage";
            this.ChkShowCreateNewAlpacaDriverMessage.Size = new System.Drawing.Size(235, 17);
            this.ChkShowCreateNewAlpacaDriverMessage.TabIndex = 14;
            this.ChkShowCreateNewAlpacaDriverMessage.Text = "Show \'Create new Alpaca driver\' instructions";
            this.ChkShowCreateNewAlpacaDriverMessage.UseVisualStyleBackColor = true;
            // 
            // GrpIpVersion
            // 
            this.GrpIpVersion.Controls.Add(this.RadIpV4AndV6);
            this.GrpIpVersion.Controls.Add(this.RadIpV6);
            this.GrpIpVersion.Controls.Add(this.RadIpV4);
            this.GrpIpVersion.Location = new System.Drawing.Point(531, 73);
            this.GrpIpVersion.Name = "GrpIpVersion";
            this.GrpIpVersion.Size = new System.Drawing.Size(136, 107);
            this.GrpIpVersion.TabIndex = 15;
            this.GrpIpVersion.TabStop = false;
            this.GrpIpVersion.Text = "Supported IP Version(s)";
            // 
            // RadIpV4AndV6
            // 
            this.RadIpV4AndV6.AutoSize = true;
            this.RadIpV4AndV6.Location = new System.Drawing.Point(6, 78);
            this.RadIpV4AndV6.Name = "RadIpV4AndV6";
            this.RadIpV4AndV6.Size = new System.Drawing.Size(88, 17);
            this.RadIpV4AndV6.TabIndex = 2;
            this.RadIpV4AndV6.TabStop = true;
            this.RadIpV4AndV6.Text = "IP V4 and V6";
            this.RadIpV4AndV6.UseVisualStyleBackColor = true;
            // 
            // RadIpV6
            // 
            this.RadIpV6.AutoSize = true;
            this.RadIpV6.Location = new System.Drawing.Point(6, 52);
            this.RadIpV6.Name = "RadIpV6";
            this.RadIpV6.Size = new System.Drawing.Size(75, 17);
            this.RadIpV6.TabIndex = 1;
            this.RadIpV6.TabStop = true;
            this.RadIpV6.Text = "IP V6 Only";
            this.RadIpV6.UseVisualStyleBackColor = true;
            // 
            // RadIpV4
            // 
            this.RadIpV4.AutoSize = true;
            this.RadIpV4.Location = new System.Drawing.Point(6, 26);
            this.RadIpV4.Name = "RadIpV4";
            this.RadIpV4.Size = new System.Drawing.Size(75, 17);
            this.RadIpV4.TabIndex = 0;
            this.RadIpV4.TabStop = true;
            this.RadIpV4.Text = "IP V4 Only";
            this.RadIpV4.UseVisualStyleBackColor = true;
            // 
            // ChkMultiThreadedChooser
            // 
            this.ChkMultiThreadedChooser.AutoSize = true;
            this.ChkMultiThreadedChooser.Location = new System.Drawing.Point(269, 285);
            this.ChkMultiThreadedChooser.Name = "ChkMultiThreadedChooser";
            this.ChkMultiThreadedChooser.Size = new System.Drawing.Size(304, 17);
            this.ChkMultiThreadedChooser.TabIndex = 16;
            this.ChkMultiThreadedChooser.Text = "DIsplay Chooser while discovery is underway (Default true))";
            this.ChkMultiThreadedChooser.UseVisualStyleBackColor = true;
            // 
            // ChooserAlpacaConfigurationForm
            // 
            this.AcceptButton = this.BtnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(685, 346);
            this.Controls.Add(this.ChkMultiThreadedChooser);
            this.Controls.Add(this.GrpIpVersion);
            this.Controls.Add(this.ChkShowCreateNewAlpacaDriverMessage);
            this.Controls.Add(this.Label4);
            this.Controls.Add(this.NumExtraChooserWidth);
            this.Controls.Add(this.ChkShowDeviceDetails);
            this.Controls.Add(this.ChkListAllDiscoveredDevices);
            this.Controls.Add(this.PictureBox1);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.NumDiscoveryDuration);
            this.Controls.Add(this.NumDiscoveryBroadcasts);
            this.Controls.Add(this.ChkDNSResolution);
            this.Controls.Add(this.NumDiscoveryIpPort);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ChooserAlpacaConfigurationForm";
            this.Text = "Alpaca Discovery Configuration";
            this.Load += new System.EventHandler(this.ChooserAlpacaConfigurationForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.NumDiscoveryIpPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumDiscoveryBroadcasts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumDiscoveryDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumExtraChooserWidth)).EndInit();
            this.GrpIpVersion.ResumeLayout(false);
            this.GrpIpVersion.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        internal Button BtnOK;
        internal Button BtnCancel;
        internal NumericUpDown NumDiscoveryIpPort;
        internal CheckBox ChkDNSResolution;
        internal NumericUpDown NumDiscoveryBroadcasts;
        internal NumericUpDown NumDiscoveryDuration;
        internal Label Label1;
        internal Label Label2;
        internal Label Label3;
        internal PictureBox PictureBox1;
        internal CheckBox ChkListAllDiscoveredDevices;
        internal CheckBox ChkShowDeviceDetails;
        internal NumericUpDown NumExtraChooserWidth;
        internal Label Label4;
        internal CheckBox ChkShowCreateNewAlpacaDriverMessage;
        internal GroupBox GrpIpVersion;
        internal RadioButton RadIpV4AndV6;
        internal RadioButton RadIpV6;
        internal RadioButton RadIpV4;
        internal CheckBox ChkMultiThreadedChooser;
    }
}