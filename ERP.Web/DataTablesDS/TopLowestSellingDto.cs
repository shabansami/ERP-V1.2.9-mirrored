using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class TopLowestSellingDto
    {
        public Guid? ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitName { get; set; }
        public double TopSellPrice { get; set; }
        public double LowSellPrice { get; set; }
        public double Quantity { get; set; }
    }
}