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

namespace ERP.Desktop.Views.Transactions.Sales
{
    public partial class ItemProdDetails : BaseForm
    {
        Guid? itemProdID;
        VTSaleEntities db = DBContext.UnitDbContext;
        public ItemProdDetails(Guid? itemProId) :base()
        {
            InitializeComponent();
            StyleDataGridViews(dgv_Itemspro);
            itemProdID = itemProId;
        }

        #region Load and Fill
        private void ItemProdDetails_Load(object sender, EventArgs e)
        {
            FillDGV();
            if (itemProdID!=null)
            {
                var itemPro=db.ItemProductions.Where(x=>x.Id==itemProdID).FirstOrDefault();
                txt_ItemProName.Text=itemPro.Name;
                txt_customerName.Text = itemPro.Person != null ? itemPro.Person.Name : null;
            }
        }

        void FillDGV()
        {
            dgv_Itemspro.AutoGenerateColumns = false;
            dgv_Itemspro.DataSource = db.ItemProductionDetails.Where(x => !x.IsDeleted&&x.ItemProductionId==itemProdID).Select(x => new { Id=x.Id, ItemName=x.Item.Name, Quantity = x.Quantity }).ToList();
            
        }

        #endregion

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
