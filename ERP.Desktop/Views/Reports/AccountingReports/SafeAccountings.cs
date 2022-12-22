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
using System.Data.Entity;
using ERP.Desktop.Services;
using ERP.Desktop.Services.Employees;

namespace ERP.Desktop.Views.Reports.AccountingReports
{
    public partial class SafeAccountings : BaseForm
    {
        VTSaleEntities db;
        List<AccountingVM> data;
        PointOfSale currentPOS = null;
        string rptTitle,rptAccountName;
        Guid? accountTreeId;
        public SafeAccountings()
        {
            InitializeComponent();
            db = new VTSaleEntities();
        }

        #region Load and fill
        private void SafeAccountings_Load(object sender, EventArgs e)
        {
            var posID = Properties.Settings.Default.PointOfSale;
            if (currentPOS == null)
                currentPOS = db.PointOfSales.FirstOrDefault(x => !x.IsDeleted && x.Id == posID);
            if (currentPOS == null)
            { AlrtMsgs.ShowMessageError("الرجاء تحديد نقطة البيع من إعدادات النظام"); return; }
            
            FillCombos();
            dtpFrom.Value = new DateTime(DateTime.Now.Year, 1, 1);
            dtpTo.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        }

        private void FillCombos()
        {
            var branches = EmployeeService.GetBranchesByUser(UserServices.UserInfo);
            //var branchs = db.Branches.Where(x => !x.IsDeleted).Select(x => new IDNameVM() { ID = x.Id, Name = x.Name }).ToList();
            FillComboBox(cmbBranches, branches, "Name", "ID");
            var tranTypes = db.TransactionsTypes.Where(x => !x.IsDeleted).Select(x => new IDNameIntVM() { ID = x.Id, Name = x.Name }).ToList();
            FillComboBox(cmbo_TransactionType, tranTypes, "Name", "Id");
        }
        #endregion

        #region Changing the branch
        private void cmbBranches_SelectedIndexChanged(object sender, EventArgs e)
        {
            //var branchID = cmbBranches.GetSelectedID(null);
            //var safes = branchID != null ? db.Safes.Where(x => !x.IsDeleted && x.BranchId == branchID).Select(x => new IDNameVM() { ID = x.AccountsTreeId ?? Guid.Empty, Name = x.Name }).ToList() : new List<IDNameVM>();
            //FillComboBox(cmbSafes, safes, "Name", "ID");
        }
        #endregion

        #region Search
        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void Search()
        {
            //// 0 حساب الخزينة
            /// 1 حساب الفيزا
            /// 2 حساب المحفظة
            if (cmbBranches == null || cmbBranches.SelectedIndex == 0 || cmbBranches.SelectedIndex == -1)
            {
                AlrtMsgs.ShowMessageError("لم يتم تحديد الفرع"); return;
            }
            if (cmbAccountTypes.SelectedIndex == -1)
            { 
                AlrtMsgs.ShowMessageError("لم يتم تحديد نوع الحساب"); return;
            }

            if (dtpFrom.Value > dtpTo.Value)
            {
                AlrtMsgs.ShowMessageError("الرجاء اختيار تواريخ صحيحة");
                dtpTo.Focus();
                data = null;
                return;
            }

            Guid.TryParse(cmbBranches.SelectedValue.ToString(), out Guid branchId);
            var generalDay = db.GeneralDailies.Where(x => x.AccountsTreeId == accountTreeId&&x.BranchId== branchId && !x.IsDeleted);
            int tranTypeId;
            if (int.TryParse(cmbo_TransactionType.SelectedValue.ToString(), out tranTypeId)&&tranTypeId!=0)
                generalDay = generalDay.Where(x => x.TransactionTypeId == tranTypeId);
            generalDay = generalDay.Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= dtpFrom.Value && DbFunctions.TruncateTime(x.TransactionDate) <= dtpTo.Value);
            data = generalDay.Select(x => new AccountingVM { Debit = x.Debit, Credit = x.Credit, Note = x.Notes, Date = x.TransactionDate, TransName = x.TransactionsType.Name }).ToList();
            dgvTrans.AutoGenerateColumns = false;
            dgvTrans.DataSource = data;
            var debit = data.Sum(x => x.Debit);
            var credit = data.Sum(x => x.Credit);
            lblDebit.Text = Math.Round(debit, 3).ToString();
            lblCredit.Text = Math.Round(credit, 3).ToString();
            lblBalance.Text = Math.Round(debit - credit, 3).ToString();
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
            if (cmbAccountTypes.SelectedIndex==-1||string.IsNullOrEmpty(rptTitle)||string.IsNullOrEmpty(rptAccountName)||accountTreeId==null)
            {
                AlrtMsgs.ShowMessageError("لم يتم تحديد نوع الحساب");
                data = null;
                return;
            }

            Search();

            Report report = new Report();

            report.ReportTitle = rptTitle;
            report.ReportSource = data.ToDataTable();

            report.ReportFields.Add(new Field("Debit", "دائن"));
            report.ReportFields.Add(new Field("Credit", "مدين"));
            report.ReportFields.Add(new Field("Date", "تاريخ المعاملة"));
            report.ReportFields.Add(new Field("TransName", "المعاملة"));
            report.ReportFields.Add(new Field("Note", "البيان"));

            report.FontSize = 12;
            //Generate the prepReport tables
            report.GenerateReport();
            report.AddStrings(new List<(string, string)>
            {
                ("الفرع", cmbBranches.Text),
                ("الحساب", rptAccountName),
                ("من تاريخ", dtpFrom.Value.ToShortDateString()),
                ("الي تاريخ", dtpTo.Value.ToShortDateString()) ,

            }, 2,StringLocation.AfterTitle, fontsize: 16);
            report.AddStrings(new List<(string, string)>
            {
                ("مدين", lblDebit.Text),
                ("دائن", lblCredit.Text),
                ("الرصيد", lblBalance.Text)
            }, 1, fontsize: 16);
            report.ShowPrintPreviewDialog();
        }

        #endregion

        private void cmbAccountTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbAccountTypes.SelectedIndex)
            {
                case 0:
                    accountTreeId = currentPOS.Safe?.AccountsTreeId;
                    rptAccountName = currentPOS.Safe?.AccountsTree?.AccountName;
                    rptTitle = "تقرير حساب الخزينة";
                    break;
                case 1:
                    accountTreeId = currentPOS.BankAccountCard?.AccountsTreeId;
                    rptAccountName = currentPOS.BankAccountCard?.AccountName;
                    rptTitle = "تقرير حساب الفيزا";
                    break;
                case 2:
                    accountTreeId = currentPOS.BankAccountWallet?.AccountsTreeId;
                    rptAccountName = currentPOS.BankAccountWallet?.AccountName;
                    rptTitle = "تقرير حساب المحفظة";
                    break;
                default:
                    AlrtMsgs.ShowMessageError("لم يتم تحديد نوع الحساب"); break;
            }

        }
    }

}
