using ERP.DAL;
using ERP.Web.DataTablesDS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Web.ViewModels
{
    public class OrderSellVM :ProductionOrder
    {
        public OrderSellVM()
        {
            OrderSellItems = new List<OrderSellItemsDto>();
            ProductionOrderExpens = new List<InvoiceExpensesDT>();
        }
        public Nullable<Guid> CustomerId { get; set; }
        public System.DateTime InvoiceDate { get; set; } //تاريخ امر البيع
        //public System.DateTime ProductionOrderDate { get; set; }//تاريخ امر الانتاج المجمع
        public Nullable<Guid> QuoteOrderSellId { get; set; } //عرض السعر/امر بيع

        //public Nullable<Guid> ProductionUnderStoreId { get; set; } //مخزن تحت التصنيع
        //public Nullable<Guid> ProductionStoreId { get; set; }//مخزن التصنيع النهائى
        public int? ItemCostCalculateId { get; set; }

        public List<OrderSellItemsDto> OrderSellItems { get; set; }
        public List<InvoiceExpensesDT> ProductionOrderExpens { get; set; }
        public Guid? ExpenseTypeId { get; set; }
        public double ExpenseAmount { get; set; }
        public string ExpenseNotes { get; set; }
        public Guid? ExpenseTypeIdDeleted { get; set; }
    }
    public class OrderSellItemsDto
    {
        public OrderSellItemsDto()
        {
            ItemProductionList = new List<DropDownList>();
            ItemProductionOrderDetailsIn = new List<ItemProductionOrderDetailsDT>();
            ItemProductionOrderDetailsOut = new List<ItemProductionOrderDetailsDT>();
        }
        public Guid Id { get; set; }
        public Nullable<Guid> ItemId { get; set; }
        public string ItemName { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public Nullable<int> OrderSellItemType { get; set; }//نوع اصناف امر البيع (اصناف للبيع/اصناف للانتاج
        public double CurrentBalance { get; set; }
        public Nullable<Guid> ItemProductionId { get; set; }//التوليفة المحددة فى حالة تجهيز لانتاج مجمع
        public List<DropDownList> ItemProductionList { get; set; }
        public Nullable<Guid> StoreId { get; set; }//المخزن المحدد فى حالة تجهيز لفاتورة بيع 
        public List<DropDownList> StoreItemList { get; set; }

        //انشاء امر انتاج مجمع
        public List<ItemProductionOrderDetailsDT> ItemProductionOrderDetailsIn { get; set; }
        public List<ItemProductionOrderDetailsDT> ItemProductionOrderDetailsOut { get; set; }
    }

}