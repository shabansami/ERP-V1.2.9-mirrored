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
using static ERP.Web.Utilites.Lookups;
using ERP.Desktop.Services.Employees;

namespace ERP.Desktop.Views.Definations
{
    public partial class PointOfSales : BaseForm
    {
        Guid editID =Guid.Empty;
        VTSaleEntities db;
        POSServices _services;
        public PointOfSales()
        {
            InitializeComponent();
            db = DBContext.UnitDbContext;
            _services = new POSServices(db);
            StyleDataGridViews(dgv_pos);
        }

        #region Load and fill
        private void PointOfSales_Load(object sender, EventArgs e)
        {
            FillCombobox();
            FillDGV();
        }
        void FillCombobox()
        {
            var branches = EmployeeService.GetBranchesByUser(UserServices.UserInfo);
            CommonMethods.FillComboBox(cmb_branchs, branches, "Name", "Id");
           CommonMethods.FillComboBox(cmb_bankAccount, db.BankAccounts.Where(x => !x.IsDeleted).ToList(), "AccountName", "Id");
            CommonMethods.FillComboBox(cmbo_BankWallet, db.BankAccounts.Where(x => !x.IsDeleted).ToList(), "AccountName", "Id");
            CommonMethods.FillComboBox(cmbo_BankCard, db.BankAccounts.Where(x => !x.IsDeleted).ToList(), "AccountName", "Id");

            var customers = db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)).ToList();
            CommonMethods.FillComboBox(cmbCustomers, customers, "Name", "Id");

