using ERP.Desktop.Services;
using ERP.Desktop.Utilities;
using ERP.Desktop.Views._Main;
using PrintEngine.HTMLPrint.ReportClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ERP.Desktop.Views.Settings.MainSettings
{
    public partial class GeneralSettings : BaseForm
    {
        private string imageBase64;

        List<string> printers;
        public GeneralSettings()
        {
            InitializeComponent();
            printers = CommonMethods.GetPrinters();
        }

        #region Load
        private void GeneralSettings_Load(object sender, EventArgs e)
        {
            FillCombos();
            LoadMainSettings();
            LoadPrinterSettings();
        }


        private void FillCombos()
        {
            cmbPrinters.Items.AddRange(printers.ToArray());
            var db = DBContext.UnitDbContext;
            {
                var branches = db.Branches.Where(x => !x.IsDeleted).ToList();
                CommonMethods.FillComboBox(cmbBranches, branches, "Name", "Id");
            }
        }


        private void LoadMainSettings()
        {
            var posID = Properties.Settings.Default.PointOfSale;
            var db = DBContext.UnitDbContext;
            {
                var pos = db.PointOfSales.FirstOrDefault(x =>!x.IsDeleted && x.Id == posID);
                if(pos != null)
                {
                    cmbBranches.SelectedValue = pos.BrunchId;
                    cmbPOSs.SelectedValue = pos.Id;
                }
            }
        }
        private void LoadPrinterSettings()
        {
            var defPrinter = Properties.Settings.Default.DefaultPrinter;
            if (printers.Contains(defPrinter))
                cmbPrinters.SelectedItem = defPrinter;
            txtEntityName.Text = Properties.Settings.Default.EntityName;
            txtHeader1.Text = Properties.Settings.Default.Header1;
            txtHeader2.Text = Properties.Settings.Default.Header2;
            txtFooter1.Text = Properties.Settings.Default.Footer1;
            txtFooter2.Text = Properties.Settings.Default.Footer2;
            txtBarcodeName.Text = Properties.Settings.Default.BarcodeName;
            txtBarcodePhone.Text = Properties.Settings.Default.BarcodePhone;
            pictureBox.Image = Converters.Base64ToImage(Properties.Settings.Default.LogoBase64);
        }
        #endregion

        #region Changing in branch and POS
        private void cmbBranches_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBranches.SelectedIndex > 0 && Guid.TryParse(cmbBranches.SelectedValue + "", out Guid branchID))
            {
                var db = DBContext.UnitDbContext;
                {
                    var POSs = db.PointOfSales.Where(x => !x.IsDeleted && x.BrunchId == branchID).ToList();
                    CommonMethods.FillComboBox(cmbPOSs, POSs, "Name", "Id");
                }
            }
        }

        private void cmbPOSs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPOSs.SelectedIndex > 0 && Guid.TryParse(cmbPOSs.SelectedValue + "", out Guid POSid))
            {
                var db = DBContext.UnitDbContext;
                {
                    var pos = db.PointOfSales.FirstOrDefault(x => !x.IsDeleted && x.Id == POSid);
                    if (pos != null)
                    {
                        lblBank.Text = pos.BankAccount.Bank.Name;
                        lblBankAccount.Text = pos.BankAccount.AccountName;
                        lbl_bankWallet.Text = pos.BankAccountWallet?.AccountName;
                        lbl_bankCard.Text = pos.BankAccountCard?.AccountName;
                        lblSafe.Text = pos.Safe.Name;
                        lblDefaultCustomer.Text = pos.PersonCustomer?.Name;
                        lblDefaultSupplier.Text = pos.PersonSupplier?.Name;
                        lblDefaultPaymentType.Text = pos.PaymentType?.Name;
                        lblDefaultPolicy.Text = pos.PricingPolicy?.Name;
                        lblDefaultStore.Text = pos.Store?.Name;
                    }
                    else
                    {
                        lblBank.Text = lblBankAccount.Text= lbl_bankCard.Text= lbl_bankWallet.Text = lblSafe.Text = "";
                    }
                }
            }
        }
        #endregion

        #region Save
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbBranches.SelectedIndex > 0 && Guid.TryParse(cmbBranches.SelectedValue + "", out Guid branchID))
            {

            }
            else
            {
                AlrtMsgs.ChooseVaildData("الفرع");
                cmbBranches.Focus();
                return;
            }
            if (cmbPOSs.SelectedIndex > 0 && Guid.TryParse(cmbPOSs.SelectedValue + "", out Guid posID))
            {
                Properties.Settings.Default.PointOfSale = posID;
            }
            else
            {
                AlrtMsgs.ChooseVaildData("نقطة البيع");
                cmbPOSs.Focus();
                return;
            }
            Properties.Settings.Default.Save();
            AlrtMsgs.SaveSuccess();
        }
        private void btnSavePrintings_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.DefaultPrinter = cmbPrinters.Text;
            Properties.Settings.Default.EntityName = txtEntityName.Text;
            Properties.Settings.Default.Header1 = txtHeader1.Text;
            Properties.Settings.Default.Header2 = txtHeader2.Text;
            Properties.Settings.Default.Footer1 = txtFooter1.Text;
            Properties.Settings.Default.Footer2 = txtFooter2.Text;
            Properties.Settings.Default.BarcodeName = txtBarcodeName.Text;
            Properties.Settings.Default.BarcodePhone = txtBarcodePhone.Text;
            if(!string.IsNullOrWhiteSpace(imageBase64))
                Properties.Settings.Default.LogoBase64 = imageBase64;

            Properties.Settings.Default.Save();
            LoadPrintingSettings();
            AlrtMsgs.SaveSuccess();
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

        #region Logo
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileOpen = new OpenFileDialog())
            {
                fileOpen.Title = "Open Image file";
                fileOpen.Filter = "JPG Files (*.jpg)| *.jpg";

                if (fileOpen.ShowDialog() == DialogResult.OK)
                {
                    if (fileOpen.FileName.ToLower().EndsWith(".bmp"))
                    {
                        AlrtMsgs.ShowMessageError("ملفات BMP غير مدعومة");
                        return;
                    }
                    using (FileStream fs = File.OpenRead(fileOpen.FileName))
                    {
                        if (fs.Length > 512000)
                        {
                            AlrtMsgs.ShowMessageError("يجب ان لا بتخطي حجم الصورة 512 كب.");
                            return;
                        }
                    }
                    pictureBox.Image = Image.FromFile(fileOpen.FileName);
                    if (pictureBox.Image != null)
                        imageBase64 = Converters.ImageToBase64(pictureBox.Image, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
        }


        private void btnReset_Click(object sender, EventArgs e)
        {
            imageBase64 = null;
            pictureBox.Image = Converters.Base64ToImage(Properties.Settings.Default.LogoBase64);
        }
        #endregion
    }
}
