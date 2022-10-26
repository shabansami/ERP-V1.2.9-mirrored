using ERP.Desktop.Views._Main;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using ERP.Desktop.Utilities;
using ERP.Desktop.Services;
using System.Linq;
using ERP.Desktop.DTOs;
using System.Collections.Generic;
using static ERP.Desktop.Utilities.CommonMethods;
using ERP.DAL;
using ERP.Desktop.Services.Settings;
using ERP.Desktop.ViewModels;
using ERP.Desktop.Services.Employees;

namespace ERP.Desktop.Views.Settings.Shift
{
    public partial class Shifts : BaseForm
    {
        ShiftServices _services;
        EmployeeService employeeService;
        #region Constructors

        Guid id = Guid.Empty;
        public Shifts() : base()
        {
            InitializeComponent();
            _services = new ShiftServices();
            employeeService = new EmployeeService();
        }
        #endregion

        #region Load
        private void Shifts_Load(object sender, EventArgs e)
        {
            FillCombos();
            FillDGV();
            StyleDataGridViews(dgv_shift);
        }

        private void FillCombos()
        {
            var dbContext = DBContext.UnitDbContext;
            var POSs = dbContext.PointOfSales.Where(x => !x.IsDeleted).ToList();
            FillComboBox(cmbPOSs, POSs, "Name", "ID");

            //var emps = dbContext.Users.Where(x => x.IsActive && !x.IsDeleted && x.Person != null && !x.Person.IsDeleted && x.Person.Employees.Count(y => !y.IsDeleted && y.IsSaleMen) > 0)
            //    .Select(x => new IDNameVM() { ID = x.Person.Employees.FirstOrDefault().Id, Name = x.Person.Name }).ToList();
            var emps = employeeService.GetEmployees();
            FillComboBox(cmbEmps, emps, "Name", "ID");
        }
        void FillDGV()
        {
            dgv_shift.AutoGenerateColumns = false;
            dgv_shift.DataSource = _services.GetAllNotClosedShifts();
            dtpShiftDate.Value = DateTime.Now;
        }
        #endregion

        #region Add Shift
        private void btn_add_Click(object sender, EventArgs e)
        {
            if (cmbEmps.SelectedIndex < 1)
            {
                AlrtMsgs.ChooseVaildData("الموظف");
                cmbEmps.Focus();
                return;
            }
            if (cmbPOSs.SelectedIndex < 1)
            {
                AlrtMsgs.ChooseVaildData("نقطة البيع");
                cmbPOSs.Focus();
                return;
            }
            if (dtpShiftDate.Enabled && dtpShiftDate.Value < TimeNow.Date)
            {
                AlrtMsgs.ChooseVaildData("تاريخ الوردية");
                dtpShiftDate.Focus();
                return;
            }

            var shift = new ShiftsOffline()
            {
                Date = dtpShiftDate.Value,
                EmployeeID = Guid.Parse(cmbEmps.SelectedValue + ""),
                IsClosed = false,
                PointOfSaleID = Guid.Parse(cmbPOSs.SelectedValue + ""),

            };
            var result = _services.AddNewShift(shift);

            if (result.IsSuccessed)
            {
                AlrtMsgs.SaveSuccess();
                FillDGV();
                ClearForm();
            }
            else
            {
                AlrtMsgs.ErrorWhenExcute(result.Message);
            }
        }
        #endregion

        #region Clear Shifts 
        protected override void ClearForm()
        {
            cmbEmps.SelectedIndex = 0;
            cmbPOSs.SelectedIndex = 0;
            dtpShiftDate.Value = TimeNow;
        }
        #endregion

        #region close shift
        private void dgv_shift_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            if (e.ColumnIndex == dgv_shift.Columns[nameof(close)].Index)
            {
                id = Guid.Parse(dgv_shift.Rows[e.RowIndex].Cells[nameof(Shift_ID)].Value + "");

                ShiftClose shift = new ShiftClose(id);
                shift.ShowDialog();
                if (shift.IsShiftClosed)
                    FillDGV();
            }
        }
        #endregion

        #region Add New Emplyee

        private void btnAddEmployee_Click(object sender, EventArgs e)
        {
            //if (cmbEmps.DataSource != null)
            //{
            //    Employees person = new Employees(1);
            //    person.ParentForm = this;
            //    person.ShowDialog();
            //    DataTable emplyeesDT = DAL.GetData("select * from Persons where ISEmp=1 and IsDeleted = 0", CommandType.Text, null);
            //    Utilities.FillComboBox(cmbEmps, emplyeesDT, "Person_Name", "Person_ID");
            //    cmbEmps.SelectedValue = person.ReturnedEmpID;
            //}
        }
        #endregion
    }
}
