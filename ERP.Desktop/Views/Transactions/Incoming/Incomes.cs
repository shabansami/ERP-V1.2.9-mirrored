using ERP.DAL;
using ERP.Desktop.Services;
using ERP.Desktop.Services.Transactions;
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
using ERP.Desktop.Services.Employees;
using ERP.Web.Services;
using ERP.Web.Utilites;
using EmployeeService = ERP.Desktop.Services.Employees.EmployeeService;

namespace ERP.Desktop.Views.Transactions.Incoming
{
    public partial class Incomes : BaseForm
    {
        ExpensIncomeServices _services;
        VTSaleEntities db = DBContext.UnitDbContext;
        Guid editID;
        private List<ExpenseIncomeVM> incomes;
        public ShiftsOffline currentShift;

        public Incomes()
        {
            InitializeComponent();
            _services = new ExpensIncomeServices(db);
        }

        #region Load and Fill
        private void Incomes_Load(object sender, EventArgs e)
        {
            if (LoadMainInfo())
            {
                FillCombos();
                FillIncomes();
            }
            else
                this.BeginInvoke(new MethodInvoker(Close)); //excption: System.InvalidOperationException: Value Close() cannot be called while doing CreateHandle().

        }
        private bool LoadMainInfo()
        {
            var posID = Properties.Settings.Default.PointOfSale;
            if (currentShift == null)
                currentShift = db.ShiftsOfflines.FirstOrDefault(x => !x.IsDeleted && !x.IsClosed && x.PointOfSaleID == posID);
            if (currentShift == null)
            {
                AlrtMsgs.ShowMessageError("لا يوجد وردية مفتوحة الان");
                return false;
            }
            return true;
        }

        private void FillCombos()
        {
            var pos = db.PointOfSales.FirstOrDefault(x => x.Id == Properties.Settings.Default.PointOfSale);
            //حساب الايرادات من الاعدادات
            var incomeAccountId = db.AccountsTrees.Where(x => !x.IsDeleted && x.AccountNumber == Lookups.GeneralIncomes).FirstOrDefault().Id;
            ////حسابات الايرادات التشغيلية 
            var incomeAccounts = AccountTreeService.GetAccountTreeLastChild(incomeAccountId);
            var incomeTypes = incomeAccounts.Where(x => !x.IsDeleted).Select(x => new IDNameVM { ID = x.Id , Name = x.AccountName }).ToList();
            FillComboBox(cmbIncomeTypes, incomeTypes, "Name", "ID");

            //var branches = db.Branches.Where(x => !x.IsDeleted).Select(x => new IDNameVM { ID = x.Id, Name = x.Name }).ToList();
            var branches = EmployeeService.GetBranchesByUser(UserServices.UserInfo);
            FillComboBox(cmbBranches, branches, "Name", "ID");
            cmbBranches.SelectedValue = pos.BrunchId ?? Guid.NewGuid();

            cmbSafes.SelectedValue = pos.SafeID;
        }

        private void FillIncomes()
        {
            dgvIncomes.AutoGenerateColumns = false;
            incomes = _services.GetAllIncomes(currentShift.Id);
            dgvIncomes.DataSource = incomes;
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
            var resultValid = Validations.ValidiateControls(cmbBranches, cmbIncomeTypes, cmbSafes, txtAmount);
            if (resultValid == false)
                return;


            var customePayment = new ExpenseIncome()
            {
                PaymentDate = dtpDate.Value,
                BranchId = cmbBranches.GetSelectedID(null),
                Amount = txtAmount.Num,
                Notes = txtNotes.Text,
                SafeId = cmbSafes.GetSelectedID(null),
                ExpenseIncomeTypeAccountTreeId = cmbIncomeTypes.GetSelectedID(null),
                IsExpense = false,
                ShiftOffLineID = currentShift.Id
            };
            var result = _services.AddNew(customePayment);
            if (!result.IsSuccessed)
            {
                AlrtMsgs.ShowMessageError(result.Message);
                return;
            }
            AlrtMsgs.SaveSuccess();
            ClearForm();
            FillIncomes();
        }
        #endregion

        #region Edit and Delete
        private void dgvCustomerPayments_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            editID = Guid.Parse(dgvIncomes.Rows[e.RowIndex].Cells[nameof(ID)].Value + "");

            if (e.ColumnIndex == dgvIncomes.Columns[nameof(edit)].Index)
            {
                Edit(editID);
            }
            else if (e.ColumnIndex == dgvIncomes.Columns[nameof(delete)].Index)
            {
                Delete(editID);
            }
        }
        private void Edit(Guid id)
        {
            var expenseIncome = db.ExpenseIncomes.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
            if (expenseIncome == null)
                return;

            btnCancel.Visible = true;
            btnSave.Visible = true;
            btnAdd.Visible = false;
            dtpDate.Value = expenseIncome.PaymentDate ?? DateTime.Now;
            cmbIncomeTypes.SelectedValue = expenseIncome.ExpenseIncomeTypeAccountTreeId ?? Guid.Empty;
            cmbBranches.SelectedValue = expenseIncome.BranchId ?? Guid.Empty;
            cmbSafes.SelectedValue = expenseIncome.SafeId ?? Guid.Empty;
            txtAmount.Num = expenseIncome.Amount;
            txtNotes.Text = expenseIncome.Notes;
        }
        private void Delete(Guid id)
        {
            if (MessageBox.Show("هل تريد حذف الايراد ", "تنبيه", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                var result = _services.Delete(id);
                if (result.IsSuccessed)
                {
                    FillIncomes();
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
            var resultValid = Validations.ValidiateControls(cmbBranches, cmbIncomeTypes, cmbSafes, txtAmount);
            if (resultValid == false)
                return;

            var expenseIncome = new ExpenseIncome()
            {
                Id = editID,
                PaymentDate = dtpDate.Value,
                BranchId = cmbBranches.GetSelectedID(null),
                Amount = txtAmount.Num,
                Notes = txtNotes.Text,
                SafeId = cmbSafes.GetSelectedID(null),
                ExpenseIncomeTypeAccountTreeId = cmbIncomeTypes.GetSelectedID(null),
                IsExpense = false
            };
            var result = _services.Edit(expenseIncome);
            if (!result.IsSuccessed)
            {
                AlrtMsgs.ShowMessageError(result.Message);
                return;
            }
            AlrtMsgs.SaveSuccess();
            ClearForm();
            FillIncomes();
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
            ClearControls(true, txtAmount, txtNotes, cmbBranches, cmbIncomeTypes, cmbSafes, dtpDate);
            FillCombos();
        }
        #endregion

        #region Printing
        private void btnPrint_Click(object sender, EventArgs e)
        {
            Report report = new Report();

            report.ReportTitle = "تقرير ايرادات";
            report.ReportSource = incomes.ToDataTable();

            report.ReportFields.Add(new Field("IncomeTypeName", "نوع الايراد"));
            report.ReportFields.Add(new Field("Amount", "المبلغ"));
            report.ReportFields.Add(new Field("PaymentDate", "تاريخ العملية"));
            report.ReportFields.Add(new Field("BranchName", "الفرع"));
            report.ReportFields.Add(new Field("SafeName", "الخزينة"));
            report.ReportFields.Add(new Field("IsApprovalStatus", "معتمد"));

            report.FontSize = 12;
            //Generate the prepReport tables
            report.GenerateReport();
            report.ShowPrintPreviewDialog();
        }
        #endregion
    }
}
