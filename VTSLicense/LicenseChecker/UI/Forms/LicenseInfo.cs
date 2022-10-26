using System.Windows.Forms;
using VTSLicense.LicenseChecker.UI.UserControls;

namespace VTSLicense.LicenseChecker.UI.Forms
{
    public partial class LicenseInfo : Form
    {
        public LicenseInfo()
        {
            InitializeComponent();

            var conn = new LicenseInfoControl();
            Controls.Add(conn);
            conn.Dock = DockStyle.Fill;
        }
    }
}