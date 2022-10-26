using System.IO;
using VTSLicense.LicenseChecker.Logic;
using System.Windows.Forms;

namespace VTSLicense.LicenseChecker.UI.Forms
{
    public partial class Activation : Form
    {
        public byte[] CertificatePublicKeyData { private get; set; }
        private const string AppName = "VTS";
        //TODO:Change app name

        public Activation()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            if (MessageBox.Show("هل أنت متأكد من الالغاء؟", string.Empty, MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes) Close();
        }

        private void Activation_Load(object sender, System.EventArgs e)
        {
            //Assign the application information values to the license control
            licenseActivateControl.AppName = AppName;
            licenseActivateControl.LicenseObjectType = typeof(LicenseFeatures);
            licenseActivateControl.CertificatePublicKeyData = CertificatePublicKeyData;
            //Display the device unique ID
            licenseActivateControl.ShowUID();
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            //Call license control to validate the license string
            if (licenseActivateControl.ValidateLicense())
            {
                //If license if valid, save the license string into a local file
                File.WriteAllText(Path.Combine(Application.StartupPath, "license.lic"),
                    licenseActivateControl.LicenseBASE64String);

                MessageBox.Show("سيتم اغلاق البرنامج لاكمال التفعيل.", string.Empty, MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Close();
            }
        }
    }
}