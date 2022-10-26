using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class ItemUnitVM
    {
        public ItemUnitVM()
        {
            ItemsDetails = new List<ItemUnitDetails>();
        }
        public Nullable<Guid> UnitId { get; set; }
        public double GeneralQuantity { get; set; }
        public double GeneralSellPrice { get; set; }
        public Nullable<Guid> ItemtypeId { get; set; }
        public List<ItemUnitDetails> ItemsDetails { get; set; }
    }
    public class ItemUnitDetails
    {
        public Guid ItemUnitId { get; set; }
        public Nullable<Guid> ItemId { get; set; }
        public string ItemName { get; set; }
        public Guid? ItemUnitBase { get; set; } //الوحدة الاساسية للصنف لمقارنتها والتأكد من عدم اختيار نفس الوحدة مرتين
        public double Quantity { get; set; }
        public double SellPrice { get; set; }
        public double SellPriceCustome { get; set; }
    }
}