using ERP.DAL;
using ERP.Desktop.Services;
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

namespace ERP.Desktop.Views.Items
{
    public partial class ItemSearchWithPrices : BaseForm
    {
        VTSaleEntities db = DBContext.UnitDbContext;
        List<ItemVM> items;
        bool isReturnItem;
        TypeAssistant assistant;

        public ItemVM SelectedItem;
        public ItemSearchWithPrices() : base()
        {
            InitializeComponent();
            this.KeyPreview = true;
            assistant = new TypeAssistant();
            assistant.Idled += assistant_Idled;
        }
        public ItemSearchWithPrices(bool returnItem = true) : this()
        {
            lblNotes.Visible = isReturnItem = returnItem;
        }

        private void ItemsSearch_Load(object sender, EventArgs e)
        {
            FillGrid();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            assistant.TextChanged();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Down)
            {
                DGItems.Focus();
            }
        }

        private void ItemsSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void ItemsSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void DGItems_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (isReturnItem && e.RowIndex > -1)
            {
                var id = Guid.Parse(DGItems.Rows[e.RowIndex].Cells[nameof(Item_ID)].Value + "");
                SelectedItem = items.FirstOrDefault(x => x.ID == id);
                this.Close();
            }
        }



        private void btn_search_Click(object sender, EventArgs e)
        {
            FillGrid();
        }


        private void DGItems_KeyDown(object sender, KeyEventArgs e)
        {
            if (isReturnItem && e.KeyData == Keys.Enter && DGItems.CurrentRow != null)
            {
                var id = Guid.Parse(DGItems.CurrentRow.Cells[nameof(Item_ID)].Value + "");
                SelectedItem = items.FirstOrDefault(x => x.ID == id);
                this.Close();
            }
        }

        public void FillGrid()
        {
            var list = db.Items.Where(x => !x.IsDeleted).Select(x => new ItemVM()
            {
                ID = x.Id,
                BarCode = x.BarCode + "",
                GroupBasicId = x.GroupBasicId ?? Guid.Empty,
                GroupBasicName = x.GroupBasic != null ? x.GroupBasic.Name : "",
                GroupSellId = x.GroupSellId ?? Guid.Empty,
                GroupSellName = x.GroupSell != null ? x.GroupSell.Name : "",
                Name = x.Name,
                ItemCode = x.ItemCode,
                Price = x.SellPrice
            });
            items = list.ToList();
            if (txtSearch.Text.Length == 0)
            {
                DGItems.AutoGenerateColumns = false;
                DGItems.DataSource = items;
            }
            else
            {
                DGItems.AutoGenerateColumns = false;
                DGItems.DataSource = list.Where(x => x.Name.Contains(txtSearch.Text) || x.BarCode.Contains(txtSearch.Text) || x.GroupBasicName.Contains(txtSearch.Text) || x.GroupSellName.Contains(txtSearch.Text) || (x.ItemCode + "").Contains(txtSearch.Text)).ToList();
            }
        }

        private void Search()
        {
            if (txtSearch.TextLength > 2)
            {
                DGItems.AutoGenerateColumns = false;
                DGItems.DataSource = items.Where(x => x.Name.Contains(txtSearch.Text) || x.BarCode.Contains(txtSearch.Text) || x.GroupBasicName.Contains(txtSearch.Text) || x.GroupSellName.Contains(txtSearch.Text) || (x.ItemCode + "").Contains(txtSearch.Text)).ToList();
            }
            else
            {
                DGItems.AutoGenerateColumns = false;
                DGItems.DataSource = items;
            }
        }
        void assistant_Idled(object sender, EventArgs e)
        {
            this.Invoke(
            new MethodInvoker(() =>
            {
                //The method we want to delay
                Search();
            }));
        }

        private void DGItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                var id = Guid.Parse(DGItems.Rows[e.RowIndex].Cells[nameof(Item_ID)].Value + "");
                var selectedItem = db.Items.FirstOrDefault(x => x.Id == id);
                if (selectedItem != null)
                {
                    lblItemName.Text = selectedItem.Name;
                    lblUnitName.Text = selectedItem.Unit?.Name;
                    dgvItemPrices.AutoGenerateColumns = false;
                    dgvItemPrices.DataSource = selectedItem.ItemPrices.Where(x=>!x.IsDeleted).Select(x=>new { Name = x.PricingPolicy.Name, Price = x.SellPrice}).ToList();
                }
            }
        }
    }
}
