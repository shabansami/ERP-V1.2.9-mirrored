using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class InventoryInvoiceVM
    {
        public int Id { get; set; }
        public DateTime InvoiceDate { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> StoreId { get; set; }
        public int? ItemId { get; set; }
        public string ItemName { get; set; }
        public double TotalDifferenceAmount { get; set; }
        public Nullable<int> ItemCostCalculateId { get; set; }
        public string Notes { get; set; }
        public int MyProperty { get; set; }
    }

    public class InventoryInvoiceDetailDto
    {
        public int? ItemId { get; set; }
        public string ItemName { get; set; }
        public double Balance { get; set; }
        public double BalanceReal { get; set; }
    }
}