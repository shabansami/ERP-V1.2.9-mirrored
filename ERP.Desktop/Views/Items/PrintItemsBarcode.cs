using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using PrintEngine;
using ERP.Desktop.Views._Main;
using ERP.DAL;
using ERP.Desktop.Services;
using System.Linq;
using static ERP.Web.Utilites.Lookups;
using ERP.Desktop.Utilities;
using PrintEngine.HTMLPrint;
using ERP.Desktop.ViewModels;
using ERP.Desktop.Services.Reports.ItemReports;
using System.Threading.Tasks;
using ERP.Web.Utilites;
using ERP.Desktop.Services.Items;

namespace ERP.Desktop.Views.Items
{
    public partial class PrintItemsBarcode : BaseForm
    {
        VTSaleEntities db;
        private int _supplierId, _invoiceId;
        BalanceService balanceService;
        public PrintItemsBarcode()
        {
            InitializeComponent();
            StyleDataGridViews(dgvItems);
            db = DBContext.UnitDbContext;
            balanceService = new BalanceService(db);
        }
        public PrintItemsBarcode(int supplierId, int invoiceId) : this()
        {
            _supplierId = supplierId;
            _invoiceId = invoiceId;
        }

        #region Load and fill

        private void PrintItemsBarcode_Load(object sender, EventArgs e)
        {
            GetPrinters();
            FillCombos();
            cmbSuppliers.SelectedIndex = _supplierId;
            cmbInvoices.SelectedValue = _invoiceId;
            if (_supplierId != 0 && _invoiceId != 0)
            {
                FillDgvBySupplier();
            }
            CommonMethods.AutoComplateTextbox(txtSearch);

        }


        private void FillCombos()
        {
            var suppliers = db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)).ToList();
            CommonMethods.FillComboBox(cmbSuppliers, suppliers, "Name", "Id");

