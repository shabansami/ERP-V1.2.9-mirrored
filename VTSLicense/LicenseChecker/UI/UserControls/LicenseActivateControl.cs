using System;
using System.Windows.Forms;
using VTSLicense.Core;
using VTSLicense.Core.Enums;

namespace VTSLicense.LicenseChecker.UI.UserControls
{
    public partial class LicenseActivateControl : UserControl
    {
        public string AppName { get; set; }

        public byte[] CertificatePublicKeyData { private get; set; }

        public bool ShowMessageAfterValidation { get; set; }

        public Type LicenseObjectType { get; set; }

        public string LicenseBASE64String => txtLicense.Text.Trim();

        public LicenseActivateControl()
        {
            InitializeComponent();

            ShowMessageAfterValidation = true;
        }

        public void ShowUID()
        {
            txtUID.Text = LicenseHandler.GenerateUID(AppName);
        }

        public bool ValidateLicense()
        {
            if (string.IsNullOrWhiteSpace(txtLicense.Text))
            {
                MessageBox.Show("Please input license", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            //Check the activation string
            var _licStatus = LicenseStatus.UNDEFINED;
            var _msg = "";
            var _lic = LicenseHandler.ParseLicenseFromBASE64String(LicenseObjectType, txtLicense.Text.Trim(),
                CertificatePublicKeyData, out _licStatus, out _msg);

            switch (_licStatus)
            {
                case LicenseStatus.VALID:
                    if (ShowMessageAfterValidation)
                        MessageBox.Show("تم التفعيل بنجاح", "تأكيد التفعيل", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                    return true;

                case LicenseStatus.CRACKED:
                case LicenseStatus.INVALID:
                case LicenseStatus.UNDEFINED:
                    if (ShowMessageAfterValidation)
                        MessageBox.Show(_msg, "فشل التفعيل", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                    return false;

                default:
                    return false;
            }
        }


        private void lnkCopy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(txtUID.Text);
        }
    }
}