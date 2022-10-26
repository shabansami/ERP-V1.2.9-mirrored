using ERP.Desktop.DTOs;
using ERP.DAL;
using ERP.Desktop.Services;
using ERP.Desktop.Services.Items;
using ERP.Desktop.Utilities;
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
using ERP.Desktop.Utilities.FormManager;
using PrintEngine.HTMLPrint;

namespace ERP.Desktop.Views.Items
{
    public partial class ItemsList : BaseForm
    {
        Guid id = Guid.Empty;
        ItemsServices _services;
        VTSaleEntities db;
        TypeAssistant assistant;
        List<ItemsVM> items;
        public ItemsList()
        {
            InitializeComponent();
            db = DBContext.UnitDbContext;
            _services = new ItemsServices(db);
            assistant = new TypeAssistant();
            assistant.Idled += assistant_Idled;
        }

        #region Load and Fill
        private void ItemsList_Load(object sender, EventArgs e)
        {
            CommonMethods.AutoComplateTextbox(txtSearch);
            FillDGV();
        }

        void FillDGV()
        {
            items = _services.GetAllItems();
            dgv_Items.AutoGenerateColumns = false;
            dgv_Items.DataSource = items;
        }
        #endregion

        #region Edit and Delete
        private void dgv_Items_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            var senderGrid = (DataGridView)sender;
            id = Guid.Parse(dgv_Items.Rows[e.RowIndex].Cells[nameof(ColId)].Value + "");

            if (e.ColumnIndex == senderGrid.Columns[nameof(edit1)].Index && e.RowIndex >= 0)
            {
                id = Guid.Parse(dgv_Items.Rows[e.RowIndex].Cells[nameof(ColId)].Value + "");
                var form = new ItemAddEdit(id);
                form.ShowDialog();
                FillDGV();
            }
            else if (e.ColumnIndex == senderGrid.Columns[nameof(delete1)].Index && e.RowIndex >= 0)
            {
                Delete(id);
            }
        }

        private void Delete(Guid id)
        {
            if (MessageBox.Show("هل تريد حذف هذا الصنف", "تنبيه", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                var del = db.Items.Find(id);
                if (del.InventoryInvoiceDetails.Count(x => !x.IsDeleted) > 0 ||
                    del.ItemProductionDetails.Count(x => !x.IsDeleted) > 0 ||
                    del.ItemIntialBalances.Count(x => !x.IsDeleted) > 0 ||
                    del.ItemProductionDetails.Count(x => !x.IsDeleted) > 0 ||
                    del.MaintenanceDamageDetails.Count(x => !x.IsDeleted) > 0 ||
                    del.MaintenanceDetails.Count(x => !x.IsDeleted) > 0 ||
                    del.MaintenanceSpareParts.Count(x => !x.IsDeleted) > 0 ||
                    del.PriceInvoicesDetails.Count(x => !x.IsDeleted) > 0 ||
                    del.ProductionOrderDetails.Count(x => !x.IsDeleted) > 0 ||
                    //del.ProductionOrders.Count(x => !x.IsDeleted) > 0 ||
                    del.PurchaseBackInvoicesDetails.Count(x => !x.IsDeleted) > 0 ||
                    del.PurchaseInvoicesDetails.Count(x => !x.IsDeleted) > 0 ||
                    del.SellBackInvoicesDetails.Count(x => !x.IsDeleted) > 0 ||
                    del.SellInvoicesDetails.Count(x => !x.IsDeleted) > 0 ||
                    del.StoreAdjustments.Count(x => !x.IsDeleted) > 0 ||
                    del.ItemUnits.Count(x => !x.IsDeleted) > 0 ||
                    del.StoresTransferDetails.Count(x => !x.IsDeleted) > 0)
                {
                    AlrtMsgs.ShowMessageError("لا يمكن حذف الصنف لانه مرتبط ببيانات اخري");
                    return;
                }

                del.IsDeleted = true;
                del.DeletedBy = UserServices.UserInfo.UserId;
                del.DeletedOn = DateTime.Now;
                foreach (var itemPrice in db.ItemPrices.Where(x => !x.IsDeleted && x.ItemId == id))
                {
                    itemPrice.IsDeleted = true;
                    itemPrice.DeletedBy = UserServices.UserInfo.UserId;
                    itemPrice.DeletedOn = DateTime.Now;
                }
                db.SaveChanges(UserServices.UserInfo.UserId);
                FillDGV();
            }
        }
        #endregion

        #region Search
        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void Search()
        {
            dgv_Items.AutoGenerateColumns = false;
            if (txtSearch.Text.Length > 2)
            {
                items = _services.GetFilteredItems(txtSearch.Text);
                dgv_Items.DataSource = items;
            }
            else
            {
                items = _services.GetAllItems();
                dgv_Items.DataSource = items;
            }
        }

        void assistant_Idled(object sender, EventArgs e)
        {
            this.Invoke(
            new MethodInvoker(() =>
            {
                Search();
            }));
        }
        private void txt_Name_TextChanged(object sender, EventArgs e)
        {
            assistant.TextChanged();
        }
        #endregion

        #region Add new Item
        private void btnAdd_Click(object sender, EventArgs e)
        {
            var form = FormManager.Show<ItemAddEdit>();
        }
        #endregion

        #region Print
        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (items == null)
            {
                AlrtMsgs.ShowMessageError("لا يوجد بيانات للطباعة");
                return;
            }

            Report report = new Report();

            report.ReportTitle = "قائمة الاصناف";
            report.ReportSource = items.ToDataTable();

            report.ReportFields.Add(new Field("Name", "اسم الصنف"));
            report.ReportFields.Add(new Field("ItemTypeName", "نوع الصنف"));
            report.ReportFields.Add(new Field("GroupBasicName", "المجموعة"));
            report.ReportFields.Add(new Field("UnitName", "الوحدة"));
            report.ReportFields.Add(new Field("SellPrice", "سعر البيع"));
            report.ReportFields.Add(new Field("ItemCode", "كود الصنف"));
            report.ReportFields.Add(new Field("BarCode", "الباركود"));
            report.ReportFields.Add(new Field("AvailableToSell", "متوفر للبيع"));

            report.FontSize = 12;
            //Generate the prepReport tables
            report.GenerateReport();
            report.ShowPrintPreviewDialog();
        }
        #endregion
    }
}
