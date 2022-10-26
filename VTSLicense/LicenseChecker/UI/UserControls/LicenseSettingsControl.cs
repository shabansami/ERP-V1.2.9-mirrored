using System;
using System.Security;
using System.Windows.Forms;
using VTSLicense.Core;
using VTSLicense.Core.Enums;
using VTSLicense.LicenseChecker.UI.EventArgs;

namespace VTSLicense.LicenseChecker.UI.UserControls
{
    public delegate void LicenseSettingsValidatingHandler(object sender, LicenseSettingsValidatingEventArgs e);

    public delegate void LicenseGeneratedHandler(object sender, LicenseGeneratedEventArgs e);

    public partial class LicenseSettingsControl : UserControl
    {
        public event LicenseSettingsValidatingHandler OnLicenseSettingsValidating;
        public event LicenseGeneratedHandler OnLicenseGenerated;

        protected LicenseEntity _lic;

        public LicenseEntity License
        {
            set
            {
                _lic = value;
                pgLicenseSettings.SelectedObject = _lic;
            }
        }

        public byte[] CertificatePrivateKeyData { set; private get; }

        public SecureString CertificatePassword { set; private get; }

        public bool AllowVolumeLicense
        {
            get => grpbxLicenseType.Enabled;
            set
            {
                if (!value) rdoSingleLicense.Checked = true;

                grpbxLicenseType.Enabled = value;
            }
        }

        public LicenseSettingsControl()
        {
            InitializeComponent();
        }


        private void LicenseTypeRadioButtons_CheckedChanged(object sender, System.EventArgs e)
        {
            txtUID.Text = string.Empty;
            txtUID.Enabled = rdoSingleLicense.Checked || rbExpiration.Checked;
            dtpExpirationDate.Enabled = rbExpiration.Checked;
        }

        private void btnGenLicense_Click(object sender, System.EventArgs e)
        {
            if (_lic == null) throw new ArgumentException("LicenseEntity is invalid");

            if (rdoSingleLicense.Checked)
            {
                if (LicenseHandler.ValidateUIDFormat(txtUID.Text.Trim()))
                {
                    _lic.Type = LicenseTypes.Single;
                    _lic.UID = txtUID.Text.Trim();
                }
                else
                {
                    MessageBox.Show("License UID is blank or invalid", string.Empty, MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
            }
            else if (rdoVolumeLicense.Checked)
            {
                _lic.Type = LicenseTypes.Volume;
                _lic.UID = string.Empty;
            }
            else if (rbExpiration.Checked)
            {
                if (LicenseHandler.ValidateUIDFormat(txtUID.Text.Trim()))
                {
                    _lic.Type = LicenseTypes.ExpirationDate;
                    _lic.UID = txtUID.Text.Trim();
                    _lic.ExpireDateTime = dtpExpirationDate.Value;
                }
                else
                {
                    MessageBox.Show("License UID is blank or invalid", string.Empty, MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
            }

            _lic.CreateDateTime = DateTime.Now;

            if (OnLicenseSettingsValidating != null)
            {
                var _args = new LicenseSettingsValidatingEventArgs()
                    { License = _lic, CancelGenerating = false };

                OnLicenseSettingsValidating(this, _args);

                if (_args.CancelGenerating) return;
            }

            if (OnLicenseGenerated != null)
            {
                var _licStr =
                    LicenseHandler.GenerateLicenseBASE64String(_lic, CertificatePrivateKeyData, CertificatePassword);

                OnLicenseGenerated(this, new LicenseGeneratedEventArgs() { LicenseBASE64String = _licStr });
            }
        }
    }
}