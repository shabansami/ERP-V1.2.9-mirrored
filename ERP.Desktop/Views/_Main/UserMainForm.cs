using ERP.Desktop.Utilities.FormManager;
using ERP.Desktop.Views._Main.Authentication;
using ERP.Desktop.Views.Settings.Shift;
using ERP.Desktop.Views.Definations;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ERP.Desktop.Views.Settings.MainSettings;
using ERP.Desktop.Views.Transactions.Sales;
using ERP.Desktop.Views.Items;
using ERP.Desktop.Views.Transactions.SalesBack;
using ERP.Desktop.Views.Actors.Customers;
using ERP.Desktop.Views.Transactions.Purchases;
using ERP.Desktop.Views.Transactions.PurchasesBack;
using ERP.Desktop.Views.Transactions.Pricing;
using ERP.Desktop.Views.Actors.Suppliers;
using ERP.Desktop.Views.Transactions.Incoming;
using ERP.Desktop.Views.Reports.ProductReports;
using ERP.Desktop.Views.Reports.AccountingReports;
using PrintEngine.HTMLPrint.ReportClasses;
using ERP.Desktop.Views.Transactions.InventoryInvoices;
using ERP.Desktop.Views.Transactions.Outcoming;

namespace ERP.Desktop.Views._Base.Main
{
    public partial class UserMainForm : Form
    {
        #region Constructor

        public UserMainForm()
        {
            InitializeComponent();

            SetMdiBackgroundImage();
            SetFormValues();
            SetPrintingDefaults();
            ApplyPermissions();
            FormManager.ParentForm = this;
        }




        #endregion

        #region Form Methods

        private void SetFormValues()
        {
            //lblPersonName.Text = UserInfo.UserName;
            //lblUserRole.Text = UserInfo.UserRoleName;
            Text = $"VTS ";
        }

        /// <summary>
        /// Set the default properties in HTMLReportEngine
        /// </summary>
        private void SetPrintingDefaults()
        {
            //EntityInformation.FooterLine1 = Settings.Default.footerLine1;
            //EntityInformation.FooterLine2 = Settings.Default.footerLine2;
            //EntityInformation.EntityLogoName = Environment.CurrentDirectory + @"\Resources\CenterLogo.png";
        }

        private void ApplyPermissions()
        {
            //if (!UserInfo.UserIsAdmin)
            //{
            //    //Get permissions of each form by roleId of the logged in user
            //    PermissionHandler.GetPermissionsOfEachForm();

            //    //Check what are the roles of the user and disable those that he doesn't have access to
            //    PermissionHandler.DisableControlBasedOnRole(menuStrip1);
            //    MenuAdmin.Visible = false;
            //}
        }


        /// <summary>
        /// Set the background image of the mdi container
        /// Setting it in the designer is not enough because the MdiClient ovverides the form properties
        /// </summary>
        private void SetMdiBackgroundImage()
        {
            //Get the MdiClient itself
            var client = Controls.OfType<MdiClient>().First();
            client.BackColor = Color.White;
        }

        private void UserMainForm_Load(object sender, EventArgs e)
        {
            LoadPrintingSettings();
        }

        private void LoadPrintingSettings()
        {
            EntityInformation.EntityName = Properties.Settings.Default.EntityName;
            EntityInformation.EntityLogoName = Properties.Settings.Default.LogoBase64;
            EntityInformation.HeaderLine1 = Properties.Settings.Default.Header1;
            EntityInformation.HeaderLine2 = Properties.Settings.Default.Header2;
            EntityInformation.FooterLine1 = Properties.Settings.Default.Footer1;
            EntityInformation.FooterLine2 = Properties.Settings.Default.Footer2;
        }
        #endregion

        #region MenuStrip Item Clicks

        private void MenuItemLogout_Click(object sender, EventArgs e)
        {
            Program.SetMainForm(new LoginScreen());
            Program.ShowMainForm();

            Close();
        }

        /// <summary>
        /// Refresh the UserInfo when the form is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Frm_Closed(object sender, EventArgs e)
        {
            SetFormValues();
        }

        #endregion


        private void MenuItemShifts_Click(object sender, EventArgs e)
        {
            var form = FormManager.Show<Shifts>();
        }

        private void MenuItemSettings_Click(object sender, EventArgs e)
        {
            var form = FormManager.Show<PointOfSales>();
        }

        private void btnSystemSettings_Click(object sender, EventArgs e)
        {
            var form = FormManager.Show<GeneralSettings>();
        }


        private void btnSales_Click(object sender, EventArgs e)
        {
            var form = new SellInvoicesCashier();
            form.ShowDialog();
        }

        private void btnSalesBack_Click(object sender, EventArgs e)
        {
            var form = new SellBackInvoicesCashier();
            form.ShowDialog();
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            var form = FormManager.Show<CustomerSelect>();
        }

        private void btnPurchase_Click(object sender, EventArgs e)
        {
            var form = new PurchaseInvoicesCashier();
            form.ShowDialog();
        }

        private void btnPurchaseBack_Click(object sender, EventArgs e)
        {
            var form = new PurchaseBackInvoicesCashier();
            form.ShowDialog();
        }

        private void btnSuppliers_Click(object sender, EventArgs e)
        {
            var form = FormManager.Show<SupplierSelect>();
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            var form = FormManager.Show<ItemAddEdit>();
        }

        private void iconMenuItem4_Click(object sender, EventArgs e)
        {
            var form = FormManager.Show<ItemsList>();
        }

        private void btnCustomersPayments_Click(object sender, EventArgs e)
        {
            var form = FormManager.Show<CustomerPayments>();
        }

        private void btnItemSearchWithPrice_Click(object sender, EventArgs e)
        {
            var form = FormManager.Show<ItemSearchWithPrices>();
        }

        private void btnIncomes_Click(object sender, EventArgs e)
        {
            var form = FormManager.Show<Transactions.Incoming.Incomes>();

        }

        private void btnExpenses_Click(object sender, EventArgs e)
        {
            var form = FormManager.Show<Transactions.Outcoming.Expenses>();

        }

        private void btnItemsBalance_Click(object sender, EventArgs e)
        {
            var form = FormManager.Show<ItemBalances>();
        }

        private void btnSafeAccountings_Click(object sender, EventArgs e)
        {
            var form = FormManager.Show<SafeAccountings>();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Program.CheckTimeTamper();
        }

        private void btnPrintBarcode_Click(object sender, EventArgs e)
        {
            var form = FormManager.Show<PrintItemsBarcode>();
        }

        private void btnPriceInvoices_Click(object sender, EventArgs e)
        {
            var form = new PriceInvoicesCashier();
            form.ShowDialog();
        }

        private void btnInventoryItems_Click(object sender, EventArgs e)
        {
            var form = FormManager.Show<AllInvoices>();
        }

        private void btnNewInventoryInvoice_Click(object sender, EventArgs e)
        {
            var form = FormManager.Show<NewInventoryInvoice>();
        }

        private void iconMenuItem5_Click(object sender, EventArgs e)
        {
            var form = FormManager.Show<ItemSellQuantity>();

        }

        private void iconMenuItem6_Click(object sender, EventArgs e)
        {
            var form = FormManager.Show<PrintShift>();

        }

        private void btnEmployeeLoan_Click(object sender, EventArgs e)
        {
            var form = FormManager.Show<EmployeeLoans>();

        }

    }
}