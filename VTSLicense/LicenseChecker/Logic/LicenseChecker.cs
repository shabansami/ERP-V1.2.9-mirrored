using System.IO;
using System.Reflection;
using System.Windows.Forms;
using VTSLicense.Core;
using VTSLicense.Core.Enums;
using VTSLicense.LicenseChecker.UI.Forms;

namespace VTSLicense.LicenseChecker.Logic
{
    public static class LicenseChecker
    {
        private static byte[] _certPubicKeyData;

        public static bool CheckLicense()
        {
            //Initialize variables with default values
            LicenseFeatures lic = null;
            var msg = string.Empty;
            var status = LicenseStatus.UNDEFINED;

            //Read public key from assembly
            var assembly = Assembly.GetExecutingAssembly();
            using (var mem = new MemoryStream())
            {
                assembly.GetManifestResourceStream("VTSLicense.LicenseChecker.Certificate.LicenseVerify.cer")?.CopyTo(mem);

                _certPubicKeyData = mem.ToArray();
            }

            //Check if the XML license file exists
            if (File.Exists("license.lic"))
            {
                lic = (LicenseFeatures)LicenseHandler.ParseLicenseFromBASE64String(
                    typeof(LicenseFeatures),
                    File.ReadAllText("license.lic"),
                    _certPubicKeyData,
                    out status,
                    out msg);
            }
            else
            {
                status = LicenseStatus.INVALID;
                msg = "نسخة البرنامج غير مفعلة!";
            }

            switch (status)
            {
                case LicenseStatus.VALID:

                    return true;

                default:
                    //for the other status of license file, show the warning message
                    //and also popup the activation form for user to activate your application
                    MessageBox.Show(msg, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    using (var frm = new Activation())
                    {
                        frm.CertificatePublicKeyData = _certPubicKeyData;
                        frm.ShowDialog();
                        Application.Exit();
                        return false;
                    }
            }
        }
    }
}