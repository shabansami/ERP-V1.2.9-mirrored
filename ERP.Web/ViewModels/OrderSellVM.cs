using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public Nullable<Guid> QuoteId { get; set; }
        public string Notes { get; set; }

        public List<OrderSellItemsDto> OrderSellItems { get; set; }
    }
    public class OrderSellItemsDto
    {
        public Guid Id { get; set; }
        public Nullable<Guid> ItemId { get; set; }
        public string ItemName { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public Nullable<int> OrderSellItemType { get; set; }//نوع اصناف امر البيع (اصناف للبيع/اصناف للانتاج
        public double CurrentBalance { get; set; }
    }

}