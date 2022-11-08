using ERP.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Web.ViewModels
{
    public class ItemStockBalanceVM
    {
        public ItemStockBalanceVM()
        {
            ItemStocks = new List<ItemStock>();
            ParemeterReportList = new List<int>();
        }
        public List<ItemStock> ItemStocks { get; set; }
        public List<int> ParemeterReportList { get; set; }
        public DateTime? dtFrom { get; set; }
        public DateTime? dtTo { get; set; }
        public Guid? ItemId { get; set; }
        public Guid? ItemGroup { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? StoreId { get; set; }
        public int? CostCalculationPurchaseId { get; set; } = 1; //احتساب تكلفة الصنف شراء
        public int? CostCalculationSellId { get; set; } = 1;//احتساب تكلفة الصنف بيع
        public SelectList listItems { get; set; }
    }
}