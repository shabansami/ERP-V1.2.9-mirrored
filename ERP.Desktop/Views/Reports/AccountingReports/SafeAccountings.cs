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

namespace ERP.Desktop.Views.Reports.AccountingReports
{
    public partial class SafeAccountings : BaseForm
    {
        VTSaleEntities db;
        List<AccountingVM> data;
        public SafeAccountings()
        {
            InitializeComponent();
            db = new VTSaleEntities();
        }

        #region Load and fill
        private void SafeAccountings_Load(object sender, EventArgs e)
        {
            FillCombos();
            dtpFrom.Value = new DateTime(DateTime.Now.Year, 1, 1);
            dtpTo.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        }

        private void FillCombos()
        {
            var branchs = db.Branches.Where(x => !x.IsDeleted).Select(x => new IDNameVM() { ID = x.Id, Name = x.Name }).ToList();
            FillComboBox(cmbBranches, branchs, "Name", "ID");
            var tranTypes = db.TransactionsTypes.Where(x => !x.IsDeleted).Select(x => new IDNameIntVM() { ID = x.Id, Name = x.Name }).ToList();
            FillComboBox(cmbo_TransactionType, tranTypes, "Name", "Id");
        }
        #endregion

        #region Changing the branch
        private void cmbBranches_SelectedIndexChanged(object sender, EventArgs e)
        {
            var branchID = cmbBranches.GetSelectedID(null);
            var safes = branchID != null ? db.Safes.Where(x => !x.IsDeleted && x.BranchId == branchID).Select(x => new IDNameVM() { ID = x.AccountsTreeId ?? Guid.Empty, Name = x.Name }).ToList() : new List<IDNameVM>();
            FillComboBox(cmbSafes, safes, "Name", "ID");
        }
        #endregion

        #region Search
        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void Search()
        {
            var safeId = cmbSafes.GetSelectedID(null);
            if (safeId == null)
            {
                AlrtMsgs.ShowMessageError("لم يتم تحديد خزينة");
                cmbSafes.DroppedDown = true;
                cmbSafes.Focus();
                data = null;
                return;
            }
            if (dtpFrom.Value > dtpTo.Value)
            {
                AlrtMsgs.ShowMessageError("الرجاء اختيار تواريخ صحيحة");
                dtpTo.Focus();
                data = null;
                return;
            }

            var generalDay = db.GeneralDailies.Where(x => x.AccountsTreeId == safeId && !x.IsDeleted);
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
            if (cmbSafes.SelectedIndex < 1)
            {
                AlrtMsgs.ShowMessageError("لم يتم تحديد خزينة");
                cmbSafes.DroppedDown = true;
                cmbSafes.Focus();
                data = null;
                return;
            }

            Search();

            Report report = new Report();

            report.ReportTitle = "تقرير حساب الخزينة ";
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
                ("الخزينة", cmbSafes.Text),
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
    }

}
