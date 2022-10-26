using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Windows.Forms;
using VTSLicense;
using VTSLicense.Core;
using VTSLicense.LicenseChecker.Logic;
using VTSLicense.LicenseChecker.UI.EventArgs;

namespace ActivationTool
{
    public partial class MainForm : Form
    {
        private byte[] _certPubicKeyData;
        private readonly SecureString _certPwd = new SecureString();

        public MainForm()
        {
            InitializeComponent();

            //Certificate password
            _certPwd.AppendChar('V');
            _certPwd.AppendChar('T');
            _certPwd.AppendChar('S');
            _certPwd.AppendChar('1');
            _certPwd.AppendChar('9');
            _certPwd.AppendChar('9');
            _certPwd.AppendChar('1');
        }


        private void licSettings_OnLicenseGenerated(object sender, LicenseGeneratedEventArgs e)
        {
            //Event raised when license string is generated. Just show it in the text box
            licenseStringContainer1.LicenseString = e.LicenseBASE64String;
        }


        private void btnGenSvrMgmLic_Click(object sender, EventArgs e)
        {
            //Event raised when "Generate License" button is clicked. 
            //Call the core library to generate the license
            licenseStringContainer1.LicenseString = LicenseHandler.GenerateLicenseBASE64String(
                new LicenseFeatures(),
                _certPubicKeyData,
                _certPwd);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //Read public key from assembly
            var assembly = Assembly.GetExecutingAssembly();
            using (var mem = new MemoryStream())
            {
                assembly.GetManifestResourceStream("ActivationTool.Certificate.LicenseSign.pfx")?.CopyTo(mem);

                _certPubicKeyData = mem.ToArray();
            }

            //Initialize the path for the certificate to sign the XML license file
            licenseSettingsControl1.CertificatePrivateKeyData = _certPubicKeyData;
            licenseSettingsControl1.CertificatePassword = _certPwd;

            //Initialize a new license object
            licenseSettingsControl1.License = new LicenseFeatures();
        }
    }
}