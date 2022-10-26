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

namespace ERP.Desktop.Views.Transactions.SalesBack
{
    public partial class ShowSellInvoices : BaseForm
    {
        VTSaleEntities db;
        public bool selected;
        public List<Guid> sellDetailIds = new List<Guid>();
        public ShowSellInvoices() : base()
        {
            InitializeComponent();
            StyleDataGridViews(dgv_SellInvoice);
            db = DBContext.UnitDbContext;
        }

        #region Load and Fill
        private void ShowSellInvoices_Load(object sender, EventArgs e)
        {
            txt_sellInvoiceId.Select();
            FillDGV(null);
        }

        void FillDGV(string sellInvoNum)
        {
            dgv_SellInvoice.AutoGenerateColumns = false;
            var db = DBContext.UnitDbContext;
            {
                dgv_SellInvoice.DataSource = db.SellInvoicesDetails.Where(x => !x.IsDeleted && x.SellInvoice.InvoiceNumber == sellInvoNum).Select(x => new
                {
                    Id = x.Id,
                    SellInvoiceId = x.SellInvoiceId,
                    ItemId = x.ItemId,
                    ItemName = x.Item != null ? x.Item.Name : null,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    Amount = x.Amount,
                    ItemDiscount = x.ItemDiscount

                }).ToList();
            }
        }
        #endregion


        private void txt_sellInvoiceId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                FillDGV(txt_sellInvoiceId.Text);
            }
        }


        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            if (dgv_SellInvoice.Rows.Count == 0) return;

            //Check all if nothing checked, check nothing if all is checked
            foreach (DataGridViewRow row in dgv_SellInvoice.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.GetType() == typeof(DataGridViewCheckBoxCell))
                    {

                        DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)cell;
                        chk.Value = true;
                    }
                }
            }
        }

        private void btn_Update_Click_1(object sender, EventArgs e)
        {
            //sellInvoId = Convert.ToInt32(dgv_SellInvoice.Rows[e.RowIndex].Cells["SellInvoiceId"].Value);
            //selected = true;
            //this.Close();

            foreach (DataGridViewRow row in dgv_SellInvoice.Rows)
            {
                if (!((bool?)row.Cells[nameof(selectItm)].Value ?? false)) continue;

                Guid invoId, sellDetailsId;
                if (!Guid.TryParse(row.Cells[nameof(SellInvoiceId)].Value.ToString(), out invoId))
                {
                    AlrtMsgs.ChooseVaildData("حدث خطأ .. تأكد من تحديد الاصناف");
                    sellDetailIds = new List<Guid>(); return;
                }

                if (!Guid.TryParse(row.Cells[nameof(Id)].Value.ToString(), out sellDetailsId))
                {
                    AlrtMsgs.ChooseVaildData("حدث خطأ .. تأكد من تحديد الاصناف");
                    sellDetailIds = new List<Guid>(); return;
                }

                sellDetailIds.Add(sellDetailsId);
            }

            if (sellDetailIds.Count() > 0)
            {
                selected = true;
                this.Close();
            }
            else
            {
                sellDetailIds = new List<Guid>();
                AlrtMsgs.ChooseVaildData("حدث خطأ .. تأكد من تحديد الاصناف");
                return;
            }

        }
    }
}
