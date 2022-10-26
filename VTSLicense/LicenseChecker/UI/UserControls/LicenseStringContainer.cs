using System.IO;
using System.Text;
using System.Windows.Forms;

namespace VTSLicense.LicenseChecker.UI.UserControls
{
    public partial class LicenseStringContainer : UserControl
    {
        public string LicenseString
        {
            get => txtLicense.Text;
            set => txtLicense.Text = value;
        }

        public LicenseStringContainer()
        {
            InitializeComponent();
        }

        private void lnkCopy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtLicense.Text)) Clipboard.SetText(txtLicense.Text);
        }

        private void lnkSaveToFile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (dlgSaveFile.ShowDialog() == DialogResult.OK)
                //Save license data into local file
                File.WriteAllText(dlgSaveFile.FileName, txtLicense.Text.Trim(), Encoding.UTF8);
        }
    }
}