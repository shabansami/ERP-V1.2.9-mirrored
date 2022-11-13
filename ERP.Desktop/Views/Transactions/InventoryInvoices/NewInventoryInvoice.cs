
using ERP.Desktop.DTOs;
using ERP.DAL;
using ERP.Desktop.Services;
using ERP.Desktop.Services.Transactions;
using ERP.Desktop.Utilities;
using ERP.Desktop.ViewModels;
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
using ERP.Desktop.Services.Employees;

namespace ERP.Desktop.Views.Transactions.InventoryInvoices
{
    public partial class NewInventoryInvoice : BaseForm
    {
        InventoryInvoicesServices _services;
        VTSaleEntities db;
        bool isOpenFromAllInvoicesForm = false;

        /// <summary>
        /// Open from the main form
        /// </summary>
        public NewInventoryInvoice()
        {
            InitializeComponent();
            db = DBContext.UnitDbContext;
            _services = new InventoryInvoicesServices(db);
        }

        /// <summary>
        /// open from all invoices
        /// </summary>
        /// <param name="invoicesServices">Inventory Invoices Services Object</param>
        public NewInventoryInvoice(InventoryInvoicesServices invoicesServices)
        {
            InitializeComponent();
            this._services = invoicesServices;
            db = invoicesServices.db;
            isOpenFromAllInvoicesForm = true;
        }


        #region Load and Fill
        private void NewInventoryInvoice_Load(object sender, EventArgs e)
        {
            if (_services == null)
            {
                AlrtMsgs.ShowMessageError("خطأ");
                Close();
                return;
            }
            FillCombos();
        }

        private void FillCombos()
        {
            var groups = db.Groups.Where(x => !x.IsDeleted).Select(x => new IDNameVM { ID = x.Id, Name = x.Name }).ToList();
            FillComboBox(cmbGroups, groups, "Name", "ID");
            var itemTypes = db.ItemTypes.Where(x => !x.IsDeleted).Select(x => new IDNameVM { ID = x.Id, Name = x.Name }).ToList();
            FillComboBox(cmbItemTypes, itemTypes, "Name", "ID");
            var branches = EmployeeService.GetBranchesByUser(UserServices.UserInfo);
            //var branchs = db.Branches.Where(x => !x.IsDeleted).Select(x => new IDNameVM { ID = x.Id, Name = x.Name }).ToList();
            FillComboBox(cmbBranchs, branches, "Name", "ID");
            var costCalcs = db.ItemCostCalculations.Where(x => !x.IsDeleted).Select(x => new IDNameIntVM { ID = x.Id, Name = x.Name }).ToList();
            FillComboBox(cmbCalcMethods, costCalcs, "Name", "ID");
        }
        #endregion

        #region Search Selection Changed
        private void cmbItemTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            var itemTypeID = cmbItemTypes.GetSelectedID(null);
            var items = db.Items.Where(x => !x.IsDeleted && x.ItemTypeId == itemTypeID).ToList();
            FillComboBox(cmbItems, items, "Name", "ID");
        }

        private void cmbBranchs_SelectedIndexChanged(object sender, EventArgs e)
        {
            var branchID = cmbBranchs.GetSelectedID(null);
            var stores = db.Stores.Where(x => !x.IsDeleted && x.BranchId == branchID).ToList();
            FillComboBox(cmbStores, stores, "Name", "ID");
        }
        #endregion

        #region Search
        private async void btnSearch_Click(object sender, EventArgs e)
        {
            btnSearch.Enabled = false;
            if (Validations.ValidiateControls(cmbBranchs, cmbStores))
            {
                SplashScreen.ShowSplashScreen();
                var groupID = cmbGroups.GetSelectedID(null);
                var itemTypeID = cmbItemTypes.GetSelectedID(null);
                var itemID = cmbItems.GetSelectedID(null);
                var branchID = cmbBranchs.GetSelectedID(null);
                var storeID = cmbStores.GetSelectedID(null);
                string itemCode = txtItemCode.Text, itemBarcode = txtItemBarcode.Text;
                List<ItemBalanceDto> resulT = null;
                await Task.Run(() =>
                {
                    resulT = _services.SearchItemBalances(itemCode, itemBarcode, groupID, itemTypeID, itemID, branchID, storeID);
                });

                dgvItems.AutoGenerateColumns = false;
                dgvItems.DataSource = resulT;
                SplashScreen.CloseSplashScreen();
            }
            btnSearch.Enabled = true;
        }
        #endregion

        #region Add new
        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("هل تريد حفظ الفاتورة؟", "حفظ فاتورة جرد جديدة", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            if (dgvItems.Rows.Count == 0)
            {
                AlrtMsgs.ShowMessageError("لا يوجد اصناف للحفظ");
                return;
            }

            if (Validations.ValidiateControls(cmbBranchs, cmbStores, cmbCalcMethods))
            {

                List<ItemBalanceDto> list = new List<ItemBalanceDto>();
                bool isThereIsZeroBalanse = false;
                foreach (DataGridViewRow row in dgvItems.Rows)
                {
                    if (!double.TryParse(row.Cells[nameof(colRealBalance)].Value + "", out double realBalance))
                    {
                        AlrtMsgs.ShowMessageError("خطأ في قيمة رصيد فعلي مدخلة");
                        row.Cells[nameof(colRealBalance)].Value = 0;
                        dgvItems.CurrentCell = row.Cells[nameof(colRealBalance)];
                        dgvItems.BeginEdit(false);
                        return;
                    }

                    if (realBalance == 0)
                        isThereIsZeroBalanse = true;

                    list.Add(new ItemBalanceDto()
                    {
                        Id = Guid.Parse(row.Cells[nameof(colID)].Value + ""),
                        Balance = double.Parse(row.Cells[nameof(colBalance)].Value + ""),
                        BalanceReal = realBalance
                    });
                }

                if (isThereIsZeroBalanse && MessageBox.Show("يوجد رصيد قيمته 0 هل تريد اكمال العملية؟", "التأكد من ادخال الرصيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;

                var branchID = cmbBranchs.GetSelectedID(null);
                var storeID = cmbStores.GetSelectedID(null);
                var cost = cmbCalcMethods.GetSelectedIntID(null);
                var date = dtpInvoiceDate.Value;
                var notes = txtNotes.Text;

                DtoResultObj<InventoryInvoice> result = null;

                await Task.Run(() => { result = _services.AddNew(date, branchID, storeID, cost, notes, list); });

                if (!result.IsSuccessed)
                {
                    AlrtMsgs.ShowMessageError(result.Message);
                }
                else
                {
                    AlrtMsgs.SaveSuccess();
                    if (isOpenFromAllInvoicesForm)
                        Close();
                    else
                        ClearForm();
                }
            }
        }
        #endregion

        #region 
        protected override void ClearForm()
        {
            dgvItems.DataSource = null;
            ClearControls(false, txtItemBarcode, txtItemCode, txtNotes, cmbBranchs, cmbCalcMethods, cmbGroups, cmbItemTypes, cmbItems, cmbStores);
        }
        #endregion

    }
}
