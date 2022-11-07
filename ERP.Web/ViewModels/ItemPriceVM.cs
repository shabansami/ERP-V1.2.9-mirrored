using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class ItemPriceVM
    {
        public ItemPriceVM()
        {
            ItemsDetails = new List<ItemCustomers>();
        }
        public Nullable<Guid> PricingPolicyId { get; set; }
        public Nullable<Guid> PersonCategoryId { get; set; }
        public Nullable<Guid> CustomerId { get; set; }
        public Nullable<Guid> SupplierId { get; set; }
        public Nullable<Guid> ItemtypeId { get; set; }
        public bool ShowAllItems { get; set; }
        public List<ItemCustomers> ItemsDetails { get; set; }
    }

    public class ItemCustomers
    {
        public Guid ItemPriceId { get; set; }
        public Nullable<Guid> ItemId { get; set; }
        public string ItemName { get; set; }
        public Nullable<double> SellPrice { get; set; }
        public Nullable<double> SellPriceCustome { get; set; }
    }
}