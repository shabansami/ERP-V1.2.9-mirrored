using ERP.DAL;
using ERP.Desktop.Services;
using ERP.Desktop.Services.Transactions;
using ERP.Desktop.Utilities;
using ERP.Desktop.ViewModels;
using ERP.Desktop.Views._Main;
using ERP.Web.Utilites;
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
using static ERP.Desktop.Utilities.CommonMethods;

namespace ERP.Desktop.Views.Transactions.Incoming
{
    public partial class CustomerPayments : BaseForm
    {
        Guid editID;

        CustomerPaymentServices _services;
        VTSaleEntities db = DBContext.UnitDbContext;
        List<CustomerPaymentVm> customerPayments;
        public CustomerPayments()
        {
            InitializeComponent();
            _services = new CustomerPaymentServices(db);
            //StyleDataGridView(dgvCustomerPayments);
        }

        #region Load and Fill
        private void CustomerPayments_Load(object sender, EventArgs e)
        {
            FillCombos();
            FillCustomersPayments();
        }
        private void FillCombos()
        {
            var pos = db.PointOfSales.FirstOrDefault(x => x.Id == Properties.Settings.Default.PointOfSale);
            var customers = db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)).Select(x => new IDNameVM { ID = x.Id, Name = x.Name }).ToList();
            FillComboBox(cmbCustomers, customers, "Name", "ID");

            var branches = db.Branches.Where(x => !x.IsDeleted).Select(x => new IDNameVM { ID = x.Id, Name = x.Name }).ToList();
            FillComboBox(cmbBranches, branches, "Name", "ID");
            if (pos != null)
            {
                cmbBranches.SelectedValue = pos.Store.BranchId;

                cmbSafes.SelectedValue = pos.SafeID;
            }
        }

        private void FillCustomersPayments()
        {
            dgvCustomerPayments.AutoGenerateColumns = false;
            customerPayments = _services.GetAll();
            dgvCustomerPayments.DataSource = customerPayments;
        }

        #endregion

        #region Get the safes of a branch when selected branch changed
        private void cmbBranches_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Guid.TryParse(cmbBranches.SelectedValue + "", out Guid id))
            {
                var safes = db.Safes.Where(x => !x.IsDeleted && x.BranchId == id).Select(x => new IDNameVM { ID = x.Id, Name = x.Name }).ToList();
                FillComboBox(cmbSafes, safes, "Name", "ID");
            }
        }

        #endregion

        #region Add New
        private void btnAdd_Click(object sender, EventArgs e)
        {
            var resultValid = Validations.ValidiateControls(cmbBranches, cmbCustomers, cmbSafes, txtAmount);
            if (resultValid == false)
                return;

            Guid? invoID = null;
            if (Guid.TryParse(txtInvoNumber.Text, out Guid id))
            {
                invoID = id;

                //TODO: invoice number
            }

            var customePayment = new CustomerPayment()
            {
                PaymentDate = dtpDate.Value,
                BranchId = cmbBranches.GetSelectedID(null),
                Amount = txtAmount.Num,
                Notes = txtNotes.Text,
                SafeId = cmbSafes.GetSelectedID(null),
                SellInvoiceId = invoID,
                CustomerId = cmbCustomers.GetSelectedID(null),
            };
            var result = _services.AddNew(customePayment);
            if (!result.IsSuccessed)
            {
                AlrtMsgs.ShowMessageError(result.Message);
                return;
            }
            AlrtMsgs.SaveSuccess();
            ClearForm();
            FillCustomersPayments();
        }
        #endregion

        #region Edit and Delete
        private void dgvCustomerPayments_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            editID = Guid.Parse(dgvCustomerPayments.Rows[e.RowIndex].Cells[nameof(ID)].Value + "");

            if (e.ColumnIndex == dgvCustomerPayments.Columns[nameof(edit)].Index)
            {
                Edit(editID);
            }
            else if (e.ColumnIndex == dgvCustomerPayments.Columns[nameof(delete)].Index)
            {
                Delete(editID);
            }
        }
        private void Edit(Guid id)
        {
            var payment = db.CustomerPayments.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
            if (payment == null)
                return;

            btnCancel.Visible = true;
            btnSave.Visible = true;
            btnAdd.Visible = false;
            dtpDate.Value = payment.PaymentDate ?? DateTime.Now;
            cmbCustomers.SelectedValue = payment.CustomerId ?? Guid.Empty;
            txtInvoNumber.Text = payment.SellInvoiceId + "";
            cmbBranches.SelectedValue = payment.BranchId ?? Guid.Empty;
            cmbSafes.SelectedValue = payment.SafeId ?? Guid.Empty;
            txtAmount.Num = payment.Amount;
            txtNotes.Text = payment.Notes;
        }
        private void Delete(Guid id)
        {
            if (MessageBox.Show("هل تريد حذف دفعة العميل هذه ", "تنبيه", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                var result = _services.Delete(id);
                if (result.IsSuccessed)
                {
                    FillCustomersPayments();
                    AlrtMsgs.DeleteSuccess();
                }
                else
                {
                    AlrtMsgs.ShowMessageError(result.Message);
                }
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            var resultValid = Validations.ValidiateControls(cmbBranches, cmbCustomers, cmbSafes, txtAmount);
            if (resultValid == false)
                return;

            Guid? invoID = null;
            if (Guid.TryParse(txtInvoNumber.Text, out Guid id))
            {
                invoID = id;
                //TODO: InvoiceNumber
            }

            var customePayment = new CustomerPayment()
            {
                Id = editID,
                PaymentDate = dtpDate.Value,
                BranchId = cmbBranches.GetSelectedID(null),
                Amount = txtAmount.Num,
                Notes = txtNotes.Text,
                SafeId = cmbSafes.GetSelectedID(null),
                SellInvoiceId = invoID,
                CustomerId = cmbCustomers.GetSelectedID(null),
            };
            var result = _services.Edit(customePayment);
            if (!result.IsSuccessed)
            {
                AlrtMsgs.ShowMessageError(result.Message);
                return;
            }
            AlrtMsgs.SaveSuccess();
            ClearForm();
            FillCustomersPayments();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
        #endregion

        #region Clear Form
        protected override void ClearForm()
        {
            btnAdd.Visible = true;
            btnCancel.Visible = false;
            btnSave.Visible = false;
            editID = Guid.Empty;
            ClearControls(true, txtAmount, txtInvoNumber, txtNotes, cmbBranches, cmbCustomers, cmbSafes, dtpDate);
        }
        #endregion

        #region Validations
        private void txtInvoNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            Validations.txtIsNumberOnly(e);
        }


        #endregion

        #region Printing
        private void btnPrint_Click(object sender, EventArgs e)
        {
            Report report = new Report();

            report.ReportTitle = "تقرير دفعات عميل ";
            report.ReportSource = customerPayments.ToDataTable();

            report.ReportFields.Add(new Field("CustomerName", "اسم العميل"));
            report.ReportFields.Add(new Field("Amount", "المبلغ"));
            report.ReportFields.Add(new Field("PaymentDate", "تاريخ العملية"));
            report.ReportFields.Add(new Field("BranchName", "الفرع"));
            report.ReportFields.Add(new Field("SafeName", "الخزينة"));
            report.ReportFields.Add(new Field("EmployeeName", "المندوب"));
            report.ReportFields.Add(new Field("IsApprovalStatus", "حالة الفاتورة"));
            report.ReportFields.Add(new Field("InvoType", "نوع الفاتورة"));

            report.FontSize = 12;
            //Generate the prepReport tables
            report.GenerateReport();
            report.ShowPrintPreviewDialog();
        }
        #endregion

    }
}
