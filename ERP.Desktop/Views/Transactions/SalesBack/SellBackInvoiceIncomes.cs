using ERP.DAL;
using ERP.Desktop.Services;
using ERP.Desktop.Utilities;
using ERP.Desktop.ViewModels;
using ERP.Desktop.Views._Main;
using ERP.Desktop.Views.Definations;
using ERP.Web.Services;
using ERP.Web.Utilites;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace ERP.Desktop.Views.Transactions.SalesBack
{
    public partial class SellBackInvoiceIncomes : BaseForm
    {
        Guid editingID;
        Guid invoiceID;
        public bool IsChanged = false;
        VTSaleEntities db = DBContext.UnitDbContext;
        public SellBackInvoiceIncomes(Guid invoiceId)
        {
            InitializeComponent();
            StyleDataGridViews(dgvInvoiceIncomes);
            invoiceID = invoiceId;
            TopLevel = true;
        }

        private void ExpensesInvoices_Load(object sender, EventArgs e)
        {
            FillIncomeTypes();
            FillDGV();
        }

        private void FillIncomeTypes()
        {
            //حساب الايرادات من الاعدادات
            var incomeAccountId = db.AccountsTrees.Where(x => !x.IsDeleted && x.AccountNumber == Lookups.GeneralIncomes).FirstOrDefault().Id;
            ////حسابات الايرادات التشغيلية 
            var incomeAccounts = AccountTreeService.GetAccountTreeLastChild(incomeAccountId);

            CommonMethods.FillComboBox(cmb_expenses, incomeAccounts.Where(x => !x.IsDeleted).Select(x => new IDNameVM { ID = x.Id, Name = x.AccountName }).ToList(), "Name", "ID");
        }

        private void txt_value_KeyPress(object sender, KeyPressEventArgs e)
        {
            Validations.txtIsNumberOrDecimal(e, txt_value);
        }
        void FillDGV()
        {
            var list = db.SellBackInvoiceIncomes.Where(x => !x.IsDeleted && x.SellBackInvoiceId == invoiceID).Select(x => new ExpensIncomeVM
            {
                ID = x.Id,
                Amount = x.Amount,
                TypeID = x.IncomeTypeAccountTreeId ?? Guid.Empty,
                TypeName = x.IncomeTypeAccountTree != null ? x.IncomeTypeAccountTree.AccountName : ""
            }).ToList();

            dgvInvoiceIncomes.AutoGenerateColumns = false;
            dgvInvoiceIncomes.DataSource = list;
            lbl_allval.Text = list.Sum(x => x.Amount) + "";
        }
        private void btn_add_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(txt_value.Text, out double amount))
            {
                AlrtMsgs.EnterVaildData("المبلغ");
                txt_value.Focus();
                return;
            }
            if (!Guid.TryParse(cmb_expenses.SelectedValue + "", out Guid typeID) || typeID == Guid.Empty)
            {
                AlrtMsgs.ChooseVaildData("نوع الايراد");
                cmb_expenses.Focus();
                return;
            }

            db.SellBackInvoiceIncomes.Add(new SellBackInvoiceIncome()
            {
                Amount = amount,
                IncomeTypeAccountTreeId = typeID,
                SellBackInvoiceId = invoiceID
            });
            db.SaveChanges(UserServices.UserInfo.UserId);
            IsChanged = true;
            FillDGV();
            ClearForm();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        protected override void ClearForm()
        {
            txt_value.Text = string.Empty;
            cmb_expenses.SelectedIndex = 0;
            btn_add.Visible = true;
            btn_cancel.Visible = btn_save.Visible = false;
        }

        private void dgExpensesInvoices_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 1)
                return;
            editingID = Guid.Parse(dgvInvoiceIncomes.Rows[e.RowIndex].Cells[nameof(Expense_ID)].Value.ToString());
            var senderGrid = (DataGridView)sender;
            if (e.ColumnIndex == senderGrid.Columns[nameof(edit)].Index)
            {
                txt_value.Text = dgvInvoiceIncomes.Rows[e.RowIndex].Cells[nameof(Amount)].Value.ToString();
                cmb_expenses.SelectedValue = Guid.Parse(dgvInvoiceIncomes.Rows[e.RowIndex].Cells[nameof(TypeID)].Value.ToString());
                btn_add.Visible = false;
                btn_save.Visible = true;
                btn_cancel.Visible = true;
            }
            else if (e.ColumnIndex == senderGrid.Columns[nameof(delete)].Index)
            {
                DialogResult dialog = MessageBox.Show(" هل تريد حذف؟" + dgvInvoiceIncomes.Rows[e.RowIndex].Cells[nameof(exName)].Value, "حذف", MessageBoxButtons.YesNo);
                if (dialog == DialogResult.Yes)
                {
                    var income = db.SellBackInvoiceIncomes.FirstOrDefault(x => x.Id == editingID);
                    if (income != null)
                    {
                        income.IsDeleted = true;
                        income.DeletedBy = UserServices.UserInfo.UserId;
                        income.DeletedOn = CommonMethods.TimeNow;
                        db.SaveChanges(UserServices.UserInfo.UserId);
                        IsChanged = true;
                        FillDGV();
                    }
                }

            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            if (editingID == Guid.Empty)
                return;

            if (!double.TryParse(txt_value.Text, out double amount))
            {
                AlrtMsgs.EnterVaildData("المبلغ");
                txt_value.Focus();
                return;
            }
            if (!Guid.TryParse(cmb_expenses.SelectedValue + "", out Guid typeID) || typeID == Guid.Empty)
            {
                AlrtMsgs.ChooseVaildData("نوع الايراد");
                cmb_expenses.Focus();
                return;
            }
            var income = db.SellBackInvoiceIncomes.FirstOrDefault(x => x.Id == editingID);
            if (income != null)
            {
                income.Amount = amount;
                income.IncomeTypeAccountTreeId = typeID;
                db.SaveChanges(UserServices.UserInfo.UserId);
                IsChanged = true;
                FillDGV();
                ClearForm();
            }
        }




      
    }
}
