using ERP.Web.DataTablesDS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Web.ViewModels
{
    public class OrderSellVM
    {
        public OrderSellVM()
        {
            OrderSellItems = new List<OrderSellItemsDto>();
        }
        public Guid Id { get; set; }
        public Nullable<Guid> BranchId { get; set; }
        public Nullable<Guid> CustomerId { get; set; }
        public System.DateTime InvoiceDate { get; set; }
        public Nullable<Guid> QuoteOrderSellId { get; set; } //عرض السعر/امر بيع
        public string Notes { get; set; }

        public List<OrderSellItemsDto> OrderSellItems { get; set; }
    }
    public class OrderSellItemsDto
    {
        public OrderSellItemsDto()
        {
            ItemProductionList = new List<DropDownList>();
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
    }

}