            var suppliers = db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)).ToList();
            CommonMethods.FillComboBox(cmbSuppliers, suppliers, "Name", "Id");

            var paymentTypes = db.PaymentTypes.Where(x => !x.IsDeleted).ToList();
            CommonMethods.FillComboBox(cmbPaymentTypes, paymentTypes, "Name", "Id");

            var policies = db.PricingPolicies.Where(x => !x.IsDeleted).ToList();
            CommonMethods.FillComboBox(cmbPricePolicies, policies, "Name", "Id");
        }

        void FillDGV()
        {
            dgv_pos.AutoGenerateColumns = false;
            dgv_pos.DataSource = _services.GetAllPointOfSales();
        }
        #endregion

        #region ClearForm
        protected override void ClearForm()
        {
            ClearControls(false, txt_Point, cmbCustomers, cmbPaymentTypes, cmbPricePolicies, cmbSuppliers, cmb_bankAccount, cmbo_BankWallet, cmbo_BankCard, cmb_branchs, cmb_safe, cmb_store);
            btnCancelChanges.Visible = false;
            btnSaveChanges.Visible = false;
            btnAdd.Visible = true;
            ckCanChangeItemPrice.Checked = ckCanDiscount.Checked = false;
        }
        #endregion

        #region Add new 

        private void btnAdd_Click(object sender, EventArgs e)
        {

            if (!Guid.TryParse(cmb_branchs.SelectedValue + "", out Guid branchID) || branchID ==Guid.Empty)
            {
                AlrtMsgs.ChooseVaildData("فرع");
                cmb_branchs.Focus();
                return;

            }
            if (!Guid.TryParse(cmb_safe.SelectedValue + "", out Guid safeID) || safeID ==Guid.Empty)
            {
                AlrtMsgs.ChooseVaildData("الخزينة");
                cmb_safe.Focus();
                return;
            }           
            if (!Guid.TryParse(cmb_store.SelectedValue + "", out Guid storeId) || storeId == Guid.Empty)
            {
                AlrtMsgs.ChooseVaildData("المخزن الافتراضى");
                cmb_store.Focus();
                return;
            }
            if (!Guid.TryParse(cmb_bankAccount.SelectedValue + "", out Guid bankAccountID) || bankAccountID ==Guid.Empty)
            {
                AlrtMsgs.ChooseVaildData("الحساب البنكي");
                cmb_bankAccount.Focus();
                return;

            }
            if (!Guid.TryParse(cmbo_BankWallet.SelectedValue + "", out Guid bankWalletID) || bankWalletID ==Guid.Empty)
            {
                AlrtMsgs.ChooseVaildData("حساب المحفظة البنكية");
                cmbo_BankWallet.Focus();
                return;

            }
            if (!Guid.TryParse(cmbo_BankCard.SelectedValue + "", out Guid bankCardID) || bankCardID ==Guid.Empty)
            {
                AlrtMsgs.ChooseVaildData("حساب البطاقة البنكية");
                cmbo_BankCard.Focus();
                return;

            }
            if (string.IsNullOrWhiteSpace(txt_Point.Text))
            {
                AlrtMsgs.ChooseVaildData("اسم نقطة البيع");
                txt_Point.Focus();
                return;

            }

            DtoResult result = _services.AddNewPOS(new PointOfSale
            {
                Name = txt_Point.Text,
                SafeID = safeID,
                BrunchId = branchID,
                StoreID = storeId,
                BankAccountID = bankAccountID,
                DefaultBankWalletId = bankWalletID,
                DefaultBankCardId = bankCardID,
                DefaultCustomerID = cmbCustomers.GetSelectedID(null),
                DefaultPaymentID = cmbPaymentTypes.GetSelectedIntID(null),
                DefaultPricePolicyID = cmbPricePolicies.GetSelectedID(null),
                DefaultSupplierID = cmbSuppliers.GetSelectedID(null),
                CanDiscount = ckCanDiscount.Checked,
                CanChangePrice = ckCanChangeItemPrice.Checked,
            });

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

        #region Edit and Delete
        private void dgv_pos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var senderGrid = (DataGridView)sender;
            var id = Guid.Parse(dgv_pos.Rows[e.RowIndex].Cells[nameof(PointId)].Value.ToString());
            if (e.ColumnIndex == senderGrid.Columns[nameof(btn_edit)].Index)
            {
                Edit(id);
            }
            else if (e.ColumnIndex == senderGrid.Columns[nameof(btn_delete)].Index)
            {
                if (MessageBox.Show("هل تريد حذف هذه النقطة", "تنبيه", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    var result = _services.Delete(id);
                    if (result.IsSuccessed)
                    {
                        AlrtMsgs.DeleteSuccess();
                        ClearForm();
                        FillDGV();
                    }
                    else
                    {
                        AlrtMsgs.ShowMessageError(result.Message);
                    }
                }
            }
        }


        private void Edit(Guid id)
        {
            var pos = db.PointOfSales.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
            if (pos != null)
            {
                this.editID = id;
                btnCancelChanges.Visible = true;
                btnSaveChanges.Visible = true;
                btnAdd.Visible = false;
                cmb_bankAccount.SelectedValue = pos.BankAccountID ?? Guid.NewGuid();
                cmbo_BankWallet.SelectedValue = pos.DefaultBankWalletId ?? Guid.NewGuid();
                cmbo_BankCard.SelectedValue = pos.DefaultBankCardId ?? Guid.NewGuid();
                cmb_branchs.SelectedValue = pos.BrunchId ?? Guid.NewGuid();
                cmb_safe.SelectedValue = pos.SafeID ?? Guid.NewGuid();
                cmb_store.SelectedValue = pos.StoreID??Guid.NewGuid();
                txt_Point.Text = pos.Name;
                if (pos.DefaultCustomerID != null)
                    cmbCustomers.SelectedValue = pos.DefaultCustomerID;
                if (pos.DefaultPaymentID != null)
                    cmbPaymentTypes.SelectedValue = pos.DefaultPaymentID;
                if (pos.DefaultPricePolicyID != null)
                    cmbPricePolicies.SelectedValue = pos.DefaultPricePolicyID;
                if (pos.DefaultSupplierID != null)
                    cmbSuppliers.SelectedValue = pos.DefaultSupplierID;
                ckCanChangeItemPrice.Checked = pos.CanChangePrice;
                ckCanDiscount.Checked = pos.CanDiscount;
            }
        }

        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            if (!Guid.TryParse(cmb_branchs.SelectedValue + "", out Guid branchID) || branchID == Guid.Empty)
            {
                AlrtMsgs.ChooseVaildData("فرع");
                cmb_branchs.Focus();
                return;

            }
            if (!Guid.TryParse(cmb_safe.SelectedValue + "", out Guid safeID) || safeID == Guid.Empty)
            {
                AlrtMsgs.ChooseVaildData("الخزينة");
                cmb_safe.Focus();
                return;
            }          
            if (!Guid.TryParse(cmb_store.SelectedValue + "", out Guid storeId) || storeId == Guid.Empty)
            {
                AlrtMsgs.ChooseVaildData("المخزن الافتراضى");
                cmb_store.Focus();
                return;
            }
            if (!Guid.TryParse(cmb_bankAccount.SelectedValue + "", out Guid bankAccountID) || bankAccountID == Guid.Empty)
            {
                AlrtMsgs.ChooseVaildData("الحساب البنكي");
                cmb_bankAccount.Focus();
                return;

            }
            if (!Guid.TryParse(cmbo_BankWallet.SelectedValue + "", out Guid bankWalletID) || bankWalletID == Guid.Empty)
            {
                AlrtMsgs.ChooseVaildData("حساب المحفظة البنكية");
                cmbo_BankWallet.Focus();
                return;

            }
            if (!Guid.TryParse(cmbo_BankCard.SelectedValue + "", out Guid bankCardID) || bankCardID == Guid.Empty)
            {
                AlrtMsgs.ChooseVaildData("حساب البطاقة البنكية");
                cmbo_BankCard.Focus();
                return;

            }
            if (string.IsNullOrWhiteSpace(txt_Point.Text))
            {
                AlrtMsgs.ChooseVaildData("اسم نقطة البيع");
                txt_Point.Focus();
                return;

            }
            var pos = db.PointOfSales.Find(editID);
            pos.BrunchId = branchID;
            pos.BankAccountID = bankAccountID;
            pos.DefaultBankWalletId = bankWalletID;
            pos.DefaultBankCardId = bankCardID;
            pos.SafeID = safeID;
            pos.StoreID = storeId;
            pos.Name = txt_Point.Text;
            pos.DefaultSupplierID = cmbSuppliers.GetSelectedID(null);
            pos.DefaultCustomerID = cmbCustomers.GetSelectedID(null);
            pos.DefaultPaymentID = cmbPaymentTypes.GetSelectedIntID(null);
            pos.DefaultPricePolicyID = cmbPricePolicies.GetSelectedID(null);
            pos.CanChangePrice = ckCanChangeItemPrice.Checked;
            pos.CanDiscount = ckCanDiscount.Checked;
            db.SaveChanges(UserServices.UserInfo.UserId);
            AlrtMsgs.SaveSuccess();
            ClearForm();
            FillDGV();
        }
        private void btnCancelChanges_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        #endregion


        private void cmb_branchs_SelectedIndexChanged(object sender, EventArgs e)
        {
            var db = DBContext.UnitDbContext;
            {
                Guid.TryParse(cmb_branchs.SelectedValue + "", out Guid branchID);
                CommonMethods.FillComboBox(cmb_safe, db.Safes.Where(x => !x.IsDeleted && x.BranchId == branchID).ToList(), "Name", "Id");
                CommonMethods.FillComboBox(cmb_store, db.Stores.Where(x => !x.IsDeleted && x.BranchId == branchID&&!x.IsDamages).ToList(), "Name", "Id");
            }
        }
    }
}