            var groups = db.Groups.Where(x => !x.IsDeleted).ToList();
            CommonMethods.FillComboBox(cmb_Groups, groups, "Name", "ID");
        }

        #endregion

        #region Fill Invoices
        private void cmb_Supplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            var supplierID = cmbSuppliers.GetSelectedID(null);
            var invoices = db.PurchaseInvoices.Where(x => !x.IsDeleted && x.SupplierId == supplierID).Select(x => new IDNameVM() { ID = x.Id, Name = x.InvoiceDate + "" }).ToList();
            CommonMethods.FillComboBox(cmbInvoices, invoices, "Name", "ID");
        }
        #endregion

        #region Control Events

        private void btnSearchTab1_Click(object sender, EventArgs e)
        {
            if (cmbInvoices.SelectedIndex <= 0)
            {
                MessageBox.Show("من فضلك اختر الفاتورة");
                return;
            }

            FillDgvBySupplier();
        }


        private void BtnCheckAllItems_Click(object sender, EventArgs e)
        {
            if (dgvItems.Rows.Count == 0) return;

            //Check all if nothing checked, check nothing if all is checked
            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.GetType() == typeof(DataGridViewCheckBoxCell))
                    {

                        DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)cell;
                        chk.Value = true;
                    }
                }
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            //if didn't select printer
            if (cmbPrinters.SelectedIndex < 0)
            {
                MessageBox.Show("الرجاء اختيار طابعة الباركود", "طباعة الباركود", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbPrinters.Focus();
                return;
            }
            PrintBarcodes();
        }

        private void GetPrinters()
        {
            var printers = new List<string>();
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                printers.Add(printer);
            }
            cmbPrinters.Items.AddRange(printers.ToArray());
        }


        #endregion

        #region Fill DataGrid

        private void FillDgvBySupplier()
        {
            dgvItems.DataSource = null;
            var invoiceID = cmbInvoices.GetSelectedID(null);
            if (invoiceID == null)
                return;
            dgvItems.DataSource = db.PurchaseInvoicesDetails.Where(x => !x.IsDeleted && x.PurchaseInvoiceId == invoiceID).Select(x => new
            {
                ID = x.ItemId,
                Name = x.Item.Name,
                Price = x.Item.SellPrice,
                //Quantity = x.Quantity,
                Barcode = x.Item.BarCode,
                ItemCode = x.Item.ItemCode.ToString()
            }).ToList();
        }


        private async void btnFillDgvByGroup_Click(object sender, EventArgs e)
        {
            dgvItems.DataSource = null;
            SplashScreen.ShowSplashScreen();
            var groupID = cmb_Groups.GetSelectedID(null);
            List<ItemBalanceVM> items = null;
            await Task.Run(() => {
                var list = db.Items.Where(x => !x.IsDeleted);
                if (groupID != null)
                    list = list.Where(x => x.GroupBasicId == groupID);
                items = list.AsEnumerable().Select(x => new ItemBalanceVM
                {
                    ID = x.Id,
                    Name = x.Name,
                    Price = x.SellPrice,
                    //Quantity = balanceService.GetBalanceByItemAllStores(x.Id),
                    Barcode = x.BarCode,
                    ItemCode=x.ItemCode.ToString()
                }).ToList();
            });
            dgvItems.DataSource = items;
            SplashScreen.CloseSplashScreen();
        }

        #endregion

        #region Genarate New Barcode

        public string GenerateRandomBarCode()
        {
            string randomNumber = CommonMethods.GenerateRandomNumber();
            if (BarCodeExists(randomNumber))
                randomNumber = GenerateRandomBarCode();
            return randomNumber;
        }

        public bool BarCodeExists(string barcode)
        {
            var count = db.Database.SqlQuery<int>($"select COUNT(Barcode) from Items where Barcode='{barcode}'").FirstOrDefault();
            return count != 0;
        }

        public string SetBarCode(int itemBalanceID)
        {
            var barcode = GenerateRandomBarCode();
            db.Database.ExecuteSqlCommand($"UPDATE Items SET Barcode = {barcode} WHERE ID = " + itemBalanceID);
            return barcode;
        }

        private void txt_printCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            Validations.txtIsNumberOnly(e);
        }

        private async void btnFillDgvByItem_Click(object sender, EventArgs e)
        {
            dgvItems.DataSource = null;
            SplashScreen.ShowSplashScreen();
            var itemName = txtSearch.Text;
            List<ItemBalanceVM> items = null;
            await Task.Run(() => {
                var list = db.Items.Where(x => !x.IsDeleted);
                if (!string.IsNullOrEmpty(itemName))
                    list = list.Where(x => x.Name.Trim() == itemName.Trim());
                items = list.AsEnumerable().Select(x => new ItemBalanceVM
                {
                    ID = x.Id,
                    Name = x.Name,
                    Price = x.SellPrice,
                    //Quantity = balanceService.GetBalanceByItemAllStores(x.Id),
                    Barcode = x.BarCode,
                    ItemCode = x.ItemCode.ToString()
                }).ToList();
            });
            dgvItems.DataSource = items;
            SplashScreen.CloseSplashScreen();
        }




        #endregion

        #region Printing

        private void PrintBarcodes()
        {
            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                if (!((bool?)row.Cells[nameof(Choose)].Value ?? false)) continue;


                var barcode = row.Cells[nameof(Barcode)].Value + "";
                var itemCode = row.Cells[nameof(ItemCode)].Value + "";
                if (string.IsNullOrWhiteSpace(barcode))
                {
                    barcode = SetBarCode((int)row.Cells[nameof(Id)].Value);
                }
                var ItemName = row.Cells[nameof(Item_AName)].Value.ToString();
                var price = row.Cells[nameof(SalePrice)].Value.ToString();

                double printCount = 0;
                double customeCount = 0;
                if(double.TryParse(txt_printCount.Text,out customeCount))
                    if(customeCount > 0)
                        printCount = customeCount;
                   else
                        printCount = double.Parse(row.Cells[nameof(PrintCount)].Value.ToString()); 
                else
                        printCount = double.Parse(row.Cells[nameof(PrintCount)].Value.ToString());


                if (printCount <= 0||printCount==0)
                    continue;

                Report report = new Report
                {
                    ReportFont = "3 of 9 Barcode",
                    IncludeVtsFooter = false,
                    FontSize = 100
                };
                report.IncludeEntityInformation = false;
                report.IncludeVtsHeaders = false;
                report.IncludeBarcodeFont = true;
                report.BarcodeFontPath = "'" + /*Environment.CurrentDirectory.Replace("\\", "/") +*/ "F:/3OF9.TTF'";
                report.GenerateReport();
                if (rdo_itemOnly.Checked)
                    report.AddHtmlLine($"<div style='margin-top:-5px'>" +
                                   $"<div style='margin: -1px auto auto; width: 100%; text-align: center; font-size: 12pt; font-weight: bold; width: 5rem;' class='text-wrap'>" +
                                   $"<span>{ItemName}</span></div>" +
                                   $"</div>");
                else
                    report.AddHtmlLine($"<div style='margin-top:-5px'>" +
                                       "<p  style='font-size:7pt;'>" +
                                       $"<span>{Properties.Settings.Default.BarcodeName}</span>" +
                                       $"<span>{Properties.Settings.Default.BarcodePhone}</span> </p>" +
                                       $"<span class='barcode' style = 'margin:auto;font-family: IDAutomationHC39M; margin-top:-1px; display:table; font-size:9pt;'> " +
                                       $"({barcode})</span>" +
                                       $"<div style='margin: -1px auto auto;width: 100%;text-align: center;font-size:8pt;font-weight:bold;'><span>{ItemName}</span></div>" +
                                       //$"<div style='margin: -1px auto auto;width: 100%;text-align: center;font-size:8pt;font-weight:bold;'>" +
                                       //$"<table style = 'margin-left: auto;margin-right: auto;'><tbody><tr><td>L.E &nbsp; {price } </td><td>&nbsp;&nbsp;&nbsp;-  {itemCode} </td></tr></tbody></table>"+
                                       //$"</div>"+
                                       $"<table style='font-size:7pt;text-align: center;margin-left: auto;margin-right: auto;'><tr><td>L.E {price}</td><td>&nbsp;&nbsp;&nbsp; {itemCode}</td></tr></table>" +
                                       //$"<p  style='font-size: 6pt;text-align: center;font-weight: bold;'><span>{itemCode}</span></p>" +
                                       $"</div>");
                if (ckDoubled.Checked)
                {
                    report.AddHtmlLine($"<div style='margin-top:5px;'>" +
                                       "<p  style='font-size:7pt;'>" +
                                       $"<span>{Properties.Settings.Default.BarcodeName}</span>" +
                                       $"<span>{Properties.Settings.Default.BarcodePhone}</span> </p>" +
                                   $"<span class='barcode' style = 'margin:auto;font-family: IDAutomationHC39M; margin-top:-1px; display:table; font-size:14pt;'> " +
                                   $"({barcode})</span>" +
                                   $"<div style='margin: -1px auto auto;width: 100%;text-align: center;font-size:9pt;font-weight:bold;'><span>{ItemName}</span>&nbsp;&nbsp;&nbsp;&nbsp;<span>{itemCode}</span>&nbsp;&nbsp;&nbsp;&nbsp;<span>{itemCode}</span></div>" +
                                   //$"<div style='margin: -1px auto auto;width: 100%;text-align: center;font-size:8pt;font-weight:bold;'>" +
                                   //$"<table style = 'margin-left: auto;margin-right: auto;'><tbody><tr><td> {price } L.E</td><td> {itemCode} </td></tr></tbody></table>" +
                                   //$"</div>" +
                                   $"<p style='font-size:8pt;text-align: center;'><span> L.E {price}</span>&nbsp;&nbsp;&nbsp;&nbsp;<span>{itemCode}</span></p>" +
                                       //$"<p  style='font-size: 6pt;text-align: center;font-weight: bold;'><span>{itemCode}</span></p>" +
                                       $"</div>");
                }
                report.Print(cmbPrinters.Text, (ckDoubled.Checked ? (int)Math.Ceiling(printCount / 2) : (int)printCount), false);
            }

            AlrtMsgs.ShowCustomSuccess("تم الانتهاء من الطباعة", "تأكيد العملية");
        }
        #endregion
    }

    internal class ItemBalanceVM
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public double? Price { get; set; }
        public double Quantity { get; set; }
        public string Barcode { get; set; }
        public string ItemCode { get; set; }
    }
}