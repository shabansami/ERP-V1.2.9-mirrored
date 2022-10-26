using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class ItemsMaterialDT
    {
        public Guid Id { get; set; }
        public Guid? ItemId { get; set; }
        public string ItemName { get; set; }
        public double? ItemCost { get; set; }
        public Guid? StoreId { get; set; }
        public int? ItemCostCalculateId { get; set; }
        public string ItemCostCalculateName { get; set; }
        public string StoreName { get; set; }
        public double Quantity { get; set; } //الكمية 
        public double Quantitydamage { get; set; } //الكمية التالفة 
        public string DT_DatasourceMaterial { get; set; }
        public string Actions { get; set; }
    }
}