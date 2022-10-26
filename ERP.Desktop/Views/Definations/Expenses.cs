using ERP.Desktop.Utilities;
using ERP.DAL;
using System;
using System.Data;
using System.Windows.Forms;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using ERP.Desktop.Services;
using ERP.Desktop.Views._Main;
using System.Collections.Generic;
using ERP.Desktop.DTOs;
using ERP.Desktop.Services.Definations;
using ERP.Desktop.Services.Actors;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Desktop.Views.Definations
{
    public partial class Expenses : BaseForm
    {
        Guid? editID=null;
        public Expenses() :base()
        {
            InitializeComponent();
            StyleDataGridViews(dgv_Expens);
        }

        #region Load and Fill
        private void Expenses_Load(object sender, EventArgs e)
        {
            FillDGV();
        }

        void FillDGV()
        {
            dgv_Expens.AutoGenerateColumns = false;
            var db = DBContext.UnitDbContext;
            {
                dgv_Expens.DataSource = db.ExpenseTypes.Where(x => !x.IsDeleted).Select(x => new { x.Name, x.Id }).ToList();
            }
        }
        #endregion

        #region ClearForm
        protected override void ClearForm()
        {
            btn_Cancel.Visible = false;
            btn_Update.Visible = false;
            btn_Add.Visible = true;
            txt_Expens.Text = "";
        }
        #endregion

        #region Add New
        private void btn_Add_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_Expens.Text))
            {
                AlrtMsgs.ChooseVaildData("نوع المصروف");
                txt_Expens.Focus();
                return;
            }
            //var db = DBContext.UnitDbContext;
            //{
                ExpenseType exp = new ExpenseType();
                exp.Name = txt_Expens.Text;
                //exp.CreatedBy = 1;
                //exp.CreatedOn = DateTime.Now;
                //db.ExpenseTypes.Add(exp);
               var isSaved = AccountServices<ExpenseType>.AddAccountTree(GeneralSettingCl.AccountTreeExpensesAccount, txt_Expens.Text, AccountTreeSelectorTypesCl.Expense, exp, UserServices.UserInfo.UserId);
                if(isSaved==true)
                {
                AlrtMsgs.SaveSuccess();
                FillDGV();
                ClearForm();
                }

            //}
        }
        #endregion

        #region Edit and Delete

        private void dgv_Expens_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void Edit(int index)
        {
            btn_Cancel.Visible = true;
            btn_Update.Visible = true;
            btn_Add.Visible = false;
            txt_Expens.Text = dgv_Expens.Rows[index].Cells[nameof(ExpensName)].Value.ToString();
        }

        private void Delete(int id)
        {
            if (MessageBox.Show("هل تريد حذف نوع المصروف هذا ", "تنبيه", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                // comment by shaban sami انواع المصروفات يتم تعريفها وادارتها فى النسخة الويب لارتباطها بشجرة الحسابات
                //===============================================================================================================

                //var db = DBContext.UnitDbContext;
                //{
                //    var del = db.ExpenseTypes.Find(id);
                //    if(del.ExpenseIncomes.Count(x=>!x.IsDeleted) > 0 || 
                //        del.EmployeeReturnCustodies.Count(x=>!x.IsDeleted) > 0 || 
                //        del.ExpenseIncomes.Count(x=>!x.IsDeleted) > 0 || 
                //        del.ProductionOrderExpenses.Count(x=>!x.IsDeleted) > 0 || 
                //        del.PurchaseBackInvoicesExpenses.Count(x=>!x.IsDeleted) > 0 || 
                //        del.PurchaseInvoicesExpenses.Count(x=>!x.IsDeleted) > 0)
                //    {
                //        AlrtMsgs.ShowMessageError("لا يمكن حذف البيانات لانها مرتبطة ببيانات اخري");
                //        return;
                //    }
                //    del.IsDeleted = true;
                //    del.DeletedBy = 1;
                //    del.DeletedOn = DateTime.Now;
                //    db.SaveChanges();

                //    FillDGV();
                //}
            }
        }


        private void btn_Update_Click(object sender, EventArgs e)
        {
            SaveChanges(editID);
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
        private void SaveChanges(Guid? id)
        {
            if (string.IsNullOrEmpty(txt_Expens.Text))
            {
                AlrtMsgs.ChooseVaildData("نوع المصروف");
                txt_Expens.Focus();
                return;

            }
            var db = DBContext.UnitDbContext;
            {
                var del = db.ExpenseTypes.Find(id);
                del.Name = txt_Expens.Text;
                db.SaveChanges(UserServices.UserInfo.UserId);
                ClearForm();
                FillDGV();
            }
        }
        #endregion
    }
}
