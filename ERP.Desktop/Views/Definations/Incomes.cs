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
    public partial class Incomes : BaseForm
    {
        Guid? id =null;
        public Incomes()
        {
            InitializeComponent();
            Begin();
        }

        void FillDGV()
        {
            dgvIncomes.AutoGenerateColumns = false;
            var db = DBContext.UnitDbContext;
            {
                dgvIncomes.DataSource = db.IncomeTypes.Where(x => !x.IsDeleted).Select(x => new { x.Name, x.Id }).ToList();
            }
            StyleDataGridViews(dgvIncomes);
        }

        void Begin()
        {
            btn_Cancel.Visible = false;
            btn_Update.Visible = false;
            btn_Add.Visible = true;
            txt_Revenue.Text = "";
            FillDGV();
        }



        private void Edit(int index)
        {
            btn_Cancel.Visible = true;
            btn_Update.Visible = true;
            btn_Add.Visible = false;
            txt_Revenue.Text = dgvIncomes.Rows[index].Cells["RevName"].Value.ToString();
        }

        private void Update(Guid? id)
        {
            if (string.IsNullOrEmpty(txt_Revenue.Text))
            {
                AlrtMsgs.ChooseVaildData("ادخل نوع الايراد");
                txt_Revenue.Focus();
                return;

            }
            var db = DBContext.UnitDbContext;
            {
                var del = db.IncomeTypes.Find(id);
                del.Name = txt_Revenue.Text;
                db.SaveChanges(UserServices.UserInfo.UserId);
                AlrtMsgs.SaveSuccess();
                Begin();
            }
        }
        private void Cancel()
        {
            Begin();
        }

        private void Delete(Guid? id)
        {
            if (MessageBox.Show("هل تريد حذف هذه النقطة", "تنبيه", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                // comment by shaban sami انواع الايرادات يتم تعريفها وادارتها فى النسخة الويب لارتباطها بشجرة الحسابات
                //===============================================================================================================

                //var db = DBContext.UnitDbContext;
                //{
                //    var del = db.IncomeTypes.Find(id);
                //    if (del.ExpenseIncomes.Count(x => !x.IsDeleted) > 0 ||
                //        del.SellBackInvoiceIncomes.Count(x => !x.IsDeleted) > 0 ||
                //        del.SellInvoiceIncomes.Count(x => !x.IsDeleted) > 0)
                //    {
                //        AlrtMsgs.ShowMessageError("لا يمكن حذف البيانات لانها مرتبطة ببيانات اخري");
                //        return;
                //    }
                //    del.IsDeleted = true;
                //    del.DeletedBy = 1;
                //    del.DeletedOn = DateTime.Now;
                //    db.SaveChanges();
                //    AlrtMsgs.DeleteSuccess();

                //    Begin();
                //}
            }
        }

        private void dgv_Revenue_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            id = Guid.Parse(dgvIncomes.Rows[e.RowIndex].Cells["RevID"].Value.ToString());

            if (e.ColumnIndex == senderGrid.Columns["btn_edit"].Index && e.RowIndex >= 0)
            {
                Edit(e.RowIndex);
            }
            else if (e.ColumnIndex == senderGrid.Columns["btn_delete"].Index && e.RowIndex >= 0)
            {
                Delete(id);
            }
        }

        private void btn_Update_Click(object sender, EventArgs e)
        {
            Update(id);
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_Revenue.Text))
            {
                AlrtMsgs.ChooseVaildData("ادخل نوع الايراد");
                txt_Revenue.Focus();
                return;

            }

            //var db = DBContext.UnitDbContext;
            //{
            IncomeType exp = new IncomeType();
            exp.Name = txt_Revenue.Text;
            //exp.CreatedBy = 1;
            //exp.CreatedOn = DateTime.Now;
            //db.IncomeTypes.Add(exp);
            //db.SaveChanges();
            //AlrtMsgs.SaveSuccess();
            var  isSaved = AccountServices<IncomeType>.AddAccountTree(GeneralSettingCl.AccountTreeGeneralRevenus, exp.Name, AccountTreeSelectorTypesCl.Revenuse, exp, UserServices.UserInfo.UserId);
                if (isSaved == true)
                {
                    AlrtMsgs.SaveSuccess();
                    FillDGV();
                    ClearForm();
                    Begin();

                }


            //}

        }
    }
}
