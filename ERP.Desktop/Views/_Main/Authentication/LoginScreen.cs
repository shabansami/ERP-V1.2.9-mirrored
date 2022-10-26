using ERP.Desktop.Services.DataBase;
using ERP.Desktop.Services;
using ERP.Desktop.Views._Base.Main;
using System;
using System.Windows.Forms;
using VTSLicense.LicenseChecker.Logic;
using ERP.Desktop.Utilities;

namespace ERP.Desktop.Views._Main.Authentication
{
    public partial class LoginScreen : Form
    {
        #region Constructor

        public LoginScreen()
        {
            InitializeComponent();
        }

        #endregion

        #region Button Events

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Login();
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Login method
        /// </summary>
        private void Login()
        {
            if (!Validations.ValidiateControls(txt_username, txt_password))
                return;
            if (!DatabaseConnection.ValidConnection())
            {
                MessageBox.Show("فشل الاتصال بقاعدة البيانات. برجاء التحقق من اعدادت الاتصال", "فشل العملية", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var userInfo = UserServices.Login(txt_username.Text, txt_password.Text);

            if (userInfo == null)
            {
                MessageBox.Show("!بيانات الدخول خاطئة", "فشل العملية", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!userInfo.IsActive)
            {
                MessageBox.Show("!الحساب غير مفعل", "فشل العملية", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            UserServices.UserInfo = userInfo;

            Program.SetMainForm(new UserMainForm());
            Program.ShowMainForm();
            Close();
        }

        #endregion

        private void LoginScreen_Shown(object sender, EventArgs e)
        {
            btn_login.Enabled = true;
        }

        private void LoginScreen_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void LoginScreen_Load(object sender, EventArgs e)
        {
            //if (!LicenseChecker.CheckLicense())
            //{
            //    Close();
            //    Application.Exit();
            //}

#if DEBUG
            txt_username.Text = "admin";
            txt_password.Text = "1";
#endif
        }
    }
}