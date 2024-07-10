using System;
using System.Windows.Forms;

namespace ASCOM.Com
{
    /// <summary>
    /// Create a message dialogue with a "suppress in future" checkbox.
    /// </summary>
    public partial class CheckedMessageBox
    {
        /// <summary>
        /// 
        /// </summary>
        public CheckedMessageBox()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Initialise the state of the suppress dialogue checkbox
            ChkDoNotShowAgain.Checked = Configuration.GetBool(GlobalConstants.SUPPRESS_ALPACA_DRIVER_ADMIN_DIALOGUE, GlobalConstants.SUPPRESS_ALPACA_DRIVER_ADMIN_DIALOGUE_DEFAULT);
            CenterToParent();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ChkDoNotShowAgain_CheckedChanged(object sender, EventArgs e)
        {
            // The checkbox has been clicked so record the new value
            Configuration.SetName(GlobalConstants.SUPPRESS_ALPACA_DRIVER_ADMIN_DIALOGUE, ChkDoNotShowAgain.Checked.ToString());
        }
    }
}
