using ERP.DAL;
using ERP.Desktop.Views._Main;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ERP.Desktop.Utilities.CommonMethods;
using ERP.Desktop.Utilities;
using ERP.Desktop.ViewModels;
using PrintEngine.HTMLPrint;
using ERP.Web.Utilites;
using System.Data.Entity;

namespace ERP.Desktop.Views.Reports.ProductReports
{
    public partial class ItemSellQuantity : BaseForm
    {
        VTSaleEntities db;
        List<ItemProductionDetails> data;
        public ItemSellQuantity()
        {
            InitializeComponent();
            db = new VTSaleEntities();
        }

        #region Load and fill
        private void ItemSellQuantity_Load(object sender, EventArgs e)
        {
            FillCombos();
            dtpFrom.Value = new DateTime(DateTime.Now.Year, 1, 1);
            dtpTo.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        }

        private void FillCombos()
        {
            var customers = db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)).Select(x => new IDNameVM { ID = x.Id, Name = x.Name }).ToList();
            FillComboBox(cmb_customer, customers, "Name", "ID");
        }
        #endregion

        #region Search
        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void Search()
        {
            if (dtpFrom.Value > dtpTo.Value)
            {
                AlrtMsgs.ShowMessageError("الرجاء اختيار تواريخ صحيحة");
                dtpTo.Focus();
                data = null;
                return;
            }

            var from = dtpFrom.Value.Date;
            var to = dtpTo.Value.Date.AddDays(1);

            var sellInvoicesDetails = db.SellInvoicesDetails.Where(x => !x.IsDeleted && x.SellInvoice.IsFinalApproval);
            Guid custId;
            if (Guid.TryParse(cmb_customer.SelectedValue.ToString(), out custId) && custId != Guid.Empty)
                sellInvoicesDetails = sellInvoicesDetails.Where(x => x.SellInvoice.CustomerId == custId);
            sellInvoicesDetails = sellInvoicesDetails.Where(x => DbFunctions.TruncateTime(x.SellInvoice.InvoiceDate) >=  from&& DbFunctions.TruncateTime(x.SellInvoice.InvoiceDate) <= to);
            var ddata = sellInvoicesDetails.Select(x => new { ItemId = x.ItemId, ItemName = x.Item.Name, Quantity = x.Quantity }).GroupBy(x => new { ItemId = x.ItemId, ItemName = x.ItemName, Quantity = x.Quantity }).ToList();
            data = ddata.Select(x => new ItemProductionDetails
            {
                ItemName = x.FirstOrDefault().ItemName,
                Quantity = x.Sum(y => y.Quantity)
            }).ToList();
            dgvTrans.AutoGenerateColumns = false;
            dgvTrans.DataSource = data;
            var itemsQuantity = data.Sum(x => x.Quantity);
            lblItemsQuantity.Text = Math.Round(itemsQuantity, 3).ToString();
        }
        #endregion

        #region Printing
        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (data == null)
            {
                AlrtMsgs.ShowMessageError("لا يوجد بيانات للطباعة");
                return;
            }
            Search();

            Report report = new Report();

            report.ReportTitle = "تقرير كميات اصناف ";
            report.ReportSource = data.ToDataTable();

            report.ReportFields.Add(new Field("ItemName", "الصنف"));
            report.ReportFields.Add(new Field("Quantity", "الكمية"));

            report.FontSize = 12;
            //Generate the prepReport tables
            report.GenerateReport();
            report.AddStrings(new List<(string, string)>
            {
                ("العميل", cmb_customer.Text),
                ("من تاريخ", dtpFrom.Value.ToShortDateString()),
                ("الي تاريخ", dtpTo.Value.ToShortDateString()) ,

            }, 2, StringLocation.AfterTitle, fontsize: 16);
            report.AddStrings(new List<(string, string)>
            {
                ("اجمالى العدد", lblItemsQuantity.Text),
            }, 1, fontsize: 16);
            report.ShowPrintPreviewDialog();
        }
        #endregion
    }

}
