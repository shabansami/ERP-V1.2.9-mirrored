using ERP.Desktop.DTOs;
using ERP.DAL;
using ERP.Desktop.Utilities;
using ERP.Desktop.ViewModels;
using ERP.Desktop.Views._Main;
using PrintEngine.HTMLPrint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ERP.Desktop.Views.Transactions.InventoryInvoices
{
    public partial class ShowInventoryInvoice : BaseForm
    {
        InventoryInvoice inventoryInvoices;
        private List<InventoryItemVM> items;

        public ShowInventoryInvoice(InventoryInvoice inventoryInvoice)
        {
            InitializeComponent();
            this.inventoryInvoices = inventoryInvoice;
        }

        #region Load and fill
        private void ShowInventoryInvoice_Load(object sender, EventArgs e)
        {
            lblBranch.Text = inventoryInvoices.Branch?.Name;
            lblCostCalc.Text = inventoryInvoices.ItemCostCalculation.Name;
            lblDate.Text = inventoryInvoices.InvoiceDate.ToString();
            lblDiff.Text = inventoryInvoices.TotalDifferenceAmount.ToString();
            lblStore.Text = inventoryInvoices.Store.Name;

            items = inventoryInvoices.InventoryInvoiceDetails.Where(x => !x.IsDeleted).Select(x => new InventoryItemVM
            {
                Id = x.Id,
                Balance = x.Balance,
                BalanceReal = x.BalanceReal,
                ItemName = x.Item.Name,
                DifferenceAmount = x.DifferenceAmount,
                DifferenceCount = x.DifferenceCount
            }).ToList();

            dgvItems.AutoGenerateColumns = false;
            dgvItems.DataSource = items;
        }
        #endregion

        #region Printing
        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (items == null)
            {
                AlrtMsgs.ShowMessageError("لا يوجد بيانات للطباعة");
                return;
            }

            Report report = new Report();

            report.ReportTitle = "فاتورة جرد";
            report.ReportSource = items.ToDataTable();

            report.ReportFields.Add(new Field("ItemName", "الصنف"));
            report.ReportFields.Add(new Field("Balance", "الرصيد"));
            report.ReportFields.Add(new Field("BalanceReal", "الرصيد الفعلي"));
            report.ReportFields.Add(new Field("DifferenceCount", "عدد الفرق"));
            report.ReportFields.Add(new Field("DifferenceAmount", "قيمة الفرق"));

            report.FontSize = 12;
            //Generate the prepReport tables
            report.GenerateReport();

            var list = new List<(string, string)>();
            list.Add(("تاريخ الفاتورة", lblDate.Text));
            list.Add(("احتساب تكلفة الفروق", lblCostCalc.Text));
            list.Add(("الفرع", lblBranch.Text));
            list.Add(("المخزن", lblStore.Text));
            report.AddStrings(list,2,StringLocation.AfterTitle);
            report.AddStrings(new List<(string, string)>() { ("اجمالى تكلفة الفرق بين الرصيدين",lblDiff.Text) },1);
            report.ShowPrintPreviewDialog();
        }
        #endregion
    }

    
}
