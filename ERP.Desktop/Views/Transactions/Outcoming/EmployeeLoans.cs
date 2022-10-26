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
using System.Data.Entity;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Desktop.Views.Transactions.Outcoming
{
    public partial class EmployeeLoans : BaseForm
    {
        EmployeeService _services;
        VTSaleEntities db = DBContext.UnitDbContext;
        Guid editID;
        private List<ExpenseIncomeVM> employeeLoans;
        public ShiftsOffline currentShift;
        public EmployeeLoans()
        {
            InitializeComponent();
            _services = new EmployeeService();
        }

        #region Load and Fill
        private void EmployeeLoans_Load(object sender, EventArgs e)
        {
            if (LoadMainInfo())
            {
            FillCombos();
            FillEmployeeLoans();
            }else
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
            var deparments = db.Departments.Where(x => !x.IsDeleted).Select(x => new IDNameVM { ID = x.Id, Name = x.Name }).ToList();
            FillComboBox(cmbo_Department, deparments, "Name", "ID");

            var branches = db.Branches.Where(x => !x.IsDeleted).Select(x => new IDNameVM { ID = x.Id, Name = x.Name }).ToList();
            FillComboBox(cmb_branches, branches, "Name", "ID");
            cmb_branches.SelectedValue = pos.BrunchId;

            cmb_safes.SelectedValue = pos.SafeID;
        }

        private void FillEmployeeLoans()
        {
            dgv_loanEmployees.AutoGenerateColumns = false;
            var list = db.ContractLoans.Where(x => !x.IsDeleted&&x.ShiftOfflineID== currentShift.Id);
            employeeLoans = list.Select(x => new ExpenseIncomeVM { Id = x.Id, SafeName=x.Safe.Name, IsApprovalStatus = x.IsApproval?"معتمد":"غير معتمد",PaymentDate=x.LoanDate.ToString(), EmployeeName = x.ContractScheduling.Contract.Employee.Person.Name,  Notes = x.Notes, Amount = x.Amount,  ContractSchedulingName = x.ContractScheduling.Name}).ToList();
            dgv_loanEmployees.DataSource = employeeLoans;
        }

            #endregion

            #region Get the safes of a branch when selected branch changed
            private void cmbBranches_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Guid.TryParse(cmb_branches.SelectedValue + "", out Guid id))
            {
                var safes = db.Safes.Where(x => !x.IsDeleted && x.BranchId == id).Select(x => new IDNameVM { ID = x.Id, Name = x.Name }).ToList();
                FillComboBox(cmb_safes, safes, "Name", "ID");
            }
        }

        #endregion

        #region Add New
        private void btnAdd_Click(object sender, EventArgs e)
        {
            var resultValid = Validations.ValidiateControls(cmb_branches, cmbo_employee,cmbo_contractScheduling,cmbo_employee,cmbo_Department, cmb_safes, txt_amount,txt_AmountMonth,txt_numberMonths);
            if (resultValid == false)
                return;

            //التأكد من ان عدد الاقساط اقل من او يساوى عدد الاشهر المتبقية للموظف
            if (!Guid.TryParse(cmbo_contractScheduling.SelectedValue.ToString(), out Guid contractScheduling))
            {
                AlrtMsgs.ShowMessageError("تأكد من اختيار الشهر");
                return;
            }
            var contract = db.ContractSchedulings.Find(contractScheduling).Contract;
            int.TryParse(txt_numberMonths.Text, out int NumberMonths);
            int.TryParse(txt_AmountMonth.Text, out int AmountMonth);
            //الاشهر
            if (contract.ContractSalaryTypeId == (int)ContractSalaryTypeCl.Monthly)
            {
                if (!IsMonthNumberValid(NumberMonths, contract.Id))
                {
                    AlrtMsgs.ShowMessageError("عدد الاقساط اكبر من عدد الاشهر المتبقية ");
                    return;
                }
            }
            //الاسابيع
            if (contract.ContractSalaryTypeId == (int)ContractSalaryTypeCl.Weekly)
            {
                if (!IsMonthNumberValid(NumberMonths, contract.Id))
                {
                    AlrtMsgs.ShowMessageError("عدد الاقساط اكبر من عدد الاسابيع المتبقية ");
                    return;
                }
            }
            double.TryParse(txt_amount.Text, out double amount);

         var loan= new ContractLoan
            {
                ContractSchedulingId = contractScheduling,
                Amount = amount,
                AmountMonth = AmountMonth,
                NumberMonths = NumberMonths,
                Notes = txt_notes.Text,
                BranchId = cmb_branches.GetSelectedID(null),
                SafeId = cmb_safes.GetSelectedID(null),
                LoanDate = dtp_Date.Value,
                ShiftOfflineID=currentShift.Id

            };
            var result = _services.AddNew(loan,db);
            if (!result.IsSuccessed)
            {
                AlrtMsgs.ShowMessageError(result.Message);
                return;
            }
            AlrtMsgs.SaveSuccess();
            ClearForm();
            FillEmployeeLoans();
        }
        bool IsMonthNumberValid(int numberMonths, Guid? contractId)
        {
            var contractSchedulings = db.ContractSchedulings.Where(x => !x.IsDeleted && x.ContractId == contractId && !x.IsPayed).Count();
            if (numberMonths > contractSchedulings)
                return false;
            else
                return true;
        }
        #endregion

        #region Edit and Delete
        private void dgvCustomerPayments_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            editID = Guid.Parse(dgv_loanEmployees.Rows[e.RowIndex].Cells[nameof(ID)].Value.ToString());

            if (e.ColumnIndex == dgv_loanEmployees.Columns[nameof(colEdit)].Index)
            {
                Edit(editID);
            }
            else if (e.ColumnIndex == dgv_loanEmployees.Columns[nameof(colDelete)].Index)
            {
                Delete(editID);
            }
        }
        private void Edit(Guid id)
        {
            var contractLoans = db.ContractLoans.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
            if (contractLoans == null)
                return;

            btnCancel.Visible = true;
            btnSave.Visible = true;
            btnAdd.Visible = false;
            dtp_Date.Value = contractLoans.LoanDate ?? DateTime.Now;
            cmbo_Department.SelectedValue = contractLoans.ContractScheduling?.Contract?.Employee?.DepartmentId ?? Guid.Empty;
            cmbo_employee.SelectedValue = contractLoans.ContractScheduling?.Contract?.EmployeeId ?? Guid.Empty;
            cmbo_contractScheduling.SelectedValue = contractLoans?.ContractSchedulingId ?? Guid.Empty;
            cmb_branches.SelectedValue = contractLoans?.BranchId ?? Guid.Empty;
            cmb_safes.SelectedValue = contractLoans.SafeId ?? Guid.Empty;
            txt_amount.Num = contractLoans.Amount;
            txt_AmountMonth.Num = contractLoans.AmountMonth;
            txt_numberMonths.Num = contractLoans.NumberMonths;
            txt_notes.Text = contractLoans.Notes;
        }
        private void Delete(Guid id)
        {
            if (MessageBox.Show("هل تريد حذف سلفة الموظف  ", "تنبيه", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                var result = _services.Delete(id,db);
                if (result.IsSuccessed)
                {
                    FillEmployeeLoans();
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
            var resultValid = Validations.ValidiateControls(cmb_branches, cmbo_employee, cmb_safes, txt_amount);
            if (resultValid == false)
                return;
            //التأكد من ان عدد الاقساط اقل من او يساوى عدد الاشهر المتبقية للموظف
            if (!Guid.TryParse(cmbo_contractScheduling.SelectedValue.ToString(), out Guid contractScheduling))
            {
                AlrtMsgs.ShowMessageError("تأكد من اختيار الشهر");
                return;
            }
            var contract = db.ContractSchedulings.Find(contractScheduling).Contract;
            int.TryParse(txt_numberMonths.Text, out int NumberMonths);
            int.TryParse(txt_AmountMonth.Text, out int AmountMonth);
            //الاشهر
            if (contract.ContractSalaryTypeId == (int)ContractSalaryTypeCl.Monthly)
            {
                if (!IsMonthNumberValid(NumberMonths, contract.Id))
                {
                    AlrtMsgs.ShowMessageError("عدد الاقساط اكبر من عدد الاشهر المتبقية ");
                    return;
                }
            }
            //الاسابيع
            if (contract.ContractSalaryTypeId == (int)ContractSalaryTypeCl.Weekly)
            {
                if (!IsMonthNumberValid(NumberMonths, contract.Id))
                {
                    AlrtMsgs.ShowMessageError("عدد الاقساط اكبر من عدد الاسابيع المتبقية ");
                    return;
                }
            }
            double.TryParse(txt_amount.Text, out double amount);

            var contractLoan = new ContractLoan()
            {
                Id = editID,
                ContractSchedulingId = contractScheduling,
                Amount = amount,
                AmountMonth = AmountMonth,
                NumberMonths = NumberMonths,
                Notes = txt_notes.Text,
                BranchId = cmb_branches.GetSelectedID(null),
                SafeId = cmb_safes.GetSelectedID(null),
                LoanDate = dtp_Date.Value
            };
            var result = _services.Edit(contractLoan, db);
            if (!result.IsSuccessed)
            {
                AlrtMsgs.ShowMessageError(result.Message);
                return;
            }
            AlrtMsgs.SaveSuccess();
            ClearForm();
            FillEmployeeLoans();
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
            ClearControls(true, txt_amount, txt_notes, cmb_safes, dtp_Date);
            //cmbo_Department.SelectedValue = null;
            //cmbo_employee.SelectedValue = null;
            //cmbo_contractScheduling.SelectedValue = null;
        }
        #endregion

        #region Printing
        private void btnPrint_Click(object sender, EventArgs e)
        {
            Report report = new Report();

            report.ReportTitle = "تقرير السلف";
            report.ReportSource = employeeLoans.ToDataTable();

            report.ReportFields.Add(new Field("EmployeeName", "اسم الموظف"));
            report.ReportFields.Add(new Field("ContractSchedulingName", "الشهر"));
            report.ReportFields.Add(new Field("Amount", "قيمة السلفة"));
            report.ReportFields.Add(new Field("PaymentDate", "تاريخ العملية"));
            report.ReportFields.Add(new Field("Notes", "البيان"));
            report.ReportFields.Add(new Field("SafeName", "الخزينة"));
            report.ReportFields.Add(new Field("IsApprovalStatus", "حالة الاعتماد"));

            report.FontSize = 12;
            //Generate the prepReport tables
            report.GenerateReport();
            report.ShowPrintPreviewDialog();
        }
        #endregion

        private void cmbo_employee_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (Guid.TryParse(cmbo_employee.SelectedValue + "", out Guid id)&& id !=Guid.Empty)
            {
                var schedules = _services.GetContractSchedulingEmployee(id.ToString(),db);
                FillComboBox(cmbo_contractScheduling, schedules, "Name", "ID");
            }

        }

        private void cmbo_Department_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Guid.TryParse(cmbo_Department.SelectedValue + "", out Guid id)&&id!=Guid.Empty)
            {
                var employees = _services.GetEmployees(id);
                FillComboBox(cmbo_employee, employees, "Name", "ID");
                FillComboBox(cmbo_contractScheduling, new List<IDNameVM>(), "Name", "ID");
            }

        }

        private void txt_amount_KeyPress(object sender, KeyPressEventArgs e)
        {
            Validations.txtIsNumberOrDecimal(e, (Control)sender);
        }
        void CalcScheduleAmount()
        {
            double.TryParse(txt_amount.Text,out double amount);
            double.TryParse(txt_numberMonths.Text,out double numberMonths);
            txt_AmountMonth.Text = Math.Round(amount / numberMonths, 2, MidpointRounding.ToEven).ToString();
        }

        private void txt_numberMonths_KeyPress(object sender, KeyPressEventArgs e)
        {
            Validations.txtIsNumberOnly(e);

        }

        private void txt_amount_TextChanged(object sender, EventArgs e)
        {
            CalcScheduleAmount();
        }

        private void txt_numberMonths_TextChanged(object sender, EventArgs e)
        {
            CalcScheduleAmount();

        }

        private void cmbo_contractScheduling_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
