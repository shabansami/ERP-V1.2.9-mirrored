using VTSLicense.Core;

namespace VTSLicense.LicenseChecker.UI.EventArgs
{
    public class LicenseSettingsValidatingEventArgs : System.EventArgs
    {
        public LicenseEntity License { get; set; }
        public bool CancelGenerating { get; set; }
    }
}