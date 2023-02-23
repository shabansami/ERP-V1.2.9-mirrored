using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class ItemDetailsDT
    {
        public Guid Id { get; set; }
        public Guid? ItemId { get; set; }
        public Guid? StoreId { get; set; }
        public Guid? ContainerId { get; set; }
        public string ItemName { get; set; }
        public double Quantity { get; set; }
        public string QuantityUnitName { get; set; }//الكمية بالوحدة الكبرى ان وجدت
        public double QuantityUnit { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public string StoreName { get; set; }
        public string ContainerName { get; set; }
        public double ItemDiscount { get; set; }
        public string ItemEntryDate { get; set; }
        public string DT_Datasource { get; set; }
        public string Actions { get; set; }
        public int? Num { get; set; }

        // seel
        public Guid? ProductionOrderId { get; set; }
        public int? IsIntial { get; set; }
        public Guid? SerialItemId { get; set; }

        //for sell invoice
        public bool? IsDiscountItemVal { get; set; }

        public double ItemDiscountPercent => ItemDiscount == 0 ? 0 : ItemDiscount / Amount * 100;
        public double Safy => Amount - ItemDiscount;

        public string ItemCode { get; set; }
        public Guid? ItemUnitsId { get; set; }//وحدة الصنف فى جدول ItemUnit (عند البيع بوحدة 
        public Guid? UnitId { get; set; }//وحدة الصنف Unit (عند البيع بوحدة 
        public double CurrentBalanceVal { get; set; }//رصيد الصنف الحالى
    }
}