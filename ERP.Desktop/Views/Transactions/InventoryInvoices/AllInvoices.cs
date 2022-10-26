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
using ERP.Desktop.Services.Transactions;
using ERP.Desktop.Utilities;
using static ERP.Desktop.Utilities.Extensions;
using ERP.Desktop.ViewModels;
using PrintEngine.HTMLPrint;

namespace ERP.Desktop.Views.Transactions.InventoryInvoices
{
    public partial class AllInvoices : BaseForm
    {
        VTSaleEntities db;
        InventoryInvoicesServices _services;
        private List<InventoryInvoiceVM> invoices;

        public AllInvoices()
        {
            InitializeComponent();
            db = new VTSaleEntities();
            _services = new InventoryInvoicesServices(db);
        }

        #region Load and fill
        private void AllInvoices_Load(object sender, EventArgs e)
        {
            FillDGV();
        }

        private void FillDGV()
        {
            invoices = _services.GetAll();
            dgvInvoices.AutoGenerateColumns = false;
            dgvInvoices.DataSource = invoices;
        }
        #endregion

        #region Add new Inventory invoice Form
        private void btnAddNew_Click(object sender, EventArgs e)
        {
            var form = new NewInventoryInvoice(_services);
            form.ShowDialog();
            FillDGV();
        }
        #endregion

        #region Priting
        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (invoices == null)
            {
                AlrtMsgs.ShowMessageError("لا يوجد بيانات للطباعة");
                return;
            }

            Report report = new Report();

            report.ReportTitle = "فواير الجرد";
            report.ReportSource = invoices.ToDataTable();

            report.ReportFields.Add(new Field("InvoiceDate", "تاريخ الفاتورة"));
            report.ReportFields.Add(new Field("BranchName", "الفرع"));
            report.ReportFields.Add(new Field("StoreName", "المخزن"));
            report.ReportFields.Add(new Field("TotalDifferenceAmount", "اجمالي قيمة الفرق"));

            report.FontSize = 12;
            //Generate the prepReport tables
            report.GenerateReport();
            report.ShowPrintPreviewDialog();
        }
        #endregion

        #region Delete and show Invoice
        private void dgvInvoices_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            if(e.ColumnIndex == dgvInvoices.Columns[nameof(colShow)].Index)
            {
                if(Guid.TryParse(dgvInvoices.Rows[e.RowIndex].Cells[nameof(colGuid)].Value + "", out Guid guid))
                {
                    var result = _services.GetInventoryInvoice(guid);
                    if (result.IsSuccessed)
                    {
                        var form = new ShowInventoryInvoice(result.Object);
                        form.ShowDialog();
                    }
                    else
                    {
                        AlrtMsgs.ShowMessageError(result.Message);
                    }
                }
            }
            else if(e.ColumnIndex == dgvInvoices.Columns[nameof(colDelete)].Index)
            {
                if (AlrtMsgs.CheckDelete() == DialogResult.Yes && Guid.TryParse(dgvInvoices.Rows[e.RowIndex].Cells[nameof(colGuid)].Value +"",out Guid guid))
                {
                    var result = _services.Delete(guid);
                    if (result.IsSuccessed)
                    {
                        AlrtMsgs.DeleteSuccess();
                        FillDGV();
                    }
                    else
                    {
                        AlrtMsgs.ShowMessageError(result.Message);
                    }
                }
            }
        }
        #endregion
    }
}
