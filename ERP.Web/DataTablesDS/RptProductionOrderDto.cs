using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class RptProductionOrderDto
    {
        public RptProductionOrderDto()
        {
            MaterialItems = new List<MaterialItem>();
        }
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public bool? IsDone { get; set; }
        public string FinalItemName { get; set; }
        public string ProductionOrderDate { get; set; }
        public double OrderQuantity { get; set; }
        public double FinalItemCost { get; set; }
        public double MaterialItemCost { get; set; }
        public double TotalCost { get; set; }
        public List<MaterialItem> MaterialItems { get; set; }
        public int? Actions { get; set; } = null;
    }
   public class MaterialItem
    {
        public string ItemName { get; set; }
        public double Quantity { get; set; }
        public double ItemCost { get; set; }
        public double Quantitydamage { get; set; }
    }
